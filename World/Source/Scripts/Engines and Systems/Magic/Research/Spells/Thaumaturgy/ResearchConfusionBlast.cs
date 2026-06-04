using System;
using Server; 
using Server.Targeting;
using Server.Network;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Misc;

namespace Server.Spells.Research
{
	public class ResearchConfusionBlast : ResearchSpell
	{
		public override int spellIndex { get { return 6; } }
		public int CirclePower = 4;
		public static int spellID = 6;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }
		public override double RequiredSkill{ get{ return (double)(Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 8 ))); } }
		public override int RequiredMana{ get{ return Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 7 )); } }

		private static SpellInfo m_Info = new SpellInfo(
				Server.Misc.Research.SpellInformation( spellID, 2 ),
				Server.Misc.Research.CapsCast( Server.Misc.Research.SpellInformation( spellID, 4 ) ),
				212,
				9001,
				Reagent.MandrakeRoot,Reagent.PixieSkull,Reagent.FairyEgg
			);

		public ResearchConfusionBlast( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
			else if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				SpellHelper.Turn( Caster, p );

				if ( p is Item )
					p = ((Item)p).GetWorldLocation();

				List<Mobile> targets = new List<Mobile>();

				Map map = Caster.Map;
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );

				if ( map != null )
				{
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), 5 );

					foreach ( Mobile m in eable )
					{
						Mobile pet = m;
						if ( m is BaseCreature )
							pet = ((BaseCreature)m).GetMaster();

						if ( m.Alive && Caster != m && Caster != pet && Caster.InLOS( m ) && m.Blessed == false && Caster.CanBeHarmful( m, true ) )
						{
							targets.Add( m );
						}
					}

					eable.Free();
				}

				if ( targets.Count > 0 )
				{						
					for ( int i = 0; i < targets.Count; ++i )
					{
						Mobile m = targets[i];

						SpellHelper.Turn( Caster, m );

						if ( SpellHelper.ResolveMagicAbsorption( CirclePower, m ) )
						{
							double duration = DamagingSkill( Caster )/12;
							if ( duration > 20 ){ duration = 20.0; }
							if ( duration < 5 ){ duration = 5.0; }

							if ( m is BaseCreature )
							{
								BaseCreature mon = (BaseCreature)m;
								mon.Pacify( Caster, DateTime.Now + TimeSpan.FromSeconds( duration ) );
							}
							else if ( m != Caster )
							{
								m.Paralyze( TimeSpan.FromSeconds( duration ) );
								Timer.DelayCall( TimeSpan.FromSeconds( duration ), new TimerStateCallback( Recover_Callback ), m );
								BuffInfo.RemoveBuff( m, BuffIcon.Confusion );
								BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.Confusion, 1063684, TimeSpan.FromSeconds( duration ), m ) );
							}

							Effects.SendLocationEffect( m.Location, m.Map, 0x3039, 85, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0x9B5 ), 0 );
							Effects.PlaySound( m.Location, m.Map, 0x5C9 );
						}
					}
				}
			}

			FinishSequence();
		}

		private void Recover_Callback( object state )
		{
			Mobile defender = state as Mobile;

			if ( defender != null )
			{
				defender.Paralyzed = false;
				defender.Combatant = null;
				BuffInfo.RemoveBuff( defender, BuffIcon.Confusion );
				defender.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, "You recover your senses." );
			}
		}

		private class InternalTarget : Target
		{
			private ResearchConfusionBlast m_Owner;

			public InternalTarget( ResearchConfusionBlast owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
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