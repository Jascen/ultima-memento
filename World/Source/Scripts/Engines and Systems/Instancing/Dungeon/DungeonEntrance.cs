using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Instancing
{
	// Scaffold world object: double-click to drop into an instanced dungeon copy.
	// A player in an active party enters a shared copy with their party; a solo
	// player enters a private one (see DungeonInstanceType.OwnerKey).
	public class DungeonEntrance : Item
	{
		[Constructable]
		public DungeonEntrance() : base( 0x1AD7 ) // a dark hole / pit graphic
		{
			Name = "a dungeon entrance";
			Movable = false;
			Hue = 0x455;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from == null )
				return;

			if ( !from.InRange( GetWorldLocation(), 2 ) )
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
				return;
			}

			DungeonInstanceType.Instance.SendOwnerToTheirInstance( from );
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			list.Add( 1070722, "Double-click to enter the dungeon" );
		}

		public DungeonEntrance( Serial serial ) : base( serial )
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
