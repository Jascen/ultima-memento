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

/*
3002004	Tell
3001024	Quests
3000548	Done
*/

namespace Server.Misc
{
    class FishingQuestFunctions
    {
		public static int ChanceToFindQuestedItem()
		{
			return 10;
		}

		public static bool SkipMe( Mobile m )
		{
			bool Skip = true;

			if	(
				m is BoatPirateMage ||
				m is ElfBoatPirateArcher ||
				m is ElfBoatPirateBard ||
				m is ElfBoatPirateMage ||
				m is BoatPirateArcher ||
				m is BoatPirateBard 
				)
				{ Skip = false; }

			if ( m.EmoteHue == 123 ){ Skip = false; }

			return Skip;
		}
		
		public static void CheckTarget( Mobile m, Mobile target, Item box )
		{
			string explorer = PlayerSettings.GetQuestInfo( m, "FishingQuest" );

			if ( PlayerSettings.GetQuestState( m, "FishingQuest" )  )
			{
				string sPCTarget = "";
				string sPCTitle = "";
				string sPCName = "";
				string sPCRegion = "";
				int nPCDone = 0;
				int nPCFee = 0;
				string sPCWorld = "";
				string sPCCategory = "";
				string sPCStory = "";

				string[] explorers = explorer.Split('#');
				int nEntry = 1;
				foreach (string explorerz in explorers)
				{
					if ( nEntry == 1 ){ sPCTarget = explorerz; }
					else if ( nEntry == 2 ){ sPCTitle = explorerz; }
					else if ( nEntry == 3 ){ sPCName = explorerz; }
					else if ( nEntry == 4 ){ sPCRegion = explorerz; }
					else if ( nEntry == 5 ){ nPCDone = Convert.ToInt32(explorerz); }
					else if ( nEntry == 6 ){ nPCFee = Convert.ToInt32(explorerz); }
					else if ( nEntry == 7 ){ sPCWorld = explorerz; }
					else if ( nEntry == 8 ){ sPCCategory = explorerz; }
					else if ( nEntry == 9 ){ sPCStory = explorerz; }

					nEntry++;
				}

				Region reg = Region.Find( target.Location, target.Map );
				bool IsSeaTarget = false;
				if ( target.WhisperHue == 999 ){ IsSeaTarget = true; }
				if ( reg.IsPartOf( typeof( PirateRegion ) ) && ( target is AncientLich || target is PirateCaptain || target is ElfPirateCaptain ) ){ IsSeaTarget = true; }

				if ( sPCCategory == "Item" && target != null )
				{
					if ( sPCCategory == "Item" && target.WhisperHue == 999 && FishingQuestFunctions.ChanceToFindQuestedItem() >= Utility.RandomMinMax( 1, 100 ) && Server.Lands.LandName( Server.Lands.GetLand( target.Map, target.Location, target.X, target.Y ) ) == sPCWorld && nPCDone != 1 )
					{
						m.PrivateOverheadMessage(MessageType.Regular, 1153, false, "Ahh...they had " + sPCName + "!", m.NetState);
						explorer = explorer.Replace("#0#", "#1#");
						m.SendSound( 0x3D );
						LoggingFunctions.LogQuestItem( m, sPCName );
						PlayerSettings.SetQuestInfo( m, "FishingQuest", explorer );
					}
				}
				else if ( target != null )
				{
					string sexplorer = target.GetType().ToString();

					if ( sexplorer == sPCTarget && IsSeaTarget == true && Server.Lands.LandName( Server.Lands.GetLand( target.Map, target.Location, target.X, target.Y ) ) == sPCWorld && nPCDone != 1 )
					{
						m.PrivateOverheadMessage(MessageType.Regular, 1153, false, "The quested bounty has been fulfilled!", m.NetState);
						explorer = explorer.Replace("#0#", "#1#");
						m.SendSound( 0x3D );
						LoggingFunctions.LogQuestKill( m, "sea", target );
						PlayerSettings.SetQuestInfo( m, "FishingQuest", explorer );
					}
				}
			}
		}

		public static void QuestTimeAllowed( Mobile m )
		{
			DateTime TimeFinished = DateTime.Now;
			string sFinished = Convert.ToString(TimeFinished);
			PlayerSettings.SetQuestInfo( m, "FishingQuest", sFinished );
		}

