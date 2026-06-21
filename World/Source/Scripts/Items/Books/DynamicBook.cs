using System;
using Server.Gumps;
using Server.Misc;
using Server.Network;
using System.Globalization;

namespace Server.Items
{
	public class DynamicBook : Item, IShowableBook
	{
		[Constructable]
		public DynamicBook( ) : base( 0x1C11 )
		{
			Weight = 1.0;

			if ( BookTitle == "" || BookTitle == null )
			{
				ItemID = RandomThings.GetRandomBookItemID();
				Hue = Utility.RandomColor(0);
				SetBookCover( 0, this );
				BookTitle = Server.Misc.RandomThings.GetBookTitle();
				Name = BookTitle;
				BookAuthor = Server.Misc.RandomThings.GetRandomAuthor();
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( "Written by " + BookAuthor );
		}

		public class DynamicSythGump : Gump
		{
			public DynamicSythGump( Mobile from, DynamicBook book ): base( 100, 100 )
			{
				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;
				this.AddPage(0);

				AddImage(0, 0, 30521);
				AddImage(51, 41, 11428);
				AddImage(52, 438, 11426);
				AddHtml( 275, 45, 445, 20, @"<BODY><BASEFONT Color=#FF0000>" + book.BookTitle + " by " + book.BookAuthor + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 275, 84, 445, 521, @"<BODY><BASEFONT Color=#00FF06>" + book.BookText + "</BASEFONT></BODY>", (bool)false, (bool)true);
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				Mobile from = state.Mobile;
				from.SendSound( 0x54D );
			}
		}

		public class DynamicJediGump : Gump
		{
			public DynamicJediGump( Mobile from, DynamicBook book ): base( 100, 100 )
			{
				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;
				this.AddPage(0);

				AddImage(0, 0, 30521);
				AddImage(51, 41, 11435);
				AddImage(52, 438, 11433);
				AddHtml( 275, 45, 445, 20, @"<BODY><BASEFONT Color=#308EB3>" + book.BookTitle + " by " + book.BookAuthor + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 275, 84, 445, 521, @"<BODY><BASEFONT Color=#00FF06>" + book.BookText + "</BASEFONT></BODY>", (bool)false, (bool)true);
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				Mobile from = state.Mobile;
				from.SendSound( 0x54D );
			}
		}

		public class DynamicBookGump : Gump
		{
			public DynamicBookGump( Mobile from, DynamicBook book ): base( 100, 100 )
			{
				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				string color = "#d6c382";

				this.AddPage(0);

				AddImage(0, 0, 7005, book.Hue-1);
				AddImage(0, 0, 7006);
				AddImage(0, 0, 7024, 2736);
				AddImage(362, 55, 1262, 2736);
				AddImage(408, 94, book.BookCover, 2736);
				AddHtml( 73, 49, 251, 20, @"<BODY><BASEFONT Color=" + color + ">" + book.BookTitle + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 73, 76, 251, 20, @"<BODY><BASEFONT Color=" + color + ">by " + book.BookAuthor + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 73, 105, 251, 290, @"<BODY><BASEFONT Color=" + color + ">" + book.BookText + "</BASEFONT></BODY>", (bool)false, (bool)true);
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				Mobile from = state.Mobile;
				from.SendSound( 0x55 );
			}
		}

		public static void SetBookCover( int cover, DynamicBook book )
		{
			if ( cover == 0 ){ cover = Utility.RandomMinMax( 1, 80 ); }

			switch( cover )
			{
				case 1: book.BookCover = 0x4F1; break; // Man Fighting Skeleton
				case 2: book.BookCover = 0x4F2; break; // Dungeon Door
				case 3: book.BookCover = 0x4F3; break; // Castle
				case 4: book.BookCover = 0x4F4; break; // Old Man
				case 5: book.BookCover = 0x4F5; break; // Sword and Shield
				case 6: book.BookCover = 0x4F6; break; // Lion with Sword
				case 7: book.BookCover = 0x4F7; break; // Chalice
				case 8: book.BookCover = 0x4F8; break; // Two Women
				case 9: book.BookCover = 0x4F9; break; // Dragon
				case 10: book.BookCover = 0x4FA; break; // Dragon
				case 11: book.BookCover = 0x4FB; break; // Dragon
				case 12: book.BookCover = 0x4FC; break; // Wizard Hat
				case 13: book.BookCover = 0x4FD; break; // Skeleton Dancing
				case 14: book.BookCover = 0x4FE; break; // Skull Crown
				case 15: book.BookCover = 0x4FF; break; // Devil Pitchfork
				case 16: book.BookCover = 0x500; break; // Sun Symbol
				case 17: book.BookCover = 0x501; break; // Griffon
				case 18: book.BookCover = 0x502; break; // Unicorn
				case 19: book.BookCover = 0x503; break; // Mermaid
				case 20: book.BookCover = 0x504; break; // Merman
				case 21: book.BookCover = 0x505; break; // Crown
				case 22: book.BookCover = 0x506; break; // Demon
				case 23: book.BookCover = 0x507; break; // Hell
				case 24: book.BookCover = 0x514; break; // Arch Devil
				case 25: book.BookCover = 0x515; break; // Grim Reaper
				case 26: book.BookCover = 0x516; break; // Castle
				case 27: book.BookCover = 0x517; break; // Tombstone
				case 28: book.BookCover = 0x518; break; // Dragon Crest
				case 29: book.BookCover = 0x519; break; // Cross
				case 30: book.BookCover = 0x51A; break; // Village
				case 31: book.BookCover = 0x51B; break; // Knight
				case 32: book.BookCover = 0x51C; break; // Alchemy
				case 33: book.BookCover = 0x51D; break; // Symbol Man Magic Dragon
				case 34: book.BookCover = 0x51E; break; // Throne
				case 35: book.BookCover = 0x51F; break; // Ship
				case 36: book.BookCover = 0x520; break; // Ship with Fish
				case 37: book.BookCover = 0x579; break; // Bard
				case 38: book.BookCover = 0x57A; break; // Thief
				case 39: book.BookCover = 0x57B; break; // Witches
				case 40: book.BookCover = 0x57C; break; // Ship
				case 41: book.BookCover = 0x57D; break; // Village Map
				case 42: book.BookCover = 0x57E; break; // World Map
				case 43: book.BookCover = 0x57F; break; // Dungeon Map
				case 44: book.BookCover = 0x580; break; // Devil with 2 Servants
				case 45: book.BookCover = 0x581; break; // Druid
				case 46: book.BookCover = 0x582; break; // Star Magic Symbol
				case 47: book.BookCover = 0x583; break; // Giant
				case 48: book.BookCover = 0x584; break; // Harpy
				case 49: book.BookCover = 0x585; break; // Minotaur
				case 50: book.BookCover = 0x586; break; // Cloud Giant
				case 51: book.BookCover = 0x960; break; // Skeleton Warrior
				case 52: book.BookCover = 0x961; break; // Lich
				case 53: book.BookCover = 0x962; break; // Mind Flayer
				case 54: book.BookCover = 0x963; break; // Lizard
				case 55: book.BookCover = 0x521; break; // Mondain
				case 56: book.BookCover = 0x522; break; // Minax
				case 57: book.BookCover = 0x523; break; // Serpent Pillar
				case 58: book.BookCover = 0x524; break; // Gem of Immortality
				case 59: book.BookCover = 0x525; break; // Wizard Den
				case 60: book.BookCover = 0x526; break; // Guard
				case 61: book.BookCover = 0x527; break; // Shadowlords
				case 62: book.BookCover = 0x528; break; // Gargoyle
				case 63: book.BookCover = 0x529; break; // Moongate
				case 64: book.BookCover = 0x52A; break; // Elf
				case 65: book.BookCover = 0x52B; break; // Shipwreck
				case 66: book.BookCover = 0x52C; break; // Black Demon
				case 67: book.BookCover = 0x52D; break; // Exodus
				case 68: book.BookCover = 0x52E; break; // Sea Serpent
				case 69: book.BookCover = 0x530; break; // Hydra
				case 70: book.BookCover = 0x531; break; // Beholder
				case 71: book.BookCover = 0x532; break; // Flying Castle
				case 72: book.BookCover = 0x533; break; // Serpent
				case 73: book.BookCover = 0x534; break; // Ogre
				case 74: book.BookCover = 0x535; break; // Skeleton Graveyard
				case 75: book.BookCover = 0x536; break; // Shrine
				case 76: book.BookCover = 0x537; break; // Volcano
				case 77: book.BookCover = 0x538; break; // Castle
				case 78: book.BookCover = 0x539; break; // Dark Knight
				case 79: book.BookCover = 0x53A; break; // Skull Ring
				case 80: book.BookCover = 0x53B; break; // Serpents of Balance
			}
		}

		public void ShowBookGump( Mobile e )
		{
			if ( ItemID == 0x4CDF )
			{
				e.CloseGump( typeof( DynamicBookGump ) );
				e.CloseGump( typeof( DynamicSythGump ) );
				e.CloseGump( typeof( DynamicJediGump ) );
				e.SendGump( new DynamicSythGump( e, this ) );
				e.SendSound( 0x54D );
			}
			else if ( ItemID == 0x543C )
			{
				e.CloseGump( typeof( DynamicBookGump ) );
				e.CloseGump( typeof( DynamicSythGump ) );
				e.CloseGump( typeof( DynamicJediGump ) );
				e.SendGump( new DynamicJediGump( e, this ) );
				e.SendSound( 0x54D );
			}
			else
			{
				e.CloseGump( typeof( DynamicSythGump ) );
				e.CloseGump( typeof( DynamicBookGump ) );
				e.CloseGump( typeof( DynamicJediGump ) );
				e.SendGump( new DynamicBookGump( e, this ) );
				e.SendSound( 0x55 );
			}
		}

		public override void OnDoubleClick( Mobile e )
		{
			if (!e.InRange( this.GetWorldLocation(), 5 ) || !e.CanSee( this ) || !e.InLOS( this ) )
			{
				e.SendMessage( "That is too far away to read." );
				return;
			}

			ShowBookGump( e );
			Server.Gumps.MyLibrary.readBook ( this, e );
		}

		public DynamicBook(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
			writer.Write( BookCover );
			writer.Write( BookTitle );
			writer.Write( BookAuthor );
			writer.Write( BookText );
			writer.Write( BookRegion );
			writer.Write( BookMap );
			writer.Write( BookWorld );
			writer.Write( BookItem );
			writer.Write( BookTrue );
			writer.Write( BookPower );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			BookCover = reader.ReadInt();
			BookTitle = reader.ReadString();
			BookAuthor = reader.ReadString();
			BookText = reader.ReadString();
			BookRegion = reader.ReadString();
			BookMap = reader.ReadMap();
			BookWorld = reader.ReadString();
			BookItem = reader.ReadString();
			BookTrue = reader.ReadInt();
			BookPower = reader.ReadInt();
		}

		public int BookCover;
		[CommandProperty(AccessLevel.Owner)]
		public int Book_Cover { get { return BookCover; } set { BookCover = value; InvalidateProperties(); } }

		public string BookTitle;
		[CommandProperty(AccessLevel.Owner)]
		public string Book_Title { get { return BookTitle; } set { BookTitle = value; InvalidateProperties(); } }

		public string BookAuthor;
		[CommandProperty(AccessLevel.Owner)]
		public string Book_Author { get { return BookAuthor; } set { BookAuthor = value; InvalidateProperties(); } }

		public string BookText;
		[CommandProperty(AccessLevel.Owner)]
		public string Book_Text { get { return BookText; } set { BookText = value; InvalidateProperties(); } }

		public string BookRegion;
		[CommandProperty(AccessLevel.Owner)]
		public string Book_Region { get { return BookRegion; } set { BookRegion = value; InvalidateProperties(); } }

		public Map BookMap;
		[CommandProperty(AccessLevel.Owner)]
		public Map Book_Map { get { return BookMap; } set { BookMap = value; InvalidateProperties(); } }

		public string BookWorld;
		[CommandProperty(AccessLevel.Owner)]
		public string Book_World { get { return BookWorld; } set { BookWorld = value; InvalidateProperties(); } }

		public string BookItem;
		[CommandProperty(AccessLevel.Owner)]
		public string Book_Item { get { return BookItem; } set { BookItem = value; InvalidateProperties(); } }

		public int BookTrue;
		[CommandProperty(AccessLevel.Owner)]
		public int Book_True { get { return BookTrue; } set { BookTrue = value; InvalidateProperties(); } }

		public int BookPower;
		[CommandProperty(AccessLevel.Owner)]
		public int Book_Power { get { return BookPower; } set { BookPower = value; InvalidateProperties(); } }

		public static string BasicHelp()
		{
			string text = "BASICS OF THE GAME<BR><BR>Playing in the " + MySettings.S_ServerName + " is pretty simple as the interface is quite intuitive. Although the game is over 20 years old, some explanation is in order. After you login and are in the game world, you will see a book open with abilities. Right click on this book to make it close as you will not need it for this game. Almost any window can be closed with a right click. Your character is always in the center of the playing screen. To travel, simply move the mouse over the game world display... then right click and hold. The mouse cursor will always point away from your character, who will move in the indicated direction (for example, if you wish to walk up the screen, hold the cursor above your character). You will continue to head in that direction until you come to an obstruction or release the mouse button. The further away the cursor is from your character, the faster the character will move. Double right clicking will cause your character to move to the exact point where the cursor was... unless disabled in the options.<br><br>PAPERDOLL: Your character paperdoll will be open when you start. If it is not, pressing Alt P will open it for you. Below I will explain what this does. The left side shows boxes for the slot showing what is on your head, then the slot showing what is on your ears, then the slot showing what is on your neck, then the slot showing what is on your finger, lastly the slot showing what is on your wrist. The bottom will show your name and your title. Sometimes it is custom, while mostly it is your best practiced skill along with any fame and/or karma you gained. The right side has various buttons. Pressing the HELP button brings up a simple help menu. The only thing that you should ever use here is when your character is physically stuck in the game world and needs to be teleported out. It will teleport you into a safe area somewhere in the land. The OPTIONS button will bring up your options for the game (discussed later). The LOG OUT button Logs you out of the game. Make sure you are in a safe place. The STATS button will bring up some vital stats about your character (discussed later). The SKILLS button will bring up all of the skill available in the game. Here you manage your skill progression (discussed later). The GUILD button enables you to start your own guild. It will cost money to get started, but you can invite other players and share homes and chat with each other. The PEACE button toggles whether you are ready to fight... or not. Lastly, the STATUS button will bring up the status of your character (discussed later). The center shows your character. Here you can drag and drop clothing, armor, and other equipment worn by your character. Double click the left scroll to see how old your account is. Double click the right scroll to organize a party of other players. This is important if you plan to share the rewards of dungeon delving. Double click the backpack to open your backpack (discussed later).<br><br>MENU BAR: This menu bar can allow you to get access to certain items quicker. It can be disabled in the options. The small triangle will minimize the menu bar. The MAP button will open a mini map of your surrounding area. Pressing it a second time will make the map a bit larger (Alt R does this as well). The PAPERDOLL button will open your paperdoll. The INVENTORY button will open your backpack (discussed later). The JOURNAL button will open your journal, which shows the most recent things you saw or heard. The CHAT button does not work as the chat option is disabled. Type the command [c instead. The HELP button will bring up the help menu that was already discussed. The ? button brings up a very outdated information screen. It is best not to use it.<br><br>BACKPACK: When you double click the backpack on the paperdoll, it will open (Alt I will do that as well). You can only carry a certain amount of weight based off of your strength. If you strength is extremely high, then you are at the mercy at how much the backpack can actually hold. The image on the right shows how you can actually have containers within the backpack to help organize things better. You can drag and drop items between the containers. Sometimes your containers will close when you travel between different worlds. If you close a container, that has other container from within it open, those containers will also close. OPTIONS Pressing the options button will open this window (as will pressing Alt O). You can change many things in the options section. You can control the volume of music and sounds. You can change the fonts and colors of such fonts. You can setup macros to create shortcut keys for commonly used series of commands. You can also filter obscenities. This is also where you can set your pathfinding, war mode, targeting system, and menu bar options. You can choose to offset your interface windows (like containers) when opening. Pay attention to the macro options, as you can learn about some of the pre built shortcut keys... along with learning how to steer ships when you can afford to buy one.<br><br>STATS: There are many aspects of your character, and the stats button will display this for you. You can see what comprises of your abilities and if you have any bonuses to regeneration of statistics like mana, stamina, or hit points. You can see the values of your karma and fame (which can also appear as a title on your paperdoll). Your hunger and thirst will be shown so you can determine if you need to eat or drink (of course, the game will tell you when you are starving or dying of thirst without this statistic). You can tell how fast you can cast spells or apply bandages. If you murdered anybody innocent, you can see that value here. If you use tithing points (people doing knightship), you can see that value here to know if you need to make a donation of gold to a shrine sooner or later.<br><br>SKILLS: Pressing the skills button will open this (as will pressing Alt K). Here you can see the many skills available in the game for your character to become proficient at. You will have a maximum of 1,000 skill points to use. The skill with the blue dots to the left are skills that are activated to use most of the time (meaning, sometimes they are working in the background as well). For these skills, you can click and drag them off of this scroll and it will make a button on your screen that you can click on to activate the skill in the future. To the right of each value is an up arrow that you can change to a down arrow or a lock. You can lock a skill at a certain value so it does not raise or lower any more than that. You can change it to a down arrow as well. This will tell the game that this skill will decrease if another skill raises (and you have used up all 1,000 points). You can see the example on the right. The lower right number of 193. 1 indicates that this character has used that many skill points so far. Some magic items add to your skills and will be reflected here. If you just want to see the skill values (without the addition that magic items provide), click on the 'show real' option on the bottom right. The 'show caps' option will show you the maximum value you can have a skill at. Each skill is allowed to go up to 100 each (without going over 1,000 total). You can find scrolls of power that will allow a skill to go above 100, and this option will show you that. Skills are organized by category and you can even click the 'new group' button to make a group of your own. Then you can drag and drop skills in this 'group' so you can select a particular set of skills you may want to keep your eye on for that character.<br><br>STATUS: The status window shows your character's strength, dexterity, intelligence, hit points, stamina, mana, luck, carried weight, followers, damage, and carried gold. You can also see the maximum value you are allowed for strength, dexterity, and intelligence (always 250). Double clicking this window will switch it from a detailed view to a smaller bar view. You can set your strength, dexterity, and intelligence to raise and lower similar to skills described above... with the arrows on the left of each value. On the very right is your values of defense against physical, fire, cold, poison, and energy. All creatures have these values and some attacks deal damage in all or some of these categories. You will one day want all of these values high (maximum of 70% in each). The rest of the game features can be learned while in the game. As an example, you can learn some more commands from the message of day. You can also visit a sage and buy a scroll that will detail what all of the skills do and how to use them. Many commands you can type in the bottom left of the world view window by typing a '[' symbol (without quotes)... along with the command. For example, '[c' will bring up the chat window. '[status' will bring up the stats window. '[motd' will bring up the message of the day. On the message of the day window, press the ? on the upper right to learn more commands. It will be up to you and explore. Now lets get into some of the common things that you will probably do in the game.<br><br>CHAT: This is a means to communicate within the game world when you are not on the same screen as another player. Type '[c' to begin using the chat system. This also lets you send a message to another player that they can read later on. Keep in mind that this is character specific and not account specific. If you send a message to a character, but the player logs in with a different character, they will not see the message until they log in with 'that character'. This chat feature has many options Internet chat systems have. You can see who is online. You can establish channels. You can even set some privacy levels to ignore others or not be seen at all.<br><br>CITIZENS: Many citizens have a context menu when you single left click them. This brings up a list of services they provide for you. Some may be grayed out as being something they cannot provide 'you' in particular (if they train tailoring for example, and you are already a master at it, it will be grayed out because they cannot teach you any more about it). Many citizens have a 'hire' option. Make sure to explore what you can hire them to do for you. It may come in handy later. Be careful when single left clicking them as you may still be in war mode and accidentally attack them. If they live through your initial attack, run away or risk becoming a murderer. Murders take about 8 hours of real time (while in game) to go away. It will take 40 hours of real time (while in game) to go away if you continue to murder while you are a murderer. ";
			if ( MySettings.S_Bribery >= 1000 )
			{
				text = text + "Another option is to visit the Assassin Guildmaster, where you can hire them for " + MySettings.S_Bribery + " gold to convince the guards to forget about a single murder you may have committed. Assassin Guild members pay only half of that amount, and fugitives are too well known to be forgiven by bribes. If you do not have enough gold in your pack, they will simply take it from your bank box. ";
			}
			text = text + "You will not be allowed in a settlement while being a murderer, unless you perhaps disguise yourself. Criminals on the other hand will lose their criminal status in a few minutes.<br><br>BANKS: Banks are a safe place in which to leave your valuables. You can't carry everything with you and this does nicely until you can afford a home. Also, the world is dangerous. You will want to keep extra equipment in the bank in case you lose your favored set. Bankers are attentive and if you simply go into the building and say 'bank', they will give you access to your bank box. Banks are the only place that will take your non gold currency and exchange it for gold. You will find copper and silver coins out there and you need them converted. Giving them to a banker will do this. You can also put the coins in your bank box and double click them. This will convert them as well. If you need to carry a large amount of gold somewhere, you can convert funds to an official document called a check. To do this, simply say the word 'check' and the amount.<br><br>INNS & TAVERNS: Inns (candle signs) and Taverns (wine/grape signs) are the safe places for adventurers to rest and relax. You cannot cast spells here or attack anyone else. These places are good to negotiate trades, buy food and drink, or simply chat and play the games that they offer (the tavern offers games... not inns). You log out instantly when in these places, otherwise... it will take a few minutes for your character to log out when out and about. Taverns are a good place to hire henchman to adventure with as well. Sometimes there are also bar patrons in taverns that will tell of places to go and rumors they heard.<br><br>PRACTICE: Some settlements have a place for you to practice your weapon skills. Equip a weapon by putting it in your paperdoll hand and then double click a training dummy. You can only train a weapon skill so high on these, but it will get you started. For ranged weapons, use an archery butte to practice with. If you are able to find a thief, they usually have dummies that you can practice pick pocketing with.<br><br>COMBAT: To start a fight with someone (or something), press the peace button on the paperdoll to go into war mode. Then double click a target. You will now be in a fight. Remember to press the button again to go back to peace mode, so you don't accidentally attack anyone. Most weapons need you toe to toe with your opponent, while bows allow you to be a bit of a distance away. You can also attack with followers or spells. Casting a harmful spell on another will start a fight as well. Keep in mind, there are times you may need to run away from a fight. It is better to live and fight another day. Killing citizens or other players will give you murder counts. The exception is when you kill another player that is already a murderer or criminal. You can tell when you highlight them. If they are red, they are a murder. If they are grey, they are a criminal.<br><br>WEAPONS: There are many different weapons in the game. Hovering your cursor over them will show you the statistics of the weapon. You can see the damage it causes and type of damage. You can see how often you can swing the weapon (or shoot). There will be a strength requirement to use the weapon, along with the skill type when using the weapon (swordsmanship, marksmanship, etc). The weapon will indicate if it is a one or two handed weapon. This is important if you want to hold a shield, torch, or lantern. Two handed weapons do not allow for that. Some weapons will have magical properties that will also be listed in this manner. Each weapon has a durability. You item will break if it gets too low so you either need to fix it yourself (usually with blacksmithing or bowcrafting skills) or find a citizen you can hire to fix it for you.<br><br>ARMOR: Armor not only covers the medieval metal armors of knights, but also the minor items like leather gloves and hats. If you look to the right, you can see the wizard's hat has some bonuses to resistances (see the section on Status). Armor has a strength requirement and durability like weapons do. You can buy many pieces of armor that go over many sections of the body. Areas like hands, arms, legs, neck, chest, and head can all have some type of armor worn on it. You can equip them like a weapon or jewelry. Drag and drop the piece of armor on the paperdoll. If it does not equip, you are either not strong enough or you have a piece of armor or clothing in that spot already. The heavier the armor, the less likely you can meditate or sneak around. Leather armor is usually good for doing such things, where even switching to studded leather armor may be too heavy. Any armor showing as 'mage armor' will allow you to meditate or sneak no matter the weight.<br><br>MAGIC GATES: You may come across a magic gate that leads elsewhere. Nothing to do but walk away or go in. They come in different colors of blue, red, or black. Powerful wizards can summon these gates while some necromantic potions can summon black gates. If you learn where these gates go, you will be able to explore new worlds or get around the current world much quicker. Sometimes, slaying a mystical creature will create a gate, but those are very... very rare.<br><br>MAGIC RUNES: Recall runes are small brown stones with a golden ankh symbol on them that one may cast a Mark Spell on. This will mark the current location of rune to that spot. One may use magic to then transport back to that spot. When runes are marked, they will change color depending on the world they go to. You can buy/make runebooks that will allow you to drop a rune stone in for easier use. You are allowed 16 per book. Dropping Recall Spell scrolls onto the book will increase the charges so you can use the book to simply teleport to those locations without knowing magic.<br><br>BOOKS OF MAGIC: There are a few books of magic in the world. The one wizards use is a simple spellbook while necromancers use a necromancer spellbook. Both of these books need spell scrolls scribed in them by dropping the appropriate spell scroll on the book. You can double click a scroll and try to cast from it, but it would simply fade afterwards. Putting it in a book lets you keep using it over and over. The other three books are for knightship, samurai, and ninjas. They are not like spellbooks as they are simply books that allow you to use those special abilities. No matter the book, you can drag and drop icons from them so you have a quicker way to cast/use them. The wizard and necromancer spells require spell components. You can also visit a sage or scribe for even easier ways to use this magic.<br><br>DEATH: You might find yourself falling under a series of unfortunate events... and thus leave the land of the living. Fear not! You can return in a couple of different ways. The first choice is to resurrect with certain penalties to your skills and abilities. This is quick, but the price to your character's progressions will take quite a step backward. You could have a comrade resurrect you by either magic or healing... or you may be lucky enough to have consumed a resurrection potion before your demise. Lastly, you could simply take your soul and search the land for a healer or shrine from where you can get yourself resurrected. For this to occur, a healer will ask for a donation for such services... or a shrine will demand tribute to the gods. You will need to have some gold saved in your bank box or gold tithed in order to take advantage of this. You can get resurrected without the fee or penalties if your total stats are 90 or less, and your skill total is 200 or less.<br><br>FINALLY: This should give you more than enough information to start your adventure in this world. Explore and try out different things and you will discover many options that are available to your character. One of the first pieces of advice would be to seek out a sage as they sell valuable information that can help you learn more about how things function. They have scrolls that contain information on how to blacksmith, tailor, or find resources out in the world by digging or skinning animals. You can learn how to chop wood, steal from dungeons, avoiding traps, and even about the different types of reagents. Figure out how you are going to get food and drink as you will need it for your journey. Good luck in your adventures!";

			return text;
		}

		public static void SetStaticText( DynamicBook book )
		{
			if ( book is TendrinsJournal ){ book.BookText = "Entry 1 - Today is looking to be a good day for Skara Brae. All of the townsfolk are getting ready for the fall celebration. I myself am making sweet rolls for the grand dinner tonight. I better get to work.<br><br>Entry 2 - The celebration did not go well. Mangar was there and he was threatening many of the council members. Apparently there is some legend of a lost artifact in ruins below Skara Brae, and he wants permission to dig. The council is weak, but with the backing of Lord British...Mangar can do very little to force them into his demands. He eventually stormed off to his tower. I will ask the council if they want me to scout out his tower tomorrow. It is in the center of Sosaria, so it will be a far walk...but I think we need to know what he is up to.<br><br>Entry 3 - I am off with the council's blessing. It will be a long journey, but I should be there early tomorrow. I have setup camp and am ready to rest for the night.<br><br>Entry 4 - I am back in Skara Brae, as I traveled all night to get here. Matters are getting worse. Mangar was not in his tower when I got there, but I scaled the walls and searched his study. There were many scrolls and books. From what I could read, he is planning something horrific for us. His plan is to magically transport Skara Brae into the Void. If he were to accomplish this, then we would not be able to get outside help and Mangar can do with Skara Brae as he pleases. I went over this with the council tonight and they sent a messenger to Britain for help.<br><br>Entry 5 - Mangar arrived in Skara Brae this morning and has been in with the council since he got here. I went over to the building to eavesdrop on the conversation. He urged the council to meet his demands to dig. If they do not, he would unleash horrific magic on us and send us into the Void. When one of the council members stated that Lord British's wizards would come find us and return us from the Void...that is when Mangar said, 'No one in Sosaria will ever know we are in the Void'. What does that mean?<br><br>Entry 6 - I returned to Mangar's tower in a hope to put an end to his tyranny. He was not there yet again, and the tower seemed to be unkempt to say the least. Searching through more scrolls, I found out what he meant the other day. His plan is to make it appear Skara Brae lies in ruins. All of Sosaria will think we perished in some type of disaster. I must return and warn the others. <br><br>Entry 7 - I am too late. Skara Brae is gone, and there is nothing but destroyed homes around me. I am sitting on the floor of what was once my home, bleeding out from a knife wound to my back. Distracted by the magical vortex earlier, Mangar crept up behind me and stabbed me. He then teleported away with a sinister laugh. It appears he has won. May the gods help all of those trapped in Skara Brae."; }
			else if ( book is BookGuideToAdventure || book is LoreGuidetoAdventure ){ book.BookText = BasicHelp(); }
			else if ( book is BookBottleCity ){ book.BookText = "It started with just a few. An experiment conjured by the Black Knight's mind. Vordo, one of his highest mages, worked for years to perfect the spell that eventually swallowed the small island of Kuldar near the Serpent Island. The gargoyles feared the Black Knight's power as they believed the island to be destroyed. The truth was something far more sinister. Within the mystical bottle the island sits floating in the water that houses the life that was brought with it. The Black Knight exiled some of his prisoners to the bottle to live out their remaining years. Centuries passed and those prisoners farmed land, built a city, had children, and lived in prosperity. Vordo decided that he wanted to rule this land as the Black Knight rules his. He told the Black Knight that the bottle was destroyed in an accident, where the Black Knight cared very little since he was onto other matters of interest. Vordo dropped his castle into the bottle, where it crashed down next to the city within. He magically entered the bottle and summoned a moongate to leave one day if he wished. He ruled with an iron fist for almost a decade until the citizens rose up and brought an end to his tyranny. They sealed off his castle and locked whatever horrors Vordo created inside. Legends tell of his ghost roaming these halls, where he carries the notes that would allow him to leave the bottle. If I could banish his spirit to the true death, if only for a brief moment, I may sieze his notes and use gate or recall magic to escape this place."; }
			else if ( book is BookofDeadClue ){ book.BookText = "It sailed the world, capturing the many lost souls that swam by her. The lone captain, a necromancer, steered the vessel into waters of death and misery. Those that live seek the blackened ship, in search of the fabled Book of the Dead. With this power, a trained necromancer can take the body parts of the dead and create a walking fiend of mindless power. Only the dark heart must be obtained to perform such a feat. Legend tells that the dark vessel often retreats to Ambrosia, where the only way to board her is to utter the deathly word of power, ‘necropalyx’. Remember this word well as it must be spoken to escape the ship. To enter the deathly hold, you must find the dark pentagram and speak the word. The ship should be anchored nearby."; }
			else if ( book is CBookTombofDurmas ){ book.BookText = "King Durmas IV had a high mage on his council that was seeking the magic for immortality. Although charged to do this by the King, this mage was really seeking the power for themselves. The success of this mage were not known until centuries later when he rose from his grave and wanted to control the entire dead of the world, slaying the living in his wake. Until we can get this powerful lich under control, they will remain forever entombed in the crypt of the Durmas family. There is only one way in and out of the crypt. There is a stone altar built where speaking the word `xormluz` will send the one standing on the altar to magically appear in the sealed crypt. Speaking the words on the opposite side`s altar will bring those back from the crypt. Research continues."; }
			else if ( book is CBookElvesandOrks ){ book.BookText = "It is told that elves and orks exist, but their lands are worlds apart. Orks, the more civilized relations to orcs, live within the Savaged Empire. The elves, rich in culture and rarities, live in a huge land of Lodor. The bridge between the valley and Lodor is said to be an icy cave. The elves only go there to visit the mountains where it is said the gods can make rare and wonderful items."; }
			else if ( book is MagestykcClueBook ){ book.BookText = "The Council of Mages has had enough of the barbaric practices that this Magestykc group has been taking part in. The summoning of demons to our realms will not be tolerated. Although they cannot all be found, the majority of them have been banished to a part of the underworld to live their remaining days. There they can summon the lords of hell and be exiled with them. Their grand wizard has escaped however and can very easily make a magical gate between Sosaria and the underworld prison. I fear this day will come and we must prepare for the coming apocalypse. We will send some of our best wizards to see if this portal was in fact created. They commonly speak their group name to activate it so we should have little difficulty finding it."; }
			else if ( book is FamiliarClue ){ book.BookText = "I have spent days in this accursed dungeon, looking for clues of the existence of the gargoyle lands. I had enough food and water to last for days, but I couldn't carry it all. I heard from the other mages that the guildmasters often search for rubies. Apparently they are used for some types of spell casting. They happily take donations, but if an apprentice were to offer them 20 or more, they are often given a gift. It doesn't matter if you practice magic, or dark magic, as long as you reached the apprentice level of skill in such fields. There was something also similar I heard from another spellcaster that the guildmaster of black magic has a liking for star sapphires. I found none of those. During my last journey, I found 23 rubies in a metal chest, and gave them to the wizard guildmaster. He gave me a crystal ball, that summons a familiar to serve me. It doesn't do much in regards to services, but it will carry some of my things for me and keep me company. The crystal ball only had 5 charges, but the mage guildmasters can be hired to charge it further. I was given an imp for a traveling companion, but I wanted a black cat. I gave the crystal ball back to the mage, where he gave me another to look at. I simply kept passing them back to him until I got the cat I sought. The mage told me that I could use colors from common dye tubs, and pour it on the crystal ball. It would retain the color, and thus the familiar would share that color. He was right. I finally had my black cat familiar, just as other famous wizards have had. I named him Moonbeam. I am resting now, deep in the bottom of this place. I will continue my quest in the morning. I hear something nearby. I should see what it is."; }
			else if ( book is LodorBook ){ book.BookText = "For years I searched for a way to journey to the world of Lodor, what some call the land of elves. Although a myth to many, I have finally reached this new world. It seems to be a peaceful place with many cities. I found the City of Lodoria where the sages were able to teach me how to gain more power in the use of wizardry. I am dying from the passing of time and this new power will help me finally finish the rituals necessary to become a lich. I will roam this world in death as I did in life, atop my dark tower where the citizens of Montor will no longer laugh at me. I will leave the magic mirror in place, where I can simply look into the mirror and utter the word 'xetivat' to magically travel to Lodor. I need only say the word backwards in Lodor’s mirror to return to Sosaria. Maybe I will be able to conquer both worlds with my new found power. I will sleep now as I grow very tired."; }
			else if ( book is CBookTheLostTribeofSosaria ){ book.BookText = "Those that lived long ago built an enormous pyramid now buried by thousands of years of sand and stone in the northwestern part of Sosaria. No one is sure of what these people were, but legends say they left Sosaria through a magical portal and settled a new land rich in woods, skins, and ore."; }
			else if ( book is LillyBook ){ book.BookText = "Centuries ago, a peaceful gargoyle race fled the land of Sosaria to settle on the Serpent Island. It was long forgotten about until the Archmage Zekylis came to the Mages’ Guild in Fawn to boast of his discovery. He found the tunnel that leads to this world in the frozen lands but would not speak of the exact location. He told tales of a tropical land with the City of Furnace. There he learned the art of creating statues from stone and the ability to turn sand into glass to make other items. What intrigues me is that I have sent agents from the Thieves’ Guild to follow him to see if they can discover the location of the tunnel. They believe they found it in the mountains of the frozen lands, but the surrounding mountains are too treacherous to climb. They have witnessed Zekylis magically appear on top of the tower, so he must have a way to reach the tower from a portal elsewhere. Years have passed since learning anything new. It was only by accident that a hunter was at the Sleepy Island Inn, telling tales of a crazy wizard living in the nearby jungle of Umber Veil. Word got back to the Thieves’ Guild and they found the home of Zekylis. Apparently he married a woman from Renika, named Lilly. She apparently died from a giant serpent bite and was buried next to the home. Her parents are also buried there as they must have died from old age. A spy hid in the shadows nearby, watching and listening. Late one night, Zekylis came out of the house and approached Lilly’s grave. The spy had to duck behind the grave stone as not to be discovered. Zekylis stopped in front of her grave and said, ‘I love you Lilly’. The spy waited for quite some time and did not hear Zekylis walk away. Growing weary, the spy peaked around and saw that Zekylis was gone. He never returned to his home and it is as if he vanished without a trace. Magic jewels were found in his home but the effects could never be determined. How Zekylis escaped so easily from the spy is a mystery. He also took the secrets he learned with him. I fear we may never know how to get into his tower."; }
			else if ( book is LearnTraps ){ book.BookText = "There are more to fear in dungeons and tombs than monsters and undead. Those with a good 'searching' skill can find these traps as they are almost always hidden. One needs a good 'remove trap' skill to disable them, a ten-foot pole to trigger them, or magic that will make the trap useless. When you walk over a hidden trap, you will passively try to disable the trap. If your skill is high enough, you will simply disable it. If you cannot disable it, then you will passively try searching for it. If you manage to find it doing this search, then it will appear but not trigger unless walked over again. If you have a ten-foot pole, you will tap it and set it off before it can do anything to you. If you have trap removal magic cast upon you, then you will have an item in your pack that will work like a ten-foot pole does. All three of these elements will be checked if they are all available for the character. Your luck will also be tested, so the more luck you have the better the chance you will avoid the trap. Containers can be targeted for a specific trap removal or magic spell, but there are some passive checks on these as well. Containers have 4 possible traps of magic, explosion, dart, or poison. The hidden traps are on the floors of dangerous places, and there are 27 different effects they may have. Some are annoyances, others are deadly, and some can be devastating where you may lose a favorite item. <br><br>-Reveals you if hidden <br>-Trip and drop backpack <br>-Trip and drop an item <br>-Turns the coins to lead <br>-Ruins an equipped item <br>-Lose 1 strength <br>-Lose 1 dexterity <br>-Lose 1 intelligence <br>-Poison <br>-Reduced to 1 hit point <br>-Reduced to 1 stamina <br>-Reduced to 1 mana <br>-Turns gems to stone <br>-Ruins reagents <br>-Puts books in magic box <br>-Teleports you far away <br>-Lowers your fame <br>-Curses an equipped item <br>-Spike trap <br>-Saw blade trap <br>-Fire trap <br>-Giant spike trap <br>-Explosion trap <br>-Electrical trap <br>-Breaks bolts and arrows <br>-Ruins bandages <br>-Breaks potion bottles<br><br>Some have avoidance checks where it may test against your resistances or magic resist skill, so walking into one does not mean certain doom. Ten-foot poles are the least effective, and they weigh quite a bit. Magic is more effective, depending on the wizard's skill in magery. The most effective measure are those skilled with the remove trap skill, but with any trap, it is best to avoid all together."; }
			else if ( book is LearnTitles ){ book.BookText = "I have taught many from one end of Sosaria to the other. During this time, I am always curious about the need for people to group others by their skills and trades. My research into this matter has proven to be extensive as society has come up with many words to describe the skilled of the world. Below I document my findings. <br> <br>Alchemy <br>-- Alchemist <br>Anatomy <br>-- Biologist <br>Druidism <br>-- Naturalist <br>Arms Lore <br>-- Man-at-arms <br>Begging <br>-- Beggar <br>Blacksmithy <br>-- Blacksmith <br>Bludgeoning <br>-- Bludgeoner <br>Bowcrafting <br>-- Bowyer <br>Bushido <br>-- Samurai <br>Camping <br>-- Explorer <br>Carpentry <br>-- Carpenter <br>Cartography <br>-- Cartographer <br>Cooking <br>-- Chef <br>Discordance <br>-- Demoralizer <br>Elementalism <br>-- Elementalist <br>Fencing <br>-- Fencer <br>Fist Fighting <br>-- Brawler <br>Focus <br>-- Driven <br>Forensics <br>-- Undertaker <br>Healing <br>-- Healer or Mortician <br>Herding <br>-- Shepherd <br>Hiding <br>-- Skulker <br>Inscription <br>-- Scribe <br>Knightship <br>-- Knight <br>Lockpicking <br>-- Lockpicker <br>Lumberjacking <br>-- Lumberjack <br>Magery <br>-- Wizard or Sorceress <br>-- Archmage if there is a<br>   raw grandmaster talent<br>   in Magery and Necromancy <br>Magic Resistance <br>-- Magic Warder <br>Marksmanship <br>-- Marksman <br>Meditation <br>-- Meditator <br>Mercantile <br>-- Merchant <br>Mining <br>-- Miner <br>Musicianship <br>-- Bard <br>Necromancy <br>-- Necromancer or Witch <br>-- Archmage if there is a<br>   raw grandmaster talent<br>   in Magery and Necromancy <br>Ninjitsu <br>-- Ninja or Yakuza <br>Parrying <br>-- Duelist <br>Peacemaking <br>-- Pacifier <br>Poisoning <br>-- Assassin <br>Provocation <br>-- Rouser <br>Psychology <br>-- Scholar <br>Remove Trap <br>-- Trespasser <br>Seafaring <br>-- Sailor or Pirate <br>Searching <br>-- Scout <br>Snooping <br>-- Spy <br>Spiritualism <br>-- Spiritualist <br>Stealing <br>-- Thief <br>Stealth <br>-- Sneak <br>Swordsmanship <br>-- Swordsman <br>Tactics <br>-- Tactician <br>Tailoring <br>-- Tailor <br>Taming <br>-- Beastmaster <br>Tasting <br>-- Food Taster <br>Tinkering <br>-- Tinker <br>Tracking <br>-- Ranger <br>Veterinary <br>-- Veterinarian <br> <br>Oriental Titles <br><br>Alchemy <br>-- Waidan <br>Fencing <br>-- Yuki Ota <br>Fist Fighting <br>-- Karateka <br>Healing <br>-- Shukenja <br>Knightship <br>-- Youxia <br>Magery <br>-- Wu Jen <br>Marksmanship <br>-- Kyudo <br>Necromancy <br>-- Fangshi <br>Spiritualism <br>-- Neidan <br>Swordsmanship <br>-- Kensai <br>Tactics <br>-- Sakushi <br> <br>Evil Titles<br><br>Magery <br>-- Warlock <br>-- or <br>-- Enchantress <br> <br>Barbaric Titles<br><br>Alchemy <br>-- Herbalist <br>Bludgeoning <br>-- Barbarian (Amazon) <br>Druidism <br>-- Beastmaster <br>Camping <br>-- Wanderer <br>Fencing <br>-- Barbarian (Amazon) <br>Knightship <br>-- Chieftain (Valkyrie) <br>Herding <br>-- Beastmaster <br>Magery <br>-- Shaman <br>Marksmanship <br>-- Barbarian (Amazon) <br>Musicianship <br>-- Chronicler <br>Necromancy <br>-- Witch Doctor <br>Parrying <br>-- Defender <br>Seafaring <br>-- Atlantean <br>Swordsmanship <br>-- Barbarian (Amazon) <br>Tactics <br>-- Warlord <br>Taming <br>-- Beastmaster <br>Veterinary <br>-- Beastmaster<br><br>"; }
			else if ( book is GoldenRangers ){ book.BookText = "This is a guide for masters in camping or tracking, in their quest for the golden feathers. If you keep this manual with you, you may be able to find these mythical feathers so you can bless an item at the Altar of Golden Rangers. Those worthy of the golden feathers must hunt for either a type of harpy, or for the braver souls, a phoenix. They are rare to find for sure but the goddess may be watching as you slay such a creature and hand you these feathers. Once obtained, you may take the feathers to the Altar of Golden Rangers. Place a single weapon or piece of armor onto the altar and speak the word 'Aurum', which then the item will be turned to gold and blessed by the goddess of rangers. Remember to keep this book with you during your hunt. You must be the one to slay the beast as she only rewards master rangers or explorers with the gift of the feathers. Good luck, don't let greed get the best of you, as the goddess will not give you feathers if you already have them. She will simply bring them to you to remind you of your past rewards."; }
			else if ( book is AlchemicalElixirs ){ book.BookText = "The magical enhancement of the mind and body is something we can explore within the realm of alchemical elixirs. Reading this book now familiarizes you with these different types of potions and you can start mixing your own. Like other forms of alchemy, you need a mortar and pestle and the appropriate reagents. An empty bottle is also required. There are 49 different types of elixirs, and they all give one enhanced skills for a certain period of time. The only skills that they cannot enhance are those of a magical nature. These include skills such as magery, necromancy, ninjitsu, bushido, and knightship. All other skills can be enhanced with elixirs.<br><br>Elixirs have varying levels of effect, and it depends on a few factors. Some elixirs will last for about 1 to 6 minutes, while others will last for about 2 to 13 minutes. Each type of elixir is listed in this book, and the potential duration for each.<br><br>The duration is determined by 3 factors. 40% relies on how good the drinker's cooking skill is. Another 40% relies on how good the drinker is at tasting. The last 20% is based on the drinker's alchemy skill, along with any potion enhancement properties they may wield. The better these elements are, the longer the elixir will last. The strength of the elixir is based on these same factors, where you will either get a +10 to +60 to the skill the elixir is meant to enhance. While a particular elixir is in effect, you cannot drink another elixir of the same type nor can you be under the affect of more than 2 elixirs at a time. Below is a list of various elixirs.<br><br>- Elixir of Alchemy<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Anatomy<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Druidism<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Arms Lore<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Begging<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Blacksmithing<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Bludgeoning<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Bowcrafting<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Camping<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Carpentry<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Cartography<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Cooking<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Discordance<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Fencing<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Fist Fighting<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Focus<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Forensics<br>    Lasts 1 to 6 minutes<br><br>- Elixir of the healer<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Herding<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Hiding<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Inscription<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Lockpicking<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Lumberjacking<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Magic Resistance<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Marksmanship<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Meditating<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Mercantile<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Mining<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Musicianship<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Parrying<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Peacemaking<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Poisoning<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Provocation<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Psychology<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Removing Trap<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Seafaring<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Searching<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Snooping<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Spiritualism<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Stealing<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Stealth<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Sword Fighting<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Tactics<br>    Lasts 1 to 6 minutes<br><br>- Elixir of Tailoring<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Taming<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Tasting<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Tinkering<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Tracking<br>    Lasts 2 to 13 minutes<br><br>- Elixir of Veterinary<br>    Lasts 1 to 6 minutes"; }
			else if ( book is AlchemicalMixtures ){ book.BookText = "The mixing of ingredients with other potions allows a good alchemist to create mixtures that can be dumped on the ground with varying effects. Some mixtures are spread over the ground, where those that walk over the liquid will suffer the effects. The others create magically sentient slimes that follow the will of the alchemist that dumped it on the ground. This book now familiarizes you with these different types of potions and you can start mixing your own. Like other forms of alchemy, you need a mortar and pestle and the appropriate reagents. A type of potion and an empty jar are also required.<br><br>The effects that mixtures have will vary on a few factors. The duration is determined by 3 factors. 40% relies on how good the user's cooking skill is. Another 40% relies on how good the user is at tasting. The last 20% is based on the user's alchemy skill, along with any potion enhancement properties they may wield. The better these elements are, the longer the mixture will last when dumped. The strength of the dumped mixture is based on these same factors, where some slimes and liquids do more damage and are more resilient. Be warned with liquids. They will harm you just as much as anyone else so keep a safe distance."; }
			else if ( book is BookOfPoisons ){ book.BookText = "Poisons are commonly used by tavern keepers, to rid their cellars of vermin that feast on their wares. Others, of a more nefarious nature, will use poisons to meet their vile goals. No one is more of an expert with poisons as alchemists and assassins are. Poisons can be created in two different ways. Some will use the leaves of the nightshade to alchemically create them. Others will seek the venom sacks of creatures, where good poisoners can extract the venom into a bottle. To master the poisoning skill, it is best to start with weaker poisons before moving up to more deadly ones.<BR><BR>0-40 Lesser Poison<BR>20-60 Regular Poison<BR>40-80 Greater Poison<BR>60-100 Deadly Poison<BR>80-120 Lethal Poison<BR><BR>Those that are good with poisoning, can throw the contents of the bottle onto the ground. Anyone that walks over the spill may be poisoned, but so may the one who dumped it. Those that are not good enough with the poisoning skill, will likely drink the contents and suffer the effects. Below are the skills needed to dump these poison bottles on the ground:<br><br>Apprentice : Lesser<br>Journeyman : Regular<br>Expert : Greater<br>Adept : Deadly<br>Master : Lethal<br><br>The strength of the dumped poison relies on 3 factors. 40% relies on how good one's alchemy skill is. Another 40% relies on how good they are at tasting. The last 20% is based on one's poisoning skill. The better these elements are, the more deadly the dumped poison is.<br><br>One may be able to taint food with these poisons, or soak their bladed weapon with it. There are two methods that assassins use to handle poisoned weapons. One is the simple method of soaking the blade and having it poison whenever it strikes their opponent. With this method, there is little control on the dosage given but it is easier to maneuver. The other is the more tactical method, where only certain weapons can be poisoned and the assassin can control when the poison is administered with the hit. Although the tactical method requires more thought, it does have the potential to allow an assassin to poison certain arrows. The choice of methods can be switched at any time [see the Help section], but only one method can be in use at a given time."; }
			else if ( book is WorkShoppes )
			{
				string mercrate = "<br><br>If you want to earn more gold from your home, see the local provisioner and see if you can buy a merchant crate. These crates allow you to craft items, place them in the crate, and the Merchants Guild will pick up your wares after a set period of time. If you decide you want something back from the crate, make sure to take it out before the guild shows up.";

				if ( !MySettings.S_MerchantCrates )
					mercrate = "";

				book.BookText = "The world is filled with opportunity, where adventurers seek the help of other in order to achieve their goals. With filled coin purses, they seek experts in various crafts to acquire their skills. Some would need armor repaired, maps deciphered, potions concocted, scrolls translated, clothing fixed, or many other things. The merchants, in the cities and villages, often cannot keep up with the demand of these requests. This provides opportunity for those that practice a trade and have their own home from which to conduct business. Seek out a tradesman and see if they have an option for you to have them build you a Shoppe of your own. These Shoppes usually demand you to part with 10,000 gold, but they can quickly pay for themselves if you are good at your craft. You may only have one type of each Shoppe at any given time. So if you are skilled in two different types of crafts, then you can have a Shoppe for each. You will be the only one to use the Shoppe, but you may give permission to others to transfer the gold out into a bank check for themselves. Shoppes require to be stocked with tools and resources, and the Shoppe will indicate what those are. Simply drop such things onto your Shoppe to amass an inventory. When you drop tools onto your Shoppe, the number of tool uses will add to the Shoppe's tool count. A Shoppe may only hold 1,000 tools and 5,000 resources. After a set period of time, customers will make requests of you which you can fulfill or refuse. Each request will display the task, who it is for, the amount of tools needed, the amount of resources required, your chance to fulfill the request (based on the difficulty and your skill), and the amount of reputation your Shoppe will acquire if you are successful.<br><br>If you fail to perform a selected task, or refuse to do it, your Shoppe's reputation will drop by that same value you would have been rewarded with. Word of mouth travels fast in the land and you will have less prestigious work if your reputation is low. If you find yourself reaching the lows of becoming a murderer, your Shoppe will be useless as no one deals with murderers. Any gold earned will stay within the Shoppe until you single click the Shoppe and Transfer the funds out of it. Your Shoppe can have no more than 500,000 gold at a time, and you will not be able to conduct any more business in it until you withdraw the funds so it can amass more. The reputation for the Shoppe cannot go below 0, and it cannot go higher than 10,000. Again, the higher the reputation, the more lucrative work you will be asked to do. If you are a member of the associated crafting guild, your reputation will have a bonus toward it based on your crafting skill. Below are the Shoppes available, the skills required, and the merchants that will build them for you:<br><br>Alchemist Shoppe<br>- Alchemy of 50<br>-- Alchemists<br><br><br>Baker Shoppe<br>- Cooking of 50<br>-- Bakers<br>-- Cooks<br>-- Culinaries<br><br><br>Blacksmith Shoppe<br>- Blacksmithing of 50<br>-- Blacksmiths<br><br><br>Bowyer Shoppe<br>- Bowcrafting of 50<br>-- Bowyers<br>-- Archers<br><br><br>Carpenter Shoppe<br>- Carpentry of 50<br>-- Carpenters<br><br><br>Cartography Shoppe<br>- Cartography of 50<br>-- Mapmakers<br>-- Cartographers<br><br><br>Herbalist Shoppe<br>- Druidism of 50<br>-- Druids<br>-- Herbalists<br><br><br>Librarian Shoppe<br>- Inscription of 50<br>-- Sages<br>-- Scribes<br>-- Librarians<br><br><br>Tailor Shoppe<br>- Taloring of 50<br>-- Tailors<br>-- Weavers<br>-- Leather Workers<br><br><br>Tinker Shoppe<br>- Tinkering of 50<br>-- Tinkers<br><br><br>Witches Shoppe<br>- Forensic of 50<br>-- Witches" + mercrate + "";
			}
			else if ( book is GreyJournal ){ book.BookText = "It has been years since we discovered the place where Weston the tinker worked on the legendary sky ship. It had been long before that, where most forgot where Weston's home was built. Long since burned to the ground, we managed to find the basement. Everything looked to be undisturbed. The sky ship appears to be more than just a myth, but before our very eyes. If one were to believe the historical significance of the relic, then the valley of which those that settled in Devil Guard has a colorful history to be sure. To think the stranger caused the castle to fall from the sky is amazing. Even more wondrous that the castle was sent to the past before making its descent. It is often too much to believe. We keep removing items off of this small ship, packing them in crates. How they make the contraption work is beyond our reasoning. We decided to make this cavern a little more hospitable, perhaps spending some nights below. It would just allow us to continue our work when discoveries run late into the night."; }
			else if ( book is RuneJournal )
			{
				TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;
				book.BookText = "With reagents being rare in the Abyss, I began to research other ways to cast magery spells. I have found various old stone tablets here that describe the use of rune stones in this manner. Used by wizards of old, before they learned to harness magic within their own mana, these runic symbols can conjure various forms of magic. One must find a rune marked with the symbols needed to use the mantra for the spell. Once the correct ones are assembled they must be selected in a magical rune bag where one can then equip the bag and unleash the magic power of the spell. This is by no means a simple process, as gathering the runes can be quite tedious, but it is a way to cast spells in a pinch. I have been searching for a spell to summon a daemon for years now. I have already found the runes that will allow me to cast such a spell without the need of a rare scroll. Many mages scoff at the use of runes, but to me they are becoming a valuable arcana that I have not been able to do without. I will attempt to write my findings on these ancient ways to cast magic spells so others may one day benefit.";
				book.BookText += "<BR><BR>The following is all of my research on rune magic, the known spells, and the rune symbols.";
				book.BookText += "<BR><BR>Rune Bags<BR><BR>Rune bags and runes are imbued with the power to assist the caster in the casting of a spell without the need of reagents. Place the bag in your pack and open it. Then you can select each of the required rune stones inside. You can cast the spell by equipping the bag and doublc clicking it, provided the proper runes are selected within it. These bags hold magical charges, that can be charged for a fee by a mage. Some spells require a certain number of charges to cast a spell.";
				book.BookText += "<BR><BR>Meanings of Runes<BR><BR>An - Negate/Dispel<BR>Bet - Small<BR>Corp - Death<BR>Des - Lower/Down<BR>Ex - Freedom<BR>Flam - Flame<BR>Grav - Energy/Field<BR>Hur - Wind<BR>In - Make/Create/Cause<BR>Jux - Danger/Trap/Harm<BR>Kal - Summon/Invoke<BR>Lor - Light<BR>Mani - Life/Healing<BR>Nox - Poison<BR>Ort - Magic<BR>Por - Move/Movement<BR>Quas - Illusion<BR>Rel - Change<BR>Sanct - Protection<BR>Tym - Time<BR>Uus - Raise/Up<BR>Vas - Great<BR>Wis - Knowledge<BR>Xen - Creature<BR>Ylem - Matter<BR>Zu - Sleep<BR><BR>Runes must be used in combinations to form spells of power! The meanings are the key!";
				book.BookText += "<BR><BR>The following is a complete list of all known spells and the runes needed to cast them.";
				bool run = true;
				int spell = 0;
				MagicSpell magicspell = MagicSpell.None;

				while ( run )
				{
					spell++;
					magicspell = (MagicSpell)spell;

					if ( SpellItems.GetRunes( magicspell ) != "" )
						book.BookText += "<BR><BR>" + cultInfo.ToTitleCase( SpellItems.GetName( magicspell ) ) + "<BR>  " + SpellItems.GetRunes( magicspell ) + "<BR><BR>" + SpellItems.GetData( magicspell ) + "<BR>________________________";

					if ( magicspell == MagicSpell.VampiricEmbrace )
						run = false;
				}
				book.BookText += "<BR><BR><BR><BR>";
			}
		}
	}

