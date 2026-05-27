using System;
using Server;
using System.Collections;
using Server.Network;
using System.Text;
using Server.Items;
using Server.Mobiles;
using Server.Targets;
using Server.Targeting;
using Server.Misc;

namespace Server.Items
{
	public class TransmutationPotion : BasePotion
	{
		public override void ResourceChanged( CraftResource resource )
		{
			if ( !ResourceCanChange() )
				return;

			m_Resource = resource;
			Hue = CraftResources.GetHue(m_Resource);
			InfoText1 = CraftResources.GetName( m_Resource );
			InvalidateProperties();
		}

		public override string DefaultDescription{ get{ return "This potion will change many materialed items from one substance to another."; } }

		[Constructable]
		public TransmutationPotion() : base( 0x2827, PotionEffect.Transmutation )
		{
			Name = "transmutation potion";
			SetMaterial();
		}

		public TransmutationPotion( Serial serial ) : base( serial )
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

		public override void OnAfterDuped( Item newItem )
		{
			TransmutationPotion item = newItem as TransmutationPotion;

			if ( item == null )
				return;

			item.Resource = m_Resource;
		}

	  	public override void Drink( Mobile m )
		{
         	if ( m.InRange( this.GetWorldLocation(), 1 ) ) 
         	{ 
				m.SendMessage( "What would you like to pour this on?" );
				m.Target = new ItemTarget( this, m, m_Resource );
         	} 
         	else 
         	{ 
            	m.LocalOverheadMessage( MessageType.Regular, 906, 1019045 ); // I can't reach that. 
         	} 
		}

		private class ItemTarget : Target
		{
			private TransmutationPotion m_Potion;
			private Mobile m_From;
			private CraftResource m_Res;

			public ItemTarget( TransmutationPotion potion, Mobile from, CraftResource resource ) :  base ( 1, false, TargetFlags.None )
			{
				m_Potion = potion;
				m_From = from;
				m_Res = resource;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Item )
				{
					Item item = (Item)targeted;

					if ( !item.IsChildOf( from.Backpack ) )
					{
						from.SendMessage("The item to transmutate must be in your pack!");
					}
					else if ( item.Layer < Layer.FirstValid || Layer.LastUserValid < item.Layer )
					{
						from.SendMessage( "This potion will not work on that!" );
					}
					else if ( ResourceMods.SetResource( item, m_Res ) )
					{
						from.SendMessage("The item has been changed.");
						m_From.PlaySound( 0x240 );
						m_Potion.Consume();
					}
					else
					{
						from.SendMessage( "This potion will not work on that!" );
					}
				}
				else
				{
					from.SendMessage( "This potion will not work on that!" );
				}
			}
		}

		public override bool StackWith(Mobile from, Item dropped, bool playSound)
		{
			if (dropped is TransmutationPotion && ((TransmutationPotion)dropped).Resource != Resource) return false;

			return base.StackWith(from, dropped, playSound);
		}

		public void SetMaterial()
		{
			if ( this.Resource == CraftResource.None )
			{
				switch ( Utility.RandomMinMax( 0, 5 ) )
				{
					case 0: ResourceMods.SetRandomResource( false, true, this, CraftResource.Iron, true, null ); break;
					case 1: ResourceMods.SetRandomResource( false, true, this, CraftResource.RegularLeather, true, null ); break;
					case 2: ResourceMods.SetRandomResource( false, true, this, CraftResource.BrittleSkeletal, true, null ); break;
					case 3: ResourceMods.SetRandomResource( false, true, this, CraftResource.RedScales, true, null ); break;
					case 4: ResourceMods.SetRandomResource( false, true, this, CraftResource.AmethystBlock, true, null ); break;
					case 5: ResourceMods.SetRandomResource( false, true, this, CraftResource.DemonSkin, true, null ); break;
				}
			}
		}
	}
}