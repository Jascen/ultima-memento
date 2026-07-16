using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using System.Text;
using Server.Commands;
using Server.Commands.Generic;
using System.IO;
using Server.Mobiles;
using System.Threading;
using Server.Gumps;
using Server.Accounting;
using Server.Regions;
using System.Globalization;
using Server.Engines.MLQuests;
using System.Linq;

namespace Server.Mobiles
{
	public class Citizens : BaseCreature
	{
		public override bool PlayerRangeSensitive { get { return true; } }

		public int CitizenService;
		[CommandProperty(AccessLevel.Owner)]
		public int Citizen_Service { get { return CitizenService; } set { CitizenService = value; InvalidateProperties(); } }

		public int CitizenType;
		[CommandProperty(AccessLevel.Owner)]
		public int Citizen_Type { get { return CitizenType; } set { CitizenType = value; InvalidateProperties(); } }

		public int CitizenCost;
		[CommandProperty(AccessLevel.Owner)]
		public int Citizen_Cost { get { return CitizenCost; } set { CitizenCost = value; InvalidateProperties(); } }

		public string CitizenPhrase;
		[CommandProperty(AccessLevel.Owner)]
		public string Citizen_Phrase { get { return CitizenPhrase; } set { CitizenPhrase = value; InvalidateProperties(); } }

		public string CitizenRumor;
		[CommandProperty(AccessLevel.Owner)]
		public string Citizen_Rumor { get { return CitizenRumor; } set { CitizenRumor = value; InvalidateProperties(); } }

		public override bool InitialInnocent{ get{ return true; } }
		public override bool DeleteCorpseOnDeath{ get{ return true; } }

		public DateTime m_NextTalk;
		public DateTime NextTalk{ get{ return m_NextTalk; } set{ m_NextTalk = value; } }
		
		public bool ShouldRemoveSomeStuff{ get; set; }

		[Constructable]
		public Citizens() : base( AIType.AI_Citizen, FightMode.None, 10, 1, 0.2, 0.4 )
		{
			if ( Female = Utility.RandomBool() ) 
			{ 
				Body = 401; 
				Name = NameList.RandomName( "female" );
			}
			else 
			{ 
				Body = 400; 			
				Name = NameList.RandomName( "male" ); 
				FacialHairItemID = Utility.RandomList( 0, 0, 8254, 8255, 8256, 8257, 8267, 8268, 8269 );
			}

			SetStr( 200, 300 );
			SetDex( 200, 300 );
			SetInt( 200, 300 );

			switch ( Utility.Random( 3 ) )
			{
				case 0: 
					Server.Misc.IntelligentAction.DressUpWizards( this, false );
					CitizenType = 1;
					switch ( Utility.RandomMinMax( 0, 5 ) )
					{
						case 1: AddItem( new Server.Items.GnarledStaff() ); break;
						case 2: AddItem( new Server.Items.BlackStaff() ); break;
						case 3: AddItem( new Server.Items.WildStaff() ); break;
						case 4: AddItem( new Server.Items.QuarterStaff() ); break;
					}
				break;
				case 1: Server.Misc.IntelligentAction.DressUpFighters( this, "", false, false, true );	CitizenType = 2;	break;
				case 2: Server.Misc.IntelligentAction.DressUpRogues( this, "", false, false, true );	CitizenType = 3;	break;
			}

			CitizenCost = 0;
			CitizenService = 0;

			SetupCitizen();

			CantWalk = true;
			Title = TavernPatrons.GetTitle();
			Hue = Utility.RandomSkinColor();
			Utility.AssignRandomHair( this );
			SpeechHue = Utility.RandomTalkHue();
			NameHue = Utility.RandomOrangeHue();
			AI = AIType.AI_Citizen;
			FightMode = FightMode.None;

			SetHits( 300, 400 );

			SetDamage( 8, 10 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 25, 30 );
			SetResistance( ResistanceType.Cold, 25, 30 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.Searching, 60.0, 82.5 );
			SetSkill( SkillName.Anatomy, 60.0, 82.5 );
			SetSkill( SkillName.Poisoning, 60.0, 82.5 );
			SetSkill( SkillName.MagicResist, 60.0, 82.5 );
			SetSkill( SkillName.Tactics, 60.0, 82.5 );
			SetSkill( SkillName.FistFighting, 60.0, 82.5 );
			SetSkill( SkillName.Swords, 60.0, 82.5 );
			SetSkill( SkillName.Fencing, 60.0, 82.5 );
			SetSkill( SkillName.Bludgeoning, 60.0, 82.5 );

			Fame = 0;
			Karma = 0;
			VirtualArmor = 30;

			int HairColor = Utility.RandomHairHue();
			HairHue = HairColor;
			FacialHairHue = HairColor;

			if ( this is HouseVisitor && Backpack != null ){ Backpack.Delete(); }
		}

