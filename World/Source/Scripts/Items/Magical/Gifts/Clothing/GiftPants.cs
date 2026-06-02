using Server.Mobiles;

namespace Server.Items
{
	public abstract class BaseGiftPants : BaseGiftClothing
	{
		public BaseGiftPants( int itemID ) : this( itemID, 0 )
		{
		}

		public BaseGiftPants( int itemID, int hue ) : base( itemID, Layer.Pants, hue )
		{
		}

		public BaseGiftPants( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override Layer Layer
		{ 
			get
			{
				if (false == RootParentEntity is PlayerMobile) return Layer.Pants;

				return ((PlayerMobile)RootParentEntity).Temptations.CanWearTightPants ? Layer.InnerLegs : Layer.Pants;
			} 
			set { base.Layer = value; }
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

	public class GiftShortPants : BaseGiftPants
	{
		[Constructable]
		public GiftShortPants() : this( 0 )
		{
		}

		[Constructable]
		public GiftShortPants( int hue ) : base( 0x152E, hue )
		{
			Weight = 2.0;
			Name = "short pants";
			ItemID = Utility.RandomList( 0x152e, 0x152f );
		}

        public GiftShortPants(Serial serial)
            : base(serial)
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

	public class GiftLongPants : BaseGiftPants
	{
		[Constructable]
		public GiftLongPants() : this( 0 )
		{
		}

		[Constructable]
		public GiftLongPants( int hue ) : base( 0x1539, hue )
		{
			Weight = 2.0;
			Name = "long pants";
			ItemID = Utility.RandomList( 0x1539, 0x153a );
		}

        public GiftLongPants(Serial serial): base(serial)
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

	[Flipable( 0x279B, 0x27E6 )]
	public class GiftTattsukeHakama : BaseGiftPants
	{
		[Constructable]
		public GiftTattsukeHakama() : this( 0 )
		{
		}

		[Constructable]
		public GiftTattsukeHakama( int hue ) : base( 0x279B, hue )
		{
			Weight = 2.0;
		}

        public GiftTattsukeHakama(Serial serial)
            : base(serial)
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