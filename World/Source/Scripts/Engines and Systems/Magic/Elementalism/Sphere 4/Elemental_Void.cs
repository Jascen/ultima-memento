using System;
using Server.Items;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Regions;
using Server.Spells.Necromancy;
using Server.Misc;

namespace Server.Spells.Elementalism
{
	public class Elemental_Void_Spell : ElementalSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Elemental Void", "Mutare",
				239,
				9031
			);

		public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

		private RunebookEntry m_Entry;
		private Runebook m_Book;

		public Elemental_Void_Spell( Mobile caster, Item scroll ) : this( caster, scroll, null, null )
		{
		}

		public Elemental_Void_Spell( Mobile caster, Item scroll, RunebookEntry entry, Runebook book ) : base( caster, scroll, m_Info )
		{
			m_Entry = entry;
			m_Book = book;
		}

		public override void GetCastSkills( out double min, out double max )
		{
			if ( TransformationSpellHelper.UnderTransformation( Caster, typeof( WraithFormSpell ) ) )
				min = max = 0;
			else if( Core.SE && m_Book != null )	//recall using Runebook charge
				min = max = 0;
			else
				base.GetCastSkills( out min, out max );
		}

		public override void OnCast()
		{
			if ( m_Entry == null )
				Caster.Target = new InternalTarget( this );
			else
				Effect( m_Entry.Location, m_Entry.Map, true );
		}

		public override bool CheckCast()
		{
			if ( Server.Misc.WeightOverloading.IsOverloaded( Caster ) )
			{
				Caster.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
				return false;
			}

			return SpellHelper.CheckTravel( Caster, TravelCheckType.RecallFrom );
		}

		public void Effect( Point3D loc, Map map, bool checkMulti )
		{
			if ( !SpellHelper.CheckTravel( Caster, TravelCheckType.RecallFrom ) )
			{
			}
			else if ( Worlds.AllowEscape( Caster, Caster.Map, Caster.Location, Caster.X, Caster.Y ) == false )
			{
				Caster.SendMessage( "That spell does not seem to work in this place." );
			}
			else if ( Worlds.RegionAllowedRecall( Caster.Map, Caster.Location, Caster.X, Caster.Y ) == false )
			{
				Caster.SendMessage( "That spell does not seem to work in this place." );
			}
			else if ( Worlds.RegionAllowedTeleport( map, loc, loc.X, loc.Y ) == false )
			{
				Caster.SendMessage( "The destination seems magically unreachable." );
			}
			else if ( !SpellHelper.CheckTravel( Caster, map, loc, TravelCheckType.RecallTo ) )
			{
			}
			else if ( Server.Misc.WeightOverloading.IsOverloaded( Caster ) )
			{
				Caster.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
			}
			else if ( !map.CanSpawnMobile( loc.X, loc.Y, loc.Z, false ) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
			else if ( (checkMulti && SpellHelper.CheckMulti( loc, map )) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
			else if ( m_Book != null && m_Book.CurCharges <= 0 )
			{
				Caster.SendLocalizedMessage( 502412 ); // There are no charges left on that item.
			}
			else if ( CheckSequence() )
			{
				BaseCreature.TeleportPets( Caster, loc, map, false );

				if ( m_Book != null )
					--m_Book.CurCharges;

				string elm = ElementalSpell.GetElement( Caster );
				int hue = 0;

				if ( elm == "air" )
					hue = 0xB42;
				else if ( elm == "earth" )
					hue = 0xB51;
				else if ( elm == "fire" )
					hue = 0xB17;
				else if ( elm == "water" )
					hue = 0;

				Caster.PlaySound( 0x658 );

				Item gate = new ElementalEffect( 0x3D5E, 5.0, null );
				gate.Name = "magic void";
				gate.Hue = hue;
				gate.Movable = false;
				gate.Light = LightType.Circle300;
				gate.MoveToWorld( Caster.Location, Caster.Map );

				Caster.MoveToWorld( loc, map );

				Caster.PlaySound( 0x658 );

				Item portal = new ElementalEffect( 0x3D5E, 5.0, null );
				portal.Name = "magic void";
				portal.Hue = hue;
				portal.Movable = false;
				portal.Light = LightType.Circle300;
				portal.MoveToWorld( loc, map );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private Elemental_Void_Spell m_Owner;

			public InternalTarget( Elemental_Void_Spell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.None )
			{
				m_Owner = owner;

				owner.Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 501029 ); // Select Marked item.
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is RecallRune )
				{
					RecallRune rune = (RecallRune)o;

					if ( rune.Marked )
						m_Owner.Effect( rune.Target, rune.TargetMap, true );
					else
						from.SendLocalizedMessage( 501805 ); // That rune is not yet marked.
				}
				else if ( o is Runebook )
				{
					RunebookEntry e = ((Runebook)o).Default;

					if ( e != null )
						m_Owner.Effect( e.Location, e.Map, true );
					else
						from.SendLocalizedMessage( 502354 ); // Target is not marked.
				}
				else if ( o is Key && ((Key)o).KeyValue != 0 && ((Key)o).Link is BaseBoat )
				{
					BaseBoat boat = ((Key)o).Link as BaseBoat;

					if ( !boat.Deleted && boat.CheckKey( ((Key)o).KeyValue ) )
						m_Owner.Effect( boat.GetMarkedLocation(), boat.Map, false );
					else
						from.Send( new MessageLocalized( from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "" ) ); // I can not recall from that object.
				}
				else
				{
					from.Send( new MessageLocalized( from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "" ) ); // I can not recall from that object.
				}
			}
			
			protected override void OnNonlocalTarget( Mobile from, object o )
			{
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}