using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Misc;
using Server.Items;
using Server.Network;
using Server.Commands;
using Server.Commands.Generic;
using Server.Mobiles;
using Server.Accounting;
using Server.Regions;

namespace Server.Misc
{
    class AssassinFunctions
    {
		public static void CheckTarget( Mobile m, Mobile target )
		{
			string victim = PlayerSettings.GetQuestInfo( m, "AssassinQuest" );

			if ( PlayerSettings.GetQuestState( m, "AssassinQuest" ) )
			{
				string sAssassinTarget = "";
				string sAssassinTitle = "";
				string sAssassinName = "";
				string sAssassinRegion = "";
				int nAssassinDone = 0;
				int nAssassinFee = 0;
				string sAssassinWorld = "";
				string sAssassinCategory = "";
				string sAssassinStory = "";

				string[] victims = victim.Split('#');
				int nEntry = 1;
				foreach (string victimz in victims)
				{
					if ( nEntry == 1 ){ sAssassinTarget = victimz; }
					else if ( nEntry == 2 ){ sAssassinTitle = victimz; }
					else if ( nEntry == 3 ){ sAssassinName = victimz; }
					else if ( nEntry == 4 ){ sAssassinRegion = victimz; }
					else if ( nEntry == 5 ){ nAssassinDone = Convert.ToInt32(victimz); }
					else if ( nEntry == 6 ){ nAssassinFee = Convert.ToInt32(victimz); }
					else if ( nEntry == 7 ){ sAssassinWorld = victimz; }
					else if ( nEntry == 8 ){ sAssassinCategory = victimz; }
					else if ( nEntry == 9 ){ sAssassinStory = victimz; }

					nEntry++;
				}

				string sVictim = target.GetType().ToString();

				if ( sVictim == sAssassinTarget && Server.Misc.Worlds.GetRegionName( target.Map, target.Location ) == sAssassinRegion && nAssassinDone != 1 )
				{
					m.PrivateOverheadMessage(MessageType.Regular, 0x22, false, "Your victim has been assassinated!", m.NetState);
					victim = victim.Replace("#0#", "#1#");
					m.SendSound( 0x3D );
					LoggingFunctions.LogQuestKill( m, "assassin", target );
					PlayerSettings.SetQuestInfo( m, "AssassinQuest", victim );
				}
			}
		}

		public static void QuestTimeAllowed( Mobile m )
		{
			DateTime TimeFinished = DateTime.Now;
			string sFinished = Convert.ToString(TimeFinished);
			PlayerSettings.SetQuestInfo( m, "AssassinQuest", sFinished );
		}

		public static int QuestTimeNew( Mobile m )
		{
			int QuestTime = 10000;
			string sTime = PlayerSettings.GetQuestInfo( m, "AssassinQuest" );

			if ( sTime.Length > 0 && !( PlayerSettings.GetQuestState( m, "AssassinQuest" ) ) )
			{
				DateTime TimeThen = Convert.ToDateTime(sTime);
				DateTime TimeNow = DateTime.Now;
				long ticksThen = TimeThen.Ticks;
				long ticksNow = TimeNow.Ticks;
				int minsThen = (int)TimeSpan.FromTicks(ticksThen).TotalMinutes;
				int minsNow = (int)TimeSpan.FromTicks(ticksNow).TotalMinutes;
				QuestTime = minsNow - minsThen;
			}
			return QuestTime;
		}

