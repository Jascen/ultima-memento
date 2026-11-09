using System;
using Server.Targeting;
using Server.Items;

namespace Server.Spells.Research
{
	public class ResearchSeeTruth : ResearchSpell
	{
		public int CirclePower = 1;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 645,
					IconGraphic = 11243,
					Name = "See Truth",
					PowerWords = "An Quas Lor",
					Description = "This magic is an ancient form of that which gypsies use to see the truth in things. If you cast this spell on a book or parchment that you feel may be falsely written, you may learn the truth of things.",
					ManaCost = 60,
					MinSkill = 20,
					TargetType = TargetFlags.None
				},
				215,
				9001,
				Reagent.GraveDust,Reagent.SeaSalt,Reagent.FairyEgg
			);

		public ResearchSeeTruth( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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
			private ResearchSeeTruth m_Owner;
			private int m_SpellIndex;
			private Item m_fromBook;
			private bool m_alwaysConsume;

			public InternalTarget( ResearchSeeTruth owner, int spellIndex, Item fromBook, bool alwaysConsume ) : base( Core.ML ? 10 : 12, false, TargetFlags.None )
			{
				m_Owner = owner;
				m_fromBook = fromBook;
				m_SpellIndex = spellIndex;
				m_alwaysConsume = alwaysConsume;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				bool consume = false;

				if ( targeted is ScrollClue )
				{
					ScrollClue scroll = (ScrollClue)targeted;
					consume = true;
					from.PlaySound( 0xF9 );

					if ( scroll.ScrollIntelligence > 0 )
					{
						from.SendMessage("That parchment hasn't been deciphered yet.");
					}
					else
					{
						string WillSay = "";

						switch ( Utility.RandomMinMax( 0, 3 ) ) 
						{
							case 0: WillSay = "The spirits tell you that this parchment is"; break;
							case 1: WillSay = "Your mind is showing you that this parchment is"; break;
							case 2: WillSay = "The voices all speak that this parchment is"; break;
							case 3: WillSay = "You can see beyond that this parchment is"; break;
						}

						if ( scroll.ScrollTrue == 1 )
						{
							from.SendMessage(WillSay + " truthfully written.");
						}
						else
						{
							from.SendMessage(WillSay + " falsely written.");
						}
					}
				}
				///////////////////////////////////////////////////////////////////////////////////
				else if ( targeted is SearchPage )
				{
					SearchPage scroll = (SearchPage)targeted;
					consume = true;
					from.PlaySound( 0xF9 );

					string WillSay = "";

					switch ( Utility.RandomMinMax( 0, 3 ) ) 
					{
						case 0: WillSay = "The spirits tell you that this legend "; break;
						case 1: WillSay = "Your mind is showing you that this legend "; break;
						case 2: WillSay = "The voices all speak that this legend "; break;
						case 3: WillSay = "You can see beyond that this legend "; break;
					}

					if ( scroll.LegendReal == 1 )
					{
						from.SendMessage(WillSay + " really happened.");
					}
					else
					{
						from.SendMessage(WillSay + " never happened.");
					}
				}
				///////////////////////////////////////////////////////////////////////////////////
				else if ( targeted is DynamicBook )
				{
					DynamicBook scroll = (DynamicBook)targeted;
					consume = true;
					from.PlaySound( 0xF9 );

					string WillSay = "";

					switch ( Utility.RandomMinMax( 0, 3 ) ) 
					{
						case 0: WillSay = "The spirits tell you that this book "; break;
						case 1: WillSay = "Your mind is showing you that this book "; break;
						case 2: WillSay = "The voices all speak that this book "; break;
						case 3: WillSay = "You can see beyond that this book "; break;
					}

					if ( scroll.BookTrue > 0 )
					{
						from.SendMessage(WillSay + " contains the truth.");
					}
					else
					{
						from.SendMessage(WillSay + " contains falsehoods.");
					}
				}
				///////////////////////////////////////////////////////////////////////////////////
				else if ( targeted is SomeRandomNote )
				{
					SomeRandomNote scroll = (SomeRandomNote)targeted;
					consume = true;
					from.PlaySound( 0xF9 );

					string WillSay = "";

					switch ( Utility.RandomMinMax( 0, 3 ) ) 
					{
						case 0: WillSay = "The spirits tell you that this parchment is"; break;
						case 1: WillSay = "Your mind is showing you that this parchment is"; break;
						case 2: WillSay = "The voices all speak that this parchment is"; break;
						case 3: WillSay = "You can see beyond that this parchment is"; break;
					}

					if ( scroll.ScrollTrue == 1 )
					{
						from.SendMessage(WillSay + " truthfully written.");
					}
					else
					{
						from.SendMessage(WillSay + " falsely written.");
					}
				}
				///////////////////////////////////////////////////////////////////////////////////
				else if ( targeted is DataPad )
				{
					consume = true;
					from.PlaySound( 0xF9 );
					string WillSay = "";

					switch ( Utility.RandomMinMax( 0, 3 ) ) 
					{
						case 0: WillSay = "The spirits tell you that this glowing book is"; break;
						case 1: WillSay = "Your mind is showing you that this glowing book is"; break;
						case 2: WillSay = "The voices all speak that this glowing book is"; break;
						case 3: WillSay = "You can see beyond that this glowing book is"; break;
					}

					from.SendMessage(WillSay + " truthfully written.");
				}
				///////////////////////////////////////////////////////////////////////////////////
				else
				{
					from.SendMessage("That is not a book or parchment.");
				}

				if ( consume ){ Server.Misc.Research.ConsumeScroll( from, true, m_SpellIndex, m_alwaysConsume, m_fromBook ); }
				m_Owner.FinishSequence();
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}