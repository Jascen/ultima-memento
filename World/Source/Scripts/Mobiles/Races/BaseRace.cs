using Server;
using System;
using Server.Mobiles;
using Server.Misc;
using System.Collections;

namespace Server.Items
{
	public class BaseRace : Item
	{
		// Race IDs above this threshold are monster-race button/encoding IDs;
		// subtract this base to get the index into the monster body lookup.
		public const int MonsterRaceIDBase = 80000;

		[Constructable]
		public BaseRace() : base( 0x4047 )
		{
			m_AosAttributes = new AosAttributes( this );
			m_AosResistances = new AosElementAttributes( this );
			m_AosSkillBonuses = new AosSkillBonuses( this );

			Layer = Layer.Special;
			LootType = LootType.Blessed;
			Movable = false;
			Weight = 0;
			SpeciesLevel = 1;
		}

		public static BaseRace GetCostume( int id )
		{
			BaseRace race = new BaseRace();

			if ( id > 80000 ){ id = GetBody( (id-80000) ); }

			ConfigureCostume( id, race );

			return race;
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);

			if ( NoFood( SpeciesID ) ){ list.Add( 1070722, "Does not need to eat"); }
			else if ( NoFoodOrDrink( SpeciesID ) ){ list.Add( 1070722, "Does not need to eat or drink"); }
			else if ( BloodDrinker( SpeciesID ) ){ list.Add( 1070722, "Needs to consume fresh blood"); }
			else if ( BrainEater( SpeciesID ) ){ list.Add( 1070722, "Needs to consume fresh brains"); }
        }

		public static void ConfigureCostume( int race, BaseRace costume )
		{
			int id = GetID( race );

			string configs = RaceDefined( id );

			if ( configs.Length > 0 )
			{
				string[] setups = configs.Split(',');
				int entry = 1;
				foreach (string stats in setups)
				{
					if ( entry == 1 ){ costume.Name = stats; }
					else if ( entry == 2 ){ costume.SpeciesIndex = Int32.Parse(stats); }
					else if ( entry == 3 ){ costume.ItemID = Int32.Parse(stats); }
					else if ( entry == 4 ){ costume.SpeciesGump = Int32.Parse(stats); }
					else if ( entry == 5 ){ costume.SpeciesID = Int32.Parse(stats); }
					else if ( entry == 6 ){ costume.SpeciesIcon = Int32.Parse(stats); }
					else if ( entry == 7 ){ costume.SpeciesWide = Int32.Parse(stats); }
					else if ( entry == 8 ){ costume.SpeciesHigh = Int32.Parse(stats); }
					else if ( entry == 9 )
					{
						int sound = Int32.Parse(stats)+1;

						if ( sound == 10000 ) // mushrooms
						{
							costume.SpeciesAngerSound = 0x451-1;
							costume.SpeciesIdleSound = 0x452-1;
							costume.SpeciesDeathSound = 0x455-1;
							costume.SpeciesAttackSound = 0x453-1;
							costume.SpeciesHurtSound = 0x454-1;
						}
						else
						{
							costume.SpeciesAngerSound = sound-1;
							costume.SpeciesIdleSound = sound;
							costume.SpeciesDeathSound = sound+3;
							costume.SpeciesAttackSound = sound+1;
							costume.SpeciesHurtSound = sound+2;
						}
					}
					else if ( entry == 10 ){ costume.SpeciesFamily = stats; }
					else if ( entry == 11 ){ costume.SpeciesAlignment = stats; }
					else if ( entry == 12 ){ costume.SpeciesStart = stats; }
					else if ( entry == 13 ){ costume.SpeciesSize = Int32.Parse(stats); }
					else if ( entry == 14 ){ costume.Resistances.Physical = Int32.Parse(stats)*5; }
					else if ( entry == 15 ){ costume.Resistances.Fire = Int32.Parse(stats)*5; }
					else if ( entry == 16 ){ costume.Resistances.Cold = Int32.Parse(stats)*5; }
					else if ( entry == 17 ){ costume.Resistances.Poison = Int32.Parse(stats)*5; }
					else if ( entry == 18 ){ costume.Resistances.Energy = Int32.Parse(stats)*5; }
					else if ( entry == 19 ){ costume.Attributes.BonusStr = Int32.Parse(stats)*5; }
					else if ( entry == 20 ){ costume.Attributes.BonusDex = Int32.Parse(stats)*5; }
					else if ( entry == 21 ){ costume.Attributes.BonusInt = Int32.Parse(stats)*5; }
					else if ( entry == 22 ){ costume.Attributes.BonusHits = Int32.Parse(stats)*5; }
					else if ( entry == 23 ){ costume.Attributes.BonusStam = Int32.Parse(stats)*5; }
					else if ( entry == 24 ){ costume.Attributes.BonusMana = Int32.Parse(stats)*5; }
					else if ( entry == 25 ){ costume.Attributes.RegenHits = Int32.Parse(stats); }
					else if ( entry == 26 ){ costume.Attributes.RegenStam = Int32.Parse(stats); }
					else if ( entry == 27 ){ costume.Attributes.RegenMana = Int32.Parse(stats); }
					else if ( entry == 28 ){ costume.Attributes.NightSight = Int32.Parse(stats); }
					else if ( entry == 29 ){ costume.Attributes.AttackChance = Int32.Parse(stats)*5; }
					else if ( entry == 30 ){ costume.Attributes.DefendChance = Int32.Parse(stats)*5; }
					else if ( entry == 31 ){ costume.Attributes.CastRecovery = Int32.Parse(stats); }
					else if ( entry == 32 ){ costume.Attributes.CastSpeed = Int32.Parse(stats); }
					else if ( entry == 33 ){ costume.Attributes.EnhancePotions = Int32.Parse(stats)*5; }
					else if ( entry == 34 ){ costume.Attributes.LowerManaCost = Int32.Parse(stats)*5; }
					else if ( entry == 35 ){ costume.Attributes.LowerRegCost = Int32.Parse(stats)*5; }
					else if ( entry == 36 ){ costume.Attributes.Luck = Int32.Parse(stats) * 50; }
					else if ( entry == 37 ){ costume.Attributes.ReflectPhysical = Int32.Parse(stats)*5; }
					else if ( entry == 38 ){ costume.Attributes.SpellDamage = Int32.Parse(stats)*5; }
					else if ( entry == 39 ){ costume.Attributes.WeaponDamage = Int32.Parse(stats)*5; }
					else if ( entry == 40 ){ costume.Attributes.WeaponSpeed = Int32.Parse(stats)*5; }
					else if ( entry == 41 && stats != "100" ){ costume.SkillBonuses.SetValues( 0, RaceSkill( Int32.Parse(stats) ), 10 ); }
					else if ( entry == 42 && stats != "100" ){ costume.SkillBonuses.SetValues( 1, RaceSkill( Int32.Parse(stats) ), 10 ); }
					else if ( entry == 43 ){ costume.SpeciesFood = Int32.Parse(stats); }
					else if ( entry == 44 ){ costume.SpeciesFemale = Int32.Parse(stats); }

					entry++;
				}
			}
		}

		public static bool GetMonsterSize( int race, string oSpecies, int oRace, int tavern )
		{
			if ( race > 80000 ){ race = GetBody( (race-80000) ); }

			int size = 1;
			string xSpecies = "";
			int allow = MySettings.S_MonsterCharacters;

			int id = GetID( race );

			string configs = RaceDefined( id );

			if ( configs.Length > 0 )
			{
				string[] setups = configs.Split(',');
				int entry = 1;
				foreach (string stats in setups)
				{
					if ( entry == 13 ){ size = Int32.Parse(stats); }
					else if ( entry == 10 ){ xSpecies = stats; }
					entry++;
				}

				if ( oSpecies != xSpecies && tavern > 0 )
					return false;

				if ( oRace == race && tavern > 0 )
					return true;

				if ( size <= allow )
					return true;
			}

			return false;
		}

		public static int GetMonsterFood( int race )
		{
			if ( race > 80000 ){ race = GetBody( (race-80000) ); }

			int food = 0;

			int id = GetID( race );

			string configs = RaceDefined( id );

			if ( configs.Length > 0 )
			{
				string[] setups = configs.Split(',');
				int entry = 1;
				foreach (string stats in setups)
				{
					if ( entry == 43 ){ food = Int32.Parse(stats); }
					entry++;
				}
			}

			return food;
		}

		public static string BeginStory( Mobile m, string world )
		{
			string rName = "";
			string rRace = "";
			string rType = "";
			string rLand = "";

			if ( m.FindItemOnLayer( Layer.Special ) != null )
			{
				if ( m.FindItemOnLayer( Layer.Special ) is BaseRace )
				{
					BaseRace info = (BaseRace)(m.FindItemOnLayer( Layer.Special ));
					rName = info.Name;
					rRace = info.SpeciesFamily;
					rType = info.SpeciesAlignment;
					rLand = info.SpeciesStart;
				}
			}

			string safe = "";
			if ( rType == "evil" && ((PlayerMobile)m).Fugitive == 0 )
			{
				if ( IsEvilSeaCreature( m ) )
				{
					safe = " You may be best served finding the pirate port of Anchor Rock, on an island you can reach through the magic gateway, where those docked there are more tolerable of the unsavory.";
					if ( world == "the Land of Lodoria" ){ safe = " You may be best served finding the pirate port of Kraken Reef, on an island you can reach through the magic gateway, where those docked there are more tolerable of the unsavory."; }
				}
				else if ( IsEvilDemonCreature( m ) )
				{
					safe = " You may be best served finding the City of Furnace, in a land you can reach through the magic gateway, where others are similar to you and would not shun you away.";
				}
				else if ( IsEvilDeadCreature( m ) )
				{
					safe = " You may be best served finding the Undercity of Umbra, deep within a cave you can reach through the magic gateway, where you would be left alone by the citizens there.";
					if ( world == "the Land of Lodoria" ){ safe = " You may be best served finding the Village of Ravendark, on an island you can reach through the magic gateway, where you would be left alone by the citizens there."; }
				}
			}

			string cave = Server.Items.BaseRace.StartName( m.RaceID );
			string zone = ", where a mystical portal suddenly appeared before you. A voice deep within your mind told you that this portal led to a life beyond what you know. Into a world inhabited by the likes of men. A land of mystery, adventure, and riches to be discovered.";
			string dead = "";
				if ( rLand == "sea" && rRace != "zombi" )
				{
					dead = " Others from the sea dared not enter this magical portal, as it would allow you to join the surface world, but rob you of your ability to survive under the waves as you once did.";
				}
				else if ( rRace == "mummy" || rRace == "zombi" || rRace == "skeleton" || rRace == "revenant" )
				{
					dead = " Unlike other undead that you can remember, you somehow feel different, even though you cannot remoember who you were or how you met your end. It is as if you have retained your soul of your former life.";
				}
				else if ( rRace == "vampyre" )
				{
					dead = " Unlike vampires that you can remember, you somehow feel different, even though you cannot remoember who you were or how you met your end. Although you have the thirst for blood, it is as if you have retained your soul of your former life, which sages refer to as vampyres. This is good as it will help you walk the lands during the daytime, without burning away.";
				}
			string path = " You decided to enter this magical vortex, and either live in peace or seek fame, riches, and power. You knew that once you went down this road, others of your kind would perhaps turn their backs on you, maybe attacking you on sight." + dead;
			string evil = "";
				if ( rType == "evil" ){ evil = " Because of who you are, and how the likes of men look upon creatures like yourself, you knew that you would have to prove yourself in their eyes. To become more famous, and to have good deeds widely spoken of, before you can be welcome in the villages and cities of the land. Though there are a few places that men look the other way, and care very little of who you are."; }
			if ( m is PlayerMobile )
			{
				if ( ((PlayerMobile)m).Fugitive > 0 )
				{
					path = " You decided to enter this magical vortex, and gather wealth and power to have your enemies bow before you. You knew that once you went down this road, others of your kind would surely loathe you, perhaps attacking you on sight." + dead;
					evil = " Because of who you are, and that you are looked upon as a murderous creature that must be vanquished, you will not be welcome in the villages and cities of the land. Though there are some rare places that men look the other way, and care very little of who you are.";
				}
			}

			path = path + evil + safe;

			if ( cave == "The Cave" )
			{
				if ( rRace == "illithid" )
					zone = "deep within the underdark of " + world + zone + path + "";
				else
					zone = "deep within a cave in " + world + zone + path + "";
			}
			else if ( cave == "The Tundra" )
			{
				if ( rRace == "devil" || rRace == "daemon" )
					zone = "in the frozen wastes of " + world + zone + path + "";
				else
					zone = "in the winterlands of " + world + zone + path + "";
			}
			else if ( cave == "The Pits" )
			{
				if ( rRace == "gargoyle" )
					zone = "in the pits, deep below " + world + zone + path + "";
				else if ( rName == "Fire Giant" || rRace == "naga" || rRace == "salamander" )
					zone = "in the volcanic caves of " + world + zone + path + "";
				else if ( rName == "Abysmal Giant" )
					zone = "in the underdark of " + world + zone + path + "";
				else
					zone = "in the hellish pits, deep below " + world + zone + path + "";
			}
			else if ( cave == "The Desert" )
			{
				if ( rRace == "mummy" )
					zone = "where you awoke in a tomb, not knowing who you were or how you met your end. You seem to be in a desert in " + world + zone + path + "";
				else
					zone = "in the hot deserts of " + world + zone + path + "";
			}
			else if ( cave == "The Sea" ){ zone = "under the seas of " + world + zone + path + ""; }
			else if ( cave == "The Mountains" ){ zone = "on the high mountains of " + world + zone + path + ""; }
			else if ( cave == "The Swamp" ){ zone = "in the putrid swamps of " + world + zone + path + ""; }
			else if ( cave == "The Tomb" )
			{
				if ( rRace == "golem" )
					zone = "where you awoke in a tomb, not knowing who you were or how you met your end. Whoever stitched you together seems to be gone now, nor can you recall where the various body parts came from that now make your body whole. You do remember, however, that you are in " + world + zone + path + "";
				else
					zone = "within a lost tomb in " + world + zone + path + "";
			}
			else if ( cave == "The Woods" ){ zone = "in the forests of " + world + zone + path + ""; }

			zone = zone + " This is where the rest of your life began, and its ending is uncertain.";

			string text = "Your journey began " + zone;

			return text;
		}

