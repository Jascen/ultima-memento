using System;
using Server;
using Server.Spells;

namespace Server.Items
{
    public class FesteringWound : Kryss
	{
		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

        [Constructable]
        public FesteringWound()
        {
            Hue = 1272;
            Name = "Festering Wound";
            Attributes.AttackChance = 30;
            Attributes.SpellChanneling = 1;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 25;
            WeaponAttributes.UseBestSkill = 1;
            WeaponAttributes.HitMagicArrow = 20;
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1070722, "Artefact");
        }

        public override void GetDamageTypes( Mobile weilder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
        {
            phys = 20;
            nrgy = 10;
            cold = 10;
            pois = 50;
            fire = 10;
            chaos = 0;
            direct = 0;
        }

        public FesteringWound( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        private void Cleanup( object state ){ Item item = new Artifact_FesteringWound(); Server.Misc.Cleanup.DoCleanup( (Item)state, item ); }

public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader ); Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( Cleanup ), this );

            int version = reader.ReadInt();
        }
    }
}