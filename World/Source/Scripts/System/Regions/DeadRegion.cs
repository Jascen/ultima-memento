using System;
using System.Xml;
using Server;
using Server.Mobiles;
using Server.Gumps;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Fourth;
using Server.Spells.Sixth;
using Server.Spells.Magical;
using Server.Spells.Bushido;
using Server.Spells.Ninjitsu;
using Server.Spells.Necromancy;
using Server.Spells.Chivalry;
using Server.Spells.Undead;
using Server.Spells.Herbalist;
using System.Text;
using System.IO;
using Server.Network;
using Server.Misc;
using Server.Items;

namespace Server.Regions
{
	public class DeadRegion : BaseRegion
	{
		// Runtime copies (dungeon instances).
		public DeadRegion( string name, Map map, int priority, params Rectangle3D[] area ) : base( name, map, priority, area )
		{
		}

		public DeadRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
		}

		public override bool AllowHousing( Mobile from, Point3D p )
		{
			return false;
		}

		public override void AlterLightLevel( Mobile m, ref int global, ref int personal )
		{
			global = LightCycle.DungeonLevel;
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