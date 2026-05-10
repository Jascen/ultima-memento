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

namespace Server.Items
{
    public class AlchemistPouch : Bag
    {
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		[Constructable]
		public AlchemistPouch() : base()
		{
			Weight = 2.0;
			MaxItems = 50;
			ItemID = 0x5776;
			Name = "alchemist's belt pouch";
			Hue = 0xAFE;
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			if ( this.Weight > 1.0 ){ list.Add( 1070722, "Single Click to Organize" ); }
		}

		public override bool OnDragDropInto( Mobile from, Item dropped, Point3D p )
        {
			if ( isAlchemy( dropped ) )
				return base.OnDragDropInto(from, dropped, p);
			else if ( dropped.Catalog == Catalogs.Potion )
				from.SendMessage("That particular item cannot be used in this pouch.");
			else
				from.SendMessage("This belt pouch is for alchemy potions.");

			return false;
        }

		public override bool OnDragDrop( Mobile from, Item dropped )
        {
			if ( isAlchemy( dropped ) )
				return base.OnDragDrop(from, dropped);
			else if ( dropped.Catalog == Catalogs.Potion )
				from.SendMessage("That particular item cannot be used in this pouch.");
			else
				from.SendMessage("This belt pouch is for alchemy potions.");

			return false;
        }

		public class AlchemistBag : Gump
		{
			private AlchemistPouch m_Pouch;

