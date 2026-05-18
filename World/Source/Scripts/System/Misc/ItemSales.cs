using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Network;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Misc;
using Server.Regions;
using Server.Multis;
using Server.Engines.Plants;
using Server.Engines.Apiculture;
using Server.Engines.Mahjong;
using System.IO;
using System.Text;

namespace Server
{
	public class ItemInformation
	{
		public static ItemSalesInfo GetData( int val )
		{
			ItemSalesInfo[] list = ItemSalesInfo.m_SellingInfo;
			return list[val];
		}

		public static int GetInfo( Type itemtype )
		{
			ItemSalesInfo[] list = ItemSalesInfo.m_SellingInfo;
			int entries = list.Length;
			int val = 0;
			bool record = false;

			while ( entries > 0 )
			{
				if ( list[val].ItemsType == itemtype )
				{
					record = true;
					entries = 0;
				}
				else
					val++;

				entries--;
			}

			if ( record )
				return val;
			else
				return -1;
		}

		public static int GetValue( GemType gemType )
		{
			switch ( gemType )
			{
				case GemType.Amber: return 50;
				case GemType.Citrine: return 60;
				case GemType.Ruby: return 70;
				case GemType.Tourmaline: return 80;
				case GemType.Amethyst: return 90;
				case GemType.Emerald: return 100;
				case GemType.Sapphire: return 110;
				case GemType.StarSapphire: return 120;
				case GemType.Diamond: return 150;
				case GemType.Pearl: return 500;

				case GemType.None:
				default:
					return 0;
			}
		}

		public static int AddUpBenefits( Item item, int price, bool checkCrafted, bool resale )
		{
			if ( item.CoinPrice > 0 )
			{
				if ( item.NotIdentified )
					return ( 2 * Utility.RandomMinMax( 5, 25 ) );

				return ( item.CoinPrice * 2 );
			}
			else if ( !MySettings.S_QualityPrices && !resale )
				return price;
			else if (	( item is BaseArmor && ((BaseArmor)item).Quality == ArmorQuality.Low ) || 
					( item is BaseClothing && ((BaseClothing)item).Quality == ClothingQuality.Low ) || 
					( item is BaseQuiver && ((BaseQuiver)item).Quality == ClothingQuality.Low ) || 
					( item is BaseWeapon && ((BaseWeapon)item).Quality == WeaponQuality.Low ) || 
					( item is BaseInstrument && ((BaseInstrument)item).Quality == InstrumentQuality.Low ) 
				)
				price = (int)( price * 0.60 );
			else if (	( item is BaseArmor && ((BaseArmor)item).Quality == ArmorQuality.Exceptional ) || 
						( item is BaseClothing && ((BaseClothing)item).Quality == ClothingQuality.Exceptional ) || 
						( item is BaseQuiver && ((BaseQuiver)item).Quality == ClothingQuality.Exceptional ) || 
						( item is BaseWeapon && ((BaseWeapon)item).Quality == WeaponQuality.Exceptional ) || 
						( item is BaseInstrument && ((BaseInstrument)item).Quality == InstrumentQuality.Exceptional ) 
				)
				price = (int)( price * 1.25 );

			if ( CraftResources.GetGold( item.Resource ) > 0 )
				price = (int)( CraftResources.GetGold( item.Resource ) * price );

			if ( item.Enchanted != MagicSpell.None && item.EnchantUsesMax == 0 && item.EnchantUses > 5 )
			{
				int level = SpellItems.GetLevel( (int)(item.Enchanted) );
				price += level * 50;
			}

			if ( price < 1 )
				price = 1;

			if ( !item.Built && checkCrafted )
				return 0;
			else if ( item is BaseInstrument && ((BaseInstrument)item).UsesRemaining < 50 )
				return 0;
			else if ( item is BaseTool && ((BaseTool)item).UsesRemaining < 20 )
				return 0;
			else if ( item is BaseHarvestTool && ((BaseHarvestTool)item).UsesRemaining < 20 )
				return 0;
			else if ( item.EnchantUsesMax > 0 && item.EnchantUses < 6 && item.Enchanted == MagicSpell.None )
				return 0;

			if ( item is BaseTrinket && ((BaseTrinket)item).GemType != GemType.None )
			{
				price += GetValue(((BaseTrinket)item).GemType);
			}

			if ( item is BaseArmor )
			{
				price +=		((BaseArmor)item).ArmorAttributes.DurabilityBonus * 2;
				price +=		((BaseArmor)item).ArmorAttributes.LowerStatReq * 2;
				price +=		((BaseArmor)item).ArmorAttributes.SelfRepair * 100;
				price +=		((BaseArmor)item).ArmorAttributes.MageArmor * 200;
				price +=		((BaseArmor)item).PhysicalBonus * 2;
				price +=		((BaseArmor)item).FireBonus * 2;
				price +=		((BaseArmor)item).ColdBonus * 2;
				price +=		((BaseArmor)item).PoisonBonus * 2;
				price +=		((BaseArmor)item).EnergyBonus * 2;
				price +=		((BaseArmor)item).DexBonus * 5;
				price +=		((BaseArmor)item).IntBonus * 5;
				price +=		((BaseArmor)item).StrBonus * 5;

				price +=		(int)(((BaseArmor)item).SkillBonuses.Skill_1_Value * 2);
				price +=		(int)(((BaseArmor)item).SkillBonuses.Skill_2_Value * 2);
				price +=		(int)(((BaseArmor)item).SkillBonuses.Skill_3_Value * 2);
				price +=		(int)(((BaseArmor)item).SkillBonuses.Skill_4_Value * 2);
				price +=		(int)(((BaseArmor)item).SkillBonuses.Skill_5_Value * 2);

				price +=		((BaseArmor)item).Attributes.SpellChanneling * 200;
				price +=		((BaseArmor)item).Attributes.DefendChance * 10;
				price +=		((BaseArmor)item).Attributes.ReflectPhysical * 2;
				price +=		((BaseArmor)item).Attributes.AttackChance * 10;
				price +=		((BaseArmor)item).Attributes.RegenHits * 5;
				price +=		((BaseArmor)item).Attributes.RegenStam * 5;
				price +=		((BaseArmor)item).Attributes.RegenMana * 5;
				price +=		((BaseArmor)item).Attributes.NightSight * 6;
				price +=		((BaseArmor)item).Attributes.BonusHits * 5;
				price +=		((BaseArmor)item).Attributes.BonusStam * 5;
				price +=		((BaseArmor)item).Attributes.BonusMana * 5;

				int lmc = ((BaseArmor)item).Attributes.LowerManaCost;
					if ( ((BaseArmor)item).Attributes.LowerManaCost > MyServerSettings.LowerMana() )
						lmc = MyServerSettings.LowerMana();
							price += lmc * 5;

				int lrc = ((BaseArmor)item).Attributes.LowerRegCost;
					if ( ((BaseArmor)item).Attributes.LowerRegCost > MyServerSettings.LowerReg() )
						lrc = MyServerSettings.LowerReg();
							price += lrc * 5;

				price +=		((BaseArmor)item).Attributes.Luck * 2;
				price +=		((BaseArmor)item).Attributes.WeaponDamage * 5;
				price +=		((BaseArmor)item).Attributes.WeaponSpeed * 6;
				price +=		((BaseArmor)item).Attributes.BonusStr * 10;
				price +=		((BaseArmor)item).Attributes.BonusDex * 10;
				price +=		((BaseArmor)item).Attributes.BonusInt * 10;
				price +=		((BaseArmor)item).Attributes.EnhancePotions * 2;
				price +=		((BaseArmor)item).Attributes.CastSpeed * 4;
				price +=		((BaseArmor)item).Attributes.CastRecovery * 4;
				price +=		((BaseArmor)item).Attributes.SpellDamage * 4;

				SkillName skill;
				double bonus;

				((BaseArmor)item).SkillBonuses.GetValues( 0, out skill, out bonus ); price += (int)(bonus*10);
				((BaseArmor)item).SkillBonuses.GetValues( 1, out skill, out bonus ); price += (int)(bonus*10);
				((BaseArmor)item).SkillBonuses.GetValues( 2, out skill, out bonus ); price += (int)(bonus*10);
				((BaseArmor)item).SkillBonuses.GetValues( 3, out skill, out bonus ); price += (int)(bonus*10);
				((BaseArmor)item).SkillBonuses.GetValues( 4, out skill, out bonus ); price += (int)(bonus*10);
			}
			else if ( item is BaseWeapon )
			{
				price +=		((BaseWeapon)item).WeaponAttributes.SelfRepair * 100;
				price +=		((BaseWeapon)item).WeaponAttributes.LowerStatReq * 2;
				price +=		((BaseWeapon)item).WeaponAttributes.HitPhysicalArea * 3;
				price +=		((BaseWeapon)item).WeaponAttributes.HitFireArea * 3;
				price +=		((BaseWeapon)item).WeaponAttributes.HitColdArea * 3;
				price +=		((BaseWeapon)item).WeaponAttributes.HitPoisonArea * 3;
				price +=		((BaseWeapon)item).WeaponAttributes.HitEnergyArea * 3;
				price +=		((BaseWeapon)item).WeaponAttributes.HitMagicArrow * 3;
				price +=		((BaseWeapon)item).WeaponAttributes.HitHarm * 3;
				price +=		((BaseWeapon)item).WeaponAttributes.HitFireball * 3;
				price +=		((BaseWeapon)item).WeaponAttributes.HitLightning * 3;
				price +=		((BaseWeapon)item).WeaponAttributes.UseBestSkill * 10;
				price +=		((BaseWeapon)item).WeaponAttributes.MageWeapon * 5;
				price +=		((BaseWeapon)item).WeaponAttributes.HitDispel * 3;
				price +=		((BaseWeapon)item).WeaponAttributes.HitLeechHits * 3;
				price +=		((BaseWeapon)item).WeaponAttributes.HitLowerAttack * 3;
				price +=		((BaseWeapon)item).WeaponAttributes.HitLowerDefend * 3;
				price +=		((BaseWeapon)item).WeaponAttributes.HitLeechMana * 3;
				price +=		((BaseWeapon)item).WeaponAttributes.HitLeechStam * 3;
				price +=		((BaseWeapon)item).WeaponAttributes.ResistPhysicalBonus * 2;
				price +=		((BaseWeapon)item).WeaponAttributes.ResistFireBonus * 2;
				price +=		((BaseWeapon)item).WeaponAttributes.ResistColdBonus * 2;
				price +=		((BaseWeapon)item).WeaponAttributes.ResistPoisonBonus * 2;
				price +=		((BaseWeapon)item).WeaponAttributes.ResistEnergyBonus * 2;

				if ( ((BaseWeapon)item).Slayer != SlayerName.None )
					price +=		200;

				if ( ((BaseWeapon)item).Slayer2 != SlayerName.None )
					price +=		200;

				price +=		(int)(((BaseWeapon)item).SkillBonuses.Skill_1_Value * 2);
				price +=		(int)(((BaseWeapon)item).SkillBonuses.Skill_2_Value * 2);
				price +=		(int)(((BaseWeapon)item).SkillBonuses.Skill_3_Value * 2);
				price +=		(int)(((BaseWeapon)item).SkillBonuses.Skill_4_Value * 2);
				price +=		(int)(((BaseWeapon)item).SkillBonuses.Skill_5_Value * 2);

				if ( item is IPugilistGlove
				 || item is GiftThrowingGloves
				 || item is LevelThrowingGloves
				 || item is ThrowingGloves
				 || item is WizardWand
				 || item is WizardStaff
				 || item is WizardStick ){} else
				price +=		((BaseWeapon)item).Attributes.SpellChanneling * 200;

				price +=		((BaseWeapon)item).Attributes.DefendChance * 10;
				price +=		((BaseWeapon)item).Attributes.ReflectPhysical * 2;
				price +=		((BaseWeapon)item).Attributes.AttackChance * 10;
				price +=		((BaseWeapon)item).Attributes.RegenHits * 5;
				price +=		((BaseWeapon)item).Attributes.RegenStam * 5;
				price +=		((BaseWeapon)item).Attributes.RegenMana * 5;
				price +=		((BaseWeapon)item).Attributes.NightSight * 6;
				price +=		((BaseWeapon)item).Attributes.BonusHits * 5;
				price +=		((BaseWeapon)item).Attributes.BonusStam * 5;
				price +=		((BaseWeapon)item).Attributes.BonusMana * 5;

				int lmc = ((BaseWeapon)item).Attributes.LowerManaCost;
					if ( ((BaseWeapon)item).Attributes.LowerManaCost > MyServerSettings.LowerMana() )
						lmc = MyServerSettings.LowerMana();
							price += lmc * 5;

				int lrc = ((BaseWeapon)item).Attributes.LowerRegCost;
					if ( ((BaseWeapon)item).Attributes.LowerRegCost > MyServerSettings.LowerReg() )
						lrc = MyServerSettings.LowerReg();
							price += lrc * 5;

				price +=		((BaseWeapon)item).Attributes.Luck * 2;
				price +=		((BaseWeapon)item).Attributes.WeaponDamage * 5;
				price +=		((BaseWeapon)item).Attributes.WeaponSpeed * 6;
				price +=		((BaseWeapon)item).Attributes.BonusStr * 10;
				price +=		((BaseWeapon)item).Attributes.BonusDex * 10;
				price +=		((BaseWeapon)item).Attributes.BonusInt * 10;
				price +=		((BaseWeapon)item).Attributes.EnhancePotions * 2;
				price +=		((BaseWeapon)item).Attributes.CastSpeed * 4;
				price +=		((BaseWeapon)item).Attributes.CastRecovery * 4;
				price +=		((BaseWeapon)item).Attributes.SpellDamage * 4;

				SkillName skill;
				double bonus;

				((BaseWeapon)item).SkillBonuses.GetValues( 0, out skill, out bonus ); price += (int)(bonus*10);
				((BaseWeapon)item).SkillBonuses.GetValues( 1, out skill, out bonus ); price += (int)(bonus*10);
				((BaseWeapon)item).SkillBonuses.GetValues( 2, out skill, out bonus ); price += (int)(bonus*10);
				((BaseWeapon)item).SkillBonuses.GetValues( 3, out skill, out bonus ); price += (int)(bonus*10);
				((BaseWeapon)item).SkillBonuses.GetValues( 4, out skill, out bonus ); price += (int)(bonus*10);
			}
			else if ( item is Spellbook )
			{
				price +=		((Spellbook)item).Resistances.Physical * 2;
				price +=		((Spellbook)item).Resistances.Fire * 2;
				price +=		((Spellbook)item).Resistances.Cold * 2;
				price +=		((Spellbook)item).Resistances.Poison * 2;
				price +=		((Spellbook)item).Resistances.Energy * 2;

				if ( ((Spellbook)item).Slayer != SlayerName.None )
					price +=		200;

				if ( ((Spellbook)item).Slayer2 != SlayerName.None )
					price +=		200;

				price +=		(int)(((Spellbook)item).SkillBonuses.Skill_1_Value * 2);
				price +=		(int)(((Spellbook)item).SkillBonuses.Skill_2_Value * 2);
				price +=		(int)(((Spellbook)item).SkillBonuses.Skill_3_Value * 2);
				price +=		(int)(((Spellbook)item).SkillBonuses.Skill_4_Value * 2);
				price +=		(int)(((Spellbook)item).SkillBonuses.Skill_5_Value * 2);

				price +=		((Spellbook)item).Attributes.SpellChanneling * 200;
				price +=		((Spellbook)item).Attributes.DefendChance * 10;
				price +=		((Spellbook)item).Attributes.ReflectPhysical * 2;
				price +=		((Spellbook)item).Attributes.AttackChance * 10;
				price +=		((Spellbook)item).Attributes.RegenHits * 5;
				price +=		((Spellbook)item).Attributes.RegenStam * 5;
				price +=		((Spellbook)item).Attributes.RegenMana * 5;
				price +=		((Spellbook)item).Attributes.NightSight * 6;
				price +=		((Spellbook)item).Attributes.BonusHits * 5;
				price +=		((Spellbook)item).Attributes.BonusStam * 5;
				price +=		((Spellbook)item).Attributes.BonusMana * 5;

				int lmc = ((Spellbook)item).Attributes.LowerManaCost;
					if ( ((Spellbook)item).Attributes.LowerManaCost > MyServerSettings.LowerMana() )
						lmc = MyServerSettings.LowerMana();
							price += lmc * 5;

				int lrc = ((Spellbook)item).Attributes.LowerRegCost;
					if ( ((Spellbook)item).Attributes.LowerRegCost > MyServerSettings.LowerReg() )
						lrc = MyServerSettings.LowerReg();
							price += lrc * 5;

				price +=		((Spellbook)item).Attributes.Luck * 2;
				price +=		((Spellbook)item).Attributes.WeaponDamage * 5;
				price +=		((Spellbook)item).Attributes.WeaponSpeed * 6;
				price +=		((Spellbook)item).Attributes.BonusStr * 10;
				price +=		((Spellbook)item).Attributes.BonusDex * 10;
				price +=		((Spellbook)item).Attributes.BonusInt * 10;
				price +=		((Spellbook)item).Attributes.EnhancePotions * 2;
				price +=		((Spellbook)item).Attributes.CastSpeed * 4;
				price +=		((Spellbook)item).Attributes.CastRecovery * 4;
				price +=		((Spellbook)item).Attributes.SpellDamage * 4;

				SkillName skill;
				double bonus;

				((Spellbook)item).SkillBonuses.GetValues( 0, out skill, out bonus ); price += (int)(bonus*10);
				((Spellbook)item).SkillBonuses.GetValues( 1, out skill, out bonus ); price += (int)(bonus*10);
				((Spellbook)item).SkillBonuses.GetValues( 2, out skill, out bonus ); price += (int)(bonus*10);
				((Spellbook)item).SkillBonuses.GetValues( 3, out skill, out bonus ); price += (int)(bonus*10);
				((Spellbook)item).SkillBonuses.GetValues( 4, out skill, out bonus ); price += (int)(bonus*10);

				int spells = ((Spellbook)item).SpellCount;
				int pageCt = 0;

				if ( ((Spellbook)item).MageryBook() )
				{
					while ( spells > 0 )
					{
						spells--;
						pageCt++;

						if ( pageCt < 9 ){			price += 10; }
						else if ( pageCt < 17 ){	price += 20; }
						else if ( pageCt < 25 ){	price += 30; }
						else if ( pageCt < 33 ){	price += 40; }
						else if ( pageCt < 41 ){	price += 50; }
						else if ( pageCt < 49 ){	price += 60; }
						else if ( pageCt < 57 ){	price += 70; }
						else {						price += 80; }
					}
				}
				else if ( item is NecromancerSpellbook )
				{
					while ( spells > 0 )
					{
						spells--;
						pageCt++;

						if ( pageCt == 1 ){			price += 12; }
						else if ( pageCt == 2 ){	price += 24; }
						else if ( pageCt == 3 ){	price += 28; }
						else if ( pageCt == 4 ){	price += 52; }
						else if ( pageCt == 5 ){	price += 52; }
						else if ( pageCt == 6 ){	price += 52; }
						else if ( pageCt == 7 ){	price += 52; }
						else if ( pageCt == 8 ){	price += 54; }
						else if ( pageCt == 9 ){	price += 78; }
						else if ( pageCt == 10 ){	price += 78; }
						else if ( pageCt == 11 ){	price += 102; }
						else if ( pageCt == 12 ){	price += 128; }
						else if ( pageCt == 13 ){	price += 128; }
						else if ( pageCt == 14 ){	price += 128; }
						else if ( pageCt == 15 ){	price += 144; }
						else if ( pageCt == 16 ){	price += 202; }
						else {						price += 228; }
					}
				}
				else if ( item is ElementalSpellbook )
				{
					while ( spells > 0 )
					{
						spells--;
						pageCt++;

						if ( pageCt < 5 ){			price += 10; }
						else if ( pageCt < 9 ){		price += 16; }
						else if ( pageCt < 13 ){	price += 22; }
						else if ( pageCt < 17 ){	price += 28; }
						else if ( pageCt < 21 ){	price += 34; }
						else if ( pageCt < 25 ){	price += 40; }
						else if ( pageCt < 29 ){	price += 46; }
						else {						price += 52; }
					}
				}
				else if ( item is MysticSpellbook )
				{
					while ( spells > 0 )
					{
						spells--;
						pageCt++;

						price += pageCt * pageCt;
					}
				}
				else if ( item is SongBook )
				{
					while ( spells > 0 )
					{
						spells--;
						pageCt++;

						if ( pageCt < 3 )
							price += 12;
						else if ( pageCt < 6 )
							price += 14;
						else if ( pageCt < 9 )
							price += 16;
						else if ( pageCt < 10 )
							price += 20;
						else
							price += 32;
					}
				}
			}
			else if ( item is BaseInstrument )
			{
				price +=		((BaseInstrument)item).Resistances.Physical * 2;
				price +=		((BaseInstrument)item).Resistances.Fire * 2;
				price +=		((BaseInstrument)item).Resistances.Cold * 2;
				price +=		((BaseInstrument)item).Resistances.Poison * 2;
				price +=		((BaseInstrument)item).Resistances.Energy * 2;

				if ( ((BaseInstrument)item).Slayer != SlayerName.None )
					price +=		200;

				if ( ((BaseInstrument)item).Slayer2 != SlayerName.None )
					price +=		200;

				price +=		(int)(((BaseInstrument)item).SkillBonuses.Skill_1_Value * 2);
				price +=		(int)(((BaseInstrument)item).SkillBonuses.Skill_2_Value * 2);
				price +=		(int)(((BaseInstrument)item).SkillBonuses.Skill_3_Value * 2);
				price +=		(int)(((BaseInstrument)item).SkillBonuses.Skill_4_Value * 2);
				price +=		(int)(((BaseInstrument)item).SkillBonuses.Skill_5_Value * 2);

				price +=		((BaseInstrument)item).Attributes.SpellChanneling * 200;
				price +=		((BaseInstrument)item).Attributes.DefendChance * 10;
				price +=		((BaseInstrument)item).Attributes.ReflectPhysical * 2;
				price +=		((BaseInstrument)item).Attributes.AttackChance * 10;
				price +=		((BaseInstrument)item).Attributes.RegenHits * 5;
				price +=		((BaseInstrument)item).Attributes.RegenStam * 5;
				price +=		((BaseInstrument)item).Attributes.RegenMana * 5;
				price +=		((BaseInstrument)item).Attributes.NightSight * 6;
				price +=		((BaseInstrument)item).Attributes.BonusHits * 5;
				price +=		((BaseInstrument)item).Attributes.BonusStam * 5;
				price +=		((BaseInstrument)item).Attributes.BonusMana * 5;

				int lmc = ((BaseInstrument)item).Attributes.LowerManaCost;
					if ( ((BaseInstrument)item).Attributes.LowerManaCost > MyServerSettings.LowerMana() )
						lmc = MyServerSettings.LowerMana();
							price += lmc * 5;

				int lrc = ((BaseInstrument)item).Attributes.LowerRegCost;
					if ( ((BaseInstrument)item).Attributes.LowerRegCost > MyServerSettings.LowerReg() )
						lrc = MyServerSettings.LowerReg();
							price += lrc * 5;

				price +=		((BaseInstrument)item).Attributes.Luck * 2;
				price +=		((BaseInstrument)item).Attributes.WeaponDamage * 5;
				price +=		((BaseInstrument)item).Attributes.WeaponSpeed * 6;
				price +=		((BaseInstrument)item).Attributes.BonusStr * 10;
				price +=		((BaseInstrument)item).Attributes.BonusDex * 10;
				price +=		((BaseInstrument)item).Attributes.BonusInt * 10;
				price +=		((BaseInstrument)item).Attributes.EnhancePotions * 2;
				price +=		((BaseInstrument)item).Attributes.CastSpeed * 4;
				price +=		((BaseInstrument)item).Attributes.CastRecovery * 4;
				price +=		((BaseInstrument)item).Attributes.SpellDamage * 4;

				SkillName skill;
				double bonus;

				((BaseInstrument)item).SkillBonuses.GetValues( 0, out skill, out bonus ); price += (int)(bonus*10);
				((BaseInstrument)item).SkillBonuses.GetValues( 1, out skill, out bonus ); price += (int)(bonus*10);
				((BaseInstrument)item).SkillBonuses.GetValues( 2, out skill, out bonus ); price += (int)(bonus*10);
				((BaseInstrument)item).SkillBonuses.GetValues( 3, out skill, out bonus ); price += (int)(bonus*10);
				((BaseInstrument)item).SkillBonuses.GetValues( 4, out skill, out bonus ); price += (int)(bonus*10);
			}
			else if ( item is BaseTrinket )
			{
				price +=		((BaseTrinket)item).Resistances.Physical * 2;
				price +=		((BaseTrinket)item).Resistances.Fire * 2;
				price +=		((BaseTrinket)item).Resistances.Cold * 2;
				price +=		((BaseTrinket)item).Resistances.Poison * 2;
				price +=		((BaseTrinket)item).Resistances.Energy * 2;

				price +=		(int)(((BaseTrinket)item).SkillBonuses.Skill_1_Value * 2);
				price +=		(int)(((BaseTrinket)item).SkillBonuses.Skill_2_Value * 2);
				price +=		(int)(((BaseTrinket)item).SkillBonuses.Skill_3_Value * 2);
				price +=		(int)(((BaseTrinket)item).SkillBonuses.Skill_4_Value * 2);
				price +=		(int)(((BaseTrinket)item).SkillBonuses.Skill_5_Value * 2);

				price +=		((BaseTrinket)item).Attributes.SpellChanneling * 200;
				price +=		((BaseTrinket)item).Attributes.DefendChance * 10;
				price +=		((BaseTrinket)item).Attributes.ReflectPhysical * 2;
				price +=		((BaseTrinket)item).Attributes.AttackChance * 10;
				price +=		((BaseTrinket)item).Attributes.RegenHits * 5;
				price +=		((BaseTrinket)item).Attributes.RegenStam * 5;
				price +=		((BaseTrinket)item).Attributes.RegenMana * 5;
				price +=		((BaseTrinket)item).Attributes.NightSight * 6;
				price +=		((BaseTrinket)item).Attributes.BonusHits * 5;
				price +=		((BaseTrinket)item).Attributes.BonusStam * 5;
				price +=		((BaseTrinket)item).Attributes.BonusMana * 5;

				int lmc = ((BaseTrinket)item).Attributes.LowerManaCost;
					if ( ((BaseTrinket)item).Attributes.LowerManaCost > MyServerSettings.LowerMana() )
						lmc = MyServerSettings.LowerMana();
							price += lmc * 5;

				int lrc = ((BaseTrinket)item).Attributes.LowerRegCost;
					if ( ((BaseTrinket)item).Attributes.LowerRegCost > MyServerSettings.LowerReg() )
						lrc = MyServerSettings.LowerReg();
							price += lrc * 5;

				price +=		((BaseTrinket)item).Attributes.Luck * 2;
				price +=		((BaseTrinket)item).Attributes.WeaponDamage * 5;
				price +=		((BaseTrinket)item).Attributes.WeaponSpeed * 6;
				price +=		((BaseTrinket)item).Attributes.BonusStr * 10;
				price +=		((BaseTrinket)item).Attributes.BonusDex * 10;
				price +=		((BaseTrinket)item).Attributes.BonusInt * 10;
				price +=		((BaseTrinket)item).Attributes.EnhancePotions * 2;
				price +=		((BaseTrinket)item).Attributes.CastSpeed * 4;
				price +=		((BaseTrinket)item).Attributes.CastRecovery * 4;
				price +=		((BaseTrinket)item).Attributes.SpellDamage * 4;

				SkillName skill;
				double bonus;

				((BaseTrinket)item).SkillBonuses.GetValues( 0, out skill, out bonus ); price += (int)(bonus*10);
				((BaseTrinket)item).SkillBonuses.GetValues( 1, out skill, out bonus ); price += (int)(bonus*10);
				((BaseTrinket)item).SkillBonuses.GetValues( 2, out skill, out bonus ); price += (int)(bonus*10);
				((BaseTrinket)item).SkillBonuses.GetValues( 3, out skill, out bonus ); price += (int)(bonus*10);
				((BaseTrinket)item).SkillBonuses.GetValues( 4, out skill, out bonus ); price += (int)(bonus*10);
			}
			else if ( item is BaseClothing )
			{
				price +=		((BaseClothing)item).Resistances.Physical * 2;
				price +=		((BaseClothing)item).Resistances.Fire * 2;
				price +=		((BaseClothing)item).Resistances.Cold * 2;
				price +=		((BaseClothing)item).Resistances.Poison * 2;
				price +=		((BaseClothing)item).Resistances.Energy * 2;

				price +=		(int)(((BaseClothing)item).SkillBonuses.Skill_1_Value * 2);
				price +=		(int)(((BaseClothing)item).SkillBonuses.Skill_2_Value * 2);
				price +=		(int)(((BaseClothing)item).SkillBonuses.Skill_3_Value * 2);
				price +=		(int)(((BaseClothing)item).SkillBonuses.Skill_4_Value * 2);
				price +=		(int)(((BaseClothing)item).SkillBonuses.Skill_5_Value * 2);

				price +=		((BaseClothing)item).Attributes.SpellChanneling * 200;
				price +=		((BaseClothing)item).Attributes.DefendChance * 10;
				price +=		((BaseClothing)item).Attributes.ReflectPhysical * 2;
				price +=		((BaseClothing)item).Attributes.AttackChance * 10;
				price +=		((BaseClothing)item).Attributes.RegenHits * 5;
				price +=		((BaseClothing)item).Attributes.RegenStam * 5;
				price +=		((BaseClothing)item).Attributes.RegenMana * 5;
				price +=		((BaseClothing)item).Attributes.NightSight * 6;
				price +=		((BaseClothing)item).Attributes.BonusHits * 5;
				price +=		((BaseClothing)item).Attributes.BonusStam * 5;
				price +=		((BaseClothing)item).Attributes.BonusMana * 5;

				int lmc = ((BaseClothing)item).Attributes.LowerManaCost;
					if ( ((BaseClothing)item).Attributes.LowerManaCost > MyServerSettings.LowerMana() )
						lmc = MyServerSettings.LowerMana();
							price += lmc * 5;

				int lrc = ((BaseClothing)item).Attributes.LowerRegCost;
					if ( ((BaseClothing)item).Attributes.LowerRegCost > MyServerSettings.LowerReg() )
						lrc = MyServerSettings.LowerReg();
							price += lrc * 5;

				price +=		((BaseClothing)item).Attributes.Luck * 2;
				price +=		((BaseClothing)item).Attributes.WeaponDamage * 5;
				price +=		((BaseClothing)item).Attributes.WeaponSpeed * 6;
				price +=		((BaseClothing)item).Attributes.BonusStr * 10;
				price +=		((BaseClothing)item).Attributes.BonusDex * 10;
				price +=		((BaseClothing)item).Attributes.BonusInt * 10;
				price +=		((BaseClothing)item).Attributes.EnhancePotions * 2;
				price +=		((BaseClothing)item).Attributes.CastSpeed * 4;
				price +=		((BaseClothing)item).Attributes.CastRecovery * 4;
				price +=		((BaseClothing)item).Attributes.SpellDamage * 4;

				SkillName skill;
				double bonus;

				((BaseClothing)item).SkillBonuses.GetValues( 0, out skill, out bonus ); price += (int)(bonus*10);
				((BaseClothing)item).SkillBonuses.GetValues( 1, out skill, out bonus ); price += (int)(bonus*10);
				((BaseClothing)item).SkillBonuses.GetValues( 2, out skill, out bonus ); price += (int)(bonus*10);
				((BaseClothing)item).SkillBonuses.GetValues( 3, out skill, out bonus ); price += (int)(bonus*10);
				((BaseClothing)item).SkillBonuses.GetValues( 4, out skill, out bonus ); price += (int)(bonus*10);
			}
			else if ( item is BaseQuiver )
			{
				price +=		((BaseQuiver)item).LowerAmmoCost * 5;
				price +=		((BaseQuiver)item).WeightReduction * 2;

				price +=		((BaseQuiver)item).Attributes.SpellChanneling * 200;
				price +=		((BaseQuiver)item).Attributes.DefendChance * 10;
				price +=		((BaseQuiver)item).Attributes.ReflectPhysical * 2;
				price +=		((BaseQuiver)item).Attributes.AttackChance * 10;
				price +=		((BaseQuiver)item).Attributes.RegenHits * 5;
				price +=		((BaseQuiver)item).Attributes.RegenStam * 5;
				price +=		((BaseQuiver)item).Attributes.RegenMana * 5;
				price +=		((BaseQuiver)item).Attributes.NightSight * 6;
				price +=		((BaseQuiver)item).Attributes.BonusHits * 5;
				price +=		((BaseQuiver)item).Attributes.BonusStam * 5;
				price +=		((BaseQuiver)item).Attributes.BonusMana * 5;

				int lmc = ((BaseQuiver)item).Attributes.LowerManaCost;
					if ( ((BaseQuiver)item).Attributes.LowerManaCost > MyServerSettings.LowerMana() )
						lmc = MyServerSettings.LowerMana();
							price += lmc * 5;

				int lrc = ((BaseQuiver)item).Attributes.LowerRegCost;
					if ( ((BaseQuiver)item).Attributes.LowerRegCost > MyServerSettings.LowerReg() )
						lrc = MyServerSettings.LowerReg();
							price += lrc * 5;

				price +=		((BaseQuiver)item).Attributes.Luck * 2;
				price +=		((BaseQuiver)item).Attributes.WeaponDamage * 5;
				price +=		((BaseQuiver)item).Attributes.WeaponSpeed * 6;
				price +=		((BaseQuiver)item).Attributes.BonusStr * 10;
				price +=		((BaseQuiver)item).Attributes.BonusDex * 10;
				price +=		((BaseQuiver)item).Attributes.BonusInt * 10;
				price +=		((BaseQuiver)item).Attributes.EnhancePotions * 2;
				price +=		((BaseQuiver)item).Attributes.CastSpeed * 4;
				price +=		((BaseQuiver)item).Attributes.CastRecovery * 4;
				price +=		((BaseQuiver)item).Attributes.SpellDamage * 4;
			}

			return price;
		}

