using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Misc;

namespace Server.Spells.Fourth
{
	public class LightningSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Lightning", "Por Ort Grav",
				239,
				9021,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

		public LightningSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this, Scroll, (int)Circle );
		}

		public override bool DelayedDamage{ get{ return false; } }

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
				bool hitThrough = SpellHelper.ResolveMagicDefense( (int)this.OneBasedCircle, ref source, ref m );

				if ( Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ) > 0 )
				{
					Point3D blast = new Point3D( ( m.X ), ( m.Y ), m.Z+10 );
					Effects.SendLocationEffect( blast, m.Map, 0x2A4E, 30, 10, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0 );
					m.PlaySound( 0x029 );
				}
				else
				{
					m.BoltEffect( 0 );
				}

				if ( hitThrough )
				{
					int nBenefit = 0;
					if ( Caster is PlayerMobile )
						nBenefit = (int)(Caster.Skills[SkillName.Magery].Value / 5);

					double damage = GetNewAosDamage( 23, 1, 4, m ) + nBenefit;

					SpellHelper.Damage( this, m, damage, 0, 0, 0, 0, 100 );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private LightningSpell m_Owner;
			private Item m_Item;
			private int m_Cir;

			public InternalTarget( LightningSpell owner, Item scroll, int circle ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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