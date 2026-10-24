using System;
using Server;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Targets;
using Server.Targeting;
using Server.Network;
using Server.Items;
using Server.Spells;

namespace Server.Network
{
	public class UpdateStatueAnimation : Packet
	{
		public UpdateStatueAnimation( Mobile m, int status, int animation, int frame ) : base( 0xBF, 17 )
		{
			m_Stream.Write( (short) 0x11 );
			m_Stream.Write( (short) 0x19 );
			m_Stream.Write( (byte) 0x5 );
			m_Stream.Write( (int) m.Serial );
			m_Stream.Write( (byte) 0 );
			m_Stream.Write( (byte) 0xFF );
			m_Stream.Write( (byte) status );
			m_Stream.Write( (byte) 0 );
			m_Stream.Write( (byte) animation );
			m_Stream.Write( (byte) 0 );
			m_Stream.Write( (byte) frame );
		}
	}
}

namespace Server.Mobiles
{
	public enum StatueType
	{
		Marble,
		Jade,
		Bronze
	}

	public enum StatuePose
	{
		Ready,
		Casting,
		Salute,
		AllPraiseMe,
		Fighting,
		HandsOnHips
	}

	public enum StatueMaterial
	{
		Antique,
		Dark,
		Medium, 
		Light
	}

	public class CharacterStatue : Mobile
	{
		private StatueType m_Type;
		private StatuePose m_Pose;
		private StatueMaterial m_Material;

