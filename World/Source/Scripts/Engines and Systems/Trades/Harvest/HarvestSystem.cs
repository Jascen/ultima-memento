using System;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;
using Server.Gumps;
using Server.Misc;
using Server.Regions;
using System.Linq;

namespace Server.Engines.Harvest
{
	public abstract class HarvestSystem
	{
		private List<HarvestDefinition> m_Definitions;

		public List<HarvestDefinition> Definitions { get { return m_Definitions; } }

		public HarvestSystem()
		{
			m_Definitions = new List<HarvestDefinition>();
		}

		public static string HarvestSystemTxt( HarvestSystem system, Item item )
		{
			string harvest = null;

			if ( system is Mining )
				harvest = "Ore";
			else if ( system is Lumberjacking )
				harvest = "Wood";
			else if ( system is Fishing )
				harvest = "Fish";
			else if ( system is Librarian )
				harvest = "Books";
			else if ( system is GraveRobbing )
				harvest = "Graves";

			if ( harvest != null )
				harvest = "Gathering: " + harvest;

			return harvest;
		}

		public virtual bool CheckTool( Mobile from, Item tool )
		{
			bool wornOut = ( tool == null || tool.Deleted || (tool is IUsesRemaining && ((IUsesRemaining)tool).UsesRemaining <= 0) );

			if ( wornOut )
				from.SendLocalizedMessage( 1044038 ); // You have worn out your tool!

			return !wornOut;
		}

		public virtual bool CheckHarvest( Mobile from, Item tool )
		{
			return CheckTool( from, tool );
		}

		public virtual bool CheckHarvest( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			return CheckTool( from, tool );
		}

		public virtual bool CheckRange( Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, bool timed )
		{
			bool inRange = ( from.Map == map && from.InRange( loc, def.MaxRange ) );

			if ( !inRange )
				def.SendMessageTo( from, timed ? def.TimedOutOfRangeMessage : def.OutOfRangeMessage );

			return inRange;
		}

		public virtual bool CheckResources( Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, bool timed )
		{
			HarvestBank bank = def.GetBank( map, loc.X, loc.Y );
			bool available = ( bank != null && bank.Current > 0 );

			if ( !available )
				def.SendMessageTo( from, timed ? def.DoubleHarvestMessage : def.NoResourcesMessage );

			return available;
		}

		public virtual void OnBadHarvestTarget( Mobile from, Item tool, object toHarvest )
		{
		}

		public virtual void OnNoNearbyHarvest( Mobile from, Item tool )
		{
			if ( m_Definitions.Count > 0 && m_Definitions[0].NoResourcesMessage != null )
				m_Definitions[0].SendMessageTo( from, m_Definitions[0].NoResourcesMessage );
		}

		protected virtual void TryCreateHarvestArrow( Mobile from, IPoint3D location )
		{
			from.QuestArrow = new HarvestArrow( from, location );
			from.QuestArrow.Update();
		}

		public virtual bool TryStartNearbyHarvest( Mobile from, Item tool )
		{
			if ( !CheckHarvest( from, tool ) ) return false;

			var maxRange = Definitions.Max(x => x.MaxRange);
			foreach ( var target in GetNearbyHarvestTargets( from, tool, maxRange ) )
			{
				if ( !IsValidHarvestTarget( from, tool, target ) ) continue;

				if ( target is Static )
				{
					var staticItem = (Static)target;
					TryCreateHarvestArrow( from, staticItem.GetWorldLocation() );
				}
				else if ( target is IPoint3D )
				{
					TryCreateHarvestArrow( from, (IPoint3D)target );
				}

				StartHarvesting( from, tool, target );
				return true;
			}

			return false;
		}

		public virtual bool IsValidHarvestTarget( Mobile from, Item tool, object toHarvest )
		{
			int tileID;
			Map map;
			Point3D loc;

			if ( !GetHarvestDetails( from, tool, toHarvest, out tileID, out map, out loc ) )
				return false;

			HarvestDefinition def = GetDefinition( tileID );

			if ( def == null )
				return false;

			if ( from.Map != map || !from.InRange( loc, def.MaxRange ) )
				return false;

			if ( !from.InLOS( loc ) )
				return false;

			HarvestBank bank = def.GetBank( map, loc.X, loc.Y );

			if ( bank == null || bank.Current <= 0 )
				return false;

			return CheckHarvest( from, tool, def, toHarvest );
		}

