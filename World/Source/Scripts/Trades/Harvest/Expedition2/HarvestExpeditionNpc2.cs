// using Server;
// using Server.Engines.Harvest;
// using Server.Items;
// using Server.Mobiles;
// using Server.Multis;
// using System;
// using System.Collections.Generic;

// namespace Harvest.Expedition
// {
//     public class HarvestExpeditionNpc2 : Mobile
//     {
//         private class ExpeditionNpcBackpack : Backpack
//         {
//             public override int DefaultMaxWeight { get { return 0; } }


//             public ExpeditionNpcBackpack()
//             {
//             }

//             public ExpeditionNpcBackpack(Serial serial) : base(serial)
//             {
//             }

//             public override void Serialize(GenericWriter writer)
//             {
//                 base.Serialize(writer);

//                 writer.Write((int)0); //version
//             }

//             public override void Deserialize(GenericReader reader)
//             {
//                 base.Deserialize(reader);

//                 int version = reader.ReadInt();
//             }
//         }

//         private int m_DurationHours;
//         private DateTime m_ExpeditionStopTime;
//         private HarvestExpeditionTimer m_ExpeditionTimer;
//         private SkillName m_ExpeditionType;
//         private BaseHouse m_House;
//         private Mobile m_Owner;

//         public HarvestExpeditionNpc(Mobile owner, BaseHouse house)
//         {
//             m_Owner = owner;
//             m_House = house;

//             InitBody();
//             InitOutfit();

//             Container pack = new ExpeditionNpcBackpack();
//             pack.Movable = false;
//             AddItem(pack);
//         }

//         public HarvestExpeditionNpc(Serial serial) : base(serial)
//         {
//         }

//         public override void OnDoubleClick(Mobile from)
//         {
//             if (from == m_Owner)
//             {
//                 var gump = new HarvestExpeditionGump(from, this);
//                 from.SendGump(gump);

//                 return;
//             }

//             base.OnDoubleClick(from);
//         }

//         public void StopTimer()
//         {
//             if (m_ExpeditionTimer == null) return;

//             m_ExpeditionTimer.Stop();
//             m_ExpeditionTimer = null;
//         }

//         protected List<Item> GetItems()
//         {
//             List<Item> list = new List<Item>();

//             foreach (Item item in Items)
//                 if (item.Movable && item != Backpack && item.Layer != Layer.Hair && item.Layer != Layer.FacialHair)
//                     list.Add(item);

//             if (Backpack != null)
//                 list.AddRange(Backpack.Items);

//             return list;
//         }

//         public void Dismiss(Mobile from)
//         {
//             Container pack = Backpack;

//             if (pack != null && pack.Items.Count > 0)
//             {
//                 SayTo(from, 1038325); // You cannot dismiss me while I am holding your goods.
//                 return;
//             }

//             Destroy(true);
//         }

//         public virtual void Destroy(bool toBackpack)
//         {
//             // Return();

//             /* Possible cases regarding item return:
// 			 * 
// 			 * 1. No item must be returned
// 			 *       -> do nothing.
// 			 * 2. ( toBackpack is false OR the vendor is in the internal map ) AND the vendor is associated with a AOS house
// 			 *       -> put the items into the moving crate or a vendor inventory,
// 			 *          depending on whether the vendor owner is also the house owner.
// 			 * 3. ( toBackpack is true OR the vendor isn't associated with any AOS house ) AND the vendor isn't in the internal map
// 			 *       -> put the items into a backpack.
// 			 * 4. The vendor isn't associated with any house AND it's in the internal map
// 			 *       -> do nothing (we can't do anything).
// 			 */

//             List<Item> list = GetItems();

//             // if ( list.Count > 0 || HoldGold > 0 ) // No case 1
//             if (list.Count > 0) // No case 1
//             {
//                 if ((!toBackpack || Map == Map.Internal) && m_House != null) // Case 2
//                 {
//                     if (m_House.IsOwner(m_Owner)) // Move to moving crate
//                     {
//                         if (m_House.MovingCrate == null)
//                             m_House.MovingCrate = new MovingCrate(m_House);

//                         // if ( HoldGold > 0 )
//                         // 	Banker.Deposit( m_House.MovingCrate, HoldGold );

//                         foreach (Item item in list)
//                         {
//                             m_House.MovingCrate.DropItem(item);
//                         }
//                     }
//                     else // Move to vendor inventory
//                     {
//                         const string ShopName = "$None"; // TODO: Vendor name?
//                         VendorInventory inventory = new VendorInventory(m_House, m_Owner, Name, ShopName);
//                         // inventory.Gold = HoldGold;

//                         foreach (Item item in list)
//                         {
//                             inventory.AddItem(item);
//                         }

//                         m_House.VendorInventories.Add(inventory);
//                     }
//                 }
//                 else if ((toBackpack || m_House == null) && Map != Map.Internal) // Case 3 - Move to backpack
//                 {
//                     Container backpack = new Backpack();

//                     // if ( HoldGold > 0 )
//                     // 	Banker.Deposit( backpack, HoldGold );

//                     foreach (Item item in list)
//                     {
//                         backpack.DropItem(item);
//                     }

//                     backpack.MoveToWorld(this.Location, this.Map);
//                 }
//             }

//             Delete();
//         }

//         private void InitBody()
//         {
//             Hue = Utility.RandomSkinColor();
//             SpeechHue = Utility.RandomTalkHue();

//             if (Female = Utility.RandomBool())
//             {
//                 Body = 0x191;
//                 Name = NameList.RandomName("female");
//             }
//             else
//             {
//                 Body = 0x190;
//                 Name = NameList.RandomName("male");
//             }
//         }

//         private void InitOutfit()
//         {
//             AddItem(new FancyShirt(Utility.RandomNeutralHue()) { Layer = Layer.InnerTorso });
//             AddItem(new LongPants(Utility.RandomNeutralHue()));
//             AddItem(new BodySash(Utility.RandomNeutralHue()));
//             AddItem(new Boots(Utility.RandomNeutralHue()));
//             AddItem(new Cloak(Utility.RandomNeutralHue()));

//             Utility.AssignRandomHair(this);
//         }

//         private void Reset()
//         {
//             StopTimer();
//             m_ExpeditionStopTime = DateTime.MinValue;

//             // Move back to their original location
//             if (Map != Map.Internal) Map = m_House.Map; // TODO: This ... ah crap no map by default
//         }

//         public override void Serialize(GenericWriter writer)
//         {
//             base.Serialize(writer);

//             writer.Write((int)0); //version
//             writer.Write(m_Owner);
//             writer.Write(m_House);
//             writer.Write((int)m_ExpeditionType);
//             writer.Write(m_DurationHours);
//             writer.Write(m_ExpeditionStopTime);
//         }

//         public override void Deserialize(GenericReader reader)
//         {
//             base.Deserialize(reader);

//             int version = reader.ReadInt();
//             m_Owner = reader.ReadMobile();
//             m_House = (BaseHouse)reader.ReadItem();
//             m_ExpeditionType = (SkillName)reader.ReadInt();
//             m_DurationHours = reader.ReadInt();
//             m_ExpeditionStopTime = reader.ReadDateTime();

//             // If it was running, restart the expedition
//             if (m_ExpeditionStopTime != DateTime.MinValue)
//             {
//                 m_ExpeditionTimer = new HarvestExpeditionTimer(this, m_ExpeditionStopTime, TimeSpan.FromSeconds(30), HarvestExpeditionTimer.DefaultInterval);
//             }
//             else
//             {
//                 Reset();
//             }
//         }
//     }
// }