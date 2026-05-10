using System;
using Server;
using Server.Network;
using Server.Multis;
using Server.Gumps;
using Server.Mobiles;
using Server.Accounting;
using Server.Misc;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Commands.Generic;
using Server.Regions;

namespace Server.Items
{
	public class Prisoner : Item
	{
		private int PrisonerReward;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Prisoner_Reward { get{ return PrisonerReward; } set{ PrisonerReward = value; InvalidateProperties(); } }

		private int PrisonerJoin;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Prisoner_Join { get{ return PrisonerJoin; } set{ PrisonerJoin = value; InvalidateProperties(); } }

		private int PrisonerType;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Prisoner_Type { get{ return PrisonerType; } set{ PrisonerType = value; InvalidateProperties(); } }

		private string PrisonerName;
		[CommandProperty( AccessLevel.GameMaster )]
		public string Prisoner_Name { get{ return PrisonerName; } set{ PrisonerName = value; InvalidateProperties(); } }

		private string PrisonerTitle;
		[CommandProperty( AccessLevel.GameMaster )]
		public string Prisoner_Title { get{ return PrisonerTitle; } set{ PrisonerTitle = value; InvalidateProperties(); } }

		private int PrisonerBody;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Prisoner_Body { get{ return PrisonerBody; } set{ PrisonerBody = value; InvalidateProperties(); } }

		private int PrisonerSound;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Prisoner_Sound { get{ return PrisonerSound; } set{ PrisonerSound = value; InvalidateProperties(); } }

		[Constructable]
		public Prisoner() : base( 0x2019 )
		{
			Movable = false;
			Name = "caged creature";

			if ( PrisonerReward < 1 ){ BuildPrisoner(); }
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1070722, PrisonerName );
			list.Add( 1049644, PrisonerTitle );
        }

		public Prisoner( Serial serial ) : base( serial )
		{
		}

