using System;
using Server;
using Server.Items;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Misc;

namespace Server.Spells.Chivalry
{
	public class SacredJourneySpell : PaladinSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Sacred Journey", "Sanctum Viatas",
				-1,
				9002
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.5 ); } }

		public override double RequiredSkill{ get{ return 15.0; } }
		public override int RequiredMana{ get{ return 10; } }
		public override int RequiredTithing{ get{ return 15; } }
		public override int MantraNumber{ get{ return 1060727; } } // Sanctum Viatas
		public override bool BlocksMovement{ get{ return false; } }

		private RunebookEntry m_Entry;
		private Runebook m_Book;

		public SacredJourneySpell( Mobile caster, Item scroll ) : this( caster, scroll, null, null )
		{
		}

		public SacredJourneySpell( Mobile caster, Item scroll, RunebookEntry entry, Runebook book ) : base( caster, scroll, m_Info )
		{
			m_Entry = entry;
			m_Book = book;
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
			if ( !base.CheckCast() )
				return false;

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

				Effects.SendLocationParticles( EffectItem.Create( Caster.Location, Caster.Map, EffectItem.DefaultDuration ), 0, 0, 0, 5033 );

				Caster.PlaySound( 0x1FC );
				Caster.MoveToWorld( loc, map );
				Caster.PlaySound( 0x1FC );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private SacredJourneySpell m_Owner;

			public InternalTarget( SacredJourneySpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.None )
			{
				m_Owner = owner;
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