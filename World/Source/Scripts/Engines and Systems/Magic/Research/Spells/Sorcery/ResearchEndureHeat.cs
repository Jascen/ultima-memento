using System;
using System.Collections;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchEndureHeat : ResearchSpell
	{
		public int CirclePower = 2;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.75 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 620,
					IconGraphic = 11221,
					Name = "Endure Heat",
					PowerWords = "Sanct Flam",
					Description = "Increases the casters fire resistance for up to 8 minutes, and it will also affect the caster's allies that are no more than 3 spaces away.",
					ManaCost = 15,
					MinSkill = 20,
					TargetType = TargetFlags.None
				},
				236,
				9011,
				Reagent.SwampBerries,Reagent.GargoyleEar
			);

		public ResearchEndureHeat( Mobile caster, Item scroll) : base( caster, scroll, SpellInfo )
		{
		}

        public override void OnCast()
        {
			if( CheckSequence() )
			{
				bool success = false;

				ArrayList targets = new ArrayList();

				foreach ( Mobile m in Caster.GetMobilesInRange( 3 ) )
				{
					if ( Caster.CanBeBeneficial( m, false, true ) && !(m is Golem) )
						targets.Add( m );
				}

				for ( int i = 0; i < targets.Count; ++i )
				{
					Mobile m = (Mobile)targets[i];

					TimeSpan duration = TimeSpan.FromSeconds( (double)(DamagingSkill( Caster ) * 4) ); 
                    int amount = (int)(DamagingSkill( Caster ) / 8);

					m.SendMessage( "Your resistance to fire has increased." );
					ResistanceMod mod1 = new ResistanceMod( ResistanceType.Fire, + amount );
						
					m.AddResistanceMod( mod1 );

					string args = String.Format("{0}", amount);
					BuffInfo.RemoveBuff( m, BuffIcon.EndureHeat );
					BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.EndureHeat, 1063640, 1063641, duration, m, args.ToString(), true));

					m.PlaySound( 0x1E9 );
					m.FixedParticles( 0x375A, 9, 20, 5016, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.Waist );
						
					new ExpireTimer( m, mod1, duration ).Start();
					success = true;
				}

				if ( success ){ Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll ); }
			}
			FinishSequence();
		}

		private class ExpireTimer : Timer
		{
			private Mobile m_Mobile;
			private ResistanceMod m_Mods;

			public ExpireTimer( Mobile m, ResistanceMod mod, TimeSpan delay ) : base( delay )
			{
				m_Mobile = m;
				m_Mods = mod;
			}

			public void DoExpire()
			{
				PlayerMobile dpm = m_Mobile as PlayerMobile;
				m_Mobile.RemoveResistanceMod( m_Mods );
				
				Stop();
			}

			protected override void OnTick()
			{
				if ( m_Mobile != null )
				{
					m_Mobile.SendMessage( "The fire protection effect has worn off." );
					m_Mobile.PlaySound( 0x1F8 );
					DoExpire();
				}
			}
		}
	}
}