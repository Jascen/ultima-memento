using System;
using Server;
using System.Collections;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class DDRelicOrbs : Item, IRelic
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
		public DDRelicOrbs() : base( 0xE2F )
		{
			Hue = Utility.RandomColor(0);

			CoinPrice = Utility.RandomMinMax( 80, 500 );
			NotIdentified = true;
			NotIDSource = Identity.Magic;
			NotIDSkill = IDSkill.Mercantile;

			switch ( Utility.RandomMinMax( 0, 5 ) ) 
			{
				case 0: ItemID = 0xE2F; Weight = 20; break;
				case 1: ItemID = 0x4FD6; Weight = 20; break;
				case 2: ItemID = 0xE2D; Weight = 20; break;
				case 3: ItemID = 0x468A; Weight = 20; break;
				case 4: ItemID = 0x468B; Weight = 40; break;
				case 5: ItemID = 0x573E; Weight = 15; break;
			}

			string[] vSpell1 = new string[] {"Clyz", "Achug", "Theram", "Quale", "Lutin", "Gad", "Croeq", "Achund", "Therrisi", "Qualorm", "Lyeit", "Garaso", "Crul", "Ackhine", "Thritai", "Quaso", "Lyetonu", "Garck", "Cuina", "Ackult", "Tig", "Quealt", "Moin", "Garund", "Daror", "Aeny", "Tinalt", "Rador", "Moragh", "Ghagha", "Deet", "Aeru", "Tinkima", "Rakeld", "Morir", "Ghatas", "Deldrad", "Ageick", "Tinut", "Rancwor", "Morosy", "Gosul", "Deldrae", "Agemor", "Tonk", "Ranildu", "Mosat", "Hatalt", "Delz", "Aghai", "Tonolde", "Ranot", "Mosd", "Hatash", "Denad", "Ahiny", "Tonper", "Ranper", "Mosrt", "Hatque", "Denold", "Aldkely", "Torint", "Ransayi", "Mosyl", "Hatskel", "Denyl", "Aleler", "Trooph", "Ranzmor", "Moszight", "Hattia", "Drahono", "Anagh", "Turbelm", "Raydan", "Naldely", "Hiert", "Draold", "Anclor", "Uighta", "Rayxwor", "Nalusk", "Hinalde", "Dynal", "Anl", "Uinga", "Rhit", "Nalwar", "Hinall", "Dyndray", "Antack", "Umnt", "Risormy", "Nag", "Hindend", "Eacki", "Ardburo", "Undaughe", "Risshy", "Nat", "Iade", "Earda", "Ardmose", "Untdran", "Rodiz", "Nator", "Iaper", "Echal", "Ardurne", "Untld", "Rodkali", "Nayth", "Iass", "Echind", "Ardyn", "Uoso", "Rodrado", "Neil", "Iawy", "Echwaro", "Ashaugha", "Urnroth", "Roort", "Nenal", "Iechi", "Eeni", "Ashdend", "Urode", "Ruina", "Newl", "Ightult", "Einea", "Ashye", "Uskdar", "Rynm", "Nia", "Ildaw", "Eldsera", "Asim", "Uskmdan", "Rynryna", "Nikim", "Ildoq", "Eldwen", "Athdra", "Usksough", "Ryns", "Nof", "Inabel", "Eldyril", "Athskel", "Usktoro", "Rynut", "Nook", "Inaony", "Elmkach", "Atkin", "Ustagee", "Samgha", "Nybage", "Inease", "Elmll", "Aughint", "Ustld", "Samnche", "Nyiy", "Ineegh", "Emath", "Aughthere", "Ustton", "Samssam", "Nyseld", "Ineiti", "Emengi", "Avery", "Verporm", "Sawor", "Nysklye", "Ineun", "Emild", "Awch", "Vesrade", "Sayimo", "Nyw", "Ingr", "Emmend", "Banend", "Voraughe", "Sayn", "Oasho", "Isbaugh", "Emnden", "Beac", "Vorril", "Sayskelu", "Oendy", "Islyei", "Endvelm", "Belan", "Vorunt", "Scheach", "Oenthi", "Issy", "Endych", "Beloz", "Whedan", "Scheyer", "Ohato", "Istin", "Engeh", "Beltiai", "Whisam", "Serat", "Oldack", "Iumo", "Engen", "Bliorm", "Whok", "Sernd", "Oldar", "Jyhin", "Engh", "Burold", "Worath", "Skell", "Oldr", "Jyon", "Engraki", "Buror", "Worav", "Skelser", "Oldtar", "Kalov", "Engroth", "Byt", "Worina", "Slim", "Omdser", "Kelol", "Engum", "Cakal", "Worryno", "Snaest", "Ond", "Kinser", "Enhech", "Carr", "Worunty", "Sniund", "Oron", "Koor", "Enina", "Cayld", "Worwaw", "Sosam", "Orrbel", "Lear", "Enk", "Cerar", "Yary", "Stayl", "Osnt", "Leert", "Enlald", "Cerl", "Yawi", "Stol", "Peright", "Legar", "Enskele", "Cerv", "Yena", "Strever", "Perpban", "Lerev", "Eoru", "Chaur", "Yero", "Swaih", "Phiunt", "Lerzshy", "Ernysi", "Chayn", "Yerrves", "Tagar", "Poll", "Llash", "Erque", "Cheimo", "Yhone", "Taienn", "Polrad", "Llotor", "Errusk", "Chekim", "Yradi", "Taiyild", "Polsera", "Loem", "Ervory", "Chreusk", "Zhugar", "Tanen", "Puon", "Loing", "Essisi", "Chrir", "Zirt", "Tasaf", "Quaev", "Lorelmo", "Essnd", "Chroelt", "Zoine", "Tasrr", "Quahang", "Lorud", "Estech", "Cloran", "Zotin", "Thaeng", "Qual", "Lour", "Estkunt", "Etoth", "Esule", "Estnight"};
				string sSpell1 = vSpell1[Utility.RandomMinMax( 0, (vSpell1.Length-1) )];

			string[] vSpell2 = new string[] {"Exotic", "Mysterious", "Enchanted", "Marvelous", "Amazing", "Astonishing", "Mystical", "Astounding", "Magical", "Divine", "Excellent", "Magnificent", "Phenomenal", "Fantastic", "Incredible", "Extraordinary", "Fabulous", "Wondrous", "Glorious", "Lost", "Fabled", "Legendary", "Mythical", "Missing", "Ancestral", "Ornate", "Ultimate", "Rare", "Wonderful", "Sacred", "Almighty", "Supreme", "Mighty", "Unspeakable", "Unknown", "Forgotten"};
				string sSpell2 = vSpell2[Utility.RandomMinMax( 0, (vSpell2.Length-1) )];

			Name = sSpell1 + "'s " + sSpell2 + " Crystal Ball";
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) && MySettings.S_IdentifyItemsOnlyInPack && from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified ) 
				from.SendMessage( "This must be in your backpack to identify." );
			else if ( from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified )
				IDCommand( from );
			else
			{
				string sThing = "";
				switch ( Utility.RandomMinMax( 0, 51 ) )
				{
					case 0:		sThing = "a prince";	break;
					case 1: 	sThing = "a king";	break;
					case 2:		sThing = "a crown";	break;
					case 3:		sThing = "a sword";	break;
					case 4:		sThing = "an axe";		break;
					case 5:		sThing = "a lion";	break;
					case 6:		sThing = "a bear";	break;
					case 7:		sThing = "a bat";		break;
					case 8:		sThing = "a queen";	break;
					case 9:		sThing = "a princess";	break;
					case 10:	sThing = "a maiden";	break;
					case 11:	sThing = "a beggar";	break;
					case 12:	sThing = "a demon";	break;
					case 13:	sThing = "a devil";	break;
					case 14:	sThing = "an angel";	break;
					case 15:	sThing = "a dragon";	break;
					case 16:	sThing = "a shadow";		break;
					case 17:	sThing = "an eagle";	break;
					case 18:	sThing = "a hawk";	break;
					case 19:	sThing = "a bard";	break;
					case 20:	sThing = "a horse";	break;
					case 21:	sThing = "a wolf";	break;
					case 22:	sThing = "a pegasus";	break;
					case 23:	sThing = "a ram";		break;
					case 24:	sThing = "a skull";	break;
					case 25:	sThing = "a spider";	break;
					case 26:	sThing = "a unicorn";	break;
					case 27:	sThing = "a scorpion";	break;
					case 28:	sThing = "a pile of treasure";	break;
					case 29:	sThing = "a dead body";	break;
					case 30:	sThing = "an eye looking back at you";		break;
					case 31:	sThing = "a cross";					break;
					case 32:	sThing = "a woman";					break;
					case 33:	sThing = "a man";					break;
					case 34:	sThing = "a forest";				break;
					case 35:	sThing = "a snow covered land";		break;
					case 36:	sThing = "an ocean";				break;
					case 37:	sThing = "a desert";				break;
					case 38:	sThing = "a jungle";				break;
					case 39:	sThing = "a keep";					break;
					case 40:	sThing = "a house";					break;
					case 41:	sThing = "some ruins";				break;
					case 42:	sThing = "a castle";				break;
					case 43:	sThing = "a city";					break;
					case 44:	sThing = "a town";					break;
					case 45:	sThing = "a village";				break;
					case 46:	sThing = "a fort";					break;
					case 47:	sThing = "a dungeon";				break;
					case 48:	sThing = "a cave";					break;
					case 49:	sThing = "a cemetery";				break;
					case 50:	sThing = "a tomb";					break;
					case 51:	sThing = "a crypt";					break;
				}
				from.PrivateOverheadMessage(MessageType.Regular, 0x14C, false, "Within in the ball you can see " + sThing + ".", from.NetState);
			}
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

		public DDRelicOrbs(Serial serial) : base(serial)
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