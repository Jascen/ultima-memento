using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchSummonDead : ResearchSpell
	{
		public int CirclePower = 7;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.25 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 653,
					IconGraphic = 11206,
					Name = "Animate Bones",
					PowerWords = "Kal Corp Xen",
					Description = "Animates the bones of a mighty skeletal warrior that will fight for the caster for 8-25 minutes. The caster may lose some karma if they attempt this.",
					ManaCost = 40,
					MinSkill = 70,
					TargetType = TargetFlags.None
				},
				269,
				9050,
				Reagent.BatWing,Reagent.GraveDust,Reagent.PixieSkull
			);

		public ResearchSummonDead( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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

				BaseCreature m_Creature = new BoneKnight();
					m_Creature.ControlSlots = 3;
					m_Creature.Body = 327;
					m_Creature.Hue = 2500;
				SpellHelper.Summon( m_Creature, Caster, 0x216, duration, false, false );
				m_Creature.FixedParticles(0x3728, 8, 20, 5042, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.Head );
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );

				KarmaMod( Caster, ((int)RequiredSkill+RequiredMana) );
			}

			FinishSequence();
		}
	}
}