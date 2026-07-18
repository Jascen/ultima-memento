using System;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

namespace Server.SpellBars
{
	public abstract class SpellBarToolbarGump : Gump
	{
		public static void Initialize()
		{
			EventSink.Login += args =>
			{
				var player = args.Mobile as PlayerMobile;
				if (player == null)
					return;

				Timer.DelayCall(TimeSpan.FromSeconds(3), () =>
				{
					var opened = false;
					foreach (var id in player.SpellBars.GetAllAutoOpenSpellBarIds())
					{
						if (player.NetState == null) return;
						
						var definition = SpellBarRegistry.GetDefinition(id);
						CommandSystem.Handle(player, String.Format("{0}{1}", CommandSystem.Prefix, definition.ToolCommand));
						opened = true;
					}

					if (opened)
						player.SendMessage(32, "Spell bars automatically opened. Use `[toolbars` to disable them.");
				});
			};
		}

		const int SPELL_ACTION_OFFSET = 100;

		private readonly SpellBarId _barId;
		private readonly ToolBarSpellBarConfiguration _configuration;
		private readonly ISpellSchool _school;

		public SpellBarToolbarGump(SpellBarId barId, PlayerMobile from)
			: base(50, 50)
		{
			_barId = barId;
			var definition = SpellBarRegistry.GetDefinition(barId);
			_school = definition.SchoolInstance;
			_configuration = SpellBarRegistry.CreateConfiguration(from, barId);

			Closable = false;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);

			bool isVertical = _configuration.IsVertical;
			bool showName = isVertical && _configuration.ShowName;

			if (isVertical)
				AddImage(5, 0, _configuration.BackgroundImage);
			else
				AddImage(0, 5, _configuration.BackgroundImage);

			int dby = 45;
			for (int spellIndex = 1; spellIndex <= _configuration.MaxSlots; spellIndex++)
			{
				int spellId = _school.GetRegistrySpellId(spellIndex);
				if (!_school.HasSpell(from, spellId))
					continue;

				if (!_configuration.IsSpellEnabled(spellIndex))
					continue;

				int spellIcon = _school.GetIcon(from, spellIndex);

				if (isVertical)
					AddButton(5, dby, spellIcon, spellIcon, SPELL_ACTION_OFFSET + spellId, GumpButtonType.Reply, 1);
				else
					AddButton(dby, 5, spellIcon, spellIcon, SPELL_ACTION_OFFSET + spellId, GumpButtonType.Reply, 1);

				dby += 45;

				if (!showName)
					continue;

				AddLabel(59, dby - 34, 0x481, _school.GetName(spellIndex));
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (info.ButtonID < 1)
				return;

			var from = sender.Mobile as PlayerMobile;
			if (from == null)
				return;

			int spellId = info.ButtonID - SPELL_ACTION_OFFSET;
			Spell spell = SpellRegistry.NewSpell(spellId, from, null);

			if (spell != null)
				spell.Cast();
			else
				from.SendMessage("That spell was not found.");

			from.SendGump(SpellBarRegistry.CreateToolbarGump(_barId, from));
		}
	}
}