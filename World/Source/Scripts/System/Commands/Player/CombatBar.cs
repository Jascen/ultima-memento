using Server.Mobiles;
using Server.Commands;
using Server.Items;

namespace Server.Gumps
{
	public class CombatBar
	{
		const string COLOR_ORANGE = "#FFA200";
		const string COLOR_YELLOW = "#FCFF00";
		const string COLOR_RED = "#FF0000";
		const int LABEL_AMOUNT_OFFSET_X = 60;

		public static void Initialize()
		{
			CommandSystem.Register("combatbar", AccessLevel.Player, new CommandEventHandler(ToolBar_OnCommand));
		}

		public static void Register(string command, AccessLevel access, CommandEventHandler handler)
		{
			CommandSystem.Register(command, access, handler);
		}

		[Usage("combatbar")]
		[Description("Opens the Combat Bar.")]
		public static void ToolBar_OnCommand(CommandEventArgs e)
		{
			Refresh(e.Mobile as PlayerMobile, true);
		}

		public static void Refresh(Mobile from, bool force = false)
		{
			if (from is PlayerMobile)
			{
				if (from.HasGump(typeof(CombatBarGump)) || force)
				{
					from.CloseGump(typeof(CombatBarGump));
					from.SendGump(new CombatBarGump((PlayerMobile)from));
				}
			}
		}

		public class CombatBarGump : Gump
		{
			public CombatBarGump(PlayerMobile player) : base(25, 25)
			{
				this.Closable = true;
				this.Disposable = true;
				this.Dragable = true;
				this.Resizable = false;

				const int GAP_SMALL = 10;
				const int GAP_MEDIUM = 20;
				const int GAP_LARGE = 25;

				AddPage(0);

				AddImage(0, 0, 30008); // Drag handle, Blue gem

				int x = 20;
				int y = 15;

				Add(x, y, "Hunger", player.Hunger, 5);
				y += GAP_MEDIUM;

				Add(x, y, "Thirst", player.Thirst, 5);
				y += GAP_MEDIUM;

				y += GAP_SMALL; // Spacer

				Add(x, y, "Karma", player.Karma);
				y += GAP_MEDIUM;

				Add(x, y, "Fame", player.Fame);
				y += GAP_MEDIUM;

				y += GAP_SMALL; // Spacer

				if (player.TithingPoints > 0)
				{
					AddItem(x + 20, y, 0x2252); // Chivalry Book
					Add(x + LABEL_AMOUNT_OFFSET_X, y, player.TithingPoints, 300);
					y += GAP_LARGE;
				}
				if (player.Backpack != null && player.Skills[SkillName.Healing].Base >= 5.0)
				{
					AddItem(x + 20, y, 0xE21); // Bandage
					Add(x + LABEL_AMOUNT_OFFSET_X, y, player.Backpack.GetAmount(typeof(Bandage), true), 50);
					y += GAP_LARGE;
				}

				if (player.Avatar.Active)
				{
					var coins = player.Avatar.PointsFarmed + player.Avatar.PointsSaved;
					Add(x, y, "Avatar<br>Coins", coins, true);
				}
			}

			private void Add(int x, int y, int amount, int warningAmount = -1)
			{
				AddHtml(x, y, 50, 40, @"<BASEFONT Color=" + (warningAmount >= 0 && amount <= warningAmount ? COLOR_RED : COLOR_YELLOW) + ">" + amount.ToString("n0") + "</BASEFONT>", false, false);
			}

			private void Add(int x, int y, string label, int amount, int warningAmount = -1)
			{
				Add(x, y, label, amount, false, warningAmount);
			}

			private void Add(int x, int y, string label, int amount, bool multiLineLabel, int warningAmount = -1)
			{
				AddHtml(x, y, 60, 40, @"<BASEFONT Color=#FCFF00>" + label + "</BASEFONT>", (bool)false, (bool)false);

				if (multiLineLabel) y += 20;
				AddHtml(x + LABEL_AMOUNT_OFFSET_X, y, 75, 20, @"<BASEFONT Color=" + (warningAmount >= 0 && amount <= warningAmount ? COLOR_RED : COLOR_YELLOW) + ">" + amount.ToString("n0") + "</BASEFONT>", false, false);
			}
		}
	}
}
