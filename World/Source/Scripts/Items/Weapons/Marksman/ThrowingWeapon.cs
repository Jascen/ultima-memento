using Server.Mobiles;

namespace Server.Items
{
	public enum ThrowingWeaponType
	{
		Stones,
		Axes,
		Daggers,
		Darts,
		Stars,
		Cards,
		Tomatoes,
	}

	public class ThrowingWeapon : Item
	{
		public ThrowingWeaponType m_ammo;

		[CommandProperty(AccessLevel.GameMaster)]
		public ThrowingWeaponType Ammo
		{
			get { return m_ammo; }
			set 
			{
				m_ammo = value;
				switch( m_ammo )
				{
					default:
					case ThrowingWeaponType.Stones: ItemID = 0x10B6; Name = "throwing stone"; break;
					case ThrowingWeaponType.Axes: ItemID = 0x10B3; Name = "throwing axe"; break;
					case ThrowingWeaponType.Daggers: ItemID = 0x10B7; Name = "throwing dagger"; break;
					case ThrowingWeaponType.Darts: ItemID = 0x10B5; Name = "throwing dart"; break;
					case ThrowingWeaponType.Stars: ItemID = 0x10B2; Name = "throwing star"; break;
					case ThrowingWeaponType.Cards: ItemID = 0x4C29; Name = "throwing card"; break;
					case ThrowingWeaponType.Tomatoes: ItemID = 0x4C28; Name = "throwing tomato"; break;
				}
				
				InvalidateProperties();
			}
		}

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		[Constructable]
		public ThrowingWeapon() : this( 1 )
		{
		}

		[Constructable]
		public ThrowingWeapon( int amount ) : base( 0x10B2 )
		{
			switch ( Utility.RandomMinMax( 0, 4 ) ) 
			{
				case 0: Ammo = ThrowingWeaponType.Axes; break;
				case 1: Ammo = ThrowingWeaponType.Daggers; break;
				case 2: Ammo = ThrowingWeaponType.Darts; break;
				case 3: Ammo = ThrowingWeaponType.Stars; break;
				case 4: Ammo = ThrowingWeaponType.Stones; break;
			}
			Stackable = true;
			Amount = amount;
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( m is PlayerMobile && m.Alive && Movable )
			{
				m.PlaceInBackpack( this );
			}
			return true;
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			list.Add( 1049644, "Double click to change ammo from " + Name );
			list.Add( 1070722, "Can Be Used With Throwing Gloves" );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) 
			{
				from.SendMessage( "This must be in your backpack to change the ammo type." );
				return;
			}
			else
			{
				Ammo = ThrowingGloves.NextWeaponType( from, m_ammo );
				from.SendMessage(68, "You have changed the ammo to " + Name + ".");
			}
		}

		public ThrowingWeapon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
            writer.Write( (int)m_ammo );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			if ( 0 < version ){ m_ammo = (ThrowingWeaponType)reader.ReadInt(); }
			else
			{
            	var _ = reader.ReadString(); // We don't care about converting this. Just default them to Stones.
			}
		}
	}
}