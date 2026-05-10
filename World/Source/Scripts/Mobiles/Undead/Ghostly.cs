using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a ghostly corpse" )]
	public class Ghostly : BaseCreature
	{
		[Constructable]
		public Ghostly() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = Server.Misc.RandomThings.GetRandomWizardName();
			Title = "the ghost";
			Body = 0x3CA;
			Hue = 1150;
			BaseSoundID = 0x482;
			int bump = 0;

			switch( Utility.RandomMinMax( 0, 24 ) )
			{
				case 0: Title = "the ruby ghost"; Hue = 0x5B5; bump = 1; break;
				case 1: Title = "the azure ghost"; Hue = 0x5B6; bump = 2; break;
				case 2: Title = "the emerald ghost"; Hue = 0xB93; bump = 3; break;
				case 3: Title = "the golden ghost"; Hue = 0x8A5; bump = 4; break;
				case 4: Title = "the dark ghost"; Hue = 1175; bump = 5; break;
			}

			SetStr( 171, 200 );
			SetDex( 126, 145 );
			SetInt( 276, 305 );

			SetHits( 103, 120 );

			SetDamage( 24, 26 );

			SetDamageType( ResistanceType.Physical, 10 );
			SetDamageType( ResistanceType.Cold, 40 );
			SetDamageType( ResistanceType.Energy, 50 );

			SetResistance( ResistanceType.Physical, 40, 60 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 55, 65 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.Necromancy, 89, 99.1 );
			SetSkill( SkillName.Spiritualism, 90.0, 99.0 );
			SetSkill( SkillName.Psychology, 100.0 );
			SetSkill( SkillName.Magery, 70.1, 80.0 );
			SetSkill( SkillName.Meditation, 85.1, 95.0 );
			SetSkill( SkillName.MagicResist, 80.1, 100.0 );
			SetSkill( SkillName.Tactics, 70.1, 90.0 );

			Fame = 8000;
			Karma = -8000;

			VirtualArmor = 50;
			PackItem( new GraveDust( 10 ) );
			PackReg( 17, 24 );

			AddItem( new LightSource() );

			BeefUp( (BaseCreature)this, bump );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.MedScrolls, 1 );
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			int killerLuck = MobileUtilities.GetLuckFromKiller( this );

			if ( Server.Misc.GetPlayerInfo.LuckyKiller( killerLuck ) )
			{
				int Magic = 0;
				int Speed = 0;
				if ( this.Hue == 0x5B5 ){ Magic = 5; Speed = 0; }
				else if ( this.Hue == 0x5B6 ){ Magic = 10; Speed = 1; }
				else if ( this.Hue == 0xB93 ){ Magic = 15; Speed = 1; }
				else if ( this.Hue == 0x8A5 ){ Magic = 20; Speed = 2; }
				else if ( this.Hue == 1175 ){ Magic = 25; Speed = 2; }

				Robe robe = new Robe();
					robe.Name = "shroud of " + this.Title;
					robe.Hue = this.Hue;
					robe.Attributes.CastRecovery = Speed;
					robe.Attributes.CastSpeed = Speed;
					robe.Attributes.LowerRegCost = 5 + Magic;
					robe.Attributes.SpellDamage = 5 + Magic;
					c.DropItem( robe );
			}
		}

		public override bool OnBeforeDeath()
		{
			this.Body = 13;
			return base.OnBeforeDeath();
		}

		public override bool ShowFameTitle{ get{ return false; } }
		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override int TreasureMapLevel{ get{ return 3; } }
		public override bool AlwaysAttackable{ get{ return true; } }
		public override int Cloths{ get{ return 10; } }
		public override ClothType ClothType{ get{ return ClothType.Haunted; } }

		public Ghostly( Serial serial ) : base( serial )
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