using System;
using Server.Network;
using Server.Gumps;

namespace Server.Items
{
	public class ManualOfItems : Item
	{
		public const int NUMBER_OF_ARTIFACTS = 290;

		[Constructable]
		public ManualOfItems() : base( 0x1C0E )
		{
			Weight = 5.0;
			Hue = Utility.RandomColor(0);
			ItemID = Utility.RandomList( 0x1C0E, 0x1C0F );
			Name = "Mystical Relic Chest";

			if ( m_Charges < 1 )
			{
				m_Charges = 1;
				m_Skill_1 = 0;
				m_Skill_2 = 0;
				m_Skill_3 = 0;
				m_Skill_4 = 0;
				m_Skill_5 = 0;
				m_Value_1 = 0.0;
				m_Value_2 = 0.0;
				m_Value_3 = 0.0;
				m_Value_4 = 0.0;
				m_Value_5 = 0.0;
				m_Slayer_1 = 0;
				m_Slayer_2 = 0;
				m_Owner = null;
				m_Extra = null;
				m_FromWho = "";
				m_HowGiven = "";
				m_Points = 100;
				m_Hue = 0;
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1060741, m_Charges.ToString() );
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			if ( m_FromWho != "" && m_FromWho != null ){ list.Add( 1070722, m_FromWho); }
			if ( m_Owner != null ){ list.Add( 1049644, "Belongs to " + m_Owner.Name + "" ); }
        }

		public override void OnDoubleClick( Mobile from )
		{
			bool CanOpen = false;

			if ( m_Owner == null ){ CanOpen = true; }
			else if ( m_Owner == from ){ CanOpen = true; }

			if ( CanOpen == true )
			{
				from.SendSound( 0x02D );
				from.CloseGump( typeof( RelicBoxGump ) );
				from.SendGump( new RelicBoxGump( from, this, 999999 ) );
			}
			else
			{
				from.SendMessage( "You cannot seem to get the chest to open. Is it yours?" );
			}
		}

