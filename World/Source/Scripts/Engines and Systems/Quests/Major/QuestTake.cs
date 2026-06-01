using Server;
using System;
using System.Collections;
using Server.Network;
using Server.Misc;
using Server.Mobiles;
using System.Globalization;

namespace Server.Items
{
	public class QuestTake : Item
	{
		public Map DeliverMap;
		public int DeliverX;
		public int DeliverY;

		[Constructable]
		public QuestTake() : base( 0x1A97 )
		{
			Weight = 1.0;
			Name = "Journal of " + RandomThings.GetRandomName() + " the ";
				if ( Utility.RandomBool() ){ Name = Name + RandomThings.GetBoyGirlJob( 0 ); }
				else { Name = Name + RandomThings.GetBoyGirlJob( 1 ); }

			ItemID = Utility.RandomList( 0x65CC, 0x65CD, 0x1A97, 0x1A98, 0x1AA3, 0x2205, 0x220F, 0x2219, 0x2223, 0x222D, 0x2255, 0x225C, 0x225D, 0x225E, 0x225F, 0x5688, 0x5689 );
				if ( ItemID == 0x1A97 || ItemID == 0x1A98 || ItemID == 0x1AA3 || ItemID == 0x5688 || ItemID == 0x5689 ){ Hue = Utility.RandomColor(0); }
		}

		public override bool OnDragLift( Mobile from )
		{
			OnDoubleClick( from );
			return false;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from is PlayerMobile )
			{
				bool HasTome = false;

				ArrayList targets = new ArrayList();
				foreach ( Item item in World.Items.Values )
				{
					if ( item is QuestTome )
					{
						if ( ((QuestTome)item).QuestTomeOwner == from )
						{
							targets.Add( item );
							HasTome = true;
						}
					}
				}
				for ( int i = 0; i < targets.Count; ++i )
				{
					Item item = ( Item )targets[ i ];
					item.ItemID = ItemID;
					item.Name = Name;
					item.Hue = Hue;
					from.AddToBackpack( item );
					from.SendMessage( "You take possession of the book!" );
					from.SendSound( 0x3D );
					this.Delete();
				}

				if ( !HasTome )
				{
					SetupBook( from );
					from.SendMessage( "You take possession of the book!" );
					from.SendSound( 0x3D );
					LoggingFunctions.LogGeneric( from, "has found a Book of Questing." );
					this.Delete();
				}
			}
		}

