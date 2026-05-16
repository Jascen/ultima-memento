using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Server.Network;
using Server.Items;

namespace Server.Spells.Dynamic
{
	public struct DynamicSpellDefinition
	{
		public ushort SpellID;
		public ushort IconGraphic;
		public int NameCliloc;
		public string Name;
		public string PowerWords;
		public string Description;
		public byte ManaCost;
		public byte MinSkill;
		public byte TargetType; // 0=Neutral, 1=Harmful, 2=Beneficial
		public ushort Reagents;
		public string[] CustomReagents;
		public ushort Cooldown;
		public byte Page;
	}

	public interface IDynamicSpellbookProvider
	{
		SpellbookType SpellbookType { get; }
		ushort BookGraphic { get; }
		ushort MinimizedGraphic { get; }
		byte SpellsPerPageSide { get; }
		byte MaxDictionaryPages { get; }
		string[] GetPageNames();
		bool DisplayManaCost { get; }
		bool DisplayMinSkill { get; }
		bool DisplayPowerWords { get; }
		string ManaCostLabel { get; }
		string MinSkillLabel { get; }
		string CustomPropertyTitle { get; }
		string CustomPropertyLabel { get; }
		string CustomPropertyName { get; }
		List<DynamicSpellDefinition> GetSpellDefinitions();
		ulong GetSpellBitmask( Mobile m, Spellbook spellbook );

		// Layout customization (0 = use client defaults)
		ushort BookHue { get; }
		ushort TextColor { get; }       // Color for headers, labels, mana cost, etc.
		ushort SpellNameColor { get; }  // Color for clickable spell names in dictionary
		ushort TitleColor { get; }      // Color for titles (Index, sphere names). 0 = use TextColor
		short ContentOffsetX { get; }
		short ContentOffsetY { get; }
		ushort PageTurnLeftGraphic { get; }
		ushort PageTurnRightGraphic { get; }
		short PageTurnLeftX { get; }
		short PageTurnLeftY { get; }
		short PageTurnRightX { get; }
		short PageTurnRightY { get; }
		ushort[] OverlayGraphics { get; }
		List<DynamicInfoPage> GetInfoPages();
	}

	public class DynamicInfoPage
	{
		public string Title { get; set; }
		public string Body { get; set; }
	}

	// Action types for the dynamic spellbook bookmark/tab button
	public static class DynamicBookmarkAction
	{
		public const byte None           = 0; // no-op (used when HasBookmark is false but we still serialize)
		public const byte JumpToPage     = 1; // payload = 1-based logical page index inside the spellbook
		public const byte ServerCallback = 2; // payload = opaque token; client sends 0xBF/0x3E back to server
	}

	// Optional interface implemented by providers that want a server-driven bookmark/tab button
	// on their dynamic spellbook (e.g. the bottom-left "help" tab on the legacy elemental spellbook).
	public interface IDynamicSpellbookBookmark
	{
		bool HasBookmark { get; }
		ushort BookmarkGraphic { get; }          // gump graphic id (e.g. 2095)
		ushort BookmarkPressedGraphic { get; }   // 0 = reuse BookmarkGraphic
		short BookmarkX { get; }                 // position in the data box
		short BookmarkY { get; }
		ushort BookmarkHue { get; }              // 0 = no hue
		byte BookmarkPage { get; }               // 0 = show on every page, otherwise the 1-based page to show on
		byte BookmarkActionType { get; }         // see DynamicBookmarkAction
		uint BookmarkAction { get; }             // payload (page index or opaque token)
		string BookmarkTooltip { get; }          // optional tooltip; null/empty for none

		// Invoked when the client presses the bookmark and ActionType is ServerCallback.
		void OnBookmarkPressed( Mobile from, Spellbook spellbook, uint token );
	}

	public static class DynamicSpellbookManager
	{
		public static void Initialize()
		{
			Console.WriteLine( "[DynamicSpellbook] DynamicSpellbookManager.Initialize() - Registering 0x38 extended packet handler" );
			PacketHandlers.RegisterExtended( 0x38, true, new OnPacketReceive( DynamicSpellbookRequest ) );
			PacketHandlers.RegisterExtended( 0x3E, true, new OnPacketReceive( DynamicSpellbookBookmarkPress ) );
		}

