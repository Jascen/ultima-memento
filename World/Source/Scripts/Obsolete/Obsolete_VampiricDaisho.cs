using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class VampiricDaisho : Daisho
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

      [Constructable]
		public VampiricDaisho()
		{
			Name = "Vampiric Daisho";
			Hue = 1153;
			WeaponAttributes.HitHarm = 50;
			WeaponAttributes.HitLeechHits = 45;
			WeaponAttributes.HitLeechStam = 20;
			Attributes.LowerManaCost = 5;
			Attributes.NightSight = 1;
			Attributes.SpellChanneling = 1;
			Slayer = SlayerName.BloodDrinking ;
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1070722, "Artefact");
        }

		public VampiricDaisho( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		private void Cleanup( object state ){ Item item = new Artifact_VampiricDaisho(); Server.Misc.Cleanup.DoCleanup( (Item)state, item ); }

public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader ); Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( Cleanup ), this );

			int version = reader.ReadInt();
		}
	}
}
