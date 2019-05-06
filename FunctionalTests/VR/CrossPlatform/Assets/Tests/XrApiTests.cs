using System.Collections;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.XR;

public class XrApiTests : XrFunctionalTestBase
{
    [UnityPlatform(include = new[] { RuntimePlatform.Android, RuntimePlatform.IPhonePlayer })]
    [Test]
    public void VerifyApplication_IsMobilePlatform()
    {
        Assert.IsTrue(Application.isMobilePlatform, "SDK returned as a non mobile platform ");
    }

    [Test]
    public void VerifyXrDevice_IsPresent()
    {
        Assert.IsTrue(XRDevice.isPresent, "XR Device is not present");
    }

    [UnityPlatform(exclude = new[] { RuntimePlatform.Android, RuntimePlatform.IPhonePlayer })]
    [Test]
    public void VerifyXRDevice_userPresence_isPresent()
    {
        if (Settings.EnabledXrTarget != "MockHMD")
        {
            var expUserPresenceState = UserPresenceState.Present;
            Assert.AreEqual(XRDevice.userPresence, expUserPresenceState, string.Format("Not mobile platform. Expected XRDevice.userPresence to be {0}, but is {1}.", expUserPresenceState, XRDevice.userPresence));
        }
    }

    [Test]
    public void VerifyXrSettings_IsDeviceActive()
    {
        Assert.IsTrue(XRSettings.isDeviceActive, "XR Device is not active");
    }

    [Test]
    public void VerifyXrSettings_LoadedDeviceName()
    {
        Assert.AreEqual(
            Settings.EnabledXrTarget, 
            XRSettings.loadedDeviceName,
            string.Format("Expected {0}, but is {1}.", Settings.EnabledXrTarget, XRSettings.loadedDeviceName));
    }

    [Test]
    public void VerifyXrModelNotEmpty()
    {
        Assert.IsNotEmpty(XRDevice.model, "Model is empty");
    }

    [Test]
    public void VerifyXrDevice_NativePtr_IsNotEmpty()
    {
        var ptr = XRDevice.GetNativePtr().ToString();
        Assert.IsNotEmpty(ptr, "Native Ptr is empty");
    }

    [Test]
    public void VerifyRefreshRateGreaterThan0()
    {
        Assert.AreNotEqual(XRDevice.refreshRate, 0, "Refresh is 0; should be greater than 0.");
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

    [Test]
    public void VerifyXrSettings_UseOcclusionMesh()
    {
        Assert.IsTrue(XRSettings.useOcclusionMesh);
    }
#if !UNITY_EDITOR
    [Test]
    public void XrApVerifyXrSettings_StereoRenderingMode()
    {

        Assert.IsTrue(XRSettings.stereoRenderingMode.ToString().Contains(Settings.StereoRenderingMode.ToString()), $"{XRSettings.stereoRenderingMode} != {Settings.StereoRenderingMode}");
    }
#endif

    [UnityTest]
    [Ignore("Inconsistent results for test. For example, this doesn't work on GearVR.")]
    public IEnumerator CanDisableAndEnableXr()
    {
        yield return new MonoBehaviourTest<SwapXrEnabled>();
    }
}



