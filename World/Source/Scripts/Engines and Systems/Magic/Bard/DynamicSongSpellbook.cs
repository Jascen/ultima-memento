using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Targeting;
using Server.Spells;

namespace Server.Items
{
	public class DynamicSongSpellbook : SongBook
	{
		public override SpellbookType SpellbookType { get { return SpellbookType.DynamicSong; } }
		public override int BookOffset { get { return 2200; } }
		public override int BookCount { get { return 16; } }

		[Constructable]
		public DynamicSongSpellbook() : this( (ulong)0 )
		{
		}

		[Constructable]
		public DynamicSongSpellbook( ulong content ) : base( content )
		{
			Name = "song book";
		}

		public override void OnDoubleClick( Mobile from )
		{
			Container pack = from.Backpack;

			if ( Parent == from || ( pack != null && Parent == pack ) )
				DisplayTo( from );
			else
				from.SendLocalizedMessage( 500207 );
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from.Alive && ( Parent == from || ( from.Backpack != null && Parent == from.Backpack ) ) )
				list.Add( new SetInstrumentEntry( this ) );
		}

		private class SetInstrumentEntry : ContextMenuEntry
		{
			private DynamicSongSpellbook m_Book;

			public SetInstrumentEntry( DynamicSongSpellbook book ) : base( 6132 ) // "Set Instrument" - currently says "Use"
			{
				m_Book = book;
			}

			public override void OnClick()
			{
				Mobile from = Owner.From;
				from.SendMessage( "Select the instrument you wish to use with this song book." );
				from.Target = new InstrumentTarget( m_Book );
			}
		}

		private class InstrumentTarget : Target
		{
			private DynamicSongSpellbook m_Book;

			public InstrumentTarget( DynamicSongSpellbook book ) : base( 1, false, TargetFlags.None )
			{
				m_Book = book;
			}

			protected override void OnTarget( Mobile from, object target )
			{
				if ( target is BaseInstrument )
				{
					m_Book.Instrument = (BaseInstrument)target;
					from.SendMessage( "You set your instrument to play for these songs." );
				}
				else
				{
					from.SendMessage( "That is not a musical instrument." );
				}
			}
		}

		public DynamicSongSpellbook( Serial serial ) : base( serial )
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
