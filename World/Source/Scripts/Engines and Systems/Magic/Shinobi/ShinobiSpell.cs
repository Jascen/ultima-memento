using System;
using Server.Network;

namespace Server.Spells.Shinobi
{
	public abstract class ShinobiSpell : Spell
	{
		public abstract double RequiredSkill{ get; }
		public abstract int RequiredMana{ get; }
		public abstract int RequiredTithing{ get; }
		public override bool ClearHandsOnCast { get { return false; } }
		public override SkillName CastSkill { get { return SkillName.Ninjitsu; } }
		public override SkillName DamageSkill { get { return SkillName.Ninjitsu; } }
		public override int CastRecoveryBase { get { return 2; } }

		public ShinobiSpell( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info )
		{
		}

		public override bool CheckCast()
		{
			int mana = ScaleMana( RequiredMana );

			if ( !base.CheckCast() )
				return false;

			if ( Caster.TithingPoints < RequiredTithing )
			{
				Caster.SendMessage( "You must have at least " + RequiredTithing.ToString() + " Tithing Points to use this ability." );
				return false;
			}
			else if ( Caster.Skills[SkillName.Ninjitsu].Value < RequiredSkill )
			{
				Caster.SendMessage( "You are not a skilled enough ninja to do that!" );
				return false;
			}
			else if ( Caster.Mana < mana )
			{
				Caster.SendLocalizedMessage( 1060174, mana.ToString() ); // You must have at least ~1_MANA_REQUIREMENT~ Mana to use this ability.
				return false;
			}
			else if ( this is CheetahPaws && MySettings.S_NoMountsInCertainRegions && Server.Mobiles.AnimalTrainer.IsNoMountRegion( Caster, Region.Find( Caster.Location, Caster.Map ) ) )
			{
				Caster.SendMessage( "This ability doesn't seem to work in this place." );
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

			if ( Caster.TithingPoints < requiredTithing )
			{
				Caster.SendMessage( "You must have at least " + RequiredTithing.ToString() + " Tithing Points to use this ability." );
				return false;
			}
			else if ( Caster.Skills[SkillName.Ninjitsu].Value < RequiredSkill )
			{
				Caster.SendMessage( "You are not a skilled enough ninja to do that!" );
				return false;
			}
			else if ( Caster.Mana < mana )
			{
				Caster.SendLocalizedMessage( 1060174, mana.ToString() ); // You must have at least ~1_MANA_REQUIREMENT~ Mana to use this ability.
				return false;
			}
			else if ( this is CheetahPaws && MySettings.S_NoMountsInCertainRegions && Server.Mobiles.AnimalTrainer.IsNoMountRegion( Caster, Region.Find( Caster.Location, Caster.Map ) ) )
			{
				Caster.SendMessage( "This ability doesn't seem to work in this place." );
				return false;
			}

			Caster.TithingPoints -= requiredTithing;

			return base.CheckFizzle();
		}

		public override void SayMantra()
		{
			Caster.PublicOverheadMessage( MessageType.Regular, 0x3B2, false, Info.Mantra );
		}

		public override void DoFizzle()
		{
			Caster.PlaySound( Caster.Female ? 0x319 : 0x429 );
			Caster.NextSpellTime = DateTime.Now;
		}

		public override void DoHurtFizzle()
		{
			Caster.PlaySound( Caster.Female ? 0x319 : 0x429 );
		}

		public virtual bool CheckResisted( Mobile target )
		{
			return false;
		}

		public override int GetMana()
		{
			return RequiredMana;
		}

		public override void OnDisturb( DisturbType type, bool message )
		{
			base.OnDisturb( type, message );

			if ( message )
			{
				Caster.PlaySound( Caster.Female ? 0x319 : 0x429 );
			}
		}

		public override void GetCastSkills( out double min, out double max )
		{
			min = RequiredSkill;
			max = RequiredSkill + 30.0;
		}
	}
}
