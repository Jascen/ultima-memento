using Server.Engines.Craft;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.GlobalShoppe
{
	[Flipable(0x3CEB, 0x3CEC)]
	public class LibrarianShoppe : CustomerOrderShoppe<OrderContext>
	{
		[Constructable]
		public LibrarianShoppe() : base(0x3CEB)
		{
			Name = "Librarian Work Shoppe";
		}

		public LibrarianShoppe(Serial serial) : base(serial)
		{
		}

		public override CraftSystem CraftSystem { get { return DefInscription.CraftSystem; } }
		public override NpcGuild Guild { get { return NpcGuild.LibrariansGuild; } }
		public override ShoppeType ShoppeType { get { return ShoppeType.Librarian; } }

		protected override SkillName PrimarySkill { get { return SkillName.Inscribe; } }

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if (dropped is ScribesPen)
				return AddTools(from, dropped);
			if (dropped is BlankScroll)
				return AddResource(from, dropped);

			return base.OnDragDrop(from, dropped);
		}

		public override void PrepareOrders(TradeSkillContext context)
		{
			context.Orders.ForEach(untypedOrder =>
			{
				var order = untypedOrder as OrderContext;
				if (order == null)
				{
					Console.WriteLine("Failed to set Librarian rewards for order ({0})", untypedOrder.GetType().Name);
					return;
				}

				if (order.IsInitialized) return;

				var rewards = LibrarianRewardCalculator.Instance;
				rewards.SetRewards(context, order);

				var item = Misc.Cache.ItemSnapshotCache.GetOrCreate(order.Type);
				order.GraphicId = item.ItemID;
				order.ItemName = item.Name;
				order.Person = CreatePersonName();

				order.IsInitialized = true;
			});
		}

		protected override IEnumerable<OrderContext> CreateOrders(CraftSystem craftSystem, Mobile from, TradeSkillContext context, int count)
		{
			if ((from is PlayerMobile) == false) yield break;
			if (count < 1) yield break;

			// Build item list
			var player = (PlayerMobile)from;
			var items = GetCraftItems(from, craftSystem)
				.Where(craftItem =>
				{
					if (!TypeUtilities.IsTypeOrDerived<SpellScroll>(craftItem.ItemType)) return false;
					if (craftItem.Recipe != null && !player.HasRecipe(craftItem.Recipe)) return false;

					// Arcane Scrolls are a bit too difficult to farm, so exclude anything that has them in their recipe
					foreach (var o in craftItem.Resources)
					{
						var res = o as CraftRes;
						if (res == null) continue;
						if (res.ItemType == typeof(ArcaneScroll)) return false;
					}

					return true;
				})
				.ToList();
			if (items.Count < 1) yield break;

			// Add 5x quantity bonus for every 5 points over 100
			var amountBonus = 5 * (int)(Math.Max(0, from.Skills[craftSystem.MainSkill].Value - 100) / 5);

			for (int i = 0; i < count; i++)
			{
				var item = Utility.Random(items);
				if (item == null) yield break;

				var amount = amountBonus + Utility.RandomMinMax(15, 40);

				var order = new OrderContext(item.ItemType)
				{
					MaxAmount = amount,
					CurrentAmount = 0,
				};

				yield return order;
			}
		}

		protected override string CreateTask(TradeSkillContext context)
		{
			string task = null;

			int category = Utility.RandomMinMax(1, 4);

			if (category == 1)
			{
				switch (Utility.RandomMinMax(0, 5))
				{
					case 0: task = "scroll"; break;
					case 1: task = "parchment"; break;
					case 2: task = "tablet"; break;
					case 3: task = "manuscript"; break;
					case 4: task = "document"; break;
					case 5: task = "paper"; break;
				}

				switch (Utility.RandomMinMax(0, 7))
				{
					case 0: task = "Translate this " + task + " of "; break;
					case 1: task = "Decipher this " + task + " of "; break;
					case 2: task = "Copy this " + task + " of "; break;
					case 3: task = "Encode this " + task + " of "; break;
					case 4: task = "Decode this " + task + " of "; break;
					case 5: task = "Verify this " + task + " of "; break;
					case 6: task = "Validate this " + task + " of "; break;
					case 7: task = "Research this " + task + " of "; break;
				}

				string[] vSpell1 = new string[] { "Clyz", "Achug", "Theram", "Quale", "Lutin", "Gad", "Croeq", "Achund", "Therrisi", "Qualorm", "Lyeit", "Garaso", "Crul", "Ackhine", "Thritai", "Quaso", "Lyetonu", "Garck", "Cuina", "Ackult", "Tig", "Quealt", "Moin", "Garund", "Daror", "Aeny", "Tinalt", "Rador", "Moragh", "Ghagha", "Deet", "Aeru", "Tinkima", "Rakeld", "Morir", "Ghatas", "Deldrad", "Ageick", "Tinut", "Rancwor", "Morosy", "Gosul", "Deldrae", "Agemor", "Tonk", "Ranildu", "Mosat", "Hatalt", "Delz", "Aghai", "Tonolde", "Ranot", "Mosd", "Hatash", "Denad", "Ahiny", "Tonper", "Ranper", "Mosrt", "Hatque", "Denold", "Aldkely", "Torint", "Ransayi", "Mosyl", "Hatskel", "Denyl", "Aleler", "Trooph", "Ranzmor", "Moszight", "Hattia", "Drahono", "Anagh", "Turbelm", "Raydan", "Naldely", "Hiert", "Draold", "Anclor", "Uighta", "Rayxwor", "Nalusk", "Hinalde", "Dynal", "Anl", "Uinga", "Rhit", "Nalwar", "Hinall", "Dyndray", "Antack", "Umnt", "Risormy", "Nag", "Hindend", "Eacki", "Ardburo", "Undaughe", "Risshy", "Nat", "Iade", "Earda", "Ardmose", "Untdran", "Rodiz", "Nator", "Iaper", "Echal", "Ardurne", "Untld", "Rodkali", "Nayth", "Iass", "Echind", "Ardyn", "Uoso", "Rodrado", "Neil", "Iawy", "Echwaro", "Ashaugha", "Urnroth", "Roort", "Nenal", "Iechi", "Eeni", "Ashdend", "Urode", "Ruina", "Newl", "Ightult", "Einea", "Ashye", "Uskdar", "Rynm", "Nia", "Ildaw", "Eldsera", "Asim", "Uskmdan", "Rynryna", "Nikim", "Ildoq", "Eldwen", "Athdra", "Usksough", "Ryns", "Nof", "Inabel", "Eldyril", "Athskel", "Usktoro", "Rynut", "Nook", "Inaony", "Elmkach", "Atkin", "Ustagee", "Samgha", "Nybage", "Inease", "Elmll", "Aughint", "Ustld", "Samnche", "Nyiy", "Ineegh", "Emath", "Aughthere", "Ustton", "Samssam", "Nyseld", "Ineiti", "Emengi", "Avery", "Verporm", "Sawor", "Nysklye", "Ineun", "Emild", "Awch", "Vesrade", "Sayimo", "Nyw", "Ingr", "Emmend", "Banend", "Voraughe", "Sayn", "Oasho", "Isbaugh", "Emnden", "Beac", "Vorril", "Sayskelu", "Oendy", "Islyei", "Endvelm", "Belan", "Vorunt", "Scheach", "Oenthi", "Issy", "Endych", "Beloz", "Whedan", "Scheyer", "Ohato", "Istin", "Engeh", "Beltiai", "Whisam", "Serat", "Oldack", "Iumo", "Engen", "Bliorm", "Whok", "Sernd", "Oldar", "Jyhin", "Engh", "Burold", "Worath", "Skell", "Oldr", "Jyon", "Engraki", "Buror", "Worav", "Skelser", "Oldtar", "Kalov", "Engroth", "Byt", "Worina", "Slim", "Omdser", "Kelol", "Engum", "Cakal", "Worryno", "Snaest", "Ond", "Kinser", "Enhech", "Carr", "Worunty", "Sniund", "Oron", "Koor", "Enina", "Cayld", "Worwaw", "Sosam", "Orrbel", "Lear", "Enk", "Cerar", "Yary", "Stayl", "Osnt", "Leert", "Enlald", "Cerl", "Yawi", "Stol", "Peright", "Legar", "Enskele", "Cerv", "Yena", "Strever", "Perpban", "Lerev", "Eoru", "Chaur", "Yero", "Swaih", "Phiunt", "Lerzshy", "Ernysi", "Chayn", "Yerrves", "Tagar", "Poll", "Llash", "Erque", "Cheimo", "Yhone", "Taienn", "Polrad", "Llotor", "Errusk", "Chekim", "Yradi", "Taiyild", "Polsera", "Loem", "Ervory", "Chreusk", "Zhugar", "Tanen", "Puon", "Loing", "Essisi", "Chrir", "Zirt", "Tasaf", "Quaev", "Lorelmo", "Essnd", "Chroelt", "Zoine", "Tasrr", "Quahang", "Lorud", "Estech", "Cloran", "Zotin", "Thaeng", "Qual", "Lour", "Estkunt", "Etoth", "Esule", "Estnight" };
				string sSpell1 = vSpell1[Utility.RandomMinMax(0, (vSpell1.Length - 1))];

				string[] vSpell2 = new string[] { "Acidic", "Summoning", "Scrying", "Obscure", "Iron", "Ghoulish", "Enfeebling", "Altered", "Secret", "Obscuring", "Irresistible", "Gibbering", "Enlarged", "Confusing", "Analyzing", "Sympathetic", "Secure", "Permanent", "Keen", "Glittering", "Ethereal", "Contacting", "Animal", "Telekinetic", "Seeming", "Persistent", "Lawful", "Evil", "Continual", "Animated", "Telepathic", "Shadow", "Phantasmal", "Legendary", "Good", "Expeditious", "Control", "Antimagic", "Teleporting", "Shattering", "Phantom", "Lesser", "Grasping", "Explosive", "Crushing", "Arcane", "Temporal", "Shocking", "Phasing", "Levitating", "Greater", "Fabricated", "Cursed", "Articulated", "Tiny", "Shouting", "Planar", "Limited", "Guarding", "Faithful", "Dancing", "Binding", "Transmuting", "Shrinking", "Poisonous", "Lucubrating", "Fearful", "Dazzling", "Black", "Undead", "Silent", "Polymorphing", "Magical", "Hallucinatory", "Delayed", "Blinding", "Undetectable", "Slow", "Prismatic", "Magnificent", "Hideous", "Fire", "Demanding", "Blinking", "Unseen", "Solid", "Programmed", "Major", "Holding", "Flaming", "Dimensional", "Vampiric", "Soul", "Projected", "Mass", "Horrid", "Discern", "Burning", "Vanishing", "Spectral", "Mending", "Hypnotic", "Floating", "Disintegrating", "Cat", "Protective", "Mind", "Ice", "Flying", "Disruptive", "Chain", "Spidery", "Prying", "Minor", "Illusionary", "Force", "Dominating", "Changing", "Warding", "Stinking", "Pyrotechnic", "Mirrored", "Improved", "Forceful", "Dreaming", "Chaotic", "Water", "Stone", "Rainbow", "Misdirected", "Incendiary", "Freezing", "Elemental", "Charming", "Watery", "Misleading", "Instant", "Gaseous", "Emotional", "Chilling", "Weird", "Storming", "Resilient", "Mnemonic", "Interposing", "Gentle", "Enduring", "Whispering", "Suggestive", "Reverse", "Moving", "Invisible", "Ghostly", "Energy", "Clenched", "Climbing", "Comprehending", "Colorful", "True", "False" };
				string sSpell2 = vSpell2[Utility.RandomMinMax(0, (vSpell2.Length - 1))];

				string[] vSpell3 = new string[] { "Acid", "Tentacles", "Sigil", "Plane", "Legend", "Gravity", "Emotion", "Chest", "Alarm", "Terrain", "Simulacrum", "Poison", "Lightning", "Grease", "Endurance", "Circle", "Anchor", "Thoughts", "Skin", "Polymorph", "Lights", "Growth", "Enervation", "Clairvoyance", "Animal", "Time", "Sleep", "Prestidigitation", "Location", "Guards", "Enfeeblement", "Clone", "Antipathy", "Tongues", "Soul", "Projection", "Lock", "Hand", "Enhancer", "Cloud", "Arcana", "Touch", "Sound", "Pyrotechnics", "Lore", "Haste", "Etherealness", "Cold", "Armor", "Transformation", "Spells", "Refuge", "Lucubration", "Hat", "Evil", "Color", "Arrows", "Trap", "Sphere", "Repulsion", "Magic", "Hound", "Evocation", "Confusion", "Aura", "Trick", "Spider", "Resistance", "Mansion", "Hypnotism", "Eye", "Conjuration", "Banishment", "Turning", "Spray", "Retreat", "Mask", "Ice", "Fall", "Contagion", "Banshee", "Undead", "Stasis", "Rope", "Maze", "Image", "Fear", "Creation", "Bear", "Vanish", "Statue", "Runes", "Message", "Imprisonment", "Feather", "Curse", "Binding", "Veil", "Steed", "Scare", "Meteor", "Insanity", "Field", "Dance", "Vision", "Stone", "Screen", "Mind", "Invisibility", "Fireball", "Darkness", "Blindness", "Vocation", "Storm", "Script", "Mirage", "Invulnerability", "Flame", "Daylight", "Blink", "Wail", "Strength", "Scrying", "Misdirection", "Iron", "Flesh", "Dead", "Blur", "Walk", "Strike", "Seeing", "Missile", "Item", "Fog", "Deafness", "Body", "Wall", "Stun", "Self", "Mist", "Jar", "Force", "Death", "Bolt", "Wards", "Suggestion", "Sending", "Monster", "Jaunt", "Foresight", "Demand", "Bond", "Water", "Summons", "Servant", "Mouth", "Jump", "Form", "Disjunction", "Breathing", "Weapon", "Sunburst", "Shadow", "Mud", "Kill", "Freedom", "Disk", "Burning", "Weather", "Swarm", "Shape", "Nightmare", "Killer", "Frost", "Dismissal", "Cage", "Web", "Symbol", "Shelter", "Object", "Knock", "Gate", "Displacement", "Chain", "Wilting", "Sympathy", "Shield", "Page", "Languages", "Good", "Door", "Chaos", "Wind", "Telekinesis", "Shift", "Pattern", "Laughter", "Grace", "Drain", "Charm", "Wish", "Teleport", "Shout", "Person", "Law", "Grasp", "Dream", "Elements", "Edge", "Earth", "Dust" };
				string sSpell3 = vSpell3[Utility.RandomMinMax(0, (vSpell3.Length - 1))];

				task = task + sSpell1 + "'s Spell of " + sSpell2 + " " + sSpell3;
			}
			else if (category == 2)
			{
				switch (Utility.RandomMinMax(0, 6))
				{
					case 0: task = "a book"; break;
					case 1: task = "a lexicon"; break;
					case 2: task = "an omnibus"; break;
					case 3: task = "a manual"; break;
					case 4: task = "a folio"; break;
					case 5: task = "a codex"; break;
					case 6: task = "a tome"; break;
				}

				string build = "put them";
				switch (Utility.RandomMinMax(0, 6))
				{
					case 1: build = "bind them"; break;
					case 2: build = "combine them"; break;
					case 3: build = "assemble them"; break;
					case 4: build = "organize them"; break;
					case 5: build = "make them"; break;
					case 6: build = "write them"; break;
				}

				switch (Utility.RandomMinMax(0, 4))
				{
					case 0: task = "Take these laws from " + Server.Misc.RandomThings.MadeUpCity() + " and " + build + " into " + task; break;
					case 1: task = "Take these stories from the " + RandomThings.GetRandomKingdomName() + " " + RandomThings.GetRandomKingdom() + " and " + build + " into " + task; break;
					case 2: task = "Take these journals from my adventures in " + Server.Misc.RandomThings.MadeUpDungeon() + " and " + build + " into " + task; break;
					case 3: task = "Take these notes from " + Server.Misc.RandomThings.MadeUpDungeon() + " and " + build + " into " + task; break;
					case 4: task = "Take these records from " + Server.Misc.RandomThings.MadeUpCity() + " and " + build + " into " + task; break;
				}
			}
			else if (category == 3)
			{
				switch (Utility.RandomMinMax(0, 6))
				{
					case 0: task = "book"; break;
					case 1: task = "lexicon"; break;
					case 2: task = "omnibus"; break;
					case 3: task = "manual"; break;
					case 4: task = "folio"; break;
					case 5: task = "codex"; break;
					case 6: task = "tome"; break;
				}

				switch (Utility.RandomMinMax(0, 7))
				{
					case 0: task = "Translate this " + task; break;
					case 1: task = "Decipher this " + task; break;
					case 2: task = "Copy this " + task; break;
					case 3: task = "Encode this " + task; break;
					case 4: task = "Decode this " + task; break;
					case 5: task = "Verify this " + task; break;
					case 6: task = "Validate this " + task; break;
					case 7: task = "Research this " + task; break;
				}

				string origin = Server.Misc.RandomThings.MadeUpCity();
				switch (Utility.RandomMinMax(0, 2))
				{
					case 1: origin = Server.Misc.RandomThings.MadeUpDungeon(); break;
					case 2: origin = "the " + RandomThings.GetRandomKingdomName() + " " + RandomThings.GetRandomKingdom(); break;
				}

				switch (Utility.RandomMinMax(0, 2))
				{
					case 0: task = task + " titled '" + Server.Misc.RandomThings.GetBookTitle() + "'"; break;
					case 1: task = task + " from " + Server.Misc.RandomThings.GetBookTitle(); break;
					case 2: task = task + " about " + Server.Misc.RandomThings.GetBookTitle(); break;
				}
			}
			else
			{
				Item book = null;
				switch (Utility.RandomMinMax(0, 6))
				{
					case 0: book = new NecromancerSpellbook(); task = book.Name; book.Delete(); break;
					case 1: book = new Spellbook(); task = book.Name; book.Delete(); break;
					case 2: book = new BookOfNinjitsu(); task = book.Name; book.Delete(); break;
					case 3: book = new BookOfBushido(); task = book.Name; book.Delete(); break;
					case 4: book = new BookOfChivalry(); task = book.Name; book.Delete(); break;
					case 5: book = new SongBook(); task = book.Name; book.Delete(); break;
					case 6: book = new ElementalSpellbook(); task = book.Name; book.Delete(); break;
				}

				switch (Utility.RandomMinMax(0, 7))
				{
					case 0: task = "Translate " + task; break;
					case 1: task = "Decipher " + task; break;
					case 2: task = "Copy " + task; break;
					case 3: task = "Encode " + task; break;
					case 4: task = "Decode " + task; break;
					case 5: task = "Verify " + task; break;
					case 6: task = "Validate " + task; break;
					case 7: task = "Research " + task; break;
				}
			}

			return task;
		}

		protected override string GetDescription(OrderContext order)
		{
			var description = string.Format("Craft {0}", order.MaxAmount);

			description = string.Format("{0} {1}", description, order.ItemName);

			return description;
		}

		protected override ShoppeGump GetGump(PlayerMobile from)
		{
			var context = GetOrCreateContext(from);
			PrepareOrders(context);

			return new ShoppeGump(
				from,
				this,
				context,
				"LIBRARIAN WORK SHOPPE",
				"Scribe Quills",
				"Blank Scrolls"
			);
		}

		protected override void OnJobFailed(Mobile from, TradeSkillContext context, CustomerContext customer)
		{
			base.OnJobFailed(from, context, customer);

			from.SendSound(0x249); // Scribe
		}

		protected override void OnJobSuccess(Mobile from, TradeSkillContext context, CustomerContext customer)
		{
			base.OnJobSuccess(from, context, customer);

			from.SendSound(0x249); // Scribe
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}
	}
}