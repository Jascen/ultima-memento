using System;
using Server.Mobiles;
using Server.Accounting;

namespace Server.Items
{
	public class Gold : Item
	{
		public override double DefaultWeight
		{
			get { return ( Core.ML ? ( 0.02 / 3 ) : 0.02 ); }
		}

		[Constructable]
		public Gold() : this( 1 )
		{
		}

		[Constructable]
		public Gold( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable]
		public Gold( int amount ) : base( 0xEED )
		{
			Name = "gold coins";
			Stackable = true;
			Amount = amount;
			Light = LightType.Circle150;
		}

		public Gold( Serial serial ) : base( serial )
		{
		}

		public override int GetDropSound()
		{
			if ( Amount <= 1 )
				return 0x2E4;
			else if ( Amount <= 5 )
				return 0x2E5;
			else
				return 0x2E6;
		}

		protected override void OnAmountChange( int oldValue )
		{
			int newValue = this.Amount;

			UpdateTotal( this, TotalType.Gold, newValue - oldValue );
		}

		public override void OnDoubleClick( Mobile from )
		{
			var player = from as PlayerMobile;
			BankBox box = player.FindBankNoCreate();
			if (player != null && box != null && IsChildOf( box ))
			{		
				player.AccountGold += this.Amount;
				this.Delete();
				player.SendMessage( this.Amount + " Gold had been move to your Account Wallet balance.");

			}
			else
			{
				player.SendLocalizedMessage( 1047026 );
			}
		}

		public override int GetTotal( TotalType type )
		{
			int baseTotal = base.GetTotal( type );

			if ( type == TotalType.Gold )
				baseTotal += this.Amount;

			return baseTotal;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			Light = LightType.Circle150;
			Name = "gold coins"; 
		}
	}
}
