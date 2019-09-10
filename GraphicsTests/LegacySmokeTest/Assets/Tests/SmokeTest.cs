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
    [UnityTest]
    [PrebuildSetup("TestSetup")]
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

        try
        {
            ImageAssert.AreEqual(testCase.ReferenceImage, screenShot, testSettings.ImageComparisonSettings);
        }
        catch (AssertionException)
        {
            // test setup sets the results images directory to the testResults/ResultImages directory
            var testName = TestContext.CurrentContext.Test.Name;
            var actualImageName = "./ResultsImages/" + testName + ".png";
            var diffImageName = "./ResultsImages/" + testName + ".diff.png";
            var expectedImageName = "./ResultsImages/" + testName + ".expected.png";

            TestContext.CurrentContext.Test.Properties.Set("DiffImage", diffImageName);
            TestContext.CurrentContext.Test.Properties.Set("Image", actualImageName);
            TestContext.CurrentContext.Test.Properties.Set("ExpectedImage", expectedImageName);

            throw;
        }
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