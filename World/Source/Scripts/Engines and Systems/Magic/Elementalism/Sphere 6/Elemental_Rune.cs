using System;
using Server.Items;
using Server.Targeting;
using Server.Network;
using Server.Regions;
using Server.Misc;

namespace Server.Spells.Elementalism
{
	public class Elemental_Rune_Spell : ElementalSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Elemental Rune", "Marca",
				218,
				9002
			);

		public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

		public Elemental_Rune_Spell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			return SpellHelper.CheckTravel( Caster, TravelCheckType.Mark );
		}

		public void Target( RecallRune rune )
		{
			Region reg = Region.Find( Caster.Location, Caster.Map );
					
			if ( !Caster.CanSee( rune ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( Worlds.IsRoughWaters( reg ) )
			{
				Caster.SendMessage( "These waters are too rough to cast this spell." );
			}
			else if ( Worlds.RegionAllowedTeleport( Caster.Map, Caster.Location, Caster.X, Caster.Y ) == false )
			{
				Caster.SendMessage( "That spell does not seem to work in this place." );
			}
			else if ( !SpellHelper.CheckTravel( Caster, TravelCheckType.Mark ) )
			{
			}
			else if ( SpellHelper.CheckMulti( Caster.Location, Caster.Map, !Core.AOS ) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
			else if ( !rune.IsChildOf( Caster.Backpack ) )
			{
				Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1062422 ); // You must have this rune in your backpack in order to mark it.
			}
			else if ( CheckSequence() )
			{
				rune.Mark( Caster );

				string elm = ElementalSpell.GetElement( Caster );
				Point3D loc = new Point3D( Caster.X+1, Caster.Y+1, Caster.Z+10 );

				if ( elm == "air" )
				{
					Caster.BoltEffect( 0 );
				}
				else if ( elm == "earth" )
				{
					Caster.PlaySound( 0x64D );
					Effects.SendLocationEffect( loc, Caster.Map, 0x54F4, 16, 0, 0 );
				}
				else if ( elm == "fire" )
				{
					Caster.PlaySound( 0x1DD );
					Effects.SendLocationEffect( loc, Caster.Map, 0x5536, 30, 10, 0xB71-1, 0 );
				}
				else if ( elm == "water" )
				{
					Caster.PlaySound( 0x026 );
					Effects.SendLocationEffect( loc, Caster.Map, 0x5558, 30, 10, 0, 0 );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private Elemental_Rune_Spell m_Owner;

			public InternalTarget( Elemental_Rune_Spell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is RecallRune )
				{
					m_Owner.Target( (RecallRune) o );
				}
				else
				{
					from.Send( new MessageLocalized( from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 501797, from.Name, "" ) ); // I cannot mark that object.
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}