		protected virtual IEnumerable<object> GetNearbyHarvestTargets( Mobile from, Item tool, int range )
		{
			Map map = from.Map;

			if ( map == null || map == Map.Internal )
				yield break;

			for ( var dx = -range; dx <= range; dx++ )
			{
				for ( var dy = -range; dy <= range; dy++ )
				{
					var x = from.X + dx;
					var y = from.Y + dy;

					foreach ( var special in GetNearbySpecialHarvestTargetsAt( from, tool, x, y ) )
						yield return special;

					var tileTarget = GetHarvestTargetAt( from, tool, map, x, y );

					if ( tileTarget != null )
						yield return tileTarget;
				}
			}
		}

		protected virtual IEnumerable<object> GetNearbySpecialHarvestTargetsAt( Mobile from, Item tool, int x, int y )
		{
			yield break;
		}

		protected virtual object GetHarvestTargetAt( Mobile from, Item tool, Map map, int x, int y )
		{
			StaticTile[] statics = map.Tiles.GetStaticTiles( x, y, true );

			for ( int i = 0; i < statics.Length; i++ )
			{
				var tile = statics[i];
				var tileID = ( tile.ID & 0x3FFF ) | 0x4000;

				if ( GetDefinition( tileID ) != null )
					return new StaticTarget( new Point3D( x, y, tile.Z ), tile.ID );
			}

			var landTile = map.Tiles.GetLandTile( x, y );
			var landID = landTile.ID & TileData.MaxLandValue;

			if ( GetDefinition( landID ) != null )
				return new LandTarget( new Point3D( x, y, landTile.Z ), map );

			return null;
		}

		public virtual object GetLock( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			/* Here we prevent multiple harvesting.
			 * 
			 * Some options:
			 *  - 'return tool;' : This will allow the player to harvest more than once concurrently, but only if they use multiple tools. This seems to be as OSI.
			 *  - 'return GetType();' : This will disallow multiple harvesting of the same type. That is, we couldn't mine more than once concurrently, but we could be both mining and lumberjacking.
			 *  - 'return typeof( HarvestSystem );' : This will completely restrict concurrent harvesting.
			 */

			return typeof( HarvestSystem );
		}

		public virtual void OnConcurrentHarvest( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
		}

		public virtual void OnHarvestStarted( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
		}

		public virtual bool BeginHarvesting( Mobile from, Item tool )
		{
			if ( !CheckHarvest( from, tool ) )
				return false;

			if ( MySettings.S_AllowMacroResources )
			{ 
				from.Target = new HarvestTarget( tool, this );
				if ( Utility.RandomDouble() < 0.1 )
				{
					from.SendMessage("Target yourself to automatically choose a nearby target");
				}
			}
			else
			{
				CaptchaGump.sendCaptcha(from, HarvestSystem.SendHarvestTarget, new object[]{tool, this});
			}

			return true;
		}

        public static void SendHarvestTarget( Mobile from, object o )
        {
            if (!(o is object[]))
                return;
            object[] arglist = (object[])o;
 
            if (arglist.Length != 2)
                return;
 
            if (!(arglist[0] is Item))
                return;
 
            if (!(arglist[1] is HarvestSystem))
                return;
               
            from.Target = new HarvestTarget((Item)arglist[0], (HarvestSystem)arglist[1] );
        }

