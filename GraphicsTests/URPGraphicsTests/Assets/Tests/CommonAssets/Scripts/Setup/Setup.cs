using com.unity.cliconfigmanager;
using System.IO;
using UnityEditor.TestTools.Graphics;
using UnityEngine.TestTools;

public class TestSetup : IPrebuildSetup
{
    public void Setup()
    {
        string imageResultsSaveDir = "";
        var args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "-testResults")
            {
                imageResultsSaveDir = Path.Combine(Path.GetDirectoryName(args[i + 1]), "ResultsImages");
                break;
            }
        }

        SetupGraphicsTestCases.Setup(imageResultsPath: imageResultsSaveDir);
    }
}

public static class Setup
{
    public static void SetupAll()
    {
        new CliConfigManager().ConfigureFromCmdlineArgs();
    }
}