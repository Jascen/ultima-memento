using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Misc;

namespace Server.Multis
{
	public abstract class BaseDockedBoat : Item
	{
		private int m_MultiID;
		private Point3D m_Offset;
		private string m_ShipName;

		[CommandProperty( AccessLevel.GameMaster )]
		public int MultiID{ get{ return m_MultiID; } set{ m_MultiID = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Offset{ get{ return m_Offset; } set{ m_Offset = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public string ShipName{ get{ return m_ShipName; } set{ m_ShipName = value; InvalidateProperties(); } }

		public BaseDockedBoat( int id, Point3D offset, BaseBoat boat ) : base( 0x14F4 )
		{
			Weight = 1.0;
			LootType = LootType.Blessed;

			m_MultiID = id;
			m_Offset = offset;

			m_ShipName = boat.ShipName;

			if ( BaseBoat.isCarpet( boat ) )
			{
				if ( Hue < 1 ){ Hue = 0xABB; }
				ItemID = 0x0A59;
				Name = "magic carpet";
			}

			if ( Hue < 1 ){ Hue = 0x5BE; }
		}

		public BaseDockedBoat( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( m_MultiID );
			writer.Write( m_Offset );
			writer.Write( m_ShipName );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				case 0:
				{
					m_MultiID = reader.ReadInt();
					m_Offset = reader.ReadPoint3D();
					m_ShipName = reader.ReadString();

					if ( version == 0 )
						reader.ReadUInt();

					break;
				}
			}

			if ( LootType == LootType.Newbied )
				LootType = LootType.Blessed;

			if ( Weight == 0.0 )
				Weight = 1.0;

			if ( Hue < 1 ){ Hue = 0x5BE; }
		}

		public override void OnDoubleClick( Mobile from )
		{
			string phrase_a = "Where do you wish to place the ship?";
			string phrase_b = "You may not place a boat from this location.";
			if ( BaseBoat.isCarpet( Boat ) )
			{
				phrase_a = "Where do you wish to place the carpet?";
				phrase_b = "There is not magic from the carpet in this location.";
			}

			Region reg = Region.Find( from.Location, from.Map );

			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if ( DockSearch.NearDock(from) == false && !BaseBoat.isCarpet( Boat ) )
			{
				from.SendMessage( "You must be near a dock to launch your ship!" );
			}
			else if (
				Server.Misc.Worlds.IsSeaTown( from.Location, from.Map ) || 
				reg.IsPartOf( typeof( OutDoorBadRegion ) ) || 
				reg.IsPartOf( typeof( VillageRegion ) ) || 
				reg.IsPartOf( typeof( BargeDeadRegion ) ) || 
				reg.IsPartOf( typeof( NecromancerRegion ) ) || 
				reg.IsPartOf( typeof( DeadRegion ) ) || 
				reg.IsPartOf( typeof( PirateRegion ) ) || 
				reg.IsPartOf( typeof( OutDoorRegion ) ) || 
				reg.IsPartOf( typeof( PublicRegion ) ) || 
				Server.Misc.Worlds.IsMainRegion( Server.Misc.Worlds.GetRegionName( from.Map, from.Location ) ) )
			{
				from.LocalOverheadMessage(Network.MessageType.Emote, 0x25, false, phrase_a);
				from.Target = new InternalTarget( this );
			}
			else
			{
				from.LocalOverheadMessage(Network.MessageType.Emote, 0x25, false, phrase_b);
			}
		}

		public abstract BaseBoat Boat{ get; }

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( m_ShipName != null )
				list.Add( m_ShipName );
			else
				base.AddNameProperty( list );
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( m_ShipName != null )
				LabelTo( from, m_ShipName );
			else
				base.OnSingleClick( from );
		}

		public void OnPlacement( Mobile from, Point3D p, int hue )
		{
			if ( Deleted )
			{
				return;
			}
			else if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				string phrase_a = "You must be near a dock to launch your ship!";
				string phrase_b = "A ship can not be launched here.";
				if ( BaseBoat.isCarpet( Boat ) )
				{
					phrase_b = "The magic of the carpet cannot be used here.";
				}

				Map map = from.Map;
				Region reg = Region.Find( from.Location, from.Map );

				if ( map == null )
					return;

				if ( from.Region.IsPartOf( typeof( HouseRegion ) ) || BaseBoat.FindBoatAt( from, from.Map ) != null )
				{
					from.SendMessage( phrase_a );
					return;
				}

				BaseBoat boat = Boat;
				boat.Hue = hue;

				if ( boat == null )
					return;

				p = new Point3D( p.X - m_Offset.X, p.Y - m_Offset.Y, p.Z - m_Offset.Z );

				bool CanBuild = false;

				if ( reg.IsPartOf( typeof( OutDoorBadRegion ) ) || 
					 reg.IsPartOf( typeof( VillageRegion ) ) || 
					 reg.IsPartOf( typeof( BargeDeadRegion ) ) || 
					 reg.IsPartOf( typeof( NecromancerRegion ) ) || 
					 reg.IsPartOf( typeof( DeadRegion ) ) || 
					 Server.Misc.Worlds.IsSeaTown( from.Location, from.Map ) || 
					 reg.IsPartOf( typeof( PirateRegion ) ) || 
					 reg.IsPartOf( typeof( OutDoorRegion ) ) || 
					 reg.IsPartOf( typeof( PublicRegion ) ) || 
					 Server.Misc.Worlds.IsMainRegion( Server.Misc.Worlds.GetRegionName( from.Map, from.Location ) ) ){ CanBuild = true; }

				if ( !DockSearch.NearDock(from) && !BaseBoat.isCarpet( boat ) )
				{
					from.SendMessage( phrase_a );
				}
				else if ( BaseBoat.IsValidLocation( p, map ) && CanBuild == true && boat.CanFit( p, map, boat.ItemID ) )
				{
					Delete();

					boat.Owner = from;
					boat.Anchored = true;
					boat.ShipName = m_ShipName;

					if ( from.Skills[SkillName.Seafaring].Base >= 90 && boat.m_BoatDoor != null ){ boat.m_BoatDoor.Visible = true; boat.BoatDoor.Hue = hue; }

					uint keyValue = boat.CreateKeys( from );

					if ( boat.PPlank != null )
						boat.PPlank.KeyValue = keyValue;

					if ( boat.SPlank != null )
						boat.SPlank.KeyValue = keyValue;

					boat.TillerMan.Hue = hue;
					boat.Hold.Hue = hue;
					boat.PPlank.Hue = hue;
					boat.SPlank.Hue = hue;

					boat.MoveToWorld( p, map );
					if ( BaseBoat.isCarpet( boat ) ){ from.PlaySound( 0x1FD ); } else { from.PlaySound( 0x026 ); }
				}
				else
				{
					boat.Delete();
					from.SendMessage( phrase_b );
				}
			}
		}

		private class InternalTarget : MultiTarget
		{
			private BaseDockedBoat m_Model;
			private int m_Hue;

			public InternalTarget( BaseDockedBoat model ) : base( model.MultiID, model.Offset )
			{
				m_Model = model;
				m_Hue = model.Hue;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D ip = o as IPoint3D;

				if ( ip != null )
				{
					if ( ip is Item )
						ip = ((Item)ip).GetWorldTop();

					Point3D p = new Point3D( ip );

					Region region = Region.Find( p, from.Map );

					if ( region.IsPartOf( typeof( DungeonRegion ) ) || Server.Engines.Instancing.InstanceManager.IsInstanceMap( from.Map ) )
						from.SendLocalizedMessage( 502488 ); // You can not place a ship inside a dungeon.
					else if ( region.IsPartOf( typeof( HouseRegion ) ) )
						from.SendLocalizedMessage( 1042549 ); // A boat may not be placed in this area.
					else
						m_Model.OnPlacement( from, p, m_Hue );
				}
			}
		}
	}
}