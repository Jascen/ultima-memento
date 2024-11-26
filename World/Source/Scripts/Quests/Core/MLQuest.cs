using System;
using System.Collections.Generic;
using Server.Engines.MLQuests.Gumps;
using Server.Engines.MLQuests.Objectives;
using Server.Engines.MLQuests.Rewards;
using Server.Mobiles;

namespace Server.Engines.MLQuests
{
	public enum ObjectiveType
	{
		All,
		Any
	}

	public abstract class MLQuest
	{
		public bool Deserialized { get; private set; }
		public bool SaveEnabled { get; set; }

		// TODO: Flags? (Deserialized, SaveEnabled, Activated)

		public bool Activated { get; protected set; }

		public List<BaseObjective> Objectives { get; protected set; }

		public ObjectiveType ObjectiveType { get; protected set; }

		public List<BaseReward> Rewards { get; protected set; }

		public readonly List<MLQuestInstance> Instances;

		public bool OneTimeOnly { get; protected set; }

		public bool HasRestartDelay { get; protected set; }

		public bool HasObjective<T>() where T : BaseObjective
		{
			foreach (BaseObjective obj in Objectives)
			{
				if (obj is T)
					return true;
			}

			return false;
		}

		public bool IsSkillTrainer
		{
			get { return HasObjective<GainSkillObjective>(); }
		}

		public bool RequiresCollection
		{
			get { return HasObjective<CollectObjective>() || HasObjective<DeliverObjective>(); }
		}

		public virtual bool RecordCompletion
		{
			get { return OneTimeOnly || HasRestartDelay; }
		}

		public virtual bool IsChainTriggered { get { return false; } }
		public virtual Type NextQuest { get { return null; } }

		public TextDefinition Title { get; protected set; }
		public TextDefinition Description { get; protected set; }
		public TextDefinition RefusalMessage { get; protected set; }
		public TextDefinition InProgressMessage { get; protected set; }
		public TextDefinition CompletionMessage { get; protected set; }
		public TextDefinition CompletionNotice { get; protected set; }

		public static readonly TextDefinition CompletionNoticeDefault = new TextDefinition(1072273); // You've completed a quest!  Don't forget to collect your reward.
		public static readonly TextDefinition CompletionNoticeShort = new TextDefinition(1046258); // Your quest is complete.
		public static readonly TextDefinition CompletionNoticeShortReturn = new TextDefinition(1073775); // Your quest is complete. Return for your reward.
		public static readonly TextDefinition CompletionNoticeCraft = new TextDefinition(1073967); // You obtained what you seek, now receive your reward.

		public MLQuest()
		{
			Activated = false;
			Objectives = new List<BaseObjective>();
			ObjectiveType = ObjectiveType.All;
			Rewards = new List<BaseReward>();
			CompletionNotice = CompletionNoticeDefault;

			Instances = new List<MLQuestInstance>();

			SaveEnabled = true;
		}

		public abstract IEnumerable<Type> GetQuestGivers();

		/// <summary>
		/// Everything required to complete the Quest
		/// </summary>
		public virtual void Generate()
		{
			if (MLQuestSystem.Debug)
				Console.WriteLine("INFO: Generating quest: {0}", GetType());
		}

		#region Generation Methods

		public void PutSpawner(Spawner s, Point3D loc, Map map)
		{
			string name = String.Format("MLQS-{0}", GetType().Name);

			// Auto cleanup on regeneration
			List<Item> toDelete = new List<Item>();

			foreach (Item item in map.GetItemsInRange(loc, 0))
			{
				if (item is Spawner && item.Name == name)
					toDelete.Add(item);
			}

			foreach (Item item in toDelete)
				item.Delete();

			s.Name = name;
			s.MoveToWorld(loc, map);
		}

		public void PutDeco(Item deco, Point3D loc, Map map)
		{
			// Auto cleanup on regeneration
			List<Item> toDelete = new List<Item>();

			foreach (Item item in map.GetItemsInRange(loc, 0))
			{
				if (item.ItemID == deco.ItemID && item.Z == loc.Z)
					toDelete.Add(item);
			}

			foreach (Item item in toDelete)
				item.Delete();

			deco.MoveToWorld(loc, map);
		}

