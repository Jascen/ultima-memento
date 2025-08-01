using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a roc corpse" )]
	public class YoungRoc : BaseMount
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.Dismount;
		}

		private static int GetHue()
		{
			int rand = Utility.Random( 527 );

			/*

			500	527	No Hue Color	94.88%	0
			10	527	Green			1.90%	0x8295
			10	527	Green			1.90%	0x8163	(Very Close to Above Green)	//this one is an approximation
			5	527	Dark Green		0.95%	0x87D4
			1	527	Valorite		0.19%	0x88AB
			1	527	Midnight Blue	0.19%	0x8258

			 * */

			if( rand <= 0 )
				return 0x8258;
			else if( rand <= 1 )
				return 0x88AB;
			else if( rand <= 6 )
				return 0x87D4;
			else if( rand <= 16 )
				return 0x8163;
			else if( rand <= 26 )
				return 0x8295;

			return 0;
		}

		[Constructable]
		public YoungRoc(): base( "a rocling", 243, 0x3E94, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Hue = GetHue();
			BaseSoundID = 0x2EE;

			SetStr( 301, 410 );
			SetDex( 171, 270 );
			SetInt( 301, 325 );

			SetHits( 401, 600 );
			SetMana( 60 );

			SetDamage( 18, 23 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 45, 70 );
			SetResistance( ResistanceType.Fire, 60, 80 );
			SetResistance( ResistanceType.Cold, 5, 15 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.Anatomy, 75.1, 80.0 );
			SetSkill( SkillName.MagicResist, 85.1, 100.0 );
			SetSkill( SkillName.Tactics, 100.1, 110.0 );
			SetSkill( SkillName.FistFighting, 100.1, 120.0 );

			Fame = 10000;
			Karma = -10000;

			Tamable = true;
			ControlSlots = 3;
			MinTameSkill = 98.7;
		}

		public override void OnCarve( Mobile from, Corpse corpse, Item with )
		{
			base.OnCarve( from, corpse, with );

			if ( Utility.RandomMinMax( 1, 5 ) == 1 )
			{
				Item egg = new Eggs( Utility.RandomMinMax( 1, 7 ) );
				corpse.DropItem( egg );
			}
		}

		public override bool OverrideBondingReqs()
		{
			if ( ControlMaster.Skills[SkillName.Bushido].Base >= 90.0 )
				return true;
			return false;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Gems, 2 );
		}

		public override double GetControlChance( Mobile m, bool useBaseSkill )
		{
			double tamingChance = base.GetControlChance( m, useBaseSkill );

			if( tamingChance >= 0.95 )
			{
				return tamingChance;
			}

			double skill = (useBaseSkill? m.Skills.Bushido.Base : m.Skills.Bushido.Value);

			if( skill < 90.0 )
			{
				return tamingChance;
			}

			double bushidoChance = ( skill - 30.0 ) / 100;

			if( m.Skills.Bushido.Base >= 120 )
				bushidoChance += 0.05;

			return bushidoChance > tamingChance ? bushidoChance : tamingChance;
		}

		public override int TreasureMapLevel { get { return 3; } }
		public override int Meat { get { return 16; } }
		public override int Hides { get { return 60; } }
		public override FoodType FavoriteFood { get { return FoodType.Meat; } }
		public override bool CanAngerOnTame { get { return true; } }
		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override int Feathers{ get{ return 50; } }

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			if( 0.1 > Utility.RandomDouble() )
			{
				/* Grasping Claw
				 * Start cliloc: 1070836
				 * Effect: Physical resistance -15% for 5 seconds
				 * End cliloc: 1070838
				 * Effect: Type: "3" - From: "0x57D4F5B" (player) - To: "0x0" - ItemId: "0x37B9" - ItemIdName: "glow" - FromLocation: "(1149 808, 32)" - ToLocation: "(1149 808, 32)" - Speed: "10" - Duration: "5" - FixedDirection: "True" - Explode: "False"
				 */

				ExpireTimer timer = (ExpireTimer)m_Table[defender];

				if( timer != null )
				{
					timer.DoExpire();
					defender.SendLocalizedMessage( 1070837 ); // The creature lands another blow in your weakened state.
				}
				else
					defender.SendLocalizedMessage( 1070836 ); // The blow from the creature's claws has made you more susceptible to physical attacks.

				int effect = -(defender.PhysicalResistance * 15 / 100);

				ResistanceMod mod = new ResistanceMod( ResistanceType.Physical, effect );

				defender.FixedEffect( 0x37B9, 10, 5 );
				defender.AddResistanceMod( mod );

				timer = new ExpireTimer( defender, mod, TimeSpan.FromSeconds( 5.0 ) );
				timer.Start();
				m_Table[defender] = timer;
			}
		}

		private static Hashtable m_Table = new Hashtable();

		private class ExpireTimer : Timer
		{
			private Mobile m_Mobile;
			private ResistanceMod m_Mod;

			public ExpireTimer( Mobile m, ResistanceMod mod, TimeSpan delay )
				: base( delay )
			{
				m_Mobile = m;
				m_Mod = mod;
				Priority = TimerPriority.TwoFiftyMS;
			}

			public void DoExpire()
			{
				m_Mobile.RemoveResistanceMod( m_Mod );
				Stop();
				m_Table.Remove( m_Mobile );
			}

			protected override void OnTick()
			{
				m_Mobile.SendLocalizedMessage( 1070838 ); // Your resistance to physical attacks has returned.
				DoExpire();
			}
		}

		public YoungRoc( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)2 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if( version == 0 )
				Timer.DelayCall( TimeSpan.Zero, delegate { Hue = GetHue(); } );

			if( version <= 1 )
				Timer.DelayCall( TimeSpan.Zero, delegate { if( InternalItem != null ) { InternalItem.Hue = this.Hue; } } );

			if( version < 2 )
			{
				for ( int i = 0; i < Skills.Length; ++i )
				{
					Skills[i].Cap = Math.Max( 100.0, Skills[i].Cap * 0.9 );

					if ( Skills[i].Base > Skills[i].Cap )
					{
						Skills[i].Base = Skills[i].Cap;
					}
				}
			}
		}
	}
}