		public static void FindTarget( Mobile m, int fee )
		{
			var options = new List<Land>
			{
				Land.Sosaria,
				Land.Sosaria,
				Land.Sosaria,
				Land.Lodoria,
				Land.Lodoria,
				Land.Lodoria,
				Land.Serpent,
				Land.Serpent,
				Land.Serpent,
				Land.IslesDread,
				Land.Savaged,
				Land.Savaged,
				Land.UmberVeil,
				Land.Kuldar,
				Land.Underworld,
				Land.Ambrosia,
			};
			Land searchLand = PlayerSettings.GetRandomDiscoveredLand(m as PlayerMobile, options, null);
			string searchLocation = Lands.LandName(searchLand);

			int aCount = 0;
			Region reg = null;
			ArrayList targets = new ArrayList();
			foreach ( Mobile target in World.Mobiles.Values )
			if ( target is BaseCreature )
			{
				reg = Region.Find( target.Location, target.Map );
				string tWorld = Server.Lands.LandName( Server.Lands.GetLand( target.Map, target.Location, target.X, target.Y ) );

				if ( target.EmoteHue !=123 && target.Karma < 0 && target.Fame < fee && ( Server.Difficult.GetDifficulty( target.Location, target.Map ) <= GetPlayerInfo.GetPlayerDifficulty( m ) ) && reg.IsPartOf( typeof( DungeonRegion ) ) )
				{
					if ( searchLocation == "the Land of Sosaria" && tWorld == searchLocation ){ targets.Add( target ); aCount++; }
					else if ( searchLocation == "the Land of Lodoria"  && tWorld == searchLocation ){ targets.Add( target ); aCount++; }
					else if ( searchLocation == "the Serpent Island"  && tWorld == searchLocation ){ targets.Add( target ); aCount++; }
					else if ( searchLocation == "the Isles of Dread"  && tWorld == searchLocation ){ targets.Add( target ); aCount++; }
					else if ( searchLocation == "the Savaged Empire"  && tWorld == searchLocation ){ targets.Add( target ); aCount++; }
					else if ( searchLocation == "the Island of Umber Veil"  && tWorld == searchLocation ){ targets.Add( target ); aCount++; }
					else if ( searchLocation == "the Bottle World of Kuldar"  && tWorld == searchLocation ){ targets.Add( target ); aCount++; }
					else if ( searchLocation == "the Underworld"  && tWorld == searchLocation ){ targets.Add( target ); aCount++; }
				}

				if ( aCount < 1 ) // SAFETY CATCH IF IT FINDS NO CREATURES AT ALL...IT WILL FIND AT LEAST ONE IN SOSARIA //
				{
					if ( target.Karma < 0 && target.Fame < fee && reg.IsPartOf( typeof( DungeonRegion ) ) && tWorld == "the Land of Sosaria" )
					{
						targets.Add( target ); aCount++;
					}
				}
			}

			aCount = Utility.RandomMinMax( 1, aCount );

			int xCount = 0;
			for ( int i = 0; i < targets.Count; ++i )
			{
				xCount++;

				if ( xCount == aCount )
				{
					Mobile theVictim = ( Mobile )targets[ i ];
					string kWorld = Server.Lands.LandName( Server.Lands.GetLand( theVictim.Map, theVictim.Location, theVictim.X, theVictim.Y ) );

					string kVictim = theVictim.GetType().ToString();
					int nFee = theVictim.Fame / 5;
					nFee = (int)( (MyServerSettings.QuestRewardModifier() * 0.01) * nFee ) + 20 + nFee;
					string kDollar = nFee.ToString();

					string killName = theVictim.Name;
					string killTitle = theVictim.Title;
						if ( theVictim is Wyrms ){ killName = "a wyrm"; killTitle = ""; }
						if ( theVictim is Daemon ){ killName = "a daemon"; killTitle = ""; }
						if ( theVictim is Balron ){ killName = "a balron"; killTitle = ""; }
						if ( theVictim is RidingDragon || theVictim is Dragons ){ killName = "a dragon"; killTitle = ""; }
						if ( theVictim is BombWorshipper ){ killName = "a worshipper of the bomb"; killTitle = ""; }
						if ( theVictim is Psionicist ){ killName = "a psychic of the bomb"; killTitle = ""; }

					string myexplorer = kVictim + "#" + killTitle + "#" + killName + "#" + Server.Misc.Worlds.GetRegionName( theVictim.Map, theVictim.Location ) + "#0#" + kDollar + "#" + kWorld + "#Monster";
					PlayerSettings.SetQuestInfo( m, "AssassinQuest", myexplorer );

					string theStory = myexplorer + "#" + AssassinFunctions.QuestSentence( m ); // ADD THE STORY PART

					PlayerSettings.SetQuestInfo( m, "AssassinQuest", theStory );
				}
			}
		}

