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
	public class TitanPyros : BaseCreature
	{
		public override int BreathPhysicalDamage{ get{ return 0; } }
		public override int BreathFireDamage{ get{ return 100; } }
		public override int BreathColdDamage{ get{ return 0; } }
		public override int BreathPoisonDamage{ get{ return 0; } }
		public override int BreathEnergyDamage{ get{ return 0; } }
		public override int BreathEffectHue{ get{ return (0xB71-1); } }
		public override int BreathEffectItemID{ get{ return 0; } }
		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool HasBreath{ get{ return true; } }
		public override double BreathEffectDelay{ get{ return 0.1; } }
		public override void BreathDealDamage( Mobile target, int form ){ base.BreathDealDamage( target, 42 ); }

		[Constructable]
		public TitanPyros () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Pyros";
			Title = "the titan of fire";
			Body = 461;
			BaseSoundID = 838;
			NameHue = 0x22;

			SetStr( 986, 1185 );
			SetDex( 177, 255 );
			SetInt( 151, 250 );

			SetHits( 592, 711 );

			SetDamage( 22, 29 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Fire, 25 );
			SetDamageType( ResistanceType.Energy, 25 );

			SetResistance( ResistanceType.Physical, 65, 80 );
			SetResistance( ResistanceType.Fire, 60, 80 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.Anatomy, 25.1, 50.0 );
			SetSkill( SkillName.Psychology, 90.1, 100.0 );
			SetSkill( SkillName.Magery, 95.5, 100.0 );
			SetSkill( SkillName.Meditation, 25.1, 50.0 );
			SetSkill( SkillName.MagicResist, 100.5, 150.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.FistFighting, 90.1, 100.0 );

			Fame = 24000;
			Karma = -24000;

			VirtualArmor = 90;

			AddItem( new LighterSource() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override bool OnBeforeDeath()
		{
			int CanDie = 0;
			int CanKillIt = 0;
			Mobile winner = this;
			int RewardColor = 0x779;

			foreach ( Mobile m in this.GetMobilesInRange( 30 ) )
			{
				if ( m is PlayerMobile && m.Map == this.Map && !m.Blessed )
				{
					Item obelisk = m.Backpack.FindItemByType( typeof ( ObeliskTip ) );
					if ( obelisk != null )
					{
						ObeliskTip tip = (ObeliskTip)obelisk;
						if ( tip.ObeliskOwner == m && tip.HasFire > 0 && tip.WonFire < 1 )
						{
							CanDie = 1;
							winner = m;
							tip.WonFire = 1;
							m.SendMessage( "You absord the Titan's power into the Tongue of Flame." );
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
							if ( tip.ObeliskOwner == m && tip.HasFire > 0 && tip.WonFire > 0 )
							{
								CanKillIt = 1;
							}
						}
					}
				}
			}

			if ( CanDie == 0 && CanKillIt == 0 )
			{
				Say("No! It is your soul I will take!");
				this.Hits = this.HitsMax;
				this.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				this.PlaySound( 0x202 );
				return false;
			}
			else if ( CanKillIt == 0 )
			{
				string Iam = "the Titan of Fire";
				PlayerMobile killer = MobileUtilities.TryGetKillingPlayer( this );
				Server.Misc.LoggingFunctions.LogSlayingLord( killer, Iam );
				if ( winner is PlayerMobile )
				{
					LoggingFunctions.LogGenericQuest( winner, "has obtained the power of the fire titan" );
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
								if ( pmi.Mobile is PlayerMobile && pmi.Mobile.InRange(this.Location, 20) && pmi.Mobile.Map == this.Map && !pmi.Mobile.Blessed && !Server.Misc.PlayerSettings.GetSpecialsKilled( pmi.Mobile, "TitanPyros" ) )
								{
									Server.Misc.PlayerSettings.SetSpecialsKilled( pmi.Mobile, "TitanPyros", true );
									ManualOfItems book = new ManualOfItems();
										book.Hue = RewardColor;
										book.ItemID = 0x1AA3;
										book.Name = "Chest of Fire Titan Relics";
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
										book.m_Extra = "of the Flame";
										book.m_FromWho = "Taken from Pyros";
										book.m_HowGiven = "Acquired by";
										book.m_Points = 300;
										book.m_Hue = RewardColor;
										pmi.Mobile.AddToBackpack( book );

									pmi.Mobile.SendMessage("An item has appeared in your backpack!");
								}
							}
						}
						else if ( !Server.Misc.PlayerSettings.GetSpecialsKilled( winner, "TitanPyros" ) )
						{
							Server.Misc.PlayerSettings.SetSpecialsKilled( winner, "TitanPyros", true );
							ManualOfItems book = new ManualOfItems();
								book.Hue = RewardColor;
								book.ItemID = 0x1AA3;
								book.Name = "Chest of Fire Titan Relics";
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
								book.m_Extra = "of the Flame";
								book.m_FromWho = "Taken from Pyros";
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
					Item Arty = new Artifact_BootsofPyros();
					switch( Utility.RandomMinMax( 0, 3 ) )
					{
						case 1: Arty.Delete(); Arty = new Artifact_MantleofPyros(); break;
						case 2: Arty.Delete(); Arty = new Artifact_RobeofPyros(); break;
						case 3: Arty.Delete(); Arty = new Artifact_PyrosGrimoire(); break;
					}
					AddItem( Arty );
				}
			}
			return base.OnBeforeDeath();
		}

		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override int TreasureMapLevel{ get{ return 6; } }
		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 18; } }
		public override bool BardImmune { get { return true; } }
		public override HideType HideType{ get{ return HideType.Hellish; } }

		public TitanPyros( Serial serial ) : base( serial )
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