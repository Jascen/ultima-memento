using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Misc;

namespace Server.Spells.Sixth
{
	public class EnergyBoltSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Energy Bolt", "Corp Por",
				230,
				9022,
				Reagent.BlackPearl,
				Reagent.Nightshade
			);

		public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

		public EnergyBoltSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this, Scroll, (int)Circle );
		}

		public override bool DelayedDamage{ get{ return true; } }

		public void Target( Mobile m, Item scroll, int circle )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				Mobile source = Caster;

				SpellHelper.Turn( Caster, m );

				bool hitThrough = SpellHelper.ResolveMagicDefense( (int)this.OneBasedCircle, ref source, ref m );

				source.MovingParticles( m, 0x3818, 7, 0, false, true, PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, 3043, 4043, 0x211, 0 );
				source.PlaySound( 0x20A );

				if ( hitThrough )
				{
					int nBenefit = 0;
					if ( source is PlayerMobile )
						nBenefit = (int)(source.Skills[SkillName.Magery].Value / 5);

					double damage = GetNewAosDamage( 40, 1, 5, m ) + nBenefit;

					SpellHelper.Damage( this, m, damage, 0, 0, 0, 0, 100 );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private EnergyBoltSpell m_Owner;
			private Item m_Item;
			private int m_Cir;

			public InternalTarget( EnergyBoltSpell owner, Item scroll, int circle ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
				m_Item = scroll;
				m_Cir = circle;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile)o, m_Item, m_Cir );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}