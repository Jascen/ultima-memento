using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.MLQuests.Gumps;
using Server.Engines.MLQuests.Objectives;
using Server.Engines.MLQuests.Rewards;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Utilities;

namespace Server.Engines.MLQuests.Definitions
{
    #region Quests

    public class ArtifactRumor : MLQuest
    {
        public override Type NextQuest { get { return typeof(ArtifactRumorTownsfolk); } }
        public override bool MustQuitQuestChain { get { return true; } }

        public ArtifactRumor()
        {
            Activated = true;
            Title = "An arduous journey";
            Description = "Greetings, Adventurer! I've heard some information an artefact of great power. Would you like to hear it?";
            RefusalMessage = "The Sage shrugs. Don't say I didn't give you the chance.";
            InProgressMessage = "The Sage casually rub their thumb and fingers together, a small, almost teasing gesture, as if to silently signal that something is owed. Their eyes narrow slightly and a knowing smile tugs at the corner of their mouth.";
            CompletionNotice = CompletionNoticeShortReturn;

            Objectives.Add(new DummyObjective("Bribe the Sage"));
            Objectives.Add(new CollectObjective(10000, typeof(Gold), "Gold coins"));

            Rewards.Add(new DummyReward("Information about a powerful artefact"));
        }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return typeof(Sage);
        }

