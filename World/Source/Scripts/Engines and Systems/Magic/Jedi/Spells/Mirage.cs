using System;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;

namespace Server.Spells.Jedi
{
	public class Mirage : JediSpell
	{
		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 282,
					IconGraphic = 0x2BC1,
					Name = "Mirage",
					PowerWords = "Michal",
					Description = "This power allows the Jedi to create a projection of themselves that can distract foes from the Jedi themselves. This projection contains physical attributes as the psychic energy of the Jedi can maintain the illusion for up to about 3 minutes, depending on how powerful they are.",
					ManaCost = 12,
					TithingCost = 24,
					MinSkill = 30,
					TargetType = TargetFlags.None
				},
				-1,
				0
			);

		public int CirclePower = 3;
		public override int RequiredTithing{ get{ return SpellInfo.SpellDefinition.TithingCost; } }
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.75 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public Mirage( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if( (Caster.Followers + 3) > Caster.FollowersMax )
			{
				Caster.SendMessage( "You have too many followers to create a mirage." );
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
			else if ( (Caster.Followers + 3) > Caster.FollowersMax )
			{
				Caster.SendMessage( "You have too many followers to create a mirage." );
			}
			else if( TransformationSpellHelper.UnderTransformation( Caster, typeof( HorrificBeastSpell ) ) )
			{
				Caster.SendMessage( "You cannot create a mirage while you look like that." );
			}
			else if ( CheckSequence() )
			{
				new JediMirage( Caster ).MoveToWorld( Caster.Location, Caster.Map );
				Caster.Hidden = true;
			}

			FinishSequence();
		}
	}
}

namespace Server.Mobiles
{
	public class JediMirage : BaseCreature
	{
		private Mobile m_Caster;

		public JediMirage( Mobile caster ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
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
				AddItem( JediMirageItem( caster.Items[i] ) );
			}

			Warmode = true;

			Summoned = true;
			SummonMaster = caster;

			int min = 60;
			int max = (int)(Server.Spells.Jedi.JediSpell.GetJediDamage( m_Caster ) );
				if ( max < min ){ max = min; }

			int hits = Utility.RandomMinMax( min, max );

			SetHits( hits );

			SetDamage( 1, 1 );

			SetDamageType( ResistanceType.Physical, 100 );
			SetResistance( ResistanceType.Physical, 20, 40 );
			SetResistance( ResistanceType.Fire, 20, 40 );
			SetResistance( ResistanceType.Cold, 20, 40 );
			SetResistance( ResistanceType.Poison, 20, 40 );

			Fame = 0;
			Karma = 0;

			VirtualArmor = 1;

			ControlSlots = 3;

			TimeSpan duration = TimeSpan.FromSeconds( Server.Spells.Jedi.JediSpell.GetJediDamage( m_Caster ) / 2 );

			BuffInfo.RemoveBuff( m_Caster, BuffIcon.Mirage );
			BuffInfo.AddBuff( m_Caster, new BuffInfo( BuffIcon.Mirage, 1063530, duration, m_Caster ) );

			new UnsummonTimer( caster, this, duration ).Start();
			SummonEnd = DateTime.Now + duration;

			MirrorImage.AddClone( m_Caster );
		}

		public override bool IsHumanInTown() { return false; }

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );
			if ( m_Caster != null ){ m_Caster.DoHarmful( defender ); }
		}

		private Item JediMirageItem( Item item )
		{
			Item newItem = new Item( item.ItemID );
			newItem.Hue = item.Hue;
			newItem.Layer = item.Layer;

			return newItem;
		}

		public override bool DeleteCorpseOnDeath { get { return true; } }

		public override void OnDelete()
		{
			PlaySound( 0x0FD );
			this.FixedParticles( 0x375A, 10, 30, 5052, 0xB41, 0, EffectLayer.LeftFoot );
			BuffInfo.RemoveBuff( m_Caster, BuffIcon.Mirage );
			base.OnDelete();
		}

		public override void OnAfterDelete()
		{
			MirrorImage.RemoveClone( m_Caster );
			base.OnAfterDelete();
		}

		public override bool IsDispellable { get { return false; } }
		public override bool Commandable { get { return false; } }

		public JediMirage( Serial serial ) : base( serial )
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