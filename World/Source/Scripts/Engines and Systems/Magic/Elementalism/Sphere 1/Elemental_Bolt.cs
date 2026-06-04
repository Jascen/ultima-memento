using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Spells.Elementalism
{
	public class Elemental_Bolt_Spell : ElementalSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Elemental Bolt", "Sulita",
				212,
				9041
			);

		public override SpellCircle Circle { get { return SpellCircle.First; } }

		public Elemental_Bolt_Spell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

        public override bool DelayedDamageStacking { get { return !Core.AOS; } }

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
				Mobile source = Caster;

				SpellHelper.Turn( source, m );

				if ( SpellHelper.ResolveMagicDefense( (int)this.Circle, ref source, ref m ) )
				{
					int nBenefit = (int)(Caster.Skills[CastSkill].Value / 5);

					double damage = GetNewAosDamage( 10, 1, 4, m ) + nBenefit;

					int hit = 0x3818;
					int snd = 0x211;
					int hue = 0;

					string elm = ElementalSpell.GetElement( Caster );

					if ( elm == "air" )
					{
						hit = 0x3818;
						snd = 0x211;
						SpellHelper.Damage( this, m, damage, 50, 0, 0, 0, 50 );
					}
					else if ( elm == "earth" )
					{
						hit = 0x4F49;
						snd = 0x658;
						SpellHelper.Damage( this, m, damage, 50, 0, 0, 50, 0 );
					}
					else if ( elm == "fire" )
					{
						hit = 0x4D17;
						snd = 0x15E;
						SpellHelper.Damage( this, m, damage, 50, 50, 0, 0, 0 );
					}
					else if ( elm == "water" )
					{
						hit = 0x4F49;
						snd = 0x027;
						hue = 0xB3D;
						SpellHelper.Damage( this, m, damage, 50, 0, 50, 0, 0 );
					}

					Effects.SendMovingEffect( source, m, hit, 5, 0, false, false, hue, 0 );
					source.PlaySound( snd );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private Elemental_Bolt_Spell m_Owner;

			public InternalTarget( Elemental_Bolt_Spell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}