using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Spells.Research
{
	public class ResearchMassDeath : ResearchSpell
	{
		public override bool alwaysConsume { get{ return bool.Parse( Server.Misc.Research.SpellInformation( spellIndex, 14 )); } }
		public int CirclePower = 8;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 637,
					IconGraphic = 11258,
					Name = "Mass Death",
					PowerWords = "Vas Corp",
					Description = "Damages nearby enemies with deathly magics summoned from the void. If you hit more than one enemy, the damage amount is doubled and divided amongst each enemy evenly. The damage dealt is between 45-125 points, with the addition of the available hit points of the caster. After the mass death is unleashed, the caster will have a single hit point remaining. The caster may lose some karma if they attempt this. This spell is powerful enough that the scroll will always crumble to dust when cast.",
					ManaCost = 55,
					MinSkill = 90,
					TargetType = TargetFlags.None
				},
				233,
				9042,
				Reagent.PixieSkull,Reagent.BatWing,Reagent.DragonTooth
			);

		public ResearchMassDeath( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			Caster.SendMessage( "Choose a focal point for this spell." );
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return true; } }

		public void Target( IPoint3D p )
		{
			if ( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( Server.Misc.Worlds.NoApocalypse( Caster.Location, Caster.Map ) )
			{
				Caster.SendMessage( "You don't think it is wise to cast this here." ); 
				return;
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
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), 10 );

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
					if ( damage < 45 ){ damage = 45.0; }

					damage = damage + Caster.Hits;

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
						SpellHelper.Damage( this, m, toDeal, 20, 20, 20, 20, 20 );

						Point3D blast1 = new Point3D( ( m.X ), ( m.Y ), m.Z );
						Point3D blast2 = new Point3D( ( m.X-1 ), ( m.Y ), m.Z );
						Point3D blast3 = new Point3D( ( m.X+1 ), ( m.Y ), m.Z );
						Point3D blast4 = new Point3D( ( m.X ), ( m.Y-1 ), m.Z );
						Point3D blast5 = new Point3D( ( m.X ), ( m.Y+1 ), m.Z );

						Effects.SendLocationEffect( blast1, m.Map, 0x5475, 60, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB02 ), 0 );
						Effects.SendLocationEffect( blast2, m.Map, 0x5475, 60, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB02 ), 0 );
						Effects.SendLocationEffect( blast3, m.Map, 0x5475, 60, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB02 ), 0 );
						Effects.SendLocationEffect( blast4, m.Map, 0x5475, 60, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB02 ), 0 );
						Effects.SendLocationEffect( blast5, m.Map, 0x5475, 60, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB02 ), 0 );
						Effects.PlaySound( m.Location, m.Map, 0x108 );
					}
					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
					Effects.SendLocationEffect( Caster.Location, Caster.Map, 0x5475, 60, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB02 ), 0 );
					KarmaMod( Caster, ((int)RequiredSkill+RequiredMana) );
					Caster.Hits = 1;
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ResearchMassDeath m_Owner;

			public InternalTarget( ResearchMassDeath owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
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