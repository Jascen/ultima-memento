using System;
using System.Collections;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Elementalism
{
	public class Elemental_Protection_Spell : ElementalSpell
	{
		private static Hashtable m_Registry = new Hashtable();
		public static Hashtable Registry { get { return m_Registry; } }

		private static SpellInfo m_Info = new SpellInfo(
				"Elemental Protection", "Proteja",
				236,
				9011
			);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public Elemental_Protection_Spell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
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
			 * Reactive Armor, Protection, and Magic Reflection will stay on�even after logging out,
			 * even after dying�until you �turn them off� by casting them again.
			 */

			object[] mods = (object[])m_Table[target];

			string elm = ElementalSpell.GetElement( caster );
			int hue = 0;

			if ( elm == "air" )
				hue = 0x9A3;
			else if ( elm == "earth" )
				hue = 0xACC;
			else if ( elm == "fire" )
				hue = 0x9A1;
			else if ( elm == "water" )
				hue = 0xB40;

			if ( mods == null )
			{
				Server.Spells.Second.ProtectionSpell.EndProtection( target );

				target.PlaySound( 0x1E9 );
				target.FixedParticles( 0x373A, 9, 20, 5016, hue-1, 0, EffectLayer.Waist );

				mods = new object[2]
					{
						new ResistanceMod( ResistanceType.Physical, -15 + Math.Min( (int)(caster.Skills[SkillName.Elementalism].Value / 20), 15 ) ),
						new DefaultSkillMod( SkillName.MagicResist, true, -35 + Math.Min( (int)(caster.Skills[SkillName.Elementalism].Value / 20), 35 ) )
					};

				m_Table[target] = mods;
				Registry[target] = 100.0;

				target.AddResistanceMod( (ResistanceMod)mods[0] );
				target.AddSkillMod( (SkillMod)mods[1] );

				int physloss = -15 + (int) (caster.Skills[SkillName.Elementalism].Value / 20);
				int resistloss = -35 + (int) (caster.Skills[SkillName.Elementalism].Value / 20);
				string args = String.Format("{0}\t{1}", physloss, resistloss);
				BuffInfo.AddBuff(target, new BuffInfo(BuffIcon.ElementalProtect, 1063629, 1075815, args.ToString(),true));
			}
			else
			{
				target.PlaySound( 0x1ED );
				target.FixedParticles( 0x373A, 9, 20, 5016, hue, 0, EffectLayer.Waist );

				m_Table.Remove( target );
				Registry.Remove( target );

				target.RemoveResistanceMod( (ResistanceMod)mods[0] );
				target.RemoveSkillMod( (SkillMod)mods[1] );

				BuffInfo.RemoveBuff(target, BuffIcon.ElementalProtect);
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

				BuffInfo.RemoveBuff( m, BuffIcon.ElementalProtect );
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
