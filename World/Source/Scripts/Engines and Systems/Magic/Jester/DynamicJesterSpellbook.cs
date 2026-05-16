using System;
using Server.Spells;

namespace Server.Items
{
	public class DynamicJesterSpellbook : Spellbook
	{
		public override SpellbookType SpellbookType { get { return SpellbookType.DynamicJester; } }
		public override int BookOffset { get { return 2700; } }
		public override int BookCount { get { return 10; } }

		[Constructable]
		public DynamicJesterSpellbook() : this( (ulong)0 )
		{
		}

		[Constructable]
		public DynamicJesterSpellbook( ulong content ) : base( content, 0x1E3F )
		{
			Name = "bag of tricks";
			Layer = Layer.Trinket;
		}

		public DynamicJesterSpellbook( Serial serial ) : base( serial )
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
