using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Engines.Craft;
using Server.Mobiles;

namespace Server.Items
{
	public class RustyJunk : Item
	{
		[Constructable]
		public RustyJunk() : base( 0x1B72 )
		{
			Name = "rusty shield";
			ItemID = 0x1B72;
			Weight = 3.0;

			if ( Utility.RandomMinMax( 0, 1 ) == 1 )
			{
				switch ( Utility.RandomMinMax( 0, 20 ) )
				{
					case 0:		Name = "rusty shield";	ItemID = 0x1B72;								Weight = 5.0;		break;
					case 1:		Name = "rusty shield";	ItemID = 0x1B73;								Weight = 4.0;		break;
					case 2:		Name = "rusty shield";	ItemID = Utility.RandomList( 0x1B74, 0x1B75 );	Weight = 5.0;		break;
					case 3:		Name = "rusty shield";	ItemID = Utility.RandomList( 0x1B76, 0x1B77 );	Weight = 8.0;		break;
					case 4:		Name = "rusty shield";	ItemID = 0x1B7B;								Weight = 5.0;		break;
					case 5:		Name = "rusty shield";	ItemID = 0x1BC3;								Weight = 5.0;		break;
					case 6:		Name = "rusty shield";	ItemID = Utility.RandomList( 0x1BC4, 0x1BC5 );	Weight = 7.0;		break;
					case 7:		Name = "rusty shield";	ItemID = 0x1BC5;								Weight = 5.0;		break;
					case 8:		Name = "rusty arms";	ItemID = Utility.RandomList( 0x1410, 0x1417 );	Weight = 5.0;		break;
					case 9:		Name = "rusty leggings"; ItemID = Utility.RandomList( 0x46AA, 0x46AB );	Weight = 7.0;		break;
					case 10:	Name = "rusty helm";	ItemID = 0x1412;								Weight = 5.0;		break;
					case 11:	Name = "rusty gorget";	ItemID = 0x1413;								Weight = 2.0;		break;
					case 12:	Name = "rusty gloves";	ItemID = Utility.RandomList( 0x1414, 0x1418 );	Weight = 2.0;		break;
					case 13:	Name = "rusty armor";	ItemID = Utility.RandomList( 0x1415, 0x1416 );	Weight = 10.0;		break;
					case 14:	Name = "rusty coif";	ItemID = Utility.RandomList( 0x13BB, 0x13C0 );	Weight = 1.0;		break;
					case 15:	Name = "rusty leggings"; ItemID = Utility.RandomList( 0x13BE, 0x13C3 );	Weight = 7.0;		break;
					case 16:	Name = "rusty tunic";	ItemID = Utility.RandomList( 0x13BF, 0x13C4 );	Weight = 7.0;		break;
					case 17:	Name = "rusty gloves";	ItemID = Utility.RandomList( 0x13EB, 0x13F2 );	Weight = 2.0;		break;
					case 18:	Name = "rusty leggings"; ItemID = Utility.RandomList( 0x13F0, 0x13F1 );	Weight = 15.0;		break;
					case 19:	Name = "rusty tunic";	ItemID = Utility.RandomList( 0x13EC, 0x13ED );	Weight = 15.0;		break;
					case 20:	Name = "rusty sleeves";	ItemID = Utility.RandomList( 0x13EE, 0x13EF );	Weight = 15.0;		break;
				}
			}
			else
			{
				switch ( Utility.RandomMinMax( 0, 22 ) )
				{
					case 0:		Name = "rusty hatchet";		ItemID = Utility.RandomList( 0xF43, 0xF44 );	Weight = Utility.RandomMinMax( 4, 8 );		break;
					case 1:		Name = "rusty axe";			ItemID = Utility.RandomList( 0xF45, 0xF46 );	Weight = Utility.RandomMinMax( 4, 8 );		break;
					case 2:		Name = "rusty battle axe";	ItemID = Utility.RandomList( 0xF47, 0xF48 );	Weight = Utility.RandomMinMax( 4, 8 );		break;
					case 3:		Name = "rusty axe";			ItemID = Utility.RandomList( 0xF49, 0xF4A );	Weight = Utility.RandomMinMax( 4, 8 );		break;
					case 4:		Name = "rusty double axe";	ItemID = Utility.RandomList( 0xF4B, 0xF4C );	Weight = Utility.RandomMinMax( 4, 8 );		break;
					case 5:		Name = "rusty bardiche";	ItemID = Utility.RandomList( 0xF4D, 0xF4E );	Weight = Utility.RandomMinMax( 8, 16 );		break;
					case 6:		Name = "rusty dagger";		ItemID = Utility.RandomList( 0xF51, 0xF52 );	Weight = Utility.RandomMinMax( 1, 2 );		break;
					case 7:		Name = "rusty mace";		ItemID = Utility.RandomList( 0xF5C, 0xF5D );	Weight = Utility.RandomMinMax( 4, 8 );		break;
					case 8:		Name = "rusty broadsword";	ItemID = Utility.RandomList( 0xF5E, 0xF5F );	Weight = Utility.RandomMinMax( 4, 8 );		break;
					case 9:		Name = "rusty longsword";	ItemID = Utility.RandomList( 0xF60, 0xF61 );	Weight = Utility.RandomMinMax( 4, 8 );		break;
					case 10:	Name = "rusty spear";		ItemID = Utility.RandomList( 0xF62, 0xF63 );	Weight = Utility.RandomMinMax( 8, 16 );		break;
					case 11:	Name = "rusty war hammer";	ItemID = Utility.RandomList( 0x1438, 0x1439 );	Weight = Utility.RandomMinMax( 4, 8 );		break;
					case 12:	Name = "rusty maul";		ItemID = Utility.RandomList( 0x143A, 0x143B );	Weight = Utility.RandomMinMax( 4, 8 );		break;
					case 13:	Name = "rusty hammer pick";	ItemID = Utility.RandomList( 0x143C, 0x143D );	Weight = Utility.RandomMinMax( 4, 8 );		break;
					case 14:	Name = "rusty halberd";		ItemID = Utility.RandomList( 0x143E, 0x143F );	Weight = Utility.RandomMinMax( 8, 16 );		break;
					case 15:	Name = "rusty cutlass";		ItemID = Utility.RandomList( 0x1440, 0x1441 );	Weight = Utility.RandomMinMax( 4, 8 );		break;
					case 16:	Name = "rusty great axe";	ItemID = Utility.RandomList( 0x1442, 0x1443 );	Weight = Utility.RandomMinMax( 8, 16 );		break;
					case 17:	Name = "rusty war axe";		ItemID = Utility.RandomList( 0x13AF, 0x13B0 );	Weight = Utility.RandomMinMax( 4, 8 );		break;
					case 18:	Name = "rusty scimitar";	ItemID = Utility.RandomList( 0x13B5, 0x13B6 );	Weight = Utility.RandomMinMax( 4, 8 );		break;
					case 19:	Name = "rusty long sword";	ItemID = Utility.RandomList( 0x13B7, 0x13B8 );	Weight = Utility.RandomMinMax( 4, 8 );		break;
					case 20:	Name = "rusty barbarian sword"; ItemID = Utility.RandomList( 0x13B9, 0x13BA );	Weight = Utility.RandomMinMax( 4, 8 );		break;
					case 21:	Name = "rusty scythe";		ItemID = Utility.RandomList( 0x26BA, 0x26C4 );	Weight = Utility.RandomMinMax( 8, 16 );		break;
					case 22:	Name = "rusty pike";		ItemID = Utility.RandomList( 0x26BE, 0x26C8 );	Weight = Utility.RandomMinMax( 8, 16 );		break;
				}
			}

			Hue = Utility.RandomList( 0xB97, 0xB98, 0xB99, 0xB9A, 0xB88 );
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1070722, "Scrap Iron");
        }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) 
			{
				from.SendMessage( "This must be in your backpack to use." );
				return;
			}
			else
			{
				from.SendMessage("Select the forge to smelt this item.");
				from.Target = new InternalTarget( this );
			}
		}

		private class InternalTarget : Target
		{
			private RustyJunk m_Rusted;

			public InternalTarget( RustyJunk ore ) :  base ( 2, false, TargetFlags.None )
			{
				m_Rusted = ore;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Rusted.Deleted )
					return;

				if ( Server.Engines.Craft.DefBlacksmithy.UseForge( targeted ) )
				{
					int weight = (int)(m_Rusted.Weight);
						if ( weight < 1 ){ weight = 1; }
					double difficulty = 50.0;
					double minSkill = difficulty - 25.0;
					double maxSkill = difficulty + 25.0;
					
					if ( difficulty > from.Skills[SkillName.Mining].Value )
					{
						from.SendMessage("You have no idea how to smelt this item!");
						return;
					}

					if ( from.CheckTargetSkill( SkillName.Mining, targeted, minSkill, maxSkill ) )
					{
						IronIngot ingot = new IronIngot(1);
						ingot.Amount = weight;
						from.AddToBackpack( ingot );
						from.PlaySound( 0x208 );
						if ( weight == 1 ){ from.SendMessage("You smelt the rusty metal into a usable iron ingot!"); }
						else { from.SendMessage("You smelt the rusty metal into usable iron ingots!"); }
						m_Rusted.Delete();
					}
					else
					{
						from.PlaySound( 0x208 );
						from.SendMessage("You failed to smelt the rusty metal into anything usable!");
						m_Rusted.Delete();
					}
				}
			}
		}

		public RustyJunk(Serial serial) : base(serial)
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