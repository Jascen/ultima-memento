using System.Collections.Generic;
using System.Linq;
using Server.Misc;

namespace Server.Spells.Jester
{
    public class JesterSpellProvider
    {
        public static readonly List<SpellDefinition> SpellDefinitions = new List<SpellDefinition>
        {
            CanOfSnakes.SpellInfo.SpellDefinition,
            Clowns.SpellInfo.SpellDefinition,
            FlowerPower.SpellInfo.SpellDefinition,
            Hilarity.SpellInfo.SpellDefinition,
            Insult.SpellInfo.SpellDefinition,
            JumpAround.SpellInfo.SpellDefinition,
            PoppingBalloon.SpellInfo.SpellDefinition,
            RabbitInAHat.SpellInfo.SpellDefinition,
            SeltzerBottle.SpellInfo.SpellDefinition,
            SurpriseGift.SpellInfo.SpellDefinition,
        };

        public static int FirstSpellId
        { get { return SpellDefinitions[0].SpellID; } }

        public static int LastSpellId
        { get { return SpellDefinitions[SpellDefinitions.Count - 1].SpellID; } }

        public static int SpellCount
        { get { return SpellDefinitions.Count; } }

        public static void Cast(Mobile from, int spellID)
        {
            if (!Multis.DesignContext.Check(from)) return;
            if (!GetPlayerInfo.isJester(from))
            {
                from.SendMessage("You are not a jester!");
                return;
            }

            if (HasSpell(from, spellID))
                SpellRegistry.NewSpell(spellID, from, null).Cast();
            else
                from.SendLocalizedMessage(500015);
        }

        public static SpellDefinition GetDefinition(int spellID)
        {
            var spellDefinition = SpellDefinitions.FirstOrDefault(definition => definition.SpellID == spellID);

            return spellDefinition;
        }

        public static bool HasSpell(Mobile from, int spellID)
        {
            return SpellDefinitions.Any(definition => definition.SpellID == spellID);
        }
    }
}