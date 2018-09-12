using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.XR;
using Assert = UnityEngine.Assertions.Assert;

public class XRSmokeTest : TestBaseSetup
{
    [UnityTest]
    public IEnumerator CanBuildAndRun()
    {
        yield return null;
        
        Assert.IsTrue(XRSettings.enabled);
        Assert.AreEqual(settings.enabledXrTarget, XRSettings.loadedDeviceName, $"Loaded Device = {XRSettings.loadedDeviceName}");
    }

    [Test]
    public void XrApiCheck()
    {
        Assert.IsTrue(XRSettings.eyeTextureHeight > 0f);
        Assert.IsTrue(XRSettings.eyeTextureWidth > 0f);
        Assert.IsTrue(XRSettings.eyeTextureResolutionScale > 0f);
        Assert.IsTrue(XRSettings.renderViewportScale > 0f);
        Assert.IsTrue(XRSettings.useOcclusionMesh);
        Assert.IsTrue(XRSettings.stereoRenderingMode.ToString().Contains(settings.stereoRenderingMode.ToString()), $"{XRSettings.stereoRenderingMode} != {settings.stereoRenderingMode}");
    }
    
    [UnityTest]
    [Ignore("Inconsistent results for test.")]
    public IEnumerator CanDisableAndEnableXR()
    {
        yield return new MonoBehaviourTest<SwapXREnabled>();
    }
}