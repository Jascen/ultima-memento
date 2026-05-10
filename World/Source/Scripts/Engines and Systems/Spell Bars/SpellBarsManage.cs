using System;
using Server.Network;
using Server.Mobiles;
using Server.Misc;
using Server.Commands;
using Server.Spells.Elementalism;
using Server.Gumps;

namespace Server.SpellBars
{
	public abstract class SetupSpellBarGump : Gump
	{
		public readonly PlayerMobile Player;

		protected readonly int Origin;
		protected readonly bool IsNumberOneBar;
		protected readonly string Title;
		protected readonly string StorageKey;
		protected readonly string OpenCommand;
		protected readonly string CloseCommand;

		public abstract bool ConfigureGump();

		protected SetupSpellBarGump(PlayerMobile from, int origin, bool isNumberOneBar, string title, string storageKey, string openCommand, string closeCommand) : base(12, 50)
		{
			Player = from;
			Origin = origin;
			IsNumberOneBar = isNumberOneBar;
			Title = title;
			StorageKey = storageKey;
			OpenCommand = openCommand;
			CloseCommand = closeCommand;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);
		}

		protected void AddGlobalConfig(bool useVerticalBar, bool showSpellNamesWhenVertical)
		{
			AddImage(38, 0, 9578, Server.Misc.PlayerSettings.GetGumpHue(Player));
			AddButton(897, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);
			AddLabel(52, 14, LabelColors.OFFWHITE, Title);

			int useHorizontalBarGraphic = !useVerticalBar ? 4018 : 3609;
			int useVerticalBarGraphic = useVerticalBar ? 4018 : 3609;
			int showSpellNamesWhenVerticalGraphic = showSpellNamesWhenVertical ? 4018 : 3609;

			AddButton(75, 52, useHorizontalBarGraphic, useHorizontalBarGraphic, 91, GumpButtonType.Reply, 0);
			AddLabel(115, 55, LabelColors.OFFWHITE, "Horizontal Bar");

			AddButton(75, 82, useVerticalBarGraphic, useVerticalBarGraphic, 91, GumpButtonType.Reply, 0);
			AddLabel(115, 85, LabelColors.OFFWHITE, "Vertical Bar");

			AddButton(225, 82, showSpellNamesWhenVerticalGraphic, showSpellNamesWhenVerticalGraphic, 90, GumpButtonType.Reply, 0);
			AddLabel(265, 85, LabelColors.OFFWHITE, "Show Spell Names When Vertical");

			AddButton(500, 52, 4005, 4007, 92, GumpButtonType.Reply, 0);
			AddLabel(540, 55, LabelColors.OFFWHITE, "Open Toolbar");

			AddButton(500, 82, 4020, 4020, 93, GumpButtonType.Reply, 0);
			AddLabel(540, 85, LabelColors.OFFWHITE, "Close Toolbar");
		}

		protected void AddSpell(bool isChecked, int buttonId, int spellGraphicId, string spellName, ref int i, ref int x, ref int y)
		{
			int checkboxGraphic = isChecked ? 4018 : 3609;
			AddButton(x, y + 10, checkboxGraphic, checkboxGraphic, buttonId, GumpButtonType.Reply, 0);
			AddButton(x + 40, y, spellGraphicId, spellGraphicId, buttonId, GumpButtonType.Reply, 0);
			AddLabel(x + 100, y + 12, LabelColors.OFFWHITE, spellName);

			// Next column
			if (++i % 8 == 0)
			{
				x += 210;
				y = 135;
			}
			else
			{
				// Next row
				y += 50;
			}
		}

		protected void AddNextPageButton(int buttonId)
		{
			AddButton(897, 569, 4005, 4007, buttonId, GumpButtonType.Reply, 0); // Next Page
		}

		protected void AddPreviousPageButton(int buttonId)
		{
			AddButton(50, 569, 4014, 4015, buttonId, GumpButtonType.Reply, 0); // Previous Page
		}

