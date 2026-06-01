using System;
using System.Collections.Generic;
using Server;

namespace Server.Engines.Instancing
{
	// One object belonging to an instance, paired with the instance-local
	// coordinate it should occupy when the instance is live. Because every pool
	// map of a given type shares identical terrain, this coordinate is
	// map-independent: the item lands in the same visible spot no matter which
	// pool map the instance is given.
	public class InstanceItem
	{
		public Item Item;
		public Point3D Loc;

		public InstanceItem( Item item, Point3D loc )
		{
			Item = item;
			Loc = loc;
		}
	}

	// A single live-or-parked instance, keyed to a player (its owner) or, for
	// shared instances, to a party leader. The contents are real, persistent
	// Items: while the instance is "live" they sit on an assigned pool Map at their
	// stored coordinates; when idle they are either parked on Map.Internal
	// (persistent types) or deleted (transient types). A bounded pool of live maps
	// backs an unlimited number of instances.
	//
	// This is just the data record; all behaviour lives on the owning InstanceType.
	public class Instance
	{
		private readonly InstanceType m_Type;
		private Serial m_OwnerSerial;
		private DateTime m_LastTouched;
		private int m_LiveMapIndex = -1;
		private bool m_Built;
		private bool m_Purchased;
		private bool m_Public;
		private readonly List<Serial> m_Members = new List<Serial>();
		private readonly List<InstanceItem> m_Items = new List<InstanceItem>();

		// The system this instance belongs to (sky dwelling, dungeon, ...). All
		// lifecycle decisions are delegated here.
		public InstanceType Type { get { return m_Type; } }

		public Serial OwnerSerial { get { return m_OwnerSerial; } set { m_OwnerSerial = value; } }
		public DateTime LastTouched { get { return m_LastTouched; } set { m_LastTouched = value; } }

		// Index into Map.Maps[] of the pool map this instance currently occupies, or -1 when parked.
		public int LiveMapIndex { get { return m_LiveMapIndex; } set { m_LiveMapIndex = value; } }
		public bool IsLive { get { return m_LiveMapIndex >= 0; } }

		// True once the starter contents have been created at least once. Persistent
		// instances keep this across a park/restore cycle; transient instances are
		// discarded whole, so a fresh entry always rebuilds.
		public bool Built { get { return m_Built; } set { m_Built = value; } }

		// Type-specific "claimed" flag. Persistent-owned types (sky dwellings) use
		// it to distinguish a permanent owner from a transient look-around visit;
		// transient types ignore it.
		public bool Purchased { get { return m_Purchased; } set { m_Purchased = value; } }

		// When true the instance is listed for anyone to visit (e.g. via the sky
		// dwelling chooser). Owner-controlled; meaningless for transient types.
		public bool Public { get { return m_Public; } set { m_Public = value; } }

		// Players besides the owner who may enter: dwelling friends, or the snapshot
		// of a party for a shared instance.
		public List<Serial> Members { get { return m_Members; } }
		public List<InstanceItem> Items { get { return m_Items; } }

		public Instance( InstanceType type )
		{
			m_Type = type;
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
