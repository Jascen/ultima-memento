using System;
using Server.Network;
using Server.Gumps;
using Server.Mobiles;
using System.Collections.Generic;
using System.Collections;
using Server.Misc;
using Server.Regions; 

namespace Server.Items
{
	public class MuseumBook : Item
	{
		public static bool IsEnabled()
		{
			return true;
		}

		[Constructable]
		public MuseumBook() : base( 0x1AA3 )
		{
			Hue = 0xB71;
			Name = "Tome of Antiques";

			if ( ArtOwner == null )
			{
				KnightTrophy = Utility.RandomMinMax( 1, 56 );
				KnightStatue = Utility.RandomMinMax( 1, 56 );
				RumorWorld = Land.None;
				RumorDungeon = "";
				RumorGoal = 0;
				RumorFrom = "";
				ItemValue = 0;
				ItemsFound = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#";
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			Container pack = from.Backpack;

			if ( ArtOwner == null )
			{
				ArtOwner = from;
			}

			if ( from != ArtOwner )
			{
				from.SendMessage( "The book doesn't belong to you." );
			}
			else
			{
				from.CloseGump( typeof( MuseumBookGump ) );
				from.SendGump( new MuseumBookGump( from, this, 1, GetNext( this ) ) );
			}
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			if ( ArtOwner != null ){ list.Add( 1070722, "Belongs to " + ArtOwner.Name + "" ); }
			if ( GetTotal( this ) > 0 ){ list.Add( 1049644, "Found " + GetTotal( this ) + " out of 60 Antiques" ); }
        }

		public MuseumBook( Serial serial ) : base( serial )
		{
		}

		public Mobile ArtOwner;
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Art_Owner { get{ return ArtOwner; } set{ ArtOwner = value; } }

		public int KnightTrophy;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Knight_Trophy { get{ return KnightTrophy; } set{ KnightTrophy = value; } }

		public int KnightStatue;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Knight_Statue { get{ return KnightStatue; } set{ KnightStatue = value; } }

		public Land RumorWorld;
		[CommandProperty( AccessLevel.GameMaster )]
		public Land Rumor_World { get{ return RumorWorld; } set{ RumorWorld = value; } }

		public string RumorDungeon;
		[CommandProperty( AccessLevel.GameMaster )]
		public string Rumor_Dungeon { get{ return RumorDungeon; } set{ RumorDungeon = value; } }

		public int RumorGoal;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Rumor_Goal { get{ return RumorGoal; } set{ RumorGoal = value; } }

		public string RumorFrom;
		[CommandProperty( AccessLevel.GameMaster )]
		public string Rumor_From { get{ return RumorFrom; } set{ RumorFrom = value; } }

		public int ItemValue;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Item_Value { get{ return ItemValue; } set{ ItemValue = value; } }

		public string ItemsFound;
		[CommandProperty( AccessLevel.GameMaster )]
		public string Items_Found { get{ return ItemsFound; } set{ ItemsFound = value; } }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 ); // version
			writer.Write( (Mobile)ArtOwner );
			writer.Write( KnightTrophy );
			writer.Write( KnightStatue );
			writer.Write( (int)RumorWorld );
			writer.Write( RumorDungeon );
			writer.Write( RumorGoal );
			writer.Write( RumorFrom );
			writer.Write( ItemValue );
			writer.Write( ItemsFound );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			ArtOwner = reader.ReadMobile();
			KnightTrophy = reader.ReadInt();
			KnightStatue = reader.ReadInt();

			if ( version < 1 )
				RumorWorld = Server.Lands.LandRef( reader.ReadString() );
			else
				RumorWorld = (Land)(reader.ReadInt());

			RumorDungeon = reader.ReadString();
			RumorGoal = reader.ReadInt();
			RumorFrom = reader.ReadString();
			ItemValue = reader.ReadInt();
			ItemsFound = reader.ReadString();
		}

		public static int GetTotal( MuseumBook book )
		{
			string keys = book.ItemsFound;
			int value = 0;

			if ( keys.Length > 0 )
			{
				string[] antiques = keys.Split('#');
				int nEntry = 1;

				foreach ( string found in antiques )
				{
					if ( found == "1" ){ value++; }
					nEntry++;
				}
			}
			return value;
		}

