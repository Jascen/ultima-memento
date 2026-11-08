using System;
using Server.Targeting;
using Server.Items;

namespace Server.Spells.Jedi
{
	public class ThrowSabre : JediSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 283,
					IconGraphic = 0x2BCF,
					Name = "Throw Sabre",
					PowerWords = "Tiana",
					Description = "The Jedi can throw their equipped sword at an enemy, doing not only the sword's normal damage but also an extra 17-53 points of damage. The types of resistances that the sword inflicts damage on match that of the sword. The sword quickly returns to the Jedi's hand upon throwing it.",
					ManaCost = 16,
					TithingCost = 12,
					MinSkill = 40,
					TargetType = TargetFlags.Harmful
				},
				203,
				0
			);

		public int CirclePower = 2;
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public ThrowSabre( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if ( GetSword() == null )
			{
				Caster.SendMessage( "You need a sword equipped to throw it." );
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return true; } }

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				if ( GetSword() != null )
				{
					Item sword = GetSword();
					BaseWeapon bw = (BaseWeapon)sword;

					int min = bw.MinDamage + 10;
					int max = (int)(bw.MaxDamage + ( GetJediDamage( Caster ) / 7 ));

					int phys = bw.AosElementDamages.Physical;
					int cold = bw.AosElementDamages.Cold;
					int fire = bw.AosElementDamages.Fire;
					int engy = bw.AosElementDamages.Energy;
					int pois = bw.AosElementDamages.Poison;

					if ( ( phys + cold + fire + engy + pois ) < 1 ){ phys = 100; }

					int damage = Utility.RandomMinMax( min, max );

					Effects.SendMovingEffect( Caster, m, sword.ItemID, 30, 10, false, false, sword.Hue-1, 0 );

					Caster.PlaySound( 0x5D2 );

					//Caster.SendMessage( "" + min + "_" + max + "_" + damage + "_" + phys + "_" + fire + "_" + cold + "_" + pois + "_" + engy + "" );

					// Deal the damage
					AOS.Damage( m, Caster, damage, phys, fire, cold, pois, engy );
				}
			}
		}

		public Item GetSword()
		{
			if ( Caster.FindItemOnLayer( Layer.OneHanded ) != null )
			{
				Item oneHand = Caster.FindItemOnLayer( Layer.OneHanded );
				if ( oneHand is BaseSword ){ return oneHand; }
			}

			if ( Caster.FindItemOnLayer( Layer.TwoHanded ) != null )
			{
				Item twoHand = Caster.FindItemOnLayer( Layer.TwoHanded );
				if ( twoHand is BaseSword ){ return twoHand; }
			}

			return null;
		}

		private class InternalTarget : Target
		{
			private ThrowSabre m_Owner;

			public InternalTarget( ThrowSabre owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}