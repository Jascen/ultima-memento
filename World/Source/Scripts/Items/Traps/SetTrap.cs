using System;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class SetTrap : Item
	{
		private const int RANGE = 1;
		public Mobile owner;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner { get{ return owner; } set{ owner = value; } }

		public int power;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Power { get{ return power; } set{ power = value; } }

		public override bool HandlesOnMovement{ get{ return true; } }

		private DateTime m_DecayTime;
		private Timer m_DecayTimer;

		public virtual TimeSpan DecayDelay{ get{ return TimeSpan.FromSeconds( 180.0 ); } } // HOW LONG UNTIL THE TRAP DECAYS IN SECONDS

		[Constructable]
		public SetTrap( Mobile source, int level ) : base( 0x0702 )
		{
			Movable = false;
			Name = "a trap";
			owner = source;
			power = level;
			RefreshDecay( true );
		}

		public SetTrap(Serial serial) : base(serial)
		{
		}

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( m is PlayerMobile ) return; // Players must walk over it directly

			if ( m.InRange( this, RANGE ) )
			{
				OnMoveOver( m );
			}
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( owner == m ) return true;

			if ( m.Region.AllowHarmful( owner, m ) )
			{
				int StrMax = power;
				int StrMin = (int)(power/2);

				if ( m is PlayerMobile && Spells.Research.ResearchAirWalk.UnderEffect( m ) )
				{
					Point3D air = new Point3D( ( m.X+1 ), ( m.Y+1 ), ( m.Z+5 ) );
					Effects.SendLocationParticles(EffectItem.Create(air, m.Map, EffectItem.DefaultDuration), 0x2007, 9, 32, Server.Misc.PlayerSettings.GetMySpellHue( true, m, 0 ), 0, 5022, 0);
					m.PlaySound( 0x014 );
				}
				else if (
				( m is PlayerMobile && m.Blessed == false && m.Alive && m.AccessLevel == AccessLevel.Player && Server.Misc.SeeIfGemInBag.GemInPocket( m ) == false && Server.Misc.SeeIfJewelInBag.JewelInPocket( m ) == false ) 
				|| 
				( m is BaseCreature && m.Blessed == false && !(m is PlayerMobile ) ) 
				)
				{
					bool Sprung = Server.Items.HiddenTrap.CheckTrapAvoidance( m, this );

					if ( Sprung )
					{
						if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ Effects.SendLocationEffect( this.Location, this.Map, 4506 + 1, 18, 3, 0, 0 ); }
						else { Effects.SendLocationEffect( this.Location, this.Map, 4512 + 1, 18, 3, 0, 0 ); }
						Effects.PlaySound( this.Location, this.Map, 0x22C );
						if ( m is PlayerMobile ){ m.LocalOverheadMessage(MessageType.Emote, 0x916, true, "You triggered a trap!"); }
						int itHurts = (int)( (Utility.RandomMinMax(StrMin,StrMax) * ( 100 - m.PhysicalResistance ) ) / 100 ) + 10;
						m.Damage( itHurts, owner );
						if ( m is BaseCreature )
							owner.DoHarmful( m );
					}
					else
					{
						Effects.PlaySound( this.Location, this.Map, 0x241 );
						if ( owner != null ){ owner.SendMessage( "Your trap seems to have been thwarted!" ); }
					}
					this.Delete();
				}
			}

			return true;
		}

		public virtual void RefreshDecay( bool setDecayTime )
		{
			if( Deleted )
				return;
			if( m_DecayTimer != null )
				m_DecayTimer.Stop();
			if( setDecayTime )
				m_DecayTime = DateTime.Now + DecayDelay;

			TimeSpan ts = m_DecayTime - DateTime.Now;

			if ( ts < TimeSpan.FromMinutes( 2.0 ) )
				ts = TimeSpan.FromMinutes( 2.0 );

			m_DecayTimer = Timer.DelayCall( ts, new TimerCallback( Delete ) );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			writer.Write( m_DecayTime );
			writer.Write( (Mobile)owner );
			writer.Write( (int)power );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			switch ( version )
			{
				case 0:
				{
					m_DecayTime = reader.ReadDateTime();
					RefreshDecay( false );
					break;
				}
			}
			owner = reader.ReadMobile();
			power = reader.ReadInt();
		}
	}
}