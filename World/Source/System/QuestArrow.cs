/***************************************************************************
 *                               QuestArrow.cs
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
using Server;
using Server.Network;

namespace Server
{
	public class QuestArrow
	{
		private readonly Mobile m_Mobile;
		private readonly IPoint3D m_Target;
		private bool m_Running;

		public bool Running
		{
			get
			{
				return m_Running;
			}
		}

		public void Update()
		{
			Update( m_Target.X, m_Target.Y );
		}

		public void Update( int x, int y )
		{
			if ( !m_Running )
				return;

			NetState ns = m_Mobile.NetState;

			if ( ns == null )
				return;

			if ( ns.HighSeas )
				ns.Send( new SetArrowHS( x, y, GetTargetSerialHS() ) );
			else
				ns.Send( new SetArrow( x, y ) );
		}

		public void Stop()
		{
			Stop( m_Target.X, m_Target.Y );
		}

		public void Stop( int x, int y )
		{
			if ( !m_Running )
				return;

			m_Mobile.ClearQuestArrow();

			NetState ns = m_Mobile.NetState;

			if ( ns != null ) {
				if ( ns.HighSeas )
					ns.Send( new CancelArrowHS( x, y, GetTargetSerialHS() ) );
				else
					ns.Send( new CancelArrow() );
			}

			m_Running = false;
			OnStop();
		}

		public virtual void OnStop()
		{
		}

		public virtual void OnClick( bool rightClick )
		{
		}

		private Serial GetTargetSerialHS()
		{
			if (m_Target is IEntity)
				return ((IEntity)m_Target).Serial;

			// HS requires a serial for the target
			// Fall back to the Mobile itself
			return m_Mobile.Serial;
		}

		public QuestArrow( Mobile m, IPoint3D t )
		{
			m_Running = true;
			m_Mobile = m;
			m_Target = t;
		}

		public QuestArrow( Mobile m, Mobile t, int x, int y ) : this( m, t )
		{
			Update( x, y );
		}
	}
}