using System; 
using Server; 
using Server.Items; 
using Server.Network; 
using Server.Misc; 
using Server.Gumps;

namespace Server.Items
{
	// This book has been deprecated in favor of external documentation
	public class BeginnerBook : Item
	{
		[Constructable]
		public BeginnerBook() : base( 0x0FF1 )
		{
			Name = "The Journey Begins";
		}

		public void TitleBook()
		{
			if ( ColorText1 == null && X > 0 )
			{
				ColorText1 = "The Journey Begins";
				ColorText2 = "How to start a new";
				ColorText3 = "life in this world";
				ColorHue1 = "FF9900";
				ColorHue2 = "B57B24";
				ColorHue3 = "B57B24";
			}
		}

        public override void OnAfterSpawn()
        {
			TitleBook();
			base.OnAfterSpawn();
		}

		public override void OnAdded( object parent )
		{
			TitleBook();
			base.OnAdded( parent );
		}

		public override void OnLocationChange( Point3D oldLocation )
		{
			TitleBook();
			base.OnLocationChange( oldLocation );
		}

		public BeginnerBook( Serial serial ) : base( serial )
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