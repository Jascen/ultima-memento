using System;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Spells.Research
{
	public class ResearchRestoration : ResearchSpell
	{
		public int CirclePower = 8;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.25 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 642,
					IconGraphic = 11250,
					Name = "Restoration",
					PowerWords = "Vas In Mani",
					Description = "This spell can restore 12-125 points in health, stamina, and mana.",
					ManaCost = 50,
					MinSkill = 80,
					TargetType = TargetFlags.Beneficial
				},
				204,
				9061,
				Reagent.ButterflyWings,Reagent.Garlic,Reagent.EnchantedSeaweed
			);

		public ResearchRestoration( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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
				Caster.SendMessage( "This spell will have no affect on that." );
			}
			else if ( m.IsDeadBondedPet )
			{
				Caster.SendMessage( "This spell will have no affect on that." );
			}
			else if ( m is Golem )
			{
				Caster.SendMessage( "This spell will have no affect on that." );
			}
			else if ( CheckBSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				int toHeal = (int)(DamagingSkill( Caster )/2);
					if ( toHeal > 125 ){ toHeal = 125; }
					if ( toHeal < 12 ){ toHeal = 12; }

				SpellHelper.Heal( toHeal, m, Caster );
				m.Mana += toHeal;
				m.Stam += toHeal;

				m.FixedParticles( 0x3039, 9, 32, 5030, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.Waist );
				m.PlaySound( 0x655 );
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private ResearchRestoration m_Owner;

			public InternalTarget( ResearchRestoration owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Beneficial )
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
