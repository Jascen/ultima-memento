namespace Server.SpellBars
{
	public sealed class SpellBarState
	{
		private readonly bool[] _spellEnabled;

		public SpellBarState(int maxSlots)
		{
			_spellEnabled = new bool[maxSlots];
		}

		public SpellBarState(GenericReader reader, int maxSlots)
		{
			var version = reader.ReadInt();
			var length = reader.ReadInt();

			_spellEnabled = new bool[maxSlots];

			for (var index = 0; index < length; index++)
			{
				var isEnabled = reader.ReadBool();

				// Only store values up to the new list size; discard the excess
				if (index < _spellEnabled.Length)
					_spellEnabled[index] = isEnabled;
			}

			ShowName = reader.ReadBool();
			IsVertical = reader.ReadBool();
			OpenOnLogin = 0 < version ? reader.ReadBool() : false;
		}

		public bool IsVertical { get; set; }

		public bool OpenOnLogin { get; set; }

		public bool ShowName { get; set; }

		public static SpellBarState FromLegacy(string settings, int maxSlots)
		{
			var state = new SpellBarState(maxSlots);

			if (string.IsNullOrEmpty(settings)) return state;

			var values = settings.Split('#');

			for (var index = 0; index < maxSlots && index < values.Length; index++)
				state._spellEnabled[index] = IsLegacyEnabled(values[index]);

			if (maxSlots < values.Length)
				state.ShowName = IsLegacyEnabled(values[maxSlots]);

			if (maxSlots + 1 < values.Length)
				state.IsVertical = IsLegacyEnabled(values[maxSlots + 1]);

			return state;
		}

		public bool IsSpellEnabled(int spellIndex)
		{
			var index = spellIndex - 1;
			return 0 <= index && index < _spellEnabled.Length && _spellEnabled[index];
		}

		public void Serialize(GenericWriter writer)
		{
			writer.Write(1);
			writer.Write(_spellEnabled.Length);

			for (var index = 0; index < _spellEnabled.Length; index++)
				writer.Write(_spellEnabled[index]);

			writer.Write(ShowName);
			writer.Write(IsVertical);
			writer.Write(OpenOnLogin);
		}

		public void ToggleSpellEnabled(int spellIndex)
		{
			var index = spellIndex - 1;
			if (index < 0 || _spellEnabled.Length <= index) return;

			_spellEnabled[index] = !_spellEnabled[index];
		}

		private static bool IsLegacyEnabled(string value)
		{
			int parsed;
			return int.TryParse(value, out parsed) && 0 < parsed;
		}
	}
}