		public void SetupBook( Mobile from )
		{
			QuestTome tome = new QuestTome();

			tome.QuestTomeOwner = from;
			tome.ItemID = ItemID;
			tome.Hue = Hue;
			tome.Name = Name;

			NPCGood( from, tome );
			NPCEvil( from, tome );

			tome.QuestTomeCitizen = "";
			tome.QuestTomeGoals = 0;
			tome.QuestTomeDungeon = "";
			tome.QuestTomeLand = Land.None;
			tome.QuestTomeType = 0;

			switch ( Utility.RandomMinMax( 0, 3 ) )
			{
				case 0:	tome.GoalItem1 = Server.Misc.QuestCharacters.QuestItems( false );	break;
				case 1:	tome.GoalItem1 = Server.Misc.QuestCharacters.QuestItems( false );	break;
				case 2:	tome.GoalItem1 = RandomHerb();										break;
				case 3:	tome.GoalItem1 = RandomMagic();										break;
			}
			switch ( Utility.RandomMinMax( 0, 3 ) )
			{
				case 0:	tome.GoalItem2 = Server.Misc.QuestCharacters.QuestItems( false );	break;
				case 1:	tome.GoalItem2 = Server.Misc.QuestCharacters.QuestItems( false );	break;
				case 2:	tome.GoalItem2 = RandomHerb();										break;
				case 3:	tome.GoalItem2 = RandomMagic();										break;
			}
			switch ( Utility.RandomMinMax( 0, 3 ) )
			{
				case 0:	tome.GoalItem3 = Server.Misc.QuestCharacters.QuestItems( false );	break;
				case 1:	tome.GoalItem3 = Server.Misc.QuestCharacters.QuestItems( false );	break;
				case 2:	tome.GoalItem3 = RandomHerb();										break;
				case 3:	tome.GoalItem3 = RandomMagic();										break;
			}

			string heard = "heard";
			switch ( Utility.RandomMinMax( 0, 3 ) )
			{
				case 1:	heard = "told";		break;
				case 2:	heard = "known";	break;
				case 3:	heard = "shared";	break;
			}

			string legend = "legends";
			switch ( Utility.RandomMinMax( 0, 3 ) )
			{
				case 1:	legend = "fables";	break;
				case 2:	legend = "myths";	break;
				case 3:	legend = "lore";	break;
			}

			string hush = "whispered";
			switch ( Utility.RandomMinMax( 0, 3 ) )
			{
				case 1:	hush = "told";		break;
				case 2:	hush = "sung";		break;
				case 3:	hush = "spoken";	break;
			}

			string inn = "taverns";
			switch ( Utility.RandomMinMax( 0, 4 ) )
			{
				case 1:	inn = "camps";		break;
				case 2:	inn = "cities";		break;
				case 3:	inn = "villages";	break;
				case 4:	inn = "inns";		break;
			}

			string takes = "seized";
			switch ( Utility.RandomMinMax( 0, 4 ) )
			{
				case 1:	takes = "stolen";	break;
				case 2:	takes = "taken";	break;
				case 3:	takes = "held";		break;
				case 4:	takes = "guarded";	break;
			}

			tome.GoalItem4 = Server.Misc.QuestCharacters.QuestItems( false );

			MakeVillain( tome );

			tome.QuestTomeStoryGood = "You have found the journal of DDDDD, where they were given a quest by " + tome.QuestTomeNPCGood + " to find " + tome.GoalItem4 + " that is known to be " + takes + " by " + tome.VillainName + " " + tome.VillainTitle + ". " + tome.VillainName + " is " + tome.VillainCategory + " that has been " + heard + " in " + legend + " and " + hush + " about in " + inn + ". The goal for DDDDD was to find " + tome.GoalItem1 + ", " + tome.GoalItem2 + ", & " + tome.GoalItem3 + " to help them defeat " + tome.VillainName + " and then bring " + tome.GoalItem4 + " back to " + tome.QuestTomeNPCGood + " before " + tome.QuestTomeNPCEvil + " can use it for their nefarious plans.<br><br>This is now your quest and you will have to speak with others to find clues on the location of the relics needed, as well as where " + tome.VillainName + " dwells. Once you defeat " + tome.VillainName + " and claim " + tome.GoalItem4 + ", you can give this journal to " + tome.QuestTomeNPCGood + " in " + tome.QuestTomeWorldGood + " at the following coordinates:<br><br>" + tome.QuestTomeLocateGood + "";

			tome.QuestTomeStoryEvil = "You have found the journal of DDDDD, where they were given a quest by " + tome.QuestTomeNPCEvil + " to find " + tome.GoalItem4 + " that is known to be " + takes + " by " + tome.VillainName + " " + tome.VillainTitle + ". " + tome.VillainName + " is " + tome.VillainCategory + " that has been " + heard + " in " + legend + " and " + hush + " about in " + inn + ". The goal for DDDDD was to find " + tome.GoalItem1 + ", " + tome.GoalItem2 + ", & " + tome.GoalItem3 + " to help them defeat " + tome.VillainName + " and then bring " + tome.GoalItem4 + " back to " + tome.QuestTomeNPCEvil + " before " + tome.QuestTomeNPCGood + " can use it for their righteous pursuits.<br><br>This is now your quest and you will have to speak with others to find clues on the location of the relics needed, as well as where " + tome.VillainName + " dwells. Once you defeat " + tome.VillainName + " and claim " + tome.GoalItem4 + ", you can give this journal to " + tome.QuestTomeNPCEvil + " in " + tome.QuestTomeWorldEvil + " at the following coordinates:<br><br>" + tome.QuestTomeLocateEvil + "";

			if ( Utility.RandomBool() ) // TWIST THE STORY AROUND
			{
				tome.QuestTomeStoryGood = "You have found the journal of DDDDD, where they were given a quest by " + tome.QuestTomeNPCEvil + " to find " + tome.GoalItem4 + " that is known to be " + takes + " by " + tome.VillainName + " " + tome.VillainTitle + ". " + tome.VillainName + " is " + tome.VillainCategory + " that has been " + heard + " in " + legend + " and " + hush + " about in " + inn + ". The goal for DDDDD was to find " + tome.GoalItem1 + ", " + tome.GoalItem2 + ", & " + tome.GoalItem3 + " to help them defeat " + tome.VillainName + " and then bring " + tome.GoalItem4 + " back to " + tome.QuestTomeNPCEvil + " before " + tome.QuestTomeNPCGood + " can use it for their righteous pursuits.<br><br>It is now your quest to keep " + tome.QuestTomeNPCEvil + " from obtaining " + tome.GoalItem4 + ". To find it for " + tome.QuestTomeNPCGood + ", you will have to speak with others to find clues on the location of the relics needed, as well as where " + tome.VillainName + " dwells. Once you defeat " + tome.VillainName + " and claim " + tome.GoalItem4 + ", you can give this journal to " + tome.QuestTomeNPCGood + " in " + tome.QuestTomeWorldGood + " at the following coordinates:<br><br>" + tome.QuestTomeLocateGood + "";

				tome.QuestTomeStoryEvil = "You have found the journal of DDDDD, where they were given a quest by " + tome.QuestTomeNPCGood + " to find " + tome.GoalItem4 + " that is known to be " + takes + " by " + tome.VillainName + " " + tome.VillainTitle + ". " + tome.VillainName + " is " + tome.VillainCategory + " that has been " + heard + " in " + legend + " and " + hush + " about in " + inn + ". The goal for DDDDD was to find " + tome.GoalItem1 + ", " + tome.GoalItem2 + ", & " + tome.GoalItem3 + " to help them defeat " + tome.VillainName + " and then bring " + tome.GoalItem4 + " back to " + tome.QuestTomeNPCGood + " before " + tome.QuestTomeNPCEvil + " can use it for their nefarious plans.<br><br>It is now your quest to keep " + tome.QuestTomeNPCGood + " from obtaining " + tome.GoalItem4 + ". To find it for " + tome.QuestTomeNPCEvil + ", you will have to speak with others to find clues on the location of the relics needed, as well as where " + tome.VillainName + " dwells. Once you defeat " + tome.VillainName + " and claim " + tome.GoalItem4 + ", you can give this journal to " + tome.QuestTomeNPCEvil + " in " + tome.QuestTomeWorldEvil + " at the following coordinates:<br><br>" + tome.QuestTomeLocateEvil + "";
			}

			from.AddToBackpack( tome );
		}

