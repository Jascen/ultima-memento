using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Mobiles;

namespace Server.SpellBars
{
	public static partial class SpellBarRegistry
	{
		private static readonly SpellBarDefinition[] Definitions = BuildDefinitions();
		private static readonly Dictionary<SpellBarId, SpellBarDefinition> DefinitionMap = BuildDefinitionMap();

		public static SpellBarDefinition GetDefinition(SpellBarId id)
		{
			return DefinitionMap[id];
		}

		public static ToolBarSpellBarConfiguration CreateConfiguration(PlayerMobile from, SpellBarId id)
		{
			var definition = GetDefinition(id);
			var background = definition.SchoolInstance.GetBackgroundImage(from);
			var state = from.SpellBars.GetState(id);
			return new ToolBarSpellBarConfiguration(state, definition.SchoolInstance.MaxSlots, background);
		}

		public static SpellBarToolbarGump CreateToolbarGump(SpellBarId id, PlayerMobile from)
		{
			return (SpellBarToolbarGump)Activator.CreateInstance(GetToolbarGumpType(id), from);
		}

		public static SpellBarSetupGumpBase CreateSetupGump(SpellBarId id, PlayerMobile from, int origin, int pageNumber = 1)
		{
			var definition = GetDefinition(id);

			if (definition.UsePaging)
				return (SpellBarSetupGumpBase)Activator.CreateInstance(GetSetupGumpType(id), from, origin, pageNumber);

			return (SpellBarSetupGumpBase)Activator.CreateInstance(GetSetupGumpType(id), from, origin);
		}

		public static void Initialize()
		{
			foreach (var definition in Definitions)
				Register(definition);
		}

		public static void ToggleToolbar(PlayerMobile from, SpellBarId id)
		{
			if (from.HasGump(GetToolbarGumpType(id)))
				CloseToolbar(from, id);
			else
				OpenSetup(from, id, 0);
		}

		public static void OpenSetup(PlayerMobile from, SpellBarId id, int origin, int pageNumber = 1)
		{
			var gump = CreateSetupGump(id, from, origin, pageNumber);
			if (gump == null || !gump.ConfigureGump()) return;

			from.CloseGump(GetSetupGumpType(id));
			from.SendGump(gump);
		}

		public static void CloseToolbar(Mobile from, SpellBarId id)
		{
			from.CloseGump(GetToolbarGumpType(id));
		}

		public static void CloseSetup(Mobile from, SpellBarId id)
		{
			from.CloseGump(GetSetupGumpType(id));
		}

		public static void CloseAll(Mobile from)
		{
			foreach (var definition in Definitions)
			{
				CloseToolbar(from, definition.Id);
				CloseSetup(from, definition.Id);
			}
		}

		public static void CloseAllToolbars(Mobile from, SpellBarSchool school)
		{
			foreach (var definition in Definitions)
			{
				if (definition.School == school)
					CloseToolbar(from, definition.Id);
			}
		}

		public static void CloseAllSetup(Mobile from, SpellBarSchool school)
		{
			foreach (var definition in Definitions)
			{
				if (definition.School == school)
					CloseSetup(from, definition.Id);
			}
		}

		public static void RefreshOpenSetupGumps(Mobile mobile, SpellBarSchool school)
		{
			var player = mobile as PlayerMobile;
			if (player == null)
				return;

			foreach (var definition in Definitions)
			{
				if (definition.School != school)
					continue;

				if (!mobile.HasGump(GetSetupGumpType(definition.Id)))
					continue;

				var gump = CreateSetupGump(definition.Id, player, 0);
				if (gump == null || !gump.ConfigureGump()) continue;

				mobile.CloseGump(GetSetupGumpType(definition.Id));
				mobile.SendGump(gump);
			}
		}

		private static void Register(SpellBarDefinition definition)
		{
			CommandSystem.Register(definition.ToolCommand, AccessLevel.Player, args =>
			{
				var from = args.Mobile as PlayerMobile;
				if (from == null) return;

				from.CloseGump(GetToolbarGumpType(definition.Id));
				from.SendGump(CreateToolbarGump(definition.Id, from));
			});

			CommandSystem.Register(definition.CloseCommand, AccessLevel.Player, args =>
				args.Mobile.CloseGump(GetToolbarGumpType(definition.Id)));
		}

		private static Dictionary<SpellBarId, SpellBarDefinition> BuildDefinitionMap()
		{
			var map = new Dictionary<SpellBarId, SpellBarDefinition>();

			foreach (var definition in Definitions)
				map[definition.Id] = definition;

			return map;
		}

		private static SpellBarDefinition Bar(
			SpellBarId id,
			SpellBarSchool school,
			ISpellSchool schoolInstance,
			int barNumber,
			string toolCommand,
			string closeCommand,
			string titlePrefix,
			bool usePaging)
		{
			return new SpellBarDefinition(id, school, schoolInstance, barNumber, toolCommand, closeCommand, titlePrefix, usePaging);
		}
	}
}
