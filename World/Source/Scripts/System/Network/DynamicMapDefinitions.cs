using System.Collections.Generic;

namespace Server.Network
{
    public class DynamicMapDefinitions
    {
		public static bool Enabled = false;

        public static void Initialize()
        {
            EventSink.Login += OnLogin;
        }

        private static void OnLogin(LoginEventArgs e)
        {
            if (e == null || e.Mobile == null || !Enabled)
                return;

            NetState ns = e.Mobile.NetState;

            if (ns == null)
                return;

            ns.Send(new MapDefinitionsPacket());
        }
    }

    public sealed class MapDefinitionsPacket : Packet
    {
        public MapDefinitionsPacket() : base(0x3F)
        {
            List<Map> maps = new List<Map>();

            foreach (Map m in Map.AllMaps)
            {
                if (m == null || m == Map.Internal)
                    continue;

                if (m.MapID < 0 || m.MapID > 0x7E)
                    continue;

                if (m.Width <= 0 || m.Height <= 0)
                    continue;

                maps.Add(m);
            }

            EnsureCapacity(3 + 10 + 1 + 1 + (maps.Count * 5));

            for (int i = 0; i < 10; i++)
                m_Stream.Write((byte)0);

            m_Stream.Write((byte)0x10);
            m_Stream.Write((byte)maps.Count);

            foreach (Map m in maps)
            {
                m_Stream.Write((byte)m.MapID);
                m_Stream.Write((ushort)m.Width);
                m_Stream.Write((ushort)m.Height);
            }
        }
    }
}
