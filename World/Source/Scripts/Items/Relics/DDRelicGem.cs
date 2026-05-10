using System;
using Server;
using System.Collections;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class DDRelicGem : Item, IRelic
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

		public string RelicCameFrom;
		
		[CommandProperty(AccessLevel.Owner)]
		public string Relic_CameFrom { get { return RelicCameFrom; } set { RelicCameFrom = value; InvalidateProperties(); } }

		[Constructable]
		public DDRelicGem() : base( 0x3192 )
		{
			switch ( Utility.RandomMinMax( 0, 2 ) )
			{
				case 0:	Hue = Utility.RandomList( 0x47E, 0x47F, 0x480, 0x481, 0x482, 0xB93, 0xB94, 0xB95, 0xB96, 0xB83, 0x48D, 0x48E, 0x48F, 0x490, 0x491, 0x492, 0x489, 0x495, 0x496, 0x499 );			
					break;
				case 1:	Hue = Utility.RandomMinMax( 0x9C5, 0xA54 );			
					break;
				case 2:	Hue = Utility.RandomMinMax( 0xA5B, 0xA66 );			
					break;
			}

			ItemID = Utility.RandomList( 0xF21, 0xF10, 0xF19, 0xF13, 0xF15, 0xF16, 0xF2D, 0xF25, 0xF26 );

			Weight = 1.0;
			CoinPrice = Utility.RandomMinMax( 80, 500 );
			NotIdentified = true;
			NotIDSource = Identity.Jewelry;
			NotIDSkill = IDSkill.Mercantile;
			Light = LightType.Circle150;

			string sLook = "a rare";
			switch ( Utility.RandomMinMax( 0, 24 ) )
			{
				case 0:	sLook = "a rare";			break;
				case 1:	sLook = "a nice";			break;
				case 2:	sLook = "a pretty";			break;
				case 3:	sLook = "a superb";			break;
				case 4:	sLook = "a delightful";		break;
				case 5:	sLook = "an elegant";		break;
				case 6:	sLook = "an exquisite";		break;
				case 7:	sLook = "a fine";			break;
				case 8:	sLook = "a gorgeous";		break;
				case 9:	sLook = "a lovely";			break;
				case 10:sLook = "a magnificent";	break;
				case 11:sLook = "a marvelous";		break;
				case 12:sLook = "a splendid";		break;
				case 13:sLook = "a wonderful";		break;
				case 14:sLook = "an extraordinary";	break;
				case 15:sLook = "a strange";		break;
				case 16:sLook = "an odd";			break;
				case 17:sLook = "a unique";			break;
				case 18:sLook = "an unusual";		break;
				case 19:sLook = "a brilliant";		break;
				case 20:sLook = "a clear";			break;
				case 21:sLook = "a lustrous";		break;
				case 22:sLook = "a radiant";		break;
				case 23:sLook = "a shining";		break;
				case 24:sLook = "a precious";		break;
			}

			string sRock = "gem";
			switch ( Utility.RandomMinMax( 0, 6 ) )
			{
				case 0:	sRock = "gem";				break;
				case 1:	sRock = "jewel";			break;
				case 2:	sRock = "stone";			break;
				case 3:	sRock = "gemstone";			break;
				case 4:	sRock = "birthstone";		break;
				case 5:	sRock = "crystal";			break;
				case 6:	sRock = "shard";			break;
			}

			string sGift = "Belonged to";
			switch ( Utility.RandomMinMax( 0, 7 ) )
			{
				case 0:	sGift = "Belonged to";		break;
				case 1:	sGift = "Gifted to";		break;
				case 2:	sGift = "Stolen from";		break;
				case 3:	sGift = "Found in";			break;
				case 4:	sGift = "Lost in";			break;
				case 5:	sGift = "Taken from";		break;
				case 6:	sGift = "Gifted from";		break;
				case 7:	sGift = "Missing from";		break;
			}

			string sGift2 = "Belonged to";
			switch ( Utility.RandomMinMax( 0, 4 ) )
			{
				case 0:	sGift2 = "Stolen from";		break;
				case 1:	sGift2 = "Belonged to";		break;
				case 2:	sGift2 = "Lost by";			break;
				case 3:	sGift2 = "Taken from";		break;
				case 4:	sGift2 = "Missing from";	break;
			}

			string sEye = "Eye";
			switch ( Utility.RandomMinMax( 0, 4 ) )
			{
				case 0:	sEye = "Eye";		break;
				case 1:	sEye = "Crystal";	break;
				case 2:	sEye = "Stone";		break;
				case 3:	sEye = "Shard";		break;
				case 4:	sEye = "Gem";		break;
			}

			Name = sLook + " " + sRock;

			switch( Utility.RandomMinMax( 1, 8 ) )
			{
				case 1: RelicCameFrom = sGift + " the " + Server.Misc.RandomThings.GetRandomKingdomName() + " " + Server.Misc.RandomThings.GetRandomKingdom(); break;
				case 2: RelicCameFrom = sGift + " the " + Server.Misc.RandomThings.GetRandomKingdom() + " of " + Server.Misc.RandomThings.GetRandomKingdomName(); break;
				case 3: RelicCameFrom = sGift2 + " the " + Server.Misc.RandomThings.GetRandomNoble() + " of " + Server.Misc.RandomThings.GetRandomKingdomName(); break;
				case 4: RelicCameFrom = sGift2 + " the " + Server.Misc.RandomThings.GetRandomNoble() + " of the " + Server.Misc.RandomThings.GetRandomKingdomName() + " " + Server.Misc.RandomThings.GetRandomKingdom(); break;
				case 5: RelicCameFrom = sGift2 + " " + Server.Misc.RandomThings.GetRandomSociety(); break;
				case 6: RelicCameFrom = sGift2 + " the Ship called " + Server.Misc.RandomThings.GetRandomShipName( "", 0 ); break;
				case 7: RelicCameFrom = sGift2 + " " + Server.Misc.RandomThings.GetRandomScenePainting(); break;
				case 8: RelicCameFrom = sEye + " of the " + Server.Misc.RandomThings.GetRandomCreature(); break;
			}
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1049644, RelicCameFrom );
        }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) && MySettings.S_IdentifyItemsOnlyInPack && from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified ) 
				from.SendMessage( "This must be in your backpack to identify." );
			else if ( from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && NotIdentified )
				IDCommand( from );
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

		public DDRelicGem(Serial serial) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
            writer.Write( (int) 1 ); // version
            writer.Write( RelicCameFrom );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
            int version = reader.ReadInt();

			if ( version < 1 )
				CoinPrice = reader.ReadInt();

			RelicCameFrom = reader.ReadString();
		}
	}
}