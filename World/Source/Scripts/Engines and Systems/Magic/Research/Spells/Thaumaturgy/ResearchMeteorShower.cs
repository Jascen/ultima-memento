using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Spells.Research
{
	public class ResearchMeteorShower : ResearchSpell
	{
		public int CirclePower = 7;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 640,
					IconGraphic = 11240,
					Name = "Meteor Shower",
					PowerWords = "Kal Des Flam Ylem",
					Description = "Damages nearby enemies with falling meteorites that hit for physical damage (with some fire and energy damage) split between the enemies. If you only hit one enemy, the enemy takes between 35-125 points of damage, but if you hit more than one enemy, the damage amount is doubled and divided amongst each enemy evenly.",
					ManaCost = 40,
					MinSkill = 70,
					TargetType = TargetFlags.None
				},
				233,
				9042,
				Reagent.Bloodmoss,Reagent.Brimstone,Reagent.MandrakeRoot,Reagent.BlackPearl
			);

		public ResearchMeteorShower( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			Caster.SendMessage( "Where do you want to call down the meteors?" );
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
						}
					}

					eable.Free();
				}

				double damage = DamagingSkill( Caster )/2;
					if ( damage > 125 ){ damage = 125.0; }
					if ( damage < 35 ){ damage = 35.0; }

				if ( targets.Count > 0 )
				{
					if ( targets.Count > 1 )
						damage /= 2;
						
					double toDeal;
					for ( int i = 0; i < targets.Count; ++i )
					{
						Mobile m = targets[i];
						toDeal  = damage;
						Caster.DoHarmful( m );
						SpellHelper.Damage( this, m, toDeal, 50, 25, 0, 0, 25 );

						Point3D blast1 = new Point3D( ( m.X ), ( m.Y ), m.Z );
						Point3D blast2 = new Point3D( ( m.X-1 ), ( m.Y ), m.Z );
						Point3D blast3 = new Point3D( ( m.X+1 ), ( m.Y ), m.Z );
						Point3D blast4 = new Point3D( ( m.X ), ( m.Y-1 ), m.Z );
						Point3D blast5 = new Point3D( ( m.X ), ( m.Y+1 ), m.Z );

						Effects.SendLocationEffect( blast1, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB38 ), 0 );
						Effects.SendLocationEffect( blast2, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB38 ), 0 );
						Effects.SendLocationEffect( blast3, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB38 ), 0 );
						Effects.SendLocationEffect( blast4, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB38 ), 0 );
						Effects.SendLocationEffect( blast5, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB38 ), 0 );
						Effects.PlaySound( m.Location, m.Map, 0x65A );
					}
					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ResearchMeteorShower m_Owner;

			public InternalTarget( ResearchMeteorShower owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
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