		public static void SetInventory( MuseumBook book, int key )
		{
			string keys = book.ItemsFound;

			if ( keys.Length > 0 )
			{
				string[] discoveries = keys.Split('#');
				string entry = "";
				int nEntry = 1;

				foreach ( string item in discoveries )
				{
					if ( nEntry == key ){ entry = entry + "1#"; }
					else { entry = entry + item + "#"; }

					nEntry++;
				}

				book.ItemsFound = entry;
				book.InvalidateProperties();
			}
		}

		public static bool FoundItem( Mobile player, int type )
		{
			Item item = player.Backpack.FindItemByType( typeof ( MuseumBook ) );
			MuseumBook book = (MuseumBook)item;

			if ( type == book.RumorGoal && book.RumorDungeon == Server.Misc.Worlds.GetRegionName( player.Map, player.Location ) && book.ArtOwner == player && GetNext( book ) < 100 )
			{
				if ( Utility.RandomMinMax( 1, 3 ) != 1 )
				{
					int thing = GetNext( book );
					string say = AntiqueInfo( thing, 4, book );

					Museums relic = new Museums();
					relic.ItemID = Int32.Parse( AntiqueInfo( thing, 1, book ) );
					relic.Hue = Int32.Parse( AntiqueInfo( thing, 3, book ) );

					if ( relic.ItemID == 0x4FA4 ){ ColorMetal( relic, 0 ); }
					else if ( relic.ItemID == 0x4FC9 ){ ColorMetal( relic, 0 ); }
					else if ( relic.ItemID == 0x5328 ){ relic.Hue = Utility.RandomList( 0, 0x4A7, 0x747, 0x96C, 0x7DA, 0x415, 0x908, 0x712, 0x1CD, 0x9C2, 0x843, 0x750, 0xA94, 0x973, 0xA3A ); }
					else if ( relic.ItemID == 0x4FA9 ){ ColorMetal( relic, 0 ); }
					else if ( relic.ItemID == 0x4FBD ){ ColorMetal( relic, 0 ); }
					else if ( relic.ItemID == 0x4FBB ){ ColorMetal( relic, 0 ); }
					else if ( relic.ItemID == 0x4FBE ){ ColorMetal( relic, 0 ); }
					else if ( relic.ItemID == 0x4FA7 ){ ColorMetal( relic, 0 ); }
					else if ( relic.ItemID == 0x4FC2 ){ ColorMetal( relic, 0 ); }
					else if ( relic.ItemID == 0x4FC6 ){ ColorMetal( relic, 0 ); }
					else if ( relic.ItemID == 0x0FF5 ){ ColorMetal( relic, 0 ); }
					else if ( relic.ItemID == 0x4B45 ){ ColorMetal( relic, 0 ); }
					else if ( relic.ItemID == 0x4FA6 ){ ColorMetal( relic, 0 ); }
					else if ( relic.ItemID == 0x4FAB ){ ColorMetal( relic, 0 ); }
					else if ( relic.ItemID == 0x5327 ){ relic.Hue = Utility.RandomList( 0, 0x4A7, 0x747, 0x96C, 0x7DA, 0x415, 0x908, 0x712, 0x1CD, 0x9C2, 0x843, 0x750, 0xA94, 0x973, 0xA3A ); }
					else if ( relic.ItemID == 0x47E6 ){ ColorMetal( relic, 0 ); }
					else if ( relic.ItemID == 0x530A ){ relic.Hue = Utility.RandomColor(0); }

					relic.Name = AntiqueInfo( thing, 4, book );
					relic.DiscoverName = player.Name;
					relic.DiscoverOwner = player;
					relic.ThisDescription = AntiqueInfo( thing, 5, book );
					relic.ThisValue = book.ItemValue;

					if ( AntiqueInfo( thing, 6, book ) == "1" ){ relic.Light = LightType.Circle150; }
					else if ( AntiqueInfo( thing, 6, book ) == "2" ){ relic.Light = LightType.Circle300; }
					player.AddToBackpack( relic );

					player.LocalOverheadMessage(MessageType.Emote, 1150, true, "You found the " + say + ".");
					player.SendSound( 0x5B4 );
					book.RumorWorld = Land.None;
					book.RumorDungeon = "";
					book.RumorGoal = 0;
					book.RumorFrom = "";
					book.ItemValue = 0;
					SetInventory( book, thing );

					return true;
				}
				else
				{
					player.LocalOverheadMessage(MessageType.Emote, 1150, true, book.RumorFrom + " was either wrong or they lied.");
					player.SendSound( 0x5B3 );
					book.RumorWorld = Land.None;
					book.RumorDungeon = "";
					book.RumorGoal = 0;
					book.RumorFrom = "";
					book.ItemValue = 0;
					return false;
				}
			}

			return false;
		}

