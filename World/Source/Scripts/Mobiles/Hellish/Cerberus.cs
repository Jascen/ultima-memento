using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a cerberus corpse" )]
	public class Cerberus : BaseCreature
	{
		public override bool HasBreath{ get{ return true; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override double BreathEffectDelay{ get{ return 0.1; } }
		public override void BreathDealDamage( Mobile target, int form ){ base.BreathDealDamage( target, 9 ); }

		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.BleedAttack;
		}

		[Constructable]
		public Cerberus() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a cerberus";
			Body = 141;

			SetStr( 402, 450 );
			SetDex( 181, 205 );
			SetInt( 136, 160 );

			SetHits( 275, 325 );

			SetDamage( 18, 24 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Fire, 50 );

			SetResistance( ResistanceType.Physical, 45, 55 );
			SetResistance( ResistanceType.Fire, 70, 90 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.MagicResist, 65.1, 80.0 );
			SetSkill( SkillName.Tactics, 65.1, 90.0 );
			SetSkill( SkillName.FistFighting, 65.1, 80.0 );

			Fame = 8400;
			Karma = -8400;

			VirtualArmor = 35;

			Tamable = true;
			ControlSlots = 2;
			MinTameSkill = 105.5;

			PackItem( new SulfurousAsh( 20 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Meager );
		}

		public override int Meat{ get{ return 10; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }
		public override int Hides{ get{ return 15; } }
		public override HideType HideType{ get{ return HideType.Hellish; } }

        public override int GetDeathSound(){ return 0x56F; }
        public override int GetAttackSound(){ return 0x570; }
        public override int GetIdleSound(){ return 0x571; }
        public override int GetAngerSound(){ return 0x572; }
        public override int GetHurtSound(){ return 0x573; }

		public Cerberus( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}