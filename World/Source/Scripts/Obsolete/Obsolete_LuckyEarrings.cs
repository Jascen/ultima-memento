using System;
using Server;

namespace Server.Items
{
    public class LuckyEarrings : GoldEarrings
    {
        [Constructable]
        public LuckyEarrings()
        {
            Name = "Lucky Earrings";
            Hue = 1174;            
            Attributes.Luck = 150;
            Attributes.RegenMana = 3;
			Attributes.RegenStam = 3;
			Attributes.RegenHits = 3;
            Attributes.AttackChance = 5;
            Attributes.DefendChance = 5;
            Attributes.WeaponSpeed = 5;
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1070722, "Artefact");
        }

        public LuckyEarrings (Serial serial) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int) 0 );
        }

        private void Cleanup( object state ){ Item item = new Artifact_LuckyEarrings(); Server.Misc.Cleanup.DoCleanup( (Item)state, item ); }

public override void Deserialize(GenericReader reader)
        {
            base.Deserialize( reader ); Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( Cleanup ), this );
            int version = reader.ReadInt();
        }
    } 
}
