using System;
using System.Collections;
using System.Collections.Generic;
using Server.Engines.MLQuests;
using Server.Engines.MLQuests.Gumps;
using Server.Engines.MLQuests.Objectives;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
using Server.Regions;

namespace Server.Engines_and_Systems.Quests.BulletinBoard.Objectives
{
    public class RandomAdventuringObjective : BaseObjective
    {
        public RandomAdventuringObjective()
        {
        }

        public override void WriteToGump(Gump g, ref int y)
        {
            // Ignored, as we return a radiant instance
            return;
        }

        public override BaseObjectiveInstance CreateInstance(MLQuestInstance instance)
        {
            return new RandomAdventuringObjectiveInstance(instance, this);
        }
    }

    public class RandomAdventuringObjectiveInstance : RadiantObjectiveInstance, IDeserializable
    {
        public string EncodedQuestString { get; set; }
        
        private readonly PlayerMobile m_Player;

        private const int Version = 1;

        /// <summary>
        /// Constructor for building the 'actual' quest instance the player will receive on pressing 'Accept'.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="obj"></param>
        public RandomAdventuringObjectiveInstance(MLQuestInstance instance, BaseObjective obj) : base(instance, obj)
        {
            m_Player = instance.Player;
            
            // Build the actual quest objective
            int nFame = m_Player.Fame * 2;
            nFame = Utility.RandomMinMax( 0, nFame )+2000;
            EncodedQuestString = BuildQuestString(nFame);
        }

        public override void WriteToGump(Gump g, ref int y)
        {
            TextDefinition.AddHtmlText(g, 98, y, 312, 260, GetObjectiveText(), false, false, BaseQuestGump.COLOR_LOCALIZED, BaseQuestGump.COLOR_HTML);
        }
        
        

        #region Quest Strings
        
