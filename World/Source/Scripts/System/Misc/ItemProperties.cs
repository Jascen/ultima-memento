using System;
using Server.Items;
using Server.Misc;
using System.Globalization;

namespace Server
{
    class ItemProps
    {
		public static string formatString( string Txt, string Hue )
		{
			if ( Hue == null )
				Hue = "FFFFFF";

			return "<BASEFONT COLOR=#" + Hue + ">" + Txt + "</BASEFONT><BR>";
		}

		public static string densityText( Density density )
		{
			if ( density == Density.Weak )
				return "Density: Weak";
			else if ( density == Density.Regular )
				return "Density: Regular";
			else if ( density == Density.Great )
				return "Density: Great";
			else if ( density == Density.Greater )
				return "Density: Greater";
			else if ( density == Density.Superior )
				return "Density: Superior";
			else if ( density == Density.Ultimate )
				return "Density: Ultimate";

			return null;
		}

		public static string ItemProperties( Item item, bool fromToolMenu )
		{
			string text = null;

			if ( item.ColorText1 != null )
				text += formatString( item.ColorText1, item.ColorHue1 );

			if ( item.ColorText2 != null )
				text += formatString( item.ColorText2, item.ColorHue2 );

			if ( item.ColorText1 == null )
				text += "<BASEFONT COLOR=#FFF000>" + GetItemName( item ) + "</BASEFONT><BR>";

			if ( item is BaseTrinket && item.Catalog == Catalogs.Jewelry && ((BaseTrinket)item).GemType != GemType.None )
				text += GetStoneSetting( (BaseTrinket)item );

			if ( item.ColorText3 != null )
				text += formatString( item.ColorText3, item.ColorHue3 );

			if ( item.ColorText4 != null )
				text += formatString( item.ColorText4, item.ColorHue4 );

			if ( item.ColorText5 != null )
				text += formatString( item.ColorText5, item.ColorHue5 );

			switch (item.ArtifactLevel)
			{
				case ArtifactLevel.DecorativeArtefact:
					text += "<BASEFONT COLOR=#C6D11C>Decorative Artefact</BASEFONT><BR>";
					break;
				case ArtifactLevel.LegendaryArtefact:
					text += "<BASEFONT COLOR=#C6D11C>Legendary Artefact</BASEFONT><BR>";
					break;
				case ArtifactLevel.StandardArtefact:
					text += "<BASEFONT COLOR=#C6D11C>Artefact</BASEFONT><BR>";
					break;
				case ArtifactLevel.Artifact:
					text += "<BASEFONT COLOR=#C6D11C>Artifact</BASEFONT><BR>";
					break;
			}

			if ( item.Enchanted != MagicSpell.None )
			{
				text += "<BASEFONT COLOR=#4AADFE>" + SpellItems.GetNameUpper( item.Enchanted ) + "</BASEFONT><BR>";

				if ( item.EnchantUsesMax > 0 )
					text += "<BASEFONT COLOR=#4AADFE>Charges: " + item.EnchantUses + "/" + item.EnchantUsesMax + "</BASEFONT><BR>";
				else
					text += "<BASEFONT COLOR=#4AADFE>Charges: " + item.EnchantUses + "</BASEFONT><BR>";
			}

			AddArtyPoints( item, text );
	 
			if (item.IsSecure)
				text += "Locked Down & Secure<BR>";
			else if (item.IsLockedDown)
				text += "Locked Down<BR>";

			if ( item.InfoText1 != null )
				text += "" + item.InfoText1 + "<BR>";

			if ( item.InfoText2 != null )
				text += "" + item.InfoText2 + "<BR>";

			if ( item.InfoText3 != null )
				text += "" + item.InfoText3 + "<BR>";

			if ( item.InfoText4 != null )
				text += "" + item.InfoText4 + "<BR>";

			if ( item.InfoText5 != null )
				text += "" + item.InfoText5 + "<BR>";

			if (item.DisplayLootType)
			{
				if (item.LootType == LootType.Blessed)
					text += "Blessed<BR>";
				else if (item.LootType == LootType.Cursed)
					text += "Cursed<BR>";
				else if (item.Insured)
					text += "Insured<BR>";
			}

			if (item.DisplayWeight)
			{
				int weight = item.PileWeight + item.TotalWeight;

				if (weight > 0 && weight < 2)
					text += "Weight: 1 Stone<BR>";
				else if (weight > 0)
					text += "Weight: " + weight.ToString() + " Stones<BR>";
			}

			if (item.QuestItem)
				text += "Quest Item<BR>";

			if ( item is Runebook )
				text = AddRunebookInfo( (Runebook)item, text, fromToolMenu );

			if ( item is BaseArmor )
				text = AddArmorInfo( (BaseArmor)item, text, fromToolMenu );

			if ( item is BaseWeapon )
				text = AddWeaponInfo( (BaseWeapon)item, text, fromToolMenu );

			if ( item is BaseClothing )
				text = AddClothingInfo( (BaseClothing)item, text, fromToolMenu );

			if ( item is BaseInstrument )
				text = AddInstrumentInfo( (BaseInstrument)item, text, fromToolMenu );

			if ( item is Spellbook )
				text = AddSpellbookInfo( (Spellbook)item, text, fromToolMenu );

			if ( item is BaseQuiver )
				text = AddQuiverInfo( (BaseQuiver)item, text, fromToolMenu );

			if ( item is BaseTrinket )
				text = AddMagicInfo( (BaseTrinket)item, text, fromToolMenu );

			if ( item is BaseTool )
				text = AddToolInfo( (BaseTool)item, text, fromToolMenu );

			if ( item is BaseHarvestTool )
				text = AddHarvestToolInfo( (BaseHarvestTool)item, text, fromToolMenu );

			if ( item.LimitsMax > 0 )
				text += "" + item.Limits + " " + item.LimitsName + " Remaining<BR>";

			if ( item.InfoData != null )
				text += "<BR>" + item.InfoData + "<BR>";

			return text;
		}

