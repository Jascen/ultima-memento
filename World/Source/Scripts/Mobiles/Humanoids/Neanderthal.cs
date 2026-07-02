using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a neanderthal corpse" )]
	public class Neanderthal : BaseCreature
	{
		public override int BreathPhysicalDamage{ get{ return 100; } }
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathColdDamage{ get{ return 0; } }
		public override int BreathPoisonDamage{ get{ return 0; } }
		public override int BreathEnergyDamage{ get{ return 0; } }
		public override int BreathEffectHue{ get{ return 0; } }
		public override int BreathEffectSound{ get{ return 0x65A; } }
		public override int BreathEffectItemID{ get{ return 0x1365; } } // SMALL BOULDER
		public override bool HasBreath{ get{ return true; } }
		public override double BreathEffectDelay{ get{ return 0.1; } }
		public override void BreathDealDamage( Mobile target, int form ){ base.BreathDealDamage( target, 7 ); }
		public override double BreathDamageScalar{ get{ return 0.35; } }

		[Constructable]
		public Neanderthal () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a neanderthal";
			Body = Utility.RandomList( 267, 162 );
			BaseSoundID = 427;

			SetStr( 176, 205 );
			SetDex( 46, 65 );
			SetInt( 46, 70 );

			SetHits( 106, 123 );

			SetDamage( 8, 14 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 25, 35 );
			SetResistance( ResistanceType.Cold, 15, 25 );
			SetResistance( ResistanceType.Poison, 5, 15 );
			SetResistance( ResistanceType.Energy, 5, 15 );

			SetSkill( SkillName.MagicResist, 45.1, 60.0 );
			SetSkill( SkillName.Tactics, 50.1, 70.0 );
			SetSkill( SkillName.FistFighting, 50.1, 70.0 );
			SetSkill( SkillName.Marksmanship, 50.1, 70.0 );

			Fame = 3500;
			Karma = -3500;

			VirtualArmor = 40;

			if ( Utility.RandomBool() )
			{
				AI = AIType.AI_Archer;
				SetDex( 176, 205 );
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

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int Meat{ get{ return 2; } }
		public override int Skeletal{ get{ return Utility.Random(3); } }
		public override SkeletalType SkeletalType{ get{ return SkeletalType.Brittle; } }

		public Neanderthal( Serial serial ) : base( serial )
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