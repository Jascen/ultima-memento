using Server.Targeting;
using Server.Utilities;

namespace Server.Items
{
	public class Wool : Item, IDyable
	{
		public override string DefaultDescription{ get{ return "You can use these on a spinning wheel, which will produce spools of string."; } }

		[Constructable]
		public Wool() : this( 1 )
		{
		}

		[Constructable]
		public Wool( int amount ) : base( 0xDF8 )
		{
			Stackable = true;
			Weight = 4.0;
			Amount = amount;
		}

		public Wool( Serial serial ) : base( serial )
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
				from.SendLocalizedMessage( 502655 ); // What spinning wheel do you wish to spin this on?
				if ( Utility.RandomDouble() < 0.1 ) from.SendMessage("Target yourself to automatically choose a nearby target");
				from.Target = new PickWheelTarget( this, from );
			}
			else
			{
				from.SendLocalizedMessage( 502138 ); // That is too far away for you to use
			}
		}

		public static void OnSpun( ISpinningWheel wheel, Mobile from, Item yarn )
		{
			if ( yarn != null )
			{
				Item item = new SpoolOfThread( (yarn.Amount * 3 ) );
				item.Hue = yarn.Hue;
				yarn.Delete();

				from.AddToBackpack( item );
				from.SendLocalizedMessage( 1010576 ); // You put the balls of yarn in your backpack.
			}
		}

		private class PickWheelTarget : Target
		{
			private readonly Wool m_Wool;
			private readonly Mobile m_From;

			public PickWheelTarget( Wool wool, Mobile from ) : base( 3, false, TargetFlags.None )
			{
				m_Wool = wool;
				m_From = from;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Wool.Deleted )
					return;

				ISpinningWheel wheel = targeted as ISpinningWheel;

				if ( wheel == null && targeted is AddonComponent )
					wheel = ((AddonComponent)targeted).Addon as ISpinningWheel;

				if ( wheel == null && targeted == m_From )
					wheel = WorldUtilities.GetNearbyItem<Item>( from, Range, item => item is ISpinningWheel) as ISpinningWheel;

				if ( wheel is Item )
				{
					Item item = (Item)wheel;

					if ( !m_Wool.IsChildOf( from.Backpack ) )
					{
						from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
					}
					else if ( wheel.Spinning )
					{
						from.SendLocalizedMessage( 502656 ); // That spinning wheel is being used.
					}
					else
					{
						if ( m_Wool is TaintedWool )	wheel.BeginSpin( new SpinCallback( TaintedWool.OnSpun ), from, m_Wool );
						else wheel.BeginSpin( new SpinCallback( Wool.OnSpun ), from, m_Wool );
					}
				}
				else
				{
					from.SendLocalizedMessage( 502658 ); // Use that on a spinning wheel.
				}
			}
		}
	}
	public class TaintedWool : Wool
	{
		[Constructable]
		public TaintedWool() : this( 1 )
		{
		}
		
		[Constructable]
		public TaintedWool( int amount ) : base( 0x101F )
		{
			Stackable = true;
			Weight = 4.0;
			Amount = amount;
		}

		public TaintedWool( Serial serial ) : base( serial )
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
		
		new public static void OnSpun( ISpinningWheel wheel, Mobile from, Item yarn )
		{
			if ( yarn != null )
			{
				Item item = new SpoolOfThread( yarn.Amount );
				item.Hue = yarn.Hue;
				yarn.Delete();

				from.AddToBackpack( item );
				from.SendLocalizedMessage( 1010574 ); // You put a ball of yarn in your backpack.
			}
		}
	}
}