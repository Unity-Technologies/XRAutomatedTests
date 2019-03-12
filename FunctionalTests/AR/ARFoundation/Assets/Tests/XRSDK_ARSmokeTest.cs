using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

[UnityPlatform(include = new[] { RuntimePlatform.Android, RuntimePlatform.IPhonePlayer})]
public class XRSDK_ARSmokeTest
{
    [SetUp]
    public void SetupTest()
    {
        SceneManager.LoadScene("ARScene");
    }

    [UnityTest]
    public IEnumerator XRSubsystemsActivation()
    {
        // Determine if correct scene has been loaded
        Assert.That(SceneManager.GetActiveScene().name == "ARScene");

        // Check for AR Session and the AR Session component
        GameObject arSession = GameObject.Find("ARSession");

        Assert.IsNotNull(arSession);

        ARSession arSessionComponent = arSession.GetComponent<ARSession>();

        Assert.IsNotNull(arSessionComponent);

        // Check for the AR Rig which controls the origin of the AR scene and the camera
        GameObject arRig = GameObject.Find("ARRig");

        Assert.IsNotNull(arRig);

        ARSessionOrigin arOriginComponent = arRig.GetComponent<ARSessionOrigin>();

        Assert.IsNotNull(arOriginComponent);

        // Wait up to 120 frames for ARSession state to change from Initializing to Running
        int framesWaited = 0;
        while (ARSubsystemManager.sessionSubsystem.TrackingState != UnityEngine.Experimental.XR.TrackingState.Tracking && framesWaited < 240)
        {
            framesWaited++;
            yield return null;
        }

        Assert.That(ARSubsystemManager.sessionSubsystem.TrackingState == UnityEngine.Experimental.XR.TrackingState.Tracking, "Session State: {0}", ARSubsystemManager.sessionSubsystem.TrackingState);

        // Once the ARSession is running, the AR Background Renderer should become active and display the camera feed on the screen
        ARCameraBackground backgroundRenderer = arRig.GetComponentInChildren<ARCameraBackground>();

        Assert.That(backgroundRenderer.enabled == true, "ARBackground Renderer Enabled: {0}", backgroundRenderer.enabled);
    }
}
