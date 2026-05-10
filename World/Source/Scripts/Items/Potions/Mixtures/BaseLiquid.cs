using Server.Targeting;
using Server.Spells;

namespace Server.Items
{
	public abstract class BaseLiquid : BasePotion
	{
		public int LiquidGlow;

		public override int Hue{ get { return ( Server.Items.PotionKeg.GetPotionColor( this ) ); } }

		public BaseLiquid( PotionEffect p ) : base( 0x2155, p )
		{
		}

		public BaseLiquid( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		    writer.Write( LiquidGlow );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			LiquidGlow = reader.ReadInt();
		}

		public static int GetLiquidBonus( Mobile from )
		{
			return ( 1 + (int)(Server.Items.BasePotion.EnhancePotions( from )/20) + (int)(from.Skills[SkillName.Tasting].Value/20) + (int)(from.Skills[SkillName.Cooking].Value/20) );
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
			else if ( Server.Items.MonsterSplatter.TooMuchSplatter( from ) )
			{
				from.SendMessage( "There is too much liquid on the ground already." ); 
				return;
			}
			else
			{
				from.SendMessage( "Where do you want to dump the liquid?" );
				ThrowTarget targ = from.Target as ThrowTarget;

				if ( targ != null && targ.Potion == this )
					return;

				from.RevealingAction();
				from.Target = new ThrowTarget( this );
			}
		}

		private class ThrowTarget : Target
		{
			private BaseLiquid m_Potion;

			public BaseLiquid Potion
			{
				get{ return m_Potion; }
			}

			public ThrowTarget( BaseLiquid potion ) : base( 12, true, TargetFlags.None )
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
					MonsterSplatter.AddSplatter( p.X, p.Y, p.Z, from.Map, d, from, m_Potion.Name, ( Server.Items.PotionKeg.GetPotionColor( m_Potion ) ), m_Potion.LiquidGlow );
				}

				if ( nThrown > 0 )
				{
					from.RevealingAction();
					m_Potion.Consume();
					from.AddToBackpack( new Jar() );
				}
			}
		}
	}
}