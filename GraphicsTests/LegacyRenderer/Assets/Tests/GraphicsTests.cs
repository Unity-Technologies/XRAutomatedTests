using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Graphics;
using UnityEngine.XR;

public class GraphicsTests
{
    private string imageResultsPath;

    [OneTimeSetUp()]
    public void CreateResultsDirectoryAsset()
    {
        // this asset should be created in the prebuild setup, the value comes from a cmdline parameter
        imageResultsPath = Resources.Load<TextAsset>("ResultsImagesDirectory")?.text;
        if (imageResultsPath == null)
            imageResultsPath = string.Empty;

        // clean out any old screenshots
        foreach (var png in Directory.EnumerateFiles(Application.persistentDataPath, "*.png"))
            File.Delete(png);
    }

    [UnityTest]
    [UseGraphicsTestCases]
    public IEnumerator Test1(GraphicsTestCase testCase)
    {
        SceneManager.LoadScene(testCase.ScenePath);

        yield return null;
        
        XRDevice.DisableAutoXRCameraTracking(Camera.main, true);

        var testSettings = GameObject.FindObjectOfType<GraphicsTestSettings>();

        Assert.IsNotNull(testSettings, "No test settings script found, not a valid test");

        Screen.SetResolution(testSettings.ImageComparisonSettings.TargetWidth, testSettings.ImageComparisonSettings.TargetHeight, false);

        yield return SkipFrame(30);
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
