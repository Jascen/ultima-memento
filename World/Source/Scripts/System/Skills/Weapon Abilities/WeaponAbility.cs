using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Spells;

namespace Server.Items
{
	public abstract class WeaponAbility
	{
		public virtual int BaseMana{ get{ return 0; } }

		public virtual int AccuracyBonus{ get{ return 0; } }
		public virtual double DamageScalar{ get{ return 1.0; } }

		public virtual bool RequiresSE { get { return false; } }

		public virtual void OnHit( Mobile attacker, Mobile defender, int damage )
		{
		}

		public virtual void OnMiss( Mobile attacker, Mobile defender )
		{
		}

		public virtual bool OnBeforeSwing( Mobile attacker, Mobile defender )
		{
			return OnBeforeSwing(attacker, defender, true);
		}

		public virtual bool OnBeforeSwing(Mobile attacker, Mobile defender, bool validate)
		{
			return true;
		}

		public virtual bool OnBeforeDamage( Mobile attacker, Mobile defender )
		{
			return true;
		}

		public virtual bool RequiresTactics( Mobile from )
		{
			return true;
		}

		public virtual double GetRequiredSkill( Mobile from )
		{
			BaseWeapon weapon = from.Weapon as BaseWeapon;
			var skill = LookupRequiredSkill( weapon, this );
			if ( skill == 0 )
			{
				WeaponAbility weaved;
				if ( Server.Items.Bladeweave.BladeWeaving( from, out weaved ) && this == weaved )
				{
					skill = LookupRequiredSkill( weapon, Bladeweave );
				}
			}

			if ( 0 < skill ) return skill;

			return 200.0;
		}

		private double LookupRequiredSkill(BaseWeapon weapon, WeaponAbility ability)
		{
			if ( weapon != null )
			{
				if ( weapon.PrimaryAbility == ability ) return MyServerSettings.SpecialWeaponAbilSkill();
				else if ( weapon.SecondaryAbility == ability ) return MyServerSettings.SpecialWeaponAbilSkill()+10;
				else if ( weapon.ThirdAbility == ability ) return MyServerSettings.SpecialWeaponAbilSkill()+20;
				else if ( weapon.FourthAbility == ability ) return MyServerSettings.SpecialWeaponAbilSkill()+30;
				else if ( weapon.FifthAbility == ability ) return MyServerSettings.SpecialWeaponAbilSkill()+40;
			}

			return 0;
		}

		public virtual int CalculateMana( Mobile from )
		{
			int mana = BaseMana;

			double skillTotal = GetSkill( from, SkillName.Swords ) + GetSkill( from, SkillName.Bludgeoning )
				+ GetSkill( from, SkillName.Fencing ) + GetSkill( from, SkillName.Marksmanship ) + GetSkill( from, SkillName.Parry )
				+ GetSkill( from, SkillName.Lumberjacking ) + GetSkill( from, SkillName.Stealth ) + GetSkill( from, SkillName.FistFighting )
				+ GetSkill( from, SkillName.Poisoning ) + GetSkill( from, SkillName.Bushido ) + GetSkill( from, SkillName.Ninjitsu );

			if ( skillTotal >= 300.0 )
				mana -= 10;
			else if ( skillTotal >= 200.0 )
				mana -= 5;

			double scalar = 1.0;
			if ( !Server.Spells.Necromancy.MindRotSpell.GetMindRotScalar( from, ref scalar ) )
				scalar = 1.0;

			// Lower Mana Cost = 40%
			int lmc = Math.Min( AosAttributes.GetValue( from, AosAttribute.LowerManaCost ), MyServerSettings.LowerMana() );

			scalar -= (double)lmc / 100;
			mana = (int)(mana * scalar);

			// using a special move within 3 seconds of the previous special move costs double mana 
			if ( GetContext( from ) != null )
				mana *= 2;

			return mana;
		}

