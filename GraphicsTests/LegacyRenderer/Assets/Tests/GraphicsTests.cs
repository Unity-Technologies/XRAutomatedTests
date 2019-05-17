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
    bool check = true;

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
        
        ImageAssert.AreEqual(testCase.ReferenceImage, screenShot, testSettings.ImageComparisonSettings, imageResultsPath);
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
