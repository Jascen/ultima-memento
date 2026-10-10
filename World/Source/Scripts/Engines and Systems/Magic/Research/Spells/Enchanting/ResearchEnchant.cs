using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Gumps;
using Server.Spells;
using Server.Targeting;
using Server.Network;
using Server.Regions;
using Server.Misc;
using Server.Mobiles;

namespace Server.Spells.Research
{
	public class ResearchEnchant : ResearchSpell
	{
		public override int spellIndex { get { return 35; } }
		public int CirclePower = 5;
		public static int spellID = 35;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3.0 ); } }
		public override double RequiredSkill{ get{ return (double)(Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 8 ))); } }
		public override int RequiredMana{ get{ return Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 7 )); } }

		private static SpellInfo m_Info = new SpellInfo(
				Server.Misc.Research.SpellInformation( spellID, 2 ),
				Server.Misc.Research.CapsCast( Server.Misc.Research.SpellInformation( spellID, 4 ) ),
				266,
				9040,
				Reagent.PixieSkull,Reagent.FairyEgg,Reagent.DragonTooth,Reagent.MoonCrystal
			);

        public ResearchEnchant(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public static void Initialize()
        {
            EventSink.Logout += OnLogout;
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this, spellID);
        }

        private static void OnLogout(LogoutEventArgs e)
        {
            ResearchEnchant.EndEffects(e.Mobile);
        }

        public void Target( object o, int spellID )
        {
			if ( o is BaseWeapon )
			{
				BaseWeapon weapon = (BaseWeapon)o;

				if ( Caster.Backpack.FindItemByType( typeof ( ResearchEnchantStone ) ) != null )
				{
					DoFizzle();
				}
				else if (!Caster.CanSee(weapon))
				{
					Caster.SendLocalizedMessage(500237); // Target can not be seen.
				}
				else if (!Caster.CanBeginAction(typeof(ResearchEnchant)))
				{
					Caster.SendLocalizedMessage(1005559); // This spell is already in effect.
				}
				else if ( !weapon.IsChildOf( Caster.Backpack ) )
				{
					Caster.SendMessage( "The weapon must be in your pack to enchant." );
				}
				else if (CheckSequence())
				{
					ResearchEnchantStone orb = new ResearchEnchantStone();
					Caster.AddToBackpack( orb );
					Server.Misc.Research.ConsumeScroll( Caster, true, spellID, alwaysConsume, Scroll );

					string name = weapon.Name;
					if ( weapon.Name != null && weapon.Name != "" ){ name = weapon.Name; }
					if ( name == null ){ weapon.SyncName(); name = weapon.Name; }

					orb.EnchantOwner = Caster;
					orb.EnchantSerial = weapon.Serial;
					orb.EnchantName = name;
					orb.EnchantDmg = weapon.Attributes.WeaponDamage;
					orb.EnchantHue = weapon.Hue;

					weapon.Name = "" + name + " [enchanted]";
					weapon.Hue = 0x489;
					weapon.Attributes.WeaponDamage += 100;

					Caster.PlaySound( 0x1F7 );
					Caster.FixedParticles( 0x3039, 9, 32, 5008, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0x85E ), 0, EffectLayer.Waist );

					int val = (int)(DamagingSkill( Caster )/2);

					BuffInfo.RemoveBuff( Caster, BuffIcon.EnchantWeapon );
					BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.EnchantWeapon, 1063636, TimeSpan.FromMinutes( val ), Caster ) );

					if (val > 120)
						val = 120;

					new InternalTimer( Caster, TimeSpan.FromMinutes( val ) ).Start();
				}
			}
			else
			{
				Caster.SendMessage( "You can only enchant weapons with this spell." );
			}
            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private ResearchEnchant m_Owner;
            private int m_SpellID;

            public InternalTarget( ResearchEnchant owner, int spellID ) : base(12, false, TargetFlags.None)
            {
                m_Owner = owner;
				m_SpellID = spellID;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is BaseWeapon)
                {
                    m_Owner.Target( (BaseWeapon)o, m_SpellID );
                }
                else
                {
                    from.SendMessage("That cannot be enchanted.");
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }

		private class InternalTimer : Timer
		{
			private Mobile m_m;
			private DateTime m_Expire;

			public InternalTimer( Mobile Caster, TimeSpan duration ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 0.1 ) )
			{
				m_m = Caster;
				m_Expire = DateTime.Now + duration;
			}

			protected override void OnTick()
			{
				if ( DateTime.Now >= m_Expire )
				{
					EndEffects( m_m );
					Stop();
				}
			}
		}

		public static void EndEffects( Mobile m )
		{
			int serial = 0;
			string name = "";
			int hue = 0;
			int dmg = 0;

			ArrayList targets = new ArrayList();
			foreach ( Item item in World.Items.Values )
			{
				if ( item is ResearchEnchantStone )
				{
					ResearchEnchantStone orb = (ResearchEnchantStone)item;
					if ( ( orb.EnchantOwner == m && m != null ) || m == null )
					{
						serial = orb.EnchantSerial;
						name = orb.EnchantName;
						hue = orb.EnchantHue;
						dmg = orb.EnchantDmg;
						targets.Add( item );
					}
				}
			}

			for ( int i = 0; i < targets.Count; ++i )
			{
				Item item = ( Item )targets[ i ];
				item.Delete();
			}

			foreach ( Item item in World.Items.Values )
			{
				if ( item.Serial == serial && item is BaseWeapon )
				{
					if ( (item.Name).Contains("[enchanted]") )
					{
						BaseWeapon w = (BaseWeapon)item;
						w.Hue = hue;
						w.Name = name;
						w.Attributes.WeaponDamage = dmg;
					}
				}
			}

			if ( m != null )
			{
				m.PlaySound( 0x1F8 );
				BuffInfo.RemoveBuff( m, BuffIcon.EnchantWeapon );
			}
		}
    }
}

