using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Misc;

namespace Server.Spells.Third
{
	public class PoisonSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Poison", "In Nox",
				203,
				9051,
				Reagent.Nightshade
			);

		public override SpellCircle Circle { get { return SpellCircle.Third; } }

		public PoisonSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
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
				if ( SpellHelper.ResolveMagicDefense( (int)this.Circle, ref source, ref m ) )
				{
					if ( m.Spell != null )
						m.Spell.OnCasterHurt();

					m.Paralyzed = false;
					BuffInfo.CleanupIcons( m, true );

					if ( CheckResisted( m ) )
					{
						m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
					}
					else
					{
						int level;

						if ( Caster.InRange( m, 2 ) )
						{
							int total = (int)( Spell.ItemSkillValue( Caster, SkillName.Magery, false ) + Caster.Skills[SkillName.Poisoning].Value );

							if ( total >= 250 )
								level = 4;
							else if ( total >= 200 )
								level = 3;
							else if ( total > 150 )
								level = 2;
							else if ( total > 100 )
								level = 1;
							else
								level = 0;
						}
						else
						{
							level = 0;
						}

						m.ApplyPoison( Caster, Poison.GetPoison( level ) );
					}

					m.FixedParticles( 0x374A, 10, 15, 5021, PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.Waist );
					m.PlaySound( 0x205 );

					HarmfulSpell( m );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private PoisonSpell m_Owner;

			public InternalTarget( PoisonSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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