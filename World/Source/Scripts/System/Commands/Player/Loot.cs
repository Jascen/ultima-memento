using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Misc;
using Server.Commands;
using Server.Commands.Generic;

namespace Server.Misc
{
    class LootChoiceUpdates
    {
		public static void UpdateLootChoice( Mobile m, int nChange )
		{
			LootChoiceUpdates.InitializeLootChoice( m );

			string LootChoiceSetting = ((PlayerMobile)m).Preferences.CharacterLoot;

			string[] eachSetting = LootChoiceSetting.Split('#');
			int nLine = 1;
			string newSettings = "";

			foreach (string eachSettings in eachSetting)
			{
				if ( nLine == nChange )
				{
					string sChange = "0";
					if ( eachSettings == "0" ){ sChange = "1"; }
					newSettings = newSettings + sChange + "#";
				}
				else if ( nLine > 17 )
				{
				}
				else
				{
					newSettings = newSettings + eachSettings + "#";
				}
				nLine++;
			}

			((PlayerMobile)m).Preferences.CharacterLoot = newSettings; 
		}

		public static void InitializeLootChoice( Mobile m )
		{
			if ( ((PlayerMobile)m).Preferences.CharacterLoot == "" || ((PlayerMobile)m).Preferences.CharacterLoot == null ){((PlayerMobile)m).Preferences.CharacterLoot = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
		}
	}
}

namespace Server.Gumps 
{
    public class LootChoices : Gump
    {
		public int m_Origin;

		public static void Initialize()
		{
            CommandSystem.Register( "loot", AccessLevel.Player, new CommandEventHandler( LootChoice_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "loot" )]
		[Description( "Allows you to setup automatic looting." )]
		public static void LootChoice_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( LootChoices ) );
			from.SendGump( new LootChoices( from, 0 ) );
        }

