using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Spells.Research
{
	public class ResearchExplosion : ResearchSpell
	{
		public int CirclePower = 6;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 623,
					IconGraphic = 11225,
					Name = "Explosion",
					PowerWords = "Vas Ort Flam",
					Description = "Damages nearby enemies with an explosive force consisting of mostly heat and some physical damage. If you only hit one enemy, the enemy takes between 35-125 points of damage, but if you hit more than one enemy, the damage amount is doubled and divided amongst each enemy evenly.",
					ManaCost = 30,
					MinSkill = 60,
					TargetType = TargetFlags.None
				},
				230,
				9041,
				Reagent.FairyEgg,Reagent.Brimstone,Reagent.SulfurousAsh
			);

		public ResearchExplosion( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );

				if ( p is Item )
					p = ((Item)p).GetWorldLocation();

				List<Mobile> targets = new List<Mobile>();

				Map map = Caster.Map;

				if ( map != null )
				{
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), 5 );

					foreach ( Mobile m in eable )
					{
						Mobile pet = m;
						if ( m is BaseCreature )
							pet = ((BaseCreature)m).GetMaster();

						if ( Caster != m && Caster != pet && Caster.InLOS( m ) && m.Blessed == false && Caster.CanBeHarmful( m, true ) )
						{
							targets.Add( m );
						}
					}

					eable.Free();
				}

				double damage = DamagingSkill( Caster )/2;
					if ( damage > 125 ){ damage = 125.0; }
					if ( damage < 35 ){ damage = 35.0; }

				if ( targets.Count > 0 )
				{
					Effects.PlaySound( p, Caster.Map, 0x65A );

					if ( targets.Count > 1 )
						damage /= 2;
						
					double toDeal;
					for ( int i = 0; i < targets.Count; ++i )
					{
						Mobile m = targets[i];

						toDeal  = damage;
						Caster.DoHarmful( m );
						SpellHelper.Damage( this, m, toDeal, 25, 75, 0, 0, 0 );

						Point3D blast1 = new Point3D( ( m.X ), ( m.Y ), m.Z );
						Point3D blast2 = new Point3D( ( m.X-1 ), ( m.Y ), m.Z );
						Point3D blast3 = new Point3D( ( m.X+1 ), ( m.Y ), m.Z );
						Point3D blast4 = new Point3D( ( m.X ), ( m.Y-1 ), m.Z );
						Point3D blast5 = new Point3D( ( m.X ), ( m.Y+1 ), m.Z );

						Effects.SendLocationEffect( blast1, m.Map, Utility.RandomList( 0x36BD, 0x3822 ), 85, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0 );
						if ( Utility.RandomBool() ){ Effects.SendLocationEffect( blast2, m.Map, Utility.RandomList( 0x36BD, 0x3822 ), 85, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0 ); }
						if ( Utility.RandomBool() ){ Effects.SendLocationEffect( blast3, m.Map, Utility.RandomList( 0x36BD, 0x3822 ), 85, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0 ); }
						if ( Utility.RandomBool() ){ Effects.SendLocationEffect( blast4, m.Map, Utility.RandomList( 0x36BD, 0x3822 ), 85, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0 ); }
						if ( Utility.RandomBool() ){ Effects.SendLocationEffect( blast5, m.Map, Utility.RandomList( 0x36BD, 0x3822 ), 85, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0 ); }
						Effects.PlaySound( m.Location, m.Map, 0x307 );
					}
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ResearchExplosion m_Owner;

			public InternalTarget( ResearchExplosion owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
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