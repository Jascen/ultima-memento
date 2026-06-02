using System;
using Scripts.Mythik.Systems.Achievements;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
	public class Tasting
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Tasting].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.Target = new InternalTarget();

			m.SendLocalizedMessage( 502807 ); // What would you like to taste?

			return TimeSpan.FromSeconds( 1.0 );
		}

		[PlayerVendorTarget]
		private class InternalTarget : Target
		{
			public InternalTarget() :  base ( 2, false, TargetFlags.None )
			{
				AllowNonlocal = true;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( IsExotic( targeted ) )
				{
					ExplicitAchievement.TryAward(ExplicitAchievementType.ExoticTaste, from as PlayerMobile);
				}
				else if ( targeted is Mobile )
				{
					from.SendLocalizedMessage( 502816 ); // You feel that such an action would be inappropriate.
				}
				else if ( targeted is Food )
				{
					Food food = (Food)targeted;

					if ( food.Poison != null )
					{
						if ( from.CheckTargetSkill( SkillName.Tasting, food, 0, 125 ) )
						{
							// It appears to have a bitter taste of poison.
							if ( food.Poison == Poison.Lesser )
								from.SendLocalizedMessage( 1041579 );
							else if ( food.Poison == Poison.Regular )
								from.SendLocalizedMessage( 1041580 );
							else if ( food.Poison == Poison.Greater )
								from.SendLocalizedMessage( 1041581 );
							else if ( food.Poison == Poison.Deadly )
								from.SendLocalizedMessage( 1041582 );
							else if ( food.Poison == Poison.Lethal )
								from.SendLocalizedMessage( 1041583 );
							else
								from.SendLocalizedMessage( 1010600 ); // You detect nothing unusual about this substance.
						}
						else
						{
							food.Eat( from, false );
							from.SendMessage( "You bit off a bit too much!" );
						}
					}
					else
					{
						from.SendMessage( "This food looks safe to eat." );
					}
				}
				else if ( targeted is BaseBeverage )
				{
					BaseBeverage drink = (BaseBeverage)targeted;

					if ( drink.Poison != null )
					{
						if ( from.CheckTargetSkill( SkillName.Tasting, drink, 0, 125 ) )
						{
							// It appears to have a bitter taste of poison.
							if ( drink.Poison == Poison.Lesser )
								from.SendLocalizedMessage( 1041579 );
							else if ( drink.Poison == Poison.Regular )
								from.SendLocalizedMessage( 1041580 );
							else if ( drink.Poison == Poison.Greater )
								from.SendLocalizedMessage( 1041581 );
							else if ( drink.Poison == Poison.Deadly )
								from.SendLocalizedMessage( 1041582 );
							else if ( drink.Poison == Poison.Lethal )
								from.SendLocalizedMessage( 1041583 );
							else
								from.SendLocalizedMessage( 1010600 ); // You detect nothing unusual about this substance.
						}
						else
						{
							drink.Pour_OnTarget( from, from );
							from.SendMessage( "You swallowed a bit too much!" );
						}
					}
					else
					{
						from.SendMessage( "This liquid looks safe to drink." );
					}
				}
				else if ( targeted is Item )
				{
					Item examine = (Item)targeted;
					RelicFunctions.IDItem( from, from, examine, SkillName.Tasting );
				}
			}

			private bool IsExotic( object o )
			{
				return o is BullFrog
					|| o is FireToad
					|| o is Frog
					|| o is GiantToad
					|| o is IceToad
					|| o is Toad
					|| o is PoisonFrog
					
					|| o is DriedToad
					
					|| ( o is Item && ((Item)o).ItemID == 0x2C9B );
			}
		}
	}
}