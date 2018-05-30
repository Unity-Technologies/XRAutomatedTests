using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.XR;
using UnityEngine.XR.WSA.Input;

[Ignore("This is a one off test that is not setup")]
internal class LifeCycleTests : WindowsMrTestBase
{
    [SetUp]
    public void Setup()
    {
        InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
    }

    [TearDown]
    public void TearDown()
    {
        InteractionManager.InteractionSourceUpdated -= InteractionManager_InteractionSourceUpdated;
    }

    [UnityTest]
    public IEnumerator NoHmdInteraction()
    {
        var devicePresent = XRDevice.isPresent;
        yield return new WaitForSeconds(0.5f);

        Assert.IsFalse(devicePresent, "Headset is Present");
    }

    private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs obj)
    {
        Debug.Log("Source Updated");
    }
}
