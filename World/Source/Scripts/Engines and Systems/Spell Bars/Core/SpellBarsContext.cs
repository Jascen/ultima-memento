using System.Collections.Generic;
using System.Linq;

namespace Server.SpellBars
{
	public class SpellBarsContext
	{
		private readonly Dictionary<SpellBarId, SpellBarState> _states;

		public SpellBarsContext()
		{
			_states = new Dictionary<SpellBarId, SpellBarState>();
		}

		public SpellBarsContext(GenericReader reader)
			: this()
		{
			var version = reader.ReadInt();

			if (version < 2)
			{
				DeserializeLegacy(reader);
				return;
			}

			var count = reader.ReadInt();

			for (var index = 0; index < count; index++)
			{
				var id = (SpellBarId)reader.ReadInt();
				var isValid = IsValid(id);
				var state = new SpellBarState(reader, isValid ? GetMaxSlots(id) : 0);

				if (!isValid) continue;

				_states[id] = state;
			}
		}

		public IEnumerable<SpellBarId> GetAllAutoOpenSpellBarIds()
		{
			return _states.Where(pair => pair.Value.OpenOnLogin).Select(pair => pair.Key);
		}

		public SpellBarState GetState(SpellBarId id)
		{
			if (!IsValid(id)) throw new System.ArgumentOutOfRangeException("id");

			SpellBarState state;

			if (!_states.TryGetValue(id, out state))
			{
				state = new SpellBarState(GetMaxSlots(id));
				_states[id] = state;
			}

			return state;
		}

		public void ReadLegacyState(SpellBarId id, string settings)
		{
			if (!IsValid(id)) throw new System.ArgumentOutOfRangeException("id");

			_states[id] = SpellBarState.FromLegacy(settings, GetMaxSlots(id));
		}

		public void Serialize(GenericWriter writer)
		{
			writer.Write(2);
			writer.Write(_states.Count);

			foreach (SpellBarId id in System.Enum.GetValues(typeof(SpellBarId)))
			{
				SpellBarState state;
				if (!_states.TryGetValue(id, out state)) continue;

				writer.Write((int)id);
				state.Serialize(writer);
			}
		}

		private static int GetMaxSlots(SpellBarId id)
		{
			return SpellBarRegistry.GetDefinition(id).SchoolInstance.MaxSlots;
		}

		private static bool IsValid(SpellBarId id)
		{
			return System.Enum.IsDefined(typeof(SpellBarId), id);
		}

		private void DeserializeLegacy(GenericReader reader)
		{
			ReadLegacyState(SpellBarId.Mage_1, reader.ReadString());
			ReadLegacyState(SpellBarId.Mage_2, reader.ReadString());
			ReadLegacyState(SpellBarId.Mage_3, reader.ReadString());
			ReadLegacyState(SpellBarId.Mage_4, reader.ReadString());
			ReadLegacyState(SpellBarId.Necro_1, reader.ReadString());
			ReadLegacyState(SpellBarId.Necro_2, reader.ReadString());
			ReadLegacyState(SpellBarId.Knight_1, reader.ReadString());
			ReadLegacyState(SpellBarId.Knight_2, reader.ReadString());
			ReadLegacyState(SpellBarId.Death_1, reader.ReadString());
			ReadLegacyState(SpellBarId.Death_2, reader.ReadString());
			ReadLegacyState(SpellBarId.Bard_1, reader.ReadString());
			ReadLegacyState(SpellBarId.Bard_2, reader.ReadString());
			ReadLegacyState(SpellBarId.Priest_1, reader.ReadString());
			ReadLegacyState(SpellBarId.Priest_2, reader.ReadString());
			ReadLegacyState(SpellBarId.Ancient_1, reader.ReadString());
			ReadLegacyState(SpellBarId.Ancient_2, reader.ReadString());
			ReadLegacyState(SpellBarId.Ancient_3, reader.ReadString());
			ReadLegacyState(SpellBarId.Ancient_4, reader.ReadString());
			ReadLegacyState(SpellBarId.Monk_1, reader.ReadString());
			ReadLegacyState(SpellBarId.Monk_2, reader.ReadString());
			ReadLegacyState(SpellBarId.Elemental_1, reader.ReadString());
			ReadLegacyState(SpellBarId.Elemental_2, reader.ReadString());
		}
	}
} 