		public static string GetItemName( Item item )
		{
			string text = "";
			string material = GetMaterial( item.Resource );

			if ( item is BaseArmor || item is BaseWeapon || item is BaseClothing || item is BaseInstrument || ( item is BaseTrinket && item.Catalog == Catalogs.Jewelry ) )
			{
				if ( material != null )
					text += "" + Capitalize( material ) + " " + Capitalize( item.Name ) + "";
				else
					text += "" + Capitalize( item.Name ) + "";
			}
			else
				text = item.Name;

			return Capitalize( text );
		}

		public static string GetStoneSetting( BaseTrinket item )
		{
			if ( item.GemType == GemType.StarSapphire )
				return "<BASEFONT COLOR=#454FF5>Star Sapphire Setting</BASEFONT><BR>";
			else if ( item.GemType == GemType.Emerald )
				return "<BASEFONT COLOR=#46E657>Emerald Setting</BASEFONT><BR>";
			else if ( item.GemType == GemType.Sapphire )
				return "<BASEFONT COLOR=#6FC7EE>Sapphire Setting</BASEFONT><BR>";
			else if ( item.GemType == GemType.Ruby )
				return "<BASEFONT COLOR=#E83B3B>Ruby Setting</BASEFONT><BR>";
			else if ( item.GemType == GemType.Citrine )
				return "<BASEFONT COLOR=#E8F567>Citrine Setting</BASEFONT><BR>";
			else if ( item.GemType == GemType.Amethyst )
				return "<BASEFONT COLOR=#EB67F5>Amethyst Setting</BASEFONT><BR>";
			else if ( item.GemType == GemType.Tourmaline )
				return "<BASEFONT COLOR=#D4C346>Tourmaline Setting</BASEFONT><BR>";
			else if ( item.GemType == GemType.Amber )
				return "<BASEFONT COLOR=#DF8929>Amber Setting</BASEFONT><BR>";
			else if ( item.GemType == GemType.Diamond )
				return "<BASEFONT COLOR=#C2F5FF>Diamond Setting</BASEFONT><BR>";
			else if ( item.GemType == GemType.Pearl )
				return "<BASEFONT COLOR=#D4DFE1>Pearl Setting</BASEFONT><BR>";

			return null;
		}

        public static string AddRunebookInfo ( Runebook var, string text, bool fromToolMenu )
        {
			if ( var.Description != null && var.Description.Length > 0 )
				text += "<BASEFONT COLOR=#5FAFE3>" + var.Description + "</BASEFONT><BR>";

			text += "<BASEFONT COLOR=#EDC73A>" + var.CurCharges + " of " + var.MaxCharges + " Charges</BASEFONT><BR>";

			return text;
        }

        public static string AddToolInfo ( BaseTool var, string text, bool fromToolMenu )
        {
			if ( var.Quality == ToolQuality.Exceptional )
				text += "Exceptional<BR>";

			text = ItemSkills.BaseToolSkills( var, text );

			text += "Equipment: " + var.EquipLayerName( var.Layer ) + "<BR>";

			if ( var is IUsesRemaining && ((IUsesRemaining)var).ShowUsesRemaining )
				text += "Uses Remaining: " + ((IUsesRemaining)var).UsesRemaining.ToString() + "<BR>";

			return text;
        }

        public static string AddHarvestToolInfo ( BaseHarvestTool var, string text, bool fromToolMenu )
        {
			if ( var.Quality == ToolQuality.Exceptional )
				text += "Exceptional<BR>";

			text = ItemSkills.BaseHarvestToolSkills( var, text );

			text += "Equipment: " + var.EquipLayerName( var.Layer ) + "<BR>";

			if ( var is IUsesRemaining && ((IUsesRemaining)var).ShowUsesRemaining )
				text += "Uses Remaining: " + ((IUsesRemaining)var).UsesRemaining.ToString() + "<BR>";

			return text;
        }

        public static string AddArmorInfo ( BaseArmor var, string text, bool fromToolMenu )
        {
            if (var.BuiltBy != null)
				text += "Crafted by " + var.BuiltBy.Name + "<BR>";

			if ( var.Quality == ArmorQuality.Exceptional )
				text += "Exceptional<BR>";

			text = ItemSkills.BaseArmorSkills( var, text );

			int prop;

			if ( (prop = var.ArtifactRarity) > 0 )
				text += "Artifact Rarity: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.WeaponDamage) != 0 )
				text += "Damage Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.DefendChance) != 0 )
				text += "Defense Change Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusDex) != 0 )
				text += "Dexterity Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.EnhancePotions) != 0 )
				text += "Enhance Potions: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.CastRecovery) != 0 )
				text += "Faster Cast Recovery: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.CastSpeed) != 0 )
				text += "Faster Casting: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.AttackChance) != 0 )
				text += "Hit Chance Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusHits) != 0 )
				text += "Hit Point Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusInt) != 0 )
				text += "Intelligence Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.LowerManaCost) != 0 && MyServerSettings.LowerMana() > 0 )
			{
				if ( prop > MyServerSettings.LowerMana() ){ prop = MyServerSettings.LowerMana(); }
				text += "Lower Mana Cost: " + prop.ToString() + "<BR>";
			}

			if ( (prop = var.Attributes.LowerRegCost) != 0 && MyServerSettings.LowerReg() > 0 )
			{
				if ( prop > MyServerSettings.LowerReg() ){ prop = MyServerSettings.LowerReg(); }
				text += "Lower Reagent Cost: " + prop.ToString() + "%<BR>";
			}

			if ( (prop = var.GetLowerStatReq()) != 0 )
				text += "Lower Requirements: " + prop.ToString() + "%<BR>";

			if ( (prop = (var.GetLuckBonus() + var.Attributes.Luck)) != 0 )
				text += "Luck: " + prop.ToString() + "<BR>";

			if ( (prop = var.ArmorAttributes.MageArmor) != 0 )
				text += "Mage Armor<BR>";

			if ( (prop = var.Attributes.BonusMana) != 0 )
				text += "Mana Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.RegenMana) != 0 )
				text += "Mana Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.NightSight) != 0 )
				text += "Night Sight<BR>";

			if ( (prop = var.Attributes.ReflectPhysical) != 0 )
				text += "Reflect Physical Damage: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.RegenStam) != 0 )
				text += "Stamina Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.RegenHits) != 0 )
				text += "Hit Point Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.ArmorAttributes.SelfRepair) != 0 )
				text += "Self Repair: " + (prop*10).ToString() + "%<BR>";

			if ( (prop = var.Attributes.SpellChanneling) != 0 )
				text += "Spell Channeling<BR>";

			if ( (prop = var.Attributes.SpellDamage) != 0 )
				text += "Spell Damage Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusStam) != 0 )
				text += "Stamina Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusStr) != 0 )
				text += "Strength Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.WeaponSpeed) != 0 )
				text += "Swing Speed Increase: " + prop.ToString() + "%<BR>";

			text = AddResistInfo ( var, text );

			if ( (prop = var.GetDurabilityBonus()) > 0 )
				text += "Durability Bonus: " + prop.ToString() + "%<BR>";

			if ( (prop = var.ComputeStatReq( StatType.Str )) > 0 )
				text += "Strength Requirement: " + prop.ToString() + "<BR>";

			if ( (prop = var.ComputeStatReq( StatType.Int )) > 0 )
				text += "Intelligence Requirement: " + prop.ToString() + "<BR>";

			if ( (prop = var.ComputeStatReq( StatType.Dex )) > 0 )
				text += "Dexterity Requirement: " + prop.ToString() + "<BR>";

			text += "Equipment: " + var.EquipLayerName( var.Layer ) + "<BR>";

			if ( densityText( var.Density ) != null )
				text += "" + densityText( var.Density ) + "<BR>";

			if ( var.HitPoints >= 0 && var.MaxHitPoints > 0 )
			{
				if ( fromToolMenu )
					text += "Durability About: " + var.MaxHitPoints + "<BR>";
				else
					text += "Durability: " + var.HitPoints + "/" + var.MaxHitPoints + "<BR>";
			}

			return text;
        }

