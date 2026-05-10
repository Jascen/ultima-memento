using Server;
using System;
using System.Collections;
using Server.Network;
using Server.Targeting;
using Server.Prompts;
using Server.Misc;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

namespace Server.Items
{
	public class BaseMixture : BasePotion
	{
		public override int Hue{ get { return ( Server.Items.PotionKeg.GetPotionColor( this ) ); } }

		public string SlimeName;
		public int SlimeMagery;
		public int SlimePoisons;
		public int SlimeHue;
		public int SlimePhys;
		public int SlimeCold;
		public int SlimeFire;
		public int SlimePois;
		public int SlimeEngy;
		public int SlimeGlow;
		public int SlimeHate;
		
		[CommandProperty(AccessLevel.Owner)]
		public string Slime_Name { get { return SlimeName; } set { SlimeName = value; InvalidateProperties(); } }
		[CommandProperty(AccessLevel.Owner)]
		public int Slime_Magery { get { return SlimeMagery; } set { SlimeMagery = value; InvalidateProperties(); } }
		[CommandProperty(AccessLevel.Owner)]
		public int Slime_Poisons { get { return SlimePoisons; } set { SlimePoisons = value; InvalidateProperties(); } }
		[CommandProperty(AccessLevel.Owner)]
		public int Slime_Hue { get { return SlimeHue; } set { SlimeHue = value; InvalidateProperties(); } }
		[CommandProperty(AccessLevel.Owner)]
		public int Slime_Phys { get { return SlimePhys; } set { SlimePhys = value; InvalidateProperties(); } }
		[CommandProperty(AccessLevel.Owner)]
		public int Slime_Cold { get { return SlimeCold; } set { SlimeCold = value; InvalidateProperties(); } }
		[CommandProperty(AccessLevel.Owner)]
		public int Slime_Fire { get { return SlimeFire; } set { SlimeFire = value; InvalidateProperties(); } }
		[CommandProperty(AccessLevel.Owner)]
		public int Slime_Pois { get { return SlimePois; } set { SlimePois = value; InvalidateProperties(); } }
		[CommandProperty(AccessLevel.Owner)]
		public int Slime_Engy { get { return SlimeEngy; } set { SlimeEngy = value; InvalidateProperties(); } }
		[CommandProperty(AccessLevel.Owner)]
		public int Slime_Glow { get { return SlimeGlow; } set { SlimeGlow = value; InvalidateProperties(); } }
		[CommandProperty(AccessLevel.Owner)]
		public int Slime_Hate { get { return SlimeHate; } set { SlimeHate = value; InvalidateProperties(); } }

		[Constructable]
		public BaseMixture( PotionEffect p ) : base( 0x1FDB, p )
		{
		}

