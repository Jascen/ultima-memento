using System;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Misc;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
	public class Artifact_RodOfResurrection : GiftScepter
	{
		[Constructable]
		public Artifact_RodOfResurrection()
		{
			Name = "Rod Of Resurrection";
			Hue = 0x4AC;
			ArtifactLevel = ArtifactLevel.StandardArtefact;
			Server.Misc.Arty.ArtySetup( this, 15, "" );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( Parent != from )
			{
				from.SendMessage( "You must be holding the rod to resurrect." );
			}
			else
			{
				from.Target = new InternalTarget( from, this );
				from.SendMessage( "Who would you like to resurrect!" );
			}
			return;
		}

        public void Target( Mobile m, Mobile from, Artifact_RodOfResurrection rod )
        {
            if ( m == from )
            {
                SelfTarget( from );
                return;
            }

            if ( !from.CanSee( m ) )
            {
                from.SendLocalizedMessage( 500237 ); // Target can not be seen.
            }
            else if ( !from.Alive )
            {
                from.SendLocalizedMessage( 501040 ); // The resurrecter must be alive.
            }
            else if (m.Alive && !m.IsDeadBondedPet)
            {
                from.SendLocalizedMessage( 501041 ); // Target is not dead.
            }
            else if ( !from.InRange( m, 2 ) )
            {
                from.SendLocalizedMessage( 501042 ); // Target is not close enough.
            }
            else if ( m.Map == null || !m.Map.CanFit( m.Location, 16, false, false ) )
            {
                from.SendLocalizedMessage( 501042 ); // Target can not be resurrected at that location.
                m.SendLocalizedMessage( 502391 ); // Thou can not be resurrected there!
            }
            else if ( m is PlayerMobile )
            {
                m.PlaySound( 0x214 );
                m.FixedEffect( 0x376A, 10, 16 );
 
                m.CloseGump( typeof( ResurrectGump ) );
                m.SendGump( new ResurrectGump( m, from ) );
            }
            else if (m is BaseCreature )
			{
				BaseCreature pet = (BaseCreature)m;
				Mobile master = pet.GetMaster();
 
                m.PlaySound( 0x214 );
                m.FixedEffect( 0x376A, 10, 16 );
 
                master.CloseGump(typeof(PetResurrectGump));
                master.SendGump(new PetResurrectGump(master, pet));
            }
        }

        private void SelfTarget( Mobile from )
        {
            if ( !from.Alive )
            {
                from.SendLocalizedMessage( 501040 ); // The resurrecter must be alive.
                return;
            }

            if ( SoulOrb.FindActive( from ) != null )
            {
                from.SendMessage( "The spirits watch you already." );
                return;
            }

            var orb = SoulOrb.Create( from, SoulOrbType.Default );
            if ( orb != null )
            {
                from.SendMessage( "You feel the spirits watching you, awaiting to send you back to your body." );
                orb.Location = Location;
            }
        }

        public void ItemTarget( Item hench, Mobile from, Artifact_RodOfResurrection rod )
        {
			if ( hench is HenchmanFighterItem )
			{
				HenchmanFighterItem friend = (HenchmanFighterItem)hench;

				if ( friend.HenchDead > 0 )
				{
					friend.Name = "fighter henchman";
					friend.HenchDead = 0;
					friend.InvalidateProperties();
					from.PlaySound( 0x214 );
				}
				else
				{
					from.SendMessage("They are not dead.");
				}
			}
			else if ( hench is HenchmanWizardItem )
			{
				HenchmanWizardItem friend = (HenchmanWizardItem)hench;

				if ( friend.HenchDead > 0 )
				{
					friend.Name = "wizard henchman";
					friend.HenchDead = 0;
					friend.InvalidateProperties();
					from.PlaySound( 0x214 );
				}
				else
				{
					from.SendMessage("They are not dead.");
				}
			}
			else if ( hench is HenchmanArcherItem )
			{
				HenchmanArcherItem friend = (HenchmanArcherItem)hench;

				if ( friend.HenchDead > 0 )
				{
					friend.Name = "archer henchman";
					friend.HenchDead = 0;
					friend.InvalidateProperties();
					from.PlaySound( 0x214 );
				}
				else
				{
					from.SendMessage("They are not dead.");
				}
			}
			else if (hench is HenchmanMonsterItem )
			{
				HenchmanMonsterItem friend = (HenchmanMonsterItem)hench;

				if ( friend.HenchDead > 0 )
				{
					friend.Name = "creature henchman";
					friend.HenchDead = 0;
					friend.InvalidateProperties();
					from.PlaySound( 0x214 );
				}
				else
				{
					from.SendMessage("They are not dead.");
				}
			}
			else
			{
				from.SendMessage("This spell didn't seem to work.");
			}
		}
 
        private class InternalTarget : Target
        {
            private Mobile m_Owner;
            private Artifact_RodOfResurrection m_Rod;
 
            public InternalTarget( Mobile owner, Artifact_RodOfResurrection rod ) : base( 1, false, TargetFlags.Beneficial )
            {
                m_Owner = owner;
				m_Rod = rod;
            }
 
            protected override void OnTarget( Mobile from, object o )
            {
                if ( o is Mobile )
                {
                    m_Rod.Target( (Mobile)o, from, m_Rod );
                }
                else if ( o is Item )
                {
                    m_Rod.ItemTarget( (Item)o, from, m_Rod );
                }
            }
        }

		public Artifact_RodOfResurrection( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
