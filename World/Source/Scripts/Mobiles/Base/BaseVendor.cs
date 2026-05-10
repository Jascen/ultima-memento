using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Network;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Misc;
using Server.Regions;
using Server.Multis;
using Server.Targeting;
using Server.Engines.GlobalShoppe;
using Server.Utilities;
using System.Linq;

namespace Server.Mobiles
{
	public enum VendorShoeType
	{
		None,
		Shoes,
		Boots,
		Sandals,
		ThighBoots
	}

	public abstract class SBInfo
	{
		public SBInfo()
		{
		}

		public abstract IShopSellInfo SellInfo { get; }

        public abstract List<GenericBuyInfo> BuyInfo { get; }
	}

	public abstract class BaseVendor : BaseCreature, IVendor
	{
		public DateTime m_NextAction;
		public DateTime NextAction{ get{ return m_NextAction; } set{ m_NextAction = value; } }

		private const int MaxSell = 500;

		protected abstract List<SBInfo> SBInfos { get; }

		private ArrayList m_ArmorBuyInfo = new ArrayList();
		private ArrayList m_ArmorSellInfo = new ArrayList();

		private Dictionary<string, int> m_CoinsPerAccount;


		public override bool CanTeach { get { return true; } }

		public override bool BardImmune { get { return false; } }

		public override bool PlayerRangeSensitive { get { return true; } }

		public virtual bool IsBlackMarket { get { return false; } }
		public virtual bool IsActiveVendor { get { return true; } }
		public virtual bool IsActiveBuyer
		{
			get
			{
				if ( !MySettings.S_VendorsBuyStuff )
					return false;

				return IsActiveVendor;
			}
		} // response to vendor SELL
		public virtual bool IsActiveSeller { get { return IsActiveVendor; } } // repsonse to vendor BUY

		public virtual NpcGuild NpcGuild { get { return NpcGuild.None; } }

		public virtual bool IsInvulnerable { get { return false; } }
		public override bool Unprovokable { get { return false; } }
		public override bool Uncalmable{ get{ return false; } }

		public override bool ShowFameTitle { get { return false; } }

		public virtual int GetPriceScalar()
		{
			return 100;
		}

		public void UpdateBuyInfo()
		{
			int priceScalar = GetPriceScalar();

			IBuyItemInfo[] buyinfo = (IBuyItemInfo[])m_ArmorBuyInfo.ToArray( typeof( IBuyItemInfo ) );

			if ( buyinfo != null )
			{
				foreach ( IBuyItemInfo info in buyinfo )
					info.PriceScalar = priceScalar;
			}
		}

		public virtual void UpdateBlackMarket()
		{
			if ( this is PlayerBarkeeper ) return;

			Container bank = this.BankBox;
			if ( IsBlackMarket && MyServerSettings.BlackMarket() )
			{
				List<Item> belongings = new List<Item>();
				foreach( Item i in bank.Items )
				{
					belongings.Add(i);
				}
				foreach ( Item stuff in belongings )
				{
					stuff.Delete();
				}
			}
		}

		public virtual void UpdateCoins()
		{
			if ( this is PlayerBarkeeper ) return;

			Container pack = this.Backpack;

			if ( pack == null )
			{
				pack = new Backpack();
				pack.Movable = false;
				AddItem( pack );
			}

			List<Item> belongings = new List<Item>();
			foreach( Item i in pack.Items )
			{
				belongings.Add(i);
			}
			foreach ( Item stuff in belongings )
			{
				stuff.Delete();
			}

			int money1 = 30;
			int money2 = 120;

			double w1 = money1 * (MyServerSettings.GetGoldCutRate() * .01);
			double w2 = money2 * (MyServerSettings.GetGoldCutRate() * .01);

			money1 = (int)w1;
			money2 = (int)w2;

			this.PackGold( money1, money2 );

			int loot = 10;

			while ( loot > 0 )
			{
				loot--;

				if ( Utility.RandomBool() )
					this.PackItem( Loot.RandomItem( this, -10 ) );
			}
		}

		public BaseVendor( string title ) : base( AIType.AI_Vendor, FightMode.Closest, 15, 1, 0.1, 0.2 )
		{
			this.Title = title;

			SpeechHue = Utility.RandomTalkHue();

			InitBody();
			InitOutfit();

			Container pack;
			//these packs MUST exist, or the client will crash when the packets are sent
			pack = new Backpack();
			pack.Layer = Layer.ShopBuy;
			pack.Movable = false;
			pack.Visible = false;
			AddItem( pack );

			pack = new Backpack();
			pack.Layer = Layer.ShopResale;
			pack.Movable = false;
			pack.Visible = false;
			AddItem( pack );

			LoadSBInfo();
			RefreshSelf();
		}

		private static int GetOrCreateCoinPurse( BaseVendor vendor, Mobile from, bool sendMessage = false)
		{
			if ( vendor is PlayerBarkeeper ) return 0;
			
			// Reset ASAP
			if ( vendor.m_CoinsNeedReset )
				vendor.m_CoinsPerAccount = null;

			// Initialize and get value
			int? existing = vendor.TryGetCoinPurse( from );
			if ( existing == null ) return 0; // Unexpected case

			// Bail if coins have already been generated
			if ( !vendor.m_CoinsNeedReset )
			{
				if ( 0 < existing.Value || !vendor.HasCoinsForPurchase(from) ) return existing.Value;
			}

			// Generate a new value
			int minGold = MySettings.S_MinMerchant;
			int maxGold = MySettings.S_MaxMerchant;

			if ( MySettings.S_ScaleMerchantCoins )
			{
				double multiplier;
				var land = Lands.GetLand( vendor );
				switch (land)
				{
					default:
					case Land.Atlantis: // Not implemented
					case Land.Luna: // Off Sosaria - Mages only
					case Land.SkaraBrae: // Not a big need for Gold here
					case Land.Sosaria:
					case Land.None:
						multiplier = 1;
						break;

					case Land.UmberVeil: // Off of Sosaria
					case Land.IslesDread: // Off of Sosaria (Ancient Pyramid)
					case Land.Ambrosia: // A single Necromancer + Wandering Healers
					case Land.Lodoria: // Usually from Poseidon
					case Land.Serpent: // After Umber
					case Land.Savaged: // Lots of Vendors
						multiplier = 1.5;
						break;

					case Land.Kuldar: // Give a small reason to go back
						multiplier = 1.75;
						break;

					case Land.Underworld: // A small encampent
						multiplier = 2.0;
						break;
				}

				if (1 < multiplier)
				{
					minGold = (int)( minGold * multiplier );
					maxGold = (int)( maxGold * multiplier );
				}
			}

			int newCoins = Utility.RandomMinMax( minGold, maxGold );

			if ( from is PlayerMobile )
			{
				const int MAX_BONUS_PERCENT = 50;
				const int HALF_BONUS_PERCENT = MAX_BONUS_PERCENT / 2;

				string message = null;
				int bonusGoldPercent;
				PlayerMobile player = (PlayerMobile)from;
				if ( player.NpcGuild == vendor.NpcGuild )
				{
					bonusGoldPercent = MAX_BONUS_PERCENT;
					message = "Thanks to your guild membership, the merchant offers more coin.";
				}
				else if ( player.NpcGuild == NpcGuild.MerchantsGuild )
				{
					bonusGoldPercent = HALF_BONUS_PERCENT;
					message = "The merchant offers all of his coins.";
				}
				else
				{
					bonusGoldPercent = Math.Min( HALF_BONUS_PERCENT, (int)from.Skills[SkillName.Mercantile].Value / 2 );
					if ( 0 < bonusGoldPercent )
						message = bonusGoldPercent == HALF_BONUS_PERCENT 
							? "The merchant offers all of his coins."
							: "You convince the merchant to offer a few more coins.";
				}

				newCoins += newCoins * bonusGoldPercent / 100;

				// Chance to explain the bonus is in effect
				if ( sendMessage && !string.IsNullOrWhiteSpace(message) && Utility.RandomMinMax(1, 5) == 1 )
					from.SendMessage(message);
			}

			if ( !vendor.SetCoinPurse( from, newCoins ) ) return 0; // Unexpected case

			if ( vendor.m_CoinsNeedReset )
			{
				vendor.RefreshSelf();
				vendor.m_CoinsNeedReset = false;
				vendor.InvalidateProperties();
				vendor.m_RefreshSelfTimer = Timer.DelayCall(vendor.RefreshSelfDelay, () => vendor.RefreshSelf());
			}

			return newCoins;
		}

		private static string GetAccountId( Mobile buyer )
		{
			var player = buyer as PlayerMobile;
			if (player == null || player.Account == null) return null;

			return player.Account.Username;
		}

		private bool HasCoinsForPurchase( Mobile buyer )
		{
            var id = GetAccountId( buyer );
            if ( id == null ) return false;
			if ( !m_CoinsPerAccount.ContainsKey( id ) ) return true; // Can be generated

			return 0 < m_CoinsPerAccount[id]; // Has enough currently
		}

		private int? TryGetCoinPurse( Mobile buyer )
		{
            var id = GetAccountId( buyer );
            if ( id == null ) return null;

			// Lazily initialize the dictionary
			if ( m_CoinsPerAccount == null )
				m_CoinsPerAccount = new Dictionary<string, int>();

			int value;

			return m_CoinsPerAccount.TryGetValue( id, out value ) ? value : 0;
		}

		public bool AddToCoinPurse( Mobile buyer, int amount )
		{
			var coins = TryGetCoinPurse( buyer );
			if ( coins == null ) return false;

			return SetCoinPurse( buyer, Math.Max( 0, coins.Value + amount ) );
		}

		private bool SetCoinPurse( Mobile buyer, int amount )
		{
            var id = GetAccountId( buyer );
            if ( id == null ) return false;
			if ( m_CoinsPerAccount == null ) return false;

			m_CoinsPerAccount[id] = amount;

			return true;
		}

		public BaseVendor( Serial serial ): base( serial )
		{
		}

		public static int BeggingPose( Mobile from ) // LET US SEE IF THEY ARE BEGGING
		{
			int beggar = 0;
			if ( from is PlayerMobile )
			{
				if ( ((PlayerMobile)from).CharacterBegging > 0 )
				{
					beggar = (int)from.Skills[SkillName.Begging].Value;
				}
			}
			return beggar;
		}
		
		public static int BeggingKarma( Mobile from ) // LET US SEE IF THEY ARE BEGGING
		{
			int charisma = 0;
			if ( from.Karma > -2459 ){ charisma = 40; }
			from.CheckSkill( SkillName.Begging, 0, 125 );
			return charisma;
		}

		public static string BeggingWords() // LET US SEE IF THEY ARE BEGGING
		{
			string sSpeak = "Please give me a good price as I am so poor.";
			switch( Utility.RandomMinMax( 0, 5 ) )
			{
				case 0: sSpeak = "Please give me a good price as I am so poor."; break;
				case 1: sSpeak = "I have very little gold so whatever you can give..."; break;
				case 2: sSpeak = "I have not eaten in days so your gold will surely help."; break;
				case 3: sSpeak = "Will thou give a poor soul more for these?"; break;
				case 4: sSpeak = "I have fallen on hard times, will thou be kind?"; break;
				case 5: sSpeak = "Whatever you can give for these will surely help."; break;
			}
			return sSpeak;
		}

		private bool m_CoinsNeedReset = true;
		public DateTime LastRefreshSelf { get; private set; }
		public virtual TimeSpan RefreshSelfDelay
		{
			get
			{
				return TimeSpan.FromMinutes( 30 );
			}
		}

		public DateTime LastRestockWares { get; private set; }
		public virtual TimeSpan RestockWaresDelay
		{
			get
			{
				return TimeSpan.FromMinutes( 30 );
			}
		}

		public Container BuyPack
		{
			get
			{
				Container pack = FindItemOnLayer( Layer.ShopBuy ) as Container;

				if ( pack == null )
				{
					pack = new Backpack();
					pack.Layer = Layer.ShopBuy;
					pack.Visible = false;
					AddItem( pack );
				}

				return pack;
			}
		}

		public abstract void InitSBInfo( Mobile m );

