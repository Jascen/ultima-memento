using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Engines.Craft;
using Server.Network;
using Server.Spells;
using Server.Targeting;
using Server.Spells.Elementalism;
using System.Globalization;

namespace Server.Items
{
	public enum SpellbookType
	{
		Invalid = -1,
		Regular,
		Necromancer,
		Paladin,
		Ninja,
		Samurai,
		Elementalism,
		Song,
		DeathKnight,
		HolyMan,
		Mystic,
		Syth,
		Jedi,
		Archmage
	}

	public class Spellbook : Item, ICraftable, ISlayer
	{
		public override void ResourceChanged( CraftResource resource )
		{
			if ( !ResourceCanChange() )
				return;

			ResourceMods.Modify( this, true );
			m_Resource = resource;
			Hue = CraftResources.GetHue(m_Resource);
			TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;

			ColorHue3 = "EFB62C";
			if ( IsStandardResource( m_Resource ) )
				ColorText3 = null;
			else
				ColorText3 = cultInfo.ToTitleCase(CraftResources.GetTradeItemFullName( this, m_Resource, false, false, "bound" ));

			ResourceMods.Modify( this, false );
			InvalidateProperties();

			if ( Parent is Mobile )
				((Mobile)Parent).UpdateResistances();
		}

		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }
		public override Catalogs DefaultCatalog{ get{ return Catalogs.Book; } }

		public virtual int BasePhysicalResistance{ get{ return 0; } }
		public virtual int BaseFireResistance{ get{ return 0; } }
		public virtual int BaseColdResistance{ get{ return 0; } }
		public virtual int BasePoisonResistance{ get{ return 0; } }
		public virtual int BaseEnergyResistance{ get{ return 0; } }

		public override int PhysicalResistance{ get{ return BasePhysicalResistance + (int)(GetResourceAttrs().ArmorPhysicalResist/3) + m_AosResistances.Physical; } }
		public override int FireResistance{ get{ return BaseFireResistance + (int)(GetResourceAttrs().ArmorFireResist/2) + m_AosResistances.Fire; } }
		public override int ColdResistance{ get{ return BaseColdResistance + (int)(GetResourceAttrs().ArmorColdResist/2) + m_AosResistances.Cold; } }
		public override int PoisonResistance{ get{ return BasePoisonResistance + (int)(GetResourceAttrs().ArmorPoisonResist/2) + m_AosResistances.Poison; } }
		public override int EnergyResistance{ get{ return BaseEnergyResistance + (int)(GetResourceAttrs().ArmorEnergyResist/2) + m_AosResistances.Energy; } }

		public CraftAttributeInfo GetResourceAttrs()
		{
			CraftResourceInfo info = CraftResources.GetInfo( m_Resource );

			if ( info == null )
				return CraftAttributeInfo.Blank;

			return info.AttributeInfo;
		}

		public bool MageryBook()
		{
			if ( this is SongBook )
				return false;
			else if ( this is BookOfBushido )
				return false;
			else if ( this is DeathKnightSpellbook )
				return false;
			else if ( this is ElementalSpellbook )
				return false;
			else if ( this is HolyManSpellbook )
				return false;
			else if ( this is JediSpellbook )
				return false;
			else if ( this is BookOfChivalry )
				return false;
			else if ( this is MysticSpellbook )
				return false;
			else if ( this is NecromancerSpellbook )
				return false;
			else if ( this is BookOfNinjitsu )
				return false;
			else if ( this is AncientSpellbook )
				return false;
			else if ( this is SythSpellbook )
				return false;

			return true;
		}

		public bool AllowSlayers()
		{
			return this is ElementalSpellbook
				|| GetType() == typeof(Spellbook);
		}

		public static void Initialize()
		{
			EventSink.OpenSpellbookRequest += new OpenSpellbookRequestEventHandler( EventSink_OpenSpellbookRequest );
			EventSink.CastSpellRequest += new CastSpellRequestEventHandler( EventSink_CastSpellRequest );

			CommandSystem.Register( "AllSpells", AccessLevel.GameMaster, new CommandEventHandler( AllSpells_OnCommand ) );
		}

		[Usage( "AllSpells" )]
		[Description( "Completely fills a targeted spellbook with scrolls." )]
		private static void AllSpells_OnCommand( CommandEventArgs e )
		{
			e.Mobile.BeginTarget( -1, false, TargetFlags.None, new TargetCallback( AllSpells_OnTarget ) );
			e.Mobile.SendMessage( "Target the spellbook to fill." );
		}

