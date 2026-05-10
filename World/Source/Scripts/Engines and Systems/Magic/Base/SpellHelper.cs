using System;
using Server;
using Server.Items;
using Server.Guilds;
using Server.Multis;
using Server.Regions;
using Server.Mobiles;
using Server.Targeting;
using Server.Engines.PartySystem;
using Server.Misc;
using Server.Spells.Bushido;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using System.Collections.Generic;
using Server.Spells.Seventh;
using Server.Spells.Fifth;
using Server.Spells.Shinobi;
using Server.Spells.Elementalism;

namespace Server
{
	public class DefensiveSpell
	{
		public static void Nullify( Mobile from )
		{
			if( !from.CanBeginAction( typeof( DefensiveSpell ) ) )
				new InternalTimer( from ).Start();
		}

		private class InternalTimer : Timer
		{
			private Mobile m_Mobile;

			public InternalTimer( Mobile m ): base( TimeSpan.FromMinutes( 1.0 ) )
			{
				m_Mobile = m;

				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				m_Mobile.EndAction( typeof( DefensiveSpell ) );
			}
		}

		public static void EndDefense( Mobile from )
		{
			if ( from.MagicDamageAbsorb < 1 && from.MeleeDamageAbsorb < 1 )
				from.EndAction( typeof( DefensiveSpell ) );
		}
	}
}

namespace Server.Spells
{
	public enum TravelCheckType
	{
		RecallFrom,
		RecallTo,
		GateFrom,
		GateTo,
		Mark,
		TeleportFrom,
		TeleportTo
	}

	public class SpellHelper
	{
		private static TimeSpan AosDamageDelay = TimeSpan.FromSeconds( 1.0 );
		private static TimeSpan OldDamageDelay = TimeSpan.FromSeconds( 0.5 );

		public static TimeSpan GetDamageDelayForSpell( Spell sp )
		{
			if( !sp.DelayedDamage )
				return TimeSpan.Zero;

			return (Core.AOS ? AosDamageDelay : OldDamageDelay);
		}

		public static bool isFriend( Mobile caster, Mobile m )
		{
			if ( caster != null && m != null )
			{
				if ( m == caster )
					return true;

				if ( m is BaseCreature && ((BaseCreature)m).ControlMaster == caster )
					return true;

				Party p = Engines.PartySystem.Party.Get( caster );
				if ( p != null )
				{
					foreach ( PartyMemberInfo pmi in p.Members )
					{
						if ( pmi.Mobile == m )
							return true;
					}
				}

				if ( !caster.CanBeHarmful( m, false ) )
					return true;
			}
			return false;
		}

		public static bool CheckMulti( Point3D p, Map map )
		{
			return CheckMulti( p, map, true, 0);
		}
		
		public static bool CheckMulti(Point3D p, Map map, bool houses)
		{
			return CheckMulti(p, map, houses, 0);
		}
		
		public static bool CheckMulti( Point3D p, Map map, bool houses, int housingrange )
		{
			if( map == null || map == Map.Internal )
				return false;

			Sector sector = map.GetSector( p.X, p.Y );

			for( int i = 0; i < sector.Multis.Count; ++i )
			{
				BaseMulti multi = sector.Multis[i];

				if( multi is BaseHouse )
				{
					BaseHouse bh = (BaseHouse)multi;

					if( ( houses && bh.IsInside( p, 16 ) ) || ( housingrange > 0 && bh.InRange( p, housingrange ) ) )
						return true;
				}
				else if( multi.Contains( p ))
				{
					return true;
				}
			}
			
			return false;
		}

		public static void Turn( Mobile from, object to )
		{
			IPoint3D target = to as IPoint3D;

			if( target == null )
				return;

			if( target is Item )
			{
				Item item = (Item)target;

				if( item.RootParent != from )
					from.Direction = from.GetDirectionTo( item.GetWorldLocation() );
			}
			else if( from != target )
			{
				from.Direction = from.GetDirectionTo( target );
			}
		}

		private static TimeSpan CombatHeatDelay = TimeSpan.FromSeconds( 30.0 );
		private static bool RestrictTravelCombat = true;

