using System;
using System.Collections;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Spells.Herbalist 
{ 
	public class BlendWithForestSpell : HerbalistSpell 
	{ 
		private static SpellInfo m_Info = new SpellInfo( "", "", 239, 9021 );
		public override int HerbalistSpellCircle{ get{ return 4; } }
		public override double CastDelay{ get{ return 1.0; } } 
		public override double RequiredSkill{ get{ return 50.0; } } 
		public override int RequiredMana{ get{ return 0; } } 
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }

		private static readonly Hashtable m_Table = new Hashtable();

		public BlendWithForestSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public static bool HasEffect( Mobile m )
		{
			return m_Table[m] != null;
		}

		public static void RemoveEffect( Mobile m )
		{
			InternalItem item = (InternalItem)m_Table[m];

			if ( item == null )
				return;

			m_Table.Remove( m );

			m.Squelched = item.WasSquelched;
			m.Hidden = false;
			BuffInfo.RemoveBuff( m, BuffIcon.BlendWithForest );

			item.Delete();
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				// Carry over the original state when recast, otherwise our own mute would be restored
				bool wasSquelched = m.Squelched;
				InternalItem existing = (InternalItem)m_Table[m];
				if ( existing != null )
				{
					wasSquelched = existing.WasSquelched;
					m_Table.Remove( m );
					existing.Delete();
				}

				m.PlaySound( 0x19 );
				m.Paralyze( TimeSpan.FromSeconds( 20.0 ) );
				m.FixedParticles( 0x375A, 2, 10, 5027, 0x3D, 2, EffectLayer.Waist );
				m.Hidden = true;
				m.Squelched = true;

				InternalItem item = new InternalItem( m, wasSquelched );
				item.MoveToWorld( m.Location, m.Map );
				m_Table[m] = item;

				BuffInfo.RemoveBuff( m, BuffIcon.BlendWithForest );
				BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.BlendWithForest, 1063544, TimeSpan.FromSeconds( 20.0 ), m ) );

				foreach ( Mobile pet in World.Mobiles.Values )
				{
					if ( pet is BaseCreature )
					{
						BaseCreature bc = (BaseCreature)pet;
						if ( bc.Controlled && bc.ControlMaster == m )
							pet.Hidden = true;
					}
				}
			}

			FinishSequence();
		}

		private class InternalItem : Item
		{
			private Timer m_Timer;
			private Mobile m_Owner;
			private bool m_WasSquelched;

			public bool WasSquelched{ get{ return m_WasSquelched; } }

			public InternalItem( Mobile owner, bool wasSquelched ) : base( 0xC9E )
			{
				Movable = false;
				m_Owner = owner;
				m_WasSquelched = wasSquelched;

				m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 20.0 ), () => RemoveEffect( m_Owner ) );
			}

			public InternalItem( Serial serial ) : base( serial )
			{
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( (int) 2 ); // version

				writer.Write( m_Owner );
				writer.Write( m_WasSquelched );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();

				if ( version < 2 )
					reader.ReadTimeSpan(); // Remaining duration, no longer used

				m_Owner = reader.ReadMobile();
				m_WasSquelched = reader.ReadBool();

				// The effect doesn't survive a restart, so restore the owner and clean up once loading finishes
				Timer.DelayCall( TimeSpan.Zero, () =>
				{
					if ( m_Owner != null )
					{
						m_Owner.Hidden = false;
						m_Owner.Squelched = m_WasSquelched;
					}

					Delete();
				} );
			}

			public override void OnAfterDelete()
			{
				base.OnAfterDelete();

				if ( m_Timer != null )
					m_Timer.Stop();

				// Deleted by something other than the spell (e.g. staff), so the effect should still end
				if ( m_Owner != null && m_Table[m_Owner] == this )
					RemoveEffect( m_Owner );
			}
		}

		private class InternalTarget : Target
		{
			private BlendWithForestSpell m_Owner;

			public InternalTarget( BlendWithForestSpell owner ) : base( 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}