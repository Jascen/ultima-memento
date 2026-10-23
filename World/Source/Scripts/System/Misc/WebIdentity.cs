using System;
using System.IO;
using System.Net;
using Server.Network;

namespace Server.Misc
{
	/// <summary>
	/// Handles the WebIdentity packet used by ClassicUO Web (CUOWEB) and compatible
	/// web clients (LUOWEB).
	///
	/// The identity is sent as a 149 byte 0xA4 SystemInfo packet, before the client
	/// logs in. It contains the real address of the player as seen by the web proxy,
	/// which lets us use proper IP limits even though every web client connects from
	/// the same proxy address.
	/// </summary>
	public static class WebIdentity
	{
		public const int PacketID = 0xA4;
		public const int PacketLength = 149;

		private const string ClassicUOWebClientType = "CUOWEB";
		private const string LuOWebClientType = "LUOWEB";

		private static readonly TimeSpan MaxIdentityAge = TimeSpan.FromSeconds( 30 );

		/// <summary>
		/// Occurs after a web identity packet has been validated.
		/// </summary>
		public static event Action<WebIdentityEventArgs> OnWebIdentityReceived;

		public static void Initialize()
		{
			PacketHandlers.Register( PacketID, PacketLength, false, new OnPacketReceive( OnPacketReceive ) );

			// Web clients all connect through the game proxy, so their TCP address is
			// shared. Blocking at accept time would kick players before the identity
			// packet can tell us their real address. Defer the IP limit check to login,
			// where NetState.Address has been updated from the identity packet.
			if ( MySettings.S_WebIdentityIpLimitWorkaround )
				IPLimiter.SocketBlock = false;

			if ( string.IsNullOrEmpty( ( MySettings.S_WebIdentitySecret ?? string.Empty ).Trim() ) )
				Console.WriteLine( "WebIdentity: no secret configured; identity packets will not be authenticated." );
		}

		private static bool IsSupportedClient( string clientType )
		{
			return string.Equals( clientType, ClassicUOWebClientType, StringComparison.OrdinalIgnoreCase )
				|| string.Equals( clientType, LuOWebClientType, StringComparison.OrdinalIgnoreCase );
		}

		private static bool IsValidSecret( string configuredSecrets, string secret )
		{
			if ( configuredSecrets.Length == 0 )
				return true;

			string[] secrets = configuredSecrets.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );

			for ( int i = 0; i < secrets.Length; ++i )
			{
				if ( string.Equals( secrets[i].Trim(), secret, StringComparison.Ordinal ) )
					return true;
			}

			return false;
		}

		private static void OnPacketReceive( NetState state, PacketReader reader )
		{
			string clientType = reader.ReadString( 6 );

			if ( !IsSupportedClient( clientType ) )
			{
				// Not a web identity, let the normal SystemInfo handler consume it.
				reader.Seek( 1, SeekOrigin.Begin );
				PacketHandlers.SystemInfo( state, reader );
				return;
			}

			int version = reader.ReadByte();

			if ( version > 1 )
				Console.WriteLine( "WebIdentity: {0} sent unsupported version {1}; an update may be available.", state, version );

			DateTimeOffset timestamp = DateTimeOffset.FromUnixTimeSeconds( reader.ReadUInt32() );
			string secret = reader.ReadUTF8String();
			string userId = reader.ReadUTF8String();
			string connectingIp = reader.ReadUTF8String().Trim();
			string externalAuthProvider = reader.ReadUTF8String();
			string externalAuthUsername = reader.ReadUTF8String();
			string externalAuthId = reader.ReadUTF8String();
			string role = reader.ReadUTF8String();

			string configuredSecret = ( MySettings.S_WebIdentitySecret ?? string.Empty ).Trim();

			// An empty configured secret means validation is disabled. This is useful
			// for testing, but leaves the identity forgeable, so configure a secret in
			// production. Multiple comma separated secrets are supported in case the
			// CUOWEB and LUOWEB proxies use different secrets.
			if ( !IsValidSecret( configuredSecret, secret ) )
			{
				if ( MySettings.S_WebIdentityKickOnBadSecret )
				{
					Console.WriteLine( "WebIdentity: {0} sent an invalid secret, disconnecting.", state );
					state.Dispose();
					return;
				}

				Console.WriteLine( "WebIdentity: {0} sent an invalid secret, ignoring identity.", state );
				return;
			}

			TimeSpan age = DateTimeOffset.UtcNow - timestamp;

			if ( age > MaxIdentityAge )
			{
				Console.WriteLine( "WebIdentity: {0} sent a stale identity ({1} seconds old), disconnecting.", state, Math.Round( age.TotalSeconds ) );
				state.Dispose();
				return;
			}

			if ( MySettings.S_WebIdentityIpLimitWorkaround )
			{
				IPAddress realAddress;

				if ( !IPAddress.TryParse( connectingIp, out realAddress ) )
				{
					Console.WriteLine( "WebIdentity: {0} sent an invalid connecting IP '{1}', disconnecting.", state, connectingIp );
					state.Dispose();
					return;
				}

				state.OverrideAddress( realAddress );
			}

			Console.WriteLine( "WebIdentity: {0} ({1}) identified user '{2}' as {3} from {4}.", state, clientType, userId, role, connectingIp );

			Action<WebIdentityEventArgs> handler = OnWebIdentityReceived;

			if ( handler != null )
			{
				WebIdentityEventArgs args = new WebIdentityEventArgs( state, clientType, version, timestamp, userId, connectingIp, externalAuthProvider, externalAuthUsername, externalAuthId, role );

				try
				{
					handler( args );
				}
				catch ( Exception ex )
				{
					Console.WriteLine( "WebIdentity: event handler threw an exception: {0}", ex );
				}
			}
		}
	}

	public sealed class WebIdentityEventArgs : EventArgs
	{
		public NetState State { get; private set; }
		public string ClientType { get; private set; }
		public int Version { get; private set; }
		public DateTimeOffset Timestamp { get; private set; }
		public string UserId { get; private set; }
		public string ConnectingIp { get; private set; }
		public string ExternalAuthProvider { get; private set; }
		public string ExternalAuthUsername { get; private set; }
		public string ExternalAuthId { get; private set; }
		public string Role { get; private set; }

		public WebIdentityEventArgs( NetState state, string clientType, int version, DateTimeOffset timestamp, string userId, string connectingIp, string externalAuthProvider, string externalAuthUsername, string externalAuthId, string role )
		{
			State = state;
			ClientType = clientType;
			Version = version;
			Timestamp = timestamp;
			UserId = userId;
			ConnectingIp = connectingIp;
			ExternalAuthProvider = externalAuthProvider;
			ExternalAuthUsername = externalAuthUsername;
			ExternalAuthId = externalAuthId;
			Role = role;
		}
	}
}
