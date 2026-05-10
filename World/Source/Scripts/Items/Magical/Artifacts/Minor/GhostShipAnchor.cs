using System;
using Server;

namespace Server.Items
{
	public class GhostShipAnchor : Item
	{
		public override int LabelNumber{ get{ return 1070816; } } // Ghost Ship Anchor
		
		[Constructable]
		public GhostShipAnchor() : base( 0x14F7 )
		{
			Hue = 0x47E;
			Weight = 80;
			ArtifactLevel = ArtifactLevel.DecorativeArtefact;
		}

		public GhostShipAnchor( Serial serial ) : base( serial )
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
			if (version < 1)
				ArtifactLevel = ArtifactLevel.DecorativeArtefact;
			
			if (version < 2)
				Weight = 80;
		}
	}
}