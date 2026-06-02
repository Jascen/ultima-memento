using System;
using Server.Network;
using Server.Targeting;
using Server.Utilities;

namespace Server.Items
{
	public interface ILockpickable : IPoint2D
	{
		int LockLevel{ get; set; }
		bool Locked{ get; set; }
		Mobile Picker{ get; set; }
		int MaxLockLevel{ get; set; }
		int RequiredSkill{ get; set; }

		void LockPick( Mobile from );
	}

	[FlipableAttribute( 0x14fc, 0x14fb )]
	public class Lockpick : Item
	{
		public override string DefaultDescription
		{
			get
			{
				if ( Technology )
					return "Those skilled in lockpicking, can use these to open technological locked items. Use the access card and select the locked items to attempt to open it.";

				return "Those skilled in lockpicking, can use these to open locked items. Use the lockpick and select the locked item to attempt to open it.";
			}
		}

		[Constructable]
		public Lockpick() : this( 1 )
		{
		}

		[Constructable]
		public Lockpick( int amount ) : base( 0x14FC )
		{
			Stackable = true;
			Amount = amount;
			Weight = 0.1;
		}

		public Lockpick( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 0.1 )
				Weight = -1;
		}

		public override void OnDoubleClick( Mobile from )
		{
			from.SendLocalizedMessage( 502068 ); // What do you want to pick?
			from.Target = new InternalTarget( this );
		}

		private class InternalTarget : Target
		{
			private Lockpick m_Item;

			public InternalTarget( Lockpick item ) : base( 1, false, TargetFlags.None )
			{
				m_Item = item;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Item.Deleted )
					return;

				if ( targeted is BaseDoor && from.Skills[SkillName.Lockpicking].Value >= 30 )
				{
					UnlockUtilities.TryUnlockDoor( from, (BaseDoor)targeted, m_Item, UnlockUtilities.LockpickDoorOptions );
				}
				else if ( targeted is ILockpickable )
				{
					Item item = (Item)targeted;
					from.Direction = from.GetDirectionTo( item );

					if ( item.Catalog == Catalogs.SciFi && ((ILockpickable)targeted).Locked && m_Item.ItemID != 0x3A75 )
					{
						from.SendMessage( "This doesn't have a key hole, but it does have a card slot." );
					}
					else if ( item.Catalog == Catalogs.SciFi && ((ILockpickable)targeted).Locked && m_Item.ItemID == 0x3A75 )
					{
						from.PlaySound( 0x54B );
						new InternalTimer( from, (ILockpickable)targeted, m_Item ).Start();
					}
					else if ( ((ILockpickable)targeted).Locked && m_Item.ItemID != 0x3A75 )
					{
						from.PlaySound( 0x241 );
						new InternalTimer( from, (ILockpickable)targeted, m_Item ).Start();
					}
					else if ( ((ILockpickable)targeted).Locked && m_Item.ItemID == 0x3A75 )
					{
						from.SendMessage( "You don't see a card slot on this." );
					}
					else
					{
						// The door is not locked
						from.SendLocalizedMessage( 502069 ); // This does not appear to be locked
					}
				}
				else
				{
					from.SendLocalizedMessage( 501666 ); // You can't unlock that!
				}
			}

			private class InternalTimer : Timer
			{
				private Mobile m_From;
				private ILockpickable m_Item;
				private Lockpick m_Lockpick;
			
				public InternalTimer( Mobile from, ILockpickable item, Lockpick lockpick ) : base( TimeSpan.FromSeconds( 3.0 ) )
				{
					m_From = from;
					m_Item = item;
					m_Lockpick = lockpick;
					Priority = TimerPriority.TwoFiftyMS;
				}

				protected void BrokeLockPickTest()
				{
					// When failed, a 25% chance to break the lockpick
					if ( Utility.Random( 4 ) == 0 )
					{
						Item item = (Item)m_Item;

						// You broke the lockpick.
						if ( m_Lockpick.ItemID == 0x3A75 ){ m_From.PlaySound( 0x549 ); m_From.PrivateOverheadMessage( 0, 1150, false,  "You broke the key card.", m_From.NetState ); }
						else { m_From.PlaySound( 0x3A4 ); m_From.PrivateOverheadMessage( 0, 1150, false,  "You broke the lockpick.", m_From.NetState ); }

						m_Lockpick.Consume();
					}
				}
				