		private static void DynamicSpellbookBookmarkPress( NetState state, PacketReader pvSrc )
		{
			Mobile from = state.Mobile;

			if ( from == null )
				return;

			Serial spellbookSerial = pvSrc.ReadInt32();
			byte spellbookType = pvSrc.ReadByte();
			uint token = pvSrc.ReadUInt32();

			SpellbookType type = (SpellbookType)spellbookType;

			IDynamicSpellbookProvider provider;
			if ( !m_Providers.TryGetValue( type, out provider ) )
				return;

			IDynamicSpellbookBookmark bookmarkProvider = provider as IDynamicSpellbookBookmark;
			if ( bookmarkProvider == null || !bookmarkProvider.HasBookmark )
				return;

			Spellbook spellbook = World.FindItem( spellbookSerial ) as Spellbook;
			if ( spellbook == null || spellbook.SpellbookType != type )
				return;

			// Owner check: only the player holding the book may trigger the bookmark callback.
			if ( spellbook.RootParent != from )
				return;

			bookmarkProvider.OnBookmarkPressed( from, spellbook, token );
		}

		private static void DynamicSpellbookRequest( NetState state, PacketReader pvSrc )
		{
			Mobile from = state.Mobile;

			if ( from == null )
				return;

			Serial spellbookSerial = pvSrc.ReadInt32();
			byte spellbookType = pvSrc.ReadByte();
			uint cachedVersion = pvSrc.ReadUInt32();
			byte flags = pvSrc.ReadByte();
			bool forceRefresh = ( flags & 0x01 ) != 0;

			Console.WriteLine( "[DynamicSpellbook] 0x38 Request from {0}: serial=0x{1:X}, type={2}, cachedVersion=0x{3:X8}, forceRefresh={4}",
				from.Name, (int)spellbookSerial, spellbookType, cachedVersion, forceRefresh );

			HandleSpellbookRequest( state, spellbookSerial, spellbookType, cachedVersion, forceRefresh );
		}

		private static readonly Dictionary<SpellbookType, IDynamicSpellbookProvider> m_Providers = new Dictionary<SpellbookType, IDynamicSpellbookProvider>();
		private static readonly Dictionary<SpellbookType, uint> m_Versions = new Dictionary<SpellbookType, uint>();
		private static readonly Dictionary<SpellbookType, List<DynamicSpellDefinition>> m_CachedDefinitions = new Dictionary<SpellbookType, List<DynamicSpellDefinition>>();
		private static readonly Dictionary<SpellbookType, ushort> m_ItemGraphics = new Dictionary<SpellbookType, ushort>();

		public static void RegisterProvider( IDynamicSpellbookProvider provider, ushort itemGraphic )
		{
			Console.WriteLine( "[DynamicSpellbook] RegisterProvider: type={0}, itemGraphic=0x{1:X4}, bookGraphic=0x{2:X4}, minimizedGraphic=0x{3:X4}",
				(int)provider.SpellbookType, itemGraphic, provider.BookGraphic, provider.MinimizedGraphic );

			m_Providers[provider.SpellbookType] = provider;
			m_CachedDefinitions.Remove( provider.SpellbookType );

			List<DynamicSpellDefinition> definitions = provider.GetSpellDefinitions();
			uint version = CalculateVersion( provider, definitions );
			m_Versions[provider.SpellbookType] = version;
			m_CachedDefinitions[provider.SpellbookType] = definitions;
			m_ItemGraphics[provider.SpellbookType] = itemGraphic;

			Console.WriteLine( "[DynamicSpellbook] RegisterProvider: {0} spell definitions, version=0x{1:X8}", definitions.Count, version );

			// Broadcast registration to all connected clients
			foreach ( NetState ns in NetState.Instances )
			{
				if ( ns != null && ns.Mobile != null )
				{
					Console.WriteLine( "[DynamicSpellbook] Broadcasting 0x3C registration to {0}", ns.Mobile.Name );
					ns.Send( new RegisterSpellbookItemID( itemGraphic, (byte)provider.SpellbookType ) );
				}
			}
		}

		public static void SendAllRegistrations( NetState ns )
		{
			if ( ns == null )
				return;

			Console.WriteLine( "[DynamicSpellbook] SendAllRegistrations: sending {0} registrations to {1}",
				m_ItemGraphics.Count, ns.Mobile != null ? ns.Mobile.Name : "unknown" );

			foreach ( KeyValuePair<SpellbookType, ushort> kvp in m_ItemGraphics )
			{
				Console.WriteLine( "[DynamicSpellbook] Sending 0x3C: itemGraphic=0x{0:X4}, type={1}", kvp.Value, (int)kvp.Key );
				ns.Send( new RegisterSpellbookItemID( kvp.Value, (byte)kvp.Key ) );
			}
		}

