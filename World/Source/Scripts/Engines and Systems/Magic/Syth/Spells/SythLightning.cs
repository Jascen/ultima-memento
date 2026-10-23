using System;
using System.Collections;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Spells.Syth
{
	public class SythLightning : SythSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 275,
					IconGraphic = 0x03F2,
					Name = "Syth Lightning",
					PowerWords = "Sutta Wo",
					Description = "The power of Syth lightning can be massive, and the stronger the Syth, the more enemies that will be struck by the power of this lightning. The damage dealt will be around 12-75 points of energy damage for each that are struck.",
					ManaCost = 24,
					TithingCost = 32,
					MinSkill = 60,
					TargetType = TargetFlags.Harmful
				},
				203,
				0
			);

		public int CirclePower = 4;
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public SythLightning( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( IPoint3D p )
		{
			if ( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				if ( p is Item )
					p = ((Item)p).GetWorldLocation();

				ArrayList targets = new ArrayList();

				Map map = Caster.Map;

				if ( map != null )
				{
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), 5 );

					foreach ( Mobile m in eable )
					{
						Mobile pet = m;

						if ( Caster.Region == m.Region && Caster != m )
						{
							if ( m is BaseCreature )
								pet = ((BaseCreature)m).GetMaster();

							if ( Caster != pet )
							{
								targets.Add( m );
							}
						}
					}

					eable.Free();
				}

				int min = 20;
				int max = (int)(GetSythDamage( Caster ) / 5);

				int damage = Utility.RandomMinMax( min, max );

				int foes = (int)(GetSythDamage( Caster ) / 50);
					if ( foes < 1 ){ foes = 1; }

				if ( targets.Count > 0 )
				{
					if ( targets.Count == 1 ){ damage = (int)( damage * 1.0 ); }
					else if ( targets.Count == 2 ){ damage = (int)( damage * 0.90 ); }
					else if ( targets.Count == 3 ){ damage = (int)( damage * 0.80 ); }
					else if ( targets.Count == 4 ){ damage = (int)( damage * 0.70 ); }
					else if ( targets.Count == 5 ){ damage = (int)( damage * 0.60 ); }
					else if ( targets.Count == 6 ){ damage = (int)( damage * 0.50 ); }
					else { damage = (int)( damage * 0.40 ); }

					for ( int i = 0; i < targets.Count; ++i )
					{
						if ( foes > 0 )
						{
							foes--;

							Mobile m = (Mobile)targets[i];

							Region house = m.Region;

							if( !(house is Regions.HouseRegion) )
							{
								Caster.DoHarmful( m );
								AOS.Damage( m, Caster, damage, 0, 0, 0, 0, 100 );

								Point3D blast = new Point3D( ( m.X ), ( m.Y ), m.Z+10 );
								Effects.SendLocationEffect( blast, m.Map, 0x2A4E, 30, 10, 0xB00, 0 );
								m.PlaySound( 0x029 );
							}
						}
					}
				}
			}
		}

		private class InternalTarget : Target
		{
			private SythLightning m_Owner;

			public InternalTarget( SythLightning owner ) : base( 12, true, TargetFlags.None )
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