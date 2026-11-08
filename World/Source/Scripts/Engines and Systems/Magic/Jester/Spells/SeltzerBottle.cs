using System;
using Server.Targeting;
using Server.Items;

namespace Server.Spells.Jester
{
	public class SeltzerBottle : JesterSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 268,
					IconGraphic = 20755,
					Name = "Seltzer Bottle",
					PowerWords = "Are you thirsty?",
					Description = "Offer your audience a drink, and they will probably not ask you again. This will spray a target with freezing water, where the effectiveness is dependent on your jester skills, and it may have the potential to cover the ground in ice cold water.",
					ManaCost = 20,
					TithingCost = 50,
					MinSkill = 10,
					TargetType = TargetFlags.Harmful
				},
				-1,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.0 ); } }
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public SeltzerBottle( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			switch ( Utility.Random( 3 ))
			{
				case 0: Caster.PlaySound( Caster.Female ? 794 : 1066 ); break;
				case 1: Caster.PlaySound( Caster.Female ? 801 : 1073 ); break;
			}

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
				int damage = 1 + (int)( (Caster.Skills[SkillName.Begging].Value / 5) + (Caster.Skills[SkillName.Psychology].Value / 3) );
				Caster.MovingParticles( m, 0x3818, 7, 0, false, false, 0x84B, 0, 0 );
				Caster.PlaySound( 0x025 );
				Effects.SendLocationEffect( m.Location, m.Map, 0x23B2, 20 );

				if ( Caster.Skills[SkillName.Begging].Value >= Utility.RandomMinMax( 50, 300 ) && m != null )
				{
					int goo = 0;

					foreach ( Item splash in m.GetItemsInRange( 10 ) ){ if ( splash is MonsterSplatter ){ goo++; } }

					if ( goo == 0 )
					{
						Point3D p = m.Location;
						MonsterSplatter.AddSplatter( p.X, p.Y, p.Z, m.Map, p, Caster, "freezing water", 296, 0 );
					}
				}

				AOS.Damage( m, Caster, damage, 50, 0, 50, 0, 0 );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private SeltzerBottle m_Owner;

			public InternalTarget( SeltzerBottle owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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