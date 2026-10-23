using System;
using Server.Targeting;
using System.Collections;
using Server.Items;

namespace Server.Spells.Mystic
{
	public class AstralProjection : MysticSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 250,
					IconGraphic = 0x500E,
					Name = "Astral Projection",
					PowerWords = "Beh Cah Summ Om",
					Description = "Enter the astral plane where your soul is immune to harm. While you are in this state, you can freely travel but your interraction with the world is minimal. The better your skill, the longer it lasts. Monks use this ability to safely travel through dangerous areas.",
					ManaCost = 55,
					TithingCost = 300,
					MinSkill = 80,
					TargetType = TargetFlags.None
				},
				269,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3 ); } }
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }
		public override int MysticSpellCircle{ get{ return 8; } }

        private int m_NewBody;
        private int m_OldBody;
        private int m_NewHue;
        private int m_OldHue;

        public AstralProjection(Mobile caster, Item scroll) : base(caster, scroll, SpellInfo)
        {
        }

        public override bool CheckCast()
        {
            if (Caster.Mounted)
            {
                Caster.SendLocalizedMessage(1042561); //Please dismount first.
                return false;
            }
            else if (TransformationSpellHelper.UnderTransformation(Caster))
            {
                Caster.SendMessage("You cannot enter the astral plane while in that form.");
                return false;
            }
			else if ( DisguiseTimers.IsDisguised( Caster ) )
			{
                Caster.SendMessage("You cannot enter the astral plane while disguised.");
                return false;
            }
            else if (!Caster.CanBeginAction(typeof(AstralProjection)))
            {
                Caster.SendMessage("You are already in the astral plane.");
                return false;
            }
            else
            {
				m_NewBody = 970;
				m_NewHue = 0x4001;
            }
            m_OldBody = Caster.Body;
            m_OldHue = Caster.Hue;
            return true;
        }

        public override void OnCast()
        {
            if (!CheckSequence())
            {
                return;
            }
            else if (!Caster.CanBeginAction(typeof(AstralProjection)))
            {
                Caster.SendMessage("You are already in the astral plane.");
            }
            else if (TransformationSpellHelper.UnderTransformation(Caster))
            {
                Caster.SendMessage("You cannot enter the astral plane while in that form.");
            }
			else if ( DisguiseTimers.IsDisguised( Caster ) )
			{
                Caster.SendMessage("You cannot enter the astral plane while disguised.");
            }
            else if (!Caster.CanBeginAction(typeof(Server.Spells.Shinobi.Deception)) || !Caster.CanBeginAction(typeof(Server.Spells.Fifth.IncognitoSpell)) || (Caster.IsBodyMod && Caster.RaceID != Caster.BodyMod) )
            {
                DoFizzle();
            }
            else if (CheckSequence())
            {
                if (Caster.BeginAction(typeof(AstralProjection)))
                {
                    if (m_NewBody != 0)
                    {
                        if (this.Scroll != null)
                            Scroll.Consume();

                        Caster.PlaySound(0x655);
                        Caster.BodyValue = m_NewBody;
                        Caster.Hue = m_NewHue;
                        Caster.SendMessage("You enter the astral plane.");
                        Caster.Blessed = true;

                        StopTimer(Caster);

                        Timer t = new InternalTimer(Caster, m_OldBody, m_OldHue);

                        m_Timers[Caster] = t;

                        t.Start();
                    }
                }
                else
                {
                    Caster.SendMessage("You are already in the astral plane.");
                }
            }

            FinishSequence();
        }

        private static Hashtable m_Timers = new Hashtable();

        public static bool StopTimer(Mobile m)
        {
            Timer t = (Timer)m_Timers[m];

            if (t != null)
            {
                t.Stop();
                m_Timers.Remove(m);
            }

            return (t != null);
        }

        private class InternalTimer : Timer
        {
            private Mobile m_Owner;
            private int m_OldBody;
            private int m_OldHue;

            public InternalTimer(Mobile owner, int body, int hue) : base(TimeSpan.FromSeconds(0))
            {
                m_Owner = owner;
                m_OldBody = body;
                m_OldHue = hue;

                int val = (int)( owner.Skills[SkillName.FistFighting].Value );

                if (val > 100)
                    val = 100;

                Delay = TimeSpan.FromSeconds(val);
                Priority = TimerPriority.TwoFiftyMS;

				BuffInfo.RemoveBuff( owner, BuffIcon.AstralProjection );
				BuffInfo.AddBuff( owner, new BuffInfo( BuffIcon.AstralProjection, 1063512, Delay, owner ) );
            }

            protected override void OnTick()
            {
                if (!m_Owner.CanBeginAction(typeof(AstralProjection)))
                {
                    m_Owner.BodyValue = m_OldBody;
                    m_Owner.Hue = m_OldHue;
					m_Owner.Blessed = false;
                    m_Owner.EndAction(typeof(AstralProjection));
					BuffInfo.RemoveBuff( m_Owner, BuffIcon.AstralProjection );
                }
            }
        }
    }
}
