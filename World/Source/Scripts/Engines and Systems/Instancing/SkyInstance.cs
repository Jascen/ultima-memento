using System;
using System.Collections.Generic;
using Server;

namespace Server.Engines.Instancing
{
	// One decoration belonging to a dwelling, paired with the dwelling-local
	// coordinate it should occupy when the dwelling is live. Because every pool
	// map shares identical terrain, this coordinate is map-independent: the item
	// lands in the same visible spot no matter which pool map the owner is given.
	public class DwellingItem
	{
		public Item Item;
		public Point3D Loc;

		public DwellingItem( Item item, Point3D loc )
		{
			Item = item;
			Loc = loc;
		}
	}

	// A player's private sky dwelling. The decorations are real, persistent Items.
	// When the dwelling is "live" they sit on an assigned pool Map at their stored
	// coordinates; when idle they are parked on Map.Internal so the pool Map slot
	// can be recycled. This is what lets us back an unlimited number of owners with
	// a bounded pool of live maps.
	public class SkyInstance
	{
		private Serial m_OwnerSerial;
		private DateTime m_LastTouched;
		private int m_LiveMapIndex = -1;
		private bool m_Built;
		private bool m_Purchased;
		private readonly List<Serial> m_Friends = new List<Serial>();
		private readonly List<DwellingItem> m_Items = new List<DwellingItem>();

		public Serial OwnerSerial { get { return m_OwnerSerial; } set { m_OwnerSerial = value; } }
		public DateTime LastTouched { get { return m_LastTouched; } set { m_LastTouched = value; } }

		// A dwelling is only truly owned once purchased. Before that the record is a
		// transient visit: the player can look around and buy from the sign inside,
		// but an unpurchased dwelling is freed when it goes idle and is never saved.
		public bool Purchased { get { return m_Purchased; } set { m_Purchased = value; } }

		// Index into Map.Maps[] of the pool map this dwelling currently occupies, or -1 when parked.
		public int LiveMapIndex { get { return m_LiveMapIndex; } set { m_LiveMapIndex = value; } }
		public bool IsLive { get { return m_LiveMapIndex >= 0; } }

		// True once the starter decorations have been created at least once.
		public bool Built { get { return m_Built; } set { m_Built = value; } }

		public List<Serial> Friends { get { return m_Friends; } }
		public List<DwellingItem> Items { get { return m_Items; } }

		public SkyInstance()
		{
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

		public Map LiveMap
		{
			get { return ( m_LiveMapIndex >= 0 && m_LiveMapIndex < Map.Maps.Length ) ? Map.Maps[m_LiveMapIndex] : null; }
		}
	}
}
