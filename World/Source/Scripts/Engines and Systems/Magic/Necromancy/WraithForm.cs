using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Spells.Necromancy
{
	public class WraithFormSpell : TransformationSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Wraith Form", "Rel Xen Um",
				203,
				9031,
				Reagent.NoxCrystal,
				Reagent.PigIron
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.0 ); } }

		public override double RequiredSkill{ get{ return 20.0; } }
		public override int RequiredMana{ get{ return 17; } }

		public override int Body{ get{ return 84; } }
		public override int Hue{ get{ return 0; } }

		public override int PhysResistOffset{ get{ return +15; } }
		public override int FireResistOffset{ get{ return -5; } }
		public override int ColdResistOffset{ get{ return  0; } }
		public override int PoisResistOffset{ get{ return  0; } }
		public override int NrgyResistOffset{ get{ return -5; } }

		public WraithFormSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		private static int WraithLeechValue(Mobile wraith)
		{
			return 5 + (int)( ( 15 * Spell.ItemSkillValue( wraith, SkillName.Spiritualism, false ) ) / 100 ) ; // Wraith form gives 5-20% mana leech
		}

		public static void DoWraithLeech(Mobile wraith, Mobile defender, int damageGiven)
		{
			if ( wraith == null || defender == null) return;

			var manaLeech = Math.Min(defender.Mana, AOS.Scale(damageGiven, WraithLeechValue(wraith)));
			if (manaLeech < 1) return;

			wraith.Mana += manaLeech;
			defender.Mana -= manaLeech;
			wraith.PlaySound(0x44D);
		}

		public override void DoEffect( Mobile m )
		{
			if ( m is PlayerMobile )
				((PlayerMobile)m).IgnoreMobiles = true;
			
			m.PlaySound( 0x17F );
			m.FixedParticles( 0x374A, 1, 15, 9902, 1108, 4, EffectLayer.Waist );

			string args = String.Format("{0}", WraithLeechValue(m));
			BuffInfo.RemoveBuff( m, BuffIcon.WraithForm );
			BuffInfo.AddBuff( m, new BuffInfo(BuffIcon.WraithForm, 1063607, 1063608, args.ToString(), true));
		}
		
		public override void RemoveEffect( Mobile m )
		{
			BuffInfo.RemoveBuff( m, BuffIcon.WraithForm );

			if ( m is PlayerMobile && m.AccessLevel == AccessLevel.Player )
				((PlayerMobile)m).IgnoreMobiles = false;
		}
	}
}