		public static void RemoveMyClothes( Mobile m )
		{
			if ( m.FindItemOnLayer( Layer.OuterTorso ) != null ) { m.FindItemOnLayer( Layer.OuterTorso ).Delete(); }
			if ( m.FindItemOnLayer( Layer.MiddleTorso ) != null ) { m.FindItemOnLayer( Layer.MiddleTorso ).Delete(); }
			if ( m.FindItemOnLayer( Layer.OneHanded ) != null ) { m.FindItemOnLayer( Layer.OneHanded ).Delete(); }
			if ( m.FindItemOnLayer( Layer.TwoHanded ) != null ) { m.FindItemOnLayer( Layer.TwoHanded ).Delete(); }
			if ( m.FindItemOnLayer( Layer.Bracelet ) != null ) { m.FindItemOnLayer( Layer.Bracelet ).Delete(); }
			if ( m.FindItemOnLayer( Layer.Ring ) != null ) { m.FindItemOnLayer( Layer.Ring ).Delete(); }
			if ( m.FindItemOnLayer( Layer.Helm ) != null ) { m.FindItemOnLayer( Layer.Helm ).Delete(); }
			if ( m.FindItemOnLayer( Layer.Arms ) != null ) { m.FindItemOnLayer( Layer.Arms ).Delete(); }
			if ( m.FindItemOnLayer( Layer.OuterLegs ) != null ) { m.FindItemOnLayer( Layer.OuterLegs ).Delete(); }
			if ( m.FindItemOnLayer( Layer.Neck ) != null ) { m.FindItemOnLayer( Layer.Neck ).Delete(); }
			if ( m.FindItemOnLayer( Layer.Gloves ) != null ) { m.FindItemOnLayer( Layer.Gloves ).Delete(); }
			if ( m.FindItemOnLayer( Layer.Trinket ) != null ) { m.FindItemOnLayer( Layer.Trinket ).Delete(); }
			if ( m.FindItemOnLayer( Layer.Shoes ) != null ) { m.FindItemOnLayer( Layer.Shoes ).Delete(); }
			if ( m.FindItemOnLayer( Layer.FirstValid ) != null ) { m.FindItemOnLayer( Layer.FirstValid ).Delete(); }
			if ( m.FindItemOnLayer( Layer.Waist ) != null ) { m.FindItemOnLayer( Layer.Waist ).Delete(); }
			if ( m.FindItemOnLayer( Layer.InnerLegs ) != null ) { m.FindItemOnLayer( Layer.InnerLegs ).Delete(); }
			if ( m.FindItemOnLayer( Layer.InnerTorso ) != null ) { m.FindItemOnLayer( Layer.InnerTorso ).Delete(); }
			if ( m.FindItemOnLayer( Layer.Pants ) != null ) { m.FindItemOnLayer( Layer.Pants ).Delete(); }
			if ( m.FindItemOnLayer( Layer.Shirt ) != null ) { m.FindItemOnLayer( Layer.Shirt ).Delete(); }
		}

		public static bool GetUndead( int race )
		{
			if ( race > 80000 ){ race = GetBody( (race-80000) ); }

			string dead = "";

			int id = GetID( race );

			string configs = RaceDefined( id );

			if ( configs.Length > 0 )
			{
				string[] setups = configs.Split(',');
				int entry = 1;
				foreach (string stats in setups)
				{
					if ( entry == 10 )
						dead = stats;

					entry++;
				}
			}

			if ( dead == "skeleton" )
				return true;
			else if ( dead == "vampyre" )
				return true;
			else if ( dead == "revenant" )
				return true;
			else if ( dead == "mummy" )
				return true;
			else if ( dead == "zombi" )
				return true;

			return false;
		}

		public static bool GetMonsterMage( int race )
		{
			if ( race > 80000 ){ race = GetBody( (race-80000) ); }

			bool mage = false;

			int id = GetID( race );

			string configs = RaceDefined( id );

			if ( configs.Length > 0 )
			{
				string[] setups = configs.Split(',');
				int entry = 1;
				foreach (string stats in setups)
				{
					if ( entry == 41 && Int32.Parse(stats) == 25 )
						return true;
					else if ( entry == 42 && Int32.Parse(stats) == 49 )
						return true;

					entry++;
				}
			}

			return mage;
		}

		public static void SetMonsterMagic( Mobile from, BaseRace skin )
		{
			int id = GetID( from.RaceID );
			int sk1 = 0;
			int sk2 = 0;

			string configs = RaceDefined( id );

			if ( configs.Length > 0 )
			{
				string[] setups = configs.Split(',');
				int entry = 1;
				foreach (string stats in setups)
				{
					if ( entry == 41 ){ sk1 = Int32.Parse(stats); }
					else if ( entry == 42 ){ sk2 = Int32.Parse(stats); }
					entry++;
				}
			}

			if ( sk1 == 25 && sk2 == 49 && from.RaceMagicSchool == 3 )
			{
				skin.SkillBonuses.Skill_1_Name = SkillName.Elementalism;
				skin.SkillBonuses.Skill_2_Name = SkillName.Meditation;
			}
			else if ( sk1 == 25 && sk2 != 49 && from.RaceMagicSchool == 3 )
			{
				skin.SkillBonuses.Skill_1_Name = SkillName.Elementalism;
			}
			else if ( sk1 != 25 && sk2 == 49 && from.RaceMagicSchool == 3 )
			{
				skin.SkillBonuses.Skill_2_Name = SkillName.Elementalism;
			}

			if ( sk1 == 25 && sk2 != 49 && from.RaceMagicSchool == 1 )
			{
				skin.SkillBonuses.Skill_1_Name = SkillName.Magery;
			}
			else if ( sk1 == 25 && sk2 != 49 && from.RaceMagicSchool == 2 )
			{
				skin.SkillBonuses.Skill_1_Name = SkillName.Necromancy;
			}

			if ( sk1 != 25 && sk2 == 49 && from.RaceMagicSchool == 1 )
			{
				skin.SkillBonuses.Skill_2_Name = SkillName.Magery;
			}
			else if ( sk1 != 25 && sk2 == 49 && from.RaceMagicSchool == 2 )
			{
				skin.SkillBonuses.Skill_2_Name = SkillName.Necromancy;
			}

			if ( from.RaceMagicSchool == 0 )
			{
				if ( sk1 == 25 )
				{
					skin.SkillBonuses.Skill_1_Name = SkillName.Magery;
				}
				if ( sk2 == 49 )
				{
					skin.SkillBonuses.Skill_2_Name = SkillName.Necromancy;
				}
			}
		}

		public static string StartArea( int race )
		{
			if ( race > 80000 ){ race = GetBody( (race-80000) ); }

			string start = "";

			int id = GetID( race );

			string configs = RaceDefined( id );

			if ( configs.Length > 0 )
			{
				string[] setups = configs.Split(',');
				int entry = 1;
				foreach (string stats in setups)
				{
					if ( entry == 12 ){ start = stats; }
					entry++;
				}
			}

			return start;
		}

		public static string StartName( int race )
		{
			string start = StartArea( race );
			string zone = "";

			if ( start == "cave" ){ zone = "The Cave"; }
			else if ( start == "ice" ){ zone = "The Tundra"; }
			else if ( start == "pits" ){ zone = "The Pits"; }
			else if ( start == "sand" ){ zone = "The Desert"; }
			else if ( start == "sea" ){ zone = "The Sea"; }
			else if ( start == "sky" ){ zone = "The Mountains"; }
			else if ( start == "swamp" ){ zone = "The Swamp"; }
			else if ( start == "tomb" ){ zone = "The Tomb"; }
			else if ( start == "woods" ){ zone = "The Woods"; }

			return zone;
		}

		public static string StartSentence( string start )
		{
			string zone = "";

			if ( start == "The Cave" ){ zone = "in a cave"; }
			else if ( start == "The Tundra" ){ zone = "in the winterlands"; }
			else if ( start == "The Pits" ){ zone = "in the hellish pits"; }
			else if ( start == "The Desert" ){ zone = "in the hot deserts"; }
			else if ( start == "The Sea" ){ zone = "under the sea"; }
			else if ( start == "The Mountains" ){ zone = "on the high mountains"; }
			else if ( start == "The Swamp" ){ zone = "in the putrid swamps"; }
			else if ( start == "The Tomb" ){ zone = "in a tomb"; }
			else if ( start == "The Woods" ){ zone = "in the dense forest"; }

			return zone;
		}

		public static int GetID( int raceID )
		{
			int id = 0;

			if ( raceID == 194 ){ id = 1; }
			else if ( raceID == 676 ){ id = 2; }
			else if ( raceID == 677 ){ id = 3; }
			else if ( raceID == 678 ){ id = 4; }
			else if ( raceID == 690 ){ id = 5; }
			else if ( raceID == 343 ){ id = 6; }
			else if ( raceID == 101 ){ id = 7; }
			else if ( raceID == 75 ){ id = 8; }
			else if ( raceID == 475 ){ id = 9; }
			else if ( raceID == 259 ){ id = 10; }
			else if ( raceID == 43 ){ id = 11; }
			else if ( raceID == 38 ){ id = 12; }
			else if ( raceID == 40 ){ id = 13; }
			else if ( raceID == 102 ){ id = 14; }
			else if ( raceID == 88 ){ id = 15; }
			else if ( raceID == 765 ){ id = 16; }
			else if ( raceID == 9 ){ id = 17; }
			else if ( raceID == 10 ){ id = 18; }
			else if ( raceID == 320 ){ id = 19; }
			else if ( raceID == 748 ){ id = 20; }
			else if ( raceID == 764 ){ id = 21; }
			else if ( raceID == 146 ){ id = 22; }
			else if ( raceID == 112 ){ id = 23; }
			else if ( raceID == 126 ){ id = 24; }
			else if ( raceID == 93 ){ id = 25; }
			else if ( raceID == 137 ){ id = 26; }
			else if ( raceID == 195 ){ id = 27; }
			else if ( raceID == 509 ){ id = 28; }
			else if ( raceID == 191 ){ id = 29; }
			else if ( raceID == 427 ){ id = 30; }
			else if ( raceID == 138 ){ id = 31; }
			else if ( raceID == 804 ){ id = 32; }
			else if ( raceID == 436 ){ id = 33; }
			else if ( raceID == 766 ){ id = 34; }
			else if ( raceID == 668 ){ id = 35; }
			else if ( raceID == 669 ){ id = 36; }
			else if ( raceID == 670 ){ id = 37; }
			else if ( raceID == 301 ){ id = 38; }
			else if ( raceID == 309 ){ id = 39; }
			else if ( raceID == 312 ){ id = 40; }
			else if ( raceID == 285 ){ id = 41; }
			else if ( raceID == 313 ){ id = 42; }
			else if ( raceID == 89 ){ id = 43; }
			else if ( raceID == 2 ){ id = 44; }
			else if ( raceID == 18 ){ id = 45; }
			else if ( raceID == 729 ){ id = 46; }
			else if ( raceID == 730 ){ id = 47; }
			else if ( raceID == 316 ){ id = 48; }
			else if ( raceID == 732 ){ id = 49; }
			else if ( raceID == 128 ){ id = 50; }
			else if ( raceID == 356 ){ id = 51; }
			else if ( raceID == 363 ){ id = 52; }
			else if ( raceID == 127 ){ id = 53; }
			else if ( raceID == 257 ){ id = 54; }
			else if ( raceID == 4 ){ id = 55; }
			else if ( raceID == 158 ){ id = 56; }
			else if ( raceID == 772 ){ id = 57; }
			else if ( raceID == 773 ){ id = 58; }
			else if ( raceID == 433 ){ id = 59; }
			else if ( raceID == 774 ){ id = 60; }
			else if ( raceID == 264 ){ id = 61; }
			else if ( raceID == 777 ){ id = 62; }
			else if ( raceID == 325 ){ id = 63; }
			else if ( raceID == 725 ){ id = 64; }
			else if ( raceID == 726 ){ id = 65; }
			else if ( raceID == 771 ){ id = 66; }
			else if ( raceID == 770 ){ id = 67; }
			else if ( raceID == 792 ){ id = 68; }
			else if ( raceID == 485 ){ id = 69; }
			else if ( raceID == 510 ){ id = 70; }
			else if ( raceID == 592 ){ id = 71; }
			else if ( raceID == 632 ){ id = 72; }
			else if ( raceID == 647 ){ id = 73; }
			else if ( raceID == 69 ){ id = 74; }
			else if ( raceID == 999 ){ id = 75; }
			else if ( raceID == 11 ){ id = 76; }
			else if ( raceID == 786 ){ id = 77; }
			else if ( raceID == 202 ){ id = 78; }
			else if ( raceID == 359 ){ id = 79; }
			else if ( raceID == 176 ){ id = 80; }
			else if ( raceID == 245 ){ id = 81; }
			else if ( raceID == 253 ){ id = 82; }
			else if ( raceID == 255 ){ id = 83; }
			else if ( raceID == 78 ){ id = 84; }
			else if ( raceID == 263 ){ id = 85; }
			else if ( raceID == 280 ){ id = 86; }
			else if ( raceID == 281 ){ id = 87; }
			else if ( raceID == 357 ){ id = 88; }
			else if ( raceID == 650 ){ id = 89; }
			else if ( raceID == 154 ){ id = 90; }
			else if ( raceID == 601 ){ id = 91; }
			else if ( raceID == 341 ){ id = 92; }
			else if ( raceID == 342 ){ id = 93; }
			else if ( raceID == 261 ){ id = 94; }
			else if ( raceID == 704 ){ id = 95; }
			else if ( raceID == 66 ){ id = 96; }
			else if ( raceID == 1 ){ id = 97; }
			else if ( raceID == 428 ){ id = 98; }
			else if ( raceID == 303 ){ id = 99; }
			else if ( raceID == 7 ){ id = 100; }
			else if ( raceID == 17 ){ id = 101; }
			else if ( raceID == 41 ){ id = 102; }
			else if ( raceID == 108 ){ id = 103; }
			else if ( raceID == 182 ){ id = 104; }
			else if ( raceID == 328 ){ id = 105; }
			else if ( raceID == 65 ){ id = 106; }
			else if ( raceID == 20 ){ id = 107; }
			else if ( raceID == 157 ){ id = 108; }
			else if ( raceID == 252 ){ id = 109; }
			else if ( raceID == 758 ){ id = 110; }
			else if ( raceID == 779 ){ id = 111; }
			else if ( raceID == 172 ){ id = 112; }
			else if ( raceID == 534 ){ id = 113; }
			else if ( raceID == 33 ){ id = 114; }
			else if ( raceID == 34 ){ id = 115; }
			else if ( raceID == 35 ){ id = 116; }
			else if ( raceID == 324 ){ id = 117; }
			else if ( raceID == 326 ){ id = 118; }
			else if ( raceID == 333 ){ id = 119; }
			else if ( raceID == 541 ){ id = 120; }
			else if ( raceID == 768 ){ id = 121; }
			else if ( raceID == 42 ){ id = 122; }
			else if ( raceID == 44 ){ id = 123; }
			else if ( raceID == 45 ){ id = 124; }
			else if ( raceID == 73 ){ id = 125; }
			else if ( raceID == 163 ){ id = 126; }
			else if ( raceID == 164 ){ id = 127; }
			else if ( raceID == 165 ){ id = 128; }
			else if ( raceID == 673 ){ id = 129; }
			else if ( raceID == 271 ){ id = 130; }
			else if ( raceID == 86 ){ id = 131; }
			else if ( raceID == 85 ){ id = 132; }
			else if ( raceID == 87 ){ id = 133; }
			else if ( raceID == 306 ){ id = 134; }
			else if ( raceID == 145 ){ id = 135; }
			else if ( raceID == 143 ){ id = 136; }
			else if ( raceID == 144 ){ id = 137; }
			else if ( raceID == 50 ){ id = 138; }
			else if ( raceID == 56 ){ id = 139; }
			else if ( raceID == 57 ){ id = 140; }
			else if ( raceID == 110 ){ id = 141; }
			else if ( raceID == 148 ){ id = 142; }
			else if ( raceID == 167 ){ id = 143; }
			else if ( raceID == 168 ){ id = 144; }
			else if ( raceID == 170 ){ id = 145; }
			else if ( raceID == 247 ){ id = 146; }
			else if ( raceID == 699 ){ id = 147; }
			else if ( raceID == 724 ){ id = 148; }
			else if ( raceID == 24 ){ id = 149; }
			else if ( raceID == 314 ){ id = 150; }
			else if ( raceID == 808 ){ id = 151; }
			else if ( raceID == 689 ){ id = 152; }
			else if ( raceID == 149 ){ id = 153; }
			else if ( raceID == 174 ){ id = 154; }
			else if ( raceID == 76 ){ id = 155; }
			else if ( raceID == 189 ){ id = 156; }
			else if ( raceID == 156 ){ id = 157; }
			else if ( raceID == 499 ){ id = 158; }
			else if ( raceID == 53 ){ id = 159; }
			else if ( raceID == 54 ){ id = 160; }
			else if ( raceID == 439 ){ id = 161; }
			else if ( raceID == 95 ){ id = 162; }
			else if ( raceID == 124 ){ id = 163; }
			else if ( raceID == 125 ){ id = 164; }
			else if ( raceID == 311 ){ id = 165; }
			else if ( raceID == 3 ){ id = 166; }
			else if ( raceID == 181 ){ id = 167; }
			else if ( raceID == 304 ){ id = 168; }
			else if ( raceID == 305 ){ id = 169; }
			else if ( raceID == 307 ){ id = 170; }
			else if ( raceID == 728 ){ id = 171; }
			else if ( raceID == 810 ){ id = 172; }

			return id;
		}

