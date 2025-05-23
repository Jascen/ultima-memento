using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.SkillHandlers
{
	public class Discordance
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Discordance].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.RevealingAction();

			BaseInstrument.PickInstrument( m, new InstrumentPickedCallback( OnPickedInstrument ) );

			return TimeSpan.FromSeconds( 1.0 ); // Cannot use another skill for 1 second
		}

		public static void OnPickedInstrument( Mobile from, BaseInstrument instrument )
		{
			from.RevealingAction();
			from.SendLocalizedMessage( 1049541 ); // Choose the target for your song of discordance.
			from.Target = new DiscordanceTarget( from, instrument );
			from.NextSkillTime = DateTime.Now + TimeSpan.FromSeconds( 6.0 );
		}

		private class DiscordanceInfo
		{
			public Mobile m_From;
			public Mobile m_Creature;
			public DateTime m_EndTime;
			public bool m_Ending;
			public Timer m_Timer;
			public int m_Effect;
			public ArrayList m_Mods;

			public DiscordanceInfo( Mobile from, Mobile creature, int effect, ArrayList mods )
			{
				m_From = from;
				m_Creature = creature;
				m_EndTime = DateTime.Now;
				m_Ending = false;
				m_Effect = effect;
				m_Mods = mods;

				Apply();
			}

			public void Apply()
			{
				for ( int i = 0; i < m_Mods.Count; ++i )
				{
					object mod = m_Mods[i];

					if ( mod is ResistanceMod )
						m_Creature.AddResistanceMod( (ResistanceMod) mod );
					else if ( mod is StatMod )
						m_Creature.AddStatMod( (StatMod) mod );
					else if ( mod is SkillMod )
						m_Creature.AddSkillMod( (SkillMod) mod );
				}
			}

			public void Clear()
			{
				for ( int i = 0; i < m_Mods.Count; ++i )
				{
					object mod = m_Mods[i];

					if ( mod is ResistanceMod )
						m_Creature.RemoveResistanceMod( (ResistanceMod) mod );
					else if ( mod is StatMod )
						m_Creature.RemoveStatMod( ((StatMod) mod).Name );
					else if ( mod is SkillMod )
						m_Creature.RemoveSkillMod( (SkillMod) mod );

					BuffInfo.RemoveBuff( m_Creature, BuffIcon.Discordance );
				}
			}
		}

		private static Hashtable m_Table = new Hashtable();

		public static bool GetEffect( Mobile targ, ref int effect )
		{
			DiscordanceInfo info = m_Table[targ] as DiscordanceInfo;

			if ( info == null )
				return false;

			effect = info.m_Effect;
			return true;
		}

		private static void ProcessDiscordance( DiscordanceInfo info )
		{
			Mobile from = info.m_From;
			Mobile targ = info.m_Creature;
			bool ends = false;

			// According to uoherald bard must remain alive, visible, and 
			// within range of the target or the effect ends in 15 seconds.
			if ( !targ.Alive || targ.Deleted || !from.Alive || from.Hidden )
				ends = true;
			else
			{
				int range = (int) targ.GetDistanceToSqrt( from );
				int maxRange = BaseInstrument.GetBardRange( from, SkillName.Discordance );

				if ( from.Map != targ.Map || range > maxRange )
					ends = true;
			}

			if ( ends && info.m_Ending && info.m_EndTime < DateTime.Now )
			{
				if ( info.m_Timer != null )
					info.m_Timer.Stop();

				info.Clear();
				m_Table.Remove( targ );
			}
			else
			{
				if ( ends && !info.m_Ending )
				{
					info.m_Ending = true;
					info.m_EndTime = DateTime.Now + TimeSpan.FromSeconds( 15 );
				}
				else if ( !ends )
				{
					info.m_Ending = false;
					info.m_EndTime = DateTime.Now;
				}

				targ.FixedEffect( 0x376A, 1, 32 );
			}
		}

		public class DiscordanceTarget : Target
		{
			private BaseInstrument m_Instrument;

			public DiscordanceTarget( Mobile from, BaseInstrument inst ) : base( BaseInstrument.GetBardRange( from, SkillName.Discordance ), false, TargetFlags.None )
			{
				m_Instrument = inst;
			}

			protected override void OnTarget( Mobile from, object target )
			{
				from.RevealingAction();

				if ( m_Instrument.Parent != from && !m_Instrument.IsChildOf( from.Backpack ) )
				{
					from.NextSkillTime = DateTime.Now + TimeSpan.FromSeconds( 1.0 );
					from.SendLocalizedMessage( 1062488 ); // The instrument you are trying to play is no longer in your backpack!
				}
				else if ( target is Mobile )
				{
					from.NextSkillTime = DateTime.Now + TimeSpan.FromSeconds( 3.0 );
					Mobile targ = (Mobile)target;

					if ( targ == from || (targ is BaseCreature && !from.CanBeHarmful( targ, false ) && ((BaseCreature)targ).ControlMaster != from) )
					{
						from.SendLocalizedMessage( 1049535 ); // A song of discord would have no effect on that.
					}
					else if ( m_Table.Contains( targ ) ) //Already discorded
					{
						from.SendLocalizedMessage( 1049537 );// Your target is already in discord.
					}
					else if ( targ is Mobile )
					{
						double diff = m_Instrument.GetDifficultyFor( targ ) - 10.0;
						double music = from.Skills[SkillName.Musicianship].Value;

						if ( music > 100.0 )
							diff -= (music - 100.0) * 0.5;

						if ( !BaseInstrument.CheckMusicianship( from ) )
						{
							from.SendLocalizedMessage( 500612 ); // You play poorly, and there is no effect.
							m_Instrument.PlayInstrumentBadly( from );
							m_Instrument.ConsumeUse( from );
						}
						else if ( from.CheckTargetSkill( SkillName.Discordance, target, diff-25.0, diff+25.0 ) )
						{
							bool discordMe = true;

							if ( targ.Skills[SkillName.MagicResist].Value > Utility.RandomMinMax( 0, 125 ) && targ.Player )
								discordMe = false;

							if ( discordMe )
							{
								from.SendLocalizedMessage( 1049539 ); // You play the song surpressing your targets strength
								m_Instrument.PlayInstrumentWell( from );
								m_Instrument.ConsumeUse( from );

								ArrayList mods = new ArrayList();
								int effect;
								double scalar;

								effect = (int)( from.Skills[SkillName.Discordance].Value / -5.0 );

								if ( Core.SE && BaseInstrument.GetBaseDifficulty( targ ) >= 160.0 )
									effect /= 2;

								scalar = effect * 0.01;

								mods.Add( new ResistanceMod( ResistanceType.Physical, effect ) );
								mods.Add( new ResistanceMod( ResistanceType.Fire, effect ) );
								mods.Add( new ResistanceMod( ResistanceType.Cold, effect ) );
								mods.Add( new ResistanceMod( ResistanceType.Poison, effect ) );
								mods.Add( new ResistanceMod( ResistanceType.Energy, effect ) );

								mods.Add( new StatMod( StatType.Str, "DiscordanceStr", (int)(targ.RawStr * scalar), TimeSpan.Zero ) );
								mods.Add( new StatMod( StatType.Int, "DiscordanceInt", (int)(targ.RawInt * scalar), TimeSpan.Zero ) );
								mods.Add( new StatMod( StatType.Dex, "DiscordanceDex", (int)(targ.RawDex * scalar), TimeSpan.Zero ) );

								for ( int i = 0; i < targ.Skills.Length; ++i )
								{
									if ( targ.Skills[i].Value > 0 )
										mods.Add( new DefaultSkillMod( (SkillName)i, true, targ.Skills[i].Value * scalar ) );
								}

								DiscordanceInfo info = new DiscordanceInfo( from, targ, Math.Abs( effect ), mods );
								info.m_Timer = Timer.DelayCall<DiscordanceInfo>( TimeSpan.Zero, TimeSpan.FromSeconds( 1.25 ), new TimerStateCallback<DiscordanceInfo>( ProcessDiscordance ), info );

								m_Table[targ] = info;

								BuffInfo.RemoveBuff( targ, BuffIcon.Discordance );
								string args = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", effect, (int)(targ.RawStr * scalar), (int)(targ.RawInt * scalar), (int)(targ.RawDex * scalar), scalar);
								BuffInfo.AddBuff( targ, new BuffInfo( BuffIcon.Discordance, 1063662, args.ToString() ));
								from.NextSkillTime = DateTime.Now + TimeSpan.FromSeconds( 12.0 );
							}
							else
							{
								from.SendLocalizedMessage( 1049540 );// You fail to disrupt your target
								m_Instrument.PlayInstrumentBadly( from );
								m_Instrument.ConsumeUse( from );
							}
						}
						else
						{
							from.SendLocalizedMessage( 1049540 );// You fail to disrupt your target
							m_Instrument.PlayInstrumentBadly( from );
							m_Instrument.ConsumeUse( from );
						}
					}
					else
					{
						m_Instrument.PlayInstrumentBadly( from );
					}
				}
				else
				{
					from.SendLocalizedMessage( 1049535 ); // A song of discord would have no effect on that.
				}
			}
		}
	}
}