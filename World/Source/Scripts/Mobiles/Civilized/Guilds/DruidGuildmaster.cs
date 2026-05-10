using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.ContextMenus;
using Server.Misc;
using Server.Mobiles;

namespace Server.Mobiles
{
	public class DruidGuildmaster : BaseGuildmaster
	{
		public override NpcGuild NpcGuild{ get{ return NpcGuild.DruidsGuild; } }

		public override string TalkGumpTitle{ get{ return "The Protectors Of The Forest"; } }
		public override string TalkGumpSubject{ get{ return "Druid"; } }

		[Constructable]
		public DruidGuildmaster() : base( "druid" )
		{
			SetSkill( SkillName.Herding, 80.0, 100.0 );
			SetSkill( SkillName.Camping, 80.0, 100.0 );
			SetSkill( SkillName.Cooking, 80.0, 100.0 );
			SetSkill( SkillName.Alchemy, 80.0, 100.0 );
			SetSkill( SkillName.Druidism, 85.0, 100.0 );
			SetSkill( SkillName.Taming, 90.0, 100.0 );
			SetSkill( SkillName.Veterinary, 90.0, 100.0 );

			AddItem( new LightSource() );
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
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.None,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Druid,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.Book,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Druid,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.Potion,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Druid,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.Resource,	ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Druid,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.Reagent,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Druid,		ItemSalesInfo.World.None,	null	 );
				}
			}

			public class InternalSellInfo : GenericSellInfo
			{
				public InternalSellInfo()
				{
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.None,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Druid,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.Book,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Druid,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.Potion,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Druid,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.Resource,	ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Druid,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.Reagent,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Druid,		ItemSalesInfo.World.None,	null	 );
				}
			}
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new DeerCap() );
			switch ( Utility.RandomMinMax( 1, 5 ) )
			{
				case 1: AddItem( new Server.Items.GnarledStaff() ); break;
				case 2: AddItem( new Server.Items.BlackStaff() ); break;
				case 3: AddItem( new Server.Items.WildStaff() ); break;
				case 4: AddItem( new Server.Items.QuarterStaff() ); break;
				case 5: AddItem( new Server.Items.ShepherdsCrook() ); break;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private class FixEntry : ContextMenuEntry
		{
			private DruidGuildmaster m_Druid;
			private Mobile m_From;

			public FixEntry( DruidGuildmaster DruidGuildmaster, Mobile from ) : base( 6120, 12 )
			{
				m_Druid = DruidGuildmaster;
				m_From = from;
				Enabled = m_Druid.CheckVendorAccess( from );
			}

			public override void OnClick()
			{
				m_Druid.BeginServices( m_From );
			}
		}

		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( CheckChattingAccess( from ) )
				list.Add( new FixEntry( this, from ) );

			base.AddCustomContextEntries( from, list );
		}

        public void BeginServices(Mobile from)
        {
            if ( Deleted || !from.Alive )
                return;

			int nCost = 1000;

			if ( BeggingPose(from) > 0 ) // LET US SEE IF THEY ARE BEGGING
			{
				nCost = nCost - (int)( ( from.Skills[SkillName.Begging].Value * 0.005 ) * nCost ); if ( nCost < 1 ){ nCost = 1; }
				SayTo(from, "Since you are begging, do you still want me to tend to your pack animal for up to 5 journeys, it will only cost you " + nCost.ToString() + " gold?");
			}
			else { SayTo(from, "If you want me to tend to your pack animal for up to 5 journeys, it will cost you " + nCost.ToString() + " gold."); }

            from.Target = new RepairTarget(this);
        }

        private class RepairTarget : Target
        {
            private DruidGuildmaster m_Druid;

            public RepairTarget(DruidGuildmaster druid) : base(12, false, TargetFlags.None)
            {
                m_Druid = druid;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is PackBeastItem && from.Backpack != null)
                {
                    PackBeastItem ball = targeted as PackBeastItem;
                    Container pack = from.Backpack;

                    int toConsume = 0;

					if ( ball.Charges < 50 )
                    {
						toConsume = 1000;

						if ( BeggingPose(from) > 0 ) // LET US SEE IF THEY ARE BEGGING
						{
							toConsume = toConsume - (int)( ( from.Skills[SkillName.Begging].Value * 0.005 ) * toConsume );
						}
                    }
                    else
                    {
						m_Druid.SayTo(from, "You pack animal has been tended to enough.");
                    }

                    if (toConsume == 0)
                        return;

                    if (pack.ConsumeTotal(typeof(Gold), toConsume))
                    {
						if ( BeggingPose(from) > 0 ){ Titles.AwardKarma( from, -BeggingKarma( from ), true ); } // DO ANY KARMA LOSS
                        m_Druid.SayTo(from, "Your pack animal is properly tended to.");
                        from.SendMessage(String.Format("You pay {0} gold.", toConsume));
                        Effects.PlaySound(from.Location, from.Map, 0x5C1);
						ball.Charges = ball.Charges + 5;
                    }
                    else
                    {
                        m_Druid.SayTo(from, "It would cost you {0} gold to have that pack animal tended to.", toConsume);
                        from.SendMessage("You do not have enough gold.");
                    }
                }
				else
				{
					m_Druid.SayTo(from, "That does not need my services.");
				}
            }
        }

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is HomePlants_Mushroom )
			{
				int TreeSap = dropped.Amount;
				string sMessage = "";

				if ( TreeSap > 19 )
				{
					sMessage = "Ahhh...this is generous of you. Here...have this as a token of the guild's gratitude.";
					PackBeastItem ball = new PackBeastItem();
					ball.PorterOwner = from;
					from.AddToBackpack ( ball );
				}
				else
				{
					sMessage = "Thank you for these. Mushrooms are something we often look for.";
				}

				this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sMessage, from.NetState);
				dropped.Delete();
			}
			else if ( dropped is PackBeastItem )
			{
				string sMessage = "";

				PackBeastItem ball = (PackBeastItem)dropped;

				if ( ball.PorterType == 291 ){ ball.ItemID = 0x2127; ball.PorterType = 292; ball.Hue = 0; sMessage = "You may like a pack llama instead." ; }
				else if ( ball.PorterType == 292 ){ ball.ItemID = 0x20DB; ball.PorterType = 23; ball.Hue = 0; sMessage = "You may like a pack brown bear instead." ; }
				else if ( ball.PorterType == 23 ){ ball.ItemID = 0x20CF; ball.PorterType = 177; ball.Hue = 0; sMessage = "You may like a pack black bear instead." ; }
				else if ( ball.PorterType == 177 ){ ball.ItemID = 0x20E1; ball.PorterType = 179; ball.Hue = 0; sMessage = "You may like a pack polar bear instead." ; }
				else if ( ball.PorterType == 179 ){ ball.ItemID = 0x2126; ball.PorterType = 291; ball.Hue = 0; sMessage = "You may like a pack horse instead." ; }

				sMessage = "You would perhaps like a different pack animal? " + sMessage;
				from.AddToBackpack ( ball );

				this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sMessage, from.NetState);
			}
			return base.OnDragDrop( from, dropped );
		}

		public virtual bool CheckResurrect( Mobile m )
		{
			return true;
		}

		private DateTime m_NextResurrect;
		private static TimeSpan ResurrectDelay = TimeSpan.FromSeconds( 2.0 );

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( !m.Frozen && DateTime.Now >= m_NextResurrect && InRange( m, 4 ) && !InRange( oldLocation, 4 ) && InLOS( m ) )
			{
				if ( m.IsDeadBondedPet )
				{
					m_NextResurrect = DateTime.Now + ResurrectDelay;

					if ( m.Map == null || !m.Map.CanFit( m.Location, 16, false, false ) )
					{
						Say("I sense a spirt of an animal...somewhere.");
					}
					else
					{
						BaseCreature bc = m as BaseCreature;
					
						bc.PlaySound( 0x214 );
						bc.FixedEffect( 0x376A, 10, 16 );

						bc.ResurrectPet();

						Say("Rise my friend. I wish I could save every unfortunate animal.");
					}
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public DruidGuildmaster( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}
	}
}