using System;
using Server.Network;
using Server.Gumps;
using Server.Utilities;
using Server.Mobiles;

namespace Server.Items
{
	public enum SoulOrbType
	{
		Default,
		BloodOfVampire,
		CloningCrystalJedi,
		CloningCrystalSyth,
		RestorativeSoil,
		PermadeathPlaceholder
	}

    public class SoulOrb : Item
    {
		public override string DefaultDescription{ get{ return "These items will resurrect you automatically, after 10 seconds, if you meet an untimely end. If you want to dispose of it, use it in your pack, where it will then disappear from the world."; } }

        private Mobile m_Owner;

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; InvalidateProperties(); }
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public SoulOrbType OrbType { get; private set; }

        private Timer m_Timer;
        private static TimeSpan m_Delay = TimeSpan.FromSeconds( 10.0 );

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Delay { get { return m_Delay; } set { m_Delay = value; } }
	
        public static void Initialize()
        {
            EventSink.PlayerDeath += new PlayerDeathEventHandler(e => StartTimer(e.Mobile));
			EventSink.Login += new LoginEventHandler(e => StartTimer(e.Mobile));
        }

        private static void StartTimer(Mobile m)
        {
			SoulOrb orb = FindActive(m);
			if ( orb == null ) return;

			orb.StartTimer();
		}

        [Constructable]
        public SoulOrb() : base( 0x2C84 ) 
        {
            Name = "soul orb";
            LootType = LootType.Blessed;
			Movable = false;
            Weight = 1.0;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( OrbType == SoulOrbType.PermadeathPlaceholder )
			{
				from.SendMessage("This contains your soul. Are you trying to delete yourself?!");
				return;
			}

            var confirmation = new ConfirmationGump(
                from,
				"Delete " + Name,
                "Are you sure you wish to delete this?",
                () => Delete()
            );
            from.SendGump(confirmation);
		}

        public SoulOrb(Serial serial) : base(serial){}

		public static SoulOrb Create( Mobile from, SoulOrbType orbType )
		{
			if ( false == ( from is PlayerMobile ) ) return null;

			var player = from as PlayerMobile;
			if ( player.Temptations.HasPermanentDeath || player.Avatar.Active )
			{
				if ( orbType != SoulOrbType.PermadeathPlaceholder )
				{
					from.SendMessage("This item would have no effect.");
					return null;
				}
			}

			WorldUtilities.DeleteAllItems<SoulOrb>( item => item.m_Owner == from );

			var orb = new SoulOrb
			{
				m_Owner = from,
				OrbType = orbType
			};

			switch ( orb.OrbType )
			{
				case SoulOrbType.BloodOfVampire:
					orb.Name = "blood of a vampire";
					orb.ItemID = 0x122B;
					orb.Hue = 0;
					break;

				case SoulOrbType.CloningCrystalJedi:
					orb.Name = "replication crystal";
					orb.ItemID = 0x703;
					break;

				case SoulOrbType.CloningCrystalSyth:
					orb.Name = "replication crystal";
					orb.ItemID = 0x705;
					break;

				case SoulOrbType.RestorativeSoil:
					orb.Name = "mystical mud";
					orb.ItemID = 0x913;
					orb.Hue = 0;
					break;

				case SoulOrbType.PermadeathPlaceholder:
					orb.Visible = false;
					orb.Movable = false;
					break;
				
				case SoulOrbType.Default:
				default:
					break;
			}

			from.AddToBackpack( orb );

			if ( orb.RootParent == from )
			{
				BuffInfo.RemoveBuff( from, BuffIcon.Resurrection );
				BuffInfo.AddBuff( from, new BuffInfo( BuffIcon.Resurrection, 1063626, true ) );

				return orb;
			}
			else
			{
				from.PrivateOverheadMessage(MessageType.Regular, 38, false, "Your pack is full so it did not work!", from.NetState);
				from.SendMessage( "Your pack is full so it did not work!" );
				orb.Delete();

				return null;
			}
		}

		public static SoulOrb FindActive( Mobile from )
		{
			if ( from.Backpack == null ) return null;

			var orb = from.Backpack.FindItemByType( typeof ( SoulOrb ) ) as SoulOrb;
			if ( orb == null || orb.Deleted ) return null;

			return orb.Owner == from ? orb : null;
		}

        public override void AddNameProperties(ObjectPropertyList list)
		{
            base.AddNameProperties(list);

			switch ( OrbType )
			{
				case SoulOrbType.BloodOfVampire:
					list.Add( 1049644, "Contains vampire blood for " + m_Owner.Name );
					break;

				case SoulOrbType.CloningCrystalJedi:
				case SoulOrbType.CloningCrystalSyth:
					list.Add( 1049644, "Contains genetic patterns for " + m_Owner.Name );
					break;

				case SoulOrbType.PermadeathPlaceholder:
				case SoulOrbType.RestorativeSoil:
				case SoulOrbType.Default:
				default:
					list.Add( 1049644, "Contains the Soul of " + m_Owner.Name );
					break;
			}
        }

