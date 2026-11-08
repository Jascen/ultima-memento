using System;
using Server.Targeting;

namespace Server.Spells.Jester
{
	public class CanOfSnakes : JesterSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 260,
					IconGraphic = 20749,
					Name = "Can of Snakes",
					PowerWords = "How about some nuts?",
					Description = "Opening a can of nuts will simply dazzle the audience as snakes are released instead. These snakes will follow your commands for a short period of time. The better your jester skills, the longer they will remain and the stronger they will be. Their venom will also be equally strong.",
					ManaCost = 40,
					TithingCost = 200,
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

		public CanOfSnakes( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if( (Caster.Followers + 1) > Caster.FollowersMax )
			{
				Caster.SendMessage( "You have too many followers to open that can." );
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
				string FoolName = "a snake";
				int FoolPoisons = 1;
				int FoolHue = Utility.RandomColor(0);
				int FoolSound = 0xDB;
				int FoolBody = 52;
				int FoolPhys = 50;
				int FoolCold = 0;
				int FoolFire = 0;
				int FoolPois = 50;
				int FoolEngy = 0;

				Server.Mobiles.SummonedJoke.MakeJoker( Caster, p, FoolPoisons, FoolName, FoolBody, FoolHue, FoolSound, FoolPhys, FoolCold, FoolFire, FoolPois, FoolEngy );

				int qty = 0;

				if ( Caster.Skills[SkillName.Begging].Value >= Utility.RandomMinMax( 1, 200 ) ){ qty++; }
				if ( Caster.Skills[SkillName.Psychology].Value >= Utility.RandomMinMax( 1, 200 ) ){ qty++; }
				// if ( Caster.Skills[SkillName.Psychology].Value >= Utility.RandomMinMax( 1, 200 ) ){ qty++; }

				if ( qty > ( ( Caster.FollowersMax - Caster.Followers - 1 ) ) )
					qty = Caster.FollowersMax - Caster.Followers;

				if ( qty > 0 ){ FoolHue = Utility.RandomColor(0); Server.Mobiles.SummonedJoke.MakeJoker( Caster, p, FoolPoisons, FoolName, FoolBody, FoolHue, FoolSound, FoolPhys, FoolCold, FoolFire, FoolPois, FoolEngy ); }
				if ( qty > 1 ){ FoolHue = Utility.RandomColor(0); Server.Mobiles.SummonedJoke.MakeJoker( Caster, p, FoolPoisons, FoolName, FoolBody, FoolHue, FoolSound, FoolPhys, FoolCold, FoolFire, FoolPois, FoolEngy ); }
				// if ( qty > 2 ){ FoolHue = Utility.RandomColor(0); Server.Mobiles.SummonedJoke.MakeJoker( Caster, p, FoolPoisons, FoolName, FoolBody, FoolHue, FoolSound, FoolPhys, FoolCold, FoolFire, FoolPois, FoolEngy ); }

				Caster.PlaySound( Caster.Female ? 793 : 1065 );
				Caster.Say( "*gasp!*" );
			}

			FinishSequence();
		}
	}
}