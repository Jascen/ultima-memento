using System.Collections.Generic;
using System.Linq;
using Server.Gumps;

namespace Server.Engines.Harvest
{
    public class RichVeinGump : Gump
    {
        private class _Counter
        {
            public Map Map { get; set; }
            public int Nodes { get; set; }
            public int PersistentSpawners { get; set; }
            public int VolatileSpawners { get; set; }
        }

        private const string TextColor = "#FFFFFF";

        public RichVeinGump(Mobile from) : base(50, 50)
        {
            from.CloseGump(typeof(RichVeinGump));

            const int PADDING = 20;
            const int COLUMN_WIDTH = 140;
            const int GUMP_WIDTH = 2 * PADDING + 4 * COLUMN_WIDTH;
            const int GUMP_HEIGHT = 250;

            AddImageTiled(0, 0, GUMP_WIDTH, GUMP_HEIGHT, 163); // Background

            int y = 20;
            AddHtml(0, y, GUMP_WIDTH, 20, string.Format("<center><basefont color={0}>Rich Veins</basefont></center>", TextColor), false, false);
            y += 40;

            var x = PADDING;
            AddHtml(x, y, COLUMN_WIDTH, 20, string.Format("<left><basefont color={0}>Map Name</basefont></left>", TextColor), false, false);
            x += COLUMN_WIDTH;

            AddHtml(x, y, COLUMN_WIDTH, 20, string.Format("<center><basefont color={0}>Nodes</basefont></center>", TextColor), false, false);
            x += COLUMN_WIDTH;

            AddHtml(x, y, COLUMN_WIDTH, 20, string.Format("<center><basefont color={0}>Dynamic Spawners</basefont></center>", TextColor), false, false);
            x += COLUMN_WIDTH;

            AddHtml(x, y, COLUMN_WIDTH, 20, string.Format("<center><basefont color={0}>Persistent Spawners</basefont></center>", TextColor), false, false);
            x += COLUMN_WIDTH;

            y += 20;

            var counters = CountEntities();

            foreach (var counter in counters.OrderBy(counter => counter.Map.MapID))
            {
                x = PADDING;
                AddHtml(x, y, COLUMN_WIDTH, 20, string.Format("<left><basefont color={0}>{1}</basefont></left>", TextColor, counter.Map.Name), false, false);
                x += COLUMN_WIDTH;

                AddHtml(x, y, COLUMN_WIDTH, 20, string.Format("<center><basefont color={0}>{1}</basefont></center>", TextColor, counter.Nodes), false, false);
                x += COLUMN_WIDTH;

                AddHtml(x, y, COLUMN_WIDTH, 20, string.Format("<center><basefont color={0}>{1}</basefont></center>", TextColor, counter.VolatileSpawners), false, false);
                x += COLUMN_WIDTH;

                AddHtml(x, y, COLUMN_WIDTH, 20, string.Format("<center><basefont color={0}>{1}</basefont></center>", TextColor, counter.PersistentSpawners), false, false);
                x += COLUMN_WIDTH;

                y += 20;
            }
        }

        private IReadOnlyList<_Counter> CountEntities()
        {
            var countersByMap = new Dictionary<Map, _Counter>();

            _Counter counter;
            foreach (var item in World.Items.Values)
            {
				if (item.Map == null) continue;
                if (item.Map == Map.Internal) continue;
                if (item.Deleted) continue;

                if (!countersByMap.TryGetValue(item.Map, out counter)) countersByMap[item.Map] = counter = new _Counter { Map = item.Map };

                if (item is RichVeinMineable)
                {
                    counter.Nodes++;
                }
                else if (item is RichVeinSpawner)
                {
                    if (item is RichVeinSpawnerPersistent)
                    {
                        counter.PersistentSpawners++;
                    }
                    else
                    {
                        counter.VolatileSpawners++;
                    }
                }
            }

            return countersByMap.Values.ToList();
        }
    }
}