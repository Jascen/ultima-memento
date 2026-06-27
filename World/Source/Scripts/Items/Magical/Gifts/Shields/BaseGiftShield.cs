using Server.Mobiles;
using Server.ContextMenus;
using System.Collections.Generic;

namespace Server.Items
{
    public abstract class BaseGiftShield : BaseShield, IGiftable
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Gifter { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string How { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Points { get; set; }

        public BaseGiftShield(int itemID) : base(itemID)
        {
			Owner = null;
			Gifter = "";
			How = "";
			Points = 0;
        }

		public override bool OnDragLift( Mobile from )
		{
			if ( from is PlayerMobile && Owner == null && How == "Unearthed by" )
				Owner = from;

			Server.Misc.Arty.setArtifact( this );

			return true;
		}

        public BaseGiftShield(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
            writer.Write(Owner);
            writer.Write(Gifter);
            writer.Write(How);
            writer.Write(Points);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			Owner = reader.ReadMobile();
			Gifter = reader.ReadString();
			How = reader.ReadString();
			Points = reader.ReadInt();
        }

		public override void AddNameProperties(ObjectPropertyList list)
		{
			base.AddNameProperties(list);
			if ( Points > 5 ){ list.Add( 1070722, "Single Click to Enchant"); }
			else if ( Gifter != "" && Gifter != null ){ list.Add( 1070722, Gifter); }
			if ( Points > 5 && How == "Unearthed by" ){ list.Add( 1049644, Points + " Enchantment Points" ); }
			else if ( Owner != null ){ list.Add( 1049644, How + " " + Owner.Name + "" ); }
		}

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            if ( Points > 0 ){ list.Add(new GiftInfoEntry(from, this, GiftAttributeCategory.Melee)); }
        }
    }
}