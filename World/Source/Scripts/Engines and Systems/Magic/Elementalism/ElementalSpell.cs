using System;
using Server.Items;
using Server.Mobiles;
using Server.Utilities;
using System.Linq;

namespace Server.Spells.Elementalism
{
	public abstract class ElementalSpell : Spell
	{
		public override SkillName CastSkill{ get{ return SkillName.Elementalism; } }
		public override SkillName DamageSkill{ get{ return SkillName.Elementalism; } }

		public ElementalSpell( Mobile caster, Item scroll, SpellInfo info ): base( caster, scroll, info )
		{
		}

		public abstract SpellCircle Circle { get; }

		public int OneBasedCircle { get { return (int)Circle + 1; } }

		public override bool ConsumeReagents()
		{
			return true;
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if ( Caster.Stam < GetStam() )
			{
				Caster.SendMessage( "Insufficient stamina for this spell." );
				Server.Misc.IntelligentAction.FizzleSpell( Caster );
				return false;
			}
			else if ( ArmorFizzle( Caster ) >= Utility.RandomMinMax(1,100) )
			{
				Caster.SendMessage( "You are wearing too much armor to cast that (" + ArmorFizzle( Caster ) + "%)." );
				Server.Misc.IntelligentAction.FizzleSpell( Caster );
				return false;
			}

			return true;
		}

		private const double ChanceOffset = 20.0, ChanceLength = 100.0 / 7.0;

		public override void GetCastSkills( out double min, out double max )
		{
			int circle = (int)Circle;

			if( Scroll != null )
				circle -= 2;

			double avg = ChanceLength * circle;

			min = avg - ChanceOffset;
			max = avg + ChanceOffset;
		}

		private static int[] m_ManaTable = new int[] { 5, 7, 10, 14, 19, 24, 40, 50 };

		public override int GetMana()
		{
			return m_ManaTable[(int)Circle];
		}

		public static int GetPower( int circle )
		{
			return m_ManaTable[circle];
		}

		public static string GetElement( Mobile m )
		{
			int element = ((PlayerMobile)m).CharacterElement;

			string elm = "air";

			if ( element == 0 )
				elm = "air";

			else if ( element == 1 )
				elm = "earth";

			else if ( element == 2 )
				elm = "fire";

			else if ( element == 3 )
				elm = "water";

			return elm;
		}

		public int GetStam()
		{
			int stam = GetMana();

			double reduce = AosAttributes.GetValue( Caster, AosAttribute.LowerRegCost ) * 0.01;

			stam = stam - ( (int)(stam * reduce) );

			if ( stam < 0 )
				stam = 0;

			return stam;
		}

		public override double GetResistSkill( Mobile m )
		{
			int maxSkill = (1 + (int)Circle) * 10;
			maxSkill += (1 + ((int)Circle / 6)) * 25;

			if( m.Skills[SkillName.MagicResist].Value < maxSkill )
				m.CheckSkill( SkillName.MagicResist, 0.0, m.Skills[SkillName.MagicResist].Cap );

			return m.Skills[SkillName.MagicResist].Value;
		}

		public virtual bool CheckResisted( Mobile target )
		{
			double n = GetResistPercent( target );

			n /= 100.0;

			if( n <= 0.0 )
				return false;

			if( n >= 1.0 )
				return true;

			int maxSkill = (1 + (int)Circle) * 10;
			maxSkill += (1 + ((int)Circle / 6)) * 25;

			if( target.Skills[SkillName.MagicResist].Value < maxSkill )
				target.CheckSkill( SkillName.MagicResist, 0.0, target.Skills[SkillName.MagicResist].Cap );

			return (n >= Utility.RandomDouble());
		}

		public virtual double GetResistPercentForCircle( Mobile target, SpellCircle circle )
		{
			double firstPercent = target.Skills[SkillName.MagicResist].Value / 5.0;
			double secondPercent = target.Skills[SkillName.MagicResist].Value - (((Caster.Skills[CastSkill].Value - 20.0) / 5.0) + (1 + (int)circle) * 5.0);

			return (firstPercent > secondPercent ? firstPercent : secondPercent) / 2.0; // Seems should be about half of what stratics says.
		}

		public virtual double GetResistPercent( Mobile target )
		{
			return GetResistPercentForCircle( target, Circle );
		}

		public override TimeSpan CastDelayBase
		{
			get
			{
				return TimeSpan.FromSeconds( (3 + (int)Circle) * CastDelaySecondsPerTick );
			}
		}

