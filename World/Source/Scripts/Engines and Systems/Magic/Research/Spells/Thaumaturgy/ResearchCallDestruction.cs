using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Spells.Research
{
	public class ResearchCallDestruction : ResearchSpell
	{
		public override bool alwaysConsume { get{ return bool.Parse( Server.Misc.Research.SpellInformation( spellIndex, 14 )); } }
		public int CirclePower = 5;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 604,
					IconGraphic = 11239,
					Name = "Call Destruction",
					PowerWords = "Kal Vas Grav Por",
					Description = "Damages nearby enemies with an destructive force that causes mostly physical harm and some damage from energy. If you hit more than one enemy, the damage amount is doubled and divided amongst each enemy evenly. The damage dealt is between 8-50 points, with the addition of half the available hit points of the caster. After the destruction is unleashed, the caster will lose half of their remaining hit points.",
					ManaCost = 25,
					MinSkill = 40,
					TargetType = TargetFlags.None
				},
				233,
				9042,
				Reagent.BatWing,Reagent.BlackPearl,Reagent.Brimstone,Reagent.PigIron
			);

		public ResearchCallDestruction( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), 8 );

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

				double damage = DamagingSkill( Caster )/5;
					if ( damage > 50 ){ damage = 50.0; }
					if ( damage < 8 ){ damage = 8.0; }

					damage = damage + (int)(Caster.Hits/2);

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
						SpellHelper.Damage( this, m, toDeal, 75, 0, 0, 0, 25 );

						m.FixedParticles( 0x551A, 10, 30, 5052, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.LeftFoot );
						m.PlaySound( 0x345 );
					}
					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
					Effects.SendLocationEffect( Caster.Location, Caster.Map, 0x36B0, 60, 0xAB3, 0 );
					Caster.Hits = (int)(Caster.Hits/2);
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ResearchCallDestruction m_Owner;

			public InternalTarget( ResearchCallDestruction owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
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