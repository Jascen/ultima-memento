using System;
using Server;
using Server.Mobiles;
using Server.Misc;
using System.Collections.Generic;
using System.Collections;
using Server.Network;
using Server.Gumps;

namespace Server.Items
{
	public class HayCrate : Item
	{
		public string HayTown;
		[CommandProperty(AccessLevel.Owner)]
		public string Hay_Town { get { return HayTown; } set { HayTown = value; InvalidateProperties(); } }

		[Constructable]
		public HayCrate() : base( 0x0701 )
		{
			Name = "hay";
			Movable = false;
		}

		public static void GetNearbyTown( HayCrate hay )
		{
			if ( hay.Map == Map.SerpentIsland )
			{
				hay.HayTown = "the City of Furnace";
			}
			else
			{
				foreach ( Mobile citizen in hay.GetMobilesInRange( 200 ) )
				{
					if ( citizen is BaseVendor || citizen is TownGuards || ( citizen is Citizens && !(citizen is HouseVisitor) ) )
					{
						if ( citizen.Region.Name != null ){ hay.HayTown = Server.Misc.Worlds.GetRegionName( citizen.Map, citizen.Location ); }
					}
				}
			}

			if ( hay.HayTown == null || hay.HayTown == "" ){ hay.HayTown = "the City of Britain"; }
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( this.GetWorldLocation(), 2 ) )
			{
				bool LookInside = true;

				if ( from.Backpack.FindItemByType( typeof ( ThiefNote ) ) != null )
				{
					Item mail = from.Backpack.FindItemByType( typeof ( ThiefNote ) );
					ThiefNote envelope = (ThiefNote)mail;

					if ( envelope.NoteOwner == from && envelope.NoteItemGot > 0 && HayTown == envelope.NoteDeliverTo && envelope.NoteDeliverType == 2 )
					{
						LoggingFunctions.LogStandard( from, "has stolen " + envelope.NoteItem + "." );
						from.AddToBackpack ( new Gold( envelope.NoteReward ) );
						CustomEventSink.InvokeCombatQuestCompleted( from, envelope.NoteReward );
						Titles.AwardFame( from, ((int)(envelope.NoteReward/100)), true );
						Titles.AwardKarma( from, -((int)(envelope.NoteReward/100)), true );
						Server.Items.ThiefNote.SetupNote( envelope, from );
						from.LocalOverheadMessage(MessageType.Emote, 1150, true, "You collected your reward.");
						from.SendMessage( "You found another secret note for you." );
						from.SendSound( 0x3D );
						from.CloseGump( typeof( Server.Items.ThiefNote.NoteGump ) );
						Server.Items.ThiefNote.ThiefTimeAllowed( from );
						LookInside = false;
					}
				}

				if ( LookInside )
				{
					string message = "There is nothing of interest in here.";

					if ( Utility.RandomMinMax( 1, 20 ) == 1 && Stackable == false )
					{
						Item randItem = Loot.RandomItem( from, Utility.RandomMinMax(1,4) );
						randItem.MoveToWorld( from.Location, from.Map );
						message = "You pull something out of the hay and it falls by your feet.";
					}
					Stackable = true;

					from.SendSound( 0x057 );
					from.SendMessage( message );
				}
			}
			else
			{
				from.SendLocalizedMessage( 502138 ); // That is too far away for you to use
			}
		}

		public HayCrate( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
            writer.Write( HayTown );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			HayTown = reader.ReadString();
		} 
	}
}