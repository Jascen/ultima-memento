using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class DDRelicArmor : Item, IRelic
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
		public DDRelicArmor() : base( 0x156C )
		{
			Weight = 40;
			CoinPrice = Utility.RandomMinMax( 80, 500 );
			NotIdentified = true;
			NotIDSource = Identity.Armor;
			NotIDSkill = IDSkill.ArmsLore;
			Hue = Utility.RandomColor(0);

			string sLook = "a rare";
			switch ( Utility.RandomMinMax( 0, 18 ) )
			{
				case 0:	sLook = "a rare";	break;
				case 1:	sLook = "a nice";	break;
				case 2:	sLook = "a pretty";	break;
				case 3:	sLook = "a superb";	break;
				case 4:	sLook = "a delightful";	break;
				case 5:	sLook = "an elegant";	break;
				case 6:	sLook = "an exquisite";	break;
				case 7:	sLook = "a fine";	break;
				case 8:	sLook = "a gorgeous";	break;
				case 9:	sLook = "a lovely";	break;
				case 10:sLook = "a magnificent";	break;
				case 11:sLook = "a marvelous";	break;
				case 12:sLook = "a splendid";	break;
				case 13:sLook = "a wonderful";	break;
				case 14:sLook = "an extraordinary";	break;
				case 15:sLook = "a strange";	break;
				case 16:sLook = "an odd";	break;
				case 17:sLook = "a unique";	break;
				case 18:sLook = "an unusual";	break;
			}
			
			string sDecon = "";
			switch ( Utility.RandomMinMax( 0, 5 ) )
			{
				case 0:	sDecon = ", decorative";		break;
				case 1:	sDecon = ", ceremonial";		break;
				case 2:	sDecon = ", ornamental";		break;
			}

			string sType = "shield";
			switch ( Utility.RandomMinMax( 0, 14 ) ) 
			{
				case 0: ItemID = 0x156C; RelicFlipID1 = 0x156C; RelicFlipID2 = 0x156D;		break;
				case 1: ItemID = 0x156E; RelicFlipID1 = 0x156E; RelicFlipID2 = 0x156F;		break;
				case 2: ItemID = 0x1570; RelicFlipID1 = 0x1570; RelicFlipID2 = 0x1571;		break;
				case 3: ItemID = 0x1572; RelicFlipID1 = 0x1572; RelicFlipID2 = 0x1573;		break;
				case 4: ItemID = 0x1574; RelicFlipID1 = 0x1574; RelicFlipID2 = 0x1575;		break;
				case 5: ItemID = 0x1576; RelicFlipID1 = 0x1576; RelicFlipID2 = 0x1577;		break;
				case 6: ItemID = 0x1578; RelicFlipID1 = 0x1578; RelicFlipID2 = 0x1579;		break;
				case 7: ItemID = 0x157A; RelicFlipID1 = 0x157A; RelicFlipID2 = 0x157B;		break;
				case 8: ItemID = 0x157C; RelicFlipID1 = 0x157C; RelicFlipID2 = 0x157D;		break;
				case 9: ItemID = 0x157E; RelicFlipID1 = 0x157E; RelicFlipID2 = 0x157F;		break;
				case 10: ItemID = 0x1580; RelicFlipID1 = 0x1580; RelicFlipID2 = 0x1581;		break;
				case 11: ItemID = 0x4228; RelicFlipID1 = 0x4228; RelicFlipID2 = 0x4229;		break;
				case 12: ItemID = 0x422A; RelicFlipID1 = 0x422A; RelicFlipID2 = 0x422C;		break;
				case 13: ItemID = 0x1508; RelicFlipID1 = 0x1508; RelicFlipID2 = 0x151C;	sType = "suit of armor";	Weight = 60; break;
				case 14: ItemID = 0x1512; RelicFlipID1 = 0x1512; RelicFlipID2 = 0x151A;	sType = "suit of armor";	Weight = 60; break;
			}

			Name = sLook + sDecon + " " + sType; 
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

		public DDRelicArmor(Serial serial) : base(serial)
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