		public static int QuestTimeNew( Mobile m )
		{
			int QuestTime = 10000;
			string sTime = PlayerSettings.GetQuestInfo( m, "FishingQuest" );

			if ( sTime.Length > 0 && !( PlayerSettings.GetQuestState( m, "FishingQuest" ) ) )
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
			PlayerMobile PC = (PlayerMobile)m;

			var PirateHunt = Utility.RandomMinMax( 1, 8 ) == 1 && 4000 < fee;

			var options = new List<Land>
			{
				Land.Sosaria,
				Land.Sosaria,
				Land.Sosaria,
				Land.Lodoria,
				Land.Lodoria,
				Land.Lodoria,
				Land.IslesDread,
				Land.Serpent,
				Land.Serpent,
				Land.Serpent,
				Land.Savaged,
				Land.Savaged,
				Land.UmberVeil,
				Land.Kuldar,
				Land.Underworld,
			};
			Land searchLocation = PlayerSettings.GetRandomDiscoveredLand(m as PlayerMobile, options, null);

			int aCount = 0;
			Region reg = null;
			ArrayList targets = new ArrayList();
			foreach ( Mobile target in World.Mobiles.Values )
			if ( target is BaseCreature )
			{
				reg = Region.Find( target.Location, target.Map );

				if ( PirateHunt )
				{
					if ( searchLocation == Land.Sosaria )
					{
						if ( target is PirateCaptain && reg.IsPartOf( typeof( PirateRegion ) ) && target.Map == Map.Sosaria && target.Land == searchLocation )
						{
							targets.Add( target ); aCount++;
						}
					}
					else if ( searchLocation == Land.Lodoria )
					{
						if ( ( target is AncientLich || target is ElfPirateCaptain ) && reg.IsPartOf( typeof( PirateRegion ) ) && target.Map == Map.Lodor && target.Land == searchLocation )
						{
							targets.Add( target ); aCount++;
						}
					}
					else if ( searchLocation == Land.IslesDread )
					{
						if ( target is PirateCaptain && reg.IsPartOf( typeof( PirateRegion ) ) && target.Map == Map.IslesDread && target.Land == searchLocation )
						{
							targets.Add( target ); aCount++;
						}
					}
				}
				else
				{
					if ( searchLocation == Land.Sosaria )
					{
						if ( SkipMe( target ) == true && target.Karma < 0 && target.Fame < fee && target.WhisperHue == 999 && Server.Misc.Worlds.IsMainRegion( Server.Misc.Worlds.GetRegionName( target.Map, target.Location ) ) && target.Map == Map.Sosaria && target.Land == searchLocation )
						{
							targets.Add( target ); aCount++;
						}
					}
					else if ( searchLocation == Land.Lodoria )
					{
						if ( SkipMe( target ) == true && target.Karma < 0 && target.Fame < fee && target.WhisperHue == 999 && Server.Misc.Worlds.IsMainRegion( Server.Misc.Worlds.GetRegionName( target.Map, target.Location ) ) && target.Map == Map.Lodor && target.Land == searchLocation )
						{
							targets.Add( target ); aCount++;
						}
					}
					else if ( searchLocation == Land.Serpent )
					{
						if ( SkipMe( target ) == true && target.Karma < 0 && target.Fame < fee && target.WhisperHue == 999 && Server.Misc.Worlds.IsMainRegion( Server.Misc.Worlds.GetRegionName( target.Map, target.Location ) ) && target.Map == Map.SerpentIsland && target.Land == searchLocation )
						{
							targets.Add( target ); aCount++;
						}
					}
					else if ( searchLocation == Land.IslesDread )
					{
						if ( SkipMe( target ) == true && target.Karma < 0 && target.Fame < fee && target.WhisperHue == 999 && Server.Misc.Worlds.IsMainRegion( Server.Misc.Worlds.GetRegionName( target.Map, target.Location ) ) && target.Map == Map.IslesDread && target.Land == searchLocation )
						{
							targets.Add( target ); aCount++;
						}
					}
					else if ( searchLocation == Land.Savaged )
					{
						if ( SkipMe( target ) == true && target.Karma < 0 && target.Fame < fee && target.WhisperHue == 999 && Server.Misc.Worlds.IsMainRegion( Server.Misc.Worlds.GetRegionName( target.Map, target.Location ) ) && target.Map == Map.SavagedEmpire && target.Land == searchLocation )
						{
							targets.Add( target ); aCount++;
						}
					}
					else if ( searchLocation == Land.UmberVeil )
					{
						if ( SkipMe( target ) == true && target.Karma < 0 && target.Fame < fee && target.WhisperHue == 999 && Server.Misc.Worlds.IsMainRegion( Server.Misc.Worlds.GetRegionName( target.Map, target.Location ) ) && target.Map == Map.Sosaria && target.Land == searchLocation )
						{
							targets.Add( target ); aCount++;
						}
					}
					else if ( searchLocation == Land.Kuldar )
					{
						if ( SkipMe( target ) == true && target.Karma < 0 && target.Fame < fee && target.WhisperHue == 999 && Server.Misc.Worlds.IsMainRegion( Server.Misc.Worlds.GetRegionName( target.Map, target.Location ) ) && target.Map == Map.Sosaria && target.Land == searchLocation )
						{
							targets.Add( target ); aCount++;
						}
					}
					else if ( searchLocation == Land.Underworld )
					{
						if ( SkipMe( target ) == true && target.Karma < 0 && target.Fame < fee && target.WhisperHue == 999 && Server.Misc.Worlds.IsMainRegion( Server.Misc.Worlds.GetRegionName( target.Map, target.Location ) ) && target.Map == Map.Underworld && target.Land == searchLocation )
						{
							targets.Add( target ); aCount++;
						}
					}
				}

				if ( aCount < 1 ) // SAFETY CATCH IF IT FINDS NOT CREATURES AT ALL...IT WILL FIND AT LEAST ONE IN SOSARIA //
				{
					if ( target.Karma < 0 && target.Fame < fee && target.WhisperHue == 999 && Server.Misc.Worlds.IsMainRegion( Server.Misc.Worlds.GetRegionName( target.Map, target.Location ) ) && target.Map == Map.Sosaria && target.Land == Land.Sosaria )
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
					if ( Utility.RandomMinMax( 1, 3 ) != 1 ) // KILL SOMETHING
					{
						Mobile theone = ( Mobile )targets[ i ];
						string kWorld = Server.Lands.LandName( Server.Lands.GetLand( theone.Map, theone.Location, theone.X, theone.Y ) );

						int nFee = theone.Fame / 2;
						nFee = (int)( (MyServerSettings.QuestRewardModifier() * 0.01) * nFee ) + 20 + nFee;
						string kexplorer = theone.GetType().ToString();
						string kDollar = nFee.ToString();

						string killName = theone.Name;
						string killTitle = theone.Title;
							if ( theone is Wyrms ){ killName = "a wyrm"; killTitle = ""; }
							if ( theone is RidingDragon || theone is Dragons ){ killName = "a dragon"; killTitle = ""; }
							if ( theone is Daemon ){ killName = "a daemon"; killTitle = ""; }
							if ( theone is Balron ){ killName = "a balron"; killTitle = ""; }

						string myexplorer = kexplorer + "#" + killTitle + "#" + killName + "#the high seas#0#" + kDollar + "#" + kWorld + "#Monster";
						string useTitle = killTitle;

						if ( theone is AncientLich || theone is PirateCaptain || theone is ElfPirateCaptain )
						{
							Region argh = Region.Find( theone.Location, theone.Map );
							nFee = (theone.Fame/10)*3;
								if ( nFee > 2000 ){ nFee = 2000; }
								int nLow = (int)(nFee*0.75);
								nFee = (int)( Utility.RandomMinMax( nLow, nFee ) * 10 );
							kDollar = nFee.ToString();
							useTitle = argh.Name;
								useTitle = useTitle.Replace("the Waters of", "of");
							myexplorer = kexplorer + "#" + useTitle + "#the Pirate Captain#the high seas#0#" + kDollar + "#" + kWorld + "#Monster";
						}

						PlayerSettings.SetQuestInfo( m, "FishingQuest", myexplorer );

						string theStory = myexplorer + "#" + FishingQuestFunctions.QuestSentence( PC ); // ADD THE STORY PART

						PlayerSettings.SetQuestInfo( m, "FishingQuest", theStory );
					}
					else // FIND SOMETHING
					{
						Mobile theone = ( Mobile )targets[ i ];
						string kWorld = Server.Lands.LandName( Server.Lands.GetLand( theone.Map, theone.Location, theone.X, theone.Y ) );

						string kexplorer = theone.GetType().ToString();

						int qFame = m.Fame;
							if ( qFame < 1000 ){ qFame = 1000; }
							else if ( qFame > 10000 ){ qFame = 10000; }

						int nFee = (int)( ( ( Utility.RandomMinMax( 500, qFame ) / 10 ) * 10 ) / 2);
						nFee = (int)( (MyServerSettings.QuestRewardModifier() * 0.01) * nFee ) + 20 + nFee;

						string kDollar = nFee.ToString();

						string ItemToFind = QuestCharacters.QuestItems( true );

						string myexplorer = "##" + ItemToFind + "#the high seas#0#" + kDollar + "#" + kWorld + "#Item";
						PlayerSettings.SetQuestInfo( m, "FishingQuest", myexplorer );

						string theStory = myexplorer + "#" + FishingQuestFunctions.QuestSentence( PC ); // ADD THE STORY PART

						PlayerSettings.SetQuestInfo( m, "FishingQuest", theStory );
					}
				}
			}
		}

