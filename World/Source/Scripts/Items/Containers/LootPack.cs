using System;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Misc;
using Server.Utilities;

namespace Server
{
	public class LootPack
	{
		public static int GetRegularLuckChance( Mobile from )
		{
			int luck = from.Luck;
			if ( luck < 0 ) return 0;

			return (int)(Math.Pow( luck, 1 / 1.8 ) * 100);
		}

		public static int GetLuckChanceForKiller( Mobile dead )
		{
			List<DamageStore> list = BaseCreature.GetLootingRights( dead.DamageEntries, dead.HitsMax );

			DamageStore highest = null;

			for ( int i = 0; i < list.Count; ++i )
			{
				DamageStore ds = list[i];

				if ( ds.m_HasRight && (highest == null || ds.m_Damage > highest.m_Damage) )
					highest = ds;
			}

			if ( highest == null )
				return 0;

			return GetRegularLuckChance( highest.m_Mobile );
		}

		public static bool CheckLuck( int chance )
		{
			return ( chance > Utility.Random( 10000 ) );
		}

		private LootPackEntry[] m_Entries;

		public LootPack( LootPackEntry[] entries )
		{
			m_Entries = entries;
		}

		public void Generate( Mobile from, Container cont, bool spawning, int luckChance, int level, int dungeonLevelBonus )
		{
			if ( MySettings.S_LootChance > Utility.Random(100) )
			{
				level = LootPackChange.ScaleLevel( level );

				if ( cont == null )
					return;

				bool checkLuck = true;

				for ( int i = 0; i < m_Entries.Length; ++i )
				{
					LootPackEntry entry = m_Entries[i];

					bool shouldAdd = ( entry.Chance > Utility.Random( 10000 ) );

					if ( !shouldAdd && checkLuck )
					{
						checkLuck = false;

						if ( LootPack.CheckLuck( luckChance ) )
							shouldAdd = ( entry.Chance > Utility.Random( 10000 ) );
					}

					if ( !shouldAdd )
						continue;

					Item item = entry.Construct( from, luckChance, spawning, level, dungeonLevelBonus );
					if ( item != null )
					{
						LootPackChange.RemoveItem( item, from, level );
						if (item.Deleted) continue;

						if (item is Gold)
						{
							var amount = (int)(item.Amount * (MyServerSettings.GetGoldCutRate() * .01));
							item.Amount = Math.Max(1, amount);
							cont.TryDropItem(from, item, false);
							continue;
                        }

						item = LootPackChange.ChangeItem( item, from, level );
						NotIdentified.ConfigureItem( item, cont, from );
						ReagentJar.ConfigureItem( item, cont, from );
					}
				}
			}
		}

		public static readonly LootPackItem[] Gold = new LootPackItem[] { new LootPackItem( typeof( Gold ), 1 ) };
		public static readonly LootPackItem[] Instruments = new LootPackItem[] { new LootPackItem( typeof( BaseInstrument ), 1 ) };
		public static readonly LootPackItem[] Spellbooks = new LootPackItem[] { new LootPackItem( typeof( Spellbook ), 1 ) };
		public static readonly LootPackItem[] Quivers = new LootPackItem[] { new LootPackItem( typeof( BaseQuiver ), 1 ) };
		public static readonly LootPackItem[] LowScrollItems = new LootPackItem[] { new LootPackItem( typeof( ClumsyScroll ), 1 ) };
		public static readonly LootPackItem[] MedScrollItems = new LootPackItem[] { new LootPackItem( typeof( ArchCureScroll ), 1 ) };
		public static readonly LootPackItem[] HighScrollItems = new LootPackItem[] { new LootPackItem( typeof( ChainLightningScroll ), 1 ) };
		public static readonly LootPackItem[] GemItems = new LootPackItem[] { new LootPackItem( typeof( Amber ), 1 ) };
		public static readonly LootPackItem[] ArtyItems = new LootPackItem[] { new LootPackItem( typeof( Artifact_YashimotosHatsuburi ), 1 ) };
		public static readonly LootPackItem[] SArtyItems = new LootPackItem[] { new LootPackItem( typeof( GoldBricks ), 1 ) };
		public static readonly LootPackItem[] LowPotionItems = new LootPackItem[] { new LootPackItem( typeof( LesserHealPotion ), 1 ) };
		public static readonly LootPackItem[] MedPotionItems = new LootPackItem[] { new LootPackItem( typeof( HealPotion ), 1 ) };
		public static readonly LootPackItem[] HighPotionItems = new LootPackItem[] { new LootPackItem( typeof( GreaterHealPotion ), 1 ) };
		public static readonly LootPackItem[] ReagentItems = new LootPackItem[] { new LootPackItem( typeof( BlackPearl ), 1 ) };
		public static readonly LootPackItem[] SongItems = new LootPackItem[] { new LootPackItem( typeof( FoeRequiemScroll ), 1 ) };
		public static readonly LootPackItem[] MusicItems = new LootPackItem[] { new LootPackItem( typeof( Lute ), 1 ) };

		// ------------------------------------------------------------------------------------------------------------------------------------------

