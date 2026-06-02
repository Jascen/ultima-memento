using Server.Utilities;

namespace Server.Items
{
	public class BottleOfAcid : Item
	{
		public override Catalogs DefaultCatalog{ get{ return Catalogs.Potion; } }

		public override string DefaultDescription{ get{ return "These bottles of acid can not only eat through almost any locked container, but also destroy any traps on them as well."; } }

		public override int Hue{ get { return 1167; } }

		public override double DefaultWeight
		{
			get { return 1.0; }
		}

		[Constructable]
		public BottleOfAcid() : base( 0x180F )
		{
			Name = "bottle of acid";
			Stackable = true;
			Built = true;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1060640 ); // The item must be in your backpack to use it.
			}
			else
			{
				from.SendMessage( "What chest do you want to use the acid on?" );
				UnlockUtilities.BeginTrapDissolverUnlock( from, this, UnlockUtilities.AcidProfile, UnlockUtilities.TryMeltHead );
			}
		}

		public BottleOfAcid( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			Built = true;
		}
	}
}
