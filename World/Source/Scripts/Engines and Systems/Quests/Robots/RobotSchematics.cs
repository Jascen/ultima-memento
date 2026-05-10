using System;
using Server; 
using System.Collections;
using Server.ContextMenus;
using System.Collections.Generic;
using Server.Misc;
using Server.Network;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Commands;
using System.Globalization;
using Server.Regions;

namespace Server.Items
{
	public class RobotSchematics : Item
	{
		[Constructable]
		public RobotSchematics() : base( 0x27FB )
		{
			Weight = 2.0;
			Name = "Robot Schematics";
			Light = LightType.Circle150;

			if ( Weight > 1.0 )
			{
				Weight = 1.0;

				HaveBolts = 0;
				HaveEngineParts = 0;
				HaveCircuitBoard = 0;
				HaveGears = 0;
				HaveTransistors = 0;
				HaveXormite = 0;
				HaveMetalQty = 0;
				HaveOil = 0;

				Hue = 0xBA1;

				NeedBolts = 45;
				NeedEngineParts = 9;
				NeedCircuitBoard = 9;
				NeedGears = 45;
				NeedTransistors = 9;
				NeedXormite = 10000;
				NeedMetalQty = 1000;
				NeedOil = 27;

				TinkerLocation = Server.Items.GolemManual.GetRandomTinker();
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			from.SendSound( 0x54D );
			from.CloseGump( typeof( RobotSchematicsGump ) );
			from.SendGump( new RobotSchematicsGump( from, this ) );
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{          		
			Container pack = from.Backpack;
			int iAmount = 0;
			string sEnd = ".";

			if ( from != null )
			{
				int WhatIsDropped = dropped.Amount;

				int WhatIsNeeded = NeedMetalQty - HaveMetalQty;
				int WhatIsExtra = WhatIsDropped - WhatIsNeeded; if ( WhatIsExtra < 1 ){ WhatIsExtra = 0; }
				int WhatIsTaken = WhatIsDropped - WhatIsExtra;

				if ( dropped is RobotSheetMetal && NeedMetalQty > HaveMetalQty )
				{
					if ( WhatIsExtra > 0 ){ from.AddToBackpack( new RobotSheetMetal( WhatIsExtra ) ); }
					iAmount = WhatIsTaken;

					if ( iAmount > 1 ){ sEnd = "s."; }

					HaveMetalQty = HaveMetalQty + iAmount;
					from.SendMessage( "You added " + iAmount.ToString() + " metal plate" + sEnd );
					dropped.Delete();
					return true;
				}
				else if ( dropped is RobotCircuitBoard && NeedCircuitBoard > HaveCircuitBoard )
				{
					dropped.Delete();
					HaveCircuitBoard = HaveCircuitBoard + 1;
					from.SendMessage( "You added a circuit board." );
					dropped.Delete();
					return true;
				}
				else if ( dropped is RobotTransistor && NeedTransistors > HaveTransistors )
				{
					dropped.Delete();
					HaveTransistors = HaveTransistors + 1;
					from.SendMessage( "You added a transistor." );
					dropped.Delete();
					return true;
				}
				else if ( dropped is RobotEngineParts && NeedEngineParts > HaveEngineParts )
				{
					dropped.Delete();
					HaveEngineParts = HaveEngineParts + 1;
					from.SendMessage( "You added some engine parts." );
					dropped.Delete();
					return true;
				}
				else if ( dropped is RobotOil && NeedOil > HaveOil )
				{
					WhatIsNeeded = NeedOil - HaveOil;
					WhatIsExtra = WhatIsDropped - WhatIsNeeded; if ( WhatIsExtra < 1 ){ WhatIsExtra = 0; }
					WhatIsTaken = WhatIsDropped - WhatIsExtra;

					if ( WhatIsExtra > 0 ){ from.AddToBackpack( new RobotOil( WhatIsExtra ) ); }
					iAmount = WhatIsTaken;

					if ( iAmount > 1 ){ sEnd = "s."; }

					HaveOil = HaveOil + iAmount;
					from.SendMessage( "You added " + iAmount.ToString() + " robot oil can" + sEnd );
					dropped.Delete();
					return true;
				}
				else if ( dropped is RobotGears && NeedGears > HaveGears )
				{
					WhatIsNeeded = NeedGears - HaveGears;
					WhatIsExtra = WhatIsDropped - WhatIsNeeded; if ( WhatIsExtra < 1 ){ WhatIsExtra = 0; }
					WhatIsTaken = WhatIsDropped - WhatIsExtra;

					if ( WhatIsExtra > 0 ){ from.AddToBackpack( new RobotGears( WhatIsExtra ) ); }
					iAmount = WhatIsTaken;

					if ( iAmount > 1 ){ sEnd = "s."; }

					HaveGears = HaveGears + iAmount;
					from.SendMessage( "You added " + iAmount.ToString() + " gear" + sEnd );
					dropped.Delete();
					return true;
				}
				else if ( dropped is RobotBolt && NeedBolts > HaveBolts )
				{
					WhatIsNeeded = NeedBolts - HaveBolts;
					WhatIsExtra = WhatIsDropped - WhatIsNeeded; if ( WhatIsExtra < 1 ){ WhatIsExtra = 0; }
					WhatIsTaken = WhatIsDropped - WhatIsExtra;

					if ( WhatIsExtra > 0 ){ from.AddToBackpack( new RobotBolt( WhatIsExtra ) ); }
					iAmount = WhatIsTaken;

					if ( iAmount > 1 ){ sEnd = "s."; }

					HaveBolts = HaveBolts + iAmount;
					from.SendMessage( "You added " + iAmount.ToString() + " bolts" + sEnd );
					dropped.Delete();
					return true;
				}
				else if ( dropped is DDXormite && NeedXormite > HaveXormite )
				{
					WhatIsNeeded = NeedXormite - HaveXormite;
					WhatIsExtra = WhatIsDropped - WhatIsNeeded; if ( WhatIsExtra < 1 ){ WhatIsExtra = 0; }
					WhatIsTaken = WhatIsDropped - WhatIsExtra;

					if ( WhatIsExtra > 0 ){ from.AddToBackpack( new DDXormite( WhatIsExtra ) ); }
					iAmount = WhatIsTaken;

					if ( iAmount > 1 ){ sEnd = "s."; }

					HaveXormite = HaveXormite + iAmount;
					from.SendMessage( "You added " + iAmount.ToString() + " xormite coin" + sEnd );
					dropped.Delete();
					return true;
				}
			}

			return false;
		}

		public RobotSchematics( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 ); // version

			writer.Write( HaveBolts );
			writer.Write( HaveEngineParts );
			writer.Write( HaveCircuitBoard );
			writer.Write( HaveGears );
			writer.Write( HaveTransistors );
			writer.Write( HaveXormite );
			writer.Write( HaveMetalQty );
			writer.Write( HaveOil );
			writer.Write( NeedBolts );
			writer.Write( NeedEngineParts );
			writer.Write( NeedCircuitBoard );
			writer.Write( NeedGears );
			writer.Write( NeedTransistors );
			writer.Write( NeedXormite );
			writer.Write( NeedMetalQty );
			writer.Write( NeedOil );
			writer.Write( TinkerLocation );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			HaveBolts = reader.ReadInt();
			HaveEngineParts = reader.ReadInt();
			HaveCircuitBoard = reader.ReadInt();
			HaveGears = reader.ReadInt();
			HaveTransistors = reader.ReadInt();
			HaveXormite = reader.ReadInt();
			HaveMetalQty = reader.ReadInt();
			HaveOil = reader.ReadInt();
			NeedBolts = reader.ReadInt();
			NeedEngineParts = reader.ReadInt();
			NeedCircuitBoard = reader.ReadInt();
			NeedGears = reader.ReadInt();
			NeedTransistors = reader.ReadInt();
			NeedXormite = reader.ReadInt();
			NeedMetalQty = reader.ReadInt();
			NeedOil = reader.ReadInt();
			TinkerLocation = reader.ReadString();
		}

		public static bool ProcessRobotBook( Mobile m, Mobile tinker, Item dropped )
		{
			RobotSchematics book = (RobotSchematics)dropped;

			if ( Server.Misc.Worlds.GetRegionName( tinker.Map, tinker.Location ) != book.TinkerLocation ){ return false; }

			int tinkerSkill = (int)(m.Skills[SkillName.Tinkering].Value);
				if ( tinkerSkill > 100 ){ tinkerSkill = 100; }

			int XormiteReturn = 0;
				if ( tinkerSkill > 0 ){ XormiteReturn = (int)( book.NeedXormite * ( tinkerSkill * 0.005 ) ); }

			int HaveIngredients = 0;

			if ( book.HaveEngineParts >= book.NeedEngineParts ){ HaveIngredients++; }
			if ( book.HaveCircuitBoard >= book.NeedCircuitBoard ){ HaveIngredients++; }
			if ( book.HaveGears >= book.NeedGears ){ HaveIngredients++; }
			if ( book.HaveTransistors >= book.NeedTransistors ){ HaveIngredients++; }
			if ( book.HaveXormite >= book.NeedXormite ){ HaveIngredients++; }
			if ( book.HaveOil >= book.NeedOil ){ HaveIngredients++; }
			if ( book.HaveMetalQty >= book.NeedMetalQty ){ HaveIngredients++; }
			if ( book.HaveBolts >= book.NeedBolts ){ HaveIngredients++; }

			if ( HaveIngredients < 8 ){ return false; }

			if ( XormiteReturn > 0 ){ m.AddToBackpack( new DDXormite( XormiteReturn ) ); tinker.Say( "Here is " + XormiteReturn.ToString() + " xormite back for all of your help." ); }

			RobotItem ball = new RobotItem();

			ball.RobotOwner = m;
			ball.Hue = 0;

			m.AddToBackpack ( ball );

			LoggingFunctions.LogGenericQuest( m, "had a robot built" );

			m.PrivateOverheadMessage(MessageType.Regular, 1153, false, "My robot has been built.", m.NetState);
			m.SendSound( 0x559 );

			dropped.Delete();

			return true;
		}

		public class RobotSchematicsGump : Gump
		{
			private RobotSchematics m_Book;

			public RobotSchematicsGump( Mobile from, RobotSchematics gBook ): base( 25, 25 )
			{
				m_Book = gBook;
				RobotSchematics pedia = (RobotSchematics)gBook;

				string sText = "This schematic contains the knowledge to construct a robot. With these plans you will see what you need to obtain in order to have it constructed. These items are particularly used in the construction of robots. You will need metal, bolts, engine parts, circuit boards, gears, transistors, oil, and xormite for the tinker's fee. The tinker that can actually construct this robot is at the location shown at the bottom of this screen. If you have any tinkering skill, they may refund some of the xormite for the help you may provide in the construction. These robots are programmed for combat and will consume 3 follower slots to accompany you. As you find materials, simply drag and drop them onto this data pad to add to the materials. The top half of the screen will track what you have obtained thus far. When every item is acquired, give this data pad to a tinker and they will construct your robot. The robot built will be yours alone, and it will have a limited amount of charges. A charge is used whenever you power on the robot to travel with you. You will have to obtain more batteries in order to add more charges, where each battery will add an extra charge. A robot can only hold 100 charges at a time. Robots are controlled just like tamed beasts or summoned creatures. They can be told to follow, stay, or stop. You can also dismiss the robot, where the robot item will reappear in your pack. They are automatons so they do not need to be fed, other than the batteries already mentioned. Remember, they are not transferable once constructed. The one giving the tinker the book will own the robot that is constructed.";

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);
				AddImage(0, 0, 30521);
				AddItem(596, 33, 13697);

				AddHtml( 46, 42, 310, 20, @"<BODY><BASEFONT Color=#00FF06>Robot Schematics</BIG></BASEFONT></BODY>", (bool)false, (bool)false);

				AddItem(45, 81, 13636);
				AddHtml( 85, 83, 136, 20, @"<BODY><BASEFONT Color=#00FF06>Sheet Metal</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 245, 83, 136, 20, @"<BODY><BASEFONT Color=#00FF06>" + gBook.HaveMetalQty.ToString() + "/" + gBook.NeedMetalQty.ToString() + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);

				AddItem(46, 114, 13505);
				AddHtml( 85, 116, 136, 20, @"<BODY><BASEFONT Color=#00FF06>Engine Parts</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 245, 116, 136, 20, @"<BODY><BASEFONT Color=#00FF06>" + gBook.HaveEngineParts.ToString() + "/" + gBook.NeedEngineParts.ToString() + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);

				AddItem(44, 149, 13421);
				AddHtml( 85, 151, 136, 20, @"<BODY><BASEFONT Color=#00FF06>Curcuit Board</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 245, 151, 136, 20, @"<BODY><BASEFONT Color=#00FF06>" + gBook.HaveCircuitBoard.ToString() + "/" + gBook.NeedCircuitBoard.ToString() + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);

				AddItem(46, 183, 13382);
				AddHtml( 85, 185, 136, 20, @"<BODY><BASEFONT Color=#00FF06>Transistors</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 245, 185, 136, 20, @"<BODY><BASEFONT Color=#00FF06>" + gBook.HaveTransistors.ToString() + "/" + gBook.NeedTransistors.ToString() + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);

				AddItem(46, 218, 8238);
				AddHtml( 85, 220, 136, 20, @"<BODY><BASEFONT Color=#00FF06>Gears</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 245, 220, 136, 20, @"<BODY><BASEFONT Color=#00FF06>" + gBook.HaveGears.ToString() + "/" + gBook.NeedGears.ToString() + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);

				AddItem(396, 184, 13635);
				AddHtml( 435, 185, 136, 20, @"<BODY><BASEFONT Color=#00FF06>Oil</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 595, 185, 136, 20, @"<BODY><BASEFONT Color=#00FF06>" + gBook.HaveOil.ToString() + "/" + gBook.NeedOil.ToString() + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);

				AddItem(394, 218, 8225);
				AddHtml( 435, 220, 136, 20, @"<BODY><BASEFONT Color=#00FF06>Bolts</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 595, 220, 136, 20, @"<BODY><BASEFONT Color=#00FF06>" + gBook.HaveBolts.ToString() + "/" + gBook.NeedBolts.ToString() + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);

				AddItem(398, 41, 3823, 0xB96);
				AddHtml( 439, 43, 136, 20, @"<BODY><BASEFONT Color=#00FF06>" + gBook.HaveXormite.ToString() + "/" + gBook.NeedXormite.ToString() + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 55, 256, 660, 321, @"<BODY><BASEFONT Color=#00FF06>" + sText + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 55, 593, 653, 20, @"<BODY><BASEFONT Color=#00FF06>Bring Gathered Materials to the Tinker in " + gBook.TinkerLocation + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				Mobile from = state.Mobile;
				from.SendSound( 0x54D );
			}
		}

		public string TinkerLocation;
		[CommandProperty( AccessLevel.GameMaster )]
		public string g_TinkerLocation { get{ return TinkerLocation; } set{ TinkerLocation = value; } }

		// ----------------------------------------------------------------------------------------

		public int NeedMetalQty;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_NeedMetalQty { get{ return NeedMetalQty; } set{ NeedMetalQty = value; } }

		public int NeedBolts;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_NeedBolts { get{ return NeedBolts; } set{ NeedBolts = value; } }

		public int NeedXormite;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_NeedXormite { get{ return NeedXormite; } set{ NeedXormite = value; } }

		public int NeedOil;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_NeedOil { get{ return NeedOil; } set{ NeedOil = value; } }

		public int NeedCircuitBoard;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_NeedCircuitBoard { get{ return NeedCircuitBoard; } set{ NeedCircuitBoard = value; } }

		public int NeedEngineParts;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_NeedEngineParts { get{ return NeedEngineParts; } set{ NeedEngineParts = value; } }

		public int NeedTransistors;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_NeedTransistors { get{ return NeedTransistors; } set{ NeedTransistors = value; } }

		public int NeedGears;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_NeedGears { get{ return NeedGears; } set{ NeedGears = value; } }

		// ----------------------------------------------------------------------------------------

		public int HaveMetalQty;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveMetalQty { get{ return HaveMetalQty; } set{ HaveMetalQty = value; } }

		public int HaveBolts;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveBolts { get{ return HaveBolts; } set{ HaveBolts = value; } }

		public int HaveXormite;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveXormite { get{ return HaveXormite; } set{ HaveXormite = value; } }

		public int HaveOil;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveOil { get{ return HaveOil; } set{ HaveOil = value; } }

		public int HaveCircuitBoard;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveCircuitBoard { get{ return HaveCircuitBoard; } set{ HaveCircuitBoard = value; } }

		public int HaveEngineParts;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveEngineParts { get{ return HaveEngineParts; } set{ HaveEngineParts = value; } }

		public int HaveTransistors;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveTransistors { get{ return HaveTransistors; } set{ HaveTransistors = value; } }

		public int HaveGears;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveGears { get{ return HaveGears; } set{ HaveGears = value; } }
	}
}