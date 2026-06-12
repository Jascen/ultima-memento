using System;
using Server;
using Server.Items;

namespace Server.Spells.Magical
{
	public abstract class MagicalSpell : Spell
	{
		public abstract double RequiredSkill{ get; }
		public abstract int RequiredMana{ get; }
		public override SkillName CastSkill{ get{ return SkillName.Focus; } }
		public override SkillName DamageSkill{ get{ return SkillName.Focus; } }
		public override bool ClearHandsOnCast{ get{ return false; } }
		public override double CastDelayFastScalar{ get{ return (Core.SE? base.CastDelayFastScalar : 0); } }
		public abstract SpellCircle Circle { get; }
		public int OneBasedCircle { get { return (int)Circle + 1; } }

		public MagicalSpell( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info )
		{
		}

		public override void GetCastSkills( out double min, out double max )
		{
			min = RequiredSkill;
			max = Scroll != null ? min : RequiredSkill;
		}

		public override bool ConsumeReagents()
		{
			return true;
		}

		public override int GetMana()
		{
			return RequiredMana;
		}

		public override double GetResistSkill( Mobile m )
		{
			return m.Skills[SkillName.MagicResist].Value;
		}

		public virtual bool CheckResisted( Mobile target )
		{
			return false;
		}

		public virtual double GetResistPercentForCircle( Mobile target, SpellCircle circle )
		{
			return 0.0;
		}

		public virtual double GetResistPercent( Mobile target )
		{
			return GetResistPercentForCircle( target, Circle );
		}
	}
}