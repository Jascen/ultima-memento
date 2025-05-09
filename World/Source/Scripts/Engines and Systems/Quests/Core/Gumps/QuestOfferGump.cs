using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.MLQuests.Gumps
{
	public class QuestOfferGump : BaseQuestGump
	{
		private MLQuest m_Quest;
		private IQuestGiver m_Quester;
		private MLQuestInstance m_QuestInstance;

		public QuestOfferGump(MLQuest quest, IQuestGiver quester, PlayerMobile pm)
			: base(1049010) // Quest Offer
		{
			m_Quest = quest;
			m_Quester = quester;
			m_QuestInstance = quest.CreateInstance(quester, pm);

			Closable = false;

			CloseOtherGumps(pm);
			pm.CloseGump(typeof(QuestOfferGump));

			SetTitle(quest.Title);
			RegisterButton(ButtonPosition.Left, ButtonGraphic.Accept, 1);
			RegisterButton(ButtonPosition.Right, ButtonGraphic.Refuse, 2);

			var hasRewards = 0 < quest.Rewards.Count;
			SetPageCount(hasRewards ? 3 : 2);

			BuildPage();
			AddDescription(quest);

			BuildPage();
			AddObjectives(m_QuestInstance);

			if (hasRewards)
			{
				BuildPage();
				AddRewardsPage(quest);
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			PlayerMobile pm = sender.Mobile as PlayerMobile;

			if (pm == null)
				return;

			switch (info.ButtonID)
			{
				case 1: // Accept
					{
						m_QuestInstance.Accept();
						m_Quest.OnAccept(m_Quester, pm, m_QuestInstance);
						break;
					}
				case 2: // Refuse
					{
						if (!m_Quest.IsChainTriggered)
						{

							pm.SendMessage("You refuse the quest.");
							m_Quest.OnRefuse(m_Quester, pm);
						}
						else
						{
							var confirmation = new ConfirmationGump(
								pm,
								"This quest was triggered by a chain. If you refuse it, you may need to repeat parts of the chain.<br><br>Are you sure you wish to refuse the quest?",
								() =>
									{
										pm.SendMessage("You refuse the quest.");
										m_Quest.OnRefuse(m_Quester, pm);
									},
								() => pm.SendGump(new QuestOfferGump(m_Quest, m_Quester, pm))
							);
							pm.SendGump(confirmation);
						}
						break;
					}
			}
		}
	}
}