		public virtual void FinishHarvesting( Mobile from, Item tool, HarvestDefinition def, object toHarvest, object locked )
		{
			from.EndAction( locked );

			if ( !CheckHarvest( from, tool ) )
				return;

			int tileID;
			Map map;
			Point3D loc;

			if ( !GetHarvestDetails( from, tool, toHarvest, out tileID, out map, out loc ) )
			{
				OnBadHarvestTarget( from, tool, toHarvest );
				return;
			}
			else if ( !def.Validate( tileID ) )
			{
				OnBadHarvestTarget( from, tool, toHarvest );
				return;
			}
			
			if ( !CheckRange( from, tool, def, map, loc, true ) )
				return;
			else if ( !CheckResources( from, tool, def, map, loc, true ) )
				return;
			else if ( !CheckHarvest( from, tool, def, toHarvest ) )
				return;

			if ( SpecialHarvest( from, tool, def, map, loc ) )
				return;

			HarvestBank bank = def.GetBank( map, loc.X, loc.Y );

			if ( bank == null )
				return;

			HarvestVein vein = bank.Vein;

			if ( vein != null )
				vein = MutateVein( from, tool, def, bank, toHarvest, vein );

			if ( vein == null )
				return;

			HarvestResource primary = vein.PrimaryResource;
			HarvestResource fallback = vein.FallbackResource;
			HarvestResource resource = MutateResource( from, tool, def, map, loc, vein, primary, fallback );

			double skillValue = from.Skills[def.Skill].Value;
			double skillMin = resource.MinSkill;
			double skillMax = resource.MaxSkill;

			Type type = null;
			int usesToConsume = 1;
			bool testSkill = false;

			testSkill = from.CheckSkill( def.Skill, skillMin, skillMax );

			if ( skillValue >= resource.ReqSkill && testSkill )
			{
				type = GetResourceType( from, tool, def, map, loc, resource );

				if ( type != null )
					type = MutateType( type, from, tool, def, map, loc, resource );
				if ( type != null )
				{
					Item item = Construct( type, from );

					if ( item == null )
					{
						type = null;
					}
					else
					{
						if ( item.Stackable )
						{
							bool addUp = true;

							if ( tool is FishingPole && !Worlds.IsOnBoat( from ) && from.Skills[def.Skill].Base >= 50 )
							{
								addUp = false;
							}

							if ( addUp )
							{
								int skillCycle = MyServerSettings.Resources() - 1;

								while ( skillCycle > 0 )
								{
									from.CheckSkill( def.Skill, skillMin, skillMax );
									skillCycle--;
								}
							}

							Region reg = Region.Find( from.Location, from.Map );

							int amount = Math.Min(bank.Current, def.ConsumedPerHarvest);

							if ( item is BlankScroll )
							{
								amount = Utility.RandomMinMax( amount, (int)(amount+(from.Skills[SkillName.Inscribe].Value/10)) );
								from.SendMessage( "You find some blank scrolls.");
							}

							if ( from.Land == Land.IslesDread || ( reg.IsPartOf( "the Mines of Morinia" ) && item is BaseOre && Utility.RandomMinMax( 1, 3 ) > 1 ) )
								item.Amount = Math.Min(bank.Current, def.ConsumedPerIslesDreadHarvest);
							else
								item.Amount = amount;

							if (item is BaseOre) // Mining
							{
								var canMutate = ( item is AgapiteOre || item is VeriteOre || item is ValoriteOre ) && Utility.RandomMinMax( 1, 2 ) == 1;
								if ( canMutate )
								{
									if ( Worlds.IsExploringSeaAreas( from ) || reg.IsPartOf( "Shipwreck Grotto" ) || reg.IsPartOf( "Barnacled Cavern" ) || Server.Misc.Worlds.IsSeaTown( from.Location, from.Map ) )
									{
										int nepturiteOre = item.Amount;
										item.Delete();
										item = new NepturiteOre( nepturiteOre );
									}
									else if ( from.Land == Land.Underworld && from.Map == Map.SavagedEmpire )
									{
										int xormiteOre = item.Amount;
										item.Delete();
										item = new XormiteOre( xormiteOre );
									}
									else if ( from.Land == Land.Underworld )
									{
										int mithrilOre = item.Amount;
										item.Delete();
										item = new MithrilOre( mithrilOre );
									}
									else if ( from.Land == Land.Savaged )
									{
										int steelOre = item.Amount;
										item.Delete();
										item = new SteelOre( steelOre );
									}
									else if ( from.Land == Land.UmberVeil )
									{
										int brassOre = item.Amount;
										item.Delete();
										item = new BrassOre( brassOre );
									}
									else if ( from.Land == Land.Serpent )
									{
										int obsidianOre = item.Amount;
										item.Delete();
										item = new ObsidianOre( obsidianOre );
									}
								}

								from.SendMessage( "You dig up {0} {1} ore.", item.Amount < 2 ? "some" : item.Amount.ToString(), CraftResources.GetName(item.Resource) );
							}
							else if (item is BaseGranite) // Mining
							{
								var canMutate = ( item is AgapiteGranite || item is VeriteGranite || item is ValoriteGranite ) && Utility.RandomMinMax( 1, 2 ) == 1;
								if ( canMutate )
								{
									if ( Worlds.IsExploringSeaAreas( from ) || reg.IsPartOf( "Shipwreck Grotto" ) || reg.IsPartOf( "Barnacled Cavern" ) || Server.Misc.Worlds.IsSeaTown( from.Location, from.Map ) )
									{
										int nepturiteGranite = item.Amount;
										item.Delete();
										item = new NepturiteGranite( nepturiteGranite );
									}
									else if ( from.Land == Land.Underworld && from.Map == Map.SavagedEmpire )
									{
										int xormiteGranite = item.Amount;
										item.Delete();
										item = new XormiteGranite( xormiteGranite );
									}
									else if ( from.Land == Land.Underworld )
									{
										int mithrilGranite = item.Amount;
										item.Delete();
										item = new MithrilGranite( mithrilGranite );
									}
									else if ( from.Land == Land.Savaged )
									{
										int steelGranite = item.Amount;
										item.Delete();
										item = new SteelGranite( steelGranite );
									}
									else if ( from.Land == Land.UmberVeil )
									{
										int brassGranite = item.Amount;
										item.Delete();
										item = new BrassGranite( brassGranite );
									}
									else if ( from.Land == Land.Serpent )
									{
										int obsidianGranite = item.Amount;
										item.Delete();
										item = new ObsidianGranite( obsidianGranite );
									}
								}

								from.SendMessage( "You dig up {0} {1} granite.", item.Amount < 2 ? "some" : item.Amount.ToString(), CraftResources.GetName(item.Resource) );
							}
							else if (item is BaseLog ) // Lumberjacking
							{
								var canMutate = (item is Log) == false;
								if (canMutate)
								{
									if ( Utility.RandomBool() && ( Worlds.IsExploringSeaAreas( from ) || reg.IsPartOf( "Shipwreck Grotto" ) || reg.IsPartOf( "Barnacled Cavern" ) ) )
									{
										int driftWood = item.Amount;
										item.Delete();
										item = new DriftwoodLog( driftWood );
									}
									else if ( (item is AshLog || item is CherryLog || item is GoldenOakLog || item is HickoryLog || item is MahoganyLog) && reg.IsPartOf( typeof( NecromancerRegion ) ) )
									{
										int blackLog = item.Amount;
										item.Delete();
										item = new EbonyLog( blackLog );
									}
									else if ( (item is WalnutLog || item is RosewoodLog || item is PineLog || item is OakLog) && reg.IsPartOf( typeof( NecromancerRegion ) ) )
									{
										int ghostLog = item.Amount;
										item.Delete();
										item = new GhostLog( ghostLog );
									}
									else if ( from.Land == Land.Underworld )
									{
										int toughLog = item.Amount;
										item.Delete();
										item = new PetrifiedLog( toughLog );
									}
								}

								from.SendMessage( "You chop {0} {1} logs.", item.Amount < 2 ? "some" : item.Amount.ToString(), CraftResources.GetName(item.Resource) );
							}
							else if ( item is LesserCurePotion ) // Graverobbing - Mutation
							{
								item.Delete();
								item = Loot.RandomPotion( Utility.Random(4)+1, true );
							}
							else if ( item is Brimstone ) // Graverobbing - Mutation
							{
								item.Delete();
								item = Loot.RandomPossibleReagent();
							}
							else if ( item is HealScroll ) // Librarian - Mutation
							{
								item.Delete();
								item = Loot.RandomScroll( Utility.Random(4)+1 );
							}

							if ( tool is FishingPole && Server.Engines.Harvest.Fishing.IsNearHugeShipWreck( from ) && from.Skills[SkillName.Seafaring].Value >= Utility.RandomMinMax( 1, 250 ) )
							{
								Server.Engines.Harvest.Fishing.FishUpFromMajorWreck( from );
							}
							else if ( tool is FishingPole && Server.Engines.Harvest.Fishing.IsNearSpaceCrash( from ) && from.Skills[SkillName.Seafaring].Value >= Utility.RandomMinMax( 1, 250 ) )
							{
								Server.Engines.Harvest.Fishing.FishUpFromSpaceship( from );
							}
							else if ( tool is FishingPole && Server.Engines.Harvest.Fishing.IsNearUnderwaterRuins( from ) && from.Skills[SkillName.Seafaring].Value >= Utility.RandomMinMax( 1, 250 ) )
							{
								Server.Engines.Harvest.Fishing.FishUpFromRuins( from );
							}
						}
						else if ( item is WritingBook || item is LoreBook || item is DDRelicBook || item is Spellbook || item is ArtifactManual )
						{
							from.SendMessage( "You find a book.");
							if ( item is DDRelicBook ){ item.CoinPrice = item.CoinPrice + Utility.RandomMinMax( 1, (int)(from.Skills[SkillName.Inscribe].Value*2) ); }
							else if ( item is WritingBook ){ item.Name = "Book"; item.ItemID = RandomThings.GetRandomBookItemID(); }
							else if ( item is Spellbook ){ item.Delete(); item = Spellbook.MagicBook(); }
						}
						else if ( item is SomeRandomNote || item is ScrollClue || item is SpellScroll || item is DDRelicScrolls )
						{
							if ( item is WeakenScroll ){ item.Delete(); item = Loot.RandomScroll( 1 ); }
							else if ( item is ProtectionScroll ){ item.Delete(); item = Loot.RandomScroll( Utility.RandomMinMax(2,3) ); }
							else if ( item is UnlockScroll ){ item.Delete(); item = Loot.RandomScroll( Utility.RandomMinMax(4,5) ); }
							else if ( item is CurseScroll ){ item.Delete(); item = Loot.RandomScroll( Utility.RandomMinMax(6,7) ); }
							else if ( item is ParalyzeScroll ){ item.Delete(); item = Loot.RandomScroll( Utility.RandomMinMax(8,9) ); }
							else if ( item is ExplosionScroll ){ item.Delete(); item = Loot.RandomScroll( Utility.RandomMinMax(10,12) ); }

							from.SendMessage( "You find a scroll.");
							if ( item is DDRelicScrolls ){ item.CoinPrice = item.CoinPrice + Utility.RandomMinMax( 1, (int)(from.Skills[SkillName.Inscribe].Value*2) ); }
						}

						bank.Consume( item.Amount, from );

						if ( Give( from, item, true ) )
						{
							SendSuccessTo( from, item, resource );
						}
						else
						{
							SendPackFullTo( from, item, def, resource );
							item.Delete();
						}

						BonusHarvestResource bonus = def.GetBonusResource();

						Item bonusItem = null;
						if ( bonus != null && bonus.Type != null && skillValue >= bonus.ReqSkill )
						{
							bonusItem = Construct( bonus.Type, from );

							if ( Give( from, bonusItem, true ) )
							{
								bonus.SendSuccessTo( from );
							}
							else
							{
								bonusItem.Delete();
							}
						}

						// Servers that give more resources should not consume as many charges
						if ( MyServerSettings.Resources() == 1 ) 
							usesToConsume = item.Amount;

						EventSink.InvokeResourceHarvestSuccess(new ResourceHarvestSuccessArgs(from, tool, item, bonusItem, this));
					}
				}
			}

			// Failure - Consume a charge
			if ( type == null )
				bank.Consume( 1, from );

			if ( tool is IUsesRemaining )
			{
				IUsesRemaining toolWithUses = (IUsesRemaining)tool;

				toolWithUses.ShowUsesRemaining = true;
				toolWithUses.UsesRemaining -= usesToConsume;

				if ( toolWithUses.UsesRemaining < 1 )
				{
					tool.Delete();
					def.SendMessageTo( from, def.ToolBrokeMessage );
				}
			}

			if ( type == null )
				def.SendMessageTo( from, def.FailMessage );

			OnHarvestFinished( from, tool, def, vein, bank, resource, toHarvest );
		}

