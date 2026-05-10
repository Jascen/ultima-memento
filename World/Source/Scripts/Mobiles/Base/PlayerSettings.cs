using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Server.Misc
{
    class PlayerSettings
    {
		public static int GetMySpellHue( bool mod, Mobile m, int hue ) // ----------------------------------------------------------------------------------------
		{
			if ( m is PlayerMobile )
			{
				var from = (PlayerMobile)m;
				if ( from.Preferences.MagerySpellHue > 0 )
				{
					return mod ? from.Preferences.MagerySpellHue - 1 : from.Preferences.MagerySpellHue;
				}
			}
			return hue;
		}

		public static bool GetWanted( Mobile m ) // -------------------------------------------------------------------------------------------------
		{
			string wanted = ((PlayerMobile)m).CharacterWanted;

			bool isWanted = true;

			if ( wanted == null ){ isWanted = false; }

			return isWanted;
		}

		public static void SetSavage( Mobile m ) // -------------------------------------------------------------------------------------------------
		{
			var player = m as PlayerMobile;
			if (player == null) return;

			PlayerSettings.SetDiscovered( m, "the Savaged Empire", true );
			player.SetCharacterType( CharacterType.Savage );
			Server.Misc.MorphingTime.RemoveMyClothes( m );

			if ( m.Female )
			{
				player.Preferences.CharacterEvil = false;
				player.Preferences.CharacterOriental = false;
				player.Preferences.CharacterBarbaric = 2;
			}
			else
			{
				player.Preferences.CharacterEvil = false;
				player.Preferences.CharacterOriental = false;
				player.Preferences.CharacterBarbaric = 1;
			}
			Server.Items.BarbaricSatchel.GivePack( m );

			BaseArmor hat = new LeatherCap();
			hat.Resource = CraftResource.DinosaurLeather;
			hat.Hue = 0xB61;
			hat.Name = "savage cap";
			hat.StrRequirement = 10;
			hat.ItemID = 0x5648;
			m.AddItem( hat );

			BaseArmor gloves = new LeatherGloves();
			gloves.Resource = CraftResource.DinosaurLeather;
			gloves.Hue = 0xB61;
			gloves.Name = "savage gloves";
			gloves.StrRequirement = 10;
			gloves.ItemID = 0x564E;
			m.AddItem( gloves );

			BaseArmor chest = new LeatherChest();
			chest.Resource = CraftResource.DinosaurLeather;
			chest.Hue = 0xB61;
			chest.Name = "savage tunic";
			chest.StrRequirement = 10;
			chest.ItemID = 0x5651;
			m.AddItem( chest );

			BaseArmor boots = new LeatherBoots();
			boots.Resource = CraftResource.DinosaurLeather;
			boots.Hue = 0xB61;
			boots.Name = "savage sandals";
			boots.StrRequirement = 10;
			boots.ItemID = 0x170d;
			m.AddItem( boots );

			BaseArmor pants = new LeatherLegs();
			pants.Resource = CraftResource.DinosaurLeather;
			pants.Hue = 0xB61;
			pants.Name = "savage skirt";
			pants.StrRequirement = 10;
			pants.ItemID = 0x1C08;
			m.AddItem( pants );

			BaseArmor bracers = new LeatherArms();
			bracers.Resource = CraftResource.DinosaurLeather;
			bracers.Hue = 0xB61;
			bracers.Name = "savage bracers";
			bracers.StrRequirement = 10;
			bracers.ItemID = 0x564D;
			m.AddItem( bracers );

			SavageTalisman talisman = player.Avatar.Active ? new SavageTalisman(50, 50) : new SavageTalisman();
			talisman.ItemOwner = m;

			m.AddItem( talisman );

			BaseWeapon dagger = new Dagger();
			dagger.Resource = CraftResource.Steel;
			m.AddItem( dagger );

			m.AddToBackpack( new Gold( 400 ) );
			m.AddToBackpack( new LambLeg( 15 ) );
			m.AddToBackpack( new Bandage( 100 ) );
			m.AddToBackpack( new CulinarySet() );

			CampersTent tent = new CampersTent();
			tent.Charges = 50;
			m.AddToBackpack( tent );
		}

		public static void SetSpaceMan( Mobile m ) // -------------------------------------------------------------------------------------------------
		{
			Point3D loc = new Point3D( 7000, 4000, 0 );
			m.MoveToWorld( loc, Map.Lodor );

			((PlayerMobile)m).SetCharacterType( CharacterType.Alien );
			Server.Misc.MorphingTime.RemoveMyClothes( m );

			List<Item> contents = new List<Item>();
			foreach( Item i in m.Backpack.Items )
			{
				contents.Add(i);
			}
			foreach ( Item item in contents )
			{
				item.Delete();
			}

			for( int i = 0; i < m.Skills.Length; i++ )
			{
				Skill skill = (Skill)m.Skills[i];
				skill.Base = 0;
			}

			m.AddItem( new FancyShirt() );
			m.AddItem( new Boots() );
			m.AddItem( new LongPants() );

			BaseWeapon dagger = new Dagger();
			dagger.Name = "knife";
			m.AddItem( dagger );

			Item meat = new CookedBird( 10 );
			meat.Hue = 0xB64;
			meat.Name = "cooked alien meat";
			m.AddToBackpack( meat );

			Item water = new Pitcher( BeverageType.Water );
			water.ItemID = 0x4971;
			water.Name = "empty canteen";
			m.AddToBackpack( water );

			MedicalRecord record = new MedicalRecord();
			record.DataPatient = m.Name;
			m.AddToBackpack( record );

			loc = new Point3D( 4109, 3775, 2 );
			m.MoveToWorld( loc, Map.Sosaria );
		}

		public static void SetWanted( Mobile m ) // -------------------------------------------------------------------------------------------------
		{
			string wName = NameList.RandomName( "male" );
			string wTitle = "King";
			string wPron = "he";

			switch ( Utility.RandomMinMax( 0, 5 ) )
			{
				case 0: wTitle = "Emperor"; break;
				case 1: wTitle = "Duke"; break;
				case 2: wTitle = "Earl"; break;
				case 3: wTitle = "Baron"; break;
				case 4: wTitle = "King"; break;
				case 5: wTitle = "Prince"; break;
			}

			if ( Utility.RandomMinMax( 0, 2 ) == 2 ) 
			{
				wName = NameList.RandomName( "female" );
				wPron = "she";

				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0: wTitle = "Empress"; break;
					case 1: wTitle = "Duchess"; break;
					case 2: wTitle = "Countess"; break;
					case 3: wTitle = "Baroness"; break;
					case 4: wTitle = "Queen"; break;
					case 5: wTitle = "Princess"; break;
				}
			} 

			((PlayerMobile)m).CharacterWanted = m.Name + " is wanted for the murder of " + wTitle + " " + wName + ". The " + wTitle + " was attacked while " + wPron + " was visting " + RandomThings.GetRandomCity() + ". Citizens stated they seen " + m.Name + " leaving the area with a blood covered " + Server.Misc.RandomThings.GetRandomWeapon() + ". The guard captain " + QuestCharacters.ParchmentWriter() + " warns all citizen to be on the lookout for " + m.Name + " as they escaped their jail cell in Britain.";
			int words = Utility.RandomMinMax( 1, 3 );
			if ( words == 2 ) 
			{
				((PlayerMobile)m).CharacterWanted = m.Name + " is wanted for the murder of " + wTitle + " " + wName + ". The " + wTitle + " was attacked while " + wPron + " was visting " + RandomThings.GetRandomCity() + ". " + m.Name + " also stole " + Server.Misc.QuestCharacters.QuestItems( true ) + " that the " + wTitle + " had with them. The guard captain " + QuestCharacters.ParchmentWriter() + " warns all citizen to be on the lookout for " + m.Name + " as they escaped their jail cell in Britain.";
			}
			else if ( words == 3 ) 
			{
				((PlayerMobile)m).CharacterWanted = m.Name + " is wanted for the murder of " + wTitle + " " + wName + ". The " + wTitle + " was assassinated by orders from a group calling themselves " + RandomThings.GetRandomSociety() + ". " + m.Name + " was hired by them to carry out the deed, but their motivations remain unclear. The guard captain " + QuestCharacters.ParchmentWriter() + " warns all citizen to be on the lookout for " + m.Name + " as they escaped their jail cell in Britain.";
			}

			m.Profile = ((PlayerMobile)m).CharacterWanted;
			SetWantedState( (PlayerMobile)m );

			GuardNote note = new GuardNote();
			note.ScrollText = ((PlayerMobile)m).CharacterWanted;
			m.AddToBackpack( note );
		}

		public static void SetWantedState( PlayerMobile m ) // -------------------------------------------------------------------------------------------------
		{
			SetBardsTaleQuest( m, "BardsTaleWin", true );
			m.SetCharacterType( CharacterType.Fugitive );
			m.Kills = 1;
			m.Fugitive = 1;
		}

		public static string GetWantedStory( Mobile m ) // -------------------------------------------------------------------------------------------------
		{
			string wanted = ((PlayerMobile)m).CharacterWanted;

			return wanted;
		}

		public static bool GetQuestState( Mobile m, string quest ) // -------------------------------------------------------------------------------
		{
			PlayerSettings.MarkQuestInfo( m );

			var quests = ((PlayerMobile)m).Quests;
			string goal = quests.StandardQuest;	

			if ( quest == "StandardQuest" ){ goal = quests.StandardQuest; }
			else if ( quest == "FishingQuest" ){ goal = quests.FishingQuest; }
			else if ( quest == "AssassinQuest" ){ goal = quests.AssassinQuest; }
			else if ( quest == "MessageQuest" ){ goal = quests.MessageQuest; }
			else if ( quest == "ThiefQuest" ){ goal = quests.ThiefQuest; }

			int nEntry = 1;

			if ( goal.Length > 0 )
			{
				string[] goals = goal.Split('#');
				foreach (string goalz in goals)
				{
					nEntry++;
				}
			}

			if ( nEntry > 3 ){ return true; }

			return false;
		}

		public static int GetGumpHue( Mobile m ) // -------------------------------------------------------------------------------------------------
		{
			if (m is PlayerMobile)
			{
				PlayerSettings.MarkQuestInfo( m );

				if ( ((PlayerMobile)m).Preferences.GumpHue > 0 ){ return 0; }
			}

			return 2999;
		}

		public static bool AutoOpenWepBar( Mobile m ) // ---------------------------------------------------------------------------------------------
		{
			if ( m is PlayerMobile )
			{
				PlayerSettings.MarkQuestInfo( m );

				if ( ((PlayerMobile)m).Preferences.WeaponBarOpen ){ return true; }
			}

			return false;
		}

		public static string GetQuestInfo( Mobile m, string quest ) // ------------------------------------------------------------------------------
		{
			PlayerSettings.MarkQuestInfo( m );

			var quests = ((PlayerMobile)m).Quests;
			string goal = quests.StandardQuest;	

			if ( quest == "StandardQuest" ){ goal = quests.StandardQuest; }
			else if ( quest == "FishingQuest" ){ goal = quests.FishingQuest; }
			else if ( quest == "AssassinQuest" ){ goal = quests.AssassinQuest; }
			else if ( quest == "MessageQuest" ){ goal = quests.MessageQuest; }
			else if ( quest == "ThiefQuest" ){ goal = quests.ThiefQuest; }

			return goal;
		}

		public static void SetQuestInfo( Mobile m, string quest, string setting ) // ----------------------------------------------------------------
		{
			PlayerSettings.MarkQuestInfo( m );

			var quests = ((PlayerMobile)m).Quests;
			if ( quest == "StandardQuest" ){ quests.StandardQuest = setting; }
			else if ( quest == "FishingQuest" ){ quests.FishingQuest = setting; }
			else if ( quest == "AssassinQuest" ){ quests.AssassinQuest = setting; }
			else if ( quest == "MessageQuest" ){ quests.MessageQuest = setting; }
			else if ( quest == "ThiefQuest" ){ quests.ThiefQuest = setting; }
		}

		public static void ClearQuestInfo( Mobile m, string quest ) // ------------------------------------------------------------------------------
		{
			PlayerSettings.MarkQuestInfo( m );

			var quests = ((PlayerMobile)m).Quests;
			if ( quest == "StandardQuest" ){ quests.StandardQuest = ""; }
			else if ( quest == "FishingQuest" ){ quests.FishingQuest = ""; }
			else if ( quest == "AssassinQuest" ){ quests.AssassinQuest = ""; }
			else if ( quest == "MessageQuest" ){ quests.MessageQuest = ""; }
			else if ( quest == "ThiefQuest" ){ quests.ThiefQuest = ""; }
		}

		public static void MarkQuestInfo( Mobile m ) // ---------------------------------------------------------------------------------------------
		{
			if (m is PlayerMobile == false) return;
			
			var quests = ((PlayerMobile)m).Quests;
			if ( quests.StandardQuest == null ){ quests.StandardQuest = ""; }
			if ( quests.FishingQuest == null ){ quests.FishingQuest = ""; }
			if ( quests.AssassinQuest == null ){ quests.AssassinQuest = ""; }
			if ( quests.MessageQuest == null ){ quests.MessageQuest = ""; }
			if ( quests.ThiefQuest == null ){ quests.ThiefQuest = ""; }
		}

		public static Land GetRandomDiscoveredLand( PlayerMobile m, List<Land> options, Func<Land, bool> predicate )
		{
			if ( m == null ) return Land.None;

			options = options
				.Where(land => ( predicate != null && predicate(land) ) || GetDiscovered(m, land))
				.ToList();
			if ( options.Count == 0 ) return Land.None;

            return Utility.Random(options);
        }

		public static bool GetDiscovered( Mobile m, string world ) // -------------------------------------------------------------------------------
		{
			return GetDiscovered(m, Lands.LandRef(world));
		}

		public static bool GetDiscovered( Mobile m, Land land ) // -------------------------------------------------------------------------------
		{
			SetDiscovered( m, "none", false );
			string discovered = ((PlayerMobile)m).CharacterDiscovered;

			if ( discovered.Length > 0 )
			{
				string[] discoveries = discovered.Split('#');
				int nEntry = 1;
				foreach (string found in discoveries)
				{
					if ( nEntry == 1 && found == "1" && land == Land.Lodoria ) return true;
					else if ( nEntry == 2 && found == "1" && land == Land.Sosaria ) return true;
					else if ( nEntry == 3 && found == "1" && land == Land.UmberVeil ) return true;
					else if ( nEntry == 4 && found == "1" && land == Land.Ambrosia ) return true;
					else if ( nEntry == 5 && found == "1" && land == Land.Serpent ) return true;
					else if ( nEntry == 6 && found == "1" && land == Land.IslesDread ) return true;
					else if ( nEntry == 7 && found == "1" && land == Land.Savaged ) return true;
					else if ( nEntry == 8 && found == "1" && land == Land.Kuldar ) return true;
					else if ( nEntry == 9 && found == "1" && land == Land.Underworld ) return true;

					nEntry++;
				}
			}

			return false;
		}

		public static void SetDiscovered( Mobile m, string world, bool repeat ) // ------------------------------------------------------------------
		{
			string discovered = ((PlayerMobile)m).CharacterDiscovered;
			int records = 9; // TOTAL ENTRIES

			if ( discovered == null ){ discovered = "0#0#0#0#0#0#0#0#0#"; }

			if ( discovered.Length > 0 )
			{
				string[] discoveries = discovered.Split('#');
				string entry = "";
				int nEntry = 1;

				foreach ( string lands in discoveries )
				{
					if ( nEntry == 1 && world == "the Land of Lodoria" ){ entry = entry + "1#"; }
					else if ( nEntry == 2 && world == "the Land of Sosaria" ){ entry = entry + "1#"; }
					else if ( nEntry == 3 && world == "the Island of Umber Veil" ){ entry = entry + "1#"; }
					else if ( nEntry == 4 && world == "the Land of Ambrosia" ){ entry = entry + "1#"; }
					else if ( nEntry == 5 && world == "the Serpent Island" ){ entry = entry + "1#"; }
					else if ( nEntry == 6 && world == "the Isles of Dread" ){ entry = entry + "1#"; }
					else if ( nEntry == 7 && world == "the Savaged Empire" ){ entry = entry + "1#"; }
					else if ( nEntry == 8 && world == "the Bottle World of Kuldar" ){ entry = entry + "1#"; }
					else if ( nEntry == 9 && world == "the Underworld" ){ entry = entry + "1#"; }
					else if ( nEntry == 1 ){ entry = entry + lands + "#"; }
					else if ( nEntry == 2 ){ entry = entry + lands + "#"; }
					else if ( nEntry == 3 ){ entry = entry + lands + "#"; }
					else if ( nEntry == 4 ){ entry = entry + lands + "#"; }
					else if ( nEntry == 5 ){ entry = entry + lands + "#"; }
					else if ( nEntry == 6 ){ entry = entry + lands + "#"; }
					else if ( nEntry == 7 ){ entry = entry + lands + "#"; }
					else if ( nEntry == 8 ){ entry = entry + lands + "#"; }
					else if ( nEntry == 9 ){ entry = entry + lands + "#"; }
					else if ( nEntry == 10 ){ entry = entry + lands + "#"; }

					nEntry++;
				}

				while ( nEntry < records+1 )
				{
					entry = entry + "0#";
					nEntry++;
				}

				((PlayerMobile)m).CharacterDiscovered = entry;

				if ( repeat ){ SetDiscovered( m, world, false ); }
			}
		}

		public static string ValArtyConfig( Mobile m ) // ----------------------------------------------------------------------------------------------
		{
			string val = "";

			if ( m is PlayerMobile )
			{
				val = m.StolenArtifacts;

				if ( val == null || val == "" )
					m.StolenArtifacts = val = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#";
			}
			return val;
		}

		public static bool GetArtyConfig( Mobile m, int row ) // ---------------------------------------------------------------------------------------
		{
			bool isSET = false;
						
			if ( m != null && m is PlayerMobile )
			{
				string keys = ValArtyConfig( m );

				if ( keys.Length > 0 )
				{
					string[] configures = keys.Split('#');
					int nEntry = 1;
					foreach (string key in configures)
					{
						if ( nEntry == row && key == "1" ){ isSET = true; }

						nEntry++;
					}
				}
			}
		
			return isSET;
		}

		public static void SetArtyConfig( Mobile m, int key ) // --------------------------------------------------------------------------
		{
			if ( m != null && m is PlayerMobile )
			{
				string keys = ValArtyConfig( m );
				int records = 23; // TOTAL ENTRIES
				string entry = "";
				int nEntry = 1;

				if ( keys.Length > 0 )
				{
					string[] configures = keys.Split('#');
					foreach ( string keyset in configures )
					{
						if ( records > 0 )
						{
							records--;

							string sets = "1";
							if ( keyset != "1" ){ sets = "0"; } 

							if ( nEntry == key && sets == "1" ){ entry = entry + "0#"; }
							else if ( nEntry == key && sets == "0" ){ entry = entry + "1#"; }
							else { entry = entry + sets + "#"; }

							nEntry++;
						}
					}
				}

				while ( records > 0 )
				{
					entry = entry + "0#";
					records--;
				}

				m.StolenArtifacts = entry;
			}
		}

		public static string ValReagentConfig( Mobile m ) // ----------------------------------------------------------------------------------------------
		{
			string val = "";

			if ( m is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)m;
				val = pm.Preferences.RegBar;

				if ( val == null || val == "" )
					pm.Preferences.RegBar = val = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#";
			}
			return val;
		}

		public static bool GetReagentConfig( Mobile m, int row ) // ---------------------------------------------------------------------------------------
		{
			bool isSET = false;
						
			if ( m != null && m is PlayerMobile )
			{
				string keys = ValReagentConfig( m );

				if ( keys.Length > 0 )
				{
					string[] configures = keys.Split('#');
					int nEntry = 1;
					foreach (string key in configures)
					{
						if ( nEntry == row && key == "1" ){ isSET = true; }

						nEntry++;
					}
				}
			}
		
			return isSET;
		}

		public static void SetReagentConfig( Mobile m, int key ) // --------------------------------------------------------------------------
		{
			if ( m != null && m is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)m;

				string keys = ValReagentConfig( m );
				int records = 36; // TOTAL ENTRIES
				string entry = "";
				int nEntry = 1;

				if ( keys.Length > 0 )
				{
					string[] configures = keys.Split('#');
					foreach ( string keyset in configures )
					{
						if ( records > 0 )
						{
							records--;

							string sets = "1";
							if ( keyset != "1" ){ sets = "0"; } 

							if ( nEntry == key && sets == "1" ){ entry = entry + "0#"; }
							else if ( nEntry == key && sets == "0" ){ entry = entry + "1#"; }
							else { entry = entry + sets + "#"; }

							nEntry++;
						}
					}
				}

				while ( records > 0 )
				{
					entry = entry + "0#";
					records--;
				}

				pm.Preferences.RegBar = entry;
			}
		}

		public static string ValQuickConfig( Mobile m ) // ----------------------------------------------------------------------------------------------
		{
			string val = "";

			if ( m is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)m;
				val = pm.Preferences.QuickBar;

				if ( val == null || val == "" )
					pm.Preferences.QuickBar = val = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#";
			}
			return val;
		}

		public static bool GetQuickConfig( Mobile m, int row ) // ---------------------------------------------------------------------------------------
		{
			bool isSET = false;
						
			if ( m != null && m is PlayerMobile )
			{
				string keys = ValQuickConfig( m );

				if ( keys.Length > 0 )
				{
					string[] configures = keys.Split('#');
					int nEntry = 1;
					foreach (string key in configures)
					{
						if ( nEntry == row && key == "1" ){ isSET = true; }

						nEntry++;
					}
				}
			}
		
			return isSET;
		}

		public static void SetQuickConfig( Mobile m, int key ) // --------------------------------------------------------------------------
		{
			if ( m != null && m is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)m;

				string keys = ValQuickConfig( m );
				int records = 41; // TOTAL ENTRIES
				string entry = "";
				int nEntry = 1;

				if ( keys.Length > 0 )
				{
					string[] configures = keys.Split('#');
					foreach ( string keyset in configures )
					{
						if ( records > 0 )
						{
							records--;
							string sets = "1";
							if ( keyset != "1" ){ sets = "0"; } 

							if ( nEntry == key && sets == "1" ){ entry = entry + "0#"; }
							else if ( nEntry == key && sets == "0" ){ entry = entry + "1#"; }
							else { entry = entry + sets + "#"; }

							nEntry++;
						}
					}
				}

				while ( records > 0 )
				{
					entry = entry + "0#";
					records--;
				}

				pm.Preferences.QuickBar = entry;
			}
		}

		public static string ValLibraryConfig( Mobile m ) // ----------------------------------------------------------------------------------------------
		{
			string val = "";

			if ( m is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)m;
				val = pm.Preferences.MyLibrary;

				if ( val == null || val == "" )
					pm.Preferences.MyLibrary = val = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#";
			}
			return val;
		}

		public static bool GetLibraryConfig( Mobile m, int row ) // ---------------------------------------------------------------------------------------
		{
			bool isSET = false;
						
			if ( m != null && m is PlayerMobile )
			{
				string keys = ValLibraryConfig( m );

				if ( keys.Length > 0 )
				{
					string[] configures = keys.Split('#');
					int nEntry = 1;
					foreach (string key in configures)
					{
						if ( nEntry == row && key == "1" ){ isSET = true; }

						nEntry++;
					}
				}
			}
		
			return isSET;
		}
		public static void SetLibraryConfig( Mobile m, int key ) // --------------------------------------------------------------------------
		{
			if ( m != null && m is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)m;

				string keys = ValLibraryConfig( m );
				int records = 76; // TOTAL ENTRIES
				string entry = "";
				int nEntry = 1;

				if ( keys.Length > 0 )
				{
					string[] configures = keys.Split('#');
					foreach ( string keyset in configures )
					{
						if ( records > 0 )
						{
							records--;
							string sets = "1";
							if ( keyset != "1" ){ sets = "0"; } 

							if ( nEntry == key && sets == "1" ){ entry = entry + "0#"; }
							else if ( nEntry == key && sets == "0" ){ entry = entry + "1#"; }
							else { entry = entry + sets + "#"; }

							nEntry++;
						}
					}
				}

				while ( records > 0 )
				{
					entry = entry + "0#";
					records--;
				}

				pm.Preferences.MyLibrary = entry;
			}
		}


		public static string ValChatConfig( Mobile m ) // ----------------------------------------------------------------------------------------------
		{
			string val = "";

			if ( m is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)m;
				val = pm.Preferences.MyChat;

				if ( val == null || val == "" )
					pm.Preferences.MyChat = val = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#";
			}
			return val;
		}

		public static bool GetChatConfig( Mobile m, int row ) // ---------------------------------------------------------------------------------------
		{
			bool isSET = false;
						
			if ( m != null && m is PlayerMobile )
			{
				string keys = ValChatConfig( m );

				if ( keys.Length > 0 )
				{
					string[] configures = keys.Split('#');
					int nEntry = 1;
					foreach (string key in configures)
					{
						if ( nEntry == row && key == "1" ){ isSET = true; }

						nEntry++;
					}
				}
			}
		
			return isSET;
		}
		public static void SetChatConfig( Mobile m, int key ) // --------------------------------------------------------------------------
		{
			if ( m != null && m is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)m;

				string keys = ValChatConfig( m );
				int records = 47; // TOTAL ENTRIES
				string entry = "";
				int nEntry = 1;

				if ( keys.Length > 0 )
				{
					string[] configures = keys.Split('#');
					foreach ( string keyset in configures )
					{
						if ( records > 0 )
						{
							records--;
							string sets = "1";
							if ( keyset != "1" ){ sets = "0"; } 

							if ( nEntry == key && sets == "1" ){ entry = entry + "0#"; }
							else if ( nEntry == key && sets == "0" ){ entry = entry + "1#"; }
							else { entry = entry + sets + "#"; }

							nEntry++;
						}
					}
				}

				while ( records > 0 )
				{
					entry = entry + "0#";
					records--;
				}

				pm.Preferences.MyChat = entry;
			}
		}

		public static bool GetKeys( Mobile m, string key ) // ---------------------------------------------------------------------------------------
		{
			bool HaveIt = false;
						
			if ( m != null && m is PlayerMobile )
			{
				SetKeys( m, "none", false );
			
				string keys = ((PlayerMobile)m).CharacterKeys;

				if ( keys.Length > 0 )
				{
					string[] discoveries = keys.Split('#');
					int nEntry = 1;
					foreach (string found in discoveries)
					{
						if ( nEntry == 1 && found == "1" && key == "UndermountainKey" ){ HaveIt = true; }
						else if ( nEntry == 2 && found == "1" && key == "BlackKnightKey" ){ HaveIt = true; }
						else if ( nEntry == 3 && found == "1" && key == "RangerOutpost" ){ HaveIt = true; }
						else if ( nEntry == 4 && found == "1" && key == "VordoKey" ){ HaveIt = true; }
						else if ( nEntry == 5 && found == "1" && key == "SkullGate" ){ HaveIt = true; }
						else if ( nEntry == 6 && found == "1" && key == "SerpentPillars" ){ HaveIt = true; }
						else if ( nEntry == 7 && found == "1" && key == "Antiques" ){ HaveIt = true; }
						else if ( nEntry == 8 && found == "1" && key == "Museums" ){ HaveIt = true; }
						else if ( nEntry == 9 && found == "1" && key == "Runes" ){ HaveIt = true; }
						else if ( nEntry == 10 && found == "1" && key == "Virtue" ){ HaveIt = true; }
						else if ( nEntry == 11 && found == "1" && key == "Corrupt" ){ HaveIt = true; }
						else if ( nEntry == 12 && found == "1" && key == "Gygax" ){ HaveIt = true; }
						else if ( nEntry == 13 && found == "1" && key == "DragonRiding" ){ HaveIt = true; }

						nEntry++;
					}
				}
			}
		
			return HaveIt;
		}

		public static void SetKeys( Mobile m, string key, bool repeat ) // --------------------------------------------------------------------------
		{
			if ( m != null && m is PlayerMobile )
			{
				string keys = ((PlayerMobile)m).CharacterKeys;

				if ( keys == null ){ keys = "0#0#0#0#0#0#0#0#0#0#0#0#0#"; }

				if ( keys.Length > 0 )
				{
					string[] discoveries = keys.Split('#');
					string entry = "";
					int nEntry = 1;
					int records = 13; // TOTAL ENTRIES

					foreach ( string keyset in discoveries )
					{
						string sets = "1";
						if ( keyset != "1" ){ sets = "0"; } 
						if ( nEntry == 1 && key == "UndermountainKey" ){ entry = entry + "1#"; }
						else if ( nEntry == 2 && key == "BlackKnightKey" ){ entry = entry + "1#"; }
						else if ( nEntry == 3 && key == "RangerOutpost" ){ entry = entry + "1#"; }
						else if ( nEntry == 4 && key == "VordoKey" ){ entry = entry + "1#"; }
						else if ( nEntry == 5 && key == "SkullGate" ){ entry = entry + "1#"; }
						else if ( nEntry == 6 && key == "SerpentPillars" ){ entry = entry + "1#"; }
						else if ( nEntry == 7 && key == "Antiques" ){ entry = entry + "1#"; }
						else if ( nEntry == 8 && key == "Museums" ){ entry = entry + "1#"; }
						else if ( nEntry == 9 && key == "Runes" ){ entry = entry + "1#"; }
						else if ( nEntry == 10 && key == "Virtue" ){ entry = entry + "1#"; }
						else if ( nEntry == 11 && key == "Corrupt" ){ entry = entry + "1#"; }
						else if ( nEntry == 12 && key == "Gygax" ){ entry = entry + "1#"; }
						else if ( nEntry == 13 && key == "DragonRiding" ){ entry = entry + "1#"; }

						else if ( nEntry == 1 ){ entry = entry + sets + "#"; }
						else if ( nEntry == 2 ){ entry = entry + sets + "#"; }
						else if ( nEntry == 3 ){ entry = entry + sets + "#"; }
						else if ( nEntry == 4 ){ entry = entry + sets + "#"; }
						else if ( nEntry == 5 ){ entry = entry + sets + "#"; }
						else if ( nEntry == 6 ){ entry = entry + sets + "#"; }
						else if ( nEntry == 7 ){ entry = entry + sets + "#"; }
						else if ( nEntry == 8 ){ entry = entry + sets + "#"; }
						else if ( nEntry == 9 ){ entry = entry + sets + "#"; }
						else if ( nEntry == 10 ){ entry = entry + sets + "#"; }
						else if ( nEntry == 11 ){ entry = entry + sets + "#"; }
						else if ( nEntry == 12 ){ entry = entry + sets + "#"; }
						else if ( nEntry == 13 ){ entry = entry + sets + "#"; }

						nEntry++;
					}

					while ( nEntry < records+1 )
					{
						entry = entry + "0#";
						nEntry++;
					}

					((PlayerMobile)m).CharacterKeys = entry;

					if ( repeat ){ SetKeys( m, key, false ); }
				}
			}
		}

		public static bool GetSpecialsKilled( Mobile m, string who ) // ---------------------------------------------------------------------------------------
		{
			SetSpecialsKilled( m, "none", false );
			string whos = ((PlayerMobile)m).KilledSpecialMonsters;

			bool AlreadyDid = false;

			if ( whos.Length > 0 )
			{
				string[] enemies = whos.Split('#');
				int nEntry = 1;
				foreach (string found in enemies)
				{
					if ( nEntry == 1 && found == "1" && who == "Arachnar" ){ AlreadyDid = true; }
					else if ( nEntry == 2 && found == "1" && who == "BlackGateDemon" ){ AlreadyDid = true; }
					else if ( nEntry == 3 && found == "1" && who == "BloodDemigod" ){ AlreadyDid = true; }
					else if ( nEntry == 4 && found == "1" && who == "Xurtzar" ){ AlreadyDid = true; }
					else if ( nEntry == 5 && found == "1" && who == "CaddelliteDragon" ){ AlreadyDid = true; }
					else if ( nEntry == 6 && found == "1" && who == "DragonKing" ){ AlreadyDid = true; }
					else if ( nEntry == 7 && found == "1" && who == "Vulcrum" ){ AlreadyDid = true; }
					else if ( nEntry == 8 && found == "1" && who == "OrkDemigod" ){ AlreadyDid = true; }
					else if ( nEntry == 9 && found == "1" && who == "Mangar" ){ AlreadyDid = true; }
					else if ( nEntry == 10 && found == "1" && who == "Astaroth" ){ AlreadyDid = true; }
					else if ( nEntry == 11 && found == "1" && who == "Faulinei" ){ AlreadyDid = true; }
					else if ( nEntry == 12 && found == "1" && who == "Nosfentor" ){ AlreadyDid = true; }
					else if ( nEntry == 13 && found == "1" && who == "Tarjan" ){ AlreadyDid = true; }
					else if ( nEntry == 14 && found == "1" && who == "Dracula" ){ AlreadyDid = true; }
					else if ( nEntry == 15 && found == "1" && who == "LichKing" ){ AlreadyDid = true; }
					else if ( nEntry == 16 && found == "1" && who == "Surtaz" ){ AlreadyDid = true; }
					else if ( nEntry == 17 && found == "1" && who == "TitanLithos" ){ AlreadyDid = true; }
					else if ( nEntry == 18 && found == "1" && who == "TitanPyros" ){ AlreadyDid = true; }
					else if ( nEntry == 19 && found == "1" && who == "TitanHydros" ){ AlreadyDid = true; }
					else if ( nEntry == 20 && found == "1" && who == "TitanStatos" ){ AlreadyDid = true; }
					else if ( nEntry == 21 && found == "1" && who == "Jormungandr" ){ AlreadyDid = true; }
					else if ( nEntry == 22 && found == "1" && who == "Exodus" ){ AlreadyDid = true; }

					nEntry++;
				}
			}

			return AlreadyDid;
		}

		public static void SetSpecialsKilled( Mobile m, string who, bool repeat ) // ----------------------------------------------------------------
		{
			string whos = ((PlayerMobile)m).KilledSpecialMonsters;
			int records = 22; // TOTAL ENTRIES

			if ( whos == null ){ whos = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }

			if ( whos.Length > 0 )
			{
				string[] enemies = whos.Split('#');
				string entry = "";
				int nEntry = 1;

				foreach ( string killed in enemies )
				{
					if ( nEntry == 1 && who == "Arachnar" ){ entry = entry + "1#"; }
					else if ( nEntry == 2 && who == "BlackGateDemon" ){ entry = entry + "1#"; }
					else if ( nEntry == 3 && who == "BloodDemigod" ){ entry = entry + "1#"; }
					else if ( nEntry == 4 && who == "Xurtzar" ){ entry = entry + "1#"; }
					else if ( nEntry == 5 && who == "CaddelliteDragon" ){ entry = entry + "1#"; }
					else if ( nEntry == 6 && who == "DragonKing" ){ entry = entry + "1#"; }
					else if ( nEntry == 7 && who == "Vulcrum" ){ entry = entry + "1#"; }
					else if ( nEntry == 8 && who == "OrkDemigod" ){ entry = entry + "1#"; }
					else if ( nEntry == 9 && who == "Mangar" ){ entry = entry + "1#"; }
					else if ( nEntry == 10 && who == "Astaroth" ){ entry = entry + "1#"; }
					else if ( nEntry == 11 && who == "Faulinei" ){ entry = entry + "1#"; }
					else if ( nEntry == 12 && who == "Nosfentor" ){ entry = entry + "1#"; }
					else if ( nEntry == 13 && who == "Tarjan" ){ entry = entry + "1#"; }
					else if ( nEntry == 14 && who == "Dracula" ){ entry = entry + "1#"; }
					else if ( nEntry == 15 && who == "LichKing" ){ entry = entry + "1#"; }
					else if ( nEntry == 16 && who == "Surtaz" ){ entry = entry + "1#"; }
					else if ( nEntry == 17 && who == "TitanLithos" ){ entry = entry + "1#"; }
					else if ( nEntry == 18 && who == "TitanPyros" ){ entry = entry + "1#"; }
					else if ( nEntry == 19 && who == "TitanHydros" ){ entry = entry + "1#"; }
					else if ( nEntry == 20 && who == "TitanStatos" ){ entry = entry + "1#"; }
					else if ( nEntry == 21 && who == "Jormungandr" ){ entry = entry + "1#"; }
					else if ( nEntry == 22 && who == "Exodus" ){ entry = entry + "1#"; }

					else if ( nEntry == 1 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 2 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 3 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 4 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 5 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 6 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 7 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 8 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 9 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 10 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 11 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 12 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 13 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 14 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 15 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 16 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 17 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 18 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 19 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 20 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 21 ){ entry = entry + killed + "#"; }
					else if ( nEntry == 22 ){ entry = entry + killed + "#"; }

					nEntry++;
				}

				while ( nEntry < records+1 )
				{
					entry = entry + "0#";
					nEntry++;
				}

				((PlayerMobile)m).KilledSpecialMonsters = entry;

				if ( repeat ){ SetSpecialsKilled( m, who, false ); }
			}
		}

		public static bool GetBardsTaleQuest( Mobile m, string part ) // -----------------------------------------------------------------------------
		{
			SetBardsTaleQuest( m, "none", false );
			string quest = ((PlayerMobile)m).Quests.BardsTaleQuest;

			string[] quests = quest.Split('#');
			int nEntry = 1;
			foreach (string goal in quests)
			{
				if ( nEntry == 1 && goal == "1" && part == "BardsTaleMadGodName" ){ return true; }
				else if ( nEntry == 2 && goal == "1" && part == "BardsTaleCatacombKey" ){ return true; }
				else if ( nEntry == 3 && goal == "1" && part == "BardsTaleEbonyKey" ){ return true; }
				else if ( nEntry == 4 && goal == "1" && part == "BardsTaleKylearanKey" ){ return true; }
				else if ( nEntry == 5 && goal == "1" && part == "BardsTaleHarkynKey" ){ return true; }
				else if ( nEntry == 6 && goal == "1" && part == "BardsTaleDragonKey" ){ return true; }
				else if ( nEntry == 7 && goal == "1" && part == "BardsTaleSpectreEye" ){ return true; }
				else if ( nEntry == 8 && goal == "1" && part == "BardsTaleCrystalSword" ){ return true; }
				else if ( nEntry == 9 && goal == "1" && part == "BardsTaleSilverSquare" ){ return true; }
				else if ( nEntry == 10 && goal == "1" && part == "BardsTaleBedroomKey" ){ return true; }
				else if ( nEntry == 11 && goal == "1" && part == "BardsTaleSilverTriangle" ){ return true; }
				else if ( nEntry == 12 && goal == "1" && part == "BardsTaleCrystalGolem" ){ return true; }
				else if ( nEntry == 13 && goal == "1" && part == "BardsTaleSilverCircle" ){ return true; }
				else if ( nEntry == 14 && goal == "1" && part == "BardsTaleMangarKey" ){ return true; }
				else if ( nEntry == 15 && goal == "1" && part == "BardsTaleWin" ){ return true; }

				nEntry++;
			}

			return false;
		}

		public static void SetBardsTaleQuest( Mobile m, string part, bool repeat ) // ---------------------------------------------------------------
		{
			string quest = ((PlayerMobile)m).Quests.BardsTaleQuest;
			int records = 15; // TOTAL ENTRIES

			if ( quest == null ){ quest = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }

			if ( quest.Length > 0 )
			{
				string[] quests = quest.Split('#');
				string entry = "";
				int nEntry = 1;
				int Finished = 0;

				foreach ( string goal in quests )
				{
					if ( nEntry == 1 && part == "BardsTaleMadGodName" ){ entry = entry + "1#"; }
					else if ( nEntry == 2 && part == "BardsTaleCatacombKey" ){ entry = entry + "1#"; }
					else if ( nEntry == 3 && part == "BardsTaleEbonyKey" ){ entry = entry + "1#"; }
					else if ( nEntry == 4 && part == "BardsTaleKylearanKey" ){ entry = entry + "1#"; }
					else if ( nEntry == 5 && part == "BardsTaleHarkynKey" ){ entry = entry + "1#"; }
					else if ( nEntry == 6 && part == "BardsTaleDragonKey" ){ entry = entry + "1#"; }
					else if ( nEntry == 7 && part == "BardsTaleSpectreEye" ){ entry = entry + "1#"; }
					else if ( nEntry == 8 && part == "BardsTaleCrystalSword" ){ entry = entry + "1#"; }
					else if ( nEntry == 9 && part == "BardsTaleSilverSquare" ){ entry = entry + "1#"; }
					else if ( nEntry == 10 && part == "BardsTaleBedroomKey" ){ entry = entry + "1#"; }
					else if ( nEntry == 11 && part == "BardsTaleSilverTriangle" ){ entry = entry + "1#"; }
					else if ( nEntry == 12 && part == "BardsTaleCrystalGolem" ){ entry = entry + "1#"; }
					else if ( nEntry == 13 && part == "BardsTaleSilverCircle" ){ entry = entry + "1#"; }
					else if ( nEntry == 14 && part == "BardsTaleMangarKey" ){ entry = entry + "1#"; }
					else if ( nEntry == 15 && part == "BardsTaleWin" ){ entry = entry + "1#"; Finished = 1; }

					else if ( nEntry == 1 ){ entry = entry + goal + "#"; }
					else if ( nEntry == 2 ){ entry = entry + goal + "#"; }
					else if ( nEntry == 3 ){ entry = entry + goal + "#"; }
					else if ( nEntry == 4 ){ entry = entry + goal + "#"; }
					else if ( nEntry == 5 ){ entry = entry + goal + "#"; }
					else if ( nEntry == 6 ){ entry = entry + goal + "#"; }
					else if ( nEntry == 7 ){ entry = entry + goal + "#"; }
					else if ( nEntry == 8 ){ entry = entry + goal + "#"; }
					else if ( nEntry == 9 ){ entry = entry + goal + "#"; }
					else if ( nEntry == 10 ){ entry = entry + goal + "#"; }
					else if ( nEntry == 11 ){ entry = entry + goal + "#"; }
					else if ( nEntry == 12 ){ entry = entry + goal + "#"; }
					else if ( nEntry == 13 ){ entry = entry + goal + "#"; }
					else if ( nEntry == 14 ){ entry = entry + goal + "#"; }
					else if ( nEntry == 15 ){ entry = entry + goal + "#"; }

					nEntry++;
				}

				while ( nEntry < records+1 )
				{
					entry = entry + "0#";
					nEntry++;
				}

				((PlayerMobile)m).Quests.BardsTaleQuest = entry;

				if ( Finished > 0 ){ ((PlayerMobile)m).Quests.BardsTaleQuest = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#1#"; }

				if ( repeat ){ SetBardsTaleQuest( m, part, false ); }
			}
		}

		public static void LootContainer( Mobile m, Container box ) // -------------------------------------------------------------------------------------
		{
			var player = m as PlayerMobile;
			if (player == null) return;

			string looting = player.Preferences.CharacterLoot;

			if ( looting == null ){ Server.Misc.LootChoiceUpdates.InitializeLootChoice( m ); looting = player.Preferences.CharacterLoot; }

			if ( looting.Length > 0 )
			{
				int foundCoins = 0;
				int foundNuggets = 0;
				int foundGems = 0;
				int foundJewels = 0;
				int foundArrows = 0;
				int foundBolts = 0;
				int foundBandages = 0;
				int foundScrolls = 0;
				int foundReagents = 0;
				int foundPotions = 0;

				List<Item> belongings = new List<Item>();

				string[] discoveries = looting.Split('#');
				int nEntry = 1;
				foreach (string found in discoveries)
				{
					if ( nEntry == 1 && found == "1" )
					{
						foreach( Item i in box.Items )
						{
							if ( i is Gold ){ belongings.Add(i); foundCoins = 1; }
							else if ( i is DDCopper ){ belongings.Add(i); foundCoins = 1; }
							else if ( i is DDSilver ){ belongings.Add(i); foundCoins = 1; }
							else if ( i is DDXormite ){ belongings.Add(i); foundCoins = 1; }
							else if ( i is DDGoldNuggets ){ belongings.Add(i); foundNuggets = 1; }
						}
					}
					else if ( nEntry == 2 && found == "1" )
					{
						foreach( Item i in box.Items )
						{
							if ( i is StarSapphire ){ belongings.Add(i); foundGems = 1; }
							else if ( i is Emerald ){ belongings.Add(i); foundGems = 1; }
							else if ( i is Sapphire ){ belongings.Add(i); foundGems = 1; }
							else if ( i is Ruby ){ belongings.Add(i); foundGems = 1; }
							else if ( i is Citrine ){ belongings.Add(i); foundGems = 1; }
							else if ( i is Amethyst ){ belongings.Add(i); foundGems = 1; }
							else if ( i is Oyster ){ belongings.Add(i); foundGems = 1; }
							else if ( i is Tourmaline ){ belongings.Add(i); foundGems = 1; }
							else if ( i is Tourmaline ){ belongings.Add(i); foundGems = 1; }
							else if ( i is Amber ){ belongings.Add(i); foundGems = 1; }
							else if ( i is Crystals ){ belongings.Add(i); foundGems = 1; }
							else if ( i is Diamond ){ belongings.Add(i); foundGems = 1; }
							else if ( i is DDGemstones ){ belongings.Add(i); foundGems = 1; }
							else if ( i is DDJewels ){ belongings.Add(i); foundJewels = 1; }
						}
					}
					else if ( nEntry == 3 && found == "1" )
					{
						foreach( Item i in box.Items )
						{
							if ( i is Arrow ){ belongings.Add(i); foundArrows = 1; }
							else if ( i is ManyArrows100 ){ belongings.Add(i); foundArrows = 1; }
							else if ( i is ManyArrows1000 ){ belongings.Add(i); foundArrows = 1; }
							else if ( i is Bolt ){ belongings.Add(i); foundBolts = 1; }
							else if ( i is ManyBolts100 ){ belongings.Add(i); foundBolts = 1; }
							else if ( i is ManyBolts1000 ){ belongings.Add(i); foundBolts = 1; }
						}
					}
					else if ( nEntry == 4 && found == "1" )
					{
						foreach( Item i in box.Items )
						{
							if ( i is Elemental_Armor_Scroll || i is Elemental_Bolt_Scroll || i is Elemental_Mend_Scroll || 
							i is Elemental_Sanctuary_Scroll || i is Elemental_Pain_Scroll || i is Elemental_Protection_Scroll || 
							i is Elemental_Purge_Scroll || i is Elemental_Steed_Scroll || i is Elemental_Call_Scroll || 
							i is Elemental_Force_Scroll || i is Elemental_Wall_Scroll || i is Elemental_Warp_Scroll || 
							i is Elemental_Field_Scroll || i is Elemental_Restoration_Scroll || i is Elemental_Strike_Scroll || 
							i is Elemental_Void_Scroll || i is Elemental_Blast_Scroll || i is Elemental_Echo_Scroll || 
							i is Elemental_Fiend_Scroll || i is Elemental_Hold_Scroll || i is Elemental_Barrage_Scroll || 
							i is Elemental_Rune_Scroll || i is Elemental_Storm_Scroll || i is Elemental_Summon_Scroll || 
							i is Elemental_Devastation_Scroll || i is Elemental_Fall_Scroll || i is Elemental_Gate_Scroll || 
							i is Elemental_Havoc_Scroll || i is Elemental_Apocalypse_Scroll || i is Elemental_Lord_Scroll || 
							i is Elemental_Soul_Scroll || i is Elemental_Spirit_Scroll )
							{ belongings.Add(i); foundScrolls = 1; }
						}
					}
					else if ( nEntry == 5 && found == "1" )
					{
						foreach( Item i in box.Items )
						{
							if ( i is Bandage ){ belongings.Add(i); foundBandages = 1; }
						}
					}
					else if ( nEntry == 6 && found == "1" )
					{
						foreach( Item i in box.Items )
						{
							if ( i is ReactiveArmorScroll || i is ClumsyScroll || i is CreateFoodScroll || i is FeeblemindScroll || 
							 i is HealScroll || i is MagicArrowScroll || i is NightSightScroll || i is WeakenScroll || 
							 i is AgilityScroll || i is CunningScroll || i is CureScroll || i is HarmScroll || 
							 i is MagicTrapScroll || i is MagicUnTrapScroll || i is ProtectionScroll || i is StrengthScroll || 
							 i is BlessScroll || i is FireballScroll || i is MagicLockScroll || i is PoisonScroll || 
							 i is TelekinisisScroll || i is TeleportScroll || i is UnlockScroll || i is WallOfStoneScroll || 
							 i is ArchCureScroll || i is ArchProtectionScroll || i is CurseScroll || i is FireFieldScroll || 
							 i is GreaterHealScroll || i is LightningScroll || i is ManaDrainScroll || i is RecallScroll || 
							 i is BladeSpiritsScroll || i is DispelFieldScroll || i is IncognitoScroll || i is MagicReflectScroll || 
							 i is MindBlastScroll || i is ParalyzeScroll || i is PoisonFieldScroll || i is SummonCreatureScroll || 
							 i is DispelScroll || i is EnergyBoltScroll || i is ExplosionScroll || i is InvisibilityScroll || 
							 i is MarkScroll || i is MassCurseScroll || i is ParalyzeFieldScroll || i is RevealScroll || 
							 i is ChainLightningScroll || i is EnergyFieldScroll || i is FlamestrikeScroll || i is GateTravelScroll || 
							 i is ManaVampireScroll || i is MassDispelScroll || i is MeteorSwarmScroll || i is PolymorphScroll || 
							 i is EarthquakeScroll || i is EnergyVortexScroll || i is ResurrectionScroll || i is SummonAirElementalScroll || 
							 i is SummonDaemonScroll || i is SummonEarthElementalScroll || i is SummonFireElementalScroll || i is SummonWaterElementalScroll )
							{ belongings.Add(i); foundScrolls = 1; }
						}
					}
					else if ( nEntry == 7 && found == "1" )
					{
						foreach( Item i in box.Items )
						{
							if ( i is AnimateDeadScroll || i is BloodOathScroll || i is CorpseSkinScroll || i is CurseWeaponScroll || 
							 i is EvilOmenScroll || i is HorrificBeastScroll || i is LichFormScroll || i is MindRotScroll || 
							 i is PainSpikeScroll || i is PoisonStrikeScroll || i is StrangleScroll || i is SummonFamiliarScroll || 
							 i is VampiricEmbraceScroll || i is VengefulSpiritScroll || i is WitherScroll || i is WraithFormScroll || 
							 i is ExorcismScroll )
							{ belongings.Add(i); foundScrolls = 1; }
						}
					}
					else if ( nEntry == 8 && found == "1" )
					{
						foreach( Item i in box.Items )
						{
							if ( i is BlackPearl ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is Bloodmoss ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is Garlic ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is Ginseng ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is MandrakeRoot ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is Nightshade ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is SpidersSilk ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is SulfurousAsh ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is reagents_magic_jar1 ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is ReagentJar ){ belongings.Add(i); foundReagents = 1; }
						}
					}
					else if ( nEntry == 9 && found == "1" )
					{
						foreach( Item i in box.Items )
						{
							if ( i is BatWing ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is DaemonBlood ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is PigIron ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is NoxCrystal ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is GraveDust ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is reagents_magic_jar1 ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is reagents_magic_jar2 ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is reagents_magic_jar3 ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is ReagentJar ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is EyeOfToad ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is BeetleShell ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is GargoyleEar ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is RedLotus ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is SwampBerries ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is SilverWidow ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is MoonCrystal ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is BlackPearl ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is Bloodmoss ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is Brimstone ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is PixieSkull ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is BitterRoot ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is BlackSand ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is BloodRose ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is DriedToad ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is Maggot ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is MummyWrap ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is VioletFungus ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is WerewolfClaw ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is Wolfsbane ){ belongings.Add(i); foundReagents = 1; }
						}
					}
					else if ( nEntry == 10 && found == "1" )
					{
						foreach( Item i in box.Items )
						{
							if ( i is NotIdentified && i.NotIDSource == Identity.Reagent ){ belongings.Add(i); foundReagents = 1; }
						}
					}
					else if ( nEntry == 11 && found == "1" )
					{
						foreach( Item i in box.Items )
						{
							if ( i is BasePotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is AutoResPotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is ShieldOfEarthPotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is WoodlandProtectionPotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is ProtectiveFairyPotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is HerbalHealingPotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is GraspingRootsPotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is BlendWithForestPotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is SwarmOfInsectsPotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is VolcanicEruptionPotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is TreefellowPotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is StoneCirclePotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is DruidicRunePotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is LureStonePotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is NaturesPassagePotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is MushroomGatewayPotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is RestorativeSoilPotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is FireflyPotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is HellsGateScroll ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is ManaLeechScroll ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is NecroCurePoisonScroll ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is NecroPoisonScroll ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is NecroUnlockScroll ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is PhantasmScroll ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is RetchedAirScroll ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is SpectreShadowScroll ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is UndeadEyesScroll ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is VampireGiftScroll ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is WallOfSpikesScroll ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is BloodPactScroll ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is GhostlyImagesScroll ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is GhostPhaseScroll ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is GraveyardGatewayScroll ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is HellsBrandScroll ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is MagicalDyes ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is UnusualDyes ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is BottleOfAcid ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is CrystallineJar ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is NecroSkinPotion ){ belongings.Add(i); foundPotions = 1; }
							else if ( i is TransmutationPotion ){ belongings.Add(i); foundPotions = 1; }
						}
					}
					else if ( nEntry == 12 && found == "1" )
					{
						foreach( Item i in box.Items )
						{
							if ( i is NotIdentified && i.NotIDSource == Identity.Potion ){ belongings.Add(i); foundPotions = 1; }
						}
					}
					else if ( nEntry == 13 && found == "1" )
					{
						foreach( Item i in box.Items )
						{
							if ( i is ArmysPaeonScroll ){ belongings.Add(i); foundScrolls = 1; }
							else if ( i is EnchantingEtudeScroll ){ belongings.Add(i); foundScrolls = 1; }
							else if ( i is EnergyCarolScroll ){ belongings.Add(i); foundScrolls = 1; }
							else if ( i is EnergyThrenodyScroll ){ belongings.Add(i); foundScrolls = 1; }
							else if ( i is FireCarolScroll ){ belongings.Add(i); foundScrolls = 1; }
							else if ( i is FireThrenodyScroll ){ belongings.Add(i); foundScrolls = 1; }
							else if ( i is FoeRequiemScroll ){ belongings.Add(i); foundScrolls = 1; }
							else if ( i is IceCarolScroll ){ belongings.Add(i); foundScrolls = 1; }
							else if ( i is IceThrenodyScroll ){ belongings.Add(i); foundScrolls = 1; }
							else if ( i is KnightsMinneScroll ){ belongings.Add(i); foundScrolls = 1; }
							else if ( i is MagesBalladScroll ){ belongings.Add(i); foundScrolls = 1; }
							else if ( i is MagicFinaleScroll ){ belongings.Add(i); foundScrolls = 1; }
							else if ( i is PoisonCarolScroll ){ belongings.Add(i); foundScrolls = 1; }
							else if ( i is PoisonThrenodyScroll ){ belongings.Add(i); foundScrolls = 1; }
							else if ( i is SheepfoeMamboScroll ){ belongings.Add(i); foundScrolls = 1; }
							else if ( i is SinewyEtudeScroll ){ belongings.Add(i); foundScrolls = 1; }
						}
					}
					else if ( nEntry == 14 && found == "1" )
					{
						foreach( Item i in box.Items )
						{
							if ( i is NotIdentified && i.NotIDSource == Identity.Scroll ){ belongings.Add(i); foundScrolls = 1; }
						}
					}
					else if ( nEntry == 15 && found == "1" )
					{
						foreach( Item i in box.Items )
						{
							if ( i is EyeOfToad ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is FairyEgg ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is GargoyleEar ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is BeetleShell ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is MoonCrystal ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is PixieSkull ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is RedLotus ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is SeaSalt ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is SilverWidow ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is SwampBerries ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is Brimstone ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is ButterflyWings ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is reagents_magic_jar3 ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is ReagentJar ){ belongings.Add(i); foundReagents = 1; }
						}
					}
					else if ( nEntry == 16 && found == "1" )
					{
						foreach( Item i in box.Items )
						{
							if ( i is BlackPearl ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is Ginseng ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is SwampBerries ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is Garlic ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is BeetleShell ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is MandrakeRoot ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is EyeOfToad ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is RedLotus ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is Nightshade ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is ButterflyWings ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is SpidersSilk ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is SulfurousAsh ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is Brimstone ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is Bloodmoss ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is FairyEgg ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is SeaSalt ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is MoonCrystal ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is SilverWidow ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is reagents_magic_jar3 ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is reagents_magic_jar1 ){ belongings.Add(i); foundReagents = 1; }
							else if ( i is ReagentJar ){ belongings.Add(i); foundReagents = 1; }
						}
					}

					nEntry++;
				}

				int sound = 0;
				foreach ( Item stuff in belongings )
				{
					sound = 1;
					BaseContainer.PutStuffInContainer( m, 1, stuff );
					//m.AddToBackpack( stuff );
				}

				if ( sound > 0 )
				{
					m.PlaySound( 0x048 );

					string sMessage = "You take some ";

					if ( foundCoins > 0 ){ sMessage = sMessage + "coins, "; }
					if ( foundGems > 0 ){ sMessage = sMessage + "gems, "; }
					if ( foundNuggets > 0 ){ sMessage = sMessage + "nuggets, "; }
					if ( foundJewels > 0 ){ sMessage = sMessage + "jewels, "; }
					if ( foundArrows > 0 ){ sMessage = sMessage +" arrows, "; }
					if ( foundBolts > 0 ){ sMessage = sMessage + "bolts, "; }
					if ( foundBandages > 0 ){ sMessage = sMessage + "bandages, "; }
					if ( foundScrolls > 0 ){ sMessage = sMessage + "scrolls, "; }
					if ( foundReagents > 0 ){ sMessage = sMessage + "reagents, "; }
					if ( foundPotions > 0 ){ sMessage = sMessage + "potions, "; }

					sMessage = sMessage + "and put them in your pack.";

					m.SendMessage( sMessage );
				}
			}

			Server.Gumps.RegBar.RefreshRegBar( m );
			Server.Gumps.QuickBar.RefreshQuickBar( m );
			Server.Gumps.WealthBar.RefreshWealthBar( m );
		}
	}
}