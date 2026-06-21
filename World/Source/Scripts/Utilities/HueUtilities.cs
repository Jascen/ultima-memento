using Server.Items;

namespace Server.Utilities
{
	public static class HueUtilities
	{
		public static int RandomMetalHue()
		{
			var resource = CraftResources.GetRandomNonBasicResource(CraftResourceType.Metal);
			return Server.Items.CraftResources.GetHue(resource);
		}

		public static int RandomBrightMetalHue()
		{
			var resource = (CraftResource)Utility.RandomList(
				(int)CraftResource.Nepturite,
				// (int)CraftResource.Obsidian,
				(int)CraftResource.Steel,
				(int)CraftResource.Brass,
				(int)CraftResource.Mithril,
				(int)CraftResource.Xormite,
				(int)CraftResource.Dwarven
			);

			return Server.Items.CraftResources.GetHue(resource);
		}

	}
}