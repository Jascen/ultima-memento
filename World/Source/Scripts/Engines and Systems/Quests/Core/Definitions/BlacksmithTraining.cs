using System;
using System.Collections.Generic;
using System.Text;
using Server.Engines.MLQuests.Objectives;
using Server.Engines.MLQuests.Rewards;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.MLQuests.Definitions
{
    public class BlacksmithBasicsQuest : MLQuest
    {
        public override Type NextQuest { get { return typeof(RingArmorQuest.HintQuest); } } // Optional delivery

        public BlacksmithBasicsQuest()
        {
            Activated = true;
            Title = "The Flame's Call";
            var builder = new StringBuilder();
            builder.Append("The sound of the hammer striking metal rings out like a heartbeat in the air, steady and rhythmic. In the nearby forge, you see glowing coals crackle and hiss as they feed the flames.");
            builder.Append("<br><br>");
            builder.Append("\"You there,\" the nearby Smith says, his voice gruff but warm, like a voice that has shouted over the roar of the forge for years. \"I don't take kindly to idle hands, but if you've got the fire in your belly and the grit in your bones, maybe it's time to teach you how to strike. You've got that look in your eyes - the one that says you want to learn, not just watch. Well, there's no better way than by swinging a hammer yourself. If you're up for it, head to the mines and gather some Ore.\"");
            Description = builder.ToString();

            Objectives.Add(new DummyObjective("Collect the following:"));
            Objectives.Add(new CollectObjective(500, typeof(IronIngot), "Iron Ingots") { DoNotConsume = true });
            Objectives.Add(new DummyObjective("- Return to the Blacksmith"));
            Objectives.Add(new DummyObjective(""));
            Objectives.Add(new DummyObjective("* Click yourself to view your Quest Log"));
            Objectives.Add(new DummyObjective(""));
            Objectives.Add(new DummyObjective("Tips:"));
            Objectives.Add(new DummyObjective("- Tinkers and Miners sell Shovels and Pickaxes"));
            Objectives.Add(new DummyObjective("- Double-click an item to use it"));
            Objectives.Add(new DummyObjective("- Shovels are used on the mountainside or cave floor"));
            Objectives.Add(new DummyObjective("- Ore is used on a Forge to make Ingots"));
            Objectives.Add(new DummyObjective("- Pack animals at the animal trainer can carry much"));
            Objectives.Add(new DummyObjective("heavier loads than you"));

            builder.Clear();
            builder.Append("Mark the ingots as a quest item when you are ready.<br><br>");
            builder.Append("- Click yourself to view your Quest Log<br>");
            builder.Append("- Click the reticle next to the quest<br>");
            builder.Append("- Target the ingots<br>");
            builder.Append("- Return to the Blacksmith");
            InProgressMessage = builder.ToString();

            Rewards.Add(new ItemReward("Smith Hammer", typeof(SmithHammer)));

            builder.Clear();
            builder.Append("\"Aye? Done already? Let me have a look.\" The Smith reaches for the ingots. His weathered hands are steady, fingertips tracing the surface with a practiced eye. He catches every burr - every imperfection - his sharp gaze measuring each one. You wince under the weight of his scrutiny, but there's no malice - only the sharp, critical precision of a seasoned craftsman.");
            builder.Append("<br><br>");
            builder.Append("The Smith raises his brow and shrugs. \"Not bad for a novice, but you've a ways to go yet.\"");
            builder.Append("<br><br>");
            builder.Append("His gaze shifts to your iron ingots, and he gives a slow, deliberate nod. \"Well, there's only one way to appreciate how raw material affects your work.\" He gestures to your iron ingots. \"Keep 'em.\"");
            builder.Append("<br><br>");
            builder.Append("As he turns back to his work, he can't help himself... with one final remark, the gruff smith adds:");
            builder.Append("<br>");
            builder.Append("\"Nothing is free in this world - remember that!\"");
            builder.Append("<br><br>");
            builder.Append("<br>* Click 'Continue' below to complete the quest");
            CompletionMessage = builder.ToString();
        }

        public override Type QuestRecipient { get { return typeof(Blacksmith); } }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return QuestRecipient; // Quest Giver - Any Blacksmith
        }

        public override bool CanOffer(IQuestGiver quester, PlayerMobile pm, MLQuestContext context, bool message)
        {
            if (!base.CanOffer(quester, pm, context, message)) return false;

            return pm.Skills.Blacksmith.Value < 60; // Only offer to "beginners" in the skill
        }

        public override void OnAccepted(IQuestGiver quester, MLQuestInstance instance)
        {
            base.OnAccepted(quester, instance);

            MLQuestSystem.Tell(quester, instance.Player, "Take this.");
            var item = new TrainingShovel();
            instance.Player.SendMessage("You have received a {0}.", item.Name);
            instance.Player.AddToBackpack(item);
        }
    }
}

#region Items

namespace Server.Items
{
    public class TrainingShovel : Spade
    {
        [Constructable]
        public TrainingShovel() : base()
        {
            Name = "training shovel";
            Weight = 1;
        }

        public TrainingShovel(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add("Drag onto Paperdoll");
            list.Add("Only mines Iron Ore");
        }

        public override void AppendChildProperties(ObjectPropertyList list)
        {
            // Suppress behavior
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
#endregion
