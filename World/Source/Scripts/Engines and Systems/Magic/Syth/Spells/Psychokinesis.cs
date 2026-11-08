using System;
using Server.Targeting;
using Server.Items;

namespace Server.Spells.Syth
{
    public class Psychokinesis : SythSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 270,
					IconGraphic = 0x040E,
					Name = "Psychokinesis",
					PowerWords = "Dzwol Hyal",
					Description = "Allows a Syth to use or move an object that is out of reach. Can be used to set off some Chest Traps from a safe distance as well.",
					ManaCost = 5,
					TithingCost = 6,
					MinSkill = 10,
					TargetType = TargetFlags.Beneficial
				},
				203,
				0
			);

		public int CirclePower = 1;
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

        public Psychokinesis(Mobile caster, Item scroll) : base(caster, scroll, SpellInfo)
        {
        }

        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(ITelekinesisable obj)
        {
            if (this.CheckSequence())
            {
                SpellHelper.Turn(this.Caster, obj);

                obj.OnTelekinesis(this.Caster);
            }
        }

        public void Target(Container item)
        {
            if (this.CheckSequence())
            {
                SpellHelper.Turn(this.Caster, item);

                object root = item.RootParent;

                if (!item.IsAccessibleTo(this.Caster))
                {
                    item.OnDoubleClickNotAccessible(this.Caster);
                }
                else if (!item.CheckItemUse(this.Caster, item))
                {
                }
                else if (root != null && root is Mobile && root != this.Caster)
                {
                    item.OnSnoop(this.Caster);
                }
                else if (item is Corpse && !((Corpse)item).CheckLoot(this.Caster, null))
                {
                }
                else if ( this.Caster.Region.OnDoubleClick(this.Caster, item) )
                {
                    Effects.SendLocationParticles(EffectItem.Create(item.Location, item.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 0, 0, 5022, 0);
                    Effects.PlaySound(item.Location, item.Map, 0x1F5);

                    item.OnItemUsed(this.Caster, item);
                }
            }
        }

#region Grab
        public void Target(Item item)
        {
            if (this.CheckSequence())
            {
                SpellHelper.Turn(this.Caster, item);
                object root = item.RootParent;

				if (item.Movable == false){ Caster.SendMessage( "That item does not seem to move." ); }
				else if (item.Amount > 1){ Caster.SendMessage( "There are too many items stacked here to move." ); }
				else if (item.Weight > (Caster.Int / 20)){ Caster.SendMessage( "That is to heavy to move." ); }
				else if (item.RootParentEntity != null){ Caster.SendMessage( "You can not move objects that are inside of other objects or being worn." ); }
				else
				{
					Effects.SendLocationParticles(EffectItem.Create(item.Location, item.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 0, 0, 5022, 0);
					Effects.PlaySound(item.Location, item.Map, 0x1F5);
					Caster.AddToBackpack( item );
					Caster.SendMessage( "You move the object to within your grasp and place it in your backpack."); 
				}
			}
        }
#endregion

        public class InternalTarget : Target
        {
            private readonly Psychokinesis m_Owner;
            public InternalTarget(Psychokinesis owner) : base(Core.ML ? 10 : 12, false, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is ITelekinesisable)
                    this.m_Owner.Target((ITelekinesisable)o);
                else if (o is Container)
                    this.m_Owner.Target((Container)o);
                    else if (o is Item)
                    this.m_Owner.Target((Item)o);
                else
					from.SendMessage( "This power will not work on that!"); 
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}