namespace Server.Items
{
	public class ArcaneScroll : Item
	{
		public override string DefaultDescription{ get{ return "These scrolls have arcane symbols written on them. They are used by scribes to create powerful spells."; } }

		[Constructable]
		public ArcaneScroll() : this( 1 )
		{
		}

		[Constructable]
		public ArcaneScroll( int amount ) : base( 0xEF3 )
		{
			Name = "Arcane Scroll";
			Stackable = true;
			Hue = 2291;
			Weight = 0.1;
			Amount = amount;
		}

		public ArcaneScroll( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}