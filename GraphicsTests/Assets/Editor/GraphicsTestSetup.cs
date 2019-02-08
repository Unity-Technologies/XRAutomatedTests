using UnityEngine.TestTools;
using NUnit.Framework;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text;

public class GraphicsTestSetup : IPrebuildSetup
{
    public void Setup()
    {
        new EnablePlatformPrebuildStep().Setup();
        new UnityEditor.TestTools.Graphics.SetupGraphicsTestCases().Setup();
    }
}

public class GraphicsTestCleanup : IPostBuildCleanup
{
    public void Cleanup()
    {
        var args = System.Environment.GetCommandLineArgs();

        string testResultsPath = string.Empty;

        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "-testResults")
                testResultsPath = args[i + 1];
        }

        // default test results location is project root directory
        if (testResultsPath == string.Empty)
            testResultsPath = "./";

        var imagesDir = Path.GetDirectoryName(testResultsPath);
        imagesDir += "/ResultsImages";


        imagesDir = Path.GetFullPath(imagesDir);

        if (Directory.Exists(imagesDir))
            Directory.Delete(imagesDir, true);

        Directory.CreateDirectory(imagesDir);

        if(PlatformSettings.BuildTargetGroup == BuildTargetGroup.Android)
        {
            // should find a better way to get this path
            string fullPath = "/storage/self/primary/Android/data/" + PlayerSettings.applicationIdentifier + "/files/";

            var proc = new System.Diagnostics.Process();
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.FileName = "adb.exe";
            proc.StartInfo.Arguments = "pull " + fullPath + " " + imagesDir;
            proc.Start();

            var output = new StringBuilder();

            while (!proc.HasExited)
            {
                output.Append(proc.StandardOutput.ReadToEnd());
            }

            Debug.Log(output);

            // whole dir is copied since pull doesn't take wildcards so do this cleanup.
            var files = Directory.GetFiles(imagesDir + "/files", "*.png");

            foreach (var moveFile in files)
            {
                File.Move(moveFile, imagesDir + "/" + Path.GetFileName(moveFile));
            }

            Directory.Delete(imagesDir + "/files", true);
        }
        else if(PlatformSettings.BuildTargetGroup == BuildTargetGroup.Standalone)
        {
            var files = Directory.GetFiles(Application.persistentDataPath, "*.png");

            foreach (var moveFile in files)
            {
                File.Move(moveFile, imagesDir + "/" + Path.GetFileName(moveFile));
            }
        }

        if (Directory.GetFiles(imagesDir).Length == 0)
            Directory.Delete(imagesDir);
    }
}
