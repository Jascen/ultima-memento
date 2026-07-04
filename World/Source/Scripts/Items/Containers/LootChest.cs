using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.ContextMenus;
using Server.Engines.PartySystem;
using Server.Misc;

namespace Server.Items
{
	public class LootChest : LockableContainer
	{
		[Constructable]
		public LootChest() : this( 0 )
		{
		}

		[Constructable]
		public LootChest( int level ) : base( 0xe40 )
		{
			Name = "chest";
			ContainerFunctions.BuildContainer( this, 0, Utility.RandomList( 1, 2 ), 0, 0 );
			ContainerFunctions.LockTheContainer( level, this, 1 );
			Weight = 51.0 + (double)level;
			Movable = true;
		}

		private bool NeedsFilling()
		{
			return Weight > 50;
		}

		private void TryFillContainer( Mobile from )
		{
			if ( NeedsFilling() )
			{
				Movable = true;
				int FillMeUpLevel = (int)(this.Weight - 51);
				this.Weight = 5.0;

				if ( GetPlayerInfo.LuckyPlayer( from.Luck ) )
				{
					FillMeUpLevel = FillMeUpLevel + Utility.RandomMinMax( 1, 2 );
				}

				ContainerFunctions.FillTheContainer( FillMeUpLevel, this, from );
			}
		}

		public override void Open( Mobile from )
		{
			TryFillContainer( from );
			from.SendSound( 0x48, GetWorldLocation() );
			base.Open( from );
		}

		public override bool OnDragLift( Mobile from )
		{
			TryFillContainer( from );

			return true;
		}

		public override bool DisplaysContent{ get{ return !NeedsFilling(); } }

		public override void AddNameProperties(ObjectPropertyList list)
		{
			base.AddNameProperties(list);
			
			if ( NeedsFilling() )
				list.Add("Filled with treasure");
		}

		public LootChest( Serial serial ) : base( serial )
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