		public static int GetSellPrice( int val, bool guild )
		{
			ItemSalesInfo info = GetData( val );

			if ( info == null )
				return 0;

			if ( MyServerSettings.HigherPrice() > 0 )
				return (int)(info.iPrice + ( info.iPrice * MyServerSettings.HigherPrice() ));

			if ( info.iCategory == ItemSalesInfo.Category.Resource && MyServerSettings.ResourcePrice() > 0 )
				return (int)(info.iPrice + ( info.iPrice * MyServerSettings.ResourcePrice() ));

			if ( guild )
				return info.iPrice;

			return Utility.RandomMinMax( info.iPrice, (int)(info.iPrice*1.4) );
		}

		public static int GetBuysPrice( int val, bool guild, Item item, bool fluctuate, bool resale )
		{
			if ( val < 0 )
				return 0;

			ItemSalesInfo info = GetData( val );
			if ( info == null )
				return 0;

			int price = info.iPrice;

			if ( item != null )
				price = AddUpBenefits( item, price, false, resale );

			if ( info.iPrice >= 5000 && item != null && item.ArtifactLevel > (int)ArtifactLevel.None && resale )
				price = (int)(price * 3);
			else if ( resale && item != null && item is BaseClothing )
				price = (int)(price * 15);
			else if ( resale && item != null && item is Spellbook )
				price = (int)(price * 5);
			else if ( resale )
				price = (int)(price * 10);
			else
				price = (int)(price / 2);

			if ( !guild && fluctuate )
				price = Utility.RandomMinMax( (int)( price * 0.6 ), price );

			if ( price < 1 )
				price = 1;

			if ( MySettings.S_SellGoldCutRate > 0 )
			{
				price = (int)(price - ( price * MyServerSettings.SellGoldCutRate() ));
					if ( price < 1 )
						price = 1;
			}

			return price;
		}

		public static int GetQty( int val, bool guild )
		{
			int qty = 0;
			int rar = 0;
			ItemSalesInfo.Category tCategory = ItemSalesInfo.Category.None;

			ItemSalesInfo info = GetData( val );
			if ( info != null )
			{
				qty = info.iQty;
				rar = info.iRarity;
				tCategory = info.iCategory;
			}

			if ( rar == 200 )
				qty = 0;
			else if ( rar > 100 && !guild )
				qty = 0;
			else if ( guild )
				qty = qty * 10;

			if ( Utility.RandomMinMax( 1, 100 ) < rar && rar < 101 && rar > 0 )
				qty = 0;

			if ( qty < 1 )
				return 0;

			qty = Utility.RandomMinMax( 1, qty );

			if ( ( tCategory == ItemSalesInfo.Category.Resource || tCategory == ItemSalesInfo.Category.Reagent ) )
			{
				if ( qty > 0 )
				{
					qty = Utility.RandomMinMax( 10, 50 );

					if ( ( guild || MySettings.S_SoldResource ) && qty > 0 )
						qty = Utility.RandomMinMax( 100,850 );
				}

				if ( qty < 1 )
					qty = 0;
			}

			return qty;
		}

		public static bool WillDeal( int val, Mobile m, bool selling, bool blackMarket, ItemSalesInfo.World world, bool guild )
		{
			if ( MySettings.S_NoBuyResources && iCategory(val) == ItemSalesInfo.Category.Resource )
				return false;

			if ( blackMarket )
				return Utility.RandomBool();

			if ( MySettings.S_SellAll && selling )
				return true;

			if ( MySettings.S_BuyAll && !selling )
				return true;

			if ( guild )
				return true;

			// These areas only sell a couple of items so let them always sell
			if ( ( world == ItemSalesInfo.World.Ambrosia || world == ItemSalesInfo.World.Elf ) && WorldTest( val, m, world ) )
				return true;

			if ( Utility.RandomMinMax(1,10) > 4 )
				return true;

			return false;
		}

		public static void GetSellList( Mobile m, List<GenericBuyInfo> LIST, ItemSalesInfo.Category v_Category, ItemSalesInfo.Material v_Material, ItemSalesInfo.Market v_Market, ItemSalesInfo.World v_World, Type specificType )
		{
			ItemSalesInfo[] list = ItemSalesInfo.m_SellingInfo;

			bool v_Guild = false;
				if ( m is BaseGuildmaster )
					v_Guild = true;

			int entries = list.Length;
			int val = 0;
			int price = 0;
			int qty = 0;
			bool chemist = false;
			bool set = false;

			Item oItem = null;
			int oItemID = 0;
			int oHue = 0;
			string oName = null;

			string CurrentMonth = DateTime.Now.ToString("MM");

			Drinks( LIST, v_Market );

			while ( entries > 0 )
			{
				Type itemType = list[val].ItemsType;

				if ( itemType != null )
				{
					set = false;
					oItem = null;
					oItemID = 0;
					oHue = 0;
					oName = null;

					chemist = Chemist( val, v_Market, v_Category );

					if ( ( specificType != null && itemType == specificType ) || ( iSells(val) && v_Market == iMarket(val) && v_Category == iCategory(val) ) )
					{
						set = true;
					}
					else if
					(
						WillDeal( val, m, true, false, iWorld(val), v_Guild ) && itemType != null && specificType == null && 
						( chemist ||
						(
						( !chemist ) && 
						( v_Category == iCategory(val) || v_Category == ItemSalesInfo.Category.All ) && 
						( v_Material == iMaterial(val) || v_Material == ItemSalesInfo.Material.All ) && 
						( v_Market == iMarket(val) || v_Market == ItemSalesInfo.Market.All ) && 
						( v_World == iWorld(val) || WorldTest( val, m, v_World ) )
						)
						)
					)
					{
						set = true;
					}

					if ( CurrentMonth != "12" && iCategory(val) == ItemSalesInfo.Category.Christmas )
						set = false;

					if ( CurrentMonth != "10" && iCategory(val) == ItemSalesInfo.Category.Halloween )
						set = false;

					if ( v_Market == ItemSalesInfo.Market.Sage && v_Category == ItemSalesInfo.Category.Artifact && iMarket(val) != ItemSalesInfo.Market.Thief && iCategory(val) == ItemSalesInfo.Category.Artifact )
					{
						// This section is just for the sage to display artifacts that cannot be bought.
						oItem = (Item)Activator.CreateInstance( itemType );

						if ( oItem != null )
						{
							oItemID = oItem.ItemID;
							oHue = oItem.Hue;
							oName = oItem.Name;
							oItem.Delete();				

							if ( !LIST.Contains( new GenericBuyInfo( oName, itemType, 0, 1, oItemID, oHue ) ) )
								LIST.Add( new GenericBuyInfo( oName, itemType, 0, 1, oItemID, oHue ) );
						}
					}
					else if ( set )
					{
						qty = GetQty( val, v_Guild );

						// These areas only sell a couple of items so let them always sell at least 1
						if ( qty < 1 && ( iWorld(val) == ItemSalesInfo.World.Ambrosia || iWorld(val) == ItemSalesInfo.World.Elf ) )
							qty = 1;

						price = GetSellPrice( val, v_Guild );

						if ( !MySettings.S_LawnsAllowed && itemType == typeof( LawnTools ) )
							qty = 0;
						else if ( qty < 0 )
							qty = 0;
						else if ( !MySettings.S_ShantysAllowed && itemType == typeof( ShantyTools ) )
							qty = 0;
						else if ( !MySettings.S_Basements && itemType == typeof( BasementDoor ) )
							qty = 0;

						if ( qty > 0 )
						{
							oItem = (Item)Activator.CreateInstance( itemType );

							if ( oItem != null )
							{
								oItemID = oItem.ItemID;
								ResourceMods.DefaultItemHue( oItem );
								oHue = oItem.Hue;
									oHue = ClothHue( oHue, iMaterial(val), iMarket(val) );
								oName = oItem.Name;
								oItem.Delete();

								if ( !LIST.Contains( new GenericBuyInfo( oName, itemType, price, qty, oItemID, oHue ) ) )
									LIST.Add( new GenericBuyInfo( oName, itemType, price, qty, oItemID, oHue ) );
							}
						}
					}
				}
				entries--;
				val++;
			}

		}

		public static void GetBuysList( Mobile m, GenericSellInfo LIST, ItemSalesInfo.Category v_Category, ItemSalesInfo.Material v_Material, ItemSalesInfo.Market v_Market, ItemSalesInfo.World v_World, Type specificType, bool force = false )
		{
			ItemSalesInfo[] list = ItemSalesInfo.m_SellingInfo;

			bool v_Guild = false;
				if ( m is BaseGuildmaster )
					v_Guild = true;

			int entries = list.Length;
			int val = 0;
			int price = 0;
			bool chemist = false;
			bool set = false;

			if ( v_Market == ItemSalesInfo.Market.Cartographer && !(LIST.IsInList( typeof( PresetMapEntry ) ) ) )
				LIST.Add( typeof( PresetMapEntry ), 3 );

			while ( entries > 0 )
			{
				Type itemType = list[val].ItemsType;

				if ( itemType != null )
				{
					set = false;
					chemist = Chemist( val, v_Market, v_Category );

					if ( force || ( ( specificType != null && itemType == specificType ) || ( iRarity(val) == 200 && v_Market == iMarket(val) ) ) || ( iBuys(val) && v_Market == iMarket(val) && v_Category == iCategory(val) ) )
					{
						set = true;
					}
					else if
					(
						WillDeal( val, m, false, false, iWorld(val), v_Guild ) && list[val].ItemsType != null && specificType == null && 
						( chemist ||
						(
						( !chemist ) && 
						( v_Category == iCategory(val) || v_Category == ItemSalesInfo.Category.All ) && 
						( v_Material == iMaterial(val) || v_Material == ItemSalesInfo.Material.All ) && 
						( v_Market == iMarket(val) || v_Market == ItemSalesInfo.Market.All )
						)
						)
					)
					{
						set = true;
					}

					if ( !SetAllowedSell( iCategory(val), list[val].ItemsType ) )
						set = false;

					if ( set )
					{
						price = GetBuysPrice( val, v_Guild, null, !force, false );

						if ( LIST.IsInList( list[val].ItemsType ) )
							price = 0;

						if ( price > 0 )
							LIST.Add( list[val].ItemsType, price );
					}
				}
				entries--;
				val++;
			}
		}

