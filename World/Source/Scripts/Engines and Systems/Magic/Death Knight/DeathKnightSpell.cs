using System;
using Server.Network;
using Server.Items;

namespace Server.Spells.DeathKnight
{
	public abstract class DeathKnightSpell : Spell
	{
		public abstract int RequiredTithing { get; }
		public abstract double RequiredSkill { get; }
		public abstract int RequiredMana{ get; }
		public override bool ClearHandsOnCast { get { return false; } }
		public override SkillName CastSkill { get { return SkillName.Knightship; } }
		public override SkillName DamageSkill { get { return SkillName.Knightship; } }
		public override int CastRecoveryBase { get { return 7; } }

		public DeathKnightSpell( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info )
		{
		}

		public static string SpellDescription( int spell )
		{
			string txt = "This skull holds the knowledge of Death Knight magic: ";
			string skl = "0";

			if ( spell == 750 ){ 		skl = "40";	txt += "Banish summoned creatures back to their realm, demons back to hell, or elementals back to their plane of existence."; }
			else if ( spell == 751 ){ 	skl = "15";	txt += "The death knight's target is healed by demonic forces for a significant amount."; }
			else if ( spell == 752 ){ 	skl = "90";	txt += "Summons the devil to battle with the death knight."; }
			else if ( spell == 753 ){ 	skl = "30";	txt += "The next target hit becomes marked by the grim reaper. All damage dealt to it is increased, but the death knight takes extra damage from other kinds of creatures."; }
			else if ( spell == 754 ){ 	skl = "5";	txt += "Your hand holds the powers of a hag, where it can remove curses from items and others."; }
			else if ( spell == 755 ){ 	skl = "70";	txt += "The death knights's enemy is scorched by a hellfire that continues to burn the enemy for a short duration."; }
			else if ( spell == 756 ){ 	skl = "25";	txt += "Calls down a bolt of energy from Lucifer himself, and temporarily stuns the enemy."; }
			else if ( spell == 757 ){ 	skl = "80";	txt += "The forces of Orcus surround the knight and refelecta a certain amount of magical effects back at the caster."; }
			else if ( spell == 758 ){ 	skl = "60";	txt += "Channels hatred to form a barrier around the target, shielding them from physical harm."; }
			else if ( spell == 759 ){ 	skl = "45";	txt += "Drains the enemy of their soul, reducing their mana for a short period of time."; }
			else if ( spell == 760 ){ 	skl = "20";	txt += "Greatly increases the target's strength for a short period."; }
			else if ( spell == 761 ){ 	skl = "10";	txt += "The death knight's enemy is damaged by a demonic energy from the nine hells."; }
			else if ( spell == 762 ){ 	skl = "35";	txt += "The death knight's target has their skin regenerate health over time."; }
			else if ( spell == 763 ){ 	skl = "50";	txt += "The death knight unleashes the forces of hell unto his nearby enemies, causing much damage."; }

			if ( skl == "0" )
				return txt;

			return txt + " It requires a Death Knight to be at least a " + skl + " in Knightship.";
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			int mana = GetMana();

			if ( Caster.Stam < mana )
			{
				Caster.SendMessage( "You are too fatigued to do that now." );
				return false;
			}
			else if ( Caster.Karma > 0 )
			{
				Caster.SendMessage( "You have too much Karma to cast this spell." );
				return false;
			}
			else if ( Caster.Skills[CastSkill].Value < RequiredSkill )
			{
				Caster.SendMessage( "You must have at least " + RequiredSkill + " Knightship to cast this spell." );
				return false;
			}
			else if ( GetSoulsInLantern( Caster ) < RequiredTithing )
			{
				Caster.SendMessage( "You must have at least " + RequiredTithing + " Souls to cast this spell." );
				return false;
			}
			else if ( Caster.Mana < mana )
			{
				Caster.SendMessage( "You must have at least " + mana + " Mana to cast this spell." );
				return false;
			}

			return true;
		}

