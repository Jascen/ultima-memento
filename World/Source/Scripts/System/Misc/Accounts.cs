using Server.Accounting;
using Server.Commands;
using Server.Engines.Help;
using Server.Misc;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Regions;
using Server;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Security;
using System.Text;
using System.Xml;
using System;

namespace Server.Misc
{
	public class AccountPrompt
	{
		public static void Initialize()
		{
			if ( Accounts.Count == 0 && !Core.Service )
			{
				Console.WriteLine( "This server has no accounts." );
				Console.Write( "Do you want to create the owner account now? (y/n)" );

				if( Console.ReadKey( true ).Key == ConsoleKey.Y )
				{
					Console.WriteLine();

					Console.Write( "Username: " );
					string username = Console.ReadLine();

					Console.Write( "Password: " );
					string password = Console.ReadLine();

					Account a = new Account( username, password );
					a.AccessLevel = AccessLevel.Owner;

					Console.WriteLine( "Account created." );
				}
				else
				{
					Console.WriteLine();

					Console.WriteLine( "Account not created." );
				}
			}
		}
	}
}

namespace Server
{
	public class AccessRestrictions
	{
		public static void Initialize()
		{
			EventSink.SocketConnect += new SocketConnectEventHandler( EventSink_SocketConnect );
		}
			
		private static void EventSink_SocketConnect( SocketConnectEventArgs e )
		{
			try
			{
				IPAddress ip = ((IPEndPoint)e.Socket.RemoteEndPoint).Address;

				if ( Firewall.IsBlocked( ip ) )
				{
					Console.WriteLine( "Client: {0}: Firewall blocked connection attempt.", ip );
					e.AllowConnection = false;
					return;
				}
				else if ( IPLimiter.SocketBlock && !IPLimiter.Verify( ip ) )
				{
					Console.WriteLine( "Client: {0}: Past IP limit threshold", ip );

					using ( StreamWriter op = new StreamWriter( "ipLimits.log", true ) )
						op.WriteLine( "{0}\tPast IP limit threshold\t{1}", ip, DateTime.Now );
	
					e.AllowConnection = false;
					return;
				}
			}
			catch
			{
				e.AllowConnection = false;
			}
		}
	}
}

namespace Server.Accounting
{
	public class AccountAttackLimiter
	{
		public static bool Enabled = true;

		public static void Initialize()
		{
			if ( !Enabled )
				return;

			PacketHandlers.RegisterThrottler( 0x80, new ThrottlePacketCallback( Throttle_Callback ) );
			PacketHandlers.RegisterThrottler( 0x91, new ThrottlePacketCallback( Throttle_Callback ) );
			PacketHandlers.RegisterThrottler( 0xCF, new ThrottlePacketCallback( Throttle_Callback ) );
		}

		public static bool Throttle_Callback( NetState ns )
		{
			InvalidAccountAccessLog accessLog = FindAccessLog( ns );

			if ( accessLog == null )
				return true;

			return ( DateTime.Now >= (accessLog.LastAccessTime + ComputeThrottle( accessLog.Counts )) );
		}

		private static List<InvalidAccountAccessLog> m_List = new List<InvalidAccountAccessLog>();

		public static InvalidAccountAccessLog FindAccessLog( NetState ns )
		{
			if ( ns == null )
				return null;

			IPAddress ipAddress = ns.Address;

			for ( int i = 0; i < m_List.Count; ++i )
			{
				InvalidAccountAccessLog accessLog = m_List[i];

				if ( accessLog.HasExpired )
					m_List.RemoveAt( i-- );
				else if ( accessLog.Address.Equals( ipAddress ) )
					return accessLog;
			}

			return null;
		}

		public static void RegisterInvalidAccess( NetState ns )
		{
			if ( ns == null || !Enabled )
				return;

			InvalidAccountAccessLog accessLog = FindAccessLog( ns );

			if ( accessLog == null )
				m_List.Add( accessLog = new InvalidAccountAccessLog( ns.Address ) );

			accessLog.Counts += 1;
			accessLog.RefreshAccessTime();

			if ( accessLog.Counts >= 3 ) {
				try {
					using ( StreamWriter op = new StreamWriter( "throttle.log", true ) ) {
						op.WriteLine(
							"{0}\t{1}\t{2}",
							DateTime.Now,
							ns,
							accessLog.Counts
						);
					}
				}
				catch {
				}
			}
		}

		public static TimeSpan ComputeThrottle( int counts )
		{
			if ( counts >= 15 )
				return TimeSpan.FromMinutes( 5.0 );

			if ( counts >= 10 )
				return TimeSpan.FromMinutes( 1.0 );

			if ( counts >= 5 )
				return TimeSpan.FromSeconds( 20.0 );

			if ( counts >= 3 )
				return TimeSpan.FromSeconds( 10.0 );

			if ( counts >= 1 )
				return TimeSpan.FromSeconds( 2.0 );

			return TimeSpan.Zero;
		}
	}

	public class InvalidAccountAccessLog
	{
		private IPAddress m_Address;
		private DateTime m_LastAccessTime;
		private int m_Counts;

		public IPAddress Address
		{
			get{ return m_Address; }
			set{ m_Address = value; }
		}

		public DateTime LastAccessTime
		{
			get{ return m_LastAccessTime; }
			set{ m_LastAccessTime = value; }
		}

		public bool HasExpired
		{
			get{ return ( DateTime.Now >= ( m_LastAccessTime + TimeSpan.FromHours( 1.0 ) ) ); }
		}

		public int Counts
		{
			get{ return m_Counts; }
			set{ m_Counts = value; }
		}

		public void RefreshAccessTime()
		{
			m_LastAccessTime = DateTime.Now;
		}

		public InvalidAccountAccessLog( IPAddress address )
		{
			m_Address = address;
			RefreshAccessTime();
		}
	}
}

namespace Server.Accounting
{
	public class AccountComment
	{
		private string m_AddedBy;
		private string m_Content;
		private DateTime m_LastModified;

		/// <summary>
		/// A string representing who added this comment.
		/// </summary>
		public string AddedBy
		{
			get{ return m_AddedBy; }
		}

		/// <summary>
		/// Gets or sets the body of this comment. Setting this value will reset LastModified.
		/// </summary>
		public string Content
		{
			get{ return m_Content; }
			set{ m_Content = value; m_LastModified = DateTime.Now; }
		}

		/// <summary>
		/// The date and time when this account was last modified -or- the comment creation time, if never modified.
		/// </summary>
		public DateTime LastModified
		{
			get{ return m_LastModified; }
		}

		/// <summary>
		/// Constructs a new AccountComment instance.
		/// </summary>
		/// <param name="addedBy">Initial AddedBy value.</param>
		/// <param name="content">Initial Content value.</param>
		public AccountComment( string addedBy, string content )
		{
			m_AddedBy = addedBy;
			m_Content = content;
			m_LastModified = DateTime.Now;
		}

		/// <summary>
		/// Deserializes an AccountComment instance from an xml element.
		/// </summary>
		/// <param name="node">The XmlElement instance from which to deserialize.</param>
		public AccountComment( XmlElement node )
		{
			m_AddedBy = Utility.GetAttribute( node, "addedBy", "empty" );
			m_LastModified = Utility.GetXMLDateTime( Utility.GetAttribute( node, "lastModified" ), DateTime.Now );
			m_Content = Utility.GetText( node, "" );
		}

		/// <summary>
		/// Serializes this AccountComment instance to an XmlTextWriter.
		/// </summary>
		/// <param name="xml">The XmlTextWriter instance from which to serialize.</param>
		public void Save( XmlTextWriter xml )
		{
			xml.WriteStartElement( "comment" );

			xml.WriteAttributeString( "addedBy", m_AddedBy );

			xml.WriteAttributeString( "lastModified", XmlConvert.ToString( m_LastModified, XmlDateTimeSerializationMode.Local ) );

			xml.WriteString( m_Content );

			xml.WriteEndElement();
		}
	}
}

namespace Server.Misc
{
	public enum PasswordProtection
	{
		None,
		Crypt,
		NewCrypt
	}