		public override void Drink( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1060640 ); // The item must be in your backpack to use it.
			}
			else if ( !from.Region.AllowHarmful( from, from ) )
			{
				from.SendMessage( "That doesn't feel like a good idea." ); 
				return;
			}
			else if ( ( from.FollowersMax - from.Followers ) < 1 )
			{
				from.SendMessage("You have too many followers to control the slime.");
				return;
			}
			else
			{
				from.SendMessage( "Where do you want to dump the mixture?" );
				ThrowTarget targ = from.Target as ThrowTarget;

				if ( targ != null && targ.Potion == this )
					return;

				from.RevealingAction();
				from.Target = new ThrowTarget( this );
			}
		}

		private class ThrowTarget : Target
		{
			private BaseMixture m_Potion;

			public BaseMixture Potion
			{
				get{ return m_Potion; }
			}

			public ThrowTarget( BaseMixture potion ) : base( 12, true, TargetFlags.None )
			{
				m_Potion = potion;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Potion.Deleted || m_Potion.Map == Map.Internal )
					return;
					
				IPoint3D p = targeted as IPoint3D;
				Point3D d = new Point3D( p );

				if ( p == null || from.Map == null )
					return;

				SpellHelper.GetSurfaceTop( ref p );

				int nThrown = 1;

				if ( from.GetDistanceToSqrt( d ) > 8 )
				{
					nThrown = 0;
					from.SendMessage( "That is too far away." );
				}
				else if ( !from.CanSee( d ) )
				{
					nThrown = 0;
					from.SendLocalizedMessage( 500237 ); // Target can not be seen.
				}
				else if ( (from.Paralyzed || from.Blessed || from.Frozen || (from.Spell != null && from.Spell.IsCasting)) )
				{
					nThrown = 0;
					from.SendMessage( "You cannot do that yet." );
				}
				else
				{
					Effects.PlaySound(d, from.Map, 0x026);

					bool bonusSlime = from.Skills[SkillName.Cooking].Value >= Utility.RandomMinMax( 1, 200 )
						|| from.Skills[SkillName.Tasting].Value >= Utility.RandomMinMax( 1, 200 );

					int toAdd = bonusSlime ? 2 : 1;
					if ( toAdd > ( ( from.FollowersMax - from.Followers - 1 ) ) )
						toAdd = from.FollowersMax - from.Followers;
					if ( toAdd < 1 ) return;

					Server.Mobiles.AlchemicSlime.MakeSlime( from, d, m_Potion.SlimeMagery, m_Potion.SlimePoisons, m_Potion.SlimeName, m_Potion.SlimeHue, m_Potion.SlimePhys, m_Potion.SlimeCold, m_Potion.SlimeFire, m_Potion.SlimePois, m_Potion.SlimeEngy, m_Potion.SlimeGlow, m_Potion.SlimeHate );
					if ( toAdd > 1 ){ Server.Mobiles.AlchemicSlime.MakeSlime( from, d, m_Potion.SlimeMagery, m_Potion.SlimePoisons, m_Potion.SlimeName, m_Potion.SlimeHue, m_Potion.SlimePhys, m_Potion.SlimeCold, m_Potion.SlimeFire, m_Potion.SlimePois, m_Potion.SlimeEngy, m_Potion.SlimeGlow, m_Potion.SlimeHate ); }
				}

				if ( nThrown > 0 )
				{
					from.RevealingAction();
					m_Potion.Consume();
					from.AddToBackpack( new Jar() );
				}
			}
		}

		public static int Buff( Mobile m, string category )
		{
			int value = 10;

			if ( category == "time" )
			{
				int time = 30; 																// MIN 30
				int bonus = (int)(Server.Items.BasePotion.EnhancePotions( m )/2); 			// MAX 25
				int skill1 = (int)(m.Skills[SkillName.Cooking].Value/2); 					// MAX 62
				int skill2 = (int)(m.Skills[SkillName.Tasting].Value/2); 					// MAX 62

				return time + bonus + skill1 + skill2;
			}

			if ( category == "strength" )
			{
				int buff_default = 15;														// +15 DEFAULT
				int buff_bonus = (int)(Server.Items.BasePotion.EnhancePotions( m ) / 5 );	// +10 MAX
				int buff_skill1 = (int)(m.Skills[SkillName.Cooking].Value / 4); 			// +31 MAX
				int buff_skill2 = (int)(m.Skills[SkillName.Tasting].Value / 3); 			// +41 MAX

				return buff_default + buff_bonus + buff_skill1 + buff_skill2;
			}

			if ( category == "skills" )
			{
				int skill = 40; 															// MIN 40
				int skb_bonus = (int)(Server.Items.BasePotion.EnhancePotions( m )/2);		// MAX 25
				int skb_skill1 = (int)(m.Skills[SkillName.Cooking].Value/2); 				// MAX 62
				int skb_skill2 = (int)(m.Skills[SkillName.Tasting].Value/2); 				// MAX 62

				return skill + skb_bonus + skb_skill1 + skb_skill2;
			}

			if ( category == "damage" )
			{
				int damage = 1; 															// MIN 1
				int dmg_bonus = (int)(Server.Items.BasePotion.EnhancePotions( m )/40); 		// MAX 2
				int dmg_skill1 = (int)(m.Skills[SkillName.Cooking].Value/25); 				// MAX 5
				int dmg_skill2 = (int)(m.Skills[SkillName.Tasting].Value/15); 				// MAX 8

				return damage + dmg_bonus + dmg_skill1 + dmg_skill2;
			}

			if ( category == "poison" )
			{
				int poison = (int)(m.Skills[SkillName.Poisoning].Value/25) + 1; 		// MAX 6

				return poison;
			}


			return value;
		}

		public BaseMixture( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( ( int) 0 ); // version
		    writer.Write( SlimeName );
		    writer.Write( SlimeMagery );
		    writer.Write( SlimePoisons );
		    writer.Write( SlimeHue );
		    writer.Write( SlimePhys );
		    writer.Write( SlimeCold );
		    writer.Write( SlimeFire );
		    writer.Write( SlimePois );
		    writer.Write( SlimeEngy );
		    writer.Write( SlimeGlow );
		    writer.Write( SlimeHate );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			SlimeName = reader.ReadString();
			SlimeMagery = reader.ReadInt();
			SlimePoisons = reader.ReadInt();
			SlimeHue = reader.ReadInt();
			SlimePhys = reader.ReadInt();
			SlimeCold = reader.ReadInt();
			SlimeFire = reader.ReadInt();
			SlimePois = reader.ReadInt();
			SlimeEngy = reader.ReadInt();
			SlimeGlow = reader.ReadInt();
			SlimeHate = reader.ReadInt();
		}
	}
}