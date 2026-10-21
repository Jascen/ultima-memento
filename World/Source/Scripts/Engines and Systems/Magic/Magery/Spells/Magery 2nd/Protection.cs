using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Misc;

namespace Server.Spells.Second
{
	public class ProtectionSpell : MagerySpell
	{
		private static Hashtable m_Registry = new Hashtable();
		public static Hashtable Registry { get { return m_Registry; } }

		private static SpellInfo m_Info = new SpellInfo(
				"Protection", "Uus Sanct",
				236,
				9011,
				Reagent.Garlic,
				Reagent.Ginseng,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public ProtectionSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( Core.AOS )
				return true;

			if ( m_Registry.ContainsKey( Caster ) )
			{
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
				return false;
			}
			else if ( !Caster.CanBeginAction( typeof( DefensiveSpell ) ) )
			{
				Caster.SendLocalizedMessage( 1005385 ); // The spell will not adhere to you at this time.
				return false;
			}

			return true;
		}

		private static Hashtable m_Table = new Hashtable();

		public static void Toggle( Mobile caster, Mobile target )
		{
			/* Players under the protection spell effect can no longer have their spells "disrupted" when hit.
			 * Players under the protection spell have decreased physical resistance stat value (-15 + (Inscription/20),
			 * a decreased "magic resistance" skill value by -35 + (Inscription/20),
			 * and a slower casting speed modifier (technically, a negative "faster cast speed") of 2 points.
			 * The protection spell has an indefinite duration, becoming active when cast, and deactivated when re-cast.
			 * Reactive Armor, Protection, and Magic Reflection will stay on—even after logging out,
			 * even after dying—until you “turn them off” by casting them again.
			 */

			object[] mods = (object[])m_Table[target];

			if ( mods == null )
			{
				Server.Spells.Elementalism.Elemental_Protection_Spell.EndProtection( target );

				target.PlaySound( 0x1E9 );
				target.FixedParticles( 0x375A, 9, 20, 5016, PlayerSettings.GetMySpellHue( true, caster, 0 ), 0, EffectLayer.Waist );

				mods = new object[2]
					{
						new ResistanceMod( ResistanceType.Physical, -15 + Math.Min( (int)(caster.Skills[SkillName.Inscribe].Value / 20), 15 ) ),
						new DefaultSkillMod( SkillName.MagicResist, true, -35 + Math.Min( (int)(caster.Skills[SkillName.Inscribe].Value / 20), 35 ) )
					};

				m_Table[target] = mods;
				Registry[target] = 100.0;

				target.AddResistanceMod( (ResistanceMod)mods[0] );
				target.AddSkillMod( (SkillMod)mods[1] );

				int physloss = -15 + (int) (caster.Skills[SkillName.Inscribe].Value / 20);
				int resistloss = -35 + (int) (caster.Skills[SkillName.Inscribe].Value / 20);
				string args = String.Format("{0}\t{1}", physloss, resistloss);
				BuffInfo.AddBuff(target, new BuffInfo(BuffIcon.Protection, 1075814, 1075815, args.ToString(),true));
			}
			else
			{
				target.PlaySound( 0x1ED );
				target.FixedParticles( 0x375A, 9, 20, 5016, PlayerSettings.GetMySpellHue( true, caster, 0 ), 0, EffectLayer.Waist );

				m_Table.Remove( target );
				Registry.Remove( target );

				target.RemoveResistanceMod( (ResistanceMod)mods[0] );
				target.RemoveSkillMod( (SkillMod)mods[1] );

				BuffInfo.RemoveBuff(target, BuffIcon.Protection);
			}
		}

		public static void EndProtection( Mobile m )
		{
			if ( m_Table.Contains( m ) )
			{
				object[] mods = (object[]) m_Table[ m ];

				m_Table.Remove( m );
				Registry.Remove( m );

				m.RemoveResistanceMod( (ResistanceMod) mods[ 0 ] );
				m.RemoveSkillMod( (SkillMod) mods[ 1 ] );

				BuffInfo.RemoveBuff( m, BuffIcon.Protection );
			}
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
				Toggle( Caster, Caster );

			FinishSequence();
		}
	}
}
