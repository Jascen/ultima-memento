namespace Server.SpellBars
{
	public interface ISpellBarDescriptor
	{
		string CloseCommand { get; }

		int Number { get; }

		string Title { get; }

		string ToolCommand { get; }
	}
}