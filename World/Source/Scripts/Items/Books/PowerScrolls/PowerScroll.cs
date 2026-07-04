using System;

namespace Server.Items
{
    public class PowerScroll : SpecialScroll
    {
        private static SkillName[] m_Skills = new SkillName[]
            {
                SkillName.Alchemy,
                SkillName.Anatomy,
                SkillName.Druidism,
                SkillName.Mercantile,
                SkillName.ArmsLore,
                SkillName.Parry,
                SkillName.Begging,
                SkillName.Blacksmith,
                SkillName.Bowcraft,
                SkillName.Peacemaking,
                SkillName.Camping,
                SkillName.Carpentry,
                SkillName.Cartography,
                SkillName.Cooking,
                SkillName.Searching,
                SkillName.Discordance,
                SkillName.Psychology,
                SkillName.Healing,
                SkillName.Seafaring,
                SkillName.Forensics,
                SkillName.Herding,
                SkillName.Hiding,
                SkillName.Provocation,
                SkillName.Inscribe,
                SkillName.Lockpicking,
                SkillName.Magery,
                SkillName.MagicResist,
                SkillName.Tactics,
                SkillName.Snooping,
                SkillName.Musicianship,
                SkillName.Poisoning,
                SkillName.Marksmanship,
                SkillName.Spiritualism,
                SkillName.Stealing,
                SkillName.Tailoring,
                SkillName.Taming,
                SkillName.Tasting,
                SkillName.Tinkering,
                SkillName.Tracking,
                SkillName.Veterinary,
                SkillName.Swords,
                SkillName.Bludgeoning,
                SkillName.Fencing,
                SkillName.FistFighting,
                SkillName.Lumberjacking,
                SkillName.Mining,
                SkillName.Meditation,
                SkillName.Stealth,
                SkillName.RemoveTrap,
                SkillName.Necromancy,
                SkillName.Focus,
                SkillName.Knightship,
                SkillName.Bushido,
                SkillName.Ninjitsu,
                SkillName.Elementalism,

				/* Intentionally omit these:
				SkillName.Mysticism,
				SkillName.Imbuing,
				SkillName.Throwing,
				*/
			};

        public PowerScroll() : this(SkillName.Alchemy, 0.0)
        {
        }

        [Constructable]
        public PowerScroll(SkillName skill, double value) : base(skill, value)
        {
            Name = GetSkillDisplayName(skill);
            LootType = LootType.Regular;
            Weight = 0;
            Hue = 0x481;
        }

        public PowerScroll(Serial serial) : base(serial)
        {
        }

        public static SkillName[] Skills { get { return m_Skills; } }
        public override string DefaultTitle { get { return Name; } }
        public override int Message { get { return 1049469; } } /* using a scroll increases the maximum amount of a specific skill or your maximum statistics.
																* When used, the effect is not immediately seen without a gain of points with that skill or statistics.
																* You can view your maximum skill values in your skills window.
																* You can view your maximum statistic value in your statistics window. */
        public override int Title { get { return 0; } }

        public static PowerScroll CreateRandom(int min, int max)
        {
            min /= 5;
            max /= 5;

            return new PowerScroll(Skills[Utility.Random(Skills.Length)], 100 + (Utility.RandomMinMax(min, max) * 5));
        }

        public static PowerScroll CreateRandomNoCraft(int min, int max)
        {
            min /= 5;
            max /= 5;

            SkillName skillName;

            do
            {
                skillName = Skills[Utility.Random(Skills.Length)];
            } while (IsCraftSkill(skillName));

            return new PowerScroll(skillName, 100 + (Utility.RandomMinMax(min, max) * 5));
        }

        public static PowerScroll RandomPowerScroll()
        {
            int power;
            int roll = Utility.RandomMinMax(1, 100);
            if (roll >= 95) { power = 25; }
            else if (roll >= 85) { power = 20; }
            else if (roll >= 70) { power = 15; }
            else if (roll >= 50) { power = 10; }
            else { power = 5; }

            return CreateRandom(power, power);
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1070722, Value + " Skill");

            if (Value == 105) { list.Add(1049644, "Wondrous Scroll"); }
            else if (Value == 110) { list.Add(1049644, "Exalted Scroll"); }
            else if (Value == 115) { list.Add(1049644, "Mythical Scroll"); }
            else if (Value == 120) { list.Add(1049644, "Legendary Scroll"); }
            else if (Value == 125) { list.Add(1049644, "Power Scroll"); }

			var shrine = GetShrineRegion(Skill);
			if (!string.IsNullOrEmpty(shrine))
				list.Add(string.Format("Requires the {0}", shrine));
        }

