using System;
using Server.Items;

namespace Server.Misc
{
	public class Arty
	{
		public static void ArtySetup( Item item, int pointsThatAreIgnored, string extra )
		{
			// points = points * 10;
			// points = 200 - points;
			// if ( points < 50 ){ points = 50; }
			int points = 0;

			if ( item is IGiftable )
			{
				var gift = (IGiftable)item;
				gift.Owner = null;
				gift.Gifter = "Unearthed by";
				gift.Points = points;
			}
		}

		public static void setArtifact( Item item )
		{
			if ( item.ArtifactLevel > (int)ArtifactLevel.None )
			{
				Type itemType = item.GetType();
				Item arty = null;

				if ( itemType != null )
				{
					arty = (Item)Activator.CreateInstance(itemType);
					item.Name = arty.Name;

					if ( !MySettings.S_ChangeArtyLook )
					{
						if ( !(item is BaseQuiver) ){ item.ItemID = arty.ItemID; }
						item.Hue = arty.Hue;
					}

					arty.Delete();
				}
			}
		}
	}
}