using System;
using Server;
using Server.Network;
using Server.Multis;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
using Server.Accounting;
using System.Collections.Generic;
using System.Collections;
using Server.Regions; 
using System.Globalization;

namespace Server.Items
{
	public class QuestTome : Item
	{
		public Map DeliverMap;
		public int DeliverX;
		public int DeliverY;

		public Mobile QuestTomeOwner;
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile QuestTome_Owner { get{ return QuestTomeOwner; } set{ QuestTomeOwner = value; } }

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public string QuestTomeStoryGood;
		[CommandProperty( AccessLevel.GameMaster )]
		public string QuestTome_StoryGood { get{ return QuestTomeStoryGood; } set{ QuestTomeStoryGood = value; } }

		public string QuestTomeLocateGood;
		[CommandProperty( AccessLevel.GameMaster )]
		public string QuestTome_LocateGood { get{ return QuestTomeLocateGood; } set{ QuestTomeLocateGood = value; } }

		public Land QuestTomeWorldGood;
		[CommandProperty( AccessLevel.GameMaster )]
		public Land QuestTome_WorldGood { get{ return QuestTomeWorldGood; } set{ QuestTomeWorldGood = value; } }

		public string QuestTomeNPCGood;
		[CommandProperty( AccessLevel.GameMaster )]
		public string QuestTome_NPCGood { get{ return QuestTomeNPCGood; } set{ QuestTomeNPCGood = value; } }

		public string QuestTomeStoryEvil;
		[CommandProperty( AccessLevel.GameMaster )]
		public string QuestTome_StoryEvil { get{ return QuestTomeStoryEvil; } set{ QuestTomeStoryEvil = value; } }

		public string QuestTomeLocateEvil;
		[CommandProperty( AccessLevel.GameMaster )]
		public string QuestTome_LocateEvil { get{ return QuestTomeLocateEvil; } set{ QuestTomeLocateEvil = value; } }

		public Land QuestTomeWorldEvil;
		[CommandProperty( AccessLevel.GameMaster )]
		public Land QuestTome_WorldEvil { get{ return QuestTomeWorldEvil; } set{ QuestTomeWorldEvil = value; } }

		public string QuestTomeNPCEvil;
		[CommandProperty( AccessLevel.GameMaster )]
		public string QuestTome_NPCEvil { get{ return QuestTomeNPCEvil; } set{ QuestTomeNPCEvil = value; } }

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public string QuestTomeCitizen;
		[CommandProperty( AccessLevel.GameMaster )]
		public string QuestTome_Citizen { get{ return QuestTomeCitizen; } set{ QuestTomeCitizen = value; } }

		public int QuestTomeGoals;
		[CommandProperty(AccessLevel.Owner)]
		public int QuestTome_Goals { get { return QuestTomeGoals; } set { QuestTomeGoals = value; InvalidateProperties(); } }

		public string QuestTomeDungeon;
		[CommandProperty( AccessLevel.GameMaster )]
		public string QuestTome_Dungeon { get{ return QuestTomeDungeon; } set{ QuestTomeDungeon = value; } }

		public Land QuestTomeLand;
		[CommandProperty( AccessLevel.GameMaster )]
		public Land QuestTome_Land { get{ return QuestTomeLand; } set{ QuestTomeLand = value; } }

		public int QuestTomeType;
		[CommandProperty(AccessLevel.Owner)]
		public int QuestTome_Type { get { return QuestTomeType; } set { QuestTomeType = value; InvalidateProperties(); } }

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public string GoalItem1;
		[CommandProperty(AccessLevel.Owner)]
		public string Goal_Item1 { get { return GoalItem1; } set { GoalItem1 = value; InvalidateProperties(); } }

		public string GoalItem2;
		[CommandProperty(AccessLevel.Owner)]
		public string Goal_Item2 { get { return GoalItem2; } set { GoalItem2 = value; InvalidateProperties(); } }

		public string GoalItem3;
		[CommandProperty(AccessLevel.Owner)]
		public string Goal_Item3 { get { return GoalItem3; } set { GoalItem3 = value; InvalidateProperties(); } }

		public string GoalItem4;
		[CommandProperty(AccessLevel.Owner)]
		public string Goal_Item4 { get { return GoalItem4; } set { GoalItem4 = value; InvalidateProperties(); } }

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public string VillainCategory;
		[CommandProperty(AccessLevel.Owner)]
		public string Villain_Category { get { return VillainCategory; } set { VillainCategory = value; InvalidateProperties(); } }

