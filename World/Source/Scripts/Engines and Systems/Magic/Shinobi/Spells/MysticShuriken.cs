using System;
using Server.Targeting;

namespace Server.Spells.Shinobi
{
	public class MysticShuriken : ShinobiSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 296,
					IconGraphic = 10877,
					Name = "Mystic Shuriken",
					PowerWords = "Misutikku Shuriken",
					Description = "Summons a shuriken out of thin air and hurdles it toward your opponent, causing much damage from afar.",
					ManaCost = 40,
					TithingCost = 45,
					MinSkill = 60,
					TargetType = TargetFlags.Harmful
				},
				-1,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public MysticShuriken( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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

				SpellHelper.Turn( Caster, m );

				double damage = GetNewAosDamage( 40, 1, 5, m );

				// Do the effects
				source.MovingParticles( m, 0x27AC, 7, 0, false, false, 0, 0, 0 );
				source.PlaySound( 0x5D2 );

				// Deal the damage
				SpellHelper.Damage( this, m, damage, 100, 0, 0, 0, 0 );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private MysticShuriken m_Owner;

			public InternalTarget( MysticShuriken owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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