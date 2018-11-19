using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Threading;

#if UNITY_METRO
using UnityEngine.XR.WSA;
#endif

internal class ProjectionMode : TestBaseSetup
{
#if UNITY_METRO
    [TearDown]
    public void TearDown()
    {

        HolographicSettings.IsContentProtectionEnabled = false;
        HolographicSettings.ReprojectionMode = HolographicSettings.HolographicReprojectionMode.Disabled;

    }

    [UnityTest]
    public IEnumerator ReprojectionModeDisabled()
    {
        WmrDeviceCheck();
        var disable = HolographicSettings.HolographicReprojectionMode.Disabled;
        var expectedReprojectionMode = HolographicSettings.HolographicReprojectionMode.Disabled;

        HolographicSettings.ReprojectionMode = disable;

        yield return new WaitForSeconds(1f);

        Assert.AreEqual(expectedReprojectionMode, disable, "Re-projection mode was not set");
    }

    [UnityTest]
    public IEnumerator ReprojectionModeOrientationOnly()
    {
        WmrDeviceCheck();
        var orientationOnly = HolographicSettings.HolographicReprojectionMode.OrientationOnly;
        var expectedReprojectionMode = HolographicSettings.HolographicReprojectionMode.OrientationOnly;

        HolographicSettings.ReprojectionMode = orientationOnly;

        yield return new WaitForSeconds(1f);

        Assert.AreEqual(expectedReprojectionMode, orientationOnly, "Re-projection mode was not set");
    }

    [UnityTest]
    public IEnumerator ReprojectionModePositionAndOrientation()
    {
        WmrDeviceCheck();
        var PositonAndOrientation = HolographicSettings.HolographicReprojectionMode.PositionAndOrientation;
        var expectedReprojectionMode = HolographicSettings.HolographicReprojectionMode.PositionAndOrientation;

        HolographicSettings.ReprojectionMode = PositonAndOrientation;

        yield return new WaitForSeconds(1f);

        Assert.AreEqual(expectedReprojectionMode, PositonAndOrientation, "Re-projection mode was not set");
    }

    [UnityTest]
    public IEnumerator DisableContentProtection()
    {
        WmrDeviceCheck();
        HolographicSettings.IsContentProtectionEnabled = false;
        
        yield return null;

        Assert.IsFalse(HolographicSettings.IsContentProtectionEnabled, "Content Protection is enabled");
    }

    [UnityTest]
    public IEnumerator EnableContentProtection()
    {
        WmrDeviceCheck();
        HolographicSettings.IsContentProtectionEnabled = true;

        yield return null;

        Assert.IsTrue(HolographicSettings.IsContentProtectionEnabled, "Content Protection is disabled");
    }
#endif
}
