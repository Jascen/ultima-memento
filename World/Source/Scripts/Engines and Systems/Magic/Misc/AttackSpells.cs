using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Items;

namespace Server.Spells.Magical
{
	public class AttackSpells : MagicalSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"", "",
				203,
				9041
			);

		public override SpellCircle Circle { get { return SpellCircle.Eighth; } }
		public override double RequiredSkill{ get{ return 0.0; } }
		public override int RequiredMana{ get{ return 4; } }
		public override SkillName CastSkill{ get{ return SkillName.Magery; } }
		public override SkillName DamageSkill{ get{ return SkillName.Magery; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }

		public AttackSpells( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return true; } }

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
			}
			else if ( !m.Alive )
			{
			}
			else if ( Caster.CanBeHarmful( m ) )
			{
				Caster.DoHarmful( m );

				Mobile source = Caster;

				SpellHelper.Turn( source, m );

				int circle = 1;
				bool poisoned = false;
				int magery = (int)( Caster.Skills[SkillName.Magery].Base );
				int necros = (int)( Caster.Skills[SkillName.Necromancy].Base );
					if ( necros > magery ){ magery = necros; }

				if ( magery >= 80 && Caster.Mana >= 50 ){ 				circle = 8; }
				else if ( magery >= 66 && Caster.Mana >= 40 ){ 			circle = 7; }
				else if ( magery >= 52 && Caster.Mana >= 20 ){ 			circle = 6; }
				else if ( magery >= 38 && Caster.Mana >= 14 ){ 			circle = 5; }
				else if ( magery >= 24 && Caster.Mana >= 11 ){ 			circle = 4; }
				else if ( magery >= 10 && Caster.Mana >= 9 ){ 			circle = 3; }
				else if ( Caster.Mana >= 6 && Utility.RandomBool() ){ 	circle = 2; }

				circle = Utility.RandomMinMax( 1, circle );

				if ( circle == 8 ){ 		Caster.Mana = Caster.Mana - 46; }
				else if ( circle == 7 ){ 	Caster.Mana = Caster.Mana - 36; }
				else if ( circle == 6 ){ 	Caster.Mana = Caster.Mana - 16; }
				else if ( circle == 5 ){ 	Caster.Mana = Caster.Mana - 10; }
				else if ( circle == 4 ){ 	Caster.Mana = Caster.Mana - 7; }
				else if ( circle == 3 ){ 	Caster.Mana = Caster.Mana - 5; }
				else { 						Caster.Mana = Caster.Mana - 2; }

				int dmg = (int)( magery / 2 );

				if ( circle == 2 && dmg > 17 ){ dmg = 17; }
				else if ( circle == 3 && dmg > 24 ){ dmg = 24; }
				else if ( circle == 4 && dmg > 31 ){ dmg = 31; }
				else if ( circle == 5 && dmg > 38 ){ dmg = 38; }
				else if ( circle == 6 && dmg > 45 ){ dmg = 45; }
				else if ( circle == 7 && dmg > 52 ){ dmg = 52; }
				else if ( circle == 8 && dmg > 59 ){ dmg = 59; }
				else { dmg = 10; }

				if ( SpellHelper.ResolveMagicDefense( circle, ref source, ref m ) )
				{
					dmg = GetNewAosDamage( dmg, 1, 5, m );

					if ( m.CheckSkill( SkillName.MagicResist, 0, 125 ) )
					{
						dmg = (int)( dmg / 2 );
						m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
					}

					if ( m is BaseCreature && Utility.RandomBool() && ((BaseCreature)m).GetMaster() != null )
						dmg = dmg * 2;

					int phy = 0;
					int fir = 0;
					int cld = 0;
					int psn = 0;
					int egy = 0;

					int spells = Utility.RandomMinMax( 1, 62 );
					int wizardry = Wizardry( Caster );

	/* air */		if ( wizardry == 1 ){ spells = Utility.RandomList( 1, 3, 5, 8, 9, 50 ); }
	/* cold */		else if ( wizardry == 3 ){ spells = Utility.RandomList( 27, 28, 29, 30, 31, 32, 33, 62, 61 ); }
	/* fire */		else if ( wizardry == 4 ){ spells = Utility.RandomList( 7, 13, 17, 18, 20, 22, 23, 24, 25, 26, 56 ); }
	/* main */		else if ( wizardry == 5 ){ spells = Utility.RandomList( 2, 4, 5, 6, 12, 11, 14, 15, 25, 45 ); }
	/* nature */	else if ( wizardry == 6 ){ spells = Utility.RandomList( 11, 19, 34, 41, 44, 46, 47, 48, 49, 50, 51, 53, 57, 59 ); }
	/* necro */		else if ( wizardry == 7 ){ spells = Utility.RandomList( 6, 10, 15, 21, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 55, 58 ); }
	/* storm */		else if ( wizardry == 8 ){ spells = Utility.RandomList( 3, 5, 8, 9, 16, 50 ); }
	/* water */		else if ( wizardry == 9 ){ spells = Utility.RandomList( 52, 52, 52, 52, 52, 52, 3, 5, 8, 9, 16, 50, 54, 60 ); }

					if ( spells == 1 ) // magic arrow
					{
						source.MovingParticles( m, 0x36E4, 5, 0, false, false, 0, 0, 3600, 0, 0, 0 );
						source.PlaySound( 0x1E5 );
						phy = 100;
					}
					else if ( spells == 2 ) // harm
					{
						m.FixedParticles( 0x374A, 10, 30, 5013, 0, 2, EffectLayer.Waist );
						m.PlaySound( 0x0FC );
						phy = 20;
						fir = 20;
						cld = 20;
						psn = 20;
						egy = 20;
					}
					else if ( spells == 3 ) // lightning
					{
						m.FixedParticles( 0x2A4E, 10, 15, 5038, Utility.RandomList( 0, 0x48D, 0x48E, 0x48F, 0x490, 0x491 ), 2, EffectLayer.Head );
						//Effects.SendLocationEffect( m.Location, m.Map, 0x2A4E, 30, 10, Utility.RandomList( 0, 0x48D, 0x48E, 0x48F, 0x490, 0x491 ), 0 );
						m.PlaySound( 0x029 );
						egy = 100;
					}
					else if ( spells == 4 ) // mind blast
					{
						m.FixedParticles( 0x374A, 10, 15, 5038, 1181, 2, EffectLayer.Head );
						m.PlaySound( 0x213 );
						phy = 50;
						egy = 50;
					}
					else if ( spells == 5 ) // energy bolt
					{
						source.MovingParticles( m, 0x3818, 7, 0, false, true, 0, 0, 3043, 4043, 0x211, 0 );
						m.PlaySound( 0x20A );
						egy = 100;
					}
					else if ( spells == 6 ) // web
					{
						source.MovingParticles( m, 0x10D3, 7, 0, false, false, 0, 0, 0 );
						m.PlaySound( 0x62D );
						double webbed = ((double)(Caster.Fame/200));
							if ( webbed > 15.0 ){ webbed = 15.0; }
						m.Paralyze( TimeSpan.FromSeconds( webbed ) );
						phy = 100;
					}
					else if ( spells == 7 ) // radiation
					{
						m.FixedParticles( 0x3400, 10, 30, 5013, 0xB96, 2, EffectLayer.Waist );
						//Effects.SendLocationEffect( m.Location, m.Map, 0x3400, 60, 0xB96, 0 );
						m.PlaySound( 0x108 );
						egy = 100;
					}
					else if ( spells == 8 ) // electricity
					{
						if ( Utility.RandomBool() )
						{
							m.FixedParticles( Utility.RandomList( 0x3967, 0x3979 ), 10, 30, 5013, 0, 2, EffectLayer.Waist );
							//Effects.SendLocationEffect( m.Location, m.Map, Utility.RandomList( 0x3967, 0x3979 ), 30, 10 );
							m.PlaySound( 0x5C3 );
						}
						else
						{
							m.FixedParticles( 0x5547, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
							m.PlaySound( 0x665 );
						}
						egy = 100;
					}
					else if ( spells == 9 ) // electrical storm
					{
						m.FixedParticles( Utility.RandomList( 0x3967, 0x3979 ), 10, 30, 5013, 0, 2, EffectLayer.Head );
						//Effects.SendLocationEffect( m.Location, m.Map, Utility.RandomList( 0x3967, 0x3979 ), 30, 10 );
						m.PlaySound( 0x5C3 );
						m.BoltEffect( 0 );
						egy = 100;
					}
					else if ( spells == 10 ) // dark void
					{
						m.FixedParticles( 0x3400, 10, 30, 5052, Utility.RandomList( 0x496, 0x844, 0x9C1 ), 0, EffectLayer.Head );
						//Effects.SendLocationEffect( m.Location, m.Map, 0x3400, 60, Utility.RandomList( 0x496, 0x844, 0x9C1 ), 0 );
						m.PlaySound( 0x108 );

						int drain = ((int)(Caster.Fame/500));

						m.Mana = m.Mana - drain;
							if ( m.Mana < 0 ){ m.Mana = 0; }

						m.Stam = m.Stam - drain;
							if ( m.Stam < 0 ){ m.Stam = 0; }

						m.SendMessage( "You feel your soul draining!" );
						phy = 20;
						fir = 20;
						cld = 20;
						psn = 20;
						egy = 20;
					}
					else if ( spells == 11 ) // acid
					{
						m.FixedParticles( 0x1A84, 10, 30, 5052, 0x48E, 0, EffectLayer.Head );
						//Effects.SendLocationEffect( m.Location, m.Map, 0x1A84, 30, 10, 0x48F, 1167 );
						m.PlaySound( 0x026 );
						phy = 20;
						psn = 60;
						egy = 20;
					}
					else if ( spells == 12 ) // magical sparkles
					{
						int sparks = Utility.RandomMinMax(1,4);

						if ( sparks == 1 )
						{
							m.FixedParticles( 0x3039, 10, 30, 5052, Utility.RandomList( 0, 0x48D, 0x48E, 0x48F, 0x490, 0x491 ), 0, EffectLayer.Head );
						}
						else if ( sparks == 2 )
						{
							m.FixedParticles( 0x5469, 10, 30, 5052, Utility.RandomList( 0, 0x48D, 0x48E, 0x48F, 0x490, 0x491 ), 0, EffectLayer.Head );
						}
						else if ( sparks == 3 )
						{
							m.FixedParticles( 0x3F29, 10, 30, 5052, Utility.RandomList( 0, 0x48D, 0x48E, 0x48F, 0x490, 0x491 ), 0, EffectLayer.LeftFoot );
						}
						else
						{
							m.FixedParticles( 0x54E1, 10, 30, 5052, Utility.RandomList( 0, 0x48D, 0x48E, 0x48F, 0x490, 0x491 ), 0, EffectLayer.Head );
						}
						m.PlaySound( Utility.RandomList( 0x1DF, 0x1E2, 0x1E8, 0x1ED, 0x1F1, 0x1F7, 0x1FD, 0x203, 0x209, 0x20B, 0x5BC, 0x5C4, 0x5C5, 0x5C9 ) );
						phy = 20;
						fir = 20;
						cld = 20;
						psn = 20;
						egy = 20;
					}
					else if ( spells == 13 ) // fire tornado
					{
						m.FixedParticles( 0x3F29, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
						m.PlaySound( 0x345 );
						fir = 100;
					}
					else if ( spells == 14 ) // magic tentacles
					{
						m.FixedParticles( 0x5475, 10, 30, 5052, Utility.RandomList( 0, 0x48D, 0x48E, 0x48F, 0x490, 0x491 ), 0, EffectLayer.LeftFoot );
						m.PlaySound( Utility.RandomList( 0x1DF, 0x1E2, 0x1E8, 0x1ED, 0x1F1, 0x1F7, 0x1FD, 0x203, 0x209, 0x20B, 0x5BC, 0x5C4, 0x5C5, 0x5C9 ) );
						phy = 20;
						fir = 20;
						cld = 20;
						psn = 20;
						egy = 20;
					}
					else if ( spells == 15 ) // vortex
					{
						m.FixedParticles( 0x5508, 10, 30, 5052, Utility.RandomList( 0, 0x48D, 0x48E, 0x48F, 0x490, 0x491 ), 0, EffectLayer.Head );
						m.PlaySound( 0x665 );
						phy = 50;
						egy = 50;
					}
					else if ( spells == 16 ) // shoot lightning
					{
						source.MovingParticles( m, 0x3818, 5, 0, false, false, 0, 0, 3600, 0, 0, 0 );
						m.PlaySound( 0x211 );
						egy = 100;
					}

					else if ( spells == 17 ) // fire bolt
					{
						source.MovingParticles( m, 0x4D17, 5, 0, false, false, 0, 0, 3600, 0, 0, 0 );
						m.PlaySound( 0x15E );
						fir = 100;
					}
					else if ( spells == 18 ) // fireball
					{
						if ( circle > 5 )
						{
							m.FixedParticles( 0x5562, 10, 30, 5052, 0, 0, EffectLayer.Head );
							m.PlaySound( 0x44B );
						}
						else
						{
							source.MovingParticles( m, 0x36D4, 7, 0, false, true, 0, 0, 9502, 4019, 0x160, 0 );
							m.PlaySound( Core.AOS ? 0x15E : 0x44B );
						}
						fir = 100;
					}
					else if ( spells == 19 ) // devastate
					{
						m.FixedParticles( 0x2A4E, 10, 30, 5052, 0, 0, EffectLayer.Head );
						//Effects.SendLocationEffect( m.Location, m.Map, 0x2A4E, 30, 10, 0, 0 );
						m.PlaySound( 0x029 );
						fir = 100;
					}
					else if ( spells == 20 ) // meteors
					{
						Effects.SendLocationEffect( m.Location, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, 0xB38, 0 );
						if ( circle > 3 )
						{
							Point3D blast2 = new Point3D( ( m.X-1 ), ( m.Y ), m.Z );
							Effects.SendLocationEffect( blast2, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, 0xB38, 0 );
						}
						if ( circle > 5 )
						{
							Point3D blast3 = new Point3D( ( m.X+1 ), ( m.Y ), m.Z );
							Effects.SendLocationEffect( blast3, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, 0xB38, 0 );
						}
						if ( circle > 6 )
						{
							Point3D blast4 = new Point3D( ( m.X ), ( m.Y-1 ), m.Z );
							Effects.SendLocationEffect( blast4, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, 0xB38, 0 );
						}
						if ( circle > 7 )
						{
							Point3D blast5 = new Point3D( ( m.X ), ( m.Y+1 ), m.Z );
							Effects.SendLocationEffect( blast5, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, 0xB38, 0 );
						}
						m.PlaySound( 0x65A );
						phy = 50;
						fir = 50;
					}
					else if ( spells == 21 ) // destruction
					{
						m.FixedParticles( 0x36B0, 10, 30, 5052, 0xAB3, 0, EffectLayer.Head );
						//Effects.SendLocationEffect( m.Location, m.Map, 0x36B0, 60, 0xAB3, 0 );
						m.PlaySound( 0x664 );
						phy = 50;
						fir = 50;
					}
					else if ( spells == 22 ) // flame bolt
					{
						source.MovingParticles( m, 0x3818, 5, 0, false, false, 0xAD2, 0, 3600, 0, 0, 0 );
						m.PlaySound( 0x658 );
						fir = 100;
					}
					else if ( spells == 23 ) // flame strike
					{
						if ( circle > 5 && Utility.RandomBool() )
						{
							m.FixedParticles( 0x551A, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
							m.PlaySound( 0x345 );
						}
						else
						{
							m.FixedParticles( 0x3709, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
							m.PlaySound( 0x208 );
						}
						fir = 100;
					}
					else if ( spells == 24 ) // ignite
					{
						Point3D blast1 = new Point3D( ( m.X ), ( m.Y ), m.Z );
						Effects.SendLocationEffect( blast1, m.Map, 0x3728, 85, 10, 0xB70, 0 );
						if ( circle > 3 )
						{
							Point3D blast2 = new Point3D( ( m.X-1 ), ( m.Y ), m.Z );
							Effects.SendLocationEffect( blast2, m.Map, 0x3728, 85, 10, 0xB70, 0 );
						}
						if ( circle > 5 )
						{
							Point3D blast3 = new Point3D( ( m.X+1 ), ( m.Y ), m.Z );
							Effects.SendLocationEffect( blast3, m.Map, 0x3728, 85, 10, 0xB70, 0 );
						}
						if ( circle > 6 )
						{
							Point3D blast4 = new Point3D( ( m.X ), ( m.Y-1 ), m.Z );
							Effects.SendLocationEffect( blast4, m.Map, 0x3728, 85, 10, 0xB70, 0 );
						}
						if ( circle > 7 )
						{
							Point3D blast5 = new Point3D( ( m.X ), ( m.Y+1 ), m.Z );
							Effects.SendLocationEffect( blast5, m.Map, 0x3728, 85, 10, 0xB70, 0 );
						}
						m.PlaySound( 0x208 );
						fir = 100;
					}
					else if ( spells == 25 ) // explosion
					{
						Point3D blast1 = new Point3D( ( m.X ), ( m.Y ), m.Z );
						Effects.SendLocationEffect( blast1, m.Map, Utility.RandomList( 0x36BD, 0x3822 ), 85, 10, 0, 0 );
						if ( circle > 3 )
						{
							Point3D blast2 = new Point3D( ( m.X-1 ), ( m.Y ), m.Z );
							Effects.SendLocationEffect( blast2, m.Map, Utility.RandomList( 0x36BD, 0x3822 ), 85, 10, 0, 0 );
						}
						if ( circle > 5 )
						{
							Point3D blast3 = new Point3D( ( m.X+1 ), ( m.Y ), m.Z );
							Effects.SendLocationEffect( blast3, m.Map, Utility.RandomList( 0x36BD, 0x3822 ), 85, 10, 0, 0 );
						}
						if ( circle > 6 )
						{
							Point3D blast4 = new Point3D( ( m.X ), ( m.Y-1 ), m.Z );
							Effects.SendLocationEffect( blast4, m.Map, Utility.RandomList( 0x36BD, 0x3822 ), 85, 10, 0, 0 );
						}
						if ( circle > 7 )
						{
							Point3D blast5 = new Point3D( ( m.X ), ( m.Y+1 ), m.Z );
							Effects.SendLocationEffect( blast5, m.Map, Utility.RandomList( 0x36BD, 0x3822 ), 85, 10, 0, 0 );
						}
						m.PlaySound( 0x307 );
						phy = 50;
						fir = 50;
					}
					else if ( spells == 26 ) // steam
					{
						m.FixedParticles( 0x3400, 10, 30, 5052, 0x9C4, 0, EffectLayer.Head );
						//Effects.SendLocationEffect( m.Location, m.Map, 0x3400, 60, 10, 0x9C4, 0 );
						m.PlaySound( 0x108 );
						fir = 100;
					}

					else if ( spells == 27 ) // ice bolt
					{
						source.MovingParticles( m, 0x4D18, 5, 0, false, false, 0, 0, 3600, 0, 0, 0 );
						m.PlaySound( 0x650 );
						cld = 100;
					}
					else if ( spells == 28 ) // icicle
					{
						source.MovingParticles( m, 0x28EF, 5, 0, false, false, 0xB77, 0, 3600, 0, 0, 0 );
						m.PlaySound( 0x1E5 );
						phy = 25;
						cld = 75;
					}
					else if ( spells == 29 ) // hail storm
					{
						m.FixedParticles( Utility.RandomList(0x384E,0x3859), 20, 10, 5044, 0, 0, EffectLayer.Head );
						m.PlaySound( 0x64F );
						phy = 50;
						cld = 50;
					}
					else if ( spells == 30 ) // frost strike
					{
						m.FixedParticles( 0x23B32, 10, 30, 5052, 0x809, 0, EffectLayer.LeftFoot );
						m.PlaySound( 0x64F );
						phy = 25;
						cld = 75;
					}
					else if ( spells == 31 ) // avalanche
					{
						Point3D blast1 = new Point3D( ( m.X ), ( m.Y ), m.Z );
						Effects.SendLocationEffect( blast1, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, 0xB77, 0 );
						if ( circle > 3 )
						{
							Point3D blast2 = new Point3D( ( m.X-1 ), ( m.Y ), m.Z );
							Effects.SendLocationEffect( blast2, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, 0xB77, 0 );
						}
						if ( circle > 5 )
						{
							Point3D blast3 = new Point3D( ( m.X+1 ), ( m.Y ), m.Z );
							Effects.SendLocationEffect( blast3, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, 0xB77, 0 );
						}
						if ( circle > 6 )
						{
							Point3D blast4 = new Point3D( ( m.X ), ( m.Y-1 ), m.Z );
							Effects.SendLocationEffect( blast4, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, 0xB77, 0 );
						}
						if ( circle > 7 )
						{
							Point3D blast5 = new Point3D( ( m.X ), ( m.Y+1 ), m.Z );
							Effects.SendLocationEffect( blast5, m.Map, Utility.RandomList( 0x33E5, 0x33F5 ), 85, 10, 0xB77, 0 );
						}
						m.PlaySound( 0x65A );
						phy = 50;
						cld = 50;
					}
					else if ( spells == 32 ) // snow ball
					{
						source.MovingParticles( m, 0x36E4, 7, 0, false, true, 0xBB3, 0, 9502, 4019, 0x160, 0 );
						m.PlaySound( 0x650 );
						phy = 50;
						cld = 50;
					}
					else if ( spells == 33 ) // cold
					{
						m.FixedParticles( 0x5590, 10, 30, 5052, 0xB77, 0, EffectLayer.Head );
						m.PlaySound( Utility.RandomList(0x10B,0x5590) );
						cld = 100;
					}

					else if ( spells == 34 ) // poison bolt
					{
						source.MovingParticles( m, 0x4F49, 5, 0, false, false, 0, 0, 3600, 0, 0, 0 );
						m.PlaySound( 0x658 );
						cld = 100;
					}
					else if ( spells == 35 ) // physic blast
					{
						m.FixedParticles( 0x3822, 20, 10, 5044, 0xAF1, 0, EffectLayer.Head );
						m.PlaySound( 0x658 );
						phy = 100;
					}
					else if ( spells == 36 ) // evil lightning
					{
						m.FixedParticles( 0x55A6, 20, 10, 5044, EffectLayer.Head );
						m.PlaySound( 0x653 );
						egy = 100;
					}
					else if ( spells == 37 ) // strike
					{
						m.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
						m.PlaySound( 0x307 );
						phy = 50;
						fir = 25;
						egy = 25;
					}
					else if ( spells == 38 ) // mind rot
					{
						m.PlaySound( 0x1FB );
						m.PlaySound( 0x258 );
						m.FixedParticles( 0x373A, 1, 17, 9903, 15, 4, EffectLayer.Head );
						phy = 50;
						psn = 50;
					}
					else if ( spells == 39 ) // pain spike
					{
						m.FixedParticles( 0x37C4, 1, 8, 9916, 39, 3, EffectLayer.Head );
						m.FixedParticles( 0x37C4, 1, 8, 9502, 39, 4, EffectLayer.Head );
						m.PlaySound( 0x210 );
						phy = 50;
						psn = 50;
					}
					else if ( spells == 40 ) // strangle
					{
						m.PlaySound( 0x22F );
						m.FixedParticles( 0x36CB, 1, 9, 9911, 67, 5, EffectLayer.Head );
						m.FixedParticles( 0x374A, 1, 17, 9502, 1108, 4, (EffectLayer)255 );
						phy = 75;
						psn = 25;
					}
					else if ( spells == 41 ) // wither
					{
						Effects.PlaySound( m.Location, m.Map, 0x1FB );
						m.PlaySound( 0x10B );
						m.FixedParticles( 0x37CC, 1, 9, 9911, 0x916, 5, EffectLayer.Waist );
						phy = 100;
					}
					else if ( spells == 42 ) // poison strike
					{
						m.FixedParticles( 0x36B0, 1, 9, 9911, 9915, 5, EffectLayer.Waist );
						m.PlaySound( 0x229 );
						psn = 100;
						poisoned = true;
					}
					else if ( spells == 43 ) // poison
					{
						m.FixedParticles( 0x374A, 10, 15, 5021, 0, 0, EffectLayer.Waist );
						m.PlaySound( 0x205 );
						psn = 100;
						poisoned = true;
					}
					else if ( spells == 44 ) // poison
					{
						m.FixedParticles( 0x3400, 10, 30, 5052, 0, 0, EffectLayer.Waist );
						m.PlaySound( 0x108 );
						psn = 100;
						poisoned = true;
					}
					else if ( spells == 45 ) // poison
					{
						m.FixedParticles( 0x36B0, 10, 30, 5052, 9915, 0, EffectLayer.Waist );
						m.PlaySound( 0x229 );
						psn = 100;
						poisoned = true;
					}
					else if ( spells == 46 ) // venom vine
					{
						m.FixedParticles( 0x5475, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
						m.PlaySound( 0x64F );
						phy = 50;
						psn = 50;
						poisoned = true;
					}
					else if ( spells == 47 ) // vines
					{
						m.FixedParticles( 0x5487, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
						m.PlaySound( 0x64F );
						phy = 100;
					}
					else if ( spells == 48 ) // leaves
					{
						m.FixedParticles( 0x54F4, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
						m.PlaySound( 0x10B );

						if ( m is PlayerMobile && Utility.RandomBool() )
						{
							IMount mount = m.Mount;

							if ( mount != null )
							{
								m.SendLocalizedMessage( 1062315 ); // You fall off your mount!
								Server.Mobiles.EtherealMount.EthyDismount( m );
								mount.Rider = null;
							}
							m.Animate( 22, 5, 1, true, false, 0 );
						}
						phy = 100;
					}
					else if ( spells == 49 ) // magical
					{
						m.FixedParticles( 0x3039, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
						m.PlaySound( Utility.RandomList( 0x1DF, 0x1E2, 0x1E8, 0x1ED, 0x1F1, 0x1F7, 0x1FD, 0x203, 0x209, 0x20B, 0x5BC, 0x5C4, 0x5C5, 0x5C9 ) );
						phy = 20;
						fir = 20;
						cld = 20;
						psn = 20;
						egy = 20;
					}
					else if ( spells == 50 ) // air
					{
						if ( circle > 5 )
						{
							m.FixedParticles( 0x5492, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
							//Effects.SendLocationEffect( m.Location, m.Map, 0x5492, 30, 10, 0, 0 );
						}
						else
						{
							m.FixedParticles( 0x5590, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
							//Effects.SendLocationEffect( m.Location, m.Map, 0x5590, 30, 10, 0, 0 );
						}
						m.PlaySound( Utility.RandomList(0x10B,0x5590) );
						if ( m is PlayerMobile && Utility.RandomBool() )
						{
							IMount mount = m.Mount;

							if ( mount != null )
							{
								m.SendLocalizedMessage( 1062315 ); // You fall off your mount!
								Server.Mobiles.EtherealMount.EthyDismount( m );
								mount.Rider = null;
							}
							m.Animate( 22, 5, 1, true, false, 0 );
						}
						phy = 100;
					}
					else if ( spells == 51 ) // stone hands
					{
						m.FixedParticles( 0x3837, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
						//Effects.SendLocationEffect( m.Location, m.Map, 0x3837, 23, 10, 0, 0 );
						m.PlaySound( 0x65A );
						phy = 50;
					}
					else if ( spells == 52 ) // water
					{
						if ( Utility.RandomBool() )
						{
							m.FixedParticles( 0x1A84, 10, 30, 5052, 0xB3D, 0, EffectLayer.Waist );
							m.PlaySound( 0x026 );
						}
						else
						{
							m.FixedParticles( 0x5558, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
							m.PlaySound( 0x026 );
						}
						phy = 75;
						cld = 25;
					}
					else if ( spells == 53 ) // weed
					{
						m.FixedParticles( 0x3400, 10, 30, 5052, 0xB97, 0, EffectLayer.LeftFoot );
						//Effects.SendLocationEffect( m.Location, m.Map, 0x3400, 60, 0xB97, 0 );
						m.PlaySound( 0x64F );
						double weed = ((double)(Caster.Fame/200));
							if ( weed > 15.0 ){ weed = 15.0; }
						m.Paralyze( TimeSpan.FromSeconds( weed ) );
						phy = 75;
						psn = 25;
					}
					else if ( spells == 54 ) // water globe
					{
						if ( Utility.RandomBool() )
						{
							m.FixedParticles( 0x37E5, 10, 30, 5052, 0, 0, EffectLayer.Head );
							//Effects.SendLocationEffect( m.Location, m.Map, 0x37E5, 85, 10, 0, 0 );
							m.PlaySound( 0x5BF );
						}
						else
						{
							m.FixedParticles( 0x559A, 10, 30, 5052, 0, 0, EffectLayer.Head );
							//Point3D blast = new Point3D( ( m.X ), ( m.Y ), m.Z+12 );
							//Effects.SendLocationEffect( blast, m.Map, 0x559A, 85, 10, 0, 0 );
							m.PlaySound( 0x56D );
						}
						phy = 50;
						cld = 50;
					}
					else if ( spells == 55 ) // poison field
					{
						m.FixedParticles( Utility.RandomList(0x3915,0x3924), 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
						//Effects.SendLocationEffect( m.Location, m.Map, Utility.RandomList(0x3915,0x3924), 85, 10, 0, 0 );
						m.PlaySound( 0x5BC );
						psn = 100;
						poisoned = true;
					}
					else if ( spells == 56 ) // fire field
					{
						if ( Utility.RandomBool() )
						{
							m.FixedParticles( Utility.RandomList(0x3998,0x398D), 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
							m.PlaySound( 0x356 );
						}
						else
						{
							m.FixedParticles( 0x55B1, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
							m.PlaySound( 0x5CF );
						}
						fir = 100;
					}
					else if ( spells == 57 ) // bird wings
					{
						m.FixedParticles( 0x3FE5, 10, 30, 5052, 0xB60, 0, EffectLayer.Head );
						//Point3D blast = new Point3D( ( m.X ), ( m.Y ), m.Z+15 );
						//Effects.SendLocationEffect( blast, m.Map, 0x3FE5, 85, 10, 0xB60, 0 );
						m.PlaySound( 0x64D );
						phy = 100;
					}
					else if ( spells == 58 ) // throwing skull
					{
						source.MovingParticles( m, 0x3FF9, 7, 0, false, true, 0, 0, 9502, 4019, 0x160, 0 );
						source.PlaySound( 0x658 );

						int drain = ((int)(Caster.Fame/500));

						m.Mana = m.Mana - drain;
							if ( m.Mana < 0 ){ m.Mana = 0; }

						m.Stam = m.Stam - drain;
							if ( m.Stam < 0 ){ m.Stam = 0; }

						m.SendMessage( "You feel your soul draining!" );
						phy = 20;
						fir = 20;
						cld = 20;
						psn = 20;
						egy = 20;
					}
					else if ( spells == 59 ) // insects
					{
						m.FixedParticles( 0x554F, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
						//Effects.SendLocationEffect( m.Location, m.Map, 0x554F, 85, 10, 0, 0 );
						m.PlaySound( Utility.RandomList(0x5CC,0x5CB) );
						phy = 50;
						psn = 50;
						if ( Utility.RandomBool() ){ poisoned = true; }
					}
					else if ( spells == 60 ) // water splash
					{
						if ( Utility.RandomBool() )
						{
							m.FixedParticles( 0x5536, 10, 30, 5052, 0, 0, EffectLayer.Head );
							m.PlaySound( 0x5CA );
						}
						else
						{
							m.FixedParticles( 0x23B2, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
							m.PlaySound( 0x026 );
						}
						if ( m is PlayerMobile && Utility.RandomBool() )
						{
							IMount mount = m.Mount;

							if ( mount != null )
							{
								m.SendLocalizedMessage( 1062315 ); // You fall off your mount!
								Server.Mobiles.EtherealMount.EthyDismount( m );
								mount.Rider = null;
							}
							m.Animate( 22, 5, 1, true, false, 0 );
						}
						phy = 50;
						cld = 50;
					}
					else if ( spells == 61 ) // ice storm
					{
						if ( circle < 6 )
						{
							m.FixedParticles( Utility.RandomList(0x384E,0x3859), 10, 30, 5052, 0xB79, 0, EffectLayer.LeftFoot );
						}
						else
						{
							m.FixedParticles( 0x55BB, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
						}
						m.PlaySound( 0x5CE );
						phy = 50;
						cld = 50;
					}
					else if ( spells == 62 ) // ice spike
					{
						m.FixedParticles( 0x5571, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
						m.PlaySound( 0x65D );
						phy = 50;
						cld = 50;
					}

					/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

					if ( poisoned )
					{
						switch( circle )
						{
							case 4: m.ApplyPoison( m, Poison.Lesser );	break;
							case 5: m.ApplyPoison( m, Poison.Regular );	break;
							case 6: m.ApplyPoison( m, Poison.Greater );	break;
							case 7: m.ApplyPoison( m, Poison.Deadly );	break;
							case 8: m.ApplyPoison( m, Poison.Lethal );	break;
						}
					}

					SpellHelper.Damage( this, m, dmg, phy, fir, cld, psn, egy );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private AttackSpells m_Owner;

			public InternalTarget( AttackSpells owner ) : base( 10, false, TargetFlags.Harmful )
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

		public static int Wizardry( Mobile m )
		{
			int wizardry = 2;

			/*
			air		1
			any		2
			cold	3
			fire	4
			main	5
			nature	6
			necro	7
			storm	8
			water	9
			*/

			if ( Server.Misc.Worlds.IsCrypt( m.Location, m.Map ) && m is BaseCreature && ((BaseCreature)m).GetMaster() == null ){ wizardry = 7; }
			else if ( Server.Misc.Worlds.IsSeaDungeon( m.Location, m.Map ) && m is BaseCreature && ((BaseCreature)m).GetMaster() == null ){ wizardry = Utility.RandomList( 8, 9 ); }
			else if ( Server.Misc.Worlds.IsFireDungeon( m.Location, m.Map ) && m is BaseCreature && ((BaseCreature)m).GetMaster() == null ){ wizardry = 4; }
			else if ( Server.Misc.Worlds.IsIceDungeon( m.Location, m.Map ) && m is BaseCreature && ((BaseCreature)m).GetMaster() == null ){ wizardry = 3; }
			else if ( m is AirElemental ){ wizardry = Utility.RandomList( 1 ); }
			else if ( m is ElementalCalledAir ){ wizardry = Utility.RandomList( 1 ); }
			else if ( m is ElementalCalledEarth ){ wizardry = Utility.RandomList( 6 ); }
			else if ( m is Angel ){ wizardry = Utility.RandomList( 1, 6 ); }
			else if ( m is Archangel ){ wizardry = Utility.RandomList( 1, 6 ); }
			else if ( m is Typhoon ){ wizardry = Utility.RandomList( 1, 9 ); }
			else if ( m is AncientSphinx ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is Archmage ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is Beholder ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is BlackCat ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is Devil ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is ElderGazer ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is EvilMageLord ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is EyeOfTheDeep ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is FloatingEye ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is Gazer ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is GhostWizard ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is GolemController ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is HenchmanWizard ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is Hydra ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is Imp ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is Jackalwitch ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is Mangar ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is MindFlayer ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is MutantDaemon ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is OgreMagi ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is PirateCrewMage ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is Psionicist ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is RoyalSphinx ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is RuneBeetle ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is RuneGuardian ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is SaklethMage ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is Seeker ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is SerpentarWizard ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is SerpynSorceress ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is Tarjan ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is TerathanAvenger ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is TerathanMatriarch ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is Vordo ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is WhiteCat ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is WineElemental ){ wizardry = Utility.RandomList( 2 ); }
			else if ( m is CaddelliteDragon ){ wizardry = Utility.RandomList( 3, 4, 5 ); }
			else if ( m is CaddelliteElemental ){ wizardry = Utility.RandomList( 3, 4, 5 ); }
			else if ( m is GarnetElemental ){ wizardry = Utility.RandomList( 3, 5 ); }
			else if ( m is IceElemental ){ wizardry = Utility.RandomList( 3 ); }
			else if ( m is IceGiant ){ wizardry = Utility.RandomList( 3 ); }
			else if ( m is IceSerpent ){ wizardry = Utility.RandomList( 3 ); }
			else if ( m is PrimevalSilverDragon ){ wizardry = Utility.RandomList( 3, 5 ); }
			else if ( m is SapphireElemental ){ wizardry = Utility.RandomList( 3, 5 ); }
			else if ( m is SilverElemental ){ wizardry = Utility.RandomList( 3, 5 ); }
			else if ( m is SpinelElemental ){ wizardry = Utility.RandomList( 3, 5 ); }
			else if ( m is AbysmalDaemon ){ wizardry = Utility.RandomList( 4, 5, 7 ); }
			else if ( m is Archfiend ){ wizardry = Utility.RandomList( 4, 5, 7 ); }
			else if ( m is AshDragon ){ wizardry = Utility.RandomList( 4, 7 ); }
			else if ( m is CinderElemental ){ wizardry = Utility.RandomList( 4 ); }
			else if ( m is Demon ){ wizardry = Utility.RandomList( 4, 5, 7 ); }
			else if ( m is DilithiumElemental ){ wizardry = Utility.RandomList( 4, 5 ); }
			else if ( m is Efreet ){ wizardry = Utility.RandomList( 4 ); }
			else if ( m is FireBat ){ wizardry = Utility.RandomList( 4 ); }
			else if ( m is FireElemental ){ wizardry = Utility.RandomList( 4 ); }
			else if ( m is ElementalCalledFire ){ wizardry = Utility.RandomList( 4 ); }
			else if ( m is FireToad ){ wizardry = Utility.RandomList( 4 ); }
			else if ( m is LavaElemental ){ wizardry = Utility.RandomList( 4 ); }
			else if ( m is LavaGiant ){ wizardry = Utility.RandomList( 4 ); }
			else if ( m is LesserDemon ){ wizardry = Utility.RandomList( 4, 5, 7 ); }
			else if ( m is LowerDemon ){ wizardry = Utility.RandomList( 4, 5, 7 ); }
			else if ( m is MagmaElemental ){ wizardry = Utility.RandomList( 4 ); }
			else if ( m is MeteorElemental ){ wizardry = Utility.RandomList( 4 ); }
			else if ( m is PrimevalAmberDragon ){ wizardry = Utility.RandomList( 4, 5 ); }
			else if ( m is PrimevalDragon ){ wizardry = Utility.RandomList( 4, 5 ); }
			else if ( m is PrimevalFireDragon ){ wizardry = Utility.RandomList( 4, 5 ); }
			else if ( m is PrimevalGreenDragon ){ wizardry = Utility.RandomList( 4, 5 ); }
			else if ( m is PrimevalRedDragon ){ wizardry = Utility.RandomList( 4, 5 ); }
			else if ( m is PrimevalRoyalDragon ){ wizardry = Utility.RandomList( 4, 3, 5, 8 ); }
			else if ( m is PrimevalRunicDragon ){ wizardry = Utility.RandomList( 4, 3, 5, 8 ); }
			else if ( m is PrimevalVolcanicDragon ){ wizardry = Utility.RandomList( 4, 5 ); }
			else if ( m is QuartzElemental ){ wizardry = Utility.RandomList( 4, 5 ); }
			else if ( m is StarRubyElemental ){ wizardry = Utility.RandomList( 4, 5 ); }
			else if ( m is Sunlyte ){ wizardry = Utility.RandomList( 4 ); }
			else if ( m is TopazElemental ){ wizardry = Utility.RandomList( 4, 5 ); }
			else if ( m is Vulcrum ){ wizardry = Utility.RandomList( 4 ); }
			else if ( m is AbrozShaman ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is AbyssGiant ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is AncientEnt ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is AncientEttin ){ wizardry = Utility.RandomList( 5, 6, 4 ); }
			else if ( m is AncientGargoyle ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is AncientLich ){ wizardry = Utility.RandomList( 5, 7, 3, 4 ); }
			else if ( m is AncientWyrm ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is AsianDragon ){ wizardry = Utility.RandomList( 5, 6, 4, 3 ); }
			else if ( m is BlackGateDemon ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is BloodDemigod ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is BloodDemon ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is BoneDemon ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is BoneMagi ){ wizardry = Utility.RandomList( 5, 7, 4, 3 ); }
			else if ( m is SkeletalGargoyle ){ wizardry = Utility.RandomList( 5, 7, 4, 3 ); }
			else if ( m is BottleDragon ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is CodexGargoyleA ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is CodexGargoyleB ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is CrystalDragon ){ wizardry = Utility.RandomList( 5, 4, 3 ); }
			else if ( m is CrystalElemental ){ wizardry = Utility.RandomList( 5, 4, 3 ); }
			else if ( m is Daemonic ){ wizardry = Utility.RandomList( 5, 4, 7 ); }
			else if ( m is DaemonTemplate ){ wizardry = Utility.RandomList( 5, 4, 7 ); }
			else if ( m is DarkReaper ){ wizardry = Utility.RandomList( 5, 6, 7 ); }
			else if ( m is DarkWisp ){ wizardry = Utility.RandomList( 5, 6, 7 ); }
			else if ( m is DeadReaper ){ wizardry = Utility.RandomList( 5, 6, 7 ); }
			else if ( m is DeadWizard ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is DemiLich ){ wizardry = Utility.RandomList( 5, 7, 4 ); }
			else if ( m is DemonicGhost ){ wizardry = Utility.RandomList( 5, 7, 4 ); }
			else if ( m is Dracolich ){ wizardry = Utility.RandomList( 5, 7, 4 ); }
			else if ( m is Dracula ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is DragonGhost ){ wizardry = Utility.RandomList( 5, 7, 4 ); }
			else if ( m is DragonKing ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is DrakkulMage ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is DruidFairy ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is ElderDragon ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is Ent ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is EtherealWarrior ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is EttinShaman ){ wizardry = Utility.RandomList( 5, 6, 4 ); }
			else if ( m is EvilEnt ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is AncientReaper ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is Faerie ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is Fairy ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is Fiend ){ wizardry = Utility.RandomList( 5, 4, 7 ); }
			else if ( m is FireDemon ){ wizardry = Utility.RandomList( 5, 4, 7 ); }
			else if ( m is FireGargoyle ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is FireMephit ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is FireNaga ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is FireSalamander ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is FrostTrollShaman ){ wizardry = Utility.RandomList( 5, 3, 6 ); }
			else if ( m is FungalMage ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is Gargoyle ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is GargoyleAmethyst ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is GargoyleEmerald ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is GargoyleMarble ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is GargoyleOnyx ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is GargoyleRuby ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is GargoyleSapphire ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is GargoyleWarrior ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is GemElemental ){ wizardry = Utility.RandomList( 5, 3, 4, 8 ); }
			else if ( m is GhostGargoyle ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is Ghostly ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is GnomeMage ){ wizardry = Utility.RandomList( 5 ); }
			else if ( m is HarpyElder ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is HarpyHen ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is HillGiantShaman ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is IceDevil ){ wizardry = Utility.RandomList( 5, 3 ); }
			else if ( m is JadeSerpent ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is Server.Mobiles.Jedi ){ wizardry = Utility.RandomList( 5, 6, 3 ); }
			else if ( m is KhumashGor ){ wizardry = Utility.RandomList( 5, 4, 3, 7 ); }
			else if ( m is Kobold ){ wizardry = Utility.RandomList( 5 ); }
			else if ( m is KoboldMage ){ wizardry = Utility.RandomList( 5 ); }
			else if ( m is Lich ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is LichKing ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is LichLord ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is MLDryad ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is MummyGiant ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is NativeWitchDoctor ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is Nazghoul ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is NeptarWizard ){ wizardry = Utility.RandomList( 5, 9 ); }
			else if ( m is OphidianArchmage ){ wizardry = Utility.RandomList( 5, 6, 7 ); }
			else if ( m is OphidianMage ){ wizardry = Utility.RandomList( 5, 6, 7 ); }
			else if ( m is OphidianMatriarch ){ wizardry = Utility.RandomList( 5, 6, 7 ); }
			else if ( m is OrcishMage ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is OrkDemigod ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is Pegasus ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is Phantom ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is Pixie ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is Placeron ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is PoisonCloud ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is PoisonElemental ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is RadiationDragon ){ wizardry = Utility.RandomList( 5, 4, 7 ); }
			else if ( m is RatmanMage ){ wizardry = Utility.RandomList( 5, 4, 3, 7 ); }
			else if ( m is ReanimatedDragon ){ wizardry = Utility.RandomList( 5, 7, 4 ); }
			else if ( m is Reaper ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is ReptalarShaman ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is RevenantLion ){ wizardry = Utility.RandomList( 5 ); }
			else if ( m is SandGiant ){ wizardry = Utility.RandomList( 5, 4, 1 ); }
			else if ( m is Satan ){ wizardry = Utility.RandomList( 5, 7, 4 ); }
			else if ( m is SavageShaman ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is SeaDragon ){ wizardry = Utility.RandomList( 5, 9 ); }
			else if ( m is SeaGhost ){ wizardry = Utility.RandomList( 5, 9, 3 ); }
			else if ( m is SeaGiant ){ wizardry = Utility.RandomList( 5, 9 ); }
			else if ( m is SeaHag ){ wizardry = Utility.RandomList( 5, 9, 7 ); }
			else if ( m is SeaHagGreater ){ wizardry = Utility.RandomList( 5, 9, 7 ); }
			else if ( m is Shade ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is ShadowDemon ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is Shadowlord ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is ShadowRecluse ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is ShadowWisp ){ wizardry = Utility.RandomList( 5, 7, 3 ); }
			else if ( m is ShadowWyrm ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is ShamanicCyclops ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is Shroud ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is SkeletalDragon ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is SkeletalMage ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is SkeletalWizard ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is SkeletonDragon ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is SlasherOfVoid ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is SlimeDevil ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is SoulSucker ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is SpectralGargoyle ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is Spectre ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is Bodak ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is Spectres ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is Spirit ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is Sprite ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is StarGiant ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is StygianGargoyle ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is StygianGargoyleLord ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is Succubus ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is SummonedAirElemental ){ wizardry = Utility.RandomList( 5, 1 ); }
			else if ( m is SummonedAirElementalGreater ){ wizardry = Utility.RandomList( 5, 1 ); }
			else if ( m is SummonedDaemon ){ wizardry = Utility.RandomList( 5, 4, 7 ); }
			else if ( m is SummonedDaemonGreater ){ wizardry = Utility.RandomList( 5, 4, 7 ); }
			else if ( m is SummonedFireElemental ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is SummonedFireElementalGreater ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is SummonedWaterElemental ){ wizardry = Utility.RandomList( 5, 9 ); }
			else if ( m is ElementalCalledWater ){ wizardry = Utility.RandomList( 9 ); }
			else if ( m is SummonedWaterElementalGreater ){ wizardry = Utility.RandomList( 5, 9 ); }
			else if ( m is Surtaz ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is TheAncientTree ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is TitanHydros ){ wizardry = Utility.RandomList( 5, 9 ); }
			else if ( m is TitanLich ){ wizardry = Utility.RandomList( 5, 4, 7, 3 ); }
			else if ( m is TitanPyros ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is TitanStratos ){ wizardry = Utility.RandomList( 5, 1 ); }
			else if ( m is TrilithiumElemental ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is TrollWitchDoctor ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is UndeadDruid ){ wizardry = Utility.RandomList( 5, 6, 7, 3 ); }
			else if ( m is UrcShaman ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is UrkShaman ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is Vampire ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is VampireLord ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is VampirePrince ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is VampireWoods ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is VampiricDragon ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is VoidDragon ){ wizardry = Utility.RandomList( 5, 7, 4 ); }
			else if ( m is VolcanicDragon ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is Wraith ){ wizardry = Utility.RandomList( 5, 7, 4, 3 ); }
			else if ( m is xDryad ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is Xenomutant ){ wizardry = Utility.RandomList( 5, 7 ); }
			else if ( m is Xurtzar ){ wizardry = Utility.RandomList( 5, 7, 4 ); }
			else if ( m is ZombieDragon ){ wizardry = Utility.RandomList( 5, 7, 4, 3 ); }
			else if ( m is ZombieMage ){ wizardry = Utility.RandomList( 5, 7, 4, 3 ); }
			else if ( m is ZornTheBlacksmith ){ wizardry = Utility.RandomList( 5, 4 ); }
			else if ( m is ZuluuWitchDoctor ){ wizardry = Utility.RandomList( 5, 6 ); }
			else if ( m is AntaurKing ){ wizardry = Utility.RandomList( 6 ); }
			else if ( m is AntaurProgenitor ){ wizardry = Utility.RandomList( 6 ); }
			else if ( m is ForestElemental ){ wizardry = Utility.RandomList( 6 ); }
			else if ( m is ForestGiant ){ wizardry = Utility.RandomList( 6 ); }
			else if ( m is JungleGiant ){ wizardry = Utility.RandomList( 6 ); }
			else if ( m is KelpElemental ){ wizardry = Utility.RandomList( 6 ); }
			else if ( m is MysticalFox ){ wizardry = Utility.RandomList( 6 ); }
			else if ( m is WalkingReaper ){ wizardry = Utility.RandomList( 6, 5 ); }
			else if ( m is Wisp ){ wizardry = Utility.RandomList( 6, 5 ); }
			else if ( m is WoodlandDevil ){ wizardry = Utility.RandomList( 6, 5, 7 ); }
			else if ( m is BloodElemental ){ wizardry = Utility.RandomList( 7, 4 ); }
			else if ( m is BloodSpawn ){ wizardry = Utility.RandomList( 7, 4 ); }
			else if ( m is DreadSpider ){ wizardry = Utility.RandomList( 7 ); }
			else if ( m is DriderWizard ){ wizardry = Utility.RandomList( 7, 5 ); }
			else if ( m is GraveDustElemental ){ wizardry = Utility.RandomList( 7 ); }
			else if ( m is Naga ){ wizardry = Utility.RandomList( 7 ); }
			else if ( m is PrimevalAbysmalDragon ){ wizardry = Utility.RandomList( 7, 5 ); }
			else if ( m is PrimevalBlackDragon ){ wizardry = Utility.RandomList( 7, 5 ); }
			else if ( m is PrimevalNightDragon ){ wizardry = Utility.RandomList( 7, 5 ); }
			else if ( m is PrimevalStygianDragon ){ wizardry = Utility.RandomList( 7, 5 ); }
			else if ( m is Server.Mobiles.Syth ){ wizardry = Utility.RandomList( 7 ); }
			else if ( m is ToxicElemental ){ wizardry = Utility.RandomList( 7 ); }
			else if ( m is Wyvra ){ wizardry = Utility.RandomList( 7 ); }
			else if ( m is XormiteElemental ){ wizardry = Utility.RandomList( 7 ); }
			else if ( m is CloudGiant ){ wizardry = Utility.RandomList( 8 ); }
			else if ( m is ElderTitan ){ wizardry = Utility.RandomList( 8 ); }
			else if ( m is ElectricalElemental ){ wizardry = Utility.RandomList( 8 ); }
			else if ( m is ElfPirateCrewMage ){ wizardry = Utility.RandomList( 8, 5 ); }
			else if ( m is Jormungandr ){ wizardry = Utility.RandomList( 8 ); }
			else if ( m is PrimevalSeaDragon ){ wizardry = Utility.RandomList( 8, 5 ); }
			else if ( m is StormGiant ){ wizardry = Utility.RandomList( 8 ); }
			else if ( m is Titan ){ wizardry = Utility.RandomList( 8 ); }
			else if ( m is Calamari ){ wizardry = Utility.RandomList( 9 ); }
			else if ( m is Dagon ){ wizardry = Utility.RandomList( 9 ); }
			else if ( m is DeepSeaDevil ){ wizardry = Utility.RandomList( 9, 5 ); }
			else if ( m is DeepSeaGiant ){ wizardry = Utility.RandomList( 9, 5 ); }
			else if ( m is DeepSeaSerpent ){ wizardry = Utility.RandomList( 9 ); }
			else if ( m is DeepWaterElemental ){ wizardry = Utility.RandomList( 9 ); }
			else if ( m is DemonOfTheSea ){ wizardry = Utility.RandomList( 9, 5, 7 ); }
			else if ( m is Krakoa ){ wizardry = Utility.RandomList( 9 ); }
			else if ( m is Kuthulu ){ wizardry = Utility.RandomList( 9 ); }
			else if ( m is LesserSeaSnake ){ wizardry = Utility.RandomList( 9 ); }
			else if ( m is Leviathan ){ wizardry = Utility.RandomList( 9 ); }
			else if ( m is SeaHorses ){ wizardry = Utility.RandomList( 9 ); }
			else if ( m is SeaSerpent ){ wizardry = Utility.RandomList( 9 ); }
			else if ( m is TritunMage ){ wizardry = Utility.RandomList( 9, 5 ); }
			else if ( m is WaterElemental ){ wizardry = Utility.RandomList( 9 ); }
			else if ( m is WaterNaga ){ wizardry = Utility.RandomList( 9, 5 ); }
			else if ( m is WaterSpawn ){ wizardry = Utility.RandomList( 9 ); }
			else if ( m is WaterWeird ){ wizardry = Utility.RandomList( 9 ); }
			else if ( m is DustElemental ){ wizardry = Utility.RandomList( 1 ); }
			else if ( m is ElementalLordWater ){ wizardry = Utility.RandomList( 9 ); }
			else if ( m is ElementalLordAir ){ wizardry = Utility.RandomList( 1, 8 ); }
			else if ( m is ElementalLordFire ){ wizardry = Utility.RandomList( 4 ); }
			else if ( m is Balron )
			{
				Balron monster = (Balron)m;

				if ( monster.rCategory == "fire" ){ wizardry = Utility.RandomList( 4, 5 ); }
				else if ( monster.rCategory == "poison" ){ wizardry = Utility.RandomList( 7, 5 ); }
				else if ( monster.rCategory == "radiation" ){ wizardry = Utility.RandomList( 7, 5, 4 ); }
				else if ( monster.rCategory == "void" ){ wizardry = Utility.RandomList( 7, 5 ); }
				else if ( monster.rCategory == "cold" ){ wizardry = Utility.RandomList( 3, 5 ); }
				else if ( monster.rCategory == "electrical" ){ wizardry = Utility.RandomList( 8, 5 ); }
				else if ( monster.rCategory == "steam" ){ wizardry = Utility.RandomList( 9, 4, 5 ); }
				else if ( monster.rCategory == "weed" ){ wizardry = Utility.RandomList( 7, 5 ); }
				else if ( monster.rCategory == "wind" ){ wizardry = Utility.RandomList( 1, 5 ); }
				else if ( monster.rCategory == "star" ){ wizardry = Utility.RandomList( 4, 5 ); }
				else if ( monster.rCategory == "sand" ){ wizardry = Utility.RandomList( 1, 5 ); }
				else if ( monster.rCategory == "storm" ){ wizardry = Utility.RandomList( 8, 5 ); }
			}
			else if ( m is Daemon )
			{
				Daemon monster = (Daemon)m;

				if ( monster.rCategory == "fire" ){ wizardry = Utility.RandomList( 4, 5 ); }
				else if ( monster.rCategory == "poison" ){ wizardry = Utility.RandomList( 7, 5 ); }
				else if ( monster.rCategory == "radiation" ){ wizardry = Utility.RandomList( 7, 5, 4 ); }
				else if ( monster.rCategory == "void" ){ wizardry = Utility.RandomList( 7, 5 ); }
				else if ( monster.rCategory == "cold" ){ wizardry = Utility.RandomList( 3, 5 ); }
				else if ( monster.rCategory == "electrical" ){ wizardry = Utility.RandomList( 8, 5 ); }
				else if ( monster.rCategory == "steam" ){ wizardry = Utility.RandomList( 9, 4, 5 ); }
				else if ( monster.rCategory == "weed" ){ wizardry = Utility.RandomList( 7, 5 ); }
				else if ( monster.rCategory == "wind" ){ wizardry = Utility.RandomList( 1, 5 ); }
				else if ( monster.rCategory == "star" ){ wizardry = Utility.RandomList( 4, 5 ); }
				else if ( monster.rCategory == "sand" ){ wizardry = Utility.RandomList( 1, 5 ); }
				else if ( monster.rCategory == "storm" ){ wizardry = Utility.RandomList( 8, 5 ); }
			}
			else if ( m is Dragons )
			{
				Dragons monster = (Dragons)m;

				if ( monster.rCategory == "fire" ){ wizardry = Utility.RandomList( 4, 5 ); }
				else if ( monster.rCategory == "poison" ){ wizardry = Utility.RandomList( 7, 5 ); }
				else if ( monster.rCategory == "radiation" ){ wizardry = Utility.RandomList( 7, 5, 4 ); }
				else if ( monster.rCategory == "void" ){ wizardry = Utility.RandomList( 7, 5 ); }
				else if ( monster.rCategory == "cold" ){ wizardry = Utility.RandomList( 3, 5 ); }
				else if ( monster.rCategory == "electrical" ){ wizardry = Utility.RandomList( 8, 5 ); }
				else if ( monster.rCategory == "steam" ){ wizardry = Utility.RandomList( 9, 4, 5 ); }
				else if ( monster.rCategory == "weed" ){ wizardry = Utility.RandomList( 7, 5 ); }
				else if ( monster.rCategory == "wind" ){ wizardry = Utility.RandomList( 1, 5 ); }
				else if ( monster.rCategory == "star" ){ wizardry = Utility.RandomList( 4, 5 ); }
				else if ( monster.rCategory == "sand" ){ wizardry = Utility.RandomList( 1, 5 ); }
				else if ( monster.rCategory == "storm" ){ wizardry = Utility.RandomList( 8, 5 ); }
			}
			else if ( m is RidingDragon )
			{
				RidingDragon monster = (RidingDragon)m;

				if ( monster.rCategory == "fire" ){ wizardry = Utility.RandomList( 4, 5 ); }
				else if ( monster.rCategory == "poison" ){ wizardry = Utility.RandomList( 7, 5 ); }
				else if ( monster.rCategory == "radiation" ){ wizardry = Utility.RandomList( 7, 5, 4 ); }
				else if ( monster.rCategory == "void" ){ wizardry = Utility.RandomList( 7, 5 ); }
				else if ( monster.rCategory == "cold" ){ wizardry = Utility.RandomList( 3, 5 ); }
				else if ( monster.rCategory == "electrical" ){ wizardry = Utility.RandomList( 8, 5 ); }
				else if ( monster.rCategory == "steam" ){ wizardry = Utility.RandomList( 9, 4, 5 ); }
				else if ( monster.rCategory == "weed" ){ wizardry = Utility.RandomList( 7, 5 ); }
				else if ( monster.rCategory == "wind" ){ wizardry = Utility.RandomList( 1, 5 ); }
				else if ( monster.rCategory == "star" ){ wizardry = Utility.RandomList( 4, 5 ); }
				else if ( monster.rCategory == "sand" ){ wizardry = Utility.RandomList( 1, 5 ); }
				else if ( monster.rCategory == "storm" ){ wizardry = Utility.RandomList( 8, 5 ); }
			}
			else if ( m is Wyrms )
			{
				Wyrms monster = (Wyrms)m;

				if ( monster.rCategory == "fire" ){ wizardry = Utility.RandomList( 4, 5 ); }
				else if ( monster.rCategory == "poison" ){ wizardry = Utility.RandomList( 7, 5 ); }
				else if ( monster.rCategory == "radiation" ){ wizardry = Utility.RandomList( 7, 5, 4 ); }
				else if ( monster.rCategory == "void" ){ wizardry = Utility.RandomList( 7, 5 ); }
				else if ( monster.rCategory == "cold" ){ wizardry = Utility.RandomList( 3, 5 ); }
				else if ( monster.rCategory == "electrical" ){ wizardry = Utility.RandomList( 8, 5 ); }
				else if ( monster.rCategory == "steam" ){ wizardry = Utility.RandomList( 9, 4, 5 ); }
				else if ( monster.rCategory == "weed" ){ wizardry = Utility.RandomList( 7, 5 ); }
				else if ( monster.rCategory == "wind" ){ wizardry = Utility.RandomList( 1, 5 ); }
				else if ( monster.rCategory == "star" ){ wizardry = Utility.RandomList( 4, 5 ); }
				else if ( monster.rCategory == "sand" ){ wizardry = Utility.RandomList( 1, 5 ); }
				else if ( monster.rCategory == "storm" ){ wizardry = Utility.RandomList( 8, 5 ); }
			}
			else if ( m is ElfMage || m is EvilMage || m is OrkMage )
			{
				if ( m.EmoteHue == 17 ){ wizardry = Utility.RandomList( 1, 5 ); }
				else if ( m.EmoteHue == 16 ){ wizardry = Utility.RandomList( 3, 5 ); }
				else if ( m.EmoteHue == 10 ){ wizardry = Utility.RandomList( 9, 8, 5 ); }
				else if ( m.EmoteHue == 9 ){ wizardry = Utility.RandomList( 6, 5 ); }
				else if ( m.EmoteHue == 8 ){ wizardry = Utility.RandomList( 4, 5 ); }
				else if ( m.EmoteHue == 7 ){ wizardry = Utility.RandomList( 3, 5 ); }
				else if ( m.EmoteHue == 6 ){ wizardry = Utility.RandomList( 6, 5 ); }
				else if ( m.EmoteHue == 5 ){ wizardry = Utility.RandomList( 7, 5 ); }
				else if ( m.EmoteHue == 3 ){ wizardry = Utility.RandomList( 7, 4, 5 ); }
				else if ( m.EmoteHue == 2 ){ wizardry = Utility.RandomList( 7, 5 ); }
			}

			return wizardry;
		}
	}
}