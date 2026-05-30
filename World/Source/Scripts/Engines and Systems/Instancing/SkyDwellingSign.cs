using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Instancing;
using Knives.TownHouses;

namespace Server.Engines.Instancing
{
	// A TownHouseSign that is auto-placed inside a freshly generated sky-dwelling
	// instance. Buying it claims that instance permanently for the player. It reuses
	// the standard sign's price, buyer requirements (CanBuyHouse) and confirm gump,
	// so the purchase looks and costs exactly like the non-instanced one -- but the
	// outcome is the player owning the instance they are already standing in. The
	// sign deletes itself once purchased. Per-player ownership lives in
	// SkyInstanceManager.
	[Flipable( 0xC0B, 0xC0C )]
	public class SkyDwellingSign : TownHouseSign
	{
		[Constructable]
		public SkyDwellingSign()
		{
			Name = "Purchase this Sky Dwelling";
		}

		public override void OnDoubleClick( Mobile m )
		{
			// Staff get the standard setup gump to configure price/requirements.
			if ( m.AccessLevel != AccessLevel.Player )
			{
				base.OnDoubleClick( m );
				return;
			}

			if ( !Visible )
				return;

			if ( SkyInstanceManager.OwnsDwelling( m ) )
			{
				m.SendMessage( "You already own this sky dwelling." );
				return;
			}

			if ( CanBuyHouse( m ) )
				new TownHouseConfirmGump( m, this );
			else
				m.SendMessage( "You cannot purchase a sky dwelling." );
		}

		public override void Purchase( Mobile m, bool sellitems )
		{
			if ( m == null )
				return;

			if ( SkyInstanceManager.OwnsDwelling( m ) )
			{
				m.SendMessage( "You already own this sky dwelling." );
				return;
			}

			if ( !CanBuyHouse( m ) )
			{
				m.SendMessage( "You cannot purchase a sky dwelling." );
				return;
			}

			int price = Free ? 0 : Price;

			if ( m.AccessLevel == AccessLevel.Player && !Server.Mobiles.Banker.Withdraw( m, price ) )
			{
				m.SendMessage( "You cannot afford this sky dwelling." );
				return;
			}

			if ( m.AccessLevel == AccessLevel.Player && price > 0 )
				m.SendLocalizedMessage( 1060398, price.ToString() ); // ~1_AMOUNT~ gold has been withdrawn from your bank box.

			if ( !SkyInstanceManager.Purchase( m ) )
			{
				m.SendMessage( "You already own this sky dwelling." );
				return;
			}

			// The buyer is already standing in the instance; the sign has done its
			// job, so it removes itself, leaving the dwelling theirs to decorate.
			m.SendMessage( "You have purchased this sky dwelling! It is now yours." );
			Delete();
		}

		public SkyDwellingSign( Serial serial ) : base( serial )
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
