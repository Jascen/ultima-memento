using System;
using Server;
using Server.Network;
using System.Text;
using Server.Items;
using Server.Gumps;
using Server.Regions;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Accounting;

namespace Server.Items
{
	public class RobotItem : Item
	{
		public int RobotSerial;
		public Mobile RobotOwner;
		public string RobotName;
		public int m_Charges;

		[CommandProperty(AccessLevel.Owner)]
		public int Robot_Serial{ get { return RobotSerial; } set { RobotSerial = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public Mobile Robot_Owner{ get { return RobotOwner; } set { RobotOwner = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Robot_Name { get { return RobotName; } set { RobotName = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get{ return m_Charges; }
			set{ m_Charges = value; InvalidateProperties(); }
		}

		[Constructable]
		public RobotItem() : base( 0x357F )
		{
			Name = "a robot";
			Weight = 1.0;
			RobotSerial = 0;
			Charges = 5;
		}

		public RobotItem( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			ArrayList pets = new ArrayList();

			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is Robot )
				{
					BaseCreature bc = (BaseCreature)m;
					if ( bc.Controlled && bc.ControlMaster == from )
						pets.Add( bc );
				}
			}

			int nFollowers = from.Followers;

			if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001);
			}
			else if ( pets.Count > 0 )
			{
				from.SendMessage("You already have a robot.");
			}
			else if ( nFollowers > 0 )
			{
				from.SendMessage("You already have too many in your group.");
			}
			else if ( Charges == 0 )
			{
				from.SendMessage("Your robot needs another battery.");
			}
			else if ( RobotOwner == null || RobotOwner.Serial != from.Serial )
			{
				from.SendMessage("This is not your robot!");
			}
			else
			{
				Map map = from.Map;
				ConsumeCharge( from );
				this.InvalidateProperties();

				BaseCreature friend = new Robot();

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

				friend.ControlMaster = from;
				friend.Controlled = true;
				friend.ControlOrder = OrderType.Come;
				friend.ControlSlots = 5;
				friend.Loyalty = 100;
				friend.Summoned = true;
				friend.Hue = this.Hue;
				friend.SummonMaster = from;

				if ( RobotName != null ){ friend.Name = RobotName; } else { friend.Name = "a robot"; }

				from.PlaySound( 0x665 );
				friend.MoveToWorld( loc, map );
				friend.OnAfterSpawn();
				this.LootType = LootType.Blessed;
				this.Visible = false;
				this.RobotSerial = friend.Serial;
			}
		}

		public void ConsumeCharge( Mobile from )
		{
			--Charges;
		}

		private static string GetOwner( int serial )
		{
			string sOwner = null;

			foreach ( Mobile owner in World.Mobiles.Values )
			if ( owner.Serial == serial )
			{
				sOwner = owner.Name;
			}

			return sOwner;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1060584, "{0}\t{1}", m_Charges.ToString(), "Uses" );
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			string sType = "a robot";
			if ( RobotName != "a robot" ){ sType = RobotName + " the robot"; }

			string sInfo = sType;
			list.Add( 1070722, sInfo );

			string sOwner = RobotOwner != null ? RobotOwner.Name : "nobody";
			list.Add( 1049644, "Belongs To " + sOwner + ""); // PARENTHESIS
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
            writer.Write( RobotSerial );
            writer.Write( RobotOwner );
            writer.Write( RobotName );
			writer.Write( (int) m_Charges );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			RobotSerial = reader.ReadInt();
			RobotOwner = reader.ReadMobile();
			RobotName = reader.ReadString();
			switch ( version )
			{
				case 0:
				{
					m_Charges = (int)reader.ReadInt();

					break;
				}
			}
			LootType = LootType.Regular;
			Visible = true;
			ItemID = 0x357F;
		}
	}
}