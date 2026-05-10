using System;
using Server.Mobiles;

namespace Server.Misc
{
	public class SkillCheck
	{
		public static bool DisableSkillGains = false;
		private static readonly bool AntiMacroCode = MyServerSettings.NoMacroing();		// Change this to false to disable anti-macro code
		public static TimeSpan AntiMacroExpire = TimeSpan.FromMinutes( 5.0 ); 			// How long do we remember targets/locations?
		public const int Allowance = 3;													// How many times may we use the same location/target for gain
		private const int LocationSize = 5; 											// The size of each location, make this smaller so players don't have to move as far
		private static bool[] UseAntiMacro = new bool[]
		{
			// true if this skill uses the anti-macro code, false if it does not
			false,// Alchemy = 0,
			true,// Anatomy = 1,
			true,// Druidism = 2,
			true,// ItemID = 3,
			true,// ArmsLore = 4,
			false,// Parry = 5,
			true,// Begging = 6,
			false,// Blacksmith = 7,
			false,// Bowcrafting = 8,
			true,// Peacemaking = 9,
			true,// Camping = 10,
			false,// Carpentry = 11,
			false,// Cartography = 12,
			false,// Cooking = 13,
			true,// Searching = 14,
			true,// Discordance = 15,
			true,// EvalInt = 16,
			true,// Healing = 17,
			false,// Seafaring = 18,
			true,// Forensics = 19,
			true,// Herding = 20,
			true,// Hiding = 21,
			true,// Provocation = 22,
			false,// Inscribe = 23,
			true,// Lockpicking = 24,
			true,// Magery = 25,
			true,// MagicResist = 26,
			false,// Tactics = 27,
			true,// Snooping = 28,
			true,// Musicianship = 29,
			true,// Poisoning = 30,
			false,// Marksmanship = 31,
			true,// Spiritualism = 32,
			true,// Stealing = 33,
			false,// Tailoring = 34,
			true,// Taming = 35,
			true,// Tasting = 36,
			false,// Tinkering = 37,
			true,// Tracking = 38,
			true,// Veterinary = 39,
			false,// Swords = 40,
			false,// Bludgeoning = 41,
			false,// Fencing = 42,
			false,// FistFighting = 43,
			true,// Lumberjacking = 44,
			true,// Mining = 45,
			true,// Meditation = 46,
			true,// Stealth = 47,
			true,// RemoveTrap = 48,
			true,// Necromancy = 49,
			false,// Focus = 50,
			true,// Knightship = 51
			true,// Bushido = 52
			true,//Ninjitsu = 53
			true // Elementalism = 54
		};

		public static void Initialize()
		{
			Mobile.SkillCheckLocationHandler = new SkillCheckLocationHandler( Mobile_SkillCheckLocation );
			Mobile.SkillCheckDirectLocationHandler = new SkillCheckDirectLocationHandler( Mobile_SkillCheckDirectLocation );

			Mobile.SkillCheckTargetHandler = new SkillCheckTargetHandler( Mobile_SkillCheckTarget );
			Mobile.SkillCheckDirectTargetHandler = new SkillCheckDirectTargetHandler( Mobile_SkillCheckDirectTarget );
		}

		public static bool Mobile_SkillCheckLocation( Mobile from, SkillName skillName, double minSkill, double maxSkill )
		{
			Skill skill = from.Skills[skillName];

			if ( skill == null )
				return false;

			double value = skill.Value;

			if ( value < minSkill )
				return false; // Too difficult
			else if ( value >= maxSkill )
				return true; // No challenge

			double chance = (value - minSkill) / (maxSkill - minSkill);

			Point2D loc = new Point2D( from.Location.X / LocationSize, from.Location.Y / LocationSize );
			return CheckSkill( from, skill, loc, chance );
		}

		public static bool Mobile_SkillCheckDirectLocation( Mobile from, SkillName skillName, double chance )
		{
			Skill skill = from.Skills[skillName];

			if ( skill == null )
				return false;

			if ( chance < 0.0 )
				return false; // Too difficult
			else if ( chance >= 1.0 )
				return true; // No challenge

			Point2D loc = new Point2D( from.Location.X / LocationSize, from.Location.Y / LocationSize );
			return CheckSkill( from, skill, loc, chance );
		}

