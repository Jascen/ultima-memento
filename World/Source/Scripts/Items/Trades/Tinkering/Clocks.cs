using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public enum MoonPhase
	{
		NewMoon,
		WaxingCrescentMoon,
		FirstQuarter,
		WaxingGibbous,
		FullMoon,
		WaningGibbous,
		LastQuarter,
		WaningCrescent
	}

	[Flipable( 0x104B, 0x104C )]
	public class Clock : Item
	{
		private static DateTime m_ServerStart;

		public static DateTime ServerStart
		{
			get{ return m_ServerStart; }
		}

		public static void Initialize()
		{
			m_ServerStart = DateTime.Now;
		}

		[Constructable]
		public Clock() : this( 0x104B )
		{
		}

		[Constructable]
		public Clock( int itemID ) : base( itemID )
		{
			Weight = 3.0;
		}

		public Clock( Serial serial ) : base( serial )
		{
		}

		public const double SecondsPerUOMinute = 5.0;
		public const double MinutesPerUODay = SecondsPerUOMinute * 24;

		private static DateTime WorldStart = new DateTime( 1997, 9, 1 );

		public static MoonPhase GetMoonPhase( Map map, int x, int y )
		{
			x = 100; y = 100; map = Map.Sosaria;
			int hours, minutes, totalMinutes;

			GetTime( map, x, y, out hours, out minutes, out totalMinutes );

			if ( map != null )
				totalMinutes /= 10 + (map.MapIndex * 20);

			return (MoonPhase)(totalMinutes % 8);
		}

		public static void GetTime( Map map, int x, int y, out int hours, out int minutes )
		{
			x = 100; y = 100; map = Map.Sosaria;
			int totalMinutes;

			GetTime( map, x, y, out hours, out minutes, out totalMinutes );
		}

		public static void GetTime( Map map, int x, int y, out int hours, out int minutes, out int totalMinutes )
		{
			x = 100; y = 100; map = Map.Sosaria;
			TimeSpan timeSpan = DateTime.Now - WorldStart;

			totalMinutes = (int)(timeSpan.TotalSeconds / SecondsPerUOMinute);

			if ( map != null )
				totalMinutes += map.MapIndex * 320;

			// Really on OSI this must be by subserver
			totalMinutes += x / 16;

			hours = (totalMinutes / 60) % 24;
			minutes = totalMinutes % 60;
		}

		public static void GetTime( out int generalNumber, out string exactTime )
		{
			GetTime( null, 0, 0, out generalNumber, out exactTime );
		}

		public static void GetTime( Mobile from, out int generalNumber, out string exactTime )
		{
			//GetTime( from.Map, from.X, from.Y, out generalNumber, out exactTime );
			GetTime( Map.Sosaria, 100, 100, out generalNumber, out exactTime );
		}

		public static void GetTime( Map map, int x, int y, out int generalNumber, out string exactTime )
		{
			x = 100; y = 100; map = Map.Sosaria;
			int hours, minutes;

			GetTime( map, x, y, out hours, out minutes );

			// 00:00 AM - 00:59 AM : Witching hour
			// 01:00 AM - 03:59 AM : Middle of night
			// 04:00 AM - 07:59 AM : Early morning
			// 08:00 AM - 11:59 AM : Late morning
			// 12:00 PM - 12:59 PM : Noon
			// 01:00 PM - 03:59 PM : Afternoon
			// 04:00 PM - 07:59 PM : Early evening
			// 08:00 PM - 11:59 AM : Late at night

			if ( hours >= 20 )
				generalNumber = 1042957; // It's late at night
			else if ( hours >= 16 )
				generalNumber = 1042956; // It's early in the evening
			else if ( hours >= 13 )
				generalNumber = 1042955; // It's the afternoon
			else if ( hours >= 12 )
				generalNumber = 1042954; // It's around noon
			else if ( hours >= 08 )
				generalNumber = 1042953; // It's late in the morning
			else if ( hours >= 04 )
				generalNumber = 1042952; // It's early in the morning
			else if ( hours >= 01 )
				generalNumber = 1042951; // It's the middle of the night
			else
				generalNumber = 1042950; // 'Tis the witching hour. 12 Midnight.

			hours %= 12;

			if ( hours == 0 )
				hours = 12;

			exactTime = String.Format( "{0}:{1:D2}", hours, minutes );
		}

		public override void OnDoubleClick( Mobile from )
		{
			int genericNumber;
			string exactTime;

			GetTime( from, out genericNumber, out exactTime );

			SendLocalizedMessageTo( from, genericNumber );
			SendLocalizedMessageTo( from, 1042958, exactTime ); // ~1_TIME~ to be exact
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

			if ( Weight == 2.0 )
				Weight = 3.0;
		}
	}

	[Flipable( 0x104B, 0x104C )]
	public class ClockRight : Clock
	{
		[Constructable]
		public ClockRight() : base( 0x104B )
		{
		}

		public ClockRight( Serial serial ) : base( serial )
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

	[Flipable( 0x104B, 0x104C )]
	public class ClockLeft : Clock
	{
		[Constructable]
		public ClockLeft() : base( 0x104C )
		{
		}

		public ClockLeft( Serial serial ) : base( serial )
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

	[Flipable( 0x44D5, 0x44D9 )]
	public class DDRelicClock1 : Clock, IRelic
	{
		public override void ItemIdentified( bool id )
		{
			m_NotIdentified = id;
			if ( !id )
			{
				ColorHue3 = "FDC844";
				ColorText3 = "Worth " + CoinPrice + " Gold";
			}
		}

		[Constructable]
		public DDRelicClock1() : base( 0x44D5 )
		{
			Weight = 100;

			CoinPrice = Utility.RandomMinMax( 80, 500 );
			NotIdentified = true;
			NotIDSource = Identity.Merchant;
			NotIDSkill = IDSkill.Mercantile;

			string sLook = "a rare";
			switch ( Utility.RandomMinMax( 0, 18 ) )
			{
				case 0:	sLook = "a rare";	break;
				case 1:	sLook = "a nice";	break;
				case 2:	sLook = "a pretty";	break;
				case 3:	sLook = "a superb";	break;
				case 4:	sLook = "a delightful";	break;
				case 5:	sLook = "an elegant";	break;
				case 6:	sLook = "an exquisite";	break;
				case 7:	sLook = "a fine";	break;
				case 8:	sLook = "a gorgeous";	break;
				case 9:	sLook = "a lovely";	break;
				case 10:sLook = "a magnificent";	break;
				case 11:sLook = "a marvelous";	break;
				case 12:sLook = "a splendid";	break;
				case 13:sLook = "a wonderful";	break;
				case 14:sLook = "an extraordinary";	break;
				case 15:sLook = "a strange";	break;
				case 16:sLook = "an odd";	break;
				case 17:sLook = "a unique";	break;
				case 18:sLook = "an unusual";	break;
			}
			Name = sLook + " grandfather clock";
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) && MySettings.S_IdentifyItemsOnlyInPack && from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified ) 
				from.SendMessage( "This must be in your backpack to identify." );
			else if ( from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified )
				IDCommand( from );
			else
				base.OnDoubleClick( from );
		}

		public override void IDCommand( Mobile m )
		{
			if ( this.NotIDSkill == IDSkill.Tasting )
				RelicFunctions.IDItem( m, m, this, SkillName.Tasting );
			else if ( this.NotIDSkill == IDSkill.ArmsLore )
				RelicFunctions.IDItem( m, m, this, SkillName.ArmsLore );
			else
				RelicFunctions.IDItem( m, m, this, SkillName.Mercantile );
		}

		public DDRelicClock1( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version < 1 )
				CoinPrice = reader.ReadInt();
		}
	}
	[Flipable( 0x44DD, 0x44E1 )]
	public class DDRelicClock2 : Clock, IRelic
	{
		public override void ItemIdentified( bool id )
		{
			m_NotIdentified = id;
			if ( !id )
			{
				ColorHue3 = "FDC844";
				ColorText3 = "Worth " + CoinPrice + " Gold";
			}
		}

		[Constructable]
		public DDRelicClock2() : base( 0x44DD )
		{
			Weight = 100;

			CoinPrice = Utility.RandomMinMax( 80, 500 );
			NotIdentified = true;
			NotIDSource = Identity.Merchant;
			NotIDSkill = IDSkill.Mercantile;

			string sLook = "a rare";
			switch ( Utility.RandomMinMax( 0, 18 ) )
			{
				case 0:	sLook = "a rare";	break;
				case 1:	sLook = "a nice";	break;
				case 2:	sLook = "a pretty";	break;
				case 3:	sLook = "a superb";	break;
				case 4:	sLook = "a delightful";	break;
				case 5:	sLook = "an elegant";	break;
				case 6:	sLook = "an exquisite";	break;
				case 7:	sLook = "a fine";	break;
				case 8:	sLook = "a gorgeous";	break;
				case 9:	sLook = "a lovely";	break;
				case 10:sLook = "a magnificent";	break;
				case 11:sLook = "a marvelous";	break;
				case 12:sLook = "a splendid";	break;
				case 13:sLook = "a wonderful";	break;
				case 14:sLook = "an extraordinary";	break;
				case 15:sLook = "a strange";	break;
				case 16:sLook = "an odd";	break;
				case 17:sLook = "a unique";	break;
				case 18:sLook = "an unusual";	break;
			}
			Name = sLook + " grandfather clock";
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) && MySettings.S_IdentifyItemsOnlyInPack && from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified ) 
				from.SendMessage( "This must be in your backpack to identify." );
			else if ( from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified )
				IDCommand( from );
			else
				base.OnDoubleClick( from );
		}

		public override void IDCommand( Mobile m )
		{
			if ( this.NotIDSkill == IDSkill.Tasting )
				RelicFunctions.IDItem( m, m, this, SkillName.Tasting );
			else if ( this.NotIDSkill == IDSkill.ArmsLore )
				RelicFunctions.IDItem( m, m, this, SkillName.ArmsLore );
			else
				RelicFunctions.IDItem( m, m, this, SkillName.Mercantile );
		}

		public DDRelicClock2( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version < 1 )
				CoinPrice = reader.ReadInt();
		}
	}
	[Flipable( 0x48D4, 0x48D8 )]
	public class DDRelicClock3 : Clock, IRelic
	{
		public override void ItemIdentified( bool id )
		{
			m_NotIdentified = id;
			if ( !id )
			{
				ColorHue3 = "FDC844";
				ColorText3 = "Worth " + CoinPrice + " Gold";
			}
		}

		[Constructable]
		public DDRelicClock3() : base( 0x48D4 )
		{
			Weight = 100;

			CoinPrice = Utility.RandomMinMax( 80, 500 );
			NotIdentified = true;
			NotIDSource = Identity.Merchant;
			NotIDSkill = IDSkill.Mercantile;

			string sLook = "a rare";
			switch ( Utility.RandomMinMax( 0, 18 ) )
			{
				case 0:	sLook = "a rare";	break;
				case 1:	sLook = "a nice";	break;
				case 2:	sLook = "a pretty";	break;
				case 3:	sLook = "a superb";	break;
				case 4:	sLook = "a delightful";	break;
				case 5:	sLook = "an elegant";	break;
				case 6:	sLook = "an exquisite";	break;
				case 7:	sLook = "a fine";	break;
				case 8:	sLook = "a gorgeous";	break;
				case 9:	sLook = "a lovely";	break;
				case 10:sLook = "a magnificent";	break;
				case 11:sLook = "a marvelous";	break;
				case 12:sLook = "a splendid";	break;
				case 13:sLook = "a wonderful";	break;
				case 14:sLook = "an extraordinary";	break;
				case 15:sLook = "a strange";	break;
				case 16:sLook = "an odd";	break;
				case 17:sLook = "a unique";	break;
				case 18:sLook = "an unusual";	break;
			}
			Name = sLook + " grandfather clock";
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) && MySettings.S_IdentifyItemsOnlyInPack && from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified ) 
				from.SendMessage( "This must be in your backpack to identify." );
			else if ( from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified )
				IDCommand( from );
			else
				base.OnDoubleClick( from );
		}

		public override void IDCommand( Mobile m )
		{
			if ( this.NotIDSkill == IDSkill.Tasting )
				RelicFunctions.IDItem( m, m, this, SkillName.Tasting );
			else if ( this.NotIDSkill == IDSkill.ArmsLore )
				RelicFunctions.IDItem( m, m, this, SkillName.ArmsLore );
			else
				RelicFunctions.IDItem( m, m, this, SkillName.Mercantile );
		}

		public DDRelicClock3( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version < 1 )
				CoinPrice = reader.ReadInt();
		}
	}
}