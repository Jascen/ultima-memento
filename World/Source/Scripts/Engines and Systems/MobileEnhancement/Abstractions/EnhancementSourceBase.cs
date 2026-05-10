using System.Collections.Generic;

namespace Server.Engines.MobileEnhancement
{
	public abstract class EnhancementSourceBase<TEnhancement, TRecipient>
		where TEnhancement : class
		where TRecipient : IEnhancementRecipient<TEnhancement>
	{
		protected readonly List<IEnhancementRecipient<TEnhancement>> Recipients = new List<IEnhancementRecipient<TEnhancement>>();

		protected abstract TRecipient CreateRecipient(Mobile mobile);

		/// <summary>Track the recipient and apply the enhancement.</summary>
		protected void RegisterRecipient(IEnhancementRecipient<TEnhancement> enhancementRecipient)
		{
			var context = Engine.Instance.GetOrCreateContext(enhancementRecipient.TargetMobile);
			context.AddEnhancement(enhancementRecipient);

			Recipients.Add(enhancementRecipient);
		}

		/// <summary>Remove the recipient and the enhancement from the mobile's context.</summary>
		protected void RemoveRecipient(IEnhancementRecipient<TEnhancement> recipient)
		{
			if (recipient == null || recipient.TargetMobile == null || recipient.TargetMobile.Deleted) return;
			if (!Recipients.Remove(recipient)) return;

			var context = Engine.Instance.GetContextOrDefault(recipient.TargetMobile);
			context.RemoveEnhancement(recipient);
		}
	}
}