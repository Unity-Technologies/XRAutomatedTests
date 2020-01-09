using com.unity.cliconfigmanager;
using System.IO;
using System;
using UnityEditor;
using UnityEditor.TestTools.Graphics;
using UnityEngine.TestTools;

public class TestSetup : IPrebuildSetup
{
    string imageResultsSaveDir = "";

    public void Setup()
    {
        SetupActualImagePath();
        ConfigureSettings();
        SetupGraphicsTestCases.Setup(imageResultsPath: imageResultsSaveDir);
    }

    private void SetupActualImagePath()
    {
        var args = Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "-testResults")
            {
                if (string.IsNullOrEmpty(args[i + 1]))
                    break;

                imageResultsSaveDir = Path.Combine(Path.GetDirectoryName(args[i + 1]), "ResultsImages");
                break;
            }
        }
    }

    [MenuItem("Tests/Configure Settings")]
    public static void ConfigureSettings()
    {
        new CliConfigManager().ConfigureFromCmdlineArgs();
    }
}