		public static void FindInnocentTarget( Mobile m )
		{
			string searchLocation = "the Land of Sosaria";
			switch ( Utility.RandomMinMax( 0, 13 ) )
			{
				case 0:		searchLocation = "the Land of Sosaria";			break;
				case 1:		searchLocation = "the Land of Sosaria";			break;
				case 2:		searchLocation = "the Land of Sosaria";			break;
				case 3:		searchLocation = "the Land of Lodoria";			if ( !( PlayerSettings.GetDiscovered( m, "the Land of Lodoria" ) ) ){ searchLocation = "the Land of Sosaria"; } break;
				case 4:		searchLocation = "the Land of Lodoria";			if ( !( PlayerSettings.GetDiscovered( m, "the Land of Lodoria" ) ) ){ searchLocation = "the Land of Sosaria"; } break;
				case 5:		searchLocation = "the Land of Lodoria";			if ( !( PlayerSettings.GetDiscovered( m, "the Land of Lodoria" ) ) ){ searchLocation = "the Land of Sosaria"; } break;
				case 6:		searchLocation = "the Serpent Island";			if ( !( PlayerSettings.GetDiscovered( m, "the Serpent Island" ) ) ){ searchLocation = "the Land of Sosaria"; } break;
				case 7:		searchLocation = "the Serpent Island";			if ( !( PlayerSettings.GetDiscovered( m, "the Serpent Island" ) ) ){ searchLocation = "the Land of Sosaria"; } break;
				case 8:		searchLocation = "the Serpent Island";			if ( !( PlayerSettings.GetDiscovered( m, "the Serpent Island" ) ) ){ searchLocation = "the Land of Sosaria"; } break;
				case 9:		searchLocation = "the Isles of Dread";			if ( !( PlayerSettings.GetDiscovered( m, "the Isles of Dread" ) ) ){ searchLocation = "the Land of Sosaria"; } break;
				case 10:	searchLocation = "the Savaged Empire";			if ( !( PlayerSettings.GetDiscovered( m, "the Savaged Empire" ) ) ){ searchLocation = "the Land of Sosaria"; } break;
				case 11:	searchLocation = "the Savaged Empire";			if ( !( PlayerSettings.GetDiscovered( m, "the Savaged Empire" ) ) ){ searchLocation = "the Land of Sosaria"; } break;
				case 12:	searchLocation = "the Island of Umber Veil";	if ( !( PlayerSettings.GetDiscovered( m, "the Island of Umber Veil" ) ) ){ searchLocation = "the Land of Sosaria"; } break;
				case 13:	searchLocation = "the Bottle World of Kuldar";	if ( !( PlayerSettings.GetDiscovered( m, "the Bottle World of Kuldar" ) ) ){ searchLocation = "the Land of Sosaria"; } break;
			}

			int aCount = 0;
			Region reg = null;
			ArrayList targets = new ArrayList();
			foreach ( Mobile target in World.Mobiles.Values )
			if ( target is BaseVendor )
			{
				reg = Region.Find( target.Location, target.Map );
				string tWorld = Server.Lands.LandName( Server.Lands.GetLand( target.Map, target.Location, target.X, target.Y ) );

				if ( target.Blessed == false && reg.IsPartOf( typeof( VillageRegion ) ) )
				{
					if ( searchLocation == "the Land of Sosaria" && tWorld == searchLocation ){ targets.Add( target ); aCount++; }
					else if ( searchLocation == "the Land of Lodoria"  && tWorld == searchLocation ){ targets.Add( target ); aCount++; }
					else if ( searchLocation == "the Serpent Island"  && tWorld == searchLocation ){ targets.Add( target ); aCount++; }
					else if ( searchLocation == "the Isles of Dread"  && tWorld == searchLocation ){ targets.Add( target ); aCount++; }
					else if ( searchLocation == "the Savaged Empire"  && tWorld == searchLocation ){ targets.Add( target ); aCount++; }
					else if ( searchLocation == "the Island of Umber Veil"  && tWorld == searchLocation ){ targets.Add( target ); aCount++; }
					else if ( searchLocation == "the Bottle World of Kuldar"  && tWorld == searchLocation ){ targets.Add( target ); aCount++; }
				}
			}

			aCount = Utility.RandomMinMax( 1, aCount );

			int xCount = 0;
			for ( int i = 0; i < targets.Count; ++i )
			{
				xCount++;

				if ( xCount == aCount )
				{
					Mobile theVictim = ( Mobile )targets[ i ];
					string kWorld = Server.Lands.LandName( Server.Lands.GetLand( theVictim.Map, theVictim.Location, theVictim.X, theVictim.Y ) );

					string kVictim = theVictim.GetType().ToString();
					int nFee = 1000;
					nFee = (int)( (MyServerSettings.QuestRewardModifier() * 0.01) * nFee ) + 20 + nFee;
					string kDollar = nFee.ToString();

					string myexplorer = kVictim + "#" + theVictim.Title + "#" + theVictim.Name + "#" + Server.Misc.Worlds.GetRegionName( theVictim.Map, theVictim.Location ) + "#0#" + kDollar + "#" + kWorld + "#Innocent";
					PlayerSettings.SetQuestInfo( m, "AssassinQuest", myexplorer );

					string theStory = myexplorer + "#" + AssassinFunctions.QuestSentence( m ); // ADD THE STORY PART

					PlayerSettings.SetQuestInfo( m, "AssassinQuest", theStory );
				}
			}
		}

