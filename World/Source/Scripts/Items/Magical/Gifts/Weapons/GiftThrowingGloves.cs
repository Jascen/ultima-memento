using System;

namespace Server.Items
{
	[FlipableAttribute( 0x13C6, 0x13CE )]
	public class GiftThrowingGloves : BaseGiftRanged, IThrowingGloves
	{
		public string GloveType;
		
		[CommandProperty(AccessLevel.Owner)]
		public string Glove_Type { get { return GloveType; } set { GloveType = value; InvalidateProperties(); } }

		public override int EffectID
		{
			get
			{
				if ( GloveType == "Stones" ){ return 0xF8B; }
				else if ( GloveType == "Axes" ){ return 0x48B0; }
				else if ( GloveType == "Knives" ){ return 0x902; }
				else if ( GloveType == "Darts" ){ return 0x2804; }
				else { return 0x27AC; }
			}
		}

		public override Type AmmoType{ get{ return typeof( ThrowingWeapon ); } }
		public override Item Ammo{ get{ return new ThrowingWeapon(); } }

		public override WeaponAbility PrimaryAbility
		{
			get
			{
				if ( GloveType == "Stones" ){ return WeaponAbility.ConcussionBlow; }
				else if ( GloveType == "Axes" ){ return WeaponAbility.ArmorIgnore; }
				else if ( GloveType == "Knives" ){ return WeaponAbility.ArmorIgnore; }
				else if ( GloveType == "Darts" ){ return WeaponAbility.ParalyzingBlow; }
				else { return WeaponAbility.ShadowStrike; }
			}
		}

		public override WeaponAbility SecondaryAbility
		{
			get
			{
				if ( GloveType == "Stones" ){ return WeaponAbility.StunningStrike; }
				else if ( GloveType == "Axes" ){ return WeaponAbility.TalonStrike; }
				else if ( GloveType == "Knives" ){ return WeaponAbility.TalonStrike; }
				else if ( GloveType == "Darts" ){ return WeaponAbility.ArmorIgnore; }
				else { return WeaponAbility.ParalyzingBlow; }
			}
		}

		public override WeaponAbility ThirdAbility
		{
			get
			{
				if ( GloveType == "Stones" ){ return WeaponAbility.CrushingBlow; }
				else if ( GloveType == "Axes" ){ return WeaponAbility.BleedAttack; }
				else if ( GloveType == "Knives" ){ return WeaponAbility.InfectiousStrike; }
				else if ( GloveType == "Darts" ){ return WeaponAbility.InfectiousStrike; }
				else { return WeaponAbility.InfectiousStrike; }
			}
		}

		public override WeaponAbility FourthAbility
		{
			get
			{
				if ( GloveType == "Stones" ){ return WeaponAbility.DeathBlow; }
				else if ( GloveType == "Axes" ){ return WeaponAbility.ConsecratedStrike; }
				else if ( GloveType == "Knives" ){ return WeaponAbility.DevastatingBlow; }
				else if ( GloveType == "Darts" ){ return WeaponAbility.ToxicStrike; }
				else { return WeaponAbility.ShadowInfectiousStrike; }
			}
		}

		public override WeaponAbility FifthAbility
		{
			get
			{
				if ( GloveType == "Stones" ){ return WeaponAbility.NerveStrike; }
				else if ( GloveType == "Axes" ){ return WeaponAbility.DoubleStrike; }
				else if ( GloveType == "Knives" ){ return WeaponAbility.DeathBlow; }
				else if ( GloveType == "Darts" ){ return WeaponAbility.LightningStriker; }
				else { return WeaponAbility.DevastatingBlow; }
			}
		}

		public override int AosStrengthReq{ get{ return 20; } }
		public override int AosMinDamage{ get{ return 12; } }
		public override int AosMaxDamage{ get{ return 16; } }
		public override int AosSpeed{ get{ return 23; } }
		public override float MlSpeed{ get{ return 4.00f; } }

		public override int OldStrengthReq{ get{ return 20; } }
		public override int OldMinDamage{ get{ return 12; } }
		public override int OldMaxDamage{ get{ return 16; } }
		public override int OldSpeed{ get{ return 20; } }

		public override int DefMaxRange{ get{ return 8; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 60; } }

		public override SkillName DefSkill{ get{ return SkillName.Marksmanship; } }
		public override WeaponType DefType{ get{ return WeaponType.Ranged; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Punching; } }

		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		[Constructable]
		public GiftThrowingGloves() : base( 0x13C6 )
		{
			if ( GloveType == "" || GloveType == null ){ GloveType = "Stones"; }
			Name = "throwing gloves";
			Weight = 2.0;
			Hue = Utility.RandomColor(0);
			Layer = Layer.OneHanded;
			Attributes.SpellChanneling = 1;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) 
			{
				from.SendMessage( "This must be in your backpack to change the weapon type." );
				return;
			}
			else
			{
				if ( GloveType == "Stones" ){ GloveType = "Axes"; }
				else if ( GloveType == "Axes" ){ GloveType = "Knives"; }
				else if ( GloveType == "Knives" ){ GloveType = "Darts"; }
				else if ( GloveType == "Darts" ){ GloveType = "Stars"; }
				else { GloveType = "Stones"; }
				from.SendMessage(68, "You have changed the gloves to throw " + GloveType + ".");
				this.InvalidateProperties();
			}
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			list.Add( 1049644, "Double click to change type from " + GloveType );
			list.Add( 1070722, "Cannot be used with other weapons" );
		}

		public GiftThrowingGloves( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
            writer.Write( GloveType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
            GloveType = reader.ReadString();
		}
	}
}