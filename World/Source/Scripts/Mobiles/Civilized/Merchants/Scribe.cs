using System.Collections.Generic;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
	public class Scribe : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }
		public override bool IsBlackMarket { get { return true; } }

		public override string TalkGumpTitle{ get{ return "The Written Word"; } }
		public override string TalkGumpSubject{ get{ return "Scribe"; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.LibrariansGuild; } }

		[Constructable]
		public Scribe() : base( "the scribe" )
		{
			SetSkill( SkillName.Psychology, 60.0, 83.0 );
			SetSkill( SkillName.Inscribe, 90.0, 100.0 );
			SetSkill( SkillName.Mercantile, 65.0, 88.0 );
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
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.All,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Scribe,		ItemSalesInfo.World.None,	null	 );
				}
			}

			public class InternalSellInfo : GenericSellInfo
			{
				public InternalSellInfo()
				{
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.All,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Scribe,		ItemSalesInfo.World.None,	null	 );
				}
			}
		}

		public override void UpdateBlackMarket()
		{
			base.UpdateBlackMarket();

			if ( IsBlackMarket && MyServerSettings.BlackMarket() )
			{
				int v=8; while ( v > 0 ){ v--;
				ItemInformation.BlackMarketList( this, ItemSalesInfo.Category.All,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.Orient,	typeof( BookOfBushido )	 );
				ItemInformation.BlackMarketList( this, ItemSalesInfo.Category.All,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.None,	typeof( BookOfChivalry )	 );
				ItemInformation.BlackMarketList( this, ItemSalesInfo.Category.All,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.Orient,	typeof( BookOfNinjitsu )	 );
				ItemInformation.BlackMarketList( this, ItemSalesInfo.Category.All,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.None,	typeof( ElementalSpellbook )	 );
				ItemInformation.BlackMarketList( this, ItemSalesInfo.Category.All,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.Orient,	typeof( MysticSpellbook )	 );
				ItemInformation.BlackMarketList( this, ItemSalesInfo.Category.All,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.None,	typeof( NecromancerSpellbook )	 );
				ItemInformation.BlackMarketList( this, ItemSalesInfo.Category.All,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.None,	typeof( SongBook )	 );
				ItemInformation.BlackMarketList( this, ItemSalesInfo.Category.All,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.None,	typeof( Spellbook )	 );
				}
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is Gold )
			{
				string sMessage = "";

				if ( dropped.Amount == 500 )
				{
					if ( from.Skills[SkillName.Inscribe].Value >= 30 )
					{
						if ( Server.Misc.Research.AlreadyHasBag( from ) )
						{
							this.PublicOverheadMessage( MessageType.Regular, 0, false, string.Format ( "Here. You already have a pack." ) ); 
						}
						else
						{
							ResearchBag bag = new ResearchBag();
							from.PlaySound( 0x2E6 );
							Server.Misc.Research.SetupBag( from, bag );
							from.AddToBackpack( bag );
							this.PublicOverheadMessage( MessageType.Regular, 0, false, string.Format ( "Good luck with your research." ) ); 
							dropped.Delete();
						}
					}
					else
					{
						sMessage = "You need to be a neophyte scribe before I sell that to you.";
						from.AddToBackpack ( dropped );
					}
				}
				else
				{
					sMessage = "You look like you need this more than I do.";
					from.AddToBackpack ( dropped );
				}

				this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sMessage, from.NetState);
			}
			else if ( dropped is SmallHollowBook )
			{
				dropped.ItemID = 0x56F9;
				from.PlaySound( 0x249 );
				from.AddToBackpack ( dropped );
				this.PrivateOverheadMessage(MessageType.Regular, 1153, false, "I have rebound your book.", from.NetState);
			}
			else if ( dropped is LargeHollowBook )
			{
				dropped.ItemID = 0x5703;
				from.PlaySound( 0x249 );
				from.AddToBackpack ( dropped );
				this.PrivateOverheadMessage(MessageType.Regular, 1153, false, "I have rebound your book.", from.NetState);
			}
			else if ( dropped is Runebook )
			{
				((Runebook)dropped).ChangeGraphic( this, from, (Runebook)dropped );
			}

			return base.OnDragDrop( from, dropped );
		}

		public Scribe( Serial serial ) : base( serial )
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