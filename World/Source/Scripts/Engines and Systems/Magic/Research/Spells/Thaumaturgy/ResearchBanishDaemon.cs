using System;
using Server.Targeting;
using Server.Mobiles;
using Server.Items;

namespace Server.Spells.Research
{
	public class ResearchBanishDaemon : ResearchSpell
	{
		public int CirclePower = 4;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 603,
					IconGraphic = 11238,
					Name = "Banish Daemon",
					PowerWords = "An Flam Corp Xen",
					Description = "This spell can send a demonic creature back to the plane of hell from which they spawned. If they cannot be banished, then they may take 50-200 points of energy damage.",
					ManaCost = 40,
					MinSkill = 80,
					TargetType = TargetFlags.Harmful
				},
				203,
				9031,
				Reagent.GargoyleEar,Reagent.DemonClaw
			);

		public ResearchBanishDaemon( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
			Caster.SendMessage( "Which demonic creature do you wish to banish?" );
		}

		public void Target( Mobile m )
		{
			SlayerEntry exorcism = SlayerGroup.GetEntryByName( SlayerName.Exorcism );

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

				if (!exorcism.Slays(m))
				{
					Caster.SendMessage( "This spell cannot be used on this type of creature." );
				}
				else if( bc.IsBonded )
				{
					Caster.SendMessage("This spell cannot banish such a creature!");
				}
				else if ( exorcism.Slays(m) && !bc.IsDispellable )
				{
					m.Say("Your pitiful spell amuses me, mortal!");
					m.FixedParticles( 0x5508, 10, 30, 5052, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.Head );
					m.PlaySound( 0x653 );
					SpellHelper.Damage(this, m, damage, 0, 0, 0, 0, 100);
				}
				else if ( m.Fame >= 23000 )
				{
					m.Say("Your magic is puny in comparison to my power!");
					m.FixedParticles( 0x5508, 10, 30, 5052, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.Head );
					m.PlaySound( 0x653 );
					SpellHelper.Damage(this, m, damage, 0, 0, 0, 0, 100);
				}
				else if (CheckHSequence(m))
				{
					int exChance = (int)(m.Fame/200)+10;
					if ( DamagingSkill( Caster ) >= Utility.RandomMinMax( 1, exChance ) )
					{
						m.Say("No! You cannot banish me! I will return from the Underworld!");
						SpellHelper.Turn(Caster, m);
						m.FixedParticles( 0x5508, 10, 30, 5052, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.Head );
						m.PlaySound( 0x653 );
						new InternalTimer(m).Start();
					}
					else
					{
						Caster.SendMessage( "You fail at your exorcism, but did cause some energy damage." );
						m.FixedParticles( 0x5508, 10, 30, 5052, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.Head );
						m.PlaySound( 0x653 );
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
			private ResearchBanishDaemon m_Owner;
			public InternalTarget( ResearchBanishDaemon owner ) : base( 12, false, TargetFlags.Harmful )
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