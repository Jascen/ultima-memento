using System;
using System.Collections.Generic;
using Server.Engines_and_Systems.Quests.BulletinBoard.Objectives;
using Server.Engines.MLQuests;
using Server.Engines.MLQuests.Rewards;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines_and_Systems.Quests.BulletinBoard.Quests
{
    /// <summary>
    /// A migration of the bulletin board quest format into MLQuest. Represents a random quest picked up from an adventuring board.
    /// </summary>
    public class RandomAdventuringQuest : MLQuest
    {
        public RandomAdventuringQuest()
        {
            BuildDefaultDisplay();
            
            Objectives.Add(new RandomAdventuringObjective());
        }

        public override IEnumerable<Type> GetQuestGivers()
        {
            // This quest should be handled by the bulletin board system. It does not have normal quest givers.
            yield return typeof(StandardQuestBoard);
        }

        private void BuildDefaultDisplay()
        {
            Title = "Adventuring Contract";
            Description =
                "A job taken from the bulletin board. You should view the objectives and complete them as soon as possible.";
            
            Rewards.Add(new DummyReward("Gold"));

            Activated = true;
        }
    }
}