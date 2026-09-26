using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a bloody corpse" )]
	public class BloodDemigod : BaseCreature
	{
		[Constructable]
		public BloodDemigod () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "goddess" );
			Title = "the demigoddess";
			Body = 436;
			Hue = 0x845;
			BaseSoundID = 0x4B0;

			SetStr( 986, 1185 );
			SetDex( 177, 255 );
			SetInt( 151, 250 );

			SetHits( 592, 711 );

			SetDamage( 22, 29 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Fire, 25 );
			SetDamageType( ResistanceType.Energy, 25 );

			SetResistance( ResistanceType.Physical, 65, 80 );
			SetResistance( ResistanceType.Fire, 60, 80 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.Anatomy, 25.1, 50.0 );
			SetSkill( SkillName.Psychology, 90.1, 100.0 );
			SetSkill( SkillName.Magery, 95.5, 100.0 );
			SetSkill( SkillName.Meditation, 25.1, 50.0 );
			SetSkill( SkillName.MagicResist, 100.5, 150.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.FistFighting, 90.1, 100.0 );

			Fame = 24000;
			Karma = -24000;

			VirtualArmor = 90;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.MedScrolls, 3 );
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );
			DemigodBlood ingut = new DemigodBlood();
   			ingut.Amount = Utility.RandomMinMax( 1, 3 );
   			c.DropItem(ingut);

			int killerLuck = MobileUtilities.GetLuckFromKiller( this );
			PlayerMobile killer = MobileUtilities.TryGetKillingPlayer( this );

			if ( GetPlayerInfo.LuckyKiller( killerLuck ) && Utility.RandomMinMax( 1, 5 ) == 1 && !Server.Misc.PlayerSettings.GetSpecialsKilled( killer, "BloodDemigod" ) )
			{
				Server.Misc.PlayerSettings.SetSpecialsKilled( killer, "BloodDemigod", true );
				ManualOfItems book = new ManualOfItems();
					book.Hue = 0x870;
					book.Name = "Chest of Bloody Relics";
					book.m_Charges = 1;
					book.m_Skill_1 = 99;
					book.m_Skill_2 = 2;
					book.m_Skill_3 = 23;
					book.m_Skill_4 = 0;
					book.m_Skill_5 = 0;
					book.m_Value_1 = 10.0;
					book.m_Value_2 = 10.0;
					book.m_Value_3 = 10.0;
					book.m_Value_4 = 0.0;
					book.m_Value_5 = 0.0;
					book.m_Slayer_1 = 24;
					book.m_Slayer_2 = 0;
					book.m_Owner = killer;
					book.m_Extra = "of Blood";
					book.m_FromWho = "Taken from the Demigoddess of Blood";
					book.m_HowGiven = "Acquired by";
					book.m_Points = 150;
					book.m_Hue = 0x870;
					c.DropItem( book );
			}
		}

		public override int Meat{ get{ return 1; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int Hides{ get{ return 18; } }
		public override HideType HideType{ get{ return HideType.Hellish; } }
		public override int Skin{ get{ return Utility.Random(6); } }
		public override SkinType SkinType{ get{ return SkinType.Demon; } }
		public override int Skeletal{ get{ return Utility.Random(8); } }
		public override SkeletalType SkeletalType{ get{ return SkeletalType.Devil; } }

		public void DrainLife()
		{
			ArrayList list = new ArrayList();

			foreach ( Mobile m in this.GetMobilesInRange( 2 ) )
			{
				if ( m == this || !CanBeHarmful( m ) )
					continue;

				if ( m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team) )
					list.Add( m );
				else if ( m.Player )
					list.Add( m );
			}

			foreach ( Mobile m in list )
			{
				if ( !m.CheckSkill( SkillName.MagicResist, 0, 125 ) && !Server.Items.HiddenTrap.IAmAWeaponSlayer( m, this ) )
				{
					DoHarmful( m );

					m.FixedParticles( 0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist );
					m.PlaySound( 0x231 );

					m.SendMessage( "You feel the life drain out of you!" );

					int toDrain = Utility.RandomMinMax( 10, 40 );

					Hits += toDrain;
					m.Damage( toDrain, this );
				}
			}
		}

		public override void OnGaveMeleeAttack( Mobile m )
		{
			base.OnGaveMeleeAttack( m );

			if ( 0.1 >= Utility.RandomDouble() )
				DrainLife();
		}

		public override void OnGotMeleeAttack( Mobile m )
		{
			base.OnGotMeleeAttack( m );

			if ( 0.1 >= Utility.RandomDouble() )
				DrainLife();
		}

		public BloodDemigod( Serial serial ) : base( serial )
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