		public override bool CheckFizzle()
		{
			int requiredTithing = GetTithing( Caster, this );
			int mana = GetMana();

			if ( Caster.Stam < mana )
			{
				Caster.SendMessage( "You are too fatigued to do that now." );
				return false;
			}
			else if ( Caster.Karma > 0 )
			{
				Caster.SendMessage( "You have too much Karma to cast this spell." );
				return false;
			}
			else if ( Caster.Skills[CastSkill].Value < RequiredSkill )
			{
				Caster.SendMessage( "You must have at least " + RequiredSkill + " Knightship to cast this spell." );
				return false;
			}
			else if ( GetSoulsInLantern( Caster ) < requiredTithing )
			{
				Caster.SendMessage( "You must have at least " + requiredTithing + " Souls to cast this spell." );
				return false;
			}
			else if ( Caster.Mana < mana )
			{
				Caster.SendMessage( "You must have at least " + mana + " Mana to cast this spell." );
				return false;
			}

			if ( !base.CheckFizzle() )
				return false;

			return true;
		}

		public override void DoFizzle()
		{
			Caster.PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, "You fail to invoke the power.", Caster.NetState);
			Caster.FixedParticles( 0x3735, 1, 30, 9503, EffectLayer.Waist );
			Caster.PlaySound( 0x19D );
			Caster.NextSpellTime = DateTime.Now;
		}

		public override int ComputeKarmaAward()
		{
			int circle = (int)(RequiredSkill / 10);
				if ( circle < 1 ){ circle = 1; }
			return -( 40 + ( 10 * circle ) );
		}

		public override int GetMana()
		{
			return RequiredMana ;
		}

		public static int GetTithing( Mobile Caster, DeathKnightSpell spell )
		{
			if ( AosAttributes.GetValue( Caster, AosAttribute.LowerRegCost ) > Utility.Random( 100 ) )
				return 0;

			return spell.RequiredTithing;
		}

		public override void SayMantra()
		{
			Caster.PublicOverheadMessage( MessageType.Regular, 0x3B2, false, Info.Mantra );
			Caster.PlaySound( 0x19E );
		}

		public override void DoHurtFizzle()
		{
			Caster.PlaySound( 0x19D );
		}

		public override void OnDisturb( DisturbType type, bool message )
		{
			base.OnDisturb( type, message );

			if ( message )
				Caster.PlaySound( 0x19D );
		}

		public override void OnBeginCast()
		{
			base.OnBeginCast();

			Caster.FixedEffect( 0x37C4, 10, 42, 4, 3 );
		}

		public override void GetCastSkills( out double min, out double max )
		{
			min = RequiredSkill;
			max = RequiredSkill + 40.0;
		}

		public int ComputePowerValue( int div )
		{
			return ComputePowerValue( Caster, div );
		}

		public static int ComputePowerValue( Mobile from, int div )
		{
			if ( from == null )
				return 0;

			int v = (int) Math.Sqrt( ( from.Karma * -1 ) + 20000 + ( from.Skills.Knightship.Fixed * 10 ) );

			return v / div;
		}

		public static void DrainSoulsInLantern( Mobile from, int tithing )
		{
			if ( AosAttributes.GetValue( from, AosAttribute.LowerRegCost ) > Utility.Random( 100 ) )
				tithing = 0;

			if ( tithing > 0 )
			{
				foreach ( Item item in World.Items.Values )
				{
					if ( item is SoulLantern )
					{
						SoulLantern lantern = (SoulLantern)item;
						if ( lantern.owner == from )
						{
							lantern.TrappedSouls = lantern.TrappedSouls - tithing;
							if ( lantern.TrappedSouls < 1 ){ lantern.TrappedSouls = 0; }
							lantern.InvalidateProperties();
						}
					}
				}
			}
		}

		public static int GetSoulsInLantern( Mobile from )
		{
			int souls = 0;

			foreach ( Item item in World.Items.Values )
			{
				if ( item is SoulLantern )
				{
					SoulLantern lantern = (SoulLantern)item;
					if ( lantern.owner == from )
					{
						souls = lantern.TrappedSouls;
					}
				}
			}

			return souls;
		}

		public static double GetKarmaPower( Mobile from )
		{
			int karma = ( from.Karma * -1 );
				if ( karma < 1 ){ karma = 0; }
				if ( karma > 15000 ){ karma = 15000; }

			double hate = karma / 125;

			return hate;
		}
	}
}