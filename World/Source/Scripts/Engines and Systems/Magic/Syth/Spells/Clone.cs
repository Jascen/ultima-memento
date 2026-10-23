using System;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Syth
{
	public class CloneBody : SythSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 279,
					IconGraphic = 0x08D5,
					Name = "Clone",
					PowerWords = "Itsu Sutta",
					Description = "This power allows the Syth to create a cloning crystal that can hold the Syth's genetic pattern. If the Syth meets an untimely end, the crystal will activate in 30 seconds and create a clone body that the Syth's soul can then occupy. These crystal are quite fragile so you would need to make sure the crystal did not crumble while you were resting for long periods of time.",
					ManaCost = 40,
					TithingCost = 250,
					MinSkill = 100,
					TargetType = TargetFlags.None
				},
				203,
				0
			);

		public int CirclePower = 8;
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public CloneBody( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				var m = Caster;
				if ( SoulOrb.Create( m, SoulOrbType.CloningCrystalSyth ) != null )
				{
					m.PlaySound( 0x243 );
					Effects.SendLocationEffect(m.Location, m.Map, 0x373A, 15, 0x81F, 0);
					m.SendMessage( "You create a cloning crystal with your genetic pattern." );
				}
			}

            FinishSequence();
		}
	}
}