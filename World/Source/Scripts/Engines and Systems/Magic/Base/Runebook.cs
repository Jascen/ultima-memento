using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Multis;
using Server.Engines.Craft;
using Server.ContextMenus;
using System.Globalization;

namespace Server.Items
{
	public class Runebook : Item, ISecurable, ICraftable
	{
		public const int MAX_RECALL_RUNES = 16;

		public override void ResourceChanged( CraftResource resource )
		{
			if ( !ResourceCanChange() )
				return;

			ResourceMods.Modify( this, true );
			m_Resource = resource;
			Hue = CraftResources.GetHue(m_Resource);
			ResourceMods.DefaultItemHue( this );
			TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

			ColorHue3 = "EFB62C";
			if ( IsStandardResource( m_Resource ) )
				ColorText3 = null;
			else
				ColorText3 = cultInfo.ToTitleCase(CraftResources.GetTradeItemFullName( this, m_Resource, false, false, "bound" ));

			ResourceMods.Modify( this, false );
			InvalidateProperties();
		}

		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override string DefaultDescription{ get{ return "These books are a way to store multiple marked location runes. Locations are added to a book by dropping a marked rune onto the book, where each book will hold a total of 16 locations. The complete instructions for using the book are on the last two pages of the book text."; } }

		public override void DefaultMainHue( Item item )
		{
			ResourceMods.DefaultItemHue( item );
		}

		private List<RunebookEntry> m_Entries;
		private string m_Description;
		private int m_CurCharges, m_MaxCharges;
		private int m_DefaultIndex;
		private SecureLevel m_Level;
		private List<Mobile> m_Openers = new List<Mobile>();

