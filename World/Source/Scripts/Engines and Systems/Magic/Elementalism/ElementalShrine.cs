using System;
using Server;
using Server.Mobiles;
using Server.Misc;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Spells.Elementalism;
using Server.Gumps;
using Server.SpellBars;

namespace Server.Items
{
	public class ElementalShrine : Item
	{
		public override bool HandlesOnSpeech{ get{ return true; } }

		public override void OnSpeech( SpeechEventArgs e )
		{
			if ( !e.Handled )
			{
				Mobile m = e.Mobile;

				string keyword = "culara";

				if ( !m.Player )
					return;

				if ( !m.InRange( GetWorldLocation(), 2 ) )
					return;

				bool isMatch = false;

				if ( e.Speech.ToLower().IndexOf( keyword.ToLower() ) >= 0 )
					isMatch = true;

				if ( !isMatch )
					return;

				e.Handled = true;

				if ( Name == "fire" && ((PlayerMobile)m).CharacterElement == 2 )
				{
					m.FixedParticles( 0x3709, 10, 30, 5052, 0, 0, EffectLayer.LeftFoot );
					m.PlaySound( 0x208 );
					MorphingTime.ColorOnlyClothes( m, Server.Spells.Elementalism.ElementalSpell.ElementalHue( this.Name ), 0 );
					m.SendMessage( "Your equipment is burned with elemental fire." );
				}
				else if ( Name == "water" && ((PlayerMobile)m).CharacterElement == 3 )
				{
					Effects.SendLocationEffect( m.Location, m.Map, 0x23B2, 30, 10, 0, 0 );
					Effects.PlaySound( m.Location, m.Map, 0x026 );
					MorphingTime.ColorOnlyClothes( m, Server.Spells.Elementalism.ElementalSpell.ElementalHue( this.Name ), 0 );
					m.SendMessage( "Your equipment is soaked with elemental water." );
				}
				else if ( Name == "air" && ((PlayerMobile)m).CharacterElement == 0 )
				{
					Effects.SendLocationEffect( m.Location, m.Map, 0x5590, 30, 10, 0xB24, 0 );
					m.PlaySound( 0x10B );
					MorphingTime.ColorOnlyClothes( m, Server.Spells.Elementalism.ElementalSpell.ElementalHue( this.Name ), 0 );
					m.SendMessage( "Your equipment is gusted with elemental air." );
				}
				else if ( Name == "earth" && ((PlayerMobile)m).CharacterElement == 1 )
				{
					Point3D hands = new Point3D( ( m.X ), ( m.Y ), ( m.Z+5 ) );
					Effects.SendLocationEffect( hands, m.Map, 0x3837, 23, 10, 0, 0 );
					m.PlaySound( 0x65A );
					MorphingTime.ColorOnlyClothes( m, Server.Spells.Elementalism.ElementalSpell.ElementalHue( this.Name ), 0 );
					m.SendMessage( "Your equipment is grounded with elemental earth." );
				}
			}
		}

		[Constructable]
		public ElementalShrine() : base(0x1B73)
		{
			Movable = false;
			Visible = false;
			Name = "shrine";
		}

		public ElementalShrine(Serial serial) : base(serial)
		{
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( m is PlayerMobile && m.Skills[SkillName.Elementalism].Base >= 5.0 )
			{
				bool newWindows = false;

				if ( Name == "fire" && ((PlayerMobile)m).CharacterElement != 2 )
				{
					((PlayerMobile)m).CharacterElement = 2;
					Effects.SendLocationEffect( Location, Map, 0x3709, 30, 10, 0, 0 );
					Effects.PlaySound( m.Location, m.Map, 0x208 );
					ElementalSpell.ChangeBooks( m, 2 );
					m.SendMessage( "You are now focused on the elemental magic of fire." );
					newWindows = true;
				}
				else if ( Name == "water" && ((PlayerMobile)m).CharacterElement != 3 )
				{
					((PlayerMobile)m).CharacterElement = 3;
					Effects.SendLocationEffect( Location, Map, 0x23B2, 30, 10, 0, 0 );
					Effects.PlaySound( m.Location, m.Map, 0x026 );
					ElementalSpell.ChangeBooks( m, 3 );
					m.SendMessage( "You are now focused on the elemental magic of water." );
					newWindows = true;
				}
				else if ( Name == "air" && ((PlayerMobile)m).CharacterElement != 0 )
				{
					((PlayerMobile)m).CharacterElement = 0;
					Effects.SendLocationEffect( this.Location, this.Map, 0x5590, 30, 10, 0xB24, 0 );
					m.PlaySound( 0x10B );
					ElementalSpell.ChangeBooks( m, 0 );
					m.SendMessage( "You are now focused on the elemental magic of air." );
					newWindows = true;
				}
				else if ( Name == "earth" && ((PlayerMobile)m).CharacterElement != 1 )
				{
					((PlayerMobile)m).CharacterElement = 1;
					Point3D hands = new Point3D( ( this.X ), ( this.Y ), ( this.Z+5 ) );
					Effects.SendLocationEffect( hands, this.Map, 0x3837, 23, 10, 0, 0 );
					m.PlaySound( 0x65A );
					ElementalSpell.ChangeBooks( m, 1 );
					m.SendMessage( "You are now focused on the elemental magic of earth." );
					newWindows = true;
				}

				if ( newWindows )
				{
					ElementalSpell.UnequipBook( m );
					if ( m.HasGump( typeof( SpellBarsElement1 ) ) )
					{
						m.CloseGump( typeof( SpellBarsElement1 ) );
						m.SendGump( new SpellBarsElement1( m ) );
					}
					if ( m.HasGump( typeof( SpellBarsElement2 ) ) )
					{
						m.CloseGump( typeof( SpellBarsElement2 ) );
						m.SendGump( new SpellBarsElement2( m ) );
					}
					if ( m.HasGump( typeof( QuickBar ) ) )
					{
						m.CloseGump( typeof( QuickBar ) );
						m.SendGump( new QuickBar( m ) );
					}
					if ( m.HasGump( typeof( SetupBarsElement1 ) ) )
					{
						m.CloseGump( typeof( SetupBarsElement1 ) );
						m.SendGump( new SetupBarsElement1( (PlayerMobile)m, 0 ) );
					}
					if ( m.HasGump( typeof( SetupBarsElement2 ) ) )
					{
						m.CloseGump( typeof( SetupBarsElement2 ) );
						m.SendGump( new SetupBarsElement2( (PlayerMobile)m, 0 ) );
					}
					m.CloseGump( typeof( ElementalSpellbookGump ) );
					m.CloseGump( typeof( ElementalSpellHelp ) );
				}
			}
			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}