		protected void InvokeCommand(string c, Mobile from)
		{
			CommandSystem.Handle(from, String.Format("{0}{1}", CommandSystem.Prefix, c));
		}
	}
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Server.SpellBars
{
	public class SetupBarsArch1 : SetupBarsArch
	{
		public SetupBarsArch1(PlayerMobile from, int origin, int pageNumber = 1)
			: base(from, origin, pageNumber, true, "SPELL BAR - ANCIENT - I", "SetupBarsArch1", "archtool1", "archclose1")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("archspell1", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("archspell1")]
		[Description("Opens Spell Bar Editor For Archmages - 1.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsArch1(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from, int pageNumber)
		{
			var gump = new SetupBarsArch1(from, Origin, pageNumber);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public class SetupBarsArch2 : SetupBarsArch
	{
		public SetupBarsArch2(PlayerMobile from, int origin, int pageNumber = 1)
			: base(from, origin, pageNumber, true, "SPELL BAR - ANCIENT - II", "SetupBarsArch2", "archtool2", "archclose2")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("archspell2", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("archspell2")]
		[Description("Opens Spell Bar Editor For Archmages - 2.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsArch2(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from, int pageNumber)
		{
			var gump = new SetupBarsArch2(from, Origin, pageNumber);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public class SetupBarsArch3 : SetupBarsArch
	{
		public SetupBarsArch3(PlayerMobile from, int origin, int pageNumber = 1)
			: base(from, origin, pageNumber, true, "SPELL BAR - ANCIENT - III", "SetupBarsArch3", "archtool3", "archclose3")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("archspell3", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("archspell3")]
		[Description("Opens Spell Bar Editor For Archmages - 3.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsArch3(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from, int pageNumber)
		{
			var gump = new SetupBarsArch3(from, Origin, pageNumber);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public class SetupBarsArch4 : SetupBarsArch
	{
		public SetupBarsArch4(PlayerMobile from, int origin, int pageNumber = 1)
			: base(from, origin, pageNumber, true, "SPELL BAR - ANCIENT - IV", "SetupBarsArch4", "archtool4", "archclose4")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("archspell4", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("archspell4")]
		[Description("Opens Spell Bar Editor For Archmages - 4.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsArch4(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from, int pageNumber)
		{
			var gump = new SetupBarsArch4(from, Origin, pageNumber);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public abstract class SetupBarsArch : SetupSpellBarGump
	{
		protected readonly int PageNumber;

		protected SetupBarsArch(PlayerMobile from, int origin, int pageNumber, bool isNumberOneBar, string title, string storageKey, string openCommand, string closeCommand)
			: base(from, origin, isNumberOneBar, title, storageKey, openCommand, closeCommand)
		{
			PageNumber = pageNumber;
		}

		protected abstract void ReopenGump(PlayerMobile from, int pageNumber);

		public static int GetSpellIcon(int i)
		{
			int spellId = 1 + i;
			return Int32.Parse(Server.Misc.Research.SpellInformation(spellId, 11));
		}

		public static string GetSpellName(int i)
		{
			int spellId = 1 + i;
			return Server.Misc.Research.SpellInformation(spellId, 2);
		}

		public override bool ConfigureGump()
		{
			string settings = ToolBarUpdates.GetToolBarSettings(Player, StorageKey);
			string[] eachSpell = settings.Split('#');
			if (eachSpell.Length != 67)
			{
				Console.WriteLine("Invalid bar setting length '{0}' ({1})", eachSpell.Length, GetType());
				return false;
			}

			AddGlobalConfig(eachSpell[65] == "1", eachSpell[64] == "1");

			int x = 75;
			int y = 135;
			if (PageNumber == 1)
			{
				for (int i = 0; i < 32;)
				{
					string spellName = GetSpellName(i);
					if (string.IsNullOrWhiteSpace(spellName)) break;

					bool isChecked = eachSpell[i] == "1";
					AddSpell(isChecked, i + 1, GetSpellIcon(i), spellName, ref i, ref x, ref y);
				}

				AddPreviousPageButton(102);
				AddNextPageButton(102);
			}
			else if (PageNumber == 2)
			{
				for (int i = 32; i < eachSpell.Length;)
				{
					string spellName = GetSpellName(i);
					if (string.IsNullOrWhiteSpace(spellName)) break;

					bool isChecked = eachSpell[i] == "1";
					AddSpell(isChecked, i + 1, GetSpellIcon(i), spellName, ref i, ref x, ref y);
				}

				AddPreviousPageButton(101);
				AddNextPageButton(101);
			}

			return true;
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			PlayerMobile from = (PlayerMobile)sender.Mobile;

			if (0 < info.ButtonID)
			{
				const int totalOptions = 66; // Note: Changed per bar
				if (info.ButtonID < 90) { ToolBarUpdates.UpdateToolBar(from, info.ButtonID, StorageKey, totalOptions); }
				else if (info.ButtonID == 90) { ToolBarUpdates.UpdateToolBar(from, totalOptions - 1, StorageKey, totalOptions); }
				else if (info.ButtonID == 91) { ToolBarUpdates.UpdateToolBar(from, totalOptions, StorageKey, totalOptions); }
				else if (info.ButtonID == 92) { InvokeCommand(OpenCommand, from); }
				else if (info.ButtonID == 93) { InvokeCommand(CloseCommand, from); }

				if (100 < info.ButtonID)
				{
					ReopenGump(from, info.ButtonID - 100);
					return;
				}
			}

			if (info.ButtonID < 1 && Origin > 0) { from.SendGump(new Server.Engines.Help.HelpGump(from, 7)); from.SendSound(0x4A); }
			else if (info.ButtonID < 1 || 100 < info.ButtonID) { }
			else { ReopenGump(from, PageNumber); from.SendSound(0x4A); }
		}
	}
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Server.SpellBars
{
	public class SetupBarsMage1 : SetupBarsMage
	{
		public SetupBarsMage1(PlayerMobile from, int origin, int pageNumber = 1)
			: base(from, origin, pageNumber, true, "SPELL BAR - MAGERY - I", "SetupBarsMage1", "magetool1", "mageclose1")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("magespell1", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("magespell1")]
		[Description("Opens Spell Bar Editor For Mages - 1.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsMage1(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from, int pageNumber)
		{
			var gump = new SetupBarsMage1(from, Origin, pageNumber);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public class SetupBarsMage2 : SetupBarsMage
	{
		public SetupBarsMage2(PlayerMobile from, int origin, int pageNumber = 1)
			: base(from, origin, pageNumber, true, "SPELL BAR - MAGERY - II", "SetupBarsMage2", "magetool2", "mageclose2")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("magespell2", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("magespell2")]
		[Description("Opens Spell Bar Editor For Mages - 2.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsMage2(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from, int pageNumber)
		{
			var gump = new SetupBarsMage2(from, Origin, pageNumber);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public class SetupBarsMage3 : SetupBarsMage
	{
		public SetupBarsMage3(PlayerMobile from, int origin, int pageNumber = 1)
			: base(from, origin, pageNumber, true, "SPELL BAR - MAGERY - III", "SetupBarsMage3", "magetool3", "mageclose3")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("magespell3", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("magespell3")]
		[Description("Opens Spell Bar Editor For Mages - 3.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsMage3(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from, int pageNumber)
		{
			var gump = new SetupBarsMage3(from, Origin, pageNumber);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public class SetupBarsMage4 : SetupBarsMage
	{
		public SetupBarsMage4(PlayerMobile from, int origin, int pageNumber = 1)
			: base(from, origin, pageNumber, true, "SPELL BAR - MAGERY - IV", "SetupBarsMage4", "magetool4", "mageclose4")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("magespell4", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("magespell4")]
		[Description("Opens Spell Bar Editor For Mages - 4.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsMage4(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from, int pageNumber)
		{
			var gump = new SetupBarsMage4(from, Origin, pageNumber);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public abstract class SetupBarsMage : SetupSpellBarGump
	{
		protected readonly int PageNumber;

		protected SetupBarsMage(PlayerMobile from, int origin, int pageNumber, bool isNumberOneBar, string title, string storageKey, string openCommand, string closeCommand)
			: base(from, origin, isNumberOneBar, title, storageKey, openCommand, closeCommand)
		{
			PageNumber = pageNumber;
		}

		protected abstract void ReopenGump(PlayerMobile from, int pageNumber);

		public static int GetSpellIcon(int i)
		{
			return 2240 + i;
		}

		public static string GetSpellName(int i)
		{
			if (i == 0) return "Clumsy";
			if (i == 1) return "Create Food";
			if (i == 2) return "Feeblemind";
			if (i == 3) return "Heal";
			if (i == 4) return "Magic Arrow";
			if (i == 5) return "Night Sight";
			if (i == 6) return "Reactive Armor";
			if (i == 7) return "Weaken";
			if (i == 8) return "Agility";
			if (i == 9) return "Cunning";
			if (i == 10) return "Cure";
			if (i == 11) return "Harm";
			if (i == 12) return "Magic Trap";
			if (i == 13) return "Remove Trap";
			if (i == 14) return "Protection";
			if (i == 15) return "Strength";
			if (i == 16) return "Bless";
			if (i == 17) return "Fireball";
			if (i == 18) return "MagicLock";
			if (i == 19) return "Poison";
			if (i == 20) return "Telekinesis";
			if (i == 21) return "Teleport";
			if (i == 22) return "Unlock";
			if (i == 23) return "Wall Of Stone";
			if (i == 24) return "Arch Cure";
			if (i == 25) return "Arch Protection";
			if (i == 26) return "Curse";
			if (i == 27) return "Fire Field";
			if (i == 28) return "Greater Heal";
			if (i == 29) return "Lightning";
			if (i == 30) return "Mana Drain";
			if (i == 31) return "Recall";
			if (i == 32) return "Blade Spirits";
			if (i == 33) return "Dispel Field";
			if (i == 34) return "Incognito";
			if (i == 35) return "Magic Reflect";
			if (i == 36) return "Mind Blast";
			if (i == 37) return "Paralyze";
			if (i == 38) return "Poison Field";
			if (i == 39) return "Summon Creature";
			if (i == 40) return "Dispel";
			if (i == 41) return "Energy Bolt";
			if (i == 42) return "Explosion";
			if (i == 43) return "Invisibility";
			if (i == 44) return "Mark";
			if (i == 45) return "Mass Curse";
			if (i == 46) return "Paralyze Field";
			if (i == 47) return "Reveal";
			if (i == 48) return "Chain Lightning";
			if (i == 49) return "Energy Field";
			if (i == 50) return "Flame Strike";
			if (i == 51) return "Gate Travel";
			if (i == 52) return "Mana Vampire";
			if (i == 53) return "Mass Dispel";
			if (i == 54) return "Meteor Swarm";
			if (i == 55) return "Polymorph";
			if (i == 56) return "Earthquake";
			if (i == 57) return "Energy Vortex";
			if (i == 58) return "Resurrection";
			if (i == 59) return "Air Elemental";
			if (i == 60) return "Summon Daemon";
			if (i == 61) return "Earth Elemental";
			if (i == 62) return "Fire Elemental";
			if (i == 63) return "Water Elemental";

			return "";
		}

		public override bool ConfigureGump()
		{
			string settings = ToolBarUpdates.GetToolBarSettings(Player, StorageKey);
			string[] eachSpell = settings.Split('#');
			if (eachSpell.Length != 67)
			{
				Console.WriteLine("Invalid bar setting length '{0}' ({1})", eachSpell.Length, GetType());
				return false;
			}

			AddGlobalConfig(eachSpell[65] == "1", eachSpell[64] == "1");

			int x = 75;
			int y = 135;
			if (PageNumber == 1)
			{
				for (int i = 0; i < 32;)
				{
					string spellName = GetSpellName(i);
					if (string.IsNullOrWhiteSpace(spellName)) break;

					bool isChecked = eachSpell[i] == "1";
					AddSpell(isChecked, i + 1, GetSpellIcon(i), spellName, ref i, ref x, ref y);
				}

				AddPreviousPageButton(102);
				AddNextPageButton(102);
			}
			else if (PageNumber == 2)
			{
				for (int i = 32; i < eachSpell.Length;)
				{
					string spellName = GetSpellName(i);
					if (string.IsNullOrWhiteSpace(spellName)) break;

					bool isChecked = eachSpell[i] == "1";
					AddSpell(isChecked, i + 1, GetSpellIcon(i), spellName, ref i, ref x, ref y);
				}

				AddPreviousPageButton(101);
				AddNextPageButton(101);
			}

			return true;
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			PlayerMobile from = (PlayerMobile)sender.Mobile;

			if (0 < info.ButtonID)
			{
				const int totalOptions = 66; // Note: Changed per bar
				if (info.ButtonID < 90) { ToolBarUpdates.UpdateToolBar(from, info.ButtonID, StorageKey, totalOptions); }
				else if (info.ButtonID == 90) { ToolBarUpdates.UpdateToolBar(from, totalOptions - 1, StorageKey, totalOptions); }
				else if (info.ButtonID == 91) { ToolBarUpdates.UpdateToolBar(from, totalOptions, StorageKey, totalOptions); }
				else if (info.ButtonID == 92) { InvokeCommand(OpenCommand, from); }
				else if (info.ButtonID == 93) { InvokeCommand(CloseCommand, from); }

				if (100 < info.ButtonID)
				{
					ReopenGump(from, info.ButtonID - 100);
					return;
				}
			}

			if (info.ButtonID < 1 && Origin > 0) { from.SendGump(new Server.Engines.Help.HelpGump(from, 7)); from.SendSound(0x4A); }
			else if (info.ButtonID < 1 || 100 < info.ButtonID) { }
			else { ReopenGump(from, PageNumber); from.SendSound(0x4A); }
		}
	}
}

namespace Server.SpellBars
{
	public class SetupBarsElement1 : SetupBarsElementalism
	{
		public SetupBarsElement1(PlayerMobile from, int origin)
			: base(from, origin, true, "SPELL BAR - ELEMENTALIST - I", "SetupBarsElly1", "elementtool1", "elementclose1")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("elementspell1", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("elementspell1")]
		[Description("Opens Spell Bar Editor For Elementalists - 1.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsElement1(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from)
		{
			var gump = new SetupBarsElement1(from, Origin);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public class SetupBarsElement2 : SetupBarsElementalism
	{
		public SetupBarsElement2(PlayerMobile from, int origin)
			: base(from, origin, true, "SPELL BAR - ELEMENTALIST - II", "SetupBarsElly2", "elementtool2", "elementclose2")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("elementspell2", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("elementspell2")]
		[Description("Opens Spell Bar Editor For Elementalists - 2.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsElement2(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from)
		{
			var gump = new SetupBarsElement2(from, Origin);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public abstract class SetupBarsElementalism : SetupSpellBarGump
	{
		protected SetupBarsElementalism(PlayerMobile from, int origin, bool isNumberOneBar, string title, string storageKey, string openCommand, string closeCommand)
			: base(from, origin, isNumberOneBar, title, storageKey, openCommand, closeCommand)
		{
		}

		protected abstract void ReopenGump(PlayerMobile from);

		public static int GetSpellIcon(int i, int characterElement)
		{
			int book = 0x6717;
			if (characterElement == 1) { book = 0x6713; }
			else if (characterElement == 2) { book = 0x6719; }
			else if (characterElement == 3) { book = 0x6715; }

			int spellId = 300 + i;
			return ElementalSpell.SpellIcon(book, spellId);
		}

		public static string GetSpellName(int i)
		{
			int spellId = 300 + i;
			return ElementalSpell.CommonInfo(spellId, 1);
		}

		public override bool ConfigureGump()
		{
			string settings = ToolBarUpdates.GetToolBarSettings(Player, StorageKey);
			string[] eachSpell = settings.Split('#');
			if (eachSpell.Length != 35)
			{
				Console.WriteLine("Invalid bar setting length '{0}' ({1})", eachSpell.Length, GetType());
				return false;
			}

			AddGlobalConfig(eachSpell[33] == "1", eachSpell[32] == "1");

			int x = 75;
			int y = 135;
			for (int i = 0; i < eachSpell.Length;)
			{
				string spellName = GetSpellName(i);
				if (string.IsNullOrWhiteSpace(spellName)) break;

				bool isChecked = eachSpell[i] == "1";
				AddSpell(isChecked, i + 1, GetSpellIcon(i, Player.CharacterElement), spellName, ref i, ref x, ref y);
			}

			return true;
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			PlayerMobile from = (PlayerMobile)sender.Mobile;

			if (0 < info.ButtonID)
			{
				const int totalOptions = 34; // Note: Changed per bar
				if (info.ButtonID < 90) { ToolBarUpdates.UpdateToolBar(from, info.ButtonID, StorageKey, totalOptions); }
				else if (info.ButtonID == 90) { ToolBarUpdates.UpdateToolBar(from, totalOptions - 1, StorageKey, totalOptions); }
				else if (info.ButtonID == 91) { ToolBarUpdates.UpdateToolBar(from, totalOptions, StorageKey, totalOptions); }
				else if (info.ButtonID == 92) { InvokeCommand(OpenCommand, from); }
				else if (info.ButtonID == 93) { InvokeCommand(CloseCommand, from); }
			}

			if (info.ButtonID < 1 && Origin > 0) { from.SendGump(new Server.Engines.Help.HelpGump(from, 7)); from.SendSound(0x4A); }
			else if (info.ButtonID < 1) { }
			else { ReopenGump(from); from.SendSound(0x4A); }
		}
	}
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Server.SpellBars
{
	public class SetupBarsNecro1 : SetupBarsNecro
	{
		public SetupBarsNecro1(PlayerMobile from, int origin)
			: base(from, origin, true, "SPELL BAR - NECROMANCER - I", "SetupBarsNecro1", "necrotool1", "necroclose1")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("necrospell1", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("necrospell1")]
		[Description("Opens Spell Bar Editor For Necromancers - 1.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsNecro1(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from)
		{
			var gump = new SetupBarsNecro1(from, Origin);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public class SetupBarsNecro2 : SetupBarsNecro
	{
		public SetupBarsNecro2(PlayerMobile from, int origin)
			: base(from, origin, true, "SPELL BAR - NECROMANCER - II", "SetupBarsNecro2", "necrotool2", "necroclose2")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("necrospell2", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("necrospell2")]
		[Description("Opens Spell Bar Editor For Necromancers - 2.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsNecro2(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from)
		{
			var gump = new SetupBarsNecro2(from, Origin);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public abstract class SetupBarsNecro : SetupSpellBarGump
	{
		protected SetupBarsNecro(PlayerMobile from, int origin, bool isNumberOneBar, string title, string storageKey, string openCommand, string closeCommand)
			: base(from, origin, isNumberOneBar, title, storageKey, openCommand, closeCommand)
		{
		}

		protected abstract void ReopenGump(PlayerMobile from);

		public static int GetSpellIcon(int i)
		{
			return 20480 + i;
		}

		public static string GetSpellName(int i)
		{
			if (i == 0) return "Animate Dead";
			if (i == 1) return "Blood Oath";
			if (i == 2) return "Corpse Skin";
			if (i == 3) return "Curse Weapon";
			if (i == 4) return "Evil Omen";
			if (i == 5) return "Horrific Beast";
			if (i == 6) return "Lich Form";
			if (i == 7) return "Mind Rot";
			if (i == 8) return "Pain Spike";
			if (i == 9) return "Poison Strike";
			if (i == 10) return "Strangle";
			if (i == 11) return "Summon Familiar";
			if (i == 12) return "Vampiric Embrace";
			if (i == 13) return "Vengeful Spirit";
			if (i == 14) return "Wither";
			if (i == 15) return "Wraith Form";
			if (i == 16) return "Exorcism";

			return "";
		}

		public override bool ConfigureGump()
		{
			string settings = ToolBarUpdates.GetToolBarSettings(Player, StorageKey);
			string[] eachSpell = settings.Split('#');
			if (eachSpell.Length != 20)
			{
				Console.WriteLine("Invalid bar setting length '{0}' ({1})", eachSpell.Length, GetType());
				return false;
			}

			AddGlobalConfig(eachSpell[18] == "1", eachSpell[17] == "1");

			int x = 75;
			int y = 135;
			for (int i = 0; i < eachSpell.Length;)
			{
				string spellName = GetSpellName(i);
				if (string.IsNullOrWhiteSpace(spellName)) break;

				bool isChecked = eachSpell[i] == "1";
				AddSpell(isChecked, i + 1, GetSpellIcon(i), spellName, ref i, ref x, ref y);
			}

			return true;
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			PlayerMobile from = (PlayerMobile)sender.Mobile;

			if (0 < info.ButtonID)
			{
				const int totalOptions = 19; // Note: Changed per bar
				if (info.ButtonID < 90) { ToolBarUpdates.UpdateToolBar(from, info.ButtonID, StorageKey, totalOptions); }
				else if (info.ButtonID == 90) { ToolBarUpdates.UpdateToolBar(from, totalOptions - 1, StorageKey, totalOptions); }
				else if (info.ButtonID == 91) { ToolBarUpdates.UpdateToolBar(from, totalOptions, StorageKey, totalOptions); }
				else if (info.ButtonID == 92) { InvokeCommand(OpenCommand, from); }
				else if (info.ButtonID == 93) { InvokeCommand(CloseCommand, from); }
			}

			if (info.ButtonID < 1 && Origin > 0) { from.SendGump(new Server.Engines.Help.HelpGump(from, 7)); from.SendSound(0x4A); }
			else if (info.ButtonID < 1) { }
			else { ReopenGump(from); from.SendSound(0x4A); }
		}
	}
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Server.SpellBars
{
	public class SetupBarsKnight1 : SetupBarsKnight
	{
		public SetupBarsKnight1(PlayerMobile from, int origin)
			: base(from, origin, true, "SPELL BAR - KNIGHT - I", "SetupBarsKnight1", "knighttool1", "knightclose1")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("knightspell1", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("knightspell1")]
		[Description("Opens Spell Bar Editor For Knights - 1.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsKnight1(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from)
		{
			var gump = new SetupBarsKnight1(from, Origin);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public class SetupBarsKnight2 : SetupBarsKnight
	{
		public SetupBarsKnight2(PlayerMobile from, int origin)
			: base(from, origin, false, "SPELL BAR - KNIGHT - II", "SetupBarsKnight2", "knighttool2", "knightclose2")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("knightspell2", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("knightspell2")]
		[Description("Opens Spell Bar Editor For Knights - 2.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsKnight2(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from)
		{
			var gump = new SetupBarsKnight2(from, Origin);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public abstract class SetupBarsKnight : SetupSpellBarGump
	{
		protected SetupBarsKnight(PlayerMobile from, int origin, bool isNumberOneBar, string title, string storageKey, string openCommand, string closeCommand)
			: base(from, origin, isNumberOneBar, title, storageKey, openCommand, closeCommand)
		{
		}

		protected abstract void ReopenGump(PlayerMobile from);

		public static int GetSpellIcon(int i)
		{
			return 20736 + i;
		}

		public static string GetSpellName(int i)
		{
			if (i == 0) return "Cleanse by Fire";
			if (i == 1) return "Close Wounds";
			if (i == 2) return "Consecrate Weapon";
			if (i == 3) return "Dispel Evil";
			if (i == 4) return "Divine Fury";
			if (i == 5) return "Enemy of One";
			if (i == 6) return "Holy Light";
			if (i == 7) return "Noble Sacrifice";
			if (i == 8) return "Remove Curse";
			if (i == 9) return "Sacred Journey";

			return "";
		}

		public override bool ConfigureGump()
		{
			string settings = ToolBarUpdates.GetToolBarSettings(Player, StorageKey);
			string[] eachSpell = settings.Split('#');
			if (eachSpell.Length != 13)
			{
				Console.WriteLine("Invalid bar setting length '{0}' ({1})", eachSpell.Length, GetType());
				return false;
			}

			AddGlobalConfig(eachSpell[11] == "1", eachSpell[10] == "1");

			int x = 75;
			int y = 135;
			for (int i = 0; i < eachSpell.Length;)
			{
				string spellName = GetSpellName(i);
				if (string.IsNullOrWhiteSpace(spellName)) break;

				bool isChecked = eachSpell[i] == "1";
				AddSpell(isChecked, i + 1, GetSpellIcon(i), spellName, ref i, ref x, ref y);
			}

			return true;
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			PlayerMobile from = (PlayerMobile)sender.Mobile;

			if (0 < info.ButtonID)
			{
				const int totalOptions = 12; // Note: Changed per bar
				if (info.ButtonID < 90) { ToolBarUpdates.UpdateToolBar(from, info.ButtonID, StorageKey, totalOptions); }
				else if (info.ButtonID == 90) { ToolBarUpdates.UpdateToolBar(from, totalOptions - 1, StorageKey, totalOptions); }
				else if (info.ButtonID == 91) { ToolBarUpdates.UpdateToolBar(from, totalOptions, StorageKey, totalOptions); }
				else if (info.ButtonID == 92) { InvokeCommand(OpenCommand, from); }
				else if (info.ButtonID == 93) { InvokeCommand(CloseCommand, from); }
			}

			if (info.ButtonID < 1 && Origin > 0) { from.SendGump(new Server.Engines.Help.HelpGump(from, 7)); from.SendSound(0x4A); }
			else if (info.ButtonID < 1) { }
			else { ReopenGump(from); from.SendSound(0x4A); }
		}
	}
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Server.SpellBars
{

	public class SetupBarsBard1 : SetupBarsBard
	{
		public SetupBarsBard1(PlayerMobile from, int origin)
			: base(from, origin, true, "SPELL BAR - BARD - I", "SetupBarsBard1", "bardtool1", "bardclose1")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("bardsong1", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("bardsong1")]
		[Description("Opens Spell Bar Editor For Bards - 1.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsBard1(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from)
		{
			var gump = new SetupBarsBard1(from, Origin);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public class SetupBarsBard2 : SetupBarsBard
	{
		public SetupBarsBard2(PlayerMobile from, int origin)
			: base(from, origin, false, "SPELL BAR - BARD - II", "SetupBarsBard2", "bardtool2", "bardclose2")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("bardsong2", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("bardsong2")]
		[Description("Opens Spell Bar Editor For Bards - 2.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsBard2(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from)
		{
			var gump = new SetupBarsBard2(from, Origin);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public abstract class SetupBarsBard : SetupSpellBarGump
	{
		protected SetupBarsBard(PlayerMobile from, int origin, bool isNumberOneBar, string title, string storageKey, string openCommand, string closeCommand)
			: base(from, origin, isNumberOneBar, title, storageKey, openCommand, closeCommand)
		{
		}

		protected abstract void ReopenGump(PlayerMobile from);

		public static int GetSpellIcon(int i)
		{
			if (10 < i) i++; // Skip a slot

			return 1028 + i;
		}

		public static string GetSpellName(int i)
		{
			if (i == 0) return "Army's Paeon";
			if (i == 1) return "Enchanting Etude";
			if (i == 2) return "Energy Carol";
			if (i == 3) return "Energy Threnody";
			if (i == 4) return "Fire Carol";
			if (i == 5) return "Fire Threnody";
			if (i == 6) return "Foe Requiem";
			if (i == 7) return "Ice Carol";
			if (i == 8) return "Ice Threnody";
			if (i == 9) return "Knight's Minne";
			if (i == 10) return "Mage's Ballad";
			if (i == 11) return "Magic Finale";
			if (i == 12) return "Poison Carol";
			if (i == 13) return "Poison Threnody";
			if (i == 14) return "Shepherd's Dance";
			if (i == 15) return "Sinewy Etude";

			return "";
		}

		public override bool ConfigureGump()
		{
			string settings = ToolBarUpdates.GetToolBarSettings(Player, StorageKey);
			string[] eachSpell = settings.Split('#');
			if (eachSpell.Length != 19)
			{
				Console.WriteLine("Invalid bar setting length '{0}' ({1})", eachSpell.Length, GetType());
				return false;
			}

			AddGlobalConfig(eachSpell[17] == "1", eachSpell[16] == "1");

			int x = 75;
			int y = 135;
			for (int i = 0; i < eachSpell.Length;)
			{
				string spellName = GetSpellName(i);
				if (string.IsNullOrWhiteSpace(spellName)) break;

				bool isChecked = eachSpell[i] == "1";
				AddSpell(isChecked, i + 1, GetSpellIcon(i), spellName, ref i, ref x, ref y);
			}

			return true;
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			PlayerMobile from = (PlayerMobile)sender.Mobile;

			if (0 < info.ButtonID)
			{
				const int totalOptions = 18; // Note: Changed per bar
				if (info.ButtonID < 90) { ToolBarUpdates.UpdateToolBar(from, info.ButtonID, StorageKey, totalOptions); }
				else if (info.ButtonID == 90) { ToolBarUpdates.UpdateToolBar(from, totalOptions - 1, StorageKey, totalOptions); }
				else if (info.ButtonID == 91) { ToolBarUpdates.UpdateToolBar(from, totalOptions, StorageKey, totalOptions); }
				else if (info.ButtonID == 92) { InvokeCommand(OpenCommand, from); }
				else if (info.ButtonID == 93) { InvokeCommand(CloseCommand, from); }
			}

			if (info.ButtonID < 1 && Origin > 0) { from.SendGump(new Server.Engines.Help.HelpGump(from, 7)); from.SendSound(0x4A); }
			else if (info.ButtonID < 1) { }
			else { ReopenGump(from); from.SendSound(0x4A); }
		}
	}
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Server.SpellBars
{
	public class SetupBarsDeath1 : SetupBarsDeath
	{
		public SetupBarsDeath1(PlayerMobile from, int origin)
			: base(from, origin, true, "SPELL BAR - KNIGHT - I", "SetupBarsDeath1", "deathtool1", "deathclose1")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("deathspell1", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("deathspell1")]
		[Description("Opens Spell Bar Editor For Death Knights - 1.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsDeath1(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from)
		{
			var gump = new SetupBarsDeath1(from, Origin);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public class SetupBarsDeath2 : SetupBarsDeath
	{
		public SetupBarsDeath2(PlayerMobile from, int origin)
			: base(from, origin, false, "SPELL BAR - KNIGHT - II", "SetupBarsDeath2", "deathtool2", "deathclose2")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("deathspell2", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("deathspell2")]
		[Description("Opens Spell Bar Editor For Death Knights - 2.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsDeath2(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from)
		{
			var gump = new SetupBarsDeath2(from, Origin);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public abstract class SetupBarsDeath : SetupSpellBarGump
	{
		protected SetupBarsDeath(PlayerMobile from, int origin, bool isNumberOneBar, string title, string storageKey, string openCommand, string closeCommand)
			: base(from, origin, isNumberOneBar, title, storageKey, openCommand, closeCommand)
		{
		}

		protected abstract void ReopenGump(PlayerMobile from);

		public static int GetSpellIcon(int i)
		{
			if (i == 0) return 0x5010;
			if (i == 1) return 0x5009;
			if (i == 2) return 0x5005;
			if (i == 3) return 0x402;
			if (i == 4) return 0x5002;
			if (i == 5) return 0x3E9;
			if (i == 6) return 0x5DC0;
			if (i == 7) return 0x1B;
			if (i == 8) return 0x3EE;
			if (i == 9) return 0x5006;
			if (i == 10) return 0x2B;
			if (i == 11) return 0x12;
			if (i == 12) return 0x500C;
			if (i == 13) return 0x2E;

			return 0;
		}

		public static string GetSpellName(int i)
		{
			if (i == 0) return "Banish";
			if (i == 1) return "Demonic Touch";
			if (i == 2) return "Devil Pact";
			if (i == 3) return "Grim Reaper";
			if (i == 4) return "Hag Hand";
			if (i == 5) return "Hellfire";
			if (i == 6) return "Lucifer's Bolt";
			if (i == 7) return "Orb of Orcus";
			if (i == 8) return "Shield of Hate";
			if (i == 9) return "Soul Reaper";
			if (i == 10) return "Strength of Steel";
			if (i == 11) return "Strike";
			if (i == 12) return "Succubus Skin";
			if (i == 13) return "Wrath";

			return "";
		}

		public override bool ConfigureGump()
		{
			string settings = ToolBarUpdates.GetToolBarSettings(Player, StorageKey);
			string[] eachSpell = settings.Split('#');
			if (eachSpell.Length != 17)
			{
				Console.WriteLine("Invalid bar setting length '{0}' ({1})", eachSpell.Length, GetType());
				return false;
			}

			AddGlobalConfig(eachSpell[15] == "1", eachSpell[14] == "1");

			int x = 75;
			int y = 135;
			for (int i = 0; i < eachSpell.Length;)
			{
				string spellName = GetSpellName(i);
				if (string.IsNullOrWhiteSpace(spellName)) break;

				bool isChecked = eachSpell[i] == "1";
				AddSpell(isChecked, i + 1, GetSpellIcon(i), spellName, ref i, ref x, ref y);
			}

			return true;
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			PlayerMobile from = (PlayerMobile)sender.Mobile;

			if (0 < info.ButtonID)
			{
				const int totalOptions = 16; // Note: Changed per bar
				if (info.ButtonID < 90) { ToolBarUpdates.UpdateToolBar(from, info.ButtonID, StorageKey, totalOptions); }
				else if (info.ButtonID == 90) { ToolBarUpdates.UpdateToolBar(from, totalOptions - 1, StorageKey, totalOptions); }
				else if (info.ButtonID == 91) { ToolBarUpdates.UpdateToolBar(from, totalOptions, StorageKey, totalOptions); }
				else if (info.ButtonID == 92) { InvokeCommand(OpenCommand, from); }
				else if (info.ButtonID == 93) { InvokeCommand(CloseCommand, from); }
			}

			if (info.ButtonID < 1 && Origin > 0) { from.SendGump(new Server.Engines.Help.HelpGump(from, 7)); from.SendSound(0x4A); }
			else if (info.ButtonID < 1) { }
			else { ReopenGump(from); from.SendSound(0x4A); }
		}
	}
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Server.SpellBars
{
	public class SetupBarsPriest1 : SetupBarsPriest
	{
		public SetupBarsPriest1(PlayerMobile from, int origin)
			: base(from, origin, true, "SPELL BAR - PRIEST - I", "SetupBarsPriest1", "holytool1", "holyclose1")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("holyspell1", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("holyspell1")]
		[Description("Opens Spell Bar Editor For Prayers - 1.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsPriest1(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from)
		{
			var gump = new SetupBarsPriest1(from, Origin);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public class SetupBarsPriest2 : SetupBarsPriest
	{
		public SetupBarsPriest2(PlayerMobile from, int origin)
			: base(from, origin, false, "SPELL BAR - PRIEST - II", "SetupBarsPriest2", "holytool2", "holyclose2")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("holyspell2", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("holyspell2")]
		[Description("Opens Spell Bar Editor For Prayers - 2.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsPriest2(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from)
		{
			var gump = new SetupBarsPriest2(from, Origin);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public abstract class SetupBarsPriest : SetupSpellBarGump
	{
		protected SetupBarsPriest(PlayerMobile from, int origin, bool isNumberOneBar, string title, string storageKey, string openCommand, string closeCommand)
			: base(from, origin, isNumberOneBar, title, storageKey, openCommand, closeCommand)
		{
		}

		protected abstract void ReopenGump(PlayerMobile from);

		public static int GetSpellIcon(int i)
		{
			if (i == 0) return 0x965;
			if (i == 1) return 0x966;
			if (i == 2) return 0x967;
			if (i == 3) return 0x968;
			if (i == 4) return 0x969;
			if (i == 5) return 0x96A;
			if (i == 6) return 0x96B;
			if (i == 7) return 0x96C;
			if (i == 8) return 0x96E;
			if (i == 9) return 0x96D;
			if (i == 10) return 0x96F;
			if (i == 11) return 0x970;
			if (i == 12) return 0x971;
			if (i == 13) return 0x972;

			return 0;
		}

		public static string GetSpellName(int i)
		{
			if (i == 0) return "Banish";
			if (i == 1) return "Dampen Spirit";
			if (i == 2) return "Enchant";
			if (i == 3) return "Hammer of Faith";
			if (i == 4) return "Heavenly Light";
			if (i == 5) return "Nourish";
			if (i == 6) return "Purge";
			if (i == 7) return "Rebirth";
			if (i == 8) return "Sacred Boon";
			if (i == 9) return "Sactify";
			if (i == 10) return "Seance";
			if (i == 11) return "Smite";
			if (i == 12) return "Touch of Life";
			if (i == 13) return "Trial by Fire";

			return "";
		}

		public override bool ConfigureGump()
		{
			string settings = ToolBarUpdates.GetToolBarSettings(Player, StorageKey);
			string[] eachSpell = settings.Split('#');
			if (eachSpell.Length != 17)
			{
				Console.WriteLine("Invalid bar setting length '{0}' ({1})", eachSpell.Length, GetType());
				return false;
			}

			AddGlobalConfig(eachSpell[15] == "1", eachSpell[14] == "1");

			int x = 75;
			int y = 135;
			for (int i = 0; i < eachSpell.Length;)
			{
				string spellName = GetSpellName(i);
				if (string.IsNullOrWhiteSpace(spellName)) break;

				bool isChecked = eachSpell[i] == "1";
				AddSpell(isChecked, i + 1, GetSpellIcon(i), spellName, ref i, ref x, ref y);
			}

			return true;
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			PlayerMobile from = (PlayerMobile)sender.Mobile;

			if (0 < info.ButtonID)
			{
				const int totalOptions = 16; // Note: Changed per bar
				if (info.ButtonID < 90) { ToolBarUpdates.UpdateToolBar(from, info.ButtonID, StorageKey, totalOptions); }
				else if (info.ButtonID == 90) { ToolBarUpdates.UpdateToolBar(from, totalOptions - 1, StorageKey, totalOptions); }
				else if (info.ButtonID == 91) { ToolBarUpdates.UpdateToolBar(from, totalOptions, StorageKey, totalOptions); }
				else if (info.ButtonID == 92) { InvokeCommand(OpenCommand, from); }
				else if (info.ButtonID == 93) { InvokeCommand(CloseCommand, from); }
			}

			if (info.ButtonID < 1 && Origin > 0) { from.SendGump(new Server.Engines.Help.HelpGump(from, 7)); from.SendSound(0x4A); }
			else if (info.ButtonID < 1) { }
			else { ReopenGump(from); from.SendSound(0x4A); }
		}
	}
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Server.SpellBars
{
	public class SetupBarsMonk1 : SetupBarsMonk
	{
		public SetupBarsMonk1(PlayerMobile from, int origin)
			: base(from, origin, true, "SPELL BAR - MONK - I", "SetupBarsMonk1", "monktool1", "monkclose1")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("monkspell1", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("monkspell1")]
		[Description("Opens Spell Bar Editor For Monks - 1.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsMonk1(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from)
		{
			var gump = new SetupBarsMonk1(from, Origin);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public class SetupBarsMonk2 : SetupBarsMonk
	{
		public SetupBarsMonk2(PlayerMobile from, int origin)
			: base(from, origin, false, "SPELL BAR - MONK - II", "SetupBarsMonk2", "monktool2", "monkclose2")
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register("monkspell2", AccessLevel.Player, new CommandEventHandler(CreateBarCommand));
		}

		[Usage("monkspell2")]
		[Description("Opens Spell Bar Editor For Monks - 2.")]
		private static void CreateBarCommand(CommandEventArgs e)
		{
			PlayerMobile player = e.Mobile as PlayerMobile;
			if (player == null) return;

			var gump = new SetupBarsMonk2(player, 0);
			if (!gump.ConfigureGump()) return;

			player.CloseGump(gump.GetType());
			player.SendGump(gump);
		}

		protected override void ReopenGump(PlayerMobile from)
		{
			var gump = new SetupBarsMonk2(from, Origin);
			if (!gump.ConfigureGump()) return;

			from.CloseGump(gump.GetType());
			from.SendGump(gump);
		}
	}

	public abstract class SetupBarsMonk : SetupSpellBarGump
	{
		protected SetupBarsMonk(PlayerMobile from, int origin, bool isNumberOneBar, string title, string storageKey, string openCommand, string closeCommand)
			: base(from, origin, isNumberOneBar, title, storageKey, openCommand, closeCommand)
		{
		}

		protected abstract void ReopenGump(PlayerMobile from);

		public static int GetSpellIcon(int i)
		{
			if (i == 0) return 0x500E;
			if (i == 1) return 0x410;
			if (i == 2) return 0x15;
			if (i == 3) return 0x971;
			if (i == 4) return 0x4B2;
			if (i == 5) return 0x5DC2;
			if (i == 6) return 0x1A;
			if (i == 7) return 0x96D;
			if (i == 8) return 0x5001;
			if (i == 9) return 0x19;

			return 0;
		}

		public static string GetSpellName(int i)
		{
			if (i == 0) return "Astral Projection";
			if (i == 1) return "Astral Travel";
			if (i == 2) return "Create Robe";
			if (i == 3) return "Gentle Touch";
			if (i == 4) return "Leap";
			if (i == 5) return "Psionic Blast";
			if (i == 6) return "Psychic Wall";
			if (i == 7) return "Purity of Body";
			if (i == 8) return "Quivering Palm";
			if (i == 9) return "Wind Runner";

			return "";
		}

		public override bool ConfigureGump()
		{
			string settings = ToolBarUpdates.GetToolBarSettings(Player, StorageKey);
			string[] eachSpell = settings.Split('#');
			if (eachSpell.Length != 13)
			{
				Console.WriteLine("Invalid bar setting length '{0}' ({1})", eachSpell.Length, GetType());
				return false;
			}

			AddGlobalConfig(eachSpell[11] == "1", eachSpell[10] == "1");

			int x = 75;
			int y = 135;
			for (int i = 0; i < eachSpell.Length;)
			{
				string spellName = GetSpellName(i);
				if (string.IsNullOrWhiteSpace(spellName)) break;

				bool isChecked = eachSpell[i] == "1";
				AddSpell(isChecked, i + 1, GetSpellIcon(i), spellName, ref i, ref x, ref y);
			}

			return true;
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			PlayerMobile from = (PlayerMobile)sender.Mobile;

			if (0 < info.ButtonID)
			{
				const int totalOptions = 12; // Note: Changed per bar
				if (info.ButtonID < 90) { ToolBarUpdates.UpdateToolBar(from, info.ButtonID, StorageKey, totalOptions); }
				else if (info.ButtonID == 90) { ToolBarUpdates.UpdateToolBar(from, totalOptions - 1, StorageKey, totalOptions); }
				else if (info.ButtonID == 91) { ToolBarUpdates.UpdateToolBar(from, totalOptions, StorageKey, totalOptions); }
				else if (info.ButtonID == 92) { InvokeCommand(OpenCommand, from); }
				else if (info.ButtonID == 93) { InvokeCommand(CloseCommand, from); }
			}

			if (info.ButtonID < 1 && Origin > 0) { from.SendGump(new Server.Engines.Help.HelpGump(from, 7)); from.SendSound(0x4A); }
			else if (info.ButtonID < 1) { }
			else { ReopenGump(from); from.SendSound(0x4A); }
		}
	}
}