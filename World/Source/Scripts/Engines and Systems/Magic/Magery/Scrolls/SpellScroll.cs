using System.Collections.Generic;
using Server.Spells;
using Server.Spells.Elementalism;
using Server.ContextMenus;
using Server.Engines.Craft;
using System;

namespace Server.Items
{
	public class SpellScroll : Item, ICraftable
	{
		public override Catalogs DefaultCatalog{ get{ return Catalogs.Scroll; } }

		private bool m_IsEphemeralSpellScroll;
		public bool IsEphemeralSpellScroll
		{
			get { return m_IsEphemeralSpellScroll && !MySettings.S_UseLegacyInscription; }
			set { m_IsEphemeralSpellScroll = value; InvalidateProperties(); }
		}

		private int m_SpellID;

		public int SpellID
		{
			get
			{
				return m_SpellID;
			}
		}

		public SpellScroll( Serial serial ) : base( serial )
		{
		}

		[Constructable]
		public SpellScroll( int spellID, int itemID ) : this( spellID, itemID, 1 )
		{
		}

		[Constructable]
		public SpellScroll( int spellID, int itemID, int amount ) : base( itemID )
		{
			Stackable = true;
			Amount = amount;
			m_SpellID = spellID;
			InfoFill();
		}

		public void InfoFill()
		{
			MagicSpell magicspell = SpellItems.GetID( this.GetType() );
			ClassName( m_SpellID, this );
			
			Built = true;
			Weight = 0.1;

			if ( SpellItems.GetCircle( magicspell ) != null )
			{
				Name = SpellItems.GetName( magicspell ) + " scroll";
				InfoData = "This scroll contains a magery spell, " + SpellItems.GetName( magicspell ) + ". " + SpellItems.GetData( magicspell );
				InfoText2 = SpellItems.GetCircle( magicspell );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
			writer.Write( (int) m_SpellID );
			writer.Write( m_IsEphemeralSpellScroll );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_SpellID = reader.ReadInt();
			m_IsEphemeralSpellScroll = 0 < version ? reader.ReadBool() : false;

			InfoFill();
		}

		public static void ClassName( int spellID, Item scroll )
		{
			if ( spellID < 64 )
				scroll.InfoText1 = "Magery Spell";
			else if ( spellID >= 100 && spellID <= 116 )
				scroll.InfoText1 = "Necromancer Spell";
			else if ( spellID >= 131 && spellID <= 146 )
				scroll.InfoText1 = "Witches Brew";
			else if ( spellID >= 147 && spellID <= 162 )
				scroll.InfoText1 = "Druidic Herbs";
			else if ( spellID >= 300 && spellID <= 331 )
			{
				scroll.InfoText1 = "Elementalist Spell";
				scroll.InfoText2 = ElementalSpell.CommonInfo( spellID, 6 ) + " Sphere";
			}
			else if ( spellID >= 750 && spellID <= 763 )
				scroll.InfoText1 = "Death Knight Magic";
			else if ( spellID >= 770 && spellID <= 783 )
				scroll.InfoText1 = "Holy Magic";
			else if ( spellID >= 351 && spellID <= 366 )
				scroll.InfoText1 = "Bard Song";
			else if ( spellID >= 280 && spellID <= 289 )
				scroll.InfoText1 = "Jedi Master Holocron";
			else if ( spellID >= 270 && spellID <= 279 )
				scroll.InfoText1 = "Syth Lord Mysticron";
			else if ( spellID >= 250 && spellID <= 259 )
				scroll.InfoText1 = "Mystic Ability";
		}

		public override bool StackWith(Mobile from, Item dropped, bool playSound)
		{
			if (dropped is SpellScroll)
			{
				if (IsEphemeralSpellScroll != ((SpellScroll)dropped).IsEphemeralSpellScroll)
					return false;
			}

			return base.StackWith(from, dropped, playSound);
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( !IsEphemeralSpellScroll && from.Alive && this.Movable && !( m_SpellID >= 130 && m_SpellID <= 162 ) )
				list.Add( new ContextMenus.AddToSpellbookEntry() );
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if ( IsEphemeralSpellScroll )
			{
				list.Add( "Ephemeral Magic" );
				list.Add( "Cannot be added to spellbooks" );
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Multis.DesignContext.Check( from ) )
				return; // They are customizing

			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				return;
			}

			Spell spell = SpellRegistry.NewSpell( m_SpellID, from, this );

			if ( spell != null )
				spell.Cast();
			else
				from.SendLocalizedMessage( 502345 ); // This spell has been temporarily disabled.
		}
		
		public int OnCraft( int quality, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			IsEphemeralSpellScroll = true;

			return quality;
		}
	}
}