using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;
using Assert = UnityEngine.Assertions.Assert;

public class DaydreamSmokeTest : EnableVRPrebuildStep
{
    [UnityTest]
    public IEnumerator CanDeployAndRunDaydreamAppOnAndroid()
    {
        yield return new MonoBehaviourTest<DaydreamMonoBehaviourTest>();
    }
}

public class DaydreamMonoBehaviourTest : MonoBehaviour, IMonoBehaviourTest
{
    public bool IsTestFinished { get; private set; }

    void Awake()
    {
        Assert.IsTrue(XRSettings.enabled);
        Assert.AreEqual("daydream", XRSettings.loadedDeviceName);
        IsTestFinished = true;
    }
}