		public void BuildPrisoner()
		{
			int monster = Utility.RandomMinMax( 1, 47 );

			switch ( Utility.RandomMinMax( 0, 9 ) )
			{
				case 0: monster = Utility.RandomMinMax( 1, 12 ); break;
				case 1: monster = Utility.RandomMinMax( 13, 22 ); break;
				case 2: monster = Utility.RandomMinMax( 23, 30 ); break;
				case 3: monster = Utility.RandomMinMax( 31, 34 ); break;
				case 4: monster = Utility.RandomMinMax( 35, 41 ); break;
				case 5: monster = Utility.RandomMinMax( 42, 47 ); break;
				case 6: monster = 48; break;
				case 7: monster = 49; break;
				case 8: monster = 50; break;
			}

			switch ( monster )
			{
				case 1: PrisonerTitle = "the bugbear"; PrisonerName = NameList.RandomName( "giant" ); PrisonerBody = 343; PrisonerSound = 427; PrisonerType = 1; break;
				case 2: PrisonerTitle = "the morlock"; PrisonerName = NameList.RandomName( "savage" ); PrisonerBody = 332; PrisonerSound = 427; PrisonerType = 1; break;
				case 3: PrisonerTitle = "the mind flayer"; PrisonerName = NameList.RandomName( "vampire" ); PrisonerBody = 768; PrisonerSound = 898; PrisonerType = 3; break;
				case 4: PrisonerTitle = "the hobgoblin"; PrisonerName = NameList.RandomName( "giant" ); PrisonerBody = 11; PrisonerSound = 1114; PrisonerType = 1; break;
				case 5: PrisonerTitle = "the goblin"; PrisonerName = NameList.RandomName( "goblin" ); PrisonerBody = 647; PrisonerSound = 0x543; PrisonerType = 2; break;
				case 6: PrisonerTitle = "the goblin"; PrisonerName = NameList.RandomName( "goblin" ); PrisonerBody = 632; PrisonerSound = 0x543; PrisonerType = 1; break;
				case 7: PrisonerTitle = "the gnoll"; PrisonerName = NameList.RandomName( "urk" ); PrisonerBody = 510; PrisonerSound = 1114; PrisonerType = 1; break;
				case 8: PrisonerTitle = "the satyr"; PrisonerName = NameList.RandomName( "elf_male" ); PrisonerBody = 271; PrisonerSound = 1414; PrisonerType = 1; break;
				case 9: PrisonerTitle = "the centaur"; PrisonerName = NameList.RandomName( "centaur" ); PrisonerBody = 101; PrisonerSound = 679; PrisonerType = 2; break;
				case 10: PrisonerTitle = "the pixie"; PrisonerName = NameList.RandomName( "pixie" ); PrisonerBody = 128; PrisonerSound = 1127; PrisonerType = 3; break;
				case 11: PrisonerTitle = "the minotaur"; PrisonerName = NameList.RandomName( "greek" ); PrisonerBody = 78; PrisonerSound = 1358; PrisonerType = 1; break;
				case 12: PrisonerTitle = "the minotaur"; PrisonerName = NameList.RandomName( "greek" ); PrisonerBody = 650; PrisonerSound = 1358; PrisonerType = 1; break;
				case 13: PrisonerTitle = "the sleestax"; PrisonerName = NameList.RandomName( "lizardman" ); PrisonerBody = 541; PrisonerSound = 417; PrisonerType = 1; break;
				case 14: PrisonerTitle = "the sakkhra"; PrisonerName = NameList.RandomName( "lizardman" ); PrisonerBody = 326; PrisonerSound = 417; PrisonerType = 3; break;
				case 15: PrisonerTitle = "the sakkhra"; PrisonerName = NameList.RandomName( "lizardman" ); PrisonerBody = 333; PrisonerSound = 417; PrisonerType = 1; break;
				case 16: PrisonerTitle = "the sakkhra"; PrisonerName = NameList.RandomName( "lizardman" ); PrisonerBody = 324; PrisonerSound = 417; PrisonerType = 1; break;
				case 17: PrisonerTitle = "the lizardman"; PrisonerName = NameList.RandomName( "lizardman" ); PrisonerBody = 33; PrisonerSound = 417; PrisonerType = 1; break;
				case 18: PrisonerTitle = "the lizardman"; PrisonerName = NameList.RandomName( "lizardman" ); PrisonerBody = 35; PrisonerSound = 417; PrisonerType = 1; break;
				case 19: PrisonerTitle = "the lizardman"; PrisonerName = NameList.RandomName( "lizardman" ); PrisonerBody = 36; PrisonerSound = 417; PrisonerType = 1; break;
				case 20: PrisonerTitle = "the kobold"; PrisonerName = NameList.RandomName( "goblin" ); PrisonerBody = 253; PrisonerSound = 0x543; PrisonerType = 3; break;
				case 21: PrisonerTitle = "the kobold"; PrisonerName = NameList.RandomName( "goblin" ); PrisonerBody = 245; PrisonerSound = 0x543; PrisonerType = 1; break;
				case 22: PrisonerTitle = "the grathek"; PrisonerName = NameList.RandomName( "lizardman" ); PrisonerBody = 534; PrisonerSound = 417; PrisonerType = 1; break;
				case 23: PrisonerTitle = "the orx"; PrisonerName = NameList.RandomName( "ork" ); PrisonerBody = 107; PrisonerSound = 1114; PrisonerType = 1; break;
				case 24: PrisonerTitle = "the orx"; PrisonerName = NameList.RandomName( "ork" ); PrisonerBody = 108; PrisonerSound = 1114; PrisonerType = 1; break;
				case 25: PrisonerTitle = "the orc"; PrisonerName = NameList.RandomName( "orc" ); PrisonerBody = 17; PrisonerSound = 1114; PrisonerType = 3; break;
				case 26: PrisonerTitle = "the orc"; PrisonerName = NameList.RandomName( "orc" ); PrisonerBody = 7; PrisonerSound = 1114; PrisonerType = 1; break;
				case 27: PrisonerTitle = "the orc"; PrisonerName = NameList.RandomName( "orc" ); PrisonerBody = 182; PrisonerSound = 1114; PrisonerType = 1; break;
				case 28: PrisonerTitle = "the urc"; PrisonerName = NameList.RandomName( "urk" ); PrisonerBody = 20; PrisonerSound = 1114; PrisonerType = 1; break;
				case 29: PrisonerTitle = "the urc"; PrisonerName = NameList.RandomName( "urk" ); PrisonerBody = 252; PrisonerSound = 1114; PrisonerType = 2; break;
				case 30: PrisonerTitle = "the urc"; PrisonerName = NameList.RandomName( "urk" ); PrisonerBody = 157; PrisonerSound = 1114; PrisonerType = 3; break;
				case 31: PrisonerTitle = "the tritun"; PrisonerName = NameList.RandomName( "drakkul" ); PrisonerBody = 690; PrisonerSound = 1363; PrisonerType = 1; break;
				case 32: PrisonerTitle = "the tritun"; PrisonerName = NameList.RandomName( "drakkul" ); PrisonerBody = 678; PrisonerSound = 1363; PrisonerType = 3; break;
				case 33: PrisonerTitle = "the neptar"; PrisonerName = NameList.RandomName( "drakkul" ); PrisonerBody = 677; PrisonerSound = 1363; PrisonerType = 3; break;
				case 34: PrisonerTitle = "the neptar"; PrisonerName = NameList.RandomName( "drakkul" ); PrisonerBody = 676; PrisonerSound = 1363; PrisonerType = 1; break;
				case 35: PrisonerTitle = "the ratman"; PrisonerName = NameList.RandomName( "ratman" ); PrisonerBody = 42; PrisonerSound = 437; PrisonerType = 2; break;
				case 36: PrisonerTitle = "the ratman"; PrisonerName = NameList.RandomName( "ratman" ); PrisonerBody = 44; PrisonerSound = 437; PrisonerType = 1; break;
				case 37: PrisonerTitle = "the ratman"; PrisonerName = NameList.RandomName( "ratman" ); PrisonerBody = 45; PrisonerSound = 437; PrisonerType = 1; break;
				case 38: PrisonerTitle = "the ratman"; PrisonerName = NameList.RandomName( "ratman" ); PrisonerBody = 163; PrisonerSound = 437; PrisonerType = 1; break;
				case 39: PrisonerTitle = "the ratman"; PrisonerName = NameList.RandomName( "ratman" ); PrisonerBody = 164; PrisonerSound = 437; PrisonerType = 1; break;
				case 40: PrisonerTitle = "the ratman"; PrisonerName = NameList.RandomName( "ratman" ); PrisonerBody = 165; PrisonerSound = 437; PrisonerType = 1; break;
				case 41: PrisonerTitle = "the ratman"; PrisonerName = NameList.RandomName( "ratman" ); PrisonerBody = 73; PrisonerSound = 437; PrisonerType = 3; break;
				case 42: PrisonerTitle = "the serpyn"; PrisonerName = NameList.RandomName( "drakkul" ); PrisonerBody = 143; PrisonerSound = 634; PrisonerType = 1; break;
				case 43: PrisonerTitle = "the serpyn"; PrisonerName = NameList.RandomName( "drakkul" ); PrisonerBody = 145; PrisonerSound = 634; PrisonerType = 1; break;
				case 44: PrisonerTitle = "the serpyn"; PrisonerName = NameList.RandomName( "drakkul" ); PrisonerBody = 144; PrisonerSound = 644; PrisonerType = 3; break;
				case 45: PrisonerTitle = "the ophidian"; PrisonerName = NameList.RandomName( "drakkul" ); PrisonerBody = 85; PrisonerSound = 639; PrisonerType = 3; break;
				case 46: PrisonerTitle = "the ophidian"; PrisonerName = NameList.RandomName( "drakkul" ); PrisonerBody = 86; PrisonerSound = 634; PrisonerType = 1; break;
				case 47: PrisonerTitle = "the ophidian"; PrisonerName = NameList.RandomName( "drakkul" ); PrisonerBody = 87; PrisonerSound = 644; PrisonerType = 3; break;
				case 48:
							HenchmanItem fighter = new HenchmanFighterItem();
							PrisonerName = fighter.HenchName; 
							PrisonerTitle = fighter.HenchTitle; 
							PrisonerBody = fighter.HenchBody; 
							PrisonerSound = 0; 
							PrisonerType = 97;
							fighter.Delete();
					break;
				case 49:
							HenchmanItem archer = new HenchmanArcherItem();
							PrisonerName = archer.HenchName; 
							PrisonerTitle = archer.HenchTitle; 
							PrisonerBody = archer.HenchBody; 
							PrisonerSound = 0; 
							PrisonerType = 98;
							archer.Delete();
					break;
				case 50:
							HenchmanItem wizard = new HenchmanWizardItem();
							PrisonerName = wizard.HenchName; 
							PrisonerTitle = wizard.HenchTitle; 
							PrisonerBody = wizard.HenchBody; 
							PrisonerSound = 0; 
							PrisonerType = 97;
							wizard.Delete();
					break;
			}

			int reward = Utility.RandomMinMax( 10, 20 );
			int join = Utility.RandomMinMax( 50, 100 );

			if ( PrisonerType == 1 ){ PrisonerTitle = PrisonerTitle + " " + GetMeleeTitle(); PrisonerReward = (reward*100); PrisonerJoin = (join*100); }
			else if ( PrisonerType == 2 ){ PrisonerTitle = PrisonerTitle + " " + GetArcherTitle(); PrisonerReward = (reward*125); PrisonerJoin = (join*125); }
			else if ( PrisonerType == 3 ){ PrisonerTitle = PrisonerTitle + " " + GetMageTitle(); PrisonerReward = (reward*150); PrisonerJoin = (join*150); }
			else if ( PrisonerType == 97 ){ PrisonerReward = (reward*150); PrisonerJoin = (4000+(10*Utility.RandomMinMax( 10, 100 ))); }
			else if ( PrisonerType == 98 ){ PrisonerReward = (reward*150); PrisonerJoin = (5000+(10*Utility.RandomMinMax( 10, 100 ))); }
			else if ( PrisonerType == 99 ){ PrisonerReward = (reward*150); PrisonerJoin = (6000+(10*Utility.RandomMinMax( 10, 100 ))); }
		}

