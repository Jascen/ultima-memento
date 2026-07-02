using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an adventurer corpse" )]
	public class Adventurers : BaseCreature
	{
		public int CitizenType;
		[CommandProperty(AccessLevel.Owner)]
		public int Citizen_Type { get { return CitizenType; } set { CitizenType = value; InvalidateProperties(); } }

		public int CitizenLevel;
		[CommandProperty(AccessLevel.Owner)]
		public int Citizen_Level { get { return CitizenLevel; } set { CitizenLevel = value; InvalidateProperties(); } }

		[Constructable]
		public Adventurers () : base( AIType.AI_Melee, FightMode.Evil, 10, 1, 0.2, 0.4 )
		{
			if ( Female = Utility.RandomBool() ) 
			{ 
				Body = 401; 
				Name = NameList.RandomName( "female" );
			}
			else 
			{ 
				Body = 400; 			
				Name = NameList.RandomName( "male" ); 
				FacialHairItemID = Utility.RandomList( 0, 0, 8254, 8255, 8256, 8257, 8267, 8268, 8269 );
			}

			SetStr( 300 );
			SetDex( 300 );
			SetInt( 300 );

			switch ( Utility.Random( 3 ) )
			{
				case 0: Server.Misc.IntelligentAction.DressUpWizards( this, false ); 					CitizenType = 1;	break;
				case 1: Server.Misc.IntelligentAction.DressUpFighters( this, "", false, false, true );	CitizenType = 2;	break;
				case 2: Server.Misc.IntelligentAction.DressUpRogues( this, "", false, false, true );	CitizenType = 3;	break;
			}

			Title = TavernPatrons.GetTitle();
			Hue = Utility.RandomSkinColor();
			Utility.AssignRandomHair( this );
			SpeechHue = Utility.RandomTalkHue();
			AI = AIType.AI_Citizen;
			HairHue = Utility.RandomHairHue();
			FacialHairHue = HairHue;
			CitizenLevel = Utility.RandomMinMax( 1, 9 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, (CitizenLevel*4), (CitizenLevel*7) );
			SetResistance( ResistanceType.Fire, (CitizenLevel*4), (CitizenLevel*7) );
			SetResistance( ResistanceType.Cold, (CitizenLevel*4), (CitizenLevel*7) );
			SetResistance( ResistanceType.Poison, (CitizenLevel*4), (CitizenLevel*7) );
			SetResistance( ResistanceType.Energy, (CitizenLevel*4), (CitizenLevel*7) );

			if ( CitizenType == 1 )
			{
				AI = AIType.AI_Mage;
				SetStr( (CitizenLevel*50), (CitizenLevel*70) );
				SetDex( (CitizenLevel*70), (CitizenLevel*90) );
				SetInt( (CitizenLevel*100), (CitizenLevel*130) );

				SetHits( (CitizenLevel*100), (CitizenLevel*130) );

				SetSkill( SkillName.Psychology, (28+(CitizenLevel*7)) );
				SetSkill( SkillName.Magery, (28+(CitizenLevel*7)) );
				SetSkill( SkillName.Meditation, (28+(CitizenLevel*7)) );
				SetSkill( SkillName.MagicResist, (28+(CitizenLevel*7)) );
				SetSkill( SkillName.Tactics, (28+(CitizenLevel*7)) );
				SetSkill( SkillName.FistFighting, (28+(CitizenLevel*7)) );
				SetSkill( SkillName.Marksmanship, (28+(CitizenLevel*7)) );

				AddRangeWeapon();
			}
			else if ( CitizenType == 2 )
			{
				AI = AIType.AI_Melee;
				SetStr( (CitizenLevel*100), (CitizenLevel*130) );
				SetDex( (CitizenLevel*70), (CitizenLevel*90) );
				SetInt( (CitizenLevel*50), (CitizenLevel*70) );

				SetHits( (CitizenLevel*100), (CitizenLevel*130) );

				SetSkill( SkillName.Fencing, (28+(CitizenLevel*7)) );
				SetSkill( SkillName.Bludgeoning, (28+(CitizenLevel*7)) );
				SetSkill( SkillName.Swords, (28+(CitizenLevel*7)) );
				SetSkill( SkillName.MagicResist, (28+(CitizenLevel*7)) );
				SetSkill( SkillName.Tactics, (28+(CitizenLevel*7)) );
				SetSkill( SkillName.FistFighting, (28+(CitizenLevel*7)) );
				SetSkill( SkillName.Marksmanship, (28+(CitizenLevel*7)) );
				SetSkill( SkillName.Parry, (28+(CitizenLevel*7)) );
			}
			else
			{
				AI = AIType.AI_Archer;
				SetStr( (CitizenLevel*70), (CitizenLevel*90) );
				SetDex( (CitizenLevel*100), (CitizenLevel*130) );
				SetInt( (CitizenLevel*50), (CitizenLevel*70) );

				SetHits( (CitizenLevel*100), (CitizenLevel*130) );

				SetSkill( SkillName.MagicResist, (28+(CitizenLevel*7)) );
				SetSkill( SkillName.Tactics, (28+(CitizenLevel*7)) );
				SetSkill( SkillName.FistFighting, (28+(CitizenLevel*7)) );
				SetSkill( SkillName.Marksmanship, (28+(CitizenLevel*7)) );

				AddRangeWeapon();
			}

			SetDamage( (CitizenLevel*2), (CitizenLevel*3) );

			Fame = 2500 * CitizenLevel;
			Karma = Fame;

			VirtualArmor = CitizenLevel * 10;
		}

		public override void GenerateLoot()
		{
			if ( CitizenLevel > 8 ){ AddLoot( LootPack.FilthyRich ); }
			if ( CitizenLevel > 6 ){ AddLoot( LootPack.FilthyRich ); }
			if ( CitizenLevel > 4 ){ AddLoot( LootPack.Rich ); }
			if ( CitizenLevel > 2 ){ AddLoot( LootPack.Average ); }
			AddLoot( LootPack.Meager );

			if ( CitizenType == 1 ){ AddLoot( LootPack.MedScrolls, ( (int)( ( CitizenLevel / 3 ) + 1 ) ) ); }
		}

		public void AddRangeWeapon()
		{
			if ( FindItemOnLayer( Layer.OneHanded ) != null ) { FindItemOnLayer( Layer.OneHanded ).Delete(); }
			if ( FindItemOnLayer( Layer.TwoHanded ) != null ) { FindItemOnLayer( Layer.TwoHanded ).Delete(); }

			if ( Utility.RandomBool() )
			{
				ThrowingGloves glove = new ThrowingGloves();
				switch ( Utility.Random( 5 ))		   
				{
					case 0: glove.GloveType = ThrowingWeaponType.Stones; break;
					case 1: glove.GloveType = ThrowingWeaponType.Axes; break;
					case 2: glove.GloveType = ThrowingWeaponType.Daggers; break;
					case 3: glove.GloveType = ThrowingWeaponType.Darts; break;
					case 4: glove.GloveType = ThrowingWeaponType.Stars; break;
				};

				ThrowingWeapon ammo = new ThrowingWeapon( Utility.RandomMinMax( 15, 30 ) )
				{
					Ammo = glove.GloveType
				};

				AddItem( glove );
				PackItem( ammo );
			}
			else if ( CitizenType == 1 )
			{
				switch ( Utility.Random( 2 ))		   
				{
					case 0: AddItem( new WizardStaff() );		break;
					case 1: AddItem( new WizardStick() );		break;
				};

				PackItem( new MageEye( Utility.RandomMinMax( 15, 30 ) ) );
			}
			else
			{
				switch ( Utility.Random( 8 ))		   
				{
					case 0: AddItem( new Bow() );					PackItem( new Arrow( Utility.RandomMinMax( 15, 30 ) ) );		break;
					case 1: AddItem( new Crossbow() );				PackItem( new Bolt( Utility.RandomMinMax( 15, 30 ) ) );			break;
					case 2: AddItem( new HeavyCrossbow() );			PackItem( new Bolt( Utility.RandomMinMax( 15, 30 ) ) );			break;
					case 3: AddItem( new RepeatingCrossbow() );		PackItem( new Bolt( Utility.RandomMinMax( 15, 30 ) ) );			break;
					case 4: AddItem( new CompositeBow() );			PackItem( new Arrow( Utility.RandomMinMax( 15, 30 ) ) );		break;
					case 5: AddItem( new MagicalShortbow() );		PackItem( new Arrow( Utility.RandomMinMax( 15, 30 ) ) );		break;
					case 6: AddItem( new ElvenCompositeLongbow() );	PackItem( new Arrow( Utility.RandomMinMax( 15, 30 ) ) );		break;
					case 7: AddItem( new Harpoon() );				PackItem( new HarpoonRope( Utility.RandomMinMax( 15, 30 ) ) );	break;
				};
			}
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool ClickTitle{ get{ return false; } }
		public override bool ShowFameTitle{ get{ return false; } }
		public override bool AlwaysAttackable{ get{ return true; } }
		public override int Meat{ get{ return 1; } }
		public override int TreasureMapLevel{ get{ return (int)((CitizenLevel/2)+1); } }
		public override int Skeletal{ get{ return Utility.Random(3); } }
		public override SkeletalType SkeletalType{ get{ return SkeletalType.Brittle; } }

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );
			Server.Misc.IntelligentAction.CryOut( this );
		}

		public Adventurers( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
			writer.Write( CitizenType );
			writer.Write( CitizenLevel );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			CitizenType = reader.ReadInt();
			CitizenLevel = reader.ReadInt();
		}
	}
}