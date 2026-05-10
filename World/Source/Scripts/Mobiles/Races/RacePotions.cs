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
	public class RacePotions : Item
	{
		[Constructable]
		public RacePotions() : base( 0x506C )
		{
			Weight = 1.0;
			Name = "gypsy potion shelf";
			Hue = 0xABE;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from is PlayerMobile )
			{
				var player = (PlayerMobile)from;
				if (player.Avatar.Active && !player.Avatar.UnlockMonsterRaces)
				{
					from.SendMessage("An Avatar must unlock the Monster Races system before using this.");
					return;
				}
			}

			if ( from.InRange( this.GetWorldLocation(), 4 ) && MyServerSettings.MonstersAllowed() )
			{
				if ( from.RaceSection < 1 ){ from.RaceSection = 1; }
				from.CloseGump( typeof( GypsyTarotGump ) );
				from.CloseGump( typeof( WelcomeGump ) );
				from.CloseGump( typeof( RacePotionsGump ) );
				from.SendGump( new RacePotionsGump( from, 0, false ) );
				from.PlaySound( 0x02F );
			}
		}

		public RacePotions( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public class RacePotionsGump : Gump
		{
			private bool m_RequireConfirmation;
			private int m_Tavern; 

			public RacePotionsGump( Mobile from, int tavern, bool requireConfirmation ): base( 50, 50 )
			{
				m_Tavern = tavern;
				m_RequireConfirmation = requireConfirmation;
				string color = "#c09b88";
				int page = from.RaceSection;

				string species = "";
				if ( tavern > 0 )
				{
					if ( from.FindItemOnLayer( Layer.Special ) != null )
					{
						if ( from.FindItemOnLayer( Layer.Special ) is BaseRace )
						{
							BaseRace info = (BaseRace)(from.FindItemOnLayer( Layer.Special ));
							species = info.SpeciesFamily;
						}
					}
				}

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				int x = 194;
				int y = 337;
				int p = 173;

				int race = 79999+page;
				int btn = 0;

				int next = page+1;
				bool nxt = false;
				if ( page == p ){ next = 1; }
				while ( !nxt )
				{
					if ( next == 1 )
						nxt = true;
					else if ( BaseRace.GetMonsterSize( (79999+next), species, from.RaceID, tavern ) )
						nxt = true;
					else
						next++;

					if ( next == p+1 ){ next = 1; nxt = true; }
				}
				int prev = page-1;
				bool prv = false;
				if ( page == 1 ){ prev = p; }
				while ( !prv )
				{
					if ( prev == 1 )
						prv = true;
					else if ( BaseRace.GetMonsterSize( (79999+prev), species, from.RaceID, tavern) )
						prv = true;
					else
						prev--;
				}

				string titleG = "GYPSY POTION SHELF";

				if ( tavern > 0 )
				{
					AddImage(0, 0, 2612, Server.Misc.PlayerSettings.GetGumpHue( from ));
					titleG = "CREATURE APPEARANCE";
					AddButton(736, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);	// CLOSE
				}
				else
				{
					AddImage(0, 0, 2613, Server.Misc.PlayerSettings.GetGumpHue( from ));
					AddImage(387, 0, 2613, Server.Misc.PlayerSettings.GetGumpHue( from ));
					AddImage(774, 0, 2613, Server.Misc.PlayerSettings.GetGumpHue( from ));
					AddButton(10, 10, 3610, 3610, 9999, GumpButtonType.Reply, 0);	// HELP

					int g = 0;
					int gc = 0;
					int gx = 804;
					int gy = 56;
					int gm = 30;
					int go = MySettings.S_MonsterCharacters;
					int gb = 6000;

					AddHtml( 786, 11, 213, 20, @"<BODY><BASEFONT Color=" + color + ">CATEGORIES</BASEFONT></BODY>", (bool)false, (bool)false);

					gc++; AddButton(gx, gy, 2447, 2447, 123456789, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Human</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm;

					while ( g < 44 )
					{
						g++;

						if ( gc == 18 || gc == 36 ){ gx=gx+126; gy=56; }

						if ( g == 1 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Aquatic</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 2 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Bugbear</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 3 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Centaur</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 4 && go >= 2 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Cyclops</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 5 && go >= 2 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Daemon</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 6 && go >= 2 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Dagon</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 7 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Demon</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 8 && go >= 3 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Devil</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 9 && go >= 2 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Dragon</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 10 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Drakkul</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 11 && go >= 2 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Ettin</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 12 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Fey</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 13 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Gargoyle</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 14 && go >= 3 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Giant</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 15 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Gnoll</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 16 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Goblin</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 17 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Golem</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 18 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Hobgoblin</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 19 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Illithid</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 20 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Imp</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 21 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Kilrathi</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 22 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Kobold</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 23 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Minotaur</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 24 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Mummy</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 25 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Mushroom</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 26 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Naga</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 27 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Ogre</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 28 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Orc</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 29 && go >= 2 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Owlbear</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 30 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Plant</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 31 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Reptilian</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 32 && go >= 2 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Revenant</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 33 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Rodent</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 34 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Salamander</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 35 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Satyr</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 36 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Serpent</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 37 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Skeleton</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 38 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Sphinx</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 39 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Succubus</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 40 && go >= 2 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Titan</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 41 && go >= 2 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Tree</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 42 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Troll</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 43 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Vampyre</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
						else if ( g == 44 && go >= 1 ){ gc++; AddButton(gx, gy, 2447, 2447, g+gb, GumpButtonType.Reply, 0); AddHtml( gx+18, gy-4, 98, 20, @"<BODY><BASEFONT Color=" + color + ">Zombi</BASEFONT></BODY>", (bool)false, (bool)false); gy=gy+gm; }
					}

					AddButton(1125, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);	// CLOSE
				}

				AddHtml( 128, 11, 249, 20, @"<BODY><BASEFONT Color=" + color + "><RIGHT>" + titleG + "</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);

				// MONSTER IMAGE BACKDROP
				AddImage(45, 110, 155);
				AddImage(45, 263, 155);
				AddImage(47, 113, 163);
				AddImage(47, 265, 163);

				if ( race > 80000 )
				{
					BaseRace costume = Server.Items.BaseRace.GetCostume( race );
					btn = 80000+costume.SpeciesID;

					// LEFT SIDE ---------------------------------------------------------------------------------------------------------------------------------------------------------

					string alignment = "#e0e2b7";

					if ( costume.SpeciesAlignment == "good" ){ alignment = "#97cb9a"; }
					else if ( costume.SpeciesAlignment == "evil" ){ alignment = "#d17777"; }

					AddHtml( 105, 42, 100, 20, @"<BODY><BASEFONT Color=" + color + ">Species:</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 105, 77, 100, 20, @"<BODY><BASEFONT Color=" + color + ">Alignment:</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 234, 42, 157, 20, @"<BODY><BASEFONT Color=" + color + ">" + MorphingTime.CapitalizeWords(costume.SpeciesFamily) + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 234, 77, 157, 20, @"<BODY><BASEFONT Color=" + alignment + ">" + MorphingTime.CapitalizeWords(costume.SpeciesAlignment) + "</BASEFONT></BODY>", (bool)false, (bool)false);

					x = x - (int)( costume.SpeciesWide / 2 );
					y = y - (int)( costume.SpeciesHigh / 2 );
					AddImage(x, y, costume.SpeciesIcon);	// MONSTER IMAGE

					AddButton(12, 573, 4014, 4014, prev, GumpButtonType.Reply, 0);	// PREV
					AddButton(347, 573, 4005, 4005, next, GumpButtonType.Reply, 0);	// NEXT

					// RIGHT SIDE -----------------------------------------------------------------------------------------------------------------------------------------------------

					AddImage(452, 32, 2001);						// PAPERDOLL CONTAINER
					AddImage(459, 51, 50000+costume.SpeciesGump);	// MONSTER PAPERDOLL
					AddImage(453, 51, 50422);						// BACKPACK
					AddHtml( 479, 308, 200, 20, @"<BODY><BASEFONT Color=#000008><CENTER>" + (costume.Name).ToUpper() + "</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(653, 254, 4023, 4023, btn, GumpButtonType.Reply, 0);	// OK BUTTON
					AddHtml( 403, 372, 356, 217, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.BaseRace.GetAbilities( btn-80000 ) + "</BASEFONT></BODY>", (bool)false, (bool)false);

					costume.Delete();
				}
				else
				{
					// LEFT SIDE ---------------------------------------------------------------------------------------------------------------------------------------------------------

					string alignment = "#e0e2b7";

					AddHtml( 105, 42, 100, 20, @"<BODY><BASEFONT Color=" + color + ">Species:</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 105, 77, 100, 20, @"<BODY><BASEFONT Color=" + color + ">Alignment:</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 234, 42, 157, 20, @"<BODY><BASEFONT Color=" + color + ">Human</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 234, 77, 157, 20, @"<BODY><BASEFONT Color=" + alignment + ">Neutral</BASEFONT></BODY>", (bool)false, (bool)false);

					x = x - 15;
					y = y - 29;
					AddImage(x, y, 2818);	// HUMAN IMAGE

					AddButton(12, 573, 4014, 4014, prev, GumpButtonType.Reply, 0);	// PREV
					AddButton(347, 573, 4005, 4005, next, GumpButtonType.Reply, 0);	// NEXT

					// RIGHT SIDE -----------------------------------------------------------------------------------------------------------------------------------------------------

					AddImage(452, 32, 2001);	// PAPERDOLL CONTAINER
					AddImage(459, 51, 50994);	// MONSTER PAPERDOLL
					AddImage(453, 51, 50422);	// BACKPACK
					AddHtml( 479, 308, 200, 20, @"<BODY><BASEFONT Color=#000008><CENTER>HUMAN</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(653, 254, 4023, 4023, 1000, GumpButtonType.Reply, 0);	// OK BUTTON
					AddHtml( 403, 372, 356, 217, @"<BODY><BASEFONT Color=" + color + "><BR>Humans are the average, most common race in the land.</BASEFONT></BODY>", (bool)false, (bool)false);
				}
			}

			public override void OnResponse( NetState state, RelayInfo info ) 
			{
				Mobile from = state.Mobile; 

				from.CloseGump( typeof( GypsyTarotGump ) );
				from.CloseGump( typeof( WelcomeGump ) );
				from.CloseGump( typeof( RacePotionsGump ) );

				if ( info.ButtonID == 123456789 )
				{
					from.RaceSection = 0;
					from.SendGump( new RacePotionsGump( from, m_Tavern, m_RequireConfirmation ) );
					from.SendSound( 0x4A ); 
				}
				else if ( info.ButtonID > 6000 && info.ButtonID < 6100 )
				{
					int quick = info.ButtonID - 6000;
					int move = 0;

					if ( quick == 1 ){ move = 1; }
					else if ( quick == 2 ){ move = 6; }
					else if ( quick == 3 ){ move = 7; }
					else if ( quick == 4 ){ move = 8; }
					else if ( quick == 5 ){ move = 11; }
					else if ( quick == 6 ){ move = 21; }
					else if ( quick == 7 ){ move = 23; }
					else if ( quick == 8 ){ move = 29; }
					else if ( quick == 9 ){ move = 34; }
					else if ( quick == 10 ){ move = 35; }
					else if ( quick == 11 ){ move = 43; }
					else if ( quick == 12 ){ move = 50; }
					else if ( quick == 13 ){ move = 53; }
					else if ( quick == 14 ){ move = 57; }
					else if ( quick == 15 ){ move = 70; }
					else if ( quick == 16 ){ move = 71; }
					else if ( quick == 17 ){ move = 74; }
					else if ( quick == 18 ){ move = 76; }
					else if ( quick == 19 ){ move = 77; }
					else if ( quick == 20 ){ move = 78; }
					else if ( quick == 21 ){ move = 80; }
					else if ( quick == 22 ){ move = 81; }
					else if ( quick == 23 ){ move = 84; }
					else if ( quick == 24 ){ move = 90; }
					else if ( quick == 25 ){ move = 92; }
					else if ( quick == 26 ){ move = 94; }
					else if ( quick == 27 ){ move = 97; }
					else if ( quick == 28 ){ move = 100; }
					else if ( quick == 29 ){ move = 110; }
					else if ( quick == 30 ){ move = 111; }
					else if ( quick == 31 ){ move = 113; }
					else if ( quick == 32 ){ move = 121; }
					else if ( quick == 33 ){ move = 122; }
					else if ( quick == 34 ){ move = 129; }
					else if ( quick == 35 ){ move = 130; }
					else if ( quick == 36 ){ move = 131; }
					else if ( quick == 37 ){ move = 138; }
					else if ( quick == 38 ){ move = 151; if ( MySettings.S_MonsterCharacters > 1 ){ move = 150; } }
					else if ( quick == 39 ){ move = 152; }
					else if ( quick == 40 ){ move = 155; }
					else if ( quick == 41 ){ move = 38; }
					else if ( quick == 42 ){ move = 157; }
					else if ( quick == 43 ){ move = 163; }
					else if ( quick == 44 ){ move = 166; }

					from.RaceSection = move+1;
					from.SendGump( new RacePotionsGump( from, m_Tavern, m_RequireConfirmation ) );
					from.SendSound( 0x4A ); 
				}
				else if ( info.ButtonID == 9999 )
				{
					from.SendGump( new RacePotionsGump( from, m_Tavern, m_RequireConfirmation ) );
					from.SendGump( new CreatureHelpGump( from, m_Tavern ) );
					from.SendSound( 0x4A ); 
				}
				else if ( info.ButtonID > 0 && info.ButtonID < 180 )
				{
					from.RaceSection = info.ButtonID;
					from.SendGump( new RacePotionsGump( from, m_Tavern, m_RequireConfirmation ) );
					from.SendSound( 0x4A ); 
				}
				else if ( info.ButtonID == 1000 )
				{
					ConfirmationGump.PromptIfFalse(
						from,
						m_RequireConfirmation,
						() =>
							{
								BaseRace.BackToHuman( from );
								if ( m_Tavern == 0 ){ from.PlaySound( Utility.RandomList( 0x030, 0x031 ) ); }
								Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 0, 0, 5042, 0 );
							},
						onConfirmed => new ConfirmationGump(from, "Confirm Race Change", "Changing to a Human will cause you to permanently lose all racial bonuses. You will not be able to change back. Are you sure you want to change?", onConfirmed )
					);
				}
				else if ( info.ButtonID > 80000 )
				{
					int race = info.ButtonID - 80000;
					BaseRace.CreateRace( from, race, true );
					if ( m_Tavern == 0 ){ from.PlaySound( Utility.RandomList( 0x030, 0x031 ) ); }
					Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 0, 0, 5042, 0 );
				}
				else
				{
					if ( m_Tavern > 0 )
					{
						from.SendSound( 0x4A ); 
						from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) );
					}
					else
					{
						from.PlaySound( 0x02F );
					}
				}
			}
		}

		public static string RaceHelp( int origin )
		{
			string txt = "";

			if ( origin < 1 ){ txt = txt + "This shelf contains many of the gypsy's concoctions she brewed, where drinking them will physically alter you to another creature. You can use the arrows at the bottom to navigate through the shelf. If you choose to start your journey as a creature, search through this shelf and find one that appeals to you. Once you find something of interest, drink the potion by pressing the OK button next to the paperdoll image of the creature. Each listed potion notes the species and lists any benefits you may gain from being that creature. You will also see representations of these creatures, as that would be the appearance you will have for the rest of that character's life. You will only be able to change your creature appearance to another creature of the same species, but some creatures only have one appearance option available.<BR><BR>"; }

			txt = txt + "Because these creatures use the static in-game models, you cannot customize their appearances like you can with humans. The images shown are what you will look like. You can, however, use all of the same types of items as humans do. Instead of seeing them represented on your character, they will be displayed as icons on your character's paperdoll window. You can hover over these icons to see the item's information, as well as select them to drag and drop them within your inventory. Creatures cannot ride mounts, so they have to find other means to travel swiftly. This could be from magical spells, or just a good pair of hiking boots.<BR><BR>";

			txt = txt + "As a creature, you can hover your cursor over your paperdoll to see the additional attributes you have being that type of creature. As your creature progresses through the game, these attributes will fluctuate based on your character level. Your character level can be viewed by using the INFO button on your paperdoll, and it is a combination of three different areas. The first is your total skills acquired, where the second is your total statistics gained such as strength or dexterity. The final area is your fame and karma, which will cause your level to raise higher or lower most often. The closer to zero your fame and karma are, the less impact on your level. <BR><BR>";

			txt = txt + "Some creatures do not need to eat or drink, and these are usually creatures like the undead. There are others that only need to drink, and that is commonly plant-type creatures. Vampires suffer from the effects of hunger and thirst as humans do, but they can never eat and drink common foods. Instead, they must feast on fresh blood which satisfies both thirst and hunger simultaneously. Some other may need to eat fresh brains to survive. The only way to get fresh blood or fresh brains is to kill birds, animals, humanoids, or giants.<BR><BR>";

			txt = txt + "There are two common adventure starting paths one can take. They can simply enter the world in the average way, or they can be labeled a murderous criminal. Where regular characters have the ability to achieve up to 1,000 skill points, those that begin as murderous criminals can achieve up to 1,300 points. Each creature has an inherit alignment that sets the tone of their outlook from others in the world. Creatures of good alignment cannot start their journey as a murderous criminal. Neutral creatures can choose to be regular adventurers, or murderous criminals. Those of evil alignment can also choose either, but avoiding the murderous criminal path does not welcome you into civilized villages and cities. One that begins with evil alignment would need to achieve a fame and karma title of admirable (2,500 or more of each). This then assumes that you have no recorded murders, or criminal actions pending. Falling below admirable would have citizens and guards look fearfully at you and try to drive you away. Places such as the port islands, the Undercity of Umbra, the Village of Ravendark, or even Xardok care very little if you are and evil creature or not. This is probably why such creatures gather at those places.<BR><BR>";

			txt = txt + "After you enter the world as a creature, there a setting you can configure later on. When you access the HELP section of your character paperdoll, you can see this option appear in the SETTINGS section. By default, your creature character will sometimes make sounds when hurting others or getting hurt. You can turn these sounds on or off. You can also access this information again in the HELP section, using the button labeled CREATURE HELP. So all of the information contained here is readily accessible later on.<BR><BR>";

			txt = txt + "There are two other settings you can configure later on, but only in taverns and inns. These are also accessed by the HELP button, and then the SETTINGS section. One setting is the type of magic you want your creature to focus on, which can only be accessed in taverns or inns. You will only see this option if the creature has some inherent magical ability like magery or necromancy. Changing your CREATURE MAGIC will change what the creature has proficiency in. Meaning if you select a creature with a bonus to magery, altering this setting could have that creature instead have a bonus to necromancy or elementalism. The other option is the CREATURE TYPE. This either lets you change to a human forever, or change your appearance to another creature model within the same species. If there is only one creature appearance model for your species, the only option here will be to turn to human. When you turn back to a human, you will lose all of the attributes of the race you were.<BR><BR>";

			txt = txt + "This world has become an accepting land to most creatures. Long ago, races such as dwarves, elves, bobbits, and fuzzies lived among humans. Now you may stumble into cities and see a troll drinking at the tavern, or a minotaur resting by a campfire with his human comrades. Although most creatures are considered murderous monsters to be killed on sight, some have rose above the rest and performed acts of heroism or simply convinced others they mean no harm. You could be an ogre that rids the world of evil, or a lizardman practicing dark magic with necromantic spells. The choice is yours.<BR><BR>";

			if ( origin < 1 ){ txt = txt + "NOTE: The gaming environment does allow for discovered options to make slight changes to your character's appearance, without maneuvering away from the human models of dressing and appearance customization. Items such as orc masks or bone helms, can allow you to superficially play an orc or a skeletal knight. There are also magical orbs of essence that can change your skin and hair colors to mimic those of the drow, orcs, lizardmen, and vampires. Again, these are superficial and only give you the appearance of such a race. These creatures, however, instill a fantastical sense of becoming something different.<BR><BR>"; }

			return txt;
		}

		public static string RaceEquipment()
		{
			string txt = "Here is an example of a bugbear character named Drub of the Savage Claw. The left is their in-game avatar, and the right is their paperdoll image. Note the various equipment icons on the left and right of the bugbear's paperdoll image. Creatures can equip the same items as humans, and manage the equipment in a similar fashion. You can select the icons to drag and drop equipment, or hover over the icons to get the needed information for the item.<BR><BR>NOTE: When robes are equipped on humans, the armor tunics and sleeves/arms are covered by the robe. It is the same for creatures, where a robe icon will cover the two icons of the lower left of the bugbear's paperdoll image.";

			return txt;
		}
	}
}