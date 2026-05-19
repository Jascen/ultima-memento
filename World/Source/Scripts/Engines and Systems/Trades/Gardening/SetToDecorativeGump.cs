using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.Plants
{
	public class SetToDecorativeGump : Gump
	{
		private PlantItem m_Plant;

		public SetToDecorativeGump( PlantItem plant ) : base( 20, 20 )
		{
			m_Plant = plant;

			DrawBackground();

			AddLabel( 115, 85, 0x44, "Set plant" );
			AddLabel( 82, 105, 0x44, "to decorative mode?" );

			AddButton( 98, 140, 0x47E, 0x480, 1, GumpButtonType.Reply, 0 ); // Cancel

			AddButton( 138, 141, 0xD2, 0xD2, 2, GumpButtonType.Reply, 0 ); // Help
			AddLabel( 143, 141, 0x835, "?" );

			AddButton( 168, 140, 0x481, 0x483, 3, GumpButtonType.Reply, 0 ); // Ok
		}

		private void DrawBackground()
		{
			AddBackground( 50, 50, 200, 150, 0xE10 );

			AddItem( 25, 45, 0xCEB );
			AddItem( 25, 118, 0xCEC );

			AddItem( 227, 45, 0xCEF );
			AddItem( 227, 118, 0xCF0 );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( info.ButtonID == 0 || m_Plant.Deleted || m_Plant.PlantStatus != PlantStatus.Stage9 )
				return;
			
			if ( info.ButtonID == 3 && !from.InRange( m_Plant.GetWorldLocation(), 3 ) )
			{
				from.LocalOverheadMessage( MessageType.Regular, 0x3E9, 500446 ); // That is too far away.
				return;
			}

			if ( !m_Plant.IsUsableBy( from ) )
			{
				m_Plant.LabelTo( from, 1061856 ); // You must have the item in your backpack or locked down in order to use it.
				return;
			}

			switch ( info.ButtonID )
			{
				case 1: // Cancel
				{
					from.SendGump( new ReproductionGump( m_Plant ) );

					break;
				}
				case 2: // Help
				{
					from.SendGump(
						new InfoHelpGump(
							from,
							"Decorative Mode",
							"When a plant reaches its maximum growth level, a Decorative Mode symbol will appear in the Resources Menu.  This symbol is displayed as a leafy plant with a red / symbol through it.<BR><BR>Pressing this button (and clicking Yes to apply) will set the plant to Decorative Mode.<BR><BR>A plant set to Decorative Mode will not produce seeds or resources, and cannot be used for cross-pollination.  A Decorative Plant does not need upkeep, however, and is always in a healthy state.<BR><BR>Decorative Mode should be activated if you simply want your plant to be used as a house decoration, and therefore do not want to have to water it or keep it healthy.<BR><BR>A Decorative Plant will have the tag [decorative] displayed above it when single-clicked.<BR><BR>IMPORTANT NOTE : Once a plant is set to Decorative Mode, it cannot be set back to its normal state.  A Decorative Plant will never produce seeds or resources again.",
							onClose: () => from.SendGump( new SetToDecorativeGump( m_Plant ) )
						)
					);

					break;
				}
				case 3: // Ok
				{
					m_Plant.PlantStatus = PlantStatus.DecorativePlant;
					m_Plant.LabelTo( from, 1053077 ); // You prune the plant. This plant will no longer produce resources or seeds, but will require no upkeep.

					break;
				}
			}
		}
	}
}