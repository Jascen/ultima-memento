using Server.Mobiles;

namespace Server.Engines.Avatar
{
	public interface IAvatarOnlyItem
	{
		bool IsPersistent { get; }
		PlayerMobile PlayerOwner { get; }
	}
}