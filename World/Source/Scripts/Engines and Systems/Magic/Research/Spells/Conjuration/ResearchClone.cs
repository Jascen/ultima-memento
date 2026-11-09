using System;
using Server.Targeting;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Spells.Necromancy;

namespace Server.Spells.Research
{
	public class ResearchClone : ResearchSpell
	{
		public int CirclePower = 3;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.5 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 607,
					IconGraphic = 11197,
					Name = "Clone",
					PowerWords = "In Quas Xen",
					Description = "Creates a clone of the caster that will appear and distract your enemies while you remain hidden from view.",
					ManaCost = 25,
					MinSkill = 45,
					TargetType = TargetFlags.None
				},
				230,
				9041,
				Reagent.GargoyleEar,Reagent.FairyEgg
			);

		private static Dictionary<Mobile, int> m_CloneCount = new Dictionary<Mobile, int>();

		public static bool HasClone( Mobile m )
		{
			return m_CloneCount.ContainsKey( m );
		}

		public static void AddClone( Mobile m )
		{
			if ( m == null )
				return;

			if ( m_CloneCount.ContainsKey( m ) )
				m_CloneCount[m]++;
			else
				m_CloneCount[m] = 1;
		}

		public static void RemoveClone( Mobile m )
		{
			if ( m == null )
				return;

			if ( m_CloneCount.ContainsKey( m ) )
			{
				m_CloneCount[m]--;

				if ( m_CloneCount[m] == 0 )
					m_CloneCount.Remove( m );
			}
		}

		public ResearchClone( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override bool CheckCast()
		{
			if ( Caster.Mounted )
			{
				Caster.SendLocalizedMessage( 1063132 ); // You cannot use this ability while mounted.
				return false;
			}
			else if ( (Caster.Followers + 1) > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1063133 ); // You cannot summon a mirror image because you have too many followers.
				return false;
			}
			else if( TransformationSpellHelper.UnderTransformation( Caster, typeof( HorrificBeastSpell ) ) )
			{
				Caster.SendLocalizedMessage( 1061091 ); // You cannot cast that spell in this form.
				return false;
			}

			return base.CheckCast();
		}

		public override void OnBeginCast()
		{
			base.OnBeginCast();

			Caster.SendLocalizedMessage( 1063134 ); // You begin to summon a mirror image of yourself.
		}

		public override void OnCast()
		{
			if ( Caster.Mounted )
			{
				Caster.SendLocalizedMessage( 1063132 ); // You cannot use this ability while mounted.
			}
			else if ( (Caster.Followers + 1) > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1063133 ); // You cannot summon a mirror image because you have too many followers.
			}
			else if( TransformationSpellHelper.UnderTransformation( Caster, typeof( HorrificBeastSpell ) ) )
			{
				Caster.SendLocalizedMessage( 1061091 ); // You cannot cast that spell in this form.
			}
			else if ( CheckSequence() )
			{
				Effects.SendLocationParticles( EffectItem.Create( Caster.Location, Caster.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 0, 0, 5042, 0 );
				Effects.PlaySound( Caster, Caster.Map, 0x201 );
				Caster.Hidden = true;
				new Clone( Caster ).MoveToWorld( Caster.Location, Caster.Map );
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
			}

			FinishSequence();
		}
	}
}