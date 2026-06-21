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
using Server.Multis;

namespace Server.Items
{
	public class ShinobiScroll : Item
	{
		public override bool DisplayWeight { get { return false; } }

		public int CheetahPaws;
		public int Deception;
		public int EagleEye;
		public int Espionage;
		public int FerretFlee;
		public int MonkeyLeap;
		public int MysticShuriken;
		public int TigerStrength;

		public int Page;
		[CommandProperty(AccessLevel.Owner)]
		public int Page_ { get { return Page; } set { Page = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int CheetahPaws_ { get { return CheetahPaws; } set { CheetahPaws = value; InvalidateProperties(); } }
		[CommandProperty(AccessLevel.Owner)]
		public int Deception_ { get { return Deception; } set { Deception = value; InvalidateProperties(); } }
		[CommandProperty(AccessLevel.Owner)]
		public int EagleEye_ { get { return EagleEye; } set { EagleEye = value; InvalidateProperties(); } }
		[CommandProperty(AccessLevel.Owner)]
		public int Espionage_ { get { return Espionage; } set { Espionage = value; InvalidateProperties(); } }
		[CommandProperty(AccessLevel.Owner)]
		public int FerretFlee_ { get { return FerretFlee; } set { FerretFlee = value; InvalidateProperties(); } }
		[CommandProperty(AccessLevel.Owner)]
		public int MonkeyLeap_ { get { return MonkeyLeap; } set { MonkeyLeap = value; InvalidateProperties(); } }
		[CommandProperty(AccessLevel.Owner)]
		public int MysticShuriken_ { get { return MysticShuriken; } set { MysticShuriken = value; InvalidateProperties(); } }
		[CommandProperty(AccessLevel.Owner)]
		public int TigerStrength_ { get { return TigerStrength; } set { TigerStrength = value; InvalidateProperties(); } }

		public Mobile owner;
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner { get{ return owner; } set{ owner = value; } }

		[Constructable]
		public ShinobiScroll() : base( 0x5C15 )
		{
			Weight = 1.0;
			Name = "shinobi scroll";
		}

		public override void OnDoubleClick( Mobile from )
		{
			bool remove = false;
			if ( owner == null )
			{
				foreach ( Item item in World.Items.Values )
				if ( item is ShinobiScroll )
				{
					if ( ((ShinobiScroll)item).owner == from )
					{
						remove = true;
						from.AddToBackpack( item );
						from.PlaySound( 0x249 );
						from.SendMessage( "You already have a scroll so you toss this one out!" );
					}
				}
				owner = from;
				InvalidateProperties();
			}

			if ( Page == 0 ){ Page = 1; } else if ( Page < 2 ){ Page = 10; } else if ( Page > 10 ){ Page = 1; }

			if ( remove )
			{
				this.Delete();
			}
			else
			{
				if ( owner != from )
				{
					from.SendMessage( "This is not your scroll!" );
					return;
				}
				else if ( !IsChildOf( from.Backpack ) ) 
				{
					from.SendMessage( "This must be in your backpack to read." );
					return;
				}
				else
				{
					from.CloseGump( typeof( ShinobiScrollGump ) );
					from.SendGump( new ShinobiScrollGump( from, this ) );
					from.PlaySound( 0x249 );
				}
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			string win = "";
			if ( dropped is SummonItems && ((SummonItems)dropped).Owner == from )
			{
				if ( CheetahPaws < 1 && ( 			dropped.Name == ShinobiInfo( 290, "prize1" ) || dropped.Name == ShinobiInfo( 290, "prize2" ) ) ){ win = ShinobiInfo( 290, "name" ); CheetahPaws = 1; }
				else if ( Deception < 1 && ( 		dropped.Name == ShinobiInfo( 291, "prize1" ) || dropped.Name == ShinobiInfo( 291, "prize2" ) ) ){ win = ShinobiInfo( 291, "name" ); Deception = 1; }
				else if ( EagleEye < 1 && ( 		dropped.Name == ShinobiInfo( 292, "prize1" ) || dropped.Name == ShinobiInfo( 292, "prize2" ) ) ){ win = ShinobiInfo( 292, "name" ); EagleEye = 1; }
				else if ( Espionage < 1 && ( 		dropped.Name == ShinobiInfo( 293, "prize1" ) || dropped.Name == ShinobiInfo( 293, "prize2" ) ) ){ win = ShinobiInfo( 293, "name" ); Espionage = 1; }
				else if ( FerretFlee < 1 && ( 		dropped.Name == ShinobiInfo( 294, "prize1" ) || dropped.Name == ShinobiInfo( 294, "prize2" ) ) ){ win = ShinobiInfo( 294, "name" ); FerretFlee = 1; }
				else if ( MonkeyLeap < 1 && ( 		dropped.Name == ShinobiInfo( 295, "prize1" ) || dropped.Name == ShinobiInfo( 295, "prize2" ) ) ){ win = ShinobiInfo( 295, "name" ); MonkeyLeap = 1; }
				else if ( MysticShuriken < 1 && ( 	dropped.Name == ShinobiInfo( 296, "prize1" ) || dropped.Name == ShinobiInfo( 296, "prize2" ) ) ){ win = ShinobiInfo( 296, "name" ); MysticShuriken = 1; }
				else if ( TigerStrength < 1 && ( 	dropped.Name == ShinobiInfo( 297, "prize1" ) || dropped.Name == ShinobiInfo( 297, "prize2" ) ) ){ win = ShinobiInfo( 297, "name" ); TigerStrength = 1; }

				if ( win != "" )
				{
					from.CloseGump( typeof( ShinobiScrollGump ) );
					from.SendMessage( "You learned the " + win + " ability!" );
					from.PlaySound( 0x4D5 );
					dropped.Delete();
					InvalidateProperties();
				}
			}

			return false;
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);

			int skills = CheetahPaws + Deception + EagleEye + Espionage + FerretFlee + MonkeyLeap + MysticShuriken + TigerStrength;

            list.Add( 1049644, "" + skills + " Abilities");
			if ( owner != null ){ list.Add( 1070722, "Belongs to " + owner.Name + "" ); }
        } 

		public static bool GetShinobi( Mobile m, int skill )
		{
			ArrayList abilities = new ArrayList();
			foreach( Item item in m.Backpack.FindItemsByType( typeof( ShinobiScroll ), true ) )
			{
				abilities.Add( item );
			}
			for ( int i = 0; i < abilities.Count; ++i )
			{
				ShinobiScroll scroll = (ShinobiScroll)abilities[ i ];

				if ( scroll.owner == m )
				{
					if ( skill == 290 && scroll.CheetahPaws > 0 ){ return true; }
					else if ( skill == 291 && scroll.Deception > 0 ){ return true; }
					else if ( skill == 292 && scroll.EagleEye > 0 ){ return true; }
					else if ( skill == 293 && scroll.Espionage > 0 ){ return true; }
					else if ( skill == 294 && scroll.FerretFlee > 0 ){ return true; }
					else if ( skill == 295 && scroll.MonkeyLeap > 0 ){ return true; }
					else if ( skill == 296 && scroll.MysticShuriken > 0 ){ return true; }
					else if ( skill == 297 && scroll.TigerStrength > 0 ){ return true; }
				}
			}
			return false;
		}

		public static bool GetShinobiScroll( Mobile m, int skill, ShinobiScroll scroll )
		{
			if ( scroll.owner == m )
			{
				if ( skill == 290 && scroll.CheetahPaws > 0 ){ return true; }
				else if ( skill == 291 && scroll.Deception > 0 ){ return true; }
				else if ( skill == 292 && scroll.EagleEye > 0 ){ return true; }
				else if ( skill == 293 && scroll.Espionage > 0 ){ return true; }
				else if ( skill == 294 && scroll.FerretFlee > 0 ){ return true; }
				else if ( skill == 295 && scroll.MonkeyLeap > 0 ){ return true; }
				else if ( skill == 296 && scroll.MysticShuriken > 0 ){ return true; }
				else if ( skill == 297 && scroll.TigerStrength > 0 ){ return true; }
			}
			return false;
		}

		public ShinobiScroll( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 ); // version
			writer.Write( CheetahPaws );
			writer.Write( Deception );
			writer.Write( EagleEye );
			writer.Write( Espionage );
			writer.Write( FerretFlee );
			writer.Write( MonkeyLeap );
			writer.Write( MysticShuriken );
			writer.Write( TigerStrength );
			writer.Write( Page );
			writer.Write( (Mobile)owner);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
            CheetahPaws = reader.ReadInt();
			Deception = reader.ReadInt();
			EagleEye = reader.ReadInt();
			Espionage = reader.ReadInt();
			FerretFlee = reader.ReadInt();
			MonkeyLeap = reader.ReadInt();
			MysticShuriken = reader.ReadInt();
			TigerStrength = reader.ReadInt();
			Page = reader.ReadInt();
			owner = reader.ReadMobile();
		}

        public static void InvokeCommand( string c, Mobile from )
        {
            CommandSystem.Handle(from, String.Format("{0}{1}", CommandSystem.Prefix, c));
        }

		public static void DoShinobi( Mobile from, int skill )
		{
			if ( skill == 290 && GetShinobi( from, skill ) ){ InvokeCommand( "CheetahPaws", from ); }
			else if ( skill == 291 && GetShinobi( from, skill ) ){ InvokeCommand( "Deception", from ); }
			else if ( skill == 292 && GetShinobi( from, skill ) ){ InvokeCommand( "EagleEye", from ); }
			else if ( skill == 293 && GetShinobi( from, skill ) ){ InvokeCommand( "Espionage", from ); }
			else if ( skill == 294 && GetShinobi( from, skill ) ){ InvokeCommand( "FerretFlee", from ); }
			else if ( skill == 295 && GetShinobi( from, skill ) ){ InvokeCommand( "MonkeyLeap", from ); }
			else if ( skill == 296 && GetShinobi( from, skill ) ){ InvokeCommand( "MysticShuriken", from ); }
			else if ( skill == 297 && GetShinobi( from, skill ) ){ InvokeCommand( "TigerStrength", from ); }
		}

		public static string ShinobiSpeech( ShinobiScroll scroll )
		{
			int skills = scroll.CheetahPaws + scroll.Deception + scroll.EagleEye + scroll.Espionage + scroll.FerretFlee + scroll.MonkeyLeap + scroll.MysticShuriken + scroll.TigerStrength;

			string text = "Ninjas are the exotic assassins, thieves, and spies of the land. They prefer stealth and sneak attacks above most other forms of conflict, and they have a series of mystical maneuvers they can perform. These abilities are the way of the shinobi. ";

			if ( skills < 8 )
			{
				text = text + "To prove yourself of being worthy of these abilities, you will have to overcome a series of trials. Each ability will list the item you must seek and the place where it can be sought. The ninja has a choice of trial to undertake, as each ability provides two choices. One is in the land of Sosaria and the other is in the land of Lodoria. Slay the beasts and get the items. Place them on this scroll and the shinobi secrets will be revealed to you. ";
			}

			text = text + "<br><br>Each ability will display the description as well as the skill required, the tithing gold needed, along with the mana necessary. To tithe gold to the spirits, find a Shrine of Durama where you can select it and then offer the gold you wish to part with. You can usually find these shrines in a dojo, and some ninjas tithe gold to almost any other deity. There are choices for having a horizontal or vertical menu bar to use these abilities quickly, but you can also select the icon within this scroll to use the skill.";

			text = text + "<br><br>There are some commands you can type to use your shinobi abilities: <br><br>[CheetahPaws <br><br>[Deception <br><br>[EagleEye <br><br>[Espionage <br><br>[FerretFlee <br><br>[MonkeyLeap <br><br>[MysticShuriken <br><br>[TigerStrength <br><br>";

			return text;
		}

		public static string ShinobiInfo( int ability, string type )
		{
			string str = "";

			if ( ability == 290 )
			{
				if ( type == "name" ){ 			str = "Cheetah Paws"; }
				else if ( type == "points" ){ 	str = "65"; }
				else if ( type == "mana" ){ 	str = "60"; }
				else if ( type == "skill" ){	str = "80"; }
				else if ( type == "icon" ){ 	str = "10876"; }
				else if ( type == "prize1" ){	str = "chest of suffering"; }
				else if ( type == "prize2" ){	str = "egg of the harpy hen"; }
				else if ( type == "where1" ){	str = "the Ancient Pyramid"; }
				else if ( type == "where2" ){	str = "Dungeon Covetous"; }
				else
				{ 
					str = "This increases the running speed of the Ninja for about 10-25 minutes, making them run as fast as a cheetah. This power cannot be called upon within certain areas and will often cease to function when entering those areas.";
				}
			}
			else if ( ability == 291 )
			{
				if ( type == "name" ){ 			str = "Deception"; }
				else if ( type == "points" ){ 	str = "20"; }
				else if ( type == "mana" ){ 	str = "15"; }
				else if ( type == "skill" ){	str = "30"; }
				else if ( type == "icon" ){ 	str = "10871"; }
				else if ( type == "prize1" ){	str = "braclet of war"; }
				else if ( type == "prize2" ){	str = "face of the ancient king"; }
				else if ( type == "where1" ){	str = "Dungeon Clues"; }
				else if ( type == "where2" ){	str = "the Lodoria Catacombs"; }
				else
				{ 
					str = "The ninja can disguise themselves, where others would not recognize them and some guards may look the other way.";
				}
			}
			else if ( ability == 292 )
			{
				if ( type == "name" ){ 			str = "Eagle Eye"; }
				else if ( type == "points" ){ 	str = "55"; }
				else if ( type == "mana" ){ 	str = "50"; }
				else if ( type == "skill" ){	str = "70"; }
				else if ( type == "icon" ){ 	str = "10872"; }
				else if ( type == "prize1" ){	str = "stump of the ancients"; }
				else if ( type == "prize2" ){	str = "wand of Talosh"; }
				else if ( type == "where1" ){	str = "Dardin's Pit"; }
				else if ( type == "where2" ){	str = "Dungeon Deceit"; }
				else
				{ 
					str = "The eyes of the ninja are focused where they can perhaps spot hidden creatures, traps, or treasure.";
				}
			}
			else if ( ability == 293 )
			{
				if ( type == "name" ){ 			str = "Espionage"; }
				else if ( type == "points" ){ 	str = "15"; }
				else if ( type == "mana" ){ 	str = "10"; }
				else if ( type == "skill" ){	str = "20"; }
				else if ( type == "icon" ){ 	str = "10873"; }
				else if ( type == "prize1" ){	str = "dark blood"; }
				else if ( type == "prize2" ){	str = "head of Urg"; }
				else if ( type == "where1" ){	str = "Dungeon Doom"; }
				else if ( type == "where2" ){	str = "Dungeon Despise"; }
				else
				{ 
					str = "Some of the more minor locks can be manipulated with this ability, but not all of them as master thieves can.";
				}
			}
			else if ( ability == 294 )
			{
				if ( type == "name" ){ 			str = "Ferret Flee"; }
				else if ( type == "points" ){ 	str = "35"; }
				else if ( type == "mana" ){ 	str = "30"; }
				else if ( type == "skill" ){	str = "50"; }
				else if ( type == "icon" ){ 	str = "10874"; }
				else if ( type == "prize1" ){	str = "firescale tooth"; }
				else if ( type == "prize2" ){	str = "crown of Vorgol"; }
				else if ( type == "where1" ){	str = "the Fires of Hell"; }
				else if ( type == "where2" ){	str = "the City of Embers"; }
				else
				{ 
					str = "If held in place by things such as paralysis magic, spider webbing, or nets the ninja can attempt to free themselves from such holds and escape.";
				}
			}
			else if ( ability == 295 )
			{
				if ( type == "name" ){ 			str = "Monkey Leap"; }
				else if ( type == "points" ){ 	str = "25"; }
				else if ( type == "mana" ){ 	str = "20"; }
				else if ( type == "skill" ){	str = "40"; }
				else if ( type == "icon" ){ 	str = "10875"; }
				else if ( type == "prize1" ){	str = "ichor of Xthizx"; }
				else if ( type == "prize2" ){	str = "claw of Saramon"; }
				else if ( type == "where1" ){	str = "the Mines of Morinia"; }
				else if ( type == "where2" ){	str = "Dungeon Hythloth"; }
				else
				{ 
					str = "Allows the ninja to leap toward or away from a location very quickly.";
				}
			}
			else if ( ability == 296 )
			{
				if ( type == "name" ){ 			str = "Mystic Shuriken"; }
				else if ( type == "points" ){ 	str = "45"; }
				else if ( type == "mana" ){ 	str = "40"; }
				else if ( type == "skill" ){	str = "60"; }
				else if ( type == "icon" ){ 	str = "10877"; }
				else if ( type == "prize1" ){	str = "heart of a vampire queen"; }
				else if ( type == "prize2" ){	str = "horn of the frozen hells"; }
				else if ( type == "where1" ){	str = "the Perinian Depths"; }
				else if ( type == "where2" ){	str = "the Ice Fiend Lair"; }
				else
				{ 
					str = "Summons a shuriken out of thin air and hurdles it toward your opponent, causing much damage from afar.";
				}
			}
			else if ( ability == 297 )
			{
				if ( type == "name" ){ 			str = "Tiger Strength"; }
				else if ( type == "points" ){	str = "75"; }
				else if ( type == "mana" ){ 	str = "70"; }
				else if ( type == "skill" ){	str = "90"; }
				else if ( type == "icon" ){ 	str = "10878"; }
				else if ( type == "prize1" ){	str = "hourglass of ages"; }
				else if ( type == "prize2" ){	str = "elemental salt"; }
				else if ( type == "where1" ){	str = "the Dungeon of Time Awaits"; }
				else if ( type == "where2" ){	str = "Dungeon Shame"; }
				else
				{ 
					str = "Calls forth a mystical tiger from the realm of Durama to fight with the ninja during their journey.";
				}
			}

			return str;
		}

		public class ShinobiScrollGump : Gump
		{
			private ShinobiScroll mScroll;
			public ShinobiScrollGump( Mobile from, ShinobiScroll scroll ): base( 100, 100 )
			{
				mScroll = scroll;
				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				int left = scroll.Page - 1; 	if ( left < 1 ){ left = 10; }
				int right = scroll.Page + 1;	if ( right > 10 ){ right = 1; }

				AddImage(0, 0, 10879);
				AddButton(41, 12, 4014, 4014, left, GumpButtonType.Reply, 0);
				AddButton(454, 12, 4005, 4005, right, GumpButtonType.Reply, 0);

				if ( scroll.Page == 1 )
				{
					AddHtml( 71, 75, 384, 140, @"<BODY><BASEFONT Color=#111111><BIG>" + ShinobiSpeech( mScroll ) + "</BIG></BASEFONT></BODY>", (bool)false, (bool)true);
				}
				else if ( scroll.Page == 2 )
				{
					AddButton(60, 90, 4011, 4011, 21, GumpButtonType.Reply, 0);
					AddHtml( 100, 90, 340, 21, @"<BODY><BASEFONT Color=#111111><BIG>Open Horizontal Ability Bar</BIG></BASEFONT></BODY>", (bool)false, (bool)false);

					AddButton(60, 135, 4011, 4011, 22, GumpButtonType.Reply, 0);
					AddHtml( 100, 135, 340, 21, @"<BODY><BASEFONT Color=#111111><BIG>Open Vertical Ability Bar</BIG></BASEFONT></BODY>", (bool)false, (bool)false);

					AddButton(60, 180, 4017, 4017, 23, GumpButtonType.Reply, 0);
					AddHtml( 100, 180, 340, 21, @"<BODY><BASEFONT Color=#111111><BIG>Close Ability Bar</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				}
				else if ( scroll.Page >= 3 && scroll.Page <= 10 )
				{
					int ability = 287 + scroll.Page;
					string clue = "";
					if ( GetShinobiScroll( from, ability, mScroll ) )
					{
						AddButton(47, 56, Int32.Parse( Server.Items.ShinobiScroll.ShinobiInfo( ability, "icon" ) ), Int32.Parse( Server.Items.ShinobiScroll.ShinobiInfo( ability, "icon" ) ), ability, GumpButtonType.Reply, 0);
						clue = ShinobiInfo( ability, "text" );
					}
					else
					{
						AddImage(47, 57, Int32.Parse( Server.Items.ShinobiScroll.ShinobiInfo( ability, "icon" ) ));
						clue = "To be worthy of this ability, you need to either get the " + ShinobiInfo( ability, "prize1" ) + " at " + ShinobiInfo( ability, "where1" ) + " in the Land of Sosaria or the " + ShinobiInfo( ability, "prize2" ) + " at " + ShinobiInfo( ability, "where2" ) + " in the Land of Lodoria.";
					}

					AddHtml( 56, 109, 414, 92, @"<BODY><BASEFONT Color=#111111><BIG>" + clue + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 98, 68, 340, 21, @"<BODY><BASEFONT Color=#111111><BIG>" + ShinobiInfo( ability, "name" ) + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);

					AddItem(71, 205, 10126);
					AddHtml( 110, 205, 48, 21, @"<BODY><BASEFONT Color=#111111><BIG>" + ShinobiInfo( ability, "skill" ) + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
					AddTooltip("Required skill");

					AddItem(220, 208, 3822);
					AddHtml( 254, 205, 48, 21, @"<BODY><BASEFONT Color=#111111><BIG>" + ShinobiInfo( ability, "points" ) + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
					AddTooltip("Required tithing");

					AddItem(364, 204, 10231);
					AddHtml( 402, 205, 48, 21, @"<BODY><BASEFONT Color=#111111><BIG>" + ShinobiInfo( ability, "mana" ) + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false);
					AddTooltip("Required mana");
				}
			}

			public override void OnResponse( NetState state, RelayInfo info ) 
			{
				Mobile from = state.Mobile;

				bool showScroll = true;
				from.PlaySound( 0x249 );

				if ( info.ButtonID < 1 ){ showScroll = false; }
				else if ( info.ButtonID > 289 ){ DoShinobi( from, info.ButtonID ); }
				else if ( info.ButtonID > 20 && info.ButtonID < 24 )
				{
					from.CloseGump( typeof( ShinobiRow ) );
					from.CloseGump( typeof( ShinobiColumn ) );
					showScroll = false;
				}

				if ( info.ButtonID == 21 ){ 		from.SendGump( new ShinobiRow( from, mScroll ) ); 		showScroll = false; }
				else if ( info.ButtonID == 22 ){ 	from.SendGump( new ShinobiColumn( from, mScroll ) ); 	showScroll = false; }

				if ( info.ButtonID < 290 && info.ButtonID > 0 ){ from.SendSound( 0x4A ); }

				from.CloseGump( typeof( ShinobiScrollGump ) );

				if ( showScroll )
				{
					if ( info.ButtonID >= 1 && info.ButtonID <= 10 ){ mScroll.Page = info.ButtonID; }
					from.SendGump( new ShinobiScrollGump( from, mScroll ) );
				}
			}
		}

		public class ShinobiRow : Gump
		{
			private ShinobiScroll mScroll;
			public ShinobiRow( Mobile from, ShinobiScroll scroll ): base( 25, 25 )
			{
				mScroll = scroll;
				this.Closable=false;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);
				AddImage(2, 0, 10880);
				int line = 0;
				if ( scroll.CheetahPaws > 0 ){ 		line = line+50; AddButton(line, 2, 10876, 10876, 290, GumpButtonType.Reply, 0); }
				if ( scroll.Deception > 0 ){ 		line = line+50; AddButton(line, 2, 10871, 10871, 291, GumpButtonType.Reply, 0); }
				if ( scroll.EagleEye > 0 ){ 		line = line+50; AddButton(line, 2, 10872, 10872, 292, GumpButtonType.Reply, 0); }
				if ( scroll.Espionage > 0 ){ 		line = line+50; AddButton(line, 2, 10873, 10873, 293, GumpButtonType.Reply, 0); }
				if ( scroll.FerretFlee > 0 ){ 		line = line+50; AddButton(line, 2, 10874, 10874, 294, GumpButtonType.Reply, 0); }
				if ( scroll.MonkeyLeap > 0 ){ 		line = line+50; AddButton(line, 2, 10875, 10875, 295, GumpButtonType.Reply, 0); }
				if ( scroll.MysticShuriken > 0 ){ 	line = line+50; AddButton(line, 2, 10877, 10877, 296, GumpButtonType.Reply, 0); }
				if ( scroll.TigerStrength > 0 ){ 	line = line+50; AddButton(line, 2, 10878, 10878, 297, GumpButtonType.Reply, 0); }
			}

			public override void OnResponse( NetState state, RelayInfo info ) 
			{
				Mobile from = state.Mobile; 
				DoShinobi( from, info.ButtonID );
				from.CloseGump( typeof( ShinobiRow ) );
				if ( mScroll.owner == from )
				{
					from.SendGump( new ShinobiRow( from, mScroll ) );
				}
			}
		}

		public class ShinobiColumn : Gump
		{
			private ShinobiScroll mScroll;
			public ShinobiColumn( Mobile from, ShinobiScroll scroll ): base( 25, 25 )
			{
				mScroll = scroll;
				this.Closable=false;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);
				AddImage(4, 0, 10880);
				int line = 3;
				if ( scroll.CheetahPaws > 0 ){ 		line = line+50; AddButton(2, line, 10876, 10876, 290, GumpButtonType.Reply, 0); }
				if ( scroll.Deception > 0 ){ 		line = line+50; AddButton(2, line, 10871, 10871, 291, GumpButtonType.Reply, 0); }
				if ( scroll.EagleEye > 0 ){ 		line = line+50; AddButton(2, line, 10872, 10872, 292, GumpButtonType.Reply, 0); }
				if ( scroll.Espionage > 0 ){ 		line = line+50; AddButton(2, line, 10873, 10873, 293, GumpButtonType.Reply, 0); }
				if ( scroll.FerretFlee > 0 ){ 		line = line+50; AddButton(2, line, 10874, 10874, 294, GumpButtonType.Reply, 0); }
				if ( scroll.MonkeyLeap > 0 ){ 		line = line+50; AddButton(2, line, 10875, 10875, 295, GumpButtonType.Reply, 0); }
				if ( scroll.MysticShuriken > 0 ){ 	line = line+50; AddButton(2, line, 10877, 10877, 296, GumpButtonType.Reply, 0); }
				if ( scroll.TigerStrength > 0 ){ 	line = line+50; AddButton(2, line, 10878, 10878, 297, GumpButtonType.Reply, 0); }
			}

			public override void OnResponse( NetState state, RelayInfo info ) 
			{
				Mobile from = state.Mobile; 
				DoShinobi( from, info.ButtonID );
				from.CloseGump( typeof( ShinobiColumn ) );
				if ( mScroll.owner == from )
				{
					from.SendGump( new ShinobiColumn( from, mScroll ) );
				}
			}
		}
	}
}