using System;
using Server.Engines.Craft;

namespace Server.Items
{
	public enum GemType
	{
		None,
		StarSapphire,
		Emerald,
		Sapphire,
		Ruby,
		Citrine,
		Amethyst,
		Tourmaline,
		Amber,
		Diamond,
		Pearl
	}

	public enum TrinketQuality
	{
		Low,
		Regular,
		Exceptional
	}

	public abstract class BaseTrinket : Item, ICraftable
	{
		public override Catalogs DefaultCatalog{ get{ return Catalogs.Jewelry; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.Iron; } }

		private int m_MaxHitPoints;
		private int m_HitPoints;
		private TrinketQuality m_Quality;

		private AosAttributes m_AosAttributes;
		private AosArmorAttributes m_AosArmorAttributes;
		private AosElementAttributes m_AosResistances;
		private AosSkillBonuses m_AosSkillBonuses;

		public override void MagicSpellChanged( MagicSpell spell )
		{
			SpellItems.ChangeMagicSpell( spell, this, false );
		}

		public override void CastEnchantment( Mobile from )
		{
			Server.Items.SpellItems.CastEnchantment( from, this );
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TrinketQuality Quality
		{
			get{ return m_Quality; }
			set{ UnscaleDurability(); m_Quality = value; ScaleDurability(); InvalidateProperties(); }
		}

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

		[CommandProperty( AccessLevel.Player )]
		public AosAttributes Attributes
		{
			get{ return m_AosAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosElementAttributes Resistances
		{
			get{ return m_AosResistances; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosSkillBonuses SkillBonuses
		{
			get{ return m_AosSkillBonuses; }
			set{}
		}

		public override void ResourceChanged( CraftResource resource )
		{
			if ( !ResourceCanChange() || Catalog != Catalogs.Jewelry )
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

		private GemType m_GemType;
		[CommandProperty(AccessLevel.Owner)]
		public GemType GemType
		{
			get
			{
				return m_GemType;
			}
			set
			{
				if ( m_GemType != value )
				{
					if ( Catalog != Catalogs.Jewelry )
						m_GemType = GemType.None;
					else
					{
						ResourceMods.Modify( this, true );
						m_GemType = value;
						ItemID = GraphicID = GemItemID();
						ResourceMods.Modify( this, false );

						if ( Parent is Mobile )
							((Mobile)Parent).UpdateResistances();
					}
					InvalidateProperties();
				}
			}
		}

		public virtual int BasePhysicalResistance{ get{ return 0; } }
		public virtual int BaseFireResistance{ get{ return 0; } }
		public virtual int BaseColdResistance{ get{ return 0; } }
		public virtual int BasePoisonResistance{ get{ return 0; } }
		public virtual int BaseEnergyResistance{ get{ return 0; } }

		public override int PhysicalResistance{ get{ return BasePhysicalResistance + GetLevelableArtiResistBonus() + (int)(GetResourceAttrs().ArmorPhysicalResist/3) + m_AosResistances.Physical; } }
		public override int FireResistance{ get{ return BaseFireResistance + GetLevelableArtiResistBonus() + (int)(GetResourceAttrs().ArmorFireResist/2) + m_AosResistances.Fire; } }
		public override int ColdResistance{ get{ return BaseColdResistance + GetLevelableArtiResistBonus() + (int)(GetResourceAttrs().ArmorColdResist/2) + m_AosResistances.Cold; } }
		public override int PoisonResistance{ get{ return BasePoisonResistance + GetLevelableArtiResistBonus() + (int)(GetResourceAttrs().ArmorPoisonResist/2) + m_AosResistances.Poison; } }
		public override int EnergyResistance{ get{ return BaseEnergyResistance + GetLevelableArtiResistBonus() + (int)(GetResourceAttrs().ArmorEnergyResist/2) + m_AosResistances.Energy; } }

		public virtual int BaseGemTypeNumber{ get{ return 0; } }

		public virtual int InitMinHits{ get{ return 0; } }
		public virtual int InitMaxHits{ get{ return 0; } }

		public CraftAttributeInfo GetResourceAttrs()
		{
			CraftResourceInfo info = CraftResources.GetInfo( m_Resource );

			if ( info == null )
				return CraftAttributeInfo.Blank;

			return info.AttributeInfo;
		}

		private int GetLevelableArtiResistBonus()
		{
			// Legendary Artifacts get a flat resist bonus
			return this is ILevelable ? 2 : 0;
		}

		public int GemItemID()
		{
			int id = 26413;

			switch ( Layer )
			{
				case Layer.Helm: id += 1; break;
				case Layer.Earrings: id += 2; break;
				case Layer.Neck: id += 3; break;
				case Layer.Ring: id += 4; break;
			}

			return ( id += (int)GemType*5 );
		}

		public override void OnAfterDuped( Item newItem )
		{
			BaseTrinket jewel = newItem as BaseTrinket;

			if ( jewel == null )
				return;

			jewel.m_AosAttributes = new AosAttributes( newItem, m_AosAttributes );
			jewel.m_AosResistances = new AosElementAttributes( newItem, m_AosResistances );
			jewel.m_AosSkillBonuses = new AosSkillBonuses( newItem, m_AosSkillBonuses );
			jewel.m_AosArmorAttributes = new AosArmorAttributes( newItem, m_AosArmorAttributes );
		}

		protected void ResetAllAttributes()
		{
			m_AosAttributes = new AosAttributes(this);
			m_AosResistances = new AosElementAttributes(this);
			m_AosSkillBonuses = new AosSkillBonuses(this);
			m_AosArmorAttributes = new AosArmorAttributes(this);
		}

		public virtual int ArtifactRarity{ get{ return 0; } }

		public BaseTrinket( int itemID ) : this( itemID, Layer.Trinket )
		{
		}

		public BaseTrinket( int itemID, Layer layer ) : base( itemID )
		{
			m_AosAttributes = new AosAttributes( this );
			m_AosResistances = new AosElementAttributes( this );
			m_AosSkillBonuses = new AosSkillBonuses( this );
			m_AosArmorAttributes = new AosArmorAttributes( this );
			m_GemType = GemType.None;

			Layer = layer;

			m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax( InitMinHits, InitMaxHits );
		}

		public override void OnAdded( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile from = (Mobile)parent;

				m_AosSkillBonuses.AddTo( from );

				int strBonus = m_AosAttributes.BonusStr;
				int dexBonus = m_AosAttributes.BonusDex;
				int intBonus = m_AosAttributes.BonusInt;

				if ( strBonus != 0 || dexBonus != 0 || intBonus != 0 )
				{
					string modName = this.Serial.ToString();

					if ( strBonus != 0 )
						from.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

					if ( dexBonus != 0 )
						from.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

					if ( intBonus != 0 )
						from.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
				}

				from.CheckStatTimers();
			}

			base.OnAdded( parent );
		}

		public override void OnRemoved( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile from = (Mobile)parent;

				m_AosSkillBonuses.Remove();

				string modName = this.Serial.ToString();

				from.RemoveStatMod( modName + "Str" );
				from.RemoveStatMod( modName + "Dex" );
				from.RemoveStatMod( modName + "Int" );

				from.CheckStatTimers();
			}
		}

		public BaseTrinket( Serial serial ) : base( serial )
		{
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
			if ( m_Quality == TrinketQuality.Exceptional )
			{
				if ( CraftResources.GetClilocLowerCaseName( m_Resource ) > 0 && m_SubResource == CraftResource.None )
					list.Add( 1053100, "#{0}\t{1}", CraftResources.GetClilocLowerCaseName( m_Resource ), GetNameString() ); // exceptional ~1_oretype~ ~2_armortype~
				else
					list.Add( 1050040, GetNameString() ); // exceptional ~1_ITEMNAME~
			}
			else
			{
				if ( CraftResources.GetClilocLowerCaseName( m_Resource ) > 0 && m_SubResource == CraftResource.None )
					list.Add( 1053099, "#{0}\t{1}", CraftResources.GetClilocLowerCaseName( m_Resource ), GetNameString() ); // ~1_oretype~ ~2_armortype~
				else if ( Name == null )
					list.Add( LabelNumber );
				else
					list.Add( Name );
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( GemType != GemType.None )
				list.Add( 1032058+((int)GemType) );

			if ( m_BuiltBy != null )
				list.Add( 1050043, m_BuiltBy.Name ); // crafted by ~1_NAME~

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

			if ( (prop = (GetLuckBonus() + m_AosAttributes.Luck)) != 0 )
				list.Add( 1060436, prop.ToString() ); // luck ~1_val~

			if ( (prop = m_AosAttributes.BonusMana) != 0 )
				list.Add( 1060439, prop.ToString() ); // mana increase ~1_val~

			if ( (prop = m_AosAttributes.RegenMana) != 0 )
				list.Add( 1060440, prop.ToString() ); // mana regeneration ~1_val~

			if ( (prop = m_AosAttributes.NightSight) != 0 && !(this is TrinketCandle) && !(this is TrinketLantern) && !(this is TrinketTorch) )
				list.Add( 1060441 ); // night sight

			if ( (prop = m_AosAttributes.ReflectPhysical) != 0 )
				list.Add( 1060442, prop.ToString() ); // reflect physical damage ~1_val~%

			if ( (prop = m_AosAttributes.RegenStam) != 0 )
				list.Add( 1060443, prop.ToString() ); // stamina regeneration ~1_val~

			if ( (prop = m_AosAttributes.RegenHits) != 0 )
				list.Add( 1060444, prop.ToString() ); // hit point regeneration ~1_val~

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

			list.Add( 1061182, EquipLayerName( Layer ) );

			if ( m_HitPoints >= 0 && m_MaxHitPoints > 0 )
				list.Add( 1060639, "{0}\t{1}", m_HitPoints, m_MaxHitPoints ); // durability ~1_val~ / ~2_val~
		}

		public void UnscaleDurability()
		{
			int scale = 100 + m_AosArmorAttributes.DurabilityBonus;

			m_HitPoints = ( ( m_HitPoints * 100 ) + ( scale - 1 ) ) / scale;
			m_MaxHitPoints = ( ( m_MaxHitPoints * 100 ) + ( scale - 1 ) ) / scale;

			InvalidateProperties();
		}

		public void ScaleDurability()
		{
			int scale = 100 + m_AosArmorAttributes.DurabilityBonus;

			m_HitPoints = ( ( m_HitPoints * scale ) + 99 ) / 100;
			m_MaxHitPoints = ( ( m_MaxHitPoints * scale ) + 99 ) / 100;

			InvalidateProperties();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)5 ); // version

			m_AosArmorAttributes.Serialize( writer );
			writer.WriteEncodedInt( (int) m_Quality );
			writer.WriteEncodedInt( (int) m_MaxHitPoints );
			writer.WriteEncodedInt( (int) m_HitPoints );

			writer.WriteEncodedInt( (int) m_GemType );

			m_AosAttributes.Serialize( writer );
			m_AosResistances.Serialize( writer );
			m_AosSkillBonuses.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 5:
				case 4:
				{
					if (version < 5)
					{
						 m_AosArmorAttributes = new AosArmorAttributes( this );
						 Quality = TrinketQuality.Regular;
					}
					else
					{
						 m_AosArmorAttributes = new AosArmorAttributes( this, reader );
						 Quality = (TrinketQuality)reader.ReadEncodedInt();
					}
					goto case 3;
				}
				case 3:
				{
					m_MaxHitPoints = reader.ReadEncodedInt();
					m_HitPoints = reader.ReadEncodedInt();

					goto case 2;
				}
				case 2:
				{
					if ( version < 4 )
						m_Resource = (CraftResource)reader.ReadEncodedInt();

					m_GemType = (GemType)reader.ReadEncodedInt();

					goto case 1;
				}
				case 1:
				{
					m_AosAttributes = new AosAttributes( this, reader );
					m_AosResistances = new AosElementAttributes( this, reader );
					m_AosSkillBonuses = new AosSkillBonuses( this, reader );

					if ( Parent is Mobile )
						m_AosSkillBonuses.AddTo( (Mobile)Parent );

					int strBonus = m_AosAttributes.BonusStr;
					int dexBonus = m_AosAttributes.BonusDex;
					int intBonus = m_AosAttributes.BonusInt;

					if ( Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0) )
					{
						Mobile m = (Mobile)Parent;

						string modName = Serial.ToString();

						if ( strBonus != 0 )
							m.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

						if ( dexBonus != 0 )
							m.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

						if ( intBonus != 0 )
							m.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
					}

					if ( Parent is Mobile )
						((Mobile)Parent).CheckStatTimers();

					break;
				}
				case 0:
				{
					m_AosAttributes = new AosAttributes( this );
					m_AosResistances = new AosElementAttributes( this );
					m_AosSkillBonuses = new AosSkillBonuses( this );

					break;
				}
			}
		}
		#region ICraftable Members

		public static void RandomGem( BaseTrinket ring )
		{
			switch ( Utility.Random( 13 ) ) 
			{
				case 1: ring.GemType = GemType.StarSapphire; break;
				case 2: ring.GemType = GemType.Emerald; break;
				case 3: ring.GemType = GemType.Sapphire; break;
				case 4: ring.GemType = GemType.Ruby; break;
				case 5: ring.GemType = GemType.Citrine; break;
				case 6: ring.GemType = GemType.Amethyst; break;
				case 7: ring.GemType = GemType.Tourmaline; break;
				case 8: ring.GemType = GemType.Amber; break;
				case 9: ring.GemType = GemType.Diamond; break;
			}

			if ( Utility.Random(100) == 0 )
				ring.GemType = GemType.Pearl;
		}

		public int OnCraft( int quality, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (TrinketQuality)quality;

			Type resourceType = typeRes;

			if ( resourceType == null )
				resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

			Resource = CraftResources.GetFromType( resourceType );

			CraftContext context = craftSystem.GetContext( from );

			if ( context != null && context.DoNotColor )
				Hue = 0;

			if ( tool is BaseRunicTool )
				Resource = ((BaseRunicTool)tool).Resource;

			GemType = GetGemType(craftItem);

			return 1;
		}

		public static GemType GetGemType( CraftItem craftItem )
		{
			if ( 1 < craftItem.Resources.Count )
			{
				Type resourceType = craftItem.Resources.GetAt( 1 ).ItemType;
			
				if ( resourceType == typeof( StarSapphire ) )
					return GemType.StarSapphire;
				if ( resourceType == typeof( Emerald ) )
					return GemType.Emerald;
				if ( resourceType == typeof( Sapphire ) )
					return GemType.Sapphire;
				if ( resourceType == typeof( Ruby ) )
					return GemType.Ruby;
				if ( resourceType == typeof( Citrine ) )
					return GemType.Citrine;
				if ( resourceType == typeof( Amethyst ) )
					return GemType.Amethyst;
				if ( resourceType == typeof( Tourmaline ) )
					return GemType.Tourmaline;
				if ( resourceType == typeof( Amber ) )
					return GemType.Amber;
				if ( resourceType == typeof( Diamond ) )
					return GemType.Diamond;
				if ( resourceType == typeof( Oyster ) )
					return GemType.Pearl;
			}

			return GemType.None;
		}

		#endregion
	}
}