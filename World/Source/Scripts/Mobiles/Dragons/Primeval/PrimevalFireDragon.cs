using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a dragon corpse" )]
	public class PrimevalFireDragon : BaseCreature
	{
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool HasBreath{ get{ return true; } }
		public override double BreathEffectDelay{ get{ return 0.1; } }
		public override void BreathDealDamage( Mobile target, int form ){ base.BreathDealDamage( target, 9 ); }

		[Constructable]
		public PrimevalFireDragon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "dragon" );
			Title = "the primeval fire dragon";
			Body = 722;
			BaseSoundID = 362;
			Resource = CraftResource.UmberScales;

			SetStr( 896, 985 );
			SetDex( 86, 175 );
			SetInt( 586, 675 );

			SetHits( 558, 611 );

			SetDamage( 23, 30 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Fire, 50 );

			SetResistance( ResistanceType.Physical, 65, 75 );
			SetResistance( ResistanceType.Fire, 100 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 60, 70 );
			SetResistance( ResistanceType.Energy, 60, 70 );

			SetSkill( SkillName.Psychology, 80.1, 100.0 );
			SetSkill( SkillName.Magery, 80.1, 100.0 );
			SetSkill( SkillName.Meditation, 52.5, 75.0 );
			SetSkill( SkillName.MagicResist, 100.5, 150.0 );
			SetSkill( SkillName.Tactics, 97.6, 100.0 );
			SetSkill( SkillName.FistFighting, 97.6, 100.0 );

			Fame = 20000;
			Karma = -20000;

			VirtualArmor = 60;

			Tamable = true;
			ControlSlots = 4;
			MinTameSkill = 114.9;
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			Server.Mobiles.Dragons.DropSpecial( this, this.Name + " " + this.Title, c, 10, 0x54C );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Gems, 4 );
		}

		public override bool AutoDispel{ get{ return !Controlled; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int Meat{ get{ return 22; } }
		public override int Hides{ get{ return 25; } }
		public override HideType HideType{ get{ return HideType.Draconic; } }
		public override int Scales{ get{ return 8; } }
		public override ScaleType ScaleType{ get{ return ResourceScales(); } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override bool CanAngerOnTame { get { return true; } }
		public override int Skin{ get{ return Utility.Random(5); } }
		public override SkinType SkinType{ get{ return SkinType.Dragon; } }
		public override int Skeletal{ get{ return Utility.Random(5); } }
		public override SkeletalType SkeletalType{ get{ return SkeletalType.Draco; } }

        public override int GetAngerSound()
        {
            return 0x63E;
        }

        public override int GetDeathSound()
        {
            return 0x63F;
        }

        public override int GetHurtSound()
        {
            return 0x640;
        }

        public override int GetIdleSound()
        {
            return 0x641;
        }

		public PrimevalFireDragon( Serial serial ) : base( serial )
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