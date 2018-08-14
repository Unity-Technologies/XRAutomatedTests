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

[UnityPlatform(include = new[]
{
    RuntimePlatform.WindowsEditor,
    RuntimePlatform.WindowsPlayer,
    RuntimePlatform.Android,
    RuntimePlatform.IPhonePlayer,
    RuntimePlatform.OSXEditor,
    RuntimePlatform.OSXPlayer,
    RuntimePlatform.Lumin,
    RuntimePlatform.WSAPlayerARM,
    RuntimePlatform.WSAPlayerX64,
    RuntimePlatform.WSAPlayerX86
})]
[PrebuildSetup("EnablePlatformPrebuildStep")]
public class XRSmokeTest
{
    private CurrentSettings settings;

    [OneTimeSetUp]
    public void Setup()
    {
        settings = Resources.Load<CurrentSettings>("settings");
        
    }

    [UnitySetUp]
    public IEnumerator SetUpAndEnableXR()
    {
        if (XRSettings.loadedDeviceName != settings.enabledXrTarget)
        {
            XRSettings.LoadDeviceByName(settings.enabledXrTarget);
        }

        yield return null;

        XRSettings.enabled = true;
    }

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