	public class TendrinsJournal : DynamicBook
	{
		[Constructable]
		public TendrinsJournal( )
		{
			Weight = 1.0;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 30, this );
			SetStaticText( this );
			BookTitle = "Tendrin's Journal";
			Name = BookTitle;
			BookAuthor = "Tendrin Horum";
		}

		public TendrinsJournal( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class CBookNecroticAlchemy : DynamicBook
	{
		[Constructable]
		public CBookNecroticAlchemy( )
		{
			Weight = 1.0;
			Hue = 0x4AA;
			ItemID = 0x2B6F;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 32, this );
			SetStaticText( this );
			BookTitle = "Necrotic Alchemy";
			Name = BookTitle;
			switch( Utility.RandomMinMax( 0, 3 ) )
			{
				case 0: BookAuthor = NameList.RandomName( "vampire" ) + " the Vampire"; break;
				case 1: BookAuthor = NameList.RandomName( "ancient lich" ) + " the Lich"; break;
				case 2: BookAuthor = NameList.RandomName( "evil mage" ) + " the Warlock"; break;
				case 3: BookAuthor = NameList.RandomName( "evil witch" ) + " the Witch"; break;
			}
		}

		public CBookNecroticAlchemy( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
			Timer.DelayCall( TimeSpan.FromSeconds( 15.0 ), new TimerCallback( Delete ) );
		}
	}

