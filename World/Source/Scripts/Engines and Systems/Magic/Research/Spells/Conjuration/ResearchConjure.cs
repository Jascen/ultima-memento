using System;
using Server.Targeting;
using Server.Items;

namespace Server.Spells.Research
{
	public class ResearchConjure : ResearchSpell
	{
		public override bool alwaysConsume { get{ return bool.Parse( Server.Misc.Research.SpellInformation( spellIndex, 14 )); } }
		public int CirclePower = 1;
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.0 ); } }
		public override double RequiredSkill{ get{ return SpellInfo.SpellDefinition.MinSkill; } }
		public override int RequiredMana{ get{ return SpellInfo.SpellDefinition.ManaCost; } }

		public static readonly SpellInfo SpellInfo = new SpellInfo(
				new SpellDefinition
				{
					SpellID = 610,
					IconGraphic = 11195,
					Name = "Conjure",
					PowerWords = "Kal Xen",
					Description = "The magic of this spell will conjure a random item of minor significance, but it may be significant dependent on the caster's current motivations. Due to the creative nature of the spell, something needs to be destroyed in turn. That means that the spell scroll will always crumble to dust when cast.",
					ManaCost = 10,
					MinSkill = 15,
					TargetType = TargetFlags.None
				},
				203,
				9041,
				Reagent.MoonCrystal,Reagent.FairyEgg
			);

		public ResearchConjure( Mobile caster, Item scroll ) : base( caster, scroll, SpellInfo )
		{
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				Item item = new Dagger();
				string msg = "You conjure a dagger.";

				switch( Utility.RandomMinMax( 1, 27 ) )
				{
					case 1: item = new Apple(); item.Amount = Utility.RandomMinMax( 1, 5 ); msg = "You conjure some apples.";
						if ( Server.Items.BaseRace.BloodDrinker( Caster.RaceID ) ){ item = new BloodyDrink(); item.Amount = Utility.RandomMinMax( 1, 5 ); msg = "You conjure fresh blood."; }
						else if ( Server.Items.BaseRace.BrainEater( Caster.RaceID ) ){ item = new FreshBrain(); item.Amount = Utility.RandomMinMax( 1, 5 ); msg = "You conjure fresh brains."; }
					break;
					case 2: item = new Arrow(); item.Amount = Utility.RandomMinMax( 1, 10 ); msg = "You conjure some arrows."; break;
					case 3: item = new Backpack(); msg = "You conjure a backpack."; break;
					case 4: item = new Bag(); msg = "You conjure a bag."; break;
					case 5: item = new Bandage(); item.Amount = Utility.RandomMinMax( 1, 10 ); msg = "You conjure some bandages."; break;
					case 6: item = new Bedroll(); msg = "You conjure a bedroll."; break;
					case 7: item = new Beeswax(); msg = "You conjure some beeswax."; break;
					case 8: item = new WritingBook(); msg = "You conjure a book."; break;
					case 9: item = new Bolt(); item.Amount = Utility.RandomMinMax( 1, 10 ); msg = "You conjure some crossbow bolts."; break;
					case 10: item = new Bottle(); msg = "You conjure a bottle."; break;
					case 11: item = new BreadLoaf(); item.Amount = Utility.RandomMinMax( 1, 5 ); msg = "You conjure some bread.";
						if ( Server.Items.BaseRace.BloodDrinker( Caster.RaceID ) ){ item = new BloodyDrink(); item.Amount = Utility.RandomMinMax( 1, 5 ); msg = "You conjure fresh blood."; }
						else if ( Server.Items.BaseRace.BrainEater( Caster.RaceID ) ){ item = new FreshBrain(); item.Amount = Utility.RandomMinMax( 1, 5 ); msg = "You conjure fresh brains."; }
					break;
					case 12: item = new Candle(); msg = "You conjure a candle."; break;
					case 13: item = new Club(); msg = "You conjure a club."; break;
					case 14: item = new Dagger(); msg = "You conjure a dagger."; break;
					case 15: item = new FloppyHat(); msg = "You conjure a hat."; break;
					case 16: item = new Jar(); msg = "You conjure a jar."; break;
					case 17: item = new Kindling(); item.Amount = Utility.RandomMinMax( 1, 5 ); msg = "You conjure some kindling."; break;
					case 18: item = new Lantern(); msg = "You conjure a lantern."; break;
					case 19: item = new Lockpick(); msg = "You conjure a lockpick."; break;
					case 20: item = new OilCloth(); msg = "You conjure an oil cloth."; break;
					case 21: item = new Pouch(); msg = "You conjure a pouch."; break;
					case 22: item = new Robe(); msg = "You conjure a robe."; break;
					case 23: item = new Shoes(); msg = "You conjure some shoes."; break;
					case 24: item = new SpoolOfThread(); item.Amount = Utility.RandomMinMax( 1, 5 ); msg = "You conjure some string."; break;
					case 25: item = new TenFootPole(); msg = "You conjure a ten foot pole."; break;
					case 26: item = new Torch(); msg = "You conjure a torch."; break;
					case 27: item = new Pitcher( BeverageType.Water ); msg = "You conjure a decanter of water."; 
						if ( Server.Items.BaseRace.BloodDrinker( Caster.RaceID ) ){ item = new BloodyDrink(); item.Amount = Utility.RandomMinMax( 1, 5 ); msg = "You conjure fresh blood."; }
						else if ( Server.Items.BaseRace.BrainEater( Caster.RaceID ) ){ item = new FreshBrain(); item.Amount = Utility.RandomMinMax( 1, 5 ); msg = "You conjure fresh brains."; }
					break;
				}

				Caster.SendMessage( msg );

				Caster.AddToBackpack( item );

				Caster.FixedParticles( 0, 10, 5, 2003, Server.Misc.PlayerSettings.GetMySpellHue( true, Caster, 0 ), 0, EffectLayer.RightHand );
				Caster.PlaySound( 0x1E2 );

				Server.Misc.Research.ConsumeScroll( Caster, true, spellIndex, alwaysConsume, Scroll );
			}

			FinishSequence();
		}
	}
}