		public static void AddWater( Mobile m )
		{
			Item water = new ElementalEffect( Utility.RandomMinMax( 0x5691, 0x569A ), 5.0, null );
			water.Name = "water";
			water.Hue = 0xB3F;
			water.MoveToWorld( m.Location, m.Map );

			int extraWater = Utility.RandomMinMax( 2, 4 );

			for( int i = 0; i < extraWater; i++ )
			{
				Item wet = new ElementalEffect( Utility.RandomMinMax( 0x5691, 0x569A ), 5.0, null );
				wet.Name = "water";
				wet.Hue = 0xB3F;
				wet.MoveToWorld( new Point3D(
				m.X + Utility.RandomMinMax( -1, 1 ),
				m.Y + Utility.RandomMinMax( -1, 1 ),
				m.Z ), m.Map );
			}
		}

		public static int ElementalHue( string element )
		{
			int hue = 0;

			int val = Utility.RandomMinMax( 0, 3 );
				if ( element == "earth" ){ val = 0; }
				else if ( element == "water" ){ val = 3; }
				else if ( element == "air" ){ val = 2; }
				else if ( element == "fire" ){ val = 1; }

			switch ( val )
			{
				case 0:	hue = Utility.RandomList( 0xB79, 0xB51, 0x85D, 0x82E, 0xB61, 0xABE, 0xABF, 0xAC0 ); break; // EARTH
				case 1:	hue = Utility.RandomList( 0xB17, 0x981, 0x86C, 0x775 ); break; // FIRE
				case 2:	hue = Utility.RandomList( 0x8C1, 0xB2B, 0x613, 0xB4D, 0xAFE, 0xAF8, 0x8E4 ); break; // AIR
				case 3:	hue = Utility.RandomList( 0x97F, 0xB3D, 0xB0A, 0x5CE ); break; // WATER
			}

			return hue;
		}

