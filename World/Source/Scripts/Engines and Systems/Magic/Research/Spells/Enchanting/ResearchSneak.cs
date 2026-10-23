using System;
using Server;
using System.Collections;
using Server.Network;
using System.Text;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchSneak : ResearchSpell
	{
		public override int spellIndex { get { return 3; } }
		public int CirclePower = 1;
		public static int spellID = 3;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.75 ); } }
		public override double RequiredSkill{ get{ return (double)(Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 8 ))); } }
		public override int RequiredMana{ get{ return Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 7 )); } }

		private static SpellInfo m_Info = new SpellInfo(
				Server.Misc.Research.SpellInformation( spellID, 2 ),
				Server.Misc.Research.CapsCast( Server.Misc.Research.SpellInformation( spellID, 4 ) ),
				212,
				9061,
				Reagent.FairyEgg,Reagent.PixieSkull
			);

		public ResearchSneak( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

        public static void Initialize()
        {
			EventSink.Logout += OnLogout;
        }

        public override void OnCast()
        {
			Caster.Target = new InternalTarget( this );
        }

        private static void OnLogout(LogoutEventArgs e)
        {
			ResearchSneak.RemoveEffect(e.Mobile);
        }

		private static Hashtable m_Table = new Hashtable();

		public static bool HasEffect( Mobile m )
		{
			return ( m_Table[m] != null );
		}

		public static void RemoveEffect( Mobile m )
		{
			object[] mods = (object[])m_Table[m];
			
			if ( mods != null )
			{
				m.RemoveSkillMod( (SkillMod)mods[0] );
				m.RemoveSkillMod( (SkillMod)mods[1] );
			}
			
			m_Table.Remove( m );
			m.EndAction( typeof( ResearchSneak ) );
			m.Hidden = false;
			BuffInfo.RemoveBuff( m, BuffIcon.Sneak );
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckBSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Effects.SendLocationParticles( EffectItem.Create( m.Location, m.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 0, 0, 5042, 0 );
				Effects.PlaySound( m, m.Map, 0x201 );

				int percentage = (int)(SpellHelper.GetOffsetScalar( Caster, m, false )*100);
				TimeSpan length = SpellHelper.GetDuration( Caster, m );

				int MyHide = 100 - (int)m.Skills[SkillName.Hiding].Base;
					if ( MyHide < 0 ){ MyHide = 0; }
				int MyStealth = 100 - (int)m.Skills[SkillName.Stealth].Base;
					if ( MyStealth < 0 ){ MyStealth = 0; }

				object[] mods = new object[]
				{
					new DefaultSkillMod( SkillName.Hiding, true, MyHide ),
					new DefaultSkillMod( SkillName.Stealth, true, MyStealth ),
				};

				m_Table[m] = mods;

				m.AddSkillMod( (SkillMod)mods[0] );
				m.AddSkillMod( (SkillMod)mods[1] );

				double time = DamagingSkill( Caster )*2;
					if ( time > 480 ){ time = 480.0; }
					if ( time < 120 ){ time = 120.0; }

				new InternalTimer( m, TimeSpan.FromSeconds( time ) ).Start();

				m.Hidden = true;

				BuffInfo.RemoveBuff( m, BuffIcon.Sneak );
				BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.Sneak, 1063654, 1063655, length, m));

				m.BeginAction( typeof( ResearchSneak ) );
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
			}

			FinishSequence();
		}

		private class InternalTimer : Timer
		{
			private Mobile m_m;
			private DateTime m_Expire;
			
			public InternalTimer( Mobile m, TimeSpan duration ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 0.1 ) )
			{
				m_m = m;
				m_Expire = DateTime.Now + duration;
				
			}
			
			protected override void OnTick()
			{
				if ( DateTime.Now >= m_Expire )
				{
					ResearchSneak.RemoveEffect( m_m );
					Stop();
				}
			}
		}

		private class InternalTarget : Target
		{
			private ResearchSneak m_Owner;

			public InternalTarget( ResearchSneak owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Beneficial )
			{
				m_Owner = owner;
			} 

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}