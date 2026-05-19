using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class WizardWand : BaseBashing
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.PsychicAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ZapManaStrike; } }
		public override WeaponAbility ThirdAbility{ get{ return WeaponAbility.ElementalStrike; } }
		public override WeaponAbility FourthAbility{ get{ return WeaponAbility.MagicProtection; } }
		public override WeaponAbility FifthAbility{ get{ return WeaponAbility.MagicProtection2; } }

		public override int AosStrengthReq{ get{ return 20; } }
		public override int AosMinDamage{ get{ return 10; } }
		public override int AosMaxDamage{ get{ return 12; } }
		public override int AosSpeed{ get{ return 44; } }
		public override float MlSpeed{ get{ return 2.50f; } }

		public override int OldStrengthReq{ get{ return 10; } }
		public override int OldMinDamage{ get{ return 8; } }
		public override int OldMaxDamage{ get{ return 24; } }
		public override int OldSpeed{ get{ return 40; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 40; } }

		[Constructable]
		public WizardWand() : base( 0x13B4 )
		{
			Weight = 2.0;
			Attributes.SpellChanneling = 1;

			if ( ItemID == 0x13B4 )
			{
				string make = "Wand";
				ItemID = Utility.RandomList( 0xDF2, 0xDF3, 0xDF4, 0xDF5 );

				if ( Utility.RandomMinMax( 1, 10 ) == 1 ) // 10% ARE SCEPTERS
				{
					Weight = 5.0;
					make = "Scepter";
					ItemID = Utility.RandomList( 0x26BC, 0x26C6 );
				}

				switch ( Utility.RandomMinMax( 0, 14 ) ) 
				{
					case 0: Name = make + " of Wizardry";			break;
					case 1: Name = make + " of Sorcery";			break;
					case 2: Name = make + " of the Magician";		break;
					case 3: Name = make + " of the Warlock";		break;
					case 4: Name = make + " of the Wizard";			break;
					case 5: Name = make + " of the Sorcerer";		break;
					case 6: Name = make + " of Wizards";			break;
					case 7: Name = make + " of Sorcerers";			break;
					case 8: Name = make + " of Magicians";			break;
					case 9: Name = make + " of Warlocks";			break;
					case 10: Name = make + " of the Witch";			break;
					case 11: Name = make + " of Witches";			break;
					case 12: Name = make + " of Magery";			break;
					case 13: Name = make + " of Mages";				break;
					case 14: Name = make + " of the Mages";			break;
				}

				Attributes.LowerManaCost = Utility.RandomMinMax( 1, MyServerSettings.LowMana() );
				Attributes.LowerRegCost = Utility.RandomMinMax( 1, MyServerSettings.LowReg() );
				Attributes.RegenMana = 4;
			}
		}

		public override bool CanEquip( Mobile from )
		{
			if ( !CheckWizardEquip( from ) )
				return false;

			return base.CanEquip( from );
		}

		public WizardWand( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			if (version == 0)
			{
				var lmcCap = MyServerSettings.LowMana();
				if (lmcCap < Attributes.LowerManaCost)
					Attributes.LowerManaCost = Utility.RandomMinMax( 1, lmcCap );
			}
		}
	}
}