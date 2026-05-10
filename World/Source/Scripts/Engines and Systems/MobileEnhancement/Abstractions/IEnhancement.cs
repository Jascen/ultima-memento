using System;

namespace Server.Engines.MobileEnhancement
{
	public interface IEnhancement
	{
		/// <summary>Type used to identify this enhancement (e.g. for one-instance-per-type per mobile).</summary>
		Type UniqueEnhancementType { get; }

		/// <summary>Whether the enhancement should still be applied. Called when a validation is due.</summary>
		bool GetIsValid(DateTime now);

		/// <summary>
		/// When to run the next validation. <paramref name="lastValidated"/> is null when the enhancement has not yet been validated.
		/// Return null to schedule no further automatic checks (perpetual with no further checks).
		/// </summary>
		DateTime? GetNextValidationAt(DateTime appliedAt, DateTime? lastValidated);

		void Remove();

		bool TryApply();
	}
}