		public void SetupCitizen()
		{
			TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

			if ( Backpack != null ){ Backpack.Delete(); }
			Container pack = new Backpack();
			pack.Movable = false;
			AddItem( pack );

			if ( this.FindItemOnLayer( Layer.OneHanded ) != null )
			{
				Item myOneHand = this.FindItemOnLayer( Layer.OneHanded );
				ResourceMods.SetRandomResource( false, true, myOneHand, CraftResource.None, false, this );
			}

			if ( this.FindItemOnLayer( Layer.TwoHanded ) != null )
			{
				Item myTwoHand = this.FindItemOnLayer( Layer.TwoHanded );
				ResourceMods.SetRandomResource( false, true, myTwoHand, CraftResource.None, false, this );
			}

			string dungeon = QuestCharacters.SomePlace( "tavern" );
				if ( Utility.RandomMinMax( 1, 3 ) == 1 ){ dungeon = RandomThings.MadeUpDungeon(); }

			string Clues = QuestCharacters.SomePlace( "tavern" );
				if ( Utility.RandomMinMax( 1, 3 ) == 1 ){ Clues = RandomThings.MadeUpDungeon(); }

			string city = RandomThings.GetRandomCity();
				if ( Utility.RandomMinMax( 1, 3 ) == 1 ){ city = RandomThings.MadeUpCity(); }

			string adventurer = Server.Misc.TavernPatrons.Adventurer();

			int relic = Utility.RandomMinMax( 1, 59 );
			string item = Server.Items.SomeRandomNote.GetSpecialItem( relic, 1 );
				item = "the '" + cultInfo.ToTitleCase(item) + "'";

			string locale = Server.Items.SomeRandomNote.GetSpecialItem( relic, 0 );
				if ( Utility.RandomBool() ) // CITIZENS LIE HALF THE TIME
				{
					if ( Utility.RandomBool() ){ locale = RandomThings.MadeUpDungeon(); }
					else { locale = QuestCharacters.SomePlace( null ); }
				}

			if ( Utility.RandomMinMax( 1, 3 ) > 1 )
			{
				item = QuestCharacters.QuestItems( true );
				locale = dungeon;
			}

			string preface = "I found";

			int topic = Utility.RandomMinMax( 0, 40 );
				if ( this is HouseVisitor ){ topic = 100; }

			switch ( topic )
			{
				case 0:	CitizenRumor = "I heard that " + item + " can be obtained in " + locale + "."; break;
				case 1:	CitizenRumor = "I heard something about " + item + " and " + locale + "."; break;
				case 2:	CitizenRumor = "Someone told me that " + locale + " is where you would look for " + item + "."; break;
				case 3:	CitizenRumor = "I heard many tales of adventurers going to " + locale + " and seeing " + item + "."; break;
				case 4:	CitizenRumor = QuestCharacters.RandomWords() + " was in the tavern talking about " + item + " and " + locale + "."; break;
				case 5:	CitizenRumor = "I was talking with the local " + RandomThings.GetRandomJob() + ", and they mentioned " + item + " and " + locale + "."; break;
				case 6:	CitizenRumor = "I met with " + QuestCharacters.RandomWords() + " and they told me to bring back " + item + " from " + locale + "."; break;
				case 7:	CitizenRumor = "I heard that " + item + " can be found in " + locale + "."; break;
				case 8:	CitizenRumor = "Someone from " + RandomThings.GetRandomCity() + " died in " + locale + " searching for " + item + "."; break;
				case 9:	CitizenRumor = Server.Misc.TavernPatrons.GetRareLocation( this, true, false );		break;
			}

			switch( Utility.RandomMinMax( 0, 13 ) )
			{
				case 0: preface = "I found"; 											break;
				case 1: preface = "I heard rumours about"; 								break;
				case 2: preface = "I heard a story about"; 								break;
				case 3: preface = "I overheard someone tell of"; 						break;
				case 4: preface = "Some " + adventurer + " found"; 						break;
				case 5: preface = "Some " + adventurer + " heard rumours about"; 		break;
				case 6: preface = "Some " + adventurer + " heard a story about"; 		break;
				case 7: preface = "Some " + adventurer + " overheard another tell of"; 	break;
				case 8: preface = "Some " + adventurer + " is spreading rumors about"; 	break;
				case 9: preface = "Some " + adventurer + " is telling tales about"; 	break;
				case 10: preface = "We found"; 											break;
				case 11: preface = "We heard rumours about"; 							break;
				case 12: preface = "We heard a story about"; 							break;
				case 13: preface = "We overheard someone tell of"; 						break;
			}

			if ( CitizenRumor == null ){ CitizenRumor = preface + " " + Server.Misc.TavernPatrons.CommonTalk( "", city, dungeon, this, adventurer, true ) + "."; }

			if ( this is HouseVisitor )
			{
				CitizenService = 0;
			}
			else if ( CitizenType == 1 )
			{
				if ( Utility.RandomMinMax( 1, 10 ) == 1 ){ CitizenService = Utility.RandomMinMax( 1, 8 ); }
			}
			else if ( CitizenType == 4 ) // SMITH
			{
				CitizenService = 0;
				CitizenType = 0;
				switch ( Utility.RandomMinMax( 1, 50 ) )
				{
					case 1: CitizenService = 1;		CitizenType = 2;	break;
					case 2: CitizenService = 2;		CitizenType = 2;	break;
					case 3: CitizenService = 20;	CitizenType = 20;	break;
					case 4: CitizenService = 20;	CitizenType = 20;	break;
					case 5: CitizenService = 20;	CitizenType = 20;	break;
				}
			}
			else if ( CitizenType == 5 ) // LUMBERJACK
			{
				CitizenService = 0;
				CitizenType = 0;
				switch ( Utility.RandomMinMax( 1, 50 ) )
				{
					case 1: CitizenService = 3;		CitizenType = 2;	break;
					case 2: CitizenService = 4;		CitizenType = 2;	break;
					case 3: CitizenService = 21;	CitizenType = 21;	break;
					case 4: CitizenService = 21;	CitizenType = 21;	break;
					case 5: CitizenService = 21;	CitizenType = 21;	break;
				}
			}
			else if ( CitizenType == 6 ) // LEATHER
			{
				CitizenService = 0;
				CitizenType = 0;
				switch ( Utility.RandomMinMax( 1, 50 ) )
				{
					case 1: CitizenService = 2;		CitizenType = 22;	break;
					case 2: CitizenService = 2;		CitizenType = 22;	break;
					case 3: CitizenService = 22;	CitizenType = 22;	break;
					case 4: CitizenService = 22;	CitizenType = 22;	break;
					case 5: CitizenService = 22;	CitizenType = 22;	break;
				}
			}
			else if ( CitizenType == 7 ) // MINER
			{
				CitizenService = 0;
				CitizenType = 0;
				switch ( Utility.RandomMinMax( 1, 50 ) )
				{
					case 1: CitizenService = 1;		CitizenType = 2;	break;
					case 2: CitizenService = 2;		CitizenType = 2;	break;
					case 3: CitizenService = 23;	CitizenType = 23;	break;
					case 4: CitizenService = 23;	CitizenType = 23;	break;
					case 5: CitizenService = 23;	CitizenType = 23;	break;
				}
			}
			else if ( CitizenType == 8 ) // SMELTER
			{
				CitizenService = 0;
				CitizenType = 0;
				switch ( Utility.RandomMinMax( 1, 50 ) )
				{
					case 1: CitizenService = 1;		CitizenType = 2;	break;
					case 2: CitizenService = 2;		CitizenType = 2;	break;
					case 3: CitizenService = 20;	CitizenType = 20;	break;
					case 4: CitizenService = 20;	CitizenType = 20;	break;
					case 5: CitizenService = 23;	CitizenType = 23;	break;
					case 6: CitizenService = 23;	CitizenType = 23;	break;
				}
			}
			else if ( CitizenType == 9 ) // ALCHEMIST
			{
				CitizenService = 0;
				CitizenType = 0;
				switch ( Utility.RandomMinMax( 1, 50 ) )
				{
					case 1: CitizenService = 24;	CitizenType = 24;	break;
					case 2: CitizenService = 24;	CitizenType = 24;	break;
					case 3: CitizenService = 25;	CitizenType = 25;	break;
					case 4: CitizenService = 25;	CitizenType = 25;	break;
				}
			}
			else if ( CitizenType == 10 ) // COOK
			{
				CitizenService = 0;
				CitizenType = 0;
				switch ( Utility.RandomMinMax( 1, 50 ) )
				{
					case 1: CitizenService = 26;	CitizenType = 26;	break;
					case 2: CitizenService = 26;	CitizenType = 26;	break;
					case 3: CitizenService = 26;	CitizenType = 26;	break;
				}
			}
			else if ( CitizenType == 11 ) // BUTCHER
			{
				CitizenService = 0;
				CitizenType = 0;
				switch ( Utility.RandomMinMax( 1, 50 ) )
				{
					case 1: CitizenService = 27;	CitizenType = 27;	break;
					case 2: CitizenService = 27;	CitizenType = 27;	break;
					case 3: CitizenService = 27;	CitizenType = 27;	break;
				}
			}
			else if ( CitizenType == 12 ) // BARD
			{
				CitizenService = 0;
				CitizenType = 0;
			}
			else
			{
				switch ( Utility.RandomMinMax( 1, 50 ) )
				{
					case 1: CitizenService = 1;		break;
					case 2: CitizenService = 2;		break;
					case 3: CitizenService = 3;		break;
					case 4: CitizenService = 4;		break;
					case 5: CitizenService = 5;		break;
				}
			}

			string phrase = "";

			int initPhrase = Utility.RandomMinMax( 0, 6 );
				if ( this is TavernPatronNorth || this is TavernPatronSouth || this is TavernPatronEast || this is TavernPatronWest ){ initPhrase = Utility.RandomMinMax( 0, 4 ); }

			switch ( initPhrase )
			{
				case 0:	phrase = "Greetings, Z~Z~Z~Z~Z."; break;
				case 1:	phrase = "Hail, Z~Z~Z~Z~Z."; break;
				case 2:	phrase = "Good day to you, Z~Z~Z~Z~Z."; break;
				case 3:	phrase = "Hello, Z~Z~Z~Z~Z."; break;
				case 4:	phrase = "We are just here to rest after exploring " + dungeon + "."; break;
				case 5:	phrase = "This is the first time I have been to Y~Y~Y~Y~Y."; break;
				case 6:	phrase = "Hail, Z~Z~Z~Z~Z. Welcome to Y~Y~Y~Y~Y."; break;
			}

			if ( CitizenService == 1 )
			{
				if ( CitizenType == 1 ){ CitizenPhrase = phrase + " I can recharge any magic items you may have. Such items have a magical spell imbued in it, and allows you to cast such spell. I can only recharge such items if they have a minimum and maximum amount of uses, as those without a maximum amount can never be recharged by any wizard. If you want my help, then simply hand me your wand so I can perform the ritual needed."; }
				else if ( CitizenType == 2 ){ CitizenPhrase = phrase + " I am quite a skilled blacksmith, so if you need any metal armor repaired I can do it for you. Just hand me the armor and I will see what I can do."; }
				else { CitizenPhrase = phrase + " If you need a chest or box unlocked, I can help you with that. Just hand me the container and I will see what I can do. I promise to give it back."; }
			}
			else if ( CitizenService == 2 )
			{
				if ( CitizenType == 2 ){ CitizenPhrase = phrase + " I am quite a skilled blacksmith, so if you need any metal weapons repaired I can do it for you. Just hand me the weapon and I will see what I can do."; }
				else { CitizenPhrase = phrase + " I am quite a skilled leather worker, so if you need any leather item repaired I can do it for you. Just hand me the item and I will see what I can do."; }
			}
			else if ( CitizenService == 3 )
			{
				if ( CitizenType == 2 ){ CitizenPhrase = phrase + " I am quite a skilled wood worker, so if you need any wooden weapons repaired I can do it for you. Just hand me the weapon and I will see what I can do."; }
				else { CitizenPhrase = phrase + " I am quite a skilled wood worker, so if you need any wooden weapons repaired I can do it for you. Just hand me the weapon and I will see what I can do."; }
			}
			else if ( CitizenService == 4 )
			{
				if ( CitizenType == 2 ){ CitizenPhrase = phrase + " I am quite a skilled wood worker, so if you need any wooden armor repaired I can do it for you. Just hand me the armor and I will see what I can do."; }
				else { CitizenPhrase = phrase + " I am quite a skilled wood worker, so if you need any wooden armor repaired I can do it for you. Just hand me the armor and I will see what I can do."; }
			}
			else if ( CitizenService == 5 )
			{
				string aty1 = "a magic item"; if (Utility.RandomBool() ){ aty1 = "an enchanted item"; } else if (Utility.RandomBool() ){ aty1 = "a special item"; }
				string aty2 = "found"; if (Utility.RandomBool() ){ aty2 = "discovered"; }
				string aty3 = "willing to part with"; if (Utility.RandomBool() ){ aty3 = "willing to trade"; } else if (Utility.RandomBool() ){ aty3 = "willing to sell"; }

				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0:	CitizenPhrase = phrase + " I have " + aty1 + " I " + aty2 + " while exploring " + Clues + " that I am " + aty3 + " for G~G~G~G~G gold."; break;
					case 1:	CitizenPhrase = phrase + " I won " + aty1 + " from a card game in " + city + " that I am " + aty3 + " for G~G~G~G~G gold."; break;
					case 2:	CitizenPhrase = phrase + " I have " + aty1 + " I " + aty2 + " on the remains of some " + adventurer + " that I am " + aty3 + " for G~G~G~G~G gold."; break;
					case 3:	CitizenPhrase = phrase + " I have " + aty1 + " I " + aty2 + " from a chest in " + Clues + " that I am " + aty3 + " for G~G~G~G~G gold."; break;
					case 4:	CitizenPhrase = phrase + " I have " + aty1 + " I " + aty2 + " on a beast I killed in " + Clues + " that I am " + aty3 + " for G~G~G~G~G gold."; break;
					case 5:	CitizenPhrase = phrase + " I have " + aty1 + " I " + aty2 + " on some " + adventurer + " in " + Clues + " that I am " + aty3 + " for G~G~G~G~G gold."; break;
				}
				CitizenPhrase = CitizenPhrase + " You can look in my backpack to examine the item if you wish. If you want to trade, then hand me the gold and I will give you the item.";
			}
			else if ( CitizenType == 20 && CitizenService == 20 )
			{
				dungeon = RandomThings.MadeUpDungeon();
				city = RandomThings.MadeUpCity();

				Item crate = new CrateOfMetal();
				ResourceMods.SetRandomResource( false, true, crate, CraftResource.Iron, false, this );
				((CrateOfMetal)crate).Fill();
				CitizenCost = (int)(crate.Limits * ( 2 * CraftResources.GetGold( crate.Resource ) ));

				string dug = "smelted";
				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0:	dug = "mined"; break;
					case 1:	dug = "smelted"; break;
					case 2:	dug = "forged"; break;
					case 3:	dug = "dug up"; break;
					case 4:	dug = "excavated"; break;
					case 5:	dug = "formed"; break;
				}

				string sell = "willing to part with"; if (Utility.RandomBool() ){ sell = "willing to trade"; } else if (Utility.RandomBool() ){ sell = "willing to sell"; }
				string cave = "cave"; if (Utility.RandomBool() ){ cave = "mine"; }

				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + dug + " in a " + cave + " near " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 1:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + dug + " in a " + cave + " outside of " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 2:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + dug + " in a " + cave + " by " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 3:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + dug + " in a " + cave + " near " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 4:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + dug + " in a " + cave + " by " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 5:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + dug + " in a " + cave + " outside of " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
				}
				CitizenPhrase = CitizenPhrase + " You can look in my backpack to examine the ingots if you wish. If you want to trade, then hand me the gold and I will give you the ingots.";

