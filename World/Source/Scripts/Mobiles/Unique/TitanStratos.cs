using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Misc;
using Server.Engines.PartySystem;

namespace Server.Mobiles
{
	[CorpseName( "a titan corpse" )]
	public class TitanStratos : BaseCreature
	{
		public override int BreathPhysicalDamage{ get{ return 100; } }
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathColdDamage{ get{ return 0; } }
		public override int BreathPoisonDamage{ get{ return 0; } }
		public override int BreathEnergyDamage{ get{ return 0; } }
		public override int BreathEffectHue{ get{ return 0; } }
		public override int BreathEffectSound{ get{ return 0x654; } }
		public override int BreathEffectItemID{ get{ return 0; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool HasBreath{ get{ return true; } }
		public override double BreathEffectDelay{ get{ return 0.1; } }
		public override void BreathDealDamage( Mobile target, int form ){ base.BreathDealDamage( target, 44 ); }

		[Constructable]
		public TitanStratos() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Stratos";
			Title = "the titan of air";
			Body = 436;
			Hue = 0x4001;
			BaseSoundID = 655;

			SetStr( 898, 1030 );
			SetDex( 68, 200 );
			SetInt( 488, 620 );

			SetHits( 558, 599 );

			SetDamage( 29, 35 );

			SetDamageType( ResistanceType.Physical, 75 );
			SetDamageType( ResistanceType.Energy, 25 );

			SetResistance( ResistanceType.Physical, 65, 75 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 45, 55 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 70, 80 );

			SetSkill( SkillName.Psychology, 80.1, 100.0 );
			SetSkill( SkillName.Magery, 80.1, 100.0 );
			SetSkill( SkillName.Meditation, 52.5, 75.0 );
			SetSkill( SkillName.MagicResist, 100.3, 130.0 );
			SetSkill( SkillName.Tactics, 97.6, 100.0 );
			SetSkill( SkillName.FistFighting, 97.6, 100.0 );

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 70;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 3 );
			AddLoot( LootPack.Gems, 3 );
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			if ( this.Body == 13 && willKill == false && Utility.RandomBool() )
				this.Body = 436;
			else if ( willKill == false && Utility.RandomBool() )
				this.Body = 13;

			base.OnDamage( amount, from, willKill );
		}

