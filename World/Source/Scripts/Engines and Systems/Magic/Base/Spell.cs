using System;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;
using Server.Misc;
using Server.Spells.Second;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using System.Collections.Generic;
using Server.Engines.PartySystem;
using Server.Spells.Bushido;
using Server.Gumps;
using Server.Spells.HolyMan;
using Server.Spells.Song;
using Server.Spells.Mystic;
using Server.Spells.Syth;
using Server.Spells.Jedi;
using Server.Spells.Jester;
using Server.Spells.Research;
using Server.Spells.Shinobi;
using Server.Spells.Elementalism;
using Server.Spells.DeathKnight;
using Server.Spells.Chivalry;

namespace Server.Spells
{
	public abstract class Spell : ISpell
	{
		private Mobile m_Caster;
		private Item m_Scroll;
		private SpellInfo m_Info;
		private SpellState m_State;
		private DateTime m_StartCastTime;

		public SpellState State{ get{ return m_State; } set{ m_State = value; } }
		public Mobile Caster{ get{ return m_Caster; } }
		public SpellInfo Info{ get{ return m_Info; } }
		public string Name{ get{ return m_Info.Name; } }
		public string Mantra{ get{ return m_Info.Mantra; } }
		public Type[] Reagents{ get{ return m_Info.Reagents; } }
		public Item Scroll{ get{ return m_Scroll; } }
		public DateTime StartCastTime { get { return m_StartCastTime; } }

		private static TimeSpan NextSpellDelay = TimeSpan.FromSeconds( 0.75 );
		private static TimeSpan AnimateDelay = TimeSpan.FromSeconds( 1.5 );

		public virtual SkillName CastSkill{ get{ return SkillName.Magery; } }
		public virtual SkillName DamageSkill{ get{ return SkillName.Psychology; } }

		public virtual bool RevealOnCast{ get{ return true; } }
		public virtual bool ClearHandsOnCast{ get{ return true; } }
		public virtual bool ShowHandMovement{ get{ return true; } }

		public virtual bool DelayedDamage{ get{ return false; } }

        public virtual bool DelayedDamageStacking { get { return true; } }
        //In reality, it's ANY delayed Damage spell Post-AoS that can't stack, but, only 
        //Expo & Magic Arrow have enough delay and a short enough cast time to bring up 
        //the possibility of stacking 'em.  Note that a MA & an Explosion will stack, but
		//of course, two MA's won't.

		private static Dictionary<Type, DelayedDamageContextWrapper> m_ContextTable = new Dictionary<Type, DelayedDamageContextWrapper>();

		private class DelayedDamageContextWrapper
		{
			private Dictionary<Mobile, Timer> m_Contexts = new Dictionary<Mobile, Timer>();

			public void Add( Mobile m, Timer t )
			{
				Timer oldTimer;
				if( m_Contexts.TryGetValue( m, out oldTimer ) )
				{
					oldTimer.Stop();
					m_Contexts.Remove( m );
				}

				m_Contexts.Add( m, t );
			}

			public void Remove( Mobile m )
			{
				m_Contexts.Remove( m );
			}
		}

		public void StartDelayedDamageContext( Mobile m, Timer t )
		{
			if( DelayedDamageStacking )
				return; //Sanity

			DelayedDamageContextWrapper contexts;

			if( !m_ContextTable.TryGetValue( GetType(), out contexts ) )
			{
				contexts = new DelayedDamageContextWrapper();
				m_ContextTable.Add( GetType(), contexts );
			}

			contexts.Add( m, t );
		}

		public void RemoveDelayedDamageContext( Mobile m )
		{
			DelayedDamageContextWrapper contexts;

			if( !m_ContextTable.TryGetValue( GetType(), out contexts ) )
				return;

			contexts.Remove( m );
		}

		public static double ItemSkillValue( Mobile caster, SkillName skill, bool fxd )
		{
			double var = caster.Skills[skill].Value;
			double cir = 100.0;

			if ( fxd )
			{
				var = caster.Skills[skill].Fixed;
				cir = 1000.0;
			}

			if ( caster.ItemCastSpell && var < cir )
				var = cir;

			return var;
		}