		public string RandomHerb()
		{
			string herb = "bottle of dragon tears";

			Item bottle = new DDRelicReagent();
			herb = bottle.Name;
			bottle.Delete();

			switch ( Utility.RandomMinMax( 0, 8 ) )
			{
				case 0:	herb = "mystical " + herb;		break;
				case 1:	herb = "magical " + herb;		break;
				case 2:	herb = "enchanted " + herb;		break;
				case 3:	herb = "cursed " + herb;		break;
				case 4:	herb = "ancient " + herb;		break;
				case 5:	herb = "tainted " + herb;		break;
				case 6:	herb = "charmed " + herb;		break;
				case 7:	herb = "tainted " + herb;		break;
				case 8:	herb = "ensorcelled " + herb;	break;
			}

			TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;
			herb = "" + cultInfo.ToTitleCase(herb) + "";

			return herb;
		}

		public string RandomMagic()
		{
			string spell = "Merlin's Scroll of Acidic Storm";

			Item scroll = new DDRelicScrolls();
			spell = scroll.Name;
			scroll.Delete();

			string inner = "Spell of ";

			switch ( Utility.RandomMinMax( 0, 5 ) )
			{
				case 0:	inner = "Spell of ";		break;
				case 1:	inner = "Scroll of ";		break;
				case 2:	inner = "Parchment of ";	break;
				case 3:	inner = "Wand of ";			break;
				case 4:	inner = "Staff of ";		break;
				case 5:	inner = "Book of ";			break;
			}

			if ( spell.Contains("Spell of ") ){ spell = spell.Replace("Spell of ", inner); }

			return spell;
		}

