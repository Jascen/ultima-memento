using Server.Mobiles;
using System;

namespace Server.Engines.MobileEnhancement
{
	/// <summary>
	/// Tracks the Source, validates range, and allows a grace period
	/// after the target goes out of range before the effect expires.
	/// </summary>
	public abstract class RangeDependentRecipient<TEnhancement> : Recipient<TEnhancement>
		where TEnhancement : class
	{
		protected readonly int MaxExpirationTimeSeconds;
		protected readonly int MaxRange;
		protected readonly PlayerMobile Source;
		protected readonly int ValidationIntervalSeconds;

		/// <summary>When the target first went out of range; null if currently in range or never out of range.</summary>
		private DateTime? m_OutOfRangeSince;

		protected RangeDependentRecipient(PlayerMobile source, Mobile targetMobile) : this(source, targetMobile, 10, 5, 30)
		{
		}

		protected RangeDependentRecipient(PlayerMobile source, Mobile targetMobile, int maxRange, int validationIntervalSeconds, int maxExpirationTimeSeconds) : base(targetMobile)
		{
			Source = source;
			MaxRange = maxRange;
			ValidationIntervalSeconds = validationIntervalSeconds;
			MaxExpirationTimeSeconds = maxExpirationTimeSeconds;
		}

		/// <summary>Valid if base rules pass and either in range or within the out-of-range grace period.</summary>
		public override bool GetIsValid(DateTime now)
		{
			if (!base.GetIsValid(now)) return false;

			if (TargetMobile.InRange(Source.Location, MaxRange))
			{
				m_OutOfRangeSince = null;
				return true;
			}

			// Range check failed. Make sure we set when.
			if (m_OutOfRangeSince == null) m_OutOfRangeSince = now;

			return now < m_OutOfRangeSince.Value.AddSeconds(MaxExpirationTimeSeconds);
		}

		/// <summary>Next validation time; null for the bard (self), otherwise every VALIDATION_INTERVAL_SECONDS.</summary>
		public override DateTime? GetNextValidationAt(DateTime appliedAt, DateTime? lastValidated)
		{
			if (lastValidated != null)
			{
				return lastValidated.Value.AddSeconds(ValidationIntervalSeconds);
			}
			else
			{
				return DateTime.Now.AddSeconds(ValidationIntervalSeconds);
			}
		}
	}
}