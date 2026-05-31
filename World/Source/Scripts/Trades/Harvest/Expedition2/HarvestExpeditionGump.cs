using Server;
using Server.Gumps;
using Server.Network;

namespace Harvest.Expedition
{
    public class HarvestExpeditionGump : Gump
    {
        private enum _Responses
        {
            Close = 0,

            // Fall through value
            // ExpeditionBaseValue = 100,
        }

        private Mobile m_From;
        private HarvestExpeditionNpc m_Npc;

        public HarvestExpeditionGump(Mobile from, HarvestExpeditionNpc npc) : base(50, 200)
        {
            from.CloseGump(typeof(HarvestExpeditionGump));

            m_From = from;
            m_Npc = npc;

            AddBackground(25, 10, 530, 180, 0x1453);

            AddImageTiled(35, 20, 510, 160, 0xA40);
            AddAlphaRegion(35, 20, 510, 160);

            // AddButton(390, 24, 0x15E1, 0x15E5, (int)_Responses.OpenBackpack, GumpButtonType.Reply, 0);
            // AddLabel(408, 21, 0x480, "Open Backpack");

            // AddButton(390, 44, 0x15E1, 0x15E5, 2, GumpButtonType.Reply, 0);
            // AddHtmlLocalized(408, 41, 120, 20, 1019069, 0x7FFF, false, false); // Customize

            // AddButton(390, 64, 0x15E1, 0x15E5, 3, GumpButtonType.Reply, 0);
            // AddHtmlLocalized(408, 61, 120, 20, 1062434, 0x7FFF, false, false); // Rename Shop

            // AddButton(390, 84, 0x15E1, 0x15E5, 4, GumpButtonType.Reply, 0);
            // AddHtmlLocalized(408, 81, 120, 20, 3006217, 0x7FFF, false, false); // Rename Vendor

            // AddButton(390, 104, 0x15E1, 0x15E5, (int)_Responses.OpenPaperdoll, GumpButtonType.Reply, 0);
            // AddLabel(408, 101, 0x480, "Open Paperdoll");

            // AddButton(390, 124, 0x15E1, 0x15E5, 6, GumpButtonType.Reply, 0);
            // AddLabel(408, 121, 0x480, "Collect Gold");

            // AddButton(390, 144, 0x15E1, 0x15E5, (int)_Responses.DimissGatherer, GumpButtonType.Reply, 0);
            // AddLabel(408, 141, 0x480, "Dismiss Gatherer");

            // AddButton(390, 162, 0x15E1, 0x15E5, 0, GumpButtonType.Reply, 0);
            // AddHtmlLocalized(408, 161, 120, 20, 1011012, 0x7FFF, false, false); // CANCEL

            AddHarvestSystemExpedition(SkillName.Mining);
        }

        private void AddHarvestSystemExpedition(SkillName skillName)
        {
            // AddButton(390, 144, 0x15E1, 0x15E5, (int)_Responses.ExpeditionBaseValue + (int)skillName, GumpButtonType.Reply, 0);
            // AddLabel(408, 141, 0x480, m_Npc.Skills[skillName].Name);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            //if (info.ButtonID == 1 || info.ButtonID == 2) // See goods or Customize
            //    m_Vendor.CheckTeleport(from);

            //if (!m_Vendor.CanInteractWith(from, true))
            //    return;

            var response = (_Responses)info.ButtonID;
            if (response == _Responses.Close) return;

            switch (response)
            {
                default:
                    {

                        break;
                    }
            }
        }
    }
}