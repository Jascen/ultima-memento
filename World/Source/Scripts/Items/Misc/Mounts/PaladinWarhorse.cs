using Server.Mobiles;

namespace Server.Items 
{ 
	public class PaladinWarhorse : EtherealMount 
	{ 
		[Constructable] 
		public PaladinWarhorse() : base( 0x4C59, 0x3EBE ) 
		{ 
			Name = "Silver Griffon";
			ItemID = 0x4C59;
			Hue = 0x99B;
		} 

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1070722, "Holy Mount For Grandmaster Knights");
        } 

		public PaladinWarhorse( Serial serial ) : base( serial ) 
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