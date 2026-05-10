using Server.Commands.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Commands
{
	public class OrganizeCommand : BaseCommand
	{
		public static readonly List<Organizer> Organizers;

		static OrganizeCommand()
		{
			Organizers = new List<Organizer>
			{
				new Organizer(
					"Crafting",
					1185, // Bright Red
					item =>
					{
						return
							item is Bottle

							|| item is BlankScroll
							|| item is Beeswax

							// Lumberjacking
							|| item is BaseLog
							|| item is BaseWoodBoard
							|| item is BarkFragment

							// Metals / Mining
							|| item is BaseOre
							|| item is BaseIngot
							|| item is BaseGranite
							|| item is BaseBlocks
							|| item is BaseScales

							// Tailoring
							|| item is Cotton
							|| item is Wool
							|| item is Flax
							|| item is BaseFabric
							|| item is BaseLeather
							|| item is BaseHides
							|| item is BaseSkins
							|| item is BaseSkeletal

							|| item is BaseTool
							|| item is BaseHarvestTool
							|| item is Spade;
					}
				),
				new Organizer(
					"Casting",
					1083, // Ice Green
					item =>
					{
						return item is BaseReagent
							|| item is Spellbook
							|| item is RuneStone
							|| item is MagicRuneBag
							|| item.Catalog == Catalogs.Reagent;
					}
				),
				new Organizer(
					"Currency",
					2369, // Gold
					item =>
					{
						return
							item is Gold
							|| item is Crystals
							|| item is DDGemstones
							|| item is DDJewels
							|| item is DDGoldNuggets
							|| item is DDSilver
							|| item is DDCopper
							|| item is DDXormite;
					}
				),
				new Organizer(
					"Consumables",
					1195, // Ice blue
					item =>
					{
						return item is BasePotion
							|| item is SpellScroll
							|| item.Catalog == Catalogs.Potion;
					}
				),
				new Organizer(
					"Armor",
					1174, // Bright Gold
					item =>
					{
						return item is BaseArmor
							|| item is BaseQuiver;
					}
				),
				new Organizer(
					"Weapons",
					2197, // Retro Pink
					item =>
					{
						return item is BaseWeapon;
					}
				),
				new Organizer(
					"Clothing",
					1283, // Pink Blue
					item =>
					{
						return item is BaseClothing;
					}
				),
				new Organizer(
					"Jewelry",
					1161, // Blaze
					item =>
					{
						return item.Catalog == Catalogs.Jewelry
							|| item.Catalog == Catalogs.Gem;
					}
				),
				new Organizer(
					"Decorative",
					1952, // Bright Pink
					item =>
					{
						return item is IRelic
							|| item is IAddon
							|| item is BaseAddonDeed
							|| item is ColoredWallTorch;
					}
				),
				new Organizer(
					"Unidentified Items",
					1382, // Bright Yellow
					item =>
					{
						if (false == (item is NotIdentified)) return false;

						return true;
					}
				),

				// Keep this last
				new Organizer(
					"Trinkets & Offhands",
					1793, // Other Bright Red
					item =>
					{
						return item is BaseTrinket
							|| item is BaseInstrument;
					}
				),
			};

			// Check and warn for duplicate names
			foreach (var group in Organizers.GroupBy(o => o.ContainerName))
			{
				if (group.Count() <= 1) continue;

				Console.WriteLine("[Organize]: WARNING -- Multiple organizers have the name '{0}'", group.Key);
			}
		}

		public OrganizeCommand()
		{
			AccessLevel = AccessLevel.Player;
			Supports = CommandSupport.AllItems;
			Commands = new string[] { "Organize" };
			ObjectTypes = ObjectTypes.Both;
			Usage = "Organize [<Horizontal_Space> | NULL] [<Vertical_Space> | NULL]";
			Description = "Organizes a targeted container and all it's contents into separate bags.";
		}

		public static void Initialize()
		{
			TargetCommands.Register(new OrganizeCommand());
		}

		public override void Execute(CommandEventArgs e, object obj)
		{
			if (e.Mobile == null) return;

			if (obj is Container)
			{
				int horizontalSpace;
				horizontalSpace = 0 < e.Arguments.Length && int.TryParse(e.Arguments[0], out horizontalSpace) ? horizontalSpace : 20;

				int verticalSpace;
				verticalSpace = 1 < e.Arguments.Length && int.TryParse(e.Arguments[1], out verticalSpace) ? verticalSpace : 20;

				Organize(e.Mobile as PlayerMobile, (Container)obj, horizontalSpace, verticalSpace);
			}
			else
				e.Mobile.SendMessage("That is not a container.");
		}

		private void Organize(PlayerMobile from, Container target, int horizontalSpace, int verticalSpace)
		{
			if (from == null) return;
			if (target.Deleted) return;
			if (target is OrganizerContainer) return;
			if (!ItemUtilities.HasItemOwnershipRights(from, target, true)) return;

			if (!from.InRange(target.GetWorldLocation(), 3))
			{
				from.SendMessage("You will have to get closer to the container!");
				return;
			}

			if (target is ILockable && ((ILockable)target).Locked
				|| target is LockableContainer && ((LockableContainer)target).CheckLocked(from))
			{
				from.SendMessage("That container is locked.");
				return;
			}

			var existingOrganizerContainers = new List<OrganizerContainer>();
			var destinations = new Dictionary<string, OrganizerContainer>();

			foreach (var item in target.FindItemsByType(typeof(Item)))
			{
				if (item is OrganizerContainer)
				{
					var organizer = (OrganizerContainer)item;
					organizer.AcceptItems = true;

					existingOrganizerContainers.Add(organizer);
					if (!destinations.ContainsKey(item.Name)) destinations.Add(item.Name, organizer);
				}

				if (item is Container && false == (item is NotIdentified || item is BaseQuiver)) continue;
				if (!item.Movable) continue;
				if (item.IsLockedDown) continue;

				foreach (var organizer in Organizers)
				{
					if (!organizer.Test(item)) continue;
					if (item.Parent is NotIdentified) continue;
					if (item.Parent is LockableContainer && ((LockableContainer)item.Parent).Locked) continue;

					OrganizerContainer container;
					if (!destinations.TryGetValue(organizer.ContainerName, out container))
					{
						container = destinations[organizer.ContainerName] = new OrganizerContainer
						{
							Name = organizer.ContainerName,
							Hue = organizer.Hue
						};
					}

					if (!container.TryDropItem(from, item, true))
						container.AddItem(item);
					break;
				}
			}

			if (destinations.Values.Any(x => 0 < x.Items.Count))
			{
				// Sort alphabetically by container name
				var sortedItems = destinations.Values.OrderBy(container => container.Name);
				ItemUtilities.SortItems(target, sortedItems, horizontalSpace, verticalSpace);
			}

			foreach (var container in existingOrganizerContainers.Concat(destinations.Values))
			{
				container.AcceptItems = false;
				if (0 < container.Items.Count) continue;

				// Remove empty containers
				container.Delete();
			}
		}

		public class Organizer
		{
			public readonly string ContainerName;
			public readonly int Hue;
			public readonly Func<Item, bool> Test;

			public Organizer(string containerName, int hue, Func<Item, bool> test)
			{
				ContainerName = containerName;
				Hue = hue;
				Test = test;
			}
		}

		private class OrganizerContainer : BaseContainer, IValidate
		{
			public OrganizerContainer() : base(0xE75)
			{
				Weight = 0;
				MaxItems = 0;
				InfoText1 = "[Organizer]";
				AcceptItems = true;
			}

			public OrganizerContainer(Serial serial) : base(serial)
			{
			}

			public bool AcceptItems { get; set; }

			public override int MaxWeight
			{ get { return 0; } }

			public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
			{
				if (Items.Count < 1)
				{
					// Item vanishes
					Delete();
					from.SendMessage("The empty container vanishes from your hand.");

					return false;
				}

				return true;
			}

			public override void Deserialize(GenericReader reader)
			{
				// Never do anything. This is a pass-thru container
				base.Deserialize(reader);

				Visible = false; // Start hidden, it may be deleted
				ValidationQueue.Add(this);
			}

			public override bool DropToItem(Mobile from, Item target, Point3D p)
			{
				if (Parent != null && target != Parent)
				{
					if (from != null)
						from.SendMessage("This cannot be moved outside of the current container.");

					return false;
				}

				return base.DropToItem(from, target, p);
			}
			public override void Serialize(GenericWriter writer)
			{
				// Never do anything. This is a pass-thru container
				base.Serialize(writer);
			}

			public void Validate()
			{
				if (0 < Items.Count)
					Visible = true;
				else
					Delete();
			}
		}
	}
}