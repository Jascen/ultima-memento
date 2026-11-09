using System;
using Server.Targeting;
using Server.Items;

namespace Server.Spells.Research
{
	public class ResearchAirWalk : ResearchSpell
	{
		public int CirclePower = 7;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 601,
					IconGraphic = 11249,
					Name = "Air Walk",
					PowerWords = "Vas Hur Por",
					Description = "This spell creates an apple high cushion of air under the caster's feet for 20-80 minutes. When moving on this cushion of air, the caster can avoid almost any floor trap even if they are hidden. It does not help the caster avoid stone face traps or exploding mushrooms, nor does it avoid traps placed on containers. Any harmful liquids on a floor can also be avoided.",
					ManaCost = 55,
					MinSkill = 65,
					TargetType = TargetFlags.None
				},
				203,
				9031,
				Reagent.SpidersSilk,Reagent.PixieSkull,Reagent.ButterflyWings
			);

		public ResearchAirWalk( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public static void RemoveEffect( Mobile m )
		{
			m.EndAction( typeof( ResearchAirWalk ) );
		}

		public static bool UnderEffect( Mobile m )
		{
			if ( !m.CanBeginAction( typeof( ResearchAirWalk ) ) )
				return true;

			return false;
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				if ( !Caster.CanBeginAction( typeof( ResearchAirWalk ) ) )
				{
					ResearchAirWalk.RemoveEffect( Caster );
				}

				int TotalTime = (int)( ( DamagingSkill( Caster ) * 20 ) / 60 );
				new InternalTimer( Caster, TimeSpan.FromSeconds( TotalTime ) ).Start();
				Caster.BeginAction( typeof( ResearchAirWalk ) );
				Point3D air = new Point3D( ( Caster.X+1 ), ( Caster.Y+1 ), ( Caster.Z+5 ) );
				Effects.SendLocationParticles(EffectItem.Create(air, Caster.Map, EffectItem.DefaultDuration), 0x5590, 9, 32, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, 5022, 0);
				Caster.PlaySound( 0x014 );
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );

				BuffInfo.RemoveBuff( Caster, BuffIcon.AirWalk );
				BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.AirWalk, 1063632, 1063633, TimeSpan.FromSeconds( TotalTime ), Caster));
			}
            FinishSequence();
		}

		private class InternalTimer : Timer
		{
			private Mobile m_m;
			private DateTime m_Expire;

			public InternalTimer( Mobile Caster, TimeSpan duration ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 0.1 ) )
			{
				m_m = Caster;
				m_Expire = DateTime.Now + duration;
			}

			protected override void OnTick()
			{
				if ( DateTime.Now >= m_Expire )
				{
					ResearchAirWalk.RemoveEffect( m_m );
					BuffInfo.RemoveBuff( m_m, BuffIcon.AirWalk );
					Stop();
				}
			}
		}
	}
}