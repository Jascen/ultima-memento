using System;

namespace Server.Items
{
	public interface IThrowingGloves
	{
		ThrowingWeaponType GloveType { get; set; }
	}

	[FlipableAttribute( 0x13C6, 0x13CE )]
	public partial class ThrowingGloves : BaseRanged, IThrowingGloves
	{
		private ThrowingWeaponType m_GloveType;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public ThrowingWeaponType GloveType { get { return m_GloveType; } set { m_GloveType = value; InvalidateProperties(); } }

		public override int EffectID { get { return ThrowingGloves.GetEffectID( GloveType ); } }

		public override Type AmmoType{ get{ return typeof( ThrowingWeapon ); } }
		public override Item Ammo{ get{ return new ThrowingWeapon(); } }

		public override WeaponAbility PrimaryAbility { get { return ThrowingGloves.GetPrimaryAbility( GloveType ); } }
		public override WeaponAbility SecondaryAbility { get { return ThrowingGloves.GetSecondaryAbility( GloveType ); } }
		public override WeaponAbility ThirdAbility { get { return ThrowingGloves.GetThirdAbility( GloveType ); } }
		public override WeaponAbility FourthAbility { get { return ThrowingGloves.GetFourthAbility( GloveType ); } }
		public override WeaponAbility FifthAbility { get { return ThrowingGloves.GetFifthAbility( GloveType ); } }

		public override int AosStrengthReq{ get{ return 20; } }
		public override int AosMinDamage{ get{ return 8; } }
		public override int AosMaxDamage{ get{ return 11; } }
		public override float MlSpeed{ get{ return 2.00f; } }

		public override int DefMaxRange{ get{ return 6; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 60; } }
		public override int DefHitSound{ get{ return 0x5D2; } }
		public override int DefMissSound{ get{ return 0x5D3; } }

		public override SkillName DefSkill{ get{ return SkillName.Marksmanship; } }
		public override WeaponType DefType{ get{ return WeaponType.Ranged; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Punching; } }

		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		[Constructable]
		public ThrowingGloves() : base( 0x13C6 )
		{
			GloveType = ThrowingWeaponType.Stones;
			Name = "throwing gloves";
			Weight = 2.0;
			Hue = Utility.RandomColor(0);
			Layer = Layer.OneHanded;
			Attributes.SpellChanneling = 1;
			Resource = CraftResource.RegularLeather;
		}

		public override void OnDoubleClick( Mobile from )
		{
			ThrowingGloves.ChangeGloveType( from, this );
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			list.Add("Throwing: {0}", ThrowingGloves.GetAmmoName( GloveType ));
			list.Add( 1049644, "Double click to change ammo type" );
			list.Add( 1070722, "Cannot be used with other weapons" );
		}

		public ThrowingGloves( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
            writer.Write( (int)GloveType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( 0 < version ){ GloveType = (ThrowingWeaponType)reader.ReadInt(); }
			else
			{
            	var _ = reader.ReadString(); // We don't care about converting this. Just default them to Stones.
			}
		}
	}

	public partial class ThrowingGloves
	{
		public static int GetEffectID( ThrowingWeaponType GloveType )
		{
			switch ( GloveType )
			{
				case ThrowingWeaponType.Stones: return 0x10B6;
				case ThrowingWeaponType.Axes: return 0x10B3;
				case ThrowingWeaponType.Daggers: return 0x529F;
				case ThrowingWeaponType.Darts: return 0x52B0;
				case ThrowingWeaponType.Cards: return 0x4C29;
				case ThrowingWeaponType.Tomatoes: return 0x4C28;
				default: return 0x10B2;
			}
		}

		public static WeaponAbility GetPrimaryAbility( ThrowingWeaponType GloveType )
		{
			switch ( GloveType )
			{
				case ThrowingWeaponType.Stones: return WeaponAbility.ConcussionBlow;
				case ThrowingWeaponType.Axes: return WeaponAbility.ArmorIgnore;
				case ThrowingWeaponType.Daggers: return WeaponAbility.ArmorIgnore;
				case ThrowingWeaponType.Darts: return WeaponAbility.ParalyzingBlow;
				case ThrowingWeaponType.Cards: return WeaponAbility.ArmorIgnore;
				case ThrowingWeaponType.Tomatoes: return WeaponAbility.ConcussionBlow;
				default: return WeaponAbility.ShadowStrike;
			}
		}

