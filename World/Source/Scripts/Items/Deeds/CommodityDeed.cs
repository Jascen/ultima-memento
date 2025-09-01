using System;
using Server.Items.Abstractions;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class CommodityDeed : Item
	{
		private Item m_Commodity;

		[CommandProperty(AccessLevel.GameMaster)]
		public Item Commodity
		{
			get { return m_Commodity; }
			set { m_Commodity = value; InvalidateProperties(); }
		}

		public override string DefaultName
		{
			get 
			{ 
				if (m_Commodity != null)
				{
					string commodityName = m_Commodity.Name ?? m_Commodity.GetType().Name;
					return "a commodity deed for " + commodityName.ToLower();
				}
				return "a commodity deed"; 
			}
		}

		[Constructable]
		public CommodityDeed() : base(0x14F0)
		{
			Weight = 1.0;
			Hue = 71;
		}

		public CommodityDeed(Item commodity) : this()
		{
			SetCommodity(commodity);
		}

		public CommodityDeed(Serial serial) : base(serial)
		{
		}

		public void SetCommodity(Item commodity)
		{
			ICommodity commodityItem = commodity as ICommodity;
			if (commodityItem != null && commodityItem.IsCommodity)
			{
				m_Commodity = commodity;
				InvalidateProperties();
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
				return;
			}

			if (m_Commodity != null)
			{
				RedeemCommodity(from);
			}
			else
			{
				from.SendMessage("Target the commodity you wish to deed.");
				from.Target = new CommodityTarget(this);
			}
		}

		private void RedeemCommodity(Mobile from)
		{
			if (m_Commodity == null || m_Commodity.Deleted)
			{
				from.SendLocalizedMessage(500466); // You destroy the item.
				Delete();
				return;
			}

			if (!from.AddToBackpack(m_Commodity))
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
				return;
			}

			m_Commodity = null;
			Delete();
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (m_Commodity != null)
			{
				string commodityName = m_Commodity.Name ?? m_Commodity.GetType().Name;
				list.Add(1060658, "Amount\t{0}", m_Commodity.Amount);
				list.Add(1060659, "Type\t{0}", commodityName);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version

			writer.Write(m_Commodity);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			m_Commodity = reader.ReadItem();
		}
	}

	public class CommodityTarget : Target
	{
		private CommodityDeed m_Deed;

		public CommodityTarget(CommodityDeed deed) : base(12, false, TargetFlags.None)
		{
			m_Deed = deed;
		}

		protected override void OnTarget(Mobile from, object targeted)
		{
			if (m_Deed == null || m_Deed.Deleted)
				return;

			Item item = targeted as Item;
			if (item == null)
			{
				from.SendMessage("That is not a valid commodity.");
				return;
			}

			ICommodity commodityItem = item as ICommodity;
			if (commodityItem == null || !commodityItem.IsCommodity)
			{
				from.SendLocalizedMessage(1047027); // That is not a commodity the bankers will fill a commodity deed with.
				return;
			}

			if (!item.IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
				return;
			}

			m_Deed.SetCommodity(item);
			item.MoveToWorld(new Point3D(0, 0, 0), Map.Internal);

			from.SendMessage("You have filled the commodity deed.");
		}
	}
}