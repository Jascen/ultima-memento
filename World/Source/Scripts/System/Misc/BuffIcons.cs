using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Network;

namespace Server
{
	public class BuffInfo
	{
		public static bool Enabled { get { return Core.ML; } }

		public static void Initialize()
		{
			if( Enabled )
			{
				EventSink.ClientVersionReceived += new ClientVersionReceivedHandler( delegate( ClientVersionReceivedArgs args )
				{
					PlayerMobile pm = args.State.Mobile as PlayerMobile;
					
					if( pm != null )
						Timer.DelayCall( TimeSpan.Zero, pm.ResendBuffs );
				} );
			}
		}

		#region Properties
		private BuffIcon m_ID;
		public BuffIcon ID { get { return m_ID; } }

		private int m_TitleCliloc;
		public int TitleCliloc { get { return m_TitleCliloc; } }

		private int m_SecondaryCliloc;
		public int SecondaryCliloc { get { return m_SecondaryCliloc; } }

		private TimeSpan m_TimeLength;
		public TimeSpan TimeLength { get { return m_TimeLength; } }

		private DateTime m_TimeStart;
		public DateTime TimeStart { get { return m_TimeStart; } }

		private Timer m_Timer;
		public Timer Timer { get { return m_Timer; } }

		private bool m_RetainThroughDeath;
		public bool RetainThroughDeath { get { return m_RetainThroughDeath; } }

		private TextDefinition m_Args;
		public TextDefinition Args { get { return m_Args; } }

		#endregion

		#region Constructors
		public BuffInfo( BuffIcon iconID, int titleCliloc ): this( iconID, titleCliloc, titleCliloc + 1 )
		{
		}

		public BuffInfo( BuffIcon iconID, int titleCliloc, int secondaryCliloc )
		{
			m_ID = iconID;
			m_TitleCliloc = titleCliloc;
			m_SecondaryCliloc = secondaryCliloc;
		}

		public BuffInfo( BuffIcon iconID, int titleCliloc, TimeSpan length, Mobile m ): this( iconID, titleCliloc, titleCliloc + 1, length, m )
		{
		}

		//Only the timed one needs to Mobile to know when to automagically remove it.
		public BuffInfo( BuffIcon iconID, int titleCliloc, int secondaryCliloc, TimeSpan length, Mobile m ): this( iconID, titleCliloc, secondaryCliloc )
		{
			m_TimeLength = length;
			m_TimeStart = DateTime.Now;

			m_Timer = Timer.DelayCall( length, new TimerCallback(
				delegate
				{
					PlayerMobile pm = m as PlayerMobile;

					if( pm == null )
						return;

					pm.RemoveBuff( this );
				} ) );
		}

		public BuffInfo( BuffIcon iconID, int titleCliloc, TextDefinition args ): this( iconID, titleCliloc, titleCliloc + 1, args )
		{
		}

		public BuffInfo( BuffIcon iconID, int titleCliloc, int secondaryCliloc, TextDefinition args ): this( iconID, titleCliloc, secondaryCliloc )
		{
			m_Args = args;
		}

		public BuffInfo( BuffIcon iconID, int titleCliloc, bool retainThroughDeath ): this( iconID, titleCliloc, titleCliloc + 1, retainThroughDeath )
		{
		}

		public BuffInfo( BuffIcon iconID, int titleCliloc, int secondaryCliloc, bool retainThroughDeath ): this( iconID, titleCliloc, secondaryCliloc )
		{
			m_RetainThroughDeath = retainThroughDeath;
		}

		public BuffInfo( BuffIcon iconID, int titleCliloc, TextDefinition args, bool retainThroughDeath ): this( iconID, titleCliloc, titleCliloc + 1, args, retainThroughDeath )
		{
		}

		public BuffInfo( BuffIcon iconID, int titleCliloc, int secondaryCliloc, TextDefinition args, bool retainThroughDeath ): this( iconID, titleCliloc, secondaryCliloc, args )
		{
			m_RetainThroughDeath = retainThroughDeath;
		}

