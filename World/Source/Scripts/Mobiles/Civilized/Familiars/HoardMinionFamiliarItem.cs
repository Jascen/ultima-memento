using System;
using Server.Mobiles;

namespace Server.Items
{
	public class HoardMinionFamiliarItem : Item
	{
		private const int SUMMON_DURATION_MINUTES = 10;

		[Constructable]
		public HoardMinionFamiliarItem() : base( 0x2611 )
		{
			Name = "A hoard minion";
			Light = LightType.Circle300;
			Weight = 1.0;
		}

		public HoardMinionFamiliarItem( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			int availableFollowers = from.FollowersMax - from.Followers;

			if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001);
			}
			else if ( availableFollowers < 1 )
			{
				from.SendMessage("You already have too many in your group.");
			}
			else
			{
				Map map = from.Map;
				bool validLocation = false;
				Point3D loc = from.Location;

				for ( int j = 0; !validLocation && j < 10; ++j )
				{
					int x = from.X + Utility.Random( 3 ) - 1;
					int y = from.Y + Utility.Random( 3 ) - 1;
					int z = map.GetAverageZ( x, y );

					if ( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
						loc = new Point3D( x, y, Z );
					else if ( validLocation = map.CanFit( x, y, z, 16, false, false ) )
						loc = new Point3D( x, y, z );
				}

				if ( !validLocation ) return;

				BaseCreature friend = new HoardMinionFamiliar();
				friend.ControlMaster = from;
				friend.Controlled = true;
				friend.ControlOrder = OrderType.Come;
				friend.Loyalty = 100;
				friend.Summoned = true;
				friend.SummonMaster = from;
				friend.Blessed = true;
				friend.MoveToWorld( loc, map );
				Timer.DelayCall( TimeSpan.FromMinutes( SUMMON_DURATION_MINUTES ), () => friend.Delete() );

				friend.FixedParticles( 0x374A, 1, 15, 5054, 23, 7, EffectLayer.Head );
				friend.PlaySound( 0x1F9 );
				from.FixedParticles( 0x0000, 10, 5, 2054, EffectLayer.Head );

				Delete();
			}
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			list.Add(string.Format("Summons a hoard minion for {0} minutes", SUMMON_DURATION_MINUTES));
			list.Add("[Automatically picks up stackable items off the ground]");
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
		}
	}
}