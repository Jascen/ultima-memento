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
	public class CorpseChest : LockableContainer
	{
		public override CraftResource DefaultResource{ get{ return CraftResource.BrittleSkeletal; } }

		[Constructable]
		public CorpseChest() : this( 1 )
		{
		}

		[Constructable]
		public CorpseChest( int level ) : base( 0xe40 )
		{
			Catalog = Catalogs.Body;

			string sCorpse = ContainerFunctions.GetOwner( "Body" );
			Name = sCorpse;
			Movable = true;
			Weight = 11.0 + (double)level;
			GumpID = 0x2A73;
			DropSound = 0x48;
			ItemID = 3786 + Utility.Random( 8 );

			TrapType = TrapType.None;
			TrapPower = 0;
			TrapLevel = 0;
			Locked = false;
            LockLevel = 0;
			MaxLockLevel = 0;
			RequiredSkill = 0;
		}

		private bool NeedsFilling()
		{
			return Weight > 10;
		}

		private void TryFillContainer( Mobile from )
		{
			if ( NeedsFilling() )
			{
				Movable = true;
				int FillMeUpLevel = (int)(this.Weight - 11);
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

		public CorpseChest( Serial serial ) : base( serial )
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