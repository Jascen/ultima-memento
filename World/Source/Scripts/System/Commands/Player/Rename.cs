using Server.Commands.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Utilities;

namespace Server.Commands
{
	public class RenameCommand : BaseCommand
	{
		public RenameCommand()
		{
			AccessLevel = AccessLevel.Player;
			Supports = CommandSupport.AllItems;
			Commands = new string[] { "Rename" };
			ObjectTypes = ObjectTypes.Both;
			Usage = "Rename <new name>";
			Description = "Renames a targeted container.";
		}

		public static void Initialize()
		{
			TargetCommands.Register(new RenameCommand());
		}

		public override void Execute(CommandEventArgs e, object obj)
		{
			if (e.Mobile == null) return;

			var newName = e.ArgString;
			if (string.IsNullOrEmpty(newName))
			{
				e.Mobile.SendMessage("Usage: [Rename <new name>");
				return;
			}

			const int MAX_NAME_LENGTH = 64;
			if (MAX_NAME_LENGTH < newName.Length)
			{
				e.Mobile.SendMessage("Usage: [Rename <new name>");
				return;
			}

			var container = obj as Container;
			if (container == null)
			{
				e.Mobile.SendMessage("That is not a container.");
				return;
			}

			if (container.Deleted) return;
			if (container is MovingBox || container is DungeoneerCrate || container is TrashBarrel || container is TrashChest) return;

			if (!ItemUtilities.HasItemOwnershipRights(e.Mobile as PlayerMobile, container))
			{
				e.Mobile.SendMessage("You may only rename containers that belong to you.");
				return;
			}

			container.Name = newName;
		}
	}
}