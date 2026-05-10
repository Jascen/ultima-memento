using System;
using System.Collections.Generic;
using Server.Engines.MLQuests.Gumps;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.MLQuests.Objectives
{
	public class DeliverObjective : BaseObjective
	{
		public Type Delivery { get; private set; }
		public int Amount { get; private set; }
		public Type Destination { get; private set; }
		public bool SpawnsDelivery { get; private set; }

		public DeliverObjective(Type delivery, int amount, Type destination)
			: this(delivery, amount, destination, true)
		{
		}

		public DeliverObjective(Type delivery, int amount, Type destination, bool spawnsDelivery)
		{
			Delivery = delivery;
			Amount = amount;
			Destination = destination;
			SpawnsDelivery = spawnsDelivery;
		}

		public virtual void SpawnDelivery(Container pack)
		{
			if (!SpawnsDelivery || pack == null)
				return;

			List<Item> delivery = new List<Item>();

			for (int i = 0; i < Amount; ++i)
			{
				Item item = Activator.CreateInstance(Delivery) as Item;

				if (item == null)
					continue;

				delivery.Add(item);

				if (item.Stackable && Amount > 1)
				{
					item.Amount = Amount;
					break;
				}
			}

			foreach (Item item in delivery)
			{
				item.QuestItem = true;
				if (string.IsNullOrWhiteSpace(item.InfoText1)) item.InfoText1 = GetDeliveryInstructions();
				else if (string.IsNullOrWhiteSpace(item.InfoText2)) item.InfoText2 = GetDeliveryInstructions();
				else if (string.IsNullOrWhiteSpace(item.InfoText3)) item.InfoText3 = GetDeliveryInstructions();
				else if (string.IsNullOrWhiteSpace(item.InfoText4)) item.InfoText4 = GetDeliveryInstructions();
				else if (string.IsNullOrWhiteSpace(item.InfoText5)) item.InfoText5 = GetDeliveryInstructions();

				pack.DropItem(item); // Confirmed: on OSI items are added even if your pack is full
			}
		}

		public override void WriteToGump(Gump g, ref int y)
		{
			TextDefinition.AddHtmlText(g, 98, y, 366 - 5, 16, GetDeliveryInstructions(), false, false, BaseQuestGump.COLOR_LOCALIZED, BaseQuestGump.COLOR_HTML);
			y += 16;
		}

		public override BaseObjectiveInstance CreateInstance(MLQuestInstance instance)
		{
			return new DeliverObjectiveInstance(this, instance);
		}

		private string GetDeliveryInstructions()
		{
			return string.Format("Deliver to {0}", QuesterNameAttribute.GetQuesterNameFor(Destination));
		}
	}

	#region Timed

	public class TimedDeliverObjective : DeliverObjective
	{
		private TimeSpan m_Duration;

		public override bool IsTimed { get { return true; } }
		public override TimeSpan Duration { get { return m_Duration; } }

		public TimedDeliverObjective(TimeSpan duration, Type delivery, int amount, Type destination)
			: this(duration, delivery, amount, destination, true)
		{
		}

		public TimedDeliverObjective(TimeSpan duration, Type delivery, int amount, Type destination, bool spawnsDelivery)
			: base(delivery, amount, destination, spawnsDelivery)
		{
			m_Duration = duration;
		}
	}

	#endregion

	public class DeliverObjectiveInstance : BaseObjectiveInstance
	{
		private DeliverObjective m_Objective;
		private bool m_HasCompleted;

		public DeliverObjective Objective
		{
			get { return m_Objective; }
			set { m_Objective = value; }
		}

		public bool HasCompleted
		{
			get { return m_HasCompleted; }
			set { m_HasCompleted = value; }
		}

		public DeliverObjectiveInstance(DeliverObjective objective, MLQuestInstance instance)
			: base(instance, objective)
		{
			m_Objective = objective;
		}

		public virtual bool IsDestination(IQuestGiver quester, Type type)
		{
			Type destType = m_Objective.Destination;

			return destType != null && destType.IsAssignableFrom(type);
		}

		public override bool IsCompleted()
		{
			return m_HasCompleted;
		}

		public override void OnQuestAccepted()
		{
			m_Objective.SpawnDelivery(Instance.Player.Backpack);
		}

		// This is VERY similar to CollectObjective.GetCurrentTotal
		private int GetCurrentTotal()
		{
			Container pack = Instance.Player.Backpack;

			if (pack == null)
				return 0;

			Item[] items = pack.FindItemsByType(m_Objective.Delivery, false); // Note: subclasses are included
			int total = 0;

			foreach (Item item in items)
				total += item.Amount;

			return total;
		}

		public override bool OnBeforeClaimReward()
		{
			PlayerMobile pm = Instance.Player;

			int total = GetCurrentTotal();
			int desired = m_Objective.Amount;

			if (total < desired)
			{
				pm.SendLocalizedMessage(1074861); // You do not have everything you need!
				pm.SendLocalizedMessage(1074885, String.Format("{0}\t{1}", total, desired)); // You have ~1_val~ item(s) but require ~2_val~
				return false;
			}

			return true;
		}

		// TODO: This is VERY similar to CollectObjective.OnClaimReward
		public override void OnClaimReward()
		{
			Container pack = Instance.Player.Backpack;

			if (pack == null)
				return;

			Item[] items = pack.FindItemsByType(m_Objective.Delivery, false);
			int left = m_Objective.Amount;

			foreach (Item item in items)
			{
				if (left == 0)
					break;

				if (item.Amount > left)
				{
					item.Consume(left);
					left = 0;
				}
				else
				{
					item.Delete();
					left -= item.Amount;
				}
			}
		}

		public override void OnQuestCancelled()
		{
			OnClaimReward(); // same effect
		}

		public override void OnExpire()
		{
			OnQuestCancelled();

			Instance.Player.SendLocalizedMessage(1074813); // You have failed to complete your delivery.
		}

		public override void WriteToGump(Gump g, ref int y)
		{
			m_Objective.WriteToGump(g, ref y);

			base.WriteToGump(g, ref y);

			// No extra instance stuff printed for this objective
		}

		public override DataType ExtraDataType { get { return DataType.DeliverObjective; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(m_HasCompleted);
		}
	}
}
