using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.DeathKnight
{
	public class LucifersBoltSpell : DeathKnightSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Lucifer's Bolt", "Lucifer Fulgur",
				230,
				9022
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1 ); } }
		public override int RequiredTithing{ get{ return 35; } }
		public override double RequiredSkill{ get{ return 25.0; } }
		public override int RequiredMana{ get{ return 24; } }

		public LucifersBoltSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
			else if ( Core.AOS && (m.Frozen || m.Paralyzed || (m.Spell != null && m.Spell.IsCasting)) )
			{
				Caster.SendLocalizedMessage( 1061923 ); // The target is already frozen.
			}
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				SpellHelper.CheckReflect( 4, Caster, ref m );

				double duration = 7.0 + ( GetKarmaPower( m ) * 0.2 );

				m.Paralyze( TimeSpan.FromSeconds( duration ) );
				m.FixedEffect( 0x376A, 6, 1 );
				m.BoltEffect( 0 );
				DrainSoulsInLantern( Caster, RequiredTithing );
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private LucifersBoltSpell m_Owner;

			public InternalTarget( LucifersBoltSpell owner ) : base( 12, false, TargetFlags.Harmful )
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
