using System;
using Server;

namespace Server.Items
{
	public class Artifact_ArcaneCap : GiftLeatherCap
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		[Constructable]
		public Artifact_ArcaneCap()
		{
			Name = "Arcane Cap";
			Hue = 0x556;
			Attributes.NightSight = 1;
			Attributes.DefendChance = 8;
			Attributes.CastSpeed = 1;
			Attributes.LowerManaCost = 5;
			Attributes.LowerRegCost = 5;
			Attributes.SpellDamage = 5;
			ArtifactLevel = ArtifactLevel.StandardArtefact;
			Server.Misc.Arty.ArtySetup( this, 6, "" );
		}

		public Artifact_ArcaneCap( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if (version < 1)
			{
				Attributes.DefendChance = 8;
				Attributes.LowerManaCost = 5;
				Attributes.LowerRegCost = 5;
				Attributes.SpellDamage = 5;
			}
		}
	}
}