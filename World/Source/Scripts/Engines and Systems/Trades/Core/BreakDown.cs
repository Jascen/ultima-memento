using System;
using Server;
using Server.Targeting;
using Server.Items;
using Server.Gumps;
using System.Linq;
using Server.Mobiles;

namespace Server.Engines.Craft
{
	public enum BreakDownResult
	{
		Success,
		Invalid,
		NoSkill,
		BadAmnt
	}

	public class BreakDown
	{
		public BreakDown()
		{
		}

		public static void Do( Mobile from, CraftSystem craftSystem, BaseTool tool )
		{
			int num = craftSystem.CanCraft( from, tool, null );

			if ( num > 0 && num != 1044267 )
			{
				from.SendGump( new CraftGump( from, craftSystem, tool, num ) );
			}
			else
			{
				from.Target = new InternalTarget( craftSystem, tool );
				from.SendLocalizedMessage( 1044273 ); // Target an item or container to recycle all items within it.
			}
		}

		private class InternalTarget : Target
		{
			private CraftSystem m_CraftSystem;
			private BaseTool m_Tool;

			public InternalTarget( CraftSystem craftSystem, BaseTool tool ) :  base ( 2, false, TargetFlags.None )
			{
				m_CraftSystem = craftSystem;
				m_Tool = tool;
			}