		public static void ColorMetal( Item item, int color )
		{
			if ( color < 1 ){ color = Utility.RandomMinMax( 0, 37 ); }

			switch ( color ) 
			{
				case 1: item.Hue = CraftResources.GetHue( CraftResource.StarRubyBlock );	item.Name = (CraftResources.GetName( CraftResource.StarRubyBlock )).ToLower() + " " + item.Name;	break;
				case 2: item.Hue = CraftResources.GetHue( CraftResource.SpinelBlock );		item.Name = (CraftResources.GetName( CraftResource.SpinelBlock )).ToLower() + " " + item.Name;		break;
				case 3: item.Hue = CraftResources.GetHue( CraftResource.SilverBlock );		item.Name = (CraftResources.GetName( CraftResource.SilverBlock )).ToLower() + " " + item.Name;		break;
				case 4: item.Hue = CraftResources.GetHue( CraftResource.SapphireBlock );	item.Name = (CraftResources.GetName( CraftResource.SapphireBlock )).ToLower() + " " + item.Name;	break;
				case 5: item.Hue = CraftResources.GetHue( CraftResource.RubyBlock );		item.Name = (CraftResources.GetName( CraftResource.RubyBlock )).ToLower() + " " + item.Name;		break;
				case 6: item.Hue = CraftResources.GetHue( CraftResource.QuartzBlock );		item.Name = (CraftResources.GetName( CraftResource.QuartzBlock )).ToLower() + " " + item.Name;		break;
				case 7: item.Hue = CraftResources.GetHue( CraftResource.OnyxBlock );		item.Name = (CraftResources.GetName( CraftResource.OnyxBlock )).ToLower() + " " + item.Name;		break;
				case 8: item.Hue = CraftResources.GetHue( CraftResource.JadeBlock );		item.Name = (CraftResources.GetName( CraftResource.JadeBlock )).ToLower() + " " + item.Name;		break;
				case 9: item.Hue = CraftResources.GetHue( CraftResource.GarnetBlock );		item.Name = (CraftResources.GetName( CraftResource.GarnetBlock )).ToLower() + " " + item.Name;		break;
				case 10: item.Hue = CraftResources.GetHue( CraftResource.EmeraldBlock );	item.Name = (CraftResources.GetName( CraftResource.EmeraldBlock )).ToLower() + " " + item.Name;		break;
				case 11: item.Hue = CraftResources.GetHue( CraftResource.AmethystBlock );	item.Name = (CraftResources.GetName( CraftResource.AmethystBlock )).ToLower() + " " + item.Name;	break;
				case 12: item.Hue = 0x47E; item.Name = "pearl " + item.Name;		break;
				case 13: item.Hue = CraftResources.GetHue( CraftResource.Obsidian );		item.Name = (CraftResources.GetName( CraftResource.Obsidian )).ToLower() + " " + item.Name;	break;
				case 14: item.Hue = CraftResources.GetHue( CraftResource.DullCopper );		item.Name = (CraftResources.GetName( CraftResource.DullCopper )).ToLower() + " " + item.Name;		break;
				case 15: item.Hue = CraftResources.GetHue( CraftResource.ShadowIron );		item.Name = (CraftResources.GetName( CraftResource.ShadowIron )).ToLower() + " " + item.Name;		break;
				case 16: item.Hue = CraftResources.GetHue( CraftResource.Copper );			item.Name = (CraftResources.GetName( CraftResource.Copper )).ToLower() + " " + item.Name;			break;
				case 17: item.Hue = CraftResources.GetHue( CraftResource.Bronze );			item.Name = (CraftResources.GetName( CraftResource.Bronze )).ToLower() + " " + item.Name;			break;
				case 18: item.Hue = CraftResources.GetHue( CraftResource.Gold );			item.Name = (CraftResources.GetName( CraftResource.Gold )).ToLower() + " " + item.Name;				break;
				case 19: item.Hue = CraftResources.GetHue( CraftResource.Agapite );			item.Name = (CraftResources.GetName( CraftResource.Agapite )).ToLower() + " " + item.Name;			break;
				case 20: item.Hue = CraftResources.GetHue( CraftResource.Verite );			item.Name = (CraftResources.GetName( CraftResource.Verite )).ToLower() + " " + item.Name;			break;
				case 21: item.Hue = CraftResources.GetHue( CraftResource.Valorite );		item.Name = (CraftResources.GetName( CraftResource.Valorite )).ToLower() + " " + item.Name;			break;
				case 22: item.Hue = CraftResources.GetHue( CraftResource.Steel );			item.Name = (CraftResources.GetName( CraftResource.Steel )).ToLower() + " " + item.Name;			break;
				case 23: item.Hue = CraftResources.GetHue( CraftResource.Brass );			item.Name = (CraftResources.GetName( CraftResource.Brass )).ToLower() + " " + item.Name;			break;
				case 24: item.Hue = CraftResources.GetHue( CraftResource.Nepturite );		item.Name = (CraftResources.GetName( CraftResource.Nepturite )).ToLower() + " " + item.Name;		break;
				case 25: item.Hue = CraftResources.GetHue( CraftResource.ShadowIron );		item.Name = (CraftResources.GetName( CraftResource.ShadowIron )).ToLower() + " " + item.Name;		break;
				case 26: item.Hue = 0x486; item.Name = "violet " + item.Name;			break;
				case 27: item.Hue = 0x5B6; item.Name = "azurite " + item.Name;			break;
				case 28: item.Hue = 0x495; item.Name = "turquoise " + item.Name;		break;
				case 29: item.Hue = CraftResources.GetHue( CraftResource.Mithril );			item.Name = (CraftResources.GetName( CraftResource.Mithril )).ToLower() + " " + item.Name;			break;
				case 30: item.Hue = CraftResources.GetHue( CraftResource.CaddelliteBlock );	item.Name = (CraftResources.GetName( CraftResource.CaddelliteBlock )).ToLower() + " " + item.Name;	break;
				case 31: item.Hue = 0x71B; item.Name = "wooden " + item.Name;			break;
				case 32: item.Hue = 0xB92; item.Name = "bone " + item.Name;				break;
				case 33: item.Hue = 0xA61; item.Name = "diamond " + item.Name;			break;
				case 34: item.Hue = 0x54F; item.Name = "amber " + item.Name;			break;
				case 35: item.Hue = 0x550; item.Name = "tourmaline " + item.Name;		break;
				case 36: item.Hue = 0x4F2; item.Name = "star sapphire " + item.Name;	break;
				case 37: item.Hue = CraftResources.GetHue( CraftResource.TopazBlock );		item.Name = (CraftResources.GetName( CraftResource.TopazBlock )).ToLower() + " " + item.Name;		break;
			}
		}

