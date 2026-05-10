using System;
using Server;

namespace Server.Items
{
	public class Artifact_SpiritOfTheTotem : GiftBearMask
	{
		public override int BasePhysicalResistance{ get{ return 20; } }

		[Constructable]
		public Artifact_SpiritOfTheTotem()
		{
			Hue = 0x455;
			Name = "Spirit of the Totem";
			Attributes.BonusStr = 20;
			Attributes.ReflectPhysical = 15;
			Attributes.AttackChance = 15;
			ArtifactLevel = ArtifactLevel.StandardArtefact;
			Server.Misc.Arty.ArtySetup( this, 5, "" );
		}

		public Artifact_SpiritOfTheTotem( Serial serial ) : base( serial )
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

			switch ( version )
			{
				case 0:
				{
					Resistances.Physical = 0;
					break;
				}
			}
		}
	}
}