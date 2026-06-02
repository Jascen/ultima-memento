using System;
using Server.Targeting;
using Server.Utilities;

namespace Server.Spells.Shinobi
{
	public class Espionage : ShinobiSpell
	{
		public override int spellIndex { get { return 293; } }
		private static SpellInfo m_Info = new SpellInfo(
				"Espionage", "Supai",
				-1,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3.0 ); } }
		public override double RequiredSkill{ get{ return (double)(Int32.Parse(  Server.Items.ShinobiScroll.ShinobiInfo( spellIndex, "skill" ))); } }
		public override int RequiredTithing{ get{ return Int32.Parse(  Server.Items.ShinobiScroll.ShinobiInfo( spellIndex, "points" )); } }
		public override int RequiredMana{ get{ return Int32.Parse(  Server.Items.ShinobiScroll.ShinobiInfo( spellIndex, "mana" )); } }

		public Espionage( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		private class InternalTarget : Target
		{
			private Espionage m_Owner;

			public InternalTarget( Espionage owner ) : base( 2, false, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D loc = o as IPoint3D;

				if ( loc == null )
					return;

				if ( m_Owner.CheckSequence() ) {
					SpellHelper.Turn( from, o );

					Effects.PlaySound( loc, from.Map, 0x241 );

					UnlockUtilities.TrySpellUnlock( from, o, UnlockUtilities.EspionageUnlockProfile );
				}

				m_Owner.FinishSequence();
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}
