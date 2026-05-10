using System;
using Server; 
using System.Collections;
using Server.ContextMenus;
using System.Collections.Generic;
using Server.Misc;
using Server.Network;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Commands;
using System.Globalization;
using Server.Regions;

namespace Server.Items
{
	public class Cargo : Item
	{
		public int CargoValue;
		public int CargoKarma;
		public int CargoType;
		public int CargoMaterial;
		public int CargoBox;
		public int CargoQty;
		public int CargoHue;
		public string CargoFrom;
		public string CargoContains;
		public string CargoVendor;
		public string CargoShip;
		
		[CommandProperty(AccessLevel.Owner)]
		public int Cargo_Value { get { return CargoValue; } set { CargoValue = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Cargo_Karma { get { return CargoKarma; } set { CargoKarma = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Cargo_Type { get { return CargoType; } set { CargoType = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Cargo_Material { get { return CargoMaterial; } set { CargoMaterial = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Cargo_Box { get { return CargoBox; } set { CargoBox = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Cargo_Qty { get { return CargoQty; } set { CargoQty = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Cargo_Hue { get { return CargoHue; } set { CargoHue = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Cargo_From { get { return CargoFrom; } set { CargoFrom = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Cargo_Contains { get { return CargoContains; } set { CargoContains = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Cargo_Vendor { get { return CargoVendor; } set { CargoVendor = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Cargo_Ship { get { return CargoShip; } set { CargoShip = value; InvalidateProperties(); } }

		[Constructable]
		public Cargo() : base( 0x4F86 )
		{
			Weight = 50;
			CreateCargo();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( this.GetWorldLocation(), 4 ) )
			{
				from.CloseGump( typeof( CargoGump ) );
				from.SendGump( new CargoGump( from, this ) );
			}
		}

		public class CargoGump : Gump
		{
			private Cargo m_Cargo;

			public CargoGump( Mobile from, Cargo cargo ): base( 50, 50 )
			{
				from.SendSound( 0x2F ); 
				string color = "#89afe8";
				string value = "#d7ba6e";
				m_Cargo = cargo;
				TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;
				string box = "" + cultInfo.ToTitleCase(cargo.Name) + "";

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				AddImage(0, 0, 7020, Server.Misc.PlayerSettings.GetGumpHue( from ));

				AddHtml( 12, 12, 200, 20, @"<BODY><BASEFONT Color=" + color + ">" + box + "</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( cargo.CargoQty > 0 )
				{
					string virtualName = "";
					if ( cargo.CargoType == 33 )
					{
						if ( cargo.CargoMaterial == 5 ){ virtualName = "Lethal Venom Sacks"; }
						else if ( cargo.CargoMaterial == 4 ){ virtualName = "Deadly Venom Sacks"; }
						else if ( cargo.CargoMaterial == 3 ){ virtualName = "Greater Venom Sacks"; }
						else if ( cargo.CargoMaterial == 2 ){ virtualName = "Venom Sacks"; }
						else { virtualName = "Lesser Venom Sacks"; }
					}
					else if ( cargo.CargoType == 22 )
					{
						if ( cargo.CargoMaterial == 1 ){ virtualName = "Bottles of Liquor"; }
						else if ( cargo.CargoMaterial == 2 ){ virtualName = "Bottles of Ale"; }
						else { virtualName = "Bottles of Wine"; }
					}
					else if ( cargo.CargoType == 35 )
					{
						virtualName = "Stone Bust";
					}
					string ITEM = virtualName;

					if ( virtualName == "" )
					{
						Type itemType = ScriptCompiler.FindTypeByName( cargo.CargoContains );
						if ( itemType == null )
						{
							cargo.Delete();
							return;
						}
						Item item = (Item)Activator.CreateInstance(itemType);
						item.SyncName();
						ITEM = item.Name;
						if ( item.Name != null && item.Name != "" ){ ITEM = item.Name; }
						item.Delete();
					}

					ITEM = "" + cultInfo.ToTitleCase( ITEM ) + "";

					AddHtml( 12, 42, 247, 20, @"<BODY><BASEFONT Color=" + value + ">Contains " + cargo.CargoQty + " " + ITEM + "</BASEFONT></BODY>", (bool)false, (bool)false);
				}

				string karma = "<BODY><BASEFONT Color=#ff0000>Bad</BASEFONT></BODY>"; if ( cargo.CargoKarma > 0 ){ karma = "<BODY><BASEFONT Color=#2cdb51>Good</BASEFONT></BODY>"; }
				AddHtml( 12, 72, 131, 20, @"<BODY><BASEFONT Color=" + color + ">Delivery Karma:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 158, 72, 87, 20, @"" + karma + "", (bool)false, (bool)false);

				AddHtml( 12, 102, 131, 20, @"<BODY><BASEFONT Color=" + color + ">Base Value:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 12, 132, 131, 20, @"<BODY><BASEFONT Color=" + color + ">Sailing Bonus:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 12, 162, 131, 20, @"<BODY><BASEFONT Color=" + color + ">Merchant Bonus:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 12, 192, 131, 20, @"<BODY><BASEFONT Color=" + color + ">Begging Bonus:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 12, 222, 131, 20, @"<BODY><BASEFONT Color=" + color + ">Guild Bonus:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 12, 252, 131, 20, @"<BODY><BASEFONT Color=" + color + ">Port Bonus:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 12, 282, 131, 20, @"<BODY><BASEFONT Color=" + color + ">Total Value:</BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 158, 102, 87, 20, @"<BODY><BASEFONT Color=" + value + ">" + cargo.CargoValue + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 158, 132, 87, 20, @"<BODY><BASEFONT Color=" + value + ">" + CargoFishingGold( cargo, from ) + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 158, 162, 87, 20, @"<BODY><BASEFONT Color=" + value + ">" + CargoMerchantGold( cargo, from ) + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 158, 192, 87, 20, @"<BODY><BASEFONT Color=" + value + ">" + CargoBeggingGold( cargo, from ) + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 158, 222, 87, 20, @"<BODY><BASEFONT Color=" + value + ">" + CargoGuildGold( cargo, from ) + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 158, 252, 87, 20, @"<BODY><BASEFONT Color=" + value + ">" + CargoPortGold( cargo, from ) + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 158, 282, 87, 20, @"<BODY><BASEFONT Color=" + value + ">" + CargoTotalValue( cargo, from ) + "</BASEFONT></BODY>", (bool)false, (bool)false);

				string gotten = "pirated"; if ( cargo.CargoKarma > 0 ){ gotten = "seized"; }
				string boat = "the hold of a small boat"; if ( cargo.CargoShip != null && cargo.CargoShip != "" ){ boat = "the galleon called " + cargo.CargoShip; }
				if ( cargo.CargoQty > 0 )
				{
					AddHtml( 273, 12, 246, 334, @"<BODY><BASEFONT Color=" + value + "><BR><BR><BR>This cargo was " + gotten + " from " + boat + ". You can either keep the container and contents for yourself or you can give it to the " + cargo.CargoVendor + " for a payment. Karma for delivering it to someone is based on whether it was pirated from sailors or seized from criminals. You will only get a port bonus if you deliver it to someone in a port settlement, where pirates and sailors frequent. Only members of the Mariners Guild get the guild bonus for payment.</BASEFONT></BODY>", (bool)false, (bool)false);

					AddButton(12, 325, 4023, 4023, 1, GumpButtonType.Reply, 0);
					AddHtml( 48, 325, 97, 20, @"<BODY><BASEFONT Color=" + value + ">Keep</BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(233, 325, 4020, 4020, 0, GumpButtonType.Reply, 0);
				}
				else
				{
					AddHtml( 273, 12, 246, 334, @"<BODY><BASEFONT Color=" + value + "><BR><BR><BR>This cargo was " + gotten + " from " + boat + ". You can give it to the " + cargo.CargoVendor + " for a payment. Karma for delivering it to someone is based on whether it was pirated from sailors or seized from criminals. You will only get a port bonus if you deliver it to someone in a port settlement, where pirates and sailors frequent. Only members of the Mariners Guild get the guild bonus for payment.</BASEFONT></BODY>", (bool)false, (bool)false);
				}
			}

			public override void OnResponse( NetState state, RelayInfo info ) 
			{
				Mobile from = state.Mobile; 
				from.PlaySound( 0x2E );

				if ( info.ButtonID > 0 && m_Cargo.CargoQty > 0 )
				{
					Container box = new Barrel();
					box.Weight = 10.0;
					box.Name = "wooden barrel";
					box.Hue = 0;
					box.ItemID = 0x0FAE;
					if ( m_Cargo.CargoBox == 1 ){ box.Delete(); box = new LargeCrate(); box.Name = "wooden crate"; }
					else if ( m_Cargo.CargoBox == 2 )
					{
						box.ItemID = 0x4F86; box.Name = "wooden crate"; box.Hue = 0xB61;
						if ( m_Cargo.ItemID >= 0x4F87 && m_Cargo.ItemID <= 0x4F9E ){ box.ItemID = m_Cargo.ItemID; }
					}
					else if ( m_Cargo.CargoBox == 4 ){ box.ItemID = 0x0E83; box.Weight = 8.0; box.Name = "wooden tub"; }

					int qty = m_Cargo.CargoQty;

					if ( m_Cargo.CargoType == 33 )
					{
						VenomSack venom = new VenomSack();

						if ( m_Cargo.CargoMaterial == 5 ){ venom.Name = "lethal venom sack"; }
						else if ( m_Cargo.CargoMaterial == 4 ){ venom.Name = "deadly venom sack"; }
						else if ( m_Cargo.CargoMaterial == 3 ){ venom.Name = "greater venom sack"; }
						else if ( m_Cargo.CargoMaterial == 2 ){ venom.Name = "venom sack"; }
						else { venom.Name = "lesser venom sack"; }

						venom.Amount = qty;
						box.DropItem( venom );
					}
					else if ( m_Cargo.CargoType == 22 )
					{
						Item bottle = new BeverageBottle( BeverageType.Liquor ); bottle.Delete();
						while ( qty > 0 )
						{
							if ( m_Cargo.CargoMaterial == 1 ){ bottle = new BeverageBottle( BeverageType.Liquor ); }
							else if ( m_Cargo.CargoMaterial == 2 ){ bottle = new BeverageBottle( BeverageType.Ale ); }
							else { bottle = new BeverageBottle( BeverageType.Wine ); }
							box.DropItem( bottle );
							qty--;
						}
					}
					else
					{
						Type itemType = ScriptCompiler.FindTypeByName( m_Cargo.CargoContains );
						if ( itemType == null )
						{
							m_Cargo.Delete();
							return;
						}
						Item item = (Item)Activator.CreateInstance(itemType);

						if ( item is DDRelicStatue )
						{
							DDRelicStatue relic = (DDRelicStatue)item;

							item.ItemID = 0x12CA;
							item.Weight = 20.0;
							relic.RelicFlipID1 = 0x12CA;
							relic.RelicFlipID2 = 0x12CB;
							if ( m_Cargo.CargoHue > 0 ){ item.Hue = m_Cargo.CargoHue; }
							if ( Utility.RandomBool() ){ item.Name = "bust of " + RandomThings.GetRandomBoyName() + " the " + RandomThings.GetBoyGirlJob( 0 ); }
							else { item.Name = "bust of " + RandomThings.GetRandomBoyName() + " the " + RandomThings.GetRandomBoyNoble(); }
							relic.RelicDescription = m_Cargo.CargoFrom;
							relic.CoinPrice = m_Cargo.CargoValue;
							relic.NotIdentified = false;
							box.DropItem( item );
						}
						else if ( item.Stackable )
						{
							item.Amount = qty;
							if ( m_Cargo.CargoHue > 0 ){ item.Hue = m_Cargo.CargoHue; }
							box.DropItem( item );
						}
						else
						{
							item.Delete();
							while ( qty > 0 )
							{
								item = (Item)Activator.CreateInstance(itemType);
								if ( m_Cargo.CargoHue > 0 ){ item.Hue = m_Cargo.CargoHue; }
								box.DropItem( item );
								qty--;
							}
						}
					}
					box.MoveToWorld( from.Location, from.Map );
					m_Cargo.Delete();
				}
			}
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);

			string obtained = "Pirated: Worth " + CargoValue + " Gold";
				if ( CargoKarma > 0 ){ obtained = "Seized: Worth " + CargoValue + " Gold"; }

			list.Add( 1070722, obtained);
            list.Add( 1049644, CargoFrom);
        } 

		public Cargo(Serial serial) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
            writer.Write( (int) 0 ); // version
            writer.Write( CargoValue );
            writer.Write( CargoKarma );
            writer.Write( CargoType );
            writer.Write( CargoMaterial );
            writer.Write( CargoBox );
            writer.Write( CargoQty );
            writer.Write( CargoHue );
            writer.Write( CargoFrom );
            writer.Write( CargoContains );
            writer.Write( CargoVendor );
            writer.Write( CargoShip );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
            int version = reader.ReadInt();
            CargoValue = reader.ReadInt();
            CargoKarma = reader.ReadInt();
            CargoType = reader.ReadInt();
            CargoMaterial = reader.ReadInt();
            CargoBox = reader.ReadInt();
            CargoQty = reader.ReadInt();
            CargoHue = reader.ReadInt();
			CargoFrom = reader.ReadString();
			CargoContains = reader.ReadString();
			CargoVendor = reader.ReadString();
			CargoShip = reader.ReadString();
		}

		public void CreateCargo()
		{
			CargoValue = Utility.RandomMinMax( 1000, 3000 );
				CargoValue = (int)(CargoValue * (MyServerSettings.GetGoldCutRate() * .01));
			CargoBox = 1; // 1: CRATE, 2: LARGE CRATE, 3: BARREL, 4: TUB
			CargoMaterial = 0;
			CargoType = Utility.RandomMinMax( 1, 49 );
			CargoFrom = "From " + Server.Misc.RandomThings.MadeUpCity();
			CargoQty = 0;
			CargoHue = 0;
			CargoContains = "";

			if ( CargoType < 4 && Utility.RandomBool() ){ CargoValue = CargoValue*2; }

			if ( CargoType == 1 )
			{
				string[] coins = new string[] {"crystal", "bronze", "agapite", "verite", "valorite", "steel", "brass", "nepturite", "mithril", "caddellite", "iron", "ivory", "wooden", "stone", "obsidian", "bronze", "adamantine", "dwarven", "elven", "dragon", "demon", "marble", "amber", "wizard", "electrum", "platinum", "vulcan", "atlantean", "glass"};
					string coin = coins[Utility.RandomMinMax( 0, (coins.Length-1) )];
				CargoVendor = "Banker or Minter"; ItemID = Utility.RandomList(0x507E,0x507F); Name = "royal coffer of " + coin + " coins"; CargoFrom = "From the " + RandomThings.GetRandomKingdomName() + " " + RandomThings.GetRandomKingdom(); CargoBox = 0;
			}
			else if ( CargoType == 2 ){ CargoVendor = "Art Collector"; ItemID = Utility.RandomList(0x5080,0x5081); Name = "royal vault of riches"; CargoFrom = "From the " + RandomThings.GetRandomKingdomName() + " " + RandomThings.GetRandomKingdom(); CargoBox = 0; }
			else if ( CargoType == 3 ){ CargoVendor = "Shipwright"; ItemID = Utility.RandomList(0x545F,0x5460,0x5082,0x5083); Name = "cargo container"; CargoFrom = "From the " + RandomThings.GetRandomKingdomName() + " " + RandomThings.GetRandomKingdom(); CargoBox = 0; Hue = Utility.RandomColor( 0 ); }
			else if ( CargoType == 4 ){ CargoVendor = "Stable Worker"; ItemID = 0x4F87; Name = "crate of hay"; CargoContains = "SheafOfHay"; CargoQty = (int)(CargoValue/5); }
			else if ( CargoType == 5 ){ CargoVendor = "Bowyer"; ItemID = 0x4F88; Name = "crate of bowcrafting tools"; CargoContains = "FletcherTools"; CargoQty = (int)(CargoValue/5); }
			else if ( CargoType == 6 ){ CargoVendor = "Butcher"; ItemID = 0x4F89; Name = "crate of raw ribs"; CargoContains = "RawRibs"; CargoQty = (int)(CargoValue/16); }
			else if ( CargoType == 7 ){ CargoVendor = "Carpenter"; ItemID = 0x4F8A; Name = "crate of carpenter tools"; CargoContains = "CarpenterTools"; if ( Utility.RandomBool() ){ CargoContains = "WoodworkingTools"; } CargoQty = (int)(CargoValue/5); }
			else if ( CargoType == 8 ){ CargoVendor = "Jeweler"; ItemID = 0x4F8B; Name = "crate of jeweler kits"; CargoContains = "TinkerTools"; CargoQty = (int)(CargoValue/5); }
			else if ( CargoType == 9 )
			{
				CargoVendor = "Mage"; 
				ItemID = 0x4F8C;
				CargoQty = (int)(CargoValue/5);
				CargoMaterial = Utility.RandomMinMax( 1, 8 );
				switch( CargoMaterial )
				{
					case 1: Name = "crate of garlic"; 			CargoContains = "Garlic";			break;
					case 2: Name = "crate of ginseng";			CargoContains = "Ginseng";			break;
					case 3: Name = "crate of mandrake root";	CargoContains = "MandrakeRoot";		break;
					case 4: Name = "crate of nightshade";		CargoContains = "Nightshade";		break;
					case 5: Name = "crate of sulfurous ash";	CargoContains = "SulfurousAsh";		break;
					case 6: Name = "crate of spider silk";		CargoContains = "SpidersSilk";		break;
					case 7: Name = "crate of bloodmoss";		CargoContains = "Bloodmoss";		break;
					case 8: Name = "crate of black pearls";		CargoContains = "BlackPearl";		break;
				}
			}
			else if ( CargoType == 10 ){ CargoVendor = "Blacksmith"; ItemID = 0x4F8D; Name = "crate of smithing hammers"; CargoContains = "SmithHammer"; CargoQty = (int)(CargoValue/16); }
			else if ( CargoType == 11 ){ CargoVendor = "Provisioner"; ItemID = 0x4F8E; Name = "crate of torches"; CargoContains = "Torch"; CargoQty = (int)(CargoValue/8); }
			else if ( CargoType == 12 ){ CargoVendor = "Tailor"; ItemID = 0x4F8F; Name = "crate of sewing kits"; CargoContains = "SewingKit"; CargoQty = (int)(CargoValue/5); }
			else if ( CargoType == 13 ){ CargoVendor = "Tinker"; ItemID = 0x4F90; Name = "crate of tinker tools"; CargoContains = "TinkerTools"; CargoQty = (int)(CargoValue/5); }
			else if ( CargoType == 14 ){ CargoVendor = "Alchemist or Herbalist"; ItemID = 0x4F91; Name = "crate of mortars and pestles"; CargoContains = "MortarPestle"; CargoQty = (int)(CargoValue/8); }
			else if ( CargoType == 15 ){ CargoVendor = "Cook or Baker"; ItemID = 0x4F92; Name = "crate of culinary sets"; CargoContains = "CulinarySet"; CargoQty = (int)(CargoValue/5); }
			else if ( CargoType == 16 )
			{
				CargoVendor = "Banker or Minter"; 
				ItemID = 0x4F93;
				Name = "crate of treasure";
				CargoMaterial = Utility.RandomMinMax( 1, 8 );
				CargoFrom = "From the " + RandomThings.GetRandomKingdomName() + " " + RandomThings.GetRandomKingdom();
				switch( CargoMaterial )
				{
					case 1: CargoContains = "TreasurePile05AddonDeed";	break;
					case 2: CargoContains = "TreasurePile04AddonDeed";	break;
					case 3: CargoContains = "TreasurePile3AddonDeed";	break;
					case 4: CargoContains = "TreasurePile03AddonDeed";	break;
					case 5: CargoContains = "TreasurePile2AddonDeed";	break;
					case 6: CargoContains = "TreasurePile02AddonDeed";	break;
					case 7: CargoContains = "TreasurePile01AddonDeed";	break;
					case 8: CargoContains = "TreasurePileAddonDeed";	break;
				}
			}
			else if ( CargoType == 17 )
			{
				CargoVendor = "Bard"; 
				ItemID = 0x4F94;
				CargoQty = (int)(CargoValue/21);
				CargoMaterial = Utility.RandomMinMax( 1, 5 );
				switch( CargoMaterial )
				{
					case 1: Name = "crate of drums"; CargoContains = "Drums";				break;
					case 2: Name = "crate of tambourines"; CargoContains = "Tambourine";	break;
					case 3: Name = "crate of harps"; CargoContains = "LapHarp";				break;
					case 4: Name = "crate of lutes"; CargoContains = "Lute";				break;
					case 5: Name = "crate of flutes"; CargoContains = "BambooFlute";		break;
				}
			}
			else if ( CargoType == 18 ){ CargoVendor = "Beekeeper"; ItemID = 0x4F95; Name = "crate of beeswax"; CargoContains = "Beeswax"; CargoQty = (int)(CargoValue/50); }
			else if ( CargoType == 19 ){ CargoVendor = "Scribe"; ItemID = 0x4F96; Name = "crate of scribe pens"; CargoContains = "ScribesPen"; CargoQty = (int)(CargoValue/8); }
			else if ( CargoType == 20 ){ CargoVendor = "Bowyer"; ItemID = 0x4F97; Name = "crate of bowyer tools"; CargoContains = "FletcherTools"; CargoQty = (int)(CargoValue/5); }
			else if ( CargoType == 21 ){ CargoVendor = "Healer"; ItemID = 0x4F98; Name = "crate of bandages"; CargoContains = "Bandage"; CargoQty = (int)(CargoValue/5); }
			else if ( CargoType == 22 )
			{
				CargoVendor = "Tavern or Bar Keeper"; 
				ItemID = 0x4F99;
				CargoQty = (int)(CargoValue/7);
				CargoMaterial = Utility.RandomMinMax( 1, 3 );
				switch( CargoMaterial )
				{
					case 1: Name = "crate of liquor"; break;
					case 2: Name = "crate of ale"; break;
					case 3: Name = "crate of wine"; break;
				}
			}
			else if ( CargoType == 23 )
			{
				CargoVendor = "Necromancer or Witch"; 
				ItemID = 0x4F9A;
				CargoMaterial = Utility.RandomMinMax( 1, 19 );
				switch( CargoMaterial )
				{
					case 1: ItemID = 0x55E0;	Name = "crate of bat wings";		CargoContains = "BatWing";			CargoQty = (int)(CargoValue/5);		break;
					case 2: ItemID = 0x55E4;	Name = "crate of daemon blood";		CargoContains = "DaemonBlood";		CargoQty = (int)(CargoValue/6);		break;
					case 3: ItemID = 0x55EC;	Name = "crate of pig iron";			CargoContains = "PigIron";			CargoQty = (int)(CargoValue/5);		break;
					case 4: ItemID = 0x55EA;	Name = "crate of nox crystals";		CargoContains = "NoxCrystal";		CargoQty = (int)(CargoValue/6);		break;
					case 5: ItemID = 0x55E8;	Name = "crate of grave dust";		CargoContains = "GraveDust";		CargoQty = (int)(CargoValue/5);		break;
					case 6: ItemID = 0x6415;	Name = "crate of bitter roots";		CargoContains = "BitterRoot";		CargoQty = (int)(CargoValue/5);		break;
					case 7: ItemID = 0x6416;	Name = "crate of black sand";		CargoContains = "BlackSand";		CargoQty = (int)(CargoValue/5);		break;
					case 8: ItemID = 0x6417;	Name = "crate of blood roses";		CargoContains = "BloodRose";		CargoQty = (int)(CargoValue/5);		break;
					case 9: ItemID = 0x6418;	Name = "crate of dried toads";		CargoContains = "DriedToad";		CargoQty = (int)(CargoValue/5);		break;
					case 10: ItemID = 0x6419;	Name = "crate of maggots";			CargoContains = "Maggot";			CargoQty = (int)(CargoValue/5);		break;
					case 11: ItemID = 0x641A;	Name = "crate of mummy wraps";		CargoContains = "MummyWrap";		CargoQty = (int)(CargoValue/5);		break;
					case 12: ItemID = 0x641B;	Name = "crate of violet fungus";	CargoContains = "VioletFungus";		CargoQty = (int)(CargoValue/5);		break;
					case 13: ItemID = 0x641C;	Name = "crate of werewolf claws";	CargoContains = "WerewolfClaw";		CargoQty = (int)(CargoValue/5);		break;
					case 14: ItemID = 0x641D;	Name = "crate of wolfsbane";		CargoContains = "Wolfsbane";		CargoQty = (int)(CargoValue/5);		break;
					case 15: ItemID = 0x55E0;	Name = "crate of bat wings";		CargoContains = "BatWing";			CargoQty = (int)(CargoValue/5);		break;
					case 16: ItemID = 0x55E4;	Name = "crate of daemon blood";		CargoContains = "DaemonBlood";		CargoQty = (int)(CargoValue/6);		break;
					case 17: ItemID = 0x55EC;	Name = "crate of pig iron";			CargoContains = "PigIron";			CargoQty = (int)(CargoValue/5);		break;
					case 18: ItemID = 0x55EA;	Name = "crate of nox crystals";		CargoContains = "NoxCrystal";		CargoQty = (int)(CargoValue/6);		break;
					case 19: ItemID = 0x55E8;	Name = "crate of grave dust";		CargoContains = "GraveDust";		CargoQty = (int)(CargoValue/5);		break;
				}
			}
			else if ( CargoType == 24 ){ CargoVendor = "Mapmaker"; ItemID = 0x4F9B; Name = "crate of mapping pens"; CargoContains = "MapmakersPen"; CargoQty = (int)(CargoValue/8); }
			else if ( CargoType == 25 )
			{
				CargoVendor = "Shipwright"; 
				ItemID = 0x4F9C;
				CargoMaterial = Utility.RandomMinMax( 1, 3 );
				switch( CargoMaterial )
				{
					case 1: Name = "crate of fabric";			CargoContains = "Fabric";			CargoQty = (int)(CargoValue/5);		break;
					case 2: Name = "crate of rivet metal";		CargoContains = "IronIngot";		CargoQty = (int)(CargoValue/5);		break;
					case 3: Name = "crate of deck planks";		CargoContains = "Board";			CargoQty = (int)(CargoValue/5);		break;
				}
			}
			else if ( CargoType == 26 ){ CargoVendor = "Beekeeper"; ItemID = 0x4F9D; Name = "crate of candles"; CargoContains = "Candle"; CargoQty = (int)(CargoValue/6); }
			else if ( CargoType == 27 )
			{
				CargoVendor = "Weaponsmith or Armorer"; 
				ItemID = 0x4F9E;
				Name = "crate of armor and weapons"; 
				string army = "Army";
				switch( Utility.RandomMinMax( 1, 6 ) )
				{
					case 1: army = "Army";			break;
					case 2: army = "Troops";		break;
					case 3: army = "Guards";		break;
					case 4: army = "Soldiers";		break;
					case 5: army = "Mercenaries";	break;
					case 6: army = "Gladiators";	break;
				}
				CargoFrom = "For the " + army + " of the " + RandomThings.GetRandomKingdomName() + " " + RandomThings.GetRandomKingdom();
				if ( Utility.RandomBool() ){ CargoFrom = "From the " + RandomThings.GetRandomKingdomName() + " " + RandomThings.GetRandomKingdom(); }
			}
			else if ( CargoType == 28 )
			{
				CargoBox = 2;
				CargoVendor = "Blacksmith"; 
				ItemID = 0x50B5;
				int rare = Utility.RandomMinMax( 1, 8192 );
				if ( rare <= 1 ){ CargoMaterial = 14; Name = "crate of dwarven ore"; Hue = CraftResources.GetHue( CraftResource.Dwarven ); 					CargoContains = "DwarvenOre"; 		CargoQty = (int)(CargoValue/96); }
				else if ( rare <= 2 ){ CargoMaterial = 13; Name = "crate of xormite ore"; Hue = CraftResources.GetHue( CraftResource.Xormite ); 			CargoContains = "XormiteOre"; 		CargoQty = (int)(CargoValue/48); }
				else if ( rare <= 4 ){ CargoMaterial = 12; Name = "crate of mithril ore"; Hue = CraftResources.GetHue( CraftResource.Mithril ); 			CargoContains = "MithrilOre"; 		CargoQty = (int)(CargoValue/48); }
				else if ( rare <= 8 ){ CargoMaterial = 11; Name = "crate of obsidian ore"; Hue = CraftResources.GetHue( CraftResource.Obsidian ); 			CargoContains = "ObsidianOre"; 		CargoQty = (int)(CargoValue/36); }
				else if ( rare <= 16 ){ CargoMaterial = 10; Name = "crate of nepturite ore"; Hue = CraftResources.GetHue( CraftResource.Nepturite ); 		CargoContains = "NepturiteOre"; 	CargoQty = (int)(CargoValue/36); }
				else if ( rare <= 32 ){ CargoMaterial = 9; Name = "crate of valorite ore"; Hue = CraftResources.GetHue( CraftResource.Valorite ); 			CargoContains = "ValoriteOre"; 		CargoQty = (int)(CargoValue/36); }
				else if ( rare <= 64 ){ CargoMaterial = 8; Name = "crate of verite ore"; Hue = CraftResources.GetHue( CraftResource.Verite ); 				CargoContains = "VeriteOre"; 		CargoQty = (int)(CargoValue/32); }
				else if ( rare <= 128 ){ CargoMaterial = 7; Name = "crate of agapite ore"; Hue = CraftResources.GetHue( CraftResource.Agapite ); 			CargoContains = "AgapiteOre"; 		CargoQty = (int)(CargoValue/28); }
				else if ( rare <= 256 ){ CargoMaterial = 6; Name = "crate of gold ore"; Hue = CraftResources.GetHue( CraftResource.Gold ); 					CargoContains = "GoldOre"; 			CargoQty = (int)(CargoValue/24); }
				else if ( rare <= 512 ){ CargoMaterial = 5; Name = "crate of bronze ore"; Hue = CraftResources.GetHue( CraftResource.Bronze ); 				CargoContains = "BronzeOre"; 		CargoQty = (int)(CargoValue/20); }
				else if ( rare <= 1024 ){ CargoMaterial = 4; Name = "crate of copper ore"; Hue = CraftResources.GetHue( CraftResource.Copper ); 			CargoContains = "CopperOre"; 		CargoQty = (int)(CargoValue/16); }
				else if ( rare <= 2048 ){ CargoMaterial = 3; Name = "crate of shadow iron ore"; Hue = CraftResources.GetHue( CraftResource.ShadowIron ); 	CargoContains = "ShadowIronOre"; 	CargoQty = (int)(CargoValue/12); }
				else if ( rare <= 4096 ){ CargoMaterial = 2; Name = "crate of dull copper ore"; Hue = CraftResources.GetHue( CraftResource.DullCopper ); 	CargoContains = "DullCopperOre"; 	CargoQty = (int)(CargoValue/8); }
				else { CargoMaterial = 1; ItemID = 0x5084; Name = "crate of iron ore"; 																		CargoContains = "IronOre"; 			CargoQty = (int)(CargoValue/5); }
			}
			else if ( CargoType == 29 )
			{
				CargoVendor = "Bowyer"; 
				CargoMaterial = Utility.RandomMinMax( 1, 4 );
				switch( CargoMaterial )
				{
					case 1: ItemID = 0x5086; Name = "crate of arrows";		CargoContains = "Arrow";		CargoQty = (int)(CargoValue/5);	break;
					case 2: ItemID = 0x509F; Name = "crate of shafts";		CargoContains = "Shaft";		CargoQty = (int)(CargoValue/5);	break;
					case 3: ItemID = 0x50AA; Name = "crate of bolts";		CargoContains = "Bolt";			CargoQty = (int)(CargoValue/5);	break;
					case 4: ItemID = 0x5091; Name = "crate of feathers";	CargoContains = "Feather";		CargoQty = (int)(CargoValue/5);	break;
				}
			}
			else if ( CargoType == 30 ){ CargoVendor = "Tailor"; ItemID = 0x5089; Name = "crate of cloth"; CargoBox = 2; Hue = Utility.RandomColor( 0 ); CargoHue = Hue; CargoContains = "Fabric"; CargoQty = (int)(CargoValue); }
			else if ( CargoType == 31 ){ CargoVendor = "Provisioner"; ItemID = 0x508A; Name = "crate of pouches"; CargoContains = "Pouch"; CargoQty = (int)(CargoValue/5); }
			else if ( CargoType == 32 ){ CargoVendor = "Alchemist or Herbalist"; ItemID = 0x5090; Name = "crate of bottles"; CargoContains = "Bottle"; CargoQty = (int)(CargoValue/5); }
			else if ( CargoType == 33 )
			{
				CargoVendor = "Thief or Assassin"; 
				ItemID = 0x50A7;
				int rare = Utility.RandomMinMax( 1, 16 );

				if ( rare <= 1 ){ CargoMaterial = 5; Name = "crate of lethal venom sacks"; CargoQty = (int)(CargoValue/75); }
				else if ( rare <= 2 ){ CargoMaterial = 4; Name = "crate of deadly venom sacks"; CargoQty = (int)(CargoValue/60); }
				else if ( rare <= 4 ){ CargoMaterial = 3; Name = "crate of greater venom sacks"; CargoQty = (int)(CargoValue/45); }
				else if ( rare <= 8 ){ CargoMaterial = 2; Name = "crate of venom sacks"; CargoQty = (int)(CargoValue/30); }
				else { CargoMaterial = 1; Name = "crate of lesser venom sacks"; CargoQty = (int)(CargoValue/15); }
			}
			else if ( CargoType == 34 ){ CargoVendor = "Butcher"; ItemID = 0x50A8; Name = "crate of cleavers"; CargoContains = "Cleaver"; CargoQty = (int)(CargoValue/15); }
			else if ( CargoType == 35 ){ CargoVendor = "Art Collector"; ItemID = 0x50A9; Name = "crated stone bust"; Hue = Utility.RandomColor( 0 ); CargoHue = Hue; CargoContains = "DDRelicStatue"; CargoQty = 1; }
			else if ( CargoType == 36 ){ CargoVendor = "Sage"; ItemID = 0x50AB; Name = "crate of books"; CargoContains = "TanBook"; CargoQty = (int)(CargoValue/15); }
			else if ( CargoType == 37 ){ CargoVendor = "Provisioner"; ItemID = 0x50A0; Name = "crate of torches"; CargoContains = "Torch"; CargoQty = (int)(CargoValue/8); }
			else if ( CargoType == 38 )
			{
				ItemID = 0x55DF;
				CargoVendor = "Alchemist or Herbalist"; 
				CargoMaterial = Utility.RandomMinMax( 1, 27 );
				switch( CargoMaterial )
				{
					case 1: Name = "agility"; 					Hue = 396;		CargoContains = "AgilityPotion"; 				CargoQty = (int)(CargoValue/30); 	break;
					case 2: Name = "greater agility"; 			Hue = 396;		CargoContains = "GreaterAgilityPotion"; 		CargoQty = (int)(CargoValue/60); 	break;
					case 3: Name = "conflagration"; 			Hue = 0xAD8;	CargoContains = "ConflagrationPotion"; 			CargoQty = (int)(CargoValue/30); 	break;
					case 4: Name = "greater conflagration"; 	Hue = 0xAD8;	CargoContains = "GreaterConflagrationPotion"; 	CargoQty = (int)(CargoValue/60); 	break;
					case 5: Name = "confusion blast"; 			Hue = 0x495;	CargoContains = "ConfusionBlastPotion"; 		CargoQty = (int)(CargoValue/30); 	break;
					case 6: Name = "greater confusion blast"; 	Hue = 0x495;	CargoContains = "GreaterConfusionBlastPotion"; 	CargoQty = (int)(CargoValue/60); 	break;
					case 7: Name = "lesser cure"; 				Hue = 45;		CargoContains = "LesserCurePotion"; 			CargoQty = (int)(CargoValue/15); 	break;
					case 8: Name = "cure"; 						Hue = 45;		CargoContains = "CurePotion"; 					CargoQty = (int)(CargoValue/30); 	break;
					case 9: Name = "greater cure"; 				Hue = 45;		CargoContains = "GreaterCurePotion"; 			CargoQty = (int)(CargoValue/60); 	break;
					case 10: Name = "lesser explosion"; 		Hue = 425;		CargoContains = "LesserExplosionPotion"; 		CargoQty = (int)(CargoValue/15); 	break;
					case 11: Name = "explosion"; 				Hue = 425;		CargoContains = "ExplosionPotion"; 				CargoQty = (int)(CargoValue/30); 	break;
					case 12: Name = "greater explosion"; 		Hue = 425;		CargoContains = "GreaterExplosionPotion"; 		CargoQty = (int)(CargoValue/60); 	break;
					case 13: Name = "frostbite"; 				Hue = 0xAF3;	CargoContains = "FrostbitePotion"; 				CargoQty = (int)(CargoValue/30); 	break;
					case 14: Name = "greater frostbite"; 		Hue = 0xAF3;	CargoContains = "GreaterFrostbitePotion"; 		CargoQty = (int)(CargoValue/60); 	break;
					case 15: Name = "lesser heal"; 				Hue = 50;		CargoContains = "LesserHealPotion"; 			CargoQty = (int)(CargoValue/15); 	break;
					case 16: Name = "heal"; 					Hue = 50;		CargoContains = "HealPotion"; 					CargoQty = (int)(CargoValue/30); 	break;
					case 17: Name = "greater heal"; 			Hue = 50;		CargoContains = "GreaterHealPotion"; 			CargoQty = (int)(CargoValue/60); 	break;
					case 18: Name = "night sight"; 				Hue = 1109;		CargoContains = "NightSightPotion"; 			CargoQty = (int)(CargoValue/15); 	break;
					case 19: Name = "lesser poison"; 			Hue = 73;		CargoContains = "LesserPoisonPotion"; 			CargoQty = (int)(CargoValue/15); 	break;
					case 20: Name = "poison"; 					Hue = 73;		CargoContains = "PoisonPotion"; 				CargoQty = (int)(CargoValue/30); 	break;
					case 21: Name = "greater poison"; 			Hue = 73;		CargoContains = "GreaterPoisonPotion"; 			CargoQty = (int)(CargoValue/60); 	break;
					case 22: Name = "deadly poison"; 			Hue = 73;		CargoContains = "DeadlyPoisonPotion"; 			CargoQty = (int)(CargoValue/70); 	break;
					case 23: Name = "lethal poison"; 			Hue = 73;		CargoContains = "LethalPoisonPotion"; 			CargoQty = (int)(CargoValue/80); 	break;
					case 24: Name = "refresh"; 					Hue = 140;		CargoContains = "RefreshPotion"; 				CargoQty = (int)(CargoValue/30); 	break;
					case 25: Name = "total refresh"; 			Hue = 140;		CargoContains = "TotalRefreshPotion"; 			CargoQty = (int)(CargoValue/60); 	break;
					case 26: Name = "strength"; 				Hue = 1001;		CargoContains = "StrengthPotion"; 				CargoQty = (int)(CargoValue/30); 	break;
					case 27: Name = "greater strength"; 		Hue = 1001;		CargoContains = "GreaterStrengthPotion"; 		CargoQty = (int)(CargoValue/60); 	break;
				}
				Name = "crate of " + Name + " potions";
			}
			else if ( CargoType == 39 ){ CargoVendor = "Beekeeper"; ItemID = 0x50AC; Name = "crate of candles"; CargoContains = "CandleLong"; CargoQty = (int)(CargoValue/6); }
			else if ( CargoType == 40 ){ CargoVendor = "Healer"; ItemID = 0x5087; Name = "crate of bandages"; if ( Utility.RandomBool() ){ ItemID = 0x50A6; } CargoContains = "Bandage"; CargoQty = (int)(CargoValue/5); }
			else if ( CargoType == 41 )
			{
				CargoVendor = "Alchemist or Herbalist"; 
				CargoQty = (int)(CargoValue/5);
				CargoMaterial = Utility.RandomMinMax( 1, 20 );
				switch( CargoMaterial )
				{
					case 1: ItemID = 0x5098; Name = "crate of garlic"; 				CargoContains = "Garlic";			break;
					case 2: ItemID = 0x5099; Name = "crate of ginseng";				CargoContains = "Ginseng";			break;
					case 3: ItemID = 0x509A; Name = "crate of mandrake root";		CargoContains = "MandrakeRoot";		break;
					case 4: ItemID = 0x509B; Name = "crate of nightshade";			CargoContains = "Nightshade";		break;
					case 5: ItemID = 0x509C; Name = "crate of sulfurous ash";		CargoContains = "SulfurousAsh";		break;
					case 6: ItemID = 0x509D; Name = "crate of spider silk";			CargoContains = "SpidersSilk";		break;
					case 7: ItemID = 0x508E; Name = "crate of bloodmoss";			CargoContains = "Bloodmoss";		break;
					case 8: ItemID = 0x508F; Name = "crate of black pearls";		CargoContains = "BlackPearl";		break;
					case 9: ItemID = 0x55E5; Name = "crate of toad eyes";			CargoContains = "EyeOfToad";		break;
					case 10: ItemID = 0x55E6; Name = "crate of fairy eggs";			CargoContains = "FairyEgg";			break;
					case 11: ItemID = 0x55E7; Name = "crate of gargoyle ears";		CargoContains = "GargoyleEar";		break;
					case 12: ItemID = 0x55E1; Name = "crate of beetle shells";		CargoContains = "BeetleShell";		break;
					case 13: ItemID = 0x55E9; Name = "crate of moon crystals";		CargoContains = "MoonCrystal";		break;
					case 14: ItemID = 0x55ED; Name = "crate of pixie skulls";		CargoContains = "PixieSkull";		break;
					case 15: ItemID = 0x55EE; Name = "crate of red lotus";			CargoContains = "RedLotus";			break;
					case 16: ItemID = 0x55EF; Name = "crate of sea salt";			CargoContains = "SeaSalt";			break;
					case 17: ItemID = 0x55EB; Name = "crate of silver widows";		CargoContains = "SilverWidow";		break;
					case 18: ItemID = 0x568A; Name = "crate of swamp berries";		CargoContains = "SwampBerries";		break;
					case 19: ItemID = 0x55E2; Name = "crate of brimstone";			CargoContains = "Brimstone";		break;
					case 20: ItemID = 0x55E3; Name = "crate of butterfly wings";	CargoContains = "ButterflyWings";	break;
				}
			}
			else if ( CargoType == 42 )
			{ 
				CargoVendor = "Cook"; 
				CargoQty = (int)(CargoValue/5);
				CargoMaterial = Utility.RandomMinMax( 1, 4 );
				switch( CargoMaterial )
				{
					case 1: ItemID = 0x508B; Name = "crate of fish fillets"; 	CargoContains = "RawFishSteak";		CargoQty = (int)(CargoValue/5);		break;
					case 2: ItemID = 0x508C; Name = "crate of lamb";			CargoContains = "RawLambLeg";		CargoQty = (int)(CargoValue/9);		break;
					case 3: ItemID = 0x508D; Name = "crate of raw ribs";		CargoContains = "RawRibs";			CargoQty = (int)(CargoValue/16);	break;
					case 4: ItemID = 0x50B9; Name = "crate of fish";			CargoContains = "Fish";				CargoQty = (int)(CargoValue/6);		break;
				}
			}
			else if ( CargoType == 43 ){ CargoVendor = "Scribe"; ItemID = 0x509E; Name = "crate of blank scrolls"; if ( Utility.RandomBool() ){ ItemID = 0x50A5; } CargoContains = "BlankScroll"; CargoQty = (int)(CargoValue/5); }
			else if ( CargoType == 44 )
			{
				CargoVendor = "Baker"; 
				CargoMaterial = Utility.RandomMinMax( 1, 3 );
				switch( CargoMaterial )
				{
					case 1: ItemID = 0x50BA; Name = "crate of bread"; 		CargoContains = "BreadLoaf";		CargoQty = (int)(CargoValue/5);						break;
					case 2: ItemID = 0x50B0; Name = "tub of bread";			CargoContains = "BreadLoaf";		CargoQty = (int)(CargoValue/5);		CargoBox = 4;	break;
					case 3: ItemID = 0x50AE; Name = "crate of wheat";		CargoContains = "WheatSheaf";		CargoQty = (int)(CargoValue/5);						break;
				}
			}
			else if ( CargoType == 45 )
			{
				CargoVendor = "Farmer"; 
				CargoMaterial = Utility.RandomMinMax( 1, 25 );
				switch( CargoMaterial )
				{
					case 1: ItemID = 0x50A2; Name = "crate of pumpkins"; 						CargoContains = "Pumpkin";		CargoQty = (int)(CargoValue/11);		break;
					case 2: ItemID = 0x50A3; Name = "crate of carrots"; 						CargoContains = "Carrot";		CargoQty = (int)(CargoValue/5);		break;
					case 3: ItemID = 0x50A4; Name = "crate of bananas"; 						CargoContains = "Banana";		CargoQty = (int)(CargoValue/5);		break;
					case 4: ItemID = 0x50AD; Name = "crate of squash"; 							CargoContains = "Squash";		CargoQty = (int)(CargoValue/5);		break;
					case 5: ItemID = 0x50C4; Name = "crate of squash"; 							CargoContains = "Squash";		CargoQty = (int)(CargoValue/5);		break;
					case 6: ItemID = 0x50C5; Name = "crate of pumpkins"; 						CargoContains = "Pumpkin";		CargoQty = (int)(CargoValue/5);		break;
					case 7: ItemID = 0x50C6; Name = "crate of limes"; 							CargoContains = "Lime";			CargoQty = (int)(CargoValue/5);		break;
					case 8: ItemID = 0x50C7; Name = "crate of lemons"; 							CargoContains = "Lemon";		CargoQty = (int)(CargoValue/5);		break;
					case 9: ItemID = 0x50B3; Name = "crate of grapes"; 							CargoContains = "Grapes";		CargoQty = (int)(CargoValue/5);		break;
					case 10: ItemID = 0x50BB; Name = "crate of apples"; 						CargoContains = "Apple";		CargoQty = (int)(CargoValue/5);		break;
					case 11: ItemID = 0x50AF; Name = "tub of grapes"; 		CargoBox = 4; 		CargoContains = "Grapes";		CargoQty = (int)(CargoValue/5);		break;
					case 12: ItemID = 0x50B1; Name = "tub of lemons"; 		CargoBox = 4; 		CargoContains = "Lemon";		CargoQty = (int)(CargoValue/5);		break;
					case 13: ItemID = 0x50B2; Name = "tub of limes"; 		CargoBox = 4; 		CargoContains = "Lime";			CargoQty = (int)(CargoValue/5);		break;
					case 14: ItemID = 0x50BC; Name = "tub of pumpkins"; 	CargoBox = 4; 		CargoContains = "Pumpkin";		CargoQty = (int)(CargoValue/5);		break;
					case 15: ItemID = 0x50BD; Name = "tub of vegetables"; 	CargoBox = 4; 										CargoQty = (int)(CargoValue/5);		break;
					case 16: ItemID = 0x50BE; Name = "tub of vegetables"; 	CargoBox = 4; 										CargoQty = (int)(CargoValue/5);		break;
					case 17: ItemID = 0x50BF; Name = "tub of apples"; 		CargoBox = 4; 		CargoContains = "Apple";		CargoQty = (int)(CargoValue/5);		break;
					case 18: ItemID = 0x50C0; Name = "tub of grapes"; 		CargoBox = 4; 		CargoContains = "Grapes";		CargoQty = (int)(CargoValue/5);		break;
					case 19: ItemID = 0x50C1; Name = "barrel of apples"; 	CargoBox = 3; 		CargoContains = "Apple";		CargoQty = (int)(CargoValue/5);		break;
					case 20: ItemID = 0x50C2; Name = "barrel of bananas"; 	CargoBox = 3; 		CargoContains = "Banana";		CargoQty = (int)(CargoValue/5);		break;
					case 21: ItemID = 0x50C3; Name = "barrel of limes"; 	CargoBox = 3; 		CargoContains = "Lime";			CargoQty = (int)(CargoValue/5);		break;
					case 22: ItemID = 0x50B4; Name = "barrel of pumpkins"; 	CargoBox = 3; 		CargoContains = "Pumpkin";		CargoQty = (int)(CargoValue/5);		break;
					case 23: ItemID = 0x50B6; Name = "barrel of dates"; 	CargoBox = 3; 		CargoContains = "Dates";		CargoQty = (int)(CargoValue/5);		break;
					case 24: ItemID = 0x50B7; Name = "barrel of grapes"; 	CargoBox = 3; 		CargoContains = "Grapes";		CargoQty = (int)(CargoValue/5);		break;
					case 25: ItemID = 0x50B8; Name = "barrel of lemons"; 	CargoBox = 3; 		CargoContains = "Lemon";		CargoQty = (int)(CargoValue/5);		break;
				}

				if ( CargoMaterial == 15 || CargoMaterial == 16 )
				{
					switch( Utility.RandomMinMax( 1, 5 ) )
					{
						case 1: Name = "tub of cabbage";	CargoContains = "Cabbage";		break;
						case 2: Name = "tub of carrots";	CargoContains = "Carrot";		break;
						case 3: Name = "tub of squash";		CargoContains = "Squash";		break;
						case 4: Name = "tub of lettuce";	CargoContains = "Lettuce";		break;
						case 5: Name = "tub of onions";		CargoContains = "Onion";		break;
					}
				}
			}
			else if ( CargoType == 46 )
			{
				CargoVendor = "Blacksmith"; 
				ItemID = 0x5095;
				int rare = Utility.RandomMinMax( 1, 32768 );

				//else if ( rare <= 2 ){ CargoMaterial = 15; Name = "crate of steel ingots"; Hue = CraftResources.GetHue( CraftResource.Steel ); 				CargoContains = "SteelIngot"; 	CargoQty = (int)(CargoValue/48);	}
				//else if ( rare <= 2 ){ CargoMaterial = 15; Name = "crate of brass ingots"; Hue = CraftResources.GetHue( CraftResource.Brass ); 				CargoContains = "BrassIngot"; 	CargoQty = (int)(CargoValue/48);	}

				if ( rare <= 1 ){ CargoMaterial = 16; Name = "crate of dwarven ingots"; Hue = CraftResources.GetHue( CraftResource.Dwarven ); 					CargoContains = "DwarvenIngot"; 	CargoQty = (int)(CargoValue/96);	}
				else if ( rare <= 2 ){ CargoMaterial = 15; Name = "crate of xormite ingots"; Hue = CraftResources.GetHue( CraftResource.Xormite ); 				CargoContains = "XormiteIngot"; 	CargoQty = (int)(CargoValue/48);	}
				else if ( rare <= 4 ){ CargoMaterial = 14; Name = "crate of mithril ingots"; Hue = CraftResources.GetHue( CraftResource.Mithril ); 				CargoContains = "MithrilIngot"; 	CargoQty = (int)(CargoValue/48);	}
				else if ( rare <= 8 ){ CargoMaterial = 13; Name = "crate of brass ingots"; Hue = CraftResources.GetHue( CraftResource.Brass ); 					CargoContains = "BrassIngot"; 		CargoQty = (int)(CargoValue/44);	}
				else if ( rare <= 16 ){ CargoMaterial = 12; Name = "crate of steel ingots"; Hue = CraftResources.GetHue( CraftResource.Steel ); 				CargoContains = "SteelIngot"; 		CargoQty = (int)(CargoValue/40);	}
				else if ( rare <= 32 ){ CargoMaterial = 11; Name = "crate of obsidian ingots"; Hue = CraftResources.GetHue( CraftResource.Obsidian ); 			CargoContains = "ObsidianIngot"; 	CargoQty = (int)(CargoValue/36);	}
				else if ( rare <= 64 ){ CargoMaterial = 10; Name = "crate of nepturite ingots"; Hue = CraftResources.GetHue( CraftResource.Nepturite ); 		CargoContains = "NepturiteIngot"; 	CargoQty = (int)(CargoValue/36);	}
				else if ( rare <= 128 ){ CargoMaterial = 9; Name = "crate of valorite ingots"; Hue = CraftResources.GetHue( CraftResource.Valorite ); 			CargoContains = "ValoriteIngot"; 	CargoQty = (int)(CargoValue/36);	}
				else if ( rare <= 256 ){ CargoMaterial = 8; Name = "crate of verite ingots"; Hue = CraftResources.GetHue( CraftResource.Verite ); 				CargoContains = "VeriteIngot"; 		CargoQty = (int)(CargoValue/32);	}
				else if ( rare <= 512 ){ CargoMaterial = 7; Name = "crate of agapite ingots"; Hue = CraftResources.GetHue( CraftResource.Agapite ); 			CargoContains = "AgapiteIngot"; 	CargoQty = (int)(CargoValue/28);	}
				else if ( rare <= 1024 ){ CargoMaterial = 6; Name = "crate of gold ingots"; Hue = CraftResources.GetHue( CraftResource.Gold ); 					CargoContains = "GoldIngot"; 		CargoQty = (int)(CargoValue/24);	}
				else if ( rare <= 2048 ){ CargoMaterial = 5; Name = "crate of bronze ingots"; Hue = CraftResources.GetHue( CraftResource.Bronze ); 				CargoContains = "BronzeIngot"; 		CargoQty = (int)(CargoValue/20);	}
				else if ( rare <= 4096 ){ CargoMaterial = 4; Name = "crate of copper ingots"; Hue = CraftResources.GetHue( CraftResource.Copper ); 				CargoContains = "CopperIngot"; 		CargoQty = (int)(CargoValue/16);	}
				else if ( rare <= 8192 ){ CargoMaterial = 3; Name = "crate of shadow iron ingots"; Hue = CraftResources.GetHue( CraftResource.ShadowIron ); 	CargoContains = "ShadowIronIngot"; 	CargoQty = (int)(CargoValue/12);	}
				else if ( rare <= 16384 ){ CargoMaterial = 2; Name = "crate of dull copper ingots"; Hue = CraftResources.GetHue( CraftResource.DullCopper ); 	CargoContains = "DullCopperIngot"; 	CargoQty = (int)(CargoValue/8);		}
				else { CargoMaterial = 2; ItemID = 0x5094; Name = "crate of iron ingots";																		CargoContains = "IronIngot"; 		CargoQty = (int)(CargoValue/5);		}
			}
			else if ( CargoType == 47 )
			{
				CargoVendor = "Leatherworker or Tanner"; 
				ItemID = 0x5093;
				int rare = Utility.RandomMinMax( 1, 2048 );
				if ( rare <= 1 ){ CargoMaterial = 12; Name = "crate of alien hides"; Hue = CraftResources.GetHue( CraftResource.AlienLeather );					CargoContains = "AlienHides"; 		CargoQty = (int)(CargoValue/14);	}
				else if ( rare <= 2 ){ CargoMaterial = 11; Name = "crate of dinosaur hides"; Hue = CraftResources.GetHue( CraftResource.DinosaurLeather );		CargoContains = "DinosaurHides"; 	CargoQty = (int)(CargoValue/14);	}
				else if ( rare <= 4 ){ CargoMaterial = 10; Name = "crate of hellish hides"; Hue = CraftResources.GetHue( CraftResource.HellishLeather );		CargoContains = "HellishHides"; 	CargoQty = (int)(CargoValue/14);	}
				else if ( rare <= 8 ){ CargoMaterial = 9; Name = "crate of draconic hides"; Hue = CraftResources.GetHue( CraftResource.DraconicLeather );		CargoContains = "DraconicHides"; 	CargoQty = (int)(CargoValue/12);	}
				else if ( rare <= 16 ){ CargoMaterial = 8; Name = "crate of goliath hides"; Hue = CraftResources.GetHue( CraftResource.GoliathLeather );		CargoContains = "GoliathHides"; 	CargoQty = (int)(CargoValue/12);	}
				else if ( rare <= 32 ){ CargoMaterial = 7; Name = "crate of frozen hides"; Hue = CraftResources.GetHue( CraftResource.FrozenLeather );			CargoContains = "FrozenHides"; 		CargoQty = (int)(CargoValue/10);	}
				else if ( rare <= 64 ){ CargoMaterial = 6; Name = "crate of volcanic hides"; Hue = CraftResources.GetHue( CraftResource.VolcanicLeather );		CargoContains = "VolcanicHides"; 	CargoQty = (int)(CargoValue/10);	}
				else if ( rare <= 128 ){ CargoMaterial = 5; Name = "crate of necrotic hides"; Hue = CraftResources.GetHue( CraftResource.NecroticLeather );		CargoContains = "NecroticHides"; 	CargoQty = (int)(CargoValue/8);		}
				else if ( rare <= 256 ){ CargoMaterial = 4; Name = "crate of serpent hides"; Hue = CraftResources.GetHue( CraftResource.BarbedLeather );		CargoContains = "BarbedHides"; 		CargoQty = (int)(CargoValue/8);		}
				else if ( rare <= 512 ){ CargoMaterial = 3; Name = "crate of lizard hides"; Hue = CraftResources.GetHue( CraftResource.HornedLeather );			CargoContains = "HornedHides"; 		CargoQty = (int)(CargoValue/6);		}
				else if ( rare <= 1024 ){ CargoMaterial = 2; Name = "crate of deep sea hides"; Hue = CraftResources.GetHue( CraftResource.SpinedLeather );		CargoContains = "SpinedHides"; 		CargoQty = (int)(CargoValue/6);		}
				else { CargoMaterial = 1; ItemID = 0x5092; Name = "crate of hides";																				CargoContains = "Hides"; 			CargoQty = (int)(CargoValue/5);		}
			}
			else if ( CargoType == 48 )
			{
				CargoVendor = "Carpenter"; 
				int logger = Utility.RandomMinMax( 1, 2 );
					ItemID = 0x5085;
					string woods = "boards";
					int ordin = 0x5088;
					if ( logger == 2 ){ ItemID = 0x5096; woods = "logs"; ordin = 0x5097; CargoBox = 2; }

				int rare = Utility.RandomMinMax( 1, 16384 );

				if ( rare <= 1 ){ CargoMaterial = 15; Name = "crate of elven " + woods; Hue = CraftResources.GetHue( CraftResource.ElvenTree );						CargoContains = "ElvenBoard"; 		if ( logger == 2 ){ CargoContains = "ElvenLog"; }		CargoQty = (int)(CargoValue/14);}
				else if ( rare <= 2 ){ CargoMaterial = 14; Name = "crate of driftwood " + woods; Hue = CraftResources.GetHue( CraftResource.DriftwoodTree );		CargoContains = "DriftwoodBoard"; 	if ( logger == 2 ){ CargoContains = "DriftwoodLog"; }	CargoQty = (int)(CargoValue/5);	}
				else if ( rare <= 4 ){ CargoMaterial = 13; Name = "crate of petrified " + woods; Hue = CraftResources.GetHue( CraftResource.PetrifiedTree );		CargoContains = "PetrifiedBoard"; 	if ( logger == 2 ){ CargoContains = "PetrifiedLog"; }	CargoQty = (int)(CargoValue/8);	}
				else if ( rare <= 8 ){ CargoMaterial = 12; Name = "crate of walnut " + woods; Hue = CraftResources.GetHue( CraftResource.WalnutTree );				CargoContains = "WalnutBoard"; 		if ( logger == 2 ){ CargoContains = "WalnutLog"; }		CargoQty = (int)(CargoValue/7);	}
				else if ( rare <= 16 ){ CargoMaterial = 11; Name = "crate of rosewood " + woods; Hue = CraftResources.GetHue( CraftResource.RosewoodTree );			CargoContains = "RosewoodBoard"; 	if ( logger == 2 ){ CargoContains = "RosewoodLog"; }	CargoQty = (int)(CargoValue/7);	}
				else if ( rare <= 32 ){ CargoMaterial = 10; Name = "crate of ghostwood " + woods; Hue = CraftResources.GetHue( CraftResource.GhostTree );			CargoContains = "GhostBoard"; 		if ( logger == 2 ){ CargoContains = "GhostLog"; }		CargoQty = (int)(CargoValue/6);	}
				else if ( rare <= 64 ){ CargoMaterial = 9; Name = "crate of pine " + woods; Hue = CraftResources.GetHue( CraftResource.PineTree );					CargoContains = "PineBoard"; 		if ( logger == 2 ){ CargoContains = "PineLog"; }		CargoQty = (int)(CargoValue/6);	}
				else if ( rare <= 128 ){ CargoMaterial = 8; Name = "crate of oak " + woods; Hue = CraftResources.GetHue( CraftResource.OakTree );					CargoContains = "OakBoard"; 		if ( logger == 2 ){ CargoContains = "OakLog"; }			CargoQty = (int)(CargoValue/6);	}
				else if ( rare <= 256 ){ CargoMaterial = 7; Name = "crate of mahogany " + woods; Hue = CraftResources.GetHue( CraftResource.MahoganyTree );			CargoContains = "MahoganyBoard"; 	if ( logger == 2 ){ CargoContains = "MahoganyLog"; }	CargoQty = (int)(CargoValue/5);	}
				else if ( rare <= 512 ){ CargoMaterial = 6; Name = "crate of hickory " + woods; Hue = CraftResources.GetHue( CraftResource.HickoryTree );			CargoContains = "HickoryBoard"; 	if ( logger == 2 ){ CargoContains = "HickoryLog"; }		CargoQty = (int)(CargoValue/5);	}
				else if ( rare <= 1028 ){ CargoMaterial = 5; Name = "crate of golden oak " + woods; Hue = CraftResources.GetHue( CraftResource.GoldenOakTree );		CargoContains = "GoldenOakBoard"; 	if ( logger == 2 ){ CargoContains = "GoldenOakLog"; }	CargoQty = (int)(CargoValue/5);	}
				else if ( rare <= 2048 ){ CargoMaterial = 4; Name = "crate of ebony " + woods; Hue = CraftResources.GetHue( CraftResource.EbonyTree );				CargoContains = "EbonyBoard"; 		if ( logger == 2 ){ CargoContains = "EbonyLog"; }		CargoQty = (int)(CargoValue/5);	}
				else if ( rare <= 4096 ){ CargoMaterial = 3; Name = "crate of cherry " + woods; Hue = CraftResources.GetHue( CraftResource.CherryTree );			CargoContains = "CherryBoard"; 		if ( logger == 2 ){ CargoContains = "CherryLog"; }		CargoQty = (int)(CargoValue/5);	}
				else if ( rare <= 8192 ){ CargoMaterial = 2; Name = "crate of ash " + woods; Hue = CraftResources.GetHue( CraftResource.AshTree );					CargoContains = "AshBoard"; 		if ( logger == 2 ){ CargoContains = "AshLog"; }			CargoQty = (int)(CargoValue/5);	}
				else { CargoMaterial = 1; ItemID = ordin; Name = "crate of " + woods;																				CargoContains = "Board"; 			if ( logger == 2 ){ CargoContains = "Log"; }			CargoQty = (int)(CargoValue/5);	}
			}
			else if ( CargoType == 49 )
			{
				CargoBox = 1;
				CargoVendor = "Blacksmith"; 
				ItemID = Utility.RandomList( 0x5419, 0x541C, 0x541D );
					if ( ItemID == 0x5419 ){ CargoBox = 2; }
				int rare = Utility.RandomMinMax( 1, 128 );

				if ( rare <= 1 ){ 			CargoMaterial = 7; Name = "crate of dinosaur scales"; 	Hue = CraftResources.GetHue( CraftResource.DinosaurScales ); 	CargoContains = "DinosaurScales"; 	CargoQty = (int)(CargoValue/36); }
				else if ( rare <= 4 ){ 		CargoMaterial = 6; Name = "crate of azure scales"; 		Hue = CraftResources.GetHue( CraftResource.BlueScales ); 		CargoContains = "BlueScales"; 		CargoQty = (int)(CargoValue/32); }
				else if ( rare <= 8 ){ 		CargoMaterial = 5; Name = "crate of ivory scales"; 		Hue = CraftResources.GetHue( CraftResource.WhiteScales ); 		CargoContains = "WhiteScales"; 		CargoQty = (int)(CargoValue/28); }
				else if ( rare <= 16 ){ 	CargoMaterial = 4; Name = "crate of viridian scales"; 	Hue = CraftResources.GetHue( CraftResource.GreenScales ); 		CargoContains = "GreenScales"; 		CargoQty = (int)(CargoValue/24); }
				else if ( rare <= 32 ){ 	CargoMaterial = 3; Name = "crate of dark scales"; 		Hue = CraftResources.GetHue( CraftResource.BlackScales ); 		CargoContains = "BlackScales"; 		CargoQty = (int)(CargoValue/20); }
				else if ( rare <= 64 ){ 	CargoMaterial = 2; Name = "crate of golden scales"; 	Hue = CraftResources.GetHue( CraftResource.YellowScales ); 		CargoContains = "YellowScales"; 	CargoQty = (int)(CargoValue/16); }
				else { 						CargoMaterial = 1; Name = "crate of crimson scales"; 	Hue = CraftResources.GetHue( CraftResource.RedScales ); 		CargoContains = "RedScales"; 		CargoQty = (int)(CargoValue/12); }
			}

			if ( ItemID >= 0x4F86 && ItemID <= 0x4F9E ){ Hue = 0xB61; }

			CargoTest( this );
		}

		public static void GiveCargo( Cargo cargo, Mobile vendor, Mobile player )
		{
			if ( VendorTest( cargo, vendor ) )
			{
				string say = "Thank you!";
				if ( cargo.CargoKarma < 0 )
				{
					switch( Utility.RandomMinMax( 1, 8 ) )
					{
						case 1: say = "Do I want to know where you got this?";	break;
						case 2: say = "This has some blood on it.";	break;
						case 3: say = "I thought this was pirated?";	break;
						case 4:	say = "I won't even ask.";	break;
						case 5: say = "Let's keep this between us.";	break;
						case 6: say = "Hurry, before someone sees us.";	break;
						case 7: say = "I'll stash this away for now.";	break;
						case 8: say = "Smuggled more goods did ya?";	break;
					}
					player.SendSound( 0x5B3 );
				}
				else
				{
					switch( Utility.RandomMinMax( 1, 8 ) )
					{
						case 1: say = "Thank you for returning this.";	break;
						case 2: say = "I hope the pirates paid with their lives.";	break;
						case 3: say = "This will surely save my shop.";	break;
						case 4:	say = "I thought this was lost forever.";	break;
						case 5: say = "I never thought I would get this back.";	break;
						case 6: say = "Did you know this was stolen from me?";	break;
						case 7: say = "Damn pirates stole this from me.";	break;
						case 8: say = "You make the shipping lanes safer for the rest of us.";	break;
					}
					player.SendSound( 0x5B4 );
				}

				int gold = CargoTotalValue( cargo, player );

				Server.Engines.Harvest.Fishing.SailorSkill( player, (int)( gold / 100 ) );

				player.SendMessage( "You receive " + gold + " gold." );
				player.AddToBackpack ( new Gold( gold ) );
				cargo.Delete();
				CustomEventSink.InvokeCombatQuestCompleted( player, gold );
				Misc.Titles.AwardKarma( player, cargo.CargoKarma, true );
				int fame = cargo.CargoKarma; if ( fame < 0 ){ fame = fame * -1; }
				Misc.Titles.AwardFame( player, fame, true );

				vendor.PrivateOverheadMessage(MessageType.Regular, 1153, false, say, player.NetState);
			}
			else
			{
				vendor.PrivateOverheadMessage(MessageType.Regular, 1153, false, "I think the " + cargo.CargoVendor + " might be interested in that.", player.NetState);
			}
		}

		public static bool VendorTest( Cargo cargo, Mobile vendor )
		{
			if ( cargo.CargoVendor == "Alchemist or Herbalist" && ( vendor is AlchemistGuildmaster || vendor is Alchemist || vendor is Herbalist ) ){ return true; }
			else if ( cargo.CargoVendor == "Art Collector" && vendor is VarietyDealer ){ return true; }
			else if ( cargo.CargoVendor == "Baker" && vendor is Baker ){ return true; } 
			else if ( cargo.CargoVendor == "Banker or Minter" && ( vendor is Banker || vendor is Minter ) ){ return true; }
			else if ( cargo.CargoVendor == "Bard" && ( vendor is BardGuildmaster || vendor is Bard ) ){ return true; } 
			else if ( cargo.CargoVendor == "Beekeeper" && vendor is Beekeeper ){ return true; }
			else if ( cargo.CargoVendor == "Blacksmith" && ( vendor is BlacksmithGuildmaster || vendor is Blacksmith ) ){ return true; }
			else if ( cargo.CargoVendor == "Bowyer" && vendor is Bowyer ){ return true; } 
			else if ( cargo.CargoVendor == "Butcher" && vendor is Butcher ){ return true; }
			else if ( cargo.CargoVendor == "Carpenter" && ( vendor is CarpenterGuildmaster || vendor is Carpenter ) ){ return true; } 
			else if ( cargo.CargoVendor == "Cook or Baker" && ( vendor is Cook || vendor is Baker ) ){ return true; }
			else if ( cargo.CargoVendor == "Cook" && vendor is Cook ){ return true; } 
			else if ( cargo.CargoVendor == "Farmer" && vendor is Farmer ){ return true; } 
			else if ( cargo.CargoVendor == "Healer" && ( vendor is HealerGuildmaster || vendor is Healer ) ){ return true; }
			else if ( cargo.CargoVendor == "Jeweler" && vendor is Jeweler ){ return true; }
			else if ( cargo.CargoVendor == "Leatherworker or Tanner" && ( vendor is Tanner || vendor is LeatherWorker ) ){ return true; } 
			else if ( cargo.CargoVendor == "Mage" && ( vendor is MageGuildmaster || vendor is Mage ) ){ return true; } 
			else if ( cargo.CargoVendor == "Mapmaker" && ( vendor is CartographersGuildmaster || vendor is Mapmaker ) ){ return true; }
			else if ( cargo.CargoVendor == "Necromancer or Witch" && ( vendor is NecromancerGuildmaster || vendor is Necromancer || vendor is Witches ) ){ return true; } 
			else if ( cargo.CargoVendor == "Provisioner" && vendor is Provisioner ){ return true; }
			else if ( cargo.CargoVendor == "Sage" && vendor is Sage ){ return true; }
			else if ( cargo.CargoVendor == "Scribe" && ( vendor is LibrarianGuildmaster || vendor is Scribe ) ){ return true; }
			else if ( cargo.CargoVendor == "Shipwright" && ( vendor is FisherGuildmaster || vendor is Shipwright ) ){ return true; } 
			else if ( cargo.CargoVendor == "Stable Worker" && ( vendor is Veterinarian || vendor is Rancher || vendor is AnimalTrainer ) ){ return true; }
			else if ( cargo.CargoVendor == "Tailor" && ( vendor is TailorGuildmaster || vendor is Tailor ) ){ return true; }
			else if ( cargo.CargoVendor == "Tavern or Bar Keeper" && ( vendor is Barkeeper || vendor is TavernKeeper ) ){ return true; } 
			else if ( cargo.CargoVendor == "Thief or Assassin" && ( vendor is AssassinGuildmaster || vendor is Thief || vendor is ThiefGuildmaster ) ){ return true; } 
			else if ( cargo.CargoVendor == "Tinker" && ( vendor is TinkerGuildmaster || vendor is Tinker ) ){ return true; }
			else if ( cargo.CargoVendor == "Weaponsmith or Armorer" && ( vendor is Armorer || vendor is Weaponsmith ) ){ return true; } 

			return false;
		}

		public static int CargoFishingGold( Cargo cargo, Mobile player ){ return (int)((cargo.CargoValue*(player.Skills[SkillName.Seafaring].Value * 0.01)/3)); }
		public static int CargoMerchantGold( Cargo cargo, Mobile player ){ return (int)((cargo.CargoValue*(player.Skills[SkillName.Mercantile].Value * 0.01)/3)); }
		public static int CargoBeggingGold( Cargo cargo, Mobile player ){ if ( BaseVendor.BeggingPose( player ) > 0 ){ return (int)((cargo.CargoValue*(player.Skills[SkillName.Begging].Value * 0.01)/3)); } return 0; }
		public static int CargoGuildGold( Cargo cargo, Mobile player ){ PlayerMobile pm = (PlayerMobile)player; if ( pm.NpcGuild == NpcGuild.FishermensGuild ){ return (int)(cargo.CargoValue*(25 * 0.01)); } return 0; }
		public static int CargoPortGold( Cargo cargo, Mobile player ){ if ( Server.Multis.BaseBoat.IsInPortTown( player ) ){ return (int)(cargo.CargoValue*(25 * 0.01)); } return 0; }

		public static int CargoTotalValue( Cargo cargo, Mobile player )
		{
			int gold = cargo.CargoValue;
				gold = gold + CargoFishingGold( cargo, player );
				gold = gold + CargoMerchantGold( cargo, player );
				gold = gold + CargoBeggingGold( cargo, player );
				gold = gold + CargoGuildGold( cargo, player );
				gold = gold + CargoPortGold( cargo, player );

			if ( BaseVendor.BeggingPose( player ) > 0 ){ Titles.AwardKarma( player, -BaseVendor.BeggingKarma( player ), false ); }

			return gold;
		}

		public static void CargoTest( Cargo cargo )
		{
			if ( cargo.CargoQty > 0 )
			{
				if ( cargo.CargoType != 33 && cargo.CargoType != 22 && cargo.CargoType != 35 )
				{
					Type itemType = ScriptCompiler.FindTypeByName( cargo.CargoContains );
					if ( itemType != null )
					{
						Item item = (Item)Activator.CreateInstance(itemType);
						item.Delete();
					}
				}
			}
		}
	}
}