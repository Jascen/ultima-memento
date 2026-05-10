/***************************************************************************
 *                                 Skills.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id$
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Server.Network;

namespace Server
{
	public delegate TimeSpan SkillUseCallback( Mobile user );

	public enum SkillLock : byte
	{
		Up = 0,
		Down = 1,
		Locked = 2
	}

	public enum SkillName
	{
		Alchemy = 0,
		Anatomy = 1,
		Druidism = 2,
		Mercantile = 3,
		ArmsLore = 4,
		Parry = 5,
		Begging = 6,
		Blacksmith = 7,
		Bowcraft = 8,
		Peacemaking = 9,
		Camping = 10,
		Carpentry = 11,
		Cartography = 12,
		Cooking = 13,
		Searching = 14,
		Discordance = 15,
		Psychology = 16,
		Healing = 17,
		Seafaring = 18,
		Forensics = 19,
		Herding = 20,
		Hiding = 21,
		Provocation = 22,
		Inscribe = 23,
		Lockpicking = 24,
		Magery = 25,
		MagicResist = 26,
		Tactics = 27,
		Snooping = 28,
		Musicianship = 29,
		Poisoning = 30,
		Marksmanship = 31,
		Spiritualism = 32,
		Stealing = 33,
		Tailoring = 34,
		Taming = 35,
		Tasting = 36,
		Tinkering = 37,
		Tracking = 38,
		Veterinary = 39,
		Swords = 40,
		Bludgeoning = 41,
		Fencing = 42,
		FistFighting = 43,
		Lumberjacking = 44,
		Mining = 45,
		Meditation = 46,
		Stealth = 47,
		RemoveTrap = 48,
		Necromancy = 49,
		Focus = 50,
		Knightship = 51,
		Bushido = 52,
		Ninjitsu = 53,
		Elementalism = 54,
		Mysticism = 55,
		Imbuing = 56,
		Throwing = 57
	}

	[PropertyObject]
	public class Skill
	{
		private Skills m_Owner;
		private SkillInfo m_Info;
		private ushort m_Base;
		private ushort m_Cap;
		private SkillLock m_Lock;

		public override string ToString()
		{
			return String.Format( "[{0}: {1}]", Name, Base );
		}

		public static string CharacterTitle( string skillTitle, bool gender, int karma, double knightship, double seafaring, double magery, double necromancy, double healing, double spirits, int isBarbaric, bool isOriental, bool isMonk, bool isSyth, bool isJedi, bool isJester, bool isEvil )
		{
			if ( isBarbaric > 0 && 
				( skillTitle.Contains("Alchemist") || 
				skillTitle.Contains("Naturalist") || 
				skillTitle.Contains("Archer") || 
				skillTitle.Contains("Explorer") || 
				skillTitle.Contains("Knight") || 
				skillTitle.Contains("Fencer") || 
				skillTitle.Contains("Shepherd") || 
				skillTitle.Contains("Bludgeoner") || 
				skillTitle.Contains("Wizard") || 
				skillTitle.Contains("Bard") || 
				skillTitle.Contains("Necromancer") || 
				skillTitle.Contains("Sailor") || 
				skillTitle.Contains("Ranger") || 
				skillTitle.Contains("Duelist") || 
				skillTitle.Contains("Swordsman") || 
				skillTitle.Contains("Man-at-arms") || 
				skillTitle.Contains("Tactician") || 
				skillTitle.Contains("Veterinarian") )
			)
			{
				if ( skillTitle.Contains("Alchemist") ){ skillTitle = skillTitle.Replace("Alchemist", "Herbalist"); }
				else if ( skillTitle.Contains("Naturalist") ){ skillTitle = skillTitle.Replace("Naturalist", "Beastmaster"); }
				else if ( skillTitle.Contains("Shepherd") ){ skillTitle = skillTitle.Replace("Shepherd", "Beastmaster"); }
				else if ( skillTitle.Contains("Sailor") )
				{
					skillTitle = skillTitle.Replace("Sailor", "Atlantean");
					if ( seafaring >= 100 ){ skillTitle = skillTitle.Replace("Atlantean", "Sea Captain"); }
				}
				else if ( skillTitle.Contains("Veterinarian") ){ skillTitle = skillTitle.Replace("Veterinarian", "Beastmaster"); }
				else if ( skillTitle.Contains("Explorer") ){ skillTitle = skillTitle.Replace("Explorer", "Wanderer"); }
				else if ( skillTitle.Contains("Knight") )
				{
					if ( karma < 0 ){ skillTitle = skillTitle.Replace("Knight", "Death Knight"); }
					else if ( isBarbaric > 1 ){ skillTitle = skillTitle.Replace("Knight", "Valkyrie"); }
					else { skillTitle = skillTitle.Replace("Knight", "Chieftain"); }
				}
				else if ( skillTitle.Contains("Tactician") ){ skillTitle = skillTitle.Replace("Tactician", "Warlord"); }
				else if ( skillTitle.Contains("Duelist") ){ skillTitle = skillTitle.Replace("Duelist", "Defender"); }
				else if ( skillTitle.Contains("Necromancer") ){ skillTitle = skillTitle.Replace("Necromancer", "Witch Doctor"); }
				else if ( skillTitle.Contains("Bard") ){ skillTitle = skillTitle.Replace("Bard", "Chronicler"); }
				else if ( skillTitle.Contains("Wizard") ){ skillTitle = skillTitle.Replace("Wizard", "Shaman"); }
				else if ( skillTitle.Contains("Archer") && isBarbaric > 1 ){ skillTitle = skillTitle.Replace("Archer", "Amazon"); }
				else if ( skillTitle.Contains("Fencer") && isBarbaric > 1 ){ skillTitle = skillTitle.Replace("Fencer", "Amazon"); }
				else if ( skillTitle.Contains("Bludgeoner") && isBarbaric > 1 ){ skillTitle = skillTitle.Replace("Bludgeoner", "Amazon"); }
				else if ( skillTitle.Contains("Swordsman") && isBarbaric > 1 ){ skillTitle = skillTitle.Replace("Swordsman", "Amazon"); }
				else if ( skillTitle.Contains("Archer") ){ skillTitle = skillTitle.Replace("Archer", "Barbarian"); }
				else if ( skillTitle.Contains("Fencer") ){ skillTitle = skillTitle.Replace("Fencer", "Barbarian"); }
				else if ( skillTitle.Contains("Bludgeoner") ){ skillTitle = skillTitle.Replace("Bludgeoner", "Barbarian"); }
				else if ( skillTitle.Contains("Swordsman") ){ skillTitle = skillTitle.Replace("Swordsman", "Barbarian"); }
				else if ( skillTitle.Contains("Ranger") ){ skillTitle = skillTitle.Replace("Ranger", "Hunter"); }
				else if ( skillTitle.Contains("Man-at-arms") ){ skillTitle = skillTitle.Replace("Man-at-arms", "Gladiator"); }
			}
			else if ( !isOriental && skillTitle.Contains("Wizard") && magery >= 100 && necromancy >= 100 ){ skillTitle = skillTitle.Replace("Wizard", "Archmage"); }
			else if ( !isOriental && skillTitle.Contains("Necromancer") && magery >= 100 && necromancy >= 100 ){ skillTitle = skillTitle.Replace("Necromancer", "Archmage"); }

			else if ( ( skillTitle.Contains("Brawler") ) && isMonk )
			{
				skillTitle = skillTitle.Replace("Brawler", "Monk");
				if ( magery >= 50 || necromancy >= 50 ){ skillTitle = skillTitle.Replace("Monk", "Mystic"); }
			}
			else if ( ( skillTitle.Contains("Scholar") ) && isSyth )
			{
				skillTitle = skillTitle.Replace("Scholar", "Syth");
			}
			else if ( ( skillTitle.Contains("Scholar") ) && isJedi )
			{
				string jedi = "Jedi";
				if ( knightship >= 100 ){ jedi = "Jedi Knight"; }
				skillTitle = skillTitle.Replace("Scholar", jedi);
			}

			else if ( skillTitle.Contains("Beggar") && isJester ){ skillTitle = skillTitle.Replace("Beggar", "Jester"); }
			else if ( skillTitle.Contains("Scholar") && isJester ){ skillTitle = skillTitle.Replace("Scholar", "Joker"); }
			else if ( skillTitle.Contains("Samurai") && karma < 0 ){ skillTitle = skillTitle.Replace("Samurai", "Ronin"); }
			else if ( skillTitle.Contains("Ninja") && karma < 0 ){ skillTitle = skillTitle.Replace("Ninja", "Yakuza"); }
			else if ( skillTitle.Contains("Wizard") && isOriental == true ){ skillTitle = skillTitle.Replace("Wizard", "Wu Jen"); }
			else if ( skillTitle.Contains("Swordsman") && isOriental == true ){ skillTitle = skillTitle.Replace("Swordsman", "Kensai"); }
			else if ( skillTitle.Contains("Healer") && isOriental == true ){ skillTitle = skillTitle.Replace("Healer", "Shukenja"); }
			else if ( skillTitle.Contains("Necromancer") && isOriental == true ){ skillTitle = skillTitle.Replace("Necromancer", "Fangshi"); }
			else if ( skillTitle.Contains("Alchemist") && isOriental == true ){ skillTitle = skillTitle.Replace("Alchemist", "Waidan"); }
			else if ( skillTitle.Contains("Medium") && isOriental == true ){ skillTitle = skillTitle.Replace("Medium", "Neidan"); }
			else if ( skillTitle.Contains("Archer") && isOriental == true ){ skillTitle = skillTitle.Replace("Archer", "Kyudo"); }
			else if ( skillTitle.Contains("Fencer") && isOriental == true ){ skillTitle = skillTitle.Replace("Fencer", "Yuki Ota"); }
			else if ( skillTitle.Contains("Tactician") && isOriental == true ){ skillTitle = skillTitle.Replace("Tactician", "Sakushi"); }
			else if ( skillTitle.Contains("Knight") && isOriental == true ){ skillTitle = skillTitle.Replace("Knight", "Youxia"); }

			else if ( ( skillTitle.Contains("Healer") || skillTitle.Contains("Medium") ) && karma >= 2500 && healing >= 50 && spirits >= 50 )
			{
				skillTitle = skillTitle.Replace("Medium", "Priest");
				skillTitle = skillTitle.Replace("Healer", "Priest");

				if ( isOriental == true ){ skillTitle = skillTitle.Replace("Priest", "Buddhist"); }
			}

			else if ( skillTitle.Contains("Brawler") && isOriental == true ){ skillTitle = skillTitle.Replace("Brawler", "Karateka"); }
			else if ( skillTitle.Contains("Wizard") && isEvil == true && gender ){ skillTitle = skillTitle.Replace("Wizard", "Enchantress"); }
			else if ( skillTitle.Contains("Wizard") && isEvil == true ){ skillTitle = skillTitle.Replace("Wizard", "Warlock"); }

			if ( isBarbaric == 0 )
			{
				if ( skillTitle.Contains("Shaman") && gender ){ skillTitle = skillTitle.Replace("Shaman", "Sorceress"); }
				if ( skillTitle.Contains("Necromancer") && gender ){ skillTitle = skillTitle.Replace("Necromancer", "Witch"); }
				if ( skillTitle.Contains("Knight") && karma < 0 ){ skillTitle = skillTitle.Replace("Knight", "Death Knight"); }
				if ( skillTitle.Contains("Healer") && karma < 0 ){ skillTitle = skillTitle.Replace("Healer", "Mortician"); }
			}

			if ( skillTitle.Contains("Sailor") && karma < 0 ){ skillTitle = skillTitle.Replace("Sailor", "Pirate"); }
			if ( skillTitle.Contains("Sailor") && seafaring >= 100 ){ skillTitle = skillTitle.Replace("Sailor", "Sea Captain"); }
			if ( skillTitle.Contains("Pirate") && seafaring >= 100 ){ skillTitle = skillTitle.Replace("Pirate", "Sea Pirate"); }

			//if ( gender && isBarbaric == 0 && skillTitle.EndsWith( "man" ) )
			//	skillTitle = skillTitle.Substring( 0, skillTitle.Length - 3 ) + "woman";

			return skillTitle;
		}

		public bool IsSecondarySkill()
		{
			switch(SkillName)
			{
				// Crafting skills
				case SkillName.Alchemy:
				case SkillName.Blacksmith:
				case SkillName.Bowcraft:
				case SkillName.Carpentry:
				case SkillName.Cooking:
				case SkillName.Inscribe:
				case SkillName.Tailoring:
				case SkillName.Tinkering:
					return true;

				// Gathering skills
				case SkillName.Forensics:
				case SkillName.Lumberjacking:
				case SkillName.Mining:
					return true;

				case SkillName.Anatomy:
				case SkillName.Druidism:
				case SkillName.Mercantile:
				case SkillName.ArmsLore:
				case SkillName.Parry:
				case SkillName.Begging:
				case SkillName.Peacemaking:
				case SkillName.Camping:
				case SkillName.Cartography:
				case SkillName.Searching:
				case SkillName.Discordance:
				case SkillName.Psychology:
				case SkillName.Healing:
				case SkillName.Seafaring:
				case SkillName.Herding:
				case SkillName.Hiding:
				case SkillName.Provocation:
				case SkillName.Lockpicking:
				case SkillName.Magery:
				case SkillName.MagicResist:
				case SkillName.Tactics:
				case SkillName.Snooping:
				case SkillName.Musicianship:
				case SkillName.Poisoning:
				case SkillName.Marksmanship:
				case SkillName.Spiritualism:
				case SkillName.Stealing:
				case SkillName.Taming:
				case SkillName.Tasting:
				case SkillName.Tracking:
				case SkillName.Veterinary:
				case SkillName.Swords:
				case SkillName.Bludgeoning:
				case SkillName.Fencing:
				case SkillName.FistFighting:
				case SkillName.Meditation:
				case SkillName.Stealth:
				case SkillName.RemoveTrap:
				case SkillName.Necromancy:
				case SkillName.Focus:
				case SkillName.Knightship:
				case SkillName.Bushido:
				case SkillName.Ninjitsu:
				case SkillName.Elementalism:
				case SkillName.Mysticism:
				case SkillName.Imbuing:
				case SkillName.Throwing:
				default:
					return false;
			}
		}

		public Skill( Skills owner, SkillInfo info, GenericReader reader )
		{
			m_Owner = owner;
			m_Info = info;

			int version = reader.ReadByte();

			switch ( version )
			{
				case 0:
				{
					m_Base = reader.ReadUShort();
					m_Cap = reader.ReadUShort();
					m_Lock = (SkillLock)reader.ReadByte();

					break;
				}
				case 0xFF:
				{
					m_Base = 0;
					m_Cap = 1000;
					m_Lock = SkillLock.Up;

					break;
				}
				default:
				{
					if ( (version & 0xC0) == 0x00 )
					{
						if ( (version & 0x1) != 0 )
							m_Base = reader.ReadUShort();

						if ( (version & 0x2) != 0 )
							m_Cap = reader.ReadUShort();
						else
							m_Cap = 1000;

						if ( (version & 0x4) != 0 )
							m_Lock = (SkillLock)reader.ReadByte();
					}

					break;
				}
			}

			if ( m_Lock < SkillLock.Up || m_Lock > SkillLock.Locked )
			{
				Console.WriteLine( "Bad skill lock -> {0}.{1}", owner.Owner, m_Lock );
				m_Lock = SkillLock.Up;
			}
		}

		public Skill( Skills owner, SkillInfo info, int baseValue, int cap, SkillLock skillLock )
		{
			m_Owner = owner;
			m_Info = info;
			m_Base = (ushort)baseValue;
			m_Cap = (ushort)cap;
			m_Lock = skillLock;
		}

		public void SetLockNoRelay( SkillLock skillLock )
		{
			if ( skillLock < SkillLock.Up || skillLock > SkillLock.Locked )
				return;

			m_Lock = skillLock;
		}

		public void Serialize( GenericWriter writer )
		{
			if ( m_Base == 0 && m_Cap == 1000 && m_Lock == SkillLock.Up )
			{
				writer.Write( (byte) 0xFF ); // default
			}
			else
			{
				int flags = 0x0;

				if ( m_Base != 0 )
					flags |= 0x1;

				if ( m_Cap != 1000 )
					flags |= 0x2;

				if ( m_Lock != SkillLock.Up )
					flags |= 0x4;

				writer.Write( (byte) flags ); // version

				if ( m_Base != 0 )
					writer.Write( (short) m_Base );

				if ( m_Cap != 1000 )
					writer.Write( (short) m_Cap );

				if ( m_Lock != SkillLock.Up )
					writer.Write( (byte) m_Lock );
			}
		}

		public Skills Owner
		{
			get
			{
				return m_Owner;
			}
		}

		public SkillName SkillName
		{
			get
			{
				return (SkillName)m_Info.SkillID;
			}
		}

		public int SkillID
		{
			get
			{
				return m_Info.SkillID;
			}
		}

		[CommandProperty( AccessLevel.Counselor )]
		public string Name
		{
			get
			{
				return m_Info.Name;
			}
		}

		public SkillInfo Info
		{
			get
			{
				return m_Info;
			}
		}

		[CommandProperty( AccessLevel.Counselor )]
		public SkillLock Lock
		{
			get
			{
				return m_Lock;
			}
		}

		public int BaseFixedPoint
		{
			get
			{
				return m_Base;
			}
			set
			{
				if ( value < 0 )
					value = 0;
				else if ( value >= 0x10000 )
					value = 0xFFFF;

				ushort sv = (ushort)value;

				int oldBase = m_Base;

				if ( m_Base != sv )
				{
					if (!IsSecondarySkill()) // Secondary skills don't affect Total
						m_Owner.Total = (m_Owner.Total - m_Base) + sv;

					int delta = value - sv;
					m_Base = sv;

					m_Owner.OnSkillChange( this );

					Mobile m = m_Owner.Owner;

					if ( m != null )
						m.OnSkillChange( SkillName, (double)oldBase / 10 );
				}
			}
		}

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public double Base
		{
			get
			{
				return ((double)m_Base / 10.0);
			}
			set
			{
				BaseFixedPoint = (int)(value * 10.0);
			}
		}

		public int CapFixedPoint
		{
			get
			{
				return m_Cap;
			}
			set
			{
				if ( value < 0 )
					value = 0;
				else if ( value >= 0x10000 )
					value = 0xFFFF;

				ushort sv = (ushort)value;

				if ( m_Cap != sv )
				{
					m_Cap = sv;

					m_Owner.OnSkillChange( this );
				}
			}
		}

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public double Cap
		{
			get
			{
				return ((double)m_Cap / 10.0);
			}
			set
			{
				CapFixedPoint = (int)(value * 10.0);
			}
		}

		private static bool m_UseStatMods;

		public static bool UseStatMods{ get{ return m_UseStatMods; } set{ m_UseStatMods = value; } }

		public int Fixed
		{
			get{ return (int)(Value * 10); }
		}

		[CommandProperty( AccessLevel.Counselor )]
		public double Value
		{
			get
			{
				return this.TotalSkillValue;
			}
		}

		[CommandProperty( AccessLevel.Counselor )]
		public double TotalSkillValue
		{
			get
			{
				double baseValue = Base;
				double inv = 100.0 - baseValue;

				if( inv < 0.0 ) inv = 0.0;

				inv /= 100.0;

				double statsOffset = ((m_UseStatMods ? m_Owner.Owner.Str : m_Owner.Owner.RawStr) * m_Info.StrScale) + ((m_UseStatMods ? m_Owner.Owner.Dex : m_Owner.Owner.RawDex) * m_Info.DexScale) + ((m_UseStatMods ? m_Owner.Owner.Int : m_Owner.Owner.RawInt) * m_Info.IntScale);
				double statTotal = m_Info.StatTotal * inv;

				statsOffset *= inv;

				if( statsOffset > statTotal )
					statsOffset = statTotal;

				double value = baseValue + statsOffset;

				m_Owner.Owner.ValidateSkillMods();

				List<SkillMod> mods = m_Owner.Owner.SkillMods;

				double bonusObey = 0.0, bonusNotObey = 0.0;

				for( int i = 0; i < mods.Count; ++i )
				{
					SkillMod mod = mods[i];

					if( mod.Skill == (SkillName)m_Info.SkillID )
					{
						if( mod.Relative )
						{
							if( mod.ObeyCap )
								bonusObey += mod.Value;
							else
								bonusNotObey += mod.Value;
						}
						else
						{
							bonusObey = 0.0;
							bonusNotObey = 0.0;
							value = mod.Value;
						}
					}
				}

				value += bonusNotObey;

				if( value < Cap )
				{
					value += bonusObey;

					if( value > Cap )
						value = Cap;
				}

				if ( value > 125 ){ value = 125; }

				return value;
			}
		}

		public void Update()
		{
			m_Owner.OnSkillChange( this );
		}
	}

	public class SkillInfo
	{
		private int m_SkillID;
		private string m_Name;
		private string m_Title;
		private double m_StrScale;
		private double m_DexScale;
		private double m_IntScale;
		private double m_StatTotal;
		private SkillUseCallback m_Callback;
		private double m_StrGain;
		private double m_DexGain;
		private double m_IntGain;
		private double m_GainFactor;

		public SkillInfo( int skillID, string name, double strScale, double dexScale, double intScale, string title, SkillUseCallback callback, double strGain, double dexGain, double intGain, double gainFactor )
		{
			m_Name = name;
			m_Title = title;
			m_SkillID = skillID;
			m_StrScale = strScale / 100.0;
			m_DexScale = dexScale / 100.0;
			m_IntScale = intScale / 100.0;
			m_Callback = callback;
			m_StrGain = strGain;
			m_DexGain = dexGain;
			m_IntGain = intGain;
			m_GainFactor = gainFactor;

			m_StatTotal = strScale + dexScale + intScale;
		}

		public SkillUseCallback Callback
		{
			get
			{
				return m_Callback;
			}
			set
			{
				m_Callback = value;
			}
		}

		public int SkillID
		{
			get
			{
				return m_SkillID;
			}
		}

		public string Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				m_Name = value;
			}
		}

		public string Title
		{
			get
			{
				return m_Title;
			}
			set
			{
				m_Title = value;
			}
		}

		public double StrScale
		{
			get
			{
				return m_StrScale;
			}
			set
			{
				m_StrScale = value;
			}
		}

		public double DexScale
		{
			get
			{
				return m_DexScale;
			}
			set
			{
				m_DexScale = value;
			}
		}

		public double IntScale
		{
			get
			{
				return m_IntScale;
			}
			set
			{
				m_IntScale = value;
			}
		}

		public double StatTotal
		{
			get
			{
				return m_StatTotal;
			}
			set
			{
				m_StatTotal = value;
			}
		}

		public double StrGain
		{
			get
			{
				return m_StrGain;
			}
			set
			{
				m_StrGain = value;
			}
		}

		public double DexGain
		{
			get
			{
				return m_DexGain;
			}
			set
			{
				m_DexGain = value;
			}
		}

		public double IntGain
		{
			get
			{
				return m_IntGain;
			}
			set
			{
				m_IntGain = value;
			}
		}

		public double GainFactor
		{
			get
			{
				return m_GainFactor;
			}
			set
			{
				m_GainFactor = value;
			}
		}

		private static SkillInfo[] m_Table = new SkillInfo[58]
			{
				new SkillInfo(  0, "Alchemy",			0.0,	5.0,	5.0,	"Alchemist",	null,	0.0,	0.5,	0.5,	1.0 ),
				new SkillInfo(  1, "Anatomy",			0.0,	0.0,	0.0,	"Biologist",	null,	0.15,	0.15,	0.7,	1.0 ),
				new SkillInfo(  2, "Druidism",			0.0,	0.0,	0.0,	"Druid",	null,	0.0,	0.0,	1.0,	1.0 ),
				new SkillInfo(  3, "Mercantile",		0.0,	0.0,	0.0,	"Merchant",	null,	0.0,	0.0,	1.0,	1.0 ),
				new SkillInfo(  4, "Arms Lore",			0.0,	0.0,	0.0,	"Man-at-arms",	null,	0.75,	0.15,	0.1,	1.0 ),
				new SkillInfo(  5, "Parrying",			7.5,	2.5,	0.0,	"Duelist",	null,	0.75,	0.25,	0.0,	1.0 ),
				new SkillInfo(  6, "Begging",			0.0,	0.0,	0.0,	"Beggar",	null,	0.0,	0.0,	0.0,	1.0 ),
				new SkillInfo(  7, "Blacksmithy",		10.0,	0.0,	0.0,	"Blacksmith",	null,	1.0,	0.0,	0.0,	1.0 ),
				new SkillInfo(  8, "Bowcrafting",		6.0,	16.0,	0.0,	"Bowyer",	null,	0.6,	1.6,	0.0,	1.0 ),
				new SkillInfo(  9, "Peacemaking",		0.0,	0.0,	0.0,	"Pacifier",		null,	0.0,	0.0,	0.0,	1.0 ),
				new SkillInfo( 10, "Camping",			20.0,	15.0,	15.0,	"Explorer",	null,	2.0,	1.5,	1.5,	1.0 ),
				new SkillInfo( 11, "Carpentry",			20.0,	5.0,	0.0,	"Carpenter",	null,	2.0,	0.5,	0.0,	1.0 ),
				new SkillInfo( 12, "Cartography",		0.0,	7.5,	7.5,	"Cartographer",	null,	0.0,	0.75,	0.75,	1.0 ),
				new SkillInfo( 13, "Cooking",			0.0,	20.0,	30.0,	"Chef",		null,	0.0,	2.0,	3.0,	1.0 ),
				new SkillInfo( 14, "Searching",			0.0,	0.0,	0.0,	"Scout",	null,	0.0,	0.4,	0.6,	4.0 ),
				new SkillInfo( 15, "Discordance",		0.0,	2.5,	2.5,	"Demoralizer",		null,	0.0,	0.25,	0.25,	1.0 ),
				new SkillInfo( 16, "Psychology",		0.0,	0.0,	0.0,	"Scholar",	null,	0.0,	0.0,	1.0,	1.0 ),
				new SkillInfo( 17, "Healing",			6.0,	6.0,	8.0,	"Healer",	null,	0.6,	0.6,	0.8,	1.0 ),
				new SkillInfo( 18, "Seafaring",			0.0,	0.0,	0.0,	"Sailor",	null,	0.5,	0.5,	0.0,	1.0 ),
				new SkillInfo( 19, "Forensics",			0.0,	0.0,	0.0,	"Undertaker",	null,	0.0,	0.2,	0.8,	1.0 ),
				new SkillInfo( 20, "Herding",			16.25,	6.25,	2.5,	"Shepherd",	null,	1.625,	0.625,	0.25,	1.0 ),
				new SkillInfo( 21, "Hiding",			0.0,	0.0,	0.0,	"Skulker",	null,	0.0,	0.8,	0.2,	1.3 ),
				new SkillInfo( 22, "Provocation",		0.0,	4.5,	0.5,	"Rouser",		null,	0.0,	0.45,	0.05,	1.5 ),
				new SkillInfo( 23, "Inscription",		0.0,	2.0,	8.0,	"Scribe",	null,	0.0,	0.2,	0.8,	1.0 ),
				new SkillInfo( 24, "Lockpicking",		0.0,	25.0,	0.0,	"Lockpicker",	null,	0.0,	2.0,	0.0,	1.0 ),
				new SkillInfo( 25, "Magery",			0.0,	0.0,	15.0,	"Wizard",		null,	0.0,	0.0,	1.5,	1.0 ),
				new SkillInfo( 26, "Magic Resistance",	0.0,	0.0,	0.0,	"Magic Warder",		null,	0.25,	0.25,	0.5,	4.0 ),
				new SkillInfo( 27, "Tactics",			0.0,	0.0,	0.0,	"Tactician",	null,	0.0,	0.0,	0.0,	1.0 ),
				new SkillInfo( 28, "Snooping",			0.0,	25.0,	0.0,	"Spy",	null,	0.0,	2.5,	0.0,	1.2 ),
				new SkillInfo( 29, "Musicianship",		0.0,	0.0,	0.0,	"Bard",		null,	0.0,	0.8,	0.2,	1.0 ),
				new SkillInfo( 30, "Poisoning",			0.0,	4.0,	16.0,	"Assassin",	null,	0.0,	0.4,	1.6,	1.0 ),
				new SkillInfo( 31, "Marksmanship",		2.5,	7.5,	0.0,	"Deadeye",	null,	0.25,	0.75,	0.0,	1.0 ),
				new SkillInfo( 32, "Spiritualism",		0.0,	0.0,	0.0,	"Spiritualist",	null,	0.0,	0.0,	1.0,	1.0 ),
				new SkillInfo( 33, "Stealing",			0.0,	10.0,	0.0,	"Thief",	null,	0.0,	1.0,	0.0,	1.3 ),
				new SkillInfo( 34, "Tailoring",			3.75,	16.25,	5.0,	"Tailor",	null,	0.38,	1.63,	0.5,	1.0 ),
				new SkillInfo( 35, "Taming",			14.0,	2.0,	4.0,	"Beastmaster",	null,	1.4,	0.2,	0.4,	1.0 ),
				new SkillInfo( 36, "Tasting",			0.0,	0.0,	0.0,	"Food Taster",		null,	0.2,	0.0,	0.8,	1.0 ),
				new SkillInfo( 37, "Tinkering",			5.0,	2.0,	3.0,	"Tinker",	null,	0.5,	0.2,	0.3,	1.0 ),
				new SkillInfo( 38, "Tracking",			0.0,	12.5,	12.5,	"Ranger",	null,	0.0,	1.25,	1.25,	1.5 ),
				new SkillInfo( 39, "Veterinary",		8.0,	4.0,	8.0,	"Veterinarian",	null,	0.8,	0.4,	0.8,	1.0 ),
				new SkillInfo( 40, "Swordsmanship",		7.5,	2.5,	0.0,	"Swordsman",	null,	0.75,	0.25,	0.0,	1.0 ),
				new SkillInfo( 41, "Bludgeoning",		9.0,	1.0,	0.0,	"Bludgeoner",	null,	0.9,	0.1,	0.0,	1.0 ),
				new SkillInfo( 42, "Fencing",			4.5,	5.5,	0.0,	"Fencer",	null,	0.45,	0.55,	0.0,	1.0 ),
				new SkillInfo( 43, "Fist Fighting",		9.0,	1.0,	0.0,	"Brawler",	null,	0.9,	0.1,	0.0,	1.0 ),
				new SkillInfo( 44, "Lumberjacking",		20.0,	0.0,	0.0,	"Lumberjack",	null,	2.0,	0.0,	0.0,	1.0 ),
				new SkillInfo( 45, "Mining",			20.0,	0.0,	0.0,	"Miner",	null,	2.0,	0.0,	0.0,	1.0 ),
				new SkillInfo( 46, "Meditation",		0.0,	0.0,	0.0,	"Meditator",	null,	0.0,	0.0,	0.0,	1.0 ),
				new SkillInfo( 47, "Stealth",			0.0,	0.0,	0.0,	"Sneak",	null,	0.0,	0.0,	0.0,	2.0 ),
				new SkillInfo( 48, "Remove Trap",		0.0,	0.0,	0.0,	"Trespasser",	null,	0.0,	0.0,	0.0,	4.0 ),
				new SkillInfo( 49, "Necromancy",		0.0,	0.0,	0.0,	"Necromancer",	null,	0.0,	0.0,	0.0,	1.0 ),
				new SkillInfo( 50, "Focus",				0.0,	0.0,	0.0,	"Driven",	null,	0.0,	0.0,	0.0,	1.0 ),
				new SkillInfo( 51, "Knightship",		0.0,	0.0,	0.0,	"Knight",	null,	0.0,	0.0,	0.0,	1.0 ),
				new SkillInfo( 52, "Bushido",			0.0,	0.0,	0.0,	"Samurai",	null,	0.0,	0.0,	0.0,	1.0 ),
				new SkillInfo( 53, "Ninjitsu",			0.0,	0.0,	0.0,	"Ninja",	null,	0.0,	0.0,	0.0,	1.5 ),
				new SkillInfo( 54, "Elementalism",		0.0,	0.0,	15.0,	"Elementalist",		null,	0.0,	1.0,	1.0,	1.0 ),
				new SkillInfo( 55, "Mysticism",			0.0,	0.0,	0.0,	"Mystic",	null,	0.0,	0.0,	0.0,	1.0 ),
				new SkillInfo( 56, "Imbuing",			0.0,	0.0,	0.0,	"Artificer",	null,	0.0,	0.0,	0.0,	1.0 ),
				new SkillInfo( 57, "Throwing",			0.0,	0.0,	0.0,	"Bladeweaver",	null,	0.0,	0.0,	0.0,	1.0 ),
			};

		public static SkillInfo[] Table
		{
			get
			{
				return m_Table;
			}
			set
			{
				m_Table = value;
			}
		}
	}

	[PropertyObject]
	public class Skills
	{
		private Mobile m_Owner;
		private Skill[] m_Skills;
		private int m_Total, m_Cap;
		private Skill m_Highest;

		#region Skill Getters & Setters
		[CommandProperty( AccessLevel.Counselor )]
		public Skill Alchemy{ get{ return this[SkillName.Alchemy]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Anatomy{ get{ return this[SkillName.Anatomy]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Druidism{ get{ return this[SkillName.Druidism]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Mercantile{ get{ return this[SkillName.Mercantile]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill ArmsLore{ get{ return this[SkillName.ArmsLore]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Parry{ get{ return this[SkillName.Parry]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Begging{ get{ return this[SkillName.Begging]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Blacksmith{ get{ return this[SkillName.Blacksmith]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Bowcraft{ get{ return this[SkillName.Bowcraft]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Peacemaking{ get{ return this[SkillName.Peacemaking]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Camping{ get{ return this[SkillName.Camping]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Carpentry{ get{ return this[SkillName.Carpentry]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Cartography{ get{ return this[SkillName.Cartography]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Cooking{ get{ return this[SkillName.Cooking]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Searching{ get{ return this[SkillName.Searching]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Discordance{ get{ return this[SkillName.Discordance]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Psychology{ get{ return this[SkillName.Psychology]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Healing{ get{ return this[SkillName.Healing]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Seafaring{ get{ return this[SkillName.Seafaring]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Forensics{ get{ return this[SkillName.Forensics]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Herding{ get{ return this[SkillName.Herding]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Hiding{ get{ return this[SkillName.Hiding]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Provocation{ get{ return this[SkillName.Provocation]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Inscribe{ get{ return this[SkillName.Inscribe]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Lockpicking{ get{ return this[SkillName.Lockpicking]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Magery{ get{ return this[SkillName.Magery]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill MagicResist{ get{ return this[SkillName.MagicResist]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Tactics{ get{ return this[SkillName.Tactics]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Snooping{ get{ return this[SkillName.Snooping]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Musicianship{ get{ return this[SkillName.Musicianship]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Poisoning{ get{ return this[SkillName.Poisoning]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Marksmanship{ get{ return this[SkillName.Marksmanship]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Spiritualism{ get{ return this[SkillName.Spiritualism]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Stealing{ get{ return this[SkillName.Stealing]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Tailoring{ get{ return this[SkillName.Tailoring]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Taming{ get{ return this[SkillName.Taming]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Tasting{ get{ return this[SkillName.Tasting]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Tinkering{ get{ return this[SkillName.Tinkering]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Tracking{ get{ return this[SkillName.Tracking]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Veterinary{ get{ return this[SkillName.Veterinary]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Swords{ get{ return this[SkillName.Swords]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Bludgeoning{ get{ return this[SkillName.Bludgeoning]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Fencing{ get{ return this[SkillName.Fencing]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill FistFighting{ get{ return this[SkillName.FistFighting]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Lumberjacking{ get{ return this[SkillName.Lumberjacking]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Mining{ get{ return this[SkillName.Mining]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Meditation{ get{ return this[SkillName.Meditation]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Stealth{ get{ return this[SkillName.Stealth]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill RemoveTrap{ get{ return this[SkillName.RemoveTrap]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Necromancy{ get{ return this[SkillName.Necromancy]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Focus{ get{ return this[SkillName.Focus]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Knightship{ get{ return this[SkillName.Knightship]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Bushido{ get{ return this[SkillName.Bushido]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Ninjitsu{ get{ return this[SkillName.Ninjitsu]; } set{} }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Elementalism { get { return this[SkillName.Elementalism]; } set { } }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Mysticism { get { return this[SkillName.Mysticism]; } set { } }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Imbuing { get { return this[SkillName.Imbuing]; } set { } }

		[CommandProperty( AccessLevel.Counselor )]
		public Skill Throwing { get { return this[SkillName.Throwing]; } set { } }

		#endregion

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public int Cap
		{
			get{ return m_Cap; } 
			set{ m_Cap = value; }
		}

		public int Total
		{
			get{ return m_Total; }
			set{ m_Total = value; }
		}

		public Mobile Owner
		{
			get{ return m_Owner; }
		}

		public int Length
		{
			get{ return m_Skills.Length; }
		}

		public Skill this[SkillName name]
		{
			get{ return this[(int)name]; }
		}

		public Skill this[int skillID]
		{
			get
			{
				if ( skillID < 0 || skillID >= m_Skills.Length )
					return null;

				Skill sk = m_Skills[skillID];

				if ( sk == null )
					m_Skills[skillID] = sk = new Skill( this, SkillInfo.Table[skillID], 0, 1000, SkillLock.Up );

				return sk;
			}
		}

		public override string ToString()
		{
			return "...";
		}

		public static bool UseSkill( Mobile from, SkillName name )
		{
			return UseSkill( from, (int)name );
		}

		public static bool UseSkill( Mobile from, int skillID )
		{
			if ( !from.CheckAlive() )
				return false;
			else if ( !from.Region.OnSkillUse( from, skillID ) )
				return false;
			else if ( !from.AllowSkillUse( (SkillName)skillID ) )
				return false;

			if ( skillID >= 0 && skillID < SkillInfo.Table.Length )
			{
				SkillInfo info = SkillInfo.Table[skillID];

				if ( info.Callback != null )
				{
					if ( from.NextSkillTime <= DateTime.Now && from.Spell == null )
					{
						from.DisruptiveAction();

						from.NextSkillTime = DateTime.Now + info.Callback( from );

						return true;
					}
					else
					{
						from.SendSkillMessage();
					}
				}
				else
				{
					from.SendLocalizedMessage( 500014 ); // That skill cannot be used directly.
				}
			}

			return false;
		}

		public Skill Highest
		{
			get
			{
				if ( m_Highest == null )
				{
					Skill highest = null;
					int value = int.MinValue;

					for ( int i = 0; i < m_Skills.Length; ++i )
					{
						Skill sk = m_Skills[i];

						if ( sk != null && sk.BaseFixedPoint > value )
						{
							value = sk.BaseFixedPoint;
							highest = sk;
						}
					}

					if ( highest == null && m_Skills.Length > 0 )
						highest = this[0];

					m_Highest = highest;
				}

				return m_Highest;
			}
		}

		public void Serialize( GenericWriter writer )
		{
			m_Total = 0;

			writer.Write( (int) 3 ); // version

			writer.Write( (int) m_Cap );
			writer.Write( (int) m_Skills.Length );

			for ( int i = 0; i < m_Skills.Length; ++i )
			{
				Skill sk = m_Skills[i];

				if ( sk == null )
				{
					writer.Write( (byte) 0xFF );
				}
				else
				{
					sk.Serialize( writer );
					if (!sk.IsSecondarySkill()) // Secondary skills don't affect Total
						m_Total += sk.BaseFixedPoint;
				}
			}
		}

		public Skills( Mobile owner )
		{
			m_Owner = owner;
			m_Cap = 7000;

			SkillInfo[] info = SkillInfo.Table;

			m_Skills = new Skill[info.Length];

			//for ( int i = 0; i < info.Length; ++i )
			//	m_Skills[i] = new Skill( this, info[i], 0, 1000, SkillLock.Up );
		}

		public Skills( Mobile owner, GenericReader reader )
		{
			m_Owner = owner;

			int version = reader.ReadInt();

			switch ( version )
			{
				case 3:
				case 2:
				{
					m_Cap = reader.ReadInt();

					goto case 1;
				}
				case 1:
				{
					if ( version < 2 )
						m_Cap = 7000;

					if ( version < 3 )
						/*m_Total =*/ reader.ReadInt();

					SkillInfo[] info = SkillInfo.Table;

					m_Skills = new Skill[info.Length];

					int count = reader.ReadInt();

					for ( int i = 0; i < count; ++i )
					{
						if ( i < info.Length )
						{
							Skill sk = new Skill( this, info[i], reader );

							if ( sk.BaseFixedPoint != 0 || sk.CapFixedPoint != 1000 || sk.Lock != SkillLock.Up )
							{
								m_Skills[i] = sk;
								if (!sk.IsSecondarySkill()) // Secondary skills don't affect Total
									m_Total += sk.BaseFixedPoint;
							}
						}
						else
						{
							new Skill( this, null, reader );
						}
					}

					//for ( int i = count; i < info.Length; ++i )
					//	m_Skills[i] = new Skill( this, info[i], 0, 1000, SkillLock.Up );

					break;
				}
				case 0:
				{
					reader.ReadInt();

					goto case 1;
				}
			}
		}

		public void OnSkillChange( Skill skill )
		{
			if ( skill == m_Highest ) // could be downgrading the skill, force a recalc
				m_Highest = null;
			else if ( m_Highest != null && skill.BaseFixedPoint > m_Highest.BaseFixedPoint )
				m_Highest = skill;

			m_Owner.OnSkillInvalidated( skill );

			NetState ns = m_Owner.NetState;

			if ( ns != null )
				ns.Send( new SkillChange( skill ) );
		}
	}
}