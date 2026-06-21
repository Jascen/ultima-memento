using System;
using System.Collections.Generic;
using System.Text;
using Server.Engines.Craft;
using Server.Engines.MLQuests.Objectives;
using Server.Engines.MLQuests.Rewards;
using Server.Engines.MLQuests.Utilities;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.MLQuests.Definitions
{
    #region Quests

    /*
    [go 2981 1023
    [go 3155 2600
    [go 1612 1451
    [go 2478 890
    [go 856 712
    [go 917 2097
    */

    public class RingArmorQuest : MLQuest
    {
        public class HintQuest : MLQuest
        {
            public override bool IsChainTriggered { get { return true; } }
            public override Type NextQuest { get { return typeof(RingArmorQuest); } }

            public HintQuest()
            {
                Activated = true;
                Title = "Delivery: The Hammer and Anvil";
                Description = "TODO: Deliver this to the Britain Smith";

                Objectives.Add(new DeliverObjective(typeof(BlacksmithDeliveryCrate), 1, typeof(BritainGuildmasterSmithGuy)));
                Objectives.Add(new DummyObjective("- Blacksmith Crate"));
                Objectives.Add(new DummyObjective("- Location: Britain"));
                Objectives.Add(new DummyObjective(""));
                Objectives.Add(new DummyObjective("Tips:"));
                DeliveryQuestUtilities.AddBeginnerTravelTips(this);

                Rewards.Add(new ItemReward("Gold Coins", typeof(Gold), 300));
            }

            public override IEnumerable<Type> GetQuestGivers()
            {
                yield break;
            }
        }

        public override bool IsChainTriggered { get { return false; } } // Hint quest is optional
        public override Type NextQuest { get { return typeof(ChainArmorQuest.HintQuest); } } // Optional delivery

        public RingArmorQuest()
        {
            Activated = true;
            Title = "The Hammer's Return";
            var builder = new StringBuilder();
            builder.Append("The Smith looks busy and irritated as you approach. \"Teach you, ha! Not with MY customers at stake. Of course, I'm a Guildmaster - but do you think we just let anyone who asks into the guild?\"");
            builder.Append("<br><br>");
            builder.Append("The Smith points a finger at the display cases over yonder. \"Make me something useful first. You CAN make something useful, right?\" The Smith stares at you and waits impatiently for your stammering reply.");
            builder.Append("<br><br>");
            builder.Append("\"Look, I've no time for another apprentice. I've got some notes on the basics of ringmail armor that you can pore over, but you'll have to make it worth my while first.\" ");
            builder.Append("<br><br>");
            builder.Append("The Smith quickly scans the shelves and crates before acknowledging you again. \"I'm running low on shield inventory and empty stock makes for unhappy customers when they come knockin'.\"");
            builder.Append("<br><br>");
            builder.Append("\"Try your hand at a bunch of Tear Kite Shields and I'll see what's good enough to sell.\" With that, the Smith turns away and returns to work.");
            Description = builder.ToString();

            Objectives.Add(DummyObjective.CraftAndMarkQuestItems);
            Objectives.Add(new CraftObjective(20, typeof(WoodenKiteShield), "Tear Kite Shield"));
            Objectives.Add(new DummyObjective(""));
            Objectives.Add(new DummyObjective("* Click yourself to view your Quest Log"));
            Objectives.Add(new DummyObjective(""));
            Objectives.Add(new DummyObjective("Tips:"));
            Objectives.Add(new DummyObjective("- Equip your smith hammer and double click"));
            Objectives.Add(new DummyObjective("it to use it"));
            Objectives.Add(new DummyObjective("- Many tools have been developed to assist"));
            Objectives.Add(new DummyObjective("you in your journey. Click 'Help' on your Paperdoll."));

            builder.Clear();
            builder.Append("Mark the shields as a quest item when you are ready.<br><br>");
            builder.Append("- Click yourself to view your Quest Log<br>");
            builder.Append("- Click the reticle next to the quest<br>");
            builder.Append("- Target a container or the shields directly<br>");
            builder.Append("- Return to the Britain Blacksmith");
            InProgressMessage = builder.ToString();

            builder.Clear();
            builder.Append("You do your best to flag down the busy Smith and finally succeed.");
            builder.Append("<br><br>");
            builder.Append("\"Alrighty, let's see what you've come up with.\" You present your wares proudly as the Smith dons some spectacles. He swiftly grades the shields for quality: rapping them inside and out, peering at the stud work, and studying the metal form.");
            builder.Append("<br><br>");
            builder.Append("As the sellable product is sorted into piles, the Smith looks up, slightly surprised to see you still standing there. \"Hmm? What is it now?\" As you begin to request the aforementioned ringmail documents, you are interrupted.");
            builder.Append("<br><br>");
            builder.Append("\"Ah, yes. Those silly papers.\" The Smith shakes his head and walks over to the nearby shelf.");
            builder.Append("<br><br>");
            builder.Append("\"...Here.\" You quickly snatch the bag as he tosses it at your head. The Smith ignores you as he resumes work in his shop.");
            CompletionMessage = builder.ToString();

            Rewards.Add(new ConstructibleItemReward("Ringmail Armor Recipes",
                player =>
                {
                    return DefBlacksmithy.CraftSystem.AddRecipeScrolls(
                        player,
						new FancyStoneChest
						{
							Name = "Ringmail Armor Recipes",
							Hue = Server.Utilities.HueUtilities.RandomBrightMetalHue()
						},
                        typeof(RingmailGloves),
                        typeof(RingmailLegs),
                        typeof(RingmailArms),
                        typeof(RingmailChest),
                        typeof(RingmailSkirt)
                    );
                })
            );
        }

        public override Type QuestRecipient { get { return typeof(BritainGuildmasterSmithGuy); } }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return QuestRecipient;  // Quest Giver & Recipient
        }

        public override void Generate()
        {
            base.Generate();

            PutSpawner(new Spawner(1, 5, 10, 0, 0, QuestRecipient.Name), new Point3D(2981, 1023, 0), Map.Sosaria);
        }
    }

    public class ChainArmorQuest : MLQuest
    {
        public class HintQuest : MLQuest
        {
            public override bool IsChainTriggered { get { return true; } }
            public override Type NextQuest { get { return typeof(ChainArmorQuest); } }

            public HintQuest()
            {
                Activated = true;
                Title = "Delivery: Metals of Montor";
                var builder = new StringBuilder();
                builder.Append("\"You again? Still looking to hone your craft, eh? No, no, we're too busy here.\" Intentional chatter fills the air - the shop is bustling. Apprentices move about addressing customer requests, mending armor, and sharpening weapons. You briefly step to the side as someone shoulders past.");
                builder.Append("<br><br>");
                builder.Append("\"Here, I've got an idea that should satisfy us both.\" Clang! A muffled jingle of metal smacking together jars you as the Smith plops a heavy crate down on the floor in front of you.");
                builder.Append("<br><br>");
                builder.Append("\"Take this package due south to the Montor blacksmith and I'm sure someone there will be delighted to teach you something more useful.\"");
                builder.Append("<br><br>");
                builder.Append("The Smith turns away, already forgetting you and talking to himself, \"...cheaper than whatever animals they decide to use as couriers these days at Beasts of Burden...those prices are highway robbery...whetstone wheel acting up again...\"");
                Description = builder.ToString();

                builder.Clear();
                builder.Append("\"Ay', mate! I been expectin' d'ose!\" The Smith stops what she's doing to grab the crate right out of your hands.");
                builder.Append("<br><br>");
                builder.Append("She sets the crate down [clang!], flips through a few items [ting...ting...ting...], and rummages through to find a shield and katana. She eyes each of her prizes proudly and grins.");
                builder.Append("<br><br>");
                builder.Append("\"About time!\" [swoosh!] [swoosh!] You quickly step back, dodging wild swings, and eye her with bewilderment. The Smith giggles to herself. \"Ma always said I was safer swingin' hammers!\"");
                builder.Append("<br><br>");
                builder.Append("\"Ye have my d'anks!\"");
                CompletionMessage = builder.ToString();

                Objectives.Add(new DeliverObjective(typeof(BlacksmithDeliveryCrate), 1, typeof(MontorSmithGirl)));
                Objectives.Add(new DummyObjective("- Blacksmith Crate"));
                Objectives.Add(new DummyObjective("- Location: Montor"));
                Objectives.Add(new DummyObjective(""));
                Objectives.Add(new DummyObjective("Tips:"));
                DeliveryQuestUtilities.AddBeginnerTravelTips(this);

                Rewards.Add(new ItemReward("Gold Coins", typeof(Gold), 300));
            }

            public override IEnumerable<Type> GetQuestGivers()
            {
                yield break;
            }
        }

        public override bool IsChainTriggered { get { return false; } } // Hint quest is optional
        public override Type NextQuest { get { return typeof(PlateArmorQuest.HintQuest); } }

        public ChainArmorQuest()
        {
            Activated = true;
            Title = "The Heart of the Forge";
            var builder = new StringBuilder();
            builder.Append("\"Oi! M'good pal! M'good buddy! M'good friend!\" The Smith sees you enter the shop and a warm smile breaks her face. She immediately approaches you.");
            builder.Append("<br><br>");
            builder.Append("\"What can'r do ye fer?\" She looks at you expectantly and listens to your requests, clapping her hands at last.");
            builder.Append("<br><br>");
            builder.Append("\"Aha! Ye've come to da right joint.\" She walks to her coffer and pulls out a stack of scrolls - waving them at you.");
            builder.Append("<br><br>");
            builder.Append("\"Sure, sure, yer good fer d'ese.\" She waggles a finger at you teasingly, with a sparkle in her eye. \"But only if'n ye prove its,\" she says, as she plops the stack of scrolls back into her coffer.");
            builder.Append("<br><br>");
            builder.Append("\"Chainmail is a might challenge more den ringmail, eh.\" With a grin, she says, \"Show me yer skill with'n da gloves and leggin's!\"");
            Description = builder.ToString();

            Objectives.Add(DummyObjective.CraftAndMarkQuestItems);
            Objectives.Add(new CraftObjective(15, typeof(RingmailGloves), "Ringmail Gloves"));
            Objectives.Add(new CraftObjective(15, typeof(RingmailLegs), "Ringmail Leggings"));
            Objectives.Add(new DummyObjective(""));
            Objectives.Add(new DummyObjective("Tips:"));
            Objectives.Add(new DummyObjective("- Resistances are vital to your survival"));
            Objectives.Add(new DummyObjective("- Fistfighting and other weapon skills reduce your chance"));
            Objectives.Add(new DummyObjective("to be hit"));
            Objectives.Add(new DummyObjective("- Exceptionally crafted weapons and armor both"));
            Objectives.Add(new DummyObjective("receive a single random magical property"));
            Objectives.Add(new DummyObjective("- Exceptionally crafted armor have more total resists"));
            Objectives.Add(new DummyObjective("- Exceptionally crafted weapons deal more damage"));
            Objectives.Add(new DummyObjective("- The Arms Lore skill increases the exceptional benefit"));

            builder.Clear();
            builder.Append("The smith looks up from her work in surprise. \"Done, hm? Swift, yes, yes. I be seein' ye workin'.\"");
            builder.Append("<br><br>");
            builder.Append("\"Give'r 'ere, c'mon. Lemmee see dat.\" She works methodically to drape the gloves out across a few tables and grabs the leggings to hang on a rack. With her pliers suddenly in hand, she pulls on a few (seemingly) random rings, testing for shoddy workmanship.");
            builder.Append("<br><br>");
            builder.Append("\"Fine work, yes! Mmm, tis good.\" She drops the tools, walks away and returns with a not-insignificantly-sized blade in hand. Recalling your last near-miss involving a katana, you take a step back and a knowing grin splits her face. \"I says, yer a quick study.\"");
            builder.Append("<br><br>");
            builder.Append("With a quick snap of the wrist, the blade strikes the ringmail. \"Tend the diff'rence - rings expose more skin!\" Her other hand briefly hovers above the table, her open grasp suddenly releasing a stack of scrolls to scatter chainmail patterns across your view.");
            builder.Append("<br><br>");
            builder.Append("\"Yer ready... good luck.\"");
            CompletionMessage = builder.ToString();

            Rewards.Add(new ConstructibleItemReward("Chain Armor Recipes",
                player =>
                {
                    return DefBlacksmithy.CraftSystem.AddRecipeScrolls(
                        player,
						new FancyStoneChest
						{
							Name = "Chain Armor Recipes",
							Hue = Server.Utilities.HueUtilities.RandomBrightMetalHue()
						},
                        typeof(ChainCoif),
                        typeof(ChainLegs),
                        typeof(ChainChest),
                        typeof(ChainSkirt)
                    );
                })
            );
        }

        public override Type QuestRecipient { get { return typeof(MontorSmithGirl); } }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return QuestRecipient;  // Quest Giver & Recipient
        }

        public override void Generate()
        {
            base.Generate();

            PutSpawner(new Spawner(1, 5, 10, 0, 0, QuestRecipient.Name), new Point3D(3155, 2600, 5), Map.Sosaria);
        }
    }

    public class PlateArmorQuest : MLQuest
    {
        public class HintQuest : MLQuest
        {
            public override bool IsChainTriggered { get { return true; } }
            public override Type NextQuest { get { return typeof(PlateArmorQuest); } }

            public HintQuest()
            {
                Activated = true;
                Title = "Delivery: Forged Iron";

                var builder = new StringBuilder();
                builder.Append("\"Plate mail, hmmm?! If'n yer up fer rivalin' me, ye betta step it up. Yer good, but not dat good.\" The smith pokes you in the chest with a gnarly finger and raises an eyebrow at you. She turns away briefly and then spreads map out on the table.");
                builder.Append("<br><br>");
                builder.Append("\"Time fer findin' safety in da mountains - Devil Guard lies northwest o' here. Take me goods with ye.\" She stuffs a crate in your arms and smacks you on the butt.");
                builder.Append("<br><br>");
                builder.Append("\"Get yer tookus movin'!\"");
                builder.Append("<br><br>");
                Description = builder.ToString();

                Objectives.Add(new DeliverObjective(typeof(BlacksmithDeliveryCrate), 1, typeof(DevilGuardSmithGuy)));
                Objectives.Add(new DummyObjective("- Blacksmith Crate"));
                Objectives.Add(new DummyObjective("- Location: Devil Guard"));
                Objectives.Add(new DummyObjective(""));
                Objectives.Add(new DummyObjective("Tips:"));
                DeliveryQuestUtilities.AddBeginnerTravelTips(this);

                Rewards.Add(new ItemReward("Gold Coins", typeof(Gold), 300));

                builder.Clear();
                builder.Append("\"STOP!\" A stern, but even voice commands you. The blacksmith looks up, staring at you with icy blue eyes, pausing his hammer strokes just long enough to visually scold you. Softly, but with a tone used to being followed, he asserts: \"Wipe your boots at the door.\" *clang clang clang*");
                builder.Append("<br><br>");
                builder.Append("\"Leave the crate beside the scrap iron barrel, and do not disturb a thing in my shop.\" The smith doesn't bother to look up from his work this time.");
                builder.Append("<br><br>");
                builder.Append("");
                builder.Append("<br><br>");
                CompletionMessage = builder.ToString();
            }

            public override IEnumerable<Type> GetQuestGivers()
            {
                yield break;
            }
        }

        public override bool IsChainTriggered { get { return false; } } // Hint quest is optional
        public override Type NextQuest { get { return typeof(AnimalArmorQuest.HintQuest); } }

        public PlateArmorQuest()
        {
            Activated = true;
            Title = "A Test of Strength";

            var builder = new StringBuilder();
            builder.Append("\"What is it that you want?\" The smith softly inquires, noticing that you haven't yet left. Again, the smith doesn't bother to look up from his work. *stroke, stroke, stroke - turn - stroke, stroke, stroke - into the coals*");
            builder.Append("<br><br>");
            builder.Append("You look to his left, mesmerized by the mirror finish on various pieces of platemail hanging beside him. A sniff breaks the spell.");
            builder.Append("<br><br>");
            builder.Append("\"Huh. A fellow tradesman? Yes, I'll teach you, though plate is in less demand 'round these parts. Demonstrate your capability with a chainmail coif and tunic...or 15.\" He finally looks up, a piercing blue gaze daring you to complain.\"");
            builder.Append("<br><br>");
            Description = builder.ToString();

            Objectives.Add(DummyObjective.CraftAndMarkQuestItems);
            Objectives.Add(new CraftObjective(15, typeof(ChainCoif), "Chainmail Coif"));
            Objectives.Add(new CraftObjective(15, typeof(ChainChest), "Chainmail Tunic"));
            Objectives.Add(new DummyObjective(""));
            Objectives.Add(new DummyObjective("Tips:"));
            Objectives.Add(new DummyObjective("- Hit Point Regen can help with damage from being poisoned"));
            Objectives.Add(new DummyObjective("- Stats (Strength, Dexterity, and Intelligence) have a hard"));
            Objectives.Add(new DummyObjective("cap of 150, regardless of bonuses"));
            Objectives.Add(new DummyObjective("- You can permanently increase Stats by raising your skills"));
            Objectives.Add(new DummyObjective("- You can temporarily increase Stats with potions, spells,"));
            Objectives.Add(new DummyObjective("and equipment"));

            builder.Clear();
            builder.Append("\"Aye. See my chainmail over yonder?\" With a short motion, the head of his hammer briefly guides your gaze before returning back to the metal. *clang clang clang*");
            builder.Append("<br><br>");
            builder.Append("\"Go, take your wares, and compare your quality against mine. Do. Not. Touch. A. Thing. When you are ready, I wish to see every flaw in your workmanship. You will show me how you have room for improvement in every piece.\"");
            builder.Append("<br><br>");
            builder.Append("...you complete the arduous task, humbled by the methodical activity, but excited by the growth opportunity.");
            builder.Append("<br><br>");
            builder.Append("\"Very well. Take these platemail recipes.\"");
            CompletionMessage = builder.ToString();

            Rewards.Add(new ConstructibleItemReward("Plate Armor Recipes",
                player =>
                {
                    return DefBlacksmithy.CraftSystem.AddRecipeScrolls(
                        player,
						new FancyStoneChest
						{
							Name = "Plate Armor Recipes",
							Hue = Server.Utilities.HueUtilities.RandomBrightMetalHue()
						},
                        typeof(PlateArms),
                        typeof(PlateGloves),
                        typeof(PlateGorget),
                        typeof(PlateLegs),
                        typeof(PlateSkirt),
                        typeof(PlateChest),
                        typeof(FemalePlateChest),
                        typeof(PlateMempo),
                        typeof(PlateDo),
                        typeof(PlateHiroSode),
                        typeof(PlateSuneate),
                        typeof(PlateHaidate)
                    );
                })
            );
        }

        public override Type QuestRecipient { get { return typeof(DevilGuardSmithGuy); } }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return QuestRecipient;  // Quest Giver & Recipient
        }

        public override void Generate()
        {
            base.Generate();

            PutSpawner(new Spawner(1, 5, 10, 0, 0, QuestRecipient.Name), new Point3D(1612, 1451, 7), Map.Sosaria);
        }
    }

    public class AnimalArmorQuest : MLQuest
    {
        public class HintQuest : MLQuest
        {
            public override bool IsChainTriggered { get { return true; } }
            public override Type NextQuest { get { return typeof(AnimalArmorQuest); } }

            public HintQuest()
            {
                Activated = true;
                Title = "Delivery: The Iron Golem";

                var builder = new StringBuilder();
                builder.Append("\"Alas, I am not the correct mentor for the next step in your crafting journey.\" A surprising silence fills the air, an unfamiliar sound in this shop. The smith stares into the coals for a moment...and then looks up at you.");
                builder.Append("<br><br>");
                builder.Append("\"I suggest you travel northeast, to another town shielded by mountains. Seek out Yew. I am aware of a blacksmith who used to live there. He is...interesting. An excellent craftsman, but with less focus on metals and...humans. Heh.\" The blacksmith chuckles to himself before handing over a crate of blacksmith supplies.");
                builder.Append("<br><br>");
                builder.Append("\"Take this. An offering will improve your odds of success in building this relationship.\"");
                builder.Append("<br><br>");
                Description = builder.ToString();

                Objectives.Add(new DeliverObjective(typeof(BlacksmithDeliveryCrate), 1, typeof(YewSmithGuy)));
                Objectives.Add(new DummyObjective("- Blacksmith Crate"));
                Objectives.Add(new DummyObjective("- Location: Yew"));
                Objectives.Add(new DummyObjective(""));
                Objectives.Add(new DummyObjective("Tips:"));
                DeliveryQuestUtilities.AddNoviceTravelTips(this);

                builder.Clear();
                builder.Append("The blacksmith looks up, adjusts his glasses, and squints in the low flickering lighting. \"Who's there? Leave us alone.\" He bends back towards his work while the apprentice pumps the bellows.");
                builder.Append("<br><br>");
                builder.Append("You look across the room for expert craftsmanship and notice very little...stuff. You feel that something is askance, but you're searching to describe it. Suddenly, you realize: of the few items in the room, almost none are armor or weapons.");
                builder.Append("<br><br>");
                builder.Append("As the smith finishes his piece, the apprentice breaks away from the bellows to address you. You explain your delivery and the apprentice accepts the goods while the blacksmith continues avoiding you.");
                CompletionMessage = builder.ToString();

                Rewards.Add(new ItemReward("Gold Coins", typeof(Gold), 300));
            }

            public override IEnumerable<Type> GetQuestGivers()
            {
                yield break;
            }
        }

        public override bool IsChainTriggered { get { return false; } } // Hint quest is optional
        public override Type NextQuest { get { return typeof(RoyalArmorQuest.HintQuest); } }

        public AnimalArmorQuest()
        {
            Activated = true;
            Title = "The Unbreakable Bond";

            var builder = new StringBuilder();
            builder.Append("Out of nowhere, you see a squirrel running around the shop while a blacksmith and apprentice go about their business. It chitters at you and runs away. You attempt to converse with the blacksmith, but you are promptly ignored.");
            builder.Append("<br><br>");
            builder.Append("The apprentice looks up and notices you. \"Hmm? Ah, yes. Many thanks for the delivery. Uh, the master doesn't...spend much time breaking from his work to...uh...socialize. \" ");
            builder.Append("<br><br>");
            builder.Append("You hear the chittering of the squirrel, as if mocking you. The smith looks up, briefly smiling fondly at the squirrel as if the two are sharing a secret. ");
            builder.Append("<br><br>");
            builder.Append("\"Ah, yes yes. I, too, am looking to improve my craft. The master has turned away many potential aides...\" *A chuckle and a wry grin spreads on the apprentice's face*");
            builder.Append("<br><br>");
            builder.Append("\"Well, you should know that most are no longer interested after learning the master rarely crafts anything for humans anyway.\"");
            builder.Append("<br><br>");
            builder.Append("...the conversation continues, with you pressing the point - you will not be deterred from your path - until, finally, the squirrel approaches closely, chittering at you. It isn't the apprentice that relents! Rather, in complete surprise, the smith stops working and looks at you.");
            builder.Append("<br><br>");
            builder.Append("\"Enough!\", says the smith. \"I've no trust for humans, but if you wish to prove yourself, it is a simple task. I will not teach you weapons, but I would be willing to part with a recipe on animal barding. Do what little we can to protect creatures on this realm.\" ");
            builder.Append("<br><br>");
            builder.Append("Seeing you still undeterred, he says: \"Go - build some platemail gorgets and chestpieces.\" ");
            builder.Append("<br><br>");
            builder.Append("And with that, the smith returns to work. The apprentice briefly raises eyebrows, shrugs, and returns to work, ignoring you as well.");
            Description = builder.ToString();

            Objectives.Add(DummyObjective.CraftAndMarkQuestItems);
            Objectives.Add(new CraftObjective(10, typeof(PlateGorget), "Platemail Gorget"));
            Objectives.Add(new CraftObjective(10, typeof(PlateChest), "Platemail (tunic)"));

            builder.Clear();
            builder.Append("The blacksmith sees you carrying in the extensive armor pieces, briefly pauses his work, and grunts.");
            builder.Append("<br><br>");
            builder.Append("\"I will not waste efforts supporting humans fighting humans...the cycle which never ends. Aye, ye can't get into too much trouble, I suppose.\"");
            builder.Append("<br><br>");
            builder.Append("The smith turns his head to look at the apprentice, gives a quick nod, and returns to work. The  apprentice goes wide-eyed, before catching himself, and scrambles to the coffer. Pulling two scrolls and snapping the lid shut, he turns to you and proffers the recipes.");
            CompletionMessage = builder.ToString();

            Rewards.Add(new ConstructibleItemReward("Animal Armor Recipes",
                player =>
                {
                    return DefBlacksmithy.CraftSystem.AddRecipeScrolls(
                        player,
						new FancyStoneChest
						{
							Name = "Animal Armor Recipes",
							Hue = Server.Utilities.HueUtilities.RandomBrightMetalHue()
						},
                        typeof(HorseArmor),
                        typeof(DragonBardingDeed)
                    );
                })
            );
        }

        public override Type QuestRecipient { get { return typeof(YewSmithGuy); } }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return QuestRecipient;  // Quest Giver & Recipient
        }

        public override void Generate()
        {
            base.Generate();

            PutSpawner(new Spawner(1, 5, 10, 0, 0, QuestRecipient.Name), new Point3D(2478, 890, 7), Map.Sosaria);
        }
    }

    public class RoyalArmorQuest : MLQuest
    {
        public class HintQuest : MLQuest
        {
            public override bool IsChainTriggered { get { return true; } }
            public override Type NextQuest { get { return typeof(RoyalArmorQuest); } }

            public HintQuest()
            {
                Activated = true;
                Title = "Delivery: Smelted Moon Rocks";

                var builder = new StringBuilder();
                builder.Append("The apprentice waves at you. Quickly hurrying to you, he points at a nearby crate full of your platemail pieces. ");
                builder.Append("<br><br>");
                builder.Append("\"The master will never ask, but it would be healthy for us to maintain, uh, connections across the realm. Would you be so kind as to deliver this to Moon for a small fee?\"");
                Description = builder.ToString();

                Objectives.Add(new DeliverObjective(typeof(BlacksmithDeliveryCrate), 1, typeof(MoonSmithGuy)));
                Objectives.Add(new DummyObjective("- Blacksmith Crate"));
                Objectives.Add(new DummyObjective("- Location: The city of Moon"));
                Objectives.Add(new DummyObjective(""));
                Objectives.Add(new DummyObjective("Tips:"));
                DeliveryQuestUtilities.AddNoviceTravelTips(this);

                builder.Clear();
                builder.Append("As you enter the shop, you gaze in wonder at the scene set before you, barely remembering to keep your mouth from falling open. Decorations, embellishments, and even metal baubles adorn the room in stark contrast to the last smithy you visited in Yew. It even smells nice in here!");
                builder.Append("<br><br>");
                builder.Append("The blacksmith greets you immediately, \"Ahoy! How might I serve thee?\" The attentive shopkeeper with a smile that never falters guides you around.");
                builder.Append("<br><br>");
                builder.Append("\"Oh oh, I see. Yes, indeed.\" *She nods her head at you*");
                builder.Append("<br><br>");
                builder.Append("\"Thanks to you, good Samaritan, for delivery of these goods.\"");
                CompletionMessage = builder.ToString();

                Rewards.Add(new ItemReward("Gold Coins", typeof(Gold), 300));
            }

            public override IEnumerable<Type> GetQuestGivers()
            {
                yield break;
            }
        }

        public override bool IsChainTriggered { get { return false; } } // Hint quest is optional
        public override Type NextQuest { get { return typeof(TridentQuest.HintQuest); } }

        public RoyalArmorQuest()
        {
            Activated = true;
            Title = "Shattered Steel & Broken Bonds";

            var builder = new StringBuilder();
            builder.Append("You are expecting the impressive shopware this time, and bask in the extravagant scenery. You approach the blacksmith, hoping your previous gesture of support will win you some initial gratitude.");
            builder.Append("<br><br>");
            builder.Append("\"Welcome back, kind soul! How might I be of service?\" The blacksmith greets you eagerly, eyes laser-focused on you.");
            builder.Append("<br><br>");
            builder.Append("As you explain your desire to hone your craft, the enthusiasm once exhibited by the blacksmith quickly dissipates, leaving behind a much more stoic demeanor. She looks unconcerned with the conversation.");
            builder.Append("<br><br>");
            builder.Append("\"Mmm. If you are so interested in my wares, why don't you buy them to study? I do run a business here, after all.\" As you sputter to respectfully decline, citing the hallmark quality of the goods, the smith produces a pair of royal armor bracers.");
            builder.Append("<br><br>");
            builder.Append("\"Yes, yes, I get it. Quiet yourself.\" She cradles the bracers, presenting them delicately, as one might a newborn.");
            builder.Append("<br><br>");
            builder.Append("\"Peer upon these: breathe in the beauty, the exquisite nature that radiates from this art. Have you honestly ever created art in your work before?\" You slowly shake your head, growing concerned with the direction of the conversation.");
            builder.Append("<br><br>");
            builder.Append("\"EXACTLY! At least you can be honest with yourself.\" You remain silent, not knowing what to do next. She carefully tucks the bracers away.");
            builder.Append("<br><br>");
            builder.Append("\"I accept your sincerity. I will teach you what I know, but for a price. Bring me ART - the canvas will be horse barding!\"");
            Description = builder.ToString();

            Objectives.Add(DummyObjective.CraftAndMarkQuestItems);
            Objectives.Add(new CraftObjective(1, typeof(HorseArmor), "Horse Barding"));

            builder.Clear();
            builder.Append("\"Master, I wish to share my art with you.\" As the blacksmith finishes with a customer, you address her reverently, hoping it will set the right tone. You fetch the large connecting pieces and present them to her.");
            builder.Append("<br><br>");
            builder.Append("Her face is cool and serene as she spends a moment studying the barding, walking around the pieces, and turning them for different perspectives. Then, all of a sudden:");
            builder.Append("<br><br>");
            builder.Append("\"I see it - incredible! The intent, the connection.\" She pauses, closes her eyes, and places a hand softly on the barding.");
            builder.Append("<br><br>");
            builder.Append("\"Smooth, flowing water down a stream...tall, proud trees that end in the gnarled roots...\" You remain silent (like last time) and wait, hoping this will conclude positively.");
            builder.Append("<br><br>");
            builder.Append("\"Do you feel it? You have achieved enlightenment. Follow me, I will teach you about producing royal armor...\" As the smith opens her eyes and walks towards the opposite end of the shop, you follow, excited for the next steps...");
            CompletionMessage = builder.ToString();

            Rewards.Add(new ConstructibleItemReward("Royal Armor Recipes",
                player =>
                {
                    return DefBlacksmithy.CraftSystem.AddRecipeScrolls(
                        player,
						new FancyStoneChest
						{
							Name = "Royal Armor Recipes",
							Hue = Server.Utilities.HueUtilities.RandomBrightMetalHue()
						},
                        typeof(RoyalBoots),
                        typeof(RoyalGloves),
                        typeof(RoyalGorget),
                        typeof(RoyalHelm),
                        typeof(RoyalsLegs),
                        typeof(RoyalArms),
                        typeof(RoyalChest)
                    );
                })
            );
        }

        public override Type QuestRecipient { get { return typeof(MoonSmithGuy); } }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return QuestRecipient;  // Quest Giver & Recipient
        }

        public override void Generate()
        {
            base.Generate();

            PutSpawner(new Spawner(1, 5, 10, 0, 0, QuestRecipient.Name), new Point3D(856, 712, 5), Map.Sosaria);
        }
    }

    public class TridentQuest : MLQuest
    {
        public class HintQuest : MLQuest
        {
            public override bool IsChainTriggered { get { return true; } }
            public override Type NextQuest { get { return typeof(TridentQuest); } }

            public HintQuest()
            {
                Activated = true;
                Title = "Delivery: The Titan's Helm";

                var builder = new StringBuilder();
                builder.Append("\"There is nothing further to teach you!\" The blacksmith says with exasperation, briefly snapping at you.");
                builder.Append("<br><br>");
                builder.Append("\"Wait - don't leave. I have an idea.\" The blacksmith scurries off for a moment and returns with a crate.");
                builder.Append("<br><br>");
                builder.Append("She hands you the crate. \"You really are on your own. But I could use some help with this. You owe me, after all.\"");
                builder.Append("<br><br>");
                builder.Append("\"Don't give me that look. You can always ask the Grey blacksmith for an apprenticeship. Lord knows what that would entail though, I don't know anyone better than me at the art,\" she says smugly.");
                Description = builder.ToString();

                Objectives.Add(new DeliverObjective(typeof(BlacksmithDeliveryCrate), 1, typeof(GreySmithGuy)));
                Objectives.Add(new DummyObjective("- Blacksmith Crate"));
                Objectives.Add(new DummyObjective("- Location: The city of Grey"));
                Objectives.Add(new DummyObjective(""));
                Objectives.Add(new DummyObjective("Tips:"));
                Objectives.Add(new DummyObjective("- "));

                builder.Clear();
                builder.Append("\"Who are you?! What do you want?! Where are you from?!\" The blacksmith demands in a rolling tirade. You stop in your tracks, bewildered, standing at the front door of the shop like a deer caught in the torchlight.");
                builder.Append("<br><br>");
                builder.Append("\"Uhhhh...\" barely escapes your mouth as you begin responding to the interrogation.");
                builder.Append("<br><br>");
                builder.Append("The smith visibly relaxes. \"Oh, ohhh. Fair enough. Well, you can never be too careful around these parts, can you?\" He gives a nervous chuckle.");
                builder.Append("<br><br>");
                builder.Append("\"Thanks, and watch out for the moon people.\"");
                CompletionMessage = builder.ToString();

                Rewards.Add(new ItemReward("Gold Coins", typeof(Gold), 300));
            }

            public override IEnumerable<Type> GetQuestGivers()
            {
                yield break;
            }
        }

        public override bool IsChainTriggered { get { return false; } } // Hint quest is optional

        public TridentQuest()
        {
            Activated = true;
            Title = "Forging the Legacy";

            var builder = new StringBuilder();
            builder.Append("\"Teach you? Of course, I'll teach you!\" the blacksmith exclaims. \"Rule number 1: watch out for thieves, trust NO ONE, and always tuck your currency into a pouch inside your vest. Rule number 2: the biggest thing to...\"");
            builder.Append("<br><br>");
            builder.Append("You cough and interrupt the lecture, refocusing him on blacksmithing as a craft. \"Wha? Oh. Uhh, sure,\" the smith acquiesces.");
            builder.Append("<br><br>");
            builder.Append("\"Aaaaaalrighty then. That's, uhh, good. Very good -wait! Did you just say royal armor?! Uh. Then, uh. We'll just pick up where you left off. Build me a pair of royal boots and a royal mantle.\" The blacksmith seems pleased with himself and turns to get back to work.");
            builder.Append("<br><br>");
            builder.Append("You quickly stop him, with assurances that you'll craft the required royal armor pieces, but ask politely what you'll be learning from him as recompense.");
            builder.Append("<br><br>");
            builder.Append("He hesitates, eyes suddenly seeing into the distance, \"Of course, I'll, uhhhh...be teaching you how to craft...uhhhh...\"");
            builder.Append("<br><br>");
            builder.Append("\"A trident!\" he finishes his thought with newfound certainty and nods to himself.");
            builder.Append("<br><br>");
            builder.Append("\"...and I'll be teaching you all about the world.\" He nods to himself again, certain yet again.");
            Description = builder.ToString();

            Objectives.Add(DummyObjective.CraftAndMarkQuestItems);
            Objectives.Add(new CraftObjective(1, typeof(RoyalBoots), "Royal Boots"));
            Objectives.Add(new CraftObjective(1, typeof(RoyalArms), "Royal Mantle"));

            builder.Clear();
            builder.Append("\"Hey, look at that! Those look great.\" The blacksmith covets your delivery of royal armor pieces.");
            builder.Append("<br><br>");
            builder.Append("Mumbling to himself, you make out a few tidbits \"How does that even attach...smooth joint...decorative AND functional...hmmm...\"");
            builder.Append("<br><br>");
            builder.Append("You clear your throat loudly, breaking the smith from his reverie and he looks up momentarily startled.");
            builder.Append("<br><br>");
            builder.Append("\"Ah, right...\" He starts, voice fading as he forgets what comes next.");
            builder.Append("<br><br>");
            builder.Append("\"...trident?\" you volunteer, helpfully.");
            builder.Append("<br><br>");
            builder.Append("\"YES!\" he exclaims. \"This way,\" as he walks towards the equipment in the back of the shop.");
            CompletionMessage = builder.ToString();

            Rewards.Add(new ConstructibleItemReward("Trident Recipe",
                player =>
                {
                    return DefBlacksmithy.CraftSystem.AddRecipeScrolls(
                        player,
						new FancyStoneChest
						{
							Name = "Trident Recipe",
							Hue = Server.Utilities.HueUtilities.RandomBrightMetalHue()
						},
                        typeof(Pitchfork)
                    );
                })
            );
        }

        public override Type QuestRecipient { get { return typeof(GreySmithGuy); } }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return QuestRecipient;  // Quest Giver & Recipient
        }

        public override void Generate()
        {
            base.Generate();

            PutSpawner(new Spawner(1, 5, 10, 0, 0, QuestRecipient.Name), new Point3D(917, 2097, 5), Map.Sosaria);
        }
    }

    #endregion

    #region Mobiles

    [QuesterName("the Smith Guildmaster in Britain")]
    public class BritainGuildmasterSmithGuy : BlacksmithGuildmaster
    {
        [Constructable]
        public BritainGuildmasterSmithGuy()
        {
        }

        public BritainGuildmasterSmithGuy(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [QuesterName("the Smith in Montor")]
    public class MontorSmithGirl : Blacksmith
    {
        [Constructable]
        public MontorSmithGirl()
        {
            Body = 0x191;
            Female = true;
            Name = NameList.RandomName("female");
        }

        public MontorSmithGirl(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [QuesterName("the Smith in Devil Guard")]
    public class DevilGuardSmithGuy : Blacksmith
    {
        [Constructable]
        public DevilGuardSmithGuy()
        {
            // TODO: Male
        }

        public DevilGuardSmithGuy(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [QuesterName("the Smith in Yew")]
    public class YewSmithGuy : Blacksmith
    {
        [Constructable]
        public YewSmithGuy()
        {
        }

        public YewSmithGuy(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [QuesterName("the Smith in Moon")]
    public class MoonSmithGuy : Blacksmith
    {
        [Constructable]
        public MoonSmithGuy()
        {
        }

        public MoonSmithGuy(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [QuesterName("the Smith in Grey")]
    public class GreySmithGuy : Blacksmith
    {
        [Constructable]
        public GreySmithGuy()
        {
        }

        public GreySmithGuy(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    #endregion


    #region Items

    public class BlacksmithDeliveryCrate : Item
    {
        [Constructable]
        public BlacksmithDeliveryCrate() : base(0x4F8D)
        {
            Name = "blacksmith crate";
            Weight = 10.0;
            ResourceMods.DefaultItemHue(this);
        }

        public BlacksmithDeliveryCrate(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion
}