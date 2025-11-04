using System;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Spells.Chivalry;

namespace Server.Spells.Jedi
{
    public class StasisField : JediSpell
	{
		public override int spellIndex { get { return 288; } }
		public int CirclePower = 5;
		public static int spellID = 288;
		public override int RequiredTithing{ get{ return Int32.Parse(  Server.Spells.Jedi.JediSpell.SpellInfo( spellIndex, 10 )); } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }
		public override double RequiredSkill{ get{ return (double)(Int32.Parse( Server.Spells.Jedi.JediSpell.SpellInfo( spellIndex, 2 ))); } }
		public override int RequiredMana{ get{ return Int32.Parse(  Server.Spells.Jedi.JediSpell.SpellInfo( spellIndex, 3 )); } }

		private static SpellInfo m_Info = new SpellInfo(
				Server.Spells.Jedi.JediSpell.SpellInfo( spellID, 1 ),
				Server.Misc.Research.CapsCast( Server.Spells.Jedi.JediSpell.SpellInfo( spellID, 4 ) ),
				218,
				0
			);

        public StasisField(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
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
			else if ( m.Frozen || m.Paralyzed || ( m.Spell != null && m.Spell.IsCasting ) )
			{
				Caster.SendLocalizedMessage( 1061923 ); // The target is already frozen.
			}
			else if ( BuffInfo.HasParalyzeImmunity( m ) )
			{
				Caster.SendLocalizedMessage( 1070804 ); // Your target resists paralysis.
				m.SendLocalizedMessage( 1070813 ); // You resist paralysis.
			}
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				SpellHelper.CheckReflect( CirclePower, Caster, ref m );

				double duration;

				int secs = (int)((GetJediDamage( Caster )/25) - (GetResistSkill( m ) / 10) + (Caster.Skills[SkillName.Psychology].Value / 2) );
				
				if( !Core.SE )
					secs += 2;

				if ( !m.Player )
					secs *= 3;

				if ( secs < 0 )
					secs = 0;

				duration = secs;

				m.Paralyze( TimeSpan.FromSeconds( duration ) );

				BuffInfo.RemoveBuff( m, BuffIcon.StasisField );
				BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.StasisField, 1063528, TimeSpan.FromSeconds( duration ), m ) );

				m.PlaySound( 0x204 );
				m.FixedEffect( 0x376A, 6, 1, 0xB41, 0 );

				HarmfulSpell( m );
				DrainCrystals( Caster, RequiredTithing );
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private StasisField m_Owner;

			public InternalTarget( StasisField owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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