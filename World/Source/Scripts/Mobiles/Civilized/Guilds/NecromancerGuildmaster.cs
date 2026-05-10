using Server;
using System;
using System.Collections.Generic;
using System.Collections;
using Server.Items;
using Server.Multis;
using Server.Guilds;
using Server.ContextMenus;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Mobiles
{
	public class NecromancerGuildmaster : BaseGuildmaster
	{
		public override NpcGuild NpcGuild{ get{ return NpcGuild.NecromancersGuild; } }

		public override string TalkGumpTitle{ get{ return "Dealing With Deathly Things"; } }
		public override string TalkGumpSubject{ get{ return "Necromancer"; } }

		[Constructable]
		public NecromancerGuildmaster() : base( "black magic" )
		{
			SetSkill( SkillName.Spiritualism, 65.0, 88.0 );
			SetSkill( SkillName.Inscribe, 60.0, 83.0 );
			SetSkill( SkillName.Meditation, 60.0, 83.0 );
			SetSkill( SkillName.MagicResist, 65.0, 88.0 );
			SetSkill( SkillName.Necromancy, 64.0, 100.0 );
			SetSkill( SkillName.Forensics, 82.0, 100.0 );

			Hue = 1150;
			HairHue = 932;

			RangePerception = BaseCreature.DefaultRangePerception;
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
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.None,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Necro,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.Weapon,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Mage,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.Weapon,		ItemSalesInfo.Material.Wood,	ItemSalesInfo.Market.Necro,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.Reagent,		ItemSalesInfo.Material.None,	ItemSalesInfo.Market.Necro,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.Reagent,		ItemSalesInfo.Material.None,	ItemSalesInfo.Market.Witch,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.Book,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Necro,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.Scroll,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Necro,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.All,			ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Evil,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.All,			ItemSalesInfo.Material.None,	ItemSalesInfo.Market.Death,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.Halloween,	ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.None,	null	 );
				}
			}

			public class InternalSellInfo : GenericSellInfo
			{
				public InternalSellInfo()
				{
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.None,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Necro,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.Weapon,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Mage,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.Weapon,		ItemSalesInfo.Material.Wood,	ItemSalesInfo.Market.Necro,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.Reagent,		ItemSalesInfo.Material.None,	ItemSalesInfo.Market.Necro,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.Reagent,		ItemSalesInfo.Material.None,	ItemSalesInfo.Market.Witch,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.Book,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Necro,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.Scroll,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Necro,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.All,			ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Evil,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.All,			ItemSalesInfo.Material.None,	ItemSalesInfo.Market.Death,		ItemSalesInfo.World.None,	null	 );
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.Halloween,	ItemSalesInfo.Material.All,		ItemSalesInfo.Market.All,		ItemSalesInfo.World.None,	null	 );
				}
			}
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Server.Items.BlackStaff() );
		}

		///////////////////////////////////////////////////////////////////////////

		public virtual bool HealsYoungPlayers{ get{ return true; } }

		private DateTime m_NextResurrect;
		private static TimeSpan ResurrectDelay = TimeSpan.FromSeconds( 2.0 );

		public virtual void OfferResurrection( Mobile m )
		{
			Direction = GetDirectionTo( m );

			m.PlaySound( 0x214 );
			m.FixedEffect( 0x376A, 10, 16 );

			m.CloseGump( typeof( ResurrectCostGump ) );
			m.SendGump( new ResurrectCostGump( m, 1 ) );
		}

		public virtual void OfferHeal( PlayerMobile m )
		{
			Direction = GetDirectionTo( m );

			if ( m.CheckYoungHealTime() )
			{
				Say( "You look like you need soe healing dark one." );

				m.PlaySound( 0x1F2 );
				m.FixedEffect( 0x376A, 9, 32 );

				m.Hits = m.HitsMax;
			}
			else
			{
				Say( 501228 ); // I can do no more for you at this time.
			}
		}

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( !m.Frozen && m is PlayerMobile && DateTime.Now >= m_NextResurrect && InRange( m, 6 ) && ( m.Criminal || m.Kills > 0 || m.Karma < 0 ) && this.CanSee( m ) && this.InLOS( m ) )
			{
				if ( !m.Alive )
				{
					m_NextResurrect = DateTime.Now + ResurrectDelay;

					if ( m.Map == null || !m.Map.CanFit( m.Location, 16, false, false ) )
					{
						m.SendLocalizedMessage( 502391 ); // Thou can not be resurrected there!
					}
					else
					{
						OfferResurrection( m );
					}
				}
				else if ( m.Hits < m.HitsMax && m is PlayerMobile )
				{
					OfferHeal( (PlayerMobile) m );
				}
			}
		}

		private class FixEntry : ContextMenuEntry
		{
			private NecromancerGuildmaster m_Necromancer;
			private Mobile m_From;

			public FixEntry( NecromancerGuildmaster NecromancerGuildmaster, Mobile from ) : base( 6120, 12 )
			{
				m_Necromancer = NecromancerGuildmaster;
				m_From = from;
				Enabled = m_Necromancer.CheckVendorAccess( from );
			}

			public override void OnClick()
			{
				m_Necromancer.BeginServices( m_From );
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
				nCost = nCost - (int)( ( from.Skills[SkillName.Begging].Value * 0.005 ) * nCost ); if ( nCost < 1 ){ nCost = 1; }
				SayTo(from, "Since you are begging, do you still want me to charge a crystal balls of summoning with 5 charges, it will only cost you " + nCost.ToString() + " gold?");
			}
			else { SayTo(from, "If you want me to charge a crystal ball of summoning with 5 charges, it will cost you " + nCost.ToString() + " gold."); }

            from.Target = new RepairTarget(this);
        }

        private class RepairTarget : Target
        {
            private NecromancerGuildmaster m_Necromancer;

            public RepairTarget(NecromancerGuildmaster necro) : base(12, false, TargetFlags.None)
            {
                m_Necromancer = necro;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is HenchmanFamiliarItem && from.Backpack != null)
                {
                    HenchmanFamiliarItem ball = targeted as HenchmanFamiliarItem;
                    Container pack = from.Backpack;

                    int toConsume = 0;

					if ( ball.Charges < 50 )
                    {
						toConsume = 500;

						if ( BeggingPose(from) > 0 ) // LET US SEE IF THEY ARE BEGGING
						{
							toConsume = toConsume - (int)( ( from.Skills[SkillName.Begging].Value * 0.005 ) * toConsume );
						}
                    }
                    else
                    {
						m_Necromancer.SayTo(from, "That crystal ball has too many charges already.");
                    }

                    if (toConsume == 0)
                        return;

                    if (pack.ConsumeTotal(typeof(Gold), toConsume))
                    {
						if ( BeggingPose(from) > 0 ){ Titles.AwardKarma( from, -BeggingKarma( from ), true ); } // DO ANY KARMA LOSS
                        m_Necromancer.SayTo(from, "Your crystal ball is charged.");
                        from.SendMessage(String.Format("You pay {0} gold.", toConsume));
                        Effects.PlaySound(from.Location, from.Map, 0x5C1);
						ball.Charges = ball.Charges + 5;
                    }
                    else
                    {
                        m_Necromancer.SayTo(from, "It would cost you {0} gold to have that charged.", toConsume);
                        from.SendMessage("You do not have enough gold.");
                    }
                }
				else
				{
					m_Necromancer.SayTo(from, "That does not need my services.");
				}
            }
        }

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is StarSapphire )
			{
				int StarSapphires = dropped.Amount;
				string sMessage = "";

				if ( ( StarSapphires > 19 ) && ( from.Skills[SkillName.Elementalism].Base >= 50 || from.Skills[SkillName.Magery].Base >= 50 || from.Skills[SkillName.Necromancy].Base >= 50 ) )
				{
					sMessage = "Ahhh...this is generous of you. Here...have this as a token of the guild's gratitude.";
					HenchmanFamiliarItem ball = new HenchmanFamiliarItem();
					ball.FamiliarOwner = from;
					from.AddToBackpack ( ball );
				}
				else
				{
					sMessage = "Thank you for these. Star sapphires are something we often look for.";
				}

				this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sMessage, from.NetState);
				dropped.Delete();
			}
			else if ( dropped is HenchmanFamiliarItem )
			{
				string sMessage = "";

				int HighSpellCaster = 0;
				if ( from.Skills[SkillName.Elementalism].Base >= 50 || from.Skills[SkillName.Magery].Base >= 50 || from.Skills[SkillName.Necromancy].Base >= 50 ){ HighSpellCaster = 1; }
				if ( from.Skills[SkillName.Elementalism].Base >= 100 || from.Skills[SkillName.Magery].Base >= 100 || from.Skills[SkillName.Necromancy].Base >= 100 ){ HighSpellCaster = 2; }

				if ( HighSpellCaster > 0 )
				{
					HenchmanFamiliarItem ball = (HenchmanFamiliarItem)dropped;

					if ( ball.FamiliarType == 0x16 ){ ball.FamiliarType = 0xD9; sMessage = "Your familiar is now in the form of a dog." ; }
					else if ( ball.FamiliarType == 0xD9 ){ ball.FamiliarType = 238; sMessage = "Your familiar is now in the form of a rat." ; }
					else if ( ball.FamiliarType == 238 ){ ball.FamiliarType = 0xC9; sMessage = "Your familiar is now in the form of a cat." ; }
					else if ( ball.FamiliarType == 0xC9 ){ ball.FamiliarType = 0xD7; sMessage = "Your familiar is now in the form of a huge rat." ; }
					else if ( ball.FamiliarType == 0xD7 ){ ball.FamiliarType = 80; sMessage = "Your familiar is now in the form of a large toad." ; }
					else if ( ball.FamiliarType == 80 ){ ball.FamiliarType = 81; sMessage = "Your familiar is now in the form of a huge frog." ; }
					else if ( ball.FamiliarType == 81 ){ ball.FamiliarType = 340; sMessage = "Your familiar is now in the form of a large cat." ; }
					else if ( ball.FamiliarType == 340 ){ ball.FamiliarType = 277; sMessage = "Your familiar is now in the form of a wolf." ; }
					else if ( ball.FamiliarType == 277 ){ ball.FamiliarType = 0xCE; sMessage = "Your familiar is now in the form of a large lizard." ; }
					else if ( ball.FamiliarType == 0xCE && HighSpellCaster == 1 ){ ball.FamiliarType = 590; sMessage = "Your familiar is now in the form of a small dragon." ; }
					else if ( ball.FamiliarType == 0xCE && HighSpellCaster == 2 ){ ball.FamiliarType = 0x3C; sMessage = "Your familiar is now in the form of a dragon." ; }
					else if ( ball.FamiliarType == 590 || ball.FamiliarType == 0x3C ){ ball.FamiliarType = 315; sMessage = "Your familiar is now in the form of a large scorpion." ; }
					else if ( ball.FamiliarType == 315 ){ ball.FamiliarType = 120; sMessage = "Your familiar is now in the form of a huge beetle." ; }
					else if ( ball.FamiliarType == 120 ){ ball.FamiliarType = 202; sMessage = "Your familiar is now in the form of an imp." ; }
					else if ( ball.FamiliarType == 202 && HighSpellCaster == 1 ){ ball.FamiliarType = 140; sMessage = "Your familiar is now in the form of a spider." ; }
					else if ( ball.FamiliarType == 202 && HighSpellCaster == 2 ){ ball.FamiliarType = 173; sMessage = "Your familiar is now in the form of a giant spider." ; }
					else if ( ball.FamiliarType == 140 || ball.FamiliarType == 173 ){ ball.FamiliarType = 317; sMessage = "Your familiar is now in the form of a bat." ; }
					else if ( ball.FamiliarType == 317 ){ ball.FamiliarType = 242; sMessage = "Your familiar is now in the form of a giant insect." ; }
					else if ( ball.FamiliarType == 242 ){ ball.FamiliarType = 0x15; sMessage = "Your familiar is now in the form of a serpent." ; }
					else if ( ball.FamiliarType == 0x15 && HighSpellCaster == 1 ){ ball.FamiliarType = 0x4; sMessage = "Your familiar is now in the form of a demon." ; }
					else if ( ball.FamiliarType == 0x15 && HighSpellCaster == 2 ){ ball.FamiliarType = 0x9; sMessage = "Your familiar is now in the form of a daemon." ; }
					else if ( ball.FamiliarType == 0x4 || ball.FamiliarType == 0x9 ){ ball.FamiliarType = 0x16; sMessage = "Your familiar is now in the form of a gazer." ; }

					sMessage = "You would perhaps like a different familiar? " + sMessage;
					from.AddToBackpack ( ball );
				}
				else
				{
					sMessage = "Thank you for this. I could only assume an apprentice spell caster lost this.";
					dropped.Delete();
				}

				this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sMessage, from.NetState);
			}
			return base.OnDragDrop( from, dropped );
		}

		public NecromancerGuildmaster( Serial serial ) : base( serial )
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