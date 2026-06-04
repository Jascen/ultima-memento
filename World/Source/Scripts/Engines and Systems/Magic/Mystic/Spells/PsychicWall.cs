using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Items;

namespace Server.Spells.Mystic
{
	public class PsychicWall : MysticSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Psychic Wall", "Cah Summ Om Lum",
				269,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3 ); } }
		public override int RequiredTithing{ get{ return 500; } }
		public override double RequiredSkill{ get{ return 60.0; } }
		public override int RequiredMana{ get{ return 45; } }
		public override int MysticSpellCircle{ get{ return 4; } }

		public PsychicWall( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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