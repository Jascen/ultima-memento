namespace Server.SpellBars
{
	public interface ISpellBarConfiguration
	{
		int BackgroundImage { get; }

		bool IsVertical { get; }

		int MaxSlots { get; }

		bool ShowName { get; }

		bool IsSpellEnabled(int spellIndex);

		void ToggleIsVertical();

		void ToggleShowName();

		void ToggleSpellEnabled(int spellIndex);
	}
}