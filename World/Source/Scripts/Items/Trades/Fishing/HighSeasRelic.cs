using System;
using Server;
using Server.Misc;
using Server.Mobiles;

namespace Server.Items
{
	public class HighSeasRelic : Item, IRelic
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
		public string RelicOrigin;

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_FlipID1 { get { return RelicFlipID1; } set { RelicFlipID1 = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_FlipID2 { get { return RelicFlipID2; } set { RelicFlipID2 = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Relic_Origin { get { return RelicOrigin; } set { RelicOrigin = value; InvalidateProperties(); } }

		[Constructable]
		public HighSeasRelic() : base( 0x41FD )
		{
			Hue = Utility.RandomList( 0xB97, 0xB98, 0xB99, 0xB9A, 0xB88 );
			NotIdentified = true;

			switch ( Utility.RandomMinMax( 0, 20 ) )
			{
				case 0:	Name = "anchor";				ItemID = 0x897;		Weight = 100.0;		CoinPrice = Utility.RandomMinMax( 80, 200 );
					RelicFlipID1 = 0x897;	RelicFlipID2 = 0x898;	ResourceMods.SetRandomResource( false, false, this, CraftResource.Iron, false, null );	Hue = CraftResources.GetHue(Resource); break;
				case 1:	Name = "ruined barrel";			ItemID = 0x1EB5;	Weight = 20.0;		CoinPrice = Utility.RandomMinMax( 20, 80 );
					RelicFlipID1 = 0x1EB5;	RelicFlipID2 = 0x1EB5;	break;
				case 2:	Name = "old cannon ball";		ItemID = 0x0E73;	Weight = 10.0;		CoinPrice = Utility.RandomMinMax( 10, 20 );
					RelicFlipID1 = 0x0E73;	RelicFlipID2 = 0x0E73;	break;
				case 3:	Name = "old cannon balls";		ItemID = 0x0E74;	Weight = 50.0;		CoinPrice = Utility.RandomMinMax( 20, 40 );
					RelicFlipID1 = 0x0E74;	RelicFlipID2 = 0x0E74;	break;
				case 4:	Name = "broken clock";			ItemID = 0x0C1F;	Weight = 2.0;		CoinPrice = Utility.RandomMinMax( 20, 40 );
					RelicFlipID1 = 0x0C1F;	RelicFlipID2 = 0x0C1F;	break;
				case 5:	Name = "model ship";			ItemID = 0x14F3;	Weight = 5.0;		CoinPrice = Utility.RandomMinMax( 60, 130 );
					RelicFlipID1 = 0x14F3;	RelicFlipID2 = 0x14F4;	break;
				case 6:	Name = "mounted lobster";		ItemID = 0x46BC;	Weight = 2.0;		CoinPrice = Utility.RandomMinMax( 40, 80 );
					RelicFlipID1 = 0x46BC;	RelicFlipID2 = 0x46BD;	break;
				case 7:	Name = "mounted crab";			ItemID = 0x46BA;	Weight = 2.0;		CoinPrice = Utility.RandomMinMax( 40, 80 );
					RelicFlipID1 = 0x46BA;	RelicFlipID2 = 0x46BB;	break;
				case 8:	Name = "ruined spyglass";		ItemID = 0x14F5;	Weight = 1.0;		CoinPrice = Utility.RandomMinMax( 20, 40 );
					RelicFlipID1 = 0x14F5;	RelicFlipID2 = 0x14F6;	break;
				case 9:	Name = "soggy rope";			ItemID = 0x14F8;	Weight = 10.0;		CoinPrice = Utility.RandomMinMax( 10, 30 );
					RelicFlipID1 = 0x14F8;	RelicFlipID2 = 0x14FA;	break;
				case 10: Name = "worn flogging whip";	ItemID = 0x166E;	Weight = 2.0;		CoinPrice = Utility.RandomMinMax( 10, 30 );
					RelicFlipID1 = 0x166E;	RelicFlipID2 = 0x166E;	break;
				case 11: Name = "cracked hourglass";	ItemID = 0x1810;	Weight = 2.0;		CoinPrice = Utility.RandomMinMax( 20, 40 );
					RelicFlipID1 = 0x1810;	RelicFlipID2 = 0x1813;	break;
				case 12: Name = "keg of rum";			ItemID = 0x1AD6;	Weight = 20.0;		CoinPrice = Utility.RandomMinMax( 50, 120 );
					RelicFlipID1 = 0x1AD6;	RelicFlipID2 = 0x1AD7;	Hue = 0x96D;	break;
				case 13: Name = "warped oars";			ItemID = 0x1E2A;	Weight = 10.0;		CoinPrice = Utility.RandomMinMax( 40, 100 );
					RelicFlipID1 = 0x1E2A;	RelicFlipID2 = 0x1E2B;	break;
				case 14: Name = "ruined fishing net"; 	ItemID = 0x1EA3;	Weight = 10.0;		CoinPrice = Utility.RandomMinMax( 30, 90 );
					RelicFlipID1 = 0x1EA3;	RelicFlipID2 = 0x1EA4;	break;
				case 15: Name = "rotten fishing net"; 	ItemID = 0x1EA5;	Weight = 10.0;		CoinPrice = Utility.RandomMinMax( 30, 90 );
					RelicFlipID1 = 0x1EA5;	RelicFlipID2 = 0x1EA6;	break;
				case 16: Name = "bottle of rum";		ItemID = 0xE26;		Weight = 1.0;		CoinPrice = Utility.RandomMinMax( 10, 30 );
					RelicFlipID1 = 0xE26;	RelicFlipID2 = 0xEFE;	break;
				case 17: Name = "broken globe";			ItemID = 0x1047;	Weight = 1.0;		CoinPrice = Utility.RandomMinMax( 20, 40 );
					RelicFlipID1 = 0x1047;	RelicFlipID2 = 0x1048;	break;
				case 18: Name = "busted sextant";		ItemID = 0x1057;	Weight = 1.0;		CoinPrice = Utility.RandomMinMax( 10, 30 );
					RelicFlipID1 = 0x1057;	RelicFlipID2 = 0x1058;	break;
				case 19: Name = "ruined pirate hat";	ItemID = 0x171B;	Weight = 1.0;		CoinPrice = Utility.RandomMinMax( 10, 30 );
					RelicFlipID1 = 0x171B;	RelicFlipID2 = 0x171B;	break;
				case 20: Name = "the captain's log";
							Hue = Utility.RandomColor(0);
							ItemID = Utility.RandomList( 0xFBD, 0xFBE, 0xFEF, 0xFF0, 0xFF1, 0xFF2, 0x42BF, 0xE3B, 0x0E3B, 0x2B6F, 0x2254, 0x42BF );
							Weight = 1.0;
							CoinPrice = Utility.RandomMinMax( 50, 130 );
							RelicFlipID1 = ItemID;	RelicFlipID2 = ItemID;	break;
			}

			string boat = RandomThings.GetRandomShipName( "", 0 );
			RelicOrigin = "Recovered From " + boat;
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1070722, RelicOrigin);
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

		public HighSeasRelic(Serial serial) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
            writer.Write( (int) 1 ); // version
            writer.Write( RelicFlipID1 );
            writer.Write( RelicFlipID2 );
            writer.Write( RelicOrigin );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
            int version = reader.ReadInt();

			if ( version < 1 )
				CoinPrice = reader.ReadInt();

            RelicFlipID1 = reader.ReadInt();
            RelicFlipID2 = reader.ReadInt();
			RelicOrigin = reader.ReadString();
		}
	}
}