		public void HarmfulSpell( Mobile m )
		{
			if ( m is BaseCreature )
				((BaseCreature)m).OnHarmfulSpell( m_Caster );
		}

		public Spell( Mobile caster, Item scroll, SpellInfo info )
		{
			m_Caster = caster;
			m_Scroll = scroll;
			m_Info = info;

			caster.ItemCastSpell = false;
			caster.ScrollCastSpell = false;
			caster.NoManaUseSpell = false;
			if ( caster is PlayerMobile && m_Scroll != null )
			{
				if ( m_Scroll != null )
					caster.ItemCastSpell = true;
				if ( m_Scroll is SpellScroll || m_Scroll is MagicRuneBag )
					caster.ScrollCastSpell = true;
				if ( m_Scroll.EnchantUsesMax == 0 && m_Scroll.Enchanted != MagicSpell.None )
					caster.NoManaUseSpell = true;
			}
		}

		public virtual int GetNewAosDamage( int bonus, int dice, int sides, Mobile singleTarget )
		{
			if( singleTarget != null )
			{
				return GetNewAosDamage( bonus, dice, sides, (Caster.Player && singleTarget.Player), GetDamageScalar( singleTarget ) );
			}
			else
			{
				return GetNewAosDamage( bonus, dice, sides, false );
			}
		}

		public virtual int GetNewAosDamage( int bonus, int dice, int sides, bool playerVsPlayer )
		{
			return GetNewAosDamage( bonus, dice, sides, playerVsPlayer, 1.0 );
		}

		public static bool isFriendly( Mobile caster, Mobile m )
		{
			Party p = Engines.PartySystem.Party.Get( caster );

			if ( !caster.CanBeBeneficial( m, false ) )
				return false;

			if ( m is Golem )
				return false;

			if ( caster == m )
				return true;

			if ( m is BaseCreature && ((BaseCreature)m).GetMaster() == caster )
				return true;

			if ( p != null )
			{
				foreach ( PartyMemberInfo pmi in p.Members )
				{
					if ( pmi.Mobile is PlayerMobile && pmi.Mobile.InRange( caster.Location, 10 ) && pmi.Mobile == m )
						return true;
				}
			}

			return false;
		}

		public virtual int GetNewAosDamage( int bonus, int dice, int sides, bool playerVsPlayer, double scalar )
		{
			int damage = Utility.Dice( dice, sides, bonus ) * 100;
			int damageBonus = 0;

			int inscribeSkill = GetInscribeFixed( m_Caster );
			int inscribeBonus = (inscribeSkill + (1000 * (inscribeSkill / 1000))) / 200;
			damageBonus += inscribeBonus;

			if ( inscribeSkill >= 1000 )
				damageBonus += 10;

			int intBonus = Caster.Int / 3;
			damageBonus += intBonus;

			int sdiBonus = AosAttributes.GetValue( m_Caster, AosAttribute.SpellDamage );

			if ( MyServerSettings.SpellDamageIncreaseVsMonsters() > 0 && sdiBonus > MyServerSettings.SpellDamageIncreaseVsMonsters() )
				sdiBonus = MyServerSettings.SpellDamageIncreaseVsMonsters();

			// PvP spell damage increase cap of 15% from an item�s magic property
			if ( playerVsPlayer && MyServerSettings.SpellDamageIncreaseVsPlayers() > 0 && sdiBonus > MyServerSettings.SpellDamageIncreaseVsPlayers() )
				sdiBonus = MyServerSettings.SpellDamageIncreaseVsPlayers();

			damageBonus += sdiBonus;

			TransformContext context = TransformationSpellHelper.GetContext( Caster );

			damage = AOS.Scale( damage, 100 + damageBonus );

			int evalSkill = GetDamageFixed( m_Caster );
			int evalScale = 30 + ((9 * evalSkill) / 100);

			damage = AOS.Scale( damage, evalScale );

			damage = AOS.Scale( damage, (int)(scalar*100) );

			return damage / 100;
		}

		public virtual bool IsCasting{ get{ return m_State == SpellState.Casting; } }

