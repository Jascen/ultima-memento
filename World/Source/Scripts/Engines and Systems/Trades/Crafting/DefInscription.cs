using System;
using Server.Items;
using Server.Spells;
using Server.Spells.Elementalism;

namespace Server.Engines.Craft
{
	public class DefInscription : CraftSystem
	{
		public override SkillName MainSkill
		{
			get { return SkillName.Inscribe; }
		}

        public override int GumpImage
        {
            get { return 9599; }
        }

		public override int GumpTitleNumber
		{
			get { return 1044009; } // <CENTER>INSCRIPTION MENU</CENTER>
		}

        public override bool ShowGumpInfo
        {
            get { return true; }
        }

		public override string CraftSystemTxt
		{
			get { return "Crafting: Inscription"; }
		}

		private static CraftSystem m_CraftSystem;

		public static CraftSystem CraftSystem
		{
			get
			{
				if (m_CraftSystem == null)
					m_CraftSystem = new DefInscription();

				return m_CraftSystem;
			}
		}

		public override double GetChanceAtMin(CraftItem item)
		{
			return 0.0; // 0%
		}

		private DefInscription()
			: base(1, 1, 1.25)// base( 1, 1, 3.0 )
		{
		}

		public override string GetCategoryWarning( int selectedGroup )
		{
			return "Crafted scrolls cannot be placed in spellbooks";
		}

		public override int CanCraft(Mobile from, BaseTool tool, Type typeItem)
		{
			if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
				return 1044038; // You have worn out your tool!
			else if (!BaseTool.CheckAccessible(tool, from))
				return 1044263; // The tool must be on your person to use.

			if (typeItem != null)
			{
				object o = Activator.CreateInstance(typeItem);

				if (o is SpellScroll)
				{
					SpellScroll scroll = (SpellScroll)o;
					Spellbook book = Spellbook.Find(from, scroll.SpellID);

					bool hasSpell = (book != null && book.HasSpell(scroll.SpellID));

					scroll.Delete();

					return (hasSpell ? 0 : 1042404); // null : You don't have that spell!
				}
				else if (o is Item)
				{
					((Item)o).Delete();
				}
			}

			return 0;
		}

		public override void PlayCraftEffect(Mobile from)
		{
			CraftSystem.CraftSound( from, 0x249, m_Tools );
		}

		private static Type typeofSpellScroll = typeof(SpellScroll);

		public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, CraftItem item)
		{
			if (toolBroken)
				from.SendLocalizedMessage(1044038); // You have worn out your tool

			if (!typeofSpellScroll.IsAssignableFrom(item.ItemType)) //  not a scroll
			{
				if (failed)
				{
					if (lostMaterial)
						return 1044043; // You failed to create the item, and some of your materials are lost.
					else
						return 1044157; // You failed to create the item, but no materials were lost.
				}
				else
				{
					if (quality == 0)
						return 502785; // You were barely able to make this item.  It's quality is below average.
					else if (quality == 2)
						return 1044155; // You create an exceptional quality item.
					else
						return 1044154; // You create the item.
				}
			}
			else
			{
				if (failed)
					return 501630; // You fail to inscribe the scroll, and the scroll is ruined.
				else
					return 501629; // You inscribe the spell and put the scroll in your backpack.
			}
		}

		private int m_Circle, m_Mana;

		private enum Reg { BlackPearl, Bloodmoss, Garlic, Ginseng, MandrakeRoot, Nightshade, SulfurousAsh, SpidersSilk }

		private Type[] m_RegTypes = new Type[]
			{
				typeof( BlackPearl ),
				typeof( Bloodmoss ),
				typeof( Garlic ),
				typeof( Ginseng ),
				typeof( MandrakeRoot ),
				typeof( Nightshade ),
				typeof( SulfurousAsh ),
				typeof( SpidersSilk )
			};

		private int m_Index;

		private void AddSpell(Type type, params Reg[] regs)
		{
			AddSpell(-1, type, false, regs);
		}

		private void AddSpell(int recipeNumber, Type type,  params Reg[] regs)
		{
			AddSpell(recipeNumber, type, true, regs );
		}

