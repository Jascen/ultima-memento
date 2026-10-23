using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.Spells.Jedi
{
    public class SoothingTouch : JediSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 287,
					IconGraphic = 0x2BCD,
					Name = "Soothing Touch",
					PowerWords = "Kurklan",
					Description = "The Jedi can heal themselves and others. With a high enough level of Jedi power, they could potentially cure poison, stop bleeding, and heal mortal wounds.",
					ManaCost = 32,
					TithingCost = 48,
					MinSkill = 10,
					TargetType = TargetFlags.Beneficial
				},
				204,
				0
			);

		public int CirclePower = 1;
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

        public SoothingTouch(Mobile caster, Item scroll) : base(caster, scroll, SpellInfo)
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
			else if ( m is BaseCreature && ((BaseCreature)m).IsAnimatedDead )
			{
				Caster.SendLocalizedMessage( 1061654 ); // You cannot heal that which is not alive.
			}
			else if ( m.IsDeadBondedPet )
			{
				Caster.SendLocalizedMessage( 1060177 ); // You cannot heal a creature that is already dead!
			}
			else if ( m is Golem )
			{
				Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 500951 ); // You cannot heal that.
			}
			else if ( MortalStrike.IsWounded( m ) )
			{
				if ( GetJediDamage( Caster ) > Utility.RandomMinMax( 185, 750 ) )
				{
					MortalStrike.EndWound( m );
				}
				else
				{
					Caster.LocalOverheadMessage( MessageType.Regular, 0x22, (Caster == m) ? 1005000 : 1010398 );
				}
			}
			else if ( m.Poisoned )
			{
				double healing = Caster.Skills[SkillName.Psychology].Value;
				double anatomy = (double)( GetJediDamage( Caster ) / 2 );
				double chance = ((healing - 30.0) / 50.0) - (m.Poison.Level * 0.1);

				if ( healing >= 60.0 && anatomy >= 60.0 && chance > Utility.RandomDouble() )
				{
					if ( m.CurePoison( Caster ) )
					{
						Caster.SendLocalizedMessage( 1010058 );
						if ( Caster != m ){ m.SendLocalizedMessage( 1010059 ); }
					}
				}
				else
				{
					Caster.LocalOverheadMessage( MessageType.Regular, 0x22, 1010060 );
				}
			}
			else if ( BleedAttack.IsBleeding( m ) )
			{
				if ( GetJediDamage( Caster ) > Utility.RandomMinMax( 185, 750 ) )
				{
					BleedAttack.EndBleed( m, false );
				}
				else
				{
					Caster.LocalOverheadMessage( MessageType.Regular, 0x22, 1060159 );
				}
			}
			else if ( CheckBSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				int toHeal = (int)( Caster.Skills[SkillName.Psychology].Value * 0.2 ) + (int)( GetJediDamage( Caster ) * 0.1 );
				toHeal += Utility.Random( 1, 10 );
				toHeal = MyServerSettings.PlayerLevelMod( toHeal, Caster );

				SpellHelper.Heal( toHeal, m, Caster );

				m.FixedParticles( 0x376A, 9, 32, 5030, 0xB41, 0, EffectLayer.Waist );
				m.PlaySound( 0x202 );
			}
		}

		public class InternalTarget : Target
		{
			private SoothingTouch m_Owner;

			public InternalTarget( SoothingTouch owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Beneficial )
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