		public void NPCEvil( Mobile m, QuestTome book )
        {
			int c = 0;
			bool picked = false;

			while ( !picked )
			{
				ArrayList npcs = new ArrayList();
				foreach ( Mobile msg in World.Mobiles.Values )
				if ( msg is EpicCharacter && msg.Name != "the Great Earth Serpent" )
				{
					if ( ((EpicCharacter)msg).MyAlignment == "evil" )
					{
						npcs.Add( msg ); c++;
					}
				}

				int o = Utility.RandomMinMax( 0, c );

				for ( int i = 0; i < npcs.Count; ++i )
				{
					EpicCharacter dude = ( EpicCharacter )npcs[ i ];

					if ( i == o )
					{
						Point3D WhoLoc = new Point3D(dude.MyX, dude.MyY, 0);
						Map WhoMap = dude.MyWorld;

						DeliverMap = dude.MyWorld;
						DeliverX = WhoLoc.X;
						DeliverY = WhoLoc.Y;

						book.DeliverMap = dude.MyWorld;
						book.DeliverX = WhoLoc.X;
						book.DeliverY = WhoLoc.Y;

						string my_location = "";

						int xLong = 0, yLat = 0;
						int xMins = 0, yMins = 0;
						bool xEast = false, ySouth = false;

						if ( Sextant.Format( WhoLoc, WhoMap, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth ) )
						{
							my_location = String.Format( "{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W" );
						}

						book.QuestTomeWorldEvil = Server.Lands.GetLand( WhoMap, WhoLoc, dude.MyX, dude.MyY );
						book.QuestTomeNPCEvil = dude.Name + " " + dude.Title;
						book.QuestTomeLocateEvil = my_location;
						picked = true;
					}
				}
			}
        }

		public void NPCGood( Mobile m, QuestTome book )
        {
			int c = 0;
			bool picked = false;

			while ( !picked )
			{
				ArrayList npcs = new ArrayList();
				foreach ( Mobile msg in World.Mobiles.Values )
				if ( msg is EpicCharacter && msg.Name != "the Great Earth Serpent" )
				{
					if ( ((EpicCharacter)msg).MyAlignment == "good" )
					{
						npcs.Add( msg ); c++;
					}
				}

				int o = Utility.RandomMinMax( 0, c );

				for ( int i = 0; i < npcs.Count; ++i )
				{
					EpicCharacter dude = ( EpicCharacter )npcs[ i ];

					if ( i == o )
					{
						Point3D WhoLoc = new Point3D(dude.MyX, dude.MyY, 0);
						Map WhoMap = dude.MyWorld;

						DeliverMap = dude.MyWorld;
						DeliverX = WhoLoc.X;
						DeliverY = WhoLoc.Y;

						book.DeliverMap = dude.MyWorld;
						book.DeliverX = WhoLoc.X;
						book.DeliverY = WhoLoc.Y;

						string my_location = "";

						int xLong = 0, yLat = 0;
						int xMins = 0, yMins = 0;
						bool xEast = false, ySouth = false;

						if ( Sextant.Format( WhoLoc, WhoMap, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth ) )
						{
							my_location = String.Format( "{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W" );
						}

						book.QuestTomeWorldGood = Server.Lands.GetLand( WhoMap, WhoLoc, dude.MyX, dude.MyY );
						book.QuestTomeNPCGood = dude.Name + " " + dude.Title;
						book.QuestTomeLocateGood = my_location;
						picked = true;
					}
				}
			}
        }

