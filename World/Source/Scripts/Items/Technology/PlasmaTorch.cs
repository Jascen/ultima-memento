using Server.Utilities;

namespace Server.Items
{
	public class PlasmaTorch : Item
	{
		public override double DefaultWeight
		{
			get { return 1.0; }
		}

		[Constructable]
		public PlasmaTorch() : base( 0x2D86 )
		{
			Name = "plasma torch";
			Technology = true;
			InfoText1 = "used to melt through most";
			InfoText2 = "chest traps and locks";
			Stackable = true;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1060640 ); // The item must be in your backpack to use it.
			}
			else
			{
				from.SendMessage( "What chest do you want to use the torch on?" );
				UnlockUtilities.BeginTrapDissolverUnlock( from, this, UnlockUtilities.PlasmaTorchProfile );
			}
		}

		public PlasmaTorch( Serial serial ) : base( serial )
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