		public static IDynamicSpellbookProvider GetProvider( SpellbookType type )
		{
			IDynamicSpellbookProvider provider;
			m_Providers.TryGetValue( type, out provider );
			return provider;
		}

		public static bool IsDynamicSpellbook( SpellbookType type )
		{
			return m_Providers.ContainsKey( type );
		}

		public static uint GetVersion( SpellbookType type )
		{
			uint version;
			return m_Versions.TryGetValue( type, out version ) ? version : 0u;
		}

		public static List<DynamicSpellDefinition> GetSpellDefinitions( SpellbookType type )
		{
			List<DynamicSpellDefinition> definitions;
			m_CachedDefinitions.TryGetValue( type, out definitions );
			return definitions;
		}

		public static void InvalidateCache( SpellbookType type, bool forceReload )
		{
			IDynamicSpellbookProvider provider;
			if ( !m_Providers.TryGetValue( type, out provider ) )
				return;

			List<DynamicSpellDefinition> definitions = provider.GetSpellDefinitions();
			uint newVersion = CalculateVersion( provider, definitions );
			m_Versions[type] = newVersion;
			m_CachedDefinitions[type] = definitions;

			foreach ( NetState ns in NetState.Instances )
			{
				if ( ns != null && ns.Mobile != null )
					ns.Send( new InvalidateSpellbookCache( (byte)type, newVersion, forceReload ) );
			}
		}

		public static void HandleSpellbookRequest( NetState state, Serial spellbookSerial, byte spellbookType, uint cachedVersion, bool forceRefresh )
		{
			SpellbookType type = (SpellbookType)spellbookType;

			IDynamicSpellbookProvider provider;
			if ( !m_Providers.TryGetValue( type, out provider ) )
			{
				Console.WriteLine( "[DynamicSpellbook] HandleSpellbookRequest: No provider found for type {0}", spellbookType );
				return;
			}

			uint currentVersion = GetVersion( type );

			Spellbook spellbook = World.FindItem( spellbookSerial ) as Spellbook;
			if ( spellbook == null || spellbook.SpellbookType != type )
			{
				Console.WriteLine( "[DynamicSpellbook] HandleSpellbookRequest: Spellbook not found or type mismatch (serial=0x{0:X}, expected type={1})", (int)spellbookSerial, spellbookType );
				return;
			}

			ulong spellBitmask = provider.GetSpellBitmask( state.Mobile, spellbook );

			Console.WriteLine( "[DynamicSpellbook] HandleSpellbookRequest: currentVersion=0x{0:X8}, bitmask=0x{1:X16}", currentVersion, spellBitmask );

			if ( !forceRefresh && cachedVersion == currentVersion )
			{
				Console.WriteLine( "[DynamicSpellbook] Sending 0x39 CacheValid" );
				state.Send( new SpellbookCacheValid( spellbookSerial, spellbookType, currentVersion, spellBitmask ) );
			}
			else
			{
				List<DynamicSpellDefinition> definitions = GetSpellDefinitions( type );
				if ( definitions != null && definitions.Count > 0 )
				{
					Console.WriteLine( "[DynamicSpellbook] Sending 0x3A FullData with {0} spell definitions", definitions.Count );

					IDynamicSpellbookBookmark bookmark = provider as IDynamicSpellbookBookmark;

					state.Send( new SpellbookFullData(
						spellbookSerial, spellbookType, currentVersion, spellBitmask,
						provider.BookGraphic, provider.MinimizedGraphic, definitions,
						provider.SpellsPerPageSide, provider.MaxDictionaryPages,
						provider.GetPageNames(), provider.DisplayManaCost, provider.DisplayMinSkill, provider.DisplayPowerWords, provider.ManaCostLabel, provider.MinSkillLabel,
						provider.CustomPropertyTitle, provider.CustomPropertyLabel, provider.CustomPropertyName,
						provider.BookHue, provider.TextColor, provider.SpellNameColor,
						provider.ContentOffsetX, provider.ContentOffsetY,
						provider.PageTurnLeftGraphic, provider.PageTurnRightGraphic,
						provider.PageTurnLeftX, provider.PageTurnLeftY,
						provider.PageTurnRightX, provider.PageTurnRightY,
						provider.OverlayGraphics,
						provider.TitleColor,
						provider.GetInfoPages(),
						bookmark
					) );
				}
				else
				{
					Console.WriteLine( "[DynamicSpellbook] HandleSpellbookRequest: No definitions found for type {0}", spellbookType );
				}
			}
		}

