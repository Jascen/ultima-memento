using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Instancing
{
	// The "climb" teleporter at the foot of the sky-dwelling rope. Instead of
	// teleporting straight up to the (template) dwelling, it opens the chooser gump
	// so the player can pick whose dwelling to enter -- their own, one they were
	// invited to, or any public one.
	//
	// It still carries the original KeywordTeleporter PointDest/MapDest so staff can
	// climb to the SerpentIsland template for editing; only players get the gump.
	// Placed via Decoration/Monopoly/Sosaria/sky_home.cfg (replacing the plain
	// KeywordTeleporter there). To swap an already-placed one in a live world, use
	// [skyinstance installrope.
	public class SkyDwellingTeleporter : KeywordTeleporter
	{
		[Constructable]
		public SkyDwellingTeleporter()
		{
		}

		public override void StartTeleport( Mobile m )
		{
			if ( m == null )
				return;

			// Staff keep the raw climb so they can reach the template dwelling on
			// SerpentIsland for editing; players get the chooser instead.
			if ( m.AccessLevel > AccessLevel.Player )
			{
				base.StartTeleport( m );
				return;
			}

			m.CloseGump( typeof( SkyDwellingChooserGump ) );
			new SkyDwellingChooserGump( m );
		}

		public SkyDwellingTeleporter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
