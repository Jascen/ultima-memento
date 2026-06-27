using System;
using System.Collections.Generic;
using System.Text;
using Server.Engines.Craft;
using Server.Engines.MLQuests.Objectives;
using Server.Engines.MLQuests.Rewards;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.MLQuests.Definitions
{
    #region Quests

    public class SupportingTheWarQuest : MLQuest
    {
        public SupportingTheWarQuest()
        {
            Activated = true;
            HasRestartDelay = true; // This quest is meant to be repeated
            Title = "Protecting our soldiers";
            var builder = new StringBuilder();
            builder.Append("Hear me now! Our once-thriving towns stand at the brink of ruin, and the time has come for each of us to do our part. The Great War has cast a shadow over our peaceful lands, and the forces of darkness grow bolder with each passing day. Trade routes have been shattered, crops are burning, and our homes are at risk.");
            builder.Append("<br><br>");
            builder.Append("But there is still hope, and that hope lies in our hands. We need brave souls, yes, but we also need the skilled hands of craftsmen, like you, to support our warriors on the front lines. The soldiers of Sosaria need your help.");
            builder.Append("<br><br>");
            builder.Append("Our soldiers must be protected from the deadly forces they face. By signing up today, you can forge the very armor that will keep our brave defenders safe and give them the strength they need to turn the tide of this war. Your craft, your skill, can make the difference between victory and defeat. The time to act is now -- stand with us, and together, we shall reclaim our land!");
            Description = builder.ToString();

            Objectives.Add(new DummyObjective("Collect the following:"));
            Objectives.Add(new CollectObjective(5, typeof(RingmailChest), "Ringmail Tunic"));
            Objectives.Add(new DummyObjective(""));
            Objectives.Add(new DummyObjective("* Click yourself to view your Quest Log"));

            builder.Clear();
            builder.Append("Mark the shields as a quest item when you are ready.<br><br>");
            builder.Append("- Click yourself to view your Quest Log<br>");
            builder.Append("- Click the reticle next to the quest<br>");
            builder.Append("- Target a container or the tunics directly<br>");
            builder.Append("- Return to the Recruiter");
            InProgressMessage = builder.ToString();

            builder.Clear();
            builder.Append("By the Virtues, your generosity will not be forgotten! Your contribution to this cause is beyond measure, and the warriors on the front lines will wear the Ringmail Tunics you've crafted with pride, knowing they were forged by the hands of true heroes like you. You've shown what it means to stand for Sosaria.");
            builder.Append("<br><br>");
            builder.Append("Your work will protect the innocent and help bring us one step closer to victory. The courage you've shown in this dark time speaks volumes, and I am proud to count you among our allies. Rest assured, your sacrifice will be honored, and your name will echo alongside those of the brave who have fought to keep our world safe. Thank you, friend. Together, we will restore the peace we once knew.");
            CompletionMessage = builder.ToString();

            Rewards.Add(new ConstructibleItemReward("3 New Weapon Recipes",
                player =>
                {
                    return DefBlacksmithy.CraftSystem.GetRandomRecipeScrolls(player, 3, recipe => !player.HasRecipe(recipe) && DefBlacksmithy.CraftSystem.IsWeaponRecipe(recipe));
                })
            );
        }

		public override Type QuestRecipient { get { return typeof(WarEffortRecruiter); } }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return QuestRecipient;  // Quest Giver & Recipient
        }

        public override void Generate()
        {
            base.Generate();

            // TODO: Place one in all Towns
            PutSpawner(new Spawner(1, 5, 10, 0, 0, QuestRecipient.Name), new Point3D(2999, 1062, 0), Map.Sosaria); // Britain Well
            PutSpawner(new Spawner(1, 5, 10, 0, 0, QuestRecipient.Name), new Point3D(7054, 725, 65), Map.Sosaria); // The Port
            PutSpawner(new Spawner(1, 5, 10, 0, 0, QuestRecipient.Name), new Point3D(4461, 1823, 0), Map.Sosaria); // Xardok's Castle
        }

        public override bool CanOffer(IQuestGiver quester, PlayerMobile pm, MLQuestContext context, bool message)
        {
            if (!base.CanOffer(quester, pm, context, message)) return false;

            return DefBlacksmithy.CraftSystem.NeedsWeaponRecipe(pm);
        }

        public override TimeSpan GetRestartDelay()
        {
            return TimeSpan.FromHours(23);
        }
    }

    #endregion

    #region Mobiles

    [QuesterName("any Local Recruiter")]
    public class WarEffortRecruiter : BasePerson
    {
        public override bool CanShout { get { return true; } }

        [Constructable]
        public WarEffortRecruiter() : base()
        {
            Name = "Local Recruiter";
            AI = AIType.AI_Citizen;
            FightMode = FightMode.None;
            Blessed = true;
            CantWalk = true;

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Utility.AssignRandomHair(this);
                HairHue = Utility.RandomHairHue();
            }
            else
            {
                Body = 0x190;
                Utility.AssignRandomHair(this);
                int HairColor = Utility.RandomHairHue();
                FacialHairItemID = Utility.RandomList(0, 8254, 8255, 8256, 8257, 8267, 8268, 8269);
                HairHue = HairColor;
                FacialHairHue = HairColor;
            }

            Server.Misc.MorphingTime.RemoveMyClothes(this);
            Server.Misc.IntelligentAction.DressUpFighters(this, "", false, false, true);
            Title = ""; // Clear the Title
        }

        public WarEffortRecruiter(Serial serial) : base(serial)
        {
        }

        public override void Shout(PlayerMobile pm)
        {
            string message;
            switch (Utility.RandomMinMax(1, 4))
            {
                default:
                case 1: message = "Oi! You there! Yes, you! I need hands, skilled and steady!"; break;
                case 2: message = "We're in need of armor, strong and ready! The war's not waiting, and neither are we! If you've got the craft in your blood, the forge needs you now!"; break;
                case 3: message = "Steel your nerves and your hearts! We're building for the fight of our lives—get to the forge, and do your part!"; break;
                case 4: message = "The need's great, and the pay's fair!"; break;
            }

            MLQuestSystem.Tell(this, pm, message);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    #endregion


    #region Items


    #endregion
}