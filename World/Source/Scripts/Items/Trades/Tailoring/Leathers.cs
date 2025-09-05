using System;
using Server.Items;
using Server.Items.Abstractions;
using Server.Network;

namespace Server.Items
{
	public abstract class BaseLeather : Item, ICommodity
	{
		public override string DefaultDescription{ get{ return "Tailors, with leatherworking tools, can create leather armor and weapons with this. Scribes also use leather to bind books."; } }

		public override Catalogs DefaultCatalog{ get{ return Catalogs.Crafting; } }

		public virtual bool IsCommodity { get { return true; } }

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 2 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version < 2 )
				m_Resource = (CraftResource)reader.ReadInt();

			Hue = CraftResources.GetHue( m_Resource );
			Name = CraftResources.GetTradeItemFullName( this, m_Resource, false, false, null );
			Built = true;
		}

		public BaseLeather( CraftResource resource ) : this( resource, 1 )
		{
		}

		public BaseLeather( CraftResource resource, int amount ) : base( 0x1081 )
		{
			Stackable = true;
			Amount = amount;
			m_Resource = resource;
			Hue = CraftResources.GetHue( resource );
			Name = CraftResources.GetTradeItemFullName( this, resource, false, false, null );
			Built = true;
		}

		public BaseLeather( Serial serial ) : base( serial )
		{
		}
		
		public bool Scissor( Mobile from, Scissors scissors )
		{
			if ( Deleted || !from.CanSee( this ) ) return false;

			base.ScissorHelper( from, new Bandage(), 1 );

			return true;
		}

		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;

			Hue = sender.DyedHue;

			return true;
		}
	}

	public class Leather : BaseLeather
	{
		[Constructable]
		public Leather() : this( 1 )
		{
		}

		[Constructable]
		public Leather( int amount ) : base( CraftResource.RegularLeather, amount )
		{
		}

		public Leather( Serial serial ) : base( serial )
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

	public class SpinedLeather : BaseLeather
	{
		[Constructable]
		public SpinedLeather() : this( 1 )
		{
		}

		[Constructable]
		public SpinedLeather( int amount ) : base( CraftResource.SpinedLeather, amount )
		{
		}

		public SpinedLeather( Serial serial ) : base( serial )
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

	public class HornedLeather : BaseLeather
	{
		[Constructable]
		public HornedLeather() : this( 1 )
		{
		}

		[Constructable]
		public HornedLeather( int amount ) : base( CraftResource.HornedLeather, amount )
		{
		}

		public HornedLeather( Serial serial ) : base( serial )
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

	public class BarbedLeather : BaseLeather
	{
		[Constructable]
		public BarbedLeather() : this( 1 )
		{
		}

		[Constructable]
		public BarbedLeather( int amount ) : base( CraftResource.BarbedLeather, amount )
		{
		}

		public BarbedLeather( Serial serial ) : base( serial )
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

	public class NecroticLeather : BaseLeather
	{
		[Constructable]
		public NecroticLeather() : this( 1 )
		{
		}

		[Constructable]
		public NecroticLeather( int amount ) : base( CraftResource.NecroticLeather, amount )
		{
		}

		public NecroticLeather( Serial serial ) : base( serial )
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

	public class VolcanicLeather : BaseLeather
	{
		[Constructable]
		public VolcanicLeather() : this( 1 )
		{
		}

		[Constructable]
		public VolcanicLeather( int amount ) : base( CraftResource.VolcanicLeather, amount )
		{
		}

		public VolcanicLeather( Serial serial ) : base( serial )
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

	public class FrozenLeather : BaseLeather
	{
		[Constructable]
		public FrozenLeather() : this( 1 )
		{
		}

		[Constructable]
		public FrozenLeather( int amount ) : base( CraftResource.FrozenLeather, amount )
		{
		}

		public FrozenLeather( Serial serial ) : base( serial )
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

	public class GoliathLeather : BaseLeather
	{
		[Constructable]
		public GoliathLeather() : this( 1 )
		{
		}

		[Constructable]
		public GoliathLeather( int amount ) : base( CraftResource.GoliathLeather, amount )
		{
		}

		public GoliathLeather( Serial serial ) : base( serial )
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

	public class DraconicLeather : BaseLeather
	{
		[Constructable]
		public DraconicLeather() : this( 1 )
		{
		}

		[Constructable]
		public DraconicLeather( int amount ) : base( CraftResource.DraconicLeather, amount )
		{
		}

		public DraconicLeather( Serial serial ) : base( serial )
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

	public class HellishLeather : BaseLeather
	{
		[Constructable]
		public HellishLeather() : this( 1 )
		{
		}

		[Constructable]
		public HellishLeather( int amount ) : base( CraftResource.HellishLeather, amount )
		{
		}

		public HellishLeather( Serial serial ) : base( serial )
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

	public class DinosaurLeather : BaseLeather
	{
		[Constructable]
		public DinosaurLeather() : this( 1 )
		{
		}

		[Constructable]
		public DinosaurLeather( int amount ) : base( CraftResource.DinosaurLeather, amount )
		{
		}

		public DinosaurLeather( Serial serial ) : base( serial )
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

	public class AlienLeather : BaseLeather
	{
		[Constructable]
		public AlienLeather() : this( 1 )
		{
		}

		[Constructable]
		public AlienLeather( int amount ) : base( CraftResource.AlienLeather, amount )
		{
		}

		public AlienLeather( Serial serial ) : base( serial )
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
	public class AdesoteLeather : BaseLeather, IScissorable, IDyable
	{
		[Constructable]
		public AdesoteLeather() : this( 1 )
		{
		}

		[Constructable]
		public AdesoteLeather( int amount ) : base( CraftResource.Adesote, amount )
		{
			ItemID = 0x1763;
		}

		public AdesoteLeather( Serial serial ) : base( serial )
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
	public class BiomeshLeather : BaseLeather, IScissorable, IDyable
	{
		[Constructable]
		public BiomeshLeather() : this( 1 )
		{
		}

		[Constructable]
		public BiomeshLeather( int amount ) : base( CraftResource.Biomesh, amount )
		{
			ItemID = 0x1763;
		}

		public BiomeshLeather( Serial serial ) : base( serial )
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
	public class CerlinLeather : BaseLeather, IScissorable, IDyable
	{
		[Constructable]
		public CerlinLeather() : this( 1 )
		{
		}

		[Constructable]
		public CerlinLeather( int amount ) : base( CraftResource.Cerlin, amount )
		{
			ItemID = 0x1763;
		}

		public CerlinLeather( Serial serial ) : base( serial )
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
	public class DurafiberLeather : BaseLeather, IScissorable, IDyable
	{
		[Constructable]
		public DurafiberLeather() : this( 1 )
		{
		}

		[Constructable]
		public DurafiberLeather( int amount ) : base( CraftResource.Durafiber, amount )
		{
			ItemID = 0x1763;
		}

		public DurafiberLeather( Serial serial ) : base( serial )
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
	public class FlexicrisLeather : BaseLeather, IScissorable, IDyable
	{
		[Constructable]
		public FlexicrisLeather() : this( 1 )
		{
		}

		[Constructable]
		public FlexicrisLeather( int amount ) : base( CraftResource.Flexicris, amount )
		{
			ItemID = 0x1763;
		}

		public FlexicrisLeather( Serial serial ) : base( serial )
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
	public class HyperclothLeather : BaseLeather, IScissorable, IDyable
	{
		[Constructable]
		public HyperclothLeather() : this( 1 )
		{
		}

		[Constructable]
		public HyperclothLeather( int amount ) : base( CraftResource.Hypercloth, amount )
		{
			ItemID = 0x1763;
		}

		public HyperclothLeather( Serial serial ) : base( serial )
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
	public class NylarLeather : BaseLeather, IScissorable, IDyable
	{
		[Constructable]
		public NylarLeather() : this( 1 )
		{
		}

		[Constructable]
		public NylarLeather( int amount ) : base( CraftResource.Nylar, amount )
		{
			ItemID = 0x1763;
		}

		public NylarLeather( Serial serial ) : base( serial )
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
	public class NyloniteLeather : BaseLeather, IScissorable, IDyable
	{
		[Constructable]
		public NyloniteLeather() : this( 1 )
		{
		}

		[Constructable]
		public NyloniteLeather( int amount ) : base( CraftResource.Nylonite, amount )
		{
			ItemID = 0x1763;
		}

		public NyloniteLeather( Serial serial ) : base( serial )
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
	public class PolyfiberLeather : BaseLeather, IScissorable, IDyable
	{
		[Constructable]
		public PolyfiberLeather() : this( 1 )
		{
		}

		[Constructable]
		public PolyfiberLeather( int amount ) : base( CraftResource.Polyfiber, amount )
		{
			ItemID = 0x1763;
		}

		public PolyfiberLeather( Serial serial ) : base( serial )
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
	public class SynclothLeather : BaseLeather, IScissorable, IDyable
	{
		[Constructable]
		public SynclothLeather() : this( 1 )
		{
		}

		[Constructable]
		public SynclothLeather( int amount ) : base( CraftResource.Syncloth, amount )
		{
			ItemID = 0x1763;
		}

		public SynclothLeather( Serial serial ) : base( serial )
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
	public class ThermoweaveLeather : BaseLeather, IScissorable, IDyable
	{
		[Constructable]
		public ThermoweaveLeather() : this( 1 )
		{
		}

		[Constructable]
		public ThermoweaveLeather( int amount ) : base( CraftResource.Thermoweave, amount )
		{
			ItemID = 0x1763;
		}

		public ThermoweaveLeather( Serial serial ) : base( serial )
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