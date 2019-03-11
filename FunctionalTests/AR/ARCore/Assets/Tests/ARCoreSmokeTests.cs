using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using GoogleARCore;

[UnityPlatform(include = new[] { RuntimePlatform.Android })]
public class ARCoreSmokeTests : EnableARPrebuildStep
{
    [SetUp]
    public void SetupTest()
    {
        SceneManager.LoadScene("SmokeTest");
    }

    [UnityTest]
    public IEnumerator SessionExistsAndIsConnected()
    {
        // Make sure we're in the correct scene
        Assert.That(SceneManager.GetActiveScene().name == "SmokeTest");

        var device = GameObject.Find("ARCore Device");

        // Make sure the AR Core device exists in the scene
        Assert.IsNotNull(device);

        var session = device.GetComponent<GoogleARCore.ARCoreSession>();

        //Make sure the session component exists on the device
        Assert.IsNotNull(session);

        // Wait a frame so we can give the AR session a chance to connect.
        int framesWaited = 0;
        while (Session.Status != SessionStatus.Tracking && framesWaited < 360)
        {
            framesWaited++;
            yield return null;
        }

        // Check to see that the session connected successfully
        Assert.That(Session.Status == SessionStatus.Tracking, "Connection State: {0}", Session.Status);
    }
}
