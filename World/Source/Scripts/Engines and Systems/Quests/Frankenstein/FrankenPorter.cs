using System;
using Server.Items;
using System.Collections.Generic;
using Server.ContextMenus;

namespace Server.Mobiles 
{
	[CorpseName( "a huge corpse" )] 
	public class FrankenPorter : BaseCreature
	{
		public int PorterLevel;
		[CommandProperty(AccessLevel.GameMaster)]
		public int Porter_Level{ get { return PorterLevel; } set { PorterLevel = value; InvalidateProperties(); } }

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
		public FrankenPorter( ) : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			m_NextTalking = (DateTime.Now + TimeSpan.FromSeconds( 60 ));

			Name = "a reanimation";
			Body = 752;
			BaseSoundID = 684;
			ControlSlots = 4;
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

		public FrankenPorter( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void OnAfterSpawn()
		{
			if ( PorterLevel < 10 ){ SetStr( 100 ); }
			else if ( PorterLevel < 15 ){ SetStr( 150 ); }
			else if ( PorterLevel < 20 ){ SetStr( 200 ); }
			else if ( PorterLevel < 25 ){ SetStr( 250 ); }
			else if ( PorterLevel < 30 ){ SetStr( 300 ); }
			else if ( PorterLevel < 35 ){ SetStr( 350 ); }
			else if ( PorterLevel < 40 ){ SetStr( 400 ); }
			else if ( PorterLevel < 45 ){ SetStr( 450 ); }
			else if ( PorterLevel < 50 ){ SetStr( 500 ); }
			else if ( PorterLevel < 55 ){ SetStr( 550 ); }
			else if ( PorterLevel < 60 ){ SetStr( 600 ); }
			else if ( PorterLevel < 65 ){ SetStr( 650 ); }
			else if ( PorterLevel < 70 ){ SetStr( 700 ); }
			else if ( PorterLevel < 75 ){ SetStr( 750 ); }
			else if ( PorterLevel < 80 ){ SetStr( 800 ); }
			else if ( PorterLevel < 85 ){ SetStr( 850 ); }
			else if ( PorterLevel < 90 ){ SetStr( 900 ); }
			else if ( PorterLevel < 95 ){ SetStr( 950 ); }
			else { SetStr( 1000 ); }

			if ( PorterLevel >= 65 ){ Body = 999; }
		}

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); // version
            writer.Write( PorterLevel );
			Loyalty = 100;
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt();
			PorterLevel = reader.ReadInt();
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