using System;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Spells.Chivalry;
using Server.Misc;

namespace Server.Spells.Fifth
{
	public class ParalyzeSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Paralyze", "An Ex Por",
				218,
				9012,
				Reagent.Garlic,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public ParalyzeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
			else if ( Core.AOS && (m.Frozen || m.Paralyzed || (m.Spell != null && m.Spell.IsCasting && !(m.Spell is PaladinSpell))) )
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

				SpellHelper.CheckReflect( (int)this.Circle, Caster, ref m );

				double duration;

				int nBenefit = 0;
				if ( Caster is PlayerMobile )
					nBenefit = (int)(Caster.Skills[SkillName.Magery].Value / 2);

				int secs = (int)((Spell.ItemSkillValue( Caster, DamageSkill, false ) / 10) - (GetResistSkill( m ) / 10)) + nBenefit;
				
				if( !Core.SE )
					secs += 2;

				if ( !m.Player )
					secs *= 3;

				if ( secs < 0 )
					secs = 0;

				duration = secs;

				m.Paralyze( TimeSpan.FromSeconds( duration ) );

				BuffInfo.RemoveBuff( m, BuffIcon.Paralyze );
				BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.Paralyze, 1063621, TimeSpan.FromSeconds( duration ), m ) );

				m.PlaySound( 0x204 );
				m.FixedEffect( 0x376A, 6, 1, PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0 );

				HarmfulSpell( m );
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private ParalyzeSpell m_Owner;

			public InternalTarget( ParalyzeSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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