		public static string AddWeaponInfo ( BaseWeapon var, string text, bool fromToolMenu )
		{
			int prop;
			TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            if (var.BuiltBy != null)
				text += "Crafted by " + var.BuiltBy.Name + "<BR>";

			if ( var.Quality == WeaponQuality.Exceptional )
				text += "Exceptional<BR>";

			if ( !String.IsNullOrEmpty( var.EngravedText ) )
				text += "" + var.EngravedText + "<BR>";

			text = ItemSkills.BaseWeaponSkills( var, text );

			if ( (prop = var.ArtifactRarity) > 0 )
				text += "Artifact Rarity: " + prop.ToString() + "<BR>";

			if ( var is IUsesRemaining && ((IUsesRemaining)var).ShowUsesRemaining )
				text += "Uses Remaining: " + ((IUsesRemaining)var).UsesRemaining.ToString() + "<BR>";

			if ( var.Poison != null && var.PoisonCharges > 0 )
			{
				if ( var.Poison == Poison.Lesser )
					text += "Lesser Poison Uses: " + var.PoisonCharges.ToString() + "<BR>";
				else if ( var.Poison == Poison.Regular )
					text += "Poison Uses: " + var.PoisonCharges.ToString() + "<BR>";
				else if ( var.Poison == Poison.Greater )
					text += "Greater Poison Uses: " + var.PoisonCharges.ToString() + "<BR>";
				else if ( var.Poison == Poison.Deadly )
					text += "Deadly Poison Uses: " + var.PoisonCharges.ToString() + "<BR>";
				else if ( var.Poison == Poison.Lethal )
					text += "Lethal Poison Uses: " + var.PoisonCharges.ToString() + "<BR>";
			}

			if( var.Slayer != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( var.Slayer );
				if( entry != null )
					text += "" + cultInfo.ToTitleCase(CliLocTable.Lookup( entry.Title )) + "<BR>";
			}

			if( var.Slayer2 != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( var.Slayer2 );
				if( entry != null )
					text += "" + cultInfo.ToTitleCase(CliLocTable.Lookup( entry.Title )) + "<BR>";
			}

			text = AddResistInfo ( var, text );

			if ( Core.ML && var is BaseRanged && ( (BaseRanged) var ).Balanced )
				text += "Balanced<BR>";

			if ( (prop = var.WeaponAttributes.UseBestSkill) != 0 )
				text += "Use Best Weapon Skill<BR>";

			if ( (prop = (var.GetDamageBonus() + var.Attributes.WeaponDamage)) != 0 )
				text += "Damage Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.DefendChance) != 0 )
				text += "Defense Chance Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.EnhancePotions) != 0 )
				text += "Enhance Potions: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.CastRecovery) != 0 )
				text += "Faster Cast Recovery: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.CastSpeed) != 0 )
				text += "Faster Casting: " + prop.ToString() + "<BR>";

			if ( (prop = (var.GetHitChanceBonus() + var.Attributes.AttackChance)) != 0 )
				text += "Hit Chance Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.WeaponAttributes.HitColdArea) != 0 )
				text += "Hit Cold Area: " + prop.ToString() + "%<BR>";

			if ( (prop = var.WeaponAttributes.HitDispel) != 0 )
				text += "Hit Dispel: " + prop.ToString() + "%<BR>";

			if ( (prop = var.WeaponAttributes.HitEnergyArea) != 0 )
				text += "Hit Energy Area: " + prop.ToString() + "%<BR>";

			if ( (prop = var.WeaponAttributes.HitFireArea) != 0 )
				text += "Hit Fire Area: " + prop.ToString() + "%<BR>";

			if ( (prop = var.WeaponAttributes.HitFireball) != 0 )
				text += "Hit Fireball: " + prop.ToString() + "%<BR>";

			if ( (prop = var.WeaponAttributes.HitHarm) != 0 )
				text += "Hit Harm: " + prop.ToString() + "%<BR>";

			if ( (prop = var.WeaponAttributes.HitLeechHits) != 0 )
				text += "Hit Life Leech: " + prop.ToString() + "%<BR>";

			if ( (prop = var.WeaponAttributes.HitLightning) != 0 )
				text += "Hit Lightning: " + prop.ToString() + "%<BR>";

			if ( (prop = var.WeaponAttributes.HitLowerAttack) != 0 )
				text += "Hit Lower Attack: " + prop.ToString() + "%<BR>";

			if ( (prop = var.WeaponAttributes.HitLowerDefend) != 0 )
				text += "Hit Lower Defense: " + prop.ToString() + "%<BR>";

			if ( (prop = var.WeaponAttributes.HitMagicArrow) != 0 )
				text += "Hit Magic Arrow: " + prop.ToString() + "%<BR>";

			if ( (prop = var.WeaponAttributes.HitLeechMana) != 0 )
				text += "Hit Mana Leech: " + prop.ToString() + "%<BR>";

			if ( (prop = var.WeaponAttributes.HitPhysicalArea) != 0 )
				text += "Hit Physical Area: " + prop.ToString() + "%<BR>";

			if ( (prop = var.WeaponAttributes.HitPoisonArea) != 0 )
				text += "Hit Poison Area: " + prop.ToString() + "%<BR>";

			if ( (prop = var.WeaponAttributes.HitLeechStam) != 0 )
				text += "Hit Stamina Leech: " + prop.ToString() + "%<BR>";

			if ( Core.ML && var is BaseRanged && ( prop = ( (BaseRanged) var ).Velocity ) != 0 )
				text += "Velocity: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusDex) != 0 )
				text += "Dexterity Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusHits) != 0 )
				text += "Hit Point Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusInt) != 0 )
				text += "Intelligence Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.LowerManaCost) != 0 && MyServerSettings.LowerMana() > 0 )
			{
				if ( prop > MyServerSettings.LowerMana() ){ prop = MyServerSettings.LowerMana(); }
				text += "Lower Mana Cost: " + prop.ToString() + "<BR>";
			}

			if ( (prop = var.Attributes.LowerRegCost) != 0 && MyServerSettings.LowerReg() > 0 )
			{
				if ( prop > MyServerSettings.LowerReg() ){ prop = MyServerSettings.LowerReg(); }
				text += "Lower Reagent Cost: " + prop.ToString() + "%<BR>";
			}

			if ( (prop = var.GetLowerStatReq()) != 0 )
				text += "Lower Requirements: " + prop.ToString() + "%<BR>";

			if ( (prop = (var.GetLuckBonus() + var.Attributes.Luck)) != 0 )
				text += "Luck: " + prop.ToString() + "<BR>";

			if ( (prop = var.WeaponAttributes.MageWeapon) != 0 )
				text += "Mage Weapon: " + (30 - prop).ToString() + " Skill<BR>";

			if ( (prop = var.Attributes.BonusMana) != 0 )
				text += "Mana Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.RegenMana) != 0 )
				text += "Mana Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.NightSight) != 0 && !(var is LightSword) && !(var is DoubleLaserSword) && !(var is LevelLaserSword) && !(var is LevelDoubleLaserSword) )
				text += "Night Sight<BR>";

			if ( (prop = var.Attributes.ReflectPhysical) != 0 )
				text += "Reflect Physical Damage: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.RegenStam) != 0 )
				text += "Stamina Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.RegenHits) != 0 )
				text += "Hit Point Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.WeaponAttributes.SelfRepair) != 0 )
				text += "Self Repair: " + (prop*10).ToString() + "%<BR>";

			if ( (prop = var.Attributes.SpellChanneling) != 0 )
				text += "Spell Channeling<BR>";

			if ( (prop = var.Attributes.SpellDamage) != 0 )
				text += "Spell Damage Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusStam) != 0 )
				text += "Stamina Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusStr) != 0 )
				text += "Strength Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.WeaponSpeed) != 0 )
				text += "Swing Speed Increase: " + prop.ToString() + "%<BR>";

			int phys, fire, cold, pois, nrgy, chaos, direct;

			var.GetDamageTypes( null, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct );

			if ( phys != 0 )
				text += "Physical Damage: " + phys.ToString() + "%<BR>";

			if ( fire != 0 )
				text += "Fire Damage: " + fire.ToString() + "%<BR>";

			if ( cold != 0 )
				text += "Cold Damage: " + cold.ToString() + "%<BR>";

			if ( pois != 0 )
				text += "Poison Damage: " + pois.ToString() + "%<BR>";

			if ( nrgy != 0 )
				text += "Energy Damage: " + nrgy.ToString() + "%<BR>";

			if ( Core.ML && chaos != 0 )
				text += "Chaos Damage: " + chaos.ToString() + "%<BR>";

			if ( Core.ML && direct != 0 )
				text += "Direct Damage: " + direct.ToString() + "%<BR>";

			text += "Weapon Damage " + var.MinDamage.ToString() + " - " + var.MaxDamage.ToString() + "<BR>";

			if ( Core.ML )
				text += "Weapon Speed: " + String.Format( "{0}s", var.Speed ) + "<BR>";
			else
				text += "Weapon Speed: " + var.Speed.ToString() + "<BR>";

			if ( var.MaxRange > 1 )
				text += "Range: " + var.MaxRange.ToString() + "<BR>";

			int strReq = AOS.Scale( var.StrRequirement, 100 - var.GetLowerStatReq() );

			if ( strReq > 0 )
				text += "Strength Requirement: " + strReq.ToString() + "<BR>";

			text += "Equipment: " + var.EquipLayerName( var.Layer ) + "<BR>";

			if ( densityText( var.Density ) != null )
				text += "" + densityText( var.Density ) + "<BR>";

			if ( Core.SE || var.WeaponAttributes.UseBestSkill == 0 )
			{
				switch ( var.Skill )
				{
					case SkillName.Swords:  		text += "Skill: Swordsmanship<BR>"; break;
					case SkillName.Bludgeoning:  	text += "Skill: Bludgeoning<BR>"; break;
					case SkillName.Fencing: 		text += "Skill: Fencing<BR>"; break;
					case SkillName.Marksmanship: 	text += "Skill: Marksmanship<BR>"; break;
					case SkillName.FistFighting: 	text += "Skill: Fist Fighting<BR>"; break;
				}
			}

			if ( var.HitPoints >= 0 && var.MaxHitPoints > 0 )
			{
				if ( fromToolMenu )
					text += "Durability About: " + var.MaxHitPoints + "<BR>";
				else
					text += "Durability: " + var.HitPoints + "/" + var.MaxHitPoints + "<BR>";
			}

			return text;
        }