		public static void PayAssassin( Mobile m, Mobile leader )
		{
			string victim = PlayerSettings.GetQuestInfo( m, "AssassinQuest" );

			if ( PlayerSettings.GetQuestState( m, "AssassinQuest" )  )
			{
				string sAssassinTarget = "";
				string sAssassinTitle = "";
				string sAssassinName = "";
				string sAssassinRegion = "";
				int nAssassinDone = 0;
				int nAssassinFee = 0;
				string sAssassinWorld = "";
				string sAssassinCategory = "";
				string sAssassinStory = "";

				string[] victims = victim.Split('#');
				int nEntry = 1;
				foreach (string victimz in victims)
				{
					if ( nEntry == 1 ){ sAssassinTarget = victimz; }
					else if ( nEntry == 2 ){ sAssassinTitle = victimz; }
					else if ( nEntry == 3 ){ sAssassinName = victimz; }
					else if ( nEntry == 4 ){ sAssassinRegion = victimz; }
					else if ( nEntry == 5 ){ nAssassinDone = Convert.ToInt32(victimz); }
					else if ( nEntry == 6 ){ nAssassinFee = Convert.ToInt32(victimz); }
					else if ( nEntry == 7 ){ sAssassinWorld = victimz; }
					else if ( nEntry == 8 ){ sAssassinCategory = victimz; }
					else if ( nEntry == 9 ){ sAssassinStory = victimz; }

					nEntry++;
				}

				if ( nAssassinDone > 0 && nAssassinFee > 0 )
				{
					m.SendSound( 0x3D );
					m.AddToBackpack ( new Gold( nAssassinFee ) );
					string sMessage = "";
					switch ( Utility.RandomMinMax( 0, 9 ) )
					{
						case 0:	sMessage = "I assume the death was quick. Here is " + nAssassinFee.ToString() + " gold for you.";		break;
						case 1:	sMessage = "I bet they never seen you coming. Here is " + nAssassinFee.ToString() + " gold for you.";		break;
						case 2:	sMessage = "Was the death swift? Here is " + nAssassinFee.ToString() + " gold for you.";		break;
						case 3:	sMessage = "Were there any witnesses? Here is " + nAssassinFee.ToString() + " gold for you.";		break;
						case 4:	sMessage = "I am impressed. Here is " + nAssassinFee.ToString() + " gold for you.";		break;
						case 5:	sMessage = "Word of your deed already reached my ears. Here is " + nAssassinFee.ToString() + " gold for you.";		break;
						case 6:	sMessage = "How you did that one, I'll never know. Here is " + nAssassinFee.ToString() + " gold for you.";		break;
						case 7:	sMessage = "You are one of my best. Here is " + nAssassinFee.ToString() + " gold for you.";		break;
						case 8:	sMessage = "Did you leave the body behind? Here is " + nAssassinFee.ToString() + " gold for you.";		break;
						case 9:	sMessage = "Next time, strike from the shadows. Here is " + nAssassinFee.ToString() + " gold for you.";		break;
					}
					leader.Say( sMessage );

					int totalKarma = nAssassinFee;
						if ( sAssassinCategory == "Innocent" ){ totalKarma = nAssassinFee * 2; } // MORE KARMA LOSS FOR CITIZENS

					CustomEventSink.InvokeCombatQuestCompleted( m, nAssassinFee );
					Titles.AwardFame( m, ((int)(totalKarma/100)), true );
					Titles.AwardKarma( m, -((int)(totalKarma/100)), true );

					AssassinFunctions.QuestTimeAllowed( m );

					m.Criminal = false;
					if ( sAssassinCategory == "Innocent" && m.Kills > 0 ){ m.Kills = m.Kills - 1; } // REMOVE THE KILL FOR THIS CIVILIAN
				}
			}
		}

