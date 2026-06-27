namespace Server.Items
{
	public class MysticMonkRobe : BaseGiftOuterTorso
	{
		[Constructable]
		public MysticMonkRobe() : this( 0 )
		{
		}

		[Constructable]
		public MysticMonkRobe( int hue ) : base( 0x1F03, hue )
		{
			Name = "robe";
			Weight = 2.0;
			LootType = LootType.Blessed;
		}

		public override bool CanEquip( Mobile from )
		{
			if ( Owner != from )
			{
				from.SendMessage( "You cannot seem to wear the robe!" );
				return false;
			}

			return base.CanEquip( from );
		}

		public override bool DisplayLootType{ get{ return false; } }

        public MysticMonkRobe(Serial serial) : base(serial)
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