        public override bool CanOffer(IQuestGiver quester, PlayerMobile pm, MLQuestContext context, bool message)
        {
            if (!base.CanOffer(quester, pm, context, message)) return false;

            if (GetArtifactRumorObjectiveInstance.HasRewardItem(pm))
            {
                MLQuestSystem.Tell(quester, pm, "It looks like you're already searching for an artefact. Come back after you're done.");
                return false;
            }

            return true;
        }
    }

    public class ArtifactRumorTownsfolk : MLQuest
    {
        public override bool IsChainTriggered { get { return true; } }
        public override bool MustQuitQuestChain { get { return true; } }

        public ArtifactRumorTownsfolk()
        {
            Activated = true;
            HasRestartDelay = true; // Set after total quest completion
            Title = "An arduous journey (part 2)";
            Description = "The Sage paused for a moment, their expression shifting as if something faint had reached their ears, leaving them unsure whether it was real or just their imagination playing tricks. Y'know, you might want to speak to the townsfolk to confirm my thoughts...";
            RefusalMessage = "Very well, I understand your reluctance. Come back to me when you are ready.";
            InProgressMessage = "There is nothing I can do for you until you speak to the townsfolk and fully refine the rumor.";
            CompletionMessage = "So it is real?! I thought I might be imagining it, but now I'm sure I wasn't just making it up. Good luck on your journey!";
            CompletionNotice = CompletionNoticeShortReturn;

            Objectives.Add(new GetArtifactRumorObjective()); // Awards a Search Page

            Rewards.Add(new DummyReward("The location of a powerful artefact"));
        }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return typeof(Sage);
            yield return typeof(Citizens);
        }

        public override TimeSpan GetRestartDelay()
        {
            return TimeSpan.FromDays(3);
        }

        public override bool CanOffer(IQuestGiver quester, PlayerMobile pm, MLQuestContext context, bool message)
        {
            if (!base.CanOffer(quester, pm, context, message)) return false;
            if ((quester is Sage) == false) return false;

            if (GetArtifactRumorObjectiveInstance.HasRewardItem(pm))
            {
                MLQuestSystem.Tell(quester, pm, "It looks like you're already searching for an artefact. Come back after you're done.");
                return false;
            }

            return true;
        }
    }

    #endregion

    #region Objectives
    public class GetArtifactRumorObjective : BaseObjective
    {
        public virtual bool ShowDetailed { get { return true; } }

        public GetArtifactRumorObjective()
        {
        }

        public override void WriteToGump(Gump g, ref int y)
        {
            g.AddLabel(98, y, BaseQuestGump.COLOR_LABEL, "Speak to the townsfolk and verify the Sage's rumor");
        }

        public override BaseObjectiveInstance CreateInstance(MLQuestInstance instance)
        {
            return new GetArtifactRumorObjectiveInstance(this, instance);
        }
    }

    public class GetArtifactRumorObjectiveInstance : BaseObjectiveInstance, IDeserializable
    {
        private enum RumorType
        {
            Land = 0,
            Dungeon = 1,
            Item = 2
        }

        public Land Land { get; set; }
        public string Dungeon { get; set; }
        public int RelicNumber { get; set; }
        protected bool HasLand { get { return Land != Land.None; } }
        protected bool HasDungeon { get { return !string.IsNullOrWhiteSpace(Dungeon); } }
        protected bool HasRelicNumber { get { return 0 < RelicNumber; } }

        public GetArtifactRumorObjective Objective { get; protected set; }

        public GetArtifactRumorObjectiveInstance(GetArtifactRumorObjective objective, MLQuestInstance instance)
            : base(instance, objective)
        {
            Objective = objective;
        }

        public override bool IsCompleted()
        {
            return HasLand && HasDungeon && HasRelicNumber;
        }

        public override bool OnBeforeClaimReward()
        {
            if (HasRewardItem(Instance.Player)) return false;

            var searchBase = GetSearchBase();
            return searchBase != null;
        }

        public override void OnAfterClaimReward()
        {
            Container pack = Instance.Player.Backpack;
            if (pack == null) return;

            var searchBase = GetSearchBase();
            if (searchBase == null) return;

            var questItem = new SearchPage(Instance.Player, searchBase, RelicNumber);
            Instance.Player.AddToBackpack(questItem);

            OnQuestCancelled(); // same thing, clear other quest items
        }

        public override void WriteToGump(Gump g, ref int y)
        {
            Objective.WriteToGump(g, ref y);

            if (Objective.ShowDetailed)
            {
                base.WriteToGump(g, ref y);

                if (HasLand)
                {
                    y += 16;
                    g.AddLabel(103, y, BaseQuestGump.COLOR_LABEL, GetOrAddHint(RumorType.Land));
                }

                if (HasDungeon)
                {
                    y += 16;
                    g.AddLabel(103, y, BaseQuestGump.COLOR_LABEL, GetOrAddHint(RumorType.Dungeon));
                }

                if (HasRelicNumber)
                {
                    y += 16;
                    g.AddLabel(103, y, BaseQuestGump.COLOR_LABEL, GetOrAddHint(RumorType.Item));
                }

                if (IsCompleted())
                {
                    y += 16;
                    g.AddLabel(103, y, BaseQuestGump.COLOR_LABEL, string.Format("Return to {0}.", QuesterNameAttribute.GetQuesterNameFor(Instance.QuesterType)));
                }
            }
        }

        public static bool HasRewardItem(PlayerMobile playerMobile)
        {
            return World.Items.Values.Any(item => item is SearchPage && ((SearchPage)item).Owner == playerMobile);
        }

        public virtual bool TryGetRumor(IQuestGiver quester)
        {
            var citizen = quester as Citizens;
            if (citizen == null) return false;
            if (!citizen.CanTellRumor()) return false;
            if (IsCompleted()) return true;

            citizen.MarkToldRumor(); // Always flag the Citizen as talked to
            if (1 < Utility.RandomMinMax(1, 10)) return false; // Small chance the Citizen can help

            var hintType = !HasLand ? RumorType.Land
                : !HasDungeon ? RumorType.Dungeon
                : RumorType.Item;
            var hint = GetOrAddHint(hintType, true);

            MLQuestSystem.Tell(quester, Instance.Player, hint);

            return false;
        }

        private string GetOrAddHint(RumorType rumorType, bool forCitizen = false)
        {
            switch (rumorType)
            {
                case RumorType.Land:
                    {
                        if (!HasLand)
                        {
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
                            Land = Utility.Random(options); // Intentionally anywhere
                        }

                        var map = Lands.MapName(Land);
                        var name = Worlds.GetMyMapDisplayName(map);

                        return forCitizen
                            ? string.Format("I *think* I saw it... or maybe I didn't... wait, no, I definitely saw it! It was, like, right there... or was it? I dunno, man. I think it was in the world of {0}", name)
                            : string.Format("You must search the world of {0}.", name);
                    }

                case RumorType.Dungeon:
                    {
                        if (!HasDungeon)
                        {
                            List<SearchBase> candidates = GetCandidates(Instance.Player, Land);
                            SearchBase target = candidates[Utility.RandomMinMax(0, candidates.Count - 1)];
                            Dungeon = Worlds.GetRegionName(target.Map, target.Location);
                        }

                        return forCitizen
                            ? string.Format("I heard the artefact was in {0} and I checked it out but I didn't find anything. Maybe you'll have better luck.", Dungeon)
                            : string.Format("The artefact was somewhere in {0}.", Dungeon);
                    }

                case RumorType.Item:
                    {
                        if (!HasRelicNumber)
                        {
                            RelicNumber = Utility.RandomMinMax(1, ArtifactQuestList.MaxNumber);
                        }

                        var itemName = ArtifactQuestList.GetArtifact(RelicNumber, 1);

                        return forCitizen
                            ? string.Format("I wish you luck in acquiring the {0}.", itemName)
                            : string.Format("The artefact is {0}.", itemName);
                    }

                default:
                    Console.WriteLine("Failed to generate hint for Rumor '{0}'", rumorType);
                    return string.Format("Error: Failed to generate hint for Rumor '{0}'", rumorType);
            }
        }

        private static List<SearchBase> GetCandidates(PlayerMobile playerMobile, Land land, Func<SearchBase, bool> predicate = null)
        {
            return SearchPage.GetCandidates(playerMobile, false, item =>
                item is SearchBase && Lands.GetLand(item.Map, item.Location, item.X, item.Y) == land
                && (predicate == null || predicate(item))
            ).ToList();
        }

        private SearchBase GetSearchBase()
        {
            var destination = GetCandidates(Instance.Player, Land, item => Worlds.GetRegionName(item.Map, item.Location) == Dungeon).FirstOrDefault();
            if (destination != null) return destination;

            return null;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // Version
            writer.Write((int)Land);
            writer.Write(Dungeon);
            writer.Write(RelicNumber);
        }

        public void Deserialize(GenericReader reader)
        {
            // Base deserialize was already handled

            int version = reader.ReadInt();
            Land = (Land)reader.ReadInt();
            Dungeon = reader.ReadString();
            RelicNumber = reader.ReadInt();
        }
    }
    #endregion

}

