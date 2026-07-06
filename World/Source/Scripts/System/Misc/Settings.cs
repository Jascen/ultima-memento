using System;
using Server.Misc;
using Server.Mobiles;

namespace Server
{
    class MyServerSettings
    {
		public const int DEFAULT_SKILL_COUNT = 10;
		public const int MAX_BONUS_SKILL_COUNT = 10;

		public static void UpdateWarning()
		{
			if ( Utility.DateUpdated() != 20240922 )
				Console.WriteLine( "Warning: Your World.exe requires an update!" );
		}

		public static double ServerSaveMinutes() // HOW MANY MINUTES BETWEEN AUTOMATIC SERVER SAVES
		{
			if ( MySettings.S_ServerSaveMinutes > 240 ){ MySettings.S_ServerSaveMinutes = 240.0; }
			else if ( MySettings.S_ServerSaveMinutes < 10 ){ MySettings.S_ServerSaveMinutes = 10.0; }

			return MySettings.S_ServerSaveMinutes;
		}

		public static int FloorTrapTrigger()
		{
			// THERE ARE MANY HIDDEN TRAPS ON THE FLOOR, BUT THE PERCENT CHANCE
			// IS SET BELOW THAT THEY WILL TRIGGER WHEN WALKED OVER BY PLAYERS
			// 20% IS THE DEFAULT...WHERE 0 IS NEVER AND 100 IS ALWAYS

			if ( MySettings.S_FloorTrapTrigger < 5 ){ MySettings.S_FloorTrapTrigger = 5; }

			return MySettings.S_FloorTrapTrigger;
		}

		public static int GetUnidentifiedChance()
		{
			// CHANCE THAT ITEMS ARE UNIDENTIFIED
			// IF YOU SET THIS VERY LOW, THEN MERCANTILE STARTS TO BECOME A USELESS SKILL

			if ( MySettings.S_GetUnidentifiedChance < 10 ){ MySettings.S_GetUnidentifiedChance = 10; }

			return MySettings.S_GetUnidentifiedChance;
		}

		public static bool NoMacroing()
		{
			// SOME SKILLS ARE MEANT TO BE WORKED ACTIVELY BY THE PLAYER
			// THIS SETS THE TONE FOR GAME DIFFICULTY AND CHARACTER DEVELOPMENT
			// SETTING THE BELOW TO FALSE WILL IGNORE THIS FEATURE OF THE GAME

			if ( Resources() > 1 )
				return false;

			if ( TrainMulti() > 1 )
				return false;

			if ( MySettings.S_CraftMany )
				return false;

			return MySettings.S_NoMacroing;
		}

		public static bool AlwaysUnderAlacrity()
		{
			// GAIN AN ADDITIONAL +0.2 TO +0.5 EVERY TIME YOU GAIN
			return false;
		}

		public static double StatGain()
		{
			// THIS IS NOT ADVISED, BUT YOU CAN INCREASE THE CHANCE OF A STAT GAIN TO OCCUR
			// STATS ONLY GAIN WHEN SKILLS ARE USED, SO A SKILL GAIN POTENTIAL MUST PRECEDE A STAT GAIN

			if ( MySettings.S_StatGain > 50 ){ MySettings.S_StatGain = 50.0; } else if ( MySettings.S_StatGain < 10 ){ MySettings.S_StatGain = 10.0; }

			return MySettings.S_StatGain; // LOWER THIS VALUE FOR MORE STAT GAIN - 33.3 IS DEFAULT - 0.01 IS VERY OFTEN
		}

		public static TimeSpan StatGainDelay()
		{
			// THIS IS NOT ADVISED, BUT YOU CAN CHANGE THE TIME BETWEEN STAT GAINS
			// HOW MANY MINUTES BETWEEN STAT GAINS

			if ( MySettings.S_StatGainDelay > 60 ){ MySettings.S_StatGainDelay = 60.0; } else if ( MySettings.S_StatGainDelay < 5 ){ MySettings.S_StatGainDelay = 5.0; }

			return TimeSpan.FromMinutes( MySettings.S_StatGainDelay ); // 15.0 IS DEFAULT
		}

