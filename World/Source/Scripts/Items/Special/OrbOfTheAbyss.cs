using System;
using Server;
using Server.Misc;

namespace Server.Items
{
	public class OrbOfTheAbyss : TrinketTalisman
	{
		public Mobile owner;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner { get{ return owner; } set{ owner = value; } }

		[Constructable]
		public OrbOfTheAbyss()
		{
			Name = "orb of the abyss";
			ItemID = 0x2C84;
			Hue = 0x489;
		}

		public override bool CanEquip( Mobile from )
		{
			if ( owner != from )
			{
				from.SendMessage( "This is not your orb!" );
				return false;
			}

			return base.CanEquip( from );
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
            if ( owner != null ){ list.Add( 1049644, "Belongs to " + owner.Name + ""); }
        } 

		public OrbOfTheAbyss( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
			writer.Write( (Mobile)owner);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			owner = reader.ReadMobile();

			if ( ItemID != 0x2C84 && ItemID < 26413 ){ ItemID = 0x2C84; Name = "orb of the abyss"; }
		}

		public static bool ChangeOrb( Mobile m, Mobile tinker, Item orb )
		{
			if ( orb.ItemID == 0x2C84 ){ orb.ItemID = 0x6745; orb.Name = "ring of the abyss"; orb.Layer = Layer.Ring; }
			else if ( orb.ItemID == 0x6745 ){ orb.ItemID = 0x6744; orb.Name = "amulet of the abyss"; orb.Layer = Layer.Neck; }
			else if ( orb.ItemID == 0x6744 ){ orb.ItemID = 0x6741; orb.Name = "bracelet of the abyss"; orb.Layer = Layer.Bracelet; }
			else if ( orb.ItemID == 0x6741 ){ orb.ItemID = 0x6743; orb.Name = "earrings of the abyss"; orb.Layer = Layer.Earrings; }
			else { orb.ItemID = 0x2C84; orb.Name = "orb of the abyss"; orb.Layer = Layer.Trinket; }

			tinker.Say( "Here. You now have the " + orb.Name + "." );

			m.PlaySound( 0x542 );

			return false;
		}
	}
}