using UnityEditor.TestTools.Graphics;
using UnityEngine.TestTools;
using com.unity.cliconfigmanager;
using System.IO;
using UnityEditor;

public class TestSetup
{
    [MenuItem("Tests/Configure Settings")]
    public static void ConfigureSettings()
    {
        new CliConfigManager().ConfigureFromCmdlineArgs();
    }
}

public class ActualImageSetup : IPrebuildSetup
{
    public void Setup()
    {
        string imageResultsSaveDir = "";
        var args = System.Environment.GetCommandLineArgs();

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

        SetupGraphicsTestCases.Setup(imageResultsPath: imageResultsSaveDir);
    }
}