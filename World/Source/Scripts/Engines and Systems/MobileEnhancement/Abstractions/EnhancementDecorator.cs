using System;

namespace Server.Engines.MobileEnhancement
{
	public class EnhancementDecorator : IEnhancement
	{
		public EnhancementDecorator(IEnhancement enhancement)
		{
			UniqueEnhancementType = enhancement.UniqueEnhancementType;
			GetIsValidFunc = enhancement.GetIsValid;
			GetNextValidationAtFunc = enhancement.GetNextValidationAt;
			RemoveFunc = enhancement.Remove;
			TryApplyFunc = enhancement.TryApply;
		}

		public Func<DateTime, bool> GetIsValidFunc { get; set; }
		public Func<DateTime, DateTime?, DateTime?> GetNextValidationAtFunc { get; set; }
		public Action RemoveFunc { get; set; }
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