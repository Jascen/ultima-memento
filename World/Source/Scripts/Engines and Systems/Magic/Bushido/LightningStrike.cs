using System;
using Server.Mobiles;

namespace Server.Spells.Bushido
{
	public class LightningStrike : SamuraiMove
	{
		public LightningStrike()
		{
		}

		public override int BaseMana{ get{ return 5; } }
		public override double RequiredSkill{ get{ return 50.0; } }

		public override TextDefinition AbilityMessage{ get{ return new TextDefinition( 1063167 ); } } // You prepare to strike quickly.

		public override bool DelayedContext{ get{ return true; } }

		public override void CheckGain(Mobile m)
		{
            if (m.Skills[MoveSkill].Value >= RequiredSkill + 37.5)
            {
                if (0.25 > Utility.RandomDouble())
                {
                    m.CheckSkillExplicit(MoveSkill, 0, m.Skills[MoveSkill].Cap);
                }
            }
            else
            {
                base.CheckGain(m);
            }
		}

		public override int GetAccuracyBonus( Mobile attacker )
		{
			var bushido = attacker.Skills[SkillName.Bushido].Value;
			var bonus = (int)Math.Max(0, (bushido - 50) / 2);

			return Math.Min(50, 25 + bonus);
		}

		public override bool Validate(Mobile from)
		{
			bool isValid = base.Validate(from);
			if (!isValid) return false;

			var player = from as PlayerMobile;
			if (player == null) return true;

			player.ExecutesLightningStrike = BaseMana;

			return true;
		}

		public override bool IgnoreArmor( Mobile attacker )
		{
			double bushido = attacker.Skills[SkillName.Bushido].Value;
			double criticalChance = (bushido * bushido) / 72000.0;
			return ( criticalChance >= Utility.RandomDouble() );
		}

		public override bool OnBeforeSwing( Mobile attacker, Mobile defender )
		{
			/* no mana drain before actual hit */
			bool enoughMana = CheckMana(attacker, false);
			return Validate(attacker);
		}

		public override bool ValidatesDuringHit { get { return false; } }

		public override void OnHit( Mobile attacker, Mobile defender, int damage )
		{
			ClearCurrentMove(attacker);
			if (CheckMana(attacker, true))
			{
				attacker.SendLocalizedMessage(1063168); // You attack with lightning precision!
				defender.SendLocalizedMessage(1063169); // Your opponent's quick strike causes extra damage!
				defender.FixedParticles(0x3818, 1, 11, 0x13A8, 0, 0, EffectLayer.Waist);
				defender.PlaySound(0x51D);
				CheckGain(attacker);
				SetContext(attacker);
			}
		}

		public override void OnClearMove( Mobile attacker )
		{
			PlayerMobile player = attacker as PlayerMobile; // this can be deletet if the PlayerMobile parts are moved to Server.Mobile 
			if (player == null) return;

			player.ExecutesLightningStrike = 0;
		}
	}
}
