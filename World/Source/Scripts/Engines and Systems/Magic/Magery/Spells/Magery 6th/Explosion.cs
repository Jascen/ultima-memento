using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Misc;

namespace Server.Spells.Sixth
{
	public class ExplosionSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Explosion", "Vas Ort Flam",
				230,
				9041,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot
			);

		public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

		public ExplosionSpell( Mobile caster, Item scroll )
			: base( caster, scroll, m_Info )
		{
		}

		public override bool DelayedDamageStacking { get { return !Core.AOS; } }

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage { get { return false; } }

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				Mobile attacker = Caster, defender = m;

				SpellHelper.Turn( Caster, m );

				Mobile source = Caster;
				bool hitThrough = SpellHelper.ResolveMagicDefense( (int) this.Circle, ref source, ref m );

				InternalTimer t = new InternalTimer( this, attacker, defender, m, hitThrough );
				t.Start();
			}

			FinishSequence();
		}

		private class InternalTimer : Timer
		{
			private MagerySpell m_Spell;
			private Mobile m_Target;
			private Mobile m_Attacker, m_Defender;
			private bool m_ApplyEffect;

			public InternalTimer( MagerySpell spell, Mobile attacker, Mobile defender, Mobile target, bool applyEffect )
				: base( TimeSpan.FromSeconds( Core.AOS ? 3.0 : 2.5 ) )
			{
				m_Spell = spell;
				m_Attacker = attacker;
				m_Defender = defender;
				m_Target = target;
				m_ApplyEffect = applyEffect;

				if ( m_Spell != null )
					m_Spell.StartDelayedDamageContext( attacker, this );

				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				if ( Utility.RandomBool() )
				{
					m_Target.FixedParticles( 0x36BD, 20, 10, 5044, PlayerSettings.GetMySpellHue( true, m_Attacker, 0 ), 0, EffectLayer.Head );
				}
				else
				{
					Effects.SendLocationEffect( m_Target.Location, m_Target.Map, 0x3822, 60, 10, PlayerSettings.GetMySpellHue( true, m_Attacker, 0 ), 0 );
				}
				m_Target.PlaySound( 0x307 );

				if ( m_ApplyEffect && m_Attacker.HarmfulCheck( m_Defender ) )
				{
					int nBenefit = 0;
					if ( m_Attacker is PlayerMobile )
						nBenefit = (int)(m_Attacker.Skills[SkillName.Magery].Value / 5);

					double damage = m_Spell.GetNewAosDamage( 40, 1, 5, m_Defender ) + nBenefit;

					SpellHelper.Damage( m_Spell, m_Target, damage, 0, 100, 0, 0, 0 );
				}

				if ( m_Spell != null )
					m_Spell.RemoveDelayedDamageContext( m_Attacker );
			}
		}

		private class InternalTarget : Target
		{
			private ExplosionSpell m_Owner;

			public InternalTarget( ExplosionSpell owner )
				: base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile) o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}