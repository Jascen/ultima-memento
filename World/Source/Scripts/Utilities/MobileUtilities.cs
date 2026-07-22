using Server;
using Server.Mobiles;

public class MobileUtilities
{
	/// <summary>
	/// Returns the Luck from the Player that killed or last damaged the provided Mobile
	/// </summary>
	public static int GetLuckFromKiller(BaseCreature m)
	{
		var lastKiller = m.LastKiller;
		var player = TryGetMasterPlayer(lastKiller);
		
		// Check for provocation
		if (player == null && m is BaseCreature)
		{
			BaseCreature bc = (BaseCreature)m;
			if (bc.BardProvoked)
			{
				lastKiller = bc;
				player = TryGetMasterPlayer(bc.BardMaster);
			}
		}

		// I don't know why this is done places, but we'll do it anyways
		if (player == null)
		{
			lastKiller = m.FindMostRecentDamager(true);
			player = TryGetMasterPlayer(lastKiller);
		}

		if (player == null) return 0;

		var luck = player.Luck;
		if (player == lastKiller) return luck;

		// Something other than the Player killed it, only use half the Player's Luck
		return luck / 2;
	}

	/// <summary>
	/// Returns the first ancestor that is a PlayerMobile
	/// </summary>
	public static PlayerMobile TryGetMasterPlayer(Mobile m)
	{
		while (m != null)
		{
			if (m is PlayerMobile) return (PlayerMobile)m;
			if (m is BaseCreature == false) return null; // Only BaseCreature can have masters

			m = ((BaseCreature)m).GetMaster();
		}

		return null;
	}

	/// <summary>
	/// Returns the PlayerMobile that killed or last damaged the provided Mobile
	/// </summary>
	public static PlayerMobile TryGetKillingPlayer(Mobile m)
	{
		var player = TryGetMasterPlayer(m.LastKiller);
		if (player != null) return player;
		
		// Check for provocation
		if (m is BaseCreature)
		{
			BaseCreature bc = (BaseCreature)m;
			if (bc.BardProvoked)
			{
				player = TryGetMasterPlayer(bc.BardMaster);
				if (player != null) return player;
			}
		}

		// I don't know why this is done places, but we'll do it anyways
		var lastDamager = m.FindMostRecentDamager(true);

		return TryGetMasterPlayer(lastDamager);
	}
}