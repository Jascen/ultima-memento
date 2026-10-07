using System;
using System.Collections.Generic;
using Server.Engines.Avatar;
using Server.Engines.Craft;
using Server.Mobiles;
using Server.Network;
using Server.SkillHandlers;

namespace Server.Items
{
	public enum ClothingQuality
	{
		Low,
		Regular,
		Exceptional
	}

	public interface IArcaneEquip
	{
		bool IsArcane{ get; }
		int CurArcaneCharges{ get; set; }
		int MaxArcaneCharges{ get; set; }
	}

	public abstract class BaseClothing : Item, IDyable, IScissorable, ICraftable, IWearableDurability
	{
		public virtual bool CanFortify{ get{ return true; } }

		private int m_MaxHitPoints;
		private int m_HitPoints;
		private ClothingQuality m_Quality;
		private int m_StrReq = -1;

		private AosAttributes m_AosAttributes;
		private AosArmorAttributes m_AosClothingAttributes;
		private AosSkillBonuses m_AosSkillBonuses;
		private AosElementAttributes m_AosResistances;

		[CommandProperty( AccessLevel.GameMaster )]
		public override Density Density { get { return CraftResources.GetDensity( this ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxHitPoints
		{
			get{ return m_MaxHitPoints; }
			set{ m_MaxHitPoints = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitPoints
		{
			get 
			{
				return m_HitPoints;
			}
			set 
			{
				if ( value != m_HitPoints && MaxHitPoints > 0 )
				{
					m_HitPoints = value;

					if ( m_HitPoints < 0 )
						Delete();
					else if ( m_HitPoints > MaxHitPoints )
						m_HitPoints = MaxHitPoints;

					InvalidateProperties();
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int StrRequirement
		{
			get{ return ( m_StrReq == -1 ? (Core.AOS ? AosStrReq : OldStrReq) : m_StrReq ); }
			set{ m_StrReq = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public ClothingQuality Quality
		{
			get{ return m_Quality; }
			set{ m_Quality = value; InvalidateProperties(); }
		}

		public override void ResourceChanged( CraftResource resource )
		{
			if ( !ResourceCanChange() )
				return;

			ResourceMods.Modify( this, true );
			m_Resource = resource;
			Hue = CraftResources.GetHue(m_Resource);
			ResourceMods.Modify( this, false );

			InvalidateProperties();

			if ( Parent is Mobile )
				((Mobile)Parent).UpdateResistances();
		}

		public override void SubResourceChanged( CraftResource resource )
		{
			if ( resource != CraftResource.None )
			{
				Hue = CraftResources.GetHue( resource );
				SubResource = resource;
				SubName = CraftResources.GetName( resource );
			}
		}

		public override void MagicSpellChanged( MagicSpell spell )
		{
			SpellItems.ChangeMagicSpell( spell, this, false );
		}

		public override void CastEnchantment( Mobile from )
		{
			Server.Items.SpellItems.CastEnchantment( from, this );
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosAttributes Attributes
		{
			get{ return m_AosAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosArmorAttributes ClothingAttributes
		{
			get{ return m_AosClothingAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosSkillBonuses SkillBonuses
		{
			get{ return m_AosSkillBonuses; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosElementAttributes Resistances
		{
			get{ return m_AosResistances; }
			set{}
		}

		private int fabricBonus( int proc )
		{
			if ( CraftResources.GetType( Resource ) == CraftResourceType.Fabric )
				return proc;

			return (int)(proc/3);
		}

		private int GetLevelableArtiResistBonus()
		{
			// Legendary Artifacts get a flat resist bonus
			return this is ILevelable ? 2 : 0;
		}

		public virtual int BasePhysicalResistance{ get{ return 0; } }
		public virtual int BaseFireResistance{ get{ return 0; } }
		public virtual int BaseColdResistance{ get{ return 0; } }
		public virtual int BasePoisonResistance{ get{ return 0; } }
		public virtual int BaseEnergyResistance{ get{ return 0; } }

		public override int PhysicalResistance{ get{ return BasePhysicalResistance + GetLevelableArtiResistBonus() + fabricBonus( GetResourceAttrs().ArmorPhysicalResist ) + m_AosResistances.Physical; } }
		public override int FireResistance{ get{ return BaseFireResistance + GetLevelableArtiResistBonus() + fabricBonus( GetResourceAttrs().ArmorFireResist ) + m_AosResistances.Fire; } }
		public override int ColdResistance{ get{ return BaseColdResistance + GetLevelableArtiResistBonus() + fabricBonus( GetResourceAttrs().ArmorColdResist ) + m_AosResistances.Cold; } }
		public override int PoisonResistance{ get{ return BasePoisonResistance + GetLevelableArtiResistBonus() + fabricBonus( GetResourceAttrs().ArmorPoisonResist ) + m_AosResistances.Poison; } }
		public override int EnergyResistance{ get{ return BaseEnergyResistance + GetLevelableArtiResistBonus() + fabricBonus( GetResourceAttrs().ArmorEnergyResist ) + m_AosResistances.Energy; } }

		public virtual int ArtifactRarity{ get{ return 0; } }

		public virtual int InitMinHits{ get{ return 20; } }
		public virtual int InitMaxHits{ get{ return 30; } }

		public virtual int BaseStrBonus{ get{ return 0; } }
		public virtual int BaseDexBonus{ get{ return 0; } }
		public virtual int BaseIntBonus{ get{ return 0; } }

		public virtual Race RequiredRace { get { return null; } }

		public override CraftResource DefaultResource{ get{ return CraftResource.Fabric; } }

		public CraftAttributeInfo GetResourceAttrs()
		{
			CraftResourceInfo info = CraftResources.GetInfo( m_Resource );

			if ( info == null )
				return CraftAttributeInfo.Blank;

			return info.AttributeInfo;
		}

		public override bool CanEquip( Mobile from )
		{
			if( from.AccessLevel < AccessLevel.GameMaster )
			{
				if ( this is IAvatarOnlyItem )
				{
					var avatarOnlyItem = this as IAvatarOnlyItem;
					var player = from as PlayerMobile;
					if ( player == null || !player.Avatar.Active )
					{
						from.SendMessage("Only an Avatar may use this item.");
						return false;
					}

					if ( avatarOnlyItem.PlayerOwner != player )
					{
						from.SendMessage("This item belongs to another.");
						return false;
					}
				}

				if( RequiredRace != null && from.Race != RequiredRace )
				{
					if( RequiredRace == Race.Elf )
						from.SendLocalizedMessage( 1072203 ); // Only Elves may use this.
					else
						from.SendMessage( "Only {0} may use this.", RequiredRace.PluralName );

					return false;
				}
				else if( !AllowMaleWearer && !from.Female )
				{
					if( AllowFemaleWearer )
						from.SendLocalizedMessage( 1010388 ); // Only females can wear this.
					else
						from.SendMessage( "You may not wear this." );

					return false;
				}
				else if( !AllowFemaleWearer && from.Female )
				{
					if( AllowMaleWearer )
						from.SendLocalizedMessage( 1063343 ); // Only males can wear this.
					else
						from.SendMessage( "You may not wear this." );

					return false;
				}
				else
				{
					int strBonus = ComputeStatBonus( StatType.Str );
					int strReq = ComputeStatReq( StatType.Str );

					if( from.Str < strReq || (from.Str + strBonus) < 1 )
					{
						from.SendLocalizedMessage( 500213 ); // You are not strong enough to equip that.
						return false;
					}
				}
			}

			return base.CanEquip( from );
		}

		public virtual int AosStrReq{ get{ return 10; } }
		public virtual int OldStrReq{ get{ return 0; } }

		public virtual bool AllowMaleWearer{ get{ return true; } }
		public virtual bool AllowFemaleWearer{ get{ return true; } }
		public virtual bool CanBeBlessed{ get{ return true; } }

		public int ComputeStatReq( StatType type )
		{
			int v;

			//if ( type == StatType.Str )
				v = StrRequirement;

			return AOS.Scale( v, 100 - GetLowerStatReq() );
		}

		public int ComputeStatBonus( StatType type )
		{
			if ( type == StatType.Str )
				return BaseStrBonus + Attributes.BonusStr;
			else if ( type == StatType.Dex )
				return BaseDexBonus + Attributes.BonusDex;
			else
				return BaseIntBonus + Attributes.BonusInt;
		}

		public virtual void AddStatBonuses( Mobile parent )
		{
			if ( parent == null )
				return;

			int strBonus = ComputeStatBonus( StatType.Str );
			int dexBonus = ComputeStatBonus( StatType.Dex );
			int intBonus = ComputeStatBonus( StatType.Int );

			if ( strBonus == 0 && dexBonus == 0 && intBonus == 0 )
				return;

			string modName = this.Serial.ToString();

			if ( strBonus != 0 )
				parent.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

			if ( dexBonus != 0 )
				parent.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

			if ( intBonus != 0 )
				parent.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
		}

		public static void ValidateMobile( Mobile m )
		{
			for ( int i = m.Items.Count - 1; i >= 0; --i )
			{
				if ( i >= m.Items.Count )
					continue;

				Item item = m.Items[i];

				if ( item is BaseClothing )
				{
					BaseClothing clothing = (BaseClothing)item;

					if( clothing.RequiredRace != null && m.Race != clothing.RequiredRace )
					{
						if( clothing.RequiredRace == Race.Elf )
							m.SendLocalizedMessage( 1072203 ); // Only Elves may use this.
						else
							m.SendMessage( "Only {0} may use this.", clothing.RequiredRace.PluralName );

						m.AddToBackpack( clothing );
					}
					else if ( !clothing.AllowMaleWearer && !m.Female && m.AccessLevel < AccessLevel.GameMaster )
					{
						if ( clothing.AllowFemaleWearer )
							m.SendLocalizedMessage( 1010388 ); // Only females can wear this.
						else
							m.SendMessage( "You may not wear this." );

						m.AddToBackpack( clothing );
					}
					else if ( !clothing.AllowFemaleWearer && m.Female && m.AccessLevel < AccessLevel.GameMaster )
					{
						if ( clothing.AllowMaleWearer )
							m.SendLocalizedMessage( 1063343 ); // Only males can wear this.
						else
							m.SendMessage( "You may not wear this." );

						m.AddToBackpack( clothing );
					}
				}
			}
		}

		public int GetLowerStatReq()
		{
			if ( !Core.AOS )
				return 0;

			return m_AosClothingAttributes.LowerStatReq;
		}

		public override void DefaultMainHue( Item item )
		{
			ResourceMods.DefaultItemHue( item );
		}

		public override void OnAdded( object parent )
		{
			DefaultMainHue( this );
			Mobile mob = parent as Mobile;

			if ( mob != null )
			{
				if ( Core.AOS )
					m_AosSkillBonuses.AddTo( mob );

				AddStatBonuses( mob );
				mob.CheckStatTimers();
			}

			base.OnAdded( parent );
		}

		public override void OnRemoved( object parent )
		{
			Mobile mob = parent as Mobile;

			if ( mob != null )
			{
				if ( Core.AOS )
					m_AosSkillBonuses.Remove();

				string modName = this.Serial.ToString();

				mob.RemoveStatMod( modName + "Str" );
				mob.RemoveStatMod( modName + "Dex" );
				mob.RemoveStatMod( modName + "Int" );

				mob.CheckStatTimers();
			}

			base.OnRemoved( parent );
		}

		public virtual int OnHit( BaseWeapon weapon, int damageTaken )
		{
			int Absorbed = Utility.RandomMinMax( 1, 4 );

			damageTaken -= Absorbed;

			if ( damageTaken < 0 ) 
				damageTaken = 0;

			if ( Density == Density.None ) return damageTaken; //
	
			/*
				100% - Weak
				50% - Regular
				33% - Great
				20% - Greater
				14% - Superior
				9% - Ultimate
			*/
			double baseValue = Math.Pow(1.5, (int)Density);
			double testValue = 1f / (int)baseValue;
			if ( Utility.RandomDouble() < testValue )
			{
				if ( this is ILevelable )
				{
					LevelItemManager.RepairItems( Parent as Mobile );
				}
				else if ( !ArmsLore.AvoidDurabilityHit( Parent as Mobile ) )
				{
					int wear;

					if ( weapon.Type == WeaponType.Bashing )
						wear = Absorbed / 2;
					else
						wear = Utility.Random( 2 );

					if ( wear > 0 && m_MaxHitPoints > 0 )
					{
						if ( m_AosClothingAttributes.SelfRepair > Utility.Random( 10 ) )
							HitPoints += Utility.RandomMinMax( 1, (int)Density );

						if ( m_HitPoints >= wear )
						{
							HitPoints -= wear;
							wear = 0;
						}
						else
						{
							wear -= HitPoints;
							HitPoints = 0;
						}

						if ( wear > 0 )
						{
							if ( m_MaxHitPoints > wear )
							{
								MaxHitPoints -= wear;

								if ( Parent is Mobile )
									((Mobile)Parent).LocalOverheadMessage( MessageType.Regular, 0x3B2, 1061121 ); // Your equipment is severely damaged.
							}
						}
					}

					if ( MaxHitPoints < 1 )
						Delete();
				}
			}

			return damageTaken;
		}

		public BaseClothing( int itemID, Layer layer ) : this( itemID, layer, 0 )
		{
		}

		public BaseClothing( int itemID, Layer layer, int hue ) : base( itemID )
		{
			Layer = layer;
			Hue = hue;

			m_Quality = ClothingQuality.Regular;

			m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax( InitMinHits, InitMaxHits );

			m_AosAttributes = new AosAttributes( this );
			m_AosClothingAttributes = new AosArmorAttributes( this );
			m_AosSkillBonuses = new AosSkillBonuses( this );
			m_AosResistances = new AosElementAttributes( this );
		}

		public override void OnAfterDuped( Item newItem )
		{
			BaseClothing clothing = newItem as BaseClothing;

			if ( clothing == null )
				return;

			clothing.m_AosAttributes = new AosAttributes( newItem, m_AosAttributes );
			clothing.m_AosResistances = new AosElementAttributes( newItem, m_AosResistances );
			clothing.m_AosSkillBonuses = new AosSkillBonuses( newItem, m_AosSkillBonuses );
			clothing.m_AosClothingAttributes = new AosArmorAttributes( newItem, m_AosClothingAttributes );
		}

		protected void ResetAllAttributes()
		{
			m_AosAttributes = new AosAttributes(this);
			m_AosResistances = new AosElementAttributes(this);
			m_AosSkillBonuses = new AosSkillBonuses(this);
			m_AosClothingAttributes = new AosArmorAttributes(this);
		}

		public BaseClothing( Serial serial ) : base( serial )
		{
		}

		public override bool AllowEquipedCast( Mobile from )
		{
			if ( base.AllowEquipedCast( from ) )
				return true;

			return ( m_AosAttributes.SpellChanneling != 0 );
		}

		public void UnscaleDurability()
		{
			int scale = 100 + m_AosClothingAttributes.DurabilityBonus;

			m_HitPoints = ( ( m_HitPoints * 100 ) + ( scale - 1 ) ) / scale;
			m_MaxHitPoints = ( ( m_MaxHitPoints * 100 ) + ( scale - 1 ) ) / scale;

			InvalidateProperties();
		}

		public void ScaleDurability()
		{
			int scale = 100 + m_AosClothingAttributes.DurabilityBonus;

			m_HitPoints = ( ( m_HitPoints * scale ) + 99 ) / 100;
			m_MaxHitPoints = ( ( m_MaxHitPoints * scale ) + 99 ) / 100;

			InvalidateProperties();
		}

		public override bool CheckPropertyConfliction( Mobile m )
		{
			if ( base.CheckPropertyConfliction( m ) )
				return true;

			return false;
		}

		public virtual int GetLuckBonus()
		{
			CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );

			if ( resInfo == null )
				return 0;

			CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

			if ( attrInfo == null )
				return 0;

			return attrInfo.ArmorLuck;
		}

		private string GetNameString()
		{
			string name = this.Name;

			if ( name == null )
				name = String.Format( "#{0}", LabelNumber );

			return name;
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( CraftResources.GetClilocLowerCaseName( m_Resource ) > 0 && m_SubResource == CraftResource.None )
				list.Add( 1053099, "#{0}\t{1}", CraftResources.GetClilocLowerCaseName( m_Resource ), GetNameString() );
			else if ( Name == null )
				list.Add( LabelNumber );
			else
				list.Add( Name );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_BuiltBy != null )
				list.Add( 1050043, m_BuiltBy.Name ); // crafted by ~1_NAME~

			if ( m_Quality == ClothingQuality.Exceptional )
				list.Add( 1060636 ); // exceptional

			if( RequiredRace == Race.Elf )
				list.Add( 1075086 ); // Elves Only

			if ( m_AosSkillBonuses != null )
				m_AosSkillBonuses.GetProperties( list );

			int prop;

			if ( (prop = ArtifactRarity) > 0 )
				list.Add( 1061078, prop.ToString() ); // artifact rarity ~1_val~

			if ( (prop = m_AosAttributes.WeaponDamage) != 0 )
				list.Add( 1060401, prop.ToString() ); // damage increase ~1_val~%

			if ( (prop = m_AosAttributes.DefendChance) != 0 )
				list.Add( 1060408, prop.ToString() ); // defense chance increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusDex) != 0 )
				list.Add( 1060409, prop.ToString() ); // dexterity bonus ~1_val~

			if ( (prop = m_AosAttributes.EnhancePotions) != 0 )
				list.Add( 1060411, prop.ToString() ); // enhance potions ~1_val~%

			if ( (prop = m_AosAttributes.CastRecovery) != 0 )
				list.Add( 1060412, prop.ToString() ); // faster cast recovery ~1_val~

			if ( (prop = m_AosAttributes.CastSpeed) != 0 )
				list.Add( 1060413, prop.ToString() ); // faster casting ~1_val~

			if ( (prop = m_AosAttributes.AttackChance) != 0 )
				list.Add( 1060415, prop.ToString() ); // hit chance increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusHits) != 0 )
				list.Add( 1060431, prop.ToString() ); // hit point increase ~1_val~

			if ( (prop = m_AosAttributes.BonusInt) != 0 )
				list.Add( 1060432, prop.ToString() ); // intelligence bonus ~1_val~

			if ( (prop = m_AosAttributes.LowerManaCost) != 0 && MyServerSettings.LowerMana() > 0 )
			{
				if ( prop > MyServerSettings.LowerMana() ){ prop = MyServerSettings.LowerMana(); }
				list.Add( 1060433, prop.ToString() ); // lower mana cost ~1_val~%
			}

			if ( (prop = m_AosAttributes.LowerRegCost) != 0 && MyServerSettings.LowerReg() > 0 )
			{
				if ( prop > MyServerSettings.LowerReg() ){ prop = MyServerSettings.LowerReg(); }
				list.Add( 1060434, prop.ToString() ); // lower reagent cost ~1_val~%
			}

			if ( (prop = m_AosClothingAttributes.LowerStatReq) != 0 )
				list.Add( 1060435, prop.ToString() ); // lower requirements ~1_val~%

			if ( (prop = (GetLuckBonus() + m_AosAttributes.Luck)) != 0 )
				list.Add( 1060436, prop.ToString() ); // luck ~1_val~

			if ( (prop = m_AosClothingAttributes.MageArmor) != 0 )
				list.Add( 1060437 ); // mage armor

			if ( (prop = m_AosAttributes.BonusMana) != 0 )
				list.Add( 1060439, prop.ToString() ); // mana increase ~1_val~

			if ( (prop = m_AosAttributes.RegenMana) != 0 )
				list.Add( 1060440, prop.ToString() ); // mana regeneration ~1_val~

			if ( (prop = m_AosAttributes.NightSight) != 0 )
				list.Add( 1060441 ); // night sight

			if ( (prop = m_AosAttributes.ReflectPhysical) != 0 )
				list.Add( 1060442, prop.ToString() ); // reflect physical damage ~1_val~%

			if ( (prop = m_AosAttributes.RegenStam) != 0 )
				list.Add( 1060443, prop.ToString() ); // stamina regeneration ~1_val~

			if ( (prop = m_AosAttributes.RegenHits) != 0 )
				list.Add( 1060444, prop.ToString() ); // hit point regeneration ~1_val~

			if ( (prop = m_AosClothingAttributes.SelfRepair) != 0 )
				list.Add( 1060450, prop.ToString() ); // self repair ~1_val~0%

			if ( (prop = m_AosAttributes.SpellChanneling) != 0 )
				list.Add( 1060482 ); // spell channeling

			if ( (prop = m_AosAttributes.SpellDamage) != 0 )
				list.Add( 1060483, prop.ToString() ); // spell damage increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusStam) != 0 )
				list.Add( 1060484, prop.ToString() ); // stamina increase ~1_val~

			if ( (prop = m_AosAttributes.BonusStr) != 0 )
				list.Add( 1060485, prop.ToString() ); // strength bonus ~1_val~

			if ( (prop = m_AosAttributes.WeaponSpeed) != 0 )
				list.Add( 1060486, prop.ToString() ); // swing speed increase ~1_val~%

			base.AddResistanceProperties( list );

			if ( (prop = m_AosClothingAttributes.DurabilityBonus) > 0 )
				list.Add( 1060410, prop.ToString() ); // durability ~1_val~%

			list.Add( 1061182, EquipLayerName( Layer ) );

			if ( Density != Density.None )
				list.Add( 1061182 + (int)Density );

			if ( (prop = ComputeStatReq( StatType.Str )) > 0 )
				list.Add( 1061170, prop.ToString() ); // strength requirement ~1_val~

			if ( Density != Density.None ) // hidden when None
				list.Add( 1060639, "{0}\t{1}", m_HitPoints, m_MaxHitPoints ); // durability ~1_val~ / ~2_val~
		}

		public override void OnSingleClick( Mobile from )
		{
			List<EquipInfoAttribute> attrs = new List<EquipInfoAttribute>();

			AddEquipInfoAttributes( from, attrs );

			int number;

			if ( Name == null )
			{
				number = LabelNumber;
			}
			else
			{
				this.LabelTo( from, Name );
				number = 1041000;
			}

			if ( attrs.Count == 0 && BuiltBy == null && Name != null )
				return;

			EquipmentInfo eqInfo = new EquipmentInfo( number, m_BuiltBy, false, attrs.ToArray() );

			from.Send( new DisplayEquipmentInfo( this, eqInfo ) );
		}

		public virtual void AddEquipInfoAttributes( Mobile from, List<EquipInfoAttribute> attrs )
		{
			if ( DisplayLootType )
			{
				if ( LootType == LootType.Blessed )
					attrs.Add( new EquipInfoAttribute( 1038021 ) ); // blessed
				else if ( LootType == LootType.Cursed )
					attrs.Add( new EquipInfoAttribute( 1049643 ) ); // cursed
			}

			if ( m_Quality == ClothingQuality.Exceptional )
				attrs.Add( new EquipInfoAttribute( 1018305 - (int)m_Quality ) );
		}

		#region Serialization
		private static void SetSaveFlag( ref SaveFlag flags, SaveFlag toSet, bool setIf )
		{
			if ( setIf )
				flags |= toSet;
		}

		private static bool GetSaveFlag( SaveFlag flags, SaveFlag toGet )
		{
			return ( (flags & toGet) != 0 );
		}

		[Flags]
		private enum SaveFlag
		{
			None				= 0x00000000,
			Resource			= 0x00000001,
			Attributes			= 0x00000002,
			ClothingAttributes	= 0x00000004,
			SkillBonuses		= 0x00000008,
			Resistances			= 0x00000010,
			MaxHitPoints		= 0x00000020,
			HitPoints			= 0x00000040,
			NotUsedAnymore		= 0x00000080,
			NoLonger_Used		= 0x00000100,
			Quality				= 0x00000200,
			StrReq				= 0x00000400
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 3 ); // version

			SaveFlag flags = SaveFlag.None;

			SetSaveFlag( ref flags, SaveFlag.Resource,			m_Resource != DefaultResource );
			SetSaveFlag( ref flags, SaveFlag.Attributes,		!m_AosAttributes.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.ClothingAttributes,!m_AosClothingAttributes.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.SkillBonuses,		!m_AosSkillBonuses.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.Resistances,		!m_AosResistances.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.MaxHitPoints,		m_MaxHitPoints != 0 );
			SetSaveFlag( ref flags, SaveFlag.HitPoints,			m_HitPoints != 0 );
			SetSaveFlag( ref flags, SaveFlag.NotUsedAnymore,	m_Built != false );
			SetSaveFlag( ref flags, SaveFlag.NoLonger_Used,		m_BuiltBy != null );
			SetSaveFlag( ref flags, SaveFlag.Quality,			m_Quality != ClothingQuality.Regular );
			SetSaveFlag( ref flags, SaveFlag.StrReq,			m_StrReq != -1 );

			writer.WriteEncodedInt( (int) flags );

			if ( GetSaveFlag( flags, SaveFlag.Resource ) )
				writer.WriteEncodedInt( (int) m_Resource );

			if ( GetSaveFlag( flags, SaveFlag.Attributes ) )
				m_AosAttributes.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.ClothingAttributes ) )
				m_AosClothingAttributes.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
				m_AosSkillBonuses.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.Resistances ) )
				m_AosResistances.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.MaxHitPoints ) )
				writer.WriteEncodedInt( (int) m_MaxHitPoints );

			if ( GetSaveFlag( flags, SaveFlag.HitPoints ) )
				writer.WriteEncodedInt( (int) m_HitPoints );

			if ( GetSaveFlag( flags, SaveFlag.NoLonger_Used ) )
				writer.Write( (Mobile) m_BuiltBy );

			if ( GetSaveFlag( flags, SaveFlag.Quality ) )
				writer.WriteEncodedInt( (int) m_Quality );

			if ( GetSaveFlag( flags, SaveFlag.StrReq ) )
				writer.WriteEncodedInt( (int) m_StrReq );
		}

		public override void Deserialize( GenericReader reader )
		{
			int nul;
			Mobile nol = null;

			base.Deserialize( reader );

			int version = reader.ReadInt();

			SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

			if ( GetSaveFlag( flags, SaveFlag.Resource ) )
				nul = reader.ReadEncodedInt();

			if ( GetSaveFlag( flags, SaveFlag.Attributes ) )
				m_AosAttributes = new AosAttributes( this, reader );
			else
				m_AosAttributes = new AosAttributes( this );

			if ( GetSaveFlag( flags, SaveFlag.ClothingAttributes ) )
				m_AosClothingAttributes = new AosArmorAttributes( this, reader );
			else
				m_AosClothingAttributes = new AosArmorAttributes( this );

			if ( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
				m_AosSkillBonuses = new AosSkillBonuses( this, reader );
			else
				m_AosSkillBonuses = new AosSkillBonuses( this );

			if ( GetSaveFlag( flags, SaveFlag.Resistances ) )
				m_AosResistances = new AosElementAttributes( this, reader );
			else
				m_AosResistances = new AosElementAttributes( this );

			if ( GetSaveFlag( flags, SaveFlag.MaxHitPoints ) )
				m_MaxHitPoints = reader.ReadEncodedInt();

			if ( GetSaveFlag( flags, SaveFlag.HitPoints ) )
				m_HitPoints = reader.ReadEncodedInt();

			if ( GetSaveFlag( flags, SaveFlag.NoLonger_Used ) && version < 10 )
				m_BuiltBy = reader.ReadMobile();
			else if ( GetSaveFlag( flags, SaveFlag.NoLonger_Used ) )
				nol = reader.ReadMobile();

			if ( GetSaveFlag( flags, SaveFlag.Quality ) )
				m_Quality = (ClothingQuality)reader.ReadEncodedInt();
			else
				m_Quality = ClothingQuality.Regular;

			if ( GetSaveFlag( flags, SaveFlag.StrReq ) )
				m_StrReq = reader.ReadEncodedInt();
			else
				m_StrReq = -1;

			if ( GetSaveFlag( flags, SaveFlag.NotUsedAnymore ) && version < 2 )
				m_Built = true;
			else if ( GetSaveFlag( flags, SaveFlag.NotUsedAnymore ) ){}

			if ( m_MaxHitPoints == 0 && m_HitPoints == 0 )
				m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax( InitMinHits, InitMaxHits );

			Mobile parent = Parent as Mobile;

			if ( parent != null )
			{
				m_AosSkillBonuses.AddTo( parent );
				AddStatBonuses( parent );
				parent.CheckStatTimers();
			}
			
			if (version < 3 && Resource == CraftResource.GildedSpec)
			{
				m_AosSkillBonuses.SetValues(3, SkillName.Alchemy, 0);
				m_AosSkillBonuses.SetValues(4, SkillName.Tracking, 5);
			}
		}
		#endregion

		public virtual bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;
			else if ( RootParent is Mobile && from != RootParent )
				return false;

			Hue = sender.DyedHue;

			return true;
		}

		public virtual bool Scissor( Mobile from, Scissors scissors )
		{
			bool extraCloth = false;

			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 502437 ); // Items you wish to cut must be in your backpack.
				return false;
			}

			CraftResource resource = Resource;
				if ( !IsStandardResource( SubResource ) )
				{
					resource = SubResource;
					if ( CraftResources.GetType( resource ) == CraftResourceType.Fabric )
						extraCloth = true;
				}

			string msg = Scissors.CutUp( from, this, resource, extraCloth );

			if ( msg != null )
			{
				from.SendMessage( msg );
				return false;
			}

			from.SendLocalizedMessage( 502440 ); // Scissors can not be used on that to produce anything.
			return false;
		}

		public void DistributeBonuses( int amount )
		{
			for ( int i = 0; i < amount; ++i )
			{
				switch ( Utility.Random( 5 ) )
				{
					case 0: ++m_AosResistances.Physical; break;
					case 1: ++m_AosResistances.Fire; break;
					case 2: ++m_AosResistances.Cold; break;
					case 3: ++m_AosResistances.Poison; break;
					case 4: ++m_AosResistances.Energy; break;
				}
			}

			InvalidateProperties();
		}

		#region ICraftable Members

		public virtual int OnCraft( int quality, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (ClothingQuality)quality;

			if ( DefaultResource != CraftResource.None )
			{
				Type resourceType = typeRes;

				if ( resourceType == null )
					resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

				Resource = CraftResources.GetFromType( resourceType );
			}

			if ( 0 < resHue )
				Hue = resHue;

			CraftContext context = craftSystem.GetContext( from );

			if ( context != null && context.DoNotColor )
				Hue = 0;

			return quality;
		}

		#endregion
	}
}