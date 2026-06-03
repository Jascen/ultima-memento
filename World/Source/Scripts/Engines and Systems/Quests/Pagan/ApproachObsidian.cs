using System;
using Server.Mobiles;
using Server.Misc;
using Server.Network;
using Server.Utilities;
using Server.Timers;

namespace Server.Items
{
	public class ApproachObsidian : Item
	{
		[Constructable]
		public ApproachObsidian() : base(0x2161)
		{
			Movable = false;
			Visible = false;
			Name = "floor";
		}

		public ApproachObsidian(Serial serial) : base(serial)
		{
		}

		public override bool OnMoveOver( Mobile mobile )
		{
			if ( false == ( mobile is PlayerMobile ) ) return true;

			var m = (PlayerMobile)mobile;
			if ( m.IsTitanOfEther ) return true;

			var tip = m.Backpack.FindItemByType<ObeliskTip>();
			if ( tip == null ) return true;
			if ( tip.ObeliskOwner != mobile ) return true;
			if ( tip.WonAir + tip.WonFire + tip.WonEarth + tip.WonWater < 4 ) return true;

			WorldUtilities.DeleteAllItems<ObeliskTip>( item => item.ObeliskOwner == mobile );

			GoodiesTimer.Create(m);

			m.IsTitanOfEther = true;
			m.RefreshSkillCap();
			m.StatCap += 50;

			Server.Items.QuestSouvenir.GiveReward( m, "Obelisk Tip", 0, 0x185F );
			Server.Items.QuestSouvenir.GiveReward( m, "Breath of Air", 0, 0x1860 );
			Server.Items.QuestSouvenir.GiveReward( m, "Tongue of Flame", 0, 0x1861 );
			Server.Items.QuestSouvenir.GiveReward( m, "Heart of Earth", 0, 0x1862 );
			Server.Items.QuestSouvenir.GiveReward( m, "Tear of the Seas", 0, 0x1863 );

			m.AddToBackpack( new ObsidianGate() );
			if (m.Temptations.LimitTitanBonus) m.AddToBackpack( new SoulStone() );
			m.SendMessage( "Some items have appeared in your pack." );
			m.SendMessage( "You can change your title for this achievement." );
			m.LocalOverheadMessage( MessageType.Emote, 1150, true, "You are now a Titan of Ether!" );
			LoggingFunctions.LogGeneric( m, "has become a Titan of Ether." );

			return true;
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