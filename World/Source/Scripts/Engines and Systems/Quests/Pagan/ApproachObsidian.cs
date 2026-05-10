using System;
using Server.Mobiles;
using Server.Misc;
using Server.Network;
using Server.Utilities;

namespace Server.Items
{
	public class ApproachObsidian : Item
	{
		[Constructable]
		public ApproachObsidian() : base(0x2161)
		{
			Movable = false;
			Visible = false;
			Name = "floor";
		}

		public ApproachObsidian(Serial serial) : base(serial)
		{
		}

		public override bool OnMoveOver( Mobile mobile )
		{
			if ( false == ( mobile is PlayerMobile ) ) return true;

			var m = (PlayerMobile)mobile;
			if ( m.IsTitanOfEther ) return true;

			var tip = m.Backpack.FindItemByType<ObeliskTip>();
			if ( tip == null ) return true;
			if ( tip.ObeliskOwner != mobile ) return true;
			if ( tip.WonAir + tip.WonFire + tip.WonEarth + tip.WonWater < 4 ) return true;

			WorldUtilities.DeleteAllItems<ObeliskTip>( item => item.ObeliskOwner == mobile );

			TitanRiches(m);

			m.IsTitanOfEther = true;
			m.RefreshSkillCap();
			m.StatCap = 300;

			Server.Items.QuestSouvenir.GiveReward( m, "Obelisk Tip", 0, 0x185F );
			Server.Items.QuestSouvenir.GiveReward( m, "Breath of Air", 0, 0x1860 );
			Server.Items.QuestSouvenir.GiveReward( m, "Tongue of Flame", 0, 0x1861 );
			Server.Items.QuestSouvenir.GiveReward( m, "Heart of Earth", 0, 0x1862 );
			Server.Items.QuestSouvenir.GiveReward( m, "Tear of the Seas", 0, 0x1863 );

			m.AddToBackpack( new ObsidianGate() );
			if (m.Temptations.LimitTitanBonus) m.AddToBackpack( new SoulStone() );
			m.SendMessage( "Some items have appeared in your pack." );
			m.SendMessage( "You can change your title for this achievement." );
			m.LocalOverheadMessage( MessageType.Emote, 1150, true, "You are now a Titan of Ether!" );
			LoggingFunctions.LogGeneric( m, "has become a Titan of Ether." );

			return true;
		}

		public static void TitanRiches( Mobile m )
		{
			Map map = m.Map;

			if ( map != null )
			{
				for ( int x = -12; x <= 12; ++x )
				{
					for ( int y = -12; y <= 12; ++y )
					{
						double dist = Math.Sqrt(x*x+y*y);

						if ( dist <= 12 )
							new GoodiesTimer( map, m.X + x, m.Y + y ).Start();
					}
				}
			}
		}

		public class GoodiesTimer : Timer
		{
			private Map m_Map;
			private int m_X, m_Y;

			public GoodiesTimer( Map map, int x, int y ) : base( TimeSpan.FromSeconds( Utility.RandomDouble() * 5.0 ) )
			{
				m_Map = map;
				m_X = x;
				m_Y = y;
			}

			protected override void OnTick()
			{
				int z = m_Map.GetAverageZ( m_X, m_Y );
				bool canFit = m_Map.CanFit( m_X, m_Y, z, 6, false, false );

				for ( int i = -3; !canFit && i <= 3; ++i )
				{
					canFit = m_Map.CanFit( m_X, m_Y, z + i, 6, false, false );

					if ( canFit )
						z += i;
				}

				if ( !canFit )
					return;

				Item g = new Gold( 100, 200 ); g.Delete();

				int r1 = (int)( Utility.RandomMinMax( 80, 160 ) * (MyServerSettings.GetGoldCutRate() * .01) );
				int r2 = (int)( Utility.RandomMinMax( 200, 400 ) * (MyServerSettings.GetGoldCutRate() * .01) );
				int r3 = (int)( Utility.RandomMinMax( 400, 800 ) * (MyServerSettings.GetGoldCutRate() * .01) );
				int r4 = (int)( Utility.RandomMinMax( 800, 1200 ) * (MyServerSettings.GetGoldCutRate() * .01) );
				int r5 = (int)( Utility.RandomMinMax( 1200, 1600 ) * (MyServerSettings.GetGoldCutRate() * .01) );

				switch ( Utility.Random( 21 ) )
				{
					case 0: g = new Crystals( r1 ); break;
					case 1: g = new DDGemstones( r2 ); break;
					case 2: g = new DDJewels( r2 ); break;
					case 3: g = new DDGoldNuggets( r3 ); break;
					case 4: g = new Gold( r3 ); break;
					case 5: g = new Gold( r3 ); break;
					case 6: g = new Gold( r3 ); break;
					case 7: g = new DDSilver( r4 ); break;
					case 8: g = new DDSilver( r4 ); break;
					case 9: g = new DDSilver( r4 ); break;
					case 10: g = new DDSilver( r4 ); break;
					case 11: g = new DDSilver( r4 ); break;
					case 12: g = new DDSilver( r4 ); break;
					case 13: g = new DDCopper( r5 ); break;
					case 14: g = new DDCopper( r5 ); break;
					case 15: g = new DDCopper( r5 ); break;
					case 16: g = new DDCopper( r5 ); break;
					case 17: g = new DDCopper( r5 ); break;
					case 18: g = new DDCopper( r5 ); break;
					case 19: g = new DDCopper( r5 ); break;
					case 20: g = new DDCopper( r5 ); break;
				}

				g.MoveToWorld( new Point3D( m_X, m_Y, z ), m_Map );

				if ( 0.5 >= Utility.RandomDouble() )
				{
					switch ( Utility.Random( 3 ) )
					{
						case 0: // Fire column
						{
							Effects.SendLocationParticles( EffectItem.Create( g.Location, g.Map, EffectItem.DefaultDuration ), 0x3709, 10, 30, 5052 );
							Effects.PlaySound( g, g.Map, 0x208 );

							break;
						}
						case 1: // Explosion
						{
							Effects.SendLocationParticles( EffectItem.Create( g.Location, g.Map, EffectItem.DefaultDuration ), 0x36BD, 20, 10, 5044 );
							Effects.PlaySound( g, g.Map, 0x307 );

							break;
						}
						case 2: // Ball of fire
						{
							Effects.SendLocationParticles( EffectItem.Create( g.Location, g.Map, EffectItem.DefaultDuration ), 0x36FE, 10, 10, 5052 );

							break;
						}
					}
				}
			}
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