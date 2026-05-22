using System;
using System.Collections.Generic;
using Server.Items; 
using Server.Network;
using Server.ContextMenus;

namespace Server.Mobiles 
{ 
	public class InnKeeper : BaseVendor 
	{ 
		private List<SBInfo> m_SBInfos = new List<SBInfo>(); 
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } } 

		[Constructable]
		public InnKeeper() : base( "the innkeeper" ) 
		{
			Item candle = new HeldLight();
			candle.Name = "candle";
			candle.ItemID = 0xA0F;
			candle.Light = LightType.Circle150;
			AddItem( candle );
		} 

		public override void InitSBInfo( Mobile m )
		{
			m_Merchant = m;
			m_SBInfos.Add( new MyStock() );
		}

		public class MyStock: SBInfo
		{
			private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
			private IShopSellInfo m_SellInfo = new InternalSellInfo();

			public MyStock()
			{
			}

			public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
			public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

			public class InternalBuyInfo : List<GenericBuyInfo>
			{
				public InternalBuyInfo()
				{
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.None,		ItemSalesInfo.Material.None,		ItemSalesInfo.Market.Inn,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.Tavern,		ItemSalesInfo.Material.None,		ItemSalesInfo.Market.Cook,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.Supply,		ItemSalesInfo.Material.None,		ItemSalesInfo.Market.Cook,		ItemSalesInfo.World.None,	null	 );
				}
			}

			public class InternalSellInfo : GenericSellInfo
			{
				public InternalSellInfo()
				{
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.None,		ItemSalesInfo.Material.None,		ItemSalesInfo.Market.Inn,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.Tavern,		ItemSalesInfo.Material.None,		ItemSalesInfo.Market.Cook,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.Supply,		ItemSalesInfo.Material.None,		ItemSalesInfo.Market.Cook,		ItemSalesInfo.World.None,	null	 );
				}
			}
		}

		public override bool HandlesOnSpeech( Mobile from )
		{
			if ( from.InRange( this.Location, 12 ) )
				return true;

			return base.HandlesOnSpeech( from );
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			if ( !e.Handled && e.Mobile.InRange( this.Location, 12 ) && e.Mobile is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)(e.Mobile);
                bool isMatch = false;
				bool canOpen = false;
				BankBox cont = pm.FindBankNoCreate();

				if ( e.Speech.Contains( "rent" ) )
                    isMatch = true;

                if (!isMatch)
                    return;

                e.Handled = true;
				InnRoom inn = e.Mobile.InnRoom;
				if ( inn != null && cont != null )
				{
					if ( pm.InnTime > DateTime.Now )
						canOpen = true;
					else if ( cont.ConsumeTotal( typeof( Gold ), RoomCost( pm ) ) )
					{
						canOpen = true;
						pm.InnTime = DateTime.Now + TimeSpan.FromDays( 7.0 );
					}
					else
					{
						this.SayTo( pm, "Please give me " + RoomCost( pm ) + " gold for a room." );
						pm.SendMessage( "Give the innkeeper " + RoomCost( pm ) + " gold, or put that amount in the bank." );
					}

					if ( canOpen )
						inn.Open();
				}
			}

			base.OnSpeech( e );
		}

		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive )
				list.Add( new OpenInnEntry( from, this ) );

			base.AddCustomContextEntries( from, list );
		}

		///////////////////////////////////////////////////////////////////////////
		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			list.Add( new SpeechGumpEntry( from, this ) );
		}

		public override bool TryTalk( PlayerMobile from )
		{
			if ( from == null )
				return false;

			new SpeechGumpEntry( from, this ).OnClick();
			return true;
		}

		public class SpeechGumpEntry : ContextMenuEntry
		{
			private Mobile m_Mobile;
			private Mobile m_Giver;
			
			public SpeechGumpEntry( Mobile from, Mobile giver ) : base( 6146, 3 )
			{
				m_Mobile = from;
				m_Giver = giver;
			}

			public override void OnClick()
			{
			    if( !( m_Mobile is PlayerMobile ) )
				return;
				
				PlayerMobile mobile = (PlayerMobile) m_Mobile;
					m_Giver.SayTo( m_Mobile, "If you want to rent a room, it will cost you " + RoomCost( mobile ) + " gold per week." );
            }
        }
		///////////////////////////////////////////////////////////////////////////

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is Gold && from.InnRoom != null )
			{
				InnRoom inn = from.InnRoom;
				inn.DropItem( dropped );
				this.PrivateOverheadMessage(MessageType.Regular, 1153, false, "Thank you. You can now try to rent a room.", from.NetState);
				return true;
			}

			return base.OnDragDrop( from, dropped );
		}

		public static int RoomCost( PlayerMobile m )
		{
			InnRoom inn = m.InnRoom;

			if ( inn != null )
			{
				int stored = inn.TotalStored;
				int cost = stored + 10;
				return cost;
			}

			return 0;
		}

		public InnKeeper( Serial serial ) : base( serial ) 
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