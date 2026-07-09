using System;
using Server;
using Server.Misc;

namespace Server.Items
{
	public enum TrapType
	{
		None,
		MagicTrap,
		ExplosionTrap,
		DartTrap,
		PoisonTrap
	}

	public abstract class TrapableContainer : BaseContainer, ITelekinesisable
	{
		private TrapType m_TrapType;
		private int m_TrapPower;
		private int m_TrapLevel;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsActive { get { return TrapType != TrapType.None; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public TrapType TrapType
		{
			get
			{
				return m_TrapType;
			}
			set
			{
				m_TrapType = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int TrapPower
		{
			get
			{
				return m_TrapPower;
			}
			set
			{
				m_TrapPower = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int TrapLevel
		{
			get
			{
				return m_TrapLevel;
			}
			set
			{
				m_TrapLevel = value;
			}
		}

		public virtual bool TrapOnOpen{ get{ return true; } }

		public TrapableContainer( int itemID ) : base( itemID )
		{
		}

		public TrapableContainer( Serial serial ) : base( serial )
		{
		}

		private void SendMessageTo( Mobile to, int number, int hue )
		{
			if ( Deleted || !to.CanSee( this ) )
				return;

			to.Send( new Network.MessageLocalized( Serial, ItemID, Network.MessageType.Regular, hue, 3, number, "", "" ) );
		}

		private void SendMessageTo( Mobile to, string text, int hue )
		{
			if ( Deleted || !to.CanSee( this ) )
				return;

			to.Send( new Network.UnicodeMessage( Serial, ItemID, Network.MessageType.Regular, hue, 3, "ENU", "", text ) );
		}

		public static bool PassiveSearching( TrapableContainer box, Mobile m )
		{
			if ( m.Skills.Searching.Value >= 5 && box.TrapType != TrapType.None )
			{
				if ( !box.SearchedThis && m.CheckSkill( SkillName.Searching, 0, 125 ) )
				{
					m.PlaySound( m.Female ? 778 : 1049 ); m.Say( "*ah!*" );
					m.SendMessage( "This container is trapped!" );
					box.SearchedThis = true;
					return true;
				}
			}

			box.SearchedThis = true;
			return false;
		}

		public virtual bool ExecuteTrap( Mobile from )
		{
			if ( m_TrapType != TrapType.None )
			{
				Point3D loc = this.GetWorldLocation();
				Map facet = this.Map;

				if ( from.AccessLevel >= AccessLevel.GameMaster )
				{
					SendMessageTo( from, "That is trapped, but you open it with your godly powers.", 0x3B2 );
					return false;
				}

				int nTrapLevel = TrapLevel * 10;
				int nTrapLevel2 = nTrapLevel + 20;

				if ( (int)(from.Skills[SkillName.RemoveTrap].Value ) < nTrapLevel )
				{
					from.CheckTargetSkill( SkillName.RemoveTrap, this, 0, nTrapLevel2 );
				}
				else if ( from.CheckTargetSkill( SkillName.RemoveTrap, this, 0, nTrapLevel2 ) )
				{
					from.PlaySound( 0x241 );
					TrapPower = 0;
					TrapLevel = 0;
					TrapType = TrapType.None;
					SendMessageTo( from, "That was trapped, but you were able to disable it.", 0x3B2 );
					return false;
				}

				if ( from.Backpack != null )
				{
					Item magicwand = from.Backpack.FindItemByType( typeof ( TrapWand ) );
					Item tenfootpole = from.Backpack.FindItemByType( typeof ( TenFootPole ) );

					if ( GetPlayerInfo.LuckyPlayer(from.Luck) )
					{
						from.PlaySound( 0x241 );
						TrapPower = 0;
						TrapLevel = 0;
						TrapType = TrapType.None;
						SendMessageTo( from, "That was trapped, but with luck on your side...it broke.", 0x3B2 );
						return false;
					}
					if ( magicwand != null )
					{
						TrapWand wands = (TrapWand)magicwand;
						int nPower = wands.WandPower;
						int nAgainst = Utility.RandomMinMax( nTrapLevel, nTrapLevel2 );
						if ( nPower >= nAgainst )
						{
							from.PlaySound( 0x1F0 );
							TrapPower = 0;
							TrapLevel = 0;
							TrapType = TrapType.None;
							SendMessageTo( from, "That was trapped, but your magic orb disabled it.", 0x3B2 );
							return false;
						}
					}
					if ( tenfootpole != null )
					{
						TenFootPole poles = (TenFootPole)tenfootpole;
						if ( poles.Tap >= Utility.RandomMinMax( nTrapLevel, nTrapLevel2 ) )
						{
							from.PlaySound( 0x3FD );
							TrapPower = 0;
							TrapLevel = 0;
							TrapType = TrapType.None;
							poles.ConsumeLimits( 1 );
							if ( poles.Limits < 1 )
							{
								SendMessageTo( from, "You tap your ten foot pole, disabling a trap and breaking the pole.", 0x3B2 );
							}
							else
							{
								SendMessageTo( from, "You tap your ten foot pole, disabling a trap.", 0x3B2 );
								poles.InvalidateProperties();
							}
							return false;
						}
					}
				}

				int MagicAvoid = (int)(( from.Skills[SkillName.RemoveTrap].Value + from.EnergyResistance ) / 3);
				if (MagicAvoid > 90){ MagicAvoid = 90; }

				switch ( m_TrapType )
				{
					case TrapType.ExplosionTrap:
					{
						SendMessageTo( from, 502999, 0x3B2 ); // You set off a trap!

						if ( from.InRange( loc, 3 ) )
						{
							int damage = Utility.RandomMinMax( 50, 200 );
							damage = (int)( ( damage * ( 100 - from.FireResistance ) ) / 100 );
							AOS.Damage( from, damage, 0, 100, 0, 0, 0 );

							// Your skin blisters from the heat!
							from.LocalOverheadMessage( Network.MessageType.Regular, 0x2A, 503000 );
						}

						Effects.SendLocationEffect( loc, facet, 0x36BD, 15, 10 );
						Effects.PlaySound( loc, facet, 0x307 );

						break;
					}
					case TrapType.MagicTrap:
					{
						if ( from.InRange( loc, 1 ) )
						{
							int damage = Utility.RandomMinMax( 50, 200 );
							damage = (int)( ( damage * ( 100 - MagicAvoid ) ) / 100 );
							from.Damage( damage );
						}

						Effects.PlaySound( loc, Map, 0x307 );

						Effects.SendLocationEffect( new Point3D( loc.X - 1, loc.Y, loc.Z ), Map, 0x36BD, 15 );
						Effects.SendLocationEffect( new Point3D( loc.X + 1, loc.Y, loc.Z ), Map, 0x36BD, 15 );

						Effects.SendLocationEffect( new Point3D( loc.X, loc.Y - 1, loc.Z ), Map, 0x36BD, 15 );
						Effects.SendLocationEffect( new Point3D( loc.X, loc.Y + 1, loc.Z ), Map, 0x36BD, 15 );

						Effects.SendLocationEffect( new Point3D( loc.X + 1, loc.Y + 1, loc.Z + 11 ), Map, 0x36BD, 15 );

						break;
					}
					case TrapType.DartTrap:
					{
						SendMessageTo( from, 502999, 0x3B2 ); // You set off a trap!

						if ( from.InRange( loc, 3 ) )
						{
							int damage = Utility.RandomMinMax( 50, 200 );
							damage = (int)( ( damage * ( 100 - from.PhysicalResistance ) ) / 100 );
							AOS.Damage( from, damage, 100, 0, 0, 0, 0 );

							// A dart imbeds itself in your flesh!
							from.LocalOverheadMessage( Network.MessageType.Regular, 0x62, 502998 );
						}

						Effects.PlaySound( loc, facet, 0x223 );

						break;
					}
					case TrapType.PoisonTrap:
					{
						SendMessageTo( from, 502999, 0x3B2 ); // You set off a trap!

						if ( from.InRange( loc, 3 ) )
						{
							Poison poison = Poison.Lesser;

							int itHurts = from.PoisonResistance;
							int itSicks = 0;

							if ( itHurts >= 70 ){ itSicks = 1; }
							else if ( itHurts >= 50 ){ itSicks = 2; }
							else if ( itHurts >= 30 ){ itSicks = 3; }
							else if ( itHurts >= 10 ){ itSicks = 4; }
							else { itSicks = 5; }

							switch( Utility.RandomMinMax( 1, itSicks ) )
							{
								case 1: poison = Poison.Lesser;		break;
								case 2: poison = Poison.Regular;	break;
								case 3: poison = Poison.Greater;	break;
								case 4: poison = Poison.Deadly;		break;
								case 5: poison = Poison.Lethal;		break;
							}

							from.ApplyPoison( from, poison );

							// You are enveloped in a noxious green cloud!
							from.LocalOverheadMessage( Network.MessageType.Regular, 0x44, 503004 );
						}

						Effects.SendLocationEffect( loc, facet, 0x113A, 10, 20 );
						Effects.PlaySound( loc, facet, 0x231 );

						break;
					}
				}

				m_TrapType = TrapType.None;
				m_TrapPower = 0;
				m_TrapLevel = 0;
				return true;
			}

			return false;
		}

		public virtual void OnTelekinesis( Mobile from )
		{
			Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 5022 );
			Effects.PlaySound( Location, Map, 0x1F5 );

			if( this.TrapOnOpen )
			{
				ExecuteTrap( from );
			}
		}

		public override void Open( Mobile from )
		{
			if ( PassiveSearching( this, from ) )
				return;

			if ( !this.TrapOnOpen || !ExecuteTrap( from ) )
				base.Open( from );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version

			writer.Write( (int) m_TrapLevel );

			writer.Write( (int) m_TrapPower );
			writer.Write( (int) m_TrapType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
				{
					m_TrapLevel = reader.ReadInt();
					goto case 1;
				}
				case 1:
				{
					m_TrapPower = reader.ReadInt();
					goto case 0;
				}
				case 0:
				{
					m_TrapType = (TrapType)reader.ReadInt();
					break;
				}
			}
		}
	}
}