#region Items

namespace Server.Items
{
    public class SearchPage : Item
    {
        private Mobile m_Owner;
        private string m_SearchDungeon;
        private string m_SearchItem;

        [CommandProperty(AccessLevel.GameMaster)]
        public Map GetMap { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int GetX { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int GetY { get; private set; }

        [CommandProperty(AccessLevel.Owner)]
        public int LegendPercent { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get { return m_Owner; } set { m_Owner = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.Owner)]
        public string SearchMessage { get; set; }

        [CommandProperty(AccessLevel.Owner)]
        public string SearchDungeon { get { return m_SearchDungeon; } set { m_SearchDungeon = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.Owner)]
        public Map DungeonMap { get; set; }

        [CommandProperty(AccessLevel.Owner)]
        public string SearchWorld { get; set; }

        [CommandProperty(AccessLevel.Owner)]
        public string SearchType { get; set; }

        [CommandProperty(AccessLevel.Owner)]
        public string SearchItem { get { return m_SearchItem; } set { m_SearchItem = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.Owner)]
        public int LegendReal { get; set; }

        public SearchPage(Mobile from, SearchBase searchBase, int relicNumber) : base(0x2159)
        {
            LegendPercent = 70;
            m_Owner = from;
            Weight = 1.0;
            Hue = 0x995;
            Name = "highly reliable legend for " + from.Name;

            // CHECK TO SEE IF THE NOTE IS FALSE OR TRUE
            if (LegendPercent >= Utility.RandomMinMax(1, 100)) { LegendReal = 1; }

            SearchItem = ArtifactQuestList.GetArtifact(relicNumber, 1);
            SearchType = ArtifactQuestList.GetArtifact(relicNumber, 2);

            UseSearchLocation(this, searchBase);
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);
            list.Add(1070722, SearchItem);
            list.Add(1049644, m_SearchDungeon);
            list.Add(1049644, "Discard at any time to abandon quest");
        }

        public class SearchGump : Gump
        {
            private SearchPage m_Book;
            private Map m_Map;
            private int m_X;
            private int m_Y;

            public SearchGump(Mobile from, Item parchment) : base(100, 100)
            {
                SearchPage scroll = (SearchPage)parchment;
                string sText = scroll.SearchMessage;
                from.PlaySound(0x249);

                m_Book = scroll;
                m_Map = scroll.GetMap;
                m_X = scroll.GetX;
                m_Y = scroll.GetY;

                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                AddPage(0);

                AddImage(0, 0, 10901, 2786);
                AddImage(0, 0, 10899, 2117);
                AddHtml(45, 78, 386, 218, @"<BODY><BASEFONT Color=#d9c781>" + sText + "</BASEFONT></BODY>", (bool)false, (bool)true);

                if (Sextants.HasSextant(from))
                    AddButton(377, 325, 10461, 10461, 1, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                Mobile from = state.Mobile;

                if (info.ButtonID > 0)
                {
                    from.CloseGump(typeof(Sextants.MapGump));
                    from.SendGump(new SearchGump(from, m_Book));
                    from.SendGump(new Sextants.MapGump(from, m_Map, m_X, m_Y, null));
                }
                else
                {
                    from.PlaySound(0x249);
                    from.CloseGump(typeof(Sextants.MapGump));
                }
            }
        }

        public override void OnDoubleClick(Mobile e)
        {
            if (!IsChildOf(e.Backpack))
            {
                e.SendMessage("This must be in your backpack to read.");
            }
            else
            {
                e.CloseGump(typeof(SearchGump));
                e.SendGump(new SearchGump(e, this));
            }
        }

        public static List<SearchBase> GetCandidates(PlayerMobile from, bool restrictDifficulty, Func<SearchBase, bool> predicate = null)
        {
            return World.Items.Values
                .Where(item => item is SearchBase)
                .Cast<SearchBase>()
                .Where(item => predicate == null || predicate(item))
                .Where(target => !restrictDifficulty || Server.Difficult.GetDifficulty(target.Location, target.Map) <= GetPlayerInfo.GetPlayerDifficulty(from))
                .ToList();
        }

        public static void UseRandomSearchLocation(SearchPage scroll, string DungeonNow, PlayerMobile from)
        {
            // Default
            string thisWorld = "the Land of Sosaria";
            string thisPlace = "the Dungeon of Doom";
            Map realMap = Map.Sosaria;
            Map thisMap = Map.Sosaria;

            List<SearchBase> candidates = GetCandidates(from, true);
            if (0 < candidates.Count)
            {
                SearchBase finding = candidates[Utility.RandomMinMax(0, candidates.Count - 1)];
                thisMap = Server.Misc.Worlds.GetMyDefaultMap(finding.Land);
                realMap = finding.Map;
                thisPlace = Server.Misc.Worlds.GetRegionName(finding.Map, finding.Location);
                thisWorld = Lands.LandName(finding.Land);
            }

            SetSearchLocation(scroll, thisPlace, thisWorld, thisMap, realMap);
        }

        private static void UseSearchLocation(SearchPage scroll, SearchBase item)
        {
            string itemLandName = Lands.LandName(item.Land);
            string thisPlace = Worlds.GetRegionName(item.Map, item.Location);
            Map itemMap = item.Map;
            Map baseMap = Worlds.GetMyDefaultMap(item.Land);

            SetSearchLocation(scroll, thisPlace, itemLandName, baseMap, itemMap);
        }

        private static void SetSearchLocation(SearchPage scroll, string thisPlace, string thisWorld, Map thisMap, Map realMap)
        {
            string Word1 = "Legends";
            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: Word1 = "Rumors"; break;
                case 2: Word1 = "Myths"; break;
                case 3: Word1 = "Tales"; break;
                case 4: Word1 = "Stories"; break;
            }
            string Word2 = "lost";
            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: Word2 = "kept"; break;
                case 2: Word2 = "seen"; break;
                case 3: Word2 = "taken"; break;
                case 4: Word2 = "hidden"; break;
            }
            string Word3 = "deep in";
            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: Word3 = "within"; break;
                case 2: Word3 = "somewhere in"; break;
                case 3: Word3 = "somehow in"; break;
                case 4: Word3 = "far in"; break;
            }
            string Word4 = "centuries ago";
            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: Word4 = "thousands of years ago"; break;
                case 2: Word4 = "decades ago"; break;
                case 3: Word4 = "millions of years ago"; break;
                case 4: Word4 = "many years ago"; break;
            }

