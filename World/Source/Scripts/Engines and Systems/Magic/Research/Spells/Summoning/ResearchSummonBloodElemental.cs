using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchSummonBloodElemental : ResearchSpell
	{
		public int CirclePower = 8;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.25 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 651,
					IconGraphic = 11234,
					Name = "Blood Elemental",
					PowerWords = "Uus Mani",
					Description = "Summons a blood elemental that is controlled by the caster for 8-25 minutes.",
					ManaCost = 50,
					MinSkill = 90,
					TargetType = TargetFlags.None
				},
				269,
				9050,
				Reagent.BlackPearl,Reagent.Bloodmoss,Reagent.DragonBlood,Reagent.DaemonBlood
			);

		public ResearchSummonBloodElemental( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if ( (Caster.Followers + 4) > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1049645 ); // You have too many followers to summon that creature.
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				double time = DamagingSkill( Caster )*6;
					if ( time > 1500 ){ time = 1500.0; }
					if ( time < 480 ){ time = 480.0; }

				TimeSpan duration = TimeSpan.FromSeconds( time );

				BaseCreature m_Creature = new BloodElemental();
				m_Creature.ControlSlots = 5;
				SpellHelper.Summon( m_Creature, Caster, 0x216, duration, false, false );
				m_Creature.FixedParticles(0x3728, 8, 20, 5042, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.Head );
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
			}

			FinishSequence();
		}
	}
}