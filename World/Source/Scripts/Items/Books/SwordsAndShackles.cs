using Server.Network;
using Server.Gumps;

namespace Server.Items
{
	public class SwordsAndShackles : Item, IShowableBook
	{
		[Constructable]
		public SwordsAndShackles() : base( 0x529D )
		{
			Weight = 1.0;
			Hue = 0x944;
			ItemID = Utility.RandomList( 0x529D, 0x529E );
			Name = "Skulls and Shackles";
		}

		public void ShowBookGump( Mobile from )
		{
			from.CloseGump( typeof( SwordsAndShacklesGump ) );
			from.SendGump( new SwordsAndShacklesGump( from, 1 ) );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.InRange(GetWorldLocation(), 2 ) )
			{
				from.SendMessage("That is too far away to read.");
				return;
			}

			ShowBookGump( from );
			Server.Gumps.MyLibrary.readBook( this, from );
		}

		public SwordsAndShackles( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public class SwordsAndShacklesGump : Gump
		{
			private int m_Page;

			public SwordsAndShacklesGump( Mobile from, int page ): base( 50, 50 )
			{
				from.SendSound( 0x55 );
				string color = "#76b4d4";
				m_Page = page;

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				int max = 11; // MAX PAGES
				int page1 = m_Page-1; if ( page1 < 1 ){ page1 = max; }
				int page2 = m_Page+1; if ( page2 > max ){ page2 = 1; }

				AddImage(0, 0, 7010, 2878);
				AddImage(0, 0, 7011);
				AddImage(0, 0, 7025, 2736);
				AddButton(110, 67, 4014, 4014, page1, GumpButtonType.Reply, 0);
				AddButton(906, 70, 4005, 4005, page2, GumpButtonType.Reply, 0);

				AddHtml( 596, 72, 299, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>SKULLS & SHACKLES</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);

				if ( m_Page == 1 )
				{
					AddHtml( 151, 72, 299, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>THE BASICS OF FISHING</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 116, 107, 393, 486, @"<BODY><BASEFONT Color=" + color + ">Fishing is the patient art of luring fish toward your lure in the pursuit of feeding yourself. In its basic form you would acquire a fishing pole and head toward the shore to see what you can catch. Fishing poles can be crafted by carpenters or purchased from fisherman. A fishing pole can only be used a set number of times and it will eventually break from over use. Simply hold the fishing pole, use it, and then select a nearby spot in the water to fish in. During this initial practice, you will catch some fish or pull up soggy clothes or rusty armor. It may be morbid, but you might even pull up the remains of a drowning victim or a bag someone may have dropped in the water years ago.<br><br>You cannot practice on the shore forever if you plan to become even more proficient. After you reach the level of apprentice (50) in seafaring, you will have to acquire a ship and sail away from the safety of shore. Here you will catch more types of things, but also risk fishing up sea serpents. If you face such a beast, and claim victory over it, you may find special items. If you happen to catch a glimpse of a sunken ship below the surface of the waves, you may be able to pull up some decorative treasure from the wreckage below.<br><br>You will likely come into some unusually exotic fish out at sea. Although you are free to slice these up and cook them, they are often more valuable due to how rare they are. If you want to earn some gold for these types of fish, simply find a dock that has a fish tub and place the fish within it. You will be awarded an amount of gold that can be increased based on your seafaring skill.<br><br>Magic fish are often caught by expert fisherman, and you can simply eat these raw and have a temporary increase in strength, dexterity, or intelligence. Unusual seaweed may get caught on your line, and they should not simply be tossed away. Look these plants over because they commonly have alchemical properties that could be of great use when far from land. You will need empty bottles, and then you could use the seaweed to attempt and squeeze the fluids from them to create potions. As previously mentioned, you could likely pull up rusty armor and weapons. Although useless to adventurers, these rusty items can be melted down and reused by iron workers and blacksmiths in town. Simply bring the items to their shops and place them into scrap iron barrels to then acquire the gold. The payment for such items are a gold coin per stone weight of the rusty item.<br><br>Those that choose to be known by this profession are either known as 'sailors' or 'pirates', and it depends on your karma which one you are. Those of barbaric backgrounds will have titles of 'Atlantean' as that is their sea worthy heritage. Grandmasters of this skill are given the additional title of 'captain'.</BASEFONT></BODY>", (bool)false, (bool)true);
					AddImage(594, 180, 10887);
				}
				else if ( m_Page == 2 )
				{
					AddHtml( 151, 72, 299, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>THROWING HARPOONS</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 116, 107, 393, 486, @"<BODY><BASEFONT Color=" + color + ">Harpoons are sometimes the weapon of choice for sailors of the high seas. using the marksmanship skill, one can throw a spear like weapon toward their foe. Although sailing merchants sell such weapons, blacksmiths are able to construct them. In order to use a harpoon, one must be able to throw it and then pull it back to themselves so they can throw it again. In order to accomplish this, one would need a good supply of harpoon rope. This style of rope is inexpensive and also commonly sold by sailor merchants. Tailors are able to weave such rope as well. Whenever you throw a harpoon, these ropes usually are expended so be sure to bring a good supply with you on your journey if you use such a weapon. The better your seafaring skill, the more effective you can be with such weapons as well.</BASEFONT></BODY>", (bool)false, (bool)false);
					AddImage(594, 180, 10887);
				}
				else if ( m_Page == 3 )
				{
					AddHtml( 151, 72, 299, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>YOUR OWN SHIP</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 116, 107, 393, 486, @"<BODY><BASEFONT Color=" + color + ">When you have earned enough gold from your various efforts, you may be able to own your very own ship. Shipwrights often sell ships, where the larger the ship the larger the hold for which you can store your cargo. To launch a ship, or put it in dry dock, you most commonly need to be at a dock to do so. There are exceptions to this as there are a small amount of islands that allow you to launch and dock your ship. Sea captains do not suffer from this limitation as they may launch and dock their ships from any shore. Sailing merchants sometimes sell docking lanterns, which are especially bright lanterns for ships to dock. These can only be used from your home and they allow you to build a sea side home to dock and launch your ship if you have not achieved that level of sailing skill. When you launch your ship you are given a key for you pack and your bank. You can use this to secure your boat or cast recalling teleport magic on it to return to your ship from afar. If you are nowhere near your ship, and you have no such magic, you can give your key to a shipwright and they will transport you to your vessel. This will cost you 1,000 gold of course.<br><br>To pilot your ship, simply be on board and double click the tiller man. A steering mechanism will appear and you can then sail the seas. This mechanism also allows you to rename your vessel and raise or drop the anchor. You must be on board when using this mechanism. The center of the mechanism is transparent so you can position it over your radar map if you choose. The mechanism allows you to size it to match the two style of radar maps to overlay.<br><br>Masters in the seafaring skill will have an additional feature to their ships that other sailors do not, a lower deck. This is a public area below your ship that has comforts such as a tavern, provisioner, bank, and healer. As long as you are not in combat on your ship, you can go below deck and relax. If your ship is commanded to sail, and you go below deck, your ship will sail onward until it is stopped by an obstacle. If your real world comrades are below deck, and you dock your ship, your comrades will appear on the land where you docked your ship.</BASEFONT></BODY>", (bool)false, (bool)true);
					AddImage(638, 180, 10890);
				}
				else if ( m_Page == 4 )
				{
					AddHtml( 151, 72, 299, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>SAILING YOUR SHIP</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
					AddImage(279, 305, 10921);
					AddHtml( 126, 102, 801, 203, @"<BODY><BASEFONT Color=" + color + ">After you launch your ship into the sea, it is time to climb on board and sail away. While on board, double click the tillerman and a window will appear with a steering wheel. The gold buttons on the wheel itself will move your vessel in that direction. The red button in the very center will stop your ship. There are additional buttons and their functions are labeled on the left. These functions drop or raise your anchor, name your vessel, turn left or right, come about, or set your boat to move one step at a time (use the same button to disable the one step to move normally). You can also use maps to plot a course to follow. You can only do this on maps drawn by other characters, and you cannot do this on the really large world maps. Open the map and choose the course plotting option at the top. Pick the various path points on the map, while ensuring no land masses will get in your way. Once you are done, click the top of the map to indicate you are done plotting the course. You can also clear the plotted course with the option on the bottom. If you are satisfied with the plotted course, hand the map to the tillerman where they will verify you indeed have a map. Tell the tillerman to follow this course by simply saying 'start' while on your ship. The tillerman will then follow the course you plotted.</BASEFONT></BODY>", (bool)false, (bool)false);
				}
				else if ( m_Page == 5 )
				{
					AddHtml( 151, 72, 299, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>FISHING NETS</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 116, 107, 393, 486, @"<BODY><BASEFONT Color=" + color + ">Fishing nets are rare items a fisherman would use to see what they can bring up from the deep. These can be commonly found by slaying sea monsters or obtaining ocean treasure and they have four different levels of difficulty to use them:<br><br>-Fishing Net<br>-Strong Fishing Net<br>-Ancient Fishing Net<br>-Neptune's Fishing Net<br><br>These nets cannot be used near any shores. When used, the net sinks slowly into the sea with some bubbles, another wave of bubble heralds the arrival of commensurate creatures appearing around your ship so be ready to fight. These nets are used by the bravest of sailors who are looking to make a name for themselves on the high seas.</BASEFONT></BODY>", (bool)false, (bool)false);
					AddImage(594, 180, 10887);
				}
				else if ( m_Page == 6 )
				{

					AddHtml( 151, 72, 299, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>MESSAGES IN A BOTTLE</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 116, 107, 393, 486, @"<BODY><BASEFONT Color=" + color + ">On occasion, a lucky sailor will discover a message in a bottle. Looking at the note inside will reveal a message from a sailor on board a sinking ship. The notes will look old to be sure so the chances that the ship survived their fate are slim. If you wish to attempt to fish up the wreckage, you need to grab a sextant and fishing pole, board your ship, and sail to the coordinates on the note. Once you reach the spot, be sure to have the bottled message in your pack and then you can begin fishing in the waters. The goal of most sailors is to bring up the ship's chest of plunder it contained before it rested in the murky depths below. While attempting to bring up this chest, you will bring up other parts of the wreckage like paintings and bones of the sailors that met their fate on that ship. Keep in mind that these shipwrecks are not like the ones you can visually see on the ocean floor. You can fish in those spots however but the chest of riches it contained was forever lost long ago.</BASEFONT></BODY>", (bool)false, (bool)false);
					AddImage(594, 180, 10887);
				}
				else if ( m_Page == 7 )
				{

					AddHtml( 151, 72, 299, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>DESERTED BOATS</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 116, 107, 393, 486, @"<BODY><BASEFONT Color=" + color + ">There are some that set out in the morning to catch some fish for their families to eat. They have their tiny boats and head away from shore to have a better chance at catching larger fish. There are some, however, that never return. Whether they fell out of their boats and drowned, or their anchor lines broke and their boats drifted away, these now abandoned boats float out on the seas for others to perhaps find. These often contain minor goods and treasure, so feel free to search them if you sail across one. Most sailors will often bring an axe with them so they can chop up these ships to salvage some usable wood. The more skilled the sailor, the better quality of wood that can be salvaged. The amount of wood is greatly affected by the sailor's carpentry skill.</BASEFONT></BODY>", (bool)false, (bool)false);
					AddItem(675, 397, 8857);
					AddItem(682, 203, 8862);
				}
				else if ( m_Page == 8 )
				{
					AddHtml( 151, 72, 299, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>SMALL BOATS</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 116, 107, 393, 486, @"<BODY><BASEFONT Color=" + color + ">There are sometimes small boats out on the seas and they can be easily identified as they have no sails or tiller man. They will, however, have a single crew member on board. They may be a simple citizen out fishing for the day, or perhaps shipping cargo to another place across the horizon. They can also be a roguish pirate that has yet to make a name for themselves to have a crew and galleon at their disposal. How you deal with these boats is up to your morality. The innocent sailors will often ignore others unless they are murderers, criminals, or just have an unsavory karma. The buccaneers and such, however, will simply attack you on sight. However, you handle these sailor is up to you. You can attack them from afar or you can board their ship if you have a grappling hook in your pack. Grappling hooks are sold by sailor merchants and are used to board these smaller boats or huge galleons. To board a ship, simply use the grappling hook and target the crew member on the boat. If you slay the crew member, their boat will sink with only a small section poking out from beneath the waves. Luckily, this will be the hold of the boat so you can search their belongings to see what you wish to take for yourself. Like deserted boats, you can also chop these hulls for wood.</BASEFONT></BODY>", (bool)false, (bool)false);
					AddItem(638, 254, 20889);
					AddItem(692, 307, 6045);
					AddImage(809, 463, 10889);
					AddItem(650, 352, 6055);
					AddItem(711, 299, 6053);
					AddItem(671, 329, 6045);
				}
				else if ( m_Page == 9 )
				{
					AddHtml( 151, 72, 299, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>GALLEONS</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 116, 107, 393, 486, @"<BODY><BASEFONT Color=" + color + ">Galleons are very large ships that very few could ever acquire. You would never be in a position to own such a vessel, but there are those out there that have them and you will see them on the high seas. Like the small boats, you could have the innocent or the villains on these ships. Pirates tend to slay the crew of the innocent sailors and take all of their wealth. Whatever your motivations are, you can choose to attack any galleon you encounter. A galleon captain can have a crew of 9-12 members, and they will all shoot or throw something at nearby enemies. Some throw rocks, daggers, harpoons, or boulders. Others may be more magically inclined and unleash bolts of fire, cold, or energy. Galleon captains often times let their crew fight, participating only a minor amount for some when enemies have not boarded their vessels.<br><br>Although you can shoot at the crew yourself, you may not want to or have the means to do so. Like smaller boats, to board a galleon you will need a grappling hook. Simply use it and target any crewmember to launch yourself on board their ship. If you need to return to your ship, you can use your ship's plank to do so. Be warned, that the captain also has a grappling hook they tend to use. If you think you can attack the captain from the safety of your deck, you would be mistaken. They will most likely use their grappling hook to grab onto you and pull you toward their ship so they can have an easier time slaying you.<br><br>Unlike smaller boats, galleon captains can be quite strong or sometimes very powerful opponents. They didn't get a galleon and crew without becoming such. If you plan to attack such a vessel, you would be wise to judge the fight as it progresses. The more powerful the captain, however, the better the loot within the ship's hold. Many think of pirates and sailors as merely men or elves. The high seas are filled with other creatures that seek plunder on the waves. You could face a powerful ogre and their crew of hideous creatures. A devil from hell could have a vessel with a demonic crew looking for souls. If you are a pirate, you may run into a militia crew of sailing soldiers, looking for the likes of you to bring to justice. Whatever the crew, they will support their captains with their lives. Any remaining crew members will heal their captains, so you must dispatch of the crew before facing off with the captain.<br><br>If you manage to sink a galleon, you will see their hold peeking out from the surface of the water. Like the smaller boats, you can search through this hold for whatever goods and riches you want. If you are after some good deck planks, make sure to chop the remaining part of the ship. Be sure to look through a pirate's loot carefully, as there may be a bounty on their heads and the wanted parchment may be in the hold. You can read over the parchment to see what the bounty is on that particular pirate. Giving this bounty parchment to a town or city guard will grant you the reward.<br><br>Lastly, not all enemies on the high seas are pirates. Some are cultists that worship the many vile water deities of myth and legend. They seek only to rid the seas of those that trespass in their god's domain. Like pirates, they are often sought for justice or feared by sailors. They take what they want as long as it meets their sinister plans, and they have little sympathy for those they take it from.</BASEFONT></BODY>", (bool)false, (bool)true);
					AddImage(598, 249, 10886);
				}
				else if ( m_Page == 10 )
				{
					AddHtml( 151, 72, 299, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>CARGO</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 116, 107, 393, 486, @"<BODY><BASEFONT Color=" + color + ">Most ships deal in cargo. Whether they are transporting it from settlement to settlement, or have it within their holds as plunder, cargo is a large currency on the high seas. When you sink a vessel, either a boat or galleon, their hold may contain some cargo of various types. It can be either crates, chests, tubs, or barrels. You can peek within the contents of cargo, and determine what it is and how to best use it. If you claim cargo from pirates or villains, the cargo will indicate that it was seized and that you will gain karma if you choose to give the goods to the listed merchant that would want it. If you take cargo from the innocent, then it will indicate that it was plundered and that you will lose karma if you choose to exchange it with merchants. Fame can be earned no matter the source of the cargo.<br><br>Cargo has a value and it is determined by a few factors. The first is a random value of the cargo itself. You then factor in how much skill you have as a sailor (seafaring) and how effective you are as a merchant (mercantile). If you are one to beg, and your demeanor is set to begging, you may get some extra gold for the cargo. Being a member of the Mariners Guild will also provide a bonus to the value of the cargo. The last factor is whether you are in a port or not. Parting with cargo in a port will yield more gold, but you are free to rid yourself of it anywhere you can find the appropriate merchant.<br><br>Almost all cargo, however, can simply be claimed by yourself. Each cargo you claim will have the appropriate option for you to do so. The information on the cargo will indicate to you how much of something is stored within the cargo container. If you choose to keep it, you will get the contents as well as the container that held it. So if you want a barrel of 100 lemons, you will get the barrel and the 100 lemons. What cargo you wish to keep is up to you.</BASEFONT></BODY>", (bool)false, (bool)true);
					AddImage(594, 180, 10887);
				}
				else if ( m_Page == 11 )
				{
					AddHtml( 151, 72, 299, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>SAILING PORTS</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 116, 107, 393, 486, @"<BODY><BASEFONT Color=" + color + ">There are sailing ports in the many lands that are homes to sailors and pirates that have little interest in returning to the mainland. <br><br>Sosaria; Port of Anchor Rock<br>Lodoria: Port of Kraken Reef<br>Serpent Island: Port of Serpent Sails<br>Isles of Dread: Port of Shadows<br>Savaged Empire: Port of Savage Seas<br><br>These ports are neutral territory for both sailors and pirates, so the laws are very strict that no harm come to those that visit. Some of the docks around these ports are larger than others, where the Port of Shadows is hidden near the Forgotten Lighthouse. Anchor Rock and Kraken Reef are built on top of large cavernous areas, where some sailors choose to build their homes. Some don't bother to build homes in these caves, but instead mine for nepturite or log for driftwood from the dead trees within. The settlement area, on top of this rocky formation, is where sailors come to rest and barter for goods and services. This particular part of the port is a public area, where those that visit any port village will be within the same port area to better interact with each other. Ports are a common place where sailors or pirates relieve themselves of their cargo since they can get more gold for such transactions.<br><br>The dock area is where you can leave your ship if you choose to embark again later. Depending on the port you visit, the dock can be very large or quite small. Some have access to other public areas like the bank, tavern, or guilds. Any creature giving your ship chase will often dive below the surface when approaching the docks, that is why many sailors seek these refuges when they are too weak to fight.</BASEFONT></BODY>", (bool)false, (bool)true);
					AddImage(596, 140, 10891);
				}
			}

			public override void OnResponse( NetState state, RelayInfo info ) 
			{
				Mobile from = state.Mobile; 
				if ( info.ButtonID > 0 || info.ButtonID < 0 )
				{
					from.SendGump( new SwordsAndShacklesGump( from, info.ButtonID ) );
				}
				else
					from.PlaySound( 0x55 );
			}
		}
	}
}