        private string BuildQuestString(int fee)
        {
            var player = m_Player;
            string encodedQuestString = null;
            var options = new List<Land>
            {
                Land.Sosaria,
                Land.Sosaria,
                Land.Sosaria,
                Land.Lodoria,
                Land.Lodoria,
                Land.Lodoria,
                Land.Serpent,
                Land.Serpent,
                Land.Serpent,
                Land.IslesDread,
                Land.Savaged,
                Land.Savaged,
                Land.UmberVeil,
                Land.Kuldar,
                Land.Underworld,
                Land.Ambrosia,
            };
            Land searchLocation = PlayerSettings.GetRandomDiscoveredLand(player, options, null);

            int aCount = 0;
            Region reg = null;
            ArrayList targets = new ArrayList();
            foreach (Mobile target in World.Mobiles.Values)
                if (target is BaseCreature)
                {
                    reg = Region.Find(target.Location, target.Map);
                    Land tWorld = Server.Lands.GetLand(target.Map, target.Location, target.X, target.Y);

                    if (target.EmoteHue != 123 && target.Karma < 0 && target.Fame < fee &&
                        (Server.Difficult.GetDifficulty(target.Location, target.Map) <=
                         GetPlayerInfo.GetPlayerDifficulty(player)) && reg.IsPartOf(typeof(DungeonRegion)))
                    {
                        if (searchLocation == Land.Sosaria && tWorld == searchLocation)
                        {
                            targets.Add(target);
                            aCount++;
                        }
                        else if (searchLocation == Land.Lodoria && tWorld == searchLocation)
                        {
                            targets.Add(target);
                            aCount++;
                        }
                        else if (searchLocation == Land.Serpent && tWorld == searchLocation)
                        {
                            targets.Add(target);
                            aCount++;
                        }
                        else if (searchLocation == Land.IslesDread && tWorld == searchLocation)
                        {
                            targets.Add(target);
                            aCount++;
                        }
                        else if (searchLocation == Land.Savaged && tWorld == searchLocation)
                        {
                            targets.Add(target);
                            aCount++;
                        }
                        else if (searchLocation == Land.UmberVeil && tWorld == searchLocation)
                        {
                            targets.Add(target);
                            aCount++;
                        }
                        else if (searchLocation == Land.Kuldar && tWorld == searchLocation)
                        {
                            targets.Add(target);
                            aCount++;
                        }
                        else if (searchLocation == Land.Underworld && tWorld == searchLocation)
                        {
                            targets.Add(target);
                            aCount++;
                        }
                    }

                    if (aCount < 1) // SAFETY CATCH IF IT FINDS NO CREATURES AT ALL...IT WILL FIND AT LEAST ONE IN SOSARIA //
                    {
                        if (target.Karma < 0 && target.Fame < fee && reg.IsPartOf(typeof(DungeonRegion)) &&
                            tWorld == Land.Sosaria)
                        {
                            targets.Add(target);
                            aCount++;
                        }
                    }
                }

            aCount = Utility.RandomMinMax(1, aCount);

            int xCount = 0;
            for (int i = 0; i < targets.Count; ++i)
            {
                xCount++;

                if (xCount == aCount)
                {
                    if (Utility.RandomMinMax(1, 2) == 1) // KILL SOMETHING
                    {
                        Mobile theone = (Mobile)targets[i];
                        string kWorld =
                            Server.Lands.LandName(Server.Lands.GetLand(theone.Map, theone.Location, theone.X,
                                theone.Y));

                        string kexplorer = theone.GetType().ToString();
                        int nFee = theone.Fame / 5;
                        nFee = (int)((MyServerSettings.QuestRewardModifier() * 0.01) * nFee) + 20 + nFee;
                        string kDollar = nFee.ToString();

                        string killName = theone.Name;
                        string killTitle = theone.Title;
                        if (theone is Wyrms)
                        {
                            killName = "a wyrm";
                            killTitle = "";
                        }

                        if (theone is Daemon)
                        {
                            killName = "a daemon";
                            killTitle = "";
                        }

                        if (theone is Balron)
                        {
                            killName = "a balron";
                            killTitle = "";
                        }

                        if (theone is RidingDragon || theone is Dragons)
                        {
                            killName = "a dragon";
                            killTitle = "";
                        }

                        if (theone is BombWorshipper)
                        {
                            killName = "a worshipper of the bomb";
                            killTitle = "";
                        }

                        if (theone is Psionicist)
                        {
                            killName = "a psychic of the bomb";
                            killTitle = "";
                        }

                        string myexplorer = kexplorer + "#" + killTitle + "#" + killName + "#" +
                                            Server.Misc.Worlds.GetRegionName(theone.Map, theone.Location) + "#0#" +
                                            kDollar + "#" + kWorld + "#Monster";
                        string theStory =
                            myexplorer + "#" + QuestSentence(myexplorer); // ADD THE STORY PART

                        encodedQuestString = theStory;
                    }
                    else // FIND SOMETHING
                    {
                        Mobile theone = (Mobile)targets[i];
                        string kWorld =
                            Server.Lands.LandName(Server.Lands.GetLand(theone.Map, theone.Location, theone.X,
                                theone.Y));

                        string kexplorer = theone.GetType().ToString();
                        int nFee = theone.Fame / 3;
                        nFee = (int)((MyServerSettings.QuestRewardModifier() * 0.01) * nFee) + 20 + nFee;
                        string kDollar = nFee.ToString();

                        string ItemToFind = QuestCharacters.QuestItems(true);

                        string myexplorer = "##" + ItemToFind + "#" +
                                            Server.Misc.Worlds.GetRegionName(theone.Map, theone.Location) + "#0#" +
                                            kDollar + "#" + kWorld + "#Item";
                        string theStory =
                            myexplorer + "#" + QuestSentence(myexplorer); // ADD THE STORY PART

                        encodedQuestString = theStory;
                    }
                }
            }

            return encodedQuestString;
        }