		public static int DidAssassin( Mobile m )
		{
			int nSucceed = 0;

			string victim = PlayerSettings.GetQuestInfo( m, "AssassinQuest" );

			if ( PlayerSettings.GetQuestState( m, "AssassinQuest" )  )
			{
				string sAssassinTarget = "";
				string sAssassinTitle = "";
				string sAssassinName = "";
				string sAssassinRegion = "";
				int nAssassinDone = 0;
				int nAssassinFee = 0;
				string sAssassinWorld = "";
				string sAssassinCategory = "";
				string sAssassinStory = "";

				string[] victims = victim.Split('#');
				int nEntry = 1;
				foreach (string victimz in victims)
				{
					if ( nEntry == 1 ){ sAssassinTarget = victimz; }
					else if ( nEntry == 2 ){ sAssassinTitle = victimz; }
					else if ( nEntry == 3 ){ sAssassinName = victimz; }
					else if ( nEntry == 4 ){ sAssassinRegion = victimz; }
					else if ( nEntry == 5 ){ nAssassinDone = Convert.ToInt32(victimz); }
					else if ( nEntry == 6 ){ nAssassinFee = Convert.ToInt32(victimz); }
					else if ( nEntry == 7 ){ sAssassinWorld = victimz; }
					else if ( nEntry == 8 ){ sAssassinCategory = victimz; }
					else if ( nEntry == 9 ){ sAssassinStory = victimz; }

					nEntry++;
				}

				if ( nAssassinDone > 0 && nAssassinFee > 0 )
				{
					nSucceed = 1;
				}
			}
			return nSucceed;
		}

		public static string QuestSentence( Mobile m )
		{
			string victim = PlayerSettings.GetQuestInfo( m, "AssassinQuest" );

			string sVictimQuest = "";

			if ( PlayerSettings.GetQuestState( m, "AssassinQuest" ) )
			{
				string sAssassinTarget = "";
				string sAssassinTitle = "";
				string sAssassinName = "";
				string sAssassinRegion = "";
				int nAssassinDone = 0;
				int nAssassinFee = 0;
				string sAssassinWorld = "";
				string sAssassinCategory = "";
				string sAssassinStory = "";

				string[] victims = victim.Split('#');
				int nEntry = 1;
				foreach (string victimz in victims)
				{
					if ( nEntry == 1 ){ sAssassinTarget = victimz; }
					else if ( nEntry == 2 ){ sAssassinTitle = victimz; }
					else if ( nEntry == 3 ){ sAssassinName = victimz; }
					else if ( nEntry == 4 ){ sAssassinRegion = victimz; }
					else if ( nEntry == 5 ){ nAssassinDone = Convert.ToInt32(victimz); }
					else if ( nEntry == 6 ){ nAssassinFee = Convert.ToInt32(victimz); }
					else if ( nEntry == 7 ){ sAssassinWorld = victimz; }
					else if ( nEntry == 8 ){ sAssassinCategory = victimz; }
					else if ( nEntry == 9 ){ sAssassinStory = victimz; }

					nEntry++;
				}

				string sWorth = nAssassinFee.ToString("#,##0");
				string sVictimCalled = sAssassinName;
					if ( sAssassinTitle.Length > 0 ){ sVictimCalled = sAssassinTitle; }

				string sGiver = QuestCharacters.QuestGiver();

				string sWord1 = "us";
				switch ( Utility.RandomMinMax( 0, 4 ) )
				{
					case 0:	sWord1 = "one of us";	break;
					case 1:	sWord1 = "us";			break;
					case 2:	sWord1 = "you";			break;
					case 3:	sWord1 = "someone";		break;
					case 4:	sWord1 = "our order";	break;
				}

				string sWord2 = "go to";
				switch ( Utility.RandomMinMax( 0, 4 ) )
				{
					case 0:	sWord2 = "go to";		break;
					case 1:	sWord2 = "travel to";	break;
					case 2:	sWord2 = "journey to";	break;
					case 3:	sWord2 = "seek out";	break;
					case 4:	sWord2 = "venture to";	break;
				}

				string sWord3 = "mate";
				switch ( Utility.RandomMinMax( 0, 3 ) )
				{
					case 0:	sWord3 = "mate";		break;
					case 1:	sWord3 = "slay";			break;
					case 2:	sWord3 = "kill";			break;
					case 3:	sWord3 = "murder";			break;
				}

				sVictimQuest = sGiver + " wants " + sWord1 + " to " + sWord2 + " " + sAssassinRegion + " in " + sAssassinWorld + " and " + sWord3 + " " + sVictimCalled + " for " + sWorth + " gold";
			}
			return sVictimQuest;
		}