        public static string AddClothingInfo ( BaseClothing var, string text, bool fromToolMenu )
        {
			int prop;

            if (var.BuiltBy != null)
				text += "Crafted by " + var.BuiltBy.Name + "<BR>";

			if ( var.Quality == ClothingQuality.Exceptional )
				text += "Exceptional<BR>";

			text = ItemSkills.BaseClothingSkills( var, text );

			if ( (prop = var.ArtifactRarity) > 0 )
				text += "Artifact Rarity: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.WeaponDamage) != 0 )
				text += "Damage Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.DefendChance) != 0 )
				text += "Defense Change Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusDex) != 0 )
				text += "Dexterity Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.EnhancePotions) != 0 )
				text += "Enhance Potions: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.CastRecovery) != 0 )
				text += "Faster Cast Recovery: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.CastSpeed) != 0 )
				text += "Faster Casting: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.AttackChance) != 0 )
				text += "Hit Chance Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusHits) != 0 )
				text += "Hit Point Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusInt) != 0 )
				text += "Intelligence Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.LowerManaCost) != 0 && MyServerSettings.LowerMana() > 0 )
			{
				if ( prop > MyServerSettings.LowerMana() ){ prop = MyServerSettings.LowerMana(); }
				text += "Lower Mana Cost: " + prop.ToString() + "<BR>";
			}

			if ( (prop = var.Attributes.LowerRegCost) != 0 && MyServerSettings.LowerReg() > 0 )
			{
				if ( prop > MyServerSettings.LowerReg() ){ prop = MyServerSettings.LowerReg(); }
				text += "Lower Reagent Cost: " + prop.ToString() + "%<BR>";
			}

			if ( (prop = var.GetLowerStatReq()) != 0 )
				text += "Lower Requirements: " + prop.ToString() + "%<BR>";

			if ( (prop = (var.GetLuckBonus() + var.Attributes.Luck)) != 0 )
				text += "Luck: " + prop.ToString() + "<BR>";

			if ( (prop = var.ClothingAttributes.MageArmor) != 0 )
				text += "Mage Armor<BR>";

			if ( (prop = var.Attributes.BonusMana) != 0 )
				text += "Mana Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.RegenMana) != 0 )
				text += "Mana Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.NightSight) != 0 )
				text += "Night Sight<BR>";

			if ( (prop = var.Attributes.ReflectPhysical) != 0 )
				text += "Reflect Physical Damage: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.RegenStam) != 0 )
				text += "Stamina Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.RegenHits) != 0 )
				text += "Hit Point Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.ClothingAttributes.SelfRepair) != 0 )
				text += "Self Repair: " + (prop*10).ToString() + "%<BR>";

			if ( (prop = var.Attributes.SpellChanneling) != 0 )
				text += "Spell Channeling<BR>";

			if ( (prop = var.Attributes.SpellDamage) != 0 )
				text += "Spell Damage Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusStam) != 0 )
				text += "Stamina Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusStr) != 0 )
				text += "Strength Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.WeaponSpeed) != 0 )
				text += "Swing Speed Increase: " + prop.ToString() + "%<BR>";

			text = AddResistInfo ( var, text );

			text += "Equipment: " + var.EquipLayerName( var.Layer ) + "<BR>";

			if ( densityText( var.Density ) != null )
				text += "" + densityText( var.Density ) + "<BR>";

			if ( (prop = var.ClothingAttributes.DurabilityBonus) > 0 )
				text += "Durability Bonus: " + prop.ToString() + "%<BR>";

			if ( (prop = var.ComputeStatReq( StatType.Str )) > 0 )
				text += "Strength Requirement: " + prop.ToString() + "<BR>";

			if ( var.HitPoints >= 0 && var.MaxHitPoints > 0 )
			{
				if ( fromToolMenu )
					text += "Durability About: " + var.MaxHitPoints + "<BR>";
				else
					text += "Durability: " + var.HitPoints + "/" + var.MaxHitPoints + "<BR>";
			}

			return text;
        }