		public string VillainType;
		[CommandProperty(AccessLevel.Owner)]
		public string Villain_Type { get { return VillainType; } set { VillainType = value; InvalidateProperties(); } }

		public string VillainName;
		[CommandProperty(AccessLevel.Owner)]
		public string Villain_Name { get { return VillainName; } set { VillainName = value; InvalidateProperties(); } }

		public string VillainTitle;
		[CommandProperty(AccessLevel.Owner)]
		public string Villain_Title { get { return VillainTitle; } set { VillainTitle = value; InvalidateProperties(); } }

		public int VillainBody;
		[CommandProperty(AccessLevel.Owner)]
		public int Villain_Body { get { return VillainBody; } set { VillainBody = value; InvalidateProperties(); } }

		public int VillainHue;
		[CommandProperty(AccessLevel.Owner)]
		public int Villain_Hue { get { return VillainHue; } set { VillainHue = value; InvalidateProperties(); } }

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[Constructable]
		public QuestTome() : base( 0x1A97 )
		{
			Name = "lost journal";
			Weight = 1.0;
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			if ( QuestTomeOwner != null ){ list.Add( 1049644, "Belongs to " + QuestTomeOwner.Name + "" ); }
        }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1060640 ); // The item must be in your backpack to use it.
			}
			else if ( QuestTomeOwner != from )
			{
				from.SendMessage( "This book does not belong and it crumbles to dust!" );
				bool remove = true;
				foreach ( Account a in Accounts.GetAccounts() )
				{
					if (a == null)
						break;

					int index = 0;

					for (int i = 0; i < a.Length; ++i)
					{
						Mobile m = a[i];

						if (m == null)
							continue;

						if ( m == QuestTomeOwner )
						{
							m.AddToBackpack( this );
							remove = false;
						}

						++index;
					}
				}
				if ( remove )
				{
					this.Delete();
				}
			}
			else if ( QuestTomeGoals > 2 && from.Region.Name == QuestTomeDungeon && QuestTomeCitizen != "" )
			{
				QuestTomeCitizen = "";
				QuestTomeLand = Land.None;
				QuestTomeType = 0;

				Type mobType = ScriptCompiler.FindTypeByName( VillainType );
				Mobile mob = (Mobile)Activator.CreateInstance( mobType );
				BaseCreature monster = (BaseCreature)mob;

				SummonPrison.SetDifficultyForMonster( monster );

				Map map = from.Map;

				bool validLocation = false;
				Point3D loc = from.Location;

				for ( int j = 0; !validLocation && j < 10; ++j )
				{
					int x = from.X + Utility.Random( 3 ) - 1;
					int y = from.Y + Utility.Random( 3 ) - 1;
					int z = map.GetAverageZ( x, y );

					if ( validLocation = map.CanFit( x, y, from.Z, 16, false, false ) )
						loc = new Point3D( x, y, from.Z );
					else if ( validLocation = map.CanFit( x, y, z, 16, false, false ) )
						loc = new Point3D( x, y, z );
				}

				monster.NameHue = 0x22;
				monster.Hue = VillainHue;
				if ( VillainBody > 0 ){ monster.Body = VillainBody; }
				monster.Title = VillainTitle;
				monster.Name = VillainName;
				monster.MoveToWorld( loc, map );
				monster.Combatant = from;
				monster.Fame = 0;
				monster.Karma = 0;
				Effects.SendLocationParticles( EffectItem.Create( monster.Location, monster.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				monster.PlaySound( 0x1FE );
			}
			else
			{
				from.CloseGump( typeof( QuestTomeGump ) );
				from.SendGump( new QuestTomeGump( this, from, 0 ) );
			}
		}

		public QuestTome(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)2);

			writer.Write( DeliverMap );
			writer.Write( DeliverX );
			writer.Write( DeliverY );

			writer.Write( (Mobile)QuestTomeOwner );
			writer.Write( QuestTomeStoryGood );
			writer.Write( QuestTomeLocateGood );
			writer.Write( (int)QuestTomeWorldGood );
			writer.Write( QuestTomeNPCGood );
			writer.Write( QuestTomeStoryEvil );
			writer.Write( QuestTomeLocateEvil );
			writer.Write( (int)QuestTomeWorldEvil );
			writer.Write( QuestTomeNPCEvil );
			writer.Write( QuestTomeCitizen );
			writer.Write( QuestTomeGoals );
			writer.Write( QuestTomeDungeon );
			writer.Write( (int)QuestTomeLand );
			writer.Write( QuestTomeType );
			writer.Write( GoalItem1 );
			writer.Write( GoalItem2 );
			writer.Write( GoalItem3 );
			writer.Write( GoalItem4 );
			writer.Write( VillainCategory );
			writer.Write( VillainType );
			writer.Write( VillainName );
			writer.Write( VillainTitle );
			writer.Write( VillainBody );
			writer.Write( VillainHue );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
				{
					DeliverMap = reader.ReadMap();
					DeliverX = reader.ReadInt();
					DeliverY = reader.ReadInt();
					break;
				}
			}

			QuestTomeOwner = reader.ReadMobile();
            QuestTomeStoryGood = reader.ReadString();
            QuestTomeLocateGood = reader.ReadString();

			if ( version < 1 )
				QuestTomeWorldGood = Server.Lands.LandRef( reader.ReadString() );
			else
				QuestTomeWorldGood = (Land)(reader.ReadInt());

            QuestTomeNPCGood = reader.ReadString();
            QuestTomeStoryEvil = reader.ReadString();
            QuestTomeLocateEvil = reader.ReadString();

			if ( version < 1 )
				QuestTomeWorldEvil = Server.Lands.LandRef( reader.ReadString() );
			else
				QuestTomeWorldEvil = (Land)(reader.ReadInt());

            QuestTomeNPCEvil = reader.ReadString();
            QuestTomeCitizen = reader.ReadString();
            QuestTomeGoals = reader.ReadInt();
            QuestTomeDungeon = reader.ReadString();

			if ( version < 1 )
				QuestTomeLand = Server.Lands.LandRef( reader.ReadString() );
			else
				QuestTomeLand = (Land)(reader.ReadInt());

            QuestTomeType = reader.ReadInt();
            GoalItem1 = reader.ReadString();
            GoalItem2 = reader.ReadString();
            GoalItem3 = reader.ReadString();
            GoalItem4 = reader.ReadString();
			VillainCategory = reader.ReadString();
			VillainType = reader.ReadString();
			VillainName = reader.ReadString();
			VillainTitle = reader.ReadString();
			VillainBody = reader.ReadInt();
			VillainHue = reader.ReadInt();
		}

		private class QuestTomeGump : Gump
		{
			private QuestTome m_Book;
			private Map m_Map;
			private int m_X;
			private int m_Y;

			public QuestTomeGump( QuestTome book, Mobile from, int page ) : base( 50, 50 )
			{
				m_Book = book;

				from.SendSound( 0x55 );

				m_Map = book.DeliverMap;
				m_X = book.DeliverX;
				m_Y = book.DeliverY;

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				string color = "#c6c67b";
				string story = m_Book.QuestTomeStoryGood;
				string locat = m_Book.QuestTomeLocateGood;
				string world = Server.Lands.LandName( m_Book.QuestTomeWorldGood );
				string names = m_Book.QuestTomeNPCGood;
						
				if ( ((PlayerMobile)from).KarmaLocked ) // THEY ARE ON AN EVIL PATH
				{
					color = "#cfa495";
					story = m_Book.QuestTomeStoryEvil;
					locat = m_Book.QuestTomeLocateEvil;
					world = Server.Lands.LandName( m_Book.QuestTomeWorldEvil );
					names = m_Book.QuestTomeNPCEvil;
				}

				AddImage(0, 0, 7032, Server.Misc.PlayerSettings.GetGumpHue( from ));
				AddHtml( 12, 12, 665, 20, @"<BODY><BASEFONT Color=" + color + ">" + m_Book.Name + "</BASEFONT></BODY>", (bool)false, (bool)false);

				string dead = m_Book.Name; if ( dead.Contains("Journal of ") ){ dead = dead.Replace("Journal of ", ""); }
				if ( story.Contains("DDDDD") ){ story = story.Replace("DDDDD", dead); }

				if ( page > 0 )
				{
					AddButton(864, 9, 4017, 4017, 2, GumpButtonType.Reply, 0);
					AddHtml( 12, 43, 878, 548, @"<BODY><BASEFONT Color=" + color + ">There are many times when adventurers are given a grand quest to obtain a magical item by slaying a powerful creature and thus using the item for good or evil. You have found the journal of one of these adventurers. What fate became of them, you will never know. Did they lose their journal? Did they perish in their search for " + m_Book.GoalItem4 + "?<br><br>Now you possess the journal and you can pursue this quest as it is yours alone. The quest has two forks that you may go down. If your karma is locked, the goal will lead you down the vile path of " + m_Book.QuestTomeNPCEvil + ". Otherwise, your quest will service good for " + m_Book.QuestTomeNPCGood + ". You may only have a single journal quest at any one time. If you find another journal, and choose to take it while you currently have a journal, then you will get a new journal with the same unfinished quest you had before.<br><br>To defeat " + m_Book.VillainName + " " + m_Book.VillainTitle + " and claim " + m_Book.GoalItem4 + ", you will have to find 3 unique items to aid you. You have no idea where these items are, so you will have to speak to citizens (orange names) to see if they have heard rumors that can help you. If a citizen does not initially mention anything about your quest, you will have to seek out another. When you finally get a clue, a small tune will play and your journal will be updated with that rumor they gave you. It could be true or it could be false. You won’t know until you pursue it. Sometimes the item may be in a chest or bag on a pedestal in a dungeon, or held by one of the more powerful creatures within that dungeon.<br><br>Once you collect the required relics, you must then figure out where " + m_Book.VillainName + " is. Again, talking to citizens may reveal a hint. Once you learn where " + m_Book.VillainName + " is, make haste to that location and face them in battle. Once you enter the area, find a strategic place you wish to combat them and then open the journal to call them forth to face you. The battle will surely be harsh so you best be prepared. Be sure to slay them so you can take " + m_Book.GoalItem4 + " from them. Making them vanish by other means will rob you of your goal, as would leaving the area they are in. If they do manage to escape, you will have to seek out rumors again to determine where " + m_Book.VillainName + " has fled to.<br><br>Slaying " + m_Book.VillainName + " will reveal an abundance of wealth they have taken from other adventurers that failed to be victorious. Feel free to take this treasure for yourself, as " + m_Book.VillainName + " " + m_Book.VillainTitle + " will no longer need it. Once you have acquired " + m_Book.GoalItem4 + ", seek out " + m_Book.QuestTomeNPCGood + " or " + m_Book.QuestTomeNPCEvil + " and hand them the journal. Your morality and fame will be affected by your choice of ethics and you will be rewarded with an item of your choosing. When you select your reward, the item will appear in your pack. Each item will appear with a number of points you can spend to enhance your item. This allows you to tailor the item to suit your style. To begin, single click the items and select 'Enchant'. A menu will appear that you can choose which attributes you want the item to have. Be careful, as you cannot change an attribute once you select it.</BASEFONT></BODY>", (bool)false, (bool)false);
				}
				else
				{
					AddButton(864, 9, 4017, 4017, 0, GumpButtonType.Reply, 0);
					AddButton(792, 9, 3610, 3610, 1, GumpButtonType.Reply, 0);

					if ( Sextants.HasSextant( from ) )
						AddButton(756, 12, 10461, 10461, 3, GumpButtonType.Reply, 0);

					AddHtml( 12, 46, 346, 20, @"<BODY><BASEFONT Color=" + color + ">Quest for " + from.Name + "</BASEFONT></BODY>", (bool)false, (bool)false);

					if ( m_Book.QuestTomeCitizen != "" ){ story = GetRumor( m_Book, false ) + "<br><br>" + story; }

					AddHtml( 12, 82, 878, 358, @"<BODY><BASEFONT Color=" + color + ">" + story + "</BASEFONT></BODY>", (bool)false, (bool)false);

					TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

					if ( m_Book.QuestTomeGoals < 4 )
					{
						AddHtml( 55, 461, 346, 20, @"<BODY><BASEFONT Color=" + color + ">" + cultInfo.ToTitleCase( m_Book.GoalItem1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false);
						if ( m_Book.QuestTomeGoals > 0 ){ AddItem(0, 448, 20413); }
						AddHtml( 55, 513, 346, 20, @"<BODY><BASEFONT Color=" + color + ">" + cultInfo.ToTitleCase( m_Book.GoalItem2 ) + "</BASEFONT></BODY>", (bool)false, (bool)false);
						if ( m_Book.QuestTomeGoals > 1 ){ AddItem(0, 500, 20413); }
						AddHtml( 55, 564, 346, 20, @"<BODY><BASEFONT Color=" + color + ">" + cultInfo.ToTitleCase( m_Book.GoalItem3 ) + "</BASEFONT></BODY>", (bool)false, (bool)false);
						if ( m_Book.QuestTomeGoals > 2 ){ AddItem(0, 551, 20413); }
					}
					else
					{
						AddHtml( 55, 513, 346, 20, @"<BODY><BASEFONT Color=" + color + ">" + cultInfo.ToTitleCase( m_Book.GoalItem4 ) + "</BASEFONT></BODY>", (bool)false, (bool)false);
						AddItem(0, 500, 20413);
					}
				}
			}

			public override void OnResponse( NetState state, RelayInfo info ) 
			{
				Mobile from = state.Mobile; 
				from.CloseGump( typeof( Sextants.MapGump ) );

				if ( info.ButtonID == 1 ){ from.SendGump( new QuestTomeGump( m_Book, from, 1 ) ); }
				else if ( info.ButtonID == 2 ){ from.SendGump( new QuestTomeGump( m_Book, from, 0 ) ); }
				else if ( info.ButtonID == 3 )
				{
					from.SendGump( new QuestTomeGump( m_Book, from, 0 ) );
					from.SendGump( new Sextants.MapGump( from, m_Map, m_X, m_Y, null ) );
				}

				from.SendSound( 0x55 );
			}
		}

		public static string TellRumor( PlayerMobile player, Citizens citizen )
		{
			string rumor = "";

			if ( citizen.CanTellRumor() )
			{
				QuestTome book = player.Backpack.FindItemByType( typeof ( QuestTome ) ) as QuestTome;
				if ( book != null && book.QuestTomeOwner == player )
				{
					if ( Utility.RandomMinMax( 1, 10 ) > 1 ){ citizen.MarkToldRumor(); }

					if ( citizen.CanTellRumor() && book.QuestTomeCitizen == "" && book.QuestTomeGoals < 4 )
					{
						citizen.MarkToldRumor();
						SetRumor( citizen, player, book );
						rumor = GetRumor( book, true );
					}
				}
			}

			return rumor;
		}

		public static string GetRumor( QuestTome book, bool talk )
		{
			int goal = book.QuestTomeType;
			string locate = "held by a powerful creature";
			if ( goal == 2 ){ locate = "lost somewhere"; }
			if ( book.QuestTomeGoals == 3 ){ locate = "found"; goal = 3; }

			string world = Server.Lands.LandName( book.QuestTomeLand );
			string dungeon = book.QuestTomeDungeon;
			string from = book.QuestTomeCitizen;
			string item = book.GoalItem1;
				if ( book.QuestTomeGoals == 1 ){ item = book.GoalItem2; }
				else if ( book.QuestTomeGoals == 2 ){ item = book.GoalItem3; }
				else if ( book.QuestTomeGoals == 3 ){ item = book.VillainName + " " + book.VillainTitle; }

			if ( talk )
			{
				string who = "I heard";
				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0:	who = "I heard";																								break;
					case 1:	who = "I learned";																								break;
					case 2:	who = "I found out";																							break;
					case 3:	who = "The " + RandomThings.GetRandomJob() + " in " + RandomThings.GetRandomCity() + " told me";				break;
					case 4:	who = "I overheard some " + RandomThings.GetRandomJob() + " say";												break;
					case 5:	who = "My friend told me";																						break;
				}
				return who + " that " + item + " may be " + locate + " within " + dungeon + " in " + world + ".";
			}

			if ( world != "" ){ return "" + from + " has told you that " + item + " may be " + locate + " within " + dungeon + " in " + world + "."; }

			return "";
		}

		public static void SetRumor( Mobile m, PlayerMobile player, QuestTome book )
		{
			book.QuestTomeType = Utility.RandomMinMax( 1, 2 );

			if ( book.QuestTomeGoals > 2 ){ book.QuestTomeType = 3; }

			var options = new List<Land>
			{
				Land.Sosaria,
				Land.Lodoria,
				Land.Serpent,
				Land.Sosaria,
				Land.Lodoria,
				Land.Serpent,
				Land.UmberVeil,
				Land.Ambrosia,
				Land.IslesDread,
				Land.Savaged,
				Land.Kuldar,
			};
			Land searchLocation = PlayerSettings.GetRandomDiscoveredLand(player, options, null);

			string dungeon = "Dungeon Doom";

			int aCount = 0;

			ArrayList targets = new ArrayList();

			if ( book.QuestTomeType == 1 )
			{
				foreach ( Mobile target in World.Mobiles.Values )
				if ( target.Region is DungeonRegion && target.Fame >= 18000 && !( target is Exodus || target is CodexGargoyleA || target is CodexGargoyleB || target is Syth ) )
				{
					if ( target.Land == searchLocation )
					{
						targets.Add( target );
						aCount++;
					}
				}
			}
			else
			{
				foreach ( Item target in World.Items.Values )
				if ( target is SearchBase || target is StealBase )
				{
					if ( target.Land == searchLocation )
					{
						targets.Add( target );
						aCount++;
					}
				}
			}

			aCount = Utility.RandomMinMax( 1, aCount );

			int xCount = 0;
			for ( int i = 0; i < targets.Count; ++i )
			{
				xCount++;

				if ( xCount == aCount )
				{
					if ( book.QuestTomeType == 1 )
					{
						Mobile finding = ( Mobile )targets[ i ];
						dungeon = Server.Misc.Worlds.GetRegionName( finding.Map, finding.Location );
					}
					else
					{
						Item finding = ( Item )targets[ i ];
						dungeon = Server.Misc.Worlds.GetRegionName( finding.Map, finding.Location );
					}
				}
			}

			book.QuestTomeLand = searchLocation;
			book.QuestTomeDungeon = dungeon;
			book.QuestTomeCitizen = "" + m.Name + " " + m.Title + "";
		}

		public static bool FoundItem( Mobile player, int type, MajorItemOnCorpse chest )
		{
			QuestTome book = player.Backpack.FindItemByType( typeof ( QuestTome ) ) as QuestTome;
			if (book == null) return false;

			if ( type == book.QuestTomeType && book.QuestTomeDungeon == Server.Misc.Worlds.GetRegionName( player.Map, player.Location ) && book.QuestTomeOwner == player && book.QuestTomeGoals < 3 )
			{
				if ( Utility.RandomMinMax( 1, 3 ) != 1 )
				{
					string relic = book.GoalItem1;
						if ( book.QuestTomeGoals == 1 ){ relic = book.GoalItem2; }
						else if ( book.QuestTomeGoals == 2 ){ relic = book.GoalItem3; }

					player.LocalOverheadMessage(MessageType.Emote, 1150, true, "You found " + relic + ".");
					player.SendSound( 0x5B4 );
					book.QuestTomeCitizen = "";
					book.QuestTomeDungeon = "";
					book.QuestTomeLand = Land.None;
					book.QuestTomeType = 0;
					book.QuestTomeGoals++;

					return true;
				}
				else
				{
					player.LocalOverheadMessage(MessageType.Emote, 1150, true, book.QuestTomeCitizen + " was either wrong or they lied.");
					player.SendSound( 0x5B3 );
					book.QuestTomeCitizen = "";
					book.QuestTomeDungeon = "";
					book.QuestTomeLand = Land.None;
					book.QuestTomeType = 0;

					return false;
				}
			}
			else if ( chest != null && book.VillainName == chest.VillainName && book.VillainTitle == chest.VillainTitle && book.QuestTomeOwner == player && book.QuestTomeGoals >= 3 )
			{
				player.AddToBackpack(new HoardMinionFamiliarItem());
				ApproachObsidian.TitanRiches( player );
				CustomEventSink.InvokeCombatQuestCompleted(player, 10000);
				player.LocalOverheadMessage(MessageType.Emote, 1150, true, "You found " + book.GoalItem4 + ".");
				book.QuestTomeGoals++;

				return true;
			}
			return false;
		}

		public static void BossEscaped( Mobile from, string region )
		{
			if ( from.Backpack.FindItemByType( typeof ( QuestTome ) ) != null )
			{
				Item item = from.Backpack.FindItemByType( typeof ( QuestTome ) );
				QuestTome book = (QuestTome)item;

				if ( book.QuestTomeGoals > 2 && book.QuestTomeDungeon == region && book.QuestTomeOwner == from )
				{
					ArrayList targets = new ArrayList();
					foreach ( Mobile creature in World.Mobiles.Values )
					{
						if ( creature.Name == book.VillainName && creature.Title == book.VillainTitle )
						{
							targets.Add( creature );
						}
					}
					for ( int i = 0; i < targets.Count; ++i )
					{
						Mobile creature = ( Mobile )targets[ i ];

						Effects.SendLocationParticles( EffectItem.Create( creature.Location, creature.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
						creature.PlaySound( 0x1FE );

						creature.Delete();
					}
				}
			}
		}
	}
}