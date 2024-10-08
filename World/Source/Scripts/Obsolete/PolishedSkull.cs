using System;
using Server;

namespace Server.Items
{
	public class PolishedSkull : Item
	{
		[Constructable]
		public PolishedSkull() : this( 1 )
		{
		}

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		[Constructable]
		public PolishedSkull( int amount ) : base( 0x26AB )
		{
			Name = "polished skull";
			Stackable = true;
			Amount = amount;
		}

		public PolishedSkull( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( Cleanup ), this );
		}

		private void Cleanup( object state )
		{
			Item item = new BrittleSkeletal();
			item.Amount = ((Item)state).Amount;
			Server.Misc.Cleanup.DoCleanup( (Item)state, item );
		}
	}
}