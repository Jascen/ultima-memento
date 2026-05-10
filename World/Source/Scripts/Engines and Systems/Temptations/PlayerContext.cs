using System;

namespace Server.Temptation
{
	[PropertyObject]
	public class PlayerContext
	{
		public static readonly PlayerContext Default = new PlayerContext();

		public PlayerContext()
		{
		}

		public PlayerContext(PlayerContext context)
		{
			Flags = context.Flags;
		}

		public PlayerContext(GenericReader reader)
		{
			int version = reader.ReadInt();

			Flags = (TemptationFlags)reader.ReadInt();
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool AcceleratedSkillGain
		{ get { return GetFlag(TemptationFlags.Deathwish); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Active
		{ get { return this != Default; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool CanUsePuzzleboxes
		{
			get { return GetFlag(TemptationFlags.Puzzle_master); }
			set { SetFlag(TemptationFlags.Puzzle_master, value); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool CanWearTightPants
		{ get { return GetFlag(TemptationFlags.Strongest_Avenger); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public TemptationFlags Flags { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool HasAggressiveNerfs
		{
			get { return GetFlag(TemptationFlags.Aggressive_nerfs); }
			set { SetFlag(TemptationFlags.Aggressive_nerfs, value); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool HasPermanentDeath
		{
			get { return GetFlag(TemptationFlags.Deathwish); }
			set { SetFlag(TemptationFlags.Deathwish, value); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IncreaseMobDifficulty
		{
			get { return GetFlag(TemptationFlags.Strongest_Avenger); }
			set { SetFlag(TemptationFlags.Strongest_Avenger, value); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsBerserk
		{
			get { return GetFlag(TemptationFlags.I_can_take_it); }
			set { SetFlag(TemptationFlags.I_can_take_it, value); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool LimitTitanBonus
		{ get { return GetFlag(TemptationFlags.Puzzle_master | TemptationFlags.Aggressive_nerfs); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ReduceFugitiveSkillCap
		{ get { return GetFlag(TemptationFlags.Puzzle_master | TemptationFlags.Aggressive_nerfs); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ReduceRacialMagicalAttributes
		{ get { return GetFlag(TemptationFlags.Puzzle_master | TemptationFlags.Aggressive_nerfs); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ReduceStatGainDelay
		{ get { return GetFlag(TemptationFlags.Deathwish); } }

		public void Serialize(GenericWriter writer)
		{
			writer.Write(0); // version

			writer.Write((int)Flags);
		}

		public override string ToString()
		{
			return "...";
		}

		private bool GetFlag(TemptationFlags flag)
		{
			return (Flags & flag) != 0;
		}

		private void SetFlag(TemptationFlags flag, bool value)
		{
			// Never allow Default to be updated
			if (this == Default)
			{
				Console.WriteLine("[Temptation] Attempted to update Default PlayerContext");
				return;
			}

			if (value)
				Flags |= flag;
			else
				Flags &= ~flag;
		}
	}
}