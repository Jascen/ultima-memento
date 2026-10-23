using System;
using Server.Items;  
using Server.Network;

namespace Server.Mobiles 
{
	[CorpseName( "a broken machine" )] 
	public class GolemFighter : BaseCreature
	{
		public const int MaxControlSlots = 3;

		private bool m_Stunning;

		public int PorterExodus;
		[CommandProperty(AccessLevel.Owner)]
		public int Fighter_Exodus{ get { return PorterExodus; } set { PorterExodus = value; InvalidateProperties(); } }

		private DateTime m_NextTalking;
		public DateTime NextTalking{ get{ return m_NextTalking; } set{ m_NextTalking = value; } }
		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( DateTime.Now >= m_NextTalking && InRange( m, 20 ) )
			{
				this.Loyalty = 100;
				m_NextTalking = (DateTime.Now + TimeSpan.FromSeconds( 300 ));
			}
		}

		[Constructable] 
		public GolemFighter( ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8 )
		{
			m_NextTalking = (DateTime.Now + TimeSpan.FromSeconds( 60 ));

			Name = "a golem";
			Body = 752;
			ControlSlots = MaxControlSlots;
			ActiveSpeed = 0.1;
			PassiveSpeed = 0.2;
		}

		public override bool ClickTitle{ get{ return false; } }
		public override bool ShowFameTitle{ get{ return false; } }
		public override bool AlwaysAttackable{ get{ return true; } }
		public override bool DeleteOnRelease{ get{ return true; } }
		public override bool DeleteCorpseOnDeath{ get{ return true; } }
		public override bool IsDispellable { get { return false; } }
		public override bool IsBondable{ get{ return false; } }
		public override bool CanBeRenamedBy( Mobile from ){ return true; }
		public override bool BleedImmune{ get{ return true; } }
		public override bool BardImmune { get { return !Core.SE; } }
		public override bool Unprovokable { get { return Core.SE; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override bool IsScaredOfScaryThings{ get{ return false; } }
		public override bool IsScaryToPets{ get{ return true; } }

		public GolemFighter( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void OnAfterSpawn()
		{
			double scalar = 1.0;

			if ( Hue == 2118 )
			{
				Title = "of Exodus";
				if ( PorterExodus == 1 ){ scalar = 1.7; }
				else if ( PorterExodus == 2 ){ scalar = 1.8; }
				else if ( PorterExodus == 3 ){ scalar = 1.9; }
				else if ( PorterExodus == 4 ){ scalar = 2.0; }
				else if ( PorterExodus == 5 ){ scalar = 2.1; }
				else if ( PorterExodus == 6 ){ scalar = 2.2; }
				else if ( PorterExodus == 7 ){ scalar = 2.3; }
				else if ( PorterExodus == 8 ){ scalar = 2.4; }
				else if ( PorterExodus == 9 ){ scalar = 2.5; }
			}
			else if ( Resource == CraftResource.DullCopper ){ scalar = 1.1; }
			else if ( Resource == CraftResource.ShadowIron ){ scalar = 1.2; }
			else if ( Resource == CraftResource.Copper ){ scalar = 1.3; }
			else if ( Resource == CraftResource.Bronze ){ scalar = 1.4; }
			else if ( Resource == CraftResource.Gold ){ scalar = 1.5; }
			else if ( Resource == CraftResource.Agapite ){ scalar = 1.6; }
			else if ( Resource == CraftResource.Verite ){ scalar = 1.7; }
			else if ( Resource == CraftResource.Valorite ){ scalar = 1.8; }
			else { Hue = 0x430; scalar = 1.0; }

			SetStr( (int)(300*scalar) );
			SetDex( (int)(100*scalar) );
			SetInt( (int)(60*scalar) );

			SetHits( (int)(210*scalar) );

			SetDamage( (int)(5*scalar), (int)(10*scalar) );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, (int)(26*scalar) );
			SetResistance( ResistanceType.Fire, (int)(20*scalar) );
			SetResistance( ResistanceType.Cold, (int)(20*scalar) );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, (int)(20*scalar) );

			SetSkill( SkillName.MagicResist, (50.0*scalar) );
			SetSkill( SkillName.Tactics, (50.0*scalar) );
			SetSkill( SkillName.FistFighting, (50.0*scalar) );
		}

		public override bool OnBeforeDeath()
		{
			Effects.SendLocationEffect(this.Location, this.Map, 0x36B0, 9, 10, 0, 0);
			this.PlaySound( 0x307 );
			this.AIObject.DoOrderRelease();
			return false;
		}

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); // version
            writer.Write( PorterExodus );
			Loyalty = 100;
		} 

		public override int GetAngerSound()
		{
			return 541;
		}

		public override int GetIdleSound()
		{
			return 542;
		}

		public override int GetDeathSound()
		{
			return 545;
		}

		public override int GetAttackSound()
		{
			return 562;
		}

		public override int GetHurtSound()
		{
			return 320;
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			if ( !m_Stunning && 0.3 > Utility.RandomDouble() )
			{
				m_Stunning = true;

				defender.Animate( 21, 6, 1, true, false, 0 );
				this.PlaySound( 0xEE );
				defender.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, "You have been stunned by a colossal blow!" );

				BaseWeapon weapon = this.Weapon as BaseWeapon;
				if ( weapon != null )
					weapon.OnHit( this, defender );

				if ( defender.Alive )
				{
					defender.Frozen = true;
					Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( Recover_Callback ), defender );
				}
			}
		}

		private void Recover_Callback( object state )
		{
			Mobile defender = state as Mobile;

			if ( defender != null )
			{
				defender.Frozen = false;
				defender.Combatant = null;
				defender.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, "You recover your senses." );
			}

			m_Stunning = false;
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			if ( Controlled || Summoned )
			{
				Mobile master = ( this.ControlMaster );

				if ( master == null )
					master = this.SummonMaster;

				if ( master != null && master.Player && master.Map == this.Map && master.InRange( Location, 20 ) )
				{
					if ( master.Mana >= amount )
					{
						master.Mana -= amount;
					}
					else
					{
						amount -= master.Mana;
						master.Mana = 0;
						master.Damage( amount );
					}
				}
			}

			base.OnDamage( amount, from, willKill );
		}

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt();
			PorterExodus = reader.ReadInt();

			LeaveNowTimer thisTimer = new LeaveNowTimer( this ); 
			thisTimer.Start(); 
		} 
	}
}