	public class AccountHandler
	{
		private static int MaxAccountsPerIP = 10;

		private static TimeSpan DeleteDelay = TimeSpan.FromDays( MyServerSettings.DeleteDelay() );

		public static PasswordProtection ProtectPasswords = PasswordProtection.NewCrypt;

		private static AccessLevel m_LockdownLevel;

		public static AccessLevel LockdownLevel
		{
			get{ return m_LockdownLevel; }
			set{ m_LockdownLevel = value; }
		}

		public static bool AutoAccountCreation()
		{
			return MySettings.S_AutoAccounts;
		}

		private static CityInfo[] StartingCities = new CityInfo[]
			{
				new CityInfo( "Britain",		"",	1075079, 3649,	1257,	0  ),
				new CityInfo( "Fawn",			"",	1075072, 2361,	297,	0  ),
				new CityInfo( "Moon",			"",	1075073, 505,	833,	0  ),
				new CityInfo( "Yew",			"",	1075074, 2609,	1041,	0  ),
				new CityInfo( "Devil Guard",	"",	1075075, 1745,	2073,	0  ),
				new CityInfo( "Death Gulch",	"",	1075077, 4705,	2041,	0  ),
				new CityInfo( "Grey",			"",	1075078, 609,	2809,	0  ),
				new CityInfo( "Montor",			"",	1075076, 3849,	3649,	0  )
			};

		private static bool PasswordCommandEnabled = true;

		public static void Initialize()
		{
			EventSink.DeleteRequest += new DeleteRequestEventHandler( EventSink_DeleteRequest );
			EventSink.AccountLogin += new AccountLoginEventHandler( EventSink_AccountLogin );
			EventSink.GameLogin += new GameLoginEventHandler( EventSink_GameLogin );

			if ( PasswordCommandEnabled )
				CommandSystem.Register( "Password", AccessLevel.Player, new CommandEventHandler( Password_OnCommand ) );
		}

		[Usage( "Password <newPassword> <repeatPassword>" )]
		[Description( "Changes the password of the commanding players account." )]
		public static void Password_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			Account acct = from.Account as Account;

			if ( acct == null )
				return;

			IPAddress[] accessList = acct.LoginIPs;

			if ( accessList.Length == 0 )
				return;

			NetState ns = from.NetState;

			if ( ns == null )
				return;

			if ( e.Length == 0 )
			{
				from.SendMessage( "You must specify the new password." );
				return;
			}
			else if ( e.Length == 1 )
			{
				from.SendMessage( "To prevent potential typing mistakes, you must type the password twice. Use the format:" );
				from.SendMessage( "Password \"(newPassword)\" \"(repeated)\"" );
				return;
			}

			string pass = e.GetString( 0 );
			string pass2 = e.GetString( 1 );

			if ( pass != pass2 )
			{
				from.SendMessage( "The passwords do not match." );
				return;
			}

			bool isSafe = true;

			for ( int i = 0; isSafe && i < pass.Length; ++i )
				isSafe = ( pass[i] >= 0x20 && pass[i] < 0x80 );

			if ( !isSafe )
			{
				from.SendMessage( "That is not a valid password." );
				return;
			}

			try
			{
				acct.SetPassword( pass );
				from.SendMessage( "The password to your account has changed." );
			}
			catch
			{
			}
		}

		private static void EventSink_DeleteRequest( DeleteRequestEventArgs e )
		{
			NetState state = e.State;
			int index = e.Index;

			Account acct = state.Account as Account;

			if ( acct == null )
			{
				state.Dispose();
			}
			else if ( index < 0 || index >= acct.Length )
			{
				state.Send( new DeleteResult( DeleteResultType.BadRequest ) );
				state.Send( new CharacterListUpdate( acct ) );
			}
			else
			{
				Mobile m = acct[index];

				if ( m == null )
				{
					state.Send( new DeleteResult( DeleteResultType.CharNotExist ) );
					state.Send( new CharacterListUpdate( acct ) );
				}
				else if ( m.NetState != null )
				{
					state.Send( new DeleteResult( DeleteResultType.CharBeingPlayed ) );
					state.Send( new CharacterListUpdate( acct ) );
				}
				else if ( DateTime.Now < (m.CreationTime + DeleteDelay) )
				{
					state.Send( new DeleteResult( DeleteResultType.CharTooYoung ) );
					state.Send( new CharacterListUpdate( acct ) );
				}
				else if ( m.AccessLevel == AccessLevel.Player && Region.Find( m.LogoutLocation, m.LogoutMap ).GetRegion( typeof( Jail ) ) != null )	//Don't need to check current location, if netstate is null, they're logged out
				{
					state.Send( new DeleteResult( DeleteResultType.BadRequest ) );
					state.Send( new CharacterListUpdate( acct ) );
				}
				else
				{
					Console.WriteLine( "Client: {0}: Deleting character {1} (0x{2:X})", state, index, m.Serial.Value );

					acct.Comments.Add( new AccountComment( "System", String.Format( "Character #{0} {1} deleted by {2}", index + 1, m, state ) ) );

					m.Delete();

					state.Send( new CharacterListUpdate( acct ) );
				}
			}
		}

		public static bool CanCreate( IPAddress ip )
		{
			if ( !IPTable.ContainsKey( ip ) )
				return true;

			return ( IPTable[ip] < MaxAccountsPerIP );
		}

		private static Dictionary<IPAddress, Int32> m_IPTable;

		public static Dictionary<IPAddress, Int32> IPTable
		{
			get
			{
				if ( m_IPTable == null )
				{
					m_IPTable = new Dictionary<IPAddress, Int32>();

					foreach ( Account a in Accounts.GetAccounts() )
						if ( a.LoginIPs.Length > 0 )
						{
							IPAddress ip = a.LoginIPs[0];

							if ( m_IPTable.ContainsKey( ip ) )
								m_IPTable[ip]++;
							else
								m_IPTable[ip] = 1;
						}
				}

				return m_IPTable;
			}
		}	

		private static Account CreateAccount( NetState state, string un, string pw )
		{
			if ( un.Length == 0 || pw.Length == 0 )
				return null;

			bool isSafe = true;

			for ( int i = 0; isSafe && i < un.Length; ++i )
				isSafe = ( un[i] >= 0x20 && un[i] < 0x80 );

			for ( int i = 0; isSafe && i < pw.Length; ++i )
				isSafe = ( pw[i] >= 0x20 && pw[i] < 0x80 );

			if ( !isSafe )
				return null;

			if ( !CanCreate( state.Address ) )
			{
				Console.WriteLine( "Login: {0}: Account '{1}' not created, ip already has {2} account{3}.", state, un, MaxAccountsPerIP, MaxAccountsPerIP == 1 ? "" : "s" );
				return null;
			}

			Console.WriteLine( "Login: {0}: Creating new account '{1}'", state, un );

			Account a = new Account( un, pw );

			return a;
		}

