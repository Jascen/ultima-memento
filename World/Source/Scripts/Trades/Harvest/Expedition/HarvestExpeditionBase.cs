using Server;

namespace Harvest.Expedition
{
    public abstract class HarvestExpeditionBase
    {
        protected SkillName Skill { get; private set; }

        protected HarvestExpeditionBase(SkillName skillName)
        {
            Skill = skillName;
        }

        public abstract void GetReward(HarvestExpeditionNpc npc, int durationHours, double percentComplete);

        protected bool CheckSuccess(HarvestExpeditionNpc npc)
        {
            return npc.CheckSkill(Skill, (double)npc.Skills[Skill].Value / 125);
        }
    }
}
