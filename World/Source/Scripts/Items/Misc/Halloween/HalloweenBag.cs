using System;
using Server;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
	public class HalloweenCostume : Robe
	{
		[Constructable]
		public HalloweenCostume()
		{
			LootType = LootType.Blessed;
			ItemID = 9902;
			Weight = 100;

			if ( 2 > Utility.Random( 100 ) ) { Name="Dragon Costume"; Hue = 38; }
			else {
			switch ( Utility.RandomMinMax( 0, 24 ) )
			{
				case 0: Name="Ghost Costume"; Hue = 1150; break;
				case 1: Name="Mummy Costume"; Hue = 646; break;
				case 2: Name="Zombie Costume"; Hue = 357; break;
				case 3: Name="Vampire Costume"; Hue = 1316; break;
				case 4: Name="Skeleton Costume"; Hue = 761; break;
				case 5: Name="Devil Costume"; Hue = 1194; break;
				case 6: Name="Lizardman Costume"; Hue = 765; break;
				case 7: Name="Orc Costume"; Hue = 645; break;
				case 8: Name="Goblin Costume"; Hue = 61; break;
				case 9: Name="Harpy Costume"; Hue = 443; break;
				case 10: Name="Gargoyle Costume"; Hue = 440; break;
				case 11: Name="Headless Costume"; Hue = 546; break;
				case 12: Name="Lich Costume"; Hue = 919; break;
				case 13: Name="Toad Costume"; Hue = 268; break;
				case 14: Name="Elemental Costume"; Hue = 1159; break;
				case 15: Name="Centaur Costume"; Hue = 505; break;
				case 16: Name="Succubus Costume"; Hue = 617; break;
				case 17: Name="Werewolf Costume"; Hue = 2312; break;
				case 18: Name="Ape Costume"; Hue = 903; break;
				case 19: Name="Nightmare Costume"; Hue = 0x386; break;
				case 20: Name="Treant Costume"; Hue = 345; break;
				case 21: Name="Minotaur Costume"; Hue = 537; break;
				case 22: Name="Frankenstein Costume"; Hue = 446; break;
				case 23: Name="Swamp Thing Costume"; Hue = 752; break;
				case 24: Name="Black Knight Costume"; Hue = 1; break;
			}}
		}
		
		public override void OnRemoved( object parent )
		{      	
			Mobile owner = parent as Mobile;
	
			if (owner != null)
			{
				owner.HueMod = -1;
            	owner.BodyMod = 0;
				owner.RaceBody();
				owner.NameMod = null;
			}						
      		
		}		
	
		public override bool CanEquip( Mobile mob )
		{
			if ( mob.BodyMod > 0 )
			{
				mob.PublicOverheadMessage( MessageType.Regular, 0x44, false, "This costume will not fit in this form" );
				return false;
			}

			return base.CanEquip( mob );
		}

		public override bool OnEquip( Mobile mob )
		{
			if ( !base.OnEquip( mob ) )
				return false;

			if (this.Name == "Ghost Costume")
			{
				mob.BodyMod = 0x3CA;
				mob.HueMod = 1150;
				mob.NameMod = "Spooky Ghost";
			}
			else if (this.Name == "Mummy Costume")
			{
				mob.BodyMod = 154;
				mob.HueMod = 0;
				mob.NameMod = "Scary Mummy";
			}
			else if (this.Name == "Zombie Costume")
			{
				mob.BodyMod = 728;
				mob.HueMod = 0;
				mob.NameMod = "Creepy Zombie";
			}
			else if (this.Name == "Vampire Costume")
			{
				mob.BodyMod = 125;
				mob.HueMod = 0;
				mob.NameMod = "Terrifying Vampire";
			}
			else if (this.Name == "Skeleton Costume")
			{
				mob.BodyMod = 90;
				mob.HueMod = 0;
				mob.NameMod = "Frightening Skeleton";
			}
			else if (this.Name == "Devil Costume")
			{
				mob.BodyMod = 509;
				mob.HueMod = 0;
				mob.NameMod = "Horrifying Devil";
			}
			else if (this.Name == "Lizardman Costume")
			{
				mob.BodyMod = 534;
				mob.HueMod = 0;
				mob.NameMod = "Ugly Lizardman";
			}
			else if (this.Name == "Orc Costume")
			{
				mob.BodyMod = 7;
				mob.HueMod = 0;
				mob.NameMod = "Foul Orc";
			}
			else if (this.Name == "Goblin Costume")
			{
				mob.BodyMod = 632;
				mob.HueMod = 61;
				mob.NameMod = "Goofy Goblin";
			}
			else if (this.Name == "Harpy Costume")
			{
				mob.BodyMod = 153;
				mob.HueMod = 0;
				mob.NameMod = "Unsightly Harpy";
			}
			else if (this.Name == "Gargoyle Costume")
			{
				mob.BodyMod = 4;
				mob.HueMod = 0;
				mob.NameMod = "Horrid Gargoyle";
			}
			else if (this.Name == "Headless Costume")
			{
				mob.BodyMod = 31;
				mob.HueMod = 0;
				mob.NameMod = "Freaky Headless One";
			}
			else if (this.Name == "Lich Costume")
			{
				mob.BodyMod = 24;
				mob.HueMod = 0;
				mob.NameMod = "Dreadful Lich";
			}
			else if (this.Name == "Toad Costume")
			{
				mob.BodyMod = 270;
				mob.HueMod = 268;
				mob.NameMod = "Decrepit Toad";
			}
			else if (this.Name == "Elemental Costume")
			{
				switch ( Utility.RandomMinMax( 0, 3 ) )
				{
					case 0: mob.BodyMod = 14; break;
					case 1: mob.BodyMod = 13; break;
					case 2: mob.BodyMod = 15; break;
					case 3: mob.BodyMod = 16; break;
				}
				mob.HueMod = 0;
				mob.NameMod = "Monstrous Elemental";
			}
			else if (this.Name == "Centaur Costume")
			{
				mob.BodyMod = 101;
				mob.HueMod = 0;
				mob.NameMod = "Wild Centaur";
			}
			else if (this.Name == "Succubus Costume")
			{
				mob.BodyMod = 149;
				mob.HueMod = 0;
				mob.NameMod = "Evil Succubus";
			}
			else if (this.Name == "Dragon Costume")
			{
				mob.BodyMod = 59;
				mob.HueMod = 0;
				mob.NameMod = "Diabolical Dragon";
			}
			else if (this.Name == "Werewolf Costume")
			{
				mob.BodyMod = 708;
				mob.HueMod = 0;
				mob.NameMod = "Ravaging Werewolf";
			}
			else if (this.Name == "Ape Costume")
			{
				mob.BodyMod = 332;
				mob.HueMod = 0x902;
				mob.NameMod = "Primitive Ape";
			}
			else if (this.Name == "Nightmare Costume")
			{
				mob.BodyMod = 795;
				mob.HueMod = 0;
				mob.NameMod = "Worst Nightmare";
			}
			else if (this.Name == "Treant Costume")
			{
				mob.BodyMod = 309;
				mob.HueMod = 0;
				mob.NameMod = "Tranquil Treant";
			}
			else if (this.Name == "Minotaur Costume")
			{
				mob.BodyMod = 78;
				mob.HueMod = 0;
				mob.NameMod = "Mangy Minotaur";
			}
			else if (this.Name == "Frankenstein Costume")
			{
				mob.BodyMod = 69;
				mob.HueMod = 0;
				mob.NameMod = "Frankenstein's Monster";
			}
			else if (this.Name == "Swamp Thing Costume")
			{
				mob.BodyMod = 172;
				mob.HueMod = 0;
				mob.NameMod = "Swamp Thing";
			}
			else if (this.Name == "Black Knight Costume")
			{
				mob.BodyMod = 311;
				mob.HueMod = 0;
				mob.NameMod = "The Black Knight";
			}

			Mobiles.IMount mt = mob.Mount;
			if ( mt != null )
			{
				Server.Mobiles.EtherealMount.EthyDismount( mob );
				mt.Rider = null;
			}

			return true;
		}

		public HalloweenCostume( Serial serial ) : base( serial )
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
	public class ChocolateMonster : Item
	{
		[Constructable]
		public ChocolateMonster(): base()
		{
			Weight = 0.1;
			int Gift = Utility.RandomMinMax( 1, 9 );

			if (Gift == 1){
				Name = "Chocolate";
				ItemID = 12246;
				Hue = 1120;

				if ( 90 > Utility.Random( 100 ) ){
					switch ( Utility.RandomMinMax( 0, 18 ) )
					{
						case 0: Name="Chocolate Goat"; ItemID=9600; break;
						case 1: Name="Chocolate Centaur"; ItemID=9601; break;
						case 2: Name="Chocolate Demon"; ItemID=9604; break;
						case 3: Name="Chocolate Vampire"; ItemID=9610; break;
						case 4: Name="Chocolate Gargoyle"; ItemID=9613; break;
						case 5: Name="Chocolate Horse"; ItemID=9630; break;
						case 6: Name="Chocolate Fairy"; ItemID=9654; break;
						case 7: Name="Chocolate Unicorn"; ItemID=9678; break;
						case 8: Name="Chocolate Snake"; ItemID=9663; break;
						case 9: Name="Chocolate Ratman"; ItemID=9655; break;
						case 10: Name="Chocolate Zombie"; ItemID=9685; break;
						case 11: Name="Chocolate Minotaur"; ItemID=11657; break;
						case 12: Name="Chocolate Werewolf"; ItemID=11670; break;
						case 13: Name="Chocolate Dragon"; ItemID=8406; break;
						case 14: Name="Chocolate Beholder"; ItemID=8436; break;
						case 15: Name="Chocolate Ogre"; ItemID=8415; break;
						case 16: Name="Chocolate Lizardman"; ItemID=8414; break;
						case 17: Name="Chocolate Elemental"; ItemID=8435; break;
						case 18: Name="Chocolate Spider"; ItemID=8445; break;
					}
				}
			}
			else if (Gift == 2){Hue=1150; Name="Marshmallow"; ItemID=2538;}
			else if (Gift == 3){Hue=0; Name="Jaw Breaker"; ItemID=6251;}
			else if (Gift == 4){Hue=0; Name="Sour Drop"; ItemID=5930;}
			else if (Gift == 5){Hue=0; Name="Lemon Drop"; ItemID=5928;}
			else if (Gift == 6){Hue=2965; Name="Pixie Sticks"; ItemID=3978;}
			else if (Gift == 7){Hue=1167; Name="Jelly Beans"; ItemID=3611;}
			else if (Gift == 8){Hue=0; Name="Hard Candy"; ItemID=3967;}
			else {Hue=1195; Name="Gum Ball"; ItemID=3699;}
		}
		
		public override void OnDoubleClick( Mobile m )
		{
			m.PlaySound( Utility.Random( 0x3A, 3 ) );

			if ( m.Body.IsHuman && !m.Mounted )
				m.Animate( 34, 5, 1, true, false, 0 );

			String phrase = "";
			switch ( Utility.RandomMinMax( 0, 6 ) ) 
			{
				case 0: phrase = "Mmmmmmmmm"; break;
				case 1: phrase = "Tastes pretty good"; break;
				case 2: phrase = "I feel a sugar buzz coming on"; break;
				case 3: phrase = "I think it was stale"; break;
				case 4: phrase = "Do I feel a razor blade?"; break;
				case 5: phrase = "Ouch...my tooth!"; break;
				case 6: phrase = "This will go straight to the hips"; break;
			}

			m.PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, phrase, m.NetState);

			m.Stam = m.Stam + 300;
			m.Hits = m.Hits + 300;
			m.Mana = m.Mana + 300;
			m.Thirst = 20;
			m.Hunger = 20;
			this.Delete();
		}

		public ChocolateMonster( Serial serial ) : base( serial )
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
	public class HalloweenBag : BoneContainer
	{
		private static int[] m_Color = new int[]
			{
				0x386,
				1258,
				1194,
				1150
			};

		[Constructable]
		public HalloweenBag( )
        	{
           	Name = "Halloween Bones";
			Hue = m_Color[Utility.Random( m_Color.Length )];

			DropItem ( new HalloweenCostume() );
			DropItem ( new Pumpkin() );
			DropItem ( new Pumpkin() );
			DropItem ( new Pumpkin() );
			DropItem ( new TrickOrTreatBag() );

			int nGift = Utility.Random ( 2 );
			if ( nGift == 1 ) { DropItem( new CarvedPumpkin() ); }
			else { DropItem( new CarvedPumpkin2() ); }
		}

		public HalloweenBag(Serial serial) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version 
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}