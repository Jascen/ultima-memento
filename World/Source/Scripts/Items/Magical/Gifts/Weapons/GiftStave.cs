using Server;
using System;
using System.Collections;
using Server.Targeting;
using Server.Prompts;
using Server.Items;
using Server.Misc;
using Server.Network;
using Server.Spells;
using Server.Mobiles;

namespace Server.Items
{
	public class GiftStave : BaseGiftStave
	{
		public override int EffectID
		{
			get
			{
				if ( damageType == 1 ){ return 0x4D17; } 		// Fire
				else if ( damageType == 2 ){ return 0x4D18; } 	// Cold
				else if ( damageType == 3 ){ return 0x3818; } 	// Energy
				else if ( damageType == 4 ){ return 0x4F49; } 	// Poison
				return 0x4F48;
			}
		}

		public override int DefHitSound
		{
			get
			{
				if ( damageType == 1 ){ return 0x15E; } 		// Fire
				else if ( damageType == 2 ){ return 0x650; } 	// Cold
				else if ( damageType == 3 ){ return 0x211; } 	// Energy
				else if ( damageType == 4 ){ return 0x658; } 	// Poison
				return 0x1E5;
			}
		}

		public override Type AmmoType{ get{ return typeof( MageEye ); } }

		public override WeaponAbility PrimaryAbility
		{
			get
			{
				if ( damageType == 1 ){ return WeaponAbility.ZapStamStrike; } 		// Fire
				else if ( damageType == 2 ){ return WeaponAbility.ZapDexStrike; } 	// Cold
				else if ( damageType == 3 ){ return WeaponAbility.ZapIntStrike; } 	// Energy
				else if ( damageType == 4 ){ return WeaponAbility.ZapStrStrike;} 	// Poison
				return WeaponAbility.ZapManaStrike;
			}
		}
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MagicProtection; } }
		public override WeaponAbility ThirdAbility{ get{ return WeaponAbility.ElementalStrike; } }
		public override WeaponAbility FourthAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility FifthAbility{ get{ return WeaponAbility.MagicProtection2; } }

		public override int AosStrengthReq{ get{ return 20; } }
		public override int AosMinDamage{ get{ return Core.ML ? 15 : 16; } }
		public override int AosMaxDamage{ get{ return Core.ML ? 19 : 18; } }
		public override int AosSpeed{ get{ return 25; } }
		public override float MlSpeed{ get{ return 5.00f; } }

		public override int OldStrengthReq{ get{ return 15; } }
		public override int OldMinDamage{ get{ return 9; } }
		public override int OldMaxDamage{ get{ return 41; } }
		public override int OldSpeed{ get{ return 20; } }

		public override int DefMaxRange{ get{ return 10; } }

		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 90; } }

		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Pierce2H; } }

		[Constructable]
		public GiftStave() : base( 0x0908 )
		{
			Name = "stave";
			Weight = 7.0;
			Layer = Layer.TwoHanded;
		}

		public GiftStave( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			writer.Write( damageType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			damageType = reader.ReadInt();
		}
	}

	public class GiftSceptre : BaseGiftStave
	{
		public override int EffectID
		{
			get
			{
				if ( damageType == 1 ){ return 0x4D17; } 		// Fire
				else if ( damageType == 2 ){ return 0x4D18; } 	// Cold
				else if ( damageType == 3 ){ return 0x3818; } 	// Energy
				else if ( damageType == 4 ){ return 0x4F49; } 	// Poison
				return 0x4F48;
			}
		}

		public override int DefHitSound
		{
			get
			{
				if ( damageType == 1 ){ return 0x15E; } 		// Fire
				else if ( damageType == 2 ){ return 0x650; } 	// Cold
				else if ( damageType == 3 ){ return 0x211; } 	// Energy
				else if ( damageType == 4 ){ return 0x658; } 	// Poison
				return 0x1E5;
			}
		}

		public override Type AmmoType{ get{ return typeof( MageEye ); } }

		public override WeaponAbility PrimaryAbility
		{
			get
			{
				if ( damageType == 1 ){ return WeaponAbility.ZapStamStrike; } 		// Fire
				else if ( damageType == 2 ){ return WeaponAbility.ZapDexStrike; } 	// Cold
				else if ( damageType == 3 ){ return WeaponAbility.ZapIntStrike; } 	// Energy
				else if ( damageType == 4 ){ return WeaponAbility.ZapStrStrike;} 	// Poison
				return WeaponAbility.ZapManaStrike;
			}
		}
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MagicProtection; } }
		public override WeaponAbility ThirdAbility{ get{ return WeaponAbility.ElementalStrike; } }
		public override WeaponAbility FourthAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility FifthAbility{ get{ return WeaponAbility.MagicProtection2; } }

		public override int AosStrengthReq{ get{ return 15; } }
		public override int AosMinDamage{ get{ return Core.ML ? 11 : 12; } }
		public override int AosMaxDamage{ get{ return Core.ML ? 15 : 14; } }
		public override int AosSpeed{ get{ return 25; } }
		public override float MlSpeed{ get{ return 5.00f; } }

		public override int OldStrengthReq{ get{ return 15; } }
		public override int OldMinDamage{ get{ return 9; } }
		public override int OldMaxDamage{ get{ return 41; } }
		public override int OldSpeed{ get{ return 20; } }

		public override int DefMaxRange{ get{ return 8; } }

		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 90; } }

		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Bash1H; } }

		[Constructable]
		public GiftSceptre() : base( 0xDF2 )
		{
			Name = "sceptre";
			Weight = 3.0;
			Layer = Layer.OneHanded;
			ItemID = Utility.RandomList( 0x0DF2, 0x0DF3, 0x0DF4, 0x0DF5, 0x269D, 0x269E, 0x26BC, 0x26C6, 0x639D, 0x639E, 0x639F, 0x63A0 );
		}

		public GiftSceptre( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			writer.Write( damageType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			damageType = reader.ReadInt();
		}
	}

	public class BaseGiftStave : BaseGiftRanged
	{
		public int damageType;
		[CommandProperty(AccessLevel.Owner)]
		public int damage_Type { get { return damageType; } set { damageType = value; InvalidateProperties(); } }

		public override int EffectID{ get{ return 0x4F48; } }
		public override Type AmmoType{ get{ return typeof( MageEye ); } }
		public override Item Ammo{ get{ return new MageEye(); } }

		public override int DefHitSound{ get{ return 0x54A; } }
		public override int DefMissSound{ get{ return 0x4BB; } }

		public override SkillName DefSkill{ get{ return SkillName.Marksmanship; } }
		public override WeaponType DefType{ get{ return WeaponType.Ranged; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Pierce2H; } }

		public override SkillName AccuracySkill{ get{ return SkillName.Marksmanship; } }

		public BaseGiftStave( int itemID ) : base( itemID )
		{
			damageType = 0;
		}

		public override void OnLocationChange( Point3D oldLocation )
		{
			EnergyType();
			base.OnLocationChange( oldLocation );
		}

		public override void OnDoubleClick( Mobile from )
		{
			base.OnDoubleClick( from );

			Target t;

			if ( !IsChildOf( from.Backpack ) && !(Parent == from) )
			{
				from.SendMessage( "The item must be in your possession to use it." );
			}
			else
			{
				from.SendMessage( "What gems do you want to transmorph?" );
				t = new GemTarget();
				from.Target = t;
			}
		}

		public override bool CanEquip( Mobile from )
		{
			if ( !CheckWizardEquip( from ) )
				return false;

			return base.CanEquip( from );
		}

		public override bool OnEquip( Mobile from )
		{
			from.SendMessage( "You need mage eye crystals to power this item, and you can turn common gems into that with this." );
			return base.OnEquip( from );
		}

		private class GemTarget : Target
		{
			public GemTarget() : base( 1, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				Item iGem = targeted as Item;

				if ( iGem is StarSapphire || iGem is Emerald || iGem is Sapphire || iGem is Ruby || iGem is Citrine || iGem is Amethyst || iGem is Tourmaline || iGem is Amber || iGem is Diamond )
				{
					if ( !iGem.IsChildOf( from.Backpack ) )
					{
						from.SendMessage( "You can only transmorph gems in your pack." );
					}
					else
					{
						int amount = 4;
						if ( iGem is StarSapphire ){ amount = iGem.Amount * 62; }
						else if ( iGem is Emerald ){ amount = iGem.Amount * 50; }
						else if ( iGem is Sapphire ){ amount = iGem.Amount * 50; }
						else if ( iGem is Ruby ){ amount = iGem.Amount * 37; }
						else if ( iGem is Citrine ){ amount = iGem.Amount * 25; }
						else if ( iGem is Amethyst ){ amount = iGem.Amount * 50; }
						else if ( iGem is Tourmaline ){ amount = iGem.Amount * 47; }
						else if ( iGem is Amber ){ amount = iGem.Amount * 25; }
						else if ( iGem is Diamond ){ amount = iGem.Amount * 100; }
						amount = (int)(amount/4);

						from.RevealingAction();
						from.PlaySound( 0x243 );
						from.AddToBackpack( new MageEye(amount) );
						from.SendMessage( "You transmorph the gems into mage eyes." );
						iGem.Delete();
					}
				}
				else
				{
					from.SendMessage( "This can only transmorph certain gems." );
				}
			}
		}

		public void EnergyType()
		{
			int physical = 100 - AosElementDamages.Fire - AosElementDamages.Cold - AosElementDamages.Energy - AosElementDamages.Poison;
			damageType = 0;
			if ( AosElementDamages.Fire > AosElementDamages.Cold && AosElementDamages.Fire > AosElementDamages.Poison && AosElementDamages.Fire > AosElementDamages.Energy && AosElementDamages.Fire > physical ){ damageType = 1; }
			else if ( AosElementDamages.Cold > AosElementDamages.Fire && AosElementDamages.Cold > AosElementDamages.Poison && AosElementDamages.Cold > AosElementDamages.Energy && AosElementDamages.Cold > physical ){ damageType = 2; }
			else if ( AosElementDamages.Energy > AosElementDamages.Cold && AosElementDamages.Energy > AosElementDamages.Fire && AosElementDamages.Energy > AosElementDamages.Poison && AosElementDamages.Energy > physical ){ damageType = 3; }
			else if ( AosElementDamages.Poison > AosElementDamages.Fire && AosElementDamages.Poison > AosElementDamages.Cold && AosElementDamages.Poison > AosElementDamages.Energy && AosElementDamages.Poison > physical ){ damageType = 4; }
		}

		public BaseGiftStave( Serial serial ) : base( serial )
		{
		}

		public override TimeSpan OnSwing( Mobile attacker, Mobile defender )
		{
			WeaponAbility a = WeaponAbility.GetCurrentAbility( attacker );

			// Make sure we've been standing still for .25/.5/1 second depending on Era
			if ( DateTime.Now > (attacker.LastMoveTime + TimeSpan.FromSeconds( Core.SE ? 0.25 : (Core.AOS ? 0.5 : 1.0) )) || (Core.AOS && WeaponAbility.GetCurrentAbility( attacker ) is MovingShot) )
			{
				bool canSwing = true;

				if ( Core.AOS )
				{
					canSwing = ( !attacker.Paralyzed && !attacker.Frozen );

					if ( canSwing )
					{
						Spell sp = attacker.Spell as Spell;

						canSwing = ( sp == null || !sp.IsCasting || !sp.BlocksMovement );
					}
				}

				if ( canSwing && attacker.HarmfulCheck( defender ) )
				{
					attacker.DisruptiveAction();
					attacker.Send( new Swing( 0, attacker, defender ) );

					if ( OnFired( attacker, defender ) )
					{
						if ( CheckHit( attacker, defender ) )
							OnHit( attacker, defender );
						else
							OnMiss( attacker, defender );
					}
				}

				attacker.RevealingAction();

				return GetDelay( attacker );
			}
			else
			{
				attacker.RevealingAction();

				return TimeSpan.FromSeconds( 0.25 );
			}
		}

		public static bool HasStaff( Mobile from )
		{
			if ( from.FindItemOnLayer( Layer.OneHanded ) != null )
			{
				Item oneHand = from.FindItemOnLayer( Layer.OneHanded );
				if ( oneHand is BaseGiftStave ){ return true; }
			}
			if ( from.FindItemOnLayer( Layer.TwoHanded ) != null )
			{
				Item twoHand = from.FindItemOnLayer( Layer.TwoHanded );
				if ( twoHand is BaseGiftStave ){ return true; }
			}
			if ( from.Backpack.FindItemByType( typeof ( GiftStave ) ) != null )
			{
				return true;
			}
			if ( from.Backpack.FindItemByType( typeof ( GiftSceptre ) ) != null )
			{
				return true;
			}

			return false;
		}

		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			base.OnHit( attacker, defender, damageBonus );
		}

		public override void OnMiss( Mobile attacker, Mobile defender )
		{
			base.OnMiss( attacker, defender );
		}

		public override bool OnFired( Mobile attacker, Mobile defender )
		{
			BaseQuiver quiver = attacker.FindItemOnLayer( Layer.Cloak ) as BaseQuiver;
			Container pack = attacker.Backpack;

			if ( attacker.Player )
			{
				if ( quiver == null || quiver.LowerAmmoCost == 0 || quiver.LowerAmmoCost > Utility.Random( 100 ) )
				{
					if ( quiver != null && quiver.ConsumeTotal( AmmoType, 1 ) )
						quiver.InvalidateWeight();
					else if ( pack == null || !pack.ConsumeTotal( AmmoType, 1 ) )
						return false;
				}
			}

			attacker.MovingEffect( defender, EffectID, 18, 1, false, false );

			Server.Gumps.QuickBar.RefreshQuickBar( attacker );

			return true;
		}

		public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (int) 0 ); }
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt(); }
	}
}