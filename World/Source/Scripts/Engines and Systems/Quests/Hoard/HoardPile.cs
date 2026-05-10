using System;
using System.Globalization;
using Server.Misc;

namespace Server.Items
{
	public class HoardPiles : Item
	{
		private static readonly TextInfo m_TextInfo = new CultureInfo("en-US", false).TextInfo;

		private int m_Uses;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Uses { get{ return m_Uses; } set{ m_Uses = value; InvalidateProperties(); } }

		public string HoardName;
		
		[CommandProperty(AccessLevel.Owner)]
		public string Hoard_Name { get { return HoardName; } set { HoardName = value; InvalidateProperties(); } }

		private DateTime m_DecayTime;
		private Timer m_DecayTimer;

		public virtual TimeSpan DecayDelay{ get{ return TimeSpan.FromMinutes( 10.0 ); } } // HOW LONG UNTIL THE PILE DECAYS IN MINUTES

		[Constructable]
		public HoardPiles() : base( 0x0879 )
		{
			Movable = false;
			Name = "treasure hoard";
			Light = LightType.Circle225;
			ItemID = Utility.RandomList( 0x0879, 0x08AD );
		}

		public virtual void RefreshDecay( bool setDecayTime )
		{
			if( Deleted )
				return;
			if( m_DecayTimer != null )
				m_DecayTimer.Stop();
			if( setDecayTime )
				m_DecayTime = DateTime.Now + DecayDelay;

			TimeSpan ts = m_DecayTime - DateTime.Now;

			if ( ts < TimeSpan.FromMinutes( 2.0 ) )
				ts = TimeSpan.FromMinutes( 2.0 );

			m_DecayTimer = Timer.DelayCall( ts, new TimerCallback( Delete ) );
		}

