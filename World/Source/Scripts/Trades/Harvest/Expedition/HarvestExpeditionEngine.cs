using Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Harvest.Expedition
{
    public class HarvestExpeditionOption
    {
        public HarvestExpeditionType Type { get; set; }
        public int DurationHours { get; set; }
    }

    public class HarvestExpeditionEngine
    {
        private static HarvestExpeditionEngine m_Engine;

        private Dictionary<Serial, HarvestExpedition> ExpeditionsByPlayer;
        private List<HarvestExpeditionOption> Options;
        private DateTime NextRestock;

        private HarvestExpeditionEngine()
        {
        }

        public static HarvestExpeditionEngine Instance
        {
            get
            {
                if (m_Engine == null)
                    m_Engine = new HarvestExpeditionEngine();

                return m_Engine;
            }
        }

        public static void Configure()
        {
            EventSink.WorldSave += OnWorldSave;
            EventSink.WorldLoad += OnWorldLoad;
        }

        private const string RootDirectory = "Saves/HarvestExpedition";
        private const string Filename = "Expeditions.bin";

        private static void OnWorldSave(WorldSaveEventArgs args)
        {
            try
            {
                if (!Directory.Exists(RootDirectory))
                    Directory.CreateDirectory(RootDirectory);

                GenericWriter writer = new BinaryFileWriter(Path.Combine(RootDirectory, Filename), true);

                writer.Write(0); // version

                var expeditionsByPlayer = Instance.ExpeditionsByPlayer;
                var count = expeditionsByPlayer != null ? expeditionsByPlayer.Count : 0;
                writer.Write(count);
                if (0 < count)
                {
                    foreach (var expedition in expeditionsByPlayer.Values)
                    {
                        expedition.Serialize(writer);
                    }
                }

                writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
        }

        private static void OnWorldLoad()
        {
            Directory.CreateDirectory(RootDirectory);

            var filepath = Path.Combine(RootDirectory, Filename);
            if (File.Exists(filepath))
            {
                try
                {
                    using (FileStream bin = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var sw = Stopwatch.StartNew();
                        GenericReader reader = new BinaryFileReader(new BinaryReader(bin));

                        int version = reader.ReadInt();

                        int count = reader.ReadInt();
                        for (int i = 0; i < count; ++i)
                        {
                            var expedition = new HarvestExpedition(reader);
                            Instance.ExpeditionsByPlayer.Add(expedition.Owner.Serial, expedition);
                            expedition.Start();
                        }

                        Console.WriteLine("Loaded '{0}' expeditions in {1} ms", count, sw.ElapsedMilliseconds);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.Source);
                    Console.WriteLine(e.StackTrace);
                }
            }

            var timer = new RandomizeTimer();
            timer.Start();
        }

        private void RefreshHarvestOptions()
        {
        }

        private HarvestExpeditionOption CreateHarvestOption()
        {
            return new HarvestExpeditionOption
            {
                Type = (HarvestExpeditionType)Utility.RandomList(
                    new[]
                    {
                        (int) HarvestExpeditionType.Mining,
                        (int) HarvestExpeditionType.Lumberjacking
                    }),
                DurationHours = Utility.RandomList(new[] { 1, 1, 2, 4, 8, 12, 24 })
            };
        }

        private class RandomizeTimer : Timer
        {
            public RandomizeTimer() : base(TimeSpan.Zero, TimeSpan.FromHours(1))
            {
            }
        }
    }
}