		[CommandProperty( AccessLevel.GameMaster )]
		public StatueType StatueType
		{
			get { return m_Type; }
			set { m_Type = value; InvalidateHues(); InvalidatePose(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public StatuePose Pose
		{
			get { return m_Pose; }
			set { m_Pose = value; InvalidatePose(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public StatueMaterial Material
		{
			get { return m_Material; }
			set { m_Material = value; InvalidateHues(); InvalidatePose(); }
		}

		private Mobile m_SculptedBy;
		private DateTime m_SculptedOn;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile SculptedBy
		{
			get{ return m_SculptedBy; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime SculptedOn
		{
			get{ return m_SculptedOn; }
		}

		private CharacterStatuePlinth m_Plinth;

		public CharacterStatuePlinth Plinth
		{
			get { return m_Plinth; }
			set { m_Plinth = value; }
		}

		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get{ return m_IsRewardItem; }
			set{ m_IsRewardItem = value; }
		}

		public CharacterStatue( Mobile from, StatueType type ) : base()
		{
			m_Type = type;
			m_Pose = StatuePose.Ready;
			m_Material = StatueMaterial.Antique;

			Direction = Direction.South;
			AccessLevel = AccessLevel.Counselor;
			Hits = HitsMax;
			Blessed = true;
			Frozen = true;

			CloneBody( from );
			CloneClothes( from );
			InvalidateHues();
		}

		public CharacterStatue( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			DisplayPaperdollTo( from );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_SculptedBy != null )
			{
				if ( m_SculptedBy.Title != null )
					list.Add( 1076202, m_SculptedBy.Title + " " + m_SculptedBy.Name ); // Sculpted by ~1_Name~
				else
					list.Add( 1076202, m_SculptedBy.Name ); // Sculpted by ~1_Name~
			}
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from.Alive && m_SculptedBy != null )
			{
				BaseHouse house = BaseHouse.FindHouseAt( this );

				if ( ( house != null && house.IsCoOwner( from ) ) || (int) from.AccessLevel > (int) AccessLevel.Counselor )
					list.Add( new DemolishEntry( this ) );
			}
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if ( m_Plinth != null && !m_Plinth.Deleted )
				m_Plinth.Delete();
		}

		protected override void OnMapChange( Map oldMap )
		{
			InvalidatePose();

			if ( m_Plinth != null )
				m_Plinth.Map = Map;
		}

		protected override void OnLocationChange( Point3D oldLocation )
		{
			if ( m_Plinth != null )
			{
				Point2D offset = m_Plinth.StatueOffset;
				m_Plinth.Location = new Point3D( X - offset.X, Y - offset.Y, Z - 5 );
			}

			InvalidatePose();
		}

		public override bool CanBeRenamedBy( Mobile from )
		{
			return false;
		}

		public override bool CanBeDamaged()
		{
			return false;
		}

		public void OnRequestedAnimation( Mobile from )
		{				
			from.Send( new UpdateStatueAnimation( this, 1, m_Animation, m_Frames ) );
		}

		public override void OnAosSingleClick( Mobile from )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.Write( (int) m_Type );
			writer.Write( (int) m_Pose );
			writer.Write( (int) m_Material );

			writer.Write( (Mobile) m_SculptedBy );
			writer.Write( (DateTime) m_SculptedOn );

			writer.Write( (Item) m_Plinth );
			writer.Write( (bool) m_IsRewardItem );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_Type = (StatueType) reader.ReadInt();
			m_Pose = (StatuePose) reader.ReadInt();
			m_Material = (StatueMaterial) reader.ReadInt();

			m_SculptedBy = reader.ReadMobile();
			m_SculptedOn = reader.ReadDateTime();

			m_Plinth = reader.ReadItem() as CharacterStatuePlinth;
			m_IsRewardItem = reader.ReadBool();

			InvalidatePose();

			Frozen = true;

			if( m_SculptedBy == null )
			{
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( Delete ) );
			}
		}
		
		public void Sculpt( Mobile by )
		{
			Sculpt( by, DateTime.Now );
		}

		public void Sculpt( Mobile by, DateTime on )
		{
			m_SculptedBy = by;
			m_SculptedOn = on;

			InvalidateProperties();
		}

		public void Demolish( Mobile by )
		{
			CharacterStatueDeed deed = new CharacterStatueDeed( this );
			deed.IsRewardItem = m_IsRewardItem;

			if ( by.PlaceInBackpack( deed ) )
			{
				CharacterStatuePlinth plinth = m_Plinth;
				m_Plinth = null;

				if ( plinth != null )
				{
					plinth.ReleaseStatue();
					plinth.Delete();
				}

				Internalize();
			}
			else
			{
				by.SendLocalizedMessage( 500720 ); // You don't have enough room in your backpack!
				deed.Statue = null;
				deed.Delete();
			}
		}

		public void Restore( CharacterStatue from )
		{
			m_Material = from.Material;
			m_Pose = from.Pose;

			Direction = from.Direction;

			CloneBody( from );
			CloneClothes( from );

			InvalidateHues();
			InvalidatePose();
		}

		public void CloneBody( Mobile from )
		{
			Name = from.Name;
			BodyValue = from.BodyValue;
			HairItemID = from.HairItemID;
			FacialHairItemID = from.FacialHairItemID;
		}

		public void CloneClothes( Mobile from )
		{
			for ( int i = Items.Count - 1; i >= 0; i -- )
				Items[ i ].Delete();

			for ( int i = from.Items.Count - 1; i >= 0; i -- )
			{
				Item item = from.Items[ i ];

				if ( item.Layer != Layer.Backpack && item.Layer != Layer.Mount && item.Layer != Layer.Bank )
					AddItem( CloneItem( item ) );
			}
		}

		public Item CloneItem( Item item )
		{
			Item cloned = new Item( item.ItemID );
			cloned.Layer = item.Layer;
			cloned.Name = item.Name;
			cloned.Hue = item.Hue;
			cloned.Weight = item.Weight;
			cloned.Movable = false;
			
			return cloned;
		}

		public void InvalidateHues()
		{
			Hue = 0xB8F + (int) m_Type * 4 + (int) m_Material;

			HairHue = Hue;

			if ( FacialHairItemID > 0 )
				FacialHairHue = Hue;

			for ( int i = Items.Count - 1; i >= 0; i -- )
				Items[ i ].Hue = Hue;

			if ( m_Plinth != null )
				m_Plinth.InvalidateHue();
		}

		private int m_Animation;
		private int m_Frames;

		public void InvalidatePose()
		{
			switch ( m_Pose )
			{
				case StatuePose.Ready: 
						m_Animation = 4;
						m_Frames = 0;
						break;
				case StatuePose.Casting:
						m_Animation = 16;
						m_Frames = 2;
						break;
				case StatuePose.Salute:
						m_Animation = 33;
						m_Frames = 1;
						break;
				case StatuePose.AllPraiseMe:
						m_Animation = 17;
						m_Frames = 4;
						break;
				case StatuePose.Fighting:
						m_Animation = 31;
						m_Frames = 5;
						break;
				case StatuePose.HandsOnHips:
						m_Animation = 6;
						m_Frames = 1;
						break;
			}

			// The poses above are human animations; other bodies number their animations differently, so they stand idle instead.
			if ( !Body.IsHuman )
			{
				m_Animation = Body.IsMonster ? 1 : 2;
				m_Frames = 0;
			}

			if ( m_Plinth != null && !World.Loading )
				m_Plinth.UpdateFootprint();

			if( Map != null )
			{
				ProcessDelta();

				Packet p = null;

				IPooledEnumerable eable = Map.GetClientsInRange( Location );

				foreach( NetState state in eable )
				{
					state.Mobile.ProcessDelta();

					if( p == null )
						p = Packet.Acquire( new UpdateStatueAnimation( this, 1, m_Animation, m_Frames ) );

					state.Send( p );
				}

				Packet.Release( p );

				eable.Free();
			}
		}

		private class DemolishEntry : ContextMenuEntry
		{
			private CharacterStatue m_Statue;

			public DemolishEntry( CharacterStatue statue ) : base( 6275, 2 )
			{
				m_Statue = statue;
			}

			public override void OnClick()
			{
				if ( m_Statue.Deleted )
					return;

				m_Statue.Demolish( Owner.From );
			}
		}
	}

	public class CharacterStatueDeed : Item
	{
		public override int LabelNumber
		{ 
			get
			{ 
				if ( m_Statue != null )
				{
					switch ( m_Statue.StatueType )
					{
						case StatueType.Marble: return 1076189;
						case StatueType.Jade: return 1076188;
						case StatueType.Bronze: return 1076190;
					}
				}

				return 1076173; 
			} 
		}

		private CharacterStatue m_Statue;
		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public CharacterStatue Statue
		{
			get { return m_Statue; }
			set { m_Statue = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public StatueType StatueType
		{
			get
			{ 
				if ( m_Statue != null )
					return m_Statue.StatueType; 

				return StatueType.Marble;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get{ return m_IsRewardItem; }
			set{ m_IsRewardItem = value; InvalidateProperties(); }
		}

		public CharacterStatueDeed( CharacterStatue statue ) : base( 0x14F0 )
		{
			m_Statue = statue;
		
			LootType = LootType.Blessed;
			Weight = 1.0;
		}

		public CharacterStatueDeed( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Statue != null )
				list.Add( 1076231, m_Statue.Name ); // Statue of ~1_Name~
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1076194 ); // Select a place where you would like to put your statue.
				from.Target = new CharacterStatueTarget( this, StatueType );
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		public override void OnDelete()
		{
			base.OnDelete();

			if ( m_Statue != null )
				m_Statue.Delete();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.Write( (Mobile) m_Statue );
			writer.Write( (bool) m_IsRewardItem );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_Statue = reader.ReadMobile() as CharacterStatue;
			m_IsRewardItem = reader.ReadBool();
		}
	}
	
	public class CharacterStatueSubjectTarget : Target
	{
		private Item m_Maker;
		private StatueType m_Type;
		private BaseHouse m_House;

		public CharacterStatueSubjectTarget( Item maker, StatueType type, BaseHouse house ) : base( 12, false, TargetFlags.None )
		{
			m_Maker = maker;
			m_Type = type;
			m_House = house;
		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			if ( m_Maker == null || m_Maker.Deleted )
				return;

			PlayerMobile subject = targeted as PlayerMobile;

			if ( !m_Maker.IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else if ( CharacterStatueTarget.FindSculptingHouse( from ) != m_House )
				from.SendLocalizedMessage( 502092 ); // You must be in your house to do this.
			else if ( subject == null )
				from.SendMessage( "You can only make a statue of a player." );
			else if ( !subject.Alive )
				from.SendMessage( "You cannot make a statue of a ghost." );
			else if ( !CharacterStatueTarget.InOriginalForm( subject ) )
				from.SendMessage( "They must be in their original form to be sculpted." );
			else
			{
				from.SendLocalizedMessage( 1076194 ); // Select a place where you would like to put your statue.
				from.Target = new CharacterStatueTarget( m_Maker, m_Type, subject );
			}
		}
	}

	public class CharacterStatueTarget : Target
	{
		private Item m_Maker;
		private StatueType m_Type;
		private Mobile m_Subject;

		public CharacterStatueTarget( Item maker, StatueType type ) : this( maker, type, null )
		{
		}

		public CharacterStatueTarget( Item maker, StatueType type, Mobile subject ) : base( -1, true, TargetFlags.None )
		{
			m_Maker = maker;
			m_Type = type;
			m_Subject = subject;
		}

		public static BaseHouse FindSculptingHouse( Mobile from )
		{
			BaseHouse house = BaseHouse.FindHouseAt( from );

			if ( house == null || !house.IsCoOwner( from ) )
				return null;

			return house;
		}

		public static bool InOriginalForm( Mobile m )
		{
			return !m.IsBodyMod || ( m.RaceID > 0 && m.BodyMod.BodyID == m.RaceID );
		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			IPoint3D p = targeted as IPoint3D;
			Map map = from.Map;

			if ( p == null || map == null || m_Maker == null || m_Maker.Deleted )
				return;

			CharacterStatue backup = m_Maker is CharacterStatueDeed ? ( (CharacterStatueDeed) m_Maker ).Statue : null;
			Mobile subject = backup != null ? backup : ( m_Subject != null ? m_Subject : from );

			if ( m_Maker.IsChildOf( from.Backpack ) )
			{
				SpellHelper.GetSurfaceTop( ref p );
				BaseHouse house = null;
				Point3D loc = new Point3D( p );

				if ( targeted is Item && !((Item) targeted).IsLockedDown && !((Item) targeted).IsSecure && !(targeted is AddonComponent) )
				{
					from.SendLocalizedMessage( 1076191 ); // Statues can only be placed in houses.
					return;
				}
				else if ( subject.Deleted || !subject.Alive )
				{
					from.SendMessage( "The subject of your statue is no longer available." );
					return;
				}
				else if ( !InOriginalForm( subject ) )
				{
					if ( subject == from )
						from.SendLocalizedMessage( 1073648 ); // You may only proceed while in your original state...
					else
						from.SendMessage( "They must be in their original form to be sculpted." );
					return;
				}

				AddonFitResult result = CouldFit( loc, map, from, ref house, CharacterStatuePlinth.IsLargeBody( subject.Body ) );

				if ( result == AddonFitResult.Valid )
				{
					CharacterStatue statue = new CharacterStatue( subject, m_Type );
					CharacterStatuePlinth plinth = new CharacterStatuePlinth( statue );

					house.Addons.Add( plinth );

					statue.Plinth = plinth;
					plinth.MoveToWorld( loc, map );

					if ( backup != null )
						statue.Restore( backup );
					else
						statue.InvalidatePose();

					plinth.UpdateFootprint();

					from.CloseGump( typeof( CharacterStatueGump ) );
					from.SendGump( new CharacterStatueGump( m_Maker, statue, from ) );
				}
				else if ( result == AddonFitResult.Blocked )
					from.SendLocalizedMessage( 500269 ); // You cannot build that there.
				else if ( result == AddonFitResult.NotInHouse )
					from.SendLocalizedMessage( 1076192 ); // Statues can only be placed in houses where you are the owner or co-owner.
				else if ( result == AddonFitResult.DoorTooClose )
					from.SendLocalizedMessage( 500271 ); // You cannot build near the door.
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		public static AddonFitResult CouldFit( Point3D p, Map map, Mobile from, ref BaseHouse house )
		{
			return CouldFit( p, map, from, ref house, false );
		}

		public static AddonFitResult CouldFit( Point3D p, Map map, Mobile from, ref BaseHouse house, bool large )
		{
			house = BaseHouse.FindHouseAt( p, map, 20 );

			if ( house == null || FindSculptingHouse( from ) != house )
				return AddonFitResult.NotInHouse;

			foreach ( Point3D tile in CharacterStatuePlinth.Footprint( p, large ) )
			{
				if ( !map.CanFit( tile.X, tile.Y, tile.Z, 20, true, true, true ) )
					return AddonFitResult.Blocked;

				if ( BaseHouse.FindHouseAt( tile, map, 20 ) != house )
					return AddonFitResult.NotInHouse;

				if ( CheckDoors( tile, 20, house ) != AddonFitResult.Valid )
					return AddonFitResult.DoorTooClose;
			}

			return AddonFitResult.Valid;
		}

		public static AddonFitResult CheckDoors( Point3D p, int height, BaseHouse house )
		{
			ArrayList doors = house.Doors;

			for ( int i = 0; i < doors.Count; i ++ )
			{
				BaseDoor door = doors[ i ] as BaseDoor;

				Point3D doorLoc = door.GetWorldLocation();
				int doorHeight = door.ItemData.CalcHeight;

				if ( Utility.InRange( doorLoc, p, 1 ) && (p.Z == doorLoc.Z || ((p.Z + height) > doorLoc.Z && (doorLoc.Z + doorHeight) > p.Z)) )
					return AddonFitResult.DoorTooClose;
			}

			return AddonFitResult.Valid;
		}
	}
}

namespace Server.Gumps
{
	public class CharacterStatueGump : Gump
	{
		private Item m_Maker;
		private CharacterStatue m_Statue;
		private Timer m_Timer;
		private Mobile m_Owner;
	
		private enum Buttons
		{
			Close,
			Sculpt,			
			PosePrev,
			PoseNext,
			DirPrev,
			DirNext,
			MatPrev,
			MatNext,
			Restore
		}
	
		public CharacterStatueGump( Item maker, CharacterStatue statue, Mobile owner ) : base( 60, 36 )
		{
			m_Maker = maker;
			m_Statue = statue;
			m_Owner = owner;
			
			if ( m_Statue == null )
				return;
			
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
		
			AddPage( 0 );

			AddImage(30, 22, 1140);
			AddHtml( 91, 71, 270, 26, @"<BODY><BASEFONT Color=#111111><BIG><CENTER>Character Statue Carving</CENTER></BIG></BASEFONT></BODY>", (bool)false, (bool)false, false);

			AddHtml( 92, 110, 104, 19, @"<BODY><BASEFONT Color=#111111><BIG>Direction</BIG></BASEFONT></BODY>", (bool)false, (bool)false, false);
			AddHtml( 92, 135, 104, 19, @"<BODY><BASEFONT Color=#111111><BIG>" + GetDirectionNumber( m_Statue.Direction ) + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false, false);

			AddButton(93, 165, 4014, 4014, (int)Buttons.DirNext, GumpButtonType.Reply, 0);
			AddButton(130, 165, 4005, 4005, (int)Buttons.DirPrev, GumpButtonType.Reply, 0);

			AddHtml( 255, 110, 104, 19, @"<BODY><BASEFONT Color=#111111><BIG>Material</BIG></BASEFONT></BODY>", (bool)false, (bool)false, false);
			AddHtml( 255, 135, 104, 19, @"<BODY><BASEFONT Color=#111111><BIG>" + GetMaterialNumber( m_Statue.StatueType, m_Statue.Material ) + "</BIG></BASEFONT></BODY>", (bool)false, (bool)false, false);

			AddButton(294, 165, 4014, 4014, (int)Buttons.MatNext, GumpButtonType.Reply, 0);
			AddButton(331, 165, 4005, 4005, (int)Buttons.MatPrev, GumpButtonType.Reply, 0);

			AddButton(66, 232, 241, 243, (int)Buttons.Close, GumpButtonType.Reply, 0);
			AddButton(319, 232, 247, 248, (int)Buttons.Sculpt, GumpButtonType.Reply, 0);

			// restore			
			if ( m_Maker is CharacterStatueDeed )
			{
				AddButton(197, 219, 2322, 2324, (int)Buttons.Restore, GumpButtonType.Reply, 0);
			}

			m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 2.5 ), TimeSpan.FromSeconds( 2.5 ), new TimerCallback( CheckOnline ) );
		}

		private void CheckOnline()
		{
			if ( m_Owner != null && m_Owner.NetState == null )
			{
				if ( m_Timer != null )
					m_Timer.Stop();

				if ( m_Statue != null && !m_Statue.Deleted )
					m_Statue.Delete();
			}
		}
		
		private string GetMaterialNumber( StatueType type, StatueMaterial material )
		{
			switch ( material )
			{
				case StatueMaterial.Antique:
					
					switch ( type )
					{
						case StatueType.Bronze: return "Bronze";
						case StatueType.Jade: return "Jade";
						case StatueType.Marble: return "Marble";
					}

					return "Bronze";
				case StatueMaterial.Dark: 
					
					if ( type == StatueType.Marble )
						return "Dark";

					return "Dark";
				case StatueMaterial.Medium: return "Medium";
				case StatueMaterial.Light: return "Light";	
				default: return "Bronze";
			}
		}
		
		private string GetDirectionNumber( Direction direction )
		{
			switch ( direction )
			{
				case Direction.North: return "North";
				case Direction.Right: return "Right";
				case Direction.East: return "East";
				case Direction.Down: return "Down";	
				case Direction.South: return "South";
				case Direction.Left: return "Left";
				case Direction.West: return "West";
				case Direction.Up: return "Up";
				default: return "South";
			}
		}
		
		public override void OnResponse( NetState state, RelayInfo info )
		{		
			if ( m_Statue == null || m_Statue.Deleted )
				return;
				
			bool sendGump = false;
				
			if ( info.ButtonID == (int) Buttons.Sculpt )
			{					
				Mobile sculptor = state.Mobile;
				DateTime sculptedOn = DateTime.Now;

				if ( m_Maker is CharacterStatueDeed )
				{
					CharacterStatue backup = ( (CharacterStatueDeed) m_Maker ).Statue;

					if ( backup != null )
					{
						if ( backup.SculptedBy != null )
						{
							sculptor = backup.SculptedBy;
							sculptedOn = backup.SculptedOn;
						}

						backup.Delete();
					}
				}

				if ( m_Maker != null )
					m_Maker.Delete();

				m_Statue.Sculpt( sculptor, sculptedOn );
			}
			else if ( info.ButtonID == (int) Buttons.PosePrev )
			{
				m_Statue.Pose = (StatuePose) ( ( (int) m_Statue.Pose + 5 ) % 6 );
				sendGump = true;
			}
			else if ( info.ButtonID == (int) Buttons.PoseNext )
			{
				m_Statue.Pose = (StatuePose) ( ( (int) m_Statue.Pose + 1 ) % 6 );
				sendGump = true;
			}
			else if ( info.ButtonID == (int) Buttons.DirPrev )
			{
				m_Statue.Direction = (Direction) ( ( (int) m_Statue.Direction + 7 ) % 8 );
				m_Statue.InvalidatePose();
				sendGump = true;
			}
			else if ( info.ButtonID == (int) Buttons.DirNext )
			{
				m_Statue.Direction = (Direction) ( ( (int) m_Statue.Direction + 1 ) % 8 );
				m_Statue.InvalidatePose();
				sendGump = true;
			}
			else if ( info.ButtonID == (int) Buttons.MatPrev )
			{
				m_Statue.Material = (StatueMaterial) ( ( (int) m_Statue.Material + 3 ) % 4 );
				sendGump = true;
			}
			else if ( info.ButtonID == (int) Buttons.MatNext )
			{
				m_Statue.Material = (StatueMaterial) ( ( (int) m_Statue.Material + 1 ) % 4 );
				sendGump = true;
			}
			else if ( info.ButtonID == (int) Buttons.Restore )
			{
				if ( m_Maker is CharacterStatueDeed )
				{
					CharacterStatue backup = ( (CharacterStatueDeed) m_Maker ).Statue;
					
					if ( backup != null )
						m_Statue.Restore( backup );
				}
				
				sendGump = true;
			}
			else
			{
				m_Statue.Delete();
			}

			if ( sendGump )
				state.Mobile.SendGump( new CharacterStatueGump( m_Maker, m_Statue, m_Owner ) );
			
			if ( m_Timer != null )
				m_Timer.Stop();
		}

		public override void OnServerClose( NetState owner )
		{
			if ( m_Timer != null )
				m_Timer.Stop();

			if ( m_Statue != null && !m_Statue.Deleted )
				m_Statue.Delete();
		}
	}
}

namespace Server.Items
{    
	public class CharacterStatueMaker : Item
	{
		public override int LabelNumber{ get{ return 1076173; } } // Character Statue Maker
	
		private bool m_IsRewardItem;
		private StatueType m_Type;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get{ return m_IsRewardItem; }
			set{ m_IsRewardItem = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public StatueType StatueType
		{
			get{ return m_Type; }
			set{ m_Type = value; InvalidateHue(); }
		}
		
		public CharacterStatueMaker( StatueType type ) : base( 0x32F0 )
		{
			m_Type = type;
			InvalidateHue();
			Weight = 5.0;
		}

		public CharacterStatueMaker( Serial serial ) : base( serial )
		{
		}    	
		
		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				return;
			}

			BaseHouse house = CharacterStatueTarget.FindSculptingHouse( from );

			if ( house == null )
			{
				from.SendLocalizedMessage( 502092 ); // You must be in your house to do this.
				return;
			}

			from.SendMessage( "Who do you want to make a statue of?" );
			from.Target = new CharacterStatueSubjectTarget( this, m_Type, house );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.Write( (bool) m_IsRewardItem );
			writer.Write( (int) m_Type );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_IsRewardItem = reader.ReadBool();
			m_Type = (StatueType) reader.ReadInt();
		}
		
		public void InvalidateHue()
		{
			Hue = 0xB8F + (int) m_Type * 4;
		}
	}
	
	public class MarbleStatueMaker : CharacterStatueMaker
	{
		[Constructable]
		public MarbleStatueMaker() : base( StatueType.Marble )
		{
		}
		
		public MarbleStatueMaker( Serial serial ) : base( serial )
		{
		}    	

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
	
	public class JadeStatueMaker : CharacterStatueMaker
	{
		[Constructable]
		public JadeStatueMaker() : base( StatueType.Jade )
		{
		}
		
		public JadeStatueMaker( Serial serial ) : base( serial )
		{
		}    	

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
	
	public class BronzeStatueMaker : CharacterStatueMaker
	{
		[Constructable]
		public BronzeStatueMaker() : base( StatueType.Bronze )
		{
		}
		
		public BronzeStatueMaker( Serial serial ) : base( serial )
		{
		}    	

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}

namespace Server.Items
{
	public class CharacterStatuePlinth : Static, IAddon
	{
		public Item Deed{ get{ return new CharacterStatueDeed( m_Statue ); } }
		public override int LabelNumber{ get{ return 1076201; } } // Character Statue

		private CharacterStatue m_Statue;
		private List<Item> m_Pieces = new List<Item>();

		public CharacterStatue Statue{ get{ return m_Statue; } }

		// Large races stand on a 2x2 base: the statue's tile plus these three.
		private static readonly Point2D[] m_LargeOffsets = new Point2D[]{ new Point2D( 1, 0 ), new Point2D( 0, 1 ), new Point2D( 1, 1 ) };

		// Large-tier bodies whose art still fits a single base tile.
		private static readonly int[] m_SingleBaseBodies = new int[]
		{
			2, 10, 18, 38, 40, 43, 75, 76, 102, 174, 195, 257, 285, 310, 475, 601, 768
		};

		public static bool IsLargeBody( Body body )
		{
			return BaseRace.GetMonsterSizeTier( body.BodyID ) >= 2 && Array.IndexOf( m_SingleBaseBodies, body.BodyID ) < 0;
		}

		public static List<Point3D> Footprint( Point3D p, bool large )
		{
			List<Point3D> tiles = new List<Point3D>();
			tiles.Add( p );

			if ( large )
			{
				foreach ( Point2D offset in m_LargeOffsets )
					tiles.Add( new Point3D( p.X + offset.X, p.Y + offset.Y, p.Z ) );
			}

			return tiles;
		}

		public bool FootprintFits( Point3D p, BaseHouse house )
		{
			List<Point3D> current = Footprint( Location, m_Pieces.Count > 0 );

			foreach ( Point3D tile in Footprint( p, m_Pieces.Count > 0 ) )
			{
				if ( !house.IsInside( tile, 20 ) )
					return false;

				// Tiles the base already covers are occupied by its own pieces and statue.
				if ( !current.Contains( tile ) && !Map.CanFit( tile.X, tile.Y, tile.Z, 20, false, true, true ) )
					return false;
			}

			return true;
		}

		public void UpdateFootprint()
		{
			bool large = m_Statue != null && IsLargeBody( m_Statue.Body );

			if ( Map == null || Map == Map.Internal )
				return;

			if ( large && m_Pieces.Count == 0 )
			{
				foreach ( Point2D offset in m_LargeOffsets )
				{
					CharacterStatuePlinthPiece piece = new CharacterStatuePlinthPiece( this );
					piece.Hue = Hue;
					piece.MoveToWorld( new Point3D( X + offset.X, Y + offset.Y, Z ), Map );
					m_Pieces.Add( piece );
				}
			}
			else if ( !large && m_Pieces.Count > 0 )
			{
				DeletePieces();
			}

			if ( m_Statue != null )
				m_Statue.Location = StatueLocation;
		}

		// Bodies whose art reads better standing on the front (south) tile of a 2x2 base than on the back one.
		private static readonly int[] m_FrontStandingBodies = new int[]
		{
			9, 88, 89, 137, 138, 144, 146, 172, 189, 191, 259, 264, 303, 309, 311, 312, 313, 314, 316, 320, 325, 427, 428, 433, 436, 485,
			509, 670, 725, 726, 729, 730, 732, 748, 758, 764, 765, 766, 770, 771, 772, 773, 774, 777, 792, 999
		};

		public Point2D StatueOffset
		{
			get
			{
				if ( m_Pieces.Count > 0 && m_Statue != null && Array.IndexOf( m_FrontStandingBodies, m_Statue.Body.BodyID ) >= 0 )
					return new Point2D( 1, 1 );

				return new Point2D( 0, 0 );
			}
		}

		public Point3D StatueLocation
		{
			get { return new Point3D( X + StatueOffset.X, Y + StatueOffset.Y, Z + 5 ); }
		}

		private void DeletePieces()
		{
			foreach ( Item piece in m_Pieces )
			{
				if ( !piece.Deleted )
					piece.Delete();
			}

			m_Pieces.Clear();
		}

		public void ReleaseStatue()
		{
			m_Statue = null;
		}

		public CharacterStatuePlinth( CharacterStatue statue ) : base( 0x32F2 )
		{
			m_Statue = statue;

			InvalidateHue();
		}

		public CharacterStatuePlinth( Serial serial ) : base( serial )
		{
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			DeletePieces();

			if ( m_Statue != null && !m_Statue.Deleted )
				m_Statue.Delete();
		}

		public override void OnMapChange()
		{
			if ( m_Statue != null )
				m_Statue.Map = Map;

			foreach ( Item piece in m_Pieces )
				piece.Map = Map;
		}

		public override void OnLocationChange( Point3D oldLocation )
		{
			if ( m_Statue != null )
				m_Statue.Location = StatueLocation;

			for ( int i = 0; i < m_Pieces.Count; i++ )
				m_Pieces[i].Location = new Point3D( X + m_LargeOffsets[i].X, Y + m_LargeOffsets[i].Y, Z );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_Statue != null )
				from.SendGump( new CharacterPlinthGump( m_Statue ) );			
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 1 ); // version

			writer.Write( (Mobile) m_Statue );
			writer.Write( m_Pieces );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_Statue = reader.ReadMobile() as CharacterStatue;

			if ( version >= 1 )
				m_Pieces = reader.ReadStrongItemList();

			if( m_Statue == null || m_Statue.SculptedBy == null || Map == Map.Internal )
			{
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( Delete ) );
			}
		}

		public void InvalidateHue()
		{
			if ( m_Statue != null )
				Hue = 0xB8F + (int) m_Statue.StatueType * 4 + (int) m_Statue.Material;

			foreach ( Item piece in m_Pieces )
				piece.Hue = Hue;
		}

		public virtual bool CouldFit( IPoint3D p, Map map )
		{
			if ( map == null )
				return false;

			Point3D point = new Point3D( p.X, p.Y, p.Z );
			BaseHouse house = BaseHouse.FindHouseAt( point, map, 20 );

			if ( house == null )
				return false;

			foreach ( Point3D tile in Footprint( point, m_Pieces.Count > 0 ) )
			{
				if ( !map.CanFit( tile, 20 ) || CharacterStatueTarget.CheckDoors( tile, 20, house ) != AddonFitResult.Valid )
					return false;
			}

			return true;
		}

		private class CharacterPlinthGump : Gump
		{
			public CharacterPlinthGump( CharacterStatue statue ) : base( 60, 30 )
			{
				Closable = true;
				Disposable = true;
				Dragable = true;
				Resizable = false;
			
				AddPage( 0 );
				AddImage( 0, 0, 0x24F4 );
				AddHtml( 55, 50, 150, 20, statue.Name, false, false );
				AddHtml( 55, 75, 150, 20, statue.SculptedOn.ToString( "G", new CultureInfo("de-DE") ), false, false );
				AddHtmlLocalized( 55, 100, 150, 20, GetTypeNumber( statue.StatueType ), 0, false, false );
			}

			public int GetTypeNumber( StatueType type )
			{
				switch ( type )
				{
					case StatueType.Marble: return 1076181;
					case StatueType.Jade: return 1076180;
					case StatueType.Bronze: return 1076230;
					default: return 1076181;
				}
			}
		}
	}

	public class CharacterStatuePlinthPiece : Static
	{
		public override int LabelNumber{ get{ return 1076201; } } // Character Statue

		private CharacterStatuePlinth m_Plinth;

		public CharacterStatuePlinth Plinth{ get{ return m_Plinth; } }

		public CharacterStatuePlinthPiece( CharacterStatuePlinth plinth ) : base( 0x32F2 )
		{
			m_Plinth = plinth;
		}

		public CharacterStatuePlinthPiece( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_Plinth != null && !m_Plinth.Deleted )
				m_Plinth.OnDoubleClick( from );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.Write( (Item) m_Plinth );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_Plinth = reader.ReadItem() as CharacterStatuePlinth;

			if ( m_Plinth == null )
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( Delete ) );
		}
	}
}