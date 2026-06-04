using System;
using Server;

namespace Server.Items
{
	public abstract class BaseMeleeWeapon : BaseWeapon
	{
		public BaseMeleeWeapon( int itemID ) : base( itemID )
		{
		}

		public BaseMeleeWeapon( Serial serial ) : base( serial )
		{
		}

		public override int AbsorbDamage( Mobile attacker, Mobile defender, int damage )
		{
			damage = base.AbsorbDamage( attacker, defender, damage );
			if ( damage < 1 ) return 0;

			int absorb = defender.MeleeDamageAbsorb;
			if ( absorb < 1 ) return damage;

			// Absorb half of the damage
			int absorbed = Math.Min( absorb, damage / 2 );
			if ( absorbed < 1 ) return damage;

			// Only consume half of the charges for what is absorbed
			defender.MeleeDamageAbsorb -= Math.Max( 1, absorbed / 2 );
			if ( defender.MeleeDamageAbsorb < 1 )
			{
				defender.MeleeDamageAbsorb = 0;
				DefensiveSpell.Nullify( defender );
			}

			attacker.PlaySound( 0x1F1 );
			attacker.FixedEffect( 0x374A, 10, 16 );
			defender.SendMessage( "Your shield absorbs some of the damage.", damage );

			return damage - absorbed;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}
	}
}