		public string GetMeleeTitle()
		{
			string title = "warrior";
			switch ( Utility.RandomMinMax( 0, 12 ) )
			{
				case 0: title = "fighter"; break;
				case 1: title = "knight"; break;
				case 2: title = "champion"; break;
				case 3: title = "warrior"; break;
				case 4: title = "soldier"; break;
				case 5: title = "vanquisher"; break;
				case 6: title = "battler"; break;
				case 7: title = "gladiator"; break;
				case 8: title = "mercenary"; break;
				case 9: title = "nomad"; break;
				case 10: title = "berserker"; break;
				case 11: title = "pit fighter"; break;
				case 12: title = "brute"; break;
			}
			return title;
		}

		public string GetArcherTitle()
		{
			string title = "archer";
			switch ( Utility.RandomMinMax( 0, 1 ) )
			{
				case 0: title = "bowman"; break;
				case 1: title = "archer"; break;
			}
			return title;
		}

		public string GetMageTitle()
		{
			string title = "wizard";
			switch ( Utility.RandomMinMax( 0, 4 ) )
			{
				case 0: title = "wizard"; break;
				case 1: title = "shaman"; break;
				case 2: title = "mage"; break;
				case 3: title = "conjurer"; break;
				case 4: title = "magician"; break;
			}
			return title;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( ( int) 0 ); // version
			writer.Write( PrisonerReward );
			writer.Write( PrisonerJoin );
			writer.Write( PrisonerType );
			writer.Write( PrisonerName );
			writer.Write( PrisonerTitle );
			writer.Write( PrisonerBody );
			writer.Write( PrisonerSound );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			PrisonerReward = reader.ReadInt();
			PrisonerJoin = reader.ReadInt();
			PrisonerType = reader.ReadInt();
			PrisonerName = reader.ReadString();
			PrisonerTitle = reader.ReadString();
			PrisonerBody = reader.ReadInt();
			PrisonerSound = reader.ReadInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			from.CloseGump( typeof( PrisonerGump ) );
			from.SendGump( new PrisonerGump( this, from ) );
		}

