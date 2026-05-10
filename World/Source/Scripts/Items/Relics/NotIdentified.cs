using Server.Mobiles;
using Server.Misc;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Server.Items
{
	public class NotIdentified : LockableContainer
	{
		public override bool DisplayLootType{ get{ return false; } }
		public override bool DisplaysContent{ get{ return false; } }
		public override bool DoNotCountContents{ get{ return true; } }

		[Constructable]
		public NotIdentified() : base( 0x9A8 )
		{
			Name = "unknown item";
			Locked = true;
			LockLevel = 1000;
			MaxLockLevel = 1000;
			RequiredSkill = 1000;
			Weight = 0.01;
			VirtualContainer = true;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from is PlayerMobile && ((PlayerMobile)from).Preferences.DoubleClickID && from.InRange( this.GetWorldLocation(), 3 ) )
				IDCommand( from );
		}

		public override bool TryDropItem( Mobile from, Item dropped, bool sendFullMessage )
		{
			return false;
		}

		public override bool CheckLocked( Mobile from )
		{
			return true;
		}

		public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
		{
			return false;
		}

		public override void IDCommand( Mobile m )
		{
			if ( this.NotIDSkill == IDSkill.Tasting )
				RelicFunctions.IDItem( m, m, this, SkillName.Tasting );
			else if ( this.NotIDSkill == IDSkill.ArmsLore )
				RelicFunctions.IDItem( m, m, this, SkillName.ArmsLore );
			else
				RelicFunctions.IDItem( m, m, this, SkillName.Mercantile );
		}

		public static void AddAsUnidentified( Item item, Container cont, Mobile m )
		{
			ResourceMods.DefaultItemHue( item );
			Container unk = new NotIdentified();
			unk.ItemID = item.ItemID;
			unk.Hue = item.Hue;
			unk.Name = RandomThings.GetOddityAdjective() + " item";
			unk.NotIdentified = true;
			unk.NotIDAttempts = 0;
			unk.NotIDSource = Identity.Merchant;
			unk.NotIDSkill = IDSkill.Mercantile;
			unk.CoinPrice = 100;
			unk.Weight = item.Weight;

			bool package = false;

			CraftResourceType resType = CraftResources.GetType( item.Resource );

			if ( item is BaseTrinket && item.Catalog == Catalogs.Trinket )
			{
				unk.NotIDSource = Identity.Artifact;
				package = true;
			}
			else if ( item is BaseTrinket )
			{
				unk.NotIDSource = Identity.Jewelry;
				package = true;
			}
			else if ( item.Catalog == Catalogs.Potion )
			{
				unk.NotIDSource = Identity.Potion;
				unk.NotIDSkill = IDSkill.Tasting;
				unk.ItemID = 0x2827;
				unk.Hue = Utility.RandomColor(0);
				unk.Name = RandomThings.GetOddityAdjective() + " bottle of liquid";
				if ( Worlds.isSciFiRegion( m ) )
				{
					unk.ItemID = 0x27FF;
					unk.Name = RandomThings.GetOddityAdjective() + " syringe of liquid";
				}
				if ( item.Amount > 1 )
				{
					unk.ColorText3 = "Amount: " + item.Amount + "";
					unk.ColorHue3 = "87E15A";
				}
				package = true;
			}
			else if ( item.Catalog == Catalogs.Reagent )
			{
				unk.NotIDSource = Identity.Reagent;
				unk.NotIDSkill = IDSkill.Tasting;
				unk.ItemID = 0x282F;
				unk.Hue = Utility.RandomColor(0);
				unk.Name = RandomThings.GetOddityAdjective() + " jar of reagents";
				if ( Worlds.isSciFiRegion( m ) )
				{
					unk.ItemID = 0x27FE;
					unk.Name = RandomThings.GetOddityAdjective() + " bottle of reagents";
				}
				if ( item.Amount > 1 )
				{
					unk.ColorText3 = "Amount: " + item.Amount + "";
					unk.ColorHue3 = "87E15A";
				}
				package = true;
			}
			else if ( item.Catalog == Catalogs.Scroll )
			{
				unk.NotIDSource = Identity.Scroll;
				unk.ItemID = Utility.RandomList( 0x4CC4, 0x4CC5 );
				unk.Hue = Utility.RandomColor(0);
				unk.Name = RandomThings.GetOddityAdjective() + " scroll";
				if ( item.Amount > 1 )
				{
					unk.ColorText3 = "Amount: " + item.Amount + "";
					unk.ColorHue3 = "87E15A";
				}
				package = true;
			}
			else if ( item is BaseWizardStaff || item is MagicalWand || item is BaseStaff || item is WizardWand || item is BaseWizardStaff || item is BaseLevelStave || item is BaseGiftStave || item is GiftScepter || item is LevelScepter || item is Scepter )
			{
				unk.NotIDSource = Identity.Wand;
				package = true;
			}
			else if ( item is BaseShoes )
			{
				unk.NotIDSource = Identity.Leather;
				package = true;
			}
			else if ( item is BaseClothing || resType == CraftResourceType.Fabric )
			{
				unk.NotIDSource = Identity.Clothing;
				package = true;
			}
			else if ( item is BaseQuiver )
			{
				unk.NotIDSource = Identity.Archer;
				package = true;
			}
			else if ( item is BaseInstrument )
			{
				unk.NotIDSource = Identity.Music;
				package = true;
			}
			else if ( item is Spellbook || item is Runebook )
			{
				unk.NotIDSource = Identity.Scroll;
				package = true;
			}
			else if ( item is BaseArmor && ( resType == CraftResourceType.Metal || resType == CraftResourceType.Scales || resType == CraftResourceType.Skeletal || resType == CraftResourceType.Block ) )
			{
				unk.NotIDSource = Identity.Armor;
				unk.NotIDSkill = IDSkill.ArmsLore;
				package = true;
			}
			else if ( item is BaseWeapon && ( resType == CraftResourceType.Metal || resType == CraftResourceType.Scales || resType == CraftResourceType.Skeletal || resType == CraftResourceType.Block ) )
			{
				unk.NotIDSource = Identity.Weapon;
				unk.NotIDSkill = IDSkill.ArmsLore;
				package = true;
			}
			else if ( ( item is BaseWeapon || item is BaseArmor ) && ( resType == CraftResourceType.Leather || resType == CraftResourceType.Skin ) )
			{
				unk.NotIDSource = Identity.Leather;
				unk.NotIDSkill = IDSkill.ArmsLore;
				package = true;
			}
			else if ( item is BaseRanged && resType == CraftResourceType.Wood )
			{
				unk.NotIDSource = Identity.Archer;
				unk.NotIDSkill = IDSkill.ArmsLore;
				package = true;
			}
			else if ( ( item is BaseWeapon || item is BaseArmor ) && resType == CraftResourceType.Wood )
			{
				unk.NotIDSource = Identity.Wood;
				unk.NotIDSkill = IDSkill.ArmsLore;
				package = true;
			}

			if ( package )
			{
				unk.DropItem(item);
				cont.DropItem(unk);
			}
			else
			{
                if (!cont.TryStackItem(item, null))
                    cont.DropItem(item);
                unk.Delete();
			}
		}

		public static void ConfigureItem( Item item, Container cont, Mobile m )
		{
			if ( MyServerSettings.GetUnidentifiedChance() >= Utility.RandomMinMax( 1, 100 ) )
			{
				AddAsUnidentified(item, cont, m );
			}
			else
            {
                if ( !cont.TryStackItem( item, null ) )
                    cont.DropItem( item );
			}
		}

		public static string IDVirConItem( NotIdentified unk, Mobile player )
		{
			string name = null;

			List<Item> items = new List<Item>();
			foreach (Item item in unk.Items)
			{
				items.Add(item);

				if ( item.ColorText1 != null )
					name = item.ColorText1;

				if ( item.ColorText2 != null )
					name = name + " " + item.ColorText2;

				if ( name == null )
					name = item.Name;
			}
			foreach (Item item in items)
			{
				if ( unk.Parent == null )
					item.MoveToWorld ( unk.Location, unk.Map );
				else if ( unk.Parent is Container )
				{
                    var parent = ((Container)(unk.Parent));
                    if (item.Stackable && parent.TryDropItem(player, item, false)) // Try to stack
                    {
                        if (item.Deleted) continue; // Auto-stacked
                    }
                    else
                        parent.AddItem(item); // If it bounced, force-add it back (it was there before being ID'd)

					item.Location = unk.Location;
				}
				else
					player.AddToBackpack ( item );
			}
			unk.Delete();

			return name;
		}

		public static void DoAutoDelete( Container container, Mobile mobile )
		{
			DoAutoDelete( container, mobile.Skills[SkillName.ArmsLore].Value, mobile.Skills[SkillName.Mercantile].Value, mobile.Skills[SkillName.Tasting].Value );
		}

		public static void DoAutoDelete( Container container, IEnumerable<Mobile> mobiles )
		{
			if ( mobiles == null || !mobiles.Any() ) return;

			double mercantileSkill = 0;
			double armsLoreSkill = 0;
			double tastingSkill = 0;

			foreach( var originalMobile in mobiles )
			{
				var mobile = originalMobile;
				if (mobile is BaseCreature)
					mobile = ((BaseCreature)mobile).ControlMaster;

				if (mobile is PlayerMobile)
				{
					// Pick the highest skill
					armsLoreSkill = Math.Max(armsLoreSkill, mobile.Skills[SkillName.ArmsLore].Value);
					mercantileSkill = Math.Max(mercantileSkill, mobile.Skills[SkillName.Mercantile].Value);
					tastingSkill = Math.Max(tastingSkill, mobile.Skills[SkillName.Tasting].Value);
				}
			}

			DoAutoDelete( container, armsLoreSkill, mercantileSkill, tastingSkill );
		}

		protected static void DoAutoDelete( Container container, double armsLoreSkill, double mercantileSkill, double tastingSkill )
		{
			if ( MySettings.S_UnidentifiedItem_GuaranteedItemChecks.Length < 1 ) return;

			// Determine how many items we should not delete
			int guaranteedArmsLoreItems = 0;
			int guaranteedMercantileItems = 0;
			int guaranteedTastingItems = 0;
			foreach( var breakpoint in MySettings.S_UnidentifiedItem_GuaranteedItemChecks )
			{
				if ( breakpoint <= armsLoreSkill ) guaranteedArmsLoreItems++;
				if ( breakpoint <= mercantileSkill ) guaranteedMercantileItems++;
				if ( breakpoint <= tastingSkill ) guaranteedTastingItems++;
			}

			const int SKILL_TO_GUARANTEE_ALL_DROPS = 125;
			var toDelete = container.Items
				.Where(item => item is NotIdentified)
				.Cast<NotIdentified>()
				.Where(item =>
				{
					// Never delete artifacts
					if (item.Items.Any(contained => 0 < contained.ArtifactLevel)) return false;

					double skillValue;
					switch (item.NotIDSkill)
					{
						case IDSkill.ArmsLore:
							skillValue = armsLoreSkill;

							if (0 < guaranteedArmsLoreItems)
							{
								guaranteedArmsLoreItems--;
								return false;
							}
							break;

						case IDSkill.Tasting:
							skillValue = tastingSkill;

							if (0 < guaranteedTastingItems)
							{
								guaranteedTastingItems--;
								return false;
							}
							break;

						case IDSkill.Mercantile:
						default:
							skillValue = mercantileSkill;

							if (0 < guaranteedMercantileItems)
							{
								guaranteedMercantileItems--;
								return false;
							}
							break;
					}

					if (SKILL_TO_GUARANTEE_ALL_DROPS <= skillValue) return false;

					// Roll against player skill
					return skillValue < Utility.RandomMinMax(0, SKILL_TO_GUARANTEE_ALL_DROPS);
				});

			if (toDelete.Any())
			{
				toDelete
					.ToList()
					.ForEach(item => item.Delete());
			}
		}

		public static string CannotIDVirConItem( NotIdentified unk, Mobile player )
		{
			if ( unk.NotIDAttempts + 1 > Item.MAX_ID_ATTEMPTS )
				 return "Only a vendor can identify this item now as too many attempts were made.";
			
			unk.NotIDAttempts++;

			return  "You cannot seem to identify this item.";
		}

		public NotIdentified( Serial serial ) : base( serial )
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