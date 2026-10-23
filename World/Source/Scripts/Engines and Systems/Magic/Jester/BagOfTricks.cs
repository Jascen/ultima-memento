using System;
using System.Collections;
using Server.Network;
using Server.Gumps;
using Server.Mobiles;
using Server.Utilities;
using Server.Spells.Jester;
using Server.Spells;

namespace Server.Items
{
	public class BagOfTricks : Item
	{
		const int GOLD_MULTIPLIER = 10;
		const int MAX_PRANKS = 50000;

		public override bool DisplayWeight { get { return false; } }

		public int PrankPoints;

		[CommandProperty(AccessLevel.Owner)]
		public int Prank_Points { get { return PrankPoints; } set { PrankPoints = value; InvalidateProperties(); } }

		[Constructable]
		public BagOfTricks() : base( 0x1E3F )
		{
			Weight = 1.0;
			Name = "bag of tricks";
			Hue = Utility.RandomColor(0);
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) 
			{
				from.SendMessage( "This must be in your backpack to use." );
				return;
			}
			else
			{
				from.CloseGump( typeof( BagOfTricksGump ) );
				from.SendGump( new BagOfTricksGump( from ) );
				from.PlaySound( 0x48 );
			}
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
            if ( PrankPoints > 0 ){ list.Add( 1070722, "" + PrankPoints + " Prank Points"); }
        } 

		public static int GetPranks( Mobile m )
		{
			int pranks = 0;

			foreach( Item i in m.Backpack.FindItemsByType( typeof( BagOfTricks ), true ) )
			{
				BagOfTricks tricks = (BagOfTricks)i;
				pranks = pranks + tricks.PrankPoints;
			}

			return pranks;
		}

		public static void UsePranks( Mobile m, int pranks )
		{
			if (pranks < 1) return;

			ArrayList tricks = new ArrayList();
			foreach( Item item in m.Backpack.FindItemsByType( typeof( BagOfTricks ), true ) )
			{
				tricks.Add( item );
			}
			for ( int i = 0; i < tricks.Count; ++i )
			{
				BagOfTricks bag = (BagOfTricks)tricks[ i ];

				if ( pranks > 0 )
				{
					if ( bag.PrankPoints >= pranks ){ bag.PrankPoints = bag.PrankPoints - pranks; pranks = 0; bag.InvalidateProperties(); }
					else if ( pranks > bag.PrankPoints ){ pranks = pranks - bag.PrankPoints; bag.PrankPoints = 0; bag.InvalidateProperties(); }
				}
			}
		}

