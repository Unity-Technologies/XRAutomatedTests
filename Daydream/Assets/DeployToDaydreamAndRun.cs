using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;
using Assert = UnityEngine.Assertions.Assert;

[UnityPlatform(include = new[] { RuntimePlatform.Android })]
public class DaydreamSmokeTest : EnableVRPrebuildStep
{
    [UnityTest]
    [Explicit] // Added to ensure this is only run against devices that support Daydream
               // Requires that --testFilter=DaydreamSmokeTest parameter is used to run.
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
