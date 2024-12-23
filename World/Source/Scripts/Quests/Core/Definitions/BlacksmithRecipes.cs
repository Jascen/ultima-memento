using System;
using System.Collections.Generic;
using Server.Engines.Craft;
using Server.Engines.MLQuests.Objectives;
using Server.Engines.MLQuests.Rewards;
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

    public class HintRingArmorQuest : MLQuest
    {
        public override bool IsChainTriggered { get { return true; } }
        public override Type NextQuest { get { return typeof(RingArmorQuest); } }

        public HintRingArmorQuest()
        {
            Activated = true;
            Title = "Delivery: The Hammer and Anvil";
            Description = "TODO: Deliver this to the Britain Smith";
            RefusalMessage = "RefusalMessage HintRingArmorQuest";
            InProgressMessage = "InProgressMessage HintRingArmorQuest";
            CompletionMessage = "CompletionMessage HintRingArmorQuest";

            Objectives.Add(new DeliverObjective(typeof(BlacksmithDeliveryCrate), 1, typeof(BritainGuildmasterSmithGuy)));
            Objectives.Add(new DummyObjective("- Crate of Ingots"));

            Rewards.Add(new ItemReward("Gold Coins", typeof(Gold), 300));
        }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return typeof(BritainGuildmasterSmithGuy); // Quest Recipient
        }
    }

    public class RingArmorQuest : MLQuest
    {
        public override bool IsChainTriggered { get { return false; } } // Hint quest is optional
        public override Type NextQuest { get { return typeof(HintChainArmorQuest); } } // Optional delivery

        public RingArmorQuest()
        {
            Activated = true;
            Title = "The Hammer's Return";
            Description = "Description RingArmorQuest";
            RefusalMessage = "RefusalMessage RingArmorQuest";
            InProgressMessage = "InProgressMessage RingArmorQuest";
            CompletionMessage = "CompletionMessage RingArmorQuest";

            Objectives.Add(DummyObjective.CraftAndMarkQuestItems);
            Objectives.Add(new CraftObjective(20, typeof(WoodenKiteShield), "Kite Shield"));

            Rewards.Add(new ConstructibleItemReward("Ringmail Armor Recipes",
                player =>
                {
                    return DefBlacksmithy.CraftSystem.GetRecipeScrolls(
                        player,
                        typeof(RingmailGloves),
                        typeof(RingmailLegs),
                        typeof(RingmailArms),
                        typeof(RingmailChest),
                        typeof(RingmailSkirt)
                    );
                })
            );
        }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return typeof(BritainGuildmasterSmithGuy);  // Quest Giver & Recipient
        }

        public override void Generate()
        {
            base.Generate();

            PutSpawner(new Spawner(1, 5, 10, 0, 0, "BritainGuildmasterSmithGuy"), new Point3D(2981, 1023, 0), Map.Sosaria);
        }
    }

    public class HintChainArmorQuest : MLQuest
    {
        public override bool IsChainTriggered { get { return true; } }
        public override Type NextQuest { get { return typeof(ChainArmorQuest); } }

        public HintChainArmorQuest()
        {
            Activated = true;
            Title = "Delivery: Metals of Montor";
            Description = "TODO: Deliver this to the Montor Smith";
            RefusalMessage = "RefusalMessage HintChainArmorQuest";
            InProgressMessage = "InProgressMessage HintChainArmorQuest";
            CompletionMessage = "CompletionMessage HintChainArmorQuest";

            Objectives.Add(new DeliverObjective(typeof(BlacksmithDeliveryCrate), 1, typeof(MontorSmithGuy)));
            Objectives.Add(new DummyObjective("- A package"));

            Rewards.Add(new ItemReward("Gold Coins", typeof(Gold), 300));
        }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return typeof(MontorSmithGuy); // Quest Recipient
        }
    }

    public class ChainArmorQuest : MLQuest
    {
        public override bool IsChainTriggered { get { return false; } } // Hint quest is optional
        public override Type NextQuest { get { return typeof(HintPlateArmorQuest); } }

        public ChainArmorQuest()
        {
            Activated = true;
            Title = "The Heart of the Forge";
            Description = "Description ChainArmorQuest";
            RefusalMessage = "RefusalMessage ChainArmorQuest";
            InProgressMessage = "InProgressMessage ChainArmorQuest";
            CompletionMessage = "CompletionMessage ChainArmorQuest";

            Objectives.Add(DummyObjective.CraftAndMarkQuestItems);
            Objectives.Add(new CraftObjective(15, typeof(RingmailGloves), "Ringmail Gloves"));
            Objectives.Add(new CraftObjective(15, typeof(RingmailLegs), "Ringmail Leggings"));

            Rewards.Add(new ConstructibleItemReward("Chain Armor Recipes",
                player =>
                {
                    return DefBlacksmithy.CraftSystem.GetRecipeScrolls(
                        player,
                        typeof(ChainCoif),
                        typeof(ChainLegs),
                        typeof(ChainChest),
                        typeof(ChainSkirt)
                    );
                })
            );
        }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return typeof(MontorSmithGuy);  // Quest Giver & Recipient
        }

        public override void Generate()
        {
            base.Generate();

            PutSpawner(new Spawner(1, 5, 10, 0, 0, "MontorSmithGuy"), new Point3D(3155, 2600, 0), Map.Sosaria);
        }
    }

    public class HintPlateArmorQuest : MLQuest
    {
        public override bool IsChainTriggered { get { return true; } }
        public override Type NextQuest { get { return typeof(PlateArmorQuest); } }

        public HintPlateArmorQuest()
        {
            Activated = true;
            Title = "Delivery: Forged Iron";
            Description = "TODO: Deliver this to the Devil Guard Smith";
            RefusalMessage = "RefusalMessage HintPlateArmorQuest";
            InProgressMessage = "InProgressMessage HintPlateArmorQuest";
            CompletionMessage = "CompletionMessage HintPlateArmorQuest";

            Objectives.Add(new DeliverObjective(typeof(BlacksmithDeliveryCrate), 1, typeof(DevilGuardSmithGuy)));
            Objectives.Add(new DummyObjective("- A package"));

            Rewards.Add(new ItemReward("Gold Coins", typeof(Gold), 300));
        }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return typeof(DevilGuardSmithGuy); // Quest Recipient
        }
    }

    public class PlateArmorQuest : MLQuest
    {
        public override bool IsChainTriggered { get { return false; } } // Hint quest is optional
        public override Type NextQuest { get { return typeof(HintAnimalArmorQuest); } }

        public PlateArmorQuest()
        {
            Activated = true;
            Title = "A Test of Strength";
            Description = "Description PlateArmorQuest";
            RefusalMessage = "RefusalMessage PlateArmorQuest";
            InProgressMessage = "InProgressMessage PlateArmorQuest";
            CompletionMessage = "CompletionMessage PlateArmorQuest";

            Objectives.Add(DummyObjective.CraftAndMarkQuestItems);
            Objectives.Add(new CraftObjective(15, typeof(ChainCoif), "Chainmail Coif"));
            Objectives.Add(new CraftObjective(15, typeof(ChainChest), "Chainmail Tunic"));

            Rewards.Add(new ConstructibleItemReward("Plate Armor Recipes",
                player =>
                {
                    return DefBlacksmithy.CraftSystem.GetRecipeScrolls(
                        player,
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

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return typeof(DevilGuardSmithGuy);  // Quest Giver & Recipient
        }

        public override void Generate()
        {
            base.Generate();

            PutSpawner(new Spawner(1, 5, 10, 0, 0, "DevilGuardSmithGuy"), new Point3D(1612, 1451, 0), Map.Sosaria);
        }
    }

    public class HintAnimalArmorQuest : MLQuest
    {
        public override bool IsChainTriggered { get { return true; } }
        public override Type NextQuest { get { return typeof(AnimalArmorQuest); } }

        public HintAnimalArmorQuest()
        {
            Activated = true;
            Title = "Delivery: The Iron Golem";
            Description = "TODO: Deliver this to the Yew Smith";
            RefusalMessage = "RefusalMessage HintAnimalArmorQuest";
            InProgressMessage = "InProgressMessage HintAnimalArmorQuest";
            CompletionMessage = "CompletionMessage HintAnimalArmorQuest";

            Objectives.Add(new DeliverObjective(typeof(BlacksmithDeliveryCrate), 1, typeof(YewSmithGuy)));
            Objectives.Add(new DummyObjective("- A package"));

            Rewards.Add(new ItemReward("Gold Coins", typeof(Gold), 300));
        }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return typeof(YewSmithGuy); // Quest Recipient
        }
    }

    public class AnimalArmorQuest : MLQuest
    {
        public override bool IsChainTriggered { get { return false; } } // Hint quest is optional
        public override Type NextQuest { get { return typeof(HintRoyalArmorQuest); } }

        public AnimalArmorQuest()
        {
            Activated = true;
            Title = "The Unbreakable Bond";
            Description = "TODO: Check out the Moon Blacksmith";
            Description = "Description AnimalArmorQuest";
            RefusalMessage = "RefusalMessage AnimalArmorQuest";
            InProgressMessage = "InProgressMessage AnimalArmorQuest";
            CompletionMessage = "CompletionMessage AnimalArmorQuest";

            Objectives.Add(DummyObjective.CraftAndMarkQuestItems);
            Objectives.Add(new CraftObjective(10, typeof(PlateGorget), "Platemail Gorget"));
            Objectives.Add(new CraftObjective(10, typeof(PlateChest), "Platemail"));

            Rewards.Add(new ConstructibleItemReward("Animal Armor Recipes",
                player =>
                {
                    return DefBlacksmithy.CraftSystem.GetRecipeScrolls(
                        player,
                        typeof(HorseArmor),
                        typeof(DragonBardingDeed)
                    );
                })
            );
        }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return typeof(YewSmithGuy);  // Quest Giver & Recipient
        }

        public override void Generate()
        {
            base.Generate();

            PutSpawner(new Spawner(1, 5, 10, 0, 0, "YewSmithGuy"), new Point3D(2478, 890, 0), Map.Sosaria);
        }
    }

    public class HintRoyalArmorQuest : MLQuest
    {
        public override bool IsChainTriggered { get { return true; } }
        public override Type NextQuest { get { return typeof(RoyalArmorQuest); } }

        public HintRoyalArmorQuest()
        {
            Activated = true;
            Title = "Delivery: Smelted Moon Rocks";
            Description = "TODO: Deliver this to the Moon Smith";
            RefusalMessage = "RefusalMessage HintRoyalArmorQuest";
            InProgressMessage = "InProgressMessage HintRoyalArmorQuest";
            CompletionMessage = "CompletionMessage HintRoyalArmorQuest";

            Objectives.Add(new DeliverObjective(typeof(BlacksmithDeliveryCrate), 1, typeof(MoonSmithGuy)));
            Objectives.Add(new DummyObjective("- A package"));

            Rewards.Add(new ItemReward("Gold Coins", typeof(Gold), 300));
        }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return typeof(MoonSmithGuy); // Quest Recipient
        }
    }

    public class RoyalArmorQuest : MLQuest
    {
        public override bool IsChainTriggered { get { return false; } } // Hint quest is optional
        public override Type NextQuest { get { return typeof(HintTridentQuest); } }

        public RoyalArmorQuest()
        {
            Activated = true;
            Title = "Shattered Steel & Broken Bonds";
            Description = "TODO: Check out the Grey Blacksmith";
            RefusalMessage = "RefusalMessage RoyalArmorQuest";
            InProgressMessage = "InProgressMessage RoyalArmorQuest";
            CompletionMessage = "CompletionMessage RoyalArmorQuest";

            Objectives.Add(DummyObjective.CraftAndMarkQuestItems);
            Objectives.Add(new CraftObjective(1, typeof(HorseArmor), "Horse Barding"));
            // Objectives.Add(new CraftObjective(1, typeof(DragonBardingDeed), "Dragon Barding"));

            Rewards.Add(new ConstructibleItemReward("Royal Armor Recipes",
                player =>
                {
                    return DefBlacksmithy.CraftSystem.GetRecipeScrolls(
                        player,
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

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return typeof(MoonSmithGuy); // Quest Giver & Recipient
        }

        public override void Generate()
        {
            base.Generate();

            PutSpawner(new Spawner(1, 5, 10, 0, 0, "MoonSmithGuy"), new Point3D(856, 712, 0), Map.Sosaria);
        }
    }

    public class HintTridentQuest : MLQuest
    {
        public override bool IsChainTriggered { get { return true; } }
        public override Type NextQuest { get { return typeof(TridentQuest); } }

        public HintTridentQuest()
        {
            Activated = true;
            Title = "Delivery: The Titan's Helm";
            Description = "TODO: Deliver this to the Grey Smith";
            RefusalMessage = "RefusalMessage HintTridentQuest";
            InProgressMessage = "InProgressMessage HintTridentQuest";
            CompletionMessage = "CompletionMessage HintTridentQuest";

            Objectives.Add(new DeliverObjective(typeof(BlacksmithDeliveryCrate), 1, typeof(MoonSmithGuy)));
            Objectives.Add(new DummyObjective("- A package"));

            Rewards.Add(new ItemReward("Gold Coins", typeof(Gold), 300));
        }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return typeof(GreySmithGuy); // Quest Recipient
        }
    }

    public class TridentQuest : MLQuest
    {
        public override bool IsChainTriggered { get { return false; } } // Hint quest is optional

        public TridentQuest()
        {
            Activated = true;
            Title = "Forging the Legacy";
            Description = "TODO: Hint to the Caverns of Poseidon";
            RefusalMessage = "RefusalMessage TridentQuest";
            InProgressMessage = "InProgressMessage TridentQuest";
            CompletionMessage = "CompletionMessage TridentQuest";

            Objectives.Add(DummyObjective.CraftAndMarkQuestItems);
            Objectives.Add(new CraftObjective(1, typeof(RoyalBoots), "Royal Boots"));
            Objectives.Add(new CraftObjective(1, typeof(RoyalArms), "Royal Mantle"));

            Rewards.Add(new ConstructibleItemReward("Trident Recipe",
                player =>
                {
                    return DefBlacksmithy.CraftSystem.GetRecipeScrolls(
                        player,
                        typeof(Pitchfork)
                    );
                })
            );
        }

        public override IEnumerable<Type> GetQuestGivers()
        {
            yield return typeof(GreySmithGuy);  // Quest Giver & Recipient
        }

        public override void Generate()
        {
            base.Generate();

            PutSpawner(new Spawner(1, 5, 10, 0, 0, "GreySmithGuy"), new Point3D(917, 2097, 0), Map.Sosaria);
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
    public class MontorSmithGuy : Blacksmith
    {
        [Constructable]
        public MontorSmithGuy()
        {
        }

        public MontorSmithGuy(Serial serial) : base(serial)
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