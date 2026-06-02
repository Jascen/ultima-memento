using Server.Utilities;

namespace Server.Items
{
	public class MasterSkeletonsKey : Item
	{
		public override string DefaultDescription
		{
			get
			{
				if ( Technology )
					return "These access cards can open almost any technological door or container. Use the access card and select locked item to see if it works.";

				return "These keys can open almost any door or container. Use the key and select locked item to see if it works.";
			}
		}

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		[Constructable]
		public MasterSkeletonsKey() : base( 0x410B )
		{
			Name = "master skeleton key";
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1060640 ); // The item must be in your backpack to use it.
			}
			else
			{
				from.SendMessage( "What locked container or door do you want to use the key on?" );
				UnlockUtilities.BeginSkeletonKeyUnlock( from, this, SkeletonKeyTier.Master, UnlockUtilities.SkeletonKeyMessagesMaster );
			}
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			list.Add( 1049644, "Open any locked container or door" );
		}

		public MasterSkeletonsKey( Serial serial ) : base( serial )
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
		}
	}
}