		public static void PayAdventurer( Mobile m )
		{
			string explorer = PlayerSettings.GetQuestInfo( m, "FishingQuest" );

			if ( PlayerSettings.GetQuestState( m, "FishingQuest" )  )
			{
				string sPCTarget = "";
				string sPCTitle = "";
				string sPCName = "";
				string sPCRegion = "";
				int nPCDone = 0;
				int nPCFee = 0;
				string sPCWorld = "";
				string sPCCategory = "";
				string sPCStory = "";

				string[] explorers = explorer.Split('#');
				int nEntry = 1;
				foreach (string explorerz in explorers)
				{
					if ( nEntry == 1 ){ sPCTarget = explorerz; }
					else if ( nEntry == 2 ){ sPCTitle = explorerz; }
					else if ( nEntry == 3 ){ sPCName = explorerz; }
					else if ( nEntry == 4 ){ sPCRegion = explorerz; }
					else if ( nEntry == 5 ){ nPCDone = Convert.ToInt32(explorerz); }
					else if ( nEntry == 6 ){ nPCFee = Convert.ToInt32(explorerz); }
					else if ( nEntry == 7 ){ sPCWorld = explorerz; }
					else if ( nEntry == 8 ){ sPCCategory = explorerz; }
					else if ( nEntry == 9 ){ sPCStory = explorerz; }

					nEntry++;
				}

				if ( nPCDone > 0 && nPCFee > 0 )
				{
					m.SendSound( 0x3D );
					m.AddToBackpack ( new Gold( nPCFee ) );
					string sMessage = "Here is " + nPCFee.ToString() + " gold for you.";
					m.PrivateOverheadMessage(MessageType.Regular, 1150, false, sMessage, m.NetState);
					FishingQuestFunctions.QuestTimeAllowed( m );

					CustomEventSink.InvokeCombatQuestCompleted( m, nPCFee );
					Titles.AwardFame( m, ((int)(nPCFee/100)), true );
					if ( ((PlayerMobile)m).KarmaLocked == true ){ Titles.AwardKarma( m, -((int)(nPCFee/100)), true ); }
					else { Titles.AwardKarma( m, ((int)(nPCFee/100)), true ); }
				}
			}
		}