		public static string CommonInfo( int id, int cat )
		{
			string info = "";

			string shortName = "";
			string longName = "";
			string mantra = "";
			string sphere = "First";
			string skill = "0";

			if ( id == 300 ){ 	   shortName = "Armor"; longName = "Elemental Armor"; mantra = "Armura"; }
			else if ( id == 301 ){ shortName = "Bolt"; longName = "Elemental Bolt"; mantra = "Sulita"; }
			else if ( id == 302 ){ shortName = "Mend"; longName = "Elemental Mend"; mantra = "Vindeca"; }
			else if ( id == 303 ){ shortName = "Sanctuary"; longName = "Elemental Sanctuary"; mantra = "Invata"; }
			else if ( id == 304 ){ shortName = "Pain"; longName = "Elemental Pain"; mantra = "Durere"; sphere = "Second"; }
			else if ( id == 305 ){ shortName = "Protection"; longName = "Elemental Protection"; mantra = "Proteja"; sphere = "Second"; }
			else if ( id == 306 ){ shortName = "Purge"; longName = "Elemental Purge"; mantra = "Purifica"; sphere = "Second"; }
			else if ( id == 307 ){ shortName = "Steed"; longName = "Elemental Steed"; mantra = "Faptura"; sphere = "Second"; }
			else if ( id == 308 ){ shortName = "Call"; longName = "Elemental Call"; mantra = "Striga"; sphere = "Third"; skill = "9"; }
			else if ( id == 309 ){ shortName = "Force"; longName = "Elemental Force"; mantra = "Forta"; sphere = "Third"; skill = "9"; }
			else if ( id == 310 ){ shortName = "Wall"; longName = "Elemental Wall"; mantra = "Perete"; sphere = "Third"; skill = "9"; }
			else if ( id == 311 ){ shortName = "Warp"; longName = "Elemental Warp"; mantra = "Urzeala"; sphere = "Third"; skill = "9"; }
			else if ( id == 312 ){ shortName = "Field"; longName = "Elemental Field"; mantra = "Limite"; sphere = "Fourth"; skill = "23"; }
			else if ( id == 313 ){ shortName = "Restoration"; longName = "Elemental Restoration"; mantra = "Restabili"; sphere = "Fourth"; skill = "23"; }
			else if ( id == 314 ){ shortName = "Strike"; longName = "Elemental Strike"; mantra = "Lovitura"; sphere = "Fourth"; skill = "23"; }
			else if ( id == 315 ){ shortName = "Void"; longName = "Elemental Void"; mantra = "Mutare"; sphere = "Fourth"; skill = "23"; }
			else if ( id == 316 ){ shortName = "Blast"; longName = "Elemental Blast"; mantra = "Deteriora"; sphere = "Fifth"; skill = "38"; }
			else if ( id == 317 ){ shortName = "Echo"; longName = "Elemental Echo"; mantra = "Oglinda"; sphere = "Fifth"; skill = "38"; }
			else if ( id == 318 ){ shortName = "Fiend"; longName = "Elemental Fiend"; mantra = "Diavol"; sphere = "Fifth"; skill = "38"; }
			else if ( id == 319 ){ shortName = "Hold"; longName = "Elemental Hold"; mantra = "Temnita"; sphere = "Fifth"; skill = "38"; }
			else if ( id == 320 ){ shortName = "Barrage"; longName = "Elemental Barrage"; mantra = "Baraj"; sphere = "Sixth"; skill = "52"; }
			else if ( id == 321 ){ shortName = "Rune"; longName = "Elemental Rune"; mantra = "Marca"; sphere = "Sixth"; skill = "52"; }
			else if ( id == 322 ){ shortName = "Storm"; longName = "Elemental Storm"; mantra = "Furtuna"; sphere = "Sixth"; skill = "52"; }
			else if ( id == 323 ){ shortName = "Summon"; longName = "Elemental Summon"; mantra = "Convoca"; sphere = "Sixth"; skill = "52"; }
			else if ( id == 324 ){ shortName = "Devastation"; longName = "Elemental Devastation"; mantra = "Devasta"; sphere = "Seventh"; skill = "66"; }
			else if ( id == 325 ){ shortName = "Fall"; longName = "Elemental Fall"; mantra = "Toamna"; sphere = "Seventh"; skill = "66"; }
			else if ( id == 326 ){ shortName = "Gate"; longName = "Elemental Gate"; mantra = "Poarta"; sphere = "Seventh"; skill = "66"; }
			else if ( id == 327 ){ shortName = "Havoc"; longName = "Elemental Havoc"; mantra = "Haotic"; sphere = "Seventh"; skill = "66"; }
			else if ( id == 328 ){ shortName = "Apocalypse"; longName = "Elemental Apocalypse"; mantra = "Moarte"; sphere = "Eighth"; skill = "80"; }
			else if ( id == 329 ){ shortName = "Lord"; longName = "Elemental Lord"; mantra = "Dumnezeu"; sphere = "Eighth"; skill = "80"; }
			else if ( id == 330 ){ shortName = "Soul"; longName = "Elemental Soul"; mantra = "Viata"; sphere = "Eighth"; skill = "80"; }
			else if ( id == 331 ){ shortName = "Spirit"; longName = "Elemental Spirit"; mantra = "Fantoma"; sphere = "Eighth"; skill = "80"; }

			if ( cat == 1 ){ info = shortName; }
			else if ( cat == 2 ){ info = longName; }
			else if ( cat == 3 ){ info = mantra; }
			else if ( cat == 4 ){ }
			else if ( cat == 5 ){ info = longName.ToLower(); }
			else if ( cat == 6 ){ info = sphere; }
			else if ( cat == 7 ){ info = skill; }

			return info;
		}

		public static string SpellDescription( int spell )
		{
			string txt = "This scroll contains an elemental spell, where an example of what it does is provided within the Sphere of Air Elementalism: " + DescriptionInfo( spell, 0x6717 ) + "";

			return txt + " It requires at least a " + CommonInfo( spell, 7 ) + " in Elementalism.";
		}

