using Server.Targeting;
using Server.Utilities;

namespace Server.Items
{
	public class Flax : Item
	{
		public override string DefaultDescription{ get{ return "You can use these on a spinning wheel, which will produce spools of string."; } }

		[Constructable]
		public Flax() : this( 1 )
		{
		}

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		[Constructable]
		public Flax( int amount ) : base( 0x1A9C )
		{
			Stackable = true;
			Amount = amount;
		}

		public Flax( Serial serial ) : base( serial )
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
				Item item = new SpoolOfThread( (yarn.Amount * 6 ) );
				item.Hue = yarn.Hue;
				yarn.Delete();

				from.AddToBackpack( item );
				from.SendLocalizedMessage( 1010577 ); // You put the spools of thread in your backpack.
			}
		}

		private class PickWheelTarget : Target
		{
			private readonly Flax m_Flax;
			private readonly Mobile m_From;

			public PickWheelTarget( Flax flax, Mobile from ) : base( 3, false, TargetFlags.None )
			{
				m_Flax = flax;
				m_From = from;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Flax.Deleted )
					return;

				ISpinningWheel wheel = targeted as ISpinningWheel;

				if ( wheel == null && targeted is AddonComponent )
					wheel = ((AddonComponent)targeted).Addon as ISpinningWheel;

				if ( wheel == null && targeted == m_From )
					wheel = WorldUtilities.GetNearbyItem<Item>( from, Range, item => item is ISpinningWheel) as ISpinningWheel;;

				if ( wheel is Item )
				{
					Item item = (Item)wheel;

					if ( !m_Flax.IsChildOf( from.Backpack ) )
					{
						from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
					}
					else if ( wheel.Spinning )
					{
						from.SendLocalizedMessage( 502656 ); // That spinning wheel is being used.
					}
					else
					{
						wheel.BeginSpin( new SpinCallback( Flax.OnSpun ), from, m_Flax );
					}
				}
				else
				{
					from.SendLocalizedMessage( 502658 ); // Use that on a spinning wheel.
				}
			}
		}
	}
}