		private static void AllSpells_OnTarget( Mobile from, object obj )
		{
			if ( obj is Spellbook || obj is AncientSpellbook )
			{
				Spellbook book = (Spellbook)obj;

				if ( book.BookCount == 64 )
					book.Content = ulong.MaxValue;
				else
					book.Content = (1ul << book.BookCount) - 1;

				from.SendMessage( "The spellbook has been filled." );

				CommandLogging.WriteLine( from, "{0} {1} filling spellbook {2}", from.AccessLevel, CommandLogging.Format( from ), CommandLogging.Format( book ) );
			}
			else if ( obj is ResearchBag )
			{
				ResearchBag bag = (ResearchBag)obj;

				bag.ResearchSpells = "1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#";
				bag.ResearchPrep1 = "99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#";
				bag.ResearchPrep2 = "99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#99#";

				bag.SpellsMagery = "1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#";
				bag.SpellsNecromancy = "1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#";
				bag.RuneFound = "1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#1#";
			}
			else
			{
				from.BeginTarget( -1, false, TargetFlags.None, new TargetCallback( AllSpells_OnTarget ) );
				from.SendMessage( "That is not a spellbook. Try again." );
			}
		}

		private static void EventSink_OpenSpellbookRequest( OpenSpellbookRequestEventArgs e )
		{
			Mobile from = e.Mobile;

			if ( !Multis.DesignContext.Check( from ) )
				return; // They are customizing

			SpellbookType type;

			switch ( e.Type )
			{
				default:
				case 1: 	type = SpellbookType.Regular; break;
				case 2: 	type = SpellbookType.Necromancer; break;
				case 3: 	type = SpellbookType.Paladin; break;
				case 4: 	type = SpellbookType.Ninja; break;
				case 5: 	type = SpellbookType.Samurai; break;
				case 6:		type = SpellbookType.Elementalism; break;
				case 7:		type = SpellbookType.Song; break;
				case 8:		type = SpellbookType.DeathKnight; break;
				case 9:		type = SpellbookType.HolyMan; break;
				case 10:	type = SpellbookType.Mystic; break;
				case 11:	type = SpellbookType.Syth; break;
				case 12:	type = SpellbookType.Jedi; break;
				case 13:	type = SpellbookType.Archmage; break;
			}

			Spellbook book = Spellbook.Find( from, -1, type );

			if ( book != null )
				book.DisplayTo( from );
		}

		private static void EventSink_CastSpellRequest( CastSpellRequestEventArgs e )
		{
			Mobile from = e.Mobile;

			if ( !Multis.DesignContext.Check( from ) )
				return; // They are customizing

			Spellbook book = e.Spellbook as Spellbook;
			int spellID = e.SpellID;

			if ( book == null || !book.HasSpell( spellID ) )
				book = Find( from, spellID );

			if ( book != null && book.HasSpell( spellID ) )
			{
				SpecialMove move = SpellRegistry.GetSpecialMove( spellID );

				if ( move != null )
				{
					SpecialMove.SetCurrentMove( from, move );
				}
				else
				{
					Spell spell = SpellRegistry.NewSpell( spellID, from, null );
	
					if ( spell != null )
						spell.Cast();
					else
						from.SendLocalizedMessage( 502345 ); // This spell has been temporarily disabled.
				}
			}
			else
			{
				from.SendLocalizedMessage( 500015 ); // You do not have that spell!
			}
		}

		private static Dictionary<Mobile, List<Spellbook>> m_Table = new Dictionary<Mobile, List<Spellbook>>();

