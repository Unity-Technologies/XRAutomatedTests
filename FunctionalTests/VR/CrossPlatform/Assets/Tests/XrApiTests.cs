using System.Collections;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.XR;

public class XrApiTests : XrFunctionalTestBase
{
    [UnityTest]
    public IEnumerator VerifyApplication_IsMobilePlatform()
    {
        if (IsMobilePlatform())
        {
            Assert.IsTrue(Application.isMobilePlatform, "SDK returned as a non mobile platform ");
        }
        else
        {
            Assert.IsFalse(Application.isMobilePlatform, "SDK returned as a mobile platform");
        }

        yield return null;
    }

    [UnityTest]
    public IEnumerator VerifyXrDevice_isPresent()
    {
        Assert.IsTrue(XRDevice.isPresent, "XR Device is not present");
        yield return null;
    }

    [UnityPlatform(exclude = new[] { RuntimePlatform.Android })]
    [UnityTest]
    public IEnumerator VerifyXRDevice_userPresence_isPresent()
    {
        var expUserPresenceState = UserPresenceState.Present;
        Assert.AreEqual(XRDevice.userPresence,expUserPresenceState, string.Format("Not mobile platform. Expected XRDevice.userPresence to be {0}, but is {1}.", expUserPresenceState, XRDevice.userPresence));

        yield return null;
    }

    [UnityTest]
    public IEnumerator VerifyXrSettings_isDeviceActive()
    {
        Assert.IsTrue(XRSettings.isDeviceActive, "XR Device is not active");
        yield return null;
    }

    [UnityTest]
    public IEnumerator VerifyXrSettings_LoadedDeviceName()
    {
        yield return null;
        Assert.AreEqual(
            Settings.EnabledXrTarget, 
            XRSettings.loadedDeviceName,
            string.Format("Expected {0}, but is {1}.", Settings.EnabledXrTarget, XRSettings.loadedDeviceName));
    }

    [UnityTest]
    public IEnumerator VerifyXrModelNotEmpty()
    {
        var model = XRDevice.model;
        Assert.IsNotEmpty(model, "Model is empty");
        yield return null;
    }

    [UnityTest]
    public IEnumerator VerifyXrDevice_NativePtr_IsNotEmpty()
    {
        var ptr = XRDevice.GetNativePtr().ToString();
        Assert.IsNotEmpty(ptr, "Native Ptr is empty");
        yield return null;
    }

    [UnityTest]
    public IEnumerator VerifyRefreshRateGreaterThan0()
    {
        var refreshRate = XRDevice.refreshRate;
        Assert.AreNotEqual(refreshRate, 0, "Refresh is 0; should be greater than 0.");
        yield return null;
    }
}



