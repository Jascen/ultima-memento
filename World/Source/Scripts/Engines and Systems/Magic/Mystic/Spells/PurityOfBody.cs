using System;
using Server.Targeting;
using Server.Items;

namespace Server.Spells.Mystic
{
	public class PurityOfBody : MysticSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 257,
					IconGraphic = 0x96D,
					Name = "Purity of Body",
					PowerWords = "Om Summ Cah Beh",
					Description = "You can cleanse your body of poisons with this ability due to your physical discipline, and as such, it cannot be used to aid anyone else.",
					ManaCost = 35,
					TithingCost = 25,
					MinSkill = 40,
					TargetType = TargetFlags.Beneficial
				},
				269,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.5 ); } }
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }
		public override int MysticSpellCircle{ get{ return 3; } }

		public PurityOfBody( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			if ( CheckBSequence( Caster ) )
			{
				SpellHelper.Turn( Caster, Caster );

				Poison p = Caster.Poison;

				if ( p != null )
				{
					int chanceToCure = 10000 + (int)(Caster.Skills[SkillName.FistFighting].Value * 75) - ((p.Level + 1) * (Core.AOS ? (p.Level < 4 ? 3300 : 3100) : 1750));
					chanceToCure /= 100;

					if ( chanceToCure > Utility.Random( 100 ) )
					{
						if ( Caster.CurePoison( Caster ) )
						{
							Caster.SendLocalizedMessage( 1010059 ); // You have been cured of all poisons.
						}
					}
					else
					{
						Caster.SendMessage( "You failed to purify your body!" );
					}
				}

				Caster.PlaySound( 0x212 );
				Caster.PlaySound( 0x206 );

				Effects.SendLocationParticles( EffectItem.Create( Caster.Location, Caster.Map, EffectItem.DefaultDuration ), 0x376A, 1, 29, 0x47D, 2, 9962, 0 );
				Effects.SendLocationParticles( EffectItem.Create( new Point3D( Caster.X, Caster.Y, Caster.Z - 7 ), Caster.Map, EffectItem.DefaultDuration ), 0x37C4, 1, 29, 0x47D, 2, 9502, 0 );
			}

			FinishSequence();
		}
	}
}