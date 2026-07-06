using System;
using Server;
using Server.Network;

namespace Server.Engines.Instancing
{
	public class DungeonInstanceExitTeleporter : Item
	{
		[Constructable]
		public DungeonInstanceExitTeleporter() : this( 7107 )
		{
		}

		public DungeonInstanceExitTeleporter( int itemID ) : base( itemID > 0 ? itemID : 7107 )
		{
			Name = "a dungeon instance exit";
			Movable = false;
			Visible = true;
			Hue = 0x455;
			Weight = -2;
		}

		public override bool OnMoveOver( Mobile from )
		{
			if ( from == null || !from.Player )
				return true;

			return !DungeonInstanceType.Instance.LeaveInstance( from );
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

			DungeonInstanceType.Instance.LeaveInstance( from );
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			list.Add( 1070722, "Walk over or double-click to leave the instance" );
		}

		public DungeonInstanceExitTeleporter( Serial serial ) : base( serial )
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