		public override bool OnBeforeDeath()
		{
			int CanDie = 0;
			int CanKillIt = 0;
			Mobile winner = this;
			int RewardColor = 0xB74;

			foreach ( Mobile m in this.GetMobilesInRange( 30 ) )
			{
				if ( m is PlayerMobile && m.Map == this.Map && !m.Blessed )
				{
					Item obelisk = m.Backpack.FindItemByType( typeof ( ObeliskTip ) );
					if ( obelisk != null )
					{
						ObeliskTip tip = (ObeliskTip)obelisk;
						if ( tip.ObeliskOwner == m && tip.HasAir > 0 && tip.WonAir < 1 )
						{
							CanDie = 1;
							winner = m;
							tip.WonAir = 1;
							m.SendMessage( "You absord the Titan's power into the Breath of Air." );
							m.PlaySound( 0x65A );
							m.FixedParticles( 0x375A, 1, 30, 9966, 33, 2, EffectLayer.Head );
						}
					}
				}
			}
			if ( CanDie == 0 )
			{
				foreach ( Mobile m in this.GetMobilesInRange( 30 ) )
				{
					if ( m is PlayerMobile && m.Map == this.Map && !m.Blessed && ((PlayerMobile)m).IsTitanOfEther )
					{
						CanKillIt = 1;
					}
					if ( m is PlayerMobile && m.Map == this.Map && !m.Blessed ) // ANYONE WITH THE BLACKROCK CAN KILL IT
					{
						Item obelisk = m.Backpack.FindItemByType( typeof ( ObeliskTip ) );
						if ( obelisk != null )
						{
							ObeliskTip tip = (ObeliskTip)obelisk;
							if ( tip.ObeliskOwner == m && tip.HasAir > 0 && tip.WonAir > 0 )
							{
								CanKillIt = 1;
							}
						}
					}
				}
			}

			if ( CanDie == 0 && CanKillIt == 0 )
			{
				Say("No! The power of the wind will blow you away!");
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				return false;
			}
			else if ( CanKillIt == 0 )
			{
				this.Body = 13;
				string Iam = "the Titan of Air";
				PlayerMobile killer = MobileUtilities.TryGetKillingPlayer( this );
				Server.Misc.LoggingFunctions.LogSlayingLord( killer, Iam );
				if ( winner is PlayerMobile )
				{
					LoggingFunctions.LogGenericQuest( winner, "has obtained the power of the air titan" );
				}

				if ( winner != null )
				{
					if ( winner is BaseCreature )
						winner = ((BaseCreature)winner).GetMaster();

					if ( winner is PlayerMobile && !winner.Blessed )
					{
						Party p = Engines.PartySystem.Party.Get( winner );
						if ( p != null )
						{
							foreach ( PartyMemberInfo pmi in p.Members )
							{
								if ( pmi.Mobile is PlayerMobile && pmi.Mobile.InRange(this.Location, 20) && pmi.Mobile.Map == this.Map && !pmi.Mobile.Blessed && !Server.Misc.PlayerSettings.GetSpecialsKilled( pmi.Mobile, "TitanStratos" ) )
								{
									Server.Misc.PlayerSettings.SetSpecialsKilled( pmi.Mobile, "TitanStratos", true );
									ManualOfItems book = new ManualOfItems();
										book.Hue = RewardColor;
										book.ItemID = 0x1A97;
										book.Name = "Chest of Air Titan Relics";
										book.m_Charges = 1;
										book.m_Skill_1 = 0;
										book.m_Skill_2 = 0;
										book.m_Skill_3 = 0;
										book.m_Skill_4 = 0;
										book.m_Skill_5 = 0;
										book.m_Value_1 = 0.0;
										book.m_Value_2 = 0.0;
										book.m_Value_3 = 0.0;
										book.m_Value_4 = 0.0;
										book.m_Value_5 = 0.0;
										book.m_Slayer_1 = 5;
										book.m_Slayer_2 = 0;
										book.m_Owner = pmi.Mobile;
										book.m_Extra = "of the Sky";
										book.m_FromWho = "Taken from Stratos";
										book.m_HowGiven = "Acquired by";
										book.m_Points = 300;
										book.m_Hue = RewardColor;
										pmi.Mobile.AddToBackpack( book );

									pmi.Mobile.SendMessage("An item has appeared in your backpack!");
								}
							}
						}
						else if ( !Server.Misc.PlayerSettings.GetSpecialsKilled( winner, "TitanStratos" ) )
						{
							Server.Misc.PlayerSettings.SetSpecialsKilled( winner, "TitanStratos", true );
							ManualOfItems book = new ManualOfItems();
								book.Hue = RewardColor;
								book.ItemID = 0x1A97;
								book.Name = "Chest of Air Titan Relics";
								book.m_Charges = 1;
								book.m_Skill_1 = 0;
								book.m_Skill_2 = 0;
								book.m_Skill_3 = 0;
								book.m_Skill_4 = 0;
								book.m_Skill_5 = 0;
								book.m_Value_1 = 0.0;
								book.m_Value_2 = 0.0;
								book.m_Value_3 = 0.0;
								book.m_Value_4 = 0.0;
								book.m_Value_5 = 0.0;
								book.m_Slayer_1 = 5;
								book.m_Slayer_2 = 0;
								book.m_Owner = winner;
								book.m_Extra = "of the Sky";
								book.m_FromWho = "Taken from Stratos";
								book.m_HowGiven = "Acquired by";
								book.m_Points = 300;
								book.m_Hue = RewardColor;
								winner.AddToBackpack( book );

							winner.SendMessage("An item has appeared in your backpack!");
						}
					}
				}

				if ( GetPlayerInfo.LuckyKiller( winner.Luck ) && Utility.RandomMinMax( 1, 10 ) == 1 )
				{
					Item Arty = new Artifact_BootsofStratos();
					switch( Utility.RandomMinMax( 0, 3 ) )
					{
						case 1: Arty.Delete(); Arty = new Artifact_RobeofStratos(); break;
						case 2: Arty.Delete(); Arty = new Artifact_MantleofStratos(); break;
						case 3: Arty.Delete(); Arty = new Artifact_StratosManual(); break;
					}
					AddItem( Arty );
				}
			}
			return base.OnBeforeDeath();
		}

		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override int TreasureMapLevel{ get{ return 6; } }
		public override bool BardImmune { get { return true; } }

		public TitanStratos( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}