		protected void LoadSBInfo()
		{
			LastRestockWares = DateTime.Now;

			for ( int i = 0; i < m_ArmorBuyInfo.Count; ++i )
			{
				GenericBuyInfo buy = m_ArmorBuyInfo[i] as GenericBuyInfo;

				if ( buy != null )
					buy.DeleteDisplayEntity();
			}

			SBInfos.Clear();

			InitSBInfo( this );

			m_ArmorBuyInfo.Clear();
			m_ArmorSellInfo.Clear();

			for ( int i = 0; i < SBInfos.Count; i++ )
			{
				SBInfo sbInfo = (SBInfo)SBInfos[i];
				m_ArmorBuyInfo.AddRange( sbInfo.BuyInfo );
				m_ArmorSellInfo.Add( sbInfo.SellInfo );
			}
		}

		public virtual bool GetGender()
		{
			return Utility.RandomBool();
		}

		public virtual void InitBody()
		{
			InitStats( 100, 100, 25 );

			SpeechHue = Utility.RandomTalkHue();
			Hue = Utility.RandomSkinColor();

			if ( IsInvulnerable && !Core.AOS )
				NameHue = 0x35;

			if ( this is Roscoe || this is Garth )
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
			}
			else if ( this is KungFu )
			{
				if ( Female = GetGender() )
				{
					Body = 0x191;
					Name = NameList.RandomName( "tokuno female" );
				}
				else
				{
					Body = 0x190;
					Name = NameList.RandomName( "tokuno male" );
				}
			}
			else if ( Female = GetGender() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
			}
		}

		public virtual int GetRandomHue()
		{
			switch ( Utility.Random( 5 ) )
			{
				default:
				case 0: return Utility.RandomBlueHue();
				case 1: return Utility.RandomGreenHue();
				case 2: return Utility.RandomRedHue();
				case 3: return Utility.RandomYellowHue();
				case 4: return Utility.RandomNeutralHue();
			}
		}

		public virtual int GetShoeHue()
		{
			if ( 0.1 > Utility.RandomDouble() )
				return 0;

			return Utility.RandomNeutralHue();
		}

		public virtual VendorShoeType ShoeType
		{
			get { return VendorShoeType.Shoes; }
		}

		public virtual int RandomBrightHue()
		{
			if ( 0.1 > Utility.RandomDouble() )
				return Utility.RandomList( 0x62, 0x71 );

			return Utility.RandomList( 0x03, 0x0D, 0x13, 0x1C, 0x21, 0x30, 0x37, 0x3A, 0x44, 0x59 );
		}

		public static void DoJokes( Mobile m )
		{
			int act = Utility.Random( 28 );
			if ( m is PlayerMobile ){ act = Utility.Random( 22 ); }
			switch ( act )
			{
				case 0: m.Say("Why did the king go to the dentist? To get his teeth crowned."); break;
				case 1: m.Say("When a knight in armor was killed in battle, what sign did they put on his grave? Rust in peace!"); break;
				case 2: m.Say("What do you call a mosquito in a tin suit? A bite in shining armor."); break;
				case 3: m.Say("There are many castles in the world, but who is strong enough to move one? Any chess player"); break;
				case 4: m.Say("What king was famous because he spent so many nights at his Round Table writing books? King Author!"); break;
				case 5: m.Say("How do you find a princess? You follow the foot prince."); break;
				case 6: m.Say("Why were the early days called the dark ages? Because there were so many knights!"); break;
				case 7: m.Say("Why did Arthur have a round table? So no one could corner him!"); break;
				case 8: m.Say("Who invented King Arthur's round table? Sir Cumference!"); break;
				case 9: m.Say("Why did the knight run about shouting for a tin opener? He had a bee in his suit of armor!"); break;
				case 10: m.Say("What was Camelot famous for? It's knight life!"); break;
				case 11: m.Say("What did the toad say when the princess would not kiss him? Warts the matter with you?"); break;
				case 12: m.Say("What do you call the young royal who keeps falling down? Prince Harming!"); break;
				case 13: m.Say("What do you call a cat that flies over the castle wall? A cat-a-pult!"); break;
				case 14: m.Say("What game do the fish play in the moat? Trout or dare!"); break;
				case 15: m.Say("What did the fish say to the other when the horse fell in the moat? See horse!"); break;
				case 16: m.Say("What do you call an angry princess just awakened from a long sleep? Slapping beauty!"); break;
				case 17: m.Say("How did the prince get into the castle when the drawbridge was broken? He used a rowmoat!"); break;
				case 18: m.Say("How did the girl dragon win the beauty contest? She was the beast of the show!"); break;
				case 19: m.Say("Why did the dinosaur live longer than the dragon? Because it didn’t smoke!"); break;
				case 20: m.Say("What did the dragon say when it saw the Knight? 'Not more tinned food!'"); break;
				case 21: m.Say("What do you do with a green dragon? Wait until it ripens!"); break;
				case 22: m.PlaySound( m.Female ? 780 : 1051 ); m.Say( "*claps*" ); break;
				case 23: m.Say( "*bows*" ); m.Animate( 32, 5, 1, true, false, 0 ); break;
				case 24: m.PlaySound( m.Female ? 794 : 1066 ); m.Say( "*giggles*" ); break;
				case 25: m.PlaySound( m.Female ? 801 : 1073 ); m.Say( "*laughs*" ); break;
				case 26: m.PlaySound( 792 ); m.Say( "*sticks out tongue*" ); break;
				case 27: m.PlaySound( m.Female ? 783 : 1054 ); m.Say( "*woohoo!*" ); break;
			};

			if ( act < 22 && Utility.RandomBool() )
			{
				switch ( Utility.Random( 6 ))
				{
					case 0: m.PlaySound( m.Female ? 780 : 1051 ); break;
					case 1: m.Animate( 32, 5, 1, true, false, 0 ); break;
					case 2: m.PlaySound( m.Female ? 794 : 1066 ); break;
					case 3: m.PlaySound( m.Female ? 801 : 1073 ); break;
					case 4: m.PlaySound( 792 ); break;
					case 5: m.PlaySound( m.Female ? 783 : 1054 ); break;
				};
			}
		}

		public override void OnThink()
		{
			bool ran = false;
			if ( DateTime.Now >= m_NextAction && ( this is Jester || ( this.Location == this.Home && this.RangeHome == 0 ) ) )
			{
				int range = 1;
				if ( this is Fisherman || this is Ranger || this is Bowyer )
					range = 7;

				m_NextAction = (DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( 5, 15 ) ) );

				if ( this is Jester )
				{
					DoJokes( this );
					m_NextAction = (DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( 25, 35 ) ) );
					ran = true;
				}
				else
				{
					ArrayList acts = new ArrayList();
					foreach ( Item act in this.GetItemsInRange( range ) )
					{
						if ( act.Map == this.Map && act is ArcheryButte && ( this is Ranger || this is Bowyer ) && ( act.X == X || act.Y == Y ) )
						{
							if ( this is Bowyer ){ this.Title = "the archer"; }
								acts.Add( act );
						}
						else if ( act.Map == this.Map && act is TrainingDummy && this is Fighter )
						{
							acts.Add( act );
						}
						else if ( act.Map == this.Map && act is Actions )
						{
							int z = Z - act.Z;
								if ( z < 0 )
									z = act.Z - Z;

							if ( z < 15 )
							{
								acts.Add( act );
							}
						}
					}
					for ( int i = 0; i < acts.Count; ++i )
					{
						ran = true;
						Item it = ( Item )acts[ i ];
						it.OnDoubleClick( this );
					}
				}

