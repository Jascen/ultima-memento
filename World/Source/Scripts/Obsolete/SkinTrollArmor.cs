using System;
using Server;

namespace Server.Items
{
	public class SkinTrollLegs : LeatherLegs ///////////////////////////////////////////////////////
	{
		[Constructable]
		public SkinTrollLegs()
		{
			Name = "Troll Skin Leggings";
			Hue = CraftResources.GetHue( CraftResource.TrollSkin );

			ArmorAttributes.DurabilityBonus = 30;
			ArmorAttributes.LowerStatReq = 0;
			ArmorAttributes.MageArmor = 0;

			Attributes.SpellDamage = 0;
			Attributes.CastSpeed = 0;
			Attributes.DefendChance = 0;
			Attributes.LowerManaCost = 0;
			Attributes.LowerRegCost = 0;
			Attributes.ReflectPhysical = 0;
			Attributes.Luck = 0;
			Attributes.NightSight = 0;
			Attributes.BonusDex = 0;
			Attributes.BonusInt = 0;
			Attributes.BonusStr = 1;
			Attributes.RegenHits = 5;
			Attributes.RegenMana = 0;
			Attributes.RegenStam = 0;

			PhysicalBonus = 2;
			ColdBonus = 0;
			EnergyBonus = 0;
			FireBonus = 0;
			PoisonBonus = 0;
		}

