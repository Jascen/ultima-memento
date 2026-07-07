using System;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Regions;

namespace Server.Items
{
	[FlipableAttribute( 0x1f14, 0x1f15, 0x1f16, 0x1f17 )]
	public class RecallRune : Item
	{
		public override string DefaultDescription{ get{ return "You can use these strange stones, to magically mark a location. A magical ability is required to mark the rune. Once marked, you can use gating or recall magic to select the stone and teleport to that location. You can also drop these runes on a runebook, which allows for easier organization if you have many runes."; } }

		private string m_Description;
		private bool m_Marked;
		private Point3D m_Target;
		private Map m_TargetMap;
		private BaseHouse m_House;

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			if ( m_House != null && !m_House.Deleted )
			{
				writer.Write( (int) 1 ); // version

				writer.Write( (Item) m_House );
			}
			else
			{
				writer.Write( (int) 0 ); // version
			}

			writer.Write( (string) m_Description );
			writer.Write( (bool) m_Marked );
			writer.Write( (Point3D) m_Target );
			writer.Write( (Map) NormalizeTargetMap( m_TargetMap ) );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_House = reader.ReadItem() as BaseHouse;
					goto case 0;
				}
				case 0:
				{
					m_Description = reader.ReadString();
					m_Marked = reader.ReadBool();
					m_Target = reader.ReadPoint3D();
					m_TargetMap = NormalizeTargetMap( reader.ReadMap() );

					CalculateHue();

					break;
				}
			}
		}

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public BaseHouse House
		{
			get
			{
				if ( m_House != null && m_House.Deleted )
					House = null;

				return m_House;
			}
			set{ m_House = value; CalculateHue(); InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
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

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public bool Marked
		{
			get
			{
				return m_Marked;
			}
			set
			{
				if ( m_Marked != value )
				{
					m_Marked = value;
					CalculateHue();
					InvalidateProperties();
				}
			}
		}

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public Point3D Target
		{
			get
			{
				return m_Target;
			}
			set
			{
				m_Target = value;
			}
		}

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public Map TargetMap
		{
			get
			{
				m_TargetMap = NormalizeTargetMap( m_TargetMap );
				return m_TargetMap;
			}
			set
			{
				value = NormalizeTargetMap( value );

				if ( !Object.ReferenceEquals( m_TargetMap, value ) )
				{
					m_TargetMap = value;
					CalculateHue();
					InvalidateProperties();
				}
			}
		}

		private static Map NormalizeTargetMap( Map map )
		{
			return Server.Engines.Instancing.DungeonInstanceType.NormalizeExternalMap( map );
		}

		private void CalculateHue()
		{
			if ( !m_Marked )
				Hue = 0;
			else if ( m_TargetMap == Map.Sosaria )
				Hue = (House != null ? 0x47E : 0x967);
			else if ( m_TargetMap == Map.Lodor )
				Hue = (House != null ? 0x47E : 0x490);
			else if ( m_TargetMap == Map.Underworld )
				Hue = (House != null ? 0x47E : 0x48D);
			else if ( m_TargetMap == Map.SerpentIsland )
				Hue = (House != null ? 0x47E : 0x48E);
			else if ( m_TargetMap == Map.IslesDread )
				Hue = (House != null ? 0x47E : 0x489);
			else if ( m_TargetMap == Map.SavagedEmpire )
				Hue = (House != null ? 0x47E : 0x48F);
		}

		public void Mark( Mobile m )
		{
			if ( m == null )
				return;

			if ( Server.Engines.Instancing.InstanceManager.IsInstanceMap( m.Map ) )
			{
				m.SendMessage( "You cannot mark runes inside an instance." );
				return;
			}

			m_Marked = true;

			bool setDesc = false;
			if ( Core.AOS )
			{
				m_House = BaseHouse.FindHouseAt( m );

				if ( m_House == null )
				{
					m_Target = m.Location;
					m_TargetMap = NormalizeTargetMap( m.Map );
				}
				else
				{
					HouseSign sign = m_House.Sign;

					if ( sign != null )
						m_Description = sign.Name;
					else
						m_Description = null;

					if ( m_Description == null || (m_Description = m_Description.Trim()).Length == 0 )
						m_Description = "an unnamed house";

					setDesc = true;

					int x = m_House.BanLocation.X;
					int y = m_House.BanLocation.Y + 2;
					int z = m_House.BanLocation.Z;

					Map map = m_House.Map;

					if ( map != null && !map.CanFit( x, y, z, 16, false, false ) )
						z = map.GetAverageZ( x, y );

					m_Target = new Point3D( x, y, z );
					m_TargetMap = NormalizeTargetMap( map );
				}
			}
			else
			{
				m_House = null;
				m_Target = m.Location;
				m_TargetMap = NormalizeTargetMap( m.Map );
			}

			if( !setDesc )
				m_Description = BaseRegion.GetRuneNameFor( Region.Find( m_Target, m_TargetMap ) );

			CalculateHue();
			InvalidateProperties();
		}

		private const string RuneFormat = "a recall rune for {0}";

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Marked )
			{
				string desc;

				if ( (desc = m_Description) == null || (desc = desc.Trim()).Length == 0 )
					desc = "an unknown location";

				list.Add( ("{0}"), String.Format( RuneFormat, desc ) );
			}
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( m_Marked )
			{
				string desc;

				if ( (desc = m_Description) == null || (desc = desc.Trim()).Length == 0 )
					desc = "an unknown location";

				LabelTo( from, ("{0}"), String.Format( RuneFormat, desc ) );
			}
			else
			{
				LabelTo( from, "an unmarked recall rune" );
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			int number;

			if ( !IsChildOf( from.Backpack ) )
			{
				number = 1042001; // That must be in your pack for you to use it.
			}
			else if ( House != null )
			{
				number = 1062399; // You cannot edit the description for this rune.
			}
			else if ( m_Marked )
			{
				number = 501804; // Please enter a description for this marked object.

				from.Prompt = new RenamePrompt( this );
			}
			else
			{
				number = 501805; // That rune is not yet marked.
			}

			from.SendLocalizedMessage( number );
		}

		private class RenamePrompt : Prompt
		{
			private RecallRune m_Rune;

			public RenamePrompt( RecallRune rune )
			{
				m_Rune = rune;
			}

			public override void OnResponse( Mobile from, string text )
			{
				if ( m_Rune.House == null && m_Rune.Marked )
				{
					m_Rune.Description = text;
					from.SendLocalizedMessage( 1010474 ); // The etching on the rune has been changed.
				}
			}
		}

		[Constructable]
		public RecallRune() : base( 0x1F14 )
		{
			Weight = 1.0;
			CalculateHue();
		}

		public RecallRune( Serial serial ) : base( serial )
		{
		}
	}
}
