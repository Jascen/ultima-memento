using System;

namespace Server.Items
{
	public class Bottle : Item
	{
		public override string DefaultDescription{ get{ return "These bottles are often used by alchemists to store potions in."; } }

		[Constructable]
		public Bottle() : this( 1 )
		{
		}

		[Constructable]
		public Bottle( int amount ) : base( 0xF0E )
		{
			Stackable = true;
			Weight = 0.5;
			Amount = amount;
			Built = true;
		}

		public Bottle( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if (version < 1) Weight = 0.5;
		}
	}
}