using System;
using Server.Misc;
using Server.Network;
using Server.Items;

namespace Server.Spells.Research
{
	public abstract class ResearchSpell : Spell
	{
		public int spellIndex { get { return ResearchSpellProvider.GetResearchIndex( Info.SpellDefinition.SpellID ); } }
		public virtual bool alwaysConsume { get { return false; } }
		public abstract double RequiredSkill { get; }
		public abstract int RequiredMana{ get; }
		public override bool ClearHandsOnCast { get { return true; } }
		public override SkillName CastSkill { get { return SkillName.Inscribe; } }
		public override SkillName DamageSkill { get { return SkillName.Inscribe; } }
		public override int CastRecoveryBase { get { return 1; } }

		public override bool ConsumeReagents()
		{
			if ( !Caster.Player )
				return true;

			if ( !ResearchSettings.BookCaster( Caster ) )
				return true;

			if ( !alwaysConsume && AosAttributes.GetValue( Caster, AosAttribute.LowerRegCost ) > Utility.Random( 100 ) )
				return true;

			Container pack = Caster.Backpack;

			if ( pack == null )
				return false;

			if ( pack.ConsumeTotal( Info.Reagents, Info.Amounts ) == -1 )
			{
				if ( ResearchSettings.BookCaster( Caster ) )
				{
					AncientSpellbook book = ResearchSettings.GetAncientTome( Caster );
					if ( book != null )
					{
						book.paper--;
						book.quill--;
					}
				}
				return true;
			}

			return false;
		}

		public static double CastingSkill( Mobile from )
		{
			double skill = from.Skills[SkillName.Necromancy].Value;
				if ( from.Skills[SkillName.Magery].Value > skill ){ skill = from.Skills[SkillName.Magery].Value; }

			return skill;
		}

		public static double DamagingSkill( Mobile from )
		{
			return ( from.Skills[SkillName.Necromancy].Value + from.Skills[SkillName.Magery].Value + from.Skills[SkillName.Spiritualism].Value + from.Skills[SkillName.Psychology].Value ) / 2;
		}

		public static void KarmaMod( Mobile from, int mod )
		{
			int karma = -(int)(70 + mod);
			if ( karma != 0 ){ Misc.Titles.AwardKarma( from, karma, true ); }
		}

		public static bool SpellPrepared( Mobile from, int spellIndex )
		{
			if ( Server.Misc.ResearchSettings.ResearchMaterials( from ) != null )
			{
				ResearchBag bag = (ResearchBag)( Server.Misc.ResearchSettings.ResearchMaterials( from ) );
				if ( Server.Misc.Research.GetPrepared( bag, spellIndex ) > 0 )
					return true;
			}
			return false;
		}

		public ResearchSpell( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			AncientSpellbook book = null;
				if ( ResearchSettings.BookCaster( Caster ) )
					book = ResearchSettings.GetAncientTome( Caster );

			if ( book != null && book.paper < 1 )
			{
				Caster.SendMessage( "You do not have enough pages in your book to cast this spell." );
				return false;
			}
			else if ( book != null && book.quill < 1 )
			{
				Caster.SendMessage( "You do not have enough quills for your book to cast this spell." );
				return false;
			}

			if ( CastingSkill( Caster ) < RequiredSkill )
			{
				Caster.SendMessage( "You must have at least " + RequiredSkill + " in magery or necromancy to cast this spell." );
				return false;
			}
			else if ( Caster.Mana < GetMana() )
			{
				Caster.SendMessage( "You must have at least " + GetMana() + " mana to cast this spell." );
				return false;
			}
			else if ( !SpellPrepared( Caster, spellIndex ) && !ResearchSettings.BookCaster( Caster ) )
			{
				Caster.SendMessage( "You do not have that spell prepared." );
				return false;
			}

			return true;
		}

		public override bool CheckFizzle()
		{
			if ( CastingSkill( Caster ) < RequiredSkill )
			{
				Caster.SendMessage( "You must have at least " + RequiredSkill + " in magery or necromancy to cast this spell." );
				return false;
			}
			else if ( Caster.Mana < GetMana() )
			{
				Caster.SendMessage( "You must have at least " + GetMana() + " mana to cast this spell." );
				return false;
			}
			else if ( !SpellPrepared( Caster, spellIndex ) && !ResearchSettings.BookCaster( Caster ) )
			{
				Caster.SendMessage( "You do not have that spell prepared." );
				return false;
			}

			return true;
		}

		public override int GetMana()
		{
			return RequiredMana ;
		}

		public override void SayMantra()
		{
			Caster.PublicOverheadMessage( MessageType.Regular, 0x3B2, false, Info.Mantra );
		}

		public override void DoHurtFizzle()
		{
			Caster.PlaySound( 0x1D6 );
		}

		public override void OnDisturb( DisturbType type, bool message )
		{
			base.OnDisturb( type, message );

			if ( message )
				Caster.PlaySound( 0x1D6 );
		}

		public override void OnBeginCast()
		{
			base.OnBeginCast();

			Caster.FixedEffect( 0x37C4, 10, 42, 4, 3 );
		}

		public override void GetCastSkills( out double min, out double max )
		{
			min = RequiredSkill;
			max = RequiredSkill + 40.0;
		}

		public int ComputePowerValue( int div )
		{
			return ComputePowerValue( Caster, div );
		}

		public static int ComputePowerValue( Mobile from, int div )
		{
			if ( from == null )
				return 0;

			int v = (int) Math.Sqrt( ( from.Karma * -1 ) + 20000 + ( ( from.Skills.Necromancy.Fixed + from.Skills.Magery.Fixed ) * 10 ) );

			return v / div;
		}
	}
}