		private void AddSpell(int recipeNumber, Type type, bool requireArcaneScroll, params Reg[] regs)
		{
			// Temp Hack: Remove Recipe
			recipeNumber = -1;

			double minSkill, maxSkill;

			string title = "Magery Circles 1 & 2";

			switch (m_Circle)
			{
				default:
				case 0: minSkill = -25.0; maxSkill = 25.0; break;
				case 1: minSkill = -10.8; maxSkill = 39.2; break;
				case 2: minSkill = 03.5; maxSkill = 53.5; title = "Magery Circles 3 & 4"; break;
				case 3: minSkill = 17.8; maxSkill = 67.8; title = "Magery Circles 3 & 4"; break;
				case 4: minSkill = 32.1; maxSkill = 82.1; title = "Magery Circles 5 & 6"; break;
				case 5: minSkill = 46.4; maxSkill = 96.4; title = "Magery Circles 5 & 6"; break;
				case 6: minSkill = 60.7; maxSkill = 110.7; title = "Magery Circles 7 & 8"; break;
				case 7: minSkill = 75.0; maxSkill = 125.0; title = "Magery Circles 7 & 8"; break;
			}

			int index = AddCraftRecipe(recipeNumber, type, title, 1044381 + m_Index++, minSkill, maxSkill, m_RegTypes[(int)regs[0]], 1044353 + (int)regs[0], 1, 1044361 + (int)regs[0]);

			for (int i = 1; i < regs.Length; ++i)
				AddRes(index, m_RegTypes[(int)regs[i]], 1044353 + (int)regs[i], 1, 1044361 + (int)regs[i]);

			if ( requireArcaneScroll )
				AddRes( index, typeof( ArcaneScroll ), "Arcane Scroll", 1, 1053098 );
			else
				AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);

