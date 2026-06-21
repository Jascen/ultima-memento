using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.Spells.Research
{
	public class ResearchFrostStrike : ResearchSpell
	{
		public override int spellIndex { get { return 40; } }
		public int CirclePower = 7;
		public static int spellID = 40;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2 ); } }
		public override double RequiredSkill{ get{ return (double)(Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 8 ))); } }
		public override int RequiredMana{ get{ return Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 7 )); } }

		private static SpellInfo m_Info = new SpellInfo(
				Server.Misc.Research.SpellInformation( spellID, 2 ),
				Server.Misc.Research.CapsCast( Server.Misc.Research.SpellInformation( spellID, 4 ) ),
				245,
				9042,
				Reagent.MoonCrystal,Reagent.GraveDust
			);

		public ResearchFrostStrike( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Mobile source = Caster;
				bool hitThrough = SpellHelper.ResolveMagicDefense( CirclePower, ref source, ref m );

				m.FixedParticles( 0x5571, 10, 30, 5052, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.LeftFoot );
				m.PlaySound( 0x65D );

				if ( hitThrough )
				{
					double damage = DamagingSkill( Caster )/2;
						if ( damage > 125 ){ damage = 125.0; }
						if ( damage < 35 ){ damage = 35.0; }

					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );

					SpellHelper.Damage( this, m, damage, 0, 0, 100, 0, 0 );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ResearchFrostStrike m_Owner;

			public InternalTarget( ResearchFrostStrike owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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