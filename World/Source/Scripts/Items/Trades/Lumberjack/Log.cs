using Server.Targeting;
using Server.Utilities;

namespace Server.Items
{
	public abstract class BaseLog : Item
	{
		public override string DefaultDescription{ get{ return "These logs can be used at a saw mill, which will cut them into boards. The boards can then be used for crafting."; } }

		public override Catalogs DefaultCatalog{ get{ return Catalogs.Crafting; } }

		public override double DefaultWeight
		{
			get { return 2; }
		}

		[Constructable]
		public BaseLog() : this( 1 )
		{
		}

		[Constructable]
		public BaseLog( int amount ) : this( CraftResource.RegularWood, amount )
		{
		}

		public abstract BaseWoodBoard GetLog();

		[Constructable]
		public BaseLog( CraftResource resource ) : this( resource, 1 )
		{
		}

		[Constructable]
		public BaseLog( CraftResource resource, int amount ) : base( 0x1BE0 )
		{
			Stackable = true;
			Amount = amount;
			m_Resource = resource;
			Hue = CraftResources.GetHue( resource );
			Name = CraftResources.GetTradeItemFullName( this, resource, true, false, null );
			Built = true;
		}

		public BaseLog( Serial serial ) : base( serial )
		{
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
			Name = CraftResources.GetTradeItemFullName( this, m_Resource, true, false, null );
			ItemID = 0x1BE0;
			Built = true;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;
			
			if ( from.InRange( this.GetWorldLocation(), 2 ) )
			{
				from.SendMessage( "Select the saw mill on which to cut the logs." );
				if ( Utility.RandomDouble() < 0.1 ) from.SendMessage("Target yourself to automatically choose a nearby target");
				from.Target = new InternalTarget( this, from );
			}
			else
			{
				from.SendMessage( "The logs are too far away." );
			}
		}

		private class InternalTarget : Target
		{
			private readonly BaseLog m_Log;
			private readonly Mobile m_From;

			public InternalTarget( BaseLog log, Mobile from ) :  base ( 2, false, TargetFlags.None )
			{
				m_Log = log;
				m_From = from;
			}

			private bool IsSawmill( object obj )
			{
				if ( obj is Item )
				{
					Item saw = (Item)obj;

					if ( saw.Name == "saw mill" && 
						( saw.ItemID == 1928 || saw.ItemID == 4525 || saw.ItemID == 7130 || saw.ItemID == 4530 || saw.ItemID == 7127 )
					   )
						return true;
					else
						return false;
				}

				return false;
			}

			private bool IsSawmillNearby(Mobile from, int range)
			{
				return WorldUtilities.HasNearbyItem<Item>( from, range, item => IsSawmill(item) );
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (m_Log.Deleted) return;

				if (!from.InRange(m_Log.GetWorldLocation(), 2))
				{
					from.SendMessage("The logs are too far away.");
					return;
				}

				if (!IsSawmill(targeted) && false == ( targeted == m_From && IsSawmillNearby( from, Range ) ) )
				{
					from.SendMessage("That is not a saw mill.");
					return;
				}

				double difficulty;
				switch (m_Log.Resource)
				{
					default: difficulty = 20.0; break;
					case CraftResource.AshTree: difficulty = 55.0; break;
					case CraftResource.CherryTree: difficulty = 60.0; break;
					case CraftResource.EbonyTree: difficulty = 65.0; break;
					case CraftResource.GoldenOakTree: difficulty = 70.0; break;
					case CraftResource.HickoryTree: difficulty = 75.0; break;
					case CraftResource.MahoganyTree: difficulty = 80.0; break;
					case CraftResource.DriftwoodTree: difficulty = 80.0; break;
					case CraftResource.OakTree: difficulty = 85.0; break;
					case CraftResource.PineTree: difficulty = 90.0; break;
					case CraftResource.GhostTree: difficulty = 90.0; break;
					case CraftResource.RosewoodTree: difficulty = 95.0; break;
					case CraftResource.WalnutTree: difficulty = 99.0; break;
					case CraftResource.PetrifiedTree: difficulty = 99.9; break;
					case CraftResource.ElvenTree: difficulty = 100.1; break;
				}

				double minSkill = difficulty - 25.0;
				double maxSkill = difficulty + 25.0;
				if (difficulty > 50.0 && difficulty > from.Skills[SkillName.Lumberjacking].Value || from.Skills[SkillName.Lumberjacking].Value < minSkill)
				{
					from.SendMessage("You have no idea how to best cut this type of wood! (You need to raise your lumberjacking)");
					return;
				}
				
				int lost = 0;
				int remaining = m_Log.Amount;
				while(0 < remaining)
				{
					if (maxSkill < from.Skills[SkillName.Lumberjacking].Value) break; // Short-circuit if they exceed the max skill

					if (!from.CheckTargetSkillExplicit(SkillName.Lumberjacking, targeted, minSkill, maxSkill))
						lost++;

					remaining--;
				}

				from.PlaySound(0x21C);

				int boards = m_Log.Amount - lost;
				if (0 < lost)
				{
					string message = boards == 0
						? "You try to cut the logs but ruin all of the wood."
						: "You try to cut the logs but ruin some of the wood.";
					from.SendMessage(message);
				}
				else
				{
					from.SendMessage("You cut the logs and put some boards in your backpack.");
				}

				if (0 < boards)
				{
					BaseWoodBoard wood = m_Log.GetLog();

					m_Log.Delete(); // Delete early to prevent going overweight

					wood.Amount = boards * 2;
					from.AddToBackpack(wood);
				}
				else
				{
					m_Log.Delete();
				}
			}
		}
	}

