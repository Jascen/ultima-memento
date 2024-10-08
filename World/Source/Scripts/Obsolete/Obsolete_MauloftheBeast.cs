using System;
using Server;

namespace Server.Items
{
    public class MauloftheBeast : Maul
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

        [Constructable]
        public MauloftheBeast()
        {
            Name = "Maul of the Beast";
            Hue = 1779;
            Attributes.WeaponDamage = 60;
            WeaponAttributes.HitLeechHits = 35;
            WeaponAttributes.HitLeechMana = 35;
            WeaponAttributes.HitLeechStam = 35;
            WeaponAttributes.SelfRepair = 2;
            Attributes.SpellChanneling = 1;
            Attributes.WeaponSpeed = -30;
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1070722, "Artefact");
        }

        public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
        {
            phys = 100;
            cold = 0;
            fire = 0;
            nrgy = 0;
            pois = 0;
            chaos = 0;
            direct = 0;
        }
        public MauloftheBeast( Serial serial )
            : base( serial )
        {
        }
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }
        private void Cleanup( object state ){ Item item = new Artifact_MauloftheBeast(); Server.Misc.Cleanup.DoCleanup( (Item)state, item ); }

public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader ); Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( Cleanup ), this );
            int version = reader.ReadInt();
        }
    }
}