		public virtual void OnHarvestFinished( Mobile from, Item tool, HarvestDefinition def, HarvestVein vein, HarvestBank bank, HarvestResource resource, object harvested )
		{
			if (!MySettings.S_AutoRepeatHarvesting) return;
			if (tool.Deleted) return;
			if (from.FindItemOnLayer(tool.Layer) != tool) return; // Make sure it's still equipped

			Timer.DelayCall(TimeSpan.FromMilliseconds(500), () => StartHarvesting(from, tool, harvested));
		}

		public virtual bool SpecialHarvest( Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc )
		{
			return false;
		}

		public virtual Item Construct( Type type, Mobile from )
		{
			try{ return Activator.CreateInstance( type ) as Item; }
			catch{ return null; }
		}

		public virtual HarvestVein MutateVein( Mobile from, Item tool, HarvestDefinition def, HarvestBank bank, object toHarvest, HarvestVein vein )
		{
			return vein;
		}

		public virtual void SendSuccessTo( Mobile from, Item item, HarvestResource resource )
		{
			resource.SendSuccessTo( from );
		}

		public virtual void SendPackFullTo( Mobile from, Item item, HarvestDefinition def, HarvestResource resource )
		{
			def.SendMessageTo( from, def.PackFullMessage );
		}

		public virtual bool Give( Mobile m, Item item, bool placeAtFeet )
		{
			if (BaseContainer.PutStuffInContainer( m, 3, item )) return true;

			if ( !placeAtFeet )
				return false;

			Map map = m.Map;

			if ( map == null )
				return false;

			List<Item> atFeet = new List<Item>();

			foreach ( Item obj in m.GetItemsInRange( 0 ) )
				atFeet.Add( obj );

			for ( int i = 0; i < atFeet.Count; ++i )
			{
				Item check = atFeet[i];

				if ( check.StackWith( m, item, false ) )
					return true;
			}

			item.MoveToWorld( m.Location, map );
			return true;
		}

