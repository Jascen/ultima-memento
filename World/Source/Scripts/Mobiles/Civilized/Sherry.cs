using System;
using Server; 
using Server.Misc;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Net;
using Server.Network;
using Server.Mobiles;
using Server.Accounting;
using Server.Guilds;
using Server.Items;
using Server.Gumps;
using Server.Commands;

namespace Server.Mobiles
{
	public class SherryTheMouse : BasePerson
	{
		private DateTime m_NextTalk;
		public DateTime NextTalk{ get{ return m_NextTalk; } set{ m_NextTalk = value; } }
		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if( m is PlayerMobile )
			{
				if ( DateTime.Now >= m_NextTalk && InRange( m, 4 ) && InLOS( m ) )
				{
					Say("Squeak");
					m_NextTalk = (DateTime.Now + TimeSpan.FromSeconds( 30 ));
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime NextFeed { get; set; }

		[Constructable]
		public SherryTheMouse() : base( )
		{
			SpeechHue = Utility.RandomTalkHue();
			NameHue = 1276;

			Body = 238;
			BaseSoundID = 0xCC;

			Name = "Sherry";
			Title = "the Mouse";
			Direction = Direction.East;
			CantWalk = true;

			SetStr( 100 );
			SetDex( 100 );
			SetInt( 100 );

			SetDamage( 15, 20 );
			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 25, 30 );
			SetResistance( ResistanceType.Cold, 25, 30 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.FistFighting, 100 );
			Karma = 1000;
			VirtualArmor = 30;
		}

		public override void OnDoubleClick( Mobile from )
		{
			bool CanTalk = true;

			if ( !(this.CanSee( from )) ){ CanTalk = false; }
			if ( !(this.InLOS( from )) ){ CanTalk = false; }

			if ( CanTalk )
			{
				this.PlaySound( 0x0CD );
				from.CloseGump( typeof( SherryGump ) );
				from.SendGump( new SherryGump( from, this ) );
			}
			else
			{
				from.SendMessage( "She is too far away from you." );
			}
		}

		public class SherryGump : Gump
		{
			public Mobile mouse;

			public SherryGump( Mobile from, Mobile rat ): base( 50, 50 )
			{
				mouse = rat;
				this.Closable=true;
				this.Disposable=false;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);
				AddImage(20, 16, 1243);
				AddButton(202, 247, 2020, 2020, 1, GumpButtonType.Reply, 0);
				AddHtml( 62, 288, 178, 27, @"<BODY><BASEFONT Color=#111111><BIG><CENTER>Sherry the Mouse</CENTER></BIG></BASEFONT></BODY>", (bool)false, (bool)false);
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				Mobile from = state.Mobile; 

				mouse.PlaySound( 0x0CD );

				if ( info.ButtonID > 0 )
				{
					switch ( Utility.RandomMinMax( 0, 8 ) )
					{

						case 0:	mouse.Say("Oft have I wished that stranger would return."); break;
						case 1:	mouse.Say("We must bring the shards into harmony, so that they resonate in such a manner that matches the original universe."); break;
						case 2:	mouse.Say("Yet sometimes one must sacrifice a pawn to save a king."); break;
						case 3:	mouse.Say("Suddenly the shutters blew open and Lord British fell to the ground, one hand shielding his eyes."); break;
						case 4:	mouse.Say("I witnessed them all from my tiny mousehole."); break;
						case 5:	mouse.Say("But I am but a mouse, and none hear me."); break;
						case 6:	mouse.Say("A shard of a universe is a powerful thing."); break;
						case 7:	mouse.Say("Aid the nobility that resideth in human heart."); break;
						case 8:	mouse.Say("Even pawns have lives and loves at home, my lord."); break;
					}
				}
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is CheeseWheel || dropped is CheeseWedge || dropped is CheeseSlice )
			{
				if ( DateTime.Now < NextFeed )
				{
					PrivateOverheadMessage(MessageType.Regular, 1153, false, "My tummy hurts..." , from.NetState);
					return false;
				}

				if ( Utility.RandomDouble() < 0.1 )
				{
					NextFeed = DateTime.Now.AddMinutes(Utility.RandomMinMax( 13, 30 ));
				}

				this.PlaySound( 0x0CD );

				string sMessage = "Squeak";

				int relic = Utility.RandomMinMax( 1, 59 );

				int chance = dropped.Amount;
					if ( chance > 75 ){ chance = 75; }

				int pick = Utility.RandomMinMax( 0, 8 );
					if ( chance >= Utility.RandomMinMax( 1, 100 ) ){ pick = 9; }

				switch ( pick )
				{
					case 0:	sMessage = "I heard that the " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 1 ) + " can be obtained in " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 0 ) + "."; break;
					case 1:	sMessage = "Nystal said something about the " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 1 ) + " and " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 0 ) + "."; break;
					case 2:	sMessage = "Someone told Lord British that " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 0 ) + " is where you would look for the " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 1 ) + "."; break;
					case 3:	sMessage = "Lord British would tell me tales of knights going to " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 0 ) + " and bringing back the " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 1 ) + "."; break;
					case 4:	sMessage = QuestCharacters.RandomWords() + " was in the kitchen whispering about the " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 1 ) + " and " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 0 ) + "."; break;
					case 5:	sMessage = "I saw a note from the " + RandomThings.GetRandomJob() + ", and it mentioned the " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 1 ) + " and " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 0 ) + "."; break;
					case 6:	sMessage = "Lord British met with " + QuestCharacters.RandomWords() + " and told them to bring back the " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 1 ) + " from " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 0 ) + "."; break;
					case 7:	sMessage = "I heard that the " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 1 ) + " can be found in " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 0 ) + "."; break;
					case 8:	sMessage = "Someone from " + RandomThings.GetRandomCity() + " died in " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 0 ) + " searching for the " + Server.Items.SomeRandomNote.GetSpecialItem( relic, 1 ) + "."; break;
					case 9:	sMessage = Server.Misc.TavernPatrons.GetRareLocation( this, false, false );		break;
				}
				this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sMessage, from.NetState);
				dropped.Delete();
				return true;
			}

			return base.OnDragDrop( from, dropped );
		}

		public SherryTheMouse( Serial serial ) : base( serial )
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