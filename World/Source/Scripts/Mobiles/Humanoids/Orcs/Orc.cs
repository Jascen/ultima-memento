using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an orc corpse" )]
	public class Orc : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }

		[Constructable]
		public Orc() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "orc" );
			Title = "the orc";
			Body = Utility.RandomList( 17, 474 );
			BaseSoundID = 0x45A;

			SetStr( 96, 120 );
			SetDex( 81, 105 );
			SetInt( 36, 60 );

			SetHits( 58, 72 );

			SetDamage( 5, 7 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 25, 30 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 10, 20 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.MagicResist, 50.1, 75.0 );
			SetSkill( SkillName.Tactics, 55.1, 80.0 );
			SetSkill( SkillName.FistFighting, 50.1, 70.0 );
			SetSkill( SkillName.Marksmanship, 50.1, 70.0 );

			Fame = 1500;
			Karma = -1500;

			VirtualArmor = 28;

			if ( Utility.RandomBool() )
			{
				AI = AIType.AI_Archer;
				SetDex( 96, 120 );
				MonsterGloves gloves = new MonsterGloves();
				gloves.ThrowType = "Stones";
				AddItem( gloves );

				ThrowingWeapon toss = new ThrowingWeapon( Utility.RandomMinMax( 10, 30 ) );
				toss.Ammo = ThrowingWeaponType.Stones;
				PackItem( toss );
			}
			else
			{
				switch ( Utility.Random( 20 ) )
				{
					case 0: PackItem( new Scimitar() ); break;
					case 1: PackItem( new Katana() ); break;
					case 2: PackItem( new WarMace() ); break;
					case 3: PackItem( new WarHammer() ); break;
					case 4: PackItem( new Kryss() ); break;
					case 5: PackItem( new Pitchfork() ); break;
				}
			}

			PackItem( new ThighBoots() );

			switch ( Utility.Random( 3 ) )
			{
				case 0: PackItem( new Ribs() ); break;
				case 1: PackItem( new Shaft() ); break;
				case 2: PackItem( new Candle() ); break;
			}
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 1; } }
		public override int Meat{ get{ return 1; } }
		public override int Skeletal{ get{ return Utility.Random(2); } }
		public override SkeletalType SkeletalType{ get{ return SkeletalType.Orc; } }

		public Orc( Serial serial ) : base( serial )
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