		public static int ItemTableRef( Item item )
		{
			ItemSalesInfo[] list = ItemSalesInfo.m_SellingInfo;
			int entries = list.Length;
			int val = 0;

			while ( entries > 0 )
			{
				Type itemType = list[val].ItemsType;

				if ( itemType == item.GetType() )
					return val;

				entries--;
				val++;
			}

			return 0;
		}

		public static void BlackMarketList( Mobile m, ItemSalesInfo.Category v_Category, ItemSalesInfo.Material v_Material, ItemSalesInfo.Market v_Market, ItemSalesInfo.World v_World )
		{
			BlackMarketList( m, v_Category, v_Material, v_Market, v_World, null );
		}

		public static void BlackMarketList( Mobile m, ItemSalesInfo.Category v_Category, ItemSalesInfo.Material v_Material, ItemSalesInfo.Market v_Market, ItemSalesInfo.World v_World, Type specificType )
		{
			ItemSalesInfo[] list = ItemSalesInfo.m_SellingInfo;

			int entries = list.Length;
			int val = 0;
			int price = 0;
			bool set = false;
			bool skip = false;

			while ( entries > 0 )
			{
				Type itemType = list[val].ItemsType;

				if ( itemType != null )
				{
					set = false;
					skip = false;

					if ( specificType != null && itemType == specificType && WillDeal( val, m, true, true, iWorld(val), false ) )
					{
						set = true;
					}
					else if
					(
						WillDeal( val, m, true, true, iWorld(val), false ) && list[val].ItemsType != null && specificType == null && 
						(
						( v_Category == iCategory(val) || v_Category == ItemSalesInfo.Category.All ) && 
						( v_Material == iMaterial(val) || v_Material == ItemSalesInfo.Material.All ) && 
						( v_Market == iMarket(val) || v_Market == ItemSalesInfo.Market.All ) && 
						( v_World == iWorld(val) || WorldTest( val, m, v_World ) )
						)
					)
						set = true;

					if ( set )
					{
						price = GetBuysPrice( val, false, null, true, false );

						if ( price > 0 )
						{
							Item product = null;

							if ( itemType == typeof( MagicalWand ) )
								product = new MagicalWand( Utility.RandomList(8,7,7,6,6,6,5,5,5,5,4,4,4,4,4,3,3,3,3,3,3,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1) );
							else
								product = (Item)Activator.CreateInstance( list[val].ItemsType );

							if ( !(product is BaseInstrument) && v_Market == ItemSalesInfo.Market.Bard )
								skip = true;
							else if ( !(product is BaseClothing) && v_Material == ItemSalesInfo.Material.Cloth )
								skip = true;
							else if ( CraftResources.GetType( product.Resource ) == CraftResourceType.Leather && v_Material == ItemSalesInfo.Material.Cloth )
								skip = true;
							else if ( product is BaseRanged && v_Material == ItemSalesInfo.Material.Wood && v_Market != ItemSalesInfo.Market.Bow )
								skip = true;

							if ( skip )
							{
								product.Delete();
							}
							else
							{
								if ( CraftResources.GetType( product.Resource ) == CraftResourceType.Metal && Utility.Random(20) == 0 )
									product.Resource = CraftResource.AmethystBlock;
								else if ( CraftResources.GetType( product.Resource ) == CraftResourceType.Leather && Utility.Random(20) == 0 )
									product.Resource = CraftResource.DemonSkin;

								ResourceMods.SetRandomResource( false, false, product, product.Resource, true, m );

								if ( product is BaseTrinket && v_Market == ItemSalesInfo.Market.Jeweler )
									BaseTrinket.RandomGem( (BaseTrinket)product );

								if ( Item.IsStandardResource( product.Resource ) && !(product is MagicalWand) )
									product.Delete();
								else
									m.BankBox.DropItem( product );
							}
						}
					}
				}
				entries--;
				val++;
			}
		}

		public static int ClothHue ( int hue, ItemSalesInfo.Material v_Material, ItemSalesInfo.Market v_Market )
		{
			if ( v_Material == ItemSalesInfo.Material.Cloth )
			{
				if ( v_Market == ItemSalesInfo.Market.Sailor )
					return Utility.RandomDyedHue();
				if ( v_Market == ItemSalesInfo.Market.Tailor )
					return Utility.RandomDyedHue();
				if ( v_Market == ItemSalesInfo.Market.Wizard )
					return Utility.RandomDyedHue();
			}

			return hue;
		}

		public static bool SetAllowedSell( ItemSalesInfo.Category v_Category, Type itemType )
		{
			if ( v_Category == ItemSalesInfo.Category.MonsterRace && MySettings.S_MonsterCharacters < 1 )
				return false;

			if ( !MySettings.S_BuyCloth )
			{
				if ( itemType == typeof( SpoolOfThread ) )
					return false;
				if ( itemType == typeof( Flax ) )
					return false;
				if ( itemType == typeof( Cotton ) )
					return false;
				if ( itemType == typeof( Wool ) )
					return false;
				if ( itemType == typeof( Fabric ) )
					return false;
			}

			return true;
		}

		public static bool WorldTest( int val, Mobile m, ItemSalesInfo.World world )
		{
			Region reg = Region.Find( m.Location, m.Map );

			ItemSalesInfo.World area = ItemSalesInfo.World.None;
			ItemSalesInfo info = GetData( val );
			if ( info != null )
				area = info.iWorld;

			if ( world == ItemSalesInfo.World.Orient || area == ItemSalesInfo.World.Orient )
				return false;
			if ( area == ItemSalesInfo.World.None )
				return true;
			if ( reg.IsPartOf( "the Enchanted Pass" ) && area == ItemSalesInfo.World.Elf )
				return true;
			if ( Worlds.isHauntedRegion( m ) && area == ItemSalesInfo.World.Necro )
				return true;
			if ( ( Worlds.IsSeaDungeon( m.Location, m.Map ) || Worlds.IsWaterSea( m ) ) && area == ItemSalesInfo.World.Sea )
				return true;
			if ( m.Land == Land.Lodoria && area == ItemSalesInfo.World.Lodor )
				return true;
			if ( m.Land == Land.Sosaria && area == ItemSalesInfo.World.Sosaria )
				return true;
			if ( m.Land == Land.Underworld && area == ItemSalesInfo.World.Underworld )
				return true;
			if ( m.Land == Land.Serpent && area == ItemSalesInfo.World.Serpent )
				return true;
			if ( m.Land == Land.IslesDread && area == ItemSalesInfo.World.Dread )
				return true;
			if ( m.Land == Land.Savaged && area == ItemSalesInfo.World.Savage )
				return true;
			if ( m.Land == Land.Ambrosia && area == ItemSalesInfo.World.Ambrosia )
				return true;
			if ( m.Land == Land.UmberVeil && area == ItemSalesInfo.World.Umber )
				return true;

			return false;
		}

