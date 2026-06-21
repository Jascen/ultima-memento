using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Spells.Elementalism
{
	public class Elemental_Storm_Spell : ElementalSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Elemental Storm", "Furtuna",
				230,
				9041
			);

		public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

		public Elemental_Storm_Spell( Mobile caster, Item scroll ): base( caster, scroll, m_Info )
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
			private ElementalSpell m_Spell;
			private Mobile m_Target;
			private Mobile m_Attacker, m_Defender;
			private bool m_ApplyEffect;

			public InternalTimer( ElementalSpell spell, Mobile attacker, Mobile defender, Mobile target, bool applyEffect ): base( TimeSpan.FromSeconds( Core.AOS ? 3.0 : 2.5 ) )
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
				string elm = ElementalSpell.GetElement( m_Attacker );

				Point3D blast1w = new Point3D( ( m_Target.X ), ( m_Target.Y ), m_Target.Z );
				Point3D blast2w = new Point3D( ( m_Target.X-1 ), ( m_Target.Y ), m_Target.Z );
				Point3D blast3w = new Point3D( ( m_Target.X+1 ), ( m_Target.Y ), m_Target.Z );
				Point3D blast4w = new Point3D( ( m_Target.X ), ( m_Target.Y-1 ), m_Target.Z );
				Point3D blast5w = new Point3D( ( m_Target.X ), ( m_Target.Y+1 ), m_Target.Z );

				if ( elm == "air" )
				{
					Effects.SendLocationEffect( blast1w, m_Target.Map, 0x5590, 30, 10, 0xB24, 0 );
					Effects.SendLocationEffect( blast2w, m_Target.Map, 0x5590, 30, 10, 0xB24, 0 );
					Effects.SendLocationEffect( blast3w, m_Target.Map, 0x5590, 30, 10, 0xB24, 0 );
					Effects.SendLocationEffect( blast4w, m_Target.Map, 0x5590, 30, 10, 0xB24, 0 );
					Effects.SendLocationEffect( blast5w, m_Target.Map, 0x5590, 30, 10, 0xB24, 0 );
					m_Target.PlaySound( 0x654 );
				}
				else if ( elm == "earth" )
				{
					Effects.SendLocationEffect( blast1w, m_Target.Map, 0x54F4, 30, 10, 0, 0 );
					Effects.SendLocationEffect( blast2w, m_Target.Map, 0x54F4, 30, 10, 0, 0 );
					Effects.SendLocationEffect( blast3w, m_Target.Map, 0x54F4, 30, 10, 0, 0 );
					Effects.SendLocationEffect( blast4w, m_Target.Map, 0x54F4, 30, 10, 0, 0 );
					Effects.SendLocationEffect( blast5w, m_Target.Map, 0x54F4, 30, 10, 0, 0 );
					m_Target.PlaySound( 0x10B );
				}
				else if ( elm == "fire" )
				{
					Point3D blast7w = new Point3D( ( m_Target.X+2 ), ( m_Target.Y+2 ), m_Target.Z+12 );
					Point3D blast8w = new Point3D( ( m_Target.X ), ( m_Target.Y+2 ), m_Target.Z+12 );
					Point3D blast9w = new Point3D( ( m_Target.X+2 ), ( m_Target.Y ), m_Target.Z+12 );
					Effects.SendLocationEffect( blast7w, m_Target.Map, 0x23B2, 60, 10, 0xB71-1, 0 );
					Effects.SendLocationEffect( blast8w, m_Target.Map, 0x23B2, 60, 10, 0xB71-1, 0 );
					Effects.SendLocationEffect( blast9w, m_Target.Map, 0x23B2, 60, 10, 0xB71-1, 0 );
					m_Target.PlaySound( 0x656 );
				}
				else if ( elm == "water" )
				{
					Point3D blast6w = new Point3D( ( m_Target.X+2 ), ( m_Target.Y+2 ), m_Target.Z+15 );
					Effects.SendLocationEffect( blast6w, m_Target.Map, 0x5492, 30, 10, 0xB75-1, 0 );
					m_Target.PlaySound( 0x64F );
				}

				if ( m_ApplyEffect && m_Attacker.HarmfulCheck( m_Defender ) )
				{
					int nBenefit = (int)(m_Attacker.Skills[SkillName.Elementalism].Value / 5);
					double damage = m_Spell.GetNewAosDamage( 40, 1, 5, m_Defender ) + nBenefit;

					if ( elm == "air" )
						SpellHelper.Damage( m_Spell, m_Target, damage, 30, 0, 0, 0, 70 );
					else if ( elm == "earth" )
						SpellHelper.Damage( m_Spell, m_Target, damage, 50, 0, 0, 50, 0 );
					else if ( elm == "fire" )
						SpellHelper.Damage( m_Spell, m_Target, damage, 30, 70, 0, 0, 0 );
					else if ( elm == "water" )
					{
						ElementalSpell.AddWater( m_Target );
						SpellHelper.Damage( m_Spell, m_Target, damage, 30, 0, 70, 0, 0 );
					}
				}

				if ( m_Spell != null )
					m_Spell.RemoveDelayedDamageContext( m_Attacker );
			}
		}

		private class InternalTarget : Target
		{
			private Elemental_Storm_Spell m_Owner;

			public InternalTarget( Elemental_Storm_Spell owner ): base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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