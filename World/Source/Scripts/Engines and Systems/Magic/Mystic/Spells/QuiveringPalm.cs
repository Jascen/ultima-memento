using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Mystic
{
	public class QuiveringPalm : MysticSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 258,
					IconGraphic = 0x5001,
					Name = "Quivering Palm",
					PowerWords = "Summ Cah Beh Ra",
					Description = "You must be wearing some sort of pugilist gloves for this ability to work. It temporarily enhances the kind of damage the gloves do. The type of damage inflicted when hitting a target will be converted to the target's worst resistance type. The duration of the effect is affected by your fist fighting skill.",
					ManaCost = 20,
					TithingCost = 20,
					MinSkill = 20,
					TargetType = TargetFlags.None
				},
				269,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }

		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override bool BlocksMovement{ get{ return false; } }
		public override int MysticSpellCircle{ get{ return 1; } }

		public QuiveringPalm( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			BaseWeapon weapon = Caster.Weapon as BaseWeapon;

			if ( weapon == null || weapon is Fists )
			{
				Caster.SendMessage( "You must be wearing some type of pugilist gloves!" );
			}
			else if ( !( weapon is IPugilistGlove ) )
			{
				Caster.SendMessage( "You must be wearing some type of pugilist gloves!" );
			}
			else if ( CheckSequence() )
			{
				Caster.PlaySound( 0x212 );
				Effects.SendLocationParticles( EffectItem.Create( Caster.Location, Caster.Map, EffectItem.DefaultDuration ), 0x376A, 1, 29, 0x47D, 2, 9962, 0 );

				double seconds = Caster.Skills[SkillName.FistFighting].Value;

				TimeSpan duration = TimeSpan.FromSeconds( seconds );

				Timer t = (Timer)m_Table[weapon];

				if ( t != null )
					t.Stop();

				weapon.Consecrated = true;

				Caster.SendMessage( "Your fists glow with magical energy." );
				m_Table[weapon] = t = new ExpireTimer( weapon, duration, Caster );
                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.ConsecrateWeapon, 1063605, duration, Caster));

				t.Start();
			}

			FinishSequence();
		}

		private static Hashtable m_Table = new Hashtable();

		private class ExpireTimer : Timer
		{
			private BaseWeapon m_Weapon;
			private Mobile m_Caster;

			public ExpireTimer( BaseWeapon weapon, TimeSpan delay, Mobile caster ) : base( delay )
			{
				m_Weapon = weapon;
				Priority = TimerPriority.FiftyMS;
				m_Caster = caster;
			}

			protected override void OnTick()
			{
				if ( m_Table[m_Weapon] == null ) return;

				m_Caster.SendMessage( "Your fists return to normal." );
				m_Weapon.Consecrated = false;
				Effects.PlaySound( m_Weapon.GetWorldLocation(), m_Weapon.Map, 0x1F8 );
				m_Table.Remove( this );
			}
		}
	}
}