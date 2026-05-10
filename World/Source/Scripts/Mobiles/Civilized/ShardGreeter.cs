using Server.ContextMenus;
using System.Collections.Generic;
using Server.Misc;
using Server.Network;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Accounting;
using System;
using Server.Temptation;
using Server.Engines.Avatar;

namespace Server.Mobiles
{
    public class ShardGreeter : BasePerson
	{
		public override bool IsInvulnerable{ get{ return true; } }

		[Constructable]
		public ShardGreeter() : base( )
		{
			Direction = Direction.South;
			CantWalk = true;
			Female = true;

			SpeechHue = Utility.RandomTalkHue();
			Hue = 1009;
			NameHue = 0x92E;
			Body = 0x191;
			Name = NameList.RandomName( "female" );
			Title = "the gypsy";

			FancyDress dress = new FancyDress(0xAFE);
			dress.ItemID = 0x1F00;
			AddItem( dress );
			AddItem( new Sandals() );

			Utility.AssignRandomHair( this );
			HairHue = 0x92E;
			HairItemID = 8252;
			FacialHairItemID = 0;
		}

		public ShardGreeter(Serial serial) : base(serial)
		{
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list ) 
		{ 
			base.GetContextMenuEntries( from, list ); 
			list.Add( new ShardGreeterEntry( from, this ) ); 
		} 

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); 
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public class ShardGreeterEntry : ContextMenuEntry
		{
			private Mobile m_Mobile;
			private Mobile m_Giver;

			public ShardGreeterEntry( Mobile from, Mobile giver ) : base( 6146, 3 )
			{
				m_Mobile = from;
				m_Giver = giver;
			}

			public override void OnClick()
			{
				if( !( m_Mobile is PlayerMobile ) )
					return;

				PlayerMobile mobile = (PlayerMobile) m_Mobile;

				if ( m_Mobile.X == 3567 && m_Mobile.Y == 3404 )
				{
					m_Giver.PlaySound( 778 );
					mobile.CloseGump( typeof( GypsyTarotGump ) );
					mobile.CloseGump( typeof( WelcomeGump ) );
					mobile.CloseGump( typeof( RacePotions.RacePotionsGump ) );
					mobile.SendGump(new GypsyTarotGump( mobile, 0 ) );
				}
				else
				{
					m_Giver.Say( "Please, " + m_Mobile.Name + ". Take a seat and we will begin." );
				}
			}
		}
	}
}

namespace Server.Gumps
{
	public class MonsterGump : Gump
    {
        public MonsterGump( Mobile from ) : base( 50, 50 )
        {
			string color = "#efc99b";
			from.SendSound( 0x4A );

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			AddImage(0, 0, 7034, Server.Misc.PlayerSettings.GetGumpHue( from ));
			AddHtml( 12, 12, 425, 20, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.BaseRace.StartName( from.RaceID ) + " - " + from.Name + "</BASEFONT></BODY>", (bool)false, (bool)false);
			AddHtml( 12, 42, 509, 351, @"<BODY><BASEFONT Color=" + color + ">" + from.Profile + "</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(496, 9, 4017, 4017, 0, GumpButtonType.Reply, 0);
        }

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			from.SendSound( 0x4A );
		}
    }

