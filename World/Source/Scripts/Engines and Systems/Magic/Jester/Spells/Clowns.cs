using System;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Targeting;

namespace Server.Spells.Jester
{
	public class Clowns : JesterSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 261,
					IconGraphic = 20751,
					Name = "Clowns",
					PowerWords = "Get ready for the show!",
					Description = "Fool your audience by creating illusions of yourself, which may help you sneak away or distract one from yourself. Your illusions will vary in duration and amount, depending on your jester skills.",
					ManaCost = 25,
					TithingCost = 50,
					MinSkill = 10,
					TargetType = TargetFlags.Beneficial
				},
				-1,
				0
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3.0 ); } }
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public Clowns( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if( (Caster.Followers + 1) > Caster.FollowersMax )
			{
				Caster.SendMessage( "You have too many followers to be clowning around." );
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			if ( Caster.Mounted )
			{
				Caster.SendLocalizedMessage( 1063132 ); // You cannot use this ability while mounted.
			}
			else if ( (Caster.Followers + 1) > Caster.FollowersMax )
			{
				Caster.SendMessage( "You have too many followers to be clowning around." );
			}
			else if( TransformationSpellHelper.UnderTransformation( Caster, typeof( HorrificBeastSpell ) ) )
			{
				Caster.SendMessage( "You cannot be clowning around while you look like that." );
			}
			else if ( CheckSequence() )
			{
				Effects.SendLocationParticles( EffectItem.Create( Caster.Location, Caster.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 0, 0, 5042, 0 );

				Caster.PlaySound( Caster.Female ? 780 : 1051 );
				Caster.Say( "*applauds*" );

				new Clown( Caster ).MoveToWorld( Caster.Location, Caster.Map );

				int qty = 0;

				if ( Caster.Skills[SkillName.Begging].Value >= Utility.RandomMinMax( 1, 200 ) ){ qty++; }
				if ( Caster.Skills[SkillName.Psychology].Value >= Utility.RandomMinMax( 1, 100 ) ){ qty++; }

				if ( qty > ( ( Caster.FollowersMax - Caster.Followers - 1 ) ) )
					qty = Caster.FollowersMax - Caster.Followers;

				if ( qty > 0 ){ new Clown( Caster ).MoveToWorld( Caster.Location, Caster.Map ); }
				if ( qty > 1 ){ new Clown( Caster ).MoveToWorld( Caster.Location, Caster.Map ); }
			}

			FinishSequence();
		}
	}
}

namespace Server.Mobiles
{
	public class Clown : BaseCreature
	{
		private Mobile m_Caster;

		public Clown( Mobile caster ) : base( AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4 )
		{
			m_Caster = caster;

			Body = caster.Body;

			Hue = caster.Hue;
			Female = caster.Female;

			Name = caster.Name;
			NameHue = caster.NameHue;

			Title = caster.Title;
			Kills = caster.Kills;

			HairItemID = caster.HairItemID;
			HairHue = caster.HairHue;

			FacialHairItemID = caster.FacialHairItemID;
			FacialHairHue = caster.FacialHairHue;

			for ( int i = 0; i < caster.Skills.Length; ++i )
			{
				Skills[i].Base = caster.Skills[i].Base;
				Skills[i].Cap = caster.Skills[i].Cap;
			}

			for( int i = 0; i < caster.Items.Count; i++ )
			{
				AddItem( ClownItem( caster.Items[i] ) );
			}

			Warmode = true;

			Summoned = true;
			SummonMaster = caster;

			ControlOrder = OrderType.Follow;
			ControlTarget = caster;

			TimeSpan duration = TimeSpan.FromSeconds( 30 + caster.Skills.Psychology.Fixed / 40 );

			new UnsummonTimer( caster, this, duration ).Start();
			SummonEnd = DateTime.Now + duration;

			MirrorImage.AddClone( m_Caster );
		}

		protected override BaseAI ForcedAI { get { return new ClownAI( this ); } }

		public override bool IsHumanInTown() { return false; }

		private Item ClownItem( Item item )
		{
			Item newItem = new Item( item.ItemID );
			newItem.Hue = item.Hue;
			newItem.Layer = item.Layer;

			return newItem;
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			Delete();
		}

		public override bool DeleteCorpseOnDeath { get { return true; } }

		public override void OnDelete()
		{
			switch ( Utility.Random( 6 ))
			{
				case 0: PlaySound( Female ? 780 : 1051 ); break;
				case 1: Animate( 32, 5, 1, true, false, 0 ); break;
				case 2: PlaySound( Female ? 794 : 1066 ); break;
				case 3: PlaySound( Female ? 801 : 1073 ); break;
				case 4: PlaySound( 792 ); break;
				case 5: PlaySound( Female ? 783 : 1054 ); break;
			};

			Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x3728, 10, 15, 5042 );

			base.OnDelete();
		}

		public override void OnAfterDelete()
		{
			MirrorImage.RemoveClone( m_Caster );
			base.OnAfterDelete();
		}

		public override bool IsDispellable { get { return false; } }
		public override bool Commandable { get { return false; } }

		public Clown( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

			writer.Write( m_Caster );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_Caster = reader.ReadMobile();

			MirrorImage.AddClone( m_Caster );
		}
	}
}

namespace Server.Mobiles
{
	public class ClownAI : BaseAI
	{
		public ClownAI( Clown m ) : base ( m )
		{
			m.CurrentSpeed = m.ActiveSpeed;
		}

		public override bool Think()
		{
			// Clones only follow their owners
			Mobile master = m_Mobile.SummonMaster;

			if ( master != null && master.Map == m_Mobile.Map && master.InRange( m_Mobile, m_Mobile.RangePerception ) )
			{
				int iCurrDist = (int)m_Mobile.GetDistanceToSqrt( master );
				bool bRun = (iCurrDist > 5);

				WalkMobileRange( master, 2, bRun, 0, 1 );
			}
			else
				WalkRandom( 2, 2, 1 );

			if ( Utility.RandomMinMax( 1, 10 ) == 1 )
			{
				switch ( Utility.Random( 6 ) )
				{
					case 0: m_Mobile.PlaySound( m_Mobile.Female ? 780 : 1051 ); break;
					case 1: m_Mobile.Animate( 32, 5, 1, true, false, 0 ); break;
					case 2: m_Mobile.PlaySound( m_Mobile.Female ? 794 : 1066 ); break;
					case 3: m_Mobile.PlaySound( m_Mobile.Female ? 801 : 1073 ); break;
					case 4: m_Mobile.PlaySound( 792 ); break;
					case 5: m_Mobile.PlaySound( m_Mobile.Female ? 783 : 1054 ); break;
				};
			}

			return true;
		}
		
		public override bool CanSearching { get { return false; } }
	}
}