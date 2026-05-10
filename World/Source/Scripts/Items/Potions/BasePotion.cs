using System;
using Server;
using Server.Engines.Craft;
using System.Collections.Generic;

namespace Server.Items
{
	public enum PotionEffect
	{
		Nightsight,
		CureLesser,
		Cure,
		CureGreater,
		Agility,
		AgilityGreater,
		Strength,
		StrengthGreater,
		PoisonLesser,
		Poison,
		PoisonGreater,
		PoisonDeadly,
		Refresh,
		RefreshTotal,
		HealLesser,
		Heal,
		HealGreater,
		ExplosionLesser,
		Explosion,
		ExplosionGreater,
		Conflagration,
		ConflagrationGreater,
		MaskOfDeath,		// Mask of Death is not available in OSI but does exist in cliloc files
		MaskOfDeathGreater,	// included in enumeration for compatability if later enabled by OSI
		ConfusionBlast,
		ConfusionBlastGreater,
		InvisibilityLesser,
		Invisibility,
		InvisibilityGreater,
		RejuvenateLesser,
		Rejuvenate,
		RejuvenateGreater,
		ManaLesser,
		Mana,
		ManaGreater,
		Invulnerability,
		PoisonLethal,
		SilverSerpentVenom,
		GoldenSerpentVenom,
		ElixirAlchemy,
		ElixirAnatomy,
		ElixirAnimalLore,
		ElixirAnimalTaming,
		ElixirArchery,
		ElixirArmsLore,
		ElixirBegging,
		ElixirBlacksmith,
		ElixirCamping,
		ElixirCarpentry,
		ElixirCartography,
		ElixirCooking,
		ElixirDetectHidden,
		ElixirDiscordance,
		ElixirEvalInt,
		ElixirFencing,
		ElixirFishing,
		ElixirFletching,
		ElixirFocus,
		ElixirForensics,
		ElixirHealing,
		ElixirHerding,
		ElixirHiding,
		ElixirInscribe,
		ElixirItemID,
		ElixirLockpicking,
		ElixirLumberjacking,
		ElixirMacing,
		ElixirMagicResist,
		ElixirMeditation,
		ElixirMining,
		ElixirMusicianship,
		ElixirParry,
		ElixirPeacemaking,
		ElixirPoisoning,
		ElixirProvocation,
		ElixirRemoveTrap,
		ElixirSnooping,
		ElixirSpiritSpeak,
		ElixirStealing,
		ElixirStealth,
		ElixirSwords,
		ElixirTactics,
		ElixirTailoring,
		ElixirTasteID,
		ElixirTinkering,
		ElixirTracking,
		ElixirVeterinary,
		ElixirWrestling,
		MixtureSlime,
		MixtureIceSlime,
		MixtureFireSlime,
		MixtureDiseasedSlime,
		MixtureRadiatedSlime,
		LiquidFire,
		LiquidGoo,
		LiquidIce,
		LiquidRot,
		LiquidPain,
		Resurrect,
		SuperPotion,
		Repair,
		Durability,
		HairOil,
		HairDye,
		Frostbite,
		FrostbiteGreater,
		Transmutation
	}

	public abstract class BasePotion : Item, ICraftable
	{
		public const int MAX_ENHANCED_POTIONS = 50;

		public override Catalogs DefaultCatalog{ get{ return Catalogs.Potion; } }

		private PotionEffect m_PotionEffect;

		public PotionEffect PotionEffect
		{
			get
			{
				return m_PotionEffect;
			}
			set
			{
				m_PotionEffect = value;
				InvalidateProperties();
			}
		}

		public override int LabelNumber{ get{ return 1041314 + (int)m_PotionEffect; } }

		public BasePotion( int itemID, PotionEffect effect ) : base( itemID )
		{
			m_PotionEffect = effect;

			Stackable = true;
			Weight = 1.0;
			Built = true;
		}

		public BasePotion( Serial serial ) : base( serial )
		{
		}

		public virtual bool RequireFreeHand{ get{ return false; } }

		public static bool HasFreeHand( Mobile m )
		{
			Item handOne = m.FindItemOnLayer( Layer.OneHanded );
			Item handTwo = m.FindItemOnLayer( Layer.TwoHanded );

			if ( handTwo is BaseWeapon )
				handOne = handTwo;
			
			if ( handOne is BaseRanged )
			{
				BaseRanged ranged = (BaseRanged) handOne;

				if ( ranged.Balanced )
					return true;
			}

			if (	( handOne is IPugilistGlove ) || 
					( handOne is LevelThrowingGloves ) || 
					( handOne is GiftThrowingGloves ) || 
					( handOne is ThrowingGloves ) )
			{
				return true;
			}

			return ( handOne == null || handTwo == null );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

			if ( from.InRange( this.GetWorldLocation(), 1 ) )
			{
				if (!RequireFreeHand || HasFreeHand(from))
				{
					if (this is BaseExplosionPotion && Amount > 1)
					{
						BasePotion pot = (BasePotion)Activator.CreateInstance(this.GetType());

						if (pot != null)
						{
							Amount--;

							if (from.Backpack != null && !from.Backpack.Deleted)
							{
								from.Backpack.DropItem(pot);
							}
							else
							{
								pot.MoveToWorld(from.Location, from.Map);
							}
							pot.Drink( from );
						}
					}
					else
					{
						this.Drink( from );
					}
				}
				else
				{
					from.SendLocalizedMessage(502172); // You must have a free hand to drink a potion.
				}
			}
			else
			{
				from.SendLocalizedMessage( 502138 ); // That is too far away for you to use
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (int) m_PotionEffect );
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
					m_PotionEffect = (PotionEffect)reader.ReadInt();
					break;
				}
			}

