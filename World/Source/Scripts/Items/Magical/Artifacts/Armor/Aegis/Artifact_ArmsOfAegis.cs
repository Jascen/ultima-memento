using System;
using Server;

namespace Server.Items
{
	public class Artifact_ArmsOfAegis : GiftPlateArms
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		public override int BasePhysicalResistance{ get{ return 16; } }

		[Constructable]
		public Artifact_ArmsOfAegis()
		{
			Name = "Arms of Aegis";
			Hue = 0x47E;
			ItemID = 0x1410;
			ArmorAttributes.SelfRepair = 5;
			Attributes.ReflectPhysical = 10;
			Attributes.DefendChance = 10;
			Attributes.LowerManaCost = 8;
			SkillBonuses.SetValues( 0, SkillName.Parry, 5 );
			ArtifactLevel = ArtifactLevel.StandardArtefact;
			Server.Misc.Arty.ArtySetup( this, 6, "" );
		}

		public Artifact_ArmsOfAegis( Serial serial ) : base( serial )
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
				Attributes.ReflectPhysical = 10;
				Attributes.DefendChance = 10;
				Attributes.LowerManaCost = 8;
				SkillBonuses.SetValues( 0, SkillName.Parry, 5 );
			}
		}
	}
}