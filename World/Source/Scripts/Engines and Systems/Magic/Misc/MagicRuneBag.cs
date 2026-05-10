using Server;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Gumps;

namespace Server.Items
{
	public class MagicRuneBag : Item
	{
		public override void ResourceChanged( CraftResource resource )
		{
			if ( !ResourceCanChange() )
				return;

			ResourceMods.Modify( this, true );
			m_Resource = resource;
			Hue = CraftResources.GetHue(m_Resource);
			Name = CraftResources.GetTradeItemFullName( this, m_Resource, false, false, "rune bag" );
			ResourceMods.Modify( this, false );
			ColorText1 = Name;
			ColorHue1 = "0080FF";
			ResourceMods.DefaultItemHue( this );
			InvalidateProperties();
		}

		public override bool DisplayWeight{ get{ return false; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }
		public override Catalogs DefaultCatalog{ get{ return Catalogs.Trinket; } }
		public override string DefaultDescription{ get{ return "These ancient, magical bags were once used by aspiring scholars to cast spells with the use of rune stones. Wizards eventually learned how to wield the magic without these, but they may still be of use to those that are not magically gifted."; } }

		[Constructable]
		public MagicRuneBag() : base( 0x0E84 )
		{
			Weight = 0.1;
			Layer = Layer.Trinket;
			EnchantUsesMax = 200;
			ResourceMods.SetRandomResource( false, false, this, CraftResource.RegularLeather, false, null );
		}

		public override bool OnEquip( Mobile from )
		{
			from.CloseGump( typeof( MagicRuneGump ) );
			if ( Enchanted == MagicSpell.None )
			{
				from.LocalOverheadMessage( MessageType.Emote, 0x916, true, "There is no magic on the bag!" );
				from.SendMessage( "There is no magic on the bag!" );
				return false;
			}
			base.OnEquip( from );
			return true;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1061182, EquipLayerName( Layer ) );
		}

		public override void MagicSpellChanged( MagicSpell spell )
		{
			Enchanted = spell;
			EnchantMod = SpellItems.GetCircleNumber( spell );
		}

		public override void CastEnchantment( Mobile from )
		{
			SpellItems.CastEnchantment( from, this );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( Parent == from ) 
			{
				CastEnchantment( from );
			}
			else if ( RootParent == from )
			{
				from.CloseGump( typeof( MagicRuneGump ) );
				from.SendGump( new MagicRuneGump( from, this ) );
				from.PlaySound( 0x48 );
			}
			else
				from.SendMessage( "You can only use this from your pack." );
		}

		public class MagicRuneGump : Gump
		{
			private MagicRuneBag mBag;
			private Mobile mFrom;

