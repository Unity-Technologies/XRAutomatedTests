using System.Collections;
using UnityEngine.Experimental.XR;
using UnityEngine;
using UnityEngine.XR;
using NUnit.Framework;
using UnityEngine.TestTools;

public class TrackingSpace : TestBaseSetup
{
    [UnityTest]
    public IEnumerator GetCurrentTrackingSpace()
    {
        yield return new WaitForSeconds(2f);
        var trackingSpace = XRDevice.GetTrackingSpaceType();

        Assert.IsNotNull(trackingSpace, "Tracking space is not reading correctly");
    }

    [Ignore("Not Supported by current Automation Setup")]
    [UnityTest]
    public IEnumerator SwitchTrackingModes()
    {
        TrackingSpaceType trackingSpaceResult = new TrackingSpaceType();

        if (XRDevice.GetTrackingSpaceType() != TrackingSpaceType.RoomScale)
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
        yield return null;
    }

    [Ignore("This API is not hooked up yet")]
    [Test]
    public void BoundaryConfigured()
    {
        Assert.IsTrue(Boundary.configured, "Boundary is not configured");
    }
}
