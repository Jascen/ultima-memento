using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	public class DDRelicRugAddon : BaseAddon
	{
		public int RelicGoldValue;
		public int RelicMainID;
		public int RelicRugID;
		public int RelicFound;
		public int RelicColor;
		public string RelicQuality;
		
		[CommandProperty(AccessLevel.Owner)]
		public int Relic_Value { get { return RelicGoldValue; } set { RelicGoldValue = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_Color { get { return RelicColor; } set { RelicColor = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_MainID { get { return RelicMainID; } set { RelicMainID = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_RugID { get { return RelicRugID; } set { RelicRugID = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_Found { get { return RelicFound; } set { RelicFound = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Relic_Quality { get { return RelicQuality; } set { RelicQuality = value; InvalidateProperties(); } }

		public override BaseAddonDeed Deed
		{
			get
			{
				return new DDRelicRugAddonDeed( Relic_Value, Relic_MainID, Relic_RugID, Relic_Color, Relic_Quality, 1 );
			}
		}

		[Constructable]
		public DDRelicRugAddon() : this( 0, 0, 0, 0, "blank" )
		{
		}

		[ Constructable ]
		public DDRelicRugAddon( int RelCost, int RelID, int RelRugID, int RelHue, string RelQuality )
		{
			if ( RelRugID == 1 )
			{
				AddComplexComponent( (BaseAddon) this, 2755, -1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 1
				AddComplexComponent( (BaseAddon) this, 2807, 0, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 2
				AddComplexComponent( (BaseAddon) this, 2757, 2, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 3
				AddComplexComponent( (BaseAddon) this, 2807, 1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 4
				AddComplexComponent( (BaseAddon) this, 2752, 0, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 5
				AddComplexComponent( (BaseAddon) this, 2806, -1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 6
				AddComplexComponent( (BaseAddon) this, 2752, 1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 7
				AddComplexComponent( (BaseAddon) this, 2752, 0, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 8
				AddComplexComponent( (BaseAddon) this, 2752, 1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 9
				AddComplexComponent( (BaseAddon) this, 2754, 2, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 10
				AddComplexComponent( (BaseAddon) this, 2756, -1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 11
				AddComplexComponent( (BaseAddon) this, 2806, -1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 12
				AddComplexComponent( (BaseAddon) this, 2808, 2, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 13
				AddComplexComponent( (BaseAddon) this, 2808, 2, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 14
				AddComplexComponent( (BaseAddon) this, 2809, 0, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 15
				AddComplexComponent( (BaseAddon) this, 2809, 1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 16
			}
			else if ( RelRugID == 2 )
			{
				AddComplexComponent( (BaseAddon) this, 2755, -1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 1
				AddComplexComponent( (BaseAddon) this, 2807, 0, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 2
				AddComplexComponent( (BaseAddon) this, 2757, 2, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 3
				AddComplexComponent( (BaseAddon) this, 2807, 1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 4
				AddComplexComponent( (BaseAddon) this, 2749, 0, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 5
				AddComplexComponent( (BaseAddon) this, 2806, -1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 6
				AddComplexComponent( (BaseAddon) this, 2749, 1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 7
				AddComplexComponent( (BaseAddon) this, 2749, 0, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 8
				AddComplexComponent( (BaseAddon) this, 2749, 1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 9
				AddComplexComponent( (BaseAddon) this, 2754, 2, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 10
				AddComplexComponent( (BaseAddon) this, 2756, -1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 11
				AddComplexComponent( (BaseAddon) this, 2806, -1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 12
				AddComplexComponent( (BaseAddon) this, 2808, 2, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 13
				AddComplexComponent( (BaseAddon) this, 2808, 2, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 14
				AddComplexComponent( (BaseAddon) this, 2809, 0, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 15
				AddComplexComponent( (BaseAddon) this, 2809, 1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 16
			}
			else if ( RelRugID == 3 )
			{
				AddComplexComponent( (BaseAddon) this, 2755, -1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 1
				AddComplexComponent( (BaseAddon) this, 2807, 0, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 2
				AddComplexComponent( (BaseAddon) this, 2757, 2, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 3
				AddComplexComponent( (BaseAddon) this, 2807, 1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 4
				AddComplexComponent( (BaseAddon) this, 2750, 0, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 5
				AddComplexComponent( (BaseAddon) this, 2806, -1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 6
				AddComplexComponent( (BaseAddon) this, 2750, 1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 7
				AddComplexComponent( (BaseAddon) this, 2750, 0, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 8
				AddComplexComponent( (BaseAddon) this, 2750, 1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 9
				AddComplexComponent( (BaseAddon) this, 2754, 2, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 10
				AddComplexComponent( (BaseAddon) this, 2756, -1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 11
				AddComplexComponent( (BaseAddon) this, 2806, -1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 12
				AddComplexComponent( (BaseAddon) this, 2808, 2, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 13
				AddComplexComponent( (BaseAddon) this, 2808, 2, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 14
				AddComplexComponent( (BaseAddon) this, 2809, 0, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 15
				AddComplexComponent( (BaseAddon) this, 2809, 1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 16
			}
			else if ( RelRugID == 4 )
			{
				AddComplexComponent( (BaseAddon) this, 2755, -1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 1
				AddComplexComponent( (BaseAddon) this, 2807, 0, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 2
				AddComplexComponent( (BaseAddon) this, 2757, 2, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 3
				AddComplexComponent( (BaseAddon) this, 2807, 1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 4
				AddComplexComponent( (BaseAddon) this, 2751, 0, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 5
				AddComplexComponent( (BaseAddon) this, 2806, -1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 6
				AddComplexComponent( (BaseAddon) this, 2751, 1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 7
				AddComplexComponent( (BaseAddon) this, 2751, 0, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 8
				AddComplexComponent( (BaseAddon) this, 2751, 1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 9
				AddComplexComponent( (BaseAddon) this, 2754, 2, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 10
				AddComplexComponent( (BaseAddon) this, 2756, -1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 11
				AddComplexComponent( (BaseAddon) this, 2806, -1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 12
				AddComplexComponent( (BaseAddon) this, 2808, 2, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 13
				AddComplexComponent( (BaseAddon) this, 2808, 2, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 14
				AddComplexComponent( (BaseAddon) this, 2809, 0, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 15
				AddComplexComponent( (BaseAddon) this, 2809, 1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 16
			}
			else if ( RelRugID == 5 )
			{
				AddComplexComponent( (BaseAddon) this, 2753, 0, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 1
				AddComplexComponent( (BaseAddon) this, 2755, -1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 2
				AddComplexComponent( (BaseAddon) this, 2806, -1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 3
				AddComplexComponent( (BaseAddon) this, 2807, 0, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 4
				AddComplexComponent( (BaseAddon) this, 2753, 1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 5
				AddComplexComponent( (BaseAddon) this, 2753, 1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 6
				AddComplexComponent( (BaseAddon) this, 2754, 2, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 7
				AddComplexComponent( (BaseAddon) this, 2756, -1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 8
				AddComplexComponent( (BaseAddon) this, 2757, 2, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 9
				AddComplexComponent( (BaseAddon) this, 2806, -1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 10
				AddComplexComponent( (BaseAddon) this, 2807, 1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 11
				AddComplexComponent( (BaseAddon) this, 2808, 2, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 12
				AddComplexComponent( (BaseAddon) this, 2808, 2, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 13
				AddComplexComponent( (BaseAddon) this, 2809, 0, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 14
				AddComplexComponent( (BaseAddon) this, 2809, 1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 15
				AddComplexComponent( (BaseAddon) this, 2753, 0, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 16
			}
			else if ( RelRugID == 6 )
			{
				AddComplexComponent( (BaseAddon) this, 2771, -1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 1
				AddComplexComponent( (BaseAddon) this, 2773, 2, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 2
				AddComplexComponent( (BaseAddon) this, 2775, 0, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 3
				AddComplexComponent( (BaseAddon) this, 2775, 1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 4
				AddComplexComponent( (BaseAddon) this, 2770, 2, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 5
				AddComplexComponent( (BaseAddon) this, 2769, 0, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 6
				AddComplexComponent( (BaseAddon) this, 2769, 0, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 7
				AddComplexComponent( (BaseAddon) this, 2769, 1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 8
				AddComplexComponent( (BaseAddon) this, 2769, 1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 9
				AddComplexComponent( (BaseAddon) this, 2772, -1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 10
				AddComplexComponent( (BaseAddon) this, 2774, -1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 11
				AddComplexComponent( (BaseAddon) this, 2774, -1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 12
				AddComplexComponent( (BaseAddon) this, 2776, 2, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 13
				AddComplexComponent( (BaseAddon) this, 2776, 2, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 14
				AddComplexComponent( (BaseAddon) this, 2777, 0, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 15
				AddComplexComponent( (BaseAddon) this, 2777, 1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 16
			}
			else if ( RelRugID == 7 )
			{
				AddComplexComponent( (BaseAddon) this, 2797, 0, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 1
				AddComplexComponent( (BaseAddon) this, 2797, 1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 2
				AddComplexComponent( (BaseAddon) this, 2799, -1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 3
				AddComplexComponent( (BaseAddon) this, 2801, 2, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 4
				AddComplexComponent( (BaseAddon) this, 2802, -1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 5
				AddComplexComponent( (BaseAddon) this, 2803, 0, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 6
				AddComplexComponent( (BaseAddon) this, 2803, 1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 7
				AddComplexComponent( (BaseAddon) this, 2804, 2, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 8
				AddComplexComponent( (BaseAddon) this, 2797, 0, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 9
				AddComplexComponent( (BaseAddon) this, 2797, 1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 10
				AddComplexComponent( (BaseAddon) this, 2798, 2, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 11
				AddComplexComponent( (BaseAddon) this, 2800, -1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 12
				AddComplexComponent( (BaseAddon) this, 2802, -1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 13
				AddComplexComponent( (BaseAddon) this, 2804, 2, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 14
				AddComplexComponent( (BaseAddon) this, 2805, 0, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 15
				AddComplexComponent( (BaseAddon) this, 2805, 1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 16
			}
			else if ( RelRugID == 8 )
			{
				AddComplexComponent( (BaseAddon) this, 2796, 0, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 1
				AddComplexComponent( (BaseAddon) this, 2796, 0, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 2
				AddComplexComponent( (BaseAddon) this, 2796, 1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 3
				AddComplexComponent( (BaseAddon) this, 2796, 1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 4
				AddComplexComponent( (BaseAddon) this, 2798, 2, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 5
				AddComplexComponent( (BaseAddon) this, 2799, -1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 6
				AddComplexComponent( (BaseAddon) this, 2800, -1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 7
				AddComplexComponent( (BaseAddon) this, 2801, 2, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 8
				AddComplexComponent( (BaseAddon) this, 2802, -1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 9
				AddComplexComponent( (BaseAddon) this, 2802, -1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 10
				AddComplexComponent( (BaseAddon) this, 2803, 0, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 11
				AddComplexComponent( (BaseAddon) this, 2803, 1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 12
				AddComplexComponent( (BaseAddon) this, 2804, 2, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 13
				AddComplexComponent( (BaseAddon) this, 2804, 2, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 14
				AddComplexComponent( (BaseAddon) this, 2805, 0, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 15
				AddComplexComponent( (BaseAddon) this, 2805, 1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 16
			}
			else if ( RelRugID == 9 )
			{
				AddComplexComponent( (BaseAddon) this, 2758, 0, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 1
				AddComplexComponent( (BaseAddon) this, 2758, 1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 2
				AddComplexComponent( (BaseAddon) this, 2762, -1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 3
				AddComplexComponent( (BaseAddon) this, 2764, 2, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 4
				AddComplexComponent( (BaseAddon) this, 2765, -1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 5
				AddComplexComponent( (BaseAddon) this, 2766, 0, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 6
				AddComplexComponent( (BaseAddon) this, 2766, 1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 7
				AddComplexComponent( (BaseAddon) this, 2767, 2, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 8
				AddComplexComponent( (BaseAddon) this, 2758, 0, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 9
				AddComplexComponent( (BaseAddon) this, 2758, 1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 10
				AddComplexComponent( (BaseAddon) this, 2761, 2, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 11
				AddComplexComponent( (BaseAddon) this, 2763, -1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 12
				AddComplexComponent( (BaseAddon) this, 2765, -1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 13
				AddComplexComponent( (BaseAddon) this, 2767, 2, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 14
				AddComplexComponent( (BaseAddon) this, 2768, 0, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 15
				AddComplexComponent( (BaseAddon) this, 2768, 1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 16
			}
			else if ( RelRugID == 10 )
			{
				AddComplexComponent( (BaseAddon) this, 2759, 0, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 1
				AddComplexComponent( (BaseAddon) this, 2759, 1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 2
				AddComplexComponent( (BaseAddon) this, 2762, -1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 3
				AddComplexComponent( (BaseAddon) this, 2764, 2, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 4
				AddComplexComponent( (BaseAddon) this, 2765, -1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 5
				AddComplexComponent( (BaseAddon) this, 2766, 0, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 6
				AddComplexComponent( (BaseAddon) this, 2766, 1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 7
				AddComplexComponent( (BaseAddon) this, 2767, 2, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 8
				AddComplexComponent( (BaseAddon) this, 2759, 0, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 9
				AddComplexComponent( (BaseAddon) this, 2759, 1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 10
				AddComplexComponent( (BaseAddon) this, 2761, 2, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 11
				AddComplexComponent( (BaseAddon) this, 2763, -1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 12
				AddComplexComponent( (BaseAddon) this, 2765, -1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 13
				AddComplexComponent( (BaseAddon) this, 2767, 2, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 14
				AddComplexComponent( (BaseAddon) this, 2768, 0, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 15
				AddComplexComponent( (BaseAddon) this, 2768, 1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 16
			}
			else if ( RelRugID == 11 )
			{
				AddComplexComponent( (BaseAddon) this, 2760, 0, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 1
				AddComplexComponent( (BaseAddon) this, 2762, -1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 2
				AddComplexComponent( (BaseAddon) this, 2765, -1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 3
				AddComplexComponent( (BaseAddon) this, 2766, 0, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 4
				AddComplexComponent( (BaseAddon) this, 2760, 0, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 5
				AddComplexComponent( (BaseAddon) this, 2763, -1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 6
				AddComplexComponent( (BaseAddon) this, 2765, -1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 7
				AddComplexComponent( (BaseAddon) this, 2768, 0, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 8
				AddComplexComponent( (BaseAddon) this, 2760, 1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 9
				AddComplexComponent( (BaseAddon) this, 2764, 2, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 10
				AddComplexComponent( (BaseAddon) this, 2766, 1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 11
				AddComplexComponent( (BaseAddon) this, 2767, 2, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 12
				AddComplexComponent( (BaseAddon) this, 2760, 1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 13
				AddComplexComponent( (BaseAddon) this, 2761, 2, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 14
				AddComplexComponent( (BaseAddon) this, 2767, 2, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 15
				AddComplexComponent( (BaseAddon) this, 2768, 1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 16
			}
			else if ( RelRugID == 12 )
			{
				AddComplexComponent( (BaseAddon) this, 2778, 0, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 1
				AddComplexComponent( (BaseAddon) this, 2778, 0, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 2
				AddComplexComponent( (BaseAddon) this, 2778, 1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 3
				AddComplexComponent( (BaseAddon) this, 2778, 1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 4
				AddComplexComponent( (BaseAddon) this, 2779, 2, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 5
				AddComplexComponent( (BaseAddon) this, 2780, -1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 6
				AddComplexComponent( (BaseAddon) this, 2781, -1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 7
				AddComplexComponent( (BaseAddon) this, 2782, 2, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 8
				AddComplexComponent( (BaseAddon) this, 2783, -1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 9
				AddComplexComponent( (BaseAddon) this, 2783, -1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 10
				AddComplexComponent( (BaseAddon) this, 2784, 0, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 11
				AddComplexComponent( (BaseAddon) this, 2784, 1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 12
				AddComplexComponent( (BaseAddon) this, 2785, 2, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 13
				AddComplexComponent( (BaseAddon) this, 2785, 2, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 14
				AddComplexComponent( (BaseAddon) this, 2786, 0, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 15
				AddComplexComponent( (BaseAddon) this, 2786, 1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 16
			}
			else
			{
				AddComplexComponent( (BaseAddon) this, 2795, 0, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 1
				AddComplexComponent( (BaseAddon) this, 2795, 0, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 2
				AddComplexComponent( (BaseAddon) this, 2795, 1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 3
				AddComplexComponent( (BaseAddon) this, 2795, 1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 4
				AddComplexComponent( (BaseAddon) this, 2787, 2, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 5
				AddComplexComponent( (BaseAddon) this, 2788, -1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 6
				AddComplexComponent( (BaseAddon) this, 2789, -1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 7
				AddComplexComponent( (BaseAddon) this, 2790, 2, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 8
				AddComplexComponent( (BaseAddon) this, 2791, -1, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 9
				AddComplexComponent( (BaseAddon) this, 2791, -1, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 10
				AddComplexComponent( (BaseAddon) this, 2792, 0, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 11
				AddComplexComponent( (BaseAddon) this, 2792, 1, -1, 0, RelHue, -1, RelQuality + " carpet", 1);// 12
				AddComplexComponent( (BaseAddon) this, 2793, 2, 0, 0, RelHue, -1, RelQuality + " carpet", 1);// 13
				AddComplexComponent( (BaseAddon) this, 2793, 2, 1, 0, RelHue, -1, RelQuality + " carpet", 1);// 14
				AddComplexComponent( (BaseAddon) this, 2794, 0, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 15
				AddComplexComponent( (BaseAddon) this, 2794, 1, 2, 0, RelHue, -1, RelQuality + " carpet", 1);// 16
			}

			RelicGoldValue = RelCost;
			RelicMainID = RelID;
			RelicRugID = RelRugID;
			RelicColor = RelHue;
			RelicQuality = RelQuality;
		}

		public DDRelicRugAddon( Serial serial ) : base( serial )
		{
		}

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource)
        {
            AddComplexComponent(addon, item, xoffset, yoffset, zoffset, hue, lightsource, null, 1);
        }

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource, string name, int amount)
        {
            AddonComponent ac;
            ac = new AddonComponent(item);
            if (name != null && name.Length > 0)
                ac.Name = name;
            if (hue != 0)
                ac.Hue = hue;
            if (amount > 1)
            {
                ac.Stackable = true;
                ac.Amount = amount;
            }
            if (lightsource != -1)
                ac.Light = (LightType) lightsource;
            addon.AddComponent(ac, xoffset, yoffset, zoffset);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
            writer.Write( RelicGoldValue );
            writer.Write( RelicMainID );
            writer.Write( RelicRugID );
            writer.Write( RelicFound );
            writer.Write( RelicColor );
            writer.Write( RelicQuality );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
            RelicGoldValue = reader.ReadInt();
            RelicMainID = reader.ReadInt();
            RelicRugID = reader.ReadInt();
            RelicFound = reader.ReadInt();
            RelicColor = reader.ReadInt();
            RelicQuality = reader.ReadString();
		}
	}

	public class DDRelicRugAddonDeed : BaseAddonDeed, IRelic
	{
		public override void ItemIdentified( bool id )
		{
			m_NotIdentified = id;
			if ( !id )
			{
				ColorHue3 = "FDC844";
				ColorText3 = "Worth " + CoinPrice + " Gold";
			}
			else
			{
				ColorHue3 = null;
				ColorText3 = null;
			}
		}

		public override void ItemPriced( int val )
		{
			if ( !m_NotIdentified )
			{
				ColorHue3 = "FDC844";
				ColorText3 = "Worth " + val + " Gold";
			}
			else
			{
				ColorHue3 = null;
				ColorText3 = null;
			}
			m_CoinPrice = val;
		}

		public int RelicMainID;
		public int RelicRugID;
		public int RelicFound;
		public int RelicColor;
		public string RelicQuality;

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_Color { get { return RelicColor; } set { RelicColor = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_MainID { get { return RelicMainID; } set { RelicMainID = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_RugID { get { return RelicRugID; } set { RelicRugID = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_Found { get { return RelicFound; } set { RelicFound = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Relic_Quality { get { return RelicQuality; } set { RelicQuality = value; InvalidateProperties(); } }

		public override BaseAddon Addon
		{
			get
			{
				return new DDRelicRugAddon( CoinPrice, Relic_MainID, Relic_RugID, Hue, Relic_Quality );
			}
		}

		[Constructable]
		public DDRelicRugAddonDeed() : this( 0, 0, 0, 0, "blank", 0 )
		{
		}

		[Constructable]
		public DDRelicRugAddonDeed( int RelCost, int RelID, int RelRugID, int RelHue, string RelQuality, int RelRolled )
		{
			Weight = 60;

			if ( Relic_Found > 0 ) { RelicFound = 1; }

			/// QUALITY ////////////////
			string sLook = "";
			if ( Relic_Found > 0 ){ sLook = Relic_Quality; RelicQuality = Relic_Quality; }
			else if ( RelQuality != "blank" ){ sLook = RelQuality; RelicQuality = RelQuality; }
			else
			{
				sLook = "a rare";
				switch ( Utility.RandomMinMax( 0, 18 ) )
				{
					case 0:	sLook = "a rare";	break;
					case 1:	sLook = "a nice";	break;
					case 2:	sLook = "a pretty";	break;
					case 3:	sLook = "a superb";	break;
					case 4:	sLook = "a delightful";	break;
					case 5:	sLook = "an elegant";	break;
					case 6:	sLook = "an exquisite";	break;
					case 7:	sLook = "a fine";	break;
					case 8:	sLook = "a gorgeous";	break;
					case 9:	sLook = "a lovely";	break;
					case 10:sLook = "a magnificent";	break;
					case 11:sLook = "a marvelous";	break;
					case 12:sLook = "a splendid";	break;
					case 13:sLook = "a wonderful";	break;
					case 14:sLook = "an extraordinary";	break;
					case 15:sLook = "a strange";	break;
					case 16:sLook = "an odd";	break;
					case 17:sLook = "a unique";	break;
					case 18:sLook = "an unusual";	break;
				}
				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0:	sLook = sLook + ", decorative";		break;
					case 1:	sLook = sLook + ", ceremonial";		break;
					case 2:	sLook = sLook + ", ornamental";		break;
				}
				RelicQuality = sLook;
			}

			/// VALUE ////////////////
			if ( Relic_Found > 0 ){}
			else if ( RelCost > 0 ){ CoinPrice = RelCost; }
			else { CoinPrice = Utility.RandomMinMax( 80, 500 ); NotIdentified = true; NotIDSource = Identity.Merchant; NotIDSkill = IDSkill.Mercantile; }

			/// COLOR ////////////////
			if ( Relic_Found > 0 ){ RelicColor = Relic_Color; Hue  = Relic_Color; }
			else if ( RelRolled > 0 ){ RelicColor = RelHue; Hue = RelHue; }
			else
			{
				Hue = Utility.RandomColor(0);
				RelicColor = Hue;
			}

			/// ITEMID ////////////////
			if ( Relic_Found > 0 ){ RelicMainID = Relic_MainID; ItemID = Relic_MainID; }
			else if ( RelID > 0 ){ RelicMainID = RelID; ItemID = RelID; }
			else { ItemID = Utility.RandomList( 0x3F08, 0x3F09, 0x3F0A, 0x3F0D, 0x3F17, 0x3F18, 0x3F11 ); RelicMainID = ItemID; }

			/// RUGID ////////////////
			if ( Relic_Found > 0 ){ RelicRugID = Relic_RugID; }
			else if ( RelRugID > 0 ){ RelicRugID = RelRugID; }
			else { RelicRugID = Utility.RandomMinMax( 1, 13 ); }

			Name = sLook + " carpet";
			RelicFound = 1;
		}

		public DDRelicRugAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) && MySettings.S_IdentifyItemsOnlyInPack && from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified ) 
				from.SendMessage( "This must be in your backpack to identify." );
			else if ( from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified )
				IDCommand( from );
			else if ( NotIdentified )
				from.SendMessage( "This must be appraised before you can place it in your home." );
			else
				base.OnDoubleClick( from );
		}

		public override void IDCommand( Mobile m )
		{
			if ( this.NotIDSkill == IDSkill.Tasting )
				RelicFunctions.IDItem( m, m, this, SkillName.Tasting );
			else if ( this.NotIDSkill == IDSkill.ArmsLore )
				RelicFunctions.IDItem( m, m, this, SkillName.ArmsLore );
			else
				RelicFunctions.IDItem( m, m, this, SkillName.Mercantile );
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);

			if ( NotIdentified )
				list.Add( 1049644, "Appraise Before Placing in Your Home");
			else
				list.Add( 1049644, "Double Click To Place In Your Home");
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 1 ); // Version
            writer.Write( RelicMainID );
            writer.Write( RelicRugID );
            writer.Write( RelicFound );
            writer.Write( RelicColor );
            writer.Write( RelicQuality );
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version < 1 )
				CoinPrice = reader.ReadInt();

            RelicMainID = reader.ReadInt();
            RelicRugID = reader.ReadInt();
            RelicFound = reader.ReadInt();
            RelicColor = reader.ReadInt();
            RelicQuality = reader.ReadString();
		}
	}
}