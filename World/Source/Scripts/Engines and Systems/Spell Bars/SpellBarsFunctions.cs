using System;
using Server;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Misc;
using Server.Commands;
using Server.Commands.Generic;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using Server.Spells.Eighth;
using Server.Spells.Necromancy;
using Server.Spells.Chivalry;
using Server.Spells.DeathKnight; 
using Server.Spells.Song;
using Server.Spells.HolyMan;
using Server.Spells.Research;
using Server.Prompts;
using Server.Gumps;

namespace Server.SpellBars
{
    class ToolBarUpdates
    {
		public static void UpdateToolBar( PlayerMobile m, int nChange, string ToolBar, int nTotal )
		{
			ToolBarUpdates.InitializeToolBar( m, ToolBar );

			string ToolBarSetting = "";

			if ( ToolBar == "SetupBarsArch1" ){ ToolBarSetting = m.SpellBars.Arch1; }
			else if ( ToolBar == "SetupBarsArch2" ){ ToolBarSetting = m.SpellBars.Arch2; }
			else if ( ToolBar == "SetupBarsArch3" ){ ToolBarSetting = m.SpellBars.Arch3; }
			else if ( ToolBar == "SetupBarsArch4" ){ ToolBarSetting = m.SpellBars.Arch4; }
			else if ( ToolBar == "SetupBarsMage1" ){ ToolBarSetting = m.SpellBars.Mage1; }
			else if ( ToolBar == "SetupBarsMage2" ){ ToolBarSetting = m.SpellBars.Mage2; }
			else if ( ToolBar == "SetupBarsMage3" ){ ToolBarSetting = m.SpellBars.Mage3; }
			else if ( ToolBar == "SetupBarsMage4" ){ ToolBarSetting = m.SpellBars.Mage4; }
			else if ( ToolBar == "SetupBarsNecro1" ){ ToolBarSetting = m.SpellBars.Necro1; }
			else if ( ToolBar == "SetupBarsNecro2" ){ ToolBarSetting = m.SpellBars.Necro2; }
			else if ( ToolBar == "SetupBarsKnight1" ){ ToolBarSetting = m.SpellBars.Knight1; }
			else if ( ToolBar == "SetupBarsKnight2" ){ ToolBarSetting = m.SpellBars.Knight2; }
			else if ( ToolBar == "SetupBarsDeath1" ){ ToolBarSetting = m.SpellBars.Death1; }
			else if ( ToolBar == "SetupBarsDeath2" ){ ToolBarSetting = m.SpellBars.Death2; }
			else if ( ToolBar == "SetupBarsElly1" ){ ToolBarSetting = m.SpellBars.Elly1; }
			else if ( ToolBar == "SetupBarsElly2" ){ ToolBarSetting = m.SpellBars.Elly2; }
			else if ( ToolBar == "SetupBarsBard1" ){ ToolBarSetting = m.SpellBars.Bard1; }
			else if ( ToolBar == "SetupBarsBard2" ){ ToolBarSetting = m.SpellBars.Bard2; }
			else if ( ToolBar == "SetupBarsPriest1" ){ ToolBarSetting = m.SpellBars.Priest1; }
			else if ( ToolBar == "SetupBarsPriest2" ){ ToolBarSetting = m.SpellBars.Priest2; }
			else if ( ToolBar == "SetupBarsMonk1" ){ ToolBarSetting = m.SpellBars.Monk1; }
			else if ( ToolBar == "SetupBarsMonk2" ){ ToolBarSetting = m.SpellBars.Monk2; }

			string[] eachSetting = ToolBarSetting.Split('#');
			int nLine = 1;
			string newSettings = "";

			foreach (string eachSettings in eachSetting)
			{
				if ( nLine == nChange )
				{
					string sChange = "0";
					if ( eachSettings == "0" ){ sChange = "1"; }
					newSettings = newSettings + sChange + "#";
				}
				else if ( nLine > nTotal )
				{
				}
				else
				{
					newSettings = newSettings + eachSettings + "#";
				}
				nLine++;
			}

			if ( ToolBar == "SetupBarsArch1" ){ m.SpellBars.Arch1 = newSettings; }
			else if ( ToolBar == "SetupBarsArch2" ){ m.SpellBars.Arch2 = newSettings; }
			else if ( ToolBar == "SetupBarsArch3" ){ m.SpellBars.Arch3 = newSettings; }
			else if ( ToolBar == "SetupBarsArch4" ){ m.SpellBars.Arch4 = newSettings; }
			else if ( ToolBar == "SetupBarsMage1" ){ m.SpellBars.Mage1 = newSettings; }
			else if ( ToolBar == "SetupBarsMage2" ){ m.SpellBars.Mage2 = newSettings; }
			else if ( ToolBar == "SetupBarsMage3" ){ m.SpellBars.Mage3 = newSettings; }
			else if ( ToolBar == "SetupBarsMage4" ){ m.SpellBars.Mage4 = newSettings; }
			else if ( ToolBar == "SetupBarsNecro1" ){ m.SpellBars.Necro1 = newSettings; }
			else if ( ToolBar == "SetupBarsNecro2" ){ m.SpellBars.Necro2 = newSettings; }
			else if ( ToolBar == "SetupBarsKnight1" ){ m.SpellBars.Knight1 = newSettings; }
			else if ( ToolBar == "SetupBarsKnight2" ){ m.SpellBars.Knight2 = newSettings; }
			else if ( ToolBar == "SetupBarsDeath1" ){ m.SpellBars.Death1 = newSettings; }
			else if ( ToolBar == "SetupBarsDeath2" ){ m.SpellBars.Death2 = newSettings; }
			else if ( ToolBar == "SetupBarsElly1" ){ m.SpellBars.Elly1 = newSettings; }
			else if ( ToolBar == "SetupBarsElly2" ){ m.SpellBars.Elly2 = newSettings; }
			else if ( ToolBar == "SetupBarsBard1" ){ m.SpellBars.Bard1 = newSettings; }
			else if ( ToolBar == "SetupBarsBard2" ){ m.SpellBars.Bard2 = newSettings; }
			else if ( ToolBar == "SetupBarsPriest1" ){ m.SpellBars.Priest1 = newSettings; }
			else if ( ToolBar == "SetupBarsPriest2" ){ m.SpellBars.Priest2 = newSettings; }
			else if ( ToolBar == "SetupBarsMonk1" ){ m.SpellBars.Monk1 = newSettings; }
			else if ( ToolBar == "SetupBarsMonk2" ){ m.SpellBars.Monk2 = newSettings; }
		}

