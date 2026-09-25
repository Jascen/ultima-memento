using Server.Mobiles;
using Server.Utilities;
using System.Linq;

namespace Server.Items
{
	public class GolemPorterItem : Item
	{
		public int PorterSerial;
		public Mobile PorterOwner;
		public int PorterHue;
		public int PorterExodus;
		public int PorterType;
		public string PorterName;
		public int m_Charges;

		[CommandProperty(AccessLevel.Owner)]
		public int Porter_Serial{ get { return PorterSerial; } set { PorterSerial = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public Mobile Porter_Owner{ get { return PorterOwner; } set { PorterOwner = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Porter_Hue{ get { return PorterHue; } set { PorterHue = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Porter_Exodus{ get { return PorterExodus; } set { PorterExodus = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public bool IsPorter{ get { return PorterType == 0; } set { PorterType = value ? 0 : 1; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Porter_Name { get { return PorterName; } set { PorterName = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get{ return m_Charges; }
			set{ m_Charges = value; InvalidateProperties(); }
		}

		[Constructable]
		public GolemPorterItem() : base( 0x3566 )
		{
			PorterHue = 0x430;
			Name = "a golem";
			Weight = 1.0;
			PorterSerial = 0;
			Charges = 5;
			ItemID = 0x3566;
		}

		public GolemPorterItem( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001);
				return;
			}
			
			var hasGolem = WorldUtilities
				.ForEachMobile<BaseCreature>(mobile => mobile is GolemPorter || mobile is GolemFighter)
				.Any(bc => bc.Controlled && bc.ControlMaster == from);
			if ( hasGolem )
			{
				from.SendMessage("You already have a golem.");
			}
			else if ( from.FollowersMax - from.Followers < (IsPorter ? GolemPorter.MaxControlSlots : GolemFighter.MaxControlSlots) )
			{
				from.SendMessage("You already have too many in your group.");
			}
			else if ( Charges == 0 )
			{
				from.SendMessage("Your golem needs another power crystal.");
			}
			else if ( PorterOwner == null || PorterOwner.Serial != from.Serial )
			{
				from.SendMessage("This is not your golem!");
			}
			else
			{
				Map map = from.Map;
				ConsumeCharge( from );
				this.InvalidateProperties();

				BaseCreature friend;
				if ( IsPorter ) friend = new GolemPorter { PorterExodus = PorterExodus };
				else friend = new GolemFighter { PorterExodus = PorterExodus };

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
				friend.Loyalty = 100;
				friend.Summoned = true;
				friend.Hue = this.PorterHue;
				friend.Resource = this.Resource;
				friend.SummonMaster = from;

				if ( PorterName != null ){ friend.Name = PorterName; } else { friend.Name = "a golem"; }

				from.PlaySound( 0x665 );
				friend.MoveToWorld( loc, map );
				friend.OnAfterSpawn();
				this.LootType = LootType.Blessed;
				this.Visible = false;
				this.PorterSerial = friend.Serial;
			}
		}

		public void ConsumeCharge( Mobile from )
		{
			--Charges;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1060584, "{0}\t{1}", m_Charges.ToString(), "Uses" );
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			string sType = "a golem";
			if ( PorterName != "a golem" ){ sType = PorterName + " the golem"; }

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
            writer.Write( PorterHue );
            writer.Write( PorterType );
            writer.Write( PorterExodus );
            writer.Write( PorterName );
			writer.Write( (int) m_Charges );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			PorterSerial = reader.ReadInt();
			PorterOwner = reader.ReadMobile();
			PorterHue = reader.ReadInt();
			PorterType = reader.ReadInt();
			PorterExodus = reader.ReadInt();
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
			ItemID = 0x3566;
		}
	}
}