using System;
using Server;

namespace Server.Items
{
	public class Artifact_HolyKnightsBreastplate : GiftRoyalChest
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }
		public override int BasePhysicalResistance{ get{ return 25; } }

		[Constructable]
		public Artifact_HolyKnightsBreastplate()
		{
			Name = "Holy Knight's Breastplate";
			Hue = 0x47E;
			Attributes.BonusHits = 10;
			Attributes.ReflectPhysical = 15;
			SkillBonuses.SetValues( 0, SkillName.Knightship, 5 );
			SkillBonuses.SetValues( 1, SkillName.Focus, 5 );
			ArtifactLevel = ArtifactLevel.StandardArtefact;
			Server.Misc.Arty.ArtySetup( this, 8, "" );
		}

		public Artifact_HolyKnightsBreastplate( Serial serial ) : base( serial )
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
				SkillBonuses.SetValues( 0, SkillName.Knightship, 5 );
				SkillBonuses.SetValues( 1, SkillName.Focus, 5 );
			}
		}
	}
}