			public MagicRuneGump( Mobile from, MagicRuneBag bag ): base( 50, 50 )
			{
				string color = "#FFC100";

				mBag = bag;
				mFrom = from;

				Closable=true;
				Disposable=true;
				Dragable=true;
				Resizable=false;

				int xm = -34;
				int ym = -27;

				AddPage(0);

				AddImage(0, 0, 10465);

				AddHtml( 12, 11, 424, 66, @"<BODY><BASEFONT Color=" + color + ">This magical bag will hold runes of power for which you can combine to cast spells from the bag. When a bag is set for a spell, it must then be equipped in order to cast it by double clicking the bag.</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 13, 370, 424, 163, @"<BODY><BASEFONT Color=" + color + ">You can add runes by dropping them onto the bag. You can use the down arrow buttons to remove the rune from the bag. To select a rune to have ready for a spell, select the button to the right of the arrow for the rune. If you have a proper combination, then the bag will be readied for a spell and can then be equipped to use. These bags always have the book to reference this unusual form of magic, and you can read the book by selecting the scroll button on the bottom right. These bags can have a certain number of charges, where a mage can recharge it for you for a fee.</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( mBag.m_Rune_An )
				{
					AddItem(108+xm, 122+ym, 0x2379);
					AddHtml( 70+xm, 120+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_An ) + "><RIGHT>An</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(145+xm, 120+ym, 252, 252, 1, GumpButtonType.Reply, 0);
					AddButton(164+xm, 121+ym, Button( mBag.m_Selected_An ), Button( mBag.m_Selected_An ), 101, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Bet )
				{
					AddItem(222+xm, 122+ym, 0x237A);
					AddHtml( 184+xm, 120+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Bet ) + "><RIGHT>Bet</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(260+xm, 120+ym, 252, 252, 2, GumpButtonType.Reply, 0);
					AddButton(279+xm, 121+ym, Button( mBag.m_Selected_Bet ), Button( mBag.m_Selected_Bet ), 102, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Corp )
				{
					AddItem(336+xm, 122+ym, 0x237B);
					AddHtml( 298+xm, 120+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Corp ) + "><RIGHT>Corp</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(374+xm, 120+ym, 252, 252, 3, GumpButtonType.Reply, 0);
					AddButton(393+xm, 121+ym, Button( mBag.m_Selected_Corp ), Button( mBag.m_Selected_Corp ), 103, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Des )
				{
					AddItem(108+xm, 151+ym, 0x237C);
					AddHtml( 70+xm, 149+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Des ) + "><RIGHT>Des</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(145+xm, 150+ym, 252, 252, 4, GumpButtonType.Reply, 0);
					AddButton(164+xm, 151+ym, Button( mBag.m_Selected_Des ), Button( mBag.m_Selected_Des ), 104, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Ex )
				{
					AddItem(222+xm, 151+ym, 0x237D);
					AddHtml( 184+xm, 149+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Ex ) + "><RIGHT>Ex</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(260+xm, 150+ym, 252, 252, 5, GumpButtonType.Reply, 0);
					AddButton(279+xm, 151+ym, Button( mBag.m_Selected_Ex ), Button( mBag.m_Selected_Ex ), 105, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Flam )
				{
					AddItem(336+xm, 151+ym, 0x2387);
					AddHtml( 298+xm, 149+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Flam ) + "><RIGHT>Flam</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(374+xm, 150+ym, 252, 252, 6, GumpButtonType.Reply, 0);
					AddButton(393+xm, 151+ym, Button( mBag.m_Selected_Flam ), Button( mBag.m_Selected_Flam ), 106, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Grav )
				{
					AddItem(108+xm, 182+ym, 0x2389);
					AddHtml( 70+xm, 180+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Grav ) + "><RIGHT>Grav</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(145+xm, 180+ym, 252, 252, 7, GumpButtonType.Reply, 0);
					AddButton(164+xm, 181+ym, Button( mBag.m_Selected_Grav ), Button( mBag.m_Selected_Grav ), 107, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Hur )
				{
					AddItem(222+xm, 182+ym, 0x238A);
					AddHtml( 184+xm, 180+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Hur ) + "><RIGHT>Hur</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(260+xm, 180+ym, 252, 252, 8, GumpButtonType.Reply, 0);
					AddButton(279+xm, 181+ym, Button( mBag.m_Selected_Hur ), Button( mBag.m_Selected_Hur ), 108, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_In )
				{
					AddItem(336+xm, 182+ym, 0x2393);
					AddHtml( 298+xm, 180+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_In ) + "><RIGHT>In</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(374+xm, 180+ym, 252, 252, 9, GumpButtonType.Reply, 0);
					AddButton(393+xm, 181+ym, Button( mBag.m_Selected_In ), Button( mBag.m_Selected_In ), 109, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Jux )
				{
					AddItem(108+xm, 213+ym, 0x2394);
					AddHtml( 70+xm, 211+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Jux ) + "><RIGHT>Jux</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(145+xm, 210+ym, 252, 252, 10, GumpButtonType.Reply, 0);
					AddButton(164+xm, 211+ym, Button( mBag.m_Selected_Jux ), Button( mBag.m_Selected_Jux ), 110, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Kal )
				{
					AddItem(222+xm, 213+ym, 0x2395);
					AddHtml( 184+xm, 211+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Kal ) + "><RIGHT>Kal</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(260+xm, 210+ym, 252, 252, 11, GumpButtonType.Reply, 0);
					AddButton(279+xm, 211+ym, Button( mBag.m_Selected_Kal ), Button( mBag.m_Selected_Kal ), 111, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Lor )
				{
					AddItem(336+xm, 213+ym, 0x2396);
					AddHtml( 298+xm, 211+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Lor ) + "><RIGHT>Lor</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(374+xm, 210+ym, 252, 252, 12, GumpButtonType.Reply, 0);
					AddButton(393+xm, 211+ym, Button( mBag.m_Selected_Lor ), Button( mBag.m_Selected_Lor ), 112, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Mani )
				{
					AddItem(108+xm, 242+ym, 0x237E);
					AddHtml( 70+xm, 240+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Mani ) + "><RIGHT>Mani</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(145+xm, 240+ym, 252, 252, 13, GumpButtonType.Reply, 0);
					AddButton(164+xm, 241+ym, Button( mBag.m_Selected_Mani ), Button( mBag.m_Selected_Mani ), 113, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Nox )
				{
					AddItem(222+xm, 242+ym, 0x238B);
					AddHtml( 184+xm, 240+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Nox ) + "><RIGHT>Nox</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(260+xm, 240+ym, 252, 252, 14, GumpButtonType.Reply, 0);
					AddButton(279+xm, 241+ym, Button( mBag.m_Selected_Nox ), Button( mBag.m_Selected_Nox ), 114, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Ort )
				{
					AddItem(336+xm, 242+ym, 0x2398);
					AddHtml( 298+xm, 240+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Ort ) + "><RIGHT>Ort</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(374+xm, 240+ym, 252, 252, 15, GumpButtonType.Reply, 0);
					AddButton(393+xm, 241+ym, Button( mBag.m_Selected_Ort ), Button( mBag.m_Selected_Ort ), 115, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Por )
				{
					AddItem(108+xm, 274+ym, 0x237F);
					AddHtml( 70+xm, 272+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Por ) + "><RIGHT>Por</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(145+xm, 270+ym, 252, 252, 16, GumpButtonType.Reply, 0);
					AddButton(164+xm, 271+ym, Button( mBag.m_Selected_Por ), Button( mBag.m_Selected_Por ), 116, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Quas )
				{
					AddItem(222+xm, 274+ym, 0x2380);
					AddHtml( 184+xm, 272+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Quas ) + "><RIGHT>Quas</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(260+xm, 270+ym, 252, 252, 17, GumpButtonType.Reply, 0);
					AddButton(279+xm, 271+ym, Button( mBag.m_Selected_Quas ), Button( mBag.m_Selected_Quas ), 117, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Rel )
				{
					AddItem(336+xm, 274+ym, 0x2381);
					AddHtml( 298+xm, 272+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Rel ) + "><RIGHT>Rel</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(374+xm, 270+ym, 252, 252, 18, GumpButtonType.Reply, 0);
					AddButton(393+xm, 271+ym, Button( mBag.m_Selected_Rel ), Button( mBag.m_Selected_Rel ), 118, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Sanct )
				{
					AddItem(108+xm, 302+ym, 0x2382);
					AddHtml( 70+xm, 300+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Sanct ) + "><RIGHT>Sanct</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(145+xm, 300+ym, 252, 252, 19, GumpButtonType.Reply, 0);
					AddButton(164+xm, 301+ym, Button( mBag.m_Selected_Sanct ), Button( mBag.m_Selected_Sanct ), 119, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Tym )
				{
					AddItem(222+xm, 302+ym, 0x2383);
					AddHtml( 184+xm, 300+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Tym ) + "><RIGHT>Tym</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(260+xm, 300+ym, 252, 252, 20, GumpButtonType.Reply, 0);
					AddButton(279+xm, 301+ym, Button( mBag.m_Selected_Tym ), Button( mBag.m_Selected_Tym ), 120, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Uus )
				{
					AddItem(336+xm, 302+ym, 0x2384);
					AddHtml( 298+xm, 300+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Uus ) + "><RIGHT>Uus</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(374+xm, 300+ym, 252, 252, 21, GumpButtonType.Reply, 0);
					AddButton(393+xm, 301+ym, Button( mBag.m_Selected_Uus ), Button( mBag.m_Selected_Uus ), 121, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Vas )
				{
					AddItem(108+xm, 331+ym, 0x2385);
					AddHtml( 70+xm, 329+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Vas ) + "><RIGHT>Vas</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(145+xm, 330+ym, 252, 252, 22, GumpButtonType.Reply, 0);
					AddButton(164+xm, 331+ym, Button( mBag.m_Selected_Vas ), Button( mBag.m_Selected_Vas ), 122, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Wis )
				{
					AddItem(222+xm, 331+ym, 0x2399);
					AddHtml( 184+xm, 329+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Wis ) + "><RIGHT>Wis</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(260+xm, 330+ym, 252, 252, 23, GumpButtonType.Reply, 0);
					AddButton(279+xm, 331+ym, Button( mBag.m_Selected_Wis ), Button( mBag.m_Selected_Wis ), 123, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Xen )
				{
					AddItem(336+xm, 331+ym, 0x239C);
					AddHtml( 298+xm, 329+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Xen ) + "><RIGHT>Xen</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(374+xm, 330+ym, 252, 252, 24, GumpButtonType.Reply, 0);
					AddButton(393+xm, 331+ym, Button( mBag.m_Selected_Xen ), Button( mBag.m_Selected_Xen ), 124, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Ylem )
				{
					AddItem(108+xm, 362+ym, 0x239D);
					AddHtml( 70+xm, 360+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_Ylem ) + "><RIGHT>Ylem</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(145+xm, 360+ym, 252, 252, 25, GumpButtonType.Reply, 0);
					AddButton(164+xm, 361+ym, Button( mBag.m_Selected_Ylem ), Button( mBag.m_Selected_Ylem ), 125, GumpButtonType.Reply, 0);
				}
				if ( mBag.m_Rune_Zu )
				{
					AddItem(222+xm, 362+ym, 0x239E);
					AddHtml( 184+xm, 360+ym, 50, 16, @"<BODY><BASEFONT Color=" + Colors( mBag.m_Selected_An ) + "><RIGHT>Zu</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(260+xm, 360+ym, 252, 252, 26, GumpButtonType.Reply, 0);
					AddButton(279+xm, 361+ym, Button( mBag.m_Selected_Zu ), Button( mBag.m_Selected_Zu ), 126, GumpButtonType.Reply, 0);
				}

				AddButton(447, 11, 4017, 4017, 0, GumpButtonType.Reply, 0);
				AddButton(448, 515, 4011, 4011, 999, GumpButtonType.Reply, 0);
			}

			public int Button( bool val )
			{
				if ( val )
					return 209;

				return 208;
			}

			public string Colors( bool val )
			{
				if ( val )
					return "#17F61E";

				return "#FFC100";
			}

			public override void OnResponse( NetState state, RelayInfo info ) 
			{
				if ( info.ButtonID == 999 )
				{
					Item item = new RuneJournal();
					item.Weight = -50.0;
					item.OnDoubleClick( mFrom );
					item.Delete();
				}
				else if ( info.ButtonID == 1 ){ mBag.m_Rune_An = false; mBag.m_Selected_An = false; mFrom.AddToBackpack( new An() ); }
				else if ( info.ButtonID == 2 ){ mBag.m_Rune_Bet = false; mBag.m_Selected_Bet = false; mFrom.AddToBackpack( new Bet() ); }
				else if ( info.ButtonID == 3 ){ mBag.m_Rune_Corp = false; mBag.m_Selected_Corp = false; mFrom.AddToBackpack( new Corp() ); }
				else if ( info.ButtonID == 4 ){ mBag.m_Rune_Des = false; mBag.m_Selected_Des = false; mFrom.AddToBackpack( new Des() ); }
				else if ( info.ButtonID == 5 ){ mBag.m_Rune_Ex = false; mBag.m_Selected_Ex = false; mFrom.AddToBackpack( new Ex() ); }
				else if ( info.ButtonID == 6 ){ mBag.m_Rune_Flam = false; mBag.m_Selected_Flam = false; mFrom.AddToBackpack( new Flam() ); }
				else if ( info.ButtonID == 7 ){ mBag.m_Rune_Grav = false; mBag.m_Selected_Grav = false; mFrom.AddToBackpack( new Grav() ); }
				else if ( info.ButtonID == 8 ){ mBag.m_Rune_Hur = false; mBag.m_Selected_Hur = false; mFrom.AddToBackpack( new Hur() ); }
				else if ( info.ButtonID == 9 ){ mBag.m_Rune_In = false; mBag.m_Selected_In = false; mFrom.AddToBackpack( new In() ); }
				else if ( info.ButtonID == 10 ){ mBag.m_Rune_Jux = false; mBag.m_Selected_Jux = false; mFrom.AddToBackpack( new Jux() ); }
				else if ( info.ButtonID == 11 ){ mBag.m_Rune_Kal = false; mBag.m_Selected_Kal = false; mFrom.AddToBackpack( new Kal() ); }
				else if ( info.ButtonID == 12 ){ mBag.m_Rune_Lor = false; mBag.m_Selected_Lor = false; mFrom.AddToBackpack( new Lor() ); }
				else if ( info.ButtonID == 13 ){ mBag.m_Rune_Mani = false; mBag.m_Selected_Mani = false; mFrom.AddToBackpack( new Mani() ); }
				else if ( info.ButtonID == 14 ){ mBag.m_Rune_Nox = false; mBag.m_Selected_Nox = false; mFrom.AddToBackpack( new Nox() ); }
				else if ( info.ButtonID == 15 ){ mBag.m_Rune_Ort = false; mBag.m_Selected_Ort = false; mFrom.AddToBackpack( new Ort() ); }
				else if ( info.ButtonID == 16 ){ mBag.m_Rune_Por = false; mBag.m_Selected_Por = false; mFrom.AddToBackpack( new Por() ); }
				else if ( info.ButtonID == 17 ){ mBag.m_Rune_Quas = false; mBag.m_Selected_Quas = false; mFrom.AddToBackpack( new Quas() ); }
				else if ( info.ButtonID == 18 ){ mBag.m_Rune_Rel = false; mBag.m_Selected_Rel = false; mFrom.AddToBackpack( new Rel() ); }
				else if ( info.ButtonID == 19 ){ mBag.m_Rune_Sanct = false; mBag.m_Selected_Sanct = false; mFrom.AddToBackpack( new Sanct() ); }
				else if ( info.ButtonID == 20 ){ mBag.m_Rune_Tym = false; mBag.m_Selected_Tym = false; mFrom.AddToBackpack( new Tym() ); }
				else if ( info.ButtonID == 21 ){ mBag.m_Rune_Uus = false; mBag.m_Selected_Uus = false; mFrom.AddToBackpack( new Uus() ); }
				else if ( info.ButtonID == 22 ){ mBag.m_Rune_Vas = false; mBag.m_Selected_Vas = false; mFrom.AddToBackpack( new Vas() ); }
				else if ( info.ButtonID == 23 ){ mBag.m_Rune_Wis = false; mBag.m_Selected_Wis = false; mFrom.AddToBackpack( new Wis() ); }
				else if ( info.ButtonID == 24 ){ mBag.m_Rune_Xen = false; mBag.m_Selected_Xen = false; mFrom.AddToBackpack( new Xen() ); }
				else if ( info.ButtonID == 25 ){ mBag.m_Rune_Ylem = false; mBag.m_Selected_Ylem = false; mFrom.AddToBackpack( new Ylem() ); }
				else if ( info.ButtonID == 26 ){ mBag.m_Rune_Zu = false; mBag.m_Selected_Zu = false; mFrom.AddToBackpack( new Zu() ); }
				else if ( info.ButtonID == 101 ){ if ( mBag.m_Selected_An ){ mBag.m_Selected_An = false; } else { mBag.m_Selected_An = true; } } 
				else if ( info.ButtonID == 102 ){ if ( mBag.m_Selected_Bet ){ mBag.m_Selected_Bet = false; } else { mBag.m_Selected_Bet = true; } } 
				else if ( info.ButtonID == 103 ){ if ( mBag.m_Selected_Corp ){ mBag.m_Selected_Corp = false; } else { mBag.m_Selected_Corp = true; } } 
				else if ( info.ButtonID == 104 ){ if ( mBag.m_Selected_Des ){ mBag.m_Selected_Des = false; } else { mBag.m_Selected_Des = true; } } 
				else if ( info.ButtonID == 105 ){ if ( mBag.m_Selected_Ex ){ mBag.m_Selected_Ex = false; } else { mBag.m_Selected_Ex = true; } } 
				else if ( info.ButtonID == 106 ){ if ( mBag.m_Selected_Flam ){ mBag.m_Selected_Flam = false; } else { mBag.m_Selected_Flam = true; } } 
				else if ( info.ButtonID == 107 ){ if ( mBag.m_Selected_Grav ){ mBag.m_Selected_Grav = false; } else { mBag.m_Selected_Grav = true; } } 
				else if ( info.ButtonID == 108 ){ if ( mBag.m_Selected_Hur ){ mBag.m_Selected_Hur = false; } else { mBag.m_Selected_Hur = true; } } 
				else if ( info.ButtonID == 109 ){ if ( mBag.m_Selected_In ){ mBag.m_Selected_In = false; } else { mBag.m_Selected_In = true; } } 
				else if ( info.ButtonID == 110 ){ if ( mBag.m_Selected_Jux ){ mBag.m_Selected_Jux = false; } else { mBag.m_Selected_Jux = true; } } 
				else if ( info.ButtonID == 111 ){ if ( mBag.m_Selected_Kal ){ mBag.m_Selected_Kal = false; } else { mBag.m_Selected_Kal = true; } } 
				else if ( info.ButtonID == 112 ){ if ( mBag.m_Selected_Lor ){ mBag.m_Selected_Lor = false; } else { mBag.m_Selected_Lor = true; } } 
				else if ( info.ButtonID == 113 ){ if ( mBag.m_Selected_Mani ){ mBag.m_Selected_Mani = false; } else { mBag.m_Selected_Mani = true; } } 
				else if ( info.ButtonID == 114 ){ if ( mBag.m_Selected_Nox ){ mBag.m_Selected_Nox = false; } else { mBag.m_Selected_Nox = true; } } 
				else if ( info.ButtonID == 115 ){ if ( mBag.m_Selected_Ort ){ mBag.m_Selected_Ort = false; } else { mBag.m_Selected_Ort = true; } } 
				else if ( info.ButtonID == 116 ){ if ( mBag.m_Selected_Por ){ mBag.m_Selected_Por = false; } else { mBag.m_Selected_Por = true; } } 
				else if ( info.ButtonID == 117 ){ if ( mBag.m_Selected_Quas ){ mBag.m_Selected_Quas = false; } else { mBag.m_Selected_Quas = true; } } 
				else if ( info.ButtonID == 118 ){ if ( mBag.m_Selected_Rel ){ mBag.m_Selected_Rel = false; } else { mBag.m_Selected_Rel = true; } } 
				else if ( info.ButtonID == 119 ){ if ( mBag.m_Selected_Sanct ){ mBag.m_Selected_Sanct = false; } else { mBag.m_Selected_Sanct = true; } } 
				else if ( info.ButtonID == 120 ){ if ( mBag.m_Selected_Tym ){ mBag.m_Selected_Tym = false; } else { mBag.m_Selected_Tym = true; } } 
				else if ( info.ButtonID == 121 ){ if ( mBag.m_Selected_Uus ){ mBag.m_Selected_Uus = false; } else { mBag.m_Selected_Uus = true; } } 
				else if ( info.ButtonID == 122 ){ if ( mBag.m_Selected_Vas ){ mBag.m_Selected_Vas = false; } else { mBag.m_Selected_Vas = true; } } 
				else if ( info.ButtonID == 123 ){ if ( mBag.m_Selected_Wis ){ mBag.m_Selected_Wis = false; } else { mBag.m_Selected_Wis = true; } } 
				else if ( info.ButtonID == 124 ){ if ( mBag.m_Selected_Xen ){ mBag.m_Selected_Xen = false; } else { mBag.m_Selected_Xen = true; } } 
				else if ( info.ButtonID == 125 ){ if ( mBag.m_Selected_Ylem ){ mBag.m_Selected_Ylem = false; } else { mBag.m_Selected_Ylem = true; } } 
				else if ( info.ButtonID == 126 ){ if ( mBag.m_Selected_Zu ){ mBag.m_Selected_Zu = false; } else { mBag.m_Selected_Zu = true; } } 

				if ( info.ButtonID > 0 )
				{
					SetSpell();
					mFrom.SendSound( 0x4A );
					mFrom.CloseGump( typeof( MagicRuneGump ) );
					mFrom.SendGump( new MagicRuneGump( mFrom, mBag ) );
				}
				else { mFrom.PlaySound( 0x48 ); }
			}

			public void SetSpell()
			{
				if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Clumsy ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,true,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,true,false ) ){ mBag.MagicSpellChanged( MagicSpell.CreateFood ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,true,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Feeblemind ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,true,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Heal ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,true,false ) ){ mBag.MagicSpellChanged( MagicSpell.MagicArrow ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,true,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.NightSight ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.ReactiveArmor ); }
				else if ( SpellCheck( mBag,false,false,false,true,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Weaken ); }
				else if ( SpellCheck( mBag,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Agility ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,true,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Cunning ); }
				else if ( SpellCheck( mBag,true,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Cure ); }
				else if ( SpellCheck( mBag,true,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Harm ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,true,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.MagicTrap ); }
				else if ( SpellCheck( mBag,true,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.RemoveTrap ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,true,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Protection ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,true,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Strength ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,true,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Bless ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Fireball ); }
				else if ( SpellCheck( mBag,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.MagicLock ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,true,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Poison ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,true,false,false,false,false,false,false,false,false,true,false ) ){ mBag.MagicSpellChanged( MagicSpell.Telekinesis ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,true,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Teleport ); }
				else if ( SpellCheck( mBag,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Unlock ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,true,false ) ){ mBag.MagicSpellChanged( MagicSpell.WallOfStone ); }
				else if ( SpellCheck( mBag,true,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,true,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.ArchCure ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,true,true,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.ArchProtection ); }
				else if ( SpellCheck( mBag,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Curse ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,true,true,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.FireField ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,true,false,false,false,true,false,false,false,false,false,false,false,false,true,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.GreaterHeal ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,true,false,false,false,false,false,false,false,true,true,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Lightning ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,true,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.ManaDrain ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,true,false,false,false,true,true,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Recall ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,true,true,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false ) ){ mBag.MagicSpellChanged( MagicSpell.BladeSpirits ); }
				else if ( SpellCheck( mBag,true,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.DispelField ); }
				else if ( SpellCheck( mBag,false,false,false,false,true,false,false,false,true,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Incognito ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,true,true,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.MagicReflect ); }
				else if ( SpellCheck( mBag,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,true,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.MindBlast ); }
				else if ( SpellCheck( mBag,true,false,false,false,true,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Paralyze ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,true,false,true,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.PoisonField ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.SummonCreature ); }
				else if ( SpellCheck( mBag,true,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Dispel ); }
				else if ( SpellCheck( mBag,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.EnergyBolt ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,true,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,true,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Explosion ); }
				else if ( SpellCheck( mBag,true,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,true,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Invisibility ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,true,false,false,false,false,false,false,false,false,true,false ) ){ mBag.MagicSpellChanged( MagicSpell.Mark ); }
				else if ( SpellCheck( mBag,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,true,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.MassCurse ); }
				else if ( SpellCheck( mBag,false,false,false,false,true,false,true,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.ParalyzeField ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,true,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Reveal ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,true,false,false,false,false,false,false,false,true,false,false,false,false,false,false,true,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.ChainLightning ); }
				else if ( SpellCheck( mBag,true,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.EnergyField ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,true,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.FlameStrike ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,true,false,false,false,true,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.GateTravel ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,true,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.ManaVampire ); }
				else if ( SpellCheck( mBag,true,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,true,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.MassDispel ); }
				else if ( SpellCheck( mBag,false,false,false,true,false,true,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false ) ){ mBag.MagicSpellChanged( MagicSpell.MeteorSwarm ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,true,false,false,true,false ) ){ mBag.MagicSpellChanged( MagicSpell.Polymorph ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,true,false,false,false,false,false,true,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Earthquake ); }
				else if ( SpellCheck( mBag,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,true,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.EnergyVortex ); }
				else if ( SpellCheck( mBag,true,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Resurrection ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,true,false,false,true,false,false,false,false,false,false,false,false,false,false,true,false,true,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.AirElemental ); }
				else if ( SpellCheck( mBag,false,false,true,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,true,false,true,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.SummonDaemon ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,true,false,true,true,false ) ){ mBag.MagicSpellChanged( MagicSpell.EarthElemental ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,true,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,true,false,true,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.FireElemental ); }
				else if ( SpellCheck( mBag,true,false,false,false,false,true,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,true,false,true,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.WaterElemental ); }
				else if ( SpellCheck( mBag,true,false,true,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.CurseWeapon ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,true,true,false,false,true,false,false,false,false,false,false,false,false,false,false,true,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.BloodOath ); }
				else if ( SpellCheck( mBag,true,false,true,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false ) ){ mBag.MagicSpellChanged( MagicSpell.CorpseSkin ); }
				else if ( SpellCheck( mBag,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,true,true,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.EvilOmen ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.PainSpike ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,true,false,false,true,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.WraithForm ); }
				else if ( SpellCheck( mBag,true,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.MindRot ); }
				else if ( SpellCheck( mBag,false,true,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.SummonFamiliar ); }
				else if ( SpellCheck( mBag,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.AnimateDead ); }
				else if ( SpellCheck( mBag,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,true,false,true,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.HorrificBeast ); }
				else if ( SpellCheck( mBag,false,false,false,false,false,false,false,false,true,false,false,false,false,true,false,false,false,false,false,false,false,true,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.PoisonStrike ); }
				else if ( SpellCheck( mBag,true,false,false,false,false,true,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Wither ); }
				else if ( SpellCheck( mBag,false,true,false,false,false,false,false,false,true,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Strangle ); }
				else if ( SpellCheck( mBag,false,false,true,false,false,false,false,false,false,false,false,false,false,false,true,false,false,true,false,false,false,false,false,true,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.LichForm ); }
				else if ( SpellCheck( mBag,false,false,true,false,false,false,true,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.Exorcism ); }
				else if ( SpellCheck( mBag,false,true,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,true,false,true ) ){ mBag.MagicSpellChanged( MagicSpell.VengefulSpirit ); }
				else if ( SpellCheck( mBag,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,true,false,false,false,false,true,false,false ) ){ mBag.MagicSpellChanged( MagicSpell.VampiricEmbrace ); }
				else { mBag.MagicSpellChanged( MagicSpell.None ); }
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{          		
			if ( from != null && dropped is RuneStone )
			{
				bool var = ((RuneStone)dropped).PutInBag( this, from );

				if ( var )
					dropped.Delete();

				return var;
			}
			else if ( dropped is Gold )
			{
				BaseVendor mage = null;

				foreach ( Mobile m in from.GetMobilesInRange( 10 ) )
				{
					if ( BaseVendor.MagicRecharge( m ) && m is BaseVendor)
						mage = (BaseVendor)m;
				}

				if ( mage == null )
					return false;
				else
				{
					int uses = EnchantUses;
					int maxs = EnchantUsesMax;
					int rpt = 0;
					if ( uses > EnchantUsesMax )
						return false;
					else
					{
						int chg = EnchantUsesMax - uses;
						int gold = dropped.Amount;

						if ( chg >= gold )
						{
							rpt = gold;
							EnchantUses += gold;
						}
						else
						{
							EnchantUses = EnchantUsesMax;
							gold -= chg;
							rpt = chg;
							from.AddToBackpack( new Gold( gold ) );
						}

                        Effects.PlaySound(from.Location, from.Map, 0x5C1);

						mage.AddToCoinPurse( from, rpt );
						mage.InvalidateProperties();
						mage.SayTo( from, "I recharged the rune bag for " + rpt + " gold." );

						dropped.Delete();
						return true;
					}
				}
			}

			return false;
		}

		public static bool SpellCheck( MagicRuneBag bag, bool x_an, bool x_bet, bool x_corp, bool x_des, bool x_ex, bool x_flam, bool x_grav, bool x_hur, bool x_in, bool x_jux, bool x_kal, bool x_lor, bool x_mani, bool x_nox, bool x_ort, bool x_por, bool x_quas, bool x_rel, bool x_sanct, bool x_tym, bool x_uus, bool x_vas, bool x_wis, bool x_xen, bool x_ylem, bool x_zu )
		{
			if ( bag.m_Selected_An == x_an && 
			bag.m_Selected_Bet == x_bet && 
			bag.m_Selected_Corp == x_corp && 
			bag.m_Selected_Des == x_des && 
			bag.m_Selected_Ex == x_ex && 
			bag.m_Selected_Flam == x_flam && 
			bag.m_Selected_Grav == x_grav && 
			bag.m_Selected_Hur == x_hur && 
			bag.m_Selected_In == x_in && 
			bag.m_Selected_Jux == x_jux && 
			bag.m_Selected_Kal == x_kal && 
			bag.m_Selected_Lor == x_lor && 
			bag.m_Selected_Mani == x_mani && 
			bag.m_Selected_Nox == x_nox && 
			bag.m_Selected_Ort == x_ort && 
			bag.m_Selected_Por == x_por && 
			bag.m_Selected_Quas == x_quas && 
			bag.m_Selected_Rel == x_rel && 
			bag.m_Selected_Sanct == x_sanct && 
			bag.m_Selected_Tym == x_tym && 
			bag.m_Selected_Uus == x_uus && 
			bag.m_Selected_Vas == x_vas && 
			bag.m_Selected_Wis == x_wis && 
			bag.m_Selected_Xen == x_xen && 
			bag.m_Selected_Ylem == x_ylem && 
			bag.m_Selected_Zu == x_zu )
				return true;

			return false;
		}

		public MagicRuneBag( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
			writer.Write( m_Rune_An );
			writer.Write( m_Rune_Bet );
			writer.Write( m_Rune_Corp );
			writer.Write( m_Rune_Des );
			writer.Write( m_Rune_Ex );
			writer.Write( m_Rune_Flam );
			writer.Write( m_Rune_Grav );
			writer.Write( m_Rune_Hur );
			writer.Write( m_Rune_In );
			writer.Write( m_Rune_Jux );
			writer.Write( m_Rune_Kal );
			writer.Write( m_Rune_Lor );
			writer.Write( m_Rune_Mani );
			writer.Write( m_Rune_Nox );
			writer.Write( m_Rune_Ort );
			writer.Write( m_Rune_Por );
			writer.Write( m_Rune_Quas );
			writer.Write( m_Rune_Rel );
			writer.Write( m_Rune_Sanct );
			writer.Write( m_Rune_Tym );
			writer.Write( m_Rune_Uus );
			writer.Write( m_Rune_Vas );
			writer.Write( m_Rune_Wis );
			writer.Write( m_Rune_Xen );
			writer.Write( m_Rune_Ylem );
			writer.Write( m_Rune_Zu );
			writer.Write( m_Selected_An );
			writer.Write( m_Selected_Bet );
			writer.Write( m_Selected_Corp );
			writer.Write( m_Selected_Des );
			writer.Write( m_Selected_Ex );
			writer.Write( m_Selected_Flam );
			writer.Write( m_Selected_Grav );
			writer.Write( m_Selected_Hur );
			writer.Write( m_Selected_In );
			writer.Write( m_Selected_Jux );
			writer.Write( m_Selected_Kal );
			writer.Write( m_Selected_Lor );
			writer.Write( m_Selected_Mani );
			writer.Write( m_Selected_Nox );
			writer.Write( m_Selected_Ort );
			writer.Write( m_Selected_Por );
			writer.Write( m_Selected_Quas );
			writer.Write( m_Selected_Rel );
			writer.Write( m_Selected_Sanct );
			writer.Write( m_Selected_Tym );
			writer.Write( m_Selected_Uus );
			writer.Write( m_Selected_Vas );
			writer.Write( m_Selected_Wis );
			writer.Write( m_Selected_Xen );
			writer.Write( m_Selected_Ylem );
			writer.Write( m_Selected_Zu );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_Rune_An = reader.ReadBool();
			m_Rune_Bet = reader.ReadBool();
			m_Rune_Corp = reader.ReadBool();
			m_Rune_Des = reader.ReadBool();
			m_Rune_Ex = reader.ReadBool();
			m_Rune_Flam = reader.ReadBool();
			m_Rune_Grav = reader.ReadBool();
			m_Rune_Hur = reader.ReadBool();
			m_Rune_In = reader.ReadBool();
			m_Rune_Jux = reader.ReadBool();
			m_Rune_Kal = reader.ReadBool();
			m_Rune_Lor = reader.ReadBool();
			m_Rune_Mani = reader.ReadBool();
			m_Rune_Nox = reader.ReadBool();
			m_Rune_Ort = reader.ReadBool();
			m_Rune_Por = reader.ReadBool();
			m_Rune_Quas = reader.ReadBool();
			m_Rune_Rel = reader.ReadBool();
			m_Rune_Sanct = reader.ReadBool();
			m_Rune_Tym = reader.ReadBool();
			m_Rune_Uus = reader.ReadBool();
			m_Rune_Vas = reader.ReadBool();
			m_Rune_Wis = reader.ReadBool();
			m_Rune_Xen = reader.ReadBool();
			m_Rune_Ylem = reader.ReadBool();
			m_Rune_Zu = reader.ReadBool();
			m_Selected_An = reader.ReadBool();
			m_Selected_Bet = reader.ReadBool();
			m_Selected_Corp = reader.ReadBool();
			m_Selected_Des = reader.ReadBool();
			m_Selected_Ex = reader.ReadBool();
			m_Selected_Flam = reader.ReadBool();
			m_Selected_Grav = reader.ReadBool();
			m_Selected_Hur = reader.ReadBool();
			m_Selected_In = reader.ReadBool();
			m_Selected_Jux = reader.ReadBool();
			m_Selected_Kal = reader.ReadBool();
			m_Selected_Lor = reader.ReadBool();
			m_Selected_Mani = reader.ReadBool();
			m_Selected_Nox = reader.ReadBool();
			m_Selected_Ort = reader.ReadBool();
			m_Selected_Por = reader.ReadBool();
			m_Selected_Quas = reader.ReadBool();
			m_Selected_Rel = reader.ReadBool();
			m_Selected_Sanct = reader.ReadBool();
			m_Selected_Tym = reader.ReadBool();
			m_Selected_Uus = reader.ReadBool();
			m_Selected_Vas = reader.ReadBool();
			m_Selected_Wis = reader.ReadBool();
			m_Selected_Xen = reader.ReadBool();
			m_Selected_Ylem = reader.ReadBool();
			m_Selected_Zu = reader.ReadBool();
		}

		public bool m_Rune_An;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_An { get { return m_Rune_An; } set { m_Rune_An = value; InvalidateProperties(); } }

		public bool m_Rune_Bet;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Bet { get { return m_Rune_Bet; } set { m_Rune_Bet = value; InvalidateProperties(); } }

		public bool m_Rune_Corp;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Corp { get { return m_Rune_Corp; } set { m_Rune_Corp = value; InvalidateProperties(); } }

		public bool m_Rune_Des;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Des { get { return m_Rune_Des; } set { m_Rune_Des = value; InvalidateProperties(); } }

		public bool m_Rune_Ex;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Ex { get { return m_Rune_Ex; } set { m_Rune_Ex = value; InvalidateProperties(); } }

		public bool m_Rune_Flam;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Flam { get { return m_Rune_Flam; } set { m_Rune_Flam = value; InvalidateProperties(); } }

		public bool m_Rune_Grav;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Grav { get { return m_Rune_Grav; } set { m_Rune_Grav = value; InvalidateProperties(); } }

		public bool m_Rune_Hur;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Hur { get { return m_Rune_Hur; } set { m_Rune_Hur = value; InvalidateProperties(); } }

		public bool m_Rune_In;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_In { get { return m_Rune_In; } set { m_Rune_In = value; InvalidateProperties(); } }

		public bool m_Rune_Jux;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Jux { get { return m_Rune_Jux; } set { m_Rune_Jux = value; InvalidateProperties(); } }

		public bool m_Rune_Kal;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Kal { get { return m_Rune_Kal; } set { m_Rune_Kal = value; InvalidateProperties(); } }

		public bool m_Rune_Lor;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Lor { get { return m_Rune_Lor; } set { m_Rune_Lor = value; InvalidateProperties(); } }

		public bool m_Rune_Mani;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Mani { get { return m_Rune_Mani; } set { m_Rune_Mani = value; InvalidateProperties(); } }

		public bool m_Rune_Nox;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Nox { get { return m_Rune_Nox; } set { m_Rune_Nox = value; InvalidateProperties(); } }

		public bool m_Rune_Ort;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Ort { get { return m_Rune_Ort; } set { m_Rune_Ort = value; InvalidateProperties(); } }

		public bool m_Rune_Por;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Por { get { return m_Rune_Por; } set { m_Rune_Por = value; InvalidateProperties(); } }

		public bool m_Rune_Quas;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Quas { get { return m_Rune_Quas; } set { m_Rune_Quas = value; InvalidateProperties(); } }

		public bool m_Rune_Rel;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Rel { get { return m_Rune_Rel; } set { m_Rune_Rel = value; InvalidateProperties(); } }

		public bool m_Rune_Sanct;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Sanct { get { return m_Rune_Sanct; } set { m_Rune_Sanct = value; InvalidateProperties(); } }

		public bool m_Rune_Tym;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Tym { get { return m_Rune_Tym; } set { m_Rune_Tym = value; InvalidateProperties(); } }

		public bool m_Rune_Uus;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Uus { get { return m_Rune_Uus; } set { m_Rune_Uus = value; InvalidateProperties(); } }

		public bool m_Rune_Vas;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Vas { get { return m_Rune_Vas; } set { m_Rune_Vas = value; InvalidateProperties(); } }

		public bool m_Rune_Wis;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Wis { get { return m_Rune_Wis; } set { m_Rune_Wis = value; InvalidateProperties(); } }

		public bool m_Rune_Xen;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Xen { get { return m_Rune_Xen; } set { m_Rune_Xen = value; InvalidateProperties(); } }

		public bool m_Rune_Ylem;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Ylem { get { return m_Rune_Ylem; } set { m_Rune_Ylem = value; InvalidateProperties(); } }

		public bool m_Rune_Zu;
		[CommandProperty(AccessLevel.Owner)]
		public bool Rune_Zu { get { return m_Rune_Zu; } set { m_Rune_Zu = value; InvalidateProperties(); } }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public bool m_Selected_An;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_An { get { return m_Selected_An; } set { m_Selected_An = value; InvalidateProperties(); } }

		public bool m_Selected_Bet;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Bet { get { return m_Selected_Bet; } set { m_Selected_Bet = value; InvalidateProperties(); } }

		public bool m_Selected_Corp;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Corp { get { return m_Selected_Corp; } set { m_Selected_Corp = value; InvalidateProperties(); } }

		public bool m_Selected_Des;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Des { get { return m_Selected_Des; } set { m_Selected_Des = value; InvalidateProperties(); } }

		public bool m_Selected_Ex;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Ex { get { return m_Selected_Ex; } set { m_Selected_Ex = value; InvalidateProperties(); } }

		public bool m_Selected_Flam;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Flam { get { return m_Selected_Flam; } set { m_Selected_Flam = value; InvalidateProperties(); } }

		public bool m_Selected_Grav;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Grav { get { return m_Selected_Grav; } set { m_Selected_Grav = value; InvalidateProperties(); } }

		public bool m_Selected_Hur;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Hur { get { return m_Selected_Hur; } set { m_Selected_Hur = value; InvalidateProperties(); } }

		public bool m_Selected_In;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_In { get { return m_Selected_In; } set { m_Selected_In = value; InvalidateProperties(); } }

		public bool m_Selected_Jux;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Jux { get { return m_Selected_Jux; } set { m_Selected_Jux = value; InvalidateProperties(); } }

		public bool m_Selected_Kal;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Kal { get { return m_Selected_Kal; } set { m_Selected_Kal = value; InvalidateProperties(); } }

		public bool m_Selected_Lor;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Lor { get { return m_Selected_Lor; } set { m_Selected_Lor = value; InvalidateProperties(); } }

		public bool m_Selected_Mani;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Mani { get { return m_Selected_Mani; } set { m_Selected_Mani = value; InvalidateProperties(); } }

		public bool m_Selected_Nox;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Nox { get { return m_Selected_Nox; } set { m_Selected_Nox = value; InvalidateProperties(); } }

		public bool m_Selected_Ort;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Ort { get { return m_Selected_Ort; } set { m_Selected_Ort = value; InvalidateProperties(); } }

		public bool m_Selected_Por;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Por { get { return m_Selected_Por; } set { m_Selected_Por = value; InvalidateProperties(); } }

		public bool m_Selected_Quas;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Quas { get { return m_Selected_Quas; } set { m_Selected_Quas = value; InvalidateProperties(); } }

		public bool m_Selected_Rel;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Rel { get { return m_Selected_Rel; } set { m_Selected_Rel = value; InvalidateProperties(); } }

		public bool m_Selected_Sanct;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Sanct { get { return m_Selected_Sanct; } set { m_Selected_Sanct = value; InvalidateProperties(); } }

		public bool m_Selected_Tym;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Tym { get { return m_Selected_Tym; } set { m_Selected_Tym = value; InvalidateProperties(); } }

		public bool m_Selected_Uus;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Uus { get { return m_Selected_Uus; } set { m_Selected_Uus = value; InvalidateProperties(); } }

		public bool m_Selected_Vas;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Vas { get { return m_Selected_Vas; } set { m_Selected_Vas = value; InvalidateProperties(); } }

		public bool m_Selected_Wis;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Wis { get { return m_Selected_Wis; } set { m_Selected_Wis = value; InvalidateProperties(); } }

		public bool m_Selected_Xen;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Xen { get { return m_Selected_Xen; } set { m_Selected_Xen = value; InvalidateProperties(); } }

		public bool m_Selected_Ylem;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Ylem { get { return m_Selected_Ylem; } set { m_Selected_Ylem = value; InvalidateProperties(); } }

		public bool m_Selected_Zu;
		[CommandProperty(AccessLevel.Owner)]
		public bool Selected_Zu { get { return m_Selected_Zu; } set { m_Selected_Zu = value; InvalidateProperties(); } }

		public static int RuneID( Item r )
		{
			int id = 0;

			if ( r is An ){ id = 0x2379; }
			else if ( r is Bet ){ id = 0x237A; }
			else if ( r is Corp ){ id = 0x237B; }
			else if ( r is Des ){ id = 0x237C; }
			else if ( r is Ex ){ id = 0x237D; }
			else if ( r is Flam ){ id = 0x2387; }
			else if ( r is Grav ){ id = 0x2389; }
			else if ( r is Hur ){ id = 0x238A; }
			else if ( r is In ){ id = 0x2393; }
			else if ( r is Jux ){ id = 0x2394; }
			else if ( r is Kal ){ id = 0x2395; }
			else if ( r is Lor ){ id = 0x2396; }
			else if ( r is Mani ){ id = 0x237E; }
			else if ( r is Nox ){ id = 0x238B; }
			else if ( r is Ort ){ id = 0x2398; }
			else if ( r is Por ){ id = 0x237F; }
			else if ( r is Quas ){ id = 0x2380; }
			else if ( r is Rel ){ id = 0x2381; }
			else if ( r is Sanct ){ id = 0x2382; }
			else if ( r is Tym ){ id = 0x2383; }
			else if ( r is Uus ){ id = 0x2384; }
			else if ( r is Vas ){ id = 0x2385; }
			else if ( r is Wis ){ id = 0x2399; }
			else if ( r is Xen ){ id = 0x239C; }
			else if ( r is Ylem ){ id = 0x239D; }
			else if ( r is Zu ){ id = 0x239E; }

			return id;
		}
	}

	public abstract class RuneStone : Item
	{
		public override bool DisplayWeight{ get{ return false; } }

		public RuneStone() : base( 0x1F15 )
		{
			Weight = 0.01;
			SetRune( this );
		}

		public RuneStone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public static void SetRune( RuneStone stone )
		{
			stone.ItemID = MagicRuneBag.RuneID( stone );
			stone.ColorText1 = "Magic Rune";
			stone.ColorHue1 = "1BC0DA";
			stone.ColorHue2 = "FEA9FF";

			if ( stone is An ){			stone.ColorText2 = "An";	}
			else if ( stone is Bet ){	stone.ColorText2 = "Bet";	}
			else if ( stone is Corp ){	stone.ColorText2 = "Corp";	}
			else if ( stone is Des ){	stone.ColorText2 = "Des";	}
			else if ( stone is Ex ){	stone.ColorText2 = "Ex";	}
			else if ( stone is Flam ){	stone.ColorText2 = "Flam";	}
			else if ( stone is Grav ){	stone.ColorText2 = "Grav";	}
			else if ( stone is Hur ){	stone.ColorText2 = "Hur";	}
			else if ( stone is In ){	stone.ColorText2 = "In";	}
			else if ( stone is Jux ){	stone.ColorText2 = "Jux";	}
			else if ( stone is Kal ){	stone.ColorText2 = "Kal";	}
			else if ( stone is Lor ){	stone.ColorText2 = "Lor";	}
			else if ( stone is Mani ){	stone.ColorText2 = "Mani";	}
			else if ( stone is Nox ){	stone.ColorText2 = "Nox";	}
			else if ( stone is Ort ){	stone.ColorText2 = "Ort";	}
			else if ( stone is Por ){	stone.ColorText2 = "Por";	}
			else if ( stone is Quas ){	stone.ColorText2 = "Quas";	}
			else if ( stone is Rel ){	stone.ColorText2 = "Rel";	}
			else if ( stone is Sanct ){	stone.ColorText2 = "Sanct";	}
			else if ( stone is Tym ){	stone.ColorText2 = "Tym";	}
			else if ( stone is Uus ){	stone.ColorText2 = "Uus";	}
			else if ( stone is Vas ){	stone.ColorText2 = "Vas";	}
			else if ( stone is Wis ){	stone.ColorText2 = "Wis";	}
			else if ( stone is Xen ){	stone.ColorText2 = "Xen";	}
			else if ( stone is Ylem ){	stone.ColorText2 = "Ylem";	}
			else if ( stone is Zu ){	stone.ColorText2 = "Zu";	}

			stone.Name = stone.ColorText2 + " Stone";
		}

		public bool PutInBag( MagicRuneBag bag, Mobile from )
		{
			bool success = false;

			if ( this is An				&&		!bag.Rune_An ){			bag.Rune_An = true;			success = true;		}
			else if ( this is Bet		&&		!bag.Rune_Bet ){		bag.Rune_Bet = true;		success = true;		}
			else if ( this is Corp		&&		!bag.Rune_Corp ){		bag.Rune_Corp = true;		success = true;		}
			else if ( this is Des		&&		!bag.Rune_Des ){		bag.Rune_Des = true;		success = true;		}
			else if ( this is Ex		&&		!bag.Rune_Ex ){			bag.Rune_Ex = true;			success = true;		}
			else if ( this is Flam		&&		!bag.Rune_Flam ){		bag.Rune_Flam = true;		success = true;		}
			else if ( this is Grav		&&		!bag.Rune_Grav ){		bag.Rune_Grav = true;		success = true;		}
			else if ( this is Hur		&&		!bag.Rune_Hur ){		bag.Rune_Hur = true;		success = true;		}
			else if ( this is In		&&		!bag.Rune_In ){			bag.Rune_In = true;			success = true;		}
			else if ( this is Jux		&&		!bag.Rune_Jux ){		bag.Rune_Jux = true;		success = true;		}
			else if ( this is Kal		&&		!bag.Rune_Kal ){		bag.Rune_Kal = true;		success = true;		}
			else if ( this is Lor		&&		!bag.Rune_Lor ){		bag.Rune_Lor = true;		success = true;		}
			else if ( this is Mani		&&		!bag.Rune_Mani ){		bag.Rune_Mani = true;		success = true;		}
			else if ( this is Nox		&&		!bag.Rune_Nox ){		bag.Rune_Nox = true;		success = true;		}
			else if ( this is Ort		&&		!bag.Rune_Ort ){		bag.Rune_Ort = true;		success = true;		}
			else if ( this is Por		&&		!bag.Rune_Por ){		bag.Rune_Por = true;		success = true;		}
			else if ( this is Quas		&&		!bag.Rune_Quas ){		bag.Rune_Quas = true;		success = true;		}
			else if ( this is Rel		&&		!bag.Rune_Rel ){		bag.Rune_Rel = true;		success = true;		}
			else if ( this is Sanct		&&		!bag.Rune_Sanct ){		bag.Rune_Sanct = true;		success = true;		}
			else if ( this is Tym		&&		!bag.Rune_Tym ){		bag.Rune_Tym = true;		success = true;		}
			else if ( this is Uus		&&		!bag.Rune_Uus ){		bag.Rune_Uus = true;		success = true;		}
			else if ( this is Vas		&&		!bag.Rune_Vas ){		bag.Rune_Vas = true;		success = true;		}
			else if ( this is Wis		&&		!bag.Rune_Wis ){		bag.Rune_Wis = true;		success = true;		}
			else if ( this is Xen		&&		!bag.Rune_Xen ){		bag.Rune_Xen = true;		success = true;		}
			else if ( this is Ylem		&&		!bag.Rune_Ylem ){		bag.Rune_Ylem = true;		success = true;		}
			else if ( this is Zu		&&		!bag.Rune_Zu ){			bag.Rune_Zu = true;			success = true;		}

			if ( success )
			{
				from.SendMessage( "You place the rune in the bag." );

				if ( from.HasGump( typeof( MagicRuneBag.MagicRuneGump ) ) )
				{
					from.CloseGump( typeof( MagicRuneBag.MagicRuneGump ) );
					from.SendGump( new MagicRuneBag.MagicRuneGump( from, bag ) );
				}
				from.PlaySound( 0x48 );
				return true;
			}

			from.SendMessage( "That rune is already in the bag." );
			return false;
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			SetRune( this );
		}
	}

	public class An : RuneStone
	{
		[Constructable]
		public An() : base()
		{
		}
	
		public An( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Bet : RuneStone
	{
		[Constructable]
		public Bet() : base()
		{
		}
	
		public Bet( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Corp : RuneStone
	{
		[Constructable]
		public Corp() : base()
		{
		}
	
		public Corp( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Des : RuneStone
	{
		[Constructable]
		public Des() : base()
		{
		}
	
		public Des( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Ex : RuneStone
	{
		[Constructable]
		public Ex() : base()
		{
		}
	
		public Ex( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Flam : RuneStone
	{
		[Constructable]
		public Flam() : base()
		{
		}
	
		public Flam( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Grav : RuneStone
	{
		[Constructable]
		public Grav() : base()
		{
		}
	
		public Grav( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Hur : RuneStone
	{
		[Constructable]
		public Hur() : base()
		{
		}
	
		public Hur( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class In : RuneStone
	{
		[Constructable]
		public In() : base()
		{
		}
	
		public In( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Jux : RuneStone
	{
		[Constructable]
		public Jux() : base()
		{
		}
	
		public Jux( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Kal : RuneStone
	{
		[Constructable]
		public Kal() : base()
		{
		}
	
		public Kal( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Lor : RuneStone
	{
		[Constructable]
		public Lor() : base()
		{
		}
	
		public Lor( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Mani : RuneStone
	{
		[Constructable]
		public Mani() : base()
		{
		}

		public Mani( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Nox : RuneStone
	{
		[Constructable]
		public Nox() : base()
		{
		}
	
		public Nox( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Ort : RuneStone
	{
		[Constructable]
		public Ort() : base()
		{
		}
	
		public Ort( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Por : RuneStone
	{
		[Constructable]
		public Por() : base()
		{
		}
	
		public Por( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Quas : RuneStone
	{
		[Constructable]
		public Quas() : base()
		{
		}
	
		public Quas( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Rel : RuneStone
	{
		[Constructable]
		public Rel() : base()
		{
		}
	
		public Rel( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Sanct : RuneStone
	{
		[Constructable]
		public Sanct() : base()
		{
		}
	
		public Sanct( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Tym : RuneStone
	{
		[Constructable]
		public Tym() : base()
		{
		}
	
		public Tym( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Uus : RuneStone
	{
		[Constructable]
		public Uus() : base()
		{
		}
	
		public Uus( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Vas : RuneStone
	{
		[Constructable]
		public Vas() : base()
		{
		}
	
		public Vas( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Wis : RuneStone
	{
		[Constructable]
		public Wis() : base()
		{
		}
	
		public Wis( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Xen : RuneStone
	{
		[Constructable]
		public Xen() : base()
		{
		}
	
		public Xen( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Ylem : RuneStone
	{
		[Constructable]
		public Ylem() : base()
		{
		}
	
		public Ylem( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Zu : RuneStone
	{
		[Constructable]
		public Zu() : base()
		{
		}
	
		public Zu( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}