using Server.Mobiles;

namespace Server.Items 
{ 
	public class DaemonMount : EtherealMount 
	{ 
		[Constructable] 
		public DaemonMount() : base( 11669, 384 ) 
		{ 
			Name = "Daemon Servant";
			ItemID = 11669;
			Hue = 0x4AA;
		} 

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1070722, "Evil Mount For Grandmasters in both Necromancy and Magery");
        }

		public DaemonMount( Serial serial ) : base( serial ) 
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