		#region Magic Items
		public static readonly LootPackItem[] MagicItemsPoor = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseWeapon ), 3 ),
				new LootPackItem( typeof( BaseRanged ), 1 ),
				new LootPackItem( typeof( BaseArmor ), 3 ),
				new LootPackItem( typeof( BaseShield ), 1 ),
				new LootPackItem( typeof( BaseTrinket ), 2 ),
				new LootPackItem( typeof( BaseClothing ), 1 )
			};

		public static readonly LootPackItem[] MagicItemsMeager1 = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseWeapon ), 56 ),
				new LootPackItem( typeof( BaseRanged ), 14 ),
				new LootPackItem( typeof( BaseArmor ), 61 ),
				new LootPackItem( typeof( BaseShield ), 11 ),
				new LootPackItem( typeof( BaseTrinket ), 42 ),
				new LootPackItem( typeof( BaseClothing ), 20 )
			};

		public static readonly LootPackItem[] MagicItemsMeager2 = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseWeapon ), 28 ),
				new LootPackItem( typeof( BaseRanged ), 7 ),
				new LootPackItem( typeof( BaseArmor ), 30 ),
				new LootPackItem( typeof( BaseShield ), 5 ),
				new LootPackItem( typeof( BaseTrinket ), 21 ),
				new LootPackItem( typeof( BaseClothing ), 10 )
			};

		public static readonly LootPackItem[] MagicItemsAverage1 = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseWeapon ), 90 ),
				new LootPackItem( typeof( BaseRanged ), 23 ),
				new LootPackItem( typeof( BaseArmor ), 98 ),
				new LootPackItem( typeof( BaseShield ), 17 ),
				new LootPackItem( typeof( BaseTrinket ), 68 ),
				new LootPackItem( typeof( BaseClothing ), 32 )
			};

		public static readonly LootPackItem[] MagicItemsAverage2 = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseWeapon ), 54 ),
				new LootPackItem( typeof( BaseRanged ), 13 ),
				new LootPackItem( typeof( BaseArmor ), 57 ),
				new LootPackItem( typeof( BaseShield ), 10 ),
				new LootPackItem( typeof( BaseTrinket ), 40 ),
				new LootPackItem( typeof( BaseClothing ), 20 )
			};

		public static readonly LootPackItem[] MagicItemsRich1 = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseWeapon ), 211 ),
				new LootPackItem( typeof( BaseRanged ), 53 ),
				new LootPackItem( typeof( BaseArmor ), 227 ),
				new LootPackItem( typeof( BaseShield ), 39 ),
				new LootPackItem( typeof( BaseTrinket ), 158 ),
				new LootPackItem( typeof( BaseClothing ), 76 )
			};

		public static readonly LootPackItem[] MagicItemsRich2 = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseWeapon ), 170 ),
				new LootPackItem( typeof( BaseRanged ), 43 ),
				new LootPackItem( typeof( BaseArmor ), 184 ),
				new LootPackItem( typeof( BaseShield ), 32 ),
				new LootPackItem( typeof( BaseTrinket ), 128 ),
				new LootPackItem( typeof( BaseClothing ), 61 )
			};

		public static readonly LootPackItem[] MagicItemsFilthyRich1 = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseWeapon ), 219 ),
				new LootPackItem( typeof( BaseRanged ), 55 ),
				new LootPackItem( typeof( BaseArmor ), 236 ),
				new LootPackItem( typeof( BaseShield ), 41 ),
				new LootPackItem( typeof( BaseTrinket ), 164 ),
				new LootPackItem( typeof( BaseClothing ), 86 )
			};

		public static readonly LootPackItem[] MagicItemsFilthyRich2 = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseWeapon ), 239 ),
				new LootPackItem( typeof( BaseRanged ), 60 ),
				new LootPackItem( typeof( BaseArmor ), 257 ),
				new LootPackItem( typeof( BaseShield ), 90 ),
				new LootPackItem( typeof( BaseTrinket ), 45 ),
				new LootPackItem( typeof( BaseClothing ), 86 )
			};

		public static readonly LootPackItem[] MagicItemsUltraRich = new LootPackItem[]
			{
				new LootPackItem( typeof( BaseWeapon ), 276 ),
				new LootPackItem( typeof( BaseRanged ), 69 ),
				new LootPackItem( typeof( BaseArmor ), 397 ),
				new LootPackItem( typeof( BaseShield ), 52 ),
				new LootPackItem( typeof( BaseTrinket ), 207 ),
				new LootPackItem( typeof( BaseClothing ), 207 )
			};

		#endregion

		// ------------------------------------------------------------------------------------------------------------------------------------------

		#region Monster definitions
		public static readonly LootPack MonsterPoor = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, Gold,					   100.00, "2d10+20" ),
				new LootPackEntry( false, MagicItemsPoor,		  	 1.00, 1, 1, 0, 25 ),
				new LootPackEntry( false, Instruments,			  	 0.04, 1, 1, 0, 25 ),
				new LootPackEntry( false, Spellbooks,				 0.04, 1, 1, 0, 25 ),
				new LootPackEntry( false, Quivers,				  	 0.02, 1, 1, 0, 25 ),
				new LootPackEntry( false, MagicItemsMeager1,	 	20.40, 1, 2, 0, 25 ),
				new LootPackEntry( false, LowPotionItems,			20.00, 1 )
			} );

		public static readonly LootPack MonsterMeager = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, Gold,						100.00, "4d10+40" ),
				new LootPackEntry( false, MagicItemsMeager1,	 	 20.40, 1, 2, 0, 30 ),
				new LootPackEntry( false, MagicItemsMeager2,	 	 10.20, 1, 2, 0, 30 ),
				new LootPackEntry( false, Instruments,				  0.20, 1, 2, 0, 30 ),
				new LootPackEntry( false, Spellbooks,				  0.20, 1, 2, 0, 30 ),
				new LootPackEntry( false, Quivers,				  	  0.10, 1, 2, 0, 30 ),
				new LootPackEntry( false, LowPotionItems,			 50.00, 1 )
			} );

		public static readonly LootPack MonsterAverage = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, Gold,					   100.00, "8d10+100" ),
				new LootPackEntry( false, MagicItemsAverage1, 		32.80, 1, 3, 0, 40 ),
				new LootPackEntry( false, MagicItemsAverage1, 		32.80, 1, 3, 0, 40 ),
				new LootPackEntry( false, MagicItemsAverage2, 		19.50, 1, 3, 0, 40 ),
				new LootPackEntry( false, Instruments,				 0.80, 1, 3, 0, 40 ),
				new LootPackEntry( false, Spellbooks,				 0.80, 1, 3, 0, 40 ),
				new LootPackEntry( false, Quivers,				  	 0.40, 1, 3, 0, 40 ),
				new LootPackEntry( false, MedPotionItems,			20.00, 1 )
			} );

		public static readonly LootPack MonsterRich = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, Gold,					   100.00, "15d10+225" ),
				new LootPackEntry( false, MagicItemsRich1,	 		76.30, 2, 3, 20, 40 ),
				new LootPackEntry( false, MagicItemsRich1,	 		76.30, 2, 3, 20, 40 ),
				new LootPackEntry( false, MagicItemsRich2,	 		61.70, 2, 4, 20, 40 ),
				new LootPackEntry( false, Instruments,				 4.00, 2, 3, 20, 40 ),
				new LootPackEntry( false, Spellbooks,				 4.00, 2, 3, 20, 40 ),
				new LootPackEntry( false, Quivers,				  	 2.00, 2, 3, 20, 40 ),
				new LootPackEntry( false, SArtyItems,				 1.00, 2, 3, 20, 40 ),
				new LootPackEntry( false, MedPotionItems,			50.00, 1 )
			} );

		public static readonly LootPack MonsterFilthyRich = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, Gold,						   100.00, "3d100+400" ),
				new LootPackEntry( false, MagicItemsFilthyRich1,		79.50, 2, 4, 25,  50 ),
				new LootPackEntry( false, MagicItemsFilthyRich1,		79.50, 2, 4, 25,  50 ),
				new LootPackEntry( false, MagicItemsFilthyRich2,		77.60, 3, 4, 25,  50 ),
				new LootPackEntry( false, Instruments,					 4.00, 2, 4, 25,  50 ),
				new LootPackEntry( false, Spellbooks,					 4.00, 2, 4, 25,  50 ),
				new LootPackEntry( false, Quivers,				  	  	 2.00, 2, 4, 25,  50 ),
				new LootPackEntry( false, SArtyItems,				  	 1.00, 1 ),
				new LootPackEntry( false, ArtyItems,				 	 0.50, 1 ),
				new LootPackEntry( false, HighPotionItems,				25.00, 1 )
			} );

		public static readonly LootPack MonsterUltraRich = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, Gold,						100.00, "6d100+600" ),
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 2, 5, 40, 70 ),
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 2, 5, 40, 70 ),
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 2, 5, 40, 70 ),
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 2, 5, 40, 70 ),
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 2, 5, 40, 70 ),
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 2, 5, 40, 70 ),
				new LootPackEntry( false, Instruments,				  8.00, 2, 5, 40, 70 ),
				new LootPackEntry( false, Spellbooks,				  8.00, 2, 5, 40, 70 ),
				new LootPackEntry( false, Quivers,				  	  4.00, 2, 5, 40, 70 ),
				new LootPackEntry( false, SArtyItems,				  2.00, 1 ),
				new LootPackEntry( false, ArtyItems,				  1.00, 1 ),
				new LootPackEntry( false, HighPotionItems,			 50.00, 1 )
			} );

		public static readonly LootPack MonsterMegaRich = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, Gold,						100.00, "10d100+800" ),
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 3, 5, 40, 70 ),
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 3, 5, 40, 70 ),
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 3, 5, 40, 70 ),
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 3, 5, 40, 70 ),
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 3, 5, 40, 70 ),
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 3, 5, 40, 70 ),
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 3, 5, 40, 70 ),
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 3, 5, 40, 70 ),
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 3, 5, 40, 70 ),
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 3, 5, 40, 70 ),
				new LootPackEntry( false, Instruments,				  8.00, 3, 5, 40, 70 ),
				new LootPackEntry( false, Spellbooks,				  8.00, 3, 5, 40, 70 ),
				new LootPackEntry( false, Quivers,				  	  6.00, 3, 5, 40, 70 ),
				new LootPackEntry( false, SArtyItems,				  4.00, 1 ),
				new LootPackEntry( false, ArtyItems,				  2.00, 1 ),
				new LootPackEntry( false, HighPotionItems,			 75.00, 1 )
			} );
		#endregion

		// ------------------------------------------------------------------------------------------------------------------------------------------

		#region Treasure definitions
		public static readonly LootPack TreasurePoor = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, MagicItemsPoor,		  	10.00, 1, 1, 10, 25 ),
				new LootPackEntry( false, Instruments,			  	 0.04, 1, 1, 10, 25 ),
				new LootPackEntry( false, Spellbooks,				 0.04, 1, 1, 10, 25 ),
				new LootPackEntry( false, Quivers,				  	 0.02, 1, 1, 10, 25 ),
				new LootPackEntry( false, MagicItemsMeager1,	 	20.40, 1, 1, 10, 25 ),
				new LootPackEntry( false, LowScrollItems,			 5.00, 1 ),
				new LootPackEntry( false, GemItems,					20.00, 1 ),
				new LootPackEntry( false, ReagentItems,				 5.00, 1 ),
				new LootPackEntry( false, LowPotionItems,			 5.00, 1 )
			} );

		public static readonly LootPack TreasureMeager = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, MagicItemsMeager1,	 	 20.40, 1, 2, 10, 30 ),
				new LootPackEntry( false, Instruments,				  0.20, 1, 2, 10, 30 ),
				new LootPackEntry( false, Spellbooks,				  0.20, 1, 2, 10, 30 ),
				new LootPackEntry( false, Quivers,				  	  0.10, 1, 2, 10, 30 ),
				new LootPackEntry( false, LowScrollItems,			 10.00, 1 ),
				new LootPackEntry( false, GemItems,					 40.00, 1 ),
				new LootPackEntry( false, ReagentItems,				 10.00, 1 ),
				new LootPackEntry( false, LowPotionItems,			 10.00, 1 )
			} );

		public static readonly LootPack TreasureAverage = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, MagicItemsAverage1, 		32.80, 1, 3, 10, 40 ),
				new LootPackEntry( false, Instruments,				 0.80, 1, 3, 10, 40 ),
				new LootPackEntry( false, Spellbooks,				 0.80, 1, 3, 10, 40 ),
				new LootPackEntry( false, Quivers,				  	 0.40, 1, 3, 10, 40 ),
				new LootPackEntry( false, MedScrollItems,			20.00, 1 ),
				new LootPackEntry( false, GemItems,					60.00, 1 ),
				new LootPackEntry( false, ReagentItems,				20.00, 1 ),
				new LootPackEntry( false, MedPotionItems,			20.00, 1 )
			} );

		public static readonly LootPack TreasureRich = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, MagicItemsRich1,	 		76.30, 2, 3, 20, 40 ),
				new LootPackEntry( false, Instruments,				 4.00, 2, 3, 20, 40 ),
				new LootPackEntry( false, Spellbooks,				 4.00, 2, 3, 20, 40 ),
				new LootPackEntry( false, Quivers,				  	 2.00, 2, 3, 20, 40 ),
				new LootPackEntry( false, SArtyItems,				 0.50, 2, 3, 20, 40 ),
				new LootPackEntry( false, HighScrollItems,			30.00, 1 ),
				new LootPackEntry( false, GemItems,					70.00, 1 ),
				new LootPackEntry( false, ReagentItems,				30.00, 1 ),
				new LootPackEntry( false, HighPotionItems,			30.00, 1 )
			} );

		public static readonly LootPack TreasureFilthyRich = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, MagicItemsFilthyRich1,		79.50, 2, 4, 35, 50 ),
				new LootPackEntry( false, Instruments,					 4.00, 2, 4, 35, 50 ),
				new LootPackEntry( false, Spellbooks,					 4.00, 2, 4, 35, 50 ),
				new LootPackEntry( false, Quivers,				  	  	 2.00, 2, 4, 35, 50 ),
				new LootPackEntry( false, SArtyItems,				  	 0.50, 1 ),
				new LootPackEntry( false, ArtyItems,				 	 0.25, 1 ),
				new LootPackEntry( false, HighScrollItems,			 	40.00, 1 ),
				new LootPackEntry( false, GemItems,						80.00, 1 ),
				new LootPackEntry( false, ReagentItems,					40.00, 1 ),
				new LootPackEntry( false, HighPotionItems,				40.00, 1 )
			} );

		public static readonly LootPack TreasureUltraRich = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 2, 5, 50, 70 ),
				new LootPackEntry( false, Instruments,				  8.00, 2, 5, 50, 70 ),
				new LootPackEntry( false, Spellbooks,				  8.00, 2, 5, 50, 70 ),
				new LootPackEntry( false, Quivers,				  	  4.00, 2, 5, 50, 70 ),
				new LootPackEntry( false, SArtyItems,				  1.00, 1 ),
				new LootPackEntry( false, ArtyItems,				  0.50, 1 ),
				new LootPackEntry( false, HighScrollItems,			 50.00, 1 ),
				new LootPackEntry( false, GemItems,					 90.00, 1 ),
				new LootPackEntry( false, ReagentItems,				 50.00, 1 ),
				new LootPackEntry( false, HighPotionItems,			 50.00, 1 )
			} );

		public static readonly LootPack TreasureMegaRich = new LootPack( new LootPackEntry[]
			{
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 3, 5, 50, 70 ),
				new LootPackEntry( false, MagicItemsUltraRich,		100.00, 3, 5, 50, 70 ),
				new LootPackEntry( false, Instruments,				  8.00, 3, 5, 50, 70 ),
				new LootPackEntry( false, Spellbooks,				  8.00, 3, 5, 50, 70 ),
				new LootPackEntry( false, Quivers,				  	  6.00, 3, 5, 50, 70 ),
				new LootPackEntry( false, SArtyItems,				  2.00, 1 ),
				new LootPackEntry( false, ArtyItems,				  0.75, 1 ),
				new LootPackEntry( false, HighScrollItems,			 60.00, 1 ),
				new LootPackEntry( false, GemItems,					100.00, 1 ),
				new LootPackEntry( false, ReagentItems,				 60.00, 1 ),
				new LootPackEntry( false, HighPotionItems,			 60.00, 1 )
			} );
		#endregion

		// ------------------------------------------------------------------------------------------------------------------------------------------

		#region Generic accessors
		public static LootPack Poor{ get{ return MonsterPoor; } }
		public static LootPack Meager{ get{ return MonsterMeager; } }
		public static LootPack Average{ get{ return MonsterAverage; } }
		public static LootPack Rich{ get{ return MonsterRich; } }
		public static LootPack FilthyRich{ get{ return MonsterFilthyRich; } }
		public static LootPack UltraRich{ get{ return MonsterUltraRich; } }
		public static LootPack SuperBoss{ get{ return MonsterMegaRich; } }
		public static LootPack TPoor{ get{ return TreasurePoor; } }
		public static LootPack TMeager{ get{ return TreasureMeager; } }
		public static LootPack TAverage{ get{ return TreasureAverage; } }
		public static LootPack TRich{ get{ return TreasureRich; } }
		public static LootPack TFilthyRich{ get{ return TreasureFilthyRich; } }
		public static LootPack TUltraRich{ get{ return TreasureUltraRich; } }
		public static LootPack TMegaRich{ get{ return TreasureMegaRich; } }
		#endregion

		public static readonly LootPack LowScrolls = new LootPack( new LootPackEntry[] { new LootPackEntry( false, LowScrollItems,		100.00, 1 ) } );
		public static readonly LootPack MedScrolls = new LootPack( new LootPackEntry[] { new LootPackEntry( false, MedScrollItems,		100.00, 1 ) } );
		public static readonly LootPack HighScrolls = new LootPack( new LootPackEntry[] { new LootPackEntry( false, HighScrollItems,	100.00, 1 ) } );
		public static readonly LootPack Gems = new LootPack( new LootPackEntry[] { new LootPackEntry( false, GemItems,					100.00, 1 ) } );
		public static readonly LootPack LowPotions = new LootPack( new LootPackEntry[] { new LootPackEntry( false, LowPotionItems,		100.00, 1 ) } );
		public static readonly LootPack MedPotions = new LootPack( new LootPackEntry[] { new LootPackEntry( false, MedPotionItems,		100.00, 1 ) } );
		public static readonly LootPack HighPotions = new LootPack( new LootPackEntry[] { new LootPackEntry( false, HighPotionItems,	100.00, 1 ) } );
		public static readonly LootPack Songs = new LootPack( new LootPackEntry[] { new LootPackEntry( false, SongItems,				100.00, 1 ) } );
		public static readonly LootPack Music = new LootPack( new LootPackEntry[] { new LootPackEntry( false, MusicItems,				100.00, 1 ) } );
	}

	public class LootPackEntry
	{
		private int m_Chance;
		private LootPackDice m_Quantity;

		private int m_MaxProps, m_MinIntensity, m_MaxIntensity;

		private bool m_AtSpawnTime;

		private LootPackItem[] m_Items;

		public int Chance
		{
			get{ return m_Chance; }
			set{ m_Chance = value; }
		}

		public LootPackDice Quantity
		{
			get{ return m_Quantity; }
			set{ m_Quantity = value; }
		}

		public int MaxProps
		{
			get{ return m_MaxProps; }
			set{ m_MaxProps = value; }
		}

		public int MinIntensity
		{
			get{ return m_MinIntensity; }
			set{ m_MinIntensity = value; }
		}

		public int MaxIntensity
		{
			get{ return m_MaxIntensity; }
			set{ m_MaxIntensity = value; }
		}

		public LootPackItem[] Items
		{
			get{ return m_Items; }
			set{ m_Items = value; }
		}

		public static bool playOrient( Mobile m ) // SEE IF PLAYER IS SET TO ORIENTAL MODE
		{
			if ( m != null )
			{
				if ( GetPlayerInfo.OrientalPlay( m ) )
					return true;
			}

			return false;
		}

		public Item Construct( Mobile from, int luckChance, bool spawning, int level, int dungeonLevelBonus )
		{
			if ( m_AtSpawnTime != spawning )
				return null;

			int totalChance = 0;

			for ( int i = 0; i < m_Items.Length; ++i )
				totalChance += m_Items[i].Chance;

			int rnd = Utility.Random( totalChance );

			for ( int i = 0; i < m_Items.Length; ++i )
			{
				LootPackItem item = m_Items[i];

				if ( rnd < item.Chance )
					return Mutate( from, luckChance, item.Construct( playOrient( from ), from ), level, dungeonLevelBonus );

				rnd -= item.Chance;
			}

			return null;
		}

		public Item Mutate( Mobile from, int luckChance, Item item, int level, int dungeonLevelBonus )
		{
			if ( item != null && !( item.NotModAble || item.ArtifactLevel > (int)ArtifactLevel.None ) )
			{
				if ( item is BaseWeapon || item is BaseArmor || item is BaseTrinket || item is BaseInstrument || item is BaseQuiver || item is BaseClothing || item is Spellbook )
				{
					if ( Worlds.isSciFiRegion( from ) && Utility.Random(20) == 0 && item is BaseRanged )
					{
						item.Delete();
						item = Loot.RandomSciFiGun();
					}
					// if ( Worlds.isSciFiRegion( from ) && Utility.Random(20) == 0 && item is BaseWeapon )
					// {
					// 	item.Delete();
					// 	item = Loot.RandomSciFiWeapon();
					// }

					int bonusProps = GetBonusProperties();
					int min = m_MinIntensity;
					int max = m_MaxIntensity;

					int levelNormalized = NormalizeLevel(level, dungeonLevelBonus);
					switch( levelNormalized )
					{
						// No benefit
						case 0: break;
						case 1: break;

						// Increase top
						case 2: max += 5; break;
						case 3: max += 15; break;

						// Increase bottom and top 
						case 4: min += 10; break;
						case 5: min += 15; break;
						case 6: min += 20; break;

						// Bonus property
						case 7: ++bonusProps; break;
						case 8: ++bonusProps; break;

						// Ensure a bare minimum is set
						case 9: min = 70; break;
						case 10: min = 70; max = 100; break;
					}

					if ( bonusProps < m_MaxProps && LootPack.CheckLuck( luckChance ) )
						++bonusProps;

					int props = 1 + bonusProps;

					// Make sure we're not spawning items with 6 properties.
					if ( props > m_MaxProps )
						props = m_MaxProps;

					BaseRunicTool.ApplyAttributes( luckChance, item, props, min, max );
				}
			}

			if ( item != null && item.Stackable )
				item.Amount = m_Quantity.Roll();

			return item;
		}

		private static int NormalizeLevel( int level, int dungeonLevelBonus )
		{
			// Heat goes up to 4. 5 is Epic, which isn't implemented at this time.
			// Level goes up to 12.
			//  4 + (12/2) = 10
			return dungeonLevelBonus + (level / 2);
		}

		public static CraftAttributeInfo GetResourceAttrs( CraftResource resource )
		{
			CraftResourceInfo info = CraftResources.GetInfo( resource );

			if ( info == null )
				return CraftAttributeInfo.Blank;

			return info.AttributeInfo;
		}

		public static Item Enchant(Mobile from, Item item, int props, int min, int max, bool isRunicTool)
		{
			min = Math.Max(0, min);
			max = Math.Max(min + 1, max);

			int luckChance = from != null &&from.Luck > 0 ? LootPack.GetRegularLuckChance(from) : 0;

			if ( item is BaseWeapon )
				BaseRunicTool.ApplyAttributesTo( (BaseWeapon)item, isRunicTool, luckChance, props, min, max );
			else if ( item is BaseArmor )
				BaseRunicTool.ApplyAttributesTo( (BaseArmor)item, isRunicTool, luckChance, props, min, max );
			else if ( item is BaseTrinket )
				BaseRunicTool.ApplyAttributesTo( (BaseTrinket)item, isRunicTool, luckChance, props, min, max );
			else if ( item is BaseQuiver )
				BaseRunicTool.ApplyAttributesTo( (BaseQuiver)item, isRunicTool, luckChance, props, min, max );
			else if ( item is BaseHat )
				BaseRunicTool.ApplyAttributesTo( (BaseHat)item, isRunicTool, luckChance, props, min, max );
			else if ( item is BaseClothing )
				BaseRunicTool.ApplyAttributesTo( (BaseClothing)item, isRunicTool, luckChance, props, min, max );
			else if ( item is BaseInstrument )
				BaseRunicTool.ApplyAttributesTo( (BaseInstrument)item, isRunicTool, luckChance, props, min, max );
			else if ( item is Spellbook )
				BaseRunicTool.ApplyAttributesTo( (Spellbook)item, isRunicTool, luckChance, props, min, max );

			return item;
		}

		public static Item Enchant(Mobile from, int enchant, Item item)
		{
			if (item == null) return null;
			
			/*
			@ 500 -- 5-6 properties, 80-100% intensity
			@ 400 -- 4-6 properties, 64-100% intensity
			@ 300 -- 3-5 properties, 48-75% intensity
			@ 200 -- 2-3 properties, 32-50% intensity
			@ 100 -- 1-1 properties, 25-25% intensity
			*/
			int props = Utility.RandomMinMax(enchant / 100, Math.Min(6, enchant / 60));
			if (props < 1) return item;

			int min = (int)Math.Max(25, enchant / 6.25);
			int max = Math.Max(25, Math.Min(100, enchant / 4));

			return Enchant(from, item, props, min, max, false);
		}

		public static void MakeFixedDrop( Mobile creature, Item item )
		{
			MakeFixedDrop( creature, creature, item );
		}

		public static void MakeFixedDrop( Mobile creature, IEntity locationEntity, Item item )
		{
			int level = LootPackChange.MonsterLevel( IntelligentAction.GetCreatureLevel( creature ) );
			int dungeonLevelBonus = Server.Difficult.GetDifficultyBounded( locationEntity );
			int levelNormalized = NormalizeLevel(level, dungeonLevelBonus);

			MakeFixedDrop( item, levelNormalized );
		}

		public static void MakeFixedDrop( Item item, int level )
		{
			level = Math.Max(1, Math.Min(10, level)); // 1 to 10

			switch( level )
			{
				case 1:  BaseRunicTool.ApplyAttributes( item, 1, 1,  5,  40, true ); break;
				case 2:  BaseRunicTool.ApplyAttributes( item, 1, 2,  5,  40, true ); break;
				case 3:  BaseRunicTool.ApplyAttributes( item, 1, 3,  5,  40, true ); break;
				case 4:  BaseRunicTool.ApplyAttributes( item, 2, 2, 20,  60, true ); break;
				case 5:  BaseRunicTool.ApplyAttributes( item, 2, 3, 20,  60, true ); break;
				case 6:  BaseRunicTool.ApplyAttributes( item, 2, 4, 40,  60, true ); break;
				case 7:  BaseRunicTool.ApplyAttributes( item, 3, 3, 40,  80, true ); break;
				case 8:  BaseRunicTool.ApplyAttributes( item, 3, 4, 40,  80, true ); break;
				case 9:  BaseRunicTool.ApplyAttributes( item, 4, 4, 60, 100, true); break;
				case 10: BaseRunicTool.ApplyAttributes( item, 5, 5, 80, 100, true); break;
			}
		}

		public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, string quantity ) : this( atSpawnTime, items, chance, new LootPackDice( quantity ), 0, 0, 0 )
		{
		}

		public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, int quantity ) : this( atSpawnTime, items, chance, new LootPackDice( 0, 0, quantity ), 0, 0, 0 )
		{
		}

		public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, string quantity, int maxProps, int minIntensity, int maxIntensity ) : this( atSpawnTime, items, chance, new LootPackDice( quantity ), maxProps, minIntensity, maxIntensity )
		{
		}

		public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, int quantity, int maxProps, int minIntensity, int maxIntensity ) : this( atSpawnTime, items, chance, new LootPackDice( 0, 0, quantity ), maxProps, minIntensity, maxIntensity )
		{
		}

		public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, LootPackDice quantity, int maxProps, int minIntensity, int maxIntensity )
		{
			m_AtSpawnTime = atSpawnTime;
			m_Items = items;
			m_Chance = (int)(100 * chance);
			m_Quantity = quantity;
			m_MaxProps = maxProps;
			m_MinIntensity = minIntensity;
			m_MaxIntensity = maxIntensity;
		}

		public int GetBonusProperties()
		{
			int p0=0, p1=0, p2=0, p3=0, p4=0, p5=0;

			int props = m_MaxProps;
				if ( props > 5 ){ props = 5; }

			switch ( props )
			{
				case 1: p0= 3; p1= 1; break;
				case 2: p0= 6; p1= 3; p2= 1; break;
				case 3: p0=10; p1= 6; p2= 3; p3= 1; break;
				case 4: p0=16; p1=12; p2= 6; p3= 5; p4=1; break;
				case 5: p0=30; p1=25; p2=20; p3=15; p4=9; p5=1; break;
			}

			int pc = p0+p1+p2+p3+p4+p5;

			int rnd = Utility.Random( pc );

			if ( rnd < p5 )
				return 5;
			else
				rnd -= p5;

			if ( rnd < p4 )
				return 4;
			else
				rnd -= p4;

			if ( rnd < p3 )
				return 3;
			else
				rnd -= p3;

			if ( rnd < p2 )
				return 2;
			else
				rnd -= p2;

			if ( rnd < p1 )
				return 1;

			return 0;
		}
	}

	public class LootPackItem
	{
		private Type m_Type;
		private int m_Chance;

		public int Chance
		{
			get{ return m_Chance; }
			set{ m_Chance = value; }
		}

		public Item Construct( bool playOrient, Mobile creator )
		{
			try
			{
				Item item;
				Type type = m_Type;

				creator = creator != null ? MobileUtilities.TryGetMasterPlayer(creator) : null;
				if (creator is PlayerMobile)
				{
					var player = (PlayerMobile)creator;
					var tasting = (int)player.Skills[SkillName.Tasting].Value;
					if (30 <= tasting && Utility.RandomMinMax(0, 125) < tasting)
					{
						// Chance to increase potion tier
						if ( type == typeof( LesserHealPotion ) )
							type = Utility.RandomMinMax(0, 60) < tasting ? typeof( HealPotion ) : type;
						else if ( type == typeof( HealPotion ) )
							type = Utility.RandomMinMax(60, 125) < tasting ? typeof( GreaterHealPotion ) : type;
					}
				}

				if ( type == typeof( BaseRanged ) )
					item = Loot.RandomRangedWeapon( playOrient );
				else if ( type == typeof( BaseWeapon ) )
					item = Loot.RandomWeapon( playOrient );
				else if ( type == typeof( BaseArmor ) )
					item = Loot.RandomArmor( playOrient );
				else if ( type == typeof( BaseHat ) )
					item = Loot.RandomHats( playOrient );
				else if ( type == typeof( BaseClothing ) )
				{
					if ( Utility.Random( 4 ) == 0 )
						item = Loot.RandomHats( playOrient );
					else
						item = Loot.RandomClothing( playOrient );
				}
				else if ( type == typeof( BaseShield ) )
					item = Loot.RandomShield();
				else if ( type == typeof( BaseTrinket ) )
					item = Loot.RandomJewelry();
				else if ( type == typeof( BaseQuiver ) )
					item = Loot.RandomQuiver();
				else if ( type == typeof( BaseInstrument ) )
					item = Loot.RandomInstrument();
				else if ( type == typeof( Spellbook ) )
					item = Loot.RandomSpellbook( playOrient );
				else if ( type == typeof( Amber ) ) // gem
					item = Loot.RandomGem();
				else if ( type == typeof( Artifact_YashimotosHatsuburi ) )
					item = Loot.RandomArty();
				else if ( type == typeof( LesserHealPotion ) )
					item = Loot.RandomPotion( 4, false );
				else if ( type == typeof( HealPotion ) )
					item = Loot.RandomPotion( 8, false );
				else if ( type == typeof( GreaterHealPotion ) )
					item = Loot.RandomPotion( 12, false );
				else if ( type == typeof( BlackPearl ) )
				{
					item = Loot.RandomPossibleReagent();

					var cooking = creator != null ? (int)creator.Skills[SkillName.Cooking].Value : 0;
					if (creator is PlayerMobile && item.Stackable && 30 <= cooking && Utility.RandomMinMax(0, 125) < cooking)
						item.Amount += (int)(0.2 * item.Amount);
				}
				else if ( type == typeof( GoldBricks ) )
					item = Loot.RandomSArty( playOrient, null );
				else if ( type == typeof( ClumsyScroll ) ) // low scroll
					item = Loot.RandomScroll( 4 );
				else if ( type == typeof( ArchCureScroll ) ) // med scroll
					item = Loot.RandomScroll( 8 );
				else if ( type == typeof( ChainLightningScroll ) ) // high scroll
					item = Loot.RandomScroll( 12 );
				else if ( type == typeof( FoeRequiemScroll ) )
					item = Loot.RandomSong();
				else if ( type == typeof( Lute ) )
					item = Loot.RandomInstrument();
				else
					item = Activator.CreateInstance( type ) as Item;

				return item;
			}
			catch
			{
			}

			return null;
		}

		public LootPackItem( Type type, int chance )
		{
			m_Type = type;
			m_Chance = chance;
		}
	}

	public class LootPackDice
	{
		private int m_Count, m_Sides, m_Bonus;

		public int Count
		{
			get{ return m_Count; }
			set{ m_Count = value; }
		}

		public int Sides
		{
			get{ return m_Sides; }
			set{ m_Sides = value; }
		}

		public int Bonus
		{
			get{ return m_Bonus; }
			set{ m_Bonus = value; }
		}

		public int Roll()
		{
			int v = m_Bonus;
			double w;

			for ( int i = 0; i < m_Count; ++i )
			   v += Utility.Random( 1, m_Sides );

			w = v * (MyServerSettings.GetGoldCutRate() * .01);

			return (int)w;
		}

		public LootPackDice( string str )
		{
			int start = 0;
			int index = str.IndexOf( 'd', start );

			if ( index < start )
				return;

			m_Count = Utility.ToInt32( str.Substring( start, index-start ) );

			bool negative;

			start = index + 1;
			index = str.IndexOf( '+', start );

			if ( negative = (index < start) )
				index = str.IndexOf( '-', start );

			if ( index < start )
				index = str.Length;

			m_Sides = Utility.ToInt32( str.Substring( start, index-start ) );

			if ( index == str.Length )
				return;

			start = index + 1;
			index = str.Length;

			m_Bonus = Utility.ToInt32( str.Substring( start, index-start ) );

			if ( negative )
				m_Bonus *= -1;
		}

		public LootPackDice( int count, int sides, int bonus )
		{
			m_Count = count;
			m_Sides = sides;
			m_Bonus = bonus;
		}
	}

	public class LootPackChange
	{
		/// <returns>True if the item is deleted</returns>
		public static bool RemoveItem( Item item, Mobile from, int level )
		{
			// Disallow items based on mob level
			if ( !(Utility.RandomMinMax( 3, 12 ) > level) )
			{
				bool delete = false;
				switch ( CraftResources.GetType( item.Resource ) )
				{
					case CraftResourceType.Skin:
					case CraftResourceType.Block:
					case CraftResourceType.Scales:
						delete = true;
						break;

					case CraftResourceType.Metal:
						BaseTrinket trinket = item as BaseTrinket;
						delete = trinket != null && trinket.GemType == GemType.Pearl;
						break;
				}

				if ( delete )
				{
					if ( item.Parent is NotIdentified )
						((NotIdentified)(item.Parent)).Delete();

					item.Delete();
					return true;
				}
			}

			return false;
		}

		public static Item ChangeItem( Item item, Mobile from, int level )
		{
			bool resourceMe = false;

			if ( ResourceMods.RarityIgnore( item.Resource ) )
				resourceMe = true;
			else if ( Utility.RandomMinMax( 0, 12 ) < level )
				resourceMe = true;

			if ( resourceMe )
				ResourceMods.SetRandomResource( true, true, item, CraftResource.None, false, from );

			level = LootPackChange.ScaleLevel( level );

			item = Food.ModifyFood( item, from );				// MAKE STAR TREK TYPE FOOD OR RACE SPECIFIC
			item = ResourceMods.GetRandomItem( item, from );	// MAKE RESOURCES FOR THE AREA

			if ( Worlds.isSciFiRegion( from ) )
			{
				if ( !item.NotModAble )
				{
					if ( item.Catalog == Catalogs.Trinket || item is BaseQuiver || item is BaseHarvestTool || item is BaseTool || item.Catalog == Catalogs.Scroll || item.Catalog == Catalogs.Book || item.Catalog == Catalogs.Stone )
					{
						item.Delete();
						item = Loot.RandomSciFiItems();
					}

					if ( item is SkeletonsKey ){ item.Name = "minimal access card"; item.ItemID = 0x3A75; item.Hue = 0x59A; item.Technology = true; }
					else if ( item is MasterSkeletonsKey ){ item.Name = "full access card"; item.ItemID = 0x3A75; item.Hue = 0x66D; item.Technology = true; }
					else if ( item is Lockpick ){ item.Name = "security card"; item.ItemID = 0x3A75; item.Hue = 0x53C; item.Technology = true; }
					else if ( item is BasePotion ){ Server.Items.BasePotion.MakePillBottle( item ); item.Technology = true; }
					else if ( item is Krystal ){ item.Technology = true; }
					else if ( item is Spellbook ){ item.Delete(); item = new DataPad(); item.Technology = true; }
					else if ( item is StarSapphire ){ item.ItemID = 0xF26; item.Hue = 0x996; item.Name = "kyber crystal"; item.Technology = true; }
					else if ( item is Emerald ){ item.ItemID = 0xF25; item.Hue = 0x950; item.Name = "etaan crystal"; item.Technology = true; }
					else if ( item is Sapphire ){ item.ItemID = 0xF2D; item.Hue = 0xB40; item.Name = "trilithium crystal"; item.Technology = true; }
					else if ( item is Ruby ){ item.ItemID = 0xF16; item.Hue = 0x94F; item.Name = "lava crystal"; item.Technology = true; }
					else if ( item is Citrine ){ item.ItemID = 0xF21; item.Hue = 0xB54; item.Name = "dilithium crystal"; item.Technology = true; }
					else if ( item is Amethyst ){ item.ItemID = 0xF10; item.Hue = 0x94A; item.Name = "dantari crystal"; item.Technology = true; }
					else if ( item is Tourmaline ){ item.ItemID = 0xF19; item.Hue = 0x86C; item.Name = "vexxtal crystal"; item.Technology = true; }
					else if ( item is Amber ){ item.ItemID = 0xF13; item.Hue = 0x8FC; item.Name = "nova crystal"; item.Technology = true; }
					else if ( item is Diamond ){ item.ItemID = 0xF15; item.Hue = 0x90F; item.Name = "permafrost crystal"; item.Technology = true; }
					else if ( item is Bedroll ){ item.Name = "sleeping bag"; item.Hue = Utility.RandomColor(0); item.Technology = true; }
					else if ( item is Spyglass ){ item.Name = "binoculars"; item.ItemID = 0x3562; item.Technology = true; }
					else if ( item is ArtifactManual ){ item.Name = "magnifying lense"; item.ItemID = 0x202F; item.Hue = 0; item.Technology = true; }
					else if ( item is GolemManual ){ item.Delete(); item = new RobotSchematics(); item.Technology = true; }
					else if ( item is BaseHat && Utility.RandomBool() ) // ONLY HALF THE HATS BECOME GOGGLES
					{
						item.ItemID = Utility.RandomList( 0x2FB8, 0x3172 );
						item.Name = "Goggles";
						item.ColorText1 = null;
						item.ColorText2 = null;
						item.Technology = true; 
						switch( Utility.RandomMinMax( 0, 10 ) )
						{
							case 1: item.Name = "Pilot Goggles"; break;
							case 2: item.Name = "Medical Goggles"; break;
							case 3: item.Name = "Security Goggles"; break;
							case 4: item.Name = "Engineering Goggles"; break;
							case 5: item.Name = "Science Goggles"; break;
							case 6: item.Name = "Laboratory Goggles"; break;
							case 7: item.Name = "Safety Goggles"; break;
							case 8: item.Name = "Sun Goggles"; break;
							case 9: item.Name = "Night Goggles";
								if ( item is BaseClothing ){ ((BaseClothing)item).Attributes.NightSight = 1; }
								break;
							case 10: item.Name = "Soldier Goggles"; break;
						}
						ResourceMods.SetRandomResource( true, true, item, CraftResource.Iron, false, from );
					}
					else if ( item is BaseInstrument )
					{
						item.ColorText2 = null;
						item.ColorText1 = Server.Misc.RandomThings.GetRandomAlienRace() + " " + item.Name + "";
						item.ColorHue1 = "11DADA";
					}
				}
			}
			if ( item is CandleLarge || item is Candelabra || item is CandelabraStand )
			{
				ResourceMods.SetRandomResource( false, false, item, CraftResource.Iron, false, null );
				if ( Utility.RandomBool() )
					ResourceMods.SetRandomResource( false, false, item, CraftResource.AmethystBlock, false, null );
			}
			if ( GetPlayerInfo.EvilPlay( from ) )
			{
				if ( item is UnusualDyes ){ item.Hue = Utility.RandomEvilHue(); }
			}
			if ( LootPackEntry.playOrient( from ) )
			{
				if ( item is DDRelicWeapon ){ DDRelicWeapon.MakeOriental( item ); }
				else if ( item is DDRelicStatue ){ DDRelicStatue.MakeOriental( item ); }
				else if ( item is DDRelicBanner && item.ItemID != 0x2886 && item.ItemID != 0x2887 ){ DDRelicBanner.MakeOriental( item ); }
			}

			if ( item is MagicalWand )
			{
				item.Delete();
				item = new MagicalWand( level );
			}
			else if ( ( item is BaseArmor || item is BaseWeapon || item is BaseClothing || item is BaseTrinket || item is BaseInstrument ) && Utility.Random(100) == 0 )
			{
				SpellItems.setSpell( level, item );
			}

			if ( item == null )
				item = Loot.RandomCoins( from );

			RandomThings.SpecialName( item, from, from.Region );

			if ( !Worlds.isSciFiRegion( from ) && item is BaseTrinket && item.Catalog == Catalogs.Jewelry )
				BaseTrinket.RandomGem( (BaseTrinket)item );

			if ( item.Hue == 0 && item is BaseClothing )
				item.Hue = Utility.RandomColor(0);

			if ( item.Hue == 0 && ( item is BaseBook || item is Runebook || item is Spellbook ) && !(item is ElementalSpellbook) && item.Resource == CraftResource.RegularLeather )
				item.Hue = Utility.RandomColor(0);

			SetAmount( item, level );

			return item;
		}

		public static void SetAmount( Item item, int level )
		{
			if ( item is BaseReagent )
				item.Amount = Utility.RandomMinMax( 2, 8 ) * level;
			else if ( item is MagicalDyes )
				item.Amount = Utility.RandomMinMax( 1, level+2 );
			else if ( item is MageEye || item is HarpoonRope || item is Arrow || item is Bolt || item is Krystal || item is ThrowingWeapon )
				item.Amount = Utility.RandomMinMax( 5, 20 ) * level;
			else if ( item is Shuriken )
				item.Amount = Utility.RandomMinMax( 2, 10 ) * level;
			else if ( item is BaseTool )
				((BaseTool)item).UsesRemaining = Utility.RandomMinMax( 3, 75 );
			else if ( item is BaseHarvestTool )
				((BaseHarvestTool)item).UsesRemaining = Utility.RandomMinMax( 3, 75 );
			else if ( item.Catalog == Catalogs.Crafting && item.Stackable )
				item.Amount = Utility.RandomMinMax( 2, 10 ) * level;
			else if ( item.Catalog == Catalogs.Gem && item.Stackable )
				item.Amount = Utility.RandomMinMax( 1, level );
			else if ( item.Catalog == Catalogs.Potion || item.Catalog == Catalogs.Scroll )
				item.Amount = 1;
			else if ( item.Stackable ) 
				item.Amount = Utility.RandomMinMax( 1, 10 );
		}

		public static void MakeCoins( Container pack, Mobile m )
		{
			if ( pack != null )
			{
				int amount = 0;
				BaseCreature bc = null;
					if ( m is BaseCreature )
						bc = (BaseCreature)m;

				List<Item> pockets = new List<Item>();
				foreach( Item i in pack.Items )
				{
					if ( i is Gold )
					{
						pockets.Add(i);
						amount += i.Amount;
					}
				}
				foreach ( Item coins in pockets )
				{
					coins.Delete();
				}

				if ( amount > 0 )
				{
					if ( Worlds.isSciFiRegion( m ) )
					{
						int xormite = (int)(amount/3);
						pack.DropItem( new DDXormite( xormite ) );
						if ( bc != null ){ bc.Coins = xormite; bc.CoinType = "xormite"; }
					}
					else if ( (Region.Find( m.Location, m.Map )).IsPartOf( "the Mines of Morinia" ) && Utility.RandomMinMax( 1, 5 ) == 1 )
					{
						int crystals = (int)(amount/5);
						pack.DropItem( new Crystals( crystals ) );
						if ( bc != null ){ bc.Coins = crystals; bc.CoinType = "crystals"; }
					}
					else if ( m.Land == Land.Underworld )
					{
						int jewels = (int)(amount/2);
						pack.DropItem( new DDJewels( jewels ) );
						if ( bc != null ){ bc.Coins = jewels; bc.CoinType = "jewels"; }
					}
					else
					{
						if ( bc != null ){ bc.Coins = amount; bc.CoinType = "gold"; }

						amount = amount * 10;

						if (Utility.RandomMinMax( 1, 100 ) > 99)
						{
							int nGm = 20;
							int nGms = (int)Math.Floor((decimal)(amount/nGm));
							if (nGms > 0)
							{
								int nGemstones = Utility.RandomMinMax( 1, nGms );
									if ( nGemstones < 10 ){ nGemstones = Utility.RandomMinMax( 10, 15 ); }
								pack.DropItem( new DDGemstones( nGemstones ) );
								amount = amount - (nGemstones * nGm);
							}
						}
						if (Utility.RandomMinMax( 1, 100 ) > 95)
						{
							int nGs = 10;
							int nGps = (int)Math.Floor((decimal)(amount/nGs));
							if (nGps > 0)
							{
								int nNuggets = Utility.RandomMinMax( 1, nGps );
									if ( nNuggets < 10 ){ nNuggets = Utility.RandomMinMax( 10, 15 ); }
								pack.DropItem( new DDGoldNuggets( nNuggets ) );
								amount = amount - (nNuggets * nGs);
							}
						}
						if (Utility.RandomMinMax( 1, 100 ) > 66)
						{
							int nGp = 10;
							int nGpp = (int)Math.Floor((decimal)(amount/nGp));
							if (nGpp > 0)
							{
								int nGold = Utility.RandomMinMax( 1, nGpp );
									if ( nGold < 10 ){ nGold = Utility.RandomMinMax( 10, 15 ); }
								pack.DropItem( new Gold( nGold ) );
								amount = amount - (nGold * nGp);
							}
						}
						if (Utility.RandomMinMax( 1, 100 ) > 33)
						{
							int nSp = 5;
							int nSpp = (int)Math.Floor((decimal)(amount/nSp));
							if (nSpp > 0)
							{
								int nSilver = Utility.RandomMinMax( 1, nSpp );
									if ( nSilver < 10 ){ nSilver = Utility.RandomMinMax( 10, 15 ); }
								pack.DropItem( new DDSilver( nSilver ) );
								amount = amount - (nSilver * nSp);
							}
						}
						if (amount > 0)
						{
							if ( amount < 10 ){ amount = Utility.RandomMinMax( 10, 15 ); }
							foreach (var item in ItemUtilities.AddStacks(amount, () => new DDCopper()))
								pack.DropItem( item );
						}
					}
				}
			}
		}

		public static int MonsterLevel( int level )
		{
			level = (int)( level / 10 );

			if ( level < 1 )
				level = 1;
			if ( level > 12 )
				level = 12;

			return level;
		}

		public static int ScaleLevel( int level )
		{
			if ( level < 1 )
				level = 1;
			if ( level > 12 )
				level = 12;

			return level;
		}

		public static int FillCycle( int level )
		{
			level = ScaleLevel( level );

			int filler = (int)(level/2);
				if (filler < 1)
					filler = 1;

			int fillup = filler + 2;

			return Utility.RandomMinMax( filler,fillup );
		}

		public static void AddGoldToContainer( int amt, Container box, Mobile from, int level )
		{
			Container bag = new Bag();

			if ( amt < 1 )
			{
				amt = ( ScaleLevel( level ) + 1 ) * Utility.RandomMinMax( 40, 160 );
				amt = (int)(amt * (MyServerSettings.GetGoldCutRate() * .01));
			}

			bag.DropItem( new Gold( amt ) );

			MakeCoins( bag, from );

			List<Item> pockets = new List<Item>();
			foreach( Item i in bag.Items )
			{
				pockets.Add(i);
			}
			foreach ( Item coins in pockets )
			{
				box.DropItem(coins);
			}

			bag.Delete();
		}
	}
}