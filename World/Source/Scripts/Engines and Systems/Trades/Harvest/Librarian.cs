using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Misc;
using Server.Network;
using Server.Regions;
using Server.Multis;

namespace Server.Engines.Harvest
{
	public class Librarian : HarvestSystem
	{
		private static Librarian m_System;

		public static Librarian System
		{
			get
			{
				if ( m_System == null )
					m_System = new Librarian();

				return m_System;
			}
		}

		private HarvestDefinition m_Definition;

		public HarvestDefinition Definition
		{
			get{ return m_Definition; }
		}

		private Librarian()
		{
			HarvestResource[] res;
			HarvestVein[] veins;

			#region Librarian
			HarvestDefinition library = new HarvestDefinition();
			library.BankWidth = 1;
			library.BankHeight = 1;
			library.MinTotal = 1;
			library.MaxTotal = 2;
			library.MinRespawn = TimeSpan.FromMinutes( 50.0 );
			library.MaxRespawn = TimeSpan.FromMinutes( 70.0 );
			library.Skill = SkillName.Inscribe;
			library.Tiles = m_LibraryTiles;
			library.MaxRange = 1;
			library.ConsumedPerHarvest = 1 * MyServerSettings.Resources();
			library.ConsumedPerIslesDreadHarvest = library.ConsumedPerHarvest + (int)(library.ConsumedPerHarvest/2) + 2;
			library.EffectActions = new int[]{ 4 };
			library.EffectSounds = new int[]{ 0x55, 0x4F };
			library.EffectCounts = new int[]{ 1 };
			library.EffectDelay = TimeSpan.FromSeconds( 0.0 );
			library.EffectSoundDelay = TimeSpan.FromSeconds( 0.1 );
			library.NoResourcesMessage = "The shelf is completely empty."; // 1060530; // The shelf is completely empty.
			library.FailMessage = "You shuffle around and find nothing of interest."; // 1060531; // You shuffle around and find nothing of interest.
			library.OutOfRangeMessage = "That is too far away."; // 500446; // That is too far away.
			library.PackFullMessage = "You don't have enough room in your backpack!"; // 500720; // You don't have enough room in your backpack!
			library.ToolBrokeMessage = "You broke your tool."; // 1044038; // You broke your tool.

			res = new HarvestResource[]
				{
					new HarvestResource( 00.0, 00.0, 100.0, "", typeof( BlankScroll ) ),
					new HarvestResource( 65.0, 25.0, 105.0, "", typeof( WritingBook ) ),
					new HarvestResource( 70.0, 30.0, 110.0, "", typeof( SomeRandomNote ) ),
					new HarvestResource( 75.0, 35.0, 115.0, "", typeof( ScrollClue ) ),
					new HarvestResource( 80.0, 40.0, 120.0, "", typeof( WeakenScroll ) ),
					new HarvestResource( 85.0, 45.0, 125.0, "", typeof( ProtectionScroll ) ),
					new HarvestResource( 90.0, 50.0, 130.0, "", typeof( UnlockScroll ) ),
					new HarvestResource( 95.0, 55.0, 135.0, "", typeof( CurseScroll ) ),
					new HarvestResource( 99.0, 59.0, 139.0, "", typeof( ParalyzeScroll ) ),
					new HarvestResource( 100.1, 69.0, 140.0, "", typeof( ExplosionScroll ) )
				};

			veins = new HarvestVein[]
				{
					new HarvestVein( 45.0, 0.0, res[0], res[0] ),
					new HarvestVein( 15.0, 0.5, res[1], res[0] ),
					new HarvestVein( 11.0, 0.5, res[2], res[0] ),
					new HarvestVein( 08.0, 0.5, res[3], res[0] ),
					new HarvestVein( 06.0, 0.5, res[4], res[0] ),
					new HarvestVein( 05.0, 0.5, res[5], res[0] ),
					new HarvestVein( 04.0, 0.5, res[6], res[0] ),
					new HarvestVein( 03.0, 0.5, res[7], res[0] ),
					new HarvestVein( 02.0, 0.5, res[8], res[0] ),
					new HarvestVein( 01.0, 0.5, res[9], res[0] )
				};

			if ( Core.ML )
			{
				library.BonusResources = new BonusHarvestResource[]
				{
					new BonusHarvestResource( 0, 80.0, null, null ),	//Nothing
					new BonusHarvestResource( 40, 10.0, 1074542, typeof( LoreBook ) ),
					new BonusHarvestResource( 60, 5.0, 1074542, typeof( DDRelicScrolls ) ),
					new BonusHarvestResource( 60, 5.0, 1074542, typeof( DDRelicBook ) )
				};
			}

			library.RandomizeVeins = Core.ML;

			library.Resources = res;
			library.Veins = veins;

			m_Definition = library;
			Definitions.Add( library );
			#endregion
		}

