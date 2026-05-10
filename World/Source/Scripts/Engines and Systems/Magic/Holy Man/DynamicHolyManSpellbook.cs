using System;
using Server.Spells;

namespace Server.Items
{
	public class DynamicHolyManSpellbook : HolyManSpellbook
	{
		public override SpellbookType SpellbookType { get { return SpellbookType.DynamicHolyMan; } }
		public override int BookOffset { get { return 2500; } }
		public override int BookCount { get { return 14; } }

		[Constructable]
		public DynamicHolyManSpellbook() : this( (ulong)0 )
		{
		}

		[Constructable]
		public DynamicHolyManSpellbook( ulong content ) : base( content, null )
		{
			Name = "holy man spellbook";
		}

		public override void OnDoubleClick( Mobile from )
		{
			Container pack = from.Backpack;

			// Mirror HolyManSpellbook.OnDoubleClick gate: book bound to its owner.
			if ( Owner != from )
				from.SendMessage( "These pages appears as scribbles to you." );
			else if ( Parent == from || ( pack != null && Parent == pack ) )
				DisplayTo( from );
			else
				from.SendLocalizedMessage( 500207 );
		}

		public DynamicHolyManSpellbook( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
