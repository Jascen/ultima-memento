using Server.Network;

namespace Server.Items
{
	public class HikingBoots : BaseShoes
	{
		[Constructable]
		public HikingBoots() : base( 0x2FC4 )
		{
			Name = "hiking boots";
			CoinPrice = 5;
		}

		public override bool OnEquip( Mobile from )
		{
			if ( MySettings.S_NoMountsInCertainRegions && Server.Mobiles.AnimalTrainer.IsNoMountRegion( from, Region.Find( from.Location, from.Map ) ) )
			{
				from.Send(SpeedControl.Disable);
				Weight = 5.0;
			}
			else
			{
				Weight = 3.0;
				from.Send(SpeedControl.MountSpeed);
			}
		
			return base.OnEquip(from);
		}

        public override bool CanEquip( Mobile from )
        {
            if ( !base.CanEquip(from) ) return false;
			if ( from.RaceID > 0 ) return true;

			from.SendMessage( "This won't fit Humans." );
			return false;
        }

        public override void OnRemoved( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile from = (Mobile)parent;
				if ( from.RaceID > 0 ){ from.Send(SpeedControl.Disable); }
			}
			base.OnRemoved(parent);
		}

        public override void AddNameProperty(ObjectPropertyList list)
        {
            base.AddNameProperty(list);
			
			list.Add("[Monster races only]");
			list.Add("Increase movement speed");
        }

		public HikingBoots( Serial serial ) : base( serial )
		{
		}

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( !MyServerSettings.MonstersAllowed() )
				this.Delete();
		}
	}
}
