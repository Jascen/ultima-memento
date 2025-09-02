using System;
using Server.Items;
using Server.Network;
using Server.Items.Abstractions;

namespace Server.Items
{
	public abstract class BaseFabric : Item, IScissorable, IDyable, ICommodity
	{
		public override string DefaultDescription{ get{ return "This cloth is commonly used by tailors to make clothing. You can also cut it with scissors to make bandages."; } }

		public override Catalogs DefaultCatalog{ get{ return Catalogs.Crafting; } }

		public virtual bool IsCommodity { get { return true; } }

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;

			Hue = sender.DyedHue;

			return true;
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
			Name = CraftResources.GetTradeItemFullName( this, m_Resource, false, false, null );
			Built = true;
		}

		public BaseFabric( CraftResource resource ) : this( resource, 1 )
		{
		}

		public BaseFabric( CraftResource resource, int amount ) : base( 0x1763 )
		{
			Stackable = true;
			Amount = amount;
			m_Resource = resource;
			Hue = CraftResources.GetHue( resource );
			Name = CraftResources.GetTradeItemFullName( this, m_Resource, false, false, null );
			Built = true;
		}

		public BaseFabric( Serial serial ) : base( serial )
		{
		}
		
		public bool Scissor( Mobile from, Scissors scissors )
		{
			if ( Deleted || !from.CanSee( this ) ) return false;

			base.ScissorHelper( from, new Bandage(), 1 );

			return true;
		}
	}

	public class Fabric : BaseFabric
	{
		[Constructable]
		public Fabric() : this( 1 )
		{
		}

		[Constructable]
		public Fabric( int amount ) : base( CraftResource.Fabric, amount )
		{
		}

		public Fabric( Serial serial ) : base( serial )
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

	public class FurryFabric : BaseFabric
	{
		[Constructable]
		public FurryFabric() : this( 1 )
		{
		}

		[Constructable]
		public FurryFabric( int amount ) : base( CraftResource.FurryFabric, amount )
		{
			ItemID = 0x670F;
		}

		public FurryFabric( Serial serial ) : base( serial )
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

	public class WoolyFabric : BaseFabric
	{
		[Constructable]
		public WoolyFabric() : this( 1 )
		{
		}

		[Constructable]
		public WoolyFabric( int amount ) : base( CraftResource.WoolyFabric, amount )
		{
			ItemID = 0x670F;
		}

		public WoolyFabric( Serial serial ) : base( serial )
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

	public class SilkFabric : BaseFabric
	{
		[Constructable]
		public SilkFabric() : this( 1 )
		{
		}

		[Constructable]
		public SilkFabric( int amount ) : base( CraftResource.SilkFabric, amount )
		{
		}

		public SilkFabric( Serial serial ) : base( serial )
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

	public class HauntedFabric : BaseFabric
	{
		[Constructable]
		public HauntedFabric() : this( 1 )
		{
		}

		[Constructable]
		public HauntedFabric( int amount ) : base( CraftResource.HauntedFabric, amount )
		{
		}

		public HauntedFabric( Serial serial ) : base( serial )
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

	public class ArcticFabric : BaseFabric
	{
		[Constructable]
		public ArcticFabric() : this( 1 )
		{
		}

		[Constructable]
		public ArcticFabric( int amount ) : base( CraftResource.ArcticFabric, amount )
		{
		}

		public ArcticFabric( Serial serial ) : base( serial )
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

	public class PyreFabric : BaseFabric
	{
		[Constructable]
		public PyreFabric() : this( 1 )
		{
		}

		[Constructable]
		public PyreFabric( int amount ) : base( CraftResource.PyreFabric, amount )
		{
		}

		public PyreFabric( Serial serial ) : base( serial )
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

	public class VenomousFabric : BaseFabric
	{
		[Constructable]
		public VenomousFabric() : this( 1 )
		{
		}

		[Constructable]
		public VenomousFabric( int amount ) : base( CraftResource.VenomousFabric, amount )
		{
		}

		public VenomousFabric( Serial serial ) : base( serial )
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

	public class MysteriousFabric : BaseFabric
	{
		[Constructable]
		public MysteriousFabric() : this( 1 )
		{
		}

		[Constructable]
		public MysteriousFabric( int amount ) : base( CraftResource.MysteriousFabric, amount )
		{
		}

		public MysteriousFabric( Serial serial ) : base( serial )
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

	public class VileFabric : BaseFabric
	{
		[Constructable]
		public VileFabric() : this( 1 )
		{
		}

		[Constructable]
		public VileFabric( int amount ) : base( CraftResource.VileFabric, amount )
		{
		}

		public VileFabric( Serial serial ) : base( serial )
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

	public class DivineFabric : BaseFabric
	{
		[Constructable]
		public DivineFabric() : this( 1 )
		{
		}

		[Constructable]
		public DivineFabric( int amount ) : base( CraftResource.DivineFabric, amount )
		{
		}

		public DivineFabric( Serial serial ) : base( serial )
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

	public class FiendishFabric : BaseFabric
	{
		[Constructable]
		public FiendishFabric() : this( 1 )
		{
		}

		[Constructable]
		public FiendishFabric( int amount ) : base( CraftResource.FiendishFabric, amount )
		{
		}

		public FiendishFabric( Serial serial ) : base( serial )
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