using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Items;

namespace Server.Spells.Jester
{
	public abstract class JesterSpell : Spell
	{
		public abstract int RequiredTithing { get; }
		public abstract double RequiredSkill { get; }
		public abstract int RequiredMana{ get; }
		public override bool ClearHandsOnCast { get { return false; } }
		public override SkillName CastSkill { get { return SkillName.Begging; } }
		public override SkillName DamageSkill { get { return SkillName.Psychology; } }
		public override int CastRecoveryBase { get { return 2; } }

		public JesterSpell( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info )
		{
		}

		public override bool CheckCast()
		{
			int mana = ScaleMana( RequiredMana );

			if ( !base.CheckCast() )
				return false;

			if ( Server.Items.BagOfTricks.GetPranks( Caster ) < RequiredTithing )
			{
				Caster.SendMessage( "You must have at least " + RequiredTithing.ToString() + " pranks to use this ability." );
				return false;
			}
			else if ( !Server.Misc.GetPlayerInfo.isJester( Caster ) )
			{
				Caster.SendMessage( "You are not a jester!" );
				return false;
			}
			else if ( Caster.Mana < mana )
			{
				Caster.SendLocalizedMessage( 1060174, mana.ToString() ); // You must have at least ~1_MANA_REQUIREMENT~ Mana to use this ability.
				return false;
			}

			return true;
		}

		public override bool CheckFizzle()
		{
			int requiredTithing = this.RequiredTithing;

			if ( AosAttributes.GetValue( Caster, AosAttribute.LowerRegCost ) > Utility.Random( 100 ) )
				requiredTithing = 0;

			int mana = ScaleMana( RequiredMana );

			if ( Server.Items.BagOfTricks.GetPranks( Caster ) < requiredTithing )
			{
				Caster.SendMessage( "You must have at least " + RequiredTithing.ToString() + " pranks to use this ability." );
				return false;
			}
			else if ( !Server.Misc.GetPlayerInfo.isJester( Caster ) )
			{
				Caster.SendMessage( "You are not a jester!" );
				return false;
			}
			else if ( Caster.Mana < mana )
			{
				Caster.SendLocalizedMessage( 1060174, mana.ToString() ); // You must have at least ~1_MANA_REQUIREMENT~ Mana to use this ability.
				return false;
			}

			Server.Items.BagOfTricks.UsePranks( Caster, requiredTithing );

			return base.CheckFizzle();
		}

		public override void SayMantra()
		{
			Caster.PublicOverheadMessage( MessageType.Regular, 0x3B2, false, Info.Mantra );
		}

		public override void DoFizzle()
		{
			Caster.PlaySound( Caster.Female ? 816 : 1090 );
			Caster.Say( "*sigh*" );
			Caster.NextSpellTime = DateTime.Now;
		}

		public override void DoHurtFizzle()
		{
			Caster.PlaySound( Caster.Female ? 812 : 1086 );
			Caster.Say( "*oops*" );
		}

		public virtual bool CheckResisted( Mobile target )
		{
			return false;
		}

		public override void OnDisturb( DisturbType type, bool message )
		{
			base.OnDisturb( type, message );

			if ( message )
			{
				Caster.PlaySound( Caster.Female ? 812 : 1086 );
				Caster.Say( "*oops*" );
			}
		}

		public override void OnBeginCast()
		{
			base.OnBeginCast();
		}

		public override void GetCastSkills( out double min, out double max )
		{
			min = RequiredSkill;
			max = RequiredSkill + 50.0;
		}

		public override int GetMana()
		{
			return RequiredMana;
		}

		public static int Buff( Mobile m, string category )
		{
			int value = 10;
			double var = 2.0;
				if ( m.Skills[SkillName.Psychology].Value >= Utility.RandomMinMax( 1, 400 ) ){ var = 1.5; }
				else if ( m.Skills[SkillName.Psychology].Value >= Utility.RandomMinMax( 1, 200 ) ){ var = 1.8; }

			int time = 10; 																// MIN 10
			int skill1 = (int)(m.Skills[SkillName.Begging].Value/2); 					// MAX 60
			int skill2 = (int)(m.Skills[SkillName.Psychology].Value); 						// MAX 120
			int TotalTime = (int)(( time + skill1 + skill2 ));

			int buff_default = 10;														// +10 DEFAULT
			int buff_skill1 = (int)(m.Skills[SkillName.Begging].Value / 4); 			// +25 MAX
			int buff_skill2 = (int)(m.Skills[SkillName.Psychology].Value / 2); 			// +60 MAX
			int TotalBuff = ( buff_default + buff_skill1 + buff_skill2 );

			int skill = 20; 															// MIN 20
			int skb_skill1 = (int)(m.Skills[SkillName.Begging].Value/2); 				// MAX 60
			int skb_skill2 = (int)(m.Skills[SkillName.Psychology].Value); 					// MAX 120
			int TotalSkill = (int)( skill + skb_skill1 + skb_skill2 );

			int damage = 1; 															// MIN 1
			int dmg_skill1 = (int)(m.Skills[SkillName.Begging].Value/25); 				// MAX 4
			int dmg_skill2 = (int)(m.Skills[SkillName.Psychology].Value/15); 				// MAX 8
			int TotalDamage = (int)( damage + dmg_skill1 + dmg_skill2 );

			int TotalPoison = (int)(m.Skills[SkillName.Psychology].Value/25) + 1; 			// MAX 5

			if ( category == "time" ){ value = (int)(TotalTime/var); }
			else if ( category == "strength" ){ value = (int)(TotalBuff/var); }
			else if ( category == "skills" ){ value = (int)(TotalSkill/var); }
			else if ( category == "damage" ){ value = (int)(TotalDamage/var); }
			else if ( category == "poison" ){ value = (int)(TotalPoison/var); }
			else if ( category == "hurts" ){ value = TotalBuff; }
			else if ( category == "range" ){ value = TotalPoison; }

			return value;
		}
	}
}
