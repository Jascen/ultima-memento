using System.Collections;
using Server.Misc;
using Server.Network;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Regions;
using Server.Targeting;
using Server.Utilities;

namespace Server.Items
{
	public class DemonPrison : Item
	{
		[Constructable]
		public DemonPrison() : base( 0x1444 )
		{
			Weight = 4.0;
			Name = "Daemon Shard";
			Light = LightType.Circle225;

			if ( Weight > 3.0 )
			{
				Weight = 3.0;

				HaveShardA = 0;
				HaveShardB = 0;
				HaveShardC = 0;
				HaveShardD = 0;
				HaveGold = 0;

				WizardLocation = Server.Items.DemonPrison.GetRandomMage();

				PieceRumor = Server.Items.CubeOnCorpse.GetRumor();
				PieceLocation = Server.Items.CubeOnCorpse.PickDungeon();

				CreateDaemonic( this );
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( Weight > 2.0 && from.Map == Map.SavagedEmpire && from.X >= 726 && from.Y >= 2576 && from.X <= 735 && from.Y <= 2585 )
			{
				Weight = 1.0;
			}

			if ( Weight < 1.5 )
			{
				if ( ( ( HaveShardB + HaveShardC + HaveShardD + HaveShardA ) > 3 ) && HaveGold < NeedGold )
				{
					int need = NeedGold - HaveGold;
					from.SendMessage( "You still need " + need + " gold." );
					from.SendMessage( "Choose some gold coins to add to the shard." );
					from.Target = new GoldTarget( this );
				}
				else
				{
					from.CloseGump( typeof( DemonPrisonGump ) );
					from.SendGump( new DemonPrisonGump( from, this ) );
				}
			}
		}

		private class GoldTarget : Target
		{
			private DemonPrison m_DemonPrison;

			public GoldTarget( DemonPrison shrd ) : base( 1, false, TargetFlags.None )
			{
				m_DemonPrison = shrd;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Gold )
				{
					Item gold = targeted as Item;

					if ( !gold.IsChildOf( from.Backpack ) )
					{
						from.SendMessage( "You can only add gold coins from your pack." );
					}
					else
					{
						int cost = m_DemonPrison.NeedGold - m_DemonPrison.HaveGold;
						var iAmount = ItemUtilities.ConsumeClamped(gold, cost);
						m_DemonPrison.HaveGold += iAmount;

						from.PlaySound( 0x1FA );
						from.SendMessage( "The gold has been added to the shard." );
					}
				}
				else
				{
					from.SendMessage( "You cannot add that to the shard." );
				}
			}
		}

		public static string GetRandomMage()
		{
			int aCount = 0;
			Region reg = null;
			string sRegion = "";

			ArrayList targets = new ArrayList();
			foreach ( Mobile target in World.Mobiles.Values )
			if ( target is BaseVendor )
			{
				reg = Region.Find( target.Location, target.Map );

				if (	target.Land == Land.Sosaria || 
						target.Land == Land.Lodoria || 
						target.Land == Land.Serpent || 
						target.Land == Land.IslesDread || 
						target.Land == Land.Savaged || 
						target.Land == Land.UmberVeil || 
						target.Land == Land.Kuldar )
				{
					if ( ( target is NecromancerGuildmaster || target is MageGuildmaster || target is Mage || target is Necromancer || target is Witches ) && reg.IsPartOf( typeof( VillageRegion ) ))
					{
						targets.Add( target ); aCount++;
					}
				}
			}

			aCount = Utility.RandomMinMax( 1, aCount );

			int xCount = 0;
			for ( int i = 0; i < targets.Count; ++i )
			{
				Mobile vet = ( Mobile )targets[ i ];
				xCount++;

				if ( xCount == aCount )
				{
					sRegion = Server.Misc.Worlds.GetRegionName( vet.Map, vet.Location );
				}
			}

			return sRegion;
		}

		public DemonPrison( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 ); // version

			writer.Write( HaveShardA );
			writer.Write( HaveShardB );
			writer.Write( HaveShardC );
			writer.Write( HaveShardD );
			writer.Write( HaveGold );
			writer.Write( NeedGold );
			writer.Write( WizardLocation );
			writer.Write( PieceLocation );
			writer.Write( PieceRumor );
			writer.Write( DaemonType );
			writer.Write( DaemonBody );
			writer.Write( DaemonName );
			writer.Write( DaemonTitle );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			HaveShardA = reader.ReadInt();
			HaveShardB = reader.ReadInt();
			HaveShardC = reader.ReadInt();
			HaveShardD = reader.ReadInt();
			HaveGold = reader.ReadInt();
			NeedGold = reader.ReadInt();
			WizardLocation = reader.ReadString();
			PieceLocation = reader.ReadString();
			PieceRumor = reader.ReadString();
			DaemonType = reader.ReadInt();
			DaemonBody = reader.ReadInt();
			DaemonName = reader.ReadString();
			DaemonTitle = reader.ReadString();
		}

		public static bool ProcessDemonPrison( Mobile m, Mobile wizard, Item dropped )
		{
			DemonPrison shard = (DemonPrison)dropped;

			if ( Server.Misc.Worlds.GetRegionName( wizard.Map, wizard.Location ) != shard.WizardLocation ){ return false; }

			int wizardSkill = (int)(m.Skills[SkillName.Magery].Value);
				if ( m.Skills[SkillName.Necromancy].Value > m.Skills[SkillName.Magery].Value ){ wizardSkill = (int)(m.Skills[SkillName.Necromancy].Value); }
				if ( m.Skills[SkillName.Elementalism].Value > m.Skills[SkillName.Magery].Value ){ wizardSkill = (int)(m.Skills[SkillName.Elementalism].Value); }
				if ( m.Skills[SkillName.Elementalism].Value > m.Skills[SkillName.Necromancy].Value ){ wizardSkill = (int)(m.Skills[SkillName.Elementalism].Value); }
				if ( wizardSkill > 100 ){ wizardSkill = 100; }

			int GoldReturn = 0;
				if ( wizardSkill > 0 ){ GoldReturn = (int)( shard.NeedGold * ( wizardSkill * 0.005 ) ); }

			int HaveIngredients = 0;

			if ( shard.HaveShardB >= 0 ){ HaveIngredients++; }
			if ( shard.HaveShardC >= 0 ){ HaveIngredients++; }
			if ( shard.HaveShardD >= 0 ){ HaveIngredients++; }
			if ( shard.HaveGold >= shard.NeedGold ){ HaveIngredients++; }
			if ( shard.HaveShardA >= 0 ){ HaveIngredients++; }

			if ( HaveIngredients < 5 ){ return false; }

			if ( (m.Followers + 3) > m.FollowersMax )
			{
				wizard.Say( "You have too many followers with you to shatter this shard." );
				return false;
			}

			if ( GoldReturn > 0 ){ m.AddToBackpack( new Gold( GoldReturn ) ); wizard.Say( "Here is " + GoldReturn.ToString() + " gold back for all of your help." ); }

			BaseCreature daemon = new Daemonic( shard.DaemonBody, shard.DaemonType );
			daemon.OnAfterSpawn();
			daemon.Controlled = true;
			daemon.ControlMaster = m;
			daemon.IsBonded = true;
			daemon.Name = shard.DaemonName;
			daemon.Title = shard.DaemonTitle;
			daemon.MoveToWorld( m.Location, m.Map );
			daemon.ControlTarget = m;
			daemon.Tamable = true;
			daemon.MinTameSkill = 29.1;
			daemon.ControlOrder = OrderType.Follow;

			LoggingFunctions.LogGenericQuest( m, "has freed a daemon" );
			m.PrivateOverheadMessage(MessageType.Regular, 1153, false, "Your daemon has been freed from the shard.", m.NetState);

			m.PlaySound( 0x3D );

			dropped.Delete();

			return true;
		}

