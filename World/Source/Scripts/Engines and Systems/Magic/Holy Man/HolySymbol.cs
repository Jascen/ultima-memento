using System;
using Server;
using Server.Misc;
using Server.Mobiles;

namespace Server.Items
{
	public class HolySymbol : TrinketTalisman
	{
		public Mobile owner;
		public int BanishedEvil;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner { get{ return owner; } set{ owner = value; } }

		[CommandProperty(AccessLevel.Owner)]
		public int Banished_Evil { get { return BanishedEvil; } set { BanishedEvil = value; InvalidateProperties(); } }

		[Constructable]
		public HolySymbol() : this(null)
		{
		}

		public HolySymbol( Mobile gifted )
		{
			Name = "holy symbol";
			ItemID = 0x2C7F;
			Hue = 0x47E;
			owner = gifted;
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			string sPower = string.Format("{0:n0}", BanishedEvil);
            if ( owner != null ){ list.Add( 1070722, "Piety for " + owner.Name + ": " + sPower + ""); }
        } 

		public override bool CanEquip( Mobile from )
		{
			if ( owner != from )
			{
				from.SendMessage( "This is not your holy symbol!" );
				return false;
			}

			return base.CanEquip( from );
		}

		public HolySymbol( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
			writer.Write( (Mobile)owner);
            writer.Write( BanishedEvil );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			owner = reader.ReadMobile();
			BanishedEvil = reader.ReadInt();
		}
	}
}