        public void StartTimer()
        {
			if ( Owner.Alive || Owner.Deleted ) return;

			if ( m_Timer != null )
			{
				m_Timer.Stop();
				m_Timer = null;
			}

			m_Timer = Timer.DelayCall(m_Delay, m_Delay, () =>
				{
					if ( Owner != null && !Owner.Deleted && !Deleted )
					{
						if ( Owner.Alive )
						{
							if ( m_Timer != null )
							{
								m_Timer.Stop();
								m_Timer = null;
							}

							return;
						}

						if ( Owner.NetState == null ) return;

						// Don't use this gump for permadeath
						if ( OrbType == SoulOrbType.PermadeathPlaceholder )
						{
							Owner.Resurrect();
						}
						else
						{
							var gump = new AutoResurrectGump(this);
							Owner.SendSound( 0x0F8 );
							Owner.CloseGump( typeof( AutoResurrectGump ) );
							Owner.SendGump( gump );
						}
					}
				}
			);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write( 2 ); // version  
			writer.Write( m_Owner );
			writer.Write( (int)OrbType );
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

			if ( version > 0 )
			{
				m_Owner = reader.ReadMobile();

				if ( version > 1 )
				{
					OrbType = (SoulOrbType)reader.ReadInt();
				}
				else
				{
					if ( Name == "blood of a vampire" ){ OrbType = SoulOrbType.BloodOfVampire; }
					else if ( Name == "cloning crystal" ) { 
						if ( ItemID == 0x703 ){ OrbType = SoulOrbType.CloningCrystalJedi; }
						else if ( ItemID == 0x705 ){ OrbType = SoulOrbType.CloningCrystalSyth; }
						else { OrbType = SoulOrbType.Default; }
					}
					else { OrbType = SoulOrbType.Default; }
				}
			}
            else
            {
                // none when the world starts 
                Timer.DelayCall(TimeSpan.FromSeconds(30), () => Delete());
            }
        }
    }

	public class AutoResurrectGump : Gump
	{
		private readonly SoulOrb m_Orb;

		public AutoResurrectGump( SoulOrb orb ): base( 50, 50 )
		{
			m_Orb = orb;

            Closable=true;
			Disposable=true;
			Dragable=true;

			const string color = "#b7cbda";

			AddPage(0);

			int img = 9586;
			if ( orb.Owner.Karma < 0 ){ img = 9587; }

			AddImage(0, 0, img, Server.Misc.PlayerSettings.GetGumpHue( orb.Owner ));
			AddHtml( 10, 11, 349, 20, @"<BODY><BASEFONT Color=" + color + ">RESURRECTION</BASEFONT></BODY>", (bool)false, (bool)false);
			AddButton(368, 10, 4017, 4017, 0, GumpButtonType.Reply, 0);

			AddHtml( 11, 41, 385, 141, @"<BODY><BASEFONT Color=" + color + ">The spirits offer their aid.<br><br>Do you accept?</BASEFONT></BODY>", (bool)false, (bool)false);

			AddButton(10, 225, 4023, 4023, 1, GumpButtonType.Reply, 0);
			AddButton(367, 225, 4020, 4020, 0, GumpButtonType.Reply, 0);
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			if ( info.ButtonID == 1 )
			{
				if( from.Map == null || !from.Map.CanFit( from.Location, 16, false, false ) )
				{
					from.SendLocalizedMessage( 502391 ); // Thou can not be resurrected there!
					return;
				}

				switch ( m_Orb.OrbType )
				{
					case SoulOrbType.BloodOfVampire:
						from.SendMessage("The blood pours out of the bottle, restoring your life.");
						break;

					case SoulOrbType.CloningCrystalJedi:
					case SoulOrbType.CloningCrystalSyth:
						from.SendMessage("The crystal forms a clone of your body, restoring your life.");
						break;

					case SoulOrbType.RestorativeSoil:
					case SoulOrbType.Default:
					default:
						from.SendMessage("The orb glows, releasing your soul.");
						break;
				}

				from.Resurrect();
				from.FixedEffect( 0x376A, 10, 16, Server.Misc.PlayerSettings.GetMySpellHue( true, from, 0 ), 0 );
				Server.Misc.Death.Penalty( from, false );
				BuffInfo.RemoveBuff( from, BuffIcon.Resurrection );
				m_Orb.Delete();
			}
			else
			{
				m_Orb.StartTimer();
			}
		}
	}
}