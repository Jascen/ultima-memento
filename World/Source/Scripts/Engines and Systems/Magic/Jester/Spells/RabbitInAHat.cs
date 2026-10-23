using System;
using Server.Targeting;

namespace Server.Spells.Jester
{
	public class RabbitInAHat : JesterSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 267,
					IconGraphic = 20753,
					Name = "Rabbit in a Hat",
					PowerWords = "What's in the hat?",
					Description = "Alakazam! Pull rabbits from a hat to dazzle your audience, although petting them may be unwise. Said to be the babies of the killer rabbit of Caerbannog, these critters will follow your commands for a short period of time, and maul those you set them loose on. The strength of the rabbits, and the time they remain, is dependent on your jester skills.",
					ManaCost = 30,
					TithingCost = 150,
					MinSkill = 10,
					TargetType = TargetFlags.None
				},
				-1,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3.0 ); } }
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public RabbitInAHat( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if( (Caster.Followers + 1) > Caster.FollowersMax )
			{
				Caster.SendMessage( "You have too many followers to reach into the hat." );
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
				string FoolName = "a rabbit";
				int FoolPoisons = 0;
				int FoolHue = 0xBB4;
				int FoolSound = 0xA3;
				int FoolBody = 205;
				int FoolPhys = 100;
				int FoolCold = 0;
				int FoolFire = 0;
				int FoolPois = 0;
				int FoolEngy = 0;

				Server.Mobiles.SummonedJoke.MakeJoker( Caster, p, FoolPoisons, FoolName, FoolBody, FoolHue, FoolSound, FoolPhys, FoolCold, FoolFire, FoolPois, FoolEngy );

				int qty = 0;

				if ( Caster.Skills[SkillName.Begging].Value >= Utility.RandomMinMax( 1, 200 ) ){ qty++; }
				if ( Caster.Skills[SkillName.Begging].Value >= Utility.RandomMinMax( 1, 200 ) ){ qty++; }
				if ( Caster.Skills[SkillName.Psychology].Value >= Utility.RandomMinMax( 1, 200 ) ){ qty++; }
				if ( Caster.Skills[SkillName.Psychology].Value >= Utility.RandomMinMax( 1, 200 ) ){ qty++; }

				if ( qty > ( ( Caster.FollowersMax - Caster.Followers - 1 ) ) )
					qty = Caster.FollowersMax - Caster.Followers;

				if ( qty > 0 ){ Server.Mobiles.SummonedJoke.MakeJoker( Caster, p, FoolPoisons, FoolName, FoolBody, FoolHue, FoolSound, FoolPhys, FoolCold, FoolFire, FoolPois, FoolEngy ); }
				if ( qty > 1 ){ Server.Mobiles.SummonedJoke.MakeJoker( Caster, p, FoolPoisons, FoolName, FoolBody, FoolHue, FoolSound, FoolPhys, FoolCold, FoolFire, FoolPois, FoolEngy ); }
				if ( qty > 2 ){ Server.Mobiles.SummonedJoke.MakeJoker( Caster, p, FoolPoisons, FoolName, FoolBody, FoolHue, FoolSound, FoolPhys, FoolCold, FoolFire, FoolPois, FoolEngy ); }
				if ( qty > 3 ){ Server.Mobiles.SummonedJoke.MakeJoker( Caster, p, FoolPoisons, FoolName, FoolBody, FoolHue, FoolSound, FoolPhys, FoolCold, FoolFire, FoolPois, FoolEngy ); }

				Caster.PlaySound( Caster.Female ? 811 : 1085 );
				Caster.Say( "*oooh*" );
			}

			FinishSequence();
		}
	}
}