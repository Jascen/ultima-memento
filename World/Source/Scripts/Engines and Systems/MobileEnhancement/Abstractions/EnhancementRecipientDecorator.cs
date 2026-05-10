using System;

namespace Server.Engines.MobileEnhancement
{
	public class EnhancementRecipientDecorator<TEnhancement, TRecipient> : IEnhancementRecipient<TEnhancement>
		where TEnhancement : class
		where TRecipient : IEnhancementRecipient<TEnhancement>
	{
		public readonly TRecipient Source;

		public EnhancementRecipientDecorator(TRecipient recipient)
		{
			TargetMobile = recipient.TargetMobile;
			UniqueEnhancementType = recipient.UniqueEnhancementType;
			GetIsValidFunc = recipient.GetIsValid;
			GetNextValidationAtFunc = recipient.GetNextValidationAt;
			RemoveFunc = recipient.Remove;
			TryApplyFunc = recipient.TryApply;
			Source = recipient;
		}

		public Func<DateTime, bool> GetIsValidFunc { get; set; }
		public Func<DateTime, DateTime?, DateTime?> GetNextValidationAtFunc { get; set; }
		public Action RemoveFunc { get; set; }
		public Mobile TargetMobile { get; set; }
		public Func<bool> TryApplyFunc { get; set; }
		public Type UniqueEnhancementType { get; set; }

		public bool GetIsValid(DateTime now)
		{
			return GetIsValidFunc(now);
		}

		public DateTime? GetNextValidationAt(DateTime appliedAt, DateTime? lastValidated)
		{
			return GetNextValidationAtFunc(appliedAt, lastValidated);
		}

		public void Remove()
		{
			RemoveFunc();
		}

		public bool TryApply()
		{
			return TryApplyFunc();
		}
	}
}