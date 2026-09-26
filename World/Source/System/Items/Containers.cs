using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
	public class BankBox : Container
	{
		private Mobile m_Owner;
		private bool m_Open;

		public override int DefaultMaxWeight
		{
			get
			{
				return 0;
			}
		}

		public override bool IsVirtualItem
		{
			get { return true; }
		}

		public BankBox( Serial serial ) : base( serial )
		{
		}

		public Mobile Owner
		{
			get
			{
				return m_Owner;
			}
		}

		public bool Opened
		{
			get
			{
				return m_Open;
			}
		}

		public void Open()
		{
			m_Open = true;

			if ( m_Owner != null )
			{
				m_Owner.PrivateOverheadMessage( MessageType.Regular, 0x3B2, true, String.Format( "Your bank is holding {0} of {1} items", TotalItems, MaxItems ), m_Owner.NetState );
				m_Owner.Send( new EquipUpdate( this ) );
				DisplayTo( m_Owner );
			}
		}

		public override bool CheckLift( Mobile from, Item item, ref LRReason reject )
		{
			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (Mobile) m_Owner );
			writer.Write( (bool) m_Open );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Owner = reader.ReadMobile();
					m_Open = reader.ReadBool();

					if ( m_Owner == null )
						Delete();

					break;
				}
			}

			if ( this.ItemID != 0x6540 )
				this.ItemID = 0x6540;

			GumpID = 0xA39;
		}

		private static bool m_SendRemovePacket;

		public static bool SendDeleteOnClose{ get{ return m_SendRemovePacket; } set{ m_SendRemovePacket = value; } }

		public void Close()
		{
			m_Open = false;

			if ( m_Owner != null && m_SendRemovePacket )
				m_Owner.Send( this.RemovePacket );
		}

		public override void OnSingleClick( Mobile from )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
		}

		public override DeathMoveResult OnParentDeath( Mobile parent )
		{
			return DeathMoveResult.RemainEquiped;
		}

		public BankBox( Mobile owner ) : base( 0x6540 )
		{
			Layer = Layer.Bank;
			Movable = false;
			m_Owner = owner;
			GumpID = 0xA39;
		}

		public override bool IsAccessibleTo(Mobile check)
		{
		 	if ( ( check == m_Owner && m_Open ) || check.AccessLevel >= AccessLevel.GameMaster )
		 		return base.IsAccessibleTo (check);
		 	else
		 		return false;
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
		 	if ( ( from == m_Owner && m_Open ) || from.AccessLevel >= AccessLevel.GameMaster )
		 		return base.OnDragDrop( from, dropped );
			else
		 		return false;
		}

		public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
		{
		 	if ( ( from == m_Owner && m_Open ) || from.AccessLevel >= AccessLevel.GameMaster )
		 		return base.OnDragDropInto (from, item, p);
		 	else
		 		return false;
		}
	}

	public class InnRoom : Container
	{
		private Mobile m_Owner;
		private int v_TotalItems;

		[CommandProperty( AccessLevel.GameMaster )]
		public int TotalStored
		{
			get { return v_TotalItems; }
		}

		public override int DefaultMaxWeight
		{
			get
			{
				return 0;
			}
		}

		public override int DefaultMaxItems{ get{ return 500; } }

		public void VirtualTotals()
		{
			v_TotalItems = 0;

			List<Item> items = m_Items;

			if ( items == null )
				return;

			for ( int i = 0; i < items.Count; ++i )
			{
				Item item = items[i];

				item.UpdateTotals();

				if ( item.IsVirtualItem )
					continue;

				v_TotalItems += item.TotalItems + 1;
			}
		}

		public override int GetTotal(TotalType type)
        {
			VirtualTotals();
			return 0;
        }

		public override bool IsVirtualItem
		{
			get { return true; }
		}

		public override bool CheckHold( Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight )
		{
			int maxItems = this.MaxItems;

			if ( maxItems != 0 && v_TotalItems > maxItems )
			{
				if ( message )
					SendFullItemsMessage( m, item );

				return false;
			}

			return true;
		}

		public InnRoom( Serial serial ) : base( serial )
		{
		}

		public Mobile Owner
		{
			get
			{
				return m_Owner;
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			DisplayTo( m_Owner );
		}

		public void Open()
		{
			ItemID = 0x4CF1;
			if ( m_Owner != null )
			{
				m_Owner.InnOpen = true;
				VirtualTotals();
				this.MoveToWorld( m_Owner.Location, m_Owner.Map );
				m_Owner.PrivateOverheadMessage( MessageType.Regular, 0x3B2, true, String.Format( "Your inn room has {0} items", v_TotalItems ), m_Owner.NetState );
				DisplayTo( m_Owner );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			writer.Write( (Mobile) m_Owner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_Owner = reader.ReadMobile();
			GumpID = 0xB3B;
		}

		public InnRoom( Mobile owner ) : base( 0x4CF0 )
		{
			Movable = false;
			Visible = false;
			m_Owner = owner;
			GumpID = 0xB3B;
			LiftOverride = true;
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
		}

		public override bool IsAccessibleTo(Mobile check)
		{
		 	if ( check == m_Owner || check.AccessLevel >= AccessLevel.GameMaster )
		 		return base.IsAccessibleTo (check);
		 	else
		 		return false;
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			VirtualTotals();
		 	if ( from == m_Owner || from.AccessLevel >= AccessLevel.GameMaster )
		 		return base.OnDragDrop( from, dropped );
			else
		 		return false;
		}

		public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
		{
			VirtualTotals();
		 	if ( from == m_Owner || from.AccessLevel >= AccessLevel.GameMaster )
		 		return base.OnDragDropInto (from, item, p);
		 	else
		 		return false;
		}
	}
}