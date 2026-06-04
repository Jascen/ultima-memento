using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Items;

namespace Server.Spells.Elementalism
{
	public class Elemental_Echo_Spell : ElementalSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Elemental Echo", "Oglinda",
				242,
				9012
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public Elemental_Echo_Spell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			DefensiveSpell.EndDefense( Caster );

			string elm = ElementalSpell.GetElement( Caster );

			if ( !base.CheckCast() )
				return false;

			if ( Caster.MagicDamageAbsorb > 0 )
			{
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
				return false;
			}
			else if ( Caster.Backpack.FindItemByType( typeof ( Amethyst ) ) == null && elm == "air" )
			{
				Caster.SendMessage( "You need an amethyst to cast this spell!" );
				return false;
			}
			else if ( Caster.Backpack.FindItemByType( typeof ( Emerald ) ) == null && elm == "earth" )
			{
				Caster.SendMessage( "You need an emerald to cast this spell!" );
				return false;
			}
			else if ( Caster.Backpack.FindItemByType( typeof ( Ruby ) ) == null && elm == "fire" )
			{
				Caster.SendMessage( "You need a ruby to cast this spell!" );
				return false;
			}
			else if ( Caster.Backpack.FindItemByType( typeof ( Sapphire ) ) == null && elm == "water" )
			{
				Caster.SendMessage( "You need a sapphire to cast this spell!" );
				return false;
			}

			return true;
		}

		private static Hashtable m_Table = new Hashtable();

		public override void OnCast()
		{
			DefensiveSpell.EndDefense( Caster );

			string elm = ElementalSpell.GetElement( Caster );

			if ( Caster.MagicDamageAbsorb > 0 )
			{
				Caster.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
			}
			else if ( Caster.Backpack.FindItemByType( typeof ( Amethyst ) ) == null && elm == "air" )
			{
				Caster.SendMessage( "You need an amethyst to cast this spell!" );
			}
			else if ( Caster.Backpack.FindItemByType( typeof ( Emerald ) ) == null && elm == "earth" )
			{
				Caster.SendMessage( "You need an emerald to cast this spell!" );
			}
			else if ( Caster.Backpack.FindItemByType( typeof ( Ruby ) ) == null && elm == "fire" )
			{
				Caster.SendMessage( "You need a ruby to cast this spell!" );
			}
			else if ( Caster.Backpack.FindItemByType( typeof ( Sapphire ) ) == null && elm == "water" )
			{
				Caster.SendMessage( "You need a sapphire to cast this spell!" );
			}
			else if ( CheckSequence() )
			{
				int value = (int)( ( Caster.Skills[CastSkill].Value * 2 ) / 4 );
				Caster.MagicDamageAbsorb = value;
				Fifth.MagicReflectSpell.AddReflect( Caster );
				int hue = 0;
				Item rock = null;

				if ( elm == "air" )
				{
					rock = Caster.Backpack.FindItemByType( typeof ( Amethyst ) );
					hue = 0xBB4;
				}
				else if ( elm == "earth" )
				{
					rock = Caster.Backpack.FindItemByType( typeof ( Emerald ) );
					hue = 0xB44;
				}
				else if ( elm == "fire" )
				{
					rock = Caster.Backpack.FindItemByType( typeof ( Ruby ) );
					hue = 0;
				}
				else if ( elm == "water" )
				{
					rock = Caster.Backpack.FindItemByType( typeof ( Sapphire ) );
					hue = 0xB3F;
				}

				if ( rock != null ){ rock.Consume(); }

				Point3D loc1 = new Point3D( Caster.X, Caster.Y, Caster.Z+10 );
				Point3D loc2 = new Point3D( Caster.X-1, Caster.Y, Caster.Z+5 );
				Point3D loc3 = new Point3D( Caster.X, Caster.Y-1, Caster.Z+5 );
				Point3D loc4 = new Point3D( Caster.X+1, Caster.Y+1, Caster.Z+15 );

				Effects.SendLocationEffect( loc1, Caster.Map, 0x5469, 30, 10, hue, 0 );
				Effects.SendLocationEffect( loc2, Caster.Map, 0x5469, 30, 10, hue, 0 );
				Effects.SendLocationEffect( loc3, Caster.Map, 0x5469, 30, 10, hue, 0 );
				Effects.SendLocationEffect( loc4, Caster.Map, 0x5469, 30, 10, hue, 0 );

				Caster.PlaySound( 0x5C9 );

				BuffInfo.RemoveBuff( Caster, BuffIcon.ElementalEcho );
				BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.ElementalEcho, 1063630 ) );
			}

			FinishSequence();
		}
	}
}