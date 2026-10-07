using Server.Mobiles;

namespace Server.Engines.Avatar
{
	public interface IAvatarOnlyItem
	{
		PlayerMobile PlayerOwner { get; }
	}
}