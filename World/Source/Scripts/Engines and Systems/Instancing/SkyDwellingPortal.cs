using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Instancing
{
	public class SkyDwellingPortal : Item
	{
		[Constructable]
		public SkyDwellingPortal() : base( 0x1F14 ) // a recall rune graphic — clearly magical, not vendor-y
		{
			Name = "sky dwelling portal";
			Movable = false;
			Hue = 0x47E; // pale blue, distinguishes from a normal rune
			Light = LightType.Circle300;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from == null )
				return;

			if ( !from.InRange( GetWorldLocation(), 3 ) )
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
				return;
			}

			SkyInstanceManager.SendOwnerToTheirInstance( from );
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			list.Add( 1070722, "Double-click to enter your private sky dwelling" );
		}

		public SkyDwellingPortal( Serial serial ) : base( serial )
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
