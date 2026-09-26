using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a liche's corpse" )]
	public class LichKing : BaseCreature
	{
		[Constructable]
		public LichKing() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "ancient lich" );
			Title = "the king of the dead";
			Body = 768;
			BaseSoundID = 609;

			SetStr( 316, 405 );
			SetDex( 196, 215 );
			SetInt( 1066, 1145 );

			SetHits( 660, 695 );

			SetDamage( 29, 35 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Cold, 40 );
			SetDamageType( ResistanceType.Energy, 40 );

			SetResistance( ResistanceType.Physical, 65, 75 );
			SetResistance( ResistanceType.Fire, 35, 40 );
			SetResistance( ResistanceType.Cold, 60, 70 );
			SetResistance( ResistanceType.Poison, 60, 70 );
			SetResistance( ResistanceType.Energy, 35, 40 );

			SetSkill( SkillName.Psychology, 120.1, 130.0 );
			SetSkill( SkillName.Magery, 120.1, 130.0 );
			SetSkill( SkillName.Meditation, 100.1, 101.0 );
			SetSkill( SkillName.Poisoning, 100.1, 101.0 );
			SetSkill( SkillName.MagicResist, 175.2, 200.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.FistFighting, 75.1, 100.0 );

			Fame = 28000;
			Karma = -28000;

			VirtualArmor = 60;
			PackReg( 30, 275 );
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );
			GhostlyDust ingut = new GhostlyDust();
   			ingut.Amount = Utility.RandomMinMax( 1, 3 );
   			c.DropItem(ingut);

			PlayerMobile killer = MobileUtilities.TryGetKillingPlayer( this );
			if ( killer != null )
			{
				int killerLuck = MobileUtilities.GetLuckFromKiller( this );

				if ( GetPlayerInfo.LuckyKiller( killerLuck ) && Utility.RandomMinMax( 1, 5 ) == 1 && !Server.Misc.PlayerSettings.GetSpecialsKilled( killer, "LichKing" ) )
				{
					Server.Misc.PlayerSettings.SetSpecialsKilled( killer, "LichKing", true );
					ManualOfItems book = new ManualOfItems();
						book.Hue = 0x845;
						book.Name = "Chest of Lich King Relics";
						book.m_Charges = 1;
						book.m_Skill_1 = 99;
						book.m_Skill_2 = 0;
						book.m_Skill_3 = 0;
						book.m_Skill_4 = 0;
						book.m_Skill_5 = 0;
						book.m_Value_1 = 20.0;
						book.m_Value_2 = 0.0;
						book.m_Value_3 = 0.0;
						book.m_Value_4 = 0.0;
						book.m_Value_5 = 0.0;
						book.m_Slayer_1 = 6;
						book.m_Slayer_2 = 0;
						book.m_Owner = killer;
						book.m_Extra = "of the Lich King";
						book.m_FromWho = "Taken from the King of the Dead";
						book.m_HowGiven = "Acquired by";
						book.m_Points = 150;
						book.m_Hue = 0x845;
						c.DropItem( book );
				}

				if ( GetPlayerInfo.LuckyKiller( killerLuck ) && Server.Misc.IntelligentAction.FameBasedEvent( this ) )
				{
					LootChest MyChest = new LootChest( Server.Misc.IntelligentAction.FameBasedLevel( this ) );
					Server.Misc.ContainerFunctions.MakeTomb( MyChest, this, 1 );
					c.DropItem( MyChest );
				}

				if ( GetPlayerInfo.LuckyKiller( killerLuck ) && Utility.RandomMinMax( 1, 50 ) == 1 )
				{
					DemonPrison shard = new DemonPrison();
					c.DropItem( shard );
				}
			}
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 3 );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override void OnAfterSpawn()
		{
			Server.Misc.IntelligentAction.BeforeMyBirth( this );
			base.OnAfterSpawn();
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );
			Server.Misc.IntelligentAction.DoSpecialAbility( this, attacker );
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );
			Server.Misc.IntelligentAction.DoSpecialAbility( this, defender );
		}

		public override bool OnBeforeDeath()
		{
			Server.Misc.IntelligentAction.BeforeMyDeath( this );
			Server.Misc.IntelligentAction.DropItem( this );
			return base.OnBeforeDeath();
		}

		public override bool Unprovokable{ get{ return true; } }
		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int Skeletal{ get{ return Utility.Random(4); } }
		public override SkeletalType SkeletalType{ get{ return SkeletalType.Lich; } }

        public override int GetAngerSound()
        {
            return 0x61E;
        }

        public override int GetDeathSound()
        {
            return 0x61F;
        }

        public override int GetHurtSound()
        {
            return 0x620;
        }

        public override int GetIdleSound()
        {
            return 0x621;
        }

		public LichKing( Serial serial ) : base( serial )
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