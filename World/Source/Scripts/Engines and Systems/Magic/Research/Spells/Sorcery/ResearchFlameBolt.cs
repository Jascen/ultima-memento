using System;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchFlameBolt : ResearchSpell
	{
		public int CirclePower = 1;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.50 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 626,
					IconGraphic = 11223,
					Name = "Flame Bolt",
					PowerWords = "In Ort Flam",
					Description = "Throws a magical bolt of flame at your enemies that can cause 12-100 points of fire damage.",
					ManaCost = 15,
					MinSkill = 30,
					TargetType = TargetFlags.Harmful
				},
				236,
				9031,
				Reagent.Brimstone,Reagent.SulfurousAsh
			);

		public ResearchFlameBolt( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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

				m.FixedParticles(0x5562, 10, 30, 5052, Server.Misc.PlayerSettings.GetMySpellHue(true, Caster, 0), 0, EffectLayer.Head);
				m.PlaySound(0x44B);

				if ( hitThrough )
				{
					double damage = DamagingSkill( Caster )/2.5;
						if ( damage > 100 ){ damage = 100.0; }
						if ( damage < 12 ){ damage = 12.0; }

					SpellHelper.Damage( this, m, damage, 0, 100, 0, 0, 0 );
					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ResearchFlameBolt m_Owner;

			public InternalTarget( ResearchFlameBolt owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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