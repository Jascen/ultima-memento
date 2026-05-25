namespace Server.SpellBars
{
	public sealed class SpellBarDefinition : ISpellBarDescriptor
	{
		private static readonly string[] RomanNumerals = { "I", "II", "III", "IV" };

		public SpellBarDefinition(
			SpellBarId id,
			SpellBarSchool school,
			ISpellSchool schoolInstance,
			int barNumber,
			string storageKey,
			string toolCommand,
			string closeCommand,
			string setupCommand,
			string titlePrefix,
			bool usePaging)
		{
			Id = id;
			School = school;
			SchoolInstance = schoolInstance;
			Number = barNumber;
			UsePaging = usePaging;
			StorageKey = storageKey;
			ToolCommand = toolCommand;
			CloseCommand = closeCommand;
			SetupCommand = setupCommand;
			Title = string.Format("{0} - {1}", titlePrefix, RomanNumerals[barNumber - 1]);
		}

		public string CloseCommand { get; private set; }
		public SpellBarId Id { get; private set; }
		public int Number { get; private set; }
		public SpellBarSchool School { get; private set; }
		public ISpellSchool SchoolInstance { get; private set; }
		public string SetupCommand { get; private set; }
		public string StorageKey { get; private set; }
		public string Title { get; private set; }
		public string ToolCommand { get; private set; }
		public bool UsePaging { get; private set; }
	}
}