using System;
using Server;

namespace Server.Items
{
	public class Artifact_TunicOfAegis : GiftPlateChest
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		public override int BasePhysicalResistance{ get{ return 25; } }

		[Constructable]
		public Artifact_TunicOfAegis()
		{
			Name = "Tunic of Aegis";
			Hue = 0x47E;
			ItemID = 0x1415;
			ArmorAttributes.SelfRepair = 5;
			Attributes.ReflectPhysical = 15;
			Attributes.DefendChance = 15;
			Attributes.LowerManaCost = 10;
			SkillBonuses.SetValues( 0, SkillName.Parry, 10 );
			ArtifactLevel = ArtifactLevel.StandardArtefact;
			Server.Misc.Arty.ArtySetup( this, 8, "" );
		}

		public Artifact_TunicOfAegis( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 2 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if (version < 2)
			{
				Attributes.ReflectPhysical = 15;
				Attributes.DefendChance = 15;
				SkillBonuses.SetValues( 0, SkillName.Parry, 10 );
			}
		}
	}
}