				if ( !ran )
					m_NextAction = (DateTime.Now).AddYears(1);
			}

			base.OnThink();

			if ( PlayersInSight() )
				this.CantWalk = true;
			else
				this.CantWalk = false;
		}

		private bool PlayersInSight()
		{
			int players = 0;
			bool enemies = false;

			foreach ( Mobile player in GetMobilesInRange( 6 ) )
			{
				if ( player is PlayerMobile )
				{
					if ( CanSee( player ) && InLOS( player ) ){ ++players; }
				}

				if ( IsEnemy( player ) )
					enemies = true;
			}

			if ( players > 0 && !enemies )
				return true;

			return false;
		}

		public override void OnAfterSpawn()
		{
			ControlSlots = RangeHome;

			bool runActs = true;

			base.OnAfterSpawn();

			Region reg = Region.Find( this.Location, this.Map );

			if ( !Server.Items.EssenceBase.ColorCitizen( this ) )
				Server.Misc.MorphingTime.CheckMorph( this );

			if ( this is BaseVendor && ( reg.Name == "the Grey Archeological Dig" || this is Merchant ) )
			{
				Item candle = new HeldLight();
				candle.Name = "lantern";
				candle.ItemID = 0xA22;
				candle.Light = LightType.Circle300;
				AddItem( candle );
			}

			if ( ( reg.Name == "the Basement" || reg.Name == "the Dungeon Room" || reg.Name == "the Camping Tent" ) && this is Provisioner )
			{
				this.Title = "the merchant";
				runActs = false;
			}
			else if ( Server.Misc.Worlds.IsSeaTown( this.Location, this.Map ) )
			{
				if ( this is Provisioner && reg.Name != "the Port" ){ this.Title = "the dock worker"; if ( Utility.RandomBool() ){ this.Title = "the merchant"; } }
				else if ( this is Fisherman && reg.Name != "the Port" ){ runActs = false; this.Title = "the sailor"; }
				else if ( this is Carpenter && reg.Name != "the Port" ){ runActs = false; this.Title = "the cooper"; }
				else if ( this is Waiter ){ this.Title = "the cabin boy"; if ( this.Female ){ this.Title = "the serving wench"; } }
				else if ( this is Weaponsmith && reg.Name != "the Port" ){ runActs = false; this.Title = "the master-at-arms"; }
				else if ( this is Ranger )
				{
					this.Title = "the harpooner";
					if ( this.FindItemOnLayer( Layer.OneHanded ) != null ) { this.FindItemOnLayer( Layer.OneHanded ).Delete(); }
					if ( this.FindItemOnLayer( Layer.TwoHanded ) != null ) { this.FindItemOnLayer( Layer.TwoHanded ).Delete(); }
					if ( this.FindItemOnLayer( Layer.Helm ) != null ) { this.FindItemOnLayer( Layer.Helm ).Delete(); }
					if ( this.FindItemOnLayer( Layer.Helm ) != null ) { this.FindItemOnLayer( Layer.Helm ).Delete(); }
					if ( Utility.RandomBool() ){ this.AddItem( new SkullCap( Utility.RandomDyedHue() ) ); }
					this.AddItem( new Harpoon() );
					runActs = false;
				}
				else if ( this is Shipwright && reg.Name != "the Port" ){ runActs = false; this.Title = "the boatswain"; }

				if ( !(this is Shipwright && reg.Name == "the Port" ) )
				{
					if ( !(this is Jester) && !(this is Druid) && !(this is VarietyDealer) )
					{
						if ( this.FindItemOnLayer( Layer.Helm ) != null ) { this.FindItemOnLayer( Layer.Helm ).Delete(); }
						if ( Utility.RandomBool() ){ this.AddItem( new SkullCap() ); }
						else { this.AddItem( new Bandana() ); }
						MorphingTime.ColorMixClothes( this );
					}
				}
			}
			else if ( reg.Name == "the Thieves Guild" && this is Provisioner )
			{
				this.Title = "the fence";
			}
			else if ( reg.Name == "the Ship's Lower Deck" && !(this is Jester) )
			{
				if ( this is Provisioner  ){ this.Title = "the quartermaster"; }
				else if ( this is Waiter ){ this.Title = "the cabin boy"; if ( this.Female ){ this.Title = "the serving wench"; } }
				if ( this.FindItemOnLayer( Layer.Helm ) != null ) { this.FindItemOnLayer( Layer.Helm ).Delete(); }
				if ( Utility.RandomBool() ){ this.AddItem( new SkullCap() ); }
				else { this.AddItem( new Bandana() ); }
				MorphingTime.ColorOnlyClothes( this, Utility.RandomColor(0), 0 );
			}
			else if ( reg.Name == "the Wizards Guild" && this is Waiter && this.Body == 400 )
			{
				this.Title = "the butler";
			}
			else if ( reg.Name == "the Wizards Guild" && this is Waiter && this.Body == 401 )
			{
				this.Title = "the maid";
			}

			if ( Server.Items.ActionFunc.HasActs( this ) && runActs )
			{
				Server.Items.ActionFunc.RemoveActions( true, this.Home, this.Map );
				Server.Items.ActionFunc.MakeActs( (BaseCreature)this );
			}

			if ( Title.Contains("mistress") && !Female )
				Title = Title.Replace("mistress", "master");
			else if ( Title.Contains("master") && Female )
				Title = Title.Replace("master", "mistress");
		}

		protected override void OnMapChange( Map oldMap )
		{
			base.OnMapChange( oldMap );

			if ( !Server.Items.EssenceBase.ColorCitizen( this ) )
			{
				Server.Misc.MorphingTime.CheckMorph( this );
			}

			LoadSBInfo();
		}

		public virtual int GetHairHue()
		{
			return Utility.RandomHairHue();
		}

		public virtual void InitOutfit()
		{
			Server.Misc.IntelligentAction.DressUpMerchants( this );

			int hairHue = GetHairHue();

			Utility.AssignRandomHair( this, hairHue );

			if ( !Female )
			{
				FacialHairItemID = Utility.RandomList( 0, 8254, 8255, 8256, 8257, 8267, 8268, 8269 );
				FacialHairHue = hairHue;
			}
		}

		public virtual void Restock()
		{
			// Restock wares
			LoadSBInfo();
		}

		private Timer m_RefreshSelfTimer;

		public virtual void RefreshSelf()
		{
			if (m_RefreshSelfTimer != null)
			{
				m_RefreshSelfTimer.Stop();
				m_RefreshSelfTimer = null;
			}

			LastRefreshSelf = DateTime.Now;

			m_CoinsNeedReset = true;
			InvalidateProperties();
			UpdateBlackMarket();
			UpdateCoins();
		}

		public override bool OnBeforeDeath()
		{
			Server.Misc.MorphingTime.TurnToSomethingOnDeath( this );

			Mobile killer = this.LastKiller;

			if (killer is BaseCreature)
			{
				BaseCreature bc_killer = (BaseCreature)killer;
				if(bc_killer.Summoned)
				{
					if(bc_killer.SummonMaster != null)
						killer = bc_killer.SummonMaster;
				}
				else if(bc_killer.Controlled)
				{
					if(bc_killer.ControlMaster != null)
						killer=bc_killer.ControlMaster;
				}
				else if(bc_killer.BardProvoked)
				{
					if(bc_killer.BardMaster != null)
						killer=bc_killer.BardMaster;
				}
			}

			if ( killer is PlayerMobile )
			{
				killer.Criminal = true;
				killer.Kills = killer.Kills + 1;
				Server.Items.DisguiseTimers.RemoveDisguise( killer );
			}

			if ( !base.OnBeforeDeath() )
				return false;

			string bSay = "Help! Guards!";
			this.PublicOverheadMessage( MessageType.Regular, 0, false, string.Format ( bSay ) ); 

			return true;
		}

		public override bool IsEnemy( Mobile m )
		{
			if ( !IntelligentAction.GetMyEnemies( m, this, true ) )
				return false;

			if ( m.Region != this.Region && !(m is PlayerMobile) )
				return false;

			return true;
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			switch ( Utility.Random( 4 ))		   
			{
				case 0: Say("Leave this place!"); break;
				case 1: Say("" + defender.Name + ", we have heard of you!"); break;
				case 2: Say("We have been told to watch for you, " + defender.Name + "!"); break;
				case 3: Say("Guards, " + defender.Name + " is here!"); break;
			};
		}

		private static TimeSpan InventoryDecayTime = TimeSpan.FromHours( 2.0 );

		public virtual void VendorBuy( Mobile from )
		{
			if ( !IsActiveSeller )
				return;

			if ( !from.CheckAlive() )
				return;

			if ( !CheckVendorAccess( from ) )
			{
				this.Say( "I have no business with you." );
				return;
			}

			if ( DateTime.Now - LastRestockWares > RestockWaresDelay )
				Restock();

			var _ = GetOrCreateCoinPurse( this, from ); // Lazy generate

			UpdateBuyInfo();

			if ( this.RaceID == 0 && Utility.RandomBool() ){ this.PlaySound( this.Female ? 797 : 1069 ); }

			int count = 0;
			List<BuyItemState> list;
			IBuyItemInfo[] buyInfo = this.GetBuyInfo();
			IShopSellInfo[] sellInfo = this.GetSellInfo();

            list = new List<BuyItemState>( buyInfo.Length );
			Container cont = this.BuyPack;

			List<ObjectPropertyList> opls = null;

			for ( int idx = 0; idx < buyInfo.Length; idx++ )
			{
				IBuyItemInfo buyItem = (IBuyItemInfo)buyInfo[idx];

				int sales = 250;
					if ( this is Sage ){ sales = 500; }

				if ( buyItem.Amount <= 0 || list.Count >= sales )
					continue;

				// NOTE: Only GBI supported; if you use another implementation of IBuyItemInfo, this will crash
				GenericBuyInfo gbi = (GenericBuyInfo)buyItem;
				IEntity disp = gbi.GetDisplayEntity();

				var price = buyItem.Price;
				var name = buyItem.Name;

				if ( opls == null ) {
					opls = new List<ObjectPropertyList>();
				}

				if ( disp is Item ) {
					var item = (Item)disp;
					if (MutatePurchase(item, from, false))
					{
						price = GetMutationCost(buyItem.Price, from);
						name = string.Format("(Magical) {0}", item.Name);
					}
                    else
                    {
                        opls.Add(((Item)disp).PropertyList);
                    }
                } else if ( disp is Mobile ) {
					opls.Add( ( ( Mobile ) disp ).PropertyList );
				}

				list.Add( new BuyItemState( name, cont.Serial, disp == null ? (Serial)0x7FC0FFEE : disp.Serial, price, buyItem.Amount, buyItem.ItemID, buyItem.Hue ) );
				count++;
			}

			List<Item> playerItems = cont.Items;

			for ( int i = playerItems.Count - 1; i >= 0; --i )
			{
				if ( i >= playerItems.Count )
					continue;

				Item item = playerItems[i];

				if ( ( item.LastMoved + InventoryDecayTime ) <= DateTime.Now )
					item.Delete();
			}

			for ( int i = 0; i < playerItems.Count; ++i )
			{
				Item item = playerItems[i];

				int price = 0;
				string name = null;

				foreach ( IShopSellInfo ssi in sellInfo )
				{
					if ( ssi.IsSellable( item ) )
					{
						price = ssi.GetBuyPriceFor( item );
						name = ssi.GetNameFor( item );
						break;
					}
				}

				if ( name != null && list.Count < 250 )
				{
					list.Add( new BuyItemState( name, cont.Serial, item.Serial, price, item.Amount, item.ItemID, item.Hue ) );
					count++;

					if ( opls == null ) {
						opls = new List<ObjectPropertyList>();
					}

					opls.Add( item.PropertyList );
				}
			}

			//one (not all) of the packets uses a byte to describe number of items in the list.  Osi = dumb.
			//if ( list.Count > 255 )
			//	Console.WriteLine( "Vendor Warning: Vendor {0} has more than 255 buy items, may cause client errors!", this );

			int inventory = 0;

			if ( list.Count > 0 )
			{
				inventory++;

				list.Sort( new BuyItemStateComparer() );

				SendPacksTo( from );

				NetState ns = from.NetState;

				if ( ns == null )
					return;

				if ( ns.ContainerGridLines )
					from.Send( new VendorBuyContent6017( list ) );
				else
					from.Send( new VendorBuyContent( list ) );

				from.Send( new VendorBuyList( this, list ) );

				if ( ns.HighSeas )
					from.Send( new DisplayBuyListHS( this ) );
				else
					from.Send( new DisplayBuyList( this ) );

				from.Send( new MobileStatusExtended( from ) );//make sure their gold amount is sent

				if ( opls != null ) {
					for ( int i = 0; i < opls.Count; ++i ) {
						from.Send( opls[i] );
					}
				}

				if ( this is Sage )
					SayTo( from, "Have a look, but you cannot buy the priceless research items!" );
				else
					SayTo( from, 500186 ); // Greetings.  Have a look around.
			}

			if ( inventory < 1 )
				SayTo( from, "Sorry, but I have nothing for sale right now." );
		}

		public virtual void SendPacksTo( Mobile from )
		{
			Item pack = FindItemOnLayer( Layer.ShopBuy );

			if ( pack == null )
			{
				pack = new Backpack();
				pack.Layer = Layer.ShopBuy;
				pack.Movable = false;
				pack.Visible = false;
				AddItem( pack );
			}

			from.Send( new EquipUpdate( pack ) );

			pack = FindItemOnLayer( Layer.ShopSell );

			if ( pack != null )
				from.Send( new EquipUpdate( pack ) );

			pack = FindItemOnLayer( Layer.ShopResale );

			if ( pack == null )
			{
				pack = new Backpack();
				pack.Layer = Layer.ShopResale;
				pack.Movable = false;
				pack.Visible = false;
				AddItem( pack );
			}

			from.Send( new EquipUpdate( pack ) );
		}

		public virtual void VendorSell( Mobile from ) // Send list of items that are for sale
		{
			if ( BeggingPose(from) > 0 && !(this is PlayerBarkeeper) ) // LET US SEE IF THEY ARE BEGGING
			{
				from.Say( BeggingWords() );
			}

			if ( !IsActiveBuyer )
				return;

			if ( !from.CheckAlive() )
				return;

			if ( !CheckVendorAccess( from ) )
			{
				//Say( 501522 ); // I shall not treat with scum like thee!
				this.Say( "I have no business with you." );
				return;
			}

			int coins = GetOrCreateCoinPurse( this, from, true ); // Lazy generate
			if ( coins < 1 )
			{
				if ( from != null )
					SayTo( from, "I have no gold to barter with." );

				return;
			}

			if ( from != null )
				SayTo( from, true, "I have {0} gold to barter with.", coins );

			Container pack = from.Backpack;

			if ( pack != null )
			{
				IShopSellInfo[] info = GetSellInfo();

				Hashtable table = new Hashtable();

				foreach ( IShopSellInfo ssi in info )
				{
					Item[] items = pack.FindItemsByType( ssi.Types );

					foreach ( Item item in items )
					{
						LockableContainer parentcon = item.ParentEntity as LockableContainer;

						if ( item is Container && ( (Container)item ).Items.Count != 0 )
							continue;

						if ( parentcon != null && parentcon.Locked == true )
							continue;

						if ( item.IsStandardLoot() && item.Movable && ssi.IsSellable( item ) )
						{
							PlayerMobile pm = (PlayerMobile)from;

							int barter = (int)from.Skills[SkillName.Mercantile].Value;

							int GuildMember = 0;

							if ( barter < 100 && this.NpcGuild != NpcGuild.None && this.NpcGuild == pm.NpcGuild ){ barter = 100; GuildMember = 1; } // FOR GUILD MEMBERS

							if ( BeggingPose(from) > 0 && GuildMember == 0 && !(this is PlayerBarkeeper) ) // LET US SEE IF THEY ARE BEGGING
							{
								Titles.AwardKarma( from, -BeggingKarma( from ), true );
								barter = (int)from.Skills[SkillName.Begging].Value;
							}

							table[item] = new SellItemState( item, ssi.GetSellPriceFor( item, barter ), ssi.GetNameFor( item ) );
						}
					}
				}

				if ( table.Count > 0 )
				{
					SendPacksTo( from );

					if ( this.RaceID == 0 && Utility.RandomBool() ){ this.PlaySound( this.Female ? 797 : 1069 ); }

					from.Send( new VendorSellList( this, table ) );
				}
				else
				{
					Say( true, "You have nothing I would be interested in." );
				}
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( from.Blessed )
			{
				string sSay = "I cannot deal with you while you are in that state.";
				this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sSay, from.NetState);
				return false;
			}
			else if ( IntelligentAction.GetMyEnemies( from, this, false ) )
			{
				string sSay = "I don't think I should accept that from you.";
				this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sSay, from.NetState);
				return false;
			}
			else
			{
				PlayerMobile pm = (PlayerMobile)from;

				if ( dropped is BaseQuiver && ( this is Bowyer || this is ArcherGuildmaster ) )
				{
					if ( dropped.ItemID == 0x5770 )
					{
						dropped.ItemID = Utility.RandomList( 0x2B02, 0x2B03 );
					}
					else
					{
						dropped.ItemID = 0x5770;
					}

					this.PrivateOverheadMessage(MessageType.Regular, 1153, false, "Let me fix that quiver for you.", from.NetState);
					Effects.PlaySound(from.Location, from.Map, 0x55);
					return false;
				}
				else if ( ( dropped is DrakkhenEggRed || dropped is DrakkhenEggBlack ) && ( this is Druid || this is DruidTree || this is DruidGuildmaster ) )
				{
					if ( dropped is DrakkhenEggRed )
					{
						Server.Items.DrakkhenEggRed.ProcessDrakkhenEgg( from, this, dropped );
					}
					else if ( dropped is DrakkhenEggBlack )
					{
						Server.Items.DrakkhenEggBlack.ProcessDrakkhenEgg( from, this, dropped );
					}
				}
				else if ( dropped is GolemManual && ( this is Tinker || this is TinkerGuildmaster ) )
				{
					Server.Items.GolemManual.ProcessGolemBook( from, this, dropped );
				}
				else if ( dropped is OrbOfTheAbyss && ( this is Tinker || this is TinkerGuildmaster ) )
				{
					Server.Items.OrbOfTheAbyss.ChangeOrb( from, this, dropped );
				}
				else if ( dropped is RobotSchematics && ( this is Tinker || this is TinkerGuildmaster ) )
				{
					Server.Items.RobotSchematics.ProcessRobotBook( from, this, dropped );
				}
				else if ( dropped is AlienEgg && ( this is AnimalTrainer || this is Veterinarian ) )
				{
					Server.Items.AlienEgg.ProcessAlienEgg( from, this, dropped );
				}
				else if ( dropped is DragonEgg && ( this is AnimalTrainer || this is Veterinarian ) )
				{
					Server.Items.DragonEgg.ProcessDragonEgg( from, this, dropped );
				}
				else if ( dropped is DracolichSkull && ( this is NecromancerGuildmaster ) )
				{
					Server.Items.DracolichSkull.ProcessDracolichSkull( from, this, dropped );
				}
				else if ( dropped is DemonPrison && ( this is NecromancerGuildmaster || this is MageGuildmaster || this is Mage || this is Necromancer || this is Witches ) )
				{
					Server.Items.DemonPrison.ProcessDemonPrison( from, this, dropped );
				}

				if ( dropped is Cargo )
				{
					Server.Items.Cargo.GiveCargo( (Cargo)dropped, this, from );
				}
				else if ( dropped is Museums && this is VarietyDealer )
				{
					Server.Items.Museums.GiveAntique( (Museums)dropped, this, from );
				}
				else if ( Server.Multis.BaseBoat.isRolledCarpet( dropped ) && ( this is Tailor || this is TailorGuildmaster ) )
				{
					Item carpet = null;

					if ( dropped is MagicDockedCarpetA || dropped is MagicCarpetADeed ){ carpet = new MagicCarpetBDeed(); }
					else if ( dropped is MagicDockedCarpetB || dropped is MagicCarpetBDeed ){ carpet = new MagicCarpetCDeed(); }
					else if ( dropped is MagicDockedCarpetC || dropped is MagicCarpetCDeed ){ carpet = new MagicCarpetDDeed(); }
					else if ( dropped is MagicDockedCarpetD || dropped is MagicCarpetDDeed ){ carpet = new MagicCarpetEDeed(); }
					else if ( dropped is MagicDockedCarpetE || dropped is MagicCarpetEDeed ){ carpet = new MagicCarpetFDeed(); }
					else if ( dropped is MagicDockedCarpetF || dropped is MagicCarpetFDeed ){ carpet = new MagicCarpetGDeed(); }
					else if ( dropped is MagicDockedCarpetG || dropped is MagicCarpetGDeed ){ carpet = new MagicCarpetHDeed(); }
					else if ( dropped is MagicDockedCarpetH || dropped is MagicCarpetHDeed ){ carpet = new MagicCarpetIDeed(); }
					else if ( dropped is MagicDockedCarpetI || dropped is MagicCarpetIDeed ){ carpet = new MagicCarpetADeed(); }

					dropped.Delete();
					carpet.Hue = dropped.Hue;
					from.AddToBackpack( carpet );
					SayTo(from, "I altered your magic carpet.");
					Effects.PlaySound(from.Location, from.Map, 0x248);
				}
				else if ( dropped is Gold && dropped.Amount == 1000 && ( this is Mapmaker || this is CartographersGuildmaster ) )
				{
					MapWorld wMap = new MapWorld();

					wMap.WorldMap = this.Land;

					if ( wMap.WorldMap == Land.Atlantis )
						wMap.WorldMap = Land.Sosaria;
					else if ( wMap.WorldMap == Land.Luna )
						wMap.WorldMap = Land.Sosaria;
					else if ( wMap.WorldMap == Land.SkaraBrae )
						wMap.WorldMap = Land.Sosaria;

					wMap.Name = "world map";
					wMap.ColorText3 = Lands.LandName( wMap.WorldMap );
					wMap.ColorHue3 = "63B95F";

					from.AddToBackpack ( wMap );

					AddToCoinPurse( from, dropped.Amount );
					string sMessage = "Thank you. Here is your world map.";
					this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sMessage, from.NetState);
					dropped.Delete();
					return true;
				}
				else if (	( dropped is DDCopper || dropped is DDSilver ) && 
							( this is Minter || this is Banker )	)
				{
					int nRate = 5;
					string sCoin = "silver";
					if ( dropped is DDCopper ){ nRate = 10; sCoin = "copper";}

					int nCoins = dropped.Amount;
					int nGold = (int)Math.Floor((decimal)(dropped.Amount / nRate));
					int nChange = dropped.Amount - ( nGold * nRate );

					string sMessage = "Sorry, you do not have enough here to exchange for even a single gold coin.";

					if ( ( nGold > 0 ) && ( nChange > 0 ) )
					{
						sMessage = "Here is " + nGold.ToString() + " gold for you, and " + nChange.ToString() + " " + sCoin + " back in change.";
						from.AddToBackpack ( new Gold( nGold ) );
					}
					else if ( nGold > 0 )
					{
						sMessage = "Here is " + nGold.ToString() + " gold for you.";
						from.AddToBackpack ( new Gold( nGold ) );
					}

					this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sMessage, from.NetState);

					if ( ( nChange > 0 ) && ( dropped is DDCopper ) ){ from.AddToBackpack ( new DDCopper( nChange ) ); }
					else if ( ( nChange > 0 ) && ( dropped is DDSilver ) ){ from.AddToBackpack ( new DDSilver( nChange ) ); }

					dropped.Delete();
					return true;
				}
				else if ( ( dropped is DDXormite ) && ( this is Minter || this is Banker ) )
				{
					int nGold = dropped.Amount * 3;

					string sMessage = "Here is " + nGold.ToString() + " gold for you.";
					from.AddToBackpack ( new Gold( nGold ) );

					this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sMessage, from.NetState);

					dropped.Delete();
					return true;
				}
				else if ( ( dropped is Crystals ) && ( this is Minter || this is Banker ) )
				{
					int nGold = dropped.Amount * 5;

					string sMessage = "Here is " + nGold.ToString() + " gold for you.";
					from.AddToBackpack ( new Gold( nGold ) );

					this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sMessage, from.NetState);

					dropped.Delete();
					return true;
				}
				else if ( ( dropped is DDJewels ) && ( this is Minter || this is Banker ) )
				{
					int nGold = dropped.Amount * 2;

					string sMessage = "Here is " + nGold.ToString() + " gold for you.";
					from.AddToBackpack ( new Gold( nGold ) );

					this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sMessage, from.NetState);

					dropped.Delete();
					return true;
				}
				else if ( ( dropped is DDGemstones ) && ( this is Minter || this is Banker ) )
				{
					int nGold = dropped.Amount * 2;

					string sMessage = "Here is " + nGold.ToString() + " gold for you.";
					from.AddToBackpack ( new Gold( nGold ) );

					this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sMessage, from.NetState);

					dropped.Delete();
					return true;
				}
				else if ( ( dropped is DDGoldNuggets ) && ( this is Minter || this is Banker ) )
				{
					int nGold = dropped.Amount;

					string sMessage = "Here is " + nGold.ToString() + " gold for you.";
					from.AddToBackpack ( new Gold( nGold ) );

					this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sMessage, from.NetState);

					dropped.Delete();
					return true;
				}
				else if ( ( dropped is HenchmanFighterItem || dropped is HenchmanArcherItem || dropped is HenchmanWizardItem ) && ( this is TavernKeeper || this is Barkeeper ) )
				{
					int fairTrade = 1;
					string sMessage = "";
					switch ( Utility.RandomMinMax( 0, 7 ) )
					{
						case 0:	sMessage = "So, this follower is not working out for you?"; break;
						case 1:	sMessage = "Looking for a replacement henchman eh?"; break;
						case 2:	sMessage = "Well...this one is looking for fame and fortune."; break;
						case 3:	sMessage = "Maybe this one will be a better fit in your group."; break;
						case 4:	sMessage = "Not all relationships work out."; break;
						case 5:	sMessage = "At you least you parted ways amiably."; break;
						case 6:	sMessage = "This one has been hanging out around here."; break;
						case 7:	sMessage = "This one also seeks great treasure.";		break;
					}
					if ( dropped is HenchmanFighterItem )
					{
						HenchmanFighterItem myFollower = (HenchmanFighterItem)dropped;
						if ( myFollower.HenchDead > 0 ){ fairTrade = 0; } else
						{
							HenchmanFighterItem newFollower = new HenchmanFighterItem();
							newFollower.HenchTimer = myFollower.HenchTimer;
							newFollower.HenchBandages = myFollower.HenchBandages;
							from.AddToBackpack ( newFollower );
						}
					}
					else if ( dropped is HenchmanWizardItem )
					{
						HenchmanWizardItem myFollower = (HenchmanWizardItem)dropped;
						if ( myFollower.HenchDead > 0 ){ fairTrade = 0; } else
						{
							HenchmanWizardItem newFollower = new HenchmanWizardItem();
							newFollower.HenchTimer = myFollower.HenchTimer;
							newFollower.HenchBandages = myFollower.HenchBandages;
							from.AddToBackpack ( newFollower );
						}
					}
					else if ( dropped is HenchmanArcherItem )
					{
						HenchmanArcherItem myFollower = (HenchmanArcherItem)dropped;
						if ( myFollower.HenchDead > 0 ){ fairTrade = 0; } else
						{
							HenchmanArcherItem newFollower = new HenchmanArcherItem();
							newFollower.HenchTimer = myFollower.HenchTimer;
							newFollower.HenchBandages = myFollower.HenchBandages;
							from.AddToBackpack ( newFollower );
						}
					}
					if ( fairTrade == 1 )
					{
						this.PrivateOverheadMessage(MessageType.Regular, 1153, false, sMessage, from.NetState);
						dropped.Delete();
						return true;
					}
					else { this.PrivateOverheadMessage(MessageType.Regular, 1153, false, "This is not a graveyard! Bury them somewhere else!", from.NetState); }
				}
				else if ( dropped is BookBox && ( this is Mage || this is KeeperOfChivalry || this is Witches || this is Necromancer || this is MageGuildmaster ) )
				{
					Container pack = (Container)dropped;
						List<Item> items = new List<Item>();
						foreach (Item item in pack.Items)
						{
							items.Add(item);
						}
						foreach (Item item in items)
						{
							from.AddToBackpack ( item );
						}
					this.PrivateOverheadMessage(MessageType.Regular, 1153, false, "The curse has been lifted from the books.", from.NetState);
					dropped.Delete();
					return true;
				}
				else if ( dropped is JewelryBox && ( this is Jeweler ) )
				{
					Container pack = (Container)dropped;
						List<Item> items = new List<Item>();
						foreach (Item item in pack.Items)
						{
							items.Add(item);
						}
						foreach (Item item in items)
						{
							from.AddToBackpack ( item );
						}
					
					Item junk = ((JewelryBox)dropped).TryGetJunk();
					if (junk != null)
						from.AddToBackpack ( junk );
					
					string message = "";
					if (items.Any())
						message = "I've untangled your jewelry.";
					if (junk != null)
						message += " Some of it was unrecoverable.";

					if (!string.IsNullOrWhiteSpace(message))
						this.PrivateOverheadMessage(MessageType.Regular, 1153, false, message, from.NetState);

					dropped.Delete();
					return true;
				}
				else if ( dropped is CurseItem && ( this is Mage || this is KeeperOfChivalry || this is Witches || this is Necromancer || this is MageGuildmaster ) )
				{
					Container pack = (Container)dropped;
						List<Item> items = new List<Item>();
						foreach (Item item in pack.Items)
						{
							items.Add(item);
						}
						foreach (Item item in items)
						{
							from.AddToBackpack ( item );
						}
					string curseName = dropped.Name;
						if ( curseName == ""){ curseName = "item"; }
					this.PrivateOverheadMessage(MessageType.Regular, 1153, false, "The curse has been lifted from the " + curseName + ".", from.NetState);
					dropped.Delete();
					return true;
				}
				else if ( ( dropped is SewageItem || dropped is SlimeItem ) && ( this is TavernKeeper || this is Barkeeper || this is Waiter ) )
				{
					Container pack = (Container)dropped;
						List<Item> items = new List<Item>();
						foreach (Item item in pack.Items)
						{
							items.Add(item);
						}
						foreach (Item item in items)
						{
							from.AddToBackpack ( item );
						}
					this.PrivateOverheadMessage(MessageType.Regular, 1153, false, "The item has been cleaned.", from.NetState);
					dropped.Delete();
					return true;
				}
				else if ( dropped is WeededItem && ( this is Alchemist || this is Herbalist ) )
				{
					Container pack = (Container)dropped;
						List<Item> items = new List<Item>();
						foreach (Item item in pack.Items)
						{
							items.Add(item);
						}
						foreach (Item item in items)
						{
							from.AddToBackpack ( item );
						}
					this.PrivateOverheadMessage(MessageType.Regular, 1153, false, "The weeds have been removed.", from.NetState);
					dropped.Delete();
					return true;
				}

				//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

				/* TODO: Thou art giving me? and fame/karma for gold gifts */
			}

			return base.OnDragDrop( from, dropped );
		}

		private GenericBuyInfo LookupDisplayObject( object obj )
		{
			IBuyItemInfo[] buyInfo = this.GetBuyInfo();

			for ( int i = 0; i < buyInfo.Length; ++i ) {
				GenericBuyInfo gbi = (GenericBuyInfo)buyInfo[i];

				if ( gbi.GetDisplayEntity() == obj )
					return gbi;
			}

			return null;
		}

        private void ProcessSinglePurchase( int pricePerItem, BuyItemResponse buy, IBuyItemInfo bii, List<BuyItemResponse> validBuy, ref int controlSlots, ref bool fullPurchase, ref int totalCost )
		{
			int amount = buy.Amount;

			if ( amount > bii.Amount )
				amount = bii.Amount;

			if ( amount <= 0 )
				return;

			int slots = bii.ControlSlots * amount;

			if ( controlSlots >= slots )
			{
				controlSlots -= slots;
			}
			else
			{
				fullPurchase = false;
				return;
			}

            totalCost += pricePerItem * amount;

			validBuy.Add( buy );
		}

		private void ProcessValidPurchase( int amount, IBuyItemInfo bii, Mobile buyer, Container cont )
		{
			if ( amount > bii.Amount )
				amount = bii.Amount;

			if ( amount < 1 )
				return;

			bii.Amount -= amount;

			IEntity o = bii.GetEntity();

			if ( o is Item )
			{
				Item item = (Item)o;

				if ( item.Stackable )
				{
					item.Amount = amount;

					if ( cont == null || !cont.TryDropItem( buyer, item, false ) )
						item.MoveToWorld( buyer.Location, buyer.Map );
				}
				else
				{
					item.Amount = 1;
					MutatePurchase(item, buyer, true);
					item.Purchased = true;

					if ( cont == null || !cont.TryDropItem( buyer, item, false ) )
						item.MoveToWorld( buyer.Location, buyer.Map );

					for ( int i = 1; i < amount; i++ )
					{
						item = bii.GetEntity() as Item;
						MutatePurchase(item, buyer, true);
						item.Purchased = true;
							
						if ( item is Spear ){ item.ItemID = 0xF62; }
						else if ( item is Club ){ item.ItemID = 0x13B4; }
						else if ( item is Cleaver ){ item.ItemID = 0xEC3; }

						if ( item != null )
						{
							item.Amount = 1;

							if ( cont == null || !cont.TryDropItem( buyer, item, false ) )
								item.MoveToWorld( buyer.Location, buyer.Map );
						}
					}
				}
			}
			else if ( o is Mobile )
			{
				Mobile m = (Mobile)o;

				m.Direction = (Direction)Utility.Random( 8 );
				m.MoveToWorld( buyer.Location, buyer.Map );
				m.PlaySound( m.GetIdleSound() );

				if ( m is BaseCreature )
				{
					( (BaseCreature)m ).SetControlMaster( buyer );
					( (BaseCreature)m ).Tamable = true;
					( (BaseCreature)m ).MinTameSkill = 29.1;
				}

				for ( int i = 1; i < amount; ++i )
				{
					m = bii.GetEntity() as Mobile;

					if ( m != null )
					{
						m.Direction = (Direction)Utility.Random( 8 );
						m.MoveToWorld( buyer.Location, buyer.Map );

						if ( m is BaseCreature )
						{
							( (BaseCreature)m ).SetControlMaster( buyer );
							( (BaseCreature)m ).Tamable = true;
							( (BaseCreature)m ).MinTameSkill = 29.1;
						}
					}
				}
			}
		}

		private bool MutatePurchase(Item item, Mobile buyer, bool doMutation)
		{
			if ( this is Sage ) return false;

			if ( ( 
				item is BaseWeapon
				|| item is BaseArmor
				|| item is BaseTrinket
				|| item is BaseQuiver
				|| item is BaseHat
				|| item is BaseClothing
				|| item is BaseInstrument
				|| item is Spellbook
			) == false ) return false;
			if ( item is BaseTool ) return false;

			var tier = GetPlayerTier(buyer);
			switch(tier)
			{
				case 1: if (doMutation) BaseRunicTool.ApplyAttributes(item, 2, 2, 5, 25); break; // 5k - After a few Normal dungeons
				case 2: if (doMutation) BaseRunicTool.ApplyAttributes(item, 2, 2, 5, 30); break; // 10k - After a lot of Normal dungeons?
				case 3: if (doMutation) BaseRunicTool.ApplyAttributes(item, 2, 3, 15, 40); break; // 15k - Starting Difficult dungeons
				case 4: if (doMutation) BaseRunicTool.ApplyAttributes(item, 2, 3, 20, 50); break; // 20k - After a lot of Difficult
				case 5: if (doMutation) BaseRunicTool.ApplyAttributes(item, 2, 4, 25, 50); break; // 25k - Begin Challenging
				case 6: if (doMutation) BaseRunicTool.ApplyAttributes(item, 3, 4, 40, 70); break; // 30k - Lots of Challenging
				// Hard
				// Deadly
				default: return false;
			}

			// Always basic resources.
			// This gives Players the opportunity to enhance and flip items.
			// Non-basic resource drops are 

			return true;
		}

		public int GetMutationCost(int price, Mobile buyer)
		{
			return price * (1 + GetPlayerTier(buyer));
		}

		private int GetPlayerTier(Mobile player) // Max of 6
		{
			var karma = Math.Min(15000, Math.Abs(player.Karma));
			var fame = Math.Min(15000, player.Fame);

			var level = (fame + karma) / 5000;

			return level;
		}

        public virtual bool OnBuyItems( Mobile buyer, List<BuyItemResponse> list )
		{
			if ( !IsActiveSeller )
				return false;

			if ( !buyer.CheckAlive() )
				return false;

			if ( !CheckVendorAccess( buyer ) )
			{
				//Say( 501522 ); // I shall not treat with scum like thee!
				this.Say( "I have no business with you." );
				return false;
			}

			UpdateBuyInfo();

			IBuyItemInfo[] buyInfo = this.GetBuyInfo();
			IShopSellInfo[] info = GetSellInfo();
			int totalCost = 0;
            List<BuyItemResponse> validBuy = new List<BuyItemResponse>( list.Count );
			Container cont;
			bool bought = false;
			bool fromBank = false;
			bool fullPurchase = true;
			int controlSlots = buyer.FollowersMax - buyer.Followers;
			bool tryGettingArty = false;

			foreach ( BuyItemResponse buy in list )
			{
				Serial ser = buy.Serial;
				int amount = buy.Amount;

				if ( ser.IsItem )
				{
					Item item = World.FindItem( ser );

					if ( item.ArtifactLevel > 0 )
					{
						item = null;
						tryGettingArty = true;
					}

					if ( item == null )
						continue;

					GenericBuyInfo gbi = LookupDisplayObject( item );

					if ( gbi != null )
					{
						int price = MutatePurchase(item, buyer, false) ? GetMutationCost(gbi.Price, buyer) : gbi.Price;
                        ProcessSinglePurchase( price, buy, gbi, validBuy, ref controlSlots, ref fullPurchase, ref totalCost );
					}
					else if ( item != this.BuyPack && item.IsChildOf( this.BuyPack ) )
					{
						if ( amount > item.Amount )
							amount = item.Amount;

						if ( amount <= 0 )
							continue;

						foreach ( IShopSellInfo ssi in info )
						{
							if ( ssi.IsSellable( item ) )
							{
								if ( ssi.IsResellable( item ) )
								{
									totalCost += ssi.GetBuyPriceFor( item ) * amount;
									validBuy.Add( buy );
									break;
								}
							}
						}
					}
				}
				else if ( ser.IsMobile )
				{
					Mobile mob = World.FindMobile( ser );

					if ( mob == null )
						continue;

					GenericBuyInfo gbi = LookupDisplayObject( mob );

					if ( gbi != null )
						ProcessSinglePurchase( gbi.Price, buy, gbi, validBuy, ref controlSlots, ref fullPurchase, ref totalCost );
				}
			}//foreach

			if ( tryGettingArty )
			{
				SayTo( buyer, true, "No! Those are research items and not actual artifacts!" );
				this.PlaySound( this.Female ? 802 : 1074 );
			}
			else if ( fullPurchase && validBuy.Count == 0 )
				SayTo( buyer, 500190 ); // Thou hast bought nothing!
			else if ( validBuy.Count == 0 )
				SayTo( buyer, 500187 ); // Your order cannot be fulfilled, please try again.

			if ( validBuy.Count == 0 || tryGettingArty )
				return false;

			bought = ( buyer.AccessLevel >= AccessLevel.GameMaster );

			cont = buyer.Backpack;
			if ( !bought && cont != null )
			{
				if ( cont.ConsumeTotal( typeof( Gold ), totalCost ) )
					bought = true;
				else if ( totalCost < 2000 )
					SayTo( buyer, 500192 );//Begging thy pardon, but thou casnt afford that.
			}

			if ( !bought && totalCost >= 2000 )
			{
				cont = buyer.FindBankNoCreate();
				if ( cont != null && cont.ConsumeTotal( typeof( Gold ), totalCost ) )
				{
					bought = true;
					fromBank = true;
				}
				else
				{
					SayTo( buyer, 500191 ); //Begging thy pardon, but thy bank account lacks these funds.
				}
			}

			if ( !bought )
				return false;
			else
				buyer.PlaySound( 0x32 );

			cont = buyer.Backpack;
			if ( cont == null )
				cont = buyer.BankBox;

			int totalItems = 0;
			foreach ( BuyItemResponse buy in validBuy )
			{
				Serial ser = buy.Serial;
				int amount = buy.Amount;

				if ( amount < 1 )
					continue;

				if ( ser.IsItem )
				{
					Item item = World.FindItem( ser );

					if ( item == null )
						continue;
					
					totalItems += amount;

					GenericBuyInfo gbi = LookupDisplayObject( item );

					if ( gbi != null )
					{
						ProcessValidPurchase( amount, gbi, buyer, cont );
					}
					else
					{
						if ( amount > item.Amount )
							amount = item.Amount;

						foreach ( IShopSellInfo ssi in info )
						{
							if ( ssi.IsSellable( item ) )
							{
								if ( ssi.IsResellable( item ) )
								{
									Item buyItem;
									if ( amount >= item.Amount )
									{
										buyItem = item;
									}
									else
									{
										buyItem = Mobile.LiftItemDupe( item, item.Amount - amount );

										if ( buyItem == null )
											buyItem = item;
									}

									if ( buyItem is Spear ){ buyItem.ItemID = 0xF62; }
									else if ( buyItem is Club ){ buyItem.ItemID = 0x13B4; }
									else if ( buyItem is Cleaver ){ buyItem.ItemID = 0xEC3; }

									if ( cont == null || !cont.TryDropItem( buyer, buyItem, false ) )
										buyItem.MoveToWorld( buyer.Location, buyer.Map );

									break;
								}
							}
						}
					}
				}
				else if ( ser.IsMobile )
				{
					Mobile mob = World.FindMobile( ser );

					if ( mob == null )
						continue;

					totalItems += amount;

					GenericBuyInfo gbi = LookupDisplayObject( mob );

					if ( gbi != null )
						ProcessValidPurchase( amount, gbi, buyer, cont );
				}
			}//foreach

			AddToCoinPurse( buyer, totalCost );

			// Chance to learn mercantile
			int skillChecks = totalCost / 500; // One check per 500 gold
			SkillUtilities.DoSkillChecks(buyer, SkillName.Mercantile, skillChecks, totalItems);

			if ( fullPurchase )
			{
				if ( buyer.AccessLevel >= AccessLevel.GameMaster )
					SayTo( buyer, true, "I would not presume to charge thee anything.  Here are the goods you requested." );
				else if ( fromBank )
					SayTo( buyer, true, "The total of thy purchase is {0} gold, which has been withdrawn from your bank account.  My thanks for the patronage.", totalCost );
				else
					SayTo( buyer, true, "The total of thy purchase is {0} gold.  My thanks for the patronage.", totalCost );
			}
			else
			{
				if ( buyer.AccessLevel >= AccessLevel.GameMaster )
					SayTo( buyer, true, "I would not presume to charge thee anything.  Unfortunately, I could not sell you all the goods you requested." );
				else if ( fromBank )
					SayTo( buyer, true, "The total of thy purchase is {0} gold, which has been withdrawn from your bank account.  My thanks for the patronage.  Unfortunately, I could not sell you all the goods you requested.", totalCost );
				else
					SayTo( buyer, true, "The total of thy purchase is {0} gold.  My thanks for the patronage.  Unfortunately, I could not sell you all the goods you requested.", totalCost );
			}

			return true;
		}

		public virtual bool CheckVendorAccess( Mobile from )
		{
			PlayerMobile pm = (PlayerMobile)from;

			if ( AccessLevel.Counselor <= pm.AccessLevel ) // Staff can always buy
				return true;

			if ( this is BaseGuildmaster && this.NpcGuild != pm.NpcGuild ) // ONLY GUILD MEMBERS CAN BUY FROM GUILD MASTERS
				return false;

			if ( !from.Alive || from.Blessed )
				return false;

			if ( !(this.CanSee( from )) || !(this.InLOS( from )) )
				return false;

			bool publicRegion = false;

			if ( from.Region.IsPartOf( typeof( PublicRegion ) ) )
				publicRegion = true;

			if ( from.Region.IsPartOf( typeof( StartRegion ) ) )
				publicRegion = true;

			if ( from.Region.IsPartOf( typeof( SafeRegion ) ) )
				publicRegion = true;

			if ( from.Region.IsPartOf( typeof( ProtectedRegion ) ) )
				publicRegion = true;

			if ( from.Region.IsPartOf( typeof( NecromancerRegion ) ) && GetPlayerInfo.EvilPlayer( from ) )
				publicRegion = true;

			if ( !publicRegion && ( from.Criminal || from.Kills > 0 ) )
				return false;

			if ( !publicRegion && from is PlayerMobile && ((PlayerMobile)from).Fugitive == 1 )
				return false;

			if ( IntelligentAction.GetMyEnemies( from, this, false ) )
				return false;

			return true;
		}

        public virtual bool OnSellItems( Mobile seller, List<SellItemResponse> list )
		{
			if ( !IsActiveBuyer )
				return false;

			if ( !seller.CheckAlive() )
				return false;

			if ( !CheckVendorAccess( seller ) )
			{
				this.Say( "I have no business with you." );
				return false;
			}

			IShopSellInfo[] info = GetSellInfo();
			IBuyItemInfo[] buyInfo = this.GetBuyInfo();
			int GiveGold = 0;
			int Sold = 0;
			int SoldPrice = 0;
			PlayerMobile pm = (PlayerMobile)seller;
			int GuildMember = 0;
			Container cont;

			bool isInGuild = NpcGuild != NpcGuild.None && NpcGuild == pm.NpcGuild;
			bool isBegging = BeggingPose(seller) > 0 && !(this is PlayerBarkeeper);

			int SoldBarter = (int)seller.Skills[SkillName.Mercantile].Value;
			if ( SoldBarter < 100 && isInGuild ){ SoldBarter = 100; GuildMember = 1; } // FOR GUILD MEMBERS

			if ( isBegging && GuildMember == 0 ) // LET US SEE IF THEY ARE BEGGING
				SoldBarter = (int)seller.Skills[SkillName.Begging].Value;

			foreach ( SellItemResponse resp in list )
			{
				if ( resp.Item.RootParent != seller || resp.Amount <= 0 || !resp.Item.IsStandardLoot() || !resp.Item.Movable || ( resp.Item is Container && ( (Container)resp.Item ).Items.Count != 0 ) )
					continue;

				foreach ( IShopSellInfo ssi in info )
				{
					if ( ssi.IsSellable( resp.Item ) )
					{
						Sold++;
						SoldPrice += ssi.GetSellPriceFor( resp.Item, SoldBarter ) * resp.Amount;
						break;
					}
				}
			}

			if ( Sold > MaxSell )
			{
				SayTo( seller, true, "You may only sell {0} items at a time!", MaxSell );
				return false;
			}

			if ( Sold == 0 )
			{
				return true;
			}
			
			var coins = TryGetCoinPurse( seller );
			if ( coins == null ) return false; // Guard

			if ( !MySettings.S_RichMerchants && !MySettings.S_UseRemainingGold && !pm.Preferences.IgnoreVendorGoldSafeguard && SoldPrice > coins )
			{
				SayTo( seller, true, "Sorry, but I only have {0} gold to barter with.", coins );
				return false;
			}

			seller.PlaySound( 0x32 );

			foreach ( SellItemResponse resp in list )
			{
				if ( resp.Item.RootParent != seller || resp.Amount <= 0 || !resp.Item.IsStandardLoot() || !resp.Item.Movable || ( resp.Item is Container && ( (Container)resp.Item ).Items.Count != 0 ) )
					continue;

				if ( isBegging ) // LET US SEE IF THEY ARE BEGGING
				{
					Titles.AwardKarma( seller, -BeggingKarma( seller ), true );
				}

				foreach ( IShopSellInfo ssi in info )
				{
					if ( ssi.IsSellable( resp.Item ) )
					{
						int amount = resp.Amount;

						if ( amount > resp.Item.Amount )
							amount = resp.Item.Amount;

						if ( ssi.IsResellable( resp.Item ) )
						{
							bool found = false;

							foreach ( IBuyItemInfo bii in buyInfo )
							{
								if ( bii.Restock( resp.Item, amount ) )
								{
									resp.Item.Consume( amount );
									found = true;

									break;
								}
							}

							if ( !found )
							{
								cont = this.BuyPack;

								if ( amount < resp.Item.Amount )
								{
									Item item = Mobile.LiftItemDupe( resp.Item, resp.Item.Amount - amount );

									if ( item != null )
									{
										item.SetLastMoved();
										cont.DropItem( item );
									}
									else
									{
										resp.Item.SetLastMoved();
										cont.DropItem( resp.Item );
									}
								}
								else
								{
									resp.Item.SetLastMoved();
									cont.DropItem( resp.Item );
								}
							}
						}
						else
						{
							if ( amount < resp.Item.Amount )
								resp.Item.Amount -= amount;
							else
								resp.Item.Delete();
						}

						int barter = (int)seller.Skills[SkillName.Mercantile].Value;
						if ( barter < 100 && isInGuild ){ barter = 100; GuildMember = 1; } // FOR GUILD MEMBERS

						if ( isBegging && GuildMember == 0 ) // LET US SEE IF THEY ARE BEGGING
						{
							seller.CheckSkill( SkillName.Begging, 0, 125 );
							barter = (int)seller.Skills[SkillName.Begging].Value;
						}

						GiveGold += ssi.GetSellPriceFor( resp.Item, barter ) * amount;
						break;
					}
				}
			}

			if ( GiveGold > 0 )
			{
				if ( !MySettings.S_RichMerchants && GiveGold > coins )
				{
					GiveGold = coins.Value;
					SayTo( seller, true, "I give you my remaining {0} gold.", coins );
				}

				// Deduct the gold
				AddToCoinPurse( seller, 0 - GiveGold );

				// Chance to learn mercantile
				if ( !isBegging && !isInGuild )
				{
					// 500 / 1000 / 2000 / 3000 / etc
					int skillChecks = GiveGold < 500 ? 0 : 1 + ( GiveGold / 1000 );
					SkillUtilities.DoSkillChecks(seller, SkillName.Mercantile, skillChecks, Sold);
				}

				while ( GiveGold > 60000 )
				{
					seller.AddToBackpack( new Gold( 60000 ) );
					GiveGold -= 60000;
				}

				seller.AddToBackpack( new Gold( GiveGold ) );

				seller.PlaySound( 0x0037 );//Gold dropping sound
			}

			return true;
		}

		public override void SendPropertiesTo( Mobile from )
		{
			if ( from is PlayerMobile && ((PlayerMobile)from).Preferences.SuppressVendorTooltip )
				return;

			from.Send( PropertyList );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( GetPlayerInfo.GetNPCGuild( this ) != null )
				list.Add( 1072172, "{0}\t{1}", "51C273", GetPlayerInfo.GetNPCGuild( this ) );

			if ( IsActiveBuyer && !MySettings.S_RichMerchants )
			{
				if ( !m_CoinsNeedReset )
					list.Add( 1072173, "{0}\t{1}", "FBFF00", "Traded Recently" );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)3 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version == 1 )
			{
				List<SBInfo> sbInfos = this.SBInfos;
				int index;
				while ( ( index = reader.ReadEncodedInt() ) > 0 )
				{
					int doubled = reader.ReadEncodedInt();
					if ( sbInfos != null )
						index -= 1;
				}
			}

			if ( version == 2 )
				AI = AIType.AI_Vendor;

			if ( IsParagon )
				IsParagon = false;

			if ( NameHue == 0x35 )
				NameHue = -1;

			Timer.DelayCall(TimeSpan.FromSeconds(10.0), () =>
			{
				if (Deleted) return;

				LoadSBInfo();
				if (this is PlayerBarkeeper) return;

				RefreshSelf();
			});
		}

		public static Mobile m_Merchant;

		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive && IsActiveVendor )
			{
				bool buysThings = true;

                if ( !m_CoinsNeedReset && !MySettings.S_RichMerchants && !HasCoinsForPurchase( from ))
					buysThings = false;
				else if ( !IsActiveBuyer )
					buysThings = false;
				
				if ( IsActiveSeller && CheckVendorAccess( from ) )
					list.Add( new VendorBuyEntry( from, this ) );

				if ( buysThings && CheckVendorAccess( from ) )
					list.Add( new VendorSellEntry( from, this ) );

				if ( IsBlackMarket && this.BankBox.TotalItems > 0 && MyServerSettings.BlackMarket() && CheckVendorAccess( from ) )
					list.Add( new BlackMarketEntry( from, this ) );
			}

			if (
				( from.Skills[SkillName.Forensics].Value >= 50 && ( this is Undertaker || this is Witches || this is Necromancer || this is NecromancerGuildmaster ) ) || 
				( from.Skills[SkillName.Druidism].Value >= 50 && ( this is Herbalist || this is DruidTree || this is Druid || this is DruidGuildmaster ) ) || 
				( from.Skills[SkillName.Alchemy].Value >= 50 && ( this is Alchemist || this is AlchemistGuildmaster ) ) || 
				( from.Skills[SkillName.Blacksmith].Value >= 50 && ( this is Blacksmith || this is BlacksmithGuildmaster ) ) || 
				( from.Skills[SkillName.Bowcraft].Value >= 50 && ( this is Bowyer || this is ArcherGuildmaster ) ) || 
				( from.Skills[SkillName.Carpentry].Value >= 50 && ( this is Carpenter || this is CarpenterGuildmaster ) ) || 
				( from.Skills[SkillName.Cartography].Value >= 50 && ( this is Mapmaker || this is CartographersGuildmaster ) ) || 
				( from.Skills[SkillName.Cooking].Value >= 50 && ( this is Cook || this is Baker || this is CulinaryGuildmaster ) ) || 
				( from.Skills[SkillName.Inscribe].Value >= 50 && ( this is Scribe || this is Sage || this is LibrarianGuildmaster ) ) || 
				( from.Skills[SkillName.Tailoring].Value >= 50 && ( this is Weaver || this is Tailor || this is LeatherWorker || this is TailorGuildmaster ) ) || 
				( from.Skills[SkillName.Tinkering].Value >= 50 && ( this is Tinker || this is TinkerGuildmaster ) ) 
			)
			{
				if ( CheckVendorAccess( from ) )
					list.Add( new SetupShoppeEntry( from, this ) );
			}

			if ( VendorRepairs( this ) && CheckVendorAccess( from ) )
				list.Add( new RepairEntry( from, this ) );

			if ( RelicFunctions.VendorDoesID( this ) && CheckVendorAccess( from ) )
				list.Add( new IdentifyEntry( from, this ) );

			if ( MagicRecharge( this ) && CheckVendorAccess( from ) )
				list.Add( new RechargeEntry( from, this ) );

			base.AddCustomContextEntries( from, list );
		}

		public virtual IShopSellInfo[] GetSellInfo()
		{
			return (IShopSellInfo[])m_ArmorSellInfo.ToArray( typeof( IShopSellInfo ) );
		}

		public virtual IBuyItemInfo[] GetBuyInfo()
		{
			return (IBuyItemInfo[])m_ArmorBuyInfo.ToArray( typeof( IBuyItemInfo ) );
		}

		public static bool VendorRepairs( Mobile m )
		{
			if (
				m is ArcherGuildmaster || 
				m is Armorer || 
				m is Bard || 
				m is BardGuildmaster || 
				m is Blacksmith || 
				m is BlacksmithGuildmaster || 
				m is Bowyer || 
				m is Carpenter || 
				m is CarpenterGuildmaster || 
				m is Furtrader || 
				m is Garth || 
				m is IronWorker || 
				m is LeatherWorker || 
				m is Lumberjack || 
				m is Ranger || 
				m is RangerGuildmaster || 
				m is Tailor || 
				m is TailorGuildmaster || 
				m is Tanner || 
				m is Tinker || 
				m is TinkerGuildmaster || 
				m is Undertaker || 
				m is Weaver || 
				m is Weaponsmith )
				return true;

			return false;
		}

		public static bool MagicRecharge( Mobile m )
		{
			if (
				m is Mage || 
				m is MageGuildmaster || 
				m is Roscoe || 
				m is Enchanter )
				return true;

			return false;
		}

		public override bool CanBeDamaged()
		{
			return !IsInvulnerable;
		}

        public void BeginRecharge( Mobile from )
        {
            if ( Deleted || !from.Alive )
                return;

			int cost = 20;

			if ( BeggingPose(from) > 0 && !(this is PlayerBarkeeper) ) // LET US SEE IF THEY ARE BEGGING
			{
				cost = cost - (int)( ( from.Skills[SkillName.Begging].Value * 0.005 ) * cost ); if ( cost < 1 ){ cost = 1; }
				SayTo(from, "Since you are begging, which item shall I recharge, for " + cost.ToString() + " gold per circle charge?");
			}
			else { SayTo(from, "Which item shall I recharge, for " + cost.ToString() + " gold per circle charge?"); }

			from.SendMessage( "" + cost + " gold per charge for first circle and " + cost*8 + " gold per charge for eighth circle." );

            from.Target = new ChargeTarget( this, cost );
        }

        public void BeginIdentify( Mobile from )
        {
            if ( Deleted || !from.Alive )
                return;

			int idCost = 200;

			if ( BeggingPose(from) > 0 && !(this is PlayerBarkeeper) ) // LET US SEE IF THEY ARE BEGGING
			{
				idCost = idCost - (int)( ( from.Skills[SkillName.Begging].Value * 0.005 ) * idCost ); if ( idCost < 1 ){ idCost = 1; }
				SayTo(from, "Since you are begging, which item shall I examine, for " + idCost.ToString() + " gold?");
			}
			else { SayTo(from, "Which item shall I examine, for " + idCost.ToString() + " gold?"); }

            from.Target = new IDTarget(this);
        }

        public void BeginRepair( Mobile from )
        {
            if  (Deleted || !from.Alive )
                return;

			int cost = 10;

			string pnt = "durablity";
			if ( this is Bard || this is BardGuildmaster )
				pnt = "use";

			if ( BeggingPose(from) > 0 && !(this is PlayerBarkeeper) ) // LET US SEE IF THEY ARE BEGGING
			{
				cost = cost - (int)( ( from.Skills[SkillName.Begging].Value * 0.005 ) * cost ); if ( cost < 1 ){ cost = 1; }
				SayTo(from, "Since you are begging, do you still want to hire me to repair something...at least " + cost.ToString() + " gold per " + pnt + "?");
			}
			else { SayTo(from, "You want to hire me to repair what at " + cost.ToString() + " gold per " + pnt + "?"); }

            from.Target = new RepairTarget( this, cost );
        }

		private class RepairTarget : Target
        {
            private BaseVendor m_Vendor;
			private int m_Cost;

            public RepairTarget( BaseVendor vendor, int cost ) : base(12, false, TargetFlags.None)
            {
                m_Vendor = vendor;
				m_Cost = cost;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
				if ( targeted is Item )
				{
					Item rep = (Item)targeted;
					Container pack = from.Backpack;

					if ( !CanRepairItem( m_Vendor, from, rep ) )
						return;

					int repaired = 0;
					int worn = 0;
					int spent = 0;

					if ( rep is BaseWeapon )
						worn = ((BaseWeapon)rep).MaxHitPoints - ((BaseWeapon)rep).HitPoints - 1;
					else if ( rep is BaseArmor )
						worn = ((BaseArmor)rep).MaxHitPoints - ((BaseArmor)rep).HitPoints - 1;
					else if ( rep is BaseClothing )
						worn = ((BaseClothing)rep).MaxHitPoints - ((BaseClothing)rep).HitPoints - 1;
					else if ( rep is BaseInstrument )
						worn = ((BaseInstrument)rep).InitMinUses - ((BaseInstrument)rep).UsesRemaining;

					if ( worn < 1 )
					{
						m_Vendor.SayTo( from, "That does not need to be repaired." );
						return;
					}

					while ( worn > 0 )
					{
						worn--;
						if ( pack.ConsumeTotal( typeof( Gold ), m_Cost ) )
						{
							spent += m_Cost;
							repaired++;
						}
						else
							worn = 0;
					}

					if ( spent < 1 )
					{
						m_Vendor.SayTo( from, "You will need to bring more gold if you want me to repair that." );
					}
					else
					{
						if ( repaired < 1 )
						{
							m_Vendor.SayTo( from, "I cannot repair that." );
							return;
						}

						if ( repaired == 1 )
							m_Vendor.SayTo( from, "I repaired it 1 time for " + spent + " gold." );
						else
							m_Vendor.SayTo( from, "I repaired it " + repaired + " times for " + spent + " gold." );

						if ( rep is BaseWeapon )
						{
							((BaseWeapon)rep).MaxHitPoints--;
							((BaseWeapon)rep).HitPoints += repaired;
						}
						else if ( rep is BaseArmor )
						{
							((BaseArmor)rep).MaxHitPoints--;
							((BaseArmor)rep).HitPoints += repaired;
						}
						else if ( rep is BaseClothing )
						{
							((BaseClothing)rep).MaxHitPoints--;
							((BaseClothing)rep).HitPoints += repaired;
						}
						else if ( rep is BaseInstrument )
						{
							((BaseInstrument)rep).UsesRemaining += repaired;
						}

						m_Vendor.AddToCoinPurse( from, spent );
						from.SendMessage( String.Format( "You pay {0} gold.", spent ) );
						if ( BeggingPose( from ) > 0 && !(m_Vendor is PlayerBarkeeper) )
							Titles.AwardKarma( from, -BeggingKarma( from ), true );
							// DO ANY KARMA LOSS
					}

				}
				else
					m_Vendor.SayTo( from, "I cannot repair that." );
            }
        }

		public static bool CanRepairItem( Mobile m, Mobile from, Item item )
		{
			bool repair = false;

			if ( 
				( item is BaseWeapon && ((BaseWeapon)item).HitPoints >= ((BaseWeapon)item).MaxHitPoints-1 ) || 
				( item is BaseArmor && ((BaseArmor)item).HitPoints >= ((BaseArmor)item).MaxHitPoints-1 ) || 
				( item is BaseInstrument && ((BaseInstrument)item).UsesRemaining >= ((BaseInstrument)item).InitMinUses ) 
			)
			{
				m.SayTo( from, "That does not need to be repaired." );
				return false;
			}

			if ( ( item is BaseKilrathi || item is BaseGiftStave || item is BaseWizardStaff || item is LightSword || item is DoubleLaserSword ) && 
				( m is Tinker || m is TinkerGuildmaster ) )
				repair = true;

			else if ( item is BaseClothing && 
				( m is Tailor || m is Weaver || m is TailorGuildmaster ) )
				repair = true;

			else if ( item is BaseRanged && CraftResources.GetType( item.Resource ) == CraftResourceType.Wood &&
				( m is Bowyer || m is Ranger || m is RangerGuildmaster || m is ArcherGuildmaster ) )
				repair = true;

			else if ( item is BaseInstrument && 
				( m is Bard || m is BardGuildmaster ) )
				repair = true;

			else if ( ( item is BaseWeapon || item is BaseArmor ) && CraftResources.GetType( item.Resource ) == CraftResourceType.Wood &&
				( m is Lumberjack || m is Carpenter || m is CarpenterGuildmaster ) )
				repair = true;

			else if ( ( item is BaseWeapon || item is BaseArmor ) && ( CraftResources.GetType( item.Resource ) == CraftResourceType.Leather || CraftResources.GetType( item.Resource ) == CraftResourceType.Skin ) &&
				( m is Tanner || m is Furtrader || m is LeatherWorker || m is TailorGuildmaster ) )
				repair = true;

			else if ( ( item is BaseWeapon || item is BaseArmor ) && CraftResources.GetType( item.Resource ) == CraftResourceType.Skeletal && m is Undertaker )
				repair = true;

			else if ( item is BaseWeapon && 
				( CraftResources.GetType( item.Resource ) == CraftResourceType.Metal || CraftResources.GetType( item.Resource ) == CraftResourceType.Scales || CraftResources.GetType( item.Resource ) == CraftResourceType.Block ) &&
				( m is Weaponsmith ) )
				repair = true;

			else if ( item is BaseArmor && 
				( CraftResources.GetType( item.Resource ) == CraftResourceType.Metal || CraftResources.GetType( item.Resource ) == CraftResourceType.Scales || CraftResources.GetType( item.Resource ) == CraftResourceType.Block ) &&
				( m is Armorer ) )
				repair = true;

			else if ( ( item is BaseWeapon || item is BaseArmor ) && 
				( CraftResources.GetType( item.Resource ) == CraftResourceType.Metal || CraftResources.GetType( item.Resource ) == CraftResourceType.Scales || CraftResources.GetType( item.Resource ) == CraftResourceType.Block ) &&
				( m is Garth || m is Blacksmith || m is IronWorker || m is BlacksmithGuildmaster ) )
				repair = true;

			if ( !repair )
				m.SayTo( from, "I cannot repair that." );

			return repair;
		}

        private class IDTarget : Target
        {
            private BaseVendor m_Vendor;

            public IDTarget(BaseVendor vendor) : base(12, false, TargetFlags.None)
            {
                m_Vendor = vendor;
            }

            protected override void OnTarget(Mobile from, object targeted)
			{
				if ( targeted is Item )
				{
					Item examine = (Item)targeted;
					int nCost = 200;

					if ( examine is ScrollClue && ( m_Vendor is Sage || m_Vendor is Scribe || m_Vendor is LibrarianGuildmaster ) )
					{
						Container packs = from.Backpack;
						ScrollClue WhatIsIt = (ScrollClue)examine;

						if ( BeggingPose(from) > 0 ) // LET US SEE IF THEY ARE BEGGING
							nCost = nCost - (int)( ( from.Skills[SkillName.Begging].Value * 0.005 ) * nCost ); if ( nCost < 1 ){ nCost = 1; }

						int toConsume = nCost;

						if ( WhatIsIt.ScrollIntelligence == 0 )
						{
							m_Vendor.SayTo( from, "That was already deciphered by someone." );
						}
						else if (packs.ConsumeTotal(typeof(Gold), toConsume))
						{
							if ( WhatIsIt.ScrollIntelligence >= 80 ){ WhatIsIt.Name = "diabolically coded parchment"; }
							else if ( WhatIsIt.ScrollIntelligence >= 70 ){ WhatIsIt.Name = "ingeniously coded parchment"; }
							else if ( WhatIsIt.ScrollIntelligence >= 60 ){ WhatIsIt.Name = "deviously coded parchment"; }
							else if ( WhatIsIt.ScrollIntelligence >= 50 ){ WhatIsIt.Name = "cleverly coded parchment"; }
							else if ( WhatIsIt.ScrollIntelligence >= 40 ){ WhatIsIt.Name = "adeptly coded parchment"; }
							else if ( WhatIsIt.ScrollIntelligence >= 30 ){ WhatIsIt.Name = "expertly coded parchment"; }
							else { WhatIsIt.Name = "plainly coded parchment"; }

							WhatIsIt.ScrollIntelligence = 0;
							WhatIsIt.InvalidateProperties();
							from.SendMessage(String.Format("You pay {0} gold.", toConsume));
							m_Vendor.SayTo(from, "Let me show you what this reads...");
							WhatIsIt.ScrollSolved = "Deciphered by " + m_Vendor.Name + " the Scribe";
							from.PlaySound( 0x249 );
							WhatIsIt.InvalidateProperties();
						}
						else
						{
							m_Vendor.SayTo(from, "It would cost you {0} gold to have that deciphered.", toConsume);
							from.SendMessage("You do not have enough gold.");
						}
					}
					else if ( examine.CoinPrice > 0 && examine.NotIdentified && RelicFunctions.VendorCanID( examine.NotIDSource, m_Vendor ) )
					{
						if ( examine.NotIDSource == Identity.None )
						{
							m_Vendor.SayTo(from, "I cannot really tell you much about that." );
							return;
						}

						Container packs = from.Backpack;

						if ( BeggingPose(from) > 0 && !(m_Vendor is PlayerBarkeeper) ) // LET US SEE IF THEY ARE BEGGING
							nCost = nCost - (int)( ( from.Skills[SkillName.Begging].Value * 0.005 ) * nCost ); if ( nCost < 1 ){ nCost = 1; }

						int toConsume = nCost;

						if ( packs.ConsumeTotal(typeof(Gold), toConsume) )
						{
							if ( examine is NotIdentified )
								RelicFunctions.IDItem( m_Vendor, from, examine, SkillName.Alchemy );
							else
							{
								from.SendMessage(String.Format("You pay {0} gold.", toConsume));
								examine.NotIdentified = false;
								m_Vendor.SayTo(from, "That is worth about " + examine.CoinPrice + " gold." );
							}

							m_Vendor.AddToCoinPurse( from, toConsume );

							if ( BeggingPose( from ) > 0 && !(m_Vendor is PlayerBarkeeper) )
								Titles.AwardKarma( from, -BeggingKarma( from ), true );
								// DO ANY KARMA LOSS
						}
						else
						{
							m_Vendor.SayTo(from, "It would cost you {0} gold for me to examine that.", toConsume);
							from.SendMessage("You do not have enough gold.");
						}
					}
					else
					{
						m_Vendor.SayTo(from, "I cannot identify that.");
					}
				}
				else
					m_Vendor.SayTo(from, "I cannot identify that.");
            }
        }

		private class ChargeTarget : Target
		{
            private BaseVendor m_Vendor;
			private int m_Cost;

            public ChargeTarget( BaseVendor vendor, int cost ) : base(12, false, TargetFlags.None)
            {
                m_Vendor = vendor;
				m_Cost = cost;
            }

            protected override void OnTarget(Mobile from, object targeted)
			{
				if ( targeted is MagicRuneBag )
				{
					m_Vendor.SayTo( from, "Place some gold in the bag and I will charge it by that amount." );
					return;
				}
				else if ( targeted is Item && ((Item)targeted).Enchanted != MagicSpell.None && ((Item)targeted).EnchantUsesMax > 0 )
				{
					Item rep = (Item)targeted;
					Container pack = from.Backpack;

					m_Cost = m_Cost * SpellItems.GetLevel( (int)(rep.Enchanted) );

					int repaired = 0;
					int worn = rep.EnchantUsesMax - rep.EnchantUses;
					int spent = 0;

					if ( worn < 1 )
					{
						m_Vendor.SayTo( from, "That does not require my services." );
						return;
					}

					while ( worn > 0 )
					{
						worn--;
						if ( pack.ConsumeTotal( typeof( Gold ), m_Cost ) )
						{
							spent += m_Cost;
							repaired++;
						}
						else
							worn = 0;

						if ( ( repaired + rep.EnchantUses ) >= rep.EnchantUsesMax )
							worn = 0;
					}

					if ( spent < 1 )
					{
						m_Vendor.SayTo( from, "You will need to bring more gold if you want me to recharge that." );
					}
					else
					{
						if ( repaired < 1 )
						{
							m_Vendor.SayTo( from, "I cannot recharge that." );
							return;
						}

						if ( repaired == 1 )
							m_Vendor.SayTo( from, "I recharged it 1 time for " + spent + " gold." );
						else
							m_Vendor.SayTo( from, "I recharged it " + repaired + " times for " + spent + " gold." );

						rep.EnchantUses += repaired;
                        Effects.PlaySound(from.Location, from.Map, 0x5C1);

						m_Vendor.AddToCoinPurse( from, spent );

						from.SendMessage( String.Format( "You pay {0} gold.", spent ) );
						if ( BeggingPose( from ) > 0 && !(m_Vendor is PlayerBarkeeper) )
							Titles.AwardKarma( from, -BeggingKarma( from ), true );
							// DO ANY KARMA LOSS
					}
				}
				else
					m_Vendor.SayTo(from, "That does not require my services.");
            }
        }
	}
}

