using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;
using Assert = UnityEngine.Assertions.Assert;

[UnityPlatform(include = new[] { RuntimePlatform.Android, RuntimePlatform.IPhonePlayer})]
public class CardboardSmokeTest : EnableVRPrebuildStep
{
    [UnityTest]
    public IEnumerator CanDeployAndRunCardboardApp()
    {
        yield return new MonoBehaviourTest<CardboardMonoBehaviourTest>();
    }
}

public class CardboardMonoBehaviourTest : MonoBehaviour, IMonoBehaviourTest
{
    public bool IsTestFinished { get; private set; }

    void Awake()
    {
        Assert.IsTrue(XRSettings.enabled);
        Assert.AreEqual("cardboard", XRSettings.loadedDeviceName);
        IsTestFinished = true;
    }
}
