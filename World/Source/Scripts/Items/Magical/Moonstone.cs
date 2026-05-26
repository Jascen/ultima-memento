using Server;
using System;
using System.Collections;
using Server.Network;
using Server.Targeting;
using Server.Prompts;
using Server.Misc;
using Server.Mobiles;

namespace Server.Items
{
	public class MoonStone : Item
	{
		private InternalTimer m_MoonTimer;

		[Constructable]
		public MoonStone() : base( 0xF8B )
		{
			Weight = 1.0;
			Name = "Moonstone";
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			list.Add( 1070722, "Magically Open A Moongate" );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1060640 ); // The item must be in your backpack to use it.
			else if ( from is PlayerMobile && from.Land == Land.Kuldar && !( Server.Misc.PlayerSettings.GetKeys( from, "VordoKey" ) ) )
				from.SendMessage( "This magical gate doesn't seem to do anything." );
			else if ( !Worlds.AllowEscape( from, from.Map, from.Location, from.X, from.Y ) && from.Land != Land.Kuldar )
				from.SendMessage( "This magical gate doesn't seem to do anything." );
			else if ( !Worlds.RegionAllowedRecall( from.Map, from.Location, from.X, from.Y ) && from.Land != Land.Ambrosia && from.Land != Land.Kuldar )
				from.SendMessage( "This magical gate doesn't seem to do anything." );
			else
			{
				Effects.PlaySound( from.Location, from.Map, 0x20E );
				m_MoonTimer = new InternalTimer(from);
				m_MoonTimer.Start ();
				this.Delete();
			}
		}

		private class InternalTimer : Timer
		{
			private GateMoon m_MoonGate;
			private Item m_MoonEffect;
			
			public InternalTimer ( Mobile from ) : base (TimeSpan.FromSeconds(0))
			{
				Delay = TimeSpan.FromSeconds(120);
				Priority = TimerPriority.OneSecond;
				m_MoonGate = new GateMoon();
				m_MoonGate.Visible = false;
				m_MoonGate.Weight = -2.0;
				m_MoonGate.MoveToWorld (new Point3D(from.X, from.Y, from.Z), from.Map);
				m_MoonEffect = new StonePaversDark();
				m_MoonEffect.Visible = true;
				m_MoonEffect.Weight = -2.0;
				m_MoonEffect.ItemID = 0x4C82;
				m_MoonEffect.Light = LightType.Circle300;
				m_MoonEffect.MoveToWorld (new Point3D(from.X, from.Y, from.Z+5), from.Map);
			}
			
			protected override void OnTick ()
			{
				((Item)m_MoonGate).Delete ();
				((Item)m_MoonEffect).Delete ();
				Stop();
			}
		}

		public MoonStone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( ( int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}