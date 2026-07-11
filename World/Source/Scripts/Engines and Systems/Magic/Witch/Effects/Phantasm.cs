using System;
using Server.Targeting;
using Server.Network;
using Server.Items;

namespace Server.Spells.Undead
{
	public class PhantasmSpell : UndeadSpell
	{
		private static SpellInfo m_Info = new SpellInfo( "", "", 239, 9021 );
		public override double RequiredSkill{ get{ return 25.0; } }
		public override int RequiredMana{ get{ return 0; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.0 ); } }

		public PhantasmSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
			Caster.SendMessage( "What trap do you want the spirit to disable?" );
		}

		public void Target( TrapableContainer item )
		{
			if ( !Caster.CanSee( item ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( item.TrapLevel > (int)(Caster.Skills[DamageSkill].Value) )
			{
				base.DoFizzle();
			}
			else if ( item.TrapType == TrapType.None )
			{
				Caster.PrivateOverheadMessage(MessageType.Regular, 0x14C, false, "That does not seem to be trapped.", Caster.NetState);
			}
			else if ( CheckSequence() )
			{
				SpellHelper.Turn( Caster, item );

				Point3D loc = item.GetWorldLocation();

				Effects.SendLocationParticles( EffectItem.Create( loc, item.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 5015 );
				Effects.PlaySound( loc, item.Map, 0x37D );

				item.DisableTrap();
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private PhantasmSpell m_Owner;

			public InternalTarget( PhantasmSpell owner ) : base( 12, false, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is TrapableContainer )
				{
					m_Owner.Target( (TrapableContainer)o );
				}
				else
				{
					from.SendMessage( "The spirit cannot disarm that" );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}