		public static void CreateDaemonic( DemonPrison shard )
		{
			int daemon = Utility.RandomMinMax( 1, 146 );

			shard.DaemonType = daemon;
			shard.DaemonBody = 9;
			shard.NeedGold = 100000;
			shard.DaemonName = NameList.RandomName( "daemon" );

			switch ( daemon )
			{
				case 1: shard.Hue = 0x8E4; 	shard.DaemonTitle = "the bloodstone daemon"; 	break;
				case 2: shard.Hue = 0xB2A; 	shard.DaemonTitle = "the mercury daemon"; 	break;
				case 3: shard.Hue = 0x916; 	shard.DaemonTitle = "the scarlet daemon"; 	break;
				case 4: shard.Hue = 0xB51; 	shard.DaemonTitle = "the poison daemon"; 	break;
				case 5: shard.Hue = 0x82B; 	shard.DaemonTitle = "the glare daemon"; 	break;
				case 6: shard.Hue = 0x8D8; 	shard.DaemonTitle = "the glaze daemon"; 	break;
				case 7: shard.Hue = 0x921; 	shard.DaemonTitle = "the radiant daemon"; 	break;
				case 8: shard.Hue = 0x77C; 	shard.DaemonTitle = "the blood daemon"; 	break;
				case 9: shard.Hue = 0x871; 	shard.DaemonTitle = "the rust daemon"; 	break;
				case 10: shard.Hue = 0x996; 	shard.DaemonTitle = "the sapphire daemon"; 	break;
				case 11: shard.Hue = 0xB56; 	shard.DaemonTitle = "the azurite daemon"; 	break;
				case 12: shard.Hue = 0x95B; 	shard.DaemonTitle = "the brass daemon"; 	break;
				case 13: shard.Hue = 0x796; 	shard.DaemonTitle = "the cobolt daemon"; 	break;
				case 14: shard.Hue = 0xB65; 	shard.DaemonTitle = "the mithril daemon"; 	break;
				case 15: shard.Hue = 0xB05; 	shard.DaemonTitle = "the palladium daemon"; 	break;
				case 16: shard.Hue = 0xB3B; 	shard.DaemonTitle = "the pearl daemon"; 	break;
				case 17: shard.Hue = 0x99F; 	shard.DaemonTitle = "the steel daemon"; 	break;
				case 18: shard.Hue = 0x98B; 	shard.DaemonTitle = "the titanium daemon"; 	break;
				case 19: shard.Hue = 0xB7C; 	shard.DaemonTitle = "the turquoise daemon"; 	break;
				case 20: shard.Hue = 0x6F7; 	shard.DaemonTitle = "the violet daemon"; 	break;
				case 21: shard.Hue = 0x7C3; 	shard.DaemonTitle = "the amethyst daemon"; 	break;
				case 22: shard.Hue = 0x7C6; 	shard.DaemonTitle = "the bright daemon"; 	break;
				case 23: shard.Hue = 0x92B; 	shard.DaemonTitle = "the bronze daemon"; 	break;
				case 24: shard.Hue = 0x943; 	shard.DaemonTitle = "the cadmium daemon"; 	break;
				case 25: shard.Hue = 0x8D0; 	shard.DaemonTitle = "the cerulean daemon"; 	break;
				case 26: shard.Hue = 0x8B6; 	shard.DaemonTitle = "the darkhorned daemon"; 	break;
				case 27: shard.Hue = 0xB7E; 	shard.DaemonTitle = "the diamond daemon"; 	break;
				case 28: shard.Hue = 0xB1B; 	shard.DaemonTitle = "the gilded daemon"; 	break;
				case 29: shard.Hue = 0x829; 	shard.DaemonTitle = "the grey daemon"; 	break;
				case 30: shard.Hue = 0xB94; 	shard.DaemonTitle = "the jade daemon"; 	break;
				case 31: shard.Hue = 0x77E; 	shard.DaemonTitle = "the jadefire daemon"; 	break;
				case 32: shard.Hue = 0x88B; 	shard.DaemonTitle = "the murky daemon"; 	break;
				case 33: shard.Hue = 0x994; 	shard.DaemonTitle = "the platinum daemon"; 	break;
				case 34: shard.Hue = 0x6F5; 	shard.DaemonTitle = "the purple daemon"; 	break;
				case 35: shard.Hue = 0x869; 	shard.DaemonTitle = "the quartz daemon"; 	break;
				case 36: shard.Hue = 0xB02; 	shard.DaemonTitle = "the sanguine daemon"; 	break;
				case 37: shard.Hue = 0x93E; 	shard.DaemonTitle = "the ruby daemon"; 	break;
				case 38: shard.Hue = 0x7CA; 	shard.DaemonTitle = "the rubystar daemon"; 	break;
				case 39: shard.Hue = 0x94D; 	shard.DaemonTitle = "the spinel daemon"; 	break;
				case 40: shard.Hue = 0x883; 	shard.DaemonTitle = "the topaz daemon"; 	break;
				case 41: shard.Hue = 0x95D; 	shard.DaemonTitle = "the valorite daemon"; 	break;
				case 42: shard.Hue = 0x7CB; 	shard.DaemonTitle = "the velvet daemon"; 	break;
				case 43: shard.Hue = 0x95E; 	shard.DaemonTitle = "the verite daemon"; 	break;
				case 44: shard.Hue = 0xB5A; 	shard.DaemonTitle = "the zircon daemon"; 	break;
				case 45: shard.Hue = 0x957; 	shard.DaemonTitle = "the agapite daemon"; 	break;
				case 46: shard.Hue = 0x7C7; 	shard.DaemonTitle = "the akira daemon"; 	break;
				case 47: shard.Hue = 0x7CE; 	shard.DaemonTitle = "the amber daemon"; 	break;
				case 48: shard.Hue = 0x944; 	shard.DaemonTitle = "the azure daemon"; 	break;
				case 49: shard.Hue = 0x8DD; 	shard.DaemonTitle = "the ebony daemon"; 	break;
				case 50: shard.Hue = 0x8E3; 	shard.DaemonTitle = "the evil daemon"; 	break;
				case 51: shard.Hue = 0x942; 	shard.DaemonTitle = "the iron daemon"; 	break;
				case 52: shard.Hue = 0x943; 	shard.DaemonTitle = "the garnet daemon"; 	break;
				case 53: shard.Hue = 0x950; 	shard.DaemonTitle = "the emerald daemon"; 	break;
				case 54: shard.Hue = 0x702; 	shard.DaemonTitle = "the redstar daemon"; 	break;
				case 55: shard.Hue = 0xB3B; 	shard.DaemonTitle = "the marble daemon"; 	break;
				case 56: shard.Hue = 0x708; 	shard.DaemonTitle = "the vermillion daemon"; 	break;
				case 57: shard.Hue = 0x77A; 	shard.DaemonTitle = "the ochre daemon"; 	break;
				case 58: shard.Hue = 0xB5E; 	shard.DaemonTitle = "the onyx daemon"; 	break;
				case 59: shard.Hue = 0x95B; 	shard.DaemonTitle = "the umber daemon"; 	break;
				case 60: shard.Hue = 0x6FB; 	shard.DaemonTitle = "the baneful daemon"; 	break;
				case 61: shard.Hue = 0x870; 	shard.DaemonTitle = "the bloodhorned daemon"; 	break;
				case 62: shard.Hue = 0xA9F; 	shard.DaemonTitle = "the corrupt daemon"; 	break;
				case 63: shard.Hue = 0xBB0; 	shard.DaemonTitle = "the dark daemon"; 	break;
				case 64: shard.Hue = 0x877; 	shard.DaemonTitle = "the dismal daemon"; 	break;
				case 65: shard.Hue = 0x87E; 	shard.DaemonTitle = "the drowhorned daemon"; 	break;
				case 66: shard.Hue = 0x705; 	shard.DaemonTitle = "the gold daemon"; 	break;
				case 67: shard.Hue = 0x8B8; 	shard.DaemonTitle = "the grim daemon"; 	break;
				case 68: shard.Hue = 0x6FD; 	shard.DaemonTitle = "the malicious daemon"; 	break;
				case 69: shard.Hue = 0x86B; 	shard.DaemonTitle = "the shadowhorned daemon"; 	break;
				case 70: shard.Hue = 0x95C; 	shard.DaemonTitle = "the shadowy daemon"; 	break;
				case 71: shard.Hue = 0x7CC; 	shard.DaemonTitle = "the vile daemon"; 	break;
				case 72: shard.Hue = 0x6FE; 	shard.DaemonTitle = "the wicked daemon"; 	break;
				case 73: shard.Hue = 0x6F9; 	shard.DaemonTitle = "the umbra daemon"; 	break;
				case 74: shard.Hue = 0x776; 	shard.DaemonTitle = "the burnt daemon"; 	break;
				case 75: shard.Hue = 0x86C; 	shard.DaemonTitle = "the fire daemon";	break;
				case 76: shard.Hue = 0x701; 	shard.DaemonTitle = "the firelight daemon";	break;
				case 77: shard.Hue = 0xB12; 	shard.DaemonTitle = "the lava daemon";	break;
				case 78: shard.Hue = 0xB38; 	shard.DaemonTitle = "the lavarock daemon"; 	break;
				case 79: shard.Hue = 0xB13; 	shard.DaemonTitle = "the magma daemon";	break;
				case 80: shard.Hue = 0x827; 	shard.DaemonTitle = "the vulcan daemon";	break;
				case 81: shard.Hue = 0xAB3; 	shard.DaemonTitle = "the charcoal daemon"; 	break;
				case 82: shard.Hue = 0xAFA; 	shard.DaemonTitle = "the cinder daemon";	break;
				case 83: shard.Hue = 0x93D; 	shard.DaemonTitle = "the darkfire daemon";	break;
				case 84: shard.Hue = 0xB54; 	shard.DaemonTitle = "the flare daemon";	break;
				case 85: shard.Hue = 0x775; 	shard.DaemonTitle = "the hell daemon";	break;
				case 86: shard.Hue = 0x779; 	shard.DaemonTitle = "the firerock daemon";	break;
				case 87: shard.Hue = 0xB09; 	shard.DaemonTitle = "the steam daemon"; 	break;
				case 88: shard.Hue = 0x85D; 	shard.DaemonTitle = "the forest daemon"; 	break;
				case 89: shard.Hue = 0x6F6; 	shard.DaemonTitle = "the green daemon"; 	break;
				case 90: shard.Hue = 0xB28; 	shard.DaemonTitle = "the greenhorned daemon"; 	break;
				case 91: shard.Hue = 0xB00; 	shard.DaemonTitle = "the evergreen daemon"; 	break;
				case 92: shard.Hue = 0xACC; 	shard.DaemonTitle = "the grove daemon"; 	break;
				case 93: shard.Hue = 0x856; 	shard.DaemonTitle = "the moss daemon"; 	break;
				case 94: shard.Hue = 0x91E; 	shard.DaemonTitle = "the woodland daemon"; 	break;
				case 95: shard.Hue = 0x883; 	shard.DaemonTitle = "the amazon daemon"; 	break;
				case 96: shard.Hue = 0xB44; 	shard.DaemonTitle = "the jungle daemon"; 	break;
				case 97: shard.Hue = 0x706; 	shard.DaemonTitle = "the nova daemon";	break;
				case 98: shard.Hue = 0xAF7; 	shard.DaemonTitle = "the crimson daemon"; 	break;
				case 99: shard.Hue = 0x86A; 	shard.DaemonTitle = "the dusk daemon"; 	break;
				case 100: shard.Hue = 0xB01; 	shard.DaemonTitle = "the red daemon"; 	break;
				case 101: shard.Hue = 0x6FC; 	shard.DaemonTitle = "the sky daemon"; 	break;
				case 102: shard.Hue = 0x95E; 	shard.DaemonTitle = "the spring daemon"; 	break;
				case 103: shard.Hue = 0x703; 	shard.DaemonTitle = "the orchid daemon"; 	break;
				case 104: shard.Hue = 0x981; 	shard.DaemonTitle = "the solar daemon";	break;
				case 105: shard.Hue = 0x6F8; 	shard.DaemonTitle = "the star daemon";	break;
				case 106: shard.Hue = 0x869; 	shard.DaemonTitle = "the sun daemon";	break;
				case 107: shard.Hue = 0x95D; 	shard.DaemonTitle = "the moon daemon"; 	break;
				case 108: shard.Hue = 0xB9D; 	shard.DaemonTitle = "the night daemon"; 	break;
				case 109: shard.Hue = 0xB31; 	shard.DaemonTitle = "the mountain daemon"; 	break;
				case 110: shard.Hue = 0x99B; 	shard.DaemonTitle = "the rock daemon"; 	break;
				case 111: shard.Hue = 0xB32; 	shard.DaemonTitle = "the obsidian daemon"; 	break;
				case 112: shard.Hue = 0x855; 	shard.DaemonTitle = "the blue daemon"; 	break;
				case 113: shard.Hue = 0x959; 	shard.DaemonTitle = "the copper daemon"; 	break;
				case 114: shard.Hue = 0x952; 	shard.DaemonTitle = "the copperish daemon"; 	break;
				case 115: shard.Hue = 0x797; 	shard.DaemonTitle = "the yellow daemon"; 	break;
				case 116: shard.Hue = 0x957; 	shard.DaemonTitle = "the earth daemon"; 	break;
				case 117: shard.Hue = 0x713; 	shard.DaemonTitle = "the desert daemon"; 	break;
				case 118: shard.Hue = 0x8BC; 	shard.DaemonTitle = "the dune daemon"; 	break;
				case 119: shard.Hue = 0x712; 	shard.DaemonTitle = "the sand daemon"; 	break;
				case 120: shard.Hue = 0x945; 	shard.DaemonTitle = "the nepturite daemon"; 	break;
				case 121: shard.Hue = 0x8D1; 	shard.DaemonTitle = "the storm daemon"; 	break;
				case 122: shard.Hue = 0x8C2; 	shard.DaemonTitle = "the tide daemon"; 	break;
				case 123: shard.Hue = 0xB07; 	shard.DaemonTitle = "the seastone daemon"; 	break;
				case 124: shard.Hue = 0x707; 	shard.DaemonTitle = "the aqua daemon"; 	break;
				case 125: shard.Hue = 0xB3D; 	shard.DaemonTitle = "the lagoon daemon"; 	break;
				case 126: shard.Hue = 0x7CD; 	shard.DaemonTitle = "the loch daemon"; 	break;
				case 127: shard.Hue = 0xAE9; 	shard.DaemonTitle = "the algae daemon"; 	break;
				case 128: shard.Hue = 0x854; 	shard.DaemonTitle = "the coastal daemon"; 	break;
				case 129: shard.Hue = 0xB7F; 	shard.DaemonTitle = "the coral daemon"; 	break;
				case 130: shard.Hue = 0xAFF; 	shard.DaemonTitle = "the ivy daemon"; 	break;
				case 131: shard.Hue = 0x860; 	shard.DaemonTitle = "the glacial daemon"; 	break;
				case 132: shard.Hue = 0xAF3; 	shard.DaemonTitle = "the ice daemon";	break;
				case 133: shard.Hue = 0xB7A; 	shard.DaemonTitle = "the icehorned daemon";	break;
				case 134: shard.Hue = 0xAFD; 	shard.DaemonTitle = "the silver daemon"; 	break;
				case 135: shard.Hue = 0x86D; 	shard.DaemonTitle = "the blizzard daemon"; 	break;
				case 136: shard.Hue = 0x87D; 	shard.DaemonTitle = "the frost daemon"; 	break;
				case 137: shard.Hue = 0x8BA; 	shard.DaemonTitle = "the snow daemon"; 	break;
				case 138: shard.Hue = 0x911; 	shard.DaemonTitle = "the white daemon"; 	break;
				case 139: shard.Hue = 0xAB1; 	shard.DaemonTitle = "the black daemon"; 	break;
				case 140: shard.Hue = 0x88D; 	shard.DaemonTitle = "the mire daemon"; 	break;
				case 141: shard.Hue = 0x945; 	shard.DaemonTitle = "the moor daemon"; 	break;
				case 142: shard.Hue = 0x8B2; 	shard.DaemonTitle = "the bog daemon"; 	break;
				case 143: shard.Hue = 0xB27; 	shard.DaemonTitle = "the boghorned daemon"; 	break;
				case 144: shard.Hue = 0x77D; 	shard.DaemonTitle = "the swampfire daemon";	break;
				case 145: shard.Hue = 0x8EC; 	shard.DaemonTitle = "the marsh daemon"; 	break;
				case 146: shard.Hue = 0x7C7; 	shard.DaemonTitle = "the xormite daemon";	break;
			}

			shard.Name = "Shard of " + shard.DaemonName + " " + shard.DaemonTitle;
		}

