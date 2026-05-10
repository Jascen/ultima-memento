using Server.Spells.Chivalry;
using Server.Targeting;
using System;

namespace Server.Items
{
    public abstract class ConsecrateItemBase : Item
    {
        private const int DEFAULT_HUE = 0x973;
        private int _uses;
        protected bool IsLegacyItem;

        public ConsecrateItemBase(Serial serial) : base(serial)
        {
        }

        protected ConsecrateItemBase(int uses, int itemID) : base(itemID)
        {
            Weight = 1.0;
            Hue = DEFAULT_HUE;
            Uses = uses;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Uses
        {
            get { return _uses; }
            set { _uses = value; InvalidateProperties(); }
        }

        public void Apply(Mobile from, object targeted)
        {
            if (Deleted) { return; }

            var weapon = targeted as BaseWeapon;
            if (!Validate(from, weapon)) return;
            if (!ApplyBonus(from, weapon)) return;

            if (--Uses < 1)
            {
                Delete();
                from.SendMessage(32, "You use the last of the magic");
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060584, Uses.ToString()); // uses remaining: ~1_val~
            list.Add("Consecrates a weapon for 4 hours");
        }

        public override void OnDoubleClick(Mobile from)
        {
            PromptForTarget(from, "Which weapon would you like to consecrate?");
        }

        protected bool ApplyBonus(Mobile from, BaseWeapon weapon)
        {
            ConsecrateWeaponSpell.Apply(from, weapon, TimeSpan.FromHours(4), false);
            return true;
        }

        protected void PromptForTarget(Mobile from, string message)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("This must be in your backpack to use");
                return;
            }

            from.SendMessage(message);
            from.Target = new InternalTarget(this);
        }

        protected virtual bool Validate(Mobile from, BaseWeapon weapon)
        {
            if (weapon == null || weapon.Deleted) return false;

            if (!weapon.IsChildOf(from.Backpack))
            {
                from.SendMessage(32, "This must be in your backpack");
                return false;
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3); // version
            writer.Write((int)_uses);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            IsLegacyItem = version < 2;
            _uses = reader.ReadInt();

            switch(version)
            {
                case 2:
                    if (Hue != DEFAULT_HUE) Hue = DEFAULT_HUE;
                    break;
            }
        }

        private class InternalTarget : Target
        {
            private readonly ConsecrateItemBase _itemBase;

            public InternalTarget(ConsecrateItemBase itemBase) : base(1, false, TargetFlags.None)
            {
                _itemBase = itemBase;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                _itemBase.Apply(from, targeted);
            }
        }
    }
}