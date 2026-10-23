using System;
using Server.Network;
using Server.Items;

namespace Server.Spells.Syth
{
	public abstract class SythSpell : Spell
	{
		public abstract int RequiredTithing { get; }
		public abstract double RequiredSkill { get; }
		public abstract int RequiredMana{ get; }
		public override bool ClearHandsOnCast { get { return false; } }
		public override SkillName CastSkill { get { return SkillName.Psychology; } }
		public override SkillName DamageSkill { get { return SkillName.Swords; } }
		public override int CastRecoveryBase { get { return 7; } }

		public SythSpell( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info )
		{
		}

		public static string SpellDescription( int spell )
		{
			SpellDefinition definition = SythSpellProvider.GetDefinition( spell );
			if ( definition == null )
				return "";

			return "This contains the knowledge of a Syth ability: " + definition.Description + " It requires a Syth to be at least a " + definition.MinSkill + " in ability.";
		}

		public static bool SythNotIllegal( Mobile from, bool checkSword )
		{
			bool HoldingSword = false;
			bool WearingCloth = false;

			if ( !checkSword )
			{
				HoldingSword = true;
			}
			else if ( from.FindItemOnLayer( Layer.OneHanded ) != null )
			{
				Item oneHand = from.FindItemOnLayer( Layer.OneHanded );
				if ( oneHand is BaseSword ){ HoldingSword = true; }
			}
			if ( from.FindItemOnLayer( Layer.TwoHanded ) != null )
			{
				Item twoHand = from.FindItemOnLayer( Layer.TwoHanded );
				if ( twoHand is BaseSword ){ HoldingSword = true; }
				else if ( twoHand is BaseShield && twoHand.Name != null && (twoHand.Name).Contains("Syth") ){ WearingCloth = true; }
			}

			if ( !HoldingSword ){ return false; }

			if ( from.FindItemOnLayer( Layer.Helm ) != null )
			{
				Item hat = from.FindItemOnLayer( Layer.Helm );
				if ( ( hat.ItemID == 0x4CDA || hat.ItemID == 0x4CDC || hat.ItemID == 0x2FBB ) && hat.Name != null && (hat.Name).Contains("Syth") )
					WearingCloth = true;
			}
			if ( from.FindItemOnLayer( Layer.OuterTorso ) != null )
			{
				Item robe = from.FindItemOnLayer( Layer.OuterTorso );
				if ( robe.Name != null && (robe.Name).Contains("Syth") )
					WearingCloth = true;
			}
			if ( from.FindItemOnLayer( Layer.Cloak ) != null )
			{
				Item cloak = from.FindItemOnLayer( Layer.Cloak );
				if ( cloak.Name != null && (cloak.Name).Contains("Syth") )
					WearingCloth = true;
			}
			if ( from.FindItemOnLayer( Layer.Trinket ) != null )
			{
				Item talisman = from.FindItemOnLayer( Layer.Trinket );
				if ( talisman.Name != null && (talisman.Name).Contains("Syth") && talisman.ItemID == 0x4CDE )
					WearingCloth = true;
			}

			if ( from.Skills[SkillName.Psychology].Base < 10 || from.Skills[SkillName.Swords].Base < 10 || from.Skills[SkillName.Tactics].Base < 10 )
			{
				return false;
			}
			else if ( !WearingCloth )
			{
				return false;
			}

			return true;
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if ( Caster.Karma > 0 )
			{
				Caster.SendMessage( "You have too much Karma to use this power." );
				return false;
			}
			else if ( GetSythSkillMax( Caster ) < RequiredSkill )
			{
				Caster.SendMessage( "You must have at least " + RequiredSkill + " Psychology and Swords to use this power." );
				return false;
			}
			else if ( GetCrystals( Caster ) < RequiredTithing )
			{
				Caster.SendMessage( "You must have at least " + RequiredTithing + " Crystals in your datacron to use this power." );
				return false;
			}
			else if ( Caster.Mana < GetMana() )
			{
				Caster.SendMessage( "You must have at least " + GetMana() + " Mana to use this power." );
				return false;
			}
			else if ( this is SythSpeed && MySettings.S_NoMountsInCertainRegions && Server.Mobiles.AnimalTrainer.IsNoMountRegion( Caster, Region.Find( Caster.Location, Caster.Map ) ) )
			{
				Caster.SendMessage( "This power doesn't seem to work in this place." );
				return false;
			}

			return true;
		}

