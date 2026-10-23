using System;
using Server.Targeting;
using Server.Items;
using Server.Utilities;

namespace Server.Spells.Mystic
{
	public class CreateRobe : MysticSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 252,
					IconGraphic = 0x15,
					Name = "Create Robe",
					PowerWords = "Ra Beh Cah Summ",
					Description = "Creates a robe that you will need in order to use the other abilities in this tome. The robe will have power based on your overall skill as a monk, and no one else may wear the robe. You can only have one such robe at a time, so creating a new robe will cause any others you own to go back to the astral plane. After creation, single click the robe and select the 'Enchant' option to spend the points on attributes you want the robe to have.",
					ManaCost = 20,
					TithingCost = 150,
					MinSkill = 25,
					TargetType = TargetFlags.None
				},
				269,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3 ); } }
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }
		public override int MysticSpellCircle{ get{ return 1; } }

		public CreateRobe( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				foreach ( Item item in WorldUtilities.ForEachItem<MysticMonkRobe>( item => item.Owner == Caster ) )
				{
					item.Delete();
				}

				MysticMonkRobe robe = new MysticMonkRobe( 2422 );
				robe.Owner = Caster;
				robe.Gifter = "Mystical Monk's Robe";
				robe.How = "Belongs to";
				robe.Points = Math.Max(0, (int)(Caster.Skills[SkillName.FistFighting].Value * 2) - 50);
				Caster.AddToBackpack( robe );

				Caster.FixedParticles( 0x376A, 1, 62, 9923, 3, 3, EffectLayer.Waist );
				Caster.PlaySound( 0x5C9 );
			}

			FinishSequence();
		}
	}
}