		public static string TellRumor( Mobile player, Citizens citizen )
		{
			string rumor = "";

			if ( citizen.CanTellRumor() )
			{
				MuseumBook book = player.Backpack.FindItemByType( typeof ( MuseumBook ) ) as MuseumBook;
				if ( book != null && book.ArtOwner == player )
				{
					int antique = GetNext( book );

					if ( Utility.RandomMinMax( 1, 10 ) > 1 ){ citizen.MarkToldRumor(); }

					if ( citizen.CanTellRumor() && book.RumorFrom == "" && antique < 100 )
					{
						citizen.MarkToldRumor();
						SetRumor( citizen, book );
						rumor = GetRumor( book, antique, true );
					}
				}
			}

			return rumor;
		}

		public static string GetRumor( MuseumBook book, int item, bool talk )
		{
			int goal = book.RumorGoal;
			string locate = "held by a powerful creature";
			if ( goal == 2 ){ locate = "lost somewhere"; }

			string world = Server.Lands.LandName( book.RumorWorld );
			string dungeon = book.RumorDungeon;
			string from = book.RumorFrom;

			if ( talk )
			{
				string who = "I heard";
				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0:	who = "I heard";																								break;
					case 1:	who = "I learned";																								break;
					case 2:	who = "I found out";																							break;
					case 3:	who = "The " + RandomThings.GetRandomJob() + " in " + RandomThings.GetRandomCity() + " told me";				break;
					case 4:	who = "I overheard some " + RandomThings.GetRandomJob() + " say";												break;
					case 5:	who = "My friend told me";																						break;
				}
				return who + " that the " + MuseumBook.AntiqueInfo( item, 4, book ) + " may be " + locate + " within " + dungeon + " in " + world + ".";
			}

			if ( world != "" ){ return "" + from + " has told you that the " + MuseumBook.AntiqueInfo( item, 4, book ) + " may be " + locate + " within " + dungeon + " in " + world + "."; }

