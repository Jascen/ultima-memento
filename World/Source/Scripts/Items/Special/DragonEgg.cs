using Server.Misc;
using Server.Network;
using Server.Gumps;
using Server.Mobiles;
using Server.Utilities;

namespace Server.Items
{
	public class DragonEgg : Item
	{
		[Constructable]
		public DragonEgg() : base( 0x278C )
		{
			Weight = 4.0;
			Name = "Dragon Egg";
			Light = LightType.Circle225;

			if ( Weight > 3.0 )
			{
				Weight = 3.0;

				HavePotionA = 0;
				HavePotionB = 0;
				HavePotionC = 0;
				HavePotionD = 0;
				HaveGold = 0;

				AnimalTrainerLocation = Server.Items.AlienEgg.GetRandomVet();

				PieceRumor = Server.Items.CubeOnCorpse.GetRumor();
				PieceLocation = Server.Items.CubeOnCorpse.PickDungeon();
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( Weight > 2.0 && from.Map == Map.Lodor && from.X >= 5296 && from.Y >= 664 && from.X <= 5318 && from.Y <= 686 )
			{
				Weight = 1.0;
			}

			if ( Weight < 1.5 )
			{
				from.CloseGump( typeof( DragonEggGump ) );
				from.SendGump( new DragonEggGump( from, this ) );
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

		public DragonEgg( Serial serial ) : base( serial )
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
			writer.Write( AnimalTrainerLocation );
			writer.Write( PieceLocation );
			writer.Write( PieceRumor );
			writer.Write( DragonType );
			writer.Write( DragonBody );
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
			AnimalTrainerLocation = reader.ReadString();
			PieceLocation = reader.ReadString();
			PieceRumor = reader.ReadString();
			DragonType = reader.ReadInt();
			DragonBody = reader.ReadInt();
		}

		public static bool ProcessDragonEgg( Mobile m, Mobile vet, Item dropped )
		{
			DragonEgg egg = (DragonEgg)dropped;

			if ( Server.Misc.Worlds.GetRegionName( vet.Map, vet.Location ) != egg.AnimalTrainerLocation ){ return false; }

			int vetSkill = (int)(m.Skills[SkillName.Veterinary].Value);
				if ( vetSkill > 100 ){ vetSkill = 100; }

			int GoldReturn = 0;
				if ( vetSkill > 0 ){ GoldReturn = (int)( egg.NeedGold * ( vetSkill * 0.005 ) ); }

			int HaveIngredients = 0;

			if ( egg.HavePotionB >= 0 ){ HaveIngredients++; }
			if ( egg.HavePotionC >= 0 ){ HaveIngredients++; }
			if ( egg.HavePotionD >= 0 ){ HaveIngredients++; }
			if ( egg.HaveGold >= egg.NeedGold ){ HaveIngredients++; }
			if ( egg.HavePotionA >= 0 ){ HaveIngredients++; }

			if ( HaveIngredients < 5 ){ return false; }

			int followers = 3;
			if ( (dropped.Name).Contains(" dragon") ){ followers = 2; }

			if ( (m.Followers + followers) > m.FollowersMax )
			{
				vet.Say( "You have too many followers with you to hatch this egg." );
				return false;
			}

			if ( GoldReturn > 0 ){ m.AddToBackpack( new Gold( GoldReturn ) ); vet.Say( "Here is " + GoldReturn.ToString() + " gold back for all of your help." ); }

			BaseCreature dragon = new RidingDragon( "a dragon", egg.DragonBody, egg.DragonType );
			dragon.OnAfterSpawn();
			dragon.Controlled = true;
			dragon.ControlMaster = m;
			dragon.IsBonded = true;
			dragon.MoveToWorld( m.Location, m.Map );
			dragon.ControlTarget = m;
			dragon.Tamable = true;
			dragon.MinTameSkill = 29.1;
			dragon.ControlOrder = OrderType.Follow;

			string style = "dragon";
			if ( followers == 3 ){ style = "wyrm"; dragon.Name = (dragon.Name).Replace(" dragon", " wyrm"); }

			LoggingFunctions.LogGenericQuest( m, "has hatched a " + style + "" );
			m.PrivateOverheadMessage(MessageType.Regular, 1153, false, "Your " + style + " has hatched.", m.NetState);

			m.PlaySound( 0x041 );

			dropped.Delete();

			return true;
		}

		public class DragonEggGump : Gump
		{
			public DragonEggGump( Mobile from, DragonEgg egg ): base( 50, 50 )
			{
				from.SendSound( 0x4A ); 
				string color = "#94d3b4";
				string sDragon = "dragon";
					if ( egg.DragonBody == 59 ){ sDragon = "wyrm"; }

				string sText = "This egg contains the embryo of a " + sDragon + ". Dwarves would take these eggs and brew the potions of the four elements to pour over the shell. The elixir of the flame, the potion of the earth, the mixture of the sea, and the oil of the winds are the four alchemical potions used in this process. Once these liquids are poured onto the shell, it could be broken by the young " + sDragon + " and the power of all the elements combined would mature the " + sDragon + " to almost be fully grown. These alchemical skills died off with the dwarven race, but you did hear rumors of these potions being seen in various places. These are usually in long lost dungeons or ruins, and said to be last seen in small chests resting on a runic pedestal. If you could get them, and bring the egg to an animal expert, they may be able to help you hatch it. The animal expert will require some gold (placed onto the egg) as you will need the help of a particular animal expert and they will require payment for their services. This animal expert is at the location shown below. If you have any veterinary skill, they may refund some of the gold for the help you may provide in the birth. When hatched and almost fully grown, these " + sDragon + "s will become your bonded pet. You will have to feed it and stable it when required. You can also perform some druidism on it without having any proficiency in the skill. This will help you with information about them, like what they want to eat.";

				string sRumor = egg.PieceRumor + " " + egg.PieceLocation;

				if ( egg.HavePotionA == 0 ){ sRumor = "The elixir of the flame " + sRumor; }
				else if ( egg.HavePotionB == 0 ){ sRumor = "The potion of the earth " + sRumor; }
				else if ( egg.HavePotionC == 0 ){ sRumor = "The mixture of the sea " + sRumor; }
				else if ( egg.HavePotionD == 0 ){ sRumor = "The oil of the winds " + sRumor; }
				else if ( egg.HaveGold < egg.NeedGold ){ sRumor = "You have obtained everything except the gold."; }
				else { sRumor = "You have obtained everything you need."; }

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				AddImage(0, 0, 7015, Server.Misc.PlayerSettings.GetGumpHue( from ));
				AddHtml( 12, 12, 420, 20, @"<BODY><BASEFONT Color=" + color + ">" + (egg.Name).ToUpper() + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddButton(863, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);

				AddHtml( 12, 40, 173, 20, @"<BODY><BASEFONT Color=" + color + ">Gold: " + egg.HaveGold.ToString() + "/" + egg.NeedGold.ToString() + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 12, 70, 874, 20, @"<BODY><BASEFONT Color=" + color + ">Bring Gathered Materials to the Animal Expert in " + egg.AnimalTrainerLocation + "</BASEFONT></BODY>", (bool)false, (bool)false);
				AddHtml( 12, 100, 874, 20, @"<BODY><BASEFONT Color=" + color + ">" + sRumor + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddHtml( 12, 339, 878, 251, @"<BODY><BASEFONT Color=" + color + ">" + sText + "</BASEFONT></BODY>", (bool)false, (bool)false);

				AddItem(708, 130, 11665, egg.Hue);

				AddItem(93, 210, 13042);
				AddItem(273, 210, 13042);
				AddItem(453, 210, 13042);
				AddItem(633, 210, 13042);

				if ( egg.HavePotionA > 0 ){ AddItem(105, 210, 10279, 0xB54); }
				if ( egg.HavePotionB > 0 ){ AddItem(285, 210, 10279, 0xB27); }
				if ( egg.HavePotionC > 0 ){ AddItem(465, 210, 10279, 0xB46); }
				if ( egg.HavePotionD > 0 ){ AddItem(645, 210, 10279, 0xB49); }
			}

			public override void OnResponse(NetState state, RelayInfo info)
			{
				Mobile from = state.Mobile;
				from.SendSound( 0x4A ); 
			}
		}

		public string AnimalTrainerLocation;
		[CommandProperty( AccessLevel.GameMaster )]
		public string g_AnimalTrainerLocation { get{ return AnimalTrainerLocation; } set{ AnimalTrainerLocation = value; } }

		public string PieceLocation;
		[CommandProperty( AccessLevel.GameMaster )]
		public string g_PieceLocation { get{ return PieceLocation; } set{ PieceLocation = value; } }

		public string PieceRumor;
		[CommandProperty( AccessLevel.GameMaster )]
		public string g_PieceRumor { get{ return PieceRumor; } set{ PieceRumor = value; } }

		public int DragonType;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_DragonType { get{ return DragonType; } set{ DragonType = value; } }

		public int DragonBody;
		[CommandProperty( AccessLevel.GameMaster )]
		public int g_DragonBody { get{ return DragonBody; } set{ DragonBody = value; } }

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