		public static bool CheckSkill( Mobile from, Skill skill, object amObj, double chance )
		{
			SkillName skillName = skill.SkillName;

			if ( from.Skills.Cap == 0 )
				return false;

			double gainer = 2.0;

			if ( from is PlayerMobile )
			{
				if ( IsGuildSkill( from, skillName ) == true )
				{
					switch( Utility.RandomMinMax( 0, 5 ) )
					{
						case 0: gainer = 1.5; break;
						case 1: gainer = 1.4; break;
						case 2: gainer = 1.3; break;
						case 3: gainer = 1.2; break;
						case 4: gainer = 1.1; break;
						case 5: gainer = 1.0; break;
					}
				}
			}

			gainer = gainer - MyServerSettings.SkillGain();

			bool success = ( chance >= Utility.RandomDouble() );
			double gc = (double)(from.Skills.Cap - from.Skills.Total) / from.Skills.Cap;
			gc += ( skill.Cap - skill.Base ) / skill.Cap;
			gc /= gainer;

			gc += ( 1.0 - chance ) * ( success ? 0.5 : (Core.AOS ? 0.0 : 0.2) );
			gc /= gainer;

			gc *= skill.Info.GainFactor;

			if ( gc < 0.01 )
				gc = 0.01;

			if ( from is BaseCreature && ((BaseCreature)from).Controlled )
				gc *= 2;

			if (from is PlayerMobile)
			{	
				var player = (PlayerMobile)from;
				if (skill.Base <= 50)
					gc *= 1.5;
				else if (skill.Base <= 70)
					gc *= 1;
				else if (skill.Base <= 85)
					gc *= 0.75;
				else if (skill.Base <= 95)
					gc *= 0.5;
				else if (skill.Base <= 105)
					gc *= 0.46;
				else if (skill.Base <= 110)
					gc *= 0.42;
				else if (skill.Base <= 115)
					gc *= 0.38;
				else if (skill.Base <= 120)
					gc *= 0.38;
				else if (skill.Base <= 125)
					gc *= 0.38;
				
				if (player.Avatar.Active)
				{
					if (player.Avatar.SkillGainRateLevel > 0)
						gc *= 1 + (Engines.Avatar.Constants.SKILL_GAIN_RATE_PER_LEVEL * player.Avatar.SkillGainRateLevel * 0.01);
				}

				if (player.Temptations.AcceleratedSkillGain)
					gc *= 1.25;
			}
			
			if ( from.Alive && !DisableSkillGains && ( ( gc >= Utility.RandomDouble() && AllowGain( from, skill, amObj ) ) || skill.Base < 10.0 ) )
			{
				Gain( from, skill );
			}

			return success;
		}

		public static bool Mobile_SkillCheckTarget( Mobile from, SkillName skillName, object target, double minSkill, double maxSkill )
		{
			Skill skill = from.Skills[skillName];

			if ( skill == null )
				return false;

			double value = skill.Value;

			if ( value < minSkill )
				return false; // Too difficult
			else if ( value >= maxSkill )
				return true; // No challenge

			double chance = (value - minSkill) / (maxSkill - minSkill);

			return CheckSkill( from, skill, target, chance );
		}

		public static bool Mobile_SkillCheckDirectTarget( Mobile from, SkillName skillName, object target, double chance )
		{
			Skill skill = from.Skills[skillName];

			if ( skill == null )
				return false;

			if ( chance < 0.0 )
				return false; // Too difficult
			else if ( chance >= 1.0 )
				return true; // No challenge

			return CheckSkill( from, skill, target, chance );
		}

