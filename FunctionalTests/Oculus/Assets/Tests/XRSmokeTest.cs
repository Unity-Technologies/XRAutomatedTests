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

    [UnityTest]
    public IEnumerator CanBuildAndRun()
    {
        yield return null;
        
        Assert.IsTrue(XRSettings.enabled);
        Assert.AreEqual(settings.enabledXrTarget, XRSettings.loadedDeviceName, $"Loaded Device = {XRSettings.loadedDeviceName}");
    }

    [UnityTest]
    public IEnumerator CanDisableXR()
    {
        
        yield return null;
        
        XRSettings.enabled = false;
        
        yield return null;
        
        Assert.IsFalse(XRSettings.enabled);
        Assert.AreEqual(settings.enabledXrTarget, XRSettings.loadedDeviceName, $"Loaded Device = {XRSettings.loadedDeviceName}");
    }
    
    [UnityTest]
    public IEnumerator CanDisableAndEnableXR()
    {
        yield return null;
        
        XRSettings.LoadDeviceByName("");
        yield return null;
        
        XRSettings.enabled = false;
        
        yield return null;
        
        
        Assert.IsFalse(XRSettings.enabled);
        Assert.AreEqual("", XRSettings.loadedDeviceName, $"Loaded Device = {XRSettings.loadedDeviceName}");
        
        XRSettings.LoadDeviceByName(settings.enabledXrTarget);

        yield return null;

        XRSettings.enabled = true;

        yield return null;
        
        Assert.IsTrue(XRSettings.enabled);
        Assert.AreEqual(settings.enabledXrTarget, XRSettings.loadedDeviceName, $"Loaded Device = {XRSettings.loadedDeviceName}");

    }

    [TearDown]
    public void CleanUp()
    {
        if (XRSettings.loadedDeviceName != settings.enabledXrTarget)
        {
            XRSettings.LoadDeviceByName(settings.enabledXrTarget);
        }

        XRSettings.enabled = true;
    }
}