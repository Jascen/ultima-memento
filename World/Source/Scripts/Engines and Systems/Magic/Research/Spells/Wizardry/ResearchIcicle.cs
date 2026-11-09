using System;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchIcicle : ResearchSpell
	{
		public int CirclePower = 1;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.50 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 632,
					IconGraphic = 11251,
					Name = "Icicle",
					PowerWords = "Ex Des Bet",
					Description = "Throws a magical icicle at your enemies that can cause up to 80 points of cold damage.",
					ManaCost = 10,
					MinSkill = 15,
					TargetType = TargetFlags.Harmful
				},
				236,
				9031,
				Reagent.MoonCrystal,Reagent.PigIron
			);

		public ResearchIcicle( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

        public override bool DelayedDamageStacking { get { return !Core.AOS; } }

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

				SpellHelper.Turn( source, m );

				bool hitThrough = SpellHelper.ResolveMagicDefense( CirclePower, ref source, ref m );

				source.MovingParticles( m, 0x28EF, 5, 0, false, false, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB77 ), 0, 3600, 0, 0, 0 );
				source.PlaySound( 0x1E5 );

				if ( hitThrough )
				{
					double damage = DamagingSkill( Caster )/3;
						if ( damage > 80 ){ damage = 80.0; }
						if ( damage < 1 ){ damage = 1.0; }

					SpellHelper.Damage( this, m, damage, 0, 0, 100, 0, 0 );
					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ResearchIcicle m_Owner;

			public InternalTarget( ResearchIcicle owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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