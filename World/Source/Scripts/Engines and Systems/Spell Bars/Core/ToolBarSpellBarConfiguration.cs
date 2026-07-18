namespace Server.SpellBars
{
	public sealed class ToolBarSpellBarConfiguration : ISpellBarConfiguration
	{
		private readonly int _maxSlots;
		private readonly SpellBarState _state;

		public ToolBarSpellBarConfiguration(SpellBarState state, int maxSlots, int backgroundImage)
		{
			_maxSlots = maxSlots;
			_state = state;
			BackgroundImage = backgroundImage;
		}

		public int BackgroundImage { get; private set; }

		public bool IsVertical
		{ get { return _state.IsVertical; } }

		public int MaxSlots
		{ get { return _maxSlots; } }

		public bool ShowName
		{ get { return _state.ShowName; } }

		public bool IsSpellEnabled(int spellIndex)
		{
			return _state.IsSpellEnabled(spellIndex);
		}

		public void ToggleIsVertical()
		{
			_state.IsVertical = !_state.IsVertical;
		}

		public void ToggleShowName()
		{
			_state.ShowName = !_state.ShowName;
		}

		public void ToggleSpellEnabled(int spellIndex)
		{
			_state.ToggleSpellEnabled(spellIndex);
		}
	}
}