		public static int GetBody( int id )
		{
			int race = 0;

			if ( id == 1 ){ race = 194; }
			else if ( id == 2 ){ race = 676; }
			else if ( id == 3 ){ race = 677; }
			else if ( id == 4 ){ race = 678; }
			else if ( id == 5 ){ race = 690; }
			else if ( id == 6 ){ race = 343; }
			else if ( id == 7 ){ race = 101; }
			else if ( id == 8 ){ race = 75; }
			else if ( id == 9 ){ race = 475; }
			else if ( id == 10 ){ race = 259; }
			else if ( id == 11 ){ race = 43; }
			else if ( id == 12 ){ race = 38; }
			else if ( id == 13 ){ race = 40; }
			else if ( id == 14 ){ race = 102; }
			else if ( id == 15 ){ race = 88; }
			else if ( id == 16 ){ race = 765; }
			else if ( id == 17 ){ race = 9; }
			else if ( id == 18 ){ race = 10; }
			else if ( id == 19 ){ race = 320; }
			else if ( id == 20 ){ race = 748; }
			else if ( id == 21 ){ race = 764; }
			else if ( id == 22 ){ race = 146; }
			else if ( id == 23 ){ race = 112; }
			else if ( id == 24 ){ race = 126; }
			else if ( id == 25 ){ race = 93; }
			else if ( id == 26 ){ race = 137; }
			else if ( id == 27 ){ race = 195; }
			else if ( id == 28 ){ race = 509; }
			else if ( id == 29 ){ race = 191; }
			else if ( id == 30 ){ race = 427; }
			else if ( id == 31 ){ race = 138; }
			else if ( id == 32 ){ race = 804; }
			else if ( id == 33 ){ race = 436; }
			else if ( id == 34 ){ race = 766; }
			else if ( id == 35 ){ race = 668; }
			else if ( id == 36 ){ race = 669; }
			else if ( id == 37 ){ race = 670; }
			else if ( id == 38 ){ race = 301; }
			else if ( id == 39 ){ race = 309; }
			else if ( id == 40 ){ race = 312; }
			else if ( id == 41 ){ race = 285; }
			else if ( id == 42 ){ race = 313; }
			else if ( id == 43 ){ race = 89; }
			else if ( id == 44 ){ race = 2; }
			else if ( id == 45 ){ race = 18; }
			else if ( id == 46 ){ race = 729; }
			else if ( id == 47 ){ race = 730; }
			else if ( id == 48 ){ race = 316; }
			else if ( id == 49 ){ race = 732; }
			else if ( id == 50 ){ race = 128; }
			else if ( id == 51 ){ race = 356; }
			else if ( id == 52 ){ race = 363; }
			else if ( id == 53 ){ race = 127; }
			else if ( id == 54 ){ race = 257; }
			else if ( id == 55 ){ race = 4; }
			else if ( id == 56 ){ race = 158; }
			else if ( id == 57 ){ race = 772; }
			else if ( id == 58 ){ race = 773; }
			else if ( id == 59 ){ race = 433; }
			else if ( id == 60 ){ race = 774; }
			else if ( id == 61 ){ race = 264; }
			else if ( id == 62 ){ race = 777; }
			else if ( id == 63 ){ race = 325; }
			else if ( id == 64 ){ race = 725; }
			else if ( id == 65 ){ race = 726; }
			else if ( id == 66 ){ race = 771; }
			else if ( id == 67 ){ race = 770; }
			else if ( id == 68 ){ race = 792; }
			else if ( id == 69 ){ race = 485; }
			else if ( id == 70 ){ race = 510; }
			else if ( id == 71 ){ race = 592; }
			else if ( id == 72 ){ race = 632; }
			else if ( id == 73 ){ race = 647; }
			else if ( id == 74 ){ race = 69; }
			else if ( id == 75 ){ race = 999; }
			else if ( id == 76 ){ race = 11; }
			else if ( id == 77 ){ race = 786; }
			else if ( id == 78 ){ race = 202; }
			else if ( id == 79 ){ race = 359; }
			else if ( id == 80 ){ race = 176; }
			else if ( id == 81 ){ race = 245; }
			else if ( id == 82 ){ race = 253; }
			else if ( id == 83 ){ race = 255; }
			else if ( id == 84 ){ race = 78; }
			else if ( id == 85 ){ race = 263; }
			else if ( id == 86 ){ race = 280; }
			else if ( id == 87 ){ race = 281; }
			else if ( id == 88 ){ race = 357; }
			else if ( id == 89 ){ race = 650; }
			else if ( id == 90 ){ race = 154; }
			else if ( id == 91 ){ race = 601; }
			else if ( id == 92 ){ race = 341; }
			else if ( id == 93 ){ race = 342; }
			else if ( id == 94 ){ race = 261; }
			else if ( id == 95 ){ race = 704; }
			else if ( id == 96 ){ race = 66; }
			else if ( id == 97 ){ race = 1; }
			else if ( id == 98 ){ race = 428; }
			else if ( id == 99 ){ race = 303; }
			else if ( id == 100 ){ race = 7; }
			else if ( id == 101 ){ race = 17; }
			else if ( id == 102 ){ race = 41; }
			else if ( id == 103 ){ race = 108; }
			else if ( id == 104 ){ race = 182; }
			else if ( id == 105 ){ race = 328; }
			else if ( id == 106 ){ race = 65; }
			else if ( id == 107 ){ race = 20; }
			else if ( id == 108 ){ race = 157; }
			else if ( id == 109 ){ race = 252; }
			else if ( id == 110 ){ race = 758; }
			else if ( id == 111 ){ race = 779; }
			else if ( id == 112 ){ race = 172; }
			else if ( id == 113 ){ race = 534; }
			else if ( id == 114 ){ race = 33; }
			else if ( id == 115 ){ race = 34; }
			else if ( id == 116 ){ race = 35; }
			else if ( id == 117 ){ race = 324; }
			else if ( id == 118 ){ race = 326; }
			else if ( id == 119 ){ race = 333; }
			else if ( id == 120 ){ race = 541; }
			else if ( id == 121 ){ race = 768; }
			else if ( id == 122 ){ race = 42; }
			else if ( id == 123 ){ race = 44; }
			else if ( id == 124 ){ race = 45; }
			else if ( id == 125 ){ race = 73; }
			else if ( id == 126 ){ race = 163; }
			else if ( id == 127 ){ race = 164; }
			else if ( id == 128 ){ race = 165; }
			else if ( id == 129 ){ race = 673; }
			else if ( id == 130 ){ race = 271; }
			else if ( id == 131 ){ race = 86; }
			else if ( id == 132 ){ race = 85; }
			else if ( id == 133 ){ race = 87; }
			else if ( id == 134 ){ race = 306; }
			else if ( id == 135 ){ race = 145; }
			else if ( id == 136 ){ race = 143; }
			else if ( id == 137 ){ race = 144; }
			else if ( id == 138 ){ race = 50; }
			else if ( id == 139 ){ race = 56; }
			else if ( id == 140 ){ race = 57; }
			else if ( id == 141 ){ race = 110; }
			else if ( id == 142 ){ race = 148; }
			else if ( id == 143 ){ race = 167; }
			else if ( id == 144 ){ race = 168; }
			else if ( id == 145 ){ race = 170; }
			else if ( id == 146 ){ race = 247; }
			else if ( id == 147 ){ race = 699; }
			else if ( id == 148 ){ race = 724; }
			else if ( id == 149 ){ race = 24; }
			else if ( id == 150 ){ race = 314; }
			else if ( id == 151 ){ race = 808; }
			else if ( id == 152 ){ race = 689; }
			else if ( id == 153 ){ race = 149; }
			else if ( id == 154 ){ race = 174; }
			else if ( id == 155 ){ race = 76; }
			else if ( id == 156 ){ race = 189; }
			else if ( id == 157 ){ race = 156; }
			else if ( id == 158 ){ race = 499; }
			else if ( id == 159 ){ race = 53; }
			else if ( id == 160 ){ race = 54; }
			else if ( id == 161 ){ race = 439; }
			else if ( id == 162 ){ race = 95; }
			else if ( id == 163 ){ race = 124; }
			else if ( id == 164 ){ race = 125; }
			else if ( id == 165 ){ race = 311; }
			else if ( id == 166 ){ race = 3; }
			else if ( id == 167 ){ race = 181; }
			else if ( id == 168 ){ race = 304; }
			else if ( id == 169 ){ race = 305; }
			else if ( id == 170 ){ race = 307; }
			else if ( id == 171 ){ race = 728; }
			else if ( id == 172 ){ race = 810; }

			return race;
		}

		public static void CreateRace( Mobile m, int id, bool makeOne )
		{
			if ( m.Alive && m is PlayerMobile )
			{
				if ( m.FindItemOnLayer( Layer.Special ) != null )
				{
					if ( m.FindItemOnLayer( Layer.Special ) is BaseRace )
					{
						// THEY ALREADY HAVE ONE

						BaseRace skin = (BaseRace)(m.FindItemOnLayer( Layer.Special ));

						skin.Owner = m;
						m.BodyMod = skin.SpeciesID;
						m.HueMod = 0;
						m.RaceID = skin.SpeciesID;
						m.RaceAngerSound = skin.SpeciesAngerSound;
						m.RaceIdleSound = skin.SpeciesIdleSound;
						m.RaceDeathSound = skin.SpeciesDeathSound;
						m.RaceAttackSound = skin.SpeciesAttackSound;
						m.RaceHurtSound = skin.SpeciesHurtSound;

						Mobiles.IMount mt = m.Mount;
						if ( mt != null )
						{
							Server.Mobiles.EtherealMount.EthyDismount( m );
							mt.Rider = null;
						}
					}
					else
						makeOne = true;
				}
				else
					makeOne = true;

				if ( makeOne )
				{
					Item race = GetCostume( id );

					if ( m.FindItemOnLayer( Layer.Special ) != null ){ (m.FindItemOnLayer( Layer.Special )).Delete(); }

					ArrayList costume = new ArrayList();
					foreach ( Item item in World.Items.Values )
					{
						if ( item is BaseRace )
						{
							if ( ((BaseRace)item).Owner == m )
							{
								costume.Add( item );
							}
						}
					}
					for ( int i = 0; i < costume.Count; ++i )
					{
						Item item = ( Item )costume[ i ];
						item.Delete();
					}

					m.AddToBackpack( race );
					m.EquipItem( race );
				}

				if ( m.FindItemOnLayer( Layer.Special ) != null )
				{
					if ( m.FindItemOnLayer( Layer.Special ) is BaseRace )
						Server.Items.BaseRace.SetMonsterMagic( m, (BaseRace)(m.FindItemOnLayer( Layer.Special )) );
				}
			}
			m.InvalidateProperties();
		}

		public static void SyncRace( Mobile m, bool LevelUp )
		{
			if ( m.RaceID > 0 && m.Alive )
			{
				if ( m.BodyMod == 0 )
					CreateRace( m, m.RaceID, false );

				if ( m.FindItemOnLayer( Layer.Special ) == null )
					CreateRace( m, m.RaceID, false );

				if ( LevelUp )
					SetProperties( m );

				m.InvalidateProperties();
			}
			else if ( m.RaceID > 0 && !m.Alive )
			{
				if ( m.FindItemOnLayer( Layer.Special ) != null && m.FindItemOnLayer( Layer.Special ) is BaseRace && m is PlayerMobile )
				{
					(m.FindItemOnLayer( Layer.Special )).Delete();
				}

				m.InvalidateProperties();
			}
		}

