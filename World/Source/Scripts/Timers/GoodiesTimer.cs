using System;
using Server.Items;

namespace Server.Timers
{
	public class GoodiesTimer : Timer
	{
		private readonly Map m_Map;
		private readonly int m_X, m_Y;

		public GoodiesTimer(Map map, int x, int y) : base(TimeSpan.FromSeconds(Utility.RandomDouble() * 5.0))
		{
			m_Map = map;
			m_X = x;
			m_Y = y;
		}

		public static void Create(IEntity m)
		{
			Create(m.Map, m.X, m.Y);
		}

		public static void Create(Map map, int fromX, int fromY, int radius = 12)
		{
			if ( map == null ) return;

			for ( int x = 0 - radius; x <= radius; ++x )
			{
				for ( int y = 0 - radius; y <= radius; ++y )
				{
					double dist = Math.Sqrt(x*x+y*y);
					if ( dist <= radius )
						new GoodiesTimer( map, fromX + x, fromY + y ).Start();
				}
			}
		}

		protected override void OnTick()
		{
			int z = m_Map.GetAverageZ(m_X, m_Y);
			bool canFit = m_Map.CanFit(m_X, m_Y, z, 6, false, false);
			for (int i = -3; !canFit && i <= 3; ++i)
			{
				canFit = m_Map.CanFit(m_X, m_Y, z + i, 6, false, false);

				if (canFit)
					z += i;
			}

			if (!canFit)
				return;

			Item g = new Gold(100, 200); g.Delete();

			int r1 = (int)(Utility.RandomMinMax(80, 160) * (MyServerSettings.GetGoldCutRate() * .01));
			int r2 = (int)(Utility.RandomMinMax(200, 400) * (MyServerSettings.GetGoldCutRate() * .01));
			int r3 = (int)(Utility.RandomMinMax(400, 800) * (MyServerSettings.GetGoldCutRate() * .01));
			int r4 = (int)(Utility.RandomMinMax(800, 1200) * (MyServerSettings.GetGoldCutRate() * .01));
			int r5 = (int)(Utility.RandomMinMax(1200, 1600) * (MyServerSettings.GetGoldCutRate() * .01));

			switch (Utility.Random(21))
			{
				case 0: g = new Crystals(r1); break;
				case 1: g = new DDGemstones(r2); break;
				case 2: g = new DDJewels(r2); break;
				case 3: g = new DDGoldNuggets(r3); break;
				case 4: g = new Gold(r3); break;
				case 5: g = new Gold(r3); break;
				case 6: g = new Gold(r3); break;
				case 7: g = new DDSilver(r4); break;
				case 8: g = new DDSilver(r4); break;
				case 9: g = new DDSilver(r4); break;
				case 10: g = new DDSilver(r4); break;
				case 11: g = new DDSilver(r4); break;
				case 12: g = new DDSilver(r4); break;
				case 13: g = new DDCopper(r5); break;
				case 14: g = new DDCopper(r5); break;
				case 15: g = new DDCopper(r5); break;
				case 16: g = new DDCopper(r5); break;
				case 17: g = new DDCopper(r5); break;
				case 18: g = new DDCopper(r5); break;
				case 19: g = new DDCopper(r5); break;
				case 20: g = new DDCopper(r5); break;
			}

			g.MoveToWorld(new Point3D(m_X, m_Y, z), m_Map);

			if (0.5 >= Utility.RandomDouble())
			{
				switch (Utility.Random(3))
				{
					case 0: // Fire column
						{
							Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
							Effects.PlaySound(g, g.Map, 0x208);

							break;
						}
					case 1: // Explosion
						{
							Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36BD, 20, 10, 5044);
							Effects.PlaySound(g, g.Map, 0x307);

							break;
						}
					case 2: // Ball of fire
						{
							Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36FE, 10, 10, 5052);

							break;
						}
				}
			}
		}
	}

}