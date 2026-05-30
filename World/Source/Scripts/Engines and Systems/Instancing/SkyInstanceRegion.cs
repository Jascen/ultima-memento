using System;
using Server;
using Server.Mobiles;
using Server.Regions;

namespace Server.Engines.Instancing
{
	// Covers an entire sky-dwelling pool map. It is bound to the map, not to an
	// owner: the owner of a given pool map changes as dwellings are parked and
	// recycled, so on enter we just nudge whichever dwelling currently lives here.
	public class SkyInstanceRegion : BaseRegion
	{
		private readonly int m_MapIndex;

		public int MapIndex { get { return m_MapIndex; } }

		public SkyInstanceRegion( Map map )
			: base( "Sky Dwelling", map, 50, Footprint() )
		{
			m_MapIndex = map.MapIndex;
		}

		// A bounded box around the dwelling footprint rather than the whole map:
		// region registration force-creates every Sector it covers, so a full-map
		// region would allocate hundreds of thousands of sectors per pool map. The
		// dwelling and its visitors only ever occupy this small area.
		private const int HalfExtent = 64;

		private static Rectangle3D Footprint()
		{
			return new Rectangle3D(
				new Point3D( SkyInstanceManager.DwellingX - HalfExtent, SkyInstanceManager.DwellingY - HalfExtent, sbyte.MinValue ),
				new Point3D( SkyInstanceManager.DwellingX + HalfExtent, SkyInstanceManager.DwellingY + HalfExtent, sbyte.MaxValue ) );
		}

		public override bool AllowHousing( Mobile from, Point3D p )
		{
			return false; // private instances are not for player housing
		}

		public override void OnEnter( Mobile m )
		{
			base.OnEnter( m );

			if ( m is PlayerMobile )
				SkyInstanceManager.OnPlayerEnteredMap( m_MapIndex, (PlayerMobile)m );
		}
	}
}
