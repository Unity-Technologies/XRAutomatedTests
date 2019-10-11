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
    
    [Ignore("Not working in XR SDK.")]
    [Test]
    public void VerifyXrDevice_IsPresent()
    {
        AssertNotUsingEmulation();
        Assert.IsTrue(XRDevice.isPresent, "XR Device is not present");
    }

    [Ignore("Not working in XR SDK.")]
    [UnityPlatform(exclude = new[] { RuntimePlatform.Android, RuntimePlatform.IPhonePlayer })]
    [Test]
    public void VerifyXRDevice_userPresence_isPresent()
    {
        var expUserPresenceState = UserPresenceState.Present;
        var mockHmd = "MockHMD";

        if (Settings.EnabledXrTarget == mockHmd || Application.isEditor)
        {
            var reasonString = Settings.EnabledXrTarget == mockHmd ? $"EnabledXrTarget == {mockHmd}" : "Test is running in the Editor";

            Assert.Ignore("{0}: UserPresenceState.Present will always be false. Ignoring", reasonString);
        }
        else
        {
            Assert.AreEqual(XRDevice.userPresence, expUserPresenceState, string.Format("Not mobile platform. Expected XRDevice.userPresence to be {0}, but is {1}.", expUserPresenceState, XRDevice.userPresence));
        }
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

    [Ignore("Not working in XR SDK.")]
    [Test]
    public void VerifyXrModelNotEmpty()
    {
        AssertNotUsingEmulation();
        Assert.IsNotEmpty(XRDevice.model, $"Expected {XRDevice.model} to be a non-empty string, but it is empty.");
    }

    [Test]
    public void VerifyXrDevice_NativePtr_IsNotEmpty()
    {
        var ptr = XRDevice.GetNativePtr().ToString();
        Assert.IsNotEmpty(ptr, "Native Ptr is empty");
    }

    [Ignore("Not working in XR SDK.")]
    [Test]
    public void VerifyRefreshRateGreaterThan0()
    {
        AssertNotUsingEmulation();
        Assert.True(XRDevice.refreshRate > 0, "Expected XRDevice.refreshRate > 0, but is {0}", XRDevice.refreshRate);
    }

    [Test]
    public void VerifyXrSettings_EyeTextureHeight_GreaterThan0()
    {
        Assert.IsTrue(XRSettings.eyeTextureHeight > 0f);
    }

    [Test]
    public void VerifyXrSettings_EyeTextureWidth_GreaterThan0()
    {
        Assert.IsTrue(XRSettings.eyeTextureWidth > 0f);
    }

    [Test]
    public void VerifyXrSettings_EyeTextureResolutionScale_GreaterThan0()
    {
        Assert.IsTrue(XRSettings.eyeTextureResolutionScale > 0f);
    }

    [Test]
    public void VerifyXrSettings_RenderViewportScale_GreaterThan0()
    {
        Assert.IsTrue(XRSettings.renderViewportScale > 0f);
    }

    [Ignore("Not working in XR SDK.")]
    [Test]
    public void VerifyXrSettings_UseOcclusionMesh()
    {
        Assert.IsTrue(XRSettings.useOcclusionMesh);
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