		public static void MakeVillain( QuestTome book )
        {
			book.VillainHue = 0;
			book.VillainBody = 0;
			bool color = false;

			int enemy = Utility.RandomMinMax( 1, 10 );

			if ( enemy == 1 )
			{
				switch ( Utility.RandomMinMax( 0, 3 ) )
				{
					case 0:	book.VillainType = typeof(Archfiend).Name;			book.VillainName = NameList.RandomName( "daemon" );		book.VillainBody = Utility.RandomList( 9, 320 );	color = true;	book.VillainCategory = "a daemon";		break;
					case 1:	book.VillainType = typeof(Archfiend).Name;			book.VillainName = NameList.RandomName( "demonic" );	book.VillainBody = Utility.RandomList( 191, 427 );	color = true;
						switch ( Utility.RandomMinMax( 0, 2 ) )
						{
							case 0:	book.VillainCategory = "a balron"; break;
							case 1:	book.VillainCategory = "a balor";  break;
							case 2:	book.VillainCategory = "a balrog"; break;
						}
					break;
					case 2:	book.VillainType = typeof(Archfiend).Name;			book.VillainName = NameList.RandomName( "devil" );		book.VillainBody = Utility.RandomList( 765, 804, 436, 88, 138 );	book.VillainCategory = "a devil";		break;
					case 3:	book.VillainType = typeof(Succubus).Name;			book.VillainName = NameList.RandomName( "goddess" );	book.VillainBody = 174;	color = true;
						switch ( Utility.RandomMinMax( 0, 2 ) )
						{
							case 0:	book.VillainCategory = "a succubus"; break;
							case 1:	book.VillainCategory = "a demoness"; break;
							case 2:	book.VillainCategory = "a daemoness"; break;
						}
					break;
				}
			}
			else if ( enemy == 2 )
			{
				book.VillainCategory = "a giant";
				book.VillainName = NameList.RandomName( "giant" );

				switch ( Utility.RandomMinMax( 0, 18 ) )
				{
					case 0:		book.VillainType = typeof(AbyssGiant).Name;		break;
					case 1:		book.VillainType = typeof(CloudGiant).Name;		break;
					case 2:		book.VillainType = typeof(FireGiant).Name;			break;
					case 3:		book.VillainType = typeof(ForestGiant).Name;		break;
					case 4:		book.VillainType = typeof(FrostGiant).Name;		break;
					case 5:		book.VillainType = typeof(HillGiant).Name;			break;
					case 6:		book.VillainType = typeof(HillGiantShaman).Name;	break;
					case 7:		book.VillainType = typeof(IceGiant).Name;			break;
					case 8:		book.VillainType = typeof(JungleGiant).Name;		break;
					case 9:		book.VillainType = typeof(LavaGiant).Name;			break;
					case 10:	book.VillainType = typeof(MountainGiant).Name;		break;
					case 11:	book.VillainType = typeof(SandGiant).Name;			break;
					case 12:	book.VillainType = typeof(StarGiant).Name;			break;
					case 13:	book.VillainType = typeof(StoneGiant).Name;		break;
					case 14:	book.VillainType = typeof(StormGiant).Name;		break;
					case 15:	book.VillainType = typeof(AncientCyclops).Name;	book.VillainName = NameList.RandomName( "greek" );		book.VillainCategory = "a cyclops";	break;
					case 16:	book.VillainType = typeof(AncientEttin).Name;		book.VillainCategory = "an ettin";	break;
					case 17:	book.VillainType = typeof(OgreLord).Name;			book.VillainCategory = "an ogre";	break;
					case 18:	book.VillainType = typeof(Giant).Name;				break;
				}
			}
			else if ( enemy == 3 )
			{
				book.VillainCategory = "a dragon";
				book.VillainName = NameList.RandomName( "dragon" );
				switch ( Utility.RandomMinMax( 0, 24 ) )
				{
					case 0:		book.VillainType = typeof(Dragons).Name;					book.VillainBody = Utility.RandomList( 12, 59 );	color = true;	break;
					case 1:		book.VillainType = typeof(AncientWyrm).Name;				break;
					case 2:		book.VillainType = typeof(ShadowWyrm).Name;				break;
					case 3:		book.VillainType = typeof(VolcanicDragon).Name;			break;
					case 4:		book.VillainType = typeof(VoidDragon).Name;				break;
					case 5:		book.VillainType = typeof(AshDragon).Name;					break;
					case 6:		book.VillainType = typeof(CrystalDragon).Name;				break;
					case 7:		book.VillainType = typeof(ElderDragon).Name;				break;
					case 8:		book.VillainType = typeof(PrimevalAmberDragon).Name;		break;
					case 9:		book.VillainType = typeof(VolcanicDragon).Name;			break;
					case 10:	book.VillainType = typeof(PrimevalBlackDragon).Name;		break;
					case 11:	book.VillainType = typeof(PrimevalDragon).Name;			break;
					case 12:	book.VillainType = typeof(PrimevalFireDragon).Name;		break;
					case 13:	book.VillainType = typeof(PrimevalGreenDragon).Name;		break;
					case 14:	book.VillainType = typeof(PrimevalNightDragon).Name;		break;
					case 15:	book.VillainType = typeof(PrimevalRedDragon).Name;			break;
					case 16:	book.VillainType = typeof(PrimevalRoyalDragon).Name;		break;
					case 17:	book.VillainType = typeof(PrimevalRunicDragon).Name;		break;
					case 18:	book.VillainType = typeof(PrimevalSilverDragon).Name;		break;
					case 19:	book.VillainType = typeof(PrimevalStygianDragon).Name;		break;
					case 20:	book.VillainType = typeof(PrimevalVolcanicDragon).Name;	break;
					case 21:	book.VillainType = typeof(VampiricDragon).Name;			break;
					case 22:	book.VillainType = typeof(PrimevalAbysmalDragon).Name;		break;
					case 23:	book.VillainType = typeof(AncientDrake).Name;				book.VillainCategory = "a drake";	break;
					case 24:	book.VillainType = typeof(AncientWyvern).Name;				book.VillainCategory = "a wyvern";	break;
				}
			}
			else if ( enemy == 4 )
			{
				book.VillainCategory = "a beholder";
				Mobile m = new Beholder(); 
				book.VillainType = typeof(Beholder).Name;
				book.VillainName = m.Name;
				book.VillainHue = m.Hue;
				m.Delete();
			}
			else if ( enemy == 5 )
			{
				if ( Utility.RandomBool() )
				{
					book.VillainCategory = "a gargoyle";
					book.VillainType = typeof(StygianGargoyleLord).Name;
					book.VillainName = NameList.RandomName( "gargoyle name" );
				}
				else
				{
					book.VillainCategory = "a sphinx";
					book.VillainType = typeof(AncientSphinx).Name;
					book.VillainName = NameList.RandomName( "drakkul" );
				}
			}
			else if ( enemy == 6 )
			{
				if ( Utility.RandomBool() )
				{
					book.VillainCategory = "a reptilian humanoid";
					Mobile m = new Sleestax(); 
					book.VillainType = typeof(Sleestax).Name;
					book.VillainName = m.Name;
					book.VillainHue = m.Hue;
					m.Delete();
				}
				else
				{
					book.VillainCategory = "a serpentoid";
					book.VillainType = typeof(OphidianKnight).Name;
					book.VillainName = NameList.RandomName( "lizardman" );
					book.VillainBody = 306;
				}
			}
			else if ( enemy == 7 )
			{
				if ( Utility.RandomBool() )
				{
					book.VillainCategory = "an arachnid";
					book.VillainType = typeof(AbyssCrawler).Name;
					book.VillainName = NameList.RandomName( "goblin" );
					book.VillainBody = 173;
				}
				else
				{
					book.VillainCategory = "an insectoid";
					book.VillainType = typeof(AntaurKing).Name;
					book.VillainName = NameList.RandomName( "goblin" );
					book.VillainBody = 784;
				}
			}
			else if ( enemy == 8 )
			{
				if ( Utility.RandomBool() )
				{
					book.VillainCategory = "a reaper";
					Mobile m = new EvilEnt(); 
					book.VillainType = typeof(EvilEnt).Name;
					book.VillainName = m.Name;
					book.VillainHue = m.Hue;
					m.Delete();
				}
				else
				{
					book.VillainCategory = "an elemental";
					book.VillainType = typeof(CrystalGoliath).Name;
					book.VillainName = NameList.RandomName( "urk" );
					book.VillainBody = 753;
				}
			}
			else if ( enemy == 9 )
			{
				switch ( Utility.RandomMinMax( 0, 4 ) )
				{
					case 0:		book.VillainType = typeof(Dracolich).Name;					book.VillainName = NameList.RandomName( "dragon" );			book.VillainBody = Utility.RandomList( 104, 323 );		book.VillainCategory = "a dracolich";		break;
					case 1:		book.VillainType = typeof(AncientLich).Name;				book.VillainName = NameList.RandomName( "ancient lich" );	book.VillainCategory = "a lich";			break;
					case 2:		book.VillainType = typeof(AncientFleshGolem).Name;			book.VillainName = NameList.RandomName( "greek" );			book.VillainBody = 999;									book.VillainCategory = "a flesh golem";		break;
					case 3:		book.VillainType = typeof(GrundulVarg).Name;				book.VillainName = NameList.RandomName( "ancient lich" );	book.VillainBody = Utility.RandomList( 768, 65, 107 );	book.VillainCategory = "a dread lord";		break;
					case 4:		book.VillainType = typeof(GrundulVarg).Name;				book.VillainName = NameList.RandomName( "greek" );			book.VillainBody = Utility.RandomList( 768, 65, 107 );	book.VillainCategory = "a death knight";	break;
				}
			}
			else
			{
				switch ( Utility.RandomMinMax( 0, 3 ) )
				{
					case 0:		book.VillainType = typeof(Watcher).Name;					book.VillainName = NameList.RandomName( "drakkul" );		book.VillainCategory = "a watcher";			break;
					case 1:		book.VillainType = typeof(Cerberus).Name;					book.VillainName = NameList.RandomName( "greek" );			book.VillainCategory = "a cerberus";		break;
					case 2:		book.VillainType = typeof(Styguana).Name;					book.VillainName = NameList.RandomName( "lizardman" );		book.VillainCategory = "a styguana";		break;
					case 3:		book.VillainType = typeof(HellBeast).Name;					book.VillainName = NameList.RandomName( "imp" );			book.VillainCategory = "a hell beast";		break;
				}
			}

			book.VillainHue = 0; if ( color ){ book.VillainHue = Utility.RandomColor( 0 ); }

			book.VillainTitle = RandomThings.RandomEvilTitle();
		}