		public BuffInfo( BuffIcon iconID, int titleCliloc, TimeSpan length, Mobile m, TextDefinition args ): this( iconID, titleCliloc, titleCliloc + 1, length, m, args )
		{
		}

		public BuffInfo( BuffIcon iconID, int titleCliloc, int secondaryCliloc, TimeSpan length, Mobile m, TextDefinition args ): this( iconID, titleCliloc, secondaryCliloc, length, m )
		{
			m_Args = args;
		}

		public BuffInfo( BuffIcon iconID, int titleCliloc, TimeSpan length, Mobile m, TextDefinition args, bool retainThroughDeath ): this( iconID, titleCliloc, titleCliloc + 1, length, m, args, retainThroughDeath )
		{
		}

		public BuffInfo( BuffIcon iconID, int titleCliloc, int secondaryCliloc, TimeSpan length, Mobile m, TextDefinition args, bool retainThroughDeath ): this( iconID, titleCliloc, secondaryCliloc, length, m )
		{
			m_Args = args;
			m_RetainThroughDeath = retainThroughDeath;
		}

		#endregion

		#region Convenience Methods
		public static void AddBuff( Mobile m, BuffInfo b )
		{
			PlayerMobile pm = m as PlayerMobile;

			if( pm != null )
				pm.AddBuff( b );
		}

		public static void RemoveBuff( Mobile m, BuffInfo b )
		{
			PlayerMobile pm = m as PlayerMobile;

			if( pm != null )
				pm.RemoveBuff( b );
		}

		public static void RemoveBuff( Mobile m, BuffIcon b )
		{
			PlayerMobile pm = m as PlayerMobile;

			if( pm != null )
				pm.RemoveBuff( b );
		}

		public static ParalyzeImmunityBuff GetParalyzeImmunity( Mobile m )
		{
			PlayerMobile pm = m as PlayerMobile;
			if( pm != null )
				return pm.GetParalyzeImmunity();
			
			return null;
		}

		public static bool HasParalyzeImmunity( Mobile m )
		{
			return GetParalyzeImmunity( m ) != null;
		}

		public static void AddParalyzeImmunity( Mobile m )
		{
			AddParalyzeImmunity( m, TimeSpan.FromSeconds( 5.0 ) );
		}
		
		public static void AddParalyzeImmunity( Mobile m, TimeSpan duration )
		{
			if ( m is PlayerMobile )
			{
				BuffInfo.RemoveBuff( m, BuffIcon.Deflection );
				BuffInfo.AddBuff( m, new ParalyzeImmunityBuff( m, duration ) );
			}
		}

		public static void CleanupIcons( Mobile m, bool onlyParalyzed )
		{
			if ( !onlyParalyzed )
			{
				if (m.Backpack != null)
				{
						if ( SoulOrb.FindActive(m)  == null )
							BuffInfo.RemoveBuff( m, BuffIcon.Resurrection );
						else
							BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.Resurrection, 1063626, true ) );

					Item gem = m.Backpack.FindItemByType( typeof ( GemImmortality ) );
						if ( gem == null )
							BuffInfo.RemoveBuff( m, BuffIcon.GemImmortality );
						else
							BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.GemImmortality, 1063658, true ) );

