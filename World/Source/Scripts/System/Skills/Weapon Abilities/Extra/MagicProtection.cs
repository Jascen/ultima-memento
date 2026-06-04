using System;
using Server;

namespace Server.Items
{
	public class MagicProtection : WeaponAbility
	{
		public MagicProtection(){}
		public override int BaseMana { get { return 25; } }

		public override bool OnBeforeSwing(Mobile attacker, Mobile defender, bool validate)
		{
			if (validate && (!Validate(attacker) || !CheckMana(attacker, false))) return false;
			else return true;
		}

		public override void OnHit(Mobile attacker, Mobile defender, int damage)
		{
			if (!CheckMana(attacker, true)) return;
			ClearCurrentAbility(attacker);
			attacker.SendMessage("You feel like you are protected from most magic!");
			attacker.MagicDamageAbsorb += 14;
		}
	}
}