using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Items;
using Server.Misc;

namespace Server.Spells.Fifth
{
	public class MagicReflectSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Magic Reflection", "In Jux Sanct",
				242,
				9012,
				Reagent.Garlic,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public MagicReflectSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			DefensiveSpell.EndDefense( Caster );

			if ( !base.CheckCast() )
				return false;

			if ( Caster.MagicDamageAbsorb > 0 )
			{
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
				return false;
			}
			else if ( Caster.Backpack.FindItemByType( typeof ( Diamond ) ) == null )
			{
				Caster.SendMessage( "You need a diamond to cast this spell!" );
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			DefensiveSpell.EndDefense( Caster );

			if ( Caster.MagicDamageAbsorb > 0 )
			{
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
			}
			else if ( Caster.Backpack.FindItemByType( typeof ( Diamond ) ) == null )
			{
				Caster.SendMessage( "You need a diamond to cast this spell!" );
			}
			else if ( CheckSequence() )
			{
				if ( Caster.BeginAction( typeof( DefensiveSpell ) ) )
				{
					int value = (int)( ( Spell.ItemSkillValue( Caster, SkillName.Magery, false ) + Spell.ItemSkillValue( Caster, SkillName.Psychology, false ) ) / 4 );
					Caster.MagicDamageAbsorb = value;
					Item diamond = Caster.Backpack.FindItemByType( typeof ( Diamond ) );
					if ( diamond != null ){ diamond.Consume(); }

					BuffInfo.RemoveBuff( Caster, BuffIcon.MagicReflection );
					BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.MagicReflection, 1063617 ) );

					Caster.PlaySound( 0x1ED );
					Caster.FixedParticles( 0x375A, 10, 15, 5037, PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.Waist );
				}
				else
				{
					Caster.SendLocalizedMessage( 1005385 ); // The spell will not adhere to you at this time.
				}
			}

			FinishSequence();
		}
	}
}