					Item shard = m.Backpack.FindItemByType( typeof ( JewelImmortality ) );
						if ( shard == null )
							BuffInfo.RemoveBuff( m, BuffIcon.WithstandDeath );
						else
							BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.WithstandDeath, 1063656, true ) );

					Item enchant = m.Backpack.FindItemByType( typeof ( EnchantSpellStone ) );
						if ( enchant == null )
							BuffInfo.RemoveBuff( m, BuffIcon.Enchant );

					Item enchanted = m.Backpack.FindItemByType( typeof ( ResearchEnchantStone ) );
						if ( enchanted == null )
							BuffInfo.RemoveBuff( m, BuffIcon.EnchantWeapon );
				}

				if ( !TransformationSpellHelper.UnderTransformation( m ) )
				{
					BuffInfo.RemoveBuff( m, BuffIcon.WraithForm );
					BuffInfo.RemoveBuff( m, BuffIcon.HorrificBeast );
					BuffInfo.RemoveBuff( m, BuffIcon.LichForm );
					BuffInfo.RemoveBuff( m, BuffIcon.VampiricEmbrace );
				}

				if ( m.MagicDamageAbsorb < 1 )
				{
					BuffInfo.RemoveBuff( m, BuffIcon.Absorption );
					BuffInfo.RemoveBuff( m, BuffIcon.PsychicWall );
					BuffInfo.RemoveBuff( m, BuffIcon.Deflection );
					BuffInfo.RemoveBuff( m, BuffIcon.TrialByFire );
					BuffInfo.RemoveBuff( m, BuffIcon.OrbOfOrcus );
					BuffInfo.RemoveBuff( m, BuffIcon.MagicReflection );
					BuffInfo.RemoveBuff( m, BuffIcon.ElementalEcho );
				}
				if ( !m.Poisoned )
				{
					BuffInfo.RemoveBuff( m, BuffIcon.Poisoned );
				}
			}

			if ( !m.Paralyzed )
			{
				BuffInfo.RemoveBuff( m, BuffIcon.ElementalHold );
				BuffInfo.RemoveBuff( m, BuffIcon.StasisField );
				BuffInfo.RemoveBuff( m, BuffIcon.Hilarity );
				BuffInfo.RemoveBuff( m, BuffIcon.Paralyze );
				BuffInfo.RemoveBuff( m, BuffIcon.SleepField );
				BuffInfo.RemoveBuff( m, BuffIcon.Sleep );
				BuffInfo.RemoveBuff( m, BuffIcon.MassSleep );
				BuffInfo.RemoveBuff( m, BuffIcon.ParalyzeField );
				BuffInfo.RemoveBuff( m, BuffIcon.GraspingRoots );
				BuffInfo.RemoveBuff( m, BuffIcon.Firefly );
				BuffInfo.RemoveBuff( m, BuffIcon.Begging );
				BuffInfo.RemoveBuff( m, BuffIcon.Confusion );
				BuffInfo.RemoveBuff( m, BuffIcon.Charm );
				BuffInfo.RemoveBuff( m, BuffIcon.Fear );

				if (m is PlayerMobile)
				{
					var player = (PlayerMobile)m;
					if (player.PeacedUntil < DateTime.Now)
						BuffInfo.RemoveBuff( m, BuffIcon.PeaceMaking );
				}
			}
		}
		#endregion
	}


	public enum BuffIcon : short
	{
		CheetahPaws=0x3E9, // 0x754C (30028)
		Deception=0x3EA, // 0x754A (30026)
		NightSight=0x3ED, // 0x755E (30046)
		DeathStrike, // 0x7549 (30025)
		EvilOmen, // 0x7551 (30033)
		Poisoned, // 0x7556 (30038)
		EyesOfTheDead, // 0x753A (30010)
		DivineFury, // 0x754D (30029)
		EnemyOfOne, // 0x754E (30030)
		HidingAndOrStealth, // 0x7565 (30053)
		ActiveMeditation, // 0x753B (30011)
		BloodOathCaster, // 0x7543 (30019)
		BloodOathCurse, // 0x7544 (30020)
		CorpseSkin, // 0x7546 (30022)
		Mindrot, // 0x755C (30044)
		PainSpike, // 0x755F (30047)
		Strangle, // 0x7566 (30054)
		GhostlyImages, // 0x7554 (30036)
		SpectralShadow, // 0x7540 (30016)
		DrainLifeGood, // 0x7568 (30056)
		DrainLifeBad, // 0x754F (30031)
		Projection, // 0x7550 (30032)
		Absorption, // 0x7553 (30035)
		Speed, // 0x753E (30014)
		ShieldOfHate, // 0x755D (30045)
		ReactiveArmor, // 0x7563 (30051)
		Protection, // 0x7562 (30050)
		ParalyzeField, // 0x753F (30015)
		MagicReflection, // 0x7559 (30041)
		Incognito, // 0x7557 (30039)
		ElementalArmor, // 0x754B (30027)
		AstralProjection, // 0x753D (30013)
		Polymorph, // 0x7561 (30049)
		Invisibility, // 0x7558 (30040)
		Paralyze, // 0x755B (30043)
		MassCurse, // 0x7560 (30048)
		RemoveTrap, // 0x7541 (30017)
		Clumsy, // 0x7545 (30021)
		FeebleMind, // 0x7552 (30034)
		Weaken, // 0x7569 (30057)
		Curse, // 0x7548 (30024)
		Resurrection, // 0x755A (30042)
		Agility, // 0x753C (30012)
		Cunning, // 0x7547 (30023)
		Strength, // 0x7567 (30055)
		Bless, // 0x7542 (30018)
		CheetahPawss, // 0x758A (30090)
		Deceptions, // 0x758B (30091)
		PsychicWall, // 0x758C (30092)
		WindRunner, // 0x758D (30093)
		NotUsed1, // 0x0000 (0)
		Insult, // 0x758E (30094)
		Hilarity, // 0x094B (2379)
		Deflection, // 0x094C (2380)
		Celerity, // 0x094D (2381)
		Mirage, // 0x094E (2382)
		PsychicAura, // 0x094F (2383)
		StasisField, // 0x0950 (2384)
		NotUsed2, // 0x753E (30014)
		HammerOfFaith, // 0x5011 (20497)
		SacredBoon, // 0x7590 (30096)
		Sanctify, // 0x7591 (30097)
		Seance, // 0x7592 (30098)
		TrialByFire, // 0x7593 (30099)
		Enchant, // 0x7594 (30100)
		BlendWithForest, // 0x7595 (30101)
		GrimReaper, // 0x7596 (30102)
		GraspingRoots, // 0x7598 (30104)
		WoodlandProtection, // 0x7599 (30105)
		OrbOfOrcus, // 0x759B (30107)
		ArmysPaeon, // 0x759C (30108)
		SoulReaper, // 0x759E (30110)
		StrengthOfSteel, // 0x759F (30111)
		SuccubusSkin, // 0x75A0 (30112)
		EnchantingEtude, // 0x75A1 (30113)
		EnergyCarol, // 0x75A3 (30115)
		EnergyThrenody, // 0x75A4 (30116)
		FireCarol, // 0x75A5 (30117)
		FireThrenody, // 0x75A6 (30118)
		IceCarol, // 0x75A7 (30119)
		IceThrenody, // 0x75C0 (30144)
		KnightsMinne, // 0x75C1 (30145)
		MagesBallad, // 0x75C2 (30146)
		PoisonCarol, // 0x75C3 (30147)
		PoisonThrenody, // 0x75C4 (30148)
		ShephardsDance, // 0x75F2 (30194)
		SinewyEtude, // 0x75F3 (30195)
		PotionAgility, // 0x75F4 (30196)
		WraithForm, // 0x75F5 (30197)
		ConsecrateWeapon, // 0x75F6 (30198)
		PotionInvisible, // 0x75F7 (30199)
		HorrificBeast, // 0x75F8 (30200)
		LichForm, // 0x75F9 (30201)
		PotionInvulnerable, // 0x75FA (30202)
		PotionNightSight, // 0x75FB (30203)
		PotionStrength, // 0x75FC (30204)
		VampiricEmbrace, // 0x75FD (30205)
		PotionSuperior, // 0x75FE (30206)
		CurseWeapon, // 0x75FF (30207)
		ElementalEcho, // 0x7600 (30208)
		ElementalHold, // 0x7601 (30209)
		ElementalProtect, // 0x7602 (30210)
		AirWalk, // 0x7603 (30211)
		ConfusionBlast, // 0x7604 (30212)
		EnchantWeapon, // 0x7605 (30213)
		EndureCold, // 0x7606 (30214)
		EndureHeat, // 0x7607 (30215)
		MaskDeath, // 0x7608 (30216)
		MassMight, // 0x7609 (30217)
		MassSleep, // 0x760A (30218)
		RockFlesh, // 0x760B (30219)
		Sleep, // 0x760C (30220)
		SleepField, // 0x760D (30221)
		Sneak, // 0x760E (30222)
		WithstandDeath, // 0x760F (30223)
		GemImmortality, // 0x7610 (30224)
		Intervention, // 0x7611 (30225)
		PeaceMaking, // 0x7612 (30226)
		Discordance, // 0x7613 (30227)
		Begging, // 0x7614 (30228)
		Firefly, // 0x7615 (30229)
		FishStr, // 0x75C5 (30149)
		Skip, // 0x75F6 (30198)
		Bandage, // 0x761B (30235)
		Confusion, // 0x9bc9 (39881)
		Charm, // 0x9bb5 (39861)
		Fear, // 0x9bdd (39901)
		FishDex, // 0x9bc6 (39878)
		FishInt, // 0x9bcc (39884)
		Spyglass, // 0x9bbe (39870)
		Confidence, // 0x9bbd (39869)
		Counter, // 0x9bcb (39883)
		Evasion, // 0x9bc8 (39880)
		Honorable, // 0x9bbf (39871)
		DexGainCooldown, // 0x9bcd (39885)
		IntGainCooldown, // 0x9bc0 (39872)
		StrGainCooldown, // 0x9bce (39886)
		AVAILABLE_1, // 0x9bc1 (39873)
		AVAILABLE_2, // 0x9bc7 (39879)
		AVAILABLE_3, // 0x9bc2 (39874)
		AVAILABLE_4, // 0x9bb7 (39863)
		AVAILABLE_5, // 0x9bca (39882)
		AVAILABLE_6, // 0x9bb6 (39862)
		AVAILABLE_7, // 0x9bb8 (39864)
		AVAILABLE_8, // 0x9bb9 (39865)
		AVAILABLE_9, // 0x9bba (39866)
		AVAILABLE_10, // 0x9bbb (39867)
		AVAILABLE_11, // 0x9bbc (39868)
		AVAILABLE_12, // 0x9bc3 (39875)
		AVAILABLE_13, // 0x9bc4 (39876)
		AVAILABLE_14, // 0x9bc5 (39877)
		AVAILABLE_15, // 0x9bd2 (39890)
		AVAILABLE_16, // 0x9bd3 (39891)
		AVAILABLE_17, // 0x9bd4 (39892)
		AVAILABLE_18, // 0x9bd5 (39893)
		AVAILABLE_19, // 0x9bd1 (39889)
		AVAILABLE_20, // 0x9bd6 (39894)
		AVAILABLE_21, // 0x9bd7 (39895)
		AVAILABLE_22, // 0x9bcf (39887)
		AVAILABLE_23, // 0x9bd8 (39896)
		AVAILABLE_24, // 0x9bd9 (39897)
		AVAILABLE_25, // 0x9bdb (39899)
		AVAILABLE_26, // 0x9bdc (39900)
		AVAILABLE_27, // 0x9bda (39898)
		AVAILABLE_28, // 0x9bd0 (39888)
		AVAILABLE_29, // 0x9bde (39902)
		AVAILABLE_30, // 0x9bdf (39903)

		AVAILABLE_31, // 0xC349 (49993)
		AVAILABLE_32, // 0xC34D (49997)
		AVAILABLE_33, // 0xC34E (49998)
		AVAILABLE_34, // 0xC34C (49996)
		AVAILABLE_35, // 0xC34B (49995)
		AVAILABLE_36, // 0xC34A (49994)
		AVAILABLE_37, // 0xC343 (49987)
		AVAILABLE_38, // 0xC345 (49989)
		AVAILABLE_39, // 0xC346 (49990)
		AVAILABLE_40, // 0xC347 (49991)
		AVAILABLE_41, // 0xC348 (49992)

		AVAILABLE_42, // 0x9CDE (40158)

		AVAILABLE_43, // 0x5DE1 (24033)
		AVAILABLE_44, // 0x5DDF (24031)
		AVAILABLE_45, // 0x5DE3 (24035)
		AVAILABLE_46, // 0x5DE5 (24037)
		AVAILABLE_47, // 0x5DE4 (24036)
		AVAILABLE_48, // 0x5DE6 (24038)
		AVAILABLE_49, // 0x5D51 (23889)

		AVAILABLE_50, // 0x0951 (2385)
    }

	public sealed class AddBuffPacket : Packet
	{
		public AddBuffPacket( Mobile m, BuffInfo info ): this( m, info.ID, info.TitleCliloc, info.SecondaryCliloc, info.Args, (info.TimeStart != DateTime.MinValue) ? ((info.TimeStart + info.TimeLength) - DateTime.Now) : TimeSpan.Zero )
		{
		}

		public AddBuffPacket( Mobile mob, BuffIcon iconID, int titleCliloc, int secondaryCliloc, TextDefinition args, TimeSpan length ): base( 0xDF )
		{
			bool hasArgs = (args != null);

			this.EnsureCapacity( (hasArgs ? (48 + args.ToString().Length * 2): 44) );
			m_Stream.Write( (int)mob.Serial );

			m_Stream.Write( (short)iconID );	// ID
			m_Stream.Write( (short)0x1 );		// Type 0 for removal. 1 for add 2 for Data

			m_Stream.Fill( 4 );

			m_Stream.Write( (short)iconID );	// ID
			m_Stream.Write( (short)0x01 );		// Type 0 for removal. 1 for add 2 for Data

			m_Stream.Fill( 4 );

			if( length < TimeSpan.Zero )
				length = TimeSpan.Zero;

			// Tell the Client the (de)buff lasts longer than it is
			m_Stream.Write( (short)Math.Ceiling(length.TotalSeconds) );	//Time in seconds

			m_Stream.Fill( 3 );
			m_Stream.Write( (int)titleCliloc );
			m_Stream.Write( (int)secondaryCliloc );

			if( !hasArgs )
			{
				m_Stream.Fill( 10 );
			}
			else
			{
				m_Stream.Fill( 4 );
				m_Stream.Write( (short)0x1 );	//Unknown -> Possibly something saying 'hey, I have more data!'?
				m_Stream.Fill( 2 );

				m_Stream.WriteLittleUniNull( String.Format( "\t{0}", args.ToString() ) );

				m_Stream.Write( (short)0x1 );	//Even more Unknown -> Possibly something saying 'hey, I have more data!'?
				m_Stream.Fill( 2 );
			}
		}
	}

	public sealed class RemoveBuffPacket : Packet
	{
		public RemoveBuffPacket( Mobile mob, BuffInfo info ): this( mob, info.ID )
		{
		}

		public RemoveBuffPacket( Mobile mob, BuffIcon iconID ): base( 0xDF )
		{
			this.EnsureCapacity( 13 );
			m_Stream.Write( (int)mob.Serial );

			m_Stream.Write( (short)iconID );	// ID
			m_Stream.Write( (short)0x0 );		// Type 0 for removal. 1 for add 2 for Data

			m_Stream.Fill( 4 );

			mob.Str = mob.Str;
			mob.Int = mob.Int;
			mob.Dex = mob.Dex;
		}
	}
}
