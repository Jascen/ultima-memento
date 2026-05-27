using System;
using Server.Utilities;

namespace Server.Items
{
	public class DDCopper : Item
	{
		public override void ItemIdentified( bool id )
		{
			m_NotIdentified = id;
			if ( !id )
			{
				ColorHue3 = "FDC844";
				ColorText3 = "Worth " + CoinPrice + " Gold";
			}
		}

		public override double DefaultWeight
		{
			get { return ( Core.ML ? ( 0.02 / 3 ) : 0.02 ); }
		}

		[Constructable]
		public DDCopper() : this( 1 )
		{
		}

		[Constructable]
		public DDCopper( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable]
		public DDCopper( int amount ) : base( 0xEF0 )
		{
			Name = "copper coins";
			Hue = 0x68F;
			Stackable = true;
			Amount = amount;
			Light = LightType.Circle150;
		}

		public DDCopper( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			BankBox box = from.FindBankNoCreate();

			if ( box != null && IsChildOf( box ) )
			{
				int nRate = 10;

				int nCoins = this.Amount;
				int nGold = (int)Math.Floor((decimal)(this.Amount / nRate));
				int nChange = this.Amount - ( nGold * nRate );

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

				if (nChange < 1)
					Delete();
				else if (nCoins != nChange)
					Amount = nChange;
			}
			else
			{
				from.SendLocalizedMessage( 1047026 ); // That must be in your bank box to use it.
			}
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
			Name = "copper coins";
			Hue = 0x68F;
		}
	}

	// -----------------------------------------------------------------------------------------------------------------------

	public class DDSilver : Item
	{
		public override double DefaultWeight
		{
			get { return ( Core.ML ? ( 0.02 / 3 ) : 0.02 ); }
		}

		[Constructable]
		public DDSilver() : this( 1 )
		{
		}

		[Constructable]
		public DDSilver( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable]
		public DDSilver( int amount ) : base( 0xEF0 )
		{
			Name = "silver coins";
			Stackable = true;
			Amount = amount;
			Hue = 0xB2A;
			Light = LightType.Circle150;
		}

		public DDSilver( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			BankBox box = from.FindBankNoCreate();

			if ( box != null && IsChildOf( box ) )
			{
				int nRate = 5;

				int nCoins = this.Amount;
				int nGold = (int)Math.Floor((decimal)(this.Amount / nRate));
				int nChange = this.Amount - ( nGold * nRate );

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

				if (nChange < 1)
					Delete();
				else if (nCoins != nChange)
					Amount = nChange;
			}
			else
			{
				from.SendLocalizedMessage( 1047026 ); // That must be in your bank box to use it.
			}
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
			Name = "silver coins";
			Hue = 0xB2A;
			Light = LightType.Circle150;
		}
	}

	// -----------------------------------------------------------------------------------------------------------------------

	public class DDJewels : Item
	{
		public override double DefaultWeight
		{
			get { return ( Core.ML ? ( 0.02 / 3 ) : 0.02 ); }
		}

		[Constructable]
		public DDJewels() : this( 1 )
		{
		}

		[Constructable]
		public DDJewels( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable]
		public DDJewels( int amount ) : base( 0xEF0 )
		{
			Name = "jewels";
			Stackable = true;
			Amount = amount;
			Light = LightType.Circle150;
		}

		public DDJewels( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			BankBox box = from.FindBankNoCreate();

			if ( box != null && IsChildOf( box ) )
			{
				Delete();
				int nGold = this.Amount * 2;
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

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	// -----------------------------------------------------------------------------------------------------------------------

	public class DDXormite : Item
	{
		public override double DefaultWeight
		{
			get { return ( Core.ML ? ( 0.02 / 3 ) : 0.02 ); }
		}

		[Constructable]
		public DDXormite() : this( 1 )
		{
		}

		[Constructable]
		public DDXormite( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable]
		public DDXormite( int amount ) : base( 0xEF0 )
		{
			Name = "xormite coins";
			Stackable = true;
			Amount = amount;
			Hue = 0xB96;
			Light = LightType.Circle150;
		}

		public DDXormite( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			BankBox box = from.FindBankNoCreate();

			if ( box != null && IsChildOf( box ) )
			{
				Delete();
				int nGold = this.Amount * 3;
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

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	// -----------------------------------------------------------------------------------------------------------------------

	public class DDGemstones : Item
	{
		public override double DefaultWeight
		{
			get { return ( Core.ML ? ( 0.02 / 3 ) : 0.02 ); }
		}

		[Constructable]
		public DDGemstones() : this( 1 )
		{
		}

		[Constructable]
		public DDGemstones( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable]
		public DDGemstones( int amount ) : base( 0xE99 )
		{
			Name = "gemstones";
			Stackable = true;
			Amount = amount;
			Light = LightType.Circle150;
		}

		public DDGemstones( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			BankBox box = from.FindBankNoCreate();

			if ( box != null && IsChildOf( box ) )
			{
				Delete();
				int nGold = this.Amount * 2;
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

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	// -----------------------------------------------------------------------------------------------------------------------

	public class DDGoldNuggets : Item
	{
		public override double DefaultWeight
		{
			get { return ( Core.ML ? ( 0.02 / 3 ) : 0.02 ); }
		}

		[Constructable]
		public DDGoldNuggets() : this( 1 )
		{
		}

		[Constructable]
		public DDGoldNuggets( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable]
		public DDGoldNuggets( int amount ) : base( 0x1BC8 )
		{
			Name = "gold nuggets";
			Stackable = true;
			Amount = amount;
			Light = LightType.Circle150;
		}

		public DDGoldNuggets( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			BankBox box = from.FindBankNoCreate();

			if ( box != null && IsChildOf( box ) )
			{
				Delete();
				int nGold = this.Amount;
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

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
} 