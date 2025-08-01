using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using System.Collections.Generic;
using Server.Misc;
using System.Collections;
using Server.Regions;

namespace Server.Items
{
	public class HiddenChest : Item
	{
		[Constructable]
		public HiddenChest() : base(0x2163)
		{
			Movable = false;
			Name = "a hidden chest";
			Visible = false;
		}

		public HiddenChest(Serial serial) : base(serial)
		{
		}

		public override bool OnMoveOver( Mobile m )
		{
			int level = (int)(m.Skills[SkillName.Searching].Value / 10);
				if (level < 1){level = 1;}
				if (level > 10){level = 10;}

			if ( m.AccessLevel == AccessLevel.Player && m is PlayerMobile )
				FoundBox( m, false, level, this );

			return true;
		}

		public static bool FoundBox( Mobile m, bool spell, int level, Item item )
		{
			if ( m is PlayerMobile && m.Alive && !m.Blessed )
			{
				bool foundIt = spell;

				if ( !foundIt )
				{
					if ( m.CheckSkill( SkillName.Searching, 0, 125 ) )
						foundIt = true;
					else if ( Server.SkillHandlers.Searching.SpotInTheDark( m ) )
						foundIt = true;
				}

				if ( foundIt )
				{
					m.LocalOverheadMessage(Network.MessageType.Emote, 0x3B2, false, "Your eye catches something nearby.");
					Map map = m.Map;
					string where = Server.Misc.Worlds.GetRegionName( m.Map, m.Location );

					int money = Utility.RandomMinMax( 100, 200 );

					switch( Utility.RandomMinMax( 1, level ) )
					{
						case 1: level = 1; break;
						case 2: level = 2; break;
						case 3: level = 3; break;
						case 4: level = 4; break;
						case 5: level = 5; break;
						case 6: level = 6; break;
						case 7: level = 7; break;
						case 8: level = 8; break;
						case 9: level = 9; break;
						case 10: level = 10; break;
					}

					HiddenTrapDoor mDoor = new HiddenTrapDoor( level );
					mDoor.MoveToWorld( item.Location, item.Map );
					Effects.SendLocationParticles( EffectItem.Create( mDoor.Location, mDoor.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 5024 );
					Effects.PlaySound( mDoor.Location, mDoor.Map, 0x1FA );

					if ( GetPlayerInfo.CheckLuck( m.Luck, 10, 20 ) )
					{
						HiddenBox mBox = new HiddenBox( level, where, m );
						mDoor.DropItem( mBox );
					}
					else
					{
						Item coins = new Gold( ( money * level ) );

						if ( Server.Misc.Worlds.IsOnSpaceship( item.Location, item.Map ) ){
							coins.Delete(); coins = new DDXormite(); coins.Amount = (int)( ( money * level ) / 3 ); }
						else if ( item.Land == Land.Underworld ){
							coins.Delete(); coins = new DDJewels(); coins.Amount = (int)( ( money * level ) / 2 ); }
						else if ( Utility.RandomMinMax( 1, 100 ) > 99 ){
							coins.Delete(); coins = new DDGemstones(); coins.Amount = (int)( ( money * level ) / 2 ); }
						else if ( Utility.RandomMinMax( 1, 100 ) > 95 ){
							coins.Delete(); coins = new DDGoldNuggets(); coins.Amount = (int)( ( money * level ) ); }
						else if ( Utility.RandomMinMax( 1, 100 ) > 80 ){
							coins.Delete(); coins = new DDSilver(); coins.Amount = (int)( ( money * level ) * 5 ); }
						else if ( Utility.RandomMinMax( 1, 100 ) > 60 ){
							coins.Delete(); coins = new DDCopper(); coins.Amount = (int)( ( money * level ) * 10 ); }

						if ( coins.Amount > 65000 )
							coins.Amount = 65000;

						mDoor.DropItem( coins );
					}

					ContainerFunctions.FillTheContainer( level, mDoor, m );
					item.Delete();

					return true;
				}
			}
			return false;
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

		public override bool HandlesOnMovement{ get{ return MySettings.S_EnableDungeonSoundEffects; } }

		private DateTime m_NextSound;	
		public DateTime NextSound{ get{ return m_NextSound; } set{ m_NextSound = value; } }

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if( m is PlayerMobile && MySettings.S_EnableDungeonSoundEffects )
			{
				if ( DateTime.Now >= m_NextSound && Utility.InRange( m.Location, this.Location, 10 ) )
				{
					if ( Utility.RandomBool() )
					{
						int sound = HiddenChest.DungeonSounds( this );	
						m.PlaySound( sound );	
					}
					m_NextSound = (DateTime.Now + TimeSpan.FromSeconds( 60 ));	
				}
			}
		}

		public static int DungeonSounds( Item item )
		{
			Region reg = Region.Find( item.Location, item.Map );	

			string sound = "dungeon";	

			if ( reg.IsPartOf( "the Ancient Sky Ship" ) ){ sound = "scifi"; }
			else if ( reg.IsPartOf( "the Blood Temple" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "Dungeon Covetous" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "Dungeon Despise" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "Dungeon Destard" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "Dungeon Hate" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "Dungeon Wicked" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "Dungeon Wrath" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "Frostwall Caverns" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "Stonegate Castle" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "the Cave of Banished Mages" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "the Cave of Souls" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "the Cave of the Zuluu" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "the Dragon's Maw" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "the Frozen Dungeon" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "the Frozen Hells" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "the Glacial Scar" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "the Hall of the Mountain King" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "the Mines of Morinia" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "the Temple of Osirus" ) ){ sound = "cave"; }
			else if ( reg.IsPartOf( "Dardin's Pit" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "Dungeon Ankh" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "Dungeon Bane" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "Dungeon Clues" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "Dungeon Exodus" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "Dungeon Hythloth" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "Dungeon Torment" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "Dungeon Vile" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "Dungeon Wrong" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "Harkyn's Castle" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "Kylearan's Tower" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "the Ancient Prison" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "the Azure Castle" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "the Castle of the Mad Archmage" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "the Cellar" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "the Dungeon of the Mad Archmage" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "the Forgotten Halls" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "the Halls of Ogrimar" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "the Halls of Undermountain" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "the Ice Queen Fortress" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "the Perinian Depths" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "the Stygian Abyss" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "the Tower of Brass" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "the Vault of the Black Knight" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "Vordo's Castle" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "Vordo's Dungeon" ) ){ sound = "dungeon"; }
			else if ( reg.IsPartOf( "Castle Exodus" ) ){ sound = "fire"; }
			else if ( reg.IsPartOf( "Morgaelin's Inferno" ) ){ sound = "fire"; }
			else if ( reg.IsPartOf( "the Cave of Fire" ) ){ sound = "fire"; }
			else if ( reg.IsPartOf( "the City of Embers" ) ){ sound = "fire"; }
			else if ( reg.IsPartOf( "the Fires of Hell" ) ){ sound = "fire"; }
			else if ( reg.IsPartOf( "the Volcanic Cave" ) ){ sound = "fire"; }
			else if ( reg.IsPartOf( "the Corrupt Pass" ) ){ sound = "forest"; }
			else if ( reg.IsPartOf( "Dungeon Rock" ) ){ sound = "gargoyle"; }
			else if ( reg.IsPartOf( "Dungeon Deceit" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Ancient Pyramid" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Castle of Dracula" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Catacombs" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Catacombs of Azerok" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Crypt" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Crypts of Dracula" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Crypts of Kuldar" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Dungeon of the Lich King" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Gargoyle Crypts" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Great Pyramid" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Isle of the Lich" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Lodoria Catacombs" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Lower Catacombs" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Mausoleum" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Dark Tombs" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Tomb of Kazibal" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Tomb of the Fallen Wizard" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Tombs" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Zealan Tombs" ) ){ sound = "haunted"; }
			else if ( reg.IsPartOf( "the Altar of the Dragon King" ) ){ sound = "lizard"; }
			else if ( reg.IsPartOf( "the Lizardman Cave" ) ){ sound = "lizard"; }
			else if ( reg.IsPartOf( "the Sanctum of Saltmarsh" ) ){ sound = "lizard"; }
			else if ( reg.IsPartOf( "Dungeon Shame" ) ){ sound = "magic"; }
			else if ( reg.IsPartOf( "Mangar's Tower" ) ){ sound = "magic"; }
			else if ( reg.IsPartOf( "the Dungeon of Time Awaits" ) ){ sound = "magic"; }
			else if ( reg.IsPartOf( "the Ice Fiend Lair" ) ){ sound = "magic"; }
			else if ( reg.IsPartOf( "the Mage Mansion" ) ){ sound = "magic"; }
			else if ( reg.IsPartOf( "the Mind Flayer City" ) ){ sound = "magic"; }
			else if ( reg.IsPartOf( "Argentrock Castle" ) ){ sound = "wind"; }
			else if ( reg.IsPartOf( "the Ratmen Mines" ) ){ sound = "rats"; }
			else if ( reg.IsPartOf( "the Kuldara Sewers" ) ){ sound = "sewer"; }
			else if ( reg.IsPartOf( "the Montor Sewers" ) ){ sound = "sewer"; }
			else if ( reg.IsPartOf( "the Sewers" ) ){ sound = "sewer"; }
			else if ( reg.IsPartOf( "Dungeon Scorn" ) ){ sound = "snakes"; }
			else if ( reg.IsPartOf( "the Serpent Sanctum" ) ){ sound = "snakes"; }
			else if ( reg.IsPartOf( "Terathan Keep" ) ){ sound = "spiders"; }
			else if ( reg.IsPartOf( "the Island of the Storm Giant" ) ){ sound = "thunder"; }
			else if ( reg.IsPartOf( "the Storm Giant Lair" ) ){ sound = "thunder"; }
			else if ( reg.IsPartOf( "the Caverns of Poseidon" ) ){ sound = "water"; }
			else if ( reg.IsPartOf( "the Flooded Temple" ) ){ sound = "water"; }
			else if ( reg.IsPartOf( "the Scurvy Reef" ) ){ sound = "water"; }
			else if ( reg.IsPartOf( "the Undersea Castle" ) ){ sound = "water"; }

			int value = 1;	

			if ( sound == "scifi" ){ 			value = Utility.RandomList( 0x55E, 0x549, 0x54A, 0x2F5, 0x457 ); }
			else if ( sound == "cave" ){ 		value = Utility.RandomList( 0x668, 0x669, 0x64D, 0x568, 0x567, 0x566, 0x4D0, 0x4CF, 0x382, 0x2DA, 0x290, 0x222, 0x223, 0x221, 0x220, 0x0CD, 0x102, 0x103 ); }
			else if ( sound == "dungeon" ){ 	value = Utility.RandomList( 0x476, 0x3E7, 0x391, 0x22C, 0x11D, 0x101, 0x0F5, 0x0F0, 0x0EC, 0x0EE, 0x02B, 0x02C, 0x041, 0x03F, 0x050, 0x057, 0x0CD ); }
			else if ( sound == "fire" ){ 		value = Utility.RandomList( 0x5D0, 0x5CB, 0x4BB, 0x44C, 0x359, 0x346, 0x227, 0x1DE, 0x055, 0x11D, 0x11E ); }
			else if ( sound == "forest" ){ 		value = Utility.RandomList( 0x64D, 0x5CE, 0x009, 0x00A ); }
			else if ( sound == "gargoyle" ){ 	value = Utility.RandomList( 0x04C, 0x0EE, 0x669, 0x669, 0x100, 0x176 ); }
			else if ( sound == "haunted" ){ 	value = Utility.RandomList( 0x485, 0x483, 0x3EB, 0x380, 0x37E, 0x1D9, 0x19E, 0x182, 0x180, 0x17F, 0x121, 0x105, 0x0FE, 0x0FB, 0x0FA, 0x0F9, 0x0F5, 0x0EF, 0x0EE, 0x057, 0x0CD ); }
			else if ( sound == "lizard" ){ 		value = Utility.RandomList( 0x3C2, 0x05C, 0x05F, 0x1A2, 0x1A3 ); }
			else if ( sound == "magic" ){ 		value = Utility.RandomList( 0x380, 0x37E, 0x0F6, 0x0F7, 0x0F8 ); }
			else if ( sound == "wind" ){ 		value = Utility.RandomList( 0x655, 0x5C9, 0x566, 0x291, 0x0FC, 0x015, 0x016, 0x017, 0x04C ); }
			else if ( sound == "rats" ){ 		value = Utility.RandomList( 0x0CD, 0x0CE, 0x18A, 0x1B7 ); }
			else if ( sound == "sewer" ){ 		value = Utility.RandomList( 0x5DA, 0x5B0, 0x5AC, 0x5A5, 0x3C2, 0x387, 0x2DA, 0x240, 0x230, 0x121, 0x0CD, 0x0CE, 0x011, 0x012, 0x1C9, 0x1CA ); }
			else if ( sound == "snakes" ){ 		value = Utility.RandomList( 0x5AF, 0x3C2, 0x286, 0x281, 0x27C, 0x0DD ); }
			else if ( sound == "spiders" ){ 	value = Utility.RandomList( 0x259, 0x24F, 0x184, 0x185 ); }
			else if ( sound == "thunder" ){ 	value = Utility.RandomList( 0x5CF, 0x56A, 0x029, 0x02A, 0x104 ); }
			else if ( sound == "water" ){ 		value = Utility.RandomList( 0x5B0, 0x4D2, 0x365, 0x2D9, 0x240, 0x013, 0x021, 0x023, 0x025, 0x025, 0x027, 0x028 ); }

			return (value-1);	
		}
	}
}