		public virtual void OnCasterHurt()
		{
			if ( !Caster.Player )
				return;

			if ( IsCasting )
			{
				object o = ProtectionSpell.Registry[m_Caster];

				if ( o == null )
					o = Elemental_Protection_Spell.Registry[m_Caster];

				bool disturb = true;

				if ( o != null && o is double )
				{
					if ( ((double)o) > Utility.RandomDouble()*100.0 )
						disturb = false;
				}

				if ( disturb )
					Disturb( DisturbType.Hurt, false, true );
			}
		}

		public virtual void OnCasterKilled()
		{
			Disturb( DisturbType.Kill );
		}

		public virtual void OnConnectionChanged()
		{
			FinishSequence();
		}

		public virtual bool OnCasterMoving( Direction d )
		{
			if ( IsCasting && BlocksMovement )
			{
				m_Caster.SendLocalizedMessage( 500111 ); // You are frozen and can not move.
				return false;
			}

			return true;
		}

		public virtual bool OnCasterEquiping( Item item )
		{
			if ( IsCasting )
				Disturb( DisturbType.EquipRequest );

			return true;
		}

		public virtual bool OnCasterUsingObject( object o )
		{
			if ( m_State == SpellState.Sequencing )
				Disturb( DisturbType.UseRequest );

			return true;
		}

		public virtual bool OnCastInTown( Region r )
		{
			return m_Info.AllowTown;
		}

		public virtual bool ConsumeReagents()
        {
			if ( !m_Caster.Player )
				return true;

			if ( m_Caster.ItemCastSpell )
				return true;

			if ( AosAttributes.GetValue( m_Caster, AosAttribute.LowerRegCost ) > Utility.Random( 100 ) )
				return true;

			Container pack = m_Caster.Backpack;

			if ( pack == null )
				return false;

			if ( pack.ConsumeTotal( m_Info.Reagents, m_Info.Amounts ) == -1 )
				return true;

			return false;
		}

		public virtual double GetInscribeSkill( Mobile m )
		{
			return m.Skills[SkillName.Inscribe].Value;
		}

		public virtual int GetInscribeFixed( Mobile m )
		{
			return m.Skills[SkillName.Inscribe].Fixed;
		}

		public virtual int GetDamageFixed( Mobile m )
		{
			return (int)Spell.ItemSkillValue( m, DamageSkill, true );
		}

		public virtual double GetDamageSkill( Mobile m )
		{
			return Spell.ItemSkillValue( m, DamageSkill, false );
		}

		public virtual double GetResistSkill( Mobile m )
		{
			return m.Skills[SkillName.MagicResist].Value;
		}

		public virtual double GetDamageScalar( Mobile target )
		{
			double scalar = 1.0;

			if ( target is BaseCreature )
				((BaseCreature)target).AlterDamageScalarFrom( m_Caster, ref scalar );

			if ( m_Caster is BaseCreature )
				((BaseCreature)m_Caster).AlterDamageScalarTo( target, ref scalar );

			scalar *= GetSlayerDamageScalar( target );

			target.Region.SpellDamageScalar( m_Caster, target, ref scalar );

			if( Evasion.CheckSpellEvasion( target ) )	//Only single target spells an be evaded
				scalar = 0;

			return scalar;
		}

		public virtual double GetSlayerDamageScalar( Mobile defender )
		{
			Spellbook atkBook = Spellbook.FindEquippedSpellbook( m_Caster );
			double scalar = 1.0;

			if( atkBook != null )
			{
				SlayerEntry atkSlayer = SlayerGroup.GetEntryByName( atkBook.Slayer );
				SlayerEntry atkSlayer2 = SlayerGroup.GetEntryByName( atkBook.Slayer2 );

				if( atkSlayer != null && atkSlayer.Slays( defender ) || atkSlayer2 != null && atkSlayer2.Slays( defender ) )
				{
					defender.FixedEffect( 0x37B9, 10, 5 );	//TODO: Confirm this displays on OSIs
					scalar = 2.0;
				}

				TransformContext context = TransformationSpellHelper.GetContext( defender );

				if( (atkBook.Slayer == SlayerName.Silver || atkBook.Slayer2 == SlayerName.Silver) && context != null && context.Type != typeof( HorrificBeastSpell ) )
					scalar +=.25; // Every necromancer transformation other than horrific beast take an additional 25% damage

				if( scalar != 1.0 )
					return scalar;
			}

			ISlayer defISlayer = Spellbook.FindEquippedSpellbook( defender );

			if( defISlayer == null )
				defISlayer = defender.Weapon as ISlayer;

			if( defISlayer != null )
			{
				SlayerEntry defSlayer = SlayerGroup.GetEntryByName( defISlayer.Slayer );
				SlayerEntry defSlayer2 = SlayerGroup.GetEntryByName( defISlayer.Slayer2 );

				if( defSlayer != null && defSlayer.Group.OppositionSuperSlays( m_Caster ) || defSlayer2 != null && defSlayer2.Group.OppositionSuperSlays( m_Caster ) )
					scalar = 2.0;
			}

			return scalar;
		}

