using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	public class RidingAttack : WeaponAbility
	{
		public RidingAttack(){}
		public override int BaseMana { get { return 20; } }
		public override bool CheckSkills( Mobile from )
		{
			return base.CheckSkills( from );
		}

		public override void OnHit( Mobile attacker, Mobile defender, int damage )
		{
			if( !attacker.Mounted )
			{
				attacker.SendMessage( "You must be mounted to use this ability!" );
				ClearCurrentAbility( attacker );
				return;
			}
			if( !Validate( attacker ) || !CheckMana( attacker, true ) ) return;
			ClearCurrentAbility( attacker );
			if( defender.Mounted )
			{
				Mobile mount = defender.Mount as Mobile;
				BaseMount.Dismount( defender );
				int amount = 10 + (int)(10.0 * (attacker.Skills[SkillName.Knightship].Value) / 70.0 + 5);
				if( mount != null ) AOS.Damage( mount, null, amount, 100, 0, 0, 0, 0 );
				else AOS.Damage( defender, null, amount, 100, 0, 0, 0, 0 );
			}
			else
			{
				int amount = 10 + (int)(10.0 * (attacker.Skills[SkillName.Knightship].Value) / 70.0 + 5);
				AOS.Damage( defender, attacker, amount, 100, 0, 0, 0, 0 );
				if( Server.Items.ParalyzingBlow.IsImmune( defender ) || BuffInfo.HasParalyzeImmunity( defender ) )
				{
					attacker.SendLocalizedMessage( 1070804 );
					defender.SendLocalizedMessage( 1070813 );
				}
				else
				{
					defender.Paralyze( TimeSpan.FromSeconds( 4.0 ) );
					Server.Items.ParalyzingBlow.BeginImmunity( defender, Server.Items.ParalyzingBlow.FreezeDelayDuration );
				}
			}
		}
	}
}