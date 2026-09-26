using System;
using Server;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Misc;
using Server.Regions;

namespace Server.Mobiles
{
	[CorpseName( "a sea serpents corpse" )]
	public class Jormungandr : BaseCreature
	{
		public override int BreathPhysicalDamage{ get{ return 0; } }
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathColdDamage{ get{ return 0; } }
		public override int BreathPoisonDamage{ get{ return 100; } }
		public override int BreathEnergyDamage{ get{ return 0; } }
		public override int BreathEffectHue{ get{ return 0x3F; } }
		public override int BreathEffectSound{ get{ return 0x658; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool HasBreath{ get{ return true; } }
		public override double BreathEffectDelay{ get{ return 0.1; } }
		public override void BreathDealDamage( Mobile target, int form ){ base.BreathDealDamage( target, 10 ); }

		[Constructable]
		public Jormungandr() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Jormungandr";
			Title = "the Midgard Serpent";
			Body = 874;
			Hue = 0xBAB;
			BaseSoundID = 447;

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

			Fame = 24000;
			Karma = -24000;

			VirtualArmor = 70;

			Item Venom = new VenomSack();
				Venom.Name = "venom sack";
				AddItem( Venom );

			CanSwim = true;
			CantWalk = true;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 10 );
		}

        public override void OnAfterSpawn()
        {
			base.OnAfterSpawn();
			Worlds.MoveToRandomOcean( this );
		}

		public override bool OnBeforeDeath()
		{
			PlayerMobile killer = MobileUtilities.TryGetKillingPlayer( this );

			Server.Misc.LoggingFunctions.LogSlayingLord( killer, this.Name );
			return base.OnBeforeDeath();
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			PlayerMobile killer = MobileUtilities.TryGetKillingPlayer( this );

			if ( killer != null )
			{
				int killerLuck = MobileUtilities.GetLuckFromKiller( this );

				if ( GetPlayerInfo.LuckyKiller( killerLuck ) )
				{
					StatueJormungandr trophy = new StatueJormungandr();

					string waters = Server.Misc.Worlds.GetRegionName( killer.Map, killer.Location );

					if ( waters == "the Bottle World of Kuldar" ){ 		waters = "the Kuldar Sea"; }
					else if ( waters == "the Land of Ambrosia" ){ 		waters = "the Ambrosia Lakes"; }
					else if ( waters == "the Island of Umber Veil" ){ 	waters = "the Umber Sea"; }
					else if ( waters == "the Land of Lodoria" ){ 		waters = "the Lodoria Ocean"; }
					else if ( waters == "the Underworld" ){ 			waters = "Carthax Lake"; }
					else if ( waters == "the Serpent Island" ){ 		waters = "the Serpent Seas"; }
					else if ( waters == "the Isles of Dread" ){ 		waters = "the Dreadful Sea"; }
					else if ( waters == "the Savaged Empire" ){ 		waters = "the Savage Seas"; }
					else if ( waters == "the Land of Sosaria" ){ 		waters = "the Sosaria Ocean"; }

					trophy.AnimalWhere = "From " + waters;
					string trophyKiller = killer.Name + " the " + Server.Misc.GetPlayerInfo.GetSkillTitle( killer );
					trophy.AnimalKiller = "Killed by " + trophyKiller;
					c.DropItem( trophy );

					object obj = c;
					if ( obj is Corpse )
					{
						Corpse corpse = (Corpse)obj;
						corpse.VisitedByTaxidermist = true;
					}
				}

				if ( GetPlayerInfo.LuckyKiller( killerLuck ) && Utility.RandomMinMax( 1, 5 ) == 1 && !Server.Misc.PlayerSettings.GetSpecialsKilled( killer, "Jormungandr" ) )
				{
					Server.Misc.PlayerSettings.SetSpecialsKilled( killer, "Jormungandr", true );
					ManualOfItems book = new ManualOfItems();
						book.Hue = 0xB3D;
						book.Name = "Chest of Midgard Relics";
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
						book.m_Slayer_1 = 34;
						book.m_Slayer_2 = 8;
						book.m_Owner = killer;
						book.m_Extra = "of Midgard";
						book.m_FromWho = "Taken from Jormungandr the Serpent of Midgard";
						book.m_HowGiven = "Acquired by";
						book.m_Points = 200;
						book.m_Hue = 0xB3D;
						c.DropItem( book );
				}
			}

			LootChest MyChest = new LootChest( 10 );
			Server.Misc.ContainerFunctions.MakeDemonBox( MyChest, this );
			MyChest.ItemID = Utility.RandomList( 0x2823, 0x2824 );
			MyChest.Hue = Utility.RandomList( 0xB3D, 0xB3E, 0xB3F, 0xB40 );
			c.DropItem( MyChest );
		}

		public override int Meat{ get{ return 10; } }
		public override MeatType MeatType{ get{ return MeatType.Fish; } }
		public override int Hides{ get{ return 10; } }
		public override HideType HideType{ get{ return HideType.Spined; } }
		public override int Scales{ get{ return 8; } }
		public override ScaleType ScaleType{ get{ return ScaleType.Blue; } }
		public override bool BleedImmune{ get{ return true; } }
		public override bool AutoDispel{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override Poison HitPoison{ get{ return Utility.RandomBool() ? Poison.Lesser : Poison.Regular; } }
		public override int TreasureMapLevel{ get{ return 6; } }

		public Jormungandr( Serial serial ) : base( serial )
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

namespace Server.Items
{
	[Furniture]
	[Flipable( 0x4D0A, 0x4D0B )]
	public class StatueJormungandr : BaseStatue
	{
		public string AnimalKiller;
		public string AnimalWhere;

		[CommandProperty(AccessLevel.Owner)]
		public string Animal_Killer { get { return AnimalKiller; } set { AnimalKiller = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Animal_Where { get { return AnimalWhere; } set { AnimalWhere = value; InvalidateProperties(); } }

		[Constructable]
		public StatueJormungandr() : base( 0x4D0A )
		{
			Name = "Statue of Jormungandr";
			Weight = 60;
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			if ( AnimalWhere != "" && AnimalWhere != null ){ list.Add( 1070722, AnimalWhere ); }
			if ( AnimalKiller != "" && AnimalKiller != null ){ list.Add( 1049644, AnimalKiller ); }
        }

        public StatueJormungandr( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
            writer.Write( AnimalKiller );
            writer.Write( AnimalWhere );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
            AnimalKiller = reader.ReadString();
            AnimalWhere = reader.ReadString();
	    }
	}
}