		public static void EventSink_AccountLogin( AccountLoginEventArgs e )
		{
			if ( !IPLimiter.SocketBlock && !IPLimiter.Verify( e.State.Address ) )
			{
				e.Accepted = false;
				e.RejectReason = ALRReason.InUse;

				Console.WriteLine( "Login: {0}: Past IP limit threshold", e.State );

				using ( StreamWriter op = new StreamWriter( "ipLimits.log", true ) )
					op.WriteLine( "{0}\tPast IP limit threshold\t{1}", e.State, DateTime.Now );

				return;
			}

			string un = e.Username;
			string pw = e.Password;

			e.Accepted = false;
			Account acct = Accounts.GetAccount( un ) as Account;

			if ( acct == null )
			{
				if ( AutoAccountCreation() && un.Trim().Length > 0 )	//To prevent someone from making an account of just '' or a bunch of meaningless spaces 
				{
					e.State.Account = acct = CreateAccount( e.State, un, pw );
					e.Accepted = acct == null ? false : acct.CheckAccess( e.State );

					if ( !e.Accepted )
						e.RejectReason = ALRReason.BadComm;
				}
				else
				{
					Console.WriteLine( "Login: {0}: Invalid username '{1}'", e.State, un );
					e.RejectReason = ALRReason.Invalid;
				}
			}
			else if ( !acct.HasAccess( e.State ) )
			{
				Console.WriteLine( "Login: {0}: Access denied for '{1}'", e.State, un );
				e.RejectReason = ( m_LockdownLevel > AccessLevel.Player ? ALRReason.BadComm : ALRReason.BadPass );
			}
			else if ( !acct.CheckPassword( pw ) )
			{
				Console.WriteLine( "Login: {0}: Invalid password for '{1}'", e.State, un );
				e.RejectReason = ALRReason.BadPass;
			}
			else if ( acct.Banned )
			{
				Console.WriteLine( "Login: {0}: Banned account '{1}'", e.State, un );
				e.RejectReason = ALRReason.Blocked;
			}
			else
			{
				Console.WriteLine( "Login: {0}: Valid credentials for '{1}'", e.State, un );
				e.State.Account = acct;
				e.Accepted = true;

				acct.LogAccess( e.State );
			}

			if ( !e.Accepted )
				AccountAttackLimiter.RegisterInvalidAccess( e.State );
		}

		public static void EventSink_GameLogin( GameLoginEventArgs e )
		{
			if ( !IPLimiter.SocketBlock && !IPLimiter.Verify( e.State.Address ) )
			{
				e.Accepted = false;

				Console.WriteLine( "Login: {0}: Past IP limit threshold", e.State );

				using ( StreamWriter op = new StreamWriter( "ipLimits.log", true ) )
					op.WriteLine( "{0}\tPast IP limit threshold\t{1}", e.State, DateTime.Now );

				return;
			}

			string un = e.Username;
			string pw = e.Password;

			Account acct = Accounts.GetAccount( un ) as Account;

			if ( acct == null )
			{
				e.Accepted = false;
			}
			else if ( !acct.HasAccess( e.State ) )
			{
				Console.WriteLine( "Login: {0}: Access denied for '{1}'", e.State, un );
				e.Accepted = false;
			}
			else if ( !acct.CheckPassword( pw ) )
			{
				Console.WriteLine( "Login: {0}: Invalid password for '{1}'", e.State, un );
				e.Accepted = false;
			}
			else if ( acct.Banned )
			{
				Console.WriteLine( "Login: {0}: Banned account '{1}'", e.State, un );
				e.Accepted = false;
			}
			else
			{
				acct.LogAccess( e.State );

				Console.WriteLine( "Login: {0}: Account '{1}' at character list", e.State, un );
				e.State.Account = acct;
				e.Accepted = true;
				e.CityInfo = StartingCities;
			}

			if ( !e.Accepted )
				AccountAttackLimiter.RegisterInvalidAccess( e.State );
		}
	}
}

namespace Server.Accounting
{
	public class Accounts
	{
		private static Dictionary<string, IAccount> m_Accounts = new Dictionary<string, IAccount>();

		public static void Configure()
		{
			EventSink.WorldLoad += new WorldLoadEventHandler( Load );
			EventSink.WorldSave += new WorldSaveEventHandler( Save );
		}

		static Accounts()
		{
		}

		public static int Count { get { return m_Accounts.Count; } }

		public static ICollection<IAccount> GetAccounts()
		{
#if !MONO
			return m_Accounts.Values;
#else
			return new List<IAccount>( m_Accounts.Values );
#endif
		}

		public static IAccount GetAccount( string username )
		{
			IAccount a;

			m_Accounts.TryGetValue( username, out a );

			return a;
		}

		public static void Add( IAccount a )
		{
			m_Accounts[a.Username] = a;
		}
		
		public static void Remove( string username )
		{
			m_Accounts.Remove( username );
		}

		public static void Load()
		{
			m_Accounts = new Dictionary<string, IAccount>( 32, StringComparer.OrdinalIgnoreCase );

			string filePath = Path.Combine( "Saves/Accounts", "accounts.xml" );

			if ( !File.Exists( filePath ) )
				return;

			XmlDocument doc = new XmlDocument();
			doc.Load( filePath );

			XmlElement root = doc["accounts"];

			foreach ( XmlElement account in root.GetElementsByTagName( "account" ) )
			{
				try
				{
					Account acct = new Account( account );
				}
				catch
				{
					Console.WriteLine( "Warning: Account instance load failed" );
				}
			}
		}

		public static void Save( WorldSaveEventArgs e )
		{
			if ( !Directory.Exists( "Saves/Accounts" ) )
				Directory.CreateDirectory( "Saves/Accounts" );

			string filePath = Path.Combine( "Saves/Accounts", "accounts.xml" );

			using ( StreamWriter op = new StreamWriter( filePath ) )
			{
				XmlTextWriter xml = new XmlTextWriter( op );

				xml.Formatting = Formatting.Indented;
				xml.IndentChar = '\t';
				xml.Indentation = 1;

				xml.WriteStartDocument( true );

				xml.WriteStartElement( "accounts" );

				xml.WriteAttributeString( "count", m_Accounts.Count.ToString() );

				foreach ( Account a in GetAccounts() )
					a.Save( xml );

				xml.WriteEndElement();

				xml.Close();
			}
		}
	}
}

namespace Server.Accounting
{
	public class Account : IAccount, IComparable, IComparable<Account>
	{
                private int m_Gold;
                public int Gold { get { return m_Gold; } set { m_Gold = value; }}
		public static readonly TimeSpan YoungDuration = TimeSpan.FromHours( 40.0 );

		public static readonly TimeSpan InactiveDuration = TimeSpan.FromDays( 180.0 );

		private string m_Username, m_PlainPassword, m_CryptPassword, m_NewCryptPassword;
		private AccessLevel m_AccessLevel;
		private int m_Flags;
		private DateTime m_Created, m_LastLogin;
		private TimeSpan m_TotalGameTime;
		private List<AccountComment> m_Comments;
		private List<AccountTag> m_Tags;
		private Mobile[] m_Mobiles;
		private string[] m_IPRestrictions;
		private IPAddress[] m_LoginIPs;
		private HardwareInfo m_HardwareInfo;

		public bool TrackIPAddresses = false; // SET TO false TO TRACK ONLE 1 IP IN accounts.xml

		/// <summary>
		/// Deletes the account, all characters of the account, and all houses of those characters
		/// </summary>
		public void Delete()
		{
			for ( int i = 0; i < this.Length; ++i )
			{
				Mobile m = this[i];

				if ( m == null )
					continue;

				List<BaseHouse> list = BaseHouse.GetHouses( m );

				for ( int j = 0; j < list.Count; ++j )
					list[j].Delete();

				m.Delete();

				m.Account = null;
				m_Mobiles[i] = null;
			}

			Accounts.Remove( m_Username );
		}

		/// <summary>
		/// Object detailing information about the hardware of the last person to log into this account
		/// </summary>
		public HardwareInfo HardwareInfo
		{
			get { return m_HardwareInfo; }
			set { m_HardwareInfo = value; }
		}

		/// <summary>
		/// List of IP addresses for restricted access. '*' wildcard supported. If the array contains zero entries, all IP addresses are allowed.
		/// </summary>
		public string[] IPRestrictions
		{
			get { return m_IPRestrictions; }
			set { m_IPRestrictions = value; }
		}

		/// <summary>
		/// List of IP addresses which have successfully logged into this account.
		/// </summary>
		public IPAddress[] LoginIPs
		{
			get { return m_LoginIPs; }
			set { m_LoginIPs = value; }
		}

		/// <summary>
		/// List of account comments. Type of contained objects is AccountComment.
		/// </summary>
		public List<AccountComment> Comments
		{
			get { if ( m_Comments == null ) m_Comments = new List<AccountComment>(); return m_Comments; }
		}

		/// <summary>
		/// List of account tags. Type of contained objects is AccountTag.
		/// </summary>
		public List<AccountTag> Tags
		{
			get { if ( m_Tags == null ) m_Tags = new List<AccountTag>(); return m_Tags; }
		}

