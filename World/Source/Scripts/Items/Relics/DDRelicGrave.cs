using System;
using Server;
using Server.Misc;
using Server.Mobiles;

namespace Server.Items
{
	public class DDRelicGrave : Item, IRelic
	{
		public override void ItemIdentified( bool id )
		{
			m_NotIdentified = id;
			if ( !id )
			{
				ColorHue3 = "FDC844";
				ColorText3 = "Worth " + CoinPrice + " Gold";
			}
		}

		public int RelicFlipID1;
		public int RelicFlipID2;
		public string RelicDescription;

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_FlipID1 { get { return RelicFlipID1; } set { RelicFlipID1 = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_FlipID2 { get { return RelicFlipID2; } set { RelicFlipID2 = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Relic_Describe { get { return RelicDescription; } set { RelicDescription = value; InvalidateProperties(); } }

		[Constructable]
		public DDRelicGrave() : base( 0xED4 )
		{
			Weight = 30;
			CoinPrice = Utility.RandomMinMax( 80, 500 );
			NotIdentified = true;
			NotIDSource = Identity.Merchant;
			NotIDSkill = IDSkill.Mercantile;
			Name = "gravestone";

			string sBody = "a corpse";
			switch ( Utility.RandomMinMax( 0, 2 ) )
			{
				case 0:	sBody = "corpse";	break;
				case 1:	sBody = "body";		break;
				case 2:	sBody = "skeleton";	break;
			}

			string sChain = "a chained";
			switch ( Utility.RandomMinMax( 0, 3 ) )
			{
				case 0:	sChain = "a chained";		break;
				case 1:	sChain = "a shackled";		break;
				case 2:	sChain = "a bound";			break;
				case 3:	sChain = "a manacled";		break;
			}

			ItemID = 0xED4; RelicFlipID1 = 0xED5; RelicFlipID2 = 0xED4; 
			switch ( Utility.RandomMinMax( 0, 30 ) ) 
			{
				case 0: ItemID = 0xED4; RelicFlipID1 = 0xED5; RelicFlipID2 = 0xED4; break;
				case 1: ItemID = 0xED7; RelicFlipID1 = 0xED8; RelicFlipID2 = 0xED7; break;
				case 2: ItemID = 0xEDB; RelicFlipID1 = 0xEDC; RelicFlipID2 = 0xEDB; break;
				case 3: ItemID = 0xEDD; RelicFlipID1 = 0xEDF; RelicFlipID2 = 0xEDD; break;
				case 4: ItemID = 0x1165; RelicFlipID1 = 0x1166; RelicFlipID2 = 0x1165; break;
				case 5: ItemID = 0x1167; RelicFlipID1 = 0x1168; RelicFlipID2 = 0x1167; break;
				case 6: ItemID = 0x1169; RelicFlipID1 = 0x116A; RelicFlipID2 = 0x1169; break;
				case 7: ItemID = 0x116B; RelicFlipID1 = 0x116C; RelicFlipID2 = 0x116B; break;
				case 8: ItemID = 0x116D; RelicFlipID1 = 0x116E; RelicFlipID2 = 0x116D; break;
				case 9: ItemID = 0x116F; RelicFlipID1 = 0x1170; RelicFlipID2 = 0x116F; break;
				case 10: ItemID = 0x1171; RelicFlipID1 = 0x1172; RelicFlipID2 = 0x1171; break;
				case 11: ItemID = 0x1173; RelicFlipID1 = 0x1174; RelicFlipID2 = 0x1173; break;
				case 12: ItemID = 0x1175; RelicFlipID1 = 0x1176; RelicFlipID2 = 0x1175; break;
				case 13: ItemID = 0x1177; RelicFlipID1 = 0x1178; RelicFlipID2 = 0x1177; break;
				case 14: ItemID = 0x1179; RelicFlipID1 = 0x117A; RelicFlipID2 = 0x1179; break;
				case 15: ItemID = 0x117B; RelicFlipID1 = 0x117C; RelicFlipID2 = 0x117B; break;
				case 16: ItemID = 0x117D; RelicFlipID1 = 0x117E; RelicFlipID2 = 0x117D; break;
				case 17: ItemID = 0x117F; RelicFlipID1 = 0x1180; RelicFlipID2 = 0x117F; break;
				case 18: ItemID = 0x1181; RelicFlipID1 = 0x1182; RelicFlipID2 = 0x1181; break;
				case 19: ItemID = 0x1183; RelicFlipID1 = 0x1184; RelicFlipID2 = 0x1183; break;

				case 20: ItemID = 0x124B; RelicFlipID1 = 0x1249; RelicFlipID2 = 0x124B; Name = "an iron maiden"; RelicDescription = "That Once Held " + ContainerFunctions.GetOwner( "property" ); break;

				case 21: ItemID = 0x1C20; RelicFlipID1 = 0x1C21; RelicFlipID2 = 0x1C20; Name = "a wrapped body"; RelicDescription = ContainerFunctions.GetOwner( "Body" ); break;

				case 22: ItemID = 0x1D9E; RelicFlipID1 = 0x1D9D; RelicFlipID2 = 0x1D9E; Name = "a bloody spike"; RelicDescription = "That Killed " + ContainerFunctions.GetOwner( "property" ); break;

				case 23: ItemID = 0x1A01; RelicFlipID1 = 0x1A02; RelicFlipID2 = 0x1A01; Name = sChain + " " + sBody; RelicDescription = ContainerFunctions.GetOwner( "Body" ); break;
				case 24: ItemID = 0x1A03; RelicFlipID1 = 0x1A04; RelicFlipID2 = 0x1A03; Name = sChain + " " + sBody; RelicDescription = ContainerFunctions.GetOwner( "Body" ); break;
				case 25: ItemID = 0x1A05; RelicFlipID1 = 0x1A06; RelicFlipID2 = 0x1A05; Name = sChain + " " + sBody; RelicDescription = ContainerFunctions.GetOwner( "Body" ); break;
				case 26: ItemID = 0x1A09; RelicFlipID1 = 0x1A0A; RelicFlipID2 = 0x1A09; Name = sChain + " " + sBody; RelicDescription = ContainerFunctions.GetOwner( "Body" ); break;
				case 27: ItemID = 0x1A0B; RelicFlipID1 = 0x1A0C; RelicFlipID2 = 0x1A0B; Name = sChain + " " + sBody; RelicDescription = ContainerFunctions.GetOwner( "Body" ); break;
				case 28: ItemID = 0x1A0D; RelicFlipID1 = 0x1A0E; RelicFlipID2 = 0x1A0D; Name = sChain + " " + sBody; RelicDescription = ContainerFunctions.GetOwner( "Body" ); break;
				case 29: ItemID = 0x1B7C; RelicFlipID1 = 0x1B7F; RelicFlipID2 = 0x1B7C; Name = sChain + " " + sBody; RelicDescription = ContainerFunctions.GetOwner( "Body" ); break;
				case 30: ItemID = 0x1B1D; RelicFlipID1 = 0x1B1E; RelicFlipID2 = 0x1B1D; Name = sChain + " " + sBody; RelicDescription = ContainerFunctions.GetOwner( "Body" ); break;
			}

			if ( Name == "gravestone" )
			{
				string sGrave = "gravestone";
				switch ( Utility.RandomMinMax( 0, 1 ) )
				{
					case 0:	sGrave = "gravestone";	break;
					case 1:	sGrave = "tombstone";		break;
				}

				string sCarving = "Here Lies";
				switch ( Utility.RandomMinMax( 0, 4 ) )
				{
					case 0:	sCarving = "Here Lies";			break;
					case 1:	sCarving = "Rest in Peace";		break;
					case 2:	sCarving = "We Will Remember";	break;
					case 3:	sCarving = "Here Rests";		break;
					case 4:	sCarving = "Buried Here is";	break;
				}

				RelicDescription = sCarving + " " + ContainerFunctions.GetOwner( "property" );

				string sMade = "a " + sGrave;
				switch ( Utility.RandomMinMax( 0, 50 ) )
				{
					case 0:		sMade = "a bronze " + sGrave;		Hue = 0xB9A;	CoinPrice = CoinPrice * 2; break;
					case 1:		sMade = "a jade " + sGrave;			Hue = 0xB93;	CoinPrice = CoinPrice * 2; break;
					case 2:		sMade = "a granite " + sGrave;		Hue = 0xB8E;	CoinPrice = CoinPrice * 2; break;
					case 3:		sMade = "a marble " + sGrave;		Hue = 0xB8B;	CoinPrice = CoinPrice * 2; break;
					case 4:		sMade = "a copper " + sGrave;		Hue = 0x972;	CoinPrice = CoinPrice * 2; break;
					case 5:		sMade = "a silver " + sGrave;		Hue = 0x835;	CoinPrice = CoinPrice * 2; break;
					case 7:		sMade = "an amethyst " + sGrave;	Hue = 0x492;	CoinPrice = CoinPrice * 2; break;
					case 8:		sMade = "an emerald " + sGrave;		Hue = 0x5B4;	CoinPrice = CoinPrice * 2; break;
					case 10:	sMade = "a garnet " + sGrave;		Hue = 0x48F;	CoinPrice = CoinPrice * 2; break;
					case 11:	sMade = "an onyx " + sGrave;		Hue = 0x497;	CoinPrice = CoinPrice * 2; break;
					case 12:	sMade = "a quartz " + sGrave;		Hue = 0x4AC;	CoinPrice = CoinPrice * 2; break;
					case 13:	sMade = "a ruby " + sGrave;			Hue = 0x5B5;	CoinPrice = CoinPrice * 2; break;
					case 14:	sMade = "a sapphire " + sGrave;		Hue = 0x5B6;	CoinPrice = CoinPrice * 2; break;
					case 15:	sMade = "a spinel " + sGrave;		Hue = 0x48B;	CoinPrice = CoinPrice * 2; break;
					case 16:	sMade = "a star ruby " + sGrave;	Hue = 0x48E;	CoinPrice = CoinPrice * 2; break;
					case 17:	sMade = "a topaz " + sGrave;		Hue = 0x488;	CoinPrice = CoinPrice * 2; break;
					case 18:	sMade = "an ivory " + sGrave;		Hue = 0x47E;	CoinPrice = CoinPrice * 2; break;
					case 19:	sMade = "a solid gold " + sGrave;	Hue = 0x4AC;	CoinPrice = CoinPrice * 4; Weight = Weight * 2; break;
				}
				Name = sMade;
			}
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1049644, RelicDescription);
        }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) && MySettings.S_IdentifyItemsOnlyInPack && from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified ) 
				from.SendMessage( "This must be in your backpack to identify." );
			else if ( from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified )
				IDCommand( from );
			else if ( !IsChildOf( from.Backpack ) )
				from.SendMessage( "This must be in your backpack to flip." );
			else
				if ( this.ItemID == RelicFlipID1 ){ this.ItemID = RelicFlipID2; } else { this.ItemID = RelicFlipID1; }
		}

		public override void IDCommand( Mobile m )
		{
			if ( this.NotIDSkill == IDSkill.Tasting )
				RelicFunctions.IDItem( m, m, this, SkillName.Tasting );
			else if ( this.NotIDSkill == IDSkill.ArmsLore )
				RelicFunctions.IDItem( m, m, this, SkillName.ArmsLore );
			else
				RelicFunctions.IDItem( m, m, this, SkillName.Mercantile );
		}

		public DDRelicGrave(Serial serial) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
            writer.Write( (int) 1 ); // version
            writer.Write( RelicFlipID1 );
            writer.Write( RelicFlipID2 );
            writer.Write( RelicDescription );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
            int version = reader.ReadInt();

			if ( version < 1 )
				CoinPrice = reader.ReadInt();

            RelicFlipID1 = reader.ReadInt();
            RelicFlipID2 = reader.ReadInt();
            RelicDescription = reader.ReadString();
		}
	}
}