		public BagOfTricks( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)2 ); // version
            writer.Write( PrankPoints );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
            PrankPoints = reader.ReadInt();
			if ( version < 2 ){ PrankPoints *= GOLD_MULTIPLIER; }
		}

		public static string JesterSpeech()
		{
			return "Jesters are the entertainers of the land, juggling and leaping about, spreading humorous stories and trying to make others laugh. They beg for laughs and can use psychology on their audience to be more effective for the show. Jesters also have a mischievous side, which makes them able adventurers in their own way.<br><br>If you want to become a jester, you will need to hone your skills in begging and psychology of others. You will also need to get yourself a bag of tricks and some jester clothes to fill the role. Without these things, you will not be able to perform the comedic arts. Most jesters belong to the Thieves Guild, as they sometimes dabble in hiding, stealth, and sleight of hand. You, however, could pursue whatever secondary career you wish.<br><br>There are a few jesters around the land that can sell you such items, but the royal jester will not. These jesters can be found in taverns, ships, or hanging out with actors. As mentioned already, you must wear at least one piece of jester clothing. This can be a jester hat, jester suit, or even jester shoes. A bag of tricks will have ten individual tricks that you can perform as long as you have the mana and the prank points to perform them. You don't need much skill to perform these tricks, but they will be of little use if you do not practice both of the skills mentioned.<br><br>A jester needs gold to buy pranks to fill their bags, and they can only fill it up to 50,000 prank points before it is full. To add more gold to the bag, you must be next to a local jester or even the royal jester. Drag the gold onto the bag to fill it. The points will deplete as you use your tricks.<br><br>Double click the bag to open it and learn about the tricks you can do. In this first view, you can select an ability icon to do the ability, or you can click the button next to each icon to learn about the ability in more detail. You can also set up horizontal or vertical trick bars to use these abilities quickly from the Magic Toolbars section of [toolbars.<br><br>To have a title that is commensurate to your chosen profession, you can set your skill title to 'Beggar', which will give you the title of 'Jester'. You can also set your skill title to 'Psychology', which will give you the title of 'Joker'.<br><br>If this also interests you, jesters may have circus tents for sale. These require a plot of land already placed, and then you can place the tent on the plot of land. These are different from regular tents because they have two different colors at once. So if you have a circus tent, you can dye it one color (red for example). Then you can single click the tent and select the 'Next' option to switch it to the secondary color. Then you can dye the tent a different color (blue for example). When you place the tent, it will be in the colors of red and blue. If you want to reverse the pattern, chop the tent with an axe and then use the 'Next' option on the tent to swap the colors. Then you can build the tent on your plot of land again.<br><br>Throwing gloves are the preferred weapon for jesters, but you can use whatever weapon you wish. If you do use throwing gloves, however, you will have additional options for things to throw at your enemies. Instead of just rocks, darts, or knives...you will be able to throw tomatoes or playing cards. You would just need to set your throwing gloves to use that particular object to throw.<br><br>You may also use [Spells_Jester to see the commands to cast Jester spells";
		}

		public class BagOfTricksGump : Gump
		{
			private const int CAST_SPELL_ACTION_BASE = 10000;
			private const int DETAIL_SPELL_ACTION_BASE = 20000;

			public BagOfTricksGump( Mobile from ): base( 50, 50 )
			{
				string color = "#b3706f";
				from.SendSound( 0x4A );

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);
				
				AddImage(0, 0, 7033, Server.Misc.PlayerSettings.GetGumpHue( from ));
				AddButton(863, 11, 4017, 4017, 0, GumpButtonType.Reply, 0);
				AddHtml( 12, 12, 727, 20, @"<BODY><BASEFONT Color=" + color + ">BAG OF TRICKS</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 12, 345, 880, 249, @"<BODY><BASEFONT Color=" + color + ">" + JesterSpeech() + "</BASEFONT></BODY>", (bool)false, (bool)true);

				int k = 56;
				int x = 32;
				int y = 79;
				int b = 55;
				int index = 0;

				const int MAX_SPELLS_PER_PAGE = 5;
				foreach(var definition in JesterSpellProvider.SpellDefinitions)
				{
					// Jester spells are always known
					// if ( !HasSpell( from, definition.SpellID ) ) continue;

					AddButton(x, k, definition.IconGraphic, definition.IconGraphic, CAST_SPELL_ACTION_BASE + definition.SpellID, GumpButtonType.Reply, 0);
					AddHtml( x + 55, k + 3, 172, 20, @"<BODY><BASEFONT Color=" + color + ">" + definition.Name + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddButton(x + 55, y, 4011, 4011, DETAIL_SPELL_ACTION_BASE + definition.SpellID, GumpButtonType.Reply, 0);
					k=k+b; y=y+b;

					// Flip to the next page
					if ( ++index == MAX_SPELLS_PER_PAGE)
					{
						k=56; x=312; y=79;
					}
				}

			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				Mobile from = state.Mobile;

				if ( info.ButtonID > DETAIL_SPELL_ACTION_BASE )
				{
					int spellID = info.ButtonID - DETAIL_SPELL_ACTION_BASE;
					var definition = JesterSpellProvider.GetDefinition(spellID);
					if (definition != null)
						from.SendGump( new InfoJester( definition, from ) );
				}
				else if ( info.ButtonID > CAST_SPELL_ACTION_BASE )
				{
					int spellID = info.ButtonID - CAST_SPELL_ACTION_BASE;
					JesterSpellProvider.Cast(from, spellID);
				}

				if ( info.ButtonID > 20000 ){ from.SendSound( 0x4A ); }
				else { from.PlaySound( 0x48 ); }
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is Gold )
			{
				if ( MAX_PRANKS <= PrankPoints )
				{
					from.SendMessage( "That bag is already full of pranks." );
				}
				else if ( !WorldUtilities.HasNearbyMobile<Mobile>(from, 20, m => m is Jester || m is ChucklesJester) )
				{
					from.SendMessage( "You need to be near a local jester to add pranks!" );
				}
				else
				{
					var remaining = Math.Max( 1, (MAX_PRANKS - PrankPoints) / GOLD_MULTIPLIER );
					var added = ItemUtilities.ConsumeClamped(dropped, remaining);
					if ( 0 < added )
					{
						PrankPoints = Math.Min( MAX_PRANKS, PrankPoints + added * GOLD_MULTIPLIER );
						var message = MAX_PRANKS <= PrankPoints ? "You add some more gold for pranks and now the bag is full." : "You add some more gold for pranks.";
						from.SendMessage( message );
						from.PlaySound( 0x2E6 );
						InvalidateProperties();
					}
				}
			}

			return dropped.Deleted;
		}
	}
}

namespace Server.Gumps
{
    public class InfoJester : Gump
    {
        public InfoJester( SpellDefinition spellDefinition, Mobile from ) : base( 50, 50 )
        {
			string color = "#b3706f";
			from.SendSound( 0x4A );

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			int tile1 = Utility.RandomList( 151, 152 );
			int tile2 = Utility.RandomList( 153, 154 );
			if ( Utility.RandomBool() )
			{
				tile1 = Utility.RandomList( 153, 154 );
				tile2 = Utility.RandomList( 151, 152 );
			}

			AddPage(0);

			AddImage(0, 0, 7040, Server.Misc.PlayerSettings.GetGumpHue( from ));
			AddButton(279, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);
			AddImage(10, 9, spellDefinition.IconGraphic+10);
			AddHtml( 44, 12, 226, 20, @"<BODY><BASEFONT Color=" + color + ">" + spellDefinition.Name + "</BASEFONT></BODY>", (bool)false, (bool)false);
			AddHtml( 12, 46, 294, 214, @"<BODY><BASEFONT Color=" + color + ">" + spellDefinition.Description + "</BASEFONT></BODY>", (bool)false, (bool)false);
			AddHtml( 12, 274, 181, 20, @"<BODY><BASEFONT Color=" + color + ">Prank Points: " + spellDefinition.TithingCost + "</BASEFONT></BODY>", (bool)false, (bool)false);
			AddHtml( 205, 274, 100, 20, @"<BODY><BASEFONT Color=" + color + "><RIGHT>Mana: " + spellDefinition.ManaCost + "</RIGHT></BASEFONT></BODY>", (bool)false, (bool)false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
			from.SendSound( 0x4A );
			from.CloseGump( typeof( Server.Items.BagOfTricks.BagOfTricksGump ) );
			from.CloseGump( typeof( InfoJester ) );
			from.SendGump( new Server.Items.BagOfTricks.BagOfTricksGump( from ) );
        }
    }
}