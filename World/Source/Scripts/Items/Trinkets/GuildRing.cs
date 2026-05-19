using System;
using Server;
using Server.Misc;
using System.Collections.Generic;
using System.Collections;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
	public class GuildRing : JewelryRing
	{
		public override Catalogs DefaultCatalog{ get{ return Catalogs.Jewelry; } }

		public Mobile RingOwner;
		public string RingGuild;
		public string RingName;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Ring_Owner { get{ return RingOwner; } set{ RingOwner = value; } }

		[CommandProperty(AccessLevel.Owner)]
		public string Ring_Guild { get { return RingGuild; } set { RingGuild = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string Ring_Name { get { return RingName; } set { RingName = value; InvalidateProperties(); } }

		[Constructable]
		public GuildRing( Mobile from, int guild )
		{
			this.RingOwner = from;
			Name = "guild ring";

			switch( guild )
			{
				case 1: RingGuild = "MagesGuild"; RingName = "Member of the Wizards Guild"; 
					SkillBonuses.SetValues(0, SkillName.Psychology, 10); 
					SkillBonuses.SetValues(1, SkillName.Magery, 10); 
					SkillBonuses.SetValues(2, SkillName.Meditation, 10); break;
				case 2: RingGuild = "WarriorsGuild"; RingName = "Member of the Warriors Guild"; 
					SkillBonuses.SetValues(0, SkillName.Fencing, 10); 
					SkillBonuses.SetValues(1, SkillName.Bludgeoning, 10); 
					SkillBonuses.SetValues(2, SkillName.Parry, 10); 
					SkillBonuses.SetValues(3, SkillName.Swords, 10); 
					SkillBonuses.SetValues(4, SkillName.Tactics, 10); break;
				case 3: RingGuild = "ThievesGuild"; RingName = "Member of the Thieves Guild"; 
					SkillBonuses.SetValues(0, SkillName.Hiding, 10); 
					SkillBonuses.SetValues(1, SkillName.Lockpicking, 10); 
					SkillBonuses.SetValues(2, SkillName.Snooping, 10); 
					SkillBonuses.SetValues(3, SkillName.Stealing, 10); 
					SkillBonuses.SetValues(4, SkillName.Stealth, 10); break;
				case 4: RingGuild = "RangersGuild"; RingName = "Member of the Rangers Guild"; 
					SkillBonuses.SetValues(0, SkillName.Camping, 25);
					SkillBonuses.SetValues(1, SkillName.Tracking, 25); break;
				case 5: RingGuild = "HealersGuild"; RingName = "Member of the Healers Guild"; 
					SkillBonuses.SetValues(0, SkillName.Anatomy, 15); 
					SkillBonuses.SetValues(1, SkillName.Healing, 15); 
					SkillBonuses.SetValues(2, SkillName.Veterinary, 15); break;
				case 6: RingGuild = "MinersGuild"; RingName = "Member of the Miners Guild"; SkillBonuses.SetValues(0, SkillName.Mining, 30); break;
				case 7: RingGuild = "MerchantsGuild"; RingName = "Member of the Merchants Guild"; 
					SkillBonuses.SetValues(0, SkillName.Mercantile, 15);
					SkillBonuses.SetValues(1, SkillName.ArmsLore, 15); 
					SkillBonuses.SetValues(2, SkillName.Tasting, 15); break;
				case 8: RingGuild = "TinkersGuild"; RingName = "Member of the Tinkers Guild"; SkillBonuses.SetValues(0, SkillName.Tinkering, 30); break;
				case 9: RingGuild = "TailorsGuild"; RingName = "Member of the Tailors Guild"; SkillBonuses.SetValues(0, SkillName.Tailoring, 30); break;
				case 10: RingGuild = "FishermensGuild"; RingName = "Member of the Mariners Guild"; SkillBonuses.SetValues(0, SkillName.Seafaring, 30); break;
				case 11: RingGuild = "BardsGuild"; RingName = "Member of the Bards Guild"; 
					SkillBonuses.SetValues(0, SkillName.Discordance, 10); 
					SkillBonuses.SetValues(1, SkillName.Musicianship, 10); 
					SkillBonuses.SetValues(2, SkillName.Peacemaking, 10); 
					SkillBonuses.SetValues(3, SkillName.Provocation, 10); break;
				case 12: RingGuild = "BlacksmithsGuild"; RingName = "Member of the Blacksmiths Guild"; 
					SkillBonuses.SetValues(0, SkillName.Blacksmith, 20); 
					SkillBonuses.SetValues(1, SkillName.ArmsLore, 20); break;
				case 13: RingGuild = "NecromancersGuild"; RingName = "Member of the Black Magic Guild";
					SkillBonuses.SetValues(0, SkillName.Forensics, 10); 
					SkillBonuses.SetValues(1, SkillName.Necromancy, 10); 
					SkillBonuses.SetValues(2, SkillName.Spiritualism, 10); break;
				case 14: RingGuild = "AlchemistsGuild"; RingName = "Member of the Alchemists Guild";
					SkillBonuses.SetValues(0, SkillName.Alchemy, 15); 
					SkillBonuses.SetValues(1, SkillName.Cooking, 15); 
					SkillBonuses.SetValues(2, SkillName.Tasting, 15); break;
				case 15: RingGuild = "DruidsGuild"; RingName = "Member of the Druids Guild";
					SkillBonuses.SetValues(0, SkillName.Alchemy, 5); 
					SkillBonuses.SetValues(0, SkillName.Druidism, 10); 
					SkillBonuses.SetValues(1, SkillName.Taming, 10); 
					SkillBonuses.SetValues(2, SkillName.Herding, 10); 
					SkillBonuses.SetValues(3, SkillName.Veterinary, 10); 
					SkillBonuses.SetValues(4, SkillName.Cooking, 5); break;
				case 16: RingGuild = "ArchersGuild"; RingName = "Member of the Archers Guild";
					SkillBonuses.SetValues(0, SkillName.Marksmanship, 10); 
					SkillBonuses.SetValues(1, SkillName.Bowcraft, 20); 
					SkillBonuses.SetValues(2, SkillName.Tactics, 10); break;
				case 17: RingGuild = "CarpentersGuild"; RingName = "Member of the Carpenters Guild";
					SkillBonuses.SetValues(0, SkillName.Carpentry, 20); 
					SkillBonuses.SetValues(1, SkillName.Lumberjacking, 20); break;
				case 18: RingGuild = "CartographersGuild"; RingName = "Member of the Cartographers Guild";
					SkillBonuses.SetValues(0, SkillName.Cartography, 30); break;
				case 19: RingGuild = "LibrariansGuild"; RingName = "Member of the Librarians Guild";
					SkillBonuses.SetValues(0, SkillName.Mercantile, 20); 
					SkillBonuses.SetValues(1, SkillName.Inscribe, 20); break;
				case 20: RingGuild = "CulinariansGuild"; RingName = "Member of the Culinary Guild";
					SkillBonuses.SetValues(0, SkillName.Cooking, 20); 
					SkillBonuses.SetValues(1, SkillName.Tasting, 20); break;
				case 21: RingGuild = "AssassinsGuild"; RingName = "Member of the Assassins Guild";
					SkillBonuses.SetValues(0, SkillName.Fencing, 10); 
					SkillBonuses.SetValues(1, SkillName.Hiding, 10); 
					SkillBonuses.SetValues(2, SkillName.Poisoning, 15); 
					SkillBonuses.SetValues(3, SkillName.Stealth, 10); break;
				case 22: RingGuild = "ElementalGuild"; RingName = "Member of the Elemental Guild"; 
					SkillBonuses.SetValues(0, SkillName.Elementalism, 10); 
					SkillBonuses.SetValues(1, SkillName.Focus, 10); 
					SkillBonuses.SetValues(2, SkillName.Meditation, 10); break;
			}
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1070722, RingName);
            if ( RingOwner != null ){ list.Add( 1049644, RingOwner.Name ); }
        }

		public override bool CanEquip( Mobile from )
		{
			if ( RingOwner != from )
			{
				from.SendMessage( "This is not your guild ring!" );
				return false;
			}

			return base.CanEquip( from );
		}

		public GuildRing( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
			writer.Write( (Mobile)RingOwner );
            writer.Write( RingGuild );
            writer.Write( RingName );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			RingOwner = reader.ReadMobile();
			RingGuild = reader.ReadString();
			RingName = reader.ReadString();
			if ( ItemID == 0x4CFA )
				ItemID = GraphicID = 0x6731;
		}
	}
}