		public virtual bool CheckWeaponSkill( Mobile from )
		{
			BaseWeapon weapon = from.Weapon as BaseWeapon;

			if ( weapon == null )
				return false;

			Skill skill = from.Skills[weapon.Skill];
			double reqSkill = GetRequiredSkill( from );
			bool reqTactics = Core.ML && RequiresTactics( from );

			if ( Core.ML && reqTactics && from.Skills[SkillName.Tactics].Value < reqSkill )
			{
				from.SendLocalizedMessage( 1079308, reqSkill.ToString() ); // You need ~1_SKILL_REQUIREMENT~ weapon and tactics skill to perform that attack
				return false;
			}

			if ( skill != null && skill.Value >= reqSkill )
				return true;

			/* <UBWS> */
			if ( weapon.WeaponAttributes.UseBestSkill > 0 && (from.Skills[SkillName.Swords].Value >= reqSkill || from.Skills[SkillName.Bludgeoning].Value >= reqSkill || from.Skills[SkillName.Fencing].Value >= reqSkill || from.Skills[SkillName.FistFighting].Value >= reqSkill) )
				return true;
			/* </UBWS> */

			if ( reqTactics )
			{
				from.SendLocalizedMessage( 1079308, reqSkill.ToString() ); // You need ~1_SKILL_REQUIREMENT~ weapon and tactics skill to perform that attack
			}
			else
			{
				from.SendLocalizedMessage( 1060182, reqSkill.ToString() ); // You need ~1_SKILL_REQUIREMENT~ weapon skill to perform that attack
			}

			return false;
		}

		protected double GetWeaponSkill(Mobile attacker, BaseWeapon weapon)
		{
			Skill skill = attacker.Skills[weapon.Skill];
			double reqSkill = GetRequiredSkill(attacker);
			double skilltouse = 0.0;
			if (skill != null) skilltouse = skill.Value;
			if (weapon.WeaponAttributes.UseBestSkill > 0)
			{
				double skilltouse2 = 0.0;
				if ( attacker.Skills[SkillName.Swords].Value >= reqSkill ) skilltouse2 = attacker.Skills[SkillName.Swords].Value;
				if ( attacker.Skills[SkillName.Bludgeoning].Value >= reqSkill && attacker.Skills[SkillName.Bludgeoning].Value > skilltouse2 ) skilltouse2 = attacker.Skills[SkillName.Bludgeoning].Value;
				if ( attacker.Skills[SkillName.Fencing].Value >= reqSkill && attacker.Skills[SkillName.Fencing].Value > skilltouse2 ) skilltouse2 = attacker.Skills[SkillName.Fencing].Value;
				if ( attacker.Skills[SkillName.FistFighting].Value >= reqSkill && attacker.Skills[SkillName.FistFighting].Value > skilltouse2 ) skilltouse2 = attacker.Skills[SkillName.FistFighting].Value;
				if ( attacker.Skills[SkillName.Lumberjacking].Value >= reqSkill && attacker.Skills[SkillName.Lumberjacking].Value > skilltouse2 ) skilltouse2 = attacker.Skills[SkillName.Lumberjacking].Value;
				if (skilltouse2 > skilltouse) skilltouse = skilltouse2;
			}

			return skilltouse;
		}

		public virtual bool CheckSkills( Mobile from )
		{
			return CheckWeaponSkill( from );
		}

		public virtual double GetSkill( Mobile from, SkillName skillName )
		{
			Skill skill = from.Skills[skillName];

			if ( skill == null )
				return 0.0;

			return skill.Value;
		}

		public virtual bool CheckMana( Mobile from, bool consume )
		{
			int mana = CalculateMana( from );

			if ( from.Mana < mana )
			{
				from.SendLocalizedMessage( 1060181, mana.ToString() ); // You need ~1_MANA_REQUIREMENT~ mana to perform that attack
				return false;
			}

			if ( consume )
			{
				if ( GetContext( from ) == null )
				{
					Timer timer = new WeaponAbilityTimer( from );
					timer.Start();

					AddContext( from, new WeaponAbilityContext( timer ) );
				}

				from.Mana -= mana;
			}

			return true;
		}

		public virtual bool Validate( Mobile from )
		{
			if ( !from.Player )
				return true;

			NetState state = from.NetState;

			if ( state == null )
				return false;

			if( RequiresSE && !state.SupportsExpansion( Expansion.SE ) )
			{
				from.SendLocalizedMessage( 1063456 ); // You must upgrade to Samurai Empire in order to use that ability.
				return false;
			}
			
			if ( Spells.Bushido.HonorableExecution.IsUnderPenalty( from ) )
			{
				from.SendLocalizedMessage( 1063024 ); // You cannot perform this special move right now.
				return false;
			}

			if ( Core.ML && from.Spell != null )
			{
				from.SendLocalizedMessage( 1063024 ); // You cannot perform this special move right now.
				return false;
			}

			return CheckSkills( from ) && CheckMana( from, false );
		}

