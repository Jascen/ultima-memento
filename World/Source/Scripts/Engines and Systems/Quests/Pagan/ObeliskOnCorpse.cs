using Server;
using System;
using System.Collections;
using Server.Network;
using Server.Misc;
using Server.Mobiles;

namespace Server.Items
{
	public class ObeliskOnCorpse : Item
	{
		[Constructable]
		public ObeliskOnCorpse() : base( 0x185F )
		{
			Weight = 1.0;
			Name = "obelisk tip";
		}

		public override bool OnDragLift( Mobile from )
		{
			OnDoubleClick( from );
			return false;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)from;

				bool HasObelisk = false;

				if ( pm.IsTitanOfEther )
				{
					HasObelisk = true;
					from.SendMessage( "Being a Titan of Ether, you have no use for that." );
				}
				else
				{
					ArrayList targets = new ArrayList();
					foreach ( Item item in World.Items.Values )
					{
						if ( item is ObeliskTip )
						{
							if ( ((ObeliskTip)item).ObeliskOwner == from )
							{
								targets.Add( item );
								HasObelisk = true;
							}
						}
					}
					for ( int i = 0; i < targets.Count; ++i )
					{
						Item item = ( Item )targets[ i ];

						if ( item is ObeliskTip )
						{
							from.AddToBackpack( item );
							from.SendMessage( "Your Obelisk Tip is already in your pack." );
						}
					}
				}

				if ( !HasObelisk )
				{
					SetupObelisk( from );
					from.SendMessage( "You take possession of the blackrock Obelisk Tip!" );
					from.SendSound( 0x3D );
					LoggingFunctions.LogGeneric( from, "has found the Obelisk Tip." );
					this.Delete();
				}
			}
		}

		public void SetupObelisk( Mobile from )
		{
			ObeliskTip tip = new ObeliskTip();

			tip.ObeliskOwner = from;

			tip.HasAir = 0;
			tip.WonAir = 0;
			tip.HasFire = 0;
			tip.WonFire = 0;
			tip.HasEarth = 0;
			tip.WonEarth = 0;
			tip.HasWater = 0;
			tip.WonWater = 0;

			from.AddToBackpack( tip );
		}

		public ObeliskOnCorpse( Serial serial ) : base( serial )
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