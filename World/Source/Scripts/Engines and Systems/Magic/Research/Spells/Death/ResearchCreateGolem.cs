using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchCreateGolem : ResearchSpell
	{
		public int CirclePower = 7;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.25 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 613,
					IconGraphic = 11208,
					Name = "Create Golem",
					PowerWords = "In Ort Ylem Xen",
					Description = "Creates a mighty flesh golem that will follow the commands of the cast for 8-25 minutes. The caster may lose some karma if they attempt this.",
					ManaCost = 40,
					MinSkill = 70,
					TargetType = TargetFlags.None
				},
				269,
				9050,
				Reagent.PixieSkull,Reagent.GraveDust,Reagent.DaemonBlood
			);

		public ResearchCreateGolem( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if ( (Caster.Followers + 3) > Caster.FollowersMax )
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

				if ( DamagingSkill( Caster ) > Utility.RandomMinMax( 60, 300 ) )
				{
					BaseCreature m_Creature = new AncientFleshGolem();
					m_Creature.ControlSlots = 3;
					SpellHelper.Summon( m_Creature, Caster, 0x216, duration, false, false );
					m_Creature.FixedParticles(0x3728, 8, 20, 5042, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.Head );
				}
				else
				{
					BaseCreature m_Creature = new FleshGolem();
					m_Creature.ControlSlots = 3;
					SpellHelper.Summon( m_Creature, Caster, 0x216, duration, false, false );
					m_Creature.FixedParticles(0x3728, 8, 20, 5042, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.Head );
				}
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );

				KarmaMod( Caster, ((int)RequiredSkill+RequiredMana) );
			}

			FinishSequence();
		}
	}
}