		public static string DescriptionInfo( int id, int item )
		{
			string description = "";

			string elm = "air";

			if ( item == 0x6717 ){ elm = "air"; }
			else if ( item == 0x6713 ){ elm = "earth"; }
			else if ( item == 0x6719 ){ elm = "fire"; }
			else if ( item == 0x6715 ){ elm = "water"; }

			if ( id == 300 )
			{
				description = "Increases your physical resistance while reducing your other resistances. Active until spell is deactivated by re-casting it.";
				if ( elm == "air" ){ description = "Increases your energy resistance while reducing your other resistances. Active until spell is deactivated by re-casting it."; }
				else if ( elm == "fire" ){ description = "Increases your fire resistance while reducing your other resistances. Active until spell is deactivated by re-casting it."; }
				else if ( elm == "water" ){ description = "Increases your cold resistance while reducing your other resistances. Active until spell is deactivated by re-casting it."; }
			}
			else if ( id == 301 )
			{
				description = "Shoots a magical bolt at a target, which deals fire and physical damage.";
				if ( elm == "air" ){ description = "Shoots a magical bolt at a target, which deals energy and physical damage."; }
				else if ( elm == "earth" ){ description = "Shoots a magical bolt at a target, which deals poison and physical damage."; }
				else if ( elm == "water" ){ description = "Shoots a magical bolt at a target, which deals cold and physical damage."; }
			}
			else if ( id == 302 ){ description = "Restores the target of a small amount of lost hit points."; }
			else if ( id == 303 ){ description = "Transports the elementalist to the saftey of the Lyceum. Can cast in dungeons at higher levels."; }
			else if ( id == 304 )
			{
				description = "Affects the target with flames, dealing fire damage. The closer the target is to the caster, the more damage is dealt.";
				if ( elm == "air" ){ description = "Affects the target with a swirling wind, dealing physical and energy damage. The closer the target is to the caster, the more damage is dealt."; }
				else if ( elm == "earth" ){ description = "Affects the target with falling rocks, dealing physical damage. The closer the target is to the caster, the more damage is dealt."; }
				else if ( elm == "water" ){ description = "Affects the target with freezing water, dealing cold damage. The closer the target is to the caster, the more damage is dealt."; }
			}
			else if ( id == 305 ){ description = "Prevents the caster from having their spells disrupted, but lowers their physical resistance and magic resistance. Active until the spell is deactivated by re-casting it."; }
			else if ( id == 306 )
			{
				description = "Attempts to burn away poisons affecting the target.";
				if ( elm == "air" ){ description = "Attempts to blow away poisons affecting the target."; }
				else if ( elm == "earth" ){ description = "Attempts to cleanse poisons affecting the target."; }
				else if ( elm == "water" ){ description = "Attempts to wash away poisons affecting the target."; }
			}
			else if ( id == 307 )
			{
				description = "Summons a fiery phoenix that does not fight but you can ride throughout the land. The creature disappears after a set amount of time and requires a control slot.";
				if ( elm == "air" ){ description = "Summons an air dragon that does not fight but you can ride throughout the land. The creature disappears after a set amount of time and requires a control slot."; }
				else if ( elm == "earth" ){ description = "Summons a great bear that does not fight but you can ride throughout the land. The creature disappears after a set amount of time and requires a control slot."; }
				else if ( elm == "water" ){ description = "Summons a water beetle that does not fight but you can ride throughout the land. The creature disappears after a set amount of time and requires a control slot."; }
			}
			else if ( id == 308 )
			{
				description = "A lesser fire elemental is summoned to serve the caster. The elemental disappears after a set amount of time and requires a control slot.";
				if ( elm == "air" ){ description = "A lesser air elemental is summoned to serve the caster. The elemental disappears after a set amount of time and requires a control slot."; }
				else if ( elm == "earth" ){ description = "A lesser earth elemental is summoned to serve the caster. The elemental disappears after a set amount of time and requires a control slot."; }
				else if ( elm == "water" ){ description = "A lesser water elemental is summoned to serve the caster. The elemental disappears after a set amount of time and requires a control slot."; }
			}
			else if ( id == 309 )
			{
				description = "Shoots a ball of fire at a target, dealing some physical damage but mostly fire damage.";
				if ( elm == "air" ){ description = "Shoots a bolt of lighting at a target, dealing some physical damage but mostly energy damage."; }
				else if ( elm == "earth" ){ description = "Hurls a magical rock at a target, dealing physical damage."; }
				else if ( elm == "water" ){ description = "Forces some painful water at a target, dealing some physical damage but mostly cold damage."; }
			}
			else if ( id == 310 )
			{
				description = "Creates a temporary wall of flame that blocks movement.";
				if ( elm == "air" ){ description = "Creates a temporary wall of air that blocks movement."; }
				else if ( elm == "earth" ){ description = "Creates a temporary wall of mud that blocks movement."; }
				else if ( elm == "water" ){ description = "Creates a temporary wall of seaweed that blocks movement."; }
			}
			else if ( id == 311 ){ description = "Caster is transported to the target location."; }
			else if ( id == 312 )
			{
				description = "Creates a wall of flame that deals fire damage to all who walk through it.";
				if ( elm == "air" ){ description = "Creates a wall of electricity that deals energy damage to all who walk through it."; }
				else if ( elm == "earth" ){ description = "Creates a wall of vines that deals physical and poison damage to all who walk through it."; }
				else if ( elm == "water" ){ description = "Creates a wall of water that deals cold damage to all who walk through it."; }
			}
			else if ( id == 313 ){ description = "Restores the target of a medium amount of lost hit points."; }
			else if ( id == 314 )
			{
				description = "Strikes the target with falling lava, which deals physical and fire damage.";
				if ( elm == "air" ){ description = "Strikes the target with comets from the sky, which deals physical and energy damage."; }
				else if ( elm == "earth" ){ description = "Strikes the target with falling rocks, which deals physical damage."; }
				else if ( elm == "water" ){ description = "Strikes the target with shards of ice from above, which deals physical and cold damage."; }
			}
			else if ( id == 315 ){ description = "Caster is transported to the location marked on a rune, along with their followers. If a ship key is targeted, the caster is transported to the boat the key opens."; }
			else if ( id == 316 )
			{
				description = "Makes a flaming blast hit your target with fire damage, dependent on your elementalism and intelligence. Has a short delay.";
				if ( elm == "air" ){ description = "Makes an electrical blast hit your target with energy damage, dependent on your elementalism and intelligence. Has a short delay."; }
				else if ( elm == "earth" ){ description = "Makes a blast of stone hit your target with physical damage, dependent on your elementalism and intelligence. Has a short delay."; }
				else if ( elm == "water" ){ description = "Makes a watery blast hit your target with cold damage, dependent on your elementalism and intelligence. Has a short delay."; }
			}
			else if ( id == 317 )
			{
				string rock = "a ruby";
				if ( elm == "air" ){ rock = "an amethyst"; }
				else if ( elm == "earth" ){ rock = "an emerald"; }
				else if ( elm == "water" ){ rock = "a sapphire"; }
				description = "Harmful wizard spells cast at you will be reflected back toward the caster based on your elementalism. You will need " + rock + " to make this spell work.";
			}
			else if ( id == 318 )
			{
				description = "Conjures a lava ooze creature that attacks a target based off its combat strength and proximity. It disappears after a set amount of time and requires 2 control slots.";
				if ( elm == "air" ){ description = "Conjures a starlight creature that attacks a target based off its combat strength and proximity. It disappears after a set amount of time and requires 2 control slots."; }
				else if ( elm == "earth" ){ description = "Conjures a plant creature that attacks a target based off its combat strength and proximity. It disappears after a set amount of time and requires 2 control slots."; }
				else if ( elm == "water" ){ description = "Conjures a watery ooze creature that attacks a target based off its combat strength and proximity. It disappears after a set amount of time and requires 2 control slots."; }
			}
			else if ( id == 319 )
			{
				description = "Flame strands emerge to immobilize the target for a brief amount of time. The target's magic resistance skill affects the duration of the immobilization.";
				if ( elm == "air" ){ description = "Stars appear to immobilize the target for a brief amount of time. The target's magic resistance skill affects the duration of the immobilization."; }
				else if ( elm == "earth" ){ description = "Vines emerge to immobilize the target for a brief amount of time. The target's magic resistance skill affects the duration of the immobilization."; }
				else if ( elm == "water" ){ description = "Squid tentacles emerge to immobilize the target for a brief amount of time. The target's magic resistance skill affects the duration of the immobilization."; }
			}
			else if ( id == 320 )
			{
				description = "Launches a glob of searing plasma at the target, dealing significant fire damage.";
				if ( elm == "air" ){ description = "Launches a wave of magical electricity at the target, dealing significant energy damage."; }
				else if ( elm == "earth" ){ description = "Launches an orb of vile swamp gas at the target, dealing significant poison damage."; }
				else if ( elm == "water" ){ description = "Launches a sphere of mystical water at the target, dealing significant cold damage."; }
			}
			else if ( id == 321 ){ description = "Marks a rune to the elementalist’s current location. There are magic spells and abilities that can be used on the rune to teleport one to the location it is marked with."; }
			else if ( id == 322 )
			{
				description = "Creates a volcanic storm of molten magma, causing physical and fire damage.";
				if ( elm == "air" ){ description = "Creates a swirling storms of wind around the target, causing physical and energy damage."; }
				else if ( elm == "earth" ){ description = "Causes a swirling storm of poison ivy around the target, dealing poison and physical damage."; }
				else if ( elm == "water" ){ description = "Calls forth a typhoon of swirling wind and water, causing physical and cold damage."; }
			}
			else if ( id == 323 )
			{
				description = "A magma elemental is summoned to serve the caster.";
				if ( elm == "air" ){ description = "A lightning elemental is summoned to serve the caster."; }
				else if ( elm == "earth" ){ description = "An ent is summoned to serve the caster."; }
				else if ( elm == "water" ){ description = "An ice elemental is summoned to serve the caster."; }
			}
			else if ( id == 324 )
			{
				description = "Calls down a firey malestrom, damaging nearby enemies with fire damage.";
				if ( elm == "air" ){ description = "Brings forth powerful lightning storms, damaging nearby enemies with energy damage."; }
				else if ( elm == "earth" ){ description = "Summons piles of dirt and mud to fall onto nearby enemies, dealing physical damage."; }
				else if ( elm == "water" ){ description = "Conjures a storm of ice shards crashing down on nearby enemies, dealing cold damage."; }
			}
			else if ( id == 325 )
			{
				description = "Brings down a fire storm that affects all targets within a radius around the target location. The total physical and fire damage dealt is split amongst all targets.";
				if ( elm == "air" ){ description = "Calls down stars that go nova and affects all targets within a radius around the target location. The total physical and energy damage dealt is split amongst all targets."; }
				else if ( elm == "earth" ){ description = "Brings down a ball of swamp gas that affects all targets within a radius around the target location. Physical and poison damage dealt is split amongst all targets."; }
				else if ( elm == "water" ){ description = "Brings down a hurricane that affects all targets within a radius around the target location. The total physical and cold damage dealt is split amongst all targets."; }
			}
			else if ( id == 326 ){ description = "Targeting a marked rune opens a temporary portal to the rune’s marked location. The gateway can be used by anyone to travel to that location."; }
			else if ( id == 327 )
			{
				description = "Envelopes the target in a flames, causing a massive amount of physical and fire damage.";
				if ( elm == "air" ){ description = "Conjures hurricane force winds around the target, causing a massive amount of physical and energy damage."; }
				else if ( elm == "earth" ){ description = "Envelopes the target in a swarm of deadly bees, causing a massive amount of physical and poison damage."; }
				else if ( elm == "water" ){ description = "Creates a wave of water to burst from the target, causing a massive amount of physical and cold damage."; }
			}
			else if ( id == 328 )
			{
				description = "Calls down a fire storm onto foes near the caster, causing some physical but mostly fire damage.";
				if ( elm == "air" ){ description = "Sends shards from a nearby star crashing down onto nearby enemies, causing some physical but mostly energy damage."; }
				else if ( elm == "earth" ){ description = "Erupts a massive amount of mud and dirt from the ground, causing horrific physical damage to nearby foes."; }
				else if ( elm == "water" ){ description = "Brings down a devastating shower of watery ice onto nearby enemies, causing some physical but mostly cold damage."; }
			}
			else if ( id == 329 )
			{
				description = "A Lord of the Flame is called upon to assist the caster.";
				if ( elm == "air" ){ description = "A Lord of the Skies is called upon to assist the caster."; }
				else if ( elm == "earth" ){ description = "A Lord of the Earth is called upon to assist the caster."; }
				else if ( elm == "water" ){ description = "A Lord of the Sea is called upon to assist the caster."; }
			}
			else if ( id == 330 ){ description = "Resurrects another or summons a magical item to resurrect yourself at a later time."; }
			else if ( id == 331 )
			{
				description = "Summons a lava spirit that attacks a target based off its intelligence and proximity. It disappears after a set amount of time and requires 2 control slots.";
				if ( elm == "air" ){ description = "Summons a cloud spirit that attacks a target based off its intelligence and proximity. It disappears after a set amount of time and requires 2 control slots."; }
				else if ( elm == "earth" ){ description = "Summons a earth spirit that attacks a target based off its intelligence and proximity. It disappears after a set amount of time and requires 2 control slots."; }
				else if ( elm == "water" ){ description = "Summons a water spirit that attacks a target based off its intelligence and proximity. It disappears after a set amount of time and requires 2 control slots."; }
			}

			return description;
		}

