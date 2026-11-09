using System;
using System.Collections;
using Server.Targeting;

namespace Server.Spells.Research
{
	public class ResearchIntervention : ResearchSpell
	{
		public int CirclePower = 6;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 634,
					IconGraphic = 11248,
					Name = "Intervention",
					PowerWords = "In Sanct An Jux",
					Description = "This is one of those rare spells that call upon the favor of the gods. When the spell is invoked, the caster is empowered with 10-50 additional points in their resistances. They are also protected from magery spell attack damage for 25-125 points. The effects last between 10-50 minutes.",
					ManaCost = 25,
					MinSkill = 50,
					TargetType = TargetFlags.None
				},
				242,
				9012,
				Reagent.MoonCrystal,Reagent.Brimstone,Reagent.Nightshade,Reagent.FairyEgg
			);

		public ResearchIntervention( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
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
					Caster.PlaySound( 0x5C9 );
					Point3D wings = new Point3D( Caster.X+1, Caster.Y+1, Caster.Z+18 );
					Effects.SendLocationEffect( wings, Caster.Map, 0x3FE5, 30, 10, 0, 0 );

					int modify = (int)( DamagingSkill( Caster ) / 5 );

					mods = new ResistanceMod[5]
						{
							new ResistanceMod( ResistanceType.Physical, +modify ),
							new ResistanceMod( ResistanceType.Fire, +modify ),
							new ResistanceMod( ResistanceType.Cold, +modify ),
							new ResistanceMod( ResistanceType.Poison, +modify ),
							new ResistanceMod( ResistanceType.Energy, +modify )
						};

					m_Table[targ] = mods;

					for ( int i = 0; i < mods.Length; ++i )
						targ.AddResistanceMod( mods[i] );

					int value = (int)( DamagingSkill( Caster ) / 2 );
					Caster.MagicDamageAbsorb = value;

					int TotalTime = (int)( DamagingSkill( Caster ) / 12 );
					new InternalTimer( Caster, TimeSpan.FromMinutes( TotalTime ) ).Start();
					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );

					string args = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", modify, modify, modify, modify, modify);

					BuffInfo.RemoveBuff( Caster, BuffIcon.Intervention );
					BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.Intervention, 1063660, 1063661, TimeSpan.FromMinutes( TotalTime ), Caster, args.ToString(), true));
				}
				else
				{
					DoFizzle();
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

				m.MagicDamageAbsorb = 0;
				Fifth.MagicReflectSpell.RemoveReflect( m );
				m_Table.Remove( m );
				m.PlaySound( 0x1F8 );

				BuffInfo.RemoveBuff( m, BuffIcon.Intervention );
			}
		}

		private class InternalTimer : Timer
		{
			private Mobile m_m;
			private DateTime m_Expire;

			public InternalTimer( Mobile Caster, TimeSpan duration ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 0.1 ) )
			{
				m_m = Caster;
				m_Expire = DateTime.Now + duration;
			}

			protected override void OnTick()
			{
				if ( DateTime.Now >= m_Expire )
				{
					EndArmor( m_m );
					Stop();
				}
			}
		}
	}
}