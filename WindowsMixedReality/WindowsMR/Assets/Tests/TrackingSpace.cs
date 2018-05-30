using System.Collections;
using UnityEngine.Experimental.XR;
using UnityEngine;
using UnityEngine.XR;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.XR.WSA;

//[Ignore("Metro/wsa is disabled on Katana")]
internal class TrackingSpace : WindowsMrTestBase
{
    [Test]
    public void GetCurrentTrackingSpace()
    {
        var trackingSpace = XRDevice.GetTrackingSpaceType();

        Assert.AreEqual(TrackingSpaceType.Stationary, trackingSpace, "Tracking space is not Room Scale");
    }

    [Ignore("Not Supported by current Automation Setup")]
    [UnityTest]
    public IEnumerator SwitchTrackingModes()
    {
        TrackingSpaceType trackingSpaceResult = new TrackingSpaceType();

        if (WorldManager.state != PositionalLocatorState.Inhibited)
        {
            XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
            yield return null;
            trackingSpaceResult = XRDevice.GetTrackingSpaceType();
            yield return null;
            Assert.AreEqual(TrackingSpaceType.RoomScale, XRDevice.GetTrackingSpaceType(),"Tracking space failed to switch to Room Scale");
        }

        yield return null;

        XRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);
        yield return null;
        trackingSpaceResult = XRDevice.GetTrackingSpaceType();
        yield return null;
        Assert.AreEqual(TrackingSpaceType.Stationary, trackingSpaceResult, "Tracking space failed to switch to Stationary");
    }

    [Ignore("This API is not hooked up yet")]
    [Test]
    public void BoundaryConfigured()
    {
        Assert.IsTrue(Boundary.configured, "Boundary is not configured");
    }
}
