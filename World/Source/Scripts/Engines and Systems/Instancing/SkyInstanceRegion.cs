using System;
using Server;
using Server.Mobiles;
using Server.Regions;

namespace Server.Engines.Instancing
{
	public class SkyInstanceRegion : BaseRegion
	{
		private readonly int m_InstanceId;

		public int InstanceId { get { return m_InstanceId; } }

		public SkyInstanceRegion( int instanceId, Map map, Rectangle3D area )
			: base( SkyInstanceManager.RegionNameFor( instanceId ), map, SkyInstanceManager.RegionPriority, area )
		{
			m_InstanceId = instanceId;
		}

		public override bool AllowHousing( Mobile from, Point3D p )
		{
			return false;
		}

		public override void OnEnter( Mobile m )
		{
			base.OnEnter( m );

			if ( m is PlayerMobile )
				SkyInstanceManager.OnPlayerEntered( m_InstanceId, (PlayerMobile)m );
		}

		public override void OnExit( Mobile m )
		{
			base.OnExit( m );

			if ( m is PlayerMobile )
				SkyInstanceManager.OnPlayerExited( m_InstanceId, (PlayerMobile)m );
		}
	}
}
