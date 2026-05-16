using System;
using Server.Network;
using Server.Spells;
using Server.Spells.Elementalism;
using Server.Mobiles;

namespace Server.Items
{
	public class DynamicElementalSpellbook : Spellbook
	{
		public override string DefaultDescription{ get{ return "This book is used by elementalists, where they can record the elemental magic they can unleash. Dropping such scrolls onto this book will place the spell within its pages. Some books have enhanced properties, that are only effective when the book is held."; } }

		public override SpellbookType SpellbookType{ get{ return SpellbookType.DynamicElementalism; } }
		public override int BookOffset{ get{ return 2100; } }
		public override int BookCount{ get{ return 32; } }

		[Constructable]
		public DynamicElementalSpellbook() : this( (ulong)0 )
		{
		}

		[Constructable]
		public DynamicElementalSpellbook( ulong content ) : base( content, 0x6713 )
		{
			Layer = Layer.Trinket;
			Name = "elemental spellbook";
			ItemID = Utility.RandomList( 0x6713, 0x6715, 0x6717, 0x6719 );
		}

		public override bool OnDragLift( Mobile from )
		{
			if ( from is PlayerMobile )
				SetupBook( from );

			return base.OnDragLift( from );
		}

		public override void OnDoubleClick( Mobile from )
		{
			SetupBook( from );
			Container pack = from.Backpack;

			if ( !ElementalSpell.CanUseBook( this, from, true ) )
			{
				// Element mismatch - don't open
			}
			else if ( Parent == from || ( pack != null && Parent == pack ) )
			{
				DisplayTo( from );
			}
			else
			{
				from.SendLocalizedMessage( 500207 ); // The spellbook must be in your backpack (and not in a container within) to open.
			}
		}

		// Accept Elementalism scrolls (IDs 300-331) and map them to our offset (2100-2131)
		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is SpellScroll && dropped.Amount == 1 )
			{
				SpellScroll scroll = (SpellScroll)dropped;

				// Check if this is an Elementalism scroll (300-331)
				if ( scroll.SpellID >= 300 && scroll.SpellID < 332 )
				{
					int mappedBit = scroll.SpellID - 300; // 0-31

					if ( HasSpell( mappedBit + BookOffset ) )
					{
						from.SendLocalizedMessage( 500179 ); // That spell is already present in that spellbook.
						return false;
					}

					if ( mappedBit >= 0 && mappedBit < BookCount )
					{
						Content |= (ulong)1 << mappedBit;

						InvalidateProperties();
						scroll.Delete();

						from.Send( new Network.PlaySound( 0x249, GetWorldLocation() ) );
						return true;
					}
				}
			}

			return base.OnDragDrop( from, dropped );
		}

		public void SetupBook( Mobile from )
		{
			if ( from is PlayerMobile )
				ElementalSpell.BookCover( this, ((PlayerMobile)from).CharacterElement );
		}

		public DynamicElementalSpellbook( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
