using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Avatar
{
	public class AvatarBook : Item
	{
		[Constructable]
		public AvatarBook() : base(0x2147)
		{
			Name = "The Avatar's Ascent";
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick(Mobile from)
		{
			var player = from as PlayerMobile;
			if (player == null) return;

			if (!from.InRange(this.GetWorldLocation(), 2))
			{
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
				return;
			}

			if (!player.Avatar.Active)
			{
				from.SendMessage("You must be an Avatar to use this book.");
				return;
			}

			player.SendGump(new AvatarShopGump(player));
		}

		public AvatarBook(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}