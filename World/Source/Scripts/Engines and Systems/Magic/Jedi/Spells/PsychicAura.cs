using System;
using System.Collections;
using Server.Targeting;

namespace Server.Spells.Jedi
{
    public class PsychicAura : JediSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 285,
					IconGraphic = 0x2BE5,
					Name = "Psychic Aura",
					PowerWords = "Wilems",
					Description = "The Jedi can create an aura around them that will protect them better from physical and energy damage, but also makes them weaker against cold, fire, and poison damage. A Jedi will have to use to power again in order to remove these effects.",
					ManaCost = 24,
					TithingCost = 32,
					MinSkill = 20,
					TargetType = TargetFlags.None
				},
				236,
				0
			);

		public int CirclePower = 1;
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

        public PsychicAura(Mobile caster, Item scroll) : base(caster, scroll, SpellInfo)
        {
        }

		private static Hashtable m_Table = new Hashtable();

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				Mobile targ = Caster;

				ResistanceMod[] mods = (ResistanceMod[])m_Table[targ];

				if ( mods == null )
				{
					targ.PlaySound( 0x1E9 );
					targ.FixedParticles( 0x376A, 9, 32, 5008, 0xB41, 0, EffectLayer.Waist );

					int phys = (int)( (targ.Skills[SkillName.Inscribe].Value / 15) + (GetJediDamage(Caster) / 50) );
					int engy = (int)( (targ.Skills[SkillName.Inscribe].Value / 25) + (GetJediDamage(Caster) / 75) );

					mods = new ResistanceMod[5]
						{
							new ResistanceMod( ResistanceType.Physical, phys ),
							new ResistanceMod( ResistanceType.Fire, -5 ),
							new ResistanceMod( ResistanceType.Cold, -5 ),
							new ResistanceMod( ResistanceType.Poison, -5 ),
							new ResistanceMod( ResistanceType.Energy, engy )
						};

					m_Table[targ] = mods;

					for ( int i = 0; i < mods.Length; ++i )
						targ.AddResistanceMod( mods[i] );

					string args = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", phys, -5, -5, -5, engy);

					BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.PsychicAura, 1063526, 1075818, args.ToString(), true));
				}
				else
				{
					targ.PlaySound( 0x1ED );
					targ.FixedParticles( 0x376A, 9, 32, 5008, 0xB41, 0, EffectLayer.Waist );

					m_Table.Remove( targ );

					for ( int i = 0; i < mods.Length; ++i )
						targ.RemoveResistanceMod( mods[i] );

					BuffInfo.RemoveBuff(Caster, BuffIcon.PsychicAura);
				}
			}

			FinishSequence();
		}

		public static void EndArmor( Mobile m )
		{
			if ( m_Table.Contains( m ) )
			{
				ResistanceMod[] mods = (ResistanceMod[]) m_Table[ m ];

				if ( mods != null )
				{
					for ( int i = 0; i < mods.Length; ++i )
						m.RemoveResistanceMod( mods[ i ] );
				}

				m_Table.Remove( m );
			}
		}
	}
}