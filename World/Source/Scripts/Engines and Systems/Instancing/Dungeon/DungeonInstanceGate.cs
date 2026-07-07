using System;
using Server;

namespace Server.Engines.Instancing
{
	// Configurable world item that creates a real-region-backed dungeon instance.
	// Staff choose the source dungeon by index/name and choose a champion-style
		// difficulty. Players walk over or double-click to enter the instance owned by this gate.
		public class DungeonInstanceGate : Item
		{
		private int m_DungeonIndex;
		private string m_DungeonName;
		private Difficulty m_Difficulty;

		[Constructable]
			public DungeonInstanceGate() : base( 0xF6C )
			{
				Name = "a dungeon instance gate";
				Hue = 0x455;
				m_DungeonIndex = 0;
				m_Difficulty = Difficulty.Normal;

				ApplyGateAppearance();
				RefreshDungeonName();
			}

				private void ApplyGateAppearance()
				{
					ItemID = 0xF6C;
					Movable = false;
					Visible = true;
					Light = LightType.Circle300;
					if ( Hue == 0 )
						Hue = 0x455;
				}

		[CommandProperty( AccessLevel.GameMaster )]
		public int DungeonIndex
		{
			get { return m_DungeonIndex; }
			set
			{
				DungeonInstanceType.DungeonInstanceDefinition def = DungeonInstanceType.Instance.GetDefinition( value );
				if ( def != null )
					SetDefinition( def );
				else
				{
					m_DungeonIndex = value;
					InvalidateProperties();
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string DungeonName
		{
			get { return m_DungeonName; }
			set
			{
				DungeonInstanceType.DungeonInstanceDefinition def = DungeonInstanceType.Instance.FindDefinition( value );
				if ( def != null )
					SetDefinition( def );
				else
				{
					m_DungeonName = value;
					InvalidateProperties();
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string SelectedDungeon
		{
			get
			{
				DungeonInstanceType.DungeonInstanceDefinition def = SelectedDefinition;
				return def != null ? def.DisplayName : "Unresolved";
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int AvailableDungeons
		{
			get { return DungeonInstanceType.Instance.DefinitionCount; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int SpawnEntries
		{
			get
			{
				DungeonInstanceType.DungeonInstanceDefinition def = SelectedDefinition;
				return def != null ? def.SpawnCount : 0;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string InstanceAvailability
		{
			get
			{
				DungeonInstanceType.DungeonInstanceDefinition def = SelectedDefinition;
				if ( def == null )
					return "Unresolved";

				if ( def.CanSpawnInstance )
					return def.AvailabilityLabel;

				return String.Format( "{0}: {1}", def.AvailabilityLabel, def.InstanceBlockReason );
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public new Difficulty Difficulty
		{
			get { return m_Difficulty; }
			set
			{
				m_Difficulty = DungeonInstanceType.ClampDifficulty( value );
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string InstanceStatus
		{
			get { return DungeonInstanceType.Instance.GetInstanceStatus( Serial ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool CleanupInstance
		{
			get { return false; }
			set
			{
				if ( value )
				{
					DungeonInstanceType.Instance.CleanupInstance( Serial );
					InvalidateProperties();
				}
			}
		}

		private DungeonInstanceType.DungeonInstanceDefinition SelectedDefinition
		{
			get
			{
				DungeonInstanceType.DungeonInstanceDefinition def = null;

				if ( m_DungeonName != null && m_DungeonName.Length > 0 )
					def = DungeonInstanceType.Instance.FindDefinition( m_DungeonName );

				if ( def == null )
					def = DungeonInstanceType.Instance.GetDefinition( m_DungeonIndex );

				return def;
			}
		}

		private void SetDefinition( DungeonInstanceType.DungeonInstanceDefinition def )
		{
			if ( def == null )
				return;

			m_DungeonIndex = def.Index;
			m_DungeonName = def.Name;
			InvalidateProperties();
		}

		private void RefreshDungeonName()
		{
			DungeonInstanceType.DungeonInstanceDefinition def = DungeonInstanceType.Instance.GetDefinition( m_DungeonIndex );
			if ( def != null )
				m_DungeonName = def.Name;
		}

			public override void OnDoubleClick( Mobile from )
			{
				Enter( from, true );
			}

			public override bool OnMoveOver( Mobile from )
			{
				if ( from == null || !from.Player )
					return true;

				return !Enter( from, false );
			}

			private bool Enter( Mobile from, bool requireRange )
			{
				if ( from == null )
					return false;

				if ( requireRange && !from.InRange( GetWorldLocation(), 2 ) )
				{
					from.SendLocalizedMessage( 500446 ); // That is too far away.
					return false;
				}

				DungeonInstanceType.DungeonInstanceDefinition def = SelectedDefinition;
				if ( def == null )
				{
					from.SendMessage( "This dungeon instance gate is not configured with a valid dungeon." );
					return false;
				}

				Map returnMap = this.Map != null ? this.Map : from.Map;
				returnMap = DungeonInstanceType.NormalizeExternalMap( returnMap );
				Point3D returnPoint = GetWorldLocation();

				return DungeonInstanceType.Instance.SendToConfiguredInstance( from, Serial, def, m_Difficulty, returnMap, returnPoint, InstanceOwnerKind.PublicGateway );
			}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );

			DungeonInstanceType.DungeonInstanceDefinition def = SelectedDefinition;
			if ( def != null )
			{
				list.Add( 1070722, "Dungeon: " + def.DisplayName );
				list.Add( 1070722, "Difficulty: " + m_Difficulty );
				list.Add( 1070722, "Spawn entries: " + def.SpawnCount );
				list.Add( 1070722, "Availability: " + def.AvailabilityLabel );
				if ( !def.CanSpawnInstance )
					list.Add( 1070722, "Disabled: " + def.InstanceBlockReason );
				list.Add( 1070722, "Instance: " + DungeonInstanceType.Instance.GetInstanceStatus( Serial ) );
			}
			else
			{
				list.Add( 1070722, "Dungeon: unresolved" );
			}

				list.Add( 1070722, "Walk over or double-click to enter the instance" );
			}

		public DungeonInstanceGate( Serial serial ) : base( serial )
		{
		}

		public override void OnAfterDelete()
		{
			DungeonInstanceType.Instance.CleanupInstance( Serial );
			base.OnAfterDelete();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_DungeonIndex );
			writer.Write( (string) m_DungeonName );
			writer.Write( (int) m_Difficulty );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			m_DungeonIndex = reader.ReadInt();
				m_DungeonName = reader.ReadString();
				m_Difficulty = DungeonInstanceType.ClampDifficulty( (Difficulty)reader.ReadInt() );

				if ( m_Difficulty < Difficulty.Easy || m_Difficulty > Difficulty.Deadly )
					m_Difficulty = Difficulty.Normal;

					ApplyGateAppearance();
				}
		}
	}