		public static string QuestStatus( Mobile m )
		{
			string victim = PlayerSettings.GetQuestInfo( m, "AssassinQuest" );

			string sVictimQuest = "";

			if ( PlayerSettings.GetQuestState( m, "AssassinQuest" ) )
			{
				string sAssassinTarget = "";
				string sAssassinTitle = "";
				string sAssassinName = "";
				string sAssassinRegion = "";
				int nAssassinDone = 0;
				int nAssassinFee = 0;
				string sAssassinWorld = "";
				string sAssassinCategory = "";
				string sAssassinStory = "";

				string[] victims = victim.Split('#');
				int nEntry = 1;
				foreach (string victimz in victims)
				{
					if ( nEntry == 1 ){ sAssassinTarget = victimz; }
					else if ( nEntry == 2 ){ sAssassinTitle = victimz; }
					else if ( nEntry == 3 ){ sAssassinName = victimz; }
					else if ( nEntry == 4 ){ sAssassinRegion = victimz; }
					else if ( nEntry == 5 ){ nAssassinDone = Convert.ToInt32(victimz); }
					else if ( nEntry == 6 ){ nAssassinFee = Convert.ToInt32(victimz); }
					else if ( nEntry == 7 ){ sAssassinWorld = victimz; }
					else if ( nEntry == 8 ){ sAssassinCategory = victimz; }
					else if ( nEntry == 9 ){ sAssassinStory = victimz; }

					nEntry++;
				}

				sVictimQuest = sAssassinStory;
				string sWorth = nAssassinFee.ToString("#,##0");
				if ( nAssassinDone == 1 ){ sVictimQuest = "Return to Xardok for your " + sWorth + " gold payment"; }
			}
			return sVictimQuest;
		}

		public static int QuestFailure( Mobile m )
		{
			string victim = PlayerSettings.GetQuestInfo( m, "AssassinQuest" );

			int nPenalty = 0;

			if ( PlayerSettings.GetQuestState( m, "AssassinQuest" ) )
			{
				string sAssassinTarget = "";
				string sAssassinTitle = "";
				string sAssassinName = "";
				string sAssassinRegion = "";
				int nAssassinDone = 0;
				int nAssassinFee = 0;
				string sAssassinWorld = "";
				string sAssassinCategory = "";
				string sAssassinStory = "";

				string[] victims = victim.Split('#');
				int nEntry = 1;
				foreach (string victimz in victims)
				{
					if ( nEntry == 1 ){ sAssassinTarget = victimz; }
					else if ( nEntry == 2 ){ sAssassinTitle = victimz; }
					else if ( nEntry == 3 ){ sAssassinName = victimz; }
					else if ( nEntry == 4 ){ sAssassinRegion = victimz; }
					else if ( nEntry == 5 ){ nAssassinDone = Convert.ToInt32(victimz); }
					else if ( nEntry == 6 ){ nAssassinFee = Convert.ToInt32(victimz); }
					else if ( nEntry == 7 ){ sAssassinWorld = victimz; }
					else if ( nEntry == 8 ){ sAssassinCategory = victimz; }
					else if ( nEntry == 9 ){ sAssassinStory = victimz; }

					nEntry++;
				}
				nPenalty = nAssassinFee;
			}

			if ( nPenalty < 10 )
				nPenalty = 10;

			return nPenalty;
		}
	}
}