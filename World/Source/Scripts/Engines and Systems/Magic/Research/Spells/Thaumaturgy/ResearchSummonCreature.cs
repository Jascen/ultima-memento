using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchSummonCreature : ResearchSpell
	{
		public int CirclePower = 4;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.25 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 652,
					IconGraphic = 11236,
					Name = "Spawn Creatures",
					PowerWords = "Vas Kal Xen",
					Description = "Summons random creatures that are controlled by the caster for 8-25 minutes.",
					ManaCost = 20,
					MinSkill = 45,
					TargetType = TargetFlags.None
				},
				269,
				9050,
				Reagent.Ginseng,Reagent.GraveDust,Reagent.PixieSkull
			);

		public ResearchSummonCreature( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if ( (Caster.Followers + 1) > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1049645 ); // You have too many followers to summon that creature.
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				double time = DamagingSkill( Caster )*2;
					if ( time > 480 ){ time = 480.0; }
					if ( time < 120 ){ time = 120.0; }

				int creatures = Caster.FollowersMax - Caster.Followers;
					if ( creatures > 3 ){ creatures = 3; }

				TimeSpan duration = TimeSpan.FromSeconds( time );

				BaseCreature m_Creature = new Rabbit();

				while ( creatures > 0 )
				{
					creatures--;
					switch ( Utility.RandomMinMax( 0, 10 ) ) 
					{
						case 0: m_Creature = new BlackBear();		break;
						case 1: m_Creature = new BrownBear();		break;
						case 2: m_Creature = new WolfDire();		break;
						case 3: m_Creature = new Panther();			break;
						case 4: m_Creature = new TigerRiding();		break;
						case 5: m_Creature = new TimberWolf();		break;
						case 6: m_Creature = new Scorpion();		break;
						case 7: m_Creature = new GiantSpider();		break;
						case 8: m_Creature = new HugeLizard();		break;
						case 9: m_Creature = new GiantToad();		break;
						case 10: m_Creature = new Slime();			break;
					}

					m_Creature.ControlSlots = 1;
					SpellHelper.Summon( m_Creature, Caster, 0x216, duration, false, false );
				}

				m_Creature.FixedParticles(0x3728, 8, 20, 5042, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.Head );
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
			}

			FinishSequence();
		}
	}
}