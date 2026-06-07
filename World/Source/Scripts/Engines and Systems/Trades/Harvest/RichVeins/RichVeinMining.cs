using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Engines.Harvest
{
    public class RichVeinMining : Mining
    {
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

        private static RichVeinMining m_System;

        private readonly HarvestDefinition m_rareNodeDefinition;

        private RichVeinMining()
        {
            #region Mining rare nodes

            HarvestDefinition rareNodeDefinition = new HarvestDefinition
            {
                BankWidth = 1,
                BankHeight = 1,
                MinTotal = 20,
                MaxTotal = 34,
                MinRespawn = TimeSpan.FromMinutes(50.0),
                MaxRespawn = TimeSpan.FromMinutes(70.0),
                Skill = SkillName.Mining,
                Tiles = m_RareNodeItemIds,
                MaxRange = 3,
                ConsumedPerHarvest = int.MaxValue,
                ConsumedPerIslesDreadHarvest = int.MaxValue,

                EffectActions = new int[] { 11 },
                EffectSounds = new int[] { 0x125, 0x126 },
                EffectCounts = new int[] { 3, 3, 3, 4, 4, 5 },
                EffectDelay = TimeSpan.FromSeconds(0.8),
                EffectSoundDelay = TimeSpan.FromSeconds(0.45),
                NoResourcesMessage = 503040, // There is no metal here to mine.
                DoubleHarvestMessage = 50304, // Someone has gotten to the metal before you.
                TimedOutOfRangeMessage = 503041, // You have moved too far away to continue mining.
                OutOfRangeMessage = 500446, // That is too far away.
                FailMessage = 503043, // You loosen some rocks but fail to find any useable ore.
                PackFullMessage = 500720, // You don't have enough room in your backpack!
                ToolBrokeMessage = 1044038, // You have worn out your tool!

                RandomizeVeins = true,
                Resources = new HarvestResource[]
                {
                    new HarvestResource( 00.0, 00.0, 100.0, "", typeof( IronOre ),          typeof( Granite ) ),
                    new HarvestResource( 65.0, 25.0, 105.0, "", typeof( DullCopperOre ),    typeof( DullCopperGranite ),    typeof( DullCopperElemental ) ),
                    new HarvestResource( 70.0, 30.0, 110.0, "", typeof( ShadowIronOre ),    typeof( ShadowIronGranite ),    typeof( ShadowIronElemental ) ),
                    new HarvestResource( 75.0, 35.0, 115.0, "", typeof( CopperOre ),        typeof( CopperGranite ),        typeof( CopperElemental ) ),
                    new HarvestResource( 80.0, 40.0, 120.0, "", typeof( BronzeOre ),        typeof( BronzeGranite ),        typeof( BronzeElemental ) ),
                    new HarvestResource( 85.0, 45.0, 125.0, "", typeof( GoldOre ),          typeof( GoldGranite ),          typeof( GoldenElemental ) ),
                    new HarvestResource( 90.0, 50.0, 130.0, "", typeof( AgapiteOre ),       typeof( AgapiteGranite ),       typeof( AgapiteElemental ) ),
                    new HarvestResource( 95.0, 55.0, 135.0, "", typeof( VeriteOre ),        typeof( VeriteGranite ),        typeof( VeriteElemental ) ),
                    new HarvestResource( 99.0, 59.0, 139.0, "", typeof( ValoriteOre ),      typeof( ValoriteGranite ),      typeof( ValoriteElemental ) ),
                    new HarvestResource( 100.1, 69.0, 140.0, "", typeof( DwarvenOre ),      typeof( DwarvenGranite ),       typeof( EarthElemental ) )
                },
            };

            var res = rareNodeDefinition.Resources;
            rareNodeDefinition.Veins = new HarvestVein[]
                {
                    new HarvestVein( 0, 0.0, res[0], null   ), // Iron
                    new HarvestVein( 46.0, 0, res[1], res[0] ), // Dull Copper
					new HarvestVein( 15.0, 0, res[2], res[0] ), // Shadow Iron
					new HarvestVein( 11.0, 0, res[3], res[0] ), // Copper
					new HarvestVein( 08.0, 0, res[4], res[0] ), // Bronze
					new HarvestVein( 06.0, 0, res[5], res[0] ), // Gold
					new HarvestVein( 05.0, 0, res[6], res[0] ), // Agapite
					new HarvestVein( 04.0, 0, res[7], res[0] ), // Verite
					new HarvestVein( 03.0, 0, res[8], res[0] ), // Valorite
					new HarvestVein( 02.0, 0, res[9], res[0] )  // Dwarven
                };

            m_rareNodeDefinition = rareNodeDefinition;

            Definitions.Add(rareNodeDefinition);

            #endregion Mining rare nodes
        }

        public new static RichVeinMining System
        {
            get
            {
                if (m_System == null)
                    m_System = new RichVeinMining();

                return m_System;
            }
        }

        public HarvestDefinition RareNodeDefinition
        { get { return m_rareNodeDefinition; } }

        public static void Configure()
        {
            Array.Sort(m_RareNodeItemIds);
        }

        public override bool CheckHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            if (!base.CheckHarvest(from, tool, def, toHarvest))
                return false;

            if (toHarvest is RichVeinMineable)
            {
                if (from.Skills[SkillName.Mining].Base < 65)
                {
                    from.SendMessage("That looks beyond your skill level.");
                    return false;
                }
            }

            return true;
        }

        public override bool GetHarvestDetails(Mobile from, Item tool, object toHarvest, out int tileID, out Map map, out Point3D loc)
        {
            if (toHarvest is RichVeinMineable)
            {
                RichVeinMineable obj = (RichVeinMineable)toHarvest;
                tileID = obj.ItemID;
                map = from.Map;
                loc = obj.Location;

                return !obj.Deleted && map != null && map != Map.Internal;
            }

            return base.GetHarvestDetails(from, tool, toHarvest, out tileID, out map, out loc);
        }

        protected override IEnumerable<object> GetNearbySpecialHarvestTargetsAt(Mobile from, Item tool, int x, int y)
        {
            Map map = from.Map;
            if (map == null || map == Map.Internal) yield break;

            IPooledEnumerable eable = map.GetItemsInRange(new Point3D(x, y, from.Z), 0);
            foreach (Item item in eable)
            {
                if (item is RichVeinMineable && item.X == x && item.Y == y && !item.Deleted)
                    yield return item;
            }

            eable.Free();
        }

        public override Type GetResourceType(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
        {
            if (def == m_rareNodeDefinition)
            {
                PlayerMobile pm = from as PlayerMobile;
                if (pm != null && pm.StoneMining && pm.ToggleMiningStone && from.Skills[SkillName.Mining].Base >= 100.0 && 0.1 > Utility.RandomDouble())
                    return resource.Types[1];

                return resource.Types[0];
            }

            return base.GetResourceType(from, tool, def, map, loc, resource);
        }

        public override HarvestVein MutateVein(Mobile from, Item tool, HarvestDefinition def, HarvestBank bank, object toHarvest, HarvestVein vein)
        {
            if (Pickaxe.IsGargoylePickaxe(tool) && def == m_rareNodeDefinition)
            {
                int veinIndex = Array.IndexOf(def.Veins, vein);
                if (veinIndex >= 0 && veinIndex < (def.Veins.Length - 1))
                    return def.Veins[veinIndex + 1];
            }
            else if ((from.HarvestOrdinary || tool is TrainingShovel) && def == m_rareNodeDefinition)
            {
                int veinIndex = Array.IndexOf(def.Veins, vein);
                return def.Veins[0];
            }

            return base.MutateVein(from, tool, def, bank, toHarvest, vein);
        }

        public override void OnHarvestFinished(Mobile from, Item tool, HarvestDefinition def, HarvestVein vein, HarvestBank bank, HarvestResource resource, object harvested)
        {
            base.OnHarvestFinished(from, tool, def, vein, bank, resource, harvested); //

            if (Pickaxe.IsGargoylePickaxe(tool) && def == m_rareNodeDefinition && 0.1 > Utility.RandomDouble())
            {
                HarvestResource res = vein.PrimaryResource;
                if (res == resource && res.Types.Length >= 3)
                {
                    try
                    {
                        Map map = from.Map;

                        if (map == null)
                            return;

                        BaseCreature spawned = Activator.CreateInstance(res.Types[2], new object[] { 25 }) as BaseCreature;

                        if (spawned != null)
                        {
                            int offset = Utility.Random(8) * 2;

                            for (int i = 0; i < m_Offsets.Length; i += 2)
                            {
                                int x = from.X + m_Offsets[(offset + i) % m_Offsets.Length];
                                int y = from.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

                                if (map.CanSpawnMobile(x, y, from.Z))
                                {
                                    spawned.OnBeforeSpawn(new Point3D(x, y, from.Z), map);
                                    spawned.MoveToWorld(new Point3D(x, y, from.Z), map);
                                    spawned.Combatant = from;
                                    return;
                                }
                                else
                                {
                                    int z = map.GetAverageZ(x, y);

                                    if (map.CanSpawnMobile(x, y, z))
                                    {
                                        spawned.OnBeforeSpawn(new Point3D(x, y, z), map);
                                        spawned.MoveToWorld(new Point3D(x, y, z), map);
                                        spawned.Combatant = from;
                                        return;
                                    }
                                }
                            }

                            spawned.OnBeforeSpawn(from.Location, from.Map);
                            spawned.MoveToWorld(from.Location, from.Map);
                            spawned.Combatant = from;
                        }
                    }
                    catch
                    {
                    }
                }
            }

            if (harvested is RichVeinMineable)
            {
                RichVeinMineable obj = (RichVeinMineable)harvested;
                if (bank.Current < 1)
                {
                    obj.Delete();
                }
            }
        }

        #region Tile lists

        private static int[] m_RareNodeItemIds = new int[] // Warning: Sorted during Configure
        {
            0x176C, 0x178A
        };

        #endregion Tile lists
    }
}