		public static bool IsGuildSkill( Mobile from, SkillName skillName )
		{
			PlayerMobile pm = (PlayerMobile)from;

			if ( pm.NpcGuild == NpcGuild.MagesGuild )
			{
				if ( skillName == SkillName.Psychology ){ return true; }
				else if ( skillName == SkillName.Magery ){ return true; }
				else if ( skillName == SkillName.Meditation ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.ElementalGuild )
			{
				if ( skillName == SkillName.Focus ){ return true; }
				else if ( skillName == SkillName.Elementalism ){ return true; }
				else if ( skillName == SkillName.Meditation ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.WarriorsGuild )
			{
				if ( skillName == SkillName.Fencing ){ return true; }
				else if ( skillName == SkillName.Bludgeoning ){ return true; }
				else if ( skillName == SkillName.Parry ){ return true; }
				else if ( skillName == SkillName.Swords ){ return true; }
				else if ( skillName == SkillName.Tactics ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.ThievesGuild )
			{
				if ( skillName == SkillName.Hiding ){ return true; }
				else if ( skillName == SkillName.Lockpicking ){ return true; }
				else if ( skillName == SkillName.Snooping ){ return true; }
				else if ( skillName == SkillName.Stealing ){ return true; }
				else if ( skillName == SkillName.Stealth ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.RangersGuild )
			{
				if ( skillName == SkillName.Camping ){ return true; }
				else if ( skillName == SkillName.Tracking ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.HealersGuild )
			{
				if ( skillName == SkillName.Anatomy ){ return true; }
				else if ( skillName == SkillName.Healing ){ return true; }
				else if ( skillName == SkillName.Veterinary ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.MinersGuild )
			{
				if ( skillName == SkillName.Mining ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.MerchantsGuild )
			{
				if ( skillName == SkillName.Mercantile ){ return true; }
				else if ( skillName == SkillName.ArmsLore ){ return true; }
				else if ( skillName == SkillName.Tasting ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.TinkersGuild )
			{
				if ( skillName == SkillName.Tinkering ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.TailorsGuild )
			{
				if ( skillName == SkillName.Tailoring ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.FishermensGuild )
			{
				if ( skillName == SkillName.Seafaring ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.BardsGuild )
			{
				if ( skillName == SkillName.Discordance ){ return true; }
				else if ( skillName == SkillName.Musicianship ){ return true; }
				else if ( skillName == SkillName.Peacemaking ){ return true; }
				else if ( skillName == SkillName.Provocation ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.BlacksmithsGuild )
			{
				if ( skillName == SkillName.Blacksmith ){ return true; }
				else if ( skillName == SkillName.ArmsLore ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.NecromancersGuild )
			{
				if ( skillName == SkillName.Forensics ){ return true; }
				else if ( skillName == SkillName.Necromancy ){ return true; }
				else if ( skillName == SkillName.Spiritualism ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.AlchemistsGuild )
			{
				if ( skillName == SkillName.Alchemy ){ return true; }
				else if ( skillName == SkillName.Cooking ){ return true; }
				else if ( skillName == SkillName.Tasting ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.DruidsGuild )
			{
				if ( skillName == SkillName.Druidism ){ return true; }
				else if ( skillName == SkillName.Taming ){ return true; }
				else if ( skillName == SkillName.Herding ){ return true; }
				else if ( skillName == SkillName.Veterinary ){ return true; }
				else if ( skillName == SkillName.Cooking ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.ArchersGuild )
			{
				if ( skillName == SkillName.Marksmanship ){ return true; }
				else if ( skillName == SkillName.Bowcraft ){ return true; }
				else if ( skillName == SkillName.Tactics ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.CarpentersGuild )
			{
				if ( skillName == SkillName.Carpentry ){ return true; }
				else if ( skillName == SkillName.Lumberjacking ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.CartographersGuild )
			{
				if ( skillName == SkillName.Cartography ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.LibrariansGuild )
			{
				if ( skillName == SkillName.Mercantile ){ return true; }
				else if ( skillName == SkillName.Inscribe ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.CulinariansGuild )
			{
				if ( skillName == SkillName.Cooking ){ return true; }
				else if ( skillName == SkillName.Tasting ){ return true; }
			}
			else if ( pm.NpcGuild == NpcGuild.AssassinsGuild )
			{
				if ( skillName == SkillName.Fencing ){ return true; }
				else if ( skillName == SkillName.Hiding ){ return true; }
				else if ( skillName == SkillName.Poisoning ){ return true; }
				else if ( skillName == SkillName.Stealth ){ return true; }
			}

			return false;
		}

		private static bool AllowGain( Mobile from, Skill skill, object obj )
		{
			if ( AntiMacroCode && from is PlayerMobile && UseAntiMacro[skill.Info.SkillID] )
				return ((PlayerMobile)from).AntiMacroCheck( skill, obj );
			else
				return true;
		}

		public enum Stat { Str, Dex, Int }

		public static void Gain( Mobile from, Skill skill )
		{
			if ( from.Region.IsPartOf( typeof( Regions.Jail ) ) )
				return;

			if ( from is BaseCreature && ((BaseCreature)from).IsDeadPet )
				return;

			if ( skill.SkillName == SkillName.Focus && from is BaseCreature )
				return;

			if ( skill.Base < skill.Cap && skill.Lock == SkillLock.Up )
			{
				int toGain = 1;

				if ( skill.Base <= 10.0 )
					toGain = Utility.Random( 4 ) + 1;

				Skills skills = from.Skills;
				bool isSecondarySkill = skill.IsSecondarySkill();

				// Secondary skills don't cause you to lose skill
				if ( !isSecondarySkill && from.Player && ( skills.Total / skills.Cap ) >= Utility.RandomDouble() )
				{
					for ( int i = 0; i < skills.Length; ++i )
					{
						Skill toLower = skills[i];

						if ( toLower != skill && toLower.Lock == SkillLock.Down && toLower.BaseFixedPoint >= toGain )
						{
							toLower.BaseFixedPoint -= toGain;
							break;
						}
					}
				}

				#region Scroll of Alacrity
				PlayerMobile pm = from as PlayerMobile;

				if ( from is PlayerMobile )
					if (pm != null && ( MyServerSettings.AlwaysUnderAlacrity() || ( skill.SkillName == pm.AcceleratedSkill && pm.AcceleratedStart > DateTime.Now ) ) )
						toGain *= Utility.RandomMinMax(2, 5);
					#endregion

				// Secondary skills ignore the skill cap
				if ( !from.Player || isSecondarySkill || (skills.Total + toGain) <= skills.Cap )
				{
					skill.BaseFixedPoint += toGain;

					EventSink.InvokeSkillGain(new SkillGainArgs(from, skill, toGain));

					if ( skill.SkillName == SkillName.Focus || skill.SkillName == SkillName.Meditation ){ if ( Utility.RandomMinMax( 1, 10 ) == 1 )
						{ Server.Gumps.SkillListingGump.RefreshSkillList( from ); }}
					else
						{ Server.Gumps.SkillListingGump.RefreshSkillList( from ); }
				}
			}

			if ( skill.Lock == SkillLock.Up )
			{
				SkillInfo info = skill.Info;

				if ( from.StrLock == StatLockType.Up && (info.StrGain / MyServerSettings.StatGain()) > Utility.RandomDouble() )
					GainStat( from, Stat.Str );
				else if ( from.DexLock == StatLockType.Up && (info.DexGain / MyServerSettings.StatGain()) > Utility.RandomDouble() )
					GainStat( from, Stat.Dex );
				else if ( from.IntLock == StatLockType.Up && (info.IntGain / MyServerSettings.StatGain()) > Utility.RandomDouble() )
					GainStat( from, Stat.Int );
			}
		}

		public static bool CanLower( Mobile from, Stat stat )
		{
			switch ( stat )
			{
				case Stat.Str: return ( from.StrLock == StatLockType.Down && from.RawStr > 10 );
				case Stat.Dex: return ( from.DexLock == StatLockType.Down && from.RawDex > 10 );
				case Stat.Int: return ( from.IntLock == StatLockType.Down && from.RawInt > 10 );
			}

			return false;
		}

		public static bool CanRaise( Mobile from, Stat stat )
		{
			if ( !(from is BaseCreature && ((BaseCreature)from).Controlled) )
			{
				if ( from.RawStatTotal >= from.StatCap )
					return false;
			}

			if ( from is PlayerMobile )
			{
				var player = (PlayerMobile)from;
				if (player.IsTitanOfEther)
				{
					switch ( stat )
					{
						case Stat.Str: return from.StrLock == StatLockType.Up && from.RawStr < 175;
						case Stat.Dex: return from.DexLock == StatLockType.Up && from.RawDex < 175;
						case Stat.Int: return from.IntLock == StatLockType.Up && from.RawInt < 175;
					}
				}
			}

			switch ( stat )
			{
				case Stat.Str: return from.StrLock == StatLockType.Up && from.RawStr < 150;
				case Stat.Dex: return from.DexLock == StatLockType.Up && from.RawDex < 150;
				case Stat.Int: return from.IntLock == StatLockType.Up && from.RawInt < 150;
			}

			return false;
		}

		public static void IncreaseStat( Mobile from, Stat stat, bool atrophy )
		{
			atrophy = atrophy || (from.RawStatTotal >= from.StatCap);

			switch ( stat )
			{
				case Stat.Str:
				{
					if ( atrophy )
					{
						if ( CanLower( from, Stat.Dex ) && (from.RawDex < from.RawInt || !CanLower( from, Stat.Int )) )
							--from.RawDex;
						else if ( CanLower( from, Stat.Int ) )
							--from.RawInt;
					}

					if ( CanRaise( from, Stat.Str ) )
						++from.RawStr;

					break;
				}
				case Stat.Dex:
				{
					if ( atrophy )
					{
						if ( CanLower( from, Stat.Str ) && (from.RawStr < from.RawInt || !CanLower( from, Stat.Int )) )
							--from.RawStr;
						else if ( CanLower( from, Stat.Int ) )
							--from.RawInt;
					}

					if ( CanRaise( from, Stat.Dex ) )
						++from.RawDex;

					break;
				}
				case Stat.Int:
				{
					if ( atrophy )
					{
						if ( CanLower( from, Stat.Str ) && (from.RawStr < from.RawDex || !CanLower( from, Stat.Dex )) )
							--from.RawStr;
						else if ( CanLower( from, Stat.Dex ) )
							--from.RawDex;
					}

					if ( CanRaise( from, Stat.Int ) )
						++from.RawInt;

					break;
				}
			}
		}

		private static TimeSpan m_StatGainDelay = MyServerSettings.StatGainDelay();
		private static TimeSpan m_PetStatGainDelay = MyServerSettings.PetStatGainDelay();

		public static TimeSpan GetCooldownRemaining(PlayerMobile player, StatType stat)
		{
			if (player.Avatar.Active) return TimeSpan.Zero;

			var gainDelay = player.Temptations.ReduceStatGainDelay ? TimeSpan.FromMinutes(5) : m_StatGainDelay;

			DateTime end;
			switch( stat )
			{
				case StatType.Dex: end = player.LastDexGain + gainDelay; break;
				case StatType.Int: end = player.LastIntGain + gainDelay; break;
				case StatType.Str: end = player.LastStrGain + gainDelay; break;
				default: return TimeSpan.Zero;
			}
			
			TimeSpan remaining = end - DateTime.Now;
			
			return 0 < remaining.TotalSeconds ? remaining : TimeSpan.Zero;
		}

		public static void GainStat( Mobile from, Stat stat )
		{
			var isAvatar = from is PlayerMobile && ((PlayerMobile)from).Avatar.Active;

			switch( stat )
			{
				case Stat.Str:
				{
					if ( from is BaseCreature && ((BaseCreature)from).Controlled ) {
						if ( (from.LastStrGain + m_PetStatGainDelay) >= DateTime.Now )
							return;
					}
					else if( !isAvatar && (from.LastStrGain + m_StatGainDelay) >= DateTime.Now )
						return;

					from.LastStrGain = DateTime.Now;
					break;
				}
				case Stat.Dex:
				{
					if ( from is BaseCreature && ((BaseCreature)from).Controlled ) {
						if ( (from.LastDexGain + m_PetStatGainDelay) >= DateTime.Now )
							return;
					}
					else if( !isAvatar && (from.LastDexGain + m_StatGainDelay) >= DateTime.Now )
						return;

					from.LastDexGain = DateTime.Now;
					break;
				}
				case Stat.Int:
				{
					if ( from is BaseCreature && ((BaseCreature)from).Controlled ) {
						if ( (from.LastIntGain + m_PetStatGainDelay) >= DateTime.Now )
							return;
					}

					else if( !isAvatar && (from.LastIntGain + m_StatGainDelay) >= DateTime.Now )
						return;

					from.LastIntGain = DateTime.Now;
					break;
				}
			}

			bool atrophy = ( (from.RawStatTotal / (double)from.StatCap) >= Utility.RandomDouble() );

			IncreaseStat( from, stat, atrophy );
		}
	}
}