using System;
using Server;

namespace Server.Items
{
	public class Artifact_HolyKnightsLegging : GiftRoyalsLegs
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		public override int BasePhysicalResistance{ get{ return 22; } }

		[Constructable]
		public Artifact_HolyKnightsLegging()
		{
			Name = "Holy Knight's Legging";
			Hue = 0x47E;
			Attributes.BonusHits = 10;
			Attributes.ReflectPhysical = 15;
			SkillBonuses.SetValues( 0, SkillName.Knightship, 5 );
			SkillBonuses.SetValues( 1, SkillName.Focus, 5 );
			ArtifactLevel = ArtifactLevel.StandardArtefact;
			Server.Misc.Arty.ArtySetup( this, 5, "" );
		}

		public Artifact_HolyKnightsLegging( Serial serial ) : base( serial )
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