        public static string AddInstrumentInfo ( BaseInstrument var, string text, bool fromToolMenu )
        {
			int prop;
			TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            if (var.BuiltBy != null)
				text += "Crafted by " + var.BuiltBy.Name + "<BR>";

            if (var.UsesRemaining > 0)
				text += "Uses Remaining: " + var.UsesRemaining.ToString() + "<BR>";

			if ( var.Quality == InstrumentQuality.Exceptional )
				text += "Exceptional<BR>";

			text = ItemSkills.BaseInstrumentSkills( var, text );

			if ( (prop = var.ArtifactRarity) > 0 )
				text += "Artifact Rarity: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.WeaponDamage) != 0 )
				text += "Damage Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.DefendChance) != 0 )
				text += "Defense Change Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusDex) != 0 )
				text += "Dexterity Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.EnhancePotions) != 0 )
				text += "Enhance Potions: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.CastRecovery) != 0 )
				text += "Faster Cast Recovery: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.CastSpeed) != 0 )
				text += "Faster Casting: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.AttackChance) != 0 )
				text += "Hit Chance Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusHits) != 0 )
				text += "Hit Point Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusInt) != 0 )
				text += "Intelligence Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.LowerManaCost) != 0 && MyServerSettings.LowerMana() > 0 )
			{
				if ( prop > MyServerSettings.LowerMana() ){ prop = MyServerSettings.LowerMana(); }
				text += "Lower Mana Cost: " + prop.ToString() + "<BR>";
			}

			if ( (prop = var.Attributes.LowerRegCost) != 0 && MyServerSettings.LowerReg() > 0 )
			{
				if ( prop > MyServerSettings.LowerReg() ){ prop = MyServerSettings.LowerReg(); }
				text += "Lower Reagent Cost: " + prop.ToString() + "%<BR>";
			}

			if ( (prop = (var.GetLuckBonus() + var.Attributes.Luck)) != 0 )
				text += "Luck: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusMana) != 0 )
				text += "Mana Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.RegenMana) != 0 )
				text += "Mana Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.NightSight) != 0 )
				text += "Night Sight<BR>";

			if ( (prop = var.Attributes.ReflectPhysical) != 0 )
				text += "Reflect Physical Damage: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.RegenStam) != 0 )
				text += "Stamina Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.RegenHits) != 0 )
				text += "Hit Point Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.SpellChanneling) != 0 )
				text += "Spell Channeling<BR>";

			if ( (prop = var.Attributes.SpellDamage) != 0 )
				text += "Spell Damage Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusStam) != 0 )
				text += "Stamina Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusStr) != 0 )
				text += "Strength Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.WeaponSpeed) != 0 )
				text += "Swing Speed Increase: " + prop.ToString() + "%<BR>";

			text = AddResistInfo ( var, text );

			if ( var.HitPoints >= 0 && var.MaxHitPoints > 0 )
			{
				if ( fromToolMenu )
					text += "Durability About: " + var.MaxHitPoints + "<BR>";
				else
					text += "Durability: " + var.HitPoints + "/" + var.MaxHitPoints + "<BR>";
			}

			if( var.Slayer != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( var.Slayer );
				if( entry != null )
					text += "" + cultInfo.ToTitleCase(CliLocTable.Lookup( entry.Title )) + "<BR>";
			}

			if( var.Slayer2 != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( var.Slayer2 );
				if( entry != null )
					text += "" + cultInfo.ToTitleCase(CliLocTable.Lookup( entry.Title )) + "<BR>";
			}

			text += "Equipment: " + var.EquipLayerName( var.Layer ) + "<BR>";

			return text;
        }