		public static void InitializeToolBar( PlayerMobile m, string ToolBar )
		{
			if ( ToolBar == "SetupBarsArch1" && ( m.SpellBars.Arch1 == null || m.SpellBars.Arch1.Length < 132 ) ){ m.SpellBars.Arch1 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsArch2" && ( m.SpellBars.Arch2 == null || m.SpellBars.Arch2.Length < 132 ) ){ m.SpellBars.Arch2 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsArch3" && ( m.SpellBars.Arch3 == null || m.SpellBars.Arch3.Length < 132 ) ){ m.SpellBars.Arch3 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsArch4" && ( m.SpellBars.Arch4 == null || m.SpellBars.Arch4.Length < 132 ) ){ m.SpellBars.Arch4 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsMage1" && m.SpellBars.Mage1 == null ){ m.SpellBars.Mage1 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsMage2" && m.SpellBars.Mage2 == null ){ m.SpellBars.Mage2 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsMage3" && m.SpellBars.Mage3 == null ){ m.SpellBars.Mage3 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsMage4" && m.SpellBars.Mage4 == null ){ m.SpellBars.Mage4 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsNecro1" && m.SpellBars.Necro1 == null ){ m.SpellBars.Necro1 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsNecro2" && m.SpellBars.Necro2 == null ){ m.SpellBars.Necro2 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsKnight1" && m.SpellBars.Knight1 == null ){ m.SpellBars.Knight1 = "0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsKnight2" && m.SpellBars.Knight2 == null ){ m.SpellBars.Knight2 = "0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsDeath1" && m.SpellBars.Death1 == null ){ m.SpellBars.Death1 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsDeath2" && m.SpellBars.Death2 == null ){ m.SpellBars.Death2 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsElly1" && m.SpellBars.Elly1 == null ){ m.SpellBars.Elly1 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsElly2" && m.SpellBars.Elly2 == null ){ m.SpellBars.Elly2 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsBard1" && m.SpellBars.Bard1 == null ){ m.SpellBars.Bard1 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsBard2" && m.SpellBars.Bard2 == null ){ m.SpellBars.Bard2 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsPriest1" && m.SpellBars.Priest1 == null ){ m.SpellBars.Priest1 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsPriest2" && m.SpellBars.Priest2 == null ){ m.SpellBars.Priest2 = "0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsMonk1" && m.SpellBars.Monk1 == null ){ m.SpellBars.Monk1 = "0#0#0#0#0#0#0#0#0#0#0#0#"; }
			else if ( ToolBar == "SetupBarsMonk2" && m.SpellBars.Monk2 == null ){ m.SpellBars.Monk2 = "0#0#0#0#0#0#0#0#0#0#0#0#"; }
		}

		public static string GetToolBarSettings( Mobile m, string ToolBar )
		{
			PlayerMobile pm = m as PlayerMobile;
			if (pm == null) return "";
			
			ToolBarUpdates.InitializeToolBar( pm, ToolBar );

			if ( ToolBar == "SetupBarsArch1" ){ return pm.SpellBars.Arch1; }
			else if ( ToolBar == "SetupBarsArch2" ){ return pm.SpellBars.Arch2; }
			else if ( ToolBar == "SetupBarsArch3" ){ return pm.SpellBars.Arch3; }
			else if ( ToolBar == "SetupBarsArch4" ){ return pm.SpellBars.Arch4; }
			else if ( ToolBar == "SetupBarsMage1" ){ return pm.SpellBars.Mage1; }
			else if ( ToolBar == "SetupBarsMage2" ){ return pm.SpellBars.Mage2; }
			else if ( ToolBar == "SetupBarsMage3" ){ return pm.SpellBars.Mage3; }
			else if ( ToolBar == "SetupBarsMage4" ){ return pm.SpellBars.Mage4; }
			else if ( ToolBar == "SetupBarsNecro1" ){ return pm.SpellBars.Necro1; }
			else if ( ToolBar == "SetupBarsNecro2" ){ return pm.SpellBars.Necro2; }
			else if ( ToolBar == "SetupBarsKnight1" ){ return pm.SpellBars.Knight1; }
			else if ( ToolBar == "SetupBarsKnight2" ){ return pm.SpellBars.Knight2; }
			else if ( ToolBar == "SetupBarsDeath1" ){ return pm.SpellBars.Death1; }
			else if ( ToolBar == "SetupBarsDeath2" ){ return pm.SpellBars.Death2; }
			else if ( ToolBar == "SetupBarsElly1" ){ return pm.SpellBars.Elly1; }
			else if ( ToolBar == "SetupBarsElly2" ){ return pm.SpellBars.Elly2; }
			else if ( ToolBar == "SetupBarsBard1" ){ return pm.SpellBars.Bard1; }
			else if ( ToolBar == "SetupBarsBard2" ){ return pm.SpellBars.Bard2; }
			else if ( ToolBar == "SetupBarsPriest1" ){ return pm.SpellBars.Priest1; }
			else if ( ToolBar == "SetupBarsPriest2" ){ return pm.SpellBars.Priest2; }
			else if ( ToolBar == "SetupBarsMonk1" ){ return pm.SpellBars.Monk1; }
			else if ( ToolBar == "SetupBarsMonk2" ){ return pm.SpellBars.Monk2; }

			return "";
		}

		public static int GetToolBarSetting( Mobile m, int nSetting, string ToolBar )
		{
			PlayerMobile pm = (PlayerMobile)m;
			string sSetting = "0";

			ToolBarUpdates.InitializeToolBar( pm, ToolBar );

			string ToolBarSetting = GetToolBarSettings( pm, ToolBar );

			string[] eachSetting = ToolBarSetting.Split('#');
			int nLine = 1;

			foreach (string eachSettings in eachSetting)
			{
				if ( nLine == nSetting ){ sSetting = eachSettings; }
				nLine++;
			}

			int nValue = Convert.ToInt32(sSetting);

			return nValue;
		}
	}
}