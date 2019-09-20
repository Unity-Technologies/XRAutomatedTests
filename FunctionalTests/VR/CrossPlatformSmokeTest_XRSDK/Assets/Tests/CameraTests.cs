using UnityEngine;
using UnityEngine.XR;
using NUnit.Framework;

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
        TestCubeSetup(TestCubesConfig.TestCube);
    }

    [TearDown]
    public override void TearDown()
    {
        XRSettings.eyeTextureResolutionScale = 1f;
        XRDevice.fovZoomFactor = startingZoomAmount;
        XRSettings.renderViewportScale = 1f;

        base.TearDown();
    }

    [Test]
    public void VerifyXRSettings_EyeTextureResolutionScale()
    {
        Assert.IsTrue(XRSettings.eyeTextureResolutionScale > 0);
    }
}
