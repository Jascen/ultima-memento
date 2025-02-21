using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Regions;
using Server.Mobiles;

namespace Server.Spells.DeathKnight
{
	public class WrathSpell : DeathKnightSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Wrath", "Zagan Ira",
				233,
				9042,
				false
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1 ); } }
		public override int RequiredTithing{ get{ return 70; } }
		public override double RequiredSkill{ get{ return 50.0; } }
		public override int RequiredMana{ get{ return 44; } }

		public WrathSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return true; } }

		public void Target( IPoint3D p )
		{
			if ( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				SpellHelper.Turn( Caster, p );

				if ( p is Item )
					p = ((Item)p).GetWorldLocation();

				ArrayList targets = new ArrayList();

				Map map = Caster.Map;

				bool playerVsPlayer = false;

				if ( map != null )
				{
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), 5 );

					foreach ( Mobile m in eable )
					{
						Mobile pet = m;

						if ( Caster != m )
						{
							if ( m is BaseCreature )
								pet = ((BaseCreature)m).GetMaster();

							if ( Caster != pet )
							{
								targets.Add( m );

								if ( m.Player )
									playerVsPlayer = true;
							}
						}
					}

					eable.Free();
				}

				double damage;

				int nBenefit = 0;
				if ( Caster is PlayerMobile )
				{
					nBenefit = (int)( GetKarmaPower( Caster ) / 5 );
				}

				damage = GetNewAosDamage( 32, 1, 4, Caster.Player && playerVsPlayer ) + nBenefit;
				DrainSoulsInLantern( Caster, RequiredTithing );

				if ( targets.Count > 0 )
				{
					damage = (damage * 2) / targets.Count;

					for ( int i = 0; i < targets.Count; ++i )
					{
						Mobile m = (Mobile)targets[i];
						Region house = m.Region;

						double toDeal = damage;

						if( !(house is Regions.HouseRegion) )
						{
							Caster.DoHarmful( m );
							SpellHelper.Damage( this, m, toDeal, 0, 0, 0, 0, 100 );

							m.BoltEffect( 0 );
						}
					}
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private WrathSpell m_Owner;

			public InternalTarget( WrathSpell owner ) : base( 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D p = o as IPoint3D;

				if ( p != null )
					m_Owner.Target( p );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}