		public static int DidQuest( Mobile m )
		{
			int nSucceed = 0;

			string explorer = PlayerSettings.GetQuestInfo( m, "FishingQuest" );

			if ( PlayerSettings.GetQuestState( m, "FishingQuest" ) )
			{
				string sPCTarget = "";
				string sPCTitle = "";
				string sPCName = "";
				string sPCRegion = "";
				int nPCDone = 0;
				int nPCFee = 0;
				string sPCWorld = "";
				string sPCCategory = "";
				string sPCStory = "";

				string[] explorers = explorer.Split('#');
				int nEntry = 1;
				foreach (string explorerz in explorers)
				{
					if ( nEntry == 1 ){ sPCTarget = explorerz; }
					else if ( nEntry == 2 ){ sPCTitle = explorerz; }
					else if ( nEntry == 3 ){ sPCName = explorerz; }
					else if ( nEntry == 4 ){ sPCRegion = explorerz; }
					else if ( nEntry == 5 ){ nPCDone = Convert.ToInt32(explorerz); }
					else if ( nEntry == 6 ){ nPCFee = Convert.ToInt32(explorerz); }
					else if ( nEntry == 7 ){ sPCWorld = explorerz; }
					else if ( nEntry == 8 ){ sPCCategory = explorerz; }
					else if ( nEntry == 9 ){ sPCStory = explorerz; }

					nEntry++;
				}

				if ( nPCDone > 0 && nPCFee > 0 )
				{
					nSucceed = 1;
				}
			}
			return nSucceed;
		}

