using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Multis;
using Server.Gumps;
using Server.Regions;

namespace Server.Misc
{
	public class ShantyTarget : Target
	{
		private Mobile m_From;
		private int m_SelectedID;
		private int m_Price;
		private string m_Title;
		private ShantyTools m_ShantyTools;
		private BaseHouse m_House;
		private string m_Category;
		private int m_Page;

		public ShantyTarget(ShantyTools tools, Mobile from, int itemID, int price, string title, string category, int page): base(-1, true, TargetFlags.None)
		{
			m_ShantyTools = tools;
			m_From = from;
			m_SelectedID = itemID;
			m_Price = price;
			m_Title = title;
			m_Category = category;
			m_Page = page;
			CheckLOS = false;
			m_ShantyTools.Category = category;
			m_ShantyTools.Page = page;
		}

		protected override void OnTarget(Mobile from, object targeted)
		{
			IPoint3D t = targeted as IPoint3D;
			if (t == null)
				return;
			
			Point3D loc = new Point3D(t);
			if ( t is StaticTarget )
			{
				loc.Z -= TileData.ItemTable[((StaticTarget)t).ItemID & 0x3FFF].CalcHeight;
			}
			else if ( targeted is Item )
			{
				Item item = (Item)targeted;
				ItemData id = item.ItemData;
				int checkZ = item.Z;
				int checkTop = checkZ + id.Height;
				if ( id.Foliage ){ loc.Z = checkZ; }
				else { loc.Z = checkTop; }
			}

			if ( !(Region.Find( loc, from.Map ) is HouseRegion) )
			{
				m_From.SendMessage("You can only place that in your home!");
				GumpUp();
				return;
			}

			if (ValidatePlacement(loc))
				EndPlace(loc);
			else
				GumpUp();
		}

		public bool ValidatePlacement(Point3D loc)
		{
			Map map = m_From.Map;
			if (map == null)
				return false;

			bool regionCheck = false;

			Region reg = Region.Find( loc, map );
			m_House = BaseHouse.FindHouseAt(m_From.Location, map, 1);

			if ( reg is HouseRegion )
				regionCheck = true;

			if ( m_House == null || !m_House.IsOwner(m_From) )
			{
				m_From.SendMessage("You must be standing in your house to place this!");
				return false;
			}
			else if ( !regionCheck )
			{
				m_From.SendMessage("You can only place that in your home!");
				return false;
			}

			return true;
		}

		public void EndPlace(Point3D loc)
		{
			var cont = m_From.Backpack;
			var Paid = cont != null && cont.ConsumeTotal(typeof(Gold), m_Price);
			if (!Paid) // Fallback to Bank
			{
				Paid = Banker.Withdraw(m_From, m_Price);
			}

			if (Paid)
			{
				Remodeling.EndPlacement( m_SelectedID, m_From, m_Price, m_Title, m_House, loc );
				m_From.Target = new ShantyTarget(m_ShantyTools, m_From, m_SelectedID, m_Price, m_Title, m_Category, m_Page);
				m_From.PlaySound( Utility.RandomList( 0x13E, 0x23D, 0x125, 0x126, 0x55, 0x541 ) );
				GumpUp();
			}
			else
			{
				m_From.SendMessage("You do not have enough gold for that");
				GumpUp();
			}
		}

		public void GumpUp()
		{
			m_From.CloseGump( typeof( ShantyGump ) );
			m_From.SendGump(new ShantyGump(m_From, m_ShantyTools, m_Category, m_Page, m_SelectedID, m_Price, m_Title));
		}
	}
}