using Server.Mobiles;

namespace Server.SpellBars
{
	public interface ISpellSchool
	{
		int MaxSlots { get; }

		SpellBarSchool School { get; }

		int GetBackgroundImage(PlayerMobile from);

		int GetIcon(PlayerMobile from, int slotIndex);

		string GetName(int slotIndex);

		int GetRegistrySpellId(int slotIndex);

		bool HasSpell(PlayerMobile from, int registrySpellId);
	}
}