using System;
using Server.Misc;

namespace Server.Items
{
	public class BuriedChest : LockableContainer
	{
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularWood; } }

		[Constructable]
		public BuriedChest( int level, string who, Mobile digger ) : base( 0xe40 )
		{
			if ( who == "" || who == null )
				who = ContainerFunctions.GetOwner( "Chest" );

			string[] vAdj = new string[] {"Exotic", "Mysterious", "Marvelous", "Amazing", "Astonishing", "Mystical", "Astounding", "Magnificent", "Phenomenal", "Fantastic", "Incredible", "Extraordinary", "Fabulous", "Wondrous", "Glorious", "Lost", "Fabled", "Legendary", "Mythical", "Missing", "Ancestral", "Ornate", "Wonderful", "Sacred", "Unspeakable", "Unknown", "Forgotten"};
			string sAdj = vAdj[Utility.RandomMinMax( 0, (vAdj.Length-1) )];

			ContainerFunctions.BuildContainer( this, 0, Utility.RandomList( 1, 2 ), 0, 0 );

			ContainerFunctions.FillTheContainer( level, this, digger );
			if ( level > 3 ){ ContainerFunctions.FillTheContainer( level, this, digger ); }
			if ( level > 7 ){ ContainerFunctions.FillTheContainer( level, this, digger ); }
			if ( GetPlayerInfo.LuckyPlayer( digger.Luck ) ){ ContainerFunctions.FillTheContainer( level, this, digger ); }

			ContainerFunctions.LockTheContainer( level, this, 1 );

			int xTraCash = Utility.RandomMinMax( (level*300), (level*500) );
			LootPackChange.AddGoldToContainer( xTraCash, this, digger, level );

			int artychance = GetPlayerInfo.LuckyPlayerArtifacts( digger.Luck ) + (level * 10);
			if ( Utility.Random( 100 ) < artychance )
			{
				Item arty = Loot.RandomArty();
				DropItem( arty );
			}

			Name = "Chest";
			ColorText1 = "The " + sAdj + " Chest of";
			ColorHue1 = "75b68f";
			ColorText2 = who;
			ColorHue2 = "75b68f";
		}

		public BuriedChest( Serial serial ) : base( serial )
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