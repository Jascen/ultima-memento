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
	[CorpseName( "a dragon corpse" )]
	public class YoungDragon : BaseCreature
	{
		[Constructable]
		public YoungDragon() : this( 888, 0 )
		{
		}

		[Constructable]
		public YoungDragon ( int body, int hue ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			rBody = body;
			Hue = hue; 
			BaseSoundID = 362;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 1 );
			AddLoot( LootPack.Gems, 2 );
		}

		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool HasBreath{ get{ return true; } }
		public override int Meat{ get{ return 4; } }
		public override int Hides{ get{ return 5; } }
		public override HideType HideType{ get{ return HideType.Draconic; } }
		public override int Scales{ get{ return 1; } }
		public override ScaleType ScaleType{ get{ return ResourceScales(); } }

		public YoungDragon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( rBody );
			writer.Write( rHue );
			writer.Write( rName );
			writer.Write( rCategory );
			writer.Write( rFood );
			writer.Write( rDwell );
			writer.Write( rPoison );
			writer.Write( rBlood );
			writer.Write( rBreath );
			writer.Write( rBreathPhysDmg );
			writer.Write( rBreathFireDmg );
			writer.Write( rBreathColdDmg );
			writer.Write( rBreathPoisDmg );
			writer.Write( rBreathEngyDmg );
			writer.Write( rBreathHue );
			writer.Write( rBreathSound );
			writer.Write( rBreathItemID );
			writer.Write( rBreathDelay );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			rBody = reader.ReadInt();
			rHue = reader.ReadInt();
			rName = reader.ReadString();
			rCategory = reader.ReadString();

			if ( version < 1 ){ string nu1 = reader.ReadString(); }

			rFood = reader.ReadString();

			if ( version < 1 ){ string nul = reader.ReadString(); }

			rDwell = reader.ReadString();
			rPoison = reader.ReadInt();
			rBlood = reader.ReadString();
			rBreath = reader.ReadInt();
			rBreathPhysDmg = reader.ReadInt();
			rBreathFireDmg = reader.ReadInt();
			rBreathColdDmg = reader.ReadInt();
			rBreathPoisDmg = reader.ReadInt();
			rBreathEngyDmg = reader.ReadInt();
			rBreathHue = reader.ReadInt();
			rBreathSound = reader.ReadInt();
			rBreathItemID = reader.ReadInt();
			rBreathDelay = reader.ReadDouble();
		}

		public override void OnAfterSpawn()
		{
			Region reg = Region.Find( this.Location, this.Map );
			string category = "land";

			if ( Server.Misc.Worlds.IsMainRegion( Server.Misc.Worlds.GetRegionName( this.Map, this.Location ) ) && Utility.RandomMinMax( 1, 10 ) == 1 )
			{
				switch ( Utility.Random( 5 ) )
				{
					case 0: category = "dungeon";	break;
					case 1: category = "fire";		break;
					case 2: category = "land";		break;
					case 3: category = "dungeon";	break;
					case 4: category = "fire";		break;
				}
			}
			else
			{
				if ( this.Map == Map.Lodor && this.Home.X == 5906 && this.Home.Y == 1207 ){ 	category = "snow"; }		// SPECIAL SPAWN - HARKYN CASTLE
				else if ( reg.IsPartOf( "Argentrock Castle" ) ){								category = "sky"; }
				else if ( reg.IsPartOf( "the Sanctum of Saltmarsh" ) ){							category = "swamp"; }
				else if ( reg.IsPartOf( "Dungeon Destard" ) )
				{
					if ( this.X >= 5319 && this.Y >= 892 && this.X <= 5361 && this.Y<= 922 ){ 		category = "sea"; }
					else if ( this.X >= 5131 && this.Y >= 956 && this.X <= 5152 && this.Y<= 976 ){ 	category = "fire"; }
					else if ( this.X >= 5217 && this.Y >= 901 && this.X <= 5241 && this.Y<= 927 ){ 	category = "sea"; }
					else { category = "dungeon"; }
				}
				else if ( reg.IsPartOf( "the Ancient Crash Site" ) || reg.IsPartOf( "the Ancient Sky Ship" ) ){ category = "radiation"; }
				else if ( Server.Misc.Worlds.IsFireDungeon( this.Location, this.Map ) ){		category = "fire"; }
				else if ( Server.Misc.Worlds.IsIceDungeon( this.Location, this.Map ) ){			category = "snow"; }
				else if ( Server.Misc.Worlds.IsSeaDungeon( this.Location, this.Map ) ){			category = "sea"; }
				else if ( Server.Misc.Worlds.TestTile ( this.Map, this.X, this.Y, "dirt" ) 
						&& Server.Misc.Worlds.TestMountain ( this.Map, this.X, this.Y, 15 ) ){	category = "mountain"; }
				else if ( Server.Misc.Worlds.TestMountain ( this.Map, this.X, this.Y, 10 ) ){ 	category = "mountain"; }
				else if ( Server.Misc.Worlds.TestOcean ( this.Map, this.X, this.Y, 15 ) ){ 		category = "sea"; }
				else if ( Server.Misc.Worlds.TestTile ( this.Map, this.X, this.Y, "snow" ) ){	category = "snow"; }
				else if ( Server.Misc.Worlds.TestTile ( this.Map, this.X, this.Y, "cave" ) ){	category = "dungeon"; if ( Utility.RandomBool() ){ category = "mountain"; } }
				else if ( Server.Misc.Worlds.TestTile ( this.Map, this.X, this.Y, "swamp" ) ){	category = "swamp"; }
				else if ( Server.Misc.Worlds.TestTile ( this.Map, this.X, this.Y, "jungle" ) ){	category = "jungle"; }
				else if ( Server.Misc.Worlds.TestTile ( this.Map, this.X, this.Y, "forest" ) ){	category = "forest"; }
				else if ( Server.Misc.Worlds.TestTile ( this.Map, this.X, this.Y, "sand" ) ){	category = "sand"; }
				else if ( Server.Misc.Worlds.TestMountain ( this.Map, this.X, this.Y, 15 ) ){ 	category = "mountain"; }
				else if ( reg.IsPartOf( typeof( DungeonRegion ) ) ){							category = "dungeon"; }
			}

			CreateDragon( category );

			base.OnAfterSpawn();
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if ( Utility.RandomMinMax( 1, 4 ) == 1 && !Controlled && rBlood != "" && rBlood != null && rBlood != "rust" && ( ( rBody == 59 && this.Fame > 15000 ) || ( rBody == 61 && this.Fame > 7500 ) ) )
			{
				int goo = 0;

				foreach ( Item splash in this.GetItemsInRange( 10 ) ){ if ( splash is MonsterSplatter ){ goo++; } }

				if ( goo == 0 )
				{
					if ( rBlood == "glowing goo" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "glowing goo", 0xB93, 1 ); }
					else if ( rBlood == "scorching ooze" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "scorching ooze", 0x496, 0 ); }
					else if ( rBlood == "poisonous slime" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "poisonous slime", 1167, 0 ); }
					else if ( rBlood == "toxic blood" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "toxic blood", 0x48E, 1 ); }
					else if ( rBlood == "toxic goo" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "toxic goo", 0xB93, 1 ); }
					else if ( rBlood == "hot magma" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "hot magma", 0x489, 1 ); }
					else if ( rBlood == "acidic slime" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "acidic slime", 1167, 0 ); }
					else if ( rBlood == "freezing water" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "freezing water", 296, 0 ); }
					else if ( rBlood == "scorching ooze" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "scorching ooze", 0x496, 0 ); }
					else if ( rBlood == "blue slime" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "blue slime", 0x5B6, 1 ); }
					else if ( rBlood == "green blood" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "green blood", 0x7D1, 0 ); }
					else if ( rBlood == "quick silver" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "quick silver", 0xB37, 1 ); }
					else { MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "thick blood", 0x485, 0 ); }
				}
			}
		}

		public override void OnGaveMeleeAttack( Mobile m )
		{
			base.OnGaveMeleeAttack( m );

			if ( 1 == Utility.RandomMinMax( 1, 20 ) && rBlood == "rust" && m is PlayerMobile )
			{
				Container cont = m.Backpack;
				Item iRuined = Server.Items.HiddenTrap.GetMyItem( m );

				if ( iRuined != null )
				{
					if ( Server.Items.HiddenTrap.IAmShielding( m, 100 ) || Server.Items.HiddenTrap.IAmAWeaponSlayer( m, this ) )
					{
					}
					else if ( m.CheckSkill( SkillName.MagicResist, 0, 100 ) )
					{
					}
					else if ( iRuined is BaseWeapon )
					{
						BaseWeapon iRusted = (BaseWeapon)iRuined;

						if ( CraftResources.GetType( iRuined.Resource ) == CraftResourceType.Metal )
						{
							if ( Server.Items.HiddenTrap.CheckInsuranceOnTrap( iRuined, m ) )
							{
								m.LocalOverheadMessage(MessageType.Emote, 1150, true, "The dragon almost rusted one of your protected items!");
							}
							else
							{
								m.LocalOverheadMessage(MessageType.Emote, 0x916, true, "The dragon rusted one of your equipped items!");
								RustyJunk broke = new RustyJunk();
								broke.ItemID = iRuined.GraphicID;
								broke.Name = "rusted item";
								broke.Weight = iRuined.Weight;
								m.AddToBackpack ( broke );
								iRuined.Delete();
							}
						}
					}
					if ( iRuined is BaseArmor )
					{
						BaseArmor iRusted = (BaseArmor)iRuined;

						if ( CraftResources.GetType( iRuined.Resource ) == CraftResourceType.Metal )
						{
							if ( Server.Items.HiddenTrap.CheckInsuranceOnTrap( iRuined, m ) )
							{
								m.LocalOverheadMessage(MessageType.Emote, 1150, true, "The dragon almost rusted one of your protected items!");
							}
							else
							{
								m.LocalOverheadMessage(MessageType.Emote, 0x916, true, "The dragon rusted one of your equipped items!");
								RustyJunk broke = new RustyJunk();
								broke.ItemID = iRuined.ItemID;
								broke.Name = "rusted item";
								broke.Weight = Utility.RandomMinMax( 1, 4 );
								m.AddToBackpack ( broke );
								iRuined.Delete();
							}
						}
					}
				}
			}
		}

		public int rBody;
		[CommandProperty(AccessLevel.Owner)]
		public int r_Body { get { return rBody; } set { rBody = value; InvalidateProperties(); } }

		public int rHue;
		[CommandProperty(AccessLevel.Owner)]
		public int r_Hue { get { return rHue; } set { rHue = value; InvalidateProperties(); } }

		public string rName;
		[CommandProperty(AccessLevel.Owner)]
		public string r_Name { get { return rName; } set { rName = value; InvalidateProperties(); } }

		public string rCategory;
		[CommandProperty(AccessLevel.Owner)]
		public string r_Category { get { return rCategory; } set { rCategory = value; InvalidateProperties(); } }

		public string rFood;
		[CommandProperty(AccessLevel.Owner)]
		public string r_Food { get { return rFood; } set { rFood = value; InvalidateProperties(); } }

		public string rDwell;
		[CommandProperty(AccessLevel.Owner)]
		public string r_Dwell { get { return rDwell; } set { rDwell = value; InvalidateProperties(); } }

		public int rPoison;
		[CommandProperty(AccessLevel.Owner)]
		public int r_Poison { get { return rPoison; } set { rPoison = value; InvalidateProperties(); } }

		public string rBlood;
		[CommandProperty(AccessLevel.Owner)]
		public string r_Blood { get { return rBlood; } set { rBlood = value; InvalidateProperties(); } }

		public int rBreath;
		[CommandProperty(AccessLevel.Owner)]
		public int r_Breath { get { return rBreath; } set { rBreath = value; InvalidateProperties(); } }

		public int rBreathPhysDmg;
		[CommandProperty(AccessLevel.Owner)]
		public int r_BreathPhysDmg { get { return rBreathPhysDmg; } set { rBreathPhysDmg = value; InvalidateProperties(); } }

		public int rBreathFireDmg;
		[CommandProperty(AccessLevel.Owner)]
		public int r_BreathFireDmg { get { return rBreathFireDmg; } set { rBreathFireDmg = value; InvalidateProperties(); } }

		public int rBreathColdDmg;
		[CommandProperty(AccessLevel.Owner)]
		public int r_BreathColdDmg { get { return rBreathColdDmg; } set { rBreathColdDmg = value; InvalidateProperties(); } }

		public int rBreathPoisDmg;
		[CommandProperty(AccessLevel.Owner)]
		public int r_BreathPoisDmg { get { return rBreathPoisDmg; } set { rBreathPoisDmg = value; InvalidateProperties(); } }

		public int rBreathEngyDmg;
		[CommandProperty(AccessLevel.Owner)]
		public int r_BreathEngyDmg { get { return rBreathEngyDmg; } set { rBreathEngyDmg = value; InvalidateProperties(); } }

		public int rBreathHue;
		[CommandProperty(AccessLevel.Owner)]
		public int r_BreathHue { get { return rBreathHue; } set { rBreathHue = value; InvalidateProperties(); } }

		public int rBreathSound;
		[CommandProperty(AccessLevel.Owner)]
		public int r_BreathSound { get { return rBreathSound; } set { rBreathSound = value; InvalidateProperties(); } }

		public int rBreathItemID;
		[CommandProperty(AccessLevel.Owner)]
		public int r_BreathItemID { get { return rBreathItemID; } set { rBreathItemID = value; InvalidateProperties(); } }

		public double rBreathDelay;
		[CommandProperty(AccessLevel.Owner)]
		public double r_BreathDelay { get { return rBreathDelay; } set { rBreathDelay = value; InvalidateProperties(); } }

		public override FoodType FavoriteFood
		{
			get
			{
				if ( rFood == "fish" )
					return ( FoodType.Fish );

				else if ( rFood == "gold" )
					return ( FoodType.Gold );

				else if ( rFood == "fire" )
					return ( FoodType.Fire );

				else if ( rFood == "gems" )
					return ( FoodType.Gems );

				else if ( rFood == "nox" )
					return ( FoodType.Nox );

				else if ( rFood == "sea" )
					return ( FoodType.Sea );

				else if ( rFood == "moon" )
					return ( FoodType.Moon );

				else if ( rFood == "fire_meat" )
					return FoodType.Fire | FoodType.Meat; 

				else if ( rFood == "fish_sea" )
					return FoodType.Fish | FoodType.Sea; 

				else if ( rFood == "gems_fire" )
					return FoodType.Gems | FoodType.Fire; 

				else if ( rFood == "gems_gold" )
					return FoodType.Gems | FoodType.Gold; 

				else if ( rFood == "gems_meat" )
					return FoodType.Gems | FoodType.Meat; 

				else if ( rFood == "gems_moon" )
					return FoodType.Gems | FoodType.Moon; 

				else if ( rFood == "meat_nox" )
					return FoodType.Meat | FoodType.Nox; 

				else if ( rFood == "moon_fire" )
					return FoodType.Moon | FoodType.Fire; 

				else if ( rFood == "nox_fire" )
					return FoodType.Nox | FoodType.Fire; 

				return ( FoodType.Meat );
			}
		}

		public override Poison PoisonImmune
		{
			get
			{
				return Poison.Lesser;
			}
		}

		public override Poison HitPoison
		{
			get
			{
				return Poison.Lesser;
			}
		}

		public override void BreathDealDamage( Mobile target, int form ){ base.BreathDealDamage( target, rBreath ); }

		public override int BreathPhysicalDamage{ get{ return rBreathPhysDmg; } }
		public override int BreathFireDamage{ get{ return rBreathFireDmg; } }
		public override int BreathColdDamage{ get{ return rBreathColdDmg; } }
		public override int BreathPoisonDamage{ get{ return rBreathPoisDmg; } }
		public override int BreathEnergyDamage{ get{ return rBreathEngyDmg; } }
		public override int BreathEffectHue{ get{ return rBreathHue; } }
		public override int BreathEffectSound{ get{ return rBreathSound; } }
		public override int BreathEffectItemID{ get{ return rBreathItemID; } }

		public void CreateDragon( string terrain )
		{
			if ( rHue < 1 )
			{
				bool bright = false;

				int dragon = Utility.RandomMinMax( 1, 145 ); // 146 IS OMITTED DUE TO XORMITE RARITY
				if ( terrain == "swamp" ){ dragon = Utility.RandomMinMax( 139, 145 ); }
				else if ( terrain == "fire" ){ dragon = Utility.RandomMinMax( 74, 87 ); }
				else if ( terrain == "snow" ){ dragon = Utility.RandomMinMax( 131, 138 ); }
				else if ( terrain == "sea" )
				{
					dragon = Utility.RandomMinMax( 120, 130 ); 
					if ( Utility.RandomMinMax( 1, 20 ) == 1 ){ dragon = 16; }
				}
				else if ( terrain == "radiation" ){ dragon = Utility.RandomList( 5, 6, 7, 54, 97, 104, 106, 146 ); }
				else if ( terrain == "jungle" ){ dragon = Utility.RandomList( 89, 90, 93, 95, 96 ); }
				else if ( terrain == "forest" ){ dragon = Utility.RandomMinMax( 88, 94 ); }
				else if ( terrain == "sand" ){ dragon = Utility.RandomMinMax( 112, 119 ); }
				else if ( terrain == "mountain" ){ dragon = Utility.RandomList( 109, 110, 111, 116 ); }
				else if ( terrain == "dungeon" ){ dragon = Utility.RandomMinMax( 1, 73 ); }
				else if ( terrain == "land" ){ dragon = Utility.RandomMinMax( 97, 108 ); }
				else if ( terrain == "sky" ){ dragon = Utility.RandomList( 7, 22, 33, 66, 97, 99, 101, 104, 105, 106, 107 ); }

				if ( Hue > 0 ){ dragon = Hue; }

				switch ( dragon )
				{
					case 1: rHue = 0x8E4; Resource = CraftResource.RedScales; rName = "a bloodstone dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = "thick blood"; break;
					case 2: rHue = 0xB2A; Resource = CraftResource.WhiteScales; rName = "a mercury dracon"; rDwell = "dungeon"; rFood = "gems_gold"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = "quick silver"; break;
					case 3: rHue = 0x916; Resource = CraftResource.RedScales; rName = "a scarlet dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = "toxic blood"; break;
					case 4: rHue = 0xB51; Resource = CraftResource.GreenScales; rName = "a poison dracon"; rDwell = "dungeon"; rFood = "nox"; rCategory = "poison"; if ( rBody == 59 ){ rBreath = 10; } else { rBreath = 18; } rPoison = 1; rBlood = "poisonous slime"; break;
					case 5: rHue = 0x82B; Resource = CraftResource.YellowScales; rName = "a glare dracon"; bright = true; rDwell = "dungeon"; rFood = "gold"; rCategory = "radiation"; if ( rBody == 59 ){ rBreath = 11; } else { rBreath = 39; } rPoison = 0; rBlood = "glowing goo"; break;
					case 6: rHue = 0x8D8; Resource = CraftResource.WhiteScales; rName = "a glaze dracon"; bright = true; rDwell = "dungeon"; rFood = "gold"; rCategory = "radiation"; if ( rBody == 59 ){ rBreath = 11; } else { rBreath = 39; } rPoison = 0; rBlood = "glowing goo"; break;
					case 7: rHue = 0x921; Resource = CraftResource.WhiteScales; rName = "a radiant dracon"; bright = true; rDwell = "dungeon"; rFood = "moon"; rCategory = "radiation"; if ( rBody == 59 ){ rBreath = 11; } else { rBreath = 39; } rPoison = 0; rBlood = "toxic goo"; break;
					case 8: rHue = 0x77C; Resource = CraftResource.RedScales; rName = "a blood dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "void"; if ( rBody == 59 ){ rBreath = 24; } else { rBreath = 27; } rPoison = 0; rBlood = "thick blood"; break;
					case 9: rHue = 0x871; Resource = CraftResource.BrazenScales; rName = "a rust dracon"; rDwell = "dungeon"; rFood = "gold"; rCategory = "poison"; if ( rBody == 59 ){ rBreath = 10; } else { rBreath = 18; } rPoison = 1; rBlood = "rust"; break;
					case 10: rHue = 0x996; Resource = CraftResource.BlueScales; rName = "a sapphire dracon"; rDwell = "dungeon"; rFood = "gems"; rCategory = "cold"; if ( rBody == 59 ){ rBreath = 12; } else { rBreath = 19; } rPoison = 0; rBlood = ""; break;
					case 11: rHue = 0xB56; Resource = CraftResource.BlueScales; rName = "an azurite dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "cold"; if ( rBody == 59 ){ rBreath = 12; } else { rBreath = 19; } rPoison = 0; rBlood = ""; break;
					case 12: rHue = 0x95B; Resource = CraftResource.YellowScales; rName = "a brass dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "electrical"; if ( rBody == 59 ){ rBreath = 13; } else { rBreath = 20; } rPoison = 0; rBlood = ""; break;
					case 13: rHue = 0x796; Resource = CraftResource.BlueScales; rName = "a cobolt dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "electrical"; if ( rBody == 59 ){ rBreath = 13; } else { rBreath = 20; } rPoison = 0; rBlood = ""; break;
					case 14: rHue = 0xB65; Resource = CraftResource.WhiteScales; rName = "a mithril dracon"; rDwell = "dungeon"; rFood = "gems_gold"; rCategory = "electrical"; if ( rBody == 59 ){ rBreath = 13; } else { rBreath = 20; } rPoison = 0; rBlood = ""; break;
					case 15: rHue = 0xB05; Resource = CraftResource.PlatinumScales; rName = "a palladium dracon"; rDwell = "dungeon"; rFood = "gems_gold"; rCategory = "electrical"; if ( rBody == 59 ){ rBreath = 13; } else { rBreath = 20; } rPoison = 0; rBlood = ""; break;
					case 16: rHue = 0xB3B; Resource = CraftResource.WhiteScales; rName = "a pearl dracon"; rDwell = "dungeon"; rFood = "fish_sea"; rCategory = "electrical"; if ( rBody == 59 ){ rBreath = 13; } else { rBreath = 20; } rPoison = 0; rBlood = ""; break;
					case 17: rHue = 0x99F; Resource = CraftResource.BlueScales; rName = "a steel dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "electrical"; if ( rBody == 59 ){ rBreath = 13; } else { rBreath = 20; } rPoison = 0; rBlood = ""; break;
					case 18: rHue = 0x98B; Resource = CraftResource.PlatinumScales; rName = "a titanium dracon"; rDwell = "dungeon"; rFood = "gems_gold"; rCategory = "electrical"; if ( rBody == 59 ){ rBreath = 13; } else { rBreath = 20; } rPoison = 0; rBlood = ""; break;
					case 19: rHue = 0xB7C; Resource = CraftResource.PlatinumScales; rName = "a turquoise dracon"; rDwell = "dungeon"; rFood = "gems"; rCategory = "electrical"; if ( rBody == 59 ){ rBreath = 13; } else { rBreath = 20; } rPoison = 0; rBlood = ""; break;
					case 20: rHue = 0x6F7; Resource = CraftResource.VioletScales; rName = "a violet dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "electrical"; if ( rBody == 59 ){ rBreath = 13; } else { rBreath = 20; } rPoison = 0; rBlood = ""; break;
					case 21: rHue = 0x7C3; Resource = CraftResource.VioletScales; rName = "an amethyst dracon"; rDwell = "dungeon"; rFood = "gems"; rCategory = "electrical"; if ( rBody == 59 ){ rBreath = 13; } else { rBreath = 20; } rPoison = 0; rBlood = ""; break;
					case 22: rHue = 0x7C6; Resource = CraftResource.YellowScales; rName = "a bright dracon"; bright = true; rDwell = "dungeon"; rFood = "fire"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 23: rHue = 0x92B; Resource = CraftResource.YellowScales; rName = "a bronze dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 24: rHue = 0x943; Resource = CraftResource.GreenScales; rName = "a cadmium dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 25: rHue = 0x8D0; Resource = CraftResource.BlueScales; rName = "a cerulean dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 26: rHue = 0x8B6; Resource = CraftResource.VioletScales; rName = "a darkscale dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 27: rHue = 0xB7E; Resource = CraftResource.WhiteScales; rName = "a diamond dracon"; rDwell = "dungeon"; rFood = "gems"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 28: rHue = 0xB1B; Resource = CraftResource.YellowScales; rName = "a gilded dracon"; rDwell = "dungeon"; rFood = "gold"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 29: rHue = 0x829; Resource = CraftResource.MetallicScales; rName = "a grey dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 30: rHue = 0xB94; Resource = CraftResource.GreenScales; rName = "a jade dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 31: rHue = 0x77E; Resource = CraftResource.GreenScales; rName = "a jadefire dracon"; bright = true; rDwell = "dungeon"; rFood = "fire_meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 32: rHue = 0x88B; Resource = CraftResource.BlackScales; rName = "a murky dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 33: rHue = 0x994; Resource = CraftResource.PlatinumScales; rName = "a platinum dracon"; rDwell = "dungeon"; rFood = "gems_moon"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 34: rHue = 0x6F5; Resource = CraftResource.VioletScales; rName = "a darklight dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 35: rHue = 0x869; Resource = CraftResource.YellowScales; rName = "a quartz dracon"; rDwell = "dungeon"; rFood = "gems"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 36: rHue = 0xB02; Resource = CraftResource.RedScales; rName = "a rosescale dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 37: rHue = 0x93E; Resource = CraftResource.RedScales; rName = "a ruby dracon"; rDwell = "dungeon"; rFood = "gems"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 38: rHue = 0x7CA; Resource = CraftResource.RedScales; rName = "a rubystar dracon"; bright = true; rDwell = "dungeon"; rFood = "gems_moon"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 39: rHue = 0x94D; Resource = CraftResource.VioletScales; rName = "a spinel dracon"; rDwell = "dungeon"; rFood = "gems"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 40: rHue = 0x883; Resource = CraftResource.BlueScales; rName = "a topaz dracon"; rDwell = "dungeon"; rFood = "gems"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 41: rHue = 0x95D; Resource = CraftResource.BlueScales; rName = "a valorite dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 42: rHue = 0x7CB; Resource = CraftResource.VioletScales; rName = "a velvet dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 43: rHue = 0x95E; Resource = CraftResource.GreenScales; rName = "a verite dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 44: rHue = 0xB5A; Resource = CraftResource.BlueScales; rName = "a zircon dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 45: rHue = 0x957; Resource = CraftResource.RedScales; rName = "an agapite dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 46: rHue = 0x7C7; Resource = CraftResource.GreenScales; rName = "an akira dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 47: rHue = 0x7CE; Resource = CraftResource.YellowScales; rName = "an amber dracon"; rDwell = "dungeon"; rFood = "gems"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 48: rHue = 0x944; Resource = CraftResource.BlueScales; rName = "an azure dracon"; rDwell = "dungeon"; rFood = "gems_gold"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 49: rHue = 0x8DD; Resource = CraftResource.BlackScales; rName = "an ebony dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 50: rHue = 0x8E3; Resource = CraftResource.VioletScales; rName = "an evil dracon"; rDwell = "dungeon"; rFood = "fire_meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 51: rHue = 0x942; Resource = CraftResource.WhiteScales; rName = "an iron dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 52: rHue = 0x943; Resource = CraftResource.GreenScales; rName = "a garnet dracon"; rDwell = "dungeon"; rFood = "nox"; rCategory = "poison"; if ( rBody == 59 ){ rBreath = 10; } else { rBreath = 18; } rPoison = 0; rBlood = ""; break;
					case 53: rHue = 0x950; Resource = CraftResource.GreenScales; rName = "an emerald dracon"; rDwell = "dungeon"; rFood = "nox"; rCategory = "poison"; if ( rBody == 59 ){ rBreath = 10; } else { rBreath = 18; } rPoison = 0; rBlood = ""; break;
					case 54: rHue = 0x702; Resource = CraftResource.RedScales; rName = "a redstar dracon"; bright = true; rDwell = "dungeon"; rFood = "moon_fire"; rCategory = "radiation"; if ( rBody == 59 ){ rBreath = 11; } else { rBreath = 39; } rPoison = 0; rBlood = ""; break;
					case 55: rHue = 0xB3B; Resource = CraftResource.WhiteScales; rName = "a marble dracon"; rDwell = "dungeon"; rFood = "gems"; rCategory = "steam"; if ( rBody == 59 ){ rBreath = 16; } else { rBreath = 38; } rPoison = 0; rBlood = ""; break;
					case 56: rHue = 0x708; Resource = CraftResource.RedScales; rName = "a vermillion dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "steam"; if ( rBody == 59 ){ rBreath = 16; } else { rBreath = 38; } rPoison = 0; rBlood = ""; break;
					case 57: rHue = 0x77A; Resource = CraftResource.RedScales; rName = "an ochre dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "steam"; if ( rBody == 59 ){ rBreath = 16; } else { rBreath = 38; } rPoison = 0; rBlood = ""; break;
					case 58: rHue = 0xB5E; Resource = CraftResource.BlackScales; rName = "an onyx dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "steam"; if ( rBody == 59 ){ rBreath = 16; } else { rBreath = 38; } rPoison = 0; rBlood = ""; break;
					case 59: rHue = 0x95B; Resource = CraftResource.YellowScales; rName = "an umber dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "steam"; if ( rBody == 59 ){ rBreath = 16; } else { rBreath = 38; } rPoison = 0; rBlood = ""; break;
					case 60: rHue = 0x6FB; Resource = CraftResource.VioletScales; rName = "a baneful dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "void"; if ( rBody == 59 ){ rBreath = 24; } else { rBreath = 27; } rPoison = 0; rBlood = ""; break;
					case 61: rHue = 0x870; Resource = CraftResource.RedScales; rName = "a bloodscale dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "void"; if ( rBody == 59 ){ rBreath = 24; } else { rBreath = 27; } rPoison = 0; rBlood = ""; break;
					case 62: rHue = 0xA9F; Resource = CraftResource.VioletScales; rName = "a corrupt dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "void"; if ( rBody == 59 ){ rBreath = 24; } else { rBreath = 27; } rPoison = 0; rBlood = ""; break;
					case 63: rHue = 0xBB0; Resource = CraftResource.BlackScales; rName = "a dark dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "void"; if ( rBody == 59 ){ rBreath = 25; } else { rBreath = 28; } rPoison = 0; rBlood = ""; break;
					case 64: rHue = 0x877; Resource = CraftResource.BlackScales; rName = "a dismal dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "void"; if ( rBody == 59 ){ rBreath = 25; } else { rBreath = 28; } rPoison = 0; rBlood = ""; break;
					case 65: rHue = 0x87E; Resource = CraftResource.VioletScales; rName = "a drowscale dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "void"; if ( rBody == 59 ){ rBreath = 24; } else { rBreath = 27; } rPoison = 0; rBlood = ""; break;
					case 66: rHue = 0x705; Resource = CraftResource.YellowScales; rName = "a gold dracon"; rDwell = "dungeon"; rFood = "gold"; rCategory = "void"; if ( rBody == 59 ){ rBreath = 23; } else { rBreath = 26; } rPoison = 0; rBlood = ""; break;
					case 67: rHue = 0x8B8; Resource = CraftResource.BlackScales; rName = "a grim dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "void"; if ( rBody == 59 ){ rBreath = 25; } else { rBreath = 28; } rPoison = 0; rBlood = ""; break;
					case 68: rHue = 0x6FD; Resource = CraftResource.VioletScales; rName = "a malicious dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "void"; if ( rBody == 59 ){ rBreath = 24; } else { rBreath = 27; } rPoison = 0; rBlood = ""; break;
					case 69: rHue = 0x86B; Resource = CraftResource.BlackScales; rName = "a shadowscale dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "void"; if ( rBody == 59 ){ rBreath = 25; } else { rBreath = 28; } rPoison = 0; rBlood = ""; break;
					case 70: rHue = 0x95C; Resource = CraftResource.BlackScales; rName = "a shadowy dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "void"; if ( rBody == 59 ){ rBreath = 25; } else { rBreath = 28; } rPoison = 0; rBlood = ""; break;
					case 71: rHue = 0x7CC; Resource = CraftResource.VioletScales; rName = "a vile dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "void"; if ( rBody == 59 ){ rBreath = 24; } else { rBreath = 27; } rPoison = 0; rBlood = ""; break;
					case 72: rHue = 0x6FE; Resource = CraftResource.VioletScales; rName = "a wicked dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "void"; if ( rBody == 59 ){ rBreath = 24; } else { rBreath = 27; } rPoison = 0; rBlood = ""; break;
					case 73: rHue = 0x6F9; Resource = CraftResource.UmberScales;rName = "an umbra dracon"; rDwell = "dungeon"; rFood = "meat"; rCategory = "void"; if ( rBody == 59 ){ rBreath = 24; } else { rBreath = 27; } rPoison = 0; rBlood = ""; break;
					case 74: rHue = 0x776; Resource = CraftResource.RedScales; rName = "a burnt dracon"; bright = true; rDwell = "fire"; rFood = "fire"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = "hot magma"; break;
					case 75: rHue = 0x86C; Resource = CraftResource.RedScales; rName = "a fire dracon"; bright = true; rDwell = "fire"; rFood = "fire"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = "hot magma"; break;
					case 76: rHue = 0x701; Resource = CraftResource.RedScales; rName = "a firelight dracon"; bright = true; rDwell = "fire"; rFood = "fire"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = "scorching ooze"; break;
					case 77: rHue = 0xB12; Resource = CraftResource.RedScales; rName = "a lava dracon"; bright = true; rDwell = "fire"; rFood = "fire"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = "hot magma"; break;
					case 78: rHue = 0xB38; Resource = CraftResource.BlackScales; rName = "a lavarock dracon"; rDwell = "fire"; rFood = "fire"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = "hot magma"; break;
					case 79: rHue = 0xB13; Resource = CraftResource.RedScales; rName = "a magma dracon"; bright = true; rDwell = "fire"; rFood = "fire"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = "hot magma"; break;
					case 80: rHue = 0x827; Resource = CraftResource.RedScales; rName = "a vulcan dracon"; bright = true; rDwell = "fire"; rFood = "fire"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = "hot magma"; break;
					case 81: rHue = 0xAB3; Resource = CraftResource.BlackScales; rName = "a charcoal dracon"; rDwell = "fire"; rFood = "fire"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 82: rHue = 0xAFA; Resource = CraftResource.RedScales; rName = "a cinder dracon"; bright = true; rDwell = "fire"; rFood = "fire"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 83: rHue = 0x93D; Resource = CraftResource.RedScales; rName = "a darkfire dracon"; bright = true; rDwell = "fire"; rFood = "fire"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 84: rHue = 0xB54; Resource = CraftResource.RedScales; rName = "a flare dracon"; bright = true; rDwell = "fire"; rFood = "fire"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 85: rHue = 0x775; Resource = CraftResource.RedScales; rName = "a hell dracon"; bright = true; rDwell = "fire"; rFood = "fire"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 86: rHue = 0x779; Resource = CraftResource.RedScales; rName = "a firerock dracon"; bright = true; rDwell = "fire"; rFood = "fire_meat"; rCategory = "steam"; if ( rBody == 59 ){ rBreath = 16; } else { rBreath = 38; } rPoison = 0; rBlood = ""; break;
					case 87: rHue = 0xB09; Resource = CraftResource.WhiteScales; rName = "a steam dracon"; rDwell = "fire"; rFood = "meat"; rCategory = "steam"; if ( rBody == 59 ){ rBreath = 16; } else { rBreath = 38; } rPoison = 0; rBlood = ""; break;
					case 88: rHue = 0x85D; Resource = CraftResource.GreenScales; rName = "a forest dracon"; rDwell = "forest"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 89: rHue = 0x6F6; Resource = CraftResource.GreenScales; rName = "a green dracon"; rDwell = "forest"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 90: rHue = 0xB28; Resource = CraftResource.GreenScales; rName = "a greenscale dracon"; rDwell = "forest"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 91: rHue = 0xB00; Resource = CraftResource.BlueScales; rName = "an evergreen dracon"; rDwell = "forest"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 92: rHue = 0xACC; Resource = CraftResource.GreenScales; rName = "a grove dracon"; rDwell = "forest"; rFood = "meat"; rCategory = "weed"; if ( rBody == 59 ){ rBreath = 34; } else { rBreath = 35; } rPoison = 0; rBlood = ""; break;
					case 93: rHue = 0x856; Resource = CraftResource.BlueScales; rName = "a moss dracon"; rDwell = "forest"; rFood = "meat"; rCategory = "weed"; if ( rBody == 59 ){ rBreath = 34; } else { rBreath = 35; } rPoison = 0; rBlood = ""; break;
					case 94: rHue = 0x91E; Resource = CraftResource.GreenScales; rName = "a woodland dracon"; rDwell = "forest"; rFood = "meat"; rCategory = "weed"; if ( rBody == 59 ){ rBreath = 34; } else { rBreath = 35; } rPoison = 0; rBlood = ""; break;
					case 95: rHue = 0x883; Resource = CraftResource.BlueScales; rName = "an amazon dracon"; rDwell = "jungle"; rFood = "meat_nox"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = "green blood"; break;
					case 96: rHue = 0xB44; Resource = CraftResource.GreenScales; rName = "a jungle dracon"; rDwell = "jungle"; rFood = "meat"; rCategory = "weed"; if ( rBody == 59 ){ rBreath = 34; } else { rBreath = 35; } rPoison = 0; rBlood = ""; break;
					case 97: rHue = 0x706; Resource = CraftResource.YellowScales; rName = "a nova dracon"; bright = true; rDwell = "land"; rFood = "moon"; rCategory = "radiation"; if ( rBody == 59 ){ rBreath = 11; } else { rBreath = 39; } rPoison = 0; rBlood = "glowing goo"; break;
					case 98: rHue = 0xAF7; Resource = CraftResource.RedScales; rName = "a crimson dracon"; rDwell = "land"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 99: rHue = 0x86A; Resource = CraftResource.VioletScales; rName = "a dusk dracon"; rDwell = "land"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 100: rHue = 0xB01; Resource = CraftResource.RedScales; rName = "a red dracon"; rDwell = "land"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 101: rHue = 0x6FC; Resource = CraftResource.BlueScales; rName = "a sky dracon"; rDwell = "land"; rFood = "meat"; rCategory = "wind"; if ( rBody == 59 ){ rBreath = 47; } else { rBreath = 48; } rPoison = 0; rBlood = ""; break;
					case 102: rHue = 0x95E; Resource = CraftResource.GreenScales; rName = "a spring dracon"; rDwell = "land"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 103: rHue = 0x703; Resource = CraftResource.VioletScales; rName = "an orchid dracon"; rDwell = "land"; rFood = "meat_nox"; rCategory = "poison"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 1; rBlood = ""; break;
					case 104: rHue = 0x981; Resource = CraftResource.RedScales; rName = "a solar dracon"; bright = true; rDwell = "land"; rFood = "moon_fire"; rCategory = "radiation"; if ( rBody == 59 ){ rBreath = 11; } else { rBreath = 39; } rPoison = 0; rBlood = ""; break;
					case 105: rHue = 0x6F8; Resource = CraftResource.WhiteScales; rName = "a star dracon"; bright = true; rDwell = "land"; rFood = "moon"; rCategory = "star"; if ( rBody == 59 ){ rBreath = 45; } else { rBreath = 49; } rPoison = 0; rBlood = ""; break;
					case 106: rHue = 0x869; Resource = CraftResource.YellowScales; rName = "a sun dracon"; bright = true; rDwell = "land"; rFood = "moon"; rCategory = "radiation"; if ( rBody == 59 ){ rBreath = 11; } else { rBreath = 39; } rPoison = 0; rBlood = ""; break;
					case 107: rHue = 0x95D; Resource = CraftResource.BlueScales; rName = "a moon dracon"; rDwell = "land"; rFood = "moon"; rCategory = "void"; if ( rBody == 59 ){ rBreath = 25; } else { rBreath = 28; } rPoison = 0; rBlood = ""; break;
					case 108: rHue = 0xB9D; Resource = CraftResource.BlackScales; rName = "a night dracon"; rDwell = "land"; rFood = "moon"; rCategory = "void"; if ( rBody == 59 ){ rBreath = 25; } else { rBreath = 28; } rPoison = 0; rBlood = ""; break;
					case 109: rHue = 0xB31; Resource = CraftResource.BlackScales; rName = "a mountain dracon"; rDwell = "mountain"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 110: rHue = 0x99B; Resource = CraftResource.WhiteScales; rName = "a rock dracon"; rDwell = "mountain"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 111: rHue = 0xB32; Resource = CraftResource.BlackScales; rName = "an obsidian dracon"; rDwell = "mountain"; rFood = "gems_fire"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 112: rHue = 0x855; Resource = CraftResource.BlueScales; rName = "a blue dracon"; rDwell = "sand"; rFood = "meat"; rCategory = "electrical"; if ( rBody == 59 ){ rBreath = 13; } else { rBreath = 20; } rPoison = 0; rBlood = ""; break;
					case 113: rHue = 0x959; Resource = CraftResource.RedScales; rName = "a copper dracon"; rDwell = "sand"; rFood = "meat"; rCategory = "electrical"; if ( rBody == 59 ){ rBreath = 13; } else { rBreath = 20; } rPoison = 0; rBlood = ""; break;
					case 114: rHue = 0x952; Resource = CraftResource.RedScales; rName = "a copperish dracon"; rDwell = "sand"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 115: rHue = 0x797; Resource = CraftResource.YellowScales; rName = "a yellow dracon"; rDwell = "sand"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 116: rHue = 0x957; Resource = CraftResource.YellowScales; rName = "an earth dracon"; rDwell = "sand"; rFood = "gems_meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 117: rHue = 0x713; Resource = CraftResource.YellowScales; rName = "a desert dracon"; rDwell = "sand"; rFood = "meat"; rCategory = "sand"; if ( rBody == 59 ){ rBreath = 8; } else { rBreath = 40; } rPoison = 0; rBlood = ""; break;
					case 118: rHue = 0x8BC; Resource = CraftResource.YellowScales; rName = "a dune dracon"; rDwell = "sand"; rFood = "meat"; rCategory = "sand"; if ( rBody == 59 ){ rBreath = 8; } else { rBreath = 40; } rPoison = 0; rBlood = ""; break;
					case 119: rHue = 0x712; Resource = CraftResource.YellowScales; rName = "a sand dracon"; rDwell = "sand"; rFood = "meat"; rCategory = "sand"; if ( rBody == 59 ){ rBreath = 8; } else { rBreath = 40; } rPoison = 0; rBlood = ""; break;
					case 120: rHue = 0x945; Resource = CraftResource.BlueScales; rName = "a nepturite dracon"; rDwell = "sea"; rFood = "fish_sea"; rCategory = "electrical"; if ( rBody == 59 ){ rBreath = 13; } else { rBreath = 20; } rPoison = 0; rBlood = ""; break;
					case 121: rHue = 0x8D1; Resource = CraftResource.BlueScales; rName = "a storm dracon"; rDwell = "sea"; rFood = "fish_sea"; rCategory = "storm"; if ( rBody == 59 ){ rBreath = 46; } else { rBreath = 50; } rPoison = 0; rBlood = ""; break;
					case 122: rHue = 0x8C2; Resource = CraftResource.BlueScales; rName = "a tide dracon"; rDwell = "sea"; rFood = "fish_sea"; rCategory = "electrical"; if ( rBody == 59 ){ rBreath = 13; } else { rBreath = 20; } rPoison = 0; rBlood = ""; break;
					case 123: rHue = 0xB07; Resource = CraftResource.BlueScales; rName = "a seastone dracon"; rDwell = "sea"; rFood = "fish_sea"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 124: rHue = 0x707; Resource = CraftResource.BlueScales; rName = "an aqua dracon"; rDwell = "sea"; rFood = "fish_sea"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 125: rHue = 0xB3D; Resource = CraftResource.BlueScales; rName = "a lagoon dracon"; rDwell = "sea"; rFood = "fish_sea"; rCategory = "poison"; if ( rBody == 59 ){ rBreath = 10; } else { rBreath = 18; } rPoison = 0; rBlood = ""; break;
					case 126: rHue = 0x7CD; Resource = CraftResource.BlueScales; rName = "a loch dracon"; rDwell = "sea"; rFood = "fish_sea"; rCategory = "poison"; if ( rBody == 59 ){ rBreath = 10; } else { rBreath = 18; } rPoison = 0; rBlood = ""; break;
					case 127: rHue = 0xAE9; Resource = CraftResource.GreenScales; rName = "an algae dracon"; rDwell = "sea"; rFood = "nox"; rCategory = "poison"; if ( rBody == 59 ){ rBreath = 10; } else { rBreath = 18; } rPoison = 1; rBlood = ""; break;
					case 128: rHue = 0x854; Resource = CraftResource.YellowScales; rName = "a coastal dracon"; rDwell = "sea"; rFood = "fish_sea"; rCategory = "sand"; if ( rBody == 59 ){ rBreath = 8; } else { rBreath = 40; } rPoison = 0; rBlood = ""; break;
					case 129: rHue = 0xB7F; Resource = CraftResource.RedScales; rName = "a coral dracon"; rDwell = "sea"; rFood = "fish_sea"; rCategory = "steam"; if ( rBody == 59 ){ rBreath = 16; } else { rBreath = 38; } rPoison = 0; rBlood = ""; break;
					case 130: rHue = 0xAFF; Resource = CraftResource.GreenScales; rName = "an ivy dracon"; rDwell = "sea"; rFood = "fish_sea"; rCategory = "weed"; if ( rBody == 59 ){ rBreath = 34; } else { rBreath = 35; } rPoison = 0; rBlood = ""; break;
					case 131: rHue = 0x860; Resource = CraftResource.PlatinumScales; rName = "a glacial dracon"; rDwell = "snow"; rFood = "fish"; rCategory = "cold"; if ( rBody == 59 ){ rBreath = 12; } else { rBreath = 19; } rPoison = 0; rBlood = "freezing water"; break;
					case 132: rHue = 0xAF3; Resource = CraftResource.WhiteScales; rName = "an ice dracon"; bright = true; rDwell = "snow"; rFood = "gems"; rCategory = "cold"; if ( rBody == 59 ){ rBreath = 12; } else { rBreath = 19; } rPoison = 0; rBlood = "freezing water"; break;
					case 133: rHue = 0xB7A; Resource = CraftResource.BlueScales; rName = "an icescale dracon"; bright = true; rDwell = "snow"; rFood = "gems"; rCategory = "cold"; if ( rBody == 59 ){ rBreath = 12; } else { rBreath = 19; } rPoison = 0; rBlood = "freezing water"; break;
					case 134: rHue = 0x9C4; Resource = CraftResource.WhiteScales; rName = "a silver dracon"; rDwell = "snow"; rFood = "meat"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = "quick silver"; break;
					case 135: rHue = 0x86D; Resource = CraftResource.PlatinumScales; rName = "a blizzard dracon"; rDwell = "snow"; rFood = "meat"; rCategory = "cold"; if ( rBody == 59 ){ rBreath = 12; } else { rBreath = 19; } rPoison = 0; rBlood = ""; break;
					case 136: rHue = 0x87D; Resource = CraftResource.WhiteScales; rName = "a frost dracon"; rDwell = "snow"; rFood = "meat"; rCategory = "cold"; if ( rBody == 59 ){ rBreath = 12; } else { rBreath = 19; } rPoison = 0; rBlood = ""; break;
					case 137: rHue = 0x8BA; Resource = CraftResource.WhiteScales; rName = "a snow dracon"; rDwell = "snow"; rFood = "meat"; rCategory = "cold"; if ( rBody == 59 ){ rBreath = 12; } else { rBreath = 19; } rPoison = 0; rBlood = ""; break;
					case 138: rHue = 0x911; Resource = CraftResource.WhiteScales; rName = "a white dracon"; rDwell = "snow"; rFood = "meat"; rCategory = "cold"; if ( rBody == 59 ){ rBreath = 12; } else { rBreath = 19; } rPoison = 0; rBlood = ""; break;
					case 139: rHue = 0xAB1; Resource = CraftResource.BlackScales; rName = "a black dracon"; rDwell = "swamp"; rFood = "meat_nox"; rCategory = "poison"; if ( rBody == 59 ){ rBreath = 10; } else { rBreath = 18; } rPoison = 0; rBlood = "acidic slime"; break;
					case 140: rHue = 0x88D; Resource = CraftResource.GreenScales; rName = "a mire dracon"; rDwell = "swamp"; rFood = "nox_fire"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 141: rHue = 0x945; Resource = CraftResource.BlueScales; rName = "a moor dracon"; rDwell = "swamp"; rFood = "nox_fire"; rCategory = "fire"; if ( rBody == 59 ){ rBreath = 9; } else { rBreath = 17; } rPoison = 0; rBlood = ""; break;
					case 142: rHue = 0x8B2; Resource = CraftResource.GreenScales; rName = "a bog dracon"; rDwell = "swamp"; rFood = "nox"; rCategory = "poison"; if ( rBody == 59 ){ rBreath = 10; } else { rBreath = 18; } rPoison = 1; rBlood = ""; break;
					case 143: rHue = 0xB27; Resource = CraftResource.GreenScales; rName = "a bogscale dracon"; rDwell = "swamp"; rFood = "nox"; rCategory = "poison"; if ( rBody == 59 ){ rBreath = 10; } else { rBreath = 18; } rPoison = 1; rBlood = ""; break;
					case 144: rHue = 0x77D; Resource = CraftResource.GreenScales; rName = "a swampfire dracon"; bright = true; rDwell = "swamp"; rFood = "meat_nox"; rCategory = "poison"; if ( rBody == 59 ){ rBreath = 10; } else { rBreath = 18; } rPoison = 0; rBlood = ""; break;
					case 145: rHue = 0x8EC; Resource = CraftResource.GreenScales; rName = "a marsh dracon"; rDwell = "swamp"; rFood = "meat"; rCategory = "weed"; if ( rBody == 59 ){ rBreath = 34; } else { rBreath = 35; } rPoison = 1; rBlood = ""; break;
					case 146: rHue = 0x7C7; Resource = CraftResource.GreenScales; rName = "a xormite dracon"; bright = true; rDwell = "dungeon"; rFood = "moon"; rCategory = "radiation"; if ( rBody == 59 ){ rBreath = 11; } else { rBreath = 39; } rPoison = 0; rBlood = ""; break;
				}

				if ( rCategory == "cold" ){ 			SetDamageType( ResistanceType.Physical, 50 );		SetDamageType( ResistanceType.Fire, 0 );		SetDamageType( ResistanceType.Cold, 50 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 0 ); }
				else if ( rCategory == "electrical" ){ 	SetDamageType( ResistanceType.Physical, 50 );		SetDamageType( ResistanceType.Fire, 0 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 50 ); }
				else if ( rCategory == "fire" ){ 		SetDamageType( ResistanceType.Physical, 50 );		SetDamageType( ResistanceType.Fire, 50 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 0 ); }
				else if ( rCategory == "poison" ){ 		SetDamageType( ResistanceType.Physical, 50 );		SetDamageType( ResistanceType.Fire, 0 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 50 );		SetDamageType( ResistanceType.Energy, 0 ); }
				else if ( rCategory == "radiation" ){ 	SetDamageType( ResistanceType.Physical, 20 );		SetDamageType( ResistanceType.Fire, 40 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 40 ); }
				else if ( rCategory == "sand" ){ 		SetDamageType( ResistanceType.Physical, 80 );		SetDamageType( ResistanceType.Fire, 20 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 0 ); }
				else if ( rCategory == "steam" ){ 		SetDamageType( ResistanceType.Physical, 40 );		SetDamageType( ResistanceType.Fire, 60 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 0 ); }
				else if ( rCategory == "void" ){ 		SetDamageType( ResistanceType.Physical, 20 );		SetDamageType( ResistanceType.Fire, 20 );		SetDamageType( ResistanceType.Cold, 20 );		SetDamageType( ResistanceType.Poison, 20 );		SetDamageType( ResistanceType.Energy, 20 ); }
				else if ( rCategory == "weed" ){ 		SetDamageType( ResistanceType.Physical, 80 );		SetDamageType( ResistanceType.Fire, 0 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 20 );		SetDamageType( ResistanceType.Energy, 0 ); }
				else if ( rCategory == "wind" ){ 		SetDamageType( ResistanceType.Physical, 100 );		SetDamageType( ResistanceType.Fire, 0 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 0 ); }
				else if ( rCategory == "storm" ){ 		SetDamageType( ResistanceType.Physical, 50 );		SetDamageType( ResistanceType.Fire, 0 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 50 ); }
				else if ( rCategory == "star" ){ 		SetDamageType( ResistanceType.Physical, 0 );		SetDamageType( ResistanceType.Fire, 50 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 50 ); }
				else { 									SetDamageType( ResistanceType.Physical, 100 );		SetDamageType( ResistanceType.Fire, 0 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 0 ); }

				int phys = 0;
				int fire = 0;
				int cold = 0;
				int pois = 0;
				int engy = 0;

				if ( rCategory == "cold" ){ 			phys = 50;	fire = 10; cold = 50; pois = 20; engy = 20; }
				else if ( rCategory == "electrical" ){ 	phys = 50;	fire = 20; cold = 20; pois = 10; engy = 50; }
				else if ( rCategory == "fire" ){ 		phys = 50;	fire = 50; cold = 10; pois = 20; engy = 20; }
				else if ( rCategory == "poison" ){ 		phys = 50;	fire = 20; cold = 20; pois = 50; engy = 10; }
				else if ( rCategory == "radiation" ){ 	phys = 50;	fire = 30; cold = 10; pois = 30; engy = 50; }
				else if ( rCategory == "sand" ){ 		phys = 40;	fire = 50; cold = 20; pois = 10; engy = 30; }
				else if ( rCategory == "steam" ){ 		phys = 50;	fire = 50; cold = 10; pois = 20; engy = 20; }
				else if ( rCategory == "void" ){ 		phys = 40;	fire = 40; cold = 40; pois = 40; engy = 40; }
				else if ( rCategory == "weed" ){ 		phys = 40;	fire = 15; cold = 15; pois = 50; engy = 40; }
				else if ( rCategory == "wind" ){ 		phys = 40;	fire = 10; cold = 20; pois = 10; engy = 50; }
				else if ( rCategory == "storm" ){ 		phys = 40;	fire = 50; cold = 20; pois = 10; engy = 50; }
				else if ( rCategory == "star" ){ 		phys = 40;	fire = 10; cold = 20; pois = 10; engy = 50; }
				else { 									phys = 50;	fire = 30; cold = 30; pois = 30; engy = 30; }

				Body = rBody;
				Name = rName;
				Hue = rHue;
				YellHue = dragon;

				if ( bright ){ AddItem( new LightSource() ); }

				SetStr( 251, 290 );
				SetDex( 63, 82 );
				SetInt( 61, 100 );

				SetHits( 241, 258 );

				SetDamage( 5, 8 );

				SetResistance( ResistanceType.Physical, (phys-10), phys );
				SetResistance( ResistanceType.Fire, (fire-10), fire );
				SetResistance( ResistanceType.Cold, (cold-10), cold );
				SetResistance( ResistanceType.Poison, (pois-10), pois );
				SetResistance( ResistanceType.Energy, (engy-10), engy );

				SetSkill( SkillName.MagicResist, 29.1, 40.0 );
				SetSkill( SkillName.Tactics, 57.6, 60.0 );
				SetSkill( SkillName.FistFighting, 50.1, 62.5 );
				if ( rPoison > 0 ){ SetSkill( SkillName.Poisoning, 20.1, 40.0 ); }

				Fame = 4000;
				Karma = -4000;

				VirtualArmor = 26;

				Tamable = true;
				ControlSlots = 1;
				MinTameSkill = 64.3;

				if ( rBreath == 10 || rBreath == 18 ){ 		rBreathPhysDmg = 0;		rBreathFireDmg = 0;		rBreathColdDmg = 0;		rBreathPoisDmg = 100;	rBreathEngyDmg = 0;		rBreathHue = 0x3F;	rBreathSound = 0x658;	rBreathItemID = 0x36D4;	rBreathDelay = 1.3; }
				else if ( rBreath == 11 || rBreath == 39 ){ rBreathPhysDmg = 0;		rBreathFireDmg = 0;		rBreathColdDmg = 0;		rBreathPoisDmg = 50;	rBreathEngyDmg = 50;	rBreathHue = 0x3F;	rBreathSound = 0x227;	rBreathItemID = 0x36D4;	rBreathDelay = 0.1; }
				else if ( rBreath == 24 || rBreath == 27 ){ rBreathPhysDmg = 20;	rBreathFireDmg = 20;	rBreathColdDmg = 20;	rBreathPoisDmg = 20;	rBreathEngyDmg = 20;	rBreathHue = 0x844;	rBreathSound = 0x658;	rBreathItemID = 0x37BC;	rBreathDelay = 0.1; }
				else if ( rBreath == 12 || rBreath == 19 ){ rBreathPhysDmg = 0;		rBreathFireDmg = 0;		rBreathColdDmg = 100;	rBreathPoisDmg = 0;		rBreathEngyDmg = 0;		rBreathHue = 0x481;	rBreathSound = 0x64F;	rBreathItemID = 0x36D4;	rBreathDelay = 1.3; }
				else if ( rBreath == 13 || rBreath == 20 ){ rBreathPhysDmg = 0;		rBreathFireDmg = 0;		rBreathColdDmg = 0;		rBreathPoisDmg = 0;		rBreathEngyDmg = 100;	rBreathHue = 0x9C2;	rBreathSound = 0x665;	rBreathItemID = 0x3818;	rBreathDelay = 1.3; }
				else if ( rBreath == 16 || rBreath == 38 ){ rBreathPhysDmg = 0;		rBreathFireDmg = 100;	rBreathColdDmg = 0;		rBreathPoisDmg = 0;		rBreathEngyDmg = 0;		rBreathHue = 0x9C4;	rBreathSound = 0x108;	rBreathItemID = 0x36D4;	rBreathDelay = 0.1; }
				else if ( rBreath == 25 || rBreath == 28 ){ rBreathPhysDmg = 20;	rBreathFireDmg = 20;	rBreathColdDmg = 20;	rBreathPoisDmg = 20;	rBreathEngyDmg = 20;	rBreathHue = 0x9C1;	rBreathSound = 0x653;	rBreathItemID = 0x37BC;	rBreathDelay = 0.1; }
				else if ( rBreath == 23 || rBreath == 26 ){ rBreathPhysDmg = 20;	rBreathFireDmg = 20;	rBreathColdDmg = 20;	rBreathPoisDmg = 20;	rBreathEngyDmg = 0;		rBreathHue = 0x496;	rBreathSound = 0x658;	rBreathItemID = 0x37BC;	rBreathDelay = 0.1; }
				else if ( rBreath == 34 || rBreath == 35 ){ rBreathPhysDmg = 50;	rBreathFireDmg = 0;		rBreathColdDmg = 0;		rBreathPoisDmg = 50;	rBreathEngyDmg = 0;		rBreathHue = 0;		rBreathSound = 0x56D;	rBreathItemID = Utility.RandomList( 0xCAC, 0xCAD );	rBreathDelay = 0.1; }
				else if ( rBreath == 8 || rBreath == 40 ){ 	rBreathPhysDmg = 50;	rBreathFireDmg = 50;	rBreathColdDmg = 0;		rBreathPoisDmg = 0;		rBreathEngyDmg = 0;		rBreathHue = 0x96D;	rBreathSound = 0x654;	rBreathItemID = 0x36D4;	rBreathDelay = 1.3; }
				else if ( rBreath == 45 || rBreath == 49 ){ rBreathPhysDmg = 0;		rBreathFireDmg = 50;	rBreathColdDmg = 0;		rBreathPoisDmg = 0;		rBreathEngyDmg = 50;	rBreathHue = 0xB72;	rBreathSound = 0x227;	rBreathItemID = 0x1A84;	rBreathDelay = 1.3; }
				else if ( rBreath == 47 || rBreath == 48 ){ rBreathPhysDmg = 100;	rBreathFireDmg = 0;		rBreathColdDmg = 0;		rBreathPoisDmg = 0;		rBreathEngyDmg = 0;		rBreathHue = 0xB24;	rBreathSound = 0x654;	rBreathItemID = 0x5590;	rBreathDelay = 1.3; }
				else if ( rBreath == 46 || rBreath == 50 ){ rBreathPhysDmg = 50;	rBreathFireDmg = 0;		rBreathColdDmg = 0;		rBreathPoisDmg = 0;		rBreathEngyDmg = 50;	rBreathHue = 0x9C2;	rBreathSound = 0x665;	rBreathItemID = 0x3818;	rBreathDelay = 1.3; }
				else { 										rBreathPhysDmg = 0;		rBreathFireDmg = 100;	rBreathColdDmg = 0;		rBreathPoisDmg = 0;		rBreathEngyDmg = 0;		rBreathHue = 0;		rBreathSound = 0x227;	rBreathItemID = 0x36D4;	rBreathDelay = 1.3; }

				InvalidateProperties();
			}
		}
	}
}