using Server.Misc;
using System.Collections.Generic;

namespace Server.Spells.Research
{
	public class ResearchSpellProvider
	{
		public static readonly List<SpellDefinition> SpellDefinitions = new List<SpellDefinition>
		{
			ResearchAerialServant.SpellInfo.SpellDefinition,
			ResearchAirWalk.SpellInfo.SpellDefinition,
			ResearchAvalanche.SpellInfo.SpellDefinition,
			ResearchBanishDaemon.SpellInfo.SpellDefinition,
			ResearchCallDestruction.SpellInfo.SpellDefinition,
			ResearchCauseFear.SpellInfo.SpellDefinition,
			ResearchCharm.SpellInfo.SpellDefinition,
			ResearchClone.SpellInfo.SpellDefinition,
			ResearchConflagration.SpellInfo.SpellDefinition,
			ResearchConfusionBlast.SpellInfo.SpellDefinition,
			ResearchConjure.SpellInfo.SpellDefinition,
			ResearchCreateFire.SpellInfo.SpellDefinition,
			ResearchCreateGold.SpellInfo.SpellDefinition,
			ResearchCreateGolem.SpellInfo.SpellDefinition,
			ResearchDeathSpeak.SpellInfo.SpellDefinition,
			ResearchDeathVortex.SpellInfo.SpellDefinition,
			ResearchDevastation.SpellInfo.SpellDefinition,
			ResearchDivination.SpellInfo.SpellDefinition,
			ResearchEnchant.SpellInfo.SpellDefinition,
			ResearchEndureCold.SpellInfo.SpellDefinition,
			ResearchEndureHeat.SpellInfo.SpellDefinition,
			ResearchGasCloud.SpellInfo.SpellDefinition,
			ResearchEtherealTravel.SpellInfo.SpellDefinition,
			ResearchExplosion.SpellInfo.SpellDefinition,
			ResearchExtinguish.SpellInfo.SpellDefinition,
			ResearchFadefromSight.SpellInfo.SpellDefinition,
			ResearchFlameBolt.SpellInfo.SpellDefinition,
			ResearchFrostField.SpellInfo.SpellDefinition,
			ResearchFrostStrike.SpellInfo.SpellDefinition,
			ResearchGrantPeace.SpellInfo.SpellDefinition,
			ResearchHailStorm.SpellInfo.SpellDefinition,
			ResearchHealingTouch.SpellInfo.SpellDefinition,
			ResearchIcicle.SpellInfo.SpellDefinition,
			ResearchIgnite.SpellInfo.SpellDefinition,
			ResearchIntervention.SpellInfo.SpellDefinition,
			ResearchMagicSteed.SpellInfo.SpellDefinition,
			ResearchMaskofDeath.SpellInfo.SpellDefinition,
			ResearchMassDeath.SpellInfo.SpellDefinition,
			ResearchMassMight.SpellInfo.SpellDefinition,
			ResearchMassSleep.SpellInfo.SpellDefinition,
			ResearchMeteorShower.SpellInfo.SpellDefinition,
			ResearchOpenGround.SpellInfo.SpellDefinition,
			ResearchRestoration.SpellInfo.SpellDefinition,
			ResearchRingofFire.SpellInfo.SpellDefinition,
			ResearchRockFlesh.SpellInfo.SpellDefinition,
			ResearchSeeTruth.SpellInfo.SpellDefinition,
			ResearchSleep.SpellInfo.SpellDefinition,
			ResearchSleepField.SpellInfo.SpellDefinition,
			ResearchSneak.SpellInfo.SpellDefinition,
			ResearchSnowBall.SpellInfo.SpellDefinition,
			ResearchSummonAcidElemental.SpellInfo.SpellDefinition,
			ResearchSummonBloodElemental.SpellInfo.SpellDefinition,
			ResearchSummonCreature.SpellInfo.SpellDefinition,
			ResearchSummonDead.SpellInfo.SpellDefinition,
			ResearchSummonDevil.SpellInfo.SpellDefinition,
			ResearchSummonElectricalElemental.SpellInfo.SpellDefinition,
			ResearchSummonGemElemental.SpellInfo.SpellDefinition,
			ResearchSummonIceElemental.SpellInfo.SpellDefinition,
			ResearchSummonMudElemental.SpellInfo.SpellDefinition,
			ResearchSummonPoisonElemental.SpellInfo.SpellDefinition,
			ResearchSummonWeedElemental.SpellInfo.SpellDefinition,
			ResearchSwarm.SpellInfo.SpellDefinition,
			ResearchWithstandDeath.SpellInfo.SpellDefinition,
			ResearchWizardEye.SpellInfo.SpellDefinition,
		};

