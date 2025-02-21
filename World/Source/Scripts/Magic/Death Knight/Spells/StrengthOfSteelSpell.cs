using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.DeathKnight
{
	public class StrengthOfSteelSpell : DeathKnightSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Strength of Steel", "Volac Fortitudo",
				212,
				9061
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1 ); } }
		public override int RequiredTithing{ get{ return 28; } }
		public override double RequiredSkill{ get{ return 20.0; } }
		public override int RequiredMana{ get{ return 20; } }

		public StrengthOfSteelSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
			else if ( CheckBSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				int bonus = MyServerSettings.PlayerLevelMod( (int)( GetKarmaPower( m ) / 3 ), Caster );
				double timer = ( GetKarmaPower( m ) / 10 );
				SpellHelper.AddStatBonus( Caster, m, StatType.Str, bonus, TimeSpan.FromMinutes( timer ) );

				m.PlaySound( 0x1EB );
				m.FixedParticles( 0x373A, 10, 15, 5018, EffectLayer.Waist );

				string args = String.Format("{0}", bonus);

				BuffInfo.RemoveBuff( Caster, BuffIcon.StrengthOfSteel );
				BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.StrengthOfSteel, 1063557, 1063558, TimeSpan.FromMinutes( timer ), Caster, args.ToString(), true));

				DrainSoulsInLantern( Caster, RequiredTithing );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private StrengthOfSteelSpell m_Owner;

			public InternalTarget( StrengthOfSteelSpell owner ) : base( 12, false, TargetFlags.Beneficial )
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
