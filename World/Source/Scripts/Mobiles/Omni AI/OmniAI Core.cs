// Created by Peoharen
using System;
using System.Collections.Generic;
using Server.Engines.PartySystem;
using Server.Guilds;
using Server.Items;
using Server.Spells;
using Server.Spells.Third;
using Server.Spells.Sixth;
using Server.Spells.Ninjitsu;
using Server.Targeting;
using Server.Multis;
using System.Linq;

namespace Server.Mobiles
{
	public partial class OmniAI : BaseAI
	{
		public bool IsEmpowered = false;

		private static TimeSpan TELEPORT_DELAY = TimeSpan.FromSeconds( 4 );
		public static int MAX_TELEPORT_ATTEMPTS = 3;

		private int m_TeleportAttempts;
		private DateTime m_NextTeleportTime;
		private DateTime m_NextCastTime;
		private DateTime m_NextHealTime;
		private DateTime m_NextFieldCheck;
		private DateTime m_NextCheckArmed;

		#region canuse
		public virtual bool m_IsSmart
		{
			get { return false; }
		}

		public virtual bool m_CanUseBard
		{
			// Disabled due to custom bard implementation
			get { return false && (m_Mobile.Skills[SkillName.Musicianship].Base > 10.0); }
		}

		public virtual bool m_CanUseBushido
		{
			get { return (m_Mobile.Skills[SkillName.Bushido].Base > 10.0); }
		}

		public virtual bool m_CanUseChivalry
		{
			get { return (m_Mobile.Skills[SkillName.Knightship].Base > 10.0); }
		}

		public virtual bool m_CanUseMagery
		{
			get { return (m_Mobile.Skills[SkillName.Magery].Base > 10.0); }
		}

		public virtual bool m_CanUseNecromancy
		{
			get { return (m_Mobile.Skills[SkillName.Necromancy].Base > 10.0); }
		}

		public virtual bool m_CanUseNinjitsu
		{
			get { return (m_Mobile.Skills[SkillName.Ninjitsu].Base > 10.0); }
		}
		
		public virtual bool m_SwapWeapons
		{
			get { return m_CanUseBushido || m_CanUseNinjitsu; }
		}

		public virtual bool m_MoveErratically
		{
			get
			{
				return false;
			}
		}

		public virtual bool m_RequireRanged
		{
			get
			{
				return m_Mobile.Weapon is BaseRanged;
			}
		}

		public virtual bool m_PrefersRanged
		{
			get
			{
				if (m_CanUseMagery || m_CanUseNecromancy)
				{
					var delta = m_Mobile.RawInt - m_Mobile.RawStr;
					if (100 <= delta) return true; // Int is much higher
					if (0 < delta && (m_Mobile.Weapon == null || m_Mobile.Weapon is Fists)) return true; // Int higher and no equipped weapon
				}

				return false;
			}
		}
		#endregion

		public OmniAI( BaseCreature m ) : base( m )
		{
		}

		#region getoutofmyway
		public override bool Think()
		{
			if ( m_Mobile.Deleted )
				return false;

			if ( DateTime.Now > m_NextFieldCheck )
			{
				CheckForFieldSpells();
				m_NextFieldCheck = DateTime.Now + TimeSpan.FromSeconds( 3 );
			}

			if ( DateTime.Now > m_NextCheckArmed )
			{
				CheckArmed( m_SwapWeapons );
				m_NextCheckArmed = DateTime.Now + TimeSpan.FromSeconds( 12 );
			}

			if ( ProcessTarget() )
				return true;
			else
				return base.Think();
		}

		public override bool DoActionWander()
		{
			if ( m_Mobile.Debug )
				m_Mobile.DebugSay( "I have no combatant" );

			if ( !m_Mobile.Hidden && !m_Mobile.Poisoned && m_CanUseNinjitsu )
				m_Mobile.UseSkill( SkillName.Hiding );

			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else if ( m_Mobile.Mana < m_Mobile.ManaMax && m_Mobile.Skills[SkillName.Meditation].Value > 60.0 )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I am going to meditate" );

				m_Mobile.UseSkill( SkillName.Meditation );
			}
			else
			{
				base.DoActionWander();
				TryToHeal();
			}

