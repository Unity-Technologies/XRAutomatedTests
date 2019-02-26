using UnityEngine.TestTools;
using NUnit.Framework;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text;
using System;

public static class TestConstants
{
    // default test results location is project root directory
    public static string TestResultsPath = "./";
    public static string ResultsImagesPath = "./ResultsImages";
    public static string TestResultsFile = "TestResults.xml";
}

public class GraphicsTestSetup : IPrebuildSetup
{
    public void Setup()
    {
        var args = System.Environment.GetCommandLineArgs();

        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "-testResults")
            {
                TestConstants.TestResultsFile = Path.GetFileName(args[i + 1]);
                TestConstants.TestResultsPath = Path.GetDirectoryName(args[i + 1]);
            }
                

            if (args[i] == "-imagesResultsPath")
                TestConstants.ResultsImagesPath = args[i + 1];
        }

        TestConstants.ResultsImagesPath = Path.Combine(TestConstants.TestResultsPath, TestConstants.ResultsImagesPath);

        // this is created to save image location in the test results at runtime so the test reporter creator can find them
        var imageDirectoryAsset = new TextAsset(TestConstants.ResultsImagesPath);
        AssetDatabase.CreateAsset(imageDirectoryAsset, "Assets/Resources/ResultsImagesDirectory.asset");

        if (Directory.Exists(TestConstants.ResultsImagesPath))
        {
            foreach (var png in Directory.EnumerateFiles(TestConstants.ResultsImagesPath, "*.png"))
            {
                File.Delete(png);
                File.Delete(TestConstants.TestResultsFile);
            }
        }
        else
        {
            Directory.CreateDirectory(TestConstants.ResultsImagesPath);
        }

        new EnablePlatformPrebuildStep().Setup();
        new UnityEditor.TestTools.Graphics.SetupGraphicsTestCases().Setup();

        Directory.Delete(Application.persistentDataPath, true);

        EditorApplication.quitting += CopyImages;
    }

    private void CopyImages()
    {

        if (PlatformSettings.BuildTargetGroup == BuildTargetGroup.Android)
        {
            // should find a better way to get this path
            string fullPath = "/storage/self/primary/Android/data/" + PlayerSettings.applicationIdentifier + "/files/";

            var proc = new System.Diagnostics.Process();
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.FileName = "adb.exe";
            proc.StartInfo.Arguments = "pull " + fullPath + " " + TestConstants.ResultsImagesPath;
            proc.Start();

            var output = new StringBuilder();

            while (!proc.HasExited)
            {
                output.Append(proc.StandardOutput.ReadToEnd());
            }

            Debug.Log(output);

            // whole dir is copied since pull doesn't take wildcards so do this cleanup.
            var files = Directory.GetFiles(TestConstants.ResultsImagesPath + "/files", "*.png");

            foreach (var moveFile in files)
            {
                File.Move(moveFile, TestConstants.ResultsImagesPath + "/" + Path.GetFileName(moveFile));
            }

            Directory.Delete(TestConstants.ResultsImagesPath + "/files", true);
        }
        else if (PlatformSettings.BuildTargetGroup == BuildTargetGroup.Standalone)
        {
            var files = Directory.GetFiles(Application.persistentDataPath, "*.png");

            foreach (var moveFile in files)
            {
                File.Move(moveFile, TestConstants.ResultsImagesPath + "/" + Path.GetFileName(moveFile));
            }
        }

        if (Directory.GetFiles(TestConstants.ResultsImagesPath).Length == 0)
            Directory.Delete(TestConstants.ResultsImagesPath);
    }
}
