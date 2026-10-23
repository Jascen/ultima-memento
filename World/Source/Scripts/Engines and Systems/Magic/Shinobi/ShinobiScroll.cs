using System.Collections.Generic;
using Server.Network;
using Server.Gumps;
using Server.Spells;
using Server.Spells.Shinobi;

namespace Server.Items
{
	public class ShinobiScroll : Item
	{
		public override bool DisplayWeight { get { return false; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool HasCheetahPaws { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool HasDeception { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool HasEagleEye { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool HasEspionage { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool HasFerretFlee { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool HasMonkeyLeap { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool HasMysticShuriken { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool HasTigerStrength { get; set; }

		public int Page;
		[CommandProperty(AccessLevel.GameMaster)]
		public int Page_ { get { return Page; } set { Page = value; InvalidateProperties(); } }

		public Mobile owner;
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner { get{ return owner; } set{ owner = value; } }

		private int TotalLearnedSkills
		{
			get
			{
				int skills = 0;
				if ( HasCheetahPaws ) skills++;
				if ( HasDeception ) skills++;
				if ( HasEagleEye ) skills++;
				if ( HasEspionage ) skills++;
				if ( HasFerretFlee ) skills++;
				if ( HasMonkeyLeap ) skills++;
				if ( HasMysticShuriken ) skills++;
				if ( HasTigerStrength) skills++;

				return skills;
			}
		}

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
				foreach ( var definition in ShinobiSpellProvider.SpellDefinitions )
				{
					ShinobiTrial trial = GetTrial( definition.SpellID );
					if ( !GetAbility( definition.SpellID ) && ( dropped.Name == trial.Prize1 || dropped.Name == trial.Prize2 ) )
					{
						win = definition.Name;
						SetAbility( definition.SpellID, true );
						break;
					}
				}

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

            list.Add( 1049644, "" + TotalLearnedSkills + " Abilities");
			if ( owner != null ){ list.Add( 1070722, "Belongs to " + owner.Name + "" ); }
        } 

		public static bool HasAbility( Mobile m, int spellID, ShinobiScroll scroll )
		{
			return scroll.owner == m && scroll.GetAbility( spellID );
		}

		private bool GetAbility( int spellID )
		{
			if ( spellID == CheetahPaws.SpellInfo.SpellDefinition.SpellID ) return HasCheetahPaws;
			if ( spellID == Deception.SpellInfo.SpellDefinition.SpellID ) return HasDeception;
			if ( spellID == EagleEye.SpellInfo.SpellDefinition.SpellID ) return HasEagleEye;
			if ( spellID == Espionage.SpellInfo.SpellDefinition.SpellID ) return HasEspionage;
			if ( spellID == FerretFlee.SpellInfo.SpellDefinition.SpellID ) return HasFerretFlee;
			if ( spellID == MonkeyLeap.SpellInfo.SpellDefinition.SpellID ) return HasMonkeyLeap;
			if ( spellID == MysticShuriken.SpellInfo.SpellDefinition.SpellID ) return HasMysticShuriken;
			if ( spellID == TigerStrength.SpellInfo.SpellDefinition.SpellID ) return HasTigerStrength;

			return false;
		}

		private void SetAbility( int spellID, bool value )
		{
			if ( spellID == CheetahPaws.SpellInfo.SpellDefinition.SpellID ) HasCheetahPaws = value;
			if ( spellID == Deception.SpellInfo.SpellDefinition.SpellID ) HasDeception = value;
			if ( spellID == EagleEye.SpellInfo.SpellDefinition.SpellID ) HasEagleEye = value;
			if ( spellID == Espionage.SpellInfo.SpellDefinition.SpellID ) HasEspionage = value;
			if ( spellID == FerretFlee.SpellInfo.SpellDefinition.SpellID ) HasFerretFlee = value;
			if ( spellID == MonkeyLeap.SpellInfo.SpellDefinition.SpellID ) HasMonkeyLeap = value;
			if ( spellID == MysticShuriken.SpellInfo.SpellDefinition.SpellID ) HasMysticShuriken = value;
			if ( spellID == TigerStrength.SpellInfo.SpellDefinition.SpellID ) HasTigerStrength = value;
		}

		public ShinobiScroll( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)2 ); // version
			writer.Write( HasCheetahPaws );
			writer.Write( HasDeception );
			writer.Write( HasEagleEye );
			writer.Write( HasEspionage );
			writer.Write( HasFerretFlee );
			writer.Write( HasMonkeyLeap );
			writer.Write( HasMysticShuriken );
			writer.Write( HasTigerStrength );
			writer.Write( Page );
			writer.Write( (Mobile)owner);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
            HasCheetahPaws = 1 < version ? reader.ReadBool() : reader.ReadInt() == 1;
			HasDeception = 1 < version ? reader.ReadBool() : reader.ReadInt() == 1;
			HasEagleEye = 1 < version ? reader.ReadBool() : reader.ReadInt() == 1;
			HasEspionage = 1 < version ? reader.ReadBool() : reader.ReadInt() == 1;
			HasFerretFlee = 1 < version ? reader.ReadBool() : reader.ReadInt() == 1;
			HasMonkeyLeap = 1 < version ? reader.ReadBool() : reader.ReadInt() == 1;
			HasMysticShuriken = 1 < version ? reader.ReadBool() : reader.ReadInt() == 1;
			HasTigerStrength = 1 < version ? reader.ReadBool() : reader.ReadInt() == 1;
			Page = reader.ReadInt();
			owner = reader.ReadMobile();
		}

		public static string ShinobiSpeech( ShinobiScroll scroll )
		{
			string text = "Ninjas are the exotic assassins, thieves, and spies of the land. They prefer stealth and sneak attacks above most other forms of conflict, and they have a series of mystical maneuvers they can perform. These abilities are the way of the shinobi. ";

			if ( scroll.TotalLearnedSkills < 8 )
			{
				text = text + "To prove yourself of being worthy of these abilities, you will have to overcome a series of trials. Each ability will list the item you must seek and the place where it can be sought. The ninja has a choice of trial to undertake, as each ability provides two choices. One is in the land of Sosaria and the other is in the land of Lodoria. Slay the beasts and get the items. Place them on this scroll and the shinobi secrets will be revealed to you. ";
			}

			text = text + "<br><br>Each ability will display the description as well as the skill required, the tithing gold needed, along with the mana necessary. To tithe gold to the spirits, find a Shrine of Durama where you can select it and then offer the gold you wish to part with. You can usually find these shrines in a dojo, and some ninjas tithe gold to almost any other deity. There are choices for having a horizontal or vertical menu bar to use these abilities quickly, but you can also select the icon within this scroll to use the skill.";

			text = text + "<br><br>You may also use [Spells_Shinobi to see the commands to use your shinobi abilities.";

			return text;
		}

		public class ShinobiTrial
		{
			public string Prize1;
			public string Where1;
			public string Prize2;
			public string Where2;

			public ShinobiTrial( string prize1, string where1, string prize2, string where2 )
			{
				Prize1 = prize1; Where1 = where1;
				Prize2 = prize2; Where2 = where2;
			}
		}

		private static readonly Dictionary<int, ShinobiTrial> m_Trials = new Dictionary<int, ShinobiTrial>
		{
			{ CheetahPaws.SpellInfo.SpellDefinition.SpellID, 		new ShinobiTrial( "chest of suffering", "the Ancient Pyramid", "egg of the harpy hen", "Dungeon Covetous" ) },
			{ Deception.SpellInfo.SpellDefinition.SpellID, 		new ShinobiTrial( "braclet of war", "Dungeon Clues", "face of the ancient king", "the Lodoria Catacombs" ) },
			{ EagleEye.SpellInfo.SpellDefinition.SpellID, 		new ShinobiTrial( "stump of the ancients", "Dardin's Pit", "wand of Talosh", "Dungeon Deceit" ) },
			{ Espionage.SpellInfo.SpellDefinition.SpellID, 		new ShinobiTrial( "dark blood", "Dungeon Doom", "head of Urg", "Dungeon Despise" ) },
			{ FerretFlee.SpellInfo.SpellDefinition.SpellID, 		new ShinobiTrial( "firescale tooth", "the Fires of Hell", "crown of Vorgol", "the City of Embers" ) },
			{ MonkeyLeap.SpellInfo.SpellDefinition.SpellID, 		new ShinobiTrial( "ichor of Xthizx", "the Mines of Morinia", "claw of Saramon", "Dungeon Hythloth" ) },
			{ MysticShuriken.SpellInfo.SpellDefinition.SpellID, 	new ShinobiTrial( "heart of a vampire queen", "the Perinian Depths", "horn of the frozen hells", "the Ice Fiend Lair" ) },
			{ TigerStrength.SpellInfo.SpellDefinition.SpellID, 	new ShinobiTrial( "hourglass of ages", "the Dungeon of Time Awaits", "elemental salt", "Dungeon Shame" ) },
		};

		public static ShinobiTrial GetTrial( int spellID )
		{
			ShinobiTrial trial;
			return m_Trials.TryGetValue( spellID, out trial ) ? trial : null;
		}

		public class ShinobiScrollGump : Gump
		{
			private const int FIRST_ABILITY_PAGE = 3;

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
					AddHtml( 71, 75, 384, 140, @"<BODY><BASEFONT Color=#111111><BIG>" + ShinobiSpeech( mScroll ) + "</BIG></BASEFONT></BODY>", (bool)false, (bool)true, false);
				}
				else if ( scroll.Page == 2 )
				{
					AddButton(60, 90, 4011, 4011, 21, GumpButtonType.Reply, 0);
					AddHtml( 100, 90, 340, 21, @"<BODY><BASEFONT Color=#111111><BIG>Open Horizontal Ability Bar</BIG></BASEFONT></BODY>", (bool)false, (bool)false,  false);

					AddButton(60, 135, 4011, 4011, 22, GumpButtonType.Reply, 0);
					AddHtml( 100, 135, 340, 21, @"<BODY><BASEFONT Color=#111111><BIG>Open Vertical Ability Bar</BIG></BASEFONT></BODY>", (bool)false, (bool)false,  false);

					AddButton(60, 180, 4017, 4017, 23, GumpButtonType.Reply, 0);
					AddHtml( 100, 180, 340, 21, @"<BODY><BASEFONT Color=#111111><BIG>Close Ability Bar</BIG></BASEFONT></BODY>", (bool)false, (bool)false,  false);
				}
				else if ( scroll.Page >= FIRST_ABILITY_PAGE && scroll.Page < FIRST_ABILITY_PAGE + ShinobiSpellProvider.SpellCount )
				{
					SpellDefinition definition = ShinobiSpellProvider.SpellDefinitions[scroll.Page - FIRST_ABILITY_PAGE];
					string clue = "";
					if ( HasAbility( from, definition.SpellID, mScroll ) )
					{
						AddButton(47, 56, definition.IconGraphic, definition.IconGraphic, definition.SpellID, GumpButtonType.Reply, 0);
						clue = definition.Description;
					}
					else
					{
						ShinobiTrial trial = GetTrial( definition.SpellID );
						AddImage(47, 57, definition.IconGraphic);
						clue = "To be worthy of this ability, you need to either get the " + trial.Prize1 + " at " + trial.Where1 + " in the Land of Sosaria or the " + trial.Prize2 + " at " + trial.Where2 + " in the Land of Lodoria.";
					}

					AddHtml( 56, 109, 414, 92, @"<BODY><BASEFONT Color=#111111><BIG>" + clue + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false,  false);
					AddHtml( 98, 68, 340, 21, @"<BODY><BASEFONT Color=#111111><BIG>" + definition.Name + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false,  false);

					AddItem(71, 205, 10126);
					AddHtml( 110, 205, 48, 21, @"<BODY><BASEFONT Color=#111111><BIG>" + definition.MinSkill + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false,  false);
					AddTooltip("Required skill");

					AddItem(220, 208, 3822);
					AddHtml( 254, 205, 48, 21, @"<BODY><BASEFONT Color=#111111><BIG>" + definition.TithingCost + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false,  false);
					AddTooltip("Required tithing");

					AddItem(364, 204, 10231);
					AddHtml( 402, 205, 48, 21, @"<BODY><BASEFONT Color=#111111><BIG>" + definition.ManaCost + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false,  false);
					AddTooltip("Required mana");
				}
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				Mobile from = state.Mobile;

				bool showScroll = true;
				bool isAbility = ShinobiSpellProvider.GetDefinition( info.ButtonID ) != null;
				from.PlaySound( 0x249 );

				if ( info.ButtonID < 1 ){ showScroll = false; }
				else if ( isAbility ){ ShinobiSpellProvider.Cast( from, info.ButtonID ); }
				else if ( info.ButtonID > 20 && info.ButtonID < 24 )
				{
					from.CloseGump( typeof( ShinobiRow ) );
					from.CloseGump( typeof( ShinobiColumn ) );
					showScroll = false;
				}

				if ( info.ButtonID == 21 ){ 		from.SendGump( new ShinobiRow( from, mScroll ) ); 		showScroll = false; }
				else if ( info.ButtonID == 22 ){ 	from.SendGump( new ShinobiColumn( from, mScroll ) ); 	showScroll = false; }

				if ( !isAbility && info.ButtonID > 0 ){ from.SendSound( 0x4A ); }

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
				foreach ( var definition in ShinobiSpellProvider.SpellDefinitions )
				{
					if ( !scroll.GetAbility( definition.SpellID ) ) continue;

					line = line+50;
					AddButton(line, 2, definition.IconGraphic, definition.IconGraphic, definition.SpellID, GumpButtonType.Reply, 0);
				}
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				Mobile from = state.Mobile;
				if ( ShinobiSpellProvider.GetDefinition( info.ButtonID ) != null ){ ShinobiSpellProvider.Cast( from, info.ButtonID ); }
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
				foreach ( var definition in ShinobiSpellProvider.SpellDefinitions )
				{
					if ( !scroll.GetAbility( definition.SpellID ) ) continue;

					line = line+50;
					AddButton(2, line, definition.IconGraphic, definition.IconGraphic, definition.SpellID, GumpButtonType.Reply, 0);
				}
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				Mobile from = state.Mobile;
				if ( ShinobiSpellProvider.GetDefinition( info.ButtonID ) != null ){ ShinobiSpellProvider.Cast( from, info.ButtonID ); }
				from.CloseGump( typeof( ShinobiColumn ) );
				if ( mScroll.owner == from )
				{
					from.SendGump( new ShinobiColumn( from, mScroll ) );
				}
			}
		}
	}
}
