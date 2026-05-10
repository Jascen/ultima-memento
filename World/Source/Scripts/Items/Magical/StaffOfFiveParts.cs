using System;
using Server;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Misc;

namespace Server.Items
{
	public class StaffFiveParts : QuarterStaff
	{
		public Mobile StaffOwner;
		public string StaffName;
		public int StaffMagic;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Staff_Owner { get{ return StaffOwner; } set{ StaffOwner = value; } }

		[CommandProperty(AccessLevel.Owner)]
		public string Staff_Name { get { return StaffName; } set { StaffName = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Owner)]
		public int Staff_Magic { get { return StaffMagic; } set { StaffMagic = value; InvalidateProperties(); } }

		public override int InitMinHits{ get{ return 80; } }
		public override int InitMaxHits{ get{ return 160; } }

		public override bool DisplayLootType{ get{ return false; } }


		[Constructable]
		public StaffFiveParts() : this( null, 0 )
		{
		}

		[Constructable]
		public StaffFiveParts( Mobile from, int magic )
		{
			if ( from != null && magic > 0 )
			{
				StaffMagic = magic;

				this.StaffOwner = from;
				string StaffName = StaffOwner.Name + " the Wizard";
				if ( magic == 1 ){ StaffName = StaffOwner.Name + " the Necromancer"; }
				if ( magic == 2 ){ StaffName = StaffOwner.Name + " the Elementalist"; }
				EngravedText = StaffName;

				Name = "Staff of Ultimate Power";
				Hue = 0x491;
					if ( StaffMagic > 0 ){ this.Hue = 0x96C; } 

				AosElementDamages.Physical = 100;
				Attributes.SpellChanneling = 1;
				Attributes.SpellDamage = 50;
				Attributes.CastRecovery = 2;
				Attributes.CastSpeed = 2;
				Attributes.LowerManaCost = 10;
				Attributes.LowerRegCost = 20;
				LootType = LootType.Blessed;
				WeaponAttributes.LowerStatReq = 50;
				AccuracyLevel = WeaponAccuracyLevel.Supremely;
				DamageLevel = WeaponDamageLevel.Vanq;
				DurabilityLevel = WeaponDurabilityLevel.Indestructible;

				SkillBonuses.SetValues(0, SkillName.Bludgeoning, 20);
				SkillBonuses.SetValues(1, SkillName.MagicResist, 25);
				if ( magic == 1 ){ SkillBonuses.SetValues(2, SkillName.Necromancy, 25); }
				else if ( magic == 2 ){ SkillBonuses.SetValues(2, SkillName.Elementalism, 25); }
				else { SkillBonuses.SetValues(2, SkillName.Magery, 25); }
				ArtifactLevel = ArtifactLevel.StandardArtefact;
			}

			if ( from == null )
				this.Delete();
		}

		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			base.OnHit( attacker, defender, damageBonus );

			this.AosElementDamages.Physical = 0;
			this.AosElementDamages.Fire = 0;
			this.AosElementDamages.Cold = 0;
			this.AosElementDamages.Poison = 0;
			this.AosElementDamages.Energy = 0;

			switch ( Utility.RandomMinMax( 0, 4 ) ) 
			{
				case 0: this.Hue = 0x48F; if ( StaffMagic > 0 ){ this.Hue = 0x558; } this.AosElementDamages.Poison = 100; break;
				case 1: this.Hue = 0x48D; if ( StaffMagic > 0 ){ this.Hue = 0x554; } this.AosElementDamages.Cold = 100; break;
				case 2: this.Hue = 0x48E; if ( StaffMagic > 0 ){ this.Hue = 0x54D; } this.AosElementDamages.Fire = 100; break;
				case 3: this.Hue = 0x491; if ( StaffMagic > 0 ){ this.Hue = 0x96C; } this.AosElementDamages.Physical = 100; break;
				case 4: this.Hue = 0x490; if ( StaffMagic > 0 ){ this.Hue = 0x561; } this.AosElementDamages.Energy = 100; break;
			}

			this.InvalidateProperties();
		}

