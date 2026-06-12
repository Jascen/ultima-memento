using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Misc;

namespace Server.Spells.Second
{
	public class HarmSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Harm", "An Mani",
				212,
				Core.AOS ? 9001 : 9041,
				Reagent.Nightshade,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public HarmSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return false; } }

		public override double GetSlayerDamageScalar( Mobile target )
		{
			return 1.0; //This spell isn't affected by slayer spellbooks
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Mobile source = Caster;
				if ( SpellHelper.ResolveMagicDefense( (int)this.OneBasedCircle, ref source, ref m ) )
				{
					int nBenefit = 0;
					if ( Caster is PlayerMobile )
					{
						nBenefit = (int)( Spell.ItemSkillValue( Caster, SkillName.Magery, false ) / 5 );
					}
					
					double damage = GetNewAosDamage( 17, 1, 5, m ) + nBenefit;

					if ( !m.InRange( Caster, 2 ) )
						damage *= 0.25; // 1/4 damage at > 2 tile range
					else if ( !m.InRange( Caster, 1 ) )
						damage *= 0.50; // 1/2 damage at 2 tile range

					m.FixedParticles( 0x374A, 10, 30, 5013, PlayerSettings.GetMySpellHue( true, Caster, 0 ), 2, EffectLayer.Waist );
					m.PlaySound( 0x0FC );

					SpellHelper.Damage( this, m, damage, 0, 0, 100, 0, 0 );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private HarmSpell m_Owner;

			public InternalTarget( HarmSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}