using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;

namespace Server.Engines.MobileEnhancement
{
	/// <summary> When the Owner is removed, all recipients are removed. </summary>
	public abstract class OwnerDependentEnhancement<TEnhancement, TRecipient> : EnhancementSourceBase<TEnhancement, TRecipient>
		where TEnhancement : class
		where TRecipient : RangeDependentRecipient<TEnhancement>
	{
		protected readonly PlayerMobile Caster;
		protected bool IsInitialized;
		protected TRecipient OwnerEnhancement;

		public OwnerDependentEnhancement(PlayerMobile caster)
		{
			Caster = caster;
		}

		/// <summary>Registers the bard as source recipient, then all mobiles in range (excluding self) as recipients.</summary>
		public void Initialize(IEnumerable<Mobile> mobiles)
		{
			if (IsInitialized) throw new InvalidOperationException("Already initialized");

			IsInitialized = true;

			OwnerEnhancement = CreateRecipient(Caster);

			// Wrap the Source to hijack the Remove call
			var wrapper = new EnhancementRecipientDecorator<TEnhancement, TRecipient>(OwnerEnhancement)
			{
				RemoveFunc = RemoveAll,

				// Never check the owner
				GetIsValidFunc = (now) => true,
				GetNextValidationAtFunc = (appliedAt, lastValidated) => null
			};
			RegisterRecipient(wrapper);

			// Register everyone else
			foreach (var mobile in mobiles)
			{
				// Quietly discard the owner for simplicity sake
				if (mobile == OwnerEnhancement.TargetMobile) continue;

				var recipient = CreateRecipient(mobile);
				RegisterRecipient(recipient);
			}
		}

		/// <summary>Removes all non-source recipients, then removes the source (bard) recipient.</summary>
		private void RemoveAll()
		{
			if (!IsInitialized) throw new InvalidOperationException("Not initialized");

			// Remove all recipients
			foreach (var recipient in Recipients.Where(r => r.TargetMobile != OwnerEnhancement.TargetMobile).ToList())
			{
				RemoveRecipient(recipient);
			}

			// Remove self last
			OwnerEnhancement.Remove();
		}
	}
}