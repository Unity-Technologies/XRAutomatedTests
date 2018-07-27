using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;
using Assert = UnityEngine.Assertions.Assert;

public class OculusSmokeTest
{
    [UnityTest]
    [PrebuildSetup("EnableOculusPrebuildStep")]
    public IEnumerator CanBuildAndRunForOculus()
    {
        yield return new MonoBehaviourTest<OculusMonoBehaviourTest>();
    }
}

public class OculusMonoBehaviourTest : MonoBehaviour, IMonoBehaviourTest
{
    public bool IsTestFinished { get; private set; }

    void Awake()
    {
        Assert.IsTrue(XRSettings.enabled);
        Assert.AreEqual("Oculus", XRSettings.loadedDeviceName);
        IsTestFinished = true;
    }
}
