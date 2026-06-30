using Server.Targeting;
using Server.Utilities;

namespace Server.Items
{
	public abstract class BaseClothMaterial : Item, IDyable
	{
		public override string DefaultDescription{ get{ return "You can use these on a loom, which will produce cloth you can use for tailoring."; } }

		public BaseClothMaterial( int itemID ) : this( itemID, 1 )
		{
		}

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		public BaseClothMaterial( int itemID, int amount ) : base( itemID )
		{
			Stackable = true;
			Amount = amount;
		}

		public BaseClothMaterial( Serial serial ) : base( serial )
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

		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;

			Hue = sender.DyedHue;

			return true;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( this.GetWorldLocation(), 2 ) )
			{
				from.SendLocalizedMessage( 500366 ); // Select a loom to use that on.
				if ( Utility.RandomDouble() < 0.1 ) from.SendMessage("Target yourself to automatically choose a nearby target");
				from.Target = new PickLoomTarget( this, from );
			}
			else
			{
				from.SendLocalizedMessage( 502138 ); // That is too far away for you to use
			}
		}

		private class PickLoomTarget : Target
		{
			private readonly BaseClothMaterial m_Material;
			private readonly Mobile m_From;

			public PickLoomTarget( BaseClothMaterial material, Mobile from ) : base( 3, false, TargetFlags.None )
			{
				m_Material = material;
				m_From = from;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Material.Deleted )
					return;

				ILoom loom = targeted as ILoom;

				if ( loom == null && targeted is AddonComponent )
					loom = ((AddonComponent)targeted).Addon as ILoom;

				if ( loom == null && targeted == m_From )
					loom = WorldUtilities.GetNearbyItem<Item>( from, Range, item => item is ILoom) as ILoom;

				if ( loom != null )
				{
					if ( !m_Material.IsChildOf( from.Backpack ) )
					{
						from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
					}
					else
					{
						int cycle = m_Material.Amount;
						int looms = loom.Phase;
						int amount = 0;

						bool sendMessage = false;

						while ( cycle > 0 )
						{
							cycle--;

							if ( looms >= 4 )
							{
								looms = 0;
								amount++;
								sendMessage = true;
							}
							else
							{
								looms++;
							}
						}

						m_Material.Delete();
						loom.Phase = looms;

						if ( sendMessage )
						{
							Item create = new Fabric(amount*50);
							create.Hue = m_Material.Hue;
							from.AddToBackpack( create );

							from.SendLocalizedMessage( 500368 ); // You create some cloth and put it in your backpack.
							if ( loom.Phase > 0 ){ from.SendMessage( "The loom still has some incomplete cloth started." ); }
						}
						else
						{
							from.SendMessage( "You don't have enough to create a bolt of cloth." );
						}
					}
				}
				else
				{
					from.SendLocalizedMessage( 500367 ); // Try using that on a loom.
				}
			}
		}
	}

	public class SpoolOfThread : BaseClothMaterial
	{
		[Constructable]
		public SpoolOfThread() : this( 1 )
		{
		}

		[Constructable]
		public SpoolOfThread( int amount ) : base( 0x543A, amount )
		{
			Name = "spool of string";
		}

		public SpoolOfThread( Serial serial ) : base( serial )
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
			ItemID = 0x543A;
			Name = "spool of string";
		}
	}
}