        public override bool CanUse(Mobile from)
        {
            if (!base.CanUse(from))
                return false;

            Skill skill = from.Skills[Skill];

            if (skill == null)
                return false;

            if (skill.Cap >= Value)
            {
                from.SendLocalizedMessage(1049511, GetNameLocalized()); // Your ~1_type~ is too high for this power scroll.
                return false;
            }

            if (MyServerSettings.RequirePowerscrollsInOrder() && skill.Cap < (Value - 5))
            {
                from.SendMessage("Your current cap has to be at {0} to use this scroll", Convert.ToString(Value - 5));
                return false;
            }

			var shrine = GetShrineRegion(Skill);
			if (!string.IsNullOrEmpty(shrine) && !from.Region.IsPartOf(shrine))
			{
				from.SendMessage("This magic can only be unleashed at the {0}.", shrine);
				return false;
			}

			return true;
        }

		private static string GetShrineRegion(SkillName skill)
		{
            switch (skill)
            {
                case SkillName.FistFighting:
                case SkillName.Bushido:
                case SkillName.Swords:
                case SkillName.Lumberjacking:
                case SkillName.Mining:
                case SkillName.Blacksmith:
                case SkillName.Carpentry:
                case SkillName.Bowcraft:
                case SkillName.Bludgeoning:
                case SkillName.Tactics:
                case SkillName.Parry:
                case SkillName.Fencing:
                    return "Shrine of Strength";

                case SkillName.Forensics:
                case SkillName.Tasting:
                case SkillName.Magery:
                case SkillName.Elementalism:
                case SkillName.MagicResist:
                case SkillName.Meditation:
                case SkillName.Necromancy:
                case SkillName.ArmsLore:
                case SkillName.Cartography:
                case SkillName.Cooking:
                case SkillName.Psychology:
                case SkillName.Anatomy:
                case SkillName.Alchemy:
                case SkillName.Tailoring:
                case SkillName.Tinkering:
                case SkillName.Inscribe:
                    return "Shrine of Intelligence";

                case SkillName.Begging:
                case SkillName.Camping:
                case SkillName.Discordance:
                case SkillName.Provocation:
                case SkillName.Musicianship:
                case SkillName.Marksmanship:
                case SkillName.Hiding:
                case SkillName.Stealing:
                case SkillName.Stealth:
                case SkillName.RemoveTrap:
                case SkillName.Snooping:
                case SkillName.Searching:
                case SkillName.Ninjitsu:
                case SkillName.Lockpicking:
                    return "Shrine of Dexterity";

                case SkillName.Mercantile:
                case SkillName.Spiritualism:
                case SkillName.Knightship:
                case SkillName.Peacemaking:
                case SkillName.Tracking:
                case SkillName.Veterinary:
                case SkillName.Druidism:
                case SkillName.Herding:
                case SkillName.Taming:
                case SkillName.Poisoning:
                case SkillName.Focus:
                case SkillName.Seafaring:
                case SkillName.Healing:
                    return "Shrine of Wisdom";

                default:
                    Console.WriteLine("Unknown powerscroll type {0} ({1}))", skill.ToString(), skill);
                    return string.Empty;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = (InheritsItem ? 0 : reader.ReadInt()); //Required for SpecialScroll insertion
        }

        public override string GetName()
        {
            return Name;
        }

        public override string GetNameLocalized()
        {
            return Name;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public string GetSkillDisplayName(SkillName skill)
        {
            switch (skill)
            {
                case SkillName.Alchemy: return "Alchemy";
                case SkillName.Anatomy: return "Anatomy";
                case SkillName.Druidism: return "Druidism";
                case SkillName.Mercantile: return "Mercantile";
                case SkillName.ArmsLore: return "Arms Lore";
                case SkillName.Parry: return "Parrying";
                case SkillName.Begging: return "Begging";
                case SkillName.Blacksmith: return "Blacksmithing";
                case SkillName.Bowcraft: return "Bowcrafting";
                case SkillName.Peacemaking: return "Peacemaking";
                case SkillName.Camping: return "Camping";
                case SkillName.Carpentry: return "Carpentry";
                case SkillName.Cartography: return "Cartography";
                case SkillName.Cooking: return "Cooking";
                case SkillName.Searching: return "Searching";
                case SkillName.Discordance: return "Discordance";
                case SkillName.Psychology: return "Psychology";
                case SkillName.Healing: return "Healing";
                case SkillName.Seafaring: return "Seafaring";
                case SkillName.Forensics: return "Forensics";
                case SkillName.Herding: return "Herding";
                case SkillName.Hiding: return "Hiding";
                case SkillName.Provocation: return "Provocation";
                case SkillName.Inscribe: return "Inscription";
                case SkillName.Lockpicking: return "Lockpicking";
                case SkillName.Magery: return "Magery";
                case SkillName.MagicResist: return "Magic Resist";
                case SkillName.Tactics: return "Tactics";
                case SkillName.Snooping: return "Snooping";
                case SkillName.Musicianship: return "Musicianship";
                case SkillName.Poisoning: return "Poisoning";
                case SkillName.Marksmanship: return "Marksmanship";
                case SkillName.Spiritualism: return "Spiritualism";
                case SkillName.Stealing: return "Stealing";
                case SkillName.Tailoring: return "Tailoring";
                case SkillName.Taming: return "Taming";
                case SkillName.Tasting: return "Tasting";
                case SkillName.Tinkering: return "Tinkering";
                case SkillName.Tracking: return "Tracking";
                case SkillName.Veterinary: return "Veterinary";
                case SkillName.Swords: return "Swords";
                case SkillName.Bludgeoning: return "Macing";
                case SkillName.Fencing: return "Fencing";
                case SkillName.FistFighting: return "Fist Fighting";
                case SkillName.Lumberjacking: return "Lumberjacking";
                case SkillName.Mining: return "Mining";
                case SkillName.Meditation: return "Meditation";
                case SkillName.Stealth: return "Stealth";
                case SkillName.RemoveTrap: return "Remove Traps";
                case SkillName.Necromancy: return "Necromancy";
                case SkillName.Focus: return "Focus";
                case SkillName.Knightship: return "Knightship";
                case SkillName.Bushido: return "Bushido";
                case SkillName.Ninjitsu: return "Ninjitsu";
                case SkillName.Elementalism: return "Elementalism";
                case SkillName.Mysticism: return "Mysticism";
                case SkillName.Imbuing: return "Imbuing";
                case SkillName.Throwing: return "Throwing";

                default:
                    Console.WriteLine("Failed to create skill name for {0} ({1}))", skill.ToString(), skill);
                    return skill.ToString(); // Better than null ref
            }
        }

        public override void Use(Mobile from)
        {
            if (!CanUse(from))
                return;

            from.SendLocalizedMessage(1049513, GetNameLocalized()); // You feel a surge of magic as the scroll enhances your ~1_type~!

            from.Skills[Skill].Cap = Value;

            Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
            Effects.PlaySound(from.Location, from.Map, 0x243);

            Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
            Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 4, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
            Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 4, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

            Effects.SendTargetParticles(from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);

            Delete();
        }

        private static bool IsCraftSkill(SkillName skill)
        {
            switch (skill)
            {
                // Crafting skills
                case SkillName.Alchemy:
                case SkillName.Blacksmith:
                case SkillName.Bowcraft:
                case SkillName.Carpentry:
                case SkillName.Cooking:
                case SkillName.Inscribe:
                case SkillName.Tailoring:
                case SkillName.Tinkering:
                    return true;

                // Gathering skills
                case SkillName.Forensics:
                case SkillName.Lumberjacking:
                case SkillName.Mining:
                    return true;

                case SkillName.Anatomy:
                case SkillName.Druidism:
                case SkillName.Mercantile:
                case SkillName.ArmsLore:
                case SkillName.Parry:
                case SkillName.Begging:
                case SkillName.Peacemaking:
                case SkillName.Camping:
                case SkillName.Cartography:
                case SkillName.Searching:
                case SkillName.Discordance:
                case SkillName.Psychology:
                case SkillName.Healing:
                case SkillName.Seafaring:
                case SkillName.Herding:
                case SkillName.Hiding:
                case SkillName.Provocation:
                case SkillName.Lockpicking:
                case SkillName.Magery:
                case SkillName.MagicResist:
                case SkillName.Tactics:
                case SkillName.Snooping:
                case SkillName.Musicianship:
                case SkillName.Poisoning:
                case SkillName.Marksmanship:
                case SkillName.Spiritualism:
                case SkillName.Stealing:
                case SkillName.Taming:
                case SkillName.Tasting:
                case SkillName.Tracking:
                case SkillName.Veterinary:
                case SkillName.Swords:
                case SkillName.Bludgeoning:
                case SkillName.Fencing:
                case SkillName.FistFighting:
                case SkillName.Meditation:
                case SkillName.Stealth:
                case SkillName.RemoveTrap:
                case SkillName.Necromancy:
                case SkillName.Focus:
                case SkillName.Knightship:
                case SkillName.Bushido:
                case SkillName.Ninjitsu:
                case SkillName.Elementalism:
                case SkillName.Mysticism:
                case SkillName.Imbuing:
                case SkillName.Throwing:
                default:
                    return false;
            }
        }
    }
}