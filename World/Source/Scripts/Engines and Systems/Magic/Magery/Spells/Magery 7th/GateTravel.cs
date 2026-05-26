using System;
using Server.Network;
using Server.Multis;
using Server.Items;
using Server.Targeting;
using Server.Misc;
using Server.Regions;
using Server.Mobiles;

namespace Server.Spells.Seventh
{
	public class GateTravelSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Gate Travel", "Vas Rel Por",
				263,
				9032,
				Reagent.BlackPearl,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

		private RunebookEntry m_Entry;

		public GateTravelSpell( Mobile caster, Item scroll ) : this( caster, scroll, null )
		{
		}

		public GateTravelSpell( Mobile caster, Item scroll, RunebookEntry entry ) : base( caster, scroll, m_Info )
		{
			m_Entry = entry;
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
			return SpellHelper.CheckTravel( Caster, TravelCheckType.GateFrom );
		}

		private bool GateExistsAt(Map map, Point3D loc )
		{
			bool _gateFound = false;

			IPooledEnumerable eable = map.GetItemsInRange( loc, 0 );
			foreach ( Item item in eable )
			{
				if ( item is Moongate )
				{
					_gateFound = true;
					break;
				}
			}
			eable.Free();

			return _gateFound;
		}

		public void Effect( Point3D loc, Map map, bool checkMulti )
		{
			if ( !SpellHelper.CheckTravel( Caster, TravelCheckType.GateFrom ) )
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
			else if ( !SpellHelper.CheckTravel( Caster,  map, loc, TravelCheckType.GateTo ) )
			{
			}
			else if ( !map.CanSpawnMobile( loc.X, loc.Y, loc.Z, false ) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
			else if ( (checkMulti && SpellHelper.CheckMulti( loc, map )) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
			else if ( Core.SE && ( GateExistsAt( map, loc ) || GateExistsAt( Caster.Map, Caster.Location ) ) ) // SE restricted stacking gates
			{
				Caster.SendLocalizedMessage( 1071242 ); // There is already a gate there.
			}
			else if ( CheckSequence() )
			{
				Caster.SendLocalizedMessage( 501024 ); // You open a magical gate to another location

				Effects.PlaySound( Caster.Location, Caster.Map, 0x20E );
				InternalItem firstGate = new InternalItem( loc, map );
				firstGate.Hue = Server.Misc.PlayerSettings.GetMySpellHue( false, Caster, 0 );
				firstGate.MoveToWorld( Caster.Location, Caster.Map );

				if ( Worlds.RegionAllowedTeleport( Caster.Map, Caster.Location, Caster.X, Caster.Y ) == true )
				{
					Effects.PlaySound( loc, map, 0x20E );
					InternalItem secondGate = new InternalItem( Caster.Location, Caster.Map );
					secondGate.Hue = Server.Misc.PlayerSettings.GetMySpellHue( false, Caster, 0 );
					secondGate.MoveToWorld( loc, map );
				}
			}

			FinishSequence();
		}

		[DispellableField]
		private class InternalItem : Moongate
		{
			public override bool ShowLodorWarning{ get { return false; } }

			public InternalItem( Point3D target, Map map ) : base( target, map )
			{
				Map = map;

				Dispellable = true;

				InternalTimer t = new InternalTimer( this );
				t.Start();
			}

			public InternalItem( Serial serial ) : base( serial )
			{
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				Delete();
			}

			private class InternalTimer : Timer
			{
				private Item m_Item;

				public InternalTimer( Item item ) : base( TimeSpan.FromSeconds( 30.0 ) )
				{
					Priority = TimerPriority.OneSecond;
					m_Item = item;
				}

				protected override void OnTick()
				{
					m_Item.Delete();
				}
			}
		}

		private class InternalTarget : Target
		{
			private GateTravelSpell m_Owner;

			public InternalTarget( GateTravelSpell owner ) : base( 12, false, TargetFlags.None )
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
						from.SendLocalizedMessage( 501803 ); // That rune is not yet marked.
				}
				else if ( o is Runebook )
				{
					RunebookEntry e = ((Runebook)o).Default;

					if ( e != null )
						m_Owner.Effect( e.Location, e.Map, true );
					else
						from.SendLocalizedMessage( 502354 ); // Target is not marked.
				}
				else
				{
					from.Send( new MessageLocalized( from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 501030, from.Name, "" ) ); // I can not gate travel from that object.
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