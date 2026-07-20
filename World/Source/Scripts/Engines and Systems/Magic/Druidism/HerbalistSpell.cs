using System;
using Server;
using Server.Network;

namespace Server.Spells.Herbalist
{
	public abstract class HerbalistSpell : Spell
	{
		public abstract double CastDelay{ get; }
		public abstract double RequiredSkill{ get; }
		public abstract int RequiredMana{ get; }
		public override SkillName CastSkill{ get{ return SkillName.Druidism; } }
		public override SkillName DamageSkill{ get{ return SkillName.Veterinary; } }
		public override bool ClearHandsOnCast{ get{ return false; } }
		public abstract int HerbalistSpellCircle{ get; }

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if ( Caster.Skills[CastSkill].Value < RequiredSkill )
			{
				Caster.PrivateOverheadMessage(MessageType.Regular, 0x14C, false, "You lack the understanding to use this.", Caster.NetState);
				return false;
			}

			return true;
		}

		public HerbalistSpell( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info )
		{
		}

		public virtual bool CheckResisted( Mobile target )
		{
			double n = GetResistPercent( target );

			n /= 100.0;

			if( n <= 0.0 )
				return false;

			if( n >= 1.0 )
				return true;

			int maxSkill = (1 + (int)HerbalistSpellCircle) * 10;
			maxSkill += (1 + ((int)HerbalistSpellCircle / 6)) * 25;

			if( target.Skills[SkillName.MagicResist].Value < maxSkill )
				target.CheckSkill( SkillName.MagicResist, 0.0, 120.0 );

			return (n >= Utility.RandomDouble());
		}

		public virtual double GetResistPercentForCircle( Mobile target )
		{
			double firstPercent = target.Skills[SkillName.MagicResist].Value / 5.0;
			double secondPercent = target.Skills[SkillName.MagicResist].Value - (((Caster.Skills[CastSkill].Value - 20.0) / 5.0) + (1 + (int)HerbalistSpellCircle) * 5.0);

			return (firstPercent > secondPercent ? firstPercent : secondPercent) / 2.0; // Seems should be about half of what stratics says.
		}

		public virtual double GetResistPercent( Mobile target )
		{
			return GetResistPercentForCircle( target );
		}

		public override void GetCastSkills( out double min, out double max )
		{
			min = RequiredSkill;
			max = RequiredSkill + 20.0;
		}

		public override int GetMana()
		{
			return RequiredMana;
		}

		public override TimeSpan GetCastDelay()
		{
			return TimeSpan.FromSeconds( CastDelay );
		}
	}
}