		public SkinTrollLegs( Serial serial ) : base( serial )
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
			((BaseArmor)item).Resource = CraftResource.TrollSkin;
			Server.Misc.Cleanup.DoCleanup( (Item)state, item );
		}
	}
	public class SkinTrollGloves : LeatherGloves ///////////////////////////////////////////////////
	{
		[Constructable]
		public SkinTrollGloves()
		{
			Name = "Troll Skin Gloves";
			Hue = CraftResources.GetHue( CraftResource.TrollSkin );

			ArmorAttributes.DurabilityBonus = 30;
			ArmorAttributes.LowerStatReq = 0;
			ArmorAttributes.MageArmor = 0;

			Attributes.SpellDamage = 0;
			Attributes.CastSpeed = 0;
			Attributes.DefendChance = 0;
			Attributes.LowerManaCost = 0;
			Attributes.LowerRegCost = 0;
			Attributes.ReflectPhysical = 0;
			Attributes.Luck = 0;
			Attributes.NightSight = 0;
			Attributes.BonusDex = 0;
			Attributes.BonusInt = 0;
			Attributes.BonusStr = 1;
			Attributes.RegenHits = 5;
			Attributes.RegenMana = 0;
			Attributes.RegenStam = 0;

			PhysicalBonus = 2;
			ColdBonus = 0;
			EnergyBonus = 0;
			FireBonus = 0;
			PoisonBonus = 0;
		}

		public SkinTrollGloves( Serial serial ) : base( serial )
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
			((BaseArmor)item).Resource = CraftResource.TrollSkin;
			Server.Misc.Cleanup.DoCleanup( (Item)state, item );
		}
	}
	public class SkinTrollGorget : LeatherGorget ///////////////////////////////////////////////////
	{
		[Constructable]
		public SkinTrollGorget()
		{
			Name = "Troll Skin Gorget";
			Hue = CraftResources.GetHue( CraftResource.TrollSkin );

			ArmorAttributes.DurabilityBonus = 30;
			ArmorAttributes.LowerStatReq = 0;
			ArmorAttributes.MageArmor = 0;

			Attributes.SpellDamage = 0;
			Attributes.CastSpeed = 0;
			Attributes.DefendChance = 0;
			Attributes.LowerManaCost = 0;
			Attributes.LowerRegCost = 0;
			Attributes.ReflectPhysical = 0;
			Attributes.Luck = 0;
			Attributes.NightSight = 0;
			Attributes.BonusDex = 0;
			Attributes.BonusInt = 0;
			Attributes.BonusStr = 1;
			Attributes.RegenHits = 5;
			Attributes.RegenMana = 0;
			Attributes.RegenStam = 0;

			PhysicalBonus = 2;
			ColdBonus = 0;
			EnergyBonus = 0;
			FireBonus = 0;
			PoisonBonus = 0;
		}

		public SkinTrollGorget( Serial serial ) : base( serial )
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
			((BaseArmor)item).Resource = CraftResource.TrollSkin;
			Server.Misc.Cleanup.DoCleanup( (Item)state, item );
		}
	}
	public class SkinTrollArms : LeatherArms ///////////////////////////////////////////////////////
	{
		[Constructable]
		public SkinTrollArms()
		{
			Name = "Troll Skin Arms";
			Hue = CraftResources.GetHue( CraftResource.TrollSkin );

			ArmorAttributes.DurabilityBonus = 30;
			ArmorAttributes.LowerStatReq = 0;
			ArmorAttributes.MageArmor = 0;

			Attributes.SpellDamage = 0;
			Attributes.CastSpeed = 0;
			Attributes.DefendChance = 0;
			Attributes.LowerManaCost = 0;
			Attributes.LowerRegCost = 0;
			Attributes.ReflectPhysical = 0;
			Attributes.Luck = 0;
			Attributes.NightSight = 0;
			Attributes.BonusDex = 0;
			Attributes.BonusInt = 0;
			Attributes.BonusStr = 1;
			Attributes.RegenHits = 5;
			Attributes.RegenMana = 0;
			Attributes.RegenStam = 0;

			PhysicalBonus = 2;
			ColdBonus = 0;
			EnergyBonus = 0;
			FireBonus = 0;
			PoisonBonus = 0;
		}

		public SkinTrollArms( Serial serial ) : base( serial )
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
			((BaseArmor)item).Resource = CraftResource.TrollSkin;
			Server.Misc.Cleanup.DoCleanup( (Item)state, item );
		}
	}
	public class SkinTrollChest : LeatherChest /////////////////////////////////////////////////////
	{
		[Constructable]
		public SkinTrollChest()
		{
			Name = "Troll Skin Tunic";
			Hue = CraftResources.GetHue( CraftResource.TrollSkin );

			ArmorAttributes.DurabilityBonus = 30;
			ArmorAttributes.LowerStatReq = 0;
			ArmorAttributes.MageArmor = 0;

			Attributes.SpellDamage = 0;
			Attributes.CastSpeed = 0;
			Attributes.DefendChance = 0;
			Attributes.LowerManaCost = 0;
			Attributes.LowerRegCost = 0;
			Attributes.ReflectPhysical = 0;
			Attributes.Luck = 0;
			Attributes.NightSight = 0;
			Attributes.BonusDex = 0;
			Attributes.BonusInt = 0;
			Attributes.BonusStr = 1;
			Attributes.RegenHits = 5;
			Attributes.RegenMana = 0;
			Attributes.RegenStam = 0;

			PhysicalBonus = 2;
			ColdBonus = 0;
			EnergyBonus = 0;
			FireBonus = 0;
			PoisonBonus = 0;
		}

		public SkinTrollChest( Serial serial ) : base( serial )
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
			((BaseArmor)item).Resource = CraftResource.TrollSkin;
			Server.Misc.Cleanup.DoCleanup( (Item)state, item );
		}
	}
	public class SkinTrollHelm : LeatherCap /////////////////////////////////////////
	{
		[Constructable]
		public SkinTrollHelm()
		{
			Name = "Troll Skin Cap";
			Hue = CraftResources.GetHue( CraftResource.TrollSkin );

			ArmorAttributes.DurabilityBonus = 30;
			ArmorAttributes.LowerStatReq = 0;
			ArmorAttributes.MageArmor = 0;

			Attributes.SpellDamage = 0;
			Attributes.CastSpeed = 0;
			Attributes.DefendChance = 0;
			Attributes.LowerManaCost = 0;
			Attributes.LowerRegCost = 0;
			Attributes.ReflectPhysical = 0;
			Attributes.Luck = 0;
			Attributes.NightSight = 0;
			Attributes.BonusDex = 0;
			Attributes.BonusInt = 0;
			Attributes.BonusStr = 1;
			Attributes.RegenHits = 5;
			Attributes.RegenMana = 0;
			Attributes.RegenStam = 0;

			PhysicalBonus = 2;
			ColdBonus = 0;
			EnergyBonus = 0;
			FireBonus = 0;
			PoisonBonus = 0;
		}

		public SkinTrollHelm( Serial serial ) : base( serial )
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
			((BaseArmor)item).Resource = CraftResource.TrollSkin;
			Server.Misc.Cleanup.DoCleanup( (Item)state, item );
		}
	}
}