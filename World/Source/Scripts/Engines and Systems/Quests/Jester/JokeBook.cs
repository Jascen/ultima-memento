using Server.Misc;
using Server.Mobiles;

namespace Server.Items
{
	public class JokeBook : Item
	{
		public override string DefaultDescription { get { return "These books are said to be cursed, and anyone who reads them will be cursed to tell jokes for the rest of their life. You see a note on the inside cover: 'Chuckles, may you laugh until your sides split!'"; } }

		[Constructable]
		public JokeBook() : base( 0x1A98 )
		{
			Weight = 1.0;
			Name = RandomThings.MagicWandOwner() + " Book of Jokes";
			Hue = 0xAFF;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from is PlayerMobile )
			{
				switch ( Utility.RandomMinMax( 0, 8 ) ) 
				{
					case 0: from.PlaySound( from.Female ? 801 : 1073 );		from.Say( "*laughs*" );	break;
					case 1: from.PlaySound( from.Female ? 801 : 1073 );		from.Say( "Good one!" );	break;
					case 2: from.PlaySound( from.Female ? 801 : 1073 );		from.Say( "I never heard that one before!" );	break;
					case 3: from.PlaySound( from.Female ? 801 : 1073 );		from.Say( "I always like a good laugh!" );	break;
					case 4: from.PlaySound( from.Female ? 801 : 1073 );		from.Say( "That has me in tears!" );	break;
					case 5: from.Say( "I don't get it." );							break;
					case 6: from.Say( "What does that even mean?" );				break;
					case 7: from.Say( "Is that supposed to be funny?" );			break;
					case 8: from.Say( "An orc and an elf walk into a tavern?" );	break;
				}
			}
		}

		public JokeBook( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( ( int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}