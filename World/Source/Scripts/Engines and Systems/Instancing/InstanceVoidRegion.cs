using System;
using Server;
using Server.Regions;

namespace Server.Engines.Instancing
{
	// Default region of a pool map: the unpopulated rest of the copied terrain.
	public class InstanceVoidRegion : BaseRegion
	{
		private readonly InstanceType m_Type;

		public InstanceVoidRegion( InstanceType type, Map map ) : base( null, map, 0, new Rectangle3D[0] )
		{
			m_Type = type;
		}

		public override bool YoungProtected { get { return false; } }

		public override bool AllowHousing( Mobile from, Point3D p )
		{
			return false;
		}

		public override bool OnMoveInto( Mobile m, Direction d, Point3D newLocation, Point3D oldLocation )
		{
			if ( m.AccessLevel >= AccessLevel.GameMaster )
				return true;

			m_Type.OnWalkIntoVoid( m, Map );
			return false;
		}

		public override void OnEnter( Mobile m )
		{
			base.OnEnter( m );

			if ( m.AccessLevel >= AccessLevel.GameMaster )
				return;

			// Deferred: this runs inside the move that put the mobile here.
			Map map = Map;
			Timer.DelayCall( TimeSpan.Zero, delegate { m_Type.ReturnFromVoid( m, map ); } );
		}
	}
}