				PackItem( crate );
			}
			else if ( CitizenType == 21 && CitizenService == 21 )
			{
				bool isLogs = Utility.RandomBool();
				Item crate = null;

				string contents = "boards";
					if ( isLogs ){ contents = "logs"; }

				dungeon = RandomThings.MadeUpDungeon();
				city = RandomThings.MadeUpCity();

				if ( isLogs )
					crate = new CrateOfLogs();
				else
					crate = new CrateOfWood();

				ResourceMods.SetRandomResource( false, true, crate, CraftResource.RegularWood, false, this );
				CitizenCost = (int)(crate.Limits * ( 2 * CraftResources.GetGold( crate.Resource ) ));

				if ( isLogs )
					((CrateOfLogs)crate).Fill();
				else
					((CrateOfWood)crate).Fill();
				
				string chop = "chopped";
				switch ( Utility.RandomMinMax( 0, 2 ) )
				{
					case 0:	chop = "chopped"; break;
					case 1:	chop = "cut"; break;
					case 2:	chop = "logged"; break;
				}

				string sell = "willing to part with"; if (Utility.RandomBool() ){ sell = "willing to trade"; } else if (Utility.RandomBool() ){ sell = "willing to sell"; }
				string forest = "woods"; if (Utility.RandomBool() ){ forest = "forest"; }

				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + chop + " in the " + forest + " near " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 1:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + chop + " in the " + forest + " outside of " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 2:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + chop + " in the " + forest + " by " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 3:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + chop + " in the " + forest + " near " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 4:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + chop + " in the " + forest + " by " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 5:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + chop + " in the " + forest + " outside of " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
				}
				CitizenPhrase = CitizenPhrase + " You can look in my backpack to examine the " + contents + " if you wish. If you want to trade, then hand me the gold and I will give you the " + contents + ".";

				PackItem( crate );
			}
			else if ( CitizenType == 22 && CitizenService == 22 )
			{
				dungeon = RandomThings.MadeUpDungeon();
				city = RandomThings.MadeUpCity();

				CrateOfLeather crate = new CrateOfLeather();
				ResourceMods.SetRandomResource( false, true, crate, CraftResource.RegularLeather, false, this );
				((CrateOfLeather)crate).Fill();
				CitizenCost = (int)(crate.Limits * ( 2 * CraftResources.GetGold( crate.Resource ) ));

				string carve = "skinned";
				switch ( Utility.RandomMinMax( 0, 2 ) )
				{
					case 0:	carve = "skinned"; break;
					case 1:	carve = "tanned"; break;
					case 2:	carve = "gathered"; break;
				}

				string sell = "willing to part with"; if (Utility.RandomBool() ){ sell = "willing to trade"; } else if (Utility.RandomBool() ){ sell = "willing to sell"; }

				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + carve + " near " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 1:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + carve + " outside of " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 2:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + carve + " by " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 3:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + carve + " near " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 4:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + carve + " by " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 5:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + carve + " outside of " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
				}
				CitizenPhrase = CitizenPhrase + " You can look in my backpack to examine the leather if you wish. If you want to trade, then hand me the gold and I will give you the leather.";

				PackItem( crate );
			}
			else if ( CitizenType == 23 && CitizenService == 23 )
			{
				dungeon = RandomThings.MadeUpDungeon();
				city = RandomThings.MadeUpCity();

				Item crate = new CrateOfOre();
				ResourceMods.SetRandomResource( false, true, crate, CraftResource.Iron, false, this );
				((CrateOfOre)crate).Fill();
				CitizenCost = (int)(crate.Limits * ( 2 * CraftResources.GetGold( crate.Resource ) ));

				string dug = "mined";
				switch ( Utility.RandomMinMax( 0, 2 ) )
				{
					case 0:	dug = "mined"; break;
					case 1:	dug = "dug up"; break;
					case 2:	dug = "excavated"; break;
				}

				string sell = "willing to part with"; if (Utility.RandomBool() ){ sell = "willing to trade"; } else if (Utility.RandomBool() ){ sell = "willing to sell"; }
				string cave = "cave"; if (Utility.RandomBool() ){ cave = "mine"; }

				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + dug + " in a " + cave + " near " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 1:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + dug + " in a " + cave + " outside of " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 2:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + dug + " in a " + cave + " by " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 3:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + dug + " in a " + cave + " near " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 4:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + dug + " in a " + cave + " by " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 5:	CitizenPhrase = phrase + " I have a " + crate.Name + " with " + crate.Limits + " in it, that I " + dug + " in a " + cave + " outside of " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
				}
				CitizenPhrase = CitizenPhrase + " You can look in my backpack to examine the ore if you wish. If you want to trade, then hand me the gold and I will give you the ore.";

				PackItem( crate );
			}
			else if ( CitizenType == 24 && CitizenService == 24 )
			{
				dungeon = RandomThings.MadeUpDungeon();
				city = RandomThings.MadeUpCity();

				CrateOfReagents crate = new CrateOfReagents();

				string reagent = "bloodmoss";
				int bottle = 0x508E;

				switch ( Utility.RandomMinMax( 0, 33 ) )
				{
					case 0:		bottle = 0x508E; reagent = "bloodmoss"; break;
					case 1:		bottle = 0x508F; reagent = "black pearl"; break;
					case 2:		bottle = 0x5098; reagent = "garlic"; break;
					case 3:		bottle = 0x5099; reagent = "ginseng"; break;
					case 4:		bottle = 0x509A; reagent = "mandrake root"; break;
					case 5:		bottle = 0x509B; reagent = "nightshade"; break;
					case 6:		bottle = 0x509C; reagent = "sulfurous ash"; break;
					case 7:		bottle = 0x509D; reagent = "spider silk"; break;
					case 8:		bottle = 0x568A; reagent = "swamp berry"; break;
					case 9:		bottle = 0x55E0; reagent = "bat wing"; break;
					case 10:	bottle = 0x55E1; reagent = "beetle shell"; break;
					case 11:	bottle = 0x55E2; reagent = "brimstone"; break;
					case 12:	bottle = 0x55E3; reagent = "butterfly"; break;
					case 13:	bottle = 0x55E4; reagent = "daemon blood"; break;
					case 14:	bottle = 0x55E5; reagent = "toad eyes"; break;
					case 15:	bottle = 0x55E6; reagent = "fairy eggs"; break;
					case 16:	bottle = 0x55E7; reagent = "gargoyle ears"; break;
					case 17:	bottle = 0x55E8; reagent = "grave dust"; break;
					case 18:	bottle = 0x55E9; reagent = "moon crystals"; break;
					case 19:	bottle = 0x55EA; reagent = "nox crystal"; break;
					case 20:	bottle = 0x55EB; reagent = "silver widow"; break;
					case 21:	bottle = 0x55EC; reagent = "pig iron"; break;
					case 22:	bottle = 0x55ED; reagent = "pixie skull"; break;
					case 23:	bottle = 0x55EE; reagent = "red lotus"; break;
					case 24:	bottle = 0x55EF; reagent = "sea salt"; break;
					case 25:	bottle = 0x6415; reagent = "bitter roots"; break;
					case 26:	bottle = 0x6416; reagent = "black sand"; break;
					case 27:	bottle = 0x6417; reagent = "blood roses"; break;
					case 28:	bottle = 0x6418; reagent = "dried toads"; break;
					case 29:	bottle = 0x6419; reagent = "maggots"; break;
					case 30:	bottle = 0x641A; reagent = "mummy wraps"; break;
					case 31:	bottle = 0x641B; reagent = "violet fungus"; break;
					case 32:	bottle = 0x641C; reagent = "werewolf claws"; break;
					case 33:	bottle = 0x641D; reagent = "wolfsbane"; break;
				}

				crate.CrateQty = Utility.RandomMinMax( 400, 1200 );
				crate.CrateItem = reagent;
				crate.ItemID = bottle;
				crate.Name = "crate of " + reagent + "";
				crate.Weight = crate.CrateQty * 0.1;
				CitizenCost = crate.CrateQty * 5;

				string bought = "bought";
				switch ( Utility.RandomMinMax( 0, 2 ) )
				{
					case 0:	bought = "acquired"; break;
					case 1:	bought = "purchased"; break;
					case 2:	bought = "bought"; break;
				}
				string found = "found";
				switch ( Utility.RandomMinMax( 0, 2 ) )
				{
					case 0:	found = "found"; break;
					case 1:	found = "discovered"; break;
					case 2:	found = "came upon"; break;
				}

				string sell = "willing to part with"; if (Utility.RandomBool() ){ sell = "willing to trade"; } else if (Utility.RandomBool() ){ sell = "willing to sell"; }

				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + reagent + " I " + found + " in " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 1:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + reagent + " I " + found + " deep within " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 2:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + reagent + " I " + found + " somewhere in " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 3:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + reagent + " I " + bought + " in " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 4:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + reagent + " I " + bought + " near " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 5:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + reagent + " I " + bought + " somewhere in " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
				}
				CitizenPhrase = CitizenPhrase + " You can look in my backpack to examine the reagents if you wish. If you want to trade, then hand me the gold and I will give you the reagents.";

				PackItem( crate );
			}
			else if ( CitizenType == 25 && CitizenService == 25 )
			{
				dungeon = RandomThings.MadeUpDungeon();
				city = RandomThings.MadeUpCity();

				CrateOfPotions crate = new CrateOfPotions();

				string potion = "crate of nightsight potions";
				int jug = 1109;
				int coins = 15;

				switch ( Utility.RandomMinMax( 0, 36 ) )
				{
					case 0: coins = 15; potion = "nightsight potions"; jug = 1109; break;
					case 1: coins = 15; potion = "lesser cure potions"; jug = 45; break;
					case 2: coins = 30; potion = "cure potions"; jug = 45; break;
					case 3: coins = 60; potion = "greater cure potions"; jug = 45; break;
					case 4: coins = 15; potion = "agility potions"; jug = 396; break;
					case 5: coins = 60; potion = "greater agility potions"; jug = 396; break;
					case 6: coins = 15; potion = "strength potions"; jug = 1001; break;
					case 7: coins = 60; potion = "greater strength potions"; jug = 1001; break;
					case 8: coins = 15; potion = "lesser poison potions"; jug = 73; break;
					case 9: coins = 30; potion = "poison potions"; jug = 73; break;
					case 10: coins = 60; potion = "greater poison potions"; jug = 73; break;
					case 11: coins = 90; potion = "deadly poison potions"; jug = 73; break;
					case 12: coins = 120; potion = "lethal poison potions"; jug = 73; break;
					case 13: coins = 15; potion = "refresh potions"; jug = 140; break;
					case 14: coins = 30; potion = "total refresh potions"; jug = 140; break;
					case 15: coins = 15; potion = "lesser heal potions"; jug = 50; break;
					case 16: coins = 30; potion = "heal potions"; jug = 50; break;
					case 17: coins = 60; potion = "greater heal potions"; jug = 50; break;
					case 18: coins = 15; potion = "lesser explosion potions"; jug = 425; break;
					case 19: coins = 30; potion = "explosion potions"; jug = 425; break;
					case 20: coins = 60; potion = "greater explosion potions"; jug = 425; break;
					case 21: coins = 15; potion = "lesser invisibility potions"; jug = 0x490; break;
					case 22: coins = 30; potion = "invisibility potions"; jug = 0x490; break;
					case 23: coins = 60; potion = "greater invisibility potions"; jug = 0x490; break;
					case 24: coins = 15; potion = "lesser rejuvenate potions"; jug = 0x48E; break;
					case 25: coins = 30; potion = "rejuvenate potions"; jug = 0x48E; break;
					case 26: coins = 60; potion = "greater rejuvenate potions"; jug = 0x48E; break;
					case 27: coins = 15; potion = "lesser mana potions"; jug = 0x48D; break;
					case 28: coins = 30; potion = "mana potions"; jug = 0x48D; break;
					case 29: coins = 60; potion = "greater mana potions"; jug = 0x48D; break;
					case 30: coins = 30; potion = "conflagration potions"; jug = 0xAD8; break;
					case 31: coins = 60; potion = "greater conflagration potions"; jug = 0xAD8; break;
					case 32: coins = 30; potion = "confusion blast potions"; jug = 0x495; break;
					case 33: coins = 60; potion = "greater confusion blast potions"; jug = 0x495; break;
					case 34: coins = 30; potion = "frostbite potions"; jug = 0xAF3; break;
					case 35: coins = 60; potion = "greater frostbite potions"; jug = 0xAF3; break;
					case 36: coins = 60; potion = "acid bottles"; jug = 1167; break;
				}

				crate.CrateQty = Utility.RandomMinMax( 30, 100 );
				crate.CrateItem = potion;
				crate.ItemID = 0x55DF;
				crate.Hue = jug;
				crate.Name = "crate of " + potion + "";
				crate.Weight = crate.CrateQty * 0.1;
				CitizenCost = crate.CrateQty * coins;

				string bought = "bought";
				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0:	bought = "acquired"; break;
					case 1:	bought = "purchased"; break;
					case 2:	bought = "bought"; break;
					case 3:	bought = "brewed"; break;
					case 4:	bought = "concocted"; break;
					case 5:	bought = "prepared"; break;
				}
				string found = "found";
				switch ( Utility.RandomMinMax( 0, 2 ) )
				{
					case 0:	found = "found"; break;
					case 1:	found = "discovered"; break;
					case 2:	found = "came upon"; break;
				}

				string sell = "willing to part with"; if (Utility.RandomBool() ){ sell = "willing to trade"; } else if (Utility.RandomBool() ){ sell = "willing to sell"; }

				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + potion + " I " + found + " in " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 1:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + potion + " I " + found + " deep within " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 2:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + potion + " I " + found + " somewhere in " + dungeon + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 3:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + potion + " I " + bought + " in " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 4:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + potion + " I " + bought + " near " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 5:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + potion + " I " + bought + " somewhere in " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
				}
				CitizenPhrase = CitizenPhrase + " You can look in my backpack to examine the potions if you wish. If you want to trade, then hand me the gold and I will give you the potions.";

				PackItem( crate );
			}

			if ( CitizenType == 1 && CitizenService == 2 ){ PackItem( new reagents_magic_jar1() ); CitizenCost = Utility.RandomMinMax( 70, 150 )*10; }
			else if ( CitizenType == 1 && CitizenService == 3 ){ PackItem( new reagents_magic_jar2() ); CitizenCost = Utility.RandomMinMax( 50, 90 )*10; }
			else if ( CitizenType == 1 && CitizenService == 4 ){ PackItem( new reagents_magic_jar3() ); CitizenCost = Utility.RandomMinMax( 180, 300 )*10; }
			else if ( CitizenType == 1 && CitizenService == 6 )
			{
				if ( Utility.RandomBool() )
				{
					int spellbook = Utility.RandomMinMax(1,5);
					if ( spellbook > 2 )
					{
						Spellbook tome = new Spellbook();
						CitizenCost = ItemInformation.GetBuysPrice( ItemInformation.ItemTableRef( tome ), false, tome, false, true );
						PackItem( tome ); 
					}
					else if ( spellbook == 2 )
					{
						Spellbook tome = new NecromancerSpellbook();
						CitizenCost = ItemInformation.GetBuysPrice( ItemInformation.ItemTableRef( tome ), false, tome, false, true );
						PackItem( tome ); 
					}
					else
					{
						Spellbook tome = new NecromancerSpellbook();
						CitizenCost = ItemInformation.GetBuysPrice( ItemInformation.ItemTableRef( tome ), false, tome, false, true );
						PackItem( tome ); 
					}
				}
				else
				{
					Item book = new Runebook(0);
					PackItem( book );
					CitizenCost = ItemInformation.GetBuysPrice( ItemInformation.ItemTableRef( book ), false, book, false, true );
				}
			}
			else if ( CitizenType == 1 && CitizenService == 7 )
			{
				Item scroll = Loot.RandomScroll(Utility.Random(12)+1);
				PackItem( scroll );
				CitizenCost = ItemInformation.GetBuysPrice( ItemInformation.ItemTableRef( scroll ), false, scroll, false, true );
			}
			else if ( CitizenType == 1 && CitizenService == 8 )
			{
				Item wand = new MagicalWand(0);
				PackItem( wand );
				CitizenCost = ItemInformation.GetBuysPrice( ItemInformation.ItemTableRef( wand ), false, wand, false, true );
			}
			else if ( CitizenService == 5 )
			{
				int chance = Utility.RandomMinMax( 1, 100 );
				if ( chance < 80 )
				{
					Item arty = Loot.RandomMagicalItem();
					LootPackEntry.MakeFixedDrop( arty, 6 );
					ResourceMods.SetRandomResource( false, true, arty, CraftResource.None, false, this );
					arty.Movable = false;
					arty.Name = RandomThings.MagicItemName( arty, this, Region.Find( this.Location, this.Map ) );
					arty.Name = cultInfo.ToTitleCase(arty.Name);
					PackItem( arty );
					CitizenCost = ItemInformation.GetBuysPrice( ItemInformation.ItemTableRef( arty ), false, arty, false, true );

					if ( CitizenCost < 1 )
						arty.Delete();
				}
				else if ( chance < 95 )
				{
					Item arty = Loot.RandomInstrument();
					ResourceMods.SetRandomResource( false, true, arty, CraftResource.None, false, this );
					BaseInstrument instr = (BaseInstrument)arty;

					if ( Utility.RandomMinMax( 1, 4 ) == 1 ){ arty.Hue = Utility.RandomColor(0); }
					instr.Quality = InstrumentQuality.Regular;
					if ( Utility.RandomMinMax( 1, 4 ) == 1 ){ instr.Quality = InstrumentQuality.Exceptional; }
					if ( Utility.RandomMinMax( 1, 4 ) == 1 ){ instr.Slayer = SlayerDeed.GetRandomSlayer(); }

					LootPackEntry.MakeFixedDrop( arty, 6 );
					arty.Movable = false;
					arty.Name = RandomThings.MagicItemName( arty, this, Region.Find( this.Location, this.Map ) );
					arty.Name = cultInfo.ToTitleCase(arty.Name);
					PackItem( arty );
					CitizenCost = ItemInformation.GetBuysPrice( ItemInformation.ItemTableRef( arty ), false, arty, false, true );

					if ( CitizenCost < 1 )
						arty.Delete();
				}
				else
				{
					Item arty = Loot.RandomArty();
					arty.Movable = false;
					PackItem( arty );

					CitizenCost = Utility.RandomMinMax(40000, 60000);
					// CitizenCost = ItemInformation.GetBuysPrice( ItemInformation.ItemTableRef( arty ), false, arty, false, true );

					// if ( CitizenCost < 1 )
					// 	CitizenCost = Utility.RandomMinMax( 250, 750 )*10;
				}
			}
			else if ( CitizenType == 26 && CitizenService == 26 )
			{
				city = RandomThings.MadeUpCity();

				CrateOfFood crate = new CrateOfFood();

				string food = "meat";
				int eat = 0x508C;
				int cost = 0;

				switch ( Utility.RandomMinMax( 0, 3 ) )
				{
					case 0:	cost = 6;	eat = 0x508B; food = "cooked fish steaks"; break;
					case 1:	cost = 8;	eat = 0x508C; food = "cooked lamb legs"; break;
					case 2:	cost = 7;	eat = 0x508D; food = "cooked ribs"; break;
					case 3:	cost = 6;	eat = 0x50BA; food = "baked bread"; break;
				}

				crate.CrateQty = Utility.RandomMinMax( 50, 150 );
				crate.CrateItem = food;
				crate.ItemID = eat;
				crate.Name = "crate of " + food + "";
				crate.Weight = crate.CrateQty * 0.1;
				CitizenCost = crate.CrateQty * cost;

				string bought = "bought";
				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0:	bought = "acquired"; break;
					case 1:	bought = "purchased"; break;
					case 2:	bought = "bought"; break;
					case 3:	bought = "cooked"; break;
					case 4:	bought = "baked"; break;
					case 5:	bought = "prepared"; break;
				}

				string sell = "willing to part with"; if (Utility.RandomBool() ){ sell = "willing to trade"; } else if (Utility.RandomBool() ){ sell = "willing to sell"; }

				switch ( Utility.RandomMinMax( 0, 2 ) )
				{
					case 0:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + food + " I " + bought + " in " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 1:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + food + " I " + bought + " near " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 2:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + food + " I " + bought + " somewhere in " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
				}
				CitizenPhrase = CitizenPhrase + " You can look in my backpack to examine the " + food + " if you wish. If you want to trade, then hand me the gold and I will give you the " + food + ".";

				PackItem( crate );
			}
			else if ( CitizenType == 27 && CitizenService == 27 )
			{
				city = RandomThings.MadeUpCity();

				CrateOfMeats crate = new CrateOfMeats();

				string food = "meat";
				int eat = 0x508C;
				int cost = 0;

				switch ( Utility.RandomMinMax( 0, 2 ) )
				{
					case 0:	cost = 6;	eat = 0x508B; food = "raw fish"; break;
					case 1:	cost = 8;	eat = 0x508C; food = "raw lamb"; break;
					case 2:	cost = 7;	eat = 0x508D; food = "raw ribs"; break;
				}

				crate.CrateQty = Utility.RandomMinMax( 50, 150 );
				crate.CrateItem = food;
				crate.ItemID = eat;
				crate.Name = "crate of " + food + "";
				crate.Weight = crate.CrateQty * 0.1;
				CitizenCost = crate.CrateQty * cost;

				string bought = "bought";
				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0:	bought = "acquired"; break;
					case 1:	bought = "purchased"; break;
					case 2:	bought = "bought"; break;
					case 3:	bought = "cooked"; break;
					case 4:	bought = "baked"; break;
					case 5:	bought = "prepared"; break;
				}

				string sell = "willing to part with"; if (Utility.RandomBool() ){ sell = "willing to trade"; } else if (Utility.RandomBool() ){ sell = "willing to sell"; }

				switch ( Utility.RandomMinMax( 0, 2 ) )
				{
					case 0:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + food + " I " + bought + " in " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 1:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + food + " I " + bought + " near " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
					case 2:	CitizenPhrase = phrase + " I have " + crate.CrateQty + " " + food + " I " + bought + " somewhere in " + city + " that I am " + sell + " for G~G~G~G~G gold."; break;
				}
				CitizenPhrase = CitizenPhrase + " You can look in my backpack to examine the " + food + " if you wish. If you want to trade, then hand me the gold and I will give you the " + food + ".";

				PackItem( crate );
			}

			if ( CitizenType == 1 && ( CitizenService == 2 || CitizenService == 3 || CitizenService == 4 || CitizenService == 6 || CitizenService == 7 || CitizenService == 8 ) )
			{
				string aty1 = "a jar of wizard reagents";
					if ( CitizenService == 3 ){ aty1 = "a jar of necromancer reagents"; }
					else if ( CitizenService == 4 ){ aty1 = "a jar of alchemical reagents"; }
					else if ( CitizenService == 6 ){ aty1 = "a book"; }
					else if ( CitizenService == 7 ){ aty1 = "a scroll"; }
					else if ( CitizenService == 8 ){ aty1 = "a wand"; }

				string aty3 = "willing to part with"; if (Utility.RandomBool() ){ aty3 = "willing to trade"; } else if (Utility.RandomBool() ){ aty3 = "willing to sell"; }

				CitizenPhrase = phrase + " I have " + aty1 + " that I am " + aty3 + " for G~G~G~G~G gold.";
				CitizenPhrase = CitizenPhrase + " You can look in my backpack to examine the item if you wish. If you want to trade, then hand me the gold and I will give you the item.";
			}

			string holding = "";
			List<Item> belongings = new List<Item>();
			foreach( Item i in this.Backpack.Items )
			{
				i.Movable = false;
				holding = i.Name;
				if ( i.Name != null && i.Name != "" ){} else { i.SyncName(); holding = i.Name; }
				if ( ResourceMods.SearchResource(i) != CraftResource.None ){ holding = CraftResources.GetPrefix( ResourceMods.SearchResource(i) ) + i.Name; }
				holding = cultInfo.ToTitleCase(holding);
			}

			if ( holding != "" ){ CitizenPhrase = CitizenPhrase + "<br><br>" + holding; } 
			else if ( CitizenService == 5 ){ CitizenPhrase = null; }
			else if ( ( CitizenService >= 2 && CitizenService <= 8 ) && CitizenType == 1 ){ CitizenPhrase = null; }
		}

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( !(this is HouseVisitor || ( this is Humanoid && this.Body != 593 && this.Body != 597 && this.Body != 598 )) )
			{
				if ( DateTime.Now >= m_NextTalk && InRange( m, 30 ) )
				{
					if ( Utility.RandomBool() ){ TavernPatrons.GetChatter( this ); }
					m_NextTalk = (DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( 15, 45 ) ));
				}
			}
			else if ( this is Humanoid )
			{
				if ( DateTime.Now >= m_NextTalk && InRange( m, 30 ) )
				{
					if ( Body.IsHuman && !Mounted )
					{
						switch ( Utility.Random( 2 ) )
						{
							case 0: Animate( 5, 5, 1, true,  true, 1 ); break;
							case 1: Animate( 6, 5, 1, true, false, 1 ); break;
						}
					}
					else if ( Body.IsAnimal )
					{
						switch ( Utility.Random( 3 ) )
						{
							case 0: Animate(  3, 3, 1, true, false, 1 ); break;
							case 1: Animate(  9, 5, 1, true, false, 1 ); break;
							case 2: Animate( 10, 5, 1, true, false, 1 ); break;
						}
					}
					else if ( Body.IsMonster )
					{
						switch ( Utility.Random( 2 ) )
						{
							case 0: Animate( 17, 5, 1, true, false, 1 ); break;
							case 1: Animate( 18, 5, 1, true, false, 1 ); break;
						}
					}

					PlaySound( GetIdleSound() );
					m_NextTalk = (DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( 15, 45 ) ));
				}
			}
		}
		
		public bool CanTellRumor()
		{
			return Fame == 0 && (this is HouseVisitor) == false;
		}

		private DateTime m_LastRumorTime = DateTime.MinValue;
		private const int TALK_TO_COOLDOWN_MINUTES = 15;

		public void MarkToldRumor()
		{
			if (!CanTellRumor()) return;

			Fame = 1;
			m_LastRumorTime = DateTime.Now;
			InvalidateProperties();
			Timer.DelayCall(TimeSpan.FromMinutes(TALK_TO_COOLDOWN_MINUTES), () => ResetToldRumor());
		}

		public void ResetToldRumor()
		{
			if (Fame == 0) return;

			var now = DateTime.Now;
			if (now < m_LastRumorTime.AddMinutes(TALK_TO_COOLDOWN_MINUTES)) return;

			Fame = 0;
			InvalidateProperties();
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (!CanTellRumor() && (this is HouseVisitor) == false)
			{
				list.Add("Recently Questioned");
			}
		}

		///////////////////////////////////////////////////////////////////////////
		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			if ( !(this is Humanoid || (this is HouseVisitor && (this.Body == 9 || this.Body == 320))) ){ list.Add( new SpeechGumpEntry( from, this ) ); }
		}

		public override bool TryTalk( PlayerMobile from )
		{
			if ( from == null )
				return false;
			if ( this is Humanoid || (this is HouseVisitor && (this.Body == 9 || this.Body == 320)) )
				return false;

			SpeechGumpEntry entry = new SpeechGumpEntry( from, this );

			if ( !from.CanSee( this ) || !from.InRange( this, entry.Range ) )
				return false;

			entry.OnClick();
			return true;
		}

		public class SpeechGumpEntry : ContextMenuEntry
		{
			private PlayerMobile m_Mobile;
			private Mobile m_Giver;
			
			public SpeechGumpEntry( Mobile from, Mobile giver ) : base( 6146, 3 )
			{
				m_Mobile = (PlayerMobile)from;
				m_Giver = giver;
			}

			public override void OnClick()
			{
				Citizens citizen = (Citizens)m_Giver;

				string speak = "";

				MLQuestSystem.OnDoubleClick(citizen, m_Mobile, false);

				if ( citizen.CanTellRumor() && m_Mobile.Backpack.FindItemByType( typeof ( MuseumBook ) ) != null )
				{
					speak = MuseumBook.TellRumor( m_Mobile, citizen );
				}
				if ( speak == "" && citizen.CanTellRumor() && m_Mobile.Backpack.FindItemByType( typeof ( QuestTome ) ) != null )
				{
					speak = QuestTome.TellRumor( m_Mobile, citizen );
				}

				if ( speak != "" )
				{
					m_Mobile.PlaySound( 0x5B6 );
					m_Giver.Say( speak );
				}
				else if ( citizen.CitizenService == 0 )
				{
					speak = citizen.CitizenRumor;
					if ( speak.Contains("Z~Z~Z~Z~Z") ){ speak = speak.Replace("Z~Z~Z~Z~Z", m_Mobile.Name); }
					if ( speak.Contains("Y~Y~Y~Y~Y") ){ speak = speak.Replace("Y~Y~Y~Y~Y", m_Mobile.Region.Name); }
					m_Giver.Say( speak );
				}
				else
				{
					m_Mobile.CloseGump( typeof( CitizenGump ) );
					m_Mobile.SendGump(new CitizenGump( m_Giver, m_Mobile ));
				}

				// They didn't tell a rumor, but we'll consume it anyways (to show the flag)
				citizen.MarkToldRumor();
			}
		}
		///////////////////////////////////////////////////////////////////////////

		public override bool OnBeforeDeath()
		{
			Say("In Vas Mani");
			this.Hits = this.HitsMax;
			this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
			this.PlaySound( 0x202 );
			return false;
		}

		public override bool IsEnemy( Mobile m )
		{
			return false;
		}

		public static void PopulateCities()
		{
			ArrayList wanderers = new ArrayList();
			foreach ( Mobile wanderer in World.Mobiles.Values )
			{
				if ( wanderer is Citizens && !( wanderer is HouseVisitor || wanderer is AdventurerWest || wanderer is AdventurerSouth || wanderer is AdventurerNorth || wanderer is AdventurerEast || wanderer is TavernPatronWest || wanderer is TavernPatronSouth || wanderer is TavernPatronNorth || wanderer is TavernPatronEast ) )
				{
					wanderers.Add( wanderer );
				}
				else if ( wanderer is HouseVisitor && wanderer.Karma != 1 )
				{
					wanderers.Add( wanderer );
				}
			}
			for ( int i = 0; i < wanderers.Count; ++i )
			{
				Mobile person = ( Mobile )wanderers[ i ];
				//Effects.SendLocationParticles( EffectItem.Create( person.Location, person.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				//person.PlaySound( 0x1FE );
				person.Delete();
			}

			ArrayList meetingSpots = new ArrayList();
			ArrayList meetingPets = new ArrayList();
			ArrayList meetingLawns = new ArrayList();
			ArrayList meetingDemons = new ArrayList();
			foreach ( Item item in World.Items.Values )
			{
				if ( item is MeetingSpots )
				{
					meetingSpots.Add( item );
				}
				else if ( item is MeetingPets )
				{
					meetingPets.Add( item );
				}
				else if ( ( item is LawnItem || item is ShantyItem ) && ( item.Name == "burning pit" || item.Name == "huge fire" ) )
				{
					meetingLawns.Add( item );
				}
				else if ( ( item is LawnPiece || item is ShantyPiece ) && item.Name == "summoning pentagram" && ( item.ItemID == 0x647 || item.ItemID == 0xFEA ) )
				{
					meetingDemons.Add( item );
				}
			}
			for ( int i = 0; i < meetingSpots.Count; ++i )
			{
				Item spot = ( Item )meetingSpots[ i ];
				if ( PeopleMeetingHere( spot ) ){ CreateCitizenss( spot ); }
			}
			for ( int i = 0; i < meetingPets.Count; ++i )
			{
				Item spot = ( Item )meetingPets[ i ];
				if ( MyServerSettings.Humanoid() ){ CreatePets( spot ); }
			}
			for ( int i = 0; i < meetingLawns.Count; ++i )
			{
				Item spot = ( Item )meetingLawns[ i ];
				CreateCitizenss( spot );
			}
			for ( int i = 0; i < meetingDemons.Count; ++i )
			{
				Item spot = ( Item )meetingDemons[ i ];
				CreateDaemons( spot );
			}
			CreateDragonRiders();
		}

		public static void CreatePets ( Item spot )
		{
			Mobile pet = new Humanoid();
			pet.MoveToWorld( spot.Location, spot.Map );
			Humanoid.HumanoidSetup( pet, true );

			if ( spot.Name == "east" ){ pet.Direction = Direction.East; }
			else if ( spot.Name == "south" ){ pet.Direction = Direction.South; }
			else if ( spot.Name == "west" ){ pet.Direction = Direction.West; }
			else if ( spot.Name == "north" ){ pet.Direction = Direction.North; }
		}

		public static void CreateDaemons ( Item spot )
		{
			Mobile demon = new HouseVisitor();
			demon.Body = Utility.RandomList( 9, 320 );
			demon.Name = NameList.RandomName( "devil" );
			demon.Title = null;
			demon.Hue = Utility.RandomMonsterHue();
			demon.BaseSoundID = 357;
			demon.MoveToWorld( spot.Location, spot.Map );
		}

		public static void CreateCitizenss ( Item spot )
		{
			Region reg = Region.Find( spot.Location, spot.Map );

			int total = 0;
			int mod = 2;

			bool mount = false;

			Mobile m = new ShrineCritter();
			m.MoveToWorld( spot.Location, spot.Map );

			if ( reg.Name == "the Lyceum" ){ mount = false; mod = 3; }
			else if ( Server.Mobiles.AnimalTrainer.IsNoMountRegion( m, m.Region ) && MySettings.S_NoMountsInCertainRegions ){ /* DO NOTHING IN NO MOUNT REGIONS */ }
			else if ( MySettings.S_NoMountBuilding && Server.Misc.Worlds.InBuilding( m ) ){ /* DO NOTHING IN NO MOUNT REGIONS */ }
			else if ( spot is LawnItem || spot is ShantyItem ){} // NO MOUNTS ON LAWNS
			else if ( Worlds.IsSeaTown( spot.Location, spot.Map ) && !reg.IsPartOf( "the Port" ) ){} // SEA AREAS BUT NOT THE MAIN PORT...NO HORSES
			else if ( spot.Map == Map.Sosaria && spot.Z >= 105 && spot.X == 1418 && spot.Y == 3665 ){} // UMBER VEIL TOWER...NO HORSES
			else if ( Utility.RandomBool() ){ mount = true; mod = 3; }

			m.Delete();

			Point3D cit1 = new Point3D( ( spot.X-mod ), ( spot.Y ),   	spot.Z );	Direction dir1 = Direction.East;
			Point3D cit2 = new Point3D( ( spot.X   ), ( spot.Y-mod ),   spot.Z );	Direction dir2 = Direction.South;
			Point3D cit3 = new Point3D( ( spot.X+mod ), ( spot.Y ),   	spot.Z );	Direction dir3 = Direction.West;
			Point3D cit4 = new Point3D( ( spot.X   ), ( spot.Y+mod ),	spot.Z );	Direction dir4 = Direction.North;

			Mobile citizen = null;
			int humanoids = 0;
			int humanE = 0;
			int humanS = 0;
			int humanW = 0;
			int humanN = 0;

			bool canSpawn1 = true;
			int z_1 = cit1.Z;
				if ( (spot.Map).GetAverageZ(cit1.X, cit1.Y) != z_1 ){ z_1 = (spot.Map).GetAverageZ(cit1.X, cit1.Y); }
				if ( !((spot.Map).CanSpawnMobile( cit1.X, cit1.Y, z_1 )) ){ z_1 = cit1.Z; }
				if ( spot is LawnItem && (spot.Map).CanSpawnMobile( cit1.X, cit1.Y, z_1 ) && !(Region.Find( cit1, spot.Map ) is HouseRegion) ){ canSpawn1 = true; cit1 = new Point3D( ( spot.X-mod ), ( spot.Y ), z_1 ); }
				else if ( spot is ShantyItem && (spot.Map).CanSpawnMobile( cit1.X, cit1.Y, z_1 ) && Region.Find( cit1, spot.Map ) is HouseRegion ){ canSpawn1 = true; cit1 = new Point3D( ( spot.X-mod ), ( spot.Y ), z_1 ); }
				else if ( spot is LawnItem || spot is ShantyItem ){ canSpawn1 = false; }

			bool canSpawn2 = true;
			int z_2 = cit2.Z;
				if ( (spot.Map).GetAverageZ(cit2.X, cit2.Y) != z_2 ){ z_2 = (spot.Map).GetAverageZ(cit2.X, cit2.Y); }
				if ( !((spot.Map).CanSpawnMobile( cit2.X, cit2.Y, z_2 )) ){ z_2 = cit2.Z; }
				if ( spot is ShantyItem && (spot.Map).CanSpawnMobile( cit2.X, cit2.Y, z_2 ) && !(Region.Find( cit2, spot.Map ) is HouseRegion) ){ canSpawn2 = true; cit2 = new Point3D( ( spot.X ), ( spot.Y-mod ), z_2 ); }
				else if ( spot is ShantyItem && (spot.Map).CanSpawnMobile( cit2.X, cit2.Y, z_2 ) && Region.Find( cit2, spot.Map ) is HouseRegion ){ canSpawn2 = true; cit2 = new Point3D( ( spot.X ), ( spot.Y-mod ), z_2 ); }
				else if ( spot is LawnItem || spot is ShantyItem ){ canSpawn2 = false; }

			bool canSpawn3 = true;
			int z_3 = cit3.Z;
				if ( (spot.Map).GetAverageZ(cit3.X, cit3.Y) != z_3 ){ z_3 = (spot.Map).GetAverageZ(cit3.X, cit3.Y); }
				if ( !((spot.Map).CanSpawnMobile( cit3.X, cit3.Y, z_3 )) ){ z_3 = cit3.Z; }
				if ( spot is LawnItem && (spot.Map).CanSpawnMobile( cit3.X, cit3.Y, z_3 ) && !(Region.Find( cit3, spot.Map ) is HouseRegion) ){ canSpawn3 = true; cit3 = new Point3D( ( spot.X+mod ), ( spot.Y ), z_3 ); }
				else if ( spot is LawnItem && (spot.Map).CanSpawnMobile( cit3.X, cit3.Y, z_3 ) && Region.Find( cit3, spot.Map ) is HouseRegion ){ canSpawn3 = true; cit3 = new Point3D( ( spot.X+mod ), ( spot.Y ), z_3 ); }
				else if ( spot is LawnItem || spot is ShantyItem ){ canSpawn3 = false; }

			bool canSpawn4 = true;
			int z_4 = cit4.Z;
				if ( (spot.Map).GetAverageZ(cit4.X, cit4.Y) != z_4 ){ z_4 = (spot.Map).GetAverageZ(cit4.X, cit4.Y); }
				if ( !((spot.Map).CanSpawnMobile( cit4.X, cit4.Y, z_4 )) ){ z_4 = cit4.Z; }
				if ( spot is LawnItem && (spot.Map).CanSpawnMobile( cit4.X, cit4.Y, z_4 ) && !(Region.Find( cit4, spot.Map ) is HouseRegion) ){ canSpawn4 = true; cit4 = new Point3D( ( spot.X ), ( spot.Y+mod ), z_4 ); }
				else if ( spot is LawnItem && (spot.Map).CanSpawnMobile( cit4.X, cit4.Y, z_4 ) && Region.Find( cit4, spot.Map ) is HouseRegion ){ canSpawn4 = true; cit4 = new Point3D( ( spot.X ), ( spot.Y+mod ), z_4 ); }
				else if ( spot is LawnItem || spot is ShantyItem ){ canSpawn4 = false; }

			bool process = true;
			int w = 0;

			while ( process )
			{
				w++;
				if ( ( spot is LawnItem || spot is ShantyItem ) && w > 10 ){ process = false; }

				if ( Utility.RandomBool() && humanE == 0 && canSpawn1 )
				{
					citizen = null;
					total++;
					while (citizen == null )
					{
						citizen = new Citizens();
						if ( citizen != null )
						{
							humanE = 1;
							if ( MyServerSettings.Humanoids() && humanoids < 1 && !(spot is LawnItem) && !(spot is ShantyItem) ){ citizen.Delete(); citizen = new Humanoid(); humanoids++; }
							else if ( spot is LawnItem || spot is ShantyItem ){ citizen.Delete(); citizen = new HouseVisitor(); }
							if ( !(spot is LawnItem) && !(spot is ShantyItem) ){ citizen.AddItem( new LightCitizen( false ) ); }
							citizen.MoveToWorld( cit1, spot.Map );
							if ( citizen is Humanoid ){ Humanoid.HumanoidSetup( citizen, false ); total=total-1;}
							if ( mount ){ MountCitizens ( citizen, true ); }
							citizen.Direction = dir1;
							((BaseCreature)citizen).ControlSlots = 2;
						}
					}
				}
				if ( Utility.RandomMinMax( 1, 3 ) == 1 && humanS == 0 && canSpawn2 )
				{
					citizen = null;
					total++;
					while (citizen == null )
					{
						citizen = new Citizens();
						if ( citizen != null )
						{
							humanS = 1;
							if ( MyServerSettings.Humanoids() && humanoids < 1 && !(spot is LawnItem) && !(spot is ShantyItem) ){ citizen.Delete(); citizen = new Humanoid(); humanoids++; }
							else if ( spot is LawnItem || spot is ShantyItem ){ citizen.Delete(); citizen = new HouseVisitor(); }
							if ( !(spot is LawnItem) && !(spot is ShantyItem) ){ citizen.AddItem( new LightCitizen( false ) ); }
							citizen.MoveToWorld( cit2, spot.Map );
							if ( citizen is Humanoid ){ Humanoid.HumanoidSetup( citizen, false ); total=total-1;}
							if ( mount ){ MountCitizens ( citizen, true ); }
							citizen.Direction = dir2;
							((BaseCreature)citizen).ControlSlots = 3;
						}
					}
				}
				if ( ( Utility.RandomMinMax( 1, 4 ) == 1 || total == 0 ) && humanW == 0 && canSpawn3 )
				{
					citizen = null;
					total++;
					while (citizen == null )
					{
						citizen = new Citizens();
						if ( citizen != null )
						{
							humanW = 1;
							if ( MyServerSettings.Humanoids() && humanoids < 1 && !(spot is LawnItem) && !(spot is ShantyItem) ){ citizen.Delete(); citizen = new Humanoid(); humanoids++; }
							else if ( spot is LawnItem || spot is ShantyItem ){ citizen.Delete(); citizen = new HouseVisitor(); }
							if ( !(spot is LawnItem) && !(spot is ShantyItem) ){ citizen.AddItem( new LightCitizen( false ) ); }
							citizen.MoveToWorld( cit3, spot.Map );
							if ( citizen is Humanoid ){ Humanoid.HumanoidSetup( citizen, false ); total=total-1;}
							if ( mount ){ MountCitizens ( citizen, true ); }
							citizen.Direction = dir3;
							((BaseCreature)citizen).ControlSlots = 4;
						}
					}
				}
				if ( ( Utility.RandomMinMax( 1, 4 ) == 1 || total < 2 ) && humanN == 0 && canSpawn4 )
				{
					citizen = null;
					total++;
					while (citizen == null )
					{
						citizen = new Citizens();
						if ( citizen != null )
						{
							humanN = 1;
							if ( MyServerSettings.Humanoids() && humanoids < 1 && !(spot is LawnItem) && !(spot is ShantyItem) ){ citizen.Delete(); citizen = new Humanoid(); humanoids++; }
							else if ( spot is LawnItem || spot is ShantyItem ){ citizen.Delete(); citizen = new HouseVisitor(); }
							if ( !(spot is LawnItem) && !(spot is ShantyItem) ){ citizen.AddItem( new LightCitizen( false ) ); }
							citizen.MoveToWorld( cit4, spot.Map );
							if ( citizen is Humanoid ){ Humanoid.HumanoidSetup( citizen, false ); total=total-1;}
							if ( mount ){ MountCitizens ( citizen, true ); }
							citizen.Direction = dir4;
							((BaseCreature)citizen).ControlSlots = 5;
						}
					}
				}
				if ( total >= 2 ){ process = false; }
			}
		}

		public static void CreateDragonRiders()
		{
			Point3D loc; Map map; Direction direction;

			if ( Utility.RandomBool() ){ loc = new Point3D( 3022, 969, 70 ); map = Map.Sosaria; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the City of Britain
			if ( Utility.RandomBool() ){ loc = new Point3D( 2985, 1042, 45 ); map = Map.Sosaria; direction = Direction.East; CreateDragonRider ( loc, map, direction ); } // the City of Britain
			if ( Utility.RandomBool() ){ loc = new Point3D( 6728, 1797, 30 ); map = Map.Sosaria; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the City of Kuldara
			if ( Utility.RandomBool() ){ loc = new Point3D( 6752, 1665, 80 ); map = Map.Sosaria; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the City of Kuldara
			if ( Utility.RandomBool() ){ loc = new Point3D( 355, 1071, 65 ); map = Map.IslesDread; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the Cimmeran Hold
			if ( Utility.RandomBool() ){ loc = new Point3D( 385, 1044, 99 ); map = Map.IslesDread; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the Cimmeran Hold
			if ( Utility.RandomBool() ){ loc = new Point3D( 392, 1096, 59 ); map = Map.IslesDread; direction = Direction.East; CreateDragonRider ( loc, map, direction ); } // the Cimmeran Hold
			if ( Utility.RandomBool() ){ loc = new Point3D( 1441, 3779, 30 ); map = Map.Sosaria; direction = Direction.East; CreateDragonRider ( loc, map, direction ); } // the Town of Renika
			if ( Utility.RandomBool() ){ loc = new Point3D( 1395, 3668, 115 ); map = Map.Sosaria; direction = Direction.Down; CreateDragonRider ( loc, map, direction ); } // the Island of Umber Veil
			if ( Utility.RandomBool() ){ loc = new Point3D( 795, 1016, 90 ); map = Map.SerpentIsland; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the City of Furnace
			if ( Utility.RandomBool() ){ loc = new Point3D( 878, 1135, 125 ); map = Map.SerpentIsland; direction = Direction.East; CreateDragonRider ( loc, map, direction ); } // the City of Furnace
			if ( Utility.RandomBool() ){ loc = new Point3D( 291, 1736, 60 ); map = Map.SavagedEmpire; direction = Direction.East; CreateDragonRider ( loc, map, direction ); } // the Village of Barako
			if ( Utility.RandomBool() ){ loc = new Point3D( 282, 1631, 110 ); map = Map.SavagedEmpire; direction = Direction.North; CreateDragonRider ( loc, map, direction ); } // the Savaged Empire
			if ( Utility.RandomBool() ){ loc = new Point3D( 786, 875, 55 ); map = Map.SavagedEmpire; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the Village of Kurak
			if ( Utility.RandomBool() ){ loc = new Point3D( 821, 982, 80 ); map = Map.SavagedEmpire; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the Village of Kurak
			if ( Utility.RandomBool() ){ loc = new Point3D( 2687, 3165, 60 ); map = Map.Lodor; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the Port of Dusk
			if ( Utility.RandomBool() ){ loc = new Point3D( 2956, 1248, 70 ); map = Map.Lodor; direction = Direction.North; CreateDragonRider ( loc, map, direction ); } // the City of Elidor
			if ( Utility.RandomBool() ){ loc = new Point3D( 2970, 1319, 45 ); map = Map.Lodor; direction = Direction.East; CreateDragonRider ( loc, map, direction ); } // the City of Elidor
			if ( Utility.RandomBool() ){ loc = new Point3D( 2902, 1399, 55 ); map = Map.Lodor; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the City of Elidor
			if ( Utility.RandomBool() ){ loc = new Point3D( 3737, 397, 44 ); map = Map.Lodor; direction = Direction.East; CreateDragonRider ( loc, map, direction ); } // the Town of Glacial Hills
			if ( Utility.RandomBool() ){ loc = new Point3D( 3660, 470, 44 ); map = Map.Lodor; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the Town of Glacial Hills
			if ( Utility.RandomBool() ){ loc = new Point3D( 4215, 2993, 60 ); map = Map.Lodor; direction = Direction.East; CreateDragonRider ( loc, map, direction ); } // Greensky Village
			if ( Utility.RandomBool() ){ loc = new Point3D( 2827, 2258, 35 ); map = Map.Lodor; direction = Direction.East; CreateDragonRider ( loc, map, direction ); } // the Village of Islegem
			if ( Utility.RandomBool() ){ loc = new Point3D( 4842, 3266, 50 ); map = Map.Lodor; direction = Direction.Down; CreateDragonRider ( loc, map, direction ); } // Kraken Reef Docks
			if ( Utility.RandomBool() ){ loc = new Point3D( 4815, 3112, 73 ); map = Map.Lodor; direction = Direction.Up; CreateDragonRider ( loc, map, direction ); } // Kraken Reef Docks
			if ( Utility.RandomBool() ){ loc = new Point3D( 4712, 3194, 84 ); map = Map.Lodor; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // Kraken Reef Docks
			if ( Utility.RandomBool() ){ loc = new Point3D( 1809, 2224, 70 ); map = Map.Lodor; direction = Direction.Right; CreateDragonRider ( loc, map, direction ); } // the City of Lodoria
			if ( Utility.RandomBool() ){ loc = new Point3D( 1942, 2185, 57 ); map = Map.Lodor; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the City of Lodoria
			if ( Utility.RandomBool() ){ loc = new Point3D( 2084, 2195, 32 ); map = Map.Lodor; direction = Direction.East; CreateDragonRider ( loc, map, direction ); } // the City of Lodoria
			if ( Utility.RandomBool() ){ loc = new Point3D( 841, 2019, 55 ); map = Map.Lodor; direction = Direction.East; CreateDragonRider ( loc, map, direction ); } // the Village of Portshine
			if ( Utility.RandomBool() ){ loc = new Point3D( 6763, 3649, 122 ); map = Map.Lodor; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the Village of Ravendark
			if ( Utility.RandomBool() ){ loc = new Point3D( 6759, 3756, 76 ); map = Map.Lodor; direction = Direction.Right; CreateDragonRider ( loc, map, direction ); } // the Village of Ravendark
			if ( Utility.RandomBool() ){ loc = new Point3D( 4232, 1454, 48 ); map = Map.Lodor; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the Village of Springvale
			if ( Utility.RandomBool() ){ loc = new Point3D( 4293, 1492, 45 ); map = Map.Lodor; direction = Direction.East; CreateDragonRider ( loc, map, direction ); } // the Village of Springvale
			if ( Utility.RandomBool() ){ loc = new Point3D( 4172, 1489, 45 ); map = Map.Lodor; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the Village of Springvale
			if ( Utility.RandomBool() ){ loc = new Point3D( 2381, 3155, 28 ); map = Map.Lodor; direction = Direction.East; CreateDragonRider ( loc, map, direction ); } // the Port of Starguide
			if ( Utility.RandomBool() ){ loc = new Point3D( 2302, 3154, 52 ); map = Map.Lodor; direction = Direction.West; CreateDragonRider ( loc, map, direction ); } // the Port of Starguide
			if ( Utility.RandomBool() ){ loc = new Point3D( 876, 904, 30 ); map = Map.Lodor; direction = Direction.Down; CreateDragonRider ( loc, map, direction ); } // the Village of Whisper
			if ( Utility.RandomBool() ){ loc = new Point3D( 1101, 321, 66 ); map = Map.SavagedEmpire; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // Savage Sea Docks
			if ( Utility.RandomBool() ){ loc = new Point3D( 952, 1801, 50 ); map = Map.SerpentIsland; direction = Direction.Down; CreateDragonRider ( loc, map, direction ); } // Serpent Sail Docks
			if ( Utility.RandomBool() ){ loc = new Point3D( 315, 1407, 17 ); map = Map.Sosaria; direction = Direction.Left; CreateDragonRider ( loc, map, direction ); } // Anchor Rock Docks
			if ( Utility.RandomBool() ){ loc = new Point3D( 415, 1292, 67 ); map = Map.Sosaria; direction = Direction.East; CreateDragonRider ( loc, map, direction ); } // Anchor Rock Docks
			if ( Utility.RandomBool() ){ loc = new Point3D( 5932, 2868, 45 ); map = Map.Sosaria; direction = Direction.East; CreateDragonRider ( loc, map, direction ); } // the Lunar City of Dawn
			if ( Utility.RandomBool() ){ loc = new Point3D( 3705, 1486, 55 ); map = Map.Sosaria; direction = Direction.Down; CreateDragonRider ( loc, map, direction ); } // Death Gulch
			if ( Utility.RandomBool() ){ loc = new Point3D( 1608, 1507, 48 ); map = Map.Sosaria; direction = Direction.Down; CreateDragonRider ( loc, map, direction ); } // The Town of Devil Guard
			if ( Utility.RandomBool() ){ loc = new Point3D( 2084, 258, 60 ); map = Map.Sosaria; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the Village of Fawn
			if ( Utility.RandomBool() ){ loc = new Point3D( 2168, 305, 60 ); map = Map.Sosaria; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the Village of Fawn
			if ( Utility.RandomBool() ){ loc = new Point3D( 4781, 1185, 50 ); map = Map.Sosaria; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // Glacial Coast Village
			if ( Utility.RandomBool() ){ loc = new Point3D( 869, 2068, 40 ); map = Map.Sosaria; direction = Direction.North; CreateDragonRider ( loc, map, direction ); } // the Village of Grey
			if ( Utility.RandomBool() ){ loc = new Point3D( 3070, 2615, 60 ); map = Map.Sosaria; direction = Direction.Up; CreateDragonRider ( loc, map, direction ); } // the City of Montor
			if ( Utility.RandomBool() ){ loc = new Point3D( 3180, 2613, 66 ); map = Map.Sosaria; direction = Direction.East; CreateDragonRider ( loc, map, direction ); } // the City of Montor
			if ( Utility.RandomBool() ){ loc = new Point3D( 3322, 2638, 70 ); map = Map.Sosaria; direction = Direction.East; CreateDragonRider ( loc, map, direction ); } // the City of Montor
			if ( Utility.RandomBool() ){ loc = new Point3D( 838, 692, 70 ); map = Map.Sosaria; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the Town of Moon
			if ( Utility.RandomBool() ){ loc = new Point3D( 4565, 1253, 82 ); map = Map.Sosaria; direction = Direction.Left; CreateDragonRider ( loc, map, direction ); } // the Town of Mountain Crest
			if ( Utility.RandomBool() ){ loc = new Point3D( 1823, 758, 70 ); map = Map.Sosaria; direction = Direction.Up; CreateDragonRider ( loc, map, direction ); } // the Land of Sosaria
			if ( Utility.RandomBool() ){ loc = new Point3D( 7089, 610, 100 ); map = Map.Sosaria; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the Port
			if ( Utility.RandomBool() ){ loc = new Point3D( 7025, 680, 120 ); map = Map.Sosaria; direction = Direction.South; CreateDragonRider ( loc, map, direction ); } // the Port
		}

		public static void CreateDragonRider ( Point3D loc, Map map, Direction direction )
		{
			DragonRider citizen = new DragonRider();
			citizen.MoveToWorld( loc, map );
			MountCitizens ( citizen, true );
			citizen.Direction = direction;
			((BaseCreature)citizen).ControlSlots = 2;
			//Effects.SendLocationParticles( EffectItem.Create( citizen.Location, citizen.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
			//citizen.PlaySound( 0x1FE );
		}

		public static void MountCitizens ( Mobile m, bool includeDragyns )
		{
			if ( m is DragonRider )
			{
				BaseMount dragon = new RidingDragon(); dragon.Body = Utility.RandomList( 59, 61 ); dragon.Blessed = true; dragon.Hue = Utility.RandomMonsterHue(); Server.Mobiles.BaseMount.Ride( dragon, m );
			}
			else if ( m is Humanoid ){ /* DO NOTHING FOR HUMANOIDS */ }
			else if ( m.Map == Map.Sosaria && m.X >= 2954 && m.Y >= 893 && m.X <= 3026 && m.Y <= 967 ){ /* DO NOTHING IN CASTLE BRITISH */ }
			else if ( m.Map == Map.Lodor && m.X >= 1759 && m.Y >= 2195 && m.X <= 1821 && m.Y <= 2241 ){ /* DO NOTHING IN CASTLE OF KNOWLEDGE */ }
			else if ( m.Map == Map.SavagedEmpire && m.X >= 309 && m.Y >= 1738 && m.X <= 323 && m.Y <= 1751 ){ /* DO NOTHING IN THIS SAVAGED EMPIRE SPOT */ }
			else if ( m.Map == Map.SavagedEmpire && m.X >= 284 && m.Y >= 1642 && m.X <= 298 && m.Y <= 1655 ){ /* DO NOTHING IN THIS SAVAGED EMPIRE SPOT */ }
			else if ( m.Map == Map.SavagedEmpire && m.X >= 785 && m.Y >= 896 && m.X <= 805 && m.Y <= 879 ){ /* DO NOTHING IN THIS SAVAGED EMPIRE SPOT */ }
			else if ( m.Map == Map.SavagedEmpire && m.X >= 706 && m.Y >= 953 && m.X <= 726 && m.Y <= 963 ){ /* DO NOTHING IN THIS SAVAGED EMPIRE SPOT */ }
			else if ( m.Map == Map.IslesDread && m.X >= 364 && m.Y >= 1027 && m.X <= 415 && m.Y <= 1057 ){ /* DO NOTHING IN THE CIMMERIAN CASTLE */ }
			else if ( m.Region.IsPartOf( "Kraken Reef Docks" ) || m.Region.IsPartOf( "Anchor Rock Docks" ) || m.Region.IsPartOf( "Serpent Sail Docks" ) || m.Region.IsPartOf( "Savage Sea Docks" ) || m.Region.IsPartOf( "the Forgotten Lighthouse" ) ){ /* DO NOTHING ON THE PORTS */ }
			else if ( Server.Mobiles.AnimalTrainer.IsNoMountRegion( m, m.Region ) && MySettings.S_NoMountsInCertainRegions ){ /* DO NOTHING IN NO MOUNT REGIONS */ }
			else if ( MySettings.S_NoMountBuilding && Server.Misc.Worlds.InBuilding( m ) ){ /* DO NOTHING IN NO MOUNT REGIONS */ }
			else if ( !(m is HouseVisitor ) )
			{
				BaseMount mount = new Horse();

				int roll = 0;

				switch ( Utility.Random( 30 ) )
				{
					case 0: roll = Utility.RandomMinMax( 1, 10 );
						switch ( roll )
						{
							case 1: mount = new CaveBearRiding();				break;
							case 2: mount = new DireBear();						break;
							case 3: mount = new ElderBlackBearRiding();			break;
							case 4: mount = new ElderBrownBearRiding();			break;
							case 5: mount = new ElderPolarBearRiding();			break;
							case 6: mount = new GreatBear();					break;
							case 7: mount = new GrizzlyBearRiding();			break;
							case 8: mount = new KodiakBear();					break;
							case 9: mount = new SabretoothBearRiding();			break;
							case 10: mount = new PandaRiding();					break;
						}
						break;
					case 1: roll = Utility.RandomMinMax( 1, 4 );
						switch ( roll )
						{
							case 1: mount = new BullradonRiding();				break;
							case 2: mount = new GorceratopsRiding();			break;
							case 3: mount = new GorgonRiding();					break;
							case 4: mount = new BasiliskRiding();				break;
						}
						break;
					case 2:
						roll = Utility.RandomMinMax( 0, 4 );
						if ( Server.Misc.MorphingTime.CheckNecro( m ) ){ roll = Utility.RandomMinMax( 3, 4 ); }
						switch ( roll )
						{
							case 0: mount = new WolfDire();			break;
							case 1: mount = new WhiteWolf();		break;
							case 2: mount = new WinterWolf();		break;
							case 3: mount = new BlackWolf();		break;
							case 4: mount = new DemonDog();			Server.Misc.MorphingTime.TurnToNecromancer( m );	break;
						}
						break;
					case 3: roll = Utility.RandomMinMax( 1, 6 );
						switch ( roll )
						{
							case 1: mount = new LionRiding();				break;
							case 2: mount = new SnowLion();					break;
							case 3: mount = new TigerRiding();				break;
							case 4: mount = new WhiteTigerRiding();			break;
							case 5: mount = new PredatorHellCatRiding();	break;
							case 6: mount = new SabretoothTigerRiding();	break;
						}
						break;
					case 4:
						switch ( Utility.RandomMinMax( 1, 4 ) )
						{
							case 1: mount = new DesertOstard();		break;
							case 2: mount = new ForestOstard();		break;
							case 3: mount = new FrenziedOstard();	break;
							case 4: mount = new SnowOstard();		break;
						}
						break;
					case 5: roll = Utility.RandomMinMax( 1, 5 );
						switch ( roll )
						{
							case 1: mount = new GiantHawk();		break;
							case 2: mount = new GiantRaven();		break;
							case 3: mount = new Roc();				break;
							case 4: mount = new Phoenix();			break;
							case 5: mount = new AxeBeakRiding();	break;
						}
						break;
					case 6:
						switch ( Utility.RandomMinMax( 1, 4 ) )
						{
							case 1: mount = new SwampDrakeRiding();																break;
							case 2: mount = new Wyverns();																		break;
							case 3: mount = new Teradactyl();																	break;
							case 4: mount = new GemDragon(); mount.Hue = 0; mount.ItemID = Utility.RandomMinMax( 595, 596 );	break;
						}
						break;
					case 7:
						switch ( Utility.RandomMinMax( 1, 6 ) )
						{
							case 1: mount = new Beetle();					break;
							case 2: mount = new FireBeetle();				break;
							case 3: mount = new GlowBeetleRiding();			break;
							case 4: mount = new PoisonBeetleRiding();		break;
							case 5: mount = new TigerBeetleRiding();		break;
							case 6: mount = new WaterBeetleRiding();		break;
						}
						break;
					case 8: roll = Utility.RandomMinMax( 1, 5 );
						switch ( roll )
						{
							case 1: mount = new RaptorRiding();													break;
							case 2: mount = new RavenousRiding();												break;
							case 3: mount = new RaptorRiding();			mount.Body = 116;	mount.ItemID = 116;	break;
							case 4: mount = new RaptorRiding();			mount.Body = 117;	mount.ItemID = 117;	break;
							case 5: mount = new RaptorRiding();			mount.Body = 219;	mount.ItemID = 219;	break;
						}
						break;
					case 9:
						roll = 0; if ( !MyServerSettings.SafariStore() ){ roll = 1; }
						roll = Utility.RandomMinMax( roll, 8 );
						if ( Server.Misc.MorphingTime.CheckNecro( m ) ){ roll = Utility.RandomMinMax( 3, 8 ); }
						switch ( roll )
						{
							case 0: mount = new ZebraRiding();					break;
							case 1: mount = new Unicorn();						break;
							case 2: mount = new IceSteed();						break;
							case 3: mount = new FireSteed();					break;
							case 4: mount = new Nightmare();					break;
							case 5: mount = new AncientNightmareRiding();		break;
							case 6: mount = new DarkUnicornRiding();			Server.Misc.MorphingTime.TurnToNecromancer( m );	break;
							case 7: mount = new HellSteed();					Server.Misc.MorphingTime.TurnToNecromancer( m );	break;
							case 8: mount = new Dreadhorn();					break;
						}
						break;
					case 10: roll = Utility.RandomMinMax( 1, 7 );
						switch ( roll )
						{
							case 1: mount = new Ramadon();				break;
							case 2: mount = new RidableLlama();			break;
							case 3: mount = new GriffonRiding();		break;
							case 4: mount = new HippogriffRiding();		break;
							case 5: mount = new Kirin();				break;
							case 6: mount = new ManticoreRiding();		break;
							case 7: mount = new SphinxRiding();			break;
						}
						break;
				}

				if ( mount is Horse && Utility.RandomMinMax(1,50) == 1 )
				{
					mount.Body = 587;
					mount.ItemID = 587;
					Item temp = new PlateHelm();
					ResourceMods.SetRandomResource( false, false, temp, CraftResource.Iron, true, null );
					mount.Resource = temp.Resource;
					mount.Hue = CraftResources.GetClr(mount.Resource);
					temp.Delete();
				}

				Server.Mobiles.BaseMount.Ride( mount, m );
			}
		}

		public static bool PeopleMeetingHere( Item spot )
		{
			if ( Utility.RandomBool() )
				return true;

			if ( (Region.Find( spot.Location, spot.Map )).Name == "the Lyceum" ) 
				return true;

			return false;
		}

		public Citizens( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
			writer.Write( CitizenService );
			writer.Write( CitizenType );
			writer.Write( CitizenCost );
			writer.Write( CitizenPhrase );
			writer.Write( CitizenRumor );
			writer.Write( ShouldRemoveSomeStuff );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			CitizenService = reader.ReadInt();
			CitizenType = reader.ReadInt();
			CitizenCost = reader.ReadInt();
			CitizenPhrase = reader.ReadString();
			CitizenRumor = reader.ReadString();
			if ( 0 < version )
				ShouldRemoveSomeStuff = reader.ReadBool();
		}

		public override void OnAfterSpawn()
		{
			base.OnAfterSpawn();

			if ( !(this is Humanoid) )
			{
				Server.Items.EssenceBase.ColorCitizen( this );
				Server.Misc.MorphingTime.CheckNecromancer( this );

				if ( this.Home.X > 0 && this.Home.Y > 0 && ( Math.Abs( this.X-this.Home.X ) > 2 || Math.Abs( this.Y-this.Home.Y ) > 2 || Math.Abs( this.Z-this.Home.Z ) > 2 ) )
				{
					this.Location = this.Home;
				}
				if ( Server.Misc.Worlds.isOrientalRegion( this ) )
				{
					Server.Misc.MorphingTime.RemoveMyClothes( this );
					if ( CitizenType == 1 ){ 		Server.Misc.IntelligentAction.DressUpWizards( this, true ); }
					else if ( CitizenType == 2 || this is Warriors )
					{
						if ( Utility.RandomBool() )
						{
							Server.Misc.IntelligentAction.DressUpFighters( this, "", false, true, false );
						}
						else
						{
							Server.Misc.IntelligentAction.DressUpRogues( this, "", false, true, false );
						}
					}
					else if ( CitizenType == 3 ){ 	Server.Misc.IntelligentAction.DressUpRogues( this, "", false, true, false ); }

					Title = TavernPatrons.GetTitle();
				}
			}
		}

		protected override void OnMapChange( Map oldMap )
		{
			base.OnMapChange( oldMap );
			Server.Misc.MorphingTime.CheckNecromancer( this );
		}

		public class CitizenGump : Gump
		{
			private Mobile c_Citizen;
			private Mobile c_Player;

			public CitizenGump( Mobile citizen, Mobile player ) : base( 25, 25 )
			{
				c_Citizen = citizen;
				Citizens b_Citizen = (Citizens)citizen;
				c_Player = player;

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				string speak = b_Citizen.CitizenPhrase;
				if ( speak.Contains("Z~Z~Z~Z~Z") ){ speak = speak.Replace("Z~Z~Z~Z~Z", c_Player.Name); }
				if ( speak.Contains("Y~Y~Y~Y~Y") ){ speak = speak.Replace("Y~Y~Y~Y~Y", c_Player.Region.Name); }
				if ( speak.Contains("G~G~G~G~G") ){ speak = speak.Replace("G~G~G~G~G", (b_Citizen.CitizenCost).ToString()); }

				AddPage(0);

				string color = "#d5a496";

				AddImage(0, 2, 9543, Server.Misc.PlayerSettings.GetGumpHue( player ));
				AddHtml( 12, 15, 341, 20, @"<BODY><BASEFONT Color=" + color + ">" + citizen.Name + " " + citizen.Title + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 12, 50, 380, 253, @"<BODY><BASEFONT Color=" + color + ">" + speak + "</BASEFONT></BODY>", (bool)false, (bool)true);
				AddButton(367, 12, 4017, 4017, 0, GumpButtonType.Reply, 0);
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				Mobile from = sender.Mobile;
				from.SendSound( 0x4A ); 
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			from.CloseGump( typeof( CitizenGump ) );
			int sound = 0;
			string say = "";
			bool isArmor = false; if ( dropped is BaseArmor ){ isArmor = true; }
			bool isWeapon = false; if ( dropped is BaseWeapon ){ isWeapon = true; }
			bool isMetal = false; if ( CraftResources.GetType( dropped.Resource ) == CraftResourceType.Metal ){ isMetal = true; }
			bool isWood = false; if ( CraftResources.GetType( dropped.Resource ) == CraftResourceType.Wood ){ isWood = true; }
			bool isLeather = false; if ( CraftResources.GetType( dropped.Resource ) == CraftResourceType.Leather ){ isLeather = true; }
			bool fixArmor = false;
			bool fixWeapon = false;

			if ( dropped is Cargo )
			{
				Server.Items.Cargo.GiveCargo( (Cargo)dropped, this, from );
			}
			else if ( dropped is Gold )
			{
				if ( CitizenService > 0 && CitizenCost > 0 && CitizenCost == dropped.Amount )
				{
					Item give = Backpack.Items.FirstOrDefault();
					if (give != null)
					{
						dropped.Delete();
						sound = 0x2E6;
						say = "That is a fair trade.";
						give.Movable = true;
						give.InvalidateProperties();
						from.AddToBackpack( give );
					}

					CitizenService = 0;
				}
			}
			else if ( CitizenType == 1 )
			{
				if ( CitizenType == 1 && dropped.Enchanted != MagicSpell.None && dropped.EnchantUsesMax > 0 )
				{
					if ( dropped.EnchantUses < dropped.EnchantUsesMax && dropped.EnchantUsesMax > 0 )
					{
						dropped.EnchantUses = dropped.EnchantUsesMax;
						say = "Your item is charged.";
						sound = 0x5C1;
					}
					else { say = "That has too many charges already."; }
				}
			}
			else if ( CitizenService == 1 )
			{
				if ( CitizenType == 2 && isArmor && isMetal ){ fixArmor = true; sound = 0x541; }
				else if ( CitizenType == 3 && dropped is LockableContainer )
				{
					LockableContainer box = (LockableContainer)dropped;
					say = "I unlocked it for you.";
					sound = 0x241;
					box.Locked = false;
					box.DisableTrap();
					box.LockLevel = 0;
					box.MaxLockLevel = 0;
					box.RequiredSkill = 0;
				}
			}
			else if ( CitizenService == 2 )
			{
				if ( CitizenType == 2 && isWeapon && isMetal ){ fixWeapon = true; sound = 0x541; }
				else if ( CitizenType == 3 && isArmor && isLeather ){ fixArmor = true; sound = 0x248; }
				else if ( CitizenType == 3 && isWeapon && isLeather ){ fixWeapon = true; sound = 0x248; }
			}
			else if ( CitizenService == 3 )
			{
				if ( CitizenType == 2 && isWeapon && isWood ){ fixWeapon = true; sound = 0x23D; }
				else if ( CitizenType == 3 && isWeapon && isWood ){ fixWeapon = true; sound = 0x23D; }
			}
			else if ( CitizenService == 4 )
			{
				if ( CitizenType == 2 && isArmor && isWood ){ fixArmor = true; sound = 0x23D; }
				else if ( CitizenType == 3 && isArmor && isWood ){ fixArmor = true; sound = 0x23D; }
			}

			if ( fixArmor && dropped is BaseArmor )
			{
				say = "This is repaired and ready for battle.";
				BaseArmor ba = (BaseArmor)dropped;
				ba.MaxHitPoints -= 1;
				ba.HitPoints = ba.MaxHitPoints;
			}
			else if ( fixWeapon && dropped is BaseWeapon )
			{
				say = "This is repaired and is ready for battle.";
				BaseWeapon bw = (BaseWeapon)dropped;
				bw.MaxHitPoints -= 1;
				bw.HitPoints = bw.MaxHitPoints;
			}

			SayTo(from, say);
			if ( sound > 0 ){ from.PlaySound( sound ); }

			return false;
		}
	}
}