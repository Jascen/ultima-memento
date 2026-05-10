using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using System;
using Server.Regions;

namespace Server.Gumps
{
	public class ResurrectCostGump : Gump
	{
		private string GetLossMessage( Mobile owner )
		{
			string c1 = "5";
			string c2 = "10";

			if ( GetPlayerInfo.isFromSpace( owner ) )
			{
				c1 = "10";
				c2 = "20";
			}

			return "You will suffer a " + c2 + "% loss to your fame and karma. You will also lose " + c1 + "% of your statistics and skills.";
		}

		private int m_Price;
		private int m_Healer;
		private int m_Bank;
		private int m_Tithe;
		private int m_ResurrectType;

		public ResurrectCostGump( Mobile owner, int healer ) : base( 50, 50 )
		{
			owner.SendSound( 0x0F8 ); 
			string color = "#b7cbda";

			m_Healer = healer;
			m_Price = GetPlayerInfo.GetResurrectCost( owner );
			m_Bank = Banker.GetBalance( owner );
			m_Tithe = owner.TithingPoints;
			m_ResurrectType = 0;

			string sText;

			if ( m_Price > 0 )
			{
				bool appendLossText = GetPlayerInfo.isFromSpace( owner );

				if ( m_Price > m_Bank && m_Price > m_Tithe )
				{
					appendLossText = true;

					if ( m_Healer < 2 )
					{
						sText = "You currently do not have enough gold in the bank or tithed to provide an offering to the healer. Do you wish to plead to the healer for your life back now, without providing tribute?";
					}
					else
					{
						sText = "You currently do not have enough gold in the bank or tithed to provide an offering to the shrine. Do you wish to plead to the gods for your life back now, without providing tribute?";

						if ( m_Healer == 3 )
						{
							sText = "You currently do not have enough gold in the bank or tithed to provide an offering to Azrael. Do you wish to plead to him for your life back now, without providing tribute?";
						}
						else if ( m_Healer == 4 )
						{
							sText = "You currently do not have enough gold in the bank or tithed to provide an offering to the Reaper. Do you wish to plead to him for your life back now, without providing tribute?";
						}
						else if ( m_Healer == 5 )
						{
							sText = "You currently do not have enough gold in the bank or tithed to provide an offering to the goddess of the sea. Do you wish to plead to Amphitrite for your life back now, without providing tribute?";
						}
						else if ( m_Healer == 6 )
						{
							sText = "You currently do not have enough gold in the bank or tithed to provide an offering to the Archmages. Do you wish to plead to the Archmages for your life back now, without providing tribute?";
						}
						else if ( m_Healer == 7 )
						{
							sText = "You currently do not have enough gold in the bank or tithed to provide an offering to Sin'Vraal. Do you wish to plead to him for your life back now, without providing tribute?";
						}
						else if ( m_Healer == 8 )
						{
							sText = "You currently do not have enough gold in the bank or tithed to provide an offering to the god of the sea. Do you wish to plead to Neptune for your life back now, without providing tribute?";
						}
						else if ( m_Healer == 9 )
						{
							sText = "You currently do not have enough gold in the bank or tithed to provide an offering to the lord of the sea. Do you wish to plead to Poseidon for your life back now, without providing tribute?";
						}
						else if ( m_Healer == 10 )
						{
							sText = "You currently do not have enough gold in the bank or tithed to provide an offering to Ktulu. Do you wish to plead to him for your life back now, without providing tribute?";
						}
						else if ( m_Healer == 11 )
						{
							sText = "You currently do not have enough gold in the bank or tithed to provide an offering to Durama. Do you wish to plead for your life back now, without providing tribute?";
						}
						else if ( m_Healer == 12 )
						{
							sText = "You currently do not have enough gold in the bank or tithed to provide an offering to the Ancient Dryad. Do you wish to plead for your life back now, without providing tribute?";
						}
					}
					m_ResurrectType = 1;
				}
				else
				{
					string tribute = "tithing";
					if ( m_Bank >= m_Price ){ tribute = "gold in the bank"; }

					if ( m_Healer < 2 )
					{
						sText = "You currently have enough " + tribute + " to provide an offering to the healer. Do you wish to offer the tribute to the healer for your life back?";
					}
					else
					{
						sText = "You currently have enough " + tribute + " to provide an offering to the shrine. Do you wish to offer the tribute to the gods for your life back?";

						if ( m_Healer == 3 )
						{
							sText = "Azrael is not ready for your soul just yet, and you currently have enough " + tribute + " to provide an offering to him. Do you wish to offer the tribute to Azrael for your life back?";
						}
						else if ( m_Healer == 4 )
						{
							sText = "Although the Reaper would gladly take your soul, he thinks your time has come to an end too soon. You currently have enough " + tribute + " to provide an offering to the Reaper. Do you wish to offer the tribute to him for your life back?";
						}
						else if ( m_Healer == 5 )
						{
							sText = "You currently have enough " + tribute + " to provide an offering to the goddess of the sea. Do you wish to offer the tribute to Amphitrite for your life back?";
						}
						else if ( m_Healer == 6 )
						{
							sText = "You currently have enough " + tribute + " to provide an offering to the Archmages. Do you wish to offer the tribute to the Archmages for your life back?";
						}
						else if ( m_Healer == 7 )
						{
							sText = "You currently have enough " + tribute + " to provide an offering to Sin'Vraal. Do you wish to offer the tribute to Sin'Vraal for your life back?";
						}
						else if ( m_Healer == 8 )
						{
							sText = "You currently have enough " + tribute + " to provide an offering to the god of the sea. Do you wish to offer the tribute to Neptune for your life back?";
						}
						else if ( m_Healer == 9 )
						{
							sText = "You currently have enough " + tribute + " to provide an offering to the lord of the sea. Do you wish to offer the tribute to Poseidon for your life back?";
						}
						else if ( m_Healer == 10 )
						{
							sText = "Ktulu is not ready for your soul just yet, and you currently have enough " + tribute + " to provide an offering to him. Do you wish to offer the tribute to Ktulu for your life back?";
						}
						else if ( m_Healer == 11 )
						{
							sText = "You currently have enough " + tribute + " to provide an offering to Durama. Do you wish to offer the tribute for your life back?";
						}
						else if ( m_Healer == 12 )
						{
							sText = "You currently have enough " + tribute + " to provide an offering to the Ancient Dryad. Do you wish to offer the tribute for your life back?";
						}
					}
					m_ResurrectType = 2;
				}

				if ( appendLossText )
				{
					sText += " " + GetLossMessage( owner );
				}
			}
			else
			{
				if ( m_Healer < 2 )
				{
					sText = "Do you wish to have the healer return you to life?";
				}
				else
				{
					sText = "Do you wish to have the gods return you to life?";

					if ( m_Healer == 3 )
					{
						sText = "Do you wish to have Azrael return you to life?";
					}
					else if ( m_Healer == 4 )
					{
						sText = "Do you wish to have the Reaper return you to life?";
					}
					else if ( m_Healer == 5 )
					{
						sText = "Do you wish to have Amphitrite return you to life?";
					}
					else if ( m_Healer == 6 )
					{
						sText = "Do you wish to have the Archmages return you to life?";
					}
					else if ( m_Healer == 7 )
					{
						sText = "Do you wish to have Sin'Vraal return you to life?";
					}
					else if ( m_Healer == 8 )
					{
						sText = "Do you wish to have Neptune return you to life?";
					}
					else if ( m_Healer == 9 )
					{
						sText = "Do you wish to have Poseidon return you to life?";
					}
					else if ( m_Healer == 10 )
					{
						sText = "Do you wish to have Ktulu return you to life?";
					}
					else if ( m_Healer == 11 )
					{
						sText = "Do you wish to have Durama return you to life?";
					}
					else if ( m_Healer == 12 )
					{
						sText = "Do you wish to have the Ancient Dryad return you to life?";
					}
				}
			}

			string sGrave = "RETURN TO THE LIVING";
			switch ( Utility.RandomMinMax( 0, 3 ) )
			{
				case 0:	sGrave = "YOUR LIFE BACK";			break;
				case 1:	sGrave = "YOUR RESURRECTION";		break;
				case 2:	sGrave = "RETURN TO THE LIVING";	break;
				case 3:	sGrave = "RETURN FROM THE DEAD";	break;
			}

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			int img = 9586;
				if ( owner.Karma < 0 ){ img = 9587; }

			AddImage(0, 0, img, Server.Misc.PlayerSettings.GetGumpHue( owner ));
			AddHtml( 10, 11, 349, 20, @"<BODY><BASEFONT Color=" + color + ">" + sGrave + "</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(368, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);

			AddHtml( 11, 41, 385, 141, @"<BODY><BASEFONT Color=" + color + ">" + sText + "</BASEFONT></BODY>", (bool)false, (bool)false);

			AddButton(10, 225, 4023, 4023, 1, GumpButtonType.Reply, 0);
			AddButton(367, 225, 4020, 4020, 2, GumpButtonType.Reply, 0);

			if ( m_Price > 0 )
			{
				AddHtml( 238, 193, 100, 20, @"<BODY><BASEFONT Color=" + color + ">Resurrect:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 343, 194, 54, 20, @"<BODY><BASEFONT Color=" + color + ">" + String.Format("{0} Gold", m_Price ) + "</BASEFONT></BODY>", (bool)false, (bool)false);
			}

			if ( m_Bank >= m_Price && m_Price > 0 )
			{
				AddHtml( 11, 195, 61, 20, @"<BODY><BASEFONT Color=" + color + ">Bank:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 77, 195, 100, 20, @"<BODY><BASEFONT Color=" + color + ">" + String.Format("{0} Gold", m_Bank ) + "</BASEFONT></BODY>", (bool)false, (bool)false);
			}
			else if ( m_Tithe >= m_Price && m_Price > 0 )
			{
				AddHtml( 11, 195, 61, 20, @"<BODY><BASEFONT Color=" + color + ">Tithe:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 77, 195, 100, 20, @"<BODY><BASEFONT Color=" + color + ">" + (owner.TithingPoints).ToString() + "</BASEFONT></BODY>", (bool)false, (bool)false);
			}
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			from.SendSound( 0x0F8 ); 

			from.CloseGump( typeof( ResurrectCostGump ) );

			if( info.ButtonID == 1 )
			{
				if( from.Map == null || !from.Map.CanFit( from.Location, 16, false, false ) )
				{
					from.SendLocalizedMessage( 502391 ); // Thou can not be resurrected there!
					return;
				}

				var shouldPrompt = m_ResurrectType == 1 && from.SkillsTotal > 200 && ( from.RawDex + from.RawInt + from.RawStr ) > 90;

				ConfirmationGump.PromptIfFalse(
					from,
					shouldPrompt,
					() =>
					{
						if ( m_ResurrectType == 2 && m_Bank >= m_Price )
						{
							Banker.Withdraw( from, m_Price );
							from.SendLocalizedMessage( 1060398, m_Price.ToString() ); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
							from.SendLocalizedMessage( 1060022, Banker.GetBalance( from ).ToString() ); // You have ~1_AMOUNT~ gold in cash remaining in your bank box.
							Server.Misc.Death.Penalty( from, false );
						}
						else if ( m_ResurrectType == 2 && m_Tithe >= m_Price )
						{
							from.TithingPoints = from.TithingPoints - m_Price;
							from.SendMessage( "" + m_Price.ToString() + " tithing has been offered to the gods." );
							from.SendMessage( "" + (from.TithingPoints).ToString() + " tithing remains." );
							Server.Misc.Death.Penalty( from, false );
						}
						else if ( shouldPrompt )
						{
							Server.Misc.Death.Penalty( from, true );
						}

						from.PlaySound( 0x214 );
						from.FixedEffect( 0x376A, 10, 16 );

						from.Resurrect();

						from.Hits = from.HitsMax;
						from.Stam = from.StamMax;
						from.Mana = from.ManaMax;
						from.Hidden = true;
					},
					onConfirmed => new ConfirmationGump(
						from,
						"Accept Penalty?",
						"Are you sure you wish to accept the resurrection penalty? " + GetLossMessage( from ),
						onConfirmed
					)
				);
			}
			else
			{
				from.SendMessage( "You decide to remain in the spirit realm." );
			}
		}
	}
}

namespace Server 
{ 
	public class AutoRessurection
	{ 
		public static void Initialize()
		{
			EventSink.PlayerDeath += e => QueueResurrect(e.Mobile);
			EventSink.Login += e => QueueResurrect(e.Mobile);
		}

		private static void QueueResurrect(Mobile m)
		{
			if ( m != null && !m.Alive && GetPlayerInfo.GetResurrectCost( m ) > 0 )
			{
				Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), ResurrectNow, m );
			}
		}

		private static void ResurrectNow( object state )
		{
			Mobile m = state as Mobile;
			m.CloseGump( typeof( ResurrectNowGump ) );

			Item orb = SoulOrb.FindActive( m );
			if ( orb == null )
			{
				BuffInfo.RemoveBuff( m, BuffIcon.Resurrection );
				m.SendGump( new ResurrectNowGump( m ) );
			}
		}
	}
}

namespace Server.Gumps
{
	public class ResurrectNowGump : Gump
	{
		private string GetLossMessage( Mobile owner )
		{
			string c1 = "5";
			string c2 = "10";

			if ( GetPlayerInfo.isFromSpace( owner ) )
			{
				c1 = "10";
				c2 = "20";
			}

			return "You will suffer a " + c2 + "% loss to your fame and karma. You will also lose " + c1 + "% of your statistics and skills.";
		}

