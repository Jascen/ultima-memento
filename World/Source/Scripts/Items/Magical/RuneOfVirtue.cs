using System;
using Server;
using System.Collections;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Misc;

namespace Server.Items
{
	public class RuneOfVirtue : LevelTalismanHoly
	{
		public Mobile ItemOwner;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Item_Owner { get{ return ItemOwner; } set{ ItemOwner = value; } }

		public int ItemSide;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Item_Side { get{ return ItemSide; } set{ ItemSide = value; } }

		[Constructable]
		public RuneOfVirtue()
		{
			Name = "rune";
			ItemID = 0x53C4;
			Resource = CraftResource.None;
			Layer = Layer.Trinket;
			Weight = 1.0;
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			if ( ItemOwner != null ){ list.Add( 1070722, "Rune for " + ItemOwner.Name + "" ); } else { list.Add( 1070722, "Rune"); }
        }

		public override bool CanEquip( Mobile from )
		{
			if ( ItemOwner != from )
			{
				from.LocalOverheadMessage( MessageType.Emote, 0x916, true, "This rune belongs to another!" );
				return false;
			}
			else if ( ItemSide > 0 && from.Karma > 0 )
			{
				from.LocalOverheadMessage( MessageType.Emote, 0x916, true, "Your morality is too virtuous to wield that!" );
				return false;
			}
			else if ( ItemSide < 1 && from.Karma < 0 )
			{
				from.LocalOverheadMessage( MessageType.Emote, 0x916, true, "Your morality is too corrupt to wield that!" );
				return false;
			}

			return base.CanEquip( from );
		}

		public override bool OnEquip( Mobile from )
		{
			Hue = 0;
			if ( ItemSide > 0 )
				Hue = 0xB20;

			return base.OnEquip( from );
		}

		public static void MoralityCheck( Item rune, Mobile from )
		{
			if ( rune != null && from is PlayerMobile )
			{
				if ( rune is RuneOfVirtue )
				{
					RuneOfVirtue stone = (RuneOfVirtue)rune;

					if ( stone.ItemSide > 0 && from.Karma > 0 )
					{
						from.LocalOverheadMessage( MessageType.Emote, 0x916, true, "Your morality is too virtuous to wield that rune!" );
						from.AddToBackpack ( rune );
						from.InvalidateProperties();
					}
					else if ( stone.ItemSide < 1 && from.Karma < 0 )
					{
						from.LocalOverheadMessage( MessageType.Emote, 0x916, true, "Your morality is too corrupt to wield that rune!" );
						from.AddToBackpack ( rune );
						from.InvalidateProperties();
					}
				}
			}
		}

		public static void RuneLook( RuneOfVirtue rune )
		{
			rune.Hue = 0;
			if ( rune.ItemID == 0x53C4 ){ rune.Name = "Rune of Honesty";			rune.ItemID = 0x5304;	if ( rune.ItemSide > 0 ){ rune.Name = "Rune of Deceit"; } }
			else if ( rune.ItemID == 0x5304 ){ rune.Name = "Rune of Honor";			rune.ItemID = 0x5305;	if ( rune.ItemSide > 0 ){ rune.Name = "Rune of Scorn"; } }
			else if ( rune.ItemID == 0x5305 ){ rune.Name = "Rune of Humility";		rune.ItemID = 0x5307;	if ( rune.ItemSide > 0 ){ rune.Name = "Rune of Arrogance"; } }
			else if ( rune.ItemID == 0x5307 ){ rune.Name = "Rune of Justice";		rune.ItemID = 0x5306;	if ( rune.ItemSide > 0 ){ rune.Name = "Rune of Oppression"; } }
			else if ( rune.ItemID == 0x5306 ){ rune.Name = "Rune of Sacrifice";		rune.ItemID = 0x53C2;	if ( rune.ItemSide > 0 ){ rune.Name = "Rune of Neglect"; } }
			else if ( rune.ItemID == 0x53C2 ){ rune.Name = "Rune of Spirituality";	rune.ItemID = 0x5302;	if ( rune.ItemSide > 0 ){ rune.Name = "Rune of Sacrilege"; } }
			else if ( rune.ItemID == 0x53C3 ){ rune.Name = "Rune of Valor";			rune.ItemID = 0x53C3;	if ( rune.ItemSide > 0 ){ rune.Name = "Rune of Fear"; } }
			else { rune.Name = "Rune of Compassion";								rune.ItemID = 0x53C4;	if ( rune.ItemSide > 0 ){ rune.Name = "Rune of Cruelty"; } }

			if ( rune.ItemSide > 0 ){ rune.Hue = 0xB20; }
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1060640 ); // The item must be in your backpack to use it.
			}
			else
			{
				RuneLook( this );
			}
			return;
		}

		public RuneOfVirtue( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
			writer.Write( (Mobile)ItemOwner );
			writer.Write( ItemSide );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			ItemOwner = reader.ReadMobile();
			ItemSide = reader.ReadInt();
		}
	}
}