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

        private String OnPluginEnableString = "1";
        private int OnPluginEnable = 1;
		private String OnPluginDisableString = "1";
        private int OnPluginDisable = 1;
        private String OnPlayerJoinString = "0";
        private int OnPlayerJoin = 0;
		private String OnPlayerSquadChange = "0";
		private int OnPlayerSquadChange = 0;

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
            return "0.1.1";
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

        //--------------------------------------
        //Helper Functions
        //--------------------------------------
        #region Chat Functions
        //Sends a message to chat.
        //There is a ~126 character limitation to the amount of characters you can use in one chat message.
        //You can split chat messages into multiple lines (multiple chats in-game) using "\n" (newline).

        public void toChat(String message)
        {
            List<string> multiMsg = splitMessage(message, 128);
            foreach (string send in multiMsg)
            {
                this.ExecuteCommand("procon.protected.send", "admin.say", message, "all");
            }
        }

        //Sends a message to a specific player in chat.
        //Note the two parameters.

        public void toChat(String message, String playerName)
        {

            List<string> multiMsg = splitMessage(message, 128);
            foreach (string send in multiMsg)
            {
                this.ExecuteCommand("procon.protected.send", "admin.say", message, "player", playerName);
            }

        }

        //Ra4King's super awesome splitmessage function.
        /* Admin.Say messages may exceed 128 characters; 
         * they are appropriately split up by, in order of precedence: 
         * newlines, end-of-sentence punctuation marks, commas, spaces, or arbitrarily.
         */

        private static List<string> splitMessage(string message, int maxSize)
        {
            List<string> messages = new List<string>(message.Replace("\r", "").Trim().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));

            for (int a = 0; a < messages.Count; a++)
            {
                messages[a] = messages[a].Trim();

                if (messages[a] == "")
                {
                    messages.RemoveAt(a);
                    a--;
                    continue;
                }

                if (messages[a][0] == '/')
                    messages[a] = ' ' + messages[a];

                string msg = messages[a];

                if (msg.Length > maxSize)
                {
                    List<int> splitOptions = new List<int>();
                    int split = -1;
                    do
                    {
                        split = msg.IndexOfAny(new char[] { '.', '!', '?', ';' }, split + 1);
                        if (split != -1 && split != msg.Length - 1)
                            splitOptions.Add(split);
                    } while (split != -1);

                    if (splitOptions.Count > 2)
                        split = splitOptions[(int)Math.Round(splitOptions.Count / 2.0)] + 1;
                    else if (splitOptions.Count > 0)
                        split = splitOptions[0] + 1;
                    else
                    {
                        split = msg.IndexOf(',');

                        if (split == -1)
                        {
                            split = msg.IndexOf(' ', msg.Length / 2);

                            if (split == -1)
                            {
                                split = msg.IndexOf(' ');

                                if (split == -1)
                                    split = maxSize / 2;
                            }
                        }
                    }

                    messages[a] = msg.Substring(0, split).Trim();
                    messages.Insert(a + 1, msg.Substring(split).Trim());

                    a--;
                }
            }

            return messages;
        }
        #endregion

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
            if (OnPluginEnable > 0)
                this.toConsole("OnPluginEnable()");
        }
		
		public void OnPluginDisable()
        {
			if (OnPluginDisable > 0)
				this.toConsole("OnPluginDisable()");
            this.pluginEnabled = false;
        }

        public override void OnPlayerJoin(string soldierName)
        {
            if (OnPlayerJoin > 0)
                this.toConsole("OnPlayerJoin(string soldierName)",soldierName);
        }
		
		public override void OnPlayerSquadChange(string soldierName, int teamId, int squadId) 
		{ 
			if (OnPlayerSquadChange > 0)
				this.toConsole("OnPlayerSquadChange(string soldierName, int teamId, int squadId)",soldierName, teamId.ToString(), squadId.ToString());
		}

        //List plugin variables.
        public List<CPluginVariable> GetDisplayPluginVariables()
        {
            List<CPluginVariable> lstReturn = new List<CPluginVariable>();
            lstReturn.Add(new CPluginVariable("Settings|OnPluginEnable()", typeof(string), OnPluginEnableString));
			lstReturn.Add(new CPluginVariable("Settings|OnPluginDisable()", typeof(string), OnPluginDisableString));
            lstReturn.Add(new CPluginVariable("Settings|OnPlayerJoin(string soldierName)", typeof(string), OnPlayerJoinString));
			lstReturn.Add(new CPluginVariable("Settings|OnPlayerSquadChange(string soldierName, int teamId, int squadId)", typeof(string), OnPlayerSquadChange));
            return lstReturn;
        }

        public List<CPluginVariable> GetPluginVariables()
        {
            return GetDisplayPluginVariables();
        }

        //Set variables.
        public void SetPluginVariable(String strVariable, String strValue)
        {
            if (Regex.Match(strVariable, @"OnPluginEnable()").Success)
            {
                OnPluginEnableString = strValue;
                try
                {
                    OnPluginEnable = Int32.Parse(OnPluginEnableString);
                }
                catch (Exception z)
                {
                    toConsole("Invalid setting level! Only use integer values.");
                    OnPluginEnable = 1;
                    OnPluginEnableString = "1";
                }
            }
			else if (Regex.Match(strVariable, @"OnPluginDisable()").Success)
            {
                OnPluginDisableString = strValue;
                try
                {
                    OnPluginDisable = Int32.Parse(OnPluginDisableString);
                }
                catch (Exception z)
                {
                    toConsole("Invalid setting level! Only use integer values.");
                    OnPluginDisable = 1;
                    OnPluginDisableString = "1";
                }
            }
            else if (Regex.Match(strVariable, @"OnPlayerJoin(string soldierName)").Success)
            {
                OnPlayerJoinString = strValue;
                try
                {
                    OnPlayerJoin = Int32.Parse(OnPlayerJoinString);
                }
                catch (Exception z)
                {
                    toConsole("Invalid setting level! Only use integer values.");
                    OnPlayerJoin = 0;
                    OnPlayerJoinString = "0";
                }
            }
			else if (Regex.Match(strVariable, @"OnPlayerSquadChange(string soldierName, int teamId, int squadId)").Success)
            {
                OnPlayerSquadChangeString = strValue;
                try
                {
                    OnPlayerSquadChange = Int32.Parse(OnPlayerSquadChangeString);
                }
                catch (Exception z)
                {
                    toConsole("Invalid setting level! Only use integer values.");
                    OnPlayerSquadChange = 0;
                    OnPlayerSquadChangeString = "0";
                }
            }
        }
    }
}