		public virtual void DoFizzle()
		{
			m_Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 502632 ); // The spell fizzles.

			if ( m_Caster.Player )
			{
				m_Caster.FixedParticles( 0x3735, 1, 30, 9503, EffectLayer.Waist );
				m_Caster.PlaySound( 0x5C );
			}
		}

		private CastTimer m_CastTimer;
		private AnimTimer m_AnimTimer;

		public void Disturb( DisturbType type )
		{
			Disturb( type, true, false );
		}

		public virtual bool CheckDisturb( DisturbType type, bool firstCircle, bool resistable )
		{
			return true;
		}

		public void Disturb( DisturbType type, bool firstCircle, bool resistable )
		{
			if ( !CheckDisturb( type, firstCircle, resistable ) )
				return;

			if ( m_State == SpellState.Casting )
			{
				if( !firstCircle && !Core.AOS && this is MagerySpell &&  ((MagerySpell)this).Circle == SpellCircle.First )
					return;

				m_State = SpellState.None;
				m_Caster.Spell = null;

				OnDisturb( type, true );

				if ( m_CastTimer != null )
					m_CastTimer.Stop();

				if ( m_AnimTimer != null )
					m_AnimTimer.Stop();

				if ( m_Caster.Player && type == DisturbType.Hurt )
					DoHurtFizzle();

				m_Caster.NextSpellTime = DateTime.Now + GetDisturbRecovery();
			}
			else if ( m_State == SpellState.Sequencing )
			{
				if( !firstCircle && !Core.AOS && this is MagerySpell &&  ((MagerySpell)this).Circle == SpellCircle.First )
					return;

				m_State = SpellState.None;
				m_Caster.Spell = null;

				OnDisturb( type, false );

				Targeting.Target.Cancel( m_Caster );

				if ( m_Caster.Player && type == DisturbType.Hurt )
					DoHurtFizzle();
			}
		}

		public virtual void DoHurtFizzle()
		{
			m_Caster.FixedEffect( 0x3735, 6, 30 );
			m_Caster.PlaySound( 0x5C );
		}

		public virtual void OnDisturb( DisturbType type, bool message )
		{
			if ( message )
			{
				if ( this is HolyManSpell )
				{
					m_Caster.SendMessage( "Your concentration is disturbed, thus ruining thy prayer." );
				}
				else if ( this is MysticSpell || this is JesterSpell )
				{
					m_Caster.SendMessage( "Your concentration is disturbed, thus ruining thy attempt." );
				}
				else
				{
					m_Caster.SendLocalizedMessage( 500641 ); // Your concentration is disturbed, thus ruining thy spell.
				}
			}
		}

		public virtual bool CheckCast()
		{
			return true;
		}

		public virtual void SayMantra()
		{
			if ( m_Info.Mantra != null && m_Info.Mantra.Length > 0 )
				m_Caster.PublicOverheadMessage( MessageType.Spell, m_Caster.SpeechHue, true, m_Info.Mantra, false );
		}

		public virtual bool BlockedByHorrificBeast{ get{ return true; } }
		public virtual bool BlockedByAnimalForm{ get{ return true; } }
		public virtual bool BlocksMovement{ get{ return true; } }

