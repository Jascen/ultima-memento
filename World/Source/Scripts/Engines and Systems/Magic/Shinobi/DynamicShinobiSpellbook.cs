using System;
using Server.Spells;

namespace Server.Items
{
	public class DynamicShinobiSpellbook : Spellbook
	{
		public override SpellbookType SpellbookType { get { return SpellbookType.DynamicShinobi; } }
		public override int BookOffset { get { return 2900; } }
		public override int BookCount { get { return 8; } }

		[Constructable]
		public DynamicShinobiSpellbook() : this( (ulong)0 )
		{
		}

		[Constructable]
		public DynamicShinobiSpellbook( ulong content ) : base( content, 0x5C15 )
		{
			Name = "shinobi scroll";
			Layer = Layer.Trinket;
		}

		public DynamicShinobiSpellbook( Serial serial ) : base( serial )
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
