using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a minotaur corpse" )]
	public class Minotaur : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.ParalyzingBlow;
		}

		[Constructable]
		public Minotaur() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 ) // NEED TO CHECK
		{
			Name = "a minotaur";
			Body = 263;
			BaseSoundID = 0x54E;

			SetStr( 166, 195 );
			SetDex( 46, 65 );
			SetInt( 46, 70 );

			SetHits( 100, 117 );
			SetMana( 0 );

			SetDamage( 9, 11 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 30, 35 );
			SetResistance( ResistanceType.Fire, 15, 25 );
			SetResistance( ResistanceType.Cold, 15, 25 );
			SetResistance( ResistanceType.Poison, 15, 25 );
			SetResistance( ResistanceType.Energy, 25 );

			SetSkill( SkillName.MagicResist, 55.1, 70.0 );
			SetSkill( SkillName.Tactics, 60.1, 70.0 );
			SetSkill( SkillName.FistFighting, 70.1, 80.0 );
			SetSkill( SkillName.Marksmanship, 70.1, 80.0 );

			Fame = 4000;
			Karma = -4000;

			VirtualArmor = 32;

			if ( Utility.RandomBool() )
			{
				AI = AIType.AI_Archer;
				SetDex( 166, 195 );
				MonsterGloves gloves = new MonsterGloves();
				gloves.ThrowType = "Axes";
				AddItem( gloves );

				ThrowingWeapon toss = new ThrowingWeapon( Utility.RandomMinMax( 10, 30 ) );
				toss.Ammo = ThrowingWeaponType.Axes;
				PackItem( toss );
			}
		}

		public override int Skeletal{ get{ return Utility.Random(3); } }
		public override SkeletalType SkeletalType{ get{ return SkeletalType.Minotaur; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
		}

		public Minotaur( Serial serial ) : base( serial )
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
