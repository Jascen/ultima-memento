using System;

namespace Server.Items
{
	[FlipableAttribute( 0x13C6, 0x13CE )]
	public class LevelThrowingGloves : BaseLevelRanged, IThrowingGloves
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

		public override SkillName DefSkill{ get{ return SkillName.Marksmanship; } }
		public override WeaponType DefType{ get{ return WeaponType.Ranged; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Punching; } }

		[Constructable]
		public LevelThrowingGloves() : base( 0x13C6 )
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

		public LevelThrowingGloves( Serial serial ) : base( serial )
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
}