		public static TimeSpan PetStatGainDelay()
		{
			// THIS IS NOT ADVISED, BUT YOU CAN CHANGE THE TIME BETWEEN STAT GAINS FOR PETS
			// HOW MANY MINUTES BETWEEN STAT GAINS

			if ( MySettings.S_PetStatGainDelay > 60 ){ MySettings.S_PetStatGainDelay = 60.0; } else if ( MySettings.S_PetStatGainDelay < 1 ){ MySettings.S_PetStatGainDelay = 1.0; }

			return TimeSpan.FromMinutes( MySettings.S_PetStatGainDelay ); // 5.0 IS DEFAULT
		}

		public static int GetTimeBetweenQuests()
		{
			if ( MySettings.S_GetTimeBetweenQuests > 240 ){ MySettings.S_GetTimeBetweenQuests = 240; } else if ( MySettings.S_GetTimeBetweenQuests < 1 ){ MySettings.S_GetTimeBetweenQuests = 0; }

			return MySettings.S_GetTimeBetweenQuests; // MINUTES
		}

		public static int GetTimeBetweenArtifactQuests()
		{
			if ( MySettings.S_GetTimeBetweenArtifactQuests > 20160 ){ MySettings.S_GetTimeBetweenArtifactQuests = 20160; } else if ( MySettings.S_GetTimeBetweenArtifactQuests < 1 ){ MySettings.S_GetTimeBetweenArtifactQuests = 0; }

			return MySettings.S_GetTimeBetweenArtifactQuests; // MINUTES
		}

		public static int GetGoldCutRate() // DEFAULT IS 25% OF WHAT GOLD NORMALLY DROPS
		{
			// THIS AFFECTS MONEY ELEMENTS SUCH AS...
			// MONSTER DROPS
			// CHEST DROPS
			// CARGO
			// MUSEUM SEARCHES
			// SHOPPE PROFITS
			// SOME QUESTS

			if ( MySettings.S_GetGoldCutRate < 5 ){ MySettings.S_GetGoldCutRate = 5; } else if ( MySettings.S_GetGoldCutRate > 100 ){ MySettings.S_GetGoldCutRate = 100; }

			return MySettings.S_GetGoldCutRate;
		}

		public static double DamageToPets()
		{
			// IF YOU THINK TAMER PETS SOMEHOW RUIN YOUR GAME, YOU CAN INCREASE THIS VALUE
			// AS IT WILL INCREASE A CREATURES DAMAGE TOWARD SUCH PETS AND IT ONLY ALTERS MELEE DAMAGE 

			if ( MySettings.S_DamageToPets < 1 ){ MySettings.S_DamageToPets = 1.0; }

			return MySettings.S_DamageToPets; // DEFAULT 1.0
		}

		public static int CriticalToPets()
		{
			// IF YOU THINK TAMER PETS SOMEHOW RUIN YOUR GAME, YOU CAN INCREASE THIS VALUE
			// AS IT WILL INCREASE A CREATURES CHANCE OF DOING DOUBLE MELEE DAMAGE TO PETS

			if ( MySettings.S_CriticalToPets < 1 ){ MySettings.S_CriticalToPets = 0; } else if ( MySettings.S_CriticalToPets > 100 ){ MySettings.S_CriticalToPets = 100; }

			return MySettings.S_CriticalToPets; // DEFAULT 0
		}

		public static int SpellDamageIncreaseVsMonsters()
		{
			if ( MySettings.S_SpellDamageIncreaseVsMonsters < 25 ){ MySettings.S_SpellDamageIncreaseVsMonsters = 25; } else if ( MySettings.S_SpellDamageIncreaseVsMonsters > 200 ){ MySettings.S_SpellDamageIncreaseVsMonsters = 200; }
			return MySettings.S_SpellDamageIncreaseVsMonsters;
		}

