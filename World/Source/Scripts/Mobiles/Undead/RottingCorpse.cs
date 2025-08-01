using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a rotting corpse" )]
	public class RottingCorpse : BaseCreature
	{
		[Constructable]
		public RottingCorpse() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "male" );
			switch( Utility.RandomMinMax( 1, 4 ) )
			{
				case 1: Name = NameList.RandomName( "female" );	break;
				case 2: Name = NameList.RandomName( "male" );	break;
				case 3: Name = NameList.RandomName( "elf_female" );	break;
				case 4: Name = NameList.RandomName( "elf_male" );	break;
			}

			Title = "the ancient zombie";
			switch( Utility.RandomMinMax( 0, 4 ) )
			{
				case 0: Title = "the ancient zombie";	break;
				case 1: Title = "the zombie lord";		break;
				case 2: Title = "the greater zombie";	break;
				case 3: Title = "the ancient corpse";	break;
				case 4: Title = "the greater corpse";	break;
			}

			Body = 305;
			BaseSoundID = 471;

			SetStr( 301, 350 );
			SetDex( 75 );
			SetInt( 151, 200 );

			SetHits( 1200 );
			SetStam( 150 );
			SetMana( 0 );

			SetDamage( 8, 10 );

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Cold, 50 );
			SetDamageType( ResistanceType.Poison, 50 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 50, 70 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.Poisoning, 120.0 );
			SetSkill( SkillName.MagicResist, 250.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.FistFighting, 90.1, 100.0 );

			Fame = 6000;
			Karma = -6000;

			VirtualArmor = 40;

			int[] list = new int[]
				{
					0x1CF0, 0x1CEF, 0x1CEE, 0x1CED, 0x1CE9, 0x1DA0, 0x1DAE, // pieces
					0x1CEC, 0x1CE5, 0x1CE2, 0x1CDD, 0x1AE4, 0x1DA1, 0x1DA2, 0x1DA4, 0x1DAF, 0x1DB0, 0x1DB1, 0x1DB2, // limbs
					0x1CE8, 0x1CE0, 0x1D9F, 0x1DAD // torsos
				};

			PackItem( new BodyPart( Utility.RandomList( list ) ) );
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			int killerLuck = MobileUtilities.GetLuckFromKiller( this );
			if ( GetPlayerInfo.LuckyKiller( killerLuck ) && Server.Misc.IntelligentAction.FameBasedEvent( this ) )
			{
				LootChest MyChest = new LootChest( Server.Misc.IntelligentAction.FameBasedLevel( this ) );
				Server.Misc.ContainerFunctions.MakeTomb( MyChest, this, 0 );
				c.DropItem( MyChest );
			}
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int Hides{ get{ return 3; } }
		public override HideType HideType{ get{ return HideType.Necrotic; } }
		public override int Skin{ get{ return Utility.Random(3); } }
		public override SkinType SkinType{ get{ return SkinType.Dead; } }

		public RottingCorpse( Serial serial ) : base( serial )
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