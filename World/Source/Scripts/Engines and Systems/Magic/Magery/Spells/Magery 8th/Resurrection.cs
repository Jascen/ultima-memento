using System;
using Server.Targeting;
using Server.Network;
using Server.Gumps;
using Server.Mobiles;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server.Spells.Eighth
{
    public class ResurrectionSpell : MagerySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Resurrection", "An Corp",
                245,
                9062,
                Reagent.Bloodmoss,
                Reagent.Garlic,
                Reagent.Ginseng
            );
 
        public override SpellCircle Circle { get { return SpellCircle.Eighth; } }
 
        public ResurrectionSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
        {
        }
 
        public override void OnCast()
        {
            Caster.Target = new InternalTarget( this );
        }
 
        public void Target( Mobile m )
        {
            if ( !Caster.CanSee( m ) )
            {
                Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
            }
            else if ( m == Caster && CheckBSequence( m, true ) )
            {
				if ( SoulOrb.Create( m, SoulOrbType.Default ) != null)
				{
					m.PlaySound( 0x214 );
					m.FixedEffect( 0x376A, 10, 16, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0 );
					m.SendMessage( "You summon a magical orb to protect your soul." );
				}
            }
            else if ( m == Caster )
            {
				Caster.SendMessage("You failed to summon an orb.");
			}
            else if ( !Caster.Alive )
            {
                Caster.SendLocalizedMessage( 501040 ); // The resurrecter must be alive.
            }
            else if (m.Alive && !m.IsDeadBondedPet)
            {
                Caster.SendLocalizedMessage( 501041 ); // Target is not dead.
            }
            else if ( !Caster.InRange( m, 2 ) )
            {
                Caster.SendLocalizedMessage( 501042 ); // Target is not close enough.
            }
            else if ( m.Map == null || !m.Map.CanFit( m.Location, 16, false, false ) )
            {
                Caster.SendLocalizedMessage( 501042 ); // Target can not be resurrected at that location.
                m.SendLocalizedMessage( 502391 ); // Thou can not be resurrected there!
            }
            else if ( m is PlayerMobile && CheckBSequence( m, true ) )
            {
                SpellHelper.Turn( Caster, m );
 
                m.PlaySound( 0x214 );
                m.FixedEffect( 0x376A, 10, 16, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0 );
 
                m.CloseGump( typeof( ResurrectGump ) );
                m.SendGump( new ResurrectGump( m, Caster ) );
            }
            else if (m is BaseCreature && CheckBSequence( m, true ) )
			{
				BaseCreature pet = (BaseCreature)m;
				Mobile master = pet.GetMaster();
                SpellHelper.Turn( Caster, m );
 
                m.PlaySound( 0x214 );
                m.FixedEffect( 0x376A, 10, 16, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0 );
 
                master.CloseGump(typeof(PetResurrectGump));
                master.SendGump(new PetResurrectGump(master, pet));
            }
            FinishSequence();
        }

        public void ItemTarget( Item hench )
        {
			if ( hench is HenchmanFighterItem && CheckSequence() )
			{
				HenchmanFighterItem friend = (HenchmanFighterItem)hench;

				if ( friend.HenchDead > 0 )
				{
					friend.Name = "fighter henchman";
					friend.HenchDead = 0;
					friend.InvalidateProperties();
					Caster.PlaySound( 0x214 );
				}
				else
				{
					Caster.SendMessage("They are not dead.");
				}
			}
			else if ( hench is HenchmanWizardItem && CheckSequence() )
			{
				HenchmanWizardItem friend = (HenchmanWizardItem)hench;

				if ( friend.HenchDead > 0 )
				{
					friend.Name = "wizard henchman";
					friend.HenchDead = 0;
					friend.InvalidateProperties();
					Caster.PlaySound( 0x214 );
				}
				else
				{
					Caster.SendMessage("They are not dead.");
				}
			}
			else if ( hench is HenchmanArcherItem && CheckSequence() )
			{
				HenchmanArcherItem friend = (HenchmanArcherItem)hench;

				if ( friend.HenchDead > 0 )
				{
					friend.Name = "archer henchman";
					friend.HenchDead = 0;
					friend.InvalidateProperties();
					Caster.PlaySound( 0x214 );
				}
				else
				{
					Caster.SendMessage("They are not dead.");
				}
			}
			else if (hench is HenchmanMonsterItem && CheckSequence() )
			{
				HenchmanMonsterItem friend = (HenchmanMonsterItem)hench;

				if ( friend.HenchDead > 0 )
				{
					friend.Name = "creature henchman";
					friend.HenchDead = 0;
					friend.InvalidateProperties();
					Caster.PlaySound( 0x214 );
				}
				else
				{
					Caster.SendMessage("They are not dead.");
				}
			}
			else
			{
				Caster.SendMessage("This spell didn't seem to work.");
			}
            FinishSequence();
		}
 
        private class InternalTarget : Target
        {
            private ResurrectionSpell m_Owner;
 
            public InternalTarget( ResurrectionSpell owner ) : base( 1, false, TargetFlags.Beneficial )
            {
                m_Owner = owner;
            }
 
            protected override void OnTarget( Mobile from, object o )
            {
                if ( o is Mobile )
                {
                    m_Owner.Target( (Mobile)o );
                }
                else if ( o is Item )
                {
                    m_Owner.ItemTarget( (Item)o );
                }
            }
 
            protected override void OnTargetFinish( Mobile from )
            {
                m_Owner.FinishSequence();
            }
        }
    }
}