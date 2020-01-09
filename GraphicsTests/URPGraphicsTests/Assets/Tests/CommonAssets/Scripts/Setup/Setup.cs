using com.unity.cliconfigmanager;
using UnityEditor.TestTools.Graphics;
using UnityEngine.TestTools;
using System.IO;

public class TestSetup
{
    public static void Setup()
    {
        new CliConfigManager().ConfigureFromCmdlineArgs();
    }
}

public class GraphicsSetup : IPrebuildSetup
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