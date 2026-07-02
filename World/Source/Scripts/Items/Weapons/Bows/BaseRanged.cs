using System;
using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Mobiles;

namespace Server.Items
{
	public abstract class BaseRanged : BaseMeleeWeapon
	{
		public abstract int EffectID{ get; }
		public abstract Type AmmoType{ get; }
		public abstract Item Ammo{ get; }
		
		private bool CanReplenishableAmmo
		{
			get
			{
				return AmmoType == typeof(Arrow) || AmmoType == typeof(Bolt);
			}
		}

		public override int DefHitSound{ get{ return 0x234; } }
		public override int DefMissSound{ get{ return 0x238; } }

		public override SkillName DefSkill{ get{ return SkillName.Marksmanship; } }
		public override WeaponType DefType{ get{ return WeaponType.Ranged; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.ShootXBow; } }

		public override SkillName AccuracySkill{ get{ return SkillName.Marksmanship; } }

		private Timer m_RecoveryTimer; // so we don't start too many timers
		private bool m_Balanced;
		private int m_Velocity;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Balanced
		{
			get{ return m_Balanced; }
			set{ m_Balanced = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Velocity
		{
			get{ return m_Velocity; }
			set{ m_Velocity = value; InvalidateProperties(); }
		}

		public BaseRanged( int itemID ) : base( itemID )
		{
			Layer = Layer.TwoHanded;
		}

		public BaseRanged( Serial serial ) : base( serial )
		{
		}

		public override TimeSpan OnSwing( Mobile attacker, Mobile defender )
		{
			WeaponAbility a = WeaponAbility.GetCurrentAbility( attacker );

			// Make sure we've been standing still for .25/.5/1 second depending on Era
			if ( DateTime.Now > (attacker.LastMoveTime + TimeSpan.FromSeconds( Core.SE ? 0.25 : (Core.AOS ? 0.5 : 1.0) )) || (Core.AOS && WeaponAbility.GetCurrentAbility( attacker ) is MovingShot) )
			{
				bool canSwing = true;

				canSwing =  !attacker.Paralyzed && !attacker.Frozen ;

				if ( canSwing )
				{
					Spell sp = attacker.Spell as Spell;
					canSwing =  sp == null || !sp.IsCasting || !sp.BlocksMovement ;
				}

				if ( canSwing )
				{
					PlayerMobile p = attacker as PlayerMobile;
					canSwing = p == null || p.PeacedUntil <= DateTime.Now;
				}

				if ( canSwing && attacker.HarmfulCheck( defender ) )
				{
					attacker.DisruptiveAction();
					attacker.Send( new Swing( 0, attacker, defender ) );

					if ( OnFired( attacker, defender ) )
					{
						if ( CheckHit( attacker, defender ) )
							OnHit( attacker, defender );
						else
							OnMiss( attacker, defender );
					}
				}

				if ( !( a is ShadowStrike || a is ShadowInfectiousStrike ) )
					attacker.RevealingAction();

				return GetDelay( attacker );
			}
			else
			{
				if ( !( a is ShadowStrike || a is ShadowInfectiousStrike ) )
					attacker.RevealingAction();

				return TimeSpan.FromSeconds( 0.25 );
			}
		}

		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			if ( CanReplenishableAmmo && attacker.Player && !defender.Player && (defender.Body.IsAnimal || defender.Body.IsMonster) && 0.4 >= Utility.RandomDouble() )
				defender.AddToBackpack( Ammo );

			if ( defender is BaseCreature && attacker.Player && AmmoType == typeof(ThrowingWeapon) )
			{
				BaseCreature bc = (BaseCreature)defender;

				if ( attacker.FindItemOnLayer( Layer.OneHanded ) != null )
				{
					if ( attacker.FindItemOnLayer( Layer.OneHanded ) is IThrowingGloves )
					{
						IThrowingGloves glove = (IThrowingGloves)( attacker.FindItemOnLayer( Layer.OneHanded ) );
						ThrowingWeapon knife = new ThrowingWeapon();

						if ( glove.GloveType == ThrowingWeaponType.Stones ){ knife.Ammo = ThrowingWeaponType.Stones; }
						else if ( glove.GloveType == ThrowingWeaponType.Axes ){ knife.Ammo = ThrowingWeaponType.Axes; }
						else if ( glove.GloveType == ThrowingWeaponType.Daggers ){ knife.Ammo = ThrowingWeaponType.Daggers; }
						else if ( glove.GloveType == ThrowingWeaponType.Darts ){ knife.Ammo = ThrowingWeaponType.Darts; }
						else if ( glove.GloveType == ThrowingWeaponType.Cards && Server.Misc.GetPlayerInfo.isJester( attacker ) ){ knife.Ammo = ThrowingWeaponType.Cards; }
						else if ( glove.GloveType == ThrowingWeaponType.Tomatoes && Server.Misc.GetPlayerInfo.isJester( attacker ) ){ knife.Ammo = ThrowingWeaponType.Tomatoes; }
						else { knife.Ammo = ThrowingWeaponType.Stars; }

						bc.PackItem( knife );
					}
				}
			}

			if ( Core.ML && m_Velocity > 0 )
			{
				int bonus = (int) attacker.GetDistanceToSqrt( defender );

				if ( bonus > 0 && m_Velocity > Utility.Random( 100 ) )
				{
					AOS.Damage( defender, attacker, bonus * 3, 100, 0, 0, 0, 0 );

					if ( attacker.Player )
						attacker.SendLocalizedMessage( 1072794 ); // Your arrow hits its mark with velocity!

					if ( defender.Player )
						defender.SendLocalizedMessage( 1072795 ); // You have been hit by an arrow with velocity!
				}
			}

			base.OnHit( attacker, defender, damageBonus );
		}

		public override void OnMiss( Mobile attacker, Mobile defender )
		{
			if ( attacker.Player && 0.4 >= Utility.RandomDouble() )
			{
				if ( CanReplenishableAmmo )
				{
					PlayerMobile p = attacker as PlayerMobile;

					if ( p != null )
					{
						Type ammo = AmmoType;

						if ( p.RecoverableAmmo.ContainsKey( ammo ) )
							p.RecoverableAmmo[ ammo ]++;
						else
							p.RecoverableAmmo.Add( ammo, 1 );

						if ( !p.Warmode )
						{
							if ( m_RecoveryTimer == null )
								m_RecoveryTimer = Timer.DelayCall( TimeSpan.FromSeconds( 10 ), new TimerCallback( p.RecoverAmmo ) );

							if ( !m_RecoveryTimer.Running )
								m_RecoveryTimer.Start();
						}
					}
				}
			}

			base.OnMiss( attacker, defender );
		}

		public virtual bool OnFired( Mobile attacker, Mobile defender )
		{
			if ( this is IThrowingGloves && attacker.Player )
			{
				ThrowingWeaponType ammoType;
				IThrowingGloves glove = (IThrowingGloves)this;
				if ( glove.GloveType == ThrowingWeaponType.Stones ){ ammoType = ThrowingWeaponType.Stones; }
				else if ( glove.GloveType == ThrowingWeaponType.Axes ){ ammoType = ThrowingWeaponType.Axes; }
				else if ( glove.GloveType == ThrowingWeaponType.Daggers ){ ammoType = ThrowingWeaponType.Daggers; }
				else if ( glove.GloveType == ThrowingWeaponType.Darts ){ ammoType = ThrowingWeaponType.Darts; }
				else if ( glove.GloveType == ThrowingWeaponType.Cards && Server.Misc.GetPlayerInfo.isJester( attacker ) ){ ammoType = ThrowingWeaponType.Cards; }
				else if ( glove.GloveType == ThrowingWeaponType.Tomatoes && Server.Misc.GetPlayerInfo.isJester( attacker ) ){ ammoType = ThrowingWeaponType.Tomatoes; }
				else { ammoType = ThrowingWeaponType.Stars; glove.GloveType = ThrowingWeaponType.Stars; }

				foreach( ThrowingWeapon i in attacker.Backpack.FindItemsByType( typeof( ThrowingWeapon ), true ) )
				{
					i.Ammo = ammoType;
				}
			}

			attacker.MovingEffect( defender, EffectID, 18, 1, false, false );

			Server.Gumps.QuickBar.RefreshQuickBar( attacker );

			if ( attacker.Player )
			{
				BaseQuiver quiver = attacker.FindItemOnLayer( Layer.Cloak ) as BaseQuiver;
				Container pack = attacker.Backpack;

				if ( quiver == null || Utility.Random( 100 ) >= quiver.LowerAmmoCost )
				{
					// consume ammo
					if ( quiver != null && quiver.ConsumeTotal( AmmoType, 1 ) )
						quiver.InvalidateWeight();
					else if ( pack == null || !pack.ConsumeTotal( AmmoType, 1 ) )
						return false;
				}
				else if ( quiver.FindItemByType( AmmoType ) == null && ( pack == null || pack.FindItemByType( AmmoType ) == null ) )
				{
					// lower ammo cost should not work when we have no ammo at all
					return false;
				}
			}

			return true;
		}

		public int ArrowType( int category )
		{
			int arrow = 0xF42;

			if ( category > 0 )
			{
				if ( AosElementDamages.Fire > 50 ){ arrow = 0x5BDD; }
				else if ( AosElementDamages.Cold > 50 ){ arrow = 0x5BB5; }
				else if ( AosElementDamages.Poison > 50 ){ arrow = 0x5BDF; }
				else if ( AosElementDamages.Energy > 50 ){ arrow = 0x5BDB; }
				else { arrow = 0xF42; }
			}
			else
			{
				if ( AosElementDamages.Fire > 50 ){ arrow = 0x5BDE; }
				else if ( AosElementDamages.Cold > 50 ){ arrow = 0x5BDA; }
				else if ( AosElementDamages.Poison > 50 ){ arrow = 0x5BE0; }
				else if ( AosElementDamages.Energy > 50 ){ arrow = 0x5BDC; }
				else { arrow = 0x1BFE; }
			}

			return arrow;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 3 ); // version

			writer.Write( (bool) m_Balanced );
			writer.Write( (int) m_Velocity );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 3:
				{
					m_Balanced = reader.ReadBool();
					m_Velocity = reader.ReadInt();

					goto case 2;
				}
				case 2:
				case 1:
				{
					break;
				}
				case 0:
				{
					/*m_EffectID =*/ reader.ReadInt();
					break;
				}
			}

			if ( version < 2 )
			{
				WeaponAttributes.MageWeapon = 0;
				WeaponAttributes.UseBestSkill = 0;
			}
		}
	}
}