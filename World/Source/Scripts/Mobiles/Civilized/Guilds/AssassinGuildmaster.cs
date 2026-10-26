using System;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;

namespace Server.Mobiles
{
	public class AssassinGuildmaster : BaseGuildmaster
	{
		public override NpcGuild NpcGuild{ get{ return NpcGuild.AssassinsGuild; } }

		public override string TalkGumpTitle{ get{ return "Death and Taxes"; } }
		public override string TalkGumpSubject{ get{ return "Assassin"; } }

		[Constructable]
		public AssassinGuildmaster() : base( "assassin" )
		{
			SetSkill( SkillName.Searching, 75.0, 98.0 );
			SetSkill( SkillName.Hiding, 65.0, 88.0 );
			SetSkill( SkillName.Fencing, 75.0, 98.0 );
			SetSkill( SkillName.Stealth, 85.0, 100.0 );
			SetSkill( SkillName.Poisoning, 85.0, 100.0 );
		}

		public override void InitSBInfo( Mobile m )
		{
			m_Merchant = m;
			SBInfos.Add( new MyStock() );
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
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.All,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Assassin,		ItemSalesInfo.World.None,	null	 );
				}
			}

			public class InternalSellInfo : GenericSellInfo
			{
				public InternalSellInfo()
				{
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.All,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Assassin,		ItemSalesInfo.World.None,	null	 );
				}
			}
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			switch ( Utility.RandomMinMax( 0, 3 ) )
			{
				case 0: AddItem( new Server.Items.ClothCowl( 2411 ) ); break;
				case 1: AddItem( new Server.Items.ClothHood( 2411 ) ); break;
				case 2: AddItem( new Server.Items.FancyHood( 2411 ) ); break;
				case 3: AddItem( new Server.Items.HoodedMantle( 2411 ) ); break;
			}

			AddItem( new Server.Items.Dagger() );
		}

		///////////////////////////////////////////////////////////////////////////
		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list ) 
		{ 
			base.GetContextMenuEntries( from, list ); 
			if ( MySettings.S_Bribery >= 1000 )
				list.Add( new HireEntry( from, this ) );
		} 

		private class HireEntry : ContextMenuEntry
		{
			private AssassinGuildmaster m_Giver;
			private Mobile m_From;

			public HireEntry( Mobile from, AssassinGuildmaster giver ) : base( 6120, 12 )
			{
				m_From = from;
				m_Giver = giver;
				Enabled = m_Giver.CheckVendorAccess( from, true ); // Bribery is open to non-members
			}

			public override void OnClick()
			{
			    if( !( m_From is PlayerMobile ) )
				return;
				
				m_Giver.Bribery( m_From );
			}
		}

        public void Bribery( Mobile from )
        {
            if (Deleted || !from.CheckAlive())
                return;

			PlayerMobile pm = (PlayerMobile)from;
			int cost = MySettings.S_Bribery;
				if ( pm.NpcGuild == NpcGuild.AssassinsGuild ){ cost = (int)(cost/2); }
			Container packs = from.Backpack;
			bool paid = false;

			if ( pm.Fugitive == 1 )
			{
				SayTo(from, "You are a bit too famous in the land to pursuade the guards to forget your crimes.");
				paid = true;
			}
			else if ( from.Kills < 1 )
			{
				SayTo(from, "You are not guilty of any murders.");
				paid = true;
			}
			else if ( packs.ConsumeTotal(typeof(Gold), cost) )
			{
				SayTo(from, "I will use your " + cost.ToString() + " gold to pursuade the guards to look the other way.");
				from.SendMessage(String.Format("You pay {0} gold.", cost));
				from.Kills = from.Kills - 1;
				paid = true;
			}
			else
			{
				Container cont = from.FindBankNoCreate();
				if ( cont != null && cont.ConsumeTotal( typeof( Gold ), cost ) )
				{
					SayTo(from, "I will use  " + cost.ToString() + " gold from your bank box to pursuade the guards to look the other way.");
					from.SendMessage(String.Format("You pay {0} gold from your bank box.", cost));
					from.Kills = from.Kills - 1;
					paid = true;
				}
			}

			if ( !paid )
			{
				SayTo(from, "I would require " + cost.ToString() + " gold to bribe the guards.");
			}
        }

		///////////////////////////////////////////////////////////////////////////

		public AssassinGuildmaster( Serial serial ) : base( serial )
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