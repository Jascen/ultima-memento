using System;
using Server;

namespace Server.Items
{
	public class Artifact_LeggingsOfAegis : GiftPlateLegs
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		public override int BasePhysicalResistance{ get{ return 22; } }

		[Constructable]
		public Artifact_LeggingsOfAegis()
		{
			Name = "Leggings of Aegis";
			Hue = 0x47E;
			ItemID = 0x46AA;
			ArmorAttributes.SelfRepair = 5;
			Attributes.ReflectPhysical = 15;
			Attributes.DefendChance = 15;
			Attributes.LowerManaCost = 10;
			SkillBonuses.SetValues( 0, SkillName.Parry, 10 );
			ArtifactLevel = ArtifactLevel.StandardArtefact;
			Server.Misc.Arty.ArtySetup( this, 7, "" );
		}

		public Artifact_LeggingsOfAegis( Serial serial ) : base( serial )
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
				Attributes.LowerManaCost = 10;
				SkillBonuses.SetValues( 0, SkillName.Parry, 10 );
			}
		}
	}
}