		private static uint CalculateVersion( IDynamicSpellbookProvider provider, List<DynamicSpellDefinition> definitions )
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( provider.BookGraphic );
			sb.Append( provider.MinimizedGraphic );
			sb.Append( provider.SpellsPerPageSide );
			sb.Append( provider.MaxDictionaryPages );
			sb.Append( provider.DisplayManaCost );
			sb.Append( provider.DisplayMinSkill );
			sb.Append( provider.ManaCostLabel ?? "" );
			sb.Append( provider.MinSkillLabel ?? "" );
			sb.Append( provider.CustomPropertyTitle ?? "" );
			sb.Append( provider.CustomPropertyLabel ?? "" );
			sb.Append( provider.CustomPropertyName ?? "" );
			sb.Append( provider.BookHue );
			sb.Append( provider.TextColor );
			sb.Append( provider.SpellNameColor );
			sb.Append( provider.TitleColor );
			sb.Append( provider.ContentOffsetX );
			sb.Append( provider.ContentOffsetY );
			sb.Append( provider.PageTurnLeftGraphic );
			sb.Append( provider.PageTurnRightGraphic );
			sb.Append( provider.PageTurnLeftX );
			sb.Append( provider.PageTurnLeftY );
			sb.Append( provider.PageTurnRightX );
			sb.Append( provider.PageTurnRightY );

			ushort[] overlays = provider.OverlayGraphics;
			if ( overlays != null )
			{
				sb.Append( overlays.Length );
				foreach ( ushort overlay in overlays )
					sb.Append( overlay );
			}

			string[] pageNames = provider.GetPageNames();
			if ( pageNames != null )
			{
				sb.Append( pageNames.Length );
				foreach ( string pageName in pageNames )
					sb.Append( pageName ?? "" );
			}

			sb.Append( definitions.Count );
			foreach ( DynamicSpellDefinition spell in definitions )
			{
				sb.Append( spell.SpellID );
				sb.Append( spell.IconGraphic );
				sb.Append( spell.NameCliloc );
				sb.Append( spell.Name ?? "" );
				sb.Append( spell.PowerWords ?? "" );
				sb.Append( spell.Description ?? "" );
				sb.Append( spell.ManaCost );
				sb.Append( spell.MinSkill );
				sb.Append( spell.TargetType );
				sb.Append( spell.Reagents );

				if ( spell.CustomReagents != null )
				{
					sb.Append( spell.CustomReagents.Length );
					foreach ( string reagent in spell.CustomReagents )
						sb.Append( reagent ?? "" );
				}

				sb.Append( spell.Cooldown );
				sb.Append( spell.Page );
			}

			List<DynamicInfoPage> infoPages = provider.GetInfoPages();
			if ( infoPages != null )
			{
				sb.Append( infoPages.Count );
				foreach ( DynamicInfoPage ip in infoPages )
				{
					sb.Append( ip.Title ?? "" );
					sb.Append( ip.Body ?? "" );
				}
			}

			IDynamicSpellbookBookmark bookmark = provider as IDynamicSpellbookBookmark;
			if ( bookmark != null && bookmark.HasBookmark )
			{
				sb.Append( "BM" );
				sb.Append( bookmark.BookmarkGraphic );
				sb.Append( bookmark.BookmarkPressedGraphic );
				sb.Append( bookmark.BookmarkX );
				sb.Append( bookmark.BookmarkY );
				sb.Append( bookmark.BookmarkHue );
				sb.Append( bookmark.BookmarkPage );
				sb.Append( bookmark.BookmarkActionType );
				sb.Append( bookmark.BookmarkAction );
				sb.Append( bookmark.BookmarkTooltip ?? "" );
			}

			byte[] hash = MD5.Create().ComputeHash( Encoding.UTF8.GetBytes( sb.ToString() ) );
			return BitConverter.ToUInt32( hash, 0 );
		}
	}
}
