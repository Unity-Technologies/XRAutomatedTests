using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.XR;
using UnityEngine.XR.WSA;
using System;

public class XrApiCheck : TestBaseSetup
{
    [Test]
    public void MobilePlatformCheck()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Assert.IsTrue(Application.isMobilePlatform, "SDK returned as a non mobile platform ");
        }
        else
        {
            Assert.IsFalse(Application.isMobilePlatform, "SDK returned as a mobile platform");
        }
    }

    [Test]
    public void XrPresentCheck()
    {
        Assert.IsTrue(XRDevice.isPresent, "XR Device is not present");
    }

    [Test]
    public void UserPresenceCheck()
    {
        if (XRDevice.userPresence == UserPresenceState.Present)
        {
            Assert.AreEqual(UserPresenceState.Present, XRDevice.userPresence, "User Presence reported reported unexpected value");
        }

        if (XRDevice.userPresence == UserPresenceState.NotPresent)
        {
            Assert.AreEqual(UserPresenceState.NotPresent, XRDevice.userPresence, "User Presence reported reported unexpected value");
        }
    }

    [Test]
    public void XrSettingsCheck()
    {
        Assert.IsTrue(XRSettings.isDeviceActive, "XR Device is not active");
    }

    [Test]
    public void DeviceCheck()
    {
        Assert.AreEqual(settings.enabledXrTarget, XRSettings.loadedDeviceName, "Wrong XR Device reported");
    }

    [Test]
    public void XrModel()
    {
        string model = XRDevice.model;
        Assert.IsNotEmpty(model, "Model is empty");
    }

    [Test]
    public void NativePtr()
    {
        string ptr = XRDevice.GetNativePtr().ToString();
        Assert.IsNotEmpty(ptr, "Native Ptr is empty");
    }

    [Test]
    public void RefreshRate()
    {
        float refreshRate = XRDevice.refreshRate;
        Assert.AreNotEqual(refreshRate, 0, "Refresh is 0, something went wrong");
    }
}

[UnityPlatform(include = new[]
{
    RuntimePlatform.WindowsEditor,
    RuntimePlatform.WindowsPlayer,
    RuntimePlatform.WSAPlayerARM,
    RuntimePlatform.WSAPlayerX64,
    RuntimePlatform.WSAPlayerX86
})]
public class XRApiWmrCheck : TestBaseSetup
{
    [UnityTest]
    public IEnumerator ContentProtectionTest()
    {
        WmrDeviceCheck();
        HolographicSettings.IsContentProtectionEnabled = true;
        yield return new WaitForSeconds(1f);
        Assert.IsTrue(HolographicSettings.IsContentProtectionEnabled,
            "Content Protection was not set to true to protect the user!");

        HolographicSettings.IsContentProtectionEnabled = false;
        yield return new WaitForSeconds(1f);
        Assert.IsFalse(HolographicSettings.IsContentProtectionEnabled,
            "Content Protection was not set to false");
    }

    [UnityTest]
    public IEnumerator ReprojectionModeTest()
    {
        WmrDeviceCheck();
        foreach (HolographicSettings.HolographicReprojectionMode mode in Enum.GetValues(typeof(HolographicSettings.HolographicReprojectionMode)))
        {
            HolographicSettings.ReprojectionMode = mode;
            Debug.Log("Re-projection Mode = " + mode);
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(HolographicSettings.ReprojectionMode, mode, "Re-projection mode failed to change to target!");
        }
    }

    [UnityTest]
    public IEnumerator DisplayOpaqueTest()
    {
        WmrDeviceCheck();
        yield return null;
        Assert.IsFalse(HolographicSettings.IsDisplayOpaque, "Display came back as Opaque!");
    }

    [UnityTest]
    public IEnumerator FocusFrameTest()
    {
        WmrDeviceCheck();
        if (m_Cube != null)
        {
            GameObject.Destroy(m_Cube);
        }

        GameObject focusPoint = new GameObject("Focus Point");
        focusPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        var cam = GameObject.Find("Camera");
        HolographicSettings.SetFocusPointForFrame(new Vector3(0, 0, 0), cam.GetComponent<Camera>().transform.position.normalized);
        focusPoint.transform.localPosition = new Vector3(0, 0, 0);

        Assert.AreEqual(new Vector3(0, 0, 0), focusPoint.transform.localPosition);

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 10; i++)
        {
            HolographicSettings.SetFocusPointForFrame(new Vector3(0, 0, i), cam.GetComponent<Camera>().transform.position.normalized);
            Debug.Log("Current Focus Frame Z Position:" + i);

            yield return new WaitForSeconds(0.5f);

            focusPoint.transform.localPosition = new Vector3(0, 0, i);

            Assert.AreEqual(new Vector3(0, 0, i), focusPoint.transform.localPosition);
        }

        for (int i = 10; i > 0; i--)
        {
            HolographicSettings.SetFocusPointForFrame(new Vector3(0, 0, i), cam.GetComponent<Camera>().transform.position.normalized);
            Debug.Log("Current Focus Frame Z Position:" + i);

            yield return new WaitForSeconds(0.5f);

            focusPoint.transform.localPosition = new Vector3(0, 0, i);

            Assert.AreEqual(new Vector3(0, 0, i), focusPoint.transform.localPosition);
        }

        GameObject.Destroy(focusPoint);
    }

    [Test]
    public void SpatialCoordinateSystemPtr()
    {
        string ptr = WorldManager.GetNativeISpatialCoordinateSystemPtr().ToString();
        Assert.IsNotEmpty(ptr, "Spatial Coordinate is empty");
    }

    [Test]
    public void UserPresenceCheck()
    {
        if (XRDevice.userPresence == UserPresenceState.Present)
        {
            Assert.AreEqual(UserPresenceState.Present, XRDevice.userPresence, "User Presence reported reported unexpected value");
        }

        if (XRDevice.userPresence == UserPresenceState.NotPresent)
        {
            Assert.AreEqual(UserPresenceState.NotPresent, XRDevice.userPresence, "User Presence reported reported unexpected value");
        }
    }

    [Test]
    public void WorldManagerStateCheck()
    {
        if (WorldManager.state == PositionalLocatorState.Active)
        {
            Assert.AreEqual(PositionalLocatorState.Active, WorldManager.state, "Expecting World Manager state to be Active");
        }

        if (WorldManager.state == PositionalLocatorState.Inhibited)
        {
            Assert.AreEqual(PositionalLocatorState.Inhibited, WorldManager.state, "Expecting World Manager state to be Inhibited");
        }

        if (WorldManager.state == PositionalLocatorState.OrientationOnly)
        {
            Assert.AreEqual(PositionalLocatorState.OrientationOnly, WorldManager.state, "Expecting World Manager state to be OrientationOnly");
        }
    }
}

