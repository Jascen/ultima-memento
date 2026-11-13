using System;
using System.Xml;
using Server;
using Server.Mobiles;
using Server.Gumps;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Fourth;
using Server.Spells.Sixth;
using Server.Spells.Chivalry;
using Server.Spells.Undead;
using Server.Spells.Herbalist;
using System.Text;
using System.IO;
using Server.Network;
using Server.Misc;
using System.Collections.Generic;
using System.Collections;
using Server.Items;

namespace Server.Regions
{
	public class DungeonRegion : BaseRegion
	{
		public override bool YoungProtected { get { return false; } }

		private Point3D m_EntranceLocation;
		private Map m_EntranceMap;

		public Point3D EntranceLocation{ get{ return m_EntranceLocation; } set{ m_EntranceLocation = value; } }
		public Map EntranceMap{ get{ return m_EntranceMap; } set{ m_EntranceMap = value; } }

		// Runtime copies (dungeon instances).
		public DungeonRegion( string name, Map map, int priority, params Rectangle3D[] area ) : base( name, map, priority, area )
		{
		}

		public DungeonRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
			XmlElement entrEl = xml["entrance"];

			Map entrMap = map;
			ReadMap( entrEl, "map", ref entrMap, false );

			if ( ReadPoint3D( entrEl, entrMap, ref m_EntranceLocation, false ) )
				m_EntranceMap = entrMap;
		}

		public override bool AllowHousing( Mobile from, Point3D p )
		{
			return false;
		}

		public override void AlterLightLevel( Mobile m, ref int global, ref int personal )
		{
			if ( this.Name == "the Valley of Dark Druids" ||
					this.Name == "The Castle of Vordo" ||
					this.Name == "Vordo's Castle Grounds" ||
					this.Name == "the Corrupt Pass" ||
					this.Name == "the Great Pyramid" ||
					this.Name == "the Altar of the Dragon King" ||
					this.Name == "the Hidden Valley" 
				){} else { global = LightCycle.DungeonLevel; }
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