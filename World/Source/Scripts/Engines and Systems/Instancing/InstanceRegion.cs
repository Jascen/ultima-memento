using System;
using Server;
using Server.Mobiles;
using Server.Regions;

namespace Server.Engines.Instancing
{
	// Covers a single pool map's instance footprint. It is bound to the map, not to
	// an owner: the instance occupying a given pool map changes as instances are
	// parked and recycled, so on enter we just nudge whichever instance currently
	// lives here (keeping it from being swept out from under its visitors).
	public class InstanceRegion : BaseRegion
	{
		private readonly InstanceType m_Type;
		private readonly int m_MapIndex;

		public InstanceType Type { get { return m_Type; } }
		public int MapIndex { get { return m_MapIndex; } }

		public InstanceRegion( InstanceType type, Map map )
			: base( type.RegionName, map, 50, type.RegionFootprint() )
		{
			m_Type = type;
			m_MapIndex = map.MapIndex;
		}

		public override bool AllowHousing( Mobile from, Point3D p )
		{
			return false; // private instances are not for player housing
		}

		public override void OnEnter( Mobile m )
		{
			base.OnEnter( m );

			if ( m is PlayerMobile )
				m_Type.OnPlayerEnteredMap( m_MapIndex, (PlayerMobile)m );
		}
	}
}
