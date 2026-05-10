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
	public class PackBeastItem : Item
	{
		public int PorterSerial;
		public Mobile PorterOwner;
		public int PorterType;
		public string PorterName;
		private int m_Charges;

		[CommandProperty(AccessLevel.Owner)]
		public int Porter_Serial{ get { return PorterSerial; } set { PorterSerial = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public Mobile Porter_Owner{ get { return PorterOwner; } set { PorterOwner = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Porter_Type{ get { return PorterType; } set { PorterType = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Porter_Name { get { return PorterName; } set { PorterName = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get{ return m_Charges; }
			set{ m_Charges = value; InvalidateProperties(); }
		}

		[Constructable]
		public PackBeastItem() : base( 0x2126 )
		{
			if ( PorterType > 0 ){} else { PorterType = 291; }
			Name = "pack animal";
			Weight = 1.0;
			PorterSerial = 0;
			Charges = 5;

			if ( PorterType == 292 ){ ItemID = 0x2127; }
			else if ( PorterType == 23 ){ ItemID = 0x20DB; }
			else if ( PorterType == 177 ){ ItemID = 0x20CF; }
			else if ( PorterType == 179 ){ ItemID = 0x20E1; }
			else if ( PorterType == 291 ){ ItemID = 0x2126; }
		}

		public PackBeastItem( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			int animalfriend = 0;

			foreach ( Mobile m in this.GetMobilesInRange( 20 ) )
			{
				if ( 	m is DruidGuildmaster || 
						m is DruidTree || 
						m is Druid || 
						m is AnimalTrainer || 
						m is Rancher || 
						m is Veterinarian )
					++animalfriend;
			}

			if ( animalfriend == 0 )
			{
				from.SendMessage( "You need to be near a druid or animal handler to call this pack animal!" );
			}
			else
			{
				ArrayList pets = new ArrayList();
				foreach ( Mobile m in World.Mobiles.Values )
				{
					if ( m is PackBeast )
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
					from.SendMessage("You already have a pack animal.");
				}
				else if ( nFollowers > 0 )
				{
					from.SendMessage("You already have too many in your group.");
				}
				else if ( Charges < 1 )
				{
					from.SendMessage("Your pack animal needs to be tended to by a druid guildmaster.");
				}
				else if ( PorterOwner == null || PorterOwner.Serial != from.Serial )
				{
					from.SendMessage("This is not your pack animal!");
				}
				else
				{
					Map map = from.Map;
					ConsumeCharge( from );
					this.InvalidateProperties();

					BaseCreature friend = new PackBeast();

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
					friend.Body = this.PorterType;
						if ( friend.Body == 213 ){ friend.BaseSoundID = 0xA3; }
						else if ( friend.Body == 292 ){ friend.BaseSoundID = 0x3F3; }
						else if ( friend.Body == 291 ){ friend.BaseSoundID = 0xA8; }

					friend.SummonMaster = from;
					friend.Hue = this.Hue;
					if ( PorterName != null ){ friend.Name = PorterName; } else { friend.Name = "a pack animal"; }

					friend.MoveToWorld( loc, map );
					this.LootType = LootType.Blessed;
					this.Visible = false;
					this.PorterSerial = friend.Serial;
				}
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
			string sType = "a pack animal";

			if ( PorterType == 292 ){ sType = "a pack llama"; if ( PorterName != "a pack animal" ){ sType = PorterName + " the pack llama"; } }
			else if ( PorterType == 23 ){ sType = "a pack bear"; if ( PorterName != "a pack animal" ){ sType = PorterName + " the pack bear"; } }
			else if ( PorterType == 177 ){ sType = "a pack bear"; if ( PorterName != "a pack animal" ){ sType = PorterName + " the pack bear"; } }
			else if ( PorterType == 179 ){ sType = "a pack bear"; if ( PorterName != "a pack animal" ){ sType = PorterName + " the pack bear"; } }
			else if ( PorterType == 291 ){ sType = "a pack horse"; if ( PorterName != "a pack animal" ){ sType = PorterName + " the pack horse"; } }

			string sInfo = sType;
			list.Add( 1070722, sInfo );

			string sOwner = PorterOwner != null ? PorterOwner.Name : "nobody";
			list.Add( 1049644, "Belongs To " + sOwner + ""); // PARENTHESIS
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
            writer.Write( PorterSerial );
            writer.Write( PorterOwner );
            writer.Write( PorterType );
            writer.Write( PorterName );
			writer.Write( (int) m_Charges );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			PorterSerial = reader.ReadInt();
			PorterOwner = reader.ReadMobile();
			PorterType = reader.ReadInt();
			PorterName = reader.ReadString();
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

			if ( PorterType == 34 ){ PorterType = 23; }
		}
	}
}