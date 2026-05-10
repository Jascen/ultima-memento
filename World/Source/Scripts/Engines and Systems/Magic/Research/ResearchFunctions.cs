using System;
using System.Collections;
using Server.Items;
using Server.Network;
using Server.Commands;
using Server.Mobiles;
using System.Globalization;

namespace Server.Misc
{
	class ResearchSettings
    {
		public static bool BookCaster( Mobile m )
		{
			if ( m is PlayerMobile )
				return ((PlayerMobile)m).Preferences.UsingAncientBook;

			return false;
		}

		public static AncientSpellbook GetAncientTome( Mobile m )
		{
			Item item = m.FindItemOnLayer( Layer.Trinket );
			if ( item is AncientSpellbook && ((AncientSpellbook)item).Owner == m )
				return (item as AncientSpellbook);

			if ( m.Backpack.FindItemByType( typeof ( AncientSpellbook ) ) != null )
			{
				Item book = m.Backpack.FindItemByType( typeof ( AncientSpellbook ) );
				if ( ((AncientSpellbook)book).Owner == m )
					return (book as AncientSpellbook);
			}

			return null;
		}

		public static Item ResearchMaterials( Mobile m )
		{
			Item bag = null;

			if ( m.Backpack.FindItemByType( typeof ( ResearchBag ) ) != null )
			{
				Item sack = m.Backpack.FindItemByType( typeof ( ResearchBag ) );
				ResearchBag pouch = (ResearchBag)sack;
				if ( pouch.BagOwner == m )
				{
					bag = sack;
				}
			}

			return bag;
		}

		public static bool HasSpell( Mobile from, int spellID )
		{
			bool fromBook = ResearchSettings.BookCaster( from );

			if ( fromBook )
			{
				int spell = Int32.Parse( Research.SpellInformation( spellID, 12 ) );
				Spellbook book = Spellbook.Find( from, spell );
				if ( book is AncientSpellbook )
				{
					AncientSpellbook ancient = (AncientSpellbook)book;
						if ( ancient.owner != from )
							return false;
				}
				return ( book != null && book.HasSpell( spell ) );
			}
			else if ( Server.Misc.ResearchSettings.ResearchMaterials( from ) != null )
			{
				ResearchBag bag = (ResearchBag)( Server.Misc.ResearchSettings.ResearchMaterials( from ) );
				return Server.Misc.Research.GetResearch( bag, spellID );
			}

			return false;
		}

		public static string AncientKeywords()
		{
			return "<br><br>[CastAcidElemental<br><br>[CastAerialServant<br><br>[CastAirWalk<br><br>[CastAnimateBones<br><br>[CastAvalanche<br><br>[CastBanishDaemon<br><br>[CastBloodElemental<br><br>[CastCallDestruction<br><br>[CastCauseFear<br><br>[CastCharm<br><br>[CastClone<br><br>[CastConflagration<br><br>[CastConfusionBlast<br><br>[CastConjure<br><br>[CastCreateFire<br><br>[CastCreateGold<br><br>[CastCreateGolem<br><br>[CastDeathSpeak<br><br>[CastDeathVortex<br><br>[CastDevastation<br><br>[CastDivination<br><br>[CastElectricalElemental<br><br>[CastEnchant<br><br>[CastEndureCold<br><br>[CastEndureHeat<br><br>[CastEtherealTravel<br><br>[CastExplosion<br><br>[CastExtinguish<br><br>[CastFadefromSight<br><br>[CastFlameBolt<br><br>[CastFrostField<br><br>[CastFrostStrike<br><br>[CastGasCloud<br><br>[CastGemElemental<br><br>[CastGrantPeace<br><br>[CastHailStorm<br><br>[CastHealingTouch<br><br>[CastIceElemental<br><br>[CastIcicle<br><br>[CastIgnite<br><br>[CastIntervention<br><br>[CastInvokeDevil<br><br>[CastMagicSteed<br><br>[CastMaskofDeath<br><br>[CastMassDeath<br><br>[CastMassMight<br><br>[CastMassSleep<br><br>[CastMeteorShower<br><br>[CastMudElemental<br><br>[CastOpenGround<br><br>[CastPoisonElemental<br><br>[CastRestoration<br><br>[CastRingofFire<br><br>[CastRockFlesh<br><br>[CastSeeTruth<br><br>[CastSleep<br><br>[CastSleepField<br><br>[CastSneak<br><br>[CastSnowBall<br><br>[CastSpawnCreature<br><br>[CastSwarm<br><br>[CastWeedElemental<br><br>[CastWithstandDeath<br><br>[CastWizardEye<br><br><br><br><br><br>";
		}

		public static string BagOrBook()
		{
			return "Since you can cast these ancient spells with the contents of your research bag, or an ancient spellbook, you need to make a decision which option you are going to use. By default, magic is assumed to be unleashed by using the research bag. If you want to use an ancient spellbook instead, go into the HELP section of your PAPERDOLL. Choose the SETTINGS section where you can check the box for the ANCIENT SPELLBOOK. Uncheck the box if you wish to use the contents of your research bag instead.";
		}
	}

    class Research
    {
        public static void InvokeCommand( string c, Mobile from )
        {
            CommandSystem.Handle(from, String.Format("{0}{1}", CommandSystem.Prefix, c));
        }

		public static bool AlreadyHasBag( Mobile from ) /////////////////////////////////////////////////////////////////////////////////////////////
		{
			bool HasBag = false;

			ArrayList targets = new ArrayList();
			foreach ( Item item in World.Items.Values )
			{
				if ( item is ResearchBag )
				{
					ResearchBag bag = (ResearchBag)item;
					if ( bag.BagOwner == from )
						targets.Add( item );
				}
			}
			for ( int i = 0; i < targets.Count; ++i )
			{
				Item item = ( Item )targets[ i ];
				from.AddToBackpack( item );
				HasBag = true;
			}

			return HasBag;
		}

		public static void GetRidOfBook( Mobile from ) /////////////////////////////////////////////////////////////////////////////////////////////
		{
			ArrayList targets = new ArrayList();
			foreach ( Item item in World.Items.Values )
			{
				if ( item is AncientSpellbook )
				{
					AncientSpellbook book = (AncientSpellbook)item;
					if ( book.owner == from )
						targets.Add( item );
				}
			}
			for ( int i = 0; i < targets.Count; ++i )
			{
				Item item = ( Item )targets[ i ];
				item.Delete();
			}
		}

		public static void SetupBag( Mobile from, ResearchBag bag ) /////////////////////////////////////////////////////////////////////////////////
		{
			bag.BagOwner = from;
			bag.BagScrolls = 0;
			bag.BagQuills = 0;
			bag.BagInk = 0;

			FindLocation( from, 0, "ink", bag );
			FindLocation( from, 0, "mage", bag );
			FindLocation( from, 0, "necro", bag );
			FindLocation( from, 0, "rune", bag );
			FindLocation( from, 0, "research", bag );

			bag.ResearchSpells = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#";
			bag.ResearchPrep1 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#";
			bag.ResearchPrep2 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#";

			bag.SpellsMagery = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#";
			bag.SpellsNecromancy = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#";
			bag.RuneFound = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#";
		}

		public static Land PickWorld( int level ) /////////////////////////////////////////////////////////////////////////////////////////////////
		{
			Land searchLocation = Land.Sosaria;

			switch ( Utility.RandomMinMax( 0, level ) )
			{
				case 0:		searchLocation = Land.Sosaria;		break;
				case 1:		searchLocation = Land.UmberVeil;	break;
				case 2:		searchLocation = Land.Ambrosia;		break;
				case 3:		searchLocation = Land.Lodoria;		break;
				case 4:		searchLocation = Land.IslesDread;	break;
				case 5:		searchLocation = Land.Savaged;		break;
				case 6:		searchLocation = Land.Serpent;		break;
				case 7:		searchLocation = Land.Kuldar;		break;
			}

			return searchLocation;
		}

		public static int GetMaxCircleResearched( ResearchBag bag ) /////////////////////////////////////////////////////////////////////////////////
		{
			string found = bag.SpellsMagery;
			int circle = 0;
			int current = 0;
			int entry = 0;

			if ( found.Length > 0 )
			{
				string[] spells = found.Split('#');
				foreach (string spell in spells)
				{
					entry++;
					if ( spell == "1" )
					{
						current = Int32.Parse( Research.ScrollInformation( entry, 1 ) );
						if ( current > circle ){ circle = current; }
					}
				}
			}
			found = bag.SpellsNecromancy;
			entry = 64;
			if ( found.Length > 0 )
			{
				string[] spells = found.Split('#');
				foreach (string spell in spells)
				{
					entry++;
					if ( spell == "1" )
					{
						current = Int32.Parse( Research.ScrollInformation( entry, 1 ) );
						if ( current > circle ){ circle = current; }
					}
				}
			}

			return circle;
		}

		public static int GetMaxSchoolResearched( ResearchBag bag ) /////////////////////////////////////////////////////////////////////////////////
		{
			string found = bag.ResearchSpells;
			int school = 0;
			int current = 0;
			int entry = 0;

			if ( found.Length > 0 )
			{
				string[] spells = found.Split('#');
				foreach (string spell in spells)
				{
					entry++;
					if ( spell == "1" )
					{
						current = Int32.Parse( Research.SpellInformation( entry, 1 ) );
						if ( current > school ){ school = current; }
					}
				}
			}

			return school;
		}

		public static void FindLocation( Mobile m, int level, string category, ResearchBag bag ) ////////////////////////////////////////////////////
		{
			TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;
			string dungeon = "Dungeon Doom";
			Land searchLocation = PickWorld( level );

			string oldDungeon = "z";
			if ( category == "ink" )
				oldDungeon = bag.BagInkLocation;
			else if ( category == "mage" )
				oldDungeon = bag.SpellsMageLocation;
			else if ( category == "research" )
				oldDungeon = bag.ResearchLocation;
			else if ( category == "necro" )
				oldDungeon = bag.SpellsNecroLocation;
			else
				oldDungeon = bag.RuneLocation;

			int aCount = 0;
			ArrayList targets = new ArrayList();
			foreach ( Item target in World.Items.Values )
			if ( target is SearchBase )
			{
				if ( target.Land == searchLocation && Server.Misc.Worlds.GetRegionName( target.Map, target.Location ) != oldDungeon )
				{
					targets.Add( target );
					aCount++;
				}
			}

			aCount = Utility.RandomMinMax( 1, aCount );

			int xCount = 0;
			for ( int i = 0; i < targets.Count; ++i )
			{
				xCount++;

				if ( xCount == aCount )
				{
					Item finding = ( Item )targets[ i ];
					dungeon = Server.Misc.Worlds.GetRegionName( finding.Map, finding.Location );
				}
			}

			string sEvil = " ";
			string sCat = " Wizardry";
			string sBook = "";
			if ( category == "necro" )
			{
				sEvil = " Evil";
				switch ( Utility.RandomMinMax( 0, 7 ) ) 
				{
					case 0: sEvil = " Evil";		break;
					case 1: sEvil = " Vile";		break;
					case 2: sEvil = " Sinister";	break;
					case 3: sEvil = " Wicked";		break;
					case 4: sEvil = " Corrupt";		break;
					case 5: sEvil = " Hateful";		break;
					case 6: sEvil = " Malevolent";	break;
					case 7: sEvil = " Nefarious";	break;
				}
			}
			else
			{
				sCat = " Wizardry";
				switch ( Utility.RandomMinMax( 0, 7 ) ) 
				{
					case 0: sCat = " Wizardry";		break;
					case 1: sCat = " Spells";		break;
					case 2: sCat = " Conjuration";	break;
					case 3: sCat = " Abjuration";	break;
					case 4: sCat = " Mystiscm";		break;
					case 5: sCat = " Enchanting";	break;
					case 6: sCat = " Magic";		break;
					case 7: sCat = " Sorcery";		break;
				}
			}

			sBook = Server.Misc.RandomThings.GetRandomBelongsTo( "regular" ) + " " + Server.Misc.RandomThings.GetRandomBookType(true) + " of" + sEvil + sCat;
				if ( Utility.RandomMinMax( 1, 4 ) == 1 ){ sBook = Server.Misc.RandomThings.GetRandomBelongsTo( "regular" ) + sEvil + " Spellbook"; }
				if ( sBook.Contains("  ") ){ sBook = sBook.Replace("  ", " "); }

			if ( category == "ink" )
			{
				bag.BagInkLocation = dungeon;
				bag.BagInkWorld = searchLocation;
			}
			else if ( category == "mage" )
			{
				bag.SpellsMageLocation = dungeon;
				bag.SpellsMageWorld = searchLocation;
				bag.SpellsMageItem = cultInfo.ToTitleCase(sBook);
			}
			else if ( category == "research" )
			{
				bag.ResearchLocation = dungeon;
				bag.ResearchWorld = searchLocation;
				bag.ResearchItem = cultInfo.ToTitleCase(sBook);
			}
			else if ( category == "necro" )
			{
				bag.SpellsNecroLocation = dungeon;
				bag.SpellsNecroWorld = searchLocation;
				bag.SpellsNecroItem = cultInfo.ToTitleCase(sBook);
			}
			else
			{
				bag.RuneLocation = dungeon;
				bag.RuneWorld = searchLocation;
			}
		}

		public static void ConsumeScroll( Mobile from, bool skillCheck, int spellIndex, bool automaticConsume, Item scroll )
		{
			if ( Server.Misc.ResearchSettings.ResearchMaterials( from ) != null && !ResearchSettings.BookCaster( from ) )
			{
				if ( skillCheck )
				{
					double RequiredSkill = (double)(Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 8 )));
					int min = (int)(RequiredSkill-20);
					int max = (int)(RequiredSkill+20);
					from.CheckSkill( SkillName.Magery, min, max );
					from.CheckSkill( SkillName.Necromancy, min, max );
					from.CheckSkill( SkillName.Spiritualism, min, max );
					from.CheckSkill( SkillName.Psychology, min, max );
				}

				ResearchBag bag = (ResearchBag)( Server.Misc.ResearchSettings.ResearchMaterials( from ) );

				int high = 125; if ( spellIndex == 51 ){ high = 200; } // CHARM SPELLS HAVE HIGHER SCROLL CRUMBLE RATE

