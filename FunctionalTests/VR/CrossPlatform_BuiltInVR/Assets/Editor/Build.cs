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
            
            //Special check that will setup the Windows Holographic Emulation window prior to running any tests.
            #if UNITY_WSA
            //Ideally we would just access simulationMode in cliConfigManager.PlatformSettings.
            //Unfortunately that isn't publicly exposed at the moment, so we'll just use regex to quickly determine the SimulationMode.
            string SimulationMode = getSimulationMode();
            Debug.Log("Regex thinks the simulation mode is SimulationMode="+SimulationMode);
            if (SimulationMode == "hololens")
                WindowsMRFunctionTestBase.SetupHolographicEmulationWindow(true);
            else
                WindowsMRFunctionTestBase.SetupHolographicEmulationWindow(false);
            
		    #endif
        }

        #if UNITY_WSA
        //Use Regex to determine the Simulation Mode that was passed as a command line arguement.
        //Default to not activating the emulation window if we can't determine.
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
        #endif
    }
}

