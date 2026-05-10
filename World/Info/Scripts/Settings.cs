namespace Server
{
	public static class MySettings
	{

	///////////////////////////////////////////////////////////////////////////////////////////////
	// INDEX - USE CTRL-F to get to the desired section ///////////////////////////////////////////
	// 001 - SYSTEM OPTIONS ///////////////////////////////////////////////////////////////////////
	// 002 - GAME OPTIONS /////////////////////////////////////////////////////////////////////////
	// 003 - PLAYER OPTIONS ///////////////////////////////////////////////////////////////////////
	// 004 - QUESTS & TREASURE ////////////////////////////////////////////////////////////////////
	// 005 - SKILLS ///////////////////////////////////////////////////////////////////////////////
	// 006 - CRAFTING /////////////////////////////////////////////////////////////////////////////
	// 007 - MONSTERS & CREATURES /////////////////////////////////////////////////////////////////
	// 008 - MERCHANTS ////////////////////////////////////////////////////////////////////////////
	// 009 - HOMES & SHIPS ////////////////////////////////////////////////////////////////////////
	// 010 - PETS, MOUNTS, & FOLLOWERS ////////////////////////////////////////////////////////////
	// 011 - TOWNS & CITIES ///////////////////////////////////////////////////////////////////////
	// 012 - ACKNOWLEDGEMENT //////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	///////////////////////////////////////////////////////////////////////////////////////////////
	// 001 - SYSTEM OPTIONS ///////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// Enables commands to be entered into the server console. WARNING: May cause crashes so enable it at your own risk.

		public static bool S_EnableConsole = false;

	// Enables output messages to the console. Messages are from tasks that run and the steps of running the world building command.

		public static bool ConsoleLog = false;

	// These settings will create a button on the Message of the Day. If you do not fill in a website name, the text next to the 
	// button will simply say Website. When players select the button, it should open their browser to that site.
	// EXAMPLE: https://google.com

		public static string S_WebsiteLink = "https://discord.gg/mu7aAbRyJf";
		public static string S_WebsiteName = "Memento Community Discord";

	// The game saves itself after this many minutes in decimal format between 10 and 240 minutes.

		public static double S_ServerSaveMinutes = 30.0;

	// If true, saves the game when your character logs out. Helpful for single player games.

		public static bool S_SaveOnCharacterLogout = true;

	// The server has some self-cleaning and self-sustaining scripts it runs every hour, 3 hours, & 24 hours. If you run
	// a 24x7 server, you can set the below to false since your server will run these at those times, but if you play
	// single player, and you turn the server on/off as required, then set this to true so these routines at least run
	// when you start the game for you.

		public static bool S_RunRoutinesAtStartup = true;

	// This setting is the number of days a character must exist before a player can delete them.

		public static double S_DeleteDays = 3.0;

	// If true, players can just type in a name and password and it will create an account for them.

		public static bool S_AutoAccounts = true;

	// The port you want your server to listen on.

		public static int S_Port = 2593;

	// If you want to enter your IP for external connections, you can enter it here. Otherwise, the autodetect function
	// below can likely do it for you automatically.
	// EXAMPLES
	// public static string S_Address = "192.16.1.4";
	// public static string S_Address = "211.12.35.213";

		public static string S_Address = null;

	// Here you can enter the name of your server/world

		public static string S_ServerName = "Memento";

	// If true, your public IP address will be auto detected to help with external connections.

		public static bool S_AutoDetect = true;



	///////////////////////////////////////////////////////////////////////////////////////////////
	// 002 - GAME OPTIONS /////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// If true, the blackjack tables will retain their settings and information if the server ever restarts. Better for
	// persistent worlds.

		public static bool S_PersistentBlackjack = false;

	// The percent chance a floor trap will trigger in whole number format and no less than 5 percent.

		public static int S_FloorTrapTrigger = 75;

	// If true, anytime a character makes a saving throw to avoid a trap will be notified. Otherwise, they will never know they avoided it.

		public static bool S_AnnounceTrapSaves = true;

	// If true, then dungeon environments will have random sounds as you traverse the corridors.

		public static bool S_EnableDungeonSoundEffects = true;	

	// If true, then the strange portals that lead to deep and dangerous dungeons will have an exit portal.

		public static bool S_PortalExits = true;

	// If set to 1,000 gold or higher, then the bribery system will be enabled that allows characters to give this amount of gold
	// to the Assassin Guildmaster so they can bribe the right people and remove a murder count one at a time (never applies to
	// fugitives, and assassin guild members only pay half this amount).

		public static int S_Bribery = 50000;

	// There are almost 300 classic artifacts in the game, as well as artifacts created for this game that are specifically named
	// and designed. These are items like 'Stormbringer' or 'Conan's Lost Sword'. By default, these items will retain their
	// appearance and color no matter what is done to them. Setting this to true will allow a player to use items like the Magic
	// Scissors or Magic Hammer to change the appearance of the items, but they will always retain their name. This is false by default.

		public static bool S_ChangeArtyLook = true;

	// The below setting is the number of minutes that a player character corpse will turn into bones, which can be used in
	// conjunction with the setting below. These two settings, when added together, are the total number of minutes that a
	// player has to find their corpse and potentially collect their belongings. The default for this setting is 10 minutes
	// and the below is 110 minutes for a combined 2 hours or 120 minutes.

		public static int S_CorpseDecay = 10;

	// The below setting is the number of minutes that a player character bones will decay. This option, as well as the
	// option above, could potentially be used to have player character corpses remain longer or for a more difficult style
	// of play where the corpse and belongings vanish immediatley. If running a multiplayer game, where PVP is promoted and
	// you want to use a more difficult style of play, then setting these two combined minutes to something long enough for
	// an enemy player to take the dead character's belongings may be desired. The default is 110 minutes.

		public static int S_BoneDecay = 1430;




	///////////////////////////////////////////////////////////////////////////////////////////////
	// 003 - PLAYER OPTIONS ///////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// If true, a player character cannot use macros to improve their skills quickly.

		public static bool S_NoMacroing = true;

	// You can increase the rate that stats gain from 50.0 (slow) to 10.0 (fast).

		public static double S_StatGain = 33.3;

	// How many minutes between stat gains which helps with the above setting. This can be between 5.0 to 60.0 minutes.

		public static double S_StatGainDelay = 7.5;

	// If true, new characters can choose to take the alien origin route.
	// This is a play style where one can choose to enter a transporter and be a character that crashed
	// here from another world. This gives them the ability to grandmaster 40 skills but they begin the
	// the game with no gold or any skills. They will also suffer:
	// 4x guild fees
	// 3x resurrection fees
	// suffer double stat/fame/karma/skill loss on death with no tribute
	// suffer normal stat/fame/karma/skill loss on death with tribute

		public static bool S_AllowAlienChoice = false;

	// Set to the number 0 to disable. The other values are 1, 2, or 3 where the default is 1. When greater than 0, you will
	// enable the creature character feature of the game. This allows players to become a creature of the game as their
	// player character. They can play like the game normally plays, but since they would be using creature models their
	// appearance will remain static (some species have options to change appearances to other models). They can equip and
	// use things normally but their equipment will be displayed as icons on their paperdoll (around the borders) so
	// they can manage their inventory. They will not be able to start as alien races if that option is enabled, but they can
	// do the other starting area options if their alignment allows. Disabling this option, at a later time, will return all
	// characters back to human upon world restart. The different values indicate the size of creatures allowed to choose
	// from. Option '1' allows for roughly human height humanoids like lizardment, ratmen, trolls, and ogres. Option '2' allows
	// for option '1' and includes larger creatures like ogre lords, ettins, and daemons. Option '3' includes the first two
	// options, but also allows for creatures such as giants and balrons. There is a more details explanation of this system
	// using the gypsy's shelf in her starting tent.

		public static int S_MonsterCharacters = 1;

	// If true, then characters will not get hungry or thirsty when inside places like banks, inns, or houses...allowing you
	// to step away from the game without your character starving eventually.

		public static bool S_Belly = true;

	// This number can be set from 5 to 60, which determines the number of minutes that hunger and thirst are checked for
	// reduction (default is 5).

		public static int S_FoodCheck = 5;

	// Below is the guild base fee to join a guild. This is the intial cost to join the first guild. If you quit, and join
	// another guild, then the fee will increase by that amount. It is based on the number of guild you were a member of
	// in the past. So if you were a member of 3 guilds, and try to join one for the 4th time, it will cost you 8,000 gold
	// (assuming a default base cost of 2,000 gold). If you set the second setting to false, then each time you join a guild,
	// it will be the flat fee every time. The minimum fee to join a guild is 200 gold.

		public static int S_GuildJoinFee = 2000;
		public static bool S_GuildIncrease = true;

	// This number is the maximum value a player character resistance can be, and it must be between 40 and 90.
	// The system default is 70. If you set this to 60, then a player can only achieve a maximum of 60 in each of the
	// resistance categories (physical, cold, fire, poison, and energy).

		public static int S_MaxResistance = 70;

	// These are values for resurrection behaviors. The first is the character level where you start to pay tribute for being
	// resurrected. Character levels range from 1 to 100, where a character cannot avoid fees at level 100. There is also
	// an amount of gold, per level of the characters, that it costs to get resurrected. This can be adjusted with the second
	// setting, and it cannot be lower than 1. If it was set to 20, and a charcter was level 10, then their resurrection cost
	// would be a base of 200 gold/tithe ( 20 x 10 ).

		public static int S_DeathPayLevel = 5;
		public static int S_DeathPayAmount = 20;

	// Spell damage toward monsters can be between 25 and 200 percent.

		public static int S_SpellDamageIncreaseVsMonsters = 150;

	// Spell damage toward player characters can be between 25 and 200 percent.

		public static int S_SpellDamageIncreaseVsPlayers = 100;

	// Maximum amount of lower reagent percentage, up to 100 for 100%. For equipment with lower reagent properties.
	// Setting it to zero will disable the attribute from the game.

		public static int S_LowerReg = 100;
		public static int S_LowerRegEnchantment = 20;

	// Maximum amount of lower mana percentage, up to 100 for 100%. For equipment with lower mana properties.
	// Setting it to zero will disable the attribute from the game.

		public static int S_LowerMana = 40;
		public static int S_LowerManaEnchantment = 8;

	// This setting between 0.5 and 3.0 (decimal format) will give a character that much hit points, mana, or stamina based
	// on the attribute. So a strength of 100 will give a character 200 hit points if this is set at 2.0.

		public static double S_PlayerLevelMod = 2.0;

	// If true, then characters will be able to set a custom title for their character in the HELP section.

		public static bool S_AllowCustomTitles = true;

	// This is the minimum and maximum gold that a player character starts with. Default is 100 and 150. The most gold a
	// character can begin with is 10,000 gold.

		public static int S_MinGold = 500;
		public static int S_MaxGold = 600;

	// If true, powerscrolls must be eaten in order 105 -> 110 -> 115 -> 120 -> 125

		public static bool S_RequirePreviousPowerscroll = true;


	///////////////////////////////////////////////////////////////////////////////////////////////
	// 004 - QUESTS & TREASURE ////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// The percent chance an item will be unidentified and no less than 10 percent.

		public static int S_GetUnidentifiedChance = 50;

	// If true, characters can only identify items (that require a double click) within their backpack.

		public static bool S_IdentifyItemsOnlyInPack = false;

	// When Unidentified items are generated, they execute a saving throw against the relevant ID skill
	// ID skills are: Arms Lore, Mercantile, and Tasting
	// 
	// Remarks:
	// - This setting executes after `S_GetUnidentifiedChance`
	// - If there are no entries, then this saving throw is not executed
	// - The default entries of 0/30/60 will give a Player with 60 skill will have a 100% chance to get 3/3 items
		public static int[] S_UnidentifiedItem_GuaranteedItemChecks = new[] 
		{
			// Each entry guarantees an item for that skill level
			// Order does not matter
			0, // Always guarantee at least one item
			30,
			60
		};

	// This setting determines the loot filled on corpses and chests. The higher the number, the more
	// often treasure will appear. The default and maximum is 100. Some enemies will generate a bit of
	// treasure themselves, like gold or reagents, which this does not affect. Chests will also likely
	// generate some monetary treasure, that has no affect from this setting.

		public static int S_LootChance = 100;

	// The amount of minutes before you can take a bulletin board quest after finishing one.

		public static int S_GetTimeBetweenQuests = 60;

	// The amount of minutes before you can take a sage artifact quest after finishing one.

		public static int S_GetTimeBetweenArtifactQuests = 20160;

	// The percent of gold you get from creatures, treasure, cargo, museum searches, shoppes, and some quests
	// between 5 (low) to 100 (high).

		public static int S_GetGoldCutRate = 35;

	// The gold reward from bulletin board quests is modified below between 0 and 250 percent. It also modifies
	// the thief note quests you get from whispering to the guildmaster, and the decorative items they steal.

		public static int S_QuestRewardModifier = 150;

	// If set to true, then characters can steal decoration artifacts as many times as they want. Otherwise,
	// they can only steal each one once.

		public static bool S_DecoArtySteal = true;

	// If set to true, then characters will only get lucrative items from a pedestal bag/box once every couple
	// of days.

		public static bool S_PedStealThrottle = false;

	// If set to true, then a character will get a warning before they are entering Skara Brae. This area is a
	// quest driven area, that has some quest requirements to be met before they can leave that area.

		public static bool S_WarnSkaraBrae = true;

	// If set to true, then a character will get a warning before they are entering the Bottle City. This area
	// is a quest driven area, that has some quest requirements to be met before they can leave that area.

		public static bool S_WarnBottleCity = true;




	///////////////////////////////////////////////////////////////////////////////////////////////
	// 005 - SKILLS ///////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// This value is between 0 and 10, where you can set the amount of extra GM skill points characters get. So if you set
	// it to '2' then characters get 200 (2 x 100) more skills points available.

		public static int S_SkillBoost = 0;

	// To use special weapon abilities, this is the minimum skill level required (weapon skill and tactics) where the
	// default is 70.0 (minimum of 20.0). Each ability requires 10 additional points above the previous (70, 80, 90, etc).

		public static double S_SpecialWeaponAbilSkill = 70.0;

	// This number can be set from zero to 10, where 10 will give characters faster skill gain and zero
	// leaves it normal (default).

		public static int S_SkillGain = 0;

	// This decimal number can be adjusted to set the maximum skill one can gain from training dummies,
	// training daemons, and archery buttes.

		public static double S_TrainDummies = 25.0;

	// This decimal number can be adjusted to set the maximum Stealing skill one can gain from pickpocket dips.

		public static double S_PickDips = 30.0;

	// This whole number (default '1') can be set from 1-100, where it multiplies the skill gain check on training
	// dummies, pickpocket dips, and archery buttes. Setting this greater than one will enable macroing.

		public static int S_TrainMulti = 1;




	///////////////////////////////////////////////////////////////////////////////////////////////
	// 006 - CRAFTING /////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// Setting the below value to true (default false) will allow one to craft 1, 10, or 100 items at a time. Skills
	// and stats will raise equatable to doing 1 or many at a time. Enabling this will also enable macroing.
	// Enabling this will also disable the items you craft that use all resources like arrows, shafts, and kindling.

		public static bool S_CraftMany = true;

	// Setting the below value to true (default false) will only apply if the S_CraftMany setting is true.
	// When viewing crafting line items, this will show the 1, 10, and 100 buttons next to each line item.
	// If you want to allow players to craft many items at once, and don't want the trade window screen
	// having many buttons on it, then leave this set to false.

		public static bool S_CraftButtons = false;

	// The below value should be set to a whole number from 1-100, where the amount of resources one gets (ore,
	// logs, hides, herbs, jars of body parts, meat, feathers, scales, wool, wheat, furs, fish, sand,
	// and blank scrolls) will be modified in some way by this value. The Isles of Dread will still have a
	// slightly more amount than the other lands as intended. Certain skills will still enhance what is found
	// too as they normally do. Increasing this from 1 is meant for games where resource gathering is to be made
	// easier, so an increase in this will also cause the harvesting skill to raise faster as well, but only when
	// successful resources are gathered (lumberjacking, mining, fishing, forensics, cooking, and inscription).
	// Setting this greater than one will enable macroing.

		public static int S_Resources = 1;

	// Increasing this number will increase the price of enhancing items with extraordinary tools that are used by
	// guild members, when next to their guildmasters. This value is a percentage increase from the base price.

		public static int S_GuildEnhanceMod = 100;

	// If false, characters will get a CAPTCHA windows occasionally to avoid unattended resource gathering with macros.

		public static bool S_AllowMacroResources = true;


	// If true, players cannot learn a recipe if their skill level is not high enough to craft the item.

		public static bool S_RecipesRequireMinSkill = false;

	// If false, only the player (or any character on their account) who acquired them can use them.

		public static bool S_RecipesForAnyone = false;

	// If true, targeting a node to harvest will repeat.
		public static bool S_AutoRepeatHarvesting = true;

	// If true, enables Legacy Inscription crafting.
		public static bool S_UseLegacyInscription = false;


	///////////////////////////////////////////////////////////////////////////////////////////////
	// 007 - MONSTERS & CREATURES /////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// If true, all creatures will have an ability to detect hidden characters based on their difficulty level.

		public static bool S_CreaturesSearching = true;

	// This is the body value for standard Wyrms. 723 is the newer, larger creatures. 12 is the classic wyrm where
	// 59 is the dragon body.

		public static int S_WyrmBody = 723;

	// This number is the percentage chance that an enemy will fail to dispel a summoned creature. This check only
	// occurs if the enemy can pass the various checks to even determine that they can dispel the summoned creature.
	// If set to zero, this check does not occur as per default. Valid values are between 10 and 90.

		public static int S_DispelFailure = 0;

	// Increasing this number will only increase the hit points of all creatures that percentage, so setting this
	// to 100 will double the hit points of creatures (adding an additional 100% on top of their normal hit points).
	// Setting it to 80 will increase their hit points by 80%...etc...

		public static int S_HPModifier = 0;

	// These values represent the percentage of difficulty increase for dungeons with the below categories. The higher
	// the number, the more difficult the monsters will become. So setting the difficult dungeons to 50, will increase
	// monsters in "difficult" dungeons by 50%. This increase affects their attributes, skills, fame, karma, and statistics.
	// Any creatures, that can be tamed, will have their taming skill requirements raised as well. Gold is also increased.
	// The default values here are 0, 30, 60, 90, and 120. Some higher level creatures will scale these values down, in
	// order for them to remain defeatable.

		public static int S_Normal = 0;
		public static int S_Difficult = 30;
		public static int S_Challenging = 60;
		public static int S_Hard = 90;
		public static int S_Deadly = 120;

	// If true, then the land will spawn random powerful daemons/balrons/dragons/wyrms/angels/etc that will spread
	// throughout the land.

		public static bool S_Scary = true;

	// These 5 settings control whether that particular land has safari animals spawn like elephants, giraffes, cheetahs, or zebras.
	// These are values between 0 and 100, where 0 never occurs and 100 always does. Setting it to '50' would be 50% of the time.

		public static int S_Safari_Sosaria = 0;		// Sosaria
		public static int S_Safari_Lodoria = 0;		// Lodoria
		public static int S_Safari_Serpent = 50;	// Serpent Island
		public static int S_Safari_Kuldar = 0;		// Kuldar
		public static int S_Safari_Savaged = 50;	// Savaged Empire

	// If true, then creatures will not be seen behind dungeon doors and walls or around corners unless they are
	// searching for blood. It only applies to creatures in dungeons, caves, or outside dangerous areas like
	// cemeteries. If you change this, then run the [buildworld command when the server restarts.

		public static bool S_LineOfSight = true;

	// If true, the purple named adventurers will attack nearby monsters and not just the characters that are
	// criminals, murderers, or have low karma.

		public static bool S_Purple = true;

	// These two settings set the overall minimum and maximum amount of minutes that a creature will respawn.
	// Creatures will respawn between the range below, and it is most effective for dungeon areas. Some of the
	// spawners (on the land) may have spawners that spawn multiple amounts of creatures. In those few cases,
	// the spawner will spawn one at a time in the time range provided below. Some creatures have a longer spawn
	// rate than most of the creatures, and those particular creatures will use this spawn rate. They will then
	// add additional minutes to reflect the longer spawn.

		public static int S_SpawnMin = 45;
		public static int S_SpawnMax = 60;


	// This settings controls the limit in seconds by which you can be paralyzed by a monster. 
	// The default is 10 seconds. It mainly affects mummies, ants, plants and spiders. Setting it to a
	// value higher than 10 could mean that the paralyze cooldown is lower than its duration, 
	// which can lead to frustrating fights as enemies can flee and chain-paralyze a character until they heal 
	// enough to get back into the fight. 
		public static double S_paralyzeDuration = 10.0;

	///////////////////////////////////////////////////////////////////////////////////////////////
	// 008 - MERCHANTS ////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// The options in this section are meant to help simulate an ecomony in the world where vendors sell and/or buy some things but not others.
	// this changes on a schedule and then randomizes what they buy/sell each time the schedule triggers. Some examples of this configuration is
	// a vendor in britain may not want to buy your leather hides, but the tanner in montor just might so your character may want to make the
	// journey there. There are also items that are meant to be hard to find (like runebooks) so you may have to visit several villages before
	// you find a vendor that sells one. Again, this is to cultivate world travel and exploration.

	// If true (default false) then vendors will sell anything they normally stock. Some items have a default rarity % that this setting does not affect.

		public static bool S_SellAll = false;

	// If true (default false) then vendors will buy anything they normally stock. Some items have a default rarity % that this setting does not affect.

		public static bool S_BuyAll = true;

	// If false, then vendors will NOT buy some tailor materials (cotton, flax, wool, regular cloth, and string).
	// Does not affect a custom merchant that is set to buy such items.

		public static bool S_BuyCloth = false;

	// If false, then vendors will not buy things from player characters. Merchant crates will also be disabled and act as normal containers.

		public static bool S_VendorsBuyStuff = true;

	// The settings below control how much gold a merchant has in order to participate in commerce. The first setting determines whether a
	// merchant is rich (true) and has no limits to buying, or is not rich (false) and has a limited budget. The second setting determines
	// if a merchant will use their remaining gold to buy an item from a player that they cannot afford. If set to false, then the merchant
	// will state that they cannot afford the item. If set to true, and you sell something like an axe for 50 gold but they only have 40 gold,
	// then they will take your axe and give you their remaining 40 gold. The last two settings are the minimum and maximum gold they will have
	// to do commerce with, and only is applied if they are not rich merchants. Each restocking period, vendors will set their gold back to these
	// default levels. As gameplay occurs, this value can increase for a vendor as you buy things from them, or pay for services they offer.

		public static bool S_RichMerchants = false;
		public static bool S_UseRemainingGold = false;
		public static int S_MinMerchant = 1000;
		public static int S_MaxMerchant = 2000;

	// If true, merchant gold will be scaled based on the facet they are on. This setting will entice players to sell to vendors beyond
	// the default facet (Sosaria).
	// WARNING: This will cause vendors to exceed the `S_MaxMerchant` setting

		public static bool S_ScaleMerchantCoins = true;
		
	// This setting is the percentage to decrease the prices of items that player characters sell to vendors. Zero disables this.

		public static int S_SellGoldCutRate = 25;

	// If true, then prices will fluctuate based on how good an item is and what stats it has, as well as the material it is made from.

		public static bool S_QualityPrices = true;

	// If true, then players can buy merchant crates to lock down in their house to sell the items they craft.

		public static bool S_MerchantCrates = true;

	// Increasing this number will increase the prices of items sold from vendors, by this percentage.
	// So a setting of 50 will increase prices by 50%.

		public static int S_PriceMore = 0;

	// Increasing this number will increase the prices of sold resources (ingots, wood, etc) by the percentage.
	// So a setting of 50 will increase resource sold prices by 50%. WARNING: This will stack with the increased
	// prices set with the above setting.

		public static int S_ResourcePrice = 500;

	// If true, then some merchants may sell large volumes of resources (ingots, ore, boards, leather, hides, cloth,
	// bottles, jars, and blank scrolls) and more types, except for non-magical resources (reagents). The resources
	// sold may appear based on their rarity and location of the merchant (verite, cherry wood, deep sea leather,
	// etc). EXAMPLE: If you can only get obsidian in the Serpent Island, then you will only find obsidian ingots
	// available for sale in that land. Those that set this to true, want a game where they would like to craft
	// items and spend gold gathering the resources more often than harvesting.

		public static bool S_SoldResource = false;

	// If true, then vendors will not buy resources you try to sell.
	// Consider leaving this true if you allow players to harvest many resources at once.
	// (bandages, flax, ingots, cloth, boards, ore, hides, arrows, bones, scales, feathers, etc).

		public static bool S_NoBuyResources = false;

	// If true, then some vendors will have a black market option in their context menus. When used, a different
	// purchasing screen will be presented to the player. They can see the special crafted items they have in
	// stock. They will only have one of such item at a time and they will restock the black market during the
	// regular stocking schedule. The resources items are made of will be land specific. EXAMPLE: If you can only 
	// get obsidian metal in the Serpent Island, then you will may only find obsidian items in that land.

		public static bool S_BlackMarket = false;

	// If true, then the custom merchant is enabled. After a [buildworld command, these merchants will appear in
	// the various settlements with their wagon. They will sell any custom items you set in the Info/Scripts folder.
	// This is in the Merchant.cs file. WARNING: Vendors can only sell 250 different items. NOTE: Many settings
	// here, that affect vendors, will not affect the custom merchant.

		public static bool S_CustomMerchant = false;




	///////////////////////////////////////////////////////////////////////////////////////////////
	// 009 - HOMES & SHIPS ////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// If true, then co-owners of houses will have the same permissions as owners. The security choice gump will
	// then specify this dual ownership when choosing an item security level. The default setting is false, where
	// co-owners have much more limited permissions as the standard game allows.

		public static bool S_HouseOwners = false;

	// When true (default setting), characters can use lawn tools (from architects) to add items to the outside
	// of their home like trees, shrubs, fences, lave, water, and other items. Lawn tools require an amount of
	// gold to place items. If this was previously true and characters placed lawn items, and then you set it to
	// false, the lawn items will refund the gold back to the character's bank box and the lawn tools will be
	// removed from the game.

		public static bool S_LawnsAllowed = true;

	// When true (default setting), characters can use remodeling tools (from architects) to add items to their
	// home like walls, doors, tiles, and other items. Remodeling tools require an amount of gold to place items.
	// If this was previously true and characters placed remodeling items, and then you set it to false, the
	// remodeling items will refund the gold back to the character's bank box and the remodeling tools will be
	// removed from the game.

		public static bool S_ShantysAllowed = true;

	// The number of days, no less than 5.0 (decimal format), that a boat or magic carpet will decay if on
	// the sea not used.

		public static double S_BoatDecay = 365.0;

	// The number of days, no less than 30.0 (decimal format), that a home will decay if an owner never shows up.

		public static double S_HomeDecay = 365.0;

	// If false, then houses never decay and the above setting is ignored.

		public static bool S_HousesDecay = false;

	// The amount of houses an account's characters may own. A -1 setting will be unlimited.

		public static int S_HousesPerAccount = 2;

	// If true, this means that the players can dye construction contracts so their pre-designed home is
	// entirely in that same color.

		public static bool S_AllowHouseDyes = false;

	// If true, then players can make use of the custom house system. Otherwise they can only purchase the
	// pre-built classic houses.

		public static bool S_AllowCustomHomes = true;

	// If true, the public basement system is active. This lets players buy basement doors for their homes
	// and basement doors will appear in some trade shops. These lead to the same basement public area and
	// is usually used for multiplayer game environments.

		public static bool S_Basements = true;

	// If true, then anything you set in you home will never decay. This makes the housing system's storage
	// capacity useless as any home can hold any amount of items, and it may convince players to never
	// bother with a castle or dungeon home because there is no storage limits on any house. But if you
	// don't want to worry about this game element in your world then you can allow players to drop things
	// on the floor without worrying about locking or securing them down. Players still need to lock items
	// down if they are going to decorate their home and they want them unmovable or able to be manipulated
	// with the homeowner tools.

		public static bool S_HouseStorage = false;




	///////////////////////////////////////////////////////////////////////////////////////////////
	// 010 - PETS, MOUNTS, & FOLLOWERS ////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// How many minutes between stat gains for pets that you can train. This can be between 1.0 to 60.0 minutes.

		public static double S_PetStatGainDelay = 5.0;

	// Increase this amount in decimal format to increase damage done to player controlled NPCs.

		public static double S_DamageToPets = 1.4;

	// Set a 0-100 percent chance enemies will get a critical double damage hit against player controlled NPCs.

		public static int S_CriticalToPets = 5;

	// If true, then pets will match the notoriety of their master. Otherwise, they will be blue or criminal when appropriate.
		public static bool S_PetsMatchMasterNotoriety = true;

	// If true, then pets will be able to harm the player who owns them.
	// Note: Pets abilities may still hurt owners unless this is disabled.
		public static bool S_PetsCanHarmOwner = false;

	// If true, then pets will be able to harm other pets from the same player.
		public static bool S_PetsCanHarmSiblingPets = true;

	// If true, some areas will not allow you to mount a creature for riding. This makes dungeons (for example)
	// more challenging. Player mounts get stabled when they go in certain areas like dungeons or caves and
	// they will remount them when they leave these areas. Set to false if you do not want to limit where they
	// take mounts. Keep in mind that having no mounts in dungeons does increase the difficulty.

		public static bool S_NoMountsInCertainRegions = true;

	// If true, then characters on mounts will dismount when they enter a building. They should mount their
	// steed again when they leave.

		public static bool S_NoMountBuilding = true;

	// If true, then characters on mounts will dismount when they enter a player character's home. They
	// should mount their steed again when they leave.

		public static bool S_NoMountsInHouses = true;

	// If true, then followers will attempt to keep up with you if you are running fast.

		public static bool S_FastFriends = true;

	// If true, then followers will not stack on top of each other but instead spread out a bit.

		public static bool S_FriendsAvoidHeels = false;

	// If true, then followers will not only guard you when commanded, but guard the other
	// followers in your group.

		public static bool S_FriendsGuardFriends = true;

	// The below setting default is '5', where this value can be between 0 and 20. This is the number of
	// extra stabled pets players get (beyond the normal amount of '2'), where anything more will rely
	// on their skills in druidism, taming, veterinary, and herding.

		public static int S_Stables = 5;

	// This number can be set from 0 to 30, which determines the number of days before you can bond
	// a pet one tamed (default is 7).

		public static int S_BondDays = 7;




	///////////////////////////////////////////////////////////////////////////////////////////////
	// 011 - TOWNS & CITIES ///////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// If true, guards will instantly kill criminal and murderer characters. Otherwise, they will chase
	// them in town where any player characters that get hit by the guards will be sent to prison and 
	// lose some equipment which is limited to stackable items like: potions, bandages, arrows, bolts, 
	// gems, coins, jewels, crystals, reagents, bottles, food, and water.

		public static bool S_GuardsSentenceDeath = false;

	// If true, guards will pay attention to enemies outside of their town borders.

		public static bool S_GuardsPatrolOutside = false;

	// If true, guards will move quicker to catch criminals but this only works if they do not
	// sentence them to death.

		public static bool S_GuardsSprint = true;

	// If true, when Players are jailed, only raw currency are deleted. Otherwise, it deletes all
	// stackable items and a bunch of other weird stuff.

		public static bool S_JailOnlyDeletesMoney = true;

	// If true, then adventurers that gather in towns may have a humanoid, pet, or summoned companion
	// with them. These gatherings are when 2-4 adventurers stand in a circle and face each other,
	// usually holding weapons and sometimes riding mounts. This setting adds a bit of fantasy world
	// atmosphere and lets players know that they too can perhaps have such a follower. There is
	// only about a 5% chance one will appear and then only 1 will appear in a group.

		public static bool S_Humanoids = true;




	///////////////////////////////////////////////////////////////////////////////////////////////
	// 012 - ACKNOWLEDGEMENT //////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////

	// If true, then it notifies the game that you have reviewed the various game settings here and
	// are confirming that you set each one to your personal play style and what you expect from the
	// game. Any settings here, that interfere with your enjoyment of the game, are under your
	// control and you can change these settings at any time if you wish to.

		public static bool S_Reviewed = true;


	}
}