		public class DemonPrisonGump : Gump
		{
			public DemonPrisonGump( Mobile from, DemonPrison shard ): base( 50, 50 )
			{
				from.SendSound( 0x4A ); 
				string color = "#c97e75"; 

				string sText = "This shard contains a trapped daemon. Warlocks would take these shards and combine them with the four crystals of daemonic power. The shard of hellfire, the shard of the abyss, the shard of souls, and the shard of the void are the four crystals used in this process. These are usually in long lost dungeons or ruins, and said to be last seen in small chests resting on a runic pedestal. Once these crystals are combined with the daemon shard, you can double click the shard and add the gold coins as funds. Once everything is gathered, you can shatter the crystal and free the daemon. The daemon will then owe a life debt to the one that freed it. These arcane skills are rarely ever used today, but you did hear rumors of these various shards being seen in certain places. If you could get them, and bring these shards to a mage or necromancer (spell casters), they may be able to help you release it as you cannot do it alone. The spell caster will require some gold as you will need the help of a particular spell caster and they will require payment for their services. This spell caster is at the location shown below. If you have any magery or necromancy skill, they may refund some of the gold for the help you may provide in the release. When released, these daemons will become your bonded ally. You will have to feed them and stable them when required. You can also perform some druidism on them without having any proficiency in the skill. This will help you with information about them, like what they want to eat.";

				string sRumor = shard.PieceRumor + " " + shard.PieceLocation;

				if ( shard.HaveShardA == 0 ){ sRumor = "The shard of hellfire " + sRumor; }
				else if ( shard.HaveShardB == 0 ){ sRumor = "The shard of the abyss " + sRumor; }
				else if ( shard.HaveShardC == 0 ){ sRumor = "The shard of souls " + sRumor; }
				else if ( shard.HaveShardD == 0 ){ sRumor = "The shard of the void " + sRumor; }
				else if ( shard.HaveGold < shard.NeedGold ){ sRumor = "You have obtained everything except the gold."; }
				else { sRumor = "You have obtained everything you need."; }

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				AddImage(0, 0, 7013, Server.Misc.PlayerSettings.GetGumpHue( from ));
				AddHtml( 12, 12, 420, 20, @"<BODY><BASEFONT Color=" + color + ">" + (shard.Name).ToUpper() + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(863, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);

				AddHtml( 12, 40, 173, 20, @"<BODY><BASEFONT Color=" + color + ">Gold: " + shard.HaveGold.ToString() + "/" + shard.NeedGold.ToString() + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 12, 70, 874, 20, @"<BODY><BASEFONT Color=" + color + ">Bring Gathered Materials to the spell caster in " + shard.WizardLocation + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 12, 100, 874, 20, @"<BODY><BASEFONT Color=" + color + ">" + sRumor + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 12, 339, 878, 251, @"<BODY><BASEFONT Color=" + color + ">" + sText + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddItem(734, 120, 8452, shard.Hue);

				AddItem(77, 253, 13042);
				AddItem(282, 254, 13042);
				AddItem(484, 254, 13042);
				AddItem(673, 253, 13042);

				if ( shard.HaveShardA > 0 ){ AddItem(77, 245, 1795); }
				if ( shard.HaveShardB > 0 ){ AddItem(283, 247, 1796); }
				if ( shard.HaveShardC > 0 ){ AddItem(486, 247, 1797); }
				if ( shard.HaveShardD > 0 ){ AddItem(673, 244, 1798); }
			}

			public override void OnResponse(NetState state, RelayInfo info)
			{
				Mobile from = state.Mobile;
				from.SendSound( 0x4A ); 
			}
		}

		public string WizardLocation;
		[CommandProperty( AccessLevel.GameMaster )]
		public string g_WizardLocation { get{ return WizardLocation; } set{ WizardLocation = value; } }

		public string PieceLocation;
		[CommandProperty( AccessLevel.GameMaster )]
		public string g_PieceLocation { get{ return PieceLocation; } set{ PieceLocation = value; } }

		public string PieceRumor;
		[CommandProperty( AccessLevel.GameMaster )]
		public string g_PieceRumor { get{ return PieceRumor; } set{ PieceRumor = value; } }

		public int DaemonType;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_DaemonType { get{ return DaemonType; } set{ DaemonType = value; } }

		public int DaemonBody;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_DaemonBody { get{ return DaemonBody; } set{ DaemonBody = value; } }

		public string DaemonName;
		[CommandProperty( AccessLevel.GameMaster )]
		public string g_DaemonName { get{ return DaemonName; } set{ DaemonName = value; } }

		public string DaemonTitle;
		[CommandProperty( AccessLevel.GameMaster )]
		public string g_DaemonTitle { get{ return DaemonTitle; } set{ DaemonTitle = value; } }

		// ----------------------------------------------------------------------------------------

		public int NeedGold;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_NeedGold { get{ return NeedGold; } set{ NeedGold = value; } }

		// ----------------------------------------------------------------------------------------

		public int HaveShardA;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveShardA { get{ return HaveShardA; } set{ HaveShardA = value; } }

		public int HaveGold;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveGold { get{ return HaveGold; } set{ HaveGold = value; } }

		public int HaveShardC;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveShardC { get{ return HaveShardC; } set{ HaveShardC = value; } }

		public int HaveShardB;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveShardB { get{ return HaveShardB; } set{ HaveShardB = value; } }

		public int HaveShardD;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveShardD { get{ return HaveShardD; } set{ HaveShardD = value; } }
	}
}

namespace Server.Mobiles
{
	[CorpseName( "a daemon corpse" )]
	public class Daemonic : BaseCreature
	{
		[Constructable]
		public Daemonic() : this( 9, 0 )
		{
		}

