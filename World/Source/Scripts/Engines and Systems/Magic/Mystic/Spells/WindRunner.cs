using System;
using Server.Targeting;
using System.Collections;
using Server.Items;
using Server.Mobiles;

namespace Server.Spells.Mystic
{
	public class WindRunner : MysticSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 259,
					IconGraphic = 0x19,
					Name = "Wind Runner",
					PowerWords = "Beh Ra Mu Ahm",
					Description = "This ability allows the monk to run as fast as a steed. This ability should be avoided if you already have a mount you are riding, or perhaps you have magical boots that allow you to run at this speed. using this ability in such conditions may cause unusual travel speeds, so be leery.",
					ManaCost = 50,
					TithingCost = 250,
					MinSkill = 70,
					TargetType = TargetFlags.None
				},
				269,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3 ); } }
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }
		public override int MysticSpellCircle{ get{ return 2; } }

		public WindRunner( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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
				BuffInfo.RemoveBuff( m, BuffIcon.WindRunner );
				m.EndAction( typeof( WindRunner ) );
				m.PlaySound( 0x64C ); // Cleansing winds
				m.SendMessage("You feel the wind around you dissipate");
				FastPlayer.Refresh(m as PlayerMobile);
			}
		}

		public override void OnCast()
		{
			Item shoes = Caster.FindItemOnLayer( Layer.Shoes );

            if ( Caster.Mounted )
            {
                Caster.SendMessage( "You cannot use this ability while on a mount!" );
            }
			else
			{
				if ( !Caster.CanBeginAction( typeof( WindRunner ) ) )
				{
					StopTimer( Caster );
				}

				Caster.BeginAction( typeof( WindRunner ) );

				int TotalTime = (int)( Caster.Skills[SkillName.FistFighting].Value * 5 );
				if ( TotalTime < 600 ){ TotalTime = 600; }

				m_Timers[Caster] = Timer.DelayCall(TimeSpan.FromSeconds( TotalTime ), () => RemoveEffect(Caster));
				FastPlayer.Refresh(Caster as PlayerMobile);

				BuffInfo.RemoveBuff( Caster, BuffIcon.WindRunner );
				BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.WindRunner, 1063516, TimeSpan.FromSeconds( TotalTime ), Caster ) );
				Point3D air = new Point3D( ( Caster.X+1 ), ( Caster.Y+1 ), ( Caster.Z+5 ) );
				Effects.SendLocationParticles(EffectItem.Create(air, Caster.Map, EffectItem.DefaultDuration), 0x5590, 9, 32, 0, 0, 5022, 0);
				Caster.PlaySound( 0x64F ); // Hail storm

			}

            FinishSequence();
		}
	}
}