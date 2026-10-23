using System;
using System.Collections;
using Server.Targeting;

namespace Server.Spells.Mystic
{
	public class PsychicWall : MysticSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 256,
					IconGraphic = 0x1A,
					Name = "Psychic Wall",
					PowerWords = "Cah Summ Om Lum",
					Description = "You sheer force of will creates a barrier around you, deflecting magical attacks. This does not work against odd magics like necromancy. Affected spells will often bounce back onto the caster.",
					ManaCost = 45,
					TithingCost = 500,
					MinSkill = 60,
					TargetType = TargetFlags.None
				},
				269,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3 ); } }
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }
		public override int MysticSpellCircle{ get{ return 4; } }

		public PsychicWall( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override bool CheckCast()
		{
			DefensiveSpell.EndDefense( Caster );

			if ( !base.CheckCast() )
				return false;

			if ( Caster.MagicDamageAbsorb > 0 )
			{
				Caster.SendMessage( "Your mind is already protected!" );
				return false;
			}

			return true;
		}

		private static Hashtable m_Table = new Hashtable();

		public override void OnCast()
		{
			DefensiveSpell.EndDefense( Caster );

			if ( Caster.MagicDamageAbsorb > 0 )
			{
				Caster.SendMessage( "Your mind is already protected!" );
			}
			else if ( CheckSequence() )
			{
				int value = (int)( Caster.Skills[SkillName.FistFighting].Value / 2 );
				Caster.MagicDamageAbsorb = value;
				Fifth.MagicReflectSpell.AddReflect( Caster );
				Caster.FixedParticles( 0x3039, 10, 15, 5038, 0, 2, EffectLayer.Head );
				Caster.PlaySound( 0x5BC );
				BuffInfo.RemoveBuff( Caster, BuffIcon.PsychicWall );
				BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.PsychicWall, 1063514 ) );
			}

			FinishSequence();
		}
	}
}