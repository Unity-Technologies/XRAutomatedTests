using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;
using Assert = UnityEngine.Assertions.Assert;

[UnityPlatform(include = new[] { RuntimePlatform.Android })]
public class GearVRSmokeTest
{
    [UnityTest]
    [PrebuildSetup("EnableOculusPrebuildStep")]
    public IEnumerator CanDeployAndRunGearVrAppOnAndroid()
    {
        yield return new MonoBehaviourTest<GearVrMonoBehaviourTest>();
    }
}

public class GearVrMonoBehaviourTest : MonoBehaviour, IMonoBehaviourTest
{
    public bool IsTestFinished { get; private set; }

    void Awake()
    {
        Assert.IsTrue(XRSettings.enabled);
        Assert.AreEqual("Oculus", XRSettings.loadedDeviceName);
        IsTestFinished = true;
    }
}
