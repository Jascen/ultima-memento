using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class DDRelicReagent : Item, IRelic
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
		public DDRelicReagent() : base( 0x44F1 )
		{
			Weight = 5;
			CoinPrice = Utility.RandomMinMax( 80, 500 );
			NotIdentified = true;
			NotIDSource = Identity.Reagent;
			NotIDSkill = IDSkill.Tasting;
			ItemID = Utility.RandomList( 0xE25, 0xE26, 0xE29, 0xE2A, 0xE2B, 0xE2C );
			Hue = Utility.RandomColor(0);

			string sName1 = "";
			string sName2 = "";

			if ( Utility.RandomMinMax( 1, 4 ) > 1 )
			{
				string[] vName1 = new string[] {"ant", "animal", "bat", "bear", "beetle", "boar", "brownie", "bugbear", "basilisk", "bull", "froglok", "cat", "centaur", "chimera", "cow", "crocodile", "cyclops", "dark elf", "demon", "devil", "doppelganger", "dragon", "drake", "dryad", "dwarf", "elf", "ettin", "frog", "gargoyle", "ghoul", "giant", "gnoll", "gnome", "goblin", "gorilla", "gremlin", "griffin", "hag", "hobbit", "harpy", "hippogriff", "hobgoblin", "horse", "hydra", "imp", "kobold", "kraken", "leprechaun", "lizard", "lizard man", "medusa", "human", "minotaur", "mouse", "naga", "nightmare", "nixie", "ogre", "orc", "pixie", "pegasus", "phoenix", "giant lizard", "rat", "giant snake", "satyr", "scorpion", "serpent", "shark", "snake", "sphinx", "giant spider", "spider", "sylvan", "sprite", "succubus", "sylvan", "titan", "toad", "troglodite", "troll", "unicorn", "vampire", "weasel", "werebear", "wererat", "werewolf", "werecat", "wolf", "worm", "wyrm", "wyvern", "yeti", "zombie"};
					sName1 = vName1[Utility.RandomMinMax( 0, (vName1.Length-1) )];
				string[] vName2 = new string[] {"bile", "blood", "bone dust", "essence", "extract", "eyes", "hair/skin", "herbs", "juice", "oil", "powder", "salt", "sauce", "scent", "serum", "spice", "spit", "tears", "teeth", "urine"};
					sName2 = vName2[Utility.RandomMinMax( 0, (vName2.Length-1) )];
				Name =  "bottle of " + sName1 + " " + sName2;
			}
			else
			{
				string[] vName1 = new string[] {"ants", "slime", "bat whiskers", "bees", "black cat hair", "black salt", "bloodworms", "cat whiskers", "centipedes", "coffin shavings", "crystal moonbeams", "cyclops eyelashes", "dragon scales", "efreet dust", "elemental dust", "eye of newt", "fairy dust", "fairy wings", "fire giant ash", "gelatinous goo", "genie smoke", "ghoul skin flakes", "graveyard dirt", "slime", "hell hound ash", "leeches", "lich dirt", "love honey", "mosquitoes", "mummy spice", "mystic dust", "ochre jelly", "phoenix ash", "pixie dust", "pixie wings", "ritual powder", "sea serpent salt", "serpent scales", "snake scales", "sorcerer sand", "sprite wings", "tree leaves", "reaper root", "ent sap", "vampire ash", "viper essence", "wasps", "wisp dust", "witch hazel", "worms", "zombie flesh"};
					sName1 = vName1[Utility.RandomMinMax( 0, (vName1.Length-1) )];
				Name =  "bottle of " + sName1;
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) && MySettings.S_IdentifyItemsOnlyInPack && from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified ) 
				from.SendMessage( "This must be in your backpack to identify." );
			else if ( from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified )
				IDCommand( from );
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

		public DDRelicReagent(Serial serial) : base(serial)
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