		public static bool CheckCombat( Mobile m )
		{
			if( !RestrictTravelCombat )
				return false;

			for( int i = 0; i < m.Aggressed.Count; ++i )
			{
				AggressorInfo info = m.Aggressed[i];

				if( info.Defender.Player && (DateTime.Now - info.LastCombatTime) < CombatHeatDelay )
					return true;
			}

			if( Core.Expansion == Expansion.AOS )
			{
				for( int i = 0; i < m.Aggressors.Count; ++i )
				{
					AggressorInfo info = m.Aggressors[i];

					if( info.Attacker.Player && (DateTime.Now - info.LastCombatTime) < CombatHeatDelay )
						return true;
				}
			}

			return false;
		}

		public static bool AdjustField( ref Point3D p, Map map, int height, bool mobsBlock )
		{
			if( map == null )
				return false;

			for( int offset = 0; offset < 10; ++offset )
			{
				Point3D loc = new Point3D( p.X, p.Y, p.Z - offset );

				if( map.CanFit( loc, height, true, mobsBlock ) )
				{
					p = loc;
					return true;
				}
			}

			return false;
		}
		
		public static bool CanRevealCaster( Mobile m )
		{
			if ( m is BaseCreature )
			{
				BaseCreature c = (BaseCreature)m;
						
				if ( !c.Controlled )
					return true;
			}
			
			return false;
		}

		public static void GetSurfaceTop( ref IPoint3D p )
		{
			if( p is Item )
			{
				p = ((Item)p).GetSurfaceTop();
			}
			else if( p is StaticTarget )
			{
				StaticTarget t = (StaticTarget)p;
				int z = t.Z;

				if( (t.Flags & TileFlag.Surface) == 0 )
					z -= TileData.ItemTable[t.ItemID & TileData.MaxItemValue].CalcHeight;

				p = new Point3D( t.X, t.Y, z );
			}
		}

		public static bool AddStatOffset( Mobile m, StatType type, int offset, TimeSpan duration )
		{
			if( offset > 0 )
				return AddStatBonus( m, m, type, offset, duration );
			else if( offset < 0 )
				return AddStatCurse( m, m, type, -offset, duration );

			return true;
		}

		public static bool AddStatBonus( Mobile caster, Mobile target, StatType type )
		{
			return AddStatBonus( caster, target, type, GetOffset( caster, target, type, false ), GetDuration( caster, target ) );
		}

		public static bool AddStatBonus( Mobile caster, Mobile target, StatType type, int bonus, TimeSpan duration )
		{
			int offset = bonus;
			string name = String.Format( "[Magic] {0} Offset", type );

			StatMod mod = target.GetStatMod( name );

			if( mod != null && mod.Offset < 0 )
			{
				target.AddStatMod( new StatMod( type, name, mod.Offset + offset, duration ) );
				return true;
			}
			else if( mod == null || mod.Offset < offset )
			{
				target.AddStatMod( new StatMod( type, name, offset, duration ) );
				return true;
			}

			return false;
		}

		public static bool AddStatCurse( Mobile caster, Mobile target, StatType type )
		{
			return AddStatCurse( caster, target, type, GetOffset( caster, target, type, true ), GetDuration( caster, target ) );
		}

		public static bool AddStatCurse( Mobile caster, Mobile target, StatType type, int curse, TimeSpan duration )
		{
			int offset = -curse;
			string name = String.Format( "[Magic] {0} Offset", type );

			StatMod mod = target.GetStatMod( name );

			if( mod != null && mod.Offset > 0 )
			{
				target.AddStatMod( new StatMod( type, name, mod.Offset + offset, duration ) );
				return true;
			}
			else if( mod == null || mod.Offset > offset )
			{
				target.AddStatMod( new StatMod( type, name, offset, duration ) );
				return true;
			}

			return false;
		}

		public static TimeSpan GetDuration( Mobile caster, Mobile target )
		{
			return TimeSpan.FromSeconds( ((6 * Spell.ItemSkillValue( caster, SkillName.Psychology, true )) / 50) + 1 );
		}

		private static bool m_DisableSkillCheck;

		public static bool DisableSkillCheck
		{
			get { return m_DisableSkillCheck; }
			set { m_DisableSkillCheck = value; }
		}

