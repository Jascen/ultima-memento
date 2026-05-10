namespace Server.Engines.MobileEnhancement
{
	/// <summary>An enhancement that applies to a single mobile.</summary>
	public interface IEnhancementRecipient<TEnhancement> : IEnhancement
		where TEnhancement : class
	{
		/// <summary>The mobile that receives this enhancement.</summary>
		Mobile TargetMobile { get; }
	}
}