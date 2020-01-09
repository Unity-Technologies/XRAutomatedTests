using UnityEditor.TestTools.Graphics;
using com.unity.cliconfigmanager;
using UnityEngine.TestTools;
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
                imageResultsSaveDir = Path.Combine(Path.GetDirectoryName(args[i + 1]), "ResultsImages");
                break;
            }
        }

        SetupGraphicsTestCases.Setup(imageResultsPath: imageResultsSaveDir);
    }
}

public static class Setup
{
    
}