		/// <summary>
		/// Account username. Case insensitive validation.
		/// </summary>
		public string Username
		{
			get { return m_Username; }
			set { m_Username = value; }
		}

		/// <summary>
		/// Account password. Plain text. Case sensitive validation. May be null.
		/// </summary>
		public string PlainPassword
		{
			get { return m_PlainPassword; }
			set { m_PlainPassword = value; }
		}

		/// <summary>
		/// Account password. Hashed with MD5. May be null.
		/// </summary>
		public string CryptPassword
		{
			get { return m_CryptPassword; }
			set { m_CryptPassword = value; }
		}

		/// <summary>
		/// Account username and password hashed with SHA1. May be null.
		/// </summary>
		public string NewCryptPassword
		{
			get { return m_NewCryptPassword; }
			set { m_NewCryptPassword = value; }
		}

		/// <summary>
		/// Initial AccessLevel for new characters created on this account.
		/// </summary>
		public AccessLevel AccessLevel
		{
			get { return m_AccessLevel; }
			set { m_AccessLevel = value; }
		}

		/// <summary>
		/// Internal bitfield of account flags. Consider using direct access properties (Banned, Young), or GetFlag/SetFlag methods
		/// </summary>
		public int Flags
		{
			get { return m_Flags; }
			set { m_Flags = value; }
		}

		/// <summary>
		/// Gets or sets a flag indiciating if this account is banned.
		/// </summary>
		public bool Banned
		{
			get
			{
				bool isBanned = GetFlag( 0 );

				if ( !isBanned )
					return false;

				DateTime banTime;
				TimeSpan banDuration;

				if ( GetBanTags( out banTime, out banDuration ) )
				{
					if ( banDuration != TimeSpan.MaxValue && DateTime.Now >= ( banTime + banDuration ) )
					{
						SetUnspecifiedBan( null ); // clear
						Banned = false;
						return false;
					}
				}

				return true;
			}
			set { SetFlag( 0, value ); }
		}

		/// <summary>
		/// Gets or sets a flag indicating if the characters created on this account will have the young status.
		/// </summary>
		public bool Young
		{
			get { return !GetFlag( 1 ); }
			set
			{
				SetFlag( 1, !value );

				if ( m_YoungTimer != null )
				{
					m_YoungTimer.Stop();
					m_YoungTimer = null;
				}
			}
		}

		/// <summary>
		/// The date and time of when this account was created.
		/// </summary>
		public DateTime Created
		{
			get { return m_Created; }
		}

		/// <summary>
		/// Gets or sets the date and time when this account was last accessed.
		/// </summary>
		public DateTime LastLogin
		{
			get { return m_LastLogin; }
			set { m_LastLogin = value; }
		}

		/// <summary>
		/// An account is considered inactive based upon LastLogin and InactiveDuration
		/// </summary>
		public bool Inactive
		{
			get { return ( ( m_LastLogin + InactiveDuration ) <= DateTime.Now && AccessLevel == AccessLevel.Player ); }
		}

		/// <summary>
		/// Gets the total game time of this account, also considering the game time of characters
		/// that have been deleted.
		/// </summary>
		public TimeSpan TotalGameTime
		{
			get
			{
				for ( int i = 0; i < m_Mobiles.Length; i++ )
				{
					PlayerMobile m = m_Mobiles[i] as PlayerMobile;

					if ( m != null && m.NetState != null )
						return m_TotalGameTime + ( DateTime.Now - m.SessionStart );
				}

				return m_TotalGameTime;
			}
		}

		/// <summary>
		/// Gets the value of a specific flag in the Flags bitfield.
		/// </summary>
		/// <param name="index">The zero-based flag index.</param>
		public bool GetFlag( int index )
		{
			return ( m_Flags & ( 1 << index ) ) != 0;
		}

		/// <summary>
		/// Sets the value of a specific flag in the Flags bitfield.
		/// </summary>
		/// <param name="index">The zero-based flag index.</param>
		/// <param name="value">The value to set.</param>
		public void SetFlag( int index, bool value )
		{
			if ( value )
				m_Flags |= ( 1 << index );
			else
				m_Flags &= ~( 1 << index );
		}

		/// <summary>
		/// Adds a new tag to this account. This method does not check for duplicate names.
		/// </summary>
		/// <param name="name">New tag name.</param>
		/// <param name="value">New tag value.</param>
		public void AddTag( string name, string value )
		{
			Tags.Add( new AccountTag( name, value ) );
		}

		/// <summary>
		/// Removes all tags with the specified name from this account.
		/// </summary>
		/// <param name="name">Tag name to remove.</param>
		public void RemoveTag( string name )
		{
			for ( int i = Tags.Count - 1; i >= 0; --i )
			{
				if ( i >= Tags.Count )
					continue;

				AccountTag tag = Tags[i];

				if ( tag.Name == name )
					Tags.RemoveAt( i );
			}
		}

		/// <summary>
		/// Modifies an existing tag or adds a new tag if no tag exists.
		/// </summary>
		/// <param name="name">Tag name.</param>
		/// <param name="value">Tag value.</param>
		public void SetTag( string name, string value )
		{
			for ( int i = 0; i < Tags.Count; ++i )
			{
				AccountTag tag = Tags[i];

				if ( tag.Name == name )
				{
					tag.Value = value;
					return;
				}
			}

			AddTag( name, value );
		}

		/// <summary>
		/// Gets the value of a tag -or- null if there are no tags with the specified name.
		/// </summary>
		/// <param name="name">Name of the desired tag value.</param>
		public string GetTag( string name )
		{
			for ( int i = 0; i < Tags.Count; ++i )
			{
				AccountTag tag = Tags[i];

				if ( tag.Name == name )
					return tag.Value;
			}

			return null;
		}

		public void SetUnspecifiedBan( Mobile from )
		{
			SetBanTags( from, DateTime.MinValue, TimeSpan.Zero );
		}

		public void SetBanTags( Mobile from, DateTime banTime, TimeSpan banDuration )
		{
			if ( from == null )
				RemoveTag( "BanDealer" );
			else
				SetTag( "BanDealer", from.ToString() );

			if ( banTime == DateTime.MinValue )
				RemoveTag( "BanTime" );
			else
				SetTag( "BanTime", XmlConvert.ToString( banTime, XmlDateTimeSerializationMode.Local ) );

			if ( banDuration == TimeSpan.Zero )
				RemoveTag( "BanDuration" );
			else
				SetTag( "BanDuration", banDuration.ToString() );
		}

		public bool GetBanTags( out DateTime banTime, out TimeSpan banDuration )
		{
			string tagTime = GetTag( "BanTime" );
			string tagDuration = GetTag( "BanDuration" );

			if ( tagTime != null )
				banTime = Utility.GetXMLDateTime( tagTime, DateTime.MinValue );
			else
				banTime = DateTime.MinValue;

			if ( tagDuration == "Infinite" )
			{
				banDuration = TimeSpan.MaxValue;
			}
			else if ( tagDuration != null )
			{
				banDuration = Utility.ToTimeSpan( tagDuration );
			}
			else
			{
				banDuration = TimeSpan.Zero;
			}

			return ( banTime != DateTime.MinValue && banDuration != TimeSpan.Zero );
		}

		private static MD5CryptoServiceProvider m_MD5HashProvider;
		private static SHA1CryptoServiceProvider m_SHA1HashProvider;
		private static byte[] m_HashBuffer;

		public static string HashMD5( string phrase )
		{
			if ( m_MD5HashProvider == null )
				m_MD5HashProvider = new MD5CryptoServiceProvider();

			if ( m_HashBuffer == null )
				m_HashBuffer = new byte[256];

			int length = Encoding.ASCII.GetBytes( phrase, 0, phrase.Length > 256 ? 256 : phrase.Length, m_HashBuffer, 0 );
			byte[] hashed = m_MD5HashProvider.ComputeHash( m_HashBuffer, 0, length );

			return BitConverter.ToString( hashed );
		}

