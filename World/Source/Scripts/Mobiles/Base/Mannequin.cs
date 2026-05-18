using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.ContextMenus;

namespace Server.Mobiles
{
	public class Mannequin : Mobile
	{
		private BaseHouse m_House;
		private bool m_Female;

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

		public Mannequin( BaseHouse house ) : base()
		{
			Name = "a mannequin";
			Body = 0x190;
			Female = false;
			m_Female = false;

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
				OpenBackpack( from );
			else
				DisplayPaperdollTo( from );
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
			return CanManage( from );
		}

		public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
		{
			return CanManage( from );
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
			{
				list.Add( new ManagePaperdollEntry( this, from ) );
				list.Add( new PackUpMannequinEntry( this, from ) );
			}
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

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (Item) m_House );
			writer.Write( (bool) m_Female );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					House = (BaseHouse) reader.ReadItem();
					m_Female = reader.ReadBool();
					Female = m_Female;
					break;
				}
			}

			// belt-and-suspenders: ensure state is consistent on load
			Blessed = true;
			Frozen = true;
			CantWalk = true;
		}

		private class ManagePaperdollEntry : ContextMenuEntry
		{
			private Mannequin m_Mannequin;
			private Mobile m_From;

			public ManagePaperdollEntry( Mannequin mannequin, Mobile from ) : base( 6123 ) // "Edit"
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

				m_Mannequin.DisplayPaperdollTo( m_From );
			}
		}

		private class PackUpMannequinEntry : ContextMenuEntry
		{
			private Mannequin m_Mannequin;
			private Mobile m_From;

			public PackUpMannequinEntry( Mannequin mannequin, Mobile from ) : base( 6249 ) // generic action label
			{
				m_Mannequin = mannequin;
				m_From = from;
			}

			public override void OnClick()
			{
				if ( m_Mannequin == null || m_Mannequin.Deleted )
					return;

				m_Mannequin.PackUp( m_From );
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