namespace Server.ContextMenus
{
	public class SetupShoppeEntry : ContextMenuEntry
	{
		private BaseVendor m_Vendor;
		private Mobile m_From;
		
		public SetupShoppeEntry( Mobile from, BaseVendor vendor ) : base( 6164, 12 )
		{
			m_Vendor = vendor;
			m_From = from;
			Enabled = vendor.CheckVendorAccess( from );
		}

		public override void OnClick()
		{
			if ( !m_From.HasGump( typeof( ExplainShoppeGump ) ) )
			{
				m_From.SendGump( new ExplainShoppeGump( m_From ) );
			}
		}
	}

	public class RepairEntry : ContextMenuEntry
	{
		private BaseVendor m_Vendor;
		private Mobile m_From;

		public RepairEntry( Mobile from, BaseVendor vendor ) : base( 283, 12 )
		{
			m_Vendor = vendor;
			m_From = from;
			Enabled = vendor.CheckVendorAccess( from );
		}

		public override void OnClick()
		{
			m_Vendor.BeginRepair( m_From );
		}
	}

	public class IdentifyEntry : ContextMenuEntry
	{
		private BaseVendor m_Vendor;
		private Mobile m_From;

		public IdentifyEntry( Mobile from, BaseVendor vendor ) : base( 282, 12 )
		{
			m_Vendor = vendor;
			m_From = from;
			Enabled = vendor.CheckVendorAccess( from );
		}

