using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Utilities;
using Server.ModernSkill;

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
		private enum LockpickDifficulty
		{
			NotLocked = 0,
			Trivial,
			Easy,
			Difficult,
			Challenging,
			Impossible,
		}
		
		public override string DefaultDescription
		{
			get
			{
				if ( Technology )
					return "Those skilled in lockpicking, can use these to open technological locked items. Use the access card and select the locked items to attempt to open it.";

				return "Those skilled in lockpicking, can use these to open locked items. Use the lockpick and select the locked item to attempt to open it.";
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsSciFi { get { return ItemID == 0x3A75; } }

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

		public static string GetDifficulty( double removeTrapSkill, object target )
		{
			switch (GetDifficultyInternal(removeTrapSkill, target))
			{
				default:
				case LockpickDifficulty.NotLocked: return TextDefinition.GetColorizedText("Not Locked", HtmlColors.COOL_GREEN);
				case LockpickDifficulty.Trivial: return  TextDefinition.GetColorizedText("Trivial", HtmlColors.MINT_GREEN);
				case LockpickDifficulty.Easy: return TextDefinition.GetColorizedText("Easy", HtmlColors.MINT_GREEN);
				case LockpickDifficulty.Difficult: return TextDefinition.GetColorizedText("Difficult", HtmlColors.RUST);
				case LockpickDifficulty.Challenging: return TextDefinition.GetColorizedText("Challenging", HtmlColors.PALE_RED);
				case LockpickDifficulty.Impossible: return TextDefinition.GetColorizedText("Impossible", HtmlColors.RED);
			}
		}

		private static LockpickDifficulty GetDifficultyInternal( double lockpickSkill, object target )
		{
			if ( target is ILockpickable )
			{
				var lockpickable = (ILockpickable)target;
				if ( !lockpickable.Locked ) return LockpickDifficulty.NotLocked;

				if ( lockpickable.MaxLockLevel <= lockpickSkill ) return LockpickDifficulty.Trivial;
				if ( lockpickSkill < lockpickable.RequiredSkill ) return LockpickDifficulty.Impossible;
				
				var delta = (double)( lockpickable.MaxLockLevel - lockpickSkill ) / ( 1 + Math.Abs( lockpickable.MaxLockLevel - lockpickable.LockLevel ) );
				if ( delta < 0.33 ) return LockpickDifficulty.Easy;
				if ( delta < 0.66 ) return LockpickDifficulty.Difficult;

				return LockpickDifficulty.Challenging;
			}

			return LockpickDifficulty.NotLocked;
		}

		public static bool ValidateLockpickType( Lockpick lockpick, Item targeted )
		{
			return lockpick.IsSciFi ? targeted.Catalog == Catalogs.SciFi : true;
		}

		public static bool CanDoEffect( Mobile from, Lockpick lockpick, Item targeted, bool isLocked )
		{
			from.Direction = from.GetDirectionTo( targeted );

			if ( targeted.Catalog == Catalogs.SciFi && isLocked && !lockpick.IsSciFi )
			{
				from.SendMessage( "This doesn't have a key hole, but it does have a card slot." );
				return false;
			}
			else if ( targeted.Catalog == Catalogs.SciFi && isLocked && lockpick.IsSciFi )
			{
				from.PlaySound( 0x54B );
				return true;
			}
			else if ( isLocked && !lockpick.IsSciFi )
			{
				from.PlaySound( 0x241 );
				return true;
			}
			else if ( isLocked && lockpick.IsSciFi )
			{
				from.SendMessage( "You don't see a card slot on this." );
				return false;
			}
			else
			{
				from.SendLocalizedMessage( 502069 ); // This does not appear to be locked
				return false;
			}
		}

		public static void DoEffect(Mobile from, Lockpick lockpick, Item targeted, Action onComplete)
		{
			new InternalTimer( from, (ILockpickable)targeted, lockpick, onComplete ).Start();
		}

		private class InternalTarget : Target
		{
			private Lockpick m_Item;

			public InternalTarget( Lockpick item ) : base( 2, false, TargetFlags.None )
			{
				m_Item = item;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Item.Deleted )
					return;

				if ( from is PlayerMobile && targeted is Item )
				{
					var player = (PlayerMobile)from;
					if ( player.Preferences.ModernLockpickingEnabled && LockpickAndRemoveTrapGump.TryShow(player, (Item)targeted) )
						return;
				}

				if ( targeted is BaseDoor && from.Skills[SkillName.Lockpicking].Value >= 30 )
				{
					UnlockUtilities.TryUnlockDoor( from, (BaseDoor)targeted, m_Item, UnlockUtilities.LockpickDoorOptions );
				}
				else if ( targeted is ILockpickable && targeted is Item )
				{
					if (CanDoEffect(from, m_Item, (Item)targeted, ((ILockpickable)targeted).Locked))
					{
						DoEffect(from, m_Item, (Item)targeted, null);
					}
				}
				else
				{
					from.SendLocalizedMessage( 501666 ); // You can't unlock that!
				}
			}
		}

		private class InternalTimer : Timer
		{
			private readonly Mobile m_From;
			private readonly ILockpickable m_Item;
			private readonly Lockpick m_Lockpick;
			private readonly Action m_OnComplete;

			public InternalTimer( Mobile from, ILockpickable item, Lockpick lockpick, Action onComplete ) : base( TimeSpan.FromSeconds( 1.0 ) )
			{
				m_From = from;
				m_Item = item;
				m_Lockpick = lockpick;
				m_OnComplete = onComplete;
				Priority = TimerPriority.TwoFiftyMS;
			}

			protected void BrokeLockPickTest()
			{
				// When failed, a 25% chance to break the lockpick
				if ( Utility.Random( 4 ) == 0 )
				{
					// You broke the lockpick.
					if ( m_Lockpick.IsSciFi ){ m_From.PlaySound( 0x549 ); m_From.PrivateOverheadMessage( 0, 1150, false,  "You broke the key card.", m_From.NetState ); }
					else { m_From.PlaySound( 0x3A4 ); m_From.PrivateOverheadMessage( 0, 1150, false,  "You broke the lockpick.", m_From.NetState ); }

					m_Lockpick.Consume();
				}
			}
			
			protected override void OnTick()
			{
				try
				{
					Item item = (Item)m_Item;
					if ( !m_From.InRange( item.GetWorldLocation(), 2 ) )
						return;

					if ( m_Item.LockLevel == 0 || m_Item.LockLevel == -255 )
					{
						// LockLevel of 0 means that the door can't be picklocked
						// LockLevel of -255 means it's magic locked
						if ( m_Lockpick.IsSciFi ){ m_From.PrivateOverheadMessage( 0, 1150, false,  "This lock cannot be hacked by normal means.", m_From.NetState ); }
						else { m_From.PrivateOverheadMessage( 0, 1150, false,  "This lock cannot be picked by normal means.", m_From.NetState ); }

						return;
					}

					if ( (m_From.Skills[SkillName.Lockpicking].Value+2) < m_Item.RequiredSkill )
					{
						/*
						// Do some training to gain skills
						m_From.CheckSkill( SkillName.Lockpicking, 0, m_Item.LockLevel );*/

						// The LockLevel is higher thant the LockPicking of the player
						if ( m_Lockpick.IsSciFi ){ m_From.PrivateOverheadMessage( 0, 1150, false,  "You don't see how that lock can be hacked.", m_From.NetState ); }
						else { m_From.PrivateOverheadMessage( 0, 1150, false,  "You don't see how that lock can be manipulated.", m_From.NetState ); }
						return;
					}

					if ( m_From.CheckTargetSkill( SkillName.Lockpicking, m_Item, m_Item.LockLevel, m_Item.MaxLockLevel ) )
					{
						// Success! Pick the lock!
						if ( m_Lockpick.IsSciFi ){ m_From.PlaySound( 0x54B ); m_From.PrivateOverheadMessage( 0, 1150, false,  "Your skill at hacking worked.", m_From.NetState ); }
						else { m_From.PlaySound( 0x4A ); m_From.PrivateOverheadMessage( 0, 1150, false,  "The lock quickly yields to your skill.", m_From.NetState ); }
						
						m_Item.LockPick( m_From );
					}
					else
					{
						// The player failed to pick the lock
						BrokeLockPickTest();

						if ( m_Lockpick.IsSciFi ){ m_From.PrivateOverheadMessage( 0, 1150, false,  "You are unable to hack the lock.", m_From.NetState ); }
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
				finally
				{
					if (m_OnComplete != null)
						m_OnComplete.Invoke();
				}
			}
		}
	}
}