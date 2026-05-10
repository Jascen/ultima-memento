using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Menus;
using Server.Menus.Questions;
using Server.Accounting;
using Server.Multis;
using Server.Mobiles;
using Server.Regions;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Misc;
using Server.Items;
using System.Globalization;
using Server.SpellBars;

namespace Server.Gumps 
{
    public class QuickBar : Gump
    {
		public int m_Origin;

		public static void Initialize()
		{
            CommandSystem.Register( "quickbar", AccessLevel.Player, new CommandEventHandler( ToolBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "quickbar" )]
		[Description( "Opens the Quick Bar." )]
		public static void ToolBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( QuickBar ) );
			from.SendGump( new QuickBar( from ) );
        }

		public QuickBar ( Mobile from ) : base ( 50, 50 )
		{
			int set1 = 0; 
			//int set2 = 0; 
			int set3 = 0; 
			int set4 = 0; 
			int set5 = 0; 
			int set6 = 0; 
			int set7 = 0; 
			int set8 = 0; 
			int set9 = 0; 
			int set10 = 0; 
			int set11 = 0; 
			int set12 = 0; 
			int set13 = 0; 
			int set14 = 0; 
			int set15 = 0; 
			int set16 = 0; 
			int set17 = 0; 
			int set18 = 0; 
			int set19 = 0; 
			int set20 = 0; 
			int set21 = 0; 
			int set22 = 0; 
			int set23 = 0; 
			int set24 = 0; 
			int set25 = 0; 
			int set26 = 0; 
			int set27 = 0; 
			int set28 = 0; 
			int set29 = 0; 
			int set30 = 0; 
			int set31 = 0; 
			int set32 = 0; 
			int set33 = 0; 
			int set34 = 0; 
			int set35 = 0; 
			int set36 = 0; 
			int set37 = 0; 
			int set38 = 0; 
			int set39 = 0; 
			int set40 = 0; 
			int set41 = 0; 

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);
			AddImage(0, 0, 10308); // GRAB ICON

			int bandageCount = from.Backpack.GetAmount( typeof( Bandage ), true );
			int arrowCount = from.Backpack.GetAmount( typeof( Arrow ), true );
			int boltCount = from.Backpack.GetAmount( typeof( Bolt ), true );
			int throwCount = from.Backpack.GetAmount( typeof( ThrowingWeapon ), true );
			int mageeyeCount = from.Backpack.GetAmount( typeof( MageEye ), true );
			int ropeCount = from.Backpack.GetAmount( typeof( HarpoonRope ), true );
			int krystalCount = from.Backpack.GetAmount( typeof( Krystal ), true );

			if ( from.FindItemOnLayer( Layer.Cloak ) != null )
			{
				Item myQuiver = from.FindItemOnLayer( Layer.Cloak );
				if ( myQuiver is BaseQuiver )
				{
					foreach( Item arrow in myQuiver.Items )
					{
						if ( arrow is Arrow ){ arrowCount = arrowCount + arrow.Amount; }
						if ( arrow is Bolt ){ boltCount = boltCount + arrow.Amount; }
						if ( arrow is ThrowingWeapon ){ throwCount = throwCount + arrow.Amount; }
						if ( arrow is MageEye ){ mageeyeCount = mageeyeCount + arrow.Amount; }
						if ( arrow is HarpoonRope ){ ropeCount = ropeCount + arrow.Amount; }
						if ( arrow is Krystal ){ krystalCount = krystalCount + arrow.Amount; }
					}
				}
			}

			string keys = PlayerSettings.ValQuickConfig( from );;

			if ( keys.Length > 0 )
			{
				string[] configures = keys.Split('#');
				int nEntry = 1;
				foreach (string key in configures)
				{
					if ( nEntry == 1 && key == "1" ){ set1 = 1; }
					//else if ( nEntry == 2 && key == "1" ){ set2 = 1; }
					else if ( nEntry == 3 && key == "1" ){ set3 = 1; }
					else if ( nEntry == 4 && key == "1" ){ set4 = 1; }
					else if ( nEntry == 5 && key == "1" ){ set5 = 1; }
					else if ( nEntry == 6 && key == "1" ){ set6 = 1; }
					else if ( nEntry == 7 && key == "1" ){ set7 = 1; }
					else if ( nEntry == 8 && key == "1" ){ set8 = 1; }
					else if ( nEntry == 9 && key == "1" ){ set9 = 1; }
					else if ( nEntry == 10 && key == "1" ){ set10 = 1; }
					else if ( nEntry == 11 && key == "1" ){ set11 = 1; }
					else if ( nEntry == 12 && key == "1" ){ set12 = 1; }
					else if ( nEntry == 13 && key == "1" ){ set13 = 1; }
					else if ( nEntry == 14 && key == "1" ){ set14 = 1; }
					else if ( nEntry == 15 && key == "1" ){ set15 = 1; }
					else if ( nEntry == 16 && key == "1" ){ set16 = 1; }
					else if ( nEntry == 17 && key == "1" ){ set17 = 1; }
					else if ( nEntry == 18 && key == "1" ){ set18 = 1; }
					else if ( nEntry == 19 && key == "1" ){ set19 = 1; }
					else if ( nEntry == 20 && key == "1" ){ set20 = 1; }
					else if ( nEntry == 21 && key == "1" ){ set21 = 1; }
					else if ( nEntry == 22 && key == "1" ){ set22 = 1; }
					else if ( nEntry == 23 && key == "1" ){ set23 = 1; }
					else if ( nEntry == 24 && key == "1" ){ set24 = 1; }
					else if ( nEntry == 25 && key == "1" ){ set25 = 1; }
					else if ( nEntry == 26 && key == "1" ){ set26 = 1; }
					else if ( nEntry == 27 && key == "1" ){ set27 = 1; }
					else if ( nEntry == 28 && key == "1" ){ set28 = 1; }
					else if ( nEntry == 29 && key == "1" ){ set29 = 1; }
					else if ( nEntry == 30 && key == "1" ){ set30 = 1; }
					else if ( nEntry == 31 && key == "1" ){ set31 = 1; }
					else if ( nEntry == 32 && key == "1" ){ set32 = 1; }
					else if ( nEntry == 33 && key == "1" ){ set33 = 1; }
					else if ( nEntry == 34 && key == "1" ){ set34 = 1; }
					else if ( nEntry == 35 && key == "1" ){ set35 = 1; }
					else if ( nEntry == 36 && key == "1" ){ set36 = 1; }
					else if ( nEntry == 37 && key == "1" ){ set37 = 1; }
					else if ( nEntry == 38 && key == "1" ){ set38 = 1; }
					else if ( nEntry == 39 && key == "1" ){ set39 = 1; }
					else if ( nEntry == 40 && key == "1" ){ set40 = 1; }
					else if ( nEntry == 41 && key == "1" ){ set41 = 1; }

					nEntry++;
				}
			}

