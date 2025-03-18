using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a demon dog corpse" )]
	public class DemonDog : BaseMount
	{
		public override bool HasBreath{ get{ return true; } }
		public override double BreathEffectDelay{ get{ return 0.1; } }
		public override void BreathDealDamage( Mobile target, int form ){ base.BreathDealDamage( target, 17 ); }

		[Constructable]
		public DemonDog() : this( "a demon dog" )
		{
		}

		[Constructable]
		public DemonDog( string name ) : base( name, 277, 16017, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			BaseSoundID = 0xE5;
			Hue = Utility.RandomList( 0xABD, 0x9A2, 0x94C );

			SetStr( 202, 250 );
			SetDex( 181, 205 );
			SetInt( 136, 160 );

			SetHits( 166, 225 );

			SetDamage( 14, 22 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 80 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 70, 80 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 65.1, 80.0 );
			SetSkill( SkillName.Tactics, 65.1, 90.0 );
			SetSkill( SkillName.FistFighting, 65.1, 80.0 );

			Fame = 6400;
			Karma = -6400;

			VirtualArmor = 35;

			Tamable = true;
			ControlSlots = 2;
			MinTameSkill = 95.5;

			PackItem( new SulfurousAsh( 10 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Rich );
		}

		public override int Meat{ get{ return 3; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override int TreasureMapLevel { get { return 2; } }
		public override int Hides { get { return 5; } }
		public override HideType HideType { get { return HideType.Hellish; } }

        public DemonDog( Serial serial ) : base( serial )
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
			if ( Hue != 0xABD && Hue != 0x9A2 && Hue != 0x94C ){ Hue = Utility.RandomList( 0xABD, 0x9A2, 0x94C ); }
		}
	}
}
