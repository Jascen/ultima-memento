using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.MobileEnhancement
{
	/// <summary>
	/// Per-mobile container of active enhancements. Tracks each enhancement and runs Validate() to expire or reschedule them.
	/// </summary>
	public class MobileContext
	{
		public static readonly MobileContext Default = new MobileContext();

		private readonly List<Entry> m_Entries = new List<Entry>();

		/// <summary>Removes any existing enhancement of the same UniqueEnhancementType, then applies and schedules the new one.</summary>
		public void AddEnhancement(IEnhancement enhancement)
		{
			if (this == Default) return;
			if (enhancement == null) return;

			RemoveEnhancement(enhancement);
			if (!enhancement.TryApply()) return;

			var now = DateTime.Now;

			m_Entries.Add(
				new Entry
				{
					Enhancement = enhancement,
					AppliedAt = now,
					NextCheckAt = enhancement.GetNextValidationAt(now, null)
				}
			);
		}

		/// <summary>Removes and calls Remove() on all entries whose enhancement type matches the given enhancement's UniqueEnhancementType.</summary>
		public void RemoveEnhancement(IEnhancement enhancement)
		{
			if (this == Default) return;
			if (enhancement == null) return;

			foreach (var entry in m_Entries.Where(entry => entry.Enhancement.UniqueEnhancementType == enhancement.UniqueEnhancementType).ToList())
			{
				m_Entries.Remove(entry);
				entry.Enhancement.Remove();
			}
		}

		/// <summary>Runs validation for all entries that are due (NextCheckAt &lt;= now). Invalid enhancements are removed; valid ones get NextCheckAt updated.</summary>
		public void Validate(bool force = false)
		{
			if (this == Default) return;

			List<Entry> entriesToRemove = null; // Lazy removal
			var now = DateTime.Now;
			foreach (var entry in m_Entries.ToList())
			{
				if (!force && (entry.NextCheckAt == null || now < entry.NextCheckAt.Value)) continue;

				if (!entry.Enhancement.GetIsValid(now))
				{
					if (entriesToRemove == null) entriesToRemove = new List<Entry>();

					entriesToRemove.Add(entry);
					entry.Enhancement.Remove();
					continue;
				}

				entry.NextCheckAt = entry.Enhancement.GetNextValidationAt(entry.AppliedAt, now);
			}

			if (entriesToRemove != null)
			{
				foreach (var toRemove in entriesToRemove)
				{
					m_Entries.Remove(toRemove);
				}
			}
		}

		private class Entry
		{
			public DateTime AppliedAt { get; set; }
			public IEnhancement Enhancement { get; set; }
			public DateTime? NextCheckAt { get; set; }
		}
	}
}