		public override bool OnEquip( Mobile from )
		{
			if ( this.StaffOwner == from )
				return base.OnEquip( from );

			from.LocalOverheadMessage( MessageType.Emote, 0x916, true, "The staff burns your hands!" );
			return false;
		}

		public StaffFiveParts( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
			writer.Write( (Mobile)StaffOwner );
            writer.Write( StaffName );
            writer.Write( StaffMagic );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			StaffOwner = reader.ReadMobile();
			StaffName = reader.ReadString();
			StaffMagic = reader.ReadInt();
			EngravedText = StaffName;
			LootType = LootType.Blessed;

			if ( version < 1 )
			{
				Attributes.LowerManaCost = 10;
				Attributes.LowerRegCost = 20;
			}
		}
	}
	///////////////////////////////////////////////////////////////////////////
	public class StaffPartVenom : Item
	{
		public Mobile Owner;

		[Constructable]
		public StaffPartVenom() : base( 0x3A7 )
		{
			Hue = 0x48F;
			Name = "piece of a staff";
			ColorText4 = "1st of 5 Pieces";
			ColorText5 = "Venom Piece";
			ColorHue4 = "59F06B";
			ColorHue5 = "C189E5";
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			if ( Owner != null ){ list.Add( 1049644, "Belongs to " + Owner.Name + "" ); }
        }

		public override bool OnDragLift( Mobile from )
		{
			if ( from is PlayerMobile && Owner == null )
			{
				ArrayList targets = new ArrayList();
				foreach ( Item item in World.Items.Values )
				if ( item is StaffPartVenom && ((StaffPartVenom)item).Owner == from && item != this )
				{
					targets.Add( item );
				}
				for ( int i = 0; i < targets.Count; ++i )
				{
					Item item = ( Item )targets[ i ];
					item.Delete();
				}
				this.Owner = from;
			}

			return base.OnDragLift( from );
		}

		public StaffPartVenom( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 ); // version
			writer.Write( (Mobile)Owner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version > 0 )
				Owner = reader.ReadMobile();

			Name = "piece of a staff";
			ColorText4 = "1st of 5 Pieces";
			ColorText5 = "Venom Piece";
			ColorHue4 = "59F06B";
			ColorHue5 = "C189E5";
		}
	}
	///////////////////////////////////////////////////////////////////////////
	public class StaffPartCaddellite : Item
	{
		public Mobile Owner;

		[Constructable]
		public StaffPartCaddellite() : base( 0x3A7 )
		{
			Hue = 0x48D;
			Name = "piece of a staff";
			ColorText4 = "2nd of 5 Pieces";
			ColorText5 = "Caddellite Piece";
			ColorHue4 = "59F06B";
			ColorHue5 = "C189E5";
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			if ( Owner != null ){ list.Add( 1049644, "Belongs to " + Owner.Name + "" ); }
        }

		public override bool OnDragLift( Mobile from )
		{
			if ( from is PlayerMobile && Owner == null )
			{
				ArrayList targets = new ArrayList();
				foreach ( Item item in World.Items.Values )
				if ( item is StaffPartCaddellite && ((StaffPartCaddellite)item).Owner == from && item != this )
				{
					targets.Add( item );
				}
				for ( int i = 0; i < targets.Count; ++i )
				{
					Item item = ( Item )targets[ i ];
					item.Delete();
				}
				this.Owner = from;
			}

			return base.OnDragLift( from );
		}

		public StaffPartCaddellite( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 ); // version
			writer.Write( (Mobile)Owner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version > 0 )
				Owner = reader.ReadMobile();

			Name = "piece of a staff";
			ColorText4 = "2nd of 5 Pieces";
			ColorText5 = "Caddellite Piece";
			ColorHue4 = "59F06B";
			ColorHue5 = "C189E5";
		}
	}
	///////////////////////////////////////////////////////////////////////////
	public class StaffPartFire : Item
	{
		public Mobile Owner;

		[Constructable]
		public StaffPartFire() : base( 0x3A7 )
		{
			Hue = 0x48E;
			Name = "piece of a staff";
			ColorText4 = "3rd of 5 Pieces";
			ColorText5 = "Fire Piece";
			ColorHue4 = "59F06B";
			ColorHue5 = "C189E5";
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			if ( Owner != null ){ list.Add( 1049644, "Belongs to " + Owner.Name + "" ); }
        }

		public override bool OnDragLift( Mobile from )
		{
			if ( from is PlayerMobile && Owner == null )
			{
				ArrayList targets = new ArrayList();
				foreach ( Item item in World.Items.Values )
				if ( item is StaffPartFire && ((StaffPartFire)item).Owner == from && item != this )
				{
					targets.Add( item );
				}
				for ( int i = 0; i < targets.Count; ++i )
				{
					Item item = ( Item )targets[ i ];
					item.Delete();
				}
				this.Owner = from;
			}

			return base.OnDragLift( from );
		}

		public StaffPartFire( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 ); // version
			writer.Write( (Mobile)Owner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version > 0 )
				Owner = reader.ReadMobile();

			Name = "piece of a staff";
			ColorText4 = "3rd of 5 Pieces";
			ColorText5 = "Fire Piece";
			ColorHue4 = "59F06B";
			ColorHue5 = "C189E5";
		}
	}
	///////////////////////////////////////////////////////////////////////////
	public class StaffPartLight : Item
	{
		public Mobile Owner;

		[Constructable]
		public StaffPartLight() : base( 0x3A7 )
		{
			Hue = 0x491;
			Name = "piece of a staff";
			ColorText4 = "4th of 5 Pieces";
			ColorText5 = "Light Piece";
			ColorHue4 = "59F06B";
			ColorHue5 = "C189E5";
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			if ( Owner != null ){ list.Add( 1049644, "Belongs to " + Owner.Name + "" ); }
        }

		public override bool OnDragLift( Mobile from )
		{
			if ( from is PlayerMobile && Owner == null )
			{
				ArrayList targets = new ArrayList();
				foreach ( Item item in World.Items.Values )
				if ( item is StaffPartLight && ((StaffPartLight)item).Owner == from && item != this )
				{
					targets.Add( item );
				}
				for ( int i = 0; i < targets.Count; ++i )
				{
					Item item = ( Item )targets[ i ];
					item.Delete();
				}
				this.Owner = from;
			}

			return base.OnDragLift( from );
		}

		public StaffPartLight( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 ); // version
			writer.Write( (Mobile)Owner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version > 0 )
				Owner = reader.ReadMobile();

			Name = "piece of a staff";
			ColorText4 = "4th of 5 Pieces";
			ColorText5 = "Light Piece";
			ColorHue4 = "59F06B";
			ColorHue5 = "C189E5";
		}
	}
	///////////////////////////////////////////////////////////////////////////
	public class StaffPartEnergy : Item
	{
		public Mobile Owner;

		[Constructable]
		public StaffPartEnergy() : base( 0x3A7 )
		{
			Hue = 0x490;
			Name = "piece of a staff";
			ColorText4 = "5th of 5 Pieces";
			ColorText5 = "Energy Piece";
			ColorHue4 = "59F06B";
			ColorHue5 = "C189E5";
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			if ( Owner != null ){ list.Add( 1049644, "Belongs to " + Owner.Name + "" ); }
        }

		public override bool OnDragLift( Mobile from )
		{
			if ( from is PlayerMobile && Owner == null )
			{
				ArrayList targets = new ArrayList();
				foreach ( Item item in World.Items.Values )
				if ( item is StaffPartEnergy && ((StaffPartEnergy)item).Owner == from && item != this )
				{
					targets.Add( item );
				}
				for ( int i = 0; i < targets.Count; ++i )
				{
					Item item = ( Item )targets[ i ];
					item.Delete();
				}
				this.Owner = from;
			}

			return base.OnDragLift( from );
		}

		public StaffPartEnergy( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)1 ); // version
			writer.Write( (Mobile)Owner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version > 0 )
				Owner = reader.ReadMobile();

			Name = "piece of a staff";
			ColorText4 = "5th of 5 Pieces";
			ColorText5 = "Energy Piece";
			ColorHue4 = "59F06B";
			ColorHue5 = "C189E5";
		}
	}
}