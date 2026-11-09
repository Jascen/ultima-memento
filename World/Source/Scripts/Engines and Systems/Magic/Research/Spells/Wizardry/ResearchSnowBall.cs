using System;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchSnowBall : ResearchSpell
	{
		public int CirclePower = 3;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 649,
					IconGraphic = 11252,
					Name = "Snow Ball",
					PowerWords = "Ex Des In",
					Description = "Throws a magical snow ball toward your enemy, which can cause 2-80 points of cold damage.",
					ManaCost = 10,
					MinSkill = 10,
					TargetType = TargetFlags.Harmful
				},
				236,
				9031,
				Reagent.BlackPearl,Reagent.MoonCrystal
			);

		public ResearchSnowBall( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return true; } }

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				Mobile source = Caster;

				SpellHelper.Turn( source, m );

				bool hitThrough = SpellHelper.ResolveMagicDefense( CirclePower, ref source, ref m );

				source.MovingParticles( m, 0x36E4, 7, 0, false, true, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xBB3 ), 0, 9502, 4019, 0x160, 0 );
				source.PlaySound( 0x650 );

				if ( hitThrough )
				{
					double damage = DamagingSkill( Caster )/3;
						if ( damage > 80 ){ damage = 80.0; }
						if ( damage < 2 ){ damage = 2.0; }

					SpellHelper.Damage( this, m, damage, 0, 0, 100, 0, 0 );
					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ResearchSnowBall m_Owner;

			public InternalTarget( ResearchSnowBall owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}