			return "";
		}

		public static void SetRumor( Mobile m, MuseumBook book )
		{
			book.RumorGoal = Utility.RandomMinMax( 1, 2 );
			if ( book.ItemValue < 1 )
			{
				book.ItemValue = Utility.RandomMinMax( 120, 200 ) * 100;
				book.ItemValue = (int)(book.ItemValue * (MyServerSettings.GetGoldCutRate() * .01));
			}

			var options = new List<Land>
			{
				Land.Sosaria,
				Land.Lodoria,
				Land.Serpent,
				Land.Sosaria,
				Land.Lodoria,
				Land.Serpent,
				Land.UmberVeil,
				Land.Ambrosia,
				Land.IslesDread,
				Land.Savaged,
				Land.Kuldar,
			};
			Land searchLocation = Utility.Random(options); // Intentionally anywhere

			string dungeon = "the Dungeon of Doom";

			int aCount = 0;

			ArrayList targets = new ArrayList();

			if ( book.RumorGoal == 1 )
			{
				foreach ( Mobile target in World.Mobiles.Values )
				if ( target.Region is DungeonRegion && target.Fame >= 18000 && !( target is Exodus || target is CodexGargoyleA || target is CodexGargoyleB || target is Syth ) )
				{
					if ( target.Land == searchLocation )
					{
						targets.Add( target );
						aCount++;
					}
				}
			}
			else
			{
				foreach ( Item target in World.Items.Values )
				if ( target is SearchBase || target is StealBase )
				{
					if ( target.Land == searchLocation )
					{
						targets.Add( target );
						aCount++;
					}
				}
			}

			aCount = Utility.RandomMinMax( 1, aCount );

			int xCount = 0;
			for ( int i = 0; i < targets.Count; ++i )
			{
				xCount++;

				if ( xCount == aCount )
				{
					if ( book.RumorGoal == 1 )
					{
						Mobile finding = ( Mobile )targets[ i ];
						dungeon = Server.Misc.Worlds.GetRegionName( finding.Map, finding.Location );
					}
					else
					{
						Item finding = ( Item )targets[ i ];
						dungeon = Server.Misc.Worlds.GetRegionName( finding.Map, finding.Location );
					}
				}
			}

			book.RumorWorld = searchLocation;
			book.RumorDungeon = dungeon;
			book.RumorFrom = "" + m.Name + " " + m.Title + "";
		}

		public static int GetMuseums( int key, MuseumBook book )
		{
			string keys = book.ItemsFound;

			if ( keys.Length > 0 )
			{
				string[] antiques = keys.Split('#');
				int nEntry = 1;
				foreach (string found in antiques)
				{
					if ( nEntry == key ){ return Int32.Parse( found ); }
					nEntry++;
				}
			}

			return 0;
		}

		public static int GetNext( MuseumBook book )
		{
			string keys = book.ItemsFound;
			int item = 0;

			if ( keys.Length > 0 )
			{
				string[] antiques = keys.Split('#');
				foreach (string found in antiques)
				{
					item++;
					if ( found == "0" ){ return item; }
				}
			}

			return 100;
		}

		public static int QuestValue()
		{
			return (int)(400000 * (MyServerSettings.GetGoldCutRate() * .01));
		}

		public static string AntiqueInfo( int item, int slice, MuseumBook book )
		{
			string id = "0";
			string hex = "0";
			string hue = "0";
			string name = "";
			string desc = "";
			string light = "0";

			if ( item == 1 ){ id = "20425"; hex = "0x4FC9"; name = "Dwarven Suit of Armor"; desc = "For Gimli of the Glittering Caves"; }
			else if ( item == 2 ){ id = "8850"; hex = "0x2292"; name = "Boiling Blood"; desc = "Of the Salem Witch"; }
			else if ( item == 3 ){ id = "20411"; hex = "0x4FBB"; name = "Shrine of Scorpius"; desc = "The Foe of Orion"; }
			else if ( item == 4 ){ id = "20414"; hex = "0x4FBE"; name = "Trophy for Jousting"; desc = "Won by " + GetKnight( book.KnightTrophy ); }
			else if ( item == 5 ){ id = "20407"; hex = "0x4FB7"; name = "Mirror of Galadriel"; desc = "From the Land of Middle-Earth"; light = "2"; }
			else if ( item == 6 ){ id = "20403"; hex = "0x4FB3"; name = "Anchor of the Jolly Roger"; desc = "Captain Hook's Ship"; }
			else if ( item == 7 ){ id = "1167"; hex = "0x048F"; name = "Brazier of the Dead"; desc = "Forged in Dungeon Deceit"; light = "2"; }
			else if ( item == 8 ){ id = "20415"; hex = "0x4FBF"; name = "Statue of Triton"; desc = "The Herald of Poseidon"; }
			else if ( item == 9 ){ id = "21187"; hex = "0x52C3"; name = "Banner of Blackbeard"; desc = "Pirate of the Eastern Coast"; }
			else if ( item == 10 ){ id = "8836"; hex = "0x2284"; name = "Brew of Magical Goo"; desc = "Belonged to Griselda the Witch"; }
			else if ( item == 11 ){ id = "21189"; hex = "0x52C5"; name = "Head of Drachen"; desc = "The Sea Serpent King"; }
			else if ( item == 12 ){ id = "20388"; hex = "0x4FA4"; name = "Statue of Razkarok"; desc = "The Beast of Valusia"; }
			else if ( item == 13 ){ id = "21288"; hex = "0x5328"; name = "Axe of Perun"; desc = "Shrine of the God of Lightning"; }
			else if ( item == 14 ){ id = "21355"; hex = "0x536B"; name = "Map of Britannia"; desc = "The Land of the Avatar"; }
			else if ( item == 15 ){ id = "21323"; hex = "0x534B"; name = "Leaves of the Earthmother"; desc = "From the Moonshae Isles"; }
			else if ( item == 16 ){ id = "20401"; hex = "0x4FB1"; name = "Statue of Neptune"; desc = "The Ruler of the Sea"; }
			else if ( item == 17 ){ id = "20393"; hex = "0x4FA9"; name = "Dwarven Statue"; desc = "Thorin Oakenshield"; }
			else if ( item == 18 ){ id = "21276"; hex = "0x531C"; name = "Chariot of Ben-Hur"; desc = "From the Circus Maximus"; }
			else if ( item == 19 ){ id = "21256"; hex = "0x5308"; name = "Astrological Spyglass"; desc = "Of Zodiac the Star Master"; }
			else if ( item == 20 ){ id = "20399"; hex = "0x4FAF"; name = "Head of Tuskrage"; desc = "The Giant Boar of Kuldara"; }
			else if ( item == 21 ){ id = "20429"; hex = "0x4FCD"; name = "Wand of Wonder"; desc = "Belonged to Merlin the Wizard"; light = "2"; }
			else if ( item == 22 ){ id = "8519"; hex = "0x2147"; name = "Symbol of Ultimate Wisdom"; desc = "From the Chamber of the Codex"; }
			else if ( item == 23 ){ id = "20413"; hex = "0x4FBD"; name = "Trophy of David"; desc = "For Defeating the Goliath"; }
			else if ( item == 24 ){ id = "8829"; hex = "0x227D"; name = "Cauldron of the Sea"; desc = "Belonged to Morwen the Hag"; }
			else if ( item == 25 ){ id = "20420"; hex = "0x4FC4"; hue = "0x986"; name = "Statue of a Phoenix"; desc = "The Vermilion Bird"; }
			else if ( item == 26 ){ id = "20384"; hex = "0x4FA0"; name = "Flower of the Fellowship"; desc = "Grown by Batlin the Druid"; }
			else if ( item == 27 ){ id = "20391"; hex = "0x4FA7"; name = "Statue of a Knight"; desc = GetKnight( book.KnightStatue ); }
			else if ( item == 28 ){ id = "21191"; hex = "0x52C7"; name = "Head of Druk"; desc = "The Thunder Dragon"; }
			else if ( item == 29 ){ id = "20387"; hex = "0x4FA3"; name = "Pipe of Mystical Smoke"; desc = "Belonged to Gandalf the Grey"; }
			else if ( item == 30 ){ id = "18080"; hex = "0x46A0"; name = "Cornicopia of Feasting"; desc = "Horn of the Goat Amalthea"; }
			else if ( item == 31 ){ id = "20418"; hex = "0x4FC2"; name = "Altar of Neptune"; desc = "The Ruler of the Sea"; }
			else if ( item == 32 ){ id = "6247"; hex = "0x1867"; name = "Crystal of the Ruby Knight"; desc = "From King Sarak's Vault"; light = "2"; }
			else if ( item == 33 ){ id = "20422"; hex = "0x4FC6"; name = "Statue of Athena"; desc = "The Daughter of Zeus"; }
			else if ( item == 34 ){ id = "21333"; hex = "0x5355"; name = "Desk of Elminster"; desc = "The Sage of Shadowdale"; }
			else if ( item == 35 ){ id = "20386"; hex = "0x4FA2"; name = "Flowers from the Silver River"; desc = "From the Land of Shannara"; }
			else if ( item == 36 ){ id = "732"; hex = "0x02DC"; hue = "0xB1B"; name = "Lamp of the Genie"; desc = "From the Treasure of Aladdin"; }
			else if ( item == 37 ){ id = "21244"; hex = "0x52FC"; name = "Stained Glass Lamp"; desc = "Belonged to Doctor Jekyll";}
			else if ( item == 38 ){ id = "20383"; hex = "0x4F9F"; name = "Mage Flower of D'Hara"; desc = "Grown by Zeddicus Zu'l Zorander"; }
			else if ( item == 39 ){ id = "4085"; hex = "0x0FF5"; name = "Idol of Virtue"; desc = "Symbol of the Avatar"; light = "1"; }
			else if ( item == 40 ){ id = "19724"; hex = "0x4D0C"; name = "Statue of Katalkotl"; desc = "Ruler of the Savaged Empire"; }
			else if ( item == 41 ){ id = "20409"; hex = "0x4FB9"; name = "Looking Glass of Narnia"; desc = "From the Dream World"; light = "2"; }
			else if ( item == 42 ){ id = "15283"; hex = "0x3BB3"; name = "Trophy of Sosaria"; desc = "Awarded by Lord British"; }
			else if ( item == 43 ){ id = "20390"; hex = "0x4FA6"; name = "Eternal Flame"; desc = "Of the Achaemenid Empire"; light = "2"; }
			else if ( item == 44 ){ id = "21395"; hex = "0x5393"; name = "Doomgiver and Soulcutter"; desc = "The Swords of Power"; }
			else if ( item == 45 ){ id = "21280"; hex = "0x5320"; name = "Vulcan's Inferno"; desc = "The God of Volcanic Fire"; }
			else if ( item == 46 ){ id = "21393"; hex = "0x5391"; name = "Serpent of the Stygian"; desc = "Thulsa Doom's Snake"; }
			else if ( item == 47 ){ id = "21208"; hex = "0x52D8"; name = "Head of Xithizil"; desc = "The Insectoid From Another World"; }
			else if ( item == 48 ){ id = "20430"; hex = "0x4FCE"; name = "Sword in the Stone"; desc = "For the King of Camelot"; light = "1"; }
			else if ( item == 49 ){ id = "21332"; hex = "0x5354"; name = "Candelabrum of Zeus"; desc = "From Mount Olympus"; }
			else if ( item == 50 ){ id = "20424"; hex = "0x4FC8"; name = "Brazier of Charon"; desc = "Ferryman of the Dead"; light = "2"; }
			else if ( item == 51 ){ id = "20405"; hex = "0x4FB5"; name = "Magic Mirror"; desc = "Of the Queen Grimhilde"; light = "2"; }
			else if ( item == 52 ){ id = "20395"; hex = "0x4FAB"; name = "Dwarven Shrine"; desc = "For the God Moradin"; }
			else if ( item == 53 ){ id = "8843"; hex = "0x228B"; name = "Cauldron of Serpent Venom"; desc = "Of Ezmerelda the Enchantress"; }
			else if ( item == 54 ){ id = "21347"; hex = "0x5363"; name = "Ancient Stove of Edesia"; desc = "The Goddess of Feasts"; }
			else if ( item == 55 ){ id = "21171"; hex = "0x52B3"; name = "Monument of Death"; desc = "Statue of the Grim Reaper"; }
			else if ( item == 56 ){ id = "20397"; hex = "0x4FAD"; name = "Large Gold Coins"; desc = "From the Cloud Giant's Fortress"; light = "1"; }
			else if ( item == 57 ){ id = "21287"; hex = "0x5327"; name = "Blade of the Seeker"; desc = "Shrine of the Sword of Truth"; }
			else if ( item == 58 ){ id = "21206"; hex = "0x52D6"; name = "Head of Proteus"; desc = "The Lord of the River"; }
			else if ( item == 59 ){ id = "18406"; hex = "0x47E6"; name = "Egg of Stone"; desc = "From the Last Cockatrice"; }
			else if ( item == 60 ){ id = "21258"; hex = "0x530A"; name = "Hourglass of Janus"; desc = "The God of Time"; light = "1"; }

			string value = "";
			if ( slice == 1 ){ value = id; }
			else if ( slice == 2 ){ value = hex; }
			else if ( slice == 3 ){ value = hue; }
			else if ( slice == 4 ){ value = name; }
			else if ( slice == 5 ){ value = desc; }
			else { value = light; }

			return value;
		}

		public static string GetKnight( int knight )
		{
			string statue = "King Arthur";
			if ( knight == 1 ){ statue = "King Arthur"; }
			else if ( knight == 2 ){ statue = "King Bagdemagus"; }
			else if ( knight == 3 ){ statue = "King Leodegrance"; }
			else if ( knight == 4 ){ statue = "King Pellinore"; }
			else if ( knight == 5 ){ statue = "King Uriens"; }
			else if ( knight == 6 ){ statue = "Sir Aban"; }
			else if ( knight == 7 ){ statue = "Sir Abrioris"; }
			else if ( knight == 8 ){ statue = "Sir Adragain"; }
			else if ( knight == 9 ){ statue = "Sir Aglovale"; }
			else if ( knight == 10 ){ statue = "Sir Agravain"; }
			else if ( knight == 11 ){ statue = "Sir Aqiff"; }
			else if ( knight == 12 ){ statue = "Sir Baudwin"; }
			else if ( knight == 13 ){ statue = "Sir Bedivere"; }
			else if ( knight == 14 ){ statue = "Sir Bors"; }
			else if ( knight == 15 ){ statue = "Sir Brastius"; }
			else if ( knight == 16 ){ statue = "Sir Bredbeddle"; }
			else if ( knight == 17 ){ statue = "Sir Breunor"; }
			else if ( knight == 18 ){ statue = "Sir Calogrenant"; }
			else if ( knight == 19 ){ statue = "Sir Caradoc"; }
			else if ( knight == 20 ){ statue = "Sir Constantine"; }
			else if ( knight == 21 ){ statue = "Sir Dagonet"; }
			else if ( knight == 22 ){ statue = "Sir Daniel"; }
			else if ( knight == 23 ){ statue = "Sir Degore"; }
			else if ( knight == 24 ){ statue = "Sir Dinadan"; }
			else if ( knight == 25 ){ statue = "Sir Dornar"; }
			else if ( knight == 26 ){ statue = "Sir Ector"; }
			else if ( knight == 27 ){ statue = "Sir Elyan"; }
			else if ( knight == 28 ){ statue = "Sir Gaheris"; }
			else if ( knight == 29 ){ statue = "Sir Galahad"; }
			else if ( knight == 30 ){ statue = "Sir Galehaut"; }
			else if ( knight == 31 ){ statue = "Sir Galeshin"; }
			else if ( knight == 32 ){ statue = "Sir Gareth"; }
			else if ( knight == 33 ){ statue = "Sir Gawain"; }
			else if ( knight == 34 ){ statue = "Sir Geraint"; }
			else if ( knight == 35 ){ statue = "Sir Gingalain"; }
			else if ( knight == 36 ){ statue = "Sir Griflet"; }
			else if ( knight == 37 ){ statue = "Sir Kay"; }
			else if ( knight == 38 ){ statue = "Sir Lamorak"; }
			else if ( knight == 39 ){ statue = "Sir Lancelot"; }
			else if ( knight == 40 ){ statue = "Sir Lionel"; }
			else if ( knight == 41 ){ statue = "Sir Lucan"; }
			else if ( knight == 42 ){ statue = "Sir Mador"; }
			else if ( knight == 43 ){ statue = "Sir Maleagant"; }
			else if ( knight == 44 ){ statue = "Sir Mordred"; }
			else if ( knight == 45 ){ statue = "Sir Morien"; }
			else if ( knight == 46 ){ statue = "Sir Palamedes"; }
			else if ( knight == 47 ){ statue = "Sir Pelleas"; }
			else if ( knight == 48 ){ statue = "Sir Percival"; }
			else if ( knight == 49 ){ statue = "Sir Pinel"; }
			else if ( knight == 50 ){ statue = "Sir Safir"; }
			else if ( knight == 51 ){ statue = "Sir Sagramore"; }
			else if ( knight == 52 ){ statue = "Sir Segwarides"; }
			else if ( knight == 53 ){ statue = "Sir Tor"; }
			else if ( knight == 54 ){ statue = "Sir Tristan"; }
			else if ( knight == 55 ){ statue = "Sir Ulfius"; }
			else if ( knight == 56 ){ statue = "Sir Yvain"; }

			return statue;
		}
	}
}
