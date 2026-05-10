using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Items;

namespace Server.Spells.Dynamic
{
	// [FillSpellbooks         -> bag of every legacy spellbook, fully filled
	// [FillDynamicSpellbooks  -> bag of every dynamic spellbook, fully filled
	public static class FillSpellbooksCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register( "FillSpellbooks", AccessLevel.GameMaster, new CommandEventHandler( OnLegacyCommand ) );
			CommandSystem.Register( "FillDynamicSpellbooks", AccessLevel.GameMaster, new CommandEventHandler( OnDynamicCommand ) );
		}

		[Usage( "FillSpellbooks" )]
		[Description( "Creates a bag in your backpack containing one of each legacy spellbook, all spells learned." )]
		private static void OnLegacyCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if ( from == null )
				return;

			Spellbook[] books = new Spellbook[]
			{
				new Spellbook(),                 // Magery (64)
				new NecromancerSpellbook(),      // 17
				new BookOfChivalry(),            // 10
				new BookOfBushido(),             // 6
				new BookOfNinjitsu(),            // 8
				new ElementalSpellbook(),        // 32
				new KnightshipSpellbook(),       // 10
				new JediSpellbook(),             // 11
				new SythSpellbook(),             // 11
				new MysticSpellbook(),           // 10
				new HolyManSpellbook(),          // 15
				new DeathKnightSpellbook(),      // 15
				new AncientSpellbook(),          // Research (64)
				new SongBook()                   // Bard (16)
			};

			GiveFilledBag( from, "filled spellbooks", books );
		}

		[Usage( "FillDynamicSpellbooks" )]
		[Description( "Creates a bag in your backpack containing one of each dynamic spellbook, all spells learned." )]
		private static void OnDynamicCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if ( from == null )
				return;

			Spellbook[] books = new Spellbook[]
			{
				new DynamicElementalSpellbook(),
				new DynamicSongSpellbook(),
				new DynamicDeathKnightSpellbook(),
				new DynamicDruidismSpellbook(),
				new DynamicHolyManSpellbook(),
				new DynamicJediSpellbook(),
				new DynamicJesterSpellbook(),
				new DynamicResearchSpellbook(),
				new DynamicShinobiSpellbook(),
				new DynamicSythSpellbook(),
				new DynamicWitchSpellbook(),
				// Knightship is dynamic-driven via KnightshipSpellbookProvider; the legacy
				// KnightshipSpellbook class doubles as its dynamic implementation.
				new KnightshipSpellbook()
			};

			GiveFilledBag( from, "filled dynamic spellbooks", books );
		}

		private static void GiveFilledBag( Mobile from, string bagName, IList<Spellbook> books )
		{
			Bag bag = new Bag();
			bag.Hue = 1153;
			bag.Name = bagName;

			int filled = 0;
			List<string> failures = new List<string>();

			foreach ( Spellbook book in books )
			{
				if ( book == null )
					continue;

				try
				{
					book.Content = AllSpellsBitmask( book.BookCount );
					book.InvalidateProperties();
					bag.DropItem( book );
					filled++;
				}
				catch ( Exception ex )
				{
					failures.Add( string.Format( "{0}: {1}", book.GetType().Name, ex.Message ) );
					book.Delete();
				}
			}

			if ( !from.AddToBackpack( bag ) )
			{
				bag.MoveToWorld( from.Location, from.Map );
				from.SendMessage( "Backpack was full; bag dropped at your feet." );
			}

			from.SendMessage( "Created '{0}' with {1} spellbook(s).", bagName, filled );

			foreach ( string fail in failures )
				from.SendMessage( 0x21, "Skipped {0}", fail );
		}

		// Bitmask with the lowest <bookCount> bits set. Guards against `1 << 64` undefined behavior
		// for max-size books (e.g. Magery, AncientSpellbook).
		private static ulong AllSpellsBitmask( int bookCount )
		{
			if ( bookCount >= 64 )
				return ulong.MaxValue;
			if ( bookCount <= 0 )
				return 0UL;
			return ( 1UL << bookCount ) - 1UL;
		}
	}
}
