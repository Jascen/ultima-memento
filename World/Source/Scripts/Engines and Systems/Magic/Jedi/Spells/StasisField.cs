using System;
using Server.Targeting;

namespace Server.Spells.Jedi
{
    public class StasisField : JediSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 288,
					IconGraphic = 0x2BF5,
					Name = "Stasis Field",
					PowerWords = "Greggs",
					Description = "A Jedi can create a field around another that will put them in stasis for a period of time, where they cannot take any actions for a short duration.",
					ManaCost = 36,
					TithingCost = 52,
					MinSkill = 50,
					TargetType = TargetFlags.Harmful
				},
				218,
				0
			);

		public int CirclePower = 5;
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

        public StasisField(Mobile caster, Item scroll) : base(caster, scroll, SpellInfo)
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
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Mobile source = Caster;
				bool hitThrough = SpellHelper.ResolveMagicDefense( CirclePower, ref source, ref m );

				m.PlaySound( 0x204 );
				m.FixedEffect( 0x376A, 6, 1, 0xB41, 0 );

				if ( hitThrough )
				{
					int secs = (int)((GetJediDamage( Caster )/25) - (GetResistSkill( m ) / 10) + (Caster.Skills[SkillName.Psychology].Value / 2) );
					
					if( !Core.SE )
						secs += 2;

					if ( !m.Player )
						secs *= 3;

					if ( secs < 0 )
						secs = 0;

					double duration = secs;

					m.Paralyze( TimeSpan.FromSeconds( duration ) );

					BuffInfo.RemoveBuff( m, BuffIcon.StasisField );
					BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.StasisField, 1063528, TimeSpan.FromSeconds( duration ), m ) );

					HarmfulSpell( m );
				}
			}
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