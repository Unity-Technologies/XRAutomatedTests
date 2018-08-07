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

        XRSettings.enabled = false;
        XRSettings.LoadDeviceByName("None");
    }

    [UnityTest]
    [PrebuildSetup("EnablePlatformPrebuildStep")]
    [UnityPlatform(include = new[]
    {
        RuntimePlatform.WindowsEditor,
        RuntimePlatform.WindowsPlayer,
        RuntimePlatform.Android
    })]
    public IEnumerator CanBuildAndRunOculus()
    {
        yield return null;
        
        XRSettings.LoadDeviceByName("Oculus");

        yield return null;

        Debug.Log($"Loaded Device = {XRSettings.loadedDeviceName}");
        XRSettings.enabled = true;

        yield return null;

        Assert.IsTrue(XRSettings.enabled);
        Assert.AreEqual("Oculus", XRSettings.loadedDeviceName, $"Loaded Device = {XRSettings.loadedDeviceName}");
    }

    [UnityTest]
    [PrebuildSetup("EnablePlatformPrebuildStep")]
    [UnityPlatform(include = new[]
    {
        RuntimePlatform.Android,
        RuntimePlatform.IPhonePlayer
    })]
    public IEnumerator CanBuildAndRunCardboard()
    {
        yield return null;
        
        XRSettings.LoadDeviceByName("cardboard");

        yield return null;

        Debug.Log($"Loaded Device = {XRSettings.loadedDeviceName}");
        XRSettings.enabled = true;

        yield return null;

        Assert.IsTrue(XRSettings.enabled);
        Assert.AreEqual("cardboard", XRSettings.loadedDeviceName, $"Loaded Device = {XRSettings.loadedDeviceName}");
    }

    [UnityTest]
    [PrebuildSetup("EnablePlatformPrebuildStep")]
    [UnityPlatform(include = new[]
    {
        RuntimePlatform.IPhonePlayer,
        RuntimePlatform.Android
    })]
    public IEnumerator CanBuildAndRunDaydream()
    {
        yield return null;

        XRSettings.LoadDeviceByName("daydream");

        yield return null;
        
        Debug.Log($"Loaded Device = {XRSettings.loadedDeviceName}");

        XRSettings.enabled = true;

        yield return null;

        Assert.IsTrue(XRSettings.enabled);
        Assert.AreEqual("daydream", XRSettings.loadedDeviceName, $"Loaded Device = {XRSettings.loadedDeviceName}");
    }

    [UnityTest]
    [PrebuildSetup("EnablePlatformPrebuildStep")]
    [UnityPlatform(include = new[]
    {
        RuntimePlatform.WindowsEditor,
        RuntimePlatform.WindowsPlayer,
        RuntimePlatform.OSXEditor,
        RuntimePlatform.OSXPlayer,
    })]
    public IEnumerator CanBuildAndRunOpenVR()
    {
        yield return null;
        
        XRSettings.LoadDeviceByName("OpenVR");

        yield return null;

        Debug.Log($"Loaded Device = {XRSettings.loadedDeviceName}");
        XRSettings.enabled = true;

        yield return null;

        Assert.IsTrue(XRSettings.enabled);
        Assert.AreEqual("OpenVR", XRSettings.loadedDeviceName, $"Loaded Device = {XRSettings.loadedDeviceName}");
    }

    [UnityTest]
    [PrebuildSetup("EnablePlatformPrebuildStep")]
    [UnityPlatform(include = new[]
    {
        RuntimePlatform.WSAPlayerX64,
        RuntimePlatform.WSAPlayerX86
    })]
    public IEnumerator CanBuildAndRunWindowsMR()
    {
        yield return null;
        
        XRSettings.LoadDeviceByName("Windows Mixed Reality");

        yield return null;

        Debug.Log($"Loaded Device = {XRSettings.loadedDeviceName}");
        XRSettings.enabled = true;

        yield return null;

        Assert.IsTrue(XRSettings.enabled);
        Assert.AreEqual("Windows Mixed Reality", XRSettings.loadedDeviceName, $"Loaded Device = {XRSettings.loadedDeviceName}");
    }
}