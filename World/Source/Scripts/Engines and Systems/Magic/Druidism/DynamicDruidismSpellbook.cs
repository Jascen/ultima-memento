using System;
using Server.Spells;

namespace Server.Items
{
	public class DynamicDruidismSpellbook : Spellbook
	{
		public override SpellbookType SpellbookType { get { return SpellbookType.DynamicDruidism; } }
		public override int BookOffset { get { return 2400; } }
		public override int BookCount { get { return 16; } }

		[Constructable]
		public DynamicDruidismSpellbook() : this( (ulong)0 )
		{
		}

		[Constructable]
		public DynamicDruidismSpellbook( ulong content ) : base( content, 0x5688 )
		{
			Name = "druid brewing book";
			Layer = Layer.Trinket;
		}

		public DynamicDruidismSpellbook( Serial serial ) : base( serial )
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
