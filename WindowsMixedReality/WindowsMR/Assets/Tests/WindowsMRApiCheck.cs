using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.WSA;
using NUnit.Framework;

//[Ignore("Metro/wsa is disabled on Katana")]
internal class WindowsMRApiCheck : WindowsMrTestBase
{
    Vector3 m_FocalPlanePosition;

    [Test]
    public void WorldManagerStateCheck()
    {
        if(WorldManager.state == PositionalLocatorState.Active)
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

    [Test]
    public void MobilePlatformCheck()
    {
        Assert.IsFalse(Application.isMobilePlatform, "Is not a mobile platform while in play mode!");
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
        Assert.AreEqual("WindowsMR", XRSettings.loadedDeviceName, "Wrong XR Device reported");
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
    public void SpatialCoordinateSystemPtr()
    {
        string ptr = WorldManager.GetNativeISpatialCoordinateSystemPtr().ToString();
        Assert.IsNotEmpty(ptr, "Spatial Coordinate is empty");
    }

    [Test]
    public void RefreshRate()
    {
        float refreshRate = XRDevice.refreshRate;
        Assert.AreNotEqual(refreshRate, 0, "Refresh is 0, something went wrong");
    }
}