		public override void OnClick()
		{
			m_Vendor.BeginIdentify( m_From );
		}
	}

	public class RechargeEntry : ContextMenuEntry
	{
		private BaseVendor m_Vendor;
		private Mobile m_From;

		public RechargeEntry( Mobile from, BaseVendor vendor ) : base( 284, 12 )
		{
			m_Vendor = vendor;
			m_From = from;
			Enabled = vendor.CheckVendorAccess( from );
		}

		public override void OnClick()
		{
			m_Vendor.BeginRecharge( m_From );
		}
	}

	public class VendorBuyEntry : ContextMenuEntry
	{
		private BaseVendor m_Vendor;

		public VendorBuyEntry( Mobile from, BaseVendor vendor ) : base( 6103, 12 )
		{
			m_Vendor = vendor;
			Enabled = vendor.CheckVendorAccess( from );
		}

		public override void OnClick()
		{
			m_Vendor.VendorBuy( this.Owner.From );
		}
	}

	public class VendorSellEntry : ContextMenuEntry
	{
		private BaseVendor m_Vendor;

		public VendorSellEntry( Mobile from, BaseVendor vendor ) : base( 6104, 12 )
		{
			m_Vendor = vendor;
			Enabled = vendor.CheckVendorAccess( from );
		}

		public override void OnClick()
		{
			m_Vendor.VendorSell( this.Owner.From );
		}
	}

