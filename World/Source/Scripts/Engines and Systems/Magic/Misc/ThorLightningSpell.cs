using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Magical
{
	public class ThorLightningSpell : MagicalSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"", "",
				239,
				9021
			);

		public override SpellCircle Circle { get { return SpellCircle.Eighth; } }
		public override double RequiredSkill{ get{ return 0.0; } }
		public override int RequiredMana{ get{ return 1; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.0 ); } }

		public ThorLightningSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return false; } }

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
				bool hitThrough = SpellHelper.ResolveMagicDefense( (int)this.OneBasedCircle, ref source, ref m );

				m.BoltEffect( 0 );

				if ( hitThrough )
				{
					double damage;

					if ( Core.AOS )
					{
						damage = GetNewAosDamage( 23, 1, 4, m );
					}
					else
					{
						damage = Utility.Random( 12, 9 );

						if ( CheckResisted( m ) )
						{
							damage *= 0.75;

							m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
						}

						damage *= GetDamageScalar( m );
					}

					SpellHelper.Damage( this, m, damage, 0, 0, 0, 0, 100 );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ThorLightningSpell m_Owner;

			public InternalTarget( ThorLightningSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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