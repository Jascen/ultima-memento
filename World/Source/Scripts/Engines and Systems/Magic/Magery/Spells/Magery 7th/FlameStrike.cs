using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Misc;

namespace Server.Spells.Seventh
{
	public class FlameStrikeSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Flame Strike", "Kal Vas Flam",
				245,
				9042,
				Reagent.SpidersSilk,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

		public FlameStrikeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
				SpellHelper.Turn( Caster, m );

				Mobile source = Caster;
				if ( SpellHelper.ResolveMagicDefense( (int)this.Circle, ref source, ref m ) )
				{
					double damage;

					int nBenefit = 0;
					if ( Caster is PlayerMobile )
						nBenefit = (int)(Caster.Skills[SkillName.Magery].Value / 5);

					damage = GetNewAosDamage( 48, 1, 5, m ) + nBenefit;

					m.FixedParticles( 0x3709, 10, 30, 5052, PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.LeftFoot );
					m.PlaySound( 0x208 );

					SpellHelper.Damage( this, m, damage, 0, 100, 0, 0, 0 );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private FlameStrikeSpell m_Owner;
			private Item m_Item;
			private int m_Cir;

			public InternalTarget( FlameStrikeSpell owner, Item scroll, int circle ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
				m_Item = scroll;
				m_Cir = circle;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
				{
					m_Owner.Target( (Mobile)o, m_Item, m_Cir );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}