using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Engines.PartySystem;

namespace Server.Misc
{
    class SummonQuests
    {
		// THERE SHOULD ONLY BE ONE SUCH CREATURE IN EACH DUNGEON AND THEY SHOULD BE UNIQUE SOMEHOW SO ADVENTURERS CAN FIND THEM

		public static int SummonCarriers( Mobile m, BaseCreature b, int Heat )
		{
			Region reg = Region.Find( m.Location, m.Map );

			if ( reg.IsPartOf( "Stonegate Castle" ) && m is AshDragon )
			{
				m.EmoteHue = 123;
				Item ashheart = new SummonItems();
					ashheart.Name = "heart of ash";
					ashheart.ItemID = 0xF91;
					ashheart.Hue = 0x76C;
					b.PackItem( ashheart );
			}
			else if ( reg.IsPartOf( "the Vault of the Black Knight" ) && m is WaxSculpture && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 6421, 237, Map.Lodor ) )
			{
				m.EmoteHue = 123;
				m.Name = "a mystical wax golem";

				Item wax = new SummonItems();
					wax.Name = "mystical wax";
					wax.ItemID = 0x1422;
					wax.Hue = 0x490;
					b.PackItem( wax );
			}
			else if ( reg.IsPartOf( "the Crypts of Dracula" ) && m is VampirePrince && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5741, 2788, Map.Lodor ) )
			{
				m.EmoteHue = 123;
				m.Title = "the son of Dracula";
				Server.Misc.MorphingTime.VampireDressUp( m, 605 );
				Heat = 4;

				Item fang = new SummonItems();
					fang.Name = "vampire teeth";
					fang.ItemID = 0x5738;
					fang.Hue = 0x47E;
					b.PackItem( fang );
			}
			else if ( reg.IsPartOf( "the Lodoria Catacombs" ) && m is RottingCorpse && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5502, 1806, Map.Lodor ) )
			{
				m.EmoteHue = 123;
				m.Title = "of the ancient king";

				Item head = new SummonItems();
					head.Name = "face of the ancient king";
					head.ItemID = 0x1CE1;
					head.Hue = 0;
					b.PackItem( head );
			}
			else if ( reg.IsPartOf( "Dungeon Deceit" ) && m is LichLord && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5318, 749, Map.Lodor ) )
			{
				m.EmoteHue = 123;
				m.Name = "Talosh";
				m.Title = "the wizard of fear";
				Heat = 4;

				Item wand = new SummonItems();
					wand.Name = "wand of Talosh";
					wand.ItemID = 0xDF4;
					wand.Hue = 0;
					b.PackItem( wand );
			}
			else if ( reg.IsPartOf( "Dungeon Despise" ) && m is Troll && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5503, 921, Map.Lodor ) )
			{
				m.EmoteHue = 123;
				m.Name = "Urg";
				m.Title = "the troll warlord";
				m.Hue = 0xA50;
				Heat = 6;

				Item urg = new SummonItems();
					urg.Name = "head of Urg";
					urg.ItemID = 0x0919;
					urg.Hue = 0xA50;
					b.PackItem( urg );
			}
			else if ( reg.IsPartOf( "Dungeon Destard" ) && m is ShadowWyrm && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5132, 852, Map.Lodor ) )
			{
				m.EmoteHue = 123;
				m.Name = "Dramulox";
				m.Title = "of the shadows";

				Item fire = new SummonItems();
					fire.Name = "flame of Dramulox";
					fire.ItemID = 0xDE3;
					fire.Hue = 0;
					b.PackItem( fire );
			}
			else if ( reg.IsPartOf( "the City of Embers" ) && m is LichLord && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5667, 1314, Map.Lodor ) )
			{
				m.EmoteHue = 123;
				m.Name = "Vorgol";
				m.Title = "the baron of flame";
				m.Hue = 0x9C6;
				Heat = 4;

				Item crown = new SummonItems();
					crown.Name = "crown of Vorgol";
					crown.ItemID = 0x3166;
					crown.Hue = 0x9C6;
					b.PackItem( crown );
			}
			else if ( reg.IsPartOf( "Dungeon Hythloth" ) && m is Daemon && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 6111, 84, Map.Lodor ) )
			{
				m.EmoteHue = 123;
				m.Name = "Saramon";
				m.Body = 9;
				m.Title = "the slayer of souls";
				m.Hue = 0x9C6;
				Heat = 4;

				Item claw = new SummonItems();
					claw.Name = "claw of Saramon";
					claw.ItemID = 0x5721;
					claw.Hue = 0x9C6;
					b.PackItem( claw );
			}
			else if ( reg.IsPartOf( "the Ice Fiend Lair" ) && m is Daemon && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5672, 326, Map.Lodor ) )
			{
				m.EmoteHue = 123;
				m.Title = "of the frozen hells";
				m.Body = 88;
				Heat = 4;

				Item horn = new SummonItems();
					horn.Name = "horn of the frozen hells";
					horn.ItemID = 0x2DB7;
					horn.Hue = 0x480;
					b.PackItem( horn );
			}
			else if ( reg.IsPartOf( "Dungeon Shame" ) && m is WaterElemental && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5596, 219, Map.Lodor ) )
			{
				m.EmoteHue = 123;
				m.Name = "a salt water elemental";
				m.Hue = 0x48D;
				Heat = 4;

				Item salt = new SummonItems();
					salt.Name = "elemental salt";
					salt.ItemID = 0x423A;
					salt.Hue = 0x47E;
					b.PackItem( salt );
			}
			else if ( reg.IsPartOf( "Terathan Keep" ) && m is Dragons && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5307, 1611, Map.Lodor ) )
			{
				m.EmoteHue = 123;
				m.Name = NameList.RandomName( "dragon" );
				m.Title = "the dragon of blight";
				m.Hue = 0x9C4;
				Heat = 4;

				Item plague = new SummonItems();
					plague.Name = "eye of plagues";
					plague.ItemID = 0x3199;
					plague.Hue = 0x9C9;
					b.PackItem( plague );
			}
			else if ( reg.IsPartOf( "the Halls of Undermountain" ) && m is WeedElemental && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5332, 478, Map.Lodor ) )
			{
				m.EmoteHue = 123;
				m.Name = "a tangle weed";

				Item weed = new SummonItems();
					weed.Name = "hair of the earth";
					weed.ItemID = 0xCB0;
					b.PackItem( weed );
			}
			else if ( reg.IsPartOf( "the Volcanic Cave" ) && m is FireGiant && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5994, 3414, Map.Lodor ) )
			{
				m.EmoteHue = 123;
				m.Name = "Turlox";
				m.Title = "the warlord of the sun";
				m.Hue = 0xB73;

				Item skullsun = new SummonItems();
					skullsun.Name = "skull of Turlox";
					skullsun.ItemID = 0x2203;
					skullsun.Hue = 0x54F;
					b.PackItem( skullsun );
			}
			else if ( reg.IsPartOf( "the Mausoleum" ) && m is AncientLich && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 3827, 3299, Map.Sosaria ) )
			{
				m.EmoteHue = 123;
				m.Name = "Mezlo";
				m.Title = "of the green death";
				m.Hue = 0x58B;

				Item mezlo = new SummonItems();
					mezlo.Name = "tattered robe of Mezlo";
					mezlo.ItemID = 0x3174;
					mezlo.Hue = 0x54F;
					b.PackItem( mezlo );
			}
			else if ( reg.IsPartOf( "the Tower of Brass" ) && m is Daemon && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 6519, 3572, Map.Sosaria ) )
			{
				m.EmoteHue = 123;
				m.Title = "of the dark forest";
				m.Hue = 0xA60;

				Item glood = new SummonItems();
					glood.Name = "blood of the forest";
					glood.ItemID = 0x122A;
					glood.Hue = 0xA60;
					b.PackItem( glood );
			}
			else if ( reg.IsPartOf( "Vordo's Dungeon" ) && m is MagmaElemental && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 6470, 466, Map.Sosaria ) )
			{
				m.EmoteHue = 123;
				m.Name = "a magma flow";
				m.Hue = 0x550;

				Item inferno = new SummonItems();
					inferno.Name = "cinders of life";
					inferno.ItemID = 0x223A;
					inferno.Hue = 0x550;
					b.PackItem( inferno );
			}
			else if ( reg.IsPartOf( "the Dragon's Maw" ) && m is CrystalDragon && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 4498, 3924, Map.Sosaria ) )
			{
				m.EmoteHue = 123;
				Item crysts = new SummonItems();
					crysts.Name = "crystal scales";
					crysts.ItemID = 0x2248;
					crysts.Hue = 0xA0B;
					b.PackItem( crysts );
			}
			else if ( reg.IsPartOf( "the Ancient Pyramid" ) && m is Lich && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5325, 957, Map.Sosaria ) )
			{
				m.EmoteHue = 123;
				m.Title = "the pharaoh of suffering";
				m.Hue = 0x9C7;

				Item suffer = new SummonItems();
					suffer.Name = "chest of suffering";
					suffer.ItemID = 0x1B17;
					suffer.Hue = 0x9C7;
					b.PackItem( suffer );
			}
			else if ( reg.IsPartOf( "Dungeon Exodus" ) && m is Daemon && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5944, 628, Map.Sosaria ) )
			{
				m.EmoteHue = 123;
				m.Body = 9;
				m.Title = "the torturer from below";
				m.Hue = 0x9D3;

				Item whip = new SummonItems();
					whip.Name = "whip from below";
					whip.ItemID = 0x166E;
					whip.Hue = 0;
					b.PackItem( whip );
			}
			else if ( reg.IsPartOf( "the Caverns of Poseidon" ) && m is WaterNaga && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5902, 1769, Map.Sosaria ) )
			{
				m.EmoteHue = 123;
				m.Name = NameList.RandomName( "evil witch" );
				m.Title = "the naga from the deep";
				m.Hue = 0xA09;

				Item scaly = new SummonItems();
					scaly.Name = "scale of the sea";
					scaly.ItemID = 0x26B5;
					scaly.Hue = 0xA09;
					b.PackItem( scaly );
			}
			else if ( reg.IsPartOf( "Dungeon Clues" ) && m is Titan && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5971, 2232, Map.Sosaria ) )
			{
				m.EmoteHue = 123;
				m.Name = "Marxas";
				m.Title = "the titan of war";
				Heat = 3;

				Item warb = new SummonItems();
					warb.Name = "braclet of war";
					warb.ItemID = 0x4212;
					warb.Hue = 0x9D3;
					b.PackItem( warb );
			}
			else if ( reg.IsPartOf( "Dardin's Pit" ) && m is WalkingReaper && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5616, 400, Map.Sosaria ) )
			{
				m.EmoteHue = 123;
				m.Name = NameList.RandomName( "trees" );
				m.Title = "the ancient reaper";
				Heat = 4;

				Item stump = new SummonItems();
					stump.Name = "stump of the ancients";
					stump.ItemID = 0xE57;
					stump.Hue = m.Hue;
					b.PackItem( stump );
			}
			else if ( reg.IsPartOf( "Dungeon Doom" ) && m is BloodElemental && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5325, 331, Map.Sosaria ) )
			{
				m.EmoteHue = 123;
				m.Name = "a dark blood elemental";
				m.Hue = 0x5B5;

				Item dlood = new SummonItems();
					dlood.Name = "dark blood";
					dlood.ItemID = 0x122D;
					dlood.Hue = 0x5B5;
					b.PackItem( dlood );
			}
			else if ( reg.IsPartOf( "the Fires of Hell" ) && m is Drake && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5712, 1280, Map.Sosaria ) )
			{
				m.EmoteHue = 123;
				m.Name = "a firescale drake";
				m.Hue = 0x54C;

				Item tooth = new SummonItems();
					tooth.Name = "firescale tooth";
					tooth.ItemID = 0x5747;
					tooth.Hue = 0x54C;
					b.PackItem( tooth );
			}
			else if ( reg.IsPartOf( "the Mines of Morinia" ) && m is AntaurKing )
			{
				m.EmoteHue = 123;

				Item ichor = new SummonItems();
					ichor.Name = "ichor of Xthizx";
					ichor.ItemID = 0x2827;
					ichor.Hue = 0xB96;
					b.PackItem( ichor );
			}
			else if ( reg.IsPartOf( "the Perinian Depths" ) && m is VampireLord && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5918, 419, Map.Sosaria ) )
			{
				m.EmoteHue = 123;
				m.Title = "the vampire queen";
				Server.Misc.MorphingTime.VampireDressUp( m, 606 );
				Heat = 3;

				Item vamph = new SummonItems();
					vamph.Name = "heart of a vampire queen";
					vamph.ItemID = 0x24B;
					vamph.Hue = 0;
					b.PackItem( vamph );
			}
			else if ( reg.IsPartOf( "the Dungeon of Time Awaits" ) && m is Daemon && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 5736, 793, Map.Sosaria ) )
			{
				m.EmoteHue = 123;
				m.Body = 9;
				m.Title = "the daemon of ages";
				m.Hue = 0xA65;
				Heat = 2;

				Item hour = new SummonItems();
					hour.Name = "hourglass of ages";
					hour.ItemID = 0x1810;
					hour.Hue = 0xB90;
					b.PackItem( hour );
			}
			else if ( reg.IsPartOf( "the Ancient Prison" ) && m is DeadWizard && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 1928, 569, Map.SerpentIsland ) )
			{
				m.Body = 0x190; 
				m.EmoteHue = 123;
				m.Name = "Saramak";
				m.Title = "the forgotten prisoner";

				Item hour = new SummonItems();
					hour.Name = "shackles of Saramak";
					hour.ItemID = 0x1262;
					hour.Hue = 0;
					b.PackItem( hour );
			}
			else if ( reg.IsPartOf( "the Cave of Fire" ) && ( m is Dragons || m is RidingDragon ) && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 2052, 911, Map.SerpentIsland ) )
			{
				m.EmoteHue = 123;
				m.Name = NameList.RandomName( "dragon" );
				m.Title = "the dragon of embers";
				m.Hue = 0x501;
				Heat = 4;

				Item emberh = new SummonItems();
					emberh.Name = "mouth of embers";
					emberh.ItemID = 0x2DB4;
					emberh.Hue = 0x501;
					b.PackItem( emberh );
			}
			else if ( reg.IsPartOf( "the Cave of Souls" ) && m is RottingCorpse && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 2466, 153, Map.SerpentIsland ) )
			{
				m.EmoteHue = 123;
				m.Name = "a zombie";
				m.Title = "of the shadegloom thief";

				Item shadeg = new SummonItems();
					shadeg.Name = "cowl of shadegloom";
					shadeg.ItemID = 0x278F;
					shadeg.Hue = 0;
					b.PackItem( shadeg );
			}
			else if ( reg.IsPartOf( "Dungeon Ankh" ) && m is DeadWizard && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 2044, 174, Map.SerpentIsland ) )
			{
				m.Body = 0x191; 
				m.EmoteHue = 123;
				m.Name = NameList.RandomName( "female" ); 
				m.Title = "the dutchess of virtue";

				Item dress = new SummonItems();
					dress.Name = "wedding dress of virtue";
					dress.ItemID = 0x1F00;
					dress.Hue = 0;
					b.PackItem( dress );
			}
			else if ( reg.IsPartOf( "Dungeon Bane" ) && m is ToxicElemental && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 1973, 224, Map.SerpentIsland ) )
			{
				m.EmoteHue = 123;
				m.Name = "a swamp elemental";
				m.Hue = 0xA04;

				Item lilly = new SummonItems();
					lilly.Name = "lilly pad of the bog";
					lilly.ItemID = 0xDBC;
					lilly.Hue = 0;
					b.PackItem( lilly );
			}
			else if ( reg.IsPartOf( "Dungeon Hate" ) && m is VampireLord && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 2229, 389, Map.SerpentIsland ) )
			{
				m.EmoteHue = 123;
				m.Title = "the immortal one";
				Server.Misc.MorphingTime.VampireDressUp( m, 605 );
				Heat = 4;

				Item boni = new SummonItems();
					boni.Name = "immortal bones";
					boni.ItemID = 0x1B10;
					boni.Hue = 0x66C;
					b.PackItem( boni );
			}
			else if ( reg.IsPartOf( "Dungeon Scorn" ) && m is OphidianArchmage && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 2237, 812, Map.SerpentIsland ) )
			{
				m.EmoteHue = 123;
				m.Name = "Sylpha";
				m.Title = "the princess of scorn";

				Item stafs = new SummonItems();
					stafs.Name = "staff of scorn";
					stafs.ItemID = 0x2556;
					stafs.Hue = 0;
					b.PackItem( stafs );
			}
			else if ( reg.IsPartOf( "Dungeon Torment" ) && m is Succubus && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 1977, 839, Map.SerpentIsland ) )
			{
				m.EmoteHue = 123;
				m.Name = "Hertana";
				m.Title = "of vile allurement";

				Item brain = new SummonItems();
					brain.Name = "mind of allurement";
					brain.ItemID = 0x1CF0;
					brain.Hue = 0;
					b.PackItem( brain );
			}
			else if ( reg.IsPartOf( "Dungeon Vile" ) && m is EvilMage && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 2336, 495, Map.SerpentIsland ) )
			{
				m.EmoteHue = 123;
				m.Title = "the wanderer of mystics";
				Heat = 4;

				Item masky = new WornHumanDeco();
					masky.Name = "mask of the ghost";
					masky.ItemID = 0x154B;
					masky.Hue = 0x47E;
					masky.Layer = Layer.Ring;
					b.AddItem( masky );

				Item mask = new SummonItems();
					mask.Name = "mask of the ghost";
					mask.ItemID = 0x154B;
					mask.Hue = 0x47E;
					b.PackItem( mask );
			}
			else if ( reg.IsPartOf( "Dungeon Wicked" ) && m is PoisonElemental && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 2180, 208, Map.SerpentIsland ) )
			{
				m.EmoteHue = 123;
				m.Name = "an insect swarm";
				m.Hue = 0xA04;

				Item flies = new SummonItems();
					flies.Name = "dead venom flies";
					flies.ItemID = 0xF34;
					flies.Hue = 0xA04;
					b.PackItem( flies );
			}
			else if ( reg.IsPartOf( "Dungeon Wrath" ) && m is Reaper && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 2334, 861, Map.SerpentIsland ) ) // SWAMPY AREA
			{
				m.EmoteHue = 123;
				m.Name = "a reaping willow";
				Heat = 4;

				Item branch = new SummonItems();
					branch.Name = "branch of the reaper";
					branch.ItemID = 0x3AD9;
					branch.Hue = m.Hue;
					b.PackItem( branch );
			}
			else if ( reg.IsPartOf( "the Flooded Temple" ) && m is Kraken && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 2447, 872, Map.SerpentIsland ) )
			{
				m.EmoteHue = 123;
				m.Name = "a deep sea squid";
				m.Hue = 0xA1F;

				Item ink = new SummonItems();
					ink.Name = "ink of the deep";
					ink.ItemID = 0x1D96;
					ink.Hue = 0x969;
					b.PackItem( ink );
			}
			else if ( reg.IsPartOf( "the Gargoyle Crypts" ) && m is SpectralGargoyle && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 2047, 548, Map.SerpentIsland ) )
			{
				m.EmoteHue = 123;
				m.Name = "a spirit";
				m.Title = "of a gargoyle priest";

				Item ink = new SummonItems();
					ink.Name = "amulet of the stygian abyss";
					ink.ItemID = 0x4210;
					ink.Hue = 0;
					b.PackItem( ink );
			}
			else if ( reg.IsPartOf( "the Serpent Sanctum" ) && m is OphidianKnight && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 2456, 498, Map.SerpentIsland ) )
			{
				m.EmoteHue = 123;
				m.Name = "Siluphtis";
				m.Title = "the guardian of the sanctum";

				Item snakes = new SummonItems();
					snakes.Name = "skin of the guardian";
					snakes.ItemID = 0x20FE;
					snakes.Hue = 0x842;
					b.PackItem( snakes );
			}
			else if ( reg.IsPartOf( "the Tomb of the Fallen Wizard" ) && m is AncientLich && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 2334, 32, Map.SerpentIsland ) )
			{
				m.EmoteHue = 123;
				m.Title = "the fallen wizard";

				Item orbo = new SummonItems();
					orbo.Name = "orb of the fallen wizard";
					orbo.ItemID = 0xE2E;
					orbo.Hue = 0x4A7;
					b.PackItem( orbo );
			}
			else if ( reg.IsPartOf( "the Blood Temple" ) && m is BloodElemental && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 701, 2537, Map.SavagedEmpire ) )
			{
				m.EmoteHue = 123;
				m.Name = "a bloody mist";
				m.Body = 13;
				m.Hue = 0x5B5;
				m.BaseSoundID = 655;

				Item bcry = new SummonItems();
					bcry.Name = "bleeding crystal";
					bcry.ItemID = 0x1F1C;
					bcry.Hue = 0x48E;
					b.PackItem( bcry );
			}
			else if ( reg.IsPartOf( "the Dungeon of the Mad Archmage" ) && m is Archmage && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 762, 1924, Map.SavagedEmpire ) )
			{
				m.EmoteHue = 123;
				Item jade = new SummonItems();
					jade.Name = "jade idol of Nesfatiti";
					jade.ItemID = 0x1224;
					jade.Hue = 0xB93;
					b.PackItem( jade );
			}
			else if ( reg.IsPartOf( "the Tombs" ) && m is AncientLich && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 114, 2687, Map.SavagedEmpire ) )
			{
				m.EmoteHue = 123;
				m.Title = "the seeker of the words";

				Item scrab = new SummonItems();
					scrab.Name = "scroll of Abraxus";
					scrab.ItemID = 0x227B;
					scrab.Hue = 0;
					b.PackItem( scrab );
			}
			else if ( reg.IsPartOf( "the Dungeon of the Lich King" ) && m is Demon && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 342, 2179, Map.SavagedEmpire ) )
			{
				m.EmoteHue = 123;
				m.Body = 9; 
				m.Name = "Permaxumus";
				m.Title = "the ruler of the dark circle";
				m.Hue = 0xA3A;
				m.BaseSoundID = 0x47D;

				Heat = 4;

				Item circb = new SummonItems();
					circb.Name = "sphere of the dark circle";
					circb.ItemID = 0x573E;
					circb.Hue = 0;
					b.PackItem( circb );
			}
			else if ( reg.IsPartOf( "the Forgotten Halls" ) && m is AncientLich && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 56, 3245, Map.SavagedEmpire ) ) // Shadow Lich already mutated 
			{
				m.EmoteHue = 123;
				m.Name = "Ulmarek";

				Heat = 0;

				Item urn = new SummonItems();
					urn.Name = "urn of Ulmarek's ashes";
					urn.ItemID = 0x42B3;
					urn.Hue = 0xB92;
					b.PackItem( urn );
			}
			else if ( reg.IsPartOf( "the Ice Queen Fortress" ) && m is IceColossus && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 266, 2801, Map.SavagedEmpire ) )
			{
				m.EmoteHue = 123;
				m.Name = "a greater ice elemental";

				Item frost = new SummonItems();
					frost.Name = "crystal of everfrost";
					frost.ItemID = 0x1F19;
					frost.Hue = 0x480;
					b.PackItem( frost );
			}
			else if ( reg.IsPartOf( "the Halls of Ogrimar" ) && m is OrkMage && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 950, 2335, Map.SavagedEmpire ) )
			{
				m.EmoteHue = 123;
				m.Title = "of the war wizards";

				Item tablet = new SummonItems();
					tablet.Name = "tablet of the wizard wars";
					tablet.ItemID = 0xED8;
					tablet.Hue = 0xB8B;
					b.PackItem( tablet );
			}
			else if ( reg.IsPartOf( "Dungeon Rock" ) && m is GargoyleOnyx && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 645, 2193, Map.SavagedEmpire ) ) // Obsidian Gargoyle? 
			{
				m.EmoteHue = 123;
				m.Name = NameList.RandomName( "gargoyle name" );
				m.Title = "the gargoyle of night";

				Item garst = new SummonItems();
					garst.Name = "stone of the night gargoyle";
					garst.ItemID = 0x364E;
					garst.Hue = 0;
					b.PackItem( garst );
			}
			else if ( reg.IsPartOf( "the Scurvy Reef" ) && m is DeepSeaDevil && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 369, 3866, Map.SavagedEmpire ) )
			{
				m.EmoteHue = 123;
				m.Title = "the defiler of the sea";

				Item pearl = new SummonItems();
					pearl.Name = "pearl of Neptune";
					pearl.ItemID = 0x3199;
					pearl.Hue = 0xA37;
					b.PackItem( pearl );
			}
			else if ( reg.IsPartOf( "the Undersea Castle" ) && m is SeaDragon && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 704, 3789, Map.SavagedEmpire ) )
			{
				m.EmoteHue = 123;
				m.Name = NameList.RandomName( "dragon" );
				m.Title = "the coral dragon";
				m.Hue = 0xA07;

				Item brandy = new SummonItems();
					brandy.Name = "Black Beard's brandy";
					brandy.ItemID = 0x4686;
					brandy.Hue = 0;
					b.PackItem( brandy );
			}
			else if ( reg.IsPartOf( "the Tomb of Kazibal" ) && m is Fiend && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 438, 3298, Map.SavagedEmpire ) ) //  already mutated sand demon
			{
				m.EmoteHue = 123;
				m.Body = 9;
				m.Name = "Tutamak";
				m.Hue = 0x83B;
				switch ( Utility.Random( 5 ) )
				{
					case 0: m.Title = "the sand fiend"; break;
					case 1: m.Title = "the desert fiend"; break;
					case 2: m.Title = "the fiend of the wastes"; break;
					case 3: m.Title = "the wasteland fiend"; break;
					case 4: m.Title = "the fiend of the barrens"; break;
				}

				Item lamp = new SummonItems();
					lamp.Name = "lamp of the desert";
					lamp.ItemID = 0xA16;
					lamp.Hue = 0x5B7;
					b.PackItem( lamp );
			}
			else if ( reg.IsPartOf( "the Azure Castle" ) && m is Ifreet )
			{
				m.EmoteHue = 123;
				m.Name = NameList.RandomName( "drakkul" );
				m.Title = "the soul of azure";
				m.Hue = 0x538;

				Item azure = new SummonItems();
					azure.Name = "azure dust";
					azure.ItemID = 0x2DB5;
					azure.Hue = 0x532;
					b.PackItem( azure );
			}
			else if ( reg.IsPartOf( "the Catacombs of Azerok" ) && m is DeadWizard )
			{
				m.EmoteHue = 123;
				m.Name = "Azerok";
				m.Body = 0x190; 
				m.Title = "of the Deathly Veil";

				Item skullazerok = new SummonItems();
					skullazerok.Name = "skull of Azerok";
					skullazerok.ItemID = 0x26AB;
					skullazerok.Hue = 0xB71;
					b.PackItem( skullazerok );
			}
			else if ( reg.IsPartOf( "Dungeon Covetous" ) && m is HarpyHen )
			{
				m.EmoteHue = 123;

				Item harpyegg = new SummonItems();
					harpyegg.Name = "egg of the harpy hen";
					harpyegg.ItemID = 0x41BF;
					harpyegg.Hue = 0;
					b.PackItem( harpyegg );
			}
			else if ( reg.IsPartOf( "the Glacial Scar" ) && m is FrostGiant && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 1949, 1512, Map.Underworld ) )
			{
				m.EmoteHue = 123;
				m.Name = "Murgor";
				m.Title = "the frost giant chief";
				m.Body = 325;

				Item bone = new SummonItems();
					bone.Name = "bone of the frost giant";
					bone.ItemID = 0x2559;
					bone.Hue = 0x482;
					b.PackItem( bone );
			}
			else if ( reg.IsPartOf( "the Temple of Osirus" ) && m is Drake && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 6143, 3607, Map.Lodor ) )
			{
				m.EmoteHue = 123;
				m.Name = "a silver drake";
				m.Hue = 0x430;

				Item bone = new SummonItems();
					bone.Name = "mind of silver";
					bone.ItemID = 0x1CF0;
					bone.Hue = 0x9C4;
					b.PackItem( bone );
			}
			else if ( reg.IsPartOf( "the Sanctum of Saltmarsh" ) && m is Sleestax && Server.Misc.SummonQuests.IsInLocation( b.Home.X, b.Home.Y, m.Map, 6132, 1337, Map.Lodor ) )
			{
				m.EmoteHue = 123;
				m.Name = "Scarthis";
				m.Title = "the kahn of saltmarsh";
				m.Hue = 0xB51;

				Item scale = new SummonItems();
					scale.Name = "scale of Scarthis";
					scale.ItemID = 0x26B2;
					scale.Hue = 0xB53;
					b.PackItem( scale );
			}

			Server.Misc.MorphingTime.SetGender( m );

			return Heat;
		}

		public static bool IsInLocation( int myX, int myY, Map myMap, int spotX, int spotY, Map spotMap )
		{
			if ( myMap == spotMap && myX == spotX && myY == spotY )
			{
				return true;
			}

			return false;
		}

		public static void WellTheyDied( Mobile m, BaseCreature b )
		{
			if ( m.EmoteHue == 505 )
			{
				PlayerMobile killer = MobileUtilities.TryGetKillingPlayer( m );

				Map map = m.Map;

				if ( map != null )
				{
					for ( int x = -12; x <= 12; ++x )
					{
						for ( int y = -12; y <= 12; ++y )
						{
							double dist = Math.Sqrt(x*x+y*y);

							if ( dist <= 12 )
								new GoodiesTimer( map, m.X + x, m.Y + y ).Start();
						}
					}

					SummonChest MyChest = new SummonChest( killer );
					MyChest.Prisoner = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((m.Name).ToLower());
					MyChest.AddItem(new HoardMinionFamiliarItem());

					string myName = GetFirstName( m.Name );
					int myHue = m.Hue;

					Item reward = new SummonReward();
					foreach( Item i in m.Backpack.Items )
					{
						if ( i is SummonPrison )
						{
							SummonPrison prison = (SummonPrison)i;
							reward.Hue = prison.RewardHue;
							reward.ItemID = prison.RewardID;
							reward.Name = prison.RewardName;
							MyChest.AddItem( reward );

							if ( prison.PrisonerFullNameUsed > 0 )
							{
								myName = m.Name;
							}
							if ( prison.PrisonerClothColorUsed > 0 )
							{
								for ( int c = 0; c < m.Items.Count; ++c )
								{
									Item item = m.Items[c];

									if ( !( item is Cloak ) && !( item is BaseWeapon ) && !( item is WornHumanDeco ) )
									{
										myHue = item.Hue;
									}
								}

								if ( myHue == 0 ){ myHue = prison.RewardHue; }
							}

							CustomEventSink.InvokeCombatQuestCompleted( killer, 10000 );
							break;
						}
					}

					if ( killer is PlayerMobile )
					{
						Party p = Engines.PartySystem.Party.Get( killer );
						if ( p != null )
						{
							foreach ( PartyMemberInfo pmi in p.Members )
							{
								if ( pmi.Mobile is PlayerMobile && pmi.Mobile.InRange(m.Location, 20) )
								{
									LoggingFunctions.LogSlayingLord( (PlayerMobile)pmi.Mobile, m.Name + " from the Magical Prison" );
									Titles.AwardFame( pmi.Mobile, 300, true );
									if ( ((PlayerMobile)(pmi.Mobile)).KarmaLocked == true ){ Titles.AwardKarma( pmi.Mobile, -300, true ); }
									else { Titles.AwardKarma( pmi.Mobile, 300, true ); }

									ManualOfItems book = new ManualOfItems();
										book.Hue = myHue;
										book.Name = "Chest of " + myName + " Relics";
										book.m_Charges = 1;
										book.m_Skill_1 = 99;
										book.m_Skill_2 = 0;
										book.m_Skill_3 = 0;
										book.m_Skill_4 = 0;
										book.m_Skill_5 = 0;
										book.m_Value_1 = 10.0;
										book.m_Value_2 = 0.0;
										book.m_Value_3 = 0.0;
										book.m_Value_4 = 0.0;
										book.m_Value_5 = 0.0;
										book.m_Slayer_1 = 5;
										book.m_Slayer_2 = 0;
										book.m_Owner = pmi.Mobile;
										book.m_Extra = "of " + myName;
										book.m_FromWho = "From " + m.Name;
										book.m_HowGiven = "Won by";
										book.m_Points = 250;
										book.m_Hue = myHue;
										MyChest.AddItem( book );
								}
							}
						}
						else
						{
							LoggingFunctions.LogSlayingLord( killer, m.Name + " from the Magical Prison" );
							Titles.AwardFame( killer, 300, true );
							if ( ((PlayerMobile)killer).KarmaLocked == true ){ Titles.AwardKarma( killer, -300, true ); }
							else { Titles.AwardKarma( killer, 300, true ); }

							ManualOfItems book = new ManualOfItems();
								book.Hue = myHue;
								book.Name = "Chest of " + myName + " Relics";
								book.m_Charges = 1;
								book.m_Skill_1 = 99;
								book.m_Skill_2 = 0;
								book.m_Skill_3 = 0;
								book.m_Skill_4 = 0;
								book.m_Skill_5 = 0;
								book.m_Value_1 = 10.0;
								book.m_Value_2 = 0.0;
								book.m_Value_3 = 0.0;
								book.m_Value_4 = 0.0;
								book.m_Value_5 = 0.0;
								book.m_Slayer_1 = 5;
								book.m_Slayer_2 = 0;
								book.m_Owner = killer;
								book.m_Extra = "of " + myName;
								book.m_FromWho = "From " + m.Name;
								book.m_HowGiven = "Won by";
								book.m_Points = 250;
								book.m_Hue = myHue;
								MyChest.AddItem( book );
						}
					}

					MyChest.MoveToWorld(m.Location, m.Map);
				}

				if (!(m is PlayerMobile))
				{
					Server.Misc.IntelligentAction.BurnAway( m );
					m.Delete();
				}
			}
		}

		public static string GetFirstName( string word )
		{
			string name = "";
			string[] names = word.Split(' ');
			int nEntry = 1;
			foreach (string text in names)
			{
				if ( nEntry == 1 ){ name = text; }
				nEntry++;
			}
			return name;
		}

		public class GoodiesTimer : Timer
		{
			private Map m_Map;
			private int m_X, m_Y;

			public GoodiesTimer( Map map, int x, int y ) : base( TimeSpan.FromSeconds( Utility.RandomDouble() * 5.0 ) )
			{
				m_Map = map;
				m_X = x;
				m_Y = y;
			}

			protected override void OnTick()
			{
				int z = m_Map.GetAverageZ( m_X, m_Y );
				bool canFit = m_Map.CanFit( m_X, m_Y, z, 6, false, false );

				for ( int i = -3; !canFit && i <= 3; ++i )
				{
					canFit = m_Map.CanFit( m_X, m_Y, z + i, 6, false, false );

					if ( canFit )
						z += i;
				}

				if ( !canFit )
					return;

				Item g = new Gold( 100, 200 ); g.Delete();

				int r1 = (int)( Utility.RandomMinMax( 80, 160 ) * (MyServerSettings.GetGoldCutRate() * .01) );
				int r2 = (int)( Utility.RandomMinMax( 200, 400 ) * (MyServerSettings.GetGoldCutRate() * .01) );
				int r3 = (int)( Utility.RandomMinMax( 400, 800 ) * (MyServerSettings.GetGoldCutRate() * .01) );
				int r4 = (int)( Utility.RandomMinMax( 800, 1200 ) * (MyServerSettings.GetGoldCutRate() * .01) );
				int r5 = (int)( Utility.RandomMinMax( 1200, 1600 ) * (MyServerSettings.GetGoldCutRate() * .01) );

				switch ( Utility.Random( 21 ) )
				{
					case 0: g = new Crystals( r1 ); break;
					case 1: g = new DDGemstones( r2 ); break;
					case 2: g = new DDJewels( r2 ); break;
					case 3: g = new DDGoldNuggets( r3 ); break;
					case 4: g = new Gold( r3 ); break;
					case 5: g = new Gold( r3 ); break;
					case 6: g = new Gold( r3 ); break;
					case 7: g = new DDSilver( r4 ); break;
					case 8: g = new DDSilver( r4 ); break;
					case 9: g = new DDSilver( r4 ); break;
					case 10: g = new DDSilver( r4 ); break;
					case 11: g = new DDSilver( r4 ); break;
					case 12: g = new DDSilver( r4 ); break;
					case 13: g = new DDCopper( r5 ); break;
					case 14: g = new DDCopper( r5 ); break;
					case 15: g = new DDCopper( r5 ); break;
					case 16: g = new DDCopper( r5 ); break;
					case 17: g = new DDCopper( r5 ); break;
					case 18: g = new DDCopper( r5 ); break;
					case 19: g = new DDCopper( r5 ); break;
					case 20: g = new DDCopper( r5 ); break;
				}

				g.MoveToWorld( new Point3D( m_X, m_Y, z ), m_Map );

				if ( 0.5 >= Utility.RandomDouble() )
				{
					switch ( Utility.Random( 3 ) )
					{
						case 0: // Fire column
						{
							Effects.SendLocationParticles( EffectItem.Create( g.Location, g.Map, EffectItem.DefaultDuration ), 0x3709, 10, 30, 5052 );
							Effects.PlaySound( g, g.Map, 0x208 );

							break;
						}
						case 1: // Explosion
						{
							Effects.SendLocationParticles( EffectItem.Create( g.Location, g.Map, EffectItem.DefaultDuration ), 0x36BD, 20, 10, 5044 );
							Effects.PlaySound( g, g.Map, 0x307 );

							break;
						}
						case 2: // Ball of fire
						{
							Effects.SendLocationParticles( EffectItem.Create( g.Location, g.Map, EffectItem.DefaultDuration ), 0x36FE, 10, 10, 5052 );

							break;
						}
					}
				}
			}
		}
	}
}