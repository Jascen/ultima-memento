using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Spells.Research
{
	public class ResearchHealingTouch : ResearchSpell
	{
		public int CirclePower = 4;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.25 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 631,
					IconGraphic = 11244,
					Name = "Healing Touch",
					PowerWords = "In Mani",
					Description = "Heals the one touched, restoring 12-125 hit points. You need to be close to your target, however.",
					ManaCost = 15,
					MinSkill = 30,
					TargetType = TargetFlags.Beneficial
				},
				204,
				9061,
				Reagent.Garlic,Reagent.Ginseng,Reagent.FairyEgg,Reagent.PixieSkull
			);

		public ResearchHealingTouch( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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
            else if ( !Caster.InRange( m, 2 ) )
            {
                Caster.SendLocalizedMessage( 501042 ); // Target is not close enough.
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
			else if ( m.Poisoned || Server.Items.MortalStrike.IsWounded( m ) )
			{
				Caster.LocalOverheadMessage( MessageType.Regular, 0x22, (Caster == m) ? 1005000 : 1010398 );
			}
			else if ( CheckBSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				int toHeal = (int)(DamagingSkill( Caster )/2);
					if ( toHeal > 125 ){ toHeal = 125; }
					if ( toHeal < 12 ){ toHeal = 12; }

				SpellHelper.Heal( toHeal, m, Caster );

				m.FixedParticles( 0x376A, 9, 32, 5030, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.Waist );
				m.PlaySound( 0x202 );
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private ResearchHealingTouch m_Owner;

			public InternalTarget( ResearchHealingTouch owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Beneficial )
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