		public static int SpellDamageIncreaseVsPlayers()
		{
			if ( MySettings.S_SpellDamageIncreaseVsPlayers < 25 ){ MySettings.S_SpellDamageIncreaseVsPlayers = 25; } else if ( MySettings.S_SpellDamageIncreaseVsPlayers > 200 ){ MySettings.S_SpellDamageIncreaseVsPlayers = 200; }
			return MySettings.S_SpellDamageIncreaseVsPlayers;
		}

		public static int QuestRewardModifier()
		{
			// FOR ASSSASSIN, THIEF, FISHING, & STANDARD QUESTS
			// 100 PERCENT IS STANDARD

			if ( MySettings.S_QuestRewardModifier < 0 ){ MySettings.S_QuestRewardModifier = 0; } else if ( MySettings.S_QuestRewardModifier > 250 ){ MySettings.S_QuestRewardModifier = 250; }

			return MySettings.S_QuestRewardModifier; // PERCENT
		}

		public static int PlayerLevelMod( int value, Mobile m )
		{
			if ( MySettings.S_PlayerLevelMod > 3 ){ MySettings.S_PlayerLevelMod = 3.0; } else if ( MySettings.S_PlayerLevelMod < 0.5 ){ MySettings.S_PlayerLevelMod = 0.5; }

			double mod = 1.0;
				if ( m is PlayerMobile ){ mod = MySettings.S_PlayerLevelMod; } // ONLY CHANGE THIS VALUE

			value = (int)( value * mod );
				if ( value < 0 ){ value = 1; }

			return value;
		}

		public static int WyrmBody()
		{
			if ( MySettings.S_WyrmBody != 723 && MySettings.S_WyrmBody != 12 && MySettings.S_WyrmBody != 59 ){ MySettings.S_WyrmBody = 723; }
 
			return MySettings.S_WyrmBody; // THIS IS WHAT WYRMS LOOK LIKE IN THE GAME...IF YOU WANT A DIFFERENT APPEARANCE THEN CHANGE THIS VALUE
		}

		public static bool FastFriends( Mobile m )					// IF TRUE, FOLLOWERS WILL ATTEMPT TO STAY WITH YOU IF YOU ARE RUNNING FAST
		{															// OTHERWISE THEY HAVE THEIR OWN DEFAULT SPEEDS
			if ( m is BaseCreature && ((BaseCreature)m).ControlMaster != null ){ return true; } // THIS VALUE YOU WOULD CHANGE
			return MySettings.S_FastFriends;
		}

		public static double BoatDecay() // HOW MANY DAYS A BOAT WILL LAST BEFORE IT DECAYS, WHERE using IT REFRESHES THE TIME
		{
			if ( MySettings.S_BoatDecay < 5 ){ MySettings.S_BoatDecay = 5.0; }
			return MySettings.S_BoatDecay;
		}

		public static double HomeDecay() // HOW MANY DAYS A HOUSE WILL LAST BEFORE IT DECAYS, WHERE using IT REFRESHES THE TIME
		{
			if ( MySettings.S_HomeDecay < 30 ){ MySettings.S_HomeDecay = 30.0; }
			return MySettings.S_HomeDecay;
		}

		public static double ResourcePrice()
		{
			int price = 0;

			if ( MySettings.S_ResourcePrice > 0 )
				price = MySettings.S_ResourcePrice;

			return price * 0.01;
		}

		public static double SellGoldCutRate()
		{
			int price = 0;

			if ( MySettings.S_SellGoldCutRate > 0 )
				price = MySettings.S_SellGoldCutRate;

			return price * 0.01;
		}

		public static double HigherPrice()
		{
			int price = 0;

			if ( MySettings.S_PriceMore > 0 )
				price = MySettings.S_PriceMore;

			return price * 0.01;
		}

