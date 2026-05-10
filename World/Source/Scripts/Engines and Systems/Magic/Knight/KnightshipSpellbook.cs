using System;
using Server.Network;
using Server.Spells;

namespace Server.Items
{
	public class KnightshipSpellbook : Spellbook
	{
		public override string DefaultDescription
		{
			get { return "This book is used by knights, in order for them to use various abilities to spread harmony and peace throughout the land. Some books have enhanced properties, that are only effective when the book is held."; }
		}

		public override SpellbookType SpellbookType { get { return SpellbookType.Knightship; } }
		public override int BookOffset { get { return 2000; } } // Spell ID 2000 maps to bit 0: 2000 - 2000 = 0
		public override int BookCount { get { return 10; } }

		[Constructable]
		public KnightshipSpellbook() : this( (ulong)0x3FF ) // All 10 spells enabled
		{
		}

		[Constructable]
		public KnightshipSpellbook( ulong content ) : base( content, 0x2252 ) // Same item art as Book of Chivalry
		{
			Name = "knightship book";
			Layer = Layer.Trinket;
		}

		public KnightshipSpellbook( Serial serial ) : base( serial )
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
