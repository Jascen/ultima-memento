using System;
using Server.Spells;

namespace Server.Items
{
	public class DynamicResearchSpellbook : AncientSpellbook
	{
		public override SpellbookType SpellbookType { get { return SpellbookType.DynamicResearch; } }
		public override int BookOffset { get { return 2800; } }
		public override int BookCount { get { return 64; } }

		[Constructable]
		public DynamicResearchSpellbook() : this( (ulong)0 )
		{
		}

		[Constructable]
		public DynamicResearchSpellbook( ulong content ) : base( content )
		{
			Name = "ancient spellbook";
		}

		public override void OnDoubleClick( Mobile from )
		{
			Container pack = from.Backpack;

			// Mirror AncientSpellbook.OnDoubleClick gate: book bound to its owner.
			if ( Owner != from )
				from.SendMessage( "These pages appears as scribbles to you." );
			else if ( Parent == from || ( pack != null && Parent == pack ) )
				DisplayTo( from );
			else
				from.SendLocalizedMessage( 500207 );
		}

		public DynamicResearchSpellbook( Serial serial ) : base( serial )
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