		public ResurrectNowGump( Mobile from ): base( 250, 50 )
		{
            int HealCost = GetPlayerInfo.GetResurrectCost( from );
			int BankGold = Banker.GetBalance( from );
			int TithePoints = from.TithingPoints;

			string sText = "Do you wish to plead to the gods for your life back now? You may also continue on in your spirit form and seek out a shrine or healer.";
			bool ResPenalty = false;

			if ( from.SkillsTotal > 200 && ( from.RawDex + from.RawInt + from.RawStr ) > 90 )
			{
				ResPenalty = true;

				if ( BankGold >= HealCost )
					sText = "Do you wish to plead to the gods for your life back now? " + GetLossMessage( from ) + " You have enough gold in the bank to offer the resurrection tribute, so perhaps you may want to find a shrine or healer instead of suffering the penalties.";
				else if ( TithePoints >= HealCost )
					sText = "Do you wish to plead to the gods for your life back now? " + GetLossMessage( from ) + " You have enough tithe to offer the resurrection tribute, so perhaps you may want to find a shrine or healer instead of suffering the penalties.";
				else
					sText = "Do you wish to plead to the gods for your life back now? " + GetLossMessage( from ) + " You cannot afford the resurrection tribute due to the lack of gold in the bank or tithed, so perhaps you may want to do this.";
			}

			string sGrave;
			switch ( Utility.RandomMinMax( 0, 3 ) )
			{
				default:
				case 0:	sGrave = "YOU HAVE DIED!";			break;
				case 1:	sGrave = "YOU HAVE PERISHED!";		break;
				case 2:	sGrave = "YOU MET YOUR END!";		break;
				case 3:	sGrave = "YOUR LIFE HAS ENDED!";	break;
			}

			from.SendSound( 0x0F8 ); 
			string color = "#da3f3f";

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);

