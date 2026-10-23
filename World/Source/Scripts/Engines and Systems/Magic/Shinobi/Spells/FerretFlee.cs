using System;
using Server.Targeting;

namespace Server.Spells.Shinobi
{
	public class FerretFlee : ShinobiSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 294,
					IconGraphic = 10874,
					Name = "Ferret Flee",
					PowerWords = "Ferettoran",
					Description = "If held in place by things such as paralysis magic, spider webbing, or nets the ninja can attempt to free themselves from such holds and escape.",
					ManaCost = 30,
					TithingCost = 35,
					MinSkill = 50,
					TargetType = TargetFlags.None
				},
				-1,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public FerretFlee( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			if ( !Caster.Paralyzed && !Caster.Frozen )
			{
				Caster.SendMessage( "You are not held captive!" );
			}
			else if ( CheckBSequence( Caster ) )
			{
				int chance = (int)(Caster.Skills[SkillName.Ninjitsu].Value);

				if ( chance > Utility.Random( 120 ) )
				{
					Caster.PlaySound( Caster.Female ? 779 : 1050 );
					Caster.Paralyzed = false;
					Caster.Frozen = false;
					BuffInfo.CleanupIcons( Caster, true );
					Caster.SendMessage( "You freed yourself!" );
				}
				else
				{
					Caster.PlaySound( Caster.Female ? 815 : 1089 );
					Caster.SendMessage( "You failed to free yourself!" );
				}
			}

			FinishSequence();
		}
	}
}