using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Graphics;
using UnityEngine.XR;

public class GraphicsTests
{
    bool check = true;

    [UnityTest]
    [PrebuildSetup("TestSetup")]
    [UseGraphicsTestCases]
    public IEnumerator Test1(GraphicsTestCase testCase)
    {
        var asyncLoad = SceneManager.LoadSceneAsync(testCase.ScenePath);

        yield return new WaitUntil(() => asyncLoad.isDone);

        // this pause is here on the first scene load to mitigate an Oculus issue
        // where screenshots would be black for the first couple scenes without it. Bug 1154476.
        if (check)
        {
            yield return new WaitForSeconds(1);
            check = false;
        }
        
        XRDevice.DisableAutoXRCameraTracking(Camera.main, true);

        var testSettings = GameObject.FindObjectOfType<GraphicsTestSettings>();

        Assert.IsNotNull(testSettings, "No test settings script found, not a valid test");

        Screen.SetResolution(testSettings.ImageComparisonSettings.TargetWidth, testSettings.ImageComparisonSettings.TargetHeight, false);

        // need a frame to let the resolution change.
        yield return null;
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

    protected IEnumerator SkipFrame(int frames)
    {
        Debug.Log(string.Format("Skipping {0} frames.", frames));

        for (int f = 0; f < frames; f++)
        {
            yield return null;
        }
    }
}