		public override bool CheckHarvest( Mobile from, Item tool )
		{
			if ( !base.CheckHarvest( from, tool ) )
				return false;

			if ( !(from.Region is DungeonRegion || from.Region is DeadRegion || from.Region is CaveRegion || from.Region is BardDungeonRegion || from.Region is OutDoorBadRegion) )
			{
				from.SendMessage("There is nothing here of interest.");
				return false;
			}
			else if ( from.Mounted )
			{
				from.SendMessage("You cannot examine these books while riding.");
				return false;
			}
			else if ( from.IsBodyMod && !from.Body.IsHuman && from.RaceID < 1 )
			{
				from.SendMessage("You cannot examine these books while polymorphed.");
				return false;
			}

			return true;
		}

		public override bool CheckHarvest( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			if ( !base.CheckHarvest( from, tool, def, toHarvest ) )
				return false;

			if ( !(toHarvest is StaticTarget) )
				return false;

			if ( !(from.Region is DungeonRegion || from.Region is DeadRegion || from.Region is CaveRegion || from.Region is BardDungeonRegion || from.Region is OutDoorBadRegion) )
			{
				from.SendMessage("There is nothing here of interest.");
				return false;
			}
			else if ( from.Mounted )
			{
				from.SendMessage("You cannot examine these books while riding.");
				return false;
			}
			else if ( from.IsBodyMod && !from.Body.IsHuman && from.RaceID < 1 )
			{
				from.SendMessage("You cannot examine these books while polymorphed.");
				return false;
			}

			return true;
		}

		public override bool BeginHarvesting( Mobile from, Item tool )
		{
			if ( !base.BeginHarvesting( from, tool ) )
				return false;

			from.SendMessage("Which books do you want to look through?");
			return true;
		}

		public override void OnBadHarvestTarget( Mobile from, Item tool, object toHarvest )
		{
			from.SendMessage( "That has nothing of interest." );
		}

		public override void OnHarvestStarted( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			base.OnHarvestStarted( from, tool, def, toHarvest );

			if ( Core.ML )
				from.RevealingAction();
		}

		private static int[] m_Offsets = new int[]
			{
				-1, -1,
				-1,  0,
				-1,  1,
				 0, -1,
				 0,  1,
				 1, -1,
				 1,  0,
				 1,  1
			};