		public static string HashSHA1( string phrase )
		{
			if ( m_SHA1HashProvider == null )
				m_SHA1HashProvider = new SHA1CryptoServiceProvider();

			if ( m_HashBuffer == null )
				m_HashBuffer = new byte[256];

			int length = Encoding.ASCII.GetBytes( phrase, 0, phrase.Length > 256 ? 256 : phrase.Length, m_HashBuffer, 0 );
			byte[] hashed = m_SHA1HashProvider.ComputeHash( m_HashBuffer, 0, length );

			return BitConverter.ToString( hashed );
		}

		public void SetPassword( string plainPassword )
		{
			switch ( AccountHandler.ProtectPasswords )
			{
				case PasswordProtection.None:
					{
						m_PlainPassword = plainPassword;
						m_CryptPassword = null;
						m_NewCryptPassword = null;

						break;
					}
				case PasswordProtection.Crypt:
					{
						m_PlainPassword = null;
						m_CryptPassword = HashMD5( plainPassword );
						m_NewCryptPassword = null;

						break;
					}
				default: // PasswordProtection.NewCrypt
					{
						m_PlainPassword = null;
						m_CryptPassword = null;
						m_NewCryptPassword = HashSHA1( m_Username + plainPassword );

						break;
					}
			}
		}

		public bool CheckPassword( string plainPassword )
		{
			bool ok;
			PasswordProtection curProt;

			if ( m_PlainPassword != null )
			{
				ok = ( m_PlainPassword == plainPassword );
				curProt = PasswordProtection.None;
			}
			else if ( m_CryptPassword != null )
			{
				ok = ( m_CryptPassword == HashMD5( plainPassword ) );
				curProt = PasswordProtection.Crypt;
			}
			else
			{
				ok = ( m_NewCryptPassword == HashSHA1( m_Username + plainPassword ) );
				curProt = PasswordProtection.NewCrypt;
			}

			if ( ok && curProt != AccountHandler.ProtectPasswords )
				SetPassword( plainPassword );

			return ok;
		}

		private Timer m_YoungTimer;

		public static void Initialize()
		{
			EventSink.Connected += new ConnectedEventHandler( EventSink_Connected );
			EventSink.Disconnected += new DisconnectedEventHandler( EventSink_Disconnected );
			EventSink.Login += new LoginEventHandler( EventSink_Login );
		}

		private static void EventSink_Connected( ConnectedEventArgs e )
		{
			Account acc = e.Mobile.Account as Account;

			if ( acc == null )
				return;

			if ( acc.Young && acc.m_YoungTimer == null )
			{
				acc.m_YoungTimer = new YoungTimer( acc );
				acc.m_YoungTimer.Start();
			}
		}

		private static void EventSink_Disconnected( DisconnectedEventArgs e )
		{
			Account acc = e.Mobile.Account as Account;

			if ( acc == null )
				return;

			if ( acc.m_YoungTimer != null )
			{
				acc.m_YoungTimer.Stop();
				acc.m_YoungTimer = null;
			}

			PlayerMobile m = e.Mobile as PlayerMobile;
			if ( m == null )
				return;

			acc.m_TotalGameTime += DateTime.Now - m.SessionStart;
		}

		private static void EventSink_Login( LoginEventArgs e )
		{
			PlayerMobile m = e.Mobile as PlayerMobile;

			if ( m == null )
				return;

			Account acc = m.Account as Account;

			if ( acc == null )
				return;

			if ( m.Young && acc.Young )
			{
				TimeSpan ts = YoungDuration - acc.TotalGameTime;
				int hours = Math.Max( (int) ts.TotalHours, 0 );

				m.SendAsciiMessage( "You will enjoy the benefits and relatively safe status of a young player for {0} more hour{1}.", hours, hours != 1 ? "s" : "" );
			}
		}

		public void RemoveYoungStatus( int message )
		{
			this.Young = false;

			for ( int i = 0; i < m_Mobiles.Length; i++ )
			{
				PlayerMobile m = m_Mobiles[i] as PlayerMobile;

				if ( m != null && m.Young )
				{
					m.Young = false;

					if ( m.NetState != null )
					{
						if ( message > 0 )
							m.SendLocalizedMessage( message );

						m.SendLocalizedMessage( 1019039 );
					}
				}
			}
		}

		public void CheckYoung()
		{
			if ( TotalGameTime >= YoungDuration )
				RemoveYoungStatus( 1019038 ); // You are old enough to be considered an adult, and have outgrown your status as a young player!
		}

		private class YoungTimer : Timer
		{
			private Account m_Account;

			public YoungTimer( Account account )
				: base( TimeSpan.FromMinutes( 1.0 ), TimeSpan.FromMinutes( 1.0 ) )
			{
				m_Account = account;

				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				m_Account.CheckYoung();
			}
		}

		public Account( string username, string password )
		{
			m_Username = username;
			
			SetPassword( password );

			m_AccessLevel = AccessLevel.Player;

			m_Created = m_LastLogin = DateTime.Now;
			m_TotalGameTime = TimeSpan.Zero;

			m_Mobiles = new Mobile[7];

			m_IPRestrictions = new string[0];
			m_LoginIPs = new IPAddress[0];

			Accounts.Add( this );
		}

		public Account( XmlElement node )
		{
			m_Username = Utility.GetText( node["username"], "empty" );

			string plainPassword = Utility.GetText( node["password"], null );
			string cryptPassword = Utility.GetText( node["cryptPassword"], null );
			string newCryptPassword = Utility.GetText( node["newCryptPassword"], null );

			switch ( AccountHandler.ProtectPasswords )
			{
				case PasswordProtection.None:
					{
						if ( plainPassword != null )
							SetPassword( plainPassword );
						else if ( newCryptPassword != null )
							m_NewCryptPassword = newCryptPassword;
						else if ( cryptPassword != null )
							m_CryptPassword = cryptPassword;
						else
							SetPassword( "empty" );

						break;
					}
				case PasswordProtection.Crypt:
					{
						if ( cryptPassword != null )
							m_CryptPassword = cryptPassword;
						else if ( plainPassword != null )
							SetPassword( plainPassword );
						else if ( newCryptPassword != null )
							m_NewCryptPassword = newCryptPassword;
						else
							SetPassword( "empty" );

						break;
					}
				default: // PasswordProtection.NewCrypt
					{
						if ( newCryptPassword != null )
							m_NewCryptPassword = newCryptPassword;
						else if ( plainPassword != null )
							SetPassword( plainPassword );
						else if ( cryptPassword != null )
							m_CryptPassword = cryptPassword;
						else
							SetPassword( "empty" );

						break;
					}
			}

			m_AccessLevel = (AccessLevel)Enum.Parse( typeof( AccessLevel ), Utility.GetText( node["accessLevel"], "Player" ), true );
			m_Flags = Utility.GetXMLInt32( Utility.GetText( node["flags"], "0" ), 0 );
			m_Gold = Utility.GetXMLInt32( Utility.GetText( node["accountGold"], "0"), 0);
			m_Created = Utility.GetXMLDateTime( Utility.GetText( node["created"], null ), DateTime.Now );
			m_LastLogin = Utility.GetXMLDateTime( Utility.GetText( node["lastLogin"], null ), DateTime.Now );

			m_Mobiles = LoadMobiles( node );
			m_Comments = LoadComments( node );
			m_Tags = LoadTags( node );
			m_LoginIPs = LoadAddressList( node );
			m_IPRestrictions = LoadAccessCheck( node );

			for ( int i = 0; i < m_Mobiles.Length; ++i )
			{
				if ( m_Mobiles[i] != null )
					m_Mobiles[i].Account = this;
			}

			TimeSpan totalGameTime = Utility.GetXMLTimeSpan( Utility.GetText( node["totalGameTime"], null ), TimeSpan.Zero );
			if ( totalGameTime == TimeSpan.Zero )
			{
				for ( int i = 0; i < m_Mobiles.Length; i++ )
				{
					PlayerMobile m = m_Mobiles[i] as PlayerMobile;

					if ( m != null )
						totalGameTime += m.GameTime;
				}
			}
			m_TotalGameTime = totalGameTime;

			if ( this.Young )
				CheckYoung();

			Accounts.Add( this );
		}

