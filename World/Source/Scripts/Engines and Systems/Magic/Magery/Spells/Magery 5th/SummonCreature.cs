using System;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Utilities.Constants;

namespace Server.Spells.Fifth
{
	public class SummonCreatureSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Summon Creature", "Kal Xen",
				16,
				false,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public SummonCreatureSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		// NOTE: Creature list based on 1hr of summon/release on OSI.

		private static Type[] m_Types = new Type[]
			{
				typeof( PolarBear ),
				typeof( GrizzlyBearRiding ),
				typeof( BlackBear ),
				typeof( Horse ),
				typeof( Walrus ),
				typeof( Chicken ),
				typeof( Scorpion ),
				typeof( GiantSerpent ),
				typeof( Llama ),
				typeof( Alligator ),
				typeof( GreyWolf ),
				typeof( Slime ),
				typeof( Eagle ),
				typeof( Gorilla ),
				typeof( SnowLeopard ),
				typeof( Cheetah ),
				typeof( Pig ),
				typeof( Hind ),
				typeof( Rabbit )
			};

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if ( (Caster.Followers + 2) > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1049645 ); // You have too many followers to summon that creature.
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				BaseCreature bc = Caster as BaseCreature;

				if ( bc != null && bc.EmoteHue > EmoteHues.None )
					SpawnThemedMinion( bc );
				else
					SpawnPlayerCreature();
			}

			FinishSequence();
		}

		private void SpawnPlayerCreature()
		{
			try
			{
				BaseCreature creature = (BaseCreature)Activator.CreateInstance( m_Types[Utility.Random( m_Types.Length )] );

				int nBenefit = 0;
				if ( Caster is PlayerMobile )
					nBenefit = (int)(Caster.Skills[SkillName.Magery].Value);

				TimeSpan duration = TimeSpan.FromSeconds( (( 2 * Spell.ItemSkillValue( Caster, SkillName.Magery, true ) ) / 5) + nBenefit );

				SpellHelper.Summon( creature, Caster, 0x215, duration, false, false );
			}
			catch
			{
			}
		}

		private void SpawnThemedMinion( BaseCreature from )
		{
			Map map = from.Map;
			if ( map == null )
				return;

			BaseCreature monster = PickThemedCreature( from );

			if ( monster == null )
				return;

			((BaseCreature)monster).Team = from.Team;

			Point3D loc = from.Location;

			if ( !SpellHelper.FindValidSpawnLocation( map, ref loc, true ) )
			{
				monster.Delete();
				return;
			}

			monster.IsTempEnemy = true;
			monster.YellHue = from.Serial;
			monster.MoveToWorld( loc, map );
			monster.Combatant = from.Combatant;
			Effects.SendLocationParticles( EffectItem.Create( monster.Location, monster.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
			monster.PlaySound( 0x1FE );
			IntelligentAction.OnCreatureSpawned( monster );
		}

		private static BaseCreature PickThemedCreature( BaseCreature from )
		{
			int emoteHue = from.EmoteHue;
			switch ( from.EmoteHue )
			{
				case EmoteHues.TraditionalMage:
				{
					switch ( Utility.Random( 5 ) )
					{
						default:
						case 0: return new EvilBladeSpirits();
						case 1: case 2: return new Imp();
						case 3: case 4: return new Slime();
					}
				}

				case EmoteHues.Blood:
				{
					switch ( Utility.Random( 5 ) )
					{
						default:
						case 0: return new BloodWorm();
						case 1: return new BloodSnake();
						case 2: return new Viscera();
						case 3: return new BloodSpawn();
						case 4: return new GiantLeech();
					}
				}

				case EmoteHues.Demonologist:
				{
					switch ( Utility.Random( 11 ) )
					{
						default:
						case 0: case 2: return new LesserDemon();
						case 1: case 3: return new LowerDemon();
						case 4: return new Imp();
						case 5: return new ShadowHound();
						case 7: return new Gargoyle();
						case 9: return new SoulWorm();
					}
				}

				case EmoteHues.Elementalist:
				{
					switch ( Utility.Random( 31 ) )
					{
						default:
						case 0: case 5: case 10: return new GarnetElemental();
						case 1: case 6: case 11: return new TopazElemental();
						case 2: case 7: case 12: return new QuartzElemental();
						case 3: case 8: case 13: return new SpinelElemental();
						case 4: case 9: case 14: return new StarRubyElemental();
						case 15: return new EarthElemental();
						case 16: return new AgapiteElemental();
						case 17: return new BronzeElemental();
						case 18: return new CopperElemental();
						case 19: return new DullCopperElemental();
						case 20: return new GoldenElemental();
						case 21: return new ShadowIronElemental();
						case 22: return new ValoriteElemental();
						case 23: return new VeriteElemental();
						case 24: return new PoisonElemental();
						case 25: return new ToxicElemental();
						case 26: return new WaterElemental();
						case 27: return new AirElemental();
						case 28: return new BloodElemental();
						case 29: return new FireElemental();
						case 30: return new ElectricalElemental();
					}
				}

				case EmoteHues.Necromancer:
				{
					switch ( Utility.Random( 14 ) )
					{
						default:
						case 0:  return new BoneKnight();
						case 1:  return new BoneMagi();
						case 2:  return new Ghoul();
						case 3:  return new Ghostly();
						case 4:  return new Mummy();
						case 5:  return new Shade();
						case 6:  return new SkeletalKnight();
						case 7:  return new SkeletalMage();
						case 8:  return new Skeleton();
						case 9:  return new Spectre();
						case 10: return new Wraith();
						case 11: return new Phantom();
						case 12: return new Zombie();
						case 13: return new Bodak();
					}
				}

				case EmoteHues.Druid:
				{
					switch ( Utility.Random( 4 ) )
					{
						default:
						case 0: return new DireBear();
						case 1: return new DireBoar();
						case 2: return new WolfDire();
						case 3: return new WeedElemental();
					}
				}

				case EmoteHues.IceWizard:
				{
					switch ( Utility.Random( 9 ) )
					{
						default:
						case 0: return new SnowElemental();
						case 1: case 7: return new IceSerpent();
						case 2: return new IceElemental();
						case 3: return new FrostOoze();
						case 4: return new FrostSpider();
						case 5: return new IceGolem();
						case 6: return new IceToad();
						case 8: return new WinterWolf();
					}
				}

				case EmoteHues.FireWizard:
				{
					switch ( Utility.Random( 9 ) )
					{
						default:
						case 0: return new FireDemon();
						case 1: case 6: return new LavaPuddle();
						case 2: return new CinderElemental();
						case 3: return new FireElemental();
						case 4: case 5: return new FireBat();
						case 7: case 8: return new FireMephit();
					}
				}

				case EmoteHues.SerpentMage:
				{
					switch ( Utility.Random( 11 ) )
					{
						default:
						case 0: case 5: return new GiantSerpent();
						case 1: case 6: return new GiantAdder();
						case 2: case 7: return new JungleViper();
						case 3: case 8: return new LargeSnake();
						case 4: case 9: return new Snake();
						case 10: return new SilverSerpent();
					}
				}

				case EmoteHues.WaterWizard:
				{
					switch ( Utility.Random( 5 ) )
					{
						default:
						case 0: return new WaterWeird();
						case 1: return new Typhoon();
						case 2: return new WaterElemental();
						case 3: return new StormCloud();
						case 4: return new WaterSpawn();
					}
				}

				case EmoteHues.EvilMageLord:
				case EmoteHues.SummonQuest:
				{
					switch ( Utility.Random( 5 ) )
					{
						default:
						case 0: return new EvilIcyVortex();
						case 1: return new EvilPlagueVortex();
						case 2: return new EvilEnergyVortex();
						case 3: return new EvilScorchingVortex();
						case 4: return new EvilBladeSpirits();
					}
				}

				case EmoteHues.InsaneWizard:
				{
					switch ( Utility.Random( 3 ) )
					{
						default:
						case 0: return new WineElemental();
						case 1: return new ManureGolem();
						case 2: return new Fairy();
					}
				}

				case EmoteHues.Undead:
				{
					int maxMonster = 2;
					if ( from.Fame >= 23000 )      maxMonster = 10;
					else if ( from.Fame >= 12000 ) maxMonster = 6;

					switch ( Utility.RandomMinMax( 0, maxMonster ) )
					{
						default:
						case 0:  return new FrailSkeleton();
						case 1:  return new Phantom();
						case 2:  return new Skeleton();
						case 3:  return new Zombie();
						case 4:  return new GhostWarrior();
						case 5:  return new Wight();
						case 6:  return new SkeletalWarrior();
						case 7:  return new WalkingCorpse();
						case 8:  return new SkeletalKnight();
						case 9:  return new BoneKnight();
						case 10: return new Spirit();
					}
				}

				case EmoteHues.UndeadDruid:
				{
					switch ( Utility.Random( 5 ) )
					{
						default:
						case 0: case 1: return new DeathWolf();
						case 2: case 3: return new DeathBear();
						case 4:         return new DarkReaper();
					}
				}

				case EmoteHues.Vampire:
				{
					int maxMonster = 1;
					if ( from.Fame >= 24000 )      maxMonster = 5;
					else if ( from.Fame >= 10500 ) maxMonster = 3;

					switch ( Utility.RandomMinMax( 0, maxMonster ) )
					{
						default:
						case 0: return new Bat();
						case 1: return new Zombie();
						case 2: return new Wraith();
						case 3: return new Ghoul();
						case 4: return new VampireBat();
						case 5: return new WalkingCorpse();
					}
				}

				case EmoteHues.IceQueen:
				{
					switch ( Utility.Random( 2 ) )
					{
						default:
						case 0: return new EvilIcyVortex();
						case 1: return new IceBladeSpirits();
					}
				}

				case EmoteHues.Sand:
				{
					switch ( Utility.Random( 7 ) )
					{
						default:
						case 0: return new Scorpion();
						case 1: case 2: return new SandVortex();
						case 3: case 4: return new DustElemental();
						case 5: return new SandSpider();
						case 6: return new GiantAdder();
					}
				}
			}

			return null;
		}

		public static int GetSummonStrength( Mobile from, BaseCreature m )
		{
			if ( from == null || from.Deleted ) return 0;
			if ( m == null || m.Deleted ) return 0;
			if ( from.EmoteHue <= EmoteHues.None ) return 0;

			if ( from.EmoteHue == EmoteHues.TraditionalMage )
			{
				if ( m is EvilBladeSpirits || m is Imp || m is Slime )
					return 1;
			}
			else if ( from.EmoteHue == EmoteHues.Blood )
			{
				if ( m is BloodWorm || m is BloodSnake || m is Viscera || m is BloodSpawn || m is GiantLeech )
					return 1;
			}
			else if ( from.EmoteHue == EmoteHues.Demonologist )
			{
				if ( m is LesserDemon || m is Imp || m is ShadowHound || m is Gargoyle || m is SoulWorm )
					return 1;

				if ( m is LowerDemon )
				{
					return 2;
				}
			}
			else if ( from.EmoteHue == EmoteHues.Elementalist )
			{
				if ( m is GarnetElemental || m is TopazElemental || m is QuartzElemental || m is SpinelElemental || m is StarRubyElemental || m is EarthElemental || m is AgapiteElemental || m is BronzeElemental || m is CopperElemental || m is DullCopperElemental || m is GoldenElemental || m is ShadowIronElemental || m is ValoriteElemental || m is VeriteElemental || m is WaterElemental )
					return 1;

				if ( m is PoisonElemental || m is ToxicElemental || m is AirElemental || m is BloodElemental || m is FireElemental || m is ElectricalElemental )
				{
					return 2;
				}
			}
			else if ( from.EmoteHue == EmoteHues.Necromancer )
			{
				if ( m is Bodak || m is BoneKnight || m is BoneMagi || m is Ghoul || m is Mummy || m is Shade || m is SkeletalKnight || m is SkeletalMage || m is Skeleton || m is Spectre || m is Wraith || m is Phantom || m is Zombie )
					return 1;
			}
			else if ( from.EmoteHue == EmoteHues.Druid )
			{
				if ( m is WeedElemental || m is WolfDire || m is DireBear || m is DireBoar )
					return 1;
			}
			else if ( from.EmoteHue == EmoteHues.IceWizard )
			{
				if ( m is SnowElemental || m is IceSerpent || m is WinterWolf || m is IceElemental || m is FrostOoze || m is FrostSpider || m is IceGolem || m is IceToad || m is IceSerpent )
					return 1;
			}
			else if ( from.EmoteHue == EmoteHues.FireWizard )
			{
				if ( m is FireDemon || m is LavaPuddle || m is CinderElemental || m is FireBat || m is FireElemental || m is FireMephit )
					return 1;
			}
			else if ( from.EmoteHue == EmoteHues.SerpentMage )
			{
				if ( m is GiantSerpent || m is GiantAdder || m is JungleViper || m is LargeSnake || m is Snake )
					return 1;

				if ( m is SilverSerpent )
				{
					return 2;
				}
			}
			else if ( from.EmoteHue == EmoteHues.WaterWizard )
			{
				if ( m is WaterWeird || m is Typhoon || m is WaterElemental || m is StormCloud || m is WaterSpawn )
					return 1;
			}
			else if ( from.EmoteHue == EmoteHues.EvilMageLord || from.EmoteHue == EmoteHues.SummonQuest )
			{
				if ( m is EvilIcyVortex || m is EvilPlagueVortex || m is EvilEnergyVortex || m is EvilBladeSpirits || m is EvilScorchingVortex )
					return 1;
			}
			else if ( from.EmoteHue == EmoteHues.InsaneWizard )
			{
				if ( m is WineElemental || m is ManureGolem || m is Fairy )
					return 1;
			}
			else if ( from.EmoteHue == EmoteHues.Undead )
			{
				if ( m is GhostWarrior || m is WalkingCorpse || m is Wight || m is Spirit || m is Phantom || m is FrailSkeleton || m is Zombie || m is Skeleton || m is SkeletalKnight || m is BoneKnight || m is SkeletalWarrior )
					return 1;
			}
			else if ( from.EmoteHue == EmoteHues.UndeadDruid )
			{
				if ( m is DeathBear || m is DeathWolf || m is DarkReaper )
					return 1;
			}
			else if ( from.EmoteHue == EmoteHues.Vampire )
			{
				if ( m is Bat || m is Ghoul || m is Wraith || m is WalkingCorpse || m is VampireBat || m is Zombie )
					return 1;
			}
			else if ( from.EmoteHue == EmoteHues.IceQueen )
			{
				if ( m is EvilIcyVortex || m is IceBladeSpirits )
					return 1;
			}
			else if ( from.EmoteHue == EmoteHues.Sand )
			{
				if ( m is Scorpion || m is SandVortex || m is SandSpider || m is DustElemental || m is GiantAdder )
					return 1;
			}

			return 0;
		}

		public override TimeSpan GetCastDelay()
		{
			return TimeSpan.FromTicks( base.GetCastDelay().Ticks * 2	 );
		}
	}
}