		// Research bags, scrolls and spell bars refer to spells by research index (1-based, ordered by circle then school)
		private static readonly List<SpellDefinition> m_ResearchOrder = new List<SpellDefinition>
		{
			ResearchConjure.SpellInfo.SpellDefinition,
			ResearchDeathSpeak.SpellInfo.SpellDefinition,
			ResearchSneak.SpellInfo.SpellDefinition,
			ResearchCreateFire.SpellInfo.SpellDefinition,
			ResearchSummonElectricalElemental.SpellInfo.SpellDefinition,
			ResearchConfusionBlast.SpellInfo.SpellDefinition,
			ResearchSeeTruth.SpellInfo.SpellDefinition,
			ResearchIcicle.SpellInfo.SpellDefinition,
			ResearchExtinguish.SpellInfo.SpellDefinition,
			ResearchRockFlesh.SpellInfo.SpellDefinition,
			ResearchMassMight.SpellInfo.SpellDefinition,
			ResearchEndureCold.SpellInfo.SpellDefinition,
			ResearchSummonWeedElemental.SpellInfo.SpellDefinition,
			ResearchSummonCreature.SpellInfo.SpellDefinition,
			ResearchHealingTouch.SpellInfo.SpellDefinition,
			ResearchSnowBall.SpellInfo.SpellDefinition,
			ResearchClone.SpellInfo.SpellDefinition,
			ResearchGrantPeace.SpellInfo.SpellDefinition,
			ResearchSleep.SpellInfo.SpellDefinition,
			ResearchEndureHeat.SpellInfo.SpellDefinition,
			ResearchSummonIceElemental.SpellInfo.SpellDefinition,
			ResearchEtherealTravel.SpellInfo.SpellDefinition,
			ResearchWizardEye.SpellInfo.SpellDefinition,
			ResearchFrostField.SpellInfo.SpellDefinition,
			ResearchCreateGold.SpellInfo.SpellDefinition,
			ResearchSummonDead.SpellInfo.SpellDefinition,
			ResearchCauseFear.SpellInfo.SpellDefinition,
			ResearchIgnite.SpellInfo.SpellDefinition,
			ResearchSummonMudElemental.SpellInfo.SpellDefinition,
			ResearchBanishDaemon.SpellInfo.SpellDefinition,
			ResearchFadefromSight.SpellInfo.SpellDefinition,
			ResearchGasCloud.SpellInfo.SpellDefinition,
			ResearchSwarm.SpellInfo.SpellDefinition,
			ResearchMaskofDeath.SpellInfo.SpellDefinition,
			ResearchEnchant.SpellInfo.SpellDefinition,
			ResearchFlameBolt.SpellInfo.SpellDefinition,
			ResearchSummonGemElemental.SpellInfo.SpellDefinition,
			ResearchCallDestruction.SpellInfo.SpellDefinition,
			ResearchDivination.SpellInfo.SpellDefinition,
			ResearchFrostStrike.SpellInfo.SpellDefinition,
			ResearchMagicSteed.SpellInfo.SpellDefinition,
			ResearchCreateGolem.SpellInfo.SpellDefinition,
			ResearchSleepField.SpellInfo.SpellDefinition,
			ResearchConflagration.SpellInfo.SpellDefinition,
			ResearchSummonAcidElemental.SpellInfo.SpellDefinition,
			ResearchMeteorShower.SpellInfo.SpellDefinition,
			ResearchIntervention.SpellInfo.SpellDefinition,
			ResearchHailStorm.SpellInfo.SpellDefinition,
			ResearchAerialServant.SpellInfo.SpellDefinition,
			ResearchOpenGround.SpellInfo.SpellDefinition,
			ResearchCharm.SpellInfo.SpellDefinition,
			ResearchExplosion.SpellInfo.SpellDefinition,
			ResearchSummonPoisonElemental.SpellInfo.SpellDefinition,
			ResearchSummonDevil.SpellInfo.SpellDefinition,
			ResearchAirWalk.SpellInfo.SpellDefinition,
			ResearchAvalanche.SpellInfo.SpellDefinition,
			ResearchDeathVortex.SpellInfo.SpellDefinition,
			ResearchWithstandDeath.SpellInfo.SpellDefinition,
			ResearchMassSleep.SpellInfo.SpellDefinition,
			ResearchRingofFire.SpellInfo.SpellDefinition,
			ResearchSummonBloodElemental.SpellInfo.SpellDefinition,
			ResearchDevastation.SpellInfo.SpellDefinition,
			ResearchRestoration.SpellInfo.SpellDefinition,
			ResearchMassDeath.SpellInfo.SpellDefinition,
		};

		public static int FirstSpellId
		{ get { return SpellDefinitions[0].SpellID; } }

		public static int LastSpellId
		{ get { return SpellDefinitions[SpellDefinitions.Count - 1].SpellID; } }

		public static int SpellCount
		{ get { return SpellDefinitions.Count; } }

		public static void Cast(Mobile from, int spellID)
		{
			if (ResearchSettings.ResearchMaterials(from) == null) return;
			if (!HasSpell(from, spellID)) return;
			if (!Multis.DesignContext.Check(from)) return;

			Spell spell = SpellRegistry.NewSpell(spellID, from, null);
			if (spell != null)
				spell.Cast();
		}

		public static SpellDefinition GetDefinition(int spellID)
		{
			if (spellID < FirstSpellId || spellID > LastSpellId)
				return null;

			return SpellDefinitions[spellID - FirstSpellId];
		}

		public static SpellDefinition GetDefinitionByResearchIndex(int researchIndex)
		{
			if (researchIndex < 1 || researchIndex > m_ResearchOrder.Count)
				return null;

			return m_ResearchOrder[researchIndex - 1];
		}

		public static int GetResearchIndex(int spellID)
		{
			SpellDefinition definition = GetDefinition(spellID);

			return definition == null ? 0 : m_ResearchOrder.IndexOf(definition) + 1;
		}

		public static bool HasSpell(Mobile from, int spellID)
		{
			int researchIndex = GetResearchIndex(spellID);

			return researchIndex > 0 && ResearchSettings.HasSpell(from, researchIndex);
		}
	}
}