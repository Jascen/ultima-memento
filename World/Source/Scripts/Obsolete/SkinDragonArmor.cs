using System;
using Server;

namespace Server.Items
{
	public class SkinDragonLegs : LeatherLegs ///////////////////////////////////////////////////////
	{
		[Constructable]
		public SkinDragonLegs()
		{
			Name = "Dragon Skin Leggings";
			Hue = CraftResources.GetHue( CraftResource.DragonSkin );

			ArmorAttributes.DurabilityBonus = 50;
			ArmorAttributes.LowerStatReq = 0;
			ArmorAttributes.MageArmor = 0;

			Attributes.SpellDamage = 0;
			Attributes.CastSpeed = 0;
			Attributes.DefendChance = 2;
			Attributes.LowerManaCost = 0;
			Attributes.LowerRegCost = 0;
			Attributes.ReflectPhysical = 2;
			Attributes.Luck = 0;
			Attributes.NightSight = 0;
			Attributes.BonusDex = 0;
			Attributes.BonusInt = 0;
			Attributes.BonusStr = 2;
			Attributes.RegenHits = 0;
			Attributes.RegenMana = 0;
			Attributes.RegenStam = 3;

			PhysicalBonus = 2;
			ColdBonus = 0;
			EnergyBonus = 0;
			FireBonus = 3;
			PoisonBonus = 0;
		}

		public SkinDragonLegs( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( Cleanup ), this );
		}

