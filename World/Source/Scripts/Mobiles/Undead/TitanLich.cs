using System;
using Server;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a giant corpse" )]
	public class TitanLich : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.Dismount;
		}

		public override int BreathPhysicalDamage{ get{ return 0; } }
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathColdDamage{ get{ return 0; } }
		public override int BreathPoisonDamage{ get{ return 0; } }
		public override int BreathEnergyDamage{ get{ return 100; } }
		public override int BreathEffectHue{ get{ return 0x9C2; } }
		public override int BreathEffectSound{ get{ return 0x665; } }
		public override int BreathEffectItemID{ get{ return 0x3818; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool HasBreath{ get{ return true; } }
		public override double BreathEffectDelay{ get{ return 0.1; } }
		public override void BreathDealDamage( Mobile target, int form ){ base.BreathDealDamage( target, 14 ); }

		[Constructable]
		public TitanLich() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "ancient lich" );
			Title = "the titan lich";
			Body = 771;
			Hue = Utility.RandomList( 0xB37, 0xB1B, 0x9B0, 0x960 );
			BaseSoundID = 0x47D;

			SetStr( 216, 305 );
			SetDex( 96, 115 );
			SetInt( 966, 1045 );

			SetHits( 560, 595 );

			SetDamage( 15, 27 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Cold, 40 );
			SetDamageType( ResistanceType.Energy, 40 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 25, 30 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 25, 30 );

			SetSkill( SkillName.Psychology, 120.1, 130.0 );
			SetSkill( SkillName.Magery, 120.1, 130.0 );
			SetSkill( SkillName.Meditation, 100.1, 101.0 );
			SetSkill( SkillName.Poisoning, 100.1, 101.0 );
			SetSkill( SkillName.MagicResist, 175.2, 200.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.FistFighting, 75.1, 100.0 );

			Fame = 23000;
			Karma = -23000;

			VirtualArmor = 60;
			PackReg( 30, 275 );

			int[] list = new int[]
				{
					0x1CF0, 0x1CEF, 0x1CEE, 0x1CED, 0x1CE9, 0x1DA0, 0x1DAE // pieces
				};

			PackItem( new BodyPart( Utility.RandomList( list ) ) );
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			int killerLuck = MobileUtilities.GetLuckFromKiller( this );
			if ( GetPlayerInfo.LuckyKiller( killerLuck ) && Utility.RandomMinMax( 1, 4 ) == 1 )
			{
				BaseArmor skin = null;
				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0: skin = new LeatherLegs(); skin.Resource = CraftResource.DeadSkin; c.DropItem( skin ); break;
					case 1: skin = new LeatherGloves(); skin.Resource = CraftResource.DeadSkin; c.DropItem( skin ); break;
					case 2: skin = new LeatherGorget(); skin.Resource = CraftResource.DeadSkin; c.DropItem( skin ); break;
					case 3: skin = new LeatherArms(); skin.Resource = CraftResource.DeadSkin; c.DropItem( skin ); break;
					case 4: skin = new LeatherChest(); skin.Resource = CraftResource.DeadSkin; c.DropItem( skin ); break;
					case 5: skin = new LeatherCap(); skin.Resource = CraftResource.DeadSkin; c.DropItem( skin ); break;
				}
			}

			if ( GetPlayerInfo.LuckyKiller( killerLuck ) && Utility.RandomMinMax( 1, 4 ) == 1 )
			{
				LootChest MyChest = new LootChest( Server.Misc.IntelligentAction.FameBasedLevel( this ) );
				MyChest.Name = "titan lich chest";
				MyChest.Hue = this.Hue;
				c.DropItem( MyChest );
			}
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 3 );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override int Meat{ get{ return 4; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int Hides{ get{ return 18; } }
		public override HideType HideType{ get{ if ( Utility.RandomBool() ){ return HideType.Necrotic; } else { return HideType.Goliath; } } }
		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override int Skeletal{ get{ return Utility.Random(4); } }
		public override SkeletalType SkeletalType{ get{ return SkeletalType.Lich; } }

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if ( Utility.RandomMinMax( 1, 2 ) == 1 )
			{
				int goo = 0;

				foreach ( Item splash in this.GetItemsInRange( 10 ) ){ if ( splash is MonsterSplatter && splash.Name == "green blood" ){ goo++; } }

				if ( goo == 0 )
				{
					MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "green blood", 0x7D1, 0 );
				}
			}
		}

		public TitanLich( Serial serial ) : base( serial )
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