using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchSummonDevil : ResearchSpell
	{
		public int CirclePower = 8;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.25 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 654,
					IconGraphic = 11241,
					Name = "Invoke Devil",
					PowerWords = "Uus Flam Corp Xen",
					Description = "Calls forth a devil from the realms of hell to serve the caster for 2-8 minutes. The caster may lose some karma if they attempt this.",
					ManaCost = 60,
					MinSkill = 95,
					TargetType = TargetFlags.None
				},
				269,
				9050,
				Reagent.DaemonBlood,Reagent.EyeOfToad,Reagent.Brimstone,Reagent.DemonClaw
			);

		public ResearchSummonDevil( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if ( (Caster.Followers + 5 ) > Caster.FollowersMax )
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
				double time = DamagingSkill( Caster )*2;
					if ( time > 480 ){ time = 480.0; }
					if ( time < 120 ){ time = 120.0; }

				TimeSpan duration = TimeSpan.FromSeconds( time );

				BaseCreature m_Devil = new Devil();
				m_Devil.ControlSlots = 5;
				SpellHelper.Summon( m_Devil, Caster, 0x216, duration, false, false );
				m_Devil.FixedParticles(0x3728, 8, 20, 5042, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.Head );
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );

				KarmaMod( Caster, ((int)RequiredSkill+RequiredMana) );
			}

			FinishSequence();
		}
	}
}