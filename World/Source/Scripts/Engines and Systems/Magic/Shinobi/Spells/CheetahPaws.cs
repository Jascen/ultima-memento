using System;
using Server.Targeting;
using System.Collections;
using Server.Mobiles;

namespace Server.Spells.Shinobi
{
	public class CheetahPaws : ShinobiSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 290,
					IconGraphic = 10876,
					Name = "Cheetah Paws",
					PowerWords = "Chita no ashi",
					Description = "This increases the running speed of the Ninja for about 10-25 minutes, making them run as fast as a cheetah. This power cannot be called upon within certain areas and will often cease to function when entering those areas.",
					ManaCost = 60,
					TithingCost = 65,
					MinSkill = 80,
					TargetType = TargetFlags.None
				},
				-1,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public CheetahPaws( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

        private readonly static Hashtable m_Timers = new Hashtable();

        public static bool StopTimer(Mobile m)
        {
            Timer t = (Timer)m_Timers[m];

            if (t != null)
            {
                t.Stop();
                m_Timers.Remove(m);
            }

            return (t != null);
        }

		public static bool UnderEffect( Mobile m )
		{
			return m_Timers.Contains( m );
		}

		public static void RemoveEffect( Mobile m )
		{
			if (StopTimer( m ))
			{
				BuffInfo.RemoveBuff( m, BuffIcon.CheetahPaws );
				m.EndAction( typeof( CheetahPaws ) );
				m.PlaySound( 0x64C ); // Cleansing winds
				m.SendMessage("You feel the wind around you dissipate");
				FastPlayer.Refresh(m as PlayerMobile);
			}
		}

		public override void OnCast()
		{
            if ( Caster.Mounted )
            {
                Caster.SendMessage( "You cannot use this ability while on a mount!" );
            }
			else if ( CheckSequence( ) )
			{
				if ( !Caster.CanBeginAction( typeof( CheetahPaws ) ) )
				{
					RemoveEffect( Caster );
				}

				Caster.BeginAction( typeof( CheetahPaws ) );

				int TotalTime = (int)( Caster.Skills[SkillName.Ninjitsu].Value * 5 );
				if ( TotalTime < 600 ){ TotalTime = 600; }

				m_Timers[Caster] = Timer.DelayCall(TimeSpan.FromSeconds( TotalTime ), () => RemoveEffect(Caster));
				FastPlayer.Refresh(Caster as PlayerMobile);

				BuffInfo.RemoveBuff( Caster, BuffIcon.CheetahPaws );
				BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.CheetahPaws, 1063490, TimeSpan.FromSeconds( TotalTime ), Caster ) );
				Caster.PlaySound( 0x077 ); // Cougar
			}

            FinishSequence();
		}
	}
}