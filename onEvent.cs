#region imports

//Import various C# things.
using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

//Import Procon things.
using PRoCon.Core;
using PRoCon.Core.Plugin;
using PRoCon.Core.Players;
#endregion
namespace PRoConEvents
{
    public class onEvent : PRoConPluginAPI, IPRoConPluginInterface
    {

        //--------------------------------------
        //Class level variables.
        //--------------------------------------

        private bool pluginEnabled = false;

        private String debugLevelString = "1";
        private int debugLevel = 1;

        private int iOnPluginEnable = 1;
        private int iOnPluginDisable = 1;
        private int iOnPlayerJoin = 0;
		private int iOnPlayerSquadChange = 0;

        //--------------------------------------
        //Plugin constructor. Can be left blank.
        //--------------------------------------

        public onEvent()
        {

        }

        //--------------------------------------
        //Description settings for your plugin.
        //--------------------------------------

        public string GetPluginName()
        {
            return "onEvent";
        }

        public string GetPluginVersion()
        {
            return "0.1.5";
        }

        public string GetPluginAuthor()
        {
            return "Analytalica";
        }

        public string GetPluginWebsite()
        {
            return "purebattlefield.org";
        }

        public string GetPluginDescription()
        {
            return @"<p>It is difficult to determine exactly when some of PRoCon's events will run without having to manually test them. They can be different from what RCON spits out automatically.</p>
<p>This plugin will check for most, if not all, events that PRoCon can trigger (in accordance to the API). An output control switch is available for each event so a tester can control the plugin's output.</p>

<p><b>Output format:</b></p>

<p>[Timestamp] - [Function Name] - [Function Parameters, split by | ]</p>

<p><b>Usage:</b></p>

<p>Set individual event settings to 0 (off) or 1 (on) to receive output.</p>";
        }

        public void toConsole(params String[] arguments)
        {
            if (pluginEnabled)
            {
                if (arguments.Length == 1)
                {
                    this.ExecuteCommand("procon.protected.pluginconsole.write", "onEvent: " + arguments[0]);
                }
                else
                {
                    String completeMsg = arguments[0] + " - ";
                    for (int i = 1; i < arguments.Length; i++)
                    {
                        completeMsg = completeMsg + arguments[i] + " | ";
                    }
                    toConsole(completeMsg);
                }
            }
        }

        //--------------------------------------
        //These methods run when Procon does what's on the label.
        //--------------------------------------

        public void OnPluginLoaded(string strHostName, string strPort, string strPRoConVersion)
        {
            this.RegisterEvents(this.GetType().Name, "OnPluginLoaded", "OnPlayerJoin", "OnPlayerSquadChange");
        }

        public void OnPluginEnable()
        {
            this.pluginEnabled = true;
            if (iOnPluginEnable > 0)
                this.toConsole("OnPluginEnable()");
        }
		
		public void OnPluginDisable()
        {
			if (iOnPluginDisable > 0)
				this.toConsole("OnPluginDisable()");
            this.pluginEnabled = false;
        }

        public override void OnPlayerJoin(string soldierName)
        {
            if (iOnPlayerJoin > 0)
                this.toConsole("OnPlayerJoin(string soldierName)", soldierName);
        }
		
		public override void OnPlayerSquadChange(string soldierName, int teamId, int squadId) 
		{ 
			if (iOnPlayerSquadChange > 0)
				this.toConsole("OnPlayerSquadChange(string soldierName, int teamId, int squadId)", soldierName, teamId.ToString(), squadId.ToString());
		}

        //List plugin variables.
        public List<CPluginVariable> GetDisplayPluginVariables()
        {
            List<CPluginVariable> lstReturn = new List<CPluginVariable>();
            lstReturn.Add(new CPluginVariable("Settings|OnPluginEnable()", typeof(string), iOnPluginEnable.ToString()));
			lstReturn.Add(new CPluginVariable("Settings|OnPluginDisable()", typeof(string), iOnPluginDisable.ToString()));
            lstReturn.Add(new CPluginVariable("Settings|OnPlayerJoin(string soldierName)", typeof(string), iOnPlayerJoin.ToString()));
			lstReturn.Add(new CPluginVariable("Settings|OnPlayerSquadChange(string soldierName, int teamId, int squadId)", typeof(string), iOnPlayerSquadChange.ToString()));
            return lstReturn;
        }

        public List<CPluginVariable> GetPluginVariables()
        {
            return GetDisplayPluginVariables();
        }

        //Set variables.
        public void SetPluginVariable(String strVariable, String strValue)
        {
            if (strVariable.Contains("OnPluginEnable()"))
            {
                try
                {
                    iOnPluginEnable = Int32.Parse(strValue);
                }
                catch (Exception z)
                {
                    toConsole("Invalid setting level! Only use integer values.");
                    iOnPluginEnable = 1;
                }
            }
            else if (strVariable.Contains("OnPluginDisable()"))
            {
                try
                {
                    iOnPluginDisable = Int32.Parse(strValue);
                }
                catch (Exception z)
                {
                    toConsole("Invalid setting level! Only use integer values.");
                    iOnPluginDisable = 1;
                }
            }
            else if (strVariable.Contains("OnPlayerJoin(string soldierName)"))
            {
                try
                {
                    iOnPlayerJoin = Int32.Parse(strValue);
                }
                catch (Exception z)
                {
                    toConsole("Invalid setting level! Only use integer values.");
                    iOnPlayerJoin = 0;
                }
            }
            else if (strVariable.Contains("OnPlayerSquadChange(string soldierName, int teamId, int squadId)"))
            {
                try
                {
                    iOnPlayerSquadChange = Int32.Parse(strValue);
                }
                catch (Exception z)
                {
                    toConsole("Invalid setting level! Only use integer values.");
                    iOnPlayerSquadChange = 0;
                }
            }
        }
    }
}