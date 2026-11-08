using System;
using System.Collections;
using System.Collections.Generic;
using Server.Targeting;
using Server.Items;
using Server.Spells.Sixth;

namespace Server.Spells.Shinobi
{
	public class EagleEye : ShinobiSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 292,
					IconGraphic = 10872,
					Name = "Eagle Eye",
					PowerWords = "Surudoi Me",
					Description = "The eyes of the ninja are focused where they can perhaps spot hidden creatures, traps, or treasure.",
					ManaCost = 50,
					TithingCost = 55,
					MinSkill = 70,
					TargetType = TargetFlags.Beneficial
				},
				-1,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public EagleEye( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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
				IPooledEnumerable TitemsInRange = Caster.Map.GetItemsInRange( new Point3D( p ), 1 + (int)(Caster.Skills[SkillName.Ninjitsu].Value / 20.0) );
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

						Effects.SendLocationParticles( EffectItem.Create( item.Location, item.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 0, 0, 5024, 0 );
						Caster.PlaySound( Caster.Female ? 779 : 1050 );
						Caster.SendMessage( "There is a trap nearby! " + sTrap + "" );
						Caster.Say( "*ah ha!*" );
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
						Effects.SendLocationParticles( EffectItem.Create( item.Location, item.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 0, 0, 5024, 0 );
						Caster.PlaySound( Caster.Female ? 779 : 1050 );
						Caster.SendMessage( "There is a hidden door nearby!" );
						Caster.Say( "*ah ha!*" );
						foundAnyone = true;
					}
					else if ( item is HiddenTrap )
					{
						if ( item.Weight <= 2.0 && HiddenTrap.SeeIfTrapActive( item ) )
						{
							Effects.SendLocationParticles( EffectItem.Create( item.Location, item.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 0, 0, 5024, 0 );
							Caster.PlaySound( Caster.Female ? 779 : 1050 );
							Caster.SendMessage( "There is a hidden floor trap somewhere nearby!" );
							Caster.Say( "*ah ha!*" );
							foundAnyone = true;
							HiddenTrap.DiscoverTrap( item );
						}
					}
					else if ( item is HiddenChest )
					{
						int level = (int)(Caster.Skills[SkillName.Ninjitsu].Value / 16);
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
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), 1 + (int)(Caster.Skills[SkillName.Ninjitsu].Value / 20.0) );

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

					m.FixedParticles( 0x375A, 9, 20, 5049, 0, 0, EffectLayer.Head );
					Caster.PlaySound( Caster.Female ? 779 : 1050 );
					Caster.Say( "*ah ha!*" );
					foundAnyone = true;
				}

				if ( !foundAnyone )
				{
					Caster.PlaySound( Caster.Female ? 0x31B : 0x42B );
					Caster.Say( "*groans*" );
					Caster.SendMessage( "Your don't notice anything." );
				}
			}

			FinishSequence();
		}

		// Eagle Eye uses ninjitsu and searching vs. hide and stealth 
		private static bool CheckDifficulty( Mobile from, Mobile m )
		{
			if ( InvisibilitySpell.HasTimer( m ) )
				return true;

			int ninja = from.Skills[SkillName.Ninjitsu].Fixed;
			int searching = from.Skills[SkillName.Searching].Fixed;

			int hiding = m.Skills[SkillName.Hiding].Fixed;
			int stealth = m.Skills[SkillName.Stealth].Fixed;
			int divisor = hiding + stealth;

			int chance;
			if ( divisor > 0 )
				chance = 50 * (ninja + searching) / divisor;
			else
				chance = 100;

			return chance > Utility.Random( 100 );
		}

		private class InternalTarget : Target
		{
			private EagleEye m_Owner;

			public InternalTarget( EagleEye owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
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