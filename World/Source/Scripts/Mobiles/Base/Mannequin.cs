using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.ContextMenus;

namespace Server.Mobiles
{
	public class Mannequin : Mobile
	{
		// Worn layers swappable between a manager and the mannequin.
		// Excludes Backpack, Bank, Mount, Hair, FacialHair, Special.
		private static readonly Layer[] SwappableLayers = new Layer[]
		{
			Layer.OneHanded, Layer.TwoHanded, Layer.Shoes, Layer.Pants, Layer.Shirt,
			Layer.Helm, Layer.Gloves, Layer.Ring, Layer.Trinket, Layer.Neck,
			Layer.Waist, Layer.InnerTorso, Layer.Bracelet, Layer.MiddleTorso,
			Layer.Earrings, Layer.Arms, Layer.Cloak, Layer.OuterTorso,
			Layer.OuterLegs, Layer.InnerLegs
		};

		private BaseHouse m_House;
		private bool m_Female;
		private int m_CosmeticRaceID;
		private bool m_Roaming;
		private DateTime m_PauseUntil;
		private Mobile m_PauseTarget;
		private Timer m_WanderTimer;

		private static readonly TimeSpan PauseDuration = TimeSpan.FromSeconds( 45.0 );
		private static readonly TimeSpan WanderInitial = TimeSpan.FromSeconds( 3.0 );
		private static readonly TimeSpan WanderInterval = TimeSpan.FromSeconds( 3.0 );

