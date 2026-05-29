using System;
using System.Collections.Generic;
using Server.Spells;
using Server.Targeting;

namespace Server.Items
{
	public class TrapKit : Item
	{
		private static readonly Dictionary<Mobile, DateTime> s_NextTrapPlace = new Dictionary<Mobile, DateTime>();
		private static readonly TimeSpan TrapPlaceCooldown = TimeSpan.FromSeconds( 3.0 );

		public override void ResourceChanged( CraftResource resource )
		{
			if ( !ResourceCanChange() )
				return;

			m_Resource = resource;
			Hue = CraftResources.GetHue(m_Resource);
			Name = CraftResources.GetTradeItemFullName( this, m_Resource, false, false, "trapping tools" );

			if ( CraftResources.GetBonus( m_Resource ) > 0 )
				InfoText1 = string.Format( "Trap Power +{0}", CraftResources.GetBonus( m_Resource ) );
			else
				InfoText1 = null;

			InvalidateProperties();
		}

		[Constructable]
		public TrapKit( ) : base( 0x1EBB )
		{
			Hue = 0;
			Resource = CraftResource.Iron;
			Limits = 25;
			LimitsMax = 25;
			LimitsName = "Uses";
			LimitsDelete = true;
			Weight = 5.0;
			Name = "trapping tools";
		}

		public static bool IsOnTrapCooldown( Mobile m )
		{
			if ( m == null ) return false;

			DateTime next;
			if ( !s_NextTrapPlace.TryGetValue( m, out next ) ) return false;

			return DateTime.Now < next;
		}

		public static TimeSpan GetTrapCooldownRemaining( Mobile m )
		{
			if ( m == null ) return TimeSpan.Zero;

			DateTime next;
			if ( !s_NextTrapPlace.TryGetValue( m, out next ) ) return TimeSpan.Zero;

			var remaining = next - DateTime.Now;

			if ( remaining < TimeSpan.Zero )
				return TimeSpan.Zero;

			return remaining;
		}

		public static void SetTrapCooldown( Mobile m )
		{
			if ( m == null ) return;

			s_NextTrapPlace[m] = DateTime.Now + TrapPlaceCooldown;
		}

		public bool CanPlaceTrap( Mobile from, out string message )
		{
			message = null;

			if ( from == null || from.Deleted || !from.Alive )
			{
				message = "You cannot do that now.";
				return false;
			}

			if ( !IsChildOf( from.Backpack ) )
			{
				message = "These tools must be in your backpack to use.";
				return false;
			}

			if ( Limits <= 0 )
			{
				message = "Your trapping tools have no uses remaining.";
				return false;
			}

			if ( !from.Region.AllowHarmful( from, from ) )
			{
				message = "That doesn't feel like a good idea.";
				return false;
			}

			if ( from.Skills[SkillName.RemoveTrap].Value <= 0 )
			{
				message = "You cannot figure out how these tools work!";
				return false;
			}

			return true;
		}

		public bool CanPlaceTrapAt( Mobile from, Point3D loc, Map map, out string message )
		{
			message = null;

			if ( !CanPlaceTrap( from, out message ) )
				return false;

			if ( map == null || map == Map.Internal || from.Map != map )
			{
				message = "You cannot place a trap there.";
				return false;
			}

			if ( !from.InRange( loc, 2 ) )
			{
				message = "That location is too far away.";
				return false;
			}

			if ( !from.CanSee( loc ) )
			{
				message = "You cannot see that location.";
				return false;
			}

			var region = Region.Find( loc, map );
			if ( region != null && !region.AllowHarmful( from, from ) )
			{
				message = "That doesn't feel like a good idea.";
				return false;
			}

			var traps = 0;

			foreach ( Item m in map.GetItemsInRange( loc, 10 ) )
			{
				if ( m is SetTrap )
					++traps;
			}

			if ( traps > 2 )
			{
				message = "There are too many traps in the area!";
				return false;
			}

			if ( !map.CanFit( loc.X, loc.Y, loc.Z, 16, false, false ) )
			{
				message = "You cannot place a trap there.";
				return false;
			}

			return true;
		}

		public bool TryPlaceTrap( Mobile from, Point3D loc, Map map )
		{
			string message;
			if ( !CanPlaceTrapAt( from, loc, map, out message ) )
			{
				if ( message != null )
					from.SendMessage( message );

				return false;
			}

			var power = (int)( from.Skills[SkillName.RemoveTrap].Value / 2 ) + 1;
			power += CraftResources.GetBonus( Resource );

			var trap = new SetTrap(from, power)
			{
				Hue = Hue,
				Movable = true
			};
			trap.MoveToWorld(loc, map);

			ConsumeLimits( 1 );
			SetTrapCooldown( from );

			return true;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.Spell != null )
			{
				from.SendMessage( "You are already doing something else." );
				return;
			}

