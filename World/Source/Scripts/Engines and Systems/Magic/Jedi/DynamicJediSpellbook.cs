using System;
using Server.Spells;

namespace Server.Items
{
	public class DynamicJediSpellbook : JediSpellbook
	{
		public override SpellbookType SpellbookType { get { return SpellbookType.DynamicJedi; } }
		public override int BookOffset { get { return 2600; } }
		public override int BookCount { get { return 10; } }

		[Constructable]
		public DynamicJediSpellbook() : this( (ulong)0 )
		{
		}

		[Constructable]
		public DynamicJediSpellbook( ulong content ) : base( content, null )
		{
			Name = "jedi datacron";
		}

		public override void OnDoubleClick( Mobile from )
		{
			Container pack = from.Backpack;

			// Mirror JediSpellbook.OnDoubleClick gate: book bound to its owner.
			if ( Owner != from )
				from.SendMessage( "This device seems strange to you." );
			else if ( Parent == from || ( pack != null && Parent == pack ) )
				DisplayTo( from );
			else
				from.SendLocalizedMessage( 500207 );
		}

		public DynamicJediSpellbook( Serial serial ) : base( serial )
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
