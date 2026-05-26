using System; 
using System.Collections.Generic; 
using Server; 
using Server.Misc;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Regions;
using Server.Spells;

namespace Server.Mobiles 
{ 
	public class Shipwright : BaseVendor 
	{ 
		private List<SBInfo> m_SBInfos = new List<SBInfo>(); 
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } } 

		public override string TalkGumpTitle{ get{ return "The High Seas"; } }
		public override string TalkGumpSubject{ get{ return "Shipwright"; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.FishermensGuild; } }

		[Constructable]
		public Shipwright() : base( "the shipwright" ) 
		{ 
			SetSkill( SkillName.Carpentry, 60.0, 83.0 );
			SetSkill( SkillName.Bludgeoning, 36.0, 68.0 );
			SetSkill( SkillName.Seafaring, 75.0, 98.0 );
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
					ItemInformation.GetSellList( m_Merchant, this, 	ItemSalesInfo.Category.All,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Sailor,		ItemSalesInfo.World.None,	null	 );
				}
			}

			public class InternalSellInfo : GenericSellInfo
			{
				public InternalSellInfo()
				{
					ItemInformation.GetBuysList( m_Merchant, this, 	ItemSalesInfo.Category.All,		ItemSalesInfo.Material.All,		ItemSalesInfo.Market.Sailor,		ItemSalesInfo.World.None,	null	 );
				}
			}
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Server.Items.SmithHammer() );
			AddItem( new Server.Items.TricorneHat( Utility.RandomDyedHue() ) );
		}

		///////////////////////////////////////////////////////////////////////////

		public override bool OnDragDrop( Mobile from, Item o )
		{
			if ( o is Key && ((Key)o).KeyValue != 0 && ((Key)o).Link is BaseBoat )
			{
				BaseBoat boat = ((Key)o).Link as BaseBoat;
				Container pack = from.Backpack;

				if ( !boat.Deleted && boat.CheckKey( ((Key)o).KeyValue ) )
				{

                    if (pack.ConsumeTotal(typeof(Gold), 1000))
                    {
						ReturnToBoat( boat.GetMarkedLocation(), boat.Map, from );
                        from.SendMessage(String.Format("You pay 1,000 gold."));
					}
					else
					{
                        this.SayTo(from, "It would cost you 1,000 gold to be returned to your ship.");
                        from.SendMessage("You do not have enough gold.");
					}
				}
				else
				{
					this.SayTo(from, "There is nothing I can do with that.");
				}
			}

			return base.OnDragDrop( from, o );
		}

		public void ReturnToBoat( Point3D loc, Map map, Mobile from )
		{
			if ( !SpellHelper.CheckTravel( from, TravelCheckType.RecallFrom ) )
			{
			}
			else if ( Worlds.AllowEscape( from, from.Map, from.Location, from.X, from.Y ) == false )
			{
				this.SayTo(from, "Your ship is somewhere I cannot send you." );
			}
			else if ( Worlds.RegionAllowedRecall( from.Map, from.Location, from.X, from.Y ) == false )
			{
				this.SayTo(from, "Your ship is somewhere I cannot send you." );
			}
			else if ( Worlds.RegionAllowedTeleport( map, loc, loc.X, loc.Y ) == false )
			{
				this.SayTo(from, "Your ship is somewhere I cannot send you." );
			}
			else if ( !SpellHelper.CheckTravel( from, map, loc, TravelCheckType.RecallTo ) )
			{
			}
			else if ( Server.Misc.WeightOverloading.IsOverloaded( from ) )
			{
				from.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
			}
			else if ( !map.CanSpawnMobile( loc.X, loc.Y, loc.Z ) )
			{
				from.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
			else
			{
				BaseCreature.TeleportPets( from, loc, map, false );
				from.PlaySound( 0x13 );
				Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 0, 0, 5024, 0 );
				from.MoveToWorld( loc, map );
				from.PlaySound( 0x13 );
				Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 0, 0, 5024, 0 );
			}
		}

		public Shipwright( Serial serial ) : base( serial ) 
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