using System;
using System.Xml;
using Server;
using Server.Mobiles;
using System.Text;
using System.IO;
using Server.Network;
using Server.Misc;

namespace Server.Regions
{
	public class CaveRegion : BaseRegion
	{
		// Runtime copies (dungeon instances).
		public CaveRegion( string name, Map map, int priority, params Rectangle3D[] area ) : base( name, map, priority, area )
		{
		}

		public CaveRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
		}

		public override bool AllowHousing( Mobile from, Point3D p )
		{
			return false;
		}

		public override void AlterLightLevel( Mobile m, ref int global, ref int personal )
		{
			global = LightCycle.CaveLevel;
		}

		public override void OnEnter( Mobile m )
		{
			base.OnEnter( m );
			if ( m is PlayerMobile )
			{
				LoggingFunctions.LogRegions( m, this.Name, "enter" );
			}

			Server.Misc.RegionMusic.MusicRegion( m, this );
		}

		public override void OnExit( Mobile m )
		{
			base.OnExit( m );
			if ( m is PlayerMobile )
			{
				LoggingFunctions.LogRegions( m, this.Name, "exit" );
			}
		}
	}
}