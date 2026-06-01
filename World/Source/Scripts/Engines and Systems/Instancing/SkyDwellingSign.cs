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
	// outcome is the player owning the instance they are already standing in.
	//
	// The sign is NOT removed on purchase: it converts into an owned "management"
	// sign that the owner double-clicks to set their dwelling public or private.
	// Per-player ownership lives in SkyDwellingInstanceType.
	[Flipable( 0xC0B, 0xC0C )]
	public class SkyDwellingSign : TownHouseSign
	{
		private bool m_Owned;
		private Serial m_OwnerSerial;

		public bool OwnedSign { get { return m_Owned; } }
		public Serial DwellingOwner { get { return m_OwnerSerial; } }

		[Constructable]
		public SkyDwellingSign()
		{
			Name = "Purchase this Sky Dwelling";
		}

		// Convert this sign into the owner's management sign (kept after purchase).
		public void SetOwned( Serial owner )
		{
			m_Owned = true;
			m_OwnerSerial = owner;
			Name = "Sky Dwelling Management";
			Visible = true;
			InvalidateProperties();
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

			if ( m_Owned )
			{
				// Post-purchase: only the owner manages it (public/private).
				if ( m.Serial != m_OwnerSerial )
				{
					Mobile owner = World.FindMobile( m_OwnerSerial );
					m.SendMessage( "This sky dwelling belongs to {0}.", owner != null ? owner.Name : "someone else" );
					return;
				}

				new SkyDwellingManagementGump( m );
				return;
			}

			if ( SkyDwellingInstanceType.Instance.OwnsDwelling( m ) )
			{
				m.SendMessage( "You already own a sky dwelling." );
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

			if ( m_Owned )
				return;

			if ( SkyDwellingInstanceType.Instance.OwnsDwelling( m ) )
			{
				m.SendMessage( "You already own a sky dwelling." );
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

			if ( !SkyDwellingInstanceType.Instance.Purchase( m ) )
			{
				m.SendMessage( "You already own a sky dwelling." );
				return;
			}

			// The buyer now owns the instance they are standing in. The sign stays as
			// their management control (double-click to set public/private).
			m.SendMessage( "You have purchased this sky dwelling! Double-click this sign to set it public or private." );
			SetOwned( m.Serial );
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );

			if ( m_Owned )
			{
				Mobile owner = World.FindMobile( m_OwnerSerial );
				list.Add( 1070722, owner != null
					? String.Format( "{0}'s sky dwelling -- double-click to manage", owner.Name )
					: "an owned sky dwelling" );
			}
		}

		public SkyDwellingSign( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
			writer.Write( (bool) m_Owned );
			writer.Write( (int) m_OwnerSerial );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version >= 1 )
			{
				m_Owned = reader.ReadBool();
				m_OwnerSerial = (Serial)reader.ReadInt();
			}
		}
	}
}