		public HoardPiles( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( ( int) 0 ); // version
			writer.Write( m_DecayTime );
			writer.WriteEncodedInt( (int) m_Uses );
            writer.Write( HoardName );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			switch ( version )
			{
				case 0:
				{
					m_DecayTime = reader.ReadDateTime();
					RefreshDecay( false );
					break;
				}
			}
			m_Uses = reader.ReadEncodedInt();
			HoardName = reader.ReadString();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.Blessed )
			{
				from.SendMessage( "You cannot look through that while in this state." );
			}
			else if ( !from.InRange( GetWorldLocation(), 3 ) )
			{
				from.SendMessage( "You will have to get closer to it!" );
			}
			else if ( m_Uses < 5 )
			{
				Item item = null;

				switch ( Utility.Random( 17 ) )
				{
					case 0:
						item = Loot.RandomArty();
						CustomEventSink.InvokeLootPull(new LootPullEventArgs(from, item));
						break;
					case 1:
					case 2:
						item = Loot.RandomSArty( Server.LootPackEntry.playOrient( from ), from );
						break;
					case 3:
						item = Loot.RandomRelic( from );
						break;
					case 4:
						item = Loot.RandomRare( Utility.RandomMinMax(6,12), from );
						break;
					case 5:
						item = Loot.RandomBooks( Utility.RandomMinMax(6,12) );
						break;
					case 6:
						item = Loot.RandomScroll( Utility.Random(12)+1 );
						break;
					case 7:
						int luckMod = from.Luck; if ( luckMod > 2000 ){ luckMod = 2000; }

						if ( (Region.Find( from.Location, from.Map )).IsPartOf( "the Ancient Crash Site" ) || (Region.Find( from.Location, from.Map )).IsPartOf( "the Ancient Sky Ship" ) )
							item = new DDXormite( ( luckMod + Utility.RandomMinMax( 333, 666 ) ) );
						else if ( (Region.Find( from.Location, from.Map )).IsPartOf( "the Mines of Morinia" ) )
							item = new Crystals( ( luckMod + Utility.RandomMinMax( 200, 400 ) ) );
						else if ( from.Land == Land.Underworld )
							item = new DDJewels( ( luckMod + Utility.RandomMinMax( 500, 1000 ) ) );
						else
							item = new Gold( ( luckMod + Utility.RandomMinMax( 1000, 2000 ) ) );

						break;
					case 8: case 9: case 10: case 11:
						item = Loot.RandomMagicalItem( Server.LootPackEntry.playOrient( from ) );
						item = LootPackEntry.Enchant( from, 500, item );
						break;
					case 12:
						item = Loot.RandomInstrument();
						item = LootPackEntry.Enchant( from, 500, item );
						break;
					case 13:
						item = Loot.RandomGem();
						break;
					case 14:
						item = Loot.RandomPotion( Utility.RandomMinMax(6,12), true );
						break;
					case 15:
						item = new MagicalWand( Utility.RandomMinMax(6,8)); 
						break;
					case 16:
						m_Uses = 6; // STOP GIVING LOOT WHEN THEY GET A CONTAINER
						int chestLuck = Server.Misc.GetPlayerInfo.LuckyPlayerArtifacts( from.Luck );
						if ( chestLuck < 3 ){ chestLuck = 3; }
						if ( chestLuck > 8 ){ chestLuck = 8; }
						int chestLevel = Utility.RandomMinMax( 3, chestLuck );
						item = new LootChest( chestLevel );
						item.ItemID = Utility.RandomList( 0x9AB, 0xE40, 0xE41, 0xE7C );
						item.Hue = Utility.RandomList( 0x961, 0x962, 0x963, 0x964, 0x965, 0x966, 0x967, 0x968, 0x969, 0x96A, 0x96B, 0x96C, 0x96D, 0x96E, 0x96F, 0x970, 0x971, 0x972, 0x973, 0x974, 0x975, 0x976, 0x977, 0x978, 0x979, 0x97A, 0x97B, 0x97C, 0x97D, 0x97E, 0x4AA );

						string box = "hoard chest";
						switch ( Utility.RandomMinMax( 0, 7 ) )
						{
							case 0:	box = "hoard chest";		break;
							case 1:	box = "treasure chest";		break;
							case 2:	box = "secret chest";		break;
							case 3:	box = "fabled chest";		break;
							case 4: box = "legendary chest";	break;
							case 5:	box = "mythical chest";		break;
							case 6:	box = "lost chest";			break;
							case 7:	box = "stolen chest";		break;
						}

						if ( Server.Misc.Worlds.IsOnSpaceship( from.Location, from.Map ) )
						{
							Server.Misc.ContainerFunctions.MakeSpaceCrate( ((LockableContainer)item) );
							box = item.Name;
						}

						switch ( Utility.RandomMinMax( 0, 1 ) )
						{
							case 0:	item.Name = box + " from " + Server.Misc.Worlds.GetRegionName( from.Map, from.Location );	break;
							case 1:	item.Name = box + " of " + HoardName;	break;
						}
						int xTraCash = Utility.RandomMinMax( 5000, 8000 );

						LootPackChange.AddGoldToContainer( xTraCash, (LootChest)item, from, chestLevel );
						int artychance = GetPlayerInfo.LuckyPlayerArtifacts( from.Luck ) + 10;
						if ( Utility.RandomMinMax( 0, 100 ) < artychance ){ Item artys = Loot.RandomArty(); ((LootChest)item).DropItem( artys ); }
						break;
				}

				if ( item != null )
				{
					if ( item is Container ){ item.MoveToWorld( from.Location, from.Map ); }
					else { from.AddToBackpack( item ); }
				}
				else
				{
					if ( Worlds.IsOnSpaceship( from.Location, from.Map ) )
						item = new DDXormite( ( from.Luck + Utility.RandomMinMax( 333, 666 ) ) );
					else if ( from.Land == Land.Underworld )
						item = new DDJewels( ( from.Luck + Utility.RandomMinMax( 500, 1000 ) ) );
					else
						item = new Gold( ( from.Luck + Utility.RandomMinMax( 1000, 2000 ) ) );

					if ( item != null ){ from.AddToBackpack( item ); }
				}

				if ( item != null )
				{
					m_Uses++;
					if ( Server.Misc.GetPlayerInfo.LuckyPlayer( from.Luck ) && Utility.RandomBool() )
					{
						m_Uses--;
					}

					string name = "";
					if ( item.ColorText1 != null ) name = item.ColorText1;
					if ( item.ColorText2 != null ) name = name + " " + item.ColorText2;
					if ( string.IsNullOrWhiteSpace( name ) ) name = item.Name;

					from.PlaySound( 0x2E5 );
					if ( string.IsNullOrWhiteSpace( name ) )
						from.SendMessage( "You pull something from the treasure hoard!" );
					else
					{
						from.LocalOverheadMessage( Network.MessageType.Regular, 1150, false, string.Format( "{0} !", m_TextInfo.ToTitleCase( name ) ) );
					}
				}
				else
				{
					from.SendMessage( "The item slipped out of your hand!" );
				}
			}
			
			if ( 5 <= m_Uses )
			{
				from.SendMessage( "There is nothing else worth taking from this pile!" );
				this.Delete();
			}
		}
	}
}