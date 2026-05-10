namespace Server.Engines.Avatar
{
	[PropertyObject]
	public class SkillArchive
	{
		private readonly int[] m_Skills;

		#region Skill Getters & Setters

		[CommandProperty(AccessLevel.GameMaster)]
		public int Alchemy
		{ get { return this[SkillName.Alchemy]; } set { this[SkillName.Alchemy] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Anatomy
		{ get { return this[SkillName.Anatomy]; } set { this[SkillName.Anatomy] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int ArmsLore
		{ get { return this[SkillName.ArmsLore]; } set { this[SkillName.ArmsLore] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Begging
		{ get { return this[SkillName.Begging]; } set { this[SkillName.Begging] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Blacksmith
		{ get { return this[SkillName.Blacksmith]; } set { this[SkillName.Blacksmith] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Bludgeoning
		{ get { return this[SkillName.Bludgeoning]; } set { this[SkillName.Bludgeoning] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Bowcraft
		{ get { return this[SkillName.Bowcraft]; } set { this[SkillName.Bowcraft] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Bushido
		{ get { return this[SkillName.Bushido]; } set { this[SkillName.Bushido] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Camping
		{ get { return this[SkillName.Camping]; } set { this[SkillName.Camping] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Carpentry
		{ get { return this[SkillName.Carpentry]; } set { this[SkillName.Carpentry] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Cartography
		{ get { return this[SkillName.Cartography]; } set { this[SkillName.Cartography] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Cooking
		{ get { return this[SkillName.Cooking]; } set { this[SkillName.Cooking] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Discordance
		{ get { return this[SkillName.Discordance]; } set { this[SkillName.Discordance] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Druidism
		{ get { return this[SkillName.Druidism]; } set { this[SkillName.Druidism] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Elementalism
		{ get { return this[SkillName.Elementalism]; } set { this[SkillName.Elementalism] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Fencing
		{ get { return this[SkillName.Fencing]; } set { this[SkillName.Fencing] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int FistFighting
		{ get { return this[SkillName.FistFighting]; } set { this[SkillName.FistFighting] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Focus
		{ get { return this[SkillName.Focus]; } set { this[SkillName.Focus] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Forensics
		{ get { return this[SkillName.Forensics]; } set { this[SkillName.Forensics] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Healing
		{ get { return this[SkillName.Healing]; } set { this[SkillName.Healing] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Herding
		{ get { return this[SkillName.Herding]; } set { this[SkillName.Herding] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Hiding
		{ get { return this[SkillName.Hiding]; } set { this[SkillName.Hiding] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Imbuing
		{ get { return this[SkillName.Imbuing]; } set { this[SkillName.Imbuing] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Inscribe
		{ get { return this[SkillName.Inscribe]; } set { this[SkillName.Inscribe] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Knightship
		{ get { return this[SkillName.Knightship]; } set { this[SkillName.Knightship] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Lockpicking
		{ get { return this[SkillName.Lockpicking]; } set { this[SkillName.Lockpicking] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Lumberjacking
		{ get { return this[SkillName.Lumberjacking]; } set { this[SkillName.Lumberjacking] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Magery
		{ get { return this[SkillName.Magery]; } set { this[SkillName.Magery] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int MagicResist
		{ get { return this[SkillName.MagicResist]; } set { this[SkillName.MagicResist] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Marksmanship
		{ get { return this[SkillName.Marksmanship]; } set { this[SkillName.Marksmanship] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Meditation
		{ get { return this[SkillName.Meditation]; } set { this[SkillName.Meditation] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Mercantile
		{ get { return this[SkillName.Mercantile]; } set { this[SkillName.Mercantile] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Mining
		{ get { return this[SkillName.Mining]; } set { this[SkillName.Mining] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Musicianship
		{ get { return this[SkillName.Musicianship]; } set { this[SkillName.Musicianship] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Mysticism
		{ get { return this[SkillName.Mysticism]; } set { this[SkillName.Mysticism] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Necromancy
		{ get { return this[SkillName.Necromancy]; } set { this[SkillName.Necromancy] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Ninjitsu
		{ get { return this[SkillName.Ninjitsu]; } set { this[SkillName.Ninjitsu] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Parry
		{ get { return this[SkillName.Parry]; } set { this[SkillName.Parry] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Peacemaking
		{ get { return this[SkillName.Peacemaking]; } set { this[SkillName.Peacemaking] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Poisoning
		{ get { return this[SkillName.Poisoning]; } set { this[SkillName.Poisoning] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Provocation
		{ get { return this[SkillName.Provocation]; } set { this[SkillName.Provocation] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Psychology
		{ get { return this[SkillName.Psychology]; } set { this[SkillName.Psychology] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int RemoveTrap
		{ get { return this[SkillName.RemoveTrap]; } set { this[SkillName.RemoveTrap] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Seafaring
		{ get { return this[SkillName.Seafaring]; } set { this[SkillName.Seafaring] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Searching
		{ get { return this[SkillName.Searching]; } set { this[SkillName.Searching] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Snooping
		{ get { return this[SkillName.Snooping]; } set { this[SkillName.Snooping] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Spiritualism
		{ get { return this[SkillName.Spiritualism]; } set { this[SkillName.Spiritualism] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Stealing
		{ get { return this[SkillName.Stealing]; } set { this[SkillName.Stealing] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Stealth
		{ get { return this[SkillName.Stealth]; } set { this[SkillName.Stealth] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Swords
		{ get { return this[SkillName.Swords]; } set { this[SkillName.Swords] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Tactics
		{ get { return this[SkillName.Tactics]; } set { this[SkillName.Tactics] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Tailoring
		{ get { return this[SkillName.Tailoring]; } set { this[SkillName.Tailoring] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Taming
		{ get { return this[SkillName.Taming]; } set { this[SkillName.Taming] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Tasting
		{ get { return this[SkillName.Tasting]; } set { this[SkillName.Tasting] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Throwing
		{ get { return this[SkillName.Throwing]; } set { this[SkillName.Throwing] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Tinkering
		{ get { return this[SkillName.Tinkering]; } set { this[SkillName.Tinkering] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Tracking
		{ get { return this[SkillName.Tracking]; } set { this[SkillName.Tracking] = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Veterinary
		{ get { return this[SkillName.Veterinary]; } set { this[SkillName.Veterinary] = value; } }

		#endregion Skill Getters & Setters

		public SkillArchive()
		{
			SkillInfo[] info = SkillInfo.Table;

			m_Skills = new int[info.Length];
		}

		public SkillArchive(GenericReader reader)
		{
			int version = reader.ReadInt();

			SkillInfo[] info = SkillInfo.Table;
			m_Skills = new int[info.Length];

			int count = reader.ReadInt();

			for (int i = 0; i < count; ++i)
			{
				if (i < info.Length)
				{
					m_Skills[i] = reader.ReadInt();
				}
			}
		}

		public int Length
		{
			get { return m_Skills.Length; }
		}

		public int this[SkillName name]
		{
			get { return this[(int)name]; }
			set { this[(int)name] = value; }
		}

		public int this[int skillID]
		{
			get
			{
				if (skillID < 0 || skillID >= m_Skills.Length)
					return 0;

				return m_Skills[skillID];
			}
			set { m_Skills[skillID] = value; }
		}

		public void Serialize(GenericWriter writer)
		{
			writer.Write(0); // version

			writer.Write(m_Skills.Length);
			for (int i = 0; i < m_Skills.Length; ++i)
			{
				writer.Write(m_Skills[i]);
			}
		}

		public override string ToString()
		{
			return "...";
		}
	}
}