	public class CBookDruidicHerbalism : DynamicBook
	{
		[Constructable]
		public CBookDruidicHerbalism( )
		{
			Weight = 1.0;
			ItemID = 0x2D50;
			Hue = 0;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 45, this );
			SetStaticText( this );
			BookTitle = "Druidic Herbalism";
			Name = BookTitle;
			BookAuthor = NameList.RandomName( "druid" ) + " the Druid";
		}

		public CBookDruidicHerbalism( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			Hue = 0;
			SetStaticText( this );
			Timer.DelayCall( TimeSpan.FromSeconds( 15.0 ), new TimerCallback( Delete ) );
		}
	}

	public class LoreGuidetoAdventure : DynamicBook
	{
		[Constructable]
		public LoreGuidetoAdventure( )
		{
			Weight = 1.0;
			ItemID = Utility.RandomList( 0x4FDF, 0x4FE0);

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 5, this );
			SetStaticText( this );
			BookTitle = "Guide to Adventure";
			Name = BookTitle;
			BookAuthor = RandomThings.GetRandomAuthor();
		}

		public LoreGuidetoAdventure( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class BookGuideToAdventure : DynamicBook
	{
		public Mobile owner;
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner { get{ return owner; } set{ owner = value; } }

		[Constructable]
		public BookGuideToAdventure( )
		{
			Weight = 1.0;
			ItemID = Utility.RandomList( 0x4FDF, 0x4FE0);

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 5, this );
			SetStaticText( this );
			BookTitle = "Guide to Adventure";
			Name = BookTitle;
			BookAuthor = RandomThings.GetRandomAuthor();
		}

		public BookGuideToAdventure( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
			writer.Write( (Mobile)owner);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
			owner = reader.ReadMobile();
		}
	}

