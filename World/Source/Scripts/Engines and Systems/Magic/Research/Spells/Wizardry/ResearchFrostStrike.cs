using System;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchFrostStrike : ResearchSpell
	{
		public int CirclePower = 7;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 628,
					IconGraphic = 11255,
					Name = "Frost Strike",
					PowerWords = "Ex Des Corp",
					Description = "Damages an enemy with a column of frost crystals that causes 35-125 points of cold damage.",
					ManaCost = 40,
					MinSkill = 67,
					TargetType = TargetFlags.Harmful
				},
				245,
				9042,
				Reagent.MoonCrystal,Reagent.GraveDust
			);

		public ResearchFrostStrike( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Mobile source = Caster;
				bool hitThrough = SpellHelper.ResolveMagicDefense( CirclePower, ref source, ref m );

				m.FixedParticles( 0x5571, 10, 30, 5052, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.LeftFoot );
				m.PlaySound( 0x65D );

				if ( hitThrough )
				{
					double damage = DamagingSkill( Caster )/2;
						if ( damage > 125 ){ damage = 125.0; }
						if ( damage < 35 ){ damage = 35.0; }

					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );

					SpellHelper.Damage( this, m, damage, 0, 0, 100, 0, 0 );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ResearchFrostStrike m_Owner;

			public InternalTarget( ResearchFrostStrike owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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