		public virtual Type MutateType( Type type, Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource )
		{
			return from.Region.GetResource( type );
		}

		public virtual Type GetResourceType( Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource )
		{
			if ( resource.Types.Length > 0 )
				return resource.Types[Utility.Random( resource.Types.Length )];

			return null;
		}

		public virtual HarvestResource MutateResource( Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestVein vein, HarvestResource primary, HarvestResource fallback )
		{
			bool racialBonus = (def.RaceBonus && from.Race == Race.Elf );

			if( vein.ChanceToFallback > (Utility.RandomDouble() + (racialBonus ? .20 : 0)) )
				return fallback;

			double skillValue = from.Skills[def.Skill].Value;

			if ( fallback != null && (skillValue < primary.ReqSkill || skillValue < primary.MinSkill) )
				return fallback;

			return primary;
		}

		public virtual bool OnHarvesting( Mobile from, Item tool, HarvestDefinition def, object toHarvest, object locked, bool last )
		{
			if ( !CheckHarvest( from, tool ) )
			{
				from.EndAction( locked );
				return false;
			}

			int tileID;
			Map map;
			Point3D loc;

			if ( !GetHarvestDetails( from, tool, toHarvest, out tileID, out map, out loc ) )
			{
				from.EndAction( locked );
				OnBadHarvestTarget( from, tool, toHarvest );
				return false;
			}
			else if ( !def.Validate( tileID ) )
			{
				from.EndAction( locked );
				OnBadHarvestTarget( from, tool, toHarvest );
				return false;
			}
			else if ( !CheckRange( from, tool, def, map, loc, true ) )
			{
				from.EndAction( locked );
				return false;
			}
			else if ( !CheckResources( from, tool, def, map, loc, true ) )
			{
				from.EndAction( locked );
				return false;
			}
			else if ( !CheckHarvest( from, tool, def, toHarvest ) )
			{
				from.EndAction( locked );
				return false;
			}

			DoHarvestingEffect( from, tool, def, map, loc );

			new HarvestSoundTimer( from, tool, this, def, toHarvest, locked, last ).Start();

			return !last;
		}

