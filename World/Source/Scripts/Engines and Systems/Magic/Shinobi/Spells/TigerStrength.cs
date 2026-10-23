using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Shinobi
{
	public class TigerStrength : ShinobiSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 297,
					IconGraphic = 10878,
					Name = "Tiger Strength",
					PowerWords = "Taigasutorengusu",
					Description = "Calls forth a mystical tiger from the realm of Durama to fight with the ninja during their journey.",
					ManaCost = 70,
					TithingCost = 75,
					MinSkill = 90,
					TargetType = TargetFlags.None
				},
				-1,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public TigerStrength( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if ( (Caster.Followers + 2) > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1049645 ); // You have too many followers to summon that creature.
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				TimeSpan duration = TimeSpan.FromSeconds( (Caster.Skills[SkillName.Ninjitsu].Value * 12) );
				SpellHelper.Summon( new SummonedTiger(), Caster, 0x217, duration, false, false );
			}

			FinishSequence();
		}
	}
}