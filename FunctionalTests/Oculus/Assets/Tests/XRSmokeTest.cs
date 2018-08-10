using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
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
public class XRSmokeTest
{
    private CurrentSettings settings;

    [SetUp]
    public void Setup()
    {
        settings = Resources.Load<CurrentSettings>("settings");
    }

    [UnityTest]
    [PrebuildSetup("EnablePlatformPrebuildStep")]
    [UnityPlatform(include = new[]
    {
        RuntimePlatform.WindowsEditor,
        RuntimePlatform.WindowsPlayer,
        RuntimePlatform.Android,
        RuntimePlatform.IPhonePlayer,
        RuntimePlatform.WSAPlayerX64,
        RuntimePlatform.WSAPlayerX86,
        RuntimePlatform.OSXEditor,
        RuntimePlatform.OSXPlayer
    })]
    public IEnumerator CanBuildAndRun()
    {
        yield return null;
        
        Assert.IsTrue(XRSettings.enabled);
        Assert.AreEqual(settings.enabledXrTarget, XRSettings.loadedDeviceName, $"Loaded Device = {XRSettings.loadedDeviceName}");
    }
}