		public static int LowerReg() // Overall cap
		{
			if ( MySettings.S_LowerReg > 100 )
				return 100;
			else if ( MySettings.S_LowerReg > 0 )
				return MySettings.S_LowerReg;

			return 0;
		}

		public static int LowerMana() // Overall cap
		{
			if ( MySettings.S_LowerMana > 100 )
				return 100;
			else if ( MySettings.S_LowerMana > 0 )
				return MySettings.S_LowerMana;

			return 0;
		}

		public static int LowMana() // Maxmimum enchantable value
		{
			return Math.Min(MySettings.S_LowerManaEnchantment, LowerMana());
		}

		public static int LowReg() // Maxmimum enchantable value
		{
			return Math.Min(MySettings.S_LowerRegEnchantment, LowerReg());
		}

		public static int HousesPerAccount() // HOW MANY HOUSES CAN ONE ACCOUNT HAVE, WHERE -1 IS NO LIMIT
		{
			if ( MySettings.S_HousesPerAccount == 0 ){ MySettings.S_HousesPerAccount = 1; }
			else if ( MySettings.S_HousesPerAccount < 0 ){ MySettings.S_HousesPerAccount = -1; }
			return MySettings.S_HousesPerAccount;
		}

		public static bool LineOfSight( Mobile m, bool hidden )
		{
			if ( MySettings.S_LineOfSight && m is BaseCreature && m.CanHearGhosts && hidden && m.Hidden )
				return true;
			else if ( MySettings.S_LineOfSight && m is BaseCreature && m.CanHearGhosts )
				return true;

			return false;
		}

		public static int Resources()
		{
			int res = MySettings.S_Resources;
				if ( res < 1 ){ res = 1; }
				else if ( res > 100 ){ res = 100; }

			return res;
		}

		public static bool Humanoids()
		{
			if ( MySettings.S_Humanoids && Utility.RandomMinMax(1,20) == 1 )
				return true;

			return false;
		}

		public static bool Humanoid()
		{
			if ( MySettings.S_Humanoids && Utility.RandomBool() )
				return true;

			return false;
		}

		public static bool BlackMarket()
		{
			if ( MySettings.S_BlackMarket )
				return true;

			return false;
		}

		public static double CorpseDecay()
		{
			if ( MySettings.S_CorpseDecay < 1 ){ MySettings.S_CorpseDecay = 0; }

			return (double)MySettings.S_CorpseDecay;
		}

		public static double BoneDecay()
		{
			if ( MySettings.S_BoneDecay < 1 ){ MySettings.S_BoneDecay = 0; }

			return (double)MySettings.S_BoneDecay;
		}

		public static bool AlterArtifact( Item item )
		{
			if ( item.ArtifactLevel > (int)ArtifactLevel.None && !MySettings.S_ChangeArtyLook )
				return false;

			return true;
		}

		public static bool MonstersAllowed()
		{
			if ( MySettings.S_MonsterCharacters > 0 )
				return true;

			return false;
		}

		public static double DeleteDelay()
		{
			if ( MySettings.S_DeleteDays < 1 ){ MySettings.S_DeleteDays = 0; }

			return (double)MySettings.S_DeleteDays;
		}

		public static double SpecialWeaponAbilSkill() // MIN SKILLS NEEDED TO START WEAPON SPECIAL ABILITIES
		{
			if ( MySettings.S_SpecialWeaponAbilSkill < 20 ){ MySettings.S_SpecialWeaponAbilSkill = 20.0; }
			return MySettings.S_SpecialWeaponAbilSkill;
		}

		public static int JoiningFee( Mobile m )
		{
			int fee = MySettings.S_GuildJoinFee;
				if ( fee < 200 )
					fee = 200;

			if ( m != null && m is PlayerMobile && MySettings.S_GuildIncrease )
				fee = fee + ( ((PlayerMobile)m).CharacterGuilds * fee );

			if ( fee < MySettings.S_GuildJoinFee )
				fee = MySettings.S_GuildJoinFee;

			if ( GetPlayerInfo.isFromSpace( m ) )
				fee = fee * 4;

			return fee;
		}

