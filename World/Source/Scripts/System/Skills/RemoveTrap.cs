using System;
using Server.ModernSkill;
using Server.Targeting;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.SkillHandlers
{
	public class RemoveTrap
	{
		private enum TrapDifficulty
		{
			NotTrapped = 0,
			Trivial,
			Easy,
			Difficult,
			Challenging,
			Impossible,
		}

		public static string GetDifficulty( double removeTrapSkill, object target )
		{
			switch (GetDifficultyInternal(removeTrapSkill, target))
			{
				default:
				case TrapDifficulty.NotTrapped: return TextDefinition.GetColorizedText("Not Trapped", HtmlColors.COOL_GREEN);
				case TrapDifficulty.Trivial: return  TextDefinition.GetColorizedText("Trivial", HtmlColors.MINT_GREEN);
				case TrapDifficulty.Easy: return TextDefinition.GetColorizedText("Easy", HtmlColors.MINT_GREEN);
				case TrapDifficulty.Difficult: return TextDefinition.GetColorizedText("Difficult", HtmlColors.RUST);
				case TrapDifficulty.Challenging: return TextDefinition.GetColorizedText("Challenging", HtmlColors.PALE_RED);
				case TrapDifficulty.Impossible: return TextDefinition.GetColorizedText("Impossible", HtmlColors.RED);
			}
		}

		private static TrapDifficulty GetDifficultyInternal( double removeTrapSkill, object target )
		{
			if ( target is TrapableContainer )
			{
				var container = (TrapableContainer)target;
				if ( !container.IsActive ) return TrapDifficulty.NotTrapped;

				var trapSkillLevel = container.TrapDifficulty;
				if ( removeTrapSkill < trapSkillLevel ) return TrapDifficulty.Impossible;

				var trapDifficulty = trapSkillLevel + 20;
				var delta = trapDifficulty - removeTrapSkill;
				if ( delta < 1 ) return TrapDifficulty.Trivial;
				if ( delta < 10 ) return TrapDifficulty.Easy;
				if ( delta < 20 ) return TrapDifficulty.Difficult;
				return TrapDifficulty.Challenging;
			}
			else if ( target is HiddenTrap )
			{
				var hiddenTrap = (HiddenTrap)target;
				if ( !HiddenTrap.IsActiveTrap( hiddenTrap ) ) return TrapDifficulty.NotTrapped;

				var delta = hiddenTrap.TrapDifficulty - removeTrapSkill;
				if ( delta < 1 ) return TrapDifficulty.Trivial;
				if ( delta < 10 ) return TrapDifficulty.Easy;
				if ( delta < 20 ) return TrapDifficulty.Difficult;
				return TrapDifficulty.Challenging;
			}

			return TrapDifficulty.NotTrapped;
		}

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

		public static bool CanDoEffect( Mobile from, object targeted )
		{
			if ( targeted is Mobile )
			{
				from.SendLocalizedMessage( 502816 ); // You feel that such an action would be inappropriate
				return false;
			}

			if ( targeted is TrapableContainer )
			{
				TrapableContainer targ = (TrapableContainer)targeted;

				if ( !targ.IsActive )
				{
					from.LocalOverheadMessage(MessageType.Regular, 1150, 502373 ); // That doesn't appear to be trapped
					return false;
				}

				if ( (int)from.Skills[SkillName.RemoveTrap].Value < targ.TrapDifficulty )
				{
					from.SendMessage( "This trap looks too complicated for you." );
					return false;
				}

				return true;
			}
			
			if ( targeted is HiddenTrap )
			{
				HiddenTrap trapt = (HiddenTrap)targeted;

				if ( HiddenTrap.IsActiveTrap( trapt ) )
				{
					return true;
				}
			}

			from.LocalOverheadMessage(MessageType.Regular, 1150, 502373 ); // That doesn't appear to be trapped
			return false;
		}

		public static void DoEffect( Mobile from, object targeted )
		{
			if ( targeted is TrapableContainer )
			{
				TrapableContainer targ = (TrapableContainer)targeted;

				from.Direction = from.GetDirectionTo( targ );

				int nTrapLevel = (targ.TrapLevel * 10) + 20;
				if ( from.CheckTargetSkill( SkillName.RemoveTrap, targ, 0, nTrapLevel ) )
				{
					targ.TrapPower = 0;
					targ.TrapLevel = 0;
					targ.TrapType = TrapType.None;
					from.LocalOverheadMessage(MessageType.Regular, 1150, 502377 ); // You successfully render the trap harmless
				}
				else
				{
					from.SendLocalizedMessage( 502372 ); // You fail to disarm the trap... but you don't set it off
				}
			}
			else if ( targeted is HiddenTrap )
			{
				HiddenTrap trapt = (HiddenTrap)targeted;
				if ( HiddenTrap.IsActiveTrap( trapt ) )
				{
					from.Direction = from.GetDirectionTo( trapt );
					from.PlaySound( 0x241 );
					
					if ( from.CheckSkill( SkillName.RemoveTrap, 0, trapt.TrapDifficulty ) )
					{
						HiddenTrap.DisableTrap( trapt );
						from.LocalOverheadMessage(MessageType.Regular, 1150, 502377 ); // You successfully render the trap harmless
					}
					else
					{
						from.SendLocalizedMessage( 502372 ); // You fail to disarm the trap... but you don't set it off
					}
				}
			}

			from.NextSkillTime = DateTime.Now + TimeSpan.FromSeconds( 5.0 );
		}

		private class InternalTarget : Target
		{
			public InternalTarget() :  base ( 2, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( from is PlayerMobile && targeted is Item )
				{
					var player = (PlayerMobile)from;
					if ( player.Preferences.ModernRemoveTrapEnabled && LockpickAndRemoveTrapGump.TryShow(player, (Item)targeted))
						return;
				}

				if ( CanDoEffect( from, targeted ) )
				{
					DoEffect( from, targeted );
				}
			}
		}
	}
}