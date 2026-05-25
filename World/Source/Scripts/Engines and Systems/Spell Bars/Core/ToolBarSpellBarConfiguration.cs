using Server.Mobiles;

namespace Server.SpellBars
{
	public sealed class ToolBarSpellBarConfiguration : ISpellBarConfiguration
	{
		private readonly PlayerMobile _from;
		private readonly int _isVerticalIndex;
		private readonly int _maxSlots;
		private readonly string _settings;
		private readonly int _showNameIndex;
		private readonly int _totalOptions;

		public ToolBarSpellBarConfiguration(PlayerMobile from, string storageKey, int maxSlots, int backgroundImage)
		{
			_maxSlots = maxSlots;
			_showNameIndex = maxSlots + 1;
			_isVerticalIndex = maxSlots + 2;
			_totalOptions = maxSlots + 3;
			BackgroundImage = backgroundImage;
			StorageKey = storageKey;
			_from = from;
			_settings = ToolBarUpdates.GetToolBarSettings(from, storageKey);
			IsVertical = ToolBarUpdates.GetToolBarSetting(from, _isVerticalIndex, storageKey) > 0;
			ShowName = ToolBarUpdates.GetToolBarSetting(from, _showNameIndex, storageKey) > 0;
		}

		public int BackgroundImage { get; private set; }

		public bool IsVertical { get; private set; }

		public int MaxSlots
		{ get { return _maxSlots; } }

		public bool ShowName { get; private set; }

		public string StorageKey { get; private set; }

		public bool IsSpellEnabled(int spellIndex)
		{
			return ToolBarUpdates.GetToolBarSetting(_settings, spellIndex);
		}

		public void ToggleIsVertical()
		{
			ToolBarUpdates.UpdateToolBar(_from, _isVerticalIndex, StorageKey, _totalOptions);
		}

		public void ToggleShowName()
		{
			ToolBarUpdates.UpdateToolBar(_from, _showNameIndex, StorageKey, _totalOptions);
		}

		public void ToggleSpellEnabled(int spellIndex)
		{
			ToolBarUpdates.UpdateToolBar(_from, spellIndex, StorageKey, _totalOptions);
		}
	}
}