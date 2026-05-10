namespace Server.Mobiles
{
	[PropertyObject]
	public class PlayerQuestContext
	{
		public PlayerQuestContext()
		{
		}

		public PlayerQuestContext(GenericReader reader)
		{
			int version = reader.ReadInt();

			AssassinQuest = reader.ReadString();
			BardsTaleQuest = reader.ReadString();
			EpicQuestName = reader.ReadString();
			EpicQuestNumber = reader.ReadInt();
			FishingQuest = reader.ReadString();
			MessageQuest = reader.ReadString();
			StandardQuest = reader.ReadString();
			ThiefQuest = reader.ReadString();
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string AssassinQuest { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string BardsTaleQuest { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string EpicQuestName { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int EpicQuestNumber { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string FishingQuest { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string MessageQuest { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string StandardQuest { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string ThiefQuest { get; set; }

		public void Serialize(GenericWriter writer)
		{
			writer.Write(0);

			writer.Write(AssassinQuest);
			writer.Write(BardsTaleQuest);
			writer.Write(EpicQuestName);
			writer.Write(EpicQuestNumber);
			writer.Write(FishingQuest);
			writer.Write(MessageQuest);
			writer.Write(StandardQuest);
			writer.Write(ThiefQuest);
		}
	}
}