		public static SpellbookType GetTypeForSpell( int spellID )
		{
			if ( spellID >= 0 && spellID < 64 )
				return SpellbookType.Regular;
			else if ( spellID >= 100 && spellID < 117 )
				return SpellbookType.Necromancer;
			else if ( spellID >= 200 && spellID < 210 )
				return SpellbookType.Paladin;
			else if( spellID >= 400 && spellID < 406 )
				return SpellbookType.Samurai;
			else if( spellID >= 500 && spellID < 508 )
				return SpellbookType.Ninja;
			else if ( spellID >= 300 && spellID < 332 )
				return SpellbookType.Elementalism;
			else if ( spellID >= 351 && spellID < 367 )
				return SpellbookType.Song;
			else if ( spellID >= 750 && spellID < 764 )
				return SpellbookType.DeathKnight;
			else if ( spellID >= 770 && spellID < 784 )
				return SpellbookType.HolyMan;
			else if ( spellID >= 250 && spellID < 260 )
				return SpellbookType.Mystic;
			else if ( spellID >= 270 && spellID < 280 )
				return SpellbookType.Syth;
			else if ( spellID >= 280 && spellID < 290 )
				return SpellbookType.Jedi;
			else if ( spellID >= 600 && spellID < 664 )
				return SpellbookType.Archmage;

			return SpellbookType.Invalid;
		}

		public static Spellbook FindRegular( Mobile from )
		{
			return Find( from, -1, SpellbookType.Regular );
		}

		public static Spellbook FindNecromancer( Mobile from )
		{
			return Find( from, -1, SpellbookType.Necromancer );
		}

		public static Spellbook FindPaladin( Mobile from )
		{
			return Find( from, -1, SpellbookType.Paladin );
		}

		public static Spellbook FindSamurai( Mobile from )
		{
			return Find( from, -1, SpellbookType.Samurai );
		}

		public static Spellbook FindNinja( Mobile from )
		{
			return Find( from, -1, SpellbookType.Ninja );
		}

		public static Spellbook FindElementalism( Mobile from )
		{
			return Find( from, -1, SpellbookType.Elementalism );
		}

		public static Spellbook FindSong( Mobile from )
		{
			return Find( from, -1, SpellbookType.Song );
		}

		public static Spellbook FindDeathKnight( Mobile from )
		{
			return Find( from, -1, SpellbookType.DeathKnight );
		}

		public static Spellbook FindHolyMan( Mobile from )
		{
			return Find( from, -1, SpellbookType.HolyMan );
		}

		public static Spellbook FindMystic( Mobile from )
		{
			return Find( from, -1, SpellbookType.Mystic );
		}

		public static Spellbook FindSyth( Mobile from )
		{
			return Find( from, -1, SpellbookType.Syth );
		}

		public static Spellbook FindJedi( Mobile from )
		{
			return Find( from, -1, SpellbookType.Jedi );
		}

		public static Spellbook FindArchmage( Mobile from )
		{
			return Find( from, -1, SpellbookType.Archmage );
		}

		public static Spellbook Find( Mobile from, int spellID )
		{
			return Find( from, spellID, GetTypeForSpell( spellID ) );
		}

		public static Spellbook Find( Mobile from, int spellID, SpellbookType type )
		{
			if ( from == null )
				return null;

			if ( from.Deleted )
			{
				m_Table.Remove( from );
				return null;
			}

			List<Spellbook> list = null;

			m_Table.TryGetValue( from, out list );

			bool searchAgain = false;

			if ( list == null )
				m_Table[from] = list = FindAllSpellbooks( from );
			else
				searchAgain = true;

			Spellbook book = FindSpellbookInList( list, from, spellID, type );

			if ( book == null && searchAgain )
			{
				m_Table[from] = list = FindAllSpellbooks( from );

				book = FindSpellbookInList( list, from, spellID, type );
			}

			return book;
		}

		public static Spellbook FindSpellbookInList( List<Spellbook> list, Mobile from, int spellID, SpellbookType type )
		{
			Container pack = from.Backpack;

			for ( int i = list.Count - 1; i >= 0; --i )
			{
				if ( i >= list.Count )
					continue;

				Spellbook book = list[i];

				if ( ElementalSpell.CanUseBook( book, from, false ) && !book.Deleted && (book.Parent == from || (pack != null && book.Parent == pack)) && ValidateSpellbook( book, spellID, type ) )
					return book;

				list.RemoveAt( i );
			}

			return null;
		}

		public static List<Spellbook> FindAllSpellbooks( Mobile from )
		{
			List<Spellbook> list = new List<Spellbook>();

			Item item = from.FindItemOnLayer( Layer.Trinket );

			if ( item is Spellbook )
			{
				list.Add( (Spellbook)item );
			}
		
			Container pack = from.Backpack;

			if ( pack == null )
				return list;

			for ( int i = 0; i < pack.Items.Count; ++i )
			{
				item = pack.Items[i];

				if ( item is Spellbook )
					list.Add( (Spellbook)item );
			}

			return list;
		}

