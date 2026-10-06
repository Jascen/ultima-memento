using System;
using System.Collections.Generic;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Misc;

namespace Server.Gumps
{
    public class LawnGump : Gump
    {
        int m_SelectedID;
        int m_ItemPrice = 0;
        string m_ItemTitle = "";
        int m_PlayerGold = 0;
        LawnTools m_LawnTools;
        string[] m_Categories;
        LawnGumpCategory m_CurrentCategory;
        int m_CurrentPage;

        public LawnGump(Mobile owner, LawnTools tools, string currentCategory, int currentPage, int itemID, int price, string title): base(-40, 50)
        {
			string color = "#87d287";
            m_SelectedID = itemID;
            m_ItemPrice = price;
            m_ItemTitle = title;
            m_LawnTools = tools;
            m_CurrentPage = currentPage;
			int locMod = 90;
            if (currentCategory != null && LawnRegistry.Categories.ContainsKey(currentCategory))
            {
                m_CurrentCategory = LawnRegistry.Categories[currentCategory];
            }
            m_LawnTools.Category = currentCategory;
            m_LawnTools.Page = currentPage;
			m_PlayerGold = ComputeGold(owner);

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

			if ( currentPage == 999998 )
			{
				string text = "";
				text += "These tools, with the additional use of homeowner tools, can be used to decorate the area around your house. Each item placed will cost a particular amount of gold that will be noted on the bottom left. The gold amount in your bank will be noted at the bottom as well. To begin, make sure you are always standing in your home because this is the only place where you can add items or maneuver them using the homeowner tools. Select a category on the right side using the small boxes, and the choices will appear on the left. You can page through the list with the arrow buttons provided. Select an item by using the small boxes to the left of the name and an image of the item will appear in the center so you know what it looks like.";
				text += "<br><br>";
				text += "Once an item is selected, a BUY option will be available. If you press the OK button a target cursor will appear and you can select where the item is placed. If you successfully place the item, the target cursor will remain so you can quickly place another similar item. If you do not want to place another similar item, press the ESC key to clear the target cursor. You can also select a different item to place, where then selecting the BUY button will arm your target cursor to place that item instead.";
				text += "<br><br>";
				text += "There are a few rules to placing an item. You cannot place these items within your house. You can only place items no more than 15 tiles away from your home's border. With that rule, it also means you must be standing as close to the spot as you can to place the item the furthest you can. For example: if you want to place an item as far as you can on the northwest end, you should stand in your home's closest northwest end. You may have issues placing items on the very east or south boundary of your home. If you do, simply add the item 1 tile over and move it next to your home using the homeowner tools.";
				text += "<br><br>";
				text += "Once items are placed, you can move them around using the homeowner tools. The only thing homeowner tools can do with lawn items is move them up, down, north, south, east, or west. Items cannot be moved below the ground or higher than 15 units off the ground. Like placing items, it is best to be at the closest spot in your home while moving them around. Some items may be hard to select like the magical sparkles. Pressing the CONTROL and SHIFT key at the same time will display the name of the item and then you can select that. Items may be placed directly on the surface of the world, and will not appear on top of other items that you may select to place upon. Instead, use the homeowner tools to raise the item to the elevation you want.";
				text += "<br><br>";
				text += "To sell an item, the common way is to double click it and you will be refunded for the entire amount as it is placed into your bank box. There are two types of items that cannot be sold by double clicking them, and that is the gates and stairs. Single click these items and choose the SELL option when the menu appears. The reason for this difference is that these two items have special behaviors when double clicked. Double clicking a stair piece will rotate it around, while double clicking a gate will open it. If you single click a gate, you can set the security to have it locked or unlocked. Only friends, owners, co-owners, and guild members can open a locked gate. Only owners and co-owners can sell the items as well as turn stair pieces or set security on gates.";
				text += "<br><br>";
				text += "When you demolish a home, the items surrounding the house will be automatically removed and a check for the total value will be placed in your bank box. If your house deteriorates by any other means, the items will be removed shortly after and the total value will be placed in the owner's bank box.";
				text += "<br><br>";
				text += "These lawn tools are a great way to make the atmosphere around your home project your personal style. Keep in mind that the lawn around your home is not a safe haven as your house is. Although you could place a secure fence, it can still be potentially infiltrated by others. You can slay others in your yard as well as be slain.";
				text += "<br><br>";

				AddImage(0+locMod-2, -2, 9588, Server.Misc.PlayerSettings.GetGumpHue( owner ));
				AddHtml( 62+locMod, 13, 300, 20, @"<BODY><BASEFONT Color=" + color + ">LAWN TOOLS - Help</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(595+locMod, 14, 4017, 4017, 999997, GumpButtonType.Reply, 0);
				AddHtml( 18+locMod, 84, 605, 309, @"<BODY><BASEFONT Color=" + color + ">" + text + "</BASEFONT></BODY>", (bool)false, (bool)true);
			}
			else if ( currentPage == 999995 )
			{
				AddImage(0+locMod-2, -2, 9588, Server.Misc.PlayerSettings.GetGumpHue( owner ));
				AddHtml( 62+locMod, 13, 300, 20, @"<BODY><BASEFONT Color=" + color + ">LAWN TOOLS - Remove</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(595+locMod, 14, 4017, 4017, 999997, GumpButtonType.Reply, 0);

				AddHtml( 18+locMod, 84, 605, 75, @"<BODY><BASEFONT Color=" + color + ">If you want to remove all lawn decorations, then press the Remove All button below. The gold will be refunded to your bank box. If you want to cancel this request, press the button on the upper right.</BASEFONT></BODY>", (bool)false, (bool)false);
				
				AddHtml( 54+locMod, 162, 300, 20, @"<BODY><BASEFONT Color=" + color + ">Remove Target</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(18+locMod, 160, 4023, 4023, 999993, GumpButtonType.Reply, 0);

				TextDefinition.AddHtmlText(this, 54+locMod, 202, 300, 20, "Remove All", HtmlColors.RED);
				AddButton(18+locMod, 200, 4023, 4023, 999994, GumpButtonType.Reply, 0);
			}
			else
			{
				AddImage(0+locMod-2, -2, 9588, Server.Misc.PlayerSettings.GetGumpHue( owner ));

				if (m_CurrentCategory != null && m_SelectedID > 0)
				{
					Remodeling.ItemLayout( m_SelectedID, m_ItemTitle, this );
				}

				//Title & Help
				string header = "LAWN TOOLS";
				if ( currentCategory != null && currentCategory != "" ){ header = header + " - " + currentCategory; }
				AddHtml( 62+locMod, 13, 300, 20, @"<BODY><BASEFONT Color=" + color + ">" + header + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(63+locMod, 40, 3610, 3610, 999999, GumpButtonType.Reply, 0);
				AddButton(128+locMod, 40, 4029, 4029, 999996, GumpButtonType.Reply, 0);

				//Item Cost
				AddItem(-1+locMod, 376, 3823);
				if ( m_ItemPrice > 0 )
				{
					AddHtml( 39+locMod, 378, 82, 20, @"<BODY><BASEFONT Color=" + color + ">" + String.Format("{0:0,0}", m_ItemPrice) + "</BASEFONT></BODY>", (bool)false, (bool)false);
				}

				//Bank Gold
				AddItem(248+locMod, 371, 5150);
				if ( m_PlayerGold > 0 )
				{
					AddHtml( 300+locMod, 378, 134, 20, @"<BODY><BASEFONT Color=" + color + ">" + String.Format("{0:0,0} Gold", m_PlayerGold) + "</BASEFONT></BODY>", (bool)false, (bool)false);
				}

				if (m_CurrentCategory != null && m_SelectedID > 0)
				{
					//Buy Button
					AddButton(319+locMod, 13, 4023, 4023, (int)Buttons.Place, GumpButtonType.Reply, 0);
					AddHtml( 357+locMod, 13, 60, 20, @"<BODY><BASEFONT Color=" + color + ">Buy</BASEFONT></BODY>", (bool)false, (bool)false);
				}

				//Categories
				int catMod = 50;
				AddHtml( 490+locMod+catMod, 13, 119, 20, @"<BODY><BASEFONT Color=" + color + ">Categories</BASEFONT></BODY>", (bool)false, (bool)false);
				int categoryID = 0;
				m_Categories = new string[LawnRegistry.Categories.Keys.Count];
				foreach (string categoryName in LawnRegistry.Categories.Keys)
				{
					int hue = 1477;
					if (categoryName == currentCategory)
					{
						hue = 1671;
						AddButton(577+catMod, 52 + (25 * categoryID), 2448, 2448, 80851 + categoryID, GumpButtonType.Reply, 0);
					}
					else
					{
						hue = 1477;
						AddButton(577+catMod, 52 + (25 * categoryID), 2447, 2447, 80851 + categoryID, GumpButtonType.Reply, 0);
					}
					AddLabel(590+catMod, 49 + (25 * categoryID), hue, categoryName);
					m_Categories[categoryID] = categoryName;
					categoryID++;
				}

				if (m_CurrentCategory != null)
				{
					int i = 0;
					foreach (LawnGumpEntry entry in m_CurrentCategory.Pages[m_CurrentPage].Values)
					{
						entry.AppendToGump(this, 107 + (i >= 12 ? 143 : 0), 95 + (i >= 12 ? 20 * (i - 12) : 20 * i), m_SelectedID);
						i++;
					}
				}
				else
				{
					AddHtml(105, 80, 510, 290, "<BODY><BASEFONT Color=" + color + ">With these lawn tools you can add certain terrain and items around the outside of your home. Choose a category to the right to start browsing the list of things you can decorate with. Each of them have a price that will be deducted from your bank box. For additional information, you can access the HELP screen above.</BASEFONT></BODY>", false, false);
				}

				if (m_CurrentCategory != null && m_CurrentCategory.Pages.Count > m_CurrentPage + 1)
				{
					AddButton(172, 74, 4005, 4005, (int)Buttons.Next, GumpButtonType.Reply, 0);
				}

				if (m_CurrentCategory != null && m_CurrentPage > 0)
				{
					AddButton(112, 74, 4014, 4014, (int)Buttons.Prev, GumpButtonType.Reply, 0);
				}
			}
			AddItem(7+locMod, 7, 25574);
        }

        public enum Buttons
        {
            Exit,
            Place = -2,
            Next = -3,
            Prev = -4,
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;
			from.SendSound( 0x4A ); 
			from.CloseGump( typeof( LawnGump ) );
            if (info.ButtonID == 0)
            {
                return;
            }
			else if (info.ButtonID == 999999)
			{
				from.SendGump(new LawnGump(from, m_LawnTools, "", 999998, m_SelectedID, m_ItemPrice, m_ItemTitle));
			}
			else if (info.ButtonID == 999997)
			{
                LawnTarget yt = new LawnTarget(m_LawnTools, from, 0, 0, "", "", 0);
				yt.GumpUp();
			}
			else if (info.ButtonID == 999996)
			{
				from.SendGump(new LawnGump(from, m_LawnTools, "", 999995, m_SelectedID, m_ItemPrice, m_ItemTitle));
			}
			else if (info.ButtonID == 999993)
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );
				if ( house == null || !house.IsOwner(from) )
					from.SendLocalizedMessage( 502092 ); // You must be in your house to do this.
				else
				{
					from.SendMessage("Please target the item to remove.");
					from.Target = new LawnRemoveTarget( house );
				}
			}
			else if (info.ButtonID == 999994)
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );
				if ( house != null )
				{
					if ( house.IsOwner(from) )
						LawnSystem.RemoveLawns( house, from );
					else
						from.SendLocalizedMessage( 502092 ); // You must be in your house to do this.
				}
				else
				{
					from.SendLocalizedMessage( 502092 ); // You must be in your house to do this.
				}
                LawnTarget yt = new LawnTarget(m_LawnTools, from, 0, 0, "", "", 0);
				yt.GumpUp();
			}
            else if (info.ButtonID == (int)Buttons.Next)
            {
                if (m_CurrentCategory != null && LawnRegistry.Categories[m_CurrentCategory.Name].Pages.Count > m_CurrentPage + 1)
                {
                    from.SendGump(new LawnGump(from, m_LawnTools, m_CurrentCategory.Name, m_CurrentPage + 1, m_SelectedID, m_ItemPrice, m_ItemTitle));
                }
                else
                {
                    from.SendGump(new LawnGump(from, m_LawnTools, "", 0, m_SelectedID, m_ItemPrice, m_ItemTitle));
                }
            }
            else if (info.ButtonID == (int)Buttons.Prev)
            {
                if (m_CurrentCategory != null && m_CurrentPage > 0)
                {
                    from.SendGump(new LawnGump(from, m_LawnTools, m_CurrentCategory.Name, m_CurrentPage - 1, m_SelectedID, m_ItemPrice, m_ItemTitle));
                }
                else
                {
                    from.SendGump(new LawnGump(from, m_LawnTools, "", 0, m_SelectedID, m_ItemPrice, m_ItemTitle));
                }
            }
            else if (info.ButtonID == (int)Buttons.Place)
            {
                if (m_SelectedID > 0)
                {
                    from.SendMessage("Please choose where to place the item");
                    from.Target = new LawnTarget(m_LawnTools, from, m_SelectedID, m_ItemPrice, m_ItemTitle, m_CurrentCategory.Name, m_CurrentPage);
                }
            }
            else if (info.ButtonID >= 80851 && info.ButtonID <= 80865)
            {
                //Change categories
                if (m_Categories != null && m_Categories.Length > info.ButtonID - 80851)
                {
                    if (m_CurrentCategory != null)
                    {
                        from.SendGump(new LawnGump(from, m_LawnTools,
                                                   m_Categories[info.ButtonID - 80851] == m_CurrentCategory.Name ? "" : m_Categories[info.ButtonID - 80851], 
                                                   0, m_SelectedID, m_ItemPrice, m_ItemTitle));                        
                    }
                    else
                    {
                        from.SendGump(new LawnGump(from, m_LawnTools, m_Categories[info.ButtonID - 80851], 0, m_SelectedID, m_ItemPrice, m_ItemTitle));
                    }
                }
                else
                {
                    from.SendGump(new LawnGump(from, m_LawnTools, "", 0, m_SelectedID, m_ItemPrice, m_ItemTitle));
                }
            }
            else
            {
                m_SelectedID = info.ButtonID;
                if (m_CurrentCategory != null)
                {
                    LawnGumpEntry entry = m_CurrentCategory.GetEntry(m_SelectedID);
                    if (entry != null)
                    {
                        m_ItemPrice = entry.Price;
                        m_ItemTitle = entry.Title;
                    }

		private int ComputeGold(Mobile from)
		{
			int amount = Banker.GetBalance(from);
			if (from.Backpack != null)
			{
				foreach (Gold gold in from.Backpack.FindItemsByType<Gold>(true))
				{
					amount += gold.Amount;
				}

			}

			return amount;
		}
    }
}