        public static string AddMagicInfo ( BaseTrinket var, string text, bool fromToolMenu )
        {
            if (var.BuiltBy != null)
				text += "Crafted by " + var.BuiltBy.Name + "<BR>";

			int prop;

			text = ItemSkills.BaseTrinketSkills( var, text );

			if ( (prop = var.ArtifactRarity) > 0 )
				text += "Artifact Rarity: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.WeaponDamage) != 0 )
				text += "Damage Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.DefendChance) != 0 )
				text += "Defense Change Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusDex) != 0 )
				text += "Dexterity Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.EnhancePotions) != 0 )
				text += "Enhance Potions: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.CastRecovery) != 0 )
				text += "Faster Cast Recovery: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.CastSpeed) != 0 )
				text += "Faster Casting: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.AttackChance) != 0 )
				text += "Hit Chance Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusHits) != 0 )
				text += "Hit Point Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusInt) != 0 )
				text += "Intelligence Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.LowerManaCost) != 0 && MyServerSettings.LowerMana() > 0 )
			{
				if ( prop > MyServerSettings.LowerMana() ){ prop = MyServerSettings.LowerMana(); }
				text += "Lower Mana Cost: " + prop.ToString() + "<BR>";
			}

			if ( (prop = var.Attributes.LowerRegCost) != 0 && MyServerSettings.LowerReg() > 0 )
			{
				if ( prop > MyServerSettings.LowerReg() ){ prop = MyServerSettings.LowerReg(); }
				text += "Lower Reagent Cost: " + prop.ToString() + "%<BR>";
			}

			if ( (prop = (var.GetLuckBonus() + var.Attributes.Luck)) != 0 )
				text += "Luck: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusMana) != 0 )
				text += "Mana Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.RegenMana) != 0 )
				text += "Mana Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.NightSight) != 0 && !(var is TrinketCandle) && !(var is TrinketLantern) && !(var is TrinketTorch) )
				text += "Night Sight<BR>";

			if ( (prop = var.Attributes.ReflectPhysical) != 0 )
				text += "Reflect Physical Damage: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.RegenStam) != 0 )
				text += "Stamina Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.RegenHits) != 0 )
				text += "Hit Point Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.SpellChanneling) != 0 )
				text += "Spell Channeling<BR>";

			if ( (prop = var.Attributes.SpellDamage) != 0 )
				text += "Spell Damage Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusStam) != 0 )
				text += "Stamina Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusStr) != 0 )
				text += "Strength Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.WeaponSpeed) != 0 )
				text += "Swing Speed Increase: " + prop.ToString() + "%<BR>";

			text = AddResistInfo ( var, text );

			if ( var.HitPoints >= 0 && var.MaxHitPoints > 0 )
			{
				if ( fromToolMenu )
					text += "Durability About: " + var.MaxHitPoints + "<BR>";
				else
					text += "Durability: " + var.HitPoints + "/" + var.MaxHitPoints + "<BR>";
			}

			text += "Equipment: " + var.EquipLayerName( var.Layer ) + "<BR>";

			return text;
        }

