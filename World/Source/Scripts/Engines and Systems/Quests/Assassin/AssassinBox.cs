using Server.Network;
using Server.Misc;
using Server.Utilities;

namespace Server.Items
{
	public class BoxOfAtonement : Item
	{
		[Constructable]
		public BoxOfAtonement() : base( 0x9A8 )
		{
			Name = "Box of Atonement";
			Hue = 0x497;
			Movable = false;
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is Gold )
			{
				int nPenalty = AssassinFunctions.QuestFailure( from );
				if ( ItemUtilities.ConsumeRequired(dropped, nPenalty) )
				{
					PlayerSettings.ClearQuestInfo( from, "AssassinQuest" );
					AssassinFunctions.QuestTimeAllowed( from );
					from.PrivateOverheadMessage(MessageType.Regular, 1153, false, "Your failure in this task has been forgiven.", from.NetState);
				}

				return dropped.Deleted;
			}

			return false;
		}			

		public BoxOfAtonement( Serial serial ) : base( serial )
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