	public class BookBottleCity : DynamicBook
	{
		[Constructable]
		public BookBottleCity( )
		{
			Weight = 1.0;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 30, this );
			SetStaticText( this );
			BookTitle = "The Bottle City";
			Name = BookTitle;
			BookAuthor = RandomThings.GetRandomAuthor();
		}

		public BookBottleCity( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class BookofDeadClue : DynamicBook
	{
		[Constructable]
		public BookofDeadClue( )
		{
			Weight = 1.0;
			Hue = 932;
			ItemID = 0x2B6F;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 35, this );
			SetStaticText( this );
			BookTitle = "Barge of the Dead";
			Name = BookTitle;
			switch( Utility.RandomMinMax( 0, 3 ) )
			{
				case 0: BookAuthor = NameList.RandomName( "vampire" ) + " the Vampire"; break;
				case 1: BookAuthor = NameList.RandomName( "ancient lich" ) + " the Lich"; break;
				case 2: BookAuthor = NameList.RandomName( "evil mage" ) + " the Necromancer"; break;
				case 3: BookAuthor = NameList.RandomName( "evil witch" ) + " the Witch"; break;
			}
		}

		public BookofDeadClue( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class CBookTombofDurmas : DynamicBook
	{
		[Constructable]
		public CBookTombofDurmas( )
		{
			Weight = 1.0;
			Hue = 0x966;
			ItemID = 0x2B6F;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 14, this );
			SetStaticText( this );
			BookTitle = "Tomb of Durmas";
			Name = BookTitle;
			BookAuthor = RandomThings.GetRandomAuthor();
		}

		public CBookTombofDurmas( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class CBookElvesandOrks : DynamicBook
	{
		[Constructable]
		public CBookElvesandOrks( )
		{
			Weight = 1.0;
			Hue = 956;
			ItemID = 0xFF4;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 64, this );
			SetStaticText( this );
			BookTitle = "Elves and Orks";
			Name = BookTitle;
			BookAuthor = RandomThings.GetRandomAuthor();
		}

		public CBookElvesandOrks( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class MagestykcClueBook : DynamicBook
	{
		[Constructable]
		public MagestykcClueBook( )
		{
			Weight = 1.0;
			Hue = 509;
			ItemID = 0x22C5;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 12, this );
			SetStaticText( this );
			BookTitle = "Wizards in Exile";
			Name = BookTitle;
			switch( Utility.RandomMinMax( 0, 1 ) )
			{
				case 0: BookAuthor = NameList.RandomName( "evil mage" ) + " the Wizard"; break;
				case 1: BookAuthor = NameList.RandomName( "evil witch" ) + " the Sorceress"; break;
			}
		}

		public MagestykcClueBook( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class FamiliarClue : DynamicBook
	{
		[Constructable]
		public FamiliarClue( )
		{
			Weight = 1.0;
			Hue = 459;
			ItemID = 0x22C5;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 46, this );
			SetStaticText( this );
			BookTitle = "Journal";
			Name = BookTitle;
			switch( Utility.RandomMinMax( 0, 1 ) )
			{
				case 0: BookAuthor = NameList.RandomName( "male" ) + " the Awkward"; break;
				case 1: BookAuthor = NameList.RandomName( "female" ) + " the Awkward"; break;
			}
		}

		public FamiliarClue( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class LodorBook : DynamicBook
	{
		[Constructable]
		public LodorBook( )
		{
			Weight = 1.0;
			Hue = 0;
			ItemID = 0x1C11;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 64, this );
			SetStaticText( this );
			BookTitle = "Diary";
			Name = BookTitle;
			BookAuthor = RandomThings.GetRandomAuthor();
		}

		public LodorBook( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class CBookTheLostTribeofSosaria : DynamicBook
	{
		[Constructable]
		public CBookTheLostTribeofSosaria( )
		{
			Weight = 1.0;
			Hue = 0;
			ItemID = 0xFEF;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 42, this );
			SetStaticText( this );
			BookTitle = "Lost Tribe of Sosaria";
			Name = BookTitle;
			BookAuthor = RandomThings.GetRandomAuthor();
		}

		public CBookTheLostTribeofSosaria( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class LillyBook : DynamicBook
	{
		[Constructable]
		public LillyBook( )
		{
			Weight = 1.0;
			Hue = 0;
			ItemID = 0x225A;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 57, this );
			SetStaticText( this );
			BookTitle = "Gargoyle Secrets";
			Name = BookTitle;
			BookAuthor = RandomThings.GetRandomAuthor();
		}

		public LillyBook( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class LearnTraps : DynamicBook
	{
		[Constructable]
		public LearnTraps( )
		{
			Weight = 1.0;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 2, this );
			SetStaticText( this );
			BookTitle = "Hidden Traps";
			Name = BookTitle;
			BookAuthor = "Girmo the Legless";
		}

		public LearnTraps( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class LearnTitles : DynamicBook
	{
		[Constructable]
		public LearnTitles( )
		{
			Weight = 1.0;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 17, this );
			SetStaticText( this );
			BookTitle = "Titles of the Skilled";
			Name = BookTitle;
			BookAuthor = "Cartwise the Librarian";
		}

		public LearnTitles( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class GoldenRangers : DynamicBook
	{
		[Constructable]
		public GoldenRangers( )
		{
			Weight = 1.0;
			Hue = 0;
			ItemID = 0x222D;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 48, this );
			SetStaticText( this );
			BookTitle = "The Golden Rangers";
			Name = BookTitle;
			BookAuthor = "Vara the Explorer";
		}

		public GoldenRangers( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class AlchemicalElixirs : DynamicBook
	{
		[Constructable]
		public AlchemicalElixirs( )
		{
			Weight = 1.0;
			Hue = 0;
			ItemID = 0x2219;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 32, this );
			SetStaticText( this );
			BookTitle = "Alchemical Elixirs";
			Name = BookTitle;
			BookAuthor = "Vragan the Mixologist";
		}

		public AlchemicalElixirs( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class AlchemicalMixtures : DynamicBook
	{
		[Constructable]
		public AlchemicalMixtures( )
		{
			Weight = 1.0;
			Hue = 0;
			ItemID = 0x2223;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 32, this );
			SetStaticText( this );
			BookTitle = "Alchemical Mixtures";
			Name = BookTitle;
			BookAuthor = "Miranda the Chemist";
		}

		public AlchemicalMixtures( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class BookOfPoisons : DynamicBook
	{
		[Constructable]
		public BookOfPoisons( )
		{
			Weight = 1.0;
			Hue = 0xB51;
			ItemID = 0x2B6F;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 72, this );
			SetStaticText( this );
			BookTitle = "Venom and Poisons";
			Name = BookTitle;
			BookAuthor = "Seryl the Assassin";
		}

		public BookOfPoisons( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class WorkShoppes : DynamicBook
	{
		[Constructable]
		public WorkShoppes( )
		{
			Weight = 1.0;
			Hue = 0xB50;
			ItemID = 0x2259;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 59, this );
			SetStaticText( this );
			BookTitle = "Work Shoppes";
			Name = BookTitle;
			BookAuthor = "Zanthura of the Coin";
		}

		public WorkShoppes( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class GreyJournal : DynamicBook
	{
		[Constructable]
		public GreyJournal( )
		{
			Weight = 1.0;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 71, this );
			SetStaticText( this );
			BookTitle = "Legend of the Sky Castle";
			Name = BookTitle;
			BookAuthor = "Ataru Callis";
			ItemID = 0x1C13;
			Hue = 0;
		}

		public GreyJournal( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}

	public class RuneJournal : DynamicBook
	{
		[Constructable]
		public RuneJournal( )
		{
			Weight = 1.0;

			BookRegion = null;	BookMap = null;		BookWorld = null;	BookItem = null;	BookTrue = 1;	BookPower = 0;

			SetBookCover( 46, this );
			SetStaticText( this );
			BookTitle = "Rune Magic";
			Name = BookTitle;
			BookAuthor = "Garamon the Wizard";
			ItemID = 0x5687;
			Hue = 0xAFE;
		}

		public RuneJournal( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
			SetStaticText( this );
		}
	}
}