			private BreakDownResult BreakDown( Mobile from, Item item, CraftResource resource )
			{
				try
				{
					bool extraCloth = false;

					if ( Item.IsStandardResource( item.Resource ) && !Item.IsStandardResource( item.SubResource ) )
					{
						resource = item.SubResource;
						if ( CraftResources.GetType( resource ) == CraftResourceType.Fabric )
							extraCloth = true;
					}

					bool canBreakDown = (item.Catalog == Catalogs.Wax && m_CraftSystem is DefWaxingPot)
						|| CraftResources.GetType( resource ) == m_CraftSystem.BreakDownType
						|| CraftResources.GetType( resource ) == m_CraftSystem.BreakDownTypeAlt && m_CraftSystem.BreakDownTypeAlt != CraftResourceType.None;

					if ( !canBreakDown )
						return BreakDownResult.Invalid;

					Item resc = null;

					if ( item.Catalog == Catalogs.Wax && m_CraftSystem is DefWaxingPot )
					{
						if ( 50.0 > from.Skills[ m_CraftSystem.MainSkill ].Value )
							return BreakDownResult.NoSkill;

						resc = new Beeswax();
					}
					else
					{
						CraftResourceInfo info = CraftResources.GetInfo( resource );

						if ( info == null || info.ResourceTypes.Length == 0 )
							return BreakDownResult.Invalid;

						double difficulty = CraftResources.GetSkill( resource );
						if ( difficulty > from.Skills[ m_CraftSystem.MainSkill ].Value )
							return BreakDownResult.NoSkill;

						Type resourceType = info.ResourceTypes[0];
						resc = (Item)Activator.CreateInstance( resourceType );
					}

					resc.Amount = Math.Min(60, BaseItemBreakDown( item ));

					if ( resc.Amount < 2 )
					{
						resc.Delete();
						return BreakDownResult.BadAmnt;
					}

					if ( from is PlayerMobile && false == ( (PlayerMobile)from ).Preferences.ColorlessFabricBreakdown && CraftResources.GetType( resc.Resource ) == CraftResourceType.Fabric )
						resc.Hue = item.Hue;

					resc.Amount = ( int )( resc.Amount / 2 );

					if ( resc.Amount > 0 && item.Catalog == Catalogs.Stone )
					{
						CraftResource rec = resc.Resource;
						int rock = resc.Amount;
						resc.Delete();

						if ( rec == CraftResource.DullCopper ){ resc = new DullCopperGranite(); }
						else if ( rec == CraftResource.ShadowIron ){ resc = new ShadowIronGranite(); }
						else if ( rec == CraftResource.Copper ){ resc = new CopperGranite(); }
						else if ( rec == CraftResource.Bronze ){ resc = new BronzeGranite(); }
						else if ( rec == CraftResource.Gold ){ resc = new GoldGranite(); }
						else if ( rec == CraftResource.Agapite ){ resc = new AgapiteGranite(); }
						else if ( rec == CraftResource.Verite ){ resc = new VeriteGranite(); }
						else if ( rec == CraftResource.Valorite ){ resc = new ValoriteGranite(); }
						else if ( rec == CraftResource.Nepturite ){ resc = new NepturiteGranite(); }
						else if ( rec == CraftResource.Obsidian ){ resc = new ObsidianGranite(); }
						else if ( rec == CraftResource.Steel ){ resc = new SteelGranite(); }
						else if ( rec == CraftResource.Brass ){ resc = new BrassGranite(); }
						else if ( rec == CraftResource.Mithril ){ resc = new MithrilGranite(); }
						else if ( rec == CraftResource.Xormite ){ resc = new XormiteGranite(); }
						else if ( rec == CraftResource.Dwarven ){ resc = new DwarvenGranite(); }
						else { resc = new Granite(); }

						resc.Amount = rock;
					}

					if ( extraCloth )
						resc.Amount = resc.Amount * 5;

					if ( item is BaseTrinket && item.Catalog == Catalogs.Jewelry && ((BaseTrinket)item).GemType != GemType.None )
					{
						Item gem = null;
						if ( ((BaseTrinket)item).GemType == GemType.StarSapphire )
							gem = new StarSapphire();
						else if ( ((BaseTrinket)item).GemType == GemType.Emerald )
							gem = new Emerald();
						else if ( ((BaseTrinket)item).GemType == GemType.Sapphire )
							gem = new Sapphire();
						else if ( ((BaseTrinket)item).GemType == GemType.Ruby )
							gem = new Ruby();
						else if ( ((BaseTrinket)item).GemType == GemType.Citrine )
							gem = new Citrine();
						else if ( ((BaseTrinket)item).GemType == GemType.Amethyst )
							gem = new Amethyst();
						else if ( ((BaseTrinket)item).GemType == GemType.Tourmaline )
							gem = new Tourmaline();
						else if ( ((BaseTrinket)item).GemType == GemType.Amber )
							gem = new Amber();
						else if ( ((BaseTrinket)item).GemType == GemType.Diamond )
							gem = new Diamond();
						else if ( ((BaseTrinket)item).GemType == GemType.Pearl )
							gem = new Oyster();

						if ( gem != null )
							BaseContainer.PutStuffInContainer( from, 2, gem );

					}

					item.Delete();
					BaseContainer.PutStuffInContainer( from, 2, resc );

					m_CraftSystem.PlayCraftEffect( from );
					return BreakDownResult.Success;
				}
				catch
				{
				}

				return BreakDownResult.Invalid;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Item )
				{
					if ( !((Item)targeted).IsChildOf( from.Backpack ) ) 
					{
						from.SendMessage( "You can only do this with items in your pack." );
						return;
					}

					int num = m_CraftSystem.CanCraft( from, m_Tool, null );

					if ( num > 0 )
					{
						if ( num == 1044267 )
						{
							bool anvil, forge;
				
							DefBlacksmithy.CheckAnvilAndForge( from, 2, out anvil, out forge );

							if ( !anvil )
								num = 1044266; // You must be near an anvil
							else if ( !forge )
								num = 1044265; // You must be near a forge.
						}
						
						from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, num ) );
					}
					else if ( targeted is Container )
					{
						var container = (Container)targeted;
						var confirmationGump = new ConfirmationGump(
							from,
							TextDefinition.GetColorizedText("Break down", HtmlColors.RED) + " container contents",
							string.Format("Are you absolutely sure you want to break down all '{0}' items in the container '{1}' ?", 
								TextDefinition.GetColorizedText(container.TotalItems.ToString(), HtmlColors.RED),
								TextDefinition.GetColorizedText(container.Name.ToString(), HtmlColors.RED)
							),
							() =>
								{
									if (!from.Alive) return;

									var success = false;
									var noSkill = false;
									var invalid = false;
									var badAmount = false;

									foreach (var item in container.Items.ToList())
									{
										if (item is Container) continue;

										// Bulk breakdown only supports items that can be crafted by the CraftSystem
										CraftItem craftItem = m_CraftSystem.CraftItems.SearchFor( item.GetType() );
										if ( craftItem == null || craftItem.Resources.Count == 0 )
										{
											invalid = true;
											continue;
										}

										BreakDownResult result = BreakDown(from, item, item.Resource);
										switch (result)
										{
											case BreakDownResult.Invalid: invalid = true; break;
											case BreakDownResult.NoSkill: noSkill = true; break;
											case BreakDownResult.BadAmnt: badAmount = true; break;
											case BreakDownResult.Success: success = true; break;
										}
									}

									if (invalid) from.SendLocalizedMessage(1044272); // You can't seem to break that item down.
									if (noSkill) from.SendLocalizedMessage(1044149); // You have no idea how to break this item down.
									if (badAmount) from.SendLocalizedMessage(1044144); // There is not enough here to break this down.
									if (success) from.SendLocalizedMessage(1044148); // You break the item down into ordinary resources.

									from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, null));
								},
							() =>
								{
									if (!from.Alive) return;

									from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, null));
								}
						);

						from.SendGump(confirmationGump);
					}
					else
					{
						BreakDownResult result = BreakDownResult.Invalid;
						int message;

						result = BreakDown( from, (Item)targeted, ((Item)targeted).Resource );

						switch ( result )
						{
							default:
							case BreakDownResult.Invalid: message = 1044272; break; // You can't seem to break that item down.
							case BreakDownResult.NoSkill: message = 1044149; break; // You have no idea how to break this item down.
							case BreakDownResult.Success: message = 1044148; break; // You break the item down into ordinary resources.
							case BreakDownResult.BadAmnt: message = 1044144; break; // There is not enough here to break this down.
						}
						
						from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, message ) );
					}
				}
			}
		}

		public static int BaseItemBreakDown( Item item )
		{
			if ( item is AlchemyTub ){ return 3; }
			else if ( item is AnvilEastDeed ){ return 5; }
			else if ( item is AnvilSouthDeed ){ return 5; }
			else if ( item is ArchmageRobe ){ return 16; }
			else if ( item is AssassinRobe ){ return 16; }
			else if ( item is AssassinSpike ){ return 3; }
			else if ( item is Axe ){ return 14; }
			else if ( item is AxleGears ){ return 1; }
			else if ( item is Backpack ){ return 3; }
			else if ( item is Bag ){ return 3; }
			else if ( item is Bandana ){ return 2; }
			else if ( item is Bardiche ){ return 18; }
			else if ( item is BarrelHoops ){ return 5; }
			else if ( item is BarrelTap ){ return 2; }
			else if ( item is Bascinet ){ return 15; }
			else if ( item is BattleAxe ){ return 14; }
			else if ( item is BearCap ){ return 4; }
			else if ( item is BeggarVest ){ return 8; }
			else if ( item is Belt ){ return 6; }
			else if ( item is BigBag ){ return 8; }
			else if ( item is BladedStaff ){ return 12; }
			else if ( item is BodySash ){ return 4; }
			else if ( item is Bokuto ){ return 6; }
			else if ( item is Bola ){ return 4; }
			else if ( item is BolaBall ){ return 10; }
			else if ( item is BoneArms ){ return 12; }
			else if ( item is BoneChest ){ return 22; }
			else if ( item is BoneGloves ){ return 8; }
			else if ( item is BoneHarvester ){ return 10; }
			else if ( item is BoneHelm ){ return 6; }
			else if ( item is BoneLegs ){ return 16; }
			else if ( item is BoneSkirt ){ return 16; }
			else if ( item is Bonnet ){ return 11; }
			else if ( item is Boots ){ return 8; }
			else if ( item is Bow ){ return 7; }
			else if ( item is Broadsword ){ return 10; }
			else if ( item is BrocadeGozaMatEastDeed ){ return 25; }
			else if ( item is BrocadeGozaMatSouthDeed ){ return 25; }
			else if ( item is BrocadeSquareGozaMatEastDeed ){ return 25; }
			else if ( item is BrocadeSquareGozaMatSouthDeed ){ return 25; }
			else if ( item is BronzeShield ){ return 12; }
			else if ( item is Buckler ){ return 10; }
			else if ( item is ButcherKnife ){ return 2; }
			else if ( item is Candelabra ){ return 4; }
			else if ( item is CandelabraStand ){ return 8; }
			else if ( item is CandleLarge ){ return 2; }
			else if ( item is Cap ){ return 11; }
			else if ( item is CarpenterTools ){ return 4; }
			else if ( item is ChainChest ){ return 20; }
			else if ( item is ChainCoif ){ return 10; }
			else if ( item is ChainHatsuburi ){ return 20; }
			else if ( item is ChainLegs ){ return 18; }
			else if ( item is ChainSkirt ){ return 18; }
			else if ( item is ChampionShield ){ return 18; }
			else if ( item is ChaosRobe ){ return 16; }
			else if ( item is ChaosShield ){ return 25; }
			else if ( item is Claymore ){ return 16; }
			else if ( item is Cleaver ){ return 3; }
			else if ( item is Cloak ){ return 14; }
			else if ( item is ClockLeft ){ return 1; }
			else if ( item is ClockParts ){ return 1; }
			else if ( item is ClockParts ){ return 1; }
			else if ( item is ClockRight ){ return 1; }
			else if ( item is CloseHelm ){ return 15; }
			else if ( item is ClothCowl ){ return 12; }
			else if ( item is ClothHood ){ return 12; }
			else if ( item is ClothNinjaHood ){ return 13; }
			else if ( item is ClothNinjaJacket ){ return 12; }
			else if ( item is Club ){ return 5; }
			else if ( item is ColoredWallTorch ){ return 5; }
			else if ( item is CompositeBow ){ return 7; }
			else if ( item is CrescentBlade ){ return 14; }
			else if ( item is CrestedShield ){ return 18; }
			else if ( item is Crossbow ){ return 7; }
			else if ( item is CulinarySet ){ return 4; }
			else if ( item is CultistRobe ){ return 16; }
			else if ( item is Cutlass ){ return 8; }
			else if ( item is Dagger ){ return 3; }
			else if ( item is Daisho ){ return 15; }
			else if ( item is DarkShield ){ return 18; }
			else if ( item is DeadMask ){ return 12; }
			else if ( item is DecorativePlateKabuto ){ return 25; }
			else if ( item is DeerCap ){ return 4; }
			else if ( item is DiamondMace ){ return 14; }
			else if ( item is DoubleAxe ){ return 12; }
			else if ( item is DoubleBladedStaff ){ return 16; }
			else if ( item is Doublet ){ return 8; }
			else if ( item is DragonArms ){ return 36; }
			else if ( item is DragonBardingDeed ){ return 750; }
			else if ( item is DragonChest ){ return 50; }
			else if ( item is DragonGloves ){ return 24; }
			else if ( item is DragonHelm ){ return 30; }
			else if ( item is DragonLegs ){ return 40; }
			else if ( item is DragonPedStatue ){ return 20; }
			else if ( item is DragonRobe ){ return 16; }
			else if ( item is DrakboneBracers ){ return 12; }
			else if ( item is DrakboneGreaves ){ return 14; }
			else if ( item is DrakboneGuantlets ){ return 6; }
			else if ( item is DrakboneHelm ){ return 24; }
			else if ( item is DrakboneTunic ){ return 19; }
			else if ( item is DreadHelm ){ return 15; }
			else if ( item is DruidCauldron ){ return 5; }
			else if ( item is ElegantRobe ){ return 16; }
			else if ( item is ElvenCompositeLongbow ){ return 7; }
			else if ( item is ElvenMachete ){ return 10; }
			else if ( item is ElvenShield ){ return 18; }
			else if ( item is ElvenSpellblade ){ return 8; }
			else if ( item is ExecutionersAxe ){ return 14; }
			else if ( item is ExquisiteRobe ){ return 16; }
			else if ( item is FancyDress ){ return 12; }
			else if ( item is FancyHood ){ return 12; }
			else if ( item is FancyRobe ){ return 16; }
			else if ( item is FancyShirt ){ return 8; }
			else if ( item is FancyWindChimes ){ return 15; }
			else if ( item is FeatheredHat ){ return 12; }
			else if ( item is FemaleKimono ){ return 16; }
			else if ( item is FemaleLeatherChest ){ return 8; }
			else if ( item is FemalePlateChest ){ return 20; }
			else if ( item is FemaleStuddedChest ){ return 10; }
			else if ( item is FletcherTools ){ return 3; }
			else if ( item is FloppyHat ){ return 11; }
			else if ( item is FoolsCoat ){ return 16; }
			else if ( item is ForkLeft ){ return 1; }
			else if ( item is ForkRight ){ return 1; }
			else if ( item is FormalCoat ){ return 16; }
			else if ( item is FormalRobe ){ return 16; }
			else if ( item is FormalShirt ){ return 16; }
			else if ( item is Fukiya ){ return 6; }
			else if ( item is FullApron ){ return 10; }
			else if ( item is GargoyleFlightStatue ){ return 8; }
			else if ( item is GargoyleStatue ){ return 4; }
			else if ( item is Gears ){ return 2; }
			else if ( item is GiantBag ){ return 9; }
			else if ( item is GildedDarkRobe ){ return 16; }
			else if ( item is GildedDress ){ return 16; }
			else if ( item is GildedLightRobe ){ return 16; }
			else if ( item is GildedRobe ){ return 16; }
			else if ( item is Globe ){ return 4; }
			else if ( item is GnarledStaff ){ return 7; }
			else if ( item is Goblet ){ return 2; }
			else if ( item is GozaMatEastDeed ){ return 25; }
			else if ( item is GozaMatSouthDeed ){ return 25; }
			else if ( item is GraveSpade ){ return 4; }
			else if ( item is GuardsmanShield ){ return 18; }
			else if ( item is GygaxStatue ){ return 100; }
			else if ( item is Hakama ){ return 16; }
			else if ( item is HakamaShita ){ return 14; }
			else if ( item is Halberd ){ return 20; }
			else if ( item is HalfApron ){ return 6; }
			else if ( item is HammerPick ){ return 16; }
			else if ( item is Hammers ){ return 6; }
			else if ( item is Harpoon ){ return 12; }
			else if ( item is HarpoonRope ){ return 1; }
			else if ( item is Hatchet ){ return 4; }
			else if ( item is HeaterShield ){ return 18; }
			else if ( item is HeatingStand ){ return 4; }
			else if ( item is HeavyCrossbow ){ return 10; }
			else if ( item is HeavyPlateJingasa ){ return 20; }
			else if ( item is Helmet ){ return 15; }
			else if ( item is HikingBoots ){ return 8; }
			else if ( item is Hinge ){ return 2; }
			else if ( item is HoodedMantle ){ return 12; }
			else if ( item is HorseArmor ){ return 650; }
			else if ( item is JesterGarb ){ return 16; }
			else if ( item is JesterHat ){ return 15; }
			else if ( item is JesterSuit ){ return 24; }
			else if ( item is JeweledShield ){ return 18; }
			else if ( item is JewelryBracelet ){ return 2; }
			else if ( item is JewelryCirclet ){ return 3; }
			else if ( item is JewelryEarrings ){ return 2; }
			else if ( item is JewelryNecklace ){ return 3; }
			else if ( item is JewelryRing ){ return 2; }
			else if ( item is JinBaori ){ return 12; }
			else if ( item is JokerRobe ){ return 16; }
			else if ( item is Kama ){ return 14; }
			else if ( item is Kamishimo ){ return 15; }
			else if ( item is Kasa ){ return 12; }
			else if ( item is Katana ){ return 8; }
			else if ( item is Keg ){ return 3; }
			else if ( item is Key ){ return 3; }
			else if ( item is KeyRing ){ return 2; }
			else if ( item is Kilt ){ return 8; }
			else if ( item is KnifeLeft ){ return 1; }
			else if ( item is KnifeRight ){ return 1; }
			else if ( item is Kryss ){ return 8; }
			else if ( item is Lajatang ){ return 25; }
			else if ( item is Lance ){ return 20; }
			else if ( item is Lantern ){ return 2; }
			else if ( item is LargeBag ){ return 6; }
			else if ( item is LargeBattleAxe ){ return 12; }
			else if ( item is LargeForgeEastDeed ){ return 5; }
			else if ( item is LargeForgeSouthDeed ){ return 5; }
			else if ( item is LargeKnife ){ return 3; }
			else if ( item is LargePegasusStatue ){ return 16; }
			else if ( item is LargeSack ){ return 7; }
			else if ( item is LargeStatueLion ){ return 24; }
			else if ( item is LargeStatueWolf ){ return 24; }
			else if ( item is Leafblade ){ return 5; }
			else if ( item is LeatherArms ){ return 4; }
			else if ( item is LeatherBoots ){ return 8; }
			else if ( item is LeatherBustierArms ){ return 6; }
			else if ( item is LeatherCap ){ return 2; }
			else if ( item is LeatherChest ){ return 12; }
			else if ( item is LeatherCloak ){ return 10; }
			else if ( item is LeatherDo ){ return 12; }
			else if ( item is LeatherGloves ){ return 3; }
			else if ( item is LeatherGorget ){ return 4; }
			else if ( item is LeatherHaidate ){ return 12; }
			else if ( item is LeatherHiroSode ){ return 5; }
			else if ( item is LeatherJingasa ){ return 4; }
			else if ( item is LeatherLegs ){ return 10; }
			else if ( item is LeatherMempo ){ return 8; }
			else if ( item is LeatherNinjaBelt ){ return 5; }
			else if ( item is LeatherNinjaHood ){ return 14; }
			else if ( item is LeatherNinjaJacket ){ return 13; }
			else if ( item is LeatherNinjaMitts ){ return 12; }
			else if ( item is LeatherNinjaPants ){ return 13; }
			else if ( item is LeatherRobe ){ return 18; }
			else if ( item is LeatherSandals ){ return 4; }
			else if ( item is LeatherShoes ){ return 6; }
			else if ( item is LeatherShorts ){ return 8; }
			else if ( item is LeatherSkirt ){ return 6; }
			else if ( item is LeatherSoftBoots ){ return 8; }
			else if ( item is LeatherSuneate ){ return 12; }
			else if ( item is LeatherThighBoots ){ return 10; }
			else if ( item is LeatherworkingTools ){ return 2; }
			else if ( item is LightPlateJingasa ){ return 20; }
			else if ( item is Lockpick ){ return 1; }
			else if ( item is LoinCloth ){ return 6; }
			else if ( item is LongPants ){ return 8; }
			else if ( item is Longsword ){ return 12; }
			else if ( item is Mace ){ return 6; }
			else if ( item is MagicalShortbow ){ return 7; }
			else if ( item is MagistrateRobe ){ return 16; }
			else if ( item is MaleKimono ){ return 16; }
			else if ( item is MapmakersPen ){ return 1; }
			else if ( item is Maul ){ return 10; }
			else if ( item is MediumStatueLion ){ return 16; }
			else if ( item is MedusaStatue ){ return 8; }
			else if ( item is MetalKiteShield ){ return 16; }
			else if ( item is MetalShield ){ return 14; }
			else if ( item is MortarPestle ){ return 3; }
			else if ( item is NecromancerRobe ){ return 16; }
			else if ( item is NinjaTabi ){ return 10; }
			else if ( item is NoDachi ){ return 18; }
			else if ( item is NorseHelm ){ return 15; }
			else if ( item is Obi ){ return 6; }
			else if ( item is OilCloth ){ return 1; }
			else if ( item is OniwabanBoots ){ return 8; }
			else if ( item is OniwabanGloves ){ return 3; }
			else if ( item is OniwabanHood ){ return 2; }
			else if ( item is OniwabanLeggings ){ return 10; }
			else if ( item is OniwabanTunic ){ return 12; }
			else if ( item is OrderShield ){ return 25; }
			else if ( item is OrnateAxe ){ return 16; }
			else if ( item is OrnateRobe ){ return 16; }
			else if ( item is PaperLantern ){ return 10; }
			else if ( item is PewterMug ){ return 2; }
			else if ( item is Pickaxe ){ return 4; }
			else if ( item is Pike ){ return 12; }
			else if ( item is PirateCoat ){ return 16; }
			else if ( item is PirateHat ){ return 12; }
			else if ( item is PiratePants ){ return 8; }
			else if ( item is Pitchfork ){ return 12; }
			else if ( item is Pitchforks ){ return 12; }
			else if ( item is PlainDress ){ return 10; }
			else if ( item is Plate ){ return 2; }
			else if ( item is PlateArms ){ return 18; }
			else if ( item is PlateBattleKabuto ){ return 25; }
			else if ( item is PlateChest ){ return 25; }
			else if ( item is PlateDo ){ return 28; }
			else if ( item is PlateGloves ){ return 12; }
			else if ( item is PlateGorget ){ return 10; }
			else if ( item is PlateHaidate ){ return 20; }
			else if ( item is PlateHatsuburi ){ return 20; }
			else if ( item is PlateHelm ){ return 15; }
			else if ( item is PlateHiroSode ){ return 16; }
			else if ( item is PlateLegs ){ return 20; }
			else if ( item is PlateMempo ){ return 18; }
			else if ( item is PlateSkirt ){ return 20; }
			else if ( item is PlateSuneate ){ return 20; }
			else if ( item is PotionKeg ){ return 1; }
			else if ( item is Pouch ){ return 2; }
			else if ( item is PriestRobe ){ return 16; }
			else if ( item is ProphetRobe ){ return 16; }
			else if ( item is PugilistMits ){ return 8; }
			else if ( item is QuarterStaff ){ return 6; }
			else if ( item is RadiantScimitar ){ return 10; }
			else if ( item is RepeatingCrossbow ){ return 10; }
			else if ( item is RingmailArms ){ return 14; }
			else if ( item is RingmailChest ){ return 18; }
			else if ( item is RingmailGloves ){ return 10; }
			else if ( item is RingmailLegs ){ return 16; }
			else if ( item is RingmailSkirt ){ return 16; }
			else if ( item is Robe ){ return 16; }
			else if ( item is RockUrn ){ return 6; }
			else if ( item is RockVase ){ return 6; }
			else if ( item is RoundPaperLantern ){ return 10; }
			else if ( item is RoyalArms ){ return 18; }
			else if ( item is RoyalBoots ){ return 12; }
			else if ( item is RoyalCape ){ return 14; }
			else if ( item is RoyalChest ){ return 25; }
			else if ( item is RoyalCoat ){ return 16; }
			else if ( item is RoyalGloves ){ return 12; }
			else if ( item is RoyalGorget ){ return 10; }
			else if ( item is RoyalHelm ){ return 15; }
			else if ( item is RoyalLongSkirt ){ return 10; }
			else if ( item is RoyalRobe ){ return 16; }
			else if ( item is RoyalShield ){ return 18; }
			else if ( item is RoyalShirt ){ return 16; }
			else if ( item is RoyalSkirt ){ return 8; }
			else if ( item is RoyalsLegs ){ return 20; }
			else if ( item is RoyalSword ){ return 14; }
			else if ( item is RoyalVest ){ return 8; }
			else if ( item is RuggedBackpack ){ return 4; }
			else if ( item is RuneBlade ){ return 12; }
			else if ( item is RusticShirt ){ return 16; }
			else if ( item is RusticVest ){ return 8; }
			else if ( item is SageRobe ){ return 16; }
			else if ( item is Sai ){ return 12; }
			else if ( item is SailorPants ){ return 6; }
			else if ( item is SamuraiTabi ){ return 6; }
			else if ( item is Sandals ){ return 4; }
			else if ( item is SavageArms ){ return 16; }
			else if ( item is SavageChest ){ return 26; }
			else if ( item is SavageGloves ){ return 12; }
			else if ( item is SavageHelm ){ return 10; }
			else if ( item is SavageLegs ){ return 20; }
			else if ( item is ScaledArms ){ return 54; }
			else if ( item is ScaledChest ){ return 75; }
			else if ( item is ScaledGloves ){ return 36; }
			else if ( item is ScaledGorget ){ return 30; }
			else if ( item is ScaledHelm ){ return 45; }
			else if ( item is ScaledLegs ){ return 60; }
			else if ( item is ScaledShield ){ return 18; }
			else if ( item is ScalemailShield ){ return 14; }
			else if ( item is Scales ){ return 4; }
			else if ( item is ScalingTools ){ return 4; }
			else if ( item is ScalyArms ){ return 18; }
			else if ( item is ScalyBoots ){ return 14; }
			else if ( item is ScalyChest ){ return 25; }
			else if ( item is ScalyGloves ){ return 12; }
			else if ( item is ScalyGorget ){ return 10; }
			else if ( item is ScalyHelm ){ return 15; }
			else if ( item is ScalyLegs ){ return 20; }
			else if ( item is Scepter ){ return 10; }
			else if ( item is ScholarRobe ){ return 16; }
			else if ( item is Scimitar ){ return 10; }
			else if ( item is Scissors ){ return 2; }
			else if ( item is ScribesPen ){ return 1; }
			else if ( item is Scythe ){ return 14; }
			else if ( item is SewingKit ){ return 2; }
			else if ( item is Sextant ){ return 1; }
			else if ( item is SextantParts ){ return 1; }
			else if ( item is SextantParts ){ return 4; }
			else if ( item is ShepherdsCrook ){ return 7; }
			else if ( item is ShinobiCowl ){ return 2; }
			else if ( item is ShinobiHood ){ return 2; }
			else if ( item is ShinobiMask ){ return 2; }
			else if ( item is ShinobiRobe ){ return 18; }
			else if ( item is Shirt ){ return 8; }
			else if ( item is Shoes ){ return 6; }
			else if ( item is ShojiLantern ){ return 10; }
			else if ( item is ShortPants ){ return 6; }
			else if ( item is ShortSpear ){ return 6; }
			else if ( item is ShortSword ){ return 10; }
			else if ( item is Shuriken ){ return 5; }
			else if ( item is SkinningKnifeTool ){ return 2; }
			else if ( item is Skirt ){ return 10; }
			else if ( item is SkullCap ){ return 2; }
			else if ( item is SkullMug ){ return 2; }
			else if ( item is SmallForgeDeed ){ return 5; }
			else if ( item is SmallPlateJingasa ){ return 20; }
			else if ( item is SmallStatueAngel ){ return 4; }
			else if ( item is SmallStatueDragon ){ return 4; }
			else if ( item is SmallStatueLion ){ return 8; }
			else if ( item is SmallStatueMan ){ return 4; }
			else if ( item is SmallStatueNoble ){ return 4; }
			else if ( item is SmallStatuePegasus ){ return 4; }
			else if ( item is SmallStatueSkull ){ return 4; }
			else if ( item is SmallStatueWoman ){ return 4; }
			else if ( item is SmithHammer ){ return 4; }
			else if ( item is SorcererRobe ){ return 16; }
			else if ( item is Spade ){ return 4; }
			else if ( item is Spear ){ return 12; }
			else if ( item is SphinxStatue ){ return 8; }
			else if ( item is SpiderRobe ){ return 16; }
			else if ( item is SpikedClub ){ return 6; }
			else if ( item is SpoonLeft ){ return 1; }
			else if ( item is SpoonRight ){ return 1; }
			else if ( item is Springs ){ return 2; }
			else if ( item is Spyglass ){ return 4; }
			else if ( item is SquareGozaMatEastDeed ){ return 25; }
			else if ( item is SquareGozaMatSouthDeed ){ return 25; }
			else if ( item is SquireShirt ){ return 16; }
			else if ( item is StagCap ){ return 4; }
			else if ( item is StandardPlateKabuto ){ return 25; }
			else if ( item is StatueAdventurer ){ return 8; }
			else if ( item is StatueAmazon ){ return 8; }
			else if ( item is StatueAngelTall ){ return 24; }
			else if ( item is StatueBust ){ return 4; }
			else if ( item is StatueCapeWarrior ){ return 24; }
			else if ( item is StatueDaemon ){ return 24; }
			else if ( item is StatueDemonicFace ){ return 8; }
			else if ( item is StatueDesertGod ){ return 16; }
			else if ( item is StatueDruid ){ return 8; }
			else if ( item is StatueDwarf ){ return 16; }
			else if ( item is StatueElvenKnight ){ return 8; }
			else if ( item is StatueElvenPriestess ){ return 8; }
			else if ( item is StatueElvenSorceress ){ return 8; }
			else if ( item is StatueElvenWarrior ){ return 8; }
			else if ( item is StatueFighter ){ return 8; }
			else if ( item is StatueGargoyleBust ){ return 6; }
			else if ( item is StatueGargoyleTall ){ return 8; }
			else if ( item is StatueGateGuardian ){ return 32; }
			else if ( item is StatueGiantWarrior ){ return 32; }
			else if ( item is StatueGryphon ){ return 10; }
			else if ( item is StatueGuardian ){ return 32; }
			else if ( item is StatueHorseRider ){ return 16; }
			else if ( item is StatueMermaid ){ return 10; }
			else if ( item is StatueMinotaurAttack ){ return 16; }
			else if ( item is StatueMinotaurDefend ){ return 16; }
			else if ( item is StatueNoble ){ return 8; }
			else if ( item is StatuePriest ){ return 8; }
			else if ( item is StatueSeaHorse ){ return 10; }
			else if ( item is StatueSwordsman ){ return 8; }
			else if ( item is StatueWiseManTall ){ return 24; }
			else if ( item is StatueWizard ){ return 8; }
			else if ( item is StatueWolfWinged ){ return 8; }
			else if ( item is StatueWomanTall ){ return 24; }
			else if ( item is StatueWomanWarriorPillar ){ return 16; }
			else if ( item is StoneAmphora ){ return 2; }
			else if ( item is StoneBenchLong ){ return 8; }
			else if ( item is StoneBenchShort ){ return 6; }
			else if ( item is StoneBlock ){ return 6; }
			else if ( item is StoneBuddhistSculpture ){ return 8; }
			else if ( item is StoneCasket ){ return 10; }
			else if ( item is StoneChairs ){ return 4; }
			else if ( item is StoneCoffin ){ return 10; }
			else if ( item is StoneColumn ){ return 10; }
			else if ( item is StoneFancyPedestal ){ return 8; }
			else if ( item is StoneGargoyleVase ){ return 6; }
			else if ( item is StoneGothicColumn ){ return 20; }
			else if ( item is StoneLargeAmphora ){ return 4; }
			else if ( item is StoneLargeVase ){ return 4; }
			else if ( item is StoneMingSculpture ){ return 6; }
			else if ( item is StoneMingUrn ){ return 4; }
			else if ( item is StoneOrnateAmphora ){ return 4; }
			else if ( item is StoneOrnateTallVase ){ return 8; }
			else if ( item is StoneOrnateUrn ){ return 6; }
			else if ( item is StoneOrnateVase ){ return 4; }
			else if ( item is StonePedestal ){ return 6; }
			else if ( item is StoneQinSculpture ){ return 6; }
			else if ( item is StoneQinUrn ){ return 4; }
			else if ( item is StoneRoughPillar ){ return 16; }
			else if ( item is StoneSarcophagus ){ return 10; }
			else if ( item is StoneSteps ){ return 6; }
			else if ( item is StoneTableLong ){ return 12; }
			else if ( item is StoneTableShort ){ return 10; }
			else if ( item is StoneTombStoneA ){ return 4; }
			else if ( item is StoneTombStoneB ){ return 4; }
			else if ( item is StoneTombStoneC ){ return 4; }
			else if ( item is StoneTombStoneD ){ return 4; }
			else if ( item is StoneTombStoneE ){ return 4; }
			else if ( item is StoneTombStoneF ){ return 4; }
			else if ( item is StoneTombStoneG ){ return 4; }
			else if ( item is StoneTombStoneH ){ return 4; }
			else if ( item is StoneTombStoneI ){ return 4; }
			else if ( item is StoneTombStoneJ ){ return 4; }
			else if ( item is StoneTombStoneK ){ return 4; }
			else if ( item is StoneTombStoneL ){ return 4; }
			else if ( item is StoneTombStoneM ){ return 4; }
			else if ( item is StoneTombStoneN ){ return 4; }
			else if ( item is StoneTombStoneO ){ return 4; }
			else if ( item is StoneTombStoneP ){ return 4; }
			else if ( item is StoneTombStoneQ ){ return 4; }
			else if ( item is StoneTombStoneR ){ return 4; }
			else if ( item is StoneTombStoneS ){ return 4; }
			else if ( item is StoneTombStoneT ){ return 4; }
			else if ( item is StoneVase ){ return 2; }
			else if ( item is StoneWizardTable ){ return 16; }
			else if ( item is StoneYuanSculpture ){ return 6; }
			else if ( item is StoneYuanUrn ){ return 4; }
			else if ( item is StrawHat ){ return 10; }
			else if ( item is StuddedArms ){ return 10; }
			else if ( item is StuddedBustierArms ){ return 8; }
			else if ( item is StuddedChest ){ return 14; }
			else if ( item is StuddedDo ){ return 14; }
			else if ( item is StuddedGloves ){ return 8; }
			else if ( item is StuddedGorget ){ return 6; }
			else if ( item is StuddedHaidate ){ return 14; }
			else if ( item is StuddedHiroSode ){ return 8; }
			else if ( item is StuddedLegs ){ return 12; }
			else if ( item is StuddedMempo ){ return 8; }
			else if ( item is StuddedSkirt ){ return 12; }
			else if ( item is StuddedSuneate ){ return 14; }
			else if ( item is SunShield ){ return 25; }
			else if ( item is Surcoat ){ return 14; }
			else if ( item is TallStatueLion ){ return 24; }
			else if ( item is TallStrawHat ){ return 13; }
			else if ( item is TattsukeHakama ){ return 16; }
			else if ( item is Tekagi ){ return 12; }
			else if ( item is TenFootPole ){ return 6; }
			else if ( item is Tessen ){ return 16; }
			else if ( item is Tetsubo ){ return 10; }
			else if ( item is ThighBoots ){ return 10; }
			else if ( item is ThrowingGloves ){ return 8; }
			else if ( item is ThrowingWeapon ){ return 1; }
			else if ( item is TinkerTools ){ return 2; }
			else if ( item is TrapKit ){ return 32; }
			else if ( item is TricorneHat ){ return 12; }
			else if ( item is Trumpet ){ return 20; }
			else if ( item is Tunic ){ return 12; }
			else if ( item is TwoHandedAxe ){ return 16; }
			else if ( item is VagabondRobe ){ return 16; }
			else if ( item is VampireRobe ){ return 16; }
			else if ( item is VikingSword ){ return 14; }
			else if ( item is VirtueShield ){ return 25; }
			else if ( item is Wakizashi ){ return 8; }
			else if ( item is WallTorch ){ return 5; }
			else if ( item is WarAxe ){ return 16; }
			else if ( item is WarCleaver ){ return 3; }
			else if ( item is WarFork ){ return 12; }
			else if ( item is WarHammer ){ return 16; }
			else if ( item is WarMace ){ return 14; }
			else if ( item is WaxingPot ){ return 10; }
			else if ( item is Whips ){ return 6; }
			else if ( item is WideBrimHat ){ return 12; }
			else if ( item is WildStaff ){ return 7; }
			else if ( item is WindChimes ){ return 15; }
			else if ( item is WitchCauldron ){ return 5; }
			else if ( item is WitchHat ){ return 15; }
			else if ( item is WizardHood ){ return 12; }
			else if ( item is WizardsHat ){ return 15; }
			else if ( item is WizardShirt ){ return 16; }
			else if ( item is WizardsStatue ){ return 100; }
			else if ( item is WolfCap ){ return 4; }
			else if ( item is WoodenKiteShield ){ return 8; }
			else if ( item is WoodenPlateArms ){ return 18; }
			else if ( item is WoodenPlateChest ){ return 25; }
			else if ( item is WoodenPlateGloves ){ return 12; }
			else if ( item is WoodenPlateGorget ){ return 10; }
			else if ( item is WoodenPlateHelm ){ return 15; }
			else if ( item is WoodenPlateLegs ){ return 20; }
			else if ( item is WoodenShield ){ return 9; }
			else if ( item is WoodworkingTools ){ return 2; }
			else if ( item is Yumi ){ return 10; }

			return 0;
		}
	}
}