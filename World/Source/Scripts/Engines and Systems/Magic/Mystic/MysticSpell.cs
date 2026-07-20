using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Items;

namespace Server.Spells.Mystic
{
	public abstract class MysticSpell : Spell
	{
		public abstract int RequiredTithing { get; }
		public abstract double RequiredSkill { get; }
		public abstract int RequiredMana{ get; }
		public override bool ClearHandsOnCast { get { return false; } }
		public override SkillName CastSkill { get { return SkillName.FistFighting; } }
		public override SkillName DamageSkill { get { return SkillName.FistFighting; } }
		public override int CastRecoveryBase { get { return 2; } }
		public abstract int MysticSpellCircle{ get; }

		public MysticSpell( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info )
		{
		}

		public static string SpellDescription( int spell )
		{
			string txt = "This parchment holds the knowledge of the mystics: ";
			string skl = "0";

			if ( spell == 250 ){ 			skl = "80";	txt += "Enter the astral plane where your soul is immune to harm. While you are in this state, you can freely travel but your interraction with the world is minimal. The better your skill, the longer it lasts. Monks use this ability to safely travel through dangerous areas."; }
			else if ( spell == 251 ){ 		skl = "50";	txt += "Travel through the astral plane to another location with the use of a magical recall rune. The rune must be marked by other magical means before you can travel to that location. If you wish to travel using a rune book, then set your rune book's default location and then you can target the book while using this ability."; }
			else if ( spell == 252 ){ 		skl = "25";	txt += "Creates a robe that you will need in order to use the other abilities in this tome. The robe will have power based on your overall skill as a monk, and no one else may wear the robe. You can only have one such robe at a time, so creating a new robe will cause any others you own to go back to the astral plane. After creation, single click the robe and select the 'Enchant' option to spend the points on attributes you want the robe to have."; }
			else if ( spell == 253 ){ 		skl = "30";	txt += "Perform a soothing touch, healing damage sustained. The higher your skill, the more damage you will heal with your touch."; }
			else if ( spell == 254 ){ 		skl = "35";	txt += "Allows you to leap over a long distance. This is a quick action and can allow a monk to leap toward an opponent, leap away to safety, or leap over some obstacles like rivers and streams."; }
			else if ( spell == 255 ){ 		skl = "30";	txt += "Summon your Ki to perform a mental attack that deals an amount of energy damage based upon your fist fighting and intelligence values. Elemental Resistances may reduce damage done by this attack."; }
			else if ( spell == 256 ){ 		skl = "60";	txt += "You sheer force of will creates a barrier around you, deflecting magical attacks. This does not work against odd magics like necromancy. Affected spells will often bounce back onto the caster."; }
			else if ( spell == 257 ){ 		skl = "40";	txt += "You can cleanse your body of poisons with this ability due to your physical discipline, and as such, it cannot be used to aid anyone else."; }
			else if ( spell == 258 ){ 		skl = "20";	txt += "You must be wearing some sort of pugilist gloves for this ability to work. It temporarily enhances the kind of damage the gloves do. The type of damage inflicted when hitting a target will be converted to the target's worst resistance type. The duration of the effect is affected by your fist fighting skill."; }
			else if ( spell == 259 )
			{
				skl = "70";	txt += "This ability allows the monk to run as fast as a steed. This ability should be avoided if you already have a mount you are riding, or perhaps you have magical boots that allow you to run at this speed. using this ability in such conditions may cause unusual travel speeds, so be leery.";
				if ( MySettings.S_NoMountsInCertainRegions )
					txt += " Be aware when exploring the land, that there are some areas you cannot use this ability in. These are areas such as dungeons, caves, and some indoor areas. If you enter such an area, this ability will be hindered.";
			}

			if ( skl == "0" )
				return txt;

			return txt + " It requires a Mystic to be at least a " + skl + " in ability to use it.";
		}

		public override bool CheckCast()
		{
			int mana = ScaleMana( RequiredMana );

			if ( !base.CheckCast() )
				return false;

			if ( Caster.TithingPoints < RequiredTithing )
			{
				Caster.SendLocalizedMessage( 1060173, RequiredTithing.ToString() ); // You must have at least ~1_TITHE_REQUIREMENT~ Tithing Points to use this ability,
				return false;
			}
			else if ( !MonkNotIllegal( Caster ) && !( this is CreateRobe ) )
			{
				Caster.SendMessage( "Your equipment or skills are not commensurate to that of a true monk." );
				return false;
			}
			else if ( this is WindRunner && MySettings.S_NoMountsInCertainRegions && Server.Mobiles.AnimalTrainer.IsNoMountRegion( Caster, Region.Find( Caster.Location, Caster.Map ) ) )
			{
				Caster.SendMessage( "This ability doesn't seem to work in this place." );
				return false;
			}
			else if ( Caster.Mana < mana )
			{
				Caster.SendLocalizedMessage( 1060174, mana.ToString() ); // You must have at least ~1_MANA_REQUIREMENT~ Mana to use this ability.
				return false;
			}

			return true;
		}