			int i = 2;

			int v = 0;
			int w = 35;

			if ( set1 == 1 )
			{
				v = 35;
				w = 0;
			}

			int x = 0;
			int y = 0;

			int s = 0;

			x=x+v;
			y=y+w;

			AddButton(x, y, 10349, 10349, 666, GumpButtonType.Reply, 0); // HELP ICON

			bool showICON = false;

			while ( i < 41 )
			{
				i++;
				showICON = false;

				if ( i == 3 && set3 == 1 ){ showICON = true; }
				else if ( i == 4 && set4 == 1 ){ showICON = true; }
				else if ( i == 5 && set5 == 1 ){ showICON = true; }
				else if ( i == 6 && set6 == 1 ){ showICON = true; }
				else if ( i == 7 && set7 == 1 ){ showICON = true; }
				else if ( i == 8 && set8 == 1 ){ showICON = true; }
				else if ( i == 9 && set9 == 1 ){ showICON = true; }
				else if ( i == 10 && set10 == 1 ){ showICON = true; }
				else if ( i == 11 && set11 == 1 ){ showICON = true; }
				else if ( i == 12 && set12 == 1 ){ showICON = true; }
				else if ( i == 13 && set13 == 1 ){ showICON = true; }
				else if ( i == 14 && set14 == 1 ){ showICON = true; }
				else if ( i == 15 && set15 == 1 ){ showICON = true; }
				else if ( i == 16 && set16 == 1 ){ showICON = true; }
				else if ( i == 17 && set17 == 1 ){ showICON = true; }
				else if ( i == 18 && set18 == 1 ){ showICON = true; }
				else if ( i == 19 && set19 == 1 ){ showICON = true; }
				else if ( i == 20 && set20 == 1 ){ showICON = true; }
				else if ( i == 21 && set21 == 1 ){ showICON = true; }
				else if ( i == 22 && set22 == 1 ){ showICON = true; }
				else if ( i == 23 && set23 == 1 ){ showICON = true; }
				else if ( i == 24 && set24 == 1 ){ showICON = true; }
				else if ( i == 25 && set25 == 1 ){ showICON = true; }
				else if ( i == 26 && set26 == 1 ){ showICON = true; }
				else if ( i == 27 && set27 == 1 ){ showICON = true; }
				else if ( i == 28 && set28 == 1 ){ showICON = true; }
				else if ( i == 29 && set29 == 1 ){ showICON = true; }
				else if ( i == 30 && set30 == 1 ){ showICON = true; }
				else if ( i == 31 && set31 == 1 ){ showICON = true; }
				else if ( i == 32 && set32 == 1 ){ showICON = true; }
				else if ( i == 33 && set33 == 1 ){ showICON = true; }
				else if ( i == 34 && set34 == 1 ){ showICON = true; }
				else if ( i == 35 && set35 == 1 ){ showICON = true; }
				else if ( i == 36 && set36 == 1 ){ showICON = true; }
				else if ( i == 37 && set37 == 1 ){ showICON = true; }
				else if ( i == 38 && set38 == 1 ){ showICON = true; }
				else if ( i == 39 && set39 == 1 ){ showICON = true; }
				else if ( i == 40 && set40 == 1 ){ showICON = true; }
				else if ( i == 41 && set41 == 1 ){ showICON = true; }

				if ( showICON )
				{
					x=x+v;
					y=y+w;

					if ( i == 14 || ( i >= 16 && i <= 21 ) )
					{
						if ( i == 14 ){ s = bandageCount; }
						else if ( i == 16 ){ s = arrowCount; }
						else if ( i == 17 ){ s = boltCount; }
						else if ( i == 18 ){ s = ropeCount; }
						else if ( i == 19 ){ s = mageeyeCount; }
						else if ( i == 20 ){ s = krystalCount; }
						else if ( i == 21 ){ s = throwCount; }

						if ( v == 0 )
							AddHtml( x+35, y+7, 50, 20, @"<BODY><BASEFONT Color=#34ee39>" + s + "</BASEFONT></BODY>", (bool)false, (bool)false);
						else 
							AddHtml( x+4, 35, 50, 20, @"<BODY><BASEFONT Color=#34ee39>" + s + "</BASEFONT></BODY>", (bool)false, (bool)false);
					}

					int itemImage = QuickConfig.GetItemImage(i, from);

					if ( i >= 16 && i <= 21 )
					{
						AddImage( x, y, QuickConfig.rowNumber(i,from) );
					}
					else if (0 < itemImage)
					{
						const int BACKGROUND_IMAGE = 10351;
						Rectangle2D bounds = ItemBounds.Table[itemImage];
						AddButton(x, y, BACKGROUND_IMAGE, BACKGROUND_IMAGE, i, GumpButtonType.Reply, 0);
						AddItem(x + bounds.Width / 2 - bounds.X, y + bounds.Height / 2 - bounds.Y, itemImage);
					}
					else
					{
						AddButton(x, y, QuickConfig.rowNumber(i,from), QuickConfig.rowNumber(i,from), i, GumpButtonType.Reply, 0);
					}

					if ( i == 25 || i == 26 || i == 27 || i == 29 || i == 33 || i == 34 || i == 35 )
					{
						x=x+v; y=y+w; AddButton(x, y, QuickConfig.rowNumber(i,from), QuickConfig.rowNumber(i,from), i*10, GumpButtonType.Reply, 0);
					}
					else if ( i == 32 || i == 36 )
					{
						x=x+v; y=y+w; AddButton(x, y, QuickConfig.rowNumber(i,from), QuickConfig.rowNumber(i,from), i*10, GumpButtonType.Reply, 0);
						x=x+v; y=y+w; AddButton(x, y, QuickConfig.rowNumber(i,from), QuickConfig.rowNumber(i,from), i*11, GumpButtonType.Reply, 0);
						x=x+v; y=y+w; AddButton(x, y, QuickConfig.rowNumber(i,from), QuickConfig.rowNumber(i,from), i*12, GumpButtonType.Reply, 0);
					}
				}
			}
		}

		public static void RefreshQuickBar( Mobile from )
		{
			if ( from is PlayerMobile )
			{
				if( from.HasGump( typeof(QuickBar)) )
				{
					from.CloseGump( typeof(QuickBar) );
					from.SendGump( new QuickBar( from ) );
				}
			}
		}

		public void InvokeCommand( string c, Mobile from )
        {
            CommandSystem.Handle(from, String.Format("{0}{1}", CommandSystem.Prefix, c));
        }

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			from.CloseGump( typeof( QuickBar ) );

			if ( info.ButtonID > 0 ){ from.SendGump( new QuickBar( from ) ); from.SendSound( 0x4A ); }

			if ( info.ButtonID == 3 ){ InvokeCommand( "afk", from ); }
			else if ( info.ButtonID == 4 ){ InvokeCommand( "e", from ); }
			else if ( info.ButtonID == 5 ){ InvokeCommand( "magicgate", from ); }
			else if ( info.ButtonID == 6 ){ InvokeCommand( "wealth", from ); }
			else if ( info.ButtonID == 7 ){ InvokeCommand( "motd", from ); }
			else if ( info.ButtonID == 8 ){ InvokeCommand( "corpse", from ); }
			else if ( info.ButtonID == 9 ){ from.CloseGump( typeof( QuestsGump ) ); from.SendGump( new QuestsGump( from ) ); }
			else if ( info.ButtonID == 10 ){ InvokeCommand( "c", from ); }
			else if ( info.ButtonID == 11 ){ Server.Misc.RegionMusic.MusicRegion( from, from.Region ); }
			else if ( info.ButtonID == 12 ){ InvokeCommand( "music", from ); }
			else if ( info.ButtonID == 13 ){ if ( from.HasGump( typeof( SpecialAttackGump ) ) ){ from.CloseGump( typeof( SpecialAttackGump ) ); } else { InvokeCommand( "sad", from ); } }
			else if ( info.ButtonID == 14 ){ InvokeCommand( "bandself", from ); }
			else if ( info.ButtonID == 15 ){ InvokeCommand( "bandother", from ); }
			else if ( info.ButtonID == 22 ){ if ( from.HasGump( typeof( RegBar ) ) ){ from.CloseGump( typeof( RegBar ) ); } else { InvokeCommand( "regbar", from ); } }
			else if ( info.ButtonID == 23 ){ from.CloseGump( typeof( MyLibrary ) ); from.SendGump( new MyLibrary( from, 0 ) ); from.SendSound( 0x4A ); }
			else if ( info.ButtonID == 24 ){ from.CloseGump( typeof( MyChat ) ); from.SendGump( new MyChat( from, 0 ) ); from.SendSound( 0x4A ); }
			else if ( info.ButtonID == 25 ){ if ( from.HasGump( typeof( SpellBarsBard1 ) ) ){ InvokeCommand( "bardclose1", from ); } else { InvokeCommand( "bardtool1", from ); } }
			else if ( info.ButtonID == 250 ){ if ( from.HasGump( typeof( SpellBarsBard2 ) ) ){ InvokeCommand( "bardclose2", from ); } else { InvokeCommand( "bardtool2", from ); } }
			else if ( info.ButtonID == 26 ){ if ( from.HasGump( typeof( SpellBarsKnight1 ) ) ){ InvokeCommand( "knightclose1", from ); } else { InvokeCommand( "knighttool1", from ); } }
			else if ( info.ButtonID == 260 ){ if ( from.HasGump( typeof( SpellBarsKnight2 ) ) ){ InvokeCommand( "knightclose2", from ); } else { InvokeCommand( "knighttool2", from ); } }
			else if ( info.ButtonID == 27 ){ if ( from.HasGump( typeof( SpellBarsDeath1 ) ) ){ InvokeCommand( "deathclose1", from ); } else { InvokeCommand( "deathtool1", from ); } }
			else if ( info.ButtonID == 270 ){ if ( from.HasGump( typeof( SpellBarsDeath2 ) ) ){ InvokeCommand( "deathclose2", from ); } else { InvokeCommand( "deathtool2", from ); } }
			else if ( info.ButtonID == 28 )
			{
				if ( from.HasGump( typeof( DruidPouch.DruidBar ) ) )
				{
					from.CloseGump( typeof( DruidPouch.DruidBar ) );
				}
				else if ( from.Backpack.FindItemByType( typeof ( DruidPouch ) ) != null )
				{
					DruidPouch pouch = (DruidPouch)( from.Backpack.FindItemByType( typeof ( DruidPouch ) ) );
					if ( pouch.Bar == 1 ){ from.SendGump( new DruidPouch.DruidBar( from, pouch, true ) ); }
					else { from.SendGump( new DruidPouch.DruidBar( from, pouch, false ) ); }
				}
			}
			else if ( info.ButtonID == 29 ){ if ( from.HasGump( typeof( SpellBarsElement1 ) ) ){ InvokeCommand( "elementclose1", from ); } else { InvokeCommand( "elementtool1", from ); } }
			else if ( info.ButtonID == 290 ){ if ( from.HasGump( typeof( SpellBarsElement2 ) ) ){ InvokeCommand( "elementclose2", from ); } else { InvokeCommand( "elementtool2", from ); } }
			else if ( info.ButtonID == 30 )
			{ 
				if ( from.HasGump( typeof( JediSpellbook.PowerColumn ) ) )
				{
					from.CloseGump( typeof( JediSpellbook.PowerColumn ) );
					if ( from.Backpack.FindItemByType( typeof ( JediSpellbook ) ) != null )
					{
						JediSpellbook jedi = (JediSpellbook)(from.Backpack.FindItemByType( typeof ( JediSpellbook ) ));
						if ( jedi.owner == from )
							from.SendGump( new Server.Items.JediSpellbook.PowerRow( from, jedi ) );
					}
				}
				else if ( from.HasGump( typeof( JediSpellbook.PowerRow ) ) )
				{
					from.CloseGump( typeof( JediSpellbook.PowerRow ) );
				}
				else if ( from.Backpack.FindItemByType( typeof ( JediSpellbook ) ) != null )
				{
					JediSpellbook cube = (JediSpellbook)(from.Backpack.FindItemByType( typeof ( JediSpellbook ) ));
					if ( cube.owner == from )
						from.SendGump( new Server.Items.JediSpellbook.PowerColumn( from, cube ) );
				}
			}
			else if ( info.ButtonID == 31 )
			{
				if ( from.HasGump( typeof( BagOfTricks.TricksLargeColumn ) ) ){ 		from.CloseGump( typeof( BagOfTricks.TricksLargeColumn ) );	from.SendGump( new BagOfTricks.TricksLargeRow( from ) );	}
				else if ( from.HasGump( typeof( BagOfTricks.TricksLargeRow ) ) ){ 		from.CloseGump( typeof( BagOfTricks.TricksLargeRow ) );		from.SendGump( new BagOfTricks.TricksSmallColumn( from ) );	}
				else if ( from.HasGump( typeof( BagOfTricks.TricksSmallColumn ) ) ){ 	from.CloseGump( typeof( BagOfTricks.TricksSmallColumn ) );	from.SendGump( new BagOfTricks.TricksSmallRow( from ) );	}
				else if ( from.HasGump( typeof( BagOfTricks.TricksSmallRow ) ) ){ 		from.CloseGump( typeof( BagOfTricks.TricksSmallRow ) );	}
				else { 																																from.SendGump( new BagOfTricks.TricksLargeColumn( from ) );	}
			}
			else if ( info.ButtonID == 32 ){ if ( from.HasGump( typeof( SpellBarsMage1 ) ) ){ InvokeCommand( "mageclose1", from ); } else { InvokeCommand( "magetool1", from ); } }
			else if ( info.ButtonID == 320 ){ if ( from.HasGump( typeof( SpellBarsMage2 ) ) ){ InvokeCommand( "mageclose2", from ); } else { InvokeCommand( "magetool2", from ); } }
			else if ( info.ButtonID == 352 ){ if ( from.HasGump( typeof( SpellBarsMage3 ) ) ){ InvokeCommand( "mageclose3", from ); } else { InvokeCommand( "magetool3", from ); } }
			else if ( info.ButtonID == 384 ){ if ( from.HasGump( typeof( SpellBarsMage4 ) ) ){ InvokeCommand( "mageclose4", from ); } else { InvokeCommand( "magetool4", from ); } }
			else if ( info.ButtonID == 33 ){ if ( from.HasGump( typeof( SpellBarsMonk1 ) ) ){ InvokeCommand( "monkclose1", from ); } else { InvokeCommand( "monktool1", from ); } }
			else if ( info.ButtonID == 330 ){ if ( from.HasGump( typeof( SpellBarsMonk2 ) ) ){ InvokeCommand( "monkclose2", from ); } else { InvokeCommand( "monktool2", from ); } }
			else if ( info.ButtonID == 34 ){ if ( from.HasGump( typeof( SpellBarsNecro1 ) ) ){ InvokeCommand( "necroclose1", from ); } else { InvokeCommand( "necrotool1", from ); } }
			else if ( info.ButtonID == 340 ){ if ( from.HasGump( typeof( SpellBarsNecro2 ) ) ){ InvokeCommand( "necroclose2", from ); } else { InvokeCommand( "necrotool2", from ); } }
			else if ( info.ButtonID == 35 ){ if ( from.HasGump( typeof( SpellBarsPriest1 ) ) ){ InvokeCommand( "holyclose1", from ); } else { InvokeCommand( "holytool1", from ); } }
			else if ( info.ButtonID == 350 ){ if ( from.HasGump( typeof( SpellBarsPriest2 ) ) ){ InvokeCommand( "holyclose2", from ); } else { InvokeCommand( "holytool2", from ); } }
			else if ( info.ButtonID == 36 ){ if ( from.HasGump( typeof( SetupBarsArch1 ) ) ){ InvokeCommand( "archclose1", from ); } else { InvokeCommand( "archtool1", from ); } }
			else if ( info.ButtonID == 360 ){ if ( from.HasGump( typeof( SetupBarsArch2 ) ) ){ InvokeCommand( "archclose2", from ); } else { InvokeCommand( "archtool2", from ); } }
			else if ( info.ButtonID == 396 ){ if ( from.HasGump( typeof( SetupBarsArch3 ) ) ){ InvokeCommand( "archclose3", from ); } else { InvokeCommand( "archtool3", from ); } }
			else if ( info.ButtonID == 432 ){ if ( from.HasGump( typeof( SetupBarsArch4 ) ) ){ InvokeCommand( "archclose4", from ); } else { InvokeCommand( "archtool4", from ); } }
			else if ( info.ButtonID == 37 )
			{
				if ( from.HasGump( typeof( ShinobiScroll.ShinobiColumn ) ) )
				{
					from.CloseGump( typeof( ShinobiScroll.ShinobiColumn ) );
					if ( from.Backpack.FindItemByType( typeof ( ShinobiScroll ) ) != null )
					{
						ShinobiScroll scroll = (ShinobiScroll)(from.Backpack.FindItemByType( typeof ( ShinobiScroll ) ));
						if ( scroll.owner == from )
							from.SendGump( new Server.Items.ShinobiScroll.ShinobiRow( from, scroll ) );
					}
				}
				else if ( from.HasGump( typeof( ShinobiScroll.ShinobiRow ) ) )
				{
					from.CloseGump( typeof( ShinobiScroll.ShinobiRow ) );
				}
				else if ( from.Backpack.FindItemByType( typeof ( ShinobiScroll ) ) != null )
				{
					ShinobiScroll scroll = (ShinobiScroll)(from.Backpack.FindItemByType( typeof ( ShinobiScroll ) ));
					if ( scroll.owner == from )
						from.SendGump( new Server.Items.ShinobiScroll.ShinobiColumn( from, scroll ) );
				}
			}
			else if ( info.ButtonID == 38 )
			{ 
				if ( from.HasGump( typeof( SythSpellbook.PowerColumn ) ) )
				{
					from.CloseGump( typeof( SythSpellbook.PowerColumn ) );
					if ( from.Backpack.FindItemByType( typeof ( SythSpellbook ) ) != null )
					{
						SythSpellbook syth = (SythSpellbook)(from.Backpack.FindItemByType( typeof ( SythSpellbook ) ));
						if ( syth.owner == from )
							from.SendGump( new Server.Items.SythSpellbook.PowerRow( from, syth ) );
					}
				}
				else if ( from.HasGump( typeof( SythSpellbook.PowerRow ) ) )
				{
					from.CloseGump( typeof( SythSpellbook.PowerRow ) );
				}
				else if ( from.Backpack.FindItemByType( typeof ( SythSpellbook ) ) != null )
				{
					SythSpellbook cube = (SythSpellbook)(from.Backpack.FindItemByType( typeof ( SythSpellbook ) ));
					if ( cube.owner == from )
						from.SendGump( new Server.Items.SythSpellbook.PowerColumn( from, cube ) );
				}
			}
			else if ( info.ButtonID == 39 )
			{
				if ( from.HasGump( typeof( WitchPouch.WitchBar ) ) )
				{
					from.CloseGump( typeof( WitchPouch.WitchBar ) );
				}
				else if ( from.Backpack.FindItemByType( typeof ( WitchPouch ) ) != null )
				{
					WitchPouch pouch = (WitchPouch)(from.Backpack.FindItemByType( typeof ( WitchPouch ) ));
					if ( pouch.Bar == 1 ){ from.SendGump( new WitchPouch.WitchBar( from, pouch, true ) ); }
					else { from.SendGump( new WitchPouch.WitchBar( from, pouch, false ) ); }
				}
			}
			else if ( info.ButtonID == 40 ){ if ( from.HasGump( typeof( SkillListingGump ) ) ){ from.CloseGump( typeof( SkillListingGump ) ); } else { InvokeCommand( "skilllist", from ); } }
			else if ( info.ButtonID == 41 ){ if ( from.HasGump( typeof( CombatBar ) ) ){ from.CloseGump( typeof( CombatBar ) ); } else { InvokeCommand( "combatbar", from ); } }

			if ( info.ButtonID == 666 )
			{
				from.CloseGump( typeof( QuickConfig ) );
				from.SendGump( new QuickConfig( from ) );
			}
		}
    }

    public class QuickConfig : Gump
    {
		public QuickConfig( Mobile from ) : base( 50, 50 )
		{
			int btn1 = 3609;
			int btn2 = 3609;
			int btn3 = 3609;
			int btn4 = 3609;
			int btn5 = 3609;
			int btn6 = 3609;
			int btn7 = 3609;
			int btn8 = 3609;
			int btn9 = 3609;
			int btn10 = 3609;
			int btn11 = 3609;
			int btn12 = 3609;
			int btn13 = 3609;
			int btn14 = 3609;
			int btn15 = 3609;
			int btn16 = 3609;
			int btn17 = 3609;
			int btn18 = 3609;
			int btn19 = 3609;
			int btn20 = 3609;
			int btn21 = 3609;
			int btn22 = 3609;
			int btn23 = 3609;
			int btn24 = 3609;
			int btn25 = 3609;
			int btn26 = 3609;
			int btn27 = 3609;
			int btn28 = 3609;
			int btn29 = 3609;
			int btn30 = 3609;
			int btn31 = 3609;
			int btn32 = 3609;
			int btn33 = 3609;
			int btn34 = 3609;
			int btn35 = 3609;
			int btn36 = 3609;
			int btn37 = 3609;
			int btn38 = 3609;
			int btn39 = 3609;
			int btn40 = 3609;
			int btn41 = 3609;

			PlayerMobile pm = (PlayerMobile)from;

			string keys = PlayerSettings.ValQuickConfig( from );;

			if ( keys.Length > 0 )
			{
				string[] configures = keys.Split('#');
				int nEntry = 1;
				foreach (string key in configures)
				{
					if ( nEntry == 1 && key == "1" ){ btn1 = 4017; }
					else if ( nEntry == 2 && key == "1" ){ btn2 = 4017; }
					else if ( nEntry == 3 && key == "1" ){ btn3 = 4017; }
					else if ( nEntry == 4 && key == "1" ){ btn4 = 4017; }
					else if ( nEntry == 5 && key == "1" ){ btn5 = 4017; }
					else if ( nEntry == 6 && key == "1" ){ btn6 = 4017; }
					else if ( nEntry == 7 && key == "1" ){ btn7 = 4017; }
					else if ( nEntry == 8 && key == "1" ){ btn8 = 4017; }
					else if ( nEntry == 9 && key == "1" ){ btn9 = 4017; }
					else if ( nEntry == 10 && key == "1" ){ btn10 = 4017; }
					else if ( nEntry == 11 && key == "1" ){ btn11 = 4017; }
					else if ( nEntry == 12 && key == "1" ){ btn12 = 4017; }
					else if ( nEntry == 13 && key == "1" ){ btn13 = 4017; }
					else if ( nEntry == 14 && key == "1" ){ btn14 = 4017; }
					else if ( nEntry == 15 && key == "1" ){ btn15 = 4017; }
					else if ( nEntry == 16 && key == "1" ){ btn16 = 4017; }
					else if ( nEntry == 17 && key == "1" ){ btn17 = 4017; }
					else if ( nEntry == 18 && key == "1" ){ btn18 = 4017; }
					else if ( nEntry == 19 && key == "1" ){ btn19 = 4017; }
					else if ( nEntry == 20 && key == "1" ){ btn20 = 4017; }
					else if ( nEntry == 21 && key == "1" ){ btn21 = 4017; }
					else if ( nEntry == 22 && key == "1" ){ btn22 = 4017; }
					else if ( nEntry == 23 && key == "1" ){ btn23 = 4017; }
					else if ( nEntry == 24 && key == "1" ){ btn24 = 4017; }
					else if ( nEntry == 25 && key == "1" ){ btn25 = 4017; }
					else if ( nEntry == 26 && key == "1" ){ btn26 = 4017; }
					else if ( nEntry == 27 && key == "1" ){ btn27 = 4017; }
					else if ( nEntry == 28 && key == "1" ){ btn28 = 4017; }
					else if ( nEntry == 29 && key == "1" ){ btn29 = 4017; }
					else if ( nEntry == 30 && key == "1" ){ btn30 = 4017; }
					else if ( nEntry == 31 && key == "1" ){ btn31 = 4017; }
					else if ( nEntry == 32 && key == "1" ){ btn32 = 4017; }
					else if ( nEntry == 33 && key == "1" ){ btn33 = 4017; }
					else if ( nEntry == 34 && key == "1" ){ btn34 = 4017; }
					else if ( nEntry == 35 && key == "1" ){ btn35 = 4017; }
					else if ( nEntry == 36 && key == "1" ){ btn36 = 4017; }
					else if ( nEntry == 37 && key == "1" ){ btn37 = 4017; }
					else if ( nEntry == 38 && key == "1" ){ btn38 = 4017; }
					else if ( nEntry == 39 && key == "1" ){ btn39 = 4017; }
					else if ( nEntry == 40 && key == "1" ){ btn40 = 4017; }
					else if ( nEntry == 41 && key == "1" ){ btn41 = 4017; }

					nEntry++;
				}
			}

			string color = "#ddbc4b";
			from.SendSound( 0x4A ); 

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			AddImage(0, 0, 9548, PlayerSettings.GetGumpHue( from ));
			AddHtml( 12, 12, 300, 20, @"<BODY><BASEFONT Color=" + color + ">CONFIGURE QUICK BAR</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(967, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);

			AddHtml( 14, 55, 981, 94, @"<BODY><BASEFONT Color=" + color + ">This toolbar provides a quick and convenient way to keep an eye on certain inventory items, invoke commands, and access information. Images are used to represent the function of the various buttons. You must choose what icons will appear on your quick bar, and you can select those here. The icons for spells, songs, or abilities will open or close the bar for those categories.</BASEFONT></BODY>", (bool)false, (bool)false);

			AddButton(277, 151, btn1, btn1, 1, GumpButtonType.Reply, 0);
			AddHtml( 316, 151, 223, 20, @"<BODY><BASEFONT Color=" + color + ">Horizontal Bar</BASEFONT></BODY>", (bool)false, (bool)false);

			AddButton(580, 151, btn2, btn2, 2, GumpButtonType.Reply, 0);
			AddHtml( 619, 151, 223, 20, @"<BODY><BASEFONT Color=" + color + ">Open At Login</BASEFONT></BODY>", (bool)false, (bool)false);

			int icons = 2;
			int count = 2;

			int p = 39;

			int x = 77;
			int y = 158;

			int button = btn3;

			while ( icons < 41 )
			{
				icons++;

				if ( icons == 3 ){ button = btn3; }
				else if ( icons == 4 ){ button = btn4; }
				else if ( icons == 5 ){ button = btn5; }
				else if ( icons == 6 ){ button = btn6; }
				else if ( icons == 7 ){ button = btn7; }
				else if ( icons == 8 ){ button = btn8; }
				else if ( icons == 9 ){ button = btn9; }
				else if ( icons == 10 ){ button = btn10; }
				else if ( icons == 11 ){ button = btn11; }
				else if ( icons == 12 ){ button = btn12; }
				else if ( icons == 13 ){ button = btn13; }
				else if ( icons == 14 ){ button = btn14; }
				else if ( icons == 15 ){ button = btn15; }
				else if ( icons == 16 ){ button = btn16; }
				else if ( icons == 17 ){ button = btn17; }
				else if ( icons == 18 ){ button = btn18; }
				else if ( icons == 19 ){ button = btn19; }
				else if ( icons == 20 ){ button = btn20; }
				else if ( icons == 21 ){ button = btn21; }
				else if ( icons == 22 ){ button = btn22; }
				else if ( icons == 23 ){ button = btn23; }
				else if ( icons == 24 ){ button = btn24; }
				else if ( icons == 25 ){ button = btn25; }
				else if ( icons == 26 ){ button = btn26; }
				else if ( icons == 27 ){ button = btn27; }
				else if ( icons == 28 ){ button = btn28; }
				else if ( icons == 29 ){ button = btn29; }
				else if ( icons == 30 ){ button = btn30; }
				else if ( icons == 31 ){ button = btn31; }
				else if ( icons == 32 ){ button = btn32; }
				else if ( icons == 33 ){ button = btn33; }
				else if ( icons == 34 ){ button = btn34; }
				else if ( icons == 35 ){ button = btn35; }
				else if ( icons == 36 ){ button = btn36; }
				else if ( icons == 37 ){ button = btn37; }
				else if ( icons == 38 ){ button = btn38; }
				else if ( icons == 39 ){ button = btn39; }
				else if ( icons == 40 ){ button = btn40; }
				else if ( icons == 41 ){ button = btn41; }

				count++;
				if ( count == 16 || count == 29 ){ x = x+332; y = 158; }
				y = y + p;

				int itemImage = GetItemImage(icons, from);
				if (0 < itemImage)
				{
					const int BACKGROUND_IMAGE = 10351;
					Rectangle2D bounds = ItemBounds.Table[itemImage];
					AddImage(x+36, y, BACKGROUND_IMAGE);
					AddItem(x+36 + bounds.Width / 2 - bounds.X, y + bounds.Height / 2 - bounds.Y, itemImage);
				}
				else
				{
					AddImage(x+36, y, rowNumber( icons, from ));
				}

				AddButton(x, y+6, button, button, icons, GumpButtonType.Reply, 0);
				AddHtml( x+74, y+5, 223, 20, @"<BODY><BASEFONT Color=" + color + ">" + rowText( icons ) + "</BASEFONT></BODY>", (bool)false, (bool)false);
			}

		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( info.ButtonID > 0 )
			{
				PlayerSettings.SetQuickConfig( from, info.ButtonID );
				from.SendGump( new QuickConfig( from ) );
				from.CloseGump( typeof(QuickBar) );
				from.SendGump( new QuickBar( from ) );
			}
			else
				from.SendSound( 0x4A );
		}

		public static string rowText( int set )
		{
			string row = "";

			if ( set == 3 ){ row = "Away from Keyboard"; }
			else if ( set == 4 ){ row = "Emotes"; }
			else if ( set == 5 ){ row = "Find Nearest Magic Portal"; }
			else if ( set == 6 ){ row = "Gold in the Bank"; }
			else if ( set == 7 ){ row = "Message of the Day"; }
			else if ( set == 8 ){ row = "Find Your Corpse"; }
			else if ( set == 9 ){ row = "Your Main Quests"; }
			else if ( set == 10 ){ row = "Chat & Messages"; }
			else if ( set == 11 ){ row = "Switch Song"; }
			else if ( set == 12 ){ row = "Music Playlist"; }
			else if ( set == 13 ){ row = "Special Attack Bar"; }
			else if ( set == 14 ){ row = "Bandage Yourself & Quantity"; }
			else if ( set == 15 ){ row = "Bandage Other"; }
			else if ( set == 16 ){ row = "Arrow Quantity"; }
			else if ( set == 17 ){ row = "Bolt Quantity"; }
			else if ( set == 18 ){ row = "Harpoon Rope Quantity"; }
			else if ( set == 19 ){ row = "Mage Eye Quantity"; }
			else if ( set == 20 ){ row = "Krystal Quantity"; }
			else if ( set == 21 ){ row = "Throwing Weapon Quantity"; }
			else if ( set == 22 ){ row = "Reagent Bar"; }
			else if ( set == 23 ){ row = "Library"; }
			else if ( set == 24 ){ row = "Conversations"; }
			else if ( set == 25 ){ row = "Bard Songs"; }
			else if ( set == 26 ){ row = "Chivalry Magic"; }
			else if ( set == 27 ){ row = "Death Knight Magic"; }
			else if ( set == 28 ){ row = "Druid Potions"; }
			else if ( set == 29 ){ row = "Elementalist Spells"; }
			else if ( set == 30 ){ row = "Jedi Abilities"; }
			else if ( set == 31 ){ row = "Jester Abilities"; }
			else if ( set == 32 ){ row = "Magery Spells"; }
			else if ( set == 33 ){ row = "Monk Abilities"; }
			else if ( set == 34 ){ row = "Necromancy Spells"; }
			else if ( set == 35 ){ row = "Priest Prayers"; }
			else if ( set == 36 ){ row = "Ancient Spells"; }
			else if ( set == 37 ){ row = "Shinobi Abilities"; }
			else if ( set == 38 ){ row = "Syth Abilities"; }
			else if ( set == 39 ){ row = "Witch Potions"; }
			else if ( set == 40 ){ row = "Skill List"; }
			else if ( set == 41 ){ row = "Combat Bar"; }

			return row;
		}


		public static int GetElementBookIcon( Mobile m )
		{
			int element = ((PlayerMobile)m).CharacterElement;

			if ( element == 0 )
				return 10323;

			else if ( element == 1 )
				return 10324;

			else if ( element == 2 )
				return 10325;

			else if ( element == 3 )
				return 10326;

			return 10323;
		}

		public static int rowNumber( int set, Mobile m )
		{
			int row = 0;

			if ( set == 3 ){ row = 10309; }
			else if ( set == 4 ){ row = 10327; }
			else if ( set == 5 ){ row = 10328; }
			else if ( set == 6 ){ row = 10329; }
			else if ( set == 7 ){ row = 10336; }
			else if ( set == 8 ){ row = 10321; }
			else if ( set == 9 ){ row = 10339; }
			else if ( set == 10 ){ row = 10320; }
			else if ( set == 11 ){ row = 10337; }
			else if ( set == 12 ){ row = 10338; }
			else if ( set == 13 ){ row = 10344; }
			else if ( set == 14 ){ row = 10311; }
			else if ( set == 15 ){ row = 10312; }
			else if ( set == 16 ){ row = 10310; }
			else if ( set == 17 ){ row = 10313; }
			else if ( set == 18 ){ row = 10330; }
			else if ( set == 19 ){ row = 10334; }
			else if ( set == 20 ){ row = 10333; }
			else if ( set == 21 ){ row = 10347; }
			else if ( set == 22 ){ row = 10340; }
			else if ( set == 23 ){ row = 10341; }
			else if ( set == 24 ){ row = 10342; }
			else if ( set == 25 ){ row = 10314; }
			else if ( set == 26 ){ row = 10315; }
			else if ( set == 27 ){ row = 10316; }
			else if ( set == 28 ){ row = 10322; }
			else if ( set == 29 ){ row = GetElementBookIcon( m ); }
			else if ( set == 30 ){ row = 10331; }
			else if ( set == 31 ){ row = 10332; }
			else if ( set == 32 ){ row = 10318; }
			else if ( set == 33 ){ row = 10335; }
			else if ( set == 34 ){ row = 10319; }
			else if ( set == 35 ){ row = 10317; }
			else if ( set == 36 ){ row = 10343; }
			else if ( set == 37 ){ row = 10345; }
			else if ( set == 38 ){ row = 10346; }
			else if ( set == 39 ){ row = 10348; }
			else if ( set == 40 ){ row = 10350; }

			return row;
		}

		public static int GetItemImage( int set, Mobile m )
		{
			int row = 0;
			if ( set == 41 ){ row = 0x9B7; }

			return row;
		}
	}
}