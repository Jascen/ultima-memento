using System;
using System.Collections.Generic;
using Server.Network;

namespace Server.Spells.Dynamic
{
	// 0xBF/0x39 - Cache Valid (22 bytes)
	public sealed class SpellbookCacheValid : Packet
	{
		public SpellbookCacheValid( Serial spellbookSerial, byte spellbookType, uint version, ulong spellBitmask )
			: base( 0xBF )
		{
			EnsureCapacity( 22 );

			m_Stream.Write( (short) 0x39 );              // subcommand
			m_Stream.Write( (int) spellbookSerial );      // spellbook serial
			m_Stream.Write( (byte) spellbookType );       // spellbook type
			m_Stream.Write( (int) version );              // version
			WriteUInt64( spellBitmask );                  // spell bitmask (8 bytes)
		}

		private void WriteUInt64( ulong value )
		{
			for ( int i = 7; i >= 0; --i )
				m_Stream.Write( (byte)( value >> ( i * 8 ) ) );
		}
	}

	// 0xBF/0x3A - Full Spellbook Data (variable size)
	public sealed class SpellbookFullData : Packet
	{
		public SpellbookFullData(
			Serial spellbookSerial, byte spellbookType, uint version, ulong spellBitmask,
			ushort bookGraphic, ushort minimizedGraphic, List<DynamicSpellDefinition> spells,
			byte spellsPerPageSide, byte maxDictionaryPages, string[] pageNames,
			bool displayManaCost, bool displayMinSkill, bool displayPowerWords,
			string manaCostLabel, string minSkillLabel,
			string customPropertyTitle, string customPropertyLabel, string customPropertyName,
			ushort bookHue, ushort textColor, ushort spellNameColor,
			short contentOffsetX, short contentOffsetY,
			ushort pageTurnLeftGraphic, ushort pageTurnRightGraphic,
			short pageTurnLeftX, short pageTurnLeftY,
			short pageTurnRightX, short pageTurnRightY,
			ushort[] overlayGraphics,
			ushort titleColor = 0,
			List<DynamicInfoPage> infoPages = null,
			IDynamicSpellbookBookmark bookmark = null )
			: base( 0xBF )
		{
			if ( spells == null || spells.Count == 0 )
				return;

			if ( pageNames == null )
				pageNames = new string[0];

			if ( overlayGraphics == null )
				overlayGraphics = new ushort[0];

			if ( infoPages == null )
				infoPages = new List<DynamicInfoPage>();

			bool hasBookmark = bookmark != null && bookmark.HasBookmark;
			string bookmarkTooltip = hasBookmark ? ( bookmark.BookmarkTooltip ?? "" ) : "";
			byte bookmarkTooltipLen = (byte)Math.Min( bookmarkTooltip.Length, 255 );

			// Calculate total packet size
			int size = 35; // fixed header
			size += 23; // layout fields: hue(2) + textColor(2) + spellNameColor(2) + offsets(4) + pageTurnGraphics(4) + pageTurnPositions(8) + overlayCount(1)
			size += overlayGraphics.Length * 2; // overlay graphics (2 bytes each)

			// Custom property strings (3 length-prefixed ASCII strings)
			size += 1 + ( customPropertyTitle != null ? customPropertyTitle.Length : 0 );
			size += 1 + ( customPropertyLabel != null ? customPropertyLabel.Length : 0 );
			size += 1 + ( customPropertyName != null ? customPropertyName.Length : 0 );

			// Page names
			foreach ( string name in pageNames )
				size += 1 + ( name != null ? name.Length : 0 );

			// Spell definitions
			foreach ( DynamicSpellDefinition spell in spells )
			{
				size += 20; // fixed fields per spell
				size += spell.Name != null ? spell.Name.Length : 0;
				size += spell.PowerWords != null ? spell.PowerWords.Length : 0;
				size += spell.Description != null ? spell.Description.Length : 0;

				if ( spell.CustomReagents != null )
				{
					foreach ( string reagent in spell.CustomReagents )
					{
						string truncated = ( reagent != null && reagent.Length > 15 ) ? reagent.Substring( 0, 12 ) : reagent;
						size += 1 + ( truncated != null ? truncated.Length : 0 );
					}
				}
			}

			// Custom labels: manaCostLabel(1+len) + minSkillLabel(1+len) + titleColor(2)
			size += 1 + ( manaCostLabel != null ? manaCostLabel.Length : 0 );
			size += 1 + ( minSkillLabel != null ? minSkillLabel.Length : 0 );
			size += 2; // titleColor

			// Info pages: count(1) + per page: titleLen(2) + title + bodyLen(2) + body
			size += 1;
			foreach ( DynamicInfoPage ip in infoPages )
			{
				size += 2 + ( ip.Title != null ? ip.Title.Length : 0 );
				size += 2 + ( ip.Body != null ? ip.Body.Length : 0 );
			}

			// Bookmark trailer: hasBookmark(1) + (graphic(2)+pressed(2)+x(2)+y(2)+hue(2)+page(1)+actionType(1)+action(4)+tooltipLen(1)+tooltip)
			size += 1;
			if ( hasBookmark )
				size += 17 + bookmarkTooltipLen;

			EnsureCapacity( size );

			m_Stream.Write( (short) 0x3A );               // subcommand
			m_Stream.Write( (int) spellbookSerial );       // spellbook serial
			m_Stream.Write( (byte) spellbookType );        // spellbook type
			m_Stream.Write( (int) version );               // version
			WriteUInt64( spellBitmask );                   // spell bitmask (8 bytes)
			m_Stream.Write( (short) bookGraphic );         // book graphic
			m_Stream.Write( (short) minimizedGraphic );    // minimized graphic
			m_Stream.Write( (byte) spells.Count );         // spell count
			m_Stream.Write( (int) 14400 );                 // cache TTL (4 hours)
			m_Stream.Write( (byte) spellsPerPageSide );    // spells per page side
			m_Stream.Write( (byte) maxDictionaryPages );   // max dictionary pages
			m_Stream.Write( (byte) pageNames.Length );     // page name count

			// Display flags
			byte displayFlags = 0;
			if ( displayManaCost ) displayFlags |= 0x01;
			if ( displayMinSkill ) displayFlags |= 0x02;
			if ( displayPowerWords ) displayFlags |= 0x04;
			m_Stream.Write( (byte) displayFlags );

			// Layout customization
			m_Stream.Write( (short) bookHue );
			m_Stream.Write( (short) textColor );
			m_Stream.Write( (short) spellNameColor );
			m_Stream.Write( (short) contentOffsetX );
			m_Stream.Write( (short) contentOffsetY );
			m_Stream.Write( (short) pageTurnLeftGraphic );
			m_Stream.Write( (short) pageTurnRightGraphic );
			m_Stream.Write( (short) pageTurnLeftX );
			m_Stream.Write( (short) pageTurnLeftY );
			m_Stream.Write( (short) pageTurnRightX );
			m_Stream.Write( (short) pageTurnRightY );
			m_Stream.Write( (byte) overlayGraphics.Length );
			foreach ( ushort overlay in overlayGraphics )
				m_Stream.Write( (short) overlay );

			// Custom property strings
			WriteLengthPrefixedAscii( customPropertyTitle );
			WriteLengthPrefixedAscii( customPropertyLabel );
			WriteLengthPrefixedAscii( customPropertyName );

			// Page names
			foreach ( string pageName in pageNames )
				WriteLengthPrefixedAscii( pageName );

			// Spell definitions
			foreach ( DynamicSpellDefinition spell in spells )
			{
				m_Stream.Write( (short) spell.SpellID );
				m_Stream.Write( (short) spell.IconGraphic );
				m_Stream.Write( (int) spell.NameCliloc );
				WriteLengthPrefixedAscii( spell.Name );
				WriteLengthPrefixedAscii( spell.PowerWords );
				WriteLengthPrefixedAscii( spell.Description );
				m_Stream.Write( (byte) spell.ManaCost );
				m_Stream.Write( (byte) spell.MinSkill );
				m_Stream.Write( (byte) spell.TargetType );
				m_Stream.Write( (short) spell.Reagents );

				byte reagentCount = (byte)( spell.CustomReagents != null ? spell.CustomReagents.Length : 0 );
				m_Stream.Write( (byte) reagentCount );

				if ( reagentCount > 0 )
				{
					foreach ( string reagent in spell.CustomReagents )
					{
						string truncated = ( reagent != null && reagent.Length > 15 )
							? reagent.Substring( 0, 12 ) + "..."
							: reagent ?? "";
						WriteLengthPrefixedAscii( truncated );
					}
				}

				m_Stream.Write( (short) spell.Cooldown );
				m_Stream.Write( (byte) spell.Page );
			}

			// Custom labels
			WriteLengthPrefixedAscii( manaCostLabel );
			WriteLengthPrefixedAscii( minSkillLabel );

			// Title color (0 = use TextColor)
			m_Stream.Write( (short) titleColor );

			// Info pages
			m_Stream.Write( (byte) infoPages.Count );
			foreach ( DynamicInfoPage ip in infoPages )
			{
				WriteUInt16PrefixedAscii( ip.Title );
				WriteUInt16PrefixedAscii( ip.Body );
			}

			// Bookmark trailer
			m_Stream.Write( (byte)( hasBookmark ? 0x01 : 0x00 ) );
			if ( hasBookmark )
			{
				m_Stream.Write( (short) bookmark.BookmarkGraphic );
				m_Stream.Write( (short) bookmark.BookmarkPressedGraphic );
				m_Stream.Write( (short) bookmark.BookmarkX );
				m_Stream.Write( (short) bookmark.BookmarkY );
				m_Stream.Write( (short) bookmark.BookmarkHue );
				m_Stream.Write( (byte) bookmark.BookmarkPage );
				m_Stream.Write( (byte) bookmark.BookmarkActionType );
				m_Stream.Write( (int) bookmark.BookmarkAction );
				m_Stream.Write( (byte) bookmarkTooltipLen );
				for ( int i = 0; i < bookmarkTooltipLen; i++ )
					m_Stream.Write( (byte) bookmarkTooltip[i] );
			}
		}

