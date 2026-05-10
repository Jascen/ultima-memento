using System;
using Server.Targeting;
using Server.Items;

namespace Server.SkillHandlers
{
	public class RemoveTrap
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.RemoveTrap].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.Target = new InternalTarget();

			m.SendLocalizedMessage( 502368 ); // Which trap will you attempt to disarm?

			return TimeSpan.FromSeconds( 1.0 );
		}

		private class InternalTarget : Target
		{
			public InternalTarget() :  base ( 2, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Mobile )
				{
					from.SendLocalizedMessage( 502816 ); // You feel that such an action would be inappropriate
					return;
				}
				else if ( targeted is TrapableContainer )
				{
					TrapableContainer targ = (TrapableContainer)targeted;

					from.Direction = from.GetDirectionTo( targ );

					int nTrapLevel = targ.TrapLevel * 10;

					if ( targ.TrapType == TrapType.None )
					{
						from.SendLocalizedMessage( 502373 ); // That doesn't appear to be trapped
						return;
					}

					if ( (int)(from.Skills[SkillName.RemoveTrap].Value ) < nTrapLevel )
					{
						from.SendMessage( "This trap looks too complicated for you." );
						return;
					}

					from.PlaySound( 0x241 );

					// if ( from.CheckTargetSkill( SkillName.RemoveTrap, targ, targ.TrapPower, targ.TrapPower + 30 ) )
					nTrapLevel = nTrapLevel + 20;
					if ( from.CheckTargetSkill( SkillName.RemoveTrap, targ, 0, nTrapLevel ) )
					{
						targ.TrapPower = 0;
						targ.TrapLevel = 0;
						targ.TrapType = TrapType.None;
						from.SendLocalizedMessage( 502377 ); // You successfully render the trap harmless
					}
					else
					{
						from.SendLocalizedMessage( 502372 ); // You fail to disarm the trap... but you don't set it off
					}
				}
				else if ( targeted is Item )
				{
					Item trapt = (Item)targeted;

					if ( trapt is HiddenTrap && trapt.Weight < 5.0 )
					{
						from.PlaySound( 0x241 );
						
						if ( from.CheckSkill( SkillName.RemoveTrap, 0, 125 ) )
						{
							HiddenTrap.DisableTrap( trapt );
							from.SendLocalizedMessage( 502377 ); // You successfully render the trap harmless
						}
						else
						{
							from.SendLocalizedMessage( 502372 ); // You fail to disarm the trap... but you don't set it off
						}
					}
					else { from.SendLocalizedMessage( 502373 ); }
				}
				else
				{
					from.SendLocalizedMessage( 502373 ); // That does'nt appear to be trapped
					return;
				}

				from.NextSkillTime = DateTime.Now + TimeSpan.FromSeconds( 5.0 );
			}
		}
	}
}