using System;
using Server.Items;
using Server.Utilities;

namespace Server.Spells.Mystic
{
	public class CreateRobe : MysticSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Create Robe", "Ra Beh Cah Summ",
				269,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3 ); } }
		public override int RequiredTithing{ get{ return 150; } }
		public override double RequiredSkill{ get{ return 25.0; } }
		public override int RequiredMana{ get{ return 20; } }
		public override int MysticSpellCircle{ get{ return 1; } }

		public CreateRobe( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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