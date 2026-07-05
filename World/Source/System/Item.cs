/***************************************************************************
 *                                  Item.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id$
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Items;
using Server.ContextMenus;
using Server.Gumps;
using System.Globalization;
using Server.Targeting;

namespace Server
{
	/// <summary>
	/// Enumeration of item layer values.
	/// </summary>
	public enum Layer : byte
	{
		/// <summary>
		/// Invalid layer.
		/// </summary>
		Invalid		 = 0x00,
		/// <summary>
		/// First valid layer. Equivalent to <c>Layer.OneHanded</c>.
		/// </summary>
		FirstValid   = 0x01,
		/// <summary>
		/// One handed weapon.
		/// </summary>
		OneHanded	 = 0x01,
		/// <summary>
		/// Two handed weapon or shield.
		/// </summary>
		TwoHanded	 = 0x02,
		/// <summary>
		/// Shoes.
		/// </summary>
		Shoes		 = 0x03,
		/// <summary>
		/// Pants.
		/// </summary>
		Pants		 = 0x04,
		/// <summary>
		/// Shirts.
		/// </summary>
		Shirt		 = 0x05,
		/// <summary>
		/// Helmets, hats, and masks.
		/// </summary>
		Helm		 = 0x06,
		/// <summary>
		/// Gloves.
		/// </summary>
		Gloves		 = 0x07,
		/// <summary>
		/// Rings.
		/// </summary>
		Ring		 = 0x08,
		/// <summary>
		/// Trinkets.
		/// </summary>
		Trinket	 = 0x09,
		/// <summary>
		/// Gorgets and necklaces.
		/// </summary>
		Neck		 = 0x0A,
		/// <summary>
		/// Hair.
		/// </summary>
		Hair		 = 0x0B,
		/// <summary>
		/// Half aprons.
		/// </summary>
		Waist		 = 0x0C,
		/// <summary>
		/// Torso, inner layer.
		/// </summary>
		InnerTorso	 = 0x0D,
		/// <summary>
		/// Bracelets.
		/// </summary>
		Bracelet	 = 0x0E,
		/// <summary>
		/// Special.
		/// </summary>
		Special = 0x0F,
		/// <summary>
		/// Beards and mustaches.
		/// </summary>
		FacialHair	 = 0x10,
		/// <summary>
		/// Torso, outer layer.
		/// </summary>
		MiddleTorso	 = 0x11,
		/// <summary>
		/// Earings.
		/// </summary>
		Earrings	 = 0x12,
		/// <summary>
		/// Arms and sleeves.
		/// </summary>
		Arms		 = 0x13,
		/// <summary>
		/// Cloaks.
		/// </summary>
		Cloak		 = 0x14,
		/// <summary>
		/// Backpacks.
		/// </summary>
		Backpack	 = 0x15,
		/// <summary>
		/// Torso, outer layer.
		/// </summary>
		OuterTorso	 = 0x16,
		/// <summary>
		/// Leggings, outer layer.
		/// </summary>
		OuterLegs	 = 0x17,
		/// <summary>
		/// Leggings, inner layer.
		/// </summary>
		InnerLegs	 = 0x18,
		/// <summary>
		/// Last valid non-internal layer. Equivalent to <c>Layer.InnerLegs</c>.
		/// </summary>
		LastUserValid= 0x18,
		/// <summary>
		/// Mount item layer.
		/// </summary>
		Mount		 = 0x19,
		/// <summary>
		/// Vendor 'buy pack' layer.
		/// </summary>
		ShopBuy		 = 0x1A,
		/// <summary>
		/// Vendor 'resale pack' layer.
		/// </summary>
		ShopResale	 = 0x1B,
		/// <summary>
		/// Vendor 'sell pack' layer.
		/// </summary>
		ShopSell	 = 0x1C,
		/// <summary>
		/// Bank box layer.
		/// </summary>
		Bank		 = 0x1D,
		/// <summary>
		/// Last valid layer. Equivalent to <c>Layer.Bank</c>.
		/// </summary>
		LastValid    = 0x1D
	}

	/// <summary>
	/// Internal flags used to signal how the item should be updated and resent to nearby clients.
	/// </summary>
	[Flags]
	public enum ItemDelta
	{
		/// <summary>
		/// Nothing.
		/// </summary>
		None		= 0x00000000,
		/// <summary>
		/// Resend the item.
		/// </summary>
		Update		= 0x00000001,
		/// <summary>
		/// Resend the item only if it is equiped.
		/// </summary>
		EquipOnly	= 0x00000002,
		/// <summary>
		/// Resend the item's properties.
		/// </summary>
		Properties	= 0x00000004
	}

	/// <summary>
	/// Enumeration containing possible ways to handle item ownership on death.
	/// </summary>
	public enum DeathMoveResult
	{
		/// <summary>
		/// The item should be placed onto the corpse.
		/// </summary>
		MoveToCorpse,
		/// <summary>
		/// The item should remain equiped.
		/// </summary>
		RemainEquiped,
		/// <summary>
		/// The item should be placed into the owners backpack.
		/// </summary>
		MoveToBackpack
	}

	/// <summary>
	/// Enumeration containing all possible light types. These are only applicable to light source items, like lanterns, candles, braziers, etc.
	/// </summary>
	public enum LightType
	{
		/// <summary>
		/// Window shape, arched, ray shining east.
		/// </summary>
		ArchedWindowEast,
		/// <summary>
		/// Medium circular shape.
		/// </summary>
		Circle225,
		/// <summary>
		/// Small circular shape.
		/// </summary>
		Circle150,
		/// <summary>
		/// Door shape, shining south.
		/// </summary>
		DoorSouth,
		/// <summary>
		/// Door shape, shining east.
		/// </summary>
		DoorEast,
		/// <summary>
		/// Large semicircular shape (180 degrees), north wall.
		/// </summary>
		NorthBig,
		/// <summary>
		/// Large pie shape (90 degrees), north-east corner.
		/// </summary>
		NorthEastBig,
		/// <summary>
		/// Large semicircular shape (180 degrees), east wall.
		/// </summary>
		EastBig,
		/// <summary>
		/// Large semicircular shape (180 degrees), west wall.
		/// </summary>
		WestBig,
		/// <summary>
		/// Large pie shape (90 degrees), south-west corner.
		/// </summary>
		SouthWestBig,
		/// <summary>
		/// Large semicircular shape (180 degrees), south wall.
		/// </summary>
		SouthBig,
		/// <summary>
		/// Medium semicircular shape (180 degrees), north wall.
		/// </summary>
		NorthSmall,
		/// <summary>
		/// Medium pie shape (90 degrees), north-east corner.
		/// </summary>
		NorthEastSmall,
		/// <summary>
		/// Medium semicircular shape (180 degrees), east wall.
		/// </summary>
		EastSmall,
		/// <summary>
		/// Medium semicircular shape (180 degrees), west wall.
		/// </summary>
		WestSmall,
		/// <summary>
		/// Medium semicircular shape (180 degrees), south wall.
		/// </summary>
		SouthSmall,
		/// <summary>
		/// Shaped like a wall decoration, north wall.
		/// </summary>
		DecorationNorth,
		/// <summary>
		/// Shaped like a wall decoration, north-east corner.
		/// </summary>
		DecorationNorthEast,
		/// <summary>
		/// Small semicircular shape (180 degrees), east wall.
		/// </summary>
		EastTiny,
		/// <summary>
		/// Shaped like a wall decoration, west wall.
		/// </summary>
		DecorationWest,
		/// <summary>
		/// Shaped like a wall decoration, south-west corner.
		/// </summary>
		DecorationSouthWest,
		/// <summary>
		/// Small semicircular shape (180 degrees), south wall.
		/// </summary>
		SouthTiny,
		/// <summary>
		/// Window shape, rectangular, no ray, shining south.
		/// </summary>
		RectWindowSouthNoRay,
		/// <summary>
		/// Window shape, rectangular, no ray, shining east.
		/// </summary>
		RectWindowEastNoRay,
		/// <summary>
		/// Window shape, rectangular, ray shining south.
		/// </summary>
		RectWindowSouth,
		/// <summary>
		/// Window shape, rectangular, ray shining east.
		/// </summary>
		RectWindowEast,
		/// <summary>
		/// Window shape, arched, no ray, shining south.
		/// </summary>
		ArchedWindowSouthNoRay,
		/// <summary>
		/// Window shape, arched, no ray, shining east.
		/// </summary>
		ArchedWindowEastNoRay,
		/// <summary>
		/// Window shape, arched, ray shining south.
		/// </summary>
		ArchedWindowSouth,
		/// <summary>
		/// Large circular shape.
		/// </summary>
		Circle300,
		/// <summary>
		/// Large pie shape (90 degrees), north-west corner.
		/// </summary>
		NorthWestBig,
		/// <summary>
		/// Negative light. Medium pie shape (90 degrees), south-east corner.
		/// </summary>
		DarkSouthEast,
		/// <summary>
		/// Negative light. Medium semicircular shape (180 degrees), south wall.
		/// </summary>
		DarkSouth,
		/// <summary>
		/// Negative light. Medium pie shape (90 degrees), north-west corner.
		/// </summary>
		DarkNorthWest,
		/// <summary>
		/// Negative light. Medium pie shape (90 degrees), south-east corner. Equivalent to <c>LightType.SouthEast</c>.
		/// </summary>
		DarkSouthEast2,
		/// <summary>
		/// Negative light. Medium circular shape (180 degrees), east wall.
		/// </summary>
		DarkEast,
		/// <summary>
		/// Negative light. Large circular shape.
		/// </summary>
		DarkCircle300,
		/// <summary>
		/// Opened door shape, shining south.
		/// </summary>
		DoorOpenSouth,
		/// <summary>
		/// Opened door shape, shining east.
		/// </summary>
		DoorOpenEast,
		/// <summary>
		/// Window shape, square, ray shining east.
		/// </summary>
		SquareWindowEast,
		/// <summary>
		/// Window shape, square, no ray, shining east.
		/// </summary>
		SquareWindowEastNoRay,
		/// <summary>
		/// Window shape, square, ray shining south.
		/// </summary>
		SquareWindowSouth,
		/// <summary>
		/// Window shape, square, no ray, shining south.
		/// </summary>
		SquareWindowSouthNoRay,
		/// <summary>
		/// Empty.
		/// </summary>
		Empty,
		/// <summary>
		/// Window shape, skinny, no ray, shining south.
		/// </summary>
		SkinnyWindowSouthNoRay,
		/// <summary>
		/// Window shape, skinny, ray shining east.
		/// </summary>
		SkinnyWindowEast,
		/// <summary>
		/// Window shape, skinny, no ray, shining east.
		/// </summary>
		SkinnyWindowEastNoRay,
		/// <summary>
		/// Shaped like a hole, shining south.
		/// </summary>
		HoleSouth,
		/// <summary>
		/// Shaped like a hole, shining south.
		/// </summary>
		HoleEast,
		/// <summary>
		/// Large circular shape with a moongate graphic embeded.
		/// </summary>
		Moongate,
		/// <summary>
		/// Unknown usage. Many rows of slightly angled lines.
		/// </summary>
		Strips,
		/// <summary>
		/// Shaped like a small hole, shining south.
		/// </summary>
		SmallHoleSouth,
		/// <summary>
		/// Shaped like a small hole, shining east.
		/// </summary>
		SmallHoleEast,
		/// <summary>
		/// Large semicircular shape (180 degrees), north wall. Identical graphic as <c>LightType.NorthBig</c>, but slightly different positioning.
		/// </summary>
		NorthBig2,
		/// <summary>
		/// Large semicircular shape (180 degrees), west wall. Identical graphic as <c>LightType.WestBig</c>, but slightly different positioning.
		/// </summary>
		WestBig2,
		/// <summary>
		/// Large pie shape (90 degrees), north-west corner. Equivalent to <c>LightType.NorthWestBig</c>.
		/// </summary>
		NorthWestBig2
	}

	/// <summary>
	/// Enumeration of an item's loot and steal state.
	/// </summary>
	public enum LootType : byte
	{
		/// <summary>
		/// Stealable. Lootable.
		/// </summary>
		Regular = 0,
		/// <summary>
		/// Unstealable. Unlootable, unless owned by a murderer.
		/// </summary>
		Newbied = 1,
		/// <summary>
		/// Unstealable. Unlootable, always.
		/// </summary>
		Blessed = 2,
		/// <summary>
		/// Stealable. Lootable, always.
		/// </summary>
		Cursed  = 3
	}

	public enum ArtifactLevel
	{
		None = 0,
		Artifact = 1,
		StandardArtefact = 2,
		LegendaryArtefact = 3,
		DecorativeArtefact = 4,
	}

	public class BounceInfo
	{
		public Map m_Map;
		public Point3D m_Location, m_WorldLoc;
		public object m_Parent;

		public BounceInfo( Item item )
		{
			m_Map = item.Map;
			m_Location = item.Location;
			m_WorldLoc = item.GetWorldLocation();
			m_Parent = item.Parent;
		}

		private BounceInfo( Map map, Point3D loc, Point3D worldLoc, object parent )
		{
			m_Map = map;
			m_Location = loc;
			m_WorldLoc = worldLoc;
			m_Parent = parent;
		}

		public static BounceInfo Deserialize( GenericReader reader )
		{
			if ( reader.ReadBool() )
			{
				Map map = reader.ReadMap();
				Point3D loc = reader.ReadPoint3D();
				Point3D worldLoc = reader.ReadPoint3D();

				object parent;

				Serial serial = reader.ReadInt();

				if ( serial.IsItem )
					parent = World.FindItem( serial );
				else if ( serial.IsMobile )
					parent = World.FindMobile( serial );
				else
					parent = null;

				return new BounceInfo( map, loc, worldLoc, parent );
			}
			else
			{
				return null;
			}
		}

		public static void Serialize( BounceInfo info, GenericWriter writer )
		{
			if ( info == null )
			{
				writer.Write( false );
			}
			else
			{
				writer.Write( true );

				writer.Write( info.m_Map );
				writer.Write( info.m_Location );
				writer.Write( info.m_WorldLoc );

				if ( info.m_Parent is Mobile )
					writer.Write( (Mobile) info.m_Parent );
				else if ( info.m_Parent is Item )
					writer.Write( (Item) info.m_Parent );
				else
					writer.Write( (Serial) 0 );
			}
		}
	}

	public enum TotalType
	{
		Gold,
		Items,
		Weight,
	}

	[Flags]
	public enum ExpandFlag
	{
		None		= 0x00,

		Name		= 0x01,
		Items		= 0x02,
		Bounce		= 0x04,
		Holder		= 0x08,
		Blessed		= 0x10,
		TempFlag	= 0x20,
		SaveFlag	= 0x40,
		Weight		= 0x80
	}

	public class Item : IEntity, IHued, IComparable<Item>, ISerializable, ISpawnable
	{
		public static readonly List<Item> EmptyItems = new List<Item>();

		public int CompareTo( IEntity other )
		{
			if ( other == null )
				return -1;

			return m_Serial.CompareTo( other.Serial );
		}

		public int CompareTo( Item other )
		{
			return this.CompareTo( (IEntity) other );
		}

		public int CompareTo( object other )
		{
			if ( other == null || other is IEntity )
				return this.CompareTo( (IEntity) other );

			throw new ArgumentException();
		}

		#region Standard fields
		private Serial m_Serial;
		private Point3D m_Location;
		private int m_ItemID;
		private int m_Hue;
		private int m_Amount;
		private Layer m_Layer;
		private object m_Parent;
		private Map m_Map;
		private LootType m_LootType;
		private DateTime m_LastMovedTime;
		private Direction m_Direction;
		#endregion

		private ItemDelta m_DeltaFlags;
		private ImplFlag m_Flags;

		#region Packet caches
		private Packet m_WorldPacket;
		private Packet m_WorldPacketSA;
		private Packet m_WorldPacketHS;
		private Packet m_RemovePacket;

		private Packet m_OPLPacket;
		private ObjectPropertyList m_PropertyList;
		#endregion

		public bool NameWasSynced { get; private set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public Land Land { get { return Server.Lands.GetLand( Map, Location, X, Y ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Terrain Terrain { get { return Server.Terrains.GetTerrain( Map, Location, X, Y ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual Density Density { get { return Density.None; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Difficulty Difficulty
		{
			get
			{
				int lvl = Server.Difficult.GetDifficulty( Location, Map );

				if ( lvl < -1 )
					lvl = 0;

				return (Difficulty)(lvl+1);
			}
		}

		public string m_ColorHue1;
		[CommandProperty(AccessLevel.Owner)]
		public string ColorHue1 { get { return m_ColorHue1; } set { m_ColorHue1 = value; InvalidateProperties(); } }

		public string m_ColorText1;
		[CommandProperty(AccessLevel.Owner)]
		public string ColorText1 { get { return m_ColorText1; } set { m_ColorText1 = value; InvalidateProperties(); } }

		public string m_ColorHue2;
		[CommandProperty(AccessLevel.Owner)]
		public string ColorHue2 { get { return m_ColorHue2; } set { m_ColorHue2 = value; InvalidateProperties(); } }

		public string m_ColorText2;
		[CommandProperty(AccessLevel.Owner)]
		public string ColorText2 { get { return m_ColorText2; } set { m_ColorText2 = value; InvalidateProperties(); } }

		public string m_ColorHue3;
		[CommandProperty(AccessLevel.Owner)]
		public string ColorHue3 { get { return m_ColorHue3; } set { m_ColorHue3 = value; InvalidateProperties(); } }

		public string m_ColorText3;
		[CommandProperty(AccessLevel.Owner)]
		public string ColorText3 { get { return m_ColorText3; } set { m_ColorText3 = value; InvalidateProperties(); } }

		public string m_ColorHue4;
		[CommandProperty(AccessLevel.Owner)]
		public string ColorHue4 { get { return m_ColorHue4; } set { m_ColorHue4 = value; InvalidateProperties(); } }

		public string m_ColorText4;
		[CommandProperty(AccessLevel.Owner)]
		public string ColorText4 { get { return m_ColorText4; } set { m_ColorText4 = value; InvalidateProperties(); } }

		public string m_ColorHue5;
		[CommandProperty(AccessLevel.Owner)]
		public string ColorHue5 { get { return m_ColorHue5; } set { m_ColorHue5 = value; InvalidateProperties(); } }

		public string m_ColorText5;
		[CommandProperty(AccessLevel.Owner)]
		public string ColorText5 { get { return m_ColorText5; } set { m_ColorText5 = value; InvalidateProperties(); } }

		public int m_CoinPrice;
		[CommandProperty(AccessLevel.Owner)]
		public int CoinPrice
		{
			get
			{
				return m_CoinPrice;
			}
			set
			{
				if ( m_CoinPrice != value )
				{
					ItemPriced( value );
					InvalidateProperties();
				}
			}
		}

		public int m_WorldItemID;
		[CommandProperty(AccessLevel.Owner)]
		public int WorldItemID { get { return m_WorldItemID; } set { m_WorldItemID = value; InvalidateProperties(); } }

		public bool m_Technology;
		[CommandProperty(AccessLevel.Owner)]
		public bool Technology { get { return m_Technology; } set { m_Technology = value; InfoData = DefaultDescription; InvalidateProperties(); } }

		public bool m_VirtualContainer;
		[CommandProperty(AccessLevel.Owner)]
		public bool VirtualContainer { get { return m_VirtualContainer; } set { m_VirtualContainer = value; InvalidateProperties(); } }

		public bool m_NotIdentified;
		[CommandProperty(AccessLevel.Owner)]
		public bool NotIdentified
		{
			get
			{
				return m_NotIdentified;
			}
			set
			{
				if ( m_NotIdentified != value )
				{
					ItemIdentified( value );
					InvalidateProperties();
				}
			}
		}

		public const int MAX_ID_ATTEMPTS = 6;

		public int m_NotIDAttempts;
		[CommandProperty(AccessLevel.Owner)]
		public int NotIDAttempts { get { return m_NotIDAttempts; } set { m_NotIDAttempts = value; InvalidateProperties(); } }

		public Identity m_NotIDSource;
		[CommandProperty(AccessLevel.Owner)]
		public Identity NotIDSource { get { return m_NotIDSource; } set { m_NotIDSource = value; InvalidateProperties(); } }

		public IDSkill m_NotIDSkill;
		[CommandProperty(AccessLevel.Owner)]
		public IDSkill NotIDSkill { get { return m_NotIDSkill; } set { m_NotIDSkill = value; InvalidateProperties(); } }

		public Catalogs m_Catalog;
		[CommandProperty(AccessLevel.Owner)]
		public Catalogs Catalog { get { return m_Catalog; } set { m_Catalog = value; InvalidateProperties(); } }

		public int m_GraphicID;
		[CommandProperty(AccessLevel.Owner)]
		public int GraphicID { get { return m_GraphicID; } set { m_GraphicID = value; InvalidateProperties(); } }

		public int m_GraphicHue;
		[CommandProperty(AccessLevel.Owner)]
		public int GraphicHue { get { return m_GraphicHue; } set { m_GraphicHue = value; InvalidateProperties(); } }

		public Mobile m_LastMobile;
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile LastMobile { get{ return m_LastMobile; } set{ m_LastMobile = value; } }

		public string m_LastMobileName;
		[CommandProperty(AccessLevel.Owner)]
		public string LastMobileName { get { return m_LastMobileName; } set { m_LastMobileName = value; InvalidateProperties(); } }

		public CraftResource m_Resource;
		[CommandProperty(AccessLevel.Owner)]
		public CraftResource Resource
		{
			get
			{
				return m_Resource;
			}
			set
			{
				if ( m_Resource != value )
				{
					ResourceChanged( value );

					if ( m_Resource != CraftResource.None && m_Resource != CraftResource.Iron && m_Resource != CraftResource.Fabric && m_Resource != CraftResource.RegularLeather && m_Resource != CraftResource.RegularWood && m_Resource != CraftResource.BrittleSkeletal )
						SubResource = CraftResource.None;

					SetDefaultHue();
					InvalidateProperties();
				}
			}
		}

		public CraftResource m_SubResource;
		[CommandProperty(AccessLevel.Owner)]
		public CraftResource SubResource
		{
			get
			{
				return m_SubResource;
			}
			set
			{
				if ( m_SubResource != value )
				{
					if ( m_SubResource == CraftResource.Iron || m_SubResource == CraftResource.Fabric || m_SubResource == CraftResource.RegularLeather || m_SubResource == CraftResource.RegularWood || m_SubResource == CraftResource.BrittleSkeletal )
					{
						m_SubResource = CraftResource.None;
						m_SubName = null;
					}
					else
						m_SubResource = value;

					SubResourceChanged( value );

					InvalidateProperties();
				}
			}
		}

		public string m_SubName;
		[CommandProperty(AccessLevel.Owner)]
		public string SubName
		{
			get
			{
				return m_SubName;
			}
			set
			{
				if ( m_SubName != value )
				{
					if ( m_SubResource == CraftResource.Iron || m_SubResource == CraftResource.Fabric || m_SubResource == CraftResource.RegularLeather || m_SubResource == CraftResource.RegularWood || m_SubResource == CraftResource.BrittleSkeletal )
						m_SubName = null;
					else
						m_SubName = value;

					InvalidateProperties();
				}
			}
		}

		public virtual CraftResource DefaultResource{ get{ return CraftResource.None; } }
		public virtual int DefaultHue{ get{ return 0; } }
		public virtual Catalogs DefaultCatalog{ get{ return Catalogs.None; } }
		public virtual string DefaultDescription{ get{ return null; } }
		public virtual bool DoNotCountContents{ get{ return false; } }

		public ArtifactLevel m_ArtifactLevel;
		[CommandProperty(AccessLevel.Owner)]
		public ArtifactLevel ArtifactLevel { get { return m_ArtifactLevel; } set { m_ArtifactLevel = value; InvalidateProperties(); } }

		public bool m_NotModAble;
		[CommandProperty(AccessLevel.Owner)]
		public bool NotModAble { get { return m_NotModAble; } set { m_NotModAble = value; InvalidateProperties(); } }

		public bool m_NeedsBothHands;
		[CommandProperty(AccessLevel.Owner)]
		public bool NeedsBothHands { get { return m_NeedsBothHands; } set { m_NeedsBothHands = value; InvalidateProperties(); } }

		public MagicSpell m_Enchanted;
		[CommandProperty(AccessLevel.Owner)]
		public MagicSpell Enchanted
		{
			get
			{
				return m_Enchanted;
			}
			set
			{
				if ( m_Enchanted != value )
				{
					m_Enchanted = value;
					MagicSpellChanged( m_Enchanted );
					InvalidateProperties();
				}
			}
		}

		public int m_EnchantMod;
		[CommandProperty(AccessLevel.Owner)]
		public int EnchantMod { get { return m_EnchantMod; } set { m_EnchantMod = value; InvalidateProperties(); } }

		public int m_EnchantUses;
		[CommandProperty(AccessLevel.Owner)]
		public int EnchantUses
		{
			get
			{
				return m_EnchantUses;
			}
			set
			{
				if ( m_EnchantUses != value )
				{
					m_EnchantUses = value;

					if ( m_EnchantUsesMax < 1 && m_EnchantUses < 1 )
						Enchanted = MagicSpell.None;

					InvalidateProperties();
				}
			}
		}

		public int m_EnchantUsesMax;
		[CommandProperty(AccessLevel.Owner)]
		public int EnchantUsesMax
		{
			get
			{
				return m_EnchantUsesMax;
			}
			set
			{
				if ( m_EnchantUsesMax != value )
				{
					m_EnchantUsesMax = value;
					InvalidateProperties();
				}
			}
		}

		public string m_InfoData;
		[CommandProperty(AccessLevel.Owner)]
		public string InfoData { get { return m_InfoData; } set { m_InfoData = value; InvalidateProperties(); } }

		public string m_InfoText1;
		[CommandProperty(AccessLevel.Owner)]
		public string InfoText1 { get { return m_InfoText1; } set { m_InfoText1 = value; InvalidateProperties(); } }

		public string m_InfoText2;
		[CommandProperty(AccessLevel.Owner)]
		public string InfoText2 { get { return m_InfoText2; } set { m_InfoText2 = value; InvalidateProperties(); } }

		public string m_InfoText3;
		[CommandProperty(AccessLevel.Owner)]
		public string InfoText3 { get { return m_InfoText3; } set { m_InfoText3 = value; InvalidateProperties(); } }

		public string m_InfoText4;
		[CommandProperty(AccessLevel.Owner)]
		public string InfoText4 { get { return m_InfoText4; } set { m_InfoText4 = value; InvalidateProperties(); } }

		public string m_InfoText5;
		[CommandProperty(AccessLevel.Owner)]
		public string InfoText5 { get { return m_InfoText5; } set { m_InfoText5 = value; InvalidateProperties(); } }

		public int m_Limits;
		[CommandProperty(AccessLevel.Owner)]
		public int Limits
		{
			get
			{
				return m_Limits;
			}
			set
			{
				if ( m_Limits != value )
				{
					m_Limits = value;

					if ( m_Limits < 1 && m_LimitsDelete && m_LimitsMax > 0 )
						Delete();
					else
						InvalidateProperties();
				}
			}
		}

		public int m_LimitsMax;
		[CommandProperty(AccessLevel.Owner)]
		public int LimitsMax
		{
			get
			{
				return m_LimitsMax;
			}
			set
			{
				if ( m_LimitsMax != value )
				{
					m_LimitsMax = value;
					if ( m_LimitsMax < m_Limits )
						Limits = m_LimitsMax;
					else
						InvalidateProperties();
				}
			}
		}

		public string m_LimitsName;
		[CommandProperty(AccessLevel.Owner)]
		public string LimitsName { get { return m_LimitsName; } set { m_LimitsName = value; InvalidateProperties(); } }

		public bool m_LimitsDelete;
		[CommandProperty(AccessLevel.Owner)]
		public bool LimitsDelete { get { return m_LimitsDelete; } set { m_LimitsDelete = value; InvalidateProperties(); } }

		public Mobile m_BuiltBy;
		[CommandProperty(AccessLevel.Owner)]
		public Mobile BuiltBy { get { return m_BuiltBy; } set { m_BuiltBy = value; InvalidateProperties(); } }

		public bool m_Built;
		[CommandProperty(AccessLevel.Owner)]
		public bool Built { get { return m_Built; } set { m_Built = value; InvalidateProperties(); } }

		private bool m_Purchased;
		[CommandProperty(AccessLevel.Owner)]
		public bool Purchased { get { return m_Purchased; } set { m_Purchased = value; InvalidateProperties(); } }

		public string EquipLayerName( Layer layer )
		{
			if ( layer == Layer.OneHanded )
				return "Right Hand";
			else if ( layer == Layer.TwoHanded && m_NeedsBothHands )
				return "Both Hands";
			else if ( layer == Layer.TwoHanded )
				return "Left Hand";
			else if ( layer == Layer.Shoes )
				return "Boots";
			else if ( layer == Layer.Pants )
				return "Legs";
			else if ( layer == Layer.Shirt )
				return "Chest";
			else if ( layer == Layer.Helm )
				return "Helm";
			else if ( layer == Layer.Gloves )
				return "Gloves";
			else if ( layer == Layer.Ring )
				return "Ring";
			else if ( layer == Layer.Trinket )
				return "Trinket";
			else if ( layer == Layer.Neck )
				return "Neck";
			else if ( layer == Layer.Waist )
				return "Waist";
			else if ( layer == Layer.InnerTorso )
				return "Chest";
			else if ( layer == Layer.Bracelet )
				return "Wrist";
			else if ( layer == Layer.MiddleTorso )
				return "Shirt";
			else if ( layer == Layer.Earrings )
				return "Ears";
			else if ( layer == Layer.Arms )
				return "Arms";
			else if ( layer == Layer.Cloak )
				return "Cloak";
			else if ( layer == Layer.OuterTorso )
				return "Robe";
			else if ( layer == Layer.OuterLegs )
				return "Skirt";
			else if ( layer == Layer.InnerLegs )
				return "Legs";

			return null;
		}

		public int TempFlags
		{
			get
			{
				CompactInfo info = LookupCompactInfo();

				if ( info != null )
					return info.m_TempFlags;

				return 0;
			}
			set
			{
				CompactInfo info = AcquireCompactInfo();

				info.m_TempFlags = value;

				if ( info.m_TempFlags == 0 )
					VerifyCompactInfo();
			}
		}

		public int SavedFlags
		{
			get
			{
				CompactInfo info = LookupCompactInfo();

				if ( info != null )
					return info.m_SavedFlags;

				return 0;
			}
			set
			{
				CompactInfo info = AcquireCompactInfo();

				info.m_SavedFlags = value;

				if ( info.m_SavedFlags == 0 )
					VerifyCompactInfo();
			}
		}

		/// <summary>
		/// The <see cref="Mobile" /> who is currently <see cref="Mobile.Holding">holding</see> this item.
		/// </summary>
		public Mobile HeldBy
		{
			get
			{
				CompactInfo info = LookupCompactInfo();

				if ( info != null )
					return info.m_HeldBy;

				return null;
			}
			set
			{
				CompactInfo info = AcquireCompactInfo();

				info.m_HeldBy = value;

				if ( info.m_HeldBy == null )
					VerifyCompactInfo();
			}
		}

		[Flags]
		private enum ImplFlag : byte
		{
			None			= 0x00,
			Visible			= 0x01,
			Movable			= 0x02,
			Deleted			= 0x04,
			Stackable		= 0x08,
			InQueue			= 0x10,
			Insured			= 0x20,
			PayedInsurance	= 0x40,
			QuestItem		= 0x80
		}

		private class CompactInfo
		{
			public string m_Name;

			public List<Item> m_Items;
			public BounceInfo m_Bounce;

			public Mobile m_HeldBy;
			public Mobile m_BlessedFor;

			public int m_TempFlags;
			public int m_SavedFlags;

			public double m_Weight = -1;
		}

		private CompactInfo m_CompactInfo;

		public ExpandFlag GetExpandFlags()
		{
			CompactInfo info = LookupCompactInfo();

			ExpandFlag flags = 0;

			if ( info != null )
			{
				if ( info.m_BlessedFor != null )
					flags |= ExpandFlag.Blessed;

				if ( info.m_Bounce != null )
					flags |= ExpandFlag.Bounce;

				if ( info.m_HeldBy != null )
					flags |= ExpandFlag.Holder;

				if ( info.m_Items != null )
					flags |= ExpandFlag.Items;

				if ( info.m_Name != null )
					flags |= ExpandFlag.Name;

				if ( info.m_SavedFlags != 0 )
					flags |= ExpandFlag.SaveFlag;

				if ( info.m_TempFlags != 0 )
					flags |= ExpandFlag.TempFlag;

				if ( info.m_Weight != -1 )
					flags |= ExpandFlag.Weight;
			}

			return flags;
		}

		private CompactInfo LookupCompactInfo()
		{
			return m_CompactInfo;
		}

		private CompactInfo AcquireCompactInfo()
		{
			if ( m_CompactInfo == null )
				m_CompactInfo = new CompactInfo();

			return m_CompactInfo;
		}

		private void ReleaseCompactInfo()
		{
			m_CompactInfo = null;
		}

		private void VerifyCompactInfo()
		{
			CompactInfo info = m_CompactInfo;

			if ( info == null )
				return;

			bool isValid = ( info.m_Name != null )
							|| ( info.m_Items != null )
							|| ( info.m_Bounce != null )
							|| ( info.m_HeldBy != null )
							|| ( info.m_BlessedFor != null )
							|| ( info.m_TempFlags != 0 )
							|| ( info.m_SavedFlags != 0 )
							|| ( info.m_Weight != -1 );

			if ( !isValid )
				ReleaseCompactInfo();
		}

		public List<Item> LookupItems()
		{
			if ( this is Container )
				return ( this as Container ).m_Items;

			CompactInfo info = LookupCompactInfo();

			if ( info != null )
				return info.m_Items;

			return null;
		}

		public List<Item> AcquireItems()
		{
			if ( this is Container )
			{
				Container cont = this as Container;

				if ( cont.m_Items == null ) {
					cont.m_Items = new List<Item>();
				}

				return cont.m_Items;
			}

			CompactInfo info = AcquireCompactInfo();

			info.m_Items = new List<Item>();

			return info.m_Items;
		}

		private void SetFlag( ImplFlag flag, bool value )
		{
			if ( value )
				m_Flags |= flag;
			else
				m_Flags &= ~flag;
		}

		private bool GetFlag( ImplFlag flag )
		{
			return ( (m_Flags & flag) != 0 );
		}

		public BounceInfo GetBounce()
		{
			CompactInfo info = LookupCompactInfo();

			if ( info != null )
				return info.m_Bounce;

			return null;
		}

		public void RecordBounce()
		{
			CompactInfo info = AcquireCompactInfo();

			info.m_Bounce = new BounceInfo( this );
		}

		public void ClearBounce()
		{
			CompactInfo info = LookupCompactInfo();

			if ( info != null )
			{
				BounceInfo bounce = info.m_Bounce;

				if ( bounce != null )
				{
					info.m_Bounce = null;

					if ( bounce.m_Parent is Item )
					{
						Item parent = (Item) bounce.m_Parent;

						if ( !parent.Deleted )
							parent.OnItemBounceCleared( this );
					}
					else if ( bounce.m_Parent is Mobile )
					{
						Mobile parent = (Mobile) bounce.m_Parent;

						if ( !parent.Deleted )
							parent.OnItemBounceCleared( this );
					}

					VerifyCompactInfo();
				}
			}
		}

		public void SetDefaultHue()
		{
			if ( DefaultHue > 0 && Hue == 0 && Resource == CraftResource.None )
				Hue = DefaultHue;
		}

		/// <summary>
		/// Overridable. Virtual event invoked when a client, <paramref name="from" />, invokes a 'help request' for the Item. Seemingly no longer functional in newer clients.
		/// </summary>
		public virtual void OnHelpRequest( Mobile from )
		{
		}

		/// <summary>
		/// Overridable. Method checked to see if the item can be traded.
		/// </summary>
		/// <returns>True if the trade is allowed, false if not.</returns>
		public virtual bool AllowSecureTrade( Mobile from, Mobile to, Mobile newOwner, bool accepted )
		{
			return true;
		}

		/// <summary>
		/// Overridable. Virtual event invoked when a trade has completed, either successfully or not.
		/// </summary>
		public virtual void OnSecureTrade( Mobile from, Mobile to, Mobile newOwner, bool accepted )
		{
		}

		/// <summary>
		/// Overridable. Method checked to see if the elemental resistances of this Item conflict with another Item on the <see cref="Mobile" />.
		/// </summary>
		/// <returns>
		/// <list type="table">
		/// <item>
		/// <term>True</term>
		/// <description>There is a confliction. The elemental resistance bonuses of this Item should not be applied to the <see cref="Mobile" /></description>
		/// </item>
		/// <item>
		/// <term>False</term>
		/// <description>There is no confliction. The bonuses should be applied.</description>
		/// </item>
		/// </list>
		/// </returns>
		public virtual bool CheckPropertyConfliction( Mobile m )
		{
			return false;
		}

		/// <summary>
		/// Overridable. Sends the <see cref="PropertyList">object property list</see> to <paramref name="from" />.
		/// </summary>
		public virtual void SendPropertiesTo( Mobile from )
		{
			from.Send( PropertyList );
		}

		/// <summary>
		/// Overridable. Adds the name of this item to the given <see cref="ObjectPropertyList" />. This method should be overriden if the item requires a complex naming format.
		/// </summary>
		public virtual void AddNameProperty( ObjectPropertyList list )
		{
			string name = this.Name;

			if ( name == null )
			{
				if ( m_Amount <= 1 )
					list.Add( LabelNumber );
				else
					list.Add( 1050039, "{0}\t#{1}", m_Amount, LabelNumber ); // ~1_NUMBER~ ~2_ITEMNAME~
			}
			else
			{
				if ( m_Amount <= 1 )
					list.Add( name );
				else
					list.Add( 1050039, "{0}\t{1}", m_Amount, Name ); // ~1_NUMBER~ ~2_ITEMNAME~
			}
		}

		/// <summary>
		/// Overridable. Adds the loot type of this item to the given <see cref="ObjectPropertyList" />. By default, this will be either 'blessed', 'cursed', or 'insured'.
		/// </summary>
		public virtual void AddLootTypeProperty( ObjectPropertyList list )
		{
			if ( m_LootType == LootType.Blessed )
				list.Add( 1038021 ); // blessed
			else if ( m_LootType == LootType.Cursed )
				list.Add( 1049643 ); // cursed
			else if ( Insured )
				list.Add( 1061682 ); // <b>insured</b>
		}

		/// <summary>
		/// Overridable. Adds any elemental resistances of this item to the given <see cref="ObjectPropertyList" />.
		/// </summary>
		public virtual void AddResistanceProperties( ObjectPropertyList list )
		{
			int v = PhysicalResistance;

			if ( v != 0 )
				list.Add( 1060448, v.ToString() ); // physical resist ~1_val~%

			v = FireResistance;

			if ( v != 0 )
				list.Add( 1060447, v.ToString() ); // fire resist ~1_val~%

			v = ColdResistance;

			if ( v != 0 )
				list.Add( 1060445, v.ToString() ); // cold resist ~1_val~%

			v = PoisonResistance;

			if ( v != 0 )
				list.Add( 1060449, v.ToString() ); // poison resist ~1_val~%

			v = EnergyResistance;

			if ( v != 0 )
				list.Add( 1060446, v.ToString() ); // energy resist ~1_val~%
		}

		/// <summary>
		/// Overridable. Determines whether the item will show <see cref="AddWeightProperty" />. 
		/// </summary>
		public virtual bool DisplayWeight 
		{
			get
			{
				if ( Weight <= 0 )
					return false;

				if ( !Movable && !( IsLockedDown || IsSecure ) && ItemData.Weight == 255 )
					return false;

				return true;
			} 
		}

		/// <summary>
		/// Overridable. Displays cliloc 1072788-1072789. 
		/// </summary>
		public virtual void AddWeightProperty( ObjectPropertyList list )
		{
			int weight = this.PileWeight + this.TotalWeight;

			if ( weight == 1 ) {
				list.Add( 1072788, weight.ToString() ); //Weight: ~1_WEIGHT~ stone
			} else {
				list.Add( 1072789, weight.ToString() ); //Weight: ~1_WEIGHT~ stones
			}
		}

		/// <summary>
		/// Overridable. Adds header properties. By default, this invokes <see cref="AddNameProperty" />, <see cref="AddBlessedForProperty" /> (if applicable), and <see cref="AddLootTypeProperty" /> (if <see cref="DisplayLootType" />).
		/// </summary>
		public virtual void AddNameProperties( ObjectPropertyList list )
		{
			string CHue1 = ColorHue1;
			if ( ColorHue1 == null )
				CHue1 = "FFFFFF";
			string CHue2 = ColorHue2;
			if ( ColorHue2 == null )
				CHue2 = "FFFFFF";
			string CHue3 = ColorHue3;
			if ( ColorHue3 == null )
				CHue3 = "FFFFFF";
			string CHue4 = ColorHue4;
			if ( ColorHue4 == null )
				CHue4 = "FFFFFF";
			string CHue5 = ColorHue5;
			if ( ColorHue5 == null )
				CHue5 = "FFFFFF";

			if ( ColorText1 != null )
				list.Add( 1072171, "{0}\t{1}", CHue1, ColorText1 );

			if ( ColorText2 != null )
				list.Add( 1072172, "{0}\t{1}", CHue2, ColorText2 );

			if ( ColorText1 == null || ( !IsStandardResource( Resource ) && Catalog != Catalogs.Book && Catalog != Catalogs.Trinket ) )
				AddNameProperty( list );

			if ( CoinPrice > 0 && NotIdentified && Movable )
			{
				if ( NotIDSource != Identity.None )
					list.Add( 1029759 + (int)NotIDSource );

				if ( NotIDSkill != IDSkill.None && NotIDAttempts < MAX_ID_ATTEMPTS )
					list.Add( 1029724 + (int)NotIDSkill );
			}

			if ( ColorText3 != null )
				list.Add( 1072173, "{0}\t{1}", CHue3, ColorText3 );

			if ( ColorText4 != null )
				list.Add( 1072174, "{0}\t{1}", CHue4, ColorText4 );

			if ( ColorText5 != null )
				list.Add( 1072175, "{0}\t{1}", CHue5, ColorText5 );

			switch ( ArtifactLevel )
			{
				case ArtifactLevel.LegendaryArtefact:
					list.Add( 1071128 );
					break;
				case ArtifactLevel.StandardArtefact:
					list.Add( 1070754 );
					if (ResourceCanChange())
						list.Add( "<BASEFONT COLOR=#C6D11C>May be transmuted</BASEFONT>" );
					break;
				case ArtifactLevel.Artifact:
					list.Add( 1070753 );
					break;
				case ArtifactLevel.DecorativeArtefact:
					list.Add( "<BASEFONT COLOR=#C6D11C>Decorative Artefact</BASEFONT>" );
					break;
			}

			if ( m_Enchanted != MagicSpell.None )
			{
				list.Add( 1068254 + (int)m_Enchanted );

				if ( m_EnchantUsesMax > 0 )
					list.Add( 1068253, "{0}\t{1}", m_EnchantUses.ToString(), m_EnchantUsesMax.ToString() );
				else
					list.Add( 1068254, m_EnchantUses.ToString() );

				if ( m_EnchantMod > 0 )
					list.Add( 1068252, m_EnchantMod.ToString() );
			}

			if ( InfoText1 != null )
				list.Add( 1070630, InfoText1 );

			if ( InfoText2 != null )
				list.Add( 1070631, InfoText2 );

			if ( InfoText3 != null )
				list.Add( 1070632, InfoText3 );

			if ( InfoText4 != null )
				list.Add( 1070633, InfoText4 );

			if ( InfoText5 != null )
				list.Add( 1070634, InfoText5 );

			if ( IsSecure )
				AddSecureProperty( list );
			else if ( IsLockedDown )
				AddLockedDownProperty( list );

			Mobile blessedFor = this.BlessedFor;

			if ( blessedFor != null && !blessedFor.Deleted )
				AddBlessedForProperty( list, blessedFor );

			if ( DisplayLootType )
				AddLootTypeProperty( list );

			if ( DisplayWeight )
				AddWeightProperty( list );

			if ( QuestItem )
				AddQuestItemProperty( list );

			if ( SubResource != CraftResource.None )
				list.Add( 1072041, "{0}", SubName );

			AppendChildNameProperties( list );
		}

		/// <summary>
		/// Overridable. Adds the "Quest Item" property to the given <see cref="ObjectPropertyList" />.
		/// </summary>
		public virtual void AddQuestItemProperty( ObjectPropertyList list )
		{
			list.Add( 1072351 ); // Quest Item
		}

		/// <summary>
		/// Overridable. Adds the "Locked Down & Secure" property to the given <see cref="ObjectPropertyList" />.
		/// </summary>
		public virtual void AddSecureProperty( ObjectPropertyList list )
		{
			list.Add( 501644 ); // locked down & secure
		}

		/// <summary>
		/// Overridable. Adds the "Locked Down" property to the given <see cref="ObjectPropertyList" />.
		/// </summary>
		public virtual void AddLockedDownProperty( ObjectPropertyList list )
		{
			list.Add( 501643 ); // locked down
		}

		/// <summary>
		/// Overridable. Adds the "Blessed for ~1_NAME~" property to the given <see cref="ObjectPropertyList" />.
		/// </summary>
		public virtual void AddBlessedForProperty( ObjectPropertyList list, Mobile m )
		{
			list.Add( 1062203, "{0}", m.Name ); // Blessed for ~1_NAME~
		}

		/// <summary>
		/// Overridable. Fills an <see cref="ObjectPropertyList" /> with everything applicable. By default, this invokes <see cref="AddNameProperties" />, then <see cref="Item.GetChildProperties">Item.GetChildProperties</see> or <see cref="Mobile.GetChildProperties">Mobile.GetChildProperties</see>. This method should be overriden to add any custom properties.
		/// </summary>
		public virtual void GetProperties( ObjectPropertyList list )
		{
			AddNameProperties( list );

			if ( LimitsMax > 0 )
				list.Add( 1060584, "{0}\t{1}", Limits.ToString(), m_LimitsName );
		}

		/// <summary>
		/// Overridable. Event invoked when a child (<paramref name="item" />) is building it's <see cref="ObjectPropertyList" />. Recursively calls <see cref="Item.GetChildProperties">Item.GetChildProperties</see> or <see cref="Mobile.GetChildProperties">Mobile.GetChildProperties</see>.
		/// </summary>
		public virtual void GetChildProperties( ObjectPropertyList list, Item item )
		{
			if ( m_Parent is Item )
				((Item)m_Parent).GetChildProperties( list, item );
			else if ( m_Parent is Mobile )
				((Mobile)m_Parent).GetChildProperties( list, item );
		}

		/// <summary>
		/// Overridable. Event invoked when a child (<paramref name="item" />) is building it's Name <see cref="ObjectPropertyList" />. Recursively calls <see cref="Item.GetChildProperties">Item.GetChildNameProperties</see> or <see cref="Mobile.GetChildProperties">Mobile.GetChildNameProperties</see>.
		/// </summary>
		public virtual void GetChildNameProperties( ObjectPropertyList list, Item item )
		{
			if ( m_Parent is Item )
				((Item)m_Parent).GetChildNameProperties( list, item );
			else if ( m_Parent is Mobile )
				((Mobile)m_Parent).GetChildNameProperties( list, item );
		}

		public virtual bool IsChildVisibleTo( Mobile m, Item child )
		{
			return true;
		}

		public void Bounce( Mobile from )
		{
			if ( m_Parent is Item )
				((Item)m_Parent).RemoveItem( this );
			else if ( m_Parent is Mobile )
				((Mobile)m_Parent).RemoveItem( this );

			m_Parent = null;

			BounceInfo bounce = this.GetBounce();

			if ( bounce != null )
			{
				object parent = bounce.m_Parent;

				if ( parent is Item && !((Item)parent).Deleted )
				{
					Item p = (Item)parent;
					object root = p.RootParent;
					if ( p.IsAccessibleTo( from ) && ( !(root is Mobile) || ((Mobile)root).CheckNonlocalDrop( from, this, p ) ) )
					{
						Location = bounce.m_Location;
						p.AddItem( this );
					}
					else
					{
						MoveToWorld( from.Location, from.Map );
					}
				}
				else if ( parent is Mobile && !((Mobile)parent).Deleted )
				{
					if ( !((Mobile)parent).EquipItem( this ) )
						MoveToWorld( bounce.m_WorldLoc, bounce.m_Map );
				}
				else
				{
					MoveToWorld( bounce.m_WorldLoc, bounce.m_Map );
				}

				ClearBounce();
			}
			else
			{
				MoveToWorld( from.Location, from.Map );
			}
		}

		/// <summary>
		/// Overridable. Method checked to see if this item may be equiped while casting a spell. By default, this returns false. It is overriden on spellbook and spell channeling weapons or shields.
		/// </summary>
		/// <returns>True if it may, false if not.</returns>
		/// <example>
		/// <code>
		///	public override bool AllowEquipedCast( Mobile from )
		///	{
		///		if ( from.Int &gt;= 100 )
		///			return true;
		///		
		///		return base.AllowEquipedCast( from );
		/// }</code>
		/// 
		/// When placed in an Item script, the item may be cast when equiped if the <paramref name="from" /> has 100 or more intelligence. Otherwise, it will drop to their backpack.
		/// </example>
		public virtual bool AllowEquipedCast( Mobile from )
		{
			return false;
		}

		public virtual bool CheckConflictingLayer( Mobile m, Item item, Layer layer )
		{
			return ( m_Layer == layer );
		}

		public virtual bool CanEquip( Mobile m )
		{
			return ( m_Layer != Layer.Invalid && m.FindItemOnLayer( m_Layer ) == null );
		}

		public virtual void GetChildContextMenuEntries( Mobile from, List<ContextMenuEntry> list, Item item )
		{
			if ( m_Parent is Item )
				((Item)m_Parent).GetChildContextMenuEntries( from, list, item );
			else if ( m_Parent is Mobile )
				((Mobile)m_Parent).GetChildContextMenuEntries( from, list, item );
		}

		public virtual void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( m_Parent is Item )
				((Item)m_Parent).GetChildContextMenuEntries( from, list, this );
			else if ( m_Parent is Mobile )
				((Mobile)m_Parent).GetChildContextMenuEntries( from, list, this );

			if ( m_Enchanted != MagicSpell.None && m_EnchantUses > 0 )
				list.Add( new EnchantCall( from, this ) );

			if ( m_InfoData != null )
				list.Add( new InfoDataEntry( from, this ) );

			if ( m_CoinPrice > 0 && NotIdentified && NotIDAttempts < MAX_ID_ATTEMPTS )
				list.Add( new IDEntry( from, this, NotIDSkill ) );
		}

		public class InfoDataEntry : ContextMenuEntry
		{
			private Item m_Item;
			private Mobile m_From;

			public InfoDataEntry( Mobile from, Item item ) : base( 256, 3 )
			{
				m_From = from;
				m_Item = item;
			}

			public override void OnClick()
			{
				m_From.CloseGump( typeof( InfoDataGump ) );
				m_From.SendGump( new InfoDataGump( m_From, m_Item ) ); 
			}
		}

		public class IDEntry : ContextMenuEntry
		{
			private Item m_Item;
			private Mobile m_From;
			private IDSkill m_Skill;

			public IDEntry( Mobile from, Item item, IDSkill skill ) : base( 256+(int)skill, 3 )
			{
				m_From = from;
				m_Item = item;
				m_Skill = skill;
			}

			public override void OnClick()
			{
				m_Item.IDCommand( m_From );
			}
		}

		public virtual void IDCommand( Mobile m )
		{
		}

		public class InfoDataGump : Gump
		{
			public InfoDataGump( Mobile from, Item item ) : base( 50, 50 )
			{
				Closable=true;
				Disposable=true;
				Dragable=true;
				Resizable=false;
				AddImage(0, 0, 9613);
				TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;
				AddHtml( 12, 10, 311, 241, @"<BODY><BASEFONT Color=#FFFFFF>" + cultInfo.ToTitleCase(item.Name) + "<BR><BR>" + item.InfoData + "</BASEFONT></BODY>", (bool)false, (bool)true);
			}
		}

		public class EnchantCall : ContextMenuEntry
		{
			private Mobile e_Mobile;
			private Item e_Item;
			
			public EnchantCall( Mobile from, Item item ) : base( 6097, 3 )
			{
				e_Mobile = from;
				e_Item = item;
			}

			public override void OnClick()
			{
				e_Item.CastEnchantment( e_Mobile );
            }
        }

		public virtual void CastEnchantment( Mobile from )
		{
			from.SendMessage("The item seems to do nothing.");
		}

		public bool ResourceCanChange()
		{
			if ( NotModAble ) return false;

			if ( ArtifactLevel != ArtifactLevel.None )
			{
				// Only Standard artefacts can be changed
				if ( ArtifactLevel != ArtifactLevel.StandardArtefact ) return false;

				switch ( Resource )
				{
					// Can only be changed if it's currently a basic resource
					case CraftResource.None:
					case CraftResource.Iron:
					case CraftResource.Fabric:
					case CraftResource.RegularLeather:
					case CraftResource.RegularWood:
					case CraftResource.BrittleSkeletal:
						return true;

					default:
						return false;
				}
			}

			return true;
		}

		public virtual void ResourceChanged( CraftResource resource )
		{
			m_Resource = resource;
		}

		public virtual void SubResourceChanged( CraftResource resource )
		{
		}

		public virtual void MagicSpellChanged( MagicSpell spell )
		{
		}

		public virtual void ItemPriced( int val )
		{
			m_CoinPrice = val;
		}

		public virtual void ItemIdentified( bool id )
		{
			m_NotIdentified = id;
		}

		public virtual bool VerifyMove( Mobile from )
		{
			return Movable;
		}

		public virtual DeathMoveResult OnParentDeath( Mobile parent )
		{
			if ( !Movable )
				return DeathMoveResult.RemainEquiped;
			else if ( parent.KeepsItemsOnDeath )
				return DeathMoveResult.MoveToBackpack;
			else if ( CheckBlessed( parent ) )
				return DeathMoveResult.MoveToBackpack;
			else if ( CheckNewbied() && parent.Kills < 5 )
				return DeathMoveResult.MoveToBackpack;
			else if( parent.Player && Nontransferable )
				return DeathMoveResult.MoveToBackpack;
			else
				return DeathMoveResult.MoveToCorpse;
		}

		public virtual DeathMoveResult OnInventoryDeath( Mobile parent )
		{
			if ( !Movable )
				return DeathMoveResult.MoveToBackpack;
			else if ( parent.KeepsItemsOnDeath )
				return DeathMoveResult.MoveToBackpack;
			else if ( CheckBlessed( parent ) )
				return DeathMoveResult.MoveToBackpack;
			else if ( CheckNewbied() && parent.Kills < 5 )
				return DeathMoveResult.MoveToBackpack;
			else if( parent.Player && Nontransferable )
				return DeathMoveResult.MoveToBackpack;
			else
				return DeathMoveResult.MoveToCorpse;
		}

		/// <summary>
		/// Moves the Item to <paramref name="location" />. The Item does not change maps.
		/// </summary>
		public virtual void MoveToWorld( Point3D location )
		{
			MoveToWorld( location, m_Map );
		}

		public void LabelTo( Mobile to, int number )
		{
			to.Send( new MessageLocalized( m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, number, "", "" ) );
		}

		public void LabelTo( Mobile to, int number, string args )
		{
			to.Send( new MessageLocalized( m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, number, "", args ) );
		}

		public void LabelTo( Mobile to, string text )
		{
			to.Send( new UnicodeMessage( m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, "ENU", "", text ) );
		}

		public void LabelTo( Mobile to, string format, params object[] args )
		{
			LabelTo( to, String.Format( format, args ) );
		}

		public void LabelToAffix( Mobile to, int number, AffixType type, string affix )
		{
			to.Send( new MessageLocalizedAffix( m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, number, "", type, affix, "" ) );
		}

		public void LabelToAffix( Mobile to, int number, AffixType type, string affix, string args )
		{
			to.Send( new MessageLocalizedAffix( m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, number, "", type, affix, args ) );
		}

		public virtual void LabelLootTypeTo( Mobile to )
		{
			if ( m_LootType == LootType.Blessed )
				LabelTo( to, 1041362 ); // (blessed)
			else if ( m_LootType == LootType.Cursed )
				LabelTo( to, "(cursed)" );
		}

		public bool AtWorldPoint( int x, int y )
		{
			return ( m_Parent == null && m_Location.m_X == x && m_Location.m_Y == y );
		}

		public bool AtPoint( int x, int y )
		{
			return ( m_Location.m_X == x && m_Location.m_Y == y );
		}

		/// <summary>
		/// Moves the Item to a given <paramref name="location" /> and <paramref name="map" />.
		/// </summary>
		public void MoveToWorld( Point3D location, Map map )
		{
			if ( Deleted )
				return;

			DefaultMainHue( this );

			Point3D oldLocation = GetWorldLocation();
			Point3D oldRealLocation = m_Location;

			SetLastMoved();

			if ( Parent is Mobile )
				((Mobile)Parent).RemoveItem( this );
			else if ( Parent is Item )
				((Item)Parent).RemoveItem( this );

			if ( !Object.ReferenceEquals( m_Map, map ) )
			{
				Map old = m_Map;

				if ( m_Map != null )
				{
					m_Map.OnLeave( this );

					if ( oldLocation.m_X != 0 )
					{
						Packet remPacket = null;

						IPooledEnumerable eable = m_Map.GetClientsInRange( oldLocation, GetMaxUpdateRange() );

						foreach ( NetState state in eable )
						{
							Mobile m = state.Mobile;

							if ( m.InRange( oldLocation, GetUpdateRange( m ) ) )
							{
								if ( remPacket == null )
									remPacket = this.RemovePacket;

								state.Send( remPacket );
							}
						}

						eable.Free();
					}
				}

				m_Location = location;
				this.OnLocationChange( oldRealLocation );

				ReleaseWorldPackets();

				List<Item> items = LookupItems();

				if ( items != null )
				{
					for ( int i = 0; i < items.Count; ++i )
						items[i].Map = map;
				}

				m_Map = map;

				if ( m_Map != null )
					m_Map.OnEnter( this );

				OnMapChange();

				if ( m_Map != null )
				{
					IPooledEnumerable eable = m_Map.GetClientsInRange( m_Location, GetMaxUpdateRange() );

					foreach ( NetState state in eable )
					{
						Mobile m = state.Mobile;

						if ( m.CanSee( this ) && m.InRange( m_Location, GetUpdateRange( m ) ) )
							SendInfoTo( state );
					}

					eable.Free();
				}

				RemDelta( ItemDelta.Update );

				if ( old == null || old == Map.Internal )
					InvalidateProperties();
			}
			else if ( m_Map != null )
			{
				IPooledEnumerable eable;

				if ( oldLocation.m_X != 0 )
				{
					Packet removeThis = null;

					eable = m_Map.GetClientsInRange( oldLocation, GetMaxUpdateRange() );

					foreach ( NetState state in eable )
					{
						Mobile m = state.Mobile;

						if ( !m.InRange( location, GetUpdateRange( m ) ) )
						{
							if ( removeThis == null )
								removeThis = this.RemovePacket;

							state.Send( removeThis );
						}
					}

					eable.Free();
				}

				Point3D oldInternalLocation = m_Location;

				m_Location = location;
				this.OnLocationChange( oldRealLocation );

				ReleaseWorldPackets();

				eable = m_Map.GetClientsInRange( m_Location, GetMaxUpdateRange() );

				foreach ( NetState state in eable )
				{
					Mobile m = state.Mobile;

					if ( m.CanSee( this ) && m.InRange( m_Location, GetUpdateRange( m ) ) )
						SendInfoTo( state );
				}

				eable.Free();

				m_Map.OnMove( oldInternalLocation, this );

				RemDelta( ItemDelta.Update );
			}
			else
			{
				Map = map;
				Location = location;
			}
		}

		/// <summary>
		/// Has the item been deleted?
		/// </summary>
		public bool Deleted{ get{ return GetFlag( ImplFlag.Deleted ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public LootType LootType
		{
			get
			{
				return m_LootType;
			}
			set
			{
				if ( m_LootType != value )
				{
					m_LootType = value;

					if ( DisplayLootType )
						InvalidateProperties();
				}
			}
		}

		private static TimeSpan m_DDT = TimeSpan.FromHours( 1.0 );

		public static TimeSpan DefaultDecayTime{ get{ return m_DDT; } set{ m_DDT = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual TimeSpan DecayTime
		{
			get
			{
				return m_DDT;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual bool Decays
		{
			get
			{
				return (Movable && Visible);
			}
		}

		public virtual bool OnDecay()
		{
			return ( Decays && Parent == null && Map != Map.Internal && Region.Find( Location, Map ).OnDecay( this ) );
		}

		public void SetLastMoved()
		{
			m_LastMovedTime = DateTime.Now;
		}

		public DateTime LastMoved
		{
			get
			{
				return m_LastMovedTime;
			}
			set
			{
				m_LastMovedTime = value;
			}
		}

		public bool StackWith( Mobile from, Item dropped )
		{
			return StackWith( from, dropped, true );
		}

		public virtual bool StackWith( Mobile from, Item dropped, bool playSound )
		{
			if ( dropped == this ) return true; // Safeguard. Only programmatically possible... like when Harvesting.

			if ( dropped.Stackable && Stackable && dropped.GetType() == GetType() && dropped.ItemID == ItemID && dropped.Hue == Hue && dropped.Name == Name && (dropped.Amount + Amount) <= 60000 )
			{
				if ( m_LootType != dropped.m_LootType )
					m_LootType = LootType.Regular;

				Amount += dropped.Amount;
				dropped.Delete();

				if ( playSound && from != null )
				{
					int soundID = GetDropSound();

					if ( soundID == -1 )
						soundID = 0x42;

					from.SendSound( soundID, GetWorldLocation() );
				}

				return true;
			}

			return false;
		}

		public virtual bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( Parent is Container )
				return ((Container)Parent).OnStackAttempt( from, this, dropped );

			return StackWith( from, dropped );
		}

		public Rectangle2D GetGraphicBounds()
		{
			int itemID = m_ItemID;
			bool doubled = m_Amount > 1;

			if ( itemID >= 0xEEA && itemID <= 0xEF2 ) // Are we coins?
			{
				int coinBase = (itemID - 0xEEA) / 3;
				coinBase *= 3;
				coinBase += 0xEEA;

				doubled = false;

				if ( m_Amount <= 1 )
				{
					// A single coin
					itemID = coinBase;
				}
				else if ( m_Amount <= 5 )
				{
					// A stack of coins
					itemID = coinBase + 1;
				}
				else // m_Amount > 5
				{
					// A pile of coins
					itemID = coinBase + 2;
				}
			}

			Rectangle2D bounds = ItemBounds.Table[itemID & 0x3FFF];

			if ( doubled )
			{
				bounds.Set( bounds.X, bounds.Y, bounds.Width + 5, bounds.Height + 5 );
			}

			return bounds;
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Stackable
		{
			get{ return GetFlag( ImplFlag.Stackable ); }
			set{ SetFlag( ImplFlag.Stackable, value ); }
		}

		public Packet RemovePacket
		{
			get
			{
				if ( m_RemovePacket == null )
				{
					m_RemovePacket = new RemoveItem( this );
					m_RemovePacket.SetStatic();
				}

				return m_RemovePacket;
			}
		}

		public Packet OPLPacket
		{
			get
			{
				if ( m_OPLPacket == null )
				{
					m_OPLPacket = new OPLInfo( PropertyList );
					m_OPLPacket.SetStatic();
				}

				return m_OPLPacket;
			}
		}

		public ObjectPropertyList PropertyList
		{
			get
			{
				if ( m_PropertyList == null )
				{
					m_PropertyList = new ObjectPropertyList( this );

					GetProperties( m_PropertyList );
					AppendChildProperties( m_PropertyList );

					m_PropertyList.Terminate();
					m_PropertyList.SetStatic();
				}

				return m_PropertyList;
			}
		}

		public virtual void AppendChildProperties( ObjectPropertyList list )
		{
			if ( m_Parent is Item )
				((Item)m_Parent).GetChildProperties( list, this );
			else if ( m_Parent is Mobile )
				((Mobile)m_Parent).GetChildProperties( list, this );
		}

		public virtual void AppendChildNameProperties( ObjectPropertyList list )
		{
			if ( m_Parent is Item )
				((Item)m_Parent).GetChildNameProperties( list, this );
			else if ( m_Parent is Mobile )
				((Mobile)m_Parent).GetChildNameProperties( list, this );
		}

		public void ClearProperties()
		{
			Packet.Release( ref m_PropertyList );
			Packet.Release( ref m_OPLPacket );
		}

		public void InvalidateProperties()
		{
			if ( !ObjectPropertyList.Enabled )
				return;

			if ( m_Map != null && m_Map != Map.Internal && !World.Loading )
			{
				ObjectPropertyList oldList = m_PropertyList;
				m_PropertyList = null;
				ObjectPropertyList newList = PropertyList;

				if ( oldList == null || oldList.Hash != newList.Hash )
				{
					Packet.Release( ref m_OPLPacket );
					Delta( ItemDelta.Properties );
				}
			}
			else
			{
				ClearProperties();
			}
		}

		public Packet WorldPacket
		{
			get
			{
				// This needs to be invalidated when any of the following changes:
				//  - ItemID
				//  - Amount
				//  - Location
				//  - Hue
				//  - Packet Flags
				//  - Direction

				if ( m_WorldPacket == null )
				{
					m_WorldPacket = new WorldItem( this );
					m_WorldPacket.SetStatic();
				}

				return m_WorldPacket;
			}
		}

		public Packet WorldPacketSA
		{
			get
			{
				// This needs to be invalidated when any of the following changes:
				//  - ItemID
				//  - Amount
				//  - Location
				//  - Hue
				//  - Packet Flags
				//  - Direction

				if ( m_WorldPacketSA == null )
				{
					m_WorldPacketSA = new WorldItemSA( this );
					m_WorldPacketSA.SetStatic();
				}

				return m_WorldPacketSA;
			}
		}

		public Packet WorldPacketHS
		{
			get
			{
				// This needs to be invalidated when any of the following changes:
				//  - ItemID
				//  - Amount
				//  - Location
				//  - Hue
				//  - Packet Flags
				//  - Direction

				if ( m_WorldPacketHS == null )
				{
					m_WorldPacketHS = new WorldItemHS( this );
					m_WorldPacketHS.SetStatic();
				}

				return m_WorldPacketHS;
			}
		}

		public void ReleaseWorldPackets()
		{
			Packet.Release( ref m_WorldPacket );
			Packet.Release( ref m_WorldPacketSA );
			Packet.Release( ref m_WorldPacketHS );
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Visible
		{
			get{ return GetFlag( ImplFlag.Visible ); }
			set
			{
				if ( GetFlag( ImplFlag.Visible ) != value )
				{
					SetFlag( ImplFlag.Visible, value );
					ReleaseWorldPackets();

					if ( m_Map != null )
					{
						Packet removeThis = null;
						Point3D worldLoc = GetWorldLocation();

						IPooledEnumerable eable = m_Map.GetClientsInRange( worldLoc, GetMaxUpdateRange() );

						foreach ( NetState state in eable )
						{
							Mobile m = state.Mobile;

							if ( !m.CanSee( this ) && m.InRange( worldLoc, GetUpdateRange( m ) ) )
							{
								if ( removeThis == null )
									removeThis = this.RemovePacket;

								state.Send( removeThis );
							}
						}

						eable.Free();
					}

					Delta( ItemDelta.Update );
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Movable
		{
			get{ return GetFlag( ImplFlag.Movable ); }
			set
			{
				if ( GetFlag( ImplFlag.Movable ) != value )
				{
					SetFlag( ImplFlag.Movable, value );
					ReleaseWorldPackets();
					Delta( ItemDelta.Update );
					InvalidateProperties();
				}
			}
		}

		public virtual bool ForceShowProperties{ get{ return false; } }

		public virtual int GetPacketFlags()
		{
			int flags = 0;

			if ( !Visible )
				flags |= 0x80;

			if ( Movable || ForceShowProperties )
				flags |= 0x20;

			return flags;
		}

		public virtual bool OnMoveOff( Mobile m )
		{
			return true;
		}

		public virtual bool OnMoveOver( Mobile m )
		{
			return true;
		}

		public virtual bool HandlesOnMovement{ get{ return false; } }

		public virtual void OnMovement( Mobile m, Point3D oldLocation )
		{
		}

		public void Internalize()
		{
			MoveToWorld( Point3D.Zero, Map.Internal );
		}

		public virtual void OnMapChange()
		{
		}

		public virtual void OnRemoved( object parent )
		{
		}

		public virtual void OnAdded( object parent )
		{
			DefaultMainHue( this );
			SyncItem();
		}

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public Map Map
		{
			get
			{
				return m_Map;
			}
			set
			{
				if ( !Object.ReferenceEquals( m_Map, value ) )
				{
					Map old = m_Map;

					if ( m_Map != null && m_Parent == null )
					{
						m_Map.OnLeave( this );
						SendRemovePacket();
					}

					List<Item> items = LookupItems();

					if ( items != null )
					{
						for ( int i = 0; i < items.Count; ++i )
							items[i].Map = value;
					}

					m_Map = value;

					if ( m_Map != null && m_Parent == null )
						m_Map.OnEnter( this );

					Delta( ItemDelta.Update );

					this.OnMapChange();

					if ( old == null || old == Map.Internal )
						InvalidateProperties();
				}
			}
		}

		[Flags]
		private enum SaveFlag
		{
			None			= 0x00000000,
			Direction		= 0x00000001,
			Bounce			= 0x00000002,
			LootType		= 0x00000004,
			LocationFull	= 0x00000008,
			ItemID			= 0x00000010,
			Hue				= 0x00000020,
			Amount			= 0x00000040,
			Layer			= 0x00000080,
			Name			= 0x00000100,
			Parent			= 0x00000200,
			Items			= 0x00000400,
			WeightNot1or0	= 0x00000800,
			Map				= 0x00001000,
			Visible			= 0x00002000,
			Movable			= 0x00004000,
			Stackable		= 0x00008000,
			WeightIs0		= 0x00010000,
			LocationSByteZ	= 0x00020000,
			LocationShortXY = 0x00040000,
			LocationByteXY	= 0x00080000,
			ImplFlags		= 0x00100000,
			InsuredFor		= 0x00200000,
			BlessedFor		= 0x00400000,
			HeldBy			= 0x00800000,
			IntWeight		= 0x01000000,
			SavedFlags		= 0x02000000,
			NullWeight		= 0x04000000
		}

		private static void SetSaveFlag( ref SaveFlag flags, SaveFlag toSet, bool setIf )
		{
			if ( setIf )
				flags |= toSet;
		}

		private static bool GetSaveFlag( SaveFlag flags, SaveFlag toGet )
		{
			return ( (flags & toGet) != 0 );
		}

		int ISerializable.TypeReference {
			get { return m_TypeRef; }
		}

		int ISerializable.SerialIdentity {
			get { return m_Serial; }
		}

		public virtual void Serialize( GenericWriter writer )
		{
			writer.Write( 14 ); // version

			writer.Write( Purchased );
			writer.Write( EnchantMod );
			writer.Write( ColorHue1 );
			writer.Write( ColorText1 );
			writer.Write( ColorHue2 );
			writer.Write( ColorText2 );
			writer.Write( ColorHue3 );
			writer.Write( ColorText3 );
			writer.Write( ColorHue4 );
			writer.Write( ColorText4 );
			writer.Write( ColorHue5 );
			writer.Write( ColorText5 );
			writer.Write( WorldItemID );
			writer.Write( Technology );
			writer.Write( VirtualContainer );
			writer.Write( NotIdentified );
			writer.Write( NotIDAttempts );
			writer.WriteEncodedInt( (int) m_NotIDSource );
			writer.WriteEncodedInt( (int) m_NotIDSkill );
			writer.WriteEncodedInt( (int) m_Catalog );
			writer.Write( CoinPrice );
			writer.WriteEncodedInt( (int) m_Resource );
			writer.WriteEncodedInt( (int) m_SubResource );
			writer.Write( m_SubName );
			writer.Write( (int)ArtifactLevel );
			writer.Write( NotModAble );
			writer.Write( NeedsBothHands );
			writer.Write( InfoData );
			writer.Write( InfoText1 );
			writer.Write( InfoText2 );
			writer.Write( InfoText3 );
			writer.Write( InfoText4 );
			writer.Write( InfoText5 );
			writer.Write( Limits );
			writer.Write( LimitsMax );
			writer.Write( LimitsName );
			writer.Write( LimitsDelete );
			writer.Write( BuiltBy );
			writer.Write( Built );
			writer.WriteEncodedInt( (int) m_Enchanted );
			writer.Write( EnchantUses );
			writer.Write( EnchantUsesMax );
			writer.Write( GraphicID );
			writer.Write( GraphicHue );
			writer.Write( (Mobile)LastMobile);
			writer.Write( LastMobileName );

			SaveFlag flags = SaveFlag.None;

			int x = m_Location.m_X, y = m_Location.m_Y, z = m_Location.m_Z;

			if ( x != 0 || y != 0 || z != 0 )
			{
				if ( x >= short.MinValue && x <= short.MaxValue && y >= short.MinValue && y <= short.MaxValue && z >= sbyte.MinValue && z <= sbyte.MaxValue )
				{
					if ( x != 0 || y != 0 )
					{
						if ( x >= byte.MinValue && x <= byte.MaxValue && y >= byte.MinValue && y <= byte.MaxValue )
							flags |= SaveFlag.LocationByteXY;
						else
							flags |= SaveFlag.LocationShortXY;
					}

					if ( z != 0 )
						flags |= SaveFlag.LocationSByteZ;
				}
				else
				{
					flags |= SaveFlag.LocationFull;
				}
			}

			CompactInfo info = LookupCompactInfo();
			List<Item> items = LookupItems();

			if ( m_Direction != Direction.North )
				flags |= SaveFlag.Direction;
			if ( info != null && info.m_Bounce != null )
				flags |= SaveFlag.Bounce;
			if ( m_LootType != LootType.Regular )
				flags |= SaveFlag.LootType;
			if ( m_ItemID != 0 )
				flags |= SaveFlag.ItemID;
			if ( m_Hue != 0 )
				flags |= SaveFlag.Hue;
			if ( m_Amount != 1 )
				flags |= SaveFlag.Amount;
			if ( m_Layer != Layer.Invalid )
				flags |= SaveFlag.Layer;
			if ( info != null && info.m_Name != null )
				flags |= SaveFlag.Name;
			if ( m_Parent != null )
				flags |= SaveFlag.Parent;
			if ( items != null && items.Count > 0 )
				flags |= SaveFlag.Items;
			if ( m_Map != Map.Internal )
				flags |= SaveFlag.Map;
			//if ( m_InsuredFor != null && !m_InsuredFor.Deleted )
				//flags |= SaveFlag.InsuredFor;
			if ( info != null && info.m_BlessedFor != null && !info.m_BlessedFor.Deleted )
				flags |= SaveFlag.BlessedFor;
			if ( info != null && info.m_HeldBy != null && !info.m_HeldBy.Deleted )
				flags |= SaveFlag.HeldBy;
			if ( info != null && info.m_SavedFlags != 0 )
				flags |= SaveFlag.SavedFlags;

			if ( info == null || info.m_Weight == -1 )
			{
				flags |= SaveFlag.NullWeight;
			}
			else
			{
				if ( info.m_Weight == 0.0 )
				{
					flags |= SaveFlag.WeightIs0;
				}
				else if ( info.m_Weight != 1.0 )
				{
					if ( info.m_Weight == (int) info.m_Weight )
						flags |= SaveFlag.IntWeight;
					else
						flags |= SaveFlag.WeightNot1or0;
				}
			}

			ImplFlag implFlags = ( m_Flags & ( ImplFlag.Visible | ImplFlag.Movable | ImplFlag.Stackable | ImplFlag.Insured | ImplFlag.PayedInsurance | ImplFlag.QuestItem ) );

			if ( implFlags != ( ImplFlag.Visible | ImplFlag.Movable ) )
				flags |= SaveFlag.ImplFlags;

			writer.Write( (int) flags );

			/* begin last moved time optimization */
			long ticks = m_LastMovedTime.Ticks;
			long now = DateTime.Now.Ticks;

			TimeSpan d;

			try { d = new TimeSpan( ticks - now ); }
			catch { if ( ticks < now ) d = TimeSpan.MaxValue; else d = TimeSpan.MaxValue; }

			double minutes = -d.TotalMinutes;

			if ( minutes < int.MinValue )
				minutes = int.MinValue;
			else if ( minutes > int.MaxValue )
				minutes = int.MaxValue;

			writer.WriteEncodedInt( (int) minutes );
			/* end */

			if ( GetSaveFlag( flags, SaveFlag.Direction ) )
				writer.Write( (byte) m_Direction );

			if ( GetSaveFlag( flags, SaveFlag.Bounce ) )
				BounceInfo.Serialize( info.m_Bounce, writer );

			if ( GetSaveFlag( flags, SaveFlag.LootType ) )
				writer.Write( (byte) m_LootType );

			if ( GetSaveFlag( flags, SaveFlag.LocationFull ) )
			{
				writer.WriteEncodedInt( x );
				writer.WriteEncodedInt( y );
				writer.WriteEncodedInt( z );
			}
			else
			{
				if ( GetSaveFlag( flags, SaveFlag.LocationByteXY ) )
				{
					writer.Write( (byte) x );
					writer.Write( (byte) y );
				}
				else if ( GetSaveFlag( flags, SaveFlag.LocationShortXY ) )
				{
					writer.Write( (short) x );
					writer.Write( (short) y );
				}

				if ( GetSaveFlag( flags, SaveFlag.LocationSByteZ ) )
					writer.Write( (sbyte) z );
			}

			if ( GetSaveFlag( flags, SaveFlag.ItemID ) )
				writer.WriteEncodedInt( (int) m_ItemID );

			if ( GetSaveFlag( flags, SaveFlag.Hue ) )
				writer.WriteEncodedInt( (int) m_Hue );

			if ( GetSaveFlag( flags, SaveFlag.Amount ) )
				writer.WriteEncodedInt( (int) m_Amount );

			if ( GetSaveFlag( flags, SaveFlag.Layer ) )
				writer.Write( (byte) m_Layer );

			if ( GetSaveFlag( flags, SaveFlag.Name ) )
				writer.Write( (string) info.m_Name );

			if ( GetSaveFlag( flags, SaveFlag.Parent ) )
			{
				if ( m_Parent is Mobile && !( (Mobile) m_Parent ).Deleted )
					writer.Write( ( (Mobile) m_Parent ).Serial );
				else if ( m_Parent is Item && !( (Item) m_Parent ).Deleted )
					writer.Write( ( (Item) m_Parent ).Serial );
				else
					writer.Write( (int) Serial.MinusOne );
			}

			if ( GetSaveFlag( flags, SaveFlag.Items ) )
				writer.Write( items, false );

			if ( GetSaveFlag( flags, SaveFlag.IntWeight ) )
				writer.WriteEncodedInt( (int) info.m_Weight );
			else if ( GetSaveFlag( flags, SaveFlag.WeightNot1or0 ) )
				writer.Write( (double) info.m_Weight );

			if ( GetSaveFlag( flags, SaveFlag.Map ) )
				writer.Write( (Map) m_Map );

			if ( GetSaveFlag( flags, SaveFlag.ImplFlags ) )
				writer.WriteEncodedInt( (int) implFlags );

			if ( GetSaveFlag( flags, SaveFlag.InsuredFor ) )
				writer.Write( (Mobile) null );

			if ( GetSaveFlag( flags, SaveFlag.BlessedFor ) )
				writer.Write( info.m_BlessedFor );

			if ( GetSaveFlag( flags, SaveFlag.HeldBy ) )
				writer.Write( info.m_HeldBy );

			if ( GetSaveFlag( flags, SaveFlag.SavedFlags ) )
				writer.WriteEncodedInt( info.m_SavedFlags );
		}

		public IPooledEnumerable GetObjectsInRange( int range )
		{
			Map map = m_Map;

			if ( map == null )
				return Server.Map.NullEnumerable.Instance;

			if ( m_Parent == null )
				return map.GetObjectsInRange( m_Location, range );

			return map.GetObjectsInRange( GetWorldLocation(), range );
		}

		public IPooledEnumerable GetItemsInRange( int range )
		{
			Map map = m_Map;

			if ( map == null )
				return Server.Map.NullEnumerable.Instance;

			if ( m_Parent == null )
				return map.GetItemsInRange( m_Location, range );

			return map.GetItemsInRange( GetWorldLocation(), range );
		}

		public IPooledEnumerable GetMobilesInRange( int range )
		{
			Map map = m_Map;

			if ( map == null )
				return Server.Map.NullEnumerable.Instance;

			if ( m_Parent == null )
				return map.GetMobilesInRange( m_Location, range );

			return map.GetMobilesInRange( GetWorldLocation(), range );
		}

		public IPooledEnumerable GetClientsInRange( int range )
		{
			Map map = m_Map;

			if ( map == null )
				return Server.Map.NullEnumerable.Instance;

			if ( m_Parent == null )
				return map.GetClientsInRange( m_Location, range );

			return map.GetClientsInRange( GetWorldLocation(), range );
		}

		private static int m_LockedDownFlag;
		private static int m_SecureFlag;

		public static int LockedDownFlag
		{
			get{ return m_LockedDownFlag; }
			set{ m_LockedDownFlag = value; }
		}

		public static int SecureFlag
		{
			get{ return m_SecureFlag; }
			set{ m_SecureFlag = value; }
		}

		public bool IsLockedDown
		{
			get{ return GetTempFlag( m_LockedDownFlag ); }
			set{ SetTempFlag( m_LockedDownFlag, value ); InvalidateProperties(); }
		}

		public bool IsSecure
		{
			get{ return GetTempFlag( m_SecureFlag ); }
			set{ SetTempFlag( m_SecureFlag, value ); InvalidateProperties(); }
		}

		public bool GetTempFlag( int flag )
		{
			CompactInfo info = LookupCompactInfo();

			if ( info == null )
				return false;

			return ( (info.m_TempFlags & flag) != 0 );
		}

		public void SetTempFlag( int flag, bool value )
		{
			CompactInfo info = AcquireCompactInfo();

			if ( value )
				info.m_TempFlags |= flag;
			else
				info.m_TempFlags &= ~flag;

			if ( info.m_TempFlags == 0 )
				VerifyCompactInfo();
		}

		public bool GetSavedFlag( int flag )
		{
			CompactInfo info = LookupCompactInfo();

			if ( info == null )
				return false;

			return ( ( info.m_SavedFlags & flag ) != 0 );
		}

		public void SetSavedFlag( int flag, bool value )
		{
			CompactInfo info = AcquireCompactInfo();

			if ( value )
				info.m_SavedFlags |= flag;
			else
				info.m_SavedFlags &= ~flag;

			if ( info.m_SavedFlags == 0 )
				VerifyCompactInfo();
		}

		public virtual void Deserialize( GenericReader reader )
		{
			int version = reader.ReadInt();

			SetLastMoved();

			switch ( version )
			{
				case 14:
					{
						m_Purchased = reader.ReadBool();
						goto case 13;
					}
				case 13:
					{
						m_EnchantMod = reader.ReadInt();
						goto case 12;
					}
				case 12:
					{
						ColorHue1 = reader.ReadString();
						ColorText1 = reader.ReadString();
						ColorHue2 = reader.ReadString();
						ColorText2 = reader.ReadString();
						ColorHue3 = reader.ReadString();
						ColorText3 = reader.ReadString();
						ColorHue4 = reader.ReadString();
						ColorText4 = reader.ReadString();
						ColorHue5 = reader.ReadString();
						ColorText5 = reader.ReadString();
						WorldItemID = reader.ReadInt();
						Technology = reader.ReadBool();
						VirtualContainer = reader.ReadBool();
						NotIdentified = reader.ReadBool();
						NotIDAttempts = reader.ReadInt();
						NotIDSource = (Identity)reader.ReadEncodedInt();
						NotIDSkill = (IDSkill)reader.ReadEncodedInt();
						Catalog = (Catalogs)reader.ReadEncodedInt();
							if ( DefaultCatalog != Catalogs.None )
								Catalog = DefaultCatalog;
						CoinPrice = reader.ReadInt();
						m_Resource = (CraftResource)reader.ReadEncodedInt();
						m_SubResource = (CraftResource)reader.ReadEncodedInt();
						m_SubName = reader.ReadString();
						ArtifactLevel = (ArtifactLevel)reader.ReadInt();
						NotModAble = reader.ReadBool();
						NeedsBothHands = reader.ReadBool();
						InfoData = reader.ReadString();
						InfoText1 = reader.ReadString();
						InfoText2 = reader.ReadString();
						InfoText3 = reader.ReadString();
						InfoText4 = reader.ReadString();
						InfoText5 = reader.ReadString();
						Limits = reader.ReadInt();
						LimitsMax = reader.ReadInt();
						LimitsName = reader.ReadString();
						LimitsDelete = reader.ReadBool();
						BuiltBy = reader.ReadMobile();
						Built = reader.ReadBool();
						goto case 11;
					}
				case 11:
					{
						Enchanted = (MagicSpell)reader.ReadEncodedInt();
						m_EnchantUses = reader.ReadInt();
						m_EnchantUsesMax = reader.ReadInt();
						goto case 10;
					}
				case 10:
					{
						GraphicID = reader.ReadInt();
						GraphicHue = reader.ReadInt();
						LastMobile = reader.ReadMobile();
						LastMobileName = reader.ReadString();
						goto case 6;
					}
				case 9:
				case 8:
				case 7:
				case 6:
				{
					SaveFlag flags = (SaveFlag)reader.ReadInt();

					if ( version < 7 )
					{
						LastMoved = reader.ReadDeltaTime();
					}
					else
					{
						int minutes = reader.ReadEncodedInt();

						try{ LastMoved = DateTime.Now - TimeSpan.FromMinutes( minutes ); }
						catch{ LastMoved = DateTime.Now; }
					}

					if ( GetSaveFlag( flags, SaveFlag.Direction ) )
						m_Direction = (Direction)reader.ReadByte();

					if ( GetSaveFlag( flags, SaveFlag.Bounce ) )
						AcquireCompactInfo().m_Bounce = BounceInfo.Deserialize( reader );

					if ( GetSaveFlag( flags, SaveFlag.LootType ) )
						m_LootType = (LootType)reader.ReadByte();

					int x = 0, y = 0, z = 0;

					if ( GetSaveFlag( flags, SaveFlag.LocationFull ) )
					{
						x = reader.ReadEncodedInt();
						y = reader.ReadEncodedInt();
						z = reader.ReadEncodedInt();
					}
					else
					{
						if ( GetSaveFlag( flags, SaveFlag.LocationByteXY ) )
						{
							x = reader.ReadByte();
							y = reader.ReadByte();
						}
						else if ( GetSaveFlag( flags, SaveFlag.LocationShortXY ) )
						{
							x = reader.ReadShort();
							y = reader.ReadShort();
						}

						if ( GetSaveFlag( flags, SaveFlag.LocationSByteZ ) )
							z = reader.ReadSByte();
					}

					m_Location = new Point3D( x, y, z );

					if ( GetSaveFlag( flags, SaveFlag.ItemID ) )
						m_ItemID = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.Hue ) )
						m_Hue = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.Amount ) )
						m_Amount = reader.ReadEncodedInt();
					else
						m_Amount = 1;

					if ( GetSaveFlag( flags, SaveFlag.Layer ) )
						m_Layer = (Layer)reader.ReadByte();

					if ( GetSaveFlag( flags, SaveFlag.Name ) )
					{
						string name = reader.ReadString();

						if ( name != this.DefaultName )
							AcquireCompactInfo().m_Name = name;
					}

					if ( GetSaveFlag( flags, SaveFlag.Parent ) )
					{
						Serial parent = reader.ReadInt();

						if ( parent.IsMobile )
							m_Parent = World.FindMobile( parent );
						else if ( parent.IsItem )
							m_Parent = World.FindItem( parent );
						else
							m_Parent = null;

						if ( m_Parent == null && (parent.IsMobile || parent.IsItem) )
							Delete();
					}

					if ( GetSaveFlag( flags, SaveFlag.Items ) )
					{
						List<Item> items = reader.ReadStrongItemList();

						if ( this is Container )
							( this as Container ).m_Items = items;
						else
							AcquireCompactInfo().m_Items = items;
					}

					if ( version < 8 || !GetSaveFlag( flags, SaveFlag.NullWeight ) )
					{
						double weight;

						if ( GetSaveFlag( flags, SaveFlag.IntWeight ) )
							weight = reader.ReadEncodedInt();
						else if ( GetSaveFlag( flags, SaveFlag.WeightNot1or0 ) )
							weight = reader.ReadDouble();
						else if ( GetSaveFlag( flags, SaveFlag.WeightIs0 ) )
							weight = 0.0;
						else
							weight = 1.0;

						if ( weight != DefaultWeight )
							AcquireCompactInfo().m_Weight = weight;
					}

					if ( GetSaveFlag( flags, SaveFlag.Map ) )
						m_Map = reader.ReadMap();
					else
						m_Map = Map.Internal;

					if ( GetSaveFlag( flags, SaveFlag.Visible ) )
						SetFlag( ImplFlag.Visible, reader.ReadBool() );
					else
						SetFlag( ImplFlag.Visible, true );

					if ( GetSaveFlag( flags, SaveFlag.Movable ) )
						SetFlag( ImplFlag.Movable, reader.ReadBool() );
					else
						SetFlag( ImplFlag.Movable, true );

					if ( GetSaveFlag( flags, SaveFlag.Stackable ) )
						SetFlag( ImplFlag.Stackable, reader.ReadBool() );

					if ( GetSaveFlag( flags, SaveFlag.ImplFlags ) )
						m_Flags = (ImplFlag)reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.InsuredFor ) )
						/*m_InsuredFor = */reader.ReadMobile();

					if ( GetSaveFlag( flags, SaveFlag.BlessedFor ) )
						AcquireCompactInfo().m_BlessedFor = reader.ReadMobile();

					if ( GetSaveFlag( flags, SaveFlag.HeldBy ) )
						AcquireCompactInfo().m_HeldBy = reader.ReadMobile();

					if ( GetSaveFlag( flags, SaveFlag.SavedFlags ) )
						AcquireCompactInfo().m_SavedFlags = reader.ReadEncodedInt();

					if ( m_Map != null && m_Parent == null )
						m_Map.OnEnter( this );

					break;
				}
				case 5:
				{
					SaveFlag flags = (SaveFlag)reader.ReadInt();

					LastMoved = reader.ReadDeltaTime();

					if ( GetSaveFlag( flags, SaveFlag.Direction ) )
						m_Direction = (Direction)reader.ReadByte();

					if ( GetSaveFlag( flags, SaveFlag.Bounce ) )
						AcquireCompactInfo().m_Bounce = BounceInfo.Deserialize( reader );

					if ( GetSaveFlag( flags, SaveFlag.LootType ) )
						m_LootType = (LootType)reader.ReadByte();

					if ( GetSaveFlag( flags, SaveFlag.LocationFull ) )
						m_Location = reader.ReadPoint3D();

					if ( GetSaveFlag( flags, SaveFlag.ItemID ) )
						m_ItemID = reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.Hue ) )
						m_Hue = reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.Amount ) )
						m_Amount = reader.ReadInt();
					else
						m_Amount = 1;

					if ( GetSaveFlag( flags, SaveFlag.Layer ) )
						m_Layer = (Layer)reader.ReadByte();

					if ( GetSaveFlag( flags, SaveFlag.Name ) )
					{
						string name = reader.ReadString();

						if ( name != this.DefaultName )
							AcquireCompactInfo().m_Name = name;
					}

					if ( GetSaveFlag( flags, SaveFlag.Parent ) )
					{
						Serial parent = reader.ReadInt();

						if ( parent.IsMobile )
							m_Parent = World.FindMobile( parent );
						else if ( parent.IsItem )
							m_Parent = World.FindItem( parent );
						else
							m_Parent = null;

						if ( m_Parent == null && (parent.IsMobile || parent.IsItem) )
							Delete();
					}

					if ( GetSaveFlag( flags, SaveFlag.Items ) )
					{
						List<Item> items = reader.ReadStrongItemList();

						if ( this is Container )
							( this as Container ).m_Items = items;
						else
							AcquireCompactInfo().m_Items = items;
					}

					double weight;

					if ( GetSaveFlag( flags, SaveFlag.IntWeight ) )
						weight = reader.ReadEncodedInt();
					else if ( GetSaveFlag( flags, SaveFlag.WeightNot1or0 ) )
						weight = reader.ReadDouble();
					else if ( GetSaveFlag( flags, SaveFlag.WeightIs0 ) )
						weight = 0.0;
					else
						weight = 1.0;

					if ( weight != DefaultWeight )
						AcquireCompactInfo().m_Weight = weight;

					if ( GetSaveFlag( flags, SaveFlag.Map ) )
						m_Map = reader.ReadMap();
					else
						m_Map = Map.Internal;

					if ( GetSaveFlag( flags, SaveFlag.Visible ) )
						SetFlag( ImplFlag.Visible, reader.ReadBool() );
					else
						SetFlag( ImplFlag.Visible, true );

					if ( GetSaveFlag( flags, SaveFlag.Movable ) )
						SetFlag( ImplFlag.Movable, reader.ReadBool() );
					else
						SetFlag( ImplFlag.Movable, true );

					if ( GetSaveFlag( flags, SaveFlag.Stackable ) )
						SetFlag( ImplFlag.Stackable, reader.ReadBool() );

					if ( m_Map != null && m_Parent == null )
						m_Map.OnEnter( this );

					break;
				}
				case 4: // Just removed variables
				case 3:
				{
					m_Direction = (Direction)reader.ReadInt();

					goto case 2;
				}
				case 2:
				{
					AcquireCompactInfo().m_Bounce = BounceInfo.Deserialize( reader );
					LastMoved = reader.ReadDeltaTime();

					goto case 1;
				}
				case 1:
				{
					m_LootType = (LootType) reader.ReadByte();//m_Newbied = reader.ReadBool();

					goto case 0;
				}
				case 0:
				{
					m_Location = reader.ReadPoint3D();
					m_ItemID = reader.ReadInt();
					m_Hue = reader.ReadInt();
					m_Amount = reader.ReadInt();
					m_Layer = (Layer) reader.ReadByte();

					string name = reader.ReadString();

					if ( name != this.DefaultName )
						AcquireCompactInfo().m_Name = name;

					Serial parent = reader.ReadInt();

					if ( parent.IsMobile )
						m_Parent = World.FindMobile( parent );
					else if ( parent.IsItem )
						m_Parent = World.FindItem( parent );
					else
						m_Parent = null;

					if ( m_Parent == null && (parent.IsMobile || parent.IsItem) )
						Delete();

					int count = reader.ReadInt();

					if ( count > 0 )
					{
						List<Item> items = new List<Item>( count );

						for ( int i = 0; i < count; ++i )
						{
							Item item = reader.ReadItem();

							if ( item != null )
								items.Add( item );
						}

						if ( this is Container )
							( this as Container ).m_Items = items;
						else
							AcquireCompactInfo().m_Items = items;
					}

					double weight = reader.ReadDouble();

					if ( weight != DefaultWeight )
						AcquireCompactInfo().m_Weight = weight;

					if ( version <= 3 )
					{
						reader.ReadInt();
						reader.ReadInt();
						reader.ReadInt();
					}

					m_Map = reader.ReadMap();
					SetFlag( ImplFlag.Visible, reader.ReadBool() );
					SetFlag( ImplFlag.Movable, reader.ReadBool() );

					if ( version <= 3 )
						/*m_Deleted =*/ reader.ReadBool();

					Stackable = reader.ReadBool();

					if ( m_Map != null && m_Parent == null )
						m_Map.OnEnter( this );

					break;
				}
			}

			if ( this.HeldBy != null )
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( FixHolding_Sandbox ) );

			if ( m_Resource == CraftResource.None && DefaultResource != CraftResource.None && version < 12 )
				m_Resource = DefaultResource;

			VerifyCompactInfo();

			if ( DefaultDescription != null )
				InfoData = DefaultDescription;

			SyncItem();
		}

		public void SyncItem()
		{
			if ( WorldItemID > 0 && ItemID != WorldItemID && RootParentEntity == null )
			{
				ItemID = WorldItemID;
				return;
			}
			else if ( WorldItemID > 0 && ItemID == WorldItemID && RootParentEntity != null )
			{
				ItemID = GraphicID;
				return;
			}

			SyncName();

			if ( !isModded( this ) && ItemID != WorldItemID )
				GraphicID = ItemID;

			if ( !isModHue( this ) )
				GraphicHue = Hue;

			if ( !Utility.ClothingMod() && isModded( this ) )
				undoMod( this );
		}

		public void SyncName()
		{
			if ( ItemID > 0xFFFF )
				return;

			if ( ( Name == "" || Name == null ) && LabelNumber == MainLabelNumber() )
			{
				Name = TileData.ItemTable[ItemID].Name;
				NameWasSynced = true;
			}

			if ( Name == "" || Name == null )
			{
				Name = CliLocTable.Lookup( LabelNumber );
				NameWasSynced = true;
			}

			if ( Name == "" || Name == null )
			{
				Name = Utility.AddSpaces( (this.GetType()).Name );
				NameWasSynced = true;
			}
		}

		private void FixHolding_Sandbox()
		{
			Mobile heldBy = this.HeldBy;

			if ( heldBy != null )
			{
				if ( this.GetBounce() != null )
				{
					Bounce( heldBy );
				}
				else
				{
					heldBy.Holding = null;
					heldBy.AddToBackpack( this );
					ClearBounce();
				}
			}
		}

		public virtual int GetMaxUpdateRange()
		{
			return 18;
		}

		public virtual int GetUpdateRange( Mobile m )
		{
			return 18;
		}

		public void SendInfoTo( NetState state ) {
			SendInfoTo( state, ObjectPropertyList.Enabled );
		}

		public virtual void SendInfoTo( NetState state, bool sendOplPacket ) {
			state.Send( GetWorldPacketFor( state ) );

			if ( sendOplPacket ) {
				state.Send( OPLPacket );
			}
		}

		protected virtual Packet GetWorldPacketFor( NetState state ) {
			if ( state.HighSeas )
				return this.WorldPacketHS;
			else if ( state.StygianAbyss )
				return this.WorldPacketSA;
			else
				return this.WorldPacket;
		}

		public virtual bool IsVirtualItem
		{
			get
			{
				if ( ParentEntity is Item && ((Item)ParentEntity).DoNotCountContents )
					return true;

				return false;
			}
		}

		public virtual int GetTotal( TotalType type )
		{
			return 0;
		}

		public virtual void UpdateTotal( Item sender, TotalType type, int delta )
		{
			if ( !IsVirtualItem )
			{
				if ( m_Parent is Item )
					( m_Parent as Item ).UpdateTotal( sender, type, delta );
				else if ( m_Parent is Mobile )
					( m_Parent as Mobile ).UpdateTotal( sender, type, delta );
				else if ( this.HeldBy != null )
					( this.HeldBy as Mobile ).UpdateTotal( sender, type, delta );			
			}
		}

		public virtual void UpdateTotals()
		{
		}

		public virtual int LabelNumber
		{
			get
			{
				if ( m_ItemID < 0x4000 )
					return 1020000 + m_ItemID;
				else
					return 1078872 + m_ItemID;
			}
		}

		public int MainLabelNumber()
		{
			if ( m_ItemID < 0x4000 )
				return 1020000 + m_ItemID;

			return 1078872 + m_ItemID;
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int TotalGold
		{
			get { return GetTotal( TotalType.Gold ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int TotalItems
		{
			get { return GetTotal( TotalType.Items ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int TotalWeight
		{
			get { return GetTotal( TotalType.Weight ); }
		}

		public virtual double DefaultWeight
		{
			get
			{
				if ( m_ItemID < 0 || m_ItemID > TileData.MaxItemValue || this is BaseMulti )
					return 0;

				int weight = TileData.ItemTable[m_ItemID].Weight;

				if ( weight == 255 || weight == 0 )
					weight = 1;

				return weight;
			}
		}

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public double Weight
		{
			get
			{
				CompactInfo info = LookupCompactInfo();

				if ( info != null && info.m_Weight != -1 )
					return info.m_Weight;

				return this.DefaultWeight;
			}
			set
			{
				if ( this.Weight != value )
				{
					CompactInfo info = AcquireCompactInfo();

					int oldPileWeight = this.PileWeight;

					info.m_Weight = value;

					if ( info.m_Weight == -1 )
						VerifyCompactInfo();

					int newPileWeight = this.PileWeight;

					UpdateTotal( this, TotalType.Weight, newPileWeight - oldPileWeight );

					InvalidateProperties();
				}
			}
		}

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public int PileWeight
		{
			get
			{
				return (int)Math.Ceiling( this.Weight * this.Amount );
			}
		}

		public virtual int HuedItemID
		{
			get
			{
				return m_ItemID;
			}
		}

		[Hue, CommandProperty( AccessLevel.GameMaster )]
		public virtual int Hue
		{
			get
			{
				return (QuestItem ? QuestItemHue : m_Hue);
			}
			set
			{
				if ( m_Hue != value )
				{
					m_Hue = value;
					ReleaseWorldPackets();

					Delta( ItemDelta.Update );
				}
			}
		}

		public virtual int QuestItemHue
		{
			get { return 0x04EA; }	//HMMMM... For EA?
		}

		public virtual bool Nontransferable
		{
			get { return QuestItem; }
		}

		public virtual void HandleInvalidTransfer( Mobile from )
		{
			if( QuestItem )
				from.SendLocalizedMessage( 1074769 ); // An item must be in your backpack (and not in a container within) to be toggled as a quest item.
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual Layer Layer
		{
			get
			{
				return m_Layer;
			}
			set
			{
				if ( m_Layer != value )
				{
					m_Layer = value;

					Delta( ItemDelta.EquipOnly );
				}
			}
		}

		public List<Item> Items
		{
			get
			{
				List<Item> items = LookupItems();

				if( items == null )
					items = EmptyItems;

				return items;
			}
		}

		public object RootParent
		{
			get
			{
				object p = m_Parent;

				while ( p is Item )
				{
					Item item = (Item)p;

					if ( item.m_Parent == null )
					{
						break;
					}
					else
					{
						p = item.m_Parent;
					}
				}

				return p;
			}
		}

		public bool ParentsContain<T>() where T : Item
		{
			object p = m_Parent;

			while( p is Item )
			{
				if( p is T )
					return true;

				Item item = (Item)p;

				if( item.m_Parent == null )
				{
					break;
				}
				else
				{
					p = item.m_Parent;
				}
			}

			return false;
		}

		public static bool isModded( Item item )
		{
			if ( 
			item.ItemID == 0x645C || // BELT
			item.ItemID == 0x645D || // COAT
			item.ItemID == 0x645E || // LOIN CLOTH
			item.ItemID == 0x645F || // SHIRT
			item.ItemID == 0x6460 || // SKIRT
			item.ItemID == 0x6461 || // VEST
			item.ItemID == 0x6462 || // BOOTS
			item.ItemID == 0x672A || // TOOLS
			item.ItemID == 0x6463 || // PANTS
			item.ItemID == 0x6464 || // CLOAK
			item.ItemID == 0x6465 || // HELM
			item.ItemID == 0x6466 || // HAT
			item.ItemID == 0x6467 || // GLOVES
			item.ItemID == 0x6468 || // BRACERS
			item.ItemID == 0x6469 || // ARMOR
			item.ItemID == 0x646A || // LEGGINGS
			item.ItemID == 0x646B || // MASK
			item.ItemID == 0x646C || // HOOD
			item.ItemID == 0x646D || // CIRCLET
			item.ItemID == 0x646E || // ANIMAL MASK
			item.ItemID == 0x646F || // BRACELET
			item.ItemID == 0x6470 || // RING
			item.ItemID == 0x6471 || // EARRINGS
			item.ItemID == 0x6472 || // AMULET
			item.ItemID == 0x6473 || // GORGET
			item.ItemID == 0x6474 || // ROBE
			item.ItemID == 0x6475 || // SHIELD
			item.ItemID == 0x6476 || // CANDLE
			item.ItemID == 0x6477 || // TORCH
			item.ItemID == 0x6478 || // LANTERN
			item.ItemID == 0x6479 || // CANDLE UNLIT
			item.ItemID == 0x647A || // TORCH UNLIT
			item.ItemID == 0x647B || // LANTERN UNLIT
			item.ItemID == 0x647D || // ONE-HANDED WEAPONS
			item.ItemID == 0x647E || // TWO-HANDED WEAPONS
			item.ItemID == 0x647F || // APRON WAIST
			item.ItemID == 0x6480 || // APRON OUTER
			item.ItemID == 0x6481 || // SKIRT OUTER
			item.ItemID == 0x6482 || // COAT OUTER
			item.ItemID == 0x6483 || // SHIRT OUTER
			item.ItemID == 0x6484 || // VEST OUTER
			item.ItemID == 0x6485 || // ONE-HANDED AXE
			item.ItemID == 0x6486 || // WAND
			item.ItemID == 0x6487 || // ONE-HANDED SPEAR
			item.ItemID == 0x6488 || // ONE-HANDED SWORD
			item.ItemID == 0x6489 || // DAGGER
			item.ItemID == 0x648A || // CLUB 
			item.ItemID == 0x648B || // STAFF
			item.ItemID == 0x648C || // POLE-ARM
			item.ItemID == 0x648D || // BOW 
			item.ItemID == 0x648E || // FISHING POLE
			item.ItemID == 0x66F7 || // SHOVEL
			item.ItemID == 0x648F || // ONE-HANDED MACE
			item.ItemID == 0x6490 || // ONE-HANDED HAMMER
			item.ItemID == 0x6491 || // ONE-HANDED FORK
			item.ItemID == 0x6492 || // TWO-HANDED FORK
			item.ItemID == 0x6493 || // TWO-HANDED AXE
			item.ItemID == 0x6494 || // TWO-HANDED SPEAR
			item.ItemID == 0x6495 || // TWO-HANDED SWORD
			item.ItemID == 0x6496 || // TWO-HANDED MACE
			item.ItemID == 0x6497 || // TWO-HANDED HAMMER
			item.ItemID == 0x6498 || // WHIP
			item.ItemID == 0x6499 )  // WIZARD STAFF
				return true;

			return false;
		}

		public static void modHues( Item item )
		{
			bool allowed = false;
			int hue = 0;

			if ( // LIGHTS
			item.GraphicID == 0xA0F || 
			item.GraphicID == 0xA12 || 
			item.GraphicID == 0xA15 || 
			item.GraphicID == 0xA17 || 
			item.GraphicID == 0xA22 || 
			item.GraphicID == 0xA28 || 
			item.GraphicID == 0xF6B || 
			item.GraphicID == 0xA18 || 
			item.GraphicID == 0xA25 )
			{
				allowed = true;
				hue = 0x423;
			}

			if ( // KIMONO
			item.GraphicID == 0x2783 || 
			item.GraphicID == 0x27CE )
			{
				allowed = true;
				hue = 0x424;
			}

			if ( // RED ARMOR
			item.GraphicID == 0x27C8 || 
			item.GraphicID == 0x27CB || 
			item.GraphicID == 0x27D0 || 
			item.GraphicID == 0x27D3 || 
			item.GraphicID == 0x277D || 
			item.GraphicID == 0x2780 || 
			item.GraphicID == 0x2785 || 
			item.GraphicID == 0x2788 || 
			item.GraphicID == 0x278A )
			{
				allowed = true;
				hue = 0x425;
			}

			if ( // KAMISHIMO
			item.GraphicID == 0x2799 || 
			item.GraphicID == 0x27E4 )
			{
				allowed = true;
				hue = 0x426;
			}

			if ( // BLACK ITEMS
			item.GraphicID == 0x2AB5 || 
			item.GraphicID == 0x27A6 || 
			item.GraphicID == 0x27F1 || 
			item.GraphicID == 0x63B1 || 
			item.GraphicID == 0x2AAC || 
			item.GraphicID == 0x3F65 || 
			item.GraphicID == 0x3F8F || 
			item.GraphicID == 0x0DF0 || 
			item.GraphicID == 0x0DF1 || 
			item.GraphicID == 0x27CD || 
			item.GraphicID == 0x27CF || 
			item.GraphicID == 0x27E6 || 
			item.GraphicID == 0x27E7 || 
			item.GraphicID == 0x2782 || 
			item.GraphicID == 0x2784 || 
			item.GraphicID == 0x279B || 
			item.GraphicID == 0x279C )
			{
				allowed = true;
				hue = 0x427;
			}

			if ( // LEATHER & WOOD
			item.GraphicID == 0x64B9 || 
			item.GraphicID == 0x64BA || 
			item.GraphicID == 0x64BB || 
			item.GraphicID == 0x64BC || 
			item.GraphicID == 0x64BD || 
			item.GraphicID == 0x2667 || 
			item.GraphicID == 0x2668 || 
			item.GraphicID == 0x266B || 
			item.GraphicID == 0x266C || 
			item.GraphicID == 0x266D || 
			item.GraphicID == 0x266E || 
			item.GraphicID == 0x2671 || 
			item.GraphicID == 0x26C2 || 
			item.GraphicID == 0x26C3 || 
			item.GraphicID == 0x26CC || 
			item.GraphicID == 0x26CD || 
			item.GraphicID == 0x2D1E || 
			item.GraphicID == 0x2D1F || 
			item.GraphicID == 0x2D25 || 
			item.GraphicID == 0x2D2A || 
			item.GraphicID == 0x2D2B || 
			item.GraphicID == 0x2D31 || 
			item.GraphicID == 0x63A2 || 
			item.GraphicID == 0x63A3 || 
			item.GraphicID == 0x63A4 || 
			item.GraphicID == 0x63A5 || 
			item.GraphicID == 0x63A6 || 
			item.GraphicID == 0x63A7 || 
			item.GraphicID == 0x63A8 || 
			item.GraphicID == 0x63A9 || 
			item.GraphicID == 0x63AA || 
			item.GraphicID == 0x63AB || 
			item.GraphicID == 0x63AC || 
			item.GraphicID == 0x63AD || 
			item.GraphicID == 0x63AE || 
			item.GraphicID == 0x63AF || 
			item.GraphicID == 0x63B0 || 
			item.GraphicID == 0x13B1 || 
			item.GraphicID == 0x13B2 || 
			item.GraphicID == 0x13B3 || 
			item.GraphicID == 0x13B4 || 
			item.GraphicID == 0x13F4 || 
			item.GraphicID == 0x13F5 || 
			item.GraphicID == 0x13F8 || 
			item.GraphicID == 0x13F9 || 
			item.GraphicID == 0x13FC || 
			item.GraphicID == 0x13FD || 
			item.GraphicID == 0x0E81 || 
			item.GraphicID == 0x0E82 || 
			item.GraphicID == 0x0E89 || 
			item.GraphicID == 0x0E8A || 
			item.GraphicID == 0x0F4F || 
			item.GraphicID == 0x0F50 || 
			item.GraphicID == 0x27A5 || 
			item.GraphicID == 0x27F0 || 
			item.GraphicID == 0x1C00 || 
			item.GraphicID == 0x1C01 || 
			item.GraphicID == 0x1C02 || 
			item.GraphicID == 0x1C03 || 
			item.GraphicID == 0x1C04 || 
			item.GraphicID == 0x1C05 || 
			item.GraphicID == 0x1C06 || 
			item.GraphicID == 0x1C07 || 
			item.GraphicID == 0x1C08 || 
			item.GraphicID == 0x1C09 || 
			item.GraphicID == 0x1C0A || 
			item.GraphicID == 0x1C0B || 
			item.GraphicID == 0x1C0C || 
			item.GraphicID == 0x1C0D || 
			item.GraphicID == 0x13C5 || 
			item.GraphicID == 0x13C6 || 
			item.GraphicID == 0x13C7 || 
			item.GraphicID == 0x13CB || 
			item.GraphicID == 0x13CC || 
			item.GraphicID == 0x13CD || 
			item.GraphicID == 0x13CE || 
			item.GraphicID == 0x13D2 || 
			item.GraphicID == 0x13D3 || 
			item.GraphicID == 0x13D4 || 
			item.GraphicID == 0x13D5 || 
			item.GraphicID == 0x13D6 || 
			item.GraphicID == 0x6614 || 
			item.GraphicID == 0x13DA || 
			item.GraphicID == 0x13DB || 
			item.GraphicID == 0x13DC || 
			item.GraphicID == 0x13DD || 
			item.GraphicID == 0x13E1 || 
			item.GraphicID == 0x13E2 || 
			item.GraphicID == 0x13EB || 
			item.GraphicID == 0x6613 || 
			item.GraphicID == 0x13EC || 
			item.GraphicID == 0x6617 || 
			item.GraphicID == 0x13ED || 
			item.GraphicID == 0x13EE || 
			item.GraphicID == 0x6615 || 
			item.GraphicID == 0x13EF || 
			item.GraphicID == 0x13F0 || 
			item.GraphicID == 0x6616 || 
			item.GraphicID == 0x13F1 || 
			item.GraphicID == 0x13F2 || 
			item.GraphicID == 0x1545 || 
			item.GraphicID == 0x1546 || 
			item.GraphicID == 0x1547 || 
			item.GraphicID == 0x1548 || 
			item.GraphicID == 0x2B6D || 
			item.GraphicID == 0x3164 || 
			item.GraphicID == 0x49C3 || 
			item.GraphicID == 0x1B78 || 
			item.GraphicID == 0x1B79 || 
			item.GraphicID == 0x1B7A || 
			item.GraphicID == 0x277B || 
			item.GraphicID == 0x277E || 
			item.GraphicID == 0x2786 || 
			item.GraphicID == 0x2790 || 
			item.GraphicID == 0x2798 || 
			item.GraphicID == 0x279A || 
			item.GraphicID == 0x2791 || 
			item.GraphicID == 0x2792 || 
			item.GraphicID == 0x279D || 
			item.GraphicID == 0x277A || 
			item.GraphicID == 0x2779 || 
			item.GraphicID == 0x277C || 
			item.GraphicID == 0x2776 || 
			item.GraphicID == 0x2778 || 
			item.GraphicID == 0x27C1 || 
			item.GraphicID == 0x27C3 || 
			item.GraphicID == 0x27C4 || 
			item.GraphicID == 0x27C5 || 
			item.GraphicID == 0x27C6 || 
			item.GraphicID == 0x27C9 || 
			item.GraphicID == 0x27E3 || 
			item.GraphicID == 0x27E5 || 
			item.GraphicID == 0x27E8 || 
			item.GraphicID == 0x27D1 || 
			item.GraphicID == 0x27D5 || 
			item.GraphicID == 0x27DB || 
			item.GraphicID == 0x647C || 
			item.GraphicID == 0x27DC || 
			item.GraphicID == 0x27DD || 
			item.GraphicID == 0x27A3 || 
			item.GraphicID == 0x27A8 || 
			item.GraphicID == 0x27F3 || 
			item.GraphicID == 0x27EE )
			{
				allowed = true;
				hue = 0x428;
			}

			if ( // GUARDSMAN SHIELD
			item.GraphicID == 0x3181 || 
			item.GraphicID == 0x2FCB )
			{
				allowed = true;
				hue = 0x429;
			}

			if ( // JEWELED SHIELD
			item.GraphicID == 0x2B75 || 
			item.GraphicID == 0x316C )
			{
				allowed = true;
				hue = 0x42A;
			}

			if ( // CRESTED SHIELD
			item.GraphicID == 0x317F || 
			item.GraphicID == 0x2FC9 )
			{
				allowed = true;
				hue = 0x42B;
			}

			if ( // ELVEN SHIELD
			item.GraphicID == 0x2FCA || 
			item.GraphicID == 0x3180 )
			{
				allowed = true;
				hue = 0x42C;
			}

			if ( // DARK SHIELD
			item.GraphicID == 0x2FC8 || 
			item.GraphicID == 0x317E )
			{
				allowed = true;
				hue = 0x42D;
			}

			if ( // WANDS
			item.GraphicID == 0xDF2 || 
			item.GraphicID == 0xDF3 || 
			item.GraphicID == 0xDF4 || 
			item.GraphicID == 0xDF5 || 
			item.GraphicID == 0x639D || 
			item.GraphicID == 0x639E || 
			item.GraphicID == 0x639F || 
			item.GraphicID == 0x63A0 )
			{
				allowed = true;
				hue = 0x42E;
			}

			if ( !isModHue( item ) ){ item.GraphicHue = item.Hue; }

			if ( allowed && isModded( item ) && item.GraphicHue == 0 )
			{
				item.Hue = hue;
			}
			else if ( allowed && !isModded( item ) && item.GraphicHue == 0 )
			{
				item.Hue = item.GraphicHue;
			}
		}

		public static void doMod( Item item )
		{
			if ( item.GraphicID < 1 && item.ItemID != item.WorldItemID ){ item.GraphicID = item.ItemID; }

			if ( !isModded( item ) )
			{
				if ( item.isWeapon() > 0 )
					item.ItemID = item.isWeapon();
				else if (	isCoat( item ) || 		 // ROBE
							isRobe( item ) ||
							item.ItemID == 0x563E || // Barbaric
							item.ItemID == 0x5652 || // Barbaric 
							item.ItemID == 0x567A || // Barbaric
							isShroud( item ) )
					item.ItemID = 0x6474;
				else if (	item.ItemID == 26413 || // BRACELET
							item.ItemID == 26413+5 ||
							item.ItemID == 26413+10 ||
							item.ItemID == 26413+15 ||
							item.ItemID == 26413+20 ||
							item.ItemID == 26413+25 ||
							item.ItemID == 26413+30 ||
							item.ItemID == 26413+35 ||
							item.ItemID == 26413+40 ||
							item.ItemID == 26413+45 ||
							item.ItemID == 26413+50 )
					item.ItemID = 0x646F;
				else if (	item.ItemID == 26417 || // RING
							item.ItemID == 26417+5 ||
							item.ItemID == 26417+10 ||
							item.ItemID == 26417+15 ||
							item.ItemID == 26417+20 ||
							item.ItemID == 26417+25 ||
							item.ItemID == 26417+30 ||
							item.ItemID == 26417+35 ||
							item.ItemID == 26417+40 ||
							item.ItemID == 26417+45 ||
							item.ItemID == 26417+50 )
					item.ItemID = 0x6470;
				else if (	item.ItemID == 26415 || // EARRINGS
							item.ItemID == 26415+5 ||
							item.ItemID == 26415+10 ||
							item.ItemID == 26415+15 ||
							item.ItemID == 26415+20 ||
							item.ItemID == 26415+25 ||
							item.ItemID == 26415+30 ||
							item.ItemID == 26415+35 ||
							item.ItemID == 26415+40 ||
							item.ItemID == 26415+45 ||
							item.ItemID == 26415+50 )
					item.ItemID = 0x6471;
				else if (	item.ItemID == 26416 || // AMULET
							item.ItemID == 26416+5 ||
							item.ItemID == 26416+10 ||
							item.ItemID == 26416+15 ||
							item.ItemID == 26416+20 ||
							item.ItemID == 26416+25 ||
							item.ItemID == 26416+30 ||
							item.ItemID == 26416+35 ||
							item.ItemID == 26416+40 ||
							item.ItemID == 26416+45 ||
							item.ItemID == 26416+50 )
					item.ItemID = 0x6472;
				else if (	item.ItemID == 0x2790 || // BELT
							item.ItemID == 0x27DB ||
							item.ItemID == 0x567B )
					item.ItemID = 0x645C;
				else if ( 	item.ItemID == 0x030D || // COAT
							item.ItemID == 0x030B ||
							item.ItemID == 0x0403 ||
							item.ItemID == 0x1F9F ||
							item.ItemID == 0x1FA0 ||
							item.ItemID == 0x1FA1 ||
							item.ItemID == 0x1FA2 ||
							item.ItemID == 0x1FFD ||
							item.ItemID == 0x1FFE ||
							item.ItemID == 0x230F ||
							item.ItemID == 0x2310 )
				{
					if ( item.Layer == Layer.MiddleTorso ){ item.ItemID = 0x6482; }
					else { item.ItemID = 0x645D; }
				}
				else if ( 	item.ItemID == 0x2B68 || // LOIN CLOTH
							item.ItemID == 0x315F ||
							item.ItemID == 0x55DB )
					item.ItemID = 0x645E;
				else if ( 	item.ItemID == 0x0307 || // SHIRT
							item.ItemID == 0x0311 ||
							item.ItemID == 0x0407 ||
							item.ItemID == 0x1EFD ||
							item.ItemID == 0x1EFE )
				{
					if ( item.Layer == Layer.MiddleTorso ){ item.ItemID = 0x6483; }
					else { item.ItemID = 0x645F; }
				}
				else if ( 	item.ItemID == 0x030A || // SKIRT
							item.ItemID == 0x0408 || 
							item.ItemID == 0x1516 || 
							item.ItemID == 0x279A || 
							item.ItemID == 0x27E5 || 
							item.ItemID == 0x1537 ||
							item.ItemID == 0x1538 ||
							item.ItemID == 0x230B ||
							item.ItemID == 0x230C ||
							item.ItemID == 0x2651 ||
							item.ItemID == 0x1531 )
				{
					if ( item.Layer == Layer.OuterLegs ){ item.ItemID = 0x6481; }
					else { item.ItemID = 0x6460; }
				}
				else if ( 	item.ItemID == 0x153B || // APRON
							item.ItemID == 0x153C || 
							item.ItemID == 0x153D || 
							item.ItemID == 0x153E )
				{
					if ( item.Layer == Layer.MiddleTorso ){ item.ItemID = 0x6480; }
					else { item.ItemID = 0x647F; }
				}
				else if ( 	item.ItemID == 0x0308 || // VEST
							item.ItemID == 0x030C ||
							item.ItemID == 0x030E ||
							item.ItemID == 0x1517 ||
							item.ItemID == 0x1518 ||
							item.ItemID == 0x1F7B ||
							item.ItemID == 0x1F7C ||
							item.ItemID == 0x27A1 ||
							item.ItemID == 0x27EC ||
							item.ItemID == 0x63B5 )
				{
					if ( item.Layer == Layer.MiddleTorso ){ item.ItemID = 0x6484; }
					else { item.ItemID = 0x6461; }
				}
				else if ( 	item.ItemID == 0x0406 || // BOOTS
							item.ItemID == 0x64BA ||
							item.ItemID == 0x170B ||
							item.ItemID == 0x6611 ||
							item.ItemID == 0x170C ||
							item.ItemID == 0x170D ||
							item.ItemID == 0x170E ||
							item.ItemID == 0x170F ||
							item.ItemID == 0x1710 ||
							item.ItemID == 0x1711 ||
							item.ItemID == 0x1712 ||
							item.ItemID == 0x2307 ||
							item.ItemID == 0x2308 ||
							item.ItemID == 0x26AF ||
							item.ItemID == 0x2796 ||
							item.ItemID == 0x2797 ||
							item.ItemID == 0x27E1 ||
							item.ItemID == 0x27E2 ||
							item.ItemID == 0x2B12 || 
							item.ItemID == 0x2B13 || 
							item.ItemID == 0x2B67 || 
							item.ItemID == 0x2FC4 || 
							item.ItemID == 0x315E || 
							item.ItemID == 0x317A || 
							item.ItemID == 0x4C26 || 
							item.ItemID == 0x4C27 || 
							item.ItemID == 0x567C )
					item.ItemID = 0x6462;
				else if ( 	item.ItemID == 0x0309 || // PANTS
							item.ItemID == 0x0404 ||
							item.ItemID == 0x152E ||
							item.ItemID == 0x152F ||
							item.ItemID == 0x1539 ||
							item.ItemID == 0x153A ||
							item.ItemID == 0x279B ||
							item.ItemID == 0x27E6 )
					item.ItemID = 0x6463;
				else if ( 	item.ItemID == 0x1515 || // CLOAK
							item.ItemID == 0x1530 ||
							item.ItemID == 0x2309 ||
							item.ItemID == 0x230A ||
							item.ItemID == 0x26AD ||
							item.ItemID == 0x2B04 ||
							item.ItemID == 0x2B05 ||
							item.ItemID == 0x2FC5 ||
							item.ItemID == 0x317B )
					item.ItemID = 0x6464;
				else if ( 	item.ItemID == 0x13BB || // HELM
							item.ItemID == 0x6612 ||
							item.ItemID == 0x13C0 ||
							item.ItemID == 0x1408 ||
							item.ItemID == 0x1409 ||
							item.ItemID == 0x140A ||
							item.ItemID == 0x140B ||
							item.ItemID == 0x140C ||
							item.ItemID == 0x140D ||
							item.ItemID == 0x140E ||
							item.ItemID == 0x140F ||
							item.ItemID == 0x1412 ||
							item.ItemID == 0x6610 ||
							item.ItemID == 0x1419 ||
							item.ItemID == 0x1451 ||
							item.ItemID == 0x1456 ||
							item.ItemID == 0x1966 ||
							item.ItemID == 0x1DB9 ||
							item.ItemID == 0x1DBA ||
							item.ItemID == 0x1F0B ||
							item.ItemID == 0x6618 ||
							item.ItemID == 0x1F0C ||
							item.ItemID == 0x236C ||
							item.ItemID == 0x236D ||
							item.ItemID == 0x2645 || 
							item.ItemID == 0x2646 || 
							item.ItemID == 0x2649 || 
							item.ItemID == 0x2653 || 
							item.ItemID == 0x267F || 
							item.ItemID == 0x2689 || 
							item.ItemID == 0x268A || 
							item.ItemID == 0x2774 || 
							item.ItemID == 0x2775 || 
							item.ItemID == 0x2776 || 
							item.ItemID == 0x2777 || 
							item.ItemID == 0x2778 || 
							item.ItemID == 0x2781 || 
							item.ItemID == 0x2784 || 
							item.ItemID == 0x2785 || 
							item.ItemID == 0x2789 || 
							item.ItemID == 0x27BF || 
							item.ItemID == 0x27C0 || 
							item.ItemID == 0x27C1 || 
							item.ItemID == 0x27C2 || 
							item.ItemID == 0x27C3 || 
							item.ItemID == 0x27CC || 
							item.ItemID == 0x27CF || 
							item.ItemID == 0x27D0 || 
							item.ItemID == 0x27D4 || 
							item.ItemID == 0x2B10 || 
							item.ItemID == 0x2B11 || 
							item.ItemID == 0x2FBB || 
							item.ItemID == 0x49C1 )
					item.ItemID = 0x6465;
				else if ( 	item.ItemID == 0x153F || // HAT
							item.ItemID == 0x1540 ||
							item.ItemID == 0x1543 ||
							item.ItemID == 0x1544 ||
							item.ItemID == 0x1713 ||
							item.ItemID == 0x1714 ||
							item.ItemID == 0x1715 ||
							item.ItemID == 0x1716 ||
							item.ItemID == 0x1717 ||
							item.ItemID == 0x1718 ||
							item.ItemID == 0x1719 ||
							item.ItemID == 0x171A ||
							item.ItemID == 0x171B ||
							item.ItemID == 0x171C ||
							item.ItemID == 0x172E ||
							item.ItemID == 0x2305 ||
							item.ItemID == 0x2306 ||
							item.ItemID == 0x2798 ||
							item.ItemID == 0x27E3 ||
							item.ItemID == 0x2FBC || 
							item.ItemID == 0x2FC3 || 
							item.ItemID == 0x3179 || 
							item.ItemID == 0x4C15 )
					item.ItemID = 0x6466;
				else if ( 	item.ItemID == 0x13C6 || // GLOVES
							item.ItemID == 0x64B9 ||
							item.ItemID == 0x13CE ||
							item.ItemID == 0x13D5 ||
							item.ItemID == 0x13DD ||
							item.ItemID == 0x13EB ||
							item.ItemID == 0x6613 ||
							item.ItemID == 0x13F2 ||
							item.ItemID == 0x1414 ||
							item.ItemID == 0x660C ||
							item.ItemID == 0x1418 ||
							item.ItemID == 0x1455 ||
							item.ItemID == 0x1450 ||
							item.ItemID == 0x1968 ||
							item.ItemID == 0x2643 ||
							item.ItemID == 0x2644 ||
							item.ItemID == 0x26B0 ||
							item.ItemID == 0x2792 ||
							item.ItemID == 0x27DD || 
							item.ItemID == 0x2B0C || 
							item.ItemID == 0x2B0D || 
							item.ItemID == 0x499D )
					item.ItemID = 0x6467;
				else if ( 	item.ItemID == 0x0303 || // BRACERS
							item.ItemID == 0x0304 ||
							item.ItemID == 0x0305 ||
							item.ItemID == 0x0306 ||
							item.ItemID == 0x13C5 ||
							item.ItemID == 0x13CD ||
							item.ItemID == 0x13D4 ||
							item.ItemID == 0x13DC ||
							item.ItemID == 0x13EE ||
							item.ItemID == 0x6615 ||
							item.ItemID == 0x13EF ||
							item.ItemID == 0x1410 ||
							item.ItemID == 0x660F ||
							item.ItemID == 0x1417 ||
							item.ItemID == 0x144E ||
							item.ItemID == 0x1453 ||
							item.ItemID == 0x1964 ||
							item.ItemID == 0x264E || 
							item.ItemID == 0x2657 || 
							item.ItemID == 0x2658 || 
							item.ItemID == 0x277E || 
							item.ItemID == 0x277F || 
							item.ItemID == 0x2780 || 
							item.ItemID == 0x27C9 || 
							item.ItemID == 0x27CA || 
							item.ItemID == 0x27CB || 
							item.ItemID == 0x2B0A || 
							item.ItemID == 0x2B0B || 
							item.ItemID == 0x2D01 || 
							item.ItemID == 0x2D02 || 
							item.ItemID == 0x2D03 || 
							item.ItemID == 0x2D04 )
					item.ItemID = 0x6468;
				else if ( 	item.ItemID == 0x13BF || // ARMOR
							item.ItemID == 0x64BD ||
							item.ItemID == 0x13C4 ||
							item.ItemID == 0x13CC ||
							item.ItemID == 0x13D3 ||
							item.ItemID == 0x13DB ||
							item.ItemID == 0x13E2 ||
							item.ItemID == 0x13EC ||
							item.ItemID == 0x6617 ||
							item.ItemID == 0x13ED ||
							item.ItemID == 0x1415 ||
							item.ItemID == 0x660B ||
							item.ItemID == 0x1416 ||
							item.ItemID == 0x144F ||
							item.ItemID == 0x1454 ||
							item.ItemID == 0x1969 ||
							item.ItemID == 0x1C02 || 
							item.ItemID == 0x1C03 || 
							item.ItemID == 0x1C04 || 
							item.ItemID == 0x1C05 || 
							item.ItemID == 0x1C06 || 
							item.ItemID == 0x1C07 || 
							item.ItemID == 0x1C0A || 
							item.ItemID == 0x1C0B || 
							item.ItemID == 0x1C0C || 
							item.ItemID == 0x1C0D || 
							item.ItemID == 0x2641 || 
							item.ItemID == 0x2642 || 
							item.ItemID == 0x264A || 
							item.ItemID == 0x264F || 
							item.ItemID == 0x2650 || 
							item.ItemID == 0x2654 || 
							item.ItemID == 0x2655 || 
							item.ItemID == 0x277B || 
							item.ItemID == 0x277C || 
							item.ItemID == 0x277D || 
							item.ItemID == 0x2793 || 
							item.ItemID == 0x2794 || 
							item.ItemID == 0x27C6 || 
							item.ItemID == 0x27C7 || 
							item.ItemID == 0x27C8 || 
							item.ItemID == 0x27DE || 
							item.ItemID == 0x27DF || 
							item.ItemID == 0x2B08 || 
							item.ItemID == 0x2B09 || 
							item.ItemID == 0x4B57 || 
							item.ItemID == 0x4B58 || 
							item.ItemID == 0x6399 || 
							item.ItemID == 0x639A || 
							item.ItemID == 0x639B || 
							item.ItemID == 0x639C )
					item.ItemID = 0x6469;
				else if ( 	item.ItemID == 0x13BE || // LEGGINGS
							item.ItemID == 0x64BC ||
							item.ItemID == 0x13C3 ||
							item.ItemID == 0x13CB ||
							item.ItemID == 0x13D2 ||
							item.ItemID == 0x13DA ||
							item.ItemID == 0x13E1 ||
							item.ItemID == 0x13F0 ||
							item.ItemID == 0x6616 ||
							item.ItemID == 0x13F1 ||
							item.ItemID == 0x46AA ||
							item.ItemID == 0x660E ||
							item.ItemID == 0x46AB ||
							item.ItemID == 0x1411 ||
							item.ItemID == 0x141A ||
							item.ItemID == 0x1452 ||
							item.ItemID == 0x1457 ||
							item.ItemID == 0x1965 || 
							item.ItemID == 0x1C00 || 
							item.ItemID == 0x1C01 || 
							item.ItemID == 0x1C08 || 
							item.ItemID == 0x1C09 || 
							item.ItemID == 0x2647 || 
							item.ItemID == 0x2648 || 
							item.ItemID == 0x264D || 
							item.ItemID == 0x2656 || 
							item.ItemID == 0x2659 || 
							item.ItemID == 0x2786 || 
							item.ItemID == 0x2787 || 
							item.ItemID == 0x2788 || 
							item.ItemID == 0x278A || 
							item.ItemID == 0x278B || 
							item.ItemID == 0x278D || 
							item.ItemID == 0x2791 || 
							item.ItemID == 0x279B || 
							item.ItemID == 0x27D1 || 
							item.ItemID == 0x27D2 || 
							item.ItemID == 0x27D3 || 
							item.ItemID == 0x27D5 || 
							item.ItemID == 0x27D6 || 
							item.ItemID == 0x27D8 || 
							item.ItemID == 0x27DC || 
							item.ItemID == 0x2B06 || 
							item.ItemID == 0x2B07 || 
							item.ItemID == 0x49C2 || 
							item.ItemID == 0x6396 || 
							item.ItemID == 0x6397 || 
							item.ItemID == 0x6398 || 
							item.ItemID == 0x63B4 )
					item.ItemID = 0x646A;
				else if ( 	item.ItemID == 0x0405 || // MASK
							item.ItemID == 0x141B ||
							item.ItemID == 0x141C ||
							item.ItemID == 0x1549 ||
							item.ItemID == 0x154A ||
							item.ItemID == 0x154B ||
							item.ItemID == 0x154C ||
							item.ItemID == 0x26A1 ||
							item.ItemID == 0x26A2 ||
							item.ItemID == 0x26A3 ||
							item.ItemID == 0x26A4 ||
							item.ItemID == 0x2B72 ||
							item.ItemID == 0x3169 )
					item.ItemID = 0x646B;
				else if ( 	item.ItemID == 0x278E || // HOOD
							item.ItemID == 0x64BB || 
							item.ItemID == 0x0310 || 
							item.ItemID == 0x278F || 
							item.ItemID == 0x27D9 || 
							item.ItemID == 0x27DA || 
							item.ItemID == 0x2B71 || 
							item.ItemID == 0x2FBE || 
							item.ItemID == 0x3168 || 
							item.ItemID == 0x3176 || 
							item.ItemID == 0x3177 || 
							item.ItemID == 0x4CDA || 
							item.ItemID == 0x4CDB || 
							item.ItemID == 0x4CDC || 
							item.ItemID == 0x4CDD || 
							item.ItemID == 0x4D01 || 
							item.ItemID == 0x4D02 || 
							item.ItemID == 0x4D03 || 
							item.ItemID == 0x4D04 || 
							item.ItemID == 0x4D09 || 
							item.ItemID == 0x5C11 || 
							item.ItemID == 0x5C12 || 
							item.ItemID == 0x5C13 || 
							item.ItemID == 0x5C14 )
					item.ItemID = 0x646C;
				else if ( 	item.ItemID == 26414 || // CIRCLET
							item.ItemID == 26414+5 ||
							item.ItemID == 26414+10 ||
							item.ItemID == 26414+15 ||
							item.ItemID == 26414+20 ||
							item.ItemID == 26414+25 ||
							item.ItemID == 26414+30 ||
							item.ItemID == 26414+35 ||
							item.ItemID == 26414+40 ||
							item.ItemID == 26414+45 ||
							item.ItemID == 26414+50 )
					item.ItemID = 0x646D;
				else if ( 	item.ItemID == 0x1545 || // ANIMAL MASK
							item.ItemID == 0x1546 || 
							item.ItemID == 0x1547 || 
							item.ItemID == 0x1548 || 
							item.ItemID == 0x2B6D || 
							item.ItemID == 0x3164 || 
							item.ItemID == 0x49C3 )
					item.ItemID = 0x646E;
				else if ( 	item.ItemID == 0x13C7 || // GORGET
							item.ItemID == 0x13D6 || 
							item.ItemID == 0x6614 || 
							item.ItemID == 0x1413 || 
							item.ItemID == 0x660D || 
							item.ItemID == 0x1967 || 
							item.ItemID == 0x264B || 
							item.ItemID == 0x264C || 
							item.ItemID == 0x2B0E || 
							item.ItemID == 0x2B0F || 
							item.ItemID == 0x317D )
					item.ItemID = 0x6473;
				else if ( 	item.ItemID == 0x1B72 || // SHIELD
							item.ItemID == 0x1B73 || 
							item.ItemID == 0x1B74 || 
							item.ItemID == 0x1B75 || 
							item.ItemID == 0x1B76 || 
							item.ItemID == 0x6619 || 
							item.ItemID == 0x6609 || 
							item.ItemID == 0x1B77 || 
							item.ItemID == 0x1B78 || 
							item.ItemID == 0x1B79 || 
							item.ItemID == 0x1B7A || 
							item.ItemID == 0x1B7B || 
							item.ItemID == 0x1BC3 || 
							item.ItemID == 0x1BC4 || 
							item.ItemID == 0x1BC5 || 
							item.ItemID == 0x1BC6 || 
							item.ItemID == 0x1BC7 || 
							item.ItemID == 0x2B01 || 
							item.ItemID == 0x2B74 || 
							item.ItemID == 0x2B75 || 
							item.ItemID == 0x2FC8 || 
							item.ItemID == 0x2FC9 || 
							item.ItemID == 0x2FCA || 
							item.ItemID == 0x2FCB || 
							item.ItemID == 0x316B || 
							item.ItemID == 0x316C || 
							item.ItemID == 0x317E || 
							item.ItemID == 0x317F || 
							item.ItemID == 0x3180 || 
							item.ItemID == 0x3181 )
					item.ItemID = 0x6475;
				else if ( item.ItemID == 0xA0F ){ item.ItemID = 0x6476; } // CANDLE
				else if ( 	item.ItemID == 0x6601 || // TOOLS
							item.ItemID == 0x66FA || 
							item.ItemID == 0x6708 || 
							item.ItemID == 0x6600 || 
							item.ItemID == 0x4C81 || 
							item.ItemID == 0x2B76 || 
							item.ItemID == 0x2B77 || 
							item.ItemID == 0x316D || 
							item.ItemID == 0x3170 || 
							item.ItemID == 0x2B78 || 
							item.ItemID == 0x2B79 || 
							item.ItemID == 0x661B || 
							item.ItemID == 0x640A || 
							item.ItemID == 0x66FC || 
							item.ItemID == 0x316E || 
							item.ItemID == 0x4F52 || 
							item.ItemID == 0x66F9 || 
							item.ItemID == 0x6704 || 
							item.ItemID == 0x6706 || 
							item.ItemID == 0x640B || 
							item.ItemID == 0x142B || 
							item.ItemID == 0x66F8 || 
							item.ItemID == 0x5173 || 
							item.ItemID == 0x316F )
					item.ItemID = 0x672A;
				else if ( item.ItemID == 0xA12 ){ item.ItemID = 0x6477; } // TORCH
				else if ( item.ItemID == 0xA15 || item.ItemID == 0xA17 || item.ItemID == 0xA22 ){ item.ItemID = 0x6478; } // LANTERN
				else if ( item.ItemID == 0xA28 ){ item.ItemID = 0x6479; } // CANDLE U
				else if ( item.ItemID == 0xF6B ){ item.ItemID = 0x647A; } // TORCH U
				else if ( item.ItemID == 0xA18 || item.ItemID == 0xA25 ){ item.ItemID = 0x647B; } // LANTERN U
				else if ( 	item.ItemID == 0x2B02 || // QUIVER
							item.ItemID == 0x2FB7 || 
							item.ItemID == 0x3171 || 
							item.ItemID == 0x5770 || 
							item.ItemID == 0x2B03 )
					item.ItemID = 0x647C;
				else if ( item.Catalog == Catalogs.Tool ){ /* DO NOTHING */ }
				else if ( item.Layer == Layer.OneHanded ){ item.ItemID = 0x647D; }
				else if ( item.Layer == Layer.TwoHanded ){ item.ItemID = 0x647E; }
				else if ( item.Layer == Layer.Invalid ){ item.ItemID = 0x647D; }
			}
			modHues( item );
		}

		public static void undoMod( Item item )
		{
			if ( isModded( item ) )
			{
				item.ItemID = item.GraphicID;
			}
			modHues( item );
		}

		public static bool isModHue( Item item )
		{
			if ( item.Hue >= 0x423 && item.Hue <= 0x42E )
				return true;

			return false;
		}

		public static bool isCoat( Item item )
		{
			if ( 
			item.ItemID == 0x567E || 
			item.ItemID == 0x27E7 || 
			item.ItemID == 0x279C || 
			item.ItemID == 0x2B6B || 
			item.ItemID == 0x4C16 || 
			item.ItemID == 0x4C17 || 
			item.ItemID == 0x3162 )
				return true;

			return false;
		}

		public static bool isRobe( Item item )
		{
			if ( 
			item.ItemID == 0x2B6E || 
			item.ItemID == 0x2782 || 
			item.ItemID == 0x2783 || 
			item.ItemID == 0x27CD || 
			item.ItemID == 0x27CE || 
			item.ItemID == 0x2799 || 
			item.ItemID == 0x27E4 || 
			item.ItemID == 0x283 || 
			item.ItemID == 0x284 || 
			item.ItemID == 0x285 || 
			item.ItemID == 0x286 || 
			item.ItemID == 0x287 || 
			item.ItemID == 0x288 || 
			item.ItemID == 0x289 || 
			item.ItemID == 0x28A || 
			item.ItemID == 0x301 || 
			item.ItemID == 0x302 || 
			item.ItemID == 0x1F03 || 
			item.ItemID == 0x1F04 || 
			item.ItemID == 0x201B || 
			item.ItemID == 0x201C || 
			item.ItemID == 0x201D || 
			item.ItemID == 0x201E || 
			item.ItemID == 0x201F || 
			item.ItemID == 0x2020 || 
			item.ItemID == 0x25EC || 
			item.ItemID == 0x25ED || 
			item.ItemID == 0x2652 || 
			item.ItemID == 0x26AE || 
			item.ItemID == 0x2B69 || 
			item.ItemID == 0x2B6A || 
			item.ItemID == 0x2B6C || 
			item.ItemID == 0x266E || 
			item.ItemID == 0x2B70 || 
			item.ItemID == 0x2B73 || 
			item.ItemID == 0x2FBA || 
			item.ItemID == 0x2FBD || 
			item.ItemID == 0x2FC6 || 
			item.ItemID == 0x2FC7 || 
			item.ItemID == 0x3160 || 
			item.ItemID == 0x3161 || 
			item.ItemID == 0x3163 || 
			item.ItemID == 0x3165 || 
			item.ItemID == 0x3167 || 
			item.ItemID == 0x316A || 
			item.ItemID == 0x3174 || 
			item.ItemID == 0x3175 || 
			item.ItemID == 0x3178 || 
			item.ItemID == 0x4000 || 
			item.ItemID == 0x4001 || 
			item.ItemID == 0x4002 || 
			item.ItemID == 0x4003 || 
			item.ItemID == 0x567D || 
			item.ItemID == 0x1EFF || 
			item.ItemID == 0x1F00 || 
			item.ItemID == 0x1F01 || 
			item.ItemID == 0x1F02 || 
			item.ItemID == 0x230D || 
			item.ItemID == 0x230E || 
			item.ItemID == 0x5C10 )
				return true;

			return false;
		}

		public static bool isShroud( Item item )
		{
			if ( 
			item.ItemID == 0x2683 || 
			item.ItemID == 0x2684 || 
			item.ItemID == 0x2685 || 
			item.ItemID == 0x2686 || 
			item.ItemID == 0x2687 || 
			item.ItemID == 0x204E || 
			item.ItemID == 0x25EE || 
			item.ItemID == 0x25EF || 
			item.ItemID == 0x25F0 || 
			item.ItemID == 0x25F1 )
				return true;

			return false;
		}

		public virtual int isWeapon()
		{
			if ( GraphicID == 0x0DBF || GraphicID == 0x0DC0 || GraphicID == 0x6605 || GraphicID == 0x6606 )
				return 25742;

			if ( GraphicID == 0x0F3A || GraphicID == 0x0F39 || GraphicID == 0x6608 )
				return 26359;

			return 0;

			/*
			25733	light axe
			25734	wand
			25735	light spear
			25736	light sword
			25737	dagger
			25738	club
			25739	staff
			25740	pole arm
			25741	bow
			25742	fishing pole
			25743	light mace
			25744	light hammer
			25745	fork
			25746	trident
			25747	heavy axe
			25748	heavy spear
			25749	heavy sword
			25750	heavy mace
			25751	heavy hammer
			26359	shovel
			*/
		}

		public static bool isRaceCostume( Item item )
		{
			if ( 
			item.ItemID == 0x4047 || 
			item.ItemID == 0x4048 || 
			item.ItemID == 0x4049 || 
			item.ItemID == 0x404A || 
			item.ItemID == 0x404B || 
			item.ItemID == 0x404C || 
			item.ItemID == 0x404D || 
			item.ItemID == 0x404E || 
			item.ItemID == 0x404F || 
			item.ItemID == 0x4050 || 
			item.ItemID == 0x4051 || 
			item.ItemID == 0x4052 || 
			item.ItemID == 0x4053 || 
			item.ItemID == 0x4054 || 
			item.ItemID == 0x4055 || 
			item.ItemID == 0x4056 || 
			item.ItemID == 0x4057 || 
			item.ItemID == 0x4058 || 
			item.ItemID == 0x4059 || 
			item.ItemID == 0x405A || 
			item.ItemID == 0x405B || 
			item.ItemID == 0x405C || 
			item.ItemID == 0x405D || 
			item.ItemID == 0x405E || 
			item.ItemID == 0x405F || 
			item.ItemID == 0x4060 || 
			item.ItemID == 0x4061 || 
			item.ItemID == 0x4062 || 
			item.ItemID == 0x4063 || 
			item.ItemID == 0x4064 || 
			item.ItemID == 0x4065 || 
			item.ItemID == 0x4066 || 
			item.ItemID == 0x4067 || 
			item.ItemID == 0x4068 || 
			item.ItemID == 0x4069 || 
			item.ItemID == 0x406A || 
			item.ItemID == 0x2080 || 
			item.ItemID == 0x2081 || 
			item.ItemID == 0x2082 || 
			item.ItemID == 0x2083 || 
			item.ItemID == 0x2084 || 
			item.ItemID == 0x2085 || 
			item.ItemID == 0x2086 || 
			item.ItemID == 0x2087 || 
			item.ItemID == 0x2088 || 
			item.ItemID == 0x2089 || 
			item.ItemID == 0x208A || 
			item.ItemID == 0x208B || 
			item.ItemID == 0x208C || 
			item.ItemID == 0x208D || 
			item.ItemID == 0x208E || 
			item.ItemID == 0x208F || 
			item.ItemID == 0x2090 || 
			item.ItemID == 0x2091 || 
			item.ItemID == 0x2092 || 
			item.ItemID == 0x2093 || 
			item.ItemID == 0x2094 || 
			item.ItemID == 0x2095 || 
			item.ItemID == 0x2096 || 
			item.ItemID == 0x2097 || 
			item.ItemID == 0x2098 || 
			item.ItemID == 0x2099 || 
			item.ItemID == 0x209A || 
			item.ItemID == 0x209B || 
			item.ItemID == 0x209C || 
			item.ItemID == 0x209D || 
			item.ItemID == 0x209E || 
			item.ItemID == 0x209F || 
			item.ItemID == 0x20A0 || 
			item.ItemID == 0x20A1 || 
			item.ItemID == 0x20A2 || 
			item.ItemID == 0x20A3 || 
			item.ItemID == 0x20A4 || 
			item.ItemID == 0x20A5 || 
			item.ItemID == 0x20A6 || 
			item.ItemID == 0x20A7 || 
			item.ItemID == 0x20A8 || 
			item.ItemID == 0x20A9 || 
			item.ItemID == 0x20AA || 
			item.ItemID == 0x20AB || 
			item.ItemID == 0x20AC || 
			item.ItemID == 0x20AD || 
			item.ItemID == 0x20AE || 
			item.ItemID == 0x20AF || 
			item.ItemID == 0x20B0 || 
			item.ItemID == 0x20B1 || 
			item.ItemID == 0x20B2 || 
			item.ItemID == 0x20B3 || 
			item.ItemID == 0x20B4 || 
			item.ItemID == 0x20B5 || 
			item.ItemID == 0x20B6 || 
			item.ItemID == 0x20B7 || 
			item.ItemID == 0x20B8 || 
			item.ItemID == 0x20B9 || 
			item.ItemID == 0x20BA || 
			item.ItemID == 0x20BB || 
			item.ItemID == 0x20BC || 
			item.ItemID == 0x20BD || 
			item.ItemID == 0x20BE || 
			item.ItemID == 0x20BF || 
			item.ItemID == 0x20C0 || 
			item.ItemID == 0x20C1 || 
			item.ItemID == 0x20C2 || 
			item.ItemID == 0x20C3 || 
			item.ItemID == 0x20C4 || 
			item.ItemID == 0x20C5 || 
			item.ItemID == 0x20C6 || 
			item.ItemID == 0x20C7 || 
			item.ItemID == 0x649A || 
			item.ItemID == 0x649B || 
			item.ItemID == 0x649C || 
			item.ItemID == 0x649D || 
			item.ItemID == 0x649E || 
			item.ItemID == 0x649F || 
			item.ItemID == 0x64A0 || 
			item.ItemID == 0x64A1 || 
			item.ItemID == 0x64A2 || 
			item.ItemID == 0x64A3 || 
			item.ItemID == 0x64A4 || 
			item.ItemID == 0x64A5 || 
			item.ItemID == 0x64A6 || 
			item.ItemID == 0x64A7 || 
			item.ItemID == 0x64A8 || 
			item.ItemID == 0x64A9 || 
			item.ItemID == 0x64AA || 
			item.ItemID == 0x64AB || 
			item.ItemID == 0x64AC || 
			item.ItemID == 0x64AD || 
			item.ItemID == 0x64AE || 
			item.ItemID == 0x64AF || 
			item.ItemID == 0x64B0 || 
			item.ItemID == 0x64B1 || 
			item.ItemID == 0x64B2 || 
			item.ItemID == 0x64B3 || 
			item.ItemID == 0x64B4 || 
			item.ItemID == 0x64B5 || 
			item.ItemID == 0x64B6 || 
			item.ItemID == 0x64B7 )
				return true;

			return false;
		}

		public static bool isHoodedRobe( Item item )
		{
			if ( 
			item.ItemID == 0x20F4 || 
			item.ItemID == 0x2FB9 || 
			item.ItemID == 0x3173 )
				return true;

			return false;
		}

		public static bool isPartialHat( Item item )
		{
			if ( 
			item.ItemID == 0x1DB9 || 
			item.ItemID == 0x1DBA || 
			item.ItemID == 0x140E || 
			item.ItemID == 0x140F || 
			item.ItemID == 0x140A || 
			item.ItemID == 0x140B || 
			item.ItemID == 0x13C0 || 
			item.ItemID == 0x2653 || 
			item.ItemID == 0x13BB || 
			item.ItemID == 0x6612 || 
			item.ItemID == 0x13C0 || 
			item.ItemID == 0x2653 || 
			item.ItemID == 0x0310 || 
			item.ItemID == 0x4D03 || 
			item.ItemID == 0x5C11 || 
			item.ItemID == 0x5C14 || 
			item.ItemID == 0x2B71 || 
			item.ItemID == 0x3168 || 
			item.ItemID == 0x4D03 || 
			item.ItemID == 0x4D09 || 
			item.ItemID == 0x5C11 || 
			item.ItemID == 0x5C14 )
				return true;

			return false;
		}

		public static bool isFullHat( Item item )
		{
			if ( 
			item.ItemID == 0x140C || 
			item.ItemID == 0x140D || 
			item.ItemID == 0x1408 || 
			item.ItemID == 0x1409 || 
			item.ItemID == 0x6610 || 
			item.ItemID == 0x1412 || 
			item.ItemID == 0x1419 || 
			item.ItemID == 0x2649 || 
			item.ItemID == 0x267F || 
			item.ItemID == 0x2FBB || 
			item.ItemID == 0x64BB || 
			item.ItemID == 0x49C1 || 
			item.ItemID == 0x1451 || 
			item.ItemID == 0x1456 || 
			item.ItemID == 0x1F0B || 
			item.ItemID == 0x6618 || 
			item.ItemID == 0x1F0C || 
			item.ItemID == 0x4D01 || 
			item.ItemID == 0x4D02 || 
			item.ItemID == 0x4D04 || 
			item.ItemID == 0x5C12 || 
			item.ItemID == 0x5C13 || 
			item.ItemID == 0x278E || 
			item.ItemID == 0x278F || 
			item.ItemID == 0x27D9 || 
			item.ItemID == 0x27DA || 
			item.ItemID == 0x2B72 || 
			item.ItemID == 0x2FBE || 
			item.ItemID == 0x3169 || 
			item.ItemID == 0x3176 || 
			item.ItemID == 0x3177 || 
			item.ItemID == 0x4CDA || 
			item.ItemID == 0x4CDB || 
			item.ItemID == 0x4CDC || 
			item.ItemID == 0x4CDD || 
			item.ItemID == 0x4D01 || 
			item.ItemID == 0x4D02 || 
			item.ItemID == 0x4D04 || 
			item.ItemID == 0x5C12 || 
			item.ItemID == 0x5C13 || 
			item.ItemID == 0x0405 || 
			item.ItemID == 0x141B || 
			item.ItemID == 0x141C )
				return true;

			return false;
		}

		public static bool isBarbaric( Item item )
		{
			if ( 
			item.ItemID == 0x406 || 
			item.ItemID == 0x409 || 
			item.ItemID == 0x563E || 
			item.ItemID == 0x5643 || 
			item.ItemID == 0x5648 || 
			item.ItemID == 0x564D || 
			item.ItemID == 0x564E || 
			item.ItemID == 0x564F || 
			item.ItemID == 0x5650 || 
			item.ItemID == 0x5651 || 
			item.ItemID == 0x5652 || 
			item.ItemID == 0x5679 || 
			item.ItemID == 0x567A )
				return true;

			return false;
		}

		public static bool IsCowlHood( Item item )
		{
			if ( 
			item.ItemID == 0x646C || 
			item.ItemID == 0x5C14 || 
			item.ItemID == 0x4D09 || 
			item.ItemID == 0x141B || 
			item.ItemID == 0x141C || 
			item.ItemID == 0x4D01 || 
			item.ItemID == 0x4D02 || 
			item.ItemID == 0x4D03 || 
			item.ItemID == 0x4D04 || 
			item.ItemID == 0x3176 || 
			item.ItemID == 0x3177 || 
			item.ItemID == 0x2B71 || 
			item.ItemID == 0x3168 || 
			item.ItemID == 0x4CDA || 
			item.ItemID == 0x4CDB || 
			item.ItemID == 0x4CDC || 
			item.ItemID == 0x4CDD )
				return true;

			return false;
		}

		public static bool isFullLegs( Item item )
		{
			if ( 
			item.ItemID == 0x1411 || 
			item.ItemID == 0x141A || 
			item.ItemID == 0x264D || 
			item.ItemID == 0x6396 || 
			item.ItemID == 0x6397 || 
			item.ItemID == 0x6398 || 
			item.ItemID == 0x46AA || 
			item.ItemID == 0x660E || 
			item.ItemID == 0x46AB || 
			item.ItemID == 0x1965 )
				return true;

			return false;
		}

		public static bool isJester( Item item )
		{
			if ( 
			item.ItemID == 0x27E7 || 
			item.ItemID == 0x279C || 
			item.ItemID == 0x2B6B || 
			item.ItemID == 0x4C16 || 
			item.ItemID == 0x4C17 || 
			item.ItemID == 0x3162 )
				return true;

			return false;
		}

		public static bool isArmor( Item item )
		{
			if ( // NORMAL
			item.ItemID == 0x1415 || 
			item.ItemID == 0x660B || 
			item.ItemID == 0x1416 || 
			item.ItemID == 0x6399 || 
			item.ItemID == 0x639A || 
			item.ItemID == 0x639B || 
			item.ItemID == 0x639C || 
			item.ItemID == 0x13CC || 
			item.ItemID == 0x13D3 || 
			item.ItemID == 0x264F || 
			item.ItemID == 0x2650 || 
			item.ItemID == 0x13BF || 
			item.ItemID == 0x13C4 || 
			item.ItemID == 0x2654 || 
			item.ItemID == 0x2655 || 
			item.ItemID == 0x13EC || 
			item.ItemID == 0x6617 || 
			item.ItemID == 0x13ED || 
			item.ItemID == 0x2B08 || 
			item.ItemID == 0x2B09 || 
			item.ItemID == 0x13DB || 
			item.ItemID == 0x13E2 || 
			item.ItemID == 0x1969 || 
			item.ItemID == 0x498F || 
			item.ItemID == 0x4B57 || 
			item.ItemID == 0x4B58 || 
			item.ItemID == 0x144F || 
			item.ItemID == 0x1454 || 
			item.ItemID == 0x277B || 
			item.ItemID == 0x277C || 
			item.ItemID == 0x277D || 
			item.ItemID == 0x2793 || 
			item.ItemID == 0x2794 || 
			item.ItemID == 0x27C6 || 
			item.ItemID == 0x27C7 || 
			item.ItemID == 0x27C8 || 
			item.ItemID == 0x27DE || 
			item.ItemID == 0x27DF || 
			item.ItemID == 0x2641 || 
			item.ItemID == 0x2642 || 
			item.ItemID == 0x264A )
				return true;

			if ( // FEMALE
			item.ItemID == 0x1C03 || 
			item.ItemID == 0x1C04 || 
			item.ItemID == 0x1C05 || 
			item.ItemID == 0x1C06 || 
			item.ItemID == 0x1C07 || 
			item.ItemID == 0x1C02 )
				return true;

			return false;
		}

		public virtual void AddItem( Item item )
		{
			if ( item == null || item.Deleted || item.m_Parent == this )
			{
				return;
			}
			else if ( item == this )
			{
				Console.WriteLine( "Warning: Adding item to itself: [0x{0:X} {1}].AddItem( [0x{2:X} {3}] )", this.Serial.Value, this.GetType().Name, item.Serial.Value, item.GetType().Name );
				Console.WriteLine( new System.Diagnostics.StackTrace() );
				return;
			}
			else if ( IsChildOf( item ) )
			{
				Console.WriteLine( "Warning: Adding parent item to child: [0x{0:X} {1}].AddItem( [0x{2:X} {3}] )", this.Serial.Value, this.GetType().Name, item.Serial.Value, item.GetType().Name );
				Console.WriteLine( new System.Diagnostics.StackTrace() );
				return;
			}
			else if ( item.m_Parent is Mobile )
			{
				((Mobile)item.m_Parent).RemoveItem( item );
			}
			else if ( item.m_Parent is Item )
			{
				((Item)item.m_Parent).RemoveItem( item );
			}
			else
			{
				item.SendRemovePacket();
			}

			item.Parent = this;
			item.Map = m_Map;

			List<Item> items = AcquireItems();

			items.Add( item );

			if ( !item.IsVirtualItem )
			{
				UpdateTotal( item, TotalType.Gold, item.TotalGold );
				UpdateTotal( item, TotalType.Items, item.TotalItems + 1 );
				UpdateTotal( item, TotalType.Weight, item.TotalWeight + item.PileWeight );
			}

			item.Delta( ItemDelta.Update );

			item.OnAdded( this );
			OnItemAdded( item );
		}

		private static List<Item> m_DeltaQueue = new List<Item>();

		public void Delta( ItemDelta flags )
		{
			if ( m_Map == null || m_Map == Map.Internal )
				return;

			m_DeltaFlags |= flags;

			if ( !GetFlag( ImplFlag.InQueue ) )
			{
				SetFlag( ImplFlag.InQueue, true );

				m_DeltaQueue.Add( this );
			}

			Core.Set();
		}

		public void RemDelta( ItemDelta flags )
		{
			m_DeltaFlags &= ~flags;

			if ( GetFlag( ImplFlag.InQueue ) && m_DeltaFlags == ItemDelta.None )
			{
				SetFlag( ImplFlag.InQueue, false );

				m_DeltaQueue.Remove( this );
			}
		}

		private bool m_NoMoveHS;

		public bool NoMoveHS
		{
			get { return m_NoMoveHS; }
			set { m_NoMoveHS = value; }
		}

		public void ProcessDelta()
		{
			ItemDelta flags = m_DeltaFlags;

			SetFlag( ImplFlag.InQueue, false );
			m_DeltaFlags = ItemDelta.None;

			Map map = m_Map;

			if ( map != null && !Deleted )
			{
				bool sendOPLUpdate = ObjectPropertyList.Enabled && (flags & ItemDelta.Properties) != 0;

				Container contParent = m_Parent as Container;

				if ( contParent != null && !contParent.IsPublicContainer )
				{
					if ( (flags & ItemDelta.Update) != 0 )
					{
						Point3D worldLoc = GetWorldLocation();

						Mobile rootParent = contParent.RootParent as Mobile;
						Mobile tradeRecip = null;

						if ( rootParent != null )
						{
							NetState ns = rootParent.NetState;

							if ( ns != null )
							{
								if ( rootParent.CanSee( this ) && rootParent.InRange( worldLoc, GetUpdateRange( rootParent ) ) )
								{
									if ( ns.ContainerGridLines )
										ns.Send( new ContainerContentUpdate6017( this ) );
									else
										ns.Send( new ContainerContentUpdate( this ) );

									if ( ObjectPropertyList.Enabled )
										ns.Send( OPLPacket );
								}
							}
						}

						SecureTradeContainer stc = this.GetSecureTradeCont();

						if ( stc != null )
						{
							SecureTrade st = stc.Trade;

							if ( st != null )
							{
								Mobile test = st.From.Mobile;

								if ( test != null && test != rootParent )
									tradeRecip = test;

								test = st.To.Mobile;

								if ( test != null && test != rootParent )
									tradeRecip = test;

								if ( tradeRecip != null )
								{
									NetState ns = tradeRecip.NetState;

									if ( ns != null )
									{
										if ( tradeRecip.CanSee( this ) && tradeRecip.InRange( worldLoc, GetUpdateRange( tradeRecip ) ) )
										{
											if ( ns.ContainerGridLines )
												ns.Send( new ContainerContentUpdate6017( this ) );
											else
												ns.Send( new ContainerContentUpdate( this ) );

											if ( ObjectPropertyList.Enabled )
												ns.Send( OPLPacket );
										}
									}
								}
							}
						}

						List<Mobile> openers = contParent.Openers;

						if ( openers != null )
						{
							for ( int i = 0; i < openers.Count; ++i )
							{
								Mobile mob = openers[i];

								int range = GetUpdateRange( mob );

								if ( mob.Map != map || !mob.InRange( worldLoc, range ) )
								{
									openers.RemoveAt( i-- );
								}
								else
								{
									if ( mob == rootParent || mob == tradeRecip )
										continue;

									NetState ns = mob.NetState;

									if ( ns != null )
									{
										if ( mob.CanSee( this ) )
										{
											if ( ns.ContainerGridLines )
												ns.Send( new ContainerContentUpdate6017( this ) );
											else
												ns.Send( new ContainerContentUpdate( this ) );

											if ( ObjectPropertyList.Enabled )
												ns.Send( OPLPacket );
										}
									}
								}
							}

							if ( openers.Count == 0 )
								contParent.Openers = null;
						}
						return;
					}
				}

				if ( (flags & ItemDelta.Update) != 0 )
				{
					Packet p = null;
					Point3D worldLoc = GetWorldLocation();

					IPooledEnumerable eable = map.GetClientsInRange( worldLoc, GetMaxUpdateRange() );

					foreach ( NetState state in eable ) {
						Mobile m = state.Mobile;

						if ( m.CanSee( this ) && m.InRange( worldLoc, GetUpdateRange( m ) ) ) {
							if ( m_Parent == null ) {
								SendInfoTo( state, ObjectPropertyList.Enabled );
							} else {
								if ( p == null ) {
									if ( m_Parent is Item ) {
										if ( state.ContainerGridLines )
											state.Send( new ContainerContentUpdate6017( this ) );
										else
											state.Send( new ContainerContentUpdate( this ) );
									} else if ( m_Parent is Mobile ) {
										p = new EquipUpdate( this );
										p.Acquire();
										state.Send( p );
									}
								} else {
									state.Send( p );
								}

								if ( ObjectPropertyList.Enabled ) {
									state.Send( OPLPacket );
								}
							}
						}
					}

					if ( p != null )
						Packet.Release( p );

					eable.Free();
					sendOPLUpdate = false;
				}
				else if ( (flags & ItemDelta.EquipOnly ) != 0 )
				{
					if ( m_Parent is Mobile )
					{
						Packet p = null;
						Point3D worldLoc = GetWorldLocation();

						IPooledEnumerable eable = map.GetClientsInRange( worldLoc, GetMaxUpdateRange() );

						foreach ( NetState state in eable )
						{
							Mobile m = state.Mobile;

							if ( m.CanSee( this ) && m.InRange( worldLoc, GetUpdateRange( m ) ) )
							{
								//if ( sendOPLUpdate )
								//	state.Send( RemovePacket );

								if ( p == null )
									p = Packet.Acquire( new EquipUpdate( this ) );

								state.Send( p );

								if ( ObjectPropertyList.Enabled )
									state.Send( OPLPacket );
							}
						}

						Packet.Release( p );

						eable.Free();
						sendOPLUpdate = false;
					}
				}

				if ( sendOPLUpdate )
				{
					Point3D worldLoc = GetWorldLocation();
					IPooledEnumerable eable = map.GetClientsInRange( worldLoc, GetMaxUpdateRange() );

					foreach ( NetState state in eable )
					{
						Mobile m = state.Mobile;

						if ( m.CanSee( this ) && m.InRange( worldLoc, GetUpdateRange( m ) ) )
							state.Send( OPLPacket );
					}

					eable.Free();
				}
			}
		}

		public static void ProcessDeltaQueue()
		{
			int count = m_DeltaQueue.Count;

			for ( int i = 0; i < m_DeltaQueue.Count; ++i )
			{
				m_DeltaQueue[i].ProcessDelta();

				if ( i >= count )
					break;
			}

			if ( m_DeltaQueue.Count > 0 )
				m_DeltaQueue.Clear();
		}

		public virtual void OnDelete()
		{
			if ( m_Spawner != null )
			{
				m_Spawner.Remove( this );
				m_Spawner = null;
			}
		}

		public virtual void OnParentDeleted( object parent )
		{
			this.Delete();
		}

		public virtual void FreeCache()
		{
			ReleaseWorldPackets();
			Packet.Release( ref m_RemovePacket );
			Packet.Release( ref m_OPLPacket );
			Packet.Release( ref m_PropertyList );
		}

		public virtual void Delete()
		{
			if ( Deleted )
				return;
			else if ( !World.OnDelete( this ) )
				return;

			OnDelete();

			List<Item> items = LookupItems();

			if ( items != null )
			{
				for ( int i = items.Count - 1; i >= 0; --i )
				{
					if ( i < items.Count )
						items[i].OnParentDeleted( this );
				}
			}

			SendRemovePacket();

			SetFlag( ImplFlag.Deleted, true );

			if ( Parent is Mobile )
				((Mobile)Parent).RemoveItem( this );
			else if ( Parent is Item )
				((Item)Parent).RemoveItem( this );

			ClearBounce();

			if ( m_Map != null )
			{
				if ( m_Parent == null )
					m_Map.OnLeave( this );
				m_Map = null;
			}

			World.RemoveItem( this );

			OnAfterDelete();

			FreeCache();
		}

		public void PublicOverheadMessage( MessageType type, int hue, bool ascii, string text )
		{
			if ( m_Map != null )
			{
				Packet p = null;
				Point3D worldLoc = GetWorldLocation();

				IPooledEnumerable eable = m_Map.GetClientsInRange( worldLoc, GetMaxUpdateRange() );

				foreach ( NetState state in eable )
				{
					Mobile m = state.Mobile;

					if ( m.CanSee( this ) && m.InRange( worldLoc, GetUpdateRange( m ) ) )
					{
						if ( p == null )
						{
							if ( ascii )
								p = new AsciiMessage( m_Serial, m_ItemID, type, hue, 3, this.Name, text );
							else
								p = new UnicodeMessage( m_Serial, m_ItemID, type, hue, 3, "ENU", this.Name, text );

							p.Acquire();
						}

						state.Send( p );
					}
				}

				Packet.Release( p );

				eable.Free();
			}
		}

		public void PublicOverheadMessage( MessageType type, int hue, int number )
		{
			PublicOverheadMessage( type, hue, number, "" );
		}

		public void PublicOverheadMessage( MessageType type, int hue, int number, string args )
		{
			if ( m_Map != null )
			{
				Packet p = null;
				Point3D worldLoc = GetWorldLocation();

				IPooledEnumerable eable = m_Map.GetClientsInRange( worldLoc, GetMaxUpdateRange() );

				foreach ( NetState state in eable )
				{
					Mobile m = state.Mobile;

					if ( m.CanSee( this ) && m.InRange( worldLoc, GetUpdateRange( m ) ) )
					{
						if ( p == null )
							p = Packet.Acquire( new MessageLocalized( m_Serial, m_ItemID, type, hue, 3, number, this.Name, args ) );

						state.Send( p );
					}
				}

				Packet.Release( p );

				eable.Free();
			}
		}

		public virtual void OnAfterDelete()
		{
		}

		public virtual void RemoveItem( Item item )
		{
			List<Item> items = LookupItems();

			if ( items != null && items.Contains( item ) )
			{
				item.SendRemovePacket();

				items.Remove( item );

				if ( !item.IsVirtualItem )
				{
					UpdateTotal( item, TotalType.Gold, -item.TotalGold );
					UpdateTotal( item, TotalType.Items, -( item.TotalItems + 1 ) );
					UpdateTotal( item, TotalType.Weight, -( item.TotalWeight + item.PileWeight ) );
				}

				item.Parent = null;

				item.OnRemoved( this );
				OnItemRemoved( item );
				Mobile.ShowItem( item );
			}
		}

		public virtual void OnAfterDuped( Item newItem )
		{
		}

		public virtual void DefaultMainHue( Item item )
		{
		}

		public virtual bool OnDragLift( Mobile from )
		{
			if ( from.AccessLevel < AccessLevel.GameMaster )
			{
				m_LastMobile = from;
				m_LastMobileName = from.Name;
			}

			if ( WorldItemID > 0 && ItemID == WorldItemID )
				ItemID = GraphicID;

			DefaultMainHue( this );

			return true;
		}

		public virtual bool OnEquip( Mobile from )
		{
			from.ProcessClothing();

			if ( IsCowlHood( this ) )
				from.SendMessage( "You can double click this to change the color." );

			return true;
		}

		//TODO: Move to CompactInfo.
		private ISpawner m_Spawner;

		public ISpawner Spawner{ get{ return m_Spawner; } set{ m_Spawner = value; } }

		public virtual void OnBeforeSpawn( Point3D location, Map m )
		{
		}

		public virtual void OnAfterSpawn()
		{
		}

		public virtual int PhysicalResistance{ get{ return 0; } }
		public virtual int FireResistance{ get{ return 0; } }
		public virtual int ColdResistance{ get{ return 0; } }
		public virtual int PoisonResistance{ get{ return 0; } }
		public virtual int EnergyResistance{ get{ return 0; } }

		[CommandProperty( AccessLevel.Counselor )]
		public Serial Serial
		{
			get
			{
				return m_Serial;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public IEntity ParentEntity
		{
			get
			{
				IEntity p = Parent as IEntity;

				return p;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public IEntity RootParentEntity
		{
			get
			{
				IEntity p = RootParent as IEntity;

				return p;
			}
		}

		#region Location Location Location!

		public virtual void OnLocationChange( Point3D oldLocation )
		{
			SyncItem();
			Mobile.ShowItem( this );
		}

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public virtual Point3D Location
		{
			get
			{
				return m_Location;
			}
			set
			{
				Point3D oldLocation = m_Location;

				if ( oldLocation != value )
				{
					if ( m_Map != null )
					{
						if ( m_Parent == null )
						{
							IPooledEnumerable eable;

							if ( m_Location.m_X != 0 )
							{
								Packet removeThis = null;

								eable = m_Map.GetClientsInRange( oldLocation, GetMaxUpdateRange() );

								foreach ( NetState state in eable )
								{
									Mobile m = state.Mobile;

									if ( !m.InRange( value, GetUpdateRange( m ) ) )
									{
										if ( removeThis == null )
											removeThis = this.RemovePacket;

										state.Send( removeThis );
									}
								}

								eable.Free();
							}

							Point3D oldLoc = m_Location;
							m_Location = value;
							ReleaseWorldPackets();

							SetLastMoved();

							eable = m_Map.GetClientsInRange( m_Location, GetMaxUpdateRange() );

							foreach ( NetState state in eable )
							{
								Mobile m = state.Mobile;

								if ( m.CanSee( this ) && m.InRange( m_Location, GetUpdateRange( m ) ) && ( !state.HighSeas || !m_NoMoveHS || ( m_DeltaFlags & ItemDelta.Update ) != 0 || !m.InRange( oldLoc, GetUpdateRange( m ) ) ) )
									SendInfoTo( state );
							}

							eable.Free();

							RemDelta( ItemDelta.Update );
						}
						else if ( m_Parent is Item )
						{
							m_Location = value;
							ReleaseWorldPackets();

							Delta( ItemDelta.Update );
						}
						else
						{
							m_Location = value;
							ReleaseWorldPackets();
						}

						if ( m_Parent == null )
							m_Map.OnMove( oldLocation, this );
					}
					else
					{
						m_Location = value;
						ReleaseWorldPackets();
					}

					this.OnLocationChange( oldLocation );
				}
			}
		}

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public int X
		{
			get{ return m_Location.m_X; }
			set{ Location = new Point3D( value, m_Location.m_Y, m_Location.m_Z ); }
		}

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public int Y
		{
			get{ return m_Location.m_Y; }
			set{ Location = new Point3D( m_Location.m_X, value, m_Location.m_Z ); }
		}

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public int Z
		{
			get{ return m_Location.m_Z; }
			set{ Location = new Point3D( m_Location.m_X, m_Location.m_Y, value ); }
		}
		#endregion

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int ItemID
		{
			get
			{
				return m_ItemID;
			}
			set
			{
				if ( m_ItemID != value )
				{
					int oldPileWeight = this.PileWeight;

					m_ItemID = value;
					ReleaseWorldPackets();

					int newPileWeight = this.PileWeight;

					UpdateTotal( this, TotalType.Weight, newPileWeight - oldPileWeight );

					InvalidateProperties();
					Delta( ItemDelta.Update );
				}
			}
		}

		public virtual string DefaultName
		{
			get { return null; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Name
		{
			get
			{
				CompactInfo info = LookupCompactInfo();

				if ( info != null && info.m_Name != null )
					return info.m_Name;

				return this.DefaultName;
			}
			set
			{
				if ( value == null || value != DefaultName )
				{
					CompactInfo info = AcquireCompactInfo();

					info.m_Name = value;

					if ( info.m_Name == null )
						VerifyCompactInfo();

					NameWasSynced = false;
					InvalidateProperties();
				}
			}
		}

		public virtual object Parent
		{
			get
			{
				return m_Parent;
			}
			set
			{
				if ( m_Parent == value )
					return;

				object oldParent = m_Parent;

				m_Parent = value;

				if ( m_Map != null )
				{
					if ( oldParent != null && m_Parent == null )
						m_Map.OnEnter( this );
					else if ( m_Parent != null )
						m_Map.OnLeave( this );
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public LightType Light
		{
			get
			{
				return (LightType)m_Direction;
			}
			set
			{
				if ( (LightType)m_Direction != value )
				{
					m_Direction = (Direction)value;
					ReleaseWorldPackets();

					Delta( ItemDelta.Update );
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Direction Direction
		{
			get
			{
				return m_Direction;
			}
			set
			{
				if ( m_Direction != value )
				{
					m_Direction = value;
					ReleaseWorldPackets();

					Delta( ItemDelta.Update );
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Amount
		{
			get
			{
				return m_Amount;
			}
			set
			{
				int oldValue = m_Amount;

				if ( oldValue != value )
				{
					int oldPileWeight = this.PileWeight;

					m_Amount = value;
					ReleaseWorldPackets();

					int newPileWeight = this.PileWeight;

					UpdateTotal( this, TotalType.Weight, newPileWeight - oldPileWeight );

					OnAmountChange( oldValue );

					Delta( ItemDelta.Update );

					if ( oldValue > 1 || value > 1 )
						InvalidateProperties();

					if ( !Stackable && m_Amount > 1 )
						Console.WriteLine( "Warning: 0x{0:X}: Amount changed for non-stackable item '{2}'. ({1})", m_Serial.Value, m_Amount, GetType().Name );
				}
			}
		}

		protected virtual void OnAmountChange( int oldValue )
		{
		}

		public virtual bool HandlesOnSpeech{ get{ return false; } }

		public virtual void OnSpeech( SpeechEventArgs e )
		{
		}

		public virtual bool OnDroppedToMobile( Mobile from, Mobile target )
		{
			if( Nontransferable && from.Player && from.AccessLevel <= AccessLevel.GameMaster )
			{
				HandleInvalidTransfer( from );
				return false;
			}

			return true;
		}

		public virtual bool DropToMobile( Mobile from, Mobile target, Point3D p )
		{
			if ( Deleted || from.Deleted || target.Deleted || from.Map != target.Map || from.Map == null || target.Map == null )
				return false;
			else if ( from.AccessLevel < AccessLevel.GameMaster && !from.InRange( target.Location, 2 ) )
				return false;
			else if ( !from.CanSee( target ) || !from.InLOS( target ) )
				return false;
			else if ( !from.OnDroppedItemToMobile( this, target ) )
				return false;
			else if ( !OnDroppedToMobile( from, target ) )
				return false;
			else if ( !target.OnDragDrop( from, this ) )
				return false;
			else
				return true;
		}

		public virtual bool OnDroppedInto( Mobile from, Container target, Point3D p )
		{
			if( !from.OnDroppedItemInto( this, target, p ) )
			{
				return false;
			}
			else if( Nontransferable && from.Player && target != from.Backpack && from.AccessLevel <= AccessLevel.GameMaster )
			{
				HandleInvalidTransfer( from );
				return false;
			}

			return target.OnDragDropInto( from, this, p );
		}

		public virtual bool OnDroppedOnto( Mobile from, Item target )
		{
			if ( Deleted || from.Deleted || target.Deleted || from.Map != target.Map || from.Map == null || target.Map == null )
				return false;
			else if ( from.AccessLevel < AccessLevel.GameMaster && !from.InRange( target.GetWorldLocation(), 2 ) )
				return false;
			else if ( !from.CanSee( target ) || !from.InLOS( target ) )
				return false;
			else if ( !target.IsAccessibleTo( from ) )
				return false;
			else if ( !from.OnDroppedItemOnto( this, target ) )
				return false;
			else if( Nontransferable && from.Player && from.AccessLevel <= AccessLevel.GameMaster )
			{
				HandleInvalidTransfer( from );
				return false;
			}
			else
				return target.OnDragDrop( from, this );
		}

		public virtual bool DropToItem( Mobile from, Item target, Point3D p )
		{
			if ( Deleted || from.Deleted || target.Deleted || from.Map != target.Map || from.Map == null || target.Map == null )
				return false;

			object root = target.RootParent;

			if ( from.AccessLevel < AccessLevel.GameMaster && !from.InRange( target.GetWorldLocation(), 2 ) )
				return false;
			else if ( !from.CanSee( target ) || !from.InLOS( target ) )
				return false;
			else if ( !target.IsAccessibleTo( from ) )
				return false;
			else if ( root is Mobile && !((Mobile)root).CheckNonlocalDrop( from, this, target ) )
				return false;
			else if ( !from.OnDroppedItemToItem( this, target, p ) )
				return false;
			else if ( target is Container && p.m_X != -1 && p.m_Y != -1 )
				return OnDroppedInto( from, (Container)target, p );
			else
				return OnDroppedOnto( from, target );
		}

		public virtual bool OnDroppedToWorld( Mobile from, Point3D p )
		{
			if( Nontransferable && from.Player && from.AccessLevel <= AccessLevel.GameMaster )
			{
				HandleInvalidTransfer( from );
				return false;
			}

			return true;
		}

		public virtual int GetLiftSound( Mobile from )
		{
			return 0x57;
		}

		private static int m_OpenSlots;

		public virtual bool DropToWorld( Mobile from, Point3D p )
		{
			if ( Deleted || from.Deleted || from.Map == null )
				return false;
			else if ( !from.InRange( p, 2 ) )
				return false;

			Map map = from.Map;

			if ( map == null )
				return false;

			int x = p.m_X, y = p.m_Y;
			int z = int.MinValue;

			int maxZ = from.Z + 16;

			LandTile landTile = map.Tiles.GetLandTile( x, y );
			TileFlag landFlags = TileData.LandTable[landTile.ID & TileData.MaxLandValue].Flags;

			int landZ = 0, landAvg = 0, landTop = 0;
			map.GetAverageZ( x, y, ref landZ, ref landAvg, ref landTop );

			if ( !landTile.Ignored && (landFlags & TileFlag.Impassable) == 0 )
			{
				if ( landAvg <= maxZ )
					z = landAvg;
			}

			StaticTile[] tiles = map.Tiles.GetStaticTiles( x, y, true );

			for ( int i = 0; i < tiles.Length; ++i )
			{
				StaticTile tile = tiles[i];
				ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

				if ( !id.Surface )
					continue;

				int top = tile.Z + id.CalcHeight;

				if ( top > maxZ || top < z )
					continue;

				z = top;
			}

			List<Item> items = new List<Item>();

			IPooledEnumerable eable = map.GetItemsInRange( p, 0 );

			foreach ( Item item in eable )
			{
				if ( item is BaseMulti || item.ItemID > TileData.MaxItemValue )
					continue;

				items.Add( item );

				ItemData id = item.ItemData;

				if ( !id.Surface )
					continue;

				int top = item.Z + id.CalcHeight;

				if ( top > maxZ || top < z )
					continue;

				z = top;
			}

			eable.Free();

			if ( z == int.MinValue )
				return false;

			if ( z > maxZ )
				return false;

			m_OpenSlots = (1<<20)-1;

			int surfaceZ = z;

			for ( int i = 0; i < tiles.Length; ++i )
			{
				StaticTile tile = tiles[i];
				ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

				int checkZ = tile.Z;
				int checkTop = checkZ + id.CalcHeight;

				if ( checkTop == checkZ && !id.Surface )
					++checkTop;

				int zStart = checkZ - z;
				int zEnd = checkTop - z;

				if ( zStart >= 20 || zEnd < 0 )
					continue;

				if ( zStart < 0 )
					zStart = 0;

				if ( zEnd > 19 )
					zEnd = 19;

				int bitCount = zEnd-zStart;

				m_OpenSlots &= ~(((1<<bitCount)-1)<<zStart);
			}

			for ( int i = 0; i < items.Count; ++i )
			{
				Item item = items[i];
				ItemData id = item.ItemData;

				int checkZ = item.Z;
				int checkTop = checkZ + id.CalcHeight;

				if ( checkTop == checkZ && !id.Surface )
					++checkTop;

				int zStart = checkZ - z;
				int zEnd = checkTop - z;

				if ( zStart >= 20 || zEnd < 0 )
					continue;

				if ( zStart < 0 )
					zStart = 0;

				if ( zEnd > 19 )
					zEnd = 19;

				int bitCount = zEnd-zStart;

				m_OpenSlots &= ~(((1<<bitCount)-1)<<zStart);
			}

			int height = ItemData.Height;

			if ( height == 0 )
				++height;

			if ( height > 30 )
				height = 30;

			int match = (1<<height)-1;
			bool okay = false;

			for ( int i = 0; i < 20; ++i )
			{
				if ( (i+height) > 20 )
					match >>= 1;

				okay = ((m_OpenSlots>>i)&match) == match;

				if ( okay )
				{
					z += i;
					break;
				}
			}

			if ( !okay )
				return false;

			height = ItemData.Height;

			if ( height == 0 )
				++height;

			if ( landAvg > z && (z + height) > landZ )
				return false;
			else if ( (landFlags & TileFlag.Impassable) != 0 && landAvg > surfaceZ && (z + height) > landZ )
				return false;

			for ( int i = 0; i < tiles.Length; ++i )
			{
				StaticTile tile = tiles[i];
				ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

				int checkZ = tile.Z;
				int checkTop = checkZ + id.CalcHeight;

				if ( checkTop > z && (z + height) > checkZ )
					return false;
				else if ( (id.Surface || id.Impassable) && checkTop > surfaceZ && (z + height) > checkZ )
					return false;
			}

			for ( int i = 0; i < items.Count; ++i )
			{
				Item item = items[i];
				ItemData id = item.ItemData;

				//int checkZ = item.Z;
				//int checkTop = checkZ + id.CalcHeight;

				if ( (item.Z + id.CalcHeight) > z && (z + height) > item.Z )
					return false;
			}

			p = new Point3D( x, y, z );

			if ( !from.InLOS( new Point3D( x, y, z + 1 ) ) )
				return false;
			else if ( !from.OnDroppedItemToWorld( this, p ) )
				return false;
			else if ( !OnDroppedToWorld( from, p ) )
				return false;

			int soundID = GetDropSound();

			MoveToWorld( p, from.Map );

			from.SendSound( soundID == -1 ? 0x42 : soundID, GetWorldLocation() );

			return true;
		}

		public void SendRemovePacket()
		{
			if ( !Deleted && m_Map != null )
			{
				Packet p = null;
				Point3D worldLoc = GetWorldLocation();

				IPooledEnumerable eable = m_Map.GetClientsInRange( worldLoc, GetMaxUpdateRange() );

				foreach ( NetState state in eable )
				{
					Mobile m = state.Mobile;

					if ( m.InRange( worldLoc, GetUpdateRange( m ) ) )
					{
						if ( p == null )
							p = this.RemovePacket;

						state.Send( p );
					}
				}

				eable.Free();
			}
		}

		public virtual int GetDropSound()
		{
			return -1;
		}

		public Point3D GetWorldLocation()
		{
			object root = RootParent;

			if ( root == null )
				return m_Location;
			else
				return ((IEntity)root).Location;

			//return root == null ? m_Location : new Point3D( (IPoint3D) root );
		}

		public virtual bool BlocksFit{ get{ return false; } }

		public Point3D GetSurfaceTop()
		{
			object root = RootParent;

			if ( root == null )
				return new Point3D( m_Location.m_X, m_Location.m_Y, m_Location.m_Z + (ItemData.Surface ? ItemData.CalcHeight : 0) );
			else
				return ((IEntity)root).Location;
		}

		public Point3D GetWorldTop()
		{
			object root = RootParent;

			if ( root == null )
				return new Point3D( m_Location.m_X, m_Location.m_Y, m_Location.m_Z + ItemData.CalcHeight );
			else
				return ((IEntity)root).Location;
		}

		public void SendLocalizedMessageTo( Mobile to, int number )
		{
			if ( Deleted || !to.CanSee( this ) )
				return;

			to.Send( new MessageLocalized( Serial, ItemID, MessageType.Regular, 0x3B2, 3, number, "", "" ) );
		}

		public void SendLocalizedMessageTo( Mobile to, int number, string args )
		{
			if ( Deleted || !to.CanSee( this ) )
				return;

			to.Send( new MessageLocalized( Serial, ItemID, MessageType.Regular, 0x3B2, 3, number, "", args ) );
		}

		public void SendLocalizedMessageTo( Mobile to, int number, AffixType affixType, string affix, string args )
		{
			if ( Deleted || !to.CanSee( this ) )
				return;

			to.Send( new MessageLocalizedAffix( Serial, ItemID, MessageType.Regular, 0x3B2, 3, number, "", affixType, affix, args ) );
		}

		#region OnDoubleClick[...]

		public virtual void OnDoubleClick( Mobile from )
		{
			if ( IsCowlHood( this ) )
			{
				if ( from.FindItemOnLayer( Layer.Helm ) != this )
					from.SendMessage( "You must be wearing this to change the color." );
				else
				{
					Target t;
					from.SendMessage( "What worn item do you wish to match the color of?" );
					t = new HatTarget( this );
					from.Target = t;
				}
			}
		}

		public virtual void OnDoubleClickOutOfRange( Mobile from )
		{
		}

		public virtual void OnDoubleClickCantSee( Mobile from )
		{
		}

		public virtual void OnDoubleClickDead( Mobile from )
		{
			from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019048 ); // I am dead and cannot do that.
		}

		public virtual void OnDoubleClickNotAccessible( Mobile from )
		{
			from.SendLocalizedMessage( 500447 ); // That is not accessible.
		}

		public virtual void OnDoubleClickSecureTrade( Mobile from )
		{
			from.SendLocalizedMessage( 500447 ); // That is not accessible.
		}
		#endregion

		public virtual void OnSnoop( Mobile from )
		{
		}

		public bool InSecureTrade
		{
			get
			{
				return ( GetSecureTradeCont() != null );
			}
		}

		public SecureTradeContainer GetSecureTradeCont()
		{
			object p = this;

			while ( p is Item )
			{
				if ( p is SecureTradeContainer )
					return (SecureTradeContainer)p;

				p = ((Item)p).m_Parent;
			}

			return null;
		}

		public virtual void OnItemAdded( Item item )
		{
			if ( m_Parent is Item )
				((Item)m_Parent).OnSubItemAdded( item );
			else if ( m_Parent is Mobile )
				((Mobile)m_Parent).OnSubItemAdded( item );
		}

		public virtual void OnItemRemoved( Item item )
		{
			if ( m_Parent is Item )
				((Item)m_Parent).OnSubItemRemoved( item );
			else if ( m_Parent is Mobile )
				((Mobile)m_Parent).OnSubItemRemoved( item );
		}

		public virtual void OnSubItemAdded( Item item )
		{
			if ( m_Parent is Item )
				((Item)m_Parent).OnSubItemAdded( item );
			else if ( m_Parent is Mobile )
				((Mobile)m_Parent).OnSubItemAdded( item );
		}

		public virtual void OnSubItemRemoved( Item item )
		{
			if ( m_Parent is Item )
				((Item)m_Parent).OnSubItemRemoved( item );
			else if ( m_Parent is Mobile )
				((Mobile)m_Parent).OnSubItemRemoved( item );
		}

		public virtual void OnItemBounceCleared( Item item )
		{
			if ( m_Parent is Item )
				((Item)m_Parent).OnSubItemBounceCleared( item );
			else if ( m_Parent is Mobile )
				((Mobile)m_Parent).OnSubItemBounceCleared( item );
		}

		public virtual void OnSubItemBounceCleared( Item item )
		{
			if ( m_Parent is Item )
				((Item)m_Parent).OnSubItemBounceCleared( item );
			else if ( m_Parent is Mobile )
				((Mobile)m_Parent).OnSubItemBounceCleared( item );
		}

		public virtual bool CheckTarget( Mobile from, Server.Targeting.Target targ, object targeted )
		{
			if ( m_Parent is Item )
				return ((Item)m_Parent).CheckTarget( from, targ, targeted );
			else if ( m_Parent is Mobile )
				return ((Mobile)m_Parent).CheckTarget( from, targ, targeted );

			return true;
		}

		public virtual bool IsAccessibleTo( Mobile check )
		{
			if ( m_Parent is Item )
				return ((Item)m_Parent).IsAccessibleTo( check );

			Region reg = Region.Find( GetWorldLocation(), m_Map );

			return reg.CheckAccessibility( this, check );

			/*SecureTradeContainer cont = GetSecureTradeCont();

			if ( cont != null && !cont.IsChildOf( check ) )
				return false;

			return true;*/
		}

		public bool IsChildOf( object o )
		{
			return IsChildOf( o, false );
		}

		public bool IsChildOf( object o, bool allowNull )
		{
			object p = m_Parent;

			if ( (p == null || o == null) && !allowNull )
				return false;

			if ( p == o )
				return true;

			while ( p is Item )
			{
				Item item = (Item)p;

				if ( item.m_Parent == null )
				{
					break;
				}
				else
				{
					p = item.m_Parent;

					if ( p == o )
						return true;
				}
			}

			return false;
		}

		public ItemData ItemData
		{
			get
			{
				return TileData.ItemTable[m_ItemID & TileData.MaxItemValue];
			}
		}

		public virtual void OnItemUsed( Mobile from, Item item )
		{
			if ( m_Parent is Item )
				((Item)m_Parent).OnItemUsed( from, item );
			else if ( m_Parent is Mobile )
				((Mobile)m_Parent).OnItemUsed( from, item );
		}

		public bool CheckItemUse( Mobile from )
		{
			return CheckItemUse( from, this );
		}

		public virtual bool CheckItemUse( Mobile from, Item item )
		{
			if ( m_Parent is Item )
				return ((Item)m_Parent).CheckItemUse( from, item );
			else if ( m_Parent is Mobile )
				return ((Mobile)m_Parent).CheckItemUse( from, item );
			else
				return true;
		}

		public virtual void OnItemLifted( Mobile from, Item item )
		{
			if ( m_Parent is Item )
				((Item)m_Parent).OnItemLifted( from, item );
			else if ( m_Parent is Mobile )
				((Mobile)m_Parent).OnItemLifted( from, item );
		}

		public bool CheckLift( Mobile from )
		{
			LRReason reject = LRReason.Inspecific;

			return CheckLift( from, this, ref reject );
		}

		public virtual bool CheckLift( Mobile from, Item item, ref LRReason reject )
		{
			if ( m_Parent is Item )
				return ((Item)m_Parent).CheckLift( from, item, ref reject );
			else if ( m_Parent is Mobile )
				return ((Mobile)m_Parent).CheckLift( from, item, ref reject );
			else
				return true;
		}

		public virtual bool CanTarget{ get{ return true; } }
		public virtual bool DisplayLootType{ get{ return true; } }

		public virtual void OnSingleClickContained( Mobile from, Item item )
		{
			if ( m_Parent is Item )
				((Item)m_Parent).OnSingleClickContained( from, item );
		}

		public virtual void OnAosSingleClick( Mobile from )
		{
			ObjectPropertyList opl = this.PropertyList;

			if ( opl.Header > 0 )
				from.Send( new MessageLocalized( m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, opl.Header, this.Name, opl.HeaderArgs ) );
		}

		public virtual void OnSingleClick( Mobile from )
		{
			if ( Deleted || !from.CanSee( this ) )
				return;

			if ( DisplayLootType )
				LabelLootTypeTo( from );

			NetState ns = from.NetState;

			if ( ns != null )
			{
				if ( this.Name == null )
				{
					if ( m_Amount <= 1 )
						ns.Send( new MessageLocalized( m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, LabelNumber, "", "" ) );
					else
						ns.Send( new MessageLocalizedAffix( m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, LabelNumber, "", AffixType.Append, String.Format( " : {0}", m_Amount ), "" ) );
				}
				else
				{
					ns.Send( new UnicodeMessage( m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, "ENU", "", this.Name + ( m_Amount > 1 ? " : " + m_Amount : "" ) ) );
				}
			}
		}

		private static bool m_ScissorCopyLootType;

		public static bool ScissorCopyLootType
		{
			get{ return m_ScissorCopyLootType; }
			set{ m_ScissorCopyLootType = value; }
		}

		public virtual void ScissorHelper( Mobile from, Item newItem, int amountPerOldItem )
		{
			ScissorHelper( from, newItem, amountPerOldItem, true );
		}

		public virtual void ScissorHelper( Mobile from, Item newItem, int amountPerOldItem, bool carryHue )
		{
			int amount = Amount;

			if ( amount > (60000 / amountPerOldItem) ) // let's not go over 60000
				amount = (60000 / amountPerOldItem);

			Amount -= amount;

			int ourHue = Hue;
			Map thisMap = this.Map;
			object thisParent = this.m_Parent;
			Point3D worldLoc = this.GetWorldLocation();
			LootType type = this.LootType;

			if ( Amount == 0 )
				Delete();

			newItem.Amount = amount * amountPerOldItem;

			if ( carryHue )
				newItem.Hue = ourHue;

			if ( m_ScissorCopyLootType )
				newItem.LootType = type;

			if ( !(thisParent is Container) || !((Container)thisParent).TryDropItem( from, newItem, false ) )
				newItem.MoveToWorld( worldLoc, thisMap );
		}

		public virtual void Consume()
		{
			Consume( 1 );
		}

		public virtual void Consume( int amount )
		{
			this.Amount -= amount;

			if ( this.Amount <= 0 )
				this.Delete();
		}

		public void ConsumeLimits( int amount )
		{
			--Limits;
		}

		public void ConsumeEnchants( int amount )
		{
			if ( EnchantMod > 0 )
				EnchantUses -= EnchantMod;
			else
				--EnchantUses;

			if ( EnchantUses < 1 )
				EnchantUses = 0;
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool QuestItem
		{
			get { return GetFlag( ImplFlag.QuestItem ); }
			set 
			{ 
				SetFlag( ImplFlag.QuestItem, value ); 

				InvalidateProperties();

				ReleaseWorldPackets();

				Delta( ItemDelta.Update );
			}
		}

		public bool Insured
		{
			get{ return GetFlag( ImplFlag.Insured ); }
			set{ SetFlag( ImplFlag.Insured, value ); InvalidateProperties(); }
		}

		public bool PayedInsurance
		{
			get{ return GetFlag( ImplFlag.PayedInsurance ); }
			set{ SetFlag( ImplFlag.PayedInsurance, value ); }
		}

		public Mobile BlessedFor
		{
			get
			{
				CompactInfo info = LookupCompactInfo();

				if ( info != null )
					return info.m_BlessedFor;

				return null;
			}
			set
			{
				CompactInfo info = AcquireCompactInfo();

				info.m_BlessedFor = value;

				if ( info.m_BlessedFor == null )
					VerifyCompactInfo();

				InvalidateProperties();
			}
		}

		public virtual bool CheckBlessed( object obj )
		{
			return CheckBlessed( obj as Mobile );
		}

		public virtual bool CheckBlessed( Mobile m )
		{
			if ( m_LootType == LootType.Blessed || (Mobile.InsuranceEnabled && Insured) )
				return true;

			return ( m != null && m == this.BlessedFor );
		}

		public virtual bool CheckNewbied()
		{
			return ( m_LootType == LootType.Newbied );
		}

		public virtual bool IsStandardLoot()
		{
			if ( Mobile.InsuranceEnabled && Insured )
				return false;

			if ( this.BlessedFor != null )
				return false;

			return ( m_LootType == LootType.Regular );
		}


		private class HatTarget : Target
		{
			private Item m_Hats;

			public HatTarget( Item cowl ) : base( 1, false, TargetFlags.None )
			{
				m_Hats = cowl;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Item )
				{
					Item iColorHat = targeted as Item;

					int color = 0;

					if ( from.FindItemOnLayer( Layer.Helm ) != m_Hats ) { from.SendMessage( "You must be wearing this to change the color." ); }
					else if ( from.FindItemOnLayer( Layer.Waist ) == iColorHat ) { color = iColorHat.Hue; }
					else if ( from.FindItemOnLayer( Layer.OuterTorso ) == iColorHat ) { color = iColorHat.Hue; }
					else if ( from.FindItemOnLayer( Layer.Arms ) == iColorHat ) { color = iColorHat.Hue; }
					else if ( from.FindItemOnLayer( Layer.OuterLegs ) == iColorHat ) { color = iColorHat.Hue; }
					else if ( from.FindItemOnLayer( Layer.Neck ) == iColorHat ) { color = iColorHat.Hue; }
					else if ( from.FindItemOnLayer( Layer.Gloves ) == iColorHat ) { color = iColorHat.Hue; }
					else if ( from.FindItemOnLayer( Layer.Shoes ) == iColorHat ) { color = iColorHat.Hue; }
					else if ( from.FindItemOnLayer( Layer.Cloak ) == iColorHat ) { color = iColorHat.Hue; }
					else if ( from.FindItemOnLayer( Layer.FirstValid ) == iColorHat ) { color = iColorHat.Hue; }
					else if ( from.FindItemOnLayer( Layer.InnerLegs ) == iColorHat ) { color = iColorHat.Hue; }
					else if ( from.FindItemOnLayer( Layer.InnerTorso ) == iColorHat ) { color = iColorHat.Hue; }
					else if ( from.FindItemOnLayer( Layer.Pants ) == iColorHat ) { color = iColorHat.Hue; }
					else if ( from.FindItemOnLayer( Layer.Shirt ) == iColorHat ) { color = iColorHat.Hue; }
					else
					{
						from.SendMessage( "You can only match colors of certain equipped items." );
					}

					if ( color > 0 )
					{
						m_Hats.Hue = color;
						from.SendMessage( "You change the color to match the item." );
					}
					else
					{
						from.SendMessage( "Items selected must have a distinct color." );
					}
				}
				else
				{
					from.SendMessage( "You can only match color of certain equipped items that have distinct colors." );
				}
			}
		}

		public override string ToString()
		{
			return String.Format( "0x{0:X} \"{1}\"", m_Serial.Value, GetType().Name );
		}

		internal int m_TypeRef;

		public Item()
		{
			m_Serial = Serial.NewItem;

			Visible = true;
			Movable = true;
			Amount = 1;
			m_Map = Map.Internal;

			SetLastMoved();

			World.AddItem( this );

			Type ourType = this.GetType();
			m_TypeRef = World.m_ItemTypes.IndexOf( ourType );

			if ( m_TypeRef == -1 )
			{
				World.m_ItemTypes.Add( ourType );
				m_TypeRef = World.m_ItemTypes.Count - 1;
			}
		}

		[Constructable]
		public Item( int itemID ) : this()
		{
			m_ItemID = itemID;
			SyncItem();
			m_Resource = DefaultResource;
			m_InfoData = DefaultDescription;
			m_Catalog = DefaultCatalog;
		}

		public Item( Serial serial )
		{
			m_Serial = serial;

			Type ourType = this.GetType();
			m_TypeRef = World.m_ItemTypes.IndexOf( ourType );

			if ( m_TypeRef == -1 )
			{
				World.m_ItemTypes.Add( ourType );
				m_TypeRef = World.m_ItemTypes.Count - 1;
			}
		}

		public virtual void OnSectorActivate()
		{
		}

		public virtual void OnSectorDeactivate()
		{
		}

		public static bool IsStandardResource( CraftResource resource )
		{
			return ( resource == CraftResource.None || resource == CraftResource.Iron || resource == CraftResource.Fabric || resource == CraftResource.RegularLeather || resource == CraftResource.RegularWood || resource == CraftResource.BrittleSkeletal );
		}
	}

	public enum Catalogs
	{
		None = 0,
		Body = 1,
		Book,
		Crafting,
		Jewelry,
		Potion,
		Reagent,
		SciFi,
		Scroll,
		Stone,
		TreasureChest,
		Trinket,
		Gem,
		Tool,
		Wax
	}

	public enum Identity
	{
		None = 0,
		Archer = 1,
		Armor,
		Artifact,
		Book,
		Clothing,
		Coins,
		Jewelry,
		Leather,
		Magic,
		Music,
		Potion,
		Reagent,
		Scroll,
		Wand,
		Weapon,
		Wood,
		Merchant
	}

	public enum IDSkill
	{
		None = 0,
		Mercantile = 1,
		ArmsLore,
		Tasting
	}

	public enum Difficulty
	{
		Easy = 0,
		Normal = 1,
		Difficult,
		Challenging,
		Hard,
		Deadly,
		Epic
	}

	public enum Density
	{
		None = 0,
		Weak = 1,
		Regular,
		Great,
		Greater,
		Superior,
		Ultimate
	}

	public enum CraftResource
	{
		None = 0,
		Iron = 1,
		DullCopper,
		ShadowIron,
		Copper,
		Bronze,
		Gold,
		Agapite,
		Verite,
		Valorite,
		Nepturite,
		Obsidian,
		Steel,
		Brass,
		Mithril,
		Xormite,
		Dwarven,
		Agrinium,
		Beskar,
		Carbonite,
		Cortosis,
		Durasteel,
		Durite,
		Farium,
		Laminasteel,
		Neuranium,
		Phrik,
		Promethium,
		Quadranium,
		Songsteel,
		Titanium,
		Trimantium,
		Xonolite,

		AmethystBlock = 71,
		EmeraldBlock,
		GarnetBlock,
		IceBlock,
		JadeBlock,
		MarbleBlock,
		OnyxBlock,
		QuartzBlock,
		RubyBlock,
		SapphireBlock,
		SilverBlock,
		SpinelBlock,
		StarRubyBlock,
		TopazBlock,
		CaddelliteBlock,

		RegularLeather = 101,
		HornedLeather, 
		BarbedLeather, 
		NecroticLeather, 
		VolcanicLeather, 
		FrozenLeather, 
		SpinedLeather, 
		GoliathLeather, 
		DraconicLeather, 
		HellishLeather, 
		DinosaurLeather,
		AlienLeather,
		Adesote,
		Biomesh,
		Cerlin,
		Durafiber,
		Flexicris,
		Hypercloth,
		Nylar,
		Nylonite,
		Polyfiber,
		Syncloth,
		Thermoweave,

		DemonSkin = 151,
		DragonSkin,
		NightmareSkin,
		SnakeSkin,
		TrollSkin,
		UnicornSkin,
		IcySkin,
		LavaSkin,
		Seaweed,
		DeadSkin,

		RedScales = 201,
		YellowScales,
		BlackScales,
		GreenScales,
		WhiteScales,
		BlueScales,
		DinosaurScales,
		MetallicScales,
		BrazenScales,
		UmberScales,
		VioletScales,
		PlatinumScales,
		CadalyteScales,
		GornScales,
		TrandoshanScales,
		SilurianScales,
		KraytScales,

		Fabric = 251,
		FurryFabric,
		WoolyFabric,
		SilkFabric,
		HauntedFabric,
		ArcticFabric,
		PyreFabric,
		VenomousFabric,
		MysteriousFabric,
		VileFabric,
		DivineFabric,
		FiendishFabric,

		RegularWood = 301,
		AshTree,
		CherryTree,
		EbonyTree,
		GoldenOakTree,
		HickoryTree,
		MahoganyTree,
		OakTree,
		PineTree,
		GhostTree,
		RosewoodTree,
		WalnutTree,
		PetrifiedTree,
		DriftwoodTree,
		ElvenTree,
		BorlTree,
		CosianTree,
		GreelTree,
		JaporTree,
		KyshyyykTree,
		LaroonTree,
		TeejTree,
		VeshokTree,

		BrittleSkeletal = 401,
		DrowSkeletal,
		OrcSkeletal,
		ReptileSkeletal,
		OgreSkeletal,
		TrollSkeletal,
		GargoyleSkeletal,
		MinotaurSkeletal,
		LycanSkeletal,
		SharkSkeletal,
		ColossalSkeletal,
		MysticalSkeletal,
		VampireSkeletal,
		LichSkeletal,
		SphinxSkeletal,
		DevilSkeletal,
		DracoSkeletal,
		XenoSkeletal,
		AndorianSkeletal,
		CardassianSkeletal,
		MartianSkeletal,
		RodianSkeletal,
		TuskenSkeletal,
		TwilekSkeletal,
		XindiSkeletal,
		ZabrakSkeletal,

		SpectralSpec = 501,
		DreadSpec,
		GhoulishSpec,
		WyrmSpec,
		HolySpec,
		BloodlessSpec,
		GildedSpec,
		DemilichSpec,
		WintrySpec,
		FireSpec,
		ColdSpec,
		PoisSpec,
		EngySpec,
		ExodusSpec,
		TurtleSpec
	}

	public enum MagicSpell
	{
		None = 0,
		Clumsy = 1,
		CreateFood,
		Feeblemind,
		Heal,
		MagicArrow,
		NightSight,
		ReactiveArmor,
		Weaken,

		Agility,
		Cunning,
		Cure,
		Harm,
		MagicTrap,
		RemoveTrap,
		Protection,
		Strength,

		Bless,
		Fireball,
		MagicLock,
		Poison,
		Telekinesis,
		Teleport,
		Unlock,
		WallOfStone,

		ArchCure,
		ArchProtection,
		Curse,
		FireField,
		GreaterHeal,
		Lightning,
		ManaDrain,
		Recall,

		BladeSpirits,
		DispelField,
		Incognito,
		MagicReflect,
		MindBlast,
		Paralyze,
		PoisonField,
		SummonCreature,

		Dispel,
		EnergyBolt,
		Explosion,
		Invisibility,
		Mark,
		MassCurse,
		ParalyzeField,
		Reveal,

		ChainLightning,
		EnergyField,
		FlameStrike,
		GateTravel,
		ManaVampire,
		MassDispel,
		MeteorSwarm,
		Polymorph,

		Earthquake,
		EnergyVortex,
		Resurrection,
		AirElemental,
		SummonDaemon,
		EarthElemental,
		FireElemental,
		WaterElemental,

		SummonSnakes,
		SummonDragon,
		SummonSkeleton,
		Identify,

		CurseWeapon,
		BloodOath,
		CorpseSkin,
		EvilOmen,
		PainSpike,
		WraithForm,
		MindRot,
		SummonFamiliar,
		AnimateDead,
		HorrificBeast,
		PoisonStrike,
		Wither,
		Strangle,
		LichForm,
		Exorcism,
		VengefulSpirit,
		VampiricEmbrace
	}
}