		private static WeaponAbility[] m_Abilities = new WeaponAbility[56]
			{
				null,
				new ArmorIgnore(),
				new BleedAttack(),
				new ConcussionBlow(),
				new CrushingBlow(),
				new Disarm(),
				new Dismount(),
				new DoubleStrike(),
				new InfectiousStrike(),
				new MortalStrike(),
				new MovingShot(),
				new ParalyzingBlow(),
				new ShadowStrike(),
				new WhirlwindAttack(),
				new RidingSwipe(),
				new FrenziedWhirlwind(),
				new Block(),
				new DefenseMastery(),
				new NerveStrike(),
				new TalonStrike(),
				new Feint(),
				new DualWield(),
				new DoubleShot(),
				new ArmorPierce(),
				new Bladeweave(),
                new ForceArrow(),
                new LightningArrow(),
                new PsychicAttack(),
                new SerpentArrow(),
                new ForceOfNature(),
				new Disrobe(),
				new AchillesStrike(),
				new ConsecratedStrike(),
				new StunningStrike(),
				new EarthStrike(),
				new FireStrike(),
				new FreezeStrike(),
				new ToxicStrike(),
				new LightningStriker(),
				new ElementalStrike(),
				new ZapStrStrike(),
				new ZapDexStrike(),
				new ZapIntStrike(),
				new ZapStamStrike(),
				new ZapManaStrike(),
				new MagicProtection(),
				new MagicProtection2(),
				new MeleeProtection(),
				new MeleeProtection2(),
				new RidingAttack(),
				new ShadowInfectiousStrike(),
				new DoubleWhirlwindAttack(),
				new FistsOfFury(),
				new SpinAttack(),
				new DevastatingBlow(),
				new DeathBlow()
			};

		public static WeaponAbility[] Abilities{ get{ return m_Abilities; } }

		private static Hashtable m_Table = new Hashtable();

		public static Hashtable Table{ get{ return m_Table; } }

		public static readonly WeaponAbility ArmorIgnore		= m_Abilities[ 1];
		public static readonly WeaponAbility BleedAttack		= m_Abilities[ 2];
		public static readonly WeaponAbility ConcussionBlow		= m_Abilities[ 3];
		public static readonly WeaponAbility CrushingBlow		= m_Abilities[ 4];
		public static readonly WeaponAbility Disarm				= m_Abilities[ 5];
		public static readonly WeaponAbility Dismount			= m_Abilities[ 6];
		public static readonly WeaponAbility DoubleStrike		= m_Abilities[ 7];
		public static readonly WeaponAbility InfectiousStrike	= m_Abilities[ 8];
		public static readonly WeaponAbility MortalStrike		= m_Abilities[ 9];
		public static readonly WeaponAbility MovingShot			= m_Abilities[10];
		public static readonly WeaponAbility ParalyzingBlow		= m_Abilities[11];
		public static readonly WeaponAbility ShadowStrike		= m_Abilities[12];
		public static readonly WeaponAbility WhirlwindAttack	= m_Abilities[13];

		public static readonly WeaponAbility RidingSwipe		= m_Abilities[14];
		public static readonly WeaponAbility FrenziedWhirlwind  = m_Abilities[15];
		public static readonly WeaponAbility Block				= m_Abilities[16];
		public static readonly WeaponAbility DefenseMastery		= m_Abilities[17];
		public static readonly WeaponAbility NerveStrike		= m_Abilities[18];
		public static readonly WeaponAbility TalonStrike		= m_Abilities[19];
		public static readonly WeaponAbility Feint				= m_Abilities[20];
		public static readonly WeaponAbility DualWield			= m_Abilities[21];
		public static readonly WeaponAbility DoubleShot			= m_Abilities[22];
		public static readonly WeaponAbility ArmorPierce		= m_Abilities[23];

		public static readonly WeaponAbility Bladeweave			= m_Abilities[24];
		public static readonly WeaponAbility ForceArrow			= m_Abilities[25];
		public static readonly WeaponAbility LightningArrow		= m_Abilities[26];
		public static readonly WeaponAbility PsychicAttack		= m_Abilities[27];
		public static readonly WeaponAbility SerpentArrow		= m_Abilities[28];
		public static readonly WeaponAbility ForceOfNature		= m_Abilities[29];

		public static readonly WeaponAbility Disrobe            = m_Abilities[30];

