using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a demon corpse" )]
	public class BlackGateDemon : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 150.0; } }
		public override double DispelFocus{ get{ return 25.0; } }

		private Point3D m_MoonDest;
		private int m_MoonTime;
		private InternalTimer m_MoonTimer;
		private int m_MoonHue;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public int MoonHue
		{
			get {return m_MoonHue;}
			set {m_MoonHue = value;}
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D MoonDest
		{
			get {return m_MoonDest;}
			set {m_MoonDest = value;}
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public int MoonTime
		{
			get {return m_MoonTime;}
			set {m_MoonTime = value;}
		}

		[Constructable]
		public BlackGateDemon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "devil" );
			Title = "the black gate demon";

			switch ( Utility.RandomMinMax( 0, 4 ) )
			{
				case 0: Title = "of the black gate"; break;
				case 1: Title = "of the void"; break;
				case 2: Title = "of the ethereal plane"; break;
				case 3: Title = "of the ethereal void"; break;
				case 4: Title = "of the dark portal"; break;
			}

			Body = 38;
			BaseSoundID = 357;

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

		public override bool DeleteCorpseOnDeath{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 18; } }
		public override HideType HideType{ get{ return HideType.Hellish; } }
		public override int Skin{ get{ return Utility.Random(6); } }
		public override SkinType SkinType{ get{ return SkinType.Demon; } }
		public override int Skeletal{ get{ return Utility.Random(8); } }
		public override SkeletalType SkeletalType{ get{ return SkeletalType.Devil; } }

		public BlackGateDemon( Serial serial ) : base( serial )
		{
		}

		public override bool OnBeforeDeath()
		{
			LootChest MyChest = new LootChest( Server.Misc.IntelligentAction.FameBasedLevel( this ) );
			MyChest.Name = "demonic chest";
			MyChest.Hue = 0x966;
			MyChest.MoveToWorld( Location, Map );

			int killerLuck = MobileUtilities.GetLuckFromKiller( this );
			PlayerMobile killer = MobileUtilities.TryGetKillingPlayer( this );

			if ( GetPlayerInfo.LuckyKiller( killerLuck ) && Utility.RandomMinMax( 1, 5 ) == 1 && !Server.Misc.PlayerSettings.GetSpecialsKilled( killer, "BlackGateDemon" ) )
			{
				Server.Misc.PlayerSettings.SetSpecialsKilled( killer, "BlackGateDemon", true );
				ManualOfItems book = new ManualOfItems();
					book.Hue = 0x497;
					book.Name = "Chest of Demonic Relics";
					book.m_Charges = 1;
					book.m_Skill_1 = 99;
					book.m_Skill_2 = 32;
					book.m_Skill_3 = 0;
					book.m_Skill_4 = 0;
					book.m_Skill_5 = 0;
					book.m_Value_1 = 10.0;
					book.m_Value_2 = 10.0;
					book.m_Value_3 = 0.0;
					book.m_Value_4 = 0.0;
					book.m_Value_5 = 0.0;
					book.m_Slayer_1 = 14;
					book.m_Slayer_2 = 0;
					book.m_Owner = killer;
					book.m_Extra = "of the Black Gate";
					book.m_FromWho = "Found within the Black Gate";
					book.m_HowGiven = "Acquired by";
					book.m_Points = 150;
					book.m_Hue = 0x497;
					MyChest.AddItem( book );
			}

			m_MoonTimer = new InternalTimer (this);
			m_MoonTimer.Start ();
			return base.OnBeforeDeath();
		}
		
		public override void OnAfterDelete()
		{
			m_MoonTimer = null;
			base.OnAfterDelete();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
			/*Moongate destination*/
			writer.Write((int)m_MoonDest.X);
			writer.Write((int)m_MoonDest.Y);
			writer.Write((int)m_MoonDest.Z);
			/*--------------------*/
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			/*Moongate destination*/
			int new_X = reader.ReadInt();
			int new_Y = reader.ReadInt();
			int new_Z = reader.ReadInt();
			m_MoonDest = new Point3D(5963, 3967, 10);
			/*--------------------*/			
		}
		
		private class InternalTimer : Timer
		{
			private Moongate m_MoonGate;
			
			public InternalTimer (BlackGateDemon owner) : base (TimeSpan.FromSeconds(0))
			{
				Delay = TimeSpan.FromSeconds(1800);
				Priority = TimerPriority.OneSecond;
				m_MoonGate = new Moongate ();
				m_MoonGate.MoveToWorld (new Point3D(owner.X, owner.Y, owner.Z), owner.Map);
				m_MoonGate.Target = new Point3D(5963, 3967, 10);
				m_MoonGate.TargetMap = owner.Map;
				m_MoonGate.ItemID = 0x1FD4;
			}
			
			protected override void OnTick ()
			{
				((Item)m_MoonGate).Delete ();
				Stop();
			}
		}
	}
}