		public override bool CheckFizzle()
		{
			int requiredTithing = GetTithing( Caster, this );
			int mana = GetMana();

			if ( Caster.Karma > 0 )
			{
				Caster.SendMessage( "You have too much Karma to use this power." );
				return false;
			}
			else if ( GetSythSkillMax( Caster ) < RequiredSkill )
			{
				Caster.SendMessage( "You must have at least " + RequiredSkill + " Psychology and Swords to use this power." );
				return false;
			}
			else if ( GetCrystals( Caster ) < requiredTithing )
			{
				Caster.SendMessage( "You must have at least " + requiredTithing + " Crystals in your datacron to use this power." );
				return false;
			}
			else if ( Caster.Mana < mana )
			{
				Caster.SendMessage( "You must have at least " + mana + " Mana to use this power." );
				return false;
			}
			else if ( this is SythSpeed && MySettings.S_NoMountsInCertainRegions && Server.Mobiles.AnimalTrainer.IsNoMountRegion( Caster, Region.Find( Caster.Location, Caster.Map ) ) )
			{
				Caster.SendMessage( "This power doesn't seem to work in this place." );
				return false;
			}

			DrainCrystals( Caster, requiredTithing );

			return true;
		}

		public override void FinishSequence()
		{
			base.FinishSequence();
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

		public static int GetTithing( Mobile Caster, SythSpell spell )
		{
			if ( AosAttributes.GetValue( Caster, AosAttribute.LowerRegCost ) > Utility.Random( 100 ) )
				return 0;

			return spell.RequiredTithing;
		}

		public override void SayMantra()
		{
		}

		public override void DoHurtFizzle()
		{
			Caster.PlaySound( 0x1F8 );
		}

		public override void OnDisturb( DisturbType type, bool message )
		{
			base.OnDisturb( type, message );

			if ( message )
				Caster.PlaySound( 0x1F8 );
		}

		public override void OnBeginCast()
		{
			base.OnBeginCast();

			Caster.FixedEffect( 0x37C4, 10, 42, 4, 3 );
		}

		public override void GetCastSkills( out double min, out double max )
		{
			min = RequiredSkill;
			max = RequiredSkill + 20.0;
		}

		public int ComputePowerValue( int div )
		{
			return ComputePowerValue( Caster, div );
		}

		public static int ComputePowerValue( Mobile from, int div )
		{
			if ( from == null )
				return 0;

			int v = (int) Math.Sqrt( ( from.Karma * -1 ) + 20000 + ( from.Skills.Psychology.Fixed * 10 ) );

			return v / div;
		}

		public virtual bool CheckResisted( Mobile target )
		{
			double n = GetResistPercent( target );

			n /= 100.0;

			if( n <= 0.0 )
				return false;

			if( n >= 1.0 )
				return true;

			int maxSkill = (1 + 7) * 10;
			maxSkill += (1 + (7 / 6)) * 25;

			if( target.Skills[SkillName.MagicResist].Value < maxSkill )
				target.CheckSkill( SkillName.MagicResist, 0.0, 120.0 );

			return (n >= Utility.RandomDouble());
		}

		public virtual double GetResistPercentForCircle( Mobile target )
		{
			double firstPercent = target.Skills[SkillName.MagicResist].Value / 5.0;
			double secondPercent = target.Skills[SkillName.MagicResist].Value - (((Caster.Skills[CastSkill].Value - 20.0) / 5.0) + (1 + 7) * 5.0);

			return (firstPercent > secondPercent ? firstPercent : secondPercent) / 2.0; // Seems should be about half of what stratics says.
		}

		public virtual double GetResistPercent( Mobile target )
		{
			return GetResistPercentForCircle( target );
		}

		public static void DrainCrystals( Mobile from, int tithing )
		{
			if ( AosAttributes.GetValue( from, AosAttribute.LowerRegCost ) > Utility.Random( 100 ) )
				tithing = 0;

			if ( tithing > 0 && from.Backpack != null )
			{
				var book = from.Backpack.FindItemByType<SythSpellbook>();
				if ( book != null && book.owner == from )
				{
					book.crystals -= tithing;
					if ( book.crystals < 1 ){ book.crystals = 0; }
					book.InvalidateProperties();
				}
			}
		}

		public static int GetCrystals( Mobile from )
		{
			int crystal = 0;

			if ( from.Backpack != null )
			{
				var book = from.Backpack.FindItemByType<SythSpellbook>();
				if ( book != null && book.owner == from )
				{
					crystal = book.crystals;
				}
			}

			return crystal;
		}

		public static double GetSythDamage( Mobile from )
		{
			int karma = ( from.Karma * -1 );
				if ( karma < 1 ){ karma = 0; }
				if ( karma > 15000 ){ karma = 15000; }

			double hate = karma / 120; // MAX 125
			hate = hate + from.Skills[SkillName.Tactics].Value + from.Skills[SkillName.Swords].Value; // MAX 375

			return hate;
		}

		public static bool GetSythSkill( Mobile from, int skill )
		{
			if ( from.Skills[SkillName.Swords].Value < skill ){ return false; }
			else if ( from.Skills[SkillName.Psychology].Value < skill ){ return false; }

			return true;
		}

		public static double GetSythSkillMax( Mobile from )
		{
			if ( from.Skills[SkillName.Swords].Value > from.Skills[SkillName.Psychology].Value ){ return from.Skills[SkillName.Swords].Value; }

			return from.Skills[SkillName.Psychology].Value;
		}
	}
}