		public static bool MonkNotIllegal( Mobile from )
		{
			if ( from.FindItemOnLayer( Layer.OneHanded ) != null )
			{
				Item oneHand = from.FindItemOnLayer( Layer.OneHanded );

				if ( oneHand is IPugilistGlove ){}
				else if ( oneHand is BaseWeapon )
					return false;
			}

			if ( from.FindItemOnLayer( Layer.TwoHanded ) != null )
			{
				Item twoHand = from.FindItemOnLayer( Layer.TwoHanded );

				if ( twoHand is BaseWeapon )
					return false;

				if ( twoHand is BaseArmor )
				{
					if ( ((BaseArmor)twoHand).Attributes.SpellChanneling == 0 )
						return false;
				}
			}

			if ( Server.Misc.RegenRates.GetArmorOffset( from ) > 0 )
			{
				return false;
			}

			if ( from.FindItemOnLayer( Layer.OuterTorso ) != null )
			{
				Item robe = from.FindItemOnLayer( Layer.OuterTorso );
				if ( !(robe is MysticMonkRobe) )
					return false;
			}
			else { return false; }

			if ( from.Skills[SkillName.Focus].Base < 100 || from.Skills[SkillName.Meditation].Base < 100 )
			{
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
				Caster.SendLocalizedMessage( 1060173, RequiredTithing.ToString() ); // You must have at least ~1_TITHE_REQUIREMENT~ Tithing Points to use this ability,
				return false;
			}
			else if ( !MonkNotIllegal( Caster ) && !( this is CreateRobe ) )
			{
				Caster.SendMessage( "Your equipment or skills are not commensurate to that of a true monk." );
				return false;
			}
			else if ( this is WindRunner && MySettings.S_NoMountsInCertainRegions && Server.Mobiles.AnimalTrainer.IsNoMountRegion( Caster, Region.Find( Caster.Location, Caster.Map ) ) )
			{
				Caster.SendMessage( "This ability doesn't seem to work in this place." );
				return false;
			}
			else if ( Caster.Mana < mana )
			{
				Caster.SendLocalizedMessage( 1060174, mana.ToString() ); // You must have at least ~1_MANA_REQUIREMENT~ Mana to use this ability.
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
			Caster.PlaySound( 0x1D6 );
			Caster.NextSpellTime = DateTime.Now;
		}

		public override void DoHurtFizzle()
		{
			Caster.PlaySound( 0x1D6 );
		}

		public virtual bool CheckResisted( Mobile target )
		{
			double n = GetResistPercent( target );

			n /= 100.0;

			if( n <= 0.0 )
				return false;

			if( n >= 1.0 )
				return true;

			int maxSkill = (1 + (int)MysticSpellCircle) * 10;
			maxSkill += (1 + ((int)MysticSpellCircle / 6)) * 25;

			if( target.Skills[SkillName.MagicResist].Value < maxSkill )
				target.CheckSkill( SkillName.MagicResist, 0.0, 120.0 );

			return (n >= Utility.RandomDouble());
		}

		public virtual double GetResistPercentForCircle( Mobile target )
		{
			double firstPercent = target.Skills[SkillName.MagicResist].Value / 5.0;
			double secondPercent = target.Skills[SkillName.MagicResist].Value - (((Caster.Skills[CastSkill].Value - 20.0) / 5.0) + (1 + (int)MysticSpellCircle) * 5.0);

			return (firstPercent > secondPercent ? firstPercent : secondPercent) / 2.0; // Seems should be about half of what stratics says.
		}

		public virtual double GetResistPercent( Mobile target )
		{
			return GetResistPercentForCircle( target );
		}

		public override void OnDisturb( DisturbType type, bool message )
		{
			base.OnDisturb( type, message );

			if ( message )
				Caster.PlaySound( 0x1D6 );
		}

		public override void OnBeginCast()
		{
			base.OnBeginCast();

			SendCastEffect();
		}

		public virtual void SendCastEffect()
		{
			Caster.FixedEffect( 0x37C4, 10, (int)( GetCastDelay().TotalSeconds * 28 ), 4, 3 );
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
	}
}