		public static string QuestSentence( Mobile m )
		{
			string explorer = PlayerSettings.GetQuestInfo( m, "FishingQuest" );

			string sMainQuest = "";

			if ( PlayerSettings.GetQuestState( m, "FishingQuest" ) )
			{
				string sPCTarget = "";
				string sPCTitle = "";
				string sPCName = "";
				string sPCRegion = "";
				int nPCDone = 0;
				int nPCFee = 0;
				string sPCWorld = "";
				string sPCCategory = "";
				string sPCStory = "";

				string[] explorers = explorer.Split('#');
				int nEntry = 1;
				foreach (string explorerz in explorers)
				{
					if ( nEntry == 1 ){ sPCTarget = explorerz; }
					else if ( nEntry == 2 ){ sPCTitle = explorerz; }
					else if ( nEntry == 3 ){ sPCName = explorerz; }
					else if ( nEntry == 4 ){ sPCRegion = explorerz; }
					else if ( nEntry == 5 ){ nPCDone = Convert.ToInt32(explorerz); }
					else if ( nEntry == 6 ){ nPCFee = Convert.ToInt32(explorerz); }
					else if ( nEntry == 7 ){ sPCWorld = explorerz; }
					else if ( nEntry == 8 ){ sPCCategory = explorerz; }
					else if ( nEntry == 9 ){ sPCStory = explorerz; }

					nEntry++;
				}

				string sWorth = nPCFee.ToString("#,##0");
				string sTheyCalled = sPCName;
					if ( sPCTitle.Length > 0 ){ sTheyCalled = sPCTitle; }

				string sGiver = QuestCharacters.QuestGiverKarma( ((PlayerMobile)m).KarmaLocked );

				string sWord1 = "you";
				switch ( Utility.RandomMinMax( 0, 4 ) )
				{
					case 0:	sWord1 = "a brave sailor";		break;
					case 1:	sWord1 = "a sailor";			break;
					case 2:	sWord1 = "you";					break;
					case 3:	sWord1 = "someone";				break;
					case 4:	sWord1 = "one willing";			break;
				}

				string sWord2 = "go to";
				switch ( Utility.RandomMinMax( 0, 4 ) )
				{
					case 0:	sWord2 = "sail on";		break;
					case 1:	sWord2 = "travel on";	break;
					case 2:	sWord2 = "journey on";	break;
					case 3:	sWord2 = "search on";	break;
					case 4:	sWord2 = "venture on";	break;
				}

				string sWord3 = "kill";

				if ( sPCCategory == "Item" )
				{
					string sBoat = "raft";
					switch ( Utility.RandomMinMax( 0, 4 ) )
					{
						case 0:	sBoat = "raft";			break;
						case 1:	sBoat = "ship";			break;
						case 2:	sBoat = "galleon";		break;
						case 3:	sBoat = "barge";		break;
						case 4:	sBoat = "vessel";		break;
					}
					string sTaken = "They lost it after";
					switch ( Utility.RandomMinMax( 0, 4 ) )
					{
						case 0:	sTaken = "They lost it after";			break;
						case 1:	sTaken = "It was lost when";			break;
						case 2:	sTaken = "They last saw it when";		break;
						case 3:	sTaken = "It was last seen before";		break;
						case 4:	sTaken = "They had it before";			break;
					}
					string sMonster = "a sea creature";
					switch ( Utility.RandomMinMax( 0, 4 ) )
					{
						case 0:	sMonster = "a sea creature";			break;
						case 1:	sMonster = "a monster from the deep";	break;
						case 2:	sMonster = "a creature from the sea";	break;
						case 3:	sMonster = "a horror from the deep";	break;
						case 4:	sMonster = "an ocean creature";			break;
					}
					string sHappened = "was attacked";
					switch ( Utility.RandomMinMax( 0, 4 ) )
					{
						case 0:	sHappened = "was attacked";		break;
						case 1:	sHappened = "was sunk";			break;
						case 2:	sHappened = "was destroyed";	break;
						case 3:	sHappened = "was overturned";	break;
						case 4:	sHappened = "was overrun";		break;
					}
					string sWord4 = sTaken + " their " + sBoat + " " + sHappened + " by " + sMonster;

					switch ( Utility.RandomMinMax( 0, 3 ) )
					{
						case 0:	sWord3 = "find";			break;
						case 1:	sWord3 = "seek";			break;
						case 2:	sWord3 = "look for";		break;
						case 3:	sWord3 = "bring back";		break;
					}
					sMainQuest = sGiver + " wants " + sWord1 + " to " + sWord2 + " " + sPCRegion + " in " + sPCWorld + " and " + sWord3 + " " + sTheyCalled + " for " + sWorth + " gold. " + sWord4;
				}
				else
				{
					switch ( Utility.RandomMinMax( 0, 3 ) )
					{
						case 0:	sWord3 = "eliminate";		break;
						case 1:	sWord3 = "slay";			break;
						case 2:	sWord3 = "kill";			break;
						case 3:	sWord3 = "destroy";			break;
					}
					sMainQuest = sGiver + " wants " + sWord1 + " to " + sWord2 + " " + sPCRegion + " in " + sPCWorld + " and " + sWord3 + " " + sTheyCalled + " for " + sWorth + " gold";
					if ( sPCName == "the Pirate Captain" )
					{
						sMainQuest = sGiver + " wants " + sWord1 + " to " + sWord2 + " " + sPCRegion + " in " + sPCWorld + " and " + sWord3 + " " + sPCName + ", " + sPCTitle + ", for " + sWorth + " gold";
					}
				}
			}
			return sMainQuest;
		}

