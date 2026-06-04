using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Spells;
using Server.Misc;
using Server.Mobiles;
using Server.Items;

namespace Server.Spells.DeathKnight
{
	public class OrbOfOrcusSpell : DeathKnightSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Orb of Orcus", "Orcus Arma",
				218,
				9031
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1 ); } }
		public override int RequiredTithing{ get{ return 200; } }
		public override double RequiredSkill{ get{ return 80.0; } }
		public override int RequiredMana{ get{ return 56; } }

		public OrbOfOrcusSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			DefensiveSpell.EndDefense( Caster );

			if ( !base.CheckCast() )
				return false;

			if ( Caster.MagicDamageAbsorb > 0 )
			{
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
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
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
			}
			else if ( CheckSequence() )
			{
				int value = (int)( GetKarmaPower( Caster ) / 4 );

				Caster.MagicDamageAbsorb = value;
				Fifth.MagicReflectSpell.AddReflect( Caster );

				Caster.FixedParticles( 0x375A, 10, 15, 5037, EffectLayer.Waist );
				Caster.PlaySound( 0x1E9 );

				BuffInfo.RemoveBuff( Caster, BuffIcon.OrbOfOrcus );
				BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.OrbOfOrcus, 1063551 ) );

				DrainSoulsInLantern( Caster, RequiredTithing );
			}

			FinishSequence();
		}
	}
}