		[CommandProperty( AccessLevel.GameMaster )]
		public SecureLevel Level
		{
			get{ return m_Level; }
			set{ m_Level = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Description
		{
			get
			{
				return m_Description;
			}
			set
			{
				m_Description = value;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int CurCharges
		{
			get
			{
				return m_CurCharges;
			}
			set
			{
				m_CurCharges = value;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxCharges
		{
			get
			{
				return m_MaxCharges;
			}
			set
			{
				m_MaxCharges = value;
			}
		}
		
		public List<Mobile> Openers
		{
			get
			{
				return m_Openers;
			}
			set
			{
				m_Openers = value;
			}
		}

		public override int LabelNumber{ get{ return 1041267; } } // runebook

		[Constructable]
		public Runebook( int maxCharges ) : base( 0x0F3D )
		{
			Weight = 3.0;
			m_Entries = new List<RunebookEntry>();
			m_MaxCharges = maxCharges;
			m_DefaultIndex = -1;
			m_Level = SecureLevel.CoOwners;
		}

		[Constructable]
		public Runebook() : this( 12 )
		{
		}

		public List<RunebookEntry> Entries
		{
			get
			{
				return m_Entries;
			}
		}

		public RunebookEntry Default
		{
			get
			{
				if ( m_DefaultIndex >= 0 && m_DefaultIndex < m_Entries.Count )
					return m_Entries[m_DefaultIndex];

				return null;
			}
			set
			{
				if ( value == null )
					m_DefaultIndex = -1;
				else
					m_DefaultIndex = m_Entries.IndexOf( value );
			}
		}

		public Runebook( Serial serial ) : base( serial )
		{
		}

		public override bool AllowEquipedCast( Mobile from )
		{
			return true;
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			SetSecureLevelEntry.AddTo( from, this, list );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 5 );

			writer.Write( (int) m_Level );

			writer.Write( m_Entries.Count );

			for ( int i = 0; i < m_Entries.Count; ++i )
				m_Entries[i].Serialize( writer );

			writer.Write( m_Description );
			writer.Write( m_CurCharges );
			writer.Write( m_MaxCharges );
			writer.Write( m_DefaultIndex );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			switch ( version )
			{
				case 5:
				case 4:
					{
						if (version < 5) { var SpellType = (RunebookGump.SpellType)reader.ReadInt(); }
						goto case 3;
					}
				case 3:
				case 2:
				{
					if ( version < 3 )
						m_BuiltBy = reader.ReadMobile();

					goto case 1;
				}
				case 1:
				{
					m_Level = (SecureLevel)reader.ReadInt();
					goto case 0;
				}
				case 0:
				{
					int count = reader.ReadInt();

					m_Entries = new List<RunebookEntry>( count );

					for ( int i = 0; i < count; ++i )
						m_Entries.Add( new RunebookEntry( reader ) );

					m_Description = reader.ReadString();
					m_CurCharges = reader.ReadInt();
					m_MaxCharges = reader.ReadInt();
					m_DefaultIndex = reader.ReadInt();

					break;
				}
			}
			ItemID = 0x0F3D;
		}

		public void DropRune( Mobile from, RunebookEntry e )
		{
			var index = FindIndex( e );
			if ( index < 0 ) return;

			if ( m_DefaultIndex > index )
				m_DefaultIndex -= 1;
			else if ( m_DefaultIndex == index )
				m_DefaultIndex = -1;

			m_Entries.RemoveAt( index );

			RecallRune rune = new RecallRune();

			rune.Target = e.Location;
			rune.TargetMap = e.Map;
			rune.Description = e.Description;
			rune.House = e.House;
			rune.Marked = true;

			from.AddToBackpack( rune );

			from.SendLocalizedMessage( 502421 ); // You have removed the rune.
		}

		public bool CanMove( RunebookEntry entry, bool down )
		{
			var index = FindIndex( entry );
			if (index < 0) return false;

			if ( down ) return index + 1 < m_Entries.Count;

			return 0 <= index - 1;
		}

		public int MoveRune(RunebookEntry entry, bool down)
		{
			var index = FindIndex(entry);
			var newIndex = down ? index + 1 : index - 1;

			var existingEntry = TryGetRune( newIndex );
			if ( existingEntry == null ) return index;

			if ( m_DefaultIndex == index )
				m_DefaultIndex = newIndex;

			m_Entries[index] = existingEntry;
			m_Entries[newIndex] = entry;

			return newIndex;
		}

		public int FindIndex( RunebookEntry entry )
		{
			return m_Entries.FindIndex(e => e == entry);
		}

		public RunebookEntry TryGetRune( int index )
		{
			return m_Entries.Count > index ? m_Entries[index] : null;
		}

		public bool IsOpen( Mobile toCheck )
		{
			NetState ns = toCheck.NetState;

			if ( ns != null ) {
				foreach ( Gump gump in ns.Gumps ) {
					RunebookGump bookGump = gump as RunebookGump;

					if ( bookGump != null && bookGump.Book == this ) {
						return true;
					}
				}
			}

			return false;
		}

		public override bool DisplayLootType{ get{ return Core.AOS; } }

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_BuiltBy != null )
				list.Add( 1050043, m_BuiltBy.Name ); // crafted by ~1_NAME~

			if ( m_Description != null && m_Description.Length > 0 )
				list.Add( 1072173, "{0}\t{1}", "5FAFE3", m_Description );

			list.Add( 1072174, "{0}\t{1}", "EDC73A", "" + m_CurCharges + " of " + m_MaxCharges + " Charges" );
		}
		
		public override bool OnDragLift( Mobile from )
		{
			if ( from.HasGump( typeof( RunebookGump ) ) )
			{
				from.SendLocalizedMessage( 500169 ); // You cannot pick that up.
				return false;
			}
			
			foreach ( Mobile m in m_Openers )
				if ( IsOpen( m ) )
				{
					m.CloseGump( typeof( RunebookGump ) );
					m.SendSound( 0x55 );
				}
				
			m_Openers.Clear();
			
			return true;
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( m_Description != null && m_Description.Length > 0 )
				LabelTo( from, m_Description );

			base.OnSingleClick( from );

			if ( m_BuiltBy != null )
				LabelTo( from, 1050043, m_BuiltBy.Name );
		}

		public override void OnDoubleClick( Mobile m )
		{
			var from = m as PlayerMobile;
			if (from == null) return;

			if ( from.InRange( GetWorldLocation(), (Core.ML ? 3 : 1) ) && CheckAccess( from ) )
			{
				if ( RootParent is BaseCreature )
				{
					from.SendLocalizedMessage( 502402 ); // That is inaccessible.
					return;
				}

				from.CloseGump( typeof( RunebookGump ) );
				from.SendGump( new RunebookGump( from, this, RunebookGump.PageType.Index, 0 ) );
				
				m_Openers.Add( from );
				from.SendSound( 0x55 );
			}
		}

		public virtual void OnTravel()
		{
		}

		public override void OnAfterDuped( Item newItem )
		{
			Runebook book = newItem as Runebook;

			if ( book == null )
				return;

			book.m_Entries = new List<RunebookEntry>();

			for ( int i = 0; i < m_Entries.Count; i++ )
			{
				RunebookEntry entry = m_Entries[i];

				book.m_Entries.Add( new RunebookEntry( entry.Location, entry.Map, entry.Description, entry.House ) );
			}
		}

		public bool CheckAccess( Mobile m )
		{
			if ( !IsLockedDown || m.AccessLevel >= AccessLevel.GameMaster )
				return true;

			BaseHouse house = BaseHouse.FindHouseAt( this );

			if ( house != null && house.IsAosRules && (house.Public ? house.IsBanned( m ) : !house.HasAccess( m )) )
				return false;

			return ( house != null && house.HasSecureAccess( m, m_Level ) );
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is RecallRune )
			{
				if ( IsLockedDown && from.AccessLevel < AccessLevel.GameMaster )
				{
					from.SendLocalizedMessage( 502413, null, 0x35 ); // That cannot be done while the book is locked down.
				}
				else if ( IsOpen( from ) )
				{
					from.SendLocalizedMessage( 1005571 ); // You cannot place objects in the book while viewing the contents.
				}
				else if ( m_Entries.Count < 16 )
				{
					RecallRune rune = (RecallRune)dropped;

					if ( rune.Marked && rune.TargetMap != null )
					{
						m_Entries.Add( new RunebookEntry( rune.Target, rune.TargetMap, rune.Description, rune.House ) );

						dropped.Delete();

						from.Send( new PlaySound( 0x42, GetWorldLocation() ) );

						string desc = rune.Description;

						if ( desc == null || (desc = desc.Trim()).Length == 0 )
							desc = "(indescript)";

						from.SendMessage( desc );

						return true;
					}
					else
					{
						from.SendLocalizedMessage( 502409 ); // This rune does not have a marked location.
					}
				}
				else
				{
					from.SendLocalizedMessage( 502401 ); // This runebook is full.
				}
			}
			else if ( dropped is Elemental_Warp_Scroll || dropped is Elemental_Gate_Scroll || dropped is RecallScroll || dropped is GateTravelScroll || dropped is GraveyardGatewayScroll || dropped is HellsGateScroll || dropped is AstralTravelScroll || dropped is NaturesPassagePotion || dropped is MushroomGatewayPotion )
			{
				if ( m_CurCharges < m_MaxCharges )
				{
					bool jars = false;
					if ( dropped is Elemental_Warp_Scroll || dropped is Elemental_Gate_Scroll || dropped is RecallScroll || dropped is GateTravelScroll || dropped is AstralTravelScroll )
					{
						from.Send( new PlaySound( 0x249, GetWorldLocation() ) );
					}
					else
					{
						jars = true;
						from.Send( new PlaySound( 0x240, GetWorldLocation() ) );
					}

					int amount = dropped.Amount;

					if ( amount > (m_MaxCharges - CurCharges) )
					{
						dropped.Consume( m_MaxCharges - CurCharges );
						CurCharges = m_MaxCharges;
						if ( jars ){ from.AddToBackpack( new Jar( ( m_MaxCharges - CurCharges ) ) ); }
					}
					else
					{
						CurCharges += amount;
						dropped.Delete();
						if ( jars ){ from.AddToBackpack( new Jar( amount ) ); }

						return true;
					}
				}
				else
				{
					from.SendLocalizedMessage( 502410 ); // This book already has the maximum amount of charges.
				}
			}

			return false;
		}
		
		public void ChangeGraphic(Mobile from, Mobile player, Runebook dropped)
		{
			if ( dropped.ItemID == 0x22C5 ){ dropped.ItemID = 0x0F3D; }
			else if ( dropped.ItemID == 0x0F3D ){ dropped.ItemID = 0x5687; }
			else if ( dropped.ItemID == 0x5687 ){ dropped.ItemID = 0x4F50; }
			else if ( dropped.ItemID == 0x4F50 ){ dropped.ItemID = 0x4F51; }
			else if ( dropped.ItemID == 0x4F51 ){ dropped.ItemID = 0x5463; }
			else if ( dropped.ItemID == 0x5463 ){ dropped.ItemID = 0x5464; }
			else { dropped.ItemID = 0x22C5; }

			player.PlaySound( 0x249 );
			player.AddToBackpack ( dropped );
			from.PrivateOverheadMessage(MessageType.Regular, 1153, false, "I have changed the cover of your book.", player.NetState);
		}

		#region ICraftable Members

		public int OnCraft( int quality, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			var skillValue = from.Skills[SkillName.Inscribe].Value;
			MaxCharges = 12 + (int)( quality * skillValue / 20 );
			if ( 125 <= skillValue ) MaxCharges += 6;

			return quality;
		}

		#endregion
	}

	public class RunebookEntry
	{
		private Point3D m_Location;
		private Map m_Map;
		private string m_Description;
		private BaseHouse m_House;

		public Point3D Location
		{
			get{ return m_Location; }
		}

		public Map Map
		{
			get{ return m_Map; }
		}

		public string Description
		{
			get{ return m_Description; }
			set{ m_Description = value; }
		}

		public BaseHouse House
		{
			get{ return m_House; }
		}

		public RunebookEntry( Point3D loc, Map map, string desc, BaseHouse house )
		{
			m_Location = loc;
			m_Map = map;
			m_Description = desc;
			m_House = house;
		}

		public RunebookEntry( GenericReader reader )
		{
			int version = reader.ReadByte();

			switch ( version )
			{
				case 1:
				{
					m_House = reader.ReadItem() as BaseHouse;
					goto case 0;
				}
				case 0:
				{
					m_Location = reader.ReadPoint3D();
					m_Map = reader.ReadMap();
					m_Description = reader.ReadString();

					break;
				}
			}
		}

		public void Serialize( GenericWriter writer )
		{
			if ( m_House != null && !m_House.Deleted )
			{
				writer.Write( (byte) 1 ); // version

				writer.Write( m_House );
			}
			else
			{
				writer.Write( (byte) 0 ); // version
			}

			writer.Write( m_Location );
			writer.Write( m_Map );
			writer.Write( m_Description );
		}
	}
}