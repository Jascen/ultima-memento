using System;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Spells.Chivalry;
using Server.Items;

namespace Server.Spells.Elementalism
{
	public class Elemental_Hold_Spell : ElementalSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Elemental Hold", "Temnita",
				218,
				9012
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public Elemental_Hold_Spell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

				int nBenefit = (int)(Caster.Skills[CastSkill].Value / 2);
				
				int secs = (int)((GetDamageSkill( Caster ) / 10) - (GetResistSkill( m ) / 10)) + nBenefit;
				
				if( !Core.SE )
					secs += 2;

				if ( !m.Player )
					secs *= 3;

				if ( secs < 0 )
					secs = 0;

				double duration = secs;

				m.Paralyze( TimeSpan.FromSeconds( duration ) );

				BuffInfo.RemoveBuff( m, BuffIcon.ElementalHold );
				BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.ElementalHold, 1063628, TimeSpan.FromSeconds( duration ), m ) );

				string elm = ElementalSpell.GetElement( Caster );
				Point3D loc = new Point3D( 0, 0, 0 );

				if ( elm == "air" )
				{
					m.PlaySound( 0x5C4 );
					loc = new Point3D( m.X+1, m.Y+1, m.Z+5 );
					Item effect = new ElementalEffect( 0x54E1, duration, m );
					effect.Hue = 0xBB4;
					effect.Light = LightType.Circle300;
					effect.MoveToWorld( loc, m.Map );
				}
				else if ( elm == "earth" )
				{
					m.PlaySound( 0x161 );
					loc = new Point3D( m.X+1, m.Y+1, m.Z+10 );
					Item effect = new ElementalEffect( 0x5487, duration, m );
					effect.MoveToWorld( loc, m.Map );
				}
				else if ( elm == "fire" )
				{
					m.PlaySound( 0x346 );
					loc = new Point3D( m.X+1, m.Y+1, m.Z+10 );
					Item effect = new ElementalEffect( 0x5475, duration, m );
					effect.Hue = 0xB71;
					effect.Light = LightType.Circle300;
					effect.MoveToWorld( loc, m.Map );
				}
				else if ( elm == "water" )
				{
					m.PlaySound( 0x1BF );
					loc = new Point3D( m.X+1, m.Y+1, m.Z+10 );
					Item effect = new ElementalEffect( 0x5487, duration, m );
					effect.Hue = 0xB3E;
					effect.MoveToWorld( loc, m.Map );
				}

				HarmfulSpell( m );
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private Elemental_Hold_Spell m_Owner;

			public InternalTarget( Elemental_Hold_Spell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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