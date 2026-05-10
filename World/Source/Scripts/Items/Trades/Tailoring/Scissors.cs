using System;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
	public interface IScissorable
	{
		bool Scissor( Mobile from, Scissors scissors );
	}

	[FlipableAttribute( 0xf9f, 0xf9e )]
	public class Scissors : Item
	{
		public override string DefaultDescription{ get{ return "These can cut hides into leather, or cut cloth into bandages. You can also cut leather or cloth clothing, back into basic items like leather and cloth."; } }

		[Constructable]
		public Scissors() : base( 0xF9F )
		{
			Weight = 1.0;
		}

		public Scissors( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			from.SendLocalizedMessage( 502434 ); // What should I use these scissors on?

			from.Target = new InternalTarget( this );
		}

		public static string CutUp( Mobile from, Item item, CraftResource resource, bool extraCloth )
		{
			switch( CraftResources.GetType( resource ) )
			{
				case CraftResourceType.Leather:
				case CraftResourceType.Skin:
				case CraftResourceType.Fabric:
				{
					try
					{
						CraftResourceInfo info = CraftResources.GetInfo( resource );

						double difficulty = CraftResources.GetSkill( resource );
						if ( difficulty > from.Skills.Tailoring.Value ) return "You are not skilled enough to cut that material.";

						Type resourceType = info.ResourceTypes[0];
						Item resc = (Item)Activator.CreateInstance( resourceType );

						resc.Amount = (int)item.Weight;
						if ( resc.Amount < 1 ){ resc.Amount = 1; }

						if ( extraCloth )
							resc.Amount = resc.Amount * 10;
						
						if (from is PlayerMobile && false == ((PlayerMobile)from).Preferences.ColorlessFabricBreakdown)
							resc.Hue = item.Hue;

						item.Delete();
						BaseContainer.PutStuffInContainer( from, 2, resc );
						from.PlaySound( 0x248 );
						return "You cut up the item into standard resources.";
					}
					catch
					{
						return "You fail to cut up the material.";
					}
				}
				
				default:
					return "Scissors can not be used on that to produce anything.";
			}
		}

		private class InternalTarget : Target
		{
			private Scissors m_Item;

			public InternalTarget( Scissors item ) : base( 2, false, TargetFlags.None )
			{
				m_Item = item;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Item.Deleted )
					return;

				if( Core.AOS && targeted == from )
				{
					from.SendLocalizedMessage( 1062845 + Utility.Random( 3 ) );	//"That doesn't seem like the smartest thing to do." / "That was an encounter you don't wish to repeat." / "Ha! You missed!"
				}
				else if( Core.SE && Utility.RandomDouble() > .20 && (from.Direction & Direction.Running) != 0 && ( DateTime.Now - from.LastMoveTime ) < from.ComputeMovementSpeed( from.Direction ) )
				{
					from.SendLocalizedMessage( 1063305 ); // Didn't your parents ever tell you not to run with scissors in your hand?!
				}
				else if( targeted is Item && !((Item)targeted).Movable ) 
				{
					if( targeted is IScissorable )
					{
						IScissorable obj = (IScissorable) targeted;

						if( obj.Scissor( from, m_Item ) )
							from.PlaySound( 0x248 );
					}
				}
				else if( targeted is IScissorable )
				{
					IScissorable obj = (IScissorable)targeted;

					if( obj.Scissor( from, m_Item ) )
						from.PlaySound( 0x248 );
				}
				else
				{
					from.SendLocalizedMessage( 502440 ); // Scissors can not be used on that to produce anything.
				}
			}

			protected override void OnNonlocalTarget( Mobile from, object targeted )
			{
				if ( targeted is IScissorable )
				{
					IScissorable obj = (IScissorable) targeted;

					if ( obj.Scissor( from, m_Item ) )
						from.PlaySound( 0x248 );
				}
				else
					base.OnNonlocalTarget( from, targeted );
			}
		}
	}
}