			SetManaReq(index, m_Mana);
		}

		private void AddNecroSpell(int spell, int mana, double minSkill, Type type, params Type[] regs)
		{
			AddNecroSpell(-1, spell, mana, minSkill, type, false, regs);
		}

		private void AddNecroSpell(int recipeNumber, int spell, int mana, double minSkill, Type type, bool requireArcaneScroll, params Type[] regs)
		{
			// Temp Hack: Remove Recipe
			recipeNumber = -1;

			double maxSkill = minSkill + 1;

			int id = CraftItem.ItemIDOf(regs[0]);

			int index = AddCraftRecipe(recipeNumber, type, "Necromancy Spells", 1060509 + spell, minSkill, maxSkill, regs[0], id < 0x4000 ? 1020000 + id : 1078872 + id, 1, 501627);

			for (int i = 1; i < regs.Length; ++i)
			{
				id = CraftItem.ItemIDOf(regs[i]);
				AddRes(index, regs[i], id < 0x4000 ? 1020000 + id : 1078872 + id, 1, 501627);
			}

			if ( requireArcaneScroll )
				AddRes( index, typeof( ArcaneScroll ), "Arcane Scroll", 1, 1053098 );
			else
				AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);

			SetManaReq(index, mana);
		}

		private int m_Elly;

		private void AddElementalSpell( Type type )
		{
			AddElementalSpell( -1, type, false );
		}

		private void AddElementalSpell( int recipeNumber, Type type, bool requireArcaneScroll )
		{
			// Temp Hack: Remove Recipe
			recipeNumber = -1;

			double minSkill, maxSkill;

			int blood = 2+m_Circle;

			switch (m_Circle)
			{
				default:
				case 0: minSkill = -25.0; maxSkill = 25.0; break;
				case 1: minSkill = -10.8; maxSkill = 39.2; break;
				case 2: minSkill = 03.5; maxSkill = 53.5; break;
				case 3: minSkill = 17.8; maxSkill = 67.8; break;
				case 4: minSkill = 32.1; maxSkill = 82.1; break;
				case 5: minSkill = 46.4; maxSkill = 96.4; break;
				case 6: minSkill = 60.7; maxSkill = 110.7; break;
				case 7: minSkill = 75.0; maxSkill = 125.0; break;
			}

			int index = AddCraftRecipe(recipeNumber, type, "Elemental Spells", ElementalSpell.CommonInfo( (300 + m_Elly++), 1 ), minSkill, maxSkill, typeof(DaemonBlood), 1023965, blood, 1042081);

			if ( requireArcaneScroll )
				AddRes( index, typeof( ArcaneScroll ), "Arcane Scroll", 1, 1053098 );
			else
				AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);

			SetManaReq(index, m_Mana);
		}

		public override void InitCraftList()
		{
			m_Circle = 0;
			m_Mana = 5;
			AddElementalSpell( typeof( Elemental_Armor_Scroll ) );
			AddElementalSpell( typeof( Elemental_Bolt_Scroll ) );
			AddElementalSpell( typeof( Elemental_Mend_Scroll ) );
			AddElementalSpell( typeof( Elemental_Sanctuary_Scroll ) );
			m_Circle = 1;
			m_Mana = 7;
			AddElementalSpell( typeof( Elemental_Pain_Scroll ) );
			AddElementalSpell( typeof( Elemental_Protection_Scroll ) );
			AddElementalSpell( typeof( Elemental_Purge_Scroll ) );
			AddElementalSpell( typeof( Elemental_Steed_Scroll ) );
			m_Circle = 2;
			m_Mana = 10;
			AddElementalSpell( typeof( Elemental_Call_Scroll ) );
			AddElementalSpell( typeof( Elemental_Force_Scroll ) );
			AddElementalSpell( typeof( Elemental_Wall_Scroll ) );
			AddElementalSpell( typeof( Elemental_Warp_Scroll ) );
			m_Circle = 3;
			m_Mana = 14;
			AddElementalSpell( typeof( Elemental_Field_Scroll ) );
			AddElementalSpell( typeof( Elemental_Restoration_Scroll ) );
			AddElementalSpell( typeof( Elemental_Strike_Scroll ) );
			AddElementalSpell( typeof( Elemental_Void_Scroll ) );
			m_Circle = 4;
			m_Mana = 19;
			AddElementalSpell( 1, typeof( Elemental_Blast_Scroll ), true );
			AddElementalSpell( 2, typeof( Elemental_Echo_Scroll ), true );
			AddElementalSpell( 3, typeof( Elemental_Fiend_Scroll ), true );
			AddElementalSpell( 4, typeof( Elemental_Hold_Scroll ), true );
			m_Circle = 5;
			m_Mana = 24;
			AddElementalSpell( 5, typeof( Elemental_Barrage_Scroll ), true );
			AddElementalSpell( 6, typeof( Elemental_Rune_Scroll ), true );
			AddElementalSpell( 7, typeof( Elemental_Storm_Scroll ), true );
			AddElementalSpell( 8, typeof( Elemental_Summon_Scroll ), true );
			m_Circle = 6;
			m_Mana = 40;
			AddElementalSpell( 9, typeof( Elemental_Devastation_Scroll ), true );
			AddElementalSpell( 10, typeof( Elemental_Fall_Scroll ), true );
			AddElementalSpell( 11, typeof( Elemental_Gate_Scroll ), true );
			AddElementalSpell( 12, typeof( Elemental_Havoc_Scroll ), true );
			m_Circle = 7;
			m_Mana = 50;
			AddElementalSpell( 13, typeof( Elemental_Apocalypse_Scroll ), true );
			AddElementalSpell( 14, typeof( Elemental_Lord_Scroll ), true );
			AddElementalSpell( 15, typeof( Elemental_Soul_Scroll ), true );
			AddElementalSpell( 16, typeof( Elemental_Spirit_Scroll ), true );

			m_Circle = 0;
			m_Mana = 4;

			AddSpell(typeof(ReactiveArmorScroll), Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh);
			AddSpell(typeof(ClumsyScroll), Reg.Bloodmoss, Reg.Nightshade);
			AddSpell(typeof(CreateFoodScroll), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot);
			AddSpell(typeof(FeeblemindScroll), Reg.Nightshade, Reg.Ginseng);
			AddSpell(typeof(HealScroll), Reg.Garlic, Reg.Ginseng, Reg.SpidersSilk);
			AddSpell(typeof(MagicArrowScroll), Reg.SulfurousAsh);
			AddSpell(typeof(NightSightScroll), Reg.SpidersSilk, Reg.SulfurousAsh);
			AddSpell(typeof(WeakenScroll), Reg.Garlic, Reg.Nightshade);

			m_Circle = 1;
			m_Mana = 6;

			AddSpell(typeof(AgilityScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
			AddSpell(typeof(CunningScroll), Reg.Nightshade, Reg.MandrakeRoot);
			AddSpell(typeof(CureScroll), Reg.Garlic, Reg.Ginseng);
			AddSpell(typeof(HarmScroll), Reg.Nightshade, Reg.SpidersSilk);
			AddSpell(typeof(MagicTrapScroll), Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh);
			AddSpell(typeof(MagicUnTrapScroll), Reg.Bloodmoss, Reg.SulfurousAsh);
			AddSpell(typeof(ProtectionScroll), Reg.Garlic, Reg.Ginseng, Reg.SulfurousAsh);
			AddSpell(typeof(StrengthScroll), Reg.Nightshade, Reg.MandrakeRoot);

			m_Circle = 2;
			m_Mana = 9;

			AddSpell(typeof(BlessScroll), Reg.Garlic, Reg.MandrakeRoot);
			AddSpell(typeof(FireballScroll), Reg.BlackPearl);
			AddSpell(typeof(MagicLockScroll), Reg.Bloodmoss, Reg.Garlic, Reg.SulfurousAsh);
			AddSpell(typeof(PoisonScroll), Reg.Nightshade);
			AddSpell(typeof(TelekinisisScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
			AddSpell(typeof(TeleportScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
			AddSpell(typeof(UnlockScroll), Reg.Bloodmoss, Reg.SulfurousAsh);
			AddSpell(typeof(WallOfStoneScroll), Reg.Bloodmoss, Reg.Garlic);

			m_Circle = 3;
			m_Mana = 11;

			AddSpell(typeof(ArchCureScroll), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot);
			AddSpell(typeof(ArchProtectionScroll), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot, Reg.SulfurousAsh);
			AddSpell(typeof(CurseScroll), Reg.Garlic, Reg.Nightshade, Reg.SulfurousAsh);
			AddSpell(typeof(FireFieldScroll), Reg.BlackPearl, Reg.SpidersSilk, Reg.SulfurousAsh);
			AddSpell(typeof(GreaterHealScroll), Reg.Garlic, Reg.SpidersSilk, Reg.MandrakeRoot, Reg.Ginseng);
			AddSpell(typeof(LightningScroll), Reg.MandrakeRoot, Reg.SulfurousAsh);
			AddSpell(typeof(ManaDrainScroll), Reg.BlackPearl, Reg.SpidersSilk, Reg.MandrakeRoot);
			AddSpell(typeof(RecallScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot);

			m_Circle = 4;
			m_Mana = 14;

			AddSpell( 51, typeof(BladeSpiritsScroll), Reg.BlackPearl, Reg.Nightshade, Reg.MandrakeRoot);
			AddSpell( 52, typeof(DispelFieldScroll), Reg.BlackPearl, Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh);
			AddSpell( 53, typeof(IncognitoScroll), Reg.Bloodmoss, Reg.Garlic, Reg.Nightshade);
			AddSpell( 54, typeof(MagicReflectScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.SpidersSilk);
			AddSpell( 55, typeof(MindBlastScroll), Reg.BlackPearl, Reg.MandrakeRoot, Reg.Nightshade, Reg.SulfurousAsh);
			AddSpell( 56, typeof(ParalyzeScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.SpidersSilk);
			AddSpell( 57, typeof(PoisonFieldScroll), Reg.BlackPearl, Reg.Nightshade, Reg.SpidersSilk);
			AddSpell( 58, typeof(SummonCreatureScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);

			m_Circle = 5;
			m_Mana = 20;

			AddSpell( 59, typeof(DispelScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.SulfurousAsh);
			AddSpell( 60, typeof(EnergyBoltScroll), Reg.BlackPearl, Reg.Nightshade);
			AddSpell( 61, typeof(ExplosionScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
			AddSpell( 62, typeof(InvisibilityScroll), Reg.Bloodmoss, Reg.Nightshade);
			AddSpell( 63, typeof(MarkScroll), Reg.Bloodmoss, Reg.BlackPearl, Reg.MandrakeRoot);
			AddSpell( 64, typeof(MassCurseScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.Nightshade, Reg.SulfurousAsh);
			AddSpell( 65, typeof(ParalyzeFieldScroll), Reg.BlackPearl, Reg.Ginseng, Reg.SpidersSilk);
			AddSpell( 66, typeof(RevealScroll), Reg.Bloodmoss, Reg.SulfurousAsh);

			m_Circle = 6;
			m_Mana = 40;

			AddSpell( 67, typeof(ChainLightningScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SulfurousAsh);
			AddSpell( 68, typeof(EnergyFieldScroll), Reg.BlackPearl, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh);
			AddSpell( 69, typeof(FlamestrikeScroll), Reg.SpidersSilk, Reg.SulfurousAsh);
			AddSpell( 70, typeof(GateTravelScroll), Reg.BlackPearl, Reg.MandrakeRoot, Reg.SulfurousAsh);
			AddSpell( 71, typeof(ManaVampireScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);
			AddSpell( 72, typeof(MassDispelScroll), Reg.BlackPearl, Reg.Garlic, Reg.MandrakeRoot, Reg.SulfurousAsh);
			AddSpell( 73, typeof(MeteorSwarmScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SulfurousAsh, Reg.SpidersSilk);
			AddSpell( 74, typeof(PolymorphScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);

			m_Circle = 7;
			m_Mana = 50;

			AddSpell( 75, typeof(EarthquakeScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.Ginseng, Reg.SulfurousAsh);
			AddSpell( 76, typeof(EnergyVortexScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.Nightshade);
			AddSpell( 77, typeof(ResurrectionScroll), Reg.Bloodmoss, Reg.Garlic, Reg.Ginseng);
			AddSpell( 78, typeof(SummonAirElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);
			AddSpell( 79, typeof(SummonDaemonScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh);
			AddSpell( 80, typeof(SummonEarthElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);
			AddSpell( 81, typeof(SummonFireElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh);
			AddSpell( 82, typeof(SummonWaterElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);

			AddNecroSpell(0, 23, 39.6, typeof(AnimateDeadScroll), Reagent.GraveDust, Reagent.DaemonBlood);
			AddNecroSpell(1, 13, 19.6, typeof(BloodOathScroll), Reagent.DaemonBlood);
			AddNecroSpell(2, 11, 19.6, typeof(CorpseSkinScroll), Reagent.BatWing, Reagent.GraveDust);
			AddNecroSpell(3, 7, 19.6, typeof(CurseWeaponScroll), Reagent.PigIron);
			AddNecroSpell(4, 11, 19.6, typeof(EvilOmenScroll), Reagent.BatWing, Reagent.NoxCrystal);
			AddNecroSpell(5, 11, 39.6, typeof(HorrificBeastScroll), Reagent.BatWing, Reagent.DaemonBlood);
			AddNecroSpell(6, 23, 69.6, typeof(LichFormScroll), Reagent.GraveDust, Reagent.DaemonBlood, Reagent.NoxCrystal);
			AddNecroSpell(7, 17, 29.6, typeof(MindRotScroll), Reagent.BatWing, Reagent.DaemonBlood, Reagent.PigIron);
			AddNecroSpell(8, 5, 19.6, typeof(PainSpikeScroll), Reagent.GraveDust, Reagent.PigIron);
			AddNecroSpell( 83, 9, 17, 49.6, typeof(PoisonStrikeScroll), true, Reagent.NoxCrystal);
			AddNecroSpell( 84, 10, 29, 64.6, typeof(StrangleScroll), true, Reagent.DaemonBlood, Reagent.NoxCrystal);
			AddNecroSpell( 85, 11, 17, 29.6, typeof(SummonFamiliarScroll), true, Reagent.BatWing, Reagent.GraveDust, Reagent.DaemonBlood);
			AddNecroSpell( 86, 12, 23, 98.6, typeof(VampiricEmbraceScroll), true, Reagent.BatWing, Reagent.NoxCrystal, Reagent.PigIron);
			AddNecroSpell( 87, 13, 41, 79.6, typeof(VengefulSpiritScroll), true, Reagent.BatWing, Reagent.GraveDust, Reagent.PigIron);
			AddNecroSpell( 88, 14, 23, 59.6, typeof(WitherScroll), true, Reagent.GraveDust, Reagent.NoxCrystal, Reagent.PigIron);
			AddNecroSpell( 89, 15, 17, 79.6, typeof(WraithFormScroll), true, Reagent.NoxCrystal, Reagent.PigIron);
			AddNecroSpell( 90, 16, 40, 79.6, typeof(ExorcismScroll), true, Reagent.NoxCrystal, Reagent.GraveDust);

			int index;

			// Blank Scrolls
			index = AddCraft( typeof( BlankScroll ), "Books & Scrolls", "Blank Scrolls", 40.0, 70.0, typeof( BarkFragment ), 1073477, 1, 1073478 );
			index = AddCraft( typeof( BlankScroll ), "Books & Scrolls", "A batch of Blank Scrolls", 70.0, 70.0, typeof( BarkFragment ), 1073477, 1, 1073478 );
			SetUseAllRes( index, true );

			index = AddCraft( typeof( ArcaneScroll ), "Books & Scrolls", "Arcane Scroll", 50.0, 70.0, typeof( BlankScroll ), 1044377, 1, 1044378 );
			AddRes( index, typeof( ArcaneGem ), "arcane gem", 1, 1053098 );
			AddOrSkill( index, 50.0, 50.0, SkillName.Elementalism, SkillName.Magery, SkillName.Necromancy );
			index = AddCraft( typeof( ArcaneScroll ), "Books & Scrolls", "A batch of Arcane Scrolls", 70.0, 70.0, typeof( BlankScroll ), 1044377, 1, 1044378 );
			AddRes( index, typeof( ArcaneGem ), "arcane gem", 1, 1053098 );
			AddOrSkill( index, 50.0, 50.0, SkillName.Elementalism, SkillName.Magery, SkillName.Necromancy );
			SetUseAllRes( index, true );

			// Writing Book
			index = AddCraft(typeof(WritingBook), "Books & Scrolls", "Book", 50.0, 126, typeof( Leather ), 1044462, 8, 1044463);
			AddRes( index, typeof( BlankScroll ), 1044377, 10, 1044378 );
			AddRes( index, typeof( Beeswax ), 1025154, 5, 1044253 );

			index = AddCraft( typeof( Monocle ), "Books & Scrolls", "Librarian Set", 5.0, 55.0, typeof( Leather ), 1044462, 3, 1044463);
			AddRes( index, typeof( BlankScroll ), 1044377, 2, 1044378 );

			// Runebook
			index = AddCraft( typeof( Runebook ), "Books & Scrolls", "Runebook", 45.0, 95.0, typeof( Leather ), 1044462, 8, 1044463);
			AddRes( index, typeof( BlankScroll ), 1044377, 10, 1044378 );
			AddRes( index, typeof( Beeswax ), 1025154, 5, 1044253 );
			AddRes( index, typeof( GateTravelScroll ), 1044446, 1, 1044253 );

			index = AddCraft(typeof(SongBook), "Books & Scrolls", "Bardic Songs", 50.0, 126, typeof( Leather ), 1044462, 8, 1044463);
			AddRes( index, typeof( BlankScroll ), 1044377, 10, 1044378 );
			AddRes( index, typeof( Beeswax ), 1025154, 5, 1044253 );

			index = AddCraft(typeof(BookOfBushido), "Books & Scrolls", "Bushido Book", 50.0, 126, typeof( Leather ), 1044462, 8, 1044463);
			AddRes( index, typeof( BlankScroll ), 1044377, 10, 1044378 );
			AddRes( index, typeof( Beeswax ), 1025154, 5, 1044253 );

			index = AddCraft(typeof(ElementalSpellbook), "Books & Scrolls", "Elemental Spellbook", 50.0, 126, typeof( Leather ), 1044462, 8, 1044463);
			AddRes( index, typeof( BlankScroll ), 1044377, 10, 1044378 );
			AddRes( index, typeof( Beeswax ), 1025154, 5, 1044253 );

			index = AddCraft(typeof(BookOfChivalry), "Books & Scrolls", "Knightship Book", 50.0, 126, typeof( Leather ), 1044462, 8, 1044463);
			AddRes( index, typeof( BlankScroll ), 1044377, 10, 1044378 );
			AddRes( index, typeof( Beeswax ), 1025154, 5, 1044253 );

			index = AddCraft(typeof(Spellbook), "Books & Scrolls", "Magery Spellbook", 50.0, 126, typeof( Leather ), 1044462, 8, 1044463);
			AddRes( index, typeof( BlankScroll ), 1044377, 10, 1044378 );
			AddRes( index, typeof( Beeswax ), 1025154, 5, 1044253 );

			index = AddCraft(typeof(NecromancerSpellbook), "Books & Scrolls", "Necromancer Spellbook", 50.0, 126, typeof( Leather ), 1044462, 8, 1044463);
			AddRes( index, typeof( BlankScroll ), 1044377, 10, 1044378 );
			AddRes( index, typeof( Beeswax ), 1025154, 5, 1044253 );

			index = AddCraft(typeof(BookOfNinjitsu), "Books & Scrolls", "Ninjitsu Book", 50.0, 126, typeof( Leather ), 1044462, 8, 1044463);
			AddRes( index, typeof( BlankScroll ), 1044377, 10, 1044378 );
			AddRes( index, typeof( Beeswax ), 1025154, 5, 1044253 );

			// Set the overridable material
			SetSubRes( typeof( Leather ), CraftResources.GetClilocCraftName( CraftResource.RegularLeather ) );

			// Add every material you want the player to be able to choose from
			// This will override the overridable material

			int cannot = 1049312; // You have no idea how to work this leather.

			AddSubRes( typeof( Leather ),				CraftResources.GetClilocCraftName( CraftResource.RegularLeather ), CraftResources.GetSkill( CraftResource.RegularLeather ), CraftResources.GetClilocMaterialName( CraftResource.RegularLeather ), cannot );
			AddSubRes( typeof( HornedLeather ),			CraftResources.GetClilocCraftName( CraftResource.HornedLeather ), CraftResources.GetSkill( CraftResource.HornedLeather ), CraftResources.GetClilocMaterialName( CraftResource.HornedLeather ), cannot );
			AddSubRes( typeof( BarbedLeather ),			CraftResources.GetClilocCraftName( CraftResource.BarbedLeather ), CraftResources.GetSkill( CraftResource.BarbedLeather ), CraftResources.GetClilocMaterialName( CraftResource.BarbedLeather ), cannot );
			AddSubRes( typeof( NecroticLeather ),		CraftResources.GetClilocCraftName( CraftResource.NecroticLeather ), CraftResources.GetSkill( CraftResource.NecroticLeather ), CraftResources.GetClilocMaterialName( CraftResource.NecroticLeather ), cannot );
			AddSubRes( typeof( VolcanicLeather ),		CraftResources.GetClilocCraftName( CraftResource.VolcanicLeather ), CraftResources.GetSkill( CraftResource.VolcanicLeather ), CraftResources.GetClilocMaterialName( CraftResource.VolcanicLeather ), cannot );
			AddSubRes( typeof( FrozenLeather ),			CraftResources.GetClilocCraftName( CraftResource.FrozenLeather ), CraftResources.GetSkill( CraftResource.FrozenLeather ), CraftResources.GetClilocMaterialName( CraftResource.FrozenLeather ), cannot );
			AddSubRes( typeof( SpinedLeather ),			CraftResources.GetClilocCraftName( CraftResource.SpinedLeather ), CraftResources.GetSkill( CraftResource.SpinedLeather ), CraftResources.GetClilocMaterialName( CraftResource.SpinedLeather ), cannot );
			AddSubRes( typeof( GoliathLeather ),		CraftResources.GetClilocCraftName( CraftResource.GoliathLeather ), CraftResources.GetSkill( CraftResource.GoliathLeather ), CraftResources.GetClilocMaterialName( CraftResource.GoliathLeather ), cannot );
			AddSubRes( typeof( DraconicLeather ),		CraftResources.GetClilocCraftName( CraftResource.DraconicLeather ), CraftResources.GetSkill( CraftResource.DraconicLeather ), CraftResources.GetClilocMaterialName( CraftResource.DraconicLeather ), cannot );
			AddSubRes( typeof( HellishLeather ),		CraftResources.GetClilocCraftName( CraftResource.HellishLeather ), CraftResources.GetSkill( CraftResource.HellishLeather ), CraftResources.GetClilocMaterialName( CraftResource.HellishLeather ), cannot );
			AddSubRes( typeof( DinosaurLeather ),		CraftResources.GetClilocCraftName( CraftResource.DinosaurLeather ), CraftResources.GetSkill( CraftResource.DinosaurLeather ), CraftResources.GetClilocMaterialName( CraftResource.DinosaurLeather ), cannot );
			AddSubRes( typeof( AlienLeather ),			CraftResources.GetClilocCraftName( CraftResource.AlienLeather ), CraftResources.GetSkill( CraftResource.AlienLeather ), CraftResources.GetClilocMaterialName( CraftResource.AlienLeather ), cannot );
			AddSubRes( typeof( AdesoteLeather ),		CraftResources.GetClilocCraftName( CraftResource.Adesote ), CraftResources.GetSkill( CraftResource.Adesote ), CraftResources.GetClilocMaterialName( CraftResource.Adesote ), cannot );
			AddSubRes( typeof( BiomeshLeather ),		CraftResources.GetClilocCraftName( CraftResource.Biomesh ), CraftResources.GetSkill( CraftResource.Biomesh ), CraftResources.GetClilocMaterialName( CraftResource.Biomesh ), cannot );
			AddSubRes( typeof( CerlinLeather ),			CraftResources.GetClilocCraftName( CraftResource.Cerlin ), CraftResources.GetSkill( CraftResource.Cerlin ), CraftResources.GetClilocMaterialName( CraftResource.Cerlin ), cannot );
			AddSubRes( typeof( DurafiberLeather ),		CraftResources.GetClilocCraftName( CraftResource.Durafiber ), CraftResources.GetSkill( CraftResource.Durafiber ), CraftResources.GetClilocMaterialName( CraftResource.Durafiber ), cannot );
			AddSubRes( typeof( FlexicrisLeather ),		CraftResources.GetClilocCraftName( CraftResource.Flexicris ), CraftResources.GetSkill( CraftResource.Flexicris ), CraftResources.GetClilocMaterialName( CraftResource.Flexicris ), cannot );
			AddSubRes( typeof( HyperclothLeather ),		CraftResources.GetClilocCraftName( CraftResource.Hypercloth ), CraftResources.GetSkill( CraftResource.Hypercloth ), CraftResources.GetClilocMaterialName( CraftResource.Hypercloth ), cannot );
			AddSubRes( typeof( NylarLeather ),			CraftResources.GetClilocCraftName( CraftResource.Nylar ), CraftResources.GetSkill( CraftResource.Nylar ), CraftResources.GetClilocMaterialName( CraftResource.Nylar ), cannot );
			AddSubRes( typeof( NyloniteLeather ),		CraftResources.GetClilocCraftName( CraftResource.Nylonite ), CraftResources.GetSkill( CraftResource.Nylonite ), CraftResources.GetClilocMaterialName( CraftResource.Nylonite ), cannot );
			AddSubRes( typeof( PolyfiberLeather ),		CraftResources.GetClilocCraftName( CraftResource.Polyfiber ), CraftResources.GetSkill( CraftResource.Polyfiber ), CraftResources.GetClilocMaterialName( CraftResource.Polyfiber ), cannot );
			AddSubRes( typeof( SynclothLeather ),		CraftResources.GetClilocCraftName( CraftResource.Syncloth ), CraftResources.GetSkill( CraftResource.Syncloth ), CraftResources.GetClilocMaterialName( CraftResource.Syncloth ), cannot );
			AddSubRes( typeof( ThermoweaveLeather ),	CraftResources.GetClilocCraftName( CraftResource.Thermoweave ), CraftResources.GetSkill( CraftResource.Thermoweave ), CraftResources.GetClilocMaterialName( CraftResource.Thermoweave ), cannot );

			BreakDown = true;
			Repair = false;
			CanEnhance = true;
		}
	}
}