		public static int ExtraStableSlots()
		{
			int stable = MySettings.S_Stables;

			if ( stable < 0 )
				stable = 0;

			if ( stable > 20 )
				stable = 20;

			return stable;
		}

		public static int TrainMulti()
		{
			int mult = MySettings.S_TrainMulti;
				if ( mult < 1 ){ mult = 1; }
				else if ( mult > 100 ){ mult = 100; }

			return mult;
		}

		public static bool Safari( Land land )
		{
			bool safari = false;

			if ( land == Land.Sosaria && Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Sosaria )
				safari = true;

			if ( land == Land.Lodoria && Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Lodoria )
				safari = true;

			if ( land == Land.Serpent && Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Serpent )
				safari = true;

			if ( land == Land.Kuldar && Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Kuldar )
				safari = true;

			if ( land == Land.Savaged && Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Savaged )
				safari = true;

			return safari;
		}

		public static bool SafariStore()
		{
			bool safari = false;

			if ( Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Sosaria )
				safari = true;

			if ( Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Lodoria )
				safari = true;

			if ( Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Serpent )
				safari = true;

			if ( Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Kuldar )
				safari = true;

			if ( Utility.RandomMinMax(1,100) <= MySettings.S_Safari_Savaged )
				safari = true;

			return safari;
		}

		public static int SkillBoostCount()
		{
			return Math.Max( 0, Math.Min( MAX_BONUS_SKILL_COUNT, MySettings.S_SkillBoost ) );
		}

		public static int StartTypeBonusSkillCount( CharacterType type, PlayerMobile player )
		{
			switch (type)
			{
				case CharacterType.Fugitive: return player.Avatar.Active || player.Temptations.ReduceFugitiveSkillCap ? 2 : 3;
				case CharacterType.Savage: return 1;
				case CharacterType.Alien: return MySettings.S_AllowAlienChoice ? 30 : 0;
				case CharacterType.Default: return 0;
				default: return 0;
			}
		}

		public static int SkillBase()
		{
			return 1000 * ( DEFAULT_SKILL_COUNT + SkillBoostCount() );
		}

		public static double SkillGain()
		{
			int skill = 0;

			if ( MySettings.S_SkillGain > 10 )
				skill = 10;

			if ( MySettings.S_SkillGain < 1 )
				skill = 0;

			return skill * 0.1;
		}
		
		public static bool RequirePowerscrollsInOrder()
		{
			return MySettings.S_RequirePreviousPowerscroll;
		}

		public static int FoodCheck()
		{
			int time = 5;

			if ( MySettings.S_FoodCheck > 60 )
				time = 60;

			if ( MySettings.S_FoodCheck < 5 )
				time = 5;

			return time;
		}

		public static double BondDays()
		{
			int days = MySettings.S_BondDays;

			if ( days > 30 )
				days = 30;

			if ( days < 0 )
				days = 0;

			return (double)days;
		}

		public static string BondingDays()
		{
			string text = "To bond a creature, simply give them some food that they prefer. Once you do that, give them another. They should then be bonded to you from that moment onward.";

			if ( MySettings.S_BondDays > 0 )
				text = "To bond a creature, simply give them some food that they prefer. Once you do that, give them another " + MySettings.S_BondDays.ToString() + " days later. They should then be bonded to you from that moment onward.";

			return text;
		}

		public static int StartingGold()
		{
			int min = MySettings.S_MinGold;
			int max = MySettings.S_MaxGold;

			if ( min > max )
				min = max;

			int gold = Utility.RandomMinMax(min,max);

				if ( gold < 0 )
					gold = 0;

				if ( gold > 10000 )
					gold = 10000;

			return gold;
		}

		public static bool EnableDispelLogging()
		{
			return false;
		}

		public static bool EnableHealingLogging()
		{
			return false;
		}
	}
}