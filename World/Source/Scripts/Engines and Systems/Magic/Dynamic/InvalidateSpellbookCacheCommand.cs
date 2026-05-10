using System;
using Server;
using Server.Commands;
using Server.Items;

namespace Server.Spells.Dynamic
{
	public static class InvalidateSpellbookCacheCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register( "InvalidateSpellbookCache", AccessLevel.Administrator, new CommandEventHandler( OnCommand ) );
		}

		[Usage( "InvalidateSpellbookCache <spellbookType>" )]
		[Description( "Invalidates dynamic spellbook cache for all clients. Specify the spellbook type ID (e.g. 20 for Knightship)." )]
		private static void OnCommand( CommandEventArgs e )
		{
			if ( e.Length == 0 )
			{
				e.Mobile.SendMessage( "Usage: [InvalidateSpellbookCache <spellbookType>" );
				e.Mobile.SendMessage( "Specify a spellbook type ID to invalidate." );
			}
			else
			{
				int typeId = e.GetInt32( 0 );
				SpellbookType type = (SpellbookType)typeId;
				DynamicSpellbookManager.InvalidateCache( type, true );
				e.Mobile.SendMessage( "Invalidated spellbook cache for type {0}.", typeId );
			}
		}
	}
}