		public static void Drinks( List<GenericBuyInfo> LIST, ItemSalesInfo.Market market )
		{
			int d1 = Utility.Random(3);		int x1 = Utility.Random(5);
			int d2 = Utility.Random(3);		int x2 = Utility.Random(5);
			int d3 = Utility.Random(3);		int x3 = Utility.Random(5);
			int d4 = Utility.Random(3);		int x4 = Utility.Random(5);
			int d5 = Utility.Random(3);		int x5 = Utility.Random(5);
			int d6 = Utility.Random(3);		int x6 = Utility.Random(5);

			if ( market == ItemSalesInfo.Market.Tavern )
			{
				if ( d1 == 0 ){ LIST.Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Ale, 4+x1, Utility.Random( 1,15 ), 0x282A, 0x83b ) ); }
				if ( d2 == 0 ){ LIST.Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Cider, 4+x2, Utility.Random( 1,15 ), 0x282A, 0x981 ) ); }
				if ( d3 == 0 ){ LIST.Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Liquor, 4+x3, Utility.Random( 1,15 ), 0x282A, 0xB51 ) ); }
				if ( d4 == 0 ){ LIST.Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Wine, 4+x4, Utility.Random( 1,15 ), 0x282A, 0xB64 ) ); }
			}

			if ( market == ItemSalesInfo.Market.Farmer || market == ItemSalesInfo.Market.Cook || market == ItemSalesInfo.Market.Tavern )
				{ if ( d5 == 0 ){ LIST.Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Milk, 4+x5, Utility.Random( 1,15 ), 0x282A, 0x9A3 ) ); } }

			if ( market == ItemSalesInfo.Market.Tavern || market == ItemSalesInfo.Market.Provisions )
				{ if ( d6 == 0 ){ LIST.Add( new BeverageBuyInfo( typeof( BeverageBottle ), BeverageType.Water, 4+x6, Utility.Random( 1,15 ), 0x282A, 0xB40 ) ); } }

			// ----------------------------------------------------------------------------------------------------------------------------

			if ( market == ItemSalesInfo.Market.Tavern )
			{
				if ( d1 == 1 ){ LIST.Add( new BeverageBuyInfo( typeof( Jug ), BeverageType.Ale, 16+x1, Utility.Random( 1,15 ), 0x4CEF, 0x83b ) ); }
				if ( d2 == 1 ){ LIST.Add( new BeverageBuyInfo( typeof( Jug ), BeverageType.Cider, 16+x2, Utility.Random( 1,15 ), 0x4CEF, 0x981 ) ); }
				if ( d3 == 1 ){ LIST.Add( new BeverageBuyInfo( typeof( Jug ), BeverageType.Liquor, 16+x3, Utility.Random( 1,15 ), 0x4CEF, 0xB51 ) ); }
				if ( d4 == 1 ){ LIST.Add( new BeverageBuyInfo( typeof( Jug ), BeverageType.Wine, 16+x4, Utility.Random( 1,15 ), 0x4CEF, 0xB64 ) ); }
			}

			if ( market == ItemSalesInfo.Market.Farmer || market == ItemSalesInfo.Market.Cook || market == ItemSalesInfo.Market.Tavern )
				{ if ( d5 == 1 ){ LIST.Add( new BeverageBuyInfo( typeof( Jug ), BeverageType.Milk, 16+x5, Utility.Random( 1,15 ), 0x4CEF, 0x9A3 ) ); } }

			if ( market == ItemSalesInfo.Market.Tavern || market == ItemSalesInfo.Market.Provisions )
				{ if ( d6 == 1 ){ LIST.Add( new BeverageBuyInfo( typeof( Jug ), BeverageType.Water, 16+x6, Utility.Random( 1,15 ), 0x4CEF, 0xB40 ) ); } }

			// ----------------------------------------------------------------------------------------------------------------------------

			if ( market == ItemSalesInfo.Market.Tavern )
			{
				if ( d1 == 2 ){ LIST.Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Ale, 9+x1, Utility.Random( 1,15 ), 0x65BA, 0x83b ) ); }
				if ( d2 == 2 ){ LIST.Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Cider, 9+x2, Utility.Random( 1,15 ), 0x65BA, 0x981 ) ); }
				if ( d3 == 2 ){ LIST.Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Liquor, 9+x3, Utility.Random( 1,15 ), 0x65BA, 0xB51 ) ); }
				if ( d4 == 2 ){ LIST.Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Wine, 9+x4, Utility.Random( 1,15 ), 0x65BA, 0xB64 ) ); }
			}

			if ( market == ItemSalesInfo.Market.Farmer || market == ItemSalesInfo.Market.Cook || market == ItemSalesInfo.Market.Tavern )
				{ if ( d5 == 2 ){ LIST.Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Milk, 9+x5, Utility.Random( 1,15 ), 0x65BA, 0x9A3 ) ); } }

			if ( market == ItemSalesInfo.Market.Tavern || market == ItemSalesInfo.Market.Provisions )
				{ if ( d6 == 2 ){ LIST.Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Water, 9+x6, Utility.Random( 1,15 ), 0x65BA, 0xB40 ) ); } }
		}

		public static bool Chemist( int val, ItemSalesInfo.Market mkt, ItemSalesInfo.Category cat )
		{
			bool chemist = false;

			if ( cat == ItemSalesInfo.Category.Reagent )
			{
				ItemSalesInfo.Market category = ItemSalesInfo.Market.None;

				ItemSalesInfo info = GetData( val );
				if ( info != null )
					category = info.iMarket;

				if ( mkt == ItemSalesInfo.Market.Alchemy )
				{
					if ( category == ItemSalesInfo.Market.Reg_AH ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_AHD ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_AHDW ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_AHW ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_MAHD ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_MAHDW ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_NA ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_NAHW ){ chemist = true; }
				}
				else if ( mkt == ItemSalesInfo.Market.Necro )
				{
					if ( category == ItemSalesInfo.Market.Reg_NA ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_NAHW ){ chemist = true; }
				}
				else if ( mkt == ItemSalesInfo.Market.Druid )
				{
					if ( category == ItemSalesInfo.Market.Reg_AHD ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_AHDW ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_MAHD ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_MAHDW ){ chemist = true; }
				}
				else if ( mkt == ItemSalesInfo.Market.Witch )
				{
					if ( category == ItemSalesInfo.Market.Reg_AHDW ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_AHW ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_MAHDW ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_NAHW ){ chemist = true; }
				}
				else if ( mkt == ItemSalesInfo.Market.Mage )
				{
					if ( category == ItemSalesInfo.Market.Reg_MAHD ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_MAHDW ){ chemist = true; }
				}
				else if ( mkt == ItemSalesInfo.Market.Herbalist )
				{
					if ( category == ItemSalesInfo.Market.Reg_AH ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_AHD ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_AHDW ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_AHW ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_MAHD ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_MAHDW ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Reg_NAHW ){ chemist = true; }
				}
			}
			else if ( cat == ItemSalesInfo.Category.Resource )
			{
				ItemSalesInfo.Market category = ItemSalesInfo.Market.None;

				ItemSalesInfo info = GetData( val );
				if ( info != null )
					category = info.iMarket;

				if ( mkt == ItemSalesInfo.Market.Alchemy )
				{
					if ( category == ItemSalesInfo.Market.Res_AH ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Res_NAHW ){ chemist = true; }
				}
				else if ( mkt == ItemSalesInfo.Market.Necro )
				{
					if ( category == ItemSalesInfo.Market.Res_NAHW ){ chemist = true; }
				}
				else if ( mkt == ItemSalesInfo.Market.Druid )
				{
					if ( category == ItemSalesInfo.Market.Res_DW ){ chemist = true; }
				}
				else if ( mkt == ItemSalesInfo.Market.Witch )
				{
					if ( category == ItemSalesInfo.Market.Res_DW ){ chemist = true; }
				}
				else if ( mkt == ItemSalesInfo.Market.Mage )
				{
					if ( category == ItemSalesInfo.Market.Res_MAHD ){ chemist = true; }
				}
				else if ( mkt == ItemSalesInfo.Market.Herbalist )
				{
					if ( category == ItemSalesInfo.Market.Res_AH ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Res_MAHD ){ chemist = true; }
					if ( category == ItemSalesInfo.Market.Res_NAHW ){ chemist = true; }
				}
			}

			return chemist;
		}

		public static int iRarity( int val ){ ItemSalesInfo info = GetData( val ); return ( info == null ? 0 : info.iRarity ); }
		public static bool iSells( int val ){ ItemSalesInfo info = GetData( val ); return ( info == null ? false : info.iSells ); }
		public static bool iBuys( int val ){ ItemSalesInfo info = GetData( val ); return ( info == null ? false : info.iBuys ); }
		public static ItemSalesInfo.Category iCategory( int val ){ ItemSalesInfo info = GetData( val ); return ( info == null ? ItemSalesInfo.Category.None : info.iCategory ); }
		public static ItemSalesInfo.Material iMaterial( int val ){ ItemSalesInfo info = GetData( val ); return ( info == null ? ItemSalesInfo.Material.None : info.iMaterial ); }
		public static ItemSalesInfo.Market iMarket( int val ){ ItemSalesInfo info = GetData( val ); return ( info == null ? ItemSalesInfo.Market.None : info.iMarket ); }
		public static ItemSalesInfo.World iWorld( int val ){ ItemSalesInfo info = GetData( val ); return ( info == null ? ItemSalesInfo.World.None : info.iWorld ); }
	}

	public class ItemSalesInfo
	{
		private Type m_ItemsType;
		private int m_Price;
		private int m_Qty;
		private int m_Rarity;
		private bool m_Sells;
		private bool m_Buys;
		private World m_World;
		private Category m_Category;
		private Material m_Material;
		private Market m_Market;

		public Type ItemsType{ get{ return m_ItemsType; } }
		public int iPrice { get{ return m_Price; } }
		public int iQty { get{ return m_Qty; } }
		public int iRarity { get{ return m_Rarity; } }
		public bool iSells { get{ return m_Sells; } }
		public bool iBuys { get{ return m_Buys; } }
		public World iWorld { get{ return m_World; } }
		public Category iCategory { get{ return m_Category; } }
		public Material iMaterial { get{ return m_Material; } }
		public Market iMarket { get{ return m_Market; } }

		public ItemSalesInfo( Type v_ItemType, int v_Price, int v_Qty, int v_Rarity, bool v_Sells, bool v_Buys, World v_World, Category v_Category, Material v_Material, Market v_Market )
		{
			m_ItemsType = v_ItemType;
			m_Price = v_Price;
			m_Qty = v_Qty;
			m_Rarity = v_Rarity;
			m_Sells = v_Sells;
			m_Buys = v_Buys;
			m_World = v_World;
			m_Category = v_Category;
			m_Material = v_Material;
			m_Market = v_Market;
		}

		public enum World
		{
			None = 0,
			Ambrosia = 1,
			Dread = 2,
			Elf = 3,
			Lodor = 4,
			Necro = 5,
			Orient = 6,
			Savage = 7,
			Sea = 8,
			Serpent = 9,
			Sosaria = 10,
			Umber = 11,
			Underworld = 12
		}

		public enum Material
		{
			None = 0,
			Bone = 1,
			Cloth = 2,
			Leather = 3,
			Metal = 4,
			Scales = 5,
			Wood = 6,
			All = 7
		}

		public enum Category
		{
			None = 0,
			Armor = 1,
			Artifact = 2,
			Book = 3,
			Christmas = 4,
			Halloween = 5,
			MonsterRace = 6,
			Pack = 7,
			Potion = 8,
			Rare = 9,
			Reagent = 10,
			Resource = 11,
			Rune = 12,
			Scroll = 13,
			Shield = 14,
			Supply = 15,
			Tavern = 16,
			Wand = 17,
			Weapon = 18,
			All = 19
		}

		public enum Market
		{
			None = 0,
			Alchemy = 1,
			Animals = 2,
			Art = 3,
			Assassin = 4,
			Banker = 5,
			Barber = 6,
			Bard = 7,
			Bow = 8,
			Butcher = 9,
			Carpenter = 10,
			Cartographer = 11,
			Cattle = 12,
			Cook = 13,
			Death = 14,
			Druid = 15,
			Elemental = 16,
			Evil = 17,
			Farmer = 18,
			Fighter = 19,
			Fisherman = 20,
			Glass = 21,
			Healer = 22,
			Herbalist = 23,
			Home = 24,
			Inn = 25,
			Jester = 26,
			Jeweler = 27,
			Leather = 28,
			Lumber = 29,
			Mage = 30,
			Mill = 31,
			Miner = 32,
			Monk = 33,
			Necro = 34,
			Painter = 35,
			Paladin = 36,
			Provisions = 37,
			Ranger = 38,
			Reg_AH = 39,
			Reg_AHD = 40,
			Reg_AHDW = 41,
			Reg_AHW = 42,
			Reg_MAHD = 43,
			Reg_MAHDW = 44,
			Reg_NA = 45,
			Reg_NAHW = 46,
			Res_AH = 47,
			Res_DW = 48,
			Res_MAHD = 49,
			Res_NAHW = 50,
			Sage = 51,
			Sailor = 52,
			Scribe = 53,
			Shoes = 54,
			Smith = 55,
			Stable = 56,
			Stone = 57,
			Supplies = 58,
			Tailor = 59,
			Tanner = 60,
			Tavern = 61,
			Thief = 62,
			Tinker = 63,
			Undertaker = 64,
			Wax = 65,
			Witch = 66,
			Wizard = 67,
			All = 68
		}

		public static ItemSalesInfo[] m_SellingInfo = new ItemSalesInfo[]
		{
			new ItemSalesInfo( typeof(	AbbatoirDeed	),	440	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Butcher	),
			new ItemSalesInfo( typeof(	JewelryBracelet	),	20	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	JewelryCirclet	),	20	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	AdmiralsHeartyRum	),	1600	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	AdventurerCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	JewelryEarrings	),	20	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	AlchemistPouch	),	1200	,	2	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	AlchemyCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	AlchemyPouch	),	3500	,	2	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	MortarPestle	),	8	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	AlchemyTub	),	2400	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	AgilityPotion	),	15	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	LesserCurePotion	),	15	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	LesserHealPotion	),	15	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	NightSightPotion	),	15	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	RefreshPotion	),	15	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	StrengthPotion	),	15	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	LesserExplosionPotion	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ConflagrationPotion	),	30	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ConfusionBlastPotion	),	30	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	CurePotion	),	30	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	FrostbitePotion	),	30	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	HealPotion	),	30	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	TotalRefreshPotion	),	30	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ExplosionPotion	),	42	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	GreaterAgilityPotion	),	60	,	15	,	101	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	GreaterConflagrationPotion	),	60	,	15	,	101	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	GreaterConfusionBlastPotion	),	60	,	15	,	101	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	GreaterCurePotion	),	60	,	15	,	101	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	GreaterExplosionPotion	),	60	,	15	,	101	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	GreaterFrostbitePotion	),	60	,	15	,	101	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	GreaterHealPotion	),	60	,	15	,	101	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	GreaterStrengthPotion	),	60	,	15	,	101	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	GreaterManaPotion	),	120	,	15	,	101	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	LesserInvisibilityPotion	),	120	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	LesserManaPotion	),	80	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	LesserRejuvenatePotion	),	120	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	InvisibilityPotion	),	180	,	15	,	101	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ManaPotion	),	180	,	15	,	101	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	RejuvenatePotion	),	180	,	15	,	101	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	GreaterInvisibilityPotion	),	210	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	GreaterRejuvenatePotion	),	210	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	PotionOfDexterity	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	PotionOfMight	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	PotionOfWisdom	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	AutoResPotion	),	1200	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	InvulnerabilityPotion	),	1200	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	AlienEgg	),	2000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	AlternateRealityMap	),	2000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Cartographer	),
			new ItemSalesInfo( typeof(	JewelryNecklace	),	20	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	AnvilEastDeed	),	52	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	AnvilSouthDeed	),	52	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Apple	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	AppleBobbingBarrel	),	170	,	3	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ApplePie	),	7	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	AppleTreeDeed	),	640	,	2	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	AquariumEastAddonDeed	),	1600	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	AquariumSouthAddonDeed	),	1600	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	ArcaneGem	),	20	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	ArcherQuiver	),	32	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bow	),
			new ItemSalesInfo( typeof(	ArchmageRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Wizard	),
			new ItemSalesInfo( typeof(	JewelryRing	),	20	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	Armoire	),	176	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	DDRelicArmor	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ArmsCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Arrow	),	2	,	15	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Bow	),
			new ItemSalesInfo( typeof(	DDRelicArts	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	Artifact_AbysmalGloves	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_AchillesShield	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_AchillesSpear	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_AcidProofRobe	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_Aegis	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_AegisOfGrace	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_AilricsLongbow	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_AlchemistsBauble	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ANecromancerShroud	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_AngelicEmbrace	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_AngeroftheGods	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_Annihilation	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArcaneArms	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArcaneCap	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArcaneGloves	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArcaneGorget	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArcaneLeggings	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArcaneShield	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArcaneTunic	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArcanicRobe	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArcticBeacon	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArcticDeathDealer	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArmorOfFortune	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArmorOfInsight	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArmorOfNobility	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArmsOfAegis	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArmsOfFortune	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArmsOfInsight	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArmsOfNobility	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArmsOfTheFallenKing	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArmsOfTheHarrower	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ArmsOfToxicity	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_AuraOfShadows	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_AxeOfTheHeavens	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_AxeoftheMinotaur	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BeggarsRobe	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BelmontWhip	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BeltofHercules	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BladeDance	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BladeOfInsanity	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BladeOfTheRighteous	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BlazeOfDeath	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BlightGrippedLongbow	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BloodwoodSpirit	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BoneCrusher	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_Bonesmasher	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BookOfKnowledge	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_Boomstick	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BootsofHermes	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BootsofHydros	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BootsofLithos	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BootsofPyros	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BootsofStratos	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BowOfTheJukaKing	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BowofthePhoenix	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BraceletOfHealth	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BraceletOfTheElements	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BraceletOfTheVile	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BrambleCoat	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BraveKnightOfTheBritannia	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BreathOfTheDead	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_BurglarsBandana	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_Calm	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_CandleCold	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_CandleEnergy	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_CandleFire	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_CandleNecromancer	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_CandlePoison	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_CandleWizard	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_CapOfFortune	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_CapOfTheFallenKing	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_CaptainJohnsHat	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_CaptainQuacklebushsCutlass	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_CavortingClub	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_CircletOfTheSorceress	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_CoifOfBane	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_CoifOfFire	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ColdBlood	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ColdForgedBlade	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ConansHelm	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ConansLoinCloth	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ConansSword	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_CrimsonCincture	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_CrownOfTalKeesh	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_DaggerOfVenom	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_DarkGuardiansChest	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_DarkLordsPitchfork	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_DarkNeck	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_DeathsMask	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_DetectiveBoots	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_DivineArms	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_DivineCountenance	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_DivineGloves	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_DivineGorget	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_DivineLeggings	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_DivineTunic	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_DjinnisRing	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_DreadPirateHat	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_DupresCollar	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_DupresShield	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_EarringsOfHealth	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_EarringsOfTheElements	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_EarringsOfTheMagician	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_EarringsOfTheVile	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_EmbroideredOakLeafCloak	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_EnchantedTitanLegBone	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_EssenceOfBattle	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_EternalFlame	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_EvilMageGloves	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_Excalibur	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_FalseGodsScepter	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_FangOfRactus	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_FesteringWound	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_FeyLeggings	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_FleshRipper	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_Fortifiedarms	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_FortunateBlades	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_Frostbringer	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_FurCapeOfTheSorceress	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_Fury	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GandalfsHat	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GandalfsRobe	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GandalfsStaff	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GauntletsOfNobility	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GeishasObi	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GiantBlackjack	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GladiatorsCollar	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GlassSword	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GlovesOfAegis	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GlovesOfCorruption	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GlovesOfDexterity	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GlovesOfFortune	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GlovesOfInsight	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GlovesOfRegeneration	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GlovesOfTheFallenKing	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GlovesOfTheHarrower	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GlovesOfThePugilist	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GorgetOfAegis	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GorgetOfFortune	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GorgetOfInsight	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GrayMouserCloak	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GrimReapersLantern	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GrimReapersMask	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GrimReapersRobe	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GrimReapersScythe	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GuantletsOfAnger	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_GwennosHarp	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HammerofThor	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HatOfTheMagi	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HeartOfTheLion	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HellForgedArms	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HelmOfAegis	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HelmOfBrilliance	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HelmOfInsight	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HelmOfSwiftness	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HolyKnightsArmPlates	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HolyKnightsBreastplate	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HolyKnightsGloves	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HolyKnightsGorget	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HolyKnightsLegging	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HolyKnightsPlateHelm	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HolySword	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HoodedShroudOfShadows	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HornOfKingTriton	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HuntersArms	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HuntersGloves	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HuntersGorget	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HuntersHeaddress	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HuntersLeggings	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HuntersTunic	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_HydrosLexicon	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_Indecency	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_InquisitorsArms	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_InquisitorsGorget	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_InquisitorsHelm	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_InquisitorsLeggings	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_InquisitorsResolution	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_InquisitorsTunic	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_IolosLute	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_IronwoodCrown	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_JackalsArms	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_JackalsCollar	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_JackalsGloves	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_JackalsHelm	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_JackalsLeggings	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_JackalsTunic	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_JadeScimitar	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_JesterHatofChuckles	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_JinBaoriOfGoodFortune	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_KamiNarisIndestructableDoubleAxe	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_KodiakBearMask	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LegacyOfTheDreadLord	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LeggingsOfAegis	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LeggingsOfBane	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LeggingsOfDeceit	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LeggingsOfEmbers	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LeggingsOfEnlightenment	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LeggingsOfFire	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LegsOfFortune	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LegsOfInsight	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LegsOfNobility	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LegsOfTheFallenKing	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LegsOfTheHarrower	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LieutenantOfTheBritannianRoyalGuard	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LithosTome	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LongShot	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LuckyEarrings	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LuckyNecklace	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LuminousRuneBlade	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_LunaLance	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MadmansHatchet	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MagesBand	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MagiciansIllusion	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MagiciansMempo	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MantleofHydros	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MantleofLithos	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MantleofPyros	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MantleofStratos	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MarbleShield	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MauloftheBeast	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MaulOfTheTitans	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MelisandesCorrodedHatchet	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MidnightBracers	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MidnightGloves	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MidnightHelm	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MidnightLegs	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MidnightTunic	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_MinersPickaxe	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_NightsKiss	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_NordicVikingSword	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_NoxBow	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_NoxNightlight	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_NoxRangersHeavyCrossbow	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_OblivionsNeedle	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_OrcChieftainHelm	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_OrcishVisage	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_OrnamentOfTheMagician	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_OrnateCrownOfTheHarrower	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_OssianGrimoire	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_OverseerSunderedBlade	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_Pacify	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_PadsOfTheCuSidhe	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_PendantOfTheMagi	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_Pestilence	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_PhantomStaff	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_PixieSwatter	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_PolarBearBoots	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_PolarBearCape	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_PolarBearMask	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_PowerSurge	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_PyrosGrimoire	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_Quell	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_QuiverOfBlight	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_QuiverOfElements	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_QuiverOfFire	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_QuiverOfIce	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_QuiverOfInfinity	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_QuiverOfLightning	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_QuiverOfRage	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RaedsGlory	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RamusNecromanticScalpel	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ResilientBracer	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_Retort	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RighteousAnger	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RingOfHealth	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RingOfProtection	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RingOfTheElements	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RingOfTheMagician	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RingOfTheVile	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RobeofHydros	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RobeofLithos	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RobeofPyros	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RobeofStratos	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RobeOfTeleportation	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RobeOfTheEclipse	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RobeOfTheEquinox	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RobeOfTreason	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RobinHoodsBow	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RobinHoodsFeatheredHat	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RodOfResurrection	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RoyalArchersBow	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RoyalGuardsChestplate	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RoyalGuardsGorget	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RoyalGuardSurvivalKnife	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_RuneCarvingKnife	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_SamaritanRobe	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_SamuraiHelm	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_SerpentsFang	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ShadowBlade	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ShadowDancerArms	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ShadowDancerCap	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ShadowDancerGloves	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ShadowDancerGorget	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ShadowDancerLeggings	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ShadowDancerTunic	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ShaMontorrossbow	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ShardThrasher	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ShieldOfInvulnerability	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ShimmeringTalisman	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ShroudOfDeciet	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_SilvanisFeywoodBow	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_SinbadsSword	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_SongWovenMantle	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_SoulSeeker	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_SpellWovenBritches	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_SpiritOfTheTotem	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_SprintersSandals	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_StaffOfPower	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_StaffofSnakes	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_StaffOfTheMagi	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_StitchersMittens	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_Stormbringer	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_StratosManual	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_Subdue	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_SwiftStrike	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TalonBite	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TheBeserkersMaul	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TheDragonSlayer	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TheDryadBow	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TheNightReaper	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TheRobeOfBritanniaAri	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TheTaskmaster	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TitansHammer	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TorchOfTrapFinding	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TotemArms	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TotemGloves	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TotemGorget	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TotemLeggings	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TotemOfVoid	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TotemTunic	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TownGuardsHalberd	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TunicOfAegis	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TunicOfBane	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TunicOfFire	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TunicOfTheFallenKing	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_TunicOfTheHarrower	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_VampiresRobe	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_VampiricDaisho	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_VioletCourage	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_VoiceOfTheFallenKing	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_WarriorsClasp	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_WildfireBow	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_Windsong	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_WizardsPants	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_WrathOfTheDryad	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_YashimotosHatsuburi	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Artifact_ZyronicClaw	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Artifact	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	ArtifactLargeVase	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	ArtifactVase	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	AssassinRobe	),	38	,	10	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Assassin	),
			new ItemSalesInfo( typeof(	AssassinSpike	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Assassin	),
			new ItemSalesInfo( typeof(	AwaseMisoSoup	),	3	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Axe	),	40	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Axle	),	2	,	5	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	AxleGears	),	3	,	5	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	Backgammon	),	2	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	Backpack	),	15	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	Bacon	),	7	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	Bag	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	BagOfSending	),	4000	,	10	,	90	,	false	,	false	,	World.Lodor	,	Category.Rare	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	BagOfTricks	),	200	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jester	),
			new ItemSalesInfo( typeof(	BakerCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	BallOfSummoning	),	3000	,	10	,	90	,	false	,	false	,	World.Lodor	,	Category.Rare	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	BallotBoxDeed	),	44	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	BambooChair	),	12	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	BambooFlute	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	BambooScreen	),	334	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Banana	),	2	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	Bandage	),	2	,	60	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Healer	),
			new ItemSalesInfo( typeof(	Bandana	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	DDRelicBanner	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	BannerDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	OrnateAxe	),	55	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	BarbarianBoots	),	15	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Shoes	),
			new ItemSalesInfo( typeof(	VikingSword	),	55	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RoyalSword	),	55	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Bardiche	),	60	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ArmysPaeonScroll	),	12	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	MagesBalladScroll	),	12	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	EnchantingEtudeScroll	),	14	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	SheepfoeMamboScroll	),	14	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	SinewyEtudeScroll	),	14	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	FireThrenodyScroll	),	16	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	IceThrenodyScroll	),	16	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	PoisonThrenodyScroll	),	16	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	MagicFinaleScroll	),	20	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	EnergyCarolScroll	),	32	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	EnergyThrenodyScroll	),	32	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	FireCarolScroll	),	32	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	IceCarolScroll	),	32	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	KnightsMinneScroll	),	32	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	PoisonCarolScroll	),	32	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	FoeRequiemScroll	),	36	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	BarkeepContract	),	1252	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	BarrelHoops	),	8	,	5	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	BarrelLid	),	28	,	5	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	BarrelStaves	),	28	,	5	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	BarrelTap	),	4	,	5	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	Bascinet	),	18	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	BasementDoor	),	2500	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	BattleAxe	),	26	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	BearCap	),	50	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Leather	),
			new ItemSalesInfo( typeof(	DDRelicBearRugsAddonDeed	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	Bedroll	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	apiBeeHiveDeed	),	2000	,	10	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	BeekeeperCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	Beeswax	),	200	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	), // 200 ??
			new ItemSalesInfo( typeof(	BeggarVest	),	12	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	BeginnerBook	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	Belt	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	BentoBox	),	6	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	BigBag	),	10	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	BlackCatStatue	),	100	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	BlackDyeTub	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	BlacksmithCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	BlackWellDeed	),	500	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	BladedStaff	),	40	,	31	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	BlankScroll	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Scribe	),
			new ItemSalesInfo( typeof(	AmethystBlocks	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	EmeraldBlocks	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	GarnetBlocks	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	IceBlocks	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	JadeBlocks	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	MarbleBlocks	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	OnyxBlocks	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	QuartzBlocks	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RubyBlocks	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	SapphireBlocks	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	SilverBlocks	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	SpinelBlocks	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	StarRubyBlocks	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	TopazBlocks	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	CaddelliteBlocks	),	480	,	0	,	95	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	BloodPentagramDeed	),	3800	,	3	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Death	),
			new ItemSalesInfo( typeof(	BloodyDrink	),	6	,	30	,	0	,	false	,	false	,	World.None	,	Category.MonsterRace	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	BloodyTableAddonDeed	),	1500	,	3	,	50	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Death	),
			new ItemSalesInfo( typeof(	Blowpipe	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Glass	),
			new ItemSalesInfo( typeof(	BlueDecorativeRugDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	BlueFancyRugDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	BluePlainRugDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	BlueSnowflake	),	100	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	BarkFragment	),	10	,	0	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	Board	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	AshBoard	),	6	,	15	,	50	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	CherryBoard	),	7	,	15	,	55	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	EbonyBoard	),	8	,	15	,	60	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	GoldenOakBoard	),	9	,	15	,	65	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	HickoryBoard	),	10	,	15	,	70	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	MahoganyBoard	),	11	,	15	,	75	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	OakBoard	),	12	,	15	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	PineBoard	),	13	,	15	,	85	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	GhostBoard	),	13	,	15	,	87	,	false	,	false	,	World.Necro	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	RosewoodBoard	),	14	,	15	,	89	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	WalnutBoard	),	15	,	15	,	91	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	PetrifiedBoard	),	16	,	15	,	93	,	false	,	false	,	World.Underworld	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	DriftwoodBoard	),	12	,	15	,	95	,	false	,	false	,	World.Sea	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ElvenBoard	),	17	,	15	,	97	,	false	,	false	,	World.Lodor	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	BorlBoard	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	CosianBoard	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	GreelBoard	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	JaporBoard	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	KyshyyykBoard	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	LaroonBoard	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	TeejBoard	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	VeshokBoard	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	BoatStain	),	26	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	BodyPart	),	180	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	BodySash	),	6	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	BoilingCauldronDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Death	),
			new ItemSalesInfo( typeof(	Bokuto	),	21	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Weapon	,	Material.Wood	,	Market.Monk	),
			new ItemSalesInfo( typeof(	BolaBall	),	6	,	5	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	Bolt	),	2	,	60	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Bow	),
			new ItemSalesInfo( typeof(	Bone	),	20	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	BoneArms	),	86	,	3	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Bone	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	BoneChest	),	128	,	3	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Bone	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	BoneContainer	),	800	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	BoneGloves	),	78	,	3	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Bone	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	BoneHarvester	),	35	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	BoneHelm	),	24	,	3	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Bone	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	OrcHelm	),	24	,	3	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Bone	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	BoneLegs	),	102	,	3	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Bone	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	BonePile	),	20	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	Bones	),	20	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	BrittleSkeletal	),	4	,	15	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	DrowSkeletal	),	8	,	15	,	50	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	OrcSkeletal	),	8	,	15	,	55	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	ReptileSkeletal	),	12	,	15	,	60	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	OgreSkeletal	),	16	,	15	,	65	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	TrollSkeletal	),	16	,	15	,	70	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	GargoyleSkeletal	),	20	,	15	,	75	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	MinotaurSkeletal	),	20	,	15	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	LycanSkeletal	),	24	,	15	,	85	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	SharkSkeletal	),	24	,	15	,	87	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	ColossalSkeletal	),	28	,	15	,	89	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	BoneSkirt	),	102	,	3	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Bone	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	MysticalSkeletal	),	32	,	15	,	91	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	VampireSkeletal	),	36	,	15	,	93	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	LichSkeletal	),	40	,	15	,	95	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	SphinxSkeletal	),	40	,	15	,	97	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	DevilSkeletal	),	44	,	15	,	97	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	DracoSkeletal	),	44	,	15	,	97	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	AndorianSkeletal	),	48	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	CardassianSkeletal	),	48	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	MartianSkeletal	),	48	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	RodianSkeletal	),	48	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	TuskenSkeletal	),	48	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	TwilekSkeletal	),	48	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	XenoSkeletal	),	48	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	XindiSkeletal	),	48	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	ZabrakSkeletal	),	48	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	Bonnet	),	8	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	WritingBook	),	15	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Scribe	),
			new ItemSalesInfo( typeof(	DDRelicBook	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Scribe	),
			new ItemSalesInfo( typeof(	BlankMap	),	6	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cartographer	),
			new ItemSalesInfo( typeof(	BookDruidBrewing	),	50	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	BookOfBushido	),	140	,	5	,	0	,	false	,	false	,	World.Orient	,	Category.Book	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	BookOfChivalry	),	140	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Paladin	),
			new ItemSalesInfo( typeof(	BookofDead	),	25000	,	1	,	0	,	false	,	false	,	World.Ambrosia	,	Category.None	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	BookOfNinjitsu	),	140	,	5	,	0	,	false	,	false	,	World.Orient	,	Category.Book	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	BookOfPoisons	),	50	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Assassin	),
			new ItemSalesInfo( typeof(	BookWitchBrewing	),	50	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	Boots	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Shoes	),
			new ItemSalesInfo( typeof(	Bottle	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Res_AH	),
			new ItemSalesInfo( typeof(	BottleOfAcid	),	600	,	15	,	80	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	BottleOil	),	10	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	Bow	),	40	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Wood	,	Market.Bow	),
			new ItemSalesInfo( typeof(	BowlFlour	),	7	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Mill	),
			new ItemSalesInfo( typeof(	BowyerCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bow	),
			new ItemSalesInfo( typeof(	BraceletOfBinding	),	3500	,	10	,	90	,	false	,	false	,	World.Lodor	,	Category.Rare	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	BreadLoaf	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Mill	),
			new ItemSalesInfo( typeof(	BroadcastCrystal	),	256	,	15	,	90	,	false	,	false	,	World.Lodor	,	Category.None	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Broadsword	),	35	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	BrocadeGozaMatEastDeed	),	70	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	BrocadeGozaMatSouthDeed	),	70	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	BrocadeSquareGozaMatEastDeed	),	70	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	BrocadeSquareGozaMatSouthDeed	),	70	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	BrokenArmoireDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	BrokenBedDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	BrokenBookcaseDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	BrokenChestOfDrawersDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	BrokenCoveredChairDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	BrokenFallenChairDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	BrokenVanityDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	BronzeStatueMaker	),	50000	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	BrownWellDeed	),	500	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	Bucket	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	Buckler	),	50	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	BuriedBody	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	BurningScarecrowA	),	290	,	3	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Death	),
			new ItemSalesInfo( typeof(	ButcherCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Butcher	),
			new ItemSalesInfo( typeof(	ButcherKnife	),	14	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Butcher	),
			new ItemSalesInfo( typeof(	Cabbage	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	Cake	),	13	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	CakeMix	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	Candelabra	),	280	,	1	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	CandelabraStand	),	420	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	Candle	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	CandleLarge	),	140	,	1	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	CandleLong	),	160	,	1	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	CandleReligious	),	240	,	1	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	DDRelicLight1	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	DDRelicLight2	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	DDRelicLight3	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	CandleShort	),	150	,	1	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	CandleSkull	),	190	,	1	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	CandyCane	),	20	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	EmptyCanopicJar	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	CanopicJar	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	Cantaloupe	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	Canteen	),	10	,	0	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	Cap	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	CarpenterCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	CarpenterTools	),	15	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	Carrot	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin	),	200	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin10	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin11	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin12	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin13	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin14	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin15	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin16	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin17	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin18	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin19	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin2	),	200	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin20	),	5000	,	3	,	80	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin3	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin4	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin5	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin6	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin7	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin8	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CarvedPumpkin9	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	CeramicMug	),	2	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	ChainChest	),	143	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ChainCoif	),	17	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ChainHatsuburi	),	76	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	ChainLegs	),	149	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Chainsaw	),	520	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	ChainSkirt	),	149	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ChampionShield	),	231	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ChaosShield	),	256	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	CheckerBoard	),	2	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	Checkers	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	Checkers2	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	CheesePizza	),	8	,	10	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	CheeseWheel	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.Supply	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	CherryArmoire	),	198	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	CherryBlossomTreeDeed	),	540	,	2	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	Chessboard	),	2	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	Chessmen	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	Chessmen2	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	Chessmen3	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	ChickenLeg	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Supply	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	ChristmasRobe	),	50	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	CinnamonFancyRugDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	CityMap	),	6	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cartographer	),
			new ItemSalesInfo( typeof(	Claymore	),	60	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Cleaver	),	15	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Butcher	),
			new ItemSalesInfo( typeof(	Cloak	),	8	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	Clock	),	22	,	5	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	DDRelicClock1	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	DDRelicClock2	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	DDRelicClock3	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	ClockFrame	),	24	,	5	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	ClockLeft	),	10	,	5	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	ClockParts	),	3	,	5	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	ClockRight	),	10	,	5	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	ClockworkAssembly	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	ClosedBarrel	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	CloseHelm	),	18	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Fabric	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	FurryFabric	),	6	,	15	,	55	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	WoolyFabric	),	6	,	15	,	60	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	ClothCowl	),	12	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	SilkFabric	),	9	,	15	,	65	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	HauntedFabric	),	12	,	15	,	70	,	false	,	false	,	World.Necro	,	Category.Resource	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	ArcticFabric	),	15	,	15	,	75	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	PyreFabric	),	15	,	15	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	VenomousFabric	),	18	,	15	,	85	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	ClothHood	),	12	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	MysteriousFabric	),	21	,	15	,	90	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	VileFabric	),	24	,	15	,	95	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	DivineFabric	),	27	,	15	,	96	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	FiendishFabric	),	30	,	15	,	97	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	ClothNinjaHood	),	33	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.Cloth	,	Market.Monk	),
			new ItemSalesInfo( typeof(	ClothNinjaJacket	),	24	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.Cloth	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Club	),	16	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Wood	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	CommodityDeed	),	1000	,	20	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Banker	),
			new ItemSalesInfo( typeof(	DDRelicCoins	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Banker	),
			new ItemSalesInfo( typeof(	ColorCandleLong	),	180	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	ColorCandleShort	),	170	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	ColoredArmoireA	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredArmoireB	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredCabinetA	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredCabinetB	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredCabinetC	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredCabinetD	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredCabinetE	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredCabinetF	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredCabinetG	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredCabinetH	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredCabinetI	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredCabinetJ	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredCabinetK	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredCabinetL	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredCabinetM	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredCabinetN	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredDresserA	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredDresserB	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredDresserC	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredDresserD	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredDresserE	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredDresserF	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredDresserG	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredDresserH	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredDresserI	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredDresserJ	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelf1	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelf2	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelf3	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelf4	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelf5	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelf6	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelf7	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelf8	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfA	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfB	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfC	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfD	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfE	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfF	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfG	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfH	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfI	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfJ	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfK	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfL	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfM	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfN	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfO	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfP	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfQ	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfR	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfS	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfT	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfU	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfV	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfW	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfX	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfY	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredShelfZ	),	118	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ColoredWallTorch	),	100	,	20	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	ColoringBook	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	CompositeBow	),	45	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Wood	,	Market.Bow	),
			new ItemSalesInfo( typeof(	ContractOfEmployment	),	1252	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	CookedBird	),	17	,	15	,	0	,	false	,	false	,	World.None	,	Category.Supply	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	CookieMix	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	Cookies	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	CorpseChest	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	CorpseSailor	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	Cotton	),	3	,	-1	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	CounterDark	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	CounterFancy	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	CounterLight	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	CounterPolished	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	CounterRustic	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	CounterStained	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	CounterWood	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	CounterWooden	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	CrescentBlade	),	37	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	CrestedShield	),	231	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Crossbow	),	55	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Wood	,	Market.Bow	),
			new ItemSalesInfo( typeof(	Crystals	),	10	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	CulinarySet	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	CultistRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	CurtainsDeed	),	5000	,	1	,	90	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	CurvedFlask	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	Cutlass	),	24	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DaemonDartBoardEastDeed	),	1200	,	3	,	80	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	DaemonDartBoardSouthDeed	),	1200	,	3	,	80	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	DaemonMount	),	15000	,	5	,	101	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	Dagger	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Daisho	),	66	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Weapon	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	DarkBrownTreeDeed	),	540	,	2	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	DarkHeart	),	500	,	5	,	0	,	false	,	false	,	World.Ambrosia	,	Category.None	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	DarkShield	),	231	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DartBoardEastDeed	),	32	,	0	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	DartBoardSouthDeed	),	32	,	0	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	DataPad	),	300	,	0	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	DeadBodyEWDeed	),	345	,	3	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	DeadBodyNSDeed	),	345	,	3	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	DeadMask	),	28	,	10	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	DeamonHeadA	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Death	),
			new ItemSalesInfo( typeof(	DeamonHeadB	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Death	),
			new ItemSalesInfo( typeof(	DeamonHeadC	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Death	),
			new ItemSalesInfo( typeof(	DeathKnightSpellbook	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Death	),
			new ItemSalesInfo( typeof(	BackpackArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	BloodyWaterArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	BooksFaceDownArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	BooksNorthArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	BooksWestArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	BottleArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	BrazierArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	CocoonArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	DamagedBooksArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	EggCaseArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	GruesomeStandardArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	LampPostArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	LeatherTunicArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	RockArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	RuinedPaintingArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	SaddleArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	SkinnedDeerArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	SkinnedGoatArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	SkullCandleArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	StretchedHideArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	StuddedLeggingsArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	StuddedTunicArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	TarotCardsArtifact	),	1	,	0	,	101	,	false	,	true	,	World.None	,	Category.Artifact	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	DecoBlackmoor	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoBloodspawn	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoBottlesOfLiquor	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	DecoBridle	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	DecoBridle2	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	DecoBrimstone	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoDragonsBlood2	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoEyeOfNewt	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoGarlic	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoGarlic2	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoGarlicBulb	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoGarlicBulb2	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoGinseng	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoGinseng2	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoGinsengRoot	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoGinsengRoot2	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoHay2	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	DecoMandrake	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoMandrake2	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoMandrake3	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoMandrakeRoot	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoMandrakeRoot2	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoNightshade	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoNightshade2	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoNightshade3	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoObsidian	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecoPumice	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DecorativePlateKabuto	),	95	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	DecorativeShieldDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DecoTray	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	DecoTray2	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	DecoWyrmsHeart	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DeerCap	),	50	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Leather	),
			new ItemSalesInfo( typeof(	DemonPrison	),	2000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Death	),
			new ItemSalesInfo( typeof(	DiamondMace	),	31	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Dices	),	2	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	DisguiseKit	),	700	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Assassin	),
			new ItemSalesInfo( typeof(	DockingLantern	),	58	,	15	,	50	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	DolphinEastLargeAddonDeed	),	1600	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	DolphinEastSmallAddonDeed	),	1600	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	DolphinSouthLargeAddonDeed	),	1600	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	DolphinSouthSmallAddonDeed	),	1600	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	DoubleAxe	),	52	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DoubleBladedStaff	),	35	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Doublet	),	13	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	Dough	),	8	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	DracolichSkull	),	2000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	DragonEgg	),	2000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	DragonLamp	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	DragonPedStatue	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	DrakboneBracers	),	188	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DrakboneGreaves	),	218	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DrakboneGuantlets	),	144	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DrakboneHelm	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DrakboneTunic	),	242	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DrakkhenEggBlack	),	20000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	DrakkhenEggRed	),	20000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	DreadHelm	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Dressform	),	128	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	DDRelicDrink	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	DruidCauldron	),	16	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	LureStonePotion	),	25	,	3	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	NaturesPassagePotion	),	30	,	3	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	ShieldOfEarthPotion	),	35	,	3	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	WoodlandProtectionPotion	),	40	,	3	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	StoneCirclePotion	),	45	,	3	,	101	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	GraspingRootsPotion	),	50	,	3	,	101	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	DruidicRunePotion	),	55	,	3	,	101	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	HerbalHealingPotion	),	60	,	3	,	101	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	BlendWithForestPotion	),	65	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	FireflyPotion	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	MushroomGatewayPotion	),	75	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	SwarmOfInsectsPotion	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	ProtectiveFairyPotion	),	85	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	TreefellowPotion	),	90	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	VolcanicEruptionPotion	),	95	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	RestorativeSoilPotion	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	DruidPouch	),	1200	,	2	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	Drums	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	DuctTape	),	180	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	Dyes	),	8	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	DyeTub	),	8	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	DyingPlant	),	175	,	3	,	50	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Herbalist	),
			new ItemSalesInfo( typeof(	DynamicBook	),	300	,	0	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Scribe	),
			new ItemSalesInfo( typeof(	Easle	),	116	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Painter	),
			new ItemSalesInfo( typeof(	EerieGhost	),	1500	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	EggBomb	),	34	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.Weapon	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Eggs	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	ElegantArmoire	),	198	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ElegantLowTable	),	174	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ElegantRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	Elemental_Armor_Scroll	),	10	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Bolt_Scroll	),	10	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Mend_Scroll	),	10	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Sanctuary_Scroll	),	10	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Pain_Scroll	),	16	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Protection_Scroll	),	16	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Purge_Scroll	),	16	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Steed_Scroll	),	16	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Call_Scroll	),	22	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Force_Scroll	),	22	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Wall_Scroll	),	22	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Warp_Scroll	),	22	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Field_Scroll	),	28	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Restoration_Scroll	),	28	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Strike_Scroll	),	28	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Void_Scroll	),	28	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Blast_Scroll	),	34	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Echo_Scroll	),	34	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Fiend_Scroll	),	34	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Hold_Scroll	),	34	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Barrage_Scroll	),	40	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Rune_Scroll	),	40	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Storm_Scroll	),	40	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Summon_Scroll	),	40	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Devastation_Scroll	),	46	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Fall_Scroll	),	46	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Gate_Scroll	),	46	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Havoc_Scroll	),	46	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Apocalypse_Scroll	),	52	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Lord_Scroll	),	52	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Soul_Scroll	),	52	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	Elemental_Spirit_Scroll	),	52	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	ElementalSpellbook	),	80	,	5	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Elemental	),
			new ItemSalesInfo( typeof(	ElvenCompositeLongbow	),	42	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Wood	,	Market.Bow	),
			new ItemSalesInfo( typeof(	ElvenMachete	),	35	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ElvenShield	),	231	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ElvenSpellblade	),	33	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	EmbalmingFluid	),	200	,	55	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	EmptyBookcase	),	118	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	EmptyPewterBowl	),	2	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	EmptyVialsWRack	),	120	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	EnchantedSeaweed	),	240	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	PlantBowl	),	2	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Herbalist	),
			new ItemSalesInfo( typeof(	EnormousBag	),	20	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	EverlastingBottle	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	EverlastingLoaf	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	EvilFireplaceEastFaceAddonDeed	),	6800	,	3	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Death	),
			new ItemSalesInfo( typeof(	EvilFireplaceSouthFaceAddonDeed	),	6800	,	3	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Death	),
			new ItemSalesInfo( typeof(	ExecutionersAxe	),	30	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ExquisiteRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	FabledFishingNet	),	240	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	DDRelicCloth	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	FancyArmoire	),	176	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	FancyDress	),	26	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	FancyHood	),	12	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	FancyRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	FancyShirt	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	FancyWindChimes	),	20	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	FancyWoodenChairCushion	),	24	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	Feather	),	2	,	60	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Bow	),
			new ItemSalesInfo( typeof(	FeatheredHat	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	FemaleKimono	),	18	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.Cloth	,	Market.Monk	),
			new ItemSalesInfo( typeof(	FemaleLeatherChest	),	116	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	FemalePlateChest	),	207	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	FemaleStuddedChest	),	142	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	FinishedWoodenChest	),	158	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	FirstAidKit	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Healer	),
			new ItemSalesInfo( typeof(	Fish	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	FishingNet	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	FishingPole	),	15	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	FishSteak	),	30	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	DDRelicAlchemy	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	Flax	),	3	,	-1	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	FletcherCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bow	),
			new ItemSalesInfo( typeof(	FletcherTools	),	2	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bow	),
			new ItemSalesInfo( typeof(	FloppyHat	),	7	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	FlourMillEastDeed	),	438	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Mill	),
			new ItemSalesInfo( typeof(	FlourMillSouthDeed	),	438	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Mill	),
			new ItemSalesInfo( typeof(	FoolsCoat	),	26	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jester	),
			new ItemSalesInfo( typeof(	FootStool	),	12	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ForkLeft	),	2	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	ForkRight	),	2	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	FormalCoat	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	FormalRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	FormalShirt	),	14	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	FountainDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	FountainOfLifeDeed	),	7400	,	1	,	90	,	false	,	false	,	World.Lodor	,	Category.Rare	,	Material.None	,	Market.Healer	),
			new ItemSalesInfo( typeof(	FrankenArmLeft	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	FrankenArmRight	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	FrankenBrain	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	FrankenHead	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	FrankenJournal	),	1500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	FrankenLegLeft	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	FrankenLegRight	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	FrankenTorso	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	FrenchBread	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Mill	),
			new ItemSalesInfo( typeof(	FreshBrain	),	6	,	30	,	0	,	false	,	false	,	World.None	,	Category.MonsterRace	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	FriedEggs	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	CubedFruit	),	3	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	FruitPie	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	Fukiya	),	20	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Weapon	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	FukiyaDarts	),	3	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Weapon	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	FullApron	),	10	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	FullBookcase	),	118	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	FullVialsWRack	),	130	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	DDRelicFur	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tanner	),
			new ItemSalesInfo( typeof(	Futon	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	GargoyleFlightStatue	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	GargoyleStatue	),	300	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	Gears	),	2	,	5	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	DDRelicGem	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	GemOfSeeing	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	GhostShipAnchor	),	1600	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	GiantBag	),	10	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	GiftBox	),	140	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	GiftBoxAngel	),	140	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	GiftBoxCube	),	140	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	GiftBoxCylinder	),	140	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	GiftBoxNeon	),	140	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	GiftBoxOctogon	),	140	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	GiftBoxRectangle	),	140	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	GildedDarkRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	GildedDress	),	24	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	GildedLightRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	GildedRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	GildedWoodenChest	),	158	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	GingerBreadCookie	),	20	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	GingerBreadHouseDeed	),	450	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	GlassblowingBook	),	10637	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Glass	),
			new ItemSalesInfo( typeof(	GlassMug	),	6	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Glass	),
			new ItemSalesInfo( typeof(	Globe	),	8	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	GnarledStaff	),	16	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Wood	,	Market.Witch	),
			new ItemSalesInfo( typeof(	Goblet	),	4	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	GodBrewing	),	1000	,	20	,	0	,	false	,	false	,	World.Elf	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	GodSewing	),	1000	,	20	,	0	,	false	,	false	,	World.Elf	,	Category.None	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	GodSmithing	),	1000	,	20	,	0	,	false	,	false	,	World.Elf	,	Category.None	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	GoldBricks	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	GoldenDecorativeRugDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	GolemManual	),	1500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	GothicCandelabraA	),	280	,	3	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Death	),
			new ItemSalesInfo( typeof(	GozaMatEastDeed	),	70	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	GozaMatSouthDeed	),	70	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	CubedGrain	),	6	,	0	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Mill	),
			new ItemSalesInfo( typeof(	Granite	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	DullCopperGranite	),	16	,	15	,	50	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	ShadowIronGranite	),	24	,	15	,	55	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	CopperGranite	),	32	,	15	,	60	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	BronzeGranite	),	40	,	15	,	65	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	GoldGranite	),	48	,	15	,	70	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	AgapiteGranite	),	56	,	15	,	75	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	VeriteGranite	),	64	,	15	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	ValoriteGranite	),	72	,	15	,	85	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	NepturiteGranite	),	72	,	15	,	87	,	false	,	false	,	World.Sea	,	Category.Resource	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	ObsidianGranite	),	72	,	15	,	89	,	false	,	false	,	World.Serpent	,	Category.Resource	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	MithrilGranite	),	96	,	15	,	91	,	false	,	false	,	World.Underworld	,	Category.Resource	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	DwarvenGranite	),	112	,	15	,	93	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	XormiteGranite	),	115	,	0	,	95	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	Grapes	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	GrapplingHook	),	58	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	GraveChest	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	GraveSpade	),	12	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	GreenGourd	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	GreenStocking	),	110	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	GreenTea	),	3	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	GreenTeaBasket	),	2	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	GreyTreeDeed	),	540	,	2	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	GrimWarning	),	120	,	3	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	GuardsmanShield	),	231	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	GuildCarpentry	),	500	,	5	,	101	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	GuildDeed	),	12450	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	GuildFletching	),	500	,	5	,	101	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bow	),
			new ItemSalesInfo( typeof(	GuildHammer	),	500	,	5	,	101	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	GuildSewing	),	500	,	5	,	101	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	GuildTinkering	),	500	,	5	,	101	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	GwennoGraveAddonDeed	),	10000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	StatueGygaxAddonDeed	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	HairDye	),	100	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Barber	),
			new ItemSalesInfo( typeof(	HairDyeBottle	),	1000	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Barber	),
			new ItemSalesInfo( typeof(	Hakama	),	12	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.Cloth	,	Market.Monk	),
			new ItemSalesInfo( typeof(	HakamaShita	),	16	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.Cloth	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Halberd	),	42	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	HalfApron	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	halloween_block_eastAddonDeed	),	430	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	halloween_block_southAddonDeed	),	430	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	halloween_coffin_eastAddonDeed	),	470	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	halloween_coffin_southAddonDeed	),	470	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	halloween_covered_chair	),	220	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	halloween_devil_face	),	150	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	halloween_HauntedMirror1	),	270	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	halloween_HauntedMirror2	),	270	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	halloween_ruined_bookcase	),	340	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	halloween_shackles	),	125	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenBlood	),	90	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenBonePileDeed	),	680	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenChopper	),	1760	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenColumn	),	1100	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenGrave1	),	350	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenGrave2	),	350	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenGrave3	),	350	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenMaiden	),	2780	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenPack	),	130	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenPylon	),	1800	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenPylonFire	),	2100	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenShrineChaosDeed	),	1380	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenSkullPole	),	540	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenStoneColumn	),	500	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenStoneSpike	),	600	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenStoneSpike2	),	600	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenTortSkel	),	450	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenTree1	),	800	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenTree2	),	800	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenTree3	),	800	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenTree4	),	800	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenTree5	),	800	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenTree6	),	800	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HalloweenWeb	),	185	,	3	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	Ham	),	26	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	HammerPick	),	26	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Hammers	),	28	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	HangingAxesDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	HangingPlantA	),	10000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Herbalist	),
			new ItemSalesInfo( typeof(	HangingPlantB	),	10000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Herbalist	),
			new ItemSalesInfo( typeof(	HangingPlantC	),	10000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Herbalist	),
			new ItemSalesInfo( typeof(	HangingSwordsDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	Harp	),	20	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	Harpoon	),	40	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	HarpoonRope	),	2	,	250	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	Hatchet	),	20	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	Head	),	40	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	HealerCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Healer	),
			new ItemSalesInfo( typeof(	HearthOfHomeFireDeed	),	5000	,	1	,	90	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	HeaterShield	),	231	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	HeatingStand	),	2	,	10	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	HeavyCrossbow	),	55	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Wood	,	Market.Bow	),
			new ItemSalesInfo( typeof(	HeavyPlateJingasa	),	76	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Helmet	),	31	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	HenchmanArcherItem	),	6000	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	HenchmanFighterItem	),	5000	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	HenchmanWizardItem	),	7000	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	Hides	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tanner	),
			new ItemSalesInfo( typeof(	HornedHides	),	8	,	15	,	50	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tanner	),
			new ItemSalesInfo( typeof(	BarbedHides	),	10	,	15	,	55	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tanner	),
			new ItemSalesInfo( typeof(	NecroticHides	),	10	,	15	,	60	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tanner	),
			new ItemSalesInfo( typeof(	VolcanicHides	),	12	,	15	,	65	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tanner	),
			new ItemSalesInfo( typeof(	SpinedHides	),	13	,	15	,	70	,	false	,	false	,	World.Sea	,	Category.Resource	,	Material.None	,	Market.Tanner	),
			new ItemSalesInfo( typeof(	FrozenHides	),	12	,	15	,	75	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tanner	),
			new ItemSalesInfo( typeof(	GoliathHides	),	14	,	15	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tanner	),
			new ItemSalesInfo( typeof(	DraconicHides	),	14	,	15	,	85	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tanner	),
			new ItemSalesInfo( typeof(	HellishHides	),	16	,	15	,	87	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tanner	),
			new ItemSalesInfo( typeof(	DinosaurHides	),	16	,	15	,	89	,	false	,	false	,	World.Savage	,	Category.Resource	,	Material.None	,	Market.Tanner	),
			new ItemSalesInfo( typeof(	AlienHides	),	16	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tanner	),
			new ItemSalesInfo( typeof(	HikingBoots	),	800	,	15	,	0	,	false	,	false	,	World.None	,	Category.MonsterRace	,	Material.Leather	,	Market.Shoes	),
			new ItemSalesInfo( typeof(	Hinge	),	2	,	5	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	HiveTool	),	100	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	HolidayBell	),	280	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HolidayBells	),	560	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HolidayTreeDeed	),	860	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HolidayTreeFlatDeed	),	860	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	HolyManSpellbook	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Paladin	),
			new ItemSalesInfo( typeof(	HomePlants_Cactus	),	100	,	5	,	50	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Herbalist	),
			new ItemSalesInfo( typeof(	HomePlants_Flower	),	100	,	5	,	50	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Herbalist	),
			new ItemSalesInfo( typeof(	HomePlants_Grass	),	100	,	5	,	50	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Herbalist	),
			new ItemSalesInfo( typeof(	HomePlants_Leaf	),	100	,	5	,	50	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Herbalist	),
			new ItemSalesInfo( typeof(	HomePlants_Lilly	),	100	,	5	,	50	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Herbalist	),
			new ItemSalesInfo( typeof(	HomePlants_Mushroom	),	100	,	5	,	50	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Herbalist	),
			new ItemSalesInfo( typeof(	HoneydewMelon	),	7	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	HoodedMantle	),	12	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	house_sign_sign_armor	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_bake	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_bank	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_bard	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_book	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_bow	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_fletch	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_gem	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_heal	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_herb	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_inn	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_mage	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_merc	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_necro	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_pen	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_post_a	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_post_b	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_sew	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_ship	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_smith	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_supply	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_tavern	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_tinker	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	house_sign_sign_wood	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	HouseLadderDeed	),	5000	,	1	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	HousePlacementTool	),	50	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	HugeCrate	),	400	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	IcicleLargeEast	),	80	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	IcicleLargeSouth	),	80	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	IcicleMedEast	),	70	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	IcicleMedSouth	),	70	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	IcicleSmallEast	),	60	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	IcicleSmallSouth	),	60	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	IcyPatch	),	60	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	IronIngot	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DullCopperIngot	),	6	,	15	,	50	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ShadowIronIngot	),	8	,	15	,	55	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	CopperIngot	),	9	,	15	,	60	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	BronzeIngot	),	10	,	15	,	65	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	GoldIngot	),	11	,	15	,	70	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	AgapiteIngot	),	13	,	15	,	75	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	VeriteIngot	),	14	,	15	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ValoriteIngot	),	15	,	15	,	85	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	NepturiteIngot	),	16	,	15	,	87	,	false	,	false	,	World.Sea	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ObsidianIngot	),	16	,	15	,	89	,	false	,	false	,	World.Serpent	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	SteelIngot	),	16	,	15	,	91	,	false	,	false	,	World.Savage	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	BrassIngot	),	18	,	15	,	93	,	false	,	false	,	World.Umber	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	MithrilIngot	),	19	,	15	,	95	,	false	,	false	,	World.Underworld	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	XormiteIngot	),	19	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DwarvenIngot	),	23	,	15	,	97	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	AgriniumIngot	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	BeskarIngot	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	CarboniteIngot	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	CortosisIngot	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DurasteelIngot	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DuriteIngot	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	FariumIngot	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	LaminasteelIngot	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	NeuraniumIngot	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	PhrikIngot	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	PromethiumIngot	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	QuadraniumIngot	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	SongsteelIngot	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	TitaniumIngot	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	TrimantiumIngot	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	XonoliteIngot	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DDRelicInstrument	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	InteriorDecorator	),	100	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	IronFlask	),	500	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	IronSafe	),	5000	,	5	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Banker	),
			new ItemSalesInfo( typeof(	BankChest	),	500000	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Banker	),
			new ItemSalesInfo( typeof(	IvoryTusk	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	JadeStatueMaker	),	50000	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	Jar	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHDW	),
			new ItemSalesInfo( typeof(	JarHoney	),	600	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	JarsOfWaxInstrument	),	160	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	JarsOfWaxLeather	),	160	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	JarsOfWaxMetal	),	160	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	JesterGarb	),	26	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jester	),
			new ItemSalesInfo( typeof(	JesterHat	),	12	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jester	),
			new ItemSalesInfo( typeof(	JesterShoes	),	26	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jester	),
			new ItemSalesInfo( typeof(	JesterSuit	),	26	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jester	),
			new ItemSalesInfo( typeof(	DDRelicJewels	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	JeweledShield	),	231	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	JewelerCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	JinBaori	),	20	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.Cloth	,	Market.Monk	),
			new ItemSalesInfo( typeof(	JokeBook	),	3000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jester	),
			new ItemSalesInfo( typeof(	JokerHat	),	12	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jester	),
			new ItemSalesInfo( typeof(	JokerRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jester	),
			new ItemSalesInfo( typeof(	Kama	),	61	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Weapon	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Kamishimo	),	16	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.Cloth	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Kasa	),	31	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.Cloth	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Katana	),	33	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Weapon	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Keg	),	38	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	Key	),	8	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	KeyRing	),	8	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	Kilt	),	11	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	Kindling	),	2	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Supplies	),
			new ItemSalesInfo( typeof(	KnifeLeft	),	2	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	KnifeRight	),	2	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	Kryss	),	32	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Krystal	),	10	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	Lajatang	),	108	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Weapon	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	LambLeg	),	8	,	15	,	0	,	false	,	false	,	World.None	,	Category.Supply	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	Lance	),	34	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	LandmineSetup	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	Lantern	),	2	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	LapHarp	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	LargeBackpack	),	10	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	LargeBag	),	10	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	LargeBagBall	),	3	,	15	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	LargeBattleAxe	),	33	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	LargeBedEastDeed	),	638	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	LargeBedSouthDeed	),	638	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	LargeBoatDeed	),	14000	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	LargeCrate	),	14	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	LargeCrystal	),	2000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	LargeDragonBoatDeed	),	15000	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	LargeDyingPlant	),	225	,	3	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Herbalist	),
			new ItemSalesInfo( typeof(	LargeFishingNetDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	LargeFlask	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	LargeForgeEastDeed	),	54	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	LargeForgeSouthDeed	),	54	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	LargeHollowBook	),	600	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Scribe	),
			new ItemSalesInfo( typeof(	LargeKnife	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	LargePegasusStatue	),	700	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	LargeSack	),	10	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	LargeStatueLion	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	LargeStatueWolf	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	LargeStoneTableEastDeed	),	960	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	LargeStoneTableSouthDeed	),	960	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	LargeTable	),	20	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	LargeVase	),	536	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	AniLargeVioletFlask	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	apiLargeWaxPot	),	400	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	BronzeShield	),	66	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	LawnTools	),	500	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	Leafblade	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	LearnGraniteBook	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	LearnLeatherBook	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	LearnMetalBook	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	LearnMiscBook	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	LearnReagentsBook	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	LearnScalesBook	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	LearnStealingBook	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	LearnTailorBook	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	LearnTitles	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	LearnTraps	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	LearnWoodBook	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	DDRelicLeather	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	LeatherArms	),	80	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	LeatherBoots	),	90	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Shoes	),
			new ItemSalesInfo( typeof(	LeatherBustierArms	),	97	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	LeatherCap	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	LeatherChest	),	101	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	LeatherCloak	),	120	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	LeatherDo	),	87	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	LeatherGloves	),	60	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	LeatherGorget	),	74	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	LeatherHaidate	),	54	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	LeatherHiroSode	),	49	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	LeatherJingasa	),	11	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Leather	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	HornedLeather	),	6	,	15	,	50	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	BarbedLeather	),	8	,	15	,	55	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	NecroticLeather	),	9	,	15	,	60	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	VolcanicLeather	),	10	,	15	,	65	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	SpinedLeather	),	13	,	15	,	70	,	false	,	false	,	World.Sea	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	FrozenLeather	),	11	,	15	,	75	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	GoliathLeather	),	14	,	15	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	DraconicLeather	),	15	,	15	,	85	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	HellishLeather	),	16	,	15	,	87	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	DinosaurLeather	),	18	,	15	,	89	,	false	,	false	,	World.Savage	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	AlienLeather	),	19	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	LeatherLegs	),	80	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	LeatherMempo	),	28	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	LeatherNinjaBelt	),	24	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.Cloth	,	Market.Monk	),
			new ItemSalesInfo( typeof(	LeatherNinjaHood	),	10	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	LeatherNinjaJacket	),	51	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	LeatherNinjaMitts	),	60	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	LeatherNinjaPants	),	49	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	LeatherRobe	),	160	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	LeatherSandals	),	60	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Shoes	),
			new ItemSalesInfo( typeof(	LeatherShoes	),	75	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Shoes	),
			new ItemSalesInfo( typeof(	LeatherShorts	),	86	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	LeatherSkirt	),	87	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	LeatherSoftBoots	),	120	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Shoes	),
			new ItemSalesInfo( typeof(	LeatherSuneate	),	55	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	LeatherThighBoots	),	105	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Shoes	),
			new ItemSalesInfo( typeof(	LeatherworkingTools	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	AdesoteLeather	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	BiomeshLeather	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	CerlinLeather	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	DurafiberLeather	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	FlexicrisLeather	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	HyperclothLeather	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	NylarLeather	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	NyloniteLeather	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	PolyfiberLeather	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	SynclothLeather	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	ThermoweaveLeather	),	25	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	LeftArm	),	20	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	LeftLeg	),	20	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	Lemon	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	Lettuce	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	LibrarianCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Scribe	),
			new ItemSalesInfo( typeof(	light_dragon_brazier	),	750	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	light_wall_torch	),	50	,	31	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	LightBrownTreeDeed	),	540	,	2	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	LightHouseAddonDeed	),	1600	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	LightPlateJingasa	),	56	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Lime	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	LocalMap	),	6	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cartographer	),
			new ItemSalesInfo( typeof(	Lockpick	),	12	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	Log	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Lumber	),
			new ItemSalesInfo( typeof(	AshLog	),	6	,	15	,	50	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Lumber	),
			new ItemSalesInfo( typeof(	CherryLog	),	6	,	15	,	55	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Lumber	),
			new ItemSalesInfo( typeof(	EbonyLog	),	8	,	15	,	60	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Lumber	),
			new ItemSalesInfo( typeof(	GoldenOakLog	),	8	,	15	,	65	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Lumber	),
			new ItemSalesInfo( typeof(	HickoryLog	),	10	,	15	,	70	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Lumber	),
			new ItemSalesInfo( typeof(	MahoganyLog	),	10	,	15	,	75	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Lumber	),
			new ItemSalesInfo( typeof(	OakLog	),	12	,	15	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Lumber	),
			new ItemSalesInfo( typeof(	PineLog	),	12	,	15	,	85	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Lumber	),
			new ItemSalesInfo( typeof(	GhostLog	),	12	,	15	,	87	,	false	,	false	,	World.Necro	,	Category.Resource	,	Material.None	,	Market.Lumber	),
			new ItemSalesInfo( typeof(	RosewoodLog	),	14	,	15	,	89	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Lumber	),
			new ItemSalesInfo( typeof(	WalnutLog	),	14	,	15	,	91	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Lumber	),
			new ItemSalesInfo( typeof(	PetrifiedLog	),	16	,	15	,	93	,	false	,	false	,	World.Underworld	,	Category.Resource	,	Material.None	,	Market.Lumber	),
			new ItemSalesInfo( typeof(	DriftwoodLog	),	10	,	15	,	95	,	false	,	false	,	World.Sea	,	Category.Resource	,	Material.None	,	Market.Lumber	),
			new ItemSalesInfo( typeof(	ElvenLog	),	28	,	15	,	97	,	false	,	false	,	World.Lodor	,	Category.Resource	,	Material.None	,	Market.Lumber	),
			new ItemSalesInfo( typeof(	LoinCloth	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	LongFlask	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	LongPants	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	Longsword	),	55	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	LoomEastDeed	),	376	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	LoomSouthDeed	),	376	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	LootChest	),	1200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	LoreGuidetoAdventure	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	Lute	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	Mace	),	28	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	MageEye	),	2	,	150	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	ClumsyScroll	),	10	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	CreateFoodScroll	),	10	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	FeeblemindScroll	),	10	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	HealScroll	),	10	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	MagicArrowScroll	),	10	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	NightSightScroll	),	10	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	ReactiveArmorScroll	),	10	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	WeakenScroll	),	10	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	AgilityScroll	),	20	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	CunningScroll	),	20	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	CureScroll	),	20	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	HarmScroll	),	20	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	MagicTrapScroll	),	20	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	MagicUnTrapScroll	),	20	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	ProtectionScroll	),	20	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	StrengthScroll	),	20	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	BlessScroll	),	30	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	FireballScroll	),	30	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	MagicLockScroll	),	30	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	PoisonScroll	),	30	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	TelekinisisScroll	),	30	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	TeleportScroll	),	30	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	UnlockScroll	),	30	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	WallOfStoneScroll	),	30	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	ArchCureScroll	),	40	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	ArchProtectionScroll	),	40	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	CurseScroll	),	40	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	FireFieldScroll	),	40	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	GreaterHealScroll	),	40	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	LightningScroll	),	40	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	ManaDrainScroll	),	40	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	RecallScroll	),	40	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	BladeSpiritsScroll	),	50	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	DispelFieldScroll	),	50	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	IncognitoScroll	),	50	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	MagicReflectScroll	),	50	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	MindBlastScroll	),	50	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	ParalyzeScroll	),	50	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	PoisonFieldScroll	),	50	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	SummonCreatureScroll	),	50	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	DispelScroll	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	EnergyBoltScroll	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	ExplosionScroll	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	InvisibilityScroll	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	MarkScroll	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	MassCurseScroll	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	ParalyzeFieldScroll	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	RevealScroll	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	ChainLightningScroll	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	EnergyFieldScroll	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	FlamestrikeScroll	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	GateTravelScroll	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	ManaVampireScroll	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	MassDispelScroll	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	MeteorSwarmScroll	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	PolymorphScroll	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	EarthquakeScroll	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	EnergyVortexScroll	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	ResurrectionScroll	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	SummonAirElementalScroll	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	SummonDaemonScroll	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	SummonEarthElementalScroll	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	SummonFireElementalScroll	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	SummonWaterElementalScroll	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	MagicalShortbow	),	42	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Wood	,	Market.Bow	),
			new ItemSalesInfo( typeof(	MagicWizardsHat	),	11	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Wizard	),
			new ItemSalesInfo( typeof(	MagistrateRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	MahjongGame	),	6	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	Mailbox	),	158	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	MaleKimono	),	18	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.Cloth	,	Market.Monk	),
			new ItemSalesInfo( typeof(	MalletAndChisel	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	ManyArrows100	),	200	,	10	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bow	),
			new ItemSalesInfo( typeof(	ManyArrows1000	),	2000	,	10	,	101	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bow	),
			new ItemSalesInfo( typeof(	ManyBolts100	),	200	,	10	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bow	),
			new ItemSalesInfo( typeof(	ManyBolts1000	),	2000	,	10	,	101	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bow	),
			new ItemSalesInfo( typeof(	PlaceMap	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cartographer	),
			new ItemSalesInfo( typeof(	MapleArmoire	),	198	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	MapmakersPen	),	8	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cartographer	),
			new ItemSalesInfo( typeof(	MapWorld	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cartographer	),
			new ItemSalesInfo( typeof(	MarbleStatueMaker	),	50000	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	MarbleWellDeed	),	500	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	MarlinEastAddonDeed	),	1600	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	MarlinSouthAddonDeed	),	1600	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	MasonryBook	),	10625	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	MasterSkeletonsKey	),	500	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	MaterialLiquifier	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	Maul	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	CubedMeat	),	7	,	0	,	0	,	false	,	false	,	World.None	,	Category.Supply	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	MeatPie	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Butcher	),
			new ItemSalesInfo( typeof(	MediumBoatDeed	),	12000	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	MediumCrate	),	12	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	MediumDragonBoatDeed	),	13000	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	MediumFlask	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	MediumStatueLion	),	700	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	MediumStoneTableEastDeed	),	760	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	MediumStoneTableSouthDeed	),	760	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	MedusaStatue	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	MegalodonTooth	),	4000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	DungeoneerCrate	),	500	,	1	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	MetalKiteShield	),	123	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	MetalSafe	),	5000	,	5	,	50	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Banker	),
			new ItemSalesInfo( typeof(	MetalShield	),	121	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	MetalVault	),	5000	,	5	,	50	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Banker	),
			new ItemSalesInfo( typeof(	MisoSoup	),	3	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	MongbatDartBoardEastDeed	),	1200	,	3	,	50	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	MongbatDartBoardSouthDeed	),	1200	,	3	,	50	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	MonkRobe	),	136	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.Cloth	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Monocle	),	24	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	MountedTrophyHead	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	MovingBox	),	500	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	Muffins	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	MusicianCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	MyCircusTentEastAddonDeed	),	1000	,	1	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jester	),
			new ItemSalesInfo( typeof(	MyCircusTentSouthAddonDeed	),	1000	,	1	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jester	),
			new ItemSalesInfo( typeof(	MysticSpellbook	),	190	,	5	,	0	,	false	,	false	,	World.Orient	,	Category.Book	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	MyTentEastAddonDeed	),	1000	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	MyTentSouthAddonDeed	),	1000	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	NecroHorse	),	10000	,	5	,	101	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	NecromancerBanner	),	350	,	3	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Death	),
			new ItemSalesInfo( typeof(	NecromancerCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Death	),
			new ItemSalesInfo( typeof(	NecromancerRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Necro	),
			new ItemSalesInfo( typeof(	NecromancerSpellbook	),	115	,	5	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	NecromancerTable	),	520	,	3	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Death	),
			new ItemSalesInfo( typeof(	CurseWeaponScroll	),	12	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	BloodOathScroll	),	25	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	CorpseSkinScroll	),	28	,	5	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	AnimateDeadScroll	),	52	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	EvilOmenScroll	),	52	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	PainSpikeScroll	),	52	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	SummonFamiliarScroll	),	52	,	5	,	101	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	HorrificBeastScroll	),	54	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	MindRotScroll	),	78	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	PoisonStrikeScroll	),	78	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	WraithFormScroll	),	102	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	LichFormScroll	),	128	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	StrangleScroll	),	128	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	WitherScroll	),	128	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	ExorcismScroll	),	144	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	VampiricEmbraceScroll	),	202	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	VengefulSpiritScroll	),	228	,	0	,	0	,	false	,	false	,	World.None	,	Category.Scroll	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	NecroSkinPotion	),	1000	,	1	,	0	,	false	,	false	,	World.Ambrosia	,	Category.Potion	,	Material.None	,	Market.Necro	),
			new ItemSalesInfo( typeof(	NeptunesFishingNet	),	320	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	NewArmoireA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewArmoireB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewArmoireC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewArmoireD	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewArmoireE	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewArmoireF	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewArmoireG	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewArmoireH	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewArmoireI	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewArmoireJ	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewArmorShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewArmorShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewArmorShelfC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewArmorShelfD	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewArmorShelfE	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBakerShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBakerShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBakerShelfC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBakerShelfD	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBakerShelfE	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBakerShelfF	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBakerShelfG	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBlacksmithShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBlacksmithShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBlacksmithShelfC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBlacksmithShelfD	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBlacksmithShelfE	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBookShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBookShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBookShelfC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBookShelfD	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBookShelfE	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBookShelfF	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBookShelfG	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBookShelfH	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBookShelfI	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBookShelfJ	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBookShelfK	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBookShelfL	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBookShelfM	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBowyerShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBowyerShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBowyerShelfC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewBowyerShelfD	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewCarpenterShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewCarpenterShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewCarpenterShelfC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewClothShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewClothShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewClothShelfC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewClothShelfD	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewClothShelfE	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewClothShelfF	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewClothShelfG	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewClothShelfH	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDarkBookShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDarkBookShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDarkShelf	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrawersA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrawersB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrawersC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrawersD	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrawersE	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrawersF	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrawersG	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrawersH	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrawersI	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrawersJ	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrawersK	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrawersL	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrawersM	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrawersN	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrinkShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrinkShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrinkShelfC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrinkShelfD	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewDrinkShelfE	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewHelmShelf	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewHolidayTree	),	980	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewHunterShelf	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewKitchenShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewKitchenShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewOldBookShelf	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewPotionShelf	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewRuinedBookShelf	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewShelfC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewShelfD	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewShelfE	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewShelfF	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewShelfG	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewShelfH	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewShoeShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewShoeShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewShoeShelfC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewShoeShelfD	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewSorcererShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewSorcererShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewSorcererShelfC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewSorcererShelfD	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewSupplyShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewSupplyShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewSupplyShelfC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewTailorShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewTailorShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewTailorShelfC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewTailorShelfD	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewTannerShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewTannerShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewTavernShelfC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewTavernShelfD	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewTavernShelfE	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewTavernShelfF	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewTinkerShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewTinkerShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewTinkerShelfC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewTortureShelf	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewWizardShelfA	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewWizardShelfB	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewWizardShelfC	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewWizardShelfD	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewWizardShelfE	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NewWizardShelfF	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	Nightstand	),	14	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	NinjaTabi	),	15	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.Cloth	,	Market.Monk	),
			new ItemSalesInfo( typeof(	NoDachi	),	82	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Weapon	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	NorseHelm	),	18	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Nunchaku	),	35	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Weapon	,	Material.Wood	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Obi	),	10	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.Cloth	,	Market.Monk	),
			new ItemSalesInfo( typeof(	ObsidianStone	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	OilCloth	),	8	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Assassin	),
			new ItemSalesInfo( typeof(	Onion	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	OniwabanBoots	),	120	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	OniwabanGloves	),	60	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	OniwabanHood	),	10	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	OniwabanLeggings	),	80	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	OniwabanTunic	),	101	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	DDRelicOrbs	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	OrderShield	),	256	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	IronOre	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	DullCopperOre	),	16	,	15	,	50	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	ShadowIronOre	),	24	,	15	,	55	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	CopperOre	),	32	,	15	,	60	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	BronzeOre	),	40	,	15	,	65	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	GoldOre	),	48	,	15	,	70	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	AgapiteOre	),	56	,	15	,	75	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	VeriteOre	),	64	,	15	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	ValoriteOre	),	72	,	15	,	85	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	NepturiteOre	),	72	,	15	,	90	,	false	,	false	,	World.Sea	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	ObsidianOre	),	72	,	15	,	93	,	false	,	false	,	World.Serpent	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	MithrilOre	),	96	,	15	,	95	,	false	,	false	,	World.Underworld	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	DwarvenOre	),	192	,	15	,	97	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	XormiteOre	),	96	,	0	,	97	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	AmethystStone	),	200	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	CaddelliteStone	),	200	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	EmeraldStone	),	200	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	GarnetStone	),	200	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	IceStone	),	200	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	JadeStone	),	200	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	MarbleStone	),	200	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	OnyxStone	),	200	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	QuartzStone	),	200	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	RubyStone	),	200	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	SapphireStone	),	200	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	SilverStone	),	200	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	SpinelStone	),	200	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	StarRubyStone	),	200	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	TopazStone	),	200	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	OrnateRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	OrnateWoodenChest	),	158	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	PackedCostume	),	230	,	3	,	95	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	PaintCanvas	),	500	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Painter	),
			new ItemSalesInfo( typeof(	DDRelicPainting	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Painter	),
			new ItemSalesInfo( typeof(	PaladinWarhorse	),	10000	,	10	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Paladin	),
			new ItemSalesInfo( typeof(	PandorasBox	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	PaperLantern	),	16	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Peach	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	PeachCobbler	),	10	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	PeachTreeDeed	),	640	,	2	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	Pear	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	PearTreeDeed	),	640	,	2	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	PeculiarFish	),	240	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	PentagramDeed	),	440	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	PewterBowlOfCorn	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	PewterBowlOfFoodPotatos	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	PewterBowlOfLettuce	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	PewterBowlOfPeas	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	PewterMug	),	4	,	5	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	PhillipsWoodenSteed	),	5000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	Pickaxe	),	25	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	PickpocketDipEastDeed	),	292	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	PickpocketDipSouthDeed	),	292	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	Pike	),	39	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	PinkFancyRugDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	PirateChest	),	1600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	PirateCoat	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	PirateHat	),	8	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	PiratePants	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	Pitchfork	),	19	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Pitchforks	),	19	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	PlainDress	),	13	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	PlainLowTable	),	174	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	PlainWoodenChest	),	158	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	PlasmaGrenade	),	76	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	PlasmaTorch	),	180	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	Plate	),	4	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	PlateArms	),	188	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	PlateBattleKabuto	),	94	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	PlateChest	),	243	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	PlateDo	),	310	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	PlateGloves	),	155	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	PlateGorget	),	104	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	PlateHaidate	),	235	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	PlateHatsuburi	),	76	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	PlateHelm	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	PlateHiroSode	),	222	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	PlateLegs	),	218	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	PlateMempo	),	76	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	PlateSkirt	),	218	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	PlateSuneate	),	224	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	PlayerBBEast	),	236	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	PlayerBBSouth	),	236	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	PlayersHouseTeleporter	),	4000	,	10	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	PlayersZTeleporter	),	2000	,	10	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	LesserPoisonPotion	),	15	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Assassin	),
			new ItemSalesInfo( typeof(	PoisonPotion	),	30	,	15	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Assassin	),
			new ItemSalesInfo( typeof(	GreaterPoisonPotion	),	60	,	15	,	80	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Assassin	),
			new ItemSalesInfo( typeof(	DeadlyPoisonPotion	),	120	,	10	,	90	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Assassin	),
			new ItemSalesInfo( typeof(	SilverSerpentVenom	),	280	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Assassin	),
			new ItemSalesInfo( typeof(	LethalPoisonPotion	),	320	,	2	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Assassin	),
			new ItemSalesInfo( typeof(	GoldenSerpentVenom	),	420	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Assassin	),
			new ItemSalesInfo( typeof(	PolishBoneBrush	),	12	,	10	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	PortableSmelter	),	520	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	PotionKeg	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	PottedCactusDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Herbalist	),
			new ItemSalesInfo( typeof(	Pouch	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	PowderOfTranslocation	),	500	,	20	,	90	,	false	,	false	,	World.Lodor	,	Category.None	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	PowerCoil	),	20000	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	PowerCrystal	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	PriestRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	PrizedFish	),	240	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	ProphetRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Sage	),
			new ItemSalesInfo( typeof(	ProvisionerCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	PugilistGlove	),	20	,	0	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	PugilistGloves	),	20	,	0	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	PugilistMits	),	18	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	Pumpkin	),	11	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	PumpkinGiant	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	PumpkinGreen	),	150	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	PumpkinLarge	),	50	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	PumpkinPie	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	PumpkinScarecrow	),	240	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	PumpkinTall	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	PuzzleCube	),	180	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	QuarterStaff	),	19	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Wood	,	Market.Wizard	),
			new ItemSalesInfo( typeof(	Quiche	),	12	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	RadiantScimitar	),	35	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RangerArms	),	87	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Ranger	),
			new ItemSalesInfo( typeof(	RangerChest	),	128	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Ranger	),
			new ItemSalesInfo( typeof(	RangerGloves	),	79	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Ranger	),
			new ItemSalesInfo( typeof(	RangerGorget	),	73	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Ranger	),
			new ItemSalesInfo( typeof(	RangerLegs	),	103	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Ranger	),
			new ItemSalesInfo( typeof(	RareAnvil	),	3000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RawBird	),	9	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Butcher	),
			new ItemSalesInfo( typeof(	RawChickenLeg	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Butcher	),
			new ItemSalesInfo( typeof(	RawFishSteak	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	RawLambLeg	),	9	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Butcher	),
			new ItemSalesInfo( typeof(	RawPig	),	18	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Butcher	),
			new ItemSalesInfo( typeof(	RawRibs	),	16	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Butcher	),
			new ItemSalesInfo( typeof(	DDRelicReagent	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_NAHW	),
			new ItemSalesInfo( typeof(	BatWing	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_NAHW	),
			new ItemSalesInfo( typeof(	Garlic	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_MAHD	),
			new ItemSalesInfo( typeof(	Ginseng	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_MAHD	),
			new ItemSalesInfo( typeof(	GraveDust	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_NAHW	),
			new ItemSalesInfo( typeof(	MandrakeRoot	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_MAHD	),
			new ItemSalesInfo( typeof(	SpidersSilk	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_MAHD	),
			new ItemSalesInfo( typeof(	SulfurousAsh	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_MAHD	),
			new ItemSalesInfo( typeof(	Nightshade	),	4	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_MAHD	),
			new ItemSalesInfo( typeof(	BitterRoot	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHW	),
			new ItemSalesInfo( typeof(	BlackPearl	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_MAHDW	),
			new ItemSalesInfo( typeof(	Bloodmoss	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_MAHDW	),
			new ItemSalesInfo( typeof(	BloodRose	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHW	),
			new ItemSalesInfo( typeof(	Maggot	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHW	),
			new ItemSalesInfo( typeof(	PigIron	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_NAHW	),
			new ItemSalesInfo( typeof(	VioletFungus	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHW	),
			new ItemSalesInfo( typeof(	Wolfsbane	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHW	),
			new ItemSalesInfo( typeof(	BeetleShell	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHDW	),
			new ItemSalesInfo( typeof(	Brimstone	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHDW	),
			new ItemSalesInfo( typeof(	ButterflyWings	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHD	),
			new ItemSalesInfo( typeof(	DaemonBlood	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_NAHW	),
			new ItemSalesInfo( typeof(	EyeOfToad	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHDW	),
			new ItemSalesInfo( typeof(	FairyEgg	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHD	),
			new ItemSalesInfo( typeof(	GargoyleEar	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHW	),
			new ItemSalesInfo( typeof(	MoonCrystal	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHDW	),
			new ItemSalesInfo( typeof(	NoxCrystal	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_NAHW	),
			new ItemSalesInfo( typeof(	PixieSkull	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHW	),
			new ItemSalesInfo( typeof(	RedLotus	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHDW	),
			new ItemSalesInfo( typeof(	SeaSalt	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHD	),
			new ItemSalesInfo( typeof(	SilverWidow	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHDW	),
			new ItemSalesInfo( typeof(	SwampBerries	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHDW	),
			new ItemSalesInfo( typeof(	BlackSand	),	7	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHW	),
			new ItemSalesInfo( typeof(	DriedToad	),	7	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHW	),
			new ItemSalesInfo( typeof(	MummyWrap	),	7	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHW	),
			new ItemSalesInfo( typeof(	WerewolfClaw	),	7	,	15	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AHW	),
			new ItemSalesInfo( typeof(	DemigodBlood	),	240	,	0	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AH	),
			new ItemSalesInfo( typeof(	DemonClaw	),	240	,	0	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_NA	),
			new ItemSalesInfo( typeof(	DragonBlood	),	240	,	0	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AH	),
			new ItemSalesInfo( typeof(	DragonTooth	),	240	,	0	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AH	),
			new ItemSalesInfo( typeof(	GhostlyDust	),	240	,	0	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_NA	),
			new ItemSalesInfo( typeof(	LichDust	),	240	,	0	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_NA	),
			new ItemSalesInfo( typeof(	UnicornHorn	),	240	,	0	,	0	,	false	,	false	,	World.None	,	Category.Reagent	,	Material.None	,	Market.Reg_AH	),
			new ItemSalesInfo( typeof(	reagents_magic_jar1	),	2000	,	15	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Res_MAHD	),
			new ItemSalesInfo( typeof(	reagents_magic_jar2	),	1500	,	15	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Res_NAHW	),
			new ItemSalesInfo( typeof(	reagents_magic_jar3	),	5000	,	15	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Res_MAHD	),
			new ItemSalesInfo( typeof(	ReaperCowl	),	28	,	10	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	ReaperHood	),	28	,	10	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	RecallRune	),	15	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	ReceiverCrystal	),	6	,	15	,	90	,	false	,	false	,	World.Lodor	,	Category.None	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	RedArmoire	),	198	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	RedHangingLantern	),	50	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	RedMisoSoup	),	3	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	RedPlainRugDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	RedPoinsettia	),	120	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	AniRedRibbedFlask	),	110	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	RedStocking	),	110	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	RedWellDeed	),	500	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	RepeatingCrossbow	),	46	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Wood	,	Market.Bow	),
			new ItemSalesInfo( typeof(	RibbonTree	),	800	,	3	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	RibbonTreeSmall	),	700	,	3	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Druid	),
			new ItemSalesInfo( typeof(	RibCage	),	20	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	Ribs	),	7	,	15	,	0	,	false	,	false	,	World.None	,	Category.Supply	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	RightArm	),	20	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	RightLeg	),	20	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	RingmailArms	),	85	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RingmailChest	),	121	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RingmailGloves	),	93	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RingmailLegs	),	90	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RingmailSkirt	),	90	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RoastPig	),	106	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	Robe	),	18	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	RobotBatteries	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	RobotBolt	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	RobotCircuitBoard	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	RobotEngineParts	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	RobotGears	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	RobotOil	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	RobotSchematics	),	1500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	RobotSheetMetal	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	RobotTransistor	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	RockUrn	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	RockVase	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	RomulanAle	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	RoseOfMoon	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	RoundPaperLantern	),	16	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	RoyalArms	),	188	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RoyalBoots	),	104	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RoyalCape	),	16	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	RoyalChest	),	243	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RoyalCoat	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	RoyalGloves	),	155	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RoyalGorget	),	104	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RoyalHelm	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RoyalLoinCloth	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	RoyalLongSkirt	),	12	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	RoyalRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	RoyalShield	),	231	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RoyalShirt	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	RoyalSkirt	),	11	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	RoyalsLegs	),	218	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RoyalVest	),	12	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	DDRelicRugAddonDeed	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	RuinedTapestry	),	135	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	Runebook	),	3500	,	3	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	An	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Bet	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	RuneBlade	),	55	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Corp	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Des	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Ex	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Flam	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Grav	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Hur	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	In	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Jux	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Kal	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Lor	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Mani	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Nox	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Ort	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Por	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Quas	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Rel	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Sanct	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Tym	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Uus	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Vas	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Wis	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Xen	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Ylem	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Zu	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	WheatSheaf	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	MagicRuneBag	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rune	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	RusticShirt	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	RusticVest	),	12	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	SackFlour	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Mill	),
			new ItemSalesInfo( typeof(	Safe	),	60000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Banker	),
			new ItemSalesInfo( typeof(	SageRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Sage	),
			new ItemSalesInfo( typeof(	Sai	),	56	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Weapon	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	SailorCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	SailorPants	),	7	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	SamuraiTabi	),	16	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.Cloth	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Sandals	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Shoes	),
			new ItemSalesInfo( typeof(	Sand	),	3	,	30	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Glass	),
			new ItemSalesInfo( typeof(	SandMiningBook	),	10637	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Glass	),
			new ItemSalesInfo( typeof(	Sausage	),	18	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	SausagePizza	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	ScaledArms	),	208	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ScaledChest	),	262	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ScaledGloves	),	164	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ScaledGorget	),	95	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ScaledHelm	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ScaledLegs	),	238	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ScaledShield	),	250	,	0	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DragonArms	),	188	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DragonChest	),	242	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DragonGloves	),	144	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DragonHelm	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DragonLegs	),	218	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ScalemailShield	),	230	,	0	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	BlackScales	),	64	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	BlueScales	),	64	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	BrazenScales	),	64	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	DinosaurScales	),	64	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	GreenScales	),	64	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	MetallicScales	),	64	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	PlatinumScales	),	64	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	RedScales	),	64	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	UmberScales	),	64	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	VioletScales	),	64	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	WhiteScales	),	64	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	YellowScales	),	64	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	CadalyteScales	),	128	,	0	,	95	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	GornScales	),	100	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	TrandoshanScales	),	100	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	SilurianScales	),	100	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	KraytScales	),	100	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Scales	),	8	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	ScalingTools	),	21	,	15	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ScalyArms	),	158	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ScalyBoots	),	50	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ScalyChest	),	212	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ScalyGloves	),	114	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ScalyGorget	),	55	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ScalyHelm	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ScalyLegs	),	188	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Scales	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ScarecrowDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	Scepter	),	39	,	31	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ScholarRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Sage	),
			new ItemSalesInfo( typeof(	SciFiJunk	),	1	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	Scimitar	),	36	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Scissors	),	11	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	ScribesPen	),	16	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Scribe	),
			new ItemSalesInfo( typeof(	DDRelicScrolls	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Scythe	),	39	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	SeaChart	),	18	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	SeahorseStatuette	),	1600	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	HighSeasRelic	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	SeaShell	),	240	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	SewingKit	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	Sextant	),	13	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	MagicSextant	),	1000	,	5	,	0	,	false	,	false	,	World.Underworld	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	SextantParts	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	Shaft	),	3	,	60	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Bow	),
			new ItemSalesInfo( typeof(	ShantyTools	),	400	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	SheafOfHay	),	2	,	15	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Mill	),
			new ItemSalesInfo( typeof(	ShepherdsCrook	),	20	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Wood	,	Market.Stable	),
			new ItemSalesInfo( typeof(	ShinobiCowl	),	10	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	ShinobiHood	),	10	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	ShinobiMask	),	10	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	ShinobiRobe	),	160	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	ShinobiScroll	),	280	,	5	,	0	,	false	,	false	,	World.Orient	,	Category.Book	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	ShipModelOfTheHMSCape	),	1600	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	ShipwreckedItem	),	120	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	Shirt	),	12	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	Shoes	),	8	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Shoes	),
			new ItemSalesInfo( typeof(	ShojiLantern	),	16	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	ShojiScreen	),	334	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	ShortCabinet	),	178	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ShortMusicStand	),	94	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	ShortPants	),	7	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	ShortSpear	),	23	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	ShortSword	),	35	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Spade	),	12	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Miner	),
			new ItemSalesInfo( typeof(	Shuriken	),	18	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Weapon	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	SavageArms	),	106	,	3	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Bone	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	SavageChest	),	148	,	3	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Bone	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	SavageGloves	),	98	,	3	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Bone	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	SavageHelm	),	44	,	3	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Bone	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	SavageLegs	),	122	,	3	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Bone	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	SkeletonsKey	),	100	,	1	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	DemonSkins	),	50	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	DragonSkins	),	50	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	NightmareSkins	),	50	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	SnakeSkins	),	50	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	TrollSkins	),	50	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	UnicornSkins	),	50	,	0	,	80	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	DeadSkins	),	50	,	0	,	90	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	IcySkins	),	50	,	0	,	90	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	LavaSkins	),	50	,	0	,	90	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	Seaweeds	),	50	,	0	,	90	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Leather	),
			new ItemSalesInfo( typeof(	SkinningKnifeTool	),	15	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Butcher	),
			new ItemSalesInfo( typeof(	Skirt	),	12	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	SkullCap	),	7	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	SkullDemon	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	SkullDinosaur	),	350	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	SkullDragon	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	SkullEastLargeAddonDeed	),	1600	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	SkullEastSmallAddonDeed	),	1600	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	SkullGiant	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	SkullGreatDragon	),	1200	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	SkullMinotaur	),	300	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	SkullMug	),	5	,	5	,	0	,	false	,	false	,	World.Necro	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	SkullsOnPike	),	120	,	3	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	SkullSouthLargeAddonDeed	),	1600	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	SkullSouthSmallAddonDeed	),	1600	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	SkullWyrm	),	800	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	SlaversNet	),	4000	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	SmallBagBall	),	3	,	15	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	SmallBedEastDeed	),	438	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	SmallBedSouthDeed	),	438	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	AniSmallBlueFlask	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	SmallBoatDeed	),	10000	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	SmallCrate	),	10	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	SmallDragonBoatDeed	),	11000	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	SmallFishingNetDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	SmallFlask	),	50	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	SmallForgeDeed	),	52	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	SmallHollowBook	),	500	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Scribe	),
			new ItemSalesInfo( typeof(	SmallPlateJingasa	),	56	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	SmallStatueAngel	),	300	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	SmallStatueDragon	),	300	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	SmallStatueLion	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	SmallStatueMan	),	300	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	SmallStatueNoble	),	300	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	SmallStatuePegasus	),	300	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	SmallStatueSkull	),	300	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	SmallStatueWoman	),	300	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	SmallTowerSculpture	),	776	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	SmallUrn	),	776	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	apiSmallWaxPot	),	250	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	SmithHammer	),	23	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Snowman	),	230	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	SnowPileDeco	),	80	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	SongBook	),	24	,	5	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	SorcererRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Wizard	),
			new ItemSalesInfo( typeof(	Spear	),	31	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	SpecialBeardDye	),	500	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Barber	),
			new ItemSalesInfo( typeof(	SpecialFishingNet	),	160	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	SpecialHairDye	),	500	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Barber	),
			new ItemSalesInfo( typeof(	SpecialSeaweed	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	Spellbook	),	18	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	SphinxStatue	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	SpiderRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	SpikedClub	),	28	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	SpinningHourglass	),	140	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	SpinningwheelEastDeed	),	332	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	SpinningwheelSouthDeed	),	332	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	SpoolOfThread	),	4	,	-1	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	SpoonLeft	),	2	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	SpoonRight	),	2	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	Springs	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	Spyglass	),	12	,	5	,	50	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	SquareGozaMatEastDeed	),	70	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	SquareGozaMatSouthDeed	),	70	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Squash	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	SquireShirt	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	StableCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	StableStone	),	5000	,	3	,	50	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	StagCap	),	50	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	StandardPlateKabuto	),	74	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	StandingBrokenChairDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	DDRelicStatue	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	StatueAdventurer	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueAmazon	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueAngelTall	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueBust	),	300	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueCapeWarrior	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueDaemon	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueDemonicFace	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueDesertGod	),	700	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueDruid	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueDwarf	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueEast	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueElvenKnight	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueElvenPriestess	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueElvenSorceress	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueElvenWarrior	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueFighter	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueGargoyleBust	),	300	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueGargoyleTall	),	700	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueGateGuardian	),	1500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueGiantWarrior	),	1500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueGryphon	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueGuardian	),	1500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueHorseRider	),	700	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueMermaid	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueMinotaurAttack	),	700	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueMinotaurDefend	),	700	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueNoble	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueNorth	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatuePegasus	),	720	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatuePriest	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueSeaHorse	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueSouth	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueSwordsman	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueWiseManTall	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueWizard	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueWolfWinged	),	700	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueWomanTall	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StatueWomanWarriorPillar	),	700	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StealBag	),	1	,	0	,	0	,	false	,	true	,	World.None	,	Category.None	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	StealBox	),	1	,	0	,	0	,	false	,	true	,	World.None	,	Category.None	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	StealMetalBox	),	1	,	0	,	0	,	false	,	true	,	World.None	,	Category.None	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	StolenChest	),	1	,	0	,	0	,	false	,	true	,	World.None	,	Category.None	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	StoneAmphora	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneAnkhDeed	),	5000	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneBenchLong	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneBenchShort	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneBlock	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneBuddhistSculpture	),	90	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneCasket	),	180	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneChair	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneChairs	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneCoffin	),	180	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneColumn	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneFancyPedestal	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneGargoyleVase	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneGothicColumn	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneLargeAmphora	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneLargeVase	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneMingSculpture	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneMingUrn	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneMiningBook	),	10625	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneOrnateAmphora	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneOrnateUrn	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneOrnateVase	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneOvenEastDeed	),	370	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneOvenSouthDeed	),	370	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StonePedestal	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneQinSculpture	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneQinUrn	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneRoughPillar	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneSarcophagus	),	400	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneSteps	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneTableLong	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneTableShort	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneTombStoneA	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneB	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneC	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneD	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneE	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneF	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneG	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneH	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneI	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneJ	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneK	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneL	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneM	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneN	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneO	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneP	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneQ	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneR	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneS	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneTombStoneT	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Halloween	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	StoneVase	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneWellDeed	),	500	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	StoneWizardTable	),	1160	,	0	,	0	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneYuanSculpture	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	StoneYuanUrn	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	Stool	),	12	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	StrawHat	),	7	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	StuddedArms	),	87	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	StuddedBustierArms	),	120	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	StuddedChest	),	128	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	StuddedDo	),	130	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	StuddedGloves	),	79	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	StuddedGorget	),	73	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	StuddedHaidate	),	76	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	StuddedHiroSode	),	73	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	StuddedLegs	),	103	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	StuddedMempo	),	61	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	StuddedSkirt	),	103	,	15	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	StuddedSuneate	),	78	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Armor	,	Material.Leather	,	Market.Monk	),
			new ItemSalesInfo( typeof(	SunkenBag	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	SunkenChest	),	1600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	SunShield	),	256	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	SupplyCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	Surcoat	),	14	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	SushiPlatter	),	3	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	SushiRolls	),	3	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	SweetDough	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	SwordsAndShackles	),	50	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	DDRelicTablet	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	TableWithBlueClothDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	TableWithOrangeClothDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	TableWithPurpleClothDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	TableWithRedClothDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	TailorCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	TallCabinet	),	178	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	TallMusicStand	),	114	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	TallStatueLion	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	TallStrawHat	),	8	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	Tambourine	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	TambourineTassel	),	86	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	TapestryOfSosaria	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	TarotDeck	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	tarotpoker	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	TastyHeart	),	40	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	TattsukeHakama	),	16	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.Cloth	,	Market.Monk	),
			new ItemSalesInfo( typeof(	TavernCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	TavernTable	),	1100	,	30	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	Tekagi	),	55	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Weapon	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	TenFootPole	),	500	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	CampersTent	),	500	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Supplies	),
			new ItemSalesInfo( typeof(	SmallTent	),	200	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Supplies	),
			new ItemSalesInfo( typeof(	Tessen	),	83	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Weapon	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Tetsubo	),	43	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Weapon	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	ThermalDetonator	),	76	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	ThighBoots	),	15	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Shoes	),
			new ItemSalesInfo( typeof(	ThinLongsword	),	27	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Throne	),	48	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	ThrowingGloves	),	26	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	ThrowingWeapon	),	2	,	120	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	TinkerCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	TinkerTools	),	6	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	DDRelicGrave	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	TomeOfWands	),	800	,	0	,	0	,	false	,	false	,	World.None	,	Category.Wand	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	Torch	),	8	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Provisions	),
			new ItemSalesInfo( typeof(	Torso	),	20	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	TrainingDaemonEastDeed	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fighter	),
			new ItemSalesInfo( typeof(	TrainingDaemonSouthDeed	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fighter	),
			new ItemSalesInfo( typeof(	TrainingDummyEastDeed	),	250	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fighter	),
			new ItemSalesInfo( typeof(	TrainingDummySouthDeed	),	250	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fighter	),
			new ItemSalesInfo( typeof(	TrapKit	),	420	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Thief	),
			new ItemSalesInfo( typeof(	TreasureCrate	),	400	,	5	,	50	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Banker	),
			new ItemSalesInfo( typeof(	TreasurePile01AddonDeed	),	12000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Banker	),
			new ItemSalesInfo( typeof(	TreasurePile02AddonDeed	),	12000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Banker	),
			new ItemSalesInfo( typeof(	TreasurePile03AddonDeed	),	12000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Banker	),
			new ItemSalesInfo( typeof(	TreasurePile04AddonDeed	),	12000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Banker	),
			new ItemSalesInfo( typeof(	TreasurePile05AddonDeed	),	12000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Banker	),
			new ItemSalesInfo( typeof(	TreasurePile2AddonDeed	),	20000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Banker	),
			new ItemSalesInfo( typeof(	TreasurePile3AddonDeed	),	20000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Banker	),
			new ItemSalesInfo( typeof(	TreasurePileAddonDeed	),	20000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Banker	),
			new ItemSalesInfo( typeof(	TricorneHat	),	8	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Sailor	),
			new ItemSalesInfo( typeof(	TrinketTalisman	),	200	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	TrulyRareFish	),	240	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	Trumpet	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Bard	),
			new ItemSalesInfo( typeof(	Tub	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	Tunic	),	18	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	TwoHandedAxe	),	32	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	UnbakedApplePie	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	UnbakedFruitPie	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	UnbakedMeatPie	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	UnbakedPeachCobbler	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	UnbakedPumpkinPie	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	UnbakedQuiche	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	UncookedCheesePizza	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	UncookedSausagePizza	),	30	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	UndertakerKit	),	6	,	20	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Undertaker	),
			new ItemSalesInfo( typeof(	UnmadeBedDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	VagabondRobe	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	VampireHead	),	600	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	VanityDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	DDRelicVase	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	Vase	),	456	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stone	),
			new ItemSalesInfo( typeof(	VendorRentalContract	),	1252	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Inn	),
			new ItemSalesInfo( typeof(	VirtueShield	),	256	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Wakizashi	),	38	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Weapon	,	Material.Metal	,	Market.Monk	),
			new ItemSalesInfo( typeof(	WallBannerDeed	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Art	),
			new ItemSalesInfo( typeof(	WallSconce	),	120	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	WallTorch	),	50	,	20	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	MagicalWand	),	1	,	0	,	0	,	false	,	false	,	World.None	,	Category.Wand	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	WarAxe	),	29	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	WarCleaver	),	25	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	WarHammer	),	25	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Warhorse	),	10000	,	10	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Paladin	),
			new ItemSalesInfo( typeof(	WarFork	),	31	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	WarMace	),	31	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	Wasabi	),	2	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	WasabiClumps	),	34	,	0	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	WaterBarrel	),	5000	,	1	,	95	,	false	,	false	,	World.None	,	Category.Rare	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	Watermelon	),	7	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	WaterTroughEastDeed	),	638	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	WaterTroughSouthDeed	),	638	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	WaxingPot	),	50	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	WaxPainting	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Painter	),
			new ItemSalesInfo( typeof(	WaxPaintingA	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Painter	),
			new ItemSalesInfo( typeof(	WaxPaintingB	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Painter	),
			new ItemSalesInfo( typeof(	WaxPaintingC	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Painter	),
			new ItemSalesInfo( typeof(	WaxPaintingD	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Painter	),
			new ItemSalesInfo( typeof(	WaxPaintingE	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Painter	),
			new ItemSalesInfo( typeof(	WaxPaintingF	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Painter	),
			new ItemSalesInfo( typeof(	WaxPaintingG	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Painter	),
			new ItemSalesInfo( typeof(	WaxSculptors	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	WaxSculptorsA	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	WaxSculptorsB	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	WaxSculptorsC	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	WaxSculptorsD	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	WaxSculptorsE	),	1000	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Wax	),
			new ItemSalesInfo( typeof(	DDRelicWeapon	),	1	,	0	,	200	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Smith	),
			new ItemSalesInfo( typeof(	WeaponAbilityBook	),	5	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Fighter	),
			new ItemSalesInfo( typeof(	Whip	),	16	,	0	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	Whips	),	16	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	WhiteHangingLantern	),	50	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	WhiteMisoSoup	),	3	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.None	,	Material.None	,	Market.Monk	),
			new ItemSalesInfo( typeof(	WhitePoinsettia	),	120	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	WhiteSnowflake	),	100	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	WideBrimHat	),	8	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	WildStaff	),	20	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Wood	,	Market.Druid	),
			new ItemSalesInfo( typeof(	WindChimes	),	20	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Tinker	),
			new ItemSalesInfo( typeof(	UndeadEyesScroll	),	25	,	3	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	NecroUnlockScroll	),	30	,	3	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	NecroPoisonScroll	),	35	,	3	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	PhantasmScroll	),	40	,	3	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	RetchedAirScroll	),	45	,	3	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	ManaLeechScroll	),	50	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	WallOfSpikesScroll	),	55	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	NecroCurePoisonScroll	),	60	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	BloodPactScroll	),	65	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	SpectreShadowScroll	),	70	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	GhostPhaseScroll	),	75	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	GhostlyImagesScroll	),	80	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	HellsGateScroll	),	85	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	HellsBrandScroll	),	90	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	GraveyardGatewayScroll	),	95	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	VampireGiftScroll	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	WitchCauldron	),	16	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	WitchHat	),	11	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Witch	),
			new ItemSalesInfo( typeof(	WitchPouch	),	1200	,	2	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Witch	),
			new ItemSalesInfo( typeof(	WizardHood	),	12	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Wizard	),
			new ItemSalesInfo( typeof(	WizardryCrate	),	400	,	5	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Mage	),
			new ItemSalesInfo( typeof(	WizardsHat	),	11	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Wizard	),
			new ItemSalesInfo( typeof(	WizardShirt	),	21	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Cloth	,	Market.Wizard	),
			new ItemSalesInfo( typeof(	WizardStaff	),	40	,	5	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Mage	),
			new ItemSalesInfo( typeof(	BlackStaff	),	22	,	15	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Necro	),
			new ItemSalesInfo( typeof(	WizardStick	),	38	,	5	,	0	,	false	,	false	,	World.None	,	Category.Weapon	,	Material.Metal	,	Market.Mage	),
			new ItemSalesInfo( typeof(	WolfCap	),	50	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.Leather	,	Market.Leather	),
			new ItemSalesInfo( typeof(	WondrousFish	),	240	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Fisherman	),
			new ItemSalesInfo( typeof(	WoodenBench	),	12	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	WoodenBowlOfCarrots	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	WoodenBowlOfCorn	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	WoodenBowlOfLettuce	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	WoodenBowlOfPeas	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	WoodenBowlOfStew	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	WoodenBowlOfTomatoSoup	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.Tavern	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	WoodenBox	),	14	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	WoodenCasket	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	WoodenChair	),	16	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	WoodenChairCushion	),	20	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	WoodenChest	),	30	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	WoodenCoffin	),	100	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Evil	),
			new ItemSalesInfo( typeof(	WoodenFootLocker	),	158	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	WoodenKiteShield	),	70	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Wood	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	WoodenPlateArms	),	188	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Wood	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	WoodenPlateChest	),	242	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Wood	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	WoodenPlateGloves	),	144	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Wood	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	WoodenPlateGorget	),	104	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Wood	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	WoodenPlateHelm	),	20	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Wood	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	WoodenPlateLegs	),	218	,	0	,	0	,	false	,	false	,	World.None	,	Category.Armor	,	Material.Wood	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	WoodenShield	),	30	,	15	,	0	,	false	,	false	,	World.None	,	Category.Shield	,	Material.Wood	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	WoodenThrone	),	12	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	WoodWellDeed	),	500	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Home	),
			new ItemSalesInfo( typeof(	WoodworkingTools	),	10	,	30	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	Wool	),	4	,	-1	,	0	,	false	,	false	,	World.None	,	Category.Resource	,	Material.None	,	Market.Tailor	),
			new ItemSalesInfo( typeof(	WorkShoppes	),	50	,	15	,	0	,	false	,	false	,	World.None	,	Category.Book	,	Material.None	,	Market.Scribe	),
			new ItemSalesInfo( typeof(	WorldMap	),	6	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cartographer	),
			new ItemSalesInfo( typeof(	WrappedCandy	),	20	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cook	),
			new ItemSalesInfo( typeof(	WreathDeed	),	300	,	3	,	0	,	false	,	false	,	World.None	,	Category.Christmas	,	Material.None	,	Market.None	),
			new ItemSalesInfo( typeof(	WritingTable	),	96	,	1	,	95	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Sage	),
			new ItemSalesInfo( typeof(	ElixirAlchemy	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirAnatomy	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirAnimalLore	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirAnimalTaming	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirArchery	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirArmsLore	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirBegging	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirBlacksmith	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirCamping	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirCarpentry	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirCartography	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirCooking	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirDetectHidden	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirDiscordance	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirEvalInt	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirFencing	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirFishing	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirFletching	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirFocus	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirForensics	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirHealing	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirHerding	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirHiding	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirInscribe	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirItemID	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirLockpicking	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirLumberjacking	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirMacing	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirMagicResist	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirMeditation	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirMining	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirMusicianship	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirParry	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirPeacemaking	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirPoisoning	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirProvocation	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirRemoveTrap	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirSnooping	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirSpiritSpeak	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirStealing	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirStealth	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirSwords	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirTactics	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirTailoring	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirTasteID	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirTinkering	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirTracking	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirVeterinary	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	ElixirWrestling	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	MixtureDiseasedSlime	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	MixtureFireSlime	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	MixtureIceSlime	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	LiquidFire	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	LiquidGoo	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	LiquidIce	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	LiquidPain	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	LiquidRot	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	MixtureRadiatedSlime	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	MixtureSlime	),	70	,	1	,	95	,	false	,	false	,	World.None	,	Category.Potion	,	Material.None	,	Market.Alchemy	),
			new ItemSalesInfo( typeof(	YellowGourd	),	3	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Farmer	),
			new ItemSalesInfo( typeof(	YewWoodTable	),	20	,	1	,	90	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Carpenter	),
			new ItemSalesInfo( typeof(	Yumi	),	53	,	15	,	0	,	false	,	false	,	World.Orient	,	Category.Weapon	,	Material.Wood	,	Market.Monk	),
			new ItemSalesInfo( typeof(	Amber	),	50	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	Amethyst	),	90	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	CagedAlligator	),	1520	,	1	,	50	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedApe	),	3120	,	1	,	25	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedBlackBear	),	855	,	1	,	10	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedBlackWolf	),	2400	,	1	,	10	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedBoar	),	500	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedBobcat	),	2240	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedBrownBear	),	855	,	1	,	10	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedBull	),	800	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cattle	),
			new ItemSalesInfo( typeof(	CagedCat	),	132	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedCaveBearRiding	),	4230	,	1	,	50	,	false	,	false	,	World.Serpent	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedChicken	),	100	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cattle	),
			new ItemSalesInfo( typeof(	CagedCougar	),	1120	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedCow	),	600	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cattle	),
			new ItemSalesInfo( typeof(	CagedDesertOstard	),	700	,	5	,	25	,	false	,	false	,	World.Lodor	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedDireBear	),	2140	,	1	,	50	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedDireBoar	),	900	,	1	,	25	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedDog	),	170	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedEagle	),	402	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedElderBlackBearRiding	),	4230	,	1	,	50	,	false	,	false	,	World.Dread	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedElderBrownBearRiding	),	4230	,	1	,	50	,	false	,	false	,	World.Dread	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedElderPolarBearRiding	),	4230	,	1	,	50	,	false	,	false	,	World.Dread	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedElephant	),	4520	,	1	,	75	,	false	,	false	,	World.Sosaria	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedFerret	),	106	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedForestOstard	),	700	,	5	,	25	,	false	,	false	,	World.Lodor	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedFox	),	740	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedFrog	),	622	,	1	,	25	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedGiantHawk	),	2520	,	1	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedGiantLizard	),	600	,	1	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedGiantRat	),	312	,	1	,	10	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedGiantRaven	),	2520	,	1	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedGiantSerpent	),	3720	,	1	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedGiantSnake	),	820	,	1	,	50	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedGiantToad	),	734	,	1	,	50	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedGoat	),	380	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cattle	),
			new ItemSalesInfo( typeof(	CagedGorilla	),	1060	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedGreatBear	),	2140	,	1	,	50	,	false	,	false	,	World.Dread	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedGreyWolf	),	1120	,	1	,	25	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedGriffonRiding	),	28320	,	1	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedGrizzlyBearRiding	),	1767	,	1	,	50	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedHawk	),	402	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedHippogriffRiding	),	28320	,	1	,	75	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedHorse	),	550	,	5	,	25	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedHugeLizard	),	2520	,	1	,	50	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedJackal	),	1120	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedJaguar	),	2240	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedKodiakBear	),	2140	,	1	,	50	,	false	,	false	,	World.Lodor	,	Category.None	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedLionRiding	),	2240	,	1	,	60	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedManticoreRiding	),	28320	,	1	,	80	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedMastadon	),	4520	,	1	,	75	,	false	,	false	,	World.Savage	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedMouse	),	107	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedPackBear	),	12500	,	5	,	25	,	false	,	false	,	World.Dread	,	Category.Pack	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedPackHorse	),	631	,	5	,	25	,	false	,	false	,	World.None	,	Category.Pack	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedPackLlama	),	565	,	5	,	25	,	false	,	false	,	World.Sosaria	,	Category.Pack	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedPackMule	),	10000	,	5	,	50	,	false	,	false	,	World.None	,	Category.Pack	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedPackNecroHound	),	10000	,	5	,	50	,	false	,	false	,	World.Necro	,	Category.Pack	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedPackNecroSpider	),	631	,	5	,	25	,	false	,	false	,	World.Necro	,	Category.Pack	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedPackStegosaurus	),	15500	,	5	,	50	,	false	,	false	,	World.Savage	,	Category.Pack	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedPackTurtle	),	14500	,	5	,	50	,	false	,	false	,	World.Savage	,	Category.Pack	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedPandaRiding	),	1767	,	1	,	25	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedPanther	),	1271	,	1	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedPig	),	400	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cattle	),
			new ItemSalesInfo( typeof(	CagedPolarBear	),	2140	,	1	,	25	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedRabbit	),	106	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedRaptorRiding	),	3000	,	1	,	25	,	false	,	false	,	World.Savage	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedRat	),	107	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedRidableLlama	),	490	,	5	,	25	,	false	,	false	,	World.Sosaria	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedRidgeback	),	1500	,	5	,	10	,	false	,	false	,	World.Savage	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedSheep	),	380	,	5	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cattle	),
			new ItemSalesInfo( typeof(	CagedSnowOstard	),	700	,	5	,	25	,	false	,	false	,	World.Lodor	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedSwampDragon	),	1700	,	5	,	10	,	false	,	false	,	World.Serpent	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedTigerRiding	),	2240	,	1	,	50	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	CagedTimberWolf	),	768	,	1	,	10	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedTurkey	),	150	,	3	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Cattle	),
			new ItemSalesInfo( typeof(	CagedWhiteWolf	),	2400	,	1	,	10	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedWolfDire	),	2400	,	1	,	25	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Animals	),
			new ItemSalesInfo( typeof(	CagedZebraRiding	),	650	,	1	,	25	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Stable	),
			new ItemSalesInfo( typeof(	Citrine	),	60	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	Diamond	),	150	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	Emerald	),	100	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	Oyster	),	500	,	0	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	Ruby	),	70	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	Sapphire	),	110	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	StarSapphire	),	120	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	),
			new ItemSalesInfo( typeof(	Tourmaline	),	80	,	15	,	0	,	false	,	false	,	World.None	,	Category.None	,	Material.None	,	Market.Jeweler	)
		};
	}
}

