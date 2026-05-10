using Server.Mobiles;
using Server.Commands;

namespace Server.SpellBars
{
    class AncientBook
    {
		public static void Initialize()
		{
            CommandSystem.Register( "ancient", AccessLevel.Player, new CommandEventHandler( AncientBook_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "ancient" )]
		[Description( "Switches ancient magic between book or bag." )]
		public static void AncientBook_OnCommand( CommandEventArgs e )
		{
            Mobile m = e.Mobile;
			var player = m as PlayerMobile;
			if (player == null) return;

			if ( !player.Preferences.UsingAncientBook )
			{
				player.Preferences.UsingAncientBook = true;
				m.SendMessage(38, "You are now using the ancient spellbook.");
			}
			else
			{
				player.Preferences.UsingAncientBook = false;
				m.SendMessage(68, "You are now using the research bag.");
			}
        }
    }

    class ArchClose1
    {
		public static void Initialize()
		{
            CommandSystem.Register( "archclose1", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "archclose1" )]
		[Description( "Close Spell Bar Windows For Archmages - 1." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsArch1 ) );
			from.CloseGump( typeof( SpellBarsArch1 ) );
        }
    }

    class ArchClose2
    {
		public static void Initialize()
		{
            CommandSystem.Register( "archclose2", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "archclose2" )]
		[Description( "Close Spell Bar Windows For Archmages - 2." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsArch2 ) );
			from.CloseGump( typeof( SpellBarsArch2 ) );
        }
    }

    class ArchClose3
    {
		public static void Initialize()
		{
            CommandSystem.Register( "archclose3", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "archclose3" )]
		[Description( "Close Spell Bar Windows For Archmages - 3." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsArch3 ) );
			from.CloseGump( typeof( SpellBarsArch3 ) );
        }
    }

    class ArchClose4
    {
		public static void Initialize()
		{
            CommandSystem.Register( "archclose4", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "archclose4" )]
		[Description( "Close Spell Bar Windows For Archmages - 4." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsArch4 ) );
			from.CloseGump( typeof( SpellBarsArch4 ) );
        }
    }

    class ElementClose1
    {
		public static void Initialize()
		{
            CommandSystem.Register( "elementclose1", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "elementclose1" )]
		[Description( "Close Spell Bar Windows For Elementalists - 1." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsElement1 ) );
			from.CloseGump( typeof( SpellBarsElement1 ) );
        }
    }

    class ElementClose2
    {
		public static void Initialize()
		{
            CommandSystem.Register( "elementclose2", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "elementclose2" )]
		[Description( "Close Spell Bar Windows For Elementalists - 2." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsElement2 ) );
			from.CloseGump( typeof( SpellBarsElement2 ) );
        }
    }

    class MageClose1
    {
		public static void Initialize()
		{
            CommandSystem.Register( "mageclose1", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "mageclose1" )]
		[Description( "Close Spell Bar Windows For Mages - 1." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsMage1 ) );
			from.CloseGump( typeof( SpellBarsMage1 ) );
        }
    }

    class MageClose2
    {
		public static void Initialize()
		{
            CommandSystem.Register( "mageclose2", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "mageclose2" )]
		[Description( "Close Spell Bar Windows For Mages - 2." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsMage2 ) );
			from.CloseGump( typeof( SpellBarsMage2 ) );
        }
    }

    class MageClose3
    {
		public static void Initialize()
		{
            CommandSystem.Register( "mageclose3", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "mageclose3" )]
		[Description( "Close Spell Bar Windows For Mages - 3." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsMage3 ) );
			from.CloseGump( typeof( SpellBarsMage3 ) );
        }
    }

    class MageClose4
    {
		public static void Initialize()
		{
            CommandSystem.Register( "mageclose4", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "mageclose4" )]
		[Description( "Close Spell Bar Windows For Mages - 4." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsMage4 ) );
			from.CloseGump( typeof( SpellBarsMage4 ) );
        }
    }

    class NecroClose1
    {
		public static void Initialize()
		{
            CommandSystem.Register( "necroclose1", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "necroclose1" )]
		[Description( "Close Spell Bar Windows For Necromancers - 1." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsNecro1 ) );
			from.CloseGump( typeof( SpellBarsNecro1 ) );
        }
    }

    class NecroClose2
    {
		public static void Initialize()
		{
            CommandSystem.Register( "necroclose2", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "necroclose2" )]
		[Description( "Close Spell Bar Windows For Necromancers - 2." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsNecro2 ) );
			from.CloseGump( typeof( SpellBarsNecro2 ) );
        }
    }

	class DeathClose1
    {
		public static void Initialize()
		{
            CommandSystem.Register( "deathclose1", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "deathclose1" )]
		[Description( "Close Spell Bar Windows For Death Knights - 1." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsDeath1 ) );
			from.CloseGump( typeof( SpellBarsDeath1 ) );
        }
    }

    class DeathClose2
    {
		public static void Initialize()
		{
            CommandSystem.Register( "deathclose2", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "deathclose2" )]
		[Description( "Close Spell Bar Windows For Death Knights - 2." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsDeath2 ) );
			from.CloseGump( typeof( SpellBarsDeath2 ) );
        }
    }

	class PriestClose1
    {
		public static void Initialize()
		{
            CommandSystem.Register( "holyclose1", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "holyclose1" )]
		[Description( "Close Spell Bar Windows For Prayers - 1." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsPriest1 ) );
			from.CloseGump( typeof( SpellBarsPriest1 ) );
        }
    }

    class PriestClose2
    {
		public static void Initialize()
		{
            CommandSystem.Register( "holyclose2", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "holyclose2" )]
		[Description( "Close Spell Bar Windows For Prayers - 2." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsPriest2 ) );
			from.CloseGump( typeof( SpellBarsPriest2 ) );
        }
    }

	class KnightClose1
    {
		public static void Initialize()
		{
            CommandSystem.Register( "knightclose1", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "knightclose1" )]
		[Description( "Close Spell Bar Windows For Knights - 1." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsKnight1 ) );
			from.CloseGump( typeof( SpellBarsKnight1 ) );
        }
    }

    class KnightClose2
    {
		public static void Initialize()
		{
            CommandSystem.Register( "knightclose2", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "knightclose2" )]
		[Description( "Close Spell Bar Windows For Knights - 2." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsKnight2 ) );
			from.CloseGump( typeof( SpellBarsKnight2 ) );
        }
    }

    class BardClose1
    {
		public static void Initialize()
		{
            CommandSystem.Register( "bardclose1", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "bardclose1" )]
		[Description( "Close Spell Bar Windows For Bards - 1." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsBard1 ) );
			from.CloseGump( typeof( SpellBarsBard1 ) );
        }
    }

    class BardClose2
    {
		public static void Initialize()
		{
            CommandSystem.Register( "bardclose2", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "bardclose2" )]
		[Description( "Close Spell Bar Windows For Bards - 2." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsBard2 ) );
			from.CloseGump( typeof( SpellBarsBard2 ) );
        }
    }

    class MonkClose1
    {
		public static void Initialize()
		{
            CommandSystem.Register( "monkclose1", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "monkclose1" )]
		[Description( "Close Spell Bar Windows For Monks - 1." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsMonk1 ) );
			from.CloseGump( typeof( SpellBarsMonk1 ) );
        }
    }

    class MonkClose2
    {
		public static void Initialize()
		{
            CommandSystem.Register( "monkclose2", AccessLevel.Player, new CommandEventHandler( CloseBar_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "monkclose2" )]
		[Description( "Close Spell Bar Windows For Monks - 2." )]
		public static void CloseBar_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( SetupBarsMonk2 ) );
			from.CloseGump( typeof( SpellBarsMonk2 ) );
        }
    }
}