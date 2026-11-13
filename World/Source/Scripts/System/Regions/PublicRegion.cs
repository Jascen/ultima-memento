using System;
using System.Xml;
using Server;
using Server.Mobiles;
using Server.Gumps;
using Server.Spells;
using System.Text;
using System.IO;
using Server.Network;
using Server.Misc;

namespace Server.Regions
{
	public class PublicRegion : BaseRegion
	{
		// Runtime copies (dungeon instances).
		public PublicRegion( string name, Map map, int priority, params Rectangle3D[] area ) : base( name, map, priority, area )
		{
		}

		public PublicRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
		}

		public override bool AllowHousing( Mobile from, Point3D p )
		{
			return false;
		}

		public override TimeSpan GetLogoutDelay( Mobile m )
		{
			return TimeSpan.Zero;
		}

		public override void AlterLightLevel( Mobile m, ref int global, ref int personal )
		{
			if ( this.Name != "the Lyceum" && this.Name != "the Druid's Glade" && this.Name != "the Port" ){ global = LightCycle.NightLevel; }
		}

		public override bool AllowHarmful( Mobile from, Mobile target )
		{
			if ( from is Warriors && target is Warriors )
				return true;

			return false;
		}

		public override bool OnBeginSpellCast( Mobile m, ISpell s )
		{
			if ( Worlds.IsAllowedSpell( m, s ) && this.Name != "the Chasm" )
			{
				return base.OnBeginSpellCast( m, s );
			}
			else
			{
				m.SendMessage( "That does not seem to work here." );
				return false;
			}
		}

		public override void OnEnter( Mobile m )
		{
			base.OnEnter( m );
			if ( m is PlayerMobile )
			{
				if ( this.Name == "the Druid's Glade" ){ LoggingFunctions.LogRegions( m, this.Name, "enter" ); }
				else if ( this.Name == "Xardok's Castle" ){ LoggingFunctions.LogRegions( m, this.Name, "enter" ); }
				else if ( this.Name == "the Basement" ){ LoggingFunctions.LogRegions( m, this.Name, "enter" ); }
				else if ( this.Name == "the Chamber of Tyball" ){ LoggingFunctions.LogRegions( m, this.Name, "enter" ); }
				else if ( this.Name == "the Tower of Stoneguard" ){ LoggingFunctions.LogRegions( m, this.Name, "enter" ); }
				else if ( this.Name == "the Ethereal Void" ){ LoggingFunctions.LogRegions( m, this.Name, "enter" ); }
				else if ( this.Name == "the Tower of Mondain" ){ LoggingFunctions.LogRegions( m, this.Name, "enter" ); }
				else if ( this.Name == "the Crypt of Morphius" ){ LoggingFunctions.LogRegions( m, this.Name, "enter" ); }
				else if ( this.Name == "the Castle of Shadowguard" ){ LoggingFunctions.LogRegions( m, this.Name, "enter" ); }
				else if ( this.Name == "the Guardian's Chamber" ){ LoggingFunctions.LogRegions( m, this.Name, "enter" ); }
				else if ( this.Name == "the Tomb of Lethe" ){ LoggingFunctions.LogRegions( m, this.Name, "enter" ); }
				else if ( this.Name == "Seggallions's Cave" ){ LoggingFunctions.LogRegions( m, this.Name, "enter" ); }
				else if ( this.Name == "Garamon's Castle" ){ LoggingFunctions.LogRegions( m, this.Name, "enter" ); }
				else if ( this.Name == "the Lyceum" ){ LoggingFunctions.LogRegions( m, this.Name, "enter" ); }
			}

			Server.Misc.RegionMusic.MusicRegion( m, this );
		}

		public override void OnExit( Mobile m )
		{
			base.OnExit( m );
			if ( m is PlayerMobile )
			{
				if ( 	this.Name == "Xardok's Castle" || 
						this.Name == "the Chamber of Tyball" || 
						this.Name == "the Tower of Stoneguard" || 
						this.Name == "the Basement" || 
						this.Name == "the Ethereal Void" || 
						this.Name == "the Tower of Mondain" || 
						this.Name == "the Crypt of Morphius" || 
						this.Name == "the Castle of Shadowguard" || 
						this.Name == "the Guardian's Chamber" || 
						this.Name == "the Tomb of Lethe" || 
						this.Name == "Seggallions's Cave" || 
						this.Name == "Garamon's Castle" || 
						this.Name == "the Lyceum" || 
						this.Name == "the Druid's Glade" )
				{
					LoggingFunctions.LogRegions( m, this.Name, "exit" );
				}
			}
		}
	}
}