			return true;
		}
		#endregion



		public override bool DoActionCombat()
		{
			Mobile combatant = m_Mobile.Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != m_Mobile.Map || !combatant.Alive || combatant.IsDeadBondedPet )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "My combatant is gone, so my guard is up" );

				Action = ActionType.Guard;
				return true;
			}

			if ( !m_Mobile.InRange( combatant, m_Mobile.RangePerception ) )
			{
				// They are somewhat far away, can we find something else?
				if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
				{
					m_Mobile.Combatant = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else if ( !m_Mobile.InRange( combatant, m_Mobile.RangePerception * 3 ) )
				{
					m_Mobile.Combatant = null;
				}

				combatant = m_Mobile.Combatant;

				if ( combatant == null )
				{
					if ( m_Mobile.Debug )
						m_Mobile.DebugSay( "My combatant has fled, so I am on guard" );

					Action = ActionType.Guard;
					return true;
				}
			}

			if ( MoveTo( combatant, true, m_Mobile.RangeFight ) )
			{
				m_Mobile.Direction = m_Mobile.GetDirectionTo( combatant );
			}
			else if ( OnFailedMove() )
			{
				return true;
			}
			else if ( m_Mobile.GetDistanceToSqrt( combatant ) > m_Mobile.RangePerception + 1 )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I cannot find {0}, so my guard is up", combatant.Name );

				Action = ActionType.Guard;

				return true;
			}
			else
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I should be closer to {0}", combatant.Name );
			}

			if ( !m_Mobile.Controlled && !m_Mobile.Summoned && !m_Mobile.IsParagon )
			{
				if ( m_Mobile.Hits < m_Mobile.HitsMax * 20/100 )
				{
					// We are low on health, should we flee?
					bool flee = false;

					if ( m_Mobile.Hits < combatant.Hits )
					{
						// We are more hurt than them
						int diff = combatant.Hits - m_Mobile.Hits;

						flee = ( Utility.Random( 0, 100 ) < (10 + diff) ); // (10 + diff)% chance to flee
					}
					else
					{
						flee = Utility.Random( 0, 100 ) < 10; // 10% chance to flee
					}

					if ( flee )
					{
						if ( m_Mobile.Debug )
							m_Mobile.DebugSay( "I am going to flee from {0}", combatant.Name );

						Action = ActionType.Flee;
					}
				}
			}

			if ( TryToHeal() )
				return true;
			else if ( m_Mobile.Spell == null && DateTime.Now > m_NextCastTime && m_Mobile.InRange( combatant, 12 ) )
			{
				m_NextCastTime = DateTime.Now + TimeSpan.FromSeconds( 4 );

				List<int> skill = new List<int>();

				if ( m_CanUseBard )
					skill.Add( 0 );
				if ( m_CanUseBushido )
					skill.Add( 1 );
				if ( m_CanUseChivalry )
					skill.Add( 2 );
				if ( m_CanUseMagery )
					skill.Add( 3 );
				if ( m_CanUseNecromancy )
					skill.Add( 4 );
				if ( m_CanUseNinjitsu )
					skill.Add( 5 );

				if ( skill.Count == 0 )
					return true;

				int whichone = skill[0];

				if ( skill.Count > 1 )
					whichone = skill[ Utility.Random( skill.Count ) ];

				switch( whichone )
				{
					case 0: BardPower(); break;
					case 1: BushidoPower(); break;
					case 2: ChivalryPower(); break;
					case 3: MageryPower(); break;
					case 4: NecromancerPower(); break;
					case 5: NinjitsuPower(); break;
				}

			}

			return true;
		}

		public override bool DoActionGuard()
		{
			if ( !m_Mobile.Hidden && !m_Mobile.Poisoned && m_CanUseNinjitsu )
				m_Mobile.UseSkill( SkillName.Hiding );

			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				base.DoActionGuard();
			}

			return true;
		}

		public bool OnFailedMove()
		{
			var canOnlySwim = m_Mobile.CanSwim && m_Mobile.CantWalk;
			if( m_CanUseMagery && !canOnlySwim && !m_Mobile.DisallowAllMoves && !Server.Mobiles.BasePirate.IsSailor( m_Mobile ) && DateTime.Now > m_NextTeleportTime )
			{
				if( m_Mobile.Target != null )
					m_Mobile.Target.Cancel( m_Mobile, TargetCancelType.Canceled );

				new TeleportSpell( m_Mobile, null ).Cast();

				m_Mobile.DebugSay( "I am stuck, I'm going to try teleporting away" );

				return true;
			}

			if( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true ) )
			{
				if( m_Mobile.Debug )
					m_Mobile.DebugSay( "My move is blocked, so I am going to attack {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;

				return true;
			}
			else
			{
				m_Mobile.DebugSay( "I am stuck" );
			}

			return false;
		}

		public override bool DoActionFlee()
		{
			if ( m_Mobile.CanSwim )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I can swim, so I will continue fighting" );

				Action = ActionType.Combat;
				return true;
			}

			if ( m_Mobile.Hits > (m_Mobile.HitsMax / 2) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I am stronger now, so I will continue fighting" );

				Action = ActionType.Combat;
			}
			else if ( m_CanUseNinjitsu && m_Mobile.Combatant != null && m_SmokeBombs > 0 && m_Mobile.Mana >= 10 && m_Mobile.Hidden == false )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I am using a smoke bomb. " );

				--m_SmokeBombs;

				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have {0} smoke bombs left.", m_SmokeBombs.ToString() );

				m_Mobile.Mana -= 10;
				m_Mobile.Hidden = true;
				Server.SkillHandlers.Stealth.OnUse( m_Mobile );
				m_Mobile.FixedParticles( 0x3709, 1, 30, 9904, 1108, 6, EffectLayer.RightFoot );
				m_Mobile.PlaySound( 0x22F );
			}

			m_Mobile.FocusMob = m_Mobile.Combatant;
			base.DoActionFlee();

			return true;
		}

		public override bool MoveTo( Mobile m, bool run, int range )
		{
			if ( m_Mobile.Hidden && !m_Mobile.Poisoned && m_CanUseNinjitsu )
			{
				Server.SkillHandlers.Stealth.OnUse( m_Mobile );

				if ( base.MoveTo( m, false, range ) == false )
				{
					if ( m_Mobile.Hidden && m_Mobile.AllowedStealthSteps >= 1 && m_CanUseNinjitsu )
					{
						Spell spell = new Shadowjump( m_Mobile, null );
						spell.Cast();
					}

					return false;
				}
				else
					return true;
			}
			else if ( m != null && ( m_MoveErratically || m_RequireRanged || m_PrefersRanged ) )
			{
				bool forceMove = m_MoveErratically;
				bool shouldRetreat = m_MoveErratically || m_Mobile.InRange( m, 2 );
				if ( shouldRetreat && CheckMove() )
				{
					if ( Utility.RandomDouble() < 0.25 ) // Only retreat a fraction of the time
					{
						Direction d = Direction.North;

						// Run away from the master (to allow the master to corner this mob)
						Mobile focusTarget = m is BaseCreature ? ((BaseCreature)m).GetMaster() : m;
						if ( focusTarget == null) focusTarget = m;

						switch( m_Mobile.GetDirectionTo( focusTarget ) )
						{
							case Direction.North: d = Direction.South; break;
							case Direction.South: d = Direction.North; break;
							case Direction.East: d = Direction.West; break;
							case Direction.West: d = Direction.East; break;
							case Direction.Up: d = Direction.Down; break;
							case Direction.Down: d = Direction.Up; break;
							case Direction.Right: d = Direction.Left; break;
							case Direction.Left: d = Direction.Right; break;
						}

						return DoMove( d, run );
						// base.DoActionFlee();
					}
					else if ( !forceMove ) return true; // Stay put instead
				}
				
				// Stay put as long as they're within range
				if ( !forceMove && m_Mobile.InRange( m, 4 ) )
					return true;
			}

			return base.MoveTo( m, run, range );
		}



		public override bool WalkMobileRange( Mobile m, int iSteps, bool bRun, int iWantDistMin, int iWantDistMax )
		{
			if ( m_Mobile.Hidden && !m_Mobile.Poisoned && m_CanUseNinjitsu )
			{
				Server.SkillHandlers.Stealth.OnUse( m_Mobile );

				return base.WalkMobileRange( m, iSteps, false, iWantDistMin, iWantDistMax );
			}
			else
				return base.WalkMobileRange( m, iSteps, bRun, iWantDistMin, iWantDistMax );
		}

		public override void WalkRandom( int iChanceToNotMove, int iChanceToDir, int iSteps )
		{
			if ( m_Mobile.Hidden && m_CanUseNinjitsu )
				Server.SkillHandlers.Stealth.OnUse( m_Mobile );
			
			base.WalkRandom( iChanceToNotMove, iChanceToDir, iSteps );
		}




		private static int[] m_RandomLocations = new int[]
		{
			-1, -1, -1,  0, -1,  1,
			 0, -1,  0,  1,  1, -1,
			 1,  0,  1,  1, -2, -2,
			-2, -1, -2,  0, -2,  1,
			-2,  2, -1, -2, -1,  2,
			 0, -2,  0,  2,  1, -2,
			 1,  2,  2, -2,  2, -1,
			 2,  0,  2,  1,  2,  2 
		};

		private void MarkTeleport()
		{
			m_TeleportAttempts = 0;
			m_NextTeleportTime = DateTime.Now.Add(TELEPORT_DELAY);
		}

		private bool ProcessTarget()
		{
			Target targ = m_Mobile.Target;

			if ( targ == null )
				return false;

			Mobile toTarget;

			toTarget = m_Mobile.Combatant;

			//if ( toTarget != null )
				//RunTo( toTarget );

			if ( targ is DispelSpell.InternalTarget && !(m_Mobile.AutoDispel) )
			{
				List<Mobile> targets = new List<Mobile>();

				foreach ( Mobile m in m_Mobile.GetMobilesInRange( 12 ) )
				{
					if ( m is BaseCreature )
					{
						if ( ((BaseCreature)m).IsDispellable && CanTarget( m_Mobile, m ) )
							targets.Add( m );
					}
				}

				if ( targets.Count >= 0 )
				{
					int whichone = Utility.RandomMinMax( 0, targets.Count );

					if ( targets[whichone] != null )
						targ.Invoke( m_Mobile, targets[whichone] );
				}
			}
			else if ( targ is TeleportSpell.InternalTarget || targ is Shadowjump.InternalTarget )
			{
				if ( targ is Shadowjump.InternalTarget && !m_Mobile.Hidden )
					return false;
				
				Map map = m_Mobile.Map;

				if ( MAX_TELEPORT_ATTEMPTS < ++m_TeleportAttempts || map == null || toTarget == null || toTarget.Deleted )
				{
					targ.Cancel( m_Mobile, TargetCancelType.Canceled );
					MarkTeleport();

					return true;
				}

				int px, py;
				bool teleportAway = m_Mobile.Hits < (m_Mobile.HitsMax / 10);

				if ( teleportAway )
				{
					int rx = m_Mobile.X - toTarget.X;
					int ry = m_Mobile.Y - toTarget.Y;
					double d = m_Mobile.GetDistanceToSqrt( toTarget );

					px = toTarget.X + (int)(rx * (10 / d));
					py = toTarget.Y + (int)(ry * (10 / d));
				}
				else
				{
					px = toTarget.X;
					py = toTarget.Y;
				}

				int teleRange = targ.Range;

				if ( teleRange < 0 )
					teleRange = 12;

				for ( int i = 0; i < m_RandomLocations.Length; i += 2 )
				{
					Point3D p = new Point3D( px + m_RandomLocations[i], py + m_RandomLocations[i + 1], 0 );

					if ( TryTeleport( p, map, targ, teleRange ) ) return true;
				}

				// Try finding a path to the target
				IPoint3D goal = new Point3D( toTarget.X, toTarget.Y, 0 );
				Spells.SpellHelper.GetSurfaceTop(ref goal);
				var goalPoint = new Point3D(goal.X, goal.Y, goal.Z);
				var path = new MovementPath( m_Mobile, goalPoint );
				if ( path.Success )
				{
					var x = goalPoint.X;
					var y = goalPoint.Y;
					var points = new List<Point3D>();
					foreach ( Direction d in path.Directions )
					{
						Movement.Movement.Offset( d, ref x, ref y );
						var tempPoint = new Point3D( x, y, 0 );
						if ( !m_Mobile.InRange( tempPoint, teleRange ) ) continue;

						points.Add( tempPoint );
					}

					// Try to get as close as possible to the goal
					foreach ( var p in points.Reverse<Point3D>() )
					{
						if ( TryTeleport( p, map, targ, teleRange ) ) return true;
					}
				}

				// Pick a random point within range
				for ( int i = 0; i < 10; ++i )
				{
					Point3D p = new Point3D( m_Mobile.X - teleRange + Utility.Random( teleRange * 2 + 1 ), m_Mobile.Y - teleRange + Utility.Random( teleRange * 2 + 1 ), 0 );
					if ( TryTeleport( p, map, targ, teleRange ) ) return true;
				}
			}
			// TODO: Verify Animating will not destroy the items on the corpse
			// else if ( targ is AnimateDeadSpell.InternalTarget )
			// {
			// 	Type type = null;

			// 	List<Item> itemtargets = new List<Item>();

			// 	foreach ( Item itemstofind in m_Mobile.GetItemsInRange( 5 ) )
			// 	{
			// 		if ( itemstofind is Corpse )
			// 		{
			// 			itemtargets.Add( itemstofind );
			// 		}
			// 	}

			// 	for ( int i = 0; i < itemtargets.Count; ++i )
			// 	{
			// 		Corpse items = (Corpse)itemtargets[i];

			// 		if ( items.Owner != null )
			// 			type = items.Owner.GetType();

			// 		if ( items.ItemID != 0x2006 || items.Channeled || type == typeof( PlayerMobile ) || type == null || (items.Owner != null && items.Owner.Fame < 100) || ((items.Owner != null) && (items.Owner is BaseCreature) && (((BaseCreature)items.Owner).Summoned || ((BaseCreature)items.Owner).IsBonded)) )
			// 			continue;
			// 		else
			// 		{
			// 			targ.Invoke( m_Mobile, items );
			// 			break;
			// 		}
			// 	}

			// 	if ( targ != null )
			// 		targ.Cancel( m_Mobile, TargetCancelType.Canceled );

			// }
			else if ( (targ.Flags & TargetFlags.Harmful) != 0 && toTarget != null )
			{
				if ( (targ.Range == -1 || m_Mobile.InRange( toTarget, targ.Range )) && m_Mobile.CanSee( toTarget ) && m_Mobile.InLOS( toTarget ) )
				{
					targ.Invoke( m_Mobile, toTarget );
					return true;
				}
			}
			else if ( (targ.Flags & TargetFlags.Beneficial) != 0 )
			{
				targ.Invoke( m_Mobile, m_Mobile );
				return true;
			}
			else
			{
				targ.Cancel( m_Mobile, TargetCancelType.Canceled );
			}

			return false;
		}

		private bool TryTeleport( Point3D p, Map map, Target targ, int teleRange )
		{
			if (targ.Range != -1 && !m_Mobile.InRange(p, teleRange)) return false;

			var highest = map.GetHighestEntity(p.X, p.Y);
			if (highest is LandTile) p.Z = ((LandTile)highest).Z;
			else if (highest is StaticTile) p.Z = ((StaticTile)highest).Z;
			else if (highest is Item) p.Z = ((Item)highest).Z;

			if (m_Mobile.InLOS(p) && (
					(m_Mobile.CanSwim && BaseBoat.FindBoatAt(p, map) != null) ||
					(map.CanSpawnMobile(p) && !SpellHelper.CheckMulti(p, map))
				)
			)
			{
				object entityTarget;
				if (highest is LandTile) entityTarget = new LandTarget(p, map);
				else if (highest is StaticTile) entityTarget = new StaticTarget(p, ((StaticTile)highest).ID);
				else entityTarget = highest;

				targ.Invoke(m_Mobile, entityTarget);
				MarkTeleport();

				return true;
			}

			return false;
		}

		#region Targeting
		public static Mobile FindRandomTarget( Mobile from )
		{
			return FindRandomTarget( from, true );
		}

		public static Mobile FindRandomTarget( Mobile from, bool allowcombatant )
		{
			List<Mobile> list = new List<Mobile>();

			foreach ( Mobile m in from.GetMobilesInRange( 12 ) )
			{
				if ( m != null && m != from )
					if ( CanTarget( from, m ) && from.InLOS( m ) )
					{
						if ( allowcombatant && m == from.Combatant )
							continue;
						else
							list.Add( m );
					}
			}

			if ( list.Count == 0 )
				return null;
			if ( list.Count == 1 )
				return list[0];

			return list[Utility.Random( list.Count )];
		}

		public static bool CanTarget( Mobile from, Mobile to )
		{
			return CanTarget( from, to, true, false, false );
		}

		public static bool CanTarget( Mobile from, Mobile to, bool harm )
		{
			return CanTarget( from, to, harm, false, false );
		}

		public static bool CanTarget( Mobile from, Mobile to, bool harm, bool checkguildparty, bool allownull )
		{
			if ( to == null )
				return false;
			else if ( from == null )
				return allownull;
			else if ( from == to && !harm )
				return true;
			else if ( (harm && to.Blessed) || (to.AccessLevel != AccessLevel.Player && to.Hidden) )
				return false;

			if ( checkguildparty )
			{
				//Guilds
				Guild fromguild = GetGuild( from );
				Guild toguild = GetGuild( to );

				if ( fromguild != null && toguild != null )
					if ( fromguild == toguild || fromguild.IsAlly( toguild ) )
						return !harm;

				//Parties
				Party p = GetParty( from );

				if ( p != null && p.Contains( to ) )
					return !harm;
			}

			//Default
			if ( harm )
				return ( IsGoodGuy( from ) && !(IsGoodGuy( to )) ) || ( !(IsGoodGuy( from )) && IsGoodGuy( to ) );
			else
				return  ( IsGoodGuy( from ) && IsGoodGuy( to ) ) || ( !(IsGoodGuy( from )) && !(IsGoodGuy( to )) );
		}

		public static bool IsGoodGuy( Mobile m )
		{
			if ( m.Criminal )
				return false;

			if ( m.Player && m.Kills < 5 )
				return true;

			if ( m is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)m;

				if ( bc.Controlled || bc.Summoned )
				{
					if ( bc.ControlMaster != null )
						return IsGoodGuy( bc.ControlMaster );
					else if ( bc.SummonMaster != null )
						return IsGoodGuy( bc.SummonMaster );
				}
			}

			return false;
		}

		public static Guild GetGuild( Mobile m )
		{
			Guild guild = m.Guild as Guild;

			if ( guild == null && m is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)m;
				m = bc.ControlMaster;

				if ( m != null )
					guild = m.Guild as Guild;

				m = bc.SummonMaster;

				if ( m != null && guild == null )
					guild = m.Guild as Guild;
			}

			return guild;
		}

		public static Party GetParty( Mobile m )
		{
			Party party = Party.Get( m );

			if ( party == null && m is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)m;
				m = bc.ControlMaster;

				if ( m != null )
					party = Party.Get( m );

				m = bc.SummonMaster;

				if ( m != null && party == null )
					party = Party.Get( m );
			}

			return party;
		}
		#endregion




	}
}