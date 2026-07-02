using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a morlock corpse" )]
	public class Morlock : BaseCreature
	{
		[Constructable]
		public Morlock() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a morlock";
			Body = 332;
			BaseSoundID = 427;

			SetStr( 156, 185 );
			SetDex( 111, 135 );
			SetInt( 46, 70 );

			SetHits( 106, 123 );
			SetMana( 0 );

			SetDamage( 10, 15 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 25, 35 );
			SetResistance( ResistanceType.Cold, 85, 95 );
			SetResistance( ResistanceType.Poison, 5, 10 );
			SetResistance( ResistanceType.Energy, 5, 10 );

			SetSkill( SkillName.MagicResist, 25.1, 40.0 );
			SetSkill( SkillName.Tactics, 70.1, 100.0 );
			SetSkill( SkillName.FistFighting, 45.1, 70.0 );
			SetSkill( SkillName.Marksmanship, 45.1, 70.0 );

			Fame = 1500;
			Karma = -1500;

			VirtualArmor = 30;

			if ( Utility.RandomBool() )
			{
				AI = AIType.AI_Archer;
				SetDex( 156, 185 );
				MonsterGloves gloves = new MonsterGloves();
				gloves.ThrowType = "Stones";
				AddItem( gloves );

				ThrowingWeapon toss = new ThrowingWeapon( Utility.RandomMinMax( 10, 30 ) );
				toss.Ammo = ThrowingWeaponType.Stones;
				PackItem( toss );
			}
			else
			{
				PackItem( new Club() );
			}
		}

		public override int Meat{ get{ return 2; } }
		public override int Cloths{ get{ return 8; } }
		public override ClothType ClothType{ get{ return ClothType.Wooly; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}

		public Morlock( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}