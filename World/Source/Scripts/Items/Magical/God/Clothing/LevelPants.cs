using Server.Mobiles;

namespace Server.Items
{
	public abstract class BaseLevelPants : BaseLevelClothing
	{
		public BaseLevelPants( int itemID ) : this( itemID, 0 )
		{
		}

		public BaseLevelPants( int itemID, int hue ) : base( itemID, Layer.Pants, hue )
		{
		}

		public BaseLevelPants( Serial serial ) : base( serial )
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

	public class LevelShortPants : BaseLevelPants
	{
		[Constructable]
		public LevelShortPants() : this( 0 )
		{
		}

		[Constructable]
		public LevelShortPants( int hue ) : base( 0x152E, hue )
		{
			Weight = 2.0;
			Name = "short pants";
			ItemID = Utility.RandomList( 0x152e, 0x152f );
		}

        public LevelShortPants(Serial serial): base(serial)
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

	public class LevelLongPants : BaseLevelPants
	{
		[Constructable]
		public LevelLongPants() : this( 0 )
		{
		}

		[Constructable]
		public LevelLongPants( int hue ) : base( 0x1539, hue )
		{
			Weight = 2.0;
			Name = "long pants";
			ItemID = Utility.RandomList( 0x1539, 0x153a );
		}

        public LevelLongPants(Serial serial): base(serial)
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
	public class LevelTattsukeHakama : BaseLevelPants
	{
		[Constructable]
		public LevelTattsukeHakama() : this( 0 )
		{
		}

		[Constructable]
		public LevelTattsukeHakama( int hue ) : base( 0x279B, hue )
		{
			Weight = 2.0;
		}

        public LevelTattsukeHakama(Serial serial)
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

	[FlipableAttribute( 0x2FC3, 0x3179 )]
	public class LevelElvenPants : BaseLevelPants
	{
		[Constructable]
		public LevelElvenPants() : this( 0 )
		{
		}

		[Constructable]
		public LevelElvenPants( int hue ) : base( 0x2FC3, hue )
		{
			Name = "pants";
			Weight = 2.0;
		}

        public LevelElvenPants(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}