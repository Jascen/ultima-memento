using Server;
using System;
using System.Collections;
using Server.Network;
using Server.Targeting;
using Server.Prompts;
using Server.Misc;

namespace Server.Items
{
	public class MagicScissors : Item
	{
		[Constructable]
		public MagicScissors() : base( 0xDFC )
		{
			Weight = 1.0;
			ItemID = Utility.RandomList( 0xDFC, 0xDFD );
			Hue = 0x489;
			Name = "magical scissors";
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);
			list.Add( 1070722, "Changes the Appearance of Clothes");
            list.Add( 1049644, "Belts, Boots, Cloaks, Hats, and Robes");
        } 

		public override void OnDoubleClick( Mobile from )
		{
			Target t;

			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1060640 ); // The item must be in your backpack to use it.
			}
			else
			{
				from.SendMessage( "What do you want to use the scissors on?" );
				t = new WearTarget( this );
				from.Target = t;
			}
		}

		private class WearTarget : Target
		{
			private MagicScissors m_Wear;

			public WearTarget( MagicScissors cutters ) : base( 1, false, TargetFlags.None )
			{
				m_Wear = cutters;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Item )
				{
					bool DoEffects = false;
					Item iWear = targeted as Item;

					string OldName = null;
					string NewName = null;

					if ( iWear.RootParentEntity != from )
					{
						from.SendMessage( "You can only use these scissors on items in your possession." );
					}
					else if ( !MyServerSettings.AlterArtifact( iWear ) )
					{
						from.SendMessage( "This cannot be used on artifacts!" );
					}
					else if ( iWear is BaseArmor && ( 
						iWear.ItemID == 0x1451 || 
						iWear.ItemID == 0x1456 || 
						iWear.ItemID == 0x49C1 || 
						iWear.ItemID == 0x1452 || 
						iWear.ItemID == 0x1457 || 
						iWear.ItemID == 0x49C2 || 
						iWear.ItemID == 0x144e || 
						iWear.ItemID == 0x1453 || 
						iWear.ItemID == 0x4988 || 
						iWear.ItemID == 0x144f || 
						iWear.ItemID == 0x1454 || 
						iWear.ItemID == 0x498F || 
						iWear.ItemID == 0x1450 || 
						iWear.ItemID == 0x1455 || 
						iWear.ItemID == 0x499D
					) )
					{
						if ( iWear.ItemID == 0x1451 || iWear.ItemID == 0x1456 ){ DoEffects = true; iWear.ItemID = 0x49C1; OldName = "bone helm"; NewName = "bone helm"; }
						else if ( iWear.ItemID == 0x49C1 ){ DoEffects = true; iWear.ItemID = 0x1451; OldName = "bone helm"; NewName = "bone helm"; }
						else if ( iWear.ItemID == 0x1452 || iWear.ItemID == 0x1457 ){ DoEffects = true; iWear.ItemID = 0x49C2; OldName = "bone leggings"; NewName = "bone leggings"; }
						else if ( iWear.ItemID == 0x49C2 ){ DoEffects = true; iWear.ItemID = 0x1452; OldName = "bone leggings"; NewName = "bone leggings"; }
						else if ( iWear.ItemID == 0x144e || iWear.ItemID == 0x1453 ){ DoEffects = true; iWear.ItemID = 0x4988; OldName = "bone arms"; NewName = "bone arms"; }
						else if ( iWear.ItemID == 0x4988 ){ DoEffects = true; iWear.ItemID = 0x144e; OldName = "bone arms"; NewName = "bone arms"; }
						else if ( iWear.ItemID == 0x144f || iWear.ItemID == 0x1454 ){ DoEffects = true; iWear.ItemID = 0x498F; OldName = "bone tunic"; NewName = "bone tunic"; }
						else if ( iWear.ItemID == 0x498F ){ DoEffects = true; iWear.ItemID = 0x144f; OldName = "bone tunic"; NewName = "bone tunic"; }
						else if ( iWear.ItemID == 0x1450 || iWear.ItemID == 0x1455 ){ DoEffects = true; iWear.ItemID = 0x499D; OldName = "bone gloves"; NewName = "bone gloves"; }
						else if ( iWear.ItemID == 0x499D ){ DoEffects = true; iWear.ItemID = 0x1450; OldName = "bone gloves"; NewName = "bone gloves"; }
					}
					else if ( iWear is BaseArmor && ( 
						iWear.ItemID == 0x13CC || 
						iWear.ItemID == 0x13d3 || 
						iWear.ItemID == 0x264F || 
						iWear.ItemID == 0x2650
					) )
					{
						if ( iWear.ItemID == 0x13CC || iWear.ItemID == 0x13d3 ){ DoEffects = true; iWear.ItemID = 0x264F; OldName = "leather tunic"; NewName = "leather tunic"; }
						else if ( iWear.ItemID == 0x264F ){ DoEffects = true; iWear.ItemID = 0x2650; OldName = "leather tunic"; NewName = "leather tunic"; }
						else if ( iWear.ItemID == 0x2650 ){ DoEffects = true; iWear.ItemID = 0x13CC; OldName = "leather tunic"; NewName = "leather tunic"; }
					}
					else if (	iWear is BaseClothing || 
								iWear is BaseGiftClothing || 
								iWear is BaseLevelClothing || 
								iWear is LevelBelt || 
								iWear is GiftBelt || 
								iWear is LeatherCloak || 
								iWear is LeatherCap || 
								iWear is LeatherRobe || 
								iWear is ShinobiRobe || 
								iWear is ShinobiCowl || 
								iWear is ShinobiHood || 
								iWear is ShinobiMask || 
								iWear is LeatherSandals || 
								iWear is LeatherShoes || 
								iWear is LeatherBoots || 
								iWear is HikingBoots || 
								iWear is LeatherThighBoots || 
								iWear is LeatherSoftBoots || 
								iWear is LevelLeatherCap || 
								iWear is GiftLeatherCap )
					{
						if ( iWear is BaseClothing && iWear.Layer == Layer.MiddleTorso && iWear is BaseOuterTorso && ( iWear.ItemID == 0x1541 || iWear.ItemID == 0x0409 ) )
						{
							DoEffects = true; iWear.ItemID = 0x1F04; OldName = "sash"; NewName = "robe"; iWear.Layer = Layer.OuterTorso;
						}
						else if ( iWear.ItemID == 0x55DB ){ DoEffects = true; iWear.ItemID = 0x2B68; OldName = "royal loin cloth"; NewName = "loin cloth"; }
						else if ( iWear.ItemID == 0x2B68 ){ DoEffects = true; iWear.ItemID = 0x567B; OldName = "loin cloth"; NewName = "belt"; }
						else if ( iWear.ItemID == 0x567B ){ DoEffects = true; iWear.ItemID = 0x2790; OldName = "belt"; NewName = "belt"; }
						else if ( iWear.ItemID == 0x2790 ){ DoEffects = true; iWear.ItemID = 0x55DB; OldName = "belt"; NewName = "royal loin cloth"; }

						else if ( iWear.ItemID == 0x26AD ){ DoEffects = true; iWear.ItemID = 0x2B04; OldName = "cloak"; NewName = "cloak"; }
						else if ( iWear.ItemID == 0x1515 || iWear.ItemID == 0x1530 ){ DoEffects = true; iWear.ItemID = 0x26AD; OldName = "cloak"; NewName = "cloak"; }
						else if ( iWear.ItemID == 0x2B04 ){ DoEffects = true; iWear.ItemID = 0x1515; OldName = "cloak"; NewName = "cloak"; }

						else if ( iWear.ItemID == 0x1711 ){ DoEffects = true; iWear.ItemID = 0x170f; OldName = "boots"; NewName = "shoes"; }
						else if ( iWear.ItemID == 0x170B ){ DoEffects = true; iWear.ItemID = 0x1711; OldName = "boots"; NewName = "boots"; }
						else if ( iWear.ItemID == 0x567C ){ DoEffects = true; iWear.ItemID = 0x170B; OldName = "boots"; NewName = "boots"; }
						else if ( iWear.ItemID == 0x4C27 ){ DoEffects = true; iWear.ItemID = 0x567C; OldName = "jester shoes"; NewName = "boots"; }
						else if ( iWear.ItemID == 0x2307 ){ DoEffects = true; iWear.ItemID = 0x4C27; OldName = "boots"; NewName = "jester shoes"; }
						else if ( iWear.ItemID == 0x4C26 ){ DoEffects = true; iWear.ItemID = 0x2307; OldName = "boots"; NewName = "boots"; }
						else if ( iWear.ItemID == 0x315E ){ DoEffects = true; iWear.ItemID = 0x4C26; OldName = "boots"; NewName = "boots"; }
						else if ( iWear.ItemID == 0x2B67 ){ DoEffects = true; iWear.ItemID = 0x315E; OldName = "boots"; NewName = "boots"; }
						else if ( iWear.ItemID == 0x26AF ){ DoEffects = true; iWear.ItemID = 0x2B67; OldName = "boots"; NewName = "boots"; }
						else if ( iWear.ItemID == 0x406 ){ DoEffects = true; iWear.ItemID = 0x26AF; OldName = "boots"; NewName = "boots"; }
						else if ( iWear.ItemID == 0x2FC4 ){ DoEffects = true; iWear.ItemID = 0x406; OldName = "boots"; NewName = "barbarian boots"; }
						else if ( iWear.ItemID == 0x2797 ){ DoEffects = true; iWear.ItemID = 0x2FC4; OldName = "samurai tabi"; NewName = "boots"; }
						else if ( iWear.ItemID == 0x64BA ){ DoEffects = true; iWear.ItemID = 0x2797; OldName = "oniwaban boots"; NewName = "samurai tabi"; }
						else if ( iWear.ItemID == 0x27E2 ){ DoEffects = true; iWear.ItemID = 0x64BA; OldName = "ninja tabi"; NewName = "oniwaban boots"; }
						else if ( iWear.ItemID == 0x2796 || iWear.ItemID == 0x27E1 ){ DoEffects = true; iWear.ItemID = 0x27E2; OldName = "waraji"; NewName = "ninja tabi"; }
						else if ( iWear.ItemID == 0x170d || iWear.ItemID == 0x170e ){ DoEffects = true; iWear.ItemID = 0x2796; OldName = "sandals"; NewName = "waraji"; }
						else if ( iWear.ItemID == 0x170f || iWear.ItemID == 0x1710 ){ DoEffects = true; iWear.ItemID = 0x170d; OldName = "shoes"; NewName = "sandals"; }

						else if ( iWear.ItemID == 0x5C10 ){ DoEffects = true; iWear.ItemID = 0x2FC6; OldName = "shinobi robe"; NewName = "spider robe"; }
						else if ( iWear.ItemID == 0x26AE ){ DoEffects = true; iWear.ItemID = 0x5C10; OldName = "robe"; NewName = "shinobi robe"; }
						else if ( iWear.ItemID == 0x1F04 || iWear.ItemID == 0x1F03 ){ DoEffects = true; iWear.ItemID = 0x26AE; OldName = "robe"; NewName = "robe"; }
						else if ( iWear.ItemID == 0x2684 || iWear.ItemID == 0x2683 || iWear.ItemID == 0x204E || iWear.ItemID == 0x2685 || iWear.ItemID == 0x2686 || iWear.ItemID == 0x2687 ){ DoEffects = true; iWear.ItemID = 0x1F04; OldName = "robe"; NewName = "robe"; }
						else if ( iWear.ItemID == 0x2799 || iWear.ItemID == 0x27E4 ){ DoEffects = true; iWear.ItemID = 0x2684; OldName = "robe"; NewName = "robe"; }
						else if ( iWear.ItemID == 0x230E ){ DoEffects = true; iWear.ItemID = 0x2799; OldName = "gilded dress"; NewName = "robe"; }
						else if ( iWear.ItemID == 0x230D ){ DoEffects = true; iWear.ItemID = 0x230E; OldName = "gilded dress"; NewName = "gilded dress"; }
						else if ( iWear.ItemID == 0x1EFF ){ DoEffects = true; iWear.ItemID = 0x230D; OldName = "fancy dress"; NewName = "gilded dress"; }
						else if ( iWear.ItemID == 0x1F00 ){ DoEffects = true; iWear.ItemID = 0x1EFF; OldName = "fancy dress"; NewName = "fancy dress"; }
						else if ( iWear.ItemID == 0x1f02 ){ DoEffects = true; iWear.ItemID = 0x1F00; OldName = "dress"; NewName = "fancy dress"; }
						else if ( iWear.ItemID == 0x1f01 ){ DoEffects = true; iWear.ItemID = 0x1f02; OldName = "dress"; NewName = "dress"; }
						else if ( iWear.ItemID == 0x279C || iWear.ItemID == 0x27E7 ){ DoEffects = true; iWear.ItemID = 0x1f01; OldName = "hakama shita"; NewName = "dress"; }
						else if ( iWear.ItemID == 0x2782 || iWear.ItemID == 0x27CD ){ DoEffects = true; iWear.ItemID = 0x279C; OldName = "male kimono"; NewName = "hakama shita"; }
						else if ( iWear.ItemID == 0x2783 || iWear.ItemID == 0x27CE ){ DoEffects = true; iWear.ItemID = 0x2782; OldName = "female kimono"; NewName = "male kimono"; }
						else if ( iWear.ItemID == 0x2B6B || iWear.ItemID == 0x3162 ){ DoEffects = true; iWear.ItemID = 0x2783; OldName = "jester robe"; NewName = "female kimono"; }
						else if ( iWear.ItemID == 0x4C16 ){ DoEffects = true; iWear.ItemID = 0x2B6B; OldName = "jester garb"; NewName = "jester robe"; }
						else if ( iWear.ItemID == 0x4C17 ){ DoEffects = true; iWear.ItemID = 0x4C16; OldName = "fool's coat"; NewName = "jester garb"; }
						else if ( iWear.ItemID == 0x2B69 || iWear.ItemID == 0x3160 ){ DoEffects = true; iWear.ItemID = 0x4C17; OldName = "assassin robe"; NewName = "fool's coat"; }
						else if ( iWear.ItemID == 0x2B6A || iWear.ItemID == 0x3161 ){ DoEffects = true; iWear.ItemID = 0x2B69; OldName = "fancy robe"; NewName = "assassin robe"; }
						else if ( iWear.ItemID == 0x2B6C || iWear.ItemID == 0x3163 ){ DoEffects = true; iWear.ItemID = 0x2B6A; OldName = "gilded robe"; NewName = "fancy robe"; }
						else if ( iWear.ItemID == 0x2B6E || iWear.ItemID == 0x3165 ){ DoEffects = true; iWear.ItemID = 0x2B6C; OldName = "ornate robe"; NewName = "gilded robe"; }
						else if ( iWear.ItemID == 0x201B || iWear.ItemID == 0x201C ){ DoEffects = true; iWear.ItemID = 0x2B6E; OldName = "dragon robe"; NewName = "ornate robe"; }
						else if ( iWear.ItemID == 0x201F || iWear.ItemID == 0x2020 ){ DoEffects = true; iWear.ItemID = 0x201B; OldName = "chaos robe"; NewName = "dragon robe"; }
						else if ( iWear.ItemID == 0x201D || iWear.ItemID == 0x201E ){ DoEffects = true; iWear.ItemID = 0x201F; OldName = "vampire robe"; NewName = "chaos robe"; }
						else if ( iWear.ItemID == 0x2B70 || iWear.ItemID == 0x3167 ){ DoEffects = true; iWear.ItemID = 0x201D; OldName = "magistrate robe"; NewName = "vampire robe"; }
						else if ( iWear.ItemID == 0x283 ){ DoEffects = true; iWear.ItemID = 0x2B70; OldName = "exquisite robe"; NewName = "magistrate robe"; }
						else if ( iWear.ItemID == 0x284 ){ DoEffects = true; iWear.ItemID = 0x283; OldName = "prophet robe"; NewName = "exquisite robe"; }
						else if ( iWear.ItemID == 0x285 ){ DoEffects = true; iWear.ItemID = 0x284; OldName = "elegant robe"; NewName = "prophet robe"; }
						else if ( iWear.ItemID == 0x286 ){ DoEffects = true; iWear.ItemID = 0x285; OldName = "formal robe"; NewName = "elegant robe"; }
						else if ( iWear.ItemID == 0x287 ){ DoEffects = true; iWear.ItemID = 0x286; OldName = "archmage robe"; NewName = "formal robe"; }
						else if ( iWear.ItemID == 0x288 ){ DoEffects = true; iWear.ItemID = 0x287; OldName = "priest robe"; NewName = "archmage robe"; }
						else if ( iWear.ItemID == 0x289 ){ DoEffects = true; iWear.ItemID = 0x288; OldName = "cultist robe"; NewName = "priest robe"; }
						else if ( iWear.ItemID == 0x28A ){ DoEffects = true; iWear.ItemID = 0x289; OldName = "gilded dark robe"; NewName = "cultist robe"; }
						else if ( iWear.ItemID == 0x301 ){ DoEffects = true; iWear.ItemID = 0x28A; OldName = "gilded light robe"; NewName = "gilded dark robe"; }
						else if ( iWear.ItemID == 0x302 ){ DoEffects = true; iWear.ItemID = 0x301; OldName = "sage robe"; NewName = "gilded light robe"; }
						else if ( iWear.ItemID == 0x2B73 || iWear.ItemID == 0x316A ){ DoEffects = true; iWear.ItemID = 0x302; OldName = "royal robe"; NewName = "sage robe"; }
						else if ( iWear.ItemID == 0x3175 || iWear.ItemID == 0x3178 ){ DoEffects = true; iWear.ItemID = 0x2B73; OldName = "sorcerer robe"; NewName = "royal robe"; }
						else if ( iWear.ItemID == 0x2652 || iWear.ItemID == 0x3173 ){ DoEffects = true; iWear.ItemID = 0x3175; OldName = "scholar robe"; NewName = "sorcerer robe"; }
						else if ( iWear.ItemID == 0x567E ){ DoEffects = true; iWear.ItemID = 0x2652; OldName = "pirate coat"; NewName = "scholar robe"; }
						else if ( iWear.ItemID == 0x567D ){ DoEffects = true; iWear.ItemID = 0x567E; OldName = "vagabond robe"; NewName = "pirate coat"; }
						else if ( iWear.ItemID == 0x2FBA || iWear.ItemID == 0x3174 ){ DoEffects = true; iWear.ItemID = 0x567D; OldName = "necromancer robe"; NewName = "vagabond robe"; }
						else if ( iWear.ItemID == 0x2FC6 || iWear.ItemID == 0x2FC7 ){ DoEffects = true; iWear.ItemID = 0x2FBA; OldName = "spider robe"; NewName = "necromancer robe"; }

						else if ( iWear.ItemID == 0x5C13 ){ DoEffects = true; iWear.ItemID = 0x49C3; OldName = "shinobi cowl"; NewName = "stag mask"; }
						else if ( iWear.ItemID == 0x49C3 ){ DoEffects = true; iWear.ItemID = 0x1547; OldName = "stag mask"; NewName = "deerskin cap"; }
						else if ( iWear.ItemID == 0x1547 || iWear.ItemID == 0x1548 ){ DoEffects = true; iWear.ItemID = 0x1545; OldName = "deerskin cap"; NewName = "bearskin cap"; }
						else if ( iWear.ItemID == 0x1545 || iWear.ItemID == 0x1546 ){ DoEffects = true; iWear.ItemID = 0x154B; OldName = "bearskin cap"; NewName = "tribal mask"; }
						else if ( iWear.ItemID == 0x154B ){ DoEffects = true; iWear.ItemID = 0x1549; OldName = "tribal mask"; NewName = "shaman mask"; }
						else if ( iWear.ItemID == 0x1549 ){ DoEffects = true; iWear.ItemID = 0x1540; OldName = "shaman mask"; NewName = "bandana"; }
						else if ( iWear.ItemID == 0x1540 ){ DoEffects = true; iWear.ItemID = 0x27DA; OldName = "bandana"; NewName = "ninja hood"; }
						else if ( iWear.ItemID == 0x27DA ){ DoEffects = true; iWear.ItemID = 0x278F; OldName = "ninja hood"; NewName = "executioner hood"; }
						else if ( iWear.ItemID == 0x278F ){ DoEffects = true; iWear.ItemID = 0x2798; OldName = "executioner hood"; NewName = "kasa"; }
						else if ( iWear.ItemID == 0x2798 || iWear.ItemID == 0x27E3 ){ DoEffects = true; iWear.ItemID = 5909; OldName = "kasa"; NewName = "bonnet"; }
						else if ( iWear.ItemID == 5909 ){ DoEffects = true; iWear.ItemID = 5444; OldName = "bonnet"; NewName = "skullcap"; }
						else if ( iWear.ItemID == 5444 ){ DoEffects = true; iWear.ItemID = 0x2FC3; OldName = "skullcap"; NewName = "witch hat"; }
						else if ( iWear.ItemID == 0x2FC3 || iWear.ItemID == 0x3179 ){ DoEffects = true; iWear.ItemID = 0x2B6D; OldName = "witch hat"; NewName = "wolfskin cap"; }
						else if ( iWear.ItemID == 0x2B6D || iWear.ItemID == 0x3164 ){ DoEffects = true; iWear.ItemID = 0x2B71; OldName = "wolfskin cap"; NewName = "hood"; }
						else if ( iWear.ItemID == 0x2B71 || iWear.ItemID == 0x3168 ){ DoEffects = true; iWear.ItemID = 0x3176; OldName = "hood"; NewName = "cowl"; }
						else if ( iWear.ItemID == 0x3176 || iWear.ItemID == 0x3177 ){ DoEffects = true; iWear.ItemID = 0x5C14; OldName = "cowl"; NewName = "hooded mantle"; }
						else if ( iWear.ItemID == 0x5C14 ){ DoEffects = true; iWear.ItemID = 5907; OldName = "hooded mantle"; NewName = "floppy hat"; }
						else if ( iWear.ItemID == 5907 ){ DoEffects = true; iWear.ItemID = 0x4D09; OldName = "floppy hat"; NewName = "fancy hood"; }
						else if ( iWear.ItemID == 0x4D09 ){ DoEffects = true; iWear.ItemID = 0x4CDB; OldName = "fancy hood"; NewName = "reaper hood"; }
						else if ( iWear.ItemID == 0x4CDB ){ DoEffects = true; iWear.ItemID = 0x4CDD; OldName = "reaper hood"; NewName = "reaper cowl"; }
						else if ( iWear.ItemID == 0x4CDD ){ DoEffects = true; iWear.ItemID = 0x4D01; OldName = "reaper cowl"; NewName = "daemon cowl"; }
						else if ( iWear.ItemID == 0x4D01 ){ DoEffects = true; iWear.ItemID = 0x4D02; OldName = "daemon cowl"; NewName = "lizard cowl"; }
						else if ( iWear.ItemID == 0x4D02 ){ DoEffects = true; iWear.ItemID = 0x405; OldName = "lizard cowl"; NewName = "mask of the dead"; }
						else if ( iWear.ItemID == 0x405 ){ DoEffects = true; iWear.ItemID = 0x64BB; OldName = "mask of the dead"; NewName = "oniwaban hood"; }
						else if ( iWear.ItemID == 0x64BB ){ DoEffects = true; iWear.ItemID = 0x407; OldName = "oniwaban hood"; NewName = "wizard hood"; }
						else if ( iWear.ItemID == 0x407 ){ DoEffects = true; iWear.ItemID = 0x4D03; OldName = "wizard hood"; NewName = "evil hood"; }
						else if ( iWear.ItemID == 0x4D03 ){ DoEffects = true; iWear.ItemID = 0x4D04; OldName = "evil hood"; NewName = "evil cowl"; }
						else if ( iWear.ItemID == 0x4D04 ){ DoEffects = true; iWear.ItemID = Utility.RandomList( 0x141B, 0x141C ); OldName = "evil cowl"; NewName = "orcish mask"; }
						else if ( iWear.ItemID == 0x141B || iWear.ItemID == 0x141C ){ DoEffects = true; iWear.ItemID = 0x2B72; OldName = "orcish mask"; NewName = "gargoyle mask"; }
						else if ( iWear.ItemID == 0x2B72 || iWear.ItemID == 0x3169 ){ DoEffects = true; iWear.ItemID = 5915; OldName = "gargoyle mask"; NewName = "tricorne hat"; }
						else if ( iWear.ItemID == 5915 ){ DoEffects = true; iWear.ItemID = 5912; OldName = "tricorne hat"; NewName = "wizard hat"; }
						else if ( iWear.ItemID == 5912 ){ DoEffects = true; iWear.ItemID = 5908; OldName = "wizard hat"; NewName = "wide brim hat"; }
						else if ( iWear.ItemID == 5908 ){ DoEffects = true; iWear.ItemID = 5910; OldName = "wide brim hat"; NewName = "tall straw hat"; }
						else if ( iWear.ItemID == 5910 ){ DoEffects = true; iWear.ItemID = 5911; OldName = "tall straw hat"; NewName = "straw hat"; }
						else if ( iWear.ItemID == 5911 ){ DoEffects = true; iWear.ItemID = 5916; OldName = "straw hat"; NewName = "jester hat"; }
						else if ( iWear.ItemID == 5916 ){ DoEffects = true; iWear.ItemID = 0x4C15; OldName = "jester hat"; NewName = "fool's hat"; }
						else if ( iWear.ItemID == 0x4C15 ){ DoEffects = true; iWear.ItemID = 5914; OldName = "fool's hat"; NewName = "feathered hat"; }
						else if ( iWear.ItemID == 5914 ){ DoEffects = true; iWear.ItemID = 0x1DB9; OldName = "feathered hat"; NewName = "cap"; }
						else if ( iWear.ItemID == 0x1DB9 || iWear.ItemID == 0x1DBA ){ DoEffects = true; iWear.ItemID = 0x2FBC; OldName = "cap"; NewName = "pirate cap"; }
						else if ( iWear.ItemID == 0x2FBC ){ DoEffects = true; iWear.ItemID = 0x5C11; OldName = "pirate cap"; NewName = "shinobi hood"; }
						else if ( iWear.ItemID == 0x5C11 ){ DoEffects = true; iWear.ItemID = 0x5C12; OldName = "shinobi hood"; NewName = "shinobi mask"; }
						else if ( iWear.ItemID == 0x5C12 ){ DoEffects = true; iWear.ItemID = 0x5C13; OldName = "shinobi mask"; NewName = "shinobi cowl"; }

						else if ( iWear.ItemID == 0x1537 || iWear.ItemID == 0x1538 ){ DoEffects = true; iWear.ItemID = 0x2651; OldName = "kilt"; NewName = "kilt"; }
						else if ( iWear.ItemID == 0x2651 ){ DoEffects = true; iWear.ItemID = 0x1537; OldName = "kilt"; NewName = "kilt"; }

						else if ( iWear.ItemID == 0x64BD || iWear.ItemID == 0x13d3 ){ DoEffects = true; iWear.ItemID = 0x264F; OldName = "oniwaban tunic"; NewName = "leather tunic"; }
						else if ( iWear.ItemID == 0x264F ){ DoEffects = true; iWear.ItemID = 0x2650; OldName = "leather tunic"; NewName = "leather tunic"; }
						else if ( iWear.ItemID == 0x2650 ){ DoEffects = true; iWear.ItemID = 0x13CC; OldName = "leather tunic"; NewName = "leather tunic"; }
						else if ( iWear.ItemID == 0x13CC ){ DoEffects = true; iWear.ItemID = 0x64BD; OldName = "leather tunic"; NewName = "oniwaban tunic"; }

						if (	iWear is LeatherCloak || 
								iWear is LeatherCap || 
								iWear is LeatherRobe || 
								iWear is LeatherSandals || 
								iWear is ShinobiRobe || 
								iWear is ShinobiMask || 
								iWear is ShinobiHood || 
								iWear is ShinobiCowl || 
								iWear is LeatherShoes || 
								iWear is HikingBoots || 
								iWear is LeatherBoots || 
								iWear is LeatherThighBoots || 
								iWear is LeatherSoftBoots )
						{
							NewName = "leather " + NewName;
							OldName = "leather " + OldName;
						}

						if ( DoEffects )
						{
							if ( iWear.Name == null ){ iWear.Name = NewName; }
							else if ( iWear.Name.Contains(OldName) ){ iWear.Name = iWear.Name.Replace(OldName, NewName); }
							else { iWear.Name = NewName; }

							from.PlaySound( 0x248 );
							from.SendMessage( "The scissors magical transform the clothing." );
						}
						else
						{
							from.SendMessage( "These scissors are not really meant to do that." );
						}
					}
					else
					{
						from.SendMessage( "These scissors are not really meant to do that." );
					}
				}
				else
				{
					from.SendMessage( "These scissors are not really meant to do that." );
				}
			}
		}

		public MagicScissors( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( ( int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			this.Delete();
		}
	}
}