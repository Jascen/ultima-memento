using System;
using Server.Network;
using Server.Multis;
using Server.Items;
using Server.Targeting;
using Server.Misc;
using Server.Regions;

namespace Server.Spells.Undead
{
	public class UndeadGraveyardGatewaySpell : UndeadSpell
	{
		private static SpellInfo m_Info = new SpellInfo( "", "", 239, 9021 );
		public override double RequiredSkill{ get{ return 80.0; } }
		public override int RequiredMana{ get{ return 0; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.0 ); } }

		private RunebookEntry m_Entry;

		public UndeadGraveyardGatewaySpell( Mobile caster, Item scroll ) : this( caster, scroll, null )
		{
		}

		public UndeadGraveyardGatewaySpell( Mobile caster, Item scroll, RunebookEntry entry ) : base( caster, scroll, m_Info )
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
				Caster.SendMessage( "That potion does not seem to work in this place." );
			}
			else if ( Worlds.RegionAllowedTeleport( map, loc, loc.X, loc.Y ) == false )
			{
				Caster.SendMessage( "The destination seems magically unreachable with this potion." );
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
				Caster.SendMessage( "You open a black gate to another location." ); 

				Effects.PlaySound( Caster.Location, Caster.Map, 0x653 );
				InternalItem firstGate = new InternalItem( loc, map, Server.Items.BasePotion.EnhancePotions( Caster ) );
				firstGate.MoveToWorld( Caster.Location, Caster.Map );

				if ( Worlds.RegionAllowedTeleport( Caster.Map, Caster.Location, Caster.X, Caster.Y ) == true )
				{
					Effects.PlaySound( loc, map, 0x653 );
					InternalItem secondGate = new InternalItem( Caster.Location, Caster.Map, Server.Items.BasePotion.EnhancePotions( Caster ) );
					secondGate.MoveToWorld( loc, map );
				}
			}

			FinishSequence();
		}

		[DispellableField]
		private class InternalItem : Moongate
		{
			public override bool ShowLodorWarning{ get { return false; } }

			public InternalItem( Point3D target, Map map, int enhance ) : base( target, map )
			{
				Map = map;
				ItemID = 0x1FD4;
				Name = "black gate";
				Dispellable = true;

				InternalTimer t = new InternalTimer( this, enhance );
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

				public InternalTimer( Item item, int enhance ) : base( TimeSpan.FromSeconds( 30.0+((double)(enhance)) ) )
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
			private UndeadGraveyardGatewaySpell m_Owner;

			public InternalTarget( UndeadGraveyardGatewaySpell owner ) : base( 12, false, TargetFlags.None )
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