		#endregion

		public MLQuestInstance CreateInstance(IQuestGiver quester, PlayerMobile pm)
		{
			return new MLQuestInstance(this, quester, pm);
		}

		public bool CanOffer(IQuestGiver quester, PlayerMobile pm, bool message)
		{
			return CanOffer(quester, pm, MLQuestSystem.GetContext(pm), message);
		}

		public virtual bool CanOffer(IQuestGiver quester, PlayerMobile pm, MLQuestContext context, bool message)
		{
			if (!Activated || quester.Deleted)
				return false;

			if (context != null)
			{
				if (context.IsFull)
				{
					if (message)
						MLQuestSystem.Tell(quester, pm, 1080107); // I'm sorry, I have nothing for you at this time.

					return false;
				}

				MLQuest checkQuest = this;

				while (checkQuest != null)
				{
					DateTime nextAvailable;

					if (context.HasDoneQuest(checkQuest, out nextAvailable))
					{
						if (checkQuest.OneTimeOnly)
						{
							if (message)
								MLQuestSystem.Tell(quester, pm, 1075454); // I cannot offer you the quest again.

							return false;
						}
						else if (nextAvailable > DateTime.UtcNow)
						{
							if (message)
							{
								MLQuestSystem.Tell(quester, pm, 1075575); // I'm sorry, but I don't have anything else for you right now. Could you check back with me in a few minutes?
								TimeSpan timeRemaining = nextAvailable - DateTime.UtcNow;
								string delayMessage = string.Format("You get a feeling you should return after {0:D2} days {1:D2} hours and {2:D2} minutes", timeRemaining.Days, timeRemaining.Hours, timeRemaining.Minutes);
								pm.SendMessage(delayMessage);
							}

							return false;
						}
					}

					if (checkQuest.NextQuest == null)
						break;

					checkQuest = MLQuestSystem.FindQuest(checkQuest.NextQuest);
				}
			}

			foreach (BaseObjective obj in Objectives)
			{
				if (!obj.CanOffer(quester, pm, message))
					return false;
			}

			return true;
		}

		public virtual void SendOffer(IQuestGiver quester, PlayerMobile pm)
		{
			pm.SendGump(new QuestOfferGump(this, quester, pm));
		}

		public virtual void OnAccept(IQuestGiver quester, PlayerMobile pm)
		{
			if (!CanOffer(quester, pm, true))
				return;

			MLQuestInstance instance = CreateInstance(quester, pm);

			pm.SendLocalizedMessage(1049019); // You have accepted the Quest.
			pm.SendSound(0x2E7); // private sound

			OnAccepted(instance);

			foreach (BaseObjectiveInstance obj in instance.Objectives)
				obj.OnQuestAccepted();
		}

		public virtual void OnAccepted(MLQuestInstance instance)
		{
		}

		public virtual void OnRefuse(IQuestGiver quester, PlayerMobile pm)
		{
			pm.SendGump(new QuestConversationGump(this, pm, RefusalMessage));
		}

		public virtual void GetRewards(MLQuestInstance instance)
		{
			instance.SendRewardGump();
		}

		public virtual void OnRewardClaimed(MLQuestInstance instance)
		{
		}

		public virtual void OnCancel(MLQuestInstance instance)
		{
		}

		public virtual void OnQuesterDeleted(MLQuestInstance instance)
		{
		}

		public virtual void OnPlayerDeath(MLQuestInstance instance)
		{
		}

		public virtual TimeSpan GetRestartDelay()
		{
			return TimeSpan.FromSeconds(Utility.Random(1, 5) * 30);
		}

		public static void Serialize(GenericWriter writer, MLQuest quest)
		{
			MLQuestSystem.WriteQuestRef(writer, quest);
			writer.Write(quest.Version);
		}

		public static void Deserialize(GenericReader reader, int version)
		{
			MLQuest quest = MLQuestSystem.ReadQuestRef(reader);
			int oldVersion = reader.ReadInt();

			if (quest == null)
				return; // not saved or no longer exists

			quest.Refresh(oldVersion);
			quest.Deserialized = true;
		}

		public virtual int Version { get { return 0; } }

		public virtual void Refresh(int oldVersion)
		{
		}
	}
}
