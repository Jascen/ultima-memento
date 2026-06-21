using System;
using Server.Targeting;
using Server.Network;
using Server;
using Server.Items;

namespace Server.Spells.Research
{
	public class ResearchFlameBolt : ResearchSpell
	{
		public override int spellIndex { get { return 36; } }
		public int CirclePower = 1;
		public static int spellID = 38;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.50 ); } }
		public override double RequiredSkill{ get{ return (double)(Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 8 ))); } }
		public override int RequiredMana{ get{ return Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 7 )); } }

		private static SpellInfo m_Info = new SpellInfo(
				Server.Misc.Research.SpellInformation( spellID, 2 ),
				Server.Misc.Research.CapsCast( Server.Misc.Research.SpellInformation( spellID, 4 ) ),
				236,
				9031,
				Reagent.Brimstone,Reagent.SulfurousAsh
			);

		public ResearchFlameBolt( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

				bool hitThrough = SpellHelper.ResolveMagicDefense( CirclePower, ref source, ref m );

				m.FixedParticles(0x5562, 10, 30, 5052, Server.Misc.PlayerSettings.GetMySpellHue(true, Caster, 0), 0, EffectLayer.Head);
				m.PlaySound(0x44B);

				if ( hitThrough )
				{
					double damage = DamagingSkill( Caster )/2.5;
						if ( damage > 100 ){ damage = 100.0; }
						if ( damage < 12 ){ damage = 12.0; }

					SpellHelper.Damage( this, m, damage, 0, 100, 0, 0, 0 );
					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ResearchFlameBolt m_Owner;

			public InternalTarget( ResearchFlameBolt owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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