namespace Server.Items
{
	public class ResearchEnchantStone : Item
	{
		[Constructable]
		public ResearchEnchantStone() : base( 0x3199 )
		{
			Weight = 1.0;
			Movable = false;
			Hue = 0x489;
			Name = "Enchantment Power Orb";
			LootType = LootType.Blessed;
		}

		public override bool DisplayLootType{ get{ return false; } }

		public ResearchEnchantStone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 ); // version

			writer.Write( (Mobile)EnchantOwner );
			writer.Write( EnchantSerial );
			writer.Write( EnchantName );
			writer.Write( EnchantDmg );
			writer.Write( EnchantHue );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			EnchantOwner = reader.ReadMobile();
			EnchantSerial = reader.ReadInt();
			EnchantName = reader.ReadString();
			EnchantDmg = reader.ReadInt();
			EnchantHue = reader.ReadInt();

			RunTime thisTimer = new RunTime( this ); 
			thisTimer.Start();
		}

		public class RunTime : Timer 
		{ 
			public RunTime( Item task ) : base( TimeSpan.FromSeconds( 10.0 ) )
			{ 
				Priority = TimerPriority.OneSecond; 
			} 

			protected override void OnTick() 
			{
				Server.Spells.Research.ResearchEnchant.EndEffects( null );
			} 
		}

		public Mobile EnchantOwner;
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Enchant_Owner { get{ return EnchantOwner; } set{ EnchantOwner = value; } }

		public int EnchantSerial;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Enchant_Serial { get{ return EnchantSerial; } set{ EnchantSerial = value; } }

		public string EnchantName;
		[CommandProperty( AccessLevel.GameMaster )]
		public string Enchant_Name { get{ return EnchantName; } set{ EnchantName = value; } }

		public int EnchantDmg;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Enchant_Dmg { get{ return EnchantDmg; } set{ EnchantDmg = value; } }

		public int EnchantHue;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Enchant_Hue { get{ return EnchantHue; } set{ EnchantHue = value; } }
	}
}
