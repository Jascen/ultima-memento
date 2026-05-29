using System;
using Server;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1940, 0x1AD7 )]
	public class PotionKeg : Item
	{
		public override string DefaultDescription{ get{ return "The kegs can hold a large quantity of most potions. You can drop potions on the keg to fill it, or use the keg to fill an empty bottle you have."; } }

		private PotionEffect m_Type;
		private int m_Held;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Held
		{
			get
			{
				return m_Held;
			}
			set
			{
				if ( m_Held != value )
				{
					m_Held = value;
					UpdateWeight();
					InvalidateProperties();
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public PotionEffect Type
		{
			get
			{
				return m_Type;
			}
			set
			{
				m_Type = value;
				InvalidateProperties();
			}
		}

		[Constructable]
		public PotionKeg() : base( 0x1940 )
		{
			UpdateWeight();
			SetColorKeg( this, this );
		}

		public virtual void UpdateWeight()
		{
			int held = Math.Max( 0, Math.Min( m_Held, 100 ) );
			this.Weight = 20 + ((held * 80) / 100);
			SetColorKeg( this, this );
		}

		public PotionKeg( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
			writer.Write( (int) m_Type );
			writer.Write( (int) m_Held );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			switch ( version )
			{
				case 1:
				case 0:
				{
					m_Type = (PotionEffect)reader.ReadInt();
					m_Held = reader.ReadInt();

					break;
				}
			}
			if ( version < 1 )
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( UpdateWeight ) );
		}

		public override int LabelNumber
		{ 
			get
			{ 
				if ( m_Held == 0 )
					return 1041084; // A specially lined keg for potions.
				else if( m_Type >= PotionEffect.Conflagration )
					return 1072658 + (int) m_Type - (int) PotionEffect.Conflagration;
				else
					return ( 1041620 + (int)m_Type ); 
			} 
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			int number;

			if ( m_Held <= 0 )
				number = 502246; // The keg is empty.
			else if ( m_Held < 5 )
				number = 502248; // The keg is nearly empty.
			else if ( m_Held < 20 )
				number = 502249; // The keg is not very full.
			else if ( m_Held < 30 )
				number = 502250; // The keg is about one quarter full.
			else if ( m_Held < 40 )
				number = 502251; // The keg is about one third full.
			else if ( m_Held < 47 )
				number = 502252; // The keg is almost half full.
			else if ( m_Held < 54 )
				number = 502254; // The keg is approximately half full.
			else if ( m_Held < 70 )
				number = 502253; // The keg is more than half full.
			else if ( m_Held < 80 )
				number = 502255; // The keg is about three quarters full.
			else if ( m_Held < 96 )
				number = 502256; // The keg is very full.
			else if ( m_Held < 100 )
				number = 502257; // The liquid is almost to the top of the keg.
			else
				number = 502258; // The keg is completely full.

			list.Add( number );
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			int number;

			if ( m_Held <= 0 )
				number = 502246; // The keg is empty.
			else if ( m_Held < 5 )
				number = 502248; // The keg is nearly empty.
			else if ( m_Held < 20 )
				number = 502249; // The keg is not very full.
			else if ( m_Held < 30 )
				number = 502250; // The keg is about one quarter full.
			else if ( m_Held < 40 )
				number = 502251; // The keg is about one third full.
			else if ( m_Held < 47 )
				number = 502252; // The keg is almost half full.
			else if ( m_Held < 54 )
				number = 502254; // The keg is approximately half full.
			else if ( m_Held < 70 )
				number = 502253; // The keg is more than half full.
			else if ( m_Held < 80 )
				number = 502255; // The keg is about three quarters full.
			else if ( m_Held < 96 )
				number = 502256; // The keg is very full.
			else if ( m_Held < 100 )
				number = 502257; // The liquid is almost to the top of the keg.
			else
				number = 502258; // The keg is completely full.

			this.LabelTo( from, number );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( GetWorldLocation(), 2 ) )
			{
				if ( m_Held > 0 )
				{
					Container pack = from.Backpack;

					if ( pack != null && ( ( IsJarPotion( m_Type ) && pack.ConsumeTotal( typeof( Jar ), 1 ) ) || ( !IsJarPotion( m_Type ) && pack.ConsumeTotal( typeof( Bottle ), 1 ) ) ) )
					{
						from.SendLocalizedMessage( 502242 ); // You pour some of the keg's contents into an empty bottle...

						BasePotion pot = FillBottle();

						if ( pack.TryDropItem( from, pot, false ) )
						{
							from.SendLocalizedMessage( 502243 ); // ...and place it into your backpack.
							from.PlaySound( 0x240 );

							if ( --Held == 0 )
								from.SendLocalizedMessage( 502245 ); // The keg is now empty.
						}
						else
						{
							from.SendLocalizedMessage( 502244 ); // ...but there is no room for the bottle in your backpack.
							pot.Delete();
						}
					}
					else
					{
						// TODO: Target a bottle
					}
				}
				else
				{
					from.SendLocalizedMessage( 502246 ); // The keg is empty.
				}
				SetColorKeg( this, this );
			}
			else
			{
				from.LocalOverheadMessage( Network.MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
			}
		}

		public override bool OnDragDrop( Mobile from, Item item )
		{
			if ( item is BasePotion )
			{
				BasePotion pot = (BasePotion)item;
                int toHold = Math.Min( 100 - m_Held, pot.Amount );

				if ( toHold <= 0 )
				{
					from.SendLocalizedMessage( 502233 ); // The keg will not hold any more!
					return false;
				}
				else if ( m_Held == 0 )
				{
					if ( GiveBottle( from, toHold, item ) )
					{
						this.Hue = 0x96D;
						m_Type = pot.PotionEffect;
						Held = toHold;

						SetColorKeg( item, this );

						from.PlaySound( 0x240 );

						from.SendLocalizedMessage( 502237 ); // You place the empty bottle in your backpack.

                        item.Consume( toHold );

						if( !item.Deleted )
							item.Bounce( from );

						return true;
					}
					else
					{
						from.SendLocalizedMessage( 502238 ); // You don't have room for the empty bottle in your backpack.
						return false;
					}
				}
				else if ( pot.PotionEffect != m_Type )
				{
					from.SendLocalizedMessage( 502236 ); // You decide that it would be a bad idea to mix different types of potions.
					return false;
				}
				else
				{
					if ( GiveBottle( from, toHold, item ) )
					{
						Held += toHold;

						from.PlaySound( 0x240 );

						from.SendLocalizedMessage( 502237 ); // You place the empty bottle in your backpack.

						item.Consume( toHold );

						if( !item.Deleted )
							item.Bounce( from );

						return true;
					}
					else
					{
						from.SendLocalizedMessage( 502238 ); // You don't have room for the empty bottle in your backpack.
						return false;
					}
				}
			}
			else
			{
				from.SendLocalizedMessage( 502232 ); // The keg is not designed to hold that type of object.
				return false;
			}
		}

		public bool GiveBottle( Mobile m, int amount, Item potion )
		{
			Container pack = m.Backpack;

			if ( potion is BaseMixture )
			{
				Jar jar = new Jar( amount );

				if ( pack == null || !pack.TryDropItem( m, jar, false ) )
				{
					jar.Delete();
					return false;
				}
			}
			else
			{
				Bottle bottle = new Bottle( amount );

				if ( pack == null || !pack.TryDropItem( m, bottle, false ) )
				{
					bottle.Delete();
					return false;
				}
			}

			return true;
		}

		public BasePotion FillBottle()
		{
			return CreatePotion( m_Type );
		}

		public static BasePotion CreatePotion( PotionEffect type )
		{
			switch ( type )
			{
				default:
				case PotionEffect.Nightsight:				return new NightSightPotion();

				case PotionEffect.CureLesser:				return new LesserCurePotion();
				case PotionEffect.Cure:						return new CurePotion();
				case PotionEffect.CureGreater:				return new GreaterCurePotion();

				case PotionEffect.Agility:					return new AgilityPotion();
				case PotionEffect.AgilityGreater:			return new GreaterAgilityPotion();

				case PotionEffect.Strength:					return new StrengthPotion();
				case PotionEffect.StrengthGreater:			return new GreaterStrengthPotion();

				case PotionEffect.PoisonLesser:				return new LesserPoisonPotion();
				case PotionEffect.Poison:					return new PoisonPotion();
				case PotionEffect.PoisonGreater:			return new GreaterPoisonPotion();
				case PotionEffect.PoisonDeadly:				return new DeadlyPoisonPotion();
				case PotionEffect.PoisonLethal:				return new LethalPoisonPotion();

				case PotionEffect.Refresh:					return new RefreshPotion();
				case PotionEffect.RefreshTotal:				return new TotalRefreshPotion();

				case PotionEffect.HealLesser:				return new LesserHealPotion();
				case PotionEffect.Heal:						return new HealPotion();
				case PotionEffect.HealGreater:				return new GreaterHealPotion();

				case PotionEffect.ExplosionLesser:			return new LesserExplosionPotion();
				case PotionEffect.Explosion:				return new ExplosionPotion();
				case PotionEffect.ExplosionGreater:			return new GreaterExplosionPotion();
				
				case PotionEffect.Conflagration:			return new ConflagrationPotion();
				case PotionEffect.ConflagrationGreater:		return new GreaterConflagrationPotion();

				case PotionEffect.ConfusionBlast:			return new ConfusionBlastPotion();
				case PotionEffect.ConfusionBlastGreater:	return new GreaterConfusionBlastPotion();

				case PotionEffect.InvisibilityLesser:		return new LesserInvisibilityPotion();
				case PotionEffect.Invisibility:				return new InvisibilityPotion();
				case PotionEffect.InvisibilityGreater:		return new GreaterInvisibilityPotion();
				case PotionEffect.RejuvenateLesser:			return new LesserRejuvenatePotion();
				case PotionEffect.Rejuvenate:				return new RejuvenatePotion();
				case PotionEffect.RejuvenateGreater:		return new GreaterRejuvenatePotion();
				case PotionEffect.ManaLesser:				return new LesserManaPotion();
				case PotionEffect.Mana:						return new ManaPotion();
				case PotionEffect.ManaGreater:				return new GreaterManaPotion();
				case PotionEffect.Invulnerability:			return new InvulnerabilityPotion();

				case PotionEffect.ElixirAlchemy:			return new ElixirAlchemy();
				case PotionEffect.ElixirAnatomy:			return new ElixirAnatomy();
				case PotionEffect.ElixirAnimalLore:			return new ElixirAnimalLore();
				case PotionEffect.ElixirAnimalTaming:		return new ElixirAnimalTaming();
				case PotionEffect.ElixirArchery:			return new ElixirArchery();
				case PotionEffect.ElixirArmsLore:			return new ElixirArmsLore();
				case PotionEffect.ElixirBegging:			return new ElixirBegging();
				case PotionEffect.ElixirBlacksmith:			return new ElixirBlacksmith();
				case PotionEffect.ElixirCamping:			return new ElixirCamping();
				case PotionEffect.ElixirCarpentry:			return new ElixirCarpentry();
				case PotionEffect.ElixirCartography:		return new ElixirCartography();
				case PotionEffect.ElixirCooking:			return new ElixirCooking();
				case PotionEffect.ElixirDetectHidden:		return new ElixirDetectHidden();
				case PotionEffect.ElixirDiscordance:		return new ElixirDiscordance();
				case PotionEffect.ElixirEvalInt:			return new ElixirEvalInt();
				case PotionEffect.ElixirFencing:			return new ElixirFencing();
				case PotionEffect.ElixirFishing:			return new ElixirFishing();
				case PotionEffect.ElixirFletching:			return new ElixirFletching();
				case PotionEffect.ElixirFocus:				return new ElixirFocus();
				case PotionEffect.ElixirForensics:			return new ElixirForensics();
				case PotionEffect.ElixirHealing:			return new ElixirHealing();
				case PotionEffect.ElixirHerding:			return new ElixirHerding();
				case PotionEffect.ElixirHiding:				return new ElixirHiding();
				case PotionEffect.ElixirInscribe:			return new ElixirInscribe();
				case PotionEffect.ElixirItemID:				return new ElixirItemID();
				case PotionEffect.ElixirLockpicking:		return new ElixirLockpicking();
				case PotionEffect.ElixirLumberjacking:		return new ElixirLumberjacking();
				case PotionEffect.ElixirMacing:				return new ElixirMacing();
				case PotionEffect.ElixirMagicResist:		return new ElixirMagicResist();
				case PotionEffect.ElixirMeditation:			return new ElixirMeditation();
				case PotionEffect.ElixirMining:				return new ElixirMining();
				case PotionEffect.ElixirMusicianship:		return new ElixirMusicianship();
				case PotionEffect.ElixirParry:				return new ElixirParry();
				case PotionEffect.ElixirPeacemaking:		return new ElixirPeacemaking();
				case PotionEffect.ElixirPoisoning:			return new ElixirPoisoning();
				case PotionEffect.ElixirProvocation:		return new ElixirProvocation();
				case PotionEffect.ElixirRemoveTrap:			return new ElixirRemoveTrap();
				case PotionEffect.ElixirSnooping:			return new ElixirSnooping();
				case PotionEffect.ElixirSpiritSpeak:		return new ElixirSpiritSpeak();
				case PotionEffect.ElixirStealing:			return new ElixirStealing();
				case PotionEffect.ElixirStealth:			return new ElixirStealth();
				case PotionEffect.ElixirSwords:				return new ElixirSwords();
				case PotionEffect.ElixirTactics:			return new ElixirTactics();
				case PotionEffect.ElixirTailoring:			return new ElixirTailoring();
				case PotionEffect.ElixirTasteID:			return new ElixirTasteID();
				case PotionEffect.ElixirTinkering:			return new ElixirTinkering();
				case PotionEffect.ElixirTracking:			return new ElixirTracking();
				case PotionEffect.ElixirVeterinary:			return new ElixirVeterinary();
				case PotionEffect.ElixirWrestling:			return new ElixirWrestling();

				case PotionEffect.MixtureSlime:				return new MixtureSlime();
				case PotionEffect.MixtureIceSlime:			return new MixtureIceSlime();
				case PotionEffect.MixtureFireSlime:			return new MixtureFireSlime();
				case PotionEffect.MixtureDiseasedSlime:		return new MixtureDiseasedSlime();
				case PotionEffect.MixtureRadiatedSlime:		return new MixtureRadiatedSlime();

				case PotionEffect.LiquidFire:				return new LiquidFire();
				case PotionEffect.LiquidGoo:				return new LiquidGoo();
				case PotionEffect.LiquidIce:				return new LiquidIce();
				case PotionEffect.LiquidRot:				return new LiquidRot();
				case PotionEffect.LiquidPain:				return new LiquidPain();

				case PotionEffect.Resurrect:				return new ResurrectPotion();
				case PotionEffect.SuperPotion:				return new SuperPotion();
				case PotionEffect.Repair:					return new RepairPotion();
				case PotionEffect.Durability:				return new DurabilityPotion();
				case PotionEffect.HairOil:					return new HairOilPotion();
				case PotionEffect.HairDye:					return new HairDyePotion();

				case PotionEffect.Frostbite:				return new FrostbitePotion();
				case PotionEffect.FrostbiteGreater:			return new GreaterFrostbitePotion();
			}
		}

		public static void Initialize()
		{
			TileData.ItemTable[0x1940].Height = 4;
		}

		public static void SetColorKeg( Item potion, Item keg )
		{
			PotionKeg p = (PotionKeg)keg;

			if ( p.Held < 1 )
			{ 
				keg.Hue = 0x96D;
				keg.Name = "empty potion keg";
			}
			else
			{
				if ( potion != keg )
				{
					if ( potion is BasePotion )
					{
						BasePotion pot = (BasePotion)potion;
						p.Type = pot.PotionEffect;
						keg.Hue = Server.Items.PotionKeg.GetPotionColor( potion );
					}
				}

				switch ( p.Type )
				{
					case PotionEffect.Nightsight : keg.Name = "keg of nightsight potions"; break;
					case PotionEffect.CureLesser : keg.Name = "keg of lesser cure potions"; break;
					case PotionEffect.Cure : keg.Name = "keg of cure potions"; break;
					case PotionEffect.CureGreater : keg.Name = "keg of greater cure potions"; break;
					case PotionEffect.Agility : keg.Name = "keg of agility potions"; break;
					case PotionEffect.AgilityGreater : keg.Name = "keg of greater agility potions"; break;
					case PotionEffect.Strength : keg.Name = "keg of strength potions"; break;
					case PotionEffect.StrengthGreater : keg.Name = "keg of greater strength potions"; break;
					case PotionEffect.PoisonLesser : keg.Name = "keg of lesser poison potions"; break;
					case PotionEffect.Poison : keg.Name = "keg of poison potions"; break;
					case PotionEffect.PoisonGreater : keg.Name = "keg of greater poison potions"; break;
					case PotionEffect.PoisonDeadly : keg.Name = "keg of deadly poison potions"; break;
					case PotionEffect.PoisonLethal : keg.Name = "keg of lethal poison potions"; break;
					case PotionEffect.Refresh : keg.Name = "keg of refresh potions"; break;
					case PotionEffect.RefreshTotal : keg.Name = "keg of total refresh potions"; break;
					case PotionEffect.HealLesser : keg.Name = "keg of lesser heal potions"; break;
					case PotionEffect.Heal : keg.Name = "keg of heal potions"; break;
					case PotionEffect.HealGreater : keg.Name = "keg of greater heal potions"; break;
					case PotionEffect.ExplosionLesser : keg.Name = "keg of lesser explosion potions"; break;
					case PotionEffect.Explosion : keg.Name = "keg of explosion potions"; break;
					case PotionEffect.ExplosionGreater : keg.Name = "keg of greater explosion potions"; break;
					case PotionEffect.InvisibilityLesser : keg.Name = "keg of lesser invisibility potions"; break;
					case PotionEffect.Invisibility : keg.Name = "keg of invisibility potions"; break;
					case PotionEffect.InvisibilityGreater : keg.Name = "keg of greater invisibility potions"; break;
					case PotionEffect.RejuvenateLesser : keg.Name = "keg of lesser rejuvenate potions"; break;
					case PotionEffect.Rejuvenate : keg.Name = "keg of rejuvenate potions"; break;
					case PotionEffect.RejuvenateGreater : keg.Name = "keg of greater rejuvenate potions"; break;
					case PotionEffect.ManaLesser : keg.Name = "keg of lesser mana potions"; break;
					case PotionEffect.Mana : keg.Name = "keg of mana potions"; break;
					case PotionEffect.ManaGreater : keg.Name = "keg of greater mana potions"; break;
					case PotionEffect.Invulnerability : keg.Name = "keg of invulnerability potions"; break;
					case PotionEffect.Conflagration : keg.Name = "keg of conflagration potions"; break;
					case PotionEffect.ConflagrationGreater : keg.Name = "keg of greater conflagration potions"; break;
					case PotionEffect.ConfusionBlast : keg.Name = "keg of confusion blast potions"; break;
					case PotionEffect.ConfusionBlastGreater : keg.Name = "keg of greater confusion blast potions"; break;

					case PotionEffect.ElixirAlchemy : keg.Name = "keg of alchemy elixir"; break;
					case PotionEffect.ElixirAnatomy : keg.Name = "keg of anatomy elixir"; break;
					case PotionEffect.ElixirAnimalLore : keg.Name = "keg of druidism elixir"; break;
					case PotionEffect.ElixirAnimalTaming : keg.Name = "keg of taming elixir"; break;
					case PotionEffect.ElixirArchery : keg.Name = "keg of marksmanship elixir"; break;
					case PotionEffect.ElixirArmsLore : keg.Name = "keg of arms lore elixir"; break;
					case PotionEffect.ElixirBegging : keg.Name = "keg of begging elixir"; break;
					case PotionEffect.ElixirBlacksmith : keg.Name = "keg of blacksmithing elixir"; break;
					case PotionEffect.ElixirCamping : keg.Name = "keg of camping elixir"; break;
					case PotionEffect.ElixirCarpentry : keg.Name = "keg of carpentry elixir"; break;
					case PotionEffect.ElixirCartography : keg.Name = "keg of cartography elixir"; break;
					case PotionEffect.ElixirCooking : keg.Name = "keg of cooking elixir"; break;
					case PotionEffect.ElixirDetectHidden : keg.Name = "keg of searching elixir"; break;
					case PotionEffect.ElixirDiscordance : keg.Name = "keg of discordance elixir"; break;
					case PotionEffect.ElixirEvalInt : keg.Name = "keg of psychology elixir"; break;
					case PotionEffect.ElixirFencing : keg.Name = "keg of fencing elixir"; break;
					case PotionEffect.ElixirFishing : keg.Name = "keg of seafaring elixir"; break;
					case PotionEffect.ElixirFletching : keg.Name = "keg of bowcrafting elixir"; break;
					case PotionEffect.ElixirFocus : keg.Name = "keg of focus elixir"; break;
					case PotionEffect.ElixirForensics : keg.Name = "keg of forensics elixir"; break;
					case PotionEffect.ElixirHealing : keg.Name = "keg of the healer elixir"; break;
					case PotionEffect.ElixirHerding : keg.Name = "keg of herding elixir"; break;
					case PotionEffect.ElixirHiding : keg.Name = "keg of hiding elixir"; break;
					case PotionEffect.ElixirInscribe : keg.Name = "keg of inscription elixir"; break;
					case PotionEffect.ElixirItemID : keg.Name = "keg of mercantile elixir"; break;
					case PotionEffect.ElixirLockpicking : keg.Name = "keg of lockpicking elixir"; break;
					case PotionEffect.ElixirLumberjacking : keg.Name = "keg of lumberjacking elixir"; break;
					case PotionEffect.ElixirMacing : keg.Name = "keg of bludgeoning elixir"; break;
					case PotionEffect.ElixirMagicResist : keg.Name = "keg of magic resistance elixir"; break;
					case PotionEffect.ElixirMeditation : keg.Name = "keg of meditating elixir"; break;
					case PotionEffect.ElixirMining : keg.Name = "keg of mining elixir"; break;
					case PotionEffect.ElixirMusicianship : keg.Name = "keg of musicianship elixir"; break;
					case PotionEffect.ElixirParry : keg.Name = "keg of parrying elixir"; break;
					case PotionEffect.ElixirPeacemaking : keg.Name = "keg of peacemaking elixir"; break;
					case PotionEffect.ElixirPoisoning : keg.Name = "keg of poisoning elixir"; break;
					case PotionEffect.ElixirProvocation : keg.Name = "keg of provocation elixir"; break;
					case PotionEffect.ElixirRemoveTrap : keg.Name = "keg of removing trap elixir"; break;
					case PotionEffect.ElixirSnooping : keg.Name = "keg of snooping elixir"; break;
					case PotionEffect.ElixirSpiritSpeak : keg.Name = "keg of spiritualism elixir"; break;
					case PotionEffect.ElixirStealing : keg.Name = "keg of stealing elixir"; break;
					case PotionEffect.ElixirStealth : keg.Name = "keg of stealth elixir"; break;
					case PotionEffect.ElixirSwords : keg.Name = "keg of sword fighting elixir"; break;
					case PotionEffect.ElixirTactics : keg.Name = "keg of tactics elixir"; break;
					case PotionEffect.ElixirTailoring : keg.Name = "keg of tailoring elixir"; break;
					case PotionEffect.ElixirTasteID : keg.Name = "keg of tasting elixir"; break;
					case PotionEffect.ElixirTinkering : keg.Name = "keg of tinkering elixir"; break;
					case PotionEffect.ElixirTracking : keg.Name = "keg of tracking elixir"; break;
					case PotionEffect.ElixirVeterinary : keg.Name = "keg of veterinary elixir"; break;
					case PotionEffect.ElixirWrestling : keg.Name = "keg of fist fighting"; break;

					case PotionEffect.MixtureSlime : keg.Name = "keg of slimy mixture"; break;
					case PotionEffect.MixtureIceSlime : keg.Name = "keg of slimy ice mixture"; break;
					case PotionEffect.MixtureFireSlime : keg.Name = "keg of slimy fire mixture"; break;
					case PotionEffect.MixtureDiseasedSlime : keg.Name = "keg of slimy diseased mixture"; break;
					case PotionEffect.MixtureRadiatedSlime : keg.Name = "keg of slimy irradiated mixture"; break;

					case PotionEffect.LiquidFire : keg.Name = "keg of liquid fire"; break;
					case PotionEffect.LiquidGoo : keg.Name = "keg of liquid goo"; break;
					case PotionEffect.LiquidIce : keg.Name = "keg of liquid ice"; break;
					case PotionEffect.LiquidRot : keg.Name = "keg of liquid rot"; break;
					case PotionEffect.LiquidPain : keg.Name = "keg of liquid pain"; break;

					case PotionEffect.Resurrect : keg.Name = "keg of resurrection"; break;
					case PotionEffect.SuperPotion : keg.Name = "keg of superior"; break;
					case PotionEffect.Repair : keg.Name = "keg of repair"; break;
					case PotionEffect.Durability : keg.Name = "keg of durability"; break;
					case PotionEffect.HairOil : keg.Name = "keg of hair styling"; break;
					case PotionEffect.HairDye : keg.Name = "keg of hair dye"; break;

					case PotionEffect.Frostbite : keg.Name = "keg of frostbite potions"; break;
					case PotionEffect.FrostbiteGreater : keg.Name = "keg of greater frostbite potions"; break;
				}
			}
		}

		public static int GetPotionColor( Item potion )
		{
			int color = 0;

			if ( potion is BasePotion )
			{
				BasePotion pot = (BasePotion)potion;

				switch ( pot.PotionEffect )
				{
					case PotionEffect.Nightsight : color = 1109; break;
					case PotionEffect.CureLesser : color = 45; break;
					case PotionEffect.Cure : color = 45; break;
					case PotionEffect.CureGreater : color = 45; break;
					case PotionEffect.Agility : color = 396; break;
					case PotionEffect.AgilityGreater : color = 396; break;
					case PotionEffect.Strength : color = 1001; break;
					case PotionEffect.StrengthGreater : color = 1001; break;
					case PotionEffect.PoisonLesser : color = 73; break;
					case PotionEffect.Poison : color = 73; break;
					case PotionEffect.PoisonGreater : color = 73; break;
					case PotionEffect.PoisonDeadly : color = 73; break;
					case PotionEffect.PoisonLethal : color = 73; break;
					case PotionEffect.Refresh : color = 140; break;
					case PotionEffect.RefreshTotal : color = 140; break;
					case PotionEffect.HealLesser : color = 50; break;
					case PotionEffect.Heal : color = 50; break;
					case PotionEffect.HealGreater : color = 50; break;
					case PotionEffect.ExplosionLesser : color = 425; break;
					case PotionEffect.Explosion : color = 425; break;
					case PotionEffect.ExplosionGreater : color = 425; break;
					case PotionEffect.InvisibilityLesser : color = 0x490; break;
					case PotionEffect.Invisibility : color = 0x490; break;
					case PotionEffect.InvisibilityGreater : color = 0x490; break;
					case PotionEffect.RejuvenateLesser : color = 0x48E; break;
					case PotionEffect.Rejuvenate : color = 0x48E; break;
					case PotionEffect.RejuvenateGreater : color = 0x48E; break;
					case PotionEffect.ManaLesser : color = 0x48D; break;
					case PotionEffect.Mana : color = 0x48D; break;
					case PotionEffect.ManaGreater : color = 0x48D; break;
					case PotionEffect.Invulnerability : color = 0x496; break;
					case PotionEffect.Conflagration : color = 0xAD8; break;
					case PotionEffect.ConflagrationGreater : color = 0xAD8; break;
					case PotionEffect.ConfusionBlast : color = 0x495; break;
					case PotionEffect.ConfusionBlastGreater : color = 0x495; break;

					case PotionEffect.ElixirAlchemy : color = 0x493; break;
					case PotionEffect.ElixirAnatomy : color = 0x492; break;
					case PotionEffect.ElixirAnimalLore : color = 0x491; break;
					case PotionEffect.ElixirAnimalTaming : color = 0x490; break;
					case PotionEffect.ElixirArchery : color = 0x48F; break;
					case PotionEffect.ElixirArmsLore : color = 0x48E; break;
					case PotionEffect.ElixirBegging : color = 0x48D; break;
					case PotionEffect.ElixirBlacksmith : color = 0x48C; break;
					case PotionEffect.ElixirCamping : color = 0x482; break;
					case PotionEffect.ElixirCarpentry : color = 0x47E; break;
					case PotionEffect.ElixirCartography : color = 0x40; break;
					case PotionEffect.ElixirCooking : color = 0x46; break;
					case PotionEffect.ElixirDetectHidden : color = 0x50; break;
					case PotionEffect.ElixirDiscordance : color = 0x55; break;
					case PotionEffect.ElixirEvalInt : color = 0x5A; break;
					case PotionEffect.ElixirFencing : color = 0x5E; break;
					case PotionEffect.ElixirFishing : color = 0x64; break;
					case PotionEffect.ElixirFletching : color = 0x69; break;
					case PotionEffect.ElixirFocus : color = 0x6E; break;
					case PotionEffect.ElixirForensics : color = 0x74; break;
					case PotionEffect.ElixirHealing : color = 0x78; break;
					case PotionEffect.ElixirHerding : color = 0xB95; break;
					case PotionEffect.ElixirHiding : color = 0x967; break;
					case PotionEffect.ElixirInscribe : color = 0x970; break;
					case PotionEffect.ElixirItemID : color = 0x976; break;
					case PotionEffect.ElixirLockpicking : color = 0x97B; break;
					case PotionEffect.ElixirLumberjacking : color = 0x89C; break;
					case PotionEffect.ElixirMacing : color = 0x8A1; break;
					case PotionEffect.ElixirMagicResist : color = 0x8A8; break;
					case PotionEffect.ElixirMeditation : color = 0x8AD; break;
					case PotionEffect.ElixirMining : color = 0x846; break;
					case PotionEffect.ElixirMusicianship : color = 0x84C; break;
					case PotionEffect.ElixirParry : color = 0x852; break;
					case PotionEffect.ElixirPeacemaking : color = 0x6DE; break;
					case PotionEffect.ElixirPoisoning : color = 0x9C4; break;
					case PotionEffect.ElixirProvocation : color = 0x6EE; break;
					case PotionEffect.ElixirRemoveTrap : color = 0x5B1; break;
					case PotionEffect.ElixirSnooping : color = 0x5B2; break;
					case PotionEffect.ElixirSpiritSpeak : color = 0x5B3; break;
					case PotionEffect.ElixirStealing : color = 0x5B4; break;
					case PotionEffect.ElixirStealth : color = 0x5B5; break;
					case PotionEffect.ElixirSwords : color = 0x5B6; break;
					case PotionEffect.ElixirTactics : color = 0x5B7; break;
					case PotionEffect.ElixirTailoring : color = 0x550; break;
					case PotionEffect.ElixirTasteID : color = 0x556; break;
					case PotionEffect.ElixirTinkering : color = 0x55C; break;
					case PotionEffect.ElixirTracking : color = 0x560; break;
					case PotionEffect.ElixirVeterinary : color = 0x495; break;
					case PotionEffect.ElixirWrestling : color = 0x494; break;

					case PotionEffect.MixtureSlime : color = 0x8AB; break;
					case PotionEffect.MixtureIceSlime : color = 0x480; break;
					case PotionEffect.MixtureFireSlime : color = 0x4EC; break;
					case PotionEffect.MixtureDiseasedSlime : color = 0x7D6; break;
					case PotionEffect.MixtureRadiatedSlime : color = 0xB96; break;

					case PotionEffect.LiquidFire : color = 0x489; break;
					case PotionEffect.LiquidGoo : color = 0x490; break;
					case PotionEffect.LiquidIce : color = 0x482; break;
					case PotionEffect.LiquidRot : color = 0xB97; break;
					case PotionEffect.LiquidPain : color = 0x835; break;

					case PotionEffect.Resurrect : color = 0xB06; break;
					case PotionEffect.SuperPotion : color = 0xBA4; break;
					case PotionEffect.Repair : color = 0xB7A; break;
					case PotionEffect.Durability : color = 0xB7D; break;
					case PotionEffect.HairOil : color = 0xB07; break;
					case PotionEffect.HairDye : color = 0xB04; break;

					case PotionEffect.Frostbite : color = 0xAF3; break;
					case PotionEffect.FrostbiteGreater : color = 0xAF3; break;
				}
			}
			return color;
		}

		public static bool IsJarPotion( PotionEffect p )
		{
			bool IsJar = false;

			switch ( p )
			{
				case PotionEffect.MixtureSlime : IsJar = true; break;
				case PotionEffect.MixtureIceSlime : IsJar = true; break;
				case PotionEffect.MixtureFireSlime : IsJar = true; break;
				case PotionEffect.MixtureDiseasedSlime : IsJar = true; break;
				case PotionEffect.MixtureRadiatedSlime : IsJar = true; break;

				case PotionEffect.LiquidFire : IsJar = true; break;
				case PotionEffect.LiquidGoo : IsJar = true; break;
				case PotionEffect.LiquidIce : IsJar = true; break;
				case PotionEffect.LiquidRot : IsJar = true; break;
				case PotionEffect.LiquidPain : IsJar = true; break;
			}

			return IsJar;
		}
	}
}