			if( version ==  0 )
				Stackable = Core.ML;

			Built = true;
		}

		public abstract void Drink( Mobile from );

		public static void PlayDrinkEffect( Mobile m )
		{
			m.PlaySound( 0x2D6 );

			m.AddToBackpack( new Bottle() );

			if ( m.Body.IsHuman && !m.Mounted )
				m.Animate( 34, 5, 1, true, false, 0 );
		}

		public static int EnhancePotions( Mobile m )
		{
			int EP = Math.Min(MAX_ENHANCED_POTIONS, AosAttributes.GetValue( m, AosAttribute.EnhancePotions ));
			int skillBonus = m.Skills.Alchemy.Fixed / 30;
			return EP + skillBonus;
		}

		public static TimeSpan Scale( Mobile m, TimeSpan v )
		{
			double scalar = 1.0 + ( 0.01 * EnhancePotions( m ) );

			return TimeSpan.FromSeconds( v.TotalSeconds * scalar );
		}

		public static double Scale( Mobile m, double v )
		{
			double scalar = 1.0 + ( 0.01 * EnhancePotions( m ) );

			return v * scalar;
		}

		public static int Scale( Mobile m, int v )
		{
			return AOS.Scale( v, 100 + EnhancePotions( m ) );
		}

		public override bool StackWith( Mobile from, Item dropped, bool playSound )
		{
			if( dropped is BasePotion && ((BasePotion)dropped).m_PotionEffect == m_PotionEffect )
				return base.StackWith( from, dropped, playSound );

			return false;
		}

		public static void MakePillBottle( Item potion )
		{
			BasePotion pot = (BasePotion)potion;

			string newName = "";
			string fireName = "";
			string coldName = "";
			string liqName = "";
			string typeContainer = "bottle";
				if ( Utility.RandomBool() ){ typeContainer = "syringe"; }

			switch ( pot.PotionEffect )
			{
				case PotionEffect.Nightsight : 			newName = "cornea dilation pills"; 				if ( typeContainer == "syringe" ){ newName = "cornea dilation serum"; }			break;
				case PotionEffect.CureLesser : 			newName = "weak antidote pills"; 				if ( typeContainer == "syringe" ){ newName = "weak antidote serum"; }			break;
				case PotionEffect.Cure : 				newName = "antidote pills"; 					if ( typeContainer == "syringe" ){ newName = "antidote serum"; }				break;
				case PotionEffect.CureGreater : 		newName = "powerful antidote pills"; 			if ( typeContainer == "syringe" ){ newName = "powerful antidote serum"; }		break;
				case PotionEffect.Agility : 			newName = "amphetamine pills"; 					if ( typeContainer == "syringe" ){ newName = "amphetamine serum"; }				break;
				case PotionEffect.AgilityGreater : 		newName = "powerful amphetamine pills"; 		if ( typeContainer == "syringe" ){ newName = "powerful amphetamine serum"; }	break;
				case PotionEffect.Strength : 			newName = "steroid pills"; 						if ( typeContainer == "syringe" ){ newName = "steroid serum"; }					break;
				case PotionEffect.StrengthGreater : 	newName = "powerful steroid pills"; 			if ( typeContainer == "syringe" ){ newName = "powerful steroid serum"; }		break;
				case PotionEffect.PoisonLesser : 		newName = "weak cyanide pills"; 				if ( typeContainer == "syringe" ){ newName = "weak cyanide serum"; }			break;
				case PotionEffect.Poison : 				newName = "cyanide pills"; 						if ( typeContainer == "syringe" ){ newName = "cyanide serum"; }					break;
				case PotionEffect.PoisonGreater : 		newName = "powerful cyanide pills"; 			if ( typeContainer == "syringe" ){ newName = "powerful cyanide serum"; }		break;
				case PotionEffect.PoisonDeadly : 		newName = "deadly cyanide pills"; 				if ( typeContainer == "syringe" ){ newName = "deadly cyanide serum"; }			break;
				case PotionEffect.PoisonLethal : 		newName = "lethal cyanide pills"; 				if ( typeContainer == "syringe" ){ newName = "lethal cyanide serum"; }			break;
				case PotionEffect.Refresh : 			newName = "caffeine pills"; 					if ( typeContainer == "syringe" ){ newName = "thiamin serum"; }					break;
				case PotionEffect.RefreshTotal : 		newName = "powerful caffeine pills"; 			if ( typeContainer == "syringe" ){ newName = "powerful thiamin serum"; }		break;
				case PotionEffect.HealLesser : 			newName = "weak aspirin pills"; 				if ( typeContainer == "syringe" ){ newName = "weak ketamine serum"; }			break;
				case PotionEffect.Heal : 				newName = "aspirin pills"; 						if ( typeContainer == "syringe" ){ newName = "ketamine serum"; }				break;
				case PotionEffect.HealGreater : 		newName = "powerful aspirin pills"; 			if ( typeContainer == "syringe" ){ newName = "powerfule ketamine serum"; }		break;
				case PotionEffect.InvisibilityLesser : 	newName = "weak camouflage pills"; 				if ( typeContainer == "syringe" ){ newName = "weak camouflage serum"; }			break;
				case PotionEffect.Invisibility : 		newName = "camouflage pills"; 					if ( typeContainer == "syringe" ){ newName = "camouflage serum"; }				break;
				case PotionEffect.InvisibilityGreater : newName = "powerful camouflage pills"; 			if ( typeContainer == "syringe" ){ newName = "powerful camouflage serum"; }		break;
				case PotionEffect.RejuvenateLesser : 	newName = "weak super soldier pills"; 			if ( typeContainer == "syringe" ){ newName = "weak super soldier serum"; }		break;
				case PotionEffect.Rejuvenate : 			newName = "super soldier pills"; 				if ( typeContainer == "syringe" ){ newName = "super soldier serum"; }			break;
				case PotionEffect.RejuvenateGreater : 	newName = "powerful super soldier pills"; 		if ( typeContainer == "syringe" ){ newName = "powerful super soldier serum"; }	break;
				case PotionEffect.ManaLesser : 			newName = "weak psychoactive pills"; 			if ( typeContainer == "syringe" ){ newName = "weak psychoactive serum"; }		break;
				case PotionEffect.Mana : 				newName = "psychoactive pills"; 				if ( typeContainer == "syringe" ){ newName = "psychoactive serum"; }			break;
				case PotionEffect.ManaGreater : 		newName = "powerful psychoactive pills"; 		if ( typeContainer == "syringe" ){ newName = "powerfule psychoactive serum"; }	break;
				case PotionEffect.Invulnerability : 	newName = "phencyclidine pills"; 				if ( typeContainer == "syringe" ){ newName = "phencyclidine dilation serum"; }	break;
				case PotionEffect.Conflagration : 		fireName = "gasoline"; 							break;
				case PotionEffect.ConflagrationGreater: fireName = "diesel fuel"; 						break;
				case PotionEffect.Frostbite : 			coldName = "fire extinguisher"; 				break;
				case PotionEffect.FrostbiteGreater:		coldName = "halon extinguisher"; 				break;
				case PotionEffect.ExplosionLesser : 	liqName = "weak nitroglycerin"; 				break;
				case PotionEffect.Explosion : 			liqName = "nitroglycerin"; 						break;
				case PotionEffect.ExplosionGreater : 	liqName = "strong nitroglycerin"; 				break;
			}

			if ( fireName != "" )
			{
				potion.ItemID = 0x34D7;
				potion.Name = fireName;
				potion.Hue = 0;
			}
			if ( coldName != "" )
			{
				potion.ItemID = 0x3563;
				potion.Name = coldName;
				potion.Hue = 0;
				if ( coldName == "halon extinguisher" ){ potion.Hue = 0xB50; }
			}
			else if ( liqName != "" )
			{
				potion.ItemID = 0x1FDD;
				potion.Name = liqName;
				potion.Hue = Server.Items.PotionKeg.GetPotionColor( potion );
			}
			else if ( newName != "" )
			{
				potion.ItemID = 0x27FE;
					if ( typeContainer == "syringe" ){ potion.ItemID = 0x27FF; }
				potion.Name = newName;
				potion.Hue = Server.Items.PotionKeg.GetPotionColor( potion );
			}
		}

		#region ICraftable Members

		public int OnCraft( int quality, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			if ( craftSystem is DefAlchemy )
			{
				Container pack = from.Backpack;

				if ( pack != null )
				{
					List<PotionKeg> kegs = pack.FindItemsByType<PotionKeg>();

					for ( int i = 0; i < kegs.Count; ++i )
					{
						PotionKeg keg = kegs[i];

						if ( keg == null )
							continue;

						if ( keg.Held <= 0 || keg.Held >= 100 )
							continue;

						if ( keg.Type != PotionEffect )
							continue;

						++keg.Held;

						Consume();
						from.AddToBackpack( new Bottle() );

						return -1; // signal placed in keg
					}
				}
			}

			return 1;
		}

		#endregion
	}
}