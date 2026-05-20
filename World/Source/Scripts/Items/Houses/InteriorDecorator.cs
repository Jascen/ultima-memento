using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Multis;
using Server.Regions;
using Server.Spells;
using Server.Gumps;
using Server.Targeting;
using Server.Misc;

namespace Server.Items
{
	public enum DecorateCommand
	{
		None,
		Turn,
		Up,
		Down,
		North,
		East,
		South,
		West,
		Lock,
		Secure,
		Release,
		Deed,
		Trash,
		Close
	}

	public class InteriorDecorator : Item
	{
		private DecorateCommand m_Command;

		[CommandProperty( AccessLevel.GameMaster )]
		public DecorateCommand Command{ get{ return m_Command; } set{ m_Command = value; InvalidateProperties(); } }

		[Constructable]
		public InteriorDecorator() : base( 0x1EBA )
		{
			Name = "Homeowner Tools";
			Weight = 1.0;
		}

		public InteriorDecorator( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( "For Easier Home Management" );
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

		public override void OnDoubleClick( Mobile from )
		{
			if ( !CheckUse( this, from ) )
				return;

			from.CloseGump( typeof( InternalGump ) );
			from.SendGump( new InternalGump( this ) );
			from.SendSound( 0x4A ); 
		}

		public static bool InHouse( Mobile from )
		{
			BaseHouse house = BaseHouse.FindHouseAt( from );
			return ( house != null && house.IsCoOwner( from ) );
		}

		public static bool CheckUse( InteriorDecorator tool, Mobile from )
		{
			if ( !InHouse( from ) )
				from.SendLocalizedMessage( 502092 ); // You must be in your house to do this.
			else
				return true;

			return false;
		}

		public class InternalGump : Gump
		{
			private InteriorDecorator m_Decorator;

			public InternalGump( InteriorDecorator decorator ) : base( 50, 50 )
			{
				m_Decorator = decorator;
				string color = "#ebebeb";
				int button = 3609;
				int y = 50;

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				AddImage(0, 0, 3609);

				AddButton(40, 0, 4017, 4017, 13, GumpButtonType.Reply, 0);

				AddButton(0, 25, 3610, 3610, 99, GumpButtonType.Reply, 0);

				button = 3609; if ( m_Decorator.m_Command == DecorateCommand.Turn ){ button = 4017; }
				AddButton(0, y, button, button, 1, GumpButtonType.Reply, 0);
				AddHtml( 40, y, 77, 20, @"<BODY><BASEFONT Color=" + color + ">Turn</BASEFONT></BODY>", (bool)false, (bool)false);
				y=y+25;

				button = 3609; if ( m_Decorator.m_Command == DecorateCommand.Up ){ button = 4017; }
				AddButton(0, y, button, button, 2, GumpButtonType.Reply, 0);
				AddHtml( 40, y, 77, 20, @"<BODY><BASEFONT Color=" + color + ">Up</BASEFONT></BODY>", (bool)false, (bool)false);
				y=y+25;

				button = 3609; if ( m_Decorator.m_Command == DecorateCommand.Down ){ button = 4017; }
				AddButton(0, y, button, button, 3, GumpButtonType.Reply, 0);
				AddHtml( 40, y, 77, 20, @"<BODY><BASEFONT Color=" + color + ">Down</BASEFONT></BODY>", (bool)false, (bool)false);
				y=y+25;

				button = 3609; if ( m_Decorator.m_Command == DecorateCommand.North ){ button = 4017; }
				AddButton(0, y, button, button, 4, GumpButtonType.Reply, 0);
				AddHtml( 40, y, 77, 20, @"<BODY><BASEFONT Color=" + color + ">North</BASEFONT></BODY>", (bool)false, (bool)false);
				y=y+25;

				button = 3609; if ( m_Decorator.m_Command == DecorateCommand.East ){ button = 4017; }
				AddButton(0, y, button, button, 5, GumpButtonType.Reply, 0);
				AddHtml( 40, y, 77, 20, @"<BODY><BASEFONT Color=" + color + ">East</BASEFONT></BODY>", (bool)false, (bool)false);
				y=y+25;

				button = 3609;  if ( m_Decorator.m_Command == DecorateCommand.South ){ button = 4017; }
				AddButton(0, y, button, button, 6, GumpButtonType.Reply, 0);
				AddHtml( 40, y, 77, 20, @"<BODY><BASEFONT Color=" + color + ">South</BASEFONT></BODY>", (bool)false, (bool)false);
				y=y+25;

				button = 3609; if ( m_Decorator.m_Command == DecorateCommand.West ){ button = 4017; }
				AddButton(0, y, button, button, 7, GumpButtonType.Reply, 0);
				AddHtml( 40, y, 77, 20, @"<BODY><BASEFONT Color=" + color + ">West</BASEFONT></BODY>", (bool)false, (bool)false);
				y=y+25;

				button = 4005;
				AddButton(0, y, button, button, 8, GumpButtonType.Reply, 0);
				AddHtml( 40, y, 77, 20, @"<BODY><BASEFONT Color=" + color + ">Lock</BASEFONT></BODY>", (bool)false, (bool)false);
				y=y+25;

				button = 4005;
				AddButton(0, y, button, button, 9, GumpButtonType.Reply, 0);
				AddHtml( 40, y, 77, 20, @"<BODY><BASEFONT Color=" + color + ">Secure</BASEFONT></BODY>", (bool)false, (bool)false);
				y=y+25;

				button = 4005;
				AddButton(0, y, button, button, 10, GumpButtonType.Reply, 0);
				AddHtml( 40, y, 77, 20, @"<BODY><BASEFONT Color=" + color + ">Release</BASEFONT></BODY>", (bool)false, (bool)false);
				y=y+25;

				button = 3609; if ( m_Decorator.m_Command == DecorateCommand.Deed ){ button = 4017; }
				AddButton(0, y, button, button, 11, GumpButtonType.Reply, 0);
				AddHtml( 40, y, 77, 20, @"<BODY><BASEFONT Color=" + color + ">Flip Deed</BASEFONT></BODY>", (bool)false, (bool)false);
				y=y+25;

				button = 4005;
				AddButton(0, y, button, button, 12, GumpButtonType.Reply, 0);
				AddHtml( 40, y, 77, 20, @"<BODY><BASEFONT Color=" + color + ">Trash</BASEFONT></BODY>", (bool)false, (bool)false);
				y=y+25;
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				Mobile from = sender.Mobile; 

				if ( info.ButtonID == 99 )
				{
					from.SendGump( new InternalGump( m_Decorator ) );
					from.SendGump( new InfoHelpGump( from, "Flip Deed", "This option allows you to flip some deeds that can come in one of two direction facings. So if a deed states that furniture faces east, then you can set the deed on the floor of your house and flip it to face south instead. This can flip almost any deed-like items in this manner, but not all items are called 'deeds' or look like deeds. Some items behave as deeds and those can be flipped in the same manner. Tents or bear rugs, for example, have a facing and you can flip those with this command.." ) );
				}
				else
				{
					DecorateCommand command = DecorateCommand.None;

					switch ( info.ButtonID )
					{
						case 1: command = DecorateCommand.Turn; break;
						case 2: command = DecorateCommand.Up; break;
						case 3: command = DecorateCommand.Down; break;
						case 4: command = DecorateCommand.North; break;
						case 5: command = DecorateCommand.East; break;
						case 6: command = DecorateCommand.South; break;
						case 7: command = DecorateCommand.West; break;
						case 8: command = DecorateCommand.Lock; break;
						case 9: command = DecorateCommand.Secure; break;
						case 10: command = DecorateCommand.Release; break;
						case 11: command = DecorateCommand.Deed; break;
						case 12: command = DecorateCommand.Trash; break;
						case 13: command = DecorateCommand.Close; break;
					}

					if ( command == DecorateCommand.Lock )
					{
						int[] commandi = new int[] { 0x0023 };
						from.DoSpeech( "", commandi, 0, 52 );
					}
					else if ( command == DecorateCommand.Secure )
					{
						int[] commandi = new int[] { 0x0025 };
						from.DoSpeech( "", commandi, 0, 52 );
					}
					else if ( command == DecorateCommand.Release )
					{
						int[] commandi = new int[] { 0x0024 };
						from.DoSpeech( "", commandi, 0, 52 );
					}
					else if ( command == DecorateCommand.Trash )
					{
						int[] commandi = new int[] { 0x0028 };
						from.DoSpeech( "", commandi, 0, 52 );
					}
					else if ( command == DecorateCommand.Close )
					{
						from.CloseGump( typeof( InternalGump ) );
					}
					else if ( command != DecorateCommand.None )
					{
						sender.Mobile.Target = new InternalTarget( m_Decorator );
					}
					m_Decorator.Command = command;
					if ( command != DecorateCommand.Close ) { from.CloseGump( typeof( InternalGump ) ); from.SendGump( new InternalGump( m_Decorator ) ); }
				}
			}
		}

		private class InternalTarget : Target
		{
			private InteriorDecorator m_Decorator;

			public InternalTarget( InteriorDecorator decorator ) : base( -1, false, TargetFlags.None )
			{
				CheckLOS = false;
				m_Decorator = decorator;
			}

			protected override void OnTargetNotAccessible( Mobile from, object targeted )
			{
				OnTarget( from, targeted );
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Mannequin && InteriorDecorator.CheckUse( m_Decorator, from ) )
				{
					Mannequin mannequin = (Mannequin)targeted;
					BaseHouse house = BaseHouse.FindHouseAt( from );
					BaseHouse mHouse = BaseHouse.FindHouseAt( mannequin );

					if ( house == null || !house.IsCoOwner( from ) )
					{
						from.SendLocalizedMessage( 502092 ); // You must be in your house to do this.
					}
					else if ( mHouse != house )
					{
						from.SendLocalizedMessage( 1042270 ); // That is not in your house.
					}
					else if ( m_Decorator.Command != DecorateCommand.Turn )
					{
						from.SendMessage( "You can only turn a mannequin with this tool." );
					}
					else if ( mannequin.Roaming )
					{
						from.SendMessage( "You must disable roaming on the mannequin before you can turn it." );
					}
					else
					{
						Direction next = (Direction)( ( ( (int)mannequin.Direction + 1 ) & 0x7 ) );
						mannequin.Direction = next;
					}

					from.Target = new InternalTarget( m_Decorator );
					return;
				}

				if ( targeted is Item && InteriorDecorator.CheckUse( m_Decorator, from ) )
				{
					BaseHouse house = BaseHouse.FindHouseAt( from );
					Item item = (Item)targeted;

					if ( house == null || !house.IsCoOwner( from ) )
					{
						from.SendLocalizedMessage( 502092 ); // You must be in your house to do this.
					}
					else if ( ( item.Parent != null || !house.IsInside( item ) ) && !isLawn( item, from ) && !isShanty( item, from ) )
					{
						from.SendLocalizedMessage( 1042270 ); // That is not in your house.
					}
					else if ( item is VendorRentalContract )
					{
						from.SendLocalizedMessage( 1062491 ); // You cannot use the house decorator on that object.
					}
					else if ( ( isShanty( item, from ) || isLawn( item, from ) ) && ( m_Decorator.Command == DecorateCommand.Turn || m_Decorator.Command == DecorateCommand.Deed || m_Decorator.Command == DecorateCommand.Release || m_Decorator.Command == DecorateCommand.Secure || m_Decorator.Command == DecorateCommand.Lock || m_Decorator.Command == DecorateCommand.Turn || m_Decorator.Command == DecorateCommand.Turn ) )
					{
						from.SendMessage( "You can only move these items up, down, north, south, east, or west." );
					}
					else
					{
						switch ( m_Decorator.Command )
						{
							case DecorateCommand.Up:		Up( item, from );			break;
							case DecorateCommand.Down:		Down( item, from );			break;
							case DecorateCommand.Turn:		Turn( item, from );			break;
							case DecorateCommand.North:		North( item, from, house );	break;
							case DecorateCommand.East:		East( item, from, house );	break;
							case DecorateCommand.South:		South( item, from, house );	break;
							case DecorateCommand.West:		West( item, from, house );	break;
							case DecorateCommand.Deed:		Deed( item, from );			break;
						}
					}
				}

				from.Target = new InternalTarget( m_Decorator );
			}

			private static void Turn( Item item, Mobile from )
			{
				FlipableAttribute[] attributes = (FlipableAttribute[])item.GetType().GetCustomAttributes( typeof( FlipableAttribute ), false );

				if( item is BaseDoor )
				{
					from.SendMessage("You cannot move doors around with this.");
				}
				else if( item is WaxPainting )
				{
					WaxPainting sPainting = (WaxPainting)item;
					if ( item.ItemID == sPainting.PaintingFlipID1 ){ item.ItemID = sPainting.PaintingFlipID2; } else { item.ItemID = sPainting.PaintingFlipID1; }
				}
				else if( attributes.Length > 0 )
				{
					attributes[0].Flip( item );
				}
				else if( item is Container ) // FORCE FLIP SOME CONTAINERS THAT DON'T HAVE FLIP ATTRIBUTES
				{
					bool FlipMe = true;

					if( item is StolenChest )
					{
						StolenChest sBox = (StolenChest)item;
						if ( item.ItemID == sBox.ContainerFlip ){ item.ItemID = sBox.ContainerID; } else { item.ItemID = sBox.ContainerFlip; FlipMe = false; }

						if ( sBox.ContainerFlip > 0 && sBox.ContainerID > 0 && sBox.ContainerID != sBox.ContainerFlip ){ FlipMe = false; }
					}
					else if( item is HiddenBox )
					{
						HiddenBox sBox = (HiddenBox)item;
						if ( item.ItemID == sBox.ContainerFlip ){ item.ItemID = sBox.ContainerID; } else { item.ItemID = sBox.ContainerFlip; FlipMe = false; }

						if ( sBox.ContainerFlip > 0 && sBox.ContainerID > 0 && sBox.ContainerID != sBox.ContainerFlip ){ FlipMe = false; }
					}

					if ( FlipMe )
					{
						if ( item.ItemID == 0xE42 ){ item.ItemID = 0xE43; }
						else if ( item.ItemID == 0xE40 ){ item.ItemID = 0xE41; }
						else if ( item.ItemID == 0x2811 ){ item.ItemID = 0x2812; }
						else if ( item.ItemID == 0x2813 ){ item.ItemID = 0x2814; }
						else if ( item.ItemID == 0x9AA ){ item.ItemID = 0xE7D; }
						else if ( item.ItemID == 0x9A8 ){ item.ItemID = 0xE80; }
						else if ( item.ItemID == 0xE75 ){ item.ItemID = 0x53D5; }
						else if ( item.ItemID == 0xE3D ){ item.ItemID = 0xE3C; }
						else if ( item.ItemID == 0xE3F ){ item.ItemID = 0xE3E; }
						else if ( item.ItemID == 0x1AFC ){ item.ItemID = 0x1AFD; }
						else if ( item.ItemID == 0x1AFE ){ item.ItemID = 0x1AFF; }
						else if ( item.ItemID == 0x27E0 ){ item.ItemID = 0x280A; }
						else if ( item.ItemID == 0x2802 ){ item.ItemID = 0x2803; }
						else if ( item.ItemID == 0x1AFE ){ item.ItemID = 0x1AFF; }
						else if ( item.ItemID == 0x27E0 ){ item.ItemID = 0x280A; }
						else if ( item.ItemID == 0x2802 ){ item.ItemID = 0x2803; }
						else if ( item.ItemID == 0x39A0 ){ item.ItemID = 0x39A1; }
						else if ( item.ItemID == 0x39A1 ){ item.ItemID = 0x39A0; }
						else if ( item.ItemID == 0x39BD ){ item.ItemID = 0x39BE; }
						else if ( item.ItemID == 0x39BE ){ item.ItemID = 0x39BD; }
						else if ( item.ItemID == 0x3A07 ){ item.ItemID = 0x3A08; }
						else if ( item.ItemID == 0x3A08 ){ item.ItemID = 0x3A07; }
						else if ( item.ItemID == 0x2800 ){ item.ItemID = 0x2801; }
						else if ( item.ItemID == 0x27E9 ){ item.ItemID = 0x27EA; }
						else if ( item.ItemID == 0xE43 ){ item.ItemID = 0xE42; }
						else if ( item.ItemID == 0xE41 ){ item.ItemID = 0xE40; }
						else if ( item.ItemID == 0x2812 ){ item.ItemID = 0x2811; }
						else if ( item.ItemID == 0x2814 ){ item.ItemID = 0x2813; }
						else if ( item.ItemID == 0xE7D ){ item.ItemID = 0x9AA; }
						else if ( item.ItemID == 0xE80 ){ item.ItemID = 0x9A8; }
						else if ( item.ItemID == 0x53D5 ){ item.ItemID = 0xE75; }
						else if ( item.ItemID == 0xE3C ){ item.ItemID = 0xE3D; }
						else if ( item.ItemID == 0xE3E ){ item.ItemID = 0xE3F; }
						else if ( item.ItemID == 0x1AFD ){ item.ItemID = 0x1AFC; }
						else if ( item.ItemID == 0x1AFF ){ item.ItemID = 0x1AFE; }
						else if ( item.ItemID == 0x280A ){ item.ItemID = 0x27E0; }
						else if ( item.ItemID == 0x141E ){ item.ItemID = 0x141F; }
						else if ( item.ItemID == 0x1C0E ){ item.ItemID = 0x1C0F; }
						else if ( item.ItemID == 0x4C2B ){ item.ItemID = 0x4C2C; }
						else if ( item.ItemID == 0x2803 ){ item.ItemID = 0x2802; }
						else if ( item.ItemID == 0x2801 ){ item.ItemID = 0x2800; }
						else if ( item.ItemID == 0x27EA ){ item.ItemID = 0x27E9; }
						else if ( item.ItemID == 0x280B ){ item.ItemID = 0x280C; }
						else if ( item.ItemID == 0x280D ){ item.ItemID = 0x280E; }
						else if ( item.ItemID == 0x280F ){ item.ItemID = 0x2810; }
						else if ( item.ItemID == 0x281D ){ item.ItemID = 0x281E; }
						else if ( item.ItemID == 0x281F ){ item.ItemID = 0x2820; }
						else if ( item.ItemID == 0x2821 ){ item.ItemID = 0x2822; }
						else if ( item.ItemID == 0x2823 ){ item.ItemID = 0x2824; }
						else if ( item.ItemID == 0x2825 ){ item.ItemID = 0x2826; }
						else if ( item.ItemID == 0x2DF1 ){ item.ItemID = 0x2DF2; }
						else if ( item.ItemID == 0x2DF3 ){ item.ItemID = 0x2DF4; }
						else if ( item.ItemID == 0x3330 ){ item.ItemID = 0x3331; }
						else if ( item.ItemID == 0x3332 ){ item.ItemID = 0x3333; }
						else if ( item.ItemID == 0x3334 ){ item.ItemID = 0x3335; }
						else if ( item.ItemID == 0x3336 ){ item.ItemID = 0x3337; }
						else if ( item.ItemID == 0x4025 ){ item.ItemID = 0x4026; }
						else if ( item.ItemID == 0x4102 ){ item.ItemID = 0x4106; }
						else if ( item.ItemID == 0x4910 ){ item.ItemID = 0x4911; }
						else if ( item.ItemID == 0x280C ){ item.ItemID = 0x280B; }
						else if ( item.ItemID == 0x280E ){ item.ItemID = 0x280D; }
						else if ( item.ItemID == 0x2810 ){ item.ItemID = 0x280F; }
						else if ( item.ItemID == 0x281E ){ item.ItemID = 0x281D; }
						else if ( item.ItemID == 0x2820 ){ item.ItemID = 0x281F; }
						else if ( item.ItemID == 0x2822 ){ item.ItemID = 0x2821; }
						else if ( item.ItemID == 0x2824 ){ item.ItemID = 0x2823; }
						else if ( item.ItemID == 0x2826 ){ item.ItemID = 0x2825; }
						else if ( item.ItemID == 0x2DF2 ){ item.ItemID = 0x2DF1; }
						else if ( item.ItemID == 0x2DF4 ){ item.ItemID = 0x2DF3; }
						else if ( item.ItemID == 0x3331 ){ item.ItemID = 0x3330; }
						else if ( item.ItemID == 0x3333 ){ item.ItemID = 0x3332; }
						else if ( item.ItemID == 0x3335 ){ item.ItemID = 0x3334; }
						else if ( item.ItemID == 0x3337 ){ item.ItemID = 0x3336; }
						else if ( item.ItemID == 0x4026 ){ item.ItemID = 0x4025; }
						else if ( item.ItemID == 0x4106 ){ item.ItemID = 0x4102; }
						else if ( item.ItemID == 0x4911 ){ item.ItemID = 0x4910; }
						else if ( item.ItemID == 0x4FF8 ){ item.ItemID = 0x4FF9; }
						else if ( item.ItemID == 0x4FFA ){ item.ItemID = 0x4FFB; }
						else if ( item.ItemID == 0x4FFC ){ item.ItemID = 0x4FFD; }
						else if ( item.ItemID == 0x4FFE ){ item.ItemID = 0x4FFF; }
						else if ( item.ItemID == 0x5000 ){ item.ItemID = 0x5001; }
						else if ( item.ItemID == 0x5002 ){ item.ItemID = 0x5003; }
						else if ( item.ItemID == 0x5004 ){ item.ItemID = 0x5005; }
						else if ( item.ItemID == 0x5006 ){ item.ItemID = 0x5007; }
						else if ( item.ItemID == 0x5008 ){ item.ItemID = 0x5009; }
						else if ( item.ItemID == 0x500A ){ item.ItemID = 0x500B; }
						else if ( item.ItemID == 0x500C ){ item.ItemID = 0x500D; }
						else if ( item.ItemID == 0x500E ){ item.ItemID = 0x500F; }
						else if ( item.ItemID == 0x5010 ){ item.ItemID = 0x5011; }
						else if ( item.ItemID == 0x5012 ){ item.ItemID = 0x5013; }
						else if ( item.ItemID == 0x5014 ){ item.ItemID = 0x5015; }
						else if ( item.ItemID == 0x5016 ){ item.ItemID = 0x5017; }
						else if ( item.ItemID == 0x5018 ){ item.ItemID = 0x5019; }
						else if ( item.ItemID == 0x501A ){ item.ItemID = 0x501B; }
						else if ( item.ItemID == 0x501C ){ item.ItemID = 0x501D; }
						else if ( item.ItemID == 0x501E ){ item.ItemID = 0x501F; }
						else if ( item.ItemID == 0x5020 ){ item.ItemID = 0x5021; }
						else if ( item.ItemID == 0x5022 ){ item.ItemID = 0x5023; }
						else if ( item.ItemID == 0x5024 ){ item.ItemID = 0x5025; }
						else if ( item.ItemID == 0x5026 ){ item.ItemID = 0x5027; }
						else if ( item.ItemID == 0x5028 ){ item.ItemID = 0x5029; }
						else if ( item.ItemID == 0x502A ){ item.ItemID = 0x502B; }
						else if ( item.ItemID == 0x502C ){ item.ItemID = 0x502D; }
						else if ( item.ItemID == 0x502E ){ item.ItemID = 0x502F; }
						else if ( item.ItemID == 0x5030 ){ item.ItemID = 0x5031; }
						else if ( item.ItemID == 0x5032 ){ item.ItemID = 0x5033; }
						else if ( item.ItemID == 0x5034 ){ item.ItemID = 0x5035; }
						else if ( item.ItemID == 0x5036 ){ item.ItemID = 0x5037; }
						else if ( item.ItemID == 0x5038 ){ item.ItemID = 0x5039; }
						else if ( item.ItemID == 0x503A ){ item.ItemID = 0x503B; }
						else if ( item.ItemID == 0x503C ){ item.ItemID = 0x503D; }
						else if ( item.ItemID == 0x503E ){ item.ItemID = 0x503F; }
						else if ( item.ItemID == 0x5040 ){ item.ItemID = 0x5041; }
						else if ( item.ItemID == 0x5042 ){ item.ItemID = 0x5043; }
						else if ( item.ItemID == 0x5044 ){ item.ItemID = 0x5045; }
						else if ( item.ItemID == 0x5046 ){ item.ItemID = 0x5047; }
						else if ( item.ItemID == 0x5048 ){ item.ItemID = 0x5049; }
						else if ( item.ItemID == 0x504A ){ item.ItemID = 0x504B; }
						else if ( item.ItemID == 0x504C ){ item.ItemID = 0x504D; }
						else if ( item.ItemID == 0x504E ){ item.ItemID = 0x504F; }
						else if ( item.ItemID == 0x5050 ){ item.ItemID = 0x5051; }
						else if ( item.ItemID == 0x5052 ){ item.ItemID = 0x5053; }
						else if ( item.ItemID == 0x5054 ){ item.ItemID = 0x5055; }
						else if ( item.ItemID == 0x5056 ){ item.ItemID = 0x5057; }
						else if ( item.ItemID == 0x5058 ){ item.ItemID = 0x5059; }
						else if ( item.ItemID == 0x505A ){ item.ItemID = 0x505B; }
						else if ( item.ItemID == 0x505C ){ item.ItemID = 0x505D; }
						else if ( item.ItemID == 0x505E ){ item.ItemID = 0x505F; }
						else if ( item.ItemID == 0x5060 ){ item.ItemID = 0x5061; }
						else if ( item.ItemID == 0x5062 ){ item.ItemID = 0x5063; }
						else if ( item.ItemID == 0x5064 ){ item.ItemID = 0x5065; }
						else if ( item.ItemID == 0x5066 ){ item.ItemID = 0x5067; }
						else if ( item.ItemID == 0x5068 ){ item.ItemID = 0x5069; }
						else if ( item.ItemID == 0x506A ){ item.ItemID = 0x506B; }
						else if ( item.ItemID == 0x506C ){ item.ItemID = 0x506D; }
						else if ( item.ItemID == 0x5070 ){ item.ItemID = 0x5071; }
						else if ( item.ItemID == 0x507C ){ item.ItemID = 0x507D; }
						else if ( item.ItemID == 0x507E ){ item.ItemID = 0x507F; }
						else if ( item.ItemID == 0x5080 ){ item.ItemID = 0x5081; }
						else if ( item.ItemID == 0x4FF9 ){ item.ItemID = 0x4FF8; }
						else if ( item.ItemID == 0x4FFB ){ item.ItemID = 0x4FFA; }
						else if ( item.ItemID == 0x4FFD ){ item.ItemID = 0x4FFC; }
						else if ( item.ItemID == 0x4FFF ){ item.ItemID = 0x4FFE; }
						else if ( item.ItemID == 0x5001 ){ item.ItemID = 0x5000; }
						else if ( item.ItemID == 0x5003 ){ item.ItemID = 0x5002; }
						else if ( item.ItemID == 0x5005 ){ item.ItemID = 0x5004; }
						else if ( item.ItemID == 0x5007 ){ item.ItemID = 0x5006; }
						else if ( item.ItemID == 0x5009 ){ item.ItemID = 0x5008; }
						else if ( item.ItemID == 0x500B ){ item.ItemID = 0x500A; }
						else if ( item.ItemID == 0x500D ){ item.ItemID = 0x500C; }
						else if ( item.ItemID == 0x500F ){ item.ItemID = 0x500E; }
						else if ( item.ItemID == 0x5011 ){ item.ItemID = 0x5010; }
						else if ( item.ItemID == 0x5013 ){ item.ItemID = 0x5012; }
						else if ( item.ItemID == 0x5015 ){ item.ItemID = 0x5014; }
						else if ( item.ItemID == 0x5017 ){ item.ItemID = 0x5016; }
						else if ( item.ItemID == 0x5019 ){ item.ItemID = 0x5018; }
						else if ( item.ItemID == 0x501B ){ item.ItemID = 0x501A; }
						else if ( item.ItemID == 0x501D ){ item.ItemID = 0x501C; }
						else if ( item.ItemID == 0x501F ){ item.ItemID = 0x501E; }
						else if ( item.ItemID == 0x5021 ){ item.ItemID = 0x5020; }
						else if ( item.ItemID == 0x5023 ){ item.ItemID = 0x5022; }
						else if ( item.ItemID == 0x5025 ){ item.ItemID = 0x5024; }
						else if ( item.ItemID == 0x5027 ){ item.ItemID = 0x5026; }
						else if ( item.ItemID == 0x5029 ){ item.ItemID = 0x5028; }
						else if ( item.ItemID == 0x502B ){ item.ItemID = 0x502A; }
						else if ( item.ItemID == 0x502D ){ item.ItemID = 0x502C; }
						else if ( item.ItemID == 0x502F ){ item.ItemID = 0x502E; }
						else if ( item.ItemID == 0x5031 ){ item.ItemID = 0x5030; }
						else if ( item.ItemID == 0x5033 ){ item.ItemID = 0x5032; }
						else if ( item.ItemID == 0x5035 ){ item.ItemID = 0x5034; }
						else if ( item.ItemID == 0x5037 ){ item.ItemID = 0x5036; }
						else if ( item.ItemID == 0x5039 ){ item.ItemID = 0x5038; }
						else if ( item.ItemID == 0x503B ){ item.ItemID = 0x503A; }
						else if ( item.ItemID == 0x503D ){ item.ItemID = 0x503C; }
						else if ( item.ItemID == 0x503F ){ item.ItemID = 0x503E; }
						else if ( item.ItemID == 0x5041 ){ item.ItemID = 0x5040; }
						else if ( item.ItemID == 0x5043 ){ item.ItemID = 0x5042; }
						else if ( item.ItemID == 0x5045 ){ item.ItemID = 0x5044; }
						else if ( item.ItemID == 0x5047 ){ item.ItemID = 0x5046; }
						else if ( item.ItemID == 0x5049 ){ item.ItemID = 0x5048; }
						else if ( item.ItemID == 0x504B ){ item.ItemID = 0x504A; }
						else if ( item.ItemID == 0x504D ){ item.ItemID = 0x504C; }
						else if ( item.ItemID == 0x504F ){ item.ItemID = 0x504E; }
						else if ( item.ItemID == 0x5051 ){ item.ItemID = 0x5050; }
						else if ( item.ItemID == 0x5053 ){ item.ItemID = 0x5052; }
						else if ( item.ItemID == 0x5055 ){ item.ItemID = 0x5054; }
						else if ( item.ItemID == 0x5057 ){ item.ItemID = 0x5056; }
						else if ( item.ItemID == 0x5059 ){ item.ItemID = 0x5058; }
						else if ( item.ItemID == 0x505B ){ item.ItemID = 0x505A; }
						else if ( item.ItemID == 0x505D ){ item.ItemID = 0x505C; }
						else if ( item.ItemID == 0x505F ){ item.ItemID = 0x505E; }
						else if ( item.ItemID == 0x5061 ){ item.ItemID = 0x5060; }
						else if ( item.ItemID == 0x5063 ){ item.ItemID = 0x5062; }
						else if ( item.ItemID == 0x5065 ){ item.ItemID = 0x5064; }
						else if ( item.ItemID == 0x5067 ){ item.ItemID = 0x5066; }
						else if ( item.ItemID == 0x5069 ){ item.ItemID = 0x5068; }
						else if ( item.ItemID == 0x506B ){ item.ItemID = 0x506A; }
						else if ( item.ItemID == 0x506D ){ item.ItemID = 0x506C; }
						else if ( item.ItemID == 0x5071 ){ item.ItemID = 0x5070; }
						else if ( item.ItemID == 0x507D ){ item.ItemID = 0x507C; }
						else if ( item.ItemID == 0x507F ){ item.ItemID = 0x507E; }
						else if ( item.ItemID == 0x5081 ){ item.ItemID = 0x5080; }
						else if ( item.ItemID == 0x52E2 ){ item.ItemID = 0x52E3; }
						else if ( item.ItemID == 0x52E3 ){ item.ItemID = 0x52E2; }
						else if ( item.ItemID == 0x5329 ){ item.ItemID = 0x532A; }
						else if ( item.ItemID == 0x532A ){ item.ItemID = 0x5329; }
						else if ( item.ItemID == 0x52FB ){ item.ItemID = 0x52FD; }
						else if ( item.ItemID == 0x52FD ){ item.ItemID = 0x52FB; }
						else if ( item.ItemID == 0x5718 ){ item.ItemID = 0x5719; }
						else if ( item.ItemID == 0x571A ){ item.ItemID = 0x571B; }
						else if ( item.ItemID == 0x5752 ){ item.ItemID = 0x5753; }
						else if ( item.ItemID == 0x5719 ){ item.ItemID = 0x5718; }
						else if ( item.ItemID == 0x571B ){ item.ItemID = 0x571A; }
						else if ( item.ItemID == 0x5753 ){ item.ItemID = 0x5752; }
						else if ( item.ItemID == 0xA97 ){ item.ItemID = 0xA99; }
						else if ( item.ItemID == 0xA98 ){ item.ItemID = 0xA9A; }
						else if ( item.ItemID == 0xA9B ){ item.ItemID = 0xA9C; }
						else if ( item.ItemID == 0xC14 ){ item.ItemID = 0xC15; }
						else if ( item.ItemID == 0xA9D ){ item.ItemID = 0xA9E; }
						else if ( item.ItemID == 0x19FF ){ item.ItemID = 0x1A00; }
						else if ( item.ItemID == 0x2DEF ){ item.ItemID = 0x2DF0; }
						else if ( item.ItemID == 0x3BF9 ){ item.ItemID = 0x3BFA; }
						else if ( item.ItemID == 0x3BFB ){ item.ItemID = 0x3BFC; }
						else if ( item.ItemID == 0x3BFD ){ item.ItemID = 0x3BFE; }
						else if ( item.ItemID == 0x3BFF ){ item.ItemID = 0x3C00; }
						else if ( item.ItemID == 0x3C15 ){ item.ItemID = 0x3C16; }
						else if ( item.ItemID == 0x3C17 ){ item.ItemID = 0x3C18; }
						else if ( item.ItemID == 0x3C19 ){ item.ItemID = 0x3C1A; }
						else if ( item.ItemID == 0x3C1B ){ item.ItemID = 0x3C1C; }
						else if ( item.ItemID == 0x3C1D ){ item.ItemID = 0x3C1E; }
						else if ( item.ItemID == 0x3C21 ){ item.ItemID = 0x3C22; }
						else if ( item.ItemID == 0x3C23 ){ item.ItemID = 0x3C24; }
						else if ( item.ItemID == 0x3C25 ){ item.ItemID = 0x3C26; }
						else if ( item.ItemID == 0x3C27 ){ item.ItemID = 0x3C28; }
						else if ( item.ItemID == 0x3C29 ){ item.ItemID = 0x3C2A; }
						else if ( item.ItemID == 0x3C2B ){ item.ItemID = 0x3C2C; }
						else if ( item.ItemID == 0x3C2D ){ item.ItemID = 0x3C2E; }
						else if ( item.ItemID == 0x3C2F ){ item.ItemID = 0x3C30; }
						else if ( item.ItemID == 0x3C31 ){ item.ItemID = 0x3C32; }
						else if ( item.ItemID == 0x3C33 ){ item.ItemID = 0x3C34; }
						else if ( item.ItemID == 0x3C35 ){ item.ItemID = 0x3C36; }
						else if ( item.ItemID == 0x3C37 ){ item.ItemID = 0x3C38; }
						else if ( item.ItemID == 0x3C39 ){ item.ItemID = 0x3C3A; }
						else if ( item.ItemID == 0x3C3B ){ item.ItemID = 0x3C3C; }
						else if ( item.ItemID == 0x3C3D ){ item.ItemID = 0x3C3E; }
						else if ( item.ItemID == 0x3C3F ){ item.ItemID = 0x3C40; }
						else if ( item.ItemID == 0x3C41 ){ item.ItemID = 0x3C42; }
						else if ( item.ItemID == 0x3C43 ){ item.ItemID = 0x3C44; }
						else if ( item.ItemID == 0x3C45 ){ item.ItemID = 0x3C46; }
						else if ( item.ItemID == 0x3C47 ){ item.ItemID = 0x3C48; }
						else if ( item.ItemID == 0x3C49 ){ item.ItemID = 0x3C4A; }
						else if ( item.ItemID == 0x3C4B ){ item.ItemID = 0x3C4C; }
						else if ( item.ItemID == 0x3C4D ){ item.ItemID = 0x3C4E; }
						else if ( item.ItemID == 0x3C4F ){ item.ItemID = 0x3C50; }
						else if ( item.ItemID == 0x3C51 ){ item.ItemID = 0x3C52; }
						else if ( item.ItemID == 0x3C53 ){ item.ItemID = 0x3C54; }
						else if ( item.ItemID == 0x3C55 ){ item.ItemID = 0x3C56; }
						else if ( item.ItemID == 0x3C57 ){ item.ItemID = 0x3C58; }
						else if ( item.ItemID == 0x3C59 ){ item.ItemID = 0x3C5A; }
						else if ( item.ItemID == 0x3C5B ){ item.ItemID = 0x3C5C; }
						else if ( item.ItemID == 0x3C5D ){ item.ItemID = 0x3C5E; }
						else if ( item.ItemID == 0x3C5F ){ item.ItemID = 0x3C60; }
						else if ( item.ItemID == 0x3C61 ){ item.ItemID = 0x3C62; }
						else if ( item.ItemID == 0x3C63 ){ item.ItemID = 0x3C64; }
						else if ( item.ItemID == 0x3C65 ){ item.ItemID = 0x3C66; }
						else if ( item.ItemID == 0x3C67 ){ item.ItemID = 0x3C68; }
						else if ( item.ItemID == 0x3C69 ){ item.ItemID = 0x3C6A; }
						else if ( item.ItemID == 0x3C6B ){ item.ItemID = 0x3C6C; }
						else if ( item.ItemID == 0x3C6D ){ item.ItemID = 0x3C6E; }
						else if ( item.ItemID == 0x3C6F ){ item.ItemID = 0x3C70; }
						else if ( item.ItemID == 0x3C71 ){ item.ItemID = 0x3C72; }
						else if ( item.ItemID == 0x3C73 ){ item.ItemID = 0x3C74; }
						else if ( item.ItemID == 0x3C75 ){ item.ItemID = 0x3C76; }
						else if ( item.ItemID == 0x3C77 ){ item.ItemID = 0x3C78; }
						else if ( item.ItemID == 0x3C79 ){ item.ItemID = 0x3C7A; }
						else if ( item.ItemID == 0x3C7B ){ item.ItemID = 0x3C7C; }
						else if ( item.ItemID == 0x3C7D ){ item.ItemID = 0x3C7E; }
						else if ( item.ItemID == 0x3C9B ){ item.ItemID = 0x3C9C; }
						else if ( item.ItemID == 0x3C9D ){ item.ItemID = 0x3C9E; }
						else if ( item.ItemID == 0x3C9F ){ item.ItemID = 0x3CA0; }
						else if ( item.ItemID == 0x3CA1 ){ item.ItemID = 0x3CA2; }
						else if ( item.ItemID == 0x3CA3 ){ item.ItemID = 0x3CA4; }
						else if ( item.ItemID == 0x3CA5 ){ item.ItemID = 0x3CA6; }
						else if ( item.ItemID == 0x3CA7 ){ item.ItemID = 0x3CA8; }
						else if ( item.ItemID == 0x3CAD ){ item.ItemID = 0x3CAE; }
						else if ( item.ItemID == 0x3CAF ){ item.ItemID = 0x3CB0; }
						else if ( item.ItemID == 0x3CB1 ){ item.ItemID = 0x3CB2; }
						else if ( item.ItemID == 0x3CB3 ){ item.ItemID = 0x3CB4; }
						else if ( item.ItemID == 0x3CBF ){ item.ItemID = 0x3CC0; }
						else if ( item.ItemID == 0x3CC1 ){ item.ItemID = 0x3CC2; }
						else if ( item.ItemID == 0x3CC3 ){ item.ItemID = 0x3CC4; }
						else if ( item.ItemID == 0x3CC5 ){ item.ItemID = 0x3CC6; }
						else if ( item.ItemID == 0x3CC7 ){ item.ItemID = 0x3CC8; }
						else if ( item.ItemID == 0x3CD9 ){ item.ItemID = 0x3CDA; }
						else if ( item.ItemID == 0x3CDB ){ item.ItemID = 0x3CDC; }
						else if ( item.ItemID == 0x3CDD ){ item.ItemID = 0x3CDE; }
						else if ( item.ItemID == 0x3CDF ){ item.ItemID = 0x3CE0; }
						else if ( item.ItemID == 0x3CE1 ){ item.ItemID = 0x3CE2; }
						else if ( item.ItemID == 0x3CE3 ){ item.ItemID = 0x3CE4; }
						else if ( item.ItemID == 0x3CE5 ){ item.ItemID = 0x3CE6; }
						else if ( item.ItemID == 0x3CE7 ){ item.ItemID = 0x3CE8; }
						else if ( item.ItemID == 0x3CE9 ){ item.ItemID = 0x3CEA; }
						else if ( item.ItemID == 0x3CEB ){ item.ItemID = 0x3CEC; }
						else if ( item.ItemID == 0x3CED ){ item.ItemID = 0x3CEE; }
						else if ( item.ItemID == 0x3CEF ){ item.ItemID = 0x3CF0; }
						else if ( item.ItemID == 0x3CF1 ){ item.ItemID = 0x3CF2; }
						else if ( item.ItemID == 0x3CF3 ){ item.ItemID = 0x3CF4; }
						else if ( item.ItemID == 0x3CF5 ){ item.ItemID = 0x3CF6; }
						else if ( item.ItemID == 0x3CF7 ){ item.ItemID = 0x3CF8; }
						else if ( item.ItemID == 0x3CF9 ){ item.ItemID = 0x3CFA; }
						else if ( item.ItemID == 0x3CFB ){ item.ItemID = 0x3CFC; }
						else if ( item.ItemID == 0x3CFD ){ item.ItemID = 0x3D00; }
						else if ( item.ItemID == 0x3D01 ){ item.ItemID = 0x3D02; }
						else if ( item.ItemID == 0x3D03 ){ item.ItemID = 0x3D04; }
						else if ( item.ItemID == 0x3D05 ){ item.ItemID = 0x3D06; }
						else if ( item.ItemID == 0x3D07 ){ item.ItemID = 0x3D08; }
						else if ( item.ItemID == 0x3D09 ){ item.ItemID = 0x3D0A; }
						else if ( item.ItemID == 0xA99 ){ item.ItemID = 0xA97; }
						else if ( item.ItemID == 0xA9A ){ item.ItemID = 0xA98; }
						else if ( item.ItemID == 0xA9C ){ item.ItemID = 0xA9B; }
						else if ( item.ItemID == 0xC15 ){ item.ItemID = 0xC14; }
						else if ( item.ItemID == 0xA9E ){ item.ItemID = 0xA9D; }
						else if ( item.ItemID == 0x1A00 ){ item.ItemID = 0x19FF; }
						else if ( item.ItemID == 0x2DF0 ){ item.ItemID = 0x2DEF; }
						else if ( item.ItemID == 0x3BFA ){ item.ItemID = 0x3BF9; }
						else if ( item.ItemID == 0x3BFC ){ item.ItemID = 0x3BFB; }
						else if ( item.ItemID == 0x3BFE ){ item.ItemID = 0x3BFD; }
						else if ( item.ItemID == 0x3C00 ){ item.ItemID = 0x3BFF; }
						else if ( item.ItemID == 0x3C16 ){ item.ItemID = 0x3C15; }
						else if ( item.ItemID == 0x3C18 ){ item.ItemID = 0x3C17; }
						else if ( item.ItemID == 0x3C1A ){ item.ItemID = 0x3C19; }
						else if ( item.ItemID == 0x3C1C ){ item.ItemID = 0x3C1B; }
						else if ( item.ItemID == 0x3C1E ){ item.ItemID = 0x3C1D; }
						else if ( item.ItemID == 0x3C22 ){ item.ItemID = 0x3C21; }
						else if ( item.ItemID == 0x3C24 ){ item.ItemID = 0x3C23; }
						else if ( item.ItemID == 0x3C26 ){ item.ItemID = 0x3C25; }
						else if ( item.ItemID == 0x3C28 ){ item.ItemID = 0x3C27; }
						else if ( item.ItemID == 0x3C2A ){ item.ItemID = 0x3C29; }
						else if ( item.ItemID == 0x3C2C ){ item.ItemID = 0x3C2B; }
						else if ( item.ItemID == 0x3C2E ){ item.ItemID = 0x3C2D; }
						else if ( item.ItemID == 0x3C30 ){ item.ItemID = 0x3C2F; }
						else if ( item.ItemID == 0x3C32 ){ item.ItemID = 0x3C31; }
						else if ( item.ItemID == 0x3C34 ){ item.ItemID = 0x3C33; }
						else if ( item.ItemID == 0x3C36 ){ item.ItemID = 0x3C35; }
						else if ( item.ItemID == 0x3C38 ){ item.ItemID = 0x3C37; }
						else if ( item.ItemID == 0x3C3A ){ item.ItemID = 0x3C39; }
						else if ( item.ItemID == 0x3C3C ){ item.ItemID = 0x3C3B; }
						else if ( item.ItemID == 0x3C3E ){ item.ItemID = 0x3C3D; }
						else if ( item.ItemID == 0x3C40 ){ item.ItemID = 0x3C3F; }
						else if ( item.ItemID == 0x3C42 ){ item.ItemID = 0x3C41; }
						else if ( item.ItemID == 0x3C44 ){ item.ItemID = 0x3C43; }
						else if ( item.ItemID == 0x3C46 ){ item.ItemID = 0x3C45; }
						else if ( item.ItemID == 0x3C48 ){ item.ItemID = 0x3C47; }
						else if ( item.ItemID == 0x3C4A ){ item.ItemID = 0x3C49; }
						else if ( item.ItemID == 0x3C4C ){ item.ItemID = 0x3C4B; }
						else if ( item.ItemID == 0x3C4E ){ item.ItemID = 0x3C4D; }
						else if ( item.ItemID == 0x3C50 ){ item.ItemID = 0x3C4F; }
						else if ( item.ItemID == 0x3C52 ){ item.ItemID = 0x3C51; }
						else if ( item.ItemID == 0x3C54 ){ item.ItemID = 0x3C53; }
						else if ( item.ItemID == 0x3C56 ){ item.ItemID = 0x3C55; }
						else if ( item.ItemID == 0x3C58 ){ item.ItemID = 0x3C57; }
						else if ( item.ItemID == 0x3C5A ){ item.ItemID = 0x3C59; }
						else if ( item.ItemID == 0x3C5C ){ item.ItemID = 0x3C5B; }
						else if ( item.ItemID == 0x3C5E ){ item.ItemID = 0x3C5D; }
						else if ( item.ItemID == 0x3C60 ){ item.ItemID = 0x3C5F; }
						else if ( item.ItemID == 0x3C62 ){ item.ItemID = 0x3C61; }
						else if ( item.ItemID == 0x3C64 ){ item.ItemID = 0x3C63; }
						else if ( item.ItemID == 0x3C66 ){ item.ItemID = 0x3C65; }
						else if ( item.ItemID == 0x3C68 ){ item.ItemID = 0x3C67; }
						else if ( item.ItemID == 0x3C6A ){ item.ItemID = 0x3C69; }
						else if ( item.ItemID == 0x3C6C ){ item.ItemID = 0x3C6B; }
						else if ( item.ItemID == 0x3C6E ){ item.ItemID = 0x3C6D; }
						else if ( item.ItemID == 0x3C70 ){ item.ItemID = 0x3C6F; }
						else if ( item.ItemID == 0x3C72 ){ item.ItemID = 0x3C71; }
						else if ( item.ItemID == 0x3C74 ){ item.ItemID = 0x3C73; }
						else if ( item.ItemID == 0x3C76 ){ item.ItemID = 0x3C75; }
						else if ( item.ItemID == 0x3C78 ){ item.ItemID = 0x3C77; }
						else if ( item.ItemID == 0x3C7A ){ item.ItemID = 0x3C79; }
						else if ( item.ItemID == 0x3C7C ){ item.ItemID = 0x3C7B; }
						else if ( item.ItemID == 0x3C7E ){ item.ItemID = 0x3C7D; }
						else if ( item.ItemID == 0x3C9C ){ item.ItemID = 0x3C9B; }
						else if ( item.ItemID == 0x3C9E ){ item.ItemID = 0x3C9D; }
						else if ( item.ItemID == 0x3CA0 ){ item.ItemID = 0x3C9F; }
						else if ( item.ItemID == 0x3CA2 ){ item.ItemID = 0x3CA1; }
						else if ( item.ItemID == 0x3CA4 ){ item.ItemID = 0x3CA3; }
						else if ( item.ItemID == 0x3CA6 ){ item.ItemID = 0x3CA5; }
						else if ( item.ItemID == 0x3CA8 ){ item.ItemID = 0x3CA7; }
						else if ( item.ItemID == 0x3CAE ){ item.ItemID = 0x3CAD; }
						else if ( item.ItemID == 0x3CB0 ){ item.ItemID = 0x3CAF; }
						else if ( item.ItemID == 0x3CB2 ){ item.ItemID = 0x3CB1; }
						else if ( item.ItemID == 0x3CB4 ){ item.ItemID = 0x3CB3; }
						else if ( item.ItemID == 0x3CC0 ){ item.ItemID = 0x3CBF; }
						else if ( item.ItemID == 0x3CC2 ){ item.ItemID = 0x3CC1; }
						else if ( item.ItemID == 0x3CC4 ){ item.ItemID = 0x3CC3; }
						else if ( item.ItemID == 0x3CC6 ){ item.ItemID = 0x3CC5; }
						else if ( item.ItemID == 0x3CC8 ){ item.ItemID = 0x3CC7; }
						else if ( item.ItemID == 0x3CDA ){ item.ItemID = 0x3CD9; }
						else if ( item.ItemID == 0x3CDC ){ item.ItemID = 0x3CDB; }
						else if ( item.ItemID == 0x3CDE ){ item.ItemID = 0x3CDD; }
						else if ( item.ItemID == 0x3CE0 ){ item.ItemID = 0x3CDF; }
						else if ( item.ItemID == 0x3CE2 ){ item.ItemID = 0x3CE1; }
						else if ( item.ItemID == 0x3CE4 ){ item.ItemID = 0x3CE3; }
						else if ( item.ItemID == 0x3CE6 ){ item.ItemID = 0x3CE5; }
						else if ( item.ItemID == 0x3CE8 ){ item.ItemID = 0x3CE7; }
						else if ( item.ItemID == 0x3CEA ){ item.ItemID = 0x3CE9; }
						else if ( item.ItemID == 0x3CEC ){ item.ItemID = 0x3CEB; }
						else if ( item.ItemID == 0x3CEE ){ item.ItemID = 0x3CED; }
						else if ( item.ItemID == 0x3CF0 ){ item.ItemID = 0x3CEF; }
						else if ( item.ItemID == 0x3CF2 ){ item.ItemID = 0x3CF1; }
						else if ( item.ItemID == 0x3CF4 ){ item.ItemID = 0x3CF3; }
						else if ( item.ItemID == 0x3CF6 ){ item.ItemID = 0x3CF5; }
						else if ( item.ItemID == 0x3CF8 ){ item.ItemID = 0x3CF7; }
						else if ( item.ItemID == 0x3CFA ){ item.ItemID = 0x3CF9; }
						else if ( item.ItemID == 0x3CFC ){ item.ItemID = 0x3CFB; }
						else if ( item.ItemID == 0x3D00 ){ item.ItemID = 0x3CFD; }
						else if ( item.ItemID == 0x3D02 ){ item.ItemID = 0x3D01; }
						else if ( item.ItemID == 0x3D04 ){ item.ItemID = 0x3D03; }
						else if ( item.ItemID == 0x3D06 ){ item.ItemID = 0x3D05; }
						else if ( item.ItemID == 0x3D08 ){ item.ItemID = 0x3D07; }
						else if ( item.ItemID == 0x3D0A ){ item.ItemID = 0x3D09; }
						else if ( item.ItemID == 0x569B ){ item.ItemID = 0x569C; }
						else if ( item.ItemID == 0x569C ){ item.ItemID = 0x569B; }
						else if ( item.ItemID == 0x569D ){ item.ItemID = 0x569E; }
						else if ( item.ItemID == 0x569E ){ item.ItemID = 0x569D; }
						else if ( item.ItemID == 0x569F ){ item.ItemID = 0x56A0; }
						else if ( item.ItemID == 0x56A0 ){ item.ItemID = 0x569F; }
						else if ( item.ItemID == 0x56A1 ){ item.ItemID = 0x56A2; }
						else if ( item.ItemID == 0x56A2 ){ item.ItemID = 0x56A1; }
						else if ( item.ItemID == 0x56A3 ){ item.ItemID = 0x56A4; }
						else if ( item.ItemID == 0x56A4 ){ item.ItemID = 0x56A3; }
						else if ( item.ItemID == 0x56A5 ){ item.ItemID = 0x56A6; }
						else if ( item.ItemID == 0x56A6 ){ item.ItemID = 0x56A5; }
						else if ( item.ItemID == 0x56A7 ){ item.ItemID = 0x56A8; }
						else if ( item.ItemID == 0x56A8 ){ item.ItemID = 0x56A7; }
						else if ( item.ItemID == 0x56A9 ){ item.ItemID = 0x56AA; }
						else if ( item.ItemID == 0x56AA ){ item.ItemID = 0x56A9; }
						else if ( item.ItemID == 0x570E ){ item.ItemID = 0x570D; }
						else if ( item.ItemID == 0x570D ){ item.ItemID = 0x570E; }
						else if ( item.ItemID == 0x570C ){ item.ItemID = 0x570B; }
						else if ( item.ItemID == 0x570B ){ item.ItemID = 0x570C; }
						else if ( item.ItemID == 0x575A ){ item.ItemID = 0x5754; }
						else if ( item.ItemID == 0x5754 ){ item.ItemID = 0x575A; }
						else if ( item.ItemID == 0x5755 ){ item.ItemID = 0x575B; }
						else if ( item.ItemID == 0x575B ){ item.ItemID = 0x5755; }
					}
				}
				else
				{
					if ( item.ItemID == 8784 ){ item.ItemID = 8785; }
					else if ( item.ItemID == 8782 ){ item.ItemID = 8783; }
					else if ( item.ItemID == 0x1AEE ){ item.ItemID = 0x1AEF; }
					else if ( item.ItemID == 0x3DCC ){ item.ItemID = 0x3DCD; }
					else if ( item.ItemID == 0x3DE0 ){ item.ItemID = 0x3DE1; }
					else if ( item.ItemID == 0x224 ){ item.ItemID = 0x225; }
					else if ( item.ItemID == 8785 ){ item.ItemID = 8784; }
					else if ( item.ItemID == 8783 ){ item.ItemID = 8782; }
					else if ( item.ItemID == 0x1AEF ){ item.ItemID = 0x1AEE; }
					else if ( item.ItemID == 0x3DCD ){ item.ItemID = 0x3DCC; }
					else if ( item.ItemID == 0x3DE1 ){ item.ItemID = 0x3DE0; }
					else if ( item.ItemID == 0x225 ){ item.ItemID = 0x224; }
					else if ( item.ItemID == 0x3158 ){ item.ItemID = 0x3159; }
					else if ( item.ItemID == 0x3159 ){ item.ItemID = 0x3158; }
					else if ( item.ItemID == 0x3912 ){ item.ItemID = 0x3913; }
					else if ( item.ItemID == 0x3913 ){ item.ItemID = 0x3912; }
					else if ( item.ItemID == 0x3944 ){ item.ItemID = 0x3945; }
					else if ( item.ItemID == 0x3945 ){ item.ItemID = 0x3944; }
					else if ( item.ItemID == 0x2A79 ){ item.ItemID = 0x2A7A; }
					else if ( item.ItemID == 0x2A7A ){ item.ItemID = 0x2A79; }
					else if ( item.ItemID == 0x2A75 ){ item.ItemID = 0x2A76; }
					else if ( item.ItemID == 0x2A76 ){ item.ItemID = 0x2A75; }
					else if ( item.ItemID == 0x2A71 ){ item.ItemID = 0x2A72; }
					else if ( item.ItemID == 0x2A72 ){ item.ItemID = 0x2A71; }
					else if ( item.ItemID == 0x2A77 ){ item.ItemID = 0x2A78; }
					else if ( item.ItemID == 0x2A78 ){ item.ItemID = 0x2A77; }
					else if ( item.ItemID == 0x2A73 ){ item.ItemID = 0x2A74; }
					else if ( item.ItemID == 0x2A74 ){ item.ItemID = 0x2A73; }
					else if ( item.ItemID == 0x1E6A ){ item.ItemID = 0x1E63; }
					else if ( item.ItemID == 0x1E63 ){ item.ItemID = 0x1E6A; }
					else if ( item.ItemID == 0x1E68 ){ item.ItemID = 0x1E61; }
					else if ( item.ItemID == 0x1E61 ){ item.ItemID = 0x1E68; }
					else if ( item.ItemID == 0x1E6B ){ item.ItemID = 0x1E64; }
					else if ( item.ItemID == 0x1E64 ){ item.ItemID = 0x1E6B; }
					else if ( item.ItemID == 0x1E6D ){ item.ItemID = 0x1E66; }
					else if ( item.ItemID == 0x1E66 ){ item.ItemID = 0x1E6D; }
					else if ( item.ItemID == 0x1E6C ){ item.ItemID = 0x1E65; }
					else if ( item.ItemID == 0x1E65 ){ item.ItemID = 0x1E6C; }
					else if ( item.ItemID == 0x1E67 ){ item.ItemID = 0x1E60; }
					else if ( item.ItemID == 0x1E60 ){ item.ItemID = 0x1E67; }
					else if ( item.ItemID == 0x392D ){ item.ItemID = 0x392C; } // black daemon
					else if ( item.ItemID == 0x392C ){ item.ItemID = 0x392D; } // black daemon
					else if ( item.ItemID == 0x3933 ){ item.ItemID = 0x3932; }
					else if ( item.ItemID == 0x3932 ){ item.ItemID = 0x3933; }
					else if ( item.ItemID == 0x393D ){ item.ItemID = 0x393C; }
					else if ( item.ItemID == 0x393C ){ item.ItemID = 0x393D; }
					else if ( item.ItemID == 0x3931 ){ item.ItemID = 0x3930; }
					else if ( item.ItemID == 0x3930 ){ item.ItemID = 0x3931; }
					else if ( item.ItemID == 0x3935 ){ item.ItemID = 0x3934; }
					else if ( item.ItemID == 0x3934 ){ item.ItemID = 0x3935; }
					else if ( item.ItemID == 0x52FE ){ item.ItemID = 0x52FF; }
					else if ( item.ItemID == 0x52FF ){ item.ItemID = 0x52FE; }
					else if ( item.ItemID == 0x5300 ){ item.ItemID = 0x5301; }
					else if ( item.ItemID == 0x5301 ){ item.ItemID = 0x5300; }
					else if ( item.ItemID == 0x4FDB ){ item.ItemID = 0x4FDC; }
					else if ( item.ItemID == 0x4FDC ){ item.ItemID = 0x4FDB; }
					else if ( item.ItemID == 0x4FE1 ){ item.ItemID = 0x4FE2; }
					else if ( item.ItemID == 0x4FE2 ){ item.ItemID = 0x4FE1; }
					else if ( item.ItemID == 0x4FE3 ){ item.ItemID = 0x4FE4; }
					else if ( item.ItemID == 0x4FE4 ){ item.ItemID = 0x4FE3; }
					else if ( item.ItemID == 0x4FE6 ){ item.ItemID = 0x4FE7; }
					else if ( item.ItemID == 0x4FE7 ){ item.ItemID = 0x4FE6; }
					else if ( item.ItemID == 0x4FEA ){ item.ItemID = 0x4FEB; }
					else if ( item.ItemID == 0x4FEB ){ item.ItemID = 0x4FEA; }
					else if ( item.ItemID == 0x4FEC ){ item.ItemID = 0x4FED; }
					else if ( item.ItemID == 0x4FED ){ item.ItemID = 0x4FEC; }
					else if ( item.ItemID == 0x4FEE ){ item.ItemID = 0x4FEF; }
					else if ( item.ItemID == 0x4FEF ){ item.ItemID = 0x4FEE; }
					else if ( item.ItemID == 0x4FF0 ){ item.ItemID = 0x4FF1; }
					else if ( item.ItemID == 0x4FF1 ){ item.ItemID = 0x4FF0; }
					else if ( item.ItemID == 0x4FF2 ){ item.ItemID = 0x4FF3; }
					else if ( item.ItemID == 0x4FF3 ){ item.ItemID = 0x4FF2; }
					else if ( item.ItemID == 0x4FF4 ){ item.ItemID = 0x4FF5; }
					else if ( item.ItemID == 0x4FF5 ){ item.ItemID = 0x4FF4; }
					else if ( item.ItemID == 0x392B ){ item.ItemID = 0x392A; }
					else if ( item.ItemID == 0x392A ){ item.ItemID = 0x392B; }
					else if ( item.ItemID == 0x3937 ){ item.ItemID = 0x3936; }
					else if ( item.ItemID == 0x3936 ){ item.ItemID = 0x3937; }
					else if ( item.ItemID == 0x393F ){ item.ItemID = 0x393E; }
					else if ( item.ItemID == 0x393E ){ item.ItemID = 0x393F; }
					else if ( item.ItemID == 0x392F ){ item.ItemID = 0x392E; }
					else if ( item.ItemID == 0x392E ){ item.ItemID = 0x392F; }
					else if ( item.ItemID == 0x393B ){ item.ItemID = 0x393A; }
					else if ( item.ItemID == 0x393A ){ item.ItemID = 0x393B; }
					else if ( item.ItemID == 0x2235 ){ item.ItemID = 0x2234; }
					else if ( item.ItemID == 0x2234 ){ item.ItemID = 0x2235; }
					else if ( item.ItemID == 0x21FB ){ item.ItemID = 0x21FA; }
					else if ( item.ItemID == 0x21FA ){ item.ItemID = 0x21FB; }
					else if ( item.ItemID == 0x270D ){ item.ItemID = 0x270E; }
					else if ( item.ItemID == 0x270E ){ item.ItemID = 0x270D; }
					else if ( item.ItemID == 0x21F9 ){ item.ItemID = 0x21F8; }
					else if ( item.ItemID == 0x21F8 ){ item.ItemID = 0x21F9; }
					else if ( item.ItemID == 0x21F7 ){ item.ItemID = 0x21F6; }
					else if ( item.ItemID == 0x21F6 ){ item.ItemID = 0x21F7; }
					else if ( item.ItemID == 0x44E8 ){ item.ItemID = 0x44E7; }
					else if ( item.ItemID == 0x44E7 ){ item.ItemID = 0x44E8; }
					else if ( item.ItemID == 0x1E69 ){ item.ItemID = 0x1E62; }
					else if ( item.ItemID == 0x1E62 ){ item.ItemID = 0x1E69; }
					else if ( item.ItemID == 0x3352 ){ item.ItemID = 0x3353; } //mounted abyss giant
					else if ( item.ItemID == 0x3353 ){ item.ItemID = 0x3352; } //mounted abyss giant
					else if ( item.ItemID == 0x3354 ){ item.ItemID = 0x3355; } //mounted abyss ogre
					else if ( item.ItemID == 0x3355 ){ item.ItemID = 0x3354; } //mounted abyss ogre
					else if ( item.ItemID == 0x3356 ){ item.ItemID = 0x3357; } //mounted ancient cyclops
					else if ( item.ItemID == 0x3357 ){ item.ItemID = 0x3356; } //mounted ancient cyclops
					else if ( item.ItemID == 0x3358 ){ item.ItemID = 0x3359; } //mounted ancient drake
					else if ( item.ItemID == 0x3359 ){ item.ItemID = 0x3358; } //mounted ancient drake
					else if ( item.ItemID == 0x335A ){ item.ItemID = 0x335B; } //mounted cerberus
					else if ( item.ItemID == 0x335B ){ item.ItemID = 0x335A; } //mounted cerberus
					else if ( item.ItemID == 0x335C ){ item.ItemID = 0x335D; } //mounted storm giant
					else if ( item.ItemID == 0x335D ){ item.ItemID = 0x335C; } //mounted storm giant
					else if ( item.ItemID == 0x335E ){ item.ItemID = 0x335F; } //mounted dark unicorn
					else if ( item.ItemID == 0x335F ){ item.ItemID = 0x335E; } //mounted dark unicorn
					else if ( item.ItemID == 0x3360 ){ item.ItemID = 0x3361; } //mounted deep sea giant
					else if ( item.ItemID == 0x3361 ){ item.ItemID = 0x3360; } //mounted deep sea giant
					else if ( item.ItemID == 0x3362 ){ item.ItemID = 0x3363; } //mounted dinosaur
					else if ( item.ItemID == 0x3363 ){ item.ItemID = 0x3362; } //mounted dinosaur
					else if ( item.ItemID == 0x3364 ){ item.ItemID = 0x3365; } //mounted dracolich
					else if ( item.ItemID == 0x3365 ){ item.ItemID = 0x3364; } //mounted dracolich
					else if ( item.ItemID == 0x3366 ){ item.ItemID = 0x3367; } //mounted dragon turtle
					else if ( item.ItemID == 0x3367 ){ item.ItemID = 0x3366; } //mounted dragon turtle
					else if ( item.ItemID == 0x33FD ){ item.ItemID = 0x33FE; } //mounted wyrm
					else if ( item.ItemID == 0x33FE ){ item.ItemID = 0x33FD; } //mounted wyrm
					else if ( item.ItemID == 0x3368 ){ item.ItemID = 0x3369; } //mounted drake
					else if ( item.ItemID == 0x3369 ){ item.ItemID = 0x3368; } //mounted drake
					else if ( item.ItemID == 0x336A ){ item.ItemID = 0x336B; } //mounted flesh golem
					else if ( item.ItemID == 0x336B ){ item.ItemID = 0x336A; } //mounted flesh golem
					else if ( item.ItemID == 0x336C ){ item.ItemID = 0x336D; } //mounted forest giant
					else if ( item.ItemID == 0x336D ){ item.ItemID = 0x336C; } //mounted forest giant
					else if ( item.ItemID == 0x336E ){ item.ItemID = 0x336F; } //mounted frost giant
					else if ( item.ItemID == 0x336F ){ item.ItemID = 0x336E; } //mounted frost giant
					else if ( item.ItemID == 0x3370 ){ item.ItemID = 0x3371; } //mounted griffon
					else if ( item.ItemID == 0x3371 ){ item.ItemID = 0x3370; } //mounted griffon
					else if ( item.ItemID == 0x3372 ){ item.ItemID = 0x3373; } //mounted hydra
					else if ( item.ItemID == 0x3373 ){ item.ItemID = 0x3372; } //mounted hydra
					else if ( item.ItemID == 0x3374 ){ item.ItemID = 0x3375; } //mounted jungle giant
					else if ( item.ItemID == 0x3375 ){ item.ItemID = 0x3374; } //mounted jungle giant
					else if ( item.ItemID == 0x3376 ){ item.ItemID = 0x3377; } //mounted lion
					else if ( item.ItemID == 0x3377 ){ item.ItemID = 0x3376; } //mounted lion
					else if ( item.ItemID == 0x3378 ){ item.ItemID = 0x3379; } //mounted ogre lord
					else if ( item.ItemID == 0x3379 ){ item.ItemID = 0x3378; } //mounted ogre lord
					else if ( item.ItemID == 0x337B ){ item.ItemID = 0x337C; } //mounted owlbear
					else if ( item.ItemID == 0x337C ){ item.ItemID = 0x337B; } //mounted owlbear
					else if ( item.ItemID == 0x337D ){ item.ItemID = 0x337E; } //mounted sea daemon
					else if ( item.ItemID == 0x337E ){ item.ItemID = 0x337D; } //mounted sea daemon
					else if ( item.ItemID == 0x337F ){ item.ItemID = 0x3380; } //mounted sea dragon
					else if ( item.ItemID == 0x3380 ){ item.ItemID = 0x337F; } //mounted sea dragon
					else if ( item.ItemID == 0x3381 ){ item.ItemID = 0x3382; } //mounted sea drake
					else if ( item.ItemID == 0x3382 ){ item.ItemID = 0x3381; } //mounted sea drake
					else if ( item.ItemID == 0x3383 ){ item.ItemID = 0x3384; } //mounted sea giant
					else if ( item.ItemID == 0x3384 ){ item.ItemID = 0x3383; } //mounted sea giant
					else if ( item.ItemID == 0x3385 ){ item.ItemID = 0x3386; } //mounted swamp drake
					else if ( item.ItemID == 0x3386 ){ item.ItemID = 0x3385; } //mounted swamp drake
					else if ( item.ItemID == 0x3387 ){ item.ItemID = 0x3388; } //mounted swamp thing
					else if ( item.ItemID == 0x3388 ){ item.ItemID = 0x3387; } //mounted swamp thing
					else if ( item.ItemID == 0x3389 ){ item.ItemID = 0x338A; } //mounted tiger
					else if ( item.ItemID == 0x338A ){ item.ItemID = 0x3389; } //mounted tiger
					else if ( item.ItemID == 0x338B ){ item.ItemID = 0x338C; } //mounted earth titan
					else if ( item.ItemID == 0x338C ){ item.ItemID = 0x338B; } //mounted earth titan
					else if ( item.ItemID == 0x338D ){ item.ItemID = 0x338E; } //mounted fire titan
					else if ( item.ItemID == 0x338E ){ item.ItemID = 0x338D; } //mounted fire titan
					else if ( item.ItemID == 0x338F ){ item.ItemID = 0x3390; } //mounted water titan
					else if ( item.ItemID == 0x3390 ){ item.ItemID = 0x338F; } //mounted water titan
					else if ( item.ItemID == 0x3391 ){ item.ItemID = 0x3392; } //mounted tyranasaur
					else if ( item.ItemID == 0x3392 ){ item.ItemID = 0x3391; } //mounted tyranasaur
					else if ( item.ItemID == 0x3393 ){ item.ItemID = 0x3394; } //mounted stegosaurus
					else if ( item.ItemID == 0x3394 ){ item.ItemID = 0x3393; } //mounted stegosaurus
					else if ( item.ItemID == 0x3395 ){ item.ItemID = 0x3396; } //mounted alien
					else if ( item.ItemID == 0x3396 ){ item.ItemID = 0x3395; } //mounted alien
					else if ( item.ItemID == 0x3397 ){ item.ItemID = 0x3398; } //mounted wyvern
					else if ( item.ItemID == 0x3398 ){ item.ItemID = 0x3397; } //mounted wyvern
					else if ( item.ItemID == 0x3399 ){ item.ItemID = 0x339A; } //mounted black bear
					else if ( item.ItemID == 0x339A ){ item.ItemID = 0x3399; } //mounted black bear
					else if ( item.ItemID == 0x339B ){ item.ItemID = 0x339C; } //mounted brown bear
					else if ( item.ItemID == 0x339C ){ item.ItemID = 0x339B; } //mounted brown bear
					else if ( item.ItemID == 0x339D ){ item.ItemID = 0x339E; } //mounted cave bear
					else if ( item.ItemID == 0x339E ){ item.ItemID = 0x339D; } //mounted cave bear
					else if ( item.ItemID == 0x339F ){ item.ItemID = 0x33A0; } //mounted polar bear
					else if ( item.ItemID == 0x33A0 ){ item.ItemID = 0x339F; } //mounted polar bear
					else if ( item.ItemID == 0x33A1 ){ item.ItemID = 0x33A2; } //mounted daemon
					else if ( item.ItemID == 0x33A2 ){ item.ItemID = 0x33A1; } //mounted daemon
					else if ( item.ItemID == 0x567F ){ item.ItemID = 0x5680; } //mounted daemon
					else if ( item.ItemID == 0x5680 ){ item.ItemID = 0x567F; } //mounted daemon
					else if ( item.ItemID == 0x5681 ){ item.ItemID = 0x5682; } //mounted balron
					else if ( item.ItemID == 0x5682 ){ item.ItemID = 0x5681; } //mounted balron
					else if ( item.ItemID == 0x33A3 ){ item.ItemID = 0x33A4; } //mounted dragon
					else if ( item.ItemID == 0x33A4 ){ item.ItemID = 0x33A3; } //mounted dragon
					else if ( item.ItemID == 0x33A5 ){ item.ItemID = 0x33A6; } //mounted dragon
					else if ( item.ItemID == 0x33A6 ){ item.ItemID = 0x33A5; } //mounted dragon
					else if ( item.ItemID == 0x33A7 ){ item.ItemID = 0x33A8; } //mounted ettin mage
					else if ( item.ItemID == 0x33A8 ){ item.ItemID = 0x33A7; } //mounted ettin mage
					else if ( item.ItemID == 0x33A9 ){ item.ItemID = 0x33AA; } //mounted hill giant
					else if ( item.ItemID == 0x33AA ){ item.ItemID = 0x33A9; } //mounted hill giant
					else if ( item.ItemID == 0x33AB ){ item.ItemID = 0x33AC; } //mounted lizardman
					else if ( item.ItemID == 0x33AC ){ item.ItemID = 0x33AB; } //mounted lizardman
					else if ( item.ItemID == 0x33AD ){ item.ItemID = 0x33AE; } //mounted nightmare
					else if ( item.ItemID == 0x33AE ){ item.ItemID = 0x33AD; } //mounted nightmare
					else if ( item.ItemID == 0x33AF ){ item.ItemID = 0x33B0; } //mounted satan
					else if ( item.ItemID == 0x33B0 ){ item.ItemID = 0x33AF; } //mounted satan
					else if ( item.ItemID == 0x33B1 ){ item.ItemID = 0x33B2; } //mounted unicorn
					else if ( item.ItemID == 0x33B2 ){ item.ItemID = 0x33B1; } //mounted unicorn
					else if ( item.ItemID == 0x33B3 ){ item.ItemID = 0x33B4; } //mounted skeletal dragon
					else if ( item.ItemID == 0x33B4 ){ item.ItemID = 0x33B3; } //mounted skeletal dragon
					else if ( item.ItemID == 0x33B5 ){ item.ItemID = 0x33B6; } //mounted wyvern
					else if ( item.ItemID == 0x33B6 ){ item.ItemID = 0x33B5; } //mounted wyvern
					else if ( item.ItemID == 0x33B9 ){ item.ItemID = 0x33BA; } //mounted alien 
					else if ( item.ItemID == 0x33BA ){ item.ItemID = 0x33B9; } //mounted alien 
					else if ( item.ItemID == 0x33CB ){ item.ItemID = 0x33CC; } //mounted dragon abysmal
					else if ( item.ItemID == 0x33CC ){ item.ItemID = 0x33CB; } //mounted dragon abysmal
					else if ( item.ItemID == 0x33D1 ){ item.ItemID = 0x33D2; } //mounted dragon amber
					else if ( item.ItemID == 0x33D2 ){ item.ItemID = 0x33D1; } //mounted dragon amber
					else if ( item.ItemID == 0x33CF ){ item.ItemID = 0x33D0; } //mounted dragon cinder
					else if ( item.ItemID == 0x33D0 ){ item.ItemID = 0x33CF; } //mounted dragon cinder
					else if ( item.ItemID == 0x33D7 ){ item.ItemID = 0x33D8; } //mounted dragon fire, primeval
					else if ( item.ItemID == 0x33D8 ){ item.ItemID = 0x33D7; } //mounted dragon fire, primeval
					else if ( item.ItemID == 0x33C1 ){ item.ItemID = 0x33C2; } //mounted dragon green, primeval
					else if ( item.ItemID == 0x33C2 ){ item.ItemID = 0x33C1; } //mounted dragon green, primeval
					else if ( item.ItemID == 0x33D3 ){ item.ItemID = 0x33D4; } //mounted dragon night
					else if ( item.ItemID == 0x33D4 ){ item.ItemID = 0x33D3; } //mounted dragon night
					else if ( item.ItemID == 0x33D9 ){ item.ItemID = 0x33DA; } //mounted dragon primeval
					else if ( item.ItemID == 0x33DA ){ item.ItemID = 0x33D9; } //mounted dragon primeval
					else if ( item.ItemID == 0x33D5 ){ item.ItemID = 0x33D6; } //mounted dragon reanimated
					else if ( item.ItemID == 0x33D6 ){ item.ItemID = 0x33D5; } //mounted dragon reanimated
					else if ( item.ItemID == 0x33C5 ){ item.ItemID = 0x33C6; } //mounted dragon red, primeval
					else if ( item.ItemID == 0x33C6 ){ item.ItemID = 0x33C5; } //mounted dragon red, primeval
					else if ( item.ItemID == 0x33C3 ){ item.ItemID = 0x33C4; } //mounted dragon royal
					else if ( item.ItemID == 0x33C4 ){ item.ItemID = 0x33C3; } //mounted dragon royal
					else if ( item.ItemID == 0x33BF ){ item.ItemID = 0x33C0; } //mounted dragon rune
					else if ( item.ItemID == 0x33C0 ){ item.ItemID = 0x33BF; } //mounted dragon rune
					else if ( item.ItemID == 0x33C7 ){ item.ItemID = 0x33C8; } //mounted dragon sea, primeval
					else if ( item.ItemID == 0x33C8 ){ item.ItemID = 0x33C7; } //mounted dragon sea, primeval
					else if ( item.ItemID == 0x33C9 ){ item.ItemID = 0x33CA; } //mounted dragon stygian
					else if ( item.ItemID == 0x33CA ){ item.ItemID = 0x33C9; } //mounted dragon stygian
					else if ( item.ItemID == 0x33CD ){ item.ItemID = 0x33CE; } //mounted dragon vampiric
					else if ( item.ItemID == 0x33CE ){ item.ItemID = 0x33CD; } //mounted dragon vampiric
					else if ( item.ItemID == 0x33DB ){ item.ItemID = 0x33DC; } //mounted hell beast 
					else if ( item.ItemID == 0x33DC ){ item.ItemID = 0x33DB; } //mounted hell beast 
					else if ( item.ItemID == 0x33DD ){ item.ItemID = 0x33DE; } //mounted hippogriff 
					else if ( item.ItemID == 0x33DE ){ item.ItemID = 0x33DD; } //mounted hippogriff 
					else if ( item.ItemID == 0x33BD ){ item.ItemID = 0x33BE; } //mounted lion 
					else if ( item.ItemID == 0x33BE ){ item.ItemID = 0x33BD; } //mounted lion 
					else if ( item.ItemID == 0x33B7 ){ item.ItemID = 0x33B8; } //mounted styguana 
					else if ( item.ItemID == 0x33B8 ){ item.ItemID = 0x33B7; } //mounted styguana 
					else if ( item.ItemID == 0x33BB ){ item.ItemID = 0x33BC; } //mounted watcher 
					else if ( item.ItemID == 0x33BC ){ item.ItemID = 0x33BB; } //mounted watcher 
					else if ( item.ItemID == 0x33DF ){ item.ItemID = 0x33E0; } //mounted walrus 
					else if ( item.ItemID == 0x33E0 ){ item.ItemID = 0x33DF; } //mounted walrus 
					else if ( item.ItemID == 0x33E1 ){ item.ItemID = 0x33E2; } //mounted ogre 
					else if ( item.ItemID == 0x33E2 ){ item.ItemID = 0x33E1; } //mounted ogre 
					else if ( item.ItemID == 0x33E3 ){ item.ItemID = 0x33E4; } //mounted trollbear 
					else if ( item.ItemID == 0x33E4 ){ item.ItemID = 0x33E3; } //mounted trollbear 
					else if ( item.ItemID == 0x3E20 ){ item.ItemID = 0x3E21; }
					else if ( item.ItemID == 0x3E7 ){ item.ItemID = 0xC2C; }
					else if ( item.ItemID == 0x3E8 ){ item.ItemID = 0xEA0; }
					else if ( item.ItemID == 0x2A5D ){ item.ItemID = 0x2A61; }
					else if ( item.ItemID == 0x3EA ){ item.ItemID = 0xEA7; }
					else if ( item.ItemID == 0x3EB ){ item.ItemID = 0xEA6; }
					else if ( item.ItemID == 0x3EC ){ item.ItemID = 0xEA5; }
					else if ( item.ItemID == 0x3ED ){ item.ItemID = 0xE55; }
					else if ( item.ItemID == 0xDDF ){ item.ItemID = 0xE9F; }
					else if ( item.ItemID == 0xEA3 ){ item.ItemID = 0xEA4; }
					else if ( item.ItemID == 0xEA1 ){ item.ItemID = 0xEA2; }
					else if ( item.ItemID == 0x2A65 ){ item.ItemID = 0x2A68; }
					else if ( item.ItemID == 0x3308 ){ item.ItemID = 0x3E0C; }
					else if ( item.ItemID == 0x3309 ){ item.ItemID = 0x3E0D; }
					else if ( item.ItemID == 0x330A ){ item.ItemID = 0x3E0E; }
					else if ( item.ItemID == 0x330B ){ item.ItemID = 0x3E0F; }
					else if ( item.ItemID == 0x330C ){ item.ItemID = 0x3E10; }
					else if ( item.ItemID == 0x330D ){ item.ItemID = 0x3E11; }
					else if ( item.ItemID == 0x330E ){ item.ItemID = 0x3E12; }
					else if ( item.ItemID == 0x330F ){ item.ItemID = 0x3E13; }
					else if ( item.ItemID == 0x3310 ){ item.ItemID = 0x3E14; }
					else if ( item.ItemID == 0x3311 ){ item.ItemID = 0x3E15; }
					else if ( item.ItemID == 0x3312 ){ item.ItemID = 0x3E16; }
					else if ( item.ItemID == 0x3313 ){ item.ItemID = 0x3E17; }
					else if ( item.ItemID == 0x3314 ){ item.ItemID = 0x3E18; }
					else if ( item.ItemID == 0x3315 ){ item.ItemID = 0x3E19; }
					else if ( item.ItemID == 0x3316 ){ item.ItemID = 0x3E1A; }
					else if ( item.ItemID == 0x3317 ){ item.ItemID = 0x3E1B; }
					else if ( item.ItemID == 0x3318 ){ item.ItemID = 0x3E1C; }
					else if ( item.ItemID == 0x3319 ){ item.ItemID = 0x3E1D; }
					else if ( item.ItemID == 0x331A ){ item.ItemID = 0x3E1E; }
					else if ( item.ItemID == 0x331B ){ item.ItemID = 0x3E1F; }
					else if ( item.ItemID == 0x331C ){ item.ItemID = 0x3E22; }
					else if ( item.ItemID == 0x331D ){ item.ItemID = 0x3E23; }
					else if ( item.ItemID == 0x331E ){ item.ItemID = 0x3E24; }
					else if ( item.ItemID == 0x331F ){ item.ItemID = 0x3E25; }
					else if ( item.ItemID == 0x3320 ){ item.ItemID = 0x3E26; }
					else if ( item.ItemID == 0x3321 ){ item.ItemID = 0x3DEC; }
					else if ( item.ItemID == 0x3322 ){ item.ItemID = 0x3DED; }
					else if ( item.ItemID == 0x3323 ){ item.ItemID = 0x3DEE; }
					else if ( item.ItemID == 0x3324 ){ item.ItemID = 0x3DEF; }
					else if ( item.ItemID == 0x3325 ){ item.ItemID = 0x3DF0; }
					else if ( item.ItemID == 0x3326 ){ item.ItemID = 0x3DF1; }
					else if ( item.ItemID == 0x3327 ){ item.ItemID = 0x3DF2; }
					else if ( item.ItemID == 0x3328 ){ item.ItemID = 0x3DF3; }
					else if ( item.ItemID == 0x3329 ){ item.ItemID = 0x3DF4; }
					else if ( item.ItemID == 0x332A ){ item.ItemID = 0x3DF5; }
					else if ( item.ItemID == 0x332B ){ item.ItemID = 0x3DF6; }
					else if ( item.ItemID == 0x332C ){ item.ItemID = 0x3DF7; }
					else if ( item.ItemID == 0x332D ){ item.ItemID = 0x3DF8; }
					else if ( item.ItemID == 0x332E ){ item.ItemID = 0x3DF9; }
					else if ( item.ItemID == 0x332F ){ item.ItemID = 0x3DFA; }
					else if ( item.ItemID == 0x3E21 ){ item.ItemID = 0x3E20; }
					else if ( item.ItemID == 0xC2C ){ item.ItemID = 0x3E7; }
					else if ( item.ItemID == 0xEA0 ){ item.ItemID = 0x3E8; }
					else if ( item.ItemID == 0x2A61 ){ item.ItemID = 0x2A5D; }
					else if ( item.ItemID == 0xEA7 ){ item.ItemID = 0x3EA; }
					else if ( item.ItemID == 0xEA6 ){ item.ItemID = 0x3EB; }
					else if ( item.ItemID == 0xEA5 ){ item.ItemID = 0x3EC; }
					else if ( item.ItemID == 0xE55 ){ item.ItemID = 0x3ED; }
					else if ( item.ItemID == 0xE9F ){ item.ItemID = 0xDDF; }
					else if ( item.ItemID == 0xEA4 ){ item.ItemID = 0xEA3; }
					else if ( item.ItemID == 0xEA2 ){ item.ItemID = 0xEA1; }
					else if ( item.ItemID == 0x2A68 ){ item.ItemID = 0x2A65; }
					else if ( item.ItemID == 0x3E0C ){ item.ItemID = 0x3308; }
					else if ( item.ItemID == 0x3E0D ){ item.ItemID = 0x3309; }
					else if ( item.ItemID == 0x3E0E ){ item.ItemID = 0x330A; }
					else if ( item.ItemID == 0x3E0F ){ item.ItemID = 0x330B; }
					else if ( item.ItemID == 0x3E10 ){ item.ItemID = 0x330C; }
					else if ( item.ItemID == 0x3E11 ){ item.ItemID = 0x330D; }
					else if ( item.ItemID == 0x3E12 ){ item.ItemID = 0x330E; }
					else if ( item.ItemID == 0x3E13 ){ item.ItemID = 0x330F; }
					else if ( item.ItemID == 0x3E14 ){ item.ItemID = 0x3310; }
					else if ( item.ItemID == 0x3E15 ){ item.ItemID = 0x3311; }
					else if ( item.ItemID == 0x3E16 ){ item.ItemID = 0x3312; }
					else if ( item.ItemID == 0x3E17 ){ item.ItemID = 0x3313; }
					else if ( item.ItemID == 0x3E18 ){ item.ItemID = 0x3314; }
					else if ( item.ItemID == 0x3E19 ){ item.ItemID = 0x3315; }
					else if ( item.ItemID == 0x3E1A ){ item.ItemID = 0x3316; }
					else if ( item.ItemID == 0x3E1B ){ item.ItemID = 0x3317; }
					else if ( item.ItemID == 0x3E1C ){ item.ItemID = 0x3318; }
					else if ( item.ItemID == 0x3E1D ){ item.ItemID = 0x3319; }
					else if ( item.ItemID == 0x3E1E ){ item.ItemID = 0x331A; }
					else if ( item.ItemID == 0x3E1F ){ item.ItemID = 0x331B; }
					else if ( item.ItemID == 0x3E22 ){ item.ItemID = 0x331C; }
					else if ( item.ItemID == 0x3E23 ){ item.ItemID = 0x331D; }
					else if ( item.ItemID == 0x3E24 ){ item.ItemID = 0x331E; }
					else if ( item.ItemID == 0x3E25 ){ item.ItemID = 0x331F; }
					else if ( item.ItemID == 0x3E26 ){ item.ItemID = 0x3320; }
					else if ( item.ItemID == 0x3DEC ){ item.ItemID = 0x3321; }
					else if ( item.ItemID == 0x3DED ){ item.ItemID = 0x3322; }
					else if ( item.ItemID == 0x3DEE ){ item.ItemID = 0x3323; }
					else if ( item.ItemID == 0x3DEF ){ item.ItemID = 0x3324; }
					else if ( item.ItemID == 0x3DF0 ){ item.ItemID = 0x3325; }
					else if ( item.ItemID == 0x3DF1 ){ item.ItemID = 0x3326; }
					else if ( item.ItemID == 0x3DF2 ){ item.ItemID = 0x3327; }
					else if ( item.ItemID == 0x3DF3 ){ item.ItemID = 0x3328; }
					else if ( item.ItemID == 0x3DF4 ){ item.ItemID = 0x3329; }
					else if ( item.ItemID == 0x3DF5 ){ item.ItemID = 0x332A; }
					else if ( item.ItemID == 0x3DF6 ){ item.ItemID = 0x332B; }
					else if ( item.ItemID == 0x3DF7 ){ item.ItemID = 0x332C; }
					else if ( item.ItemID == 0x3DF8 ){ item.ItemID = 0x332D; }
					else if ( item.ItemID == 0x3DF9 ){ item.ItemID = 0x332E; }
					else if ( item.ItemID == 0x3DFA ){ item.ItemID = 0x332F; }
					else if ( item.ItemID == 0x499 ){ item.ItemID = 0x49A; }
					else if ( item.ItemID == 0x121C ){ item.ItemID = 0x1229; }
					else if ( item.ItemID == 0x1224 ){ item.ItemID = 0x139A; }
					else if ( item.ItemID == 0x1226 ){ item.ItemID = 0x139B; }
					else if ( item.ItemID == 0x1227 ){ item.ItemID = 0x139C; }
					else if ( item.ItemID == 0x1228 ){ item.ItemID = 0x139D; }
					else if ( item.ItemID == 0x12D8 ){ item.ItemID = 0x12D9; }
					else if ( item.ItemID == 0x1A7F ){ item.ItemID = 0x1A80; }
					else if ( item.ItemID == 0x2D0E ){ item.ItemID = 0x2D0F; }
					else if ( item.ItemID == 0x2D10 ){ item.ItemID = 0x2D11; }
					else if ( item.ItemID == 0x2D12 ){ item.ItemID = 0x2D13; }
					else if ( item.ItemID == 0x313D ){ item.ItemID = 0x3140; }
					else if ( item.ItemID == 0x313E ){ item.ItemID = 0x313F; }
					else if ( item.ItemID == 0x3141 ){ item.ItemID = 0x3142; }
					else if ( item.ItemID == 0x3143 ){ item.ItemID = 0x3144; }
					else if ( item.ItemID == 0x3149 ){ item.ItemID = 0x314A; }
					else if ( item.ItemID == 0x314B ){ item.ItemID = 0x3182; }
					else if ( item.ItemID == 0x31C1 ){ item.ItemID = 0x31C2; }
					else if ( item.ItemID == 0x31C7 ){ item.ItemID = 0x31C8; }
					else if ( item.ItemID == 0x31C9 ){ item.ItemID = 0x31CA; }
					else if ( item.ItemID == 0x31CB ){ item.ItemID = 0x31CC; }
					else if ( item.ItemID == 0x31CD ){ item.ItemID = 0x31CE; }
					else if ( item.ItemID == 0x31CF ){ item.ItemID = 0x31D0; }
					else if ( item.ItemID == 0x31D1 ){ item.ItemID = 0x31D2; }
					else if ( item.ItemID == 0x31D3 ){ item.ItemID = 0x31D4; }
					else if ( item.ItemID == 0x31F3 ){ item.ItemID = 0x3200; }
					else if ( item.ItemID == 0x31FC ){ item.ItemID = 0x31FD; }
					else if ( item.ItemID == 0x31FE ){ item.ItemID = 0x31FF; }
					else if ( item.ItemID == 0x3201 ){ item.ItemID = 0x3202; }
					else if ( item.ItemID == 0x3203 ){ item.ItemID = 0x3204; }
					else if ( item.ItemID == 0x320B ){ item.ItemID = 0x3219; }
					else if ( item.ItemID == 0x320C ){ item.ItemID = 0x3212; }
					else if ( item.ItemID == 0x321F ){ item.ItemID = 0x3225; }
					else if ( item.ItemID == 0x322B ){ item.ItemID = 0x3235; }
					else if ( item.ItemID == 0x3A98 ){ item.ItemID = 0x3A9A; }
					else if ( item.ItemID == 0x3F19 ){ item.ItemID = 0x3F1A; }
					else if ( item.ItemID == 0x3F3D ){ item.ItemID = 0x40BC; }
					else if ( item.ItemID == 0x456E ){ item.ItemID = 0x456F; }
					else if ( item.ItemID == 0x4AA6 ){ item.ItemID = 0x4AA7; }
					else if ( item.ItemID == 0x4AA8 ){ item.ItemID = 0x4AA9; }
					else if ( item.ItemID == 0x4AAA ){ item.ItemID = 0x4AAB; }
					else if ( item.ItemID == 0x4AAC ){ item.ItemID = 0x4AAD; }
					else if ( item.ItemID == 0x4AAE ){ item.ItemID = 0x4AAF; }
					else if ( item.ItemID == 0x4AB0 ){ item.ItemID = 0x4AB1; }
					else if ( item.ItemID == 0x4AB2 ){ item.ItemID = 0x4AB3; }
					else if ( item.ItemID == 0x4AB4 ){ item.ItemID = 0x4AB5; }
					else if ( item.ItemID == 0x4AB6 ){ item.ItemID = 0x4AB7; }
					else if ( item.ItemID == 0x4B44 ){ item.ItemID = 0x4B45; }
					else if ( item.ItemID == 0x4B46 ){ item.ItemID = 0x4B47; }
					else if ( item.ItemID == 0x4B48 ){ item.ItemID = 0x4B49; }
					else if ( item.ItemID == 0x4B4A ){ item.ItemID = 0x4B4B; }
					else if ( item.ItemID == 0x4B4C ){ item.ItemID = 0x4B4D; }
					else if ( item.ItemID == 0x49A ){ item.ItemID = 0x499; }
					else if ( item.ItemID == 0x1229 ){ item.ItemID = 0x121C; }
					else if ( item.ItemID == 0x139A ){ item.ItemID = 0x1224; }
					else if ( item.ItemID == 0x139B ){ item.ItemID = 0x1226; }
					else if ( item.ItemID == 0x139C ){ item.ItemID = 0x1227; }
					else if ( item.ItemID == 0x139D ){ item.ItemID = 0x1228; }
					else if ( item.ItemID == 0x12D9 ){ item.ItemID = 0x12D8; }
					else if ( item.ItemID == 0x1A80 ){ item.ItemID = 0x1A7F; }
					else if ( item.ItemID == 0x2D0F ){ item.ItemID = 0x2D0E; }
					else if ( item.ItemID == 0x2D11 ){ item.ItemID = 0x2D10; }
					else if ( item.ItemID == 0x2D13 ){ item.ItemID = 0x2D12; }
					else if ( item.ItemID == 0x3140 ){ item.ItemID = 0x313D; }
					else if ( item.ItemID == 0x313F ){ item.ItemID = 0x313E; }
					else if ( item.ItemID == 0x3142 ){ item.ItemID = 0x3141; }
					else if ( item.ItemID == 0x3144 ){ item.ItemID = 0x3143; }
					else if ( item.ItemID == 0x314A ){ item.ItemID = 0x3149; }
					else if ( item.ItemID == 0x3182 ){ item.ItemID = 0x314B; }
					else if ( item.ItemID == 0x31C2 ){ item.ItemID = 0x31C1; }
					else if ( item.ItemID == 0x31C8 ){ item.ItemID = 0x31C7; }
					else if ( item.ItemID == 0x31CA ){ item.ItemID = 0x31C9; }
					else if ( item.ItemID == 0x31CC ){ item.ItemID = 0x31CB; }
					else if ( item.ItemID == 0x31CE ){ item.ItemID = 0x31CD; }
					else if ( item.ItemID == 0x31D0 ){ item.ItemID = 0x31CF; }
					else if ( item.ItemID == 0x31D2 ){ item.ItemID = 0x31D1; }
					else if ( item.ItemID == 0x31D4 ){ item.ItemID = 0x31D3; }
					else if ( item.ItemID == 0x3200 ){ item.ItemID = 0x31F3; }
					else if ( item.ItemID == 0x31FD ){ item.ItemID = 0x31FC; }
					else if ( item.ItemID == 0x31FF ){ item.ItemID = 0x31FE; }
					else if ( item.ItemID == 0x3202 ){ item.ItemID = 0x3201; }
					else if ( item.ItemID == 0x3204 ){ item.ItemID = 0x3203; }
					else if ( item.ItemID == 0x3219 ){ item.ItemID = 0x320B; }
					else if ( item.ItemID == 0x3212 ){ item.ItemID = 0x320C; }
					else if ( item.ItemID == 0x3225 ){ item.ItemID = 0x321F; }
					else if ( item.ItemID == 0x3235 ){ item.ItemID = 0x322B; }
					else if ( item.ItemID == 0x3A9A ){ item.ItemID = 0x3A98; }
					else if ( item.ItemID == 0x3F1A ){ item.ItemID = 0x3F19; }
					else if ( item.ItemID == 0x40BC ){ item.ItemID = 0x3F3D; }
					else if ( item.ItemID == 0x456F ){ item.ItemID = 0x456E; }
					else if ( item.ItemID == 0x4AA7 ){ item.ItemID = 0x4AA6; }
					else if ( item.ItemID == 0x4AA9 ){ item.ItemID = 0x4AA8; }
					else if ( item.ItemID == 0x4AAB ){ item.ItemID = 0x4AAA; }
					else if ( item.ItemID == 0x4AAD ){ item.ItemID = 0x4AAC; }
					else if ( item.ItemID == 0x4AAF ){ item.ItemID = 0x4AAE; }
					else if ( item.ItemID == 0x4AB1 ){ item.ItemID = 0x4AB0; }
					else if ( item.ItemID == 0x4AB3 ){ item.ItemID = 0x4AB2; }
					else if ( item.ItemID == 0x4AB5 ){ item.ItemID = 0x4AB4; }
					else if ( item.ItemID == 0x4AB7 ){ item.ItemID = 0x4AB6; }
					else if ( item.ItemID == 0x4B45 ){ item.ItemID = 0x4B44; }
					else if ( item.ItemID == 0x4B47 ){ item.ItemID = 0x4B46; }
					else if ( item.ItemID == 0x4B49 ){ item.ItemID = 0x4B48; }
					else if ( item.ItemID == 0x4B4B ){ item.ItemID = 0x4B4A; }
					else if ( item.ItemID == 0x4B4D ){ item.ItemID = 0x4B4C; }
					else if ( item.ItemID == 0x3866 ){ item.ItemID = 0x3867; }
					else if ( item.ItemID == 0x3867 ){ item.ItemID = 0x3866; }
					else if ( item.ItemID == 0x3BB3 ){ item.ItemID = 0x3BB4; }
					else if ( item.ItemID == 0x46A0 ){ item.ItemID = 0x46A1; }
					else if ( item.ItemID == 0x1868 ){ item.ItemID = 0x189E; }
					else if ( item.ItemID == 0x3BB4 ){ item.ItemID = 0x3BB3; }
					else if ( item.ItemID == 0x46A1 ){ item.ItemID = 0x46A0; }
					else if ( item.ItemID == 0x189E ){ item.ItemID = 0x1868; }
					else if ( item.ItemID == 0x4FA4 ){ item.ItemID = 0x4FA5; }
					else if ( item.ItemID == 0x4FA5 ){ item.ItemID = 0x4FA4; }
					else if ( item.ItemID == 0x4FA6 ){ item.ItemID = 0x4FC1; }
					else if ( item.ItemID == 0x4FC1 ){ item.ItemID = 0x4FA6; }
					else if ( item.ItemID == 0x4FA7 ){ item.ItemID = 0x4FA8; }
					else if ( item.ItemID == 0x4FA8 ){ item.ItemID = 0x4FA7; }
					else if ( item.ItemID == 0x4FA9 ){ item.ItemID = 0x4FAA; }
					else if ( item.ItemID == 0x4FAA ){ item.ItemID = 0x4FA9; }
					else if ( item.ItemID == 0x4FAB ){ item.ItemID = 0x4FAC; }
					else if ( item.ItemID == 0x4FAC ){ item.ItemID = 0x4FAB; }
					else if ( item.ItemID == 0x4FAD ){ item.ItemID = 0x4FAE; }
					else if ( item.ItemID == 0x4FAE ){ item.ItemID = 0x4FAD; }
					else if ( item.ItemID == 0x4FAF ){ item.ItemID = 0x4FB0; }
					else if ( item.ItemID == 0x4FB0 ){ item.ItemID = 0x4FAF; }
					else if ( item.ItemID == 0x4FB1 ){ item.ItemID = 0x4FB2; }
					else if ( item.ItemID == 0x4FB2 ){ item.ItemID = 0x4FB1; }
					else if ( item.ItemID == 0x4FB3 ){ item.ItemID = 0x4FB4; }
					else if ( item.ItemID == 0x4FB4 ){ item.ItemID = 0x4FB3; }
					else if ( item.ItemID == 0x4FB5 ){ item.ItemID = 0x4FB6; }
					else if ( item.ItemID == 0x4FB6 ){ item.ItemID = 0x4FB5; }
					else if ( item.ItemID == 0x4FB7 ){ item.ItemID = 0x4FB8; }
					else if ( item.ItemID == 0x4FB8 ){ item.ItemID = 0x4FB7; }
					else if ( item.ItemID == 0x4FB9 ){ item.ItemID = 0x4FBA; }
					else if ( item.ItemID == 0x4FBA ){ item.ItemID = 0x4FB9; }
					else if ( item.ItemID == 0x4FBB ){ item.ItemID = 0x4FBC; }
					else if ( item.ItemID == 0x4FBC ){ item.ItemID = 0x4FBB; }
					else if ( item.ItemID == 0x4FBF ){ item.ItemID = 0x4FC0; }
					else if ( item.ItemID == 0x4FC0 ){ item.ItemID = 0x4FBF; }
					else if ( item.ItemID == 0x4FC2 ){ item.ItemID = 0x4FC3; }
					else if ( item.ItemID == 0x4FC3 ){ item.ItemID = 0x4FC2; }
					else if ( item.ItemID == 0x4FC4 ){ item.ItemID = 0x4FC5; }
					else if ( item.ItemID == 0x4FC5 ){ item.ItemID = 0x4FC4; }
					else if ( item.ItemID == 0x4FC6 ){ item.ItemID = 0x4FC7; }
					else if ( item.ItemID == 0x4FC7 ){ item.ItemID = 0x4FC6; }
					else if ( item.ItemID == 0x4FC9 ){ item.ItemID = 0x4FCA; }
					else if ( item.ItemID == 0x4FCA ){ item.ItemID = 0x4FCB; }
					else if ( item.ItemID == 0x4FCB ){ item.ItemID = 0x4FCC; }
					else if ( item.ItemID == 0x4FCC ){ item.ItemID = 0x4FC9; }
					else if ( item.ItemID == 0x4D0D ){ item.ItemID = 0x4D0C; }
					else if ( item.ItemID == 0x4D0C ){ item.ItemID = 0x4D0D; }
					else if ( item.ItemID == 0x4FE8 ){ item.ItemID = 0x4FE9; }
					else if ( item.ItemID == 0x4FE9 ){ item.ItemID = 0x4FE8; }
					else if ( item.ItemID == 0x6543 ){ item.ItemID = 0x6542; }
					else if ( item.ItemID == 0x6542 ){ item.ItemID = 0x6543; }
					else if ( item.ItemID == 0x6547 ){ item.ItemID = 0x6546; }
					else if ( item.ItemID == 0x6546 ){ item.ItemID = 0x6547; }
					else if ( item.ItemID == 0x654A ){ item.ItemID = 0x6549; }
					else if ( item.ItemID == 0x6549 ){ item.ItemID = 0x654A; }
					else if ( item.ItemID == 0x654C ){ item.ItemID = 0x654B; }
					else if ( item.ItemID == 0x654B ){ item.ItemID = 0x654C; }
					else if ( item.ItemID == 0x654E ){ item.ItemID = 0x654D; }
					else if ( item.ItemID == 0x654D ){ item.ItemID = 0x654E; }
					else if ( item.ItemID == 0x6540 ){ item.ItemID = 0x654F; }
					else if ( item.ItemID == 0x655F ){ item.ItemID = 0x6550; }
					else if ( item.ItemID == 0x6552 ){ item.ItemID = 0x6551; }
					else if ( item.ItemID == 0x6551 ){ item.ItemID = 0x6552; }
					else if ( item.ItemID == 0x6554 ){ item.ItemID = 0x6553; }
					else if ( item.ItemID == 0x6553 ){ item.ItemID = 0x6554; }
					else if ( item.ItemID == 0x6557 ){ item.ItemID = 0x6556; }
					else if ( item.ItemID == 0x6556 ){ item.ItemID = 0x6557; }
					else if ( item.ItemID == 0x6559 ){ item.ItemID = 0x6558; }
					else if ( item.ItemID == 0x6558 ){ item.ItemID = 0x6559; }
					else if ( item.ItemID == 0x655B ){ item.ItemID = 0x655A; }
					else if ( item.ItemID == 0x655A ){ item.ItemID = 0x655B; }
					else if ( item.ItemID == 0x655D ){ item.ItemID = 0x655C; }
					else if ( item.ItemID == 0x655C ){ item.ItemID = 0x655D; }
					else if ( item.ItemID == 0x6567 ){ item.ItemID = 0x6566; }
					else if ( item.ItemID == 0x6566 ){ item.ItemID = 0x6567; }
					else if ( item.ItemID == 0x656C ){ item.ItemID = 0x656B; }
					else if ( item.ItemID == 0x656B ){ item.ItemID = 0x656C; }
					else if ( item.ItemID == 0x658C ){ item.ItemID = 0x658B; }
					else if ( item.ItemID == 0x658B ){ item.ItemID = 0x658C; }
					else if ( item.ItemID == 0x65AE ){ item.ItemID = 0x65AD; }
					else if ( item.ItemID == 0x65AD ){ item.ItemID = 0x65AE; }
					else if ( item.ItemID == 0x65A0 ){ item.ItemID = 0x65AF; }
					else if ( item.ItemID == 0x65BF ){ item.ItemID = 0x65B0; }
					else if ( item.ItemID == 0x65B2 ){ item.ItemID = 0x65B1; }
					else if ( item.ItemID == 0x65B1 ){ item.ItemID = 0x65B2; }
					else if ( item.ItemID == 0x65B4 ){ item.ItemID = 0x65B3; }
					else if ( item.ItemID == 0x65B3 ){ item.ItemID = 0x65B4; }
					else if ( item.ItemID == 0x65B7 ){ item.ItemID = 0x65B6; }
					else if ( item.ItemID == 0x65B6 ){ item.ItemID = 0x65B7; }
					else if ( item.ItemID == 0x65C3 ){ item.ItemID = 0x65C2; }
					else if ( item.ItemID == 0x65C2 ){ item.ItemID = 0x65C3; }
					else if ( item.ItemID == 0x65C5 ){ item.ItemID = 0x65C4; }
					else if ( item.ItemID == 0x65C4 ){ item.ItemID = 0x65C5; }
					else if ( item.ItemID == 0x65C7 ){ item.ItemID = 0x65C6; }
					else if ( item.ItemID == 0x65C6 ){ item.ItemID = 0x65C7; }
					else if ( item.ItemID == 0x65C9 ){ item.ItemID = 0x65C8; }
					else if ( item.ItemID == 0x65C8 ){ item.ItemID = 0x65C9; }
					else if ( item.ItemID == 0x65CB ){ item.ItemID = 0x65CA; }
					else if ( item.ItemID == 0x65CA ){ item.ItemID = 0x65CB; }
					else if ( item.ItemID == 0x127D ){ item.ItemID = 0x12B4; }
					else if ( item.ItemID == 0x5391 ){ item.ItemID = 0x5392; }
					else if ( item.ItemID == 0x569B ){ item.ItemID = 0x569C; }
					else if ( item.ItemID == 0x569D ){ item.ItemID = 0x569E; }
					else if ( item.ItemID == 0x569F ){ item.ItemID = 0x56A0; }
					else if ( item.ItemID == 0x56A1 ){ item.ItemID = 0x56A2; }
					else if ( item.ItemID == 0x56A3 ){ item.ItemID = 0x56A4; }
					else if ( item.ItemID == 0x56A5 ){ item.ItemID = 0x56A6; }
					else if ( item.ItemID == 0x56A7 ){ item.ItemID = 0x56A8; }
					else if ( item.ItemID == 0x56A9 ){ item.ItemID = 0x56AA; }
					else if ( item.ItemID == 0x4D0C ){ item.ItemID = 0x4D0D; }
					else if ( item.ItemID == 0x5391 ){ item.ItemID = 0x5392; }
					else if ( item.ItemID == 0x1A7F ){ item.ItemID = 0x1A80; }
					else if ( item.ItemID == 0x12B4 ){ item.ItemID = 0x127D; }
					else if ( item.ItemID == 0x5392 ){ item.ItemID = 0x5391; }
					else if ( item.ItemID == 0x569C ){ item.ItemID = 0x569B; }
					else if ( item.ItemID == 0x569E ){ item.ItemID = 0x569D; }
					else if ( item.ItemID == 0x56A0 ){ item.ItemID = 0x569F; }
					else if ( item.ItemID == 0x56A2 ){ item.ItemID = 0x56A1; }
					else if ( item.ItemID == 0x56A4 ){ item.ItemID = 0x56A3; }
					else if ( item.ItemID == 0x56A6 ){ item.ItemID = 0x56A5; }
					else if ( item.ItemID == 0x56A8 ){ item.ItemID = 0x56A7; }
					else if ( item.ItemID == 0x56AA ){ item.ItemID = 0x56A9; }
					else if ( item.ItemID == 0x4D0D ){ item.ItemID = 0x4D0C; }
					else if ( item.ItemID == 0x5392 ){ item.ItemID = 0x5391; }
					else if ( item.ItemID == 0x1A80 ){ item.ItemID = 0x1A7F; }
					else if ( item.ItemID == 21187 ){ item.ItemID = 21188; }
					else if ( item.ItemID == 21189 ){ item.ItemID = 21190; }
					else if ( item.ItemID == 21191 ){ item.ItemID = 21192; }
					else if ( item.ItemID == 21206 ){ item.ItemID = 21207; }
					else if ( item.ItemID == 21208 ){ item.ItemID = 21209; }
					else if ( item.ItemID == 21256 ){ item.ItemID = 21257; }
					else if ( item.ItemID == 21258 ){ item.ItemID = 21259; }
					else if ( item.ItemID == 21260 ){ item.ItemID = 21261; }
					else if ( item.ItemID == 21262 ){ item.ItemID = 21263; }
					else if ( item.ItemID == 21264 ){ item.ItemID = 21265; }
					else if ( item.ItemID == 21266 ){ item.ItemID = 21267; }
					else if ( item.ItemID == 21268 ){ item.ItemID = 21269; }
					else if ( item.ItemID == 21270 ){ item.ItemID = 21271; }
					else if ( item.ItemID == 21272 ){ item.ItemID = 21273; }
					else if ( item.ItemID == 21274 ){ item.ItemID = 21275; }
					else if ( item.ItemID == 21276 ){ item.ItemID = 21277; }
					else if ( item.ItemID == 21291 ){ item.ItemID = 21292; }
					else if ( item.ItemID == 21293 ){ item.ItemID = 21294; }
					else if ( item.ItemID == 21295 ){ item.ItemID = 21296; }
					else if ( item.ItemID == 21297 ){ item.ItemID = 21298; }
					else if ( item.ItemID == 21299 ){ item.ItemID = 21300; }
					else if ( item.ItemID == 21301 ){ item.ItemID = 21302; }
					else if ( item.ItemID == 21303 ){ item.ItemID = 21304; }
					else if ( item.ItemID == 21305 ){ item.ItemID = 21306; }
					else if ( item.ItemID == 21307 ){ item.ItemID = 21308; }
					else if ( item.ItemID == 21309 ){ item.ItemID = 21310; }
					else if ( item.ItemID == 21311 ){ item.ItemID = 21312; }
					else if ( item.ItemID == 21313 ){ item.ItemID = 21314; }
					else if ( item.ItemID == 21315 ){ item.ItemID = 21316; }
					else if ( item.ItemID == 21317 ){ item.ItemID = 21318; }
					else if ( item.ItemID == 21319 ){ item.ItemID = 21320; }
					else if ( item.ItemID == 21321 ){ item.ItemID = 21322; }
					else if ( item.ItemID == 21323 ){ item.ItemID = 21324; }
					else if ( item.ItemID == 21325 ){ item.ItemID = 21326; }
					else if ( item.ItemID == 21333 ){ item.ItemID = 21340; }
					else if ( item.ItemID == 21334 ){ item.ItemID = 21341; }
					else if ( item.ItemID == 21347 ){ item.ItemID = 21351; }
					else if ( item.ItemID == 21348 ){ item.ItemID = 21352; }
					else if ( item.ItemID == 21355 ){ item.ItemID = 21356; }
					else if ( item.ItemID == 21367 ){ item.ItemID = 21368; }
					else if ( item.ItemID == 21369 ){ item.ItemID = 21370; }
					else if ( item.ItemID == 21371 ){ item.ItemID = 21372; }
					else if ( item.ItemID == 21373 ){ item.ItemID = 21374; }
					else if ( item.ItemID == 21375 ){ item.ItemID = 21376; }
					else if ( item.ItemID == 21377 ){ item.ItemID = 21378; }
					else if ( item.ItemID == 21379 ){ item.ItemID = 21380; }
					else if ( item.ItemID == 21381 ){ item.ItemID = 21382; }
					else if ( item.ItemID == 21383 ){ item.ItemID = 21384; }
					else if ( item.ItemID == 21385 ){ item.ItemID = 21386; }
					else if ( item.ItemID == 21387 ){ item.ItemID = 21388; }
					else if ( item.ItemID == 21389 ){ item.ItemID = 21390; }
					else if ( item.ItemID == 21391 ){ item.ItemID = 21392; }
					else if ( item.ItemID == 21393 ){ item.ItemID = 21394; }
					else if ( item.ItemID == 21395 ){ item.ItemID = 21396; }
					else if ( item.ItemID == 21357 ){ item.ItemID = 21402; }
					else if ( item.ItemID == 21358 ){ item.ItemID = 21403; }
					else if ( item.ItemID == 21362 ){ item.ItemID = 21397; }
					else if ( item.ItemID == 21363 ){ item.ItemID = 21398; }
					else if ( item.ItemID == 21188 ){ item.ItemID = 21187; }
					else if ( item.ItemID == 21190 ){ item.ItemID = 21189; }
					else if ( item.ItemID == 21192 ){ item.ItemID = 21191; }
					else if ( item.ItemID == 21207 ){ item.ItemID = 21206; }
					else if ( item.ItemID == 21209 ){ item.ItemID = 21208; }
					else if ( item.ItemID == 21257 ){ item.ItemID = 21256; }
					else if ( item.ItemID == 21259 ){ item.ItemID = 21258; }
					else if ( item.ItemID == 21261 ){ item.ItemID = 21260; }
					else if ( item.ItemID == 21263 ){ item.ItemID = 21262; }
					else if ( item.ItemID == 21265 ){ item.ItemID = 21264; }
					else if ( item.ItemID == 21267 ){ item.ItemID = 21266; }
					else if ( item.ItemID == 21269 ){ item.ItemID = 21268; }
					else if ( item.ItemID == 21271 ){ item.ItemID = 21270; }
					else if ( item.ItemID == 21273 ){ item.ItemID = 21272; }
					else if ( item.ItemID == 21275 ){ item.ItemID = 21274; }
					else if ( item.ItemID == 21277 ){ item.ItemID = 21276; }
					else if ( item.ItemID == 21292 ){ item.ItemID = 21291; }
					else if ( item.ItemID == 21294 ){ item.ItemID = 21293; }
					else if ( item.ItemID == 21296 ){ item.ItemID = 21295; }
					else if ( item.ItemID == 21298 ){ item.ItemID = 21297; }
					else if ( item.ItemID == 21300 ){ item.ItemID = 21299; }
					else if ( item.ItemID == 21302 ){ item.ItemID = 21301; }
					else if ( item.ItemID == 21304 ){ item.ItemID = 21303; }
					else if ( item.ItemID == 21306 ){ item.ItemID = 21305; }
					else if ( item.ItemID == 21308 ){ item.ItemID = 21307; }
					else if ( item.ItemID == 21310 ){ item.ItemID = 21309; }
					else if ( item.ItemID == 21312 ){ item.ItemID = 21311; }
					else if ( item.ItemID == 21314 ){ item.ItemID = 21313; }
					else if ( item.ItemID == 21316 ){ item.ItemID = 21315; }
					else if ( item.ItemID == 21318 ){ item.ItemID = 21317; }
					else if ( item.ItemID == 21320 ){ item.ItemID = 21319; }
					else if ( item.ItemID == 21322 ){ item.ItemID = 21321; }
					else if ( item.ItemID == 21324 ){ item.ItemID = 21323; }
					else if ( item.ItemID == 21326 ){ item.ItemID = 21325; }
					else if ( item.ItemID == 21340 ){ item.ItemID = 21333; }
					else if ( item.ItemID == 21341 ){ item.ItemID = 21334; }
					else if ( item.ItemID == 21351 ){ item.ItemID = 21347; }
					else if ( item.ItemID == 21352 ){ item.ItemID = 21348; }
					else if ( item.ItemID == 21356 ){ item.ItemID = 21355; }
					else if ( item.ItemID == 21368 ){ item.ItemID = 21367; }
					else if ( item.ItemID == 21370 ){ item.ItemID = 21369; }
					else if ( item.ItemID == 21372 ){ item.ItemID = 21371; }
					else if ( item.ItemID == 21374 ){ item.ItemID = 21373; }
					else if ( item.ItemID == 21376 ){ item.ItemID = 21375; }
					else if ( item.ItemID == 21378 ){ item.ItemID = 21377; }
					else if ( item.ItemID == 21380 ){ item.ItemID = 21379; }
					else if ( item.ItemID == 21382 ){ item.ItemID = 21381; }
					else if ( item.ItemID == 21384 ){ item.ItemID = 21383; }
					else if ( item.ItemID == 21386 ){ item.ItemID = 21385; }
					else if ( item.ItemID == 21388 ){ item.ItemID = 21387; }
					else if ( item.ItemID == 21390 ){ item.ItemID = 21389; }
					else if ( item.ItemID == 21392 ){ item.ItemID = 21391; }
					else if ( item.ItemID == 21394 ){ item.ItemID = 21393; }
					else if ( item.ItemID == 21396 ){ item.ItemID = 21395; }
					else if ( item.ItemID == 21402 ){ item.ItemID = 21357; }
					else if ( item.ItemID == 21403 ){ item.ItemID = 21358; }
					else if ( item.ItemID == 21397 ){ item.ItemID = 21362; }
					else if ( item.ItemID == 21398 ){ item.ItemID = 21363; }
					else if ( item.ItemID == 0x52B3 ){ item.ItemID = 0x52B4; }
					else if ( item.ItemID == 0x52B4 ){ item.ItemID = 0x52B3; }
					else if ( item.ItemID == 0x53A6 ){ item.ItemID = 0x53A7; }
					else if ( item.ItemID == 0x53A7 ){ item.ItemID = 0x53A6; }
					else if ( item.ItemID == 0x49A0 ){ item.ItemID = 0x49B4; }
					else if ( item.ItemID == 0x49A2 ){ item.ItemID = 0x49BF; }
					else if ( item.ItemID == 0x49A3 ){ item.ItemID = 0x49C0; }
					else if ( item.ItemID == 0x49A7 ){ item.ItemID = 0x49BA; }
					else if ( item.ItemID == 0x49A8 ){ item.ItemID = 0x49B5; }
					else if ( item.ItemID == 0x49B2 ){ item.ItemID = 0x49BE; }
					else if ( item.ItemID == 0x49B3 ){ item.ItemID = 0x49BB; }
					else if ( item.ItemID == 0x49A1 ){ item.ItemID = 0x49B9; }
					else if ( item.ItemID == 0x49B4 ){ item.ItemID = 0x49A0; }
					else if ( item.ItemID == 0x49BF ){ item.ItemID = 0x49A2; }
					else if ( item.ItemID == 0x49C0 ){ item.ItemID = 0x49A3; }
					else if ( item.ItemID == 0x49BA ){ item.ItemID = 0x49A7; }
					else if ( item.ItemID == 0x49B5 ){ item.ItemID = 0x49A8; }
					else if ( item.ItemID == 0x49BE ){ item.ItemID = 0x49B2; }
					else if ( item.ItemID == 0x49BB ){ item.ItemID = 0x49B3; }
					else if ( item.ItemID == 0x49B9 ){ item.ItemID = 0x49A1; }
					else if ( item.ItemID == 0xA97 ){ item.ItemID = 0xA99; }
					else if ( item.ItemID == 0xA98 ){ item.ItemID = 0xA9A; }
					else if ( item.ItemID == 0xA9B ){ item.ItemID = 0xA9C; }
					else if ( item.ItemID == 0xC14 ){ item.ItemID = 0xC15; }
					else if ( item.ItemID == 0xA9D ){ item.ItemID = 0xA9E; }
					else if ( item.ItemID == 0x19FF ){ item.ItemID = 0x1A00; }
					else if ( item.ItemID == 0x2DEF ){ item.ItemID = 0x2DF0; }
					else if ( item.ItemID == 0x3BF9 ){ item.ItemID = 0x3BFA; }
					else if ( item.ItemID == 0x3BFB ){ item.ItemID = 0x3BFC; }
					else if ( item.ItemID == 0x3BFD ){ item.ItemID = 0x3BFE; }
					else if ( item.ItemID == 0x3BFF ){ item.ItemID = 0x3C00; }
					else if ( item.ItemID == 0x3C15 ){ item.ItemID = 0x3C16; }
					else if ( item.ItemID == 0x3C17 ){ item.ItemID = 0x3C18; }
					else if ( item.ItemID == 0x3C19 ){ item.ItemID = 0x3C1A; }
					else if ( item.ItemID == 0x3C1B ){ item.ItemID = 0x3C1C; }
					else if ( item.ItemID == 0x3C1D ){ item.ItemID = 0x3C1E; }
					else if ( item.ItemID == 0x3C21 ){ item.ItemID = 0x3C22; }
					else if ( item.ItemID == 0x3C23 ){ item.ItemID = 0x3C24; }
					else if ( item.ItemID == 0x3C25 ){ item.ItemID = 0x3C26; }
					else if ( item.ItemID == 0x3C27 ){ item.ItemID = 0x3C28; }
					else if ( item.ItemID == 0x3C29 ){ item.ItemID = 0x3C2A; }
					else if ( item.ItemID == 0x3C2B ){ item.ItemID = 0x3C2C; }
					else if ( item.ItemID == 0x3C2D ){ item.ItemID = 0x3C2E; }
					else if ( item.ItemID == 0x3C2F ){ item.ItemID = 0x3C30; }
					else if ( item.ItemID == 0x3C31 ){ item.ItemID = 0x3C32; }
					else if ( item.ItemID == 0x3C33 ){ item.ItemID = 0x3C34; }
					else if ( item.ItemID == 0x3C35 ){ item.ItemID = 0x3C36; }
					else if ( item.ItemID == 0x3C37 ){ item.ItemID = 0x3C38; }
					else if ( item.ItemID == 0x3C39 ){ item.ItemID = 0x3C3A; }
					else if ( item.ItemID == 0x3C3B ){ item.ItemID = 0x3C3C; }
					else if ( item.ItemID == 0x3C3D ){ item.ItemID = 0x3C3E; }
					else if ( item.ItemID == 0x3C3F ){ item.ItemID = 0x3C40; }
					else if ( item.ItemID == 0x3C41 ){ item.ItemID = 0x3C42; }
					else if ( item.ItemID == 0x3C43 ){ item.ItemID = 0x3C44; }
					else if ( item.ItemID == 0x3C45 ){ item.ItemID = 0x3C46; }
					else if ( item.ItemID == 0x3C47 ){ item.ItemID = 0x3C48; }
					else if ( item.ItemID == 0x3C49 ){ item.ItemID = 0x3C4A; }
					else if ( item.ItemID == 0x3C4B ){ item.ItemID = 0x3C4C; }
					else if ( item.ItemID == 0x3C4D ){ item.ItemID = 0x3C4E; }
					else if ( item.ItemID == 0x3C4F ){ item.ItemID = 0x3C50; }
					else if ( item.ItemID == 0x3C51 ){ item.ItemID = 0x3C52; }
					else if ( item.ItemID == 0x3C53 ){ item.ItemID = 0x3C54; }
					else if ( item.ItemID == 0x3C55 ){ item.ItemID = 0x3C56; }
					else if ( item.ItemID == 0x3C57 ){ item.ItemID = 0x3C58; }
					else if ( item.ItemID == 0x3C59 ){ item.ItemID = 0x3C5A; }
					else if ( item.ItemID == 0x3C5B ){ item.ItemID = 0x3C5C; }
					else if ( item.ItemID == 0x3C5D ){ item.ItemID = 0x3C5E; }
					else if ( item.ItemID == 0x3C5F ){ item.ItemID = 0x3C60; }
					else if ( item.ItemID == 0x3C61 ){ item.ItemID = 0x3C62; }
					else if ( item.ItemID == 0x3C63 ){ item.ItemID = 0x3C64; }
					else if ( item.ItemID == 0x3C65 ){ item.ItemID = 0x3C66; }
					else if ( item.ItemID == 0x3C67 ){ item.ItemID = 0x3C68; }
					else if ( item.ItemID == 0x3C69 ){ item.ItemID = 0x3C6A; }
					else if ( item.ItemID == 0x3C6B ){ item.ItemID = 0x3C6C; }
					else if ( item.ItemID == 0x3C6D ){ item.ItemID = 0x3C6E; }
					else if ( item.ItemID == 0x3C6F ){ item.ItemID = 0x3C70; }
					else if ( item.ItemID == 0x3C71 ){ item.ItemID = 0x3C72; }
					else if ( item.ItemID == 0x3C73 ){ item.ItemID = 0x3C74; }
					else if ( item.ItemID == 0x3C75 ){ item.ItemID = 0x3C76; }
					else if ( item.ItemID == 0x3C77 ){ item.ItemID = 0x3C78; }
					else if ( item.ItemID == 0x3C79 ){ item.ItemID = 0x3C7A; }
					else if ( item.ItemID == 0x3C7B ){ item.ItemID = 0x3C7C; }
					else if ( item.ItemID == 0x3C7D ){ item.ItemID = 0x3C7E; }
					else if ( item.ItemID == 0x3C9B ){ item.ItemID = 0x3C9C; }
					else if ( item.ItemID == 0x3C9D ){ item.ItemID = 0x3C9E; }
					else if ( item.ItemID == 0x3C9F ){ item.ItemID = 0x3CA0; }
					else if ( item.ItemID == 0x3CA1 ){ item.ItemID = 0x3CA2; }
					else if ( item.ItemID == 0x3CA3 ){ item.ItemID = 0x3CA4; }
					else if ( item.ItemID == 0x3CA5 ){ item.ItemID = 0x3CA6; }
					else if ( item.ItemID == 0x3CA7 ){ item.ItemID = 0x3CA8; }
					else if ( item.ItemID == 0x3CAD ){ item.ItemID = 0x3CAE; }
					else if ( item.ItemID == 0x3CAF ){ item.ItemID = 0x3CB0; }
					else if ( item.ItemID == 0x3CB1 ){ item.ItemID = 0x3CB2; }
					else if ( item.ItemID == 0x3CB3 ){ item.ItemID = 0x3CB4; }
					else if ( item.ItemID == 0x3CBF ){ item.ItemID = 0x3CC0; }
					else if ( item.ItemID == 0x3CC1 ){ item.ItemID = 0x3CC2; }
					else if ( item.ItemID == 0x3CC3 ){ item.ItemID = 0x3CC4; }
					else if ( item.ItemID == 0x3CC5 ){ item.ItemID = 0x3CC6; }
					else if ( item.ItemID == 0x3CC7 ){ item.ItemID = 0x3CC8; }
					else if ( item.ItemID == 0x3CD9 ){ item.ItemID = 0x3CDA; }
					else if ( item.ItemID == 0x3CDB ){ item.ItemID = 0x3CDC; }
					else if ( item.ItemID == 0x3CDD ){ item.ItemID = 0x3CDE; }
					else if ( item.ItemID == 0x3CDF ){ item.ItemID = 0x3CE0; }
					else if ( item.ItemID == 0x3CE1 ){ item.ItemID = 0x3CE2; }
					else if ( item.ItemID == 0x3CE3 ){ item.ItemID = 0x3CE4; }
					else if ( item.ItemID == 0x3CE5 ){ item.ItemID = 0x3CE6; }
					else if ( item.ItemID == 0x3CE7 ){ item.ItemID = 0x3CE8; }
					else if ( item.ItemID == 0x3CE9 ){ item.ItemID = 0x3CEA; }
					else if ( item.ItemID == 0x3CEB ){ item.ItemID = 0x3CEC; }
					else if ( item.ItemID == 0x3CED ){ item.ItemID = 0x3CEE; }
					else if ( item.ItemID == 0x3CEF ){ item.ItemID = 0x3CF0; }
					else if ( item.ItemID == 0x3CF1 ){ item.ItemID = 0x3CF2; }
					else if ( item.ItemID == 0x3CF3 ){ item.ItemID = 0x3CF4; }
					else if ( item.ItemID == 0x3CF5 ){ item.ItemID = 0x3CF6; }
					else if ( item.ItemID == 0x3CF7 ){ item.ItemID = 0x3CF8; }
					else if ( item.ItemID == 0x3CF9 ){ item.ItemID = 0x3CFA; }
					else if ( item.ItemID == 0x3CFB ){ item.ItemID = 0x3CFC; }
					else if ( item.ItemID == 0x3CFD ){ item.ItemID = 0x3D00; }
					else if ( item.ItemID == 0x3D01 ){ item.ItemID = 0x3D02; }
					else if ( item.ItemID == 0x3D03 ){ item.ItemID = 0x3D04; }
					else if ( item.ItemID == 0x3D05 ){ item.ItemID = 0x3D06; }
					else if ( item.ItemID == 0x3D07 ){ item.ItemID = 0x3D08; }
					else if ( item.ItemID == 0x3D09 ){ item.ItemID = 0x3D0A; }
					else if ( item.ItemID == 0xA99 ){ item.ItemID = 0xA97; }
					else if ( item.ItemID == 0xA9A ){ item.ItemID = 0xA98; }
					else if ( item.ItemID == 0xA9C ){ item.ItemID = 0xA9B; }
					else if ( item.ItemID == 0xC15 ){ item.ItemID = 0xC14; }
					else if ( item.ItemID == 0xA9E ){ item.ItemID = 0xA9D; }
					else if ( item.ItemID == 0x1A00 ){ item.ItemID = 0x19FF; }
					else if ( item.ItemID == 0x2DF0 ){ item.ItemID = 0x2DEF; }
					else if ( item.ItemID == 0x3BFA ){ item.ItemID = 0x3BF9; }
					else if ( item.ItemID == 0x3BFC ){ item.ItemID = 0x3BFB; }
					else if ( item.ItemID == 0x3BFE ){ item.ItemID = 0x3BFD; }
					else if ( item.ItemID == 0x3C00 ){ item.ItemID = 0x3BFF; }
					else if ( item.ItemID == 0x3C16 ){ item.ItemID = 0x3C15; }
					else if ( item.ItemID == 0x3C18 ){ item.ItemID = 0x3C17; }
					else if ( item.ItemID == 0x3C1A ){ item.ItemID = 0x3C19; }
					else if ( item.ItemID == 0x3C1C ){ item.ItemID = 0x3C1B; }
					else if ( item.ItemID == 0x3C1E ){ item.ItemID = 0x3C1D; }
					else if ( item.ItemID == 0x3C22 ){ item.ItemID = 0x3C21; }
					else if ( item.ItemID == 0x3C24 ){ item.ItemID = 0x3C23; }
					else if ( item.ItemID == 0x3C26 ){ item.ItemID = 0x3C25; }
					else if ( item.ItemID == 0x3C28 ){ item.ItemID = 0x3C27; }
					else if ( item.ItemID == 0x3C2A ){ item.ItemID = 0x3C29; }
					else if ( item.ItemID == 0x3C2C ){ item.ItemID = 0x3C2B; }
					else if ( item.ItemID == 0x3C2E ){ item.ItemID = 0x3C2D; }
					else if ( item.ItemID == 0x3C30 ){ item.ItemID = 0x3C2F; }
					else if ( item.ItemID == 0x3C32 ){ item.ItemID = 0x3C31; }
					else if ( item.ItemID == 0x3C34 ){ item.ItemID = 0x3C33; }
					else if ( item.ItemID == 0x3C36 ){ item.ItemID = 0x3C35; }
					else if ( item.ItemID == 0x3C38 ){ item.ItemID = 0x3C37; }
					else if ( item.ItemID == 0x3C3A ){ item.ItemID = 0x3C39; }
					else if ( item.ItemID == 0x3C3C ){ item.ItemID = 0x3C3B; }
					else if ( item.ItemID == 0x3C3E ){ item.ItemID = 0x3C3D; }
					else if ( item.ItemID == 0x3C40 ){ item.ItemID = 0x3C3F; }
					else if ( item.ItemID == 0x3C42 ){ item.ItemID = 0x3C41; }
					else if ( item.ItemID == 0x3C44 ){ item.ItemID = 0x3C43; }
					else if ( item.ItemID == 0x3C46 ){ item.ItemID = 0x3C45; }
					else if ( item.ItemID == 0x3C48 ){ item.ItemID = 0x3C47; }
					else if ( item.ItemID == 0x3C4A ){ item.ItemID = 0x3C49; }
					else if ( item.ItemID == 0x3C4C ){ item.ItemID = 0x3C4B; }
					else if ( item.ItemID == 0x3C4E ){ item.ItemID = 0x3C4D; }
					else if ( item.ItemID == 0x3C50 ){ item.ItemID = 0x3C4F; }
					else if ( item.ItemID == 0x3C52 ){ item.ItemID = 0x3C51; }
					else if ( item.ItemID == 0x3C54 ){ item.ItemID = 0x3C53; }
					else if ( item.ItemID == 0x3C56 ){ item.ItemID = 0x3C55; }
					else if ( item.ItemID == 0x3C58 ){ item.ItemID = 0x3C57; }
					else if ( item.ItemID == 0x3C5A ){ item.ItemID = 0x3C59; }
					else if ( item.ItemID == 0x3C5C ){ item.ItemID = 0x3C5B; }
					else if ( item.ItemID == 0x3C5E ){ item.ItemID = 0x3C5D; }
					else if ( item.ItemID == 0x3C60 ){ item.ItemID = 0x3C5F; }
					else if ( item.ItemID == 0x3C62 ){ item.ItemID = 0x3C61; }
					else if ( item.ItemID == 0x3C64 ){ item.ItemID = 0x3C63; }
					else if ( item.ItemID == 0x3C66 ){ item.ItemID = 0x3C65; }
					else if ( item.ItemID == 0x3C68 ){ item.ItemID = 0x3C67; }
					else if ( item.ItemID == 0x3C6A ){ item.ItemID = 0x3C69; }
					else if ( item.ItemID == 0x3C6C ){ item.ItemID = 0x3C6B; }
					else if ( item.ItemID == 0x3C6E ){ item.ItemID = 0x3C6D; }
					else if ( item.ItemID == 0x3C70 ){ item.ItemID = 0x3C6F; }
					else if ( item.ItemID == 0x3C72 ){ item.ItemID = 0x3C71; }
					else if ( item.ItemID == 0x3C74 ){ item.ItemID = 0x3C73; }
					else if ( item.ItemID == 0x3C76 ){ item.ItemID = 0x3C75; }
					else if ( item.ItemID == 0x3C78 ){ item.ItemID = 0x3C77; }
					else if ( item.ItemID == 0x3C7A ){ item.ItemID = 0x3C79; }
					else if ( item.ItemID == 0x3C7C ){ item.ItemID = 0x3C7B; }
					else if ( item.ItemID == 0x3C7E ){ item.ItemID = 0x3C7D; }
					else if ( item.ItemID == 0x3C9C ){ item.ItemID = 0x3C9B; }
					else if ( item.ItemID == 0x3C9E ){ item.ItemID = 0x3C9D; }
					else if ( item.ItemID == 0x3CA0 ){ item.ItemID = 0x3C9F; }
					else if ( item.ItemID == 0x3CA2 ){ item.ItemID = 0x3CA1; }
					else if ( item.ItemID == 0x3CA4 ){ item.ItemID = 0x3CA3; }
					else if ( item.ItemID == 0x3CA6 ){ item.ItemID = 0x3CA5; }
					else if ( item.ItemID == 0x3CA8 ){ item.ItemID = 0x3CA7; }
					else if ( item.ItemID == 0x3CAE ){ item.ItemID = 0x3CAD; }
					else if ( item.ItemID == 0x3CB0 ){ item.ItemID = 0x3CAF; }
					else if ( item.ItemID == 0x3CB2 ){ item.ItemID = 0x3CB1; }
					else if ( item.ItemID == 0x3CB4 ){ item.ItemID = 0x3CB3; }
					else if ( item.ItemID == 0x3CC0 ){ item.ItemID = 0x3CBF; }
					else if ( item.ItemID == 0x3CC2 ){ item.ItemID = 0x3CC1; }
					else if ( item.ItemID == 0x3CC4 ){ item.ItemID = 0x3CC3; }
					else if ( item.ItemID == 0x3CC6 ){ item.ItemID = 0x3CC5; }
					else if ( item.ItemID == 0x3CC8 ){ item.ItemID = 0x3CC7; }
					else if ( item.ItemID == 0x3CDA ){ item.ItemID = 0x3CD9; }
					else if ( item.ItemID == 0x3CDC ){ item.ItemID = 0x3CDB; }
					else if ( item.ItemID == 0x3CDE ){ item.ItemID = 0x3CDD; }
					else if ( item.ItemID == 0x3CE0 ){ item.ItemID = 0x3CDF; }
					else if ( item.ItemID == 0x3CE2 ){ item.ItemID = 0x3CE1; }
					else if ( item.ItemID == 0x3CE4 ){ item.ItemID = 0x3CE3; }
					else if ( item.ItemID == 0x3CE6 ){ item.ItemID = 0x3CE5; }
					else if ( item.ItemID == 0x3CE8 ){ item.ItemID = 0x3CE7; }
					else if ( item.ItemID == 0x3CEA ){ item.ItemID = 0x3CE9; }
					else if ( item.ItemID == 0x3CEC ){ item.ItemID = 0x3CEB; }
					else if ( item.ItemID == 0x3CEE ){ item.ItemID = 0x3CED; }
					else if ( item.ItemID == 0x3CF0 ){ item.ItemID = 0x3CEF; }
					else if ( item.ItemID == 0x3CF2 ){ item.ItemID = 0x3CF1; }
					else if ( item.ItemID == 0x3CF4 ){ item.ItemID = 0x3CF3; }
					else if ( item.ItemID == 0x3CF6 ){ item.ItemID = 0x3CF5; }
					else if ( item.ItemID == 0x3CF8 ){ item.ItemID = 0x3CF7; }
					else if ( item.ItemID == 0x3CFA ){ item.ItemID = 0x3CF9; }
					else if ( item.ItemID == 0x3CFC ){ item.ItemID = 0x3CFB; }
					else if ( item.ItemID == 0x3D00 ){ item.ItemID = 0x3CFD; }
					else if ( item.ItemID == 0x3D02 ){ item.ItemID = 0x3D01; }
					else if ( item.ItemID == 0x3D04 ){ item.ItemID = 0x3D03; }
					else if ( item.ItemID == 0x3D06 ){ item.ItemID = 0x3D05; }
					else if ( item.ItemID == 0x3D08 ){ item.ItemID = 0x3D07; }
					else if ( item.ItemID == 0x3D0A ){ item.ItemID = 0x3D09; }
					else if ( item.ItemID == 0x569B ){ item.ItemID = 0x569C; }
					else if ( item.ItemID == 0x569C ){ item.ItemID = 0x569B; }
					else if ( item.ItemID == 0x569D ){ item.ItemID = 0x569E; }
					else if ( item.ItemID == 0x569E ){ item.ItemID = 0x569D; }
					else if ( item.ItemID == 0x569F ){ item.ItemID = 0x56A0; }
					else if ( item.ItemID == 0x56A0 ){ item.ItemID = 0x569F; }
					else if ( item.ItemID == 0x56A1 ){ item.ItemID = 0x56A2; }
					else if ( item.ItemID == 0x56A2 ){ item.ItemID = 0x56A1; }
					else if ( item.ItemID == 0x56A3 ){ item.ItemID = 0x56A4; }
					else if ( item.ItemID == 0x56A4 ){ item.ItemID = 0x56A3; }
					else if ( item.ItemID == 0x56A5 ){ item.ItemID = 0x56A6; }
					else if ( item.ItemID == 0x56A6 ){ item.ItemID = 0x56A5; }
					else if ( item.ItemID == 0x56A7 ){ item.ItemID = 0x56A8; }
					else if ( item.ItemID == 0x56A8 ){ item.ItemID = 0x56A7; }
					else if ( item.ItemID == 0x56A9 ){ item.ItemID = 0x56AA; }
					else if ( item.ItemID == 0x56AA ){ item.ItemID = 0x56A9; }
					else if ( item.ItemID == 0x570E ){ item.ItemID = 0x570D; }
					else if ( item.ItemID == 0x570D ){ item.ItemID = 0x570E; }
					else if ( item.ItemID == 0x570C ){ item.ItemID = 0x570B; }
					else if ( item.ItemID == 0x570B ){ item.ItemID = 0x570C; }
					else if ( item.ItemID == 0x575A ){ item.ItemID = 0x5754; }
					else if ( item.ItemID == 0x5754 ){ item.ItemID = 0x575A; }
					else if ( item.ItemID == 0x5755 ){ item.ItemID = 0x575B; }
					else if ( item.ItemID == 0x575B ){ item.ItemID = 0x5755; }
					else { from.SendLocalizedMessage( 1042273 ); } // You cannot turn that.
				}
			}

			private static void Deed( Item item, Mobile from )
			{
				if ( item is AlchemistTableEastDeed ) { from.AddToBackpack( new AlchemistTableSouthDeed() ); item.Delete(); }
				else if ( item is AlchemistTableSouthDeed ) { from.AddToBackpack( new AlchemistTableEastDeed() ); item.Delete(); }
				else if ( item is AnvilEastDeed ) { from.AddToBackpack( new AnvilSouthDeed() ); item.Delete(); }
				else if ( item is AnvilSouthDeed ) { from.AddToBackpack( new AnvilEastDeed() ); item.Delete(); }
				else if ( item is ArcaneBookshelfEastDeed ) { from.AddToBackpack( new ArcaneBookshelfSouthDeed() ); item.Delete(); }
				else if ( item is ArcaneBookshelfSouthDeed ) { from.AddToBackpack( new ArcaneBookshelfEastDeed() ); item.Delete(); }
				else if ( item is ArcanistStatueEastDeed ) { from.AddToBackpack( new ArcanistStatueSouthDeed() ); item.Delete(); }
				else if ( item is ArcanistStatueSouthDeed ) { from.AddToBackpack( new ArcanistStatueEastDeed() ); item.Delete(); }
				else if ( item is BrocadeGozaMatEastDeed ) { from.AddToBackpack( new BrocadeGozaMatSouthDeed() ); item.Delete(); }
				else if ( item is BrocadeGozaMatSouthDeed ) { from.AddToBackpack( new BrocadeGozaMatEastDeed() ); item.Delete(); }
				else if ( item is BrocadeSquareGozaMatEastDeed ) { from.AddToBackpack( new BrocadeSquareGozaMatSouthDeed() ); item.Delete(); }
				else if ( item is BrocadeSquareGozaMatSouthDeed ) { from.AddToBackpack( new BrocadeSquareGozaMatEastDeed() ); item.Delete(); }
				else if ( item is BrownBearRugEastDeed ) { from.AddToBackpack( new BrownBearRugSouthDeed() ); item.Delete(); }
				else if ( item is BrownBearRugSouthDeed ) { from.AddToBackpack( new BrownBearRugEastDeed() ); item.Delete(); }
				else if ( item is DarkFlowerTapestryEastDeed ) { from.AddToBackpack( new DarkFlowerTapestrySouthDeed() ); item.Delete(); }
				else if ( item is DarkFlowerTapestrySouthDeed ) { from.AddToBackpack( new DarkFlowerTapestryEastDeed() ); item.Delete(); }
				else if ( item is DartBoardEastDeed ) { from.AddToBackpack( new DartBoardSouthDeed() ); item.Delete(); }
				else if ( item is DartBoardSouthDeed ) { from.AddToBackpack( new DartBoardEastDeed() ); item.Delete(); }
				else if ( item is DeadBodyEWDeed ) { from.AddToBackpack( new DeadBodyNSDeed() ); item.Delete(); }
				else if ( item is DeadBodyNSDeed ) { from.AddToBackpack( new DeadBodyEWDeed() ); item.Delete(); }
				else if ( item is ElvenBedEastDeed ) { from.AddToBackpack( new ElvenBedSouthDeed() ); item.Delete(); }
				else if ( item is ElvenBedSouthDeed ) { from.AddToBackpack( new ElvenBedEastDeed() ); item.Delete(); }
				else if ( item is ElvenDresserEastDeed ) { from.AddToBackpack( new ElvenDresserSouthDeed() ); item.Delete(); }
				else if ( item is ElvenDresserSouthDeed ) { from.AddToBackpack( new ElvenDresserEastDeed() ); item.Delete(); }
				else if ( item is ElvenLoveseatEastDeed ) { from.AddToBackpack( new ElvenLoveseatSouthDeed() ); item.Delete(); }
				else if ( item is ElvenLoveseatSouthDeed ) { from.AddToBackpack( new ElvenLoveseatEastDeed() ); item.Delete(); }
				else if ( item is ElvenSpinningwheelEastDeed ) { from.AddToBackpack( new ElvenSpinningwheelSouthDeed() ); item.Delete(); }
				else if ( item is ElvenSpinningwheelSouthDeed ) { from.AddToBackpack( new ElvenSpinningwheelEastDeed() ); item.Delete(); }
				else if ( item is ElvenStoveEastDeed ) { from.AddToBackpack( new ElvenStoveSouthDeed() ); item.Delete(); }
				else if ( item is ElvenStoveSouthDeed ) { from.AddToBackpack( new ElvenStoveEastDeed() ); item.Delete(); }
				else if ( item is ElvenWashBasinEastDeed ) { from.AddToBackpack( new ElvenWashBasinSouthDeed() ); item.Delete(); }
				else if ( item is ElvenWashBasinSouthDeed ) { from.AddToBackpack( new ElvenWashBasinEastDeed() ); item.Delete(); }
				else if ( item is ESpikePostEastDeed ) { from.AddToBackpack( new ESpikePostSouthDeed() ); item.Delete(); }
				else if ( item is ESpikePostSouthDeed ) { from.AddToBackpack( new ESpikePostEastDeed() ); item.Delete(); }
				else if ( item is EvilFireplaceEastFaceAddonDeed ) { from.AddToBackpack( new EvilFireplaceSouthFaceAddonDeed() ); item.Delete(); }
				else if ( item is EvilFireplaceSouthFaceAddonDeed ) { from.AddToBackpack( new EvilFireplaceEastFaceAddonDeed() ); item.Delete(); }
				else if ( item is FancyElvenTableEastDeed ) { from.AddToBackpack( new FancyElvenTableSouthDeed() ); item.Delete(); }
				else if ( item is FancyElvenTableSouthDeed ) { from.AddToBackpack( new FancyElvenTableEastDeed() ); item.Delete(); }
				else if ( item is FlourMillEastDeed ) { from.AddToBackpack( new FlourMillSouthDeed() ); item.Delete(); }
				else if ( item is FlourMillSouthDeed ) { from.AddToBackpack( new FlourMillEastDeed() ); item.Delete(); }
				else if ( item is GozaMatEastDeed ) { from.AddToBackpack( new GozaMatSouthDeed() ); item.Delete(); }
				else if ( item is GozaMatSouthDeed ) { from.AddToBackpack( new GozaMatEastDeed() ); item.Delete(); }
				else if ( item is GrayBrickFireplaceEastDeed ) { from.AddToBackpack( new GrayBrickFireplaceSouthDeed() ); item.Delete(); }
				else if ( item is GrayBrickFireplaceSouthDeed ) { from.AddToBackpack( new GrayBrickFireplaceEastDeed() ); item.Delete(); }
				else if ( item is halloween_block_eastAddonDeed ) { from.AddToBackpack( new halloween_block_southAddonDeed() ); item.Delete(); }
				else if ( item is halloween_block_southAddonDeed ) { from.AddToBackpack( new halloween_block_eastAddonDeed() ); item.Delete(); }
				else if ( item is halloween_coffin_eastAddonDeed ) { from.AddToBackpack( new halloween_coffin_southAddonDeed() ); item.Delete(); }
				else if ( item is halloween_coffin_southAddonDeed ) { from.AddToBackpack( new halloween_coffin_eastAddonDeed() ); item.Delete(); }
				else if ( item is LargeBedEastDeed ) { from.AddToBackpack( new LargeBedSouthDeed() ); item.Delete(); }
				else if ( item is LargeBedSouthDeed ) { from.AddToBackpack( new LargeBedEastDeed() ); item.Delete(); }
				else if ( item is LargeForgeEastDeed ) { from.AddToBackpack( new LargeForgeSouthDeed() ); item.Delete(); }
				else if ( item is LargeForgeSouthDeed ) { from.AddToBackpack( new LargeForgeEastDeed() ); item.Delete(); }
				else if ( item is LargeStoneTableEastDeed ) { from.AddToBackpack( new LargeStoneTableSouthDeed() ); item.Delete(); }
				else if ( item is LargeStoneTableSouthDeed ) { from.AddToBackpack( new LargeStoneTableEastDeed() ); item.Delete(); }
				else if ( item is LightFlowerTapestryEastDeed ) { from.AddToBackpack( new LightFlowerTapestrySouthDeed() ); item.Delete(); }
				else if ( item is LightFlowerTapestrySouthDeed ) { from.AddToBackpack( new LightFlowerTapestryEastDeed() ); item.Delete(); }
				else if ( item is LoomEastDeed ) { from.AddToBackpack( new LoomSouthDeed() ); item.Delete(); }
				else if ( item is LoomSouthDeed ) { from.AddToBackpack( new LoomEastDeed() ); item.Delete(); }
				else if ( item is MarlinSouthAddonDeed ) { from.AddToBackpack( new MarlinEastAddonDeed() ); item.Delete(); }
				else if ( item is MarlinEastAddonDeed ) { from.AddToBackpack( new MarlinSouthAddonDeed() ); item.Delete(); }
				else if ( item is SawMillSouthAddonDeed ) { from.AddToBackpack( new SawMillEastAddonDeed() ); item.Delete(); }
				else if ( item is SawMillEastAddonDeed ) { from.AddToBackpack( new SawMillSouthAddonDeed() ); item.Delete(); }
				else if ( item is MediumStoneTableEastDeed ) { from.AddToBackpack( new MediumStoneTableSouthDeed() ); item.Delete(); }
				else if ( item is MediumStoneTableSouthDeed ) { from.AddToBackpack( new MediumStoneTableEastDeed() ); item.Delete(); }
				else if ( item is MediumStretchedHideEastDeed ) { from.AddToBackpack( new MediumStretchedHideSouthDeed() ); item.Delete(); }
				else if ( item is MediumStretchedHideSouthDeed ) { from.AddToBackpack( new MediumStretchedHideEastDeed() ); item.Delete(); }
				else if ( item is MongbatDartBoardEastDeed ) { from.AddToBackpack( new MongbatDartBoardSouthDeed() ); item.Delete(); }
				else if ( item is MongbatDartBoardSouthDeed ) { from.AddToBackpack( new MongbatDartBoardEastDeed() ); item.Delete(); }
				else if ( item is OrnateElvenTableEastDeed ) { from.AddToBackpack( new OrnateElvenTableSouthDeed() ); item.Delete(); }
				else if ( item is OrnateElvenTableSouthDeed ) { from.AddToBackpack( new OrnateElvenTableEastDeed() ); item.Delete(); }
				else if ( item is PickpocketDipEastDeed ) { from.AddToBackpack( new PickpocketDipSouthDeed() ); item.Delete(); }
				else if ( item is PickpocketDipSouthDeed ) { from.AddToBackpack( new PickpocketDipEastDeed() ); item.Delete(); }
				else if ( item is PolarBearRugEastDeed ) { from.AddToBackpack( new PolarBearRugSouthDeed() ); item.Delete(); }
				else if ( item is PolarBearRugSouthDeed ) { from.AddToBackpack( new PolarBearRugEastDeed() ); item.Delete(); }
				else if ( item is SandstoneFireplaceEastDeed ) { from.AddToBackpack( new SandstoneFireplaceSouthDeed() ); item.Delete(); }
				else if ( item is SandstoneFireplaceSouthDeed ) { from.AddToBackpack( new SandstoneFireplaceEastDeed() ); item.Delete(); }
				else if ( item is SmallBedEastDeed ) { from.AddToBackpack( new SmallBedSouthDeed() ); item.Delete(); }
				else if ( item is SmallBedSouthDeed ) { from.AddToBackpack( new SmallBedEastDeed() ); item.Delete(); }
				else if ( item is SmallStretchedHideEastDeed ) { from.AddToBackpack( new SmallStretchedHideSouthDeed() ); item.Delete(); }
				else if ( item is SmallStretchedHideSouthDeed ) { from.AddToBackpack( new SmallStretchedHideEastDeed() ); item.Delete(); }
				else if ( item is SpinningwheelEastDeed ) { from.AddToBackpack( new SpinningwheelSouthDeed() ); item.Delete(); }
				else if ( item is SpinningwheelSouthDeed ) { from.AddToBackpack( new SpinningwheelEastDeed() ); item.Delete(); }
				else if ( item is SquareGozaMatEastDeed ) { from.AddToBackpack( new SquareGozaMatSouthDeed() ); item.Delete(); }
				else if ( item is SquareGozaMatSouthDeed ) { from.AddToBackpack( new SquareGozaMatEastDeed() ); item.Delete(); }
				else if ( item is SquirrelStatueEastDeed ) { from.AddToBackpack( new SquirrelStatueSouthDeed() ); item.Delete(); }
				else if ( item is SquirrelStatueSouthDeed ) { from.AddToBackpack( new SquirrelStatueEastDeed() ); item.Delete(); }
				else if ( item is StoneFireplaceEastDeed ) { from.AddToBackpack( new StoneFireplaceSouthDeed() ); item.Delete(); }
				else if ( item is StoneFireplaceSouthDeed ) { from.AddToBackpack( new StoneFireplaceEastDeed() ); item.Delete(); }
				else if ( item is StoneOvenEastDeed ) { from.AddToBackpack( new StoneOvenSouthDeed() ); item.Delete(); }
				else if ( item is StoneOvenSouthDeed ) { from.AddToBackpack( new StoneOvenEastDeed() ); item.Delete(); }
				else if ( item is TallElvenBedEastDeed ) { from.AddToBackpack( new TallElvenBedSouthDeed() ); item.Delete(); }
				else if ( item is TallElvenBedSouthDeed ) { from.AddToBackpack( new TallElvenBedEastDeed() ); item.Delete(); }
				else if ( item is TrainingDummyEastDeed ) { from.AddToBackpack( new TrainingDummySouthDeed() ); item.Delete(); }
				else if ( item is TrainingDummySouthDeed ) { from.AddToBackpack( new TrainingDummyEastDeed() ); item.Delete(); }
				else if ( item is TrainingDaemonEastDeed ) { from.AddToBackpack( new TrainingDaemonSouthDeed() ); item.Delete(); }
				else if ( item is TrainingDaemonSouthDeed ) { from.AddToBackpack( new TrainingDaemonEastDeed() ); item.Delete(); }
				else if ( item is TigerRugEastDeed ) { from.AddToBackpack( new TigerRugSouthDeed() ); item.Delete(); }
				else if ( item is TigerRugSouthDeed ) { from.AddToBackpack( new TigerRugEastDeed() ); item.Delete(); }
				else if ( item is WhiteTigerRugEastDeed ) { from.AddToBackpack( new WhiteTigerRugSouthDeed() ); item.Delete(); }
				else if ( item is WhiteTigerRugSouthDeed ) { from.AddToBackpack( new WhiteTigerRugEastDeed() ); item.Delete(); }
				else if ( item is WarriorStatueEastDeed ) { from.AddToBackpack( new WarriorStatueSouthDeed() ); item.Delete(); }
				else if ( item is WarriorStatueSouthDeed ) { from.AddToBackpack( new WarriorStatueEastDeed() ); item.Delete(); }
				else if ( item is WaterTroughEastDeed ) { from.AddToBackpack( new WaterTroughSouthDeed() ); item.Delete(); }
				else if ( item is WaterTroughSouthDeed ) { from.AddToBackpack( new WaterTroughEastDeed() ); item.Delete(); }
				else if ( item is DolphinEastLargeAddonDeed ) { from.AddToBackpack( new DolphinSouthLargeAddonDeed() ); item.Delete(); }
				else if ( item is DolphinSouthLargeAddonDeed ) { from.AddToBackpack( new DolphinEastLargeAddonDeed() ); item.Delete(); }
				else if ( item is DolphinEastSmallAddonDeed ) { from.AddToBackpack( new DolphinSouthSmallAddonDeed() ); item.Delete(); }
				else if ( item is DolphinSouthSmallAddonDeed ) { from.AddToBackpack( new DolphinEastSmallAddonDeed() ); item.Delete(); }
				else if ( item is RoseEastLargeAddonDeed ) { from.AddToBackpack( new RoseSouthLargeAddonDeed() ); item.Delete(); }
				else if ( item is RoseSouthLargeAddonDeed ) { from.AddToBackpack( new RoseEastLargeAddonDeed() ); item.Delete(); }
				else if ( item is RoseEastSmallAddonDeed ) { from.AddToBackpack( new RoseSouthSmallAddonDeed() ); item.Delete(); }
				else if ( item is RoseSouthSmallAddonDeed ) { from.AddToBackpack( new RoseEastSmallAddonDeed() ); item.Delete(); }
				else if ( item is SkullEastLargeAddonDeed ) { from.AddToBackpack( new SkullSouthLargeAddonDeed() ); item.Delete(); }
				else if ( item is SkullSouthLargeAddonDeed ) { from.AddToBackpack( new SkullEastLargeAddonDeed() ); item.Delete(); }
				else if ( item is SkullEastSmallAddonDeed ) { from.AddToBackpack( new SkullSouthSmallAddonDeed() ); item.Delete(); }
				else if ( item is SkullSouthSmallAddonDeed ) { from.AddToBackpack( new SkullEastSmallAddonDeed() ); item.Delete(); }
				else if ( item is GothicCandelabraA ) { from.AddToBackpack( new GothicCandelabraB() ); item.Delete(); }
				else if ( item is GothicCandelabraB ) { from.AddToBackpack( new GothicCandelabraA() ); item.Delete(); }
				else if ( item is BurningScarecrowA ) { from.AddToBackpack( new BurningScarecrowB() ); item.Delete(); }
				else if ( item is BurningScarecrowB ) { from.AddToBackpack( new BurningScarecrowA() ); item.Delete(); }
				else if ( item is DaemonDartBoardSouthDeed ) { from.AddToBackpack( new DaemonDartBoardEastDeed() ); item.Delete(); }
				else if ( item is DaemonDartBoardEastDeed ) { from.AddToBackpack( new DaemonDartBoardSouthDeed() ); item.Delete(); }
				else if ( item is BaseStatueDeed ) { Server.Items.Statues.FlipStatue( (BaseStatueDeed)item, ((BaseStatueDeed)item).StatueID ); }
				else if ( item is DDRelicBearRugsAddonDeed )
				{
					DDRelicBearRugsAddonDeed rug = (DDRelicBearRugsAddonDeed)item;

					int RelCost = rug.CoinPrice;
					int RelID = rug.RelicMainID;
					int RelRugID = rug.RelicRugID;
					int RelHue = rug.RelicColor;
					string RelQuality = rug.RelicQuality;
					int RelRolled = rug.RelicFound;

					if ( RelRugID == 1){ RelRugID = 2; RelID = 0x1545; }
					else if ( RelRugID == 2){ RelRugID = 1; RelID = 0x1546; }
					else if ( RelRugID == 3){ RelRugID = 4; RelID = 0x1545; }
					else if ( RelRugID == 4){ RelRugID = 3; RelID = 0x1546; }
					else if ( RelRugID == 5){ RelRugID = 6; RelID = 0x1545; }
					else { RelRugID = 5; RelID = 0x1546; }

					from.AddToBackpack( new DDRelicBearRugsAddonDeed( RelCost, RelID, RelRugID, RelHue, RelQuality, RelRolled ) );
					item.Delete();
				}
				else if ( item is MyTentEastAddonDeed )
				{
					MyTentEastAddonDeed tent = (MyTentEastAddonDeed)item;
					int TentHue = tent.TentColor;
					from.AddToBackpack( new MyTentSouthAddonDeed( TentHue, 1 ) );
					item.Delete();
				}
				else if ( item is MyTentSouthAddonDeed )
				{
					MyTentSouthAddonDeed tent = (MyTentSouthAddonDeed)item;
					int TentHue = tent.TentColor;
					from.AddToBackpack( new MyTentEastAddonDeed( TentHue, 1 ) );
					item.Delete();
				}
				else if ( item is MyCircusTentEastAddonDeed )
				{
					MyCircusTentEastAddonDeed tent = (MyCircusTentEastAddonDeed)item;
					int TentHue1 = tent.TentColor1;
					int TentHue2 = tent.TentColor2;
					from.AddToBackpack( new MyCircusTentSouthAddonDeed( TentHue1, TentHue2, 1 ) );
					item.Delete();
				}
				else if ( item is MyCircusTentSouthAddonDeed )
				{
					MyCircusTentSouthAddonDeed tent = (MyCircusTentSouthAddonDeed)item;
					int TentHue1 = tent.TentColor1;
					int TentHue2 = tent.TentColor2;
					from.AddToBackpack( new MyCircusTentEastAddonDeed( TentHue1, TentHue2, 1 ) );
					item.Delete();
				}
				else if ( item is BaseAddonDeed ) { from.SendMessage( "That deed cannot be flipped to face another direction!" ); }
				else { from.SendMessage( "This only flips deeds that are on the floor in your home!" ); }
			}

			private static void Up( Item item, Mobile from )
			{
				int floorZ = GetFloorZ( item );

                if ( isLawn( item, from ) && item.Z < ((from.Map).GetAverageZ(item.X, item.Y)+15) )
                {
                    item.Location = new Point3D(item.Location, item.Z + 1);
                }
                else if ( isLawn( item, from ) )
                {
                    from.SendLocalizedMessage(1042274); // You cannot raise it up any higher.
                }
				else if( item is BaseDoor )
					from.SendMessage("You cannot move doors around with this.");
				else if ( floorZ > int.MinValue && item.Z < (floorZ + 15) ) // Confirmed : no height checks here
					item.Location = new Point3D( item.Location, item.Z + 1 );
				else
					from.SendLocalizedMessage( 1042274 ); // You cannot raise it up any higher.
			}

			private static void Down( Item item, Mobile from )
			{
				int floorZ = GetFloorZ( item );

                if ( isLawn( item, from ) && item.Z > (from.Map).GetAverageZ(item.X, item.Y) )
                {
                    item.Location = new Point3D(item.Location, item.Z - 1);
                }
                else if ( isLawn( item, from ) )
                {
                    from.SendLocalizedMessage(1042275); // You cannot lower it down any further.
                }
				else if( item is BaseDoor )
					from.SendMessage("You cannot move doors around with this.");
				else if ( floorZ > int.MinValue && item.Z > GetFloorZ( item ) )
					item.Location = new Point3D( item.Location, item.Z - 1 );
				else
					from.SendLocalizedMessage( 1042275 ); // You cannot lower it down any further.
			}

			private static void North( Item item, Mobile from, BaseHouse house )
			{
				Point3D m_PointDest = new Point3D( item.X, item.Y - 1, item.Z );

				if ( isLawn( item, from ) )
                {
					if ( !ValidRegion( m_PointDest, from.Map, from ) )
					{
						from.SendMessage("You cannot move that over there!");
					}
					else if ( m_PointDest.Y > from.Location.Y + Remodeling.Front || m_PointDest.Y < from.Location.Y - Remodeling.Back )
					{
						from.SendMessage("You may have to get closer to move that within your land!");
					}
					else if ( m_PointDest.X > from.Location.X + Remodeling.Right || m_PointDest.X < from.Location.X - Remodeling.Left )
					{
						from.SendMessage("You may have to get closer to move that within your land!");
					}
					else
					{
						item.Y = (item.Y - 1);
					}
                }
				else if ( isShanty( item, from ) )
                {
					if ( !ShantyRegion( m_PointDest, from.Map ) )
					{
						from.SendMessage("You cannot move that over there!");
					}
					else
					{
						item.Y = (item.Y - 1);
					}
                }
				else if( item is BaseDoor )
					from.SendMessage("You cannot move doors around with this.");
				else if ( !SpellHelper.CheckMulti( m_PointDest, from.Map ) )
					from.SendMessage("You cannot move it north any further.");
				else if (house.IsInside(new Point3D(item.X, item.Y - 1, item.Z), item.ItemData.Height))
					item.Y = (item.Y - 1);
				else
					from.SendMessage("You cannot move it north any further.");
			}

			private static void East( Item item, Mobile from, BaseHouse house  )
			{
				Point3D m_PointDest = new Point3D( item.X + 1, item.Y, item.Z );

				if ( isLawn( item, from ) )
                {
					if ( !ValidRegion( m_PointDest, from.Map, from ) )
					{
						from.SendMessage("You cannot move that over there!");
					}
					else if ( m_PointDest.Y > from.Location.Y + Remodeling.Front || m_PointDest.Y < from.Location.Y - Remodeling.Back )
					{
						from.SendMessage("You may have to get closer to move that within your land!");
					}
					else if ( m_PointDest.X > from.Location.X + Remodeling.Right || m_PointDest.X < from.Location.X - Remodeling.Left )
					{
						from.SendMessage("You may have to get closer to move that within your land!");
					}
					else
					{
						item.X = (item.X + 1);
					}
                }
				else if ( isShanty( item, from ) )
                {
					if ( !ShantyRegion( m_PointDest, from.Map ) )
					{
						from.SendMessage("You cannot move that over there!");
					}
					else
					{
						item.X = (item.X + 1);
					}
                }
				else if( item is BaseDoor )
					from.SendMessage("You cannot move doors around with this.");
				else if ( !SpellHelper.CheckMulti( m_PointDest, from.Map ) )
					from.SendMessage("You cannot move it east any further.");
				else if (house.IsInside(new Point3D(item.X + 1, item.Y, item.Z), item.ItemData.Height))
					item.X = (item.X + 1);
				else
					from.SendMessage("You cannot move it east any further.");
			}

			private static void South(Item item, Mobile from, BaseHouse house)
			{
				Point3D m_PointDest = new Point3D( item.X, item.Y + 1, item.Z );

				if ( isLawn( item, from ) )
                {
					if ( !ValidRegion( m_PointDest, from.Map, from ) )
					{
						from.SendMessage("You cannot move that over there!");
					}
					else if ( m_PointDest.Y > from.Location.Y + Remodeling.Front || m_PointDest.Y < from.Location.Y - Remodeling.Back )
					{
						from.SendMessage("You may have to get closer to move that within your land!");
					}
					else if ( m_PointDest.X > from.Location.X + Remodeling.Right || m_PointDest.X < from.Location.X - Remodeling.Left )
					{
						from.SendMessage("You may have to get closer to move that within your land!");
					}
					else
					{
						item.Y = (item.Y + 1);
					}
                }
				else if ( isShanty( item, from ) )
                {
					if ( !ShantyRegion( m_PointDest, from.Map ) )
					{
						from.SendMessage("You cannot move that over there!");
					}
					else
					{
						item.Y = (item.Y + 1);
					}
                }
				else if( item is BaseDoor )
					from.SendMessage("You cannot move doors around with this.");
				else if ( !SpellHelper.CheckMulti( m_PointDest, from.Map ) )
					from.SendMessage("You cannot move it south any further.");
				else if (house.IsInside(new Point3D(item.X, item.Y + 1, item.Z), item.ItemData.Height))
					item.Y = (item.Y + 1);
				else
					from.SendMessage("You cannot move it south any further.");
			}

			private static void West(Item item, Mobile from, BaseHouse house)
			{
				Point3D m_PointDest = new Point3D( item.X - 1, item.Y, item.Z );

				if ( isLawn( item, from ) )
                {
					if ( !ValidRegion( m_PointDest, from.Map, from ) )
					{
						from.SendMessage("You cannot move that over there!");
					}
					else if ( m_PointDest.Y > from.Location.Y + Remodeling.Front || m_PointDest.Y < from.Location.Y - Remodeling.Back )
					{
						from.SendMessage("You may have to get closer to move that within your land!");
					}
					else if ( m_PointDest.X > from.Location.X + Remodeling.Right || m_PointDest.X < from.Location.X - Remodeling.Left )
					{
						from.SendMessage("You may have to get closer to move that within your land!");
					}
					else
					{
						item.X = (item.X - 1);
					}
                }
				else if ( isShanty( item, from ) )
                {
					if ( !ShantyRegion( m_PointDest, from.Map ) )
					{
						from.SendMessage("You cannot move that over there!");
					}
					else
					{
						item.X = (item.X - 1);
					}
                }
				else if( item is BaseDoor )
					from.SendMessage("You cannot move doors around with this.");
				else if ( !SpellHelper.CheckMulti( m_PointDest, from.Map ) )
					from.SendMessage("You cannot move it west any further.");
				else if (house.IsInside(new Point3D(item.X - 1, item.Y, item.Z), item.ItemData.Height))
					item.X = (item.X - 1);
				else
					from.SendMessage("You cannot move it west any further.");
			}

			private static bool isLawn( Item item, Mobile from )
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );

				if ( item is LawnStair && ((LawnStair)item).House == house )
					return true;

				if ( item is LawnItem && ((LawnItem)item).House == house )
					return true;

				if ( item is LawnGate && ((LawnGate)item).House == house )
					return true;

                if ( item is LawnPiece && ((LawnPiece)item).ParentLawnItem != null && ((LawnItem)(((LawnPiece)item).ParentLawnItem)).House == house )
					return true;

				return false;
			}

			private static bool isShanty( Item item, Mobile from )
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );

				if ( item is ShantyStair && ((ShantyStair)item).House == house )
					return true;

				if ( item is ShantyItem && ((ShantyItem)item).House == house )
					return true;

				if ( item is ShantyDoor && ((ShantyDoor)item).House == house )
					return true;

                if ( item is ShantyPiece && ((ShantyPiece)item).ParentShantyItem != null && ((ShantyItem)(((ShantyPiece)item).ParentShantyItem)).House == house )
					return true;

				return false;
			}

			private static int GetFloorZ( Item item )
			{
				Map map = item.Map;

				if ( map == null )
					return int.MinValue;

				StaticTile[] tiles = map.Tiles.GetStaticTiles( item.X, item.Y, true );

				int z = int.MinValue;

				for ( int i = 0; i < tiles.Length; ++i )
				{
					StaticTile tile = tiles[i];
					ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

					int top = tile.Z; // Confirmed : no height checks here

					if ( id.Surface && !id.Impassable && top > z && top <= item.Z )
						z = top;
				}

				return z;
			}

			private static bool ValidRegion( Point3D loc, Map map, Mobile from )
			{
				int x = loc.X;
				int y = loc.Y;
				int z = loc.Z;

				Point3D chk_n = new Point3D( x, y-1, z );
				Point3D chk_s = new Point3D( x, y+1, z );
				Point3D chk_e = new Point3D( x+1, y, z );
				Point3D chk_w = new Point3D( x-1, y, z );

				bool test_n = false;
				bool test_s = false;
				bool test_e = false;
				bool test_w = false;

				Region reg_n = Region.Find( chk_n, map );
				Region reg_s = Region.Find( chk_s, map );
				Region reg_e = Region.Find( chk_e, map );
				Region reg_w = Region.Find( chk_w, map );

				if ( reg_n is HouseRegion )
					test_n = false;
				else if ( reg_n.IsDefault || reg_n.Name == null || reg_n.Name == "" )
					test_n = true;
				else if ( reg_n.Name != null && reg_n.Name != "" && reg_n.AllowHousing( from, loc ) )
					test_n = true;

				if ( reg_s is HouseRegion )
					test_s = false;
				else if ( reg_s.IsDefault || reg_s.Name == null || reg_s.Name == "" )
					test_s = true;
				else if ( reg_s.Name != null && reg_s.Name != "" && reg_s.AllowHousing( from, loc ) )
					test_s = true;

				if ( reg_e is HouseRegion )
					test_e = false;
				else if ( reg_e.IsDefault || reg_e.Name == null || reg_e.Name == "" )
					test_e = true;
				else if ( reg_e.Name != null && reg_e.Name != "" && reg_e.AllowHousing( from, loc ) )
					test_e = true;

				if ( reg_w is HouseRegion )
					test_w = false;
				else if ( reg_w.IsDefault || reg_w.Name == null || reg_w.Name == "" )
					test_w = true;
				else if ( reg_w.Name != null && reg_w.Name != "" && reg_w.AllowHousing( from, loc ) )
					test_w = true;

				if ( test_n || test_s || test_e || test_w )
					return true;

				if ( !ValidatePlacement( loc, from ) )
					return false;

				return false;
			}

			private static bool ShantyRegion( Point3D loc, Map map )
			{
				Region reg = Region.Find( loc, map );

				if ( reg is HouseRegion )
					return true;

				return false;
			}

			private static bool ValidatePlacement( Point3D loc, Mobile from )
			{
				if ( from.Map == Map.Underworld && from.X > 1625 && from.Y > 0 )
					return false;

				else if ( loc.Y > from.Location.Y + Remodeling.Front || loc.Y < from.Location.Y - Remodeling.Back )
					return false;

				else if ( loc.X > from.Location.X + Remodeling.Right || loc.X < from.Location.X - Remodeling.Left )
					return false;

				return true;
			}
		}
	}
}