				protected override void OnTick()
				{
					Item item = (Item)m_Item;

					if ( m_From.Skills[SkillName.Lockpicking].Base < 1 )
					{
						int cycle = 10;

						while ( cycle > 0 )
						{
							cycle--;
							m_From.CheckTargetSkill( SkillName.Lockpicking, m_Item, 0, 10 );
						}
					}

					if ( !m_From.InRange( item.GetWorldLocation(), 2 ) )
						return;

					if ( m_Item.LockLevel == 0 || m_Item.LockLevel == -255 )
					{
						// LockLevel of 0 means that the door can't be picklocked
						// LockLevel of -255 means it's magic locked
						if ( m_Lockpick.ItemID == 0x3A75 ){ m_From.PrivateOverheadMessage( 0, 1150, false,  "This lock cannot be hacked by normal means.", m_From.NetState ); }
						else { m_From.PrivateOverheadMessage( 0, 1150, false,  "This lock cannot be picked by normal means.", m_From.NetState ); }

						return;
					}

					if ( (m_From.Skills[SkillName.Lockpicking].Value+2) < m_Item.RequiredSkill )
					{
						/*
						// Do some training to gain skills
						m_From.CheckSkill( SkillName.Lockpicking, 0, m_Item.LockLevel );*/

						// The LockLevel is higher thant the LockPicking of the player
						if ( m_Lockpick.ItemID == 0x3A75 ){ m_From.PrivateOverheadMessage( 0, 1150, false,  "You don't see how that lock can be hacked.", m_From.NetState ); }
						else { m_From.PrivateOverheadMessage( 0, 1150, false,  "You don't see how that lock can be manipulated.", m_From.NetState ); }
						return;
					}

					if ( m_From.CheckTargetSkill( SkillName.Lockpicking, m_Item, m_Item.LockLevel, m_Item.MaxLockLevel ) )
					{
						// Success! Pick the lock!
						if ( m_Lockpick.ItemID == 0x3A75 ){ m_From.PlaySound( 0x54B ); m_From.PrivateOverheadMessage( 0, 1150, false,  "Your skill at hacking worked.", m_From.NetState ); }
						else { m_From.PlaySound( 0x4A ); m_From.PrivateOverheadMessage( 0, 1150, false,  "The lock quickly yields to your skill.", m_From.NetState ); }
						
						m_Item.LockPick( m_From );
					}
					else
					{
						// The player failed to pick the lock
						BrokeLockPickTest();

						if ( m_Lockpick.ItemID == 0x3A75 ){ m_From.PrivateOverheadMessage( 0, 1150, false,  "You are unable to hack the lock.", m_From.NetState ); }
						else { m_From.PrivateOverheadMessage( 0, 1150, false,  "You are unable to pick the lock.", m_From.NetState ); }

                        // ==== Random Item Disintergration upon Failure ====
                        if (m_Item is TreasureMapChest)
                        {
                            int i_Num = 0; Item i_Destroy = null;

                            BaseContainer m_chest = m_Item as BaseContainer;                            
                            Item Dust = new DustPile();
                            
                            for (int i = 10; i > 0; i--)
                            {
                                i_Num = Utility.Random(m_chest.Items.Count);
                                // Make sure DustPiles aren't called for destruction
                                if ((m_chest.Items.Count > 0) && m_chest.Items[i_Num] is DustPile)
                                {
                                    for (int ci = (m_chest.Items.Count - 1); ci >= 0; ci--)
                                    {
                                        i_Num = ci;
                                        if (i_Num < 0) { i_Num = 0; }

                                        if (m_chest.Items[i_Num] is DustPile)
                                        {
                                            i_Destroy = null;
                                        }
                                        else
                                        {
                                            i_Destroy = m_chest.Items[i_Num];
                                            i = 0;
                                        }
                                        // Nothing left but Dust
                                        if (ci < 0 && i > 0)
                                        {
                                            i_Destroy = null; i = 0;
                                        }
                                    }
                                }
                                // Item targeted =+= prepare for object DOOM! >;D
                                else
                                {
                                    i_Destroy = m_chest.Items[i_Num]; i = 0;
                                }
                            }                            
                            // Delete chosen Item and drop a Dust Pile
                            if (i_Destroy is Gold)
                            {
                                if (i_Destroy.Amount > 1000)
                                    i_Destroy.Amount -= 1000;
                                else
                                    i_Destroy.Delete();

                                Dust.Hue = 1177; m_chest.DropItem(Dust);
                            }
                            else if (i_Destroy != null)
                            {
                                i_Destroy.Delete(); m_chest.DropItem(Dust);
                            }
                            Effects.PlaySound(m_chest.Location, m_chest.Map, 0x1DE);
                            m_chest.PublicOverheadMessage(MessageType.Regular, 2004, false, "The sound of gas escaping is heard from the chest.");
                        }
					}
				}
			}
		}
	}
}