		public static double GetOffsetScalar( Mobile caster, Mobile target, bool curse )
		{
			double percent;

			if( curse )
				percent = 8 + (Spell.ItemSkillValue( caster, SkillName.Psychology, true ) / 100) - (target.Skills.MagicResist.Fixed / 100);
			else
				percent = 1 + (Spell.ItemSkillValue( caster, SkillName.Psychology, true ) / 100);

			percent *= 0.01;

			if( percent < 0 )
				percent = 0;

			return percent;
		}

		public static int GetOffset( Mobile caster, Mobile target, StatType type, bool curse )
		{
			if( !m_DisableSkillCheck )
			{
				if ( !caster.ItemCastSpell )
					caster.CheckSkill( SkillName.Psychology, 0.0, 120.0 );

				if ( curse )
					target.CheckSkill( SkillName.MagicResist, 0.0, 120.0 );
			}

			double percent = GetOffsetScalar( caster, target, curse );

			switch( type )
			{
				case StatType.Str:
					return (int)(target.RawStr * percent);
				case StatType.Dex:
					return (int)(target.RawDex * percent);
				case StatType.Int:
					return (int)(target.RawInt * percent);
			}

			return 1 + (int)(Spell.ItemSkillValue( caster, SkillName.Magery, false ) * 0.1);
		}

		public static Guild GetGuildFor( Mobile m )
		{
			Guild g = m.Guild as Guild;

			if( g == null && m is BaseCreature )
			{
				BaseCreature c = (BaseCreature)m;
				m = c.ControlMaster;

				if( m != null )
					g = m.Guild as Guild;

				if( g == null )
				{
					m = c.SummonMaster;

					if( m != null )
						g = m.Guild as Guild;
				}
			}

			return g;
		}

		public static bool ValidIndirectTarget( Mobile from, Mobile to )
		{
			if( from == to )
				return false;

			if( to.AccessLevel > from.AccessLevel )
				return false;

			Guild fromGuild = GetGuildFor( from );
			Guild toGuild = GetGuildFor( to );

			if( fromGuild != null && toGuild != null && (fromGuild == toGuild || fromGuild.IsAlly( toGuild )) )
				return false;

			Party p = Party.Get( from );

			if( p != null && p.Contains( to ) )
				return false;

			if( to is BaseCreature )
			{
				BaseCreature c = (BaseCreature)to;

				if( c.Controlled || c.Summoned )
				{
					if( c.ControlMaster == from || c.SummonMaster == from )
						return false;

					if( p != null && (p.Contains( c.ControlMaster ) || p.Contains( c.SummonMaster )) )
						return false;
				}
			}

			if( from is BaseCreature )
			{
				BaseCreature c = (BaseCreature)from;

				if( c.Controlled || c.Summoned )
				{
					if( c.ControlMaster == to || c.SummonMaster == to )
						return false;

					p = Party.Get( to );

					if( p != null && (p.Contains( c.ControlMaster ) || p.Contains( c.SummonMaster )) )
						return false;
				}
			}

			return true;
		}

		private static int[] m_Offsets = new int[]
			{
				-1, -1,
				-1,  0,
				-1,  1,
				0, -1,
				0,  1,
				1, -1,
				1,  0,
				1,  1
			};

		public static void Summon( BaseCreature creature, Mobile caster, int sound, TimeSpan duration, bool scaleDuration, bool scaleStats )
		{
			Map map = caster.Map;

			if( map == null )
				return;

			double scale = 1.0 + ((Spell.ItemSkillValue( caster, SkillName.Magery, false ) - 100.0) / 200.0);

			if( scaleDuration )
				duration = TimeSpan.FromSeconds( duration.TotalSeconds * scale );

			if( scaleStats )
			{
				creature.RawStr = (int)(creature.RawStr * scale);
				creature.Hits = creature.HitsMax;

				creature.RawDex = (int)(creature.RawDex * scale);
				creature.Stam = creature.StamMax;

				creature.RawInt = (int)(creature.RawInt * scale);
				creature.Mana = creature.ManaMax;
			}

			Point3D p = new Point3D( caster );

			if( SpellHelper.FindValidSpawnLocation( map, ref p, true ) )
			{
				BaseCreature.Summon( creature, caster, p, sound, duration );
				return;
			}

			creature.Delete();
			caster.SendLocalizedMessage( 501942 ); // That location is blocked.
		}