		public static WeaponAbility GetSecondaryAbility( ThrowingWeaponType GloveType )
		{
			switch ( GloveType )
			{
				case ThrowingWeaponType.Stones: return WeaponAbility.StunningStrike;
				case ThrowingWeaponType.Axes: return WeaponAbility.TalonStrike;
				case ThrowingWeaponType.Daggers: return WeaponAbility.TalonStrike;
				case ThrowingWeaponType.Darts: return WeaponAbility.ArmorIgnore;
				case ThrowingWeaponType.Cards: return WeaponAbility.TalonStrike;
				case ThrowingWeaponType.Tomatoes: return WeaponAbility.StunningStrike;
				default: return WeaponAbility.ParalyzingBlow;
			}
		}

		public static WeaponAbility GetThirdAbility( ThrowingWeaponType GloveType )
		{
			switch ( GloveType )
			{
				case ThrowingWeaponType.Stones: return WeaponAbility.CrushingBlow;
				case ThrowingWeaponType.Axes: return WeaponAbility.BleedAttack;
				case ThrowingWeaponType.Daggers: return WeaponAbility.InfectiousStrike;
				case ThrowingWeaponType.Darts: return WeaponAbility.InfectiousStrike;
				case ThrowingWeaponType.Cards: return WeaponAbility.InfectiousStrike;
				case ThrowingWeaponType.Tomatoes: return WeaponAbility.CrushingBlow;
				default: return WeaponAbility.InfectiousStrike;
			}
		}

		public static WeaponAbility GetFourthAbility( ThrowingWeaponType GloveType )
		{
			switch ( GloveType )
			{
				case ThrowingWeaponType.Stones: return WeaponAbility.DeathBlow;
				case ThrowingWeaponType.Axes: return WeaponAbility.ConsecratedStrike;
				case ThrowingWeaponType.Daggers: return WeaponAbility.DevastatingBlow;
				case ThrowingWeaponType.Darts: return WeaponAbility.ToxicStrike;
				case ThrowingWeaponType.Cards: return WeaponAbility.DevastatingBlow;
				case ThrowingWeaponType.Tomatoes: return WeaponAbility.DeathBlow;
				default: return WeaponAbility.ShadowInfectiousStrike;
			}
		}

		public static WeaponAbility GetFifthAbility( ThrowingWeaponType GloveType )
		{
			switch ( GloveType )
			{
				case ThrowingWeaponType.Stones: return WeaponAbility.NerveStrike;
				case ThrowingWeaponType.Axes: return WeaponAbility.DoubleStrike;
				case ThrowingWeaponType.Daggers: return WeaponAbility.DeathBlow;
				case ThrowingWeaponType.Darts: return WeaponAbility.LightningStriker;
				case ThrowingWeaponType.Cards: return WeaponAbility.DeathBlow;
				case ThrowingWeaponType.Tomatoes: return WeaponAbility.NerveStrike;
				default: return WeaponAbility.DevastatingBlow;
			}
		}

		public static void ChangeGloveType<TGlove>( Mobile from, TGlove gloves ) where TGlove : Item, IThrowingGloves
		{
			if ( !gloves.IsChildOf( from.Backpack ) ) 
			{
				from.SendMessage( "This must be in your backpack to change the weapon type." );
				return;
			}

			gloves.GloveType = NextWeaponType( from, gloves.GloveType );

			from.SendMessage(68, "You have changed the gloves to throw " + gloves.GloveType + ".");
		}

		public static ThrowingWeaponType NextWeaponType( Mobile from, ThrowingWeaponType m_ammo )
		{
			if ( m_ammo == ThrowingWeaponType.Stones ) return ThrowingWeaponType.Axes;
			if ( m_ammo == ThrowingWeaponType.Axes ) return ThrowingWeaponType.Daggers;
			if ( m_ammo == ThrowingWeaponType.Daggers ) return ThrowingWeaponType.Darts;
			if ( m_ammo == ThrowingWeaponType.Darts ) return ThrowingWeaponType.Stars;

			if ( Server.Misc.GetPlayerInfo.isJester( from ) )
			{
				if ( m_ammo == ThrowingWeaponType.Stars ) return ThrowingWeaponType.Cards;
				if ( m_ammo == ThrowingWeaponType.Cards ) return ThrowingWeaponType.Tomatoes;
			}

			return ThrowingWeaponType.Stones;
		}

		public static string GetAmmoName( ThrowingWeaponType type )
		{
			switch ( type )
			{
				case ThrowingWeaponType.Stones:
				case ThrowingWeaponType.Axes:
				case ThrowingWeaponType.Daggers:
				case ThrowingWeaponType.Darts:
				case ThrowingWeaponType.Stars:
				case ThrowingWeaponType.Cards:
				case ThrowingWeaponType.Tomatoes:
					return type.ToString();

				default:
					return "Unknown";
			}
		}
	}
}