		public static string QuestStatus( Mobile m )
		{
			string explorer = PlayerSettings.GetQuestInfo( m, "FishingQuest" );

			string sexplorerQuest = "";

			if ( PlayerSettings.GetQuestState( m, "FishingQuest" ) )
			{
				string sPCTarget = "";
				string sPCTitle = "";
				string sPCName = "";
				string sPCRegion = "";
				int nPCDone = 0;
				int nPCFee = 0;
				string sPCWorld = "";
				string sPCCategory = "";
				string sPCStory = "";

				string[] explorers = explorer.Split('#');
				int nEntry = 1;
				foreach (string explorerz in explorers)
				{
					if ( nEntry == 1 ){ sPCTarget = explorerz; }
					else if ( nEntry == 2 ){ sPCTitle = explorerz; }
					else if ( nEntry == 3 ){ sPCName = explorerz; }
					else if ( nEntry == 4 ){ sPCRegion = explorerz; }
					else if ( nEntry == 5 ){ nPCDone = Convert.ToInt32(explorerz); }
					else if ( nEntry == 6 ){ nPCFee = Convert.ToInt32(explorerz); }
					else if ( nEntry == 7 ){ sPCWorld = explorerz; }
					else if ( nEntry == 8 ){ sPCCategory = explorerz; }
					else if ( nEntry == 9 ){ sPCStory = explorerz; }

					nEntry++;
				}

				sexplorerQuest = sPCStory;
				string sWorth = nPCFee.ToString("#,##0");
				if ( nPCDone == 1 ){ sexplorerQuest = "Return to any sailor quest bulletin board for your " + sWorth + " gold payment"; }
			}
			return sexplorerQuest;
		}

		public static int QuestFailure( Mobile m )
		{
			string explorer = PlayerSettings.GetQuestInfo( m, "FishingQuest" );

			int nPenalty = 0;

			if ( PlayerSettings.GetQuestState( m, "FishingQuest" ) )
			{
				string sPCTarget = "";
				string sPCTitle = "";
				string sPCName = "";
				string sPCRegion = "";
				int nPCDone = 0;
				int nPCFee = 0;
				string sPCWorld = "";
				string sPCCategory = "";
				string sPCStory = "";

				string[] explorers = explorer.Split('#');
				int nEntry = 1;
				foreach (string explorerz in explorers)
				{
					if ( nEntry == 1 ){ sPCTarget = explorerz; }
					else if ( nEntry == 2 ){ sPCTitle = explorerz; }
					else if ( nEntry == 3 ){ sPCName = explorerz; }
					else if ( nEntry == 4 ){ sPCRegion = explorerz; }
					else if ( nEntry == 5 ){ nPCDone = Convert.ToInt32(explorerz); }
					else if ( nEntry == 6 ){ nPCFee = Convert.ToInt32(explorerz); }
					else if ( nEntry == 7 ){ sPCWorld = explorerz; }
					else if ( nEntry == 8 ){ sPCCategory = explorerz; }
					else if ( nEntry == 9 ){ sPCStory = explorerz; }

					nEntry++;
				}
				nPenalty = nPCFee;
			}

			if ( nPenalty < 10 )
				nPenalty = 10;

			return nPenalty;
		}
	}
}