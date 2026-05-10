using Server.Gumps;

namespace Server.Items
{
	[FlipableAttribute( 0x671B, 0x671C )]
	public class SongBook : Spellbook
	{
		public override string DefaultDescription{ get{ return "This book is used by bards to write the mystical songs they find. The songs within the book can be used to produce varying magical effects. These songs require the use of a musical instrument. Dropping such scrolls onto this book will place the song within its pages. Some books have enhanced properties, that are only effective when the book is held."; } }

		public override SpellbookType SpellbookType{ get{ return SpellbookType.Song; } }
		public override int BookOffset{ get{ return 351; } }
		public override int BookCount{ get{ return 16; } }

		[Constructable]
		public SongBook() : this( (ulong)0 )
		{
		}

		[Constructable]
		public SongBook( ulong content ) : base( content, 0x671B )
		{
			Name = "bardic songs";
			Layer = Layer.Trinket;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( GetWorldLocation(), 1 ) )
			{
				from.CloseGump( typeof( SongBookGump ) );
				from.SendGump( new SongBookGump( from, this, 1 ) );
			}
		}

		public static string SpellDescription( int spell )
		{
			string txt = "This is a bardic song: ";
			string skl = "0";

			if ( spell == 351 ){ 	skl = "55";	txt = "An area of effect that regenerates your party's health slowly."; }
			else if ( spell == 352 ){ 	skl = "60";	txt = "An area of effect that raises the intelligence of your party."; }
			else if ( spell == 353 ){ 	skl = "50";	txt = "An area of effect that raises the energy resistance of your party."; }
			else if ( spell == 354 ){ 	skl = "70";	txt = "Lowers the energy resistance of your target."; }
			else if ( spell == 355 ){ 	skl = "50";	txt = "An area of effect that raises the fire resistance of your party."; }
			else if ( spell == 356 ){ 	skl = "70";	txt = "Lowers the fire resistance of your target."; }
			else if ( spell == 357 ){ 	skl = "80";	txt = "Damages your target with a burst of sonic energy."; }
			else if ( spell == 358 ){ 	skl = "50";	txt = "An area of effect that raises the cold resistance of your party."; }
			else if ( spell == 359 ){ 	skl = "70";	txt = "Lowers the ice resistance of your target."; }
			else if ( spell == 360 ){ 	skl = "50";	txt = "An area of effect that raises the physical resist of your party."; }
			else if ( spell == 361 ){ 	skl = "55";	txt = "An area of effect that regenerates your party's mana slowly."; }
			else if ( spell == 362 ){ 	skl = "90";	txt = "An area of effect that dispels all summoned creatures around you."; }
			else if ( spell == 363 ){ 	skl = "50";	txt = "An area of effect that raises the poison resistance of your party."; }
			else if ( spell == 364 ){ 	skl = "70";	txt = "Lowers the poison resistance of your target."; }
			else if ( spell == 365 ){ 	skl = "60";	txt = "An area of effect that raises the dexterity of your party."; }
			else if ( spell == 366 ){ 	skl = "60";	txt = "An area of effect that raises the strength of your party."; }

			if ( skl == "0" )
				return txt;

			return txt + " It requires at least a " + skl + " in Musicianship to perform.";
		}

		public SongBook( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			switch( version )
			{
				case 0:
				{
					var Instrument = version < 1 ? reader.ReadItem() as BaseInstrument : null;
					break;
				}
			}

			if ( ItemID != 0x671B && ItemID != 0x671C )
				ItemID = 0x671B;
		}
	}
}
