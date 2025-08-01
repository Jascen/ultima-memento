using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an elemental corpse" )]
	public class GoldenElemental : BaseCreature
	{
		private int m_RocksAmount = 0;

		public override double DispelDifficulty{ get{ return 120.5; } }
		public override double DispelFocus{ get{ return 45.0; } }

		public override int BreathPhysicalDamage{ get{ return 100; } }
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathColdDamage{ get{ return 0; } }
		public override int BreathPoisonDamage{ get{ return 0; } }
		public override int BreathEnergyDamage{ get{ return 0; } }
		public override int BreathEffectHue{ get{ return 0; } }
		public override int BreathEffectSound{ get{ return 0x65A; } }
		public override int BreathEffectItemID{ get{ return 0; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool HasBreath{ get{ return true; } }
		public override double BreathEffectDelay{ get{ return 0.1; } }
		public override void BreathDealDamage( Mobile target, int form ){ base.BreathDealDamage( target, 29 ); }

		[Constructable]
		public GoldenElemental( int rocksAmount ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a golden elemental";
			Body = Utility.RandomList( 14, 446, 974 );
			Resource = CraftResource.Gold;
			Hue = CraftResources.GetClr( Resource );
			BaseSoundID = 268;

			SetStr( 226, 255 );
			SetDex( 126, 145 );
			SetInt( 71, 92 );

			SetHits( 136, 153 );

			SetDamage( 9, 16 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 60, 75 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 50.1, 95.0 );
			SetSkill( SkillName.Tactics, 60.1, 100.0 );
			SetSkill( SkillName.FistFighting, 60.1, 100.0 );

			Fame = 3500;
			Karma = -3500;

			VirtualArmor = 60;

			if ( Utility.Random(10) == 0 )
			{
				Body = 821;
				BeefUp( (BaseCreature)this, 2 );
			}

			m_RocksAmount = rocksAmount; 
		}
		[Constructable]
		public GoldenElemental() : this( Utility.RandomMinMax( 5, 10 ) )
		{
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Gems, 1 );
		}

		public override bool AutoDispel{ get{ return true; } }
		public override bool BleedImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 1; } }
		public override int Rocks{ get{ return m_RocksAmount; } }
		public override RockType RockType{ get{ return ResourceRocks(); } }

		public GoldenElemental( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
			
			writer.Write(m_RocksAmount);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			switch( version )
			{
				case 1:
					m_RocksAmount = reader.ReadInt();
					goto case 0;

				case 0:
					if ( version == 0 )
						m_RocksAmount = Utility.RandomMinMax( 5, 10 );
					break;
			}
		}
	}
}