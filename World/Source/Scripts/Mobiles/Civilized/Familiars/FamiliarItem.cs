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
	public class HenchmanFamiliarItem : Item
	{
		public int FamiliarSerial;
		public Mobile FamiliarOwner;
		public int FamiliarType;
		public string FamiliarName;
		private int m_Charges;

		[CommandProperty(AccessLevel.Owner)]
		public int Familiar_Serial{ get { return FamiliarSerial; } set { FamiliarSerial = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public Mobile Familiar_Owner{ get { return FamiliarOwner; } set { FamiliarOwner = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Familiar_Type{ get { return FamiliarType; } set { FamiliarType = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Familiar_Name { get { return FamiliarName; } set { FamiliarName = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get{ return m_Charges; }
			set{ m_Charges = value; InvalidateProperties(); }
		}

		[Constructable]
		public HenchmanFamiliarItem() : base( 0x4FD6 )
		{
			if ( FamiliarType > 0 ){} else { FamiliarType = 202; }
			Name = "crystal ball of summoning";
			Light = LightType.Circle300;
			Weight = 1.0;
			FamiliarSerial = 0;
			Charges = 5;
		}

		public HenchmanFamiliarItem( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			ArrayList pets = new ArrayList();
			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is HenchmanFamiliar )
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
				from.SendMessage("You already have a familiar.");
			}
			else if ( nFollowers > 0 )
			{
				from.SendMessage("You already have too many in your group.");
			}
			else if ( from.Skills[SkillName.Elementalism].Base < 50 && from.Skills[SkillName.Magery].Base < 50 && from.Skills[SkillName.Necromancy].Base < 50 )
			{
				from.SendMessage("Only apprentice mages, elementalists, or necromancers may summon these familiars.");
			}
			else if ( Charges < 1 )
			{
				from.SendMessage("This crystal ball seems to be out of charges.");
			}
			else if ( FamiliarOwner == null || FamiliarOwner.Serial != from.Serial )
			{
				from.SendMessage("This is not your crystal ball!");
			}
			else
			{
				Map map = from.Map;
				ConsumeCharge( from );

				BaseCreature friend = new HenchmanFamiliar();

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
				friend.Body = this.FamiliarType;
					if ( friend.Body == 0x15 ){ friend.BaseSoundID = 0xDB; }
					else if ( friend.Body == 0xD9 ){ friend.BaseSoundID = 0x85; }
					else if ( friend.Body == 238 ){ friend.BaseSoundID = 0xCC; }
					else if ( friend.Body == 0xC9 ){ friend.BaseSoundID = 0x69; }
					else if ( friend.Body == 0xD7 ){ friend.BaseSoundID = 0x188; }
					else if ( friend.Body == 80 ){ friend.BaseSoundID = 0x26B; }
					else if ( friend.Body == 81 ){ friend.BaseSoundID = 0x266; }
					else if ( friend.Body == 340 ){ friend.BaseSoundID = 0x73; }
					else if ( friend.Body == 277 ){ friend.BaseSoundID = 0xE5; }
					else if ( friend.Body == 0xCE ){ friend.BaseSoundID = 0x5A; }
					else if ( friend.Body == 590 ){ friend.BaseSoundID = 362; }
					else if ( friend.Body == 315 ){ friend.BaseSoundID = 397; }
					else if ( friend.Body == 120 ){ friend.BaseSoundID = 397; }
					else if ( friend.Body == 202 ){ friend.BaseSoundID = 422; }
					else if ( friend.Body == 140 ){ friend.BaseSoundID = 0x388; }
					else if ( friend.Body == 173 ){ friend.BaseSoundID = 0x388; }
					else if ( friend.Body == 317 ){ friend.BaseSoundID = 0x270; }
					else if ( friend.Body == 242 ){ friend.BaseSoundID = 397; }
					else if ( friend.Body == 0x3C ){ friend.BaseSoundID = 362; }
					else if ( friend.Body == 0x4 ){ friend.BaseSoundID = 357; }
					else if ( friend.Body == 0x9 ){ friend.BaseSoundID = 357; }
					else if ( friend.Body == 0x16 ){ friend.BaseSoundID = 377; }

				friend.SummonMaster = from;
				friend.Hue = this.Hue;
				if ( FamiliarName != null ){ friend.Name = FamiliarName; } else { friend.Name = "a familiar"; }

				friend.MoveToWorld( loc, map );
				this.LootType = LootType.Blessed;
				this.Visible = false;
				this.FamiliarSerial = friend.Serial;

				friend.FixedParticles( 0x374A, 1, 15, 5054, 23, 7, EffectLayer.Head );
				friend.PlaySound( 0x1F9 );
				from.FixedParticles( 0x0000, 10, 5, 2054, EffectLayer.Head );
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
			list.Add( 1060741, m_Charges.ToString() );
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			string sType = "a familiar";
			if ( FamiliarType == 0x15 ){ sType = "a serpent familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the serpent familiar"; } }
			else if ( FamiliarType == 0xD9 ){ sType = "a dog familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the dog familiar"; } }
			else if ( FamiliarType == 238 ){ sType = "a rat familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the rat familiar"; } }
			else if ( FamiliarType == 0xC9 ){ sType = "a cat familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the cat familiar"; } }
			else if ( FamiliarType == 0xD7 ){ sType = "a huge rat familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the huge rat familiar"; } }
			else if ( FamiliarType == 80 ){ sType = "a large toad familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the large toad familiar"; } }
			else if ( FamiliarType == 81 ){ sType = "a huge frog familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the huge frog familiar"; } }
			else if ( FamiliarType == 340 ){ sType = "a large cat familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the large cat familiar"; } }
			else if ( FamiliarType == 277 ){ sType = "a wolf familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the wolf familiar"; } }
			else if ( FamiliarType == 0xCE ){ sType = "a large lizard familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the large lizard familiar"; } }
			else if ( FamiliarType == 590 ){ sType = "a small dragon familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the small dragon familiar"; } }
			else if ( FamiliarType == 315 ){ sType = "a large scorpion familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the large scorpion familiar"; } }
			else if ( FamiliarType == 120 ){ sType = "a huge beetle familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the huge beetle familiar"; } }
			else if ( FamiliarType == 202 ){ sType = "an imp familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the imp familiar"; } }
			else if ( FamiliarType == 140 ){ sType = "a spider familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the spider familiar"; } }
			else if ( FamiliarType == 173 ){ sType = "a giant spider familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the giant spider familiar"; } }
			else if ( FamiliarType == 317 ){ sType = "a bat familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the bat familiar"; } }
			else if ( FamiliarType == 242 ){ sType = "a giant insect familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the giant insect familiar"; } }
			else if ( FamiliarType == 0x3C ){ sType = "a dragon familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the dragon familiar"; } }
			else if ( FamiliarType == 0x4 ){ sType = "a demon familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the demon familiar"; } }
			else if ( FamiliarType == 0x9 ){ sType = "a daemon familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the daemon familiar"; } }
			else if ( FamiliarType == 0x16 ){ sType = "a gazer familiar"; if ( FamiliarName != "a familiar" ){ sType = FamiliarName + " the gazer familiar"; } }

			string sInfo = sType;
			list.Add( 1070722, sInfo );

			string sOwner = FamiliarOwner != null ? FamiliarOwner.Name : "nobody";
			list.Add( 1049644, "Belongs To " + sOwner + ""); // PARENTHESIS
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
            writer.Write( FamiliarSerial );
            writer.Write( FamiliarOwner );
            writer.Write( FamiliarType );
            writer.Write( FamiliarName );
			writer.Write( (int) m_Charges );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			FamiliarSerial = reader.ReadInt();
			FamiliarOwner = reader.ReadMobile();
			FamiliarType = reader.ReadInt();
			FamiliarName = reader.ReadString();
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
			if ( ItemID == 0xE2E ){ ItemID = 0x4FD6; }
		}
	}
}