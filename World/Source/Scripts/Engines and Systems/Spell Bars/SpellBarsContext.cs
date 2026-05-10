namespace Server.SpellBars
{
	[PropertyObject]
	public class SpellBarsContext
	{
		[CommandProperty( AccessLevel.GameMaster )]
		public string Mage1 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Mage2 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Mage3 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Mage4 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Necro1 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Necro2 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Knight1 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Knight2 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Death1 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Death2 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Bard1 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Bard2 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Priest1 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Priest2 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Monk1 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Monk2 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Arch1 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Arch2 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Arch3 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Arch4 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Elly1 { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Elly2 { get; set; }

		public SpellBarsContext()
		{
		}

		public SpellBarsContext(GenericReader reader)
		{
			int version = reader.ReadInt();

			Mage1 = reader.ReadString();
			Mage2 = reader.ReadString();
			Mage3 = reader.ReadString();
			Mage4 = reader.ReadString();
			Necro1 = reader.ReadString();
			Necro2 = reader.ReadString();
			Knight1 = reader.ReadString();
			Knight2 = reader.ReadString();
			Death1 = reader.ReadString();
			Death2 = reader.ReadString();
			Bard1 = reader.ReadString();
			Bard2 = reader.ReadString();
			Priest1 = reader.ReadString();
			Priest2 = reader.ReadString();
			Arch1 = reader.ReadString();
			Arch2 = reader.ReadString();
			Arch3 = reader.ReadString();
			Arch4 = reader.ReadString();
			Monk1 = reader.ReadString();
			Monk2 = reader.ReadString();
			Elly1 = reader.ReadString();
			Elly2 = reader.ReadString();
		}

		public void Serialize(GenericWriter writer)
		{
			writer.Write( 1 );

			writer.Write( Mage1 );
			writer.Write( Mage2 );
			writer.Write( Mage3 );
			writer.Write( Mage4 );
			writer.Write( Necro1 );
			writer.Write( Necro2 );
			writer.Write( Knight1 );
			writer.Write( Knight2 );
			writer.Write( Death1 );
			writer.Write( Death2 );
			writer.Write( Bard1 );
			writer.Write( Bard2 );
			writer.Write( Priest1 );
			writer.Write( Priest2 );
			writer.Write( Arch1 );
			writer.Write( Arch2 );
			writer.Write( Arch3 );
			writer.Write( Arch4 );
			writer.Write( Monk1 );
			writer.Write( Monk2 );
			writer.Write( Elly1 );
			writer.Write( Elly2 );
		}
	}
}