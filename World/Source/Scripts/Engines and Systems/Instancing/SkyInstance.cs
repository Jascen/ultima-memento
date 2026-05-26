using System;
using System.Collections.Generic;
using Server;

namespace Server.Engines.Instancing
{
	public class SkyInstance
	{
		private readonly int m_Id;
		private Serial m_OwnerSerial;
		private DateTime m_LastTouched;
		private bool m_Loaded;
		private readonly List<Item> m_TempItems = new List<Item>();
		private Item m_Floor;
		private SkyInstanceRegion m_Region;
		private readonly List<Serial> m_Friends = new List<Serial>();

		public int Id { get { return m_Id; } }
		public Serial OwnerSerial { get { return m_OwnerSerial; } set { m_OwnerSerial = value; } }
		public DateTime LastTouched { get { return m_LastTouched; } set { m_LastTouched = value; } }
		public bool Loaded { get { return m_Loaded; } set { m_Loaded = value; } }
		public List<Item> TempItems { get { return m_TempItems; } }
		public Item Floor { get { return m_Floor; } set { m_Floor = value; } }
		public SkyInstanceRegion Region { get { return m_Region; } set { m_Region = value; } }
		public List<Serial> Friends { get { return m_Friends; } }

		public SkyInstance( int id )
		{
			m_Id = id;
			m_LastTouched = DateTime.Now;
		}

		public Mobile FindOwner()
		{
			return World.FindMobile( m_OwnerSerial );
		}

		public void Touch()
		{
			m_LastTouched = DateTime.Now;
		}
	}
}