		public static Spellbook FindEquippedSpellbook( Mobile from )
		{
			Item item = from.FindItemOnLayer( Layer.Trinket );
			if ( item is Spellbook )
			{
				return (item as Spellbook);
			}

			return (from.FindItemOnLayer( Layer.Trinket ) as Spellbook);
		}

		public static bool ValidateSpellbook( Spellbook book, int spellID, SpellbookType type )
		{
			return ( book.SpellbookType == type && ( spellID == -1 || book.HasSpell( spellID ) ) );
		}

		public override bool DisplayWeight { get { return false; } }

		private AosAttributes m_AosAttributes;
		private AosElementAttributes m_AosResistances;
		private AosSkillBonuses m_AosSkillBonuses;

		[CommandProperty( AccessLevel.GameMaster )]
		public AosAttributes Attributes
		{
			get{ return m_AosAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosElementAttributes Resistances
		{
			get{ return m_AosResistances; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosSkillBonuses SkillBonuses
		{
			get{ return m_AosSkillBonuses; }
			set{}
		}

		public virtual SpellbookType SpellbookType{ get{ return SpellbookType.Regular; } }
		public virtual int BookOffset{ get{ return 0; } }
		public virtual int BookCount{ get{ return 64; } }

		private ulong m_Content;
		private int m_Count;

		public override bool CanEquip( Mobile from )
		{
			if ( !from.CanBeginAction( typeof( BaseWeapon ) ) )
				return false;

			return base.CanEquip( from );
		}

		public override bool AllowEquipedCast( Mobile from )
		{
			return true;
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( ( dropped is BlankScroll || dropped is ScribesPen ) && this is AncientSpellbook )
			{
				AncientSpellbook book = (AncientSpellbook)this;

				if ( dropped is BlankScroll && book.paper > 50000 )
				{
					from.SendMessage( "This book has too many pages already." );
				}
				else if ( dropped is BlankScroll )
				{
					from.PlaySound( 0x48 );
					book.paper = book.paper + dropped.Amount;
					dropped.Delete();
					from.SendMessage( "The blank scrolls are now extra pages in your book." );
				}
				else if ( dropped is ScribesPen && this is AncientSpellbook )
				{
					BaseTool tool = (BaseTool)dropped;

					if ( book.quill > 50000 )
					{
						from.SendMessage( "This book has too many quills set aside for it." );
					}
					else
					{
						from.PlaySound( 0x48 );
						book.quill = book.quill + tool.UsesRemaining;
						dropped.Delete();
						from.SendMessage( "The quills have been set aside for your book." );
					}
				}
			}

			if ( dropped is SpellScroll && dropped.Amount == 1 )
			{
				SpellScroll scroll = (SpellScroll)dropped;

				SpellbookType type = GetTypeForSpell( scroll.SpellID );

				if ( type != this.SpellbookType )
				{
					return false;
				}
				else if ( HasSpell( scroll.SpellID ) )
				{
					if ( this is SythSpellbook ){ from.SendMessage( "That power is already in that datacron." ); }
					else if ( this is JediSpellbook ){ from.SendMessage( "That wisdom is already in that datacron." ); }
					else { from.SendLocalizedMessage( 500179 ); } // That spell is already present in that spellbook.
					return false;
				}
				else
				{
					int val = scroll.SpellID - BookOffset;

					if ( val >= 0 && val < BookCount )
					{
						m_Content |= (ulong)1 << val;
						++m_Count;

						InvalidateProperties();

						scroll.Delete();

						if ( this is SythSpellbook ){ from.SendSound( 0x558 ); }
						else if ( this is JediSpellbook ){ from.SendSound( 0x558 ); }
						else { from.Send( new PlaySound( 0x249, GetWorldLocation() ) ); }
						return true;
					}

					return false;
				}
			}
			else
			{
				return false;
			}
		}

		public void AddAncient( int id )
		{
			int val = id - BookOffset;

			if ( val >= 0 && val < BookCount )
			{
				m_Content |= (ulong)1 << val;
				++m_Count;

				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public ulong Content
		{
			get
			{
				return m_Content;
			}
			set
			{
				if ( m_Content != value )
				{
					m_Content = value;

					m_Count = 0;

					while ( value > 0 )
					{
						m_Count += (int)(value & 0x1);
						value >>= 1;
					}

					InvalidateProperties();
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int SpellCount
		{
			get
			{
				return m_Count;
			}
		}

		public override string DefaultDescription{ get{ return "This book is used by mages, where they can record the magic they can unleash. Dropping such scrolls onto this book will place the spell within its pages. Some books have enhanced properties, that are only effective when the book is held."; } }

		[Constructable]
		public Spellbook() : this( (ulong)0 )
		{
		}

		[Constructable]
		public Spellbook( ulong content ) : this( content, 0x0EFA )
		{
		}

		public Spellbook( ulong content, int itemID ) : base( itemID )
		{
			m_AosAttributes = new AosAttributes( this );
			m_AosResistances = new AosElementAttributes( this );
			m_AosSkillBonuses = new AosSkillBonuses( this );

			Weight = 3.0;
			Layer = Layer.Trinket;

			Content = content;
		}

		public override void OnAfterDuped( Item newItem )
		{
			Spellbook book = newItem as Spellbook;

			if ( book == null )
				return;

			book.m_AosAttributes = new AosAttributes( newItem, m_AosAttributes );
			book.m_AosResistances = new AosElementAttributes( newItem, m_AosResistances );
			book.m_AosSkillBonuses = new AosSkillBonuses( newItem, m_AosSkillBonuses );

			book.Content = this.Content;
		}

		public override void OnLocationChange( Point3D oldLocation )
		{
			ResourceMods.DefaultItemHue( this );
			base.OnLocationChange( oldLocation );
		}

		public override void DefaultMainHue( Item item )
		{
			ResourceMods.DefaultItemHue( item );
		}

		public override void AddItem( Item item )
		{
			ResourceMods.DefaultItemHue( this );
			base.AddItem( item );
		}

		public override void OnAdded( object parent )
		{
			ResourceMods.DefaultItemHue( this );

			if ( parent is Mobile )
			{
				Mobile from = (Mobile)parent;

				m_AosSkillBonuses.AddTo( from );

				int strBonus = m_AosAttributes.BonusStr;
				int dexBonus = m_AosAttributes.BonusDex;
				int intBonus = m_AosAttributes.BonusInt;

				if ( strBonus != 0 || dexBonus != 0 || intBonus != 0 )
				{
					string modName = this.Serial.ToString();

					if ( strBonus != 0 )
						from.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

					if ( dexBonus != 0 )
						from.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

					if ( intBonus != 0 )
						from.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
				}

				from.CheckStatTimers();
			}
			base.OnAdded( parent );
		}

		public static Item MagicBook()
		{
			Item item = null;

			switch ( Utility.Random( 8 ) ) 
			{
				case 0: item = new SongBook();	break;
				case 1: item = new Spellbook();	break;
				case 2: item = new MysticSpellbook();	break;
				case 3: item = new BookOfNinjitsu();	break;
				case 4: item = new BookOfBushido();	break;
				case 5: item = new NecromancerSpellbook();	break;
				case 6: item = new BookOfChivalry();	break;
				case 7: item = new ElementalSpellbook();	break;
			}

			return item;
		}

		public override void OnRemoved( object parent )
		{
			if ( Core.AOS && parent is Mobile )
			{
				Mobile from = (Mobile)parent;

				m_AosSkillBonuses.Remove();

				string modName = this.Serial.ToString();

				from.RemoveStatMod( modName + "Str" );
				from.RemoveStatMod( modName + "Dex" );
				from.RemoveStatMod( modName + "Int" );

				from.CheckStatTimers();
			}
		}

		public override bool OnEquip( Mobile from )
		{
			ResourceMods.DefaultItemHue( this );

			if ( this is SongBook ){ if ( from.Skills[SkillName.Musicianship].Base < 30 )
			{
				from.SendMessage("Your need at least a natural neophyte skill in musicianship to equip that!");
				return false;
			}}
			else if ( this is NecromancerSpellbook ){ if ( from.Skills[SkillName.Necromancy].Base < 30 )
			{
				from.SendMessage("Your need at least a natural neophyte skill in necromancy to equip that!");
				return false;
			}}
			else if ( this is ElementalSpellbook )
			{
				if ( from.Skills[SkillName.Elementalism].Base < 30 )
				{
					from.SendMessage("Your need at least a natural neophyte skill in elementalism to equip that!");
					return false;
				}
				if ( !ElementalSpell.CanUseBook( this, from, true ) )
					return false;
			}

			else if ( this is BookOfNinjitsu ){ if ( from.Skills[SkillName.Ninjitsu].Base < 30 )
			{
				from.SendMessage("Your need at least a natural neophyte skill in ninjitsu to equip that!");
				return false;
			}}
			else if ( this is BookOfBushido ){ if ( from.Skills[SkillName.Bushido].Base < 30 )
			{
				from.SendMessage("Your need at least a natural neophyte skill in bushido to equip that!");
				return false;
			}}
			else if ( this is BookOfChivalry ){ if ( from.Skills[SkillName.Knightship].Base < 30 || from.Karma < 0 )
			{
				from.SendMessage("Your need at least a natural neophyte skill in knightship to equip that!");
				return false;
			}}
			else if ( this is DeathKnightSpellbook ){ if ( from.Skills[SkillName.Knightship].Base < 30 || from.Karma > 0 )
			{
				from.SendMessage("Your need at least a natural neophyte skill in knightship to equip that!");
				return false;
			}}
			else if ( this is HolyManSpellbook )
			{
				return false;
			}
			else if ( this is MysticSpellbook ){ if ( from.Skills[SkillName.Focus].Base < 100 || from.Skills[SkillName.Meditation].Base < 100 )
			{
				from.SendMessage("Your need at least a natural grandmaster skill in focus and meditation to equip that!");
				return false;
			}}
			else if ( this is SythSpellbook )
			{
				return false;
			}
			else if ( this is JediSpellbook )
			{
				return false;
			}
			else if ( this is AncientSpellbook )
			{
				if ( ((AncientSpellbook)this).Owner != from )
					return false;
				if ( from.Skills[SkillName.Magery].Base < 30 && from.Skills[SkillName.Necromancy].Base < 30 )
				{
					from.SendMessage("Your need at least a natural neophyte skill in magery or necromancy to equip that!");
					return false;
				}
			}
			else if ( from.Skills[SkillName.Magery].Base < 30 )
			{
				from.SendMessage("Your need at least a natural neophyte skill in magery to equip that!");
				return false;
			}

			return base.OnEquip( from );
		}

		public bool HasSpell( int spellID )
		{
			spellID -= BookOffset;

			return ( spellID >= 0 && spellID < BookCount && (m_Content & ((ulong)1 << spellID)) != 0 );
		}

		public Spellbook( Serial serial ) : base( serial )
		{
		}

		public void DisplayTo( Mobile to )
		{
			// The client must know about the spellbook or it will crash!

			NetState ns = to.NetState;

			if ( ns == null )
				return;

			if ( Parent == null )
			{
				to.Send( this.WorldPacket );
			}
			else if ( Parent is Item )
			{
				// What will happen if the client doesn't know about our parent?
				if ( ns.ContainerGridLines )
					to.Send( new ContainerContentUpdate6017( this ) );
				else
					to.Send( new ContainerContentUpdate( this ) );
			}
			else if ( Parent is Mobile )
			{
				// What will happen if the client doesn't know about our parent?
				to.Send( new EquipUpdate( this ) );
			}

			if ( ns.HighSeas )
				to.Send( new DisplaySpellbookHS( this ) );
			else
				to.Send( new DisplaySpellbook( this ) );

			if ( Core.AOS ) {

				if ( ns.NewSpellbook ) {
					to.Send( new NewSpellbookContent( this, ItemID, BookOffset + 1, m_Content ) );
				} else {
					//to.Send( new SpellbookContent( m_Count, BookOffset + 1, m_Content, this ) );
				}
			}
			else {
				if ( ns.ContainerGridLines ) {
					to.Send( new SpellbookContent6017( m_Count, BookOffset + 1, m_Content, this ) );
				} else {
					to.Send( new SpellbookContent( m_Count, BookOffset + 1, m_Content, this ) );
				}
			}
		}

		public override bool DisplayLootType{ get{ return Core.AOS; } }

		private string GetNameString()
		{
			string name = this.Name;

			if ( name == null )
				name = String.Format( "#{0}", LabelNumber );

			return name;
		}

		public virtual int GetLuckBonus()
		{
			CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );

			if ( resInfo == null )
				return 0;

			CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

			if ( attrInfo == null )
				return 0;

			return attrInfo.ArmorLuck;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_BuiltBy != null )
				list.Add( 1050043, m_BuiltBy.Name ); // crafted by ~1_NAME~
				
			m_AosSkillBonuses.GetProperties( list );

			if( m_Slayer != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer );
				if( entry != null )
					list.Add( entry.Title );
			}

			if( m_Slayer2 != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer2 );
				if( entry != null )
					list.Add( entry.Title );
			}

			int prop;

			if ( (prop = m_AosAttributes.WeaponDamage) != 0 )
				list.Add( 1060401, prop.ToString() ); // damage increase ~1_val~%

			if ( (prop = m_AosAttributes.DefendChance) != 0 )
				list.Add( 1060408, prop.ToString() ); // defense chance increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusDex) != 0 )
				list.Add( 1060409, prop.ToString() ); // dexterity bonus ~1_val~

			if ( (prop = m_AosAttributes.EnhancePotions) != 0 )
				list.Add( 1060411, prop.ToString() ); // enhance potions ~1_val~%

			if ( (prop = m_AosAttributes.CastRecovery) != 0 )
				list.Add( 1060412, prop.ToString() ); // faster cast recovery ~1_val~

			if ( (prop = m_AosAttributes.CastSpeed) != 0 )
				list.Add( 1060413, prop.ToString() ); // faster casting ~1_val~

			if ( (prop = m_AosAttributes.AttackChance) != 0 )
				list.Add( 1060415, prop.ToString() ); // hit chance increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusHits) != 0 )
				list.Add( 1060431, prop.ToString() ); // hit point increase ~1_val~

			if ( (prop = m_AosAttributes.BonusInt) != 0 )
				list.Add( 1060432, prop.ToString() ); // intelligence bonus ~1_val~

			if ( (prop = m_AosAttributes.LowerManaCost) != 0 && MyServerSettings.LowerMana() > 0 )
			{
				if ( prop > MyServerSettings.LowerMana() ){ prop = MyServerSettings.LowerMana(); }
				list.Add( 1060433, prop.ToString() ); // lower mana cost ~1_val~%
			}

			if ( (prop = m_AosAttributes.LowerRegCost) != 0 && MyServerSettings.LowerReg() > 0 )
			{
				if ( prop > MyServerSettings.LowerReg() ){ prop = MyServerSettings.LowerReg(); }
				list.Add( 1060434, prop.ToString() ); // lower reagent cost ~1_val~%
			}

			if ( (prop = (GetLuckBonus() + m_AosAttributes.Luck)) != 0 )
				list.Add( 1060436, prop.ToString() ); // luck ~1_val~

			if ( (prop = m_AosAttributes.BonusMana) != 0 )
				list.Add( 1060439, prop.ToString() ); // mana increase ~1_val~

			if ( (prop = m_AosAttributes.RegenMana) != 0 )
				list.Add( 1060440, prop.ToString() ); // mana regeneration ~1_val~

			if ( (prop = m_AosAttributes.NightSight) != 0 )
				list.Add( 1060441 ); // night sight

			if ( (prop = m_AosAttributes.ReflectPhysical) != 0 )
				list.Add( 1060442, prop.ToString() ); // reflect physical damage ~1_val~%

			if ( (prop = m_AosAttributes.RegenStam) != 0 )
				list.Add( 1060443, prop.ToString() ); // stamina regeneration ~1_val~

			if ( (prop = m_AosAttributes.RegenHits) != 0 )
				list.Add( 1060444, prop.ToString() ); // hit point regeneration ~1_val~

			if ( (prop = m_AosAttributes.SpellChanneling) != 0 )
				list.Add( 1060482 ); // spell channeling

			if ( (prop = m_AosAttributes.SpellDamage) != 0 )
				list.Add( 1060483, prop.ToString() ); // spell damage increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusStam) != 0 )
				list.Add( 1060484, prop.ToString() ); // stamina increase ~1_val~

			if ( (prop = m_AosAttributes.BonusStr) != 0 )
				list.Add( 1060485, prop.ToString() ); // strength bonus ~1_val~

			if ( (prop = m_AosAttributes.WeaponSpeed) != 0 )
				list.Add( 1060486, prop.ToString() ); // swing speed increase ~1_val~%

			base.AddResistanceProperties( list );

			if ( Layer == Layer.Trinket )
				list.Add( 1061182, EquipLayerName( Layer ) );

			if ( this is SongBook )
			{
				if ( m_Count == 1 ){ list.Add( 1049644, "1 Song" );  } else { list.Add( 1049644, "" + m_Count.ToString() + " Songs" ); }
			}
			else if ( this is BookOfNinjitsu || this is BookOfBushido || this is MysticSpellbook )
			{
				if ( m_Count == 1 ){ list.Add( 1049644, "1 Ability" );  } else { list.Add( 1049644, "" + m_Count.ToString() + " Abilities" ); }
			}
			else if ( this is SythSpellbook || this is JediSpellbook )
			{
				if ( m_Count == 1 ){ list.Add( 1049644, "1 Power" );  } else { list.Add( 1049644, "" + m_Count.ToString() + " Powers" ); }
			}
			else
			{
				list.Add( 1042886, m_Count.ToString() ); // ~1_NUMBERS_OF_SPELLS~ Spells
			}
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			if ( m_BuiltBy != null )
				this.LabelTo( from, 1050043, m_BuiltBy.Name ); // crafted by ~1_NAME~

			this.LabelTo( from, 1042886, m_Count.ToString() );
		}

		public override void OnDoubleClick( Mobile from )
		{
			Container pack = from.Backpack;

			if ( Parent == from || ( pack != null && Parent == pack ) )
				DisplayTo( from );
			else
				from.SendLocalizedMessage( 500207 ); // The spellbook must be in your backpack (and not in a container within) to open.
		}

		private SlayerName m_Slayer;
		private SlayerName m_Slayer2;

		[CommandProperty( AccessLevel.GameMaster )]
		public SlayerName Slayer
		{
			get { return m_Slayer; }
			set { m_Slayer = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public SlayerName Slayer2
		{
			get { return m_Slayer2; }
			set { m_Slayer2 = value; InvalidateProperties(); }
		}

		public SlayerName FirstSlayer { get { return Slayer != SlayerName.None ? Slayer : Slayer2; } }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 5 ); // version

			writer.Write( (int)m_Slayer );
			writer.Write( (int)m_Slayer2 );

			m_AosAttributes.Serialize( writer );
			m_AosResistances.Serialize( writer );
			m_AosSkillBonuses.Serialize( writer );

			writer.Write( m_Content );
			writer.Write( m_Count );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 5:
				case 4:
				case 3:
				{
					if ( version < 4 )
						m_BuiltBy = reader.ReadMobile();

					goto case 2;
				}
				case 2:
				{
					m_Slayer = (SlayerName)reader.ReadInt();
					m_Slayer2 = (SlayerName)reader.ReadInt();
					goto case 1;
				}
				case 1:
				{
					m_AosAttributes = new AosAttributes( this, reader );

					if ( version > 4 )
						m_AosResistances = new AosElementAttributes( this, reader );
					else
						m_AosResistances = new AosElementAttributes( this );

					m_AosSkillBonuses = new AosSkillBonuses( this, reader );

					goto case 0;
				}
				case 0:
				{
					m_Content = reader.ReadULong();
					m_Count = reader.ReadInt();

					break;
				}
			}

			if ( m_AosAttributes == null )
				m_AosAttributes = new AosAttributes( this );

			if ( m_AosSkillBonuses == null )
				m_AosSkillBonuses = new AosSkillBonuses( this );

			if ( Parent is Mobile )
				m_AosSkillBonuses.AddTo( (Mobile) Parent );

			int strBonus = m_AosAttributes.BonusStr;
			int dexBonus = m_AosAttributes.BonusDex;
			int intBonus = m_AosAttributes.BonusInt;

			if ( Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0) )
			{
				Mobile m = (Mobile)Parent;

				string modName = Serial.ToString();

				if ( strBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

				if ( dexBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

				if ( intBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
			}

			if ( Parent is Mobile )
				((Mobile)Parent).CheckStatTimers();
		}

		public int OnCraft( int quality, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Type resourceType = typeRes;

			if ( resourceType == null )
				resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

			Resource = CraftResources.GetFromType( resourceType );

			CraftContext context = craftSystem.GetContext( from );

			if ( context != null && context.DoNotColor )
				Hue = 0;

			return quality;
		}
	}
}