		public virtual bool CheckNextSpellTime{ get{ return true; } }

		public bool Cast()
		{
			m_StartCastTime = DateTime.Now;

			if ( Core.AOS && m_Caster.Spell is Spell && ((Spell)m_Caster.Spell).State == SpellState.Sequencing )
				((Spell)m_Caster.Spell).Disturb( DisturbType.NewCast );

			if ( m_Caster.Blessed )
			{
				m_Caster.SendMessage( "You cannot do that while in this state." );
				return false;
			}
			if ( !CanCastSpell( m_Caster, this ) )
			{
				m_Caster.SendMessage( "The darkness of the Underworld seems to be affecting this spell." );
				return false;
			}
			if ( !CantMixSpell( m_Caster, this ) )
			{
				m_Caster.SendMessage( "Elementalism, with magery or necromancy, are affecting your magic." );
				return false;
			}
			else if ( !m_Caster.CheckAlive() )
			{
				return false;
			}
			else if ( m_Caster.Spell != null && m_Caster.Spell.IsCasting )
			{
				m_Caster.SendLocalizedMessage( 502642 ); // You are already casting a spell.
			}
			else if ( BlockedByHorrificBeast && TransformationSpellHelper.UnderTransformation( m_Caster, typeof( HorrificBeastSpell ) ) || ( BlockedByAnimalForm && AnimalForm.UnderTransformation( m_Caster ) ))
			{
				m_Caster.SendLocalizedMessage( 1061091 ); // You cannot cast that spell in this form.
			}
			else if ( ( m_Caster.Paralyzed || m_Caster.Frozen ) && !( this is FerretFlee ) )
			{
				m_Caster.SendLocalizedMessage( 502643 ); // You can not cast a spell while frozen.
			}
			else if ( CheckNextSpellTime && DateTime.Now < m_Caster.NextSpellTime )
			{
				m_Caster.SendLocalizedMessage( 502644 ); // You have not yet recovered from casting a spell.
			}
			else if ( m_Caster is PlayerMobile && ( (PlayerMobile) m_Caster ).PeacedUntil > DateTime.Now )
			{
				m_Caster.SendLocalizedMessage( 1072060 ); // You cannot cast a spell while calmed.
			}
			else if ( m_Caster.Mana >= ScaleMana( GetMana() ) )
			{
				if ( m_Caster.Spell == null && m_Caster.CheckSpellCast( this ) && CheckCast() && m_Caster.Region.OnBeginSpellCast( m_Caster, this ) )
				{
					m_State = SpellState.Casting;
					m_Caster.Spell = this;

					if ( RevealOnCast )
						m_Caster.RevealingAction();

					SayMantra();

					TimeSpan castDelay = this.GetCastDelay();

					if ( ShowHandMovement && m_Caster.Body.IsHuman )
					{
						int count = (int)Math.Ceiling( castDelay.TotalSeconds / AnimateDelay.TotalSeconds );

						if ( count != 0 )
						{
							m_AnimTimer = new AnimTimer( this, count );
							m_AnimTimer.Start();
						}

						if ( m_Info.LeftHandEffect > 0 )
							Caster.FixedParticles( 0, 10, 5, m_Info.LeftHandEffect, EffectLayer.LeftHand );

						if ( m_Info.RightHandEffect > 0 )
							Caster.FixedParticles( 0, 10, 5, m_Info.RightHandEffect, EffectLayer.RightHand );
					}

					if ( m_Caster.ItemCastSpell && !m_Caster.ScrollCastSpell ){}
						else if ( ClearHandsOnCast )
							m_Caster.ClearHands();

					WeaponAbility.ClearCurrentAbility( m_Caster );

					m_CastTimer = new CastTimer( this, castDelay );
					m_CastTimer.Start();

					OnBeginCast();

					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				m_Caster.LocalOverheadMessage( MessageType.Regular, 0x22, 502625 ); // Insufficient mana
			}

			return false;
		}

		public abstract void OnCast();

		public virtual void OnBeginCast()
		{
		}

		public static bool CanCastSpell( Mobile m, ISpell s )
		{
			if ( m is PlayerMobile )
			{
				if ( m.Land == Land.Underworld && ( s is MagerySpell || s is ResearchSpell || s is ElementalSpell ) && Utility.RandomMinMax(1,3) != 1 )
				{
					Item relic1 = m.FindItemOnLayer( Layer.Trinket );
					Item relic2 = m.FindItemOnLayer( Layer.Ring );
					Item relic3 = m.FindItemOnLayer( Layer.Neck );
					Item relic4 = m.FindItemOnLayer( Layer.Bracelet );
					Item relic5 = m.FindItemOnLayer( Layer.Earrings );

					bool fizzle = true;

					if ( relic1 is OrbOfTheAbyss ){ fizzle = false; }
					else if ( relic1 is CodexWisdom ){ fizzle = false; }
					else if ( relic2 is OrbOfTheAbyss ){ fizzle = false; }
					else if ( relic3 is OrbOfTheAbyss ){ fizzle = false; }
					else if ( relic4 is OrbOfTheAbyss ){ fizzle = false; }
					else if ( relic5 is OrbOfTheAbyss ){ fizzle = false; }
					else if ( relic1 is Artifact_PyrosGrimoire ){ fizzle = false; }
					else if ( relic1 is Artifact_StratosManual ){ fizzle = false; }
					else if ( relic1 is Artifact_HydrosLexicon ){ fizzle = false; }
					else if ( relic1 is Artifact_LithosTome ){ fizzle = false; }

					if ( fizzle )
					{
						Server.Misc.IntelligentAction.FizzleSpell( m );
						return false;
					}
				}
			}
			return true;
		}

		public static bool CantMixSpell( Mobile m, ISpell s )
		{
			bool canCast = true;

			if ( m.ItemCastSpell && !m.ScrollCastSpell )
				return true;

			if ( m is PlayerMobile )
			{
				if ( m.Skills[SkillName.Necromancy].Value >= 1 && m.Skills[SkillName.Elementalism].Value >= 1 && ( s is ElementalSpell || s is NecromancerSpell ) )
					canCast = false;
				if ( m.Skills[SkillName.Magery].Value >= 1 && m.Skills[SkillName.Elementalism].Value >= 1 && ( s is ElementalSpell || s is MagerySpell ) )
					canCast = false;
				if ( m.Skills[SkillName.Elementalism].Value >= 1 && s is ResearchSpell )
					canCast = false;
			}

			if ( !canCast )
				Server.Misc.IntelligentAction.FizzleSpell( m );

			return canCast;
		}

		public virtual void GetCastSkills( out double min, out double max )
		{
			min = max = 0;	//Intended but not required for overriding.
		}

		public virtual bool CheckFizzle()
		{
			if ( Caster.ItemCastSpell )
				return true;

			double minSkill, maxSkill;

			GetCastSkills( out minSkill, out maxSkill );

			if ( DamageSkill != CastSkill )
				Caster.CheckSkill( DamageSkill, 0.0, Caster.Skills[ DamageSkill ].Cap );

			return Caster.CheckSkill( CastSkill, minSkill, maxSkill );
		}

		public abstract int GetMana();

		public virtual int ScaleMana( int mana )
		{
			if ( Caster.NoManaUseSpell )
				return 0;

			double scalar = 1.0;

			if ( !Necromancy.MindRotSpell.GetMindRotScalar( Caster, ref scalar ) )
				scalar = 1.0;

			int lmc = AosAttributes.GetValue( m_Caster, AosAttribute.LowerManaCost );
			if ( lmc > MyServerSettings.LowerMana() )
				lmc = MyServerSettings.LowerMana();

			scalar -= (double)lmc / 100;

			return (int)(mana * scalar);
		}

		public virtual TimeSpan GetDisturbRecovery()
		{
			return TimeSpan.Zero;
		}

		public virtual int CastRecoveryBase{ get{ return 6; } }
		public virtual int CastRecoveryFastScalar{ get{ return 1; } }
		public virtual int CastRecoveryPerSecond{ get{ return 4; } }
		public virtual int CastRecoveryMinimum{ get{ return 0; } }

		public virtual TimeSpan GetCastRecovery()
		{
			int fcr = AosAttributes.GetValue( m_Caster, AosAttribute.CastRecovery );

			int fcrDelay = -(CastRecoveryFastScalar * fcr);

			int delay = CastRecoveryBase + fcrDelay;

			if ( delay < CastRecoveryMinimum )
				delay = CastRecoveryMinimum;

			return TimeSpan.FromSeconds( (double)delay / CastRecoveryPerSecond );
		}

		public abstract TimeSpan CastDelayBase { get; }

		public virtual double CastDelayFastScalar { get { return 1; } }
		public virtual double CastDelaySecondsPerTick { get { return 0.25; } }
		public virtual TimeSpan CastDelayMinimum { get { return TimeSpan.FromSeconds( 0.25 ); } }

		public virtual TimeSpan GetCastDelay()
		{
			// Faster casting cap of 2 (if not using the protection spell) 
			// Faster casting cap of 0 (if using the protection spell) 
			// Paladin spells are subject to a faster casting cap of 4 
			// Paladins with magery of 70.0 or above are subject to a faster casting cap of 2 
			int fcMax = 4;

			if ( CastSkill == SkillName.Elementalism || CastSkill == SkillName.Magery || CastSkill == SkillName.Necromancy || ( CastSkill == SkillName.Knightship && m_Caster.Skills[SkillName.Magery].Value >= 70.0 ) )
				fcMax = 2;

			int fc = AosAttributes.GetValue( m_Caster, AosAttribute.CastSpeed );

			if ( fc > fcMax )
				fc = fcMax;

			if ( ProtectionSpell.Registry.Contains( m_Caster ) )
				fc -= 2;

			TimeSpan baseDelay = CastDelayBase;

			TimeSpan fcDelay = TimeSpan.FromSeconds( -(CastDelayFastScalar * fc * CastDelaySecondsPerTick) );

			TimeSpan delay = baseDelay + fcDelay;

			if ( delay < CastDelayMinimum )
				delay = CastDelayMinimum;

			return delay;
		}

		public virtual void FinishSequence()
		{
			m_State = SpellState.None;

			if ( m_Caster.Spell == this )
				m_Caster.Spell = null;

			Server.Gumps.RegBar.RefreshRegBar( m_Caster );
		}

		public virtual int ComputeKarmaAward()
		{
			return 0;
		}

		public virtual bool CheckSequence()
		{
			int mana = ScaleMana( GetMana() );

			if ( m_Caster.Deleted || !m_Caster.Alive || m_Caster.Spell != this || m_State != SpellState.Sequencing )
			{
				DoFizzle();
			}
			else if ( m_Scroll is SpellScroll && ( m_Scroll.Amount <= 0 || m_Scroll.Deleted || m_Scroll.RootParent != m_Caster ) )
			{
				DoFizzle();
			}
			else if ( !ConsumeReagents() )
			{
				m_Caster.LocalOverheadMessage( MessageType.Regular, 0x22, 502630 ); // More reagents are needed for this spell.
			}
			else if ( m_Caster.Mana < mana )
			{
				m_Caster.LocalOverheadMessage( MessageType.Regular, 0x22, 502625 ); // Insufficient mana for this spell.
			}
			else if ( !( this is FerretFlee ) && Core.AOS && (m_Caster.Frozen || m_Caster.Paralyzed) )
			{
				m_Caster.SendLocalizedMessage( 502646 ); // You cannot cast a spell while frozen.
				DoFizzle();
			}
			else if ( m_Caster is PlayerMobile && ((PlayerMobile) m_Caster).PeacedUntil > DateTime.Now )
			{
				m_Caster.SendLocalizedMessage( 1072060 ); // You cannot cast a spell while calmed.
				DoFizzle();
			}
			else if ( CheckFizzle() )
			{
				m_Caster.Mana -= mana;

				if ( this is PaladinSpell || this is DeathKnightSpell )
					m_Caster.Stam -= mana;

				if ( this is ElementalSpell ){ m_Caster.Stam -= ((ElementalSpell)this).GetStam(); }

				if ( m_Scroll is SpellScroll )
				{
					m_Scroll.Consume();
					if ( m_Scroll.Catalog == Catalogs.Potion )
						m_Caster.AddToBackpack( new Jar() );
				}
				else if ( m_Scroll != null && m_Scroll.Enchanted != MagicSpell.None )
					m_Scroll.ConsumeEnchants( 1 );

				if ( m_Caster.ItemCastSpell && !m_Caster.ScrollCastSpell ){}
					else if ( ClearHandsOnCast )
						m_Caster.ClearHands();

				int karma = ComputeKarmaAward();

				if ( karma != 0 )
					Misc.Titles.AwardKarma( Caster, karma, true );

				if( TransformationSpellHelper.UnderTransformation( m_Caster, typeof( VampiricEmbraceSpell ) ) )
				{
					bool garlic = false;

					for ( int i = 0; !garlic && i < m_Info.Reagents.Length; ++i )
						garlic = ( m_Info.Reagents[i] == Reagent.Garlic );

					if ( garlic )
					{
						m_Caster.SendLocalizedMessage( 1061651 ); // The garlic burns you!
						AOS.Damage( m_Caster, Utility.RandomMinMax( 17, 23 ), 100, 0, 0, 0, 0 );
					}
				}

				return true;
			}
			else
			{
				DoFizzle();
			}

			return false;
		}

		public bool CheckBSequence( Mobile target )
		{
			return CheckBSequence( target, false );
		}

		public bool CheckBSequence( Mobile target, bool allowDead )
		{
			if ( !target.Alive && !allowDead )
			{
				m_Caster.SendLocalizedMessage( 501857 ); // This spell won't work on that!
				return false;
			}
			else if ( Caster.CanBeBeneficial( target, true, allowDead ) && CheckSequence() )
			{
				Caster.DoBeneficial( target );
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool CheckHSequence( Mobile target )
		{
			if ( !target.Alive )
			{
				m_Caster.SendLocalizedMessage( 501857 ); // This spell won't work on that!
				return false;
			}
			else if ( Caster.CanBeHarmful( target ) && CheckSequence() )
			{
				Caster.DoHarmful( target );
				return true;
			}
			else
			{
				return false;
			}
		}

		private class AnimTimer : Timer
		{
			private Spell m_Spell;

			public AnimTimer( Spell spell, int count ) : base( TimeSpan.Zero, AnimateDelay, count )
			{
				m_Spell = spell;

				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				if ( m_Spell.State != SpellState.Casting || m_Spell.m_Caster.Spell != m_Spell )
				{
					Stop();
					return;
				}

				if ( !m_Spell.Caster.Mounted && m_Spell.Caster.Body.IsHuman && m_Spell.m_Info.Action >= 0 )
					m_Spell.Caster.Animate( m_Spell.m_Info.Action, 7, 1, true, false, 0 );

				if ( !Running )
					m_Spell.m_AnimTimer = null;
			}
		}

		private class CastTimer : Timer
		{
			private Spell m_Spell;

			public CastTimer( Spell spell, TimeSpan castDelay ) : base( castDelay )
			{
				m_Spell = spell;

				Priority = TimerPriority.TwentyFiveMS;
			}

			protected override void OnTick()
			{
				if ( m_Spell.m_State == SpellState.Casting && m_Spell.m_Caster.Spell == m_Spell )
				{
					m_Spell.m_State = SpellState.Sequencing;
					m_Spell.m_CastTimer = null;
					m_Spell.m_Caster.OnSpellCast( m_Spell );
					m_Spell.m_Caster.Region.OnSpellCast( m_Spell.m_Caster, m_Spell );
					m_Spell.m_Caster.NextSpellTime = DateTime.Now + m_Spell.GetCastRecovery();// Spell.NextSpellDelay;

					Target originalTarget = m_Spell.m_Caster.Target;

					m_Spell.OnCast();

					if ( m_Spell.m_Caster.Player && m_Spell.m_Caster.Target != originalTarget && m_Spell.Caster.Target != null )
						m_Spell.m_Caster.Target.BeginTimeout( m_Spell.m_Caster, TimeSpan.FromSeconds( 30.0 ) );

					m_Spell.m_CastTimer = null;
				}
			}
		}
	}
}