	public class WelcomeGump : Gump
    {
        public WelcomeGump( Mobile from ) : base( 400, 50 )
        {
            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			AddImage(0, 0, 2610, Server.Misc.PlayerSettings.GetGumpHue( from ));

			int header = 11474;
			if ( MySettings.S_ServerName == "Adventurers of Akalabeth" ){ header = 11377; }
			AddImage(13, 12, header, 2126);

			AddHtml( 13, 58, 482, 312, @"<BODY><BASEFONT Color=#94C541>For you, the day was normal compared to any other. However, when the evening sun finally disappeared below the landscape, you retired to bed where the sleep felt restless and the dreams more vivid. You cannot remember the details of the dream, but you can recall being drawn from this world through a swirling portal. When you awoke, you found yourself here in this forest. Your night clothes are gone and you are now dressed in some medieval garb, wielding a light in your hand.<BR><BR>Through the darkness of the night, you see a campfire just ahead. A colorful tent is next to it with the welcoming glow of lanterns about. The sounds of the nearby stream provides a tranquility, and you can see a grizzly bear soundly sleeping next to the warmth of the fire. If you were to shrug off the worries of your current life, you feel like this would be the place to start anew. You decide to see who is camping here and to perhaps find out where you are.</BASEFONT></BODY>", (bool)false, (bool)false);
			
			AddButton(468, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);
        }

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			from.SendSound( 0x4A );
		}
    }

	public class GypsyTarotGump : Gump
	{
		private enum ActionButtonType
		{
			ShowFirstTarotCard = -1,
			Close = 0,
			Home = 99,
			EnterLandBase = 100,
		}

		private enum CreaturePage
		{
			FIRST_OPTION = The_Day,

			The_Day = 1,
			The_Night = 2, // Fugitive
			The_Light = 3,
			The_Dark = 4, // Fugitive

			LAST_OPTION = The_Dark,
		}

		private enum HumanPage
		{
			FIRST_OPTION = The_Emperor,

			The_Emperor = 1,
			The_Devil = 2,
			The_Hermit = 3,
			The_Tower = 4,
			The_Magician = 5,
			The_Fool = 6,
			Death = 7,
			The_Sun = 8,
			The_Hanged_Man = 9, // Fugitive
			The_Hierophant = 10,
			The_High_Priestess = 11,
			Strength = 12, // Savage
			The_Star = 13, // Warning: Alien must ALWAYS be the last option

			LAST_OPTION = The_Star,
		}

		private bool ShowLodorOptions( PlayerMobile from )
		{
			if ( from.Avatar.Active ) return PlayerSettings.GetDiscovered( from, Land.Lodoria );

			Account a = from.Account as Account;
			if ( a == null ) return false;

			for ( int i = 0; i < a.Length; ++i )
			{
				Mobile m = a[i];
				if ( m != null && PlayerSettings.GetDiscovered( m, Land.Lodoria ) ) return true;
			}

			return false;
		}

		private bool ShowSavageOption( PlayerMobile from )
		{
			if ( from.Avatar.Active ) return from.Avatar.UnlockSavageRace && PlayerSettings.GetDiscovered( from, Land.Savaged );

			Account a = from.Account as Account;
			if ( a == null ) return false;

			for ( int i = 0; i < a.Length; ++i )
			{
				Mobile m = a[i];
				if ( m != null && PlayerSettings.GetDiscovered( m, Land.Savaged ) ) return true;
			}

			return false;
		}

		/// <summary>
		/// Skips illegal pages and cycles back to the Gypsy page
		/// </summary>
		public int pageShow( PlayerMobile from, int page, bool forward )
		{
			bool hasVisitedLodor = ShowLodorOptions( from );
			bool canBeFugitive = from.RaceID == 0 || !BaseRace.IsGood( from );
			if ( canBeFugitive && from.Avatar.Active ) canBeFugitive = from.Avatar.UnlockFugitiveMode;

			if ( from.RaceID > 0 )
			{
				if ( forward )
				{
					page++;

					if (page == (int)CreaturePage.The_Night) if ( !canBeFugitive ) { page++; }
					if (page == (int)CreaturePage.The_Light) if ( !hasVisitedLodor ) { page++; }
					if (page == (int)CreaturePage.The_Dark) if ( !canBeFugitive || !hasVisitedLodor ) { page++; }

					if ( page > (int)CreaturePage.LAST_OPTION ){ page = pageShow(from, (int)CreaturePage.FIRST_OPTION - 1, forward); }
				}
				else
				{
					page--;

					if (page == (int)CreaturePage.The_Dark) if ( !canBeFugitive || !hasVisitedLodor ) { page--; }
					if (page == (int)CreaturePage.The_Light) if ( !hasVisitedLodor ) { page--; }
					if (page == (int)CreaturePage.The_Night) if ( !canBeFugitive ) { page--; }

					if ( page < (int)CreaturePage.FIRST_OPTION ){ page = pageShow(from, (int)CreaturePage.LAST_OPTION + 1, forward); }
				}
			}
			else
			{
				var disallowAlien = !MySettings.S_AllowAlienChoice && from.RaceID == 0;
				bool canBeSavage = ShowSavageOption( from );

				if ( forward )
				{
					page++;

					if (page == (int)HumanPage.The_Hanged_Man) if ( !canBeFugitive ) { page++; }
					if (page == (int)HumanPage.The_Hierophant) if ( !hasVisitedLodor ) { page++; }
					if (page == (int)HumanPage.The_High_Priestess) if ( !hasVisitedLodor ) { page++; }
					if (page == (int)HumanPage.Strength) if ( !canBeSavage ) { page++; }
					if (page == (int)HumanPage.The_Star) if ( disallowAlien ) { page++; }

					if ( page > (int)HumanPage.LAST_OPTION ){ page = pageShow(from, (int)HumanPage.FIRST_OPTION - 1, forward); }
				}
				else
				{
					page--;

					if (page == (int)HumanPage.The_Star) if ( disallowAlien ) { page--; }
					if (page == (int)HumanPage.Strength) if ( !canBeSavage ) { page--; }
					if (page == (int)HumanPage.The_High_Priestess) if ( !hasVisitedLodor ) { page--; }
					if (page == (int)HumanPage.The_Hierophant) if ( !hasVisitedLodor ) { page--; }
					if (page == (int)HumanPage.The_Hanged_Man) if ( !canBeFugitive ) { page--; }

					if ( page < (int)HumanPage.FIRST_OPTION ){ page = pageShow(from, (int)HumanPage.LAST_OPTION + 1, forward); }
				}
			}

			return page;
		}

		public static string GypsySpeech( Mobile from )
		{
			string monst = "";
			string races = "Lastly, this realm ";
			if ( MyServerSettings.MonstersAllowed() )
			{
				monst = " There is a shelf over there with interesting potions you may want. So if you want one, drink it now and return here for your tarot card reading.";
				races = "You may not actually be of human descent. You might actually be an ogre, troll, or satyr. There are many creatures you that you can actually be. If you want to explore these ideas, look through my potion shelf behind me. There you will find various potions of alteration, that can change your life. If you choose one of these creatures to be, consider changing your name to better represent the creature you chose to play. This leads me to my final words of advice. This realm ";
			}

			return "Greetings, " + from.Name + "...you are about to enter one of the lands in the " + MySettings.S_ServerName + ". Not too long ago the Stranger arrived in Sosaria and foiled the evil plans of Exodus. Castle Exodus lies in ruins and Sosaria is once again trying to rebuild in peace. Many vile monsters still roam the land, however, but hardy adventurers have bravely sought to rid us of these terrors. To begin your journey, simply choose your fate from my deck of tarot cards (begin by pressing the top-right button). Once you look through the deck (pressing the arrow buttons) you can draw a card of your choice (by pressing the OK button on the top-right)." + monst + "<br><br>Now let me tell you some things of the world that fate has brought you to. Traveling the lands can be dangerous as other adventurers may decide to kill you for your gold or property. The taverns, inns, and banks are safe from such threats, but there are also many guards in the settlements to keep the peace. They have been known to quickly dispatch with murderers and criminals. There are many merchants throughout the settlements. They are not able to sell or buy everything they normally deal in, as their choices of what they buy and sell change from day to day.<br><br>There are secrets to be learned and magic items to be found in the many dungeons. Each settlement in Sosaria is somewhat safe in the surrounding land so hunting for food or skins should be relatively safe. I cannot say such things of other lands. There is also a minor dungeon near each settlement of Sosaria, if you wish to begin traversing the dangers below before you are fully prepared. Be warned that the vile creatures are not all that you must face. There are many deadly traps in the rooms and halls of these places that could kill you quicker than the monster you may be fleeing from.<br><br>Prepare to go forth and make your life your own. Become the finest craftsman in the land, a wealthy owner of lands and castles, the mightiest warrior, or even the most powerful wizard. The choice is yours.<br><br>This world can be travelled alone or with friends, where one could have great adventures. Like I stated already, your chosen course in life is whatever you want to do. You may be a mighty warrior or powerful wizard. You may simply start a potion shop near a large city. You may be a master of beasts or a mystical bard. This is a world where great wealth and artefacts can be obtained from the many dungeons throughout the land. You may be slain by a creature, die from hunger, get lost in the dark, or stumble onto a deadly trap. You may find powerful relics and enough gold to build your own castle.<br><br>" + races + "is best served if you have a name that is commensurate with a this rich fantasy world. You have one final chance to change your name if you need to, by simply using my journal on the table behind me. You cannot have a name that someone else already has, so it must be unique. If you want to change your name, proceed to the table where I keep my journal. Once your name is changed, return here for your tarot card reading.";
		}

		public GypsyTarotGump( PlayerMobile from, int page ): base( 50, 50 )
		{
            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);
			AddImage(0, 0, 2611, Server.Misc.PlayerSettings.GetGumpHue( from ));
			AddImage(10, 8, 1102);

			if ( page > 0 && page < 50 )
			{
				int prev = pageShow( from, page, false );
				int next = pageShow( from, page, true );

				AddImage(640, 8, cardGraphic( page, from.RaceID ));

				AddItem(269, 349, 4775);
				AddItem(586, 349, 4776);
				AddButton(317, 375, 4014, 4014, prev, GumpButtonType.Reply, 0);
				AddButton(552, 375, 4005, 4005, next, GumpButtonType.Reply, 0);

				AddHtml( 269, 12, 240, 20, @"<BODY><BASEFONT Color=#DEC6DE>" + cardText( page, 1, from.RaceID ) + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 271, 47, 356, 297, @"<BODY><BASEFONT Color=#DEC6DE>" + cardText( page, 2, from.RaceID ) + "<BR><BR>" + cardText( page, 3, from.RaceID ) + "</BASEFONT></BODY>", (bool)false, scrollBar( page, from.RaceID ));

				AddItem(566, 12, 4777);
				AddItem(580, 26, 4779);
				AddButton(599, 11, 4023, 4023, (int)ActionButtonType.EnterLandBase + page, GumpButtonType.Reply, 0);
			}
			else
			{
				int header = 11473;
				if ( MySettings.S_ServerName == "Adventurers of Akalabeth" ){ header = 11376; }

				AddImage(271, 13, header, 2813);

				AddHtml( 278, 73, 604, 320, @"<BODY><BASEFONT Color=#DEC6DE>" + GypsySpeech( from ) + "</BASEFONT></BODY>", (bool)false, (bool)true);
				AddButton(819, 14, 4011, 4011, (int)ActionButtonType.ShowFirstTarotCard, GumpButtonType.Reply, 0);
				AddItem(851, 11, 4773);
			}
		}

		public int cardGraphic( int page, int creature )
		{
			if ( creature > 0 )
			{
				switch ( (CreaturePage)page )
				{
					case CreaturePage.The_Day: return 1340;
					case CreaturePage.The_Night: return 1341;
					case CreaturePage.The_Light: return 1342;
					case CreaturePage.The_Dark: return 1343;
				}
			}
			else 
			{
				switch ( (HumanPage)page )
				{
					case HumanPage.The_Emperor: return 1106;
					case HumanPage.The_Devil: return 1105;
					case HumanPage.The_Hermit: return 1110;
					case HumanPage.The_Tower: return 1122;
					case HumanPage.The_Magician: return 1116;
					case HumanPage.The_Fool: return 1108;
					case HumanPage.Death: return 1104;
					case HumanPage.The_Sun: return 1120;
					case HumanPage.The_Hanged_Man: return 1109;
					case HumanPage.The_Hierophant: return 1111;
					case HumanPage.The_High_Priestess: return 1112;
					case HumanPage.Strength: return 1119;
					case HumanPage.The_Star: return 1118;
				}
			}

			return 0;
		}

		public bool scrollBar( int page, int creature )
		{
			if ( creature > 0 )
			{
				if ( Server.Items.BaseRace.GetUndead( creature ) ) return true;

				switch ( (CreaturePage)page )
				{
					case CreaturePage.The_Day: return false;
					case CreaturePage.The_Night: return true;
					case CreaturePage.The_Light: return false;
					case CreaturePage.The_Dark: return true;
				}
			}
			else 
			{
				switch ( (HumanPage)page )
				{
					case HumanPage.The_Emperor: return false;
					case HumanPage.The_Devil: return false;
					case HumanPage.The_Hermit: return false;
					case HumanPage.The_Tower: return false;
					case HumanPage.The_Magician: return false;
					case HumanPage.The_Fool: return false;
					case HumanPage.Death: return false;
					case HumanPage.The_Sun: return false;
					case HumanPage.The_Hanged_Man: return true;
					case HumanPage.The_Hierophant: return true;
					case HumanPage.The_High_Priestess: return true;
					case HumanPage.Strength: return true;
					case HumanPage.The_Star: return true;
				}
			}

			return false;
		}

		private static string GetStartSkillCount( CharacterType type )
		{
			int skills = MyServerSettings.DEFAULT_SKILL_COUNT + MyServerSettings.SkillBoostCount() + MyServerSettings.StartTypeBonusSkillCount( type );

			return skills.ToString();
		}

		public string cardText( int page, int section, int creature )
		{
			string card = "";
			string town = "";
			string text = "";
			string lodor = "Most adventurers are born within the Land of Sosaria, only hearing tales and legends of other lands far away. One of these lands is the elven world of Lodoria. This world is a bit larger than Sosaria and the dungeons are somewhat more difficult. What Lodoria does have is familiar locations that veteran adventurers fondly remember. Dungeons such as Shame, Destard, and Wrong can be found throughout. There are many villages and cities and they are all inhabited by the good elven people. The much more vile elven folk, the drow, seek to destroy those that embrace the light and attempt to supress their rule beneath the surface of the world. If you wish to begin your journey in Lodoria, then you will then be a human that grew up in this strange land with no ties of those from Sosaria.";

			string fate = "If you choose this fate, ";
			switch ( Utility.RandomMinMax(0,8) )
			{
				case 1: fate = "If you choose this card, "; break;
				case 2: fate = "If you take this card, "; break;
				case 3: fate = "If this is the card you want, "; break;
				case 4: fate = "If this card is yours, "; break;
				case 5: fate = "If this fate is meant for you, "; break;
				case 6: fate = "If you draw this card, "; break;
				case 7: fate = "If you choose this path, "; break;
				case 8: fate = "If you take this road, "; break;
			}

			string begin = "you will begin your journey";
			switch ( Utility.RandomMinMax(0,8) )
			{
				case 1: begin = "you will start your life"; break;
				case 2: begin = "you will enter the world"; break;
				case 3: begin = "you will be a citizen"; break;
				case 4: begin = "you will have a new life"; break;
				case 5: begin = "you may start a new life"; break;
				case 6: begin = "you may have a new home"; break;
				case 7: begin = "you may begin your journey"; break;
				case 8: begin = "you may begin a new life"; break;
			}

			fate = fate + begin;

			if ( creature > 0 )
			{
				town = Server.Items.BaseRace.StartName( creature );
				string undead = "";
				if ( Server.Items.BaseRace.GetUndead( creature ) ){ undead = " Although you do not remember your past life, you feel different from the other undead. You seem to have retained your soul, which will surely be noticed by other undead. This means they will likely attack you as they do the living."; }

				if ( Server.Items.BaseRace.BloodDrinker( creature ) ){ undead = undead + " Having a soul, however, means you can safely walk the land during the daylight."; }

				switch ( (CreaturePage)page )
				{
					case CreaturePage.The_Day: card = "THE DAY"; text = fate + " " + Server.Items.BaseRace.StartSentence( town ) + " of Sosaria." + undead + " This world has suffered three ages of darkness, where a stranger came from a far off land to bring light to each of these events. After Mondain, Minax, and Exodus were thwarted in their evil plans, Sosaria has reached a level of peace and prosperity. Although most want to lead humble lives as simple villagers, there are some that seek to explore the old dungeons, tombs, ruins, and crypts of the world. This path will lead you toward joining the ways of civilized man, but doing so will surely have your kindred banish you from their presence. It matters little to you, as you prefer to seek fame and fortune in this world rid of the most powerful evils it has ever seen."; break;

					case CreaturePage.The_Night: card = "THE NIGHT"; text = fate + " " + Server.Items.BaseRace.StartSentence( town ) + " of Sosaria." + undead + " This fate in Sosaria has a more challenging life, where you perhaps left others of your kind, but have decided to embrace your monstrous ways and seek power for yourself. You will be able to become grandmaster in " + GetStartSkillCount( CharacterType.Fugitive ) + " different skills instead of the " + GetStartSkillCount( CharacterType.Default ) + " normally accomplished. Tributes for resurrection will cost double the amount, perhaps forcing you to resurrect with penalties. You will not be allowed to enter any civilized areas, unless you perhaps find a way to disguise yourself. The exceptions are some public areas like inns, taverns, and banks. Guards will attack you on sight, merchants will attempt to chase you away, and you will not be able to join any local guilds except for the Assassin, Thief, and Black Magic guilds. The reason for this is that you are viewed as a murderous beast. Everything you need can be found throughout the world, however, so you can set forth on your journey."; break;

					case CreaturePage.The_Light: card = "THE LIGHT"; text = fate + " " + Server.Items.BaseRace.StartSentence( town ) + " of Lodoria." + undead + " This world was once ruled by dwarves, but now their cities lie in ruins and the elves have risen toward being the major civilized race of the land. Driving the drow back to their deep underdark lairs, many seeks to explore this world. Although most want to lead humble lives as simple villagers, there are some that seek to explore the old dungeons, tombs, ruins, and crypts of the world. This path will lead you toward joining the ways of civilization within the land of elves. Where you may seek glory and riches beyond your wildest dreams."; break;

					case CreaturePage.The_Dark: card = "THE DARK"; text = fate + " " + Server.Items.BaseRace.StartSentence( town ) + " of Lodoria." + undead + " This fate in Lodoria has a more challenging life, where you perhaps left others of your kind, but have decided to embrace your monstrous ways and seek power for yourself. You will be able to become grandmaster in " + GetStartSkillCount( CharacterType.Fugitive ) + " different skills instead of the " + GetStartSkillCount( CharacterType.Default ) + " normally accomplished. Tributes for resurrection will cost double the amount, perhaps forcing you to resurrect with penalties. You will not be allowed to enter any civilized areas, unless you perhaps find a way to disguise yourself. The exceptions are some public areas like inns, taverns, and banks. Guards will attack you on sight, merchants will attempt to chase you away, and you will not be able to join any local guilds except for the Assassin, Thief, and Black Magic guilds. The reason for this is that you are viewed as a murderous beast. Everything you need can be found throughout the world, however, so you can set forth on your journey."; break;
				}
			}
			else
			{
				switch ( (HumanPage)page )
				{
					case HumanPage.The_Emperor: card = "THE EMPEROR"; town = "The City of Britain"; text = fate + " in the capital city of Sosaria and the home of Lord British. Lord British's magnificent castle is situated at the northern part of the city, overlooking Britanny Bay. This tall building is the greatest architectural structure of the new age. Loyal subjects pay homage to His Majesty, and renew fealty whenever they are in the vicinity of his castle. Rumors in taverns speak of a dark secret below the castle, so dark that not even the citizens can see it. There are farms all around, as well as cemeteries for the citizens and another for the British Royal Family. Some have been seen going into the British tomb, late at night."; break;

					case HumanPage.The_Devil: card = "THE DEVIL"; town = "The Town of Devil Guard"; text = fate + " in a town totally enclosed by the Great Mountains during the Third Age of Darkness, and was only reachable by the magical gate. After the destruction of Exodus, a cavernous tunnel had torn through the mountain, providing an alternate route. Ancient legends tell of a castle that fell from the sky, crashing into the mountains and creating the valley in which Devil Guard was eventually built. Tales are told that the town was created and settled by those from the sky castle, and they named it because they were protecting others from the daemons long ago."; break;

					case HumanPage.The_Hermit: card = "THE HERMIT"; town = "The Village of Grey"; text = fate + " in this village where the inhabitants, during the Third Age of Darkness, gave several clues to the Stranger that defeated Exodus. It was even rumored that they sold ships that could fly to the stars, but none who remain know how to create such things. Legends say the Stranger flew to the sky and altered time and reality, causing a castle to fall backwards in time and crash into the land of ancient Sosaria. Now the village is often the home of those that enjoy solitude. There are no mountains to mine, but some have dug beneath the forest floor to obtain ore. The cemetery is rumored to have a secret that necromancers whisper in hush tones."; break;

					case HumanPage.The_Tower: card = "THE TOWER"; town = "The City of Montor"; text = fate + " in a vast city, where courage is especially upheld, having all the shops needed for everyone. The inhabitants of the Montors knew a lot about the mystical Four Cards that the Stranger needed to defeat Exodus, as well as tales of the lost shrines of Ambrosia. Montor is the most visited city, and also the largest in Sosaria due the trade from ships. There is a small mine to the east, as well as a tower to the northeast. This tower is said to be home of a vile lich with a magic mirror that traverses dimensions, but those rumors are often told with a tankard of ale."; break;

					case HumanPage.The_Magician: card = "THE MAGICIAN"; town = "The Town of Moon"; text = fate + " in the town where, during the Third Age of Darkness, was a city full of mages. They were, however, the corrupt and dishonest sort. Erstam also lived in the city, conducting his experiments for immortality. When Lord British chased the corrupt mages out of town after the destruction of Exodus, Erstam and the others decided to go to the Serpent Island, where no one could control them."; break;

					case HumanPage.The_Fool: card = "THE FOOL"; town = "The Town of Mountain Crest"; text = fate + " on some small islands in Sosaria, that has a harsh wintery landscape that others believe is foolish to inhabit. Along with this town, there are also settlements to the west and east. There are various caverns and dungeons within the mountains, and an unusual tower built by a wizard long ago. This place is one of the more difficult areas to live, but a snowy region may be your fate if you choose it."; break;

					case HumanPage.Death: card = "DEATH"; town = "The Undercity of Umbra"; text = fate + " in a place many people do not know of, as it was built as a haven for those that practice the necrotic arts. Deep within the mountains, just southeast of Britain, the dark halls and caverns have a spooky feel but the necromancers do provide themselves with a shoppes to provided much needed items. The cavern outside the city is one of the highest ever seen. Some say high enough to even build a castle away from the light of the sun. A death knight's tomb was also built nearby, and the Fires of Hell is but a hike away."; break;

					case HumanPage.The_Sun: card = "THE SUN"; town = "The Village of Yew"; text = fate + " in a valley of thick forest, just west of Britain and east of Moon, where the sun grows the largest trees in Sosaria. Yew is one of the land's major trading of wood. During the Third Age of Darkness, the Stranger visited Yew and learned the secrets of the Great Earth Serpent. This allowed the Stranger to free the serpent that was blocking their ship from reaching the Castle of Exodus on the Isle of Fire. Some say that freeing the serpent has caused an imbalance in the cosmos, but that could be drunken wizards telling tales. You can mine in a nearby cave, but miners discovered something on the southern side of the mountain range that they dare not enter."; break;

					case HumanPage.The_Hanged_Man: card = "THE HANGED MAN"; town = "The Britain Dungeons"; text = "You may choose a fate in this world that has a more challenging life, where you are a fugitive from justice. If you choose this path, you will be able to become grandmaster in " + GetStartSkillCount( CharacterType.Fugitive ) + " different skills instead of the " + GetStartSkillCount( CharacterType.Default ) + " normally accomplished. This is due to you relying on yourself to survive. Tributes for resurrection will cost double the amount, perhaps forcing you to resurrect with penalties. You will not be allowed to enter any civilized areas, unless you perhaps find a way to disguise yourself. The exceptions are some public areas like inns, taverns, and banks. Guards will attack you on sight, merchants will attempt to chase you away, and you will not be able to join any local guilds except for the Assassin, Thief, and Black Magic guilds. The reason for this is that you are wanted for murder. You may have actually committed the act, or you could have simply been framed. The murder was against a very powerful figure, so the many lands will never forgive the deed. Whether truth or falsehood, that is up to you to tell. Do with your life what you will. You can live a life of criminal pursuits, or you can destroy the evil that lurks in the darkest places of the land. If you wish to choose such a life, you will be on your own, and you must first escape from your prison cell. From there you are best to head for Stonewall to the northwest, but you may go where you like. Everything you need can be found throughout the world."; break;

					case HumanPage.The_Hierophant: card = "THE HIEROPHANT"; town = "The City of Lodoria"; text = lodor + " The city is the capital of Lodor, and it has every merchant you may need. The Castle of Knowledge lies on the high mountain on the western side, where scholars learn the ways of the world. It has a mine to the north and a cemetery in the south valley. The continent is large and adventurers tell tales of dungeons like Shame, Despise, and a cavern of lizardmen. Another small settlement lies to the northwest. Do you choose this fate?"; break;

					case HumanPage.The_High_Priestess: card = "THE HIGH PRIESTESS"; town = "The City of Elidor"; text = lodor + " The city is located on the second largest continent, diverse with both a forest covered south and a wintery north. The High Priestess of Elidor built the famous Hall of Illusions, where many of her subject practice prismatic magic. There are other settlements such as Springvale to the east and Glacial Hills to the north. Drunken adventurers often speak of riches from Wrong, Deceit, and the Frozen Hells. Do you wish to draw this card?"; break;

					case HumanPage.Strength: card = "STRENGTH"; town = "The Savaged Empire"; text = "You may choose a barbaric way of life to begin your journey, and it is not for the weak but those bestowed with strength. If you choose this path, you will be able to become grandmaster in " + GetStartSkillCount( CharacterType.Savage ) + " different skills instead of the " + GetStartSkillCount( CharacterType.Default ) + " normally accomplished. This is due to you relying on yourself to survive in an untamed land. Your adventure will begin as a barbarian in the Savaged Empire, which is one of the most difficult lands in the realms. It is filled with many dangerous animals and colossal dinosaurs. There are no safe places to hunt for food, which also means practicing your combat skills is equally dangerous. You will, however, begin with some leather armor that will help you surive the dangers away from the settlements. You will also begin with a talisman that will aid you in camping and cooking, so you can live off of the land better. Additional gold, food, and bandages will be provided as well as a steel dagger and a durable camping tent. Any dungeons you dare enter will be more deadly than those in Sosaria, so take some great consideration before deciding this path. Your journey will then begin in the Village of Kurak, where the outskirts have many things to hunt but also many dangers you may need to flee from. There is a cave to the north where you can mine for precious ores as well."; break;

					case HumanPage.The_Star: card = "THE STAR"; town = "The Shuttle Crash Site"; text = "All the doctor knew of you as a patient is entered into your medical record. You were near death, but placing you in the stasis chamber seemed to have performed the healing process. Your scans showed an incredible head trauma, so you will awake from your coma with no memories of what or who you were (you begin with no skills). With the space station plummeting to Sosaria, due to the Stranger draining the fuel reserves, the doctor decided to place your stasis chamber onto their last medical shuttle craft. They set it on auto-pilot and hoped for the best. It landed safely on Sosaria where you could continue your life on this primitive world. You may have an advantage as you are from a more advanced race of beings, so you have the ability to remember and learn more things (can grandmaster " + GetStartSkillCount( CharacterType.Alien ) + " different skills).<br><br>Because of your advanced knowledge of logic and science, however, some things learned about Sosaria is that they have elements you cannot fully understand. Magical resurrection, and the concept of deities, are things you cannot comprehend (costs 3 times as much gold to resurrect at a shrine or healer). The system shock from any such resurrection would surely take its toll (paying full tribute still causes a 10% loss in fame and karma, and a 5% loss in skills and attributes) which could prove to be devastating (paying no tribute at all would cause a 20% loss in fame and karma, and a 10% loss in skills and attributes).<br><br>Although you will be able to learn some of the skills that are classified as magic or divine, you will surely justify it with science. Because of your lack of superstition, unlike the inhabitants of this world, you don’t believe in the concept of luck (you will never benefit from luck). You do not have any of Sosaria's currency to barter with (you begin with no gold), and because you feel you are more advanced, you will probably not get along with the guildmasters of the crude trades they practice (guild membership costs 4 times as much as normal).<br><br>If you choose this fate, then you will appear at your crashed shuttle craft where your adventure begins. You can use the nearby computer terminal to change your skin and hair tones if you want an appearance that is slightly different than human, due to your alien heritage.<br><br>When you awake, you will have no memory of who you were. You will find yourself near the shuttle that crashed on top of the mountain. The computer system instructed you on how to setup a power source from the remaining fuel, and it appeared that an alien creature latched onto the shuttle and died in the crash. You have been using this as a source of food and have survived a few days from it. Now your supplies are running out, your canteen is empty, and all you have is a knife. You will have to venture out if you plan to survive."; break;
				}
			}

			if ( section == 1 ){ return card; }
			else if ( section == 2 ){ return town; }
			return text;
		}

		private void TryEnterLand(int page, PlayerMobile m)
		{
			m.CloseGump( typeof( GypsyTarotGump ) );
			m.CloseGump( typeof( WelcomeGump ) );
			m.CloseGump( typeof( RacePotions.RacePotionsGump ) );
			m.CloseGump( typeof( TemptationGump ) );
			
			ConfirmationGump.PromptIfFalse(
				m,
				m.Avatar.Active && m.Avatar.SelectedTemplate == AvatarStarterTemplates.None,
				() => EnterLand( page, m ),
				onConfirmed => new ConfirmationGump(m, "No template selected", "You have not selected a template yet. Are you sure you want to enter the land?", onConfirmed)
			);
		}

		public void EnterLand( int page, Mobile m )
		{
			Point3D loc = new Point3D(2999, 1030, 0);
			Map map = Map.Sosaria;

			if ( m.RaceID > 0 )
			{
				string start = Server.Items.BaseRace.StartArea( m.RaceID );
				string world = "the Land of Sosaria";

				if ( start == "cave" ){ 		loc = new Point3D(497, 4066, 0); }
				else if ( start == "ice" ){ 	loc = new Point3D(625, 3224, 0); }
				else if ( start == "pits" ){ 	loc = new Point3D(180, 4075, 0); }
				else if ( start == "sand" ){ 	loc = new Point3D(91, 3244, 0); }
				else if ( start == "sea" ){ 	loc = new Point3D(27, 4077, 0); }
				else if ( start == "sky" ){ 	loc = new Point3D(289, 3222, 20); }
				else if ( start == "swamp" ){ 	loc = new Point3D(92, 3978, 0); }
				else if ( start == "tomb" ){ 	loc = new Point3D(362, 3966, 0); }
				else if ( start == "water" ){ 	loc = new Point3D(27, 4077, 0); }
				else if ( start == "woods" ){ 	loc = new Point3D(357, 4057, 0); }

				bool keepFood = true;
				bool keepDrink = true;
				if ( Server.Items.BaseRace.NoFoodOrDrink( m.RaceID ) )
				{
					// NO NEED TO CREATE FOOD OR DRINK
					keepFood = false;
					keepDrink = false;
				}
				else if ( Server.Items.BaseRace.NoFood( m.RaceID ) )
				{
					keepFood = false;
				}
				else if ( Server.Items.BaseRace.BloodDrinker( m.RaceID ) )
				{
					keepFood = false;
					keepDrink = false;
					Item blood = new BloodyDrink();
					blood.Amount = 10;
					m.AddToBackpack( blood );
				}
				else if ( Server.Items.BaseRace.BrainEater( m.RaceID ) )
				{
					keepFood = false;
					keepDrink = false;
					Item blood = new FreshBrain();
					blood.Amount = 10;
					m.AddToBackpack( blood );
				}

				List<Item> allItems = new List<Item>();
				m.Backpack.RecurseItems( allItems );

				foreach ( Item item in allItems )
				{
					if (item is Food && !keepFood) // Delete Food
						item.Delete();
					else if (item is BaseBeverage && !keepDrink) // Delete Drink
						item.Delete();
					else if (item is BaseEquipableLight && Server.Items.BaseRace.NightEyes( m.RaceID ) && (item is BaseTrinket == false)) // Delete default light sources
						item.Delete();
				}

				if ( page == (int)CreaturePage.The_Day )
				{
					PlayerSettings.SetDiscovered( m, "the Land of Sosaria", true );
				}
				else if ( page == (int)CreaturePage.The_Night )
				{
					PlayerSettings.SetDiscovered( m, "the Land of Sosaria", true );
					PlayerSettings.SetWantedState( (PlayerMobile) m );
				}
				else if ( page == (int)CreaturePage.The_Light )
				{
					PlayerSettings.SetDiscovered( m, "the Land of Lodoria", true );
					world = "the Land of Lodoria";
				}
				else if ( page == (int)CreaturePage.The_Dark )
				{
					PlayerSettings.SetDiscovered( m, "the Land of Lodoria", true );
					world = "the Land of Lodoria";
					PlayerSettings.SetWantedState( (PlayerMobile) m );
				}
				m.Profile = Server.Items.BaseRace.BeginStory( m, world );

				if ( world == "the Land of Sosaria" )
					m.RaceHomeLand = 1;

				else if ( world == "the Land of Lodoria" )
					m.RaceHomeLand = 2;
			}
			else
			{
				switch ( (HumanPage)page )
				{
					case HumanPage.The_Emperor: loc = new Point3D(2999, 1030, 0); map = Map.Sosaria; break;
					case HumanPage.The_Devil: loc = new Point3D(1617, 1502, 2); map = Map.Sosaria; break;
					case HumanPage.The_Hermit: loc = new Point3D(851, 2062, 1); map = Map.Sosaria; break;
					case HumanPage.The_Tower: loc = new Point3D(3220, 2606, 1); map = Map.Sosaria; break;
					case HumanPage.The_Magician: loc = new Point3D(806, 710, 5); map = Map.Sosaria; break;
					case HumanPage.The_Fool: loc = new Point3D(4546, 1267, 2); map = Map.Sosaria; break;
					case HumanPage.Death:
					{
						loc = new Point3D(2666, 3325, 0); map = Map.Sosaria;
						MorphingTime.ColorOnlyClothes( m, 0, 1 );
						break;
					}
					case HumanPage.The_Sun: loc = new Point3D(2460, 893, 7); map = Map.Sosaria; break;
					case HumanPage.The_Hanged_Man:
					{
						loc = new Point3D(4104, 3232, 0); map = Map.Sosaria;
						PlayerSettings.SetWanted( m );
						break;
					}
					case HumanPage.The_Hierophant: loc = new Point3D(2111, 2187, 0); map = Map.Lodor; break;
					case HumanPage.The_High_Priestess: loc = new Point3D(2930, 1327, 0); map = Map.Lodor; break;
					case HumanPage.Strength:
					{
						loc = new Point3D(251, 1949, -28); map = Map.SavagedEmpire;
						PlayerSettings.SetSavage( m );
						break;
					}
					case HumanPage.The_Star:
					{
						loc = new Point3D(4109, 3775, 2); map = Map.Sosaria; 
						PlayerSettings.SetSpaceMan( m );
						break;
					}
				}

				if ( page == (int)HumanPage.The_Hierophant || page == (int)HumanPage.The_High_Priestess )
					PlayerSettings.SetDiscovered( m, "the Land of Lodoria", true );
				else if ( page == (int)HumanPage.Strength )
					PlayerSettings.SetDiscovered( m, "the Savaged Empire", true );
				else
					PlayerSettings.SetDiscovered( m, "the Land of Sosaria", true );
			}

			var player = (PlayerMobile)m;

			if (player.Avatar.Active)
			{
				TemptationEngine.Instance.ReplaceContext( player, new Temptation.PlayerContext(player.Temptations) { HasAggressiveNerfs = true } );
				TemptationEngine.Instance.ApplyContext( player, player.Temptations );

				Engines.Avatar.AvatarEngine.Instance.ApplyContext( player, player.Avatar );

				player.NetState.BlockAllPackets = true;
				CharacterCreation.InitializeBackpack(player);

				var template = player.Avatar.SelectedTemplate;
				if ( template > AvatarStarterTemplates.DEFAULT_START && template < AvatarStarterTemplates.DEFAULT_END )
				{
					var profession = (StarterProfessions)template;
					var skills = CharacterCreation.GetTemplateSkills( profession );
					CharacterCreation.AddSkillBasedItems( player, skills );
				}

				player.NetState.BlockAllPackets = false;
			}
			else
			{
				TemptationEngine.Instance.ApplyContext( player, player.Temptations );
			}

			CustomEventSink.InvokeBeginJourney(new BeginJourneyArgs(player));

			m.MoveToWorld( loc, map );
			Effects.SendLocationParticles( EffectItem.Create( m.Location, m.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 0, 0, 5024, 0 );
			m.SendSound( 0x65C );
			m.SendMessage( "The card vanishes from your hand as you magically appear elsewhere." );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			PlayerMobile from = state.Mobile as PlayerMobile;
			if (from == null) return;

			from.CloseGump( typeof( GypsyTarotGump ) );
			from.CloseGump( typeof( WelcomeGump ) );
			from.CloseGump( typeof( RacePotions.RacePotionsGump ) );
			if ( info.ButtonID == (int)ActionButtonType.Close ) return;

			if ( info.ButtonID == (int)ActionButtonType.ShowFirstTarotCard )
			{
				from.SendGump( new GypsyTarotGump( from, 1 ) );
				from.SendSound( 0x5BB );
			}
			else if ( info.ButtonID >= 100 )
			{
				int page = info.ButtonID - 100;

				bool isValid = pageShow(from, page - 1, true) == page; // Double-check that we can see the page when navigating forward
				if (!isValid)
				{
					from.SendMessage("Invalid character selection detected.");
					return;
				}

				if ( !from.Avatar.Active || from.Avatar.UnlockTemptations )
				{
					from.SendGump( new TemptationGump(from, new Temptation.PlayerContext(from.Temptations), from, () => TryEnterLand( page, from ), () => TryEnterLand( page, from )) );
				}
				else
				{
					TryEnterLand( page, from );
				}
			}
			else
			{
				int page = Math.Max( 0, info.ButtonID );
				from.SendGump( new GypsyTarotGump( from, page ) );
				from.SendSound( 0x5B9 );
			}
		}
	}
}