using System;
using System.Collections;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchMassMight : ResearchSpell
	{
		public int CirclePower = 2;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.75 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 638,
					IconGraphic = 11212,
					Name = "Mass Might",
					PowerWords = "In Vas Por",
					Description = "Increases the casters strength for up to 8 minutes, and it will also affect the caster's allies that are no more than 10 spaces away.",
					ManaCost = 99,
					MinSkill = 66,
					TargetType = TargetFlags.None
				},
				236,
				9011,
				Reagent.FairyEgg,Reagent.ButterflyWings
			);
		
		public ResearchMassMight( Mobile caster, Item scroll) : base( caster, scroll, SpellInfo )
		{
		}

        public override void OnCast()
        {
			if( CheckSequence() )
			{
				bool success = false;
 
				ArrayList targets = new ArrayList();

				foreach ( Mobile m in Caster.GetMobilesInRange( 10 ) )
				{
					if ( Caster.CanBeBeneficial( m, false, true ) && !(m is Golem) )
						targets.Add( m );
				}

				for ( int i = 0; i < targets.Count; ++i )
				{
					Mobile m = (Mobile)targets[i];
					
                    int amount = Math.Max(1, (int)(DamagingSkill(Caster) / 16));
					string str = "str";
						
					double duration = (double)(DamagingSkill( Caster ) * 2);
						
					StatMod mod = new StatMod( StatType.Str, str, + amount, TimeSpan.FromSeconds( duration ) );

					m.AddStatMod( mod );
						
					m.FixedParticles( 0x375A, 10, 15, 5017, 0x224, 3, EffectLayer.Waist );
					success = true;

					string args = String.Format("{0}", amount);
					BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.MassMight, 1063644, 1063645, TimeSpan.FromMinutes( duration ), m, args.ToString(), true));
				}

				if ( success ){ Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll ); }
			}
			FinishSequence();
		}
	}
}