using System;
using Server.Targeting;
using Server.Items;

namespace Server.Spells.Syth
{
	public class DeathGrip : SythSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 271,
					IconGraphic = 0x03EA,
					Name = "Death Grip",
					PowerWords = "Zayin Kun",
					Description = "The Syth can reach out with their hatred and anger, psychically gripping a foe for 5-20 points of damage every few seconds. This effect lasts for 2-75 seconds. The more a Syth is skilled with swords and tactics, and their accumulated hatred and anger, increases the effectiveness of this power.",
					ManaCost = 8,
					TithingCost = 16,
					MinSkill = 20,
					TargetType = TargetFlags.Harmful
				},
				203,
				0
			);

		public int CirclePower = 1;
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public DeathGrip( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			Caster.SendMessage( "Who do you want to put to grip?" );
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				TimeSpan duration = TimeSpan.FromSeconds( GetSythDamage(Caster) / 5 );

				Point3D grip = new Point3D( m.X+1, m.Y+1, m.Z+20 );
				Effects.SendLocationParticles(EffectItem.Create(grip, m.Map, EffectItem.DefaultDuration), 0x4CE3, 9, 32, 0x8BF-1, 0, 5022, 0);
				m.PlaySound( 0x65A );

				int min = 5;
				int max = ( (int)( GetSythDamage(Caster) / 25 ) + 5 );
				AOS.Damage( m, Caster, Utility.RandomMinMax( min, max ), true, 100, 0, 0, 0, 0 );

				new GripTimer( m, duration, min, max, Caster ).Start();

				HarmfulSpell( m );
			}
		}

		public class InternalTarget : Target
		{
			private DeathGrip m_Owner;

			public InternalTarget( DeathGrip owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}

		public class GripTimer : Timer
		{
			private Mobile m_m;
			private Mobile m_Caster;
			private DateTime m_Expire;
			private int m_Time;
			private int m_Min;
			private int m_Max;

			public GripTimer( Mobile sleeper, TimeSpan duration, int min, int max, Mobile caster ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 0.1 ) )
			{
				m_m = sleeper;
				m_Caster = caster;
				m_Expire = DateTime.Now + duration;
				m_Time = 0;
				m_Min = min;
				m_Max = max;
			}

			protected override void OnTick()
			{
				m_Time++;
				if ( m_Time > 60 && m_m != null && m_Caster != null )
				{
					m_Time = 0;
					AOS.Damage( m_m, m_Caster, Utility.RandomMinMax( m_Min, m_Max ), true, 100, 0, 0, 0, 0 );
					m_Caster.DoHarmful( m_m );
					m_Caster.RevealingAction();
					Point3D grip = new Point3D( m_m.X+1, m_m.Y+1, m_m.Z+10 );
					Effects.SendLocationParticles(EffectItem.Create(grip, m_m.Map, EffectItem.DefaultDuration), 0x4CE3, 9, 32, 0x8BF-1, 0, 5022, 0);
					m_m.PlaySound( 0x65A );
				}

				if ( DateTime.Now >= m_Expire )
				{
					Stop();
				}
			}
		}
	}
}