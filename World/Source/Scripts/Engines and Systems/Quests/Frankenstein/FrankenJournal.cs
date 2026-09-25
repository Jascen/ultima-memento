using Server.Network;
using Server.Gumps;
using Server.Utilities;
using System;

namespace Server.Items
{
	public class FrankenJournal : Item
	{
		public Mobile JournalOwner;
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Journal_Owner { get{ return JournalOwner; } set{ JournalOwner = value; InvalidateProperties(); } }

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[CommandProperty( AccessLevel.GameMaster )]
		public bool HasHead { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool HasTorso { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public int BrainLevel { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool HasBrain { get { return 0 < BrainLevel; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool HasArmLeft { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool HasArmRight { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool HasLegLeft { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool HasLegRight { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string BrainFrom { get; set; }

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[Constructable]
		public FrankenJournal() : base( 0x1A97 )
		{
			Weight = 1.0;
			Hue = 0xB51;
			Name = "Frankenstein's Journal";
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			if ( JournalOwner != null ){ list.Add( 1049644, "Now Belongs to " + JournalOwner.Name + "" ); }
        }

		public override void AppendChildProperties(ObjectPropertyList list)
		{
			base.AppendChildProperties(list);

			if ( JournalOwner != null )
			{
				list.Add( 1049644, string.Format( "Brain Level: {0}", HasBrain ? BrainLevel.ToString() : "Missing" ) );

				int parts = 0;
				if (HasHead) parts++;
				if (HasTorso) parts++;
				if (HasArmLeft) parts++;
				if (HasArmRight) parts++;
				if (HasLegLeft) parts++;
				if (HasLegRight) parts++;

				list.Add( 1049644, string.Format( "{0}/6 Body Parts", parts ) );
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1060640 ); // The item must be in your backpack to use it.
				return;
			}

			if ( JournalOwner == null )
			{
				var journal = WorldUtilities.FirstOrDefault<FrankenJournal>( item => item.JournalOwner == from && item != this );
				if ( journal != null )
				{
					from.SendMessage( "You already have a journal! You can only have one at a time." );
					return;
				}

				Journal_Owner = from;
			}

			if ( JournalOwner != from )
			{
				from.SendMessage( "This journal does not belong to you!" );
				return;
			}

			from.SendSound( 0x55 );
			from.CloseGump( typeof( FrankenGump ) );
			from.SendGump( new FrankenGump( this, from ) );
		}

		public FrankenJournal(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 2);
			writer.Write( (Mobile)JournalOwner);
			writer.Write( HasHead );
			writer.Write( HasTorso );
			writer.Write( BrainLevel );
			writer.Write( HasArmLeft );
			writer.Write( HasArmRight );
			writer.Write( HasLegLeft );
			writer.Write( HasLegRight );
			writer.Write( BrainFrom );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			JournalOwner = reader.ReadMobile();
			if ( version == 1 ) Console.WriteLine("JournalOwner: {0}", JournalOwner != null ? JournalOwner.Name : "Unknown");

			HasHead = version < 1 ? reader.ReadInt() == 1 : reader.ReadBool();
			HasTorso = version < 1 ? reader.ReadInt() == 1 : reader.ReadBool();
			BrainLevel = reader.ReadInt();
			HasArmLeft = version < 1 ? reader.ReadInt() == 1 : reader.ReadBool();
			HasArmRight = version < 1 ? reader.ReadInt() == 1 : reader.ReadBool();
			HasLegLeft = version < 1 ? reader.ReadInt() == 1 : reader.ReadBool();
			HasLegRight = version < 1 ? reader.ReadInt() == 1 : reader.ReadBool();
			BrainFrom = reader.ReadString();
		}

		private class FrankenGump : Gump
		{
			private FrankenJournal m_Journal;
			private Mobile m_From;

			public FrankenGump( FrankenJournal book, Mobile from ) : base( 50, 50 )
			{
				string color = "#edad9c";
				m_Journal = book;
				m_From = from;

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				AddImage(0, 0, 7017, Server.Misc.PlayerSettings.GetGumpHue( from ));

				AddHtml( 12, 12, 420, 20, @"<BODY><BASEFONT Color=" + color + ">FRANKENSTEIN'S JOURNAL</BASEFONT></BODY>", (bool)false, (bool)false);

				AddButton(563, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);

				AddHtml( 14, 44, 575, 360, @"<BODY><BASEFONT Color=" + color + ">This book contains the writings of Doctor Victor Frankenstein, a notable alchemist and forensic expert. Within these pages, are the secrets to reanimating a creature that can serve your purposes. Where most only have achieved such creatures of human size, this tome explains how to create a creature of great power. To do this, one would need to be at least a neophyte undertaker. While carrying this book, and using a bladed item to skin creatures, you must find the corpses of giants to obtain the body parts necessary for the construction of such a creature. Giants are creatures like ogres, ettins, and cyclops. These body parts may be difficult to sever from the creature, so you may have to slay many to collect what you need. If you get body parts you don’t need, then perhaps the undertaker in the Black Magic Guild will procure them from you.<br><br>As you collect individual severed parts, double click them and target this journal to add it to your upcoming experiment. You may only have one of each body part for this experiment: a torso, head, left arm, right arm, left leg, and right leg. You will also need a brain from a giant, and the more powerful the better. A brain of a storm giant will give your creation more power than the brain of a stupid ogre. Once you have a brain, add it to your experiment in the same manner. Unlike other body parts, you can add a different brain later on before running the final experiment. Whenever you add a different brain, you will throw the old one away.<br><br>Once you have everything you need, you then need to find a power coil that can generate enough electrical energy to reanimate the corpse. The undertaker I wrote of earlier has one in their lab, but he also will sell you a finely tinkered one to place in your home. When you are close enough to a power coil, then select the type of creature you want to reanimate. You have your choice of a reanimated warrior or a slave to carry your items for you. The warrior will fight at your command, while the other will carry your items and other creatures seem to leave it be.<br><br>An item will appear in your pack that will allow you to summon the creature. Once summoned, the item will vanish until you release the creature and then the item will reappear in your pack. If the warrior creature were to die in battle, then the item will appear in your pack as well. In order to summon your reanimation, you will need embalming fluid to keep it from rotting away. Undertakers sell these at high prices, but one good a forensics can sometimes find them on the corpses of other reanimations, zombies, or mummies. If you manage to get some embalming fluid, simply use it on the reanimation’s item in your pack to add the preservative.</BASEFONT></BODY>", (bool)false, (bool)true);

				int v = 35;

				if ( book.HasBrain )
				{
					AddItem(12, 430, 9698);
					AddHtml( 55, 430, 261, 20, @"<BODY><BASEFONT Color=" + color + ">From " + book.BrainFrom + "</BASEFONT></BODY>", (bool)false, (bool)false);
					AddHtml( 55, 460, 261, 20, @"<BODY><BASEFONT Color=" + color + ">Level " + book.BrainLevel + " Brain</BASEFONT></BODY>", (bool)false, (bool)false);
				}

				if ( book.HasArmRight ){ AddItem(449, 417+v, 14988); } // RIGHT ARM
				if ( book.HasArmLeft ){ AddItem(547, 417+v, 14991); } // LEFT ARM
				if ( book.HasLegRight ){ AddItem(471, 467+v, 16025); } // RIGHT LEG
				if ( book.HasLegLeft ){ AddItem(522, 466+v, 16002); } // LEFT LEG
				if ( book.HasTorso ){ AddItem(491, 415+v, 15003); } // TORSO
				if ( book.HasHead ){ AddItem(504, 399+v, 15873); } // HEAD

				if ( book.HasBrain && book.HasTorso && book.HasHead && book.HasArmLeft && book.HasArmRight && book.HasLegLeft && book.HasLegRight )
				{
					AddButton(12, 535, 4005, 4005, 1, GumpButtonType.Reply, 0);
					AddHtml( 55, 535, 261, 20, @"<BODY><BASEFONT Color=" + color + ">Reanimate a Slave</BASEFONT></BODY>", (bool)false, (bool)false);

					AddButton(12, 565, 4005, 4005, 2, GumpButtonType.Reply, 0);
					AddHtml( 55, 565, 261, 20, @"<BODY><BASEFONT Color=" + color + ">Reanimate a Protector</BASEFONT></BODY>", (bool)false, (bool)false);
				}
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( info.ButtonID > 0 )
				{
					Point3D loc = m_From.Location;
					Map map = m_From.Map;

					bool nearCoil = false;
					foreach ( Item coil in m_From.GetItemsInRange( 10 ) )
					{
						if ( coil is PowerCoil )
						{
							nearCoil = true;
							loc = new Point3D(coil.X, coil.Y, (coil.Z+20));
						}
					}

					if ( nearCoil )
					{
						int Fighter = info.ButtonID-1;

						FrankenPorterItem flesh = new FrankenPorterItem();

						string QuestLog = "has reanimated a flesh golem";

						flesh.PorterOwner = m_From;
						flesh.PorterLevel = m_Journal.BrainLevel;
						flesh.PorterType = Fighter;

						m_From.AddToBackpack ( flesh );

						Server.Misc.LoggingFunctions.LogGenericQuest( m_From, QuestLog );

						m_From.PrivateOverheadMessage(MessageType.Regular, 1153, false, "My experiment is a success.", m_From.NetState);

						int sound = Utility.RandomList( 0x028, 0x029 );
						Effects.SendLocationEffect( loc, map, 0x2A4E, 30, 10, 0, 0 );
						m_From.PlaySound( sound );

						m_Journal.Delete();
					}
					else
					{
						m_From.SendMessage("You need to be near a power coil to do that.");
						m_From.SendSound( 0x55 );
					}
				}
				else
				{
					m_From.SendSound( 0x55 );
					m_From.CloseGump( typeof( FrankenGump ) );
				}
			}
		}
	}
}