using System;
using System.Collections.Generic;
using System.Reflection;

namespace Server.Spells
{
	public static class SpellProviderRegistry
	{
		private static Dictionary<SpellDefinition, Type> m_TypesByDefinition;

		public static void Register(List<SpellDefinition> definitions)
		{
			if (definitions.Count == 0)
				throw new InvalidOperationException("A spell provider must contain at least one spell.");

			var typesByDefinition = GetTypesByDefinition();

			for (var i = 0; i < definitions.Count; i++)
			{
				var definition = definitions[i];

				// Spellbook.GetTypeForSpell and Spellbook.BookOffset/BookCount treat a provider as the range FirstSpellId..LastSpellId
				if (0 < i && definition.SpellID != definitions[i - 1].SpellID + 1)
					throw new InvalidOperationException(string.Format("Spell ID '{0}' must directly follow spell ID '{1}'.", definition.SpellID, definitions[i - 1].SpellID));

				Type type;
				if (!typesByDefinition.TryGetValue(definition, out type))
					throw new InvalidOperationException(string.Format("No spell declares the definition for spell ID '{0}'.", definition.SpellID));

				SpellRegistry.Register(definition.SpellID, type);
			}
		}

		private static Dictionary<SpellDefinition, Type> GetTypesByDefinition()
		{
			if (m_TypesByDefinition != null)
				return m_TypesByDefinition;

			m_TypesByDefinition = new Dictionary<SpellDefinition, Type>();

			foreach (var type in typeof(Spell).Assembly.GetTypes())
			{
				if (type.IsAbstract || !type.IsSubclassOf(typeof(Spell)))
					continue;

				var field = type.GetField("SpellInfo", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
				if (field == null || field.FieldType != typeof(SpellInfo))
					continue;

				var info = (SpellInfo)field.GetValue(null);
				if (info != null && info.SpellDefinition != null)
					m_TypesByDefinition[info.SpellDefinition] = type;
			}

			return m_TypesByDefinition;
		}
	}
}
