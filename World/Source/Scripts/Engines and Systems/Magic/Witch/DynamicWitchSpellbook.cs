using System;
using Server.Spells;

namespace Server.Items
{
	public class DynamicWitchSpellbook : Spellbook
	{
		public override SpellbookType SpellbookType { get { return SpellbookType.DynamicWitch; } }
		public override int BookOffset { get { return 3100; } }
		public override int BookCount { get { return 16; } }

		[Constructable]
		public DynamicWitchSpellbook() : this( (ulong)0 )
		{
		}

		[Constructable]
		public DynamicWitchSpellbook( ulong content ) : base( content, 0x5776 )
		{
			Name = "witch spellbook";
			Layer = Layer.Trinket;
		}

		public DynamicWitchSpellbook( Serial serial ) : base( serial )
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
