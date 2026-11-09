using System;
using Server.Targeting;
using Server.Items;
using Server.Mobiles;

namespace Server.Spells.Research
{
	public class ResearchRingofFire : ResearchSpell
	{
		public int CirclePower = 8;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 643,
					IconGraphic = 11226,
					Name = "Ring of Fire",
					PowerWords = "In Flam An Por",
					Description = "This spell creates a ring of fire at a chosen location, where demonic creatures cannot pass. Any demonic creature that touches the flames will be unable to cast spells, but they may still perform any special attacks they may have. The ring can last up to 50 seconds before extinguishing itself, and there is a chance that the demon can traverse the flaming circle if cast on uneven terrain.",
					ManaCost = 55,
					MinSkill = 85,
					TargetType = TargetFlags.None
				},
				236,
				9031,
				Reagent.Brimstone,Reagent.PigIron,Reagent.DaemonBlood
			);

		public ResearchRingofFire( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			int fires = 0;

			foreach ( Item m in Caster.GetItemsInRange( 10 ) )
			{
				if ( m is RingOfFire )
					++fires;
			}

			if ( fires > 23 )
			{
				Caster.SendMessage( "There are too many magical fires in the area!" );
			}
			else
			{
				Caster.SendMessage( "Where do you want to create a ring of fire?" );
				Caster.Target = new InternalTarget( this );
			}
		}

		public void Target( IPoint3D p )
		{
			if ( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				SpellHelper.Turn( Caster, p );

				SpellHelper.GetSurfaceTop( ref p );

				Effects.PlaySound( p, Caster.Map, 0x64F );

				double duration = DamagingSkill( Caster )/5;

				RingOfFire ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X, p.Y-3, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X-1, p.Y-2, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X-2, p.Y-1, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X-2, p.Y, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X-2, p.Y+1, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X-1, p.Y+2, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X, p.Y+3, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X+1, p.Y+2, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X+2, p.Y+1, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X+2, p.Y, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X+2, p.Y-1, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X+1, p.Y-2, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X, p.Y-4, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X-1, p.Y-3, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X-2, p.Y-2, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X+1, p.Y-3, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X+2, p.Y-2, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X-2, p.Y-2, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X-1, p.Y+3, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X, p.Y+4, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X+1, p.Y+3, p.Z), Caster.Map);
							ring = new RingOfFire( duration ); ring.MoveToWorld (new Point3D(p.X+2, p.Y+2, p.Z), Caster.Map);

				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ResearchRingofFire m_Owner;

			public InternalTarget( ResearchRingofFire owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is IPoint3D )
					m_Owner.Target( (IPoint3D)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}

namespace Server.Items
{
	public class RingOfFire : Item
	{
		public double lasts;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double Lasts
		{
			get{ return lasts; }
			set{ lasts = value; }
		}

		[Constructable]
		public RingOfFire() : this( 0.0 )
		{
		}

		[Constructable]
		public RingOfFire( double time )
		{
			ItemID = 0x19AB;
			Movable = false;
			Name = "magical fire";
			Light = LightType.Circle300;
			this.lasts = time;
			ItemRemovalTimer thisTimer = new ItemRemovalTimer( this, lasts ); 
			thisTimer.Start(); 
		}

		public RingOfFire(Serial serial) : base(serial)
		{
		}

		public override bool OnMoveOver( Mobile m )
		{
			SlayerEntry demon = SlayerGroup.GetEntryByName( SlayerName.Exorcism );

			if ( m is BaseCreature )
			{
				if ( demon.Slays(m) && ((BaseCreature)m).ControlMaster != m )
				{
					m.Mana = 0;
					return false;
				}
			}
			return true;
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
			this.Delete(); // none when the world starts 
		}

		public class ItemRemovalTimer : Timer 
		{ 
			private Item i_item; 
			public ItemRemovalTimer( Item item, Double lasts ) : base( TimeSpan.FromSeconds( lasts ) ) 
			{ 
				Priority = TimerPriority.OneSecond; 
				i_item = item; 
			} 

			protected override void OnTick() 
			{ 
				if (( i_item != null ) && ( !i_item.Deleted ))
				{
					i_item.Delete();
				}
			} 
		}
	}
}