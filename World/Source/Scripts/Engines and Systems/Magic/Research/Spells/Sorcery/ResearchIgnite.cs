using System;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Spells.Research
{
	public class ResearchIgnite : ResearchSpell
	{
		public int CirclePower = 4;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 633,
					IconGraphic = 11222,
					Name = "Ignite",
					PowerWords = "In Flam",
					Description = "Calls forth columns of fire that can cause up to 15-125 points of fire and physical damage. It will do twice the amount of damage toward creatures that suffer from watery grave or neptune's bane enchanted weapons.",
					ManaCost = 30,
					MinSkill = 40,
					TargetType = TargetFlags.None
				},
				233,
				9042,
				Reagent.Brimstone,Reagent.SulfurousAsh,Reagent.BlackPearl
			);

		public ResearchIgnite( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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
					if ( damage < 12 ){ damage = 12.0; }

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

						SlayerEntry water = SlayerGroup.GetEntryByName( SlayerName.WaterDissipation );
						SlayerEntry neptune = SlayerGroup.GetEntryByName( SlayerName.NeptunesBane );
						if ( m is BaseCreature )
						{
							if ( water.Slays(m) ){ toDeal = toDeal*2; }
							if ( neptune.Slays(m) ){ toDeal = toDeal*2; }
						}

						SpellHelper.Damage( this, m, toDeal, 50, 50, 0, 0, 0 );

						Point3D blast1 = new Point3D( ( m.X ), ( m.Y ), m.Z );
						Point3D blast2 = new Point3D( ( m.X-1 ), ( m.Y ), m.Z );
						Point3D blast3 = new Point3D( ( m.X+1 ), ( m.Y ), m.Z );
						Point3D blast4 = new Point3D( ( m.X ), ( m.Y-1 ), m.Z );
						Point3D blast5 = new Point3D( ( m.X ), ( m.Y+1 ), m.Z );

						Effects.SendLocationEffect( blast1, m.Map, 0x3728, 85, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB70 ), 0 );
						if ( Utility.RandomBool() ){ Effects.SendLocationEffect( blast2, m.Map, 0x3728, 85, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB70 ), 0 ); }
						if ( Utility.RandomBool() ){ Effects.SendLocationEffect( blast3, m.Map, 0x3728, 85, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB70 ), 0 ); }
						if ( Utility.RandomBool() ){ Effects.SendLocationEffect( blast4, m.Map, 0x3728, 85, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB70 ), 0 ); }
						if ( Utility.RandomBool() ){ Effects.SendLocationEffect( blast5, m.Map, 0x3728, 85, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB70 ), 0 ); }
						Effects.PlaySound( m.Location, m.Map, 0x208 );
					}
					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ResearchIgnite m_Owner;

			public InternalTarget( ResearchIgnite owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
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