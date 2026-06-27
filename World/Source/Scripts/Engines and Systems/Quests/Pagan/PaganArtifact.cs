using Server.Network;
using Server.Gumps;
using Server.Misc;

namespace Server.Items
{
	public class PaganArtifact : Item
	{
		public int PaganItem;
		[CommandProperty(AccessLevel.Owner)]
		public int Pagan_Item { get { return PaganItem; } set { PaganItem = value; InvalidateProperties(); } }

		public int PaganColor;
		[CommandProperty(AccessLevel.Owner)]
		public int Pagan_Color { get { return PaganColor; } set { PaganColor = value; InvalidateProperties(); } }

		public string PaganName;
		[CommandProperty(AccessLevel.Owner)]
		public string Pagan_Name { get { return PaganName; } set { PaganName = value; InvalidateProperties(); } }

		public int PaganPoints;
		[CommandProperty(AccessLevel.Owner)]
		public int Pagan_Points { get { return PaganPoints; } set { PaganPoints = value; InvalidateProperties(); } }

		[Constructable]
		public PaganArtifact( int arty ) : base( 0x05D5 )
		{
			Name = "Pagan Artifact";
			Weight = 10.0;
			ItemID = Utility.RandomList( 0x2823, 0x2824 );
			Hue = Utility.RandomList( 0x993, 0x99E, 0x95B, 0x95C, 0x95D, 0x95E, 0x954 );
			if ( arty < 1 ){ arty = Utility.RandomMinMax( 1, 15 ); }
			if ( PaganItem < 1 ){ SetupArtifactInfo( arty ); }
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1060640 ); // The item must be in your backpack to use it.
			}
			else
			{
				from.SendSound( 0x2D );
				from.CloseGump( typeof( PaganArtifactGump ) );
				from.SendGump( new PaganArtifactGump( from, this ) );
			}
		}

		public void SetupArtifactInfo( int arty )
		{
			int helms = Utility.RandomList( 0x1408, 0x140A, 0x140C, 0x1412, 0x2645, 0x2FBB, 0x140E );
			int color = Utility.RandomList( 0x6FC, 0xB2A, 0xB71, 0xB74 );

			switch ( arty )
			{
				case 1:		PaganItem = helms;		PaganColor = color;	PaganName = "Magic Helm Armour"; 		if ( PaganColor == 0xB74 ){ PaganName = "Ghost Helm Armour"; }		break;
				case 2:		PaganItem = 0x1410;		PaganColor = color;	PaganName = "Magic Arm Armour"; 		if ( PaganColor == 0xB74 ){ PaganName = "Ghost Arm Armour"; }		break;
				case 3:		PaganItem = 0x46AA;		PaganColor = color;	PaganName = "Magic Leg Armour"; 		if ( PaganColor == 0xB74 ){ PaganName = "Ghost Leg Armour"; }		break;
				case 4:		PaganItem = 0x1413;		PaganColor = color;	PaganName = "Magic Neck Armour"; 		if ( PaganColor == 0xB74 ){ PaganName = "Ghost Neck Armour"; }		break;
				case 5:		PaganItem = 0x1414;		PaganColor = color;	PaganName = "Magic Gauntlet Armour"; 	if ( PaganColor == 0xB74 ){ PaganName = "Ghost Gauntlet Armour"; }	break;
				case 6:		PaganItem = 0x1415;		PaganColor = color;	PaganName = "Magic Chest Armour"; 		if ( PaganColor == 0xB74 ){ PaganName = "Ghost Chest Armour"; }		break;

				case 7:		PaganItem = 0x1BC4;		PaganColor = 0x6FC;	PaganName = "Magic Shield"; 			break;
				case 8:		PaganItem = 0x1BC3;		PaganColor = 0xB01;	PaganName = "Daemon Shield"; 			break;

				case 9:		PaganItem = 0x1406;		PaganColor = 0x6FC;	PaganName = "Slayer"; 					break;
				case 10:	PaganItem = 0x1400;		PaganColor = 0xB71;	PaganName = "Flame Sting"; 				break;
				case 11:	PaganItem = 0xF5F;		PaganColor = 0x6FC;	PaganName = "Protector"; 				break;
				case 12:	PaganItem = 0xF60;		PaganColor = 0x6FC;	PaganName = "Blade of Striking"; 		break;
				case 13:	PaganItem = 0x2D2C;		PaganColor = 0xB44;	PaganName = "Korghin's Fang"; 			break;
				case 14:	PaganItem = 0xF5D;		PaganColor = 0xB2A;	PaganName = "Bonecrusher"; 				break;
				case 15:	PaganItem = 0x13FA;		PaganColor = 0x6FC;	PaganName = "Deceiver"; 				break;
				case 16:	PaganItem = 0x13B5;		PaganColor = 0xB2A;	PaganName = "Scimitar of Khumash-Gor"; 	break;
			}
		}

		public PaganArtifact(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
			writer.Write( PaganItem );
			writer.Write( PaganColor );
			writer.Write( PaganName );
			writer.Write( PaganPoints );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			PaganItem = reader.ReadInt();
			PaganColor = reader.ReadInt();
			PaganName = reader.ReadString();
			PaganPoints = reader.ReadInt();
		}

		private class PaganArtifactGump : Gump
		{
			private PaganArtifact m_Artifact;

			public PaganArtifactGump( Mobile from, PaganArtifact arty ) : base( 50, 50 )
			{
				string hue = "#b7765d";
				m_Artifact = arty;

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				AddImage(0, 0, 7012, Server.Misc.PlayerSettings.GetGumpHue( from ));

				AddHtml( 12, 12, 420, 20, @"<BODY><BASEFONT Color=" + hue + ">" + arty.PaganName + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddButton(447, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);

				AddHtml( 14, 44, 460, 219, @"<BODY><BASEFONT Color=" + hue + ">This chest contains a Pagan artifact, " + arty.PaganName + ". If you take it out of this chest, it will be yours. When it is in your possession, single-click the item where a menu will be presented for you to spend points to customize this artifact to your liking. Once the points are spent, the item will be as it is forever. Do you wish to take the Pagan artifact, " + arty.PaganName + ", from the chest?</BASEFONT></BODY>", (bool)false, (bool)false);

				AddButton(10, 275, 4023, 4023, 1, GumpButtonType.Reply, 0);
				AddButton(447, 275, 4020, 4020, 0, GumpButtonType.Reply, 0);

				int color = arty.PaganColor;
				int item = arty.PaganItem;

				int i = 311;
				int o = 47;


				if ( m_Artifact.PaganItem == 0x1408 || m_Artifact.PaganItem == 0x140A || m_Artifact.PaganItem == 0x140C || m_Artifact.PaganItem == 0x1412 || m_Artifact.PaganItem == 0x2645 || m_Artifact.PaganItem == 0x2FBB || m_Artifact.PaganItem == 0x140E ){ AddItem(95+i, 163+0, m_Artifact.PaganItem, color); }
				else if ( m_Artifact.PaganItem == 0x1410 ){ AddItem(97+i, 160+o, 5136, color); }
				else if ( m_Artifact.PaganItem == 0x46AA ){ AddItem(93+i, 158+o, 5137, color); }
				else if ( m_Artifact.PaganItem == 0x1413 ){ AddItem(103+i, 164+o, 5139, color); }
				else if ( m_Artifact.PaganItem == 0x1414 ){ AddItem(98+i, 160+o, 5140, color); }
				else if ( m_Artifact.PaganItem == 0x1415 ){ AddItem(97+i, 155+o, 5141, color); }
				else if ( m_Artifact.PaganName == "Magic Shield" ){ AddItem(93+i, 158+o, 7108, color); }
				else if ( m_Artifact.PaganName == "Daemon Shield" ){ AddItem(97+i, 162+o, 7107, color); }
				else if ( m_Artifact.PaganName == "Slayer" ){ AddItem(101+i, 159+o, 5126, color); }
				else if ( m_Artifact.PaganName == "Flame Sting" ){ AddItem(103+i, 155+o, 5120, color); }
				else if ( m_Artifact.PaganName == "Protector" ){ AddItem(105+i, 162+o, 3935, color); }
				else if ( m_Artifact.PaganName == "Blade of Striking" ){ AddItem(92+i, 155+o, 3936, color); }
				else if ( m_Artifact.PaganName == "Korghin's Fang" ){ AddItem(83+i, 159+o, 11564, color); }
				else if ( m_Artifact.PaganName == "Bonecrusher" ){ AddItem(103+i, 158+o, 3933, color); }
				else if ( m_Artifact.PaganName == "Deceiver" ){ AddItem(104+i, 158+o, 5114, color); }
				else if ( m_Artifact.PaganName == "Scimitar of Khumash-Gor" ){ AddItem(101+i, 160+o, 5045, color); }
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				Mobile from = sender.Mobile;

				if ( m_Artifact.PaganPoints < 1 ){ m_Artifact.PaganPoints = Utility.RandomMinMax( 80, 100 ); }

				from.SendSound( 0x2C );

				if ( info.ButtonID == 1 )
				{
					if ( 
						m_Artifact.PaganItem == 0x1408 || 
						m_Artifact.PaganItem == 0x140A || 
						m_Artifact.PaganItem == 0x140C || 
						m_Artifact.PaganItem == 0x1412 || 
						m_Artifact.PaganItem == 0x2645 || 
						m_Artifact.PaganItem == 0x2FBB || 
						m_Artifact.PaganItem == 0x140E || 
						m_Artifact.PaganItem == 0x1410 || 
						m_Artifact.PaganItem == 0x46AA || 
						m_Artifact.PaganItem == 0x1413 || 
						m_Artifact.PaganItem == 0x1414 || 
						m_Artifact.PaganItem == 0x1415
					)
					{
						BaseGiftArmor armor = new GiftPlateHelm();
						armor.Delete();

						if ( m_Artifact.PaganItem == 0x1410 ){ armor = new GiftPlateArms(); }
						else if ( m_Artifact.PaganItem == 0x46AA ){ armor = new GiftPlateLegs(); }
						else if ( m_Artifact.PaganItem == 0x1413 ){ armor = new GiftPlateGorget(); }
						else if ( m_Artifact.PaganItem == 0x1414 ){ armor = new GiftPlateGloves(); }
						else if ( m_Artifact.PaganItem == 0x1415 ){ armor = new GiftPlateChest(); }
						else { armor = new GiftPlateHelm(); armor.ItemID = m_Artifact.PaganItem; }

						armor.Name = m_Artifact.PaganName;
						armor.Hue = m_Artifact.PaganColor;
        				armor.Owner = from;
        				armor.Gifter = "Artifact from Pagan";
						armor.How = "Found by";
        				armor.Points = m_Artifact.PaganPoints;

						from.AddToBackpack( armor );
						from.SendMessage( "You take possession of the Pagan artifact, " + m_Artifact.PaganName + "!" );
						LoggingFunctions.LogGeneric( from, "has found a Pagan armor artifact." );
					}
					else if ( m_Artifact.PaganItem == 0x1BC4 || m_Artifact.PaganItem == 0x1BC3 )
					{
						BaseGiftShield shield = new GiftOrderShield();
						
						if ( m_Artifact.PaganItem == 0x1BC3 ){ shield.Delete(); shield = new GiftChaosShield(); }

						shield.Name = m_Artifact.PaganName;
						shield.Hue = m_Artifact.PaganColor;
        				shield.Owner = from;
        				shield.Gifter = "Artifact from Pagan";
						shield.How = "Found by";
        				shield.Points = m_Artifact.PaganPoints;

						from.AddToBackpack( shield );
						from.SendMessage( "You take possession of the Pagan artifact, " + m_Artifact.PaganName + "!" );
						LoggingFunctions.LogGeneric( from, "has found a Pagan shield artifact." );
					}
					else if ( m_Artifact.PaganItem == 0x1406 || m_Artifact.PaganItem == 0xF5D )
					{
						BaseGiftBashing mace = new GiftWarMace();
						
						if ( m_Artifact.PaganItem == 0x1BC3 ){ mace.Delete(); mace = new GiftMace(); mace.Slayer = SlayerName.Silver; }
						else
						{
							mace.Slayer = SlayerDeed.GetRandomSlayer();
							mace.Slayer2 = mace.Slayer;
							while ( mace.Slayer == mace.Slayer2 )
							{
								mace.Slayer2 = SlayerDeed.GetRandomSlayer();
							}
						}
						mace.Name = m_Artifact.PaganName;
						mace.Hue = m_Artifact.PaganColor;
        				mace.Owner = from;
        				mace.Gifter = "Artifact from Pagan";
						mace.How = "Found by";
        				mace.Points = m_Artifact.PaganPoints;

						from.AddToBackpack( mace );
						from.SendMessage( "You take possession of the Pagan artifact, " + m_Artifact.PaganName + "!" );
						LoggingFunctions.LogGeneric( from, "has found a Pagan weapon artifact." );
					}
					else if ( m_Artifact.PaganItem == 0x2D2C )
					{
						BaseGiftKnife dagger = new GiftDagger();

						dagger.ItemID = m_Artifact.PaganItem;
						dagger.Name = m_Artifact.PaganName;
						dagger.Hue = m_Artifact.PaganColor;
        				dagger.Owner = from;
        				dagger.Gifter = "Artifact from Pagan";
						dagger.How = "Found by";
        				dagger.Points = m_Artifact.PaganPoints;
						dagger.SkillBonuses.SetValues( 0, SkillName.Poisoning, Utility.RandomMinMax( 15, 30 ) );

						from.AddToBackpack( dagger );
						from.SendMessage( "You take possession of the Pagan artifact, " + m_Artifact.PaganName + "!" );
						LoggingFunctions.LogGeneric( from, "has found a Pagan weapon artifact." );
					}
					else if ( m_Artifact.PaganItem == 0x13FA )
					{
						BaseGiftAxe axe = new GiftLargeBattleAxe();

						axe.ItemID = m_Artifact.PaganItem;
						axe.Name = m_Artifact.PaganName;
						axe.Hue = m_Artifact.PaganColor;
        				axe.Owner = from;
        				axe.Gifter = "Artifact from Pagan";
						axe.How = "Found by";
        				axe.Points = m_Artifact.PaganPoints;
						axe.SkillBonuses.SetValues( 0, SkillName.Swords, Utility.RandomMinMax( 15, 30 ) );

						from.AddToBackpack( axe );
						from.SendMessage( "You take possession of the Pagan artifact, " + m_Artifact.PaganName + "!" );
						LoggingFunctions.LogGeneric( from, "has found a Pagan weapon artifact." );
					}
					else
					{
						BaseGiftSword sword = new GiftLongsword();
						sword.Delete();

						if ( m_Artifact.PaganItem == 0x1400 ){ sword = new GiftKryss(); sword.Slayer = SlayerName.FlameDousing; if ( Utility.RandomMinMax( 1, 4 ) == 1 ){ sword.Slayer = SlayerName.Exorcism; } }
						else if ( m_Artifact.PaganItem == 0xF5F ){ sword = new GiftBroadsword(); sword.SkillBonuses.SetValues( 0, SkillName.MagicResist, Utility.RandomMinMax( 15, 30 ) ); }
						else if ( m_Artifact.PaganItem == 0xF60 ){ sword = new GiftLongsword(); sword.SkillBonuses.SetValues( 0, SkillName.Tactics, Utility.RandomMinMax( 15, 30 ) ); }
						else { sword = new GiftScimitar(); sword.Slayer = SlayerName.Silver; }

						sword.Name = m_Artifact.PaganName;
						sword.Hue = m_Artifact.PaganColor;
        				sword.Owner = from;
        				sword.Gifter = "Artifact from Pagan";
						sword.How = "Found by";
        				sword.Points = m_Artifact.PaganPoints;

						from.AddToBackpack( sword );
						from.SendMessage( "You take possession of the Pagan artifact, " + m_Artifact.PaganName + "!" );
						LoggingFunctions.LogGeneric( from, "has found a Pagan weapon artifact." );
					}

					m_Artifact.Delete();
				}
			}
		}
	}
}