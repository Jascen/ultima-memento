namespace Server.Items
{
	public class VampiricEmbraceScroll : SpellScroll
	{
		public override string DefaultDescription{ get{ return NecromancerSpellbook.SpellDescription( 112 ); } }

		[Constructable]
		public VampiricEmbraceScroll() : this( 1 )
		{
		}

		[Constructable]
		public VampiricEmbraceScroll( int amount ) : base( 112, 0x226C, amount )
		{
			Name = "vampiric embrace scroll";
		}

		public VampiricEmbraceScroll( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			if ( version < 1 ) Name = "vampiric embrace scroll";
		}
	}
}