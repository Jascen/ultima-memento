using System;
using System.Collections;
using Server.Items;
using Server.Mobiles;

namespace Server.Spells.Jedi
{
	public class Celerity : JediSpell
	{
		public override int spellIndex { get { return 284; } }
		public int CirclePower = 3;
		public static int spellID = 284;
		public override int RequiredTithing{ get{ return Int32.Parse(  Server.Spells.Jedi.JediSpell.SpellInfo( spellIndex, 10 )); } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }
		public override double RequiredSkill{ get{ return (double)(Int32.Parse( Server.Spells.Jedi.JediSpell.SpellInfo( spellIndex, 2 ))); } }
		public override int RequiredMana{ get{ return Int32.Parse(  Server.Spells.Jedi.JediSpell.SpellInfo( spellIndex, 3 )); } }

		private static SpellInfo m_Info = new SpellInfo(
				Server.Spells.Jedi.JediSpell.SpellInfo( spellID, 1 ),
				Server.Misc.Research.CapsCast( Server.Spells.Jedi.JediSpell.SpellInfo( spellID, 4 ) ),
				-1,
				0
			);

		public Celerity( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
				BuffInfo.RemoveBuff( m, BuffIcon.Celerity );
				m.EndAction( typeof( Celerity ) );
				m.PlaySound( 0x64C ); // Cleansing winds
				m.SendMessage("You feel the wind around you dissipate");
				FastPlayer.Refresh(m as PlayerMobile);
			}
		}

		public override void OnCast()
		{
            if ( Caster.Mounted )
            {
                Caster.SendMessage( "You cannot use this power while on a mount!" );
            }
			else if ( CheckSequence() )
			{
				if ( !Caster.CanBeginAction( typeof( Celerity ) ) )
				{
					StopTimer( Caster );
				}

				Caster.BeginAction( typeof( Celerity ) );

				int TotalTime = (int)( GetJediDamage( Caster ) * 4 );
				if ( TotalTime < 600 ){ TotalTime = 600; }

				m_Timers[Caster] = Timer.DelayCall(TimeSpan.FromSeconds( TotalTime ), () => RemoveEffect(Caster));
				FastPlayer.Refresh(Caster as PlayerMobile);
				BuffInfo.RemoveBuff( Caster, BuffIcon.Celerity );
				BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.Celerity, 1063516, TimeSpan.FromSeconds( TotalTime ), Caster ) );
				Point3D air = new Point3D( ( Caster.X+1 ), ( Caster.Y+1 ), ( Caster.Z+5 ) );
				Effects.SendLocationParticles(EffectItem.Create(air, Caster.Map, EffectItem.DefaultDuration), 0x5590, 9, 32, 0, 0, 5022, 0);
				Caster.PlaySound( 0x64F ); // Hail storm
				DrainCrystals( Caster, RequiredTithing );
			}

            FinishSequence();
		}
	}
}