		public static int ArmorCheck( Item item, int val )
		{
			int num = 0;

			if ( item is BaseArmor && ((BaseArmor)item).ArmorAttributes.MageArmor == 0 && ((BaseArmor)item).Attributes.SpellChanneling == 0 )
			{
				if ( CraftResources.GetType( item.Resource ) == CraftResourceType.Leather ){ num = 2 * val; }
				else if ( CraftResources.GetType( item.Resource ) == CraftResourceType.Skin ){ num = 2 * val; }
				else if ( CraftResources.GetType( item.Resource ) == CraftResourceType.Wood ){ num = 4 * val; }
				else if ( CraftResources.GetType( item.Resource ) == CraftResourceType.Scales ){ num = 4 * val; }
				else if ( CraftResources.GetType( item.Resource ) == CraftResourceType.Skeletal ){ num = 4 * val; }
				else if ( CraftResources.GetType( item.Resource ) == CraftResourceType.Metal ){ num = 6 * val; }
				else if ( CraftResources.GetType( item.Resource ) == CraftResourceType.Block ){ num = 6 * val; }
				else if ( CraftResources.GetType( item.Resource ) == CraftResourceType.Fabric ){ /* ADD NOTHING */ }
			}
			return num;
		}

		public static int ArmorFizzle( Mobile m )
		{
			int penalty = 0;

			if ( m.FindItemOnLayer( Layer.OuterTorso ) != null ){ penalty = penalty + ArmorCheck( m.FindItemOnLayer( Layer.OuterTorso ), 5 ); }
			if ( m.FindItemOnLayer( Layer.TwoHanded ) != null ){ penalty = penalty + ArmorCheck( m.FindItemOnLayer( Layer.TwoHanded ), 3 ); }
			if ( m.FindItemOnLayer( Layer.Helm ) != null ){ penalty = penalty + ArmorCheck( m.FindItemOnLayer( Layer.Helm ), 2 ); }
			if ( m.FindItemOnLayer( Layer.Arms ) != null ){ penalty = penalty + ArmorCheck( m.FindItemOnLayer( Layer.Arms ), 3 ); }
			if ( m.FindItemOnLayer( Layer.OuterLegs ) != null ){ penalty = penalty + ArmorCheck( m.FindItemOnLayer( Layer.OuterLegs ), 4 ); }
			if ( m.FindItemOnLayer( Layer.Neck ) != null ){ penalty = penalty + ArmorCheck( m.FindItemOnLayer( Layer.Neck ), 1 ); }
			if ( m.FindItemOnLayer( Layer.Gloves ) != null ){ penalty = penalty + ArmorCheck( m.FindItemOnLayer( Layer.Gloves ), 1 ); }
			if ( m.FindItemOnLayer( Layer.Shoes ) != null ){ penalty = penalty + ArmorCheck( m.FindItemOnLayer( Layer.Shoes ), 1 ); }
			if ( m.FindItemOnLayer( Layer.Cloak ) != null ){ penalty = penalty + ArmorCheck( m.FindItemOnLayer( Layer.Cloak ), 3 ); }
			if ( m.FindItemOnLayer( Layer.Waist ) != null ){ penalty = penalty + ArmorCheck( m.FindItemOnLayer( Layer.Waist ), 1 ); }
			if ( m.FindItemOnLayer( Layer.InnerLegs ) != null ){ penalty = penalty + ArmorCheck( m.FindItemOnLayer( Layer.InnerLegs ), 4 ); }
			if ( m.FindItemOnLayer( Layer.InnerTorso ) != null ){ penalty = penalty + ArmorCheck( m.FindItemOnLayer( Layer.InnerTorso ), 4 ); }
			if ( m.FindItemOnLayer( Layer.Pants ) != null ){ penalty = penalty + ArmorCheck( m.FindItemOnLayer( Layer.Pants ), 4 ); }
			if ( m.FindItemOnLayer( Layer.Shirt ) != null ){ penalty = penalty + ArmorCheck( m.FindItemOnLayer( Layer.Shirt ), 4 ); }

			return penalty;
		}