	public class Log : BaseLog
	{
		[Constructable]
		public Log() : this( 1 )
		{
		}
		[Constructable]
		public Log( int amount ) : base( CraftResource.RegularWood, amount )
		{
		}
		public Log( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		public override BaseWoodBoard GetLog()
		{
			return new Board();
		}
	}
	public class AshLog : BaseLog
	{
		[Constructable]
		public AshLog() : this( 1 )
		{
		}
		[Constructable]
		public AshLog( int amount ) : base( CraftResource.AshTree, amount )
		{
		}
		public AshLog( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		public override BaseWoodBoard GetLog()
		{
			return new AshBoard();
		}
	}
	public class CherryLog : BaseLog
	{
		[Constructable]
		public CherryLog() : this( 1 )
		{
		}
		[Constructable]
		public CherryLog( int amount ) : base( CraftResource.CherryTree, amount )
		{
		}
		public CherryLog( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		public override BaseWoodBoard GetLog()
		{
			return new CherryBoard();
		}
	}
	public class EbonyLog : BaseLog
	{
		[Constructable]
		public EbonyLog() : this( 1 )
		{
		}
		[Constructable]
		public EbonyLog( int amount ) : base( CraftResource.EbonyTree, amount )
		{
		}
		public EbonyLog( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		public override BaseWoodBoard GetLog()
		{
			return new EbonyBoard();
		}
	}
	public class GoldenOakLog : BaseLog
	{
		[Constructable]
		public GoldenOakLog() : this( 1 )
		{
		}
		[Constructable]
		public GoldenOakLog( int amount ) : base( CraftResource.GoldenOakTree, amount )
		{
		}
		public GoldenOakLog( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		public override BaseWoodBoard GetLog()
		{
			return new GoldenOakBoard();
		}
	}
	public class HickoryLog : BaseLog
	{
		[Constructable]
		public HickoryLog() : this( 1 )
		{
		}
		[Constructable]
		public HickoryLog( int amount ) : base( CraftResource.HickoryTree, amount )
		{
		}
		public HickoryLog( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		public override BaseWoodBoard GetLog()
		{
			return new HickoryBoard();
		}
	}
	public class MahoganyLog : BaseLog
	{
		[Constructable]
		public MahoganyLog() : this( 1 )
		{
		}
		[Constructable]
		public MahoganyLog( int amount ) : base( CraftResource.MahoganyTree, amount )
		{
		}
		public MahoganyLog( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		public override BaseWoodBoard GetLog()
		{
			return new MahoganyBoard();
		}
	}
	public class OakLog : BaseLog
	{
		[Constructable]
		public OakLog() : this( 1 )
		{
		}
		[Constructable]
		public OakLog( int amount ) : base( CraftResource.OakTree, amount )
		{
		}
		public OakLog( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		public override BaseWoodBoard GetLog()
		{
			return new OakBoard();
		}
	}
	public class PineLog : BaseLog
	{
		[Constructable]
		public PineLog() : this( 1 )
		{
		}
		[Constructable]
		public PineLog( int amount ) : base( CraftResource.PineTree, amount )
		{
		}
		public PineLog( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		public override BaseWoodBoard GetLog()
		{
			return new PineBoard();
		}
	}
	public class RosewoodLog : BaseLog
	{
		[Constructable]
		public RosewoodLog() : this( 1 )
		{
		}
		[Constructable]
		public RosewoodLog( int amount ) : base( CraftResource.RosewoodTree, amount )
		{
		}
		public RosewoodLog( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		public override BaseWoodBoard GetLog()
		{
			return new RosewoodBoard();
		}
	}
	public class WalnutLog : BaseLog
	{
		[Constructable]
		public WalnutLog() : this( 1 )
		{
		}
		[Constructable]
		public WalnutLog( int amount ) : base( CraftResource.WalnutTree, amount )
		{
		}
		public WalnutLog( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		public override BaseWoodBoard GetLog()
		{
			return new WalnutBoard();
		}
	}
	public class DriftwoodLog : BaseLog
	{
		[Constructable]
		public DriftwoodLog() : this( 1 )
		{
		}
		[Constructable]
		public DriftwoodLog( int amount ) : base( CraftResource.DriftwoodTree, amount )
		{
		}
		public DriftwoodLog( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		public override BaseWoodBoard GetLog()
		{
			return new DriftwoodBoard();
		}
	}
	public class GhostLog : BaseLog
	{
		[Constructable]
		public GhostLog() : this( 1 )
		{
		}
		[Constructable]
		public GhostLog( int amount ) : base( CraftResource.GhostTree, amount )
		{
		}
		public GhostLog( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		public override BaseWoodBoard GetLog()
		{
			return new GhostBoard();
		}
	}
	public class PetrifiedLog : BaseLog
	{
		[Constructable]
		public PetrifiedLog() : this( 1 )
		{
		}
		[Constructable]
		public PetrifiedLog( int amount ) : base( CraftResource.PetrifiedTree, amount )
		{
		}
		public PetrifiedLog( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		public override BaseWoodBoard GetLog()
		{
			return new PetrifiedBoard();
		}
	}
	public class ElvenLog : BaseLog
	{
		[Constructable]
		public ElvenLog() : this( 1 )
		{
		}
		[Constructable]
		public ElvenLog( int amount ) : base( CraftResource.ElvenTree, amount )
		{
		}
		public ElvenLog( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		public override BaseWoodBoard GetLog()
		{
			return new ElvenBoard();
		}
	}
}