		private void WriteUInt64( ulong value )
		{
			for ( int i = 7; i >= 0; --i )
				m_Stream.Write( (byte)( value >> ( i * 8 ) ) );
		}

		private void WriteLengthPrefixedAscii( string value )
		{
			if ( string.IsNullOrEmpty( value ) )
			{
				m_Stream.Write( (byte) 0 );
				return;
			}

			byte len = (byte)Math.Min( value.Length, 255 );
			m_Stream.Write( (byte) len );

			for ( int i = 0; i < len; i++ )
				m_Stream.Write( (byte) value[i] );
		}

		private void WriteUInt16PrefixedAscii( string value )
		{
			if ( string.IsNullOrEmpty( value ) )
			{
				m_Stream.Write( (short) 0 );
				return;
			}

			ushort len = (ushort)Math.Min( value.Length, 65535 );
			m_Stream.Write( (short) len );

			for ( int i = 0; i < len; i++ )
				m_Stream.Write( (byte) value[i] );
		}
	}

	// 0xBF/0x3B - Invalidate Cache (11 bytes)
	public sealed class InvalidateSpellbookCache : Packet
	{
		public InvalidateSpellbookCache( byte spellbookType, uint newVersion, bool forceReload )
			: base( 0xBF )
		{
			EnsureCapacity( 11 );

			m_Stream.Write( (short) 0x3B );
			m_Stream.Write( (byte) spellbookType );
			m_Stream.Write( (int) newVersion );
			m_Stream.Write( (byte)( forceReload ? 0x01 : 0x00 ) );
		}
	}

	// 0xBF/0x3C - Register Item Graphic (8 bytes)
	public sealed class RegisterSpellbookItemID : Packet
	{
		public RegisterSpellbookItemID( ushort itemGraphic, byte spellbookType )
			: base( 0xBF )
		{
			EnsureCapacity( 8 );

			m_Stream.Write( (short) 0x3C );
			m_Stream.Write( (short) itemGraphic );
			m_Stream.Write( (byte) spellbookType );
		}
	}

	// 0xBF/0x3D - Register Spellbook Serial (10 bytes)
	// Maps a specific item serial to a dynamic spellbook type.
	// Sent before DisplaySpellbook so the client knows the type for this specific item.
	public sealed class RegisterSpellbookSerial : Packet
	{
		public RegisterSpellbookSerial( Serial serial, byte spellbookType )
			: base( 0xBF )
		{
			EnsureCapacity( 10 );

			m_Stream.Write( (short) 0x3D );
			m_Stream.Write( (int) serial );
			m_Stream.Write( (byte) spellbookType );
		}
	}
}