namespace Server.Commands
{
	public class TestStock
	{
		private static Mobile m_Mobile;

		public static void Initialize()
		{
			CommandSystem.Register( "TestStock", AccessLevel.Administrator, new CommandEventHandler( TestStock_OnCommand ) );
		}

		public static void UpdateFile(string filename, string header)
		{
			string tempfile = Path.GetTempFileName();
			StreamWriter writer = null;
			StreamReader reader = null;
			using (writer = new StreamWriter(tempfile))
			using (reader = new StreamReader(filename))
			{
				writer.WriteLine(header);
				while (!reader.EndOfStream)
				{
					writer.WriteLine(reader.ReadLine());
				}
			}

			if (writer != null)
				writer.Dispose();

			if (reader != null)
				reader.Dispose();

			File.Copy(tempfile, filename, true);
			File.Delete(tempfile);
		}

		[Usage( "TestStock" )]
		[Description( "Add all of the items available in the store listing." )]
		public static void TestStock_OnCommand( CommandEventArgs e )
		{
			m_Mobile = e.Mobile;

			ItemSalesInfo[] list = ItemSalesInfo.m_SellingInfo;

			int entries = list.Length;
			int val = 0;
			Item oItem = null;
			string sPath = "Data/stock.txt";

			/// CREATE THE FILE IF IT DOES NOT EXIST ///
			StreamWriter w = null; 
			try
			{
				using (w = File.AppendText( sPath ) ){}
			}
			catch(Exception)
			{
			}
			finally
			{
				if (w != null)
					w.Dispose();
			}

			while ( entries > 0 )
			{
				Type itemType = list[val].ItemsType;

				if ( itemType != null )
				{
					UpdateFile(sPath, "" + itemType + "");
					oItem = (Item)Activator.CreateInstance( itemType );
					int qty = ItemInformation.GetQty( val, true );
					int price = ItemInformation.GetSellPrice( val, true );
					UpdateFile(sPath, "" + oItem.Name + "");
					oItem.Delete();
				}
				entries--;
				val++;
			}
			m_Mobile.SendMessage( "Finished test. See stock.txt for details." );
		}
	}
}