		/// <summary>
		/// Deserializes a list of string values from an xml element. Null values are not added to the list.
		/// </summary>
		/// <param name="node">The XmlElement from which to deserialize.</param>
		/// <returns>String list. Value will never be null.</returns>
		public static string[] LoadAccessCheck( XmlElement node )
		{
			string[] stringList;
			XmlElement accessCheck = node["accessCheck"];

			if ( accessCheck != null )
			{
				List<string> list = new List<string>();

				foreach ( XmlElement ip in accessCheck.GetElementsByTagName( "ip" ) )
				{
					string text = Utility.GetText( ip, null );

					if ( text != null )
						list.Add( text );
				}

				stringList = list.ToArray();
			}
			else
			{
				stringList = new string[0];
			}

			return stringList;
		}

		/// <summary>
		/// Deserializes a list of IPAddress values from an xml element.
		/// </summary>
		/// <param name="node">The XmlElement from which to deserialize.</param>
		/// <returns>Address list. Value will never be null.</returns>
		public static IPAddress[] LoadAddressList( XmlElement node )
		{
			IPAddress[] list;
			XmlElement addressList = node["addressList"];

			if ( addressList != null )
			{
				int count = Utility.GetXMLInt32( Utility.GetAttribute( addressList, "count", "0" ), 0 );

				list = new IPAddress[count];

				count = 0;

				foreach ( XmlElement ip in addressList.GetElementsByTagName( "ip" ) )
				{
					if ( count < list.Length )
					{
						IPAddress address;

						if( IPAddress.TryParse( Utility.GetText( ip, null ), out address ) )
						{
							list[count] = Utility.Intern( address );
							count++;
						}
					}
				}

				if ( count != list.Length )
				{
					IPAddress[] old = list;
					list = new IPAddress[count];

					for ( int i = 0; i < count && i < old.Length; ++i )
						list[i] = old[i];
				}
			}
			else
			{
				list = new IPAddress[0];
			}

			return list;
		}

		/// <summary>
		/// Deserializes a list of Mobile instances from an xml element.
		/// </summary>
		/// <param name="node">The XmlElement instance from which to deserialize.</param>
		/// <returns>Mobile list. Value will never be null.</returns>
		public static Mobile[] LoadMobiles( XmlElement node )
		{
			Mobile[] list = new Mobile[7];
			XmlElement chars = node["chars"];

			//int length = Accounts.GetInt32( Accounts.GetAttribute( chars, "length", "6" ), 6 );
			//list = new Mobile[length];
			//Above is legacy, no longer used

			if ( chars != null )
			{
				foreach ( XmlElement ele in chars.GetElementsByTagName( "char" ) )
				{
					try
					{
						int index = Utility.GetXMLInt32( Utility.GetAttribute( ele, "index", "0" ), 0 );
						int serial = Utility.GetXMLInt32( Utility.GetText( ele, "0" ), 0 );

						if ( index >= 0 && index < list.Length )
							list[index] = World.FindMobile( serial );
					}
					catch
					{
					}
				}
			}

			return list;
		}

		/// <summary>
		/// Deserializes a list of AccountComment instances from an xml element.
		/// </summary>
		/// <param name="node">The XmlElement from which to deserialize.</param>
		/// <returns>Comment list. Value will never be null.</returns>
		public static List<AccountComment> LoadComments( XmlElement node )
		{
			List<AccountComment> list = null;
			XmlElement comments = node["comments"];

			if ( comments != null )
			{
				list = new List<AccountComment>();

				foreach ( XmlElement comment in comments.GetElementsByTagName( "comment" ) )
				{
					try { list.Add( new AccountComment( comment ) ); }
					catch { }
				}
			}

			return list;
		}

		/// <summary>
		/// Deserializes a list of AccountTag instances from an xml element.
		/// </summary>
		/// <param name="node">The XmlElement from which to deserialize.</param>
		/// <returns>Tag list. Value will never be null.</returns>
		public static List<AccountTag> LoadTags( XmlElement node )
		{
			List<AccountTag> list = null;
			XmlElement tags = node["tags"];

			if ( tags != null )
			{
				list = new List<AccountTag>();

				foreach ( XmlElement tag in tags.GetElementsByTagName( "tag" ) )
				{
					try { list.Add( new AccountTag( tag ) ); }
					catch { }
				}
			}

			return list;
		}

		/// <summary>
		/// Checks if a specific NetState is allowed access to this account.
		/// </summary>
		/// <param name="ns">NetState instance to check.</param>
		/// <returns>True if allowed, false if not.</returns>
		public bool HasAccess( NetState ns )
		{
			return ( ns != null && HasAccess( ns.Address ) );
		}

		public bool HasAccess( IPAddress ipAddress ) {
			AccessLevel level = Misc.AccountHandler.LockdownLevel;

			if ( level > AccessLevel.Player )
			{
				bool hasAccess = false;

				if ( m_AccessLevel >= level )
				{
					hasAccess = true;
				}
				else
				{
					for ( int i = 0; !hasAccess && i < this.Length; ++i )
					{
						Mobile m = this[i];

						if ( m != null && m.AccessLevel >= level )
							hasAccess = true;
					}
				}

				if ( !hasAccess )
					return false;
			}

			bool accessAllowed = ( m_IPRestrictions.Length == 0 || IPLimiter.IsExempt( ipAddress ) );

			for ( int i = 0; !accessAllowed && i < m_IPRestrictions.Length; ++i )
				accessAllowed = Utility.IPMatch( m_IPRestrictions[i], ipAddress );

			return accessAllowed;
		}

		/// <summary>
		/// Records the IP address of 'ns' in its 'LoginIPs' list.
		/// </summary>
		/// <param name="ns">NetState instance to record.</param>
		public void LogAccess( NetState ns )
		{
			if ( ns != null ) {
				LogAccess( ns.Address );
			}
		}

		public void LogAccess( IPAddress ipAddress ) {
			if ( IPLimiter.IsExempt( ipAddress ) )
				return;

			if ( m_LoginIPs.Length == 0 ) {
				if ( AccountHandler.IPTable.ContainsKey( ipAddress ) )
					AccountHandler.IPTable[ipAddress]++;
				else
					AccountHandler.IPTable[ipAddress] = 1;
			}

			bool contains = false;

			for ( int i = 0; !contains && i < m_LoginIPs.Length; ++i )
				contains = m_LoginIPs[i].Equals( ipAddress );

			if ( contains )
				return;

			IPAddress[] old = m_LoginIPs;
			m_LoginIPs = new IPAddress[old.Length + 1];

			for ( int i = 0; i < old.Length; ++i )
				m_LoginIPs[i] = old[i];

			m_LoginIPs[old.Length] = ipAddress;
		}

		/// <summary>
		/// Checks if a specific NetState is allowed access to this account. If true, the NetState IPAddress is added to the address list.
		/// </summary>
		/// <param name="ns">NetState instance to check.</param>
		/// <returns>True if allowed, false if not.</returns>
		public bool CheckAccess( NetState ns )
		{
			return ( ns != null && CheckAccess( ns.Address ) );
		}

		public bool CheckAccess( IPAddress ipAddress ) {
			bool hasAccess = this.HasAccess( ipAddress );

			if ( hasAccess ) {
				LogAccess( ipAddress );
			}

			return hasAccess;
		}

