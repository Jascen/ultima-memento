using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Spells.DeathKnight
{
	public class DemonicTouchSpell : DeathKnightSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Demonic Touch", "Raum Curare",
				224,
				9061
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1 ); } }
		public override int RequiredTithing{ get{ return 21; } }
		public override double RequiredSkill{ get{ return 15.0; } }
		public override int RequiredMana{ get{ return 16; } }

		public DemonicTouchSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
			else if ( CheckBSequence( m, false ) )
			{
				SpellHelper.Turn( Caster, m );

				m.PlaySound( 0x202 );
				m.FixedParticles( 0x376A, 1, 62, 0x480, 3, 3, EffectLayer.Waist );
				m.FixedParticles( 0x37C4, 1, 46, 0x481, 5, 3, EffectLayer.Waist );

				double toHeal = GetKarmaPower( Caster ) / 2;
				int heal = MyServerSettings.PlayerLevelMod( (int)toHeal, Caster );
				m.Heal( heal );
				DrainSoulsInLantern( Caster, RequiredTithing );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private DemonicTouchSpell m_Owner;

			public InternalTarget( DemonicTouchSpell owner ) : base( 12, false, TargetFlags.Beneficial )
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
