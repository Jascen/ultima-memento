using System;
using Server;
using Server.Misc;
using Server.Mobiles;

namespace Server.Items
{
	public class DDRelicStatue : Item, IRelic
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
		public DDRelicStatue() : base( 0x1224 )
		{
			Weight = 60;
			CoinPrice = Utility.RandomMinMax( 80, 500 );
			NotIdentified = true;
			NotIDSource = Identity.Merchant;
			NotIDSkill = IDSkill.Mercantile;

			string sMade = "Made of colored stone";
			switch ( Utility.RandomMinMax( 0, 7 ) ) 
			{
				case 0: Hue = Utility.RandomColor(0); break;
				case 1: Hue = Utility.RandomColor(0); break;
				case 2: Hue = Utility.RandomColor(0); break;
				case 3: Hue = Utility.RandomColor(0); break;
				case 4: Hue = Utility.RandomColor(0); break;
				case 5: Hue = Utility.RandomColor(0); break;
				case 6: Hue = 0; sMade = "Made of stone"; break;
				case 7: Hue = 0; sMade = "Made of stone"; break;
			}

			string sLook = "a rare";
			switch ( Utility.RandomMinMax( 0, 18 ) )
			{
				case 0:	sLook = "a rare";	break;
				case 1:	sLook = "a nice";	break;
				case 2:	sLook = "a pretty";	break;
				case 3:	sLook = "a superb";	break;
				case 4:	sLook = "a delightful";	break;
				case 5:	sLook = "an elegant";	break;
				case 6:	sLook = "an exquisite";	break;
				case 7:	sLook = "a fine";	break;
				case 8:	sLook = "a gorgeous";	break;
				case 9:	sLook = "a lovely";	break;
				case 10:sLook = "a magnificent";	break;
				case 11:sLook = "a marvelous";	break;
				case 12:sLook = "a splendid";	break;
				case 13:sLook = "a wonderful";	break;
				case 14:sLook = "an extraordinary";	break;
				case 15:sLook = "a strange";	break;
				case 16:sLook = "an odd";	break;
				case 17:sLook = "a unique";	break;
				case 18:sLook = "an unusual";	break;
			}
			
			string sDecon = "decorative";
			switch ( Utility.RandomMinMax( 0, 2 ) )
			{
				case 0:	sDecon = "decorative";		break;
				case 1:	sDecon = "ceremonial";		break;
				case 2:	sDecon = "ornamental";		break;
			}

			switch ( Utility.RandomMinMax( 0, 50 ) )
			{
				case 0:		sMade = "Made of bronze";		Hue = 0xB9A;	break;
				case 1:		sMade = "Made of jade";			Hue = 0xB93;	break;
				case 2:		sMade = "Made of granite";		Hue = 0xB8E;	break;
				case 3:		sMade = "Made of marble";		Hue = 0xB8B;	break;
				case 4:		sMade = "Made of copper";		Hue = 0x972;	break;
				case 5:		sMade = "Made of ice";			Hue = 0x480;	break;
				case 6:		sMade = "Made of silver";		Hue = 0x835;	break;
				case 7:		sMade = "Made of amethyst";		Hue = 0x492;	break;
				case 8:		sMade = "Made of emerald";		Hue = 0x5B4;	break;
				case 10:	sMade = "Made of garnet";		Hue = 0x48F;	break;
				case 11:	sMade = "Made of onyx";			Hue = 0x497;	break;
				case 12:	sMade = "Made of quartz";		Hue = 0x4AC;	break;
				case 13:	sMade = "Made of ruby";			Hue = 0x5B5;	break;
				case 14:	sMade = "Made of sapphire";		Hue = 0x5B6;	break;
				case 15:	sMade = "Made of spinel";		Hue = 0x48B;	break;
				case 16:	sMade = "Made of star ruby";	Hue = 0x48E;	break;
				case 17:	sMade = "Made of topaz";		Hue = 0x488;	break;
				case 18:	sMade = "Made of ivory";		Hue = 0x47E;	break;
				case 19:	sMade = "Made of solid gold";	Hue = 0x4AC;	CoinPrice = CoinPrice * 2; Weight = Weight * 2; break;
			}
			RelicDescription = sMade;

			switch ( Utility.RandomMinMax( 0, 37 ) ) 
			{
				case 0: ItemID = 0x1224; RelicFlipID1 = 0x1224; RelicFlipID2 = 0x139A; Name = sLook + ", " + sDecon + " statue of a woman"; break;
				case 1: ItemID = 0x1225; RelicFlipID1 = 0x1225; RelicFlipID2 = 0x1225; Name = sLook + ", " + sDecon + " statue of a man"; break;
				case 2: ItemID = 0x1226; RelicFlipID1 = 0x1226; RelicFlipID2 = 0x139B; Name = sLook + ", " + sDecon + " statue of an angel"; break;
				case 3: ItemID = 0x1226; RelicFlipID1 = 0x1226; RelicFlipID2 = 0x139B; Name = sLook + ", " + sDecon + " statue of a demon"; break;
				case 4: ItemID = 0x1227; RelicFlipID1 = 0x1227; RelicFlipID2 = 0x139C; Name = sLook + ", " + sDecon + " statue of a man"; break;
				case 5: ItemID = 0x1228; RelicFlipID1 = 0x1228; RelicFlipID2 = 0x139D; Name = sLook + ", " + sDecon + " statue of a bird"; break;
				case 6: ItemID = 0x1228; RelicFlipID1 = 0x1228; RelicFlipID2 = 0x139D; Name = sLook + ", " + sDecon + " statue of a pegasus"; break;
				case 7: ItemID = 0x12CA; RelicFlipID1 = 0x12CA; RelicFlipID2 = 0x12CB; Name = sLook + ", " + sDecon + " bust of a man"; break;
				case 8: ItemID = 0x207C; RelicFlipID1 = 0x207C; RelicFlipID2 = 0x207C; Name = sLook + ", " + sDecon + " statue of an angel"; break;
				case 9: ItemID = 0x42BB; RelicFlipID1 = 0x42BB; RelicFlipID2 = 0x42BB; Name = sLook + ", " + sDecon + " statue of a gargoyle"; Weight = 100; CoinPrice = Utility.RandomMinMax( 100, 400 ); break;
				case 10: ItemID = 0x42BB; RelicFlipID1 = 0x42BB; RelicFlipID2 = 0x42BB; Name = sLook + ", " + sDecon + " statue of a demon"; Weight = 100; CoinPrice = Utility.RandomMinMax( 100, 400 ); break;
				case 11: ItemID = 0x42C2; RelicFlipID1 = 0x42C2; RelicFlipID2 = 0x42C2; Name = sLook + ", " + sDecon + " statue of an odd creature"; Weight = 150; CoinPrice = Utility.RandomMinMax( 150, 500 ); break;
				case 12: ItemID = 0x40BC; RelicFlipID1 = 0x40BC; RelicFlipID2 = 0x40BC; Name = sLook + ", " + sDecon + " statue of a medusa"; Weight = 150; CoinPrice = Utility.RandomMinMax( 150, 500 ); break;
				case 13: ItemID = 0x42C5; RelicFlipID1 = 0x42C5; RelicFlipID2 = 0x42C5; Name = sLook + ", " + sDecon + " statue of a demon"; Weight = 150; CoinPrice = Utility.RandomMinMax( 150, 500 ); break;
				case 14: ItemID = 0x42BC; RelicFlipID1 = 0x42BC; RelicFlipID2 = 0x42BC; Name = sLook + ", " + sDecon + " bust of a demon"; break;
				case 15: ItemID = 0x48A8; RelicFlipID1 = 0x48A8; RelicFlipID2 = 0x48A9; Name = sLook + ", " + sDecon + " statue of dragon head"; Weight = 150; CoinPrice = Utility.RandomMinMax( 150, 500 ); break;
				case 16: ItemID = 0x4578; RelicFlipID1 = 0x4578; RelicFlipID2 = 0x4579; Name = sLook + ", " + sDecon + " statue of a sea horse"; Weight = 150; CoinPrice = Utility.RandomMinMax( 150, 500 ); break;
				case 17: ItemID = 0x457A; RelicFlipID1 = 0x457A; RelicFlipID2 = 0x457B; Name = sLook + ", " + sDecon + " statue of a mermaid"; Weight = 150; CoinPrice = Utility.RandomMinMax( 150, 500 ); break;
				case 18: ItemID = 0x457C; RelicFlipID1 = 0x457C; RelicFlipID2 = 0x457D; Name = sLook + ", " + sDecon + " statue of a gryphon"; Weight = 150; CoinPrice = Utility.RandomMinMax( 150, 500 ); break;
				case 19: ItemID = 0x42C0; RelicFlipID1 = 0x42C0; RelicFlipID2 = 0x42C1; Name = sLook + ", " + sDecon + " statue of a demon"; Weight = 100; CoinPrice = Utility.RandomMinMax( 100, 400 ); break;
				case 20: ItemID = 0x3F19; RelicFlipID1 = 0x3F19; RelicFlipID2 = 0x3F1A; Name = sLook + ", " + sDecon + " statue of a god"; break;
				case 21: ItemID = 0x3F1B; RelicFlipID1 = 0x3F1B; RelicFlipID2 = 0x3F1C; Name = sLook + ", " + sDecon + " statue of a knight"; break;
				case 22: ItemID = 0x4688; RelicFlipID1 = 0x4688; RelicFlipID2 = 0x4689; Name = sLook + ", " + sDecon + " statue of a cat"; sMade = "Made of onyx"; Hue = 0; RelicDescription = sMade; break;
				case 23: ItemID = 0x3142; RelicFlipID1 = 0x3143; RelicFlipID2 = 0x3142; Name = sLook + ", " + sDecon + " statue of a lion"; Weight = 100; CoinPrice = Utility.RandomMinMax( 100, 400 ); break;
				case 24: ItemID = 0x3182; RelicFlipID1 = 0x3182; RelicFlipID2 = 0x3182; Name = sLook + ", " + sDecon + " statue of a lion"; Weight = 100; CoinPrice = Utility.RandomMinMax( 100, 400 ); break;
				case 25: ItemID = 0x31C1; RelicFlipID1 = 0x31C1; RelicFlipID2 = 0x31C2; Name = sLook + ", " + sDecon + " statue of a pegasus"; Weight = 150; CoinPrice = Utility.RandomMinMax( 150, 500 ); break;
				case 26: ItemID = 0x31C7; RelicFlipID1 = 0x31C8; RelicFlipID2 = 0x31C7; Name = sLook + ", " + sDecon + " statue of a knight"; break;
				case 27: ItemID = 0x31CB; RelicFlipID1 = 0x31CB; RelicFlipID2 = 0x31CC; Name = sLook + ", " + sDecon + " statue of an explorer"; break;
				case 28: ItemID = 0x31CD; RelicFlipID1 = 0x31CD; RelicFlipID2 = 0x31CE; Name = sLook + ", " + sDecon + " statue of a wizard"; break;
				case 29: ItemID = 0x31CF; RelicFlipID1 = 0x31CF; RelicFlipID2 = 0x31D0; Name = sLook + ", " + sDecon + " statue of a spearman"; break;
				case 30: ItemID = 0x31D1; RelicFlipID1 = 0x31D1; RelicFlipID2 = 0x31D2; Name = sLook + ", " + sDecon + " statue of a priest"; break;
				case 31: ItemID = 0x31D3; RelicFlipID1 = 0x31D3; RelicFlipID2 = 0x31D4; Name = sLook + ", " + sDecon + " statue of a king"; break;
				case 32: ItemID = 0x31FC; RelicFlipID1 = 0x31FC; RelicFlipID2 = 0x31FD; Name = sLook + ", " + sDecon + " statue of a god"; break;
				case 33: ItemID = 0x31FE; RelicFlipID1 = 0x31FE; RelicFlipID2 = 0x31FF; Name = sLook + ", " + sDecon + " statue of a guard"; break;
				case 34: ItemID = 0x320B; RelicFlipID1 = 0x320B; RelicFlipID2 = 0x3219; Name = sLook + ", " + sDecon + " statue of an elf"; break;
				case 35: ItemID = 0x320C; RelicFlipID1 = 0x320C; RelicFlipID2 = 0x3212; Name = sLook + ", " + sDecon + " statue of an elf"; break;
				case 36: ItemID = 0x321F; RelicFlipID1 = 0x321F; RelicFlipID2 = 0x3225; Name = sLook + ", " + sDecon + " statue of an elf"; break;
				case 37: ItemID = 0x322B; RelicFlipID1 = 0x322B; RelicFlipID2 = 0x3235; Name = sLook + ", " + sDecon + " statue of an elf"; break;
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

		public static void MakeOriental( Item item )
		{
			DDRelicStatue relic = (DDRelicStatue)item;

			string sLook = "a rare";
			switch ( Utility.RandomMinMax( 0, 18 ) )
			{
				case 0:	sLook = "a rare";	break;
				case 1:	sLook = "a nice";	break;
				case 2:	sLook = "a pretty";	break;
				case 3:	sLook = "a superb";	break;
				case 4:	sLook = "a delightful";	break;
				case 5:	sLook = "an elegant";	break;
				case 6:	sLook = "an exquisite";	break;
				case 7:	sLook = "a fine";	break;
				case 8:	sLook = "a gorgeous";	break;
				case 9:	sLook = "a lovely";	break;
				case 10:sLook = "a magnificent";	break;
				case 11:sLook = "a marvelous";	break;
				case 12:sLook = "a splendid";	break;
				case 13:sLook = "a wonderful";	break;
				case 14:sLook = "an extraordinary";	break;
				case 15:sLook = "a strange";	break;
				case 16:sLook = "an odd";	break;
				case 17:sLook = "a unique";	break;
				case 18:sLook = "an unusual";	break;
			}
			
			string sDecon = "decorative";
			switch ( Utility.RandomMinMax( 0, 5 ) )
			{
				case 0:	sDecon = "decorative";		break;
				case 1:	sDecon = "ceremonial";		break;
				case 2:	sDecon = "ornamental";		break;
				case 3:	sDecon = Server.Misc.RandomThings.GetRandomOrientalNation();		break;
				case 4:	sDecon = Server.Misc.RandomThings.GetRandomOrientalNation();		break;
				case 5:	sDecon = Server.Misc.RandomThings.GetRandomOrientalNation();		break;
			}

			string OwnerName = Server.Misc.RandomThings.GetRandomOrientalName();
			string OwnerTitle = RandomThings.MagicItemAdj( "end", true, false, item.ItemID );

			switch ( Utility.RandomMinMax( 0, 4 ) ) 
			{
				case 0: relic.ItemID = 0x1947; relic.RelicFlipID1 = 0x1947; relic.RelicFlipID2 = 0x1948; relic.Name = sLook + ", " + sDecon + " statue of Budah"; break;
				case 1: relic.ItemID = 0x2419; relic.RelicFlipID1 = 0x2419; relic.RelicFlipID2 = 0x2419; relic.Name = sLook + ", " + sDecon + " sculpture"; break;
				case 2: relic.ItemID = 0x241A; relic.RelicFlipID1 = 0x241A; relic.RelicFlipID2 = 0x241A; relic.Name = sLook + ", " + sDecon + " sculpture"; break;
				case 3: relic.ItemID = 0x241B; relic.RelicFlipID1 = 0x241B; relic.RelicFlipID2 = 0x241B; relic.Name = sLook + ", " + sDecon + " sculpture"; break;
				case 4: relic.ItemID = 0x2848; relic.RelicFlipID1 = 0x2848; relic.RelicFlipID2 = 0x2849; relic.Name = sLook + " sculpture of " + OwnerName + " " + OwnerTitle; break;
			}
		}

		public DDRelicStatue(Serial serial) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
            writer.Write( (int)1 ); // version
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