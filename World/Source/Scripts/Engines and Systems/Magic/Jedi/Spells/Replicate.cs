using System;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Jedi
{
	public class Replicate : JediSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 289,
					IconGraphic = 0x2BD2,
					Name = "Replicate",
					PowerWords = "Leantre",
					Description = "This power allows the Jedi to create a replication crystal that can hold the Jedi's genetic pattern. If the Jedi meets an untimely end, the crystal will activate in 30 seconds and create a replicant that the Jedi's soul can then occupy. These crystal are quite fragile so you would need to make sure the crystal did not crumble while you were resting for long periods of time.",
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

		public Replicate( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				var m = Caster;
				if ( SoulOrb.Create( m, SoulOrbType.CloningCrystalJedi ) != null )
				{
					m.PlaySound( 0x244 );
					Effects.SendLocationEffect(m.Location, m.Map, 0x373A, 15, 0, 0);
					m.SendMessage( "You create a replication crystal with your genetic pattern." );
				}
			}

			FinishSequence();
		}
	}
}