				if ( Utility.RandomMinMax( 0, high ) > AosAttributes.GetValue( from, AosAttribute.LowerRegCost ) || automaticConsume )
				{
					Server.Misc.Research.SetPrepared( bag, spellIndex, -1 );
					int remaining = Server.Misc.Research.GetPrepared( bag, spellIndex );

					if ( remaining == 1 ){ from.SendMessage( "You have 1 scroll left for this spell." ); }
					else if ( remaining < 1 ){ from.SendMessage( "You have no scrolls left for this spell." ); }
					else if ( remaining < 11 ){ from.SendMessage( "You have " + remaining + " scrolls left for this spell." ); }

					if ( from.HasGump( typeof( Server.Items.ResearchBag.ResearchGump ) ) ) {
						from.CloseGump( typeof( Server.Items.ResearchBag.ResearchGump ) );
					}
				}
			}
		}

		public static int RuneIndex( string rune, int type ) ////////////////////////////////////////////////////////////////////////////////////////
		{
			int symbol = 0;

			// type > 0 looks for gump id
			// type = 0 looks for item id

			if ( rune == "an" ){ if ( type > 0 ){ symbol = 11286; } else { symbol = 19513; } }
			else if ( rune == "bet" ){ if ( type > 0 ){ symbol = 11287; } else { symbol = 19514; } }
			else if ( rune == "corp" ){ if ( type > 0 ){ symbol = 11288; } else { symbol = 19515; } }
			else if ( rune == "des" ){ if ( type > 0 ){ symbol = 11289; } else { symbol = 19516; } }
			else if ( rune == "ex" ){ if ( type > 0 ){ symbol = 11290; } else { symbol = 19517; } }
			else if ( rune == "flam" ){ if ( type > 0 ){ symbol = 11291; } else { symbol = 19518; } }
			else if ( rune == "grav" ){ if ( type > 0 ){ symbol = 11292; } else { symbol = 19519; } }
			else if ( rune == "hur" ){ if ( type > 0 ){ symbol = 11293; } else { symbol = 19520; } }
			else if ( rune == "in" ){ if ( type > 0 ){ symbol = 11294; } else { symbol = 19521; } }
			else if ( rune == "jux" ){ if ( type > 0 ){ symbol = 11295; } else { symbol = 19522; } }
			else if ( rune == "kal" ){ if ( type > 0 ){ symbol = 11296; } else { symbol = 19523; } }
			else if ( rune == "lor" ){ if ( type > 0 ){ symbol = 11297; } else { symbol = 19524; } }
			else if ( rune == "mani" ){ if ( type > 0 ){ symbol = 11298; } else { symbol = 19525; } }
			else if ( rune == "nox" ){ if ( type > 0 ){ symbol = 11299; } else { symbol = 19526; } }
			else if ( rune == "ort" ){ if ( type > 0 ){ symbol = 11304; } else { symbol = 19527; } }
			else if ( rune == "por" ){ if ( type > 0 ){ symbol = 11305; } else { symbol = 19528; } }
			else if ( rune == "quas" ){ if ( type > 0 ){ symbol = 11306; } else { symbol = 19529; } }
			else if ( rune == "rel" ){ if ( type > 0 ){ symbol = 11307; } else { symbol = 19530; } }
			else if ( rune == "sanct" ){ if ( type > 0 ){ symbol = 11308; } else { symbol = 19531; } }
			else if ( rune == "tym" ){ if ( type > 0 ){ symbol = 11309; } else { symbol = 19532; } }
			else if ( rune == "uus" ){ if ( type > 0 ){ symbol = 11310; } else { symbol = 19533; } }
			else if ( rune == "vas" ){ if ( type > 0 ){ symbol = 11311; } else { symbol = 19534; } }
			else if ( rune == "wis" ){ if ( type > 0 ){ symbol = 11312; } else { symbol = 19535; } }
			else if ( rune == "xen" ){ if ( type > 0 ){ symbol = 11315; } else { symbol = 19538; } }
			else if ( rune == "ylem" ){ if ( type > 0 ){ symbol = 11313; } else { symbol = 19536; } }
			else if ( rune == "zu" ){ if ( type > 0 ){ symbol = 11314; } else { symbol = 19537; } }

			return symbol;
		}

		public static string RuneName( int index, int type ) ////////////////////////////////////////////////////////////////////////////////////////
		{
			string symbol = "";

			// type > 0 upper
			// type = 0 lower

			if ( index == 1 ){ if ( type > 0 ){ symbol = "An"; } else { symbol = "an"; } }
			else if ( index == 2 ){ if ( type > 0 ){ symbol = "Bet"; } else { symbol = "bet"; } }
			else if ( index == 3 ){ if ( type > 0 ){ symbol = "Corp"; } else { symbol = "corp"; } }
			else if ( index == 4 ){ if ( type > 0 ){ symbol = "Des"; } else { symbol = "des"; } }
			else if ( index == 5 ){ if ( type > 0 ){ symbol = "Ex"; } else { symbol = "ex"; } }
			else if ( index == 6 ){ if ( type > 0 ){ symbol = "Flam"; } else { symbol = "flam"; } }
			else if ( index == 7 ){ if ( type > 0 ){ symbol = "Grav"; } else { symbol = "grav"; } }
			else if ( index == 8 ){ if ( type > 0 ){ symbol = "Hur"; } else { symbol = "hur"; } }
			else if ( index == 9 ){ if ( type > 0 ){ symbol = "In"; } else { symbol = "in"; } }
			else if ( index == 10 ){ if ( type > 0 ){ symbol = "Jux"; } else { symbol = "jux"; } }
			else if ( index == 11 ){ if ( type > 0 ){ symbol = "Kal"; } else { symbol = "kal"; } }
			else if ( index == 12 ){ if ( type > 0 ){ symbol = "Lor"; } else { symbol = "lor"; } }
			else if ( index == 13 ){ if ( type > 0 ){ symbol = "Mani"; } else { symbol = "mani"; } }
			else if ( index == 14 ){ if ( type > 0 ){ symbol = "Nox"; } else { symbol = "nox"; } }
			else if ( index == 15 ){ if ( type > 0 ){ symbol = "Ort"; } else { symbol = "ort"; } }
			else if ( index == 16 ){ if ( type > 0 ){ symbol = "Por"; } else { symbol = "por"; } }
			else if ( index == 17 ){ if ( type > 0 ){ symbol = "Quas"; } else { symbol = "quas"; } }
			else if ( index == 18 ){ if ( type > 0 ){ symbol = "Rel"; } else { symbol = "rel"; } }
			else if ( index == 19 ){ if ( type > 0 ){ symbol = "Sanct"; } else { symbol = "sanct"; } }
			else if ( index == 20 ){ if ( type > 0 ){ symbol = "Tym"; } else { symbol = "tym"; } }
			else if ( index == 21 ){ if ( type > 0 ){ symbol = "Uus"; } else { symbol = "uus"; } }
			else if ( index == 22 ){ if ( type > 0 ){ symbol = "Vas"; } else { symbol = "vas"; } }
			else if ( index == 23 ){ if ( type > 0 ){ symbol = "Wis"; } else { symbol = "wis"; } }
			else if ( index == 24 ){ if ( type > 0 ){ symbol = "Xen"; } else { symbol = "xen"; } }
			else if ( index == 25 ){ if ( type > 0 ){ symbol = "Ylem"; } else { symbol = "ylem"; } }
			else if ( index == 26 ){ if ( type > 0 ){ symbol = "Zu"; } else { symbol = "zu"; } }

			return symbol;
		}

		public static string CapsCast( string words ) ///////////////////////////////////////////////////////////////////////////////////////////////
		{
			TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;
			words = cultInfo.ToTitleCase(words);

			return words;
		}

		public static bool GetRunes( ResearchBag bag, int cube ) ////////////////////////////////////////////////////////////////////////////////////
		{
			string found = bag.RuneFound;

			bool HaveRune = false;

			if ( found.Length > 0 )
			{
				string[] runes = found.Split('#');
				int entry = 1;
				foreach (string rune in runes)
				{
					if ( entry == cube && rune == "1" ){ HaveRune = true; }

					entry++;
				}
			}

			return HaveRune;
		}

		public static void SetRunes( ResearchBag bag, Mobile from ) /////////////////////////////////////////////////////////////////////////////////
		{
			string found = bag.RuneFound;
			string got = "";
			int cube = 0;

			if ( found.Length > 0 )
			{
				string[] runes = found.Split('#');
				int entry = 1;
				bool updated = false;
				foreach (string rune in runes)
				{
					if ( rune == "0" && !updated ){ got = got + "1#"; cube = entry; updated = true; }
					else if ( rune == ""){ got = got + "0#"; }
					else { got = got + rune + "#"; }

					entry++;
				}

				bag.RuneFound = got;
			}

			if ( cube > 25 )
			{
				bag.RuneLocation = "";
				bag.RuneWorld = Land.None;
			}
			else
			{
				FindLocation( from, 0, "rune", bag );
			}

			string runic = RuneName( cube, 1 );
			from.LocalOverheadMessage(MessageType.Emote, 1150, true, "You found the Cube of " + runic + "!");
			Server.Items.QuestSouvenir.GiveReward( from, "Cube of " + runic + "", 0, RuneIndex( RuneName( cube, 0 ), 0 ) );
			from.SendSound( 0x3D );
			LoggingFunctions.LogGeneric( from, "has found the Cube of " + runic + "." );
		}

		public static bool SearchResult( Mobile from, ResearchBag bag ) /////////////////////////////////////////////////////////////////////////////
		{
			bool success = false;

			// CHECK THE RUNE QUEST
			if ( from.Region.Name == bag.RuneLocation )
			{
				SetRunes( bag, from );
				success = true;
			}
			if ( Research.GetRunes( bag, 26 ) )
			{
				if ( from.Region.Name == bag.SpellsMageLocation )
				{
					SetWizardry( bag, from );
					success = true;
				}
				if ( from.Region.Name == bag.SpellsNecroLocation )
				{
					SetNecromancy( bag, from );
					success = true;
				}
				if ( from.Region.Name == bag.BagInkLocation && bag.BagInk < 50000 )
				{
					SetInk( bag, from );
					success = true;
				}
				if ( from.Region.Name == bag.ResearchLocation )
				{
					SetResearch( bag, from );
					success = true;
				}
			}
			return success;
		}

		public static void SetInk( ResearchBag bag, Mobile from ) //////////////////////////////////////////////////////////////////////////////
		{
			if ( bag.BagInk >= 50000 )
			{
				from.SendMessage( "This pack can only hold 50000 bottles of octupus ink so you dump out what you found." );
			}
			else
			{
				int qty = 1;

				if ( from.Skills[SkillName.Alchemy].Value >= Utility.RandomMinMax( 25, 150 ) ){ qty++; }
				if ( from.Skills[SkillName.Cooking].Value >= Utility.RandomMinMax( 25, 150 ) ){ qty++; }
				if ( from.Skills[SkillName.Tasting].Value >= Utility.RandomMinMax( 25, 150 ) ){ qty++; }
				if ( Server.Misc.GetPlayerInfo.LuckyPlayer( from.Luck ) ){ qty++; }

				bag.BagInk = bag.BagInk + qty;
				if ( bag.BagInk > 50000 ){ bag.BagInk = 50000; }

				from.LocalOverheadMessage(MessageType.Emote, 1150, true, "You found some octopus ink!");
				from.SendSound( 0x3D );
			}

			int level = GetMaxCircleResearched( bag )-1; if ( level < 0 ){ level = 0; }
			level = Utility.RandomList(0,level);
			FindLocation( from, level, "ink", bag );
		}

		public static string SpellInformation( int index, int slice ) ///////////////////////////////////////////////////////////////////////////////
		{
			string value = "";
			string circle = "0";
			string mana = "0";
			string skill = "0";
			string name = "";
			string school = "";
			string words = "";
			string reagents = "";
			string description = "";
			string init = "0";
			string much = "false";
			string regs = "false";

			if ( index == 1 ){ circle = "1"; name = "Conjure"; school = "Conjuration"; words = "kal xen"; reagents = "Moon Crystal, Fairy Egg"; mana = "10"; skill = "15";
				init = "610"; regs = "true"; description = "The magic of this spell will conjure a random item of minor significance, but it may be significant dependent on the caster's current motivations. Due to the creative nature of the spell, something needs to be destroyed in turn. That means that the spell scroll will always crumble to dust when cast."; }
			else if ( index == 2 ){ circle = "1"; name = "Death Speak"; school = "Death"; words = "kal wis corp"; reagents = "Grave Dust, Pixie Skull"; mana = "5"; skill = "10";
				init = "614"; description = "This spell allows you to communicate with recently slain supernatural foes. If you can speak to their souls, you have the potential to restore a part of yourself. The restoration goes in order of mana, stamina, and health. The amount restored is dependent on the power of the caster and the fame of the slain creature. The caster may lose some karma if they attempt this."; }
			else if ( index == 3 ){ circle = "1"; name = "Sneak"; school = "Enchanting"; words = "por ex tym"; reagents = "Fairy Egg, Pixie Skull"; mana = "10"; skill = "10";
				init = "648"; description = "Hides the target and allows them to sneak around for 2-8 minutes. They can still be discovered, however, and they cannot sneak well in heavy armor."; }
			else if ( index == 4 ){ circle = "1"; name = "Create Fire"; school = "Sorcery"; words = "in flam ylem"; reagents = "Brimstone, Mandrake Root, Ginseng"; mana = "5"; skill = "15";
				init = "611"; description = "Creates a magical fire at the caster's feet. When anything approaches the fire, they will take 2-20 points of fire damage every couple of seconds. The magical fire can last between 15-200 seconds dependent on the power of the caster."; }
			else if ( index == 5 ){ circle = "1"; name = "Electrical Elemental"; school = "Summoning"; words = "uus grav"; reagents = "Silver Widow, Sulfurous Ash"; mana = "40"; skill = "70";
				init = "655"; description = "Summons an electrial elemental that is controlled by the caster for 8-25 minutes."; }
			else if ( index == 6 ){ circle = "1"; name = "Confusion Blast"; school = "Thaumaturgy"; words = "in quas wis"; reagents = "Mandrake Root, Pixie Skull, Fairy Egg"; mana = "15"; skill = "40";
				init = "609"; description = "Those within the confusion blast could suffer from the effects for 5-20 seconds before they regain their senses."; }
			else if ( index == 7 ){ circle = "1"; name = "See Truth"; school = "Theurgy"; words = "an quas lor"; reagents = "Grave Dust, Sea Salt, Fairy Egg"; mana = "60"; skill = "20";
				init = "645"; description = "This magic is an ancient form of that which gypsies use to see the truth in things. If you cast this spell on a book or parchment that you feel may be falsely written, you may learn the truth of things."; }
			else if ( index == 8 ){ circle = "1"; name = "Icicle"; school = "Wizardry"; words = "ex des bet"; reagents = "Moon Crystal, Pig Iron"; mana = "10"; skill = "15";
				init = "632"; description = "Throws a magical icicle at your enemies that can cause up to 80 points of cold damage."; }

			else if ( index == 9 ){ circle = "2"; name = "Extinguish"; school = "Conjuration"; words = "an flam"; reagents = "Nightshade, Sea Salt, Black Pearl"; mana = "20"; skill = "35";
				init = "624"; description = "Calls forth waves of water that can cause up to 12-125 points of cold and physical damage. It will do twice the amount of damage toward creatures that suffer from flame extinguishing enchanted weapons."; }
			else if ( index == 10 ){ circle = "2"; name = "Rock Flesh"; school = "Death"; words = "rel sanct ylem"; reagents = "Moon Crystal, Garlic, Pig Iron, Black Pearl"; mana = "10"; skill = "15";
				init = "644"; description = "With this spell, the caster gains an innate resistance to damage by transforming the very flesh into a substance as strong as stone for 1-16 minutes of a 90% physical resistance. The caster may lose some karma if they attempt this."; }
			else if ( index == 11 ){ circle = "2"; name = "Mass Might"; school = "Enchanting"; words = "in vas por"; reagents = "Fairy Egg, Butterfly Wings"; mana = "99"; skill = "66";
				init = "638"; description = "Increases the casters strength for up to 8 minutes, and it will also affect the caster's allies that are no more than 10 spaces away."; }
			else if ( index == 12 ){ circle = "2"; name = "Endure Cold"; school = "Sorcery"; words = "vas sanct flam"; reagents = "Spiders Silk, Moon Crystal"; mana = "15"; skill = "20";
				init = "619"; description = "Increases the casters cold resistance for up to 8 minutes, and it will also affect the caster's allies that are no more than 3 spaces away."; }
			else if ( index == 13 ){ circle = "2"; name = "Weed Elemental"; school = "Summoning"; words = "uus kal"; reagents = "Red Lotus, Nightshade"; mana = "40"; skill = "70";
				init = "660"; description = "Summons a weed elemental that is controlled by the caster for 8-25 minutes."; }
			else if ( index == 14 ){ circle = "2"; name = "Spawn Creatures"; school = "Thaumaturgy"; words = "vas kal xen"; reagents = "Ginseng, Grave Dust, Pixie Skull"; mana = "20"; skill = "45";
				init = "652"; description = "Summons random creatures that are controlled by the caster for 8-25 minutes."; }
			else if ( index == 15 ){ circle = "2"; name = "Healing Touch"; school = "Theurgy"; words = "in mani"; reagents = "Garlic, Ginseng, Fairy Egg, Pixie Skull"; mana = "15"; skill = "30";
				init = "631"; description = "Heals the one touched, restoring 12-125 hit points. You need to be close to your target, however."; }
			else if ( index == 16 ){ circle = "2"; name = "Snow Ball"; school = "Wizardry"; words = "ex des in"; reagents = "Black Pearl, Moon Crystal"; mana = "10"; skill = "10";
				init = "649"; description = "Throws a magical snow ball toward your enemy, which can cause 2-80 points of cold damage."; }

			else if ( index == 17 ){ circle = "3"; name = "Clone"; school = "Conjuration"; words = "in quas xen"; reagents = "Gargoyle Ear, Fairy Egg"; mana = "25"; skill = "45";
				init = "607"; description = "Creates a clone of the caster that will appear and distract your enemies while you remain hidden from view."; }
			else if ( index == 18 ){ circle = "3"; name = "Grant Peace"; school = "Death"; words = "in vas corp"; reagents = "Pixie Skull, Grave Dust"; mana = "35"; skill = "75";
				init = "629"; description = "This spell can grant an undead creature the true and final death. If they cannot be laid to rest, then they may take 50-200 points of energy damage."; }
			else if ( index == 19 ){ circle = "3"; name = "Sleep"; school = "Enchanting"; words = "in zu tym"; reagents = "Fairy Egg, Mandrake Root, Sea Salt"; mana = "15"; skill = "40";
				init = "646"; description = "This spell can put a foe to sleep from 10-60 seconds, but taking actions against them will likely wake them up from their slumber. This spell cannot affect supernatural creatures, constructs, golems, or elementals."; }
			else if ( index == 20 ){ circle = "3"; name = "Endure Heat"; school = "Sorcery"; words = "sanct flam"; reagents = "Swamp Berries, Gargoyle Ear"; mana = "15"; skill = "20";
				init = "620"; description = "Increases the casters fire resistance for up to 8 minutes, and it will also affect the caster's allies that are no more than 3 spaces away."; }
			else if ( index == 21 ){ circle = "3"; name = "Ice Elemental"; school = "Summoning"; words = "uus ex des"; reagents = "Sea Salt, Moon Crystal"; mana = "40"; skill = "70";
				init = "657"; description = "Summons an ice elemental that is controlled by the caster for 8-25 minutes."; }
			else if ( index == 22 ){ circle = "3"; name = "Ethereal Travel"; school = "Thaumaturgy"; words = "ort grav por"; reagents = "Black Pearl, Gargoyle Ear, Red Lotus"; mana = "20"; skill = "35";
				init = "622"; description = "Caster is transported to the location marked on the recall rune. One can use this spell on a ship key to travel to a boat's location if the boat is not dry docked. The caster will appear at the destination in a hidden state."; }
			else if ( index == 23 ){ circle = "3"; name = "Wizard Eye"; school = "Theurgy"; words = "por ort wis"; reagents = "Eye of Toad, Silver Widow, Gargoyle Ear, Black Pearl"; mana = "30"; skill = "50";
				init = "663"; description = "This spell allows the caster to identify items that have origins that are unknown to others."; }
			else if ( index == 24 ){ circle = "3"; name = "Frost Field"; school = "Wizardry"; words = "in ex des"; reagents = "Moon Crystal, Eye of Toad, Spiders Silk"; mana = "15"; skill = "30";
				init = "627"; description = "Creates a wall of frost that can cause 2-16 points of cold damage per second to anyone within it."; }

			else if ( index == 25 ){ circle = "4"; name = "Create Gold"; school = "Conjuration"; words = "rel ylem"; much = "true"; reagents = "Unicorn Horn, Sea Salt, Ginseng, Golden Serpent Venom"; mana = "35"; skill = "55";
				init = "612"; regs = "true"; description = "Originally created by the fiendish imp Rumpelstiltskin, this spell allows the caster to turn armor or weapons into gold. They can also transmute varying ingots into golden ingots, as well as turn coins into golden coins. Coins will often be destroyed during the transmutation process, but the better the skill of the archmage the less that is lost. This spell is powerful enough that the scroll will always crumble to dust when cast."; }
			else if ( index == 26 ){ circle = "4"; name = "Animate Bones"; school = "Death"; words = "kal corp xen"; reagents = "Bat Wing, Grave Dust, Pixie Skull"; mana = "40"; skill = "70";
				init = "653"; description = "Animates the bones of a mighty skeletal warrior that will fight for the caster for 8-25 minutes. The caster may lose some karma if they attempt this."; }
			else if ( index == 27 ){ circle = "4"; name = "Cause Fear"; school = "Enchanting"; words = "quas wis tym"; reagents = "Bloodmoss, Daemon Blood, Red Lotus"; mana = "35"; skill = "45";
				init = "605"; description = "The caster can cause fear in a foe, causing them to flee from you for 10-60 seconds. This spell does not work on golems or constructs."; }
			else if ( index == 28 ){ circle = "4"; name = "Ignite"; school = "Sorcery"; words = "in flam"; reagents = "Brimstone, Sulfurous Ash, Black Pearl"; mana = "30"; skill = "40";
				init = "633"; description = "Calls forth columns of fire that can cause up to 15-125 points of fire and physical damage. It will do twice the amount of damage toward creatures that suffer from watery grave or neptune's bane enchanted weapons."; }
			else if ( index == 29 ){ circle = "4"; name = "Mud Elemental"; school = "Summoning"; words = "uus des ylem"; reagents = "Pig Iron, Mandrake Root"; mana = "40"; skill = "70";
				init = "658"; description = "Summons a mud elemental that is controlled by the caster for 8-25 minutes."; }
			else if ( index == 30 ){ circle = "4"; name = "Banish Daemon"; school = "Thaumaturgy"; words = "an flam corp xen"; reagents = "Gargoyle Ear, Demon Claw"; mana = "40"; skill = "80";
				init = "603"; description = "This spell can send a demonic creature back to the plane of hell from which they spawned. If they cannot be banished, then they may take 50-200 points of energy damage."; }
			else if ( index == 31 ){ circle = "4"; name = "Fade from Sight"; school = "Theurgy"; words = "quas an lor"; reagents = "Red Lotus, Nox Crystal"; mana = "15"; skill = "50";
				init = "625"; description = "This spell will cause the caster to fade from the view of others, while at the same time moving them to a different location."; }
			else if ( index == 32 ){ circle = "4"; name = "Gas Cloud"; school = "Wizardry"; words = "in hur grav"; reagents = "Nightshade, Silver Widow, Swamp Berries, Silver Serpent Venom"; mana = "25"; skill = "45";
				init = "621"; description = "Creates a gaseous cloud of poison mist that will consume nearby enemies in the venomous vapor for 1-3 minutes."; }

			else if ( index == 33 ){ circle = "5"; name = "Swarm"; school = "Conjuration"; words = "kal bet xen"; reagents = "Beetle Shell, Silver Widow, Fairy Egg"; mana = "15"; skill = "40";
				init = "661"; description = "Unleashes a swarm of insects at your enemies for 1-3 minutes."; }
			else if ( index == 34 ){ circle = "5"; name = "Mask of Death"; school = "Death"; words = "quas corp"; reagents = "Lich Dust, Grave Dust, Pixie Skull"; mana = "70"; skill = "90";
				init = "636"; description = "Creates a mask for the caster that when worn the undead will see you as one of their own and ignore them. The mask remains on this plane of existence for 6-15 minutes before it returns to the realm from which it came. If you attack an undead creature while wearing the mask, it will vanish and you will have to face your foe. The caster may lose some karma if they attempt this."; }
			else if ( index == 35 ){ circle = "5"; name = "Enchant"; school = "Enchanting"; words = "ort ylem tym"; reagents = "Pixie Skull, Fairy Egg, Dragon Tooth, Moon Crystal"; mana = "45"; skill = "75";
				init = "618"; description = "This can greatly increase a weapon's damage by enchanting it for 15-60 minutes. A power orb is summoned and placed in the caster's pack that gives the enchantment strength until the spell wears off."; }
			else if ( index == 36 ){ circle = "5"; name = "Flame Bolt"; school = "Sorcery"; words = "in ort flam"; reagents = "Brimstone, Sulfurous Ash"; mana = "15"; skill = "30";
				init = "626"; description = "Throws a magical bolt of flame at your enemies that can cause 12-100 points of fire damage."; }
			else if ( index == 37 ){ circle = "5"; name = "Gem Elemental"; school = "Summoning"; words = "uus sanct"; reagents = "Moon Crystal, Pig Iron"; mana = "50"; skill = "80";
				init = "656"; description = "Summons a gem rock elemental that is controlled by the caster for 8-25 minutes."; }
			else if ( index == 38 ){ circle = "5"; name = "Call Destruction"; school = "Thaumaturgy"; words = "kal vas grav por"; much = "true"; reagents = "Bat Wing, Black Pearl, Brimstone, Pig Iron"; mana = "25"; skill = "40";
				init = "604"; regs = "true"; description = "Damages nearby enemies with an destructive force that causes mostly physical harm and some damage from energy. If you hit more than one enemy, the damage amount is doubled and divided amongst each enemy evenly. The damage dealt is between 8-50 points, with the addition of half the available hit points of the caster. After the destruction is unleashed, the caster will lose half of their remaining hit points."; }
			else if ( index == 39 ){ circle = "5"; name = "Divination"; school = "Theurgy"; words = "in wis tym"; reagents = "Ginseng, Daemon Blood, Eye of Toad"; mana = "30"; skill = "50";
				init = "617"; description = "One can cast this spell on most creatures and learn things about them that are not commonly know by others."; }
			else if ( index == 40 ){ circle = "5"; name = "Frost Strike"; school = "Wizardry"; words = "ex des corp"; reagents = "Moon Crystal, Grave Dust"; mana = "40"; skill = "67";
				init = "628"; description = "Damages an enemy with a column of frost crystals that causes 35-125 points of cold damage."; }

			else if ( index == 41 ){ circle = "6"; name = "Magic Steed"; school = "Conjuration"; words = "in xen tym"; reagents = "Bat Wing, Butterfly Wings"; mana = "30"; skill = "50";
				init = "635"; description = "Lures a horse to the caster's location where it can be ridden for 8-25 minutes."; }
			else if ( index == 42 ){ circle = "6"; name = "Create Golem"; school = "Death"; words = "in ort ylem xen"; reagents = "Pixie Skull, Grave Dust, Daemon Blood"; mana = "40"; skill = "70";
				init = "613"; description = "Creates a mighty flesh golem that will follow the commands of the cast for 8-25 minutes. The caster may lose some karma if they attempt this."; }
			else if ( index == 43 ){ circle = "6"; name = "Sleep Field"; school = "Enchanting"; words = "in zu grav tym"; reagents = "Fairy Egg, Mandrake Root, Sea Salt, Grave Dust"; mana = "30"; skill = "60";
				init = "647"; description = "This spell will create a field that will put entering foes to sleep for 10-60 seconds, but taking actions against them will likely wake them up from their slumber. This spell cannot affect supernatural creatures, constructs, golems, or elementals."; }
			else if ( index == 44 ){ circle = "6"; name = "Conflagration"; school = "Sorcery"; words = "kal vas flam corp xen"; reagents = "Sulfurous Ash, Brimstone, Gargoyle Ear"; mana = "20"; skill = "35";
				init = "608"; description = "Creates a huge inferno that can cause 2-16 points of fire damage per second to anyone within it."; }
			else if ( index == 45 ){ circle = "6"; name = "Acid Elemental"; school = "Summoning"; words = "uus corp"; reagents = "Swamp Berries, Nox Crystal, Beetle Shell, Eye of Toad"; mana = "50"; skill = "82";
				init = "650"; description = "Summons an acid elemental that is controlled by the caster for 8-25 minutes."; }
			else if ( index == 46 ){ circle = "6"; name = "Meteor Shower"; school = "Thaumaturgy"; words = "kal des flam ylem"; reagents = "Blood Moss, Brimstone, Mandrake Root, Black Pearl"; mana = "40"; skill = "70";
				init = "640"; description = "Damages nearby enemies with falling meteorites that hit for physical damage (with some fire and energy damage) split between the enemies. If you only hit one enemy, the enemy takes between 35-125 points of damage, but if you hit more than one enemy, the damage amount is doubled and divided amongst each enemy evenly."; }
			else if ( index == 47 ){ circle = "6"; name = "Intervention"; school = "Theurgy"; words = "in sanct an jux"; reagents = "Moon Crystal, Brimstone, Nightshade, Fairy Egg"; mana = "25"; skill = "50";
				init = "634"; description = "This is one of those rare spells that call upon the favor of the gods. When the spell is invoked, the caster is empowered with 10-50 additional points in their resistances. They are also protected from magery spell attack damage for 25-125 points. The effects last between 10-50 minutes."; }
			else if ( index == 48 ){ circle = "6"; name = "Hail Storm"; school = "Wizardry"; words = "vas ex des"; reagents = "Fairy Egg, Moon Crystal"; mana = "25"; skill = "55";
				init = "630"; description = "Creates a storm of ice around an enemy, causing 28-125 points of cold damage."; }

			else if ( index == 49 ){ circle = "7"; name = "Aerial Servant"; school = "Conjuration"; words = "kal ort xen"; reagents = "Butterfly Wings, Gargoyle Ear, Pegasus Feather"; mana = "50"; skill = "80";
				init = "600"; description = "Calls upon a mystical servant from the ethereal plane to aid the caster by carrying items for 1-3 hours."; }
			else if ( index == 50 ){ circle = "7"; name = "Open Ground"; school = "Death"; words = "des por ylem"; much = "true"; reagents = "Brimstone, Grave Dust, Pig Iron, Mandrake Root"; mana = "65"; skill = "85";
				init = "641"; regs = "true"; description = "Opens a huge hole in the ground that can cause anyone that goes near it to fall in. Creatures may fall to their deaths and never be seen again, while advenuterers will fall into the huge chasm below. If an adventurer has comrades, familiars, summons, or tamed beasts that stumble into the chasm, then the adventurer will fall in after them. This spell is powerful enough that the scroll will always crumble to dust when cast. The caster may lose some karma if they attempt this."; }
			else if ( index == 51 ){ circle = "7"; name = "Charm"; school = "Enchanting"; words = "an xen ex"; reagents = "Enchanted Seaweed, Fairy Egg, Pixie Skull"; mana = "60"; skill = "82";
				init = "606"; description = "This spell can charm either aggressive monsters or those of animialistic natures. When charmed, their evil or neutral tendencies are wiped from their mind and they will attack evil creatures. This charm will only last for up to a minute before they come to their senses."; }
			else if ( index == 52 ){ circle = "7"; name = "Explosion"; school = "Sorcery"; words = "vas ort flam"; reagents = "Fairy Egg, Brimstone, Sulfurous Ash"; mana = "30"; skill = "60";
				init = "623"; description = "Damages nearby enemies with an explosive force consisting of mostly heat and some physical damage. If you only hit one enemy, the enemy takes between 35-125 points of damage, but if you hit more than one enemy, the damage amount is doubled and divided amongst each enemy evenly."; }
			else if ( index == 53 ){ circle = "7"; name = "Poison Elemental"; school = "Summoning"; words = "uus nox"; reagents = "Nightshade, Nox Crystal, Silver Serpent Venom, Silver Widow"; mana = "50"; skill = "86";
				init = "659"; description = "Summons a poison elemental that is controlled by the caster for 8-25 minutes."; }
			else if ( index == 54 ){ circle = "7"; name = "Invoke Devil"; school = "Thaumaturgy"; words = "uus flam corp xen"; reagents = "Daemon Blood, Eye of Toad, Brimstone, Demon Claw"; mana = "60"; skill = "95";
				init = "654"; description = "Calls forth a devil from the realms of hell to serve the caster for 2-8 minutes. The caster may lose some karma if they attempt this."; }
			else if ( index == 55 ){ circle = "7"; name = "Air Walk"; school = "Theurgy"; words = "vas hur por"; reagents = "Spiders Silk, Pixie Skull, Butterfly Wings"; mana = "55"; skill = "65";
				init = "601"; description = "This spell creates an apple high cushion of air under the caster's feet for 20-80 minutes. When moving on this cushion of air, the caster can avoid almost any floor trap even if they are hidden. It does not help the caster avoid stone face traps or exploding mushrooms, nor does it avoid traps placed on containers. Any harmful liquids on a floor can also be avoided."; }
			else if ( index == 56 ){ circle = "7"; name = "Avalanche"; school = "Wizardry"; words = "ex vas des"; reagents = "Blood Moss, Spiders Silk, Mandrake Root, Moon Crystal"; mana = "40"; skill = "70";
				init = "602"; description = "Damages nearby enemies with falling ice and snow that hit for cold damage split between the enemies. If you only hit one enemy, the enemy takes between 35-125 points of damage, but if you hit more than one enemy, the damage amount is doubled and divided amongst each enemy evenly."; }

			else if ( index == 57 ){ circle = "8"; name = "Death Vortex"; school = "Conjuration"; words = "vas corp hur"; reagents = "Daemon Blood, Eye of Toad, Unicorn Horn"; mana = "60"; skill = "90";
				init = "615"; description = "Creates a swirling mass of black death that spreads disease and unleashes electrical force on nearby enemies for 1-3 minutes."; }
			else if ( index == 58 ){ circle = "8"; name = "Withstand Death"; school = "Death"; words = "vas an corp"; much = "true"; reagents = "Phoenix Feather, Demigod Blood, Enchanted Seaweed, Ghostly Dust"; mana = "70"; skill = "90";
				init = "662"; regs = "true"; description = "This spell was lost to the ages, but legends tell of King Wolfgang learning the secrets of this spell and creating the gem of immortality. The caster, if they can find all of the reagents needed, should be able to create a gem almost similar to that. If one is holding the jewel that is created, and they meet an untimely end, they will be restored to full health instead as the jewel vanishes from this plane of existence. You will need a sapphire to imbue the magic in for this spell to work. This spell is powerful enough that the scroll will always crumble to dust when cast."; }
			else if ( index == 59 ){ circle = "8"; name = "Mass Sleep"; school = "Enchanting"; words = "vas zu tym"; reagents = "Fairy Egg, Mandrake Root, Enchanted Seaweed, Lich Dust"; mana = "50"; skill = "85";
				init = "639"; regs = "true"; description = "This spell will cause nearby foes to sleep for 10-60 seconds, but taking actions against them will likely wake them up from their slumber. This spell cannot affect supernatural creatures, constructs, golems, or elementals. This spell is powerful enough that the scroll will always crumble to dust when cast."; }
			else if ( index == 60 ){ circle = "8"; name = "Ring of Fire"; school = "Sorcery"; words = "in flam an por"; reagents = "Brimstone, Pig Iron, Daemon Blood"; mana = "55"; skill = "85";
				init = "643"; description = "This spell creates a ring of fire at a chosen location, where demonic creatures cannot pass. Any demonic creature that touches the flames will be unable to cast spells, but they may still perform any special attacks they may have. The ring can last up to 50 seconds before extinguishing itself, and there is a chance that the demon can traverse the flaming circle if cast on uneven terrain."; }
			else if ( index == 61 ){ circle = "8"; name = "Blood Elemental"; school = "Summoning"; words = "uus mani"; reagents = "Black Pearl, Bloodmoss, Dragon Blood, Daemon Blood"; mana = "50"; skill = "90";
				init = "651"; description = "Summons a blood elemental that is controlled by the caster for 8-25 minutes."; }
			else if ( index == 62 ){ circle = "8"; name = "Armageddon"; school = "Thaumaturgy"; words = "in vas ort corp"; much = "true"; reagents = "Butterfly Wings, Black Pearl, Demigod Blood, Demon Claw"; mana = "80"; skill = "100";
				init = "616"; description = "Over 700,000 years ago, the Xorinite Wisp gave the original version of this spell to a mage named Zog who was foolish enough to cast it, and all life was destroyed. What Zog may have done to enhance the spell is unknown, but this one is a much more minor version of that spell. Once it is cast, it destroys all creatures (often destroying their corpses) in a 20 space radius. It also kills the one casting it. Only the biggest fool, full of madness, would dare to cast this spell. The caster may lose some karma if they attempt this and the spell is powerful enough that the scroll will always crumble to dust when cast."; }
			else if ( index == 63 ){ circle = "8"; name = "Restoration"; school = "Theurgy"; words = "vas in mani"; reagents = "Butterfly Wings, Garlic, Enchanted Seaweed"; mana = "50"; skill = "80";
				init = "642"; description = "This spell can restore 12-125 points in health, stamina, and mana."; }
			else if ( index == 64 ){ circle = "8"; name = "Mass Death"; school = "Wizardry"; words = "vas corp"; much = "true"; reagents = "Pixie Skull, Bat Wing, Dragon Tooth"; mana = "55"; skill = "90";
				init = "637"; regs = "true"; description = "Damages nearby enemies with deathly magics summoned from the void. If you hit more than one enemy, the damage amount is doubled and divided amongst each enemy evenly. The damage dealt is between 45-125 points, with the addition of the available hit points of the caster. After the mass death is unleashed, the caster will have a single hit point remaining. The caster may lose some karma if they attempt this. This spell is powerful enough that the scroll will always crumble to dust when cast."; }

			if ( slice == 1 ){ value = circle; }
			else if ( slice == 2 ){ value = name; }
			else if ( slice == 3 ){ value = school; }
			else if ( slice == 4 ){ value = words; }
			else if ( slice == 5 ){ value = reagents; }
			else if ( slice == 6 ){ value = description; }
			else if ( slice == 7 ){ value = mana; }
			else if ( slice == 8 ){ value = skill; }
			else if ( slice == 13 ){ value = much; }
			else if ( slice == 14 ){ value = regs; }
			else if ( slice == 9 )
			{
				value = "11270";
				if ( school == "Death" ){ value = "11269"; }
				else if ( school == "Enchanting" ){ value = "11276"; }
				else if ( school == "Sorcery" ){ value = "11275"; }
				else if ( school == "Summoning" ){ value = "11272"; }
				else if ( school == "Thaumaturgy" ){ value = "11274"; }
				else if ( school == "Theurgy" ){ value = "11273"; }
				else if ( school == "Wizardry" ){ value = "11271"; }
			}
			else if ( slice == 10 )
			{
				value = "11260";
				if ( school == "Death" ){ value = "11259"; }
				else if ( school == "Enchanting" ){ value = "11268"; }
				else if ( school == "Sorcery" ){ value = "11267"; }
				else if ( school == "Summoning" ){ value = "11262"; }
				else if ( school == "Thaumaturgy" ){ value = "11266"; }
				else if ( school == "Theurgy" ){ value = "11263"; }
				else if ( school == "Wizardry" ){ value = "11261"; }
			}
			else if ( slice == 11 )
			{
				int mod = Int32.Parse( circle );
				int mul = 0;
				if ( school == "Death" ){ mul = 8; }
				else if ( school == "Enchanting" ){ mul = 16; }
				else if ( school == "Sorcery" ){ mul = 24; }
				else if ( school == "Summoning" ){ mul = 32; }
				else if ( school == "Thaumaturgy" ){ mul = 40; }
				else if ( school == "Theurgy" ){ mul = 48; }
				else if ( school == "Wizardry" ){ mul = 56; }
				value = (11194+(mod+mul)).ToString();

				if ( index == 45 ){ value = "11233"; }
				else if ( index == 37 ){ value = "11232"; }
				else if ( index == 53 ){ value = "11231"; }
			}
			else { value = init; }

			return value;
		}

		public static string ScrollInformation( int index, int slice ) //////////////////////////////////////////////////////////////////////////////
		{
			string value = "";
			string circle = "0";
			string name = "";
			string reagents = "";
			string mana = "0";
			string skill = "0";
			string cliloc = (1061289+index).ToString();

			if ( index == 1 ){ name = "Clumsy"; circle = "1"; reagents = "Garlic, Spiders Silk, Sulfurous Ash"; mana = "8"; skill = "30"; }
			else if ( index == 2 ){ name = "Create Food"; circle = "1"; reagents = "Bloodmoss, Nightshade"; mana = "8"; skill = "30"; }
			else if ( index == 3 ){ name = "Feeblemind"; circle = "1"; reagents = "Garlic, Ginseng, Mandrake Root"; mana = "8"; skill = "30"; }
			else if ( index == 4 ){ name = "Heal"; circle = "1"; reagents = "Nightshade, Ginseng"; mana = "8"; skill = "30"; }
			else if ( index == 5 ){ name = "Magic Arrow"; circle = "1"; reagents = "Garlic, Ginseng, Spiders Silk"; mana = "8"; skill = "30"; }
			else if ( index == 6 ){ name = "Night Sight"; circle = "1"; reagents = "Sulfurous Ash"; mana = "8"; skill = "30"; }
			else if ( index == 7 ){ name = "Reactive Armor"; circle = "1"; reagents = "Spiders Silk, Sulfurous Ash"; mana = "8"; skill = "30"; }
			else if ( index == 8 ){ name = "Weaken"; circle = "1"; reagents = "Garlic, Nightshade"; mana = "8"; skill = "30"; }
			else if ( index == 9 ){ name = "Agility"; circle = "2"; reagents = "Bloodmoss, Mandrake Root"; mana = "16"; skill = "40"; }
			else if ( index == 10 ){ name = "Cunning"; circle = "2"; reagents = "Nightshade, Mandrake Root"; mana = "16"; skill = "40"; }
			else if ( index == 11 ){ name = "Cure"; circle = "2"; reagents = "Garlic, Ginseng"; mana = "16"; skill = "40"; }
			else if ( index == 12 ){ name = "Harm"; circle = "2"; reagents = "Nightshade, Spiders Silk"; mana = "16"; skill = "40"; }
			else if ( index == 13 ){ name = "Magic Trap"; circle = "2"; reagents = "Garlic, Spiders Silk, Sulfurous Ash"; mana = "16"; skill = "40"; }
			else if ( index == 14 ){ name = "Remove Trap"; circle = "2"; reagents = "Bloodmoss, Sulfurous Ash"; mana = "16"; skill = "40"; }
			else if ( index == 15 ){ name = "Protection"; circle = "2"; reagents = "Garlic, Ginseng, Sulfurous Ash"; mana = "16"; skill = "40"; }
			else if ( index == 16 ){ name = "Strength"; circle = "2"; reagents = "Nightshade, Mandrake Root"; mana = "16"; skill = "40"; }
			else if ( index == 17 ){ name = "Bless"; circle = "3"; reagents = "Garlic, Mandrake Root"; mana = "24"; skill = "50"; }
			else if ( index == 18 ){ name = "Fireball"; circle = "3"; reagents = "Black Pearl"; mana = "24"; skill = "50"; }
			else if ( index == 19 ){ name = "Magic Lock"; circle = "3"; reagents = "Bloodmoss, Garlic, Sulfurous Ash"; mana = "24"; skill = "50"; }
			else if ( index == 20 ){ name = "Poison"; circle = "3"; reagents = "Nightshade"; mana = "24"; skill = "50"; }
			else if ( index == 21 ){ name = "Telekinesis"; circle = "3"; reagents = "Bloodmoss, Mandrake Root"; mana = "24"; skill = "50"; }
			else if ( index == 22 ){ name = "Teleport"; circle = "3"; reagents = "Bloodmoss, Mandrake Root"; mana = "24"; skill = "50"; }
			else if ( index == 23 ){ name = "Unlock"; circle = "3"; reagents = "Bloodmoss, Sulfurous Ash"; mana = "24"; skill = "50"; }
			else if ( index == 24 ){ name = "Wall of Stone"; circle = "3"; reagents = "Bloodmoss, Garlic"; mana = "24"; skill = "50"; }
			else if ( index == 25 ){ name = "Arch Cure"; circle = "4"; reagents = "Garlic, Ginseng, Mandrake Root"; mana = "32"; skill = "60"; }
			else if ( index == 26 ){ name = "Arch Protection"; circle = "4"; reagents = "Garlic, Ginseng, Mandrake Root, Sulfurous Ash"; mana = "32"; skill = "60"; }
			else if ( index == 27 ){ name = "Curse"; circle = "4"; reagents = "Garlic, Nightshade, Sulfurous Ash"; mana = "32"; skill = "60"; }
			else if ( index == 28 ){ name = "Fire Field"; circle = "4"; reagents = "Black Pearl, Spiders Silk, Sulfurous Ash"; mana = "32"; skill = "60"; }
			else if ( index == 29 ){ name = "Greater Heal"; circle = "4"; reagents = "Garlic, Spiders Silk, Mandrake Root, Ginseng"; mana = "32"; skill = "60"; }
			else if ( index == 30 ){ name = "Lightning"; circle = "4"; reagents = "Mandrake Root, Sulfurous Ash"; mana = "32"; skill = "60"; }
			else if ( index == 31 ){ name = "Mana Drain"; circle = "4"; reagents = "Black Pearl, Spiders Silk, Mandrake Root"; mana = "32"; skill = "60"; }
			else if ( index == 32 ){ name = "Recall"; circle = "4"; reagents = "Black Pearl, Bloodmoss, Mandrake Root"; mana = "32"; skill = "60"; }
			else if ( index == 33 ){ name = "Blade Spirits"; circle = "5"; reagents = "Black Pearl, Nightshade, Mandrake Root"; mana = "40"; skill = "70"; }
			else if ( index == 34 ){ name = "Dispel Field"; circle = "5"; reagents = "Black Pearl, Garlic, Spiders Silk, Sulfurous Ash"; mana = "40"; skill = "70"; }
			else if ( index == 35 ){ name = "Incognito"; circle = "5"; reagents = "Bloodmoss, Garlic, Nightshade"; mana = "40"; skill = "70"; }
			else if ( index == 36 ){ name = "Magic Reflect"; circle = "5"; reagents = "Garlic, Mandrake Root, Spiders Silk"; mana = "40"; skill = "70"; }
			else if ( index == 37 ){ name = "Mind Blast"; circle = "5"; reagents = "Black Pearl, Mandrake Root, Nightshade, Sulfurous Ash"; mana = "40"; skill = "70"; }
			else if ( index == 38 ){ name = "Paralyze"; circle = "5"; reagents = "Garlic, Mandrake Root, Spiders Silk"; mana = "40"; skill = "70"; }
			else if ( index == 39 ){ name = "Poison Field"; circle = "5"; reagents = "Black Pearl, Nightshade, Spiders Silk"; mana = "40"; skill = "70"; }
			else if ( index == 40 ){ name = "Summon Creature"; circle = "5"; reagents = "Bloodmoss, Mandrake Root, Spiders Silk"; mana = "40"; skill = "70"; }
			else if ( index == 41 ){ name = "Dispel"; circle = "6"; reagents = "Garlic, Mandrake Root, Sulfurous Ash"; mana = "48"; skill = "80"; }
			else if ( index == 42 ){ name = "Energy Bolt"; circle = "6"; reagents = "Black Pearl, Nightshade"; mana = "48"; skill = "80"; }
			else if ( index == 43 ){ name = "Explosion"; circle = "6"; reagents = "Bloodmoss, Mandrake Root"; mana = "48"; skill = "80"; }
			else if ( index == 44 ){ name = "Invisibility"; circle = "6"; reagents = "Bloodmoss, Nightshade"; mana = "48"; skill = "80"; }
			else if ( index == 45 ){ name = "Mark"; circle = "6"; reagents = "Bloodmoss, Black Pearl, Mandrake Root"; mana = "48"; skill = "80"; }
			else if ( index == 46 ){ name = "Mass Curse"; circle = "6"; reagents = "Garlic, Mandrake Root, Nightshade, Sulfurous Ash"; mana = "48"; skill = "80"; }
			else if ( index == 47 ){ name = "Paralyze Field"; circle = "6"; reagents = "Black Pearl, Ginseng, Spiders Silk"; mana = "48"; skill = "80"; }
			else if ( index == 48 ){ name = "Reveal"; circle = "6"; reagents = "Bloodmoss, Sulfurous Ash"; mana = "48"; skill = "80"; }
			else if ( index == 49 ){ name = "Chain Lightning"; circle = "7"; reagents = "Black Pearl, Bloodmoss, Mandrake Root, Sulfurous Ash"; mana = "56"; skill = "90"; }
			else if ( index == 50 ){ name = "Energy Field"; circle = "7"; reagents = "Black Pearl, Mandrake Root, Spiders Silk, Sulfurous Ash"; mana = "56"; skill = "90"; }
			else if ( index == 51 ){ name = "Flame Strike"; circle = "7"; reagents = "Spiders Silk, Sulfurous Ash"; mana = "56"; skill = "90"; }
			else if ( index == 52 ){ name = "Gate Travel"; circle = "7"; reagents = "Black Pearl, Mandrake Root, Sulfurous Ash"; mana = "56"; skill = "90"; }
			else if ( index == 53 ){ name = "Mana Vampire"; circle = "7"; reagents = "Black Pearl, Bloodmoss, Mandrake Root, Spiders Silk"; mana = "56"; skill = "90"; }
			else if ( index == 54 ){ name = "Mass Dispel"; circle = "7"; reagents = "Black Pearl, Garlic, Mandrake Root, Sulfurous Ash"; mana = "56"; skill = "90"; }
			else if ( index == 55 ){ name = "Meteor Swarm"; circle = "7"; reagents = "Bloodmoss, Mandrake Root, Sulfurous Ash, Spiders Silk"; mana = "56"; skill = "90"; }
			else if ( index == 56 ){ name = "Polymorph"; circle = "7"; reagents = "Bloodmoss, Mandrake Root, Spiders Silk"; mana = "56"; skill = "90"; }
			else if ( index == 57 ){ name = "Earthquake"; circle = "8"; reagents = "Bloodmoss, Mandrake Root, Ginseng, Sulfurous Ash"; mana = "64"; skill = "100"; }
			else if ( index == 58 ){ name = "Energy Vortex"; circle = "8"; reagents = "Black Pearl, Bloodmoss, Mandrake Root, Nightshade"; mana = "64"; skill = "100"; }
			else if ( index == 59 ){ cliloc = "1061348"; name = "Resurrection"; circle = "8"; reagents = "Bloodmoss, Garlic, Ginseng"; mana = "64"; skill = "100"; }
			else if ( index == 60 ){ cliloc = "1061349"; name = "Air Elemental"; circle = "8"; reagents = "Bloodmoss, Mandrake Root, Spiders Silk"; mana = "64"; skill = "100"; }
			else if ( index == 61 ){ cliloc = "1061350"; name = "Summon Daemon"; circle = "8"; reagents = "Bloodmoss, Mandrake Root, Spiders Silk, Sulfurous Ash"; mana = "64"; skill = "100"; }
			else if ( index == 62 ){ cliloc = "1061351"; name = "Earth Elemental"; circle = "8"; reagents = "Bloodmoss, Mandrake Root, Spiders Silk"; mana = "64"; skill = "100"; }
			else if ( index == 63 ){ cliloc = "1061352"; name = "Fire Elemental"; circle = "8"; reagents = "Bloodmoss, Mandrake Root, Spiders Silk, Sulfurous Ash"; mana = "64"; skill = "100"; }
			else if ( index == 64 ){ cliloc = "1061353"; name = "Water Elemental"; circle = "8"; reagents = "Bloodmoss, Mandrake Root, Spiders Silk"; mana = "64"; skill = "100"; }
			else if ( index == 65 ){ cliloc = "1061391"; name = "Blood Oath"; circle = "1"; reagents = "Daemon Blood"; mana = "20"; skill = "30"; }
			else if ( index == 66 ){ cliloc = "1061392"; name = "Corpse Skin"; circle = "1"; reagents = "Bat Wing, Grave Dust"; mana = "17"; skill = "30"; }
			else if ( index == 67 ){ cliloc = "1061393"; name = "Curse Weapon"; circle = "1"; reagents = "Pig Iron"; mana = "11"; skill = "30"; }
			else if ( index == 68 ){ cliloc = "1061394"; name = "Evil Omen"; circle = "1"; reagents = "Bat Wing, Nox Crystal"; mana = "17"; skill = "30"; }
			else if ( index == 69 ){ cliloc = "1061398"; name = "Pain Spike"; circle = "1"; reagents = "Grave Dust, Pig Iron"; mana = "8"; skill = "30"; }
			else if ( index == 70 ){ cliloc = "1061397"; name = "Mind Rot"; circle = "2"; reagents = "Bat Wing, Daemon Blood, Pig Iron"; mana = "26"; skill = "45"; }
			else if ( index == 71 ){ cliloc = "1061401"; name = "Summon Familiar"; circle = "2"; reagents = "Bat Wing, Grave Dust, Daemon Blood"; mana = "26"; skill = "45"; }
			else if ( index == 72 ){ cliloc = "1061390"; name = "Animate Dead"; circle = "3"; reagents = "Grave Dust, Daemon Blood"; mana = "35"; skill = "55"; }
			else if ( index == 73 ){ cliloc = "1061395"; name = "Horrific Beast"; circle = "3"; reagents = "Bat Wing, Daemon Blood"; mana = "17"; skill = "55"; }
			else if ( index == 74 ){ cliloc = "1061399"; name = "Poison Strike"; circle = "4"; reagents = "Nox Crystal"; mana = "26"; skill = "65"; }
			else if ( index == 75 ){ cliloc = "1061404"; name = "Wither"; circle = "5"; reagents = "Grave Dust, Nox Crystal, Pig Iron"; mana = "35"; skill = "75"; }
			else if ( index == 76 ){ cliloc = "1061396"; name = "Lich Form"; circle = "6"; reagents = "Grave Dust, Daemon Blood, Nox Crystal"; mana = "35"; skill = "85"; }
			else if ( index == 77 ){ cliloc = "1061400"; name = "Strangle"; circle = "6"; reagents = "Daemon Blood, Nox Crystal"; mana = "44"; skill = "85"; }
			else if ( index == 78 ){ cliloc = "1061406"; name = "Exorcism"; circle = "7"; reagents = "Nox Crystal, Grave Dust"; mana = "60"; skill = "95"; }
			else if ( index == 79 ){ cliloc = "1061403"; name = "Vengeful Spirit"; circle = "7"; reagents = "Bat Wing, Grave Dust, Pig Iron"; mana = "62"; skill = "95"; }
			else if ( index == 80 ){ cliloc = "1061405"; name = "Wraith Form"; circle = "7"; reagents = "Nox Crystal, Pig Iron"; mana = "26"; skill = "95"; }
			else if ( index == 81 ){ cliloc = "1061402"; name = "Vampiric Embrace"; circle = "8"; reagents = "Bat Wing, Nox Crystal, Pig Iron"; mana = "35"; skill = "100"; }

			if ( slice == 1 ){ value = circle; }
			else if ( slice == 2 ){ value = name; }
			else if ( slice == 3 ){ value = reagents; }
			else if ( slice == 4 ){ value = mana; }
			else if ( slice == 5 ){ value = skill; }
			else { value = cliloc; }

			return value;
		}

		public static void GiveScroll( int index, Mobile from ) //////////////////////////////////////////////////////////////////////////////
		{
			if ( index == 1 ){ from.AddToBackpack( new ClumsyScroll() ); }
			else if ( index == 2 ){ from.AddToBackpack( new CreateFoodScroll() ); }
			else if ( index == 3 ){ from.AddToBackpack( new FeeblemindScroll() ); }
			else if ( index == 4 ){ from.AddToBackpack( new HealScroll() ); }
			else if ( index == 5 ){ from.AddToBackpack( new MagicArrowScroll() ); }
			else if ( index == 6 ){ from.AddToBackpack( new NightSightScroll() ); }
			else if ( index == 7 ){ from.AddToBackpack( new ReactiveArmorScroll() ); }
			else if ( index == 8 ){ from.AddToBackpack( new WeakenScroll() ); }
			else if ( index == 9 ){ from.AddToBackpack( new AgilityScroll() ); }
			else if ( index == 10 ){ from.AddToBackpack( new CunningScroll() ); }
			else if ( index == 11 ){ from.AddToBackpack( new CureScroll() ); }
			else if ( index == 12 ){ from.AddToBackpack( new HarmScroll() ); }
			else if ( index == 13 ){ from.AddToBackpack( new MagicTrapScroll() ); }
			else if ( index == 14 ){ from.AddToBackpack( new MagicUnTrapScroll() ); }
			else if ( index == 15 ){ from.AddToBackpack( new ProtectionScroll() ); }
			else if ( index == 16 ){ from.AddToBackpack( new StrengthScroll() ); }
			else if ( index == 17 ){ from.AddToBackpack( new BlessScroll() ); }
			else if ( index == 18 ){ from.AddToBackpack( new FireballScroll() ); }
			else if ( index == 19 ){ from.AddToBackpack( new MagicLockScroll() ); }
			else if ( index == 20 ){ from.AddToBackpack( new PoisonScroll() ); }
			else if ( index == 21 ){ from.AddToBackpack( new TelekinisisScroll() ); }
			else if ( index == 22 ){ from.AddToBackpack( new TeleportScroll() ); }
			else if ( index == 23 ){ from.AddToBackpack( new UnlockScroll() ); }
			else if ( index == 24 ){ from.AddToBackpack( new WallOfStoneScroll() ); }
			else if ( index == 25 ){ from.AddToBackpack( new ArchCureScroll() ); }
			else if ( index == 26 ){ from.AddToBackpack( new ArchProtectionScroll() ); }
			else if ( index == 27 ){ from.AddToBackpack( new CurseScroll() ); }
			else if ( index == 28 ){ from.AddToBackpack( new FireFieldScroll() ); }
			else if ( index == 29 ){ from.AddToBackpack( new GreaterHealScroll() ); }
			else if ( index == 30 ){ from.AddToBackpack( new LightningScroll() ); }
			else if ( index == 31 ){ from.AddToBackpack( new ManaDrainScroll() ); }
			else if ( index == 32 ){ from.AddToBackpack( new RecallScroll() ); }
			else if ( index == 33 ){ from.AddToBackpack( new BladeSpiritsScroll() ); }
			else if ( index == 34 ){ from.AddToBackpack( new DispelFieldScroll() ); }
			else if ( index == 35 ){ from.AddToBackpack( new IncognitoScroll() ); }
			else if ( index == 36 ){ from.AddToBackpack( new MagicReflectScroll() ); }
			else if ( index == 37 ){ from.AddToBackpack( new MindBlastScroll() ); }
			else if ( index == 38 ){ from.AddToBackpack( new ParalyzeScroll() ); }
			else if ( index == 39 ){ from.AddToBackpack( new PoisonFieldScroll() ); }
			else if ( index == 40 ){ from.AddToBackpack( new SummonCreatureScroll() ); }
			else if ( index == 41 ){ from.AddToBackpack( new DispelScroll() ); }
			else if ( index == 42 ){ from.AddToBackpack( new EnergyBoltScroll() ); }
			else if ( index == 43 ){ from.AddToBackpack( new ExplosionScroll() ); }
			else if ( index == 44 ){ from.AddToBackpack( new InvisibilityScroll() ); }
			else if ( index == 45 ){ from.AddToBackpack( new MarkScroll() ); }
			else if ( index == 46 ){ from.AddToBackpack( new MassCurseScroll() ); }
			else if ( index == 47 ){ from.AddToBackpack( new ParalyzeFieldScroll() ); }
			else if ( index == 48 ){ from.AddToBackpack( new RevealScroll() ); }
			else if ( index == 49 ){ from.AddToBackpack( new ChainLightningScroll() ); }
			else if ( index == 50 ){ from.AddToBackpack( new EnergyFieldScroll() ); }
			else if ( index == 51 ){ from.AddToBackpack( new FlamestrikeScroll() ); }
			else if ( index == 52 ){ from.AddToBackpack( new GateTravelScroll() ); }
			else if ( index == 53 ){ from.AddToBackpack( new ManaVampireScroll() ); }
			else if ( index == 54 ){ from.AddToBackpack( new MassDispelScroll() ); }
			else if ( index == 55 ){ from.AddToBackpack( new MeteorSwarmScroll() ); }
			else if ( index == 56 ){ from.AddToBackpack( new PolymorphScroll() ); }
			else if ( index == 57 ){ from.AddToBackpack( new EarthquakeScroll() ); }
			else if ( index == 58 ){ from.AddToBackpack( new EnergyVortexScroll() ); }
			else if ( index == 59 ){ from.AddToBackpack( new ResurrectionScroll() ); }
			else if ( index == 60 ){ from.AddToBackpack( new SummonAirElementalScroll() ); }
			else if ( index == 61 ){ from.AddToBackpack( new SummonDaemonScroll() ); }
			else if ( index == 62 ){ from.AddToBackpack( new SummonEarthElementalScroll() ); }
			else if ( index == 63 ){ from.AddToBackpack( new SummonFireElementalScroll() ); }
			else if ( index == 64 ){ from.AddToBackpack( new SummonWaterElementalScroll() ); }
			else if ( index == 65 ){ from.AddToBackpack( new BloodOathScroll() ); }
			else if ( index == 66 ){ from.AddToBackpack( new CorpseSkinScroll() ); }
			else if ( index == 67 ){ from.AddToBackpack( new CurseWeaponScroll() ); }
			else if ( index == 68 ){ from.AddToBackpack( new EvilOmenScroll() ); }
			else if ( index == 69 ){ from.AddToBackpack( new PainSpikeScroll() ); }
			else if ( index == 70 ){ from.AddToBackpack( new MindRotScroll() ); }
			else if ( index == 71 ){ from.AddToBackpack( new SummonFamiliarScroll() ); }
			else if ( index == 72 ){ from.AddToBackpack( new AnimateDeadScroll() ); }
			else if ( index == 73 ){ from.AddToBackpack( new HorrificBeastScroll() ); }
			else if ( index == 74 ){ from.AddToBackpack( new PoisonStrikeScroll() ); }
			else if ( index == 75 ){ from.AddToBackpack( new WitherScroll() ); }
			else if ( index == 76 ){ from.AddToBackpack( new LichFormScroll() ); }
			else if ( index == 77 ){ from.AddToBackpack( new StrangleScroll() ); }
			else if ( index == 78 ){ from.AddToBackpack( new ExorcismScroll() ); }
			else if ( index == 79 ){ from.AddToBackpack( new VengefulSpiritScroll() ); }
			else if ( index == 80 ){ from.AddToBackpack( new WraithFormScroll() ); }
			else if ( index == 81 ){ from.AddToBackpack( new VampiricEmbraceScroll() ); }
		}

		public static bool CheckReagents( Mobile from, int spell, bool deplete, bool ancient, int qty ) ////////////////////////////////////////////////////////////////////
		{
			bool HaveReagents = true;

			Container pack = from.Backpack;

			string reagents = Research.ScrollInformation( spell, 3 );
				if ( ancient ){ reagents = Research.SpellInformation( spell, 5 ); }

				reagents = reagents.Replace(", ", ",");

			if ( reagents.Length > 0 )
			{
				string[] regs = reagents.Split(',');
				foreach (string reg in regs)
				{
					if ( deplete )
					{
						if ( reg == "Bat Wing" && from.Backpack.FindItemByType( typeof ( BatWing ) ) != null ){ pack.ConsumeTotal(typeof(BatWing), qty); }
						else if ( reg == "Black Pearl" && from.Backpack.FindItemByType( typeof ( BlackPearl ) ) != null ){ pack.ConsumeTotal(typeof(BlackPearl), qty); }
						else if ( reg == "Bloodmoss" && from.Backpack.FindItemByType( typeof ( Bloodmoss ) ) != null ){ pack.ConsumeTotal(typeof(Bloodmoss), qty); }
						else if ( reg == "Daemon Blood" && from.Backpack.FindItemByType( typeof ( DaemonBlood ) ) != null ){ pack.ConsumeTotal(typeof(DaemonBlood), qty); }
						else if ( reg == "Garlic" && from.Backpack.FindItemByType( typeof ( Garlic ) ) != null ){ pack.ConsumeTotal(typeof(Garlic), qty); }
						else if ( reg == "Ginseng" && from.Backpack.FindItemByType( typeof ( Ginseng ) ) != null ){ pack.ConsumeTotal(typeof(Ginseng), qty); }
						else if ( reg == "Grave Dust" && from.Backpack.FindItemByType( typeof ( GraveDust ) ) != null ){ pack.ConsumeTotal(typeof(GraveDust), qty); }
						else if ( reg == "Mandrake Root" && from.Backpack.FindItemByType( typeof ( MandrakeRoot ) ) != null ){ pack.ConsumeTotal(typeof(MandrakeRoot), qty); }
						else if ( reg == "Nightshade" && from.Backpack.FindItemByType( typeof ( Nightshade ) ) != null ){ pack.ConsumeTotal(typeof(Nightshade), qty); }
						else if ( reg == "Nox Crystal" && from.Backpack.FindItemByType( typeof ( NoxCrystal ) ) != null ){ pack.ConsumeTotal(typeof(NoxCrystal), qty); }
						else if ( reg == "Pig Iron" && from.Backpack.FindItemByType( typeof ( PigIron ) ) != null ){ pack.ConsumeTotal(typeof(PigIron), qty); }
						else if ( reg == "Spiders Silk" && from.Backpack.FindItemByType( typeof ( SpidersSilk ) ) != null ){ pack.ConsumeTotal(typeof(SpidersSilk), qty); }
						else if ( reg == "Sulfurous Ash" && from.Backpack.FindItemByType( typeof ( SulfurousAsh ) ) != null ){ pack.ConsumeTotal(typeof(SulfurousAsh), qty); }
						else if ( reg == "Silver Serpent Venom" && from.Backpack.FindItemByType( typeof ( SilverSerpentVenom ) ) != null ){ pack.ConsumeTotal(typeof(SilverSerpentVenom), qty); }
						else if ( reg == "Dragon Blood" && from.Backpack.FindItemByType( typeof ( DragonBlood ) ) != null ){ pack.ConsumeTotal(typeof(DragonBlood), qty); }
						else if ( reg == "Enchanted Seaweed" && from.Backpack.FindItemByType( typeof ( EnchantedSeaweed ) ) != null ){ pack.ConsumeTotal(typeof(EnchantedSeaweed), qty); }
						else if ( reg == "Dragon Tooth" && from.Backpack.FindItemByType( typeof ( DragonTooth ) ) != null ){ pack.ConsumeTotal(typeof(DragonTooth), qty); }
						else if ( reg == "Golden Serpent Venom" && from.Backpack.FindItemByType( typeof ( GoldenSerpentVenom ) ) != null ){ pack.ConsumeTotal(typeof(GoldenSerpentVenom), qty); }
						else if ( reg == "Lich Dust" && from.Backpack.FindItemByType( typeof ( LichDust ) ) != null ){ pack.ConsumeTotal(typeof(LichDust), qty); }
						else if ( reg == "Demon Claw" && from.Backpack.FindItemByType( typeof ( DemonClaw ) ) != null ){ pack.ConsumeTotal(typeof(DemonClaw), qty); }
						else if ( reg == "Pegasus Feather" && from.Backpack.FindItemByType( typeof ( PegasusFeather ) ) != null ){ pack.ConsumeTotal(typeof(PegasusFeather), qty); }
						else if ( reg == "Phoenix Feather" && from.Backpack.FindItemByType( typeof ( PhoenixFeather ) ) != null ){ pack.ConsumeTotal(typeof(PhoenixFeather), qty); }
						else if ( reg == "Unicorn Horn" && from.Backpack.FindItemByType( typeof ( UnicornHorn ) ) != null ){ pack.ConsumeTotal(typeof(UnicornHorn), qty); }
						else if ( reg == "Demigod Blood" && from.Backpack.FindItemByType( typeof ( DemigodBlood ) ) != null ){ pack.ConsumeTotal(typeof(DemigodBlood), qty); }
						else if ( reg == "Ghostly Dust" && from.Backpack.FindItemByType( typeof ( GhostlyDust ) ) != null ){ pack.ConsumeTotal(typeof(GhostlyDust), qty); }
						else if ( reg == "Eye of Toad" && from.Backpack.FindItemByType( typeof ( EyeOfToad ) ) != null ){ pack.ConsumeTotal(typeof(EyeOfToad), qty); }
						else if ( reg == "Fairy Egg" && from.Backpack.FindItemByType( typeof ( FairyEgg ) ) != null ){ pack.ConsumeTotal(typeof(FairyEgg), qty); }
						else if ( reg == "Gargoyle Ear" && from.Backpack.FindItemByType( typeof ( GargoyleEar ) ) != null ){ pack.ConsumeTotal(typeof(GargoyleEar), qty); }
						else if ( reg == "Beetle Shell" && from.Backpack.FindItemByType( typeof ( BeetleShell ) ) != null ){ pack.ConsumeTotal(typeof(BeetleShell), qty); }
						else if ( reg == "Moon Crystal" && from.Backpack.FindItemByType( typeof ( MoonCrystal ) ) != null ){ pack.ConsumeTotal(typeof(MoonCrystal), qty); }
						else if ( reg == "Pixie Skull" && from.Backpack.FindItemByType( typeof ( PixieSkull ) ) != null ){ pack.ConsumeTotal(typeof(PixieSkull), qty); }
						else if ( reg == "Red Lotus" && from.Backpack.FindItemByType( typeof ( RedLotus ) ) != null ){ pack.ConsumeTotal(typeof(RedLotus), qty); }
						else if ( reg == "Sea Salt" && from.Backpack.FindItemByType( typeof ( SeaSalt ) ) != null ){ pack.ConsumeTotal(typeof(SeaSalt), qty); }
						else if ( reg == "Silver Widow" && from.Backpack.FindItemByType( typeof ( SilverWidow ) ) != null ){ pack.ConsumeTotal(typeof(SilverWidow), qty); }
						else if ( reg == "Swamp Berries" && from.Backpack.FindItemByType( typeof ( SwampBerries ) ) != null ){ pack.ConsumeTotal(typeof(SwampBerries), qty); }
						else if ( reg == "Brimstone" && from.Backpack.FindItemByType( typeof ( Brimstone ) ) != null ){ pack.ConsumeTotal(typeof(Brimstone), qty); }
						else if ( reg == "Butterfly Wings" && from.Backpack.FindItemByType( typeof ( ButterflyWings ) ) != null ){ pack.ConsumeTotal(typeof(ButterflyWings), qty); }
					}
					else
					{
						if ( reg == "Bat Wing" && RegCount( from, typeof( BatWing ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Black Pearl" && RegCount( from, typeof( BlackPearl ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Bloodmoss" && RegCount( from, typeof( Bloodmoss ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Daemon Blood" && RegCount( from, typeof( DaemonBlood ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Garlic" && RegCount( from, typeof( Garlic ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Ginseng" && RegCount( from, typeof( Ginseng ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Grave Dust" && RegCount( from, typeof( GraveDust ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Mandrake Root" && RegCount( from, typeof( MandrakeRoot ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Nightshade" && RegCount( from, typeof( Nightshade ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Nox Crystal" && RegCount( from, typeof( NoxCrystal ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Pig Iron" && RegCount( from, typeof( PigIron ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Spiders Silk" && RegCount( from, typeof( SpidersSilk ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Sulfurous Ash" && RegCount( from, typeof( SulfurousAsh ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Silver Serpent Venom" && RegCount( from, typeof( SilverSerpentVenom ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Dragon Blood" && RegCount( from, typeof( DragonBlood ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Enchanted Seaweed" && RegCount( from, typeof( EnchantedSeaweed ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Dragon Tooth" && RegCount( from, typeof( DragonTooth ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Golden Serpent Venom" && RegCount( from, typeof( GoldenSerpentVenom ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Lich Dust" && RegCount( from, typeof( LichDust ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Demon Claw" && RegCount( from, typeof( DemonClaw ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Unicorn Horn" && RegCount( from, typeof( UnicornHorn ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Demigod Blood" && RegCount( from, typeof( DemigodBlood ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Ghostly Dust" && RegCount( from, typeof( GhostlyDust ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Eye of Toad" && RegCount( from, typeof( EyeOfToad ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Fairy Egg" && RegCount( from, typeof( FairyEgg ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Gargoyle Ear" && RegCount( from, typeof( GargoyleEar ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Beetle Shell" && RegCount( from, typeof( BeetleShell ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Moon Crystal" && RegCount( from, typeof( MoonCrystal ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Pixie Skull" && RegCount( from, typeof( PixieSkull ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Red Lotus" && RegCount( from, typeof( RedLotus ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Sea Salt" && RegCount( from, typeof( SeaSalt ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Silver Widow" && RegCount( from, typeof( SilverWidow ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Swamp Berries" && RegCount( from, typeof( SwampBerries ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Brimstone" && RegCount( from, typeof( Brimstone ) ) < qty ){ HaveReagents = false; }
						else if ( reg == "Butterfly Wings" && RegCount( from, typeof( ButterflyWings ) ) < qty ){ HaveReagents = false; }
					}
				}
			}

			return HaveReagents;
		}

		public static int RegCount( Mobile from, Type recipe )
		{
			int reagents = 0;

			Item[] item;

			item = (from.Backpack).FindItemsByType( recipe );

			for ( int i = 0; i < item.Length; ++i )
				reagents += item[i].Amount;

			return reagents;
		}

		public static void CreateNormalSpell( ResearchBag bag, Mobile from, int spell ) /////////////////////////////////////////////////////////////
		{
			bool canMake = true;

			string name = Research.ScrollInformation( spell, 2 );
			int mana = Int32.Parse( Research.ScrollInformation( spell, 4 ) );
			int skill = Int32.Parse( Research.ScrollInformation( spell, 5 ) );
			int ink = Int32.Parse( Research.ScrollInformation( spell, 1 ) );
			string msg = "";

			if ( from.Mana < mana )
			{
				msg = "You lack the mana to scribe this spell to parchment."; canMake = false;
			}
			else if ( from.Skills[SkillName.Inscribe].Value < skill )
			{
				msg = "You lack the skill to scribe this spell to parchment."; canMake = false;
			}
			else if ( bag.BagInk < ink )
			{
				msg = "You do not have enough octopus ink in your bag."; canMake = false;
			}
			else if ( bag.BagQuills < 1 )
			{
				msg = "You need at least one quill in your bag."; canMake = false;
			}
			else if ( bag.BagScrolls < 1 )
			{
				msg = "You need at least one blank scroll in your bag."; canMake = false;
			}
			else if ( !CheckReagents( from, spell, false, false, 1 ) )
			{
				msg = "You do not have the right reagents to scribe this spell."; canMake = false;
			}
			else if ( from.Skills[SkillName.Inscribe].Value < Utility.RandomMinMax( skill-25, skill+25 ) && canMake )
			{
				from.CheckSkill( SkillName.Inscribe, 0, 125 );
				canMake = false;
				if ( from.Skills[SkillName.Inscribe].Value < Utility.RandomMinMax( 0, 125 ) ){ from.Mana = from.Mana - mana; }
				if ( from.Skills[SkillName.Inscribe].Value < Utility.RandomMinMax( 0, 125 ) ){ bag.BagInk = bag.BagInk - 1; if ( bag.BagInk < 1 ){ bag.BagInk = 0; } }
				if ( from.Skills[SkillName.Inscribe].Value < Utility.RandomMinMax( 0, 125 ) ){ bag.BagScrolls = bag.BagScrolls - 1; if ( bag.BagScrolls < 1 ){ bag.BagScrolls = 0; } }
				if ( from.Skills[SkillName.Inscribe].Value < Utility.RandomMinMax( 0, 125 ) ){ bag.BagQuills = bag.BagQuills - 1; if ( bag.BagQuills < 1 ){ bag.BagQuills = 0; } }
				if ( from.Skills[SkillName.Inscribe].Value < Utility.RandomMinMax( 0, 125 ) ){ CheckReagents( from, spell, true, false, 1 ); }
				from.PlaySound(0x249);
				msg = "You fail to scribe the " + name + " scroll and some of your materials may be lost.";
			}

			if ( canMake )
			{
				from.CheckSkill( SkillName.Inscribe, 0, 125 );
				from.Mana = from.Mana - mana;
				bag.BagInk = bag.BagInk - ink; if ( bag.BagInk < 1 ){ bag.BagInk = 0; }
				bag.BagScrolls = bag.BagScrolls - 1; if ( bag.BagScrolls < 1 ){ bag.BagScrolls = 0; }
				bag.BagQuills = bag.BagQuills - 1; if ( bag.BagQuills < 1 ){ bag.BagQuills = 0; }
				CheckReagents( from, spell, true, false, 1 );
				from.PlaySound(0x249);
				GiveScroll( spell, from );
				bag.BagMsgString = "You successfully scribe the " + name + " scroll.";
				bag.BagMessage = 2;
				bag.InvalidateProperties();
			}
			else
			{
				bag.BagMessage = 1;
				bag.BagMsgString = msg;
			}
			bag.InvalidateProperties();
		}

		public static string NextWizardry( ResearchBag bag ) ///////////////////////////////////////////////////////////////////////////////////////////
		{
			string scroll = "";
			string found = bag.SpellsMagery;
			bool check = true;
			int count = 0;

			if ( found.Length > 0 )
			{
				string[] spells = found.Split('#');
				int entry = 1;
				foreach (string spell in spells)
				{
					if ( spell == "0" && check ){ scroll = ScrollInformation( entry, 2 ); check = false; }
					else if ( spell == "1" ){ count++; }
					entry++;
				}
			}

			if ( check ){ scroll = "Clumsy"; }
			if ( count > 63 ){ scroll = ""; }

			return scroll;
		}

		public static bool GetWizardry( ResearchBag bag, int index ) ////////////////////////////////////////////////////////////////////////////////
		{
			string found = bag.SpellsMagery;

			bool HaveSpell = false;

			if ( found.Length > 0 )
			{
				string[] spells = found.Split('#');
				int entry = 1;
				foreach (string spell in spells)
				{
					if ( entry == index && spell == "1" ){ HaveSpell = true; }

					entry++;
				}
			}

			return HaveSpell;
		}

		public static void SetWizardry( ResearchBag bag, Mobile from ) //////////////////////////////////////////////////////////////////////////////
		{
			string found = bag.SpellsMagery;
			string got = "";
			int magic = 0;

			if ( found.Length > 0 )
			{
				string[] spells = found.Split('#');
				int entry = 1;
				bool updated = false;
				foreach (string spell in spells)
				{
					if ( spell == "0" && !updated ){ got = got + "1#"; magic = entry; updated = true; }
					else if ( spell == ""){ got = got + "0#"; }
					else { got = got + spell + "#"; }

					entry++;
				}

				bag.SpellsMagery = got;
			}

			from.LocalOverheadMessage(MessageType.Emote, 1150, true, "You found " + bag.SpellsMageItem + "!");
			from.SendSound( 0x3D );
			LoggingFunctions.LogGeneric( from, "has found " + bag.SpellsMageItem + "." );

			if ( magic > 63 )
			{
				bag.SpellsMageLocation = "";
				bag.SpellsMageWorld = Land.None;
			}
			else
			{
				int level = 0;
				if ( magic > 56 ){ level = Utility.RandomList(5,7); }
				else if ( magic > 48 ){ level = Utility.RandomList(4,5); }
				else if ( magic > 40 ){ level = Utility.RandomList(3,4); }
				else if ( magic > 32 ){ level = Utility.RandomList(2,3); }
				else if ( magic > 24 ){ level = Utility.RandomList(1,2); }
				else if ( magic > 16 ){ level = Utility.RandomList(0,2); }
				else if ( magic > 8 ){ level = Utility.RandomList(0,1); }

				FindLocation( from, level, "mage", bag );
			}
		}

		public static string NextNecromancy( ResearchBag bag ) //////////////////////////////////////////////////////////////////////////////////////
		{
			string scroll = "";
			string found = bag.SpellsNecromancy;
			bool check = true;
			int count = 0;

			if ( found.Length > 0 )
			{
				string[] spells = found.Split('#');
				int entry = 1;
				foreach (string spell in spells)
				{
					if ( spell == "0" && check ){ scroll = ScrollInformation( (entry+64), 2 ); check = false; }
					else if ( spell == "1" ){ count++; }
					entry++;
				}
			}

			if ( check ){ scroll = "Blood Oath"; }
			if ( count > 16 ){ scroll = ""; }

			return scroll;
		}

		public static bool GetNecromancy( ResearchBag bag, int index ) //////////////////////////////////////////////////////////////////////////////
		{
			string found = bag.SpellsNecromancy;

			bool HaveSpell = false;

			if ( found.Length > 0 )
			{
				string[] spells = found.Split('#');
				int entry = 1;
				foreach (string spell in spells)
				{
					if ( entry == index && spell == "1" ){ HaveSpell = true; }

					entry++;
				}
			}

			return HaveSpell;
		}

		public static void SetNecromancy( ResearchBag bag, Mobile from ) ////////////////////////////////////////////////////////////////////////////
		{
			string found = bag.SpellsNecromancy;
			string got = "";
			int magic = 0;

			if ( found.Length > 0 )
			{
				string[] spells = found.Split('#');
				int entry = 1;
				bool updated = false;
				foreach (string spell in spells)
				{
					if ( spell == "0" && !updated ){ got = got + "1#"; magic = entry; updated = true; }
					else if ( spell == ""){ got = got + "0#"; }
					else { got = got + spell + "#"; }

					entry++;
				}

				bag.SpellsNecromancy = got;
			}

			from.LocalOverheadMessage(MessageType.Emote, 1150, true, "You found " + bag.SpellsNecroItem + "!");
			from.SendSound( 0x3D );
			LoggingFunctions.LogGeneric( from, "has found " + bag.SpellsNecroItem + "." );

			if ( magic > 16 )
			{
				bag.SpellsNecroLocation = "";
				bag.SpellsNecroWorld = Land.None;
			}
			else
			{
				int level = 0;
				if ( magic > 14 ){ level = Utility.RandomList(5,7); }
				else if ( magic > 12 ){ level = Utility.RandomList(4,5); }
				else if ( magic > 10 ){ level = Utility.RandomList(3,4); }
				else if ( magic > 7 ){ level = Utility.RandomList(2,3); }
				else if ( magic > 6 ){ level = Utility.RandomList(1,2); }
				else if ( magic > 4 ){ level = Utility.RandomList(0,2); }
				else if ( magic > 2 ){ level = Utility.RandomList(0,1); }

				FindLocation( from, level, "necro", bag );
			}
		}

		public static string NextResearch( ResearchBag bag ) ///////////////////////////////////////////////////////////////////////////////////////////
		{
			string scroll = "";
			string found = bag.ResearchSpells;
			bool check = true;
			int count = 0;

			if ( found.Length > 0 )
			{
				string[] spells = found.Split('#');
				int entry = 1;
				foreach (string spell in spells)
				{
					if ( spell == "0" && check ){ scroll = SpellInformation( entry, 2 ) + " from the School of " + SpellInformation( entry, 3 ); check = false; }
					else if ( spell == "1" ){ count++; }
					entry++;
				}
			}

			if ( check ){ scroll = "Conjure from the School of Conjuration"; }
			if ( count > 63 ){ scroll = ""; }

			return scroll;
		}

		public static bool GetResearch( ResearchBag bag, int index ) ////////////////////////////////////////////////////////////////////////////////
		{
			string found = bag.ResearchSpells;

			bool HaveSpell = false;

			if ( found.Length > 0 )
			{
				string[] spells = found.Split('#');
				int entry = 1;
				foreach (string spell in spells)
				{
					if ( entry == index && spell == "1" ){ HaveSpell = true; }

					entry++;
				}
			}

			return HaveSpell;
		}

		public static void SetResearch( ResearchBag bag, Mobile from ) //////////////////////////////////////////////////////////////////////////////
		{
			string found = bag.ResearchSpells;
			string got = "";
			int magic = 0;

			if ( found.Length > 0 )
			{
				string[] spells = found.Split('#');
				int entry = 1;
				bool updated = false;
				foreach (string spell in spells)
				{
					if ( spell == "0" && !updated ){ got = got + "1#"; magic = entry; updated = true; }
					else if ( spell == ""){ got = got + "0#"; }
					else { got = got + spell + "#"; }

					entry++;
				}

				bag.ResearchSpells = got;
			}

			from.LocalOverheadMessage(MessageType.Emote, 1150, true, "You found " + bag.ResearchItem + "!");
			from.SendSound( 0x3D );
			LoggingFunctions.LogGeneric( from, "has found " + bag.ResearchItem + "." );

			if ( magic > 63 )
			{
				bag.ResearchLocation = "";
				bag.ResearchWorld = Land.None;
			}
			else
			{
				int level = 0;
				if ( magic > 56 ){ level = Utility.RandomList(5,7); }
				else if ( magic > 48 ){ level = Utility.RandomList(4,5); }
				else if ( magic > 40 ){ level = Utility.RandomList(3,4); }
				else if ( magic > 32 ){ level = Utility.RandomList(2,3); }
				else if ( magic > 24 ){ level = Utility.RandomList(1,2); }
				else if ( magic > 16 ){ level = Utility.RandomList(0,2); }
				else if ( magic > 8 ){ level = Utility.RandomList(0,1); }

				FindLocation( from, level, "research", bag );
			}
		}

		public static void CreateResearchSpell( ResearchBag bag, Mobile from, int spell ) ///////////////////////////////////////////////////////////
		{
			bool canMake = true;

			string name = Research.SpellInformation( spell, 2 );
			int mana = Int32.Parse( Research.SpellInformation( spell, 7 ) );
			int skill = Int32.Parse( Research.SpellInformation( spell, 8 ) );
			string msg = "";

			if ( from.Mana < mana )
			{
				msg = "You lack the mana to scribe this spell to parchment."; canMake = false;
			}
			else if ( from.Skills[SkillName.Inscribe].Value < skill )
			{
				msg = "You lack the skill to scribe this spell to parchment."; canMake = false;
			}
			else if ( bag.BagQuills < 1 )
			{
				msg = "You need at least one quill in your bag."; canMake = false;
			}
			else if ( bag.BagScrolls < 1 )
			{
				msg = "You need at least one blank scroll in your bag."; canMake = false;
			}
			else if ( !CheckReagents( from, spell, false, true, 1 ) )
			{
				msg = "You do not have the right reagents to scribe this spell."; canMake = false;
			}
			else if ( GetPrepared( bag, spell ) >= 500 )
			{
				msg = "You have too many of these spells in your bag already."; canMake = false;
			}
			else if ( from.Skills[SkillName.Inscribe].Value < Utility.RandomMinMax( skill-25, skill+25 ) && canMake )
			{
				from.CheckSkill( SkillName.Inscribe, 0, 125 );
				canMake = false;
				if ( from.Skills[SkillName.Inscribe].Value < Utility.RandomMinMax( 0, 125 ) ){ from.Mana = from.Mana - mana; }
				if ( from.Skills[SkillName.Inscribe].Value < Utility.RandomMinMax( 0, 125 ) ){ bag.BagScrolls = bag.BagScrolls - 1; if ( bag.BagScrolls < 1 ){ bag.BagScrolls = 0; } }
				if ( from.Skills[SkillName.Inscribe].Value < Utility.RandomMinMax( 0, 125 ) ){ bag.BagQuills = bag.BagQuills - 1; if ( bag.BagQuills < 1 ){ bag.BagQuills = 0; } }
				if ( from.Skills[SkillName.Inscribe].Value < Utility.RandomMinMax( 0, 125 ) ){ CheckReagents( from, spell, true, true, 1 ); }
				from.PlaySound(0x249);
				msg = "You fail to scribe the " + name + " scroll and some of your materials may be lost.";
			}

			if ( canMake )
			{
				from.CheckSkill( SkillName.Inscribe, 0, 125 );
				from.Mana = from.Mana - mana;
				bag.BagScrolls = bag.BagScrolls - 1; if ( bag.BagScrolls < 1 ){ bag.BagScrolls = 0; }
				bag.BagQuills = bag.BagQuills - 1; if ( bag.BagQuills < 1 ){ bag.BagQuills = 0; }
				CheckReagents( from, spell, true, true, 1 );
				from.PlaySound(0x249);
				SetPrepared( bag, spell, 1 );
				bag.BagMsgString = "You successfully scribe the " + name + " scroll.";
				bag.BagMessage = 2;
				bag.InvalidateProperties();
			}
			else
			{
				bag.BagMessage = 1;
				bag.BagMsgString = msg;
			}
			bag.InvalidateProperties();
		}

		public static void CreateManySpells( ResearchBag bag, Mobile from, int spell ) ///////////////////////////////////////////////////////////
		{
			bool canMake = true;
			bool stopAll = false;
			bool manaCheck = true;
			bool playSound = false;
			string name = Research.SpellInformation( spell, 2 );
			int mana = Int32.Parse( Research.SpellInformation( spell, 7 ) );
			int skill = Int32.Parse( Research.SpellInformation( spell, 8 ) );
			string msg = "";
			int total = 0;
			int reagents = 0;

			while ( !stopAll )
			{
				canMake = true;

				if ( from.Mana < mana && manaCheck)
				{
					msg = "You lack the mana to scribe this spell to parchment."; canMake = false; stopAll = true;
				}
				else if ( from.Skills[SkillName.Inscribe].Value < skill )
				{
					msg = "You lack the skill to scribe this spell to parchment."; canMake = false; stopAll = true;
				}
				else if ( bag.BagQuills < 1 )
				{
					msg = "You need at least one quill in your bag."; canMake = false; stopAll = true;
				}
				else if ( bag.BagScrolls < 1 )
				{
					msg = "You need at least one blank scroll in your bag."; canMake = false; stopAll = true;
				}
				else if ( !CheckReagents( from, spell, false, true, reagents+1 ) )
				{
					msg = "You do not have the right reagents to scribe this spell."; canMake = false; stopAll = true;
				}
				else if ( GetPrepared( bag, spell ) >= 500 )
				{
					msg = "You have too many of these spells in your bag already."; canMake = false; stopAll = true;
				}
				else if ( from.Skills[SkillName.Inscribe].Value < Utility.RandomMinMax( skill-25, skill+25 ) && canMake )
				{
					from.CheckSkill( SkillName.Inscribe, 0, 125 );
					canMake = false;
					if ( from.Skills[SkillName.Inscribe].Value < Utility.RandomMinMax( 0, 125 ) && manaCheck ){ from.Mana = from.Mana - mana; manaCheck = false; }
					if ( from.Skills[SkillName.Inscribe].Value < Utility.RandomMinMax( 0, 125 ) ){ bag.BagScrolls = bag.BagScrolls - 1; if ( bag.BagScrolls < 1 ){ bag.BagScrolls = 0; } }
					if ( from.Skills[SkillName.Inscribe].Value < Utility.RandomMinMax( 0, 125 ) ){ bag.BagQuills = bag.BagQuills - 1; if ( bag.BagQuills < 1 ){ bag.BagQuills = 0; } }
					if ( from.Skills[SkillName.Inscribe].Value < Utility.RandomMinMax( 0, 125 ) ){ reagents++; }
					playSound = true;
					msg = "You fail to scribe the " + name + " scrolls and some of your materials may be lost.";
				}

				if ( canMake )
				{
					total++;
					from.CheckSkill( SkillName.Inscribe, 0, 125 );
					if ( manaCheck ){ from.Mana = from.Mana - mana; manaCheck = false; }
					bag.BagScrolls = bag.BagScrolls - 1; if ( bag.BagScrolls < 1 ){ bag.BagScrolls = 0; }
					bag.BagQuills = bag.BagQuills - 1; if ( bag.BagQuills < 1 ){ bag.BagQuills = 0; }
					reagents++;
					playSound = true;
					SetPrepared( bag, spell, 1 );
				}
			}

			if ( reagents > 0 ){ CheckReagents( from, spell, true, true, reagents ); }

			if ( total == 1 ){ bag.BagMsgString = "You successfully scribe a single " + name + " scroll."; bag.BagMessage = 2; }
			else if ( total > 0 ){ bag.BagMsgString = "You successfully scribe " + total + " " + name + " scrolls."; bag.BagMessage = 2; }
			else { bag.BagMsgString = msg; bag.BagMessage = 1; }
			if ( playSound ){ from.PlaySound(0x249); }
			bag.InvalidateProperties();
		}

		public static void SetPrepared( ResearchBag bag, int index, int amount ) ////////////////////////////////////////////////////////////////////////////////
		{
			string scrolls = bag.ResearchPrep1;
			int cycle = 33;
			int entry = 1;
				if ( index > 32 ){ scrolls = bag.ResearchPrep2; cycle = 65; entry = 33; }

			string got = "";
			int total = 0;
			string sums = "";

			if ( scrolls.Length > 0 )
			{
				string[] spells = scrolls.Split('#');

				foreach (string spell in spells)
				{
					if ( entry < cycle )
					{
						if ( entry == index ){ total = Int32.Parse( spell ); if ( amount < 0 ){ total--; } else { total++; } if ( total < 0 ){ total = 0; } sums = (total).ToString(); got = got + sums + "#"; }
						else if ( spell == ""){ got = got + "0#"; }
						else { got = got + spell + "#"; }
					}
					entry++;
				}

				if ( index > 32 ){ bag.ResearchPrep2 = got; } else { bag.ResearchPrep1 = got; }
			}
		}

		public static int GetPrepared( ResearchBag bag, int index ) ////////////////////////////////////////////////////////////////////////////////
		{
			string scrolls = bag.ResearchPrep1;
			int cycle = 33;
			int entry = 1;
				if ( index > 32 ){ scrolls = bag.ResearchPrep2; cycle = 65; entry = 33; }

			int total = 0;

			if ( scrolls.Length > 0 )
			{
				string[] spells = scrolls.Split('#');

				foreach (string spell in spells)
				{
					if ( entry < cycle )
					{
						if ( entry == index ){ total = Int32.Parse( spell ); }
					}
					entry++;
				}
			}

			return total;
		}

		public static void CastSpell( Mobile from, int spell ) //////////////////////////////////////////////////////////////////////////////////////
		{
			if ( Server.Misc.ResearchSettings.ResearchMaterials( from ) != null )
			{
				if ( ResearchSettings.HasSpell( from, spell ) )
				{
					if ( spell == 1 ){ InvokeCommand( "CastConjure", from ); }
					else if ( spell == 2 ){ InvokeCommand( "CastDeathSpeak", from ); }
					else if ( spell == 3 ){ InvokeCommand( "CastSneak", from ); }
					else if ( spell == 4 ){ InvokeCommand( "CastCreateFire", from ); }
					else if ( spell == 5 ){ InvokeCommand( "CastElectricalElemental", from ); }
					else if ( spell == 6 ){ InvokeCommand( "CastConfusionBlast", from ); }
					else if ( spell == 7 ){ InvokeCommand( "CastSeeTruth", from ); }
					else if ( spell == 8 ){ InvokeCommand( "CastIcicle", from ); }
					else if ( spell == 9 ){ InvokeCommand( "CastExtinguish", from ); }
					else if ( spell == 10 ){ InvokeCommand( "CastRockFlesh", from ); }
					else if ( spell == 11 ){ InvokeCommand( "CastMassMight", from ); }
					else if ( spell == 12 ){ InvokeCommand( "CastEndureCold", from ); }
					else if ( spell == 13 ){ InvokeCommand( "CastWeedElemental", from ); }
					else if ( spell == 14 ){ InvokeCommand( "CastSpawnCreature", from ); }
					else if ( spell == 15 ){ InvokeCommand( "CastHealingTouch", from ); }
					else if ( spell == 16 ){ InvokeCommand( "CastSnowBall", from ); }
					else if ( spell == 17 ){ InvokeCommand( "CastClone", from ); }
					else if ( spell == 18 ){ InvokeCommand( "CastGrantPeace", from ); }
					else if ( spell == 19 ){ InvokeCommand( "CastSleep", from ); }
					else if ( spell == 20 ){ InvokeCommand( "CastEndureHeat", from ); }
					else if ( spell == 21 ){ InvokeCommand( "CastIceElemental", from ); }
					else if ( spell == 22 ){ InvokeCommand( "CastEtherealTravel", from ); }
					else if ( spell == 23 ){ InvokeCommand( "CastWizardEye", from ); }
					else if ( spell == 24 ){ InvokeCommand( "CastFrostField", from ); }
					else if ( spell == 25 ){ InvokeCommand( "CastCreateGold", from ); }
					else if ( spell == 26 ){ InvokeCommand( "CastAnimateBones", from ); }
					else if ( spell == 27 ){ InvokeCommand( "CastCauseFear", from ); }
					else if ( spell == 28 ){ InvokeCommand( "CastIgnite", from ); }
					else if ( spell == 29 ){ InvokeCommand( "CastMudElemental", from ); }
					else if ( spell == 30 ){ InvokeCommand( "CastBanishDaemon", from ); }
					else if ( spell == 31 ){ InvokeCommand( "CastFadefromSight", from ); }
					else if ( spell == 32 ){ InvokeCommand( "CastGasCloud", from ); }
					else if ( spell == 33 ){ InvokeCommand( "CastSwarm", from ); }
					else if ( spell == 34 ){ InvokeCommand( "CastMaskofDeath", from ); }
					else if ( spell == 35 ){ InvokeCommand( "CastEnchant", from ); }
					else if ( spell == 36 ){ InvokeCommand( "CastFlameBolt", from ); }
					else if ( spell == 37 ){ InvokeCommand( "CastGemElemental", from ); }
					else if ( spell == 38 ){ InvokeCommand( "CastCallDestruction", from ); }
					else if ( spell == 39 ){ InvokeCommand( "CastDivination", from ); }
					else if ( spell == 40 ){ InvokeCommand( "CastFrostStrike", from ); }
					else if ( spell == 41 ){ InvokeCommand( "CastMagicSteed", from ); }
					else if ( spell == 42 ){ InvokeCommand( "CastCreateGolem", from ); }
					else if ( spell == 43 ){ InvokeCommand( "CastSleepField", from ); }
					else if ( spell == 44 ){ InvokeCommand( "CastConflagration", from ); }
					else if ( spell == 45 ){ InvokeCommand( "CastAcidElemental", from ); }
					else if ( spell == 46 ){ InvokeCommand( "CastMeteorShower", from ); }
					else if ( spell == 47 ){ InvokeCommand( "CastIntervention", from ); }
					else if ( spell == 48 ){ InvokeCommand( "CastHailStorm", from ); }
					else if ( spell == 49 ){ InvokeCommand( "CastAerialServant", from ); }
					else if ( spell == 50 ){ InvokeCommand( "CastOpenGround", from ); }
					else if ( spell == 51 ){ InvokeCommand( "CastCharm", from ); }
					else if ( spell == 52 ){ InvokeCommand( "CastExplosion", from ); }
					else if ( spell == 53 ){ InvokeCommand( "CastPoisonElemental", from ); }
					else if ( spell == 54 ){ InvokeCommand( "CastInvokeDevil", from ); }
					else if ( spell == 55 ){ InvokeCommand( "CastAirWalk", from ); }
					else if ( spell == 56 ){ InvokeCommand( "CastAvalanche", from ); }
					else if ( spell == 57 ){ InvokeCommand( "CastDeathVortex", from ); }
					else if ( spell == 58 ){ InvokeCommand( "CastWithstandDeath", from ); }
					else if ( spell == 59 ){ InvokeCommand( "CastMassSleep", from ); }
					else if ( spell == 60 ){ InvokeCommand( "CastRingofFire", from ); }
					else if ( spell == 61 ){ InvokeCommand( "CastBloodElemental", from ); }
					else if ( spell == 62 ){ InvokeCommand( "CastDevastation", from ); }
					else if ( spell == 63 ){ InvokeCommand( "CastRestoration", from ); }
					else if ( spell == 64 ){ InvokeCommand( "CastMassDeath", from ); }
				}
			}
		}
	}
}