		[Constructable]
		public Daemonic ( int body, int hue ) : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			rBody = body;
			Hue = hue; 
			Name = NameList.RandomName( "daemon" );
			Title = "daemon";
			Body = body;
			BaseSoundID = 357;
		}

		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool HasBreath{ get{ return true; } }

		public Daemonic( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
			writer.Write( rBody );
			writer.Write( rHue );
			writer.Write( rName );
			writer.Write( rCategory );
			writer.Write( rMainColor );
			writer.Write( rFood );
			writer.Write( rDrop );
			writer.Write( rTitle );
			writer.Write( rPoison );
			writer.Write( rBlood );
			writer.Write( rBreath );
			writer.Write( rBreathPhysDmg );
			writer.Write( rBreathFireDmg );
			writer.Write( rBreathColdDmg );
			writer.Write( rBreathPoisDmg );
			writer.Write( rBreathEngyDmg );
			writer.Write( rBreathHue );
			writer.Write( rBreathSound );
			writer.Write( rBreathItemID );
			writer.Write( rBreathDelay );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			rBody = reader.ReadInt();
			rHue = reader.ReadInt();
			rName = reader.ReadString();
			rCategory = reader.ReadString();
			rMainColor = reader.ReadString();
			rFood = reader.ReadString();
			rDrop = reader.ReadString();
			rTitle = reader.ReadString();
			rPoison = reader.ReadInt();
			rBlood = reader.ReadString();
			rBreath = reader.ReadInt();
			rBreathPhysDmg = reader.ReadInt();
			rBreathFireDmg = reader.ReadInt();
			rBreathColdDmg = reader.ReadInt();
			rBreathPoisDmg = reader.ReadInt();
			rBreathEngyDmg = reader.ReadInt();
			rBreathHue = reader.ReadInt();
			rBreathSound = reader.ReadInt();
			rBreathItemID = reader.ReadInt();
			rBreathDelay = reader.ReadDouble();
		}