        public static string AddSpellbookInfo ( Spellbook var, string text, bool fromToolMenu )
        {
			int prop;
			TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

            if (var.BuiltBy != null)
				text += "Crafted by " + var.BuiltBy.Name + "<BR>";

			text = ItemSkills.SpellbookSkills( var, text );

			if( var.Slayer != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( var.Slayer );
				if( entry != null )
					text += "" + cultInfo.ToTitleCase(CliLocTable.Lookup( entry.Title )) + "<BR>";
			}

			if( var.Slayer2 != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( var.Slayer2 );
				if( entry != null )
					text += "" + cultInfo.ToTitleCase(CliLocTable.Lookup( entry.Title )) + "<BR>";
			}

			if ( (prop = var.Attributes.WeaponDamage) != 0 )
				text += "Damage Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.DefendChance) != 0 )
				text += "Defense Change Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusDex) != 0 )
				text += "Dexterity Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.EnhancePotions) != 0 )
				text += "Enhance Potions: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.CastRecovery) != 0 )
				text += "Faster Cast Recovery: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.CastSpeed) != 0 )
				text += "Faster Casting: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.AttackChance) != 0 )
				text += "Hit Chance Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusHits) != 0 )
				text += "Hit Point Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusInt) != 0 )
				text += "Intelligence Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.LowerManaCost) != 0 && MyServerSettings.LowerMana() > 0 )
			{
				if ( prop > MyServerSettings.LowerMana() ){ prop = MyServerSettings.LowerMana(); }
				text += "Lower Mana Cost: " + prop.ToString() + "<BR>";
			}

			if ( (prop = var.Attributes.LowerRegCost) != 0 && MyServerSettings.LowerReg() > 0 )
			{
				if ( prop > MyServerSettings.LowerReg() ){ prop = MyServerSettings.LowerReg(); }
				text += "Lower Reagent Cost: " + prop.ToString() + "%<BR>";
			}

			if ( (prop = (var.GetLuckBonus() + var.Attributes.Luck)) != 0 )
				text += "Luck: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusMana) != 0 )
				text += "Mana Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.RegenMana) != 0 )
				text += "Mana Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.NightSight) != 0 )
				text += "Night Sight<BR>";

			if ( (prop = var.Attributes.ReflectPhysical) != 0 )
				text += "Reflect Physical Damage: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.RegenStam) != 0 )
				text += "Stamina Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.RegenHits) != 0 )
				text += "Hit Point Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.SpellChanneling) != 0 )
				text += "Spell Channeling<BR>";

			if ( (prop = var.Attributes.SpellDamage) != 0 )
				text += "Spell Damage Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusStam) != 0 )
				text += "Stamina Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusStr) != 0 )
				text += "Strength Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.WeaponSpeed) != 0 )
				text += "Swing Speed Increase: " + prop.ToString() + "%<BR>";

			text = AddResistInfo ( var, text );

			if ( var is SongBook && var.SpellCount > 0 )
			{
				if ( var.SpellCount == 1 ){ text += "1 Song<BR>"; }
				else { text += "" + var.SpellCount.ToString() + " Songs<BR>"; }
			}
			else if ( ( var is BookOfNinjitsu || var is BookOfBushido || var is MysticSpellbook ) && var.SpellCount > 0 )
			{
				if ( var.SpellCount == 1 ){ text += "1 Ability<BR>"; }
				else { text += "" + var.SpellCount.ToString() + " Abilities<BR>"; }
			}
			else if ( ( var is JediSpellbook || var is SythSpellbook ) && var.SpellCount > 0 )
			{
				if ( var.SpellCount == 1 ){ text += "1 Power<BR>"; }
				else { text += "" + var.SpellCount.ToString() + " Powers<BR>"; }
			}
			else if ( var.SpellCount > 0 )
			{
				if ( var.SpellCount == 1 ){ text += "1 Spell<BR>"; }
				else { text += "" + var.SpellCount.ToString() + " Spells<BR>"; }
			}

			text += "Equipment: " + var.EquipLayerName( var.Layer ) + "<BR>";

			return text;
        }

		public static string AddQuiverInfo ( BaseQuiver var, string text, bool fromToolMenu )
		{
			int prop;

            if (var.BuiltBy != null)
				text += "Crafted by " + var.BuiltBy.Name + "<BR>";

			if ( var.Quality == ClothingQuality.Exceptional )
				text += "Exceptional<BR>";

			if ( (prop = var.DamageIncrease) != 0 )
				text += "Damage Modifier: " + prop.ToString() + "%<BR>";

			int phys, fire, cold, pois, nrgy, chaos, direct;
			phys = fire = cold = pois = nrgy = chaos = direct = 0;

			var.AlterBowDamage( ref phys, ref fire, ref cold, ref pois, ref nrgy, ref chaos, ref direct );

			if ( phys != 0 )
				text += "Physical Damage: " + phys.ToString() + "%<BR>";

			if ( fire != 0 )
				text += "Fire Damage: " + fire.ToString() + "%<BR>";

			if ( cold != 0 )
				text += "Cold Damage: " + cold.ToString() + "%<BR>";

			if ( pois != 0 )
				text += "Poison Damage: " + pois.ToString() + "%<BR>";

			if ( nrgy != 0 )
				text += "Energy Damage: " + nrgy.ToString() + "%<BR>";

			if ( chaos != 0 )
				text += "Chaos Damage: " + chaos.ToString() + "%<BR>";

			if ( direct != 0 )
				text += "Direct Damage: " + direct.ToString() + "%<BR>";


			if ( (prop = var.Attributes.DefendChance) != 0 )
				text += "Defense Chance Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusDex) != 0 )
				text += "Dexterity Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.EnhancePotions) != 0 )
				text += "Enhance Potions: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.CastRecovery) != 0 )
				text += "Faster Cast Recovery: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.CastSpeed) != 0 )
				text += "Faster Casting: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.AttackChance) != 0 )
				text += "Hit Chance Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusHits) != 0 )
				text += "Hit Point Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusInt) != 0 )
				text += "Intelligence Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.LowerManaCost) != 0 && MyServerSettings.LowerMana() > 0 )
			{
				if ( prop > MyServerSettings.LowerMana() ){ prop = MyServerSettings.LowerMana(); }
				text += "Lower Mana Cost: " + prop.ToString() + "<BR>";
			}

			if ( (prop = var.Attributes.LowerRegCost) != 0 && MyServerSettings.LowerReg() > 0 )
			{
				if ( prop > MyServerSettings.LowerReg() ){ prop = MyServerSettings.LowerReg(); }
				text += "Lower Reagent Cost: " + prop.ToString() + "%<BR>";
			}

			if ( (prop = (var.GetLuckBonus() + var.Attributes.Luck)) != 0 )
				text += "Luck: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusMana) != 0 )
				text += "Mana Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.RegenMana) != 0 )
				text += "Mana Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.NightSight) != 0 )
				text += "Night Sight<BR>";

			if ( (prop = var.Attributes.ReflectPhysical) != 0 )
				text += "Reflect Physical Damage: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.RegenStam) != 0 )
				text += "Stamina Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.RegenHits) != 0 )
				text += "Hit Point Regeneration: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.SpellDamage) != 0 )
				text += "Spell Damage Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.Attributes.BonusStam) != 0 )
				text += "Stamina Increase: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.BonusStr) != 0 )
				text += "Strength Bonus: " + prop.ToString() + "<BR>";

			if ( (prop = var.Attributes.WeaponSpeed) != 0 )
				text += "Swing Speed Increase: " + prop.ToString() + "%<BR>";

			if ( (prop = var.LowerAmmoCost) > 0 )
				text += "Lower Ammo Cost: " + prop.ToString() + "%<BR>";

			if ( (prop = var.WeightReduction) != 0 )
				text += "Weight Reduction: " + prop.ToString() + "%<BR>";

			text += "Equipment: " + var.EquipLayerName( var.Layer ) + "<BR>";

			return text;
        }

