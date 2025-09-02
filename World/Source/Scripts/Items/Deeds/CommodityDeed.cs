using System;
using Server.Items;
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
			Hue = 0x47;
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
				commodity.Internalize();
				Hue = 0x592; // Change color when filled
				InvalidateProperties();
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			BankBox box = from.FindBankNoCreate();
			
			if (m_Commodity != null)
			{
				if ( box != null && IsChildOf( box ) )
				{
					RedeemCommodity(from, box);
				}
				else
				{
                    from.SendLocalizedMessage(1047026); // That must be in your bank box to use it.
                }
			} else if ( box == null || !IsChildOf( box ) ) 
			{
					from.SendLocalizedMessage( 1047026 ); // That must be in your bank box to use it.
			}
			else
			{
				from.SendMessage("Target the commodity you wish to deed.");
				from.Target = new CommodityTarget(this);
			}
		}

		private void RedeemCommodity(Mobile from, BankBox box)
		{
			if (m_Commodity == null || m_Commodity.Deleted)
			{
				from.SendLocalizedMessage(500461); // You destroy the item.
				Delete();
				return;
			}

			if (!box.TryDropItem(from, m_Commodity, false))
			{
				from.SendLocalizedMessage(1080017); // That container cannot hold more items.
                return;
			}

			m_Commodity = null;
			from.SendLocalizedMessage(1047031); //The commodity has been redeemed.
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
				from.SendLocalizedMessage(1047027); // That is not a commodity the bankers will fill a commodity deed with.
				return;
			}

			ICommodity commodityItem = item as ICommodity;
			if (commodityItem == null || !commodityItem.IsCommodity)
			{
				from.SendLocalizedMessage(1047027); // That is not a commodity the bankers will fill a commodity deed with.
				return;
			}

			BankBox box = from.FindBankNoCreate();
			if (!item.IsChildOf(box))
			{
				from.SendLocalizedMessage(1047026); // That must be in your bank box to use it.
				return;
			}

			m_Deed.SetCommodity(item);

			from.SendLocalizedMessage(1047030); //The commodity has been filled.
        }
	}
}