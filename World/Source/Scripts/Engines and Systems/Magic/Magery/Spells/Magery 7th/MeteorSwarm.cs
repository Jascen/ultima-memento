using System;
using System.Collections.Generic;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using Server.Misc;

namespace Server.Spells.Seventh
{
	public class MeteorSwarmSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Meteor Swarm", "Flam Kal Des Ylem",
				233,
				9042,
				false,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

		public MeteorSwarmSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

				List<Mobile> targets = new List<Mobile>();

				Map map = Caster.Map;

				bool playerVsPlayer = false;

				if ( map != null )
				{
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), 5 );

					foreach ( Mobile m in eable )
					{
						Mobile pet = m;
						if ( m is BaseCreature )
							pet = ((BaseCreature)m).GetMaster();

						if ( Caster.Region == m.Region && Caster != m && Caster != pet && Caster.InLOS( m ) && m.Blessed == false && Caster.CanBeHarmful( m, true ) )
						{
							targets.Add( m );

							if ( m.Player )
								playerVsPlayer = true;
						}
					}

					eable.Free();
				}

				double damage;

				int nBenefit = 0;
				if ( Caster is PlayerMobile )
				{
					nBenefit = (int)(Caster.Skills[SkillName.Magery].Value / 5);
				}

				if ( Core.AOS )
					damage = GetNewAosDamage( 51, 1, 5, playerVsPlayer ) + nBenefit;
				else
					damage = Utility.Random( 27, 22 ) + nBenefit;

				if ( targets.Count > 0 )
				{
					Effects.PlaySound( p, Caster.Map, 0x160 );

					if (targets.Count > 1)
						damage /= 2;
						
					for ( int i = 0; i < targets.Count; ++i )
					{
						Mobile m = targets[i];

						double toDeal = damage;
						toDeal *= GetDamageScalar( m );
						Caster.DoHarmful( m );
						SpellHelper.Damage( this, m, toDeal, 0, 100, 0, 0, 0 );

						Point3D blast1 = new Point3D( ( m.X ), ( m.Y ), m.Z );
						Point3D blast2 = new Point3D( ( m.X-1 ), ( m.Y ), m.Z );
						Point3D blast3 = new Point3D( ( m.X+1 ), ( m.Y ), m.Z );
						Point3D blast4 = new Point3D( ( m.X ), ( m.Y-1 ), m.Z );
						Point3D blast5 = new Point3D( ( m.X ), ( m.Y+1 ), m.Z );

						Effects.SendLocationEffect( blast1, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0 );
						Effects.SendLocationEffect( blast2, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0 );
						Effects.SendLocationEffect( blast3, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0 );
						Effects.SendLocationEffect( blast4, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0 );
						Effects.SendLocationEffect( blast5, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0 );
						Effects.PlaySound( m.Location, m.Map, 0x15F );
					}
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private MeteorSwarmSpell m_Owner;

			public InternalTarget( MeteorSwarmSpell owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
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