using System;
using Server;

namespace Server
{
	public class ParalyzeImmunityBuff : EnhancedBuff
	{
		public ParalyzeImmunityBuff( Mobile target ) : base( BuffIcon.Deflection, 1075814, 1070813, TimeSpan.FromSeconds( 5.0 ), target )
		{
		}
		
		public ParalyzeImmunityBuff( Mobile target, TimeSpan duration ) : base( BuffIcon.Deflection, 1075814, 1070813, duration, target )
		{
		}
	}
}