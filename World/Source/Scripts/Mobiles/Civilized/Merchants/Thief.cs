using System;
using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.ContextMenus;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;

namespace Server.Mobiles
{
	public class Thief : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		public override string TalkGumpTitle{ get{ return "The Art Of Thievery"; } }
		public override string TalkGumpSubject{ get{ return "Thief"; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.ThievesGuild; } }

		[Constructable]
		public Thief() : base( "the thief" )
		{
			SetSkill( SkillName.Fencing, 55.0, 78.0 );
			SetSkill( SkillName.Searching, 65.0, 88.0 );
			SetSkill( SkillName.Hiding, 45.0, 68.0 );
			SetSkill( SkillName.RemoveTrap, 65.0, 88.0 );
			SetSkill( SkillName.Lockpicking, 60.0, 83.0 );
			SetSkill( SkillName.Snooping, 65.0, 88.0 );
			SetSkill( SkillName.Stealing, 65.0, 88.0 );
			SetSkill( SkillName.Stealth, 65.0, 88.0 );
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
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.None,		ItemSalesInfo.Material.None,		ItemSalesInfo.Market.Thief,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.Book,		ItemSalesInfo.Material.None,		ItemSalesInfo.Market.Thief,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.All,			ItemSalesInfo.Material.All,			ItemSalesInfo.Market.All,		ItemSalesInfo.World.None,	typeof( DisguiseKit )	 );
				}
			}

			public class InternalSellInfo : GenericSellInfo
			{
				public InternalSellInfo()
				{
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.None,		ItemSalesInfo.Material.None,		ItemSalesInfo.Market.Thief,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.Book,		ItemSalesInfo.Material.None,		ItemSalesInfo.Market.Thief,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.All,			ItemSalesInfo.Material.All,			ItemSalesInfo.Market.All,		ItemSalesInfo.World.None,	typeof( DisguiseKit )	 );
				}
			}
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			int color = Utility.RandomNeutralHue();
			switch ( Utility.RandomMinMax( 0, 5 ) )
			{
				case 0: AddItem( new Server.Items.Bandana( color ) ); break;
				case 1: AddItem( new Server.Items.SkullCap( color ) ); break;
				case 2: AddItem( new Server.Items.ClothCowl( color ) ); AddItem( new Server.Items.Cloak( color ) ); break;
				case 3: AddItem( new Server.Items.ClothHood( color ) ); AddItem( new Server.Items.Cloak( color ) ); break;
				case 4: AddItem( new Server.Items.FancyHood( color ) ); AddItem( new Server.Items.Cloak( color ) ); break;
				case 5: AddItem( new Server.Items.HoodedMantle( color ) ); AddItem( new Server.Items.Cloak( color ) ); break;
			}
		}

		private class FixEntry : ContextMenuEntry
		{
			private Thief m_Thief;
			private Mobile m_From;

			public FixEntry( Thief Thief, Mobile from ) : base( 6120, 12 )
			{
				m_Thief = Thief;
				m_From = from;
				Enabled = m_Thief.CheckVendorAccess( from );
			}

			public override void OnClick()
			{
				m_Thief.BeginServices( m_From );
			}
		}

		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive && !from.Blessed )
			{
				list.Add( new FixEntry( this, from ) );
			}

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
				SayTo(from, "Since you are begging, do you still want me to unlock a box? It will only cost you " + nCost.ToString() + ".");
			}
			else { SayTo(from, "If you want me to unlock a box, it will cost you " + nCost.ToString() + " gold."); }

            from.Target = new RepairTarget(this);
        }

        private class RepairTarget : Target
        {
            private Thief m_Thief;

            public RepairTarget(Thief thief) : base(12, false, TargetFlags.None)
            {
                m_Thief = thief;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is LockableContainer && from.Backpack != null && targeted is Item && !((Item)targeted).VirtualContainer )
                {
					LockableContainer box = (LockableContainer)targeted;
                    Container pack = from.Backpack;

                    int toConsume = 1000;
					if ( BeggingPose(from) > 0 ) // LET US SEE IF THEY ARE BEGGING
					{
						toConsume = toConsume - (int)( ( from.Skills[SkillName.Begging].Value * 0.005 ) * toConsume );
					}

                    if (toConsume == 0)
                        return;

                    if (pack.ConsumeTotal(typeof(Gold), toConsume))
                    {
						if ( BeggingPose(from) > 0 ){ Titles.AwardKarma( from, -BeggingKarma( from ), true ); } // DO ANY KARMA LOSS
                        m_Thief.SayTo(from, "That is now unlocked.");
                        from.SendMessage(String.Format("You pay {0} gold.", toConsume));
                        Effects.PlaySound(from.Location, from.Map, 0x241);
						box.Locked = false;
						box.DisableTrap();
						box.LockLevel = 0;
						box.MaxLockLevel = 0;
						box.RequiredSkill = 0;
                    }
                    else
                    {
                        m_Thief.SayTo(from, "It would cost you {0} gold to have that unlocked.", toConsume);
                        from.SendMessage("You do not have enough gold.");
                    }
                }
				else
				{
					m_Thief.SayTo(from, "That does not need my services.");
				}
            }
        }

		public Thief( Serial serial ) : base( serial )
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