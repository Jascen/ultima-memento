using System;
using Server.Spells;

namespace Server.Items
{
	public class DynamicSythSpellbook : SythSpellbook
	{
		public override SpellbookType SpellbookType { get { return SpellbookType.DynamicSyth; } }
		public override int BookOffset { get { return 3000; } }
		public override int BookCount { get { return 10; } }

		[Constructable]
		public DynamicSythSpellbook() : this( (ulong)0 )
		{
		}

		[Constructable]
		public DynamicSythSpellbook( ulong content ) : base( content, null )
		{
			Name = "syth datacron";
		}

		public override void OnDoubleClick( Mobile from )
		{
			Container pack = from.Backpack;

			// Mirror SythSpellbook.OnDoubleClick gate: book bound to its owner.
			if ( Owner != from )
				from.SendMessage( "This device seems strange to you." );
			else if ( Parent == from || ( pack != null && Parent == pack ) )
				DisplayTo( from );
			else
				from.SendLocalizedMessage( 500207 );
		}

		public DynamicSythSpellbook( Serial serial ) : base( serial )
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
