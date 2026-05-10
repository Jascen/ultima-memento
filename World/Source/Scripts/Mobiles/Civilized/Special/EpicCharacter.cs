using System;
using System.Collections;
using Server.ContextMenus;
using System.Collections.Generic;
using Server.Misc;
using Server.Network;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Commands;
using System.Globalization;

namespace Server.Mobiles
{
	public class EpicCharacter : BasePerson
	{
		public override bool InitialInnocent{ get{ return true; } }
		public static Mobile m_Mobile;
		public static Mobile m_Giver;
		public static bool m_Pay;

		public string MyAlignment;
		public string MyItemText;
		public int MyItemHue;
		public int MyItemPower;
		public Map MyWorld;
		public int MyX;
		public int MyY;

		[CommandProperty(AccessLevel.Owner)]
		public string My_Alignment { get { return MyAlignment; } set { MyAlignment = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public string My_ItemText { get { return MyItemText; } set { MyItemText = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int My_ItemHue { get { return MyItemHue; } set { MyItemHue = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int My_ItemPower { get { return MyItemPower; } set { MyItemPower = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public Map My_World { get { return MyWorld; } set { MyWorld = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int My_X { get { return MyX; } set { MyX = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int My_Y { get { return MyY; } set { MyY = value; InvalidateProperties(); } }

		[Constructable]
		public EpicCharacter() : base( )
		{
			SpeechHue = Utility.RandomTalkHue();
			NameHue = 0xB0C;
			CantWalk = true;
			Name = "a stranger";
			Body = 400;
			MyItemText = "";
			MyItemHue = 0;
			MyItemPower = 150;
			MyAlignment = "neutral";
			AI = AIType.AI_Citizen;
			FightMode = FightMode.None;

			SetStr( 200 );
			SetDex( 200 );
			SetInt( 200 );

			SetDamage( 15, 20 );
			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 25, 30 );
			SetResistance( ResistanceType.Cold, 25, 30 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.FistFighting, 100 );
			VirtualArmor = 100;
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list ) 
		{ 
			base.GetContextMenuEntries( from, list );
			if ( !from.Blessed )
			{
				list.Add( new SpeechGumpEntry( from, this ) );
				list.Add( new GiftGumpEntry( from, this, true ) );
			}
		}

		public override bool OnBeforeDeath()
		{
			Say("In Vas Mani");
			this.Hits = this.HitsMax;
			this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
			this.PlaySound( 0x202 );
			return false;
		}

		public override bool IsEnemy( Mobile m )
		{
			return false;
		}

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( !m.Frozen && m is PlayerMobile && InRange( m, 6 ) && this.Name == "Lord British" && m.Karma >= 0 && this.CanSee( m ) && this.InLOS( m ) )
			{
				if ( m.Hits < m.HitsMax && m is PlayerMobile )
				{
					OfferHeal( (PlayerMobile) m );
				}
			}
		}

		public virtual void OfferHeal( PlayerMobile m )
		{
			if ( m.CheckYoungHealTime() )
			{
				Say( "You look as though you have some wounds." );
				m.PlaySound( 0x1F2 );
				m.FixedEffect( 0x376A, 9, 32 );
				m.Hits = m.HitsMax;
			}
			else
			{
				Say( "Sorry, but I am tired and cannot heal you now." );
			}
		}

		public static void SetSpecialItemRequirement( Mobile m )
		{
			var player = (PlayerMobile)m;

			string epicName = player.Quests.EpicQuestName;
			int epicNumber = player.Quests.EpicQuestNumber;

			if ( epicName == "NEW" || epicName == "" || epicName == null )
			{
				int choice = Utility.RandomMinMax( 1, 59 );

				string KeepTrack = "_" + epicNumber.ToString() + "_";

				while ( Server.Items.SummonPrison.UsedNumberCheck( KeepTrack, choice ) == true ){ choice = Utility.RandomMinMax( 1, 59 ); }
					player.Quests.EpicQuestName = Server.Items.SummonPrison.GetItemNeeded( choice, 3 );
					player.Quests.EpicQuestNumber = choice;
			}
		}

		public static string GetSpecialItemRequirement( Mobile m )
		{
			return ((PlayerMobile)m).Quests.EpicQuestName;
		}

		public static void ClearSpecialItemRequirement( Mobile m )
		{
			string rare = ((PlayerMobile)m).Quests.EpicQuestName;

			ArrayList targets = new ArrayList();
			foreach ( Item item in World.Items.Values )
			if ( item is SummonItems && item.Name == rare )
			{
				if ( ((SummonItems)item).owner == m )
					targets.Add( item );
			}
			for ( int i = 0; i < targets.Count; ++i )
			{
				Item item = ( Item )targets[ i ];
				item.Delete();
			}
			((PlayerMobile)m).Quests.EpicQuestName = "NEW";
		}

		public static bool HaveSpecialItemRequirement( Mobile m )
		{
			string item = ((PlayerMobile)m).Quests.EpicQuestName;

			bool HasItem = false;

			if ( m != null && m.Backpack != null )
			{
				List<Item> list = new List<Item>();
				(m.Backpack).RecurseItems( list );
				foreach ( Item i in list )
				{
					if ( i is SummonItems && i.Name == item && ((SummonItems)i).Owner == m )
						HasItem = true;
				}
			}

			return HasItem;
		}

		public class SpeechGumpEntry : ContextMenuEntry
		{
			public SpeechGumpEntry( Mobile from, Mobile giver ) : base( 6146, 3 )
			{
				m_Mobile = from;
				m_Giver = giver;
			}

			public override void OnClick()
			{
			    if( !( m_Mobile is PlayerMobile ) )
				return;
				
				bool PassTest = false;

				SetSpecialItemRequirement( m_Mobile );

				if ( ((EpicCharacter)m_Giver).MyAlignment == "good" ){ if ( m_Mobile.Fame >= 4000 && m_Mobile.Karma >= 4000 ){ PassTest = true; } }
				else if ( ((EpicCharacter)m_Giver).MyAlignment == "evil" ){ if ( m_Mobile.Fame >= 4000 && m_Mobile.Karma <= -4000 ){ PassTest = true; } }
				else if ( ((EpicCharacter)m_Giver).MyAlignment == "neutral" ){ if ( m_Mobile.Fame >= 7000 ){ PassTest = true; } }

				PlayerMobile mobile = (PlayerMobile) m_Mobile;

				if ( ! mobile.HasGump( typeof( EpicGump ) ) )
				{
					mobile.SendGump( new EpicGump( m_Giver, m_Mobile, PassTest, ((EpicCharacter)m_Giver).MyAlignment ) );
				}
            }
        }

		public class GiftGumpEntry : ContextMenuEntry
		{
			public GiftGumpEntry( Mobile from, Mobile giver, bool pay ) : base( 6163, 3 )
			{
				m_Mobile = from;
				m_Giver = giver;
				m_Pay = pay;
			}

			public override void OnClick()
			{
			    if( !( m_Mobile is PlayerMobile ) )
				return;

				bool PassTest = false;

				SetSpecialItemRequirement( m_Mobile );

				string merit = "bravery";

				if ( ((EpicCharacter)m_Giver).MyAlignment == "good" && m_Pay ){ if ( m_Mobile.Fame >= 4000 && m_Mobile.Karma >= 4000 ){ PassTest = true; } merit = "valor"; }
				else if ( ((EpicCharacter)m_Giver).MyAlignment == "evil" && m_Pay ){ if ( m_Mobile.Fame >= 4000 && m_Mobile.Karma <= -4000 ){ PassTest = true; } merit = "tenacity"; }
				else if ( ((EpicCharacter)m_Giver).MyAlignment == "neutral" && m_Pay ){ if ( m_Mobile.Fame >= 7000 ){ PassTest = true; } }

				if ( m_Mobile.TotalGold < 5000 && m_Pay )
				{
					m_Mobile.SendMessage( m_Giver.Name + " needs at least 5,000 gold to construct the item for you.");
				}
				else if ( !(HaveSpecialItemRequirement( m_Mobile )) && m_Pay )
				{
					m_Mobile.SendMessage( m_Giver.Name + " will need the a symbol of your " + merit + " (" + GetSpecialItemRequirement( m_Mobile ) + ").");
				}
				else if ( PassTest == true || !m_Pay )
				{
					PlayerMobile mobile = (PlayerMobile) m_Mobile;
					{
						if ( ! mobile.HasGump( typeof( EpicBookGump ) ) )
						{
							mobile.SendGump( new EpicBookGump( m_Mobile, m_Giver, 0, m_Pay ) );
						}
					}
				}
				else
				{
					m_Mobile.SendMessage( "Your deeds do not grant you a gift of tribute.");
				}
            }
        }

		public override void OnAfterSpawn()
		{
			if ( this.X == 798 && this.Y == 1095 && this.Map == Map.SerpentIsland )
			{
				this.Body = Utility.RandomList(427,191); 
				this.Hue = 0;
				this.Name = "Lord Draxinusom";
				this.Title = "the Gargoyle King";
				this.MyAlignment = "neutral";
				this.Direction = Direction.East;
				this.MyItemText = "of the Gargoyles";
				this.MyItemHue = 0x846;
				this.MyWorld = this.Map;
				this.MyX = 882;
				this.MyY = 1134;
			}
			else if ( this.X == 2425 && this.Y == 627 && this.Map == Map.SerpentIsland )
			{
				this.Body = 21;
				this.Hue = 0x83F;
				this.Name = "the Great Earth Serpent";
				this.MyAlignment = "neutral";
				this.Direction = Direction.East;
				this.MyItemText = "of Balance";
				this.MyItemHue = 0x978;
				this.MyWorld = this.Map;
				this.MyX = 1453;
				this.MyY = 835;
			}
			else if ( this.X == 126 && this.Y == 2942 && this.Map == Map.SavagedEmpire )
			{
				this.Body = 24;
				this.Hue = 0x83B;
				this.Name = "Morphius";
				this.Title = "the Vile Lich";
				this.MyAlignment = "evil";
				this.Direction = Direction.South;
				this.MyItemText = "of the Necrotic";
				this.MyItemHue = 0xB9A;
				this.MyWorld = this.Map;
				this.MyX = 223;
				this.MyY = 1361;
			}
			else if ( this.X == 63 && this.Y == 2893 && this.Map == Map.SavagedEmpire )
			{
				this.Body = 400; 
				this.Hue = 0x83EA;
				this.FacialHairItemID = 0x204D;
				this.HairItemID = 0;
				this.FacialHairHue = 0x497;
				this.HairHue = 0x497;

				AddItem( new Boots() );
				Item cloth1 = new Robe();
					cloth1.Hue = 0x497;
					this.AddItem( cloth1 );

				this.Name = "Mondain";
				this.Title = "the Wizard";
				this.MyAlignment = "evil";
				this.Direction = Direction.South;
				this.MyItemText = "of Mondain";
				this.MyItemHue = 0x497;
				this.MyWorld = this.Map;
				this.MyX = 1128;
				this.MyY = 22;
			}
			else if ( this.X == 6067 && this.Y == 344 && this.Map == Map.Lodor )
			{
				this.Body = 400; 
				this.Hue = 0x83EA;
				this.FacialHairItemID = 0x2041;
				this.HairItemID = 0x203C;
				this.FacialHairHue = 0x497;
				this.HairHue = 0x497;

				AddItem( new Boots() );
				Item cloth1 = new Robe();
					cloth1.Hue = 0x845;
					this.AddItem( cloth1 );

				Item cloth2 = new WizardsHat();
					cloth2.Hue = 0x845;
					this.AddItem( cloth2 );

				this.Name = "Tyball";
				this.Title = "the Demonologist";
				this.MyAlignment = "evil";
				this.Direction = Direction.South;
				this.MyItemText = "of Demonic Souls";
				this.MyItemHue = 0x54D;
				this.MyWorld = this.Map;
				this.MyX = 1637;
				this.MyY = 2804;
			}
			else if ( this.X == 5415 && this.Y == 1160 && this.Map == Map.Lodor )
			{
				this.Body = 9; 
				this.Hue = 0x845;
				this.Name = "Arcadion";
				this.Title = "the Daemon";
				this.MyAlignment = "evil";
				this.Direction = Direction.South;
				this.MyItemText = "of Purgatory";
				this.MyItemHue = 0x550;
				this.MyWorld = this.Map;
				this.MyX = 3196;
				this.MyY = 3318;
			}
			else if ( this.X == 2142 && this.Y == 2754 && this.Map == Map.Lodor )
			{
				this.Body = 400; 
				this.Hue = 0x83EA;
				this.FacialHairItemID = 0;
				this.HairItemID = 0x203C;
				this.FacialHairHue = 0x8A5;
				this.HairHue = 0x8A5;

				AddItem( new Boots() );
				Item cloth1 = new FancyShirt();
					cloth1.Hue = Utility.RandomBlueHue();
					this.AddItem( cloth1 );

				Item cloth2 = new LongPants();
					cloth2.Hue = Utility.RandomNeutralHue();
					this.AddItem( cloth2 );

				this.Name = "Samhayne";
				this.Title = "the Master Sailor";
				this.MyAlignment = "good";
				this.Direction = Direction.East;
				this.MyItemText = "of Poseidon";
				this.MyItemHue = 0x542;
				this.MyWorld = this.Map;
				this.MyX = this.X;
				this.MyY = this.Y;
			}
			else if ( this.X == 5035 && this.Y == 3830 && this.Map == Map.Sosaria )
			{
				this.Body = 400; 
				this.Hue = 0x83EA;
				this.FacialHairItemID = 0x2041;
				this.HairItemID = 0x203B;
				this.FacialHairHue = 0x908;
				this.HairHue = 0x908;

				AddItem( new Boots() );
				Item cloth1 = new FancyShirt();
					cloth1.Hue = Utility.RandomYellowHue();
					this.AddItem( cloth1 );

				Item cloth2 = new LongPants();
					cloth2.Hue = Utility.RandomNeutralHue();
					this.AddItem( cloth2 );

				Item cloth3 = new TricorneHat();
					cloth3.Hue = Utility.RandomNeutralHue();
					this.AddItem( cloth3 );

				this.Name = "Seggallion";
				this.Title = "the Pirate Lord";
				this.MyAlignment = "evil";
				this.Direction = Direction.East;
				this.MyItemText = "of the Buccaneer";
				this.MyItemHue = 0x549;
				this.MyWorld = this.Map;
				this.MyX = 1878;
				this.MyY = 2215;
			}
			else if ( this.X == 4755 && this.Y == 3978 && this.Map == Map.Sosaria )
			{
				this.Body = 401; 
				this.Hue = 0x83EA;
				this.FacialHairItemID = 0;
				this.HairItemID = 0x203C;
				this.FacialHairHue = 0x497;
				this.HairHue = 0x497;

				AddItem( new Boots() );
				Item cloth1 = new FancyDress();
					cloth1.Hue = 0x497;
					this.AddItem( cloth1 );

				this.Name = "Minax";
				this.Title = "the Enchantress";
				this.MyAlignment = "evil";
				this.Direction = Direction.East;
				this.MyItemText = "of Minax";
				this.MyItemHue = 0x497;
				this.MyWorld = this.Map;
				this.MyX = 3832;
				this.MyY = 1494;
			}
			else if ( this.X == 3011 && this.Y == 951 && this.Map == Map.Sosaria )
			{
				this.Body = 400; 
				this.Hue = 0x83EA;
				this.FacialHairItemID = 0x204B;
				this.HairItemID = 0x203C;
				this.FacialHairHue = 0x370;
				this.HairHue = 0x370;

				AddItem( new Boots() );
				Item cloth1 = new Robe();
					cloth1.Hue = 0x907;
					this.AddItem( cloth1 );

				Item cloth2 = new WizardsHat();
					cloth2.Hue = 0x907;
					this.AddItem( cloth2 );

				this.Name = "Nystal";
				this.Title = "the Royal Wizard";
				this.MyAlignment = "good";
				this.Direction = Direction.East;
				this.MyItemText = "of Wizardry";
				this.MyItemHue = 0x48B;
				this.MyWorld = this.Map;
				this.MyX = this.X;
				this.MyY = this.Y;
			}
			else if ( this.X == 2990 && this.Y == 902 && this.Map == Map.Sosaria )
			{
				this.Body = 400; 
				this.Hue = 0x83EA;
				this.FacialHairItemID = 0x204B;
				this.HairItemID = 0x203C;
				this.FacialHairHue = 0x906;
				this.HairHue = 0x906;

				AddItem( new Boots() );

				Item cloth1 = new Robe();
					cloth1.Hue = 0xA47;
					this.AddItem( cloth1 );

				Item cloth2 = new JewelryCirclet();
					cloth2.Name = "royal crown";
					cloth2.Hue = 0x8A5;
					this.AddItem( cloth2 );

				Item cloth3 = new FancyShirt();
					cloth3.Hue = 0xA20;
					this.AddItem( cloth3 );

				this.Name = "Lord British";
				this.Title = "the King of Britain";
				this.MyAlignment = "good";
				this.Direction = Direction.South;
				this.MyItemText = "of Sosaria";
				this.MyItemHue = 0x430;
				this.MyWorld = this.Map;
				this.MyX = this.X;
				this.MyY = this.Y;
			}
			else if ( this.X == 6732 && this.Y == 1663 && this.Map == Map.Sosaria )
			{
				this.Body = 400; 

				AddItem( new Server.Items.Boots() );
				AddItem( new LordBlackthorneSuit()); 

				this.Name = "Lord Blackthorne";
				this.Title = "the Ruler of Kuldar";
				this.MyAlignment = "evil";
				this.Direction = Direction.East;
				this.MyItemText = "of Blackthorne";
				this.MyItemHue = 0x966;
				this.MyWorld = this.Map;
				this.MyX = this.X;
				this.MyY = this.Y;
			}
			else if ( this.X == 3025 && this.Y == 962 && this.Map == Map.Sosaria )
			{
				this.Body = 400; 
				this.Hue = 0x83EA;
				this.FacialHairItemID = 0x2041;
				this.HairItemID = 0;
				this.FacialHairHue = 0x45C;
				this.HairHue = 0x45C;

				AddItem( new Boots() );

				Item cloth1 = new Cloak();
					cloth1.Hue = Utility.RandomBlueHue();
					this.AddItem( cloth1 );

				Item cloth2 = new ChainChest();
					cloth2.Hue = 0x430;
					this.AddItem( cloth2 );

				Item cloth3 = new RingmailArms();
					cloth3.Hue = 0x430;
					this.AddItem( cloth3 );

				Item cloth4 = new ChainLegs();
					cloth4.Hue = 0x430;
					this.AddItem( cloth4 );

				Item cloth5 = new RingmailGloves();
					cloth5.Hue = 0x430;
					this.AddItem( cloth5 );

				Item cloth6 = new ChainCoif();
					cloth6.Hue = 0x430;
					this.AddItem( cloth6 );

				Item cloth7 = new OrderShield();
					cloth7.Hue = 0x430;
					this.AddItem( cloth7 );

				this.AddItem( new Longsword() );

				this.Name = "Geoffrey";
				this.Title = "the Knight";
				this.MyAlignment = "good";
				this.Direction = Direction.West;
				this.MyItemText = "of the Warrior";
				this.MyItemHue = 0;
				this.MyWorld = this.Map;
				this.MyX = this.X;
				this.MyY = this.Y;
			}
			else if ( this.X == 5615 && this.Y == 2888 && this.Map == Map.Sosaria )
			{
				this.Body = 400; 
				this.Hue = 0x83EA;
				this.FacialHairItemID = 0x2041;
				this.HairItemID = 0;
				this.FacialHairHue = 0x455;
				this.HairHue = 0x455;

				int ronin = 0x972;

				SamuraiTabi cloth1 = new SamuraiTabi( );
					cloth1.Hue = ronin;
					this.AddItem( cloth1 );

				LeatherHiroSode cloth2 = new LeatherHiroSode( );
					cloth2.Hue = ronin;
					this.AddItem( cloth2 );

				LeatherDo cloth3 = new LeatherDo( );
					cloth3.Hue = ronin;
					this.AddItem( cloth3 );

				Item glove = new LeatherGloves();
					glove.Hue = ronin;
					this.AddItem( glove );

				switch ( Utility.Random( 4 ) )
				{
					case 0: LightPlateJingasa cloth4 = new LightPlateJingasa( );	cloth4.Hue = ronin;	this.AddItem( cloth4 ); break;
					case 1: ChainHatsuburi cloth5 = new ChainHatsuburi( );	cloth5.Hue = ronin;	this.AddItem( cloth5 ); break;
					case 2: DecorativePlateKabuto cloth6 = new DecorativePlateKabuto( );	cloth6.Hue = ronin;	this.AddItem( cloth6 ); break;
					case 3: LeatherJingasa cloth7 = new LeatherJingasa( );	cloth7.Hue = ronin;	this.AddItem( cloth7 ); break;
				}

				switch ( Utility.Random( 3 ) )
				{
					case 0: StuddedHaidate cloth8 = new StuddedHaidate( );	cloth8.Hue = ronin;	this.AddItem( cloth8 ); break;
					case 1: LeatherSuneate cloth9 = new LeatherSuneate( );	cloth9.Hue = ronin;	this.AddItem( cloth9 ); break;
					case 2: PlateSuneate cloth0 = new PlateSuneate( );	cloth0.Hue = ronin;	this.AddItem( cloth0 ); break;
				}

				this.Name = "Shimazu";
				this.Title = "the Shogun Samurai";
				this.MyAlignment = "neutral";
				this.Direction = Direction.East;
				this.MyItemText = "of the Shogun";
				this.MyItemHue = 0;
				this.MyWorld = this.Map;
				this.MyX = 1328;
				this.MyY = 3589;
			}
			else if ( this.X == 313 && this.Y == 1040 && this.Map == Map.IslesDread )
			{
				this.Body = 0x190;
            	this.Hue = Utility.RandomSkinColor();

				this.FacialHairItemID = 0x204C; // BEARD
				this.HairItemID = 0x203C; // LONG HAIR
				this.FacialHairHue = 0x455;
				this.HairHue = 0x455;

				AddItem( new StuddedChest() );
				AddItem( new StuddedLegs() );
				AddItem( new Boots() );
				Item cloth2 = new JewelryCirclet();
					cloth2.Name = "circlet";
					cloth2.Hue = 0;
					this.AddItem( cloth2 );
				AddItem( new Cloak( Utility.RandomYellowHue() ) );
				AddItem( new StuddedGloves() );

				this.Name = "Gorn";
            	this.Title = "the King of Cimmeran";
				this.MyAlignment = "neutral";
				this.Direction = Direction.East;
				this.MyItemText = "of the Barbarian";
				this.MyItemHue = 0x972;
				this.MyWorld = this.Map;
				this.MyX = this.X;
				this.MyY = this.Y;
			}
			else if ( this.X == 2462 && this.Y == 865 && this.Map == Map.Sosaria )
			{
				this.Body = 401; 
				this.Hue = 0x83EA;
				this.FacialHairItemID = 0;
				this.HairItemID = 0x203C;
				this.FacialHairHue = 0x8A5;
				this.HairHue = 0x8A5;

				AddItem( new Boots() );
				Item cloth1 = new Robe();
					cloth1.Hue = 0x907;
					this.AddItem( cloth1 );

				this.Name = "Jaana";
				this.Title = "the Herb Healer";
				this.MyAlignment = "good";
				this.Direction = Direction.South;
				this.MyItemText = "of the Cleric";
				this.MyItemHue = 0x47E;
				this.MyWorld = this.Map;
				this.MyX = this.X;
				this.MyY = this.Y;
			}
			else if ( this.X == 1395 && this.Y == 3778 && this.Map == Map.Sosaria )
			{
				this.Name = "Dupre";
				this.MyAlignment = "good";
				this.Direction = Direction.South;
				this.Body = 400;

				AddItem( new Server.Items.Boots() );
				AddItem( new DupreSuit());

				this.Title = "the Paladin";
				this.MyItemText = "of the Paladin";
				this.MyItemHue = 0x430;
				this.MyWorld = this.Map;
				this.MyX = this.X;
				this.MyY = this.Y;
			}
			else if ( this.X == 2119 && this.Y == 247 && this.Map == Map.Sosaria )
			{
				this.Name = "Gwenno";
				this.MyAlignment = "good";
				this.Direction = Direction.South;

				this.Body = 401; 
				this.Hue = Utility.RandomSkinColor();
				this.HairItemID = 0x203C;
				this.HairHue = 0x45C;

				AddItem( new Boots() );
				Item cloth1 = new FancyDress();
					cloth1.Hue = 0x96F;
					this.AddItem( cloth1 );

				this.Title = "the Bard";
				this.MyItemText = "of the Minstrel";
				this.MyItemHue = 0;
				this.MyWorld = this.Map;
				this.MyX = this.X;
				this.MyY = this.Y;
			}
			else if ( this.X == 937 && this.Y == 2081 && this.Map == Map.Sosaria )
			{
				this.Name = "Iolo";
				this.MyAlignment = "good";
				this.Direction = Direction.South;

				this.Body = 0x190;
            	this.Hue = Utility.RandomSkinColor();

				this.FacialHairItemID = 0x204C;
				this.HairItemID = 0x2048;
				this.FacialHairHue = 0x430;
				this.HairHue = 0x430;

				AddItem( new FancyShirt( Utility.RandomYellowHue() ) );
				AddItem( new LongPants( Utility.RandomYellowHue() ) );
				AddItem( new Boots() );
				AddItem( new Crossbow() );
				AddItem( new Cloak( Utility.RandomYellowHue() ) );
				AddItem( new LeatherGloves() );

            	this.Title = "the Bowman";
				this.MyItemText = "of the Archer";
				this.MyItemHue = 0;
				this.MyWorld = this.Map;
				this.MyX = this.X;
				this.MyY = this.Y;
			}
			else if ( this.X == 3263 && this.Y == 2582 && this.Map == Map.Sosaria )
			{
				this.Name = "Shamino";
				this.MyAlignment = "good";
				this.Direction = Direction.East;

				this.Body = 0x190;
            	this.Hue = Utility.RandomSkinColor();

				this.FacialHairItemID = 0x2041;
				this.HairItemID = 0x203B;
				this.FacialHairHue = 0x8A5;
				this.HairHue = 0x8A5;

				AddItem( new FancyShirt( Utility.RandomYellowHue() ) );
				AddItem( new LongPants( Utility.RandomYellowHue() ) );
				AddItem( new Boots() );
				AddItem( new Hatchet() );
				AddItem( new Cloak( Utility.RandomRedHue() ) );
				AddItem( new LeatherGloves() );

            	this.Title = "the Woodsman";
				this.MyItemText = "of the Woodlands";
				this.MyItemHue = 0x840;
				this.MyWorld = this.Map;
				this.MyX = this.X;
				this.MyY = this.Y;
			}
			else if ( this.X == 3441 && this.Y == 3190 && this.Map == Map.Sosaria )
			{
				this.Name = "Stefano";
				this.MyAlignment = "neutral";
				this.Direction = Direction.West;

				this.Body = 0x190;
            	this.Hue = Utility.RandomSkinColor();

				this.FacialHairItemID = 0;
				this.HairItemID = 0x203C;
				this.FacialHairHue = 0x840;
				this.HairHue = 0x840;

				AddItem( new FancyShirt( Utility.RandomBlueHue() ) );
				AddItem( new LongPants( Utility.RandomBlueHue() ) );
				AddItem( new Boots() );
				AddItem( new Cloak( Utility.RandomBlueHue() ) );
				AddItem( new LeatherGloves() );

            	this.Title = "the Sneak";
				this.MyItemText = "of the Thief";
				this.MyItemHue = 0x83A;
				this.MyWorld = this.Map;
				this.MyX = 3317;
				this.MyY = 2064;
			}
			else if ( this.X == 1604 && this.Y == 1604 && this.Map == Map.Sosaria )
			{
				this.Body = 401; 
				this.Hue = 0x83EA;
				this.FacialHairItemID = 0;
				this.HairItemID = 0x2049;
				this.FacialHairHue = 0x5E3;
				this.HairHue = 0x5E3;

				AddItem( new Sandals() );
				Item cloth1 = new Skirt();
					cloth1.Hue = 0x907;
					this.AddItem( cloth1 );
				Item cloth2 = new FancyShirt();
					cloth2.Hue = 0x907;
					this.AddItem( cloth2 );

				this.Name = "Katrina";
				this.Title = "the Shepherd";
				this.MyAlignment = "good";
				this.Direction = Direction.East;
				this.MyItemText = "of the Beastmaster";
				this.MyItemHue = 0x840;
				this.MyWorld = this.Map;
				this.MyX = this.X;
				this.MyY = this.Y;
			}
			else if ( this.X == 4993 && this.Y == 3997 && this.Map == Map.Sosaria )
			{
				this.Body = 485; 
				this.Hue = 1461;
				this.Name = "the Guardian";
				this.MyAlignment = "evil";
				this.Direction = Direction.East;
				this.MyItemText = "of Pagan";
				this.MyItemHue = 1461;
				this.MyWorld = this.Map;
				this.MyX = 877;
				this.MyY = 2654;
			}
			else if ( this.X == 5033 && this.Y == 3750 && this.Map == Map.Sosaria )
			{
				this.Body = 400; 
				this.Hue = 0x83EA;
				this.FacialHairItemID = 0x204C;
				this.HairItemID = 0x203C;
				this.FacialHairHue = 0x370;
				this.HairHue = 0x370;

				AddItem( new Boots() );
				Item cloth1 = new Robe();
					cloth1.Hue = Utility.RandomBlueHue();
					this.AddItem( cloth1 );

				Item cloth2 = new WizardsHat();
					cloth2.Hue = Utility.RandomBlueHue();
					this.AddItem( cloth2 );

				this.Name = "Garamon";
				this.Title = "the Wizard";
				this.MyAlignment = "good";
				this.Direction = Direction.South;
				this.MyItemText = "of the Alchemist";
				this.MyItemHue = 0x6DF;
				this.MyWorld = this.Map;
				this.MyX = 6003;
				this.MyY = 3679;
			}
			else if ( this.X == 2648 && this.Y == 3306 && this.Map == Map.Sosaria )
			{
				this.Body = 401; 
				this.Hue = 0x83EA;
				this.FacialHairItemID = 0;
				this.HairItemID = 0x203D;
				this.FacialHairHue = 0x497;
				this.HairHue = 0x497;

				AddItem( new Boots() );

				Item cloth1 = new Cloak();
					cloth1.Hue = 0x497;
					this.AddItem( cloth1 );

				Item cloth2 = new ChainChest();
					cloth2.Hue = 0x963;
					this.AddItem( cloth2 );

				Item cloth3 = new RingmailArms();
					cloth3.Hue = 0x963;
					this.AddItem( cloth3 );

				Item cloth4 = new ChainLegs();
					cloth4.Hue = 0x963;
					this.AddItem( cloth4 );

				Item cloth5 = new RingmailGloves();
					cloth5.Hue = 0x963;
					this.AddItem( cloth5 );

				Item cloth6 = new NorseHelm();
					cloth6.Hue = 0x963;
					this.AddItem( cloth6 );

				Item cloth7 = new ChaosShield();
					cloth7.Hue = 0x497;
					this.AddItem( cloth7 );

				this.Name = "Mors Gotha";
				this.Title = "the Death Knight";
				this.MyAlignment = "evil";
				this.Direction = Direction.South;
				this.MyItemText = "of Death";
				this.MyItemHue = 0x963;
				this.MyWorld = this.Map;
				this.MyX = 3370;
				this.MyY = 1552;
			}
			else if ( this.X == 4136 && this.Y == 3424 && this.Map == Map.Sosaria )
			{
				this.Body = 24;
				this.Hue = 0x83B;
				this.Name = "Lethe";
				this.Title = "the Dreaded Lich";
				this.MyAlignment = "evil";
				this.Direction = Direction.South;
				this.MyItemText = "of the Undertaker";
				this.MyItemHue = 0x837;
				this.MyWorld = this.Map;
				this.MyX = 1528;
				this.MyY = 3599;
			}
		}

		public EpicCharacter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
            writer.Write( MyAlignment );
		    writer.Write( MyItemText );
		    writer.Write( MyItemHue );
		    writer.Write( MyItemPower );
		    writer.Write( MyWorld );
		    writer.Write( MyX );
		    writer.Write( MyY );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
            MyAlignment = reader.ReadString();
			MyItemText = reader.ReadString();
			MyItemHue = reader.ReadInt();
		    MyItemPower = reader.ReadInt();
			MyWorld = reader.ReadMap();
			MyX = reader.ReadInt();
			MyY = reader.ReadInt();
		}

		public class EpicBookGump : Gump
		{
			public EpicBookGump( Mobile from, Mobile giver, int page, bool pay ): base( 100, 100 )
			{
				string color = "#cfc990";
				m_Mobile = from;
				m_Giver = giver;
				m_Pay = pay;

				int NumberOfArtifacts = 291;	// SEE LISTING BELOW AND MAKE SURE IT MATCHES THE AMOUNT
												// DO THIS NUMBER+1 IN THE OnResponse SECTION BELOW

				decimal PageCount = NumberOfArtifacts / 16;
				int TotalBookPages = ( 100000 ) + ( (int)Math.Ceiling( PageCount ) );

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				AddPage(0);

				int subItem = page * 16;

				int showItem1 = subItem + 1;
				int showItem2 = subItem + 2;
				int showItem3 = subItem + 3;
				int showItem4 = subItem + 4;
				int showItem5 = subItem + 5;
				int showItem6 = subItem + 6;
				int showItem7 = subItem + 7;
				int showItem8 = subItem + 8;
				int showItem9 = subItem + 9;
				int showItem10 = subItem + 10;
				int showItem11 = subItem + 11;
				int showItem12 = subItem + 12;
				int showItem13 = subItem + 13;
				int showItem14 = subItem + 14;
				int showItem15 = subItem + 15;
				int showItem16 = subItem + 16;

				int page_prev = ( 100000 + page ) - 1;
					if ( page_prev < 100000 ){ page_prev = TotalBookPages; }
				int page_next = ( 100000 + page ) + 1;
					if ( page_next > TotalBookPages ){ page_next = 100000; }

				AddImage(0, 0, 7055, Server.Misc.PlayerSettings.GetGumpHue( from ));

				AddButton(668, 9, 4017, 4017, page_prev, GumpButtonType.Reply, 0);

				AddHtml( 61, 12, 579, 20, @"<BODY><BASEFONT Color=" + color + "><CENTER>TRIBUTE GIFTS</CENTER></BASEFONT></BODY>", (bool)false, (bool)false);

				AddButton(9, 425, 4014, 4014, page_prev, GumpButtonType.Reply, 0);
				AddButton(668, 425, 4005, 4005, page_next, GumpButtonType.Reply, 0);

				int x = 83;
				int y = 84;
				int s = 84;
				int z = 34;

				y=y+z;
				if ( Server.Items.ManualOfItems.GetRelicArtyForBook( showItem1, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem1, GumpButtonType.Reply, 0); } y=y+z;
				if ( Server.Items.ManualOfItems.GetRelicArtyForBook( showItem2, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem2, GumpButtonType.Reply, 0); } y=y+z;
				if ( Server.Items.ManualOfItems.GetRelicArtyForBook( showItem3, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem3, GumpButtonType.Reply, 0); } y=y+z;
				if ( Server.Items.ManualOfItems.GetRelicArtyForBook( showItem4, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem4, GumpButtonType.Reply, 0); } y=y+z;
				if ( Server.Items.ManualOfItems.GetRelicArtyForBook( showItem5, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem5, GumpButtonType.Reply, 0); } y=y+z;
				if ( Server.Items.ManualOfItems.GetRelicArtyForBook( showItem6, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem6, GumpButtonType.Reply, 0); } y=y+z;
				if ( Server.Items.ManualOfItems.GetRelicArtyForBook( showItem7, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem7, GumpButtonType.Reply, 0); } y=y+z;
				if ( Server.Items.ManualOfItems.GetRelicArtyForBook( showItem8, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem8, GumpButtonType.Reply, 0); } y=s-3;
				y=y+z;
				AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.ManualOfItems.GetRelicArtyForBook( showItem1, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
				AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.ManualOfItems.GetRelicArtyForBook( showItem2, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
				AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.ManualOfItems.GetRelicArtyForBook( showItem3, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
				AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.ManualOfItems.GetRelicArtyForBook( showItem4, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
				AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.ManualOfItems.GetRelicArtyForBook( showItem5, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
				AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.ManualOfItems.GetRelicArtyForBook( showItem6, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
				AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.ManualOfItems.GetRelicArtyForBook( showItem7, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
				AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.ManualOfItems.GetRelicArtyForBook( showItem8, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=s-3;

				///////////////////////////////////////////////////////////////////////////////////

				x = 375;
				y = s;

				y=y+z;
				if ( Server.Items.ManualOfItems.GetRelicArtyForBook( showItem9, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem9, GumpButtonType.Reply, 0); } y=y+z;
				if ( Server.Items.ManualOfItems.GetRelicArtyForBook( showItem10, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem10, GumpButtonType.Reply, 0); } y=y+z;
				if ( Server.Items.ManualOfItems.GetRelicArtyForBook( showItem11, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem11, GumpButtonType.Reply, 0); } y=y+z;
				if ( Server.Items.ManualOfItems.GetRelicArtyForBook( showItem12, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem12, GumpButtonType.Reply, 0); } y=y+z;
				if ( Server.Items.ManualOfItems.GetRelicArtyForBook( showItem13, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem13, GumpButtonType.Reply, 0); } y=y+z;
				if ( Server.Items.ManualOfItems.GetRelicArtyForBook( showItem14, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem14, GumpButtonType.Reply, 0); } y=y+z;
				if ( Server.Items.ManualOfItems.GetRelicArtyForBook( showItem15, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem15, GumpButtonType.Reply, 0); } y=y+z;
				if ( Server.Items.ManualOfItems.GetRelicArtyForBook( showItem16, 1 ) != "" ){ AddButton(x, y, 2447, 2447, showItem16, GumpButtonType.Reply, 0); } y=s-3;
				y=y+z;
				AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.ManualOfItems.GetRelicArtyForBook( showItem9, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
				AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.ManualOfItems.GetRelicArtyForBook( showItem10, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
				AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.ManualOfItems.GetRelicArtyForBook( showItem11, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
				AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.ManualOfItems.GetRelicArtyForBook( showItem12, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
				AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.ManualOfItems.GetRelicArtyForBook( showItem13, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
				AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.ManualOfItems.GetRelicArtyForBook( showItem14, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
				AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.ManualOfItems.GetRelicArtyForBook( showItem15, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=y+z;
				AddHtml( x+20, y, 155, 20, @"<BODY><BASEFONT Color=" + color + ">" + Server.Items.ManualOfItems.GetRelicArtyForBook( showItem16, 1 ) + "</BASEFONT></BODY>", (bool)false, (bool)false); y=s-3;
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				EpicCharacter tribute = (EpicCharacter)m_Giver;
				Mobile from = state.Mobile; 

				bool passTest = false;

				string merit = "bravery";
				if ( tribute.MyAlignment == "good" ){ merit = "valor"; }
				else if ( tribute.MyAlignment == "evil" ){ merit = "tenacity"; }

				if ( tribute.MyAlignment == "good" && m_Pay && info.ButtonID > 0 && info.ButtonID < 300 && from.TotalGold >= 5000 && HaveSpecialItemRequirement( from ) )
				{
					if ( from.Fame >= 4000 && from.Karma >= 4000 )
					{
						from.Fame = from.Fame - 4000;
						from.Karma = from.Karma - 4000;
						passTest = true;
					}
				}
				else if ( tribute.MyAlignment == "evil" && m_Pay && info.ButtonID > 0 && info.ButtonID < 300 && from.TotalGold >= 5000 && HaveSpecialItemRequirement( from ) )
				{
					if ( from.Fame >= 4000 && from.Karma <= -4000 )
					{
						from.Fame = from.Fame - 4000;
						from.Karma = from.Karma + 4000;
						passTest = true;
					}
				}
				else if ( tribute.MyAlignment == "neutral" && m_Pay && info.ButtonID > 0 && info.ButtonID < 300 && from.TotalGold >= 5000 && HaveSpecialItemRequirement( from ) )
				{
					if ( from.Fame >= 7000 )
					{
						from.Fame = from.Fame - 7000;
						passTest = true;
					}
				}

				from.SendSound( 0x55 );
                Container pack = from.Backpack;

				if ( info.ButtonID >= 100000 )
				{
					int page = info.ButtonID - 100000;
					from.SendGump( new EpicBookGump( from, m_Giver, page, m_Pay ) );
				}
				else if ( from.TotalGold < 5000 && m_Pay )
				{
					from.SendMessage( m_Giver.Name + " needs at least 5,000 gold to construct the item for you.");
				}
				else if ( !(HaveSpecialItemRequirement( from )) && m_Pay )
				{
					from.SendMessage( m_Giver.Name + " will need the a symbol of your " + merit + " (" + GetSpecialItemRequirement( from ) + ").");
				}
				else if ( ( passTest == true && pack.ConsumeTotal(typeof(Gold), 5000) ) || !m_Pay )
				{
					ClearSpecialItemRequirement( from );

					string sType = Server.Items.ManualOfItems.GetRelicArtyForBook( info.ButtonID, 2 );
					string sName = Server.Items.ManualOfItems.GetRelicArtyForBook( info.ButtonID, 1 );
					string sArty = sName;
						if ( sArty == "Talisman, Holy" ){ sArty = "Talisman"; }
						if ( sArty == "Talisman, Snake" ){ sArty = "Talisman"; }
						if ( sArty == "Talisman, Totem" ){ sArty = "Talisman"; }
						sArty = sArty + " " + tribute.MyItemText;

					if ( sName != "" )
					{
						Item reward = null;
						Type itemType = ScriptCompiler.FindTypeByName( sType );
						reward = (Item)Activator.CreateInstance(itemType);

						int points = tribute.MyItemPower;

						if ( reward is BaseGiftAxe )
						{
							((BaseGiftAxe)reward).m_Owner = from;
							((BaseGiftAxe)reward).m_Gifter = "From " + m_Giver.Name + " " + m_Giver.Title;
							((BaseGiftAxe)reward).m_How = "Tribute To";
							((BaseGiftAxe)reward).m_Points = points;
						}
						if ( reward is BaseGiftRanged )
						{
							((BaseGiftRanged)reward).m_Owner = from;
							((BaseGiftRanged)reward).m_Gifter = "From " + m_Giver.Name + " " + m_Giver.Title;
							((BaseGiftRanged)reward).m_How = "Tribute To";
							((BaseGiftRanged)reward).m_Points = points;
						}
						if ( reward is BaseGiftSpear )
						{
							((BaseGiftSpear)reward).m_Owner = from;
							((BaseGiftSpear)reward).m_Gifter = "From " + m_Giver.Name + " " + m_Giver.Title;
							((BaseGiftSpear)reward).m_How = "Tribute To";
							((BaseGiftSpear)reward).m_Points = points;
						}
						if ( reward is BaseGiftClothing )
						{
							((BaseGiftClothing)reward).m_Owner = from;
							((BaseGiftClothing)reward).m_Gifter = "From " + m_Giver.Name + " " + m_Giver.Title;
							((BaseGiftClothing)reward).m_How = "Tribute To";
							((BaseGiftClothing)reward).m_Points = points;
						}
						if ( reward is BaseGiftJewel )
						{
							((BaseGiftJewel)reward).m_Owner = from;
							((BaseGiftJewel)reward).m_Gifter = "From " + m_Giver.Name + " " + m_Giver.Title;
							((BaseGiftJewel)reward).m_How = "Tribute To";
							((BaseGiftJewel)reward).m_Points = points;
						}
						if ( reward is BaseGiftArmor )
						{
							((BaseGiftArmor)reward).m_Owner = from;
							((BaseGiftArmor)reward).m_Gifter = "From " + m_Giver.Name + " " + m_Giver.Title;
							((BaseGiftArmor)reward).m_How = "Tribute To";
							((BaseGiftArmor)reward).m_Points = points;
						}
						if ( reward is BaseGiftShield )
						{
							((BaseGiftShield)reward).m_Owner = from;
							((BaseGiftShield)reward).m_Gifter = "From " + m_Giver.Name + " " + m_Giver.Title;
							((BaseGiftShield)reward).m_How = "Tribute To";
							((BaseGiftShield)reward).m_Points = points;
						}
						if ( reward is BaseGiftKnife )
						{
							((BaseGiftKnife)reward).m_Owner = from;
							((BaseGiftKnife)reward).m_Gifter = "From " + m_Giver.Name + " " + m_Giver.Title;
							((BaseGiftKnife)reward).m_How = "Tribute To";
							((BaseGiftKnife)reward).m_Points = points;
						}
						if ( reward is BaseGiftBashing )
						{
							((BaseGiftBashing)reward).m_Owner = from;
							((BaseGiftBashing)reward).m_Gifter = "From " + m_Giver.Name + " " + m_Giver.Title;
							((BaseGiftBashing)reward).m_How = "Tribute To";
							((BaseGiftBashing)reward).m_Points = points;
						}
						if ( reward is BaseGiftWhip )
						{
							((BaseGiftWhip)reward).m_Owner = from;
							((BaseGiftWhip)reward).m_Gifter = "From " + m_Giver.Name + " " + m_Giver.Title;
							((BaseGiftWhip)reward).m_How = "Tribute To";
							((BaseGiftWhip)reward).m_Points = points;
						}
						if ( reward is BaseGiftPoleArm )
						{
							((BaseGiftPoleArm)reward).m_Owner = from;
							((BaseGiftPoleArm)reward).m_Gifter = "From " + m_Giver.Name + " " + m_Giver.Title;
							((BaseGiftPoleArm)reward).m_How = "Tribute To";
							((BaseGiftPoleArm)reward).m_Points = points;
						}
						if ( reward is BaseGiftStaff )
						{
							((BaseGiftStaff)reward).m_Owner = from;
							((BaseGiftStaff)reward).m_Gifter = "From " + m_Giver.Name + " " + m_Giver.Title;
							((BaseGiftStaff)reward).m_How = "Tribute To";
							((BaseGiftStaff)reward).m_Points = points;
						}
						if ( reward is BaseGiftSword )
						{
							((BaseGiftSword)reward).m_Owner = from;
							((BaseGiftSword)reward).m_Gifter = "From " + m_Giver.Name + " " + m_Giver.Title;
							((BaseGiftSword)reward).m_How = "Tribute To";
							((BaseGiftSword)reward).m_Points = points;
						}

						reward.Name = sArty;
						reward.Hue = tribute.MyItemHue;

						AddToItem( reward, m_Giver );

						from.AddToBackpack ( reward );

						string sEntry = "has received the " + sArty + " from " + m_Giver.Name + " " + m_Giver.Title;
						string sMessage = "You have received the " + sArty + " from " + m_Giver.Name + ".";

						LoggingFunctions.LogGenericQuest( from, sEntry );

						from.SendMessage( sMessage );
						from.PlaySound( 0x3D );
					}
				}
				else if ( passTest == false && info.ButtonID > 0 && info.ButtonID < 262 )
				{
					from.SendMessage( "Your deeds do not grant you a gift of tribute.");
				}
			}
		}

		public static void AddToItem( Item item, Mobile from )
		{
			if ( from.Name == "Lord Draxinusom" )
			{
				GiveGiftBonus( item, 6, 8, 34, 0, 0, 8.0, 8.0, 8.0, 0.0, 0.0, 23, 15 );
			}
			else if ( from.Name == "the Great Earth Serpent" )
			{
				GiveGiftBonus( item, 99, 32, 0, 0, 0, 10.0, 10.0, 0.0, 0.0, 0.0, 8, 15 );
			}
			else if ( from.Name == "Morphius" )
			{
				GiveGiftBonus( item, 36, 33, 44, 0, 0, 8.0, 8.0, 8.0, 0.0, 0.0, 35, 0 );
			}
			else if ( from.Name == "Mondain" )
			{
				GiveGiftBonus( item, 17, 31, 33, 0, 0, 8.0, 8.0, 8.0, 0.0, 0.0, 0, 0 );
			}
			else if ( from.Name == "Tyball" )
			{
				GiveGiftBonus( item, 1, 14, 50, 0, 100, 8.0, 8.0, 8.0, 0.0, 15.0, 0, 0 );
			}
			else if ( from.Name == "Arcadion" )
			{
				GiveGiftBonus( item, 99, 0, 0, 0, 0, 10.0, 0.0, 0.0, 0.0, 0.0, 5, 0 );
			}
			else if ( from.Name == "Samhayne" )
			{
				GiveGiftBonus( item, 99, 19, 12, 0, 0, 10.0, 10.0, 10.0, 0.0, 0.0, 34, 0 );
			}
			else if ( from.Name == "Seggallion" )
			{
				GiveGiftBonus( item, 99, 19, 12, 0, 0, 10.0, 10.0, 10.0, 0.0, 0.0, 34, 0 );
			}
			else if ( from.Name == "Minax" )
			{
				GiveGiftBonus( item, 17, 31, 33, 0, 0, 8.0, 8.0, 8.0, 0.0, 0.0, 0, 0 );
			}
			else if ( from.Name == "Nystal" )
			{
				GiveGiftBonus( item, 17, 31, 33, 0, 0, 8.0, 8.0, 8.0, 0.0, 0.0, 0, 0 );
			}
			else if ( from.Name == "Lord British" )
			{
				GiveGiftBonus( item, 99, 48, 38, 21, 0, 5.0, 5.0, 5.0, 5.0, 0.0, 11, 13 );
			}
			else if ( from.Name == "Lord Blackthorne" )
			{
				GiveGiftBonus( item, 99, 40, 25, 46, 0, 5.0, 5.0, 5.0, 5.0, 0.0, 5, 0 );
			}
			else if ( from.Name == "Geoffrey" )
			{
				GiveGiftBonus( item, 99, 48, 38, 21, 0, 5.0, 5.0, 5.0, 5.0, 0.0, 6, 31 );
			}
			else if ( from.Name == "Shimazu" )
			{
				GiveGiftBonus( item, 99, 9, 37, 38, 48, 5.0, 5.0, 5.0, 5.0, 5.0, 0, 0 );
			}
			else if ( from.Name == "Gorn" )
			{
				GiveGiftBonus( item, 99, 10, 23, 48, 52, 5.0, 5.0, 5.0, 5.0, 5.0, 27, 0 );
			}
			else if ( from.Name == "Jaana" )
			{
				GiveGiftBonus( item, 2, 23, 53, 0, 0, 8.0, 8.0, 8.0, 0.0, 0.0, 1, 0 );
			}
			else if ( from.Name == "Dupre" )
			{
				GiveGiftBonus( item, 99, 48, 38, 13, 0, 5.0, 5.0, 5.0, 5.0, 0.0, 1, 0 );
			}
			else if ( from.Name == "Gwenno" )
			{
				GiveGiftBonus( item, 16, 35, 39, 41, 0, 5.0, 5.0, 5.0, 5.0, 0.0, 0, 0 );
			}
			else if ( from.Name == "Iolo" )
			{
				GiveGiftBonus( item, 100, 48, 21, 29, 0, 5.0, 5.0, 5.0, 5.0, 0.0, 0, 0 );
			}
			else if ( from.Name == "Shamino" )
			{
				GiveGiftBonus( item, 99, 11, 29, 48, 52, 5.0, 5.0, 5.0, 5.0, 5.0, 33, 30 );
			}
			else if ( from.Name == "Stefano" )
			{
				GiveGiftBonus( item, 15, 25, 42, 43, 45, 5.0, 5.0, 5.0, 5.0, 5.0, 0, 0 );
			}
			else if ( from.Name == "Katrina" )
			{
				GiveGiftBonus( item, 3, 4, 24, 53, 0, 5.0, 5.0, 5.0, 5.0, 0.0, 0, 0 );
			}
			else if ( from.Name == "the Guardian" )
			{
				GiveGiftBonus( item, 99, 32, 0, 0, 0, 10.0, 10.0, 0.0, 0.0, 0.0, 0, 0 );
			}
			else if ( from.Name == "Garamon" )
			{
				GiveGiftBonus( item, 1, 14, 50, 0, 100, 8.0, 8.0, 8.0, 0.0, 15.0, 0, 0 );
			}
			else if ( from.Name == "Mors Gotha" )
			{
				GiveGiftBonus( item, 99, 48, 38, 13, 0, 5.0, 5.0, 5.0, 5.0, 0.0, 5, 0 );
			}
			else if ( from.Name == "Lethe" )
			{
				GiveGiftBonus( item, 22, 1, 36, 0, 0, 8.0, 8.0, 8.0, 0.0, 0.0, 0, 0 );
			}
		}

		public static void GiveGiftBonus( Item item, int val1, int val2, int val3, int val4, int val5, double sk1, double sk2, double sk3, double sk4, double sk5, int slay1, int slay2 )
		{
			if ( item is BaseWeapon )
			{
				if ( slay1 > 0 ){ ((BaseWeapon)item).Slayer = ResourceMods.GetSlayer( slay1 ); }
				if ( slay2 > 0 ){ ((BaseWeapon)item).Slayer2 = ResourceMods.GetSlayer( slay2 ); }

				if ( val1 == 99 ){ ((BaseWeapon)item).SkillBonuses.SetValues(0, ((BaseWeapon)item).Skill, sk1); }
				else if ( val1 == 100 && item is BaseRanged ){ ((BaseWeapon)item).SkillBonuses.SetValues(0, ResourceMods.GetSkill( 5 ), sk1); }
				else if ( val1 > 0 ){ ((BaseWeapon)item).SkillBonuses.SetValues(0, ResourceMods.GetSkill( val1 ), sk1); }
				if ( val2 > 0 ){ ((BaseWeapon)item).SkillBonuses.SetValues(1, ResourceMods.GetSkill( val2 ), sk2); }
				if ( val3 > 0 ){ ((BaseWeapon)item).SkillBonuses.SetValues(2, ResourceMods.GetSkill( val3 ), sk3); }
				if ( val4 > 0 ){ ((BaseWeapon)item).SkillBonuses.SetValues(3, ResourceMods.GetSkill( val4 ), sk4); }
				if ( val5 == 100 ){ ((BaseWeapon)item).Attributes.EnhancePotions = (int)sk5;  }
				else if ( val5 > 0 ){ ((BaseWeapon)item).SkillBonuses.SetValues(4, ResourceMods.GetSkill( val5 ), sk5); }
			}
			else if ( item is BaseArmor )
			{
				if ( val1 == 99 ){}
				else if ( val1 == 100 ){ ((BaseArmor)item).SkillBonuses.SetValues(0, ResourceMods.GetSkill( 5 ), sk1); }
				else if ( val1 > 0 ){ ((BaseArmor)item).SkillBonuses.SetValues(0, ResourceMods.GetSkill( val1 ), sk1); }
				if ( val2 > 0 ){ ((BaseArmor)item).SkillBonuses.SetValues(1, ResourceMods.GetSkill( val2 ), sk2); }
				if ( val3 > 0 ){ ((BaseArmor)item).SkillBonuses.SetValues(2, ResourceMods.GetSkill( val3 ), sk3); }
				if ( val4 > 0 ){ ((BaseArmor)item).SkillBonuses.SetValues(3, ResourceMods.GetSkill( val4 ), sk4); }
				if ( val5 == 100 ){ ((BaseArmor)item).Attributes.EnhancePotions = (int)sk5; }
				else if ( val5 > 0 ){ ((BaseArmor)item).SkillBonuses.SetValues(4, ResourceMods.GetSkill( val5 ), sk5); }
			}
			else if ( item is BaseClothing )
			{
				if ( val1 == 99 ){}
				else if ( val1 == 100 ){ ((BaseClothing)item).SkillBonuses.SetValues(0, ResourceMods.GetSkill( 5 ), sk1); }
				else if ( val1 > 0 ){ ((BaseClothing)item).SkillBonuses.SetValues(0, ResourceMods.GetSkill( val1 ), sk1); }
				if ( val2 > 0 ){ ((BaseClothing)item).SkillBonuses.SetValues(1, ResourceMods.GetSkill( val2 ), sk2); }
				if ( val3 > 0 ){ ((BaseClothing)item).SkillBonuses.SetValues(2, ResourceMods.GetSkill( val3 ), sk3); }
				if ( val4 > 0 ){ ((BaseClothing)item).SkillBonuses.SetValues(3, ResourceMods.GetSkill( val4 ), sk4); }
				if ( val5 == 100 ){ ((BaseClothing)item).Attributes.EnhancePotions = (int)sk5; }
				else if ( val5 > 0 ){ ((BaseClothing)item).SkillBonuses.SetValues(4, ResourceMods.GetSkill( val5 ), sk5); }
			}
			else if ( item is BaseTrinket )
			{
				if ( val1 == 99 ){}
				else if ( val1 == 100 ){ ((BaseTrinket)item).SkillBonuses.SetValues(0, ResourceMods.GetSkill( 5 ), sk1); }
				else if ( val1 > 0 ){ ((BaseTrinket)item).SkillBonuses.SetValues(0, ResourceMods.GetSkill( val1 ), sk1); }
				if ( val2 > 0 ){ ((BaseTrinket)item).SkillBonuses.SetValues(1, ResourceMods.GetSkill( val2 ), sk2); }
				if ( val3 > 0 ){ ((BaseTrinket)item).SkillBonuses.SetValues(2, ResourceMods.GetSkill( val3 ), sk3); }
				if ( val4 > 0 ){ ((BaseTrinket)item).SkillBonuses.SetValues(3, ResourceMods.GetSkill( val4 ), sk4); }
				if ( val5 == 100 ){ ((BaseTrinket)item).Attributes.EnhancePotions = (int)sk5; }
				else if ( val5 > 0 ){ ((BaseTrinket)item).SkillBonuses.SetValues(4, ResourceMods.GetSkill( val5 ), sk5); }
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if (	( dropped is Artifact_DupresCollar && this.Name == "Dupre" ) || 
					( dropped is Artifact_DupresShield && this.Name == "Dupre" ) || 
					( dropped is Artifact_GwennosHarp && this.Name == "Gwenno" ) || 
					( dropped is Artifact_ShaMontorrossbow && this.Name == "Shamino" ) || 
					( dropped is Artifact_IolosLute && this.Name == "Iolo" )
				)
			{
				this.Say( "Thank you, " + from.Name + "! I lost that this years ago." );
				from.SendSound( 0x5B4 );
				dropped.Delete();
				int gold = Utility.RandomMinMax(5,10) * 1000;
				from.AddToBackpack ( new BankCheck( gold ) );
				from.SendMessage( this.Name + " gave you a check for " + gold + " gold!" );
				return true;
			}
			else if ( dropped is CourierMail && !from.Blessed )
			{
				CourierMail scroll = (CourierMail)dropped;
				string FullName = this.Name + " " + this.Title;

				if ( scroll.owner == from && scroll.MsgComplete > 0 && scroll.ForWho == FullName )
				{
					string success = "has found the " + scroll.SearchItem + " for " + FullName;
					LoggingFunctions.LogGenericQuest( from, success );

					int KarmaFame = ( scroll.MsgReward * 100 ) - 100;
						if ( KarmaFame < 100 ){ KarmaFame = 100; }

					Titles.AwardFame( from, KarmaFame, true );

					if ( scroll.ForAlignment == "evil" ){ Titles.AwardKarma( from, -KarmaFame, true ); }
					else if ( scroll.ForAlignment == "good" ){ Titles.AwardKarma( from, KarmaFame, true ); }

					int GoldReward = scroll.MsgReward * 1000;
						if ( scroll.ForAlignment == "neutral" ){ GoldReward = scroll.MsgReward * 1500; }

					CustomEventSink.InvokeCombatQuestCompleted( from, GoldReward );

					from.AddToBackpack( new Gold( GoldReward ) );

					string GoldText = GoldReward.ToString();

					string sMessage = "";

					if ( scroll.ForAlignment == "good" )
					{
						switch( Utility.RandomMinMax( 0, 5 ) )
						{
							case 0: sMessage = "Thank you for bringing this to me."; 			break;
							case 1: sMessage = "I knew you could do it."; 						break;
							case 2: sMessage = "This is a great help to us all."; 				break;
							case 3: sMessage = "Good work! I am glad to see you arrive well."; 	break;
							case 4: sMessage = "Your valor will be remembered."; 				break;
							case 5: sMessage = "You have done what most others could not."; 	break;
						}
					}
					else if ( scroll.ForAlignment == "evil" )
					{
						switch( Utility.RandomMinMax( 0, 5 ) )
						{
							case 0: sMessage = "It is good that you did not fail me."; 				break;
							case 1: sMessage = "I trust you eliminated any troubles for this?"; 	break;
							case 2: sMessage = "Ahhh...another step closer to my plan."; 			break;
							case 3: sMessage = "You may prove to be useful yet."; 					break;
							case 4: sMessage = "You took long enough."; 							break;
							case 5: sMessage = "I was about to send someone to deal with you."; 	break;
						}
					}
					else
					{
						switch( Utility.RandomMinMax( 0, 5 ) )
						{
							case 0: sMessage = "Hmmm...I see you found it."; 						break;
							case 1: sMessage = "I trust you had little difficulty?";			 	break;
							case 2: sMessage = "Good! I thought for sure you were lost."; 			break;
							case 3: sMessage = "I guess my trust was well placed."; 				break;
							case 4: sMessage = "I thought you perished in the attempt."; 			break;
							case 5: sMessage = "I wasn't sure it really existed."; 					break;
						}
					}

					from.SendSound( 0x3D );
					from.SendMessage( GoldText + " gold has been added to your pack." );
					this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sMessage, from.NetState);
					dropped.Delete();
					return true;
				}
			}
			else if ( dropped is QuestTome && !from.Blessed )
			{
				QuestTome book = (QuestTome)dropped;
				string FullName = this.Name + " " + this.Title;
				if ( book.QuestTomeOwner == from && ( book.QuestTomeNPCGood == this.Name + " " + this.Title || book.QuestTomeNPCEvil == this.Name + " " + this.Title ) )
				{
					bool handled = book.QuestTomeGoals > 3;
					string sMessage = "";
					if ( handled )
					{
						string success = "has found " + book.GoalItem4 + " for " + FullName;
						LoggingFunctions.LogGenericQuest( from, success );

						int KarmaFame = 1000;

						Titles.AwardFame( from, KarmaFame, true );
						from.SendSound( 0x3D );

						if ( this.MyAlignment == "evil" ){ Titles.AwardKarma( from, -KarmaFame, true ); }
						else if ( this.MyAlignment == "good" ){ Titles.AwardKarma( from, KarmaFame, true ); }

						if ( this.MyAlignment == "good" )
						{
							sMessage = "Ahhh...you found it and perhaps saved us all! Choose your reward.";
						}
						else if ( this.MyAlignment == "evil" )
						{
							sMessage = "Good! Everything is going to plan. Choose your reward.";
						}

						if ( !from.HasGump( typeof( EpicBookGump ) ) )
						{
							from.SendGump( new EpicBookGump( from, this, 0, false ) );
						}

						dropped.Delete();
					}
					else
					{
						if ( this.MyAlignment == "good" )
						{
							sMessage = "Return to me when you find it.";
						}
						else if ( this.MyAlignment == "evil" )
						{
							sMessage = "Do not fail me in this task.";
						}
					}

					this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sMessage, from.NetState);
					return handled;
				}
			}

			return base.OnDragDrop( from, dropped );
		}
	}
}