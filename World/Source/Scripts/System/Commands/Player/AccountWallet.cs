using System;
using Server;
using Server.Items;
using System.Text;
using Server.Mobiles;
using Server.Network;
using Server.Commands;
using Server.Accounting;

namespace Server.Scripts.Commands
{
	public class AccountWalletCommands
	{
		public static void Initialize()
		{
			Properties.Initialize();
				Register( "WalletBalance", AccessLevel.Player, new CommandEventHandler( WalletBalance_OnCommand ) );
				Register( "WalletWithdraw", AccessLevel.Player, new CommandEventHandler( WalletWithdraw_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
			CommandSystem.Register(command, access, handler);
		}

		[Usage( "WalletBalance" )]
		[Description( "Account Wallet Gold balance." )]
		public static void WalletBalance_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			var player = from as PlayerMobile;
			player.SendMessage("Your Account Wallet balance is: " + player.AccountGold);
		}

		[Usage( "WalletWithdraw" )]
		[Description( "Withdraw Gold from your Account Wallet.")]
		public static void WalletWithdraw_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if (e.Arguments.Length != 1)
			{
				e.Mobile.SendMessage("[WalletWithdraw amount ");
			}
			
			var player = from as PlayerMobile;
			int balance = player.AccountGold;
			var amount = e.GetInt32(0);
			if ( balance < amount)
			{	
				player.SendMessage("You do not have " + amount + " gold to withdraw.");
			}
			else if (amount > 1000000)
			{
				player.SendMessage("You can only withdraw a maximum of 1,000,000 gold at a time.");
			}
			else
			{
				if (amount > 60000)
				{
					player.AccountGold -= amount;
					player.BankBox.DropItem( new BankCheck( amount ) );
					player.SendMessage("A Bank Check for " + amount + " has been placed in your Bank Box");
					player.SendMessage("Your new balance is " + player.AccountGold);
				}
				else
				{
					player.AccountGold -= amount;
					player.BankBox.DropItem( new Gold( amount ) );
					player.SendMessage("You have withdrawn " + amount + " Gold to your Bank Box");
					player.SendMessage("Your new balance is " + player.AccountGold);
				}
			}
			
		}
	}
}