        public LootChoices ( Mobile from, int origin ) : base ( 50, 50 )
        {
			m_Origin = origin;
			string color = "#efd290";

			LootChoiceUpdates.InitializeLootChoice( from );
			string MyServerSettings = ((PlayerMobile)from).Preferences.CharacterLoot;

			this.Closable=true;
			this.Disposable=false;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			AddImage(0, 0, 9580, Server.Misc.PlayerSettings.GetGumpHue( from ));
			AddHtml( 12, 12, 336, 20, @"<BODY><BASEFONT Color=" + color + ">LOOT</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(384, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);

			AddHtml( 52, 225, 139, 20, @"<BODY><BASEFONT Color=" + color + ">Set Loot Container</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(14, 225, 4005, 4005, 100, GumpButtonType.Reply, 0);

			int g = 266;
			string[] eachLoot = MyServerSettings.Split('#');
			int b1=0; int b2=0; int b3=0; int b4=0; int b5=0; int b6=0; int b7=0; int b8=0;
			int b9=0; int b10=0; int b11=0; int b12=0; int b13=0; int b14=0; int b15=0; int b16=0;
			int nLine = 1;

			foreach (string eachLoots in eachLoot)
			{
				if (nLine == 1 && eachLoots == "0"){ b1 = 3609; } else if (nLine == 1){ b1 = 4018; }
				if (nLine == 2 && eachLoots == "0"){ b2 = 3609; } else if (nLine == 2){ b2 = 4018; }
				if (nLine == 3 && eachLoots == "0"){ b3 = 3609; } else if (nLine == 3){ b3 = 4018; }
				if (nLine == 5 && eachLoots == "0"){ b4 = 3609; } else if (nLine == 5){ b4 = 4018; }
				if (nLine == 4 && eachLoots == "0"){ b5 = 3609; } else if (nLine == 4){ b5 = 4018; }
				if (nLine == 6 && eachLoots == "0"){ b6 = 3609; } else if (nLine == 6){ b6 = 4018; }
				if (nLine == 7 && eachLoots == "0"){ b7 = 3609; } else if (nLine == 7){ b7 = 4018; }
				if (nLine == 14 && eachLoots == "0"){ b8 = 3609; } else if (nLine == 14){ b8 = 4018; }
				if (nLine == 13 && eachLoots == "0"){ b9 = 3609; } else if (nLine == 13){ b9 = 4018; }
				if (nLine == 15 && eachLoots == "0"){ b10 = 3609; } else if (nLine == 15){ b10 = 4018; }
				if (nLine == 16 && eachLoots == "0"){ b11 = 3609; } else if (nLine == 16){ b11 = 4018; }
				if (nLine == 8 && eachLoots == "0"){ b12 = 3609; } else if (nLine == 8){ b12 = 4018; }
				if (nLine == 9 && eachLoots == "0"){ b13 = 3609; } else if (nLine == 9){ b13 = 4018; }
				if (nLine == 10 && eachLoots == "0"){ b14 = 3609; } else if (nLine == 10){ b14 = 4018; }
				if (nLine == 11 && eachLoots == "0"){ b15 = 3609; } else if (nLine == 11){ b15 = 4018; }
				if (nLine == 12 && eachLoots == "0"){ b16 = 3609; } else if (nLine == 12){ b16 = 4018; }

				nLine++;
			}

			AddHtml( 52, g, 139, 20, @"<BODY><BASEFONT Color=" + color + ">Coins & Nuggets</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(14, g, b1, b1, 99, GumpButtonType.Reply, 0); g=g+26;
			AddHtml( 52, g, 139, 20, @"<BODY><BASEFONT Color=" + color + ">Gems</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(14, g, b2, b2, 1, GumpButtonType.Reply, 0); g=g+26;
			AddHtml( 52, g, 139, 20, @"<BODY><BASEFONT Color=" + color + ">Arrows & Bolts</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(14, g, b3, b3, 2, GumpButtonType.Reply, 0); g=g+26;
			AddHtml( 52, g, 139, 20, @"<BODY><BASEFONT Color=" + color + ">Bandages</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(14, g, b4, b4, 4, GumpButtonType.Reply, 0); g=g+26;
			AddHtml( 52, g, 139, 20, @"<BODY><BASEFONT Color=" + color + ">Elemental Scrolls</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(14, g, b5, b5, 3, GumpButtonType.Reply, 0); g=g+26;
			AddHtml( 52, g, 139, 20, @"<BODY><BASEFONT Color=" + color + ">Magery Scrolls</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(14, g, b6, b6, 5, GumpButtonType.Reply, 0); g=g+26;
			AddHtml( 52, g, 139, 20, @"<BODY><BASEFONT Color=" + color + ">Necromancer Scrolls</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(14, g, b7, b7, 6, GumpButtonType.Reply, 0); g=g+26;
			AddHtml( 52, g, 139, 20, @"<BODY><BASEFONT Color=" + color + ">Unknown Scrolls</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(14, g, b8, b8, 13, GumpButtonType.Reply, 0); g=266;

			AddHtml( 236, g, 139, 20, @"<BODY><BASEFONT Color=" + color + ">Bard Songs</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(381, g, b9, b9, 12, GumpButtonType.Reply, 0); g=g+26;
			AddHtml( 236, g, 139, 20, @"<BODY><BASEFONT Color=" + color + ">Alchemic Reagents</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(381, g, b10, b10, 14, GumpButtonType.Reply, 0); g=g+26;
			AddHtml( 236, g, 139, 20, @"<BODY><BASEFONT Color=" + color + ">Herbalist Reagents</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(381, g, b11, b11, 15, GumpButtonType.Reply, 0); g=g+26;
			AddHtml( 236, g, 139, 20, @"<BODY><BASEFONT Color=" + color + ">Magery Reagents</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(381, g, b12, b12, 7, GumpButtonType.Reply, 0); g=g+26;
			AddHtml( 236, g, 139, 20, @"<BODY><BASEFONT Color=" + color + ">Necromancy Reagents</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(381, g, b13, b13, 8, GumpButtonType.Reply, 0); g=g+26;
			AddHtml( 236, g, 139, 20, @"<BODY><BASEFONT Color=" + color + ">Unknown Reagents</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(381, g, b14, b14, 9, GumpButtonType.Reply, 0); g=g+26;
			AddHtml( 236, g, 139, 20, @"<BODY><BASEFONT Color=" + color + ">Potions</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(381, g, b15, b15, 10, GumpButtonType.Reply, 0); g=g+26;
			AddHtml( 236, g, 139, 20, @"<BODY><BASEFONT Color=" + color + ">Unknown Potions</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(381, g, b16, b16, 11, GumpButtonType.Reply, 0); g=g+26;

			AddHtml( 15, 44, 394, 210, @"<BODY><BASEFONT Color=" + color + ">Check the categories of items to automatically take from common dungeon chests or corpses and put them in your backpack. Magery and necromancer reagents are those used specifically by those characters, where witches brew reagents fall into the necromancer category. Alchemic reagents are unique to alchemy only. Herbalist reagents are plants that one may find, used in druidic herbalism. You can set a container the loot will go into, but this container must remain in your main pack and not within another container.</BASEFONT></BODY>", (bool)false, (bool)false);
        }
    
		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( info.ButtonID == 99 ){ LootChoiceUpdates.UpdateLootChoice( from, 1 ); }
			else if ( info.ButtonID == 1 ){ LootChoiceUpdates.UpdateLootChoice( from, 2 ); }
			else if ( info.ButtonID == 2 ){ LootChoiceUpdates.UpdateLootChoice( from, 3 ); }
			else if ( info.ButtonID == 3 ){ LootChoiceUpdates.UpdateLootChoice( from, 4 ); }
			else if ( info.ButtonID == 4 ){ LootChoiceUpdates.UpdateLootChoice( from, 5 ); }
			else if ( info.ButtonID == 5 ){ LootChoiceUpdates.UpdateLootChoice( from, 6 ); }
			else if ( info.ButtonID == 6 ){ LootChoiceUpdates.UpdateLootChoice( from, 7 ); }
			else if ( info.ButtonID == 7 ){ LootChoiceUpdates.UpdateLootChoice( from, 8 ); }
			else if ( info.ButtonID == 8 ){ LootChoiceUpdates.UpdateLootChoice( from, 9 ); }
			else if ( info.ButtonID == 9 ){ LootChoiceUpdates.UpdateLootChoice( from, 10 ); }
			else if ( info.ButtonID == 10 ){ LootChoiceUpdates.UpdateLootChoice( from, 11 ); }
			else if ( info.ButtonID == 11 ){ LootChoiceUpdates.UpdateLootChoice( from, 12 ); }
			else if ( info.ButtonID == 12 ){ LootChoiceUpdates.UpdateLootChoice( from, 13 ); }
			else if ( info.ButtonID == 13 ){ LootChoiceUpdates.UpdateLootChoice( from, 14 ); }
			else if ( info.ButtonID == 14 ){ LootChoiceUpdates.UpdateLootChoice( from, 15 ); }
			else if ( info.ButtonID == 15 ){ LootChoiceUpdates.UpdateLootChoice( from, 16 ); }
			else if ( info.ButtonID == 16 ){ LootChoiceUpdates.UpdateLootChoice( from, 17 ); }
			else if ( info.ButtonID == 100 ){ BaseContainer.ContainerSetTarget( from, 1 ); }

			if ( info.ButtonID < 1 && m_Origin > 0 ){ from.SendSound( 0x4A ); from.SendGump( new Server.Engines.Help.HelpGump( from, 12 ) ); }
			else if ( info.ButtonID < 1 ){ }
			else { from.SendGump( new LootChoices( from, m_Origin ) ); from.SendSound( 0x4A ); }
		}
    }
}

namespace Server.Commands
{
	public class Loot
	{
		public class AutoLootCommand : BaseCommand
		{
			public AutoLootCommand()
			{
				AccessLevel = AccessLevel.Player;
				Supports = CommandSupport.AllItems;
				Commands = new string[] { "AutoLoot" };
				ObjectTypes = ObjectTypes.Items;
				Usage = "AutoLoot";
				Description = "Execute automatic looting on-demand";
			}

			public static void Initialize()
			{
				TargetCommands.Register(new AutoLootCommand());
			}

			public override void Execute(CommandEventArgs e, object obj)
			{
				if (obj is Container)
				{
					var container = (Container)obj;
					var from = e.Mobile;

					if ( !from.InRange( container.GetWorldLocation(), 3 ) )
					{
						from.SendMessage( "You will have to get closer to the container!" );
						return;
					}

					if ( container is ILockable && ((ILockable)container).Locked
						|| container is LockableContainer && ((LockableContainer)container).CheckLocked( from ) ) 
					{
						from.SendMessage( "That container is locked." );
						return;
					}

					if (container.RootParentEntity != null && container.RootParentEntity is Item && ((Item)container.RootParentEntity).Movable)
					{
						from.SendMessage( "That item is not lootable." );
						return;
					}

					// Technically OK since you have access, but let's just be safe.
					if (container.IsLockedDown || container.IsSecure)
					{
						from.SendMessage( "That container is secure." );
						return;
					}

					Server.Misc.PlayerSettings.LootContainer( from, container );
				}
				else
				{
					e.Mobile.SendMessage("That is not a container.");
				}
			}
		}
	}
}