			int img = 9586;
				if ( from.Karma < 0 ){ img = 9587; }

			AddImage(0, 0, img, Server.Misc.PlayerSettings.GetGumpHue( from ));

			AddHtml( 10, 11, 349, 20, @"<BODY><BASEFONT Color=" + color + ">" + sGrave + "</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(368, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);

			AddHtml( 11, 41, 385, 141, @"<BODY><BASEFONT Color=" + color + ">" + sText + "</BASEFONT></BODY>", (bool)false, (bool)false);

			AddButton(10, 225, 4023, 4023, 1, GumpButtonType.Reply, 0);
			AddButton(367, 225, 4020, 4020, 2, GumpButtonType.Reply, 0);

			if ( ResPenalty )
			{
				AddHtml( 238, 193, 100, 20, @"<BODY><BASEFONT Color=" + color + ">Resurrect:</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 343, 194, 54, 20, @"<BODY><BASEFONT Color=" + color + ">" + String.Format("{0} Gold", HealCost ) + "</BASEFONT></BODY>", (bool)false, (bool)false);

				if ( BankGold >= HealCost )
				{
					AddHtml( 11, 195, 61, 20, @"<BODY><BASEFONT Color=" + color + ">Bank:</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 77, 195, 100, 20, @"<BODY><BASEFONT Color=" + color + ">" + Banker.GetBalance( from ).ToString() + "</BASEFONT></BODY>", (bool)false, (bool)false);
				}
				else if ( TithePoints >= HealCost )
				{
					AddHtml( 11, 195, 61, 20, @"<BODY><BASEFONT Color=" + color + ">Tithe:</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 77, 195, 100, 20, @"<BODY><BASEFONT Color=" + color + ">" + (from.TithingPoints).ToString() + "</BASEFONT></BODY>", (bool)false, (bool)false);
				}
			}
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			from.CloseGump( typeof( ResurrectNowGump ) );
			from.SendSound( 0x0F8 ); 

			if ( info.ButtonID == 1 && !from.Alive )
			{
				bool shouldPrompt = from.SkillsTotal > 200 && ( from.RawDex + from.RawInt + from.RawStr ) > 90;

				ConfirmationGump.PromptIfFalse(
					from,
					shouldPrompt,
					() =>
					{
						from.PlaySound( 0x214 );
						from.FixedEffect( 0x376A, 10, 16 );

						from.Resurrect();

						if ( shouldPrompt )
						{
							Server.Misc.Death.Penalty( from, true );
						}

						from.Hits = from.HitsMax;
						from.Stam = from.StamMax;
						from.Mana = from.ManaMax;
						from.Hidden = true;
					},
					onConfirmed => new ConfirmationGump(
						from,
						"Accept Penalty?",
						"Are you sure you wish to accept the resurrection penalty? " + GetLossMessage( from ),
						onConfirmed
					)
				);
			}
			else { return; }
		}
	}
}

namespace Server.Misc
{
    class Death
    {
        public static void Penalty( Mobile from, bool allPenalty )
        {
			if ( from is PlayerMobile && ( ( GetPlayerInfo.isFromSpace( from ) && !allPenalty ) || allPenalty ) )
			{
				double val1 = 0.10;
				double val2 = 0.95;

				if ( GetPlayerInfo.isFromSpace( from ) && allPenalty )
				{
					val1 = 0.20;
					val2 = 0.90;
				}

				if( from.Fame > 0 ) // 10% FAME LOSS
				{
					int amount = (int)(from.Fame * val1);
					if ( from.Fame - amount < 0 ){ amount = from.Fame; }
						if ( from.Fame < 1 ){ from.Fame = 0; }
					Misc.Titles.AwardFame( from, -amount, true );
				}

				if( from.Karma > 0 ) // 10% KARMA LOSS
				{
					int amount = (int)(from.Karma * val1);
					if ( from.Karma - amount < 0 ){ amount = from.Karma; }
						if ( from.Karma < 1 ){ from.Karma = 0; }
					Misc.Titles.AwardKarma( from, -amount, true );
				}

				double loss = val2;

				if( from.RawStr * loss > 10 )
					from.RawStr = (int)(from.RawStr * loss);
						if ( from.RawStr < 10 ){ from.RawStr = 10; }
				if( from.RawInt * loss > 10 )
					from.RawInt = (int)(from.RawInt * loss);
						if ( from.RawInt < 10 ){ from.RawInt = 10; }
				if( from.RawDex * loss > 10 )
					from.RawDex = (int)(from.RawDex * loss);
						if ( from.RawDex < 10 ){ from.RawDex = 10; }

				for( int s = 0; s < from.Skills.Length; s++ )
				{
					if( from.Skills[s].Base * loss > 35 )
						from.Skills[s].Base *= loss;
				}
			}
		}
	}
}

namespace Server.Misc
{
    class GhostHelper
    {
        public static void OnGhostWalking( Mobile from )
        {
			Map map = from.Map;

			if ( map == null )
				return;

			int range = 1000; // 1000 TILES AWAY
			int HowFarAway = 0;
			int TheClosest = 1000000;
			int IsClosest = 0;
			int distchk = 0;
			int distpck = 0;

			ArrayList healers = new ArrayList();
			foreach ( Mobile healer in from.GetMobilesInRange( range ) )
			if ( healer is BaseHealer )
			{
				bool WillResurrectMe = true;

				Region reg = Region.Find( healer.Location, healer.Map );

				if ( healer is WanderingHealer || healer is EvilHealer )
				{
					WillResurrectMe = true;
				}
				else if ( ( reg.IsPartOf( "Xardok's Castle" ) || Server.Misc.Worlds.IsCrypt( from.Location, from.Map ) ) && ( from.Karma < 0 || from.Kills > 0 || from.Criminal ) )
				{
					WillResurrectMe = true;
				}
				else if ( from.Criminal || from.Kills > 0 || from.Karma < 0 )
				{
					WillResurrectMe = false;
				}

				if ( SameArea( from, healer ) == true && WillResurrectMe == true )
				{
					distchk++;
					healers.Add( healer ); 
					if ( HowFar( from.X, from.Y, healer.X, healer.Y ) < TheClosest ){ TheClosest = HowFar( from.X, from.Y, healer.X, healer.Y ); IsClosest = distchk; }
				}
			}

			int crim = 0;

			foreach ( Item shrine in from.GetItemsInRange( range ) )
			if ( shrine is AnkhWest || shrine is AnkhNorth || shrine is AltarDryad || shrine is AltarEvil || shrine is AltarDurama || shrine is AltarWizard || shrine is AltarGargoyle || shrine is AltarDaemon || shrine is AltarSea || shrine is AltarStatue || shrine is AltarShrineSouth || shrine is AltarShrineEast || shrine is AltarGodsSouth || shrine is AltarGodsEast )
			{
				Region spot = Region.Find( shrine.Location, shrine.Map );

				crim = 0;

				if ( spot.IsPartOf( typeof( VillageRegion ) ) && from.Criminal == true ){ crim = 1; }

				if ( crim == 0 )
				{
					Mobile mSp = new ShrineCritter();
					mSp.MoveToWorld(new Point3D(shrine.X, shrine.Y, shrine.Z), shrine.Map);
					if ( SameArea( from, mSp ) == true )
					{
						distchk++;
						healers.Add( mSp ); 
						if ( HowFar( from.X, from.Y, mSp.X, mSp.Y ) < TheClosest ){ TheClosest = HowFar( from.X, from.Y, mSp.X, mSp.Y ); IsClosest = distchk; }
					}
				}
			}

			for ( int h = 0; h < healers.Count; ++h )
			{
				distpck++;
				if ( distpck == IsClosest )
				{
					Mobile theHealer = ( Mobile )healers[ h ];
					HowFarAway = HowFar( from.X, from.Y, theHealer.X, theHealer.Y );
					from.QuestArrow = new GhostArrow( from, theHealer, HowFarAway*2 );
				}
			}
		}

		public static int HowFar( int x1, int y1, int x2, int y2 )
		{
            int xDelta = Math.Abs(x1 - x2);
            int yDelta = Math.Abs(y1 - y2);
            return (int)(Math.Sqrt(Math.Pow(xDelta, 2) + Math.Pow(yDelta, 2)));
		}

		public static bool SameArea( Mobile from, Mobile healer )
		{
			Map map = from.Map;
			Map mup = Map.Internal;

			int x = 9000;
			int y = 9000;
			string region = "";

			if ( healer != null ){ x = healer.X; y = healer.Y; region = Server.Misc.Worlds.GetRegionName( healer.Map, healer.Location ); mup = healer.Map; }

			Point3D location = new Point3D( from.X, from.Y, from.Z );
			Point3D loc = new Point3D( x, y, 0 );

			if ( Worlds.IsPlayerInTheLand( map, location, from.X, from.Y ) == true && Worlds.IsPlayerInTheLand( mup, loc, loc.X, loc.Y ) == true && map == mup ) // THEY ARE IN THE SAME LAND
				return true;

			else if ( region == Server.Misc.Worlds.GetRegionName( from.Map, from.Location ) ) // THEY ARE IN THE SAME REGION
				return true;

            return false;
		}
	}

	public class GhostArrow : QuestArrow
	{
		private Mobile m_From;
		private Timer m_Timer;
		private Mobile m_Target;

		public GhostArrow( Mobile from, Mobile target, int range ) : base( from, target )
		{
			m_From = from;
			m_Target = target;
			m_Timer = new GhostTimer( from, target, range, this );
			m_Timer.Start();
		}

		public override void OnClick( bool rightClick )
		{
			if ( rightClick )
			{
				m_From = null;
				Stop();
			}
		}

		public override void OnStop()
		{
			m_Timer.Stop();
		}
	}

	public class GhostTimer : Timer
	{
		private Mobile m_From, m_Target;
		private int m_Range;
		private int m_LastX, m_LastY;
		private QuestArrow m_Arrow;

		public GhostTimer( Mobile from, Mobile target, int range, QuestArrow arrow ) : base( TimeSpan.FromSeconds( 0.25 ), TimeSpan.FromSeconds( 2.5 ) )
		{
			m_From = from;
			m_Target = target;
			m_Range = range;

			m_Arrow = arrow;
		}

		protected override void OnTick()
		{
			if ( !m_Arrow.Running )
			{
				Stop();
				return;
			}
			else if ( m_From.NetState == null || m_From.Alive || m_From.Deleted || m_Target.Deleted || !m_From.InRange( m_Target, m_Range ) || GhostHelper.SameArea( m_From, m_Target ) == false )
			{
				m_Arrow.Stop();
				Stop();
				if ( !m_From.Alive ){ GhostHelper.OnGhostWalking( m_From ); }
				return;
			}

			if ( m_LastX != m_Target.X || m_LastY != m_Target.Y )
			{
				m_LastX = m_Target.X;
				m_LastY = m_Target.Y;

				m_Arrow.Update();
			}
		}
	}
}

namespace Server.Mobiles
{
	[CorpseName( "target" )]
	public class ShrineCritter : BaseCreature
	{
		[Constructable]
		public ShrineCritter() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "target";
			Body = 0;
			BaseSoundID = 0;
			Hidden = true;
			CantWalk = true;
			Timer.DelayCall( TimeSpan.FromMinutes( 10.0 ), new TimerCallback( Delete ) );

			SetSkill( SkillName.Hiding, 500.0 );
			SetSkill( SkillName.Stealth, 500.0 );
		}

		public override bool DeleteCorpseOnDeath{ get{ return true; } }

		public ShrineCritter(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( Delete ) );
		}
	}
}