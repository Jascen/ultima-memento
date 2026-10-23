using System;
using Server.Targeting;

namespace Server.Spells.Jester
{
	public class SurpriseGift : JesterSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 269,
					IconGraphic = 20752,
					Name = "Surprise Gift",
					PowerWords = "Here is a gift for you!",
					Description = "Surprise your audience with a gift they will never forget. These presents are placed on the ground where nearby foes may be intrigued enough to go open it. They will be surprise by a nice fiery bang. The explosion is equitable to your jester skills, and the range of the explosion will also increase with those skilled as fools.",
					ManaCost = 20,
					TithingCost = 80,
					MinSkill = 10,
					TargetType = TargetFlags.Harmful
				},
				-1,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3.0 ); } }
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public SurpriseGift( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if( (Caster.Followers + 3) > Caster.FollowersMax )
			{
				Caster.SendMessage( "You have too many followers to wrap a gift." );
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			Map map = Caster.Map;

			Point3D p = Caster.Location;

			if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				string FoolName = "a present";
				int FoolHue = 0;
				int FoolBody = Utility.RandomList( 1027, 1028, 1029, 1030 );
				int FoolFroze = 1;

				Caster.Hidden = true;
				Server.Mobiles.SummonedPrank.MakePrankster( Caster, p, FoolName, FoolBody, FoolHue, FoolFroze );
				Caster.Hidden = false;

				Caster.PlaySound( Caster.Female ? 794 : 1066 );
				Caster.Say( "*giggles*" );
			}

			FinishSequence();
		}
	}
}