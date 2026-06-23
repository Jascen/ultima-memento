using Server.Misc;
using Server.Network;
using Server.Gumps;
using Server.Mobiles;
using Server.Utilities;

namespace Server.Items
{
	public class DrakkhenEggRed : Item
	{
		[Constructable]
		public DrakkhenEggRed() : base( 0x1444 )
		{
			Weight = 4.0;
			Name = "Drakkhen Crystal";
			Light = LightType.Circle225;
			Hue = 0xB01;
			HaveGold = 0;
		}

		public override void OnDoubleClick( Mobile from )
		{
			from.SendSound( 0x5AA );
			from.CloseGump( typeof( DrakkhenEggGump ) );
			from.SendGump( new DrakkhenEggGump( from, this ) );
		}

		public DrakkhenEggRed( Serial serial ) : base( serial )
		{
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{          		
			string sEnd = ".";

			if ( from != null )
			{
				int WhatIsNeeded = 50000 - HaveGold;
				if ( dropped is Gold && WhatIsNeeded > 0 )
				{
					from.SendSound( 0x5AA );
					var iAmount = ItemUtilities.ConsumeClamped(dropped, WhatIsNeeded);

					if ( iAmount > 1 ){ sEnd = "s."; }

					HaveGold += iAmount;
					from.SendMessage( "You added " + iAmount.ToString() + " gold coin" + sEnd );

					return dropped.Deleted;
				}
			}

			return false;
		}

		public static bool ProcessDrakkhenEgg( Mobile m, Mobile druid, Item dropped )
		{
			DrakkhenEggRed egg = (DrakkhenEggRed)dropped;

			if ( egg.HaveGold < 50000 )
			{
				druid.Say( "You do not have enough gold for me to perform this service." );
				return false;
			}
			else if ( (m.Followers + 2) > m.FollowersMax )
			{
				druid.Say( "You have too many followers with you to crack this crystal." );
				return false;
			}

			BaseCreature drakkhen = new DrakkhenRed();
			drakkhen.OnAfterSpawn();
			drakkhen.Controlled = true;
			drakkhen.ControlMaster = m;
			drakkhen.IsBonded = true;
			drakkhen.MoveToWorld( m.Location, m.Map );
			drakkhen.ControlTarget = m;
			drakkhen.Tamable = true;
			drakkhen.ControlOrder = OrderType.Follow;

			LoggingFunctions.LogGenericQuest( m, "has cracked open a drakkhen crystal" );
			m.PrivateOverheadMessage(MessageType.Regular, 1153, false, "Your drakkhen is freed.", m.NetState);

			m.PlaySound( 0x041 );

			dropped.Delete();

			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 ); // version
			writer.Write( HaveGold );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			HaveGold = reader.ReadInt();
		}

		public class DrakkhenEggGump : Gump
		{
			public DrakkhenEggGump( Mobile from, DrakkhenEggRed egg ): base( 50, 50 )
			{
				string color = "#f73d3c";

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				int need = 50000 - egg.HaveGold;
				string cost = " Place " + need + " more gold onto the crystal and give it to a druid.";
				if ( egg.HaveGold >= 50000 ){ cost = " You can now give this to a druid as you have enough gold."; }

				AddPage(0);

				AddImage(0, 0, 7016, Server.Misc.PlayerSettings.GetGumpHue( from ));
				AddHtml( 12, 11, 420, 20, @"<BODY><BASEFONT Color=" + color + ">DRAKKHEN CRYSTAL</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(546, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);
				AddHtml( 12, 42, 561, 297, @"<BODY><BASEFONT Color=" + color + "><BR><BR><BR>You have heard tales of these gems. These rare crystals come from the mighty dragon-kin beasts in which this was found. Within it lies the infant version of the creature, but only the local druids know how to safely release it from this encased gem. If you could find such a druid, and you want to release the creature, then be ready to give 50000 gold in tribute as the druid will not do such a thing out of the kindness of their heart. When the drakkhen is released, it will be very young and only half as powerful as a drake. You can ride them if you wish but it takes centuries for them to grow as mighty as the one this was taken from, so they will never be as strong. They are rare beasts nonetheless." + cost + "</BASEFONT></BODY>", (bool)false, (bool)false);
			}

			public override void OnResponse(NetState state, RelayInfo info)
			{
				Mobile from = state.Mobile;
				from.SendSound( 0x5AA ); 
			}
		}

