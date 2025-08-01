using System;
using System.Collections;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Seventh;

namespace Server.Spells.HolyMan
{
	public class SanctifySpell : HolyManSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Sanctify", "Benedicite",
				266,
				9040
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3 ); } }
		public override int RequiredTithing{ get{ return 60; } }
		public override double RequiredSkill{ get{ return 30.0; } }
		public override int RequiredMana{ get{ return 15; } }

		private static Hashtable m_Table = new Hashtable();

		public SanctifySpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public static bool HasEffect( Mobile m )
		{
			return ( m_Table[m] != null );
		}

		public static void RemoveEffect( Mobile m )
		{
			object[] mods = (object[])m_Table[m];

			if ( mods != null )
			{
				m.RemoveStatMod( ((StatMod)mods[0]).Name );
				m.RemoveStatMod( ((StatMod)mods[1]).Name );
				m.RemoveStatMod( ((StatMod)mods[2]).Name );
				m.RemoveSkillMod( (SkillMod)mods[3] );
				m.RemoveSkillMod( (SkillMod)mods[4] );
				m.RemoveSkillMod( (SkillMod)mods[5] );
			}

			m_Table.Remove( m );

			BuffInfo.RemoveBuff( m, BuffIcon.Sanctify );

			m.EndAction( typeof( SanctifySpell ) );
		}

		public override bool CheckCast()
		{
            if (!TransformationSpellHelper.CheckCast(Caster, this))
                return false;

            return base.CheckCast();
		}

		public override void OnCast()
		{
            TransformationSpellHelper.OnCast(Caster, this);

            if ( CheckSequence() )
			{
                RemoveEffect(Caster);

				int modify = (int)( (Caster.Skills[SkillName.Healing].Value / 25) + (Caster.Skills[SkillName.Spiritualism].Value / 25) );

				modify = MyServerSettings.PlayerLevelMod( modify, Caster );

				object[] mods = new object[]
				{
					new StatMod( StatType.Str, "[] Str Offset", modify, TimeSpan.Zero ),
					new StatMod( StatType.Dex, "[] Dex Offset", modify, TimeSpan.Zero ),
					new StatMod( StatType.Int, "[] Int Offset", modify, TimeSpan.Zero ),
					new DefaultSkillMod( SkillName.Parry, true, modify ),
					new DefaultSkillMod( SkillName.Tactics, true, modify ),
					new DefaultSkillMod( SkillName.Anatomy, true, modify )
				};

				m_Table[Caster] = mods;

				Caster.AddStatMod( (StatMod)mods[0] );
				Caster.AddStatMod( (StatMod)mods[1] );
				Caster.AddStatMod( (StatMod)mods[2] );
				Caster.AddSkillMod( (SkillMod)mods[3] );
				Caster.AddSkillMod( (SkillMod)mods[4] );
				Caster.AddSkillMod( (SkillMod)mods[5] );

				double span = ( Caster.Skills[SkillName.Healing].Value + (Caster.Skills[SkillName.Spiritualism].Value / 2) );
				new InternalTimer( Caster, TimeSpan.FromSeconds( (int)span ) ).Start();

				string args = String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", modify, modify, modify, modify, modify, modify);

				BuffInfo.RemoveBuff( Caster, BuffIcon.Sanctify );
				BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.Sanctify, 1063536, 1063537, TimeSpan.FromSeconds( (int)span ), Caster, args.ToString(), true));

				Caster.BeginAction( typeof( SanctifySpell ) );
				Caster.PlaySound( 0x5C9 );
				Point3D wings = new Point3D( Caster.X+1, Caster.Y+1, Caster.Z+18 );
				Effects.SendLocationEffect( wings, Caster.Map, 0x3FE5, 30, 10, 0, 0 );
				DrainSoulsInSymbol( Caster, RequiredTithing );
			}

            FinishSequence();
		}

		private class InternalTimer : Timer
		{
			private Mobile m_Owner;
			private DateTime m_Expire;

			public InternalTimer( Mobile owner, TimeSpan duration ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 0.1 ) )
			{
				m_Owner = owner;
				m_Expire = DateTime.Now + duration;
			}

			protected override void OnTick()
			{
				if ( DateTime.Now >= m_Expire )
				{
					BuffInfo.RemoveBuff( m_Owner, BuffIcon.Sanctify );
					SanctifySpell.RemoveEffect( m_Owner );
					Stop();
				}
			}
		}
	}
}
