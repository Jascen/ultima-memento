using System;

namespace Server.Temptation
{
	[Flags]
	public enum TemptationFlags : byte
	{
		None							= 0x00000000,
		Puzzle_master					= 0x00000001,
		Strongest_Avenger				= 0x00000002,
		Famine							= 0x00000004, // Incomplete
		I_can_take_it					= 0x00000008,
		This_is_just_a_tribute			= 0x00000010, // Incomplete
		Deathwish						= 0x00000020,
		Aggressive_nerfs				= 0x00000040, // Aggressive nerfs to skills and stats
	}
}