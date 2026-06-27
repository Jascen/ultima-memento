using Server.Mobiles;

namespace Server.Items 
{ 
	public class Warhorse : EtherealMount 
	{ 
		[Constructable] 
		public Warhorse() : base( 0x55DC, 594 ) 
		{ 
			Name = "warhorse";
			Hue = 0;
			ItemID = 0x55DC;
			RegularID = 0x55DC;
			MountedID = 594;
		} 

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1070722, "Mount For Grandmaster Warriors");
        } 

		public Warhorse( Serial serial ) : base( serial ) 
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