			if ( IsOnTrapCooldown( from ) )
			{
				from.SendLocalizedMessage( 500119 ); // You must wait to perform another action
				// var remaining = GetTrapCooldownRemaining( from );
				// var seconds = Math.Max( 1, (int)Math.Ceiling( remaining.TotalSeconds ) );
				// from.SendMessage( string.Format( "You must wait {0} second{1} before setting another trap.", seconds, seconds == 1 ? "" : "s" ) );
				return;
			}

			string message;
			if ( !CanPlaceTrap( from, out message ) )
			{
				if ( message != null )
					from.SendMessage( message );

				return;
			}

			from.SendMessage( "Where do you wish to place the trap?" );
			from.Target = new PlaceTrapTarget( this );
		}

		private class PlaceTrapTarget : Target
		{
			private readonly TrapKit m_TrapKit;

			public PlaceTrapTarget( TrapKit trapKit ) : base( 2, true, TargetFlags.None )
			{
				m_TrapKit = trapKit;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_TrapKit == null || m_TrapKit.Deleted ) return;

				if ( from.Spell != null )
				{
					from.SendMessage( "You are already doing something else." );
					return;
				}

				if ( IsOnTrapCooldown( from ) )
				{
					from.SendLocalizedMessage( 500119 ); // You must wait to perform another action
					return;
				}

				var map = from.Map;
				var p = targeted as IPoint3D;
				if ( map == null || map == Map.Internal || p == null)
				{
					from.SendMessage( "You cannot place a trap there." );
					return;
				}

				SpellHelper.GetSurfaceTop( ref p );

				Point3D loc;
				if ( p is Item ) loc = ((Item)p).GetWorldLocation();
				else loc = new Point3D( p );

				string message;
				if ( !m_TrapKit.CanPlaceTrapAt( from, loc, map, out message ) )
				{
					if ( message != null )
						from.SendMessage( message );

					return;
				}

				from.SendSound( 0x55 );
				new PlaceTrapSpell( m_TrapKit, from, loc, map ).Cast();
			}
		}

		public TrapKit( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version < 1 )
			{
				string trp = reader.ReadString();

				if ( trp == "Dull Copper" ){ Resource = CraftResource.DullCopper; }
				else if ( trp == "Shadow Iron" ){ Resource = CraftResource.ShadowIron; }
				else if ( trp == "Copper" ){ Resource = CraftResource.Copper; }
				else if ( trp == "Bronze" ){ Resource = CraftResource.Bronze; }
				else if ( trp == "Gold" ){ Resource = CraftResource.Gold; }
				else if ( trp == "Agapite" ){ Resource = CraftResource.Agapite; }
				else if ( trp == "Verite" ){ Resource = CraftResource.Verite; }
				else if ( trp == "Valorite" ){ Resource = CraftResource.Valorite; }
				else if ( trp == "Nepturite" ){ Resource = CraftResource.Nepturite; }
				else if ( trp == "Obsidian" ){ Resource = CraftResource.Obsidian; }
				else if ( trp == "Steel" ){ Resource = CraftResource.Steel; }
				else if ( trp == "Brass" ){ Resource = CraftResource.Brass; }
				else if ( trp == "Mithril" ){ Resource = CraftResource.Mithril; }
				else if ( trp == "Xormite" ){ Resource = CraftResource.Xormite; }
				else if ( trp == "Dwarven" ){ Resource = CraftResource.Dwarven; }

				LimitsMax = 25;
				Limits = (int)reader.ReadInt();
				LimitsName = "Uses";
				LimitsDelete = true;
			}
		}

		private class PlaceTrapSpell : Spell
		{
			private static readonly SpellInfo m_Info = new SpellInfo( "Set Trap", "", 32, false );

			private readonly TrapKit m_TrapKit;
			private readonly Point3D m_Location;
			private readonly Map m_Map;

			public PlaceTrapSpell( TrapKit trapKit, Mobile caster, Point3D loc, Map map ) : base( caster, null, m_Info )
			{
				m_TrapKit = trapKit;
				m_Location = loc;
				m_Map = map;
			}

			public override bool BlocksMovement { get { return true; } }
			public override bool ClearHandsOnCast { get { return false; } }
			public override bool RevealOnCast { get { return true; } }
			public override bool CheckNextSpellTime { get { return false; } }
			public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }

			public override TimeSpan GetCastRecovery()
			{
				return TimeSpan.Zero;
			}

			public override TimeSpan GetCastDelay()
			{
				// Not affected by FC
				return CastDelayBase;
			}

			public override int GetMana()
			{
				return 0;
			}

			public override bool ConsumeReagents()
			{
				return true;
			}

			public override bool CheckFizzle()
			{
				return false;
			}

			public override void OnDisturb( DisturbType type, bool message )
			{
				if ( message )
					Caster.SendMessage( "You are interrupted while setting the trap." );
			}

			public override void OnCast()
			{
				if ( m_TrapKit == null || m_TrapKit.Deleted )
				{
					FinishSequence();
					return;
				}

				m_TrapKit.TryPlaceTrap( Caster, m_Location, m_Map );

				FinishSequence();
			}
		}
	}
}
