using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Graphics;
using UnityEngine.XR;

public class SmokeTest
{
    [UnityTest]
    [PrebuildSetup("TestSetup")]
    [UseGraphicsTestCases]
    public IEnumerator Test1(GraphicsTestCase testCase)
    {
        SceneManager.LoadScene(testCase.ScenePath);

        yield return null;

        var testSettings = GameObject.FindObjectOfType<GraphicsTestSettings>();

        Assert.IsNotNull(testSettings, "No test settings script found, not a valid test");

        Screen.SetResolution(testSettings.ImageComparisonSettings.TargetWidth, testSettings.ImageComparisonSettings.TargetHeight, false);

        yield return new WaitForSeconds(1);
        yield return new WaitForEndOfFrame();

        var screenShot = new Texture2D(0, 0, TextureFormat.RGBA32, false);

        screenShot = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.BothEyes);

        try
        {
            ImageAssert.AreEqual(testCase.ReferenceImage, screenShot, testSettings.ImageComparisonSettings);
        }
        catch (AssertionException e)
        {
            // test setup sets the results images directory to the testResults/ResultImages directory
            var testName = TestContext.CurrentContext.Test.Name;
            var actualImageName = "./ResultsImages/" + testName + ".png";
            TestContext.CurrentContext.Test.Properties.Set("Image", actualImageName);

            // If the exception says there was a null reference image then there isn't a diff or expected images
            if (!e.Message.Contains("But was:  null"))
            {
                var diffImageName = "./ResultsImages/" + testName + ".diff.png";
                TestContext.CurrentContext.Test.Properties.Set("DiffImage", diffImageName);

                var expectedImageName = "./ResultsImages/" + testName + ".expected.png";
                TestContext.CurrentContext.Test.Properties.Set("ExpectedImage", expectedImageName);
            }

            throw;
        }
    }
}