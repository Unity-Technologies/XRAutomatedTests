using UnityEngine;
using UnityEngine.XR;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System;
using System.IO;

public class CameraTests : XrFunctionalTestBase
{
    private string fileName;

    private float startingZoomAmount;
    private Texture2D mobileTexture;
    
    // TODO can we find a way to move this out of the test class?
    void Start()
    {
        startingZoomAmount = XRDevice.fovZoomFactor;
    }

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        XrFunctionalTestHelpers.TestCubeSetup(TestCubesConfig.TestCube);
    }

    [TearDown]
    public override void TearDown()
    {
        XRSettings.eyeTextureResolutionScale = 1f;
        XRDevice.fovZoomFactor = startingZoomAmount;
        XRSettings.renderViewportScale = 1f;

        base.TearDown();
    }

#if UNITY_EDITOR
    // TODO Add bug number
    [Ignore("Known bug")]
    [UnityTest]
    public IEnumerator CameraCheckForMultiPass()
    {
        yield return SkipFrame(OneSecOfFramesWaitTime);

        XrFunctionalTestHelpers.TestStageSetup(TestStageConfig.MultiPass);
        Assert.AreEqual(XRSettings.stereoRenderingMode, UnityEditor.PlayerSettings.stereoRenderingPath, "Expected StereoRenderingPath to be Multi pass");
    }

    // TODO Add bug number
    [Ignore("Known bug")]
    [UnityTest]
    public IEnumerator CameraCheckForInstancing()
    {
        yield return SkipFrame(OneSecOfFramesWaitTime);

        XrFunctionalTestHelpers.TestStageSetup(TestStageConfig.Instancing);
        Assert.AreEqual(XRSettings.stereoRenderingMode, UnityEditor.PlayerSettings.stereoRenderingPath, "Expected StereoRenderingPath to be Instancing");
    }
#endif

    [UnityTest]
    public IEnumerator VerifyRefreshRate()
    {
        yield return SkipFrame(OneSecOfFramesWaitTime);

        var refreshRate = XRDevice.refreshRate;
        if (IsMobilePlatform())
        {
            Assert.GreaterOrEqual(refreshRate, 60, "Refresh rate returned to lower than expected");
        } else
        {
            Assert.GreaterOrEqual(refreshRate, 89, "Refresh rate returned to lower than expected");
        }
    }

    [UnityTest]
    public IEnumerator VerifyAdjustRenderViewportScale()
    {
        yield return SkipFrame(OneSecOfFramesWaitTime);

        XRSettings.renderViewportScale = 1f;
        Assert.AreEqual(1f, XRSettings.renderViewportScale, "Render viewport scale is not being respected");

        XRSettings.renderViewportScale = 0.7f;
        Assert.AreEqual(0.7f, XRSettings.renderViewportScale, "Render viewport scale is not being respected");

        XRSettings.renderViewportScale = 0.5f;
        Assert.AreEqual(0.5f, XRSettings.renderViewportScale, "Render viewport scale is not being respected");
    }


    [UnityTest]
    public IEnumerator VerifyAdjustEyeTextureResolutionScale()
    {
        yield return SkipFrame(OneSecOfFramesWaitTime);

        var scale = 0.1f;
        var scaleCount = 0.1f;

        for (var i = 0.1f; i < 2; i++)
        {
            scale = scale + 0.1f;
            scaleCount = scaleCount + 0.1f;

            XRSettings.eyeTextureResolutionScale = scale;

            yield return null;

            Debug.Log("VerifyAdjustEyeTextureResolutionScale = " + scale);
            Assert.AreEqual(scaleCount, XRSettings.eyeTextureResolutionScale, "Eye texture resolution scale is not being respected");
        }
    }

    [UnityTest]
    public IEnumerator VerifyAdjustDeviceZoom()
    {
        yield return SkipFrame(OneSecOfFramesWaitTime);

        var zoomAmount = 0f;
        var zoomCount = 0f;

        for (var i = 0; i < 2; i++)
        {
            zoomAmount = zoomAmount + 1f;
            zoomCount = zoomCount + 1f;

            XRDevice.fovZoomFactor = zoomAmount;

            yield return null;

            Debug.Log("fovZoomFactor = " + zoomAmount);
            Assert.AreEqual(zoomCount, XRDevice.fovZoomFactor, "Zoom Factor is not being respected");
        }
    }

    // TODO Add check for existance of screenshot file and ensure it's not 0 bytes or something
    [UnityTest]
    public IEnumerator TakeScreenShot()
    {
        yield return SkipFrame(OneSecOfFramesWaitTime);

        try
        {
            if (IsMobilePlatform())
            {
                var cam = GameObject.Find("Camera");
                var width = cam.GetComponent<Camera>().scaledPixelWidth;
                var height = cam.GetComponent<Camera>().scaledPixelHeight;

                mobileTexture  = new Texture2D(width, height, TextureFormat.RGBA32, false);
                mobileTexture = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.BothEyes);
            }
            else
            {
                fileName = Application.temporaryCachePath + "/ScreenShotTest.jpg";
                ScreenCapture.CaptureScreenshot(fileName, ScreenCapture.StereoScreenCaptureMode.BothEyes);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Failed to get capture! : " + e);
            Assert.Fail("Failed to get capture! : " + e);
        }

        if (IsMobilePlatform())
        {
            yield return SkipFrame(5);

            if (IsMobilePlatform())
            {
                Assert.IsNotNull(mobileTexture, "Texture data is empty for mobile");
            }
            else
            {
                var tex = new Texture2D(2, 2);

                var texData = File.ReadAllBytes(fileName);
                Debug.Log("Screen Shot Success!" + Environment.NewLine + "File Name = " + fileName);

                tex.LoadImage(texData);

                Assert.IsNotNull(tex, "Texture Data is empty");
            }
        }
    }
}
