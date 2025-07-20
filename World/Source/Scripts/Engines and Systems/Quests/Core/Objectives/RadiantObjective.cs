using Server.Gumps;

namespace Server.Engines.MLQuests.Objectives
{
    /// <summary>
    /// An objective instance that involves some degree of randomization that should be presented to the player before they accept the quest.
    /// An example would be a bulletin board quest.
    /// </summary>
    public abstract class RadiantObjectiveInstance : BaseObjectiveInstance
    {
        protected RadiantObjectiveInstance(MLQuestInstance instance, BaseObjective obj) : base(instance, obj)
        {
        }
        
        /// <summary>
        /// Writes any information this objective instance needs to express to the player in their quest log. This occurs after they have accepted the quest.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="y"></param>
        public virtual void WriteToQuestLogGump(Gump g, ref int y)
        {
            WriteToGump(g, ref y);
        }

        /// <summary>
        /// Writes any information this objective instance needs to express to the player when they are offered a quest containing this objective. This occurs before accepting the quest.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="y"></param>
        public virtual void WriteToQuestOfferGump(Gump g, ref int y)
        {
            WriteToGump(g, ref y);
        }
    }
}