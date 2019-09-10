using UnityEditor.TestTools.Graphics;
using UnityEngine.TestTools;
using com.unity.cliconfigmanager;
using System.IO;

public class TestSetup : IPrebuildSetup
{
    public void Setup()
    {
        new CliConfigManager().ConfigureFromCmdlineArgs();

        string imageResultsSaveDir = "";
        var args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "-testResults")
            {
                imageResultsSaveDir = args[i + 1];
                break;
            }
        }

        imageResultsSaveDir = Path.Combine(Path.GetDirectoryName(imageResultsSaveDir), "ResultsImages");

        SetupGraphicsTestCases.Setup(imageResultsPath: imageResultsSaveDir);
    }
}