		public static void DropChest( Mobile m )
		{
			if ( m.Fame == 0 && m.Karma == 0 && m.Title != "" && ((BaseCreature)m).Home.X == 0 && ((BaseCreature)m).Home.Y == 0 )
			{
				bool BookExists = false;
				QuestTome book = null;

				foreach ( Item item in World.Items.Values )
				{
					if ( item is QuestTome )
					{
						if ( ((QuestTome)item).VillainName == m.Name && ((QuestTome)item).VillainTitle == m.Title )
						{
							BookExists = true;
							book = ((QuestTome)item);
						}
					}
				}

				if ( BookExists )
				{
					MajorItemOnCorpse majorChest = new MajorItemOnCorpse();
					majorChest.Name = "Chest of " + m.Name;
					majorChest.VillainName = m.Name;
					majorChest.VillainTitle = m.Title;
					majorChest.MoveToWorld( m.Location, m.Map );
					Server.Misc.IntelligentAction.BurnAway( m );
					book.QuestTomeCitizen = "";
					book.QuestTomeDungeon = "";
					book.QuestTomeLand = Land.None;
					book.QuestTomeType = 0;
					if ( m.Corpse != null ){ m.Corpse.Delete(); }
				}
			}
		}

		public QuestTake( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( ( int)1 ); // version
			writer.Write( DeliverMap );
			writer.Write( DeliverX );
			writer.Write( DeliverY );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					DeliverMap = reader.ReadMap();
					DeliverX = reader.ReadInt();
					DeliverY = reader.ReadInt();
					break;
				}
			}
		}
	}

	public class MajorItemOnCorpse : Item
	{
		public string VillainName;
		[CommandProperty( AccessLevel.GameMaster )]
		public string Villain_Name { get{ return VillainName; } set{ VillainName = value; } }

		public string VillainTitle;
		[CommandProperty( AccessLevel.GameMaster )]
		public string Villain_Title { get{ return VillainTitle; } set{ VillainTitle = value; } }

		[Constructable]
		public MajorItemOnCorpse() : base( 0x0E40 )
		{
			ItemRemovalTimer thisTimer = new ItemRemovalTimer( this ); 
			thisTimer.Start();
			ItemID = Utility.RandomList( 0x0E40, 0x0E41 );
			ResourceMods.SetRandomResource( false, false, this, CraftResource.Iron, false, null );
			Hue = CraftResources.GetHue(Resource);
			Name = "chest";
		}

		public override bool OnDragLift( Mobile from )
		{
			OnDoubleClick( from );
			return false;
		}

		public MajorItemOnCorpse( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( QuestTome.FoundItem( from, 1, this ) )
			{
				this.Delete();
			}
			else if ( from.InRange( this.GetWorldLocation(), 5 ) )
			{
				from.SendMessage( "The chest appears to be empty." );
			}
			else
			{
				from.SendLocalizedMessage( 502138 ); // That is too far away for you to use
			}
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
			this.Delete(); // none when the world starts 
		}

		public class ItemRemovalTimer : Timer 
		{ 
			private Item i_item; 
			public ItemRemovalTimer( Item item ) : base( TimeSpan.FromMinutes( 20.0 ) ) 
			{ 
				Priority = TimerPriority.OneSecond; 
				i_item = item; 
			} 

			protected override void OnTick() 
			{ 
				if (( i_item != null ) && ( !i_item.Deleted ))
				{
					i_item.Delete();
				}
			} 
		} 
	}
}