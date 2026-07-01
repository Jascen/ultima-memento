using System;

namespace Server.Items
{
	public interface IThrowingGloves
	{
	}

	[FlipableAttribute( 0x13C6, 0x13CE )]
	public class ThrowingGloves : BaseRanged, IThrowingGloves
	{
		public string GloveType;
		
		[CommandProperty(AccessLevel.Owner)]
		public string Glove_Type { get { return GloveType; } set { GloveType = value; InvalidateProperties(); } }

		public override int EffectID
		{
			get
			{
				if ( GloveType == "Stones" ){ return 0x10B6; }
				else if ( GloveType == "Axes" ){ return 0x10B3; }
				else if ( GloveType == "Daggers" ){ return 0x529F; }
				else if ( GloveType == "Darts" ){ return 0x52B0; }
				else if ( GloveType == "Cards" ){ return 0x4C29; }
				else if ( GloveType == "Tomatoes" ){ return 0x4C28; }
				else { return 0x10B2; }
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
				else if ( GloveType == "Daggers" ){ return WeaponAbility.ArmorIgnore; }
				else if ( GloveType == "Darts" ){ return WeaponAbility.ParalyzingBlow; }
				else if ( GloveType == "Cards" ){ return WeaponAbility.ArmorIgnore; }
				else if ( GloveType == "Tomatoes" ){ return WeaponAbility.ConcussionBlow; }
				else { return WeaponAbility.ShadowStrike; }
			}
		}

		public override WeaponAbility SecondaryAbility
		{
			get
			{
				if ( GloveType == "Stones" ){ return WeaponAbility.StunningStrike; }
				else if ( GloveType == "Axes" ){ return WeaponAbility.TalonStrike; }
				else if ( GloveType == "Daggers" ){ return WeaponAbility.TalonStrike; }
				else if ( GloveType == "Darts" ){ return WeaponAbility.ArmorIgnore; }
				else if ( GloveType == "Cards" ){ return WeaponAbility.TalonStrike; }
				else if ( GloveType == "Tomatoes" ){ return WeaponAbility.StunningStrike; }
				else { return WeaponAbility.ParalyzingBlow; }
			}
		}

		public override WeaponAbility ThirdAbility
		{
			get
			{
				if ( GloveType == "Stones" ){ return WeaponAbility.CrushingBlow; }
				else if ( GloveType == "Axes" ){ return WeaponAbility.BleedAttack; }
				else if ( GloveType == "Daggers" ){ return WeaponAbility.InfectiousStrike; }
				else if ( GloveType == "Darts" ){ return WeaponAbility.InfectiousStrike; }
				else if ( GloveType == "Cards" ){ return WeaponAbility.InfectiousStrike; }
				else if ( GloveType == "Tomatoes" ){ return WeaponAbility.CrushingBlow; }
				else { return WeaponAbility.InfectiousStrike; }
			}
		}

		public override WeaponAbility FourthAbility
		{
			get
			{
				if ( GloveType == "Stones" ){ return WeaponAbility.DeathBlow; }
				else if ( GloveType == "Axes" ){ return WeaponAbility.ConsecratedStrike; }
				else if ( GloveType == "Daggers" ){ return WeaponAbility.DevastatingBlow; }
				else if ( GloveType == "Darts" ){ return WeaponAbility.ToxicStrike; }
				else if ( GloveType == "Cards" ){ return WeaponAbility.DevastatingBlow; }
				else if ( GloveType == "Tomatoes" ){ return WeaponAbility.DeathBlow; }
				else { return WeaponAbility.ShadowInfectiousStrike; }
			}
		}

		public override WeaponAbility FifthAbility
		{
			get
			{
				if ( GloveType == "Stones" ){ return WeaponAbility.NerveStrike; }
				else if ( GloveType == "Axes" ){ return WeaponAbility.DoubleStrike; }
				else if ( GloveType == "Daggers" ){ return WeaponAbility.DeathBlow; }
				else if ( GloveType == "Darts" ){ return WeaponAbility.LightningStriker; }
				else if ( GloveType == "Cards" ){ return WeaponAbility.DeathBlow; }
				else if ( GloveType == "Tomatoes" ){ return WeaponAbility.NerveStrike; }
				else { return WeaponAbility.DevastatingBlow; }
			}
		}

		public override int AosStrengthReq{ get{ return 20; } }
		public override int AosMinDamage{ get{ return 10; } }
		public override int AosMaxDamage{ get{ return 14; } }
		public override int AosSpeed{ get{ return 23; } }
		public override float MlSpeed{ get{ return 4.00f; } }

		public override int OldStrengthReq{ get{ return 20; } }
		public override int OldMinDamage{ get{ return 6; } }
		public override int OldMaxDamage{ get{ return 22; } }
		public override int OldSpeed{ get{ return 20; } }

		public override int DefMaxRange{ get{ return 8; } }

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
			if ( GloveType == "" || GloveType == null ){ GloveType = "Stones"; }
			Name = "throwing gloves";
			Weight = 2.0;
			Hue = Utility.RandomColor(0);
			Layer = Layer.OneHanded;
			Attributes.SpellChanneling = 1;
			Resource = CraftResource.RegularLeather;
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
				else if ( GloveType == "Axes" ){ GloveType = "Daggers"; }
				else if ( GloveType == "Daggers" ){ GloveType = "Darts"; }
				else if ( GloveType == "Darts" ){ GloveType = "Stars"; }
				else if ( GloveType == "Stars" && Server.Misc.GetPlayerInfo.isJester( from ) ){ GloveType = "Cards"; }
				else if ( GloveType == "Cards" && Server.Misc.GetPlayerInfo.isJester( from ) ){ GloveType = "Tomatoes"; }
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

		public ThrowingGloves( Serial serial ) : base( serial )
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