		public static void BackToHuman( Mobile m )
		{
      		if ( m.RaceID > 0 )
      		{
				if ( m.FindItemOnLayer( Layer.Special ) != null ){ (m.FindItemOnLayer( Layer.Special )).Delete(); }

				m.BodyMod = 0;
				m.HueMod = -1;
				m.RaceID = 0;
				m.RaceAngerSound = 0;
				m.RaceIdleSound = 0;
				m.RaceDeathSound = 0;
				m.RaceAttackSound = 0;
				m.RaceHurtSound = 0;
				m.RaceHomeLand = 0;
				m.Female = m.RaceWasFemale;
      		}
		}

		public BaseRace( Serial serial ) : base( serial )
		{
		}

		public static void SetProperties( Mobile m )
		{
			if ( m.RaceID > 0 && m.Alive && m is PlayerMobile )
			{
				if ( m.FindItemOnLayer( Layer.Special ) != null && m.FindItemOnLayer( Layer.Special ) is BaseRace && m is PlayerMobile )
				{
					BaseRace current = (BaseRace)(m.FindItemOnLayer( Layer.Special ));
					int level = Server.Misc.GetPlayerInfo.GetPlayerLevel( m );

					if ( current.SpeciesLevel != level )
					{
						Item item = GetCostume( m.RaceID );
						BaseRace race = (BaseRace)item;
						current.SpeciesLevel = level;
						if ( level < 2 ){ level = 0; }

						if ( race.Resistances.Physical > 0 ){ current.Resistances.Physical = race.Resistances.Physical + (int)(level*0.1); }
						if ( race.Resistances.Fire > 0 ){ current.Resistances.Fire = race.Resistances.Fire + (int)(level*0.1); }
						if ( race.Resistances.Cold > 0 ){ current.Resistances.Cold = race.Resistances.Cold + (int)(level*0.1); }
						if ( race.Resistances.Poison > 0 ){ current.Resistances.Poison = race.Resistances.Poison + (int)(level*0.1); }
						if ( race.Resistances.Energy > 0 ){ current.Resistances.Energy = race.Resistances.Energy + (int)(level*0.1); }

						if ( race.Attributes.AttackChance > 0 ){ current.Attributes.AttackChance = race.Attributes.AttackChance + (int)( level * 0.1 ); }
						if ( race.Attributes.BonusDex > 0 ){ current.Attributes.BonusDex = race.Attributes.BonusDex + (int)( level * 0.1 ); }
						if ( race.Attributes.BonusHits > 0 ){ current.Attributes.BonusHits = race.Attributes.BonusHits + (int)( level * 0.3 ); }
						if ( race.Attributes.BonusInt > 0 ){ current.Attributes.BonusInt = race.Attributes.BonusInt + (int)( level * 0.1 ); }
						if ( race.Attributes.BonusMana > 0 ){ current.Attributes.BonusMana = race.Attributes.BonusMana + (int)( level * 0.3 ); }
						if ( race.Attributes.BonusStam > 0 ){ current.Attributes.BonusStam = race.Attributes.BonusStam + (int)( level * 0.3 ); }
						if ( race.Attributes.BonusStr > 0 ){ current.Attributes.BonusStr = race.Attributes.BonusStr + (int)( level * 0.1 ); }
						if ( race.Attributes.CastRecovery > 0 ){ current.Attributes.CastRecovery = race.Attributes.CastRecovery + (int)( level * 0.03 ); }
						if ( race.Attributes.CastSpeed > 0 ){ current.Attributes.CastSpeed = race.Attributes.CastSpeed + (int)( level * 0.03 ); }
						if ( race.Attributes.DefendChance > 0 ){ current.Attributes.DefendChance = race.Attributes.DefendChance + (int)( level * 0.1 ); }
						if ( race.Attributes.EnhancePotions > 0 ){ current.Attributes.EnhancePotions = race.Attributes.EnhancePotions + (int)( level * 0.4 ); }
						if ( race.Attributes.LowerManaCost > 0 ){ current.Attributes.LowerManaCost = race.Attributes.LowerManaCost + (int)( level * 0.3 ); }
						if ( race.Attributes.LowerRegCost > 0 ){ current.Attributes.LowerRegCost = race.Attributes.LowerRegCost + (int)( level * 0.3 ); }
						if ( race.Attributes.Luck > 0 ){ current.Attributes.Luck = race.Attributes.Luck + (int)( level * 5 ); }
						if ( race.Attributes.ReflectPhysical > 0 ){ current.Attributes.ReflectPhysical = race.Attributes.ReflectPhysical + (int)( level * 0.2 ); }
						if ( race.Attributes.RegenHits > 0 ){ current.Attributes.RegenHits = race.Attributes.RegenHits + (int)( level * 0.03 ); }
						if ( race.Attributes.RegenMana > 0 ){ current.Attributes.RegenMana = race.Attributes.RegenMana + (int)( level * 0.03 ); }
						if ( race.Attributes.RegenStam > 0 ){ current.Attributes.RegenStam = race.Attributes.RegenStam + (int)( level * 0.03 ); }
						if ( race.Attributes.SpellDamage > 0 ){ current.Attributes.SpellDamage = race.Attributes.SpellDamage + (int)( level * 0.1 ); }
						if ( race.Attributes.WeaponDamage > 0 ){ current.Attributes.WeaponDamage = race.Attributes.WeaponDamage + (int)( level * 0.1 ); }
						if ( race.Attributes.WeaponSpeed > 0 ){ current.Attributes.WeaponSpeed = race.Attributes.WeaponSpeed + (int)( level * 0.2 ); }

						if ( race.SkillBonuses.Skill_1_Value > 0 ){ current.SkillBonuses.Skill_1_Value = race.SkillBonuses.Skill_1_Value + (int)( level / 2 ); }
						if ( race.SkillBonuses.Skill_2_Value > 0 ){ current.SkillBonuses.Skill_2_Value = race.SkillBonuses.Skill_2_Value + (int)( level / 2 ); }

						race.Delete();

						current.m_AosSkillBonuses.AddTo( m );
						current.AddStatBonuses( m );
						m.CheckStatTimers();
					}
				}
			}
		}

		public static bool NoFood( int raceID )
		{
			if ( GetMonsterFood( raceID ) == 1 )
				return true;

			return false;
		}

		public static bool NoFoodOrDrink( int raceID )
		{
			if ( GetMonsterFood( raceID ) == 2 )
				return true;

			return false;
		}

		public static bool BloodDrinker( int raceID )
		{
			if ( GetMonsterFood( raceID ) == 3 )
				return true;

			return false;
		}

		public static bool BrainEater( int raceID )
		{
			if ( GetMonsterFood( raceID ) == 4 )
				return true;

			return false;
		}

		public static bool NightEyes( int raceID )
		{
			if ( raceID > 80000 ){ raceID = GetBody( (raceID-80000) ); }

			int eyes = 0;

			int id = GetID( raceID );

			string configs = RaceDefined( id );

			if ( configs.Length > 0 )
			{
				string[] setups = configs.Split(',');
				int entry = 1;
				foreach (string stats in setups)
				{
					if ( entry == 28 ){ eyes = Int32.Parse(stats); }
					entry++;
				}
			}

			if ( eyes > 0 )
				return true;

			return false;
		}

		public static bool IsBleeder( Mobile m )
		{
			if ( m is PlayerMobile && m.FindItemOnLayer( Layer.Special ) != null && m.FindItemOnLayer( Layer.Special ) is BaseRace )
			{
				BaseRace skin = (BaseRace)(m.FindItemOnLayer( Layer.Special ));

				if ( 	skin.SpeciesID == 433 || // EARTH GIANT
						skin.SpeciesID == 485 || // STONE GIANT
						skin.SpeciesFamily == "mushroom" || 
						skin.SpeciesFamily == "plant" || 
						skin.SpeciesFamily == "reaper" || 
						skin.SpeciesFamily == "ent" || 
						skin.SpeciesFamily == "skeleton" || 
						skin.SpeciesFamily == "mummy" || 
						skin.SpeciesFamily == "revenant" )
					return false; 
			}

			return true;
		}

		public static bool IsEvil( Mobile m )
		{
			if ( m is PlayerMobile && m.FindItemOnLayer( Layer.Special ) != null && m.FindItemOnLayer( Layer.Special ) is BaseRace )
			{
				BaseRace skin = (BaseRace)(m.FindItemOnLayer( Layer.Special ));
				if ( skin.SpeciesAlignment == "evil" )
					return true; 
			}

			return false;
		}

		public static bool IsGood( Mobile m )
		{
			if ( m is PlayerMobile && m.FindItemOnLayer( Layer.Special ) != null && m.FindItemOnLayer( Layer.Special ) is BaseRace )
			{
				BaseRace skin = (BaseRace)(m.FindItemOnLayer( Layer.Special ));
				if ( skin.SpeciesAlignment == "good" )
					return true; 
			}

			return false;
		}

		public static bool IsRavendarkCreature( Mobile m )
		{
			if ( m is PlayerMobile && m.FindItemOnLayer( Layer.Special ) != null && m.FindItemOnLayer( Layer.Special ) is BaseRace )
			{
				BaseRace skin = (BaseRace)(m.FindItemOnLayer( Layer.Special ));
				if ( skin.SpeciesStart != "sea" && 
						(
							skin.SpeciesFamily == "daemon" || 
							skin.SpeciesFamily == "demon" || 
							skin.SpeciesFamily == "devil" || 
							skin.SpeciesFamily == "succubus" || 
							skin.SpeciesFamily == "imp" || 
							skin.SpeciesFamily == "mummy" || 
							skin.SpeciesFamily == "skeleton" || 
							skin.SpeciesFamily == "illithid" || 
							skin.SpeciesFamily == "revenant" || 
							skin.SpeciesFamily == "golem" || 
							skin.SpeciesFamily == "vampyre" || 
							skin.SpeciesFamily == "zombi"
						)
					)
					return true; 
			}

			return false;
		}

		public static bool IsEvilDeadCreature( Mobile m )
		{
			if ( m is PlayerMobile && m.FindItemOnLayer( Layer.Special ) != null && m.FindItemOnLayer( Layer.Special ) is BaseRace )
			{
				BaseRace skin = (BaseRace)(m.FindItemOnLayer( Layer.Special ));
				if ( skin.SpeciesAlignment == "evil" && ((PlayerMobile)m).Fugitive == 0 && 
						(
							skin.Name == "Shadow Demon" || 
							skin.SpeciesFamily == "mummy" || 
							skin.SpeciesFamily == "skeleton" || 
							skin.SpeciesFamily == "illithid" || 
							skin.SpeciesFamily == "revenant" || 
							skin.SpeciesFamily == "golem" || 
							skin.SpeciesFamily == "vampyre" || 
							skin.SpeciesFamily == "zombi"
						)
					)
					return true; 
			}

			return false;
		}

		public static bool IsEvilSeaCreature( Mobile m )
		{
			if ( m is PlayerMobile && m.FindItemOnLayer( Layer.Special ) != null && m.FindItemOnLayer( Layer.Special ) is BaseRace )
			{
				BaseRace skin = (BaseRace)(m.FindItemOnLayer( Layer.Special ));
				if ( skin.SpeciesAlignment == "evil" && ((PlayerMobile)m).Fugitive == 0 && 
						(
							( skin.SpeciesStart == "sea" && skin.SpeciesFamily != "zombi" ) || 
							skin.SpeciesFamily == "dagon" || 
							skin.SpeciesFamily == "plant" 
						)
					)
					return true; 
			}

			return false;
		}

		public static bool IsEvilDemonCreature( Mobile m )
		{
			if ( m is PlayerMobile && m.FindItemOnLayer( Layer.Special ) != null && m.FindItemOnLayer( Layer.Special ) is BaseRace )
			{
				BaseRace skin = (BaseRace)(m.FindItemOnLayer( Layer.Special ));
				if ( skin.SpeciesAlignment == "evil" && skin.SpeciesStart != "sea" && skin.Name != "Shadow Demon" && ((PlayerMobile)m).Fugitive == 0 && 
						(
							skin.SpeciesFamily == "daemon" || 
							skin.SpeciesFamily == "demon" || 
							skin.SpeciesFamily == "devil" || 
							skin.SpeciesFamily == "giant" || 
							skin.SpeciesFamily == "succubus" 
						)
					)
					return true; 
			}

			return false;
		}

