using Server.Misc;
using Server.Network;
using Server.Gumps;
using Server.Mobiles;
using Server.Utilities;

namespace Server.Items
{
	public class DracolichSkull : Item
	{
		[Constructable]
		public DracolichSkull() : base( 0x3DCC )
		{
			ItemID = Utility.RandomList( 0x3DCC, 0x3DCD );
			Hue = 0xB70;
			Weight = 4.0;
			Name = "Mysterious Dragon Skull";

			if ( Weight > 3.0 )
			{
				Weight = 3.0;

				HavePotionA = 0;
				HavePotionB = 0;
				HavePotionC = 0;
				HavePotionD = 0;
				HaveGold = 0;
				NeedGold = 100000;

				PieceRumor = Server.Items.CubeOnCorpse.GetRumor();
				PieceLocation = Server.Items.CubeOnCorpse.PickDungeon();
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( Weight > 2.0 && from.Map == Map.Lodor && from.X >= 6082 && from.Y >= 175 && from.X <= 6089 && from.Y <= 182 )
			{
				Weight = 1.0;
				Name = "Enchanted Dragon Skull";
				from.PlaySound( 0x0F6 );
			}

			if ( Weight < 1.5 )
			{
				from.CloseGump( typeof( DracolichSkullGump ) );
				from.SendGump( new DracolichSkullGump( from, this ) );
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{          		
			string sEnd = ".";

			if ( from != null && Weight < 1.5 )
			{
				if ( dropped is Gold && NeedGold > HaveGold )
				{
					var iAmount = ItemUtilities.ConsumeClamped(dropped, NeedGold - HaveGold);

					if ( iAmount > 1 ){ sEnd = "s."; }

					HaveGold += iAmount;
					from.SendMessage( "You added " + iAmount.ToString() + " gold coin" + sEnd );

					return dropped.Deleted;
				}
			}

			return false;
		}

		public DracolichSkull( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 ); // version

			writer.Write( HavePotionA );
			writer.Write( HavePotionB );
			writer.Write( HavePotionC );
			writer.Write( HavePotionD );
			writer.Write( HaveGold );
			writer.Write( NeedGold );
			writer.Write( PieceLocation );
			writer.Write( PieceRumor );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			HavePotionA = reader.ReadInt();
			HavePotionB = reader.ReadInt();
			HavePotionC = reader.ReadInt();
			HavePotionD = reader.ReadInt();
			HaveGold = reader.ReadInt();
			NeedGold = reader.ReadInt();
			PieceLocation = reader.ReadString();
			PieceRumor = reader.ReadString();
		}

		public static bool ProcessDracolichSkull( Mobile m, Mobile necro, Item dropped )
		{
			DracolichSkull skull = (DracolichSkull)dropped;

			int HaveIngredients = 0;

			if ( skull.HavePotionB >= 0 ){ HaveIngredients++; }
			if ( skull.HavePotionC >= 0 ){ HaveIngredients++; }
			if ( skull.HavePotionD >= 0 ){ HaveIngredients++; }
			if ( skull.HaveGold >= skull.NeedGold ){ HaveIngredients++; }
			if ( skull.HavePotionA >= 0 ){ HaveIngredients++; }

			if ( HaveIngredients < 5 ){ return false; }

			if ( (m.Followers + 3) > m.FollowersMax )
			{
				necro.Say( "You have too many followers with you right now." );
				return false;
			}
			else if ( m.Skills[SkillName.Necromancy].Base < 100 )
			{
				necro.Say( "You must be a pure grandmaster necromancer if you want my help." );
				return false;
			}

			BaseCreature dragon = new SkeletonDragon();
			dragon.OnAfterSpawn();
			dragon.Controlled = true;
			dragon.ControlMaster = m;
			dragon.IsBonded = true;
			dragon.MoveToWorld( m.Location, m.Map );
			dragon.ControlTarget = m;
			dragon.Tamable = true;
			dragon.MinTameSkill = 29.1;
			dragon.ControlOrder = OrderType.Follow;

			LoggingFunctions.LogGenericQuest( m, "has reanimated a skeletal dragon" );
			m.PrivateOverheadMessage(MessageType.Regular, 1153, false, "Your skeletal dragon has reanimated.", m.NetState);

			m.PlaySound( 0x488 );

			dropped.Delete();

			return true;
		}

		public class DracolichSkullGump : Gump
		{
			public DracolichSkullGump( Mobile from, DracolichSkull skull ): base( 50, 50 )
			{
				string color = "#cacaca";
				from.SendSound( 0x4A ); 

				string sText = "<BR><BR><BR><BR><BR><BR><BR>This skull contains the essence of an undead dragon. Necromancers would take these skulls and combine it with four rare artifacts of the dead. The heart of the dead god, the eye of the mad king, the orb of the astral lich, and the mind of the planar ghost are the four relics used in this process. These are usually in long lost dungeons or ruins, and said to be last seen in small chests resting on a runic pedestal. Once these items are merged with the skull, it could be used to reanimate the bones of a long dead dragon to serve the necromancer. You have heard many rumors of these items being seen in various places. If you could get them, and bring the skull to a powerful necromancer, they may be able to help you perform the ritual needed. The necromancer will require some gold (placed onto the skull) as payment for such a spell. When reanimated, these creatures will become your bonded servant. You will have to feed it moon crystals and stable it when required. You can also perform some druidism on it without having any proficiency in the skill. This will help you with information about them. If you later feed them a ruby, they will change their skeletal form of a different style of dragon.";

				string sRumor = skull.PieceRumor + " " + skull.PieceLocation;

				if ( skull.HavePotionA == 0 ){ sRumor = "The heart of the dead god " + sRumor; }
				else if ( skull.HavePotionB == 0 ){ sRumor = "The eye of the mad king " + sRumor; }
				else if ( skull.HavePotionC == 0 ){ sRumor = "The orb of the astral lich " + sRumor; }
				else if ( skull.HavePotionD == 0 ){ sRumor = "The mind of the planar ghost " + sRumor; }
				else if ( skull.HaveGold < skull.NeedGold ){ sRumor = "You have obtained everything except the gold."; }
				else { sRumor = "You have obtained everything you need."; }

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);


				AddImage(0, 0, 7014, Server.Misc.PlayerSettings.GetGumpHue( from ));

				AddHtml( 12, 12, 420, 20, @"<BODY><BASEFONT Color=" + color + ">ENCHANTED DRAGON SKULL</BASEFONT></BODY>", (bool)false, (bool)false);

				AddButton(863, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);

				AddHtml( 601, 13, 173, 20, @"<BODY><BASEFONT Color=" + color + ">Gold: " + skull.HaveGold.ToString() + "/" + skull.NeedGold.ToString() + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 12, 43, 874, 20, @"<BODY><BASEFONT Color=" + color + ">Bring the Gathered Materials to the Black Magic Guildmaster</BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 12, 76, 878, 364, @"<BODY><BASEFONT Color=" + color + ">" + sText + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 12, 452, 874, 20, @"<BODY><BASEFONT Color=" + color + ">" + sRumor + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddItem(586, 506, 7978, 0xB5E);
				AddItem(486, 506, 7978, 0xB5E);
				AddItem(386, 506, 7978, 0xB5E);
				AddItem(286, 506, 7978, 0xB5E);

				if ( skull.HavePotionA > 0 ){ AddItem(291, 500, 3985, 0xB1F); } // HEART
				if ( skull.HavePotionB > 0 ){ AddItem(386, 506, 11418, 0xB71); } // EYE
				if ( skull.HavePotionC > 0 ){ AddItem(488, 501, 11396, 0xB51); } // ORB
				if ( skull.HavePotionD > 0 ){ AddItem(585, 504, 7408, 0xB3E); } // BRAIN
			}

			public override void OnResponse(NetState state, RelayInfo info)
			{
				Mobile from = state.Mobile;
				from.SendSound( 0x4A ); 
			}
		}

		public string PieceLocation;
		[CommandProperty( AccessLevel.GameMaster )]
		public string g_PieceLocation { get{ return PieceLocation; } set{ PieceLocation = value; } }

		public string PieceRumor;
		[CommandProperty( AccessLevel.GameMaster )]
		public string g_PieceRumor { get{ return PieceRumor; } set{ PieceRumor = value; } }

		// ----------------------------------------------------------------------------------------

		public int NeedGold;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_NeedGold { get{ return NeedGold; } set{ NeedGold = value; } }

		// ----------------------------------------------------------------------------------------

		public int HavePotionA;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HavePotionA { get{ return HavePotionA; } set{ HavePotionA = value; } }

		public int HaveGold;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HaveGold { get{ return HaveGold; } set{ HaveGold = value; } }

		public int HavePotionC;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HavePotionC { get{ return HavePotionC; } set{ HavePotionC = value; } }

		public int HavePotionB;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HavePotionB { get{ return HavePotionB; } set{ HavePotionB = value; } }

		public int HavePotionD;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_HavePotionD { get{ return HavePotionD; } set{ HavePotionD = value; } }
	}
}