		[CommandProperty( AccessLevel.GameMaster )]
		public BaseHouse House
		{
			get { return m_House; }
			set
			{
				if ( m_House != null )
					m_House.Mannequins.Remove( this );

				if ( value != null )
					value.Mannequins.Add( this );

				m_House = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsFemale
		{
			get { return m_Female; }
			set { m_Female = value; Female = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int CosmeticRaceID
		{
			get { return m_CosmeticRaceID; }
			set { m_CosmeticRaceID = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Roaming
		{
			get { return m_Roaming; }
			set { SetRoaming( value ); }
		}

		public void SetRoaming( bool value )
		{
			m_Roaming = value;

			if ( value )
			{
				Frozen = false;
				StartWanderTimer();
			}
			else
			{
				Frozen = true;
				StopWanderTimer();
			}
		}

		private void StartWanderTimer()
		{
			if ( m_WanderTimer != null )
			{
				m_WanderTimer.Stop();
				m_WanderTimer = null;
			}

			m_WanderTimer = Timer.DelayCall( WanderInitial, WanderInterval, new TimerCallback( OnWanderTick ) );
		}

		private void StopWanderTimer()
		{
			if ( m_WanderTimer != null )
			{
				m_WanderTimer.Stop();
				m_WanderTimer = null;
			}
		}

		public void PauseFor( Mobile from )
		{
			if ( from == null || from.Deleted )
				return;

			m_PauseTarget = from;
			m_PauseUntil = DateTime.UtcNow + PauseDuration;

			if ( m_Roaming )
				Direction = GetDirectionTo( from );
		}

		private bool IsPaused()
		{
			return m_PauseTarget != null && !m_PauseTarget.Deleted && DateTime.UtcNow < m_PauseUntil;
		}

		private void OnWanderTick()
		{
			if ( Deleted || !m_Roaming )
			{
				StopWanderTimer();
				return;
			}

			BaseHouse house = m_House;
			if ( house == null || house.Deleted || !house.IsInside( this.Location, 16 ) )
			{
				// Mannequin somehow outside its house; halt roaming.
				SetRoaming( false );
				return;
			}

			if ( IsPaused() )
			{
				Direction = GetDirectionTo( m_PauseTarget );
				return;
			}

			// ~25% of ticks, idle instead of moving — looks more natural
			if ( Utility.RandomDouble() < 0.25 )
				return;

			// Open any closed adjacent doors so the mannequin can pass.
			if ( this.Map != null )
			{
				IPooledEnumerable eable = this.Map.GetItemsInRange( this.Location, 1 );
				foreach ( Item item in eable )
				{
					BaseDoor door = item as BaseDoor;
					if ( door != null && !door.Open )
					{
						try { door.Use( this ); } catch { /* locked or no access — ignore */ }
					}
				}
				eable.Free();
			}

			Direction d = (Direction)( Utility.Random( 8 ) );

			int nx = X, ny = Y;
			Server.Movement.Movement.Offset( d, ref nx, ref ny );
			Point3D probe = new Point3D( nx, ny, Z );

			if ( !house.IsInside( probe, 16 ) )
				return;

			// Move handles stairs + facing. If it fails (frozen/blocked), we just try again next tick.
			this.Direction = d;
			this.Move( d );
		}

		public Mannequin( BaseHouse house ) : base()
		{
			Name = "a mannequin";
			Body = 0x190;
			Female = false;
			m_Female = false;
			m_CosmeticRaceID = 0;

			Blessed = true;
			Frozen = true;
			CantWalk = true;

			InitStats( 100, 100, 100 );

			AddItem( new MannequinBackpack() );

			House = house;
		}

		public Mannequin( Serial serial ) : base( serial )
		{
		}

		public bool CanManage( Mobile from )
		{
			if ( from == null || from.Deleted || Deleted )
				return false;

			if ( from.AccessLevel >= AccessLevel.GameMaster )
				return true;

			BaseHouse house = BaseHouse.FindHouseAt( this );
			return house != null && house.IsCoOwner( from );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from == null )
				return;

			if ( CanManage( from ) )
			{
				PauseFor( from );
				OpenBackpack( from );
			}
			else
			{
				DisplayPaperdollTo( from );
			}
		}

		public void OpenBackpack( Mobile from )
		{
			if ( this.Backpack != null )
			{
				from.Send( new EquipUpdate( this.Backpack ) );
				this.Backpack.DisplayTo( from );
			}
		}

		public override bool AllowEquipFrom( Mobile mob )
		{
			return CanManage( mob );
		}

		public override bool CheckNonlocalLift( Mobile from, Item item )
		{
			if ( CanManage( from ) )
			{
				PauseFor( from );
				return true;
			}

			return false;
		}

		public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
		{
			if ( CanManage( from ) )
			{
				PauseFor( from );
				return true;
			}

			return false;
		}

		public override bool AllowItemUse( Item item )
		{
			return false;
		}

		public override bool CanBeRenamedBy( Mobile from )
		{
			return false;
		}

		public override bool CanBeDamaged()
		{
			return false;
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			// no-op: mannequins are invulnerable; do not call base.
		}

		public override bool CanPaperdollBeOpenedBy( Mobile from )
		{
			return true;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( "(mannequin)" );
		}

		public override void OnAfterDelete()
		{
			StopWanderTimer();

			if ( m_House != null )
			{
				m_House.Mannequins.Remove( this );
				m_House = null;
			}

			base.OnAfterDelete();
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			// Managers see only the management entry (which leads to the paperdoll among other options).
			// Non-managers see the default mobile entries (view-only paperdoll).
			if ( CanManage( from ) )
				list.Add( new ManageMannequinEntry( this, from ) );
			else
				base.GetContextMenuEntries( from, list );
		}

		public bool IsEmptyForPackup()
		{
			if ( Backpack != null && Backpack.Items.Count > 0 )
				return false;

			foreach ( Item item in this.Items )
			{
				if ( item.Layer != Layer.Backpack )
					return false;
			}

			return true;
		}

		public void PackUp( Mobile from )
		{
			if ( !CanManage( from ) )
				return;

			if ( !IsEmptyForPackup() )
			{
				from.SendMessage( "The mannequin must be empty before you can pack it up." );
				return;
			}

			MannequinDeed deed = new MannequinDeed();

			if ( !from.AddToBackpack( deed ) )
			{
				deed.Delete();
				from.SendMessage( "Your backpack is full." );
				return;
			}

			Delete();
		}

		public void SwapGear( Mobile from )
		{
			if ( !CanManage( from ) )
				return;

			if ( from.Deleted )
				return;

			if ( !from.InRange( this.Location, 2 ) )
			{
				from.SendMessage( "You are too far away." );
				return;
			}

			Container playerPack = from.Backpack;
			Container manPack = this.Backpack;

			if ( playerPack == null || manPack == null )
			{
				from.SendMessage( "Both you and the mannequin need a backpack to swap gear." );
				return;
			}

			List<Item> fromPlayer = new List<Item>();
			List<Item> fromMannequin = new List<Item>();

			foreach ( Layer layer in SwappableLayers )
			{
				Item it = from.FindItemOnLayer( layer );
				if ( it != null )
					fromPlayer.Add( it );

				it = this.FindItemOnLayer( layer );
				if ( it != null )
					fromMannequin.Add( it );
			}

			// Phase 1: unequip everything into the *destination* owner's backpack.
			// Items originally on the player will go to the mannequin's pack, and vice versa.
			foreach ( Item it in fromPlayer )
				manPack.DropItem( it );

			foreach ( Item it in fromMannequin )
				playerPack.DropItem( it );

			// Phase 2: try to equip each onto the new owner. On failure, leave in destination pack.
			int playerFails = 0;
			int manFails = 0;

			foreach ( Item it in fromMannequin )
			{
				if ( it.Deleted )
					continue;

				if ( !from.EquipItem( it ) )
					playerFails++;
			}

			foreach ( Item it in fromPlayer )
			{
				if ( it.Deleted )
					continue;

				if ( !this.EquipItem( it ) )
					manFails++;
			}

			from.SendMessage(
				"Gear swapped. {0} item(s) on you and {1} on the mannequin could not be equipped and remain in the relevant backpack.",
				playerFails, manFails );
		}

		public void ApplyRace( int raceID )
		{
			if ( raceID <= 80000 )
			{
				RevertToHuman();
				return;
			}

			BaseRace costume = BaseRace.GetCostume( raceID );
			if ( costume == null )
				return;

			if ( costume.SpeciesID <= 0 )
			{
				costume.Delete();
				return;
			}

			m_CosmeticRaceID = raceID;
			Body = costume.SpeciesID;

			costume.Delete();
		}

		public void RevertToHuman()
		{
			m_CosmeticRaceID = 0;
			Body = m_Female ? 0x191 : 0x190;
		}

		public void ToggleFemale( Mobile from )
		{
			if ( !CanManage( from ) )
				return;

			m_Female = !m_Female;
			Female = m_Female;

			// Only flip body if we're currently human. Race body remains.
			if ( m_CosmeticRaceID == 0 )
				Body = m_Female ? 0x191 : 0x190;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version

			writer.Write( (bool) m_Roaming ); // v2

			writer.Write( (Item) m_House );
			writer.Write( (bool) m_Female );
			writer.Write( (int) m_CosmeticRaceID );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
				{
					m_Roaming = reader.ReadBool();
					goto case 1;
				}
				case 1:
				case 0:
				{
					House = (BaseHouse) reader.ReadItem();
					m_Female = reader.ReadBool();
					Female = m_Female;

					if ( version >= 1 )
						m_CosmeticRaceID = reader.ReadInt();

					if ( m_CosmeticRaceID > 0 )
					{
						BaseRace costume = BaseRace.GetCostume( m_CosmeticRaceID );
						if ( costume != null )
						{
							if ( costume.SpeciesID > 0 )
								Body = costume.SpeciesID;
							costume.Delete();
						}
					}
					break;
				}
			}

			// belt-and-suspenders: ensure state is consistent on load
			Blessed = true;
			Frozen = true;
			CantWalk = true;

			// Defer wander-timer start until world is fully loaded.
			if ( m_Roaming )
			{
				Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerCallback( delegate
				{
					if ( !Deleted && m_Roaming )
					{
						Frozen = false;
						StartWanderTimer();
					}
				} ) );
			}
		}

		private class ManageMannequinEntry : ContextMenuEntry
		{
			private Mannequin m_Mannequin;
			private Mobile m_From;

			public ManageMannequinEntry( Mannequin mannequin, Mobile from ) : base( 1019069 ) // "Customize"
			{
				m_Mannequin = mannequin;
				m_From = from;
			}

			public override void OnClick()
			{
				if ( m_Mannequin == null || m_Mannequin.Deleted )
					return;

				if ( !m_Mannequin.CanManage( m_From ) )
					return;

				m_From.CloseGump( typeof( MannequinOwnerGump ) );
				m_From.SendGump( new MannequinOwnerGump( m_Mannequin, m_From ) );
			}
		}
	}

	public class MannequinBackpack : Backpack
	{
		public MannequinBackpack()
		{
			Layer = Layer.Backpack;
			Movable = false;
		}

		public MannequinBackpack( Serial serial ) : base( serial )
		{
		}

		public override bool CheckHold( Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight )
		{
			if ( !base.CheckHold( m, item, message, checkItems, plusItems, plusWeight ) )
				return false;

			if ( Parent is Mannequin )
			{
				BaseHouse house = BaseHouse.FindHouseAt( this );

				if ( house != null && house.IsAosRules && !house.CheckAosStorage( 1 + item.TotalItems + plusItems ) )
				{
					if ( message )
						m.SendLocalizedMessage( 1061839 ); // This action would exceed the secure storage limit of the house.

					return false;
				}
			}

			return true;
		}

		public override bool IsAccessibleTo( Mobile m )
		{
			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
