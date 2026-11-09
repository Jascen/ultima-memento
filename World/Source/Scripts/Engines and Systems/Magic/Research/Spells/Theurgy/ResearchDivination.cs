using System;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;

namespace Server.Spells.Research
{
	public class ResearchDivination : ResearchSpell
	{
		public int CirclePower = 5;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 617,
					IconGraphic = 11247,
					Name = "Divination",
					PowerWords = "In Wis Tym",
					Description = "One can cast this spell on most creatures and learn things about them that are not commonly know by others.",
					ManaCost = 30,
					MinSkill = 50,
					TargetType = TargetFlags.None
				},
				215,
				9001,
				Reagent.Ginseng,Reagent.DaemonBlood,Reagent.EyeOfToad
			);

		public ResearchDivination( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
			Caster.SendMessage( "Who do you want to seek divine knowledge on?" );
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckSequence() )
			{
				bool success = false;

				if ( m is PlayerMobile )
				{
					Mobile p = m;
					Caster.CloseGump( typeof( StatsGump ) );
					Caster.SendGump( new StatsGump( Caster, p, 2 ) );
					success = true;
				}
				else if ( m is HenchmanMonster || m is HenchmanWizard || m is HenchmanFighter || m is HenchmanArcher )
				{
					Caster.SendMessage( "This spell can really tell you nothing of importance for this one." );
				}
				else if (	m is BaseVendor || m is BasePerson || m is Citizens || m is PackBeast || 
							m is FrankenPorter || m is FrankenFighter || m is HenchmanFamiliar || m is AerialServant || 
							m is GolemPorter || m is Robot || m is GolemFighter || m is HenchmanArcher || 
							m is HenchmanMonster || m is HenchmanFighter || m is HenchmanWizard )
				{
					Caster.SendMessage( "This spell can really tell you nothing of importance for this one." );
				}
				else
				{
					if ( Server.Items.PlayersHandbook.IsPeople( m ) )
					{
						BaseCreature c = (BaseCreature)m;
						Caster.CloseGump( typeof( Server.SkillHandlers.DruidismGump ) );
						Caster.SendGump( new Server.SkillHandlers.DruidismGump( Caster, c, 4 ) );
						Caster.SendSound( 0x0F9 );
						success = true;
					}
					else if ( m is BaseCreature )
					{
						BaseCreature c = (BaseCreature)m;
						Caster.CloseGump( typeof( Server.SkillHandlers.DruidismGump ) );
						Caster.SendGump( new Server.SkillHandlers.DruidismGump( Caster, c, 3 ) );
						Caster.SendSound( 0x0F9 );
						success = true;
					}
					else
					{
						Caster.SendMessage( "This spell doesn't seem to work on that." );
					}
				}

				if ( success )
				{
					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
					Caster.PlaySound( 0xF7 );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ResearchDivination m_Owner;
			public InternalTarget( ResearchDivination owner ) : base( 12, false, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( m_Owner !=null && o is Mobile )
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