		public static int ScrollLook( int id, int cat )
		{
			int info = 0;
			int item = 0;
			int hue = 0;

			if ( id == 300 ){ item = 25676; hue = 0xB94; }
			else if ( id == 301 ){ item = 25677; hue = 0xB64; }
			else if ( id == 302 ){ item = 25678; hue = 0xB40; }
			else if ( id == 303 ){ item = 25679; hue = 0xAE2; }
			else if ( id == 304 ){ item = 25680; hue = 0x983; }
			else if ( id == 305 ){ item = 25677; hue = 0xB94; }
			else if ( id == 306 ){ item = 25678; hue = 0xB64; }
			else if ( id == 307 ){ item = 25679; hue = 0xB40; }
			else if ( id == 308 ){ item = 25680; hue = 0xAE2; }
			else if ( id == 309 ){ item = 25681; hue = 0x983; }
			else if ( id == 310 ){ item = 25676; hue = 0xB72; }
			else if ( id == 311 ){ item = 25678; hue = 0xB94; }
			else if ( id == 312 ){ item = 25679; hue = 0xB64; }
			else if ( id == 313 ){ item = 25680; hue = 0xB40; }
			else if ( id == 314 ){ item = 25681; hue = 0xAE2; }
			else if ( id == 315 ){ item = 25679; hue = 0xB94; }
			else if ( id == 316 ){ item = 25680; hue = 0xB64; }
			else if ( id == 317 ){ item = 25681; hue = 0xB40; }
			else if ( id == 318 ){ item = 25676; hue = 0x983; }
			else if ( id == 319 ){ item = 25677; hue = 0xB72; }
			else if ( id == 320 ){ item = 25680; hue = 0xB94; }
			else if ( id == 321 ){ item = 25681; hue = 0xB64; }
			else if ( id == 322 ){ item = 25676; hue = 0xAE2; }
			else if ( id == 323 ){ item = 25677; hue = 0x983; }
			else if ( id == 324 ){ item = 25681; hue = 0xB94; }
			else if ( id == 325 ){ item = 25676; hue = 0xB40; }
			else if ( id == 326 ){ item = 25677; hue = 0xAE2; }
			else if ( id == 327 ){ item = 25678; hue = 0x983; }
			else if ( id == 328 ){ item = 25676; hue = 0xB64; }
			else if ( id == 329 ){ item = 25677; hue = 0xB40; }
			else if ( id == 330 ){ item = 25678; hue = 0xAE2; }
			else if ( id == 331 ){ item = 25679; hue = 0x983; }

			if ( cat == 1 ){ info = item; }
			else if ( cat == 2 ){ info = hue; }

			return info;
		}