        private string QuestSentence(string info)
        {
            string sMainQuest = "";

            string explorer = info;
            
            string sPCTarget = "";
            string sPCTitle = "";
            string sPCName = "";
            string sPCRegion = "";
            int nPCDone = 0;
            int nPCFee = 0;
            string sPCWorld = "";
            string sPCCategory = "";
            string sPCStory = "";

            string[] explorers = explorer.Split('#');
            int nEntry = 1;
            foreach (string explorerz in explorers)
            {
                if (nEntry == 1)
                {
                    sPCTarget = explorerz;
                }
                else if (nEntry == 2)
                {
                    sPCTitle = explorerz;
                }
                else if (nEntry == 3)
                {
                    sPCName = explorerz;
                }
                else if (nEntry == 4)
                {
                    sPCRegion = explorerz;
                }
                else if (nEntry == 5)
                {
                    nPCDone = Convert.ToInt32(explorerz);
                }
                else if (nEntry == 6)
                {
                    nPCFee = Convert.ToInt32(explorerz);
                }
                else if (nEntry == 7)
                {
                    sPCWorld = explorerz;
                }
                else if (nEntry == 8)
                {
                    sPCCategory = explorerz;
                }
                else if (nEntry == 9)
                {
                    sPCStory = explorerz;
                }

                nEntry++;
            }

            string sWorth = nPCFee.ToString("#,##0");
            string sTheyCalled = sPCName;
            if (sPCTitle.Length > 0)
            {
                sTheyCalled = sPCTitle;
            }

            string sGiver = QuestCharacters.QuestGiverKarma(m_Player.KarmaLocked);

            string sWord1 = "you";
            switch (Utility.RandomMinMax(0, 4))
            {
                case 0:
                    sWord1 = "a brave adventurer";
                    break;
                case 1:
                    sWord1 = "an adventurer";
                    break;
                case 2:
                    sWord1 = "you";
                    break;
                case 3:
                    sWord1 = "someone";
                    break;
                case 4:
                    sWord1 = "one willing";
                    break;
            }

            string sWord2 = "go to";
            switch (Utility.RandomMinMax(0, 4))
            {
                case 0:
                    sWord2 = "go to";
                    break;
                case 1:
                    sWord2 = "travel to";
                    break;
                case 2:
                    sWord2 = "journey to";
                    break;
                case 3:
                    sWord2 = "seek out";
                    break;
                case 4:
                    sWord2 = "venture to";
                    break;
            }

            string sWord3 = "kill";

            if (sPCCategory == "Item")
            {
                switch (Utility.RandomMinMax(0, 3))
                {
                    case 0:
                        sWord3 = "find";
                        break;
                    case 1:
                        sWord3 = "seek";
                        break;
                    case 2:
                        sWord3 = "search for";
                        break;
                    case 3:
                        sWord3 = "bring back";
                        break;
                }
            }
            else
            {
                switch (Utility.RandomMinMax(0, 3))
                {
                    case 0:
                        sWord3 = "eliminate";
                        break;
                    case 1:
                        sWord3 = "slay";
                        break;
                    case 2:
                        sWord3 = "kill";
                        break;
                    case 3:
                        sWord3 = "destroy";
                        break;
                }
            }

            sMainQuest = sGiver + " wants " + sWord1 + " to " + sWord2 + " " + sPCRegion + " in " + sPCWorld + " and " +
                         sWord3 + " " + sTheyCalled + " for " + sWorth + " gold";
            return sMainQuest;
        }

        public string GetObjectiveText()
        {
            string encodedInfo = EncodedQuestString;
            
            string sPCTarget = "";
            string sPCTitle = "";
            string sPCName = "";
            string sPCRegion = "";
            int nPCDone = 0;
            int nPCFee = 0;
            string sPCWorld = "";
            string sPCCategory = "";
            string sPCStory = "";

            string[] explorers = encodedInfo.Split('#');
            int nEntry = 1;
            foreach (string explorerz in explorers)
            {
                if ( nEntry == 1 ){ sPCTarget = explorerz; }
                else if ( nEntry == 2 ){ sPCTitle = explorerz; }
                else if ( nEntry == 3 ){ sPCName = explorerz; }
                else if ( nEntry == 4 ){ sPCRegion = explorerz; }
                else if ( nEntry == 5 ){ nPCDone = Convert.ToInt32(explorerz); }
                else if ( nEntry == 6 ){ nPCFee = Convert.ToInt32(explorerz); }
                else if ( nEntry == 7 ){ sPCWorld = explorerz; }
                else if ( nEntry == 8 ){ sPCCategory = explorerz; }
                else if ( nEntry == 9 ){ sPCStory = explorerz; }

                nEntry++;
            }

            return sPCStory;
        }
        
        #endregion

        public void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();
            EncodedQuestString = reader.ReadString();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Version);
            writer.Write(EncodedQuestString);
        }
    }
}