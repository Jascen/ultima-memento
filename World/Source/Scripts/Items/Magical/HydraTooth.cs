using System.Collections.Generic;
using Server.ContextMenus;
using Server.Spells;
using Server.Spells.Magical;

namespace Server.Items
{
	public class HydraTooth : SpellScroll
	{
		[Constructable]
		public HydraTooth() : this( 1 )
		{
		}

		[Constructable]
		public HydraTooth( int amount ) : base( 704, 0x5747, amount )
		{
			Name = "hydra tooth";
			Stackable = false;
			Amount = 1;
			ItemID = 0x5747;
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

			Spell spell = new SummonSkeletonSpell( from, this );
			spell.Cast();
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			// suppress
		}

		public HydraTooth( Serial serial ) : base( serial )
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