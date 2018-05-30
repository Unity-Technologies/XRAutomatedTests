using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.WSA;

//[Ignore("Metro/wsa is disabled on Katana")]
internal class HoloLensApiCheck : HoloLensTestBase
{
    Vector3 m_FocalPlanePosition;

    [Test]
    public void WorldManagerStateCheck()
    {
        Debug.Log("Current World Manager State : " + WorldManager.state);
        Assert.GreaterOrEqual(PositionalLocatorState.OrientationOnly, WorldManager.state, " World state is not available");
    }

    [Test]
    public void MobilePlatformCheck()
    {
        Assert.AreEqual(false, Application.isMobilePlatform, "Is not a mobile platform while in simulation!");
    }

    [Test]
    public void XrPresentCheck()
    {
        Assert.GreaterOrEqual(UserPresenceState.Present, XRDevice.userPresence, "XR Device came back as Unsupported");
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
    public void RefreshRate()
    {
        float refreshRate = XRDevice.refreshRate;
        Assert.AreNotEqual(refreshRate, 0, "Refresh is 0, something went wrong");
    }

    [Test]
    public void SpatialCoordinateSystemPtr()
    {
        string ptr = WorldManager.GetNativeISpatialCoordinateSystemPtr().ToString();
        Assert.IsNotEmpty(ptr, "Spatial Coordinate is empty");
    }
}