		public override void OnHarvestFinished( Mobile from, Item tool, HarvestDefinition def, HarvestVein vein, HarvestBank bank, HarvestResource resource, object harvested)
		{
			base.OnHarvestFinished( from, tool, def, vein, bank, resource, harvested );

			if ( from.Map != null && harvested is StaticTarget )
			{
				int itemID = ((StaticTarget)harvested).ItemID;

				if ( !( itemID == 0x0C16 || itemID == 0x12F3 || itemID == 0x12FF || itemID == 0x1305 || itemID == 0x130B || itemID == 0x1311 || itemID == 0x1317 || itemID == 0x131D || itemID == 0x134E || itemID == 0x1398 || itemID == 0x1399 || itemID == 0x1E20 || itemID == 0x1E21 || itemID == 0x1E22 || itemID == 0x1E23 || itemID == 0x1E24 || itemID == 0x1E25 ) )
				{
					if ( itemID == 0x3084 || itemID == 0x3085 ){ itemID = 0x2DEF; }
					else if ( itemID == 0x3086 || itemID == 0x3087 ){ itemID = 0x2DF0; }

					if ( vein.PrimaryResource == resource )
					{
						try
						{

							int search = (int)(from.Skills[SkillName.Inscribe].Value);
							if ( search > Utility.Random( 5000 ) )
							{
								if ( from.CheckSkill( SkillName.Inscribe, 0, 125 ) )
								{
									Item shelf = new BasicShelf();
										shelf.Name = "book shelf";
										shelf.ItemID = itemID;
										if ( itemID >= 0x4FDB ){ shelf.Hue = 0xABE; }

									string name = "a book shelf";

									switch ( Utility.Random( 100 ) )
									{
										case 0: shelf = new NecromancerSpellbook(); name = "necromancer spellbook"; break;
										case 1: shelf = new Spellbook(); name = "a magery spellbook"; break;
										case 2: shelf = new BookOfNinjitsu(); name = "a book of the ninja"; break;
										case 3: shelf = new BookOfBushido(); name = "a book of the samurai"; break;
										case 4: shelf = new BookOfChivalry(); name = "a book of knightship"; break;
										case 5: shelf = new SongBook(); name = "a book of bardic songs"; break;
										case 6: shelf = new ArtifactManual(); name = "an artifact manual"; break;
										case 7: shelf = new DDRelicTablet(); shelf.CoinPrice = shelf.CoinPrice + Utility.RandomMinMax( 1, (int)(from.Skills[SkillName.Inscribe].Value*2) ); name = "a stone tablet"; break;
										case 8: shelf = Server.Items.PowerScroll.RandomPowerScroll(); name = "a scroll of power"; break;
										case 9:
											switch ( Utility.Random( 6 ) )
											{
												case 0: shelf = new SmallBoatDeed(); break;
												case 1: shelf = new SmallDragonBoatDeed(); break;
												case 2: shelf = new MediumBoatDeed(); break;
												case 3: shelf = new MediumDragonBoatDeed(); break;
												case 4: shelf = new LargeBoatDeed(); break;
												case 5: shelf = new LargeDragonBoatDeed(); break;
											}
											name = "a deed to a ship";
										break;
										case 10: shelf = new TreasureMap( Utility.RandomList(1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,3,3,3,3,4,4,4,4,4,4,5,5,5,6), from.Map, from.Location, from.X, from.Y ); name = "a treasure map"; break;
										case 11: shelf = new ElementalSpellbook(); name = "elemental spellbook"; break;
									}
									if ( shelf != null )
									{
										BaseContainer.PutStuffInContainer( from, 3, shelf );
										from.SendMessage( "you found " + name + "!" );
									}
								}
							}
						}
						catch
						{
						}
					}
				}
			}
		}

		public static void Initialize()
		{
			Array.Sort( m_LibraryTiles );
		}

		#region Tile lists
		private static int[] m_LibraryTiles = new int[]
		{
			0x4A97, 0x4A98, 0x4A99, 0x4A9A, 0x4A9B, 0x4A9C, 0x4C14, 0x4C15, 0x4C16, 0x52F3, 0x52FF, 0x5305, 0x530B, 0x5311, 
			0x5317, 0x531D, 0x534E, 0x5398, 0x5399, 0x59FF, 0x5A00, 0x5E20, 0x5E21, 0x5E22, 0x5E23, 0x5E24, 0x5E25, 0x6DEF, 
			0x6DF0, 0x7084, 0x7085, 0x7086, 0x7087, 0x7BF9, 0x7BFA, 0x7BFB, 0x7BFC, 0x7BFD, 0x7BFE, 0x7C15, 0x7C16, 0x7C2B, 
			0x7C2C, 0x7C2D, 0x7C2E, 0x7C33, 0x7C34, 0x7C5F, 0x7C60, 0x7C61, 0x7C62, 0x7C73, 0x7C74, 0x7C79, 0x7C7A, 0x7CA5, 
			0x7CA6, 0x7CA7, 0x7CA8, 0x7CAF, 0x7CB0, 0x7CDB, 0x7CDC, 0x7CEB, 0x7CEC, 0x7CED, 0x7CEE, 0x7CFD, 0x7CFE, 0x7D05, 
			0x7D06, 0x9004, 0x9005, 0x900C, 0x900D, 0x9012, 0x9013, 0x902C, 0x902D, 0x9038, 0x9039, 0x903A, 0x903B, 

			0x5004, 0x5005, 0x500C, 0x500D, 0x5012, 0x5013, 0x502C, 0x502D, 0x5038, 0x5039, 0x503A, 0x503B
		};
		#endregion
	}
}