			public AlchemistBag( Mobile from, AlchemistPouch bag ) : base( 50, 50 )
			{
				string color = "#AA7BAA";
				m_Pouch = bag;
				m_Pouch.Weight = 1.0;

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				AddImage(0, 0, 7027, Server.Misc.PlayerSettings.GetGumpHue( from ));
				AddHtml( 13, 13, 300, 20, @"<BODY><BASEFONT Color=" + color + ">ALCHEMY BELT POUCH</BASEFONT></BODY>", (bool)false, (bool)false);
				AddImage(531, 51, 10900);
				AddButton(863, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);


				AddHtml( 15, 57, 497, 176, @"<BODY><BASEFONT Color=" + color + ">This bag is only for potions created by alchemists, which will have their weight greatly reduced while in this bag. Here you can configure a quick belt pouch for these potions. This is also the only place where you can open and close the quick belt pouch, which is a bar that will open with icons for easy potion access. You can configure the bar to be either horizontal or vertical. You can choose to display abbreviated names next to the icons. You have to select which potions will appear in the bar and you can only effectively use one alchemy belt pouch at a time.</BASEFONT></BODY>", (bool)false, (bool)false);

				// ------------------------------------------------------------------------

				int bDisplay = 3609; if ( bag.Titles ){ bDisplay = 4017; }
					AddButton(15, 231, bDisplay, bDisplay, 52, GumpButtonType.Reply, 0);
						AddHtml( 55, 231, 229, 20, @"<BODY><BASEFONT Color=" + color + ">Display Abbreviations</BASEFONT></BODY>", (bool)false, (bool)false);
				int bVertical = 3609; if ( bag.Bar ){ bVertical = 4017; }
					AddButton(15, 265, bVertical, bVertical, 53, GumpButtonType.Reply, 0);
						AddHtml( 55, 265, 225, 20, @"<BODY><BASEFONT Color=" + color + ">Vertical Belt Pouch</BASEFONT></BODY>", (bool)false, (bool)false);

				AddButton(353, 231, 4029, 4029, 50, GumpButtonType.Reply, 0);
					AddHtml( 393, 231, 150, 20, @"<BODY><BASEFONT Color=" + color + ">Open Belt Pouch</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(353, 265, 4020, 4020, 51, GumpButtonType.Reply, 0);
					AddHtml( 393, 265, 150, 20, @"<BODY><BASEFONT Color=" + color + ">Close Belt Pouch</BASEFONT></BODY>", (bool)false, (bool)false);

				// ------------------------------------------------------------------------

				int val = 1;

				AddButton(15, 330, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(55, 325, 9509);
						AddHtml( 95, 330, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Agility</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(15, 372, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(55, 367, 9510);
						AddHtml( 95, 372, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Agility (G)</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(15, 414, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(55, 409, 9511);
						AddHtml( 95, 414, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Conflagration</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(15, 456, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(55, 451, 9512);
						AddHtml( 95, 456, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Conflagration (G)</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(15, 498, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(55, 493, 9513);
						AddHtml( 95, 498, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Confusion Blast</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(15, 540, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(55, 535, 9514);
						AddHtml( 95, 540, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Confusion Blast (G)</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(15, 582, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(55, 577, 9515);
						AddHtml( 95, 582, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Cure (L)</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(15, 624, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(55, 619, 9516);
						AddHtml( 95, 624, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Cure</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(15, 666, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(55, 661, 9517);
						AddHtml( 95, 666, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Cure (G)</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(15, 708, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(55, 703, 9518);
						AddHtml( 95, 708, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Explosion (L)</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(15, 750, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(55, 745, 9519);
						AddHtml( 95, 750, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Explosion</BASEFONT></BODY>", (bool)false, (bool)false);

				AddButton(330, 330, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(370, 325, 9520);
						AddHtml( 410, 330, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Explosion (G)</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(330, 372, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(370, 367, 9521);
						AddHtml( 410, 372, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Frostbite</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(330, 414, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(370, 409, 9522);
						AddHtml( 410, 414, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Frostbite (G)</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(330, 456, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(370, 451, 9523);
						AddHtml( 410, 456, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Heal (L)</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(330, 498, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(370, 493, 9524);
						AddHtml( 410, 498, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Heal</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(330, 540, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(370, 535, 9525);
						AddHtml( 410, 540, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Heal (G)</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(330, 582, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(370, 577, 9526);
						AddHtml( 410, 582, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Invisibility (L)</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(330, 624, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(370, 619, 9527);
						AddHtml( 410, 624, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Invisibility</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(330, 666, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(370, 661, 9528);
						AddHtml( 410, 666, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Invisibility (G)</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(330, 708, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(370, 703, 9529);
						AddHtml( 410, 708, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Invulnerability</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(330, 750, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(370, 745, 9530);
						AddHtml( 410, 750, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Mana (L)</BASEFONT></BODY>", (bool)false, (bool)false);

				AddButton(645, 372, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(685, 367, 9531);
						AddHtml( 725, 372, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Mana</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(645, 414, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(685, 409, 9532);
						AddHtml( 725, 414, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Mana (G)</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(645, 456, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(685, 451, 9533);
						AddHtml( 725, 456, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Night Sight</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(645, 498, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(685, 493, 9534);
						AddHtml( 725, 498, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Refresh</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(645, 540, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(685, 535, 9535);
						AddHtml( 725, 540, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Refresh, Total</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(645, 582, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(685, 577, 9536);
						AddHtml( 725, 582, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Rejuvenate (L)</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(645, 624, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(685, 619, 9537);
						AddHtml( 725, 624, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Rejuvenate</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(645, 666, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(685, 661, 9538);
						AddHtml( 725, 666, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Rejuvenate (G)</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(645, 708, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(685, 703, 9539);
						AddHtml( 725, 708, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Strength</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(645, 750, buttonVal(val, bag), buttonVal(val, bag), val, GumpButtonType.Reply, 0); val++;
					AddImage(685, 745, 9540);
						AddHtml( 725, 750, 153, 20, @"<BODY><BASEFONT Color=" + color + ">Strength (G)</BASEFONT></BODY>", (bool)false, (bool)false);
			}

			public int buttonVal( int val, AlchemistPouch bag )
			{
				int button = 3609; // Unchecked

				if ( val == 1 && bag.v_01_Agility ){ button = 4017; }
				else if ( val == 2 && bag.v_02_GreaterAgility ){ button = 4017; }
				else if ( val == 3 && bag.v_03_Conflagration ){ button = 4017; }
				else if ( val == 4 && bag.v_04_GreaterConflagration ){ button = 4017; }
				else if ( val == 5 && bag.v_05_ConfusionBlast ){ button = 4017; }
				else if ( val == 6 && bag.v_06_GreaterConfusionBlast ){ button = 4017; }
				else if ( val == 7 && bag.v_07_LesserCure ){ button = 4017; }
				else if ( val == 8 && bag.v_08_Cure ){ button = 4017; }
				else if ( val == 9 && bag.v_09_GreaterCure ){ button = 4017; }
				else if ( val == 10 && bag.v_10_LesserExplosion ){ button = 4017; }
				else if ( val == 11 && bag.v_11_Explosion ){ button = 4017; }
				else if ( val == 12 && bag.v_12_GreaterExplosion ){ button = 4017; }
				else if ( val == 13 && bag.v_13_Frostbite ){ button = 4017; }
				else if ( val == 14 && bag.v_14_GreaterFrostbite ){ button = 4017; }
				else if ( val == 15 && bag.v_15_LesserHeal ){ button = 4017; }
				else if ( val == 16 && bag.v_16_Heal ){ button = 4017; }
				else if ( val == 17 && bag.v_17_GreaterHeal ){ button = 4017; }
				else if ( val == 18 && bag.v_18_LesserInvisibility ){ button = 4017; }
				else if ( val == 19 && bag.v_19_Invisibility ){ button = 4017; }
				else if ( val == 20 && bag.v_20_GreaterInvisibility ){ button = 4017; }
				else if ( val == 21 && bag.v_21_Invulnerability ){ button = 4017; }
				else if ( val == 22 && bag.v_22_LesserMana ){ button = 4017; }
				else if ( val == 23 && bag.v_23_Mana ){ button = 4017; }
				else if ( val == 24 && bag.v_24_GreaterMana ){ button = 4017; }
				else if ( val == 25 && bag.v_25_NightSight ){ button = 4017; }
				else if ( val == 26 && bag.v_26_Refresh ){ button = 4017; }
				else if ( val == 27 && bag.v_27_TotalRefresh ){ button = 4017; }
				else if ( val == 28 && bag.v_28_LesserRejuvenate ){ button = 4017; }
				else if ( val == 29 && bag.v_29_Rejuvenate ){ button = 4017; }
				else if ( val == 30 && bag.v_30_GreaterRejuvenate ){ button = 4017; }
				else if ( val == 31 && bag.v_31_Strength ){ button = 4017; }
				else if ( val == 32 && bag.v_32_GreaterStrength ){ button = 4017; }

				return button;
			}

			public override void OnResponse( NetState state, RelayInfo info ) 
			{
				Mobile from = state.Mobile; 

				if ( m_Pouch.IsChildOf( from.Backpack ) )
				{
					if ( info.ButtonID == 50 )
					{
						from.SendSound( 0x4A );
						from.CloseGump( typeof( ChemistBar ) );
						from.SendGump( new AlchemistBag( from, m_Pouch ) );
						if ( m_Pouch.Bar ){ from.SendGump( new ChemistBar( from, m_Pouch, true ) ); }
						else { from.SendGump( new ChemistBar( from, m_Pouch, false ) ); }
					}
					else if ( info.ButtonID == 51 )
					{
						from.SendSound( 0x4A );
						from.CloseGump( typeof( ChemistBar ) );
						from.SendGump( new AlchemistBag( from, m_Pouch ) );
					}
					else if ( info.ButtonID == 52 )
					{
						from.SendSound( 0x4A );
						m_Pouch.Titles = !m_Pouch.Titles;
						from.CloseGump( typeof( AlchemistBag ) );
						from.SendGump( new AlchemistBag( from, m_Pouch ) );
					}
					else if ( info.ButtonID == 53 )
					{
						from.SendSound( 0x4A );
						m_Pouch.Bar = !m_Pouch.Bar;
						from.CloseGump( typeof( AlchemistBag ) );
						from.SendGump( new AlchemistBag( from, m_Pouch ) );
					}
					else if ( info.ButtonID > 0 && info.ButtonID < 40 )
					{
						from.SendSound( 0x4A );
						if ( info.ButtonID == 1 ){ m_Pouch.v_01_Agility = !m_Pouch.v_01_Agility; }
						else if ( info.ButtonID == 2 ){ m_Pouch.v_02_GreaterAgility = !m_Pouch.v_02_GreaterAgility; }
						else if ( info.ButtonID == 3 ){ m_Pouch.v_03_Conflagration = !m_Pouch.v_03_Conflagration; }
						else if ( info.ButtonID == 4 ){ m_Pouch.v_04_GreaterConflagration = !m_Pouch.v_04_GreaterConflagration; }
						else if ( info.ButtonID == 5 ){ m_Pouch.v_05_ConfusionBlast = !m_Pouch.v_05_ConfusionBlast; }
						else if ( info.ButtonID == 6 ){ m_Pouch.v_06_GreaterConfusionBlast = !m_Pouch.v_06_GreaterConfusionBlast; }
						else if ( info.ButtonID == 7 ){ m_Pouch.v_07_LesserCure = !m_Pouch.v_07_LesserCure; }
						else if ( info.ButtonID == 8 ){ m_Pouch.v_08_Cure = !m_Pouch.v_08_Cure; }
						else if ( info.ButtonID == 9 ){ m_Pouch.v_09_GreaterCure = !m_Pouch.v_09_GreaterCure; }
						else if ( info.ButtonID == 10 ){ m_Pouch.v_10_LesserExplosion = !m_Pouch.v_10_LesserExplosion; }
						else if ( info.ButtonID == 11 ){ m_Pouch.v_11_Explosion = !m_Pouch.v_11_Explosion; }
						else if ( info.ButtonID == 12 ){ m_Pouch.v_12_GreaterExplosion = !m_Pouch.v_12_GreaterExplosion; }
						else if ( info.ButtonID == 13 ){ m_Pouch.v_13_Frostbite = !m_Pouch.v_13_Frostbite; }
						else if ( info.ButtonID == 14 ){ m_Pouch.v_14_GreaterFrostbite = !m_Pouch.v_14_GreaterFrostbite; }
						else if ( info.ButtonID == 15 ){ m_Pouch.v_15_LesserHeal = !m_Pouch.v_15_LesserHeal; }
						else if ( info.ButtonID == 16 ){ m_Pouch.v_16_Heal = !m_Pouch.v_16_Heal; }
						else if ( info.ButtonID == 17 ){ m_Pouch.v_17_GreaterHeal = !m_Pouch.v_17_GreaterHeal; }
						else if ( info.ButtonID == 18 ){ m_Pouch.v_18_LesserInvisibility = !m_Pouch.v_18_LesserInvisibility; }
						else if ( info.ButtonID == 19 ){ m_Pouch.v_19_Invisibility = !m_Pouch.v_19_Invisibility; }
						else if ( info.ButtonID == 20 ){ m_Pouch.v_20_GreaterInvisibility = !m_Pouch.v_20_GreaterInvisibility; }
						else if ( info.ButtonID == 21 ){ m_Pouch.v_21_Invulnerability = !m_Pouch.v_21_Invulnerability; }
						else if ( info.ButtonID == 22 ){ m_Pouch.v_22_LesserMana = !m_Pouch.v_22_LesserMana; }
						else if ( info.ButtonID == 23 ){ m_Pouch.v_23_Mana = !m_Pouch.v_23_Mana; }
						else if ( info.ButtonID == 24 ){ m_Pouch.v_24_GreaterMana = !m_Pouch.v_24_GreaterMana; }
						else if ( info.ButtonID == 25 ){ m_Pouch.v_25_NightSight = !m_Pouch.v_25_NightSight; }
						else if ( info.ButtonID == 26 ){ m_Pouch.v_26_Refresh = !m_Pouch.v_26_Refresh; }
						else if ( info.ButtonID == 27 ){ m_Pouch.v_27_TotalRefresh = !m_Pouch.v_27_TotalRefresh; }
						else if ( info.ButtonID == 28 ){ m_Pouch.v_28_LesserRejuvenate = !m_Pouch.v_28_LesserRejuvenate; }
						else if ( info.ButtonID == 29 ){ m_Pouch.v_29_Rejuvenate = !m_Pouch.v_29_Rejuvenate; }
						else if ( info.ButtonID == 30 ){ m_Pouch.v_30_GreaterRejuvenate = !m_Pouch.v_30_GreaterRejuvenate; }
						else if ( info.ButtonID == 31 ){ m_Pouch.v_31_Strength = !m_Pouch.v_31_Strength; }
						else if ( info.ButtonID == 32 ){ m_Pouch.v_32_GreaterStrength = !m_Pouch.v_32_GreaterStrength; }

						from.CloseGump( typeof( AlchemistBag ) );
						from.SendGump( new AlchemistBag( from, m_Pouch ) );
					}
					else
					{
						from.PlaySound( 0x48 );
					}

					if ( from.HasGump( typeof( ChemistBar ) ) )
					{
						from.CloseGump( typeof( ChemistBar ) );
						if ( m_Pouch.Bar ){ from.SendGump( new ChemistBar( from, m_Pouch, true ) ); }
						else { from.SendGump( new ChemistBar( from, m_Pouch, false ) ); }
					}
				}
			}
		}

		public class ChemistBar : Gump
		{
			private const int MAX_POTION_NUMBER = 32;

			private AlchemistPouch m_Pouch;
			public ChemistBar( Mobile from, AlchemistPouch bag, bool vertical ): base( 50, 50 )
			{
				this.Closable=false;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;
				AddPage(0);
				m_Pouch = bag;
				int btn = 0;
				int cyc = 0;
				bool overHalf = OverHalf( bag );

				if ( vertical )
				{
					if ( overHalf ){ AddImage(10, -18, 10903); } else { AddImage(0, 0, 10902); }

					int mod = 34;
					int xValue = 0;
					int yValue = 0;

					for(var i = 1; i <= MAX_POTION_NUMBER; i++)
                    {
                        if (!IsVisible(i, bag)) continue;

                        int gumpId = 9508 + i;
                        yValue += mod;
                        AddButton(xValue, yValue, gumpId, gumpId, i, GumpButtonType.Reply, 0);

                        // New column - Reset
                        if (++cyc > 15 && xValue == 0)
                        {
                            xValue = 33;
                            yValue = 0;
                        }
                    }
                }
				else
				{
					if ( overHalf ){ AddImage(-18, 10, 10903); } else { AddImage(0, 0, 10902); }

					int mod = 33;
					int xValue = 0;
					int yValue = 0;
					for(var i = 1; i <= MAX_POTION_NUMBER; i++)
                    {
                        if (!IsVisible(i, bag)) continue;

                        int gumpId = 9508 + i;
                        xValue += mod;
                        AddButton(xValue, yValue, gumpId, gumpId, btn, GumpButtonType.Reply, 0);

                        // New Row - Reset
                        if (++cyc > 15 && yValue == 0)
                        {
                            yValue = 34;
                            xValue = 0;
                        }
                    }
                }

				if ( bag.Titles )
				{
					cyc = 0;
					bool hueFlipFlop = false;
					for(var i = 1; i <= MAX_POTION_NUMBER; i++)
					{
						if (IsVisible(i, bag))
						{
							int hue = hueFlipFlop ? 2929 : 2770;
							AddLabel(cordVal(cyc, overHalf, vertical, 0), cordVal(cyc, overHalf, vertical, 1), hue, GetLabel(i));
							cyc++;
							hueFlipFlop = !hueFlipFlop;
						}
					}
				}
			}

			private bool IsVisible(int potionNumber, AlchemistPouch bag)
			{
				if (potionNumber == 01) return bag.v_01_Agility;
				if (potionNumber == 02) return bag.v_02_GreaterAgility;
				if (potionNumber == 03) return bag.v_03_Conflagration;
				if (potionNumber == 04) return bag.v_04_GreaterConflagration;
				if (potionNumber == 05) return bag.v_05_ConfusionBlast;
				if (potionNumber == 06) return bag.v_06_GreaterConfusionBlast;
				if (potionNumber == 07) return bag.v_07_LesserCure;
				if (potionNumber == 08) return bag.v_08_Cure;
				if (potionNumber == 09) return bag.v_09_GreaterCure;
				if (potionNumber == 10) return bag.v_10_LesserExplosion;
				if (potionNumber == 11) return bag.v_11_Explosion;
				if (potionNumber == 12) return bag.v_12_GreaterExplosion;
				if (potionNumber == 13) return bag.v_13_Frostbite;
				if (potionNumber == 14) return bag.v_14_GreaterFrostbite;
				if (potionNumber == 15) return bag.v_15_LesserHeal;
				if (potionNumber == 16) return bag.v_16_Heal;
				if (potionNumber == 17) return bag.v_17_GreaterHeal;
				if (potionNumber == 18) return bag.v_18_LesserInvisibility;
				if (potionNumber == 19) return bag.v_19_Invisibility;
				if (potionNumber == 20) return bag.v_20_GreaterInvisibility;
				if (potionNumber == 21) return bag.v_21_Invulnerability;
				if (potionNumber == 22) return bag.v_22_LesserMana;
				if (potionNumber == 23) return bag.v_23_Mana;
				if (potionNumber == 24) return bag.v_24_GreaterMana;
				if (potionNumber == 25) return bag.v_25_NightSight;
				if (potionNumber == 26) return bag.v_26_Refresh;
				if (potionNumber == 27) return bag.v_27_TotalRefresh;
				if (potionNumber == 28) return bag.v_28_LesserRejuvenate;
				if (potionNumber == 29) return bag.v_29_Rejuvenate;
				if (potionNumber == 30) return bag.v_30_GreaterRejuvenate;
				if (potionNumber == 31) return bag.v_31_Strength;
				if (potionNumber == 32) return bag.v_32_GreaterStrength;

				return false;
			}

			private string GetLabel(int potionNumber)
			{
				if (potionNumber == 01) return @"Ag";
				if (potionNumber == 02) return @"AgG";
				if (potionNumber == 03) return @"Cf";
				if (potionNumber == 04) return @"CfG";
				if (potionNumber == 05) return @"Cb";
				if (potionNumber == 06) return @"CbG";
				if (potionNumber == 07) return @"CuL";
				if (potionNumber == 08) return @"Cu";
				if (potionNumber == 09) return @"CuG";
				if (potionNumber == 10) return @"ExL";
				if (potionNumber == 11) return @"Ex";
				if (potionNumber == 12) return @"ExG";
				if (potionNumber == 13) return @"Fb";
				if (potionNumber == 14) return @"FbG";
				if (potionNumber == 15) return @"HeL";
				if (potionNumber == 16) return @"He";
				if (potionNumber == 17) return @"HeG";
				if (potionNumber == 18) return @"InL";
				if (potionNumber == 19) return @"In";
				if (potionNumber == 20) return @"InG";
				if (potionNumber == 21) return @"<->";
				if (potionNumber == 22) return @"MnL";
				if (potionNumber == 23) return @"Mn";
				if (potionNumber == 24) return @"MnG";
				if (potionNumber == 25) return @"NiS";
				if (potionNumber == 26) return @"Rf";
				if (potionNumber == 27) return @"RfT";
				if (potionNumber == 28) return @"RjL";
				if (potionNumber == 29) return @"Rj";
				if (potionNumber == 30) return @"RjG";
				if (potionNumber == 31) return @"St";
				if (potionNumber == 32) return @"StG";

 				return "";
			}

			public override void OnResponse( NetState state, RelayInfo info ) 
			{
				Mobile from = state.Mobile;
				if ( m_Pouch.IsChildOf( from.Backpack ) )
				{
					usePotion( info.ButtonID, from );
					from.CloseGump( typeof( ChemistBar ) );
					bool vertical = false; if ( m_Pouch.Bar ){ vertical = true; }
					from.SendGump( new ChemistBar( from, m_Pouch, vertical ) );
				}
			}

			public static int cordVal( int cyc, bool overHalf, bool vertical, int prt )
			{
				int x = 0;
				int y = 0;
				cyc++;
				int num = cyc;
				if ( num > 16 ){ num = num-16; }

				if ( vertical && overHalf )
				{
					x = -28;
					y = 8;
					if ( cyc < 17 ){ x = -28; y = y + (num*34); }
					else if ( cyc == 17 ){ x = 36+33; y = 8+34; }
					else { x = 36+33; y = y + (num*34); }
				}
				else if ( vertical )
				{
					x = 36;
					y = 8 + (num*34);
				}
				else if ( !vertical && overHalf )
				{
					x = 5;
					y = -21;
					if ( cyc < 17 ){ x = x + (num*33); y = -21; }
					else if ( cyc == 17 ){ x = x + (num*33); y = 35+34; }
					else { x = x + (num*33); y = 35+34; }
				}
				else
				{
					x = 5 + (num*33);
					y = 35;
				}

				if ( prt == 1 )
					return y;

				return x;
			}

			public static bool OverHalf( AlchemistPouch bag )
			{
				int val = 0;

				if ( bag.v_01_Agility ){ val++; }
				if ( bag.v_02_GreaterAgility ){ val++; }
				if ( bag.v_03_Conflagration ){ val++; }
				if ( bag.v_04_GreaterConflagration ){ val++; }
				if ( bag.v_05_ConfusionBlast ){ val++; }
				if ( bag.v_06_GreaterConfusionBlast ){ val++; }
				if ( bag.v_07_LesserCure ){ val++; }
				if ( bag.v_08_Cure ){ val++; }
				if ( bag.v_09_GreaterCure ){ val++; }
				if ( bag.v_10_LesserExplosion ){ val++; }
				if ( bag.v_11_Explosion ){ val++; }
				if ( bag.v_12_GreaterExplosion ){ val++; }
				if ( bag.v_13_Frostbite ){ val++; }
				if ( bag.v_14_GreaterFrostbite ){ val++; }
				if ( bag.v_15_LesserHeal ){ val++; }
				if ( bag.v_16_Heal ){ val++; }
				if ( bag.v_17_GreaterHeal ){ val++; }
				if ( bag.v_18_LesserInvisibility ){ val++; }
				if ( bag.v_19_Invisibility ){ val++; }
				if ( bag.v_20_GreaterInvisibility ){ val++; }
				if ( bag.v_21_Invulnerability ){ val++; }
				if ( bag.v_22_LesserMana ){ val++; }
				if ( bag.v_23_Mana ){ val++; }
				if ( bag.v_24_GreaterMana ){ val++; }
				if ( bag.v_25_NightSight ){ val++; }
				if ( bag.v_26_Refresh ){ val++; }
				if ( bag.v_27_TotalRefresh ){ val++; }
				if ( bag.v_28_LesserRejuvenate ){ val++; }
				if ( bag.v_29_Rejuvenate ){ val++; }
				if ( bag.v_30_GreaterRejuvenate ){ val++; }
				if ( bag.v_31_Strength ){ val++; }
				if ( bag.v_32_GreaterStrength ){ val++; }

				if ( val > 16 )
					return true;

				return false;
			}
		}

		public static void usePotion( int potion, Mobile from )
		{
			bool warn = true;

			if ( potion == 1 && from.Backpack.FindItemByType( typeof ( AgilityPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( AgilityPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 2 && from.Backpack.FindItemByType( typeof ( GreaterAgilityPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( GreaterAgilityPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 3 && from.Backpack.FindItemByType( typeof ( ConflagrationPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( ConflagrationPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 4 && from.Backpack.FindItemByType( typeof ( GreaterConflagrationPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( GreaterConflagrationPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 5 && from.Backpack.FindItemByType( typeof ( ConfusionBlastPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( ConfusionBlastPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 6 && from.Backpack.FindItemByType( typeof ( GreaterConfusionBlastPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( GreaterConfusionBlastPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 7 && from.Backpack.FindItemByType( typeof ( LesserCurePotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( LesserCurePotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 8 && from.Backpack.FindItemByType( typeof ( CurePotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( CurePotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 9 && from.Backpack.FindItemByType( typeof ( GreaterCurePotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( GreaterCurePotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 10 && from.Backpack.FindItemByType( typeof ( LesserExplosionPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( LesserExplosionPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 11 && from.Backpack.FindItemByType( typeof ( ExplosionPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( ExplosionPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 12 && from.Backpack.FindItemByType( typeof ( GreaterExplosionPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( GreaterExplosionPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 13 && from.Backpack.FindItemByType( typeof ( FrostbitePotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( FrostbitePotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 14 && from.Backpack.FindItemByType( typeof ( GreaterFrostbitePotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( GreaterFrostbitePotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 15 && from.Backpack.FindItemByType( typeof ( LesserHealPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( LesserHealPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 16 && from.Backpack.FindItemByType( typeof ( HealPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( HealPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 17 && from.Backpack.FindItemByType( typeof ( GreaterHealPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( GreaterHealPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 18 && from.Backpack.FindItemByType( typeof ( LesserInvisibilityPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( LesserInvisibilityPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 19 && from.Backpack.FindItemByType( typeof ( InvisibilityPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( InvisibilityPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 20 && from.Backpack.FindItemByType( typeof ( GreaterInvisibilityPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( GreaterInvisibilityPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 21 && from.Backpack.FindItemByType( typeof ( InvulnerabilityPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( InvulnerabilityPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 22 && from.Backpack.FindItemByType( typeof ( LesserManaPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( LesserManaPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 23 && from.Backpack.FindItemByType( typeof ( ManaPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( ManaPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 24 && from.Backpack.FindItemByType( typeof ( GreaterManaPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( GreaterManaPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 25 && from.Backpack.FindItemByType( typeof ( NightSightPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( NightSightPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 26 && from.Backpack.FindItemByType( typeof ( RefreshPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( RefreshPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 27 && from.Backpack.FindItemByType( typeof ( TotalRefreshPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( TotalRefreshPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 28 && from.Backpack.FindItemByType( typeof ( LesserRejuvenatePotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( LesserRejuvenatePotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 29 && from.Backpack.FindItemByType( typeof ( RejuvenatePotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( RejuvenatePotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 30 && from.Backpack.FindItemByType( typeof ( GreaterRejuvenatePotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( GreaterRejuvenatePotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 31 && from.Backpack.FindItemByType( typeof ( StrengthPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( StrengthPotion ) ) ).OnDoubleClick(from); warn = false; }
			else if ( potion == 32 && from.Backpack.FindItemByType( typeof ( GreaterStrengthPotion ) ) != null ){
				( from.Backpack.FindItemByType( typeof ( GreaterStrengthPotion ) ) ).OnDoubleClick(from); warn = false; }

			if ( warn ){ warnMe( from ); }
		}

		public static void warnMe( Mobile from )
		{
			string text = "You don't have that potion!";

			from.SendMessage( text );
			from.LocalOverheadMessage(MessageType.Emote, 1150, true, text);
		}

		public override bool OnDragLift( Mobile from )
		{
			from.SendMessage( "Single click this bag to organize it." );
			return base.OnDragLift( from );
		}

		public AlchemistPouch( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
			writer.Write( Bar );
			writer.Write( Titles );
			writer.Write( v_01_Agility );
			writer.Write( v_02_GreaterAgility );
			writer.Write( v_03_Conflagration );
			writer.Write( v_04_GreaterConflagration );
			writer.Write( v_05_ConfusionBlast );
			writer.Write( v_06_GreaterConfusionBlast );
			writer.Write( v_07_LesserCure );
			writer.Write( v_08_Cure );
			writer.Write( v_09_GreaterCure );
			writer.Write( v_10_LesserExplosion );
			writer.Write( v_11_Explosion );
			writer.Write( v_12_GreaterExplosion );
			writer.Write( v_13_Frostbite );
			writer.Write( v_14_GreaterFrostbite );
			writer.Write( v_15_LesserHeal );
			writer.Write( v_16_Heal );
			writer.Write( v_17_GreaterHeal );
			writer.Write( v_18_LesserInvisibility );
			writer.Write( v_19_Invisibility );
			writer.Write( v_20_GreaterInvisibility );
			writer.Write( v_21_Invulnerability );
			writer.Write( v_22_LesserMana );
			writer.Write( v_23_Mana );
			writer.Write( v_24_GreaterMana );
			writer.Write( v_25_NightSight );
			writer.Write( v_26_Refresh );
			writer.Write( v_27_TotalRefresh );
			writer.Write( v_28_LesserRejuvenate );
			writer.Write( v_29_Rejuvenate );
			writer.Write( v_30_GreaterRejuvenate );
			writer.Write( v_31_Strength );
			writer.Write( v_32_GreaterStrength );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			if (version == 0)
			{
				Bar = reader.ReadInt() == 1;
				Titles = reader.ReadInt() == 1;
				v_01_Agility = reader.ReadInt() == 1;
				v_02_GreaterAgility = reader.ReadInt() == 1;
				v_03_Conflagration = reader.ReadInt() == 1;
				v_04_GreaterConflagration = reader.ReadInt() == 1;
				v_05_ConfusionBlast = reader.ReadInt() == 1;
				v_06_GreaterConfusionBlast = reader.ReadInt() == 1;
				v_07_LesserCure = reader.ReadInt() == 1;
				v_08_Cure = reader.ReadInt() == 1;
				v_09_GreaterCure = reader.ReadInt() == 1;
				v_10_LesserExplosion = reader.ReadInt() == 1;
				v_11_Explosion = reader.ReadInt() == 1;
				v_12_GreaterExplosion = reader.ReadInt() == 1;
				v_13_Frostbite = reader.ReadInt() == 1;
				v_14_GreaterFrostbite = reader.ReadInt() == 1;
				v_15_LesserHeal = reader.ReadInt() == 1;
				v_16_Heal = reader.ReadInt() == 1;
				v_17_GreaterHeal = reader.ReadInt() == 1;
				v_18_LesserInvisibility = reader.ReadInt() == 1;
				v_19_Invisibility = reader.ReadInt() == 1;
				v_20_GreaterInvisibility = reader.ReadInt() == 1;
				v_21_Invulnerability = reader.ReadInt() == 1;
				v_22_LesserMana = reader.ReadInt() == 1;
				v_23_Mana = reader.ReadInt() == 1;
				v_24_GreaterMana = reader.ReadInt() == 1;
				v_25_NightSight = reader.ReadInt() == 1;
				v_26_Refresh = reader.ReadInt() == 1;
				v_27_TotalRefresh = reader.ReadInt() == 1;
				v_28_LesserRejuvenate = reader.ReadInt() == 1;
				v_29_Rejuvenate = reader.ReadInt() == 1;
				v_30_GreaterRejuvenate = reader.ReadInt() == 1;
				v_31_Strength = reader.ReadInt() == 1;
				v_32_GreaterStrength = reader.ReadInt() == 1;
			}
			else
			{
				Bar = reader.ReadBool();
				Titles = reader.ReadBool();
				v_01_Agility = reader.ReadBool();
				v_02_GreaterAgility = reader.ReadBool();
				v_03_Conflagration = reader.ReadBool();
				v_04_GreaterConflagration = reader.ReadBool();
				v_05_ConfusionBlast = reader.ReadBool();
				v_06_GreaterConfusionBlast = reader.ReadBool();
				v_07_LesserCure = reader.ReadBool();
				v_08_Cure = reader.ReadBool();
				v_09_GreaterCure = reader.ReadBool();
				v_10_LesserExplosion = reader.ReadBool();
				v_11_Explosion = reader.ReadBool();
				v_12_GreaterExplosion = reader.ReadBool();
				v_13_Frostbite = reader.ReadBool();
				v_14_GreaterFrostbite = reader.ReadBool();
				v_15_LesserHeal = reader.ReadBool();
				v_16_Heal = reader.ReadBool();
				v_17_GreaterHeal = reader.ReadBool();
				v_18_LesserInvisibility = reader.ReadBool();
				v_19_Invisibility = reader.ReadBool();
				v_20_GreaterInvisibility = reader.ReadBool();
				v_21_Invulnerability = reader.ReadBool();
				v_22_LesserMana = reader.ReadBool();
				v_23_Mana = reader.ReadBool();
				v_24_GreaterMana = reader.ReadBool();
				v_25_NightSight = reader.ReadBool();
				v_26_Refresh = reader.ReadBool();
				v_27_TotalRefresh = reader.ReadBool();
				v_28_LesserRejuvenate = reader.ReadBool();
				v_29_Rejuvenate = reader.ReadBool();
				v_30_GreaterRejuvenate = reader.ReadBool();
				v_31_Strength = reader.ReadBool();
				v_32_GreaterStrength = reader.ReadBool();
			}
		}

		public bool isAlchemy( Item item )
		{
			if (
				item is BaseMixture || 
				item is AgilityPotion || 
				item is GreaterAgilityPotion || 
				item is ConflagrationPotion || 
				item is GreaterConflagrationPotion || 
				item is ConfusionBlastPotion || 
				item is GreaterConfusionBlastPotion || 
				item is LesserCurePotion || 
				item is CurePotion || 
				item is GreaterCurePotion || 
				item is LesserExplosionPotion || 
				item is ExplosionPotion || 
				item is GreaterExplosionPotion || 
				item is FrostbitePotion || 
				item is GreaterFrostbitePotion || 
				item is LesserHealPotion || 
				item is HealPotion || 
				item is GreaterHealPotion || 
				item is LesserInvisibilityPotion || 
				item is InvisibilityPotion || 
				item is GreaterInvisibilityPotion || 
				item is InvulnerabilityPotion || 
				item is LesserManaPotion || 
				item is ManaPotion || 
				item is GreaterManaPotion || 
				item is NightSightPotion || 
				item is RefreshPotion || 
				item is TotalRefreshPotion || 
				item is LesserRejuvenatePotion || 
				item is RejuvenatePotion || 
				item is GreaterRejuvenatePotion || 
				item is StrengthPotion || 
				item is GreaterStrengthPotion 
			){ return true; }
			return false;
		}

		public override int GetTotal(TotalType type)
        {
			if (type != TotalType.Weight)
				return base.GetTotal(type);
			else
			{
				return (int)(TotalItemWeights() * (0.05));
			}
        }

		public override void UpdateTotal(Item sender, TotalType type, int delta)
        {
            if (type != TotalType.Weight)
                base.UpdateTotal(sender, type, delta);
            else
                base.UpdateTotal(sender, type, (int)(delta * (0.05)));
        }

		private double TotalItemWeights()
        {
			double weight = 0.0;

			foreach (Item item in Items)
				weight += (item.Weight * (double)(item.Amount));

			return weight;
        }

		public class BagWindow : ContextMenuEntry 
		{ 
			private AlchemistPouch AlchemistBag; 
			private Mobile m_From; 

			public BagWindow( Mobile from, AlchemistPouch bag ) : base( 6172, 1 ) 
			{ 
				m_From = from; 
				AlchemistBag = bag; 
			} 

			public override void OnClick() 
			{          
				if( AlchemistBag.IsChildOf( m_From.Backpack ) ) 
				{ 
					m_From.CloseGump( typeof( AlchemistBag ) );
					m_From.SendGump( new AlchemistBag( m_From, AlchemistBag ) );
					m_From.PlaySound( 0x48 );
				} 
				else 
				{
					m_From.SendMessage( "This must be in your backpack to organize." );
				} 
			} 
		} 

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list ) 
		{
			base.GetContextMenuEntries( from, list );

			if ( from.Alive )
				list.Add( new BagWindow( from, this ) );
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Bar { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Titles { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_01_Agility { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_02_GreaterAgility { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_03_Conflagration { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_04_GreaterConflagration { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_05_ConfusionBlast { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_06_GreaterConfusionBlast { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_07_LesserCure { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_08_Cure { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_09_GreaterCure { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_10_LesserExplosion { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_11_Explosion { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_12_GreaterExplosion { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_13_Frostbite { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_14_GreaterFrostbite { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_15_LesserHeal { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_16_Heal { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_17_GreaterHeal { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_18_LesserInvisibility { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_19_Invisibility { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_20_GreaterInvisibility { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_21_Invulnerability { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_22_LesserMana { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_23_Mana { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_24_GreaterMana { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_25_NightSight { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_26_Refresh { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_27_TotalRefresh { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_28_LesserRejuvenate { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_29_Rejuvenate { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_30_GreaterRejuvenate { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_31_Strength { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool v_32_GreaterStrength { get; set; }
	}
}