		public static int SpellIcon( int item, int spell )
		{
			if ( item == 0x6717 ){ spell = 11477 + spell - 300; }
			else if ( item == 0x6713 ){ spell = 11509 + spell - 300; }
			else if ( item == 0x6719 ){ spell = 11541 + spell - 300; }
			else if ( item == 0x6715 ){ spell = 11573 + spell - 300; }

			return spell;
		}

		public static string FontColor( int item )
		{
			string color = "";

			if ( item == 0x6717 ){ color = "#9484DE"; }			// AIR
			else if ( item == 0x6713 ){ color = "#ADE76b"; }	// EARTH
			else if ( item == 0x6719 ){ color = "#FFAD52"; }	// FIRE
			else if ( item == 0x6715 ){ color = "#189CE7"; }	// WATER

			return color;
		}

		public static void BookCover( Item item, int element )
		{
			if ( item.ArtifactLevel == ArtifactLevel.None )
			{
				if ( element == 0 ){ item.ItemID = 0x6717; }		// AIR
				else if ( element == 1 ){ item.ItemID = 0x6713; }	// EARTH
				else if ( element == 2 ){ item.ItemID = 0x6719; }	// FIRE
				else if ( element == 3 ){ item.ItemID = 0x6715; }	// WATER
			}
		}

