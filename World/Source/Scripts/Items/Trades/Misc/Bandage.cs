using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class Bandage : Item, IDyable
	{
		public override string DefaultDescription{ get{ return "Bandages require a good healing or veterinary skill. When you use a bandage on someone, it will begin the attempt to heal some damage. If your skills are high enough, you can cure most poisons or even bring the dead back to life."; } }

		public static int Range = ( MySettings.S_FriendsAvoidHeels ? 5 : 2 ); 

		public override int Hue{ get { return 0; } }

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		[Constructable]
		public Bandage() : this( 1 )
		{
		}

		[Constructable]
		public Bandage( int amount ) : base( 0xE21 )
		{
			Name = "bandage";
			Stackable = true;
			Amount = amount;
			Hue = 0;
		}

		public Bandage( Serial serial ) : base( serial )
		{
		}

		public virtual bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;

			Hue = sender.DyedHue;

			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			Hue = 0;
			Name = "bandage";
		}

		// This method added for [bandself command to call.
		public static void BandSelfCommandCall( Mobile from, Item m_Bandage )
		{
			if ( from.Blessed )
			{
				from.SendMessage( "You cannot use bandages while in this state." );
				return;
			}

			from.RevealingAction();

			if ( BandageContext.BeginHeal( from, from ) != null )
				m_Bandage.Consume();
				Server.Gumps.QuickBar.RefreshQuickBar( from );
        //CombatBar
        Server.Gumps.CombatBar.Refresh( from );
        //End CombatBar
		}

		// This method added for [bandother command to call.
		public static void BandOtherCommandCall( Mobile from, Item m_Bandage )
		{
			if ( from.Blessed )
			{
				from.SendMessage( "You cannot use bandages while in this state." );
				return;
			}

			from.RevealingAction();
			Bandage band = (Bandage)m_Bandage;
			from.SendLocalizedMessage( 500948 ); // Who will you use the bandages on?
			from.Target = new InternalTarget( band );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.Blessed )
			{
				from.SendMessage( "You cannot use bandages while in this state." );
				return;
			}

			if ( from.InRange( GetWorldLocation(), Range ) )
			{
				from.RevealingAction();

				from.SendLocalizedMessage( 500948 ); // Who will you use the bandages on?

				from.Target = new InternalTarget( this );
			}
			else
			{
				from.SendLocalizedMessage( 500295 ); // You are too far away to do that.
			}
		}

		private class InternalTarget : Target
		{
			private Bandage m_Bandage;

			public InternalTarget( Bandage bandage ) : base( Bandage.Range, false, TargetFlags.Beneficial )
			{
				m_Bandage = bandage;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Bandage.Deleted )
					return;

				if ( targeted is Mobile )
				{
					if ( from.InRange( m_Bandage.GetWorldLocation(), Bandage.Range ) )
					{
						if ( BandageContext.BeginHeal( from, (Mobile)targeted ) != null )
						{
							m_Bandage.Consume();
							Server.Gumps.QuickBar.RefreshQuickBar( from );
              //CombatBar
              Server.Gumps.CombatBar.Refresh( from );
              //End CombatBar
						}
					}
					else
					{
						from.SendLocalizedMessage( 500295 ); // You are too far away to do that.
					}
				}
				else if ( targeted is HenchmanFighterItem && from.Skills[SkillName.Anatomy].Value >= 80 && from.Skills[SkillName.Healing].Value >= 80 )
				{
					HenchmanFighterItem friend = (HenchmanFighterItem)targeted;

					if ( friend.HenchDead > 0 )
					{
						friend.Name = "fighter henchman";
						friend.HenchDead = 0;
						friend.InvalidateProperties();
						m_Bandage.Consume();
					}
					else
					{
						from.SendMessage("They are not dead.");
					}
				}
				else if ( targeted is HenchmanWizardItem && from.Skills[SkillName.Anatomy].Value >= 80 && from.Skills[SkillName.Healing].Value >= 80 )
				{
					HenchmanWizardItem friend = (HenchmanWizardItem)targeted;

					if ( friend.HenchDead > 0 )
					{
						friend.Name = "wizard henchman";
						friend.HenchDead = 0;
						friend.InvalidateProperties();
						m_Bandage.Consume();
					}
					else
					{
						from.SendMessage("They are not dead.");
					}
				}
				else if ( targeted is HenchmanArcherItem && from.Skills[SkillName.Anatomy].Value >= 80 && from.Skills[SkillName.Healing].Value >= 80 )
				{
					HenchmanArcherItem friend = (HenchmanArcherItem)targeted;

					if ( friend.HenchDead > 0 )
					{
						friend.Name = "archer henchman";
						friend.HenchDead = 0;
						friend.InvalidateProperties();
						m_Bandage.Consume();
					}
					else
					{
						from.SendMessage("They are not dead.");
					}
				}
				else if (targeted is HenchmanMonsterItem && from.Skills[SkillName.Anatomy].Value >= 80 && from.Skills[SkillName.Healing].Value >= 80 )
				{
					HenchmanMonsterItem friend = (HenchmanMonsterItem)targeted;

					if ( friend.HenchDead > 0 )
					{
						friend.Name = "creature henchman";
						friend.HenchDead = 0;
						friend.InvalidateProperties();
						m_Bandage.Consume();
					}
					else
					{
						from.SendMessage("They are not dead.");
					}
				}
				else
				{
					from.SendLocalizedMessage( 500970 ); // Bandages can not be used on that.
				}
			}

			protected override void OnNonlocalTarget( Mobile from, object targeted )
			{
				base.OnNonlocalTarget( from, targeted );
			}
		}
	}

	public class BandageContext
	{
		private Mobile m_Healer;
		private Mobile m_Patient;
		private int m_Slips;
		private Timer m_Timer;

		public void Slip()
		{
			m_Healer.SendLocalizedMessage( 500961 ); // Your fingers slip!
			m_Healer.LocalOverheadMessage( MessageType.Regular, 1150, 500961 );

			++m_Slips;
		}

		public BandageContext( Mobile healer, Mobile patient, TimeSpan delay )
		{
			m_Healer = healer;
			m_Patient = patient;

			m_Timer = new InternalTimer( this, delay );
			m_Timer.Start();
		}

		public BandageContext( Mobile healer, Mobile patient, int totalMilliseconds )
		{
			m_Healer = healer;
			m_Patient = patient;

			m_Timer = new DeferredHealingTimer( this, totalMilliseconds );
			m_Timer.Start();
		}

		public void StopHeal()
		{
			m_Table.Remove( m_Healer );

			if ( m_Timer != null )
			{
				m_Timer.Stop();
				
				BuffInfo.RemoveBuff( m_Healer, BuffIcon.Bandage );
			}

			m_Timer = null;
		}

		private static Dictionary<Mobile, BandageContext> m_Table = new Dictionary<Mobile, BandageContext>();

		public static BandageContext GetContext( Mobile healer )
		{
			BandageContext bc = null;
			m_Table.TryGetValue( healer, out bc );
			return bc;
		}

		public static SkillName GetPrimarySkill( Mobile m )
		{
			if ( !m.Player && !BaseCreature.IsHenchman( m ) && (m.Body.IsMonster || m.Body.IsAnimal) )
				return SkillName.Veterinary;
			else
				return SkillName.Healing;
		}

		public static SkillName GetSecondarySkill( Mobile m )
		{
			if ( !m.Player && !BaseCreature.IsHenchman( m ) && (m.Body.IsMonster || m.Body.IsAnimal) )
				return SkillName.Druidism;
			else
				return SkillName.Anatomy;
		}

		public void EndHeal( bool partial, int toHeal, bool canGainSkill, ref int rollingHealAmount )
		{
			if ( !partial )
				StopHeal();

			int healerNumber = -1, patientNumber = -1;
			bool playSound = true;
			bool checkSkills = false;
			int slips = !partial ? m_Slips : 0;

			SkillName primarySkill = GetPrimarySkill( m_Patient );
			SkillName secondarySkill = GetSecondarySkill( m_Patient );

			BaseCreature petPatient = m_Patient as BaseCreature;

			if ( !m_Healer.Alive )
			{
				healerNumber = 500962; // You were unable to finish your work before you died.
				patientNumber = -1;
				playSound = false;
			}
			else if ( !m_Healer.InRange( m_Patient, Bandage.Range ) )
			{
				healerNumber = 500963; // You did not stay close enough to heal your target.
				patientNumber = -1;
				playSound = false;
			}
			else
			{
				bool tryHealing = true;
				if ( !partial ) // Only final heal actions here
				{
					if ( !m_Patient.Alive || (petPatient != null && petPatient.IsDeadPet) )
					{
						tryHealing = false;
						double healing = m_Healer.Skills[primarySkill].Value;
						double anatomy = m_Healer.Skills[secondarySkill].Value;
						double chance = ((healing - 68.0) / 50.0) - (slips * 0.02);

						if ( (checkSkills = (healing >= 80.0 && anatomy >= 80.0)) && chance > Utility.RandomDouble() )
						{
							if ( m_Patient.Map == null || !m_Patient.Map.CanFit( m_Patient.Location, 16, false, false ) )
							{
								healerNumber = 501042; // Target can not be resurrected at that location.
								patientNumber = 502391; // Thou can not be resurrected there!
							}
							else if ( m_Patient.Region != null && m_Patient.Region.IsPartOf( "Khaldun" ) )
							{
								healerNumber = 1010395; // The veil of death in this area is too strong and resists thy efforts to restore life.
								patientNumber = -1;
							}
							else
							{
								healerNumber = 500965; // You are able to resurrect your patient.
								patientNumber = -1;

								m_Patient.PlaySound( 0x214 );
								m_Patient.FixedEffect( 0x376A, 10, 16 );

								if ( petPatient != null && petPatient.IsDeadPet )
								{
									Mobile master = petPatient.ControlMaster;

									if( master != null && m_Healer == master )
									{
										petPatient.ResurrectPet();

										for ( int i = 0; i < petPatient.Skills.Length; ++i )
										{
											petPatient.Skills[i].Base -= 0.1;
										}
									}
									else if ( master != null && master.InRange( petPatient, 3 ) )
									{
										healerNumber = 503255; // You are able to resurrect the creature.

										master.CloseGump( typeof( PetResurrectGump ) );
										master.SendGump( new PetResurrectGump( m_Healer, petPatient ) );
									}
									else
									{
										bool found = false;

										List<Mobile> friends = petPatient.Friends;

										for ( int i = 0; friends != null && i < friends.Count; ++i )
										{
											Mobile friend = friends[i];

											if ( friend.InRange( petPatient, 3 ) )
											{
												healerNumber = 503255; // You are able to resurrect the creature.

												friend.CloseGump( typeof( PetResurrectGump ) );
												friend.SendGump( new PetResurrectGump( m_Healer, petPatient ) );

												found = true;
												break;
											}
										}

										if ( !found )
											healerNumber = 1049670; // The pet's owner must be nearby to attempt resurrection.
									}
								}
								else
								{
									m_Patient.CloseGump( typeof( ResurrectGump ) );
									m_Patient.SendGump( new ResurrectGump( m_Patient, m_Healer ) );
								}
							}
						}
						else
						{
							if ( petPatient != null && petPatient.IsDeadPet )
								healerNumber = 503256; // You fail to resurrect the creature.
							else
								healerNumber = 500966; // You are unable to resurrect your patient.

							patientNumber = -1;
						}
					}
					else if ( m_Patient.Poisoned )
					{
						tryHealing = false;
						m_Healer.SendLocalizedMessage( 500969 ); // You finish applying the bandages.
						m_Healer.LocalOverheadMessage( MessageType.Regular, 1150, 500969 );

						double healing = m_Healer.Skills[primarySkill].Value;
						double anatomy = m_Healer.Skills[secondarySkill].Value;
						double chance = ((healing - 30.0) / 50.0) - (m_Patient.Poison.Level * 0.1) - (slips * 0.02);

						if ( (checkSkills = (healing >= 60.0 && anatomy >= 60.0)) && chance > Utility.RandomDouble() )
						{
							if ( m_Patient.CurePoison( m_Healer ) )
							{
								healerNumber = (m_Healer == m_Patient) ? -1 : 1010058; // You have cured the target of all poisons.
								patientNumber = 1010059; // You have been cured of all poisons.
							}
							else
							{
								healerNumber = -1;
								patientNumber = -1;
							}
						}
						else
						{
							healerNumber = 1010060; // You have failed to cure your target!
							patientNumber = -1;
						}
					}
					else if ( BleedAttack.IsBleeding( m_Patient ) )
					{
						tryHealing = false;
						checkSkills = true;
						healerNumber = 1060088; // You bind the wound and stop the bleeding
						patientNumber = 1060167; // The bleeding wounds have healed, you are no longer bleeding!

						BleedAttack.EndBleed( m_Patient, false );
					}
					else if ( MortalStrike.IsWounded( m_Patient ) )
					{
						tryHealing = false;
						healerNumber = ( m_Healer == m_Patient ? 1005000 : 1010398 );
						patientNumber = -1;
						playSound = false;
					}
					else if ( m_Patient.Hits == m_Patient.HitsMax )
					{
						tryHealing = false;
						healerNumber = 500967; // You heal what little damage your patient had.
						patientNumber = -1;
					}
				}

				if ( tryHealing )
				{
					checkSkills = true;
					patientNumber = -1;

					double healing = m_Healer.Skills[primarySkill].Value;
					// double anatomy = m_Healer.Skills[secondarySkill].Value;
					double chance = ((healing + 10.0) / 100.0) - (slips * 0.02);

					if ( chance > Utility.RandomDouble() )
					{
						healerNumber = 500969; // You finish applying the bandages.

						if (!partial)
						{
							toHeal -= (int)(toHeal * slips * 0.35);
							if ( toHeal < 1 ) toHeal = 1;
						}

						rollingHealAmount += toHeal;

						if (MyServerSettings.EnableHealingLogging())
							m_Healer.SendMessage("Healing (Pass, now {0})", rollingHealAmount);
					}
					else
					{
						healerNumber = 500968; // You apply the bandages, but they barely help.
						playSound = false;

						if (MyServerSettings.EnableHealingLogging())
							m_Healer.SendMessage("Healing (Fail)");
					}

					if ( partial )
					{
						healerNumber = -1; // Clear the message
						playSound = false;
					}
					else
					{

						if ( rollingHealAmount < 1 || m_Patient.Hits == m_Patient.HitsMax)
						{
							rollingHealAmount = 1;
							healerNumber = 500968; // You apply the bandages, but they barely help.
						}

						m_Patient.Heal( rollingHealAmount, m_Healer, true );
						rollingHealAmount = 0; // Zero out the counter
					}
				}
			}

			if ( healerNumber != -1 ){
				m_Healer.SendLocalizedMessage( healerNumber );
				m_Healer.LocalOverheadMessage( MessageType.Regular, 1150, healerNumber );}

			if ( patientNumber != -1 ){
				m_Patient.SendLocalizedMessage( patientNumber );
				m_Healer.LocalOverheadMessage( MessageType.Regular, 1150, patientNumber );}

			if ( playSound )
				m_Patient.PlaySound( 0x57 );

			if ( checkSkills && canGainSkill)
			{
				if (MyServerSettings.EnableHealingLogging())
					m_Healer.SendMessage("Healing (Check skills)");
				m_Healer.CheckSkill( secondarySkill, 0.0, 120.0 );
				m_Healer.CheckSkill( primarySkill, 0.0, 120.0 );
			}
		}

		// This is still in use by Monsters
		private class InternalTimer : Timer
		{
			private BandageContext m_Context;

			public InternalTimer( BandageContext context, TimeSpan delay ) : base( delay )
			{
				m_Context = context;
				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				int amount = CalculateHealAmount( m_Context.m_Healer, m_Context.m_Patient );
				int _ = 0;
				m_Context.EndHeal( false, amount, true, ref _);
			}
		}

		private class DeferredHealingTimer : Timer
		{
			private readonly BandageContext m_Context;
			private readonly int m_TickHealAmount;
			private readonly int m_FinalHealAmount;
			private int m_CurrentTicks;
			private int m_MaxTicks;
			private int m_AmountToHeal;
			private const int MILLISECONDS_PER_TICK = 250;
			private const int TICKS_PER_CHECK = 4;
			private const int TICKS_PER_SECOND = 4;

			public DeferredHealingTimer( BandageContext context, int totalMilliseconds ) : base( TimeSpan.FromMilliseconds( MILLISECONDS_PER_TICK ), TimeSpan.FromMilliseconds( MILLISECONDS_PER_TICK ) )
			{
				m_Context = context;
				
				m_FinalHealAmount = CalculateHealAmount( context.m_Healer, context.m_Patient );

				// Calculate amount to partially heal
				m_MaxTicks = (int)( (double)totalMilliseconds / MILLISECONDS_PER_TICK );
				int partialHealCount = (int)( m_MaxTicks / TICKS_PER_CHECK );
				if ( 0 < partialHealCount )
				{
					m_TickHealAmount = Math.Min(
						(int)( 0.33 * m_FinalHealAmount ) / partialHealCount, // Baseline as % of the total healing, divided by the number of ticks
						(int)( 0.1 * m_FinalHealAmount ) // No more than 10% of the final heal amount
					);
					m_FinalHealAmount -= m_TickHealAmount * partialHealCount; // Deduct what will be healed over time
				}

				if (MyServerSettings.EnableHealingLogging())
					context.m_Healer.SendMessage("# Ticks ({0}) // Tick value ({1}) // Burst value ({2})", 0 < partialHealCount ? partialHealCount : 0, m_TickHealAmount, m_FinalHealAmount);
			}	

			protected override void OnTick()
			{
				bool canCheck = ++m_CurrentTicks % TICKS_PER_SECOND == 0; // Only check once every second
				bool isComplete = m_MaxTicks <= m_CurrentTicks;
				if ( isComplete )
				{
					m_Context.m_Healer.SendMessage("Final (+{0}?) after {1}ms", m_FinalHealAmount, m_CurrentTicks * MILLISECONDS_PER_TICK);
					m_Context.EndHeal( false, m_FinalHealAmount, true, ref m_AmountToHeal );
				}
				else if ( canCheck )
				{
					m_Context.m_Healer.SendMessage("Partial (+{0}?)", m_TickHealAmount);
					int totalSeconds = ( m_CurrentTicks - m_CurrentTicks % TICKS_PER_SECOND ) / TICKS_PER_SECOND;
					bool canGain = totalSeconds % 3 == 0; // Once per 3rd second
					m_Context.EndHeal( true, m_TickHealAmount, canGain, ref m_AmountToHeal );
				}
			}
		}

		private static int CalculateHealAmount( Mobile healer, Mobile patient )
		{
			SkillName primarySkill = GetPrimarySkill( patient );
			SkillName secondarySkill = GetSecondarySkill( patient );

			double healing = healer.Skills[primarySkill].Value;
			double anatomy = healer.Skills[secondarySkill].Value;
		
			double min, max;

			min = ( anatomy / 2 ) + ( healing / 2 ) + 50.0;
			max = ( anatomy / 2 ) + ( healing / 2 ) + 100.0;
 
			double toHeal = min + ( Utility.RandomDouble() * (max - min) );

			if ( patient.Body.IsMonster || patient.Body.IsAnimal )
				toHeal += patient.HitsMax / 100;

			return (int) toHeal;
		}

		public static int HealTimer( Mobile healer, Mobile patient )
		{
			int dex = healer.Dex;
			double seconds;
			double resDelay = ( patient.Alive ? 0.0 : 5.0 );

			if ( healer == patient )
				seconds = 11 - 0.25 * (dex / 5); // Breakpoints: Reduce by 0.25s for every 5 dex
			else
			{
				if ( GetPrimarySkill( patient ) == SkillName.Veterinary )
					seconds = 2.0;
				else
				{
					if ( dex < 204 )
						seconds = 3.2-(Math.Sin((double)dex/130)*2.5) + resDelay;
					else
						seconds = 0.7 + resDelay;
				}
			}
			seconds *= 1000;

			return (int)seconds;
		}

		public static BandageContext BeginHeal( Mobile healer, Mobile patient )
		{
			bool isDeadPet = ( patient is BaseCreature && ((BaseCreature)patient).IsDeadPet );

			if ( patient.Hunger < 6 && patient is PlayerMobile && patient.Alive )
			{
				healer.SendMessage( "You cannot heal those that are extremely hungry." );
			}
			else if ( patient is Golem || patient is Robot )
			{
				healer.SendLocalizedMessage( 500970 ); // Bandages cannot be used on that.
			}
			else if ( patient is BaseCreature && ((BaseCreature)patient).IsAnimatedDead )
			{
				healer.SendLocalizedMessage( 500951 ); // You cannot heal that.
			}
			else if ( !patient.Poisoned && patient.Hits == patient.HitsMax && !BleedAttack.IsBleeding( patient ) && !isDeadPet )
			{
				healer.SendLocalizedMessage( 500955 ); // That being is not damaged!
			}
			else if ( !patient.Alive && (patient.Map == null || !patient.Map.CanFit( patient.Location, 16, false, false )) )
			{
				healer.SendLocalizedMessage( 501042 ); // Target cannot be resurrected at that location.
			}
			else if ( healer.CanBeBeneficial( patient, true, true ) )
			{
				healer.DoBeneficial( patient );

				int milliseconds = HealTimer( healer, patient );

				BandageContext context = GetContext( healer );

				if ( context != null )
					context.StopHeal();
				
				context = new BandageContext( healer, patient, milliseconds );

				m_Table[healer] = context;

				if ( healer != patient && patient is PlayerMobile )
					patient.SendLocalizedMessage( 1008078, false, healer.Name ); //  : Attempting to heal you.

				healer.SendLocalizedMessage( 500956 ); // You begin applying the bandages.
				healer.LocalOverheadMessage( MessageType.Regular, 1150, 500956 );

				BuffInfo.AddBuff( healer, new BuffInfo( BuffIcon.Bandage, 1063670, TimeSpan.FromMilliseconds( milliseconds ), healer ) );

				return context;
			}

			return null;
		}
	}
}
