using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Engines.Harvest;

namespace Harvest.Expedition
{

    public class HarvestSystemExpedition : HarvestExpeditionBase
    {
        public HarvestSystemExpedition(HarvestDefinition harvestDefinition) : base(harvestDefinition.Skill)
        {
        }

        private int TryCreateAmount(HarvestExpeditionNpc npc, int maxAttempts, int maxAmountPerRoll)
        {
            int total = 0;
            for (int i = 0; i < maxAttempts; ++i)
            {
                if (!CheckSuccess(npc)) continue;

                total += 1 + Utility.Random(maxAmountPerRoll);
            }

            return total;
        }

        public override void GetReward(HarvestExpeditionNpc npc, int durationHours, double percentComplete)
        {
            const int MILLISECONDS_PER_HOUR = 60 * 60 * 1000;

            switch (Skill)
            {
                case SkillName.Lumberjacking:
                case SkillName.Mining:
                    {
                        const double EFFICIENCY = 0.2;

                        HarvestSystem system;
                        switch (Skill)
                        {
                            case SkillName.Lumberjacking: system = Lumberjacking.System; break;
                            case SkillName.Mining: system = Mining.System; break;
                            default: return; // Should never hit
                        }

                        /*
                            EffectDelay for `Mining` and `Lumberjacking` is `1.6s` = `1600ms`

                            1 hours  - 3,600s  (2,250 actions)  -- 20% efficiency is 450    actions = 337 common,  90 uncommon,   22 rare
                            4 hours  - 14,400s (9,000 actions)  -- 20% efficiency is 1,800  actions = 1350 common, 360 uncommon,  90 rare
                            8 hours  - 28,800s (18,000 actions) -- 20% efficiency is 3,600  actions = 2700 common, 720 uncommon,  180 rare
                            12 hours - 43,200s (27,000 actions) -- 20% efficiency is 5,400  actions = 4050 common, 1080 uncommon, 270 rare
                            24 hours - 86,400s (54,000 actions) -- 20% efficiency is 10,800 actions = 8100 common, 2160 uncommon, 540 rare
                        */
                        var definition = system.Definitions[0];
                        var totalMilliseconds = durationHours * MILLISECONDS_PER_HOUR * percentComplete;
                        var actions = (int)(EFFICIENCY * totalMilliseconds / definition.EffectDelay.TotalMilliseconds);

                        int commonAmount = TryCreateAmount(npc, (int)(actions * 0.75), 1);
                        CreateResources(npc, system, definition, HarvestExpeditionRewardRarity.Common, commonAmount);

                        int uncommonAmount = TryCreateAmount(npc, (int)(actions * 0.20), 1);
                        CreateResources(npc, system, definition, HarvestExpeditionRewardRarity.Uncommon, uncommonAmount);

                        int rareAmount = TryCreateAmount(npc, (int)(actions * 0.05), 1);
                        CreateResources(npc, system, definition, HarvestExpeditionRewardRarity.Rare, rareAmount);
                        break;
                    }
            }
        }

        private bool CreateResources(HarvestExpeditionNpc npc, HarvestSystem system, HarvestDefinition definition, HarvestExpeditionRewardRarity rarity, int amount)
        {
            if (amount < 1) return false;

            var veins = definition.Veins;

            HarvestVein vein = null;
            switch (rarity)
            {
                case HarvestExpeditionRewardRarity.Common:
                    {
                        vein = veins[0];
                        break;
                    }

                case HarvestExpeditionRewardRarity.Uncommon:
                case HarvestExpeditionRewardRarity.Rare:
                    {
                        var candidates = veins
                            .Skip(1) // Skip basic resource
                            .Where(v => v.PrimaryResource.MinSkill <= npc.Skills[definition.Skill].Value);
                        if (rarity == HarvestExpeditionRewardRarity.Uncommon)
                        {
                            candidates = candidates.Take((veins.Length - 1) / 2); // Take 1st half
                        }
                        else // Rare
                        {
                            candidates = candidates.Skip((veins.Length - 1) / 2); // Skip 1st half. Take 2nd half
                        }

                        var random = Utility.RandomDouble();
                        vein = GetVeinFrom(candidates.ToList(), random);
                        break;
                    }
            }

            try
            {
                var type = vein.PrimaryResource.Types[0];
                var resource = system.Construct(type, npc);
                if (resource != null)
                {
                    resource.Amount = amount;
                    npc.AddToBackpack(resource);
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to generate resource. {0}", e.Message);
            }

            Console.WriteLine("Failed to generate {0} resource for {1}.", rarity.ToString(), system.GetType());

            return false;
        }

        private HarvestVein GetVeinFrom(List<HarvestVein> veins, double randomValue)
        {
            if (veins.Count == 1) return veins[0];

            randomValue *= 100; // VeinChance is in Percent Form

            while (true)
            {
                foreach (var vein in veins)
                {
                    if (randomValue <= vein.VeinChance) return vein;

                    randomValue -= vein.VeinChance;
                }
            }
        }
    }
}
