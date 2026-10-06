using System;
using Server.Items; 
using System.Collections.Generic;
using Server.ContextMenus;

namespace Server.Mobiles 
{
	[CorpseName( "a broken machine" )] 
	public class GolemPorter : BaseCreature
	{
		public const int MaxControlSlots = 5;
		
		public int PorterExodus;
		[CommandProperty(AccessLevel.Owner)]
		public int Porter_Exodus{ get { return PorterExodus; } set { PorterExodus = value; InvalidateProperties(); } }

		private DateTime m_NextTalking;
		public DateTime NextTalking{ get{ return m_NextTalking; } set{ m_NextTalking = value; } }
		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( DateTime.Now >= m_NextTalking && InRange( m, 20 ) )
			{
				this.Loyalty = 100;
				m_NextTalking = (DateTime.Now + TimeSpan.FromSeconds( 300 ));
			}
		}

		[Constructable] 
		public GolemPorter( ) : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			m_NextTalking = (DateTime.Now + TimeSpan.FromSeconds( 60 ));

			Name = "a golem";
			Body = 752;
			ControlSlots = MaxControlSlots;
			Blessed = true;
			ActiveSpeed = 0.1;
			PassiveSpeed = 0.2;

			SetStr( 100 );

			Container pack = Backpack;

			if ( pack != null )
				pack.Delete();

			pack = new StrongBackpack();
			pack.Movable = false;

			AddItem( pack );
		}

		public override bool ClickTitle{ get{ return false; } }
		public override bool ShowFameTitle{ get{ return false; } }
		public override bool AlwaysAttackable{ get{ return false; } }
		public override bool InitialInnocent{ get{ return true; } }
		public override bool DeleteOnRelease{ get{ return true; } }
		public override bool DeleteCorpseOnDeath{ get{ return true; } }
		public override bool IsDispellable { get { return false; } }
		public override bool IsBondable{ get{ return false; } }
		public override bool CanBeRenamedBy( Mobile from ){ return true; }

		public GolemPorter( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void OnAfterSpawn()
		{
			if ( Hue == 2118 )
			{
				Title = "of Exodus";
				if ( PorterExodus == 1 ){ SetStr( 600 ); }
				else if ( PorterExodus == 2 ){ SetStr( 650 ); }
				else if ( PorterExodus == 3 ){ SetStr( 700 ); }
				else if ( PorterExodus == 4 ){ SetStr( 750 ); }
				else if ( PorterExodus == 5 ){ SetStr( 800 ); }
				else if ( PorterExodus == 6 ){ SetStr( 850 ); }
				else if ( PorterExodus == 7 ){ SetStr( 900 ); }
				else if ( PorterExodus == 8 ){ SetStr( 950 ); }
				else if ( PorterExodus == 9 ){ SetStr( 1000 ); }
			}
			else if ( Resource == CraftResource.DullCopper ){ SetStr( 150 ); }
			else if ( Resource == CraftResource.ShadowIron ){ SetStr( 200 ); }
			else if ( Resource == CraftResource.Copper ){ SetStr( 250 ); }
			else if ( Resource == CraftResource.Bronze ){ SetStr( 300 ); }
			else if ( Resource == CraftResource.Gold ){ SetStr( 350 ); }
			else if ( Resource == CraftResource.Agapite ){ SetStr( 400 ); }
			else if ( Resource == CraftResource.Verite ){ SetStr( 450 ); }
			else if ( Resource == CraftResource.Valorite ){ SetStr( 65000 ); }
			else { Hue = 0x430; SetStr( 100 ); }
		}

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); // version
            writer.Write( PorterExodus );
			Loyalty = 100;
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt();
			PorterExodus = reader.ReadInt();
		} 

		public override bool IsSnoop( Mobile from )
		{
			return false;
		}

		public override bool OnDragDrop( Mobile from, Item item )
		{
			if ( PackAnimal.CheckAccess( this, from ) )
			{
				AddToBackpack( item );
				return true;
			}

			return base.OnDragDrop( from, item );
		}

		public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
		{
			return PackAnimal.CheckAccess( this, from );
		}

		public override bool CheckNonlocalLift( Mobile from, Item item )
		{
			return PackAnimal.CheckAccess( this, from );
		}

		public override void OnDoubleClick( Mobile from )
		{
			PackAnimal.TryPackOpen( this, from );
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			PackAnimal.GetContextMenuEntries( this, from, list );
		}
	}
}