using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a hoard minion corpse" )]
	public class HoardMinionFamiliar : HordeMinionFamiliar
	{
		public HoardMinionFamiliar()
		{
			Name = "a hoard minion";
			Blessed = true;
			ControlSlots = 1;
			SetStr( 65000 );
			Container pack = Backpack;

			if ( pack != null )
				pack.Delete();

			pack = new StrongBackpack();
			pack.Movable = false;

			AddItem( pack );
		}

		public HoardMinionFamiliar( Serial serial ) : base( serial )
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
			Timer.DelayCall( TimeSpan.FromMinutes( HoardMinionFamiliarItem.SUMMON_DURATION_MINUTES ), () => Delete() );
		}
	}
}