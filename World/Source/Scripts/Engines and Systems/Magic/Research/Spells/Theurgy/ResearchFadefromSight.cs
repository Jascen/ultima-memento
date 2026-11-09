using System;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchFadefromSight : ResearchSpell
	{
		public int CirclePower = 3;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 625,
					IconGraphic = 11246,
					Name = "Fade from Sight",
					PowerWords = "Quas An Lor",
					Description = "This spell will cause the caster to fade from the view of others, while at the same time moving them to a different location.",
					ManaCost = 15,
					MinSkill = 50,
					TargetType = TargetFlags.None
				},
				215,
				9031,
				Reagent.RedLotus,Reagent.NoxCrystal
			);

		public ResearchFadefromSight( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( IPoint3D p )
		{
			IPoint3D orig = p;
			Map map = Caster.Map;

			SpellHelper.GetSurfaceTop( ref p );
			
			PlayerMobile pm = Caster as PlayerMobile; // IsStealthing should be moved to Server.Mobiles

			if ( Server.Misc.WeightOverloading.IsOverloaded( Caster ) )
			{
				Caster.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
			}
			else if ( !SpellHelper.CheckTravel( Caster, TravelCheckType.TeleportFrom ) )
			{
			}
			else if ( !SpellHelper.CheckTravel( Caster, map, new Point3D( p ), TravelCheckType.TeleportTo ) )
			{
			}
			else if ( map == null || !map.CanSpawnMobile( p.X, p.Y, p.Z ) )
			{
				Caster.SendLocalizedMessage( 502831 ); // Cannot teleport to that spot.
			}
			else if ( SpellHelper.CheckMulti( new Point3D( p ), map, true, 5 ) )
			{
				Caster.SendLocalizedMessage( 502831 ); // Cannot teleport to that spot.
			}
			else if ( CheckSequence() )
			{
				SpellHelper.Turn( Caster, orig );

				Mobile m = Caster;

				Point3D from = m.Location;
				Point3D to = new Point3D( p );

				Effects.SendLocationParticles( EffectItem.Create( Caster.Location, Caster.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 0, 0, 5042, 0 );
				Effects.PlaySound( Caster, Caster.Map, 0x201 );

				m.Hidden = true;
				m.Location = to;
				m.ProcessDelta();
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
			}

			FinishSequence();
		}
		public class InternalTarget : Target
		{
			private ResearchFadefromSight m_Owner;

			public InternalTarget( ResearchFadefromSight owner ) : base( 11, true, TargetFlags.None )
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
