using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Items;
using System.Collections.Generic;
using System.Collections;

namespace Server.Spells.Research
{
	public class ResearchMaskofDeath : ResearchSpell
	{
		public override int spellIndex { get { return 34; } }
		public int CirclePower = 8;
		public static int spellID = 34;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }
		public override double RequiredSkill{ get{ return (double)(Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 8 ))); } }
		public override int RequiredMana{ get{ return Int32.Parse( Server.Misc.Research.SpellInformation( spellIndex, 7 )); } }

		private static SpellInfo m_Info = new SpellInfo(
				Server.Misc.Research.SpellInformation( spellID, 2 ),
				Server.Misc.Research.CapsCast( Server.Misc.Research.SpellInformation( spellID, 4 ) ),
				212,
				9001,
				Reagent.LichDust,Reagent.GraveDust,Reagent.PixieSkull
			);

		public ResearchMaskofDeath( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				ArrayList targets = new ArrayList();
				foreach ( Item item in World.Items.Values )
				if ( item is DeathlyMask )
				{
					DeathlyMask myMask = (DeathlyMask)item;
					if ( myMask.owner == Caster )
					{
						targets.Add( item );
					}
				}
				for ( int i = 0; i < targets.Count; ++i )
				{
					Item item = ( Item )targets[ i ];
					item.Delete();
				}

				double time = DamagingSkill( Caster )*4;
					if ( time > 900 ){ time = 900.0; }
					if ( time < 360 ){ time = 360.0; }

				Caster.PlaySound( 0x1ED );
				Caster.FixedParticles( 0x376A, 9, 32, 5008, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.Waist );
				Caster.SendMessage( "You summon the mask of death into your pack." );
				Item iMask = new DeathlyMask(Caster,time);
				Caster.AddToBackpack( iMask );
				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );

				BuffInfo.RemoveBuff( Caster, BuffIcon.MaskDeath );
				BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.MaskDeath, 1063642, 1063643, TimeSpan.FromSeconds( time ), Caster ));

				KarmaMod( Caster, ((int)RequiredSkill+RequiredMana) );
			}
			FinishSequence();
		}
	}
}

namespace Server.Items
{
	public class DeathlyMask : ClothCowl
	{
		public Mobile owner;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner
		{
			get{ return owner; }
			set{ owner = value; }
		}

		public double lasts;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double Lasts
		{
			get{ return lasts; }
			set{ lasts = value; }
		}

		[Constructable]
		public DeathlyMask() : this( null, 0.0 )
		{
		}

		[Constructable]
		public DeathlyMask( Mobile from, double time )
		{
			ItemID = 0x1451;
			Hue = 0xB7F;
			Name = "mask of death";
			this.owner = from;
			this.lasts = time;
			Weight = 1.0; 
			LootType = LootType.Blessed;						
			ItemRemovalTimer thisTimer = new ItemRemovalTimer( this, lasts ); 
			thisTimer.Start(); 
		}

		public DeathlyMask( Serial serial ) : base( serial )
		{
		}

		public override bool CanEquip( Mobile from )
		{
			if ( owner != from )
				return false;

			return base.CanEquip( from );
		}

		public override bool OnEquip( Mobile from )
		{
			return base.OnEquip( from );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			writer.Write( (Mobile)owner );
			writer.Write( lasts );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			owner = reader.ReadMobile();
			lasts = reader.ReadDouble();
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
					DeathlyMask masks = (DeathlyMask)i_item;
					Mobile from = masks.owner;
					from.LocalOverheadMessage(Network.MessageType.Emote, 0x3B2, false, "The mask of death has vanished.");
					from.PlaySound( 0x1F0 );
					i_item.Delete();
				}
			} 
		} 
	}
}