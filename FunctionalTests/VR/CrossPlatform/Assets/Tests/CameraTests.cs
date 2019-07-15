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

    [UnityTest]
    public IEnumerator VerifyRefreshRate()
    {
        yield return SkipFrame(DefaultFrameSkipCount);

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
        yield return SkipFrame(DefaultFrameSkipCount);
        var tolerance = .005;

        // Arrange
        var expRenderViewPortScale = 1f;
        // Act
        XRSettings.renderViewportScale = expRenderViewPortScale;
        // Assert
        var actRenderViewPortScale = XRSettings.renderViewportScale;
        Assert.AreEqual(
            expRenderViewPortScale, 
            actRenderViewPortScale,
            tolerance,
            string.Format("Expected XRSettings.renderViewPortScale to {0}, but is {1}", expRenderViewPortScale, actRenderViewPortScale));

        // Arrange
        expRenderViewPortScale = 0.7f;
        // Act
        XRSettings.renderViewportScale = expRenderViewPortScale;
        // Assert
        actRenderViewPortScale = XRSettings.renderViewportScale;
        Assert.AreEqual(
            expRenderViewPortScale, 
            actRenderViewPortScale, 
            tolerance,
            string.Format("Expected XRSettings.renderViewPortScale to {0}, but is {1}", expRenderViewPortScale, actRenderViewPortScale));

        // Arrange
        expRenderViewPortScale = 0.5f;
        // Act
        XRSettings.renderViewportScale = expRenderViewPortScale;
        // Assert
        actRenderViewPortScale = XRSettings.renderViewportScale;
        Assert.AreEqual(
            expRenderViewPortScale, 
            actRenderViewPortScale, 
            tolerance,
            string.Format("Expected XRSettings.renderViewPortScale to {0}, but is {1}", expRenderViewPortScale, actRenderViewPortScale));
    }


    [UnityTest]
    public IEnumerator VerifyAdjustEyeTextureResolutionScale()
    {
        yield return SkipFrame(DefaultFrameSkipCount);

        var scale = 0.1f;
        var scaleIncrement = 0.1f;
        var scaleLimit = 2f;

        do 
        {
            
            scale = scale + scaleIncrement;

            XRSettings.eyeTextureResolutionScale = scale;

            yield return SkipFrame(DefaultFrameSkipCount);

            Debug.Log("VerifyAdjustEyeTextureResolutionScale = " + scale);
            Assert.AreEqual(scale, XRSettings.eyeTextureResolutionScale, "Eye texture resolution scale is not being respected");
        }
        while (scale < scaleLimit) ;
    }

    [UnityTest]
    public IEnumerator VerifyAdjustDeviceZoom()
    {
        yield return SkipFrame(DefaultFrameSkipCount);

        var zoomAmount = 0f;
        var zoomCount = 0f;

        for (var i = 0; i < 2; i++)
        {
            zoomAmount = zoomAmount + 1f;
            zoomCount = zoomCount + 1f;

            XRDevice.fovZoomFactor = zoomAmount;

            yield return SkipFrame(DefaultFrameSkipCount);

            Debug.Log("fovZoomFactor = " + zoomAmount);
            Assert.AreEqual(zoomCount, XRDevice.fovZoomFactor, "Zoom Factor is not being respected");
        }
    }

    [UnityTest]
    public IEnumerator TakeScreenShot()
    {
        yield return SkipFrame(2);

        try
        {
            if (IsMobilePlatform())
            {
                var cam = XrFunctionalTestHelpers.Camera;
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