		public static readonly WeaponAbility AchillesStrike     = m_Abilities[31];
		public static readonly WeaponAbility ConsecratedStrike  = m_Abilities[32];
		public static readonly WeaponAbility StunningStrike     = m_Abilities[33];
		public static readonly WeaponAbility EarthStrike        = m_Abilities[34];
		public static readonly WeaponAbility FireStrike         = m_Abilities[35];
		public static readonly WeaponAbility FreezeStrike       = m_Abilities[36];
		public static readonly WeaponAbility ToxicStrike        = m_Abilities[37];
		public static readonly WeaponAbility LightningStriker    = m_Abilities[38];
		public static readonly WeaponAbility ElementalStrike    = m_Abilities[39];
		public static readonly WeaponAbility ZapStrStrike       = m_Abilities[40];
		public static readonly WeaponAbility ZapDexStrike       = m_Abilities[41];
		public static readonly WeaponAbility ZapIntStrike       = m_Abilities[42];
		public static readonly WeaponAbility ZapStamStrike      = m_Abilities[43];
		public static readonly WeaponAbility ZapManaStrike      = m_Abilities[44];
		public static readonly WeaponAbility MagicProtection    = m_Abilities[45];
		public static readonly WeaponAbility MagicProtection2   = m_Abilities[46];
		public static readonly WeaponAbility MeleeProtection    = m_Abilities[47];
		public static readonly WeaponAbility MeleeProtection2   = m_Abilities[48];
		public static readonly WeaponAbility RidingAttack       = m_Abilities[49];
		public static readonly WeaponAbility ShadowInfectiousStrike = m_Abilities[50];
		public static readonly WeaponAbility DoubleWhirlwindAttack  = m_Abilities[51];
		public static readonly WeaponAbility FistsOfFury        = m_Abilities[52];
		public static readonly WeaponAbility SpinAttack         = m_Abilities[53];
		public static readonly WeaponAbility DevastatingBlow    = m_Abilities[54];
		public static readonly WeaponAbility DeathBlow          = m_Abilities[55];

		public static bool IsWeaponAbility( Mobile m, WeaponAbility a )
		{
			if ( a == null )
				return true;

			if ( !m.Player )
				return true;

			BaseWeapon weapon = m.Weapon as BaseWeapon;

			return ( weapon != null && (weapon.PrimaryAbility == a || weapon.SecondaryAbility == a || weapon.ThirdAbility == a || weapon.FourthAbility == a || weapon.FifthAbility == a) ); // DJEYV
		}

		public virtual bool ValidatesDuringHit{ get { return true; } }

		public static WeaponAbility GetCurrentAbility( Mobile m )
		{
			if ( !Core.AOS )
			{
				ClearCurrentAbility( m );
				return null;
			}

			WeaponAbility a = (WeaponAbility)m_Table[m];

			if ( !IsWeaponAbility( m, a ) )
			{
				ClearCurrentAbility( m );
				return null;
			}

			if ( a != null && a.ValidatesDuringHit && !a.Validate( m ) )
			{
				ClearCurrentAbility( m );
				return null;
			}

			return a;
		}

		public static bool SetCurrentAbility( Mobile m, WeaponAbility a )
		{
			if ( !Core.AOS )
			{
				ClearCurrentAbility( m );
				return false;
			}

			if ( !IsWeaponAbility( m, a ) )
			{
				ClearCurrentAbility( m );
				return false;
			}

			if ( a != null && !a.Validate( m ) )
			{
				ClearCurrentAbility( m );
				return false;
			}

			if ( a == null )
			{
				m_Table.Remove( m );
			}
			else
			{
				SpecialMove.ClearCurrentMove( m );

				m_Table[m] = a;
			}

			return true;
		}

		public static void ClearCurrentAbility( Mobile m )
		{
			m_Table.Remove( m );

			CustomWeaponAbilities.Check(m);
				   
			if ( Core.AOS && m.NetState != null )
				m.Send( ClearWeaponAbility.Instance );
		}

		public WeaponAbility()
		{
		}

		private static Hashtable m_PlayersTable = new Hashtable();

		private static void AddContext( Mobile m, WeaponAbilityContext context )
		{
			m_PlayersTable[m] = context;
		}

		private static void RemoveContext( Mobile m )
		{
			WeaponAbilityContext context = GetContext( m );

			if ( context != null )
				RemoveContext( m, context );
		}

		private static void RemoveContext( Mobile m, WeaponAbilityContext context )
		{
			m_PlayersTable.Remove( m );

			context.Timer.Stop();
		}

		private static WeaponAbilityContext GetContext( Mobile m )
		{
			return ( m_PlayersTable[m] as WeaponAbilityContext );
		}

		private class WeaponAbilityTimer : Timer
		{
			private Mobile m_Mobile;

			public WeaponAbilityTimer( Mobile from ) : base ( TimeSpan.FromSeconds( 3.0 ) )
			{
				m_Mobile = from;

				Priority = TimerPriority.TwentyFiveMS;
			}

			protected override void OnTick()
			{
				RemoveContext( m_Mobile );
			}
		}

		private class WeaponAbilityContext
		{
			private Timer m_Timer;

			public Timer Timer{ get{ return m_Timer; } }

			public WeaponAbilityContext( Timer timer )
			{
				m_Timer = timer;
			}
		}
	}
}