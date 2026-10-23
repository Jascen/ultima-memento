using System;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Syth
{
	public class DrainLife : SythSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 278,
					IconGraphic = 0x0402,
					Name = "Drain Life",
					PowerWords = "Derriphan Tyuk",
					Description = "This power will drain about 10-15 points of life, every few seconds, from a creature for 10-60 seconds, where such life is transferred to the Syth. This power cannot affect supernatural creatures, constructs, golems, or elementals.",
					ManaCost = 36,
					TithingCost = 52,
					MinSkill = 90,
					TargetType = TargetFlags.Harmful
				},
				203,
				0
			);

		public int CirclePower = 7;
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public DrainLife( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			Caster.SendMessage( "Who do you want to put to drain life from?" );
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			bool CanAffect = true;

			if ( m is BaseCreature )
			{
				SlayerEntry undead = SlayerGroup.GetEntryByName( SlayerName.Silver );
				SlayerEntry elly = SlayerGroup.GetEntryByName( SlayerName.ElementalBan );
				SlayerEntry golem = SlayerGroup.GetEntryByName( SlayerName.GolemDestruction );
				if (undead.Slays(m) || elly.Slays(m) || golem.Slays(m))
				{
					CanAffect = false;
				}
			}

			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( !CanAffect )
			{
				Caster.SendMessage( "This power cannot affect supernatural creatures, golems, constructs, or elementals." );
			}
			else if ( CheckHSequence( m ) )
			{
				Point3D blast1 = new Point3D( ( m.X ), ( m.Y ), m.Z );
				Point3D blast2 = new Point3D( ( m.X-1 ), ( m.Y ), m.Z );
				Point3D blast3 = new Point3D( ( m.X+1 ), ( m.Y ), m.Z );
				Point3D blast4 = new Point3D( ( m.X ), ( m.Y-1 ), m.Z );
				Point3D blast5 = new Point3D( ( m.X ), ( m.Y+1 ), m.Z );

				Effects.SendLocationEffect( blast1, m.Map, 0x3400, 60, 0xB76, 0 );
				Effects.SendLocationEffect( blast2, m.Map, 0x3400, 60, 0xB76, 0 );
				Effects.SendLocationEffect( blast3, m.Map, 0x3400, 60, 0xB76, 0 );
				Effects.SendLocationEffect( blast4, m.Map, 0x3400, 60, 0xB76, 0 );
				Effects.SendLocationEffect( blast5, m.Map, 0x3400, 60, 0xB76, 0 );
				Effects.PlaySound( m.Location, m.Map, 0x108 );

				int min = 7;
				int max = (int)( GetSythDamage(Caster) / 25 );

				int drain = Utility.RandomMinMax( min, max );

				TimeSpan duration = TimeSpan.FromSeconds( GetSythDamage(Caster) / 5 );
				HarmfulSpell( m );
				Caster.Hits = Caster.Hits + drain;
				AOS.Damage( m, Caster, drain, true, 100, 0, 0, 0, 0 );
				new DrainTimer( Caster, m, duration, min, max ).Start();

				BuffInfo.RemoveBuff( Caster, BuffIcon.DrainLifeGood );
				BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.DrainLifeGood, 1063502, duration, Caster ) );

				BuffInfo.RemoveBuff( m, BuffIcon.DrainLifeBad );
				BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.DrainLifeBad, 1063504, duration, m ) );
			}
		}

		public class InternalTarget : Target
		{
			private DrainLife m_Owner;

			public InternalTarget( DrainLife owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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

		public class DrainTimer : Timer
		{
			private Mobile m_m;
			private Mobile m_Caster;
			private DateTime m_Expire;
			private int m_Time;
			private int m_Min;
			private int m_Max;

			public DrainTimer( Mobile caster, Mobile m, TimeSpan duration, int min, int max ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 0.1 ) )
			{
				m_m = m;
				m_Caster = caster;
				m_Expire = DateTime.Now + duration;
				m_Time = 0;
				m_Min = min;
				m_Max = max;
			}

			protected override void OnTick()
			{
				m_Time++;
				if ( m_Time > 45 )
				{
					m_Time = 0;

					int drain = Utility.RandomMinMax( m_Min, m_Max );

					Point3D blast1 = new Point3D( ( m_m.X ), ( m_m.Y ), m_m.Z );
					Point3D blast2 = new Point3D( ( m_m.X-1 ), ( m_m.Y ), m_m.Z );
					Point3D blast3 = new Point3D( ( m_m.X+1 ), ( m_m.Y ), m_m.Z );
					Point3D blast4 = new Point3D( ( m_m.X ), ( m_m.Y-1 ), m_m.Z );
					Point3D blast5 = new Point3D( ( m_m.X ), ( m_m.Y+1 ), m_m.Z );

					Effects.SendLocationEffect( blast1, m_m.Map, 0x3400, 60, 0xB76, 0 );
					Effects.SendLocationEffect( blast2, m_m.Map, 0x3400, 60, 0xB76, 0 );
					Effects.SendLocationEffect( blast3, m_m.Map, 0x3400, 60, 0xB76, 0 );
					Effects.SendLocationEffect( blast4, m_m.Map, 0x3400, 60, 0xB76, 0 );
					Effects.SendLocationEffect( blast5, m_m.Map, 0x3400, 60, 0xB76, 0 );
					Effects.PlaySound( m_m.Location, m_m.Map, 0x108 );

					m_Caster.DoHarmful( m_m );
					m_Caster.RevealingAction();
					m_Caster.Hits = m_Caster.Hits + drain;
					AOS.Damage( m_m, m_Caster, drain, true, 100, 0, 0, 0, 0 );
				}

				if ( DateTime.Now >= m_Expire || !m_Caster.Alive || m_Caster == null )
				{
					BuffInfo.RemoveBuff( m_Caster, BuffIcon.DrainLifeGood );
					BuffInfo.RemoveBuff( m_m, BuffIcon.DrainLifeBad );
					Stop();
				}
			}
		}
	}
}