		public static string Capitalize( string text )
		{
			return MorphingTime.CapitalizeWords(text);
		}

		public static string GetMaterial( CraftResource resource )
		{
			string material = CraftResources.GetName( resource );

			if ( material == "Iron" || material == "Normal" || resource == CraftResource.None )
				material = null;

			return material;
		}

		public static string AddResistInfo ( Item var, string text )
		{
			int v = var.PhysicalResistance;

			if ( v != 0 )
				text += "Physical Resist: " + v.ToString() + "%<BR>";

			v = var.FireResistance;

			if ( v != 0 )
				text += "Fire Resist: " + v.ToString() + "%<BR>";

			v = var.ColdResistance;

			if ( v != 0 )
				text += "Cold Resist: " + v.ToString() + "%<BR>";

			v = var.PoisonResistance;

			if ( v != 0 )
				text += "Poison Resist: " + v.ToString() + "%<BR>";

			v = var.EnergyResistance;

			if ( v != 0 )
				text += "Energy Resist: " + v.ToString() + "%<BR>";

			return text;
		}

		public static string AddArtyPoints( Item var, string text )
		{
			if ( var is IGiftable && ((IGiftable)var).Points > 0 )
				text += "Enchantment Points: " + ((IGiftable)var).Points.ToString() + "<BR>";

			return text;
		}

		public static void SpellbookName( Item item )
		{
			string text = item.Name;
			string book = RandomThings.GetRandomBookType(true);

			if ( item is BookOfNinjitsu )
			{
				text = RandomThings.GetRandomBelongsTo( "orient" );

				switch ( Utility.RandomMinMax( 0, 7 ) ) 
				{
					case 0: text += " " + book + " of Ninjitsu"; break;
					case 1: text += " " + book + " of the Ninja"; break;
					case 2: text += " " + book + " of the Ninja Arts"; break;
					case 3: text += " " + book + " of Ninja Way"; break;
					case 4: text += " " + book + " of Ninja Secrets"; break;
					case 5: text += " " + book + " of the Ninja Code"; break;
					case 6: text += " " + book + " of the Ninjitsu"; break;
					case 7: text += " " + book + " of the Ninja Path"; break;
				}
			}
			else if ( item is BookOfBushido )
			{
				text = RandomThings.GetRandomBelongsTo( "orient" );

				switch ( Utility.RandomMinMax( 0, 7 ) ) 
				{
					case 0: text += " " + book + " of Bushido"; break;
					case 1: text += " " + book + " of the Samurai"; break;
					case 2: text += " " + book + " of the Bushido Arts"; break;
					case 3: text += " " + book + " of Samurai Way"; break;
					case 4: text += " " + book + " of Bushido Secrets"; break;
					case 5: text += " " + book + " of the Samurai Code"; break;
					case 6: text += " " + book + " of the Samurai"; break;
					case 7: text += " " + book + " of the Samurai Path"; break;
				}
			}
			else if ( item is BookOfChivalry )
			{
				text = RandomThings.GetRandomBelongsTo( "regular" );

				switch ( Utility.RandomMinMax( 0, 7 ) ) 
				{
					case 0: text += " " + book + " of Knightship"; break;
					case 1: text += " " + book + " of the Cavalier"; break;
					case 2: text += " " + book + " of the Knight Code"; break;
					case 3: text += " " + book + " of Knightship Way"; break;
					case 4: text += " " + book + " of the Cavelier's Path"; break;
					case 5: text += " " + book + " of the Knight's Code"; break;
					case 6: text += " " + book + " of the Knight"; break;
					case 7: text += " " + book + " of the Knight's Path"; break;
				}
			}
			else if ( item is SongBook )
			{
				text = RandomThings.GetRandomBelongsTo( "regular" );

				switch ( Utility.RandomMinMax( 0, 6 ) ) 
				{
					case 0: text += " " + book + " of the Bard"; break;
					case 1: text += " " + book + " of the Minstrel"; break;
					case 2: text += " " + book + " of the Balladeer"; break;
					case 3: text += " " + book + " of the Troubadour"; break;
					case 4: text += " " + book + " of the Poet"; break;
					case 5: text += " " + book + " of the Musician"; break;
					case 6: text += " " + book + " of the Singer"; break;
				}
			}
			else if ( item is NecromancerSpellbook )
			{
				text = RandomThings.GetRandomBelongsTo( "regular" );

				string sEvil = "Evil";
				switch ( Utility.RandomMinMax( 0, 7 ) ) 
				{
					case 0: sEvil = "Evil";			break;
					case 1: sEvil = "Vile";			break;
					case 2: sEvil = "Sinister";		break;
					case 3: sEvil = "Wicked";		break;
					case 4: sEvil = "Corrupt";		break;
					case 5: sEvil = "Hateful";		break;
					case 6: sEvil = "Malevolent";	break;
					case 7: sEvil = "Nefarious";	break;
				}

				switch ( Utility.RandomMinMax( 1, 3 ) ) 
				{
					case 1: text += " " + book + " of " + sEvil + " Spells";	break;
					case 2: text += " " + book + " of " + sEvil + " Magic";		break;
					case 3: text += " " + book + " of " + sEvil + " Witchery";	break;
				}
			}
			else if ( item is ElementalSpellbook )
			{
				text = RandomThings.GetRandomBelongsTo( "regular" );

				switch ( Utility.RandomMinMax( 1, 3 ) ) 
				{
					case 1: text += " " + book + " of the Elements";		break;
					case 2: text += " " + book + " of Elemental Magic";		break;
					case 3: text += " " + book + " of Elementalism";		break;
				}
			}
			else if ( ((Spellbook)item).MageryBook() )
			{
				text = RandomThings.GetRandomBelongsTo( "regular" );

				switch ( Utility.RandomMinMax( 1, 3 ) ) 
				{
					case 1: text += " " + book + " of Spells";		break;
					case 2: text += " " + book + " of Magic";		break;
					case 3: text += " " + book + " of Wizardry";		break;
				}
			}

			if ( Item.IsStandardResource( item.Resource ) && Utility.RandomBool() )
				item.Hue = Utility.RandomColor(0);

			item.Name = text;
		}
    }
}