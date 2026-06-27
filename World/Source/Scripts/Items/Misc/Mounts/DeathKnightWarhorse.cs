using Server.Mobiles;

namespace Server.Items 
{ 
	public class DeathKnightWarhorse : EtherealMount 
	{ 
		[Constructable] 
		public DeathKnightWarhorse() : base( 0x2617, 0x3EBB )
		{ 
			Name = "Dread Horse";
			ItemID = 0x2617;
			Hue = 0xAB4;
		} 

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1070722, "Evil Mount For Grandmaster Death Knights");
        } 

		public DeathKnightWarhorse( Serial serial ) : base( serial ) 
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