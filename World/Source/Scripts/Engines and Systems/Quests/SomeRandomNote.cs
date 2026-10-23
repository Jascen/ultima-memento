using System;
using Server;
using Server.Misc;
using Server.Items;
using System.Text;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class SomeRandomNote : Item
	{
		public override Catalogs DefaultCatalog{ get{ return Catalogs.Scroll; } }

		public string ScrollMessage;
		public int ScrollTrue;

		[CommandProperty(AccessLevel.Owner)]
		public string Scroll_Message { get { return ScrollMessage; } set { ScrollMessage = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Scroll_True { get { return ScrollTrue; } set { ScrollTrue = value; InvalidateProperties(); } }

		[Constructable]
		public SomeRandomNote( ) : base( 0x4CCA )
		{
			Weight = 1.0;
			Name = "an old parchment";
			ItemID = Utility.RandomList( 0x4CCA, 0x4CCB );

			switch ( Utility.RandomMinMax( 0, 2 ) )
			{
				case 0:	Name = "parchment";	break;
				case 1:	Name = "note";		break;
				case 2:	Name = "scroll";		break;
			}

			switch ( Utility.RandomMinMax( 0, 5 ) )
			{
				case 0:	Name = "an old" + " " + Name;		break;
				case 1:	Name = "an ancient" + " " + Name;	break;
				case 2:	Name = "a worn" + " " + Name;		break;
				case 3:	Name = "a scribbled" + " " + Name;	break;
				case 4:	Name = "an unusual" + " " + Name;	break;
				case 5:	Name = "a strange" + " " + Name;	break;
			}

			string poison = "lethal";
			switch ( Utility.RandomMinMax( 0, 4 ) )
			{
				case 0:	poison = "lesser"; break;
				case 1:	poison = "regular"; break;
				case 2:	poison = "greater"; break;
				case 3:	poison = "deadly"; break;
				case 4:	poison = "lethal"; break;
			}

			string skull = "lich";

			switch ( Utility.RandomMinMax( 0, 6 ) )
			{
				case 0: skull = "lich";				break;
				case 1: skull = "lich lord";		break;
				case 2: skull = "ancient lich";		break;
				case 3: skull = "demilich";			break;
				case 4: skull = "bone magi";		break;
				case 5: skull = "skeletal mage";	break;
				case 6: skull = "skeletal wizard";	break;
			}

			ItemID = Utility.RandomList( 0xE34, 0x14ED, 0x14EE, 0x14EF, 0x14F0 );

			ScrollTrue = 1; 
			string written = "truth";
			if ( 1 == Utility.RandomMinMax( 0, 1 ) ){ written = "lies"; ScrollTrue = 0; }

			int amnt = Utility.RandomMinMax( 1, 49 );
			int relic = Utility.RandomMinMax( 1, 59 );

			// 50% TRUTH AND 50% LIES /////////////////////
			if ( written == "lies" )
			{
				switch ( amnt )
				{
					case 1:		ScrollMessage = "There is a secret stash of gold under " + RandomThings.GetRandomCity() + "'s bank. If we search long enough, we can probably find it."; break;
					case 2:		ScrollMessage = QuestCharacters.RandomWords() + " the Wizard was killed while exploring " + QuestCharacters.SomePlace( "random" ) + ". Some say his bones wander the halls to this day. If we can find him, we may be able to get that wand he perished with."; break;
					case 3:		ScrollMessage = "Mangar used a powerful spell to put the Skara Brae into a bottle he keeps on his book shelf. We can break the bottle and set everyone free."; break;
					case 4:		ScrollMessage = "Centuries ago, the dead were stuffed with herbs of resurrection and then wrapped in cloth to preserve them."; break;
					case 5:		ScrollMessage = "Some know the way of knightship, but very few have learned the ways of the vile paladin. Seek the Book of " + QuestCharacters.RandomWords() + ", deep within " + QuestCharacters.SomePlace( "random" ) + ". There you will find the answers."; break;
					case 6:		ScrollMessage = "The crown of the Lich King is said to give the wearer ultimate power from death. The maze confuses us so we never learned for sure."; break;
					case 7:		ScrollMessage = "I hidden it in the deepest part of " + QuestCharacters.SomePlace( "random" ) + ". If you find it, bring it to me. Do not open it as the power that would be unleashed would turn the mightiest demon to dust."; break;
					case 8:		ScrollMessage = "I found many tomes and scrolls that have led me to " + QuestCharacters.SomePlace( "random" ) + ". I will search there tomorrow, as " + QuestCharacters.QuestItems( true ) + " will surely be in there."; break;
					case 9:		ScrollMessage = QuestCharacters.RandomWords() + ",<br><br>Bring the four pieces to the blacksmith in " + RandomThings.GetRandomCity() + ". They have the fifth piece and they will assemble the staff for you."; break;
					case 10:	ScrollMessage = "The altar rests within " + QuestCharacters.SomePlace( "random" ) + ", only known by those of old. Take the item and place it on, where speaking '" + QuestCharacters.RandomWords() + "' makes magic gold."; break;
					case 11:	ScrollMessage = "The ..aff of f... p..ts can only be as..mb.ed on the mo.. near t.e co.. and o.ly if  'Ultimum " + QuestCharacters.RandomWords() + "' is spo..n while on the th..e of s..ne."; break;
					case 12:	ScrollMessage = QuestCharacters.RandomWords() + ", I finally learned how we can get the " + GetSpecialItem( relic, 1 ) + ". We need to find the " + RandomThings.GetRandomJob() + " in " + RandomThings.GetRandomCity() + " and pay him 1,000 gold and they will give it to us."; break;
					case 13:	ScrollMessage = QuestCharacters.RandomWords() + ", we need to go to " + RandomThings.GetRandomCity() + " and search the crates in the provisioner shop. One of them has the " + GetSpecialItem( relic, 1 ) + "."; break;
					case 14:	ScrollMessage = "The " + RandomThings.GetRandomJob() + " in " + RandomThings.GetRandomCity() + " told me that we can probably get the " + GetSpecialItem( relic, 1 ) + " if we search the castle of Lord British. We best be sneaky about it."; break;
					case 15:	ScrollMessage = "The " + RandomThings.GetRandomJob() + " in " + RandomThings.GetRandomCity() + " said that the assassins guild would pay a good price for some " + Server.Misc.RandomThings.GetRandomIntelligentRace() + " blood, as it could be used as a " + poison + " poison."; break;
					case 16:	ScrollMessage = "The thief in " + RandomThings.GetRandomCity() + " told me that wearing chainmail armor would help one become an expert in stealth, when they have achieved journeyman."; break;
					case 17:	ScrollMessage = "The thief in " + RandomThings.GetRandomCity() + " told me that wearing plate armor would help one become a grandmaster in stealth, when they have achieved master."; break;
					case 18:	ScrollMessage = UppercaseFirst( QuestCharacters.SomePlace( "parchment" ) ) + " is said to have a shard from the gem of immortality. I must get to it before " + QuestCharacters.ParchmentWriter() + " does. I must first get the map from " + RandomThings.GetRandomCity() + " so I know where to look."; break;
					case 19:	ScrollMessage = "Dear " + QuestCharacters.ParchmentWriter() + ",<br><br>I found the ancient dwarven tomb of " + QuestCharacters.QuestGiver() + ". Although it looks to have been raided decades prior, I did find some old carvings on the wall. It spoke of how the gods granted him a place where he could forge metal and ice together. He used this power to forge armor and weapons for the ancient war with the elves. I am not sure what this all means, but I will continue my search for the tomb of " + QuestCharacters.ParchmentWriter() + ". Maybe the answer lies there.<br><br> - " + QuestCharacters.QuestGiver() + ""; break;
					case 20:	ScrollMessage = "Dear " + QuestCharacters.ParchmentWriter() + ",<br><br>I am sorry I have not written to you in so long, but I found the fabled valley of the orks and have gotten lost within the vast jungles that covers her. I have been ill for several days as I have returned with a sickness I must have gotten from the swamps there. I found a stone tablet while I was there, buried halfway in the muck. It told a tale of " + NameList.RandomName( "ork_male" ) + ", an ork that could poison the steel of any weapon or armor. I am not sure what this means but I will have a sage look at it for me. I will be home soon.<br><br> - " + QuestCharacters.QuestGiver() + ""; break;
					case 21:	ScrollMessage = QuestCharacters.QuestGiver() + " was given magic from the god of power. The weapons they forged were believed to be responsible for the destruction of Ambrosia. I must find them before continuing with my plans. They are coming..."; break;
					case 22:	ScrollMessage = "Those that seek the skull of Mondain are those that desire ultimate power, or ultimate destruction."; break;
					case 23:	ScrollMessage = "The mystical words of letters three. Can only be yelled or spoken by thee. One must find the lich lord's room. And sit on the throne to enter the tomb."; break;
					case 24:	ScrollMessage = QuestCharacters.ParchmentWriter() + ",<br><br>I am off to " + RandomThings.GetRandomCity() + " to dye this bundle of leather for " + QuestCharacters.ParchmentWriter() + ". I hear the tanner there is able to help me and I have that " + Utility.RandomMinMax( 5, 200 ) + " gold from " + QuestCharacters.ParchmentWriter() + " to cover the cost. I will be back before the moon rises."; break;
					case 25:	ScrollMessage = QuestCharacters.ParchmentWriter() + ",<br><br>Everyone is dead! The black knight slain them all. I barely escaped from the gargoyle wizards he has roaming his halls. We cannot go in there with soldiers no longer. We need to find a few thieves that are eager for the wealth legend states is within his vault. Meet me in " + RandomThings.GetRandomCity() + " during the next phase of the moon, by then my leg should be healed and I can travel. If something happens to me before you get there, the word to speak that opens the vault doors is -" + QuestCharacters.RandomWords() + "-.<br><br>- " + QuestCharacters.QuestGiver() + ""; break;
					case 26:	ScrollMessage = "A knight, who had studied the lore, collected the cards, total four. He yelled at the snake to pass Exodus' lake but then he was killed by the floor."; break;
					case 27:	ScrollMessage = QuestCharacters.ParchmentWriter() + ",<br><br>The recipe for this potion is almost complete. " + QuestCharacters.ParchmentWriter() + " has discovered that tree bark may be the last ingredient we need. If you can brave the forest, and get me about 20 fragments, the guildmaster will train a pack animal for you that is safe from harm, does not need to be fed, and is very loyal. If you are to return late with the bark, " + QuestCharacters.ParchmentWriter() + " wants you to find the druid guildmaster and give it to them since they are the ones that truly need it. I will leave for " + RandomThings.GetRandomCity() + " in the morning where I will be staying with friends. If you do not want the pack horse, give the animal back to the guildmaster and they will provide other animals to you.<br><br> - " + QuestCharacters.ParchmentWriter() + " the Druid"; break;
					case 28:	ScrollMessage = QuestCharacters.ParchmentWriter() + ",<br><br>We finally deciphered the cave paintings on the wall, and they were put there by an ancient race called the Gurall. " + QuestCharacters.ParchmentWriter() + " learned from the " + RandomThings.GetRandomJob() + " they met in " + RandomThings.GetRandomCity() + ", that they once tamed and rode snake like creatures called serpyns. What makes them different from serpents is they were rumored to be born of gold and silver serpents. " + QuestCharacters.ParchmentWriter() + " did find bones of a large serpent while exploring " + QuestCharacters.SomePlace( "tavern" ) + ". They were made of gold and we thought they were perhaps carved from such. If all of this is true, then the legends of the Gurall are probably true. The problem is that we think they lived in the lands the gargoyles escaped to many centuries ago. We may never learn how they were able to tame and ride such beasts. Hurry back when your business is done in " + RandomThings.GetRandomCity() + ".<br><br> - " + QuestCharacters.ParchmentWriter() + ""; break;
					case 29:	ScrollMessage = "From " + RandomThings.GetRandomSociety() + "<br><br>We need you to go to " + QuestCharacters.SomePlace( "tavern" ) + " and find the " + RandomThings.RandomMagicalItem() + " for us. Centuries ago, " + QuestCharacters.ParchmentWriter() + " was slain by " + QuestCharacters.ParchmentWriter() + " and the item was stolen. The item looked mundane and the world quickly forgot of its existence. As the murderer traveled the lands and became well known, the item grew in power. It was indestructible and never left their hands. They died as a ruthless ruler and buried within " + QuestCharacters.SomePlace( "tavern" ) + ", but their tomb was soon raided. We need to complete the ritual and return the item to them so the curse can be lifted. Make haste and bring the item to " + QuestCharacters.ParchmentWriter() + " in " + RandomThings.GetRandomCity() + " when you find it. Time is of the essence."; break;
					case 30:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I said I would return to " + RandomThings.GetRandomCity() + " last winter with the materials needed for us to construct that golem. Although I speak true, I did find a diary of a technomancer that is making me search the lands. This technomancer believes that we can get the dark core of Exodus and build it into the golem. This would produce a golem stronger than we have ever seen. I cannot ignore this, as we need the automaton to carry out the next steps in our plan. Give me unil the time of the harvest, and I will return with or without the core."; break;
					case 31:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I know I said I would return to " + RandomThings.MadeUpCity() + ", as soon as I struck gold on the Serpent Island. What I found instead was more valuable than gold. This world is the only place where I found a very strong metal. The gargoyles here call it steel, and I have been gathering it for many moons now. Once I am able to fill my caravan, I will be going to " + RandomThings.MadeUpCity() + " and see what the blacksmith will give me for it.<br><br>- " + QuestCharacters.ParchmentWriter() + ""; break;
					case 32:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I am writing to you in the hopes that you can venture to the Isles of Dread and assist my mining efforts. I have heard legends of a mysterious metal, called brass. It was only said to be found here. The legends are true! If you can, stop by " + RandomThings.MadeUpCity() + " and get a few more pack horses. Maybe even a wagon. I will meet you in the City of Furnace on the height of the half moon.<br><br>- " + QuestCharacters.ParchmentWriter() + ""; break;
					case 33:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>If you find this note, it means that I met my demise. The wizard in " + RandomThings.GetRandomCity() + " was wrong. The magical trap spell he taught me did not work as he stated. Although it did work on the minions that crossed my path, I accidentally walked over one myself. I was engulfed in a cloud of poison and had to flee. Now I lie here, trying to...stop...the... ...venom from... ... ...my veins......"; break;
					case 34:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I am going to need you to sneak into " + RandomThings.GetRandomCity() + " and get that " + RandomThings.RandomMagicalItem() + " from the " + RandomThings.GetRandomJob() + ". I know you are wanted by the city watch, but if you can join the thieves guild, then you could get in there without gaining notice of the guards. I have heard that they seem to ignore guild members because of the gold given to corrupt watchmen. Meet me at the tavern in " + RandomThings.GetRandomCity() + ", and we will make the exchange.<br><br> - " + QuestCharacters.ParchmentWriter(); break;
					case 35:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I am going to need you to bring the wagons to " + RandomThings.GetRandomCity() + " as a legendary miner managed to dig up some orcish ore. With this we can find a blacksmith, greater than a grandmaster, and make some magical weapons and armor for the task before us. The metal is rumored to restore one's stamina, and that some other unknown magical benefits can be drawn from it. I can only wait for " + Utility.RandomMinMax( 2, 8 ) + " days, so be quick about this.<br><br> - " + QuestCharacters.ParchmentWriter(); break;
					case 36:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I am going to need you to bring the wagons to " + RandomThings.GetRandomCity() + " as a legendary lumberjack managed to chop some trollish wood. With this we can find a carpenter, greater than a grandmaster, and make some magical weapons and armor for the task before us. The metal is rumored to restore one's stamina, and that some other unknown magical benefits can be drawn from it. I can only wait for " + Utility.RandomMinMax( 2, 8 ) + " days, so be quick about this.<br><br> - " + QuestCharacters.ParchmentWriter(); break;
					case 37:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I have spent that last " + Utility.RandomMinMax( 2, 8 ) + " years in " + RandomThings.GetRandomCity() + ", learning the secrets of summoning creatures more powerful than we previously thought. Those guild mages have not clue of the power they hold, but I do. I have been researching how to speak with the spirits. With this knowledge, I will be able to weave my spells to summon stronger elementals and daemons. Soon " + RandomThings.GetRandomSociety() + " will be a group worth a seat at the table.<br><br> - " + QuestCharacters.ParchmentWriter(); break;
					case 38:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I have spent that last " + Utility.RandomMinMax( 2, 8 ) + " years in " + RandomThings.GetRandomCity() + ", learning the secrets of summoning creatures more powerful than we previously thought. Those guild mages have not clue of the power they hold, but I do. I have been researching how to speak with the spirits. With this knowledge, I will be able to weave my spells to summon stronger elementals and daemons. Soon " + RandomThings.GetRandomSociety() + " will be a group worth a seat at the table.<br><br> - " + QuestCharacters.ParchmentWriter(); break;
					case 39:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>If you find this note, it means that I met my demise. A witch in " + RandomThings.GetRandomCity() + " is said to have a book that contains the secrets of the skull gate. That one can travel to another world by stepping within it. I did not seek this book as my patience was lacking, and my confidence was strong. As soon as I stepped through it, I went nowhere. I instead was inflicted with some kind of magical poison. Now I lie here, trying to...wait...for ...the... ...poison...to... ... ...wear......"; break;
					case 40:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I have been out searching for the treasure we need, but I find that I cannot carry as much as I would like. I don't want to handle a pack animal, but you could still help. Go to " + RandomThings.MadeUpCity() + " and see if you can get me a magical coin purse. They are special pouches that helps one carry more coins. They do cost much gold, but as soon as you get one meet me at my camp near " + RandomThings.MadeUpDungeon() + ".<br><br> - " + QuestCharacters.QuestGiver() + ""; break;
					case 41:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I am alive and well and am in " + RandomThings.GetRandomCity() + " recovering from my wounds. The seas were rough and I should not have taken such a small boat out that far. " + RandomThings.GetRandomShipName( "", 0 ) + " was in sight so I flagged them for help which they did. Sadly, I lost our boat as Captain " + QuestCharacters.QuestGiver() + " decided to sell it to some " + RandomThings.GetRandomJob() + " for " + Utility.RandomMinMax( 9, 20 ) + "0 gold. They said it was part of my payment for coming on board. I plan to sail off with them to try and earn enough gold to buy a new boat. I will write again soon.<br><br> - " + QuestCharacters.QuestGiver() + ""; break;
					case 42:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>The academy is going to be expanding their alchemical research into mixtures that are not commonly brewed by alchemists. These potions will allow us to turn any mundane item into something quite powerful. In order to do this, the sages tell of a tome we need, where ancient recipes may be learned. I need you to go to " + RandomThings.MadeUpDungeon() + " and make your way to the deepest part. If the sage is correct, you will find the book we need. When you acquire it, meet me in " + RandomThings.GetRandomCity() + ". There is much more to be done.<br><br> - " + QuestCharacters.QuestGiver() + ""; break;
					case 43:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>That " + skull + " skull we found was worth the trouble. Although fragile, it did discover one interesting thing. There was a large gem resting within it. This may be the source of power of the " + skull + ", but that jeweler paid handsomely for it. Meet me in " + RandomThings.GetRandomCity() + " where I will buy you an ale and we will split the gold.<br><br> - " + QuestCharacters.ParchmentWriter() + ""; break;
					case 44:	
						string thing = "book";
						switch ( Utility.RandomMinMax( 0, 3 ) )
						{
							case 0: thing = "scroll";			break;
							case 1: thing = "book";				break;
							case 2: thing = "parchment";		break;
							case 3: thing = "tapestry";			break;
						}
						ScrollMessage = QuestCharacters.ParchmentWriter() + ",<br><br>We found an ancient " + thing + " that told of an artifact in " + Server.Misc.QuestCharacters.SomePlace( "tablet" ) + ". I didn't know what the tablet meant as I had to find one more intelligent to read it. " + QuestCharacters.ParchmentWriter() + " believes that it may rest on those strange pedestals with the runic symbols. The ones with the small chests on them. If this is true, we must find it before " + RandomThings.GetRandomSociety() + " does. " + QuestCharacters.ParchmentWriter() + " and I will meet you in " + RandomThings.GetRandomCity() + " a few days from now. We will have to bring the tablet with us or we will never find it. Do not tell anyone about this, as there are spies about.<br><br> - " + QuestCharacters.ParchmentWriter();
					break;
					case 45:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I finally did it! I found the Vortex Cube while traveling the Underworld. Almost everyone I brought with me was slain by the Slasher but I was able to best the beast. Only " + QuestCharacters.ParchmentWriter() + " and I managed to make it back to " + RandomThings.MadeUpCity() + ". When I used the Cube, I could see the path foward to getting the Codex from the Void. Get here when you can. We need to set out to find the keys, lenses, and crystals if we are to succeed. Our journey will first take us to " + RandomThings.MadeUpDungeon() + ". <br><br> - " + QuestCharacters.ParchmentWriter() + ""; break;
					case 46:
						string researcher = "sage";
						switch ( Utility.RandomMinMax( 0, 2 ) )
						{
							case 0: researcher = "sage";		break;
							case 1: researcher = "scribe";		break;
							case 2: researcher = "librarian";	break;
						}
						ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I saved the " + (Utility.RandomMinMax( 10, 49 )*10) + " gold needed to purchase a research pack from that " + researcher + " in " + RandomThings.MadeUpCity() + ". I started doing some research to learn I need to find a sphere of power in " + RandomThings.MadeUpDungeon() + ". Could you gather our friends and meet me there when the moon is at half? I would feel better if I had comrades to fight off the dangers within.<br><br> - " + QuestCharacters.ParchmentWriter() + ""; break;
					case 47:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I found a way to make use of this orb of the abyss you found. If you do not want it to interfere with your trinket, then you could take it to a blacksmith and they will forge it into a piece of armor you can wear instead. Once you do that, we can head into the Underworld and search for the fabled Titans we have read about.<br><br> - " + QuestCharacters.ParchmentWriter(); break;
					case 48:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I learned the secrets of this mysterious dragon skull we found in " + RandomThings.MadeUpDungeon() + ". It is the essence of a dracolich, but we need to remove the dark aura that conceals its true power. We need to head to " + RandomThings.MadeUpDungeon() + " and find the bloody pentagram of the great demon. If we use the skull within the pentagram, the aura will be removed and we can then pursue our goal on reanimating the creature to do our bidding.<br><br> - " + QuestCharacters.ParchmentWriter(); break;
					case 49:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>A sailor in " + RandomThings.GetRandomCity() + " told me a tale of Neptune building twin serpent pillars out on the sea. One set I found in Sosaria, while another is said to exist in the Serpent Island. If one could learn the secrets of these pillars, then they can travel to another world by stepping between them. If Neptune did in fact build these gateways, then perhaps his castle hold the clue I need to use them. If I find them, we will assemble the crew and have the winds take us there. It may be the shipping route we need to trade with the gargoyles."; break;		
				}
			}
			else
			{
				switch ( amnt )
				{
					case 1:		ScrollMessage = "The flayers live deep below the elven city. We must warn the elven king as soon as we can."; break;
					case 2:		ScrollMessage = "The blue ore is almost impossible to dig. If we can find that adamantium pickaxe that Zorn carries, we may be able to break off some chunks."; break;
					case 3:		ScrollMessage = "The Time Lord's trials consist of four but missing a fifth. The demon lies within the bottle of missing worlds."; break;
					case 4:		ScrollMessage = "If mages find our ways too questionable, we can search the city for the Black Magic Guild. There we can rest and research the dark arts without the watchful eyes of the holy mages."; break;
					case 5:		ScrollMessage = "The guards know not where we plot and plan, but your skills have risen to a level which caught our attention. Look for the oak book shelf and tell no one."; break;
					case 6:		ScrollMessage = "The prayer of Kas is known by dread. From halls lit dark and those like dead. If the words are spoken one will see. That chaos will thrive where light will be."; break;
					case 7:		ScrollMessage = QuestCharacters.RandomWords() + ",<br><br>I have received your letter and I must decline your offer as I am no longer providing the services you request. If you seek an assassin, there is an island far from the watchful eye. You may have luck there."; break;
					case 8:		ScrollMessage = QuestCharacters.RandomWords() + ",<br><br>I am writing to tell you that your brother has perished while in " + QuestCharacters.SomePlace( "random" ) + ". Many creatures are deadly for sure, but some others can surprise you if you are not careful. It seemed to be a small tornado, but it was a ravaging air elemental some wizard decided to unleash on the land. If he would have brought that sword for slaying such creatures, his armor would not have been blown from his skin which caused his demise. So you don't make the same mistakes as your brother, just remember that using weapons that are particularly good at slaying certain creatures will protect you from the strange things they may do to men in battle. I will be back in " + RandomThings.GetRandomCity() + " soon."; break;
					case 9:		ScrollMessage = "The staff of five is sought by those alive. Where time awaits, four pieces are fate. The fifth though forgotten, lays waiting on bottle's bottom."; break;
					case 10:	ScrollMessage = QuestCharacters.RandomWords() + ",<br><br>I had to flee from " + RandomThings.GetRandomCity() + " today as I could not risk being seen by the guards. The death of the " + RandomThings.GetRandomJob() + " was of my doing, but it is not what the townsfolk are whispering about. They wronged someone with powerful allies, and as such sought out the Assassins Guild to deal with the problem. I was who they sent to perform the task. Although things may seem grim, the Guild is able to make the right people forget about my involvement and look the other way in the matter. I know you thought I was just a simple " + RandomThings.GetRandomJob() + ", but I have been under the service of the Guild for many years. We have a safe haven for our group on a island off the east coase of Sosaria. It is hard to find since it rests within a mountain valley, but a cave can be found that leads you to it. If I do not see you before the next half moon, I will meet you in " + RandomThings.GetRandomCity() + " before the moon is full. Travel safe.<br><br>- " + QuestCharacters.RandomWords(); break;
					case 11:	ScrollMessage = "The ..aff of f... p..ts can only be as..mb.ed on the mo.. near t.e co.. and o.ly if  'Ultimum Potentiae' is spo..n while on the th...e of s..ne."; break;	
					case 12:	ScrollMessage = QuestCharacters.RandomWords() + ", I finally learned how we can get the " + GetSpecialItem( relic, 1 ) + ". We need to assemble the others and meet at " + GetSpecialItem( relic, 0 ) + "."; break;
					case 13:	ScrollMessage = QuestCharacters.RandomWords() + ", we need to go to " + GetSpecialItem( relic, 0 ) + " if we are going to obtain the " + GetSpecialItem( relic, 1 ) + " for " + QuestCharacters.RandomWords() + "."; break;
					case 14:	ScrollMessage = "The " + RandomThings.GetRandomJob() + " in " + RandomThings.GetRandomCity() + " told me that we can probably get the " + GetSpecialItem( relic, 1 ) + " if we search " + GetSpecialItem( relic, 0 ) + ". I will meet you in " + RandomThings.GetRandomCity() + " and we will go there together."; break;
					case 15:	
							ScrollMessage = "The " + RandomThings.GetRandomJob() + " in " + RandomThings.GetRandomCity() + " said that the assassins guild would pay a good price for some golden serpent venom, as it could be used as a lethal poison.";
							if ( Utility.RandomMinMax( 1, 2 ) == 1 ){ ScrollMessage = "The " + RandomThings.GetRandomJob() + " in " + RandomThings.GetRandomCity() + " said that the assassins guild would pay a good price for some silver serpent venom, as it could be used as a deadly poison."; }
						break;
					case 16:	ScrollMessage = "The thief in " + RandomThings.GetRandomCity() + " told me that wearing studded leather would help one become an expert in stealth, when they have achieved journeyman."; break;
					case 17:	ScrollMessage = "The thief in " + RandomThings.GetRandomCity() + " told me that wearing ringmail armor and a close helm would help one become a grandmaster in stealth, when they have achieved master."; break;
					case 18:	ScrollMessage = QuestCharacters.ParchmentWriter() + " said I could get mystical pearls if I brave the high seas and hunt for the largest sea beasts. Since my pockets are empty, this may be something I could do without paying " + QuestCharacters.ParchmentWriter() + " all that gold."; break;
					case 19:	ScrollMessage = "Dear " + QuestCharacters.ParchmentWriter() + ",<br><br>I found the ancient dwarven tomb of Dugero the Strong. Although it looks to have been raided decades prior, I did find some old carvings on the wall. It spoke of how the gods granted him a place where he could forge metal and ice together. He used this power to forge armor and weapons for the ancient war with the elves. I am not sure what this all means, but I will continue my search for Valandra's tomb. Maybe the answer lies there.<br><br> - " + QuestCharacters.QuestGiver() + ""; break;
					case 20:	ScrollMessage = "Dear " + QuestCharacters.ParchmentWriter() + ",<br><br>I am sorry I have not written to you in so long, but I found the fabled valley of the orks and have gotten lost within the vast jungles that covers her. I have been ill for several days as I have returned with a sickness I must have gotten from the swamps there. I found a stone tablet while I was there, buried halfway in the muck. It told a tale of Urag, an ork that could poison the steel of any weapon or armor. I am not sure what this means but I will have a sage look at it for me. I will be home soon.<br><br> - " + QuestCharacters.QuestGiver() + ""; break;
					case 21:	ScrollMessage = "Galzan the Wizard was given magic from the god of power. The weapons he forged were believed to be responsible for the destruction of Ambrosia. I must find them before continuing with my plans. They are coming..."; break;
					case 22:	ScrollMessage = QuestCharacters.ParchmentWriter() + " the Thief said there is a secret passage into Lord British's treasure chamber."; break;
					case 23:	ScrollMessage = "The trituns live below the sea. They rarely come above to be. They serve the storm giant on one knee. Which holds the key to invulnerability."; break;
					case 24:	ScrollMessage = QuestCharacters.ParchmentWriter() + ",<br><br>I am off to Montor to dye this bundle of leather for " + QuestCharacters.ParchmentWriter() + ". I hear the tanner there is able to provide such a service and I have that " + Utility.RandomMinMax( 5, 200 ) + " gold from " + QuestCharacters.ParchmentWriter() + " to cover the cost. I will be back before the moon rises."; break;
					case 25:	ScrollMessage = QuestCharacters.ParchmentWriter() + ",<br><br>We set sail tomorrow for the rough seas of the Serpent Island. " + QuestCharacters.ParchmentWriter() + " has discovered a secret entrance into the black knight's vault, and we intend to take as many riches as our ship can hold. They found it by pure luck as the cave is on the side of the mountain, perfect for us to anchor the ship. We might even leave " + QuestCharacters.ParchmentWriter() + " behind to bring more back treasure, which would solve our little problem with we have with them. Don't forget that sword I need. You can find it hidden behind the forge in " + RandomThings.GetRandomCity() + ". Meet me at the docks tonight, so we can get ready for sail.<br><br> - " + QuestCharacters.ParchmentWriter() + " of Skulls & Shackles"; break;
					case 26:	ScrollMessage = "A knight, who had studied the lore, collected the cards, total four. He yelled at the snake to pass Exodus' lake and then made Exodus no more."; break;
					case 27:	ScrollMessage = QuestCharacters.ParchmentWriter() + ",<br><br>The recipe for this potion is almost complete. " + QuestCharacters.ParchmentWriter() + " has discovered that wild mushrooms may be the last ingredient we need. If you can brave the forest, and get me about 20 of them, the guildmaster will train a pack animal for you that is safe from harm, does not need to be fed, and is very loyal. If you are to return late with the mushrooms, " + QuestCharacters.ParchmentWriter() + " wants you to find the druid guildmaster and give it to them since they are the ones that truly need it. I will leave for " + RandomThings.GetRandomCity() + " in the morning where I will be staying with friends. If you do not want the pack horse, give the animal back to the guildmaster and they will provide other animals to you.<br><br> - " + QuestCharacters.ParchmentWriter() + " the Druid"; break;
					case 28:	ScrollMessage = QuestCharacters.ParchmentWriter() + ",<br><br>We finally deciphered the cave paintings on the wall, and they were put there by an ancient race called the Zuluu. " + QuestCharacters.ParchmentWriter() + " learned from the " + RandomThings.GetRandomJob() + " they met in " + RandomThings.GetRandomCity() + ", that they once tamed and rode dragon like creatures called dragyns. What makes them different from dragons is they were rumored to be born of wyrms of gemmed stone. " + QuestCharacters.ParchmentWriter() + " did find bones of a dragon while exploring " + QuestCharacters.SomePlace( "tavern" ) + ". They were made of " + RandomThings.GetRandomGemType( "dragyns" ) + " and we thought they were perhaps carved from such. If all of this is true, then the legends of the Zuluu are probably true. The problem is that we think they lived on the continent that sank into the sea so many centuries ago. We may never learn how they were able to tame and ride such beasts. Hurry back when your business is done in " + RandomThings.GetRandomCity() + ".<br><br> - " + QuestCharacters.ParchmentWriter() + ""; break;
					case 29:	ScrollMessage = "From " + RandomThings.GetRandomSociety() + "<br><br>We need you to go to " + QuestCharacters.SomePlace( "tavern" ) + " and find the " + RandomThings.RandomMagicalItem() + " for us. Centuries ago, " + QuestCharacters.ParchmentWriter() + " entered the Hall of Legends and asked the god within to grant them an artifact forged in their name. Their deeds were well known throughout the many lands and the god granted their wish. The item was mundane and the world quickly forgot of their deeds as tribute to the god. As they traveled the lands and reclaimed their fame, the item grew in power. It was indestructible and never left their hands. They died as a powerful ruler of the land and now have a place within the Hall of Legends, but their artifact was lost upon their death. We need to complete the ritual and return the item to them so the curse can be lifted. Make haste and bring the item to " + QuestCharacters.ParchmentWriter() + " in " + RandomThings.GetRandomCity() + " when you find it. Time is of the essence."; break;
					case 30:	ScrollMessage = "From " + RandomThings.GetRandomSociety() + "<br><br>We all heard the stories. The Stranger destroyed Exodus and left the castle in ruins. What we have uncovered is that Minax had created a paradox years ago, that caused the very impressions of time to bend around us. Exodus was indeed destroyed, but another version, from another time, still roams the land. We need to find this automaton and destroy it. The dark core is no more, but the it's counterpart, the Psyche, still exists. It held Exodus' emotions and personality, and it was the combination of the two components that made up the sum of the hellspawn's mind. We must go to " + QuestCharacters.SomePlace( "tavern" ) + ", where they were last seen. Buy what supplies you need and meet tomorrow in " + RandomThings.GetRandomCity() + ". We will venture off from there."; break;
					case 31:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I know I said I would return to " + RandomThings.GetRandomCity() + ", as soon as I struck gold in the Savaged Empire. What I found instead was more valuable than gold. This world is the only place where I found a very strong metal. The orks here call it steel, and I have been gathering it for many moons now. Once I am able to fill my caravan, I will be going to " + RandomThings.GetRandomCity() + " and see what the blacksmith will give me for it.<br><br>- " + QuestCharacters.ParchmentWriter() + ""; break;
					case 32:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I am writing to you in the hopes that you can venture to Umber Veil and assist my mining efforts. I have heard legends of a mysterious metal, called brass. It was only said to be found here. The legends are true! If you can, stop by " + RandomThings.GetRandomCity() + " and get a few more pack horses. Maybe even a wagon. I will meet you in Renika on the height of the half moon.<br><br>- " + QuestCharacters.ParchmentWriter() + ""; break;
					case 33:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>If you find this note, it means that I met my demise. The wizard in " + RandomThings.GetRandomCity() + " was wrong. The magical trap spell he taught me was not as effective against the daemon as he said it would be. Although it did work on the minions that crossed my path, the daemon was able to use psychology on me which allowed him to see the runic symbol I placed at his feet. I barely escaped with my life. Now I lie here, trying to...stop...the... ...bleeding from... ... ...my wound......"; break;
					case 34:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I am going to need you to sneak into " + RandomThings.GetRandomCity() + " and get that " + RandomThings.RandomMagicalItem() + " from the " + RandomThings.GetRandomJob() + ". I know you are wanted by the city watch, but if you can get a disguise kit from the thieves guild, then you could get in there without gaining notice of the guards. Disguise kits are difficult to use, but those that are have reached just a raw talent of apprentice in skills such as psychology, ninjitsu, stealth, hiding, or snooping should be able to apply it effectively. Be warned that they may only last for a couple of hours before the disguise wears down, so do your business and get out. They are not perfect, as merchants often are leery about those wearing disguises. They just cannot seem to trust one that looks a bit unusual. Also, some criminal acts will cause those to see the real you so keep to yourself as much as you can. Meet me at the tavern in " + RandomThings.GetRandomCity() + ", and we will make the exchange.<br><br> - " + QuestCharacters.ParchmentWriter(); break;
					case 35:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I am going to need you to bring the wagons to " + RandomThings.GetRandomCity() + " as a legendary miner managed to dig up some dwarven ore. With this we can find a blacksmith, greater than a grandmaster, and make some magical weapons and armor for the task before us. The metal is rumored to heal a warrior's wounds, and that some other unknown magical benefits can be drawn from it. I can only wait for " + Utility.RandomMinMax( 2, 8 ) + " days, so be quick about this.<br><br> - " + QuestCharacters.ParchmentWriter(); break;
					case 36:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I am going to need you to bring the wagons to " + RandomThings.GetRandomCity() + " as a legendary lumberjack managed to chop some elven wood. With this we can find a carpenter, greater than a grandmaster, and make some magical weapons and armor for the task before us. The metal is rumored to heal a warrior's wounds, and that some other unknown magical benefits can be drawn from it. I can only wait for " + Utility.RandomMinMax( 2, 8 ) + " days, so be quick about this.<br><br> - " + QuestCharacters.ParchmentWriter(); break;
					case 37:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I have spent that last " + Utility.RandomMinMax( 2, 8 ) + " years in " + RandomThings.GetRandomCity() + ", learning the secrets of summoning creatures more powerful than we previously thought. Those guild mages have not clue of the power they hold, but I do. I have been researching how to use psychology on other creatures. With this wisdom, I will be able to weave my spells to summon stronger elementals and daemons. Soon " + RandomThings.GetRandomSociety() + " will be a group worth a seat at the table.<br><br> - " + QuestCharacters.ParchmentWriter(); break;
					case 38:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I have spent that last " + Utility.RandomMinMax( 2, 8 ) + " years in " + RandomThings.GetRandomCity() + ", learning the secrets of summoning creatures more powerful than we previously thought. Those guild mages have not clue of the power they hold, but I do. I have been researching how to use psychology on other creatures. With this wisdom, I will be able to weave my spells to summon stronger elementals and daemons. Soon " + RandomThings.GetRandomSociety() + " will be a group worth a seat at the table.<br><br> - " + QuestCharacters.ParchmentWriter(); break;
					case 39:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>If you find this note, it means that I met my demise. A witch in Ravendark is said to have a book that contains the secrets of the skull gate. That one can travel to another world by stepping within it. I did not seek this book as my patience was lacking, and my confidence was strong. As soon as I stepped through it, I went nowhere. I instead was inflicted with some kind of magical poison. Now I lie here, trying to...wait...for ...the... ...poison...to... ... ...wear......"; break;
					case 40:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I have been out searching for the reagents we need to create those potions for our friend, but I find that I cannot carry as much as I would like. I don't want to handle a pack animal, but you could still help. Go to " + RandomThings.GetRandomCity() + " and see if you can get me an alchemy rucksack. They are special bags that helps one carry more alchemical crafting items. They do cost much gold, but as soon as you get one meet me at my camp near " + RandomThings.MadeUpDungeon() + ".<br><br> - " + QuestCharacters.QuestGiver() + ""; break;
					case 41:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I am alive and well and am in " + RandomThings.GetRandomCity() + " recovering from my wounds. The seas were rough and I should not have taken such a small boat out that far. " + RandomThings.GetRandomShipName( "", 0 ) + " was in sight so I flagged them for help which they did. Sadly, I lost our boat as Captain " + QuestCharacters.QuestGiver() + " decided to use an axe and chop it up for the boards to use. They said it was part of my payment for coming on board. They said that a good carpenter can get much of the boards from a boat, but a sailor gets the best wood from it. I plan to sail off with them to try and earn enough gold to buy a new boat. I will write again soon.<br><br> - " + QuestCharacters.QuestGiver() + ""; break;
					case 42:	
						switch ( Utility.Random( 2 ) )
						{
							case 0: ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>The academy has told me that there is alchemical research into mixtures that are not commonly brewed by alchemists. These mixtures allows us to create potions that, when poured out, will unleash a liquid that can help vanquish any foes we may face on our journey. In order to do this, we need a book called Alchemical Mixtures. I need you to go to " + RandomThings.GetRandomCity() + " and see if you can acquire one of these books. Bring it to my laboratory when you found it and we will get to work.<br><br> - " + QuestCharacters.QuestGiver() + ""; break;
							case 1: ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>The academy has told me that there is alchemical research into elixirs that are not commonly brewed by alchemists. These elixirs enhance our abilities and will greatly help us on our journey. In order to do this, we need a book called Alchemical Elixirs. I need you to go to " + RandomThings.GetRandomCity() + " and see if you can acquire one of these books. Bring it to my laboratory when you found it and we will get to work.<br><br> - " + QuestCharacters.QuestGiver() + ""; break;
						}
					break;
					case 43:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>That " + skull + " skull you brought me was interesting indeed. Although fragile, it did have one interesting property. The magic from the dead spellcaster was locked within it. Although a wizard of the light would destroy such things, I myself find it quite usefull to replenish my precious mana when I perform my rituals. If you come across anymore of these, I would pay handsomely.<br><br> - " + QuestCharacters.ParchmentWriter() + " the Necromancer"; break;
					case 44:	ScrollMessage = QuestCharacters.ParchmentWriter() + ",<br><br>We found an ancient tablet that told of an artifact in " + Server.Misc.QuestCharacters.SomePlace( "tablet" ) + ". I didn't know what the tablet meant as I had to find one more intelligent to read it. " + QuestCharacters.ParchmentWriter() + " believes that it may rest on those strange pedestals with the runic symbols. The ones with the small chests on them. If this is true, we must find it before " + RandomThings.GetRandomSociety() + " does. " + QuestCharacters.ParchmentWriter() + " and I will meet you in " + RandomThings.GetRandomCity() + " a few days from now. We will have to bring the tablet with us or we will never find it. Do not tell anyone about this, as there are spies about.<br><br> - " + QuestCharacters.ParchmentWriter(); break;
					case 45:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I finally did it! I found the Codex of Ultimate Wisdom while traveling the Serpent Island. Almost everyone I brought with me was slain by the cursed creature but I was able to best the beast. Only " + QuestCharacters.ParchmentWriter() + " and I managed to make it back to " + RandomThings.MadeUpCity() + ". When I used the Codex, I could see the path foward to getting the Gem of Immortality. Get here when you can. We need to set out to find the orbs, scrolls, and keys if we are to succeed. Our journey will first take us to " + RandomThings.MadeUpDungeon() + ". <br><br> - " + QuestCharacters.ParchmentWriter() + ""; break;
					case 46:
						string researcher = "sage";
						switch ( Utility.RandomMinMax( 0, 2 ) )
						{
							case 0: researcher = "sage";		break;
							case 1: researcher = "scribe";		break;
							case 2: researcher = "librarian";	break;
						}
						ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I saved the 500 gold needed to purchase a research pack from that " + researcher + " in " + RandomThings.GetRandomCity() + ". I started doing some research to learn I need to find a cube of power in " + RandomThings.MadeUpDungeon() + ". Could you gather our friends and meet me there when the moon is at half? I would feel better if I had comrades to fight off the dangers within.<br><br> - " + QuestCharacters.ParchmentWriter() + ""; break;
					case 47:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I found a way to make use of this orb of the abyss you found. If you do not want it to interfere with your trinket, then you could take it to a tinker and they will modify it into jewelry that you can wear instead. Once you do that, we can head into the Underworld and search for the fabled Titans we have read about.<br><br> - " + QuestCharacters.ParchmentWriter(); break;
					case 48:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>I learned the secrets of this mysterious dragon skull we found in " + RandomThings.MadeUpDungeon() + ". It is the essence of a dracolich, but we need to remove the dark aura that conceals its true power. We need to head to Dungeon Hythloth and find the bloody pentagram of the great demon. If we use the skull within the pentagram, the aura will be removed and we can then pursue our goal on reanimating the creature to do our bidding.<br><br> - " + QuestCharacters.ParchmentWriter(); break;
					case 49:	ScrollMessage = "" + QuestCharacters.ParchmentWriter() + ",<br><br>A sailor in " + RandomThings.GetRandomCity() + " told me a tale of Poseidon building twin serpent pillars out on the sea. One set I found in Sosaria, while another is said to exist in Lodoria. If one could learn the secrets of these pillars, then they can travel to another world by stepping between them. If Poseidon did in fact build these gateways, then perhaps his caverns hold the clue I need to use them. If I find them, we will assemble the crew and have the winds take us there. It may be the shipping route we need to trade with the elves."; break;		}
			}
		}

		public class ClueGump : Gump
		{
			public ClueGump( Mobile from, Item parchment ): base( 100, 100 )
			{
				SomeRandomNote scroll = (SomeRandomNote)parchment;
				string sText = scroll.ScrollMessage;
				from.PlaySound( 0x249 );

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				AddImage(0, 0, 10901, 2786);
				AddImage(0, 0, 10899, 2117);
				AddHtml( 45, 78, 386, 218, @"<BODY><BASEFONT Color=#d9c781>" + sText + "</BASEFONT></BODY>", (bool)false, (bool)true);
			}

			public override void OnResponse( NetState state, RelayInfo info ) 
			{
				Mobile from = state.Mobile; 
				from.PlaySound( 0x249 );
			}
		}

		static string UppercaseFirst(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			return char.ToUpper(s[0]) + s.Substring(1);
		}

		public static string GetSpecialItem( int relic, int part )
		{
			string Part1 = "";
			switch ( relic )
			{
				case 1: Part1 = "Stonegate Castle"; break;
				case 2: Part1 = "the Vault of the Black Knight"; break;
				case 3: Part1 = "the Crypts of Dracula"; break;
				case 4: Part1 = "the Lodoria Catacombs"; break;
				case 5: Part1 = "Dungeon Deceit"; break;
				case 6: Part1 = "Dungeon Despise"; break;
				case 7: Part1 = "Dungeon Destard"; break;
				case 8: Part1 = "the City of Embers"; break;
				case 9: Part1 = "Dungeon Hythloth"; break;
				case 10: Part1 = "the Ice Fiend Lair"; break;
				case 11: Part1 = "Dungeon Shame"; break;
				case 12: Part1 = "Terathan Keep"; break;
				case 13: Part1 = "the Halls of Undermountain"; break;
				case 14: Part1 = "the Volcanic Cave"; break;
				case 15: Part1 = "the Mausoleum"; break;
				case 16: Part1 = "the Tower of Brass"; break;
				case 17: Part1 = "Vordo's Dungeon"; break;
				case 18: Part1 = "the Dragon's Maw"; break;
				case 19: Part1 = "the Ancient Pyramid"; break;
				case 20: Part1 = "Dungeon Exodus"; break;
				case 21: Part1 = "the Caverns of Poseidon"; break;
				case 22: Part1 = "Dungeon Clues"; break;
				case 23: Part1 = "Dardin's Pit"; break;
				case 24: Part1 = "Dungeon Doom"; break;
				case 25: Part1 = "the Fires of Hell"; break;
				case 26: Part1 = "the Mines of Morinia"; break;
				case 27: Part1 = "the Perinian Depths"; break;
				case 28: Part1 = "the Dungeon of Time Awaits"; break;
				case 29: Part1 = "the Ancient Prison"; break;
				case 30: Part1 = "the Cave of Fire"; break;
				case 31: Part1 = "the Cave of Souls"; break;
				case 32: Part1 = "Dungeon Ankh"; break;
				case 33: Part1 = "Dungeon Bane"; break;
				case 34: Part1 = "Dungeon Hate"; break;
				case 35: Part1 = "Dungeon Scorn"; break;
				case 36: Part1 = "Dungeon Torment"; break;
				case 37: Part1 = "Dungeon Vile"; break;
				case 38: Part1 = "Dungeon Wicked"; break;
				case 39: Part1 = "Dungeon Wrath"; break;
				case 40: Part1 = "the Flooded Temple"; break;
				case 41: Part1 = "the Gargoyle Crypts"; break;
				case 42: Part1 = "the Serpent Sanctum"; break;
				case 43: Part1 = "the Tomb of the Fallen Wizard"; break;
				case 44: Part1 = "the Blood Temple"; break;
				case 45: Part1 = "the Dungeon of the Mad Archmage"; break;
				case 46: Part1 = "the Tombs"; break;
				case 47: Part1 = "the Dungeon of the Lich King"; break;
				case 48: Part1 = "the Forgotten Halls"; break;
				case 49: Part1 = "the Ice Queen Fortress"; break;
				case 50: Part1 = "Dungeon Rock"; break;
				case 51: Part1 = "the Scurvy Reef"; break;
				case 52: Part1 = "the Undersea Castle"; break;
				case 53: Part1 = "the Tomb of Kazibal"; break;
				case 54: Part1 = "the Azure Castle"; break;
				case 55: Part1 = "the Catacombs of Azerok"; break;
				case 56: Part1 = "Dungeon Covetous"; break;
				case 57: Part1 = "the Glacial Scar"; break;
				case 58: Part1 = "the Temple of Osirus"; break;
				case 59: Part1 = "the Sanctum of Saltmarsh"; break;
			}

			if ( part > 0 ){ return GetRelicItem( Part1 ); }

			return Part1;
		}

		public static string GetRelicItem( string name )
		{
			switch ( name )
			{
				case "Stonegate Castle": return "heart of ash";
				case "the Vault of the Black Knight": return "mystical wax";
				case "the Crypts of Dracula": return "vampire teeth";
				case "the Lodoria Catacombs": return "face of the ancient king";
				case "Dungeon Deceit": return "wand of Talosh";
				case "Dungeon Despise": return "head of Urg";
				case "Dungeon Destard": return "flame of Dramulox";
				case "the City of Embers": return "crown of Vorgol";
				case "Dungeon Hythloth": return "claw of Saramon";
				case "the Frozen Hells":
				case "the Ice Fiend Lair": return "horn of the frozen hells";
				case "Dungeon Shame": return "elemental salt";
				case "Terathan Keep": return "eye of plagues";
				case "the Halls of Undermountain": return "hair of the earth";
				case "the Volcanic Cave": return "skull of Turlox";
				case "the Mausoleum": return "tattered robe of Mezlo";
				case "the Tower of Brass": return "blood of the forest";
				case "Vordo's Dungeon": return "cinders of life";
				case "the Dragon's Maw": return "crystal scales";
				case "the Ancient Pyramid": return "chest of suffering";
				case "Dungeon Exodus": return "whip from below";
				case "the Caverns of Poseidon": return "scale of the sea";
				case "Dungeon Clues": return "braclet of war";
				case "Dardin's Pit": return "stump of the ancients";
				case "Dungeon Doom": return "dark blood";
				case "the Fires of Hell": return "firescale tooth";
				case "the Mines of Morinia": return "ichor of Xthizx";
				case "the Perinian Depths": return "heart of a vampire queen";
				case "the Dungeon of Time Awaits": return "hourglass of ages";
				case "the Ancient Prison": return "shackles of Saramak";
				case "the Cave of Fire": return "mouth of embers";
				case "the Cave of Souls": return "cowl of shadegloom";
				case "Dungeon Ankh": return "wedding dress of virtue";
				case "Dungeon Bane": return "lilly pad of the bog";
				case "Dungeon Hate": return "immortal bones";
				case "Dungeon Scorn": return "staff of scorn";
				case "Dungeon Torment": return "mind of allurement";
				case "Dungeon Vile": return "mask of the ghost";
				case "Dungeon Wicked": return "dead venom flies";
				case "Dungeon Wrath": return "branch of the reaper";
				case "the Flooded Temple": return "ink of the deep";
				case "the Gargoyle Crypts": return "amulet of the stygian abyss";
				case "the Serpent Sanctum": return "skin of the guardian";
				case "the Tomb of the Fallen Wizard": return "orb of the fallen wizard";
				case "the Blood Temple": return "bleeding crystal";
				case "the Dungeon of the Mad Archmage": return "jade idol of Nesfatiti";
				case "the Tombs": return "scroll of Abraxus";
				case "the Dungeon of the Lich King": return "sphere of the dark circle";
				case "the Forgotten Halls": return "urn of Ulmarek's ashes";
				case "the Ice Queen Fortress": return "crystal of everfrost";
				case "Dungeon Rock": return "stone of the night gargoyle";
				case "the Scurvy Reef": return "pearl of Neptune";
				case "the Undersea Castle": return "Black Beard's brandy";
				case "the Tomb of Kazibal": return "lamp of the desert";
				case "the Azure Castle": return "azure dust";
				case "the Catacombs of Azerok": return "skull of Azerok";
				case "Dungeon Covetous": return "egg of the harpy hen";
				case "the Glacial Scar": return "bone of the frost giant";
				case "the Temple of Osirus": return "mind of silver";
				case "the Sanctum of Saltmarsh": return "scale of Scarthis";
			}

			return "";
		}

		public override void OnDoubleClick( Mobile e )
		{
			if ( !IsChildOf( e.Backpack ) ) 
			{
				e.SendMessage( "This must be in your backpack to read." );
			}
			else
			{
				e.CloseGump( typeof( ClueGump ) );
				e.SendGump( new ClueGump( e, this ) );
			}
		}

		public SomeRandomNote(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
            writer.Write( ScrollMessage );
            writer.Write( ScrollTrue );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			ScrollMessage = reader.ReadString();
			ScrollTrue = reader.ReadInt();
		}
	}
}