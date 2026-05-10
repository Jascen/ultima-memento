using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class DDRelicCoins : Item, IRelic
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

		public int RelicFlipID1;
		public int RelicFlipID2;

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_FlipID1 { get { return RelicFlipID1; } set { RelicFlipID1 = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Relic_FlipID2 { get { return RelicFlipID2; } set { RelicFlipID2 = value; InvalidateProperties(); } }

		[Constructable]
		public DDRelicCoins() : base( 0xE1A )
		{
			Weight = 5;
			CoinPrice = Utility.RandomMinMax( 80, 500 );
			NotIdentified = true;
			NotIDSource = Identity.Coins;
			NotIDSkill = IDSkill.Mercantile;

			string sValue = "";
			string sAge = "";

			Hue = Utility.RandomColor(0);

			switch ( Utility.RandomMinMax( 0, 1 ) ) 
			{
				case 0: ItemID = 0xE1A; RelicFlipID1 = 0xE1A; RelicFlipID2 = 0xFA4; break;
				case 1: ItemID = 0xE1B; RelicFlipID1 = 0xE1B; RelicFlipID2 = 0xFA5; break;
			}

			switch ( Utility.RandomMinMax( 0, 13 ) )
			{
				case 0:	sAge = "from a long dead civilization";		break;
				case 1:	sAge = "from an ancient race";				break;
				case 2:	sAge = "of a secret order";					break;
				case 3:	sAge = "of a far off land";					break;
				case 4:	sAge = "of unknown origin";					break;
				case 5:	sAge = "from long ago";						break;
				case 6:	sAge = "from a lost city";					break;
				case 7:	sAge = "from a mysterious land";			break;
				case 8:	sAge = "from the dark times";				break;
				case 9:	sAge = "of an old race";					break;
				case 10:sAge = "of a lost race";					break;
				case 11:sAge = "from a missing land";				break;
				case 12:sAge = "from an unknown era";				break;
				case 13:sAge = "used centuries ago";				break;
			}

			switch ( Utility.RandomMinMax( 0, 33 ) )
			{
				case 0:	sValue = "tower";	break;
				case 1: sValue = "griffin";	break;
				case 2:	sValue = "crown";	break;
				case 3:	sValue = "sword";	break;
				case 4:	sValue = "axe";		break;
				case 5:	sValue = "lion";	break;
				case 6:	sValue = "bear";	break;
				case 7:	sValue = "bat";		break;
				case 8:	sValue = "boar";	break;
				case 9:	sValue = "buffalo";	break;
				case 10:sValue = "chimera";	break;
				case 11:sValue = "snake";	break;
				case 12:sValue = "demon";	break;
				case 13:sValue = "devil";	break;
				case 14:sValue = "angel";	break;
				case 15:sValue = "dragon";	break;
				case 16:sValue = "dog";		break;
				case 17:sValue = "eagle";	break;
				case 18:sValue = "hawk";	break;
				case 19:sValue = "hippogriff";	break;
				case 20:sValue = "horse";	break;
				case 21:sValue = "wolf";	break;
				case 22:sValue = "pegasus";	break;
				case 23:sValue = "ram";		break;
				case 24:sValue = "skull";	break;
				case 25:sValue = "spider";	break;
				case 26:sValue = "unicorn";	break;
				case 27:sValue = "scorpion";	break;
				case 28:sValue = "hand";	break;
				case 29:sValue = "fist";	break;
				case 30:sValue = "eye";		break;
				case 31:sValue = "cross";	break;
				case 32:sValue = "woman";	break;
				case 33:sValue = "man";		break;
			}

			Name = "coins " + sAge + " with symbols of a " + sValue + " on them";
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) && MySettings.S_IdentifyItemsOnlyInPack && from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified ) 
				from.SendMessage( "This must be in your backpack to identify." );
			else if ( from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified )
				IDCommand( from );
			else if ( !IsChildOf( from.Backpack ) )
				from.SendMessage( "This must be in your backpack to flip." );
			else
				if ( this.ItemID == RelicFlipID1 ){ this.ItemID = RelicFlipID2; } else { this.ItemID = RelicFlipID1; }
		}

		public override void IDCommand( Mobile m )
		{
			if ( this.NotIDSkill == IDSkill.Tasting )
				RelicFunctions.IDItem( m, m, this, SkillName.Tasting );
			else if ( this.NotIDSkill == IDSkill.ArmsLore )
				RelicFunctions.IDItem( m, m, this, SkillName.ArmsLore );
			else
				RelicFunctions.IDItem( m, m, this, SkillName.Mercantile );
		}

		public DDRelicCoins(Serial serial) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
            writer.Write( (int) 1 ); // version
            writer.Write( RelicFlipID1 );
            writer.Write( RelicFlipID2 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
            int version = reader.ReadInt();

			if ( version < 1 )
				CoinPrice = reader.ReadInt();

            RelicFlipID1 = reader.ReadInt();
            RelicFlipID2 = reader.ReadInt();
		}
	}
}