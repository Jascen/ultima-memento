using System;
using System.Collections;
using Server.Mobiles;
using Server.Items;
using Server.Misc;

namespace Server.Spells.Song
{
	public class MagicFinaleSong : Song
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Magic Finale", "*plays a magic finale*",
				-1
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(0.5); } }
		public override double RequiredSkill { get { return 90.0; } }
		public override int RequiredMana { get { return 35; } }

		public MagicFinaleSong(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
		{
		}

		public override void OnCast()
		{
			base.OnCast();

			bool sings = false;

			if (CheckSequence())
			{
				sings = true;

				ArrayList targets = new ArrayList();

				foreach (Mobile m in Caster.GetMobilesInRange(4))
				{
					if (m is BaseCreature)
					{
						BaseCreature mn = m as BaseCreature;
						if (mn.IsTempEnemy)
							targets.Add(m);
					}

					if (m is BaseCreature && ((BaseCreature)m).Summoned)
						targets.Add(m);
				}

				Caster.FixedParticles(0x3709, 1, 30, 9965, 5, 7, EffectLayer.Waist);

				for (int i = 0; i < targets.Count; ++i)
				{
					Mobile m = (Mobile)targets[i];

					Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);

					m.Delete();
				}
			}

			BardFunctions.UseBardInstrument(BaseInstrument.GetInstrument(Caster), sings, Caster);
			FinishSequence();
		}
	}
}