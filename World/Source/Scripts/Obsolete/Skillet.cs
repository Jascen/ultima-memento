using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	public class Skillet : BaseTool
	{
		public override int LabelNumber{ get{ return 1044567; } } // skillet

		public override CraftSystem CraftSystem{ get{ return DefCooking.CraftSystem; } }

		[Constructable]
		public Skillet() : base( 0x97F )
		{
			Weight = 1.0;
		}

		[Constructable]
		public Skillet( int uses ) : base( uses, 0x97F )
		{
			Weight = 1.0;
		}

		public Skillet( Serial serial ) : base( serial )
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
			Item item = new CulinarySet();
			Server.Misc.Cleanup.DoCleanup( (Item)state, item );
		}
	}
}