		public static bool FindValidSpawnLocation( Map map, ref Point3D p, bool surroundingsOnly )
		{
			if( map == null )	//sanity
				return false;

			if( !surroundingsOnly )
			{
				if( map.CanSpawnMobile( p ) )	//p's fine.
				{
					p = new Point3D( p );
					return true;
				}

				int z = map.GetAverageZ( p.X, p.Y );

				if( map.CanSpawnMobile( p.X, p.Y, z ) )
				{
					p = new Point3D( p.X, p.Y, z );
					return true;
				}
			}

			int offset = Utility.Random( 8 ) * 2;

			for( int i = 0; i < m_Offsets.Length; i += 2 )
			{
				int x = p.X + m_Offsets[(offset + i) % m_Offsets.Length];
				int y = p.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

				if( map.CanSpawnMobile( x, y, p.Z ) )
				{
					p = new Point3D( x, y, p.Z );
					return true;
				}
				else
				{
					int z = map.GetAverageZ( x, y );

					if( map.CanSpawnMobile( x, y, z ) )
					{
						p = new Point3D( x, y, z );
						return true;
					}
				}
			}

			return false;
		}

		public static bool CheckTravel( Mobile caster, TravelCheckType type )
		{
			if( CheckTravel( caster, caster.Map, caster.Location, type ) )
				return true;

			SendInvalidMessage( caster, type );
			return false;
		}

		public static void SendInvalidMessage( Mobile caster, TravelCheckType type )
		{
			if( type == TravelCheckType.RecallTo || type == TravelCheckType.GateTo )
				caster.SendLocalizedMessage( 1019004 ); // You are not allowed to travel there.
			else if( type == TravelCheckType.TeleportTo )
				caster.SendLocalizedMessage( 501035 ); // You cannot teleport from here to the destination.
			else
				caster.SendLocalizedMessage( 501802 ); // Thy spell doth not appear to work...
		}

		public static bool CheckTravel( Map map, Point3D loc, TravelCheckType type )
		{
			return CheckTravel( null, map, loc, type );
		}

		private static Mobile m_TravelCaster;
		private static TravelCheckType m_TravelType;

		public static bool CheckTravel( Mobile caster, Map map, Point3D loc, TravelCheckType type )
		{
			if( IsInvalid( map, loc ) ) // null, internal, out of bounds
			{
				if( caster != null )
					SendInvalidMessage( caster, type );

				return false;
			}

			if( caster != null && caster.AccessLevel == AccessLevel.Player && caster.Region.IsPartOf( typeof( Regions.Jail ) ) )
			{
				caster.SendLocalizedMessage( 1042632 ); // You'll need a better jailbreak plan then that!
				return false;
			}

			m_TravelCaster = caster;
			m_TravelType = type;

			return true;
		}

		public static bool IsInvalid( Map map, Point3D loc )
		{
			if( map == null || map == Map.Internal )
				return true;

			int x = loc.X, y = loc.Y;

			return (x < 0 || y < 0 || x >= map.Width || y >= map.Height);
		}

		//towns
		public static bool IsTown( IPoint3D loc, Mobile caster )
		{
			if( loc is Item )
				loc = ((Item)loc).GetWorldLocation();

			return IsTown( new Point3D( loc ), caster );
		}

		public static bool IsTown( Point3D loc, Mobile caster )
		{
			return false;
		}

		public static bool CheckTown( IPoint3D loc, Mobile caster )
		{
			if( loc is Item )
				loc = ((Item)loc).GetWorldLocation();

			return CheckTown( new Point3D( loc ), caster );
		}

		public static bool CheckTown( Point3D loc, Mobile caster )
		{
			if( IsTown( loc, caster ) )
			{
				caster.SendLocalizedMessage( 500946 ); // You cannot cast this in town!
				return false;
			}

			return true;
		}

		//magic reflection
		public static void CheckReflect( int circle, Mobile caster, ref Mobile target )
		{
			CheckReflect( circle, ref caster, ref target );
		}

