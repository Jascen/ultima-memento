using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.Craft;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

namespace Server.Engines.GlobalShoppe
{
    [Flipable(0x19FF, 0x1A00)]
    public class MorticianShoppe : CustomerOrderShoppe<OrderContext>
    {
        [Constructable]
        public MorticianShoppe() : base(0x19FF)
        {
            Name = "Mortician Work Shoppe";
        }

        public MorticianShoppe(Serial serial) : base(serial)
        {
        }

        public override NpcGuild Guild { get { return NpcGuild.NecromancersGuild; } }

        protected override SkillName PrimarySkill { get { return SkillName.Necromancy; } }
        protected override ShoppeType ShoppeType { get { return ShoppeType.Mortician; } }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is WitchCauldron)
                return AddTools(from, dropped);
            if (dropped.Catalog == Catalogs.Reagent && WitchPouch.isWitchery(dropped))
                return AddResource(from, dropped);

            return base.OnDragDrop(from, dropped);
        }

        protected override IEnumerable<OrderContext> CreateOrders(Mobile from, TradeSkillContext context, int count)
        {
            if (count < 1) yield break;

            var craftSystem = DefWitchery.CraftSystem;

            // Build item list
            var items = GetCraftItems(from, craftSystem)
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

            if (Utility.RandomMinMax(1, 4) == 1)
            {
                string bodyName = NameList.RandomName("male");
                if (Utility.RandomBool()) { bodyName = NameList.RandomName("female"); }

                string bodyTitle = TavernPatrons.GetTitle();

                switch (Utility.RandomMinMax(1, 3))
                {
                    case 1: task = "corpse"; break;
                    case 2: task = "remains"; break;
                    case 3: task = "body"; break;
                }

                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1: task = "Do an autopsy on the " + task; break;
                    case 2: task = "Do an examination of the " + task; break;
                    case 3: task = "Look over the " + task; break;
                    case 4: task = "Do a postmortem on the " + task; break;
                    case 5: task = "Do a necropsy on the " + task; break;
                }

                task = task + " of " + bodyName + " " + bodyTitle;

                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1: task += " to determine the cause of death"; break;
                    case 2: task += " to find the cause of death"; break;
                    case 3: task += " to identify the murderer"; break;
                    case 4: task += " to solve who the killer is"; break;
                    case 5: task += " to see how they died"; break;
                }
            }
            else
            {
                switch (Utility.RandomMinMax(1, 4))
                {
                    case 1: task = "Brew"; break;
                    case 2: task = "Create"; break;
                    case 3: task = "Concoct"; break;
                    case 4: task = "Boil"; break;
                }

                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1: task += " an elixir"; break;
                    case 2: task += " a potion"; break;
                    case 3: task += " a draught"; break;
                    case 4: task += " a mixture"; break;
                    case 5: task += " a philter"; break;
                }

                switch (Utility.RandomMinMax(1, 4))
                {
                    case 1: task += " with "; break;
                    case 2: task += " using "; break;
                    case 3: task += " mixing "; break;
                    case 4: task += " combining "; break;
                }

                string[] sWeirds = new string[] { "ant", "ape", "baboon", "badger", "basilisk", "bear", "beaver", "beetle", "beholder", "boar", "brownie", "buffalo", "bull", "camel", "centaur", "centipede", "chimera", "cockatrice", "crocodile", "deer", "demon", "devil", "dinosaur", "djinni", "dog", "dragon", "dryad", "dwarf", "eagle", "efreet", "elemental", "elephant", "elf", "ettin", "frog", "fungi", "gargoyle", "ghast", "ghost", "ghoul", "giant", "gnoll", "gnome", "goat", "goblin", "golem", "gorgon", "griffon", "hag", "halfling", "harpy", "hell hound", "hippogriff", "hippopotamus", "hobgoblin", "horse", "hydra", "hyena", "imp", "jackal", "jaguar", "ki-rin", "kobold", "leopard", "leprechaun", "lich", "lion", "lizard", "lizardman", "lycanthrope", "lynx", "mammoth", "manticore", "mastodon", "medusa", "minotaur", "mule", "mummy", "naga", "nightmare", "ogre", "orc", "owl", "pegasus", "pixie", "porcupine", "ram", "rat", "reaper", "rhinoceros", "roc", "satyr", "scorpion", "serpent", "shadow", "skeleton", "skunk", "snake", "spectre", "sphinx", "spider", "sprite", "stag", "tiger", "titan", "toad", "troglodyte", "troll", "unicorn", "vampire", "weasel", "wight", "wisp", "wolf", "wolverine", "worm", "wraith", "wyvern", "yeti", "zombie", "zorn" };
                string sWeird = sWeirds[Utility.RandomMinMax(0, (sWeirds.Length - 1))];

                string[] sFluids = new string[] { "blood ", "eyes ", "skin ", "spit ", "bile ", "secretion ", "ooze ", "pus ", "droppings ", "urine ", "slime ", "salt " };
                string sFluid = sFluids[Utility.RandomMinMax(0, (sFluids.Length - 1))];

                string[] sTypes = new string[] { "abysmal essence ", "angelic feathers ", "animal tongues ", "ape ears ", "bat ears ", "bear hairs ", "bird beaks ", "bone powder ", "bony horns ", "cat whiskers ", "centaur fingers ", "cow eyes ", "crab meat ", "crushed gems ", "crushed stone ", "crystal shards ", "cursed leaves ", "darkness ", "dead skin ", "demonic hellfire ", "dog hairs ", "dolphin teeth ", "dragon smoke ", "dried blood ", "dryad tears ", "electricity ", "elemental powder ", "elven blood ", "enchanted frost ", "enchanted sap ", "entrails ", "fish scales ", "frog tongues ", "gargoyle horns ", "gazing eyes ", "ghostly mist ", "giant blood ", "entrails ", "gore ", "hellish smoke ", "horrid breath ", "human blood ", "hydra urine ", "illithid brains ", "imp tails ", "ink ", "insect ichor ", "ivory pieces ", "jade chunks ", "large teeth ", "leech spit ", "liquid fire ", "magical ashes ", "magical dust ", "minotaur hooves ", "mummy wraps ", "mystical air ", "mystical dirt ", "mystical mud ", "ogre thumbs ", "oil ", "oni fur ", "orcish bile ", "ostard scales ", "pig snouts ", "pixie sparkles ", "poisonous gas ", "quills ", "rat tails ", "reptile scales ", "scaly fingers ", "scorpion stingers ", "sea water ", "silver shavings ", "slime ", "sphinx fur ", "spider legs ", "sprite teeth ", "succubus pheromones ", "swamp gas ", "troll claws ", "unicorn teeth ", "vampire fangs ", "wax shavings ", "wisp light ", "wood splinters ", "worm guts ", "wyrm spit ", "wyvern poison ", "yeti claws " };
                string sType = sTypes[Utility.RandomMinMax(0, (sTypes.Length - 1))];

                switch (Utility.RandomMinMax(0, 3))
                {
                    case 0: task = task + sWeird + " " + sFluid + "and " + sType + "into a vial of "; break;
                    case 1: task = task + sWeird + " " + sFluid + "and " + sType + "into a bottle of "; break;
                    case 2: task = task + sWeird + " " + sFluid + "and " + sType + "into a flask of "; break;
                    case 3: task = task + sWeird + " " + sFluid + "and " + sType + "into a jar of "; break;
                }

                string[] sMixs = new string[] { "Acidic", "Summoning", "Scrying", "Obscure", "Iron", "Ghoulish", "Enfeebling", "Altered", "Secret", "Obscuring", "Irresistible", "Gibbering", "Enlarged", "Confusing", "Analyzing", "Sympathetic", "Secure", "Permanent", "Keen", "Glittering", "Ethereal", "Contacting", "Animal", "Telekinetic", "Seeming", "Persistent", "Lawful", "Evil", "Continual", "Animated", "Telepathic", "Shadow", "Phantasmal", "Legendary", "Good", "Expeditious", "Control", "Antimagic", "Teleporting", "Shattering", "Phantom", "Lesser", "Grasping", "Explosive", "Crushing", "Arcane", "Temporal", "Shocking", "Phasing", "Levitating", "Greater", "Fabricated", "Cursed", "Articulated", "Tiny", "Shouting", "Planar", "Limited", "Guarding", "Faithful", "Dancing", "Binding", "Transmuting", "Shrinking", "Poisonous", "Lucubrating", "Fearful", "Dazzling", "Black", "Undead", "Silent", "Polymorphing", "Magical", "Hallucinatory", "Delayed", "Blinding", "Undetectable", "Slow", "Prismatic", "Magnificent", "Hideous", "Fire", "Demanding", "Blinking", "Unseen", "Solid", "Programmed", "Major", "Holding", "Flaming", "Dimensional", "Vampiric", "Soul", "Projected", "Mass", "Horrid", "Discern", "Burning", "Vanishing", "Spectral", "Mending", "Hypnotic", "Floating", "Disintegrating", "Cat", "Protective", "Mind", "Ice", "Flying", "Disruptive", "Chain", "Spidery", "Prying", "Minor", "Illusionary", "Force", "Dominating", "Changing", "Warding", "Stinking", "Pyrotechnic", "Mirrored", "Improved", "Forceful", "Dreaming", "Chaotic", "Water", "Stone", "Rainbow", "Misdirected", "Incendiary", "Freezing", "Elemental", "Charming", "Watery", "Misleading", "Instant", "Gaseous", "Emotional", "Chilling", "Weird", "Storming", "Resilient", "Mnemonic", "Interposing", "Gentle", "Enduring", "Whispering", "Suggestive", "Reverse", "Moving", "Invisible", "Ghostly", "Energy", "Clenched", "Climbing", "Comprehending", "Colorful", "True", "False" };
                string sMix = sMixs[Utility.RandomMinMax(0, (sMixs.Length - 1))];

                string[] sEffects = new string[] { "Acid", "Tentacles", "Sigil", "Plane", "Legend", "Gravity", "Emotion", "Chest", "Alarm", "Terrain", "Simulacrum", "Poison", "Lightning", "Grease", "Endurance", "Circle", "Anchor", "Thoughts", "Skin", "Polymorph", "Lights", "Growth", "Enervation", "Clairvoyance", "Animal", "Time", "Sleep", "Prestidigitation", "Location", "Guards", "Enfeeblement", "Clone", "Antipathy", "Tongues", "Soul", "Projection", "Lock", "Hand", "Enhancer", "Cloud", "Arcana", "Touch", "Sound", "Pyrotechnics", "Lore", "Haste", "Etherealness", "Cold", "Armor", "Transformation", "Spells", "Refuge", "Lucubration", "Hat", "Evil", "Color", "Arrows", "Trap", "Sphere", "Repulsion", "Magic", "Hound", "Evocation", "Confusion", "Aura", "Trick", "Spider", "Resistance", "Mansion", "Hypnotism", "Eye", "Conjuration", "Banishment", "Turning", "Spray", "Retreat", "Mask", "Ice", "Fall", "Contagion", "Banshee", "Undead", "Stasis", "Rope", "Maze", "Image", "Fear", "Creation", "Bear", "Vanish", "Statue", "Runes", "Message", "Imprisonment", "Feather", "Curse", "Binding", "Veil", "Steed", "Scare", "Meteor", "Insanity", "Field", "Dance", "Vision", "Stone", "Screen", "Mind", "Invisibility", "Fireball", "Darkness", "Blindness", "Vocation", "Storm", "Script", "Mirage", "Invulnerability", "Flame", "Daylight", "Blink", "Wail", "Strength", "Scrying", "Misdirection", "Iron", "Flesh", "Dead", "Blur", "Walk", "Strike", "Seeing", "Missile", "Item", "Fog", "Deafness", "Body", "Wall", "Stun", "Self", "Mist", "Jar", "Force", "Death", "Bolt", "Wards", "Suggestion", "Sending", "Monster", "Jaunt", "Foresight", "Demand", "Bond", "Water", "Summons", "Servant", "Mouth", "Jump", "Form", "Disjunction", "Breathing", "Weapon", "Sunburst", "Shadow", "Mud", "Kill", "Freedom", "Disk", "Burning", "Weather", "Swarm", "Shape", "Nightmare", "Killer", "Frost", "Dismissal", "Cage", "Web", "Symbol", "Shelter", "Object", "Knock", "Gate", "Displacement", "Chain", "Wilting", "Sympathy", "Shield", "Page", "Languages", "Good", "Door", "Chaos", "Wind", "Telekinesis", "Shift", "Pattern", "Laughter", "Grace", "Drain", "Charm", "Wish", "Teleport", "Shout", "Person", "Law", "Grasp", "Dream", "Elements", "Edge", "Earth", "Dust" };
                string sEffect = sEffects[Utility.RandomMinMax(0, (sEffects.Length - 1))];

                task = task + sMix + " " + sEffect;
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

            // Ensure Orders are configured
            context.Orders.ForEach(untypedOrder =>
            {
                var order = untypedOrder as OrderContext;
                if (order == null)
                {
                    Console.WriteLine("Failed to set Mortician rewards for order ({0})", untypedOrder.GetType().Name);
                    return;
                }

                if (order.IsInitialized) return;

                var rewards = MorticianRewardCalculator.Instance;
                rewards.SetRewards(context, order);

                var item = ShoppeItemCache.GetOrCreate(order.Type);
                order.GraphicId = item.ItemID;
                order.ItemName = item.Name;
                order.Person = CreatePersonName();

                order.IsInitialized = true;
            });

            return new ShoppeGump(
                from,
                this,
                context,
                "WITCHES WORK SHOPPE",
                "Witch Cauldrons",
                "Reagents"
            );
        }

        protected override int GetSkillValue(Mobile from)
        {
            var value = (int)((from.Skills[SkillName.Forensics].Value + from.Skills[SkillName.Necromancy].Value) / 2);

            return value;
        }

        protected override void OnJobFailed(Mobile from, TradeSkillContext context, CustomerContext customer)
        {
            base.OnJobFailed(from, context, customer);

            from.SendSound(0x240); // Liquid
        }

        protected override void OnJobSuccess(Mobile from, TradeSkillContext context, CustomerContext customer)
        {
            base.OnJobSuccess(from, context, customer);

            from.SendSound(0x240); // Liquid
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