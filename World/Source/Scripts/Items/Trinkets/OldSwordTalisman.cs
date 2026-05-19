using Server.Network;

namespace Server.Items
{
	public class OldSwordTalisman : BaseTrinket
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Owner { get; set; }

		[Constructable]
		public OldSwordTalisman() : base(0xEC4)
		{
			Resource = CraftResource.None;
			Layer = Layer.Trinket;
			Weight = 1.0;
			Name = "An old sword";

			SkillBonuses.Skill_1_Name = SkillName.Alchemy;
			SkillBonuses.Skill_1_Value = 25;
			Attributes.RegenHits = 5;
			Attributes.RegenStam = 5;
			Attributes.BonusHits = 20;
			Attributes.BonusMana = -50;
		}

		public OldSwordTalisman(Serial serial) : base(serial)
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			from.SendMessage("Trinkets are equipped on your hip.");
		}

		public override void AddNameProperties(ObjectPropertyList list)
		{
			base.AddNameProperties(list);

			if ( Owner != null ){ list.Add( 1049644, "Belongs to " + Owner.Name + "" ); }
			else { list.Add(1070722, "Trinket"); }
		}

		public override bool CanEquip( Mobile from )
		{
			if ( Owner != from )
			{
				from.LocalOverheadMessage( MessageType.Emote, 0x916, true, "This talisman belongs to another!" );
				return false;
			}
			return base.CanEquip( from );
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version
			writer.Write(Owner);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			Owner = reader.ReadMobile();
		}
	}
}