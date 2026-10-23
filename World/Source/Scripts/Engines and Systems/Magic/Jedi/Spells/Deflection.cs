using System;
using Server.Targeting;
using Server.Items;

namespace Server.Spells.Jedi
{
	public class Deflection : JediSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 286,
					IconGraphic = 0x2BC4,
					Name = "Deflection",
					PowerWords = "Morden",
					Description = "This power gives the essence of the Jedi the ability to deflect most damaging magery spells back at the one who cast it. The amount that a Jedi can deflect is dependent on their power.",
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

		public Deflection( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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
				int max = (int)( GetJediDamage( Caster ) / 4 );
				Caster.MagicDamageAbsorb = Utility.RandomMinMax( min, max );
				Fifth.MagicReflectSpell.AddReflect( Caster );
				Point3D air = new Point3D( ( Caster.X+1 ), ( Caster.Y+1 ), ( Caster.Z+5 ) );
				Effects.SendLocationParticles(EffectItem.Create(air, Caster.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 0xB41, 0, 5022, 0);
				Effects.PlaySound(Caster.Location, Caster.Map, 0x0F9);
				BuffInfo.RemoveBuff( Caster, BuffIcon.Deflection );
				BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.Deflection, 1063524 ) );
			}

			FinishSequence();
		}
	}
}