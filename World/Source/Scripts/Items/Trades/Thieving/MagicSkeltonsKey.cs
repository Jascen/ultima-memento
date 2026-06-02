using System;
using Server.Utilities;

namespace Server.Items
{
	public class MagicSkeltonsKey : Item
	{
		private static TimeSpan Cooldown = TimeSpan.FromMinutes( 5 );
		private DateTime NextUseAvailable = DateTime.MinValue;

		public override string DefaultDescription
		{
			get
			{
				return "These keys can open almost any door or container. Use the key and select locked item to see if it works. The key will be damaged after each use and will need time to regain it's power.";
			}
		}

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		[Constructable]
		public MagicSkeltonsKey() : base( 0x5751 )
		{
			Name = "magic skeleton key";
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( DateTime.Now < NextUseAvailable )
			{
				from.SendMessage( "The key is still misshapen from the last use. Wait a moment before using it again." );
				return;
			}

			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1060640 ); // The item must be in your backpack to use it.
			}
			else
			{
				from.SendMessage( "What locked container or door do you want to use the key on?" );
				UnlockUtilities.BeginSkeletonKeyUnlock( from, this, SkeletonKeyTier.Master, UnlockUtilities.SkeletonKeyMessagesMagic );
			}
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			list.Add( 1049644, "Open any locked container or door" );
		}

		public override void Consume()
		{
			NextUseAvailable = DateTime.Now.Add( Cooldown );
		}

		public MagicSkeltonsKey( Serial serial ) : base( serial )
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
