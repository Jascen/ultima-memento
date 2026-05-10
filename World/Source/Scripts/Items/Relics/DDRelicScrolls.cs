using System;
using Server;
using System.Collections;
using Server.Mobiles;

namespace Server.Items
{
	public class DDRelicScrolls : Item, IRelic
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
		public DDRelicScrolls() : base( 0x227B )
		{
			Weight = 1;
			CoinPrice = Utility.RandomMinMax( 80, 500 );
			NotIdentified = true;
			NotIDSource = Identity.Scroll;
			NotIDSkill = IDSkill.Mercantile;
			ItemID = Utility.RandomList( 0x227B, 0x227A, 0x2C94, 0x2272, 0x2278, 0x2273, 0x2279, 0x46AE, 0x46AF );
			Hue = Utility.RandomColor(0);

			string[] vSpell1 = new string[] {"Clyz", "Achug", "Theram", "Quale", "Lutin", "Gad", "Croeq", "Achund", "Therrisi", "Qualorm", "Lyeit", "Garaso", "Crul", "Ackhine", "Thritai", "Quaso", "Lyetonu", "Garck", "Cuina", "Ackult", "Tig", "Quealt", "Moin", "Garund", "Daror", "Aeny", "Tinalt", "Rador", "Moragh", "Ghagha", "Deet", "Aeru", "Tinkima", "Rakeld", "Morir", "Ghatas", "Deldrad", "Ageick", "Tinut", "Rancwor", "Morosy", "Gosul", "Deldrae", "Agemor", "Tonk", "Ranildu", "Mosat", "Hatalt", "Delz", "Aghai", "Tonolde", "Ranot", "Mosd", "Hatash", "Denad", "Ahiny", "Tonper", "Ranper", "Mosrt", "Hatque", "Denold", "Aldkely", "Torint", "Ransayi", "Mosyl", "Hatskel", "Denyl", "Aleler", "Trooph", "Ranzmor", "Moszight", "Hattia", "Drahono", "Anagh", "Turbelm", "Raydan", "Naldely", "Hiert", "Draold", "Anclor", "Uighta", "Rayxwor", "Nalusk", "Hinalde", "Dynal", "Anl", "Uinga", "Rhit", "Nalwar", "Hinall", "Dyndray", "Antack", "Umnt", "Risormy", "Nag", "Hindend", "Eacki", "Ardburo", "Undaughe", "Risshy", "Nat", "Iade", "Earda", "Ardmose", "Untdran", "Rodiz", "Nator", "Iaper", "Echal", "Ardurne", "Untld", "Rodkali", "Nayth", "Iass", "Echind", "Ardyn", "Uoso", "Rodrado", "Neil", "Iawy", "Echwaro", "Ashaugha", "Urnroth", "Roort", "Nenal", "Iechi", "Eeni", "Ashdend", "Urode", "Ruina", "Newl", "Ightult", "Einea", "Ashye", "Uskdar", "Rynm", "Nia", "Ildaw", "Eldsera", "Asim", "Uskmdan", "Rynryna", "Nikim", "Ildoq", "Eldwen", "Athdra", "Usksough", "Ryns", "Nof", "Inabel", "Eldyril", "Athskel", "Usktoro", "Rynut", "Nook", "Inaony", "Elmkach", "Atkin", "Ustagee", "Samgha", "Nybage", "Inease", "Elmll", "Aughint", "Ustld", "Samnche", "Nyiy", "Ineegh", "Emath", "Aughthere", "Ustton", "Samssam", "Nyseld", "Ineiti", "Emengi", "Avery", "Verporm", "Sawor", "Nysklye", "Ineun", "Emild", "Awch", "Vesrade", "Sayimo", "Nyw", "Ingr", "Emmend", "Banend", "Voraughe", "Sayn", "Oasho", "Isbaugh", "Emnden", "Beac", "Vorril", "Sayskelu", "Oendy", "Islyei", "Endvelm", "Belan", "Vorunt", "Scheach", "Oenthi", "Issy", "Endych", "Beloz", "Whedan", "Scheyer", "Ohato", "Istin", "Engeh", "Beltiai", "Whisam", "Serat", "Oldack", "Iumo", "Engen", "Bliorm", "Whok", "Sernd", "Oldar", "Jyhin", "Engh", "Burold", "Worath", "Skell", "Oldr", "Jyon", "Engraki", "Buror", "Worav", "Skelser", "Oldtar", "Kalov", "Engroth", "Byt", "Worina", "Slim", "Omdser", "Kelol", "Engum", "Cakal", "Worryno", "Snaest", "Ond", "Kinser", "Enhech", "Carr", "Worunty", "Sniund", "Oron", "Koor", "Enina", "Cayld", "Worwaw", "Sosam", "Orrbel", "Lear", "Enk", "Cerar", "Yary", "Stayl", "Osnt", "Leert", "Enlald", "Cerl", "Yawi", "Stol", "Peright", "Legar", "Enskele", "Cerv", "Yena", "Strever", "Perpban", "Lerev", "Eoru", "Chaur", "Yero", "Swaih", "Phiunt", "Lerzshy", "Ernysi", "Chayn", "Yerrves", "Tagar", "Poll", "Llash", "Erque", "Cheimo", "Yhone", "Taienn", "Polrad", "Llotor", "Errusk", "Chekim", "Yradi", "Taiyild", "Polsera", "Loem", "Ervory", "Chreusk", "Zhugar", "Tanen", "Puon", "Loing", "Essisi", "Chrir", "Zirt", "Tasaf", "Quaev", "Lorelmo", "Essnd", "Chroelt", "Zoine", "Tasrr", "Quahang", "Lorud", "Estech", "Cloran", "Zotin", "Thaeng", "Qual", "Lour", "Estkunt", "Etoth", "Esule", "Estnight"};
				string sSpell1 = vSpell1[Utility.RandomMinMax( 0, (vSpell1.Length-1) )];

			string[] vSpell2 = new string[] {"Acidic", "Summoning", "Scrying", "Obscure", "Iron", "Ghoulish", "Enfeebling", "Altered", "Secret", "Obscuring", "Irresistible", "Gibbering", "Enlarged", "Confusing", "Analyzing", "Sympathetic", "Secure", "Permanent", "Keen", "Glittering", "Ethereal", "Contacting", "Animal", "Telekinetic", "Seeming", "Persistent", "Lawful", "Evil", "Continual", "Animated", "Telepathic", "Shadow", "Phantasmal", "Legendary", "Good", "Expeditious", "Control", "Antimagic", "Teleporting", "Shattering", "Phantom", "Lesser", "Grasping", "Explosive", "Crushing", "Arcane", "Temporal", "Shocking", "Phasing", "Levitating", "Greater", "Fabricated", "Cursed", "Articulated", "Tiny", "Shouting", "Planar", "Limited", "Guarding", "Faithful", "Dancing", "Binding", "Transmuting", "Shrinking", "Poisonous", "Lucubrating", "Fearful", "Dazzling", "Black", "Undead", "Silent", "Polymorphing", "Magical", "Hallucinatory", "Delayed", "Blinding", "Undetectable", "Slow", "Prismatic", "Magnificent", "Hideous", "Fire", "Demanding", "Blinking", "Unseen", "Solid", "Programmed", "Major", "Holding", "Flaming", "Dimensional", "Vampiric", "Soul", "Projected", "Mass", "Horrid", "Discern", "Burning", "Vanishing", "Spectral", "Mending", "Hypnotic", "Floating", "Disintegrating", "Cat", "Protective", "Mind", "Ice", "Flying", "Disruptive", "Chain", "Spidery", "Prying", "Minor", "Illusionary", "Force", "Dominating", "Changing", "Warding", "Stinking", "Pyrotechnic", "Mirrored", "Improved", "Forceful", "Dreaming", "Chaotic", "Water", "Stone", "Rainbow", "Misdirected", "Incendiary", "Freezing", "Elemental", "Charming", "Watery", "Misleading", "Instant", "Gaseous", "Emotional", "Chilling", "Weird", "Storming", "Resilient", "Mnemonic", "Interposing", "Gentle", "Enduring", "Whispering", "Suggestive", "Reverse", "Moving", "Invisible", "Ghostly", "Energy", "Clenched", "Climbing", "Comprehending", "Colorful", "True", "False"};
				string sSpell2 = vSpell2[Utility.RandomMinMax( 0, (vSpell2.Length-1) )];

			string[] vSpell3 = new string[] {"Acid", "Tentacles", "Sigil", "Plane", "Legend", "Gravity", "Emotion", "Chest", "Alarm", "Terrain", "Simulacrum", "Poison", "Lightning", "Grease", "Endurance", "Circle", "Anchor", "Thoughts", "Skin", "Polymorph", "Lights", "Growth", "Enervation", "Clairvoyance", "Animal", "Time", "Sleep", "Prestidigitation", "Location", "Guards", "Enfeeblement", "Clone", "Antipathy", "Tongues", "Soul", "Projection", "Lock", "Hand", "Enhancer", "Cloud", "Arcana", "Touch", "Sound", "Pyrotechnics", "Lore", "Haste", "Etherealness", "Cold", "Armor", "Transformation", "Spells", "Refuge", "Lucubration", "Hat", "Evil", "Color", "Arrows", "Trap", "Sphere", "Repulsion", "Magic", "Hound", "Evocation", "Confusion", "Aura", "Trick", "Spider", "Resistance", "Mansion", "Hypnotism", "Eye", "Conjuration", "Banishment", "Turning", "Spray", "Retreat", "Mask", "Ice", "Fall", "Contagion", "Banshee", "Undead", "Stasis", "Rope", "Maze", "Image", "Fear", "Creation", "Bear", "Vanish", "Statue", "Runes", "Message", "Imprisonment", "Feather", "Curse", "Binding", "Veil", "Steed", "Scare", "Meteor", "Insanity", "Field", "Dance", "Vision", "Stone", "Screen", "Mind", "Invisibility", "Fireball", "Darkness", "Blindness", "Vocation", "Storm", "Script", "Mirage", "Invulnerability", "Flame", "Daylight", "Blink", "Wail", "Strength", "Scrying", "Misdirection", "Iron", "Flesh", "Dead", "Blur", "Walk", "Strike", "Seeing", "Missile", "Item", "Fog", "Deafness", "Body", "Wall", "Stun", "Self", "Mist", "Jar", "Force", "Death", "Bolt", "Wards", "Suggestion", "Sending", "Monster", "Jaunt", "Foresight", "Demand", "Bond", "Water", "Summons", "Servant", "Mouth", "Jump", "Form", "Disjunction", "Breathing", "Weapon", "Sunburst", "Shadow", "Mud", "Kill", "Freedom", "Disk", "Burning", "Weather", "Swarm", "Shape", "Nightmare", "Killer", "Frost", "Dismissal", "Cage", "Web", "Symbol", "Shelter", "Object", "Knock", "Gate", "Displacement", "Chain", "Wilting", "Sympathy", "Shield", "Page", "Languages", "Good", "Door", "Chaos", "Wind", "Telekinesis", "Shift", "Pattern", "Laughter", "Grace", "Drain", "Charm", "Wish", "Teleport", "Shout", "Person", "Law", "Grasp", "Dream", "Elements", "Edge", "Earth", "Dust"};
				string sSpell3 = vSpell3[Utility.RandomMinMax( 0, (vSpell3.Length-1) )];

			Name = sSpell1 + "'s Spell of " + sSpell2 + " " + sSpell3;
		}

		public DDRelicScrolls(Serial serial) : base(serial)
		{
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