		public static void CheckReflect( int circle, ref Mobile caster, ref Mobile target )
		{
			if( target.MagicDamageAbsorb > 0 )
			{
				++circle;

				target.MagicDamageAbsorb -= circle;

				// This order isn't very intuitive, but you have to nullify reflect before target gets switched

				bool reflect = (target.MagicDamageAbsorb >= 0);

				if( target is BaseCreature )
					((BaseCreature)target).CheckReflect( caster, ref reflect );

				if( target.MagicDamageAbsorb <= 0 )
				{
					target.MagicDamageAbsorb = 0;
					DefensiveSpell.Nullify( target );
				}

				if( reflect )
				{
					target.FixedEffect( 0x37B9, 10, 5 );

					Mobile temp = caster;
					caster = target;
					target = temp;
				}

				if ( target.MagicDamageAbsorb < 1 )
				{
					BuffInfo.RemoveBuff( target, BuffIcon.Absorption );
					BuffInfo.RemoveBuff( target, BuffIcon.PsychicWall );
					BuffInfo.RemoveBuff( target, BuffIcon.Deflection );
					BuffInfo.RemoveBuff( target, BuffIcon.TrialByFire );
					BuffInfo.RemoveBuff( target, BuffIcon.OrbOfOrcus );
					BuffInfo.RemoveBuff( target, BuffIcon.MagicReflection );
					BuffInfo.RemoveBuff( target, BuffIcon.ElementalEcho );
				}
			}
			else if( target is BaseCreature )
			{
				bool reflect = false;

				((BaseCreature)target).CheckReflect( caster, ref reflect );

				if( reflect )
				{
					target.FixedEffect( 0x37B9, 10, 5 );

					Mobile temp = caster;
					caster = target;
					target = temp;
				}
			}
		}

		public static void Damage( Spell spell, Mobile target, double damage )
		{
			TimeSpan ts = GetDamageDelayForSpell( spell );

			Damage( spell, ts, target, spell.Caster, damage );
		}

		public static void Damage( TimeSpan delay, Mobile target, double damage )
		{
			Damage( delay, target, null, damage );
		}

		public static void Damage( TimeSpan delay, Mobile target, Mobile from, double damage )
		{
			Damage( null, delay, target, from, damage );
		}

		public static void Damage( Spell spell, TimeSpan delay, Mobile target, Mobile from, double damage )
		{
			int iDamage = (int)damage;

			if( delay == TimeSpan.Zero )
			{
				if( from is BaseCreature )
					((BaseCreature)from).AlterSpellDamageTo( target, ref iDamage );

				if( target is BaseCreature )
					((BaseCreature)target).AlterSpellDamageFrom( from, ref iDamage );

				target.Damage( iDamage, from );
			}
			else
			{
				new SpellDamageTimer( spell, target, from, iDamage, delay ).Start();
			}

			if( target is BaseCreature && from != null && delay == TimeSpan.Zero )
			{
				BaseCreature c = (BaseCreature) target;

				c.OnHarmfulSpell( from );
				c.OnDamagedBySpell( from );
			}
		}

		public static void Damage( Spell spell, Mobile target, double damage, int phys, int fire, int cold, int pois, int nrgy )
		{
			TimeSpan ts = GetDamageDelayForSpell( spell );

			Damage( spell, ts, target, spell.Caster, damage, phys, fire, cold, pois, nrgy, DFAlgorithm.Standard );
		}

		public static void Damage( Spell spell, Mobile target, double damage, int phys, int fire, int cold, int pois, int nrgy, DFAlgorithm dfa )
		{
			TimeSpan ts = GetDamageDelayForSpell( spell );

			Damage( spell, ts, target, spell.Caster, damage, phys, fire, cold, pois, nrgy, dfa );
		}

		public static void Damage( TimeSpan delay, Mobile target, double damage, int phys, int fire, int cold, int pois, int nrgy )
		{
			Damage( delay, target, null, damage, phys, fire, cold, pois, nrgy );
		}

		public static void Damage( TimeSpan delay, Mobile target, Mobile from, double damage, int phys, int fire, int cold, int pois, int nrgy )
		{
			Damage( delay, target, from, damage, phys, fire, cold, pois, nrgy, DFAlgorithm.Standard );
		}

		public static void Damage( TimeSpan delay, Mobile target, Mobile from, double damage, int phys, int fire, int cold, int pois, int nrgy, DFAlgorithm dfa )
		{
			Damage( null, delay, target, from, damage, phys, fire, cold, pois, nrgy, dfa );
		}

