using System;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Spells.Research
{
	public class ResearchDeathSpeak : ResearchSpell
	{
		public int CirclePower = 1;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 614,
					IconGraphic = 11203,
					Name = "Death Speak",
					PowerWords = "Kal Wis Corp",
					Description = "This spell allows you to communicate with recently slain supernatural foes. If you can speak to their souls, you have the potential to restore a part of yourself. The restoration goes in order of mana, stamina, and health. The amount restored is dependent on the power of the caster and the fame of the slain creature. The caster may lose some karma if they attempt this.",
					ManaCost = 5,
					MinSkill = 10,
					TargetType = TargetFlags.None
				},
				233,
				9042,
				Reagent.GraveDust,Reagent.PixieSkull
			);

		public ResearchDeathSpeak( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				Corpse toChannel = null;
				int regenerate = 0;

				int bonus = (int)(DamagingSkill( Caster )/4);
					if ( bonus > 60 ){ bonus = 60; }
					if ( bonus < 2 ){ bonus = 2; }

				foreach ( Item item in Caster.GetItemsInRange( 3 ) )
				{
					if( item is Corpse && !( (Corpse)item ).Channeled && !( (Corpse)item ).Animated )
					{
						Corpse body = (Corpse)item;

						if ( !(body.m_Owner is PlayerMobile) )
						{
							if ( body.m_Owner is BaseCreature )
							{
								SlayerEntry undead = SlayerGroup.GetEntryByName( SlayerName.Silver );
								if ( undead.Slays(body.m_Owner) )
								{
									toChannel = (Corpse)item;
									regenerate = (int)( ( ( body.m_Owner ).Fame ) / 50) + bonus;
									break;
								}
							}
						}
					}
				}

				if ( regenerate > 0 )
				{
					if ( toChannel != null )
					{
						toChannel.Channeled = true;
						toChannel.Hue = 0x835;
					}

					int mana = Caster.ManaMax - Caster.Mana;
					if ( mana > regenerate ){ Caster.Mana = Caster.Mana + regenerate; regenerate = 0; }
					else { Caster.Mana = Caster.ManaMax; regenerate = regenerate - mana; }

					int stam = Caster.ManaMax - Caster.Stam;
					if ( stam > regenerate ){ Caster.Stam = Caster.Stam + regenerate; regenerate = 0; }
					else { Caster.Stam = Caster.StamMax; regenerate = regenerate - stam; }

					int hits = Caster.ManaMax - Caster.Hits;
					if ( hits > regenerate ){ Caster.Hits = Caster.Hits + regenerate; regenerate = 0; }
					else { Caster.Hits = Caster.HitsMax; regenerate = regenerate - hits; }

					if ( Caster.Karma < 0 )
					{
						Caster.FixedParticles( 0x3400, 1, 15, 9501, 2100, 4, EffectLayer.Waist );
					}
					else
					{
						Caster.FixedParticles( 0x375A, 1, 15, 9501, 2100, 4, EffectLayer.Waist );
					}

					if ( Caster.Karma < 0 )
					{
						Caster.Say( "Xtee Mee Glau" );
						Caster.PlaySound( 0x481 );
					}
					else
					{
						Caster.Say( "Anh Mi Sah Ko" );
						Caster.PlaySound( 0x24A );
					}

					Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );

					KarmaMod( Caster, ((int)RequiredSkill+RequiredMana) );

					Caster.SendMessage( "You speak in strange tongues to the soul of the dead supernatural creature." );
				}
				else
				{
					Caster.SendMessage( "You fail to speak to any dead supernatural creatures in the area." );
					Caster.PlaySound( 0x1D6 );
				}
			}
			FinishSequence();
		}
	}
}