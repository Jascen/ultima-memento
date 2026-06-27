using System;
using Server;
using Server.Mobiles;

namespace Server.Items 
{ 
	public class NecroHorse : EtherealMount 
	{ 
		[Constructable] 
		public NecroHorse() : base( 0x2617, 0x3EBB ) 
		{ 
			Name = "Undead Horse";
			ItemID = 0x2617;
			Hue = 0xB97;
		} 

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1070722, "Undead Mount For Grandmasters in Necromancy");
        } 

		public NecroHorse( Serial serial ) : base( serial ) 
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