		private void Cleanup( object state )
		{
			Item item = new LeatherLegs();
			((BaseArmor)item).Resource = CraftResource.DragonSkin;
			Server.Misc.Cleanup.DoCleanup( (Item)state, item );
		}
	}
	public class SkinDragonGloves : LeatherGloves ///////////////////////////////////////////////////
	{
		[Constructable]
		public SkinDragonGloves()
		{
			Name = "Dragon Skin Gloves";
			Hue = CraftResources.GetHue( CraftResource.DragonSkin );

			ArmorAttributes.DurabilityBonus = 50;
			ArmorAttributes.LowerStatReq = 0;
			ArmorAttributes.MageArmor = 0;

			Attributes.SpellDamage = 0;
			Attributes.CastSpeed = 0;
			Attributes.DefendChance = 2;
			Attributes.LowerManaCost = 0;
			Attributes.LowerRegCost = 0;
			Attributes.ReflectPhysical = 2;
			Attributes.Luck = 0;
			Attributes.NightSight = 0;
			Attributes.BonusDex = 0;
			Attributes.BonusInt = 0;
			Attributes.BonusStr = 2;
			Attributes.RegenHits = 0;
			Attributes.RegenMana = 0;
			Attributes.RegenStam = 3;

			PhysicalBonus = 2;
			ColdBonus = 0;
			EnergyBonus = 0;
			FireBonus = 3;
			PoisonBonus = 0;
		}

		public SkinDragonGloves( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( Cleanup ), this );
		}

		private void Cleanup( object state )
		{
			Item item = new LeatherGloves();
			((BaseArmor)item).Resource = CraftResource.DragonSkin;
			Server.Misc.Cleanup.DoCleanup( (Item)state, item );
		}
	}
	public class SkinDragonGorget : LeatherGorget ///////////////////////////////////////////////////
	{
		[Constructable]
		public SkinDragonGorget()
		{
			Name = "Dragon Skin Gorget";
			Hue = CraftResources.GetHue( CraftResource.DragonSkin );

			ArmorAttributes.DurabilityBonus = 50;
			ArmorAttributes.LowerStatReq = 0;
			ArmorAttributes.MageArmor = 0;

			Attributes.SpellDamage = 0;
			Attributes.CastSpeed = 0;
			Attributes.DefendChance = 2;
			Attributes.LowerManaCost = 0;
			Attributes.LowerRegCost = 0;
			Attributes.ReflectPhysical = 2;
			Attributes.Luck = 0;
			Attributes.NightSight = 0;
			Attributes.BonusDex = 0;
			Attributes.BonusInt = 0;
			Attributes.BonusStr = 2;
			Attributes.RegenHits = 0;
			Attributes.RegenMana = 0;
			Attributes.RegenStam = 3;

			PhysicalBonus = 2;
			ColdBonus = 0;
			EnergyBonus = 0;
			FireBonus = 3;
			PoisonBonus = 0;
		}

		public SkinDragonGorget( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( Cleanup ), this );
		}

		private void Cleanup( object state )
		{
			Item item = new LeatherGorget();
			((BaseArmor)item).Resource = CraftResource.DragonSkin;
			Server.Misc.Cleanup.DoCleanup( (Item)state, item );
		}
	}
	public class SkinDragonArms : LeatherArms ///////////////////////////////////////////////////////
	{
		[Constructable]
		public SkinDragonArms()
		{
			Name = "Dragon Skin Arms";
			Hue = CraftResources.GetHue( CraftResource.DragonSkin );

			ArmorAttributes.DurabilityBonus = 50;
			ArmorAttributes.LowerStatReq = 0;
			ArmorAttributes.MageArmor = 0;

			Attributes.SpellDamage = 0;
			Attributes.CastSpeed = 0;
			Attributes.DefendChance = 2;
			Attributes.LowerManaCost = 0;
			Attributes.LowerRegCost = 0;
			Attributes.ReflectPhysical = 2;
			Attributes.Luck = 0;
			Attributes.NightSight = 0;
			Attributes.BonusDex = 0;
			Attributes.BonusInt = 0;
			Attributes.BonusStr = 2;
			Attributes.RegenHits = 0;
			Attributes.RegenMana = 0;
			Attributes.RegenStam = 3;

			PhysicalBonus = 2;
			ColdBonus = 0;
			EnergyBonus = 0;
			FireBonus = 3;
			PoisonBonus = 0;
		}

		public SkinDragonArms( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( Cleanup ), this );
		}

		private void Cleanup( object state )
		{
			Item item = new LeatherArms();
			((BaseArmor)item).Resource = CraftResource.DragonSkin;
			Server.Misc.Cleanup.DoCleanup( (Item)state, item );
		}
	}
	public class SkinDragonChest : LeatherChest /////////////////////////////////////////////////////
	{
		[Constructable]
		public SkinDragonChest()
		{
			Name = "Dragon Skin Tunic";
			Hue = CraftResources.GetHue( CraftResource.DragonSkin );

			ArmorAttributes.DurabilityBonus = 50;
			ArmorAttributes.LowerStatReq = 0;
			ArmorAttributes.MageArmor = 0;

			Attributes.SpellDamage = 0;
			Attributes.CastSpeed = 0;
			Attributes.DefendChance = 2;
			Attributes.LowerManaCost = 0;
			Attributes.LowerRegCost = 0;
			Attributes.ReflectPhysical = 2;
			Attributes.Luck = 0;
			Attributes.NightSight = 0;
			Attributes.BonusDex = 0;
			Attributes.BonusInt = 0;
			Attributes.BonusStr = 2;
			Attributes.RegenHits = 0;
			Attributes.RegenMana = 0;
			Attributes.RegenStam = 3;

			PhysicalBonus = 2;
			ColdBonus = 0;
			EnergyBonus = 0;
			FireBonus = 3;
			PoisonBonus = 0;
		}

		public SkinDragonChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( Cleanup ), this );
		}

		private void Cleanup( object state )
		{
			Item item = new LeatherChest();
			((BaseArmor)item).Resource = CraftResource.DragonSkin;
			Server.Misc.Cleanup.DoCleanup( (Item)state, item );
		}
	}
	public class SkinDragonHelm : LeatherCap /////////////////////////////////////////
	{
		[Constructable]
		public SkinDragonHelm()
		{
			Name = "Dragon Skin Cap";
			Hue = CraftResources.GetHue( CraftResource.DragonSkin );

			ArmorAttributes.DurabilityBonus = 50;
			ArmorAttributes.LowerStatReq = 0;
			ArmorAttributes.MageArmor = 0;

			Attributes.SpellDamage = 0;
			Attributes.CastSpeed = 0;
			Attributes.DefendChance = 2;
			Attributes.LowerManaCost = 0;
			Attributes.LowerRegCost = 0;
			Attributes.ReflectPhysical = 2;
			Attributes.Luck = 0;
			Attributes.NightSight = 0;
			Attributes.BonusDex = 0;
			Attributes.BonusInt = 0;
			Attributes.BonusStr = 2;
			Attributes.RegenHits = 0;
			Attributes.RegenMana = 0;
			Attributes.RegenStam = 3;

			PhysicalBonus = 2;
			ColdBonus = 0;
			EnergyBonus = 0;
			FireBonus = 3;
			PoisonBonus = 0;
		}

		public SkinDragonHelm( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( Cleanup ), this );
		}

		private void Cleanup( object state )
		{
			Item item = new LeatherCap();
			((BaseArmor)item).Resource = CraftResource.DragonSkin;
			Server.Misc.Cleanup.DoCleanup( (Item)state, item );
		}
	}
}