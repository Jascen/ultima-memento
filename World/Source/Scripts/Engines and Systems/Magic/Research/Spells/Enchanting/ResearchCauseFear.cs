using System;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchCauseFear : ResearchSpell
	{
		public int CirclePower = 4;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 605,
					IconGraphic = 11214,
					Name = "Cause Fear",
					PowerWords = "Quas Wis Tym",
					Description = "The caster can cause fear in a foe, causing them to flee from you for 10-60 seconds. This spell does not work on golems or constructs.",
					ManaCost = 35,
					MinSkill = 45,
					TargetType = TargetFlags.Harmful
				},
				203,
				9031,
				Reagent.Bloodmoss,Reagent.DaemonBlood,Reagent.RedLotus
			);

		public ResearchCauseFear( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			Caster.SendMessage( "Who do you want to invoke terror at?" );
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			bool CanAffect = true;

			if ( m is BaseCreature )
			{
				SlayerEntry golem = SlayerGroup.GetEntryByName( SlayerName.GolemDestruction );
				if (golem.Slays(m))
				{
					CanAffect = false;
				}
			}

			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( !CanAffect )
			{
				Caster.SendMessage( "This spell cannot affect golems or constructs." );
			}
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Mobile source = Caster;
				bool hitThrough = SpellHelper.ResolveMagicDefense( CirclePower, ref source, ref m );

				m.FixedParticles( 0x3789, 10, 25, 5032, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xBB3 ), 0, EffectLayer.Head );
				m.PlaySound( 0x19D );

				if ( hitThrough )
				{
					TimeSpan duration = TimeSpan.FromSeconds( (DamagingSkill( Caster ) / 4) );

					if ( m is PlayerMobile )
					{
						m.Paralyze( duration );
						m.SendMessage( "You are frozen in fear." );
						BuffInfo.RemoveBuff( m, BuffIcon.Fear );
						BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.Fear, 1063688, duration, m ) );
					}
					else if ( m is BaseCreature )
					{
						BaseCreature bc = (BaseCreature)m;
						bc.BeginFlee( duration );
					}

					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );

					HarmfulSpell( m );
				}
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private ResearchCauseFear m_Owner;

			public InternalTarget( ResearchCauseFear owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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