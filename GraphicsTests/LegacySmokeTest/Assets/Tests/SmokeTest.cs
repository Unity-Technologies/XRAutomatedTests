using System;
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Graphics;

public class SmokeTest
{
    private string imageResultsPath;

    [OneTimeSetUp()]
    public void CreateResultsDirectoryAsset()
    {
        // this asset should be created in the prebuild setup, the value comes from the -testResults cmdline parameter
        imageResultsPath = Resources.Load<TextAsset>("ResultsImagesDirectory")?.text;
        if (imageResultsPath == null)
        {
            imageResultsPath = string.Empty;
        }

        // clean out any old screenshots
        foreach (var png in Directory.EnumerateFiles(Application.persistentDataPath, "*.png"))
        {
            try
            {
                File.Delete(png);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception thrown while attempting to delete file {0}: {1}", png, e);
            }
        }

        var pngFiles = Directory.EnumerateFiles(Application.persistentDataPath, "*.png");
        if (Directory.EnumerateFiles(Application.persistentDataPath, "*.png").Any())
        {
            foreach (var pngFile in pngFiles)
            {
                Console.WriteLine("Failed to delete png file {0}", pngFile);
            }

            throw new Exception("Failed to complete cleanup of png files in test setup.");
        }
    }

    [UnityTest]
    [UseGraphicsTestCases]
    public IEnumerator TestAllScenes(GraphicsTestCase testCase)
    {
        SceneManager.LoadScene(testCase.ScenePath);

        yield return SkipFrame(1);

        var testSettings = GameObject.FindObjectOfType<GraphicsTestSettings>();

        Assert.IsNotNull(testSettings, "No test settings script found, not a valid test");

        Screen.SetResolution(testSettings.ImageComparisonSettings.TargetWidth, testSettings.ImageComparisonSettings.TargetHeight, false);

        // need some time to update the resolution.  Oculus desktop seems to need over a second for some reason.
        yield return SkipFrame(100);
        yield return new WaitForEndOfFrame();

        var screenShot = new Texture2D(0, 0, TextureFormat.RGBA32, false);

        screenShot = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.BothEyes);

        ImageAssert.AreEqual(testCase.ReferenceImage, screenShot, testSettings.ImageComparisonSettings, imageResultsPath);
    }

    protected IEnumerator SkipFrame(int frames)
    {
        Debug.Log(("Skipping {0} frames.", frames));

        for (int f = 0; f < frames; f++)
        {
            yield return null;
        }
    }
}