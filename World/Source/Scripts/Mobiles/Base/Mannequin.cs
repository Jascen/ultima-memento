using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.Targeting;
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

		public static bool IsSwappableLayer( Layer layer )
		{
			for ( int i = 0; i < SwappableLayers.Length; i++ )
			{
				if ( SwappableLayers[i] == layer )
					return true;
			}

			return false;
		}

		private BaseHouse m_House;
		private bool m_Roaming;
		private Mobile m_PauseTarget;
		private DateTime m_PauseUntil;
		private Timer m_WanderTimer;

		// Config gumps whose open state should stop roaming and make the mannequin face the manager.
		private static readonly Type[] m_ConfigGumpTypes = new Type[]
		{
			typeof( Server.Gumps.MannequinOwnerGump ),
			typeof( Server.Gumps.NewPlayerVendorCustomizeGump ),
			typeof( Server.Items.RacePotions.RacePotionsGump )
		};

		private static readonly TimeSpan WanderInitial = TimeSpan.FromSeconds( 3.0 );
		private static readonly TimeSpan WanderInterval = TimeSpan.FromSeconds( 3.0 );
		private static readonly TimeSpan PauseAfterAction = TimeSpan.FromSeconds( 10.0 );

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
		public int CosmeticRaceID { get; set; }

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
				// CantWalk = true makes Paths.cs treat every dry tile as a blocker
				// (it's the "can only swim" flag), so CheckMovement would always fail.
				Frozen = false;
				CantWalk = false;
				StartWanderTimer();
			}
			else
			{
				Frozen = true;
				CantWalk = true;
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

		// Called whenever a manager interacts with the mannequin. Records who, so that
		// the wander tick can face them; pauses while a config gump is open (released
		// instantly when closed via HasGump) and for a short window after any other
		// interaction (backpack open, drag-drop, lift, equip), which has no clean
		// "closed" signal — the timer expires on its own once activity stops.
		public void PauseFor( Mobile from )
		{
			if ( from == null || from.Deleted )
				return;

			m_PauseTarget = from;
			m_PauseUntil = DateTime.UtcNow + PauseAfterAction;

			if ( m_Roaming && ShouldPause() )
				Direction = GetDirectionTo( from );
		}

		private bool ShouldPause()
		{
			if ( m_PauseTarget == null || m_PauseTarget.Deleted || m_PauseTarget.NetState == null )
				return false;

			if ( DateTime.UtcNow < m_PauseUntil )
				return true;

			for ( int i = 0; i < m_ConfigGumpTypes.Length; i++ )
			{
				if ( m_PauseTarget.HasGump( m_ConfigGumpTypes[i] ) )
					return true;
			}

			// Backpack-open path: a container display isn't a gump, so HasGump() misses
			// it. Honor Container.Openers + in-range check instead so the mannequin
			// stays put while the owner browses the backpack. Once they walk out of
			// range the in-range test fails and roaming resumes.
			Container pack = Backpack;
			if ( pack != null && pack.Openers != null && pack.Openers.Contains( m_PauseTarget ) )
			{
				if ( m_PauseTarget.Map == this.Map
					&& m_PauseTarget.InRange( this.GetWorldLocation(), pack.GetUpdateRange( m_PauseTarget ) ) )
				{
					return true;
				}
			}

			return false;
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

			if ( ShouldPause() )
			{
				Direction = GetDirectionTo( m_PauseTarget );
				return;
			}

			// ~10% idle ticks for natural-feeling pauses.
			if ( Utility.RandomDouble() < 0.10 )
				return;

			// Open any closed adjacent doors so the mannequin can pass through.
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

			// Try several directions per tick — the first one that fits inside the house AND
			// actually moves wins. This avoids "turn but don't move" stalls when a single
			// random direction happens to be blocked.
			int startDir = Utility.Random( 8 );

			for ( int i = 0; i < 8; i++ )
			{
				Direction d = (Direction)( ( startDir + i ) & 0x7 );

				int nx = X, ny = Y;
				Server.Movement.Movement.Offset( d, ref nx, ref ny );
				Point3D probe = new Point3D( nx, ny, Z );

				if ( !house.IsInside( probe, 16 ) )
					continue;

				// Mobile.Move only moves when m_Direction matches d; otherwise it turns. To get
				// a single-tick move, set Direction first so the match check passes inside Move.
				if ( ( this.Direction & Direction.Mask ) != d )
					this.Direction = d;

				if ( this.Move( d ) )
					return;
			}
		}

		public Mannequin( BaseHouse house ) : base()
		{
			Name = "a mannequin";
			Body = 0x190;
			Female = false;
			CosmeticRaceID = 0;

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

			if ( from.AccessLevel > AccessLevel.Player )
				return true;

			return m_House != null && !m_House.Deleted && m_House.IsCoOwner( from );
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
			// Mirrors PlayerVendor.AllowEquipFrom: managers can equip the mannequin
			// directly. Base Mobile.AllowEquipFrom only allows self/GM, which would
			// otherwise reject every owner attempt to dress the mannequin.
			if ( CanManage( mob ) )
				return true;

			return base.AllowEquipFrom( mob );
		}

		public override bool EquipItem( Item item )
		{
			// Mannequins are display dummies — bypass item.CanEquip so race-locked,
			// gender-locked, stat-required, and skill-gated gear all goes on without
			// fuss. We still respect layer validity, layer conflicts (CheckEquip), and
			// the OnEquip hooks so visual/stat side effects on the item side fire.
			if ( item == null || item.Deleted || item.Layer == Layer.Invalid )
				return false;

			if ( !CheckEquip( item ) || !OnEquip( item ) || !item.OnEquip( this ) )
				return false;

			AddItem( item );
			return true;
		}

		public override bool CheckNonlocalLift( Mobile from, Item item )
		{
			// Mirrors PlayerVendor.CheckNonlocalLift: managers can freely lift from
			// the mannequin; do NOT defer to base, which only allows self/GM and would
			// otherwise return LRReason.TryToSteal ("That item does not belong to you").
			if ( !CanManage( from ) )
				return false;

			PauseFor( from );
			return true;
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( !CanManage( from ) )
			{
				from.SendMessage( "Only the owner can give items to this mannequin." );
				return false;
			}

			if ( dropped is Gold )
			{
				from.SendMessage( "The mannequin has no use for gold." );
				return false;
			}

			PauseFor( from );

			// Auto-equip if the item belongs to a swappable layer. If the layer is
			// occupied, dump the existing item into the mannequin's backpack first.
			if ( IsSwappableLayer( dropped.Layer ) )
			{
				Item existing = FindItemOnLayer( dropped.Layer );

				if ( existing != null && Backpack != null )
					Backpack.DropItem( existing );

				if ( EquipItem( dropped ) )
					return true;
			}

			if ( Backpack != null && Backpack.TryDropItem( from, dropped, false ) )
				return true;

			from.SendMessage( "The mannequin could not accept that." );
			return false;
		}

		public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
		{
			// Same rationale as CheckNonlocalLift — base.CheckNonlocalDrop refuses
			// any non-self, non-GM drop, which would block legitimate owner drops.
			if ( !CanManage( from ) )
				return false;

			PauseFor( from );
			return true;
		}

		public override bool AllowItemUse( Item item )
		{
			// Mannequins are display-only; no item-use interactions of any kind.
			return false;
		}

		public override bool CanBeRenamedBy( Mobile from )
		{
			if ( CanManage( from ) )
				return true;

			return base.CanBeRenamedBy( from );
		}

		public override bool CanPaperdollBeOpenedBy( Mobile from )
		{
			return true;
		}

		public override bool IsSnoop( Mobile from )
		{
			return false;
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
			base.GetContextMenuEntries( from, list );

			if ( CanManage( from ) )
				list.Add( new ManageMannequinEntry( this, from ) );
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
			if ( raceID <= BaseRace.MonsterRaceIDBase )
			{
				RevertToHuman();
				return;
			}

			// The gump emits button IDs of the form (MonsterRaceIDBase + SpeciesID), e.g.
			// 80075 for Cyclops (body 75). GetCostume expects either a page index (which
			// it converts via GetBody) when > MonsterRaceIDBase, or a raw body value
			// otherwise. Pass the body value directly to skip the page-index translation;
			// otherwise GetBody(75) returns the body for *index* 75, which is wrong.
			int bodyValue = raceID - BaseRace.MonsterRaceIDBase;

			BaseRace costume = BaseRace.GetCostume( bodyValue );
			if ( costume == null )
				return;

			if ( costume.SpeciesID <= 0 )
			{
				costume.Delete();
				return;
			}

			CosmeticRaceID = bodyValue;
			Body = costume.SpeciesID;

			costume.Delete();
		}

		public void RevertToHuman()
		{
			CosmeticRaceID = 0;
			Body = Female ? 0x191 : 0x190;
		}

		public void ToggleFemale( Mobile from )
		{
			if ( !CanManage( from ) )
				return;

			Female = !Female;

			// Only flip body if we're currently human. Race body remains.
			if ( CosmeticRaceID == 0 )
				Body = Female ? 0x191 : 0x190;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 3 ); // version

			writer.Write( (bool) m_Roaming );
			writer.Write( (Item) m_House );
			writer.Write( (int) CosmeticRaceID );

			// Note: Female is persisted by base.Serialize on Mobile.
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 3:
				{
					m_Roaming = reader.ReadBool();
					House = (BaseHouse) reader.ReadItem();
					CosmeticRaceID = reader.ReadInt();
					break;
				}
				case 2:
				{
					m_Roaming = reader.ReadBool();
					goto case 1;
				}
				case 1:
				case 0:
				{
					House = (BaseHouse) reader.ReadItem();
					Female = reader.ReadBool(); // legacy m_Female; base already owns Female now

					if ( version >= 1 )
						CosmeticRaceID = reader.ReadInt();

					break;
				}
			}

			// Migrate any legacy saves that stored CosmeticRaceID in the gump button-ID
			// form (MonsterRaceIDBase + body) — strip the prefix so it's a plain body value.
			if ( CosmeticRaceID > BaseRace.MonsterRaceIDBase )
				CosmeticRaceID -= BaseRace.MonsterRaceIDBase;

			if ( CosmeticRaceID > 0 )
			{
				BaseRace costume = BaseRace.GetCostume( CosmeticRaceID );
				if ( costume != null )
				{
					if ( costume.SpeciesID > 0 )
						Body = costume.SpeciesID;
					costume.Delete();
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
						CantWalk = false;
						StartWanderTimer();
					}
				} ) );
			}
		}

		private class ManageMannequinEntry : ContextMenuEntry
		{
			private Mannequin m_Mannequin;
			private Mobile m_From;

			public ManageMannequinEntry( Mannequin mannequin, Mobile from ) : base( 5101 ) // -> cliloc 3005101 "Edit"
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

		public override int DefaultMaxWeight{ get{ return 0; } }

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

		public override bool CheckItemUse( Mobile from, Item item )
		{
			if ( !base.CheckItemUse( from, item ) )
				return false;

			if ( item is Container )
				return true;

			from.SendLocalizedMessage( 500447 ); // That is not accessible.
			return false;
		}

		public override bool CheckTarget( Mobile from, Target targ, object targeted )
		{
			if ( !base.CheckTarget( from, targ, targeted ) )
				return false;

			if ( from.AccessLevel >= AccessLevel.GameMaster )
				return true;

			Mannequin m = RootParent as Mannequin;
			return m != null && m.CanManage( from );
		}

		public override void GetChildContextMenuEntries( Mobile from, List<ContextMenuEntry> list, Item item )
		{
			base.GetChildContextMenuEntries( from, list, item );

			Mannequin m = RootParent as Mannequin;

			if ( m == null || !m.CanManage( from ) )
				return;

			if ( Mannequin.IsSwappableLayer( item.Layer ) )
				list.Add( new EquipOntoMannequinEntry( m, item ) );
		}

		private class EquipOntoMannequinEntry : ContextMenuEntry
		{
			private Mannequin m_Mannequin;
			private Item m_Item;

			// Cliloc 3006132 "Use" — closest single-word generic action in the
			// 3005xxx-3006xxx context-menu range; "Equip" has no entry there.
			public EquipOntoMannequinEntry( Mannequin mannequin, Item item ) : base( 6132 )
			{
				m_Mannequin = mannequin;
				m_Item = item;
			}

			public override bool NonLocalUse{ get{ return true; } }

			public override void OnClick()
			{
				Mobile from = Owner.From;

				if ( m_Mannequin == null || m_Mannequin.Deleted || m_Item == null || m_Item.Deleted )
					return;

				if ( !m_Mannequin.CanManage( from ) )
					return;

				if ( !Mannequin.IsSwappableLayer( m_Item.Layer ) )
					return;

				Container pack = m_Mannequin.Backpack;
				Item existing = m_Mannequin.FindItemOnLayer( m_Item.Layer );

				if ( existing != null && pack != null )
					pack.DropItem( existing );

				if ( !m_Mannequin.EquipItem( m_Item ) )
					from.SendMessage( "The mannequin could not equip that." );
				else
					m_Mannequin.PauseFor( from );
			}
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