		public static bool CanUseBook( Item item, Mobile from, bool msg )
		{
			if ( item != null && from != null )
			{
				if ( item is ElementalSpellbook && from is PlayerMobile )
				{
					int element = ((PlayerMobile)from).CharacterElement;

					if ( element != 0 && item.ItemID == 0x6717 )
					{
						if ( msg ){ from.SendMessage("Your need to focus on air elemental magic to use that!"); }
						return false;
					}
					if ( element != 1 && item.ItemID == 0x6713 )
					{
						if ( msg ){ from.SendMessage("Your need to focus on earth elemental magic to use that!"); }
						return false;
					}
					if ( element != 2 && item.ItemID == 0x6719 )
					{
						if ( msg ){ from.SendMessage("Your need to focus on fire elemental magic to use that!"); }
						return false;
					}
					if ( element != 3 && item.ItemID == 0x6715 )
					{
						if ( msg ){ from.SendMessage("Your need to focus on water elemental magic to use that!"); }
						return false;
					}
				}
			}
			return true;
		}

		public static void UnequipBook( Mobile from )
		{
			if ( from.FindItemOnLayer( Layer.Trinket ) != null )
			{
				if ( from.FindItemOnLayer( Layer.Trinket ) is ElementalSpellbook )
				{
					if ( !CanUseBook( from.FindItemOnLayer( Layer.Trinket ), from, false ) )
					{
						Container pack = from.Backpack;
						from.AddToBackpack( from.FindItemOnLayer( Layer.Trinket ) );
					}
				}
			}
		}

		public static void ChangeBooks( Mobile m, int element )
		{
			WorldUtilities
				.ForEachItem<ElementalSpellbook>( item => item.ArtifactLevel == ArtifactLevel.None && item.EllyOwner == m )
				.ToList()
				.ForEach( item => BookCover( item, element ) );
		}
	}
}
