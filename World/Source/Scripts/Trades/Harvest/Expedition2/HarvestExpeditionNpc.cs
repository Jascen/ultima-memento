using Server;
using Server.ContextMenus;
using Server.Mobiles;
using System.Collections.Generic;

namespace Harvest.Expedition
{
    public class HarvestExpeditionNpc : BaseNPC
    {
        [Constructable]
        public HarvestExpeditionNpc()
        {
            Title = "the expeditionary";
        }

        public HarvestExpeditionNpc(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            list.Add(new MenuEntry(from, this));
        }

        public class MenuEntry : ContextMenuEntry
        {
            private readonly Mobile m_Mobile;
            private readonly HarvestExpeditionNpc m_Giver;

            public MenuEntry(Mobile from, HarvestExpeditionNpc giver) : base(6146, 3) // TODO: "Hire" ?
            {
                m_Mobile = from;
                m_Giver = giver;
            }

            public override void OnClick()
            {
                if (!(m_Mobile is PlayerMobile))
                    return;

                // TODO: Choose gump?
                var gump = new HarvestExpeditionGump(m_Mobile, m_Giver);
            }
        }
    }
}