	public class BlackMarketEntry : ContextMenuEntry
	{
		private BaseVendor m_Vendor;
		private Mobile m_From;

		public BlackMarketEntry( Mobile from, BaseVendor vendor ) : base( 6094, 12 )
		{
			m_Vendor = vendor;
			m_From = from;
			Enabled = vendor.CheckVendorAccess( from );
		}

		public override void OnClick()
		{
			m_From.CloseGump( typeof( BlackMarketGump ) );

			if ( m_Vendor.BankBox.TotalItems > 0 )
			{
				m_From.SendGump( new BlackMarketGump( m_Vendor, m_From, null, 1, 0 ) );
				m_Vendor.SayTo( m_From, "Here are some special items I have." );
			}
			else
				m_Vendor.SayTo( m_From, "Sorry, but I have nothing available." );

		}
	}
}

namespace Server
{
	public interface IShopSellInfo
	{
		//get display name for an item
		string GetNameFor( Item item );

		//get price for an item which the player is selling
		int GetSellPriceFor( Item item, int barter );

		//get price for an item which the player is buying
		int GetBuyPriceFor( Item item );

		//can we sell this item to this vendor?
		bool IsSellable( Item item );

		//What do we sell?
		Type[] Types { get; }

		//does the vendor resell this item?
		bool IsResellable( Item item );
	}

	public interface IBuyItemInfo
	{
		//get a new instance of an object (we just bought it)
		IEntity GetEntity();

		int ControlSlots { get; }

		int PriceScalar { get; set; }

		//display price of the item
		int Price { get; }

		//display name of the item
		string Name { get; }

		//display hue
		int Hue { get; }

		//display id
		int ItemID { get; }

		//amount in stock
		int Amount { get; set; }

		//max amount in stock
		int MaxAmount { get; }

		//Attempt to restock with item, (return true if restock sucessful)
		bool Restock( Item item, int amount );

		//called when its time for the whole shop to restock
		void OnRestock();
	}
}