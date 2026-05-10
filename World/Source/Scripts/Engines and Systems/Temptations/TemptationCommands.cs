using Server.Commands;
using Server.Commands.Generic;
using Server.Mobiles;

namespace Server.Temptation
{
	public class TemptationsCommand : BaseCommand
	{
		public TemptationsCommand()
		{
			AccessLevel = AccessLevel.Player;
			Supports = CommandSupport.AllMobiles;
			Commands = new string[] { "Temptations" };
			ObjectTypes = ObjectTypes.Mobiles;
			Usage = "Temptations";
			Description = "Gets the Temptations for the targeted character.";
		}

		public override void Execute(CommandEventArgs e, object obj)
		{
			var from = (PlayerMobile)e.Mobile;
			var pm = obj as PlayerMobile;
			if (pm == null)
			{
				LogFailure("That is not a player.");
				return;
			}

			if (from.Avatar.Active && !from.Avatar.UnlockTemptations)
			{
				from.SendMessage("An Avatar must unlock the Temptations system before using this command.");
				return;
			}

			from.SendGump(new TemptationGump(pm, new PlayerContext(from.Temptations), from));
		}
	}
}