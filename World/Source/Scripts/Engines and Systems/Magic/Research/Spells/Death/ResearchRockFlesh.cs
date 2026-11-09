using System;
using Server.Targeting;
using System.Collections;
using Server.Items;

namespace Server.Spells.Research
{
	public class ResearchRockFlesh : ResearchSpell
	{
		public int CirclePower = 5;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 644,
					IconGraphic = 11204,
					Name = "Rock Flesh",
					PowerWords = "Rel Sanct Ylem",
					Description = "With this spell, the caster gains an innate resistance to damage by transforming the very flesh into a substance as strong as stone for 1-16 minutes of a 90% physical resistance. The caster may lose some karma if they attempt this.",
					ManaCost = 10,
					MinSkill = 15,
					TargetType = TargetFlags.None
				},
				236,
				9011,
				Reagent.MoonCrystal,Reagent.Garlic,Reagent.PigIron,Reagent.BlackPearl
			);

		public ResearchRockFlesh( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public static Hashtable TableStoneFlesh = new Hashtable();

		public static bool HasEffect( Mobile m )
		{
			return ( TableStoneFlesh[m] != null );
		}

		public static bool UnderEffect( Mobile m )
		{
			return TableStoneFlesh.Contains( m );
		}

		public static void RemoveEffect( Mobile m )
		{
			m.HueMod = -1;
			m.BodyMod = 0;
			m.RaceBody();
			m.SendMessage( "Your flesh turns back to normal." );

			BuffInfo.RemoveBuff( m, BuffIcon.RockFlesh );

			ResistanceMod[] mods = (ResistanceMod[])TableStoneFlesh[m];
			TableStoneFlesh.Remove( m );
			for ( int i = 0; i < mods.Length; ++i )
				m.RemoveResistanceMod( mods[i] );

			Point3D hands = new Point3D( ( m.X+1 ), ( m.Y+1 ), ( m.Z+8 ) );
			Effects.SendLocationParticles(EffectItem.Create(hands, m.Map, EffectItem.DefaultDuration), 0x3837, 9, 32, Server.Misc.PlayerSettings.GetMySpellHue( true, m, 0xB7F ), 0, 5022, 0);
			m.PlaySound( 0x65A );

			m.EndAction( typeof( ResearchRockFlesh ) );
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				if ( !Caster.CanBeginAction( typeof( ResearchRockFlesh ) ) )
				{
					ResearchRockFlesh.RemoveEffect( Caster );
				}

				ResistanceMod[] mods = (ResistanceMod[])TableStoneFlesh[Caster];

				mods = new ResistanceMod[1]
					{
						new ResistanceMod( ResistanceType.Physical, 90 )
					};

				TableStoneFlesh[Caster] = mods;

				for ( int i = 0; i < mods.Length; ++i )
					Caster.AddResistanceMod( mods[i] );

				double TotalTime = DamagingSkill( Caster )*4;
				new InternalTimer( Caster, TimeSpan.FromSeconds( TotalTime ) ).Start();

				Caster.BodyMod = 14;
				Caster.HueMod = 0xB31;

				BuffInfo.RemoveBuff( Caster, BuffIcon.RockFlesh );
				BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.RockFlesh, 1063652, 1063653, TimeSpan.FromSeconds( TotalTime ), Caster));

				Mobiles.IMount mt = Caster.Mount;
					if ( mt != null )
					{
						Server.Mobiles.EtherealMount.EthyDismount( Caster );
						mt.Rider = null;
					}

				Caster.SendMessage( "Your flesh turns to stone." );

				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );

				KarmaMod( Caster, ((int)RequiredSkill+RequiredMana) );

				Point3D hands = new Point3D( ( Caster.X+1 ), ( Caster.Y+1 ), ( Caster.Z+8 ) );
				Effects.SendLocationParticles(EffectItem.Create(hands, Caster.Map, EffectItem.DefaultDuration), 0x3837, 9, 32, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0xB7F ), 0, 5022, 0);
				Caster.PlaySound( 0x65A );
			}

            FinishSequence();
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
				if ( DateTime.Now >= m_Expire && m_m != null && HasEffect( m_m ) )
				{
					ResearchRockFlesh.RemoveEffect( m_m );
					Stop();
				}
			}
		}
	}
}