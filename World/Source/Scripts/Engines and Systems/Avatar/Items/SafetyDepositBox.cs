using Server.Items;
using Server.Network;

namespace Server.Engines.Avatar
{
	public class SafetyDepositBox : Container
	{
		private bool m_Open;

		public SafetyDepositBox(Serial serial) : base(serial)
		{
		}

		public SafetyDepositBox(Mobile owner) : base(0xE7C)
		{
			Name = "safety deposit box";
			Owner = owner;
			GumpID = 0x4A;
			MaxItems = 1;
		}

		public override int DefaultMaxWeight { get { return 0; } }
		public override bool IsVirtualItem { get { return true; } }
		public bool Opened { get { return m_Open; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Owner { get; set; }

		public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
		{
			return true;
		}

		public void Close()
		{
			m_Open = false;
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			Owner = reader.ReadMobile();
			m_Open = reader.ReadBool();

			if (Owner == null)
				Delete();
		}

		public override bool IsAccessibleTo(Mobile check)
		{
			if ((check == Owner && m_Open) || IsBankOpen(check) || check.AccessLevel >= AccessLevel.GameMaster)
				return base.IsAccessibleTo(check);
			else
				return false;
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!IsBankOpen(from)) return;

			if (Owner != null)
			{
				m_Open = true;
				DisplayTo(Owner);
			}
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if ((from == Owner && m_Open) || IsBankOpen(from) || from.AccessLevel >= AccessLevel.GameMaster)
				return base.OnDragDrop(from, dropped);
			else
				return false;
		}

		public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
		{
			if ((from == Owner && m_Open) || IsBankOpen(from) || from.AccessLevel >= AccessLevel.GameMaster)
				return base.OnDragDropInto(from, item, p);
			else
				return false;
		}

		public override void OnSingleClick(Mobile from)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			writer.Write(Owner);
			writer.Write(m_Open);
		}

		private bool IsBankOpen(Mobile from)
		{
			var bank = Owner.FindBankNoCreate();
			return bank != null && bank.Opened || AccessLevel.Player < from.AccessLevel;
		}
	}
}