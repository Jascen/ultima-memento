namespace Server.Mobiles
{
	[PropertyObject]
	public class PlayerPreferenceContext
	{
		public PlayerPreferenceContext()
		{
			ColorlessFabricBreakdown = true;
		}

		public PlayerPreferenceContext(GenericReader reader)
		{
			int version = reader.ReadInt();

			DoubleClickID = reader.ReadBool();
			SuppressVendorTooltip = reader.ReadBool();
			SingleAttemptID = reader.ReadBool();
			ColorlessFabricBreakdown = reader.ReadBool();
			CharacterSheath = reader.ReadBool();
			CharacterWepAbNames = reader.ReadBool();
			CharMusical = reader.ReadString();
			WeaponBarOpen = reader.ReadBool();
			IgnoreVendorGoldSafeguard = reader.ReadBool();
			ClassicPoisoning = reader.ReadBool();

			if (1 < version)
			{
				CharacterBarbaric = reader.ReadInt();
				CharacterEvil = reader.ReadBool();
				CharacterLoot = reader.ReadString();
				CharacterOriental = reader.ReadBool();
				GumpHue = reader.ReadInt();
				MagerySpellHue = reader.ReadInt();
				MessageOfTheDay = reader.ReadBool();
				MusicPlaylist = reader.ReadString();
				MyChat = reader.ReadString();
				MyLibrary = reader.ReadString();
				QuickBar = reader.ReadString();
				RegBar = reader.ReadString();
				UsingAncientBook = reader.ReadBool();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int CharacterBarbaric { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool CharacterEvil { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string CharacterLoot { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool CharacterOriental { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool CharacterSheath { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool CharacterWepAbNames { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string CharMusical { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ClassicPoisoning { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ColorlessFabricBreakdown { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool DoubleClickID { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int GumpHue { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IgnoreVendorGoldSafeguard { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int MagerySpellHue { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool MessageOfTheDay { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string MusicPlaylist { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string MyChat { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string MyLibrary { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string QuickBar { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string RegBar { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool SingleAttemptID { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool SuppressVendorTooltip { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool UsingAncientBook { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool WeaponBarOpen { get; set; }

		public void Serialize(GenericWriter writer)
		{
			writer.Write(2);

			writer.Write(DoubleClickID);
			writer.Write(SuppressVendorTooltip);
			writer.Write(SingleAttemptID);
			writer.Write(ColorlessFabricBreakdown);
			writer.Write(CharacterSheath);
			writer.Write(CharacterWepAbNames);
			writer.Write(CharMusical);
			writer.Write(WeaponBarOpen);
			writer.Write(IgnoreVendorGoldSafeguard);
			writer.Write(ClassicPoisoning);

			writer.Write(CharacterBarbaric);
			writer.Write(CharacterEvil);
			writer.Write(CharacterLoot);
			writer.Write(CharacterOriental);
			writer.Write(GumpHue);
			writer.Write(MagerySpellHue);
			writer.Write(MessageOfTheDay);
			writer.Write(MusicPlaylist);
			writer.Write(MyChat);
			writer.Write(MyLibrary);
			writer.Write(QuickBar);
			writer.Write(RegBar);
			writer.Write(UsingAncientBook);
		}
	}
}