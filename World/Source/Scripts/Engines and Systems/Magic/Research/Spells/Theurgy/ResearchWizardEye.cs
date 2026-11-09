using System;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchWizardEye : ResearchSpell
	{
		public int CirclePower = 1;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 663,
					IconGraphic = 11245,
					Name = "Wizard Eye",
					PowerWords = "Por Ort Wis",
					Description = "This spell allows the caster to identify items that have origins that are unknown to others.",
					ManaCost = 30,
					MinSkill = 50,
					TargetType = TargetFlags.None
				},
				215,
				9001,
				Reagent.EyeOfToad,Reagent.SilverWidow,Reagent.GargoyleEar,Reagent.BlackPearl
			);

		public ResearchWizardEye( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				Caster.Target = new InternalTarget( this, spellIndex, Scroll, alwaysConsume );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ResearchWizardEye m_Owner;
			private int m_SpellIndex;
			private Item m_fromBook;
			private bool m_alwaysConsume;

			public InternalTarget( ResearchWizardEye owner, int spellIndex, Item fromBook, bool alwaysConsume ) : base( Core.ML ? 10 : 12, false, TargetFlags.None )
			{
				m_Owner = owner;
				m_fromBook = fromBook;
				m_SpellIndex = spellIndex;
				m_alwaysConsume = alwaysConsume;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( Server.Items.ArtifactManual.LookupTheItem( from, targeted ) )
				{
					Server.Misc.Research.ConsumeScroll( from, true, m_SpellIndex, m_alwaysConsume, m_fromBook );
					from.PlaySound( 0xF7 );
				}
				m_Owner.FinishSequence();
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}
