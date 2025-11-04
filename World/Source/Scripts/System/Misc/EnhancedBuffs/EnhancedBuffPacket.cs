using System;
using Server;
using Server.Network;

namespace Server
{
    public sealed class EnhancedBuffPacket : Packet
    {
        public EnhancedBuffPacket( Mobile mob, BuffIcon iconID, int titleCliloc, int secondaryCliloc, TimeSpan length ) : base( 0xDF )
        {
            this.EnsureCapacity( 44 );
            m_Stream.Write( (int)mob.Serial );

            m_Stream.Write( (short)iconID );
            m_Stream.Write( (short)0x1 );

            m_Stream.Fill( 4 );

            m_Stream.Write( (short)iconID );
            m_Stream.Write( (short)0x01 );

            m_Stream.Fill( 4 );

            if( length < TimeSpan.Zero )
                length = TimeSpan.Zero;

            m_Stream.Write( (short)Math.Ceiling(length.TotalSeconds) );

            m_Stream.Fill( 3 );
            m_Stream.Write( (int)titleCliloc );
            m_Stream.Write( (int)secondaryCliloc );

            m_Stream.Fill( 10 );
        }

        public EnhancedBuffPacket( Mobile mob, BuffIcon iconID, int titleCliloc, int secondaryCliloc, string customArgs, TimeSpan length ) : base( 0xDF )
        {
            this.EnsureCapacity( 48 + customArgs.Length * 2 );
            m_Stream.Write( (int)mob.Serial );

            m_Stream.Write( (short)iconID );
            m_Stream.Write( (short)0x1 );

            m_Stream.Fill( 4 );

            m_Stream.Write( (short)iconID );
            m_Stream.Write( (short)0x01 );

            m_Stream.Fill( 4 );

            if( length < TimeSpan.Zero )
                length = TimeSpan.Zero;

            m_Stream.Write( (short)Math.Ceiling(length.TotalSeconds) );

            m_Stream.Fill( 3 );
            m_Stream.Write( (int)titleCliloc );
            m_Stream.Write( (int)secondaryCliloc );

            m_Stream.Fill( 4 );
            m_Stream.Write( (short)0x1 );
            m_Stream.Fill( 2 );

            m_Stream.WriteLittleUniNull( String.Format( "\t{0}", customArgs ) );

            m_Stream.Write( (short)0x1 );
            m_Stream.Fill( 2 );
        }
    }
}