using System;
using System.Collections.Generic;
using Server.Items;
using Server.Network;

namespace Server.Misc
{
    class BardFunctions
    {
		public static void UseBardInstrument( Item instrument, bool succeed, Mobile singer )
		{
			if ( instrument == null )
				return;

			if ( !(instrument is BaseInstrument) )
				return;

			if ( singer == null )
				return;

			BaseInstrument harp = (BaseInstrument)instrument;

			if ( succeed )
				singer.PlaySound(harp.SuccessSound);
			else
				singer.PlaySound(harp.FailureSound);

			if ( harp.UsesRemaining > 1 )
				harp.UsesRemaining--;
			else
			{
				if ( singer != null )
					singer.SendLocalizedMessage( 502079 ); // The instrument played its last tune.

				instrument.Delete();
			}
		}
	}
}

namespace Server.Spells.Song
{
	public abstract class Song : Spell 
	{
		public SongBook m_Book;

		public virtual bool UseDefaultInstrument{ get{ return true; } } 
		public abstract double RequiredSkill{ get; } 
		public abstract int RequiredMana{ get; } 

		public override SkillName CastSkill{ get{ return SkillName.Musicianship; } } 
		public override SkillName DamageSkill{ get{ return SkillName.Musicianship; } } 

		public override bool ClearHandsOnCast{ get{ return false; } } 
		public override bool BlocksMovement{ get{ return false; } }

		public Song( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info ) 
		{ 
		}

		public static int MusicSkill( Mobile m )
		{
			return (int)(m.Skills[SkillName.Musicianship].Value + m.Skills[SkillName.Provocation].Value + m.Skills[SkillName.Discordance].Value + m.Skills[SkillName.Peacemaking].Value );
		}

		public override bool CheckCast()
		{	
			if ( !base.CheckCast() )
				return false;

			if ( Caster.Skills[CastSkill].Value < RequiredSkill )
			{
				string args = String.Format( "{0}\t{1}\t ", RequiredSkill.ToString( "F1" ), CastSkill.ToString() );
				Caster.SendLocalizedMessage( 1063013, args ); // You need at least ~1_SKILL_REQUIREMENT~ ~2_SKILL_NAME~ skill to use that ability.
				return false;
			}
			
			if ( UseDefaultInstrument && BaseInstrument.GetInstrument( Caster) == null )
			{
				BaseInstrument.PickInstrument( Caster, null );
				return false;
			}

            Spellbook book = Spellbook.Find(Caster, -1, SpellbookType.Song);

			if ( book is SongBook )
				m_Book = (SongBook)book;

            if ( m_Book == null )
            {
                Caster.SendMessage("You seem to be missing your book of songs.");
                return false;
            }

            if ( m_Book.Instrument == null || m_Book.Instrument.Parent != Caster )
            {
                // Auto-detect an equipped instrument if not already set
                BaseInstrument found = null;
                foreach ( Item item in Caster.Items )
                {
                    if ( item is BaseInstrument )
                    {
                        found = (BaseInstrument)item;
                        break;
                    }
                }

                if ( found == null && Caster.Backpack != null )
                {
                    found = Caster.Backpack.FindItemByType<BaseInstrument>();
                }

                if ( found != null )
                {
                    m_Book.Instrument = found;
                }
                else
                {
                    Caster.SendMessage("Your instrument is not equipped!");
                    return false;
                }
            }

			return true;
		}

		public override void OnCast()
        {
		}

		public override void DoFizzle()
		{
			Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1115710 ); // The song fizzles.
			// 1115737 You feel invigorated by the bard's spellsong
			// 1115758 The bard's song fills you with resilience
			// 1115759 The bard's song fills you with invincible
			// 1115774 You halt your spellsong
			// 1115938 Your spellsong has finished
			// 1149722 Your spellsong has ended
		}

		public override void GetCastSkills( out double min, out double max ) 
		{ 
			min = RequiredSkill; 
			max = RequiredSkill + 30.0; 
		} 

		public override int GetMana() 
		{ 
			return RequiredMana; 
		}

		public override TimeSpan GetCastDelay()
		{
			// Not affected by FC
			return CastDelayBase;
		}

		protected List<Mobile> GetNearbyFriends(int range = 10)
		{
			var enumerable = Caster.GetMobilesInRange(range);

			var targets = new List<Mobile>();

			foreach (Mobile m in enumerable)
			{
				if (isFriendly(Caster, m))
					targets.Add(m);
			}

			enumerable.Free();

			return targets;
		}
	} 
}