		public static void Damage( Spell spell, TimeSpan delay, Mobile target, Mobile from, double damage, int phys, int fire, int cold, int pois, int nrgy, DFAlgorithm dfa )
		{
			int iDamage = (int)damage;

			if( delay == TimeSpan.Zero )
			{
				if( from is BaseCreature )
					((BaseCreature)from).AlterSpellDamageTo( target, ref iDamage );

				if( target is BaseCreature )
					((BaseCreature)target).AlterSpellDamageFrom( from, ref iDamage );

				WeightOverloading.DFA = dfa;

				int damageGiven = AOS.Damage( target, from, iDamage, phys, fire, cold, pois, nrgy );

				WeightOverloading.DFA = DFAlgorithm.Standard;
			}
			else
			{
				new SpellDamageTimerAOS( spell, target, from, iDamage, phys, fire, cold, pois, nrgy, delay, dfa ).Start();
			}

			if( target is BaseCreature && from != null && delay == TimeSpan.Zero )
			{
				BaseCreature c = (BaseCreature) target;

				c.OnHarmfulSpell( from );
				c.OnDamagedBySpell( from );
			}
		}

		public static void DoLeech( int damageGiven, Mobile from, Mobile target )
		{
			TransformContext context = TransformationSpellHelper.GetContext( from );

			if ( context != null ) /* cleanup */
			{
				if (context.Type == typeof(WraithFormSpell))
				{
					WraithFormSpell.DoWraithLeech(from, target, damageGiven);
				}
				else if (context.Type == typeof(VampiricEmbraceSpell))
				{
					from.Hits += AOS.Scale( damageGiven, 20 );
					from.PlaySound( 0x44D );
				}
			}
		}

		public static void Heal( int amount, Mobile target, Mobile from )
		{
			Heal( amount, target, from, true );
		}
		public static void Heal( int amount, Mobile target, Mobile from, bool message )
		{
			//TODO: All Healing *spells* go through ArcaneEmpowerment
			target.Heal( amount, from, message );
		}

		private class SpellDamageTimer : Timer
		{
			private Mobile m_Target, m_From;
			private int m_Damage;
			private Spell m_Spell;

			public SpellDamageTimer( Spell s, Mobile target, Mobile from, int damage, TimeSpan delay )
				: base( delay )
			{
				m_Target = target;
				m_From = from;
				m_Damage = damage;
				m_Spell = s;

				if( m_Spell != null && m_Spell.DelayedDamage && !m_Spell.DelayedDamageStacking )
					m_Spell.StartDelayedDamageContext( target, this );

				Priority = TimerPriority.TwentyFiveMS;
			}

			protected override void OnTick()
			{
				if( m_From is BaseCreature )
					((BaseCreature)m_From).AlterSpellDamageTo( m_Target, ref m_Damage );

				if( m_Target is BaseCreature )
					((BaseCreature)m_Target).AlterSpellDamageFrom( m_From, ref m_Damage );

				m_Target.Damage( m_Damage );
				if( m_Spell != null )
					m_Spell.RemoveDelayedDamageContext( m_Target );
			}
		}

		private class SpellDamageTimerAOS : Timer
		{
			private Mobile m_Target, m_From;
			private int m_Damage;
			private int m_Phys, m_Fire, m_Cold, m_Pois, m_Nrgy;
			private DFAlgorithm m_DFA;
			private Spell m_Spell;

			public SpellDamageTimerAOS( Spell s, Mobile target, Mobile from, int damage, int phys, int fire, int cold, int pois, int nrgy, TimeSpan delay, DFAlgorithm dfa )
				: base( delay )
			{
				m_Target = target;
				m_From = from;
				m_Damage = damage;
				m_Phys = phys;
				m_Fire = fire;
				m_Cold = cold;
				m_Pois = pois;
				m_Nrgy = nrgy;
				m_DFA = dfa;
				m_Spell = s;
				if( m_Spell != null && m_Spell.DelayedDamage && !m_Spell.DelayedDamageStacking )
					m_Spell.StartDelayedDamageContext( target, this );

				Priority = TimerPriority.TwentyFiveMS;
			}

