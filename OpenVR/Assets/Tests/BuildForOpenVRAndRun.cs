using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR;
using Assert = UnityEngine.Assertions.Assert;

//[UnityPlatform(include = new[] { RuntimePlatform.WindowsEditor, RuntimePlatform.WindowsPlayer })]
public class OpenVRSmokeTest
{
    [UnityTest]
    [PrebuildSetup("EnableOpenVRPrebuildStep")]
    public IEnumerator CanBuildAndRunForOpenVR()
    {
        yield return new MonoBehaviourTest<OpenVRMonoBehaviourTest>();
    }
}

public class OpenVRMonoBehaviourTest : MonoBehaviour, IMonoBehaviourTest
{
    public bool IsTestFinished { get; private set; }

    void Awake()
    {
        Assert.IsTrue(XRSettings.enabled);
        Assert.AreEqual("OpenVR", XRSettings.loadedDeviceName);
        IsTestFinished = true;
    }
}
