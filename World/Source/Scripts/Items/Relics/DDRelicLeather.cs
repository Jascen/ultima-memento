using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class DDRelicLeather : Item, IScissorable, IRelic
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

		[Constructable]
		public DDRelicLeather() : base( 0x106B )
		{
			Weight = 40;
			CoinPrice = Utility.RandomMinMax( 80, 500 );
			NotIdentified = true;
			NotIDSource = Identity.Leather;
			NotIDSkill = IDSkill.Mercantile;

			ItemID = Utility.RandomList( 0x106B, 0x106A, 0x1069, 0x107C, 0x107B, 0x107A, 0x1079, 0x1078 );

			Hue = Utility.RandomMinMax( 2401, 2430 );

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

			string sType = "animal";
			switch ( Utility.RandomMinMax( 0, 20 ) ) 
			{
				case 0:	sType = "deer";	break;
				case 1:	sType = "wolf";	break;
				case 2:	sType = "dinosaur";	break;
				case 3:	sType = "dragon";	break;
				case 4:	sType = "crocodile";	break;
				case 5:	sType = "lizard";	break;
				case 6:	sType = "serpent";	break;
				case 7:	sType = "bear";	break;
				case 8:	sType = "lion";	break;
				case 9:	sType = "mammoth";	break;
				case 10:sType = "manticore";	break;
				case 11:sType = "rhinoceros";	break;
				case 12:sType = "sabretooth";	break;
				case 13:sType = "basilisk";	break;
				case 14:sType = "gargoyle";	break;
				case 15:sType = "unicorn";	break;
				case 16:sType = "pegasus";	break;
				case 17:sType = "demon";	break;
				case 18:sType = "griffin";	break;
				case 19:sType = "alligator";	break;
				case 20:sType = "snake";	break;
			}
			if ( (ItemID == 0x1079) || (ItemID == 0x1078) ){ Name = sLook + " bundle of " + sType + " suede"; }
			else { Name = sLook + " stretched hide of " + sType + " suede"; }
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

		public DDRelicLeather(Serial serial) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
            writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
            int version = reader.ReadInt();

			if ( version < 1 )
				CoinPrice = reader.ReadInt();
		}
		
		public bool Scissor( Mobile from, Scissors scissors )
		{
			if ( Deleted || !from.CanSee( this ) ) return false;

			base.ScissorHelper( from, new Leather(), 10 );

			return true;
		}
	}
}