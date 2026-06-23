using Server.ContextMenus;
using System.Collections.Generic;
using Server.Misc;
using Server.Network;
using Server.Gumps;
using Server.Mobiles;
using Server.Utilities;

namespace Server.Items
{
	[Flipable(0x577C, 0x577B)]
	public class FishingQuestBoard : Item
	{
		[Constructable]
		public FishingQuestBoard() : base(0x577B)
		{
			Weight = 1.0;
			Name = "Seeking Brave Sailors";
			Hue = 0x8AB;
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list ) 
		{ 
			base.GetContextMenuEntries( from, list ); 
			list.Add( new SpeechGumpEntry( from ) );
			list.Add( new FishingQuestEntry( from ) );
			list.Add( new FishingQuestComplete( from ) ); 
		}

		public override void OnDoubleClick( Mobile e )
		{
			if ( e.InRange( this.GetWorldLocation(), 4 ) )
			{
				e.CloseGump( typeof( BoardGump ) );
				e.SendGump( new BoardGump( e, "SEEKING BRAVE SAILORS", "The townsfolk are looking for brave sailors of the high seas, " + e.Name +". Sailors are given bounties on pirates or sea creatures, or items they are to search for and retrieve. Each quest must be completed to get another. If you fail at one quest, the townsfolk will not grant another unless reparations are given. The more famous a sailor, the better chance to get a high priced bounty or valuable item to find.<br><br><br><br>To get a quest, one must simply ask this bulletin board if any townsfolk wish to 'hire' them. These quests do not send you to seas you have never been. Any details of the quest can be read in the quest log (typing '[quests'). When such a quest is completed, return to any of these bulletin boards and select that you are 'done'. You will be rewarded with some gold and fame. You will gain some karma unless your karma is locked. In that case, you will lose karma instead.", "#9dc2e8", false ) );
			}
			else
			{
				e.SendLocalizedMessage( 502138 ); // That is too far away for you to use
			}
		}

		public class SpeechGumpEntry : ContextMenuEntry
		{
			private Mobile m_Mobile;
			
			public SpeechGumpEntry( Mobile from ) : base( 1024, 3 )
			{
				m_Mobile = from;
			}

			public override void OnClick()
			{
			    if( !( m_Mobile is PlayerMobile ) )
				return;
				
				m_Mobile.CloseGump( typeof( BoardGump ) );
				m_Mobile.SendGump( new BoardGump( m_Mobile, "SEEKING BRAVE SAILORS", "The townsfolk are looking for brave sailors of the high seas, " + m_Mobile.Name +". Sailors are given bounties on pirates or sea creatures, or items they are to search for and retrieve. Each quest must be completed to get another. If you fail at one quest, the townsfolk will not grant another unless reparations are given. The more famous a sailor, the better chance to get a high priced bounty or valuable item to find.<br><br><br><br>To get a quest, one must simply ask this bulletin board if any townsfolk wish to 'hire' them. These quests do not send you to seas you have never been. Any details of the quest can be read in the quest log (typing '[quests'). When such a quest is completed, return to any of these bulletin boards and select that you are 'done'. You will be rewarded with some gold and fame. You will gain some karma unless your karma is locked. In that case, you will lose karma instead.", "#9dc2e8", false ) );
            }
        }

		public class FishingQuestEntry : ContextMenuEntry
		{
			private Mobile m_Mobile;
			
			public FishingQuestEntry( Mobile from ) : base( 6120, 12 )
			{
				m_Mobile = from;
			}

			public override void OnClick()
			{
			    if( !( m_Mobile is PlayerMobile ) )
				return;

				string myQuest = PlayerSettings.GetQuestInfo( m_Mobile, "FishingQuest" );

				int nAllowedForAnotherQuest = FishingQuestFunctions.QuestTimeNew( m_Mobile );
				int nServerQuestTimeAllowed = MyServerSettings.GetTimeBetweenQuests();
				int nWhenForAnotherQuest = nServerQuestTimeAllowed - nAllowedForAnotherQuest;
				string sAllowedForAnotherQuest = nWhenForAnotherQuest.ToString();

				if ( PlayerSettings.GetQuestState( m_Mobile, "FishingQuest" ) )
				{
					m_Mobile.PrivateOverheadMessage(MessageType.Regular, 1150, false, "You are already on a quest. Return here when you are done.", m_Mobile.NetState);
				}
				else if ( nWhenForAnotherQuest > 0 )
				{
					m_Mobile.PrivateOverheadMessage(MessageType.Regular, 1150, false, "There are no quests at the moment. Check back in " + sAllowedForAnotherQuest + " minutes.", m_Mobile.NetState);
				}
				else
				{
					int nFame = m_Mobile.Fame * 2;
						nFame = Utility.RandomMinMax( 0, nFame )+2000;

					FishingQuestFunctions.FindTarget( m_Mobile, nFame );

					string TellQuest = FishingQuestFunctions.QuestStatus( m_Mobile ) + ".";
					m_Mobile.PrivateOverheadMessage(MessageType.Regular, 1150, false, TellQuest, m_Mobile.NetState);
				}
            }
        }

		public class FishingQuestComplete : ContextMenuEntry
		{
			private Mobile m_Mobile;
			
			public FishingQuestComplete( Mobile from ) : base( 548, 12 )
			{
				m_Mobile = from;
			}

			public override void OnClick()
			{
			    if( !( m_Mobile is PlayerMobile ) )
				return;

				string myQuest = PlayerSettings.GetQuestInfo( m_Mobile, "FishingQuest" );

				int nSucceed = FishingQuestFunctions.DidQuest( m_Mobile );

				if ( nSucceed > 0 )
				{
					FishingQuestFunctions.PayAdventurer( m_Mobile );
				}
				else if ( myQuest.Length > 0 )
				{
					m_Mobile.CloseGump( typeof( BoardGump ) );
					m_Mobile.SendGump( new BoardGump( m_Mobile, "YOUR REPUTATION IS AT STAKE", "You are currently on a quest that should not be too difficulty for a sailor as hardy as yourself. If you feel this quest is beyond your bravery, you may never get asked to do another unless reparations are paid. If you wish to rid yourself of this quest, then you must pay the reward offered to restore your reputation with the townsfolk. So whatever the reward were to be, you must put that total on any of these bulletin boards...if you wish to abandon this quest that is.", "#9dc2e8", false ) );
				}
				else
				{
					m_Mobile.PrivateOverheadMessage(MessageType.Regular, 1150, false, "You are not currently on a quest.", m_Mobile.NetState);
				}
            }
        }

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is Gold )
			{
				int nPenalty = FishingQuestFunctions.QuestFailure( from );

				if ( ItemUtilities.ConsumeRequired( dropped, nPenalty ) )
				{
					PlayerSettings.ClearQuestInfo( from, "FishingQuest" );
					FishingQuestFunctions.QuestTimeAllowed( from );
					from.PrivateOverheadMessage(MessageType.Regular, 1153, false, "Someone else will eventually take care of this.", from.NetState);
				}
			}

			return dropped.Deleted;
		}

		public FishingQuestBoard(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}