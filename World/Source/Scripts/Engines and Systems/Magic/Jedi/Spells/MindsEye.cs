using System;
using System.Collections;
using System.Collections.Generic;
using Server.Targeting;
using Server.Items;
using Server.Spells.Sixth;

namespace Server.Spells.Jedi
{
    public class MindsEye : JediSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 281,
					IconGraphic = 0x2BEB,
					Name = "Mind's Eye",
					PowerWords = "Vincent",
					Description = "Allows the Jedi to concentrate and see that which cannot be seen. They may be able to notice something hidden or a concealed trap.",
					ManaCost = 8,
					TithingCost = 16,
					MinSkill = 20,
					TargetType = TargetFlags.Beneficial
				},
				206,
				0
			);

		public int CirclePower = 1;
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

        public MindsEye(Mobile caster, Item scroll) : base(caster, scroll, SpellInfo)
        {
        }

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( IPoint3D p )
		{
			bool foundAnyone = false;

			if ( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckSequence() )
			{
				ArrayList ItemsToDelete = new ArrayList();
				IPooledEnumerable TitemsInRange = Caster.Map.GetItemsInRange( new Point3D( p ), 1 + (int)(Caster.Skills[SkillName.Psychology].Value / 20.0) );
				string sTrap;
				foreach ( Item item in TitemsInRange )
				{
					if ( item is BaseTrap )
					{
						BaseTrap trap = (BaseTrap) item;

						if ( trap is FireColumnTrap ){ sTrap = "(fire column trap)"; }
						else if ( trap is FlameSpurtTrap ){ sTrap = "(fire spurt trap)"; }
						else if ( trap is GasTrap ){ sTrap = "(poison gas trap)"; }
						else if ( trap is GiantSpikeTrap ){ sTrap = "(giant spike trap)"; }
						else if ( trap is MushroomTrap ){ sTrap = "(mushroom trap)"; }
						else if ( trap is SawTrap ){ sTrap = "(saw blade trap)"; }
						else if ( trap is SpikeTrap ){ sTrap = "(spike trap)"; }
						else if ( trap is StoneFaceTrap ){ sTrap = "(stone face trap)"; }
						else { sTrap = ""; }

						Effects.SendLocationParticles( EffectItem.Create( item.Location, item.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 0xB41, 0, 5024, 0 );
						Effects.PlaySound( item.Location, item.Map, 0x1FA );
						Caster.SendMessage( "There is a trap nearby! " + sTrap + "" );
						foundAnyone = true;
					}
					else if ( item is BaseDoor && (	item.ItemID == 0x35E || 
													item.ItemID == 0xF0 || 
													item.ItemID == 0xF2 || 
													item.ItemID == 0x326 || 
													item.ItemID == 0x324 || 
													item.ItemID == 0x32E || 
													item.ItemID == 0x32C || 
													item.ItemID == 0x314 || 
													item.ItemID == 0x316 || 
													item.ItemID == 0x31C || 
													item.ItemID == 0x31E || 
													item.ItemID == 0xE8 || 
													item.ItemID == 0xEA || 
													item.ItemID == 0x34C || 
													item.ItemID == 0x356 || 
													item.ItemID == 0x35C || 
													item.ItemID == 0x354 || 
													item.ItemID == 0x344 || 
													item.ItemID == 0x346 || 
													item.ItemID == 0x34E || 
													item.ItemID == 0x334 || 
													item.ItemID == 0x336 || 
													item.ItemID == 0x33C || 
													item.ItemID == 0x33E ) )
					{
						Effects.SendLocationParticles( EffectItem.Create( item.Location, item.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 0xB41, 0, 5024, 0 );
						Effects.PlaySound( item.Location, item.Map, 0x1FA );
						Caster.SendMessage( "There is a hidden door nearby!" );
						foundAnyone = true;
					}
					else if ( item is HiddenTrap )
					{
						if ( item.Weight <= 2.0 && HiddenTrap.SeeIfTrapActive( item ) )
						{
							Effects.SendLocationParticles( EffectItem.Create( item.Location, item.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 0xB41, 0, 5024, 0 );
							Effects.PlaySound( item.Location, item.Map, 0x1FA );
							Caster.SendMessage( "There is a hidden floor trap somewhere nearby!" );
							foundAnyone = true;
							HiddenTrap.DiscoverTrap( item );
						}
					}
					else if ( item is HiddenChest )
					{
						int level = (int)(GetJediDamage( Caster ) / 50);
							if (level < 1){level = 1;}
							if (level > 6){level = 6;}

						if ( HiddenChest.FoundBox( Caster, true, level, item ) )
							foundAnyone = true;

						ItemsToDelete.Add( item );
					}
				}
				TitemsInRange.Free(); /////////////////////////////////////////////////////////////////////////////

				for ( int i = 0; i < ItemsToDelete.Count; ++i )
				{
					Item rid = ( Item )ItemsToDelete[ i ];
					rid.Delete();
				}

				SpellHelper.Turn( Caster, p );

				SpellHelper.GetSurfaceTop( ref p );

				List<Mobile> targets = new List<Mobile>();

				Map map = Caster.Map;

				if ( map != null )
				{
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), 1 + (int)(GetJediDamage( Caster ) / 20.0) );

					foreach ( Mobile m in eable )
					{
						if ( m.Hidden && (m.AccessLevel == AccessLevel.Player || Caster.AccessLevel > m.AccessLevel) && CheckDifficulty( Caster, m ) )
							targets.Add( m );
					}

					eable.Free();
				}

				for ( int i = 0; i < targets.Count; ++i )
				{
					Mobile m = targets[i];

					m.RevealingAction();

					m.FixedParticles( 0x375A, 9, 20, 5049, 0xB41, 0, EffectLayer.Head );
					m.PlaySound( 0x1FD );
					foundAnyone = true;
				}
				
				if ( !foundAnyone )
				{
					Caster.PlaySound( 0x1D6 );
					Caster.SendMessage( "Your don't notice anything." );
				}
			}

		}

		// Reveal uses magery and searching vs. hide and stealth 
		private static bool CheckDifficulty( Mobile from, Mobile m )
		{
			if ( InvisibilitySpell.HasTimer( m ) )
				return true;

			int magery = (int)(GetJediDamage( from ));
			int searching = (int)(from.Skills[SkillName.Psychology].Fixed);

			int hiding = m.Skills[SkillName.Hiding].Fixed;
			int stealth = m.Skills[SkillName.Stealth].Fixed;
			int divisor = hiding + stealth;

			int chance;
			if ( divisor > 0 )
				chance = 50 * (magery + searching) / divisor;
			else
				chance = 100;

			return chance > Utility.Random( 100 );
		}

		private class InternalTarget : Target
		{
			private MindsEye m_Owner;

			public InternalTarget( MindsEye owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
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