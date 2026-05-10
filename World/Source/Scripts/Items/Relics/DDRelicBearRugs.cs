using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	public class DDRelicBearRugsAddon : BaseAddon
	{
		public int RelicGoldValue;
		public int RelicMainID;
		public int RelicRugID;
		public int RelicFound;
		public int RelicColor;
		public string RelicQuality;
		
		[CommandProperty(AccessLevel.Owner)]
		public int Relic_Value { get { return RelicGoldValue; } set { RelicGoldValue = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_Color { get { return RelicColor; } set { RelicColor = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_MainID { get { return RelicMainID; } set { RelicMainID = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_RugID { get { return RelicRugID; } set { RelicRugID = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_Found { get { return RelicFound; } set { RelicFound = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Relic_Quality { get { return RelicQuality; } set { RelicQuality = value; InvalidateProperties(); } }

		public override BaseAddonDeed Deed
		{
			get
			{
				return new DDRelicBearRugsAddonDeed( Relic_Value, Relic_MainID, Relic_RugID, Relic_Color, Relic_Quality, 1 );
			}
		}

		[Constructable]
		public DDRelicBearRugsAddon() : this( 0, 0, 0, 0, "blank" )
		{
		}

		[ Constructable ]
		public DDRelicBearRugsAddon( int RelCost, int RelID, int RelRugID, int RelHue, string RelQuality )
		{
			if ( RelRugID == 1 ) // EAST BLACK RUG
			{
				AddComplexComponent( (BaseAddon) this, 7744, 1, 1, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 1
				AddComplexComponent( (BaseAddon) this, 7745, 1, 0, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 2
				AddComplexComponent( (BaseAddon) this, 7746, 1, -1, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 3
				AddComplexComponent( (BaseAddon) this, 7747, 0, -1, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 4
				AddComplexComponent( (BaseAddon) this, 7748, 0, 0, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 5
				AddComplexComponent( (BaseAddon) this, 7749, 0, 1, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 6
				AddComplexComponent( (BaseAddon) this, 7750, -1, 1, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 7
				AddComplexComponent( (BaseAddon) this, 7751, -1, 0, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 8
				AddComplexComponent( (BaseAddon) this, 7752, -1, -1, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 9
			}
			else if ( RelRugID == 2 ) // SOUTH BLACK RUG
			{
				AddComplexComponent( (BaseAddon) this, 7734, 1, 1, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 1
				AddComplexComponent( (BaseAddon) this, 7735, 0, 1, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 2
				AddComplexComponent( (BaseAddon) this, 7736, -1, 1, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 3
				AddComplexComponent( (BaseAddon) this, 7737, -1, 0, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 4
				AddComplexComponent( (BaseAddon) this, 7738, 0, 0, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 5
				AddComplexComponent( (BaseAddon) this, 7739, 1, 0, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 6
				AddComplexComponent( (BaseAddon) this, 7740, 1, -1, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 7
				AddComplexComponent( (BaseAddon) this, 7741, 0, -1, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 8
				AddComplexComponent( (BaseAddon) this, 7742, -1, -1, 0, 1899, -1, RelQuality + " bearskin rug", 1);// 9
			}
			else if ( RelRugID == 3 ) // EAST BROWN RUG
			{
				AddComplexComponent( (BaseAddon) this, 7744, 1, 1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 1
				AddComplexComponent( (BaseAddon) this, 7745, 1, 0, 0, 0, -1, RelQuality + " bearskin rug", 1);// 2
				AddComplexComponent( (BaseAddon) this, 7746, 1, -1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 3
				AddComplexComponent( (BaseAddon) this, 7747, 0, -1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 4
				AddComplexComponent( (BaseAddon) this, 7748, 0, 0, 0, 0, -1, RelQuality + " bearskin rug", 1);// 5
				AddComplexComponent( (BaseAddon) this, 7749, 0, 1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 6
				AddComplexComponent( (BaseAddon) this, 7750, -1, 1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 7
				AddComplexComponent( (BaseAddon) this, 7751, -1, 0, 0, 0, -1, RelQuality + " bearskin rug", 1);// 8
				AddComplexComponent( (BaseAddon) this, 7752, -1, -1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 9
			}
			else if ( RelRugID == 4 ) // SOUTH BROWN RUG
			{
				AddComplexComponent( (BaseAddon) this, 7734, 1, 1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 1
				AddComplexComponent( (BaseAddon) this, 7735, 0, 1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 2
				AddComplexComponent( (BaseAddon) this, 7736, -1, 1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 3
				AddComplexComponent( (BaseAddon) this, 7737, -1, 0, 0, 0, -1, RelQuality + " bearskin rug", 1);// 4
				AddComplexComponent( (BaseAddon) this, 7738, 0, 0, 0, 0, -1, RelQuality + " bearskin rug", 1);// 5
				AddComplexComponent( (BaseAddon) this, 7739, 1, 0, 0, 0, -1, RelQuality + " bearskin rug", 1);// 6
				AddComplexComponent( (BaseAddon) this, 7740, 1, -1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 7
				AddComplexComponent( (BaseAddon) this, 7741, 0, -1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 8
				AddComplexComponent( (BaseAddon) this, 7742, -1, -1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 9
			}
			else if ( RelRugID == 5 ) // EAST WHITE RUG
			{
				AddComplexComponent( (BaseAddon) this, 7763, 1, 1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 1
				AddComplexComponent( (BaseAddon) this, 7764, 1, 0, 0, 0, -1, RelQuality + " bearskin rug", 1);// 2
				AddComplexComponent( (BaseAddon) this, 7765, 1, -1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 3
				AddComplexComponent( (BaseAddon) this, 7766, 0, -1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 4
				AddComplexComponent( (BaseAddon) this, 7767, 0, 0, 0, 0, -1, RelQuality + " bearskin rug", 1);// 5
				AddComplexComponent( (BaseAddon) this, 7768, 0, 1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 6
				AddComplexComponent( (BaseAddon) this, 7769, -1, 1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 7
				AddComplexComponent( (BaseAddon) this, 7770, -1, 0, 0, 0, -1, RelQuality + " bearskin rug", 1);// 8
				AddComplexComponent( (BaseAddon) this, 7771, -1, -1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 9
			}
			else // SOUTH WHITE RUG
			{
				AddComplexComponent( (BaseAddon) this, 7753, 1, 1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 1
				AddComplexComponent( (BaseAddon) this, 7754, 0, 1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 2
				AddComplexComponent( (BaseAddon) this, 7755, -1, 1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 3
				AddComplexComponent( (BaseAddon) this, 7756, -1, 0, 0, 0, -1, RelQuality + " bearskin rug", 1);// 4
				AddComplexComponent( (BaseAddon) this, 7757, 0, 0, 0, 0, -1, RelQuality + " bearskin rug", 1);// 5
				AddComplexComponent( (BaseAddon) this, 7758, 1, 0, 0, 0, -1, RelQuality + " bearskin rug", 1);// 6
				AddComplexComponent( (BaseAddon) this, 7759, 1, -1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 7
				AddComplexComponent( (BaseAddon) this, 7760, 0, -1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 8
				AddComplexComponent( (BaseAddon) this, 7761, -1, -1, 0, 0, -1, RelQuality + " bearskin rug", 1);// 9
			}

			RelicGoldValue = RelCost;
			RelicMainID = RelID;
			RelicRugID = RelRugID;
			RelicColor = RelHue;
			RelicQuality = RelQuality;
		}

		public DDRelicBearRugsAddon( Serial serial ) : base( serial )
		{
		}

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource)
        {
            AddComplexComponent(addon, item, xoffset, yoffset, zoffset, hue, lightsource, null, 1);
        }

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource, string name, int amount)
        {
            AddonComponent ac;
            ac = new AddonComponent(item);
            if (name != null && name.Length > 0)
                ac.Name = name;
            if (hue != 0)
                ac.Hue = hue;
            if (amount > 1)
            {
                ac.Stackable = true;
                ac.Amount = amount;
            }
            if (lightsource != -1)
                ac.Light = (LightType) lightsource;
            addon.AddComponent(ac, xoffset, yoffset, zoffset);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
            writer.Write( RelicGoldValue );
            writer.Write( RelicMainID );
            writer.Write( RelicRugID );
            writer.Write( RelicFound );
            writer.Write( RelicColor );
            writer.Write( RelicQuality );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
            RelicGoldValue = reader.ReadInt();
            RelicMainID = reader.ReadInt();
            RelicRugID = reader.ReadInt();
            RelicFound = reader.ReadInt();
            RelicColor = reader.ReadInt();
            RelicQuality = reader.ReadString();
		}
	}

	public class DDRelicBearRugsAddonDeed : BaseAddonDeed, IRelic
	{
		public override void ItemIdentified( bool id )
		{
			ColorHue3 = null;
			ColorText3 = null;
			m_NotIdentified = id;
			if ( !id )
			{
				ColorHue3 = "FDC844";
				ColorText3 = "Worth " + CoinPrice + " Gold";
			}
		}

		public override void ItemPriced( int val )
		{
			ColorHue3 = null;
			ColorText3 = null;
			if ( !m_NotIdentified )
			{
				ColorHue3 = "FDC844";
				ColorText3 = "Worth " + val + " Gold";
			}
			m_CoinPrice = val;
		}

		public int RelicMainID;
		public int RelicRugID;
		public int RelicFound;
		public int RelicColor;
		public string RelicQuality;

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_Color { get { return RelicColor; } set { RelicColor = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_MainID { get { return RelicMainID; } set { RelicMainID = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_RugID { get { return RelicRugID; } set { RelicRugID = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_Found { get { return RelicFound; } set { RelicFound = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Relic_Quality { get { return RelicQuality; } set { RelicQuality = value; InvalidateProperties(); } }

		public override BaseAddon Addon
		{
			get
			{
				return new DDRelicBearRugsAddon( CoinPrice, Relic_MainID, Relic_RugID, Hue, Relic_Quality );
			}
		}

		[Constructable]
		public DDRelicBearRugsAddonDeed() : this( 0, 0, 0, 0, "blank", 0 )
		{
		}

		[Constructable]
		public DDRelicBearRugsAddonDeed( int RelCost, int RelID, int RelRugID, int RelHue, string RelQuality, int RelRolled )
		{
			Weight = 30;

			if ( Relic_Found > 0 ) { RelicFound = 1; }

			/// QUALITY ////////////////
			string sLook = "";
			if ( Relic_Found > 0 ){ sLook = Relic_Quality; RelicQuality = Relic_Quality; }
			else if ( RelQuality != "blank" ){ sLook = RelQuality; RelicQuality = RelQuality; }
			else
			{
				sLook = "a rare";
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
				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0:	sLook = sLook + ", decorative";		break;
					case 1:	sLook = sLook + ", ceremonial";		break;
					case 2:	sLook = sLook + ", ornamental";		break;
				}
				RelicQuality = sLook;
			}

			/// VALUE ////////////////
			if ( Relic_Found > 0 ){}
			else if ( RelCost > 0 ){ CoinPrice = RelCost; }
			else { CoinPrice = Utility.RandomMinMax( 80, 500 ); NotIdentified = true; NotIDSource = Identity.Merchant; NotIDSkill = IDSkill.Mercantile; }

			/// ITEMID ////////////////
			if ( Relic_Found > 0 ){ RelicMainID = Relic_MainID; ItemID = Relic_MainID; RelicColor = Relic_Color; Hue = Relic_Color; RelicRugID = Relic_RugID; }
			else if ( RelID > 0 ){ RelicMainID = RelID; ItemID = RelID; RelicColor = RelHue; Hue = RelHue; RelicRugID = RelRugID; }
			else
			{
				switch ( Utility.RandomMinMax( 0, 5 ) ) 
				{
					case 0: ItemID = 0x1546; Hue = 1899; RelicRugID = 1; break;
					case 1: ItemID = 0x1545; Hue = 1899; RelicRugID = 2; break;
					case 2: ItemID = 0x1546; Hue = 0; RelicRugID = 3; break;
					case 3: ItemID = 0x1545; Hue = 0; RelicRugID = 4; break;
					case 4: ItemID = 0x1546; Hue = 1150; RelicRugID = 5; break;
					case 5: ItemID = 0x1545; Hue = 1150; RelicRugID = 6; break;
				}
				RelicMainID = ItemID;
				RelicColor = Hue;
			}

			Name = sLook + " bearskin rug";
			RelicFound = 1;
		}

		public DDRelicBearRugsAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) && MySettings.S_IdentifyItemsOnlyInPack && from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified ) 
				from.SendMessage( "This must be in your backpack to identify." );
			else if ( from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified )
				IDCommand( from );
			else
				base.OnDoubleClick( from );
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

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);

			if ( NotIdentified )
				list.Add( 1049644, "Appraise Before Placing in Your Home");
			else
				list.Add( 1049644, "Double Click To Place In Your Home");
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 1 ); // Version
            writer.Write( RelicMainID );
            writer.Write( RelicRugID );
            writer.Write( RelicFound );
            writer.Write( RelicColor );
            writer.Write( RelicQuality );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version < 1 )
				CoinPrice = reader.ReadInt();

            RelicMainID = reader.ReadInt();
            RelicRugID = reader.ReadInt();
            RelicFound = reader.ReadInt();
            RelicColor = reader.ReadInt();
            RelicQuality = reader.ReadString();
		}
	}
}