using System.Collections;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using Server.Utilities;

namespace Server.Items
{
    public class DDRelicTablet : Item, IRelic
	{
		public override void ItemIdentified( bool id )
		{
			m_NotIdentified = id;
		}

		public int RelicFlipID1;
		public int RelicFlipID2;
		public string RelicDescription;

		public string SearchDungeon;
		public string SearchType;
		public string SearchItem;
		public int SearchReal;

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_FlipID1 { get { return RelicFlipID1; } set { RelicFlipID1 = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_FlipID2 { get { return RelicFlipID2; } set { RelicFlipID2 = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Relic_Describe { get { return RelicDescription; } set { RelicDescription = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Search_Dungeon { get { return SearchDungeon; } set { SearchDungeon = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Search_Type { get { return SearchType; } set { SearchType = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Search_Item { get { return SearchItem; } set { SearchItem = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Search_Real { get { return SearchReal; } set { SearchReal = value; InvalidateProperties(); } }

		[Constructable]
		public DDRelicTablet() : base( 0xED7 )
		{
			Weight = 20;
			CoinPrice = Utility.RandomMinMax( 80, 500 );
			NotIdentified = true;
			NotIDSource = Identity.Artifact;
			NotIDSkill = IDSkill.Mercantile;

			if ( SearchReal > 0 )
			{
				// DO NOTHING
			}
			else
			{
				SearchReal = Utility.RandomMinMax( 1, 100 );
				int relic = Utility.RandomMinMax( 1, ArtifactQuestList.MaxNumber );
				SearchType = ArtifactQuestList.GetArtifact( relic, 2 );
				SearchItem = ArtifactQuestList.GetArtifact( relic, 1 );
				SearchDungeon = SearchLocation();
			}

			string sMade = "Made of colored stone";
			switch ( Utility.RandomMinMax( 0, 7 ) ) 
			{
				case 0: Hue = Utility.RandomColor(0); break;
				case 1: Hue = Utility.RandomColor(0); break;
				case 2: Hue = Utility.RandomColor(0); break;
				case 3: Hue = Utility.RandomColor(0); break;
				case 4: Hue = Utility.RandomColor(0); break;
				case 5: Hue = Utility.RandomColor(0); break;
				case 6: Hue = 0; sMade = "Made of stone"; break;
				case 7: Hue = 0; sMade = "Made of stone"; break;
			}

			string sLook = "a rare";
			switch ( Utility.RandomMinMax( 0, 7 ) )
			{
				case 0:	sLook = "a rare";		break;
				case 1:	sLook = "an old";		break;
				case 2:	sLook = "an ancient";	break;
				case 3:	sLook = "an unusual";	break;
				case 4:	sLook = "a curious";	break;
				case 5:	sLook = "a unique";		break;
				case 6:	sLook = "a strange";	break;
				case 7:	sLook = "an odd";		break;
			}

			switch ( Utility.RandomMinMax( 0, 50 ) )
			{
				case 0:		sMade = "Made of bronze";		Hue = 0xB9A;	break;
				case 1:		sMade = "Made of jade";			Hue = 0xB93;	break;
				case 2:		sMade = "Made of granite";		Hue = 0xB8E;	break;
				case 3:		sMade = "Made of marble";		Hue = 0xB8B;	break;
				case 4:		sMade = "Made of copper";		Hue = 0x972;	break;
				case 5:		sMade = "Made of ice";			Hue = 0x480;	break;
				case 6:		sMade = "Made of silver";		Hue = 0x835;	break;
				case 7:		sMade = "Made of amethyst";		Hue = 0x492;	break;
				case 8:		sMade = "Made of emerald";		Hue = 0x5B4;	break;
				case 10:	sMade = "Made of garnet";		Hue = 0x48F;	break;
				case 11:	sMade = "Made of onyx";			Hue = 0x497;	break;
				case 12:	sMade = "Made of quartz";		Hue = 0x4AC;	break;
				case 13:	sMade = "Made of ruby";			Hue = 0x5B5;	break;
				case 14:	sMade = "Made of sapphire";		Hue = 0x5B6;	break;
				case 15:	sMade = "Made of spinel";		Hue = 0x48B;	break;
				case 16:	sMade = "Made of star ruby";	Hue = 0x48E;	break;
				case 17:	sMade = "Made of topaz";		Hue = 0x488;	break;
				case 18:	sMade = "Made of ivory";		Hue = 0x47E;	break;
				case 19:	sMade = "Made of solid gold";	Hue = 0x4AC;	CoinPrice = CoinPrice * 2; Weight = Weight * 2; break;
			}

			Name = sLook + " tablet " + sMade;

			switch ( Utility.RandomMinMax( 0, 4 ) ) 
			{
				case 0: ItemID = 0xED7; RelicFlipID1 = 0xED7; RelicFlipID2 = 0xED8; break;
				case 1: ItemID = 0x1165; RelicFlipID1 = 0x1165; RelicFlipID2 = 0x1166; break;
				case 2: ItemID = 0x116B; RelicFlipID1 = 0x116B; RelicFlipID2 = 0x116C; break;
				case 3: ItemID = 0x1177; RelicFlipID1 = 0x1177; RelicFlipID2 = 0x1178; break;
				case 4: ItemID = 0x1181; RelicFlipID1 = 0x1181; RelicFlipID2 = 0x1182; break;
			}

			string sLanguage = "pixie";
			switch( Utility.RandomMinMax( 0, 35 ) )
			{
				case 0: sLanguage = "balron"; break;
				case 1: sLanguage = "pixie"; break;
				case 2: sLanguage = "centaur"; break;
				case 3: sLanguage = "demonic"; break;
				case 4: sLanguage = "dragon"; break;
				case 5: sLanguage = "dwarvish"; break;
				case 6: sLanguage = "elven"; break;
				case 7: sLanguage = "fey"; break;
				case 8: sLanguage = "gargoyle"; break;
				case 9: sLanguage = "cyclops"; break;
				case 10: sLanguage = "gnoll"; break;
				case 11: sLanguage = "goblin"; break;
				case 12: sLanguage = "gremlin"; break;
				case 13: sLanguage = "druidic"; break;
				case 14: sLanguage = "tritun"; break;
				case 15: sLanguage = "minotaur"; break;
				case 16: sLanguage = "naga"; break;
				case 17: sLanguage = "ogrish"; break;
				case 18: sLanguage = "orkish"; break;
				case 19: sLanguage = "sphinx"; break;
				case 20: sLanguage = "treekin"; break;
				case 21: sLanguage = "trollish"; break;
				case 22: sLanguage = "undead"; break;
				case 23: sLanguage = "vampire"; break;
				case 24: sLanguage = "dark elf"; break;
				case 25: sLanguage = "magic"; break;
				case 26: sLanguage = "human"; break;
				case 27: sLanguage = "symbolic"; break;
				case 28: sLanguage = "runic"; break;
			}

			string sPart = "a strange";
			switch( Utility.RandomMinMax( 0, 5 ) )
			{
				case 0:	sPart = "a strange ";	break;
				case 1:	sPart = "an odd ";		break;
				case 2:	sPart = "an ancient ";	break;
				case 3:	sPart = "a long dead ";	break;
				case 4:	sPart = "a cryptic ";	break;
				case 5:	sPart = "a mystical ";	break;
			}

			string sWrite = "carved";
			switch( Utility.RandomMinMax( 0, 3 ) )
			{
				case 0:	sWrite = "carved";		break;
				case 1:	sWrite = "chiseled";	break;
				case 2:	sWrite = "engraved";	break;
				case 3:	sWrite = "etched";		break;
			}

			RelicDescription = sWrite + " in " + sPart + sLanguage + " language";
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1049644, RelicDescription);
        }

		public class TabletGump : Gump
		{
			public TabletGump( Mobile from, Item tablet ): base( 100, 100 )
			{
				DDRelicTablet stone = (DDRelicTablet)tablet;

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);
				AddImage(13, 13, 102);
				AddHtml( 61, 79, 133, 88, @"<BODY><BASEFONT Color=#111111><BIG><CENTER>Somewhere in " + stone.SearchDungeon + "<BR>Lies an Artifact</CENTER></BIG></BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 38, 256, 181, 18, @"<BODY><BASEFONT Color=#111111><BIG><CENTER>" + stone.SearchItem + "</CENTER></BIG></BASEFONT></CENTER></BODY>", (bool)false, (bool)false);
			}
		}

		public override void OnDoubleClick( Mobile e )
		{
			// On the ground
			if (RootParentEntity == null)
			{
				BaseHouse house = BaseHouse.FindHouseAt(this);

				// In a house, with access
				if (house != null && (house.Public ? !house.IsBanned(e) : house.HasAccess(e)))
				{
					if (Movable)
						ItemID = ItemID == RelicFlipID1 ? RelicFlipID2 : RelicFlipID1;
					else
						e.SendMessage( "This cannot be rotated at this time." );
				}
				else if ( !IsChildOf( e.Backpack ) ) 
					e.SendMessage( "This must be in your backpack to read." );
				
				return;

			}
		
			// Try to ID it
			if (NotIdentified)
			{
				if ( e is PlayerMobile && !((PlayerMobile)e).Preferences.DoubleClickID ) return;

				if ( !IsChildOf( e.Backpack ) && MySettings.S_IdentifyItemsOnlyInPack ) 
					e.SendMessage( "This must be in your backpack to identify." );
				else
					IDCommand( e );

				return;
			}

			if ( e.Int >= SearchReal )
			{
				e.CloseGump( typeof( TabletGump ) );
				e.SendGump( new TabletGump( e, this ) );
			}
			else
				e.SendMessage( "This table looks quite old." );
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

		public static string SearchLocation()
		{
			string place = "";

			int aCount = 0;
			ArrayList targets = new ArrayList();
			foreach ( Item target in World.Items.Values )
			if ( target is SearchBase )
			{
				targets.Add( target );
				aCount++;
			}

			aCount = Utility.RandomMinMax( 1, aCount );

			int xCount = 0;
			for ( int i = 0; i < targets.Count; ++i )
			{
				xCount++;

				if ( xCount == aCount )
				{
					Item finding = ( Item )targets[ i ];
					place = Server.Misc.Worlds.GetRegionName( finding.Map, finding.Location );
				}
			}

			return place;
		}

		public DDRelicTablet(Serial serial) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
            writer.Write( (int) 1 ); // version
            writer.Write( RelicFlipID1 );
            writer.Write( RelicFlipID2 );
            writer.Write( RelicDescription );
            writer.Write( SearchDungeon );
            writer.Write( SearchType );
            writer.Write( SearchItem );
            writer.Write( SearchReal );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
            int version = reader.ReadInt();

			if ( version < 1 )
				CoinPrice = reader.ReadInt();

            RelicFlipID1 = reader.ReadInt();
            RelicFlipID2 = reader.ReadInt();
            RelicDescription = reader.ReadString();
			SearchDungeon = reader.ReadString();
			SearchType = reader.ReadString();
			SearchItem = reader.ReadString();
			SearchReal = reader.ReadInt();
		}
	}
}