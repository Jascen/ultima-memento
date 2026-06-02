using System;
using Server;
using Server.Utilities;

namespace Server.Items
{
	public class Crystals : Item
	{
		public override double DefaultWeight
		{
			get { return ( Core.ML ? ( 0.02 / 3 ) : 0.02 ); }
		}

		[Constructable]
		public Crystals() : this( 1 )
		{
		}

		[Constructable]
		public Crystals( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable]
		public Crystals( int amount ) : base( 0x1958 )
		{
			Stackable = true;
			Amount = amount;
			Name = "crystals";
			Light = LightType.Circle150;
		}

		public override void OnDoubleClick( Mobile from )
		{
			BankBox box = from.FindBankNoCreate();

			if ( box != null && IsChildOf( box ) )
			{
				Delete();
				int nGold = this.Amount * 5;
				if ( nGold > 0 )
				{
					foreach (var gold in ItemUtilities.AddStacks(nGold, () => new Gold()))
					{
						if (!box.TryStackItem(gold, from))
						{
							box.DropItem(gold);
						}
					}
				}
			}
			else
			{
				from.SendLocalizedMessage( 1047026 ); // That must be in your bank box to use it.
			}
		}

		public Crystals( Serial serial ) : base( serial )
		{
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
		}
	}
}