		public int HaveGold;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveGold { get{ return HaveGold; } set{ HaveGold = value; } }
	}
	public class DrakkhenEggBlack : Item
	{
		[Constructable]
		public DrakkhenEggBlack() : base( 0x1444 )
		{
			Weight = 4.0;
			Name = "Drakkhen Crystal";
			Light = LightType.Circle225;
			Hue = 0x99E;
			HaveGold = 0;
		}

		public override void OnDoubleClick( Mobile from )
		{
			from.SendSound( 0x5AA );
			from.CloseGump( typeof( DrakkhenEggGump ) );
			from.SendGump( new DrakkhenEggGump( from, this ) );
		}

		public DrakkhenEggBlack( Serial serial ) : base( serial )
		{
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{          		
			string sEnd = ".";

			if ( from != null )
			{
				int WhatIsNeeded = 50000 - HaveGold;
				if ( dropped is Gold && WhatIsNeeded > 0 )
				{
					from.SendSound( 0x5AA );
					var iAmount = ItemUtilities.ConsumeClamped(dropped, WhatIsNeeded);

					if ( iAmount > 1 ){ sEnd = "s."; }

					HaveGold += iAmount;
					from.SendMessage( "You added " + iAmount.ToString() + " gold coin" + sEnd );

					return dropped.Deleted;
				}
			}

			return false;
		}

		public static bool ProcessDrakkhenEgg( Mobile m, Mobile druid, Item dropped )
		{
			DrakkhenEggBlack egg = (DrakkhenEggBlack)dropped;

			if ( egg.HaveGold < 50000 )
			{
				druid.Say( "You do not have enough gold for me to perform this service." );
				return false;
			}
			else if ( (m.Followers + 2) > m.FollowersMax )
			{
				druid.Say( "You have too many followers with you to crack this crystal." );
				return false;
			}

			BaseCreature drakkhen = new DrakkhenBlack();
			drakkhen.OnAfterSpawn();
			drakkhen.Controlled = true;
			drakkhen.ControlMaster = m;
			drakkhen.IsBonded = true;
			drakkhen.MoveToWorld( m.Location, m.Map );
			drakkhen.ControlTarget = m;
			drakkhen.Tamable = true;
			drakkhen.ControlOrder = OrderType.Follow;

			LoggingFunctions.LogGenericQuest( m, "has cracked open a drakkhen crystal" );
			m.PrivateOverheadMessage(MessageType.Regular, 1153, false, "Your drakkhen is freed.", m.NetState);

			m.PlaySound( 0x041 );

			dropped.Delete();

			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 ); // version
			writer.Write( HaveGold );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			HaveGold = reader.ReadInt();
		}

		public class DrakkhenEggGump : Gump
		{
			public DrakkhenEggGump( Mobile from, DrakkhenEggBlack egg ): base( 50, 50 )
			{
				string color = "#f73d3c";

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				int need = 50000 - egg.HaveGold;
				string cost = " Place " + need + " more gold onto the crystal and give it to a druid.";
				if ( egg.HaveGold >= 50000 ){ cost = " You can now give this to a druid as you have enough gold."; }

				AddPage(0);

				AddImage(0, 0, 7016, Server.Misc.PlayerSettings.GetGumpHue( from ));
				AddHtml( 12, 11, 420, 20, @"<BODY><BASEFONT Color=" + color + ">DRAKKHEN CRYSTAL</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(546, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);
				AddHtml( 12, 42, 561, 297, @"<BODY><BASEFONT Color=" + color + "><BR><BR><BR>You have heard tales of these gems. These rare crystals come from the mighty dragon-kin beasts in which this was found. Within it lies the infant version of the creature, but only the local druids know how to safely release it from this encased gem. If you could find such a druid, and you want to release the creature, then be ready to give 50000 gold in tribute as the druid will not do such a thing out of the kindness of their heart. When the drakkhen is released, it will be very young and only half as powerful as a drake. You can ride them if you wish but it takes centuries for them to grow as mighty as the one this was taken from, so they will never be as strong. They are rare beasts nonetheless." + cost + "</BASEFONT></BODY>", (bool)false, (bool)false);
			}

			public override void OnResponse(NetState state, RelayInfo info)
			{
				Mobile from = state.Mobile;
				from.SendSound( 0x5AA ); 
			}
		}

		public int HaveGold;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveGold { get{ return HaveGold; } set{ HaveGold = value; } }
	}
}