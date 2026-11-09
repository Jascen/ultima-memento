using System;
using Server.Targeting;
using Server.Mobiles;
using Server.Items;

namespace Server.Spells.Research
{
	public class ResearchGrantPeace : ResearchSpell
	{
		public int CirclePower = 4;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 629,
					IconGraphic = 11205,
					Name = "Grant Peace",
					PowerWords = "In Vas Corp",
					Description = "This spell can grant an undead creature the true and final death. If they cannot be laid to rest, then they may take 50-200 points of energy damage.",
					ManaCost = 35,
					MinSkill = 75,
					TargetType = TargetFlags.Harmful
				},
				203,
				9031,
				Reagent.PixieSkull,Reagent.GraveDust
			);

		public ResearchGrantPeace( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
			Caster.SendMessage( "Which undead creature do you wish to banish?" );
		}

		public void Target( Mobile m )
		{
			SlayerEntry undead = SlayerGroup.GetEntryByName( SlayerName.Silver );

			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( m is BaseCreature )
			{
				BaseCreature bc = m as BaseCreature;

				double damage = DamagingSkill( Caster )-30;
					if ( damage > 200 ){ damage = 200.0; }
					if ( damage < 50 ){ damage = 50.0; }

				if (!undead.Slays(m))
				{
					Caster.SendMessage( "This spell cannot be used on this type of creature." );
				}
				else if( bc.IsBonded )
				{
					Caster.SendMessage("This spell cannot affect such a creature!");
				}
				else if ( CheckHSequence(m) )
				{
					int exChance = (int)(m.Fame/200)+10;
					if ( DamagingSkill( Caster ) >= Utility.RandomMinMax( 1, exChance ) )
					{
						m.Say("No! You cannot send me toward the light! Death is not eternal!");
						SpellHelper.Turn(Caster, m);
						m.FixedParticles(0x3709, 10, 30, 5052, 0x480, 0, EffectLayer.LeftFoot);
						m.PlaySound(0x208);
						new InternalTimer(m).Start();
					}
					else
					{
						Caster.SendMessage( "You fail at your attempt, but did cause some energy damage." );
						m.FixedParticles(0x3709, 10, 30, 5052, 0x480, 0, EffectLayer.LeftFoot);
						m.PlaySound(0x208);
						SpellHelper.Damage(this, m, damage, 0, 0, 0, 0, 100);
					}
				}
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
			}
			FinishSequence();
		}

		private class InternalTimer : Timer
		{
			Mobile m_Owner;

			public InternalTimer( Mobile owner ) : base( TimeSpan.FromSeconds( 1.5 ) )
			{
				m_Owner = owner;
			}

			protected override void OnTick()
			{
				if ( m_Owner != null) 
				{
                    if( m_Owner.CheckAlive() )
					m_Owner.Delete();
				}
			}
		}

		private class InternalTarget : Target
		{
			private ResearchGrantPeace m_Owner;
			public InternalTarget( ResearchGrantPeace owner ) : base( 12, false, TargetFlags.Harmful )
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