		public static string GetAbilities( int raceID )
		{
			string txt = "";

			if ( raceID > 0 )
			{
				Item item = GetCostume( raceID );
				BaseRace race = (BaseRace)item;

				if ( race.Resistances.Physical > 0 ){ txt = txt + "<BR>Physical Resist: " + race.Resistances.Physical + "%"; }
				if ( race.Resistances.Fire > 0 ){ txt = txt + "<BR>Fire Resist: " + race.Resistances.Fire + "%"; }
				if ( race.Resistances.Cold > 0 ){ txt = txt + "<BR>Cold Resist: " + race.Resistances.Cold + "%"; }
				if ( race.Resistances.Poison > 0 ){ txt = txt + "<BR>Poison Resist: " + race.Resistances.Poison + "%"; }
				if ( race.Resistances.Energy > 0 ){ txt = txt + "<BR>Energy Resist: " + race.Resistances.Energy + "%"; }
				if ( race.Attributes.WeaponDamage > 0 ){ txt = txt + "<BR>Damage Increase: " + race.Attributes.WeaponDamage + "%"; }
				if ( race.Attributes.DefendChance > 0 ){ txt = txt + "<BR>Defend Chance Increase: " + race.Attributes.DefendChance + "%"; }
				if ( race.Attributes.BonusDex > 0 ){ txt = txt + "<BR>Dexterity Bonus: " + race.Attributes.BonusDex + ""; }
				if ( race.Attributes.EnhancePotions > 0 ){ txt = txt + "<BR>Enhance Potions: " + race.Attributes.EnhancePotions + "%"; }
				if ( race.Attributes.CastSpeed > 0 ){ txt = txt + "<BR>Faster Casting: " + race.Attributes.CastSpeed + ""; }
				if ( race.Attributes.CastRecovery > 0 ){ txt = txt + "<BR>Faster Cast Recovery: " + race.Attributes.CastRecovery + ""; }
				if ( race.Attributes.AttackChance > 0 ){ txt = txt + "<BR>Hit Chance Increase: " + race.Attributes.AttackChance + "%"; }
				if ( race.Attributes.BonusHits > 0 ){ txt = txt + "<BR>Hit Point Increase: " + race.Attributes.BonusHits + ""; }
				if ( race.Attributes.RegenHits > 0 ){ txt = txt + "<BR>Hit Point Regeneration: " + race.Attributes.RegenHits + ""; }
				if ( race.Attributes.BonusInt > 0 ){ txt = txt + "<BR>Intelligence Bonus: " + race.Attributes.BonusInt + ""; }
				if ( race.Attributes.LowerManaCost > 0 ){ txt = txt + "<BR>Lower Mana Cost: " + race.Attributes.LowerManaCost + "%"; }
				if ( race.Attributes.LowerRegCost > 0 ){ txt = txt + "<BR>Lower Reagent Cost: " + race.Attributes.LowerRegCost + "%"; }
				if ( race.Attributes.Luck > 0 ){ txt = txt + "<BR>Luck: " + race.Attributes.Luck + ""; }
				if ( race.Attributes.BonusMana > 0 ){ txt = txt + "<BR>Mana Increase: " + race.Attributes.BonusMana + ""; }
				if ( race.Attributes.RegenMana > 0 ){ txt = txt + "<BR>Mana Regeneration: " + race.Attributes.RegenMana + ""; }
				if ( race.Attributes.NightSight > 0 ){ txt = txt + "<BR>Night Sight"; }
				if ( race.Attributes.ReflectPhysical > 0 ){ txt = txt + "<BR>Reflect Physical Damage: " + race.Attributes.ReflectPhysical + "%"; }
				if ( race.Attributes.SpellDamage > 0 ){ txt = txt + "<BR>Spell Damage Increase: " + race.Attributes.SpellDamage + "%"; }
				if ( race.Attributes.BonusStam > 0 ){ txt = txt + "<BR>Stamina Increase: " + race.Attributes.BonusStam + ""; }
				if ( race.Attributes.RegenStam > 0 ){ txt = txt + "<BR>Stamina Regeneration: " + race.Attributes.RegenStam + ""; }
				if ( race.Attributes.BonusStr > 0 ){ txt = txt + "<BR>Strength Bonus: " + race.Attributes.BonusStr + ""; }
				if ( race.Attributes.WeaponSpeed > 0 ){ txt = txt + "<BR>Swing Speed Increase: " + race.Attributes.WeaponSpeed + "%"; }

				if ( race.SkillBonuses.Skill_1_Value > 0 ){ txt = txt + "<BR>" + SkillInfo.Table[(int)(race.SkillBonuses.Skill_1_Name)].Name + " +" + race.SkillBonuses.Skill_1_Value + ""; }
				if ( race.SkillBonuses.Skill_2_Value > 0 ){ txt = txt + "<BR>" + SkillInfo.Table[(int)(race.SkillBonuses.Skill_2_Name)].Name + " +" + race.SkillBonuses.Skill_2_Value + ""; }

				if ( NoFood( raceID ) ){ txt = txt + "<BR>Does not need to eat, but still needs to drink"; }
				if ( NoFoodOrDrink( raceID ) ){ txt = txt + "<BR>Does not need to eat or drink"; }
				if ( BloodDrinker( raceID ) ){ txt = txt + "<BR>Needs to consume fresh blood"; }
				if ( BrainEater( raceID ) ){ txt = txt + "<BR>Needs to consume fresh brains"; }

				race.Delete();
			}

			return txt;
		}

		public static SkillName RaceSkill( int skill )
		{
			if ( skill == 0 ){ return SkillName.Alchemy; }
			else if ( skill == 1 ){ return SkillName.Anatomy; }
			else if ( skill == 2 ){ return SkillName.Druidism; }
			else if ( skill == 3 ){ return SkillName.Mercantile; }
			else if ( skill == 4 ){ return SkillName.ArmsLore; }
			else if ( skill == 5 ){ return SkillName.Parry; }
			else if ( skill == 6 ){ return SkillName.Begging; }
			else if ( skill == 7 ){ return SkillName.Blacksmith; }
			else if ( skill == 8 ){ return SkillName.Bowcraft; }
			else if ( skill == 9 ){ return SkillName.Peacemaking; }
			else if ( skill == 10 ){ return SkillName.Camping; }
			else if ( skill == 11 ){ return SkillName.Carpentry; }
			else if ( skill == 12 ){ return SkillName.Cartography; }
			else if ( skill == 13 ){ return SkillName.Cooking; }
			else if ( skill == 14 ){ return SkillName.Searching; }
			else if ( skill == 15 ){ return SkillName.Discordance; }
			else if ( skill == 16 ){ return SkillName.Psychology; }
			else if ( skill == 17 ){ return SkillName.Healing; }
			else if ( skill == 18 ){ return SkillName.Seafaring; }
			else if ( skill == 19 ){ return SkillName.Forensics; }
			else if ( skill == 20 ){ return SkillName.Herding; }
			else if ( skill == 21 ){ return SkillName.Hiding; }
			else if ( skill == 22 ){ return SkillName.Provocation; }
			else if ( skill == 23 ){ return SkillName.Inscribe; }
			else if ( skill == 24 ){ return SkillName.Lockpicking; }
			else if ( skill == 25 ){ return SkillName.Magery; }
			else if ( skill == 26 ){ return SkillName.MagicResist; }
			else if ( skill == 27 ){ return SkillName.Tactics; }
			else if ( skill == 28 ){ return SkillName.Snooping; }
			else if ( skill == 29 ){ return SkillName.Musicianship; }
			else if ( skill == 30 ){ return SkillName.Poisoning; }
			else if ( skill == 31 ){ return SkillName.Marksmanship; }
			else if ( skill == 32 ){ return SkillName.Spiritualism; }
			else if ( skill == 33 ){ return SkillName.Stealing; }
			else if ( skill == 34 ){ return SkillName.Tailoring; }
			else if ( skill == 35 ){ return SkillName.Taming; }
			else if ( skill == 36 ){ return SkillName.Tasting; }
			else if ( skill == 37 ){ return SkillName.Tinkering; }
			else if ( skill == 38 ){ return SkillName.Tracking; }
			else if ( skill == 39 ){ return SkillName.Veterinary; }
			else if ( skill == 40 ){ return SkillName.Swords; }
			else if ( skill == 41 ){ return SkillName.Bludgeoning; }
			else if ( skill == 42 ){ return SkillName.Fencing; }
			else if ( skill == 43 ){ return SkillName.FistFighting; }
			else if ( skill == 44 ){ return SkillName.Lumberjacking; }
			else if ( skill == 45 ){ return SkillName.Mining; }
			else if ( skill == 46 ){ return SkillName.Meditation; }
			else if ( skill == 47 ){ return SkillName.Stealth; }
			else if ( skill == 48 ){ return SkillName.RemoveTrap; }
			else if ( skill == 49 ){ return SkillName.Necromancy; }
			else if ( skill == 50 ){ return SkillName.Focus; }
			else if ( skill == 51 ){ return SkillName.Knightship; }
			else if ( skill == 52 ){ return SkillName.Bushido; }
			else if ( skill == 53 ){ return SkillName.Ninjitsu; }
			else if ( skill == 54 ){ return SkillName.Elementalism; }
			else if ( skill == 55 ){ return SkillName.Mysticism; }
			else if ( skill == 56 ){ return SkillName.Imbuing; }
			else if ( skill == 57 ){ return SkillName.Throwing; }

			return SkillName.Alchemy;
		}