		private class PrisonerGump : Gump
		{
			private Prisoner m_Jail;

			public PrisonerGump( Prisoner jail, Mobile from ) : base( 50, 50 )
			{
				m_Jail = jail;

				string color = "#c2d5dc";
				from.SendSound( 0x0EC );

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				AddImage(0, 0, 7030, Server.Misc.PlayerSettings.GetGumpHue( from ));
				AddButton(368, 9, 4017, 4017, 0, GumpButtonType.Reply, 0);


				string FullName = m_Jail.PrisonerName + " " + m_Jail.PrisonerTitle;
					if ( m_Jail.PrisonerType == 97 ){ FullName = FullName + " (Warrior)"; }
					else if ( m_Jail.PrisonerType == 98 ){ FullName = FullName + " (Archer)"; }
					else if ( m_Jail.PrisonerType == 99 ){ FullName = FullName + " (Wizard)"; }

				string paragraph = "" + m_Jail.PrisonerName + " " + m_Jail.PrisonerTitle + " has been locked in this cage and is begging to be freed. You can choose to leave them to their fate, or they will give you " + m_Jail.PrisonerReward + " gold if you release them. Another choice you can make is to offer them " + m_Jail.PrisonerJoin + " gold to join you on your journey. If you decide to do this, they will be released from this cage and become your henchman. A henchman item will appear in your backpack. Continue reading on if you need an explanation on how henchman work.";

				paragraph = paragraph + "<br><br>Henchman are followers that can join you on adventures so you do not have to traverse the dangerous dungeons alone. These henchman use a similar system for tamed animals, with a few exceptions. First, you can heal your henchmen with your healing skill. Second, you cannot transfer an active henchman to another player. Third, you cannot stable your henchmen. Lastly, you cannot be bonded to your henchmen. Although you cannot transfer your henchman, you can give the 'henchman item' to another person where they will then have possession of the henchman. Along those lines, if someone else manages to get your 'henchman item' from you, the henchman is then theirs.<br><br>You must be in an area such as an inn, tavern, or home to call your henchman. Once you call them, they will take possession of the 'henchman item' and keep it until one of the following occur...they are killed, you release them, or they get annoyed with the lack of treasure being found. For every 5 gold you give them, they will travel with you for 1 minute. This equals to 300 gold per hour, where the maximum they will take from you is enough for 6 hours of adventuring. You can pay your henchman in a few different ways. You can give them many types of treasure like coins, gems, or rare items for payment. Rare items are those unique items you may find that you can give to merchants in towns for a high price. Each time you pay them, you will get a message indicating how many minutes they will be traveling with you. When they have about 5 minutes left, they will begin to express their annoyance for the lack of treasure. This is a warning to find some treasure quickly, or your henchman will leave. If your henchman does depart, the 'henchman item' will appear in your backpack. The next time you call upon your henchman, make sure you have something to give them so they will travel with you. A henchman always remembers how much treasure you have given them. This means if a henchman has about 4 hours left of travel, and you 'release' them, they will remember that they have 4 hours of travel when you call upon them again. Keep in mind that this 'adventuring time' does not count down when you are in an area like a tavern, home, inn, bank, or camping tent.<br><br>Each henchman will have a unique name and title. As mentioned earlier, you do not stable henchmen. You instead 'release' them and their 'henchman item' will appear in your backpack and you can call the henchman later. You can release henchman anywhere you are. If a henchman is slain, the 'henchman item' will appear in your backpack. The name of the 'henchman item' will indicate that the henchman is dead. You will have to seek out a healer and 'hire' them to resurrect your henchman. When you 'hire' a healer to do this, it will cost an amount of gold indicated on the item...and you must select the 'henchman item' when the targeting cursor comes up. The 'dead' indicator will vanish and you can then return to an area like an inn, tavern, bank, or home and call your henchman again.<br><br>If you ever mount a creature or magically enhance your travel speed, your henchman will increase their speed so they can keep up with you. Henchman are only as able of an adventurer as you are. Their skill level is an average value of your total skills. Their stats are a distribution of your total non-magically-enhanced stats. So basically, the better you are...the better your henchmen will be. These henchmen only help you in your battles. They do not pick locks or remove traps. That is up to you to manage. You can give them bandages and they will use them as they need them to cure their poison or heal their wounds. You can give them potions though and they will drink them...giving you an empty bottle back. The potions they can make use of are heal, cure, rejuvenate, refresh, and mana potions. You are only able to take two henchman with you at any one time.";


				AddHtml( 12, 12, 346, 20, @"<BODY><BASEFONT Color=" + color + ">PRISONER - " + FullName + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 12, 44, 382, 361, @"<BODY><BASEFONT Color=" + color + ">" + paragraph + "</BASEFONT></BODY>", (bool)false, (bool)true);

				AddButton(9, 417, 4005, 4005, 1, GumpButtonType.Reply, 0);
				AddButton(9, 450, 4008, 4008, 2, GumpButtonType.Reply, 0);
				AddButton(9, 483, 4020, 4020, 3, GumpButtonType.Reply, 0);
				AddHtml( 48, 417, 346, 20, @"<BODY><BASEFONT Color=" + color + ">Set Them Free & Earn " + m_Jail.PrisonerReward + " Gold</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 48, 450, 346, 20, @"<BODY><BASEFONT Color=" + color + ">Give Them " + m_Jail.PrisonerJoin + " Gold to Join You</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 48, 483, 346, 20, @"<BODY><BASEFONT Color=" + color + ">Leave Them to Their Fate</BASEFONT></BODY>", (bool)false, (bool)false);
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				Mobile from = sender.Mobile;
				from.SendSound( 0x0EC );

				if ( info.ButtonID == 1 )
				{
					from.AddToBackpack ( new Gold( m_Jail.PrisonerReward ) );
					from.SendMessage( "You free " + m_Jail.PrisonerName + " from their prison." );
					LoggingFunctions.LogStandard( from, "has freed " + m_Jail.PrisonerName + " " + m_Jail.PrisonerTitle + "." );

					CustomEventSink.InvokeCombatQuestCompleted( from, m_Jail.PrisonerReward );
					Titles.AwardFame( from, ((int)((m_Jail.PrisonerReward)/100)), true );
					if ( ((PlayerMobile)from).KarmaLocked == true ){ Titles.AwardKarma( from, -((int)((m_Jail.PrisonerReward)/100)), true ); }
					else { Titles.AwardKarma( from, ((int)((m_Jail.PrisonerReward)/100)), true ); }

					m_Jail.Delete();
				}
				else if ( info.ButtonID == 2 )
				{
					int gold = from.TotalGold;
					int join = m_Jail.PrisonerJoin;
					bool begging = false;

					if ( Server.Mobiles.BaseVendor.BeggingPose(from) > 0 ) // LET US SEE IF THEY ARE BEGGING
					{
						int cut = (int)(from.Skills[SkillName.Begging].Value * 25 );
							if ( cut > 3000 ){ cut = 3000; }
						join = join - cut;
						begging = true;
					}

					if ( gold >= join )
					{
						Container cont = from.Backpack;
						cont.ConsumeTotal( typeof( Gold ), join );

						if ( begging )
							from.SendMessage( "You beg " + m_Jail.PrisonerName + " to join you as a henchman for only " + join + " gold." );
						else
							from.SendMessage( "" + m_Jail.PrisonerName + " has joined you as a henchman." );

						if ( m_Jail.PrisonerType == 97 )
						{
							HenchmanFighterItem fighter = new HenchmanFighterItem();
							fighter.HenchName = m_Jail.PrisonerName; 
							fighter.HenchTitle = m_Jail.PrisonerTitle; 
							fighter.HenchBody = m_Jail.PrisonerBody; 
							from.AddToBackpack( fighter );
						}
						else if ( m_Jail.PrisonerType == 98 )
						{
							HenchmanArcherItem archer = new HenchmanArcherItem();
							archer.HenchName = m_Jail.PrisonerName; 
							archer.HenchTitle = m_Jail.PrisonerTitle; 
							archer.HenchBody = m_Jail.PrisonerBody; 
							from.AddToBackpack( archer );
						}
						else if ( m_Jail.PrisonerType == 99 )
						{
							HenchmanWizardItem wizard = new HenchmanWizardItem();
							wizard.HenchName = m_Jail.PrisonerName; 
							wizard.HenchTitle = m_Jail.PrisonerTitle; 
							wizard.HenchBody = m_Jail.PrisonerBody; 
							from.AddToBackpack( wizard );
						}
						else
						{
							HenchmanMonsterItem item = new HenchmanMonsterItem();

							item.HenchTimer = 300;
							item.HenchWeaponID = m_Jail.PrisonerType;
							item.HenchShieldID = m_Jail.PrisonerSound;
							item.HenchHelmID = 0;
							item.HenchArmorType = 0;
							item.HenchWeaponType = 0;
							item.HenchCloakColor = 0;
							item.HenchCloak = 0;
							item.HenchRobe = 0;
							item.HenchHatColor = 0;
							item.HenchGloves = 0;
							item.HenchSpeech = Utility.RandomDyedHue();
							item.HenchDead = 0;
							item.HenchBody = m_Jail.PrisonerBody;
							item.HenchHue = 0;
							item.HenchHair = 0;
							item.HenchHairHue = 0;
							item.HenchGearColor = 0;
							item.HenchName = m_Jail.PrisonerName;
							item.HenchTitle = m_Jail.PrisonerTitle;
							item.HenchBandages = 0;
							from.AddToBackpack( item );
						}

						m_Jail.Delete();
					}
					else
					{
						from.SendMessage( "You do not have enough gold to convince them to join you." );
					}
				}
				else if ( info.ButtonID == 3 )
				{
					switch ( Utility.RandomMinMax( 0, 4 ) )
					{
						case 0: from.Say("I will leave you to your fate, " + m_Jail.PrisonerName + "!"); break;
						case 1: from.Say("" + m_Jail.PrisonerName + ", stay here and rot!"); break;
						case 2: from.Say("" + m_Jail.PrisonerName + ", the world is better with you in here!"); break;
						case 3: from.Say("You are not the sort I wish to free, " + m_Jail.PrisonerName + "."); break;
						case 4: from.Say("You must be here for a reason, " + m_Jail.PrisonerName + "."); break;
					}
				}
			}
		}
	}
}