		/// <summary>
		/// Serializes this Account instance to an XmlTextWriter.
		/// </summary>
		/// <param name="xml">The XmlTextWriter instance from which to serialize.</param>
		public void Save( XmlTextWriter xml )
		{
			xml.WriteStartElement( "account" );

			xml.WriteStartElement( "username" );
			xml.WriteString( m_Username );
			xml.WriteEndElement();

			if ( m_PlainPassword != null )
			{
				xml.WriteStartElement( "password" );
				xml.WriteString( m_PlainPassword );
				xml.WriteEndElement();
			}

			if ( m_CryptPassword != null )
			{
				xml.WriteStartElement( "cryptPassword" );
				xml.WriteString( m_CryptPassword );
				xml.WriteEndElement();
			}

			if ( m_NewCryptPassword != null )
			{
				xml.WriteStartElement( "newCryptPassword" );
				xml.WriteString( m_NewCryptPassword );
				xml.WriteEndElement();
			}

			if ( m_AccessLevel != AccessLevel.Player )
			{
				xml.WriteStartElement( "accessLevel" );
				xml.WriteString( m_AccessLevel.ToString() );
				xml.WriteEndElement();
			}

			if ( m_Flags != 0 )
			{
				xml.WriteStartElement( "flags" );
				xml.WriteString( XmlConvert.ToString( m_Flags ) );
				xml.WriteEndElement();
			}

			if ( m_Gold !=0 )
			{
				xml.WriteStartElement ("accountGold" );
				xml.WriteString( XmlConvert.ToString( m_Gold ) );
				xml.WriteEndElement();
			}

			xml.WriteStartElement( "created" );
			xml.WriteString( XmlConvert.ToString( m_Created, XmlDateTimeSerializationMode.Local ) );
			xml.WriteEndElement();

			xml.WriteStartElement( "lastLogin" );
			xml.WriteString( XmlConvert.ToString( m_LastLogin, XmlDateTimeSerializationMode.Local ) );
			xml.WriteEndElement();

			xml.WriteStartElement( "totalGameTime" );
			xml.WriteString( XmlConvert.ToString( TotalGameTime ) );
			xml.WriteEndElement();

			xml.WriteStartElement( "chars" );

			//xml.WriteAttributeString( "length", m_Mobiles.Length.ToString() );	//Legacy, Not used anymore

			for ( int i = 0; i < m_Mobiles.Length; ++i )
			{
				Mobile m = m_Mobiles[i];

				if ( m != null && !m.Deleted )
				{
					xml.WriteStartElement( "char" );
					xml.WriteAttributeString( "index", i.ToString() );
					xml.WriteString( m.Serial.Value.ToString() );
					xml.WriteEndElement();
				}
			}

			xml.WriteEndElement();

			if ( m_Comments != null && m_Comments.Count > 0 )
			{
				xml.WriteStartElement( "comments" );

				for ( int i = 0; i < m_Comments.Count; ++i )
					m_Comments[i].Save( xml );

				xml.WriteEndElement();
			}

			if ( m_Tags != null && m_Tags.Count > 0 )
			{
				xml.WriteStartElement( "tags" );

				for ( int i = 0; i < m_Tags.Count; ++i )
					m_Tags[i].Save( xml );

				xml.WriteEndElement();
			}

			if ( m_LoginIPs.Length > 0 )
			{
				xml.WriteStartElement( "addressList" );

				if ( TrackIPAddresses == false )
				{
					xml.WriteAttributeString( "count", "1" );

					for ( int i = 0; i < 1; ++i )
					{
						xml.WriteStartElement( "ip" );
						xml.WriteString( m_LoginIPs[i].ToString() );
						xml.WriteEndElement();
					}
				}
				else
				{
					xml.WriteAttributeString( "count", m_LoginIPs.Length.ToString() );

					for ( int i = 0; i < m_LoginIPs.Length; ++i )
					{
						xml.WriteStartElement( "ip" );
						xml.WriteString( m_LoginIPs[i].ToString() );
						xml.WriteEndElement();
					}
				}

				xml.WriteEndElement();
			}

			if ( m_IPRestrictions.Length > 0 )
			{
				xml.WriteStartElement( "accessCheck" );

				for ( int i = 0; i < m_IPRestrictions.Length; ++i )
				{
					xml.WriteStartElement( "ip" );
					xml.WriteString( m_IPRestrictions[i] );
					xml.WriteEndElement();
				}

				xml.WriteEndElement();
			}

			xml.WriteEndElement();
		}

		/// <summary>
		/// Gets the current number of characters on this account.
		/// </summary>
		public int Count
		{
			get
			{
				int count = 0;

				for ( int i = 0; i < this.Length; ++i )
				{
					if ( this[i] != null )
						++count;
				}

				return count;
			}
		}

		/// <summary>
		/// Gets the maximum amount of characters allowed to be created on this account. Values other than 1, 5, 6, or 7 are not supported by the client.
		/// </summary>
		public int Limit
		{
			get { return ( Core.SA ? 7 : Core.AOS ? 6 : 5 ); }
		}

		/// <summary>
		/// Gets the maxmimum amount of characters that this account can hold.
		/// </summary>
		public int Length
		{
			get { return m_Mobiles.Length; }
		}

		/// <summary>
		/// Gets or sets the character at a specified index for this account. Out of bound index values are handled; null returned for get, ignored for set.
		/// </summary>
		public Mobile this[int index]
		{
			get
			{
				if ( index >= 0 && index < m_Mobiles.Length )
				{
					Mobile m = m_Mobiles[index];

					if ( m != null && m.Deleted )
					{
						m.Account = null;
						m_Mobiles[index] = m = null;
					}

					return m;
				}

				return null;
			}
			set
			{
				if ( index >= 0 && index < m_Mobiles.Length )
				{
					if ( m_Mobiles[index] != null )
						m_Mobiles[index].Account = null;

					m_Mobiles[index] = value;

					if ( m_Mobiles[index] != null )
						m_Mobiles[index].Account = this;
				}
			}
		}

		public override string ToString()
		{
			return m_Username;
		}

		public int CompareTo( Account other )
		{
			if ( other == null )
				return -1;

			return m_Username.CompareTo( other.m_Username );
		}

		public int CompareTo( object obj )
		{
			if ( obj is Account )
				return this.CompareTo( (Account) obj );

			throw new ArgumentException();
		}
	}
}

namespace Server
{
	public class Firewall
	{
		#region Firewall Entries
		public interface IFirewallEntry
		{
			bool IsBlocked( IPAddress address );
		}

		public class IPFirewallEntry : IFirewallEntry
		{
			IPAddress m_Address;
			public IPFirewallEntry( IPAddress address )
			{
				m_Address = address;
			}

			public bool IsBlocked( IPAddress address )
			{
				return m_Address.Equals( address );
			}

			public override string ToString()
			{
				return m_Address.ToString();
			}

			public override bool Equals( object obj )
			{
				if( obj is IPAddress )
				{
					return obj.Equals( m_Address );
				}
				else if( obj is string )
				{
					IPAddress otherAddress;

					if( IPAddress.TryParse( (string)obj, out otherAddress ) )
						return otherAddress.Equals( m_Address );
				}
				else if( obj is IPFirewallEntry )
				{
					return m_Address.Equals( ((IPFirewallEntry)obj).m_Address );
				}

				return false;
			}

			public override int GetHashCode()
			{
				return m_Address.GetHashCode();
			}
		}

		public class CIDRFirewallEntry : IFirewallEntry
		{
			IPAddress m_CIDRPrefix;
			int m_CIDRLength;

			public CIDRFirewallEntry( IPAddress cidrPrefix, int cidrLength )
			{
				m_CIDRPrefix = cidrPrefix;
				m_CIDRLength = cidrLength;
			}

			public bool IsBlocked( IPAddress address )
			{
				return Utility.IPMatchCIDR( m_CIDRPrefix, address, m_CIDRLength );
			}

			public override string ToString()
			{
				return String.Format( "{0}/{1}", m_CIDRPrefix, m_CIDRLength );
			}

			public override bool Equals( object obj )
			{

				if( obj is string )
				{
					string entry= (string)obj;

					string[] str = entry.Split( '/' );

					if( str.Length == 2 )
					{
						IPAddress cidrPrefix;

						if( IPAddress.TryParse( str[0], out cidrPrefix ) )
						{
							int cidrLength;

							if( int.TryParse( str[1], out cidrLength ) )
								return m_CIDRPrefix.Equals( cidrPrefix ) && m_CIDRLength.Equals( cidrLength );
						}
					}
				}
				else if( obj is CIDRFirewallEntry )
				{
					CIDRFirewallEntry entry = obj as CIDRFirewallEntry;

					return m_CIDRPrefix.Equals( entry.m_CIDRPrefix ) && m_CIDRLength.Equals( entry.m_CIDRLength );
				}

				return false;
			}