		public static string RaceDefined( int val )
		{
			// Name,Index,ItemID,Gump,Body,Icon,x,y,Sound,Species,Alignment,Start,Size,Phy,Fir,Cld,Poi,Eny,Str,Dex,Int,Hits,Stam,Mana,RegHits,RegStam,RegMana,Night,Attack%,Defend%,CastRecover,CastSpd,Potions,LowMana,LowReg,Luck,Reflect,SpellDmg,WepDmg,WepSpeed,Skill1,Skill2,Food,Gender

			string race = "";

			switch ( val )
			{
				case 1: race = "Lurker,1,25754,719,194,2822,66,89,684,aquatic,neutral,sea,1,0,0,1,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,1,0,0,2,0,0,1,1,18,0,0,0"; break;
				case 2: race = "Neptar,2,8387,793,676,2815,139,138,1363,aquatic,neutral,sea,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,2,2,0,0,0,1,1,0,0,0,0,0,18,27,0,0"; break;
				case 3: race = "Neptar,3,8387,793,677,2816,133,134,1363,aquatic,neutral,sea,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,2,2,0,0,0,1,1,0,0,0,0,0,18,27,0,0"; break;
				case 4: race = "Tritun,4,8322,853,678,2733,103,133,1363,aquatic,neutral,sea,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,2,2,0,0,0,1,1,0,0,0,0,0,18,27,0,0"; break;
				case 5: race = "Tritun,5,8322,853,690,2734,125,133,1363,aquatic,neutral,sea,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,2,2,0,0,0,1,1,0,0,0,0,0,18,27,0,0"; break;
				case 6: race = "Bugbear,6,8341,742,343,2773,75,81,427,bugbear,neutral,cave,1,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,1,0,27,4,0,0"; break;
				case 7: race = "Centaur,7,16460,814,101,2672,68,111,679,centaur,good,woods,1,0,0,0,0,0,0,1,0,0,1,0,0,1,0,0,2,0,0,0,0,0,0,1,0,0,0,1,31,27,0,0"; break;
				case 8: race = "Cyclops,8,8343,743,75,2759,116,139,604,cyclops,neutral,sky,2,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,0,14,27,0,0"; break;
				case 9: race = "Cyclops,9,8343,743,475,2776,61,124,604,cyclops,neutral,sky,2,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,0,14,27,0,0"; break;
				case 10: race = "Cyclops,10,16472,827,259,2686,151,177,604,cyclops,neutral,sky,3,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,0,14,27,0,0"; break;
				case 11: race = "Balron,11,8340,747,43,2756,195,181,357,daemon,evil,ice,2,0,0,1,1,0,0,0,1,0,0,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 12: race = "Balron,12,8338,745,38,2753,194,189,357,daemon,evil,pits,2,0,1,0,1,0,0,0,1,0,0,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 13: race = "Balron,13,8339,746,40,2754,194,189,357,daemon,evil,pits,2,0,1,0,1,0,0,0,1,0,0,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 14: race = "Balron,14,8337,744,102,2762,194,181,357,daemon,evil,pits,2,0,1,0,1,0,0,0,1,0,0,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 15: race = "Daemon,15,8363,768,88,2791,231,212,357,daemon,evil,sea,2,0,0,1,1,0,0,0,1,0,0,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 16: race = "Daemon,16,16462,816,765,2674,362,264,357,daemon,evil,pits,2,0,1,0,1,0,0,0,1,0,0,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 17: race = "Daemon,17,8337,744,9,2750,157,184,357,daemon,evil,pits,2,0,1,0,1,0,0,0,1,0,0,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 18: race = "Daemon,18,8337,744,10,2751,189,166,357,daemon,evil,pits,2,0,1,0,1,0,0,0,1,0,0,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 19: race = "Daemon,19,8337,744,320,2771,182,185,357,daemon,evil,pits,2,0,1,0,1,0,0,0,1,0,0,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 20: race = "Daemon,20,8356,760,748,2782,222,225,357,daemon,evil,sea,3,0,0,1,1,0,0,0,1,0,0,1,0,0,1,1,0,0,0,1,0,0,0,0,0,0,0,0,26,18,0,0"; break;
				case 21: race = "Dagon,21,16473,828,764,2687,130,154,353,dagon,evil,sea,2,0,0,1,0,0,0,0,1,0,0,1,0,0,1,1,0,0,1,0,0,0,0,0,0,1,0,0,25,18,0,0"; break;
				case 22: race = "Dagon,22,25755,720,146,2823,129,185,353,dagon,evil,sea,2,0,0,1,0,0,0,0,1,0,0,1,0,0,1,1,0,0,1,0,0,0,0,0,0,1,0,0,25,18,0,0"; break;
				case 23: race = "Devil Kin,23,8383,788,112,2811,57,81,357,demon,evil,pits,1,0,1,0,1,0,0,0,1,0,0,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 24: race = "Devil Kin,24,8384,789,126,2812,73,102,357,demon,evil,pits,1,0,1,0,1,0,0,0,1,0,0,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 25: race = "Shadow Demon,25,8357,761,93,2761,94,130,655,demon,evil,pits,1,0,0,0,1,1,0,0,1,0,0,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,26,100,2,0"; break;
				case 26: race = "Demon,26,8344,748,137,2765,148,158,357,demon,evil,pits,2,0,1,0,1,0,0,0,1,0,0,1,0,0,1,1,2,0,0,0,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 27: race = "Demon,27,8364,767,195,2795,103,148,357,demon,evil,pits,2,0,1,0,1,0,0,0,1,0,0,1,0,0,1,1,2,0,0,0,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 28: race = "Devil,28,8345,749,509,2777,106,153,357,demon,evil,pits,2,0,1,0,1,0,0,0,1,0,0,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 29: race = "Balron,29,8337,744,191,2770,276,283,357,devil,evil,pits,3,0,0,0,1,0,0,0,1,0,0,1,0,0,1,1,2,0,1,0,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 30: race = "Balron,30,8337,744,427,2774,192,256,357,devil,evil,pits,3,0,0,0,1,0,0,0,1,0,0,1,0,0,1,1,2,0,1,0,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 31: race = "Devil,31,8385,790,138,2813,180,193,357,devil,evil,ice,3,0,0,1,1,0,0,0,1,0,0,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 32: race = "Devil,32,8388,791,804,2817,373,325,357,devil,evil,sea,3,0,0,1,1,0,0,0,1,0,0,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,26,100,0,0"; break;
				case 33: race = "Devil,33,8386,792,436,2814,278,244,1200,devil,evil,sea,3,0,0,1,1,0,0,0,1,0,0,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,26,100,0,1"; break;
				case 34: race = "Dragon Ogre,34,8347,750,766,2783,203,170,427,dragon,neutral,cave,2,1,1,0,0,0,1,0,0,0,0,0,1,0,0,1,2,2,0,0,0,0,0,0,0,0,1,0,100,100,0,0"; break;
				case 35: race = "Drakkul,35,16487,847,668,2727,130,111,357,drakkul,neutral,cave,1,1,1,0,0,0,1,0,1,0,0,0,0,0,0,0,2,2,1,1,0,0,0,0,0,0,0,0,100,100,0,0"; break;
				case 36: race = "Drakkul,36,16488,848,669,2728,119,108,357,drakkul,neutral,cave,1,1,1,0,0,0,1,0,1,0,0,0,0,0,0,0,2,2,1,1,0,0,0,0,0,0,0,0,100,100,0,1"; break;
				case 37: race = "Drakkul,37,8346,751,670,2779,181,174,357,drakkul,neutral,cave,2,1,1,0,0,0,1,0,1,0,0,0,0,0,0,0,2,2,1,1,0,0,0,0,0,0,0,0,100,100,0,0"; break;
				case 38: race = "Ent,38,25761,725,301,2829,97,111,442,tree,good,woods,2,1,0,0,1,0,0,0,1,1,0,0,1,0,0,0,0,2,0,0,1,0,0,0,0,0,0,0,2,100,1,0"; break;
				case 39: race = "Ent,39,16489,849,309,2729,198,211,442,tree,good,woods,3,1,0,0,1,0,0,0,1,1,0,0,1,0,0,0,0,2,0,0,1,0,0,0,0,0,0,0,2,100,1,0"; break;
				case 40: race = "Ent,40,16490,850,312,2730,163,192,442,tree,good,woods,3,1,0,0,1,0,0,0,1,1,0,0,1,0,0,0,0,2,0,0,1,0,0,0,0,0,0,0,2,100,1,0"; break;
				case 41: race = "Reaper,41,25760,725,285,2828,121,153,442,tree,neutral,woods,2,1,0,0,1,0,0,0,1,1,0,0,1,0,0,0,0,2,0,0,1,0,0,0,0,0,0,0,2,100,1,0"; break;
				case 42: race = "Reaper,42,8320,851,313,2731,191,240,442,tree,neutral,woods,3,1,0,0,1,0,0,0,1,1,0,0,1,0,0,0,0,2,0,0,1,0,0,0,0,0,0,0,2,100,1,0"; break;
				case 43: race = "Ettin,43,16476,835,89,2691,110,106,367,ettin,neutral,ice,2,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,1,1,27,100,0,0"; break;
				case 44: race = "Ettin,44,16475,834,2,2689,71,126,367,ettin,neutral,cave,2,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,1,1,27,100,0,0"; break;
				case 45: race = "Ettin,45,16475,834,18,2690,96,126,367,ettin,neutral,cave,2,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,1,1,27,100,0,0"; break;
				case 46: race = "Ettin,46,16477,836,729,2692,126,158,367,ettin,neutral,cave,2,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,1,1,27,100,0,0"; break;
				case 47: race = "Ettin,47,16477,836,730,2693,138,159,367,ettin,neutral,cave,2,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,1,1,27,100,0,0"; break;
				case 48: race = "Ettin,48,16478,837,316,2694,122,169,367,ettin,neutral,cave,2,0,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,1,1,0,25,27,0,0"; break;
				case 49: race = "Ettin,49,16471,825,732,2685,143,200,367,ettin,neutral,cave,3,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,1,1,27,100,0,0"; break;
				case 50: race = "Fairy,50,8377,769,128,2792,22,43,1127,fey,good,woods,1,0,0,0,0,1,0,0,1,0,0,1,0,1,1,0,0,0,1,1,0,0,0,1,1,0,0,0,25,100,0,0"; break;
				case 51: race = "Fairy,51,8376,770,356,2802,39,76,1127,fey,good,woods,1,0,0,0,0,1,0,0,1,0,0,1,0,1,1,0,0,0,1,1,0,0,0,1,1,0,0,0,25,100,0,1"; break;
				case 52: race = "Pixie,52,8375,782,363,2804,38,44,1127,fey,neutral,woods,1,0,0,0,0,1,0,1,0,0,1,0,0,1,1,0,0,0,0,0,0,0,0,1,0,0,0,1,28,33,0,0"; break;
				case 53: race = "Astral Gargoyle,53,8348,752,127,2764,150,119,372,gargoyle,neutral,pits,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,1,2,2,1,1,0,1,1,0,0,1,0,0,100,100,0,0"; break;
				case 54: race = "Gargoyle,54,8327,858,257,2739,169,141,372,gargoyle,neutral,pits,2,0,0,0,0,0,0,0,1,0,0,0,0,0,0,1,2,2,1,1,0,1,1,0,0,1,0,0,100,100,0,0"; break;
				case 55: race = "Gargoyle,55,8328,859,4,2740,94,110,372,gargoyle,neutral,pits,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,1,2,2,1,1,0,1,1,0,0,1,0,0,100,100,0,0"; break;
				case 56: race = "Gargoyle,56,8329,860,158,2741,126,125,372,gargoyle,neutral,pits,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,1,2,2,1,1,0,1,1,0,0,1,0,0,100,100,0,1"; break;
				case 57: race = "Abysmal Giant,57,8362,781,772,2809,130,228,609,giant,neutral,pits,3,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,0,27,14,0,0"; break;
				case 58: race = "Cloud Giant,58,8342,766,773,2784,131,201,609,giant,neutral,sky,3,0,0,0,0,1,1,0,1,1,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,1,0,27,25,0,0"; break;
				case 59: race = "Earth Giant,59,16474,833,433,2688,146,163,609,giant,neutral,woods,3,1,0,0,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,1,0,27,45,0,0"; break;
				case 60: race = "Fire Giant,60,8321,852,774,2732,172,191,609,giant,neutral,pits,3,0,1,0,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,1,0,27,7,0,0"; break;
				case 61: race = "Forest Giant,61,8325,856,264,2737,180,197,609,giant,neutral,woods,3,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,1,0,0,0,0,0,1,0,27,44,0,0"; break;
				case 62: race = "Frost Giant,62,8326,857,777,2738,166,184,609,giant,neutral,ice,3,0,0,1,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,1,0,27,10,0,0"; break;
				case 63: race = "Frost Giant,63,8365,771,325,2801,131,201,609,giant,neutral,ice,3,0,0,1,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,1,0,27,10,0,0"; break;
				case 64: race = "Hill Giant,64,8350,754,725,2780,152,165,609,giant,neutral,cave,3,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,1,0,0,0,0,0,1,0,27,10,0,0"; break;
				case 65: race = "Hill Giant,65,8350,754,726,2781,154,178,609,giant,neutral,cave,3,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,1,0,0,0,0,0,1,0,27,10,0,0"; break;
				case 66: race = "Jungle Giant,66,8367,772,771,2808,120,220,609,giant,neutral,swamp,3,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,1,0,0,0,0,0,1,0,27,0,0,0"; break;
				case 67: race = "Sand Giant,67,8331,862,770,2743,125,213,609,giant,neutral,sand,3,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,1,1,27,0,0,0"; break;
				case 68: race = "Sea Giant,68,16470,824,792,2684,178,170,609,giant,neutral,sea,3,0,0,1,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,1,0,27,18,0,0"; break;
				case 69: race = "Stone Giant,69,8380,785,485,2805,165,236,609,giant,neutral,cave,3,1,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,1,0,45,7,0,0"; break;
				case 70: race = "Gnoll,70,8349,753,510,2778,65,97,1269,gnoll,neutral,cave,1,0,0,0,0,0,1,1,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,0,27,100,0,0"; break;
				case 71: race = "Goblin,71,25776,733,592,2675,122,122,422,goblin,neutral,cave,1,0,0,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,2,0,0,0,1,21,47,0,0"; break;
				case 72: race = "Goblin,72,16463,817,632,2676,29,41,422,goblin,neutral,cave,1,0,0,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,2,0,0,0,1,21,47,0,0"; break;
				case 73: race = "Goblin,73,16463,817,647,2677,28,38,422,goblin,neutral,cave,1,0,0,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,0,0,0,0,0,2,0,0,0,1,21,47,0,0"; break;
				case 74: race = "Flesh Golem,74,8324,855,69,2736,140,139,684,golem,evil,tomb,1,0,0,0,1,0,1,0,0,1,0,0,1,0,0,1,2,2,0,0,0,0,0,0,0,0,1,0,19,100,0,0"; break;
				case 75: race = "Flesh Golem,75,8323,854,999,2735,193,169,684,golem,evil,tomb,2,0,0,0,1,0,1,0,0,1,0,0,1,0,0,1,2,2,0,0,0,0,0,0,0,0,1,0,19,100,0,0"; break;
				case 76: race = "Hobgoblin,76,16480,839,11,2696,51,78,1114,hobgoblin,neutral,cave,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,1,27,100,0,0"; break;
				case 77: race = "Mind Flayer,77,8389,794,786,2819,45,75,898,illithid,evil,cave,1,0,0,0,0,1,0,0,1,0,0,0,0,0,0,1,0,0,1,1,0,0,0,0,0,1,0,0,25,49,4,0"; break;
				case 78: race = "Imp,78,16486,846,202,2726,160,160,594,imp,neutral,pits,1,0,1,0,1,0,0,0,1,0,0,1,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,25,26,0,0"; break;
				case 79: race = "Imp,79,25756,721,359,2824,43,46,594,imp,neutral,pits,1,0,1,0,1,0,0,0,1,0,0,1,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,25,26,0,0"; break;
				case 80: race = "Kilrathi,80,8351,755,176,2767,105,113,1006,kilrathi,neutral,woods,1,0,0,0,0,0,0,1,0,0,1,0,0,1,0,1,2,0,0,0,0,0,0,0,0,0,0,1,21,47,0,0"; break;
				case 81: race = "Kobold,81,16481,840,245,2697,46,58,1347,kobold,neutral,cave,1,0,0,0,0,0,0,1,0,0,1,0,0,0,0,1,0,2,0,0,0,0,0,2,0,0,0,1,24,100,0,0"; break;
				case 82: race = "Kobold,82,16481,840,253,2698,59,90,1347,kobold,neutral,cave,1,0,0,0,0,0,0,1,0,0,1,0,0,0,0,1,0,2,0,0,0,0,0,2,0,0,0,1,25,100,0,0"; break;
				case 83: race = "Kobold,83,16481,840,255,2699,48,58,1347,kobold,neutral,cave,1,0,0,0,0,0,0,1,0,0,1,0,0,0,0,1,0,2,0,0,0,0,0,2,0,0,0,1,24,100,0,0"; break;
				case 84: race = "Minotaur,84,8371,774,78,2787,72,91,1358,minotaur,neutral,cave,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,0,14,27,0,0"; break;
				case 85: race = "Minotaur,85,8370,776,263,2797,91,97,1358,minotaur,neutral,cave,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,0,14,27,0,0"; break;
				case 86: race = "Minotaur,86,8370,776,280,2798,97,93,1358,minotaur,neutral,cave,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,0,14,27,0,0"; break;
				case 87: race = "Minotaur,87,8370,776,281,2799,88,93,1358,minotaur,neutral,cave,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,0,14,27,0,0"; break;
				case 88: race = "Minotaur,88,8369,775,357,2803,65,90,1358,minotaur,neutral,cave,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,0,14,27,0,1"; break;
				case 89: race = "Minotaur,89,8368,777,650,2807,113,118,1358,minotaur,neutral,cave,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,0,14,27,0,0"; break;
				case 90: race = "Mummy,90,25757,722,154,2825,48,63,471,mummy,evil,sand,1,0,0,0,1,0,1,0,0,1,0,0,0,1,0,1,0,2,0,0,1,0,0,0,0,0,1,0,27,19,2,0"; break;
				case 91: race = "Mummy,91,25758,723,601,2826,93,112,1149,mummy,evil,sand,2,0,0,0,1,0,1,0,0,1,0,0,0,1,0,1,0,2,0,0,1,0,0,0,0,0,1,0,27,19,2,0"; break;
				case 92: race = "Fungal,92,8390,795,341,2820,66,110,9999,mushroom,neutral,cave,1,0,0,0,1,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,1,0,1,0,1,0,0,0,0,36,1,0"; break;
				case 93: race = "Fungal,93,8391,796,342,2821,71,112,9999,mushroom,neutral,cave,1,0,0,0,1,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,1,0,1,0,1,0,0,0,0,36,1,0"; break;
				case 94: race = "Naga,94,16479,838,261,2695,128,141,634,naga,neutral,pits,1,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,1,1,0,1,1,0,0,1,0,0,25,100,0,0"; break;
				case 95: race = "Naga,95,16465,819,704,2679,125,164,644,naga,neutral,sea,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,0,0,1,1,0,1,1,0,0,1,0,0,25,100,0,1"; break;
				case 96: race = "Naga,96,16482,841,66,2716,142,136,644,naga,neutral,sea,1,0,0,1,0,0,0,0,1,0,0,1,0,0,0,0,0,0,1,1,0,1,1,0,0,1,0,0,25,100,0,1"; break;
				case 97: race = "Ogre,97,16468,822,1,2682,59,103,427,ogre,neutral,cave,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,1,0,0,0,0,0,1,0,27,100,0,0"; break;
				case 98: race = "Ogre,98,16461,815,428,2673,178,174,427,ogre,neutral,cave,2,0,0,0,0,0,1,0,0,1,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,0,27,100,0,0"; break;
				case 99: race = "Ogre,99,16469,823,303,2683,122,155,427,ogre,neutral,cave,2,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,2,2,0,0,1,0,0,0,0,0,1,0,27,100,0,0"; break;
				case 100: race = "Orc,100,8352,756,7,2749,45,56,1114,orc,neutral,cave,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,2,2,0,0,1,0,0,0,0,0,0,0,27,4,0,0"; break;
				case 101: race = "Orc,101,8352,756,17,2752,28,51,1114,orc,neutral,cave,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,2,2,0,0,1,0,0,0,0,0,0,0,27,4,0,0"; break;
				case 102: race = "Orc,102,8352,756,41,2755,43,52,1114,orc,neutral,cave,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,2,2,0,0,1,0,0,0,0,0,0,0,27,4,0,0"; break;
				case 103: race = "Orc,103,8354,757,108,2763,65,105,1114,orc,neutral,cave,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,2,2,0,0,1,0,0,0,0,0,0,0,27,4,0,0"; break;
				case 104: race = "Orc,104,8353,759,182,2768,33,73,1114,orc,neutral,cave,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,2,2,0,0,1,0,0,0,0,0,0,0,27,4,0,0"; break;
				case 105: race = "Orc,105,8355,758,328,2772,63,91,1114,orc,neutral,cave,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,2,2,0,0,1,0,0,0,0,0,0,0,27,4,0,0"; break;
				case 106: race = "Orc,106,8354,757,65,2785,81,129,1114,orc,neutral,cave,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,2,2,0,0,1,0,0,0,0,0,0,0,27,4,0,0"; break;
				case 107: race = "Urk,107,8381,786,20,2786,48,65,1114,orc,neutral,cave,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,2,2,0,0,1,0,0,0,0,0,0,0,27,4,0,0"; break;
				case 108: race = "Urk,108,8382,787,157,2794,58,63,1114,orc,neutral,cave,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,2,2,0,0,1,0,0,0,0,0,0,0,25,46,0,0"; break;
				case 109: race = "Urk,109,8381,786,252,2796,55,65,1114,orc,neutral,cave,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,2,2,0,0,1,0,0,0,0,0,0,0,27,4,0,0"; break;
				case 110: race = "Owlbear,110,8330,861,758,2742,149,171,163,owlbear,neutral,woods,2,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,2,2,0,0,1,0,0,0,0,0,1,1,2,100,0,0"; break;
				case 111: race = "Shambler,111,25759,724,779,2827,78,93,442,plant,evil,swamp,1,0,0,0,1,0,1,0,0,1,0,0,1,0,0,0,0,2,0,0,1,0,0,0,0,0,1,0,0,36,1,0"; break;
				case 112: race = "Swamp Thing,112,16485,845,172,2725,151,151,427,plant,evil,swamp,2,0,0,0,1,0,1,0,0,1,0,0,1,0,0,0,0,2,0,0,1,0,0,0,0,0,1,0,0,36,1,0"; break;
				case 113: race = "Grathek,113,16455,809,534,2669,116,88,417,reptilian,neutral,swamp,1,1,0,0,1,0,1,0,0,0,0,0,1,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,27,100,0,0"; break;
				case 114: race = "Lizardman,114,16456,810,33,2663,72,71,417,reptilian,neutral,swamp,1,1,0,0,0,0,1,0,0,0,0,0,1,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,27,4,0,0"; break;
				case 115: race = "Lizardman,115,16456,810,34,2664,89,73,417,reptilian,neutral,swamp,1,1,0,0,0,0,1,0,0,0,0,0,1,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,27,4,0,0"; break;
				case 116: race = "Lizardman,116,16456,810,35,2665,77,71,417,reptilian,neutral,swamp,1,1,0,0,0,0,1,0,0,0,0,0,1,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,27,4,0,0"; break;
				case 117: race = "Sakkhra,117,16457,811,324,2666,80,87,417,reptilian,neutral,swamp,1,1,0,0,0,0,1,0,0,0,0,0,1,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,27,4,0,0"; break;
				case 118: race = "Sakkhra,118,16457,811,326,2667,80,87,417,reptilian,neutral,swamp,1,1,0,0,0,0,1,0,0,0,0,0,1,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,27,4,0,0"; break;
				case 119: race = "Sakkhra,119,16457,811,333,2668,83,97,417,reptilian,neutral,swamp,1,1,0,0,0,0,1,0,0,0,0,0,1,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,27,4,0,0"; break;
				case 120: race = "Sleestax,120,16458,812,541,2670,131,107,417,reptilian,neutral,swamp,1,1,0,0,0,0,1,0,0,0,0,0,1,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,27,4,0,0"; break;
				case 121: race = "Revenant,121,16464,818,768,2678,144,124,1149,revenant,evil,tomb,2,0,0,0,1,0,1,0,1,0,0,1,0,0,0,1,2,0,0,0,0,0,0,0,0,0,0,0,27,49,2,0"; break;
				case 122: race = "Ratman,122,16483,842,42,2717,49,59,437,rodent,neutral,cave,1,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,0,1,28,33,0,0"; break;
				case 123: race = "Ratman,123,16483,842,44,2718,52,66,437,rodent,neutral,cave,1,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,0,1,28,33,0,0"; break;
				case 124: race = "Ratman,124,16483,842,45,2719,54,66,437,rodent,neutral,cave,1,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,0,1,28,33,0,0"; break;
				case 125: race = "Ratman,125,16483,842,73,2720,135,120,437,rodent,neutral,cave,1,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,0,1,25,46,0,0"; break;
				case 126: race = "Ratman,126,16483,842,163,2721,100,79,437,rodent,neutral,cave,1,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,0,1,28,33,0,0"; break;
				case 127: race = "Ratman,127,16483,842,164,2722,71,66,437,rodent,neutral,cave,1,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,0,1,28,33,0,0"; break;
				case 128: race = "Ratman,128,16483,842,165,2723,100,79,437,rodent,neutral,cave,1,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,0,1,28,33,0,0"; break;
				case 129: race = "Salamander,129,16484,844,673,2724,121,172,634,salamander,neutral,pits,1,0,1,0,0,0,0,0,1,0,0,0,0,0,0,1,2,2,0,0,0,1,1,0,0,0,0,1,100,100,0,0"; break;
				case 130: race = "Satyr,130,16459,813,271,2671,93,143,1414,satyr,good,woods,1,0,0,0,0,0,0,1,1,0,0,1,0,0,0,0,0,0,1,0,0,1,1,0,0,0,0,0,29,9,0,0"; break;
				case 131: race = "Ophidian,131,8374,780,86,2789,142,105,634,serpent,neutral,swamp,1,0,0,0,1,0,0,1,1,0,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,1,30,100,0,0"; break;
				case 132: race = "Ophidian,132,8373,779,85,2788,96,119,644,serpent,neutral,swamp,1,0,0,0,1,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,30,25,0,1"; break;
				case 133: race = "Ophidian,133,8373,779,87,2790,87,74,644,serpent,neutral,swamp,1,0,0,0,1,0,0,1,1,0,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,1,30,100,0,1"; break;
				case 134: race = "Serpyn,134,8332,863,306,2744,119,100,219,serpent,neutral,swamp,1,0,0,0,1,0,0,1,1,0,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,1,30,100,0,0"; break;
				case 135: race = "Serpyn,135,16466,820,145,2680,123,150,634,serpent,neutral,sea,1,0,0,0,1,0,0,1,1,0,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,30,18,0,0"; break;
				case 136: race = "Serpyn,136,16467,821,143,2681,115,155,634,serpent,neutral,sea,1,0,0,0,1,0,0,1,1,0,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,30,18,0,0"; break;
				case 137: race = "Serpyn,137,8372,778,144,2793,195,218,644,serpent,neutral,sea,2,0,0,0,1,0,0,1,1,0,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,30,18,0,1"; break;
				case 138: race = "Skeleton,138,25762,726,50,2830,17,64,1165,skeleton,evil,tomb,1,0,0,0,1,0,0,1,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,1,27,100,2,0"; break;
				case 139: race = "Skeleton,139,25763,726,56,2831,31,62,1165,skeleton,evil,tomb,1,0,0,0,1,0,0,1,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,1,27,100,2,0"; break;
				case 140: race = "Skeleton,140,25764,726,57,2832,49,64,1165,skeleton,evil,tomb,1,0,0,0,1,0,0,1,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,1,27,100,2,0"; break;
				case 141: race = "Skeleton,141,25765,727,110,2833,74,102,1001,skeleton,evil,tomb,1,0,0,0,0,1,0,0,1,0,0,0,0,0,0,1,0,0,1,1,0,0,0,0,0,1,0,0,25,49,2,0"; break;
				case 142: race = "Skeleton,142,25766,728,148,2834,44,72,1001,skeleton,evil,tomb,1,0,0,0,0,1,0,0,1,0,0,0,0,0,0,1,0,0,1,1,0,0,0,0,0,1,0,0,25,49,2,0"; break;
				case 143: race = "Skeleton,143,25767,726,167,2835,37,72,1165,skeleton,evil,tomb,1,0,0,0,1,0,0,1,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,1,27,100,2,0"; break;
				case 144: race = "Skeleton,144,25768,726,168,2836,64,72,1165,skeleton,evil,tomb,1,0,0,0,1,0,0,1,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,1,27,100,2,0"; break;
				case 145: race = "Skeleton,145,25769,726,170,2837,70,71,1165,skeleton,evil,tomb,1,0,0,0,1,0,0,1,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,1,27,100,2,0"; break;
				case 146: race = "Skeleton,146,25770,726,247,2838,59,74,1165,skeleton,evil,tomb,1,0,0,0,1,0,0,1,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,1,27,100,2,0"; break;
				case 147: race = "Skeleton,147,25771,726,699,2839,33,67,1165,skeleton,evil,tomb,1,0,0,0,1,0,0,1,0,0,0,0,0,0,0,1,2,2,0,0,0,0,0,0,0,0,1,1,27,100,2,0"; break;
				case 148: race = "Skeleton,148,25772,729,724,2840,54,91,412,skeleton,evil,tomb,1,0,0,0,0,1,0,0,1,0,0,0,0,0,1,1,0,0,1,1,0,0,0,0,0,1,0,0,25,49,2,0"; break;
				case 149: race = "Skeleton,149,25773,730,24,2841,42,73,412,skeleton,evil,tomb,1,0,0,0,1,0,0,0,1,0,0,0,0,0,1,1,0,0,1,1,0,0,0,0,0,1,0,0,25,49,2,0"; break;
				case 150: race = "Sphinx,150,8379,784,314,2800,128,157,1640,sphinx,neutral,sand,2,0,0,0,1,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,1,1,0,0,1,0,0,25,26,0,0"; break;
				case 151: race = "Sphinx,151,8378,783,808,2810,148,172,1640,sphinx,neutral,sand,1,0,0,0,0,1,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,1,1,0,0,1,0,0,25,26,0,0"; break;
				case 152: race = "Succubus,152,8333,864,689,2745,95,140,1200,succubus,evil,pits,1,0,1,0,1,0,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,26,27,0,1"; break;
				case 153: race = "Succubus,153,8334,867,149,2746,71,104,1200,succubus,evil,pits,1,0,1,0,1,0,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,26,27,0,1"; break;
				case 154: race = "Succubus,154,8358,762,174,2766,80,135,1200,succubus,evil,pits,2,0,1,0,1,0,0,0,1,0,0,1,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,26,27,0,1"; break;
				case 155: race = "Titan,155,8359,763,76,2760,48,123,609,titan,neutral,sky,2,0,0,0,0,1,1,0,1,1,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,1,1,0,25,100,0,0"; break;
				case 156: race = "Titan,156,8360,764,189,2769,84,216,609,titan,neutral,sky,3,0,0,0,0,1,1,0,1,1,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,1,1,0,25,100,0,0"; break;
				case 157: race = "Troll,157,8335,868,156,2747,107,138,461,troll,neutral,woods,1,0,0,0,0,0,1,0,0,1,0,0,1,0,0,1,2,2,0,0,0,0,0,0,0,0,1,1,100,100,0,0"; break;
				case 158: race = "Troll,158,8366,773,499,2806,57,92,461,troll,neutral,ice,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,2,2,0,0,0,0,0,0,0,0,1,0,100,100,0,0"; break;
				case 159: race = "Troll,159,8361,765,53,2757,68,91,461,troll,neutral,cave,1,0,0,0,0,0,1,0,0,1,0,0,1,0,0,1,2,2,0,0,0,0,0,0,0,0,1,1,100,100,0,0"; break;
				case 160: race = "Troll,160,8361,765,54,2758,53,92,461,troll,neutral,cave,1,0,0,0,0,0,1,0,0,1,0,0,1,0,0,1,2,2,0,0,0,0,0,0,0,0,1,1,100,100,0,0"; break;
				case 161: race = "Troll,161,8361,765,439,2775,78,88,461,troll,neutral,cave,1,0,0,0,0,0,1,0,0,1,0,0,1,0,0,1,2,2,0,0,0,0,0,0,0,0,1,1,100,100,0,0"; break;
				case 162: race = "Troll,162,8336,741,95,2748,111,139,461,troll,neutral,ice,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,2,2,0,0,0,0,0,0,0,0,1,0,100,100,0,0"; break;
				case 163: race = "Vampyre,163,25774,731,124,2842,30,66,1149,vampyre,evil,tomb,1,0,0,0,1,0,0,0,1,0,0,0,0,0,1,1,0,0,0,0,0,1,0,0,1,1,0,0,25,49,3,0"; break;
				case 164: race = "Vampyre,164,25775,732,125,2843,49,106,1149,vampyre,evil,tomb,1,0,0,0,1,0,0,0,1,0,0,0,0,0,1,1,0,0,0,0,0,1,0,0,1,1,0,0,25,49,3,0"; break;
				case 165: race = "Vampyre,165,25777,734,311,2844,61,97,1149,vampyre,evil,tomb,2,0,0,0,1,0,1,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,1,1,27,49,3,0"; break;
				case 166: race = "Zombi,166,25778,735,3,2845,23,63,471,zombi,evil,tomb,1,0,0,0,1,0,0,1,0,1,0,0,0,1,0,1,0,2,0,0,0,0,0,0,0,0,1,0,1,30,4,0"; break;
				case 167: race = "Ghoul,167,25779,736,181,2846,42,67,471,zombi,evil,tomb,1,0,0,0,1,0,0,1,0,1,0,0,0,1,0,1,0,2,0,0,0,0,0,0,0,0,1,0,19,30,4,0"; break;
				case 168: race = "Zombi,168,25780,737,304,2847,31,78,471,zombi,evil,sea,1,0,0,0,1,0,0,1,0,1,0,0,0,1,0,1,0,2,0,0,0,0,0,0,0,0,1,0,18,30,4,0"; break;
				case 169: race = "Zombi,169,25781,738,305,2848,37,80,471,zombi,evil,tomb,1,0,0,0,1,0,0,1,0,1,0,0,0,1,0,1,0,2,0,0,0,0,0,0,0,0,1,0,27,30,4,0"; break;
				case 170: race = "Wight,170,25782,739,307,2849,47,61,471,zombi,evil,tomb,1,0,0,0,1,0,0,1,0,1,0,0,0,1,0,1,0,2,0,0,0,0,0,0,0,0,1,0,19,32,4,0"; break;
				case 171: race = "Zombi,171,25778,735,728,2850,33,76,471,zombi,evil,tomb,1,0,0,0,1,0,0,1,0,1,0,0,0,1,0,1,0,2,0,0,0,0,0,0,0,0,1,0,1,30,4,0"; break;
				case 172: race = "Zombi,172,25783,797,810,2851,39,73,471,zombi,evil,tomb,1,0,0,0,1,0,0,0,1,0,0,1,0,0,1,1,0,0,0,0,0,0,1,0,0,1,0,0,49,30,4,0"; break;
			}

			return race;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			m_AosSkillBonuses.GetProperties( list );

			int prop;

			if ( (prop = m_AosAttributes.WeaponDamage) != 0 )
				list.Add( 1060401, prop.ToString() ); // damage increase ~1_val~%

			if ( (prop = m_AosAttributes.DefendChance) != 0 )
				list.Add( 1060408, prop.ToString() ); // defense chance increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusDex) != 0 )
				list.Add( 1060409, prop.ToString() ); // dexterity bonus ~1_val~

			if ( (prop = m_AosAttributes.EnhancePotions) != 0 )
				list.Add( 1060411, prop.ToString() ); // enhance potions ~1_val~%

			if ( (prop = m_AosAttributes.CastRecovery) != 0 )
				list.Add( 1060412, prop.ToString() ); // faster cast recovery ~1_val~

			if ( (prop = m_AosAttributes.CastSpeed) != 0 )
				list.Add( 1060413, prop.ToString() ); // faster casting ~1_val~

			if ( (prop = m_AosAttributes.AttackChance) != 0 )
				list.Add( 1060415, prop.ToString() ); // hit chance increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusHits) != 0 )
				list.Add( 1060431, prop.ToString() ); // hit point increase ~1_val~

			if ( (prop = m_AosAttributes.BonusInt) != 0 )
				list.Add( 1060432, prop.ToString() ); // intelligence bonus ~1_val~

			if ( (prop = m_AosAttributes.LowerManaCost) != 0 && MyServerSettings.LowerMana() > 0 )
			{
				if ( prop > MyServerSettings.LowerMana() ){ prop = MyServerSettings.LowerMana(); }
				list.Add( 1060433, prop.ToString() ); // lower mana cost ~1_val~%
			}

			if ( (prop = m_AosAttributes.LowerRegCost) != 0 && MyServerSettings.LowerReg() > 0 )
			{
				if ( prop > MyServerSettings.LowerReg() ){ prop = MyServerSettings.LowerReg(); }
				list.Add( 1060434, prop.ToString() ); // lower reagent cost ~1_val~%
			}

			if ( (prop = m_AosAttributes.Luck) != 0 )
				list.Add( 1060436, prop.ToString() ); // luck ~1_val~

			if ( (prop = m_AosAttributes.BonusMana) != 0 )
				list.Add( 1060439, prop.ToString() ); // mana increase ~1_val~

			if ( (prop = m_AosAttributes.RegenMana) != 0 )
				list.Add( 1060440, prop.ToString() ); // mana regeneration ~1_val~

			if ( (prop = m_AosAttributes.NightSight) != 0 )
				list.Add( 1060441 ); // night sight

			if ( (prop = m_AosAttributes.ReflectPhysical) != 0 )
				list.Add( 1060442, prop.ToString() ); // reflect physical damage ~1_val~%

			if ( (prop = m_AosAttributes.RegenStam) != 0 )
				list.Add( 1060443, prop.ToString() ); // stamina regeneration ~1_val~

			if ( (prop = m_AosAttributes.RegenHits) != 0 )
				list.Add( 1060444, prop.ToString() ); // hit point regeneration ~1_val~

			if ( (prop = m_AosAttributes.SpellChanneling) != 0 )
				list.Add( 1060482 ); // spell channeling

			if ( (prop = m_AosAttributes.SpellDamage) != 0 )
				list.Add( 1060483, prop.ToString() ); // spell damage increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusStam) != 0 )
				list.Add( 1060484, prop.ToString() ); // stamina increase ~1_val~

			if ( (prop = m_AosAttributes.BonusStr) != 0 )
				list.Add( 1060485, prop.ToString() ); // strength bonus ~1_val~

			if ( (prop = m_AosAttributes.WeaponSpeed) != 0 )
				list.Add( 1060486, prop.ToString() ); // swing speed increase ~1_val~%

			base.AddResistanceProperties( list );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
			m_AosAttributes.Serialize( writer );
			m_AosResistances.Serialize( writer );
			m_AosSkillBonuses.Serialize( writer );
			writer.Write( (Mobile)Owner);
			writer.Write( SpeciesIndex );
			writer.Write( SpeciesID );
			writer.Write( SpeciesGump );
			writer.Write( SpeciesIcon );
			writer.Write( SpeciesWide );
			writer.Write( SpeciesHigh );
			writer.Write( SpeciesFamily );
			writer.Write( SpeciesAlignment );
			writer.Write( SpeciesStart );
			writer.Write( SpeciesSize );
			writer.Write( SpeciesAngerSound );
			writer.Write( SpeciesIdleSound );
			writer.Write( SpeciesDeathSound );
			writer.Write( SpeciesAttackSound );
			writer.Write( SpeciesHurtSound );
			writer.Write( SpeciesLevel );
			writer.Write( SpeciesFood );
			writer.Write( SpeciesFemale );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_AosAttributes = new AosAttributes( this, reader );
			m_AosResistances = new AosElementAttributes( this, reader );
			m_AosSkillBonuses = new AosSkillBonuses( this, reader );

			if ( Parent is Mobile )
				m_AosSkillBonuses.AddTo( (Mobile)Parent );

			int strBonus = m_AosAttributes.BonusStr;
			int dexBonus = m_AosAttributes.BonusDex;
			int intBonus = m_AosAttributes.BonusInt;

			if ( Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0) )
			{
				Mobile m = (Mobile)Parent;

				string modName = Serial.ToString();

				if ( strBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

				if ( dexBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

				if ( intBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
			}

			if ( Parent is Mobile )
				((Mobile)Parent).CheckStatTimers();

			Owner = reader.ReadMobile();
			SpeciesIndex = reader.ReadInt();
			SpeciesID = reader.ReadInt(); if ( SpeciesID == 1031 ){ SpeciesID = 185; }
			SpeciesGump = reader.ReadInt();
			SpeciesIcon = reader.ReadInt();
			SpeciesWide = reader.ReadInt();
			SpeciesHigh = reader.ReadInt();
			SpeciesFamily = reader.ReadString();
			SpeciesAlignment = reader.ReadString();
			SpeciesStart = reader.ReadString();
			SpeciesSize = reader.ReadInt();
			SpeciesAngerSound = reader.ReadInt();
			SpeciesIdleSound = reader.ReadInt();
			SpeciesDeathSound = reader.ReadInt();
			SpeciesAttackSound = reader.ReadInt();
			SpeciesHurtSound = reader.ReadInt();
			SpeciesLevel = reader.ReadInt();
			SpeciesFood = reader.ReadInt();
			SpeciesFemale = reader.ReadInt();

			Layer = Layer.Special;

			if ( !MyServerSettings.MonstersAllowed() ){ Timer.DelayCall( TimeSpan.FromSeconds( 10.0 ), new TimerCallback( Delete ) ); }
		}

		public override void OnAdded( object parent )
		{
			Mobile mob = parent as Mobile;

			if ( mob != null )
			{
				m_AosSkillBonuses.AddTo( mob );
				AddStatBonuses( mob );
				mob.CheckStatTimers();
			}

			base.OnAdded( parent );
		}

		public override void OnRemoved( object parent )
		{
			Mobile mob = parent as Mobile;

			if ( mob != null )
			{
				if ( Core.AOS )
					m_AosSkillBonuses.Remove();

				string modName = this.Serial.ToString();

				mob.RemoveStatMod( modName + "Str" );
				mob.RemoveStatMod( modName + "Dex" );
				mob.RemoveStatMod( modName + "Int" );

				mob.CheckStatTimers();

				mob.BodyMod = 0;
				mob.HueMod = -1;
				mob.NameMod = null;
			}

			base.OnRemoved( parent );
		}

		public int ComputeStatBonus( StatType type )
		{
			if ( type == StatType.Str )
				return Attributes.BonusStr;
			else if ( type == StatType.Dex )
				return Attributes.BonusDex;
			else
				return Attributes.BonusInt;
		}

		public virtual void AddStatBonuses( Mobile parent )
		{
			if ( parent == null )
				return;

			int strBonus = ComputeStatBonus( StatType.Str );
			int dexBonus = ComputeStatBonus( StatType.Dex );
			int intBonus = ComputeStatBonus( StatType.Int );

			if ( strBonus == 0 && dexBonus == 0 && intBonus == 0 )
				return;

			string modName = this.Serial.ToString();

			if ( strBonus != 0 )
				parent.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

			if ( dexBonus != 0 )
				parent.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

			if ( intBonus != 0 )
				parent.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
		}

		public override bool OnEquip( Mobile m )
		{
			if ( Owner == null )
				Owner = m;

      		if( base.OnEquip( m ) )
      		{
				if ( m.RaceID == 0 )
					m.RaceWasFemale = m.Female;

				m.BodyMod = SpeciesID;
				m.HueMod = 0;
				m.RaceID = SpeciesID;
				m.RaceAngerSound = SpeciesAngerSound;
				m.RaceIdleSound = SpeciesIdleSound;
				m.RaceDeathSound = SpeciesDeathSound;
				m.RaceAttackSound = SpeciesAttackSound;
				m.RaceHurtSound = SpeciesHurtSound;
				if ( SpeciesFemale == 1 )
					m.Female = true;
				else
					m.Female = false;

				Mobiles.IMount mt = m.Mount;
				if ( mt != null )
				{
					Server.Mobiles.EtherealMount.EthyDismount( m );
					mt.Rider = null;
				}
      		}
			return base.OnEquip( m );
		}

		public override bool DisplayLootType{ get{ return false; } }
		public override int Hue { get { return 0; } }

		private AosAttributes m_AosAttributes;
		private AosElementAttributes m_AosResistances;
		private AosSkillBonuses m_AosSkillBonuses;

		[CommandProperty( AccessLevel.Player )]
		public AosAttributes Attributes
		{
			get{ return m_AosAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosElementAttributes Resistances
		{
			get{ return m_AosResistances; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosSkillBonuses SkillBonuses
		{
			get{ return m_AosSkillBonuses; }
			set{}
		}

		public override int PhysicalResistance{ get{ return m_AosResistances.Physical; } }
		public override int FireResistance{ get{ return m_AosResistances.Fire; } }
		public override int ColdResistance{ get{ return m_AosResistances.Cold; } }
		public override int PoisonResistance{ get{ return m_AosResistances.Poison; } }
		public override int EnergyResistance{ get{ return m_AosResistances.Energy; } }

		public override void OnAfterDuped( Item newItem )
		{
			BaseRace race = newItem as BaseRace;

			if ( race == null )
				return;

			race.m_AosAttributes = new AosAttributes( newItem, m_AosAttributes );
			race.m_AosResistances = new AosElementAttributes( newItem, m_AosResistances );
			race.m_AosSkillBonuses = new AosSkillBonuses( newItem, m_AosSkillBonuses );
		}

		public Mobile m_Owner;
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner { get{ return m_Owner; } set{ m_Owner = value; } }

		public int m_SpeciesIndex;
		[CommandProperty( AccessLevel.GameMaster )]
		public int SpeciesIndex { get{ return m_SpeciesIndex; } set{ m_SpeciesIndex = value; } }

		public int m_SpeciesID;
		[CommandProperty( AccessLevel.GameMaster )]
		public int SpeciesID { get{ return m_SpeciesID; } set{ m_SpeciesID = value; } }

		public int m_SpeciesGump;
		[CommandProperty( AccessLevel.GameMaster )]
		public int SpeciesGump { get{ return m_SpeciesGump; } set{ m_SpeciesGump = value; } }

		public int m_SpeciesIcon;
		[CommandProperty( AccessLevel.GameMaster )]
		public int SpeciesIcon { get{ return m_SpeciesIcon; } set{ m_SpeciesIcon = value; } }

		public int m_SpeciesWide;
		[CommandProperty( AccessLevel.GameMaster )]
		public int SpeciesWide { get{ return m_SpeciesWide; } set{ m_SpeciesWide = value; } }

		public int m_SpeciesHigh;
		[CommandProperty( AccessLevel.GameMaster )]
		public int SpeciesHigh { get{ return m_SpeciesHigh; } set{ m_SpeciesHigh = value; } }

		public string m_SpeciesFamily;
		[CommandProperty( AccessLevel.GameMaster )]
		public string SpeciesFamily { get{ return m_SpeciesFamily; } set{ m_SpeciesFamily = value; } }

		public string m_SpeciesAlignment;
		[CommandProperty( AccessLevel.GameMaster )]
		public string SpeciesAlignment { get{ return m_SpeciesAlignment; } set{ m_SpeciesAlignment = value; } }

		public string m_SpeciesStart;
		[CommandProperty( AccessLevel.GameMaster )]
		public string SpeciesStart { get{ return m_SpeciesStart; } set{ m_SpeciesStart = value; } }

		public int m_SpeciesSize;
		[CommandProperty( AccessLevel.GameMaster )]
		public int SpeciesSize { get{ return m_SpeciesSize; } set{ m_SpeciesSize = value; } }

		public int m_SpeciesAngerSound;
		[CommandProperty( AccessLevel.GameMaster )]
		public int SpeciesAngerSound { get{ return m_SpeciesAngerSound; } set{ m_SpeciesAngerSound = value; } }

		public int m_SpeciesIdleSound;
		[CommandProperty( AccessLevel.GameMaster )]
		public int SpeciesIdleSound { get{ return m_SpeciesIdleSound; } set{ m_SpeciesIdleSound = value; } }

		public int m_SpeciesDeathSound;
		[CommandProperty( AccessLevel.GameMaster )]
		public int SpeciesDeathSound { get{ return m_SpeciesDeathSound; } set{ m_SpeciesDeathSound = value; } }

		public int m_SpeciesAttackSound;
		[CommandProperty( AccessLevel.GameMaster )]
		public int SpeciesAttackSound { get{ return m_SpeciesAttackSound; } set{ m_SpeciesAttackSound = value; } }

		public int m_SpeciesHurtSound;
		[CommandProperty( AccessLevel.GameMaster )]
		public int SpeciesHurtSound { get{ return m_SpeciesHurtSound; } set{ m_SpeciesHurtSound = value; } }

		public int m_SpeciesLevel;
		[CommandProperty( AccessLevel.GameMaster )]
		public int SpeciesLevel { get{ return m_SpeciesLevel; } set{ m_SpeciesLevel = value; } }

		public int m_SpeciesFood;
		[CommandProperty( AccessLevel.GameMaster )]
		public int SpeciesFood { get{ return m_SpeciesFood; } set{ m_SpeciesFood = value; } }

		public int m_SpeciesFemale;
		[CommandProperty( AccessLevel.GameMaster )]
		public int SpeciesFemale { get{ return m_SpeciesFemale; } set{ m_SpeciesFemale = value; } }
	}
}