using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.ContextMenus;

namespace Server.Mobiles
{
	public class LibrarianGuildmaster : BaseGuildmaster
	{
		public override NpcGuild NpcGuild{ get{ return NpcGuild.LibrariansGuild; } }

		public override string TalkGumpTitle{ get{ return "The Writing On The Wall"; } }
		public override string TalkGumpSubject{ get{ return "Sage"; } }

		[Constructable]
		public LibrarianGuildmaster() : base( "librarian" )
		{
			SetSkill( SkillName.Inscribe, 60.0, 83.0 );
			SetSkill( SkillName.MagicResist, 65.0, 88.0 );
			SetSkill( SkillName.FistFighting, 60.0, 80.0 );
			SetSkill( SkillName.Mercantile, 64.0, 100.0 );
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

		///////////////////////////////////////////////////////////////////////////

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
							this.PublicOverheadMessage( MessageType.Regular, 0, false, string.Format ( "Good luck with your research." ) ); 
						}
						else
						{
							ResearchBag bag = new ResearchBag();
							from.PlaySound( 0x2E6 );
							Server.Misc.Research.SetupBag( from, bag );
							from.AddToBackpack( bag );
							this.PublicOverheadMessage( MessageType.Regular, 0, false, string.Format ( "Good luck with your research." ) ); 
						}
						dropped.Delete();
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

		private class FixEntry : ContextMenuEntry
		{
			private LibrarianGuildmaster m_Sage;
			private Mobile m_From;

			public FixEntry( LibrarianGuildmaster LibrarianGuildmaster, Mobile from ) : base( 6120, 12 )
			{
				m_Sage = LibrarianGuildmaster;
				m_From = from;
				Enabled = m_Sage.CheckVendorAccess( from );
			}

			public override void OnClick()
			{
				m_Sage.BeginServices( m_From );
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

			int nCost = 500;

			if ( BeggingPose(from) > 0 ) // LET US SEE IF THEY ARE BEGGING
			{
				nCost = nCost - (int)( ( from.Skills[SkillName.Begging].Value * 0.005 ) * nCost );
				SayTo(from, "Since you are begging, do you want me to decipher a note for " + nCost.ToString() + " gold?");
			}
			else { SayTo(from, "Do you want me to decipher a note for " + nCost.ToString() + " gold?"); }

            from.Target = new RepairTarget(this);
        }

        private class RepairTarget : Target
        {
            private LibrarianGuildmaster m_Sage;

            public RepairTarget(LibrarianGuildmaster mage) : base(12, false, TargetFlags.None)
            {
                m_Sage = mage;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
				if ( targeted is ScrollClue )
				{
					Container packs = from.Backpack;
					int nCost = 100;
					ScrollClue WhatIsIt = (ScrollClue)targeted;

					if ( BeggingPose(from) > 0 ) // LET US SEE IF THEY ARE BEGGING
					{
						nCost = nCost - (int)( ( from.Skills[SkillName.Begging].Value * 0.005 ) * nCost ); if ( nCost < 1 ){ nCost = 1; }
					}
					int toConsume = nCost;

                    if ( WhatIsIt.ScrollIntelligence == 0 )
                    {
                        m_Sage.SayTo( from, "That was already deciphered by someone." );
					}
                    else if (packs.ConsumeTotal(typeof(Gold), toConsume))
                    {
						if ( WhatIsIt.ScrollIntelligence >= 80 ){ WhatIsIt.Name = "diabolically coded parchment"; }
						else if ( WhatIsIt.ScrollIntelligence >= 70 ){ WhatIsIt.Name = "ingeniously coded parchment"; }
						else if ( WhatIsIt.ScrollIntelligence >= 60 ){ WhatIsIt.Name = "deviously coded parchment"; }
						else if ( WhatIsIt.ScrollIntelligence >= 50 ){ WhatIsIt.Name = "cleverly coded parchment"; }
						else if ( WhatIsIt.ScrollIntelligence >= 40 ){ WhatIsIt.Name = "adeptly coded parchment"; }
						else if ( WhatIsIt.ScrollIntelligence >= 30 ){ WhatIsIt.Name = "expertly coded parchment"; }
						else { WhatIsIt.Name = "plainly coded parchment"; }

						WhatIsIt.ScrollIntelligence = 0;
						WhatIsIt.InvalidateProperties();
                        from.SendMessage(String.Format("You pay {0} gold.", toConsume));
						m_Sage.SayTo(from, "Let me show you what this reads...");
						WhatIsIt.ScrollSolved = "Deciphered by " + m_Sage.Name + " the Librarian";
						from.PlaySound( 0x249 );
						WhatIsIt.InvalidateProperties();
                    }
                    else
                    {
                        m_Sage.SayTo(from, "It would cost you {0} gold to have that deciphered.", toConsume);
                        from.SendMessage("You do not have enough gold.");
                    }
				}
				else
					m_Sage.SayTo(from, "That does not need my services.");
            }
        }

		public LibrarianGuildmaster( Serial serial ) : base( serial )
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