		public class RelicBoxGump : Gump
		{
			public RelicBoxGump( Mobile from, ManualOfItems relicBox, int page ): base( 50, 50 )
			{
				string color = "#cfc990";

				m_Book = relicBox;
				ManualOfItems pedia = (ManualOfItems)relicBox;

				decimal PageCount = NUMBER_OF_ARTIFACTS / 16;
				int TotalBookPages = ( 100000 ) + ( (int)Math.Ceiling( PageCount ) );

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				int subItem = page * 16;

				int showItem1 = subItem + 1;
				int showItem2 = subItem + 2;
				int showItem3 = subItem + 3;
				int showItem4 = subItem + 4;
				int showItem5 = subItem + 5;
				int showItem6 = subItem + 6;
				int showItem7 = subItem + 7;
				int showItem8 = subItem + 8;
				int showItem9 = subItem + 9;
				int showItem10 = subItem + 10;
				int showItem11 = subItem + 11;
				int showItem12 = subItem + 12;
				int showItem13 = subItem + 13;
				int showItem14 = subItem + 14;
				int showItem15 = subItem + 15;
				int showItem16 = subItem + 16;

				int page_prev = ( 100000 + page ) - 1;
					if ( page_prev < 100000 ){ page_prev = TotalBookPages; }
				int page_next = ( 100000 + page ) + 1;
					if ( page_next > TotalBookPages ){ page_next = 100000; }

				AddImage(0, 0, 7055, Server.Misc.PlayerSettings.GetGumpHue( from ));

				AddButton(668, 9, 4017, 4017, page_prev, GumpButtonType.Reply, 0);

				AddHtml( 61, 12, 579, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>MAGICAL RELIC CHEST</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);

				if ( page == 999999 )
				{
					AddHtml( 13, 52, 681, 364, @"<BODY><BASEFONT Color=" + color + ">You have obtained a chest with powerful items of your choice. You are able to select as many items as the chest has charges. Once the charges are used up, the chest will vanish. When you make a selection, the item will appear in your pack. Some chests provide additional attributes to items such as slayer properties or skill enhancements. Each item will appear with a number of points you can spend to enhance your item. This allows you to tailor the item to suit your style. To begin, single click the items and select 'Enchant'. A menu will appear that you can choose which attributes you want the item to have. Be careful, as you cannot change an attribute once you select it.</BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(668, 425, 4005, 4005, 999998, GumpButtonType.Reply, 0);
				}
				else
				{
					AddButton(9, 425, 4014, 4014, page_prev, GumpButtonType.Reply, 0);
					AddButton(668, 425, 4005, 4005, page_next, GumpButtonType.Reply, 0);

					int x = 83;
					int y = 84;
					int s = 84;
					int z = 34;

					y=y+z;
					if ( GetRelicArtyForBook( showItem1, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem1, GumpButtonType.Reply, 0); } y=y+z;
					if ( GetRelicArtyForBook( showItem2, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem2, GumpButtonType.Reply, 0); } y=y+z;
					if ( GetRelicArtyForBook( showItem3, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem3, GumpButtonType.Reply, 0); } y=y+z;
					if ( GetRelicArtyForBook( showItem4, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem4, GumpButtonType.Reply, 0); } y=y+z;
					if ( GetRelicArtyForBook( showItem5, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem5, GumpButtonType.Reply, 0); } y=y+z;
					if ( GetRelicArtyForBook( showItem6, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem6, GumpButtonType.Reply, 0); } y=y+z;
					if ( GetRelicArtyForBook( showItem7, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem7, GumpButtonType.Reply, 0); } y=y+z;
					if ( GetRelicArtyForBook( showItem8, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem8, GumpButtonType.Reply, 0); } y=s-3;
					y=y+z;
					AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + GetRelicArtyForBook( showItem1, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
					AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + GetRelicArtyForBook( showItem2, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
					AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + GetRelicArtyForBook( showItem3, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
					AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + GetRelicArtyForBook( showItem4, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
					AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + GetRelicArtyForBook( showItem5, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
					AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + GetRelicArtyForBook( showItem6, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
					AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + GetRelicArtyForBook( showItem7, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
					AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + GetRelicArtyForBook( showItem8, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=s-3;

					///////////////////////////////////////////////////////////////////////////////////

					x = 375;
					y = s;

					y=y+z;
					if ( GetRelicArtyForBook( showItem9, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem9, GumpButtonType.Reply, 0); } y=y+z;
					if ( GetRelicArtyForBook( showItem10, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem10, GumpButtonType.Reply, 0); } y=y+z;
					if ( GetRelicArtyForBook( showItem11, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem11, GumpButtonType.Reply, 0); } y=y+z;
					if ( GetRelicArtyForBook( showItem12, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem12, GumpButtonType.Reply, 0); } y=y+z;
					if ( GetRelicArtyForBook( showItem13, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem13, GumpButtonType.Reply, 0); } y=y+z;
					if ( GetRelicArtyForBook( showItem14, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem14, GumpButtonType.Reply, 0); } y=y+z;
					if ( GetRelicArtyForBook( showItem15, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem15, GumpButtonType.Reply, 0); } y=y+z;
					if ( GetRelicArtyForBook( showItem16, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem16, GumpButtonType.Reply, 0); } y=s-3;
					y=y+z;
					AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + GetRelicArtyForBook( showItem9, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
					AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + GetRelicArtyForBook( showItem10, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
					AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + GetRelicArtyForBook( showItem11, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
					AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + GetRelicArtyForBook( showItem12, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
					AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + GetRelicArtyForBook( showItem13, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
					AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + GetRelicArtyForBook( showItem14, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
					AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + GetRelicArtyForBook( showItem15, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
					AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + GetRelicArtyForBook( showItem16, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=s-3;
				}
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				Mobile from = state.Mobile; 

				from.SendSound( 0x4A );

				if ( info.ButtonID == 999998 )
				{
					from.SendGump( new RelicBoxGump( from, m_Book, 0 ) );
				}
				else if ( info.ButtonID >= 100000 )
				{
					int page = info.ButtonID - 100000;
					from.SendGump( new RelicBoxGump( from, m_Book, page ) );
				}
				else if ( info.ButtonID > 0 && info.ButtonID < 500 )
				{
					string sType = GetRelicArtyForBook( info.ButtonID, 2 );
					string sName = GetRelicArtyForBook( info.ButtonID, 1 );
					string sArty = sName;
						if ( sArty == "Talisman, Holy" ){ sArty = "Talisman"; }
						if ( sArty == "Talisman, Snake" ){ sArty = "Talisman"; }
						if ( sArty == "Talisman, Totem" ){ sArty = "Talisman"; }
						if ( sArty == "Talisman, Talisman" ){ sArty = "Talisman"; }
						if ( m_Book.m_Extra != "" && m_Book.m_Extra != null ){ sArty = sArty + " " + m_Book.m_Extra; }

					if ( sName != "" )
					{
						Type itemType = ScriptCompiler.FindTypeByName( sType );
						var reward = (Item)Activator.CreateInstance(itemType);
						if ( reward is IGiftable )
						{
							var gift = (IGiftable)reward;
							gift.Owner = m_Book.m_Owner ?? from;
							gift.Gifter = m_Book.m_FromWho;
							gift.How = m_Book.m_HowGiven;
							gift.Points = m_Book.m_Points;
						}

						reward.Name = sArty;
						reward.Hue = m_Book.m_Hue;

						GiveItemBonus( reward, m_Book.m_Skill_1, m_Book.m_Skill_2, m_Book.m_Skill_3, m_Book.m_Skill_4, m_Book.m_Skill_5, m_Book.m_Value_1, m_Book.m_Value_2, m_Book.m_Value_3, m_Book.m_Value_4, m_Book.m_Value_5, m_Book.m_Slayer_1, m_Book.m_Slayer_2 );

						from.AddToBackpack ( reward );
						string sMessage = "You now have the " + sArty + ".";
						from.SendMessage( sMessage );
						from.PlaySound( 0x1FA );

						m_Book.m_Charges--;
						m_Book.InvalidateProperties();

						if ( m_Book.m_Charges < 1 )
						{
							m_Book.Delete();
						}
					}
				}
			}
		}

		public static string GetRelicArtyForBook( int artifact, int part )
		{
			string item = "";
			string name = "";
			int arty = 1;

			if ( artifact == arty) { name = typeof(GiftBascinet).Name; item="Bascinet"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBoneArms).Name; item="Bone Arms"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBoneChest).Name; item="Bone Chest"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBoneGloves).Name; item="Bone Gloves"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBoneHelm).Name; item="Bone Helm"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBoneLegs).Name; item="Bone Legs"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBuckler).Name; item="Buckler"; } arty++;
			if ( artifact == arty) { name = typeof(GiftChainChest).Name; item="Chain Chest"; } arty++;
			if ( artifact == arty) { name = typeof(GiftChainCoif).Name; item="Chain Coif"; } arty++;
			if ( artifact == arty) { name = typeof(GiftChainHatsuburi).Name; item="Chain Hatsuburi"; } arty++;
			if ( artifact == arty) { name = typeof(GiftChainLegs).Name; item="Chain Legs"; } arty++;
			if ( artifact == arty) { name = typeof(GiftChaosShield).Name; item="Chaos Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftCirclet).Name; item="Circlet"; } arty++;
			if ( artifact == arty) { name = typeof(GiftCloseHelm).Name; item="Close Helm"; } arty++;
			if ( artifact == arty) { name = typeof(GiftDarkShield).Name; item="Dark Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftDecorativePlateKabuto).Name; item="Decorative Plate Kabuto"; } arty++;
			if ( artifact == arty) { name = typeof(GiftDreadHelm).Name; item="Dread Helm"; } arty++;
			if ( artifact == arty) { name = typeof(GiftElvenShield).Name; item="Elven Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftFemaleLeatherChest).Name; item="Female Leather Chest"; } arty++;
			if ( artifact == arty) { name = typeof(GiftFemalePlateChest).Name; item="Female Plate Chest"; } arty++;
			if ( artifact == arty) { name = typeof(GiftFemaleStuddedChest).Name; item="Female Studded Chest"; } arty++;
			if ( artifact == arty) { name = typeof(GiftGuardsmanShield).Name; item="Guardsman Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftHeaterShield).Name; item="Heater Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftHeavyPlateJingasa).Name; item="Heavy Plate Jingasa"; } arty++;
			if ( artifact == arty) { name = typeof(GiftHelmet).Name; item="Helmet"; } arty++;
			if ( artifact == arty) { name = typeof(GiftOrcHelm).Name; item="Horned Helm"; } arty++;
			if ( artifact == arty) { name = typeof(GiftJeweledShield).Name; item="Jeweled Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBronzeShield).Name; item="Large Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherArms).Name; item="Leather Arms"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherBustierArms).Name; item="Leather Bustier Arms"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherCap).Name; item="Leather Cap"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherChest).Name; item="Leather Chest"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherCloak).Name; item="Leather Cloak"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherDo).Name; item="Leather Do"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherGloves).Name; item="Leather Gloves"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherGorget).Name; item="Leather Gorget"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherHaidate).Name; item="Leather Haidate"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherHiroSode).Name; item="Leather HiroSode"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherJingasa).Name; item="Leather Jingasa"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherLegs).Name; item="Leather Legs"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherMempo).Name; item="Leather Mempo"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherNinjaHood).Name; item="Leather Ninja Hood"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherNinjaJacket).Name; item="Leather Ninja Jacket"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherNinjaMitts).Name; item="Leather Ninja Mitts"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherNinjaPants).Name; item="Leather Ninja Pants"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherRobe).Name; item="Leather Robe"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherShorts).Name; item="Leather Shorts"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherSkirt).Name; item="Leather Skirt"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeatherSuneate).Name; item="Leather Suneate"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLightPlateJingasa).Name; item="Light Plate Jingasa"; } arty++;
			if ( artifact == arty) { name = typeof(GiftMetalKiteShield).Name; item="Metal Kite Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftMetalShield).Name; item="Metal Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftNorseHelm).Name; item="Norse Helm"; } arty++;
			if ( artifact == arty) { name = typeof(GiftOniwabanBoots).Name; item="Oniwaban Boots"; } arty++;
			if ( artifact == arty) { name = typeof(GiftOniwabanGloves).Name; item="Oniwaban Gloves"; } arty++;
			if ( artifact == arty) { name = typeof(GiftOniwabanHood).Name; item="Oniwaban Hood"; } arty++;
			if ( artifact == arty) { name = typeof(GiftOniwabanLeggings).Name; item="Oniwaban Leggings"; } arty++;
			if ( artifact == arty) { name = typeof(GiftOniwabanTunic).Name; item="Oniwaban Tunic"; } arty++;
			if ( artifact == arty) { name = typeof(GiftOrderShield).Name; item="Order Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPlateArms).Name; item="Plate Arms"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPlateBattleKabuto).Name; item="Plate Battle Kabuto"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPlateChest).Name; item="Plate Chest"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPlateDo).Name; item="Plate Do"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPlateGloves).Name; item="Plate Gloves"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPlateGorget).Name; item="Plate Gorget"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPlateHaidate).Name; item="Plate Haidate"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPlateHatsuburi).Name; item="Plate Hatsuburi"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPlateHelm).Name; item="Plate Helm"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPlateHiroSode).Name; item="Plate Hiro Sode"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPlateLegs).Name; item="Plate Legs"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPlateMempo).Name; item="Plate Mempo"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPlateSuneate).Name; item="Plate Suneate"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRingmailArms).Name; item="Ringmail Arms"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRingmailChest).Name; item="Ringmail Chest"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRingmailGloves).Name; item="Ringmail Gloves"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRingmailLegs).Name; item="Ringmail Legs"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRoyalArms).Name; item="Royal Arms"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRoyalBoots).Name; item="Royal Boots"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRoyalChest).Name; item="Royal Chest"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRoyalGloves).Name; item="Royal Gloves"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRoyalGorget).Name; item="Royal Gorget"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRoyalHelm).Name; item="Royal Helm"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRoyalsLegs).Name; item="Royal Legs"; } arty++;
			if ( artifact == arty) { name = typeof(GiftDragonArms).Name; item="Scalemail Arms"; } arty++;
			if ( artifact == arty) { name = typeof(GiftDragonGloves).Name; item="Scalemail Gloves"; } arty++;
			if ( artifact == arty) { name = typeof(GiftDragonHelm).Name; item="Scalemail Helm"; } arty++;
			if ( artifact == arty) { name = typeof(GiftDragonLegs).Name; item="Scalemail Leggings"; } arty++;
			if ( artifact == arty) { name = typeof(GiftScalemailShield).Name; item="Scalemail Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftDragonChest).Name; item="Scalemail Tunic"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRoyalShield).Name; item="Royal Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftShinobiCowl).Name; item="Leather Shinobi Cowl"; } arty++;
			if ( artifact == arty) { name = typeof(GiftShinobiHood).Name; item="Leather Shinobi Hood"; } arty++;
			if ( artifact == arty) { name = typeof(GiftShinobiMask).Name; item="Leather Shinobi Mask"; } arty++;
			if ( artifact == arty) { name = typeof(GiftShinobiRobe).Name; item="Leather Shinobi Robe"; } arty++;
			if ( artifact == arty) { name = typeof(GiftSmallPlateJingasa).Name; item="Small Plate Jingasa"; } arty++;
			if ( artifact == arty) { name = typeof(GiftStandardPlateKabuto).Name; item="Standard Plate Kabuto"; } arty++;
			if ( artifact == arty) { name = typeof(GiftStuddedArms).Name; item="Studded Arms"; } arty++;
			if ( artifact == arty) { name = typeof(GiftStuddedBustierArms).Name; item="Studded Bustier Arms"; } arty++;
			if ( artifact == arty) { name = typeof(GiftStuddedChest).Name; item="Studded Chest"; } arty++;
			if ( artifact == arty) { name = typeof(GiftStuddedDo).Name; item="Studded Do"; } arty++;
			if ( artifact == arty) { name = typeof(GiftStuddedGloves).Name; item="Studded Gloves"; } arty++;
			if ( artifact == arty) { name = typeof(GiftStuddedGorget).Name; item="Studded Gorget"; } arty++;
			if ( artifact == arty) { name = typeof(GiftStuddedHaidate).Name; item="Studded Haidate"; } arty++;
			if ( artifact == arty) { name = typeof(GiftStuddedHiroSode).Name; item="Studded Hiro Sode"; } arty++;
			if ( artifact == arty) { name = typeof(GiftStuddedLegs).Name; item="Studded Legs"; } arty++;
			if ( artifact == arty) { name = typeof(GiftStuddedMempo).Name; item="Studded Mempo"; } arty++;
			if ( artifact == arty) { name = typeof(GiftStuddedSuneate).Name; item="Studded Suneate"; } arty++;
			if ( artifact == arty) { name = typeof(GiftSunShield).Name; item="Sun Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftVirtueShield).Name; item="Virtue Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWoodenKiteShield).Name; item="Wooden Kite Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWoodenPlateArms).Name; item="Wooden Plate Arms"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWoodenPlateChest).Name; item="Wooden Plate Chest"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWoodenPlateGloves).Name; item="Wooden Plate Gloves"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWoodenPlateGorget).Name; item="Wooden Plate Gorget"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWoodenPlateHelm).Name; item="Wooden Plate Helm"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWoodenPlateLegs).Name; item="Wooden Plate Legs"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWoodenShield).Name; item="Wooden Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftAssassinSpike).Name; item="Assassin Dagger"; } arty++;
			if ( artifact == arty) { name = typeof(GiftElvenSpellblade).Name; item="Assassin Sword"; } arty++;
			if ( artifact == arty) { name = typeof(GiftAxe).Name; item="Axe"; } arty++;
			if ( artifact == arty) { name = typeof(GiftOrnateAxe).Name; item="Barbarian Axe"; } arty++;
			if ( artifact == arty) { name = typeof(GiftVikingSword).Name; item="Barbarian Sword"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBardiche).Name; item="Bardiche"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBattleAxe).Name; item="Battle Axe"; } arty++;
			if ( artifact == arty) { name = typeof(GiftDiamondMace).Name; item="Battle Mace"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBladedStaff).Name; item="Bladed Staff"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBokuto).Name; item="Bokuto"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBow).Name; item="Bow"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBroadsword).Name; item="Broadsword"; } arty++;
			if ( artifact == arty) { name = typeof(GiftButcherKnife).Name; item="Butcher Knife"; } arty++;
			if ( artifact == arty) { name = typeof(GiftChampionShield).Name; item="Champion Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftClaymore).Name; item="Claymore"; } arty++;
			if ( artifact == arty) { name = typeof(GiftCleaver).Name; item="Cleaver"; } arty++;
			if ( artifact == arty) { name = typeof(GiftClub).Name; item="Club"; } arty++;
			if ( artifact == arty) { name = typeof(GiftCompositeBow).Name; item="Composite Bow"; } arty++;
			if ( artifact == arty) { name = typeof(GiftCrescentBlade).Name; item="Crescent Blade"; } arty++;
			if ( artifact == arty) { name = typeof(GiftCrestedShield).Name; item="Crested Shield"; } arty++;
			if ( artifact == arty) { name = typeof(GiftCrossbow).Name; item="Crossbow"; } arty++;
			if ( artifact == arty) { name = typeof(GiftCutlass).Name; item="Cutlass"; } arty++;
			if ( artifact == arty) { name = typeof(GiftDagger).Name; item="Dagger"; } arty++;
			if ( artifact == arty) { name = typeof(GiftDaisho).Name; item="Daisho"; } arty++;
			if ( artifact == arty) { name = typeof(GiftDoubleAxe).Name; item="Double Axe"; } arty++;
			if ( artifact == arty) { name = typeof(GiftDoubleBladedStaff).Name; item="Double Bladed Staff"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWildStaff).Name; item="Druid Staff"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRadiantScimitar).Name; item="Falchion"; } arty++;
			if ( artifact == arty) { name = typeof(GiftGnarledStaff).Name; item="Gnarled Staff"; } arty++;
			if ( artifact == arty) { name = typeof(GiftExecutionersAxe).Name; item="Great Axe"; } arty++;
			if ( artifact == arty) { name = typeof(GiftHalberd).Name; item="Halberd"; } arty++;
			if ( artifact == arty) { name = typeof(GiftHammers).Name; item="Hammer"; } arty++;
			if ( artifact == arty) { name = typeof(GiftHammerPick).Name; item="Hammer Pick"; } arty++;
			if ( artifact == arty) { name = typeof(GiftHarpoon).Name; item="Harpoon"; } arty++;
			if ( artifact == arty) { name = typeof(GiftHatchet).Name; item="Hatchet"; } arty++;
			if ( artifact == arty) { name = typeof(GiftHeavyCrossbow).Name; item="Heavy Crossbow"; } arty++;
			if ( artifact == arty) { name = typeof(GiftKama).Name; item="Kama"; } arty++;
			if ( artifact == arty) { name = typeof(GiftKatana).Name; item="Katana"; } arty++;
			if ( artifact == arty) { name = typeof(GiftKryss).Name; item="Kryss"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLajatang).Name; item="Lajatang"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLance).Name; item="Lance"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLargeBattleAxe).Name; item="Large Battle Axe"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLargeKnife).Name; item="Large Knife"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLongsword).Name; item="Longsword"; } arty++;
			if ( artifact == arty) { name = typeof(GiftMace).Name; item="Mace"; } arty++;
			if ( artifact == arty) { name = typeof(GiftElvenMachete).Name; item="Machete"; } arty++;
			if ( artifact == arty) { name = typeof(GiftMaul).Name; item="Maul"; } arty++;
			if ( artifact == arty) { name = typeof(GiftNoDachi).Name; item="NoDachi"; } arty++;
			if ( artifact == arty) { name = typeof(GiftNunchaku).Name; item="Nunchaku"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPickaxe).Name; item="Pickaxe"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPike).Name; item="Pike"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPugilistGloves).Name; item="Pugilist Gloves"; } arty++;
			if ( artifact == arty) { name = typeof(GiftQuarterStaff).Name; item="Quarter Staff"; } arty++;
			if ( artifact == arty) { name = typeof(GiftShortSpear).Name; item="Rapier"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRepeatingCrossbow).Name; item="Repeating Crossbow"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRoyalSword).Name; item="Royal Sword"; } arty++;
			if ( artifact == arty) { name = typeof(GiftSai).Name; item="Sai"; } arty++;
			if ( artifact == arty) { name = typeof(GiftScepter).Name; item="Scepter"; } arty++;
			if ( artifact == arty) { name = typeof(GiftSceptre).Name; item="Sceptre"; } arty++;
			if ( artifact == arty) { name = typeof(GiftScimitar).Name; item="Scimitar"; } arty++;
			if ( artifact == arty) { name = typeof(GiftScythe).Name; item="Scythe"; } arty++;
			if ( artifact == arty) { name = typeof(GiftShepherdsCrook).Name; item="Shepherds Crook"; } arty++;
			if ( artifact == arty) { name = typeof(GiftShortSword).Name; item="Short Sword"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBoneHarvester).Name; item="Sickle"; } arty++;
			if ( artifact == arty) { name = typeof(GiftSpear).Name; item="Spear"; } arty++;
			if ( artifact == arty) { name = typeof(GiftSpikedClub).Name; item="Spiked Club"; } arty++;
			if ( artifact == arty) { name = typeof(GiftStave).Name; item="Stave"; } arty++;
			if ( artifact == arty) { name = typeof(GiftThinLongsword).Name; item="Sword"; } arty++;
			if ( artifact == arty) { name = typeof(GiftTekagi).Name; item="Tekagi"; } arty++;
			if ( artifact == arty) { name = typeof(GiftTessen).Name; item="Tessen"; } arty++;
			if ( artifact == arty) { name = typeof(GiftTetsubo).Name; item="Tetsubo"; } arty++;
			if ( artifact == arty) { name = typeof(GiftThrowingGloves).Name; item="Throwing Gloves"; } arty++;
			if ( artifact == arty) { name = typeof(GiftTribalSpear).Name; item="Tribal Spear"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPitchfork).Name; item="Trident"; } arty++;
			if ( artifact == arty) { name = typeof(GiftTwoHandedAxe).Name; item="Two Handed Axe"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWakizashi).Name; item="Wakizashi"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWarAxe).Name; item="War Axe"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRuneBlade).Name; item="War Blades"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWarCleaver).Name; item="War Cleaver"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLeafblade).Name; item="War Dagger"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWarFork).Name; item="War Fork"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWarHammer).Name; item="War Hammer"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWarMace).Name; item="War Mace"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWhips).Name; item="Whip"; } arty++;
			if ( artifact == arty) { name = typeof(GiftElvenCompositeLongbow).Name; item="Woodland Longbow"; } arty++;
			if ( artifact == arty) { name = typeof(GiftMagicalShortbow).Name; item="Woodland Shortbow"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBlackStaff).Name; item="Wizard Staff"; } arty++;
			if ( artifact == arty) { name = typeof(GiftYumi).Name; item="Yumi"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBandana).Name; item="Bandana"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBearMask).Name; item="Bear Mask"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBelt).Name; item="Belt"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBodySash).Name; item="Body Sash"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBonnet).Name; item="Bonnet"; } arty++;
			if ( artifact == arty) { name = typeof(GiftBoots).Name; item="Boots"; } arty++;
			if ( artifact == arty) { name = typeof(GiftCap).Name; item="Cap"; } arty++;
			if ( artifact == arty) { name = typeof(GiftCloak).Name; item="Cloak"; } arty++;
			if ( artifact == arty) { name = typeof(GiftClothNinjaHood).Name; item="Cloth Ninja Hood"; } arty++;
			if ( artifact == arty) { name = typeof(GiftClothNinjaJacket).Name; item="Cloth Ninja Jacket"; } arty++;
			if ( artifact == arty) { name = typeof(GiftCowl).Name; item="Cowl"; } arty++;
			if ( artifact == arty) { name = typeof(GiftDeerMask).Name; item="Deer Mask"; } arty++;
			if ( artifact == arty) { name = typeof(GiftDoublet).Name; item="Doublet"; } arty++;
			if ( artifact == arty) { name = typeof(GiftElvenBoots).Name; item="Fancy Boots"; } arty++;
			if ( artifact == arty) { name = typeof(GiftFancyDress).Name; item="Fancy Dress"; } arty++;
			if ( artifact == arty) { name = typeof(GiftFancyShirt).Name; item="Fancy Shirt"; } arty++;
			if ( artifact == arty) { name = typeof(GiftFeatheredHat).Name; item="Feathered Hat"; } arty++;
			if ( artifact == arty) { name = typeof(GiftFemaleKimono).Name; item="Female Kimono"; } arty++;
			if ( artifact == arty) { name = typeof(GiftFloppyHat).Name; item="Floppy Hat"; } arty++;
			if ( artifact == arty) { name = typeof(GiftFormalShirt).Name; item="Formal Shirt"; } arty++;
			if ( artifact == arty) { name = typeof(GiftFullApron).Name; item="Full Apron"; } arty++;
			if ( artifact == arty) { name = typeof(GiftFurBoots).Name; item="Fur Boots"; } arty++;
			if ( artifact == arty) { name = typeof(GiftFurCape).Name; item="Fur Cape"; } arty++;
			if ( artifact == arty) { name = typeof(GiftFurSarong).Name; item="Fur Sarong"; } arty++;
			if ( artifact == arty) { name = typeof(GiftGildedDress).Name; item="Gilded Dress"; } arty++;
			if ( artifact == arty) { name = typeof(GiftHakama).Name; item="Hakama"; } arty++;
			if ( artifact == arty) { name = typeof(GiftHakamaShita).Name; item="Hakama Shita"; } arty++;
			if ( artifact == arty) { name = typeof(GiftHalfApron).Name; item="Half Apron"; } arty++;
			if ( artifact == arty) { name = typeof(GiftHood).Name; item="Hood"; } arty++;
			if ( artifact == arty) { name = typeof(GiftHornedTribalMask).Name; item="Horned Tribal Mask"; } arty++;
			if ( artifact == arty) { name = typeof(GiftJesterHat).Name; item="Jester Hat"; } arty++;
			if ( artifact == arty) { name = typeof(GiftJesterSuit).Name; item="Jester Suit"; } arty++;
			if ( artifact == arty) { name = typeof(GiftJinBaori).Name; item="Jin Baori"; } arty++;
			if ( artifact == arty) { name = typeof(GiftKamishimo).Name; item="Kamishimo"; } arty++;
			if ( artifact == arty) { name = typeof(GiftKasa).Name; item="Kasa"; } arty++;
			if ( artifact == arty) { name = typeof(GiftKilt).Name; item="Kilt"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLoinCloth).Name; item="Loin Cloth"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLongPants).Name; item="Long Pants"; } arty++;
			if ( artifact == arty) { name = typeof(GiftMaleKimono).Name; item="Male Kimono"; } arty++;
			if ( artifact == arty) { name = typeof(GiftNinjaTabi).Name; item="Ninja Tabi"; } arty++;
			if ( artifact == arty) { name = typeof(GiftObi).Name; item="Obi"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPlainDress).Name; item="Plain Dress"; } arty++;
			if ( artifact == arty) { name = typeof(GiftPirateHat).Name; item="Pirate Hat"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRobe).Name; item="Robe"; } arty++;
			if ( artifact == arty) { name = typeof(GiftRoyalCape).Name; item="Royal Cape"; } arty++;
			if ( artifact == arty) { name = typeof(GiftSamuraiTabi).Name; item="Samurai Tabi"; } arty++;
			if ( artifact == arty) { name = typeof(GiftSandals).Name; item="Sandals"; } arty++;
			if ( artifact == arty) { name = typeof(GiftSash).Name; item="Sash"; } arty++;
			if ( artifact == arty) { name = typeof(GiftShirt).Name; item="Shirt"; } arty++;
			if ( artifact == arty) { name = typeof(GiftShoes).Name; item="Shoes"; } arty++;
			if ( artifact == arty) { name = typeof(GiftShortPants).Name; item="Short Pants"; } arty++;
			if ( artifact == arty) { name = typeof(GiftSkirt).Name; item="Skirt"; } arty++;
			if ( artifact == arty) { name = typeof(GiftSkullCap).Name; item="Skull Cap"; } arty++;
			if ( artifact == arty) { name = typeof(GiftStrawHat).Name; item="Straw Hat"; } arty++;
			if ( artifact == arty) { name = typeof(GiftSurcoat).Name; item="Surcoat"; } arty++;
			if ( artifact == arty) { name = typeof(GiftTallStrawHat).Name; item="Tall Straw Hat"; } arty++;
			if ( artifact == arty) { name = typeof(GiftTattsukeHakama).Name; item="Tattsuke Hakama"; } arty++;
			if ( artifact == arty) { name = typeof(GiftThighBoots).Name; item="Thigh Boots"; } arty++;
			if ( artifact == arty) { name = typeof(GiftTribalMask).Name; item="Tribal Mask"; } arty++;
			if ( artifact == arty) { name = typeof(GiftTricorneHat).Name; item="Tricorne Hat"; } arty++;
			if ( artifact == arty) { name = typeof(GiftTunic).Name; item="Tunic"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWaraji).Name; item="Waraji"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWideBrimHat).Name; item="Wide Brim Hat"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWitchHat).Name; item="Witch Hat"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWizardsHat).Name; item="Wizards Hat"; } arty++;
			if ( artifact == arty) { name = typeof(GiftWolfMask).Name; item="Wolf Mask"; } arty++;
			if ( artifact == arty) { name = typeof(GiftCandle).Name; item="Candle"; } arty++;
			if ( artifact == arty) { name = typeof(GiftGoldBeadNecklace).Name; item="Bead Necklace"; } arty++;
			if ( artifact == arty) { name = typeof(GiftGoldBracelet).Name; item="Gold Bracelet"; } arty++;
			if ( artifact == arty) { name = typeof(GiftGoldEarrings).Name; item="Gold Earrings"; } arty++;
			if ( artifact == arty) { name = typeof(GiftGoldNecklace).Name; item="Gold Amulet"; } arty++;
			if ( artifact == arty) { name = typeof(GiftGoldRing).Name; item="Gold Ring"; } arty++;
			if ( artifact == arty) { name = typeof(GiftLantern).Name; item="Lantern"; } arty++;
			if ( artifact == arty) { name = typeof(GiftNecklace).Name; item="Amulet"; } arty++;
			if ( artifact == arty) { name = typeof(GiftSilverBeadNecklace).Name; item="Silver Bead Necklace"; } arty++;
			if ( artifact == arty) { name = typeof(GiftSilverBracelet).Name; item="Silver Bracelet"; } arty++;
			if ( artifact == arty) { name = typeof(GiftSilverEarrings).Name; item="Silver Earrings"; } arty++;
			if ( artifact == arty) { name = typeof(GiftSilverNecklace).Name; item="Silver Amulet"; } arty++;
			if ( artifact == arty) { name = typeof(GiftSilverRing).Name; item="Silver Ring"; } arty++;
			if ( artifact == arty) { name = typeof(GiftTalismanLeather).Name; item="Trinket, Talisman"; } arty++;
			if ( artifact == arty) { name = typeof(GiftTalismanHoly).Name; item="Trinket, Symbol"; } arty++;
			if ( artifact == arty) { name = typeof(GiftTalismanSnake).Name; item="Trinket, Idol"; } arty++;
			if ( artifact == arty) { name = typeof(GiftTalismanTotem).Name; item="Trinket, Totem"; } arty++;
			if ( artifact == arty) { name = typeof(GiftTorch).Name; item="Torch"; } arty++;

			if ( part == 2 ){ item = name; }

			return item;
		}

		public static void GiveItemBonus( Item item, int val1, int val2, int val3, int val4, int val5, double sk1, double sk2, double sk3, double sk4, double sk5, int slay1, int slay2 )
		{
			if ( item is BaseWeapon )
			{
				if ( slay1 > 0 ){ ((BaseWeapon)item).Slayer = ResourceMods.GetSlayer( slay1 ); }
				if ( slay2 > 0 ){ ((BaseWeapon)item).Slayer2 = ResourceMods.GetSlayer( slay2 ); }

				if ( val1 == 99 ){ ((BaseWeapon)item).SkillBonuses.SetValues(0, ((BaseWeapon)item).Skill, sk1); }
				else if ( val1 > 0 ){ ((BaseWeapon)item).SkillBonuses.SetValues(0, ResourceMods.GetSkill( val1 ), sk1); }
				if ( val2 > 0 ){ ((BaseWeapon)item).SkillBonuses.SetValues(1, ResourceMods.GetSkill( val2 ), sk2); }
				if ( val3 > 0 ){ ((BaseWeapon)item).SkillBonuses.SetValues(2, ResourceMods.GetSkill( val3 ), sk3); }
				if ( val4 > 0 ){ ((BaseWeapon)item).SkillBonuses.SetValues(3, ResourceMods.GetSkill( val4 ), sk4); }
				if ( val5 == 100 ){ ((BaseWeapon)item).Attributes.EnhancePotions = (int)sk5;  }
				else if ( val5 > 0 ){ ((BaseWeapon)item).SkillBonuses.SetValues(4, ResourceMods.GetSkill( val5 ), sk5); }
			}
			else if ( item is BaseArmor )
			{
				if ( val1 == 99 && item is BaseShield ){ ((BaseShield)item).SkillBonuses.SetValues(0, SkillName.Parry, sk1); }
				else if ( val1 > 0 ){ ((BaseArmor)item).SkillBonuses.SetValues(0, ResourceMods.GetSkill( val1 ), sk1); }
				if ( val2 > 0 ){ ((BaseArmor)item).SkillBonuses.SetValues(1, ResourceMods.GetSkill( val2 ), sk2); }
				if ( val3 > 0 ){ ((BaseArmor)item).SkillBonuses.SetValues(2, ResourceMods.GetSkill( val3 ), sk3); }
				if ( val4 > 0 ){ ((BaseArmor)item).SkillBonuses.SetValues(3, ResourceMods.GetSkill( val4 ), sk4); }
				if ( val5 == 100 ){ ((BaseArmor)item).Attributes.EnhancePotions = (int)sk5; }
				else if ( val5 > 0 ){ ((BaseArmor)item).SkillBonuses.SetValues(4, ResourceMods.GetSkill( val5 ), sk5); }
			}
			else if ( item is BaseClothing )
			{
				if ( val1 == 99 ){}
				else if ( val1 > 0 ){ ((BaseClothing)item).SkillBonuses.SetValues(0, ResourceMods.GetSkill( val1 ), sk1); }
				if ( val2 > 0 ){ ((BaseClothing)item).SkillBonuses.SetValues(1, ResourceMods.GetSkill( val2 ), sk2); }
				if ( val3 > 0 ){ ((BaseClothing)item).SkillBonuses.SetValues(2, ResourceMods.GetSkill( val3 ), sk3); }
				if ( val4 > 0 ){ ((BaseClothing)item).SkillBonuses.SetValues(3, ResourceMods.GetSkill( val4 ), sk4); }
				if ( val5 == 100 ){ ((BaseClothing)item).Attributes.EnhancePotions = (int)sk5; }
				else if ( val5 > 0 ){ ((BaseClothing)item).SkillBonuses.SetValues(4, ResourceMods.GetSkill( val5 ), sk5); }
			}
			else if ( item is BaseTrinket )
			{
				if ( val1 == 99 ){}
				else if ( val1 > 0 ){ ((BaseTrinket)item).SkillBonuses.SetValues(0, ResourceMods.GetSkill( val1 ), sk1); }
				if ( val2 > 0 ){ ((BaseTrinket)item).SkillBonuses.SetValues(1, ResourceMods.GetSkill( val2 ), sk2); }
				if ( val3 > 0 ){ ((BaseTrinket)item).SkillBonuses.SetValues(2, ResourceMods.GetSkill( val3 ), sk3); }
				if ( val4 > 0 ){ ((BaseTrinket)item).SkillBonuses.SetValues(3, ResourceMods.GetSkill( val4 ), sk4); }
				if ( val5 == 100 ){ ((BaseTrinket)item).Attributes.EnhancePotions = (int)sk5; }
				else if ( val5 > 0 ){ ((BaseTrinket)item).SkillBonuses.SetValues(4, ResourceMods.GetSkill( val5 ), sk5); }
			}
		}

		public static ManualOfItems m_Book;

		public int m_Charges;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges { get{ return m_Charges; } set{ m_Charges = value; InvalidateProperties(); } }

		public int m_Skill_1;
		[CommandProperty( AccessLevel.GameMaster )]
		public int mSkill1 { get{ return m_Skill_1; } set{ m_Skill_1 = value; InvalidateProperties(); } }

		public int m_Skill_2;
		[CommandProperty( AccessLevel.GameMaster )]
		public int mSkill2 { get{ return m_Skill_2; } set{ m_Skill_2 = value; InvalidateProperties(); } }

		public int m_Skill_3;
		[CommandProperty( AccessLevel.GameMaster )]
		public int mSkill3 { get{ return m_Skill_3; } set{ m_Skill_3 = value; InvalidateProperties(); } }

		public int m_Skill_4;
		[CommandProperty( AccessLevel.GameMaster )]
		public int mSkill4 { get{ return m_Skill_4; } set{ m_Skill_4 = value; InvalidateProperties(); } }

		public int m_Skill_5;
		[CommandProperty( AccessLevel.GameMaster )]
		public int mSkill5 { get{ return m_Skill_5; } set{ m_Skill_5 = value; InvalidateProperties(); } }

		public double m_Value_1;
		[CommandProperty( AccessLevel.GameMaster )]
		public double mValue1 { get{ return m_Value_1; } set{ m_Value_1 = value; InvalidateProperties(); } }

		public double m_Value_2;
		[CommandProperty( AccessLevel.GameMaster )]
		public double mValue2 { get{ return m_Value_2; } set{ m_Value_2 = value; InvalidateProperties(); } }

		public double m_Value_3;
		[CommandProperty( AccessLevel.GameMaster )]
		public double mValue3 { get{ return m_Value_3; } set{ m_Value_3 = value; InvalidateProperties(); } }

		public double m_Value_4;
		[CommandProperty( AccessLevel.GameMaster )]
		public double mValue4 { get{ return m_Value_4; } set{ m_Value_4 = value; InvalidateProperties(); } }

		public double m_Value_5;
		[CommandProperty( AccessLevel.GameMaster )]
		public double mValue5 { get{ return m_Value_5; } set{ m_Value_5 = value; InvalidateProperties(); } }

		public int m_Slayer_1;
		[CommandProperty( AccessLevel.GameMaster )]
		public int mSlayer1 { get{ return m_Slayer_1; } set{ m_Slayer_1 = value; InvalidateProperties(); } }

		public int m_Slayer_2;
		[CommandProperty( AccessLevel.GameMaster )]
		public int mSlayer2 { get{ return m_Slayer_2; } set{ m_Slayer_2 = value; InvalidateProperties(); } }

		public Mobile m_Owner;
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile mOwner { get{ return m_Owner; } set{ m_Owner = value; InvalidateProperties(); } }

		public string m_Extra;
		[CommandProperty( AccessLevel.GameMaster )]
		public string mExtra { get{ return m_Extra; } set{ m_Extra = value; InvalidateProperties(); } }

		public string m_FromWho;
		[CommandProperty( AccessLevel.GameMaster )]
		public string mFromWho { get{ return m_FromWho; } set{ m_FromWho = value; InvalidateProperties(); } }

		public string m_HowGiven;
		[CommandProperty( AccessLevel.GameMaster )]
		public string mHowGiven { get{ return m_HowGiven; } set{ m_HowGiven = value; InvalidateProperties(); } }

		public int m_Points;
		[CommandProperty( AccessLevel.GameMaster )]
		public int mPoints { get{ return m_Points; } set{ m_Points = value; InvalidateProperties(); } }

		public int m_Hue;
		[CommandProperty( AccessLevel.GameMaster )]
		public int mHue { get{ return m_Hue; } set{ m_Hue = value; InvalidateProperties(); } }

		public ManualOfItems( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 ); // version
            writer.Write(m_Charges);
            writer.Write(m_Skill_1);
            writer.Write(m_Skill_2);
            writer.Write(m_Skill_3);
            writer.Write(m_Skill_4);
            writer.Write(m_Skill_5);
            writer.Write(m_Value_1);
            writer.Write(m_Value_2);
            writer.Write(m_Value_3);
            writer.Write(m_Value_4);
            writer.Write(m_Value_5);
            writer.Write(m_Slayer_1);
            writer.Write(m_Slayer_2);
            writer.Write(m_Owner);
            writer.Write(m_Extra);
            writer.Write(m_FromWho);
            writer.Write(m_HowGiven);
            writer.Write(m_Points);
            writer.Write(m_Hue);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_Charges = reader.ReadInt();
			m_Skill_1 = reader.ReadInt();
			m_Skill_2 = reader.ReadInt();
			m_Skill_3 = reader.ReadInt();
			m_Skill_4 = reader.ReadInt();
			m_Skill_5 = reader.ReadInt();
			m_Value_1 = reader.ReadDouble();
			m_Value_2 = reader.ReadDouble();
			m_Value_3 = reader.ReadDouble();
			m_Value_4 = reader.ReadDouble();
			m_Value_5 = reader.ReadDouble();
			m_Slayer_1 = reader.ReadInt();
			m_Slayer_2 = reader.ReadInt();
			m_Owner = reader.ReadMobile();
			m_Extra = reader.ReadString();
            m_FromWho = reader.ReadString();
            m_HowGiven = reader.ReadString();
			m_Points = reader.ReadInt();
			m_Hue = reader.ReadInt();
			if ( ItemID != 0x1C0E && ItemID != 0x1C0F ){ ItemID = Utility.RandomList( 0x1C0E, 0x1C0F ); }
			if ( Name.Contains("Tome ") ){ Name = Name.Replace("Tome ", "Chest "); }
		}
	}
}