using System;
using System.Collections;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;
using Server.ContextMenus;

namespace Server.Items
{
	[Flipable( 0x1C0E, 0x1C0F )]
    public class PickBoxEasy : LockableContainer
    {
		public override string DefaultDescription{ get{ return "These are locked boxes that thieves use to practice their lockpicking skills. They require a single skill point in lockpicking, and can help you learn up to 25."; } }

        [Constructable]
        public PickBoxEasy(): base( 0x1C0E )
        {
			Name = "Locked Box";
			InfoText1 = "Easy Lock";
			Hue = 0xB61;
            Locked = true;
            LockLevel = 1;
            MaxLockLevel = 25;
            RequiredSkill = 1;
            Weight = 4.0;
        }

        public override void LockPick(Mobile from)
        {
            this.Locked = true;
            from.SendMessage("The container relocks itself.");
        }

        public PickBoxEasy(Serial serial) : base(serial)
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
	/////////////////////////////////////////////////////////////////////////////////////////
	[Flipable( 0x1C0E, 0x1C0F )]
    public class PickBoxNormal : LockableContainer
    {
		public override string DefaultDescription{ get{ return "These are locked boxes that thieves use to practice their lockpicking skills. They require a 20 lockpicking, and can help you learn up to 35."; } }

        [Constructable]
        public PickBoxNormal(): base( 0x1C0E )
        {
			Name = "Locked Box";
			InfoText1 = "Normal Lock";
			Hue = 0xB61;
            Locked = true;
            LockLevel = 20;
            MaxLockLevel = 35;
            RequiredSkill = 20;
            Weight = 4.0;
        }

        public override void LockPick(Mobile from)
        {
            this.Locked = true;
            from.SendMessage("The container relocks itself.");
        }

        public PickBoxNormal(Serial serial) : base(serial)
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
	/////////////////////////////////////////////////////////////////////////////////////////
	[Flipable( 0x1C0E, 0x1C0F )]
    public class PickBoxDifficult : LockableContainer
    {
		public override string DefaultDescription{ get{ return "These are locked boxes that thieves use to practice their lockpicking skills. They require a 30 lockpicking, and can help you learn up to 45."; } }

        [Constructable]
        public PickBoxDifficult(): base( 0x1C0E )
        {
			Name = "Locked Box";
			InfoText1 = "Difficult Lock";
			Hue = 0xB61;
            Locked = true;
            LockLevel = 30;
            MaxLockLevel = 45;
            RequiredSkill = 30;
            Weight = 4.0;
        }

        public override void LockPick(Mobile from)
        {
            this.Locked = true;
            from.SendMessage("The container relocks itself.");
        }

        public PickBoxDifficult(Serial serial) : base(serial)
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
	/////////////////////////////////////////////////////////////////////////////////////////
	[Flipable( 0x1C0E, 0x1C0F )]
    public class PickBoxChallenging : LockableContainer
    {
		public override string DefaultDescription{ get{ return "These are locked boxes that thieves use to practice their lockpicking skills. They require a 40 lockpicking, and can help you learn up to 55."; } }

        [Constructable]
        public PickBoxChallenging(): base( 0x1C0E )
        {
			Name = "Locked Box";
			InfoText1 = "Challenging Lock";
			Hue = 0xB61;
            Locked = true;
            LockLevel = 40;
            MaxLockLevel = 55;
            RequiredSkill = 40;
            Weight = 4.0;
        }

        public override void LockPick(Mobile from)
        {
            this.Locked = true;
            from.SendMessage("The container relocks itself.");
        }

        public PickBoxChallenging(Serial serial) : base(serial)
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
	/////////////////////////////////////////////////////////////////////////////////////////
	[Flipable( 0x1C0E, 0x1C0F )]
    public class PickBoxHard : LockableContainer
    {
		public override string DefaultDescription{ get{ return "These are locked boxes that thieves use to practice their lockpicking skills. They require a 50 lockpicking, and can help you learn up to 65."; } }

        [Constructable]
        public PickBoxHard(): base( 0x1C0E )
        {
			Name = "Locked Box";
			InfoText1 = "Hard Lock";
			Hue = 0xB61;
            Locked = true;
            LockLevel = 50;
            MaxLockLevel = 65;
            RequiredSkill = 50;
            Weight = 4.0;
        }

        public override void LockPick(Mobile from)
        {
            this.Locked = true;
            from.SendMessage("The container relocks itself.");
        }

        public PickBoxHard(Serial serial) : base(serial)
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
	/////////////////////////////////////////////////////////////////////////////////////////
	[Flipable( 0x1C0E, 0x1C0F )]
    public class PickBoxVeryHard : LockableContainer
    {
		public override string DefaultDescription{ get{ return "These are locked boxes that thieves use to practice their lockpicking skills. They require a 60 lockpicking, and can help you learn up to 75."; } }

        [Constructable]
        public PickBoxVeryHard(): base( 0x1C0E )
        {
			Name = "Locked Box";
			InfoText1 = "Very Hard Lock";
			Hue = 0xB61;
            Locked = true;
            LockLevel = 60;
            MaxLockLevel = 75;
            RequiredSkill = 60;
            Weight = 4.0;
        }

        public override void LockPick(Mobile from)
        {
            this.Locked = true;
            from.SendMessage("The container relocks itself.");
        }

        public PickBoxVeryHard(Serial serial) : base(serial)
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
	/////////////////////////////////////////////////////////////////////////////////////////
	[Flipable( 0x1C0E, 0x1C0F )]
    public class PickBoxExtreme : LockableContainer
    {
		public override string DefaultDescription{ get{ return "These are locked boxes that thieves use to practice their lockpicking skills. They require a 70 lockpicking, and can help you learn up to 85."; } }

        [Constructable]
        public PickBoxExtreme(): base( 0x1C0E )
        {
			Name = "Locked Box";
			InfoText1 = "Extreme Lock";
			Hue = 0xB61;
            Locked = true;
            LockLevel = 70;
            MaxLockLevel = 85;
            RequiredSkill = 70;
            Weight = 4.0;
        }

        public override void LockPick(Mobile from)
        {
            this.Locked = true;
            from.SendMessage("The container relocks itself.");
        }

        public PickBoxExtreme(Serial serial) : base(serial)
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
	/////////////////////////////////////////////////////////////////////////////////////////
	[Flipable( 0x1C0E, 0x1C0F )]
    public class PickBoxMaster : LockableContainer
    {
		public override string DefaultDescription{ get{ return "These are locked boxes that thieves use to practice their lockpicking skills. They require a 80 lockpicking, and can help you learn up to 95."; } }

        [Constructable]
        public PickBoxMaster(): base( 0x1C0E )
        {
			Name = "Locked Box";
			InfoText1 = "Master Lock";
			Hue = 0xB61;
            Locked = true;
            LockLevel = 80;
            MaxLockLevel = 95;
            RequiredSkill = 80;
            Weight = 4.0;
        }

        public override void LockPick(Mobile from)
        {
            this.Locked = true;
            from.SendMessage("The container relocks itself.");
        }

        public PickBoxMaster(Serial serial) : base(serial)
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
}