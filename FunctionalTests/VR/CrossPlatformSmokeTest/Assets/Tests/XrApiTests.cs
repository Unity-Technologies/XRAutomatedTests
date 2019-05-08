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

#if !UNITY_EDITOR
    [Test]
    public void XrApVerifyXrSettings_StereoRenderingMode()
    {

        Assert.IsTrue(XRSettings.stereoRenderingMode.ToString().Contains(Settings.StereoRenderingMode.ToString()), $"{XRSettings.stereoRenderingMode} != {Settings.StereoRenderingMode}");
    }
#endif
}



