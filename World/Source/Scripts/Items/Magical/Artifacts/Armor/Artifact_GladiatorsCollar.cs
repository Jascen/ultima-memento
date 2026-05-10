namespace Server.Items
{
	public class Artifact_GladiatorsCollar : GiftPlateGorget
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		public override int BasePhysicalResistance{ get{ return 12; } }
		public override int BaseFireResistance{ get{ return 12; } }
		public override int BaseColdResistance{ get{ return 12; } }
		public override int BasePoisonResistance{ get{ return 12; } }
		public override int BaseEnergyResistance{ get{ return 12; } }

		public override int AosStrReq{ get{ return 60; } }

		public override bool CanFortify{ get{ return false; } }

		[Constructable]
		public Artifact_GladiatorsCollar()
		{
			Name = "Gladiator's Collar";
			Hue = 0x26d;
			ItemID = 0x1413;

			SkillBonuses.SetValues( 0, Utility.RandomCombatSkill(), 10.0 );

			Attributes.BonusHits = 10;
			Attributes.AttackChance = 10;

			ArmorAttributes.MageArmor = 1;
			ArtifactLevel = ArtifactLevel.StandardArtefact;
			Server.Misc.Arty.ArtySetup( this, 10, "" );
		}

		public Artifact_GladiatorsCollar( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
