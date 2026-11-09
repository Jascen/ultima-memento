using System;
using System.Collections;
using Server.Items;
using Server.Network;
using Server.Commands;
using Server.Mobiles;
using Server.Spells;
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
			return "<br><br>You can also type [Spells_Ancient to see a list of these commands.<br><br>[CastAcidElemental<br><br>[CastAerialServant<br><br>[CastAirWalk<br><br>[CastAnimateBones<br><br>[CastAvalanche<br><br>[CastBanishDaemon<br><br>[CastBloodElemental<br><br>[CastCallDestruction<br><br>[CastCauseFear<br><br>[CastCharm<br><br>[CastClone<br><br>[CastConflagration<br><br>[CastConfusionBlast<br><br>[CastConjure<br><br>[CastCreateFire<br><br>[CastCreateGold<br><br>[CastCreateGolem<br><br>[CastDeathSpeak<br><br>[CastDeathVortex<br><br>[CastDevastation<br><br>[CastDivination<br><br>[CastElectricalElemental<br><br>[CastEnchant<br><br>[CastEndureCold<br><br>[CastEndureHeat<br><br>[CastEtherealTravel<br><br>[CastExplosion<br><br>[CastExtinguish<br><br>[CastFadefromSight<br><br>[CastFlameBolt<br><br>[CastFrostField<br><br>[CastFrostStrike<br><br>[CastGasCloud<br><br>[CastGemElemental<br><br>[CastGrantPeace<br><br>[CastHailStorm<br><br>[CastHealingTouch<br><br>[CastIceElemental<br><br>[CastIcicle<br><br>[CastIgnite<br><br>[CastIntervention<br><br>[CastInvokeDevil<br><br>[CastMagicSteed<br><br>[CastMaskofDeath<br><br>[CastMassDeath<br><br>[CastMassMight<br><br>[CastMassSleep<br><br>[CastMeteorShower<br><br>[CastMudElemental<br><br>[CastOpenGround<br><br>[CastPoisonElemental<br><br>[CastRestoration<br><br>[CastRingofFire<br><br>[CastRockFlesh<br><br>[CastSeeTruth<br><br>[CastSleep<br><br>[CastSleepField<br><br>[CastSneak<br><br>[CastSnowBall<br><br>[CastSpawnCreature<br><br>[CastSwarm<br><br>[CastWeedElemental<br><br>[CastWithstandDeath<br><br>[CastWizardEye<br><br><br><br><br><br>";
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
			string school = "";
			string reagents = "";
			string much = "false";
			string regs = "false";

			SpellDefinition definition = Server.Spells.Research.ResearchSpellProvider.GetDefinitionByResearchIndex( index );

			if ( index == 1 ){ circle = "1"; school = "Conjuration"; reagents = "Moon Crystal, Fairy Egg"; regs = "true"; }
			else if ( index == 2 ){ circle = "1"; school = "Death"; reagents = "Grave Dust, Pixie Skull"; }
			else if ( index == 3 ){ circle = "1"; school = "Enchanting"; reagents = "Fairy Egg, Pixie Skull"; }
			else if ( index == 4 ){ circle = "1"; school = "Sorcery"; reagents = "Brimstone, Mandrake Root, Ginseng"; }
			else if ( index == 5 ){ circle = "1"; school = "Summoning"; reagents = "Silver Widow, Sulfurous Ash"; }
			else if ( index == 6 ){ circle = "1"; school = "Thaumaturgy"; reagents = "Mandrake Root, Pixie Skull, Fairy Egg"; }
			else if ( index == 7 ){ circle = "1"; school = "Theurgy"; reagents = "Grave Dust, Sea Salt, Fairy Egg"; }
			else if ( index == 8 ){ circle = "1"; school = "Wizardry"; reagents = "Moon Crystal, Pig Iron"; }

			else if ( index == 9 ){ circle = "2"; school = "Conjuration"; reagents = "Nightshade, Sea Salt, Black Pearl"; }
			else if ( index == 10 ){ circle = "2"; school = "Death"; reagents = "Moon Crystal, Garlic, Pig Iron, Black Pearl"; }
			else if ( index == 11 ){ circle = "2"; school = "Enchanting"; reagents = "Fairy Egg, Butterfly Wings"; }
			else if ( index == 12 ){ circle = "2"; school = "Sorcery"; reagents = "Spiders Silk, Moon Crystal"; }
			else if ( index == 13 ){ circle = "2"; school = "Summoning"; reagents = "Red Lotus, Nightshade"; }
			else if ( index == 14 ){ circle = "2"; school = "Thaumaturgy"; reagents = "Ginseng, Grave Dust, Pixie Skull"; }
			else if ( index == 15 ){ circle = "2"; school = "Theurgy"; reagents = "Garlic, Ginseng, Fairy Egg, Pixie Skull"; }
			else if ( index == 16 ){ circle = "2"; school = "Wizardry"; reagents = "Black Pearl, Moon Crystal"; }

			else if ( index == 17 ){ circle = "3"; school = "Conjuration"; reagents = "Gargoyle Ear, Fairy Egg"; }
			else if ( index == 18 ){ circle = "3"; school = "Death"; reagents = "Pixie Skull, Grave Dust"; }
			else if ( index == 19 ){ circle = "3"; school = "Enchanting"; reagents = "Fairy Egg, Mandrake Root, Sea Salt"; }
			else if ( index == 20 ){ circle = "3"; school = "Sorcery"; reagents = "Swamp Berries, Gargoyle Ear"; }
			else if ( index == 21 ){ circle = "3"; school = "Summoning"; reagents = "Sea Salt, Moon Crystal"; }
			else if ( index == 22 ){ circle = "3"; school = "Thaumaturgy"; reagents = "Black Pearl, Gargoyle Ear, Red Lotus"; }
			else if ( index == 23 ){ circle = "3"; school = "Theurgy"; reagents = "Eye of Toad, Silver Widow, Gargoyle Ear, Black Pearl"; }
			else if ( index == 24 ){ circle = "3"; school = "Wizardry"; reagents = "Moon Crystal, Eye of Toad, Spiders Silk"; }

			else if ( index == 25 ){ circle = "4"; school = "Conjuration"; reagents = "Unicorn Horn, Sea Salt, Ginseng, Golden Serpent Venom"; much = "true"; regs = "true"; }
			else if ( index == 26 ){ circle = "4"; school = "Death"; reagents = "Bat Wing, Grave Dust, Pixie Skull"; }
			else if ( index == 27 ){ circle = "4"; school = "Enchanting"; reagents = "Bloodmoss, Daemon Blood, Red Lotus"; }
			else if ( index == 28 ){ circle = "4"; school = "Sorcery"; reagents = "Brimstone, Sulfurous Ash, Black Pearl"; }
			else if ( index == 29 ){ circle = "4"; school = "Summoning"; reagents = "Pig Iron, Mandrake Root"; }
			else if ( index == 30 ){ circle = "4"; school = "Thaumaturgy"; reagents = "Gargoyle Ear, Demon Claw"; }
			else if ( index == 31 ){ circle = "4"; school = "Theurgy"; reagents = "Red Lotus, Nox Crystal"; }
			else if ( index == 32 ){ circle = "4"; school = "Wizardry"; reagents = "Nightshade, Silver Widow, Swamp Berries, Silver Serpent Venom"; }

			else if ( index == 33 ){ circle = "5"; school = "Conjuration"; reagents = "Beetle Shell, Silver Widow, Fairy Egg"; }
			else if ( index == 34 ){ circle = "5"; school = "Death"; reagents = "Lich Dust, Grave Dust, Pixie Skull"; }
			else if ( index == 35 ){ circle = "5"; school = "Enchanting"; reagents = "Pixie Skull, Fairy Egg, Dragon Tooth, Moon Crystal"; }
			else if ( index == 36 ){ circle = "5"; school = "Sorcery"; reagents = "Brimstone, Sulfurous Ash"; }
			else if ( index == 37 ){ circle = "5"; school = "Summoning"; reagents = "Moon Crystal, Pig Iron"; }
			else if ( index == 38 ){ circle = "5"; school = "Thaumaturgy"; reagents = "Bat Wing, Black Pearl, Brimstone, Pig Iron"; much = "true"; regs = "true"; }
			else if ( index == 39 ){ circle = "5"; school = "Theurgy"; reagents = "Ginseng, Daemon Blood, Eye of Toad"; }
			else if ( index == 40 ){ circle = "5"; school = "Wizardry"; reagents = "Moon Crystal, Grave Dust"; }

			else if ( index == 41 ){ circle = "6"; school = "Conjuration"; reagents = "Bat Wing, Butterfly Wings"; }
			else if ( index == 42 ){ circle = "6"; school = "Death"; reagents = "Pixie Skull, Grave Dust, Daemon Blood"; }
			else if ( index == 43 ){ circle = "6"; school = "Enchanting"; reagents = "Fairy Egg, Mandrake Root, Sea Salt, Grave Dust"; }
			else if ( index == 44 ){ circle = "6"; school = "Sorcery"; reagents = "Sulfurous Ash, Brimstone, Gargoyle Ear"; }
			else if ( index == 45 ){ circle = "6"; school = "Summoning"; reagents = "Swamp Berries, Nox Crystal, Beetle Shell, Eye of Toad"; }
			else if ( index == 46 ){ circle = "6"; school = "Thaumaturgy"; reagents = "Blood Moss, Brimstone, Mandrake Root, Black Pearl"; }
			else if ( index == 47 ){ circle = "6"; school = "Theurgy"; reagents = "Moon Crystal, Brimstone, Nightshade, Fairy Egg"; }
			else if ( index == 48 ){ circle = "6"; school = "Wizardry"; reagents = "Fairy Egg, Moon Crystal"; }

			else if ( index == 49 ){ circle = "7"; school = "Conjuration"; reagents = "Butterfly Wings, Gargoyle Ear, Pegasus Feather"; }
			else if ( index == 50 ){ circle = "7"; school = "Death"; reagents = "Brimstone, Grave Dust, Pig Iron, Mandrake Root"; much = "true"; regs = "true"; }
			else if ( index == 51 ){ circle = "7"; school = "Enchanting"; reagents = "Enchanted Seaweed, Fairy Egg, Pixie Skull"; }
			else if ( index == 52 ){ circle = "7"; school = "Sorcery"; reagents = "Fairy Egg, Brimstone, Sulfurous Ash"; }
			else if ( index == 53 ){ circle = "7"; school = "Summoning"; reagents = "Nightshade, Nox Crystal, Silver Serpent Venom, Silver Widow"; }
			else if ( index == 54 ){ circle = "7"; school = "Thaumaturgy"; reagents = "Daemon Blood, Eye of Toad, Brimstone, Demon Claw"; }
			else if ( index == 55 ){ circle = "7"; school = "Theurgy"; reagents = "Spiders Silk, Pixie Skull, Butterfly Wings"; }
			else if ( index == 56 ){ circle = "7"; school = "Wizardry"; reagents = "Blood Moss, Spiders Silk, Mandrake Root, Moon Crystal"; }

			else if ( index == 57 ){ circle = "8"; school = "Conjuration"; reagents = "Daemon Blood, Eye of Toad, Unicorn Horn"; }
			else if ( index == 58 ){ circle = "8"; school = "Death"; reagents = "Phoenix Feather, Demigod Blood, Enchanted Seaweed, Ghostly Dust"; much = "true"; regs = "true"; }
			else if ( index == 59 ){ circle = "8"; school = "Enchanting"; reagents = "Fairy Egg, Mandrake Root, Enchanted Seaweed, Lich Dust"; regs = "true"; }
			else if ( index == 60 ){ circle = "8"; school = "Sorcery"; reagents = "Brimstone, Pig Iron, Daemon Blood"; }
			else if ( index == 61 ){ circle = "8"; school = "Summoning"; reagents = "Black Pearl, Bloodmoss, Dragon Blood, Daemon Blood"; }
			else if ( index == 62 ){ circle = "8"; school = "Thaumaturgy"; reagents = "Butterfly Wings, Black Pearl, Demigod Blood, Demon Claw"; much = "true"; }
			else if ( index == 63 ){ circle = "8"; school = "Theurgy"; reagents = "Butterfly Wings, Garlic, Enchanted Seaweed"; }
			else if ( index == 64 ){ circle = "8"; school = "Wizardry"; reagents = "Pixie Skull, Bat Wing, Dragon Tooth"; much = "true"; regs = "true"; }

			if ( slice == 1 ){ value = circle; }
			else if ( slice == 2 ){ value = definition != null ? definition.Name.String : ""; }
			else if ( slice == 3 ){ value = school; }
			else if ( slice == 4 ){ value = definition != null ? definition.PowerWords.String.ToLower() : ""; }
			else if ( slice == 5 ){ value = reagents; }
			else if ( slice == 6 ){ value = definition != null ? definition.Description.String : ""; }
			else if ( slice == 7 ){ value = definition != null ? definition.ManaCost.ToString() : "0"; }
			else if ( slice == 8 ){ value = definition != null ? definition.MinSkill.ToString() : "0"; }
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
			else if ( slice == 11 ){ value = definition != null ? definition.IconGraphic.ToString() : "0"; }
			else { value = definition != null ? definition.SpellID.ToString() : "0"; }

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
				SpellDefinition definition = Server.Spells.Research.ResearchSpellProvider.GetDefinitionByResearchIndex( spell );
				if ( definition != null )
					Server.Spells.Research.ResearchSpellProvider.Cast( from, definition.SpellID );
			}
		}
	}
}