		public virtual void DoHarvestingSound( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			if ( def.EffectSounds.Length > 0 )
				from.PlaySound( Utility.RandomList( def.EffectSounds ) );
		}

		public virtual void DoHarvestingEffect( Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc )
		{
			from.Direction = from.GetDirectionTo( loc );

			int actions = Utility.RandomList( def.EffectActions );
				if ( tool is Spade || tool is GraveSpade )
				{
					actions = 14;

					if ( from.RaceID > 0 )
						actions = 5;
				}

			if ( !from.Mounted )
				from.Animate( actions, 5, 1, true, false, 0 );
		}

		public virtual HarvestDefinition GetDefinition( int tileID )
		{
			HarvestDefinition def = null;

			for ( int i = 0; def == null && i < m_Definitions.Count; ++i )
			{
				HarvestDefinition check = m_Definitions[i];

				if ( check.Validate( tileID ) )
					def = check;
			}

			return def;
		}

		public virtual void StartHarvesting( Mobile from, Item tool, object toHarvest )
		{
			if ( !CheckHarvest( from, tool ) )
				return;

			int tileID;
			Map map;
			Point3D loc;

			if ( !GetHarvestDetails( from, tool, toHarvest, out tileID, out map, out loc ) )
			{
				OnBadHarvestTarget( from, tool, toHarvest );
				return;
			}

			HarvestDefinition def = GetDefinition( tileID );

			if ( def == null )
			{
				OnBadHarvestTarget( from, tool, toHarvest );
				return;
			}

			if ( !CheckRange( from, tool, def, map, loc, false ) )
				return;
			else if ( !CheckResources( from, tool, def, map, loc, false ) )
				return;
			else if ( !CheckHarvest( from, tool, def, toHarvest ) )
				return;

			object toLock = GetLock( from, tool, def, toHarvest );

			if ( !from.BeginAction( toLock ) )
			{
				OnConcurrentHarvest( from, tool, def, toHarvest );
				return;
			}

			new HarvestTimer( from, tool, this, def, toHarvest, toLock ).Start();
			OnHarvestStarted( from, tool, def, toHarvest );
		}

