using System;
using System.Collections.Generic;
using System.Linq;
using Server.Commands.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Utilities;

namespace Server.Commands
{
	public class MobileUO
	{
		public static void Initialize()
		{
			CommandSystem.Register("SuppressTooltips", AccessLevel.Player, new CommandEventHandler(OnToggleSuppressTooltips));
		}

		[Usage("SuppressTooltips")]
		[Description("Enables or disables the vendor tooltips.")]
		private static void OnToggleSuppressTooltips(CommandEventArgs e)
		{
			var player = e.Mobile as PlayerMobile;
			if (player == null) return;

			player.Preferences.SuppressVendorTooltip = !player.Preferences.SuppressVendorTooltip;

			var message = player.Preferences.SuppressVendorTooltip
				? "Vendor tooltips disabled. Vendor tooltips will no longer be sent to the Client."
				: "Vendor tooltips have been enabled.";
			player.SendMessage(68, message);
		}

		public class OrderByCommand : BaseCommand
		{
			private readonly Func<IEnumerable<Item>, IEnumerable<Item>> m_Sort;

			public OrderByCommand(string command, string description, Func<IEnumerable<Item>, IEnumerable<Item>> sort)
			{
				AccessLevel = AccessLevel.Player;
				Supports = CommandSupport.AllItems;
				Commands = new string[] { command };
				ObjectTypes = ObjectTypes.Items;
				Usage = string.Format("{0} [<Horizontal_Space> | NULL] [<Vertical_Space> | NULL]", command);
				Description = description;
				m_Sort = sort;
			}

			public static void Initialize()
			{
				TargetCommands.Register(new OrderByCommand("OrderBy-Graphic", "Orders the items by their Graphic (Asc)", (items) => items.OrderBy(item => item.ItemID)));
				TargetCommands.Register(new OrderByCommand("OrderBy-Hue", "Orders the items by their Hue (Asc)", (items) => items.OrderBy(item => item.Hue)));
				TargetCommands.Register(new OrderByCommand("OrderBy-Name", "Orders the items by their Name (Asc)", (items) => items.OrderBy(item => item.Name)));
				TargetCommands.Register(new OrderByCommand("OrderBy-Size", "Orders the items by their Size (Asc)", (items) => items.OrderBy(item =>
				{
					var bounds = item.GetGraphicBounds();
					return bounds.Width * bounds.Height;
				})));
				TargetCommands.Register(new OrderByCommand("OrderBy-Slayer", "Orders weapons by their Slayer (Asc)", (items) =>
				{
					return items
						.Where(item => item is BaseWeapon || item is Spellbook)
						.OrderBy(item => {
							SlayerName slayer = SlayerName.None;
							if (item is BaseWeapon) slayer = ((BaseWeapon)item).FirstSlayer ;
							else if (item is Spellbook) slayer = ((Spellbook)item).FirstSlayer;
							return slayer == SlayerName.None ? string.Empty : SlayerGroup.GetName(slayer);
						});
				}));
				TargetCommands.Register(new OrderByCommand("OrderBy-Weight", "Orders the items by their Weight (Desc)", (items) => items.OrderByDescending(item => 0 < item.TotalWeight ? item.TotalWeight : item.PileWeight)));
			}

			public override void Execute(CommandEventArgs e, object obj)
			{
				if (obj is Container)
				{
					var container = (Container)obj;
					if (!ItemUtilities.HasItemOwnershipRights(e.Mobile, container, true)) return;

					int horizontalSpace;
					horizontalSpace = 0 < e.Arguments.Length && int.TryParse(e.Arguments[0], out horizontalSpace) ? horizontalSpace : 20;

					int verticalSpace;
					verticalSpace = 1 < e.Arguments.Length && int.TryParse(e.Arguments[1], out verticalSpace) ? verticalSpace : 20;

					var sortedItems = m_Sort(container.Items);
					ItemUtilities.SortItems(container, sortedItems, horizontalSpace, verticalSpace);
				}
				else
				{
					e.Mobile.SendMessage("That is not a container.");
				}
			}
		}
	}
}