			protected override void OnTick()
			{
				if( m_From is BaseCreature && m_Target != null )
					((BaseCreature)m_From).AlterSpellDamageTo( m_Target, ref m_Damage );

				if( m_Target is BaseCreature && m_From != null )
					((BaseCreature)m_Target).AlterSpellDamageFrom( m_From, ref m_Damage );

				WeightOverloading.DFA = m_DFA;

				int damageGiven = AOS.Damage( m_Target, m_From, m_Damage, m_Phys, m_Fire, m_Cold, m_Pois, m_Nrgy );

				WeightOverloading.DFA = DFAlgorithm.Standard;

				if( m_Target is BaseCreature && m_From != null )
				{
					BaseCreature c = (BaseCreature) m_Target;

					c.OnHarmfulSpell( m_From );
					c.OnDamagedBySpell( m_From );
				}

				if( m_Spell != null )
					m_Spell.RemoveDelayedDamageContext( m_Target );

			}
		}
	}

	public class TransformationSpellHelper
	{
		#region Context Stuff
		private static Dictionary<Mobile, TransformContext> m_Table = new Dictionary<Mobile, TransformContext>();

		public static void AddContext( Mobile m, TransformContext context )
		{
			m_Table[m] = context;
		}

		public static void RemoveContext( Mobile m, bool resetGraphics )
		{
			TransformContext context = GetContext( m );

			if( context != null )
				RemoveContext( m, context, resetGraphics );
		}

		public static void RemoveContext( Mobile m, TransformContext context, bool resetGraphics )
		{
			if( m_Table.ContainsKey( m ) )
			{
				m_Table.Remove( m );

				List<ResistanceMod> mods = context.Mods;

				for( int i = 0; i < mods.Count; ++i )
					m.RemoveResistanceMod( mods[i] );

				if( resetGraphics )
				{
					m.HueMod = -1;
					m.BodyMod = 0;
					m.HairHue = m.RecordHairColor;
					m.FacialHairHue = m.RecordBeardColor;
					m.RaceBody();
				}

				context.Timer.Stop();
				context.Spell.RemoveEffect( m );

				BuffInfo.RemoveBuff( m, BuffIcon.WraithForm );
				BuffInfo.RemoveBuff( m, BuffIcon.HorrificBeast );
				BuffInfo.RemoveBuff( m, BuffIcon.LichForm );
				BuffInfo.RemoveBuff( m, BuffIcon.VampiricEmbrace );
			}
		}

		public static TransformContext GetContext( Mobile m )
		{
			TransformContext context = null;

			m_Table.TryGetValue( m, out context );

			return context;
		}

		public static bool UnderTransformation( Mobile m )
		{
			return (GetContext( m ) != null);
		}

		public static bool UnderTransformation( Mobile m, Type type )
		{
			TransformContext context = GetContext( m );

			return (context != null && context.Type == type);
		}
		#endregion

		public static bool CheckCast( Mobile caster, Spell spell )
		{
			if( !caster.CanBeginAction( typeof( PolymorphSpell ) ) )
			{
				caster.SendLocalizedMessage( 1061628 ); // You can't do that while polymorphed.
				return false;
			}
			else if( AnimalForm.UnderTransformation( caster ) )
			{
				caster.SendLocalizedMessage( 1061091 ); // You cannot cast that spell in this form.
				return false;
			}

			return true;
		}

