using System;

namespace Server.Engines.MobileEnhancement
{
	/// <summary>
	/// Base for recipient enhancements that are removed only when the Source removes them.
	/// Effectively persistent (no engine validation); add to context for duplicate prevention.
	/// Requires the mobile that the recipient applies to (set in constructor).
	/// </summary>
	public abstract class Recipient<TEnhancement> : IEnhancementRecipient<TEnhancement>
		where TEnhancement : class
	{
		protected Recipient(Mobile targetMobile)
		{
			UniqueEnhancementType = typeof(TEnhancement);
			TargetMobile = targetMobile;
		}

		public Mobile TargetMobile { get; private set; }
		public Type UniqueEnhancementType { get; private set; }

		public virtual bool GetIsValid(DateTime now)
		{
			if (TargetMobile == null || TargetMobile.Deleted || !TargetMobile.Alive) return false;

			return true;
		}

		public virtual DateTime? GetNextValidationAt(DateTime appliedAt, DateTime? lastValidated)
		{
			// Never expires by default
			return null;
		}

		/// <summary>Remove the effect from TargetMobile and unregister from context; must be idempotent.</summary>
		public abstract void Remove();

		/// <summary>Apply the effect to TargetMobile; return false to skip adding to context.</summary>
		public abstract bool TryApply();
	}
}