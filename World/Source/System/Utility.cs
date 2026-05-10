/***************************************************************************
 *                                Utility.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.Win32;
using Server.Network;

namespace Server
{
	public static class Utility
	{
		private static Random m_Random = new Random();
		private static Encoding m_UTF8, m_UTF8WithEncoding;

		public static Encoding UTF8
		{
			get
			{
				if ( m_UTF8 == null )
					m_UTF8 = new UTF8Encoding( false, false );

				return m_UTF8;
			}
		}

		public static Encoding UTF8WithEncoding
		{
			get
			{
				if ( m_UTF8WithEncoding == null )
					m_UTF8WithEncoding = new UTF8Encoding( true, false );

				return m_UTF8WithEncoding;
			}
		}

		public static void Separate( StringBuilder sb, string value, string separator )
		{
			if ( sb.Length > 0 )
				sb.Append( separator );

			sb.Append( value );
		}

		public static string Intern( string str )
		{
			if ( str == null )
				return null;
			else if ( str.Length == 0 )
				return String.Empty;

			return String.Intern( str );
		}

		public static void Intern( ref string str )
		{
			str = Intern( str );
		}

		private static Dictionary<IPAddress, IPAddress> _ipAddressTable;

		public static IPAddress Intern( IPAddress ipAddress ) {
			if ( _ipAddressTable == null ) {
				_ipAddressTable = new Dictionary<IPAddress, IPAddress>();
			}

			IPAddress interned;

			if ( !_ipAddressTable.TryGetValue( ipAddress, out interned ) ) {
				interned = ipAddress;
				_ipAddressTable[ipAddress] = interned;
			}

			return interned;
		}

		public static void Intern( ref IPAddress ipAddress ) {
			ipAddress = Intern( ipAddress );
		}

		public static bool IsValidIP( string text )
		{
			bool valid = true;

			IPMatch( text, IPAddress.None, ref valid );

			return valid;
		}

		public static bool IPMatch( string val, IPAddress ip )
		{
			bool valid = true;

			return IPMatch( val, ip, ref valid );
		}

		public static string FixHtml( string str )
		{
			if( str == null )
				return "";

			bool hasOpen  = ( str.IndexOf( '<' ) >= 0 );
			bool hasClose = ( str.IndexOf( '>' ) >= 0 );
			bool hasPound = ( str.IndexOf( '#' ) >= 0 );

			if ( !hasOpen && !hasClose && !hasPound )
				return str;

			StringBuilder sb = new StringBuilder( str );

			if ( hasOpen )
				sb.Replace( '<', '(' );

			if ( hasClose )
				sb.Replace( '>', ')' );

			if ( hasPound )
				sb.Replace( '#', '-' );

			return sb.ToString();
		}

		public static bool IPMatchCIDR( string cidr, IPAddress ip )
		{
			if ( ip == null || ip.AddressFamily == AddressFamily.InterNetworkV6 )
				return false;	//Just worry about IPv4 for now


			/*
			string[] str = cidr.Split( '/' );

			if ( str.Length != 2 )
				return false;

			/* **************************************************
			IPAddress cidrPrefix;

			if ( !IPAddress.TryParse( str[0], out cidrPrefix ) )
				return false;
			 * */

			/*
			string[] dotSplit = str[0].Split( '.' );

			if ( dotSplit.Length != 4 )		//At this point and time, and for speed sake, we'll only worry about IPv4
				return false;

			byte[] bytes = new byte[4];

			for ( int i = 0; i < 4; i++ )
			{
				byte.TryParse( dotSplit[i], out bytes[i] );
			}

			uint cidrPrefix = OrderedAddressValue( bytes );

			int cidrLength = Utility.ToInt32( str[1] );
			//The below solution is the fastest solution of the three

			*/

			byte[] bytes = new byte[4];
			string[] split = cidr.Split( '.' );
			bool cidrBits = false;
			int cidrLength = 0;

			for ( int i = 0; i < 4; i++ )
			{
				int part = 0;

				int partBase = 10;

				string pattern = split[i];

				for ( int j = 0; j < pattern.Length; j++ )
				{
					char c = (char)pattern[j];


					if ( c == 'x' || c == 'X' )
					{
						partBase = 16;
					}
					else if ( c >= '0' && c <= '9' )
					{
						int offset = c - '0';

						if ( cidrBits )
						{
							cidrLength *= partBase;
							cidrLength += offset;
						}
						else
						{
							part *= partBase;
							part += offset;
						}
					}
					else if ( c >= 'a' && c <= 'f' )
					{
						int offset = 10 + ( c - 'a' );

						if ( cidrBits )
						{
							cidrLength *= partBase;
							cidrLength += offset;
						}
						else
						{
							part *= partBase;
							part += offset;
						}
					}
					else if ( c >= 'A' && c <= 'F' )
					{
						int offset = 10 + ( c - 'A' );

						if ( cidrBits )
						{
							cidrLength *= partBase;
							cidrLength += offset;
						}
						else
						{
							part *= partBase;
							part += offset;
						}
					}
					else if ( c == '/' )
					{
						if ( cidrBits || i != 3 )	//If there's two '/' or the '/' isn't in the last byte
						{
							return false;
						}

						partBase = 10;
						cidrBits = true;
					}
					else
					{
						return false;
					}
				}

				bytes[i] = (byte)part;
			}

			uint cidrPrefix = OrderedAddressValue( bytes );

			return IPMatchCIDR( cidrPrefix, ip, cidrLength );
		}

		public static bool IPMatchCIDR( IPAddress cidrPrefix, IPAddress ip, int cidrLength )
		{
			if ( cidrPrefix == null || ip == null || cidrPrefix.AddressFamily == AddressFamily.InterNetworkV6 )	//Ignore IPv6 for now
				return false;

			uint cidrValue = SwapUnsignedInt( (uint)GetLongAddressValue( cidrPrefix ) );
			uint ipValue   = SwapUnsignedInt( (uint)GetLongAddressValue( ip ) );

			return IPMatchCIDR( cidrValue, ipValue, cidrLength );
		}

		public static bool IPMatchCIDR( uint cidrPrefixValue, IPAddress ip, int cidrLength )
		{
			if ( ip == null || ip.AddressFamily == AddressFamily.InterNetworkV6)
				return false;

			uint ipValue = SwapUnsignedInt( (uint)GetLongAddressValue( ip ) );

			return IPMatchCIDR( cidrPrefixValue, ipValue, cidrLength );
		}

		public static bool IPMatchCIDR( uint cidrPrefixValue, uint ipValue, int cidrLength )
		{
			if ( cidrLength <= 0 || cidrLength >= 32 )   //if invalid cidr Length, just compare IPs
				return cidrPrefixValue == ipValue;

			uint mask = uint.MaxValue << 32-cidrLength;

			return ( ( cidrPrefixValue & mask ) == ( ipValue & mask ) );
		}

		private static uint OrderedAddressValue( byte[] bytes )
		{
			if ( bytes.Length != 4 )
				return 0;

			return (uint)(((( bytes[0] << 0x18 ) | (bytes[1] << 0x10)) | (bytes[2] << 8)) | bytes[3]) & ((uint)0xffffffff);
		}

		private static uint SwapUnsignedInt( uint source )
		{
			return (uint)( ( ( ( source & 0x000000FF ) << 0x18 )
			| ( ( source & 0x0000FF00 ) << 8 )
			| ( ( source & 0x00FF0000 ) >> 8 )
			| ( ( source & 0xFF000000 ) >> 0x18 ) ) );
		} 

		public static bool TryConvertIPv6toIPv4( ref IPAddress address )
		{
			if ( !Socket.OSSupportsIPv6 || address.AddressFamily == AddressFamily.InterNetwork )
				return true;

			byte[] addr = address.GetAddressBytes();
			if ( addr.Length == 16 )	//sanity 0 - 15 //10 11 //12 13 14 15
			{
				if ( addr[10] != 0xFF || addr[11] != 0xFF )
					return false;

				for ( int i = 0; i < 10; i++ )
				{
					if ( addr[i] != 0 )
						return false;
				}

				byte[] v4Addr = new byte[4];

				for ( int i = 0; i < 4; i++ )
				{
					v4Addr[i] = addr[12 + i];
				}

				address = new IPAddress( v4Addr );
				return true;
			}

			return false;
		}

		public static bool IPMatch( string val, IPAddress ip, ref bool valid )
		{
			valid = true;

			string[] split = val.Split( '.' );

			for ( int i = 0; i < 4; ++i )
			{
				int lowPart, highPart;

				if ( i >= split.Length )
				{
					lowPart = 0;
					highPart = 255;
				}
				else
				{
					string pattern = split[i];

					if ( pattern == "*" )
					{
						lowPart = 0;
						highPart = 255;
					}
					else
					{
						lowPart = 0;
						highPart = 0;

						bool highOnly = false;
						int lowBase = 10;
						int highBase = 10;

						for ( int j = 0; j < pattern.Length; ++j )
						{
							char c = (char)pattern[j];

							if ( c == '?' )
							{
								if ( !highOnly )
								{
									lowPart *= lowBase;
									lowPart += 0;
								}

								highPart *= highBase;
								highPart += highBase - 1;
							}
							else if ( c == '-' )
							{
								highOnly = true;
								highPart = 0;
							}
							else if ( c == 'x' || c == 'X' )
							{
								lowBase = 16;
								highBase = 16;
							}
							else if ( c >= '0' && c <= '9' )
							{
								int offset = c - '0';

								if ( !highOnly )
								{
									lowPart *= lowBase;
									lowPart += offset;
								}

								highPart *= highBase;
								highPart += offset;
							}
							else if ( c >= 'a' && c <= 'f' )
							{
								int offset = 10 + (c - 'a');

								if ( !highOnly )
								{
									lowPart *= lowBase;
									lowPart += offset;
								}

								highPart *= highBase;
								highPart += offset;
							}
							else if ( c >= 'A' && c <= 'F' )
							{
								int offset = 10 + (c - 'A');

								if ( !highOnly )
								{
									lowPart *= lowBase;
									lowPart += offset;
								}

								highPart *= highBase;
								highPart += offset;
							}
							else
							{
								valid = false;	//high & lowpart would be 0 if it got to here.
							}
						}
					}
				}

				int b = (byte)(Utility.GetAddressValue( ip ) >> (i * 8));

				if ( b < lowPart || b > highPart )
					return false;
			}

			return true;
		}

		public static bool IPMatchClassC( IPAddress ip1, IPAddress ip2 )
		{
			return ( (Utility.GetAddressValue( ip1 ) & 0xFFFFFF) == (Utility.GetAddressValue( ip2 ) & 0xFFFFFF) );
		}

		public static int InsensitiveCompare( string first, string second )
		{
			return Insensitive.Compare( first, second );
		}

		public static bool InsensitiveStartsWith( string first, string second )
		{
			return Insensitive.StartsWith( first, second );
		}

		#region To[Something]
		public static bool ToBoolean( string value )
		{
			bool b;
			bool.TryParse( value, out b );

			return b;
		}

		public static double ToDouble( string value )
		{
			double d;
			double.TryParse( value, out d );

			return d;
		}

		public static TimeSpan ToTimeSpan( string value )
		{
			TimeSpan t;
			TimeSpan.TryParse( value, out t );

			return t;
		}

		public static int ToInt32( string value )
		{
			int i;

			if( value.StartsWith( "0x" ) )
				int.TryParse( value.Substring( 2 ), NumberStyles.HexNumber, null, out i );
			else
				int.TryParse( value, out i );

			return i;
		}
		#endregion

		#region Get[Something]
		public static int GetXMLInt32( string intString, int defaultValue )
		{
			try
			{
				return XmlConvert.ToInt32( intString );
			}
			catch
			{
				int val;
				if ( int.TryParse( intString, out val ) )
					return val;

				return defaultValue;
			}
		}

		public static DateTime GetXMLDateTime( string dateTimeString, DateTime defaultValue )
		{
			try
			{
				return XmlConvert.ToDateTime( dateTimeString, XmlDateTimeSerializationMode.Local );
			}
			catch
			{
				DateTime d;

				if( DateTime.TryParse( dateTimeString, out d ) )
					return d;

				return defaultValue;
			}
		}

		public static TimeSpan GetXMLTimeSpan( string timeSpanString, TimeSpan defaultValue )
		{
			try
			{
				return XmlConvert.ToTimeSpan( timeSpanString );
			}
			catch
			{
				return defaultValue;
			}
		}

		public static string GetAttribute( XmlElement node, string attributeName )
		{
			return GetAttribute( node, attributeName, null );
		}

		public static string GetAttribute( XmlElement node, string attributeName, string defaultValue )
		{
			if ( node == null )
				return defaultValue;

			XmlAttribute attr = node.Attributes[attributeName];

			if ( attr == null )
				return defaultValue;

			return attr.Value;
		}

		public static string GetText( XmlElement node, string defaultValue )
		{
			if ( node == null )
				return defaultValue;

			return node.InnerText;
		}

		public static int GetAddressValue( IPAddress address )
		{
#pragma warning disable 618
			return (int)address.Address;
#pragma warning restore 618
		}

		public static long GetLongAddressValue( IPAddress address )
		{
#pragma warning disable 618
			return address.Address;
#pragma warning restore 618
		}
		#endregion

		public static double RandomDouble()
		{
			return m_Random.NextDouble();
		}
		#region In[...]Range
		public static bool InRange( Point3D p1, Point3D p2, int range )
		{
			return ( p1.m_X >= (p2.m_X - range) )
				&& ( p1.m_X <= (p2.m_X + range) )
				&& ( p1.m_Y >= (p2.m_Y - range) )
				&& ( p1.m_Y <= (p2.m_Y + range) );
		}

		public static bool InUpdateRange( Point3D p1, Point3D p2 )
		{
			return ( p1.m_X >= (p2.m_X - 18) )
				&& ( p1.m_X <= (p2.m_X + 18) )
				&& ( p1.m_Y >= (p2.m_Y - 18) )
				&& ( p1.m_Y <= (p2.m_Y + 18) );
		}

		public static bool InUpdateRange( Point2D p1, Point2D p2 )
		{
			return ( p1.m_X >= (p2.m_X - 18) )
				&& ( p1.m_X <= (p2.m_X + 18) )
				&& ( p1.m_Y >= (p2.m_Y - 18) )
				&& ( p1.m_Y <= (p2.m_Y + 18) );
		}

		public static bool InUpdateRange( IPoint2D p1, IPoint2D p2 )
		{
			return ( p1.X >= (p2.X - 18) )
				&& ( p1.X <= (p2.X + 18) )
				&& ( p1.Y >= (p2.Y - 18) )
				&& ( p1.Y <= (p2.Y + 18) );
		}

		#endregion
		public static Direction GetDirection( IPoint2D from, IPoint2D to )
		{
			int dx = to.X - from.X;
			int dy = to.Y - from.Y;

			int adx = Math.Abs( dx );
			int ady = Math.Abs( dy );

			if ( adx >= ady * 3 )
			{
				if ( dx > 0 )
					return Direction.East;
				else
					return Direction.West;
			}
			else if ( ady >= adx * 3 )
			{
				if ( dy > 0 )
					return Direction.South;
				else
					return Direction.North;
			}
			else if ( dx > 0 )
			{
				if ( dy > 0 )
					return Direction.Down;
				else
					return Direction.Right;
			}
			else
			{
				if ( dy > 0 )
					return Direction.Left;
				else
					return Direction.Up;
			}
		}

		/* Should probably be rewritten to use an ITile interface

		public static bool CanMobileFit( int z, StaticTile[] tiles )
		{
			int checkHeight = 15;
			int checkZ = z;

			for ( int i = 0; i < tiles.Length; ++i )
			{
				StaticTile tile = tiles[i];

				if ( ((checkZ + checkHeight) > tile.Z && checkZ < (tile.Z + tile.Height))*//* || (tile.Z < (checkZ + checkHeight) && (tile.Z + tile.Height) > checkZ)*//* )
				{
					return false;
				}
				else if ( checkHeight == 0 && tile.Height == 0 && checkZ == tile.Z )
				{
					return false;
				}
			}

			return true;
		}

		public static bool IsInContact( StaticTile check, StaticTile[] tiles )
		{
			int checkHeight = check.Height;
			int checkZ = check.Z;

			for ( int i = 0; i < tiles.Length; ++i )
			{
				StaticTile tile = tiles[i];

				if ( ((checkZ + checkHeight) > tile.Z && checkZ < (tile.Z + tile.Height))*//* || (tile.Z < (checkZ + checkHeight) && (tile.Z + tile.Height) > checkZ)*//* )
				{
					return true;
				}
				else if ( checkHeight == 0 && tile.Height == 0 && checkZ == tile.Z )
				{
					return true;
				}
			}

			return false;
		}
		*/

		public static object GetArrayCap( Array array, int index )
		{
			return GetArrayCap( array, index, null );
		}

		public static object GetArrayCap( Array array, int index, object emptyValue )
		{
			if ( array.Length > 0 )
			{
				if ( index < 0 )
				{
					index = 0;
				}
				else if ( index >= array.Length )
				{
					index = array.Length - 1;
				}

				return array.GetValue( index );
			}
			else
			{
				return emptyValue;
			}
		}

		//4d6+8 would be: Utility.Dice( 4, 6, 8 )
		public static int Dice( int numDice, int numSides, int bonus )
		{
			int total = 0;
			for (int i=0;i<numDice;++i)
				total += Random( numSides ) + 1;
			total += bonus;
			return total;
		}

		public static int RandomList( params int[] list )
		{
			return list[m_Random.Next( list.Length )];
		}

		public static bool RandomBool()
		{
			return ( m_Random.Next( 2 ) == 0 );
		}

		public static int RandomMinMax( int min, int max )
		{
			if ( min > max )
			{
				int copy = min;
				min = max;
				max = copy;
			}
			else if ( min == max )
			{
				return min;
			}

			return min + m_Random.Next( (max - min) + 1 );
		}

		public static T Random<T>( List<T> list )
		{
			return list[m_Random.Next( list.Count )];
		}

		public static T Random<T>( T[] array )
		{
			return array[m_Random.Next( array.Length )];
		}

		public static int Random( int from, int count )
		{
			if ( count == 0 )
			{
				return from;
			}
			else if ( count > 0 )
			{
				return from + m_Random.Next( count );
			}
			else
			{
				return from - m_Random.Next( -count );
			}
		}

		public static int Random( int count )
		{
			return m_Random.Next( count );
		}

		#region Random Hues

		public static int RandomNondyedHue()
		{
			switch ( Random( 6 ) )
			{
				case 0: return RandomPinkHue();
				case 1: return RandomBlueHue();
				case 2: return RandomGreenHue();
				case 3: return RandomOrangeHue();
				case 4: return RandomRedHue();
				case 5: return RandomYellowHue();
			}

			return 0;
		}

		public static int RandomPinkHue()
		{
			return Random( 1201, 54 );
		}

		public static int RandomBlueHue()
		{
			return Random( 1301, 54 );
		}

		public static int RandomGreenHue()
		{
			return Random( 1401, 54 );
		}

		public static int RandomOrangeHue()
		{
			return Random( 1501, 54 );
		}

		public static int RandomRedHue()
		{
			return Random( 1601, 54 );
		}

		public static int RandomYellowHue()
		{
			return Random( 1701, 54 );
		}

		public static int RandomNeutralHue()
		{
			return Random( 1801, 108 );
		}

		public static int RandomSnakeHue()
		{
			return RandomList( 2001, 2002, 2003, 2004, 2005, 2006, 2007, 2008, 2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018, 0x8AB, 0x8AC, 0x8AD, 0x8AE, 0x8AF, 0x8B0 );
		}

		public static int RandomBirdHue()
		{
			return Random( 2101, 30 );
		}

		public static int RandomSlimeHue()
		{
			return Random( 2201, 24 );
		}

		public static int RandomAnimalHue()
		{
			return Random( 2301, 18 );
		}

		public static int RandomMetalHue()
		{
			return Random( 2401, 30 );
		}

		public static int ClipDyedHue( int hue )
		{
			if ( hue < 2 )
				return 2;
			else if ( hue > 1001 )
				return 1001;
			else
				return hue;
		}

		public static int RandomDyedHue()
		{
			return Random( 2, 1000 );
		}

		//[Obsolete( "Depreciated, use the methods for the Mobile's race", false )]
		public static int ClipSkinHue( int hue )
		{
			if ( hue < 1002 )
				return 1002;
			else if ( hue > 1058 )
				return 1058;
			else
				return hue;
		}

		//[Obsolete( "Depreciated, use the methods for the Mobile's race", false )]
		public static int RandomSkinHue()
		{
			return Random( 1002, 57 ) | 0x8000;
		}

		public static int RandomTalkHue()
		{
			return Utility.RandomList( 0xB93, 0xB78, 0x845, 0x847, 0x84D, 0x84E, 0x560, 0x55C, 0x556, 0x54E, 0x550, 0x21, 0xB64, 0xB61, 0xAFE, 0x993, 0x999, 0xABC );
		}

		public static int RandomMonsterHue()
		{
			return Utility.RandomList( 0, 0x8E4, 0xB2A, 0x916, 0xB51, 0x82B, 0x8D8, 0x921, 0x77C, 0x871, 0x996, 0xB56, 0x95B, 0x796, 0xB65, 0xB05, 0xB3B, 0x99F, 0x98B, 0xB7C, 0x6F7, 0x7C3, 0x7C6, 0x92B, 0x943, 0x8D0, 0x8B6, 0xB7E, 0xB1B, 0x829, 0xB94, 0x77E, 0x88B, 0x994, 0x6F5, 0x869, 0xB02, 0x93E, 0x7CA, 0x94D, 0x883, 0x95D, 0x7CB, 0x95E, 0xB5A, 0x957, 0x7C7, 0x7CE, 0x944, 0x8DD, 0x8E3, 0x942, 0x943, 0x950, 0x702, 0xB3B, 0x708, 0x77A, 0xB5E, 0x95B, 0x6FB, 0x870, 0xA9F, 0xBB0, 0x877, 0x87E, 0x705, 0x8B8, 0x6FD, 0x86B, 0x95C, 0x7CC, 0x6FE, 0x6F9, 0x776, 0x86C, 0x701, 0xB12, 0xB38, 0xB13, 0x827, 0xAB3, 0xAFA, 0x93D, 0xB54, 0x775, 0x779, 0xB09, 0x85D, 0x6F6, 0xB28, 0xB00, 0xACC, 0x856, 0x91E, 0x883, 0xB44, 0x706, 0xAF7, 0x86A, 0xB01, 0x6FC, 0x95E, 0x703, 0x981, 0x6F8, 0x869, 0x95D, 0xB9D, 0xB31, 0x99B, 0xB32, 0x855, 0x959, 0x952, 0x797, 0x957, 0x713, 0x8BC, 0x712, 0x945, 0x8D1, 0x8C2, 0xB07, 0x707, 0xB3D, 0x7CD, 0xAE9, 0x854, 0xB7F, 0xAFF, 0x860, 0xAF3, 0xB7A, 0x9C4, 0x86D, 0x87D, 0x8BA, 0x911, 0xAB1, 0x88D, 0x945, 0x8B2, 0xB27, 0x77D, 0x8EC, 0x7C7 );
		}

		public static int RandomEvilHue()
		{
			return Utility.RandomList( 2074, 1967, 1180, 1181, 0xB85, 0x846, 0x5B5, 0x497, 0x485, 0x47E, 0x481, 0x430, 0x961, 0x962, 0x963, 0x964, 0x965, 0x966, 0x967, 0x968, 0x969, 0x96A, 0x96B, 0x96C, 0x6DB, 0x6DC, 0x6DD, 0x6DE, 0x6DF );
		}

		public static int RandomDrowHue()
		{
			return Utility.RandomList( 1476, 1479, 2342, 1967, 2346, 1957, 2074, 2455, 2944, 2817, 2915, 2906, 1788, 1790, 2599, 2615, 2087, 1779, 2085, 2092, 2089, 1183, 2380, 2379, 1484, 2898, 1489, 1995, 2167, 1470, 1467, 2807, 1939, 1465, 2174, 2801, 1156, 2230, 2227, 2255, 1462, 1141, 1157, 1158, 1160, 1175, 1254, 1310, 1652, 2118, 2122, 2124, 2224, 1105 );
		}

		public static int RandomOrkHue()
		{
			return Utility.RandomList( 1479, 2664, 1957, 1557, 2752, 2944, 1918, 2765, 2915, 1782, 2767, 2897, 2096, 2091, 2099, 2089, 2796, 1182, 2653, 2398, 2828, 2845, 2167, 1470, 2804, 2226, 1494, 2246, 1381, 1155, 1160, 1193, 1196, 1425, 1501, 1509, 2125, 2422, 1105, 1128, 2128 );
		}

		public static int RandomWizardHue()
		{
			return Utility.RandomList( 2916, 2251, 2664, 1956, 1574, 2074, 2452, 1918, 2875, 1788, 2241, 2263, 2615, 2635, 2897, 2843, 2087, 1909, 2091, 2085, 2092, 2338, 1183, 2380, 1989, 1990, 2653, 2828, 2827, 2826, 1995, 2928, 2227, 1495, 1496, 1382, 1494, 2246, 1391, 2877, 1384, 2173, 2903, 2808, 1196, 1646, 2118, 2122, 2124, 2125, 2422, 1105 );
		}

		public static int RandomSpecialHue()
		{
			return Utility.RandomList( 1105, 1128, 1141, 1155, 1156, 1157, 1158, 1160, 1175, 1180, 1181, 1182, 1183, 1193, 1196, 1254, 1310, 1381, 1382, 1384, 1425, 1462, 1465, 1467, 1470, 1476, 1479, 1483, 1484, 1489, 1494, 1495, 1496, 1501, 1509, 1557, 1571, 1574, 1646, 1652, 1779, 1782, 1788, 1790, 1909, 1918, 1939, 1952, 1956, 1957, 1966, 1967, 1989, 1990, 1995, 2056, 2074, 2085, 2087, 2089, 2091, 2092, 2096, 2099, 2118, 2122, 2124, 2125, 2128, 2167, 2173, 2174, 2224, 2226, 2227, 2230, 2241, 2246, 2251, 2255, 2263, 2284, 2338, 2342, 2346, 2379, 2380, 2398, 2422, 2452, 2455, 2599, 2615, 2635, 2641, 2653, 2664, 2752, 2765, 2767, 2796, 2801, 2804, 2807, 2808, 2817, 2826, 2827, 2828, 2843, 2845, 2875, 2877, 2897, 2898, 2903, 2906, 2915, 2916, 2928, 2944 );
		}

		public static int RandomSkinColor()
		{
			return Utility.RandomMinMax( 1002, 1058 );
		}

		public static int RandomHairColor()
		{
			return Utility.RandomMinMax( 0x44E, 0x47D );
		}

		public static int RandomMsgColor()
		{
			return Utility.RandomList( 68,39,19,54,89,144,1153 );
		}

		public static int DateUpdated()
		{
			return 20240922;
		}

		public static bool ClothingMod()
		{
			return true;
		}

		public static int RandomColor( int color )
		{
			int Hue = 0;

			if ( color == 0 ){ color = Utility.RandomMinMax( 0, 25 ); Hue = Utility.RandomSpecialHue(); }

			switch( color )
			{
				case 0: Hue = Utility.RandomNeutralHue(); break;
				case 1: Hue = Utility.RandomRedHue(); break;
				case 2: Hue = Utility.RandomBlueHue(); break;
				case 3: Hue = Utility.RandomGreenHue(); break;
				case 4: Hue = Utility.RandomYellowHue(); break;
				case 5: Hue = Utility.RandomSnakeHue(); break;
				case 6: Hue = Utility.RandomMetalHue(); break;
				case 7: Hue = Utility.RandomAnimalHue(); break;
				case 8: Hue = Utility.RandomSlimeHue(); break;
				case 9: Hue = Utility.RandomOrangeHue(); break;
				case 10: Hue = Utility.RandomPinkHue(); break;
				case 11: Hue = Utility.RandomDyedHue(); break;
				case 12: Hue = Utility.RandomBirdHue(); break;
				case 13: Hue = Utility.RandomEvilHue(); break;
				case 14: Hue = Utility.RandomSpecialHue(); break;
				case 15: Hue = 0; break;
			}
			return Hue;
		}

		public static string AddSpaces(string text)
		{
			StringBuilder newText = new StringBuilder(text.Length * 2);
			newText.Append(text[0]);
			for (int i = 1; i < text.Length; i++)
			{
				if (char.IsUpper(text[i]) && text[i - 1] != ' ')
					newText.Append(' ');
				newText.Append(text[i]);
			}
			return newText.ToString();
		}

		public static bool BlockedTile ( int id, string category )
		{
			if ( ( category == "water" || category == "any" ) && ( 
				id ==	0x00A8	||
				id ==	0x00A9	||
				id ==	0x00AA	||
				id ==	0x00AB	||
				id ==	0x0136	||
				id ==	0x0138
			)){ return true; }

			if ( ( category == "cave" || category == "any" ) && ( 
				id ==	0x024A	||
				id ==	0x024B	||
				id ==	0x024C	||
				id ==	0x024D	||
				id ==	0x024E	||
				id ==	0x024F	||
				id ==	0x0250	||
				id ==	0x0251	||
				id ==	0x0252	||
				id ==	0x0253	||
				id ==	0x0254	||
				id ==	0x0255	||
				id ==	0x0256	||
				id ==	0x0257	||
				id ==	0x0258	||
				id ==	0x0259	||
				id ==	0x025A	||
				id ==	0x025B	||
				id ==	0x025C	||
				id ==	0x025D	||
				id ==	0x025E	||
				id ==	0x025F	||
				id ==	0x0260	||
				id ==	0x0261	||
				id ==	0x0262	||
				id ==	0x0263	||
				id ==	0x0264	||
				id ==	0x0265	||
				id ==	0x0266	||
				id ==	0x0267	||
				id ==	0x0268	||
				id ==	0x0269	||
				id ==	0x026A	||
				id ==	0x026B	||
				id ==	0x026C	||
				id ==	0x026D	||
				id ==	0x02BC	||
				id ==	0x02BD	||
				id ==	0x02BE	||
				id ==	0x02BF	||
				id ==	0x02C0	||
				id ==	0x02C1	||
				id ==	0x02C2	||
				id ==	0x02C3	||
				id ==	0x02C4	||
				id ==	0x02C5	||
				id ==	0x02C6	||
				id ==	0x02C7	||
				id ==	0x02C8	||
				id ==	0x02C9	||
				id ==	0x02CA	||
				id ==	0x02CB
			)){ return true; }

			if ( ( category == "dirt" || category == "any" ) && ( 
				id ==	0x008D	||
				id ==	0x008E	||
				id ==	0x008F	||
				id ==	0x0090	||
				id ==	0x0091	||
				id ==	0x0092	||
				id ==	0x0093	||
				id ==	0x0094	||
				id ==	0x0095	||
				id ==	0x0096	||
				id ==	0x0097	||
				id ==	0x0098	||
				id ==	0x0099	||
				id ==	0x009A	||
				id ==	0x009B	||
				id ==	0x009C	||
				id ==	0x009D	||
				id ==	0x009E	||
				id ==	0x009F	||
				id ==	0x00A0	||
				id ==	0x00A1	||
				id ==	0x00A2	||
				id ==	0x00A3	||
				id ==	0x00A4	||
				id ==	0x00A5	||
				id ==	0x00A6	||
				id ==	0x00A7	||
				id ==	0x00DC	||
				id ==	0x00DD	||
				id ==	0x00DE	||
				id ==	0x00DF	||
				id ==	0x00E0	||
				id ==	0x00E1	||
				id ==	0x00E2	||
				id ==	0x00E3	||
				id ==	0x02D0	||
				id ==	0x02D1	||
				id ==	0x02D2	||
				id ==	0x02D3	||
				id ==	0x02D4	||
				id ==	0x02D5	||
				id ==	0x02D6	||
				id ==	0x02D7	||
				id ==	0x02E5	||
				id ==	0x02E6	||
				id ==	0x02E7	||
				id ==	0x02E8	||
				id ==	0x02E9	||
				id ==	0x02EA	||
				id ==	0x02EB	||
				id ==	0x02EC	||
				id ==	0x02ED	||
				id ==	0x02EE	||
				id ==	0x02EF	||
				id ==	0x02F0	||
				id ==	0x02F1	||
				id ==	0x02F2	||
				id ==	0x02F3	||
				id ==	0x02F4	||
				id ==	0x02F5	||
				id ==	0x02F6	||
				id ==	0x02F7	||
				id ==	0x02F8	||
				id ==	0x02F9	||
				id ==	0x02FA	||
				id ==	0x02FB	||
				id ==	0x02FC	||
				id ==	0x02FD	||
				id ==	0x02FE	||
				id ==	0x02FF	||
				id ==	0x0303	||
				id ==	0x0304	||
				id ==	0x0305	||
				id ==	0x0306	||
				id ==	0x0307	||
				id ==	0x0308	||
				id ==	0x0309	||
				id ==	0x030A	||
				id ==	0x030B	||
				id ==	0x030C	||
				id ==	0x030D	||
				id ==	0x030E	||
				id ==	0x030F	||
				id ==	0x0310	||
				id ==	0x0311	||
				id ==	0x0312	||
				id ==	0x0313	||
				id ==	0x0314	||
				id ==	0x0315	||
				id ==	0x0316	||
				id ==	0x0317	||
				id ==	0x0318	||
				id ==	0x0319	||
				id ==	0x031A	||
				id ==	0x031B	||
				id ==	0x031C	||
				id ==	0x031D	||
				id ==	0x031E	||
				id ==	0x031F	||
				id ==	0x06F4	||
				id ==	0x0777	||
				id ==	0x0778	||
				id ==	0x0779	||
				id ==	0x077A	||
				id ==	0x077B	||
				id ==	0x077C	||
				id ==	0x077D	||
				id ==	0x077E	||
				id ==	0x077F	||
				id ==	0x0780	||
				id ==	0x0781	||
				id ==	0x0782	||
				id ==	0x0783	||
				id ==	0x0784	||
				id ==	0x0785	||
				id ==	0x0786	||
				id ==	0x0787	||
				id ==	0x0788	||
				id ==	0x0789	||
				id ==	0x078A	||
				id ==	0x078B	||
				id ==	0x078C	||
				id ==	0x078D	||
				id ==	0x078E	||
				id ==	0x078F	||
				id ==	0x0790	||
				id ==	0x0791
			)){ return true; }

			if ( ( category == "forest" || category == "any" ) && ( 
				id ==	0x00ED	||
				id ==	0x00EE	||
				id ==	0x00EF	||
				id ==	0x3AF0	||
				id ==	0x3AF1	||
				id ==	0x3AF2	||
				id ==	0x3AF3	||
				id ==	0x3AF4	||
				id ==	0x3AF5	||
				id ==	0x3AF6	||
				id ==	0x3AF7	||
				id ==	0x3AF8
			)){ return true; }

			if ( ( category == "grass" || category == "any" ) && ( 
				id ==	0x0231	||
				id ==	0x0232	||
				id ==	0x0233	||
				id ==	0x0234	||
				id ==	0x0239	||
				id ==	0x023A	||
				id ==	0x023B	||
				id ==	0x023C	||
				id ==	0x023D	||
				id ==	0x023E	||
				id ==	0x023F	||
				id ==	0x0240	||
				id ==	0x0241	||
				id ==	0x06D2	||
				id ==	0x06D3	||
				id ==	0x06D4	||
				id ==	0x06D5	||
				id ==	0x06D6	||
				id ==	0x06D7	||
				id ==	0x06D8	||
				id ==	0x06D9
			)){ return true; }

			if ( ( category == "jungle" || category == "any" ) && ( 
				id ==	0x00EC	||
				id ==	0x00FC	||
				id ==	0x00FD	||
				id ==	0x00FE	||
				id ==	0x00FF	||
				id ==	0x072A
			)){ return true; }

			if ( ( category == "rock" || category == "any" ) && ( 
				id ==	0x00E4	||
				id ==	0x00E5	||
				id ==	0x00E6	||
				id ==	0x00E7	||
				id ==	0x00F4	||
				id ==	0x00F5	||
				id ==	0x00F6	||
				id ==	0x00F7	||
				id ==	0x0104	||
				id ==	0x0105	||
				id ==	0x0106	||
				id ==	0x0107	||
				id ==	0x0110	||
				id ==	0x0111	||
				id ==	0x0112	||
				id ==	0x0113	||
				id ==	0x0122	||
				id ==	0x0123	||
				id ==	0x0124	||
				id ==	0x0125	||
				id ==	0x01D3	||
				id ==	0x01D4	||
				id ==	0x01D5	||
				id ==	0x01D6	||
				id ==	0x01D7	||
				id ==	0x01D8	||
				id ==	0x01D9	||
				id ==	0x01DA	||
				id ==	0x021F	||
				id ==	0x0220	||
				id ==	0x0221	||
				id ==	0x0222	||
				id ==	0x0223	||
				id ==	0x0224	||
				id ==	0x0225	||
				id ==	0x0226	||
				id ==	0x0227	||
				id ==	0x0228	||
				id ==	0x0229	||
				id ==	0x022A	||
				id ==	0x022B	||
				id ==	0x022C	||
				id ==	0x022D	||
				id ==	0x022E	||
				id ==	0x022F	||
				id ==	0x0230	||
				id ==	0x0235	||
				id ==	0x0236	||
				id ==	0x0237	||
				id ==	0x0238	||
				id ==	0x06CD	||
				id ==	0x06CE	||
				id ==	0x06CF	||
				id ==	0x06D0	||
				id ==	0x06D1	||
				id ==	0x06DA	||
				id ==	0x06DB	||
				id ==	0x06DC	||
				id ==	0x06DD	||
				id ==	0x06EB	||
				id ==	0x06EC	||
				id ==	0x06ED	||
				id ==	0x06EE	||
				id ==	0x06EF	||
				id ==	0x06F0	||
				id ==	0x06F1	||
				id ==	0x06F2	||
				id ==	0x06FB	||
				id ==	0x06FC	||
				id ==	0x06FD	||
				id ==	0x06FE	||
				id ==	0x070E	||
				id ==	0x070F	||
				id ==	0x0710	||
				id ==	0x0711	||
				id ==	0x0712	||
				id ==	0x0713	||
				id ==	0x0714	||
				id ==	0x071D	||
				id ==	0x071E	||
				id ==	0x071F	||
				id ==	0x0720	||
				id ==	0x072B	||
				id ==	0x072C	||
				id ==	0x072D	||
				id ==	0x072E	||
				id ==	0x072F	||
				id ==	0x0730	||
				id ==	0x0731	||
				id ==	0x0732	||
				id ==	0x073B	||
				id ==	0x073C	||
				id ==	0x073D	||
				id ==	0x073E	||
				id ==	0x0749	||
				id ==	0x074A	||
				id ==	0x074B	||
				id ==	0x074C	||
				id ==	0x074D	||
				id ==	0x074E	||
				id ==	0x074F	||
				id ==	0x0750	||
				id ==	0x0759	||
				id ==	0x075A	||
				id ==	0x075B	||
				id ==	0x075C	||
				id ==	0x09EC	||
				id ==	0x09ED	||
				id ==	0x09EE	||
				id ==	0x09EF	||
				id ==	0x09F0	||
				id ==	0x09F1	||
				id ==	0x09F2	||
				id ==	0x09F3	||
				id ==	0x09F4	||
				id ==	0x09F5	||
				id ==	0x09F6	||
				id ==	0x09F7	||
				id ==	0x09F8	||
				id ==	0x09F9	||
				id ==	0x09FA	||
				id ==	0x09FB	||
				id ==	0x09FC	||
				id ==	0x09FD	||
				id ==	0x09FE	||
				id ==	0x09FF	||
				id ==	0x0A00	||
				id ==	0x0A01	||
				id ==	0x0A02	||
				id ==	0x0A03	||
				id ==	0x3F39	||
				id ==	0x3F3A	||
				id ==	0x3F3B	||
				id ==	0x3F3C	||
				id ==	0x3F3D	||
				id ==	0x3F3E	||
				id ==	0x3F3F	||
				id ==	0x3F40	||
				id ==	0x3F41	||
				id ==	0x3F42	||
				id ==	0x3F43	||
				id ==	0x3F44	||
				id ==	0x3F45	||
				id ==	0x3F46	||
				id ==	0x3F47	||
				id ==	0x3F48	||
				id ==	0x3F49	||
				id ==	0x3F4A	||
				id ==	0x3F4B	||
				id ==	0x3F4C	||
				id ==	0x3F4D	||
				id ==	0x3F4E	||
				id ==	0x3F4F	||
				id ==	0x3F50	||
				id ==	0x3F51	||
				id ==	0x3F52	||
				id ==	0x3F53	||
				id ==	0x3F54	||
				id ==	0x3F55	||
				id ==	0x3F56	||
				id ==	0x3F57	||
				id ==	0x3F58	||
				id ==	0x3F59	||
				id ==	0x3F5A	||
				id ==	0x3F5B	||
				id ==	0x3F5C	||
				id ==	0x3F5D	||
				id ==	0x3F5E	||
				id ==	0x3F5F	||
				id ==	0x3F60	||
				id ==	0x3F61	||
				id ==	0x3F62	||
				id ==	0x3F63	||
				id ==	0x3F64	||
				id ==	0x3F65	||
				id ==	0x3F66	||
				id ==	0x3F67	||
				id ==	0x3F68	||
				id ==	0x3F82	||
				id ==	0x3F83	||
				id ==	0x3F84	||
				id ==	0x3F85	||
				id ==	0x3F86	||
				id ==	0x3F87	||
				id ==	0x3F88	||
				id ==	0x3F89	||
				id ==	0x3F8A	||
				id ==	0x3F8B	||
				id ==	0x3F8C	||
				id ==	0x3F8D	||
				id ==	0x3F8E	||
				id ==	0x3F8F	||
				id ==	0x3F92	||
				id ==	0x3F93	||
				id ==	0x3F94	||
				id ==	0x3F95	||
				id ==	0x3F96	||
				id ==	0x3F97	||
				id ==	0x3F98	||
				id ==	0x3F99	||
				id ==	0x3F9A	||
				id ==	0x3F9B	||
				id ==	0x3F9C	||
				id ==	0x3F9D	||
				id ==	0x3F9E	||
				id ==	0x3F9F	||
				id ==	0x3FA0	||
				id ==	0x3FA1	||
				id ==	0x3FA2	||
				id ==	0x3FA3	||
				id ==	0x3FA4	||
				id ==	0x3FA5	||
				id ==	0x3FA6	||
				id ==	0x3FA7	||
				id ==	0x3FA8	||
				id ==	0x3FA9	||
				id ==	0x3FAA	||
				id ==	0x3FAB	||
				id ==	0x3FAC	||
				id ==	0x3FAD	||
				id ==	0x3FAE	||
				id ==	0x3FAF	||
				id ==	0x3FB0	||
				id ==	0x3FB1	||
				id ==	0x3FB2	||
				id ==	0x3FB3	||
				id ==	0x3FB4	||
				id ==	0x3FB5	||
				id ==	0x3FB6	||
				id ==	0x3FB7	||
				id ==	0x3FB8	||
				id ==	0x3FB9	||
				id ==	0x3FBA	||
				id ==	0x3FBB	||
				id ==	0x3FBC	||
				id ==	0x3FBD	||
				id ==	0x3FBE	||
				id ==	0x3FBF	||
				id ==	0x3FC0	||
				id ==	0x3FC1	||
				id ==	0x3FC2	||
				id ==	0x3FC3	||
				id ==	0x3FC4	||
				id ==	0x3FC5	||
				id ==	0x3FC6	||
				id ==	0x3FC7	||
				id ==	0x3FC8	||
				id ==	0x3FC9	||
				id ==	0x3FCA	||
				id ==	0x3FCB	||
				id ==	0x3FCC	||
				id ==	0x3FCD	||
				id ==	0x3FCE	||
				id ==	0x3FCF
			)){ return true; }

			if ( ( category == "sand" || category == "any" ) && ( 
				id ==	0x001A	||
				id ==	0x001B	||
				id ==	0x001C	||
				id ==	0x001D	||
				id ==	0x001E	||
				id ==	0x001F	||
				id ==	0x0020	||
				id ==	0x0021	||
				id ==	0x0022	||
				id ==	0x0023	||
				id ==	0x0024	||
				id ==	0x0025	||
				id ==	0x0026	||
				id ==	0x0027	||
				id ==	0x0028	||
				id ==	0x0029	||
				id ==	0x002A	||
				id ==	0x002B	||
				id ==	0x002C	||
				id ==	0x002D	||
				id ==	0x002E	||
				id ==	0x002F	||
				id ==	0x0030	||
				id ==	0x0031	||
				id ==	0x0032	||
				id ==	0x0044	||
				id ==	0x0045	||
				id ==	0x0046	||
				id ==	0x0047	||
				id ==	0x0048	||
				id ==	0x0049	||
				id ==	0x004A	||
				id ==	0x004B	||
				id ==	0x0126	||
				id ==	0x0127	||
				id ==	0x0128	||
				id ==	0x0129	||
				id ==	0x01B9	||
				id ==	0x01BA	||
				id ==	0x01BB	||
				id ==	0x01BC	||
				id ==	0x01BD	||
				id ==	0x01BE	||
				id ==	0x01BF	||
				id ==	0x01C0	||
				id ==	0x01C1	||
				id ==	0x01C2	||
				id ==	0x01C3	||
				id ==	0x01C4	||
				id ==	0x01C5	||
				id ==	0x01C6	||
				id ==	0x01C7	||
				id ==	0x01C8	||
				id ==	0x01C9	||
				id ==	0x01CA	||
				id ==	0x01CB	||
				id ==	0x01CC	||
				id ==	0x01CD	||
				id ==	0x01CE	||
				id ==	0x01CF	||
				id ==	0x01D0	||
				id ==	0x01D1
			)){ return true; }

			if ( ( category == "snow" || category == "any" ) && ( 
				id ==	0x010C	||
				id ==	0x010D	||
				id ==	0x010E	||
				id ==	0x010F	||
				id ==	0x0114	||
				id ==	0x0115	||
				id ==	0x0116	||
				id ==	0x0117	||
				id ==	0x017C	||
				id ==	0x017D	||
				id ==	0x017E	||
				id ==	0x017F	||
				id ==	0x0180	||
				id ==	0x0181	||
				id ==	0x0182	||
				id ==	0x0183	||
				id ==	0x0184	||
				id ==	0x0185	||
				id ==	0x0186	||
				id ==	0x0187	||
				id ==	0x0188	||
				id ==	0x0189	||
				id ==	0x018A	||
				id ==	0x0755	||
				id ==	0x0756	||
				id ==	0x0757	||
				id ==	0x0758	||
				id ==	0x076D	||
				id ==	0x076E	||
				id ==	0x076F	||
				id ==	0x0770	||
				id ==	0x0771	||
				id ==	0x0772	||
				id ==	0x0773
			)){ return true; }

			return false;
		}

		public static bool PassableTile ( int id, string category )
		{
			if ( ( category == "cave" || category == "any" ) && ( 
				id == 	0x0245	 || 
				id == 	0x0246	 || 
				id == 	0x0247	 || 
				id == 	0x0248	 || 
				id == 	0x0249	 || 
				id == 	0x063B	 || 
				id == 	0x063C	 || 
				id == 	0x063D	 || 
				id == 	0x063E
			)){ return true; }

			if ( ( category == "dirt" || category == "any" ) && ( 
				id ==	0x0071	||
				id ==	0x0072	||
				id ==	0x0073	||
				id ==	0x0074	||
				id ==	0x0075	||
				id ==	0x0076	||
				id ==	0x0077	||
				id ==	0x0078	||
				id ==	0x0079	||
				id ==	0x007A	||
				id ==	0x007B	||
				id ==	0x007C	||
				id ==	0x0082	||
				id ==	0x0083	||
				id ==	0x0085	||
				id ==	0x0086	||
				id ==	0x0087	||
				id ==	0x0088	||
				id ==	0x0089	||
				id ==	0x008A	||
				id ==	0x008B	||
				id ==	0x008C	||
				id ==	0x00E8	||
				id ==	0x00E9	||
				id ==	0x00EA	||
				id ==	0x00EB	||
				id ==	0x0141	||
				id ==	0x0142	||
				id ==	0x0143	||
				id ==	0x0144	||
				id ==	0x014C	||
				id ==	0x014D	||
				id ==	0x014E	||
				id ==	0x014F	||
				id ==	0x0169	||
				id ==	0x016A	||
				id ==	0x016B	||
				id ==	0x016C	||
				id ==	0x016D	||
				id ==	0x016E	||
				id ==	0x016F	||
				id ==	0x0170	||
				id ==	0x0171	||
				id ==	0x0172	||
				id ==	0x0173	||
				id ==	0x0174	||
				id ==	0x01DC	||
				id ==	0x01DD	||
				id ==	0x01DE	||
				id ==	0x01DF	||
				id ==	0x01E0	||
				id ==	0x01E1	||
				id ==	0x01E2	||
				id ==	0x01E3	||
				id ==	0x01E4	||
				id ==	0x01E5	||
				id ==	0x01E6	||
				id ==	0x01E7	||
				id ==	0x01EC	||
				id ==	0x01ED	||
				id ==	0x01EE	||
				id ==	0x01EF	||
				id ==	0x0272	||
				id ==	0x0273	||
				id ==	0x0274	||
				id ==	0x0275	||
				id ==	0x027E	||
				id ==	0x027F	||
				id ==	0x0280	||
				id ==	0x0281	||
				id ==	0x032C	||
				id ==	0x032D	||
				id ==	0x032E	||
				id ==	0x032F	||
				id ==	0x033D	||
				id ==	0x033E	||
				id ==	0x033F	||
				id ==	0x0340	||
				id ==	0x0345	||
				id ==	0x0346	||
				id ==	0x0347	||
				id ==	0x0348	||
				id ==	0x0349	||
				id ==	0x034A	||
				id ==	0x034B	||
				id ==	0x034C	||
				id ==	0x0355	||
				id ==	0x0356	||
				id ==	0x0357	||
				id ==	0x0358	||
				id ==	0x0367	||
				id ==	0x0368	||
				id ==	0x0369	||
				id ==	0x036A	||
				id ==	0x036B	||
				id ==	0x036C	||
				id ==	0x036D	||
				id ==	0x036E	||
				id ==	0x0377	||
				id ==	0x0378	||
				id ==	0x0379	||
				id ==	0x037A	||
				id ==	0x038D	||
				id ==	0x038E	||
				id ==	0x038F	||
				id ==	0x0390	||
				id ==	0x0395	||
				id ==	0x0396	||
				id ==	0x0397	||
				id ==	0x0398	||
				id ==	0x0399	||
				id ==	0x039A	||
				id ==	0x039B	||
				id ==	0x039C	||
				id ==	0x03A5	||
				id ==	0x03A6	||
				id ==	0x03A7	||
				id ==	0x03A8	||
				id ==	0x03F6	||
				id ==	0x03F7	||
				id ==	0x03F9	||
				id ==	0x03FA	||
				id ==	0x03FB	||
				id ==	0x03FC	||
				id ==	0x03FD	||
				id ==	0x03FE	||
				id ==	0x03FF	||
				id ==	0x0400	||
				id ==	0x0401	||
				id ==	0x0402	||
				id ==	0x0403	||
				id ==	0x0404	||
				id ==	0x0405	||
				id ==	0x0547	||
				id ==	0x0548	||
				id ==	0x0549	||
				id ==	0x054A	||
				id ==	0x054B	||
				id ==	0x054C	||
				id ==	0x054D	||
				id ==	0x054E	||
				id ==	0x0553	||
				id ==	0x0554	||
				id ==	0x0555	||
				id ==	0x0556	||
				id ==	0x0597	||
				id ==	0x0598	||
				id ==	0x0599	||
				id ==	0x059A	||
				id ==	0x059B	||
				id ==	0x059C	||
				id ==	0x059D	||
				id ==	0x059E	||
				id ==	0x0623	||
				id ==	0x0624	||
				id ==	0x0625	||
				id ==	0x0626	||
				id ==	0x0627	||
				id ==	0x0628	||
				id ==	0x0629	||
				id ==	0x062A	||
				id ==	0x062B	||
				id ==	0x062C	||
				id ==	0x062D	||
				id ==	0x062E	||
				id ==	0x062F	||
				id ==	0x0630	||
				id ==	0x0631	||
				id ==	0x0632	||
				id ==	0x0633	||
				id ==	0x0634	||
				id ==	0x0635	||
				id ==	0x0636	||
				id ==	0x0637	||
				id ==	0x0638	||
				id ==	0x0639	||
				id ==	0x063A	||
				id ==	0x06F3	||
				id ==	0x06F5	||
				id ==	0x06F6	||
				id ==	0x06F7	||
				id ==	0x06F8	||
				id ==	0x06F9	||
				id ==	0x06FA
			)){ return true; }

			if ( ( category == "forest" || category == "any" ) && ( 
				id ==	0x00C4	||
				id ==	0x00C5	||
				id ==	0x00C6	||
				id ==	0x00C7	||
				id ==	0x00C8	||
				id ==	0x00C9	||
				id ==	0x00CA	||
				id ==	0x00CB	||
				id ==	0x00CC	||
				id ==	0x00CD	||
				id ==	0x00CE	||
				id ==	0x00CF	||
				id ==	0x00D0	||
				id ==	0x00D1	||
				id ==	0x00D2	||
				id ==	0x00D3	||
				id ==	0x00D4	||
				id ==	0x00D5	||
				id ==	0x00D6	||
				id ==	0x00D7	||
				id ==	0x00F0	||
				id ==	0x00F1	||
				id ==	0x00F2	||
				id ==	0x00F3	||
				id ==	0x00F8	||
				id ==	0x00F9	||
				id ==	0x00FA	||
				id ==	0x00FB	||
				id ==	0x015D	||
				id ==	0x015E	||
				id ==	0x015F	||
				id ==	0x0160	||
				id ==	0x0161	||
				id ==	0x0162	||
				id ==	0x0163	||
				id ==	0x0164	||
				id ==	0x0165	||
				id ==	0x0166	||
				id ==	0x0167	||
				id ==	0x0168	||
				id ==	0x0324	||
				id ==	0x0325	||
				id ==	0x0326	||
				id ==	0x0327	||
				id ==	0x0328	||
				id ==	0x0329	||
				id ==	0x032A	||
				id ==	0x032B	||
				id ==	0x054F	||
				id ==	0x0550	||
				id ==	0x0551	||
				id ==	0x0552	||
				id ==	0x05F1	||
				id ==	0x05F2	||
				id ==	0x05F3	||
				id ==	0x05F4	||
				id ==	0x05F9	||
				id ==	0x05FA	||
				id ==	0x05FB	||
				id ==	0x05FC	||
				id ==	0x05FD	||
				id ==	0x05FE	||
				id ==	0x05FF	||
				id ==	0x0600	||
				id ==	0x0601	||
				id ==	0x0602	||
				id ==	0x0603	||
				id ==	0x0604	||
				id ==	0x0611	||
				id ==	0x0612	||
				id ==	0x0613	||
				id ==	0x0614	||
				id ==	0x0653	||
				id ==	0x0654	||
				id ==	0x0655	||
				id ==	0x0656	||
				id ==	0x065B	||
				id ==	0x065C	||
				id ==	0x065D	||
				id ==	0x065E	||
				id ==	0x065F	||
				id ==	0x0660	||
				id ==	0x0661	||
				id ==	0x0662	||
				id ==	0x066B	||
				id ==	0x066C	||
				id ==	0x066D	||
				id ==	0x066E	||
				id ==	0x06AF	||
				id ==	0x06B0	||
				id ==	0x06B1	||
				id ==	0x06B2	||
				id ==	0x06B3	||
				id ==	0x06B4	||
				id ==	0x06BB	||
				id ==	0x06BC	||
				id ==	0x06BD	||
				id ==	0x06BE	||
				id ==	0x0709	||
				id ==	0x070A	||
				id ==	0x070B	||
				id ==	0x070C	||
				id ==	0x0715	||
				id ==	0x0716	||
				id ==	0x0717	||
				id ==	0x0718	||
				id ==	0x0719	||
				id ==	0x071A	||
				id ==	0x071B	||
				id ==	0x071C
			)){ return true; }

			if ( ( category == "grass" || category == "any" ) && ( 
				id ==	0x0003	||
				id ==	0x0004	||
				id ==	0x0005	||
				id ==	0x0006	||
				id ==	0x003B	||
				id ==	0x003C	||
				id ==	0x003D	||
				id ==	0x003E	||
				id ==	0x007D	||
				id ==	0x007E	||
				id ==	0x007F	||
				id ==	0x00C0	||
				id ==	0x00C1	||
				id ==	0x00C2	||
				id ==	0x00C3	||
				id ==	0x00D8	||
				id ==	0x00D9	||
				id ==	0x00DA	||
				id ==	0x00DB	||
				id ==	0x01A4	||
				id ==	0x01A5	||
				id ==	0x01A6	||
				id ==	0x01A7	||
				id ==	0x0242	||
				id ==	0x0243	||
				id ==	0x036F	||
				id ==	0x0370	||
				id ==	0x0371	||
				id ==	0x0372	||
				id ==	0x0373	||
				id ==	0x0374	||
				id ==	0x0375	||
				id ==	0x0376	||
				id ==	0x037B	||
				id ==	0x037C	||
				id ==	0x037D	||
				id ==	0x037E	||
				id ==	0x03BF	||
				id ==	0x03C0	||
				id ==	0x03C1	||
				id ==	0x03C2	||
				id ==	0x03C3	||
				id ==	0x03C4	||
				id ==	0x03C5	||
				id ==	0x03C6	||
				id ==	0x03CB	||
				id ==	0x03CC	||
				id ==	0x03CD	||
				id ==	0x03CE	||
				id ==	0x0579	||
				id ==	0x057A	||
				id ==	0x057B	||
				id ==	0x057C	||
				id ==	0x057D	||
				id ==	0x057E	||
				id ==	0x057F	||
				id ==	0x0580	||
				id ==	0x058B	||
				id ==	0x058C	||
				id ==	0x05D7	||
				id ==	0x05D8	||
				id ==	0x05D9	||
				id ==	0x05DA	||
				id ==	0x05DB	||
				id ==	0x05DC	||
				id ==	0x05DD	||
				id ==	0x05DE	||
				id ==	0x05E3	||
				id ==	0x05E4	||
				id ==	0x05E5	||
				id ==	0x05E6	||
				id ==	0x067D	||
				id ==	0x067E	||
				id ==	0x067F	||
				id ==	0x0680	||
				id ==	0x0681	||
				id ==	0x0682	||
				id ==	0x0683	||
				id ==	0x0684	||
				id ==	0x0689	||
				id ==	0x068A	||
				id ==	0x068B	||
				id ==	0x068C	||
				id ==	0x0695	||
				id ==	0x0696	||
				id ==	0x0697	||
				id ==	0x0698	||
				id ==	0x0699	||
				id ==	0x069A	||
				id ==	0x069B	||
				id ==	0x069C	||
				id ==	0x06A1	||
				id ==	0x06A2	||
				id ==	0x06A3	||
				id ==	0x06A4	||
				id ==	0x06B5	||
				id ==	0x06B6	||
				id ==	0x06B7	||
				id ==	0x06B8	||
				id ==	0x06B9	||
				id ==	0x06BA	||
				id ==	0x06BF	||
				id ==	0x06C0	||
				id ==	0x06C1	||
				id ==	0x06C2	||
				id ==	0x06DE	||
				id ==	0x06DF	||
				id ==	0x06E0	||
				id ==	0x06E1
			)){ return true; }

			if ( ( category == "jungle" || category == "any" ) && ( 
				id ==	0x00AC	||
				id ==	0x00AD	||
				id ==	0x00AE	||
				id ==	0x00AF	||
				id ==	0x00B0	||
				id ==	0x00B3	||
				id ==	0x00B6	||
				id ==	0x00B9	||
				id ==	0x00BC	||
				id ==	0x00BD	||
				id ==	0x00BE	||
				id ==	0x00BF	||
				id ==	0x0100	||
				id ==	0x0101	||
				id ==	0x0102	||
				id ==	0x0103	||
				id ==	0x0108	||
				id ==	0x0109	||
				id ==	0x010A	||
				id ==	0x010B	||
				id ==	0x01F0	||
				id ==	0x01F1	||
				id ==	0x01F2	||
				id ==	0x01F3	||
				id ==	0x026E	||
				id ==	0x026F	||
				id ==	0x0270	||
				id ==	0x0271	||
				id ==	0x0276	||
				id ==	0x0277	||
				id ==	0x0278	||
				id ==	0x0279	||
				id ==	0x027A	||
				id ==	0x027B	||
				id ==	0x027C	||
				id ==	0x027D	||
				id ==	0x0286	||
				id ==	0x0287	||
				id ==	0x0288	||
				id ==	0x0289	||
				id ==	0x0292	||
				id ==	0x0293	||
				id ==	0x0294	||
				id ==	0x0295	||
				id ==	0x0581	||
				id ==	0x0582	||
				id ==	0x0583	||
				id ==	0x0584	||
				id ==	0x0585	||
				id ==	0x0586	||
				id ==	0x0587	||
				id ==	0x0588	||
				id ==	0x0589	||
				id ==	0x058A	||
				id ==	0x058D	||
				id ==	0x058E	||
				id ==	0x058F	||
				id ==	0x0590	||
				id ==	0x059F	||
				id ==	0x05A0	||
				id ==	0x05A1	||
				id ==	0x05A2	||
				id ==	0x05A3	||
				id ==	0x05A4	||
				id ==	0x05A5	||
				id ==	0x05A6	||
				id ==	0x05B3	||
				id ==	0x05B4	||
				id ==	0x05B5	||
				id ==	0x05B6	||
				id ==	0x05B7	||
				id ==	0x05B8	||
				id ==	0x05B9	||
				id ==	0x05BA	||
				id ==	0x05F5	||
				id ==	0x05F6	||
				id ==	0x05F7	||
				id ==	0x05F8	||
				id ==	0x0605	||
				id ==	0x0606	||
				id ==	0x0607	||
				id ==	0x0608	||
				id ==	0x0609	||
				id ==	0x060A	||
				id ==	0x060B	||
				id ==	0x060C	||
				id ==	0x060D	||
				id ==	0x060E	||
				id ==	0x060F	||
				id ==	0x0610	||
				id ==	0x0615	||
				id ==	0x0616	||
				id ==	0x0617	||
				id ==	0x0618	||
				id ==	0x0727	||
				id ==	0x0728	||
				id ==	0x0729	||
				id ==	0x0733	||
				id ==	0x0734	||
				id ==	0x0735	||
				id ==	0x0736	||
				id ==	0x0737	||
				id ==	0x0738	||
				id ==	0x0739	||
				id ==	0x073A
			)){ return true; }

			if ( ( category == "sand" || category == "any" ) && ( 
				id ==	0x0016	||
				id ==	0x0017	||
				id ==	0x0018	||
				id ==	0x0019	||
				id ==	0x0033	||
				id ==	0x0034	||
				id ==	0x0035	||
				id ==	0x0036	||
				id ==	0x0037	||
				id ==	0x0038	||
				id ==	0x0039	||
				id ==	0x003A	||
				id ==	0x011E	||
				id ==	0x011F	||
				id ==	0x0120	||
				id ==	0x0121	||
				id ==	0x012A	||
				id ==	0x012B	||
				id ==	0x012C	||
				id ==	0x012D	||
				id ==	0x01A8	||
				id ==	0x01A9	||
				id ==	0x01AA	||
				id ==	0x01AB	||
				id ==	0x0282	||
				id ==	0x0283	||
				id ==	0x0284	||
				id ==	0x0285	||
				id ==	0x028A	||
				id ==	0x028B	||
				id ==	0x028C	||
				id ==	0x028D	||
				id ==	0x028E	||
				id ==	0x028F	||
				id ==	0x0290	||
				id ==	0x0291	||
				id ==	0x0335	||
				id ==	0x0336	||
				id ==	0x0337	||
				id ==	0x0338	||
				id ==	0x0339	||
				id ==	0x033A	||
				id ==	0x033B	||
				id ==	0x033C	||
				id ==	0x0341	||
				id ==	0x0342	||
				id ==	0x0343	||
				id ==	0x0344	||
				id ==	0x034D	||
				id ==	0x034E	||
				id ==	0x034F	||
				id ==	0x0350	||
				id ==	0x0351	||
				id ==	0x0352	||
				id ==	0x0353	||
				id ==	0x0354	||
				id ==	0x0359	||
				id ==	0x035A	||
				id ==	0x035B	||
				id ==	0x035C	||
				id ==	0x03B7	||
				id ==	0x03B8	||
				id ==	0x03B9	||
				id ==	0x03BA	||
				id ==	0x03BB	||
				id ==	0x03BC	||
				id ==	0x03BD	||
				id ==	0x03BE	||
				id ==	0x03C7	||
				id ==	0x03C8	||
				id ==	0x03C9	||
				id ==	0x03CA	||
				id ==	0x05A7	||
				id ==	0x05A8	||
				id ==	0x05A9	||
				id ==	0x05AA	||
				id ==	0x05AB	||
				id ==	0x05AC	||
				id ==	0x05AD	||
				id ==	0x05AE	||
				id ==	0x05AF	||
				id ==	0x05B0	||
				id ==	0x05B1	||
				id ==	0x05B2	||
				id ==	0x064B	||
				id ==	0x064C	||
				id ==	0x064D	||
				id ==	0x064E	||
				id ==	0x064F	||
				id ==	0x0650	||
				id ==	0x0651	||
				id ==	0x0652	||
				id ==	0x0657	||
				id ==	0x0658	||
				id ==	0x0659	||
				id ==	0x065A	||
				id ==	0x0663	||
				id ==	0x0664	||
				id ==	0x0665	||
				id ==	0x0666	||
				id ==	0x0667	||
				id ==	0x0668	||
				id ==	0x0669	||
				id ==	0x066A	||
				id ==	0x066F	||
				id ==	0x0670	||
				id ==	0x0671	||
				id ==	0x0672
			)){ return true; }

			if ( ( category == "snow" || category == "any" ) && ( 
				id ==	0x011A	||
				id ==	0x011B	||
				id ==	0x011C	||
				id ==	0x011D	||
				id ==	0x012E	||
				id ==	0x012F	||
				id ==	0x0130	||
				id ==	0x0131	||
				id ==	0x0179	||
				id ==	0x017A	||
				id ==	0x017B	||
				id ==	0x0385	||
				id ==	0x0386	||
				id ==	0x0387	||
				id ==	0x0388	||
				id ==	0x0389	||
				id ==	0x038A	||
				id ==	0x038B	||
				id ==	0x038C	||
				id ==	0x0391	||
				id ==	0x0392	||
				id ==	0x0393	||
				id ==	0x0394	||
				id ==	0x039D	||
				id ==	0x039E	||
				id ==	0x039F	||
				id ==	0x03A0	||
				id ==	0x03A1	||
				id ==	0x03A2	||
				id ==	0x03A3	||
				id ==	0x03A4	||
				id ==	0x03A9	||
				id ==	0x03AA	||
				id ==	0x03AB	||
				id ==	0x03AC	||
				id ==	0x05BF	||
				id ==	0x05C0	||
				id ==	0x05C1	||
				id ==	0x05C2	||
				id ==	0x05C3	||
				id ==	0x05C4	||
				id ==	0x05C5	||
				id ==	0x05C6	||
				id ==	0x05C7	||
				id ==	0x05C8	||
				id ==	0x05C9	||
				id ==	0x05CA	||
				id ==	0x05CB	||
				id ==	0x05CC	||
				id ==	0x05CD	||
				id ==	0x05CE	||
				id ==	0x05CF	||
				id ==	0x05D0	||
				id ==	0x05D1	||
				id ==	0x05D2	||
				id ==	0x05D3	||
				id ==	0x05D4	||
				id ==	0x05D5	||
				id ==	0x05D6	||
				id ==	0x05DF	||
				id ==	0x05E0	||
				id ==	0x05E1	||
				id ==	0x05E2	||
				id ==	0x0745	||
				id ==	0x0746	||
				id ==	0x0747	||
				id ==	0x0748	||
				id ==	0x0751	||
				id ==	0x0752	||
				id ==	0x0753	||
				id ==	0x0754	||
				id ==	0x075D	||
				id ==	0x075E	||
				id ==	0x075F	||
				id ==	0x0760
			)){ return true; }

			if ( ( category == "stone" || category == "any" ) && ( 
				id ==	0x0436	||
				id ==	0x0437	||
				id ==	0x0438	||
				id ==	0x0439	||
				id ==	0x043A	||
				id ==	0x043B	||
				id ==	0x043C	||
				id ==	0x043D	||
				id ==	0x043E	||
				id ==	0x043F	||
				id ==	0x0440	||
				id ==	0x0441	||
				id ==	0x0442	||
				id ==	0x0443	||
				id ==	0x0444	||
				id ==	0x0445
			)){ return true; }

			if ( ( category == "swamp" || category == "any" ) && ( 
				id ==	0x3D65	||
				id ==	0x3D66	||
				id ==	0x3D67	||
				id ==	0x3D68	||
				id ==	0x3D69	||
				id ==	0x3D6A	||
				id ==	0x3D6B	||
				id ==	0x3D6C	||
				id ==	0x3D6D	||
				id ==	0x3D6E	||
				id ==	0x3D6F	||
				id ==	0x3D70	||
				id ==	0x3D71	||
				id ==	0x3D72	||
				id ==	0x3D73	||
				id ==	0x3D74	||
				id ==	0x3D75	||
				id ==	0x3D76	||
				id ==	0x3D77	||
				id ==	0x3D78	||
				id ==	0x3D79	||
				id ==	0x3D7A	||
				id ==	0x3D7B	||
				id ==	0x3D7C	||
				id ==	0x3D7D	||
				id ==	0x3D7E	||
				id ==	0x3D7F	||
				id ==	0x3D80	||
				id ==	0x3D81	||
				id ==	0x3D82	||
				id ==	0x3D83	||
				id ==	0x3D84	||
				id ==	0x3D85	||
				id ==	0x3D86	||
				id ==	0x3D87	||
				id ==	0x3D88	||
				id ==	0x3D89	||
				id ==	0x3D8A	||
				id ==	0x3D8B	||
				id ==	0x3D8C	||
				id ==	0x3D8D	||
				id ==	0x3D8E	||
				id ==	0x3D8F	||
				id ==	0x3D90	||
				id ==	0x3D91	||
				id ==	0x3D92	||
				id ==	0x3D93	||
				id ==	0x3D94	||
				id ==	0x3D95	||
				id ==	0x3D96	||
				id ==	0x3D97	||
				id ==	0x3D98	||
				id ==	0x3D99	||
				id ==	0x3D9A	||
				id ==	0x3D9B	||
				id ==	0x3D9C	||
				id ==	0x3D9D	||
				id ==	0x3D9E	||
				id ==	0x3D9F	||
				id ==	0x3DA0	||
				id ==	0x3DA1	||
				id ==	0x3DA2	||
				id ==	0x3DA3	||
				id ==	0x3DA4	||
				id ==	0x3DA5	||
				id ==	0x3DA6	||
				id ==	0x3DA7	||
				id ==	0x3DA8	||
				id ==	0x3DA9	||
				id ==	0x3DAA	||
				id ==	0x3DAB	||
				id ==	0x3DAC	||
				id ==	0x3DAD	||
				id ==	0x3DAE	||
				id ==	0x3DAF	||
				id ==	0x3DB0	||
				id ==	0x3DB1	||
				id ==	0x3DB2	||
				id ==	0x3DB3	||
				id ==	0x3DB4	||
				id ==	0x3DB5	||
				id ==	0x3DB6	||
				id ==	0x3DB7	||
				id ==	0x3DB8	||
				id ==	0x3DB9	||
				id ==	0x3DBA	||
				id ==	0x3DBB	||
				id ==	0x3DBC	||
				id ==	0x3DBD	||
				id ==	0x3DBE	||
				id ==	0x3DBF	||
				id ==	0x3DC0	||
				id ==	0x3DC1	||
				id ==	0x3DC2	||
				id ==	0x3DC3	||
				id ==	0x3DC4	||
				id ==	0x3DC5	||
				id ==	0x3DC6	||
				id ==	0x3DC7	||
				id ==	0x3DC8	||
				id ==	0x3DC9	||
				id ==	0x3DCA	||
				id ==	0x3DCB	||
				id ==	0x3DCC	||
				id ==	0x3DCD	||
				id ==	0x3DCE	||
				id ==	0x3DCF	||
				id ==	0x3DD0	||
				id ==	0x3DD1	||
				id ==	0x3DD2	||
				id ==	0x3DD3	||
				id ==	0x3DD4	||
				id ==	0x3DD5	||
				id ==	0x3DD6	||
				id ==	0x3DD7	||
				id ==	0x3DD8	||
				id ==	0x3DD9	||
				id ==	0x3DDA	||
				id ==	0x3DDB	||
				id ==	0x3DDC	||
				id ==	0x3DDD	||
				id ==	0x3DDE	||
				id ==	0x3DDF	||
				id ==	0x3DE0	||
				id ==	0x3DE1	||
				id ==	0x3DE2	||
				id ==	0x3DE3	||
				id ==	0x3DE4	||
				id ==	0x3DE5	||
				id ==	0x3DE6	||
				id ==	0x3DE7	||
				id ==	0x3DE8	||
				id ==	0x3DE9	||
				id ==	0x3DEA	||
				id ==	0x3DEB	||
				id ==	0x3DEC	||
				id ==	0x3DED	||
				id ==	0x3DEE	||
				id ==	0x3DEF	||
				id ==	0x3DF0	||
				id ==	0x3DF1
			)){ return true; }

			return false;
		}

		//[Obsolete( "Depreciated, use the methods for the Mobile's race", false )]
		public static int ClipHairHue( int hue )
		{
			if ( hue < 1102 )
				return 1102;
			else if ( hue > 1149 )
				return 1149;
			else
				return hue;
		}

		//[Obsolete( "Depreciated, use the methods for the Mobile's race", false )]
		public static int RandomHairHue()
		{
			return Random( 1102, 48 );
		}

		#endregion

		private static SkillName[] m_AllSkills = new SkillName[]
			{
				SkillName.Alchemy,
				SkillName.Anatomy,
				SkillName.Druidism,
				SkillName.Mercantile,
				SkillName.ArmsLore,
				SkillName.Parry,
				SkillName.Begging,
				SkillName.Blacksmith,
				SkillName.Bowcraft,
				SkillName.Peacemaking,
				SkillName.Camping,
				SkillName.Carpentry,
				SkillName.Cartography,
				SkillName.Cooking,
				SkillName.Searching,
				SkillName.Discordance,
				SkillName.Psychology,
				SkillName.Healing,
				SkillName.Seafaring,
				SkillName.Forensics,
				SkillName.Herding,
				SkillName.Hiding,
				SkillName.Provocation,
				SkillName.Inscribe,
				SkillName.Lockpicking,
				SkillName.Magery,
				SkillName.MagicResist,
				SkillName.Tactics,
				SkillName.Snooping,
				SkillName.Musicianship,
				SkillName.Poisoning,
				SkillName.Marksmanship,
				SkillName.Spiritualism,
				SkillName.Stealing,
				SkillName.Tailoring,
				SkillName.Taming,
				SkillName.Tasting,
				SkillName.Tinkering,
				SkillName.Tracking,
				SkillName.Veterinary,
				SkillName.Swords,
				SkillName.Bludgeoning,
				SkillName.Fencing,
				SkillName.FistFighting,
				SkillName.Lumberjacking,
				SkillName.Mining,
				SkillName.Meditation,
				SkillName.Stealth,
				SkillName.RemoveTrap,
				SkillName.Necromancy,
				SkillName.Focus,
				SkillName.Knightship,
				SkillName.Bushido,
				SkillName.Ninjitsu,
				SkillName.Elementalism
			};

		private static SkillName[] m_CombatSkills = new SkillName[]
			{
				SkillName.Marksmanship,
				SkillName.Swords,
				SkillName.Bludgeoning,
				SkillName.Fencing,
				SkillName.FistFighting
			};

		private static SkillName[] m_CraftSkills = new SkillName[]
			{
				SkillName.Alchemy,
				SkillName.Blacksmith,
				SkillName.Bowcraft,
				SkillName.Carpentry,
				SkillName.Cartography,
				SkillName.Cooking,
				SkillName.Inscribe,
				SkillName.Tailoring,
				SkillName.Tinkering
			};

		public static SkillName RandomSkill()
		{
			return m_AllSkills[Utility.Random(m_AllSkills.Length - ( Core.ML ? 0 : Core.SE ? 1 : Core.AOS ? 3 : 6 ) )];
		}

		public static SkillName RandomCombatSkill()
		{
			return m_CombatSkills[Utility.Random(m_CombatSkills.Length)];
		}

		public static SkillName RandomCraftSkill()
		{
			return m_CraftSkills[Utility.Random(m_CraftSkills.Length)];
		}

		public static void FixPoints( ref Point3D top, ref Point3D bottom )
		{
			if ( bottom.m_X < top.m_X )
			{
				int swap = top.m_X;
				top.m_X = bottom.m_X;
				bottom.m_X = swap;
			}

			if ( bottom.m_Y < top.m_Y )
			{
				int swap = top.m_Y;
				top.m_Y = bottom.m_Y;
				bottom.m_Y = swap;
			}

			if ( bottom.m_Z < top.m_Z )
			{
				int swap = top.m_Z;
				top.m_Z = bottom.m_Z;
				bottom.m_Z = swap;
			}
		}

		public static ArrayList BuildArrayList( IEnumerable enumerable )
		{
			IEnumerator e = enumerable.GetEnumerator();

			ArrayList list = new ArrayList();

			while ( e.MoveNext() )
			{
				list.Add( e.Current );
			}

			return list;
		}

		public static bool RangeCheck( IPoint2D p1, IPoint2D p2, int range )
		{
			return ( p1.X >= (p2.X - range) )
				&& ( p1.X <= (p2.X + range) )
				&& ( p1.Y >= (p2.Y - range) )
				&& ( p1.Y <= (p2.Y + range) );
		}

		public static void FormatBuffer( TextWriter output, Stream input, int length )
		{
			output.WriteLine( "        0  1  2  3  4  5  6  7   8  9  A  B  C  D  E  F" );
			output.WriteLine( "       -- -- -- -- -- -- -- --  -- -- -- -- -- -- -- --" );

			int byteIndex = 0;

			int whole = length >> 4;
			int rem = length & 0xF;

			for ( int i = 0; i < whole; ++i, byteIndex += 16 )
			{
				StringBuilder bytes = new StringBuilder( 49 );
				StringBuilder chars = new StringBuilder( 16 );

				for ( int j = 0; j < 16; ++j )
				{
					int c = input.ReadByte();

					bytes.Append( c.ToString( "X2" ) );

					if ( j != 7 )
					{
						bytes.Append( ' ' );
					}
					else
					{
						bytes.Append( "  " );
					}

					if ( c >= 0x20 && c < 0x80 )
					{
						chars.Append( (char)c );
					}
					else
					{
						chars.Append( '.' );
					}
				}

				output.Write( byteIndex.ToString( "X4" ) );
				output.Write( "   " );
				output.Write( bytes.ToString() );
				output.Write( "  " );
				output.WriteLine( chars.ToString() );
			}

			if ( rem != 0 )
			{
				StringBuilder bytes = new StringBuilder( 49 );
				StringBuilder chars = new StringBuilder( rem );

				for ( int j = 0; j < 16; ++j )
				{
					if ( j < rem )
					{
						int c = input.ReadByte();

						bytes.Append( c.ToString( "X2" ) );

						if ( j != 7 )
						{
							bytes.Append( ' ' );
						}
						else
						{
							bytes.Append( "  " );
						}

						if ( c >= 0x20 && c < 0x80 )
						{
							chars.Append( (char)c );
						}
						else
						{
							chars.Append( '.' );
						}
					}
					else
					{
						bytes.Append( "   " );
					}
				}

				output.Write( byteIndex.ToString( "X4" ) );
				output.Write( "   " );
				output.Write( bytes.ToString() );
				output.Write( "  " );
				output.WriteLine( chars.ToString() );
			}
		}

		private static Stack<ConsoleColor> m_ConsoleColors = new Stack<ConsoleColor>();

		public static void PushColor( ConsoleColor color )
		{
			try
			{
				m_ConsoleColors.Push( Console.ForegroundColor );
				Console.ForegroundColor = color;
			}
			catch
			{
			}
		}

		public static void PopColor()
		{
			try
			{
				Console.ForegroundColor = m_ConsoleColors.Pop();
			}
			catch
			{
			}
		}

		public static bool NumberBetween( double num, int bound1, int bound2, double allowance )
		{
			if ( bound1 > bound2 )
			{
				int i = bound1;
				bound1 = bound2;
				bound2 = i;
			}

			return ( num<bound2+allowance && num>bound1-allowance );
		}

		public static void AssignRandomHair( Mobile m )
		{
			AssignRandomHair( m, true );
		}
		public static void AssignRandomHair( Mobile m, int hue )
		{
			m.HairItemID = m.Race.RandomHair( m );
			m.HairHue = hue;
		}
		public static void AssignRandomHair( Mobile m, bool randomHue )
		{
			m.HairItemID = m.Race.RandomHair( m );

			if( randomHue )
				m.HairHue = Utility.RandomHairHue();
		}

		public static void AssignRandomFacialHair( Mobile m )
		{
			AssignRandomFacialHair( m, true );
		}
		public static void AssignRandomFacialHair( Mobile m, int hue )
		{
			m.FacialHairHue = m.Race.RandomFacialHair( m );
			m.FacialHairHue = hue;
		}
		public static void AssignRandomFacialHair( Mobile m, bool randomHue )
		{
			m.FacialHairItemID = m.Race.RandomFacialHair( m );

			if( randomHue )
				m.FacialHairHue = Utility.RandomHairHue();
		}

#if MONO
		public static List<TOutput> CastConvertList<TInput, TOutput>( List<TInput> list ) where TInput : class where TOutput : class
		{
			return list.ConvertAll<TOutput>( new  Converter<TInput, TOutput>( delegate( TInput value ) { return value as TOutput; } ) );
		}
#else
		public static List<TOutput> CastConvertList<TInput, TOutput>( List<TInput> list ) where TOutput : TInput
		{
			return list.ConvertAll<TOutput>( new Converter<TInput, TOutput>( delegate( TInput value ) { return (TOutput)value; } ) );
		}
#endif

		public static List<TOutput> SafeConvertList<TInput, TOutput>( List<TInput> list ) where TOutput : class
		{
			List<TOutput> output = new List<TOutput>( list.Capacity );

			for( int i = 0; i < list.Count; i++ )
			{
				TOutput t = list[i] as TOutput;

				if( t != null )
					output.Add( t );
			}

			return output;
		}
	}
}