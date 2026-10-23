using System;
using Server.Targeting;
using Server.Items;

namespace Server.Spells.Syth
{
	public class Absorption : SythSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 276,
					IconGraphic = 0x59E7,
					Name = "Absorption",
					PowerWords = "Taral Wai",
					Description = "This power gives the essence of the Syth the ability to absorb most damaging magery spells, and the Syth then redirects those spells back at the one who cast it. The amount that a Syth can absorb is dependent on their power.",
					ManaCost = 28,
					TithingCost = 500,
					MinSkill = 70,
					TargetType = TargetFlags.None
				},
				203,
				0
			);

		public int CirclePower = 6;
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public Absorption( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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