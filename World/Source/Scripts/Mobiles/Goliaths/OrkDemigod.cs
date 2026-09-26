using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a demigod corpse" )]
	public class OrkDemigod : BaseCreature
	{
		[Constructable]
		public OrkDemigod () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "ork" );
			Title = "the ork demigod";
			Body = 189;
			BaseSoundID = 0x59D;

			SetStr( 1096, 1185 );
			SetDex( 86, 175 );
			SetInt( 686, 775 );

			SetHits( 658, 711 );

			SetDamage( 29, 35 );

			SetDamageType( ResistanceType.Physical, 75 );
			SetDamageType( ResistanceType.Fire, 25 );

			SetResistance( ResistanceType.Physical, 65, 75 );
			SetResistance( ResistanceType.Fire, 80, 90 );
			SetResistance( ResistanceType.Cold, 70, 80 );
			SetResistance( ResistanceType.Poison, 60, 70 );
			SetResistance( ResistanceType.Energy, 60, 70 );

			SetSkill( SkillName.Psychology, 80.1, 100.0 );
			SetSkill( SkillName.Magery, 80.1, 100.0 );
			SetSkill( SkillName.Meditation, 52.5, 75.0 );
			SetSkill( SkillName.MagicResist, 100.5, 150.0 );
			SetSkill( SkillName.Tactics, 97.6, 100.0 );
			SetSkill( SkillName.FistFighting, 97.6, 100.0 );

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 70;

			if ( 1 == Utility.RandomMinMax( 0, 1 ) )
			{
				LootChest MyChest = new LootChest( Server.Misc.IntelligentAction.FameBasedLevel( this ) );
				MyChest.Name = "orkish war chest";
				MyChest.Hue = 0x8A1;
				MyChest.ItemID = 0xE41;
				PackItem( MyChest );
			}
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			PlayerMobile killer = MobileUtilities.TryGetKillingPlayer( this );

			if ( killer != null )
			{
				int killerLuck = MobileUtilities.GetLuckFromKiller( this );

				if ( GetPlayerInfo.LuckyKiller( killerLuck ) && Utility.RandomMinMax( 1, 5 ) == 1 && !Server.Misc.PlayerSettings.GetSpecialsKilled( killer, "OrkDemigod" ) )
				{
					Server.Misc.PlayerSettings.SetSpecialsKilled( killer, "OrkDemigod", true );
					ManualOfItems book = new ManualOfItems();
						book.Hue = 0x7D4;
						book.Name = "Chest of Orcish Relics";
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
						book.m_Slayer_1 = 2;
						book.m_Slayer_2 = 0;
						book.m_Owner = killer;
						book.m_Extra = "of the Orcs";
						book.m_FromWho = "Taken from the Orc Demigod";
						book.m_HowGiven = "Acquired by";
						book.m_Points = 150;
						book.m_Hue = 0x7D4;
						c.DropItem( book );
				}
			}
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 3 );
			AddLoot( LootPack.Gems, 3 );
		}

		public override int GetIdleSound()
		{
			return 0x2D3;
		}

		public override int GetHurtSound()
		{
			return 0x2D1;
		}

		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool AutoDispel{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int TreasureMapLevel{ get{ return 6; } }
		public override int Meat{ get{ return 10; } }
		public override int Skeletal{ get{ return Utility.Random(10); } }
		public override SkeletalType SkeletalType{ get{ if ( Utility.RandomBool() ){ return SkeletalType.Orc; } else { return SkeletalType.Colossal; } } }


		public OrkDemigod( Serial serial ) : base( serial )
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