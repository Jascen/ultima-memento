using System;
using Server;

namespace Server.Items
{
	public class Artifact_Aegis : GiftHeaterShield
	{
		public override int BasePhysicalResistance{ get{ return 15; } }

		[Constructable]
		public Artifact_Aegis()
		{
			Name = "Aegis";
			Hue = 0x47E;
			ArmorAttributes.SelfRepair = 5;
			Attributes.ReflectPhysical = 20;
			Attributes.DefendChance = 20;
			Attributes.LowerManaCost = 10;
			SkillBonuses.SetValues( 0, SkillName.Parry, 10 );
			ArtifactLevel = ArtifactLevel.StandardArtefact;
			Server.Misc.Arty.ArtySetup( this, 8, "" );
		}

		public Artifact_Aegis( Serial serial ) : base( serial )
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
				Attributes.ReflectPhysical = 20;
				Attributes.DefendChance = 20;
				Attributes.LowerManaCost = 10;
				SkillBonuses.SetValues( 0, SkillName.Parry, 10 );
			}
		}
	}
}