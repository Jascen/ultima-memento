using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Spells;

namespace Server.Spells.HolyMan
{
	public class SmiteSpell : HolyManSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Smite", "Percutiat",
				266,
				9040
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3 ); } }
		public override int RequiredTithing{ get{ return 80; } }
		public override double RequiredSkill{ get{ return 40.0; } }
		public override int RequiredMana{ get{ return 20; } }

		public SmiteSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			SlayerEntry holyundead = SlayerGroup.GetEntryByName( SlayerName.Silver );
			SlayerEntry holydemons = SlayerGroup.GetEntryByName( SlayerName.Exorcism );

			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Mobile source = Caster;
				if ( SpellHelper.ResolveMagicDefense( (int)SpellCircle.Fourth, ref source, ref m ) )
				{
					double damage;

					int nBenefit = (int)( (Caster.Skills[SkillName.Healing].Value / 10) + (Caster.Skills[SkillName.Spiritualism].Value / 10) );

					if ( holyundead.Slays(m) || holydemons.Slays(m) )
						nBenefit = nBenefit * 2;

					damage = GetNewAosDamage( 23, 1, 4, m ) + nBenefit;

					m.BoltEffect( 0 );

					SpellHelper.Damage( this, m, damage, 0, 0, 0, 0, 100 );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private SmiteSpell m_Owner;

			public InternalTarget( SmiteSpell owner ) : base( 12, false, TargetFlags.Harmful )
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
