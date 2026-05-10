using System;
using System.Collections;
using Server.Network;
using Server.Regions;
using Server.Mobiles;

namespace Server.Items
{
	public class Kindling : Item
	{
		private const int CAMPFIRE_COOLDOWN_MINUTES = 3;

		public override string DefaultDescription{ get{ return "Kindling can be used by proficient campers, to make a campfire. The fire can then be used by adventurers, to recovery health and stamina much quicker. There cannot be any enemies nearby for anyone to benefit from the effects."; } }

		[Constructable]
		public Kindling() : this( 1 )
		{
		}

		[Constructable]
		public Kindling( int amount ) : base( 0xDE1 )
		{
			Stackable = true;
			Weight = 1.0;
			Amount = amount;
		}

		public Kindling( Serial serial ) : base( serial )
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
		}

		public static bool EnemiesNearby( Mobile from )
		{
			if ( from is PlayerMobile && from.Combatant != null && from.InRange( from.Combatant.Location, 20 ) && from.Combatant.InLOS( from ) )
				return true;

			foreach( Mobile m in from.GetMobilesInRange( 20 ) )
			{
				if ( m is BaseCreature && !BaseCreature.IsCitizen( m ) && !((BaseCreature)m).Controlled && !((BaseCreature)m).Summoned && ((BaseCreature)m).FightMode == FightMode.Closest )
					return true;
			}

			return false;
		}

		public static bool CampAllowed( Mobile from )
		{
			if ( from.Region.IsPartOf( typeof( PublicRegion ) ) )
				return false;

			if ( from.Region.IsPartOf( typeof( ProtectedRegion ) ) )
				return false;

			if ( from.Region.IsPartOf( typeof( PirateRegion ) ) )
				return false;

			if ( from.Region.IsPartOf( typeof( SafeRegion ) ) )
				return false;

			return true;
		}

		private bool CampsNearby()
		{
			foreach( Item i in GetItemsInRange( 20 ) )
			{
				if ( i is Campfire )
				{
					Campfire fire = (Campfire)i;

					if ( fire.Status != CampfireStatus.Off )
						return true;
				}
			}

			return false;
		}

		public static void RaiseCamping( Mobile m )
		{
			int cycle = 10;

			while ( cycle > 0 )
			{
				cycle--;
				m.CheckSkill( SkillName.Camping, 0, 125 );
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)from;

				if ( !this.VerifyMove( from ) )
					return;

				bool inCombat = ( from.Combatant != null && from.InRange( from.Combatant.Location, 20 ) && from.Combatant.InLOS( from ) );

				if ( !from.InRange( this.GetWorldLocation(), 2 ) )
				{
					from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
					return;
				}
				else if ( Server.Misc.Worlds.IsOnBoat( from ) )
				{
					from.SendMessage( "You cannot start a campfire on a boat." );
					return;
				}
				else if ( inCombat )
				{
					from.SendMessage( "You cannot start a campfire while in combat." );
					return;
				}
				else if ( CampsNearby() )
				{
					from.SendMessage( "There is already a camp nearby!" );
				}
				else if ( EnemiesNearby( from ) )
				{
					from.SendMessage( "It is not safe enough to setup camp!" );
				}
				else if ( DateTime.Now >= pm.Camp )
				{
					Point3D fireLocation = GetFireLocation( from );

					if ( fireLocation == Point3D.Zero )
					{
						from.SendLocalizedMessage( 501695 ); // There is not a spot nearby to place your campfire.
					}
					else if ( !from.CheckSkill( SkillName.Camping, 0.0, 100.0 ) )
					{
						Server.Items.Kindling.RaiseCamping( from );
						from.SendLocalizedMessage( 501696 ); // You fail to ignite the campfire.
					}
					else
					{
						Server.Items.Kindling.RaiseCamping( from );
						Consume();

						if ( !this.Deleted && this.Parent == null )
							from.PlaceInBackpack( this );

						new Campfire().MoveToWorld( fireLocation, from.Map );
						pm.Camp = DateTime.Now + TimeSpan.FromMinutes( CAMPFIRE_COOLDOWN_MINUTES );
					}
				}
				else
				{
					from.SendMessage( "You can only build a campfire every {0} minutes!", CAMPFIRE_COOLDOWN_MINUTES);
				}
			}
		}

		private Point3D GetFireLocation( Mobile from )
		{
			if ( !CampAllowed( from ) )
				return Point3D.Zero;

			if ( this.Parent == null )
				return this.Location;

			ArrayList list = new ArrayList( 4 );

			AddOffsetLocation( from,  0, -1, list );
			AddOffsetLocation( from, -1,  0, list );
			AddOffsetLocation( from,  0,  1, list );
			AddOffsetLocation( from,  1,  0, list );

			if ( list.Count == 0 )
				return Point3D.Zero;

			int idx = Utility.Random( list.Count );
			return (Point3D) list[idx];
		}

		private void AddOffsetLocation( Mobile from, int offsetX, int offsetY, ArrayList list )
		{
			Map map = from.Map;

			int x = from.X + offsetX;
			int y = from.Y + offsetY;

			Point3D loc = new Point3D( x, y, from.Z );

			if ( map.CanFit( loc, 1 ) && from.InLOS( loc ) )
			{
				list.Add( loc );
			}
			else
			{
				loc = new Point3D( x, y, map.GetAverageZ( x, y ) );

				if ( map.CanFit( loc, 1 ) && from.InLOS( loc ) )
					list.Add( loc );
			}
		}
	}
}