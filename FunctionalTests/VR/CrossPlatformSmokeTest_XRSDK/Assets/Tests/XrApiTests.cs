using System.Collections;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.XR;

public class XrApiTests : XrFunctionalTestBase
{
    [Test]
    public void VerifyApplication_IsMobilePlatform()
    {
#if PLATFORM_IOS || PLATFORM_ANDROID
        Assert.IsTrue(Application.isMobilePlatform, "Exptect Application.isMobilePlatform == true, but is false ");
#else
        Assert.IsFalse(Application.isMobilePlatform, "Exptect Application.isMobilePlatform == false, but is true ");
#endif
    }

    [Test]
    public void VerifyXrSettings_IsDeviceActive()
    {
        AssertNotUsingEmulation();
        Assert.IsTrue(XRSettings.isDeviceActive, $"Expected {XRSettings.isDeviceActive} to true, but is false.");
    }

    [Test]
    public void VerifyXrSettings_LoadedDeviceName()
    {
        AssertNotUsingEmulation();
        Assert.IsNotEmpty(XRSettings.loadedDeviceName, $"Expected {XRSettings.loadedDeviceName} to be a non-empty string, but it is empty.");
    }

#if !XR_SDK

    [Test]
    public void XrApVerifyXrSettings_StereoRenderingMode()
    {
        if (Application.isEditor)
        {
            Assert.Ignore("StereoRenderMode test not valid in Editor.");
        }

        var expStereoRenderingMode = Settings.StereoRenderingMode;
        var actStereoRenderingMode = XRSettings.stereoRenderingMode.ToString();

        Assert.IsTrue(actStereoRenderingMode.Contains(expStereoRenderingMode), $"Expected StereoRenderMode to contain {expStereoRenderingMode} but it doesn't. Actual StereoRenderMode is: {actStereoRenderingMode}");
    }
#endif

}



