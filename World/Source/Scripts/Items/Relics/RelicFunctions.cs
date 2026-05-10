using System;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server
{
	public interface IRelic
	{
	}

	public class RelicFunctions
	{
		public static void IDItem( Mobile m, Mobile from, Item examine, SkillName skills )
		{
			BaseVendor vendor = null;

			if ( m is BaseVendor )
			{
				vendor = (BaseVendor)m;

				if ( !VendorCanID( examine.NotIDSource, m ) )
				{
					vendor.SayTo(from, "I cannot identify that.");
					return;
				}
			}

			SkillName skill = SkillName.Mercantile;
			if ( examine.NotIDSkill == IDSkill.ArmsLore )
				skill = SkillName.ArmsLore;
			else if ( examine.NotIDSkill == IDSkill.Tasting )
				skill = SkillName.Tasting;

			if ( !examine.NotIdentified ) // Decorative item
			{
				if ( !(examine is IRelic) && vendor == null && skills != SkillName.Mercantile && from.Skills[SkillName.Mercantile].Value > Utility.Random( 100 ) )
				{
					int gold = ItemInformation.GetBuysPrice( ItemInformation.GetInfo( examine.GetType() ), false, examine, false, false ) * examine.Amount;
					if ( gold > 0 )
					{
						if ( from.Skills[SkillName.Mercantile].Base < 50 && Utility.RandomBool() )
							from.CheckSkill( SkillName.Mercantile, 0, 100 );

						string estimate = "You could maybe get";
						switch ( Utility.RandomMinMax( 0, 10 ) )
						{
							case 0:	estimate = "This could perhaps bring";		break;
							case 1:	estimate = "One could want this for";		break;
							case 2:	estimate = "This may be worth";				break;
							case 3:	estimate = "Selling this would gain you";	break;
							case 4:	estimate = "This could be worth";			break;
							case 5:	estimate = "You might sell it for";			break;
							case 6:	estimate = "This could be sold for about";	break;
							case 7:	estimate = "Someone may take this for";		break;
							case 8:	estimate = "This could fetch a price of";	break;
							case 9:	estimate = "These are usually worth about";	break;
						}
						from.SendMessage( "" + estimate + " " + gold + " gold." );
						return;
					}
				}
				from.SendMessage( "That is already identified." );
				return;
			}
			else if ( vendor == null && skill != skills )
			{
				from.SendMessage( "That is not the correct skill to identify that." );
				return;
			}

			int attempts = 0;
			if ( vendor != null )
				attempts = 1;
			else if ( m is PlayerMobile )
			{
				if ( examine is NotIdentified && Item.MAX_ID_ATTEMPTS <= examine.NotIDAttempts )
					from.SendMessage( "Only a vendor can identify this item now as too many attempts were made." );
				else if ( !examine.Movable )
					from.SendMessage( "That cannot move so you cannot identify it." );
				else if ( !from.InRange( examine.GetWorldLocation(), 3 ) )
					from.SendMessage( "You will need to get closer to identify that." );
				else if ( !(examine.IsChildOf( from.Backpack )) && MySettings.S_IdentifyItemsOnlyInPack && vendor == null ) 
					from.SendMessage( "This must be in your backpack to identify." );
				else
				{
					attempts = 1;
					if ( examine is NotIdentified && ((PlayerMobile)from).Preferences.SingleAttemptID )
						attempts = Math.Max( 1, Item.MAX_ID_ATTEMPTS - examine.NotIDAttempts );
				}
			}

			for (int i = 1; i <= attempts; i++)
			{
				if ( examine.CoinPrice > 0 && examine.NotIdentified ) // Viable candidate
				{
					if ( vendor != null && examine is NotIdentified ) // Vendor IDing equipment
					{
						string var = NotIdentified.IDVirConItem( (NotIdentified)examine, from );
						vendor.SayTo( from, "That appears to be the " + var + "." );
						return;
					}

					if ( from.CheckTargetSkill( skill, examine, -5, 125 ) ) // Successful player attempt
					{
						if ( examine is NotIdentified )
						{
							string var = NotIdentified.IDVirConItem( (NotIdentified)examine, from );
							from.SendMessage( "You identify the " + var + "." );
							return;
						}
						else // Relic
						{
							examine.NotIdentified = false;

							if ( examine.NotIDSource == Identity.Armor || examine.NotIDSource == Identity.Weapon )
								from.SendMessage( "That is too old to be used in battle." );
							else if ( examine.NotIDSource == Identity.Music )
								from.SendMessage( "That is too old to be played." );

							from.SendMessage( "That is probably worth about " + examine.CoinPrice + " gold." );
							return;
						}
					}
					else if ( vendor == null ) // Failed player attempt
					{
						if ( examine is NotIdentified )
						{
							string var = NotIdentified.CannotIDVirConItem( (NotIdentified)examine, from );
							if ( i != attempts ) continue; // Only display failure if it's the final message

							from.SendMessage( var );
							continue;
						}
						else // Relic
						{
							examine.CoinPrice = Utility.RandomMinMax( 5, 25 );
							examine.NotIdentified = false;
							from.SendMessage( "You cannot really seem to fully identify it." );

							if ( examine.NotIDSource == Identity.Armor || examine.NotIDSource == Identity.Weapon )
								from.SendMessage( "That is too old to be used in battle." );
							else if ( examine.NotIDSource == Identity.Music )
								from.SendMessage( "That is too old to be played." );

							from.SendMessage( "Maybe you can get " + examine.CoinPrice + " gold for it." );
							continue;
						}
					}
				}
				else
				{
					from.SendMessage( "That item does not need to be identified." );
				}
			}
		}

		public static bool VendorDoesID( Mobile m )
		{
			if ( 
			m is Alchemist || 
			m is AlchemistGuildmaster || 
			m is ArcherGuildmaster || 
			m is Armorer || 
			m is Banker || 
			m is Bard || 
			m is BardGuildmaster || 
			m is Blacksmith || 
			m is BlacksmithGuildmaster || 
			m is Bowyer || 
			m is Carpenter || 
			m is CarpenterGuildmaster || 
			m is Enchanter || 
			m is Fighter || 
			m is Furtrader || 
			m is Garth || 
			m is Herbalist || 
			m is Jeweler || 
			m is LeatherWorker || 
			m is LibrarianGuildmaster || 
			m is Lumberjack || 
			m is Mage || 
			m is MageGuildmaster || 
			m is Merchant || 
			m is MerchantGuildmaster || 
			m is Minter || 
			m is Provisioner || 
			m is Ranger || 
			m is RangerGuildmaster || 
			m is Roscoe || 
			m is Sage || 
			m is Scribe || 
			m is Tailor || 
			m is TailorGuildmaster || 
			m is Tanner || 
			m is VarietyDealer || 
			m is WarriorGuildmaster || 
			m is Weaponsmith || 
			m is Weaver )
				return true;

			return false;
		}

		public static bool VendorCanID( Identity id, Mobile m )
		{
			if ( id == Identity.Archer )
				return ( m is Bowyer || m is Ranger || m is RangerGuildmaster || m is ArcherGuildmaster );
			else if ( id == Identity.Armor )
				return ( m is Armorer || m is Blacksmith || m is Fighter || m is Garth || m is WarriorGuildmaster || m is BlacksmithGuildmaster );
			else if ( id == Identity.Artifact )
				return ( m is Sage );
			else if ( id == Identity.Book )
				return ( m is Scribe || m is LibrarianGuildmaster );
			else if ( id == Identity.Clothing )
				return ( m is Tailor || m is Weaver || m is TailorGuildmaster );
			else if ( id == Identity.Coins )
				return ( m is Banker || m is Minter );
			else if ( id == Identity.Jewelry )
				return ( m is Jeweler );
			else if ( id == Identity.Leather )
				return ( m is Furtrader || m is LeatherWorker || m is Tanner );
			else if ( id == Identity.Magic )
				return ( m is Sage );
			else if ( id == Identity.Music )
				return ( m is Bard || m is BardGuildmaster );
			else if ( id == Identity.Potion )
				return ( m is Alchemist || m is AlchemistGuildmaster );
			else if ( id == Identity.Reagent )
				return ( m is Herbalist );
			else if ( id == Identity.Scroll )
				return ( m is Scribe || m is LibrarianGuildmaster );
			else if ( id == Identity.Wand )
				return ( m is Enchanter || m is Mage || m is Roscoe || m is MageGuildmaster );
			else if ( id == Identity.Weapon )
				return ( m is Blacksmith || m is Fighter || m is Garth || m is Weaponsmith || m is WarriorGuildmaster || m is BlacksmithGuildmaster );
			else if ( id == Identity.Wood )
				return ( m is Carpenter || m is Lumberjack || m is CarpenterGuildmaster );
			else if ( id == Identity.Merchant )
				return ( m is Provisioner || m is Merchant || m is VarietyDealer || m is MerchantGuildmaster );

			return false;
		}
	}
}