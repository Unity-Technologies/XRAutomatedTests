using com.unity.cliconfigmanager;
using UnityEngine;
using UnityEditor;
using System;
using System.Text.RegularExpressions;
namespace Assets.Editor
{
    public class Build
    {
        public static void CommandLineSetup()
        {
            var cliConfigManager = new CliConfigManager();
            cliConfigManager.ConfigureFromCmdlineArgs();
            
            #if UNITY_WSA
            if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.WSA)
            {
                string SimulationMode = getSimulationMode();
                Debug.Log("SimulationMode="+SimulationMode);

                if (SimulationMode == "hololens")
                    WindowsMRFunctionTestBase.SetupHolographicEmulationWindow(true);
                else
                    WindowsMRFunctionTestBase.SetupHolographicEmulationWindow(false);
            }
		    #endif
        }

        private static string getSimulationMode()
        {

            string[] cmdLine = Environment.GetCommandLineArgs();
            Regex regex = new Regex("simulationMode=(.*)");
            for (int i=0;i<cmdLine.Length;i++)
            {
                MatchCollection matches = regex.Matches(cmdLine[i]);
                foreach (Match match in matches)
                {
                    return match.Groups[1].Value.ToLower();
                }
                    
            }

            return "";
        }
    }
}