		public virtual bool GetHarvestDetails( Mobile from, Item tool, object toHarvest, out int tileID, out Map map, out Point3D loc )
		{
			if ( toHarvest is Static && !((Static)toHarvest).Movable )
			{
				Static obj = (Static)toHarvest;
				tileID = (obj.ItemID & 0x3FFF) | 0x4000;
				map = obj.Map;
				loc = obj.GetWorldLocation();
			}
			else if ( toHarvest is StaticTarget )
			{
				StaticTarget obj = (StaticTarget)toHarvest;
				tileID = (obj.ItemID & 0x3FFF) | 0x4000;
				map = from.Map;
				loc = obj.Location;
			}
			else if ( toHarvest is LandTarget )
			{
				LandTarget obj = (LandTarget)toHarvest;
				tileID = obj.TileID;
				map = from.Map;
				loc = obj.Location;
			}
			else
			{
				tileID = 0;
				map = null;
				loc = Point3D.Zero;
				return false;
			}

			return ( map != null && map != Map.Internal );
		}
	}
}

namespace Server
{
	public interface IChopable
	{
		void OnChop( Mobile from );
	}

	[AttributeUsage( AttributeTargets.Class )]
	public class FurnitureAttribute : Attribute
	{
		public static bool Check( Item item )
		{
			return ( item != null && item.GetType().IsDefined( typeof( FurnitureAttribute ), false ) );
		}

		public FurnitureAttribute()
		{
		}
	}
}