			public override int GetHashCode()
			{
				return m_CIDRPrefix.GetHashCode() ^ m_CIDRLength.GetHashCode();
			}
		}

		public class WildcardIPFirewallEntry : IFirewallEntry
		{
			string m_Entry;

			bool m_Valid = true;

			public WildcardIPFirewallEntry( string entry )
			{
				m_Entry = entry;
			}

			public bool IsBlocked( IPAddress address )
			{
				if( !m_Valid )
					return false;	//Why process if it's invalid?  it'll return false anyway after processing it.

				return Utility.IPMatch( m_Entry, address, ref m_Valid );
			}

			public override string ToString()
			{
				return m_Entry.ToString();
			}

			public override bool Equals( object obj )
			{
				if( obj is string )
					return obj.Equals( m_Entry );
				else if( obj is WildcardIPFirewallEntry )
					return m_Entry.Equals( ((WildcardIPFirewallEntry)obj).m_Entry );

				return false;
			}

			public override int GetHashCode()
			{
				return m_Entry.GetHashCode();
			}
		}
		#endregion

		private static List<IFirewallEntry> m_Blocked;

		static Firewall()
		{
			m_Blocked = new List<IFirewallEntry>();

			string path = "firewall.cfg";

			if ( File.Exists( path ) )
			{
				using ( StreamReader ip = new StreamReader( path ) )
				{
					string line;

					while ( (line = ip.ReadLine()) != null )
					{
						line = line.Trim();

						if ( line.Length == 0 )
							continue;

						m_Blocked.Add( ToFirewallEntry( line ) );

						/*
						object toAdd;

						IPAddress addr;
						if( IPAddress.TryParse( line, out addr ) )
							toAdd = addr;
						else
							toAdd = line;

						m_Blocked.Add( toAdd.ToString() );
						 * */
					}
				}
			}
		}

		public static List<IFirewallEntry> List
		{
			get
			{
				return m_Blocked;
			}
		}

		public static IFirewallEntry ToFirewallEntry( object entry )
		{
			if( entry is IFirewallEntry )
				return (IFirewallEntry)entry;
			else if( entry is IPAddress )
				return new IPFirewallEntry( (IPAddress)entry );
			else if( entry is string )
				return ToFirewallEntry( (string)entry );

			return null;
		}

		public static IFirewallEntry ToFirewallEntry( string entry )
		{
			IPAddress addr;

			if( IPAddress.TryParse( entry, out addr ) )
				return new IPFirewallEntry( addr );

			//Try CIDR parse
			string[] str = entry.Split( '/' );

			if( str.Length == 2 )
			{
				IPAddress cidrPrefix;

				if( IPAddress.TryParse( str[0], out cidrPrefix ) )
				{
					int cidrLength;

					if( int.TryParse( str[1], out cidrLength ) )
						return new CIDRFirewallEntry( cidrPrefix, cidrLength );
				}
			}

			return new WildcardIPFirewallEntry( entry );
		}

		public static void RemoveAt( int index )
		{
			m_Blocked.RemoveAt( index );
			Save();
		}

		public static void Remove( object obj )
		{
			IFirewallEntry entry = ToFirewallEntry( obj );

			if( entry != null )
			{
				m_Blocked.Remove( entry );
				Save();
			}
		}

		public static void Add( object obj )
		{
			if( obj is IPAddress )
				Add( (IPAddress)obj );
			else if( obj is string )
				Add( (string)obj );
			else if( obj is IFirewallEntry )
				Add( (IFirewallEntry)obj );
		}

		public static void Add( IFirewallEntry entry )
		{
			if( !m_Blocked.Contains( entry ) )
				m_Blocked.Add( entry );

			Save();
		}

		public static void Add( string pattern )
		{
			IFirewallEntry entry = ToFirewallEntry( pattern );

			if( !m_Blocked.Contains( entry ) )
				m_Blocked.Add( entry );

			Save();
		}

		public static void Add( IPAddress ip )
		{
			IFirewallEntry entry = new IPFirewallEntry( ip );

			if( !m_Blocked.Contains( entry ) )
				m_Blocked.Add( entry );

			Save();
		}

		public static void Save()
		{
			string path = "firewall.cfg";

			using ( StreamWriter op = new StreamWriter( path ) )
			{
				for ( int i = 0; i < m_Blocked.Count; ++i )
					op.WriteLine( m_Blocked[i] );
			}
		}

		public static bool IsBlocked( IPAddress ip )
		{
			for( int i = 0; i < m_Blocked.Count; i++ )
			{
				if( m_Blocked[i].IsBlocked( ip ) )
					return true;
			}

			return false;
			/*
			bool contains = false;

			for ( int i = 0; !contains && i < m_Blocked.Count; ++i )
			{
				if ( m_Blocked[i] is IPAddress )
					contains = ip.Equals( m_Blocked[i] );
                else if ( m_Blocked[i] is String )
                {
                    string s = (string)m_Blocked[i];

                    contains = Utility.IPMatchCIDR( s, ip );

                    if( !contains )
                        contains = Utility.IPMatch( s, ip );
                }
			}

			return contains;
			 * */
		}
	}
}

namespace Server.Accounting
{
	public class AccountTag
	{
		private string m_Name, m_Value;

		/// <summary>
		/// Gets or sets the name of this tag.
		/// </summary>
		public string Name
		{
			get{ return m_Name; }
			set{ m_Name = value; }
		}

		/// <summary>
		/// Gets or sets the value of this tag.
		/// </summary>
		public string Value
		{
			get{ return m_Value; }
			set{ m_Value = value; }
		}

		/// <summary>
		/// Constructs a new AccountTag instance with a specific name and value.
		/// </summary>
		/// <param name="name">Initial name.</param>
		/// <param name="value">Initial value.</param>
		public AccountTag( string name, string value )
		{
			m_Name = name;
			m_Value = value;
		}

		/// <summary>
		/// Deserializes an AccountTag instance from an xml element.
		/// </summary>
		/// <param name="node">The XmlElement instance from which to deserialize.</param>
		public AccountTag( XmlElement node )
		{
			m_Name = Utility.GetAttribute( node, "name", "empty" );
			m_Value = Utility.GetText( node, "" );
		}

		/// <summary>
		/// Serializes this AccountTag instance to an XmlTextWriter.
		/// </summary>
		/// <param name="xml">The XmlTextWriter instance from which to serialize.</param>
		public void Save( XmlTextWriter xml )
		{
			xml.WriteStartElement( "tag" );
			xml.WriteAttributeString( "name", m_Name );
			xml.WriteString( m_Value );
			xml.WriteEndElement();
		}
	}
}

namespace Server.Misc
{
	public class IPLimiter
	{
		public static bool Enabled = true;
		public static bool SocketBlock = true; // true to block at connection, false to block at login request

		public static int MaxAddresses = 10;
		
		public static IPAddress[] Exemptions = new IPAddress[]	//For hosting services where there are cases where IPs can be proxied
		{
			//IPAddress.Parse( "127.0.0.1" ),
		};

		public static bool IsExempt( IPAddress ip )
		{
			for ( int i = 0; i < Exemptions.Length; i++ )
			{
				if ( ip.Equals( Exemptions[i] ) )
					return true;
			}

			return false;
		}

		public static bool Verify( IPAddress ourAddress )
		{
			if ( !Enabled || IsExempt( ourAddress ) )
				return true;

			List<NetState> netStates = NetState.Instances;

			int count = 0;

			for ( int i = 0; i < netStates.Count; ++i )
			{
				NetState compState = netStates[i];

				if ( ourAddress.Equals( compState.Address ) )
				{
					++count;

					if ( count >= MaxAddresses )
						return false;
				}
			}

			return true;
		}
	}
}
