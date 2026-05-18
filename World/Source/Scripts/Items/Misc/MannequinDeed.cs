using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Spells;
using Server.Targeting;

namespace Server.Items
{
	public class MannequinDeed : Item
	{
		[Constructable]
		public MannequinDeed() : base( 0x14F0 )
		{
			Name = "a mannequin deed";
			Weight = 1.0;
		}

		public MannequinDeed( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				return;
			}

			from.SendMessage( "Where would you like to place the mannequin?" );
			from.Target = new MannequinPlacementTarget( this );
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

	public class MannequinPlacementTarget : Target
	{
		private MannequinDeed m_Deed;

		public MannequinPlacementTarget( MannequinDeed deed ) : base( 8, true, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			if ( m_Deed == null || m_Deed.Deleted )
				return;

			if ( !m_Deed.IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				return;
			}

			IPoint3D p = targeted as IPoint3D;

			if ( p == null )
			{
				from.SendLocalizedMessage( 1042001 );
				return;
			}

			SpellHelper.GetSurfaceTop( ref p );

			Point3D loc = new Point3D( p.X, p.Y, p.Z );
			Map map = from.Map;

			if ( map == null || map == Map.Internal )
			{
				from.SendLocalizedMessage( 500269 ); // You cannot build that there.
				return;
			}

			BaseHouse house = null;
			AddonFitResult result = CharacterStatueTarget.CouldFit( loc, map, from, ref house );

			switch ( result )
			{
				case AddonFitResult.Blocked:
				{
					from.SendLocalizedMessage( 500269 ); // You cannot build that there.
					return;
				}
				case AddonFitResult.NotInHouse:
				{
					from.SendLocalizedMessage( 500269 ); // You cannot build that there.
					return;
				}
				case AddonFitResult.DoorTooClose:
				{
					from.SendLocalizedMessage( 500271 ); // You cannot build near the door.
					return;
				}
			}

			if ( house == null || !house.IsCoOwner( from ) )
			{
				from.SendLocalizedMessage( 500269 ); // You cannot build that there.
				return;
			}

			Mannequin m = new Mannequin( house );
			m.Direction = from.Direction & Direction.Mask;
			m.MoveToWorld( loc, map );

			m_Deed.Delete();
		}
	}
}