            scroll.m_SearchDungeon = thisPlace;
            scroll.SearchWorld = thisWorld;
            scroll.DungeonMap = thisMap;

            Map placer;
            int xc;
            int yc;
            string EntranceLocation = Worlds.GetAreaEntrance(0, scroll.m_SearchDungeon, realMap, out placer, out xc, out yc);

            scroll.GetMap = placer;
            scroll.GetX = xc;
            scroll.GetY = yc;

            string OldMessage = "<br><br><br><br><br><br>" + scroll.SearchMessage;

            scroll.SearchMessage = scroll.SearchItem + "<br><br>" + Word1 + " tell of the " + scroll.SearchItem + " being " + Word2 + " " + Word3;
            scroll.SearchMessage = scroll.SearchMessage + " " + scroll.m_SearchDungeon + " " + Word4 + " by " + QuestCharacters.QuestGiver() + ".";
            scroll.SearchMessage = scroll.SearchMessage + " in " + scroll.SearchWorld + " at the below sextant coordinates.<br><br>" + EntranceLocation + OldMessage;

            scroll.InvalidateProperties();
        }
        public SearchPage(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2);

            writer.Write(GetMap);
            writer.Write(GetX);
            writer.Write(GetY);

            writer.Write((Mobile)m_Owner);
            writer.Write(SearchMessage);
            writer.Write(m_SearchDungeon);
            writer.Write(DungeonMap);
            writer.Write(SearchWorld);
            writer.Write(SearchType);
            writer.Write(SearchItem);
            writer.Write(LegendReal);
            writer.Write(LegendPercent);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    {
                        GetMap = reader.ReadMap();
                        GetX = reader.ReadInt();
                        GetY = reader.ReadInt();
                        break;
                    }
            }

            m_Owner = reader.ReadMobile();
            SearchMessage = reader.ReadString();
            m_SearchDungeon = reader.ReadString();
            DungeonMap = reader.ReadMap();
            SearchWorld = reader.ReadString();
            SearchType = reader.ReadString();
            SearchItem = reader.ReadString();
            LegendReal = reader.ReadInt();
            LegendPercent = reader.ReadInt();
            ItemID = 0x2159;
            Hue = 0x995;
        }
    }

    [Flipable(0x577B, 0x577C)]
    public class SearchBoard : Item
    {
        [Constructable]
        public SearchBoard() : base(0x577B)
        {
            Weight = 1.0;
            Name = "Sage Advice";
            Hue = 0x986;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("The Search for Artifacts");
        }

        public override void OnDoubleClick(Mobile e)
        {
            if (e.InRange(this.GetWorldLocation(), 4))
            {
                e.CloseGump(typeof(BoardGump));
                e.SendGump(new BoardGump(e, "SAGE ADVICE", "If you have a grand quest to unearth an artifact, you can seek the advice of sages in your journey. Their advice is not cheap, they charge 10,000 gold for the guidance. To begin your quest, visit one of the many sages in the land and give them enough gold for their advice. They will give you an artifact encyclopedia from which you can search for the first clues on the whereabouts of your artifact.<br><br>Sages are never able to give you absolute accurate information on the location of an artifact. Once you receive your encyclopedia, open it up and choose an artifact from its many pages. If you are not sure what artifact you seek, simply look through the Sage's wares for sale. At the end of their inventory, you will see research replicas of these artifacts priced at zero gold. You can hover over these artifacts to see what they may offer you, but you cannot buy them. Artifacts such as books, quivers, and instruments will be shown with some common and random qualities, where finding the actual artifact will have somewhat different properties. The remaining items have set qualities as well as a number of Enchantment Points that you can spend to make the artifact more customized for yourself. When you find these artifacts, single click them and select the Enchant option to spend the points on the additional attributes you want. After selecting an artifact from the book, you will tear out the appropriate page and toss out the remainder of the book. This page will give you your first clue on where to search. Areas the artifact may be in could span many different lands or worlds, where some you may have never been or heard of. You will be provided with the coordinates of the place you seek, so make sure you have a sextant with you.<br><br><br><br>Throughout history, many people kept these artifacts stored on blocks of crafted stone. These crafted stones are often decorated with a symbol on the surface, where a metal chest rests and the item may be inside. Some treasure hunters find the chests empty, realizing the legends were false. If nothing else, you may find a large sum of gold to cover some of your expenses on this journey. Some may provide a new clue on where the artifact is, and you will update your notes when these clues are found. The most disappointing search may yield a fake artifact. These turn out to be useless items that simply look like the artifact you were searching for. <br><br><br><br>These quests are quite involved and you may only participate in one such quest at a time. If you have not finished a quest, and try to seek a sage for another, you will find that the page of your prior quest will have gone missing. It would have been surely lost somewhere. If you finish a quest, either with success or failure, a sage will not have any new advice for you for quite some time so you will have to wait until then to begin a new quest. So good luck treasure hunter, and may the gods aid you in your journey.", "#d3d307", true));
            }
            else
            {
                e.SendLocalizedMessage(502138); // That is too far away for you to use
            }
        }

        public SearchBoard(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}

#endregion