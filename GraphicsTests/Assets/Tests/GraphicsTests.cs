using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Graphics;
using UnityEngine.XR;
using UnityEditor;


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
    }

    [UnityTest]
    [PrebuildSetup("GraphicsTestSetup")]
    [PostBuildCleanup("GraphicsTestCleanup")]
    [UseGraphicsTestCases]
    public IEnumerator Test1(GraphicsTestCase testCase)
    {
        SceneManager.LoadScene(testCase.ScenePath);

        yield return null;
        
        XRDevice.DisableAutoXRCameraTracking(Camera.main, true);

        var testSettings = GameObject.FindObjectOfType<GraphicsTestSettings>();

        Assert.IsNotNull(testSettings, "No test settings script found, not a valid test");

        Screen.SetResolution(testSettings.ImageComparisonSettings.TargetWidth, testSettings.ImageComparisonSettings.TargetHeight, false);

        yield return null;
        yield return new WaitForEndOfFrame();

        var screenShot = new Texture2D(0, 0, TextureFormat.RGBA32, false);

        screenShot = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.BothEyes);
        
        ImageAssert.AreEqual(testCase.ReferenceImage, screenShot, testSettings.ImageComparisonSettings, imageResultsPath);
    }
}
