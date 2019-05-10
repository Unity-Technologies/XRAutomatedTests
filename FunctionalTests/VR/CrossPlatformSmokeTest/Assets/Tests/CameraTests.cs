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
}
