using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Items;

namespace Server.Spells.Syth
{
	public class Absorption : SythSpell
	{
		public override int spellIndex { get { return 276; } }
		public int CirclePower = 6;
		public static int spellID = 276;
		public override int RequiredTithing{ get{ return Int32.Parse(  Server.Spells.Syth.SythSpell.SpellInfo( spellIndex, 10 )); } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3 ); } }
		public override double RequiredSkill{ get{ return (double)(Int32.Parse( Server.Spells.Syth.SythSpell.SpellInfo( spellIndex, 2 ))); } }
		public override int RequiredMana{ get{ return Int32.Parse(  Server.Spells.Syth.SythSpell.SpellInfo( spellIndex, 3 )); } }

		private static SpellInfo m_Info = new SpellInfo(
				Server.Spells.Syth.SythSpell.SpellInfo( spellID, 1 ),
				Server.Misc.Research.CapsCast( Server.Spells.Syth.SythSpell.SpellInfo( spellID, 4 ) ),
				203,
				0
			);

		public Absorption( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			DefensiveSpell.EndDefense( Caster );

			if ( !base.CheckCast() )
				return false;

			if ( Caster.MagicDamageAbsorb > 0 )
			{
				Caster.SendMessage( "Your essence is already protected!" );
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			DefensiveSpell.EndDefense( Caster );

			if ( Caster.MagicDamageAbsorb > 0 )
			{
				Caster.SendMessage( "Your essence is already protected!" );
			}
			else if ( CheckSequence() )
			{
				int min = 15;
				int max = (int)( GetSythDamage( Caster ) / 4 );
				Caster.MagicDamageAbsorb = Utility.RandomMinMax( min, max );
				Fifth.MagicReflectSpell.AddReflect( Caster );
				Point3D air = new Point3D( ( Caster.X+1 ), ( Caster.Y+1 ), ( Caster.Z+5 ) );
				Effects.SendLocationParticles(EffectItem.Create(air, Caster.Map, EffectItem.DefaultDuration), 0x3789, 9, 32, 0xB00, 0, 5022, 0);
				Caster.PlaySound( 0x64C );
				BuffInfo.RemoveBuff( Caster, BuffIcon.Absorption );
				BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.Absorption, 1063510 ) );
			}

			FinishSequence();
		}
	}
}