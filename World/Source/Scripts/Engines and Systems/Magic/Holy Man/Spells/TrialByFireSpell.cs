using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Spells;

namespace Server.Spells.HolyMan
{
	public class TrialByFireSpell : HolyManSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Trial by Fire", "Igne Iudicii",
				266,
				9040
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3 ); } }
		public override int RequiredTithing{ get{ return 500; } }
		public override double RequiredSkill{ get{ return 30.0; } }
		public override int RequiredMana{ get{ return 15; } }

		public TrialByFireSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			DefensiveSpell.EndDefense( Caster );

			if ( !base.CheckCast() )
				return false;

			if ( Caster.MagicDamageAbsorb > 0 )
			{
				Caster.SendMessage( "You are already under the effects of this prayer." );
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
				Caster.SendMessage( "You are already under the effects of this prayer." );
			}
			else if ( CheckSequence() )
			{
				int value = (int)( ( Caster.Skills[SkillName.Healing].Value + Caster.Skills[SkillName.Spiritualism].Value ) / 4 );
				Caster.MagicDamageAbsorb = value;
				Fifth.MagicReflectSpell.AddReflect( Caster );
				Caster.SendMessage( "Your body is covered by holy flames." );
				Caster.FixedParticles( 0x3709, 10, 30, 5052, 0x480, 0, EffectLayer.LeftFoot );
				Caster.PlaySound( 0x208 );
				BuffInfo.RemoveBuff( Caster, BuffIcon.TrialByFire );
				BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.TrialByFire, 1063540 ) );
			}

			FinishSequence();
		}
	}
}
