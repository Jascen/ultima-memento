using System;
using Server.Targeting;

namespace Server.Spells.Jester
{
	public class PoppingBalloon : JesterSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 266,
					IconGraphic = 20746,
					Name = "Popping Balloon",
					PowerWords = "Would you like a balloon?",
					Description = "Everyong likes balloons, until they decide to play with one of yours. These balloons will float toward your foes, where it will be easily popped and thus cause a physically explosive force. The explosion is equitable to your jester skills, and the range of the explosion will also increase with those skilled as fools.",
					ManaCost = 20,
					TithingCost = 100,
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

		public PoppingBalloon( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if( (Caster.Followers + 3) > Caster.FollowersMax )
			{
				Caster.SendMessage( "You have too many followers to blow up a balloon." );
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
				string FoolName = "a balloon";
				int FoolHue = Utility.RandomList( 0xB3D, 0xB3E, 0xB3F, 0xB40, 0xAD1, 0x9A2, 0x94C, 0x916, 0x947, 0x92E, 0x88E, 0x855 );
				int FoolBody = 1026;
				int FoolFroze = 0;

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