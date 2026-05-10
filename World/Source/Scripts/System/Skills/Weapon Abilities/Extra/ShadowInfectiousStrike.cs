using System;
using Server.Mobiles;

namespace Server.Items
{
	public class ShadowInfectiousStrike : WeaponAbility
	{
		public ShadowInfectiousStrike(){}

		public override int BaseMana{ get{ return 25; } }

		public override void OnHit( Mobile attacker, Mobile defender, int damage )
		{
			if ( !Validate( attacker ) ) return;
			ClearCurrentAbility( attacker );
			BaseWeapon weapon = attacker.Weapon as BaseWeapon;
			if ( weapon == null ) return;
			Skill skill = attacker.Skills[SkillName.Stealth];
			Poison p = weapon.Poison;
			bool canpoison = true;
			bool canhide = true;

			if ( ((PlayerMobile)attacker).Preferences.ClassicPoisoning )
			{
				attacker.SendMessage( "You cannot use this attack with your current poison settings!" );
				return;
			}
			if ( (p == null || weapon.PoisonCharges <= 0) && ((skill != null && skill.Value < 80.0) || skill == null ) )
			{
				attacker.SendMessage( "Your stealth is not sufficient, and the weapon is out of poison!" );
				return;
			}
			if ( (p == null || weapon.PoisonCharges <= 0) && (skill != null && skill.Value >= 80.0) )
			{
				attacker.SendMessage( "There is no poison on the weapon, but you are still hidden!" );
				canpoison = false;
			}
			if ( (p != null && weapon.PoisonCharges > 1) && ((skill != null && skill.Value < 80.0) || skill == null ) )
			{
				attacker.SendMessage( "Your stealth is not sufficient, but the weapon has poison!" );
				canhide = false;
			}
			if (canpoison)
			{
				if ( !CheckMana( attacker, true ) ) return;

				if ( Utility.Random( 150 ) < attacker.Skills[SkillName.Poisoning].Value )
					attacker.SendMessage( "Your strike was perfect." );
				else
					--weapon.PoisonCharges;

				int maxLevel = attacker.Skills[SkillName.Poisoning].Fixed / 200;
				if ( maxLevel < 0 ) maxLevel = 0;
				if ( p.Level > maxLevel ) p = Poison.GetPoison( maxLevel );
				if ( (attacker.Skills[SkillName.Poisoning].Value / 100.0) > Utility.RandomDouble() )
				{
					int level = p.Level + 1;
					Poison newPoison = Poison.GetPoison( level );
					if ( newPoison != null )
					{
						p = newPoison;
						attacker.SendLocalizedMessage( 1060080 );
						defender.SendLocalizedMessage( 1060081 );
					}
				}
				defender.PlaySound( 0xDD );
				defender.FixedParticles( 0x3728, 244, 25, 9941, 1266, 0, EffectLayer.Waist );
				if ( defender.ApplyPoison( attacker, p ) != ApplyPoisonResult.Immune )
				{
					Misc.Titles.AwardKarma( attacker, -20, true );
					attacker.SendLocalizedMessage( 1008096, true, defender.Name );
					defender.SendLocalizedMessage( 1008097, false, attacker.Name );
				}
			}
			if (canhide)
			{
				defender.SendLocalizedMessage( 1060079 );
				Effects.SendLocationParticles( EffectItem.Create( attacker.Location, attacker.Map, EffectItem.DefaultDuration ), 0x376A, 8, 12, 9943 );
				attacker.PlaySound( 0x482 );
				defender.FixedEffect( 0x37BE, 20, 25 );
				if ( attacker is PlayerMobile)
				{
					PlayerMobile mm = attacker as PlayerMobile;
				}
				attacker.Combatant = null;
				attacker.Warmode = false;
				attacker.Hidden = true;
			}
		}
	}
}