using System;
using Server.Items;
using Server.Items.Abstractions;
using Server.Network;

namespace Server.Items
{
	public abstract class BaseIngot : Item, ICommodity
	{
		public override string DefaultDescription{ get{ return "These bars of metal are commonly used by blacksmiths, to create armor and weapons. Tinkers uses these as well."; } }

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

			Name = CraftResources.GetTradeItemFullName( this, m_Resource, false, false, null );
			Built = true;
		}

		public BaseIngot( CraftResource resource ) : this( resource, 1 )
		{
		}

		public BaseIngot( CraftResource resource, int amount ) : base( 0x1BF2 )
		{
			Stackable = true;
			Amount = amount;
			m_Resource = resource;
			Hue = CraftResources.GetHue( resource );
			Name = CraftResources.GetTradeItemFullName( this, m_Resource, false, false, null );
			Built = true;
		}

		public BaseIngot( Serial serial ) : base( serial )
		{
		}
	}

	public class IronIngot : BaseIngot
	{
		[Constructable]
		public IronIngot() : this( 1 )
		{
		}

		[Constructable]
		public IronIngot( int amount ) : base( CraftResource.Iron, amount )
		{
		}

		public IronIngot( Serial serial ) : base( serial )
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

	public class DullCopperIngot : BaseIngot
	{
		[Constructable]
		public DullCopperIngot() : this( 1 )
		{
		}

		[Constructable]
		public DullCopperIngot( int amount ) : base( CraftResource.DullCopper, amount )
		{
		}

		public DullCopperIngot( Serial serial ) : base( serial )
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

	public class ShadowIronIngot : BaseIngot
	{
		[Constructable]
		public ShadowIronIngot() : this( 1 )
		{
		}

		[Constructable]
		public ShadowIronIngot( int amount ) : base( CraftResource.ShadowIron, amount )
		{
		}

		public ShadowIronIngot( Serial serial ) : base( serial )
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

	public class CopperIngot : BaseIngot
	{
		[Constructable]
		public CopperIngot() : this( 1 )
		{
		}

		[Constructable]
		public CopperIngot( int amount ) : base( CraftResource.Copper, amount )
		{
		}

		public CopperIngot( Serial serial ) : base( serial )
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

	public class BronzeIngot : BaseIngot
	{
		[Constructable]
		public BronzeIngot() : this( 1 )
		{
		}

		[Constructable]
		public BronzeIngot( int amount ) : base( CraftResource.Bronze, amount )
		{
		}

		public BronzeIngot( Serial serial ) : base( serial )
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

	public class GoldIngot : BaseIngot
	{
		[Constructable]
		public GoldIngot() : this( 1 )
		{
		}

		[Constructable]
		public GoldIngot( int amount ) : base( CraftResource.Gold, amount )
		{
		}

		public GoldIngot( Serial serial ) : base( serial )
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

	public class AgapiteIngot : BaseIngot
	{
		[Constructable]
		public AgapiteIngot() : this( 1 )
		{
		}

		[Constructable]
		public AgapiteIngot( int amount ) : base( CraftResource.Agapite, amount )
		{
		}

		public AgapiteIngot( Serial serial ) : base( serial )
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

	public class VeriteIngot : BaseIngot
	{
		[Constructable]
		public VeriteIngot() : this( 1 )
		{
		}

		[Constructable]
		public VeriteIngot( int amount ) : base( CraftResource.Verite, amount )
		{
		}

		public VeriteIngot( Serial serial ) : base( serial )
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

	public class ValoriteIngot : BaseIngot
	{
		[Constructable]
		public ValoriteIngot() : this( 1 )
		{
		}

		[Constructable]
		public ValoriteIngot( int amount ) : base( CraftResource.Valorite, amount )
		{
		}

		public ValoriteIngot( Serial serial ) : base( serial )
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

	public class SteelIngot : BaseIngot
	{
		[Constructable]
		public SteelIngot() : this( 1 )
		{
		}

		[Constructable]
		public SteelIngot( int amount ) : base( CraftResource.Steel, amount )
		{
		}

		public SteelIngot( Serial serial ) : base( serial )
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

	public class BrassIngot : BaseIngot
	{
		[Constructable]
		public BrassIngot() : this( 1 )
		{
		}

		[Constructable]
		public BrassIngot( int amount ) : base( CraftResource.Brass, amount )
		{
		}

		public BrassIngot( Serial serial ) : base( serial )
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

	public class MithrilIngot : BaseIngot
	{
		[Constructable]
		public MithrilIngot() : this( 1 )
		{
		}

		[Constructable]
		public MithrilIngot( int amount ) : base( CraftResource.Mithril, amount )
		{
		}

		public MithrilIngot( Serial serial ) : base( serial )
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

	public class DwarvenIngot : BaseIngot
	{
		[Constructable]
		public DwarvenIngot() : this( 1 )
		{
		}

		[Constructable]
		public DwarvenIngot( int amount ) : base( CraftResource.Dwarven, amount )
		{
		}

		public DwarvenIngot( Serial serial ) : base( serial )
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

	public class XormiteIngot : BaseIngot
	{
		[Constructable]
		public XormiteIngot() : this( 1 )
		{
		}

		[Constructable]
		public XormiteIngot( int amount ) : base( CraftResource.Xormite, amount )
		{
		}

		public XormiteIngot( Serial serial ) : base( serial )
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

	public class ObsidianIngot : BaseIngot
	{
		[Constructable]
		public ObsidianIngot() : this( 1 )
		{
		}

		[Constructable]
		public ObsidianIngot( int amount ) : base( CraftResource.Obsidian, amount )
		{
		}

		public ObsidianIngot( Serial serial ) : base( serial )
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

	public class NepturiteIngot : BaseIngot
	{
		[Constructable]
		public NepturiteIngot() : this( 1 )
		{
		}

		[Constructable]
		public NepturiteIngot( int amount ) : base( CraftResource.Nepturite, amount )
		{
		}

		public NepturiteIngot( Serial serial ) : base( serial )
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

	public class AgriniumIngot : BaseIngot
	{
		[Constructable]
		public AgriniumIngot() : this( 1 )
		{
		}

		[Constructable]
		public AgriniumIngot( int amount ) : base( CraftResource.Agrinium, amount )
		{
		}

		public AgriniumIngot( Serial serial ) : base( serial )
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

	public class BeskarIngot : BaseIngot
	{
		[Constructable]
		public BeskarIngot() : this( 1 )
		{
		}

		[Constructable]
		public BeskarIngot( int amount ) : base( CraftResource.Beskar, amount )
		{
		}

		public BeskarIngot( Serial serial ) : base( serial )
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

	public class CarboniteIngot : BaseIngot
	{
		[Constructable]
		public CarboniteIngot() : this( 1 )
		{
		}

		[Constructable]
		public CarboniteIngot( int amount ) : base( CraftResource.Carbonite, amount )
		{
		}

		public CarboniteIngot( Serial serial ) : base( serial )
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

	public class CortosisIngot : BaseIngot
	{
		[Constructable]
		public CortosisIngot() : this( 1 )
		{
		}

		[Constructable]
		public CortosisIngot( int amount ) : base( CraftResource.Cortosis, amount )
		{
		}

		public CortosisIngot( Serial serial ) : base( serial )
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

	public class DurasteelIngot : BaseIngot
	{
		[Constructable]
		public DurasteelIngot() : this( 1 )
		{
		}

		[Constructable]
		public DurasteelIngot( int amount ) : base( CraftResource.Durasteel, amount )
		{
		}

		public DurasteelIngot( Serial serial ) : base( serial )
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

	public class DuriteIngot : BaseIngot
	{
		[Constructable]
		public DuriteIngot() : this( 1 )
		{
		}

		[Constructable]
		public DuriteIngot( int amount ) : base( CraftResource.Durite, amount )
		{
		}

		public DuriteIngot( Serial serial ) : base( serial )
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

	public class FariumIngot : BaseIngot
	{
		[Constructable]
		public FariumIngot() : this( 1 )
		{
		}

		[Constructable]
		public FariumIngot( int amount ) : base( CraftResource.Farium, amount )
		{
		}

		public FariumIngot( Serial serial ) : base( serial )
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

	public class LaminasteelIngot : BaseIngot
	{
		[Constructable]
		public LaminasteelIngot() : this( 1 )
		{
		}

		[Constructable]
		public LaminasteelIngot( int amount ) : base( CraftResource.Laminasteel, amount )
		{
		}

		public LaminasteelIngot( Serial serial ) : base( serial )
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

	public class NeuraniumIngot : BaseIngot
	{
		[Constructable]
		public NeuraniumIngot() : this( 1 )
		{
		}

		[Constructable]
		public NeuraniumIngot( int amount ) : base( CraftResource.Neuranium, amount )
		{
		}

		public NeuraniumIngot( Serial serial ) : base( serial )
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

	public class PhrikIngot : BaseIngot
	{
		[Constructable]
		public PhrikIngot() : this( 1 )
		{
		}

		[Constructable]
		public PhrikIngot( int amount ) : base( CraftResource.Phrik, amount )
		{
		}

		public PhrikIngot( Serial serial ) : base( serial )
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

	public class PromethiumIngot : BaseIngot
	{
		[Constructable]
		public PromethiumIngot() : this( 1 )
		{
		}

		[Constructable]
		public PromethiumIngot( int amount ) : base( CraftResource.Promethium, amount )
		{
		}

		public PromethiumIngot( Serial serial ) : base( serial )
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

	public class QuadraniumIngot : BaseIngot
	{
		[Constructable]
		public QuadraniumIngot() : this( 1 )
		{
		}

		[Constructable]
		public QuadraniumIngot( int amount ) : base( CraftResource.Quadranium, amount )
		{
		}

		public QuadraniumIngot( Serial serial ) : base( serial )
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

	public class SongsteelIngot : BaseIngot
	{
		[Constructable]
		public SongsteelIngot() : this( 1 )
		{
		}

		[Constructable]
		public SongsteelIngot( int amount ) : base( CraftResource.Songsteel, amount )
		{
		}

		public SongsteelIngot( Serial serial ) : base( serial )
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

	public class TitaniumIngot : BaseIngot
	{
		[Constructable]
		public TitaniumIngot() : this( 1 )
		{
		}

		[Constructable]
		public TitaniumIngot( int amount ) : base( CraftResource.Titanium, amount )
		{
		}

		public TitaniumIngot( Serial serial ) : base( serial )
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

	public class TrimantiumIngot : BaseIngot
	{
		[Constructable]
		public TrimantiumIngot() : this( 1 )
		{
		}

		[Constructable]
		public TrimantiumIngot( int amount ) : base( CraftResource.Trimantium, amount )
		{
		}

		public TrimantiumIngot( Serial serial ) : base( serial )
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

	public class XonoliteIngot : BaseIngot
	{
		[Constructable]
		public XonoliteIngot() : this( 1 )
		{
		}

		[Constructable]
		public XonoliteIngot( int amount ) : base( CraftResource.Xonolite, amount )
		{
		}

		public XonoliteIngot( Serial serial ) : base( serial )
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