		public override void OnAfterSpawn()
		{
			CreateDaemonic();
			base.OnAfterSpawn();
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if ( Utility.RandomMinMax( 1, 4 ) == 1 && !Controlled && rBlood != "" && rBlood != null && rBlood != "rust" )
			{
				int goo = 0;

				foreach ( Item splash in this.GetItemsInRange( 10 ) ){ if ( splash is MonsterSplatter ){ goo++; } }

				if ( goo == 0 )
				{
					if ( rBlood == "glowing goo" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "glowing goo", 0xB93, 1 ); }
					else if ( rBlood == "scorching ooze" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "scorching ooze", 0x496, 0 ); }
					else if ( rBlood == "poisonous slime" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "poisonous slime", 1167, 0 ); }
					else if ( rBlood == "toxic blood" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "toxic blood", 0x48E, 1 ); }
					else if ( rBlood == "toxic goo" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "toxic goo", 0xB93, 1 ); }
					else if ( rBlood == "hot magma" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "hot magma", 0x489, 1 ); }
					else if ( rBlood == "acidic slime" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "acidic slime", 1167, 0 ); }
					else if ( rBlood == "freezing water" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "freezing water", 296, 0 ); }
					else if ( rBlood == "scorching ooze" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "scorching ooze", 0x496, 0 ); }
					else if ( rBlood == "blue slime" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "blue slime", 0x5B6, 1 ); }
					else if ( rBlood == "green blood" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "green blood", 0x7D1, 0 ); }
					else if ( rBlood == "quick silver" ){ MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "quick silver", 0xB37, 1 ); }
					else { MonsterSplatter.AddSplatter( this.X, this.Y, this.Z, this.Map, this.Location, this, "thick blood", 0x485, 0 ); }
				}
			}
		}

		public override void OnGaveMeleeAttack( Mobile m )
		{
			base.OnGaveMeleeAttack( m );

			if ( 1 == Utility.RandomMinMax( 1, 20 ) && rBlood == "rust" && m is PlayerMobile )
			{
				Container cont = m.Backpack;
				Item iRuined = Server.Items.HiddenTrap.GetMyItem( m );

				if ( iRuined != null )
				{
					if ( Server.Items.HiddenTrap.IAmShielding( m, 100 ) || Server.Items.HiddenTrap.IAmAWeaponSlayer( m, this ) )
					{
					}
					else if ( m.CheckSkill( SkillName.MagicResist, 0, 100 ) )
					{
					}
					else if ( iRuined is BaseWeapon )
					{
						BaseWeapon iRusted = (BaseWeapon)iRuined;

						if ( CraftResources.GetType( iRuined.Resource ) == CraftResourceType.Metal )
						{
							if ( Server.Items.HiddenTrap.CheckInsuranceOnTrap( iRuined, m ) )
							{
								m.LocalOverheadMessage(MessageType.Emote, 1150, true, "The daemon almost rusted one of your protected items!");
							}
							else
							{
								m.LocalOverheadMessage(MessageType.Emote, 0x916, true, "The daemon rusted one of your equipped items!");
								RustyJunk broke = new RustyJunk();
								broke.ItemID = iRuined.GraphicID;
								broke.Name = "rusted item";
								broke.Weight = iRuined.Weight;
								m.AddToBackpack ( broke );
								iRuined.Delete();
							}
						}
					}
					if ( iRuined is BaseArmor )
					{
						BaseArmor iRusted = (BaseArmor)iRuined;

						if ( CraftResources.GetType( iRuined.Resource ) == CraftResourceType.Metal )
						{
							if ( Server.Items.HiddenTrap.CheckInsuranceOnTrap( iRuined, m ) )
							{
								m.LocalOverheadMessage(MessageType.Emote, 1150, true, "The daemon almost rusted one of your protected items!");
							}
							else
							{
								m.LocalOverheadMessage(MessageType.Emote, 0x916, true, "The daemon rusted one of your equipped items!");
								RustyJunk broke = new RustyJunk();
								broke.ItemID = iRuined.ItemID;
								broke.Name = "rusted item";
								broke.Weight = Utility.RandomMinMax( 1, 4 );
								m.AddToBackpack ( broke );
								iRuined.Delete();
							}
						}
					}
				}
			}
		}

		public int rBody;
		[CommandProperty(AccessLevel.Owner)]
		public int r_Body { get { return rBody; } set { rBody = value; InvalidateProperties(); } }

		public int rHue;
		[CommandProperty(AccessLevel.Owner)]
		public int r_Hue { get { return rHue; } set { rHue = value; InvalidateProperties(); } }

		public string rName;
		[CommandProperty(AccessLevel.Owner)]
		public string r_Name { get { return rName; } set { rName = value; InvalidateProperties(); } }

		public string rCategory;
		[CommandProperty(AccessLevel.Owner)]
		public string r_Category { get { return rCategory; } set { rCategory = value; InvalidateProperties(); } }

		public string rMainColor;
		[CommandProperty(AccessLevel.Owner)]
		public string r_MainColor { get { return rMainColor; } set { rMainColor = value; InvalidateProperties(); } }

		public string rFood;
		[CommandProperty(AccessLevel.Owner)]
		public string r_Food { get { return rFood; } set { rFood = value; InvalidateProperties(); } }

		public string rDrop;
		[CommandProperty(AccessLevel.Owner)]
		public string r_Drop { get { return rDrop; } set { rDrop = value; InvalidateProperties(); } }

		public string rTitle;
		[CommandProperty(AccessLevel.Owner)]
		public string r_Title { get { return rTitle; } set { rTitle = value; InvalidateProperties(); } }

		public int rPoison;
		[CommandProperty(AccessLevel.Owner)]
		public int r_Poison { get { return rPoison; } set { rPoison = value; InvalidateProperties(); } }

		public string rBlood;
		[CommandProperty(AccessLevel.Owner)]
		public string r_Blood { get { return rBlood; } set { rBlood = value; InvalidateProperties(); } }

		public int rBreath;
		[CommandProperty(AccessLevel.Owner)]
		public int r_Breath { get { return rBreath; } set { rBreath = value; InvalidateProperties(); } }

		public int rBreathPhysDmg;
		[CommandProperty(AccessLevel.Owner)]
		public int r_BreathPhysDmg { get { return rBreathPhysDmg; } set { rBreathPhysDmg = value; InvalidateProperties(); } }

		public int rBreathFireDmg;
		[CommandProperty(AccessLevel.Owner)]
		public int r_BreathFireDmg { get { return rBreathFireDmg; } set { rBreathFireDmg = value; InvalidateProperties(); } }

		public int rBreathColdDmg;
		[CommandProperty(AccessLevel.Owner)]
		public int r_BreathColdDmg { get { return rBreathColdDmg; } set { rBreathColdDmg = value; InvalidateProperties(); } }

		public int rBreathPoisDmg;
		[CommandProperty(AccessLevel.Owner)]
		public int r_BreathPoisDmg { get { return rBreathPoisDmg; } set { rBreathPoisDmg = value; InvalidateProperties(); } }

		public int rBreathEngyDmg;
		[CommandProperty(AccessLevel.Owner)]
		public int r_BreathEngyDmg { get { return rBreathEngyDmg; } set { rBreathEngyDmg = value; InvalidateProperties(); } }

		public int rBreathHue;
		[CommandProperty(AccessLevel.Owner)]
		public int r_BreathHue { get { return rBreathHue; } set { rBreathHue = value; InvalidateProperties(); } }

		public int rBreathSound;
		[CommandProperty(AccessLevel.Owner)]
		public int r_BreathSound { get { return rBreathSound; } set { rBreathSound = value; InvalidateProperties(); } }

		public int rBreathItemID;
		[CommandProperty(AccessLevel.Owner)]
		public int r_BreathItemID { get { return rBreathItemID; } set { rBreathItemID = value; InvalidateProperties(); } }

		public double rBreathDelay;
		[CommandProperty(AccessLevel.Owner)]
		public double r_BreathDelay { get { return rBreathDelay; } set { rBreathDelay = value; InvalidateProperties(); } }

		public override FoodType FavoriteFood
		{
			get
			{
				if ( rFood == "fish" )
					return ( FoodType.Fish );

				else if ( rFood == "gold" )
					return ( FoodType.Gold );

				else if ( rFood == "fire" )
					return ( FoodType.Fire );

				else if ( rFood == "gems" )
					return ( FoodType.Gems );

				else if ( rFood == "nox" )
					return ( FoodType.Nox );

				else if ( rFood == "sea" )
					return ( FoodType.Sea );

				else if ( rFood == "moon" )
					return ( FoodType.Moon );

				else if ( rFood == "fire_meat" )
					return FoodType.Fire | FoodType.Meat; 

				else if ( rFood == "fish_sea" )
					return FoodType.Fish | FoodType.Sea; 

				else if ( rFood == "gems_fire" )
					return FoodType.Gems | FoodType.Fire; 

				else if ( rFood == "gems_gold" )
					return FoodType.Gems | FoodType.Gold; 

				else if ( rFood == "gems_meat" )
					return FoodType.Gems | FoodType.Meat; 

				else if ( rFood == "gems_moon" )
					return FoodType.Gems | FoodType.Moon; 

				else if ( rFood == "meat_nox" )
					return FoodType.Meat | FoodType.Nox; 

				else if ( rFood == "moon_fire" )
					return FoodType.Moon | FoodType.Fire; 

				else if ( rFood == "nox_fire" )
					return FoodType.Nox | FoodType.Fire; 

				return ( FoodType.Meat );
			}
		}

		public override Poison PoisonImmune
		{
			get
			{
				if ( rPoison > 0 )
					return Poison.Deadly;

				if ( rPoison == 0 )
					return Poison.Regular;

				return null;
			}
		}

		public override Poison HitPoison
		{
			get
			{
				if ( rPoison > 0 )
					return Poison.Greater;

				return null;
			}
		}

		public override void BreathDealDamage( Mobile target, int form ){ base.BreathDealDamage( target, rBreath ); }

		public override int BreathPhysicalDamage{ get{ return rBreathPhysDmg; } }
		public override int BreathFireDamage{ get{ return rBreathFireDmg; } }
		public override int BreathColdDamage{ get{ return rBreathColdDmg; } }
		public override int BreathPoisonDamage{ get{ return rBreathPoisDmg; } }
		public override int BreathEnergyDamage{ get{ return rBreathEngyDmg; } }
		public override int BreathEffectHue{ get{ return rBreathHue; } }
		public override int BreathEffectSound{ get{ return rBreathSound; } }
		public override int BreathEffectItemID{ get{ return rBreathItemID; } }

		public void CreateDaemonic()
		{
			if ( rHue < 1 )
			{
				rBody = Body;
				bool bright = false;

				int daemon = Utility.RandomMinMax( 1, 146 );

				if ( Hue > 0 ){ daemon = Hue; }

				switch ( daemon )
				{
					case 1: rHue = 0x8E4; rMainColor = "red"; rName = "a bloodstone daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = "thick blood"; break;
					case 2: rHue = 0xB2A; rMainColor = "white"; rName = "a mercury daemon"; rFood = "gems_gold"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = "quick silver"; break;
					case 3: rHue = 0x916; rMainColor = "red"; rName = "a scarlet daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = "toxic blood"; break;
					case 4: rHue = 0xB51; rMainColor = "green"; rName = "a poison daemon"; rFood = "nox"; rDrop = ""; rCategory = "poison"; rBreath = 10; rPoison = 1; rBlood = "poisonous slime"; break;
					case 5: rHue = 0x82B; rMainColor = "yellow"; rName = "a glare daemon"; bright = true; rFood = "gold"; rDrop = ""; rCategory = "radiation"; rBreath = 11; rPoison = 0; rBlood = "glowing goo"; break;
					case 6: rHue = 0x8D8; rMainColor = "white"; rName = "a glaze daemon"; bright = true; rFood = "gold"; rDrop = ""; rCategory = "radiation"; rBreath = 11; rPoison = 0; rBlood = "glowing goo"; break;
					case 7: rHue = 0x921; rMainColor = "white"; rName = "a radiant daemon"; bright = true; rFood = "moon"; rDrop = ""; rCategory = "radiation"; rBreath = 11; rPoison = 0; rBlood = "toxic goo"; break;
					case 8: rHue = 0x77C; rMainColor = "red"; rName = "a blood daemon"; rFood = "meat"; rDrop = ""; rCategory = "void"; rBreath = 24; rPoison = 0; rBlood = "thick blood"; break;
					case 9: rHue = 0x871; rMainColor = "rust"; rName = "a rust daemon"; rFood = "gold"; rDrop = ""; rCategory = "poison"; rBreath = 10; rPoison = 1; rBlood = "rust"; break;
					case 10: rHue = 0x996; rMainColor = "blue"; rName = "a sapphire daemon"; rFood = "gems"; rDrop = "sapphire"; rCategory = "cold"; rBreath = 12; rPoison = 0; rBlood = ""; break;
					case 11: rHue = 0xB56; rMainColor = "blue"; rName = "a azurite daemon"; rFood = "meat"; rDrop = ""; rCategory = "cold"; rBreath = 12; rPoison = 0; rBlood = ""; break;
					case 12: rHue = 0x95B; rMainColor = "yellow"; rName = "a brass daemon"; rFood = "meat"; rDrop = "brass"; rCategory = "electrical"; rBreath = 13; rPoison = 0; rBlood = ""; break;
					case 13: rHue = 0x796; rMainColor = "blue"; rName = "a cobolt daemon"; rFood = "meat"; rDrop = ""; rCategory = "electrical"; rBreath = 13; rPoison = 0; rBlood = ""; break;
					case 14: rHue = 0xB65; rMainColor = "white"; rName = "a mithril daemon"; rFood = "gems_gold"; rDrop = "mithril"; rCategory = "electrical"; rBreath = 13; rPoison = 0; rBlood = ""; break;
					case 15: rHue = 0xB05; rMainColor = "ice"; rName = "a palladium daemon"; rFood = "gems_gold"; rDrop = ""; rCategory = "electrical"; rBreath = 13; rPoison = 0; rBlood = ""; break;
					case 16: rHue = 0xB3B; rMainColor = "white"; rName = "a pearl daemon"; rFood = "fish_sea"; rDrop = ""; rCategory = "electrical"; rBreath = 13; rPoison = 0; rBlood = ""; break;
					case 17: rHue = 0x99F; rMainColor = "blue"; rName = "a steel daemon"; rFood = "meat"; rDrop = "steel"; rCategory = "electrical"; rBreath = 13; rPoison = 0; rBlood = ""; break;
					case 18: rHue = 0x98B; rMainColor = "ice"; rName = "a titanium daemon"; rFood = "gems_gold"; rDrop = ""; rCategory = "electrical"; rBreath = 13; rPoison = 0; rBlood = ""; break;
					case 19: rHue = 0xB7C; rMainColor = "ice"; rName = "a turquoise daemon"; rFood = "gems"; rDrop = ""; rCategory = "electrical"; rBreath = 13; rPoison = 0; rBlood = ""; break;
					case 20: rHue = 0x6F7; rMainColor = "purple"; rName = "a violet daemon"; rFood = "meat"; rDrop = ""; rCategory = "electrical"; rBreath = 13; rPoison = 0; rBlood = ""; break;
					case 21: rHue = 0x7C3; rMainColor = "purple"; rName = "a amethyst daemon"; rFood = "gems"; rDrop = "amethyst"; rCategory = "electrical"; rBreath = 13; rPoison = 0; rBlood = ""; break;
					case 22: rHue = 0x7C6; rMainColor = "yellow"; rName = "a bright daemon"; bright = true; rFood = "fire"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 23: rHue = 0x92B; rMainColor = "yellow"; rName = "a bronze daemon"; rFood = "meat"; rDrop = "bronze"; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 24: rHue = 0x943; rMainColor = "green"; rName = "a cadmium daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 25: rHue = 0x8D0; rMainColor = "blue"; rName = "a cerulean daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 26: rHue = 0x8B6; rMainColor = "purple"; rName = "a darkhorned daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 27: rHue = 0xB7E; rMainColor = "white"; rName = "a diamond daemon"; rFood = "gems"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 28: rHue = 0xB1B; rMainColor = "bright"; rName = "a gilded daemon"; rFood = "gold"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 29: rHue = 0x829; rMainColor = "gray"; rName = "a grey daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 30: rHue = 0xB94; rMainColor = "green"; rName = "a jade daemon"; rFood = "meat"; rDrop = "jade"; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 31: rHue = 0x77E; rMainColor = "green"; rName = "a jadefire daemon"; bright = true; rFood = "fire_meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 32: rHue = 0x88B; rMainColor = "black"; rName = "a murky daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 33: rHue = 0x994; rMainColor = "ice"; rName = "a platinum daemon"; rFood = "gems_moon"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 34: rHue = 0x6F5; rMainColor = "purple"; rName = "a purple daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 35: rHue = 0x869; rMainColor = "yellow"; rName = "a quartz daemon"; rFood = "gems"; rDrop = "quartz"; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 36: rHue = 0xB02; rMainColor = "red"; rName = "a sanguine daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 37: rHue = 0x93E; rMainColor = "red"; rName = "a ruby daemon"; rFood = "gems"; rDrop = "ruby"; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 38: rHue = 0x7CA; rMainColor = "red"; rName = "a rubystar daemon"; bright = true; rFood = "gems_moon"; rDrop = "star ruby"; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 39: rHue = 0x94D; rMainColor = "purple"; rName = "a spinel daemon"; rFood = "gems"; rDrop = "spinel"; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 40: rHue = 0x883; rMainColor = "blue"; rName = "a topaz daemon"; rFood = "gems"; rDrop = "topaz"; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 41: rHue = 0x95D; rMainColor = "blue"; rName = "a valorite daemon"; rFood = "meat"; rDrop = "valorite"; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 42: rHue = 0x7CB; rMainColor = "purple"; rName = "a velvet daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 43: rHue = 0x95E; rMainColor = "green"; rName = "a verite daemon"; rFood = "meat"; rDrop = "verite"; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 44: rHue = 0xB5A; rMainColor = "blue"; rName = "a zircon daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 45: rHue = 0x957; rMainColor = "red"; rName = "a agapite daemon"; rFood = "meat"; rDrop = "agapite"; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 46: rHue = 0x7C7; rMainColor = "green"; rName = "a akira daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 47: rHue = 0x7CE; rMainColor = "yellow"; rName = "a amber daemon"; rFood = "gems"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 48: rHue = 0x944; rMainColor = "blue"; rName = "a azure daemon"; rFood = "gems_gold"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 49: rHue = 0x8DD; rMainColor = "black"; rName = "a ebony daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 50: rHue = 0x8E3; rMainColor = "purple"; rName = "a evil daemon"; rFood = "fire_meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 51: rHue = 0x942; rMainColor = "white"; rName = "a iron daemon"; rFood = "meat"; rDrop = "iron"; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 52: rHue = 0x943; rMainColor = "green"; rName = "a garnet daemon"; rFood = "nox"; rDrop = "garnet"; rCategory = "poison"; rBreath = 10; rPoison = 0; rBlood = ""; break;
					case 53: rHue = 0x950; rMainColor = "green"; rName = "a emerald daemon"; rFood = "nox"; rDrop = "emerald"; rCategory = "poison"; rBreath = 10; rPoison = 0; rBlood = ""; break;
					case 54: rHue = 0x702; rMainColor = "red"; rName = "a redstar daemon"; bright = true; rFood = "moon_fire"; rDrop = ""; rCategory = "radiation"; rBreath = 11; rPoison = 0; rBlood = ""; break;
					case 55: rHue = 0xB3B; rMainColor = "white"; rName = "a marble daemon"; rFood = "gems"; rDrop = "marble"; rCategory = "steam"; rBreath = 16; rPoison = 0; rBlood = ""; break;
					case 56: rHue = 0x708; rMainColor = "red"; rName = "a vermillion daemon"; rFood = "meat"; rDrop = ""; rCategory = "steam"; rBreath = 16; rPoison = 0; rBlood = ""; break;
					case 57: rHue = 0x77A; rMainColor = "red"; rName = "a ochre daemon"; rFood = "meat"; rDrop = ""; rCategory = "steam"; rBreath = 16; rPoison = 0; rBlood = ""; break;
					case 58: rHue = 0xB5E; rMainColor = "black"; rName = "a onyx daemon"; rFood = "meat"; rDrop = "onyx"; rCategory = "steam"; rBreath = 16; rPoison = 0; rBlood = ""; break;
					case 59: rHue = 0x95B; rMainColor = "yellow"; rName = "a umber daemon"; rFood = "meat"; rDrop = ""; rCategory = "steam"; rBreath = 16; rPoison = 0; rBlood = ""; break;
					case 60: rHue = 0x6FB; rMainColor = "purple"; rName = "a baneful daemon"; rFood = "meat"; rDrop = ""; rCategory = "void"; rBreath = 24; rPoison = 0; rBlood = ""; break;
					case 61: rHue = 0x870; rMainColor = "red"; rName = "a bloodhorned daemon"; rFood = "meat"; rDrop = ""; rCategory = "void"; rBreath = 24; rPoison = 0; rBlood = ""; break;
					case 62: rHue = 0xA9F; rMainColor = "purple"; rName = "a corrupt daemon"; rFood = "meat"; rDrop = ""; rCategory = "void"; rBreath = 24; rPoison = 0; rBlood = ""; break;
					case 63: rHue = 0xBB0; rMainColor = "black"; rName = "a dark daemon"; rFood = "meat"; rDrop = ""; rCategory = "void"; rBreath = 25; rPoison = 0; rBlood = ""; break;
					case 64: rHue = 0x877; rMainColor = "black"; rName = "a dismal daemon"; rFood = "meat"; rDrop = ""; rCategory = "void"; rBreath = 25; rPoison = 0; rBlood = ""; break;
					case 65: rHue = 0x87E; rMainColor = "purple"; rName = "a drowhorned daemon"; rFood = "meat"; rDrop = ""; rCategory = "void"; rBreath = 24; rPoison = 0; rBlood = ""; break;
					case 66: rHue = 0x705; rMainColor = "yellow"; rName = "a gold daemon"; rFood = "gold"; rDrop = "gold"; rCategory = "void"; rBreath = 23; rPoison = 0; rBlood = ""; break;
					case 67: rHue = 0x8B8; rMainColor = "black"; rName = "a grim daemon"; rFood = "meat"; rDrop = ""; rCategory = "void"; rBreath = 25; rPoison = 0; rBlood = ""; break;
					case 68: rHue = 0x6FD; rMainColor = "purple"; rName = "a malicious daemon"; rFood = "meat"; rDrop = ""; rCategory = "void"; rBreath = 24; rPoison = 0; rBlood = ""; break;
					case 69: rHue = 0x86B; rMainColor = "black"; rName = "a shadowhorned daemon"; rFood = "meat"; rDrop = ""; rCategory = "void"; rBreath = 25; rPoison = 0; rBlood = ""; break;
					case 70: rHue = 0x95C; rMainColor = "black"; rName = "a shadowy daemon"; rFood = "meat"; rDrop = "shadow iron"; rCategory = "void"; rBreath = 25; rPoison = 0; rBlood = ""; break;
					case 71: rHue = 0x7CC; rMainColor = "purple"; rName = "a vile daemon"; rFood = "meat"; rDrop = ""; rCategory = "void"; rBreath = 24; rPoison = 0; rBlood = ""; break;
					case 72: rHue = 0x6FE; rMainColor = "purple"; rName = "a wicked daemon"; rFood = "meat"; rDrop = ""; rCategory = "void"; rBreath = 24; rPoison = 0; rBlood = ""; break;
					case 73: rHue = 0x6F9; rMainColor = "void"; rName = "a umbra daemon"; rFood = "meat"; rDrop = ""; rCategory = "void"; rBreath = 24; rPoison = 0; rBlood = ""; break;
					case 74: rHue = 0x776; rMainColor = "red"; rName = "a burnt daemon"; bright = true; rFood = "fire"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = "hot magma"; break;
					case 75: rHue = 0x86C; rMainColor = "red"; rName = "a fire daemon"; bright = true; rFood = "fire"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = "hot magma"; break;
					case 76: rHue = 0x701; rMainColor = "red"; rName = "a firelight daemon"; bright = true; rFood = "fire"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = "scorching ooze"; break;
					case 77: rHue = 0xB12; rMainColor = "red"; rName = "a lava daemon"; bright = true; rFood = "fire"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = "hot magma"; break;
					case 78: rHue = 0xB38; rMainColor = "black"; rName = "a lavarock daemon"; rFood = "fire"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = "hot magma"; break;
					case 79: rHue = 0xB13; rMainColor = "red"; rName = "a magma daemon"; bright = true; rFood = "fire"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = "hot magma"; break;
					case 80: rHue = 0x827; rMainColor = "red"; rName = "a vulcan daemon"; bright = true; rFood = "fire"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = "hot magma"; break;
					case 81: rHue = 0xAB3; rMainColor = "black"; rName = "a charcoal daemon"; rFood = "fire"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 82: rHue = 0xAFA; rMainColor = "red"; rName = "a cinder daemon"; bright = true; rFood = "fire"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 83: rHue = 0x93D; rMainColor = "red"; rName = "a darkfire daemon"; bright = true; rFood = "fire"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 84: rHue = 0xB54; rMainColor = "red"; rName = "a flare daemon"; bright = true; rFood = "fire"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 85: rHue = 0x775; rMainColor = "red"; rName = "a hell daemon"; bright = true; rFood = "fire"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 86: rHue = 0x779; rMainColor = "red"; rName = "a firerock daemon"; bright = true; rFood = "fire_meat"; rDrop = ""; rCategory = "steam"; rBreath = 16; rPoison = 0; rBlood = ""; break;
					case 87: rHue = 0xB09; rMainColor = "white"; rName = "a steam daemon"; rFood = "meat"; rDrop = "granite"; rCategory = "steam"; rBreath = 16; rPoison = 0; rBlood = ""; break;
					case 88: rHue = 0x85D; rMainColor = "green"; rName = "a forest daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 89: rHue = 0x6F6; rMainColor = "green"; rName = "a green daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 90: rHue = 0xB28; rMainColor = "green"; rName = "a greenhorned daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 91: rHue = 0xB00; rMainColor = "sea"; rName = "a evergreen daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 92: rHue = 0xACC; rMainColor = "green"; rName = "a grove daemon"; rFood = "meat"; rDrop = ""; rCategory = "weed"; rBreath = 34; rPoison = 0; rBlood = ""; break;
					case 93: rHue = 0x856; rMainColor = "sea"; rName = "a moss daemon"; rFood = "meat"; rDrop = ""; rCategory = "weed"; rBreath = 34; rPoison = 0; rBlood = ""; break;
					case 94: rHue = 0x91E; rMainColor = "green"; rName = "a woodland daemon"; rFood = "meat"; rDrop = ""; rCategory = "weed"; rBreath = 34; rPoison = 0; rBlood = ""; break;
					case 95: rHue = 0x883; rMainColor = "sea"; rName = "a amazon daemon"; rFood = "meat_nox"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = "green blood"; break;
					case 96: rHue = 0xB44; rMainColor = "green"; rName = "a jungle daemon"; rFood = "meat"; rDrop = ""; rCategory = "weed"; rBreath = 34; rPoison = 0; rBlood = ""; break;
					case 97: rHue = 0x706; rMainColor = "yellow"; rName = "a nova daemon"; bright = true; rFood = "moon"; rDrop = ""; rCategory = "radiation"; rBreath = 11; rPoison = 0; rBlood = "glowing goo"; break;
					case 98: rHue = 0xAF7; rMainColor = "red"; rName = "a crimson daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 99: rHue = 0x86A; rMainColor = "vile"; rName = "a dusk daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 100: rHue = 0xB01; rMainColor = "red"; rName = "a red daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 101: rHue = 0x6FC; rMainColor = "blue"; rName = "a sky daemon"; rFood = "meat"; rDrop = ""; rCategory = "wind"; rBreath = 47; rPoison = 0; rBlood = ""; break;
					case 102: rHue = 0x95E; rMainColor = "green"; rName = "a spring daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 103: rHue = 0x703; rMainColor = "purple"; rName = "a orchid daemon"; rFood = "meat_nox"; rDrop = ""; rCategory = "poison"; rBreath = 9; rPoison = 1; rBlood = ""; break;
					case 104: rHue = 0x981; rMainColor = "red"; rName = "a solar daemon"; bright = true; rFood = "moon_fire"; rDrop = ""; rCategory = "radiation"; rBreath = 11; rPoison = 0; rBlood = ""; break;
					case 105: rHue = 0x6F8; rMainColor = "white"; rName = "a star daemon"; bright = true; rFood = "moon"; rDrop = ""; rCategory = "star"; rBreath = 45; rPoison = 0; rBlood = ""; break;
					case 106: rHue = 0x869; rMainColor = "yellow"; rName = "a sun daemon"; bright = true; rFood = "moon"; rDrop = ""; rCategory = "radiation"; rBreath = 11; rPoison = 0; rBlood = ""; break;
					case 107: rHue = 0x95D; rMainColor = "blue"; rName = "a moon daemon"; rFood = "moon"; rDrop = ""; rCategory = "void"; rBreath = 25; rPoison = 0; rBlood = ""; break;
					case 108: rHue = 0xB9D; rMainColor = "black"; rName = "a night daemon"; rFood = "moon"; rDrop = ""; rCategory = "void"; rBreath = 25; rPoison = 0; rBlood = ""; break;
					case 109: rHue = 0xB31; rMainColor = "black"; rName = "a mountain daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 110: rHue = 0x99B; rMainColor = "white"; rName = "a rock daemon"; rFood = "meat"; rDrop = "granite"; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 111: rHue = 0xB32; rMainColor = "black"; rName = "a obsidian daemon"; rFood = "gems_fire"; rDrop = "obsidian"; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 112: rHue = 0x855; rMainColor = "blue"; rName = "a blue daemon"; rFood = "meat"; rDrop = ""; rCategory = "electrical"; rBreath = 13; rPoison = 0; rBlood = ""; break;
					case 113: rHue = 0x959; rMainColor = "red"; rName = "a copper daemon"; rFood = "meat"; rDrop = "copper"; rCategory = "electrical"; rBreath = 13; rPoison = 0; rBlood = ""; break;
					case 114: rHue = 0x952; rMainColor = "red"; rName = "a copperish daemon"; rFood = "meat"; rDrop = "dull copper"; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 115: rHue = 0x797; rMainColor = "yellow"; rName = "a yellow daemon"; rFood = "meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 116: rHue = 0x957; rMainColor = "yellow"; rName = "a earth daemon"; rFood = "gems_meat"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 117: rHue = 0x713; rMainColor = "yellow"; rName = "a desert daemon"; rFood = "meat"; rDrop = ""; rCategory = "sand"; rBreath = 8; rPoison = 0; rBlood = ""; break;
					case 118: rHue = 0x8BC; rMainColor = "yellow"; rName = "a dune daemon"; rFood = "meat"; rDrop = ""; rCategory = "sand"; rBreath = 8; rPoison = 0; rBlood = ""; break;
					case 119: rHue = 0x712; rMainColor = "yellow"; rName = "a sand daemon"; rFood = "meat"; rDrop = ""; rCategory = "sand"; rBreath = 8; rPoison = 0; rBlood = ""; break;
					case 120: rHue = 0x945; rMainColor = "blue"; rName = "a nepturite daemon"; rFood = "fish_sea"; rDrop = "nepturite"; rCategory = "electrical"; rBreath = 13; rPoison = 0; rBlood = ""; break;
					case 121: rHue = 0x8D1; rMainColor = "blue"; rName = "a storm daemon"; rFood = "fish_sea"; rDrop = ""; rCategory = "storm"; rBreath = 46; rPoison = 0; rBlood = ""; break;
					case 122: rHue = 0x8C2; rMainColor = "blue"; rName = "a tide daemon"; rFood = "fish_sea"; rDrop = ""; rCategory = "electrical"; rBreath = 13; rPoison = 0; rBlood = ""; break;
					case 123: rHue = 0xB07; rMainColor = "sea"; rName = "a seastone daemon"; rFood = "fish_sea"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 124: rHue = 0x707; rMainColor = "blue"; rName = "a aqua daemon"; rFood = "fish_sea"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 125: rHue = 0xB3D; rMainColor = "blue"; rName = "a lagoon daemon"; rFood = "fish_sea"; rDrop = ""; rCategory = "poison"; rBreath = 10; rPoison = 0; rBlood = ""; break;
					case 126: rHue = 0x7CD; rMainColor = "blue"; rName = "a loch daemon"; rFood = "fish_sea"; rDrop = ""; rCategory = "poison"; rBreath = 10; rPoison = 0; rBlood = ""; break;
					case 127: rHue = 0xAE9; rMainColor = "green"; rName = "a algae daemon"; rFood = "nox"; rDrop = ""; rCategory = "poison"; rBreath = 10; rPoison = 1; rBlood = ""; break;
					case 128: rHue = 0x854; rMainColor = "yellow"; rName = "a coastal daemon"; rFood = "fish_sea"; rDrop = ""; rCategory = "sand"; rBreath = 8; rPoison = 0; rBlood = ""; break;
					case 129: rHue = 0xB7F; rMainColor = "red"; rName = "a coral daemon"; rFood = "fish_sea"; rDrop = ""; rCategory = "steam"; rBreath = 16; rPoison = 0; rBlood = ""; break;
					case 130: rHue = 0xAFF; rMainColor = "plant"; rName = "a ivy daemon"; rFood = "fish_sea"; rDrop = ""; rCategory = "weed"; rBreath = 34; rPoison = 0; rBlood = ""; break;
					case 131: rHue = 0x860; rMainColor = "ice"; rName = "a glacial daemon"; rFood = "fish"; rDrop = "ice"; rCategory = "cold"; rBreath = 12; rPoison = 0; rBlood = "freezing water"; break;
					case 132: rHue = 0xAF3; rMainColor = "white"; rName = "a ice daemon"; bright = true; rFood = "gems"; rDrop = "ice"; rCategory = "cold"; rBreath = 12; rPoison = 0; rBlood = "freezing water"; break;
					case 133: rHue = 0xB7A; rMainColor = "blue"; rName = "a icehorned daemon"; bright = true; rFood = "gems"; rDrop = "ice"; rCategory = "cold"; rBreath = 12; rPoison = 0; rBlood = "freezing water"; break;
					case 134: rHue = 0xAFD; rMainColor = "white"; rName = "a silver daemon"; rFood = "meat"; rDrop = "silver"; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = "quick silver"; break;
					case 135: rHue = 0x86D; rMainColor = "ice"; rName = "a blizzard daemon"; rFood = "meat"; rDrop = ""; rCategory = "cold"; rBreath = 12; rPoison = 0; rBlood = ""; break;
					case 136: rHue = 0x87D; rMainColor = "white"; rName = "a frost daemon"; rFood = "meat"; rDrop = "ice"; rCategory = "cold"; rBreath = 12; rPoison = 0; rBlood = ""; break;
					case 137: rHue = 0x8BA; rMainColor = "white"; rName = "a snow daemon"; rFood = "meat"; rDrop = ""; rCategory = "cold"; rBreath = 12; rPoison = 0; rBlood = ""; break;
					case 138: rHue = 0x911; rMainColor = "white"; rName = "a white daemon"; rFood = "meat"; rDrop = ""; rCategory = "cold"; rBreath = 12; rPoison = 0; rBlood = ""; break;
					case 139: rHue = 0xAB1; rMainColor = "black"; rName = "a black daemon"; rFood = "meat_nox"; rDrop = ""; rCategory = "poison"; rBreath = 10; rPoison = 0; rBlood = "acidic slime"; break;
					case 140: rHue = 0x88D; rMainColor = "green"; rName = "a mire daemon"; rFood = "nox_fire"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 141: rHue = 0x945; rMainColor = "sea"; rName = "a moor daemon"; rFood = "nox_fire"; rDrop = ""; rCategory = "fire"; rBreath = 9; rPoison = 0; rBlood = ""; break;
					case 142: rHue = 0x8B2; rMainColor = "green"; rName = "a bog daemon"; rFood = "nox"; rDrop = ""; rCategory = "poison"; rBreath = 10; rPoison = 1; rBlood = ""; break;
					case 143: rHue = 0xB27; rMainColor = "green"; rName = "a boghorned daemon"; rFood = "nox"; rDrop = ""; rCategory = "poison"; rBreath = 10; rPoison = 1; rBlood = ""; break;
					case 144: rHue = 0x77D; rMainColor = "green"; rName = "a swampfire daemon"; bright = true; rFood = "meat_nox"; rDrop = ""; rCategory = "poison"; rBreath = 10; rPoison = 0; rBlood = ""; break;
					case 145: rHue = 0x8EC; rMainColor = "green"; rName = "a marsh daemon"; rFood = "meat"; rDrop = ""; rCategory = "weed"; rBreath = 34; rPoison = 1; rBlood = ""; break;
					case 146: rHue = 0x7C7; rMainColor = "green"; rName = "a xormite daemon"; bright = true; rFood = "moon"; rDrop = ""; rCategory = "radiation"; rBreath = 11; rPoison = 0; rBlood = ""; break;
				}

				if ( rCategory == "cold" ){ 			SetDamageType( ResistanceType.Physical, 50 );		SetDamageType( ResistanceType.Fire, 0 );		SetDamageType( ResistanceType.Cold, 50 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 0 ); }
				else if ( rCategory == "electrical" ){ 	SetDamageType( ResistanceType.Physical, 50 );		SetDamageType( ResistanceType.Fire, 0 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 50 ); }
				else if ( rCategory == "fire" ){ 		SetDamageType( ResistanceType.Physical, 50 );		SetDamageType( ResistanceType.Fire, 50 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 0 ); }
				else if ( rCategory == "poison" ){ 		SetDamageType( ResistanceType.Physical, 50 );		SetDamageType( ResistanceType.Fire, 0 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 50 );		SetDamageType( ResistanceType.Energy, 0 ); }
				else if ( rCategory == "radiation" ){ 	SetDamageType( ResistanceType.Physical, 20 );		SetDamageType( ResistanceType.Fire, 40 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 40 ); }
				else if ( rCategory == "sand" ){ 		SetDamageType( ResistanceType.Physical, 80 );		SetDamageType( ResistanceType.Fire, 20 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 0 ); }
				else if ( rCategory == "steam" ){ 		SetDamageType( ResistanceType.Physical, 40 );		SetDamageType( ResistanceType.Fire, 60 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 0 ); }
				else if ( rCategory == "void" ){ 		SetDamageType( ResistanceType.Physical, 20 );		SetDamageType( ResistanceType.Fire, 20 );		SetDamageType( ResistanceType.Cold, 20 );		SetDamageType( ResistanceType.Poison, 20 );		SetDamageType( ResistanceType.Energy, 20 ); }
				else if ( rCategory == "weed" ){ 		SetDamageType( ResistanceType.Physical, 80 );		SetDamageType( ResistanceType.Fire, 0 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 20 );		SetDamageType( ResistanceType.Energy, 0 ); }
				else if ( rCategory == "wind" ){ 		SetDamageType( ResistanceType.Physical, 100 );		SetDamageType( ResistanceType.Fire, 0 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 0 ); }
				else if ( rCategory == "storm" ){ 		SetDamageType( ResistanceType.Physical, 50 );		SetDamageType( ResistanceType.Fire, 0 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 50 ); }
				else if ( rCategory == "star" ){ 		SetDamageType( ResistanceType.Physical, 0 );		SetDamageType( ResistanceType.Fire, 50 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 50 ); }
				else { 									SetDamageType( ResistanceType.Physical, 100 );		SetDamageType( ResistanceType.Fire, 0 );		SetDamageType( ResistanceType.Cold, 0 );		SetDamageType( ResistanceType.Poison, 0 );		SetDamageType( ResistanceType.Energy, 0 ); }

				int phys = 0;
				int fire = 0;
				int cold = 0;
				int pois = 0;
				int engy = 0;

				if ( rCategory == "cold" ){ 			phys = 45;	fire = 5;  cold = 60; pois = 20; engy = 20; }
				else if ( rCategory == "electrical" ){ 	phys = 45;	fire = 20; cold = 20; pois = 5;  engy = 60; }
				else if ( rCategory == "fire" ){ 		phys = 45;	fire = 60; cold = 5;  pois = 20; engy = 20; }
				else if ( rCategory == "poison" ){ 		phys = 45;	fire = 20; cold = 20; pois = 60; engy = 5; }
				else if ( rCategory == "radiation" ){ 	phys = 45;	fire = 25; cold = 5;  pois = 25; engy = 55; }
				else if ( rCategory == "sand" ){ 		phys = 35;	fire = 55; cold = 20; pois = 5;  engy = 30; }
				else if ( rCategory == "steam" ){ 		phys = 45;	fire = 55; cold = 5;  pois = 20; engy = 20; }
				else if ( rCategory == "void" ){ 		phys = 35;	fire = 40; cold = 40; pois = 40; engy = 40; }
				else if ( rCategory == "weed" ){ 		phys = 35;	fire = 15; cold = 15; pois = 55; engy = 40; }

				else if ( rCategory == "wind" ){ 		phys = 35;	fire = 20; cold = 25; pois = 20; engy = 55; }
				else if ( rCategory == "storm" ){ 		phys = 35;	fire = 55; cold = 25; pois = 20; engy = 55; }
				else if ( rCategory == "star" ){ 		phys = 35;	fire = 20; cold = 25; pois = 20; engy = 55; }

				else { 									phys = 45;	fire = 30; cold = 30; pois = 30; engy = 30; }

				Body = rBody;
				Title = rName;
				Hue = rHue;
				YellHue = daemon;

				if ( bright ){ AddItem( new LighterSource() ); }

				SetStr( 596, 625 );
				SetDex( 86, 105 );
				SetInt( 436, 475 );

				SetHits( 578, 595 );

				SetDamage( 12, 18 );

				SetResistance( ResistanceType.Physical, phys, phys );
				SetResistance( ResistanceType.Fire, fire, fire );
				SetResistance( ResistanceType.Cold, cold, cold );
				SetResistance( ResistanceType.Poison, pois, pois );
				SetResistance( ResistanceType.Energy, engy, engy );

				SetSkill( SkillName.Psychology, 70.1, 80.0 );
				SetSkill( SkillName.Magery, 70.1, 80.0 );
				SetSkill( SkillName.MagicResist, 85.1, 95.0 );
				SetSkill( SkillName.Tactics, 70.1, 80.0 );
				SetSkill( SkillName.FistFighting, 60.1, 80.0 );
				if ( rPoison > 0 ){ SetSkill( SkillName.Poisoning, 60.1, 80.0 ); }

				Fame = 15000;
				Karma = -15000;

				VirtualArmor = 58;

				Tamable = true;
				ControlSlots = 3;
				MinTameSkill = 93.9;

				if ( rBreath == 10 || rBreath == 18 ){ 		rBreathPhysDmg = 0;		rBreathFireDmg = 0;		rBreathColdDmg = 0;		rBreathPoisDmg = 100;	rBreathEngyDmg = 0;		rBreathHue = 0x3F;	rBreathSound = 0x658;	rBreathItemID = 0x36D4;	rBreathDelay = 1.3; }
				else if ( rBreath == 11 || rBreath == 39 ){ rBreathPhysDmg = 0;		rBreathFireDmg = 0;		rBreathColdDmg = 0;		rBreathPoisDmg = 50;	rBreathEngyDmg = 50;	rBreathHue = 0x3F;	rBreathSound = 0x227;	rBreathItemID = 0x36D4;	rBreathDelay = 0.1; }
				else if ( rBreath == 24 || rBreath == 27 ){ rBreathPhysDmg = 20;	rBreathFireDmg = 20;	rBreathColdDmg = 20;	rBreathPoisDmg = 20;	rBreathEngyDmg = 20;	rBreathHue = 0x844;	rBreathSound = 0x658;	rBreathItemID = 0x37BC;	rBreathDelay = 0.1; }
				else if ( rBreath == 12 || rBreath == 19 ){ rBreathPhysDmg = 0;		rBreathFireDmg = 0;		rBreathColdDmg = 100;	rBreathPoisDmg = 0;		rBreathEngyDmg = 0;		rBreathHue = 0x481;	rBreathSound = 0x64F;	rBreathItemID = 0x36D4;	rBreathDelay = 1.3; }
				else if ( rBreath == 13 || rBreath == 20 ){ rBreathPhysDmg = 0;		rBreathFireDmg = 0;		rBreathColdDmg = 0;		rBreathPoisDmg = 0;		rBreathEngyDmg = 100;	rBreathHue = 0x9C2;	rBreathSound = 0x665;	rBreathItemID = 0x3818;	rBreathDelay = 1.3; }
				else if ( rBreath == 16 || rBreath == 38 ){ rBreathPhysDmg = 0;		rBreathFireDmg = 100;	rBreathColdDmg = 0;		rBreathPoisDmg = 0;		rBreathEngyDmg = 0;		rBreathHue = 0x9C4;	rBreathSound = 0x108;	rBreathItemID = 0x36D4;	rBreathDelay = 0.1; }
				else if ( rBreath == 25 || rBreath == 28 ){ rBreathPhysDmg = 20;	rBreathFireDmg = 20;	rBreathColdDmg = 20;	rBreathPoisDmg = 20;	rBreathEngyDmg = 20;	rBreathHue = 0x9C1;	rBreathSound = 0x653;	rBreathItemID = 0x37BC;	rBreathDelay = 0.1; }
				else if ( rBreath == 23 || rBreath == 26 ){ rBreathPhysDmg = 20;	rBreathFireDmg = 20;	rBreathColdDmg = 20;	rBreathPoisDmg = 20;	rBreathEngyDmg = 0;		rBreathHue = 0x496;	rBreathSound = 0x658;	rBreathItemID = 0x37BC;	rBreathDelay = 0.1; }
				else if ( rBreath == 34 || rBreath == 35 ){ rBreathPhysDmg = 50;	rBreathFireDmg = 0;		rBreathColdDmg = 0;		rBreathPoisDmg = 50;	rBreathEngyDmg = 0;		rBreathHue = 0;		rBreathSound = 0x56D;	rBreathItemID = Utility.RandomList( 0xCAC, 0xCAD );	rBreathDelay = 0.1; }
				else if ( rBreath == 8 || rBreath == 40 ){ 	rBreathPhysDmg = 50;	rBreathFireDmg = 50;	rBreathColdDmg = 0;		rBreathPoisDmg = 0;		rBreathEngyDmg = 0;		rBreathHue = 0x96D;	rBreathSound = 0x654;	rBreathItemID = 0x36D4;	rBreathDelay = 1.3; }
				else if ( rBreath == 45 || rBreath == 49 ){ rBreathPhysDmg = 0;		rBreathFireDmg = 50;	rBreathColdDmg = 0;		rBreathPoisDmg = 0;		rBreathEngyDmg = 50;	rBreathHue = 0xB72;	rBreathSound = 0x227;	rBreathItemID = 0x1A84;	rBreathDelay = 1.3; }
				else if ( rBreath == 47 || rBreath == 48 ){ rBreathPhysDmg = 100;	rBreathFireDmg = 0;		rBreathColdDmg = 0;		rBreathPoisDmg = 0;		rBreathEngyDmg = 0;		rBreathHue = 0xB24;	rBreathSound = 0x654;	rBreathItemID = 0x5590;	rBreathDelay = 1.3; }
				else if ( rBreath == 46 || rBreath == 50 ){ rBreathPhysDmg = 50;	rBreathFireDmg = 0;		rBreathColdDmg = 0;		rBreathPoisDmg = 0;		rBreathEngyDmg = 50;	rBreathHue = 0x9C2;	rBreathSound = 0x665;	rBreathItemID = 0x3818;	rBreathDelay = 1.3; }
				else { 										rBreathPhysDmg = 0;		rBreathFireDmg = 100;	rBreathColdDmg = 0;		rBreathPoisDmg = 0;		rBreathEngyDmg = 0;		rBreathHue = 0;		rBreathSound = 0x227;	rBreathItemID = 0x36D4;	rBreathDelay = 1.3; }

				InvalidateProperties();
			}
		}
	}
}