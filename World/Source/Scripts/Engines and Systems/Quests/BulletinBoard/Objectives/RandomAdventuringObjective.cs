using System;
using Server.Engines.MLQuests;
using Server.Engines.MLQuests.Gumps;
using Server.Engines.MLQuests.Objectives;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Engines_and_Systems.Quests.BulletinBoard.Objectives
{
    public sealed class RandomAdventuringObjective : BaseObjective
    {
        private PlayerMobile Player { get; set; }
        private RandomAdventuringObjectiveInstance PresentedInstance { get; set; }
        
        public RandomAdventuringObjective()
        {
        }
        
        public RandomAdventuringObjective(PlayerMobile player)
        {
            Player = player;
            PresentedInstance = CreateInstance(null) as RandomAdventuringObjectiveInstance;
        }

        public void WriteToGump(Gump g, RandomAdventuringObjectiveInstance instance, ref int y)
        {
            if (instance == null)
            {
                g.AddLabel(98, y, BaseQuestGump.COLOR_LABEL, "No instance");
            }
            else
            {
                g.AddLabel(98, y, BaseQuestGump.COLOR_LABEL, "Instance text");
            }

            y += 16;
        }

        public override void WriteToGump(Gump g, ref int y)
        {
            WriteToGump(g, PresentedInstance, ref y);
        }

        public override BaseObjectiveInstance CreateInstance(MLQuestInstance instance)
        {
            return new RandomAdventuringObjectiveInstance(instance, this);
        }
    }
    
    public class RandomAdventuringObjectiveInstance : BaseObjectiveInstance//, IDeserializable
    {
        private RandomAdventuringObjective Objective { get; set; }
        
        string sPCTarget = "";
        string sPCTitle = "";
        string sPCName = "";
        string sPCRegion = "";
        int nPCDone = 0;
        int nPCFee = 0;
        string sPCWorld = "";
        string sPCCategory = "";
        string sPCStory = "";
        
        public RandomAdventuringObjectiveInstance(MLQuestInstance instance, BaseObjective obj) : base(instance, obj)
        {
            Objective = obj as RandomAdventuringObjective;
        }

        public override bool IsCompleted()
        {
            return base.IsCompleted();
        }

        public override void WriteToGump(Gump g, ref int y)
        {
            Objective.WriteToGump(g, this, ref y);
        }

        /*public void Deserialize(GenericReader reader)
        {
            return;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }*/
    }
}