		public static bool OnCast( Mobile caster, Spell spell )
		{
			ITransformationSpell transformSpell = spell as ITransformationSpell;

			if( transformSpell == null )
				return false;

			if( !caster.CanBeginAction( typeof( PolymorphSpell ) ) )
			{
				caster.SendLocalizedMessage( 1061628 ); // You can't do that while polymorphed.
			}
			else if ( DisguiseTimers.IsDisguised( caster ) )
			{
				caster.SendLocalizedMessage( 1061631 ); // You can't do that while disguised.
				return false;
			}
			else if( AnimalForm.UnderTransformation( caster ) )
			{
				caster.SendLocalizedMessage( 1061091 ); // You cannot cast that spell in this form.
			}
			else if( !caster.CanBeginAction( typeof( Deception ) ) || !caster.CanBeginAction( typeof( IncognitoSpell ) ) || (caster.IsBodyMod && GetContext( caster ) == null && caster.RaceID != caster.BodyMod) )
			{
				spell.DoFizzle();
			}
			else if( spell.CheckSequence() )
			{
				TransformContext context = GetContext( caster );
				Type ourType = spell.GetType();

				bool wasTransformed = (context != null);
				bool ourTransform = (wasTransformed && context.Type == ourType);

				if( wasTransformed )
				{
					RemoveContext( caster, context, ourTransform );

					if( ourTransform )
					{
						caster.PlaySound( 0xFA );
						caster.FixedParticles( 0x3728, 1, 13, 5042, EffectLayer.Waist );
					}
				}

				if( !ourTransform )
				{
					List<ResistanceMod> mods = new List<ResistanceMod>();

					if( transformSpell.PhysResistOffset != 0 )
						mods.Add( new ResistanceMod( ResistanceType.Physical, transformSpell.PhysResistOffset ) );

					if( transformSpell.FireResistOffset != 0 )
						mods.Add( new ResistanceMod( ResistanceType.Fire, transformSpell.FireResistOffset ) );

					if( transformSpell.ColdResistOffset != 0 )
						mods.Add( new ResistanceMod( ResistanceType.Cold, transformSpell.ColdResistOffset ) );

					if( transformSpell.PoisResistOffset != 0 )
						mods.Add( new ResistanceMod( ResistanceType.Poison, transformSpell.PoisResistOffset ) );

					if( transformSpell.NrgyResistOffset != 0 )
						mods.Add( new ResistanceMod( ResistanceType.Energy, transformSpell.NrgyResistOffset ) );

					if( !((Body)transformSpell.Body).IsHuman )
					{
						Mobiles.IMount mt = caster.Mount;

						if( mt != null )
						{
							Server.Mobiles.EtherealMount.EthyDismount( caster );
							mt.Rider = null;
						}
					}

					caster.BodyMod = transformSpell.Body;
					caster.HueMod = transformSpell.Hue;
					if ( transformSpell.Hue == 0xB70 )
					{
						caster.FacialHairHue = 0x497;
						caster.HairHue = 0x497;
					}

					for( int i = 0; i < mods.Count; ++i )
						caster.AddResistanceMod( mods[i] );

					transformSpell.DoEffect( caster );

					Timer timer = new TransformTimer( caster, transformSpell );
					timer.Start();

					AddContext( caster, new TransformContext( timer, mods, ourType, transformSpell ) );
					return true;
				}
			}

			return false;
		}
	}

	public interface ITransformationSpell
	{
		int Body { get; }
		int Hue { get; }

		int PhysResistOffset { get; }
		int FireResistOffset { get; }
		int ColdResistOffset { get; }
		int PoisResistOffset { get; }
		int NrgyResistOffset { get; }

		double TickRate { get; }
		void OnTick( Mobile m );

		void DoEffect( Mobile m );
		void RemoveEffect( Mobile m );
	}

	public class TransformContext
	{
		private Timer m_Timer;
		private List<ResistanceMod> m_Mods;
		private Type m_Type;
		private ITransformationSpell m_Spell;

		public Timer Timer { get { return m_Timer; } }
		public List<ResistanceMod> Mods { get { return m_Mods; } }
		public Type Type { get { return m_Type; } }
		public ITransformationSpell Spell { get { return m_Spell; } }

		public TransformContext( Timer timer, List<ResistanceMod> mods, Type type, ITransformationSpell spell )
		{
			m_Timer = timer;
			m_Mods = mods;
			m_Type = type;
			m_Spell = spell;
		}
	}

	public class TransformTimer : Timer
	{
		private Mobile m_Mobile;
		private ITransformationSpell m_Spell;

		public TransformTimer( Mobile from, ITransformationSpell spell )
			: base( TimeSpan.FromSeconds( spell.TickRate ), TimeSpan.FromSeconds( spell.TickRate ) )
		{
			m_Mobile = from;
			m_Spell = spell;

			Priority = TimerPriority.TwoFiftyMS;
		}

		protected override void OnTick()
		{
			if( m_Mobile.Deleted || !m_Mobile.Alive || m_Mobile.Body != m_Spell.Body || m_Mobile.Hue != m_Spell.Hue )
			{
				TransformationSpellHelper.RemoveContext( m_Mobile, true );
				Stop();
			}
			else
			{
				m_Spell.OnTick( m_Mobile );
			}
		}
	}
}
