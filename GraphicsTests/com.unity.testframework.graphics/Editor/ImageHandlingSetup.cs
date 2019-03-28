using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text;

public static class TestConstants
{
    // default test results location is project root directory
    public static string TestResultsPath = "./";
    public static string ResultsImagesPath = "./ResultsImages";

    public static string FullResultsImagesPath { get => Path.Combine(TestResultsPath, ResultsImagesPath);}
}

public static class ImageHandlingSetup
{
    public static void Setup()
    {
        var args = System.Environment.GetCommandLineArgs();

        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "-testResults")
            {
                TestConstants.TestResultsPath = Path.GetDirectoryName(args[i + 1]);
            }
        }

        // this is created to save image location in the test results at runtime so the test reporter creator can find them
        var imageDirectoryAsset = new TextAsset(TestConstants.ResultsImagesPath);
        AssetDatabase.CreateAsset(imageDirectoryAsset, "Assets/Resources/ResultsImagesDirectory.asset");

        if (Directory.Exists(TestConstants.FullResultsImagesPath))
        {
            foreach (var png in Directory.EnumerateFiles(TestConstants.FullResultsImagesPath, "*.png"))
            {
                File.Delete(png);
            }
        }
        else
        {
            Directory.CreateDirectory(TestConstants.FullResultsImagesPath);
        }

        EditorApplication.quitting += CopyImages;
    }

    private static void CopyImages()
    {
        if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            // should find a better way to get this path
            string fullPath = "/storage/self/primary/Android/data/" + PlayerSettings.applicationIdentifier + "/files/";

            var proc = new System.Diagnostics.Process();
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.FileName = "adb.exe";
            proc.StartInfo.Arguments = "pull " + fullPath + " " + TestConstants.FullResultsImagesPath;
            proc.Start();

            var output = new StringBuilder();

            while (!proc.HasExited)
            {
                output.Append(proc.StandardOutput.ReadToEnd());
            }

            Debug.Log(output);

            // whole dir is copied since pull doesn't take wildcards so do this cleanup.
            var files = Directory.GetFiles(TestConstants.FullResultsImagesPath + "/files", "*.png");

            foreach (var moveFile in files)
            {
                File.Move(moveFile, TestConstants.FullResultsImagesPath + "/" + Path.GetFileName(moveFile));
            }

            Directory.Delete(TestConstants.FullResultsImagesPath + "/files", true);
        }
        else if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.Standalone)
        {
            var files = Directory.GetFiles(Application.persistentDataPath, "*.png");

            foreach (var moveFile in files)
            {
                File.Move(moveFile, TestConstants.FullResultsImagesPath + "/" + Path.GetFileName(moveFile));
            }
        }

        if (Directory.GetFiles(TestConstants.FullResultsImagesPath).Length == 0)
            Directory.Delete(TestConstants.FullResultsImagesPath);
    }
}
