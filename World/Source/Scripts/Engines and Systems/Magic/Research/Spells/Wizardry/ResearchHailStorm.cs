using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.Spells.Research
{
	public class ResearchHailStorm : ResearchSpell
	{
		public override int spellIndex { get { return 48; } }
		public int CirclePower = 6;
		public static int spellID = 48;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.75 ); } }
		public override double RequiredSkill{ get{ return (double)(Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 8 ))); } }
		public override int RequiredMana{ get{ return Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 7 )); } }

		private static SpellInfo m_Info = new SpellInfo(
				Server.Misc.Research.SpellInformation( spellID, 2 ),
				Server.Misc.Research.CapsCast( Server.Misc.Research.SpellInformation( spellID, 4 ) ),
				230,
				9041,
				Reagent.FairyEgg,Reagent.MoonCrystal
			);

		public ResearchHailStorm( Mobile caster, Item scroll ): base( caster, scroll, m_Info )
		{
		}

		public override bool DelayedDamageStacking { get { return !Core.AOS; } }

		public override void OnCast()
		{
			Caster.SendMessage( "Choose who you want to unleaseh this storm on." );
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
				bool hitThrough = SpellHelper.ResolveMagicDefense( (int) CirclePower, ref source, ref m );

				InternalTimer t = new InternalTimer( this, attacker, defender, m, hitThrough );
				t.Start();

				if ( hitThrough )
					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
			}

			FinishSequence();
		}

		private class InternalTimer : Timer
		{
			private ResearchSpell m_Spell;
			private Mobile m_Target;
			private Mobile m_Attacker, m_Defender;
			private bool m_ApplyEffect;

			public InternalTimer( ResearchSpell spell, Mobile attacker, Mobile defender, Mobile target, bool applyEffect ): base( TimeSpan.FromSeconds( Core.AOS ? 3.0 : 2.5 ) )
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
				m_Target.FixedParticles( 0x55BB, 10, 30, 5052, Server.Misc.PlayerSettings.GetMySpellHue( true, m_Attacker, 0 ), 0, EffectLayer.LeftFoot );
				m_Target.PlaySound( 0x5CE );

				if ( m_ApplyEffect && m_Attacker.HarmfulCheck( m_Defender ) )
				{
					double damage = DamagingSkill( m_Attacker )/2;
						if ( damage > 125 ){ damage = 125.0; }
						if ( damage < 28 ){ damage = 28.0; }

					SpellHelper.Damage( m_Spell, m_Target, damage, 0, 0, 100, 0, 0 );
				}

				if ( m_Spell != null )
					m_Spell.RemoveDelayedDamageContext( m_Attacker );
			}
		}

		private class InternalTarget : Target
		{
			private ResearchHailStorm m_Owner;

			public InternalTarget( ResearchHailStorm owner ): base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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