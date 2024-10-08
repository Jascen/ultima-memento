using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class MelisandesCorrodedHatchet : Hatchet
	{
		public override int LabelNumber{ get{ return 1072115; } } // Melisande's Corroded Hatchet

		[Constructable]
		public MelisandesCorrodedHatchet()
		{
			Hue = 0x494;

			SkillBonuses.SetValues( 0, SkillName.Lumberjacking, 5.0 );

			Attributes.SpellChanneling = 1;
			Attributes.WeaponSpeed = 15;
			Attributes.WeaponDamage = -50;

			WeaponAttributes.SelfRepair = 4;
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1070722, "Artefact");
        }

		public MelisandesCorrodedHatchet( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		private void Cleanup( object state ){ Item item = new